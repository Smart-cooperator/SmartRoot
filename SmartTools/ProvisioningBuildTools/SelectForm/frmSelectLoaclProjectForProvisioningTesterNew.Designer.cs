namespace ProvisioningBuildTools.SelectForm
{
    partial class frmSelectLoaclProjectForProvisioningTesterNew
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cmbProvisioningPorject = new System.Windows.Forms.ComboBox();
            this.lblProvisioningPorject = new System.Windows.Forms.Label();
            this.lblPackageName = new System.Windows.Forms.Label();
            this.cmbPackageName = new System.Windows.Forms.ComboBox();
            this.lblSerialNumber = new System.Windows.Forms.Label();
            this.cmbSerialNumber = new System.Windows.Forms.ComboBox();
            this.lblSlot = new System.Windows.Forms.Label();
            this.cmbSlot = new System.Windows.Forms.ComboBox();
            this.lblExec = new System.Windows.Forms.Label();
            this.rtbExec = new System.Windows.Forms.RichTextBox();
            this.lblSelectedTaskList = new System.Windows.Forms.Label();
            this.lblPackageFolder = new System.Windows.Forms.Label();
            this.txtPackageFolder = new System.Windows.Forms.TextBox();
            this.lblTotalTaskList = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.lsbTotal = new System.Windows.Forms.ListBox();
            this.lsbSelected = new System.Windows.Forms.ListBox();
            this.grpLoopTest = new System.Windows.Forms.GroupBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnAll = new System.Windows.Forms.Button();
            this.txtLoopCount = new System.Windows.Forms.TextBox();
            this.chkSKUList = new System.Windows.Forms.CheckedListBox();
            this.lblSelectedSKUList = new System.Windows.Forms.Label();
            this.cmbPromiseCity = new System.Windows.Forms.ComboBox();
            this.lblPromiseCity = new System.Windows.Forms.Label();
            this.lblLoopCount = new System.Windows.Forms.Label();
            this.btnClr = new System.Windows.Forms.Button();
            this.grpLoopTest.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(328, 562);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 30);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(508, 562);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cmbProvisioningPorject
            // 
            this.cmbProvisioningPorject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProvisioningPorject.FormattingEnabled = true;
            this.cmbProvisioningPorject.IntegralHeight = false;
            this.cmbProvisioningPorject.Location = new System.Drawing.Point(228, 10);
            this.cmbProvisioningPorject.Name = "cmbProvisioningPorject";
            this.cmbProvisioningPorject.Size = new System.Drawing.Size(173, 24);
            this.cmbProvisioningPorject.TabIndex = 2;
            this.cmbProvisioningPorject.SelectedIndexChanged += new System.EventHandler(this.cmbProvisioningPorject_SelectedIndexChanged);
            // 
            // lblProvisioningPorject
            // 
            this.lblProvisioningPorject.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProvisioningPorject.AutoSize = true;
            this.lblProvisioningPorject.Location = new System.Drawing.Point(30, 10);
            this.lblProvisioningPorject.Name = "lblProvisioningPorject";
            this.lblProvisioningPorject.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblProvisioningPorject.Size = new System.Drawing.Size(129, 17);
            this.lblProvisioningPorject.TabIndex = 3;
            this.lblProvisioningPorject.Text = "ProvisioningPorject";
            // 
            // lblPackageName
            // 
            this.lblPackageName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPackageName.AutoSize = true;
            this.lblPackageName.Location = new System.Drawing.Point(30, 96);
            this.lblPackageName.Name = "lblPackageName";
            this.lblPackageName.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblPackageName.Size = new System.Drawing.Size(100, 17);
            this.lblPackageName.TabIndex = 5;
            this.lblPackageName.Text = "PackageName";
            // 
            // cmbPackageName
            // 
            this.cmbPackageName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPackageName.FormattingEnabled = true;
            this.cmbPackageName.IntegralHeight = false;
            this.cmbPackageName.Location = new System.Drawing.Point(228, 96);
            this.cmbPackageName.Name = "cmbPackageName";
            this.cmbPackageName.Size = new System.Drawing.Size(655, 24);
            this.cmbPackageName.TabIndex = 4;
            this.cmbPackageName.SelectedIndexChanged += new System.EventHandler(this.cmbProvisioningPackage_SelectedIndexChanged);
            // 
            // lblSerialNumber
            // 
            this.lblSerialNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSerialNumber.AutoSize = true;
            this.lblSerialNumber.Location = new System.Drawing.Point(30, 141);
            this.lblSerialNumber.Name = "lblSerialNumber";
            this.lblSerialNumber.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblSerialNumber.Size = new System.Drawing.Size(94, 17);
            this.lblSerialNumber.TabIndex = 7;
            this.lblSerialNumber.Text = "SerialNumber";
            // 
            // cmbSerialNumber
            // 
            this.cmbSerialNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSerialNumber.FormattingEnabled = true;
            this.cmbSerialNumber.IntegralHeight = false;
            this.cmbSerialNumber.Location = new System.Drawing.Point(228, 141);
            this.cmbSerialNumber.Name = "cmbSerialNumber";
            this.cmbSerialNumber.Size = new System.Drawing.Size(173, 24);
            this.cmbSerialNumber.TabIndex = 6;
            // 
            // lblSlot
            // 
            this.lblSlot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSlot.AutoSize = true;
            this.lblSlot.Location = new System.Drawing.Point(513, 141);
            this.lblSlot.Name = "lblSlot";
            this.lblSlot.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblSlot.Size = new System.Drawing.Size(32, 17);
            this.lblSlot.TabIndex = 9;
            this.lblSlot.Text = "Slot";
            // 
            // cmbSlot
            // 
            this.cmbSlot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSlot.FormattingEnabled = true;
            this.cmbSlot.IntegralHeight = false;
            this.cmbSlot.Location = new System.Drawing.Point(710, 141);
            this.cmbSlot.Name = "cmbSlot";
            this.cmbSlot.Size = new System.Drawing.Size(173, 24);
            this.cmbSlot.TabIndex = 8;
            // 
            // lblExec
            // 
            this.lblExec.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblExec.AutoSize = true;
            this.lblExec.Location = new System.Drawing.Point(101, 479);
            this.lblExec.Name = "lblExec";
            this.lblExec.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblExec.Size = new System.Drawing.Size(38, 17);
            this.lblExec.TabIndex = 13;
            this.lblExec.Text = "Exec";
            // 
            // rtbExec
            // 
            this.rtbExec.Location = new System.Drawing.Point(145, 479);
            this.rtbExec.Name = "rtbExec";
            this.rtbExec.Size = new System.Drawing.Size(657, 77);
            this.rtbExec.TabIndex = 14;
            this.rtbExec.Text = "";
            // 
            // lblSelectedTaskList
            // 
            this.lblSelectedTaskList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSelectedTaskList.AutoSize = true;
            this.lblSelectedTaskList.Location = new System.Drawing.Point(513, 182);
            this.lblSelectedTaskList.Name = "lblSelectedTaskList";
            this.lblSelectedTaskList.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblSelectedTaskList.Size = new System.Drawing.Size(124, 17);
            this.lblSelectedTaskList.TabIndex = 15;
            this.lblSelectedTaskList.Text = "Selected Task List";
            // 
            // lblPackageFolder
            // 
            this.lblPackageFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPackageFolder.AutoSize = true;
            this.lblPackageFolder.Location = new System.Drawing.Point(30, 56);
            this.lblPackageFolder.Name = "lblPackageFolder";
            this.lblPackageFolder.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblPackageFolder.Size = new System.Drawing.Size(103, 17);
            this.lblPackageFolder.TabIndex = 16;
            this.lblPackageFolder.Text = "PackageFolder";
            // 
            // txtPackageFolder
            // 
            this.txtPackageFolder.Location = new System.Drawing.Point(228, 56);
            this.txtPackageFolder.Name = "txtPackageFolder";
            this.txtPackageFolder.ReadOnly = true;
            this.txtPackageFolder.Size = new System.Drawing.Size(655, 22);
            this.txtPackageFolder.TabIndex = 17;
            this.txtPackageFolder.TextChanged += new System.EventHandler(this.txtPackageFolder_TextChanged);
            // 
            // lblTotalTaskList
            // 
            this.lblTotalTaskList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalTaskList.AutoSize = true;
            this.lblTotalTaskList.Location = new System.Drawing.Point(30, 182);
            this.lblTotalTaskList.Name = "lblTotalTaskList";
            this.lblTotalTaskList.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblTotalTaskList.Size = new System.Drawing.Size(101, 17);
            this.lblTotalTaskList.TabIndex = 19;
            this.lblTotalTaskList.Text = "Total Task List";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(420, 257);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(82, 37);
            this.btnAdd.TabIndex = 20;
            this.btnAdd.Text = ">>";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(420, 342);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(82, 37);
            this.btnRemove.TabIndex = 21;
            this.btnRemove.Text = "<<";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // lsbTotal
            // 
            this.lsbTotal.FormattingEnabled = true;
            this.lsbTotal.ItemHeight = 16;
            this.lsbTotal.Location = new System.Drawing.Point(33, 202);
            this.lsbTotal.Name = "lsbTotal";
            this.lsbTotal.Size = new System.Drawing.Size(367, 244);
            this.lsbTotal.TabIndex = 22;
            this.lsbTotal.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lsbTaskList_MouseDown);
            this.lsbTotal.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lsbTaskList_MouseUp);
            // 
            // lsbSelected
            // 
            this.lsbSelected.FormattingEnabled = true;
            this.lsbSelected.ItemHeight = 16;
            this.lsbSelected.Location = new System.Drawing.Point(516, 202);
            this.lsbSelected.Name = "lsbSelected";
            this.lsbSelected.Size = new System.Drawing.Size(367, 244);
            this.lsbSelected.TabIndex = 23;
            this.lsbSelected.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lsbTaskList_MouseDown);
            this.lsbSelected.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lsbTaskList_MouseUp);
            // 
            // grpLoopTest
            // 
            this.grpLoopTest.BackColor = System.Drawing.SystemColors.Info;
            this.grpLoopTest.Controls.Add(this.btnClear);
            this.grpLoopTest.Controls.Add(this.btnAll);
            this.grpLoopTest.Controls.Add(this.txtLoopCount);
            this.grpLoopTest.Controls.Add(this.chkSKUList);
            this.grpLoopTest.Controls.Add(this.lblSelectedSKUList);
            this.grpLoopTest.Controls.Add(this.cmbPromiseCity);
            this.grpLoopTest.Controls.Add(this.lblPromiseCity);
            this.grpLoopTest.Controls.Add(this.lblLoopCount);
            this.grpLoopTest.Location = new System.Drawing.Point(906, 12);
            this.grpLoopTest.Name = "grpLoopTest";
            this.grpLoopTest.Size = new System.Drawing.Size(450, 580);
            this.grpLoopTest.TabIndex = 24;
            this.grpLoopTest.TabStop = false;
            this.grpLoopTest.Text = "Loop Test Option";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(326, 130);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 20;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnAll
            // 
            this.btnAll.Location = new System.Drawing.Point(210, 130);
            this.btnAll.Name = "btnAll";
            this.btnAll.Size = new System.Drawing.Size(75, 23);
            this.btnAll.TabIndex = 19;
            this.btnAll.Text = "All";
            this.btnAll.UseVisualStyleBackColor = true;
            this.btnAll.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // txtLoopCount
            // 
            this.txtLoopCount.Location = new System.Drawing.Point(152, 43);
            this.txtLoopCount.Name = "txtLoopCount";
            this.txtLoopCount.Size = new System.Drawing.Size(121, 22);
            this.txtLoopCount.TabIndex = 18;
            this.txtLoopCount.Leave += new System.EventHandler(this.txtLoopCount_Leave);
            // 
            // chkSKUList
            // 
            this.chkSKUList.FormattingEnabled = true;
            this.chkSKUList.Location = new System.Drawing.Point(36, 156);
            this.chkSKUList.Name = "chkSKUList";
            this.chkSKUList.Size = new System.Drawing.Size(386, 412);
            this.chkSKUList.TabIndex = 17;
            this.chkSKUList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chkSKUList_MouseDown);
            this.chkSKUList.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chkTaskList_MouseUp);
            // 
            // lblSelectedSKUList
            // 
            this.lblSelectedSKUList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSelectedSKUList.AutoSize = true;
            this.lblSelectedSKUList.Location = new System.Drawing.Point(33, 136);
            this.lblSelectedSKUList.Name = "lblSelectedSKUList";
            this.lblSelectedSKUList.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblSelectedSKUList.Size = new System.Drawing.Size(121, 17);
            this.lblSelectedSKUList.TabIndex = 16;
            this.lblSelectedSKUList.Text = "Selected SKU List";
            // 
            // cmbPromiseCity
            // 
            this.cmbPromiseCity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPromiseCity.FormattingEnabled = true;
            this.cmbPromiseCity.Location = new System.Drawing.Point(152, 84);
            this.cmbPromiseCity.Name = "cmbPromiseCity";
            this.cmbPromiseCity.Size = new System.Drawing.Size(121, 24);
            this.cmbPromiseCity.TabIndex = 3;
            this.cmbPromiseCity.SelectedValueChanged += new System.EventHandler(this.cmbPromiseCity_SelectedValueChanged);
            // 
            // lblPromiseCity
            // 
            this.lblPromiseCity.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPromiseCity.AutoSize = true;
            this.lblPromiseCity.Location = new System.Drawing.Point(33, 84);
            this.lblPromiseCity.Name = "lblPromiseCity";
            this.lblPromiseCity.Size = new System.Drawing.Size(86, 17);
            this.lblPromiseCity.TabIndex = 2;
            this.lblPromiseCity.Text = "Promise City";
            // 
            // lblLoopCount
            // 
            this.lblLoopCount.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLoopCount.AutoSize = true;
            this.lblLoopCount.Location = new System.Drawing.Point(33, 44);
            this.lblLoopCount.Name = "lblLoopCount";
            this.lblLoopCount.Size = new System.Drawing.Size(81, 17);
            this.lblLoopCount.TabIndex = 0;
            this.lblLoopCount.Text = "Loop Count";
            // 
            // btnClr
            // 
            this.btnClr.Location = new System.Drawing.Point(782, 176);
            this.btnClr.Name = "btnClr";
            this.btnClr.Size = new System.Drawing.Size(75, 23);
            this.btnClr.TabIndex = 25;
            this.btnClr.Text = "Clear";
            this.btnClr.UseVisualStyleBackColor = true;
            this.btnClr.Click += new System.EventHandler(this.btnClr_Click);
            // 
            // frmSelectLoaclProjectForProvisioningTesterNew
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1368, 597);
            this.ControlBox = false;
            this.Controls.Add(this.btnClr);
            this.Controls.Add(this.grpLoopTest);
            this.Controls.Add(this.lsbSelected);
            this.Controls.Add(this.lsbTotal);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.lblTotalTaskList);
            this.Controls.Add(this.txtPackageFolder);
            this.Controls.Add(this.lblPackageFolder);
            this.Controls.Add(this.lblSelectedTaskList);
            this.Controls.Add(this.rtbExec);
            this.Controls.Add(this.lblExec);
            this.Controls.Add(this.lblSlot);
            this.Controls.Add(this.cmbSlot);
            this.Controls.Add(this.lblSerialNumber);
            this.Controls.Add(this.cmbSerialNumber);
            this.Controls.Add(this.lblPackageName);
            this.Controls.Add(this.cmbPackageName);
            this.Controls.Add(this.lblProvisioningPorject);
            this.Controls.Add(this.cmbProvisioningPorject);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.MaximumSize = new System.Drawing.Size(1386, 644);
            this.MinimumSize = new System.Drawing.Size(1386, 644);
            this.Name = "frmSelectLoaclProjectForProvisioningTesterNew";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Provisioning Tester Info";
            this.Load += new System.EventHandler(this.frmSelectLoaclProjectForProvisioningTesterNew_Load);
            this.grpLoopTest.ResumeLayout(false);
            this.grpLoopTest.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmbProvisioningPorject;
        private System.Windows.Forms.Label lblProvisioningPorject;
        private System.Windows.Forms.Label lblPackageName;
        private System.Windows.Forms.ComboBox cmbPackageName;
        private System.Windows.Forms.Label lblSerialNumber;
        private System.Windows.Forms.ComboBox cmbSerialNumber;
        private System.Windows.Forms.Label lblSlot;
        private System.Windows.Forms.ComboBox cmbSlot;
        private System.Windows.Forms.Label lblExec;
        private System.Windows.Forms.RichTextBox rtbExec;
        private System.Windows.Forms.Label lblSelectedTaskList;
        private System.Windows.Forms.Label lblPackageFolder;
        private System.Windows.Forms.TextBox txtPackageFolder;
        private System.Windows.Forms.Label lblTotalTaskList;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.ListBox lsbTotal;
        private System.Windows.Forms.ListBox lsbSelected;
        private System.Windows.Forms.GroupBox grpLoopTest;
        private System.Windows.Forms.Label lblLoopCount;
        private System.Windows.Forms.Label lblPromiseCity;
        private System.Windows.Forms.Label lblSelectedSKUList;
        private System.Windows.Forms.ComboBox cmbPromiseCity;
        private System.Windows.Forms.CheckedListBox chkSKUList;
        private System.Windows.Forms.TextBox txtLoopCount;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnAll;
        private System.Windows.Forms.Button btnClr;
    }
}