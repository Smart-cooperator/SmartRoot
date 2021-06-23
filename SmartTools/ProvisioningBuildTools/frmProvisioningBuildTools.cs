using ProvisioningBuildTools.SelectForm;
using ProvisioningBuildTools.SelectInput;
using ProvisioningBuildTools.SelectOutput;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProvisioningBuildTools;
using System.Configuration;
using System.Diagnostics;
using ProvisioningBuildTools.Global;
using ProvisioningBuildTools.CLI;

namespace ProvisioningBuildTools
{
    public partial class frmProvisioningBuildTools : Form, ICommandNotify, ILogNotify, IEnhancelogNotify, ICLIWrapperNotify
    {
        private readonly Enhancelog Log;

        private string OriginTitle;

        private string ProcessingTitle;

        private readonly BackGroundCommand backGroundCommand = new BackGroundCommand();
        private readonly CLIWrapper CLIWrapper;

        public frmProvisioningBuildTools()
        {
            InitializeComponent();

            rtbCMD.WordWrap = false;
            rtbCMD.Multiline = true;
            rtbCMD.ScrollBars = RichTextBoxScrollBars.Both;
            rtbCMD.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
            rtbCMD.Font = new Font(rtbCMD.Font.FontFamily, (float)10.25);
            //rtbCMD.BackColor = Color.Black;
            rtbCMD.BackColor = Color.FromArgb(12, 12, 12);
            rtbCMD.ForeColor = Color.FromArgb(204, 204, 204);
            rtbCMD.SelectionColor = Color.FromArgb(204, 204, 204);
            rtbCMD.Font = new Font("Consolas", rtbCMD.Font.Size);
            rtbCMD.ReadOnly = true;

            CLIWrapper = new CLIWrapper(this, this);

            Log = new Enhancelog(rtbCMD, this);
            Log.Start();

            Application.ThreadException += Application_ThreadException;
        }

        private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            WriteLog(e.Exception);
        }

        private static bool IsRunAsAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(id);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public void WriteOutPut(int processId, string output)
        {
            Log.Append(output);
        }

        public void WriteErrorOutPut(int processId, string error)
        {
            Log.Append(error, true);
        }

        public void WriteLineOutPut(int processId, string outputLine)
        {
            Log.AppendLine(outputLine);
        }

        public void WriteLineErrorOutPut(int processId, string errorLine)
        {
            Log.AppendLine(errorLine, true);
        }

        public void Exit(int processId, int exitCode)
        {
            Log.AppendLine($"ExitCode {exitCode}");
            CLIWrapper.CurrentProcess = null;
        }

        public void Start(Process process)
        {
            if (CLIWrapper.CurrentMode == CLIMode.Processing)
            {
                CLIWrapper.CurrentProcess = process;
            }
        }

        public void WriteLog(string logLine, bool hasError = false)
        {
            Log.AppendLine(logLine, !hasError ? Color.LightGreen : Color.Red);
        }

        public void WriteLog(string logLine, Color color)
        {
            Log.AppendLine(logLine, color);
        }

        public void WriteLog(Exception ex)
        {
            if (ex is OperationCanceledException)
            {
                Log.AppendLine($"error message:Current command has been killed or canceled", true);
            }
            else
            {
                //Log.AppendLine($"error message:{ex.Message}{Environment.NewLine}stack trace:{Environment.NewLine}{ex.StackTrace}", true);
                Log.AppendLine($"error message:{ex.Message}", true);
            }
        }

