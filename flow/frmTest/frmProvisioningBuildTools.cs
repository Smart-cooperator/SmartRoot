using ProvisioningBuildTools.SelectForm;
using ProvisioningBuildTools.SelectInput;
using ProvisioningBuildTools.SelectOutput;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;

namespace ProvisioningBuildTools
{
    public partial class frmProvisioningBuildTools : Form, ICommandNotify, ILogNotify, IMainWindowsTitleNotify
    {
        public frmProvisioningBuildTools()
        {
            InitializeComponent();
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

        public void WriteLog(string logLine, bool showMessageBox = false)
        {
            if (!this.InvokeRequired)
            {
                //richTextBox1.AppendText($"{logLine}{Environment.NewLine}");
                AppenLine(logLine, Color.Green);
            }
            else
            {
                if (showMessageBox)
                {
                    this.Invoke(new Action(() => MessageBox.Show(logLine, "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning)));
                }
                else
                {
                    this.BeginInvoke(new Action<string, bool>(WriteLog), logLine, showMessageBox);
                }
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

        public void Write(string title)
        {
            //throw new NotImplementedException();
        }

        private void Form1_Load(object sender, EventArgs e)
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
                rtbCMD.ReadOnly = true;

                this.btnAbort.Enabled = false;

                this.Text = $"Administrator: {this.Text}(Owner by v-fengzhou@microsoft.com)";

                try
                {
                    Command.RuneWDK(this, this, this);
                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action<Exception>(WriteLog), ex);
                    this.Enabled = false;
                }

            }
            else
            {
                this.Enabled = false;
                this.BeginInvoke(new Action(() => { MessageBox.Show("Pls run as Administrator", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning); this.Close(); }));
            }
        }


        private void AppenLine(string line, bool error = false)
        {
            AppenLine(line, error ? Color.Red : Color.Black);
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
                rtbCMD.Clear();
                Command.RuneWDK(this, this, this);
                Thread.Sleep(200);
                btnClear.Enabled = false;
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
            Action runAct = null;
            Action endInvoke = new Action(() => EnableRun(true));
            Action startInvoke = new Action(() => EnableRun(false));

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
                                selectFrom = new frmSelectLoaclBranch(this, this);
                                break;
                            case ExecEnum.DropRemoteBranch:
                                break;
                            case ExecEnum.UpdateExternalDrops:
                                break;
                            case ExecEnum.InstallSurfacePackage:
                                break;
                            case ExecEnum.UploadSurfacePackage:
                                break;
                            case ExecEnum.PostBuildPackage:
                                break;
                            default:
                                break;
                        }

                        if (selectFrom.ShowDialog(this) == DialogResult.OK)
                        {
                            SelectLocalBranchOutput selectLocalBranchOutput;
                            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                            switch (execEnum)
                            {
                                case ExecEnum.OpenLocalBranch:
                                    selectLocalBranchOutput = ((ISelect<SelectLocalBranchOutput>)selectFrom).SelectResult;
                                    runAct = new Action(() => Command.OpenReposSln(selectLocalBranchOutput.SelectedLocalBranch, null, cancellationTokenSource));
                                    break;
                                case ExecEnum.BuildLocalBranch:
                                    selectLocalBranchOutput = ((ISelect<SelectLocalBranchOutput>)selectFrom).SelectResult;
                                    runAct = new Action(() => { Command.RebulidAll(selectLocalBranchOutput.SelectedLocalBranch, null, cancellationTokenSource); Command.CreatePacakge(selectLocalBranchOutput.SelectedLocalBranch, null, cancellationTokenSource); });
                                    break;
                                case ExecEnum.DropRemoteBranch:
                                    break;
                                case ExecEnum.UpdateExternalDrops:
                                    break;
                                case ExecEnum.InstallSurfacePackage:
                                    break;
                                case ExecEnum.UploadSurfacePackage:
                                    break;
                                case ExecEnum.PostBuildPackage:
                                    break;
                                default:
                                    break;
                            }

                            backGroundCommand.AsyncRun(runAct, startInvoke, endInvoke, cancellationTokenSource, this);
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
                    backGroundCommand.Abort();
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
            }

        }

        private void frmProvisioningBuildTools_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backGroundCommand.IsBusy)
            {
                e.Cancel = true;
            }
        }
    }
}
