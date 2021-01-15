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
using Utilities;

namespace ProvisioningBuildTools
{
    public partial class frmProvisioningBuildTools : Form, ICommandNotify, ILogNotify
    {
        public frmProvisioningBuildTools()
        {
            InitializeComponent();
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

        private readonly BackGroundCommand backGroundCommand = new BackGroundCommand();

        public void WriteOutPut(int processId, string outputLine)
        {
            if (!this.InvokeRequired)
            {
                //richTextBox1.AppendText($"ProcessId {processId}:{outputLine}{Environment.NewLine}");
                // richTextBox1.AppendText($"{outputLine}{Environment.NewLine}");
                AppenLine(outputLine);
            }
            else
            {
                this.BeginInvoke(new Action<int, string>(WriteOutPut), processId, outputLine);
            }
        }

        public void WriteErrorOutPut(int processId, string errorLine)
        {
            if (!this.InvokeRequired)
            {
                //richTextBox1.AppendText($"ProcessId {processId}:{errorLine}{Environment.NewLine}");
                //richTextBox1.AppendText($"{errorLine}{Environment.NewLine}");
                AppenLine(errorLine, true);
            }
            else
            {
                this.BeginInvoke(new Action<int, string>(WriteErrorOutPut), processId, errorLine);
            }
        }

        public void Exit(int processId, int exitCode)
        {
            if (!this.InvokeRequired)
            {
                //richTextBox1.AppendText($"ProcessId {processId}:ExitCode {exitCode}{Environment.NewLine}");
                //richTextBox1.AppendText($"ExitCode {exitCode}{Environment.NewLine}");
                AppenLine($"ExitCode {exitCode}");
            }
            else
            {
                this.BeginInvoke(new Action<int, int>(Exit), processId, exitCode);
            }
        }

        public void WriteLog(string logLine, bool hasError = false)
        {
            if (!this.InvokeRequired)
            {
                //richTextBox1.AppendText($"{logLine}{Environment.NewLine}");
                AppenLine(logLine, !hasError ? Color.LightGreen : Color.Red);
            }
            else
            {
                this.BeginInvoke(new Action<string, bool>(WriteLog), logLine, hasError);
            }
        }

        public void WriteLog(Exception ex)
        {
            if (!this.InvokeRequired)
            {
                //richTextBox1.AppendText($"{logLine}{Environment.NewLine}");
                AppenLine($"throw exception:{ex.Message}", true);
            }
            else
            {
                this.BeginInvoke(new Action<Exception>(WriteLog), ex);
            }
        }

        private void frmProvisioningBuildTools_Load(object sender, EventArgs e)
        {

            if (IsRunAsAdmin())
            {
                cmbExecItems.DropDownStyle = ComboBoxStyle.DropDownList;

                cmbExecItems.Items.AddRange(Enum.GetNames(typeof(ExecEnum)));

                if (cmbExecItems.Items.Count > 0)
                {
                    cmbExecItems.SelectedIndex = 0;
                }

                rtbCMD.WordWrap = false;
                rtbCMD.Multiline = true;
                rtbCMD.ScrollBars = RichTextBoxScrollBars.Both;
                rtbCMD.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
                rtbCMD.Font = new Font(rtbCMD.Font.FontFamily, (float)10.25);
                rtbCMD.BackColor = Color.Black;
                //rtbCMD.ReadOnly = true;

                this.btnAbort.Enabled = false;
                this.btnKill.Enabled = false;

                string text = this.Text;
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                string context = $"{text} [Version {version}]{Environment.NewLine}Author: v-fengzhou@microsoft.com{Environment.NewLine}{Environment.NewLine}";

                btnClear.Tag = new Action(() => { rtbCMD.Clear(); btnClear.Enabled = false; AppenLine(context, Color.LightGreen); });

                this.Text = $"Administrator: {this.Text}";
                //this.Text = $"Administrator: {this.Text}(Owner by v-fengzhou@microsoft.com)";

                btnClear.PerformClick();
            }
            else
            {
                this.Enabled = false;
                this.BeginInvoke(new Action(() => { MessageBox.Show("Pls run as Administrator", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning); this.Close(); }));
            }
        }


        private void AppenLine(string line, bool error = false)
        {
            AppenLine(line, error ? Color.Red : Color.White);
        }

        private void AppenLine(string line, Color color)
        {
            try
            {
                line = $"{line}{Environment.NewLine}";

                int start = rtbCMD.TextLength - 1;

                //if (start>80000)
                //{
                //    rtbCMD.Clear();
                //}

                rtbCMD.AppendText(line);

                int end = rtbCMD.TextLength - 1;

                rtbCMD.SelectionStart = start + 1;
                rtbCMD.SelectionLength = end - start;
                rtbCMD.SelectionColor = color;

                rtbCMD.SelectionStart = rtbCMD.TextLength;

                rtbCMD.ScrollToCaret();

            }
            catch (Exception)
            {
                rtbCMD.Clear();
            }
            finally
            {
                //Application.DoEvents();
            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                //rtbCMD.Clear();

                //btnClear.Enabled = false;

                ((Action)btnClear.Tag).Invoke();
            }
            catch (Exception ex)
            {
                this.BeginInvoke(new Action<Exception>(WriteLog), ex);
                this.Enabled = false;
            }
        }

        private void btnExec_Click(object sender, EventArgs e)
        {
            btnClear.PerformClick();

            ExecEnum execEnum;
            Form selectFrom = null;
            List<Func<CommandResult>> runAct = null;
            Action endInvoke = new Action(() => EnableRun(true));
            Action startInvoke = new Action(() => EnableRun(false));
            SelectLocalBranchOutput selectLocalBranchOutput;
            SelectRemoteBranchOutput selectRemoteBranchOutput;
            SelectPackagesInfoOutput selectPackagesInfoOutput;
            SelectRemoteBranchOutputPostBuild selectRemoteBranchOutputPostBuild;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationTokenSource cancellationTokenSourceForKill = new CancellationTokenSource();
            try
            {

                if (Enum.TryParse<ExecEnum>(cmbExecItems.SelectedItem.ToString(), out execEnum))
                {
                    if (!backGroundCommand.IsBusy)
                    {
                        switch (execEnum)
                        {
                            case ExecEnum.OpenLocalBranch:
                            case ExecEnum.BuildLocalBranch:
                            case ExecEnum.UpdateExternalDrops:
                                selectFrom = new frmSelectLoaclBranch(this, this);
                                break;
                            case ExecEnum.DropRemoteBranch:
                                selectFrom = new frmSelectRemoteBranch(this, this);
                                break;
                            case ExecEnum.PostBuildPackage:
                                selectFrom = new frmSelectRemoteBranchPostBuild(this, this);
                                break;
                            case ExecEnum.InstallSurfacePackage:
                                selectFrom = new frmSelectPackagesInfo(this, this);
                                break;
                            case ExecEnum.UploadProvisionTools:
                                selectFrom = new frmSelectLoaclBranchForUPT(this, this);
                                break;
                            default:
                                break;
                        }

                        if (selectFrom.ShowDialog(this) == DialogResult.OK)
                        {
                            switch (execEnum)
                            {
                                case ExecEnum.OpenLocalBranch:
                                    selectLocalBranchOutput = ((ISelect<SelectLocalBranchOutput>)selectFrom).SelectResult;
                                    runAct = new List<Func<CommandResult>>()
                                    {
                                        new Func<CommandResult>(() => Command.OpenReposSln(selectLocalBranchOutput.SelectedLocalBranch, this, this, cancellationTokenSource,cancellationTokenSourceForKill))
                                    };
                                    break;
                                case ExecEnum.BuildLocalBranch:
                                    selectLocalBranchOutput = ((ISelect<SelectLocalBranchOutput>)selectFrom).SelectResult;
                                    runAct = new List<Func<CommandResult>>()
                                    {
                                        new Func<CommandResult>(() => Command.RebulidAll(selectLocalBranchOutput.SelectedLocalBranch, this, this, cancellationTokenSource,cancellationTokenSourceForKill)),
                                        new Func<CommandResult>(() => Command.CreatePackage(selectLocalBranchOutput.SelectedLocalBranch, this, this, cancellationTokenSource,cancellationTokenSourceForKill))
                                    };
                                    break;
                                case ExecEnum.UpdateExternalDrops:
                                    selectLocalBranchOutput = ((ISelect<SelectLocalBranchOutput>)selectFrom).SelectResult;
                                    runAct = new List<Func<CommandResult>>()
                                    {
                                        new Func<CommandResult>(() => Command.UpdateExternalDrops(selectLocalBranchOutput.SelectedLocalBranch, this, this, cancellationTokenSource,cancellationTokenSourceForKill)),
                                        new Func<CommandResult>(() => Command.RebulidAll(selectLocalBranchOutput.SelectedLocalBranch, this, this, cancellationTokenSource,cancellationTokenSourceForKill)),
                                        new Func<CommandResult>(() => Command.CreatePackage(selectLocalBranchOutput.SelectedLocalBranch, this, this, cancellationTokenSource,cancellationTokenSourceForKill))
                                        };
                                    break;
                                case ExecEnum.DropRemoteBranch:
                                    selectRemoteBranchOutput = ((ISelect<SelectRemoteBranchOutput>)selectFrom).SelectResult;
                                    runAct = new List<Func<CommandResult>>()
                                    {
                                        new Func<CommandResult>(() => Command.CheckOutLatestBranch(selectRemoteBranchOutput.SelectProject,selectRemoteBranchOutput.NewBranchName,new Tuple<string, DateTime?, Version>(selectRemoteBranchOutput.SelectRemoteBranch,selectRemoteBranchOutput.LastModifiedTime,selectRemoteBranchOutput.Tag), this, this, cancellationTokenSource,cancellationTokenSourceForKill))
                                        };
                                    break;
                                case ExecEnum.PostBuildPackage:
                                    selectRemoteBranchOutputPostBuild = ((ISelect<SelectRemoteBranchOutputPostBuild>)selectFrom).SelectResult;
                                    runAct = new List<Func<CommandResult>>()
                                    {
                                        new Func<CommandResult>(() => Command.PostBuild(selectRemoteBranchOutputPostBuild.LocalBuildScriptsFolder,new Tuple<string, string, Version>(selectRemoteBranchOutputPostBuild.SelectProject,selectRemoteBranchOutputPostBuild.SelectRemoteBranch,selectRemoteBranchOutputPostBuild.Tag), this, this, cancellationTokenSource,cancellationTokenSourceForKill))
                                        };
                                    break;
                                case ExecEnum.InstallSurfacePackage:
                                    selectPackagesInfoOutput = ((ISelect<SelectPackagesInfoOutput>)selectFrom).SelectResult;
                                    runAct = new List<Func<CommandResult>>()
                                    {
                                        new Func<CommandResult>(() => Command.InstallSurfacePackage(selectPackagesInfoOutput.GeneratePackageConfig, this, this, cancellationTokenSource,cancellationTokenSourceForKill))
                                    };
                                    break;
                                case ExecEnum.UploadProvisionTools:
                                    selectLocalBranchOutput = ((ISelect<SelectLocalBranchOutput>)selectFrom).SelectResult;
                                    runAct = new List<Func<CommandResult>>()
                                    {
                                        new Func<CommandResult>(() => Command.UploadProvisioningTools2Nuget(selectLocalBranchOutput.SelectedLocalBranch,selectLocalBranchOutput.ProvisioningToolsPackageId,selectLocalBranchOutput.ProvisioningToolsPackageDestination,selectLocalBranchOutput.UpdateNewVersionAction, this, this, cancellationTokenSource,cancellationTokenSourceForKill)),
                                        new Func<CommandResult>(() => Command.UpdateExternalDrops(selectLocalBranchOutput.SelectedLocalBranch, this, this, cancellationTokenSource,cancellationTokenSourceForKill)),
                                        new Func<CommandResult>(() => Command.RebulidAll(selectLocalBranchOutput.SelectedLocalBranch, this, this, cancellationTokenSource,cancellationTokenSourceForKill)),
                                        new Func<CommandResult>(() => Command.CreatePackage(selectLocalBranchOutput.SelectedLocalBranch, this, this, cancellationTokenSource,cancellationTokenSourceForKill))
                                        };
                                    break;
                                default:
                                    break;
                            }

                            backGroundCommand.AsyncRun(runAct.ToArray(), startInvoke, endInvoke, cancellationTokenSource, this, cancellationTokenSourceForKill);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.BeginInvoke(new Action<Exception>(WriteLog), ex);
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
                this.BeginInvoke(new Action<Exception>(WriteLog), ex);
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
                this.BeginInvoke(new Action<Exception>(WriteLog), ex);
            }
        }
        private void EnableRun(bool enable = true)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<bool>(EnableRun), enable);
            }
            else
            {
                this.btnExec.Enabled = enable;
                this.btnClear.Enabled = enable;
                this.btnAbort.Enabled = !enable;
                this.btnKill.Enabled = !enable;
                this.TopMost = enable;

                if (this.TopMost)
                {
                    this.Activate();
                }
            }

        }

        private void frmProvisioningBuildTools_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backGroundCommand.IsBusy)
            {
                e.Cancel = true;
            }
        }

        private void frmProvisioningBuildTools_Activated(object sender, EventArgs e)
        {
            this.Invalidate();
            this.TopMost = false;
        }
    }
}
