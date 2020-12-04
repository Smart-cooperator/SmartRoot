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
    public partial class frmSelectRemoteBranch : Form, ISelect<SelectLocalBranchOutput>
    {
        private SelectLocalBranchOutput m_SelectResult;
        public SelectLocalBranchOutput SelectResult => m_SelectResult;

        public ILogNotify LogNotify { get; set; }
        public ICommandNotify CommandNotify { get; set; }

        private SelectRemoteBranchInput input;

        private static string m_latestSelectProject;

        private Action endInvoke;
        private Action startInvoke;


        public frmSelectRemoteBranch(ILogNotify logNotify, ICommandNotify commandNotify)
        {
            InitializeComponent();
            LogNotify = logNotify;
            CommandNotify = commandNotify;

            endInvoke = new Action(() => EnableRun(true));
            startInvoke = new Action(() => EnableRun(false));
            input = new SelectRemoteBranchInput(CommandNotify,LogNotify, startInvoke, endInvoke);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            m_latestSelectProject = cmbBranch.SelectedItem.ToString();
            m_SelectResult = new SelectLocalBranchOutput(cmbBranch.SelectedItem.ToString());
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
    }
}
