using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CdrContext;
using System.Configuration;

namespace CdrDbLibTestGui
{
    public partial class Form1 : Form
    {
        CdrDataContext CdrDb;

        public Form1()
        {
            InitializeComponent();

            string ConnectionString = ConfigurationManager.ConnectionStrings["CdrContextConnectionString"].ConnectionString;
            int UserIdStart = ConnectionString.IndexOf(@"User Id=") + @"User Id=".Length;
            int UserIdEnd = ConnectionString.IndexOf(";", UserIdStart);
            string UserId = ConnectionString.Substring(UserIdStart, UserIdEnd - UserIdStart);

            ConnectionString = ConnectionString.Insert(UserIdEnd, @";Password=" + PasswordPrompt(UserId));
            CdrDb = new CdrDataContext(ConnectionString);
            ConnectionString = null;
        }

        private void buttonTest1_Click(object sender, EventArgs e)
        {
            CdrContext.Patient P = new CdrContext.Patient
            {
                BirthDate = new DateTime(1955, 10, 23),
                DeathDate = null,
                Gender = CdrContext.Gender.Male,
                Ethnicity = "White",
                MaritalStatus = CdrContext.MaritalStatus.Other,
                NameLast = "Mouse",
                NameFirst = "Mickey"
            };

            CdrDb.Patients.InsertOnSubmit(P);
            CdrDb.SubmitChanges();
        }

        private static string PasswordPrompt(string UserName)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = @"Password Required",
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Width = 400, Text = "Enter password for user " + UserName };
            TextBox textBox = new TextBox() { Left = 50, Top = 40, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

    }
}
