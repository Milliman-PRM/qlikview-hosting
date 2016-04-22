using ConfigIt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CLSConfigurationProductionDaemon
{
    public partial class StatusWindow : Form
    {
        public StatusWindow()
        {
            InitializeComponent();
            CLSConfigurationServices.CLSConfigurationServices CLSServices = new CLSConfigurationServices.CLSConfigurationServices();
            string SchemaName = string.Empty;
            string OperatorEmail = string.Empty;
            if ( CLSServices.NewSchemaIsReady(out SchemaName, out OperatorEmail))
            {
                string WebConfig  = EnvironmentSettings.Elements["ProductionSiteWebConfig"].Value;
                string SMTPServer = EnvironmentSettings.Elements["SMTPServer"].Value;
                string SMTPPort   = EnvironmentSettings.Elements["SMTPPort"].Value;
                string FromEmail  = EnvironmentSettings.Elements["FromEmail"].Value;
                string Subject    = EnvironmentSettings.Elements["Subject"].Value;

                string StatusMsg = "Could not locate production system web.config file to reset database connection for new dataset.";
                if ( System.IO.File.Exists(WebConfig))
                {
                    string WebConfigContents = System.IO.File.ReadAllText(WebConfig);
                    string CurrentSchemaName = CLSConfigurationCommon.PostgresqlUtilities.FindSchemaNameInWebConfig(WebConfigContents);
                    WebConfigContents.Replace(CurrentSchemaName, SchemaName);
                    System.IO.File.WriteAllText(WebConfig, WebConfigContents);
                    CLSServices.SetActiveSchemaName(SchemaName);
                    StatusMsg = "'" + SchemaName + "' was made active for external users of the production system at:  " + System.DateTime.Now.ToString();
                }

                System.Net.Mail.MailMessage objeto_mail = new System.Net.Mail.MailMessage();
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                client.Port = System.Convert.ToInt32(SMTPPort);
                client.Host = SMTPServer;
                client.Timeout = 120000;
                client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = true;
                objeto_mail.From = new System.Net.Mail.MailAddress(FromEmail);
                objeto_mail.To.Add(new System.Net.Mail.MailAddress(OperatorEmail));
                objeto_mail.Subject = Subject;
                objeto_mail.IsBodyHtml = true;
                objeto_mail.Priority = System.Net.Mail.MailPriority.High;
                objeto_mail.Body = "<html><body>" + StatusMsg + "</body></html>";
                client.Send(objeto_mail);
            }

            Environment.Exit(0);
        }
    }
}
