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
    public partial class frmSelectLoaclBranchForUPT : Form, ISelect<SelectLocalBranchOutput>
    {
        private SelectLocalBranchOutput m_SelectResult;
        public SelectLocalBranchOutput SelectResult => m_SelectResult;

        public ILogNotify LogNotify { get; set; }
        public ICommandNotify CommandNotify { get; set; }

        private SelectLocalBranchInput input = new SelectLocalBranchInput();

        private static string m_latestSelectBranch;

        private Action endInvoke;
        private Action startInvoke;


        public frmSelectLoaclBranchForUPT(ILogNotify logNotify, ICommandNotify commandNotify)
        {
            InitializeComponent();
            LogNotify = logNotify;
            CommandNotify = commandNotify;

            endInvoke = new Action(() => EnableRun(true));
            startInvoke = new Action(() => EnableRun(false));
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            m_latestSelectBranch = cmbLocalBranches.SelectedItem.ToString();
            Tuple<string, string, Action<string>> value = input.GetByProject(cmbLocalBranches.SelectedItem.ToString(), LogNotify);
            m_SelectResult = new SelectLocalBranchOutput(cmbLocalBranches.SelectedItem.ToString(), value.Item1, value.Item2, value.Item3);

            if (MessageBox.Show($"Please make sure your change of {value.Item1} is in {Path.Combine(Command.REPOSFOLDER, m_latestSelectBranch, value.Item2)}","Double confirm",MessageBoxButtons.YesNo,MessageBoxIcon.Warning,MessageBoxDefaultButton.Button2)==DialogResult.Yes)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void frmSelectLoaclBranchForUPT_Load(object sender, EventArgs e)
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

            this.lblReposFolder.Text = Command.REPOSFOLDER;
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

        private void cmbLocalBranches_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (input.GetByProject(cmbLocalBranches.SelectedItem.ToString(), LogNotify) != null)
            {
                this.btnOK.Enabled = true;
            }
        }
    }
}
