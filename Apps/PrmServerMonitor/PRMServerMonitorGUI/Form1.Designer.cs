namespace PRMServerMonitorGUI
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
            this.ButtonRemoveOrphanTasks = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.TabPageTasks = new System.Windows.Forms.TabPage();
            this.TabPageDocCals = new System.Windows.Forms.TabPage();
            this.TabPageUserCals = new System.Windows.Forms.TabPage();
            this.ButtonCalReport = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ComboBoxServer = new System.Windows.Forms.ComboBox();
            this.tabControl1.SuspendLayout();
            this.TabPageTasks.SuspendLayout();
            this.TabPageUserCals.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonRemoveOrphanTasks
            // 
            this.ButtonRemoveOrphanTasks.Location = new System.Drawing.Point(6, 6);
            this.ButtonRemoveOrphanTasks.Name = "ButtonRemoveOrphanTasks";
            this.ButtonRemoveOrphanTasks.Size = new System.Drawing.Size(210, 23);
            this.ButtonRemoveOrphanTasks.TabIndex = 2;
            this.ButtonRemoveOrphanTasks.Text = "Remove Orphan Tasks";
            this.ButtonRemoveOrphanTasks.UseVisualStyleBackColor = true;
            this.ButtonRemoveOrphanTasks.Click += new System.EventHandler(this.ButtonRemoveOrphanTasks_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.TabPageTasks);
            this.tabControl1.Controls.Add(this.TabPageDocCals);
            this.tabControl1.Controls.Add(this.TabPageUserCals);
            this.tabControl1.Location = new System.Drawing.Point(12, 37);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(592, 388);
            this.tabControl1.TabIndex = 4;
            // 
            // TabPageTasks
            // 
            this.TabPageTasks.Controls.Add(this.ButtonRemoveOrphanTasks);
            this.TabPageTasks.Location = new System.Drawing.Point(4, 22);
            this.TabPageTasks.Name = "TabPageTasks";
            this.TabPageTasks.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageTasks.Size = new System.Drawing.Size(584, 362);
            this.TabPageTasks.TabIndex = 0;
            this.TabPageTasks.Text = "Qlikview Tasks";
            this.TabPageTasks.UseVisualStyleBackColor = true;
            // 
            // TabPageDocCals
            // 
            this.TabPageDocCals.Location = new System.Drawing.Point(4, 22);
            this.TabPageDocCals.Name = "TabPageDocCals";
            this.TabPageDocCals.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageDocCals.Size = new System.Drawing.Size(584, 362);
            this.TabPageDocCals.TabIndex = 1;
            this.TabPageDocCals.Text = "Qlikview Document CALs";
            this.TabPageDocCals.UseVisualStyleBackColor = true;
            // 
            // TabPageUserCals
            // 
            this.TabPageUserCals.Controls.Add(this.ButtonCalReport);
            this.TabPageUserCals.Location = new System.Drawing.Point(4, 22);
            this.TabPageUserCals.Name = "TabPageUserCals";
            this.TabPageUserCals.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageUserCals.Size = new System.Drawing.Size(584, 362);
            this.TabPageUserCals.TabIndex = 2;
            this.TabPageUserCals.Text = "Qlikview User CALs";
            this.TabPageUserCals.UseVisualStyleBackColor = true;
            // 
            // ButtonCalReport
            // 
            this.ButtonCalReport.Location = new System.Drawing.Point(6, 6);
            this.ButtonCalReport.Name = "ButtonCalReport";
            this.ButtonCalReport.Size = new System.Drawing.Size(259, 23);
            this.ButtonCalReport.TabIndex = 4;
            this.ButtonCalReport.Text = "Report CALs";
            this.ButtonCalReport.UseVisualStyleBackColor = true;
            this.ButtonCalReport.Click += new System.EventHandler(this.ButtonCalReport_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Server:";
            // 
            // ComboBoxServer
            // 
            this.ComboBoxServer.FormattingEnabled = true;
            this.ComboBoxServer.Items.AddRange(new object[] {
            "localhost"});
            this.ComboBoxServer.Location = new System.Drawing.Point(60, 10);
            this.ComboBoxServer.Name = "ComboBoxServer";
            this.ComboBoxServer.Size = new System.Drawing.Size(150, 21);
            this.ComboBoxServer.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 437);
            this.Controls.Add(this.ComboBoxServer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "PRM Server Monitor";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.TabPageTasks.ResumeLayout(false);
            this.TabPageUserCals.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button ButtonRemoveOrphanTasks;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage TabPageTasks;
        private System.Windows.Forms.TabPage TabPageDocCals;
        private System.Windows.Forms.TabPage TabPageUserCals;
        private System.Windows.Forms.Button ButtonCalReport;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ComboBoxServer;
    }
}

