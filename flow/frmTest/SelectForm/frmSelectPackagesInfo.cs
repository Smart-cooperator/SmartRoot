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
    public partial class frmSelectPackagesInfo : Form, ISelect<SelectLocalBranchOutput>
    {
        private SelectLocalBranchOutput m_SelectResult;
        public SelectLocalBranchOutput SelectResult => m_SelectResult;

        public ILogNotify LogNotify { get; set; }
        public ICommandNotify CommandNotify { get; set; }

        private SelectPackagesInfoInput input = new SelectPackagesInfoInput();

        private static string m_latestSelectBranch;

        private Action endInvoke;
        private Action startInvoke;


        public frmSelectPackagesInfo(ILogNotify logNotify, ICommandNotify commandNotify)
        {
            InitializeComponent();
            LogNotify = logNotify;
            CommandNotify = commandNotify;

            endInvoke = new Action(() => EnableRun(true));
            startInvoke = new Action(() => EnableRun(false));
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //m_latestSelectBranch = cmbLocalBranches.SelectedItem.ToString();
            //m_SelectResult = new SelectLocalBranchOutput(cmbLocalBranches.SelectedItem.ToString());
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void frmSelectPackagesInfo_Load(object sender, EventArgs e)
        {
            //this.dgvPackages.AutoGenerateColumns = false;
      
            //this.dgvPackages.DataSource = input.Packages;
            this.dgvPackages.CurrentCellDirtyStateChanged += dgvPackages_CurrentCellDirtyStateChanged;
        }

        private void dgvPackages_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvPackages.IsCurrentCellDirty)
            {
                dgvPackages.CurrentCellDirtyStateChanged -= dgvPackages_CurrentCellDirtyStateChanged;
                dgvPackages.CommitEdit(DataGridViewDataErrorContexts.Commit);
                dgvPackages.CurrentCellDirtyStateChanged += dgvPackages_CurrentCellDirtyStateChanged;
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
                this.Enabled = enable;
            }

        }
    }
}
