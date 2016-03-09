namespace CdrExtractTest
{
    partial class Form1
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
            this.btnBayClinic = new System.Windows.Forms.Button();
            this.btnNorthBend = new System.Windows.Forms.Button();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.chkMongoInsert = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnBayClinic
            // 
            this.btnBayClinic.Location = new System.Drawing.Point(12, 91);
            this.btnBayClinic.Name = "btnBayClinic";
            this.btnBayClinic.Size = new System.Drawing.Size(75, 23);
            this.btnBayClinic.TabIndex = 0;
            this.btnBayClinic.Text = "Bay Clinic";
            this.btnBayClinic.UseVisualStyleBackColor = true;
            this.btnBayClinic.Click += new System.EventHandler(this.btnBayClinic_Click);
            // 
            // btnNorthBend
            // 
            this.btnNorthBend.Location = new System.Drawing.Point(12, 121);
            this.btnNorthBend.Name = "btnNorthBend";
            this.btnNorthBend.Size = new System.Drawing.Size(75, 23);
            this.btnNorthBend.TabIndex = 1;
            this.btnNorthBend.Text = "North Bend";
            this.btnNorthBend.UseVisualStyleBackColor = true;
            this.btnNorthBend.Click += new System.EventHandler(this.btnNorthBend_Click);
            // 
            // txtFolder
            // 
            this.txtFolder.Location = new System.Drawing.Point(12, 29);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(259, 20);
            this.txtFolder.TabIndex = 2;
            this.txtFolder.TextChanged += new System.EventHandler(this.txtFolder_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(196, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Raw Data Path (Doubleclick for Dialog):";
            // 
            // chkMongoInsert
            // 
            this.chkMongoInsert.AutoSize = true;
            this.chkMongoInsert.Location = new System.Drawing.Point(12, 233);
            this.chkMongoInsert.Name = "chkMongoInsert";
            this.chkMongoInsert.Size = new System.Drawing.Size(147, 17);
            this.chkMongoInsert.TabIndex = 4;
            this.chkMongoInsert.Text = "Perform MongoDB Inserts";
            this.chkMongoInsert.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.chkMongoInsert);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.btnNorthBend);
            this.Controls.Add(this.btnBayClinic);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBayClinic;
        private System.Windows.Forms.Button btnNorthBend;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkMongoInsert;
    }
}

