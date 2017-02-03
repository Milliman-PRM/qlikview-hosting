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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.TabPageBoth = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.TabPagePath1Only = new System.Windows.Forms.TabPage();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.TabPagePath2Only = new System.Windows.Forms.TabPage();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.TabPageConnectionStrings = new System.Windows.Forms.TabPage();
            this.dataGridView4 = new System.Windows.Forms.DataGridView();
            this.TabPageRequiredKeys = new System.Windows.Forms.TabPage();
            this.dataGridView6 = new System.Windows.Forms.DataGridView();
            this.dataGridView5 = new System.Windows.Forms.DataGridView();
            this.OpenFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.ComboBoxProducts = new System.Windows.Forms.ComboBox();
            this.ComboBoxCfg1Version = new System.Windows.Forms.ComboBox();
            this.ComboBoxCfg2Version = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabControl1.SuspendLayout();
            this.TabPageBoth.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.TabPagePath1Only.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.TabPagePath2Only.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            this.TabPageConnectionStrings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).BeginInit();
            this.TabPageRequiredKeys.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView5)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TextBoxPath1
            // 
            this.TextBoxPath1.Location = new System.Drawing.Point(12, 29);
            this.TextBoxPath1.Name = "TextBoxPath1";
            this.TextBoxPath1.Size = new System.Drawing.Size(443, 20);
            this.TextBoxPath1.TabIndex = 1;
            this.TextBoxPath1.Text = "Double Click To Browse";
            // 
            // TextBoxPath2
            // 
            this.TextBoxPath2.Location = new System.Drawing.Point(488, 29);
            this.TextBoxPath2.Name = "TextBoxPath2";
            this.TextBoxPath2.Size = new System.Drawing.Size(443, 20);
            this.TextBoxPath2.TabIndex = 2;
            this.TextBoxPath2.Text = "Double Click To Browse";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Configuration 1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(485, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Configuration 2";
            // 
            // ButtonCompare
            // 
            this.ButtonCompare.Location = new System.Drawing.Point(430, 150);
            this.ButtonCompare.Name = "ButtonCompare";
            this.ButtonCompare.Size = new System.Drawing.Size(87, 40);
            this.ButtonCompare.TabIndex = 0;
            this.ButtonCompare.Text = "Compare";
            this.ButtonCompare.UseVisualStyleBackColor = true;
            this.ButtonCompare.Click += new System.EventHandler(this.ButtonCompare_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.TabPageBoth);
            this.tabControl1.Controls.Add(this.TabPagePath1Only);
            this.tabControl1.Controls.Add(this.TabPagePath2Only);
            this.tabControl1.Controls.Add(this.TabPageConnectionStrings);
            this.tabControl1.Controls.Add(this.TabPageRequiredKeys);
            this.tabControl1.Location = new System.Drawing.Point(12, 208);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(921, 442);
            this.tabControl1.TabIndex = 7;
            // 
            // TabPageBoth
            // 
            this.TabPageBoth.Controls.Add(this.dataGridView1);
            this.TabPageBoth.Location = new System.Drawing.Point(4, 22);
            this.TabPageBoth.Name = "TabPageBoth";
            this.TabPageBoth.Size = new System.Drawing.Size(913, 416);
            this.TabPageBoth.TabIndex = 0;
            this.TabPageBoth.Text = "Keys In Both Paths";
            this.TabPageBoth.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(913, 416);
            this.dataGridView1.TabIndex = 0;
            // 
            // TabPagePath1Only
            // 
            this.TabPagePath1Only.Controls.Add(this.dataGridView2);
            this.TabPagePath1Only.Location = new System.Drawing.Point(4, 22);
            this.TabPagePath1Only.Name = "TabPagePath1Only";
            this.TabPagePath1Only.Padding = new System.Windows.Forms.Padding(3);
            this.TabPagePath1Only.Size = new System.Drawing.Size(913, 403);
            this.TabPagePath1Only.TabIndex = 1;
            this.TabPagePath1Only.Text = "Keys In Path 1 Only";
            this.TabPagePath1Only.UseVisualStyleBackColor = true;
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.AllowUserToResizeRows = false;
            this.dataGridView2.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView2.Location = new System.Drawing.Point(3, 3);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.ReadOnly = true;
            this.dataGridView2.Size = new System.Drawing.Size(907, 397);
            this.dataGridView2.TabIndex = 0;
            // 
            // TabPagePath2Only
            // 
            this.TabPagePath2Only.Controls.Add(this.dataGridView3);
            this.TabPagePath2Only.Location = new System.Drawing.Point(4, 22);
            this.TabPagePath2Only.Name = "TabPagePath2Only";
            this.TabPagePath2Only.Size = new System.Drawing.Size(913, 403);
            this.TabPagePath2Only.TabIndex = 2;
            this.TabPagePath2Only.Text = "Keys In Path 2 Only";
            this.TabPagePath2Only.UseVisualStyleBackColor = true;
            // 
            // dataGridView3
            // 
            this.dataGridView3.AllowUserToAddRows = false;
            this.dataGridView3.AllowUserToDeleteRows = false;
            this.dataGridView3.AllowUserToResizeRows = false;
            this.dataGridView3.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView3.Location = new System.Drawing.Point(0, 0);
            this.dataGridView3.Name = "dataGridView3";
            this.dataGridView3.ReadOnly = true;
            this.dataGridView3.Size = new System.Drawing.Size(913, 403);
            this.dataGridView3.TabIndex = 0;
            // 
            // TabPageConnectionStrings
            // 
            this.TabPageConnectionStrings.Controls.Add(this.dataGridView4);
            this.TabPageConnectionStrings.Location = new System.Drawing.Point(4, 22);
            this.TabPageConnectionStrings.Name = "TabPageConnectionStrings";
            this.TabPageConnectionStrings.Size = new System.Drawing.Size(913, 416);
            this.TabPageConnectionStrings.TabIndex = 3;
            this.TabPageConnectionStrings.Text = "Connection Strings";
            this.TabPageConnectionStrings.UseVisualStyleBackColor = true;
            // 
            // dataGridView4
            // 
            this.dataGridView4.AllowUserToAddRows = false;
            this.dataGridView4.AllowUserToDeleteRows = false;
            this.dataGridView4.AllowUserToResizeRows = false;
            this.dataGridView4.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView4.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView4.Location = new System.Drawing.Point(0, 0);
            this.dataGridView4.Name = "dataGridView4";
            this.dataGridView4.Size = new System.Drawing.Size(913, 416);
            this.dataGridView4.TabIndex = 1;
            // 
            // TabPageRequiredKeys
            // 
            this.TabPageRequiredKeys.Controls.Add(this.dataGridView6);
            this.TabPageRequiredKeys.Controls.Add(this.dataGridView5);
            this.TabPageRequiredKeys.Location = new System.Drawing.Point(4, 22);
            this.TabPageRequiredKeys.Name = "TabPageRequiredKeys";
            this.TabPageRequiredKeys.Size = new System.Drawing.Size(913, 416);
            this.TabPageRequiredKeys.TabIndex = 4;
            this.TabPageRequiredKeys.Text = "Required Configuration Keys";
            this.TabPageRequiredKeys.UseVisualStyleBackColor = true;
            // 
            // dataGridView6
            // 
            this.dataGridView6.AllowUserToAddRows = false;
            this.dataGridView6.AllowUserToDeleteRows = false;
            this.dataGridView6.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView6.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView6.Location = new System.Drawing.Point(460, 3);
            this.dataGridView6.Name = "dataGridView6";
            this.dataGridView6.Size = new System.Drawing.Size(449, 410);
            this.dataGridView6.TabIndex = 4;
            // 
            // dataGridView5
            // 
            this.dataGridView5.AllowUserToAddRows = false;
            this.dataGridView5.AllowUserToDeleteRows = false;
            this.dataGridView5.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView5.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView5.Location = new System.Drawing.Point(3, 3);
            this.dataGridView5.Name = "dataGridView5";
            this.dataGridView5.Size = new System.Drawing.Size(450, 410);
            this.dataGridView5.TabIndex = 3;
            // 
            // ComboBoxProducts
            // 
            this.ComboBoxProducts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxProducts.FormattingEnabled = true;
            this.ComboBoxProducts.Location = new System.Drawing.Point(153, 47);
            this.ComboBoxProducts.Name = "ComboBoxProducts";
            this.ComboBoxProducts.Size = new System.Drawing.Size(250, 21);
            this.ComboBoxProducts.TabIndex = 9;
            this.ComboBoxProducts.TextChanged += new System.EventHandler(this.ComboBoxProducts_TextChanged);
            // 
            // ComboBoxCfg1Version
            // 
            this.ComboBoxCfg1Version.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxCfg1Version.FormattingEnabled = true;
            this.ComboBoxCfg1Version.Location = new System.Drawing.Point(171, 20);
            this.ComboBoxCfg1Version.Name = "ComboBoxCfg1Version";
            this.ComboBoxCfg1Version.Size = new System.Drawing.Size(89, 21);
            this.ComboBoxCfg1Version.Sorted = true;
            this.ComboBoxCfg1Version.TabIndex = 10;
            // 
            // ComboBoxCfg2Version
            // 
            this.ComboBoxCfg2Version.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxCfg2Version.FormattingEnabled = true;
            this.ComboBoxCfg2Version.Location = new System.Drawing.Point(643, 20);
            this.ComboBoxCfg2Version.Name = "ComboBoxCfg2Version";
            this.ComboBoxCfg2Version.Size = new System.Drawing.Size(89, 21);
            this.ComboBoxCfg2Version.Sorted = true;
            this.ComboBoxCfg2Version.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(159, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Configuration 1 Product Version:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(478, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(159, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Configuration 2 Product Version:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(141, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Configuration Is For Product:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ComboBoxCfg2Version);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.ComboBoxCfg1Version);
            this.groupBox1.Controls.Add(this.ComboBoxProducts);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 55);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(920, 77);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "For assessing presence of required keys (Optional).  Required keys are to be mana" +
    "ged in this tool\'s app.config.  ";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(945, 662);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tabControl1);
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
            this.TabPageConnectionStrings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).EndInit();
            this.TabPageRequiredKeys.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView5)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TextBoxPath1;
        private System.Windows.Forms.TextBox TextBoxPath2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button ButtonCompare;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage TabPageBoth;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TabPage TabPagePath1Only;
        private System.Windows.Forms.TabPage TabPagePath2Only;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.DataGridView dataGridView3;
        private System.Windows.Forms.TabPage TabPageConnectionStrings;
        private System.Windows.Forms.DataGridView dataGridView4;
        private System.Windows.Forms.OpenFileDialog OpenFileDialog1;
        private System.Windows.Forms.ComboBox ComboBoxProducts;
        private System.Windows.Forms.ComboBox ComboBoxCfg1Version;
        private System.Windows.Forms.ComboBox ComboBoxCfg2Version;
        private System.Windows.Forms.TabPage TabPageRequiredKeys;
        private System.Windows.Forms.DataGridView dataGridView6;
        private System.Windows.Forms.DataGridView dataGridView5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

