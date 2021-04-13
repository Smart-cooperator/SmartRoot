using ProvisioningBuildTools.SelectInput;
using ProvisioningBuildTools.SelectOutput;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProvisioningBuildTools;
using ProvisioningBuildTools.CLI;

namespace ProvisioningBuildTools.SelectForm
{
    public partial class frmSelectRemoteProject : Form, ISelect<SelectRemoteProjectOutput>
    {
        private SelectRemoteProjectOutput m_SelectResult;
        public SelectRemoteProjectOutput SelectResult => m_SelectResult;

        public ILogNotify LogNotify { get; set; }
        public ICommandNotify CommandNotify { get; set; }

        private SelectRemoteProjectInput input;
        private SelectLocalProjectInput inputLocalFolder;

        private Action endInvoke;
        private Action startInvoke;

        private object GetBranchInfoLock = new object();

        private bool GetBranchInfo { get; set; }

        private string GetBranchName { get; set; }
        public AbCLIExecInstance CLIInstance { get; set; }
        private bool hasCLISelected = false;
        private bool hasCLISelectedSuccess = false;
        private Func<bool> cliSelect;
        private Action cliClickOK;

        private string m_CommandLine;
        public string CommandLine => m_CommandLine;

        public frmSelectRemoteProject(ILogNotify logNotify, ICommandNotify commandNotify)
        {
            InitializeComponent();
            LogNotify = logNotify;
            CommandNotify = commandNotify;

            endInvoke = new Action
                (
                    () =>
                    {
                        lock (GetBranchInfoLock)
                        {
                            if (GetBranchInfo)
                            {
                                this.ShowBranchInfo(input.GetBranchInfo(GetBranchName, true));
                                GetBranchInfo = false;
                                GetBranchName = null;
                            }

                            EnableRun(true);
                        }
                    }
                );

            startInvoke = new Action(() => EnableRun(false));
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            m_SelectResult = new SelectRemoteProjectOutput(cmbProject.SelectedItem, cmbBranch.SelectedItem, this.txtTag.Text, this.txtLastModifiedTime.Text, this.txtNewBranchName.Text);

            if (CLIInstance != null)
            {
                CLIInstance.CommandLineFormatParas["project"] = cmbProject.SelectedItem?.ToString();
                CLIInstance.CommandLineFormatParas["branch"] = cmbBranch.SelectedItem?.ToString().Split(new char[] { '/', '\\' }).Last();

                if (!string.IsNullOrEmpty(txtNewBranchName.Text.Trim()))
                {
                    CLIInstance.CommandLineFormatParas["newbranchname"] = txtNewBranchName.Text.Trim();
                }

                m_CommandLine = CLIInstance.GetCommandLine();
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void frmSelectRemoteBranch_Load(object sender, EventArgs e)
        {
            //cmbLocalBranches.IntegralHeight = false;           
            this.txtNewBranchName.Enabled = false;
            this.btnOK.Enabled = false;
            inputLocalFolder = new SelectLocalProjectInput(LogNotify);
            input = new SelectRemoteProjectInput(inputLocalFolder, CommandNotify, LogNotify, startInvoke, endInvoke);
        }

        private void EnableRun(bool enable = true)
        {
            if (this.InvokeRequired)
            {
                //this.Invoke(new Action<bool>(EnableRun), enable);
                this.BeginInvoke(new Action<bool>(EnableRun), enable);
            }
            else
            {
                this.Enabled = enable;

                if (enable)
                {
                    if (!hasCLISelected && cliSelect != null)
                    {
                        hasCLISelected = true;
                        hasCLISelectedSuccess = cliSelect();
                    }
                    else if (hasCLISelected && hasCLISelectedSuccess)
                    {
                        cliClickOK();
                    }
                }
            }

        }

        private void cmbProject_DropDown(object sender, EventArgs e)
        {
            if (cmbProject.Items.Count == 0)
            {
                if (input.Projects == null || input.Projects.Count == 0)
                {
                    this.txtNewBranchName.Enabled = false;
                }
                else
                {
                    cmbProject.Items.AddRange(input.Projects.Keys.ToArray());
                }
            }
        }

        private void cmbBranch_DropDown(object sender, EventArgs e)
        {
            if (cmbProject.Items.Count != 0 && cmbBranch.Items.Count == 0 && !string.IsNullOrEmpty(cmbProject.SelectedItem?.ToString()))
            {
                cmbBranch.Items.AddRange(input.Projects[cmbProject.SelectedItem?.ToString()]);
            }
        }

        private void cmbProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbBranch.Items.Clear();
            this.txtNewBranchName.Text = string.Empty;
            this.txtLocalBranchName.Text = string.Format(Command.PERSONAL, cmbProject.SelectedItem?.ToString());
            this.txtTag.Text = string.Empty;
            this.txtLastModifiedTime.Text = string.Empty;
            this.txtNewBranchName.Enabled = true;
        }

        private void cmbBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock (GetBranchInfoLock)
            {
                BranchInfo branchInfo = input.GetBranchInfo(cmbBranch.SelectedItem?.ToString());

                if (branchInfo == null)
                {
                    GetBranchName = cmbBranch.SelectedItem?.ToString();
                    GetBranchInfo = true;
                }
                else
                {
                    ShowBranchInfo(branchInfo);
                }
            }
        }

