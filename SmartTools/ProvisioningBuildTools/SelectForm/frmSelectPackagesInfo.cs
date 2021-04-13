using ProvisioningBuildTools.SelectInput;
using ProvisioningBuildTools.SelectOutput;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProvisioningBuildTools;
using ProvisioningBuildTools.CLI;

namespace ProvisioningBuildTools.SelectForm
{
    public partial class frmSelectPackagesInfo : Form, ISelect<SelectPackagesInfoOutput>
    {
        private SelectPackagesInfoOutput m_SelectResult;
        public SelectPackagesInfoOutput SelectResult => m_SelectResult;

        public ILogNotify LogNotify { get; set; }
        public ICommandNotify CommandNotify { get; set; }
        public AbCLIExecInstance CLIInstance { get; set; }

        private string m_CommandLine;
        public string CommandLine => m_CommandLine;

        private SelectPackagesInfoInput input = new SelectPackagesInfoInput();

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
            int errorCol = -1;
            int errorRow = -1;

            if (dgvPackages.Rows.Count == 0)
            {
                return;
            }

            for (int i = 0; i < dgvPackages.Rows.Count; i++)
            {
                for (int j = 0; j < dgvPackages.Rows[i].Cells.Count; j++)
                {
                    if (dgvPackages.Rows[i].Cells[j].Value == null || string.IsNullOrWhiteSpace(dgvPackages.Rows[i].Cells[j].Value.ToString()))
                    {
                        errorCol = j;
                        errorRow = i;
                        MessageBox.Show($"Row:{errorRow},Col:{errorCol} is an empty vaule", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            m_SelectResult = new SelectPackagesInfoOutput(input.Packages);

            if (CLIInstance != null)
            {
                CLIInstance.CommandLineFormatParas["package"] = $"\"{string.Join(",", input.Packages.Select(package => package.ToString()))}\"";
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

        private void frmSelectPackagesInfo_Load(object sender, EventArgs e)
        {
            this.dgvPackages.AutoGenerateColumns = false;
            this.input.Packages.Add(new Package());
            this.dgvPackages.DataSource = input.Packages;
            this.dgvPackages.CurrentCellDirtyStateChanged += dgvPackages_CurrentCellDirtyStateChanged;
            this.btnConvert.Enabled = false;
            this.dgvPackages.RowHeadersVisible = false;
            this.dgvPackages.Columns[0].Width = (this.dgvPackages.Width - 5) * 4 / 7;
            this.dgvPackages.Columns[1].Width = (this.dgvPackages.Width - 5) * 3 / 14;
            this.dgvPackages.Columns[2].Width = (this.dgvPackages.Width - 5) * 3 / 14;
            this.dgvPackages.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvPackages.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvPackages.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dgvPackages.AllowUserToAddRows = false;
            this.dgvPackages.AllowUserToOrderColumns = false;
            this.dgvPackages.AllowUserToResizeColumns = false;
            this.dgvPackages.AllowUserToResizeRows = false;
            this.dgvPackages.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            input.Packages.Clear();
            RefreshDataSource();
            this.txtInput.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
            txtInput.Font = new Font(txtInput.Font.FontFamily, (float)9);
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
                this.Invoke(new Action<bool>(EnableRun), enable);
            }
            else
            {
                this.Enabled = enable;
            }

        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            bool canConvert = false;
            Package package = null;
            List<Package> packages = new List<Package>();
            string id = null;
            string version = null;
            string source = null;
            this.txtInput.Tag = null;

            if (!string.IsNullOrWhiteSpace(txtInput.Text.Trim()))
            {
                string[] patterns = new string[] { @"Install-Package\s+(?<Id>\S+)\s+-[Vv]ersion\s+(?<Version>\S+)",
                                                   @"Install-SurfacePackage\s+(?<Id>\S+)\s+-[Vv]ersion\s+(?<Version>\S+)\s+-[Ss]ource\s+(?<Source>\S+)",
                                                   @"\[[Ff]:(?<Source>\S+)]\[[Pp]:(?<Id>\S+)]\[[Vv]:(?<Version>\S+?)]",
                                                   @"(?<Id>\S+)\s+(?<Source>\S+)\s+(?<Version>\S+)",
                                                   @"(?<Id>\S+)\s+(?<Version>\S+)"};
                string[] sourcePatterns = new string[] { @"Devices\.(?<Source>\S+?)\.Driver", @"Microsoft\.(?<Source>\S+?)\." };

                string[] inputs = txtInput.Text.Trim().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Where(str => !string.IsNullOrWhiteSpace(str)).Select(str => str.Trim()).ToArray();

                foreach (var input in inputs)
                {
                    for (int i = 0; i < patterns.Length; i++)
                    {
                        string pattern = patterns[i];

                        Regex regex = new Regex(pattern);

                        Match ma = regex.Match(input);

                        if (ma.Success)
                        {
                            if (ma.Groups.Count == 3)
                            {
                                id = ma.Groups["Id"].Value;
                                version = ma.Groups["Version"].Value;
                                Group sourceGroup = ma.Groups["Source"];

                                if (sourceGroup.Success)
                                {
                                    source = sourceGroup.Value;
                                }
                                else
                                {
                                    foreach (var sourcePattern in sourcePatterns)
                                    {
                                        Regex sourceRegex = new Regex(sourcePattern);

                                        Match sourceMa = sourceRegex.Match(id);

                                        if (sourceMa.Success)
                                        {
                                            source = sourceMa.Groups["Source"].Value;
                                        }
                                    }
                                }
                            }
                            else if (ma.Groups.Count == 4)
                            {
                                id = ma.Groups["Id"].Value;
                                version = ma.Groups["Version"].Value;
                                source = ma.Groups["Source"].Value;
                            }
                            else if (ma.Groups.Count == 2)
                            {
                                id = ma.Groups["Id"].Value;
                                version = ma.Groups["Version"].Value;

                                foreach (var sourcePattern in sourcePatterns)
                                {
                                    Regex sourceRegex = new Regex(sourcePattern);

                                    Match sourceMa = sourceRegex.Match(id);

                                    if (sourceMa.Success)
                                    {
                                        source = sourceMa.Groups["Source"].Value;
                                    }
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(source) && !string.IsNullOrWhiteSpace(version))
                            {
                                package = new Package(id.Trim(), source.Trim(), version.Trim());

                                packages.Add(package);
                            }

                            break;
                        }
                    }
                }
            }

            if (packages != null && packages.Count != 0)
            {
                this.txtInput.Tag = packages;
                canConvert = true;
            }

            this.btnConvert.Enabled = canConvert;
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            List<Package> packages = this.txtInput.Tag as List<Package>;

            if (packages != null)
            {
                this.input.Packages.AddRange(packages);

                RefreshDataSource();
            }

            this.btnConvert.Enabled = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            this.input.Packages.Add(new Package());
            RefreshDataSource();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (this.dgvPackages.CurrentRow != null)
            {
                int currentRow = this.dgvPackages.Rows.IndexOf(this.dgvPackages.CurrentRow);
                this.input.Packages.RemoveAt(currentRow);
                RefreshDataSource();
            }
        }

        private void RefreshDataSource()
        {
            this.input.Packages = this.input.Packages.Distinct(new Package()).ToList();
            this.dgvPackages.DataSource = null;
            this.dgvPackages.DataSource = this.input.Packages;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.txtInput.Clear();
        }

        public void CLIExec()
        {
            this.Shown += frmSelectLoaclProject_Shown;
        }

        private void frmSelectLoaclProject_Shown(object sender, EventArgs e)
        {
            if (CLIInstance != null && CLIInstance.ParseSuccess && CLIInstance.FromCLI)
            {
                string package = CLIInstance.GetParameterValue("package");

                if (!string.IsNullOrEmpty(package))
                {
                    txtInput.Text = package.Replace(",", Environment.NewLine).Replace(";", Environment.NewLine).TrimEnd(Environment.NewLine.ToArray());

                    if (btnConvert.Enabled)
                    {
                        btnConvert.PerformClick();

                        if (btnOK.Enabled)
                        {
                            btnOK.PerformClick();
                        }
                    }
                    else
                    {
                        LogNotify.WriteLog($"CLI Error,unsupported package format for {CLIInstance.CLIExecEnum}", true);
                    }
                }
                else
                {
                    LogNotify.WriteLog($"CLI Error, you need to manual select for {CLIInstance.CLIExecEnum}", true);
                }
            }
        }
    }
}
