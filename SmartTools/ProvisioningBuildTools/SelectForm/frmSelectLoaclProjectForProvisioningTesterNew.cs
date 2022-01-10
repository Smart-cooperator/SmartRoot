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
using System.Configuration;
using System.Text.RegularExpressions;
using ProvisioningBuildTools.CLI;
using System.Xml.Linq;

namespace ProvisioningBuildTools.SelectForm
{
    public partial class frmSelectLoaclProjectForProvisioningTesterNew : Form, ISelect<SelectProvisioningTesterInfoOutput>
    {
        private SelectProvisioningTesterInfoOutput m_SelectResult;
        public SelectProvisioningTesterInfoOutput SelectResult => m_SelectResult;

        public ILogNotify LogNotify { get; set; }
        public ICommandNotify CommandNotify { get; set; }
        public AbCLIExecInstance CLIInstance { get; set; }

        private string m_CommandLine;
        public string CommandLine => m_CommandLine;

        private SelectProvisioningTesterInfoInput input;

        private Action endInvoke;
        private Action startInvoke;
        private bool needDoubleConfirm = true;

        private Dictionary<string, Dictionary<string, Func<string, string, XDocument>>> promiseCityDict;

        public frmSelectLoaclProjectForProvisioningTesterNew(ILogNotify logNotify, ICommandNotify commandNotify)
        {
            InitializeComponent();
            LogNotify = logNotify;
            CommandNotify = commandNotify;

            input = new SelectProvisioningTesterInfoInput(logNotify);

            endInvoke = new Action(() => EnableRun(true));
            startInvoke = new Action(() => EnableRun(false));

            Utility.SetDoubleBuffered(lsbTotal, true);
            Utility.SetDoubleBuffered(lsbSelected, true);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            GlobalValue.Root.SelectedProject = cmbProvisioningPorject.SelectedItem?.ToString();

            Project project = GlobalValue.Root.GetProject(GlobalValue.Root.SelectedProject);

            project = project ?? GlobalValue.Root.AddProject(GlobalValue.Root.SelectedProject);

            string packageName = cmbPackageName.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(packageName))
            {
                project.ProvisioningPackage = Path.Combine(txtPackageFolder.Text, packageName);
            }
            else
            {
                project.ProvisioningPackage = txtPackageFolder.Text;
            }

            project.SerialNumber = cmbSerialNumber.SelectedItem?.ToString();
            project.Slot = cmbSlot.SelectedItem?.ToString();
            project.TaskOpCodeList = string.Join(",", lsbSelected.Items.Cast<string>());

            string[] usuallyArray = string.IsNullOrWhiteSpace(project.UsuallyTaskOpCodeList) ? (new string[0]) : project.UsuallyTaskOpCodeList.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            project.UsuallyTaskOpCodeList = string.Join(",", lsbSelected.Items.Cast<string>().Concat(usuallyArray).Distinct(StringComparer.InvariantCulture));

            string args = null;

            ProvisioningTesterInfo provisioningTesterInfo = input.GetProvisioningTesterInfo(cmbProvisioningPorject.SelectedItem?.ToString());
            ProvisioningPackageInfo provisioningPackageInfo = provisioningTesterInfo.ProvisioningPackageList[packageName].Value;

            args = provisioningTesterInfo.ProvisioningPackageList[packageName].Value.GenerateProvisioningTesterArg(project.SerialNumber, project.Slot, rtbExec.Text.TrimEnd());

