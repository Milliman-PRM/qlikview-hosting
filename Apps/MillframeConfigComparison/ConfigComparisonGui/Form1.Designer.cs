namespace ConfigComparisonGui
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
            this.TextBoxPath1 = new System.Windows.Forms.TextBox();
            this.TextBoxPath2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ButtonCompare = new System.Windows.Forms.Button();
            this.FolderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.CheckBoxDoWebConfig = new System.Windows.Forms.CheckBox();
            this.CheckBoxDoAppConfig = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // TextBoxPath1
            // 
            this.TextBoxPath1.Location = new System.Drawing.Point(12, 29);
            this.TextBoxPath1.Name = "TextBoxPath1";
            this.TextBoxPath1.Size = new System.Drawing.Size(292, 20);
            this.TextBoxPath1.TabIndex = 0;
            // 
            // TextBoxPath2
            // 
            this.TextBoxPath2.Location = new System.Drawing.Point(356, 29);
            this.TextBoxPath2.Name = "TextBoxPath2";
            this.TextBoxPath2.Size = new System.Drawing.Size(292, 20);
            this.TextBoxPath2.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Path 1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(353, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Path 2";
            // 
            // ButtonCompare
            // 
            this.ButtonCompare.Location = new System.Drawing.Point(291, 91);
            this.ButtonCompare.Name = "ButtonCompare";
            this.ButtonCompare.Size = new System.Drawing.Size(75, 23);
            this.ButtonCompare.TabIndex = 4;
            this.ButtonCompare.Text = "Compare";
            this.ButtonCompare.UseVisualStyleBackColor = true;
            this.ButtonCompare.Click += new System.EventHandler(this.ButtonCompare_Click);
            // 
            // CheckBoxDoWebConfig
            // 
            this.CheckBoxDoWebConfig.AutoSize = true;
            this.CheckBoxDoWebConfig.Checked = true;
            this.CheckBoxDoWebConfig.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBoxDoWebConfig.Location = new System.Drawing.Point(12, 72);
            this.CheckBoxDoWebConfig.Name = "CheckBoxDoWebConfig";
            this.CheckBoxDoWebConfig.Size = new System.Drawing.Size(126, 17);
            this.CheckBoxDoWebConfig.TabIndex = 5;
            this.CheckBoxDoWebConfig.Text = "Evaluate Web.config";
            this.CheckBoxDoWebConfig.UseVisualStyleBackColor = true;
            // 
            // CheckBoxDoAppConfig
            // 
            this.CheckBoxDoAppConfig.AutoSize = true;
            this.CheckBoxDoAppConfig.Location = new System.Drawing.Point(12, 95);
            this.CheckBoxDoAppConfig.Name = "CheckBoxDoAppConfig";
            this.CheckBoxDoAppConfig.Size = new System.Drawing.Size(122, 17);
            this.CheckBoxDoAppConfig.TabIndex = 6;
            this.CheckBoxDoAppConfig.Text = "Evaluate App.config";
            this.CheckBoxDoAppConfig.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 124);
            this.Controls.Add(this.CheckBoxDoAppConfig);
            this.Controls.Add(this.CheckBoxDoWebConfig);
            this.Controls.Add(this.ButtonCompare);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextBoxPath2);
            this.Controls.Add(this.TextBoxPath1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TextBoxPath1;
        private System.Windows.Forms.TextBox TextBoxPath2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button ButtonCompare;
        private System.Windows.Forms.FolderBrowserDialog FolderBrowserDialog1;
        private System.Windows.Forms.CheckBox CheckBoxDoWebConfig;
        private System.Windows.Forms.CheckBox CheckBoxDoAppConfig;
    }
}

