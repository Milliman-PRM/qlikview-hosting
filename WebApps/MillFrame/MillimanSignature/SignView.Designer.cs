namespace MillimanSignature
{
    partial class SignView
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.Open = new System.Windows.Forms.Button();
            this.SignatureGrid = new System.Windows.Forms.DataGridView();
            this.Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UpdateQVW = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SignatureGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 134F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.Open, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.SignatureGrid, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.UpdateQVW, 2, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(14, 11, 14, 11);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(378, 281);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // Open
            // 
            this.Open.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Open.Location = new System.Drawing.Point(11, 252);
            this.Open.Name = "Open";
            this.Open.Size = new System.Drawing.Size(99, 25);
            this.Open.TabIndex = 2;
            this.Open.Text = "Open";
            this.toolTip1.SetToolTip(this.Open, "Open a file to sign");
            this.Open.UseVisualStyleBackColor = true;
            this.Open.Click += new System.EventHandler(this.Open_Click);
            // 
            // SignatureGrid
            // 
            this.SignatureGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SignatureGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Key,
            this.Value});
            this.tableLayoutPanel1.SetColumnSpan(this.SignatureGrid, 3);
            this.SignatureGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SignatureGrid.Enabled = false;
            this.SignatureGrid.Location = new System.Drawing.Point(3, 3);
            this.SignatureGrid.Name = "SignatureGrid";
            this.SignatureGrid.Size = new System.Drawing.Size(372, 243);
            this.SignatureGrid.TabIndex = 1;
            // 
            // Key
            // 
            this.Key.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Key.HeaderText = "Key";
            this.Key.Name = "Key";
            this.Key.Width = 53;
            // 
            // Value
            // 
            this.Value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Value.HeaderText = "Value";
            this.Value.Name = "Value";
            // 
            // UpdateQVW
            // 
            this.UpdateQVW.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.UpdateQVW.Enabled = false;
            this.UpdateQVW.Location = new System.Drawing.Point(267, 252);
            this.UpdateQVW.Name = "UpdateQVW";
            this.UpdateQVW.Size = new System.Drawing.Size(99, 25);
            this.UpdateQVW.TabIndex = 0;
            this.UpdateQVW.Text = "Update";
            this.toolTip1.SetToolTip(this.UpdateQVW, "Update the file signature");
            this.UpdateQVW.UseVisualStyleBackColor = true;
            this.UpdateQVW.Click += new System.EventHandler(this.Update_Click);
            // 
            // SignView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 281);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SignView";
            this.Text = "Milliman Signature Utility";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SignatureGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button UpdateQVW;
        private System.Windows.Forms.DataGridView SignatureGrid;
        private System.Windows.Forms.Button Open;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Key;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
    }
}

