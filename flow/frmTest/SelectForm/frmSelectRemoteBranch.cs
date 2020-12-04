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
using Utilities;

namespace ProvisioningBuildTools.SelectForm
{
    public partial class frmSelectRemoteBranch : Form, ISelect<SelectRemoteBranchOutput>
    {
        private SelectRemoteBranchOutput m_SelectResult;
        public SelectRemoteBranchOutput SelectResult => m_SelectResult;

        public ILogNotify LogNotify { get; set; }
        public ICommandNotify CommandNotify { get; set; }

        private SelectRemoteBranchInput input;

        private Action endInvoke;
        private Action startInvoke;

        private object GetBranchInfoLock = new object();

        private bool GetBranchInfo { get; set; }

        private string GetBranchName { get; set; }

        public frmSelectRemoteBranch(ILogNotify logNotify, ICommandNotify commandNotify)
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
            m_SelectResult = new SelectRemoteBranchOutput(cmbProject.SelectedItem, cmbBranch.SelectedItem,this.txtTag.Text, this.txtLastModifiedTime.Text, this.txtNewBranchName.Text);
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
            if (cmbProject.Items.Count != 0 && cmbBranch.Items.Count == 0 && !string.IsNullOrEmpty(cmbProject.SelectedItem.ToString()))
            {
                cmbBranch.Items.AddRange(input.Projects[cmbProject.SelectedItem.ToString()]);
            }
        }

        private void cmbProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbBranch.Items.Clear();
            this.txtNewBranchName.Text = string.Empty;
            this.txtLocalBranchName.Text = string.Format(Command.PERSONAL, cmbProject.SelectedItem.ToString());
            this.txtTag.Text = string.Empty;
            this.txtLastModifiedTime.Text = string.Empty;
            this.txtNewBranchName.Enabled = true;
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

        private void txtNewBranchName_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNewBranchName.Text))
            {
                this.txtLocalBranchName.Text = $"{string.Format(Command.PERSONAL, cmbProject.SelectedItem.ToString())}_{txtNewBranchName.Text}";
            }
            else
            {
                this.txtLocalBranchName.Text = string.Format(Command.PERSONAL, cmbProject.SelectedItem.ToString());
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
    }
}
