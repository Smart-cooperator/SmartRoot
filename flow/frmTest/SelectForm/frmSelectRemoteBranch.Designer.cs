namespace ProvisioningBuildTools.SelectForm
{
    partial class frmSelectRemoteBranch
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
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.lblProject = new System.Windows.Forms.Label();
            this.lblProductBranch = new System.Windows.Forms.Label();
            this.cmbProject = new System.Windows.Forms.ComboBox();
            this.lblTag = new System.Windows.Forms.Label();
            this.lblLastModifiedTime = new System.Windows.Forms.Label();
            this.txtTag = new System.Windows.Forms.TextBox();
            this.txtLastModifiedTime = new System.Windows.Forms.TextBox();
            this.lblNewBranchName = new System.Windows.Forms.Label();
            this.txtNewBranchName = new System.Windows.Forms.TextBox();
            this.txtLocalBranchName = new System.Windows.Forms.TextBox();
            this.lblLocalBranchName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(203, 322);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(342, 322);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cmbBranch
            // 
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.IntegralHeight = false;
            this.cmbBranch.Location = new System.Drawing.Point(333, 71);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(192, 24);
            this.cmbBranch.TabIndex = 2;
            // 
            // lblProject
            // 
            this.lblProject.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblProject.AutoSize = true;
            this.lblProject.Location = new System.Drawing.Point(232, 19);
            this.lblProject.Name = "lblProject";
            this.lblProject.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblProject.Size = new System.Drawing.Size(52, 17);
            this.lblProject.TabIndex = 3;
            this.lblProject.Text = "Project";
            // 
            // lblProductBranch
            // 
            this.lblProductBranch.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblProductBranch.AutoSize = true;
            this.lblProductBranch.Location = new System.Drawing.Point(178, 71);
            this.lblProductBranch.Name = "lblProductBranch";
            this.lblProductBranch.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblProductBranch.Size = new System.Drawing.Size(106, 17);
            this.lblProductBranch.TabIndex = 4;
            this.lblProductBranch.Text = "Product Branch";
            // 
            // cmbProject
            // 
            this.cmbProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProject.FormattingEnabled = true;
            this.cmbProject.IntegralHeight = false;
            this.cmbProject.Location = new System.Drawing.Point(333, 19);
            this.cmbProject.Name = "cmbProject";
            this.cmbProject.Size = new System.Drawing.Size(84, 24);
            this.cmbProject.TabIndex = 5;
            // 
            // lblTag
            // 
            this.lblTag.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblTag.AutoSize = true;
            this.lblTag.Location = new System.Drawing.Point(251, 121);
            this.lblTag.Name = "lblTag";
            this.lblTag.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblTag.Size = new System.Drawing.Size(33, 17);
            this.lblTag.TabIndex = 6;
            this.lblTag.Text = "Tag";
            // 
            // lblLastModifiedTime
            // 
            this.lblLastModifiedTime.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblLastModifiedTime.AutoSize = true;
            this.lblLastModifiedTime.Location = new System.Drawing.Point(165, 175);
            this.lblLastModifiedTime.Name = "lblLastModifiedTime";
            this.lblLastModifiedTime.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblLastModifiedTime.Size = new System.Drawing.Size(119, 17);
            this.lblLastModifiedTime.TabIndex = 7;
            this.lblLastModifiedTime.Text = "LastModifiedTime";
            // 
            // txtTag
            // 
            this.txtTag.Location = new System.Drawing.Point(333, 121);
            this.txtTag.Name = "txtTag";
            this.txtTag.ReadOnly = true;
            this.txtTag.Size = new System.Drawing.Size(192, 22);
            this.txtTag.TabIndex = 8;
            // 
            // txtLastModifiedTime
            // 
            this.txtLastModifiedTime.Location = new System.Drawing.Point(333, 175);
            this.txtLastModifiedTime.Name = "txtLastModifiedTime";
            this.txtLastModifiedTime.ReadOnly = true;
            this.txtLastModifiedTime.Size = new System.Drawing.Size(192, 22);
            this.txtLastModifiedTime.TabIndex = 9;
            // 
            // lblNewBranchName
            // 
            this.lblNewBranchName.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblNewBranchName.AutoSize = true;
            this.lblNewBranchName.Location = new System.Drawing.Point(167, 224);
            this.lblNewBranchName.Name = "lblNewBranchName";
            this.lblNewBranchName.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblNewBranchName.Size = new System.Drawing.Size(117, 17);
            this.lblNewBranchName.TabIndex = 10;
            this.lblNewBranchName.Text = "NewBranchName";
            // 
            // txtNewBranchName
            // 
            this.txtNewBranchName.Location = new System.Drawing.Point(333, 224);
            this.txtNewBranchName.Name = "txtNewBranchName";
            this.txtNewBranchName.Size = new System.Drawing.Size(192, 22);
            this.txtNewBranchName.TabIndex = 11;
            // 
            // txtLocalBranchName
            // 
            this.txtLocalBranchName.Location = new System.Drawing.Point(333, 269);
            this.txtLocalBranchName.Name = "txtLocalBranchName";
            this.txtLocalBranchName.ReadOnly = true;
            this.txtLocalBranchName.Size = new System.Drawing.Size(192, 22);
            this.txtLocalBranchName.TabIndex = 13;
            // 
            // lblLocalBranchName
            // 
            this.lblLocalBranchName.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblLocalBranchName.AutoSize = true;
            this.lblLocalBranchName.Location = new System.Drawing.Point(160, 269);
            this.lblLocalBranchName.Name = "lblLocalBranchName";
            this.lblLocalBranchName.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblLocalBranchName.Size = new System.Drawing.Size(124, 17);
            this.lblLocalBranchName.TabIndex = 12;
            this.lblLocalBranchName.Text = "LocalBranchName";
            // 
            // frmSelectRemoteBranch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 372);
            this.ControlBox = false;
            this.Controls.Add(this.txtLocalBranchName);
            this.Controls.Add(this.lblLocalBranchName);
            this.Controls.Add(this.txtNewBranchName);
            this.Controls.Add(this.lblNewBranchName);
            this.Controls.Add(this.txtLastModifiedTime);
            this.Controls.Add(this.txtTag);
            this.Controls.Add(this.lblLastModifiedTime);
            this.Controls.Add(this.lblTag);
            this.Controls.Add(this.cmbProject);
            this.Controls.Add(this.lblProductBranch);
            this.Controls.Add(this.lblProject);
            this.Controls.Add(this.cmbBranch);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.MaximumSize = new System.Drawing.Size(668, 419);
            this.MinimumSize = new System.Drawing.Size(668, 419);
            this.Name = "frmSelectRemoteBranch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Remote Branch";
            this.Load += new System.EventHandler(this.frmSelectRemoteBranch_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmbBranch;
        private System.Windows.Forms.Label lblProject;
        private System.Windows.Forms.Label lblProductBranch;
        private System.Windows.Forms.ComboBox cmbProject;
        private System.Windows.Forms.Label lblTag;
        private System.Windows.Forms.Label lblLastModifiedTime;
        private System.Windows.Forms.TextBox txtTag;
        private System.Windows.Forms.TextBox txtLastModifiedTime;
        private System.Windows.Forms.Label lblNewBranchName;
        private System.Windows.Forms.TextBox txtNewBranchName;
        private System.Windows.Forms.TextBox txtLocalBranchName;
        private System.Windows.Forms.Label lblLocalBranchName;
    }
}