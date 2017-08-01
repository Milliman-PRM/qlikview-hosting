namespace DehexifyStrings
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
            this.ButtonFromFile = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.TextBoxResult = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.ButtonReadSelections = new System.Windows.Forms.Button();
            this.TextBoxSelections = new System.Windows.Forms.TextBox();
            this.TreeViewHierarchy = new System.Windows.Forms.TreeView();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonFromFile
            // 
            this.ButtonFromFile.Location = new System.Drawing.Point(6, 6);
            this.ButtonFromFile.Name = "ButtonFromFile";
            this.ButtonFromFile.Size = new System.Drawing.Size(75, 23);
            this.ButtonFromFile.TabIndex = 0;
            this.ButtonFromFile.Text = "Read File";
            this.ButtonFromFile.UseVisualStyleBackColor = true;
            this.ButtonFromFile.Click += new System.EventHandler(this.ButtonFromFile_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // TextBoxResult
            // 
            this.TextBoxResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxResult.Location = new System.Drawing.Point(6, 35);
            this.TextBoxResult.Multiline = true;
            this.TextBoxResult.Name = "TextBoxResult";
            this.TextBoxResult.Size = new System.Drawing.Size(630, 408);
            this.TextBoxResult.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(13, 13);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(650, 475);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.ButtonFromFile);
            this.tabPage1.Controls.Add(this.TextBoxResult);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(642, 449);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Raw File";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.TreeViewHierarchy);
            this.tabPage2.Controls.Add(this.TextBoxSelections);
            this.tabPage2.Controls.Add(this.ButtonReadSelections);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(642, 449);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "User Selections";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // ButtonReadSelections
            // 
            this.ButtonReadSelections.Location = new System.Drawing.Point(7, 7);
            this.ButtonReadSelections.Name = "ButtonReadSelections";
            this.ButtonReadSelections.Size = new System.Drawing.Size(75, 23);
            this.ButtonReadSelections.TabIndex = 0;
            this.ButtonReadSelections.Text = "Browse...";
            this.ButtonReadSelections.UseVisualStyleBackColor = true;
            this.ButtonReadSelections.Click += new System.EventHandler(this.ButtonReadSelections_Click);
            // 
            // TextBoxSelections
            // 
            this.TextBoxSelections.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxSelections.Location = new System.Drawing.Point(442, 37);
            this.TextBoxSelections.Multiline = true;
            this.TextBoxSelections.Name = "TextBoxSelections";
            this.TextBoxSelections.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TextBoxSelections.Size = new System.Drawing.Size(194, 406);
            this.TextBoxSelections.TabIndex = 1;
            // 
            // TreeViewHierarchy
            // 
            this.TreeViewHierarchy.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TreeViewHierarchy.CheckBoxes = true;
            this.TreeViewHierarchy.Location = new System.Drawing.Point(7, 37);
            this.TreeViewHierarchy.Name = "TreeViewHierarchy";
            this.TreeViewHierarchy.Size = new System.Drawing.Size(429, 406);
            this.TreeViewHierarchy.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(675, 500);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ButtonFromFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox TextBoxResult;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox TextBoxSelections;
        private System.Windows.Forms.Button ButtonReadSelections;
        private System.Windows.Forms.TreeView TreeViewHierarchy;
    }
}

