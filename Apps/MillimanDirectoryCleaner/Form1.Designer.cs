namespace MillimanDirectoryCleaner
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.Process = new System.Windows.Forms.Button();
            this.SourceDirectory = new System.Windows.Forms.TextBox();
            this.Browse = new System.Windows.Forms.Button();
            this.DirectoryGrid = new System.Windows.Forms.DataGridView();
            this.Delete = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.User = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Directory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DirectoryGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.Process, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.SourceDirectory, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.Browse, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.DirectoryGrid, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(892, 444);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // Process
            // 
            this.Process.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel1.SetColumnSpan(this.Process, 2);
            this.Process.Location = new System.Drawing.Point(408, 415);
            this.Process.Name = "Process";
            this.Process.Size = new System.Drawing.Size(75, 23);
            this.Process.TabIndex = 1;
            this.Process.Text = "Process";
            this.Process.UseVisualStyleBackColor = true;
            this.Process.Click += new System.EventHandler(this.Process_Click);
            // 
            // SourceDirectory
            // 
            this.SourceDirectory.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.SourceDirectory.Location = new System.Drawing.Point(3, 7);
            this.SourceDirectory.Name = "SourceDirectory";
            this.SourceDirectory.Size = new System.Drawing.Size(440, 20);
            this.SourceDirectory.TabIndex = 2;
            // 
            // Browse
            // 
            this.Browse.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Browse.Location = new System.Drawing.Point(449, 6);
            this.Browse.Name = "Browse";
            this.Browse.Size = new System.Drawing.Size(34, 23);
            this.Browse.TabIndex = 3;
            this.Browse.Text = "......";
            this.Browse.UseVisualStyleBackColor = true;
            this.Browse.Click += new System.EventHandler(this.Browse_Click);
            // 
            // DirectoryGrid
            // 
            this.DirectoryGrid.AllowUserToAddRows = false;
            this.DirectoryGrid.AllowUserToDeleteRows = false;
            this.DirectoryGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DirectoryGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Delete,
            this.User,
            this.Directory});
            this.tableLayoutPanel1.SetColumnSpan(this.DirectoryGrid, 2);
            this.DirectoryGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DirectoryGrid.Location = new System.Drawing.Point(3, 38);
            this.DirectoryGrid.Name = "DirectoryGrid";
            this.DirectoryGrid.RowHeadersVisible = false;
            this.DirectoryGrid.Size = new System.Drawing.Size(886, 368);
            this.DirectoryGrid.TabIndex = 4;
            // 
            // Delete
            // 
            this.Delete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Delete.HeaderText = "Delete";
            this.Delete.Name = "Delete";
            this.Delete.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Delete.Width = 75;
            // 
            // User
            // 
            this.User.HeaderText = "Account";
            this.User.Name = "User";
            this.User.Width = 300;
            // 
            // Directory
            // 
            this.Directory.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Directory.HeaderText = "Directory";
            this.Directory.Name = "Directory";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(892, 444);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.Text = "Directory Cleaner";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DirectoryGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button Process;
        private System.Windows.Forms.TextBox SourceDirectory;
        private System.Windows.Forms.Button Browse;
        private System.Windows.Forms.DataGridView DirectoryGrid;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Delete;
        private System.Windows.Forms.DataGridViewTextBoxColumn User;
        private System.Windows.Forms.DataGridViewTextBoxColumn Directory;
    }
}

