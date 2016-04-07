using System;
using System.Media;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace RedoxFeedTestClient
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();

            comboBox1.SelectedIndex = 1;
            comboBox1.Focus();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            using (var client = new HttpClient())
            {
                if (comboBox1.SelectedText == comboBox1.Text)
                {
                    SystemSounds.Beep.Play();
                    comboBox1.Focus();
                    return;
                }

                string Domain = comboBox1.GetItemText(comboBox1.SelectedItem);
                string Uri = "RedoxFeedTest/WaterfallClinicViaRedox.ashx";

                if (Domain.IndexOf("localhost") != -1)
                {
                    int Temp;
                    if (!int.TryParse(textPort.Text, out Temp))
                    {
                        SystemSounds.Beep.Play();
                        textPort.Focus();
                        return;
                    }
                    Domain = Domain.Insert(Domain.Length - 1, ":" + textPort.Text);
                    Uri = Uri.Remove(0, "RedoxFeedTest/".Length);
                }

                client.BaseAddress = new Uri(Domain);   //  "http://PRMDev03.cloudapp.net/"
                client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("application-name", "RedoxEngine");
                client.DefaultRequestHeaders.Add("verification-token", "83123D10-3436-4AC4-B479-1748DEDFE8F6");

                HttpResponseMessage response;

                System.Windows.Forms.Button Sender = sender.GetType() == typeof(System.Windows.Forms.Button) ? (System.Windows.Forms.Button) sender : null;

                switch (Sender.Text)
                {
                    case "POST":
                        string x;
                        string Body = "{'Meta':{'DataModel':'Scheduling','EventType':'New','EventDateTime':'2016-03-23T20:01:33.304Z','Test':null,'Source':{'ID':'7ce6f387-c33c-417d-8682-81e83628cbd9','Name':'Redox Dev Tools'},'Message':{ 'ID':5565},'Transmission':{ 'ID':1125106},'FacilityCode':null},'Patient':{'Identifiers':[{'ID':'0000000001','IDType':'MR'},{'ID':'e167267c-16c9-4fe3-96ae-9cff5703e90a','IDType':'REDOX'}],'Demographics':{'FirstName':'Timothy','LastName':'Bixby','DOB':'2008-01-06','SSN':'101-01-0001','Sex':'Male','Race':'White','MaritalStatus':'Single','PhoneNumber':{'Home':'+18088675301','Office':null,'Mobile':null},'EmailAddresses':[],'Address':{'StreetAddress':'4762 Hickory Street','City':'Monroe','State':'WI','ZIP':'53566','County':'Green','Country':'US'}},'Notes':[]},'Visit':{'VisitNumber':'1234','VisitDateTime':'2016-03-24T17:51:22.033Z','Duration':15,'Reason':'Checkup','Instructions':null,'AttendingProvider':{'ID':4356789876,'IDType':'NPI','FirstName':'Pat','LastName':'Granite','Credentials':['MD'],'Address':{'StreetAddress':'123 Main St.','City':'Madison','State':'WI','ZIP':'53703','County':'Dane','Country':'USA'},'Location':{'Type':null,'Facility':null,'Department':null},'PhoneNumber':{'Office':null}},'ConsultingProvider':{'ID':null,'IDType':null,'FirstName':null,'LastName':null,'Credentials':[],'Address':{'StreetAddress':null,'City':null,'State':null,'ZIP':null,'County':null,'Country':null},'Location':{'Type':null,'Facility':null,'Department':null},'PhoneNumber':{'Office':null}},'ReferringProvider':{'ID':null,'IDType':null,'FirstName':null,'LastName':null,'Credentials':[],'Address':{'StreetAddress':null,'City':null,'State':null,'ZIP':null,'County':null,'Country':null},'Location':{'Type':null,'Facility':null,'Department':null},'PhoneNumber':{'Office':null}},'Location':{'Type':null,'Facility':null,'Department':'3S'},'Diagnoses':[{'Code':'034.0','Codeset':'ICD-9','Name':'Strepthroat'}]}}";

                        JObject Payload = JObject.Parse(Body);
                        response = await client.PostAsJsonAsync(Uri, Payload);
                        x = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            /*
                            Uri gizmoUrl = response.Headers.Location;
                            // HTTP PUT
                            gizmo.Price = 80;   // Update price
                            response = await client.PutAsJsonAsync(gizmoUrl, gizmo);
                            x = await response.Content.ReadAsStringAsync();
                            // HTTP DELETE
                            response = await client.DeleteAsync(gizmoUrl);
                            x = await response.Content.ReadAsStringAsync();
                            */
                        }
                        break;

                    case "GET":
                        string ChallengeString = Guid.NewGuid().ToString();
                        Uri = Uri + "?challenge=" + ChallengeString;
                        response = await client.GetAsync(Uri);
                        if (response.IsSuccessStatusCode)
                        {
                            string RspContentString = await response.Content.ReadAsStringAsync();
                            if (RspContentString != ChallengeString)
                            {
                                string MsgString = "response Content\n" + RspContentString + "\ndoes not match original challenge string:\n" + ChallengeString;
                                MessageBox.Show(MsgString);
                            }
                        }
                        break;
                }
            }
        }
    }

}