        private void frmProvisioningBuildTools_Load(object sender, EventArgs e)
        {

            if (IsRunAsAdmin())
            {
                cmbExecItems.DropDownStyle = ComboBoxStyle.DropDownList;

                string allowedCommandList = ConfigurationManager.AppSettings["allowedCommandList"];

                if (!string.IsNullOrWhiteSpace(allowedCommandList))
                {
                    foreach (var allowedCommand in allowedCommandList.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        ExecEnum execEnum;

                        if (Enum.TryParse<ExecEnum>(allowedCommand, out execEnum))
                        {
                            cmbExecItems.Items.Add(allowedCommand);
                        }
                    }
                }
                else
                {
                    cmbExecItems.Items.AddRange(Enum.GetNames(typeof(ExecEnum)));
                }

                if (cmbExecItems.Items.Count > 0)
                {
                    cmbExecItems.SelectedIndex = 0;
                }

                this.btnAbort.Enabled = false;
                this.btnKill.Enabled = false;

                this.Text = $"Administrator: {this.Text}";

                OriginTitle = this.Text;

                btnClear.PerformClick();
            }
            else
            {
                this.Enabled = false;

                try
                {
                    ProcessStartInfo processStartInfo = new ProcessStartInfo(Application.ExecutablePath);
                    processStartInfo.Verb = "RunAs";
                    Process.Start(processStartInfo);
                }
                catch (Exception)
                {

                }

                Environment.Exit(-1);
                //this.BeginInvoke(new Action(() => { MessageBox.Show("Pls run as Administrator", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning); this.Close(); }));
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                string text = this.Text;
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                string context = $"{text} [Version {version}]{Environment.NewLine}Author: v-fengzhou@microsoft.com{Environment.NewLine}";
                Log.Clear();
                btnClear.Enabled = false;
                Log.AppendLine(context, Color.LightGreen);
                ShowNextLine();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                this.Enabled = false;
            }
        }

        private void ShowNextLine(string project = null)
        {
            GlobalValue.Root.SelectedProject = project ?? GlobalValue.Root.SelectedProject;

            string[] projects = new SelectLocalProjectInput(this).LocalBranches ?? Enumerable.Empty<string>().ToArray();

            string selectedProject = GlobalValue.Root.SelectedProject ?? string.Empty;

            if (!projects.Contains(selectedProject, StringComparer.InvariantCultureIgnoreCase))
            {
                selectedProject = null;
            }

            GlobalValue.Root.SelectedProject = selectedProject;

            project = GlobalValue.Root.SelectedProject ?? "NULL";
            Log.Append($"{project}>");
            this.rtbCMD.Focus();
        }

