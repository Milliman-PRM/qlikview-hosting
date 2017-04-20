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
            this.ButtonCalReport = new System.Windows.Forms.Button();
            this.ButtonRemoveOrphanTasks = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ButtonCalReport
            // 
            this.ButtonCalReport.Location = new System.Drawing.Point(15, 73);
            this.ButtonCalReport.Margin = new System.Windows.Forms.Padding(6);
            this.ButtonCalReport.Name = "ButtonCalReport";
            this.ButtonCalReport.Size = new System.Drawing.Size(518, 44);
            this.ButtonCalReport.TabIndex = 3;
            this.ButtonCalReport.Text = "Report CALs";
            this.ButtonCalReport.UseVisualStyleBackColor = true;
            // 
            // ButtonRemoveOrphanTasks
            // 
            this.ButtonRemoveOrphanTasks.Location = new System.Drawing.Point(15, 15);
            this.ButtonRemoveOrphanTasks.Margin = new System.Windows.Forms.Padding(6);
            this.ButtonRemoveOrphanTasks.Name = "ButtonRemoveOrphanTasks";
            this.ButtonRemoveOrphanTasks.Size = new System.Drawing.Size(518, 44);
            this.ButtonRemoveOrphanTasks.TabIndex = 2;
            this.ButtonRemoveOrphanTasks.Text = "Remove Orphan Tasks";
            this.ButtonRemoveOrphanTasks.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(564, 461);
            this.Controls.Add(this.ButtonCalReport);
            this.Controls.Add(this.ButtonRemoveOrphanTasks);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ButtonCalReport;
        private System.Windows.Forms.Button ButtonRemoveOrphanTasks;
    }
}