            if (File.Exists(Path.Combine(project.ProvisioningPackage, Command.ProvisioningTester)))
            {
                Dictionary<string, XDocument> selectSkuDocumentDict = new Dictionary<string, XDocument>();

                foreach (var item in chkSKUList.CheckedItems)
                {
                    selectSkuDocumentDict.Add(item.ToString(), promiseCityDict[cmbPromiseCity.Text][item.ToString()](project.SerialNumber, provisioningPackageInfo.NodeNameForSN));
                }

                m_SelectResult = new SelectProvisioningTesterInfoOutput(
                    GlobalValue.Root.SelectedProject,
                    project.ProvisioningPackage,
                    project.SerialNumber,
                    project.Slot,
                    rtbExec.Text.TrimEnd(),
                    args,
                    provisioningTesterInfo.ProvisioningPackageList[packageName].Value.UseExternalProvisioningTester,
                    selectSkuDocumentDict,
                    provisioningPackageInfo.CurrentGenealogyFile,
                    Convert.ToInt32(txtLoopCount.Text),
                    cmbPromiseCity.Text);

                string highlight = $"Please make sure{Environment.NewLine}{Environment.NewLine}ProvisioningPackage:{project.ProvisioningPackage}{Environment.NewLine}{Environment.NewLine}ProvisioningTesterArgs:{args}{Environment.NewLine}{Environment.NewLine}SKU:{Environment.NewLine}{string.Join(Environment.NewLine, m_SelectResult.SelectSkuDocumentDict.Select(pair => pair.Key))}{Environment.NewLine}{Environment.NewLine}LoopCount:{txtLoopCount.Text}";

                if (!needDoubleConfirm || MessageBox.Show(highlight, "Double confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    if (CLIInstance != null)
                    {
                        CLIInstance.CommandLineFormatParas["project"] = cmbProvisioningPorject.SelectedItem?.ToString();
                        CLIInstance.CommandLineFormatParas["package"] = cmbPackageName.SelectedItem?.ToString();
                        CLIInstance.CommandLineFormatParas["serialnumber"] = cmbSerialNumber.SelectedItem?.ToString();
                        CLIInstance.CommandLineFormatParas["slot"] = cmbSlot.SelectedItem?.ToString();
                        CLIInstance.CommandLineFormatParas["task"] = string.Join(",", lsbSelected.Items.Cast<string>()).TrimEnd(',');
                        CLIInstance.CommandLineFormatParas["promisecity"] = cmbPromiseCity.SelectedItem?.ToString();
                        CLIInstance.CommandLineFormatParas["sku"] = string.Join(",", chkSKUList.CheckedItems.Cast<string>()).TrimEnd(',');
                        CLIInstance.CommandLineFormatParas["loopcount"] = txtLoopCount.Text;
                        CLIInstance.CommandLineFormatParas["force"] = (!needDoubleConfirm).ToString().ToLower();
                        m_CommandLine = CLIInstance.GetCommandLine();
                    }

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            else
            {
                LogNotify.WriteLog($"{Command.ProvisioningTester} not found in package path {project.ProvisioningPackage}!!!", true);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void frmSelectLoaclProjectForProvisioningTesterNew_Load(object sender, EventArgs e)
        {
            //cmbLocalBranches.IntegralHeight = false;           

            if (input.LocalBranches == null || input.LocalBranches.Length == 0)
            {
                this.btnOK.Enabled = false;
            }
            else
            {
                cmbProvisioningPorject.Items.AddRange(input.LocalBranches);
            }


            Utility.SetSelectedItem(cmbProvisioningPorject, GlobalValue.Root.SelectedProject);

            int maxAvailableSlot;

            string maxAvailableSlotStr = ConfigurationManager.AppSettings["maxAvailableSlot"];

            if (!int.TryParse(maxAvailableSlotStr, out maxAvailableSlot) || maxAvailableSlot <= 0)
            {
                maxAvailableSlot = 64;
            }

            for (int i = 0; i < maxAvailableSlot; i++)
            {
                cmbSlot.Items.Add(i.ToString());
            }

            Utility.SetSelectedItem(cmbSlot, GlobalValue.Root.GetProject(GlobalValue.Root.SelectedProject)?.Slot);

            txtLoopCount.Text = "1";

            chkSKUList.CheckOnClick = true;

            IssueBtnOk();
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

        private void lsbTaskList_MouseDown(object sender, MouseEventArgs e)
        {
            ListBox lsbTaskList = sender as ListBox;
            if (lsbTaskList.SelectedItem != null)
            {
                lsbTaskList.Tag = lsbTaskList.SelectedIndex;
            }
            else
            {
                lsbTaskList.Tag = null;
            }
        }

        private void lsbTaskList_MouseUp(object sender, MouseEventArgs e)
        {
            ListBox lsbTaskList = sender as ListBox;
            int idx1 = lsbTaskList.Tag == null ? -1 : (int)lsbTaskList.Tag;

            if (lsbTaskList.SelectedItem != null && idx1 != lsbTaskList.SelectedIndex && idx1 > -1)
            {
                int idx2 = lsbTaskList.SelectedIndex;

                object lastItem = lsbTaskList.Items[idx1];

                lsbTaskList.Items.RemoveAt(idx1);
                lsbTaskList.Items.Insert(idx2, lastItem);

                lsbTaskList.SetSelected(idx2, true);

                IssueExecConetent();
                IssueBtnOk();
            }
        }

        private void IssueExecConetent()
        {
            this.rtbExec.ReadOnly = true;

            if (lsbSelected.Items.Count == 0)
            {
                this.rtbExec.Clear();
            }
            else
            {
                IEnumerable<string> checkedItems = lsbSelected.Items.Cast<string>();
                string cmd = null;

                if (!string.IsNullOrWhiteSpace(cmd = checkedItems.FirstOrDefault(item => item.ToUpper().TrimEnd() == "Cmd".ToUpper())))
                {
                    this.rtbExec.ReadOnly = false;

                    if (this.rtbExec.Text.ToUpper().TrimEnd().Contains("Cmd".ToUpper()))
                    {

                    }
                    else
                    {
                        this.rtbExec.Clear();
                        this.rtbExec.AppendText($"-{cmd.TrimEnd()} ");
                    }
                }
                else
                {
                    this.rtbExec.Clear();

                    this.rtbExec.AppendText($"-Task {string.Join(",", checkedItems)}");
                }
            }

        }

        private void IssueBtnOk()
        {
            if (string.IsNullOrWhiteSpace(cmbProvisioningPorject.SelectedItem?.ToString())
                //|| string.IsNullOrWhiteSpace(cmbPackageName.SelectedItem?.ToString())
                || cmbPackageName.SelectedItem == null
                || string.IsNullOrWhiteSpace(txtPackageFolder.Text.ToString())
                || string.IsNullOrWhiteSpace(cmbSerialNumber.SelectedItem?.ToString())
                || string.IsNullOrWhiteSpace(cmbSlot.SelectedItem?.ToString())
                 //|| string.IsNullOrWhiteSpace(rtbExec.Text.ToString())
                 )
            {
                btnOK.Enabled = false;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(rtbExec.Text.ToString()))
                {
                    btnOK.Enabled = true;
                    return;
                }

                Regex[] regices = new Regex[] { new Regex(@"-Task\s+\S+", RegexOptions.IgnoreCase), new Regex(@"-Cmd\s+\S+", RegexOptions.IgnoreCase) };

                btnOK.Enabled = false;
                foreach (var regex in regices)
                {
                    if (regex.IsMatch(rtbExec.Text))
                    {
                        btnOK.Enabled = true;
                        break;
                    }
                }
            }
        }


        private void cmbProvisioningPorject_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPackageFolder.Text = string.Empty;
            cmbPackageName.Items.Clear();
            cmbSerialNumber.Items.Clear();
            lsbTotal.Items.Clear();
            lsbSelected.Items.Clear();

            if (cmbProvisioningPorject.SelectedItem != null)
            {
                ProvisioningTesterInfo provisioningTesterInfo = input.GetProvisioningTesterInfo(cmbProvisioningPorject.SelectedItem.ToString());

                txtPackageFolder.Text = provisioningTesterInfo.LocalProjectInfo.ProvisioningPackageFolder;
                cmbPackageName.Items.AddRange(provisioningTesterInfo.ProvisioningPackageList.Keys.ToArray());
                AdjustComboBoxDropDownListWidth(cmbPackageName);

                string provisioningPackage = GlobalValue.Root.GetProject(cmbProvisioningPorject.SelectedItem?.ToString())?.ProvisioningPackage;
                string packageName = null;

                if (!string.IsNullOrEmpty(provisioningPackage) && provisioningPackage.ToUpper().Contains(txtPackageFolder.Text.ToUpper()))
                {
                    packageName = provisioningPackage.ToUpper().Replace(txtPackageFolder.Text.ToUpper(), string.Empty).Trim(new char[] { '\\', '/' });
                }

                Utility.SetSelectedItem(cmbPackageName, packageName);

                Utility.SetSelectedItem(cmbSlot, GlobalValue.Root.GetProject(cmbProvisioningPorject.SelectedItem?.ToString())?.Slot, false);
            }
        }

        private void cmbProvisioningPackage_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbSerialNumber.Items.Clear();
            lsbTotal.Items.Clear();
            lsbSelected.Items.Clear();
            cmbPromiseCity.Items.Clear();
            promiseCityDict?.Clear();

            if (cmbPackageName.SelectedItem != null)
            {
                Project project = GlobalValue.Root.GetProject(cmbProvisioningPorject.SelectedItem?.ToString());

                ProvisioningTesterInfo provisioningTesterInfo = input.GetProvisioningTesterInfo(cmbProvisioningPorject.SelectedItem.ToString());

                ProvisioningPackageInfo provisioningPackageInfo = provisioningTesterInfo.ProvisioningPackageList[cmbPackageName.SelectedItem.ToString()].Value;

                cmbSerialNumber.Items.AddRange(provisioningTesterInfo.ProvisioningPackageList[cmbPackageName.SelectedItem.ToString()].Value.SerialNumberList.ToArray());
                Utility.SetSelectedItem(cmbSerialNumber, project?.SerialNumber);

                string[] items = provisioningTesterInfo.ProvisioningPackageList[cmbPackageName.SelectedItem.ToString()].Value.TaskList.ToArray();
                string[] lastCheckedItems = project?.TaskOpCodeList.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                string[] usuallyUsedItems = project?.UsuallyTaskOpCodeList.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                Utility.SetItems(lsbTotal, lsbSelected, items, lastCheckedItems, usuallyUsedItems);

                promiseCityDict = LoopTestHelp.GetAllSkus(Path.Combine(txtPackageFolder.Text, cmbPackageName.Text), provisioningPackageInfo.CurrentGenealogyFile, LogNotify);

                if (promiseCityDict != null && promiseCityDict.Count != 0)
                {
                    cmbPromiseCity.Items.AddRange(promiseCityDict.Keys.ToArray());
                    cmbPromiseCity.SelectedIndex = 0;
                }
                else
                {
                    cmbPromiseCity.Items.Clear();
                    cmbPromiseCity.SelectedIndex = -1;
                    chkSKUList.Items.Clear();
                }

                IssueExecConetent();
                IssueBtnOk();
            }
        }

        private void AdjustComboBoxDropDownListWidth(object comboBox)
        {
            Graphics g = null;
            Font font = null;
            try
            {
                ComboBox senderComboBox = null;
                if (comboBox is ComboBox)
                    senderComboBox = (ComboBox)comboBox;
                else if (comboBox is ToolStripComboBox)
                    senderComboBox = ((ToolStripComboBox)comboBox).ComboBox;
                else
                    return;

                int width = senderComboBox.Width;
                g = senderComboBox.CreateGraphics();
                font = senderComboBox.Font;

                //checks if a scrollbar will be displayed.
                //If yes, then get its width to adjust the size of the drop down list.
                int vertScrollBarWidth =
                    (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems)
                    ? SystemInformation.VerticalScrollBarWidth : 0;

                int newWidth;
                foreach (object s in senderComboBox.Items)  //Loop through list items and check size of each items.
                {
                    if (s != null)
                    {
                        newWidth = (int)g.MeasureString(s.ToString().Trim(), font).Width
                            + vertScrollBarWidth;
                        if (width < newWidth)
                            width = newWidth;   //set the width of the drop down list to the width of the largest item.
                    }
                }
                senderComboBox.DropDownWidth = width;
            }
            catch
            { }
            finally
            {
                if (g != null)
                    g.Dispose();
            }
        }

        private void txtPackageFolder_TextChanged(object sender, EventArgs e)
        {
            IssueBtnOk();
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
                success = Utility.SetSelectedItem(cmbProvisioningPorject, project, false, true);

                if (success)
                {
                    bool requiredCompleted = true;
                    Dictionary<string, string> requiredParas = new Dictionary<string, string>();
                    requiredParas["package"] = CLIInstance.GetParameterValue("package", string.Empty);
                    requiredParas["serialnumber"] = CLIInstance.GetParameterValue("serialnumber", string.Empty);
                    requiredParas["slot"] = CLIInstance.GetParameterValue("slot", string.Empty);
                    requiredParas["task"] = CLIInstance.GetParameterValue("task", string.Empty);
                    requiredParas["promisecity"] = CLIInstance.GetParameterValue("promisecity", "NULL");
                    requiredParas["sku"] = CLIInstance.GetParameterValue("sku", string.Empty);

                    bool succcess = false;

                    foreach (var requiredPara in requiredParas)
                    {

                        switch (requiredPara.Key)
                        {
                            case "package":
                                succcess = Utility.SetSelectedItem(cmbPackageName, requiredPara.Value, false, true);
                                break;
                            case "serialnumber":
                                succcess = Utility.SetSelectedItem(cmbSerialNumber, requiredPara.Value, false, true);
                                break;
                            case "slot":
                                succcess = Utility.SetSelectedItem(cmbSlot, requiredPara.Value, false, true);
                                break;
                            case "task":
                                succcess = true | Utility.SetSelectedItem(lsbTotal, lsbSelected, requiredPara.Value.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries), true, true);
                                break;
                            case "promisecity":
                                succcess = Utility.SetSelectedItem(cmbPromiseCity, requiredPara.Value, false, true);
                                break;
                            case "sku":
                                if (string.IsNullOrEmpty(requiredPara.Value))
                                {
                                    succcess = true;
                                }
                                else
                                {
                                    IEnumerable<string> skus = Enumerable.Empty<string>();
                                    string preVersion = null;

                                    foreach (var sku in requiredPara.Value.Split(','))
                                    {
                                        if (sku.Contains("_"))
                                        {
                                            string[] temps = sku.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                                            preVersion = string.Join("_", temps.Take(temps.Length - 1));
                                            skus = skus.Append(sku);
                                        }
                                        else
                                        {
                                            skus = skus.Append($"{preVersion}_{sku}");
                                        }
                                    }

                                    succcess = Utility.SetSelectedItem(chkSKUList, skus);
                                }
                                break;
                            default:
                                break;
                        }

                        requiredCompleted = requiredCompleted && succcess;

                        if (!succcess)
                        {
                            string value = string.IsNullOrEmpty(requiredPara.Value) ? "NULL" : requiredPara.Value;
                            LogNotify.WriteLog($"CLI Error for {requiredPara.Key}:{value}, you need to manual select for {CLIInstance.CLIExecEnum}", true);
                        }
                    }

                    IssueExecConetent();
                    IssueBtnOk();

                    if (btnOK.Enabled && requiredCompleted)
                    {
                        needDoubleConfirm = !CLIInstance.GetParameterValueBool("force", false);

                        txtLoopCount.Text = CLIInstance.GetParameterValue("loopcount", "1");

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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (lsbTotal.SelectedItem != null)
            {
                lsbSelected.Items.Add(lsbTotal.SelectedItem);

                lsbSelected.SelectedIndex = lsbSelected.Items.Count - 1;

                IssueExecConetent();
                IssueBtnOk();
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lsbSelected.SelectedItem != null)
            {
                int selectedIndex = lsbSelected.SelectedIndex;

                lsbSelected.Items.RemoveAt(selectedIndex);


                if (selectedIndex < lsbSelected.Items.Count)
                {
                    lsbSelected.SelectedIndex = selectedIndex;
                }
                else
                {
                    lsbSelected.SelectedIndex = lsbSelected.Items.Count - 1;
                }

                IssueExecConetent();
                IssueBtnOk();
            }
        }

        private void txtLoopCount_Leave(object sender, EventArgs e)
        {
            int result;

            if (!int.TryParse(txtLoopCount.Text, out result))
            {
                txtLoopCount.Text = "1";
            }
            else
            {
                if (result < 1)
                {
                    txtLoopCount.Text = "1";
                }
                else
                {
                    txtLoopCount.Text = result.ToString();
                }
            }
        }

        private void cmbPromiseCity_SelectedValueChanged(object sender, EventArgs e)
        {
            chkSKUList.Items.Clear();

            if (promiseCityDict?.Count > 0)
            {
                chkSKUList.Items.AddRange(promiseCityDict[cmbPromiseCity.Text].Keys.ToArray());
            }
        }

        private void chkSKUList_MouseDown(object sender, MouseEventArgs e)
        {
            chkSKUList.Tag = chkSKUList.SelectedItem;
        }

        private void chkTaskList_MouseUp(object sender, MouseEventArgs e)
        {
            object lastItem = chkSKUList.Tag;
            object currentItem = chkSKUList.SelectedItem;

            if (currentItem != lastItem && currentItem != null && lastItem != null)
            {

                int idx1 = chkSKUList.Items.IndexOf(lastItem);
                int idx2 = chkSKUList.Items.IndexOf(currentItem);

                bool selected1 = chkSKUList.CheckedIndices.Contains(idx1);
                bool selected2 = chkSKUList.CheckedIndices.Contains(idx2);

                chkSKUList.Items.RemoveAt(idx1);
                chkSKUList.Items.Insert(idx2, lastItem);
                chkSKUList.SetItemChecked(idx2, selected1);

                if (idx1 > idx2)
                {
                    chkSKUList.SetItemChecked(idx2 + 1, !selected2);
                }
                else
                {
                    chkSKUList.SetItemChecked(idx2 - 1, !selected2);
                }

                chkSKUList.SetSelected(idx2, true);
            }
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chkSKUList.Items.Count; i++)
            {
                chkSKUList.SetItemChecked(i, true);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chkSKUList.Items.Count; i++)
            {
                chkSKUList.SetItemChecked(i, false);
            }
        }

        private void btnClr_Click(object sender, EventArgs e)
        {
            lsbSelected.Items.Clear();

            IssueExecConetent();
            IssueBtnOk();
        }
    }
}
