namespace MillimanQVLicenseCleaner
{
    partial class Main
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
            this.CleanNow = new System.Windows.Forms.Button();
            this.LicenseGrid = new System.Windows.Forms.DataGridView();
            this.Delete = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.LastAccess = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LicenseType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UserName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Document = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Settings = new System.Windows.Forms.ListBox();
            this.Help = new System.Windows.Forms.RichTextBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LicenseGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel1.Controls.Add(this.LicenseGrid, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.CleanNow, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.Settings, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.Help, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(639, 332);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // CleanNow
            // 
            this.CleanNow.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CleanNow.Location = new System.Drawing.Point(503, 270);
            this.CleanNow.Name = "CleanNow";
            this.CleanNow.Size = new System.Drawing.Size(121, 23);
            this.CleanNow.TabIndex = 1;
            this.CleanNow.Text = "Clean QV License";
            this.CleanNow.UseVisualStyleBackColor = true;
            this.CleanNow.Click += new System.EventHandler(this.RemoveLicenseNow);
            // 
            // LicenseGrid
            // 
            this.LicenseGrid.AllowUserToAddRows = false;
            this.LicenseGrid.AllowUserToDeleteRows = false;
            this.LicenseGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.LicenseGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Delete,
            this.LastAccess,
            this.LicenseType,
            this.UserName,
            this.Document});
            this.tableLayoutPanel1.SetColumnSpan(this.LicenseGrid, 3);
            this.LicenseGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LicenseGrid.Location = new System.Drawing.Point(3, 3);
            this.LicenseGrid.Name = "LicenseGrid";
            this.LicenseGrid.Size = new System.Drawing.Size(633, 226);
            this.LicenseGrid.TabIndex = 2;
            // 
            // Delete
            // 
            this.Delete.HeaderText = "Delete";
            this.Delete.Name = "Delete";
            this.Delete.Width = 75;
            // 
            // LastAccess
            // 
            this.LastAccess.HeaderText = "Last Accessed";
            this.LastAccess.Name = "LastAccess";
            // 
            // LicenseType
            // 
            this.LicenseType.HeaderText = "License Type";
            this.LicenseType.Name = "LicenseType";
            // 
            // UserName
            // 
            this.UserName.HeaderText = "Account";
            this.UserName.Name = "UserName";
            // 
            // Document
            // 
            this.Document.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Document.HeaderText = "Document";
            this.Document.Name = "Document";
            // 
            // Settings
            // 
            this.Settings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Settings.FormattingEnabled = true;
            this.Settings.Location = new System.Drawing.Point(3, 235);
            this.Settings.Name = "Settings";
            this.Settings.Size = new System.Drawing.Size(144, 94);
            this.Settings.TabIndex = 3;
            // 
            // Help
            // 
            this.Help.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Help.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Help.Location = new System.Drawing.Point(153, 235);
            this.Help.Name = "Help";
            this.Help.Size = new System.Drawing.Size(333, 94);
            this.Help.TabIndex = 4;
            this.Help.Text = "";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(639, 332);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Main";
            this.Text = "Milliman QV License Cleaner";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.LicenseGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button CleanNow;
        private System.Windows.Forms.DataGridView LicenseGrid;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Delete;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastAccess;
        private System.Windows.Forms.DataGridViewTextBoxColumn LicenseType;
        private System.Windows.Forms.DataGridViewTextBoxColumn UserName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Document;
        private System.Windows.Forms.ListBox Settings;
        private System.Windows.Forms.RichTextBox Help;
    }
}

