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
            this.ButtonEnumerateDocCALs = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TextBoxDocName = new System.Windows.Forms.TextBox();
            this.TextBoxDocUserName = new System.Windows.Forms.TextBox();
            this.ButtonDeleteDocCalTest = new System.Windows.Forms.Button();
            this.TabPageUserCals = new System.Windows.Forms.TabPage();
            this.TextBoxUserName = new System.Windows.Forms.TextBox();
            this.ButtonDeleteTest = new System.Windows.Forms.Button();
            this.ButtonCalReport = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ComboBoxServer = new System.Windows.Forms.ComboBox();
            this.tabControl1.SuspendLayout();
            this.TabPageTasks.SuspendLayout();
            this.TabPageDocCals.SuspendLayout();
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
            this.TabPageDocCals.Controls.Add(this.ButtonEnumerateDocCALs);
            this.TabPageDocCals.Controls.Add(this.label3);
            this.TabPageDocCals.Controls.Add(this.label2);
            this.TabPageDocCals.Controls.Add(this.TextBoxDocName);
            this.TabPageDocCals.Controls.Add(this.TextBoxDocUserName);
            this.TabPageDocCals.Controls.Add(this.ButtonDeleteDocCalTest);
            this.TabPageDocCals.Location = new System.Drawing.Point(4, 22);
            this.TabPageDocCals.Name = "TabPageDocCals";
            this.TabPageDocCals.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageDocCals.Size = new System.Drawing.Size(584, 362);
            this.TabPageDocCals.TabIndex = 1;
            this.TabPageDocCals.Text = "Qlikview Document CALs";
            this.TabPageDocCals.UseVisualStyleBackColor = true;
            // 
            // ButtonEnumerateDocCALs
            // 
            this.ButtonEnumerateDocCALs.Location = new System.Drawing.Point(9, 6);
            this.ButtonEnumerateDocCALs.Name = "ButtonEnumerateDocCALs";
            this.ButtonEnumerateDocCALs.Size = new System.Drawing.Size(75, 35);
            this.ButtonEnumerateDocCALs.TabIndex = 5;
            this.ButtonEnumerateDocCALs.Text = "Enumerate Doc CALs";
            this.ButtonEnumerateDocCALs.UseVisualStyleBackColor = true;
            this.ButtonEnumerateDocCALs.Click += new System.EventHandler(this.ButtonEnumerateDocCALs_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(225, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Document:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "User ID:";
            // 
            // TextBoxDocName
            // 
            this.TextBoxDocName.Location = new System.Drawing.Point(290, 71);
            this.TextBoxDocName.Name = "TextBoxDocName";
            this.TextBoxDocName.Size = new System.Drawing.Size(288, 20);
            this.TextBoxDocName.TabIndex = 2;
            // 
            // TextBoxDocUserName
            // 
            this.TextBoxDocUserName.Location = new System.Drawing.Point(58, 71);
            this.TextBoxDocUserName.Name = "TextBoxDocUserName";
            this.TextBoxDocUserName.Size = new System.Drawing.Size(161, 20);
            this.TextBoxDocUserName.TabIndex = 1;
            // 
            // ButtonDeleteDocCalTest
            // 
            this.ButtonDeleteDocCalTest.Location = new System.Drawing.Point(6, 97);
            this.ButtonDeleteDocCalTest.Name = "ButtonDeleteDocCalTest";
            this.ButtonDeleteDocCalTest.Size = new System.Drawing.Size(75, 39);
            this.ButtonDeleteDocCalTest.TabIndex = 0;
            this.ButtonDeleteDocCalTest.Text = "Delete Doc CAL Test";
            this.ButtonDeleteDocCalTest.UseVisualStyleBackColor = true;
            this.ButtonDeleteDocCalTest.Click += new System.EventHandler(this.ButtonDeleteDocCalTest_Click);
            // 
            // TabPageUserCals
            // 
            this.TabPageUserCals.Controls.Add(this.TextBoxUserName);
            this.TabPageUserCals.Controls.Add(this.ButtonDeleteTest);
            this.TabPageUserCals.Controls.Add(this.ButtonCalReport);
            this.TabPageUserCals.Location = new System.Drawing.Point(4, 22);
            this.TabPageUserCals.Name = "TabPageUserCals";
            this.TabPageUserCals.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageUserCals.Size = new System.Drawing.Size(584, 362);
            this.TabPageUserCals.TabIndex = 2;
            this.TabPageUserCals.Text = "Qlikview User CALs";
            this.TabPageUserCals.UseVisualStyleBackColor = true;
            // 
            // TextBoxUserName
            // 
            this.TextBoxUserName.Location = new System.Drawing.Point(88, 38);
            this.TextBoxUserName.Name = "TextBoxUserName";
            this.TextBoxUserName.Size = new System.Drawing.Size(177, 20);
            this.TextBoxUserName.TabIndex = 6;
            // 
            // ButtonDeleteTest
            // 
            this.ButtonDeleteTest.Location = new System.Drawing.Point(7, 36);
            this.ButtonDeleteTest.Name = "ButtonDeleteTest";
            this.ButtonDeleteTest.Size = new System.Drawing.Size(75, 23);
            this.ButtonDeleteTest.TabIndex = 5;
            this.ButtonDeleteTest.Text = "Delete Test";
            this.ButtonDeleteTest.UseVisualStyleBackColor = true;
            this.ButtonDeleteTest.Click += new System.EventHandler(this.ButtonDeleteTest_Click);
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
            this.TabPageDocCals.ResumeLayout(false);
            this.TabPageDocCals.PerformLayout();
            this.TabPageUserCals.ResumeLayout(false);
            this.TabPageUserCals.PerformLayout();
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
        private System.Windows.Forms.Button ButtonDeleteTest;
        private System.Windows.Forms.TextBox TextBoxUserName;
        private System.Windows.Forms.Button ButtonDeleteDocCalTest;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TextBoxDocName;
        private System.Windows.Forms.TextBox TextBoxDocUserName;
        private System.Windows.Forms.Button ButtonEnumerateDocCALs;
    }
}

