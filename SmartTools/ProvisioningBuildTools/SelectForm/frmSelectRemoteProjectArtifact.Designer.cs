namespace ProvisioningBuildTools.SelectForm
{
    partial class frmSelectRemoteProjectArtifact
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
            this.btnWait = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(128, 251);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 30);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(272, 251);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
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
            this.cmbBranch.Location = new System.Drawing.Point(221, 85);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(346, 24);
            this.cmbBranch.TabIndex = 2;
            this.cmbBranch.DropDown += new System.EventHandler(this.cmbBranch_DropDown);
            this.cmbBranch.SelectedIndexChanged += new System.EventHandler(this.cmbBranch_SelectedIndexChanged);
            // 
            // lblProject
            // 
            this.lblProject.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblProject.AutoSize = true;
            this.lblProject.Location = new System.Drawing.Point(120, 33);
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
            this.lblProductBranch.Location = new System.Drawing.Point(66, 85);
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
            this.cmbProject.Location = new System.Drawing.Point(221, 33);
            this.cmbProject.Name = "cmbProject";
            this.cmbProject.Size = new System.Drawing.Size(346, 24);
            this.cmbProject.TabIndex = 5;
            this.cmbProject.DropDown += new System.EventHandler(this.cmbProject_DropDown);
            this.cmbProject.SelectedIndexChanged += new System.EventHandler(this.cmbProject_SelectedIndexChanged);
            // 
            // lblTag
            // 
            this.lblTag.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblTag.AutoSize = true;
            this.lblTag.Location = new System.Drawing.Point(139, 135);
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
            this.lblLastModifiedTime.Location = new System.Drawing.Point(53, 189);
            this.lblLastModifiedTime.Name = "lblLastModifiedTime";
            this.lblLastModifiedTime.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblLastModifiedTime.Size = new System.Drawing.Size(119, 17);
            this.lblLastModifiedTime.TabIndex = 7;
            this.lblLastModifiedTime.Text = "LastModifiedTime";
            // 
            // txtTag
            // 
            this.txtTag.Location = new System.Drawing.Point(221, 135);
            this.txtTag.Name = "txtTag";
            this.txtTag.ReadOnly = true;
            this.txtTag.Size = new System.Drawing.Size(346, 22);
            this.txtTag.TabIndex = 8;
            this.txtTag.TextChanged += new System.EventHandler(this.txtTag_TextChanged);
            // 
            // txtLastModifiedTime
            // 
            this.txtLastModifiedTime.Location = new System.Drawing.Point(221, 189);
            this.txtLastModifiedTime.Name = "txtLastModifiedTime";
            this.txtLastModifiedTime.ReadOnly = true;
            this.txtLastModifiedTime.Size = new System.Drawing.Size(346, 22);
            this.txtLastModifiedTime.TabIndex = 9;
            // 
            // btnWait
            // 
            this.btnWait.Location = new System.Drawing.Point(420, 251);
            this.btnWait.Name = "btnWait";
            this.btnWait.Size = new System.Drawing.Size(121, 30);
            this.btnWait.TabIndex = 12;
            this.btnWait.Text = "WaitForTag";
            this.btnWait.UseVisualStyleBackColor = true;
            this.btnWait.Click += new System.EventHandler(this.btnWait_Click);
            // 
            // frmSelectRemoteProjectArtifact
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 303);
            this.ControlBox = false;
            this.Controls.Add(this.btnWait);
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
            this.MaximumSize = new System.Drawing.Size(668, 350);
            this.MinimumSize = new System.Drawing.Size(668, 350);
            this.Name = "frmSelectRemoteProjectArtifact";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Remote Project for Get Artifact";
            this.Load += new System.EventHandler(this.frmSelectRemoteBranchPostBuild_Load);
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
        private System.Windows.Forms.Button btnWait;
    }
}