using ProvisioningBuildTools.SelectInput;
using ProvisioningBuildTools.SelectOutput;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;

namespace ProvisioningBuildTools.SelectForm
{
    public partial class frmSelectRemoteBranchPostBuild : Form, ISelect<SelectRemoteBranchOutputPostBuild>
    {
        private SelectRemoteBranchOutputPostBuild m_SelectResult;
        public SelectRemoteBranchOutputPostBuild SelectResult => m_SelectResult;

        public ILogNotify LogNotify { get; set; }
        public ICommandNotify CommandNotify { get; set; }

        private SelectRemoteBranchInput input;

        private SelectLocalBranchInput inputLocalFolder;

        private Action endInvoke;
        private Action startInvoke;

        private object GetBranchInfoLock = new object();

        private bool GetBranchInfo { get; set; }

        private object WaitStateLock = new object();

        private bool WaitState { get; set; }

        private string GetBranchName { get; set; }

        public frmSelectRemoteBranchPostBuild(ILogNotify logNotify, ICommandNotify commandNotify)
        {
            InitializeComponent();
            LogNotify = logNotify;
            CommandNotify = commandNotify;            

            endInvoke = new Action
                (
                    () =>
                    {
                        bool waitTag = GetWaitState();

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

                        if (waitTag)
                        {
                            Action action = new Action(
                                () =>
                                {
                                    if (btnOK.Enabled)
                                    {
                                        btnOK.PerformClick();
                                    }
                                    else
                                    {
                                        btnCancel.PerformClick();
                                    }
                                });
                            this.Invoke(action);
                        }
                    }
                );

            startInvoke = new Action(() => EnableRun(false));
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            m_SelectResult = new SelectRemoteBranchOutputPostBuild(cmbProject.SelectedItem, cmbBranch.SelectedItem, this.txtTag.Text, cmdLocalBuildFolder.SelectedItem, input.GenerateWaitGetTagFunc(cmbBranch.SelectedItem.ToString()));
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void frmSelectRemoteBranchPostBuild_Load(object sender, EventArgs e)
        {
            this.btnOK.Enabled = false;
            this.btnWait.Enabled = false;
            SetWaitState(false);
            inputLocalFolder = new SelectLocalBranchInput(LogNotify);
            input = new SelectRemoteBranchInput(inputLocalFolder,CommandNotify, LogNotify, startInvoke, endInvoke);
        }

        private void EnableRun(bool enable = true)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<bool>(EnableRun), enable);
            }
            else
            {
                this.Enabled = enable;
            }

        }

        private void cmbProject_DropDown(object sender, EventArgs e)
        {
            if (cmbProject.Items.Count == 0)
            {
                if (input.Projects == null || input.Projects.Count == 0)
                {

                }
                else
                {
                    //cmbProject.Items.AddRange(input.Projects.Keys.Select(str => Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(str)).ToArray());
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

            this.txtTag.Text = string.Empty;
            this.txtLastModifiedTime.Text = string.Empty;

            cmdLocalBuildFolder.Items.Clear();

            string project = cmbProject.SelectedItem?.ToString();

            cmdLocalBuildFolder.Items.AddRange(inputLocalFolder.LocalBranches.Where(localProject => localProject.ToLower() == project.ToLower() || localProject.ToLower().StartsWith($"{project.ToLower()}_")).Select(str => inputLocalFolder.GetProjectInfo(str).BuildScriptsFolder).ToArray());

            if (cmdLocalBuildFolder.Items.Count != 0)
            {
                cmdLocalBuildFolder.SelectedIndex = 0;
            }
        }

        private void cmbBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.btnOK.Enabled = false;
            this.btnWait.Enabled = false;

            SetWaitState(false);

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
                bool vaildBtnOk = this.txtTag.Text == $"{branchInfo.Tag}";

                this.txtTag.Text = $"{branchInfo.Tag}";
                this.txtLastModifiedTime.Text = $"{branchInfo.LastModifiedTime}";

                if (vaildBtnOk)
                {
                    ValidBtnOK();
                }
            }
        }


        private void ValidBtnOK()
        {
            bool enable = false;
            bool waitTag = false;

            if (cmbProject.SelectedItem != null && cmbBranch.SelectedItem != null && cmdLocalBuildFolder.SelectedItem != null)
            {
                if (string.IsNullOrWhiteSpace(txtTag.Text))
                {
                    TimeSpan timeSpan = DateTime.Now.Subtract((DateTime)input.GetBranchInfo(cmbBranch.SelectedItem.ToString(), true).LastModifiedTime);

                    if (timeSpan > TimeSpan.FromDays(1))
                    {
                        LogNotify.WriteLog($"{cmbBranch.SelectedItem.ToString()} Lastmodified time {(DateTime)input.GetBranchInfo(cmbBranch.SelectedItem.ToString(), true).LastModifiedTime} over due 1 day, couldn't wait tag", true);

                    }
                    else
                    {
                        waitTag = true;
                    }
                }

                if (!string.IsNullOrWhiteSpace(txtTag.Text) || waitTag)
                {
                    string project = cmbProject.SelectedItem?.ToString();
                    string localFolder = cmdLocalBuildFolder.SelectedItem?.ToString();

                    if (!string.IsNullOrEmpty(localFolder))
                    {
                        enable = true;
                    }
                }
            }

            if (enable)
            {
                if (waitTag)
                {
                    this.btnWait.Enabled = true;
                    this.btnOK.Enabled = false;
                }
                else
                {
                    this.btnOK.Enabled = true;
                    this.btnWait.Enabled = false;
                }

                SetWaitState(waitTag);
            }
            else
            {
                this.btnOK.Enabled = false;
                this.btnWait.Enabled = false;
            }
        }

        private void cmdLocalBuildFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidBtnOK();
        }

        private void txtTag_TextChanged(object sender, EventArgs e)
        {
            ValidBtnOK();
        }

        private void btnWait_Click(object sender, EventArgs e)
        {
            bool test = true;

            if (test)
            {
                this.btnOK.Enabled = true;
                btnOK.PerformClick();
            }
            else
            {
                lock (GetBranchInfoLock)
                {
                    GetBranchName = cmbBranch.SelectedItem?.ToString();
                    GetBranchInfo = true;

                    input.WaitAndGetTag(GetBranchName);
                }
            }
        }

        private bool GetWaitState()
        {
            bool waitState = false;

            lock (WaitStateLock)
            {
                waitState = WaitState;
            }

            return waitState;
        }

        private void SetWaitState(bool waitState)
        {
            lock (WaitStateLock)
            {
                WaitState = waitState;
            }
        }
    }
}
