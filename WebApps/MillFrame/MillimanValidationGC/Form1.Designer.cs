namespace PRMValidationGC
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
            this.components = new System.ComponentModel.Container();
            this.Directory = new System.Windows.Forms.TableLayoutPanel();
            this.Status = new System.Windows.Forms.TreeView();
            this.DirectoryTextbox = new System.Windows.Forms.TextBox();
            this.Browse = new System.Windows.Forms.Button();
            this.Process = new System.Windows.Forms.Button();
            this.Apply = new System.Windows.Forms.Button();
            this.Subdirs = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.Directory.SuspendLayout();
            this.SuspendLayout();
            // 
            // Directory
            // 
            this.Directory.ColumnCount = 5;
            this.Directory.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Directory.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.Directory.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.Directory.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.Directory.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.Directory.Controls.Add(this.Status, 0, 0);
            this.Directory.Controls.Add(this.DirectoryTextbox, 0, 1);
            this.Directory.Controls.Add(this.Browse, 1, 1);
            this.Directory.Controls.Add(this.Apply, 4, 1);
            this.Directory.Controls.Add(this.Process, 3, 1);
            this.Directory.Controls.Add(this.Subdirs, 2, 1);
            this.Directory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Directory.Location = new System.Drawing.Point(0, 0);
            this.Directory.Name = "Directory";
            this.Directory.RowCount = 2;
            this.Directory.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Directory.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.Directory.Size = new System.Drawing.Size(664, 308);
            this.Directory.TabIndex = 0;
            // 
            // Status
            // 
            this.Directory.SetColumnSpan(this.Status, 5);
            this.Status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Status.Location = new System.Drawing.Point(3, 3);
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(658, 272);
            this.Status.TabIndex = 0;
            // 
            // DirectoryTextbox
            // 
            this.DirectoryTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DirectoryTextbox.Location = new System.Drawing.Point(3, 281);
            this.DirectoryTextbox.Name = "DirectoryTextbox";
            this.DirectoryTextbox.ReadOnly = true;
            this.DirectoryTextbox.Size = new System.Drawing.Size(298, 20);
            this.DirectoryTextbox.TabIndex = 1;
            // 
            // Browse
            // 
            this.Browse.Location = new System.Drawing.Point(307, 281);
            this.Browse.Name = "Browse";
            this.Browse.Size = new System.Drawing.Size(74, 23);
            this.Browse.TabIndex = 2;
            this.Browse.Text = "Browse";
            this.Browse.UseVisualStyleBackColor = true;
            this.Browse.Click += new System.EventHandler(this.Browse_Click);
            // 
            // Process
            // 
            this.Process.Location = new System.Drawing.Point(507, 281);
            this.Process.Name = "Process";
            this.Process.Size = new System.Drawing.Size(74, 23);
            this.Process.TabIndex = 3;
            this.Process.Text = "Process";
            this.toolTip1.SetToolTip(this.Process, "Process the directory but do NOT  apply changes.");
            this.Process.UseVisualStyleBackColor = true;
            this.Process.Click += new System.EventHandler(this.Process_Click);
            // 
            // Apply
            // 
            this.Apply.Location = new System.Drawing.Point(587, 281);
            this.Apply.Name = "Apply";
            this.Apply.Size = new System.Drawing.Size(74, 23);
            this.Apply.TabIndex = 4;
            this.Apply.Text = "Apply";
            this.toolTip1.SetToolTip(this.Apply, "Apply the displayed changes");
            this.Apply.UseVisualStyleBackColor = true;
            this.Apply.Click += new System.EventHandler(this.Apply_Click);
            // 
            // Subdirs
            // 
            this.Subdirs.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Subdirs.AutoSize = true;
            this.Subdirs.Location = new System.Drawing.Point(387, 284);
            this.Subdirs.Name = "Subdirs";
            this.Subdirs.Size = new System.Drawing.Size(104, 17);
            this.Subdirs.TabIndex = 5;
            this.Subdirs.Text = "Include Sub Dirs";
            this.Subdirs.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 308);
            this.Controls.Add(this.Directory);
            this.Name = "Form1";
            this.Text = "PRM Garbage Collection";
            this.Directory.ResumeLayout(false);
            this.Directory.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel Directory;
        private System.Windows.Forms.TreeView Status;
        private System.Windows.Forms.TextBox DirectoryTextbox;
        private System.Windows.Forms.Button Browse;
        private System.Windows.Forms.Button Apply;
        private System.Windows.Forms.Button Process;
        private System.Windows.Forms.CheckBox Subdirs;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

