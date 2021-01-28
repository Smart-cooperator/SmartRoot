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
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;

namespace ProvisioningBuildTools.SelectForm
{
    public partial class frmSelectLoaclBranch : Form, ISelect<SelectLocalBranchOutput>
    {
        private SelectLocalBranchOutput m_SelectResult;
        public SelectLocalBranchOutput SelectResult => m_SelectResult;

        public ILogNotify LogNotify { get; set; }
        public ICommandNotify CommandNotify { get; set; }

        private SelectLocalBranchInput input;

        private static string m_latestSelectBranch;

        private Action endInvoke;
        private Action startInvoke;


        public frmSelectLoaclBranch(ILogNotify logNotify, ICommandNotify commandNotify)
        {
            InitializeComponent();
            LogNotify = logNotify;
            CommandNotify = commandNotify;

            input = new SelectLocalBranchInput(logNotify);

            endInvoke = new Action(() => EnableRun(true));
            startInvoke = new Action(() => EnableRun(false));
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            m_latestSelectBranch = cmbLocalBranches.SelectedItem?.ToString();

            LocalProjectInfo localProjectInfo = input.GetProjectInfo(cmbLocalBranches.SelectedItem?.ToString());

            if (File.Exists(Path.Combine(localProjectInfo.SourceFolder, Command.REPOSSLN)))
            {
                m_SelectResult = new SelectLocalBranchOutput(localProjectInfo);
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                LogNotify.WriteLog($"{Command.REPOSSLN} not found in project path {localProjectInfo.SourceFolder}!!!", true);
                this.DialogResult = DialogResult.Cancel;
            }

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void frmSelectLoaclBranch_Load(object sender, EventArgs e)
        {
            //cmbLocalBranches.IntegralHeight = false;           

            if (input.LocalBranches == null || input.LocalBranches.Length == 0)
            {
                this.btnOK.Enabled = false;
            }
            else
            {
                cmbLocalBranches.Items.AddRange(input.LocalBranches);
            }


            if (cmbLocalBranches.Items.Count > 0)
            {
                cmbLocalBranches.SelectedIndex = 0;
            }

            if (!string.IsNullOrEmpty(m_latestSelectBranch) && cmbLocalBranches.Items.Contains(m_latestSelectBranch))
            {
                cmbLocalBranches.SelectedItem = m_latestSelectBranch;
            }

            //this.lblReposFolder.Text = Command.REPOSFOLDER;
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
    }
}
