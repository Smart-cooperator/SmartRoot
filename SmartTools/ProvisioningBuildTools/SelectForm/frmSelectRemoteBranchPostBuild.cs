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

        private SelectLocalBranchInput inputLocalFolder = new SelectLocalBranchInput();

        private Action endInvoke;
        private Action startInvoke;

        private object GetBranchInfoLock = new object();

        private bool GetBranchInfo { get; set; }

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
            m_SelectResult = new SelectRemoteBranchOutputPostBuild(cmbProject.SelectedItem, cmbBranch.SelectedItem,this.txtTag.Text,cmdLocalBuildFolder.SelectedItem);
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
            input = new SelectRemoteBranchInput(CommandNotify, LogNotify, startInvoke, endInvoke);
        }

        private void EnableRun(bool enable = true)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<bool>(EnableRun), enable);
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
            if (cmbProject.Items.Count != 0 && cmbBranch.Items.Count == 0 && !string.IsNullOrEmpty(cmbProject.SelectedItem.ToString()))
            {
                cmbBranch.Items.AddRange(input.Projects[cmbProject.SelectedItem.ToString()]);
            }
        }

        private void cmbProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbBranch.Items.Clear();

            this.txtTag.Text = string.Empty;
            this.txtLastModifiedTime.Text = string.Empty;

            cmdLocalBuildFolder.Items.Clear();

            string project = cmbProject.SelectedItem.ToString();

            cmdLocalBuildFolder.Items.AddRange(inputLocalFolder.LocalBranches.Where(localProject => localProject == project || localProject.StartsWith($"{project}_")).Select(str=>string.Format(Command.BUILDSCRIPTSFOLDER,str)).ToArray());

            if (cmdLocalBuildFolder.Items.Count != 0)
            {
                cmdLocalBuildFolder.SelectedIndex = 0;
            }
        }

        private void cmbBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock (GetBranchInfoLock)
            {
                BranchInfo branchInfo = input.GetBranchInfo(cmbBranch.SelectedItem.ToString());

                if (branchInfo == null)
                {
                    GetBranchName = cmbBranch.SelectedItem.ToString();
                    GetBranchInfo = true;
                }
                else
                {
                    ShowBranchInfo(branchInfo);
                }
            }

            //ValidBtnOK();
        }

        private void ShowBranchInfo(BranchInfo branchInfo)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<BranchInfo>(ShowBranchInfo), branchInfo);
            }
            else
            {
                this.txtTag.Text = $"{branchInfo.Tag}";
                this.txtLastModifiedTime.Text = $"{branchInfo.LastModifiedTime}";
            }
        }


        private void ValidBtnOK()
        {
            bool enable = false;

            if (cmbProject.SelectedItem != null && cmbBranch.SelectedItem != null && cmdLocalBuildFolder.SelectedItem != null && !string.IsNullOrWhiteSpace(txtTag.Text))
            {
                string project = cmbProject.SelectedItem.ToString();
                string localFolder = cmdLocalBuildFolder.SelectedItem.ToString();

                string localProject = (new DirectoryInfo(localFolder)).Parent.Name;

                if (localProject == project || localProject.StartsWith($"{project}_"))
                {
                    enable = true;
                }
            }

            this.btnOK.Enabled = enable;
        }

        private void cmdLocalBuildFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidBtnOK();
        }

        private void txtTag_TextChanged(object sender, EventArgs e)
        {
            ValidBtnOK();
        }
    }
}
