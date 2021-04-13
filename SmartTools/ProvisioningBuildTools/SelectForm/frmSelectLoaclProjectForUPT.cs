using ProvisioningBuildTools.SelectInput;
using ProvisioningBuildTools.SelectOutput;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProvisioningBuildTools;
using ProvisioningBuildTools.Global;
using ProvisioningBuildTools.CLI;
using System.Threading;

namespace ProvisioningBuildTools.SelectForm
{
    public partial class frmSelectLoaclProjectForUPT : Form, ISelect<SelectLocalProjectOutput>
    {
        private SelectLocalProjectOutput m_SelectResult;
        public SelectLocalProjectOutput SelectResult => m_SelectResult;

        public ILogNotify LogNotify { get; set; }
        public ICommandNotify CommandNotify { get; set; }
        public AbCLIExecInstance CLIInstance { get; set; }

        private string m_CommandLine;
        public string CommandLine => m_CommandLine;

        private SelectLocalProjectInput input;

        private Action endInvoke;
        private Action startInvoke;

        private string[] defaultSelectedUploadList;

        private bool needDoubleConfirm = true;

        public frmSelectLoaclProjectForUPT(ILogNotify logNotify, ICommandNotify commandNotify)
        {
            InitializeComponent();
            LogNotify = logNotify;
            CommandNotify = commandNotify;

            input = new SelectLocalProjectInput(logNotify);

            endInvoke = new Action(() => EnableRun(true));
            startInvoke = new Action(() => EnableRun(false));

            string defaultSelectedUploadListStr = ConfigurationManager.AppSettings["defaultSelectedUploadList"];
            defaultSelectedUploadList = !string.IsNullOrEmpty(defaultSelectedUploadListStr) ? defaultSelectedUploadListStr.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { "ProvisioningTools" };
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            GlobalValue.Root.SelectedProject = cmbLocalBranches.SelectedItem?.ToString();

            LocalProjectInfo localProjectInfo = input.GetProjectInfo(cmbLocalBranches.SelectedItem?.ToString());

            m_SelectResult = new SelectLocalProjectOutput(localProjectInfo, chkPackageList.CheckedItems.Cast<string>().ToArray(), chkAppend.Visible && chkAppend.Checked, chkAppend.Text);

            string highlight = $"Please make sure{Environment.NewLine}";

            foreach (var item in localProjectInfo.GetSelectedPackagesInfo(m_SelectResult.SelectedPackagesList))
            {
                highlight = $"{highlight}your change of {item.Item1} is in {item.Item2}{Environment.NewLine}";
            }

            if (!needDoubleConfirm || MessageBox.Show(highlight, "Double confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                if (CLIInstance != null)
                {
                    CLIInstance.CommandLineFormatParas["project"] = cmbLocalBranches.SelectedItem?.ToString();
                    CLIInstance.CommandLineFormatParas["append"] = (chkAppend.Visible && chkAppend.Checked).ToString().ToLower();
                    CLIInstance.CommandLineFormatParas["package"] = string.Join(",", chkPackageList.CheckedItems.Cast<string>()).TrimEnd(',');
                    CLIInstance.CommandLineFormatParas["force"] = (!needDoubleConfirm).ToString().ToLower();
                    m_CommandLine = CLIInstance.GetCommandLine();
                }

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

            this.btnOK.Enabled = false;

            if (input.LocalBranches == null || input.LocalBranches.Length == 0)
            {

            }
            else
            {
                cmbLocalBranches.Items.AddRange(input.LocalBranches);
            }


            if (cmbLocalBranches.Items.Count > 0)
            {
                cmbLocalBranches.SelectedIndex = 0;
            }

            if (!string.IsNullOrEmpty(GlobalValue.Root.SelectedProject) && cmbLocalBranches.Items.Contains(GlobalValue.Root.SelectedProject))
            {
                cmbLocalBranches.SelectedItem = GlobalValue.Root.SelectedProject;
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

        private void cmbLocalBranches_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkPackageList.Items.Clear();

            chkPackageList.Items.AddRange(input.GetProjectInfo(cmbLocalBranches.SelectedItem.ToString()).GetTotalPakages(LogNotify));

            if (defaultSelectedUploadList != null && defaultSelectedUploadList.Length != 0)
            {
                List<int> indexList = new List<int>();

                foreach (var item in chkPackageList.Items)
                {
                    if (!string.IsNullOrEmpty(defaultSelectedUploadList.FirstOrDefault(str => !string.IsNullOrEmpty(str) && item.ToString().ToUpper().StartsWith(str.ToUpper()))))
                    {
                        indexList.Add(chkPackageList.Items.IndexOf(item));
                    }
                }

                foreach (var item in indexList)
                {
                    chkPackageList.SetItemChecked(item, true);
                }

                if (chkPackageList.CheckedItems.Count != 0)
                {
                    btnOK.Enabled = true;
                }
            }
        }

        private void chkPackageList_MouseUp(object sender, MouseEventArgs e)
        {
            if (chkPackageList.CheckedItems.Count == 0)
            {
                btnOK.Enabled = false;
            }
            else
            {
                btnOK.Enabled = true;
            }
        }

        public void CLIExec()
        {
            this.Shown += frmSelectLoaclProject_Shown;
        }

        private void frmSelectLoaclProject_Shown(object sender, EventArgs e)
        {
            if (CLIInstance != null && CLIInstance.ParseSuccess && CLIInstance.FromCLI)
            {
                bool success = false;

                string project = CLIInstance.GetParameterValue("project", string.Empty);
                success = Utility.SetSelectedItem(cmbLocalBranches, project, false, true);

                if (success)
                {
                    if (chkAppend.Enabled && chkAppend.Visible)
                    {
                        chkAppend.Checked = CLIInstance.GetParameterValueBool("append", true);
                    }

                    if (CLIInstance.Parameters.ContainsKey("package"))
                    {
                        string[] packages = CLIInstance.GetParameterValue("package", string.Empty).Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Where(str => !string.IsNullOrEmpty(str?.Trim())).ToArray();

                        if (packages != null && packages.Length > 0)
                        {
                            Utility.ClearCheckedItems(chkPackageList);
                        }

                        foreach (var package in packages)
                        {
                            if (!string.IsNullOrWhiteSpace(package))
                            {
                                if (!Utility.SetSelectedItem(chkPackageList, package))
                                {
                                    LogNotify.WriteLog($"CLI Error for package:{package}, you need to manual select for {CLIInstance.CLIExecEnum}", true);
                                    return;
                                }
                            }
                        }
                    }

                    if (btnOK.Enabled)
                    {
                        needDoubleConfirm = !CLIInstance.GetParameterValueBool("force", false);

                        btnOK.PerformClick();
                    }
                }
                else
                {
                    project = string.IsNullOrEmpty(project) ? "NULL" : project;
                    LogNotify.WriteLog($"CLI Error for project:{project}, you need to manual select for {CLIInstance.CLIExecEnum}", true);
                }
            }
        }
    }
}