        private void btnExec_Click(object sender, EventArgs e)
        {
            AbCLIExecInstance cliInstance = btnExec.Tag as AbCLIExecInstance;

            btnClear.Enabled = true;
            btnClear.PerformClick();

            Log.AppendLine(cmbExecItems.SelectedItem.ToString());

            ExecEnum execEnum;
            Form selectFrom = null;
            List<Func<CommandResult>> runAct = null;
            Action endInvoke = new Action(() => EnableRun(true));
            Action startInvoke = new Action(() => EnableRun(false));
            SelectLocalProjectOutput selectLocalBranchOutput;
            SelectRemoteProjectOutput selectRemoteBranchOutput;
            SelectPackagesInfoOutput selectPackagesInfoOutput;
            SelectRemoteProjectOutputPostBuild selectRemoteBranchOutputPostBuild;
            SelectRemoteProjectOutputArtifact selectRemoteProjectOutputArtifact;
            SelectProvisioningTesterInfoOutput selectProvisioningTesterInfoOutput;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationTokenSource cancellationTokenSourceForKill = new CancellationTokenSource();
            try
            {

                if (Enum.TryParse<ExecEnum>(cmbExecItems.SelectedItem?.ToString(), out execEnum))
                {
                    cliInstance = cliInstance ?? CLIWrapper.FindInstance(execEnum.ToString());

                    if (!backGroundCommand.IsBusy)
                    {
                        switch (execEnum)
                        {
                            case ExecEnum.OpenLocalProject:
                            case ExecEnum.BuildLocalProject:
                            case ExecEnum.CreatePackage:
                            case ExecEnum.UpdateExternalDrops:
                            case ExecEnum.CapsuleParser:
                                selectFrom = new frmSelectLoaclProject(this, this, execEnum);
                                break;
                            case ExecEnum.GetRemoteProject:
                                selectFrom = new frmSelectRemoteProject(this, this);
                                break;
                            //case ExecEnum.PostBuildPackage:
                            //    selectFrom = new frmSelectRemoteProjectPostBuild(this, this);
                            //    break;
                            case ExecEnum.GetProvisioningArtifact:
                                selectFrom = new frmSelectRemoteProjectArtifact(this, this);
                                break;
                            case ExecEnum.InstallSurfacePackage:
                                selectFrom = new frmSelectPackagesInfo(this, this);
                                break;
                            case ExecEnum.UploadNugetPackage:
                                selectFrom = new frmSelectLoaclProjectForUPT(this, this);
                                break;
                            case ExecEnum.ProvisioningTester:
                                selectFrom = new frmSelectLoaclProjectForProvisioningTesterNew(this, this);
                                break;
                            default:
                                break;
                        }

                        if (Directory.Exists(Command.REPOSFOLDER))
                        {
                            Directory.SetCurrentDirectory(Command.REPOSFOLDER);
                        }

                        ((ICLISupprot)selectFrom).CLIInstance = cliInstance;

                        if (cliInstance != null && cliInstance.ParseSuccess && cliInstance.FromCLI)
                        {
                            ((ICLISupprot)selectFrom).CLIExec();
                        }

                        if (selectFrom.ShowDialog(this) == DialogResult.OK)
                        {
                            string commandLine = ((ISelect)selectFrom).CommandLine;

                            if (!string.IsNullOrEmpty(commandLine))
                            {
                                CLIWrapper.AddHistory(commandLine);
                            }

                            switch (execEnum)
                            {
                                case ExecEnum.OpenLocalProject:
                                    selectLocalBranchOutput = ((ISelect<SelectLocalProjectOutput>)selectFrom).SelectResult;

                                    ProcessingTitle = $"{execEnum} {selectLocalBranchOutput.SelectedLocalProjectInfo.Name}";

                                    if (selectLocalBranchOutput.Append)
                                    {
                                        ProcessingTitle = $"{ProcessingTitle} -Append {selectLocalBranchOutput.AppendList}";
                                    }

                                    runAct = new List<Func<CommandResult>>()
                                    {
                                        new Func<CommandResult>(() => Command.OpenReposSln(selectLocalBranchOutput.SelectedLocalProjectInfo.SourceFolder, this, this, cancellationTokenSource,cancellationTokenSourceForKill))
                                    };
                                    break;
                                case ExecEnum.BuildLocalProject:
                                    selectLocalBranchOutput = ((ISelect<SelectLocalProjectOutput>)selectFrom).SelectResult;

                                    ProcessingTitle = $"{execEnum} {selectLocalBranchOutput.SelectedLocalProjectInfo.Name}";

                                    if (selectLocalBranchOutput.Append)
                                    {
                                        ProcessingTitle = $"{ProcessingTitle} -Append {selectLocalBranchOutput.AppendList}";
                                    }

                                    runAct = new List<Func<CommandResult>>()
                                    {
                                        new Func<CommandResult>(() => Command.RebulidAll(selectLocalBranchOutput.SelectedLocalProjectInfo.SourceFolder, this, this, cancellationTokenSource,cancellationTokenSourceForKill)),
                                    };

                                    if (selectLocalBranchOutput.Append)
                                    {
                                        runAct.Add(new Func<CommandResult>(() => Command.CreatePackage(selectLocalBranchOutput.SelectedLocalProjectInfo.SourceFolder, selectLocalBranchOutput.SelectedLocalProjectInfo.ProvisioningPackageFolder, this, this, cancellationTokenSource, cancellationTokenSourceForKill)));
                                    }
                                    break;
                                case ExecEnum.CreatePackage:
                                    selectLocalBranchOutput = ((ISelect<SelectLocalProjectOutput>)selectFrom).SelectResult;

                                    ProcessingTitle = $"{execEnum} {selectLocalBranchOutput.SelectedLocalProjectInfo.Name}";

                                    if (selectLocalBranchOutput.Append)
                                    {
                                        ProcessingTitle = $"{ProcessingTitle} -Append {selectLocalBranchOutput.AppendList}";
                                    }

                                    runAct = new List<Func<CommandResult>>()
                                    {
                                        new Func<CommandResult>(() => Command.CreatePackage(selectLocalBranchOutput.SelectedLocalProjectInfo.SourceFolder, selectLocalBranchOutput.SelectedLocalProjectInfo.ProvisioningPackageFolder, this, this, cancellationTokenSource,cancellationTokenSourceForKill))
                                    };
                                    break;
                                case ExecEnum.UpdateExternalDrops:
                                    selectLocalBranchOutput = ((ISelect<SelectLocalProjectOutput>)selectFrom).SelectResult;

                                    ProcessingTitle = $"{execEnum} {selectLocalBranchOutput.SelectedLocalProjectInfo.Name}";

                                    if (selectLocalBranchOutput.Append)
                                    {
                                        ProcessingTitle = $"{ProcessingTitle} -Append {selectLocalBranchOutput.AppendList}";
                                    }

                                    runAct = new List<Func<CommandResult>>()
                                    {
                                        new Func<CommandResult>(() => Command.UpdateExternalDrops(selectLocalBranchOutput.SelectedLocalProjectInfo.SourceFolder, this, this, cancellationTokenSource,cancellationTokenSourceForKill)),
                                        };

                                    if (selectLocalBranchOutput.Append)
                                    {
                                        runAct.Add(new Func<CommandResult>(() => Command.RebulidAll(selectLocalBranchOutput.SelectedLocalProjectInfo.SourceFolder, this, this, cancellationTokenSource, cancellationTokenSourceForKill)));
                                        runAct.Add(new Func<CommandResult>(() => Command.CreatePackage(selectLocalBranchOutput.SelectedLocalProjectInfo.SourceFolder, selectLocalBranchOutput.SelectedLocalProjectInfo.ProvisioningPackageFolder, this, this, cancellationTokenSource, cancellationTokenSourceForKill)));
                                    }

                                    break;
                                case ExecEnum.GetRemoteProject:
                                    selectRemoteBranchOutput = ((ISelect<SelectRemoteProjectOutput>)selectFrom).SelectResult;

                                    ProcessingTitle = $"{execEnum} {selectRemoteBranchOutput.SelectProject} {selectRemoteBranchOutput.SelectRemoteBranch} {selectRemoteBranchOutput.Tag}";

                                    runAct = new List<Func<CommandResult>>()
                                    {
                                        new Func<CommandResult>(() => Command.CheckOutLatestBranch(selectRemoteBranchOutput.SelectProject,selectRemoteBranchOutput.NewBranchName,new Tuple<string, DateTime?, Version>(selectRemoteBranchOutput.SelectRemoteBranch,selectRemoteBranchOutput.LastModifiedTime,selectRemoteBranchOutput.Tag), this, this, cancellationTokenSource,cancellationTokenSourceForKill))
                                        };
                                    break;
                                //case ExecEnum.PostBuildPackage:
                                //    selectRemoteBranchOutputPostBuild = ((ISelect<SelectRemoteProjectOutputPostBuild>)selectFrom).SelectResult;

                                //    ProcessingTitle = $"{execEnum} {selectRemoteBranchOutputPostBuild.SelectProject} {selectRemoteBranchOutputPostBuild.SelectRemoteBranch} {selectRemoteBranchOutputPostBuild.Tag}";

                                //    runAct = new List<Func<CommandResult>>()
                                //    {
                                //        new Func<CommandResult>(() => Command.PostBuild(selectRemoteBranchOutputPostBuild.LocalBuildScriptsFolder,new Tuple<string, string, Version, Func<ICommandNotify, ILogNotify, CancellationTokenSource, CancellationTokenSource, Version>>(selectRemoteBranchOutputPostBuild.SelectProject,selectRemoteBranchOutputPostBuild.SelectRemoteBranch,selectRemoteBranchOutputPostBuild.Tag,selectRemoteBranchOutputPostBuild.WaitGetTag), this, this, cancellationTokenSource,cancellationTokenSourceForKill))
                                //        };
                                //    break;
                                case ExecEnum.GetProvisioningArtifact:
                                    selectRemoteProjectOutputArtifact = ((ISelect<SelectRemoteProjectOutputArtifact>)selectFrom).SelectResult;

                                    ProcessingTitle = $"{execEnum} {selectRemoteProjectOutputArtifact.SelectProject} {selectRemoteProjectOutputArtifact.SelectRemoteBranch} {selectRemoteProjectOutputArtifact.Tag}";

                                    runAct = new List<Func<CommandResult>>()
                                    {
                                        new Func<CommandResult>(() => Command.GetProvisioningArtifact(selectRemoteProjectOutputArtifact.ProvisioningPackageFolder,new Tuple<string, string, Version, Func<ICommandNotify, ILogNotify, CancellationTokenSource, CancellationTokenSource, Version>>(selectRemoteProjectOutputArtifact.SelectProject,selectRemoteProjectOutputArtifact.SelectRemoteBranch,selectRemoteProjectOutputArtifact.Tag,selectRemoteProjectOutputArtifact.WaitGetTag), this, this, cancellationTokenSource,cancellationTokenSourceForKill))
                                        };
                                    break;
                                case ExecEnum.InstallSurfacePackage:
                                    selectPackagesInfoOutput = ((ISelect<SelectPackagesInfoOutput>)selectFrom).SelectResult;

                                    ProcessingTitle = $"{execEnum} {selectPackagesInfoOutput.IdList}";

                                    runAct = new List<Func<CommandResult>>()
                                    {
                                        new Func<CommandResult>(() => Command.InstallSurfacePackage(selectPackagesInfoOutput.GeneratePackageConfig, this, this, cancellationTokenSource,cancellationTokenSourceForKill))
                                    };
                                    break;
                                case ExecEnum.UploadNugetPackage:
                                    selectLocalBranchOutput = ((ISelect<SelectLocalProjectOutput>)selectFrom).SelectResult;

                                    ProcessingTitle = $"{execEnum} {selectLocalBranchOutput.SelectedLocalProjectInfo.Name} {selectLocalBranchOutput.SelectedLocalProjectInfo.SelectePackagedList}";

                                    if (selectLocalBranchOutput.Append)
                                    {
                                        ProcessingTitle = $"{ProcessingTitle} -Append {selectLocalBranchOutput.AppendList}";
                                    }

                                    runAct = new List<Func<CommandResult>>()
                                    {
                                        new Func<CommandResult>(() => Command.UploadPackage2NugetMulti(selectLocalBranchOutput.SelectedLocalProjectInfo.SourceFolder,selectLocalBranchOutput.SelectedLocalProjectInfo.BuildScriptsFolder,selectLocalBranchOutput.SelectedLocalProjectInfo.GetSelectedPackagesInfo(selectLocalBranchOutput.SelectedPackagesList), this, this, cancellationTokenSource,cancellationTokenSourceForKill)),
                                        };

                                    if (selectLocalBranchOutput.Append)
                                    {
                                        runAct.Add(new Func<CommandResult>(() => Command.UpdateExternalDrops(selectLocalBranchOutput.SelectedLocalProjectInfo.SourceFolder, this, this, cancellationTokenSource, cancellationTokenSourceForKill)));
                                        runAct.Add(new Func<CommandResult>(() => Command.RebulidAll(selectLocalBranchOutput.SelectedLocalProjectInfo.SourceFolder, this, this, cancellationTokenSource, cancellationTokenSourceForKill)));
                                        runAct.Add(new Func<CommandResult>(() => Command.CreatePackage(selectLocalBranchOutput.SelectedLocalProjectInfo.SourceFolder, selectLocalBranchOutput.SelectedLocalProjectInfo.ProvisioningPackageFolder, this, this, cancellationTokenSource, cancellationTokenSourceForKill)));
                                    }
                                    break;
                                case ExecEnum.CapsuleParser:
                                    selectLocalBranchOutput = ((ISelect<SelectLocalProjectOutput>)selectFrom).SelectResult;

                                    ProcessingTitle = $"{execEnum} {selectLocalBranchOutput.SelectedLocalProjectInfo.Name}";

                                    if (selectLocalBranchOutput.Append)
                                    {
                                        ProcessingTitle = $"{ProcessingTitle} -Append {selectLocalBranchOutput.AppendList}";
                                    }

                                    runAct = new List<Func<CommandResult>>()
                                    {
                                        new Func<CommandResult>(() => Command.RunCapsuleParser(selectLocalBranchOutput.SelectedLocalProjectInfo.CapsuleFolder,selectLocalBranchOutput.SelectedLocalProjectInfo.CapsuleInfoConfigurationPaths, this, this, cancellationTokenSource,cancellationTokenSourceForKill))
                                    };
                                    break;
                                case ExecEnum.ProvisioningTester:
                                    selectProvisioningTesterInfoOutput = ((ISelect<SelectProvisioningTesterInfoOutput>)selectFrom).SelectResult;

                                    ProcessingTitle = $"{execEnum} {selectProvisioningTesterInfoOutput.SelectProject} -ProvisioningPackage {new DirectoryInfo(selectProvisioningTesterInfoOutput.SelectProvisioningPackage).Name} -SN {selectProvisioningTesterInfoOutput.SelectSerialNumber} -Slot {selectProvisioningTesterInfoOutput.SelectSlot} -Task {selectProvisioningTesterInfoOutput.SelectTaskOpList} -SKU {string.Join(",",selectProvisioningTesterInfoOutput.SelectSkuDocumentDict.Select(pair => pair.Key.Split('_').Last()))} -LoopCount {selectProvisioningTesterInfoOutput.SelectLoopCount}";

                                    runAct = new List<Func<CommandResult>>()
                                    {
                                        new Func<CommandResult>(() => Command.RunLoopTest(selectProvisioningTesterInfoOutput.SelectLoopCount,selectProvisioningTesterInfoOutput.SelectGenealogyFile,selectProvisioningTesterInfoOutput.SelectSkuDocumentDict,selectProvisioningTesterInfoOutput.UseExternalProvisioningTester,selectProvisioningTesterInfoOutput.SelectProvisioningPackage,selectProvisioningTesterInfoOutput.SelectSerialNumber,selectProvisioningTesterInfoOutput.SelectArgs, this, this, cancellationTokenSource,cancellationTokenSourceForKill)),
                                    };

                                    break;
                                default:
                                    break;
                            }

                            ProcessingTitle = commandLine ?? ProcessingTitle;

                            ShowNextLine();
                            Log.AppendLine(ProcessingTitle);

                            ProcessingTitle = $"{OriginTitle} {ProcessingTitle}";

                            backGroundCommand.AsyncRun(runAct.ToArray(), startInvoke, endInvoke, cancellationTokenSource, this, cancellationTokenSourceForKill);
                        }
                        else
                        {
                            this.TopMost = true;
                            this.Activate();
                            this.TopMost = false;
                            this.btnClear.Enabled = true;
                            ShowNextLine();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ShowNextLine();
            }
            finally
            {
                btnExec.Tag = null;
            }
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            try
            {
                if (backGroundCommand.IsBusy)
                {
                    btnAbort.Enabled = false;
                    backGroundCommand.Abort();
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex);
            }
        }

        private void btnKill_Click(object sender, EventArgs e)
        {
            try
            {
                if (backGroundCommand.IsBusy)
                {
                    btnKill.Enabled = false;
                    backGroundCommand.Kill();
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex);
            }
        }
        private void EnableRun(bool enable = true)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<bool>(EnableRun), enable);
            }
            else
            {
                this.btnExec.Enabled = enable;
                this.btnClear.Enabled = enable;
                this.btnAbort.Enabled = !enable;
                this.btnKill.Enabled = !enable;
                this.cmbExecItems.Enabled = enable;
                this.TopMost = enable;
                this.Text = !enable ? ProcessingTitle : OriginTitle;

                if (this.TopMost)
                {
                    this.Activate();
                    this.TopMost = false;
                }

                Log.AppendLine(string.Empty);
                Log.AppendLine(string.Format("{0} {1}", ProcessingTitle.Replace(OriginTitle, string.Empty).Trim(), !enable ? "started." : "finished."), Color.LightSkyBlue);
                Log.AppendLine(string.Empty);

                if (enable)
                {
                    ShowNextLine();
                }

                if (enable)
                {
                    CLIWrapper.CurrentMode = CLIMode.NonProcessing;
                }
                else
                {
                    CLIWrapper.CurrentMode = CLIMode.Processing;
                }
            }

        }

