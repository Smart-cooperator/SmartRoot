namespace ProvisioningBuildTools
{
    partial class frmProvisioningBuildTools
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
            this.rtbCMD = new System.Windows.Forms.RichTextBox();
            this.cmbExecItems = new System.Windows.Forms.ComboBox();
            this.btnExec = new System.Windows.Forms.Button();
            this.btnAbort = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rtbCMD
            // 
            this.rtbCMD.Location = new System.Drawing.Point(12, 12);
            this.rtbCMD.Name = "rtbCMD";
            this.rtbCMD.Size = new System.Drawing.Size(1173, 605);
            this.rtbCMD.TabIndex = 1;
            this.rtbCMD.Text = "";
            // 
            // cmbExecItems
            // 
            this.cmbExecItems.FormattingEnabled = true;
            this.cmbExecItems.IntegralHeight = false;
            this.cmbExecItems.ItemHeight = 16;
            this.cmbExecItems.Location = new System.Drawing.Point(1206, 76);
            this.cmbExecItems.Name = "cmbExecItems";
            this.cmbExecItems.Size = new System.Drawing.Size(184, 24);
            this.cmbExecItems.TabIndex = 2;
            // 
            // btnExec
            // 
            this.btnExec.Location = new System.Drawing.Point(1260, 172);
            this.btnExec.Name = "btnExec";
            this.btnExec.Size = new System.Drawing.Size(75, 23);
            this.btnExec.TabIndex = 3;
            this.btnExec.Text = "Exec";
            this.btnExec.UseVisualStyleBackColor = true;
            this.btnExec.Click += new System.EventHandler(this.btnExec_Click);
            // 
            // btnAbort
            // 
            this.btnAbort.Location = new System.Drawing.Point(1260, 257);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(75, 23);
            this.btnAbort.TabIndex = 4;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(1260, 348);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // frmProvisioningBuildTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1402, 629);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnExec);
            this.Controls.Add(this.cmbExecItems);
            this.Controls.Add(this.rtbCMD);
            this.MaximumSize = new System.Drawing.Size(1420, 676);
            this.MinimumSize = new System.Drawing.Size(1420, 676);
            this.Name = "frmProvisioningBuildTools";
            this.Text = "Provisioning Build Tools";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmProvisioningBuildTools_FormClosing);
            this.Load += new System.EventHandler(this.frmProvisioningBuildTools_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.RichTextBox rtbCMD;
        private System.Windows.Forms.ComboBox cmbExecItems;
        private System.Windows.Forms.Button btnExec;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.Button btnClear;
    }
}

