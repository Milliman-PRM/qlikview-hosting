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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.TabPageBoth = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.TabPagePath1Only = new System.Windows.Forms.TabPage();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.TabPagePath2Only = new System.Windows.Forms.TabPage();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.tabControl1.SuspendLayout();
            this.TabPageBoth.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.TabPagePath1Only.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.TabPagePath2Only.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            this.SuspendLayout();
            // 
            // TextBoxPath1
            // 
            this.TextBoxPath1.Location = new System.Drawing.Point(12, 29);
            this.TextBoxPath1.Name = "TextBoxPath1";
            this.TextBoxPath1.Size = new System.Drawing.Size(372, 20);
            this.TextBoxPath1.TabIndex = 1;
            this.TextBoxPath1.Text = "Double Click To Browse";
            // 
            // TextBoxPath2
            // 
            this.TextBoxPath2.Location = new System.Drawing.Point(400, 29);
            this.TextBoxPath2.Name = "TextBoxPath2";
            this.TextBoxPath2.Size = new System.Drawing.Size(372, 20);
            this.TextBoxPath2.TabIndex = 2;
            this.TextBoxPath2.Text = "Double Click To Browse";
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
            this.label2.Location = new System.Drawing.Point(397, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Path 2";
            // 
            // ButtonCompare
            // 
            this.ButtonCompare.Location = new System.Drawing.Point(348, 95);
            this.ButtonCompare.Name = "ButtonCompare";
            this.ButtonCompare.Size = new System.Drawing.Size(87, 40);
            this.ButtonCompare.TabIndex = 0;
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
            this.CheckBoxDoAppConfig.Enabled = false;
            this.CheckBoxDoAppConfig.Location = new System.Drawing.Point(12, 95);
            this.CheckBoxDoAppConfig.Name = "CheckBoxDoAppConfig";
            this.CheckBoxDoAppConfig.Size = new System.Drawing.Size(122, 17);
            this.CheckBoxDoAppConfig.TabIndex = 6;
            this.CheckBoxDoAppConfig.Text = "Evaluate App.config";
            this.CheckBoxDoAppConfig.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.TabPageBoth);
            this.tabControl1.Controls.Add(this.TabPagePath1Only);
            this.tabControl1.Controls.Add(this.TabPagePath2Only);
            this.tabControl1.Location = new System.Drawing.Point(13, 151);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(759, 399);
            this.tabControl1.TabIndex = 7;
            // 
            // TabPageBoth
            // 
            this.TabPageBoth.Controls.Add(this.dataGridView1);
            this.TabPageBoth.Location = new System.Drawing.Point(4, 22);
            this.TabPageBoth.Name = "TabPageBoth";
            this.TabPageBoth.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageBoth.Size = new System.Drawing.Size(751, 373);
            this.TabPageBoth.TabIndex = 0;
            this.TabPageBoth.Text = "Keys In Both Paths";
            this.TabPageBoth.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(745, 367);
            this.dataGridView1.TabIndex = 0;
            // 
            // TabPagePath1Only
            // 
            this.TabPagePath1Only.Controls.Add(this.dataGridView2);
            this.TabPagePath1Only.Location = new System.Drawing.Point(4, 22);
            this.TabPagePath1Only.Name = "TabPagePath1Only";
            this.TabPagePath1Only.Padding = new System.Windows.Forms.Padding(3);
            this.TabPagePath1Only.Size = new System.Drawing.Size(751, 373);
            this.TabPagePath1Only.TabIndex = 1;
            this.TabPagePath1Only.Text = "Path 1 Only";
            this.TabPagePath1Only.UseVisualStyleBackColor = true;
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView2.Location = new System.Drawing.Point(3, 3);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.ReadOnly = true;
            this.dataGridView2.Size = new System.Drawing.Size(745, 367);
            this.dataGridView2.TabIndex = 0;
            // 
            // TabPagePath2Only
            // 
            this.TabPagePath2Only.Controls.Add(this.dataGridView3);
            this.TabPagePath2Only.Location = new System.Drawing.Point(4, 22);
            this.TabPagePath2Only.Name = "TabPagePath2Only";
            this.TabPagePath2Only.Size = new System.Drawing.Size(627, 435);
            this.TabPagePath2Only.TabIndex = 2;
            this.TabPagePath2Only.Text = "Path 2 Only";
            this.TabPagePath2Only.UseVisualStyleBackColor = true;
            // 
            // dataGridView3
            // 
            this.dataGridView3.AllowUserToAddRows = false;
            this.dataGridView3.AllowUserToDeleteRows = false;
            this.dataGridView3.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView3.Location = new System.Drawing.Point(0, 0);
            this.dataGridView3.Name = "dataGridView3";
            this.dataGridView3.ReadOnly = true;
            this.dataGridView3.Size = new System.Drawing.Size(627, 435);
            this.dataGridView3.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.CheckBoxDoAppConfig);
            this.Controls.Add(this.CheckBoxDoWebConfig);
            this.Controls.Add(this.ButtonCompare);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextBoxPath2);
            this.Controls.Add(this.TextBoxPath1);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "Form1";
            this.Text = "Application Configuration Comparison Tool";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.TabPageBoth.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.TabPagePath1Only.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.TabPagePath2Only.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
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
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage TabPageBoth;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TabPage TabPagePath1Only;
        private System.Windows.Forms.TabPage TabPagePath2Only;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.DataGridView dataGridView3;
    }
}

