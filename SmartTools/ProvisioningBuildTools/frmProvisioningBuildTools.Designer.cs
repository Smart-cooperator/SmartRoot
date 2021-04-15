﻿namespace ProvisioningBuildTools
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmProvisioningBuildTools));
            this.rtbCMD = new System.Windows.Forms.RichTextBox();
            this.cmbExecItems = new System.Windows.Forms.ComboBox();
            this.btnExec = new System.Windows.Forms.Button();
            this.btnAbort = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnKill = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbCMD
            // 
            this.rtbCMD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbCMD.Location = new System.Drawing.Point(3, 3);
            this.rtbCMD.Name = "rtbCMD";
            this.tableLayoutPanel1.SetRowSpan(this.rtbCMD, 7);
            this.rtbCMD.Size = new System.Drawing.Size(1188, 623);
            this.rtbCMD.TabIndex = 1;
            this.rtbCMD.Text = "";
            this.rtbCMD.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbCMD_LinkClicked);
            // 
            // cmbExecItems
            // 
            this.cmbExecItems.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F);
            this.cmbExecItems.FormattingEnabled = true;
            this.cmbExecItems.ItemHeight = 20;
            this.cmbExecItems.Location = new System.Drawing.Point(1197, 23);
            this.cmbExecItems.Name = "cmbExecItems";
            this.cmbExecItems.Size = new System.Drawing.Size(250, 28);
            this.cmbExecItems.TabIndex = 2;
            // 
            // btnExec
            // 
            this.btnExec.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnExec.Location = new System.Drawing.Point(1197, 140);
            this.btnExec.Name = "btnExec";
            this.btnExec.Size = new System.Drawing.Size(250, 40);
            this.btnExec.TabIndex = 3;
            this.btnExec.Text = "Exec";
            this.btnExec.UseVisualStyleBackColor = true;
            this.btnExec.Click += new System.EventHandler(this.btnExec_Click);
            // 
            // btnAbort
            // 
            this.btnAbort.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnAbort.Location = new System.Drawing.Point(1197, 257);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(250, 40);
            this.btnAbort.TabIndex = 4;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnClear.Location = new System.Drawing.Point(1197, 374);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(250, 40);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnKill
            // 
            this.btnKill.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnKill.Location = new System.Drawing.Point(1197, 491);
            this.btnKill.Name = "btnKill";
            this.btnKill.Size = new System.Drawing.Size(250, 40);
            this.btnKill.TabIndex = 6;
            this.btnKill.Text = "Kill";
            this.btnKill.UseVisualStyleBackColor = true;
            this.btnKill.Click += new System.EventHandler(this.btnKill_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.rtbCMD, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.cmbExecItems, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnClear, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnExec, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnAbort, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnKill, 1, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1450, 629);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // frmProvisioningBuildTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1450, 629);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1468, 676);
            this.Name = "frmProvisioningBuildTools";
            this.Text = "Provisioning Build Tools";
            this.Activated += new System.EventHandler(this.frmProvisioningBuildTools_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmProvisioningBuildTools_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmProvisioningBuildTools_FormClosed);
            this.Load += new System.EventHandler(this.frmProvisioningBuildTools_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.RichTextBox rtbCMD;
        private System.Windows.Forms.ComboBox cmbExecItems;
        private System.Windows.Forms.Button btnExec;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnKill;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}

