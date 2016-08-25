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
            this.TextPatIndexCollectionName = new System.Windows.Forms.TextBox();
            this.LabelCollectionName = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ButtonBuildMrnList
            // 
            this.ButtonBuildMrnList.Location = new System.Drawing.Point(12, 55);
            this.ButtonBuildMrnList.Name = "ButtonBuildMrnList";
            this.ButtonBuildMrnList.Size = new System.Drawing.Size(260, 23);
            this.ButtonBuildMrnList.TabIndex = 0;
            this.ButtonBuildMrnList.Text = "Build Map of WOAH IDs To NBMC MRNs";
            this.ButtonBuildMrnList.UseVisualStyleBackColor = true;
            this.ButtonBuildMrnList.Click += new System.EventHandler(this.ButtonBuildMrnList_Click);
            // 
            // ButtonExtractDiagnoses
            // 
            this.ButtonExtractDiagnoses.Location = new System.Drawing.Point(12, 134);
            this.ButtonExtractDiagnoses.Name = "ButtonExtractDiagnoses";
            this.ButtonExtractDiagnoses.Size = new System.Drawing.Size(260, 23);
            this.ButtonExtractDiagnoses.TabIndex = 1;
            this.ButtonExtractDiagnoses.Text = "Extract Diagnosis Codes";
            this.ButtonExtractDiagnoses.UseVisualStyleBackColor = true;
            this.ButtonExtractDiagnoses.Click += new System.EventHandler(this.ButtonExtractDiagnoses_Click);
            // 
            // TextPatIndexCollectionName
            // 
            this.TextPatIndexCollectionName.Location = new System.Drawing.Point(102, 12);
            this.TextPatIndexCollectionName.Name = "TextPatIndexCollectionName";
            this.TextPatIndexCollectionName.Size = new System.Drawing.Size(170, 20);
            this.TextPatIndexCollectionName.TabIndex = 2;
            this.TextPatIndexCollectionName.Text = "patientindex";
            // 
            // LabelCollectionName
            // 
            this.LabelCollectionName.AutoSize = true;
            this.LabelCollectionName.Location = new System.Drawing.Point(12, 15);
            this.LabelCollectionName.Name = "LabelCollectionName";
            this.LabelCollectionName.Size = new System.Drawing.Size(87, 13);
            this.LabelCollectionName.TabIndex = 3;
            this.LabelCollectionName.Text = "Collection Name:";
            this.LabelCollectionName.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(189, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Should be run from a domain computer";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 164);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(211, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Should be run from prm-data-1.milliman.com";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 377);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LabelCollectionName);
            this.Controls.Add(this.TextPatIndexCollectionName);
            this.Controls.Add(this.ButtonExtractDiagnoses);
            this.Controls.Add(this.ButtonBuildMrnList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "NBMC Unity Driver GUI";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonBuildMrnList;
        private System.Windows.Forms.Button ButtonExtractDiagnoses;
        private System.Windows.Forms.TextBox TextPatIndexCollectionName;
        private System.Windows.Forms.Label LabelCollectionName;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

