namespace WoahCdrAggregationTestGui
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
            this.groupFeedSelection = new System.Windows.Forms.GroupBox();
            this.labelElapsedTime = new System.Windows.Forms.Label();
            this.labelNbmcPatientsCompleted = new System.Windows.Forms.Label();
            this.labelBcPatientsCompleted = new System.Windows.Forms.Label();
            this.radioNBMCAllscriptsViaIntelliware = new System.Windows.Forms.RadioButton();
            this.radioBayClinicCernerAmbulatory = new System.Windows.Forms.RadioButton();
            this.buttonAggregate = new System.Windows.Forms.Button();
            this.buttonEndAllThreads = new System.Windows.Forms.Button();
            this.timerUiUpdate = new System.Windows.Forms.Timer(this.components);
            this.checkBoxResetMongoDB = new System.Windows.Forms.CheckBox();
            this.ButtonAddICD10CodeMeanings = new System.Windows.Forms.Button();
            this.groupFeedSelection.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupFeedSelection
            // 
            this.groupFeedSelection.Controls.Add(this.labelElapsedTime);
            this.groupFeedSelection.Controls.Add(this.labelNbmcPatientsCompleted);
            this.groupFeedSelection.Controls.Add(this.labelBcPatientsCompleted);
            this.groupFeedSelection.Controls.Add(this.radioNBMCAllscriptsViaIntelliware);
            this.groupFeedSelection.Controls.Add(this.radioBayClinicCernerAmbulatory);
            this.groupFeedSelection.Location = new System.Drawing.Point(12, 12);
            this.groupFeedSelection.Name = "groupFeedSelection";
            this.groupFeedSelection.Size = new System.Drawing.Size(440, 366);
            this.groupFeedSelection.TabIndex = 0;
            this.groupFeedSelection.TabStop = false;
            this.groupFeedSelection.Text = "Feed Selection";
            // 
            // labelElapsedTime
            // 
            this.labelElapsedTime.AutoSize = true;
            this.labelElapsedTime.Location = new System.Drawing.Point(6, 343);
            this.labelElapsedTime.Name = "labelElapsedTime";
            this.labelElapsedTime.Size = new System.Drawing.Size(13, 13);
            this.labelElapsedTime.TabIndex = 3;
            this.labelElapsedTime.Text = "0";
            // 
            // labelNbmcPatientsCompleted
            // 
            this.labelNbmcPatientsCompleted.AutoSize = true;
            this.labelNbmcPatientsCompleted.Location = new System.Drawing.Point(196, 46);
            this.labelNbmcPatientsCompleted.Name = "labelNbmcPatientsCompleted";
            this.labelNbmcPatientsCompleted.Size = new System.Drawing.Size(13, 13);
            this.labelNbmcPatientsCompleted.TabIndex = 4;
            this.labelNbmcPatientsCompleted.Text = "0";
            // 
            // labelBcPatientsCompleted
            // 
            this.labelBcPatientsCompleted.AutoSize = true;
            this.labelBcPatientsCompleted.Location = new System.Drawing.Point(196, 24);
            this.labelBcPatientsCompleted.Name = "labelBcPatientsCompleted";
            this.labelBcPatientsCompleted.Size = new System.Drawing.Size(13, 13);
            this.labelBcPatientsCompleted.TabIndex = 3;
            this.labelBcPatientsCompleted.Text = "0";
            // 
            // radioNBMCAllscriptsViaIntelliware
            // 
            this.radioNBMCAllscriptsViaIntelliware.AutoSize = true;
            this.radioNBMCAllscriptsViaIntelliware.Location = new System.Drawing.Point(7, 44);
            this.radioNBMCAllscriptsViaIntelliware.Name = "radioNBMCAllscriptsViaIntelliware";
            this.radioNBMCAllscriptsViaIntelliware.Size = new System.Drawing.Size(170, 17);
            this.radioNBMCAllscriptsViaIntelliware.TabIndex = 1;
            this.radioNBMCAllscriptsViaIntelliware.TabStop = true;
            this.radioNBMCAllscriptsViaIntelliware.Text = "NBMC Allscripts via. Intelliware";
            this.radioNBMCAllscriptsViaIntelliware.UseVisualStyleBackColor = true;
            // 
            // radioBayClinicCernerAmbulatory
            // 
            this.radioBayClinicCernerAmbulatory.AutoSize = true;
            this.radioBayClinicCernerAmbulatory.Location = new System.Drawing.Point(7, 20);
            this.radioBayClinicCernerAmbulatory.Name = "radioBayClinicCernerAmbulatory";
            this.radioBayClinicCernerAmbulatory.Size = new System.Drawing.Size(160, 17);
            this.radioBayClinicCernerAmbulatory.TabIndex = 0;
            this.radioBayClinicCernerAmbulatory.TabStop = true;
            this.radioBayClinicCernerAmbulatory.Text = "Bay Clinic Cerner Ambulatory";
            this.radioBayClinicCernerAmbulatory.UseVisualStyleBackColor = true;
            // 
            // buttonAggregate
            // 
            this.buttonAggregate.Location = new System.Drawing.Point(458, 13);
            this.buttonAggregate.Name = "buttonAggregate";
            this.buttonAggregate.Size = new System.Drawing.Size(160, 23);
            this.buttonAggregate.TabIndex = 1;
            this.buttonAggregate.Text = "Aggregate";
            this.buttonAggregate.UseVisualStyleBackColor = true;
            this.buttonAggregate.Click += new System.EventHandler(this.buttonAggregate_Click);
            // 
            // buttonEndAllThreads
            // 
            this.buttonEndAllThreads.Location = new System.Drawing.Point(458, 355);
            this.buttonEndAllThreads.Name = "buttonEndAllThreads";
            this.buttonEndAllThreads.Size = new System.Drawing.Size(160, 23);
            this.buttonEndAllThreads.TabIndex = 2;
            this.buttonEndAllThreads.Text = "End All Threads";
            this.buttonEndAllThreads.UseVisualStyleBackColor = true;
            this.buttonEndAllThreads.Click += new System.EventHandler(this.buttonEndAllThreads_Click);
            // 
            // timerUiUpdate
            // 
            this.timerUiUpdate.Tick += new System.EventHandler(this.timerUiUpdate_Tick);
            // 
            // checkBoxResetMongoDB
            // 
            this.checkBoxResetMongoDB.AutoSize = true;
            this.checkBoxResetMongoDB.Location = new System.Drawing.Point(458, 43);
            this.checkBoxResetMongoDB.Name = "checkBoxResetMongoDB";
            this.checkBoxResetMongoDB.Size = new System.Drawing.Size(105, 17);
            this.checkBoxResetMongoDB.TabIndex = 3;
            this.checkBoxResetMongoDB.Text = "Reset MongoDB";
            this.checkBoxResetMongoDB.UseVisualStyleBackColor = true;
            // 
            // ButtonAddICD10CodeMeanings
            // 
            this.ButtonAddICD10CodeMeanings.Location = new System.Drawing.Point(458, 91);
            this.ButtonAddICD10CodeMeanings.Name = "ButtonAddICD10CodeMeanings";
            this.ButtonAddICD10CodeMeanings.Size = new System.Drawing.Size(160, 23);
            this.ButtonAddICD10CodeMeanings.TabIndex = 4;
            this.ButtonAddICD10CodeMeanings.Text = "Add ICD10 Code Meanings";
            this.ButtonAddICD10CodeMeanings.UseVisualStyleBackColor = true;
            this.ButtonAddICD10CodeMeanings.Click += new System.EventHandler(this.ButtonAddICD10CodeMeanings_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(629, 390);
            this.Controls.Add(this.ButtonAddICD10CodeMeanings);
            this.Controls.Add(this.checkBoxResetMongoDB);
            this.Controls.Add(this.buttonEndAllThreads);
            this.Controls.Add(this.buttonAggregate);
            this.Controls.Add(this.groupFeedSelection);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupFeedSelection.ResumeLayout(false);
            this.groupFeedSelection.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupFeedSelection;
        private System.Windows.Forms.RadioButton radioNBMCAllscriptsViaIntelliware;
        private System.Windows.Forms.RadioButton radioBayClinicCernerAmbulatory;
        private System.Windows.Forms.Button buttonAggregate;
        private System.Windows.Forms.Button buttonEndAllThreads;
        private System.Windows.Forms.Label labelBcPatientsCompleted;
        private System.Windows.Forms.Timer timerUiUpdate;
        private System.Windows.Forms.Label labelNbmcPatientsCompleted;
        private System.Windows.Forms.Label labelElapsedTime;
        private System.Windows.Forms.CheckBox checkBoxResetMongoDB;
        private System.Windows.Forms.Button ButtonAddICD10CodeMeanings;
    }
}

