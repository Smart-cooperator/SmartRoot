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
            this.label1 = new System.Windows.Forms.Label();
            this.lblPackageFolder = new System.Windows.Forms.Label();
            this.txtPackageFolder = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.lsbTotal = new System.Windows.Forms.ListBox();
            this.lsbSelected = new System.Windows.Forms.ListBox();
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
            this.lblProvisioningPorject.Anchor = System.Windows.Forms.AnchorStyles.Right;
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
            this.lblPackageName.Anchor = System.Windows.Forms.AnchorStyles.Right;
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
            this.lblSerialNumber.Anchor = System.Windows.Forms.AnchorStyles.Right;
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
            this.lblSlot.Anchor = System.Windows.Forms.AnchorStyles.Right;
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
            this.lblExec.Anchor = System.Windows.Forms.AnchorStyles.Right;
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
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(513, 182);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(124, 17);
            this.label1.TabIndex = 15;
            this.label1.Text = "Selected Task List";
            // 
            // lblPackageFolder
            // 
            this.lblPackageFolder.Anchor = System.Windows.Forms.AnchorStyles.Right;
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
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 182);
            this.label2.Name = "label2";
            this.label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label2.Size = new System.Drawing.Size(101, 17);
            this.label2.TabIndex = 19;
            this.label2.Text = "Total Task List";
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
            // frmSelectLoaclProjectForProvisioningTesterNew
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(918, 597);
            this.ControlBox = false;
            this.Controls.Add(this.lsbSelected);
            this.Controls.Add(this.lsbTotal);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtPackageFolder);
            this.Controls.Add(this.lblPackageFolder);
            this.Controls.Add(this.label1);
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
            this.MaximumSize = new System.Drawing.Size(936, 644);
            this.MinimumSize = new System.Drawing.Size(936, 644);
            this.Name = "frmSelectLoaclProjectForProvisioningTesterNew";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Provisioning Tester Info";
            this.Load += new System.EventHandler(this.frmSelectLoaclProjectForProvisioningTesterNew_Load);
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblPackageFolder;
        private System.Windows.Forms.TextBox txtPackageFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.ListBox lsbTotal;
        private System.Windows.Forms.ListBox lsbSelected;
    }
}