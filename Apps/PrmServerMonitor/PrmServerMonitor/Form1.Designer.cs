namespace PrmServerMonitor
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
            this.ButtonCalReport = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ButtonRemoveOrphanTasks
            // 
            this.ButtonRemoveOrphanTasks.Location = new System.Drawing.Point(13, 13);
            this.ButtonRemoveOrphanTasks.Name = "ButtonRemoveOrphanTasks";
            this.ButtonRemoveOrphanTasks.Size = new System.Drawing.Size(259, 23);
            this.ButtonRemoveOrphanTasks.TabIndex = 0;
            this.ButtonRemoveOrphanTasks.Text = "Remove Orphan Tasks";
            this.ButtonRemoveOrphanTasks.UseVisualStyleBackColor = true;
            this.ButtonRemoveOrphanTasks.Click += new System.EventHandler(this.ButtonRemoveOrphanTasks_Click);
            // 
            // ButtonCalReport
            // 
            this.ButtonCalReport.Location = new System.Drawing.Point(13, 43);
            this.ButtonCalReport.Name = "ButtonCalReport";
            this.ButtonCalReport.Size = new System.Drawing.Size(259, 23);
            this.ButtonCalReport.TabIndex = 1;
            this.ButtonCalReport.Text = "Report CALs";
            this.ButtonCalReport.UseVisualStyleBackColor = true;
            this.ButtonCalReport.Click += new System.EventHandler(this.ButtonCalReport_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.ButtonCalReport);
            this.Controls.Add(this.ButtonRemoveOrphanTasks);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ButtonRemoveOrphanTasks;
        private System.Windows.Forms.Button ButtonCalReport;
    }
}

