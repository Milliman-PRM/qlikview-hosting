namespace NbmcUnityTestGui
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
            this.ButtonBuildMrnList = new System.Windows.Forms.Button();
            this.ButtonExtractDiagnoses = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.ButtonExploreSystem = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ButtonExtractLabs = new System.Windows.Forms.Button();
            this.CheckboxUse9SamplePatients = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonBuildMrnList
            // 
            this.ButtonBuildMrnList.Location = new System.Drawing.Point(6, 19);
            this.ButtonBuildMrnList.Name = "ButtonBuildMrnList";
            this.ButtonBuildMrnList.Size = new System.Drawing.Size(260, 23);
            this.ButtonBuildMrnList.TabIndex = 0;
            this.ButtonBuildMrnList.Text = "Build Map of WOAH IDs To NBMC MRNs";
            this.ButtonBuildMrnList.UseVisualStyleBackColor = true;
            this.ButtonBuildMrnList.Click += new System.EventHandler(this.ButtonBuildMrnList_Click);
            // 
            // ButtonExtractDiagnoses
            // 
            this.ButtonExtractDiagnoses.Location = new System.Drawing.Point(6, 19);
            this.ButtonExtractDiagnoses.Name = "ButtonExtractDiagnoses";
            this.ButtonExtractDiagnoses.Size = new System.Drawing.Size(260, 23);
            this.ButtonExtractDiagnoses.TabIndex = 1;
            this.ButtonExtractDiagnoses.Text = "Extract Diagnosis Codes";
            this.ButtonExtractDiagnoses.UseVisualStyleBackColor = true;
            this.ButtonExtractDiagnoses.Click += new System.EventHandler(this.ButtonExtractDiagnoses_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // ButtonExploreSystem
            // 
            this.ButtonExploreSystem.Location = new System.Drawing.Point(6, 62);
            this.ButtonExploreSystem.Name = "ButtonExploreSystem";
            this.ButtonExploreSystem.Size = new System.Drawing.Size(260, 23);
            this.ButtonExploreSystem.TabIndex = 6;
            this.ButtonExploreSystem.Text = "Explore System";
            this.ButtonExploreSystem.UseVisualStyleBackColor = true;
            this.ButtonExploreSystem.Click += new System.EventHandler(this.ButtonExploreSystem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ButtonBuildMrnList);
            this.groupBox1.Location = new System.Drawing.Point(12, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(272, 62);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Should be run from a domain computer";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.CheckboxUse9SamplePatients);
            this.groupBox2.Controls.Add(this.ButtonExtractLabs);
            this.groupBox2.Controls.Add(this.ButtonExtractDiagnoses);
            this.groupBox2.Controls.Add(this.ButtonExploreSystem);
            this.groupBox2.Location = new System.Drawing.Point(12, 103);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(272, 262);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Should be run from prm-data-1.milliman.com";
            // 
            // ButtonExtractLabs
            // 
            this.ButtonExtractLabs.Location = new System.Drawing.Point(6, 108);
            this.ButtonExtractLabs.Name = "ButtonExtractLabs";
            this.ButtonExtractLabs.Size = new System.Drawing.Size(260, 23);
            this.ButtonExtractLabs.TabIndex = 7;
            this.ButtonExtractLabs.Text = "Extract Labs";
            this.ButtonExtractLabs.UseVisualStyleBackColor = true;
            this.ButtonExtractLabs.Click += new System.EventHandler(this.ButtonExtractLabs_Click);
            // 
            // CheckboxUse9SamplePatients
            // 
            this.CheckboxUse9SamplePatients.AutoSize = true;
            this.CheckboxUse9SamplePatients.Location = new System.Drawing.Point(6, 239);
            this.CheckboxUse9SamplePatients.Name = "CheckboxUse9SamplePatients";
            this.CheckboxUse9SamplePatients.Size = new System.Drawing.Size(133, 17);
            this.CheckboxUse9SamplePatients.TabIndex = 8;
            this.CheckboxUse9SamplePatients.Text = "Use 9 Sample Patients";
            this.CheckboxUse9SamplePatients.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(296, 377);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "NBMC Unity Driver GUI";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ButtonBuildMrnList;
        private System.Windows.Forms.Button ButtonExtractDiagnoses;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button ButtonExploreSystem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button ButtonExtractLabs;
        private System.Windows.Forms.CheckBox CheckboxUse9SamplePatients;
    }
}

