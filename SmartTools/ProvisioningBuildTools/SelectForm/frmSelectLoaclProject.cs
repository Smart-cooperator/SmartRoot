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
using ProvisioningBuildTools.Global;
using ProvisioningBuildTools.CLI;
using System.Threading;

namespace ProvisioningBuildTools.SelectForm
{
    public partial class frmSelectLoaclProject : Form, ISelect<SelectLocalProjectOutput>
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

        private ExecEnum execEnum;


        public frmSelectLoaclProject(ILogNotify logNotify, ICommandNotify commandNotify, ExecEnum execEnum)
        {
            InitializeComponent();
            LogNotify = logNotify;
            CommandNotify = commandNotify;

            input = new SelectLocalProjectInput(logNotify);

            endInvoke = new Action(() => EnableRun(true));
            startInvoke = new Action(() => EnableRun(false));

            this.execEnum = execEnum;
            bool visible = false;

            switch (execEnum)
            {
                case ExecEnum.OpenLocalProject:
                    break;
                case ExecEnum.BuildLocalProject:
                    chkAppend.Text = "Append: CreatePackage";
                    chkAppend.Visible = true;
                    visible = true;
                    break;
                case ExecEnum.CreatePackage:
                    break;
                case ExecEnum.UpdateExternalDrops:
                    chkAppend.Text = "Append: RebuildAll + CreatePackage";
                    chkAppend.Visible = true;
                    visible = true;
                    break;
                case ExecEnum.GetRemoteProject:
                    break;
                //case ExecEnum.PostBuildPackage:
                //    break;
                case ExecEnum.InstallSurfacePackage:
                    break;
                case ExecEnum.UploadNugetPackage:
                    break;
                case ExecEnum.CapsuleParser:

                    break;
                default:
                    break;
            }

            if (!visible)
            {
                this.MinimumSize = new Size(this.Size.Width, this.Size.Height - 30);
                this.MaximumSize = new Size(this.Size.Width, this.Size.Height - 30);
                this.Size = new Size(this.Size.Width, this.Size.Height - 30);

                this.btnOK.Location = new Point(this.btnOK.Location.X, this.btnOK.Location.Y - 30);
                this.btnCancel.Location = new Point(this.btnCancel.Location.X, this.btnCancel.Location.Y - 30);

                this.lblReposFolder.Location = new Point(this.lblReposFolder.Location.X, this.cmbLocalBranches.Location.Y);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            GlobalValue.Root.SelectedProject = cmbLocalBranches.SelectedItem?.ToString();

            LocalProjectInfo localProjectInfo = input.GetProjectInfo(cmbLocalBranches.SelectedItem?.ToString());

            if (File.Exists(Path.Combine(localProjectInfo.SourceFolder, Command.REPOSSLN)))
            {
                m_SelectResult = new SelectLocalProjectOutput(localProjectInfo, chkAppend.Visible && chkAppend.Checked, chkAppend.Text);

                if (CLIInstance != null)
                {
                    CLIInstance.CommandLineFormatParas["project"] = cmbLocalBranches.SelectedItem?.ToString();
                    CLIInstance.CommandLineFormatParas["append"] = (chkAppend.Visible && chkAppend.Checked).ToString().ToLower();
                    m_CommandLine = CLIInstance.GetCommandLine();
                }

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
                string[] allowedProjects = input.LocalBranches;

                if (allowedProjects.Length == 0)
                {
                    this.btnOK.Enabled = false;
                }

                cmbLocalBranches.Items.AddRange(allowedProjects);
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

                    if (btnOK.Enabled)
                    {
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