        private void frmProvisioningBuildTools_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backGroundCommand.IsBusy)
            {
                e.Cancel = true;
            }

            if (!e.Cancel)
            {
                Log.Close();
            }
        }

        private void frmProvisioningBuildTools_Activated(object sender, EventArgs e)
        {
            this.Invalidate();
            this.TopMost = false;
        }

        private void frmProvisioningBuildTools_FormClosed(object sender, FormClosedEventArgs e)
        {
            GlobalValue.Serialize();
            Environment.Exit(0);
        }

        private void rtbCMD_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("msedge.exe", e.LinkText);
            }
            catch (Exception)
            {
                System.Diagnostics.Process.Start("IExplore.exe", e.LinkText);
            }
        }

        public string GetHistory(bool upOrDown)
        {
            try
            {
                return CLIWrapper.GetHistory(upOrDown);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void HandleInput(string command)
        {
            try
            {
                CLIWrapper.HandleInput(command);
            }
            catch (Exception ex)
            {
                Log.AppendLine($"Handle CLI Error with messgae: {ex.Message}", true);
                ShowNextLine();
            }
        }

        public void HandleNonProcessingError(string errorMessgae)
        {
            errorMessgae = errorMessgae ?? "Unknown Error";
            Log.AppendLine($"Handle CLI Error with messgae: {errorMessgae}", true);
            ShowNextLine();
        }

        public void HandleNonProcessingException(Exception ex)
        {
            ex = ex ?? default(Exception);
            Log.AppendLine($"Handle CLI Error with messgae: {ex.Message}", true);
            ShowNextLine();
        }

        public void HandleNonProcessingSkip()
        {
            ShowNextLine();
        }

        public void HandleNonProcessingClear()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(HandleNonProcessingClear));
            }
            else
            {
                btnClear.Enabled = true;
                btnClear.PerformClick();
            }
        }

        public void HandleNonProcessingChangeProject(string project)
        {
            ShowNextLine(project);
        }

        public void HandleNonProcessingHelp(string help)
        {
            Log.AppendLine(help);
            ShowNextLine();
        }

        public void HandleNonProcessingExec(AbCLIExecInstance instance)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<AbCLIExecInstance>(HandleNonProcessingExec), instance);
            }
            else
            {
                string item = cmbExecItems.Items.Cast<string>().FirstOrDefault(str => str.ToUpper() == instance.CLIExecEnum.ToString().ToUpper());

                if (!string.IsNullOrEmpty(item))
                {
                    cmbExecItems.SelectedItem = instance.CLIExecEnum.ToString();
                    btnExec.Tag = instance;
                    btnExec.PerformClick();
                }
                else
                {
                    WriteLog($"{instance.CLIExecEnum.ToString()} not supported!!!", true);
                }
            }
        }
    }
}
