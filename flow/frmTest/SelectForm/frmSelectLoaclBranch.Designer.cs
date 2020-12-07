namespace ProvisioningBuildTools.SelectForm
{
    partial class frmSelectLoaclBranch
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
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(218, 91);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(350, 91);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
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
            this.cmbLocalBranches.Location = new System.Drawing.Point(380, 27);
            this.cmbLocalBranches.Name = "cmbLocalBranches";
            this.cmbLocalBranches.Size = new System.Drawing.Size(114, 24);
            this.cmbLocalBranches.TabIndex = 2;
            // 
            // lblReposFolder
            // 
            this.lblReposFolder.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblReposFolder.AutoSize = true;
            this.lblReposFolder.Location = new System.Drawing.Point(105, 27);
            this.lblReposFolder.Name = "lblReposFolder";
            this.lblReposFolder.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblReposFolder.Size = new System.Drawing.Size(228, 17);
            this.lblReposFolder.TabIndex = 3;
            this.lblReposFolder.Text = "C:\\Users\\v-fengzhou\\source\\repos\\";
            // 
            // frmSelectLoaclBranch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 139);
            this.ControlBox = false;
            this.Controls.Add(this.lblReposFolder);
            this.Controls.Add(this.cmbLocalBranches);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.MaximumSize = new System.Drawing.Size(668, 186);
            this.MinimumSize = new System.Drawing.Size(668, 186);
            this.Name = "frmSelectLoaclBranch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Loacl Branch";
            this.Load += new System.EventHandler(this.frmSelectLoaclBranch_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmbLocalBranches;
        private System.Windows.Forms.Label lblReposFolder;
    }
}