        private void ShowBranchInfo(BranchInfo branchInfo)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<BranchInfo>(ShowBranchInfo), branchInfo);
            }
            else
            {
                this.txtTag.Text = $"{branchInfo.Tag}";
                this.txtLastModifiedTime.Text = $"{branchInfo.LastModifiedTime}";
            }
        }

        private void txtNewBranchName_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNewBranchName.Text))
            {
                this.txtLocalBranchName.Text = $"{string.Format(Command.PERSONAL, cmbProject.SelectedItem?.ToString())}_{txtNewBranchName.Text}";
            }
            else
            {
                this.txtLocalBranchName.Text = string.Format(Command.PERSONAL, cmbProject.SelectedItem?.ToString());
            }
        }

        private void txtLocalBranchName_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtLocalBranchName.Text))
            {
                this.btnOK.Enabled = false;
            }
            else
            {
                this.btnOK.Enabled = true;
            }
        }

        public void CLIExec()
        {
            cliSelect = () =>
            {
                bool success = false;

                if (CLIInstance != null && CLIInstance.ParseSuccess && CLIInstance.FromCLI)
                {
                    cmbProject_DropDown(cmbProject, EventArgs.Empty);

                    string project = CLIInstance.GetParameterValue("project", string.Empty);
                    success = Utility.SetSelectedItem(cmbProject, project.Split('_').First(), false, true);

                    if (success)
                    {
                        cmbBranch_DropDown(cmbBranch, EventArgs.Empty);

                        string branch = CLIInstance.GetParameterValue("branch", string.Empty);

                        Func<string[], string, string> findFunc = (srcs, des) =>
                        {
                            string actualItem = null;

                            string[] tempsrcs = srcs.Select(str => str.Split(new char[] { '/', '\\' }).Last().ToUpper().Trim()).ToArray();
                            string tempdes = des.Split(new char[] { '/', '\\' }).Last().ToUpper().Trim();

                            if (!string.IsNullOrEmpty(tempdes))
                            {
                                string temp = tempsrcs.FirstOrDefault(tempsrc => tempsrc == tempdes);

                                if (!string.IsNullOrEmpty(temp))
                                {
                                    actualItem = srcs[tempsrcs.ToList().IndexOf(temp)];
                                }
                                else
                                {
                                    string[] temps = tempsrcs.Where(tempsrc => tempsrc.StartsWith(tempdes)).ToArray();

                                    if (temps != null && temps.Length == 1)
                                    {
                                        actualItem = srcs[tempsrcs.ToList().IndexOf(temps[0])];
                                    }
                                }
                            }

                            return actualItem;
                        };

                        success = Utility.SetSelectedItem(cmbBranch, branch, findFunc);

                        if (success)
                        {
                            string temp = CLIInstance.GetParameterValue("newbranchname", null);

                            if (!string.IsNullOrEmpty(temp))
                            {
                                txtNewBranchName.Text = temp;
                            }
                        }
                        else
                        {
                            branch = string.IsNullOrEmpty(branch) ? "NULL" : branch;
                            LogNotify.WriteLog($"CLI Error for branch:{branch}, you need to manual select for {CLIInstance.CLIExecEnum}", true);
                        }
                    }
                    else
                    {
                        project = string.IsNullOrEmpty(project) ? "NULL" : project;
                        LogNotify.WriteLog($"CLI Error for project:{project}, you need to manual select for {CLIInstance.CLIExecEnum}", true);
                    }
                }

                return success;
            };

            cliClickOK = () =>
            {
                if (CLIInstance != null && CLIInstance.ParseSuccess && CLIInstance.FromCLI)
                {
                    if (btnOK.Enabled)
                    {
                        btnOK.PerformClick();
                    }
                }
            };
        }
    }
}
