namespace ProvisioningBuildTools.SelectForm
{
    partial class frmSelectLoaclProjectForUPT
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
            this.cmbLocalBranches = new System.Windows.Forms.ComboBox();
            this.lblReposFolder = new System.Windows.Forms.Label();
            this.chkPackageList = new System.Windows.Forms.CheckedListBox();
            this.chkAppend = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(207, 275);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 30);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(350, 275);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cmbLocalBranches
            // 
            this.cmbLocalBranches.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLocalBranches.FormattingEnabled = true;
            this.cmbLocalBranches.IntegralHeight = false;
            this.cmbLocalBranches.Location = new System.Drawing.Point(350, 35);
            this.cmbLocalBranches.Name = "cmbLocalBranches";
            this.cmbLocalBranches.Size = new System.Drawing.Size(185, 24);
            this.cmbLocalBranches.TabIndex = 2;
            this.cmbLocalBranches.SelectedIndexChanged += new System.EventHandler(this.cmbLocalBranches_SelectedIndexChanged);
            // 
            // lblReposFolder
            // 
            this.lblReposFolder.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblReposFolder.AutoSize = true;
            this.lblReposFolder.Location = new System.Drawing.Point(130, 35);
            this.lblReposFolder.Name = "lblReposFolder";
            this.lblReposFolder.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblReposFolder.Size = new System.Drawing.Size(163, 17);
            this.lblReposFolder.TabIndex = 3;
            this.lblReposFolder.Text = "LocalProvisioningPorject";
            // 
            // chkPackageList
            // 
            this.chkPackageList.CheckOnClick = true;
            this.chkPackageList.FormattingEnabled = true;
            this.chkPackageList.Location = new System.Drawing.Point(172, 142);
            this.chkPackageList.Name = "chkPackageList";
            this.chkPackageList.Size = new System.Drawing.Size(312, 123);
            this.chkPackageList.TabIndex = 4;
            this.chkPackageList.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chkPackageList_MouseUp);
            // 
            // chkAppend
            // 
            this.chkAppend.AutoSize = true;
            this.chkAppend.Checked = true;
            this.chkAppend.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAppend.Location = new System.Drawing.Point(133, 90);
            this.chkAppend.Name = "chkAppend";
            this.chkAppend.Size = new System.Drawing.Size(414, 21);
            this.chkAppend.TabIndex = 5;
            this.chkAppend.Text = "Append: UpdateExternalDrops + RebuildAll + CreatePackage";
            this.chkAppend.UseVisualStyleBackColor = true;
            // 
            // frmSelectLoaclProjectForUPT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 317);
            this.ControlBox = false;
            this.Controls.Add(this.chkAppend);
            this.Controls.Add(this.chkPackageList);
            this.Controls.Add(this.lblReposFolder);
            this.Controls.Add(this.cmbLocalBranches);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.MaximumSize = new System.Drawing.Size(668, 364);
            this.MinimumSize = new System.Drawing.Size(668, 364);
            this.Name = "frmSelectLoaclProjectForUPT";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Loacl Project for Upload Package to Nuget";
            this.Load += new System.EventHandler(this.frmSelectLoaclBranchForUPT_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmbLocalBranches;
        private System.Windows.Forms.Label lblReposFolder;
        private System.Windows.Forms.CheckedListBox chkPackageList;
        private System.Windows.Forms.CheckBox chkAppend;
    }
}