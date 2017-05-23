using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UptimeMonitorLib;

namespace UptimeMonitor
{
    public partial class Form1 : Form
    {
        UptimeTest Library = null;
        int TimerIntervalSeconds = 5 * 60;  // default can be overridden with appsetting named "TimerIntervalSeconds"
        string FormTextBase = "PRM Uptime Monitor";

        /// <summary>
        /// Constructor
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            this.TextBoxLogPath.DoubleClick += new System.EventHandler(this.PromptForLogPath);
            this.FormClosing += new FormClosingEventHandler(this.Form1_FormClosing);
        }

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            EnableStartGui(true);

            if (ConfigurationManager.AppSettings.AllKeys.Contains("TimerIntervalSeconds"))
            {
                int.TryParse(ConfigurationManager.AppSettings["TimerIntervalSeconds"], out TimerIntervalSeconds);
            }

            this.Text = FormTextBase + " - Time Interval " + TimerIntervalSeconds + " Seconds";

            TextBoxLogPath.Text = Directory.GetCurrentDirectory(); ;

            Library = new UptimeTest(TimerIntervalSeconds);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ButtonStop.Enabled == true)
            {
                ButtonStop_Click(null, null);
            }
        }

        /// <summary>
        /// Handler to start monitoring
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonStart_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(TextBoxLogPath.Text))
            {
                EnableStartGui(false);

                Library.Start(TextBoxLogPath.Text);
            }
            else
            {
                MessageBox.Show("Log path not found, aborting");
            }
        }

        /// <summary>
        /// Handler to discontinue monitoring
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonStop_Click(object sender, EventArgs e)
        {
            Library.Stop();
            EnableStartGui(true);
        }

        /// <summary>
        /// Handler to prompt the user for logging folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PromptForLogPath(object sender, EventArgs e)
        {
            DialogResult Res = FolderBrowserDialog1.ShowDialog(this);
            if (Res == DialogResult.OK && Directory.Exists(FolderBrowserDialog1.SelectedPath))
            {
                TextBoxLogPath.Text = FolderBrowserDialog1.SelectedPath;
            }
        }

        /// <summary>
        /// Manages the Gui state to enable either starting (default) or stopping the monitoring process
        /// </summary>
        /// <param name="Enable"></param>
        public void EnableStartGui(bool Enable = true)
        {
            ButtonStart.Enabled = Enable;
            ButtonStop.Enabled = !ButtonStart.Enabled;
        }
    }
}
