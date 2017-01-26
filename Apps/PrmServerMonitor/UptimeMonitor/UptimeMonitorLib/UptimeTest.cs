using System;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace UptimeMonitorLib
{
    public class UptimeTest
    {
        Timer TestStartTimer;
        TextWriterTraceListener LogWriter;
        HttpClient Client;

        public UptimeTest(int TimerIntervalSeconds)
        {
            Trace.AutoFlush = true;

            Client = new HttpClient();
            Client.BaseAddress = new Uri("https://prm.milliman.com/prm/time.aspx");
            Client.DefaultRequestHeaders.Accept.Clear();

            TestStartTimer = new Timer(TimerIntervalSeconds * 1000);
            TestStartTimer.Elapsed += PerformUptimeTest;
            TestStartTimer.AutoReset = true;
        }

        public void Start(string LogFolder)
        {
            string LogFileName = "UptimeMonitorLog_" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt";

            ResetTrace();
            LogWriter = new TextWriterTraceListener(Path.Combine(LogFolder, LogFileName));
            Trace.Listeners.Add(LogWriter);

            TestStartTimer.Start();
        }

        public void Stop()
        {
            TestStartTimer.Stop();

            LogWriter.Flush();
            LogWriter.Close();
            ResetTrace();
            LogWriter = null;
        }

        private void ResetTrace()
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new ConsoleTraceListener());
        }
        private void PerformUptimeTest(Object source, ElapsedEventArgs e)
        {
            //LogWriter.WriteLine("The Elapsed event was raised at " + e.SignalTime.ToString("HH:mm:ss.fff"));

            HttpResponseMessage Response = Client.GetAsync(Client.BaseAddress).Result;
            if (Response.IsSuccessStatusCode)
            {
                var Body = Response.Content.ReadAsStringAsync().Result;

                XmlDocument Doc = new XmlDocument();
                Doc.LoadXml(Body);

                XmlNodeList a = Doc.GetElementsByTagName("span");
                string LogText = "";

                foreach (XmlNode b in a)
                {
                    switch (b.Attributes["id"].Value)
                    {
                        // put the SystemTime at front of the string and Status at the back
                        case "Status":
                            byte[] DecodedStatus = System.Convert.FromBase64String(b.InnerText);
                            string x = new CopyOfMillimanCommon().AutoDecrypt(b.InnerText);
                            LogText = LogText + x;
                            //LogText = LogText + System.Text.Encoding.UTF8.GetString(x);
                            break;

                        case "SystemTime":
                            LogText = b.InnerText + " - " + LogText;
                            break;

                        default:
                            // something unexpected
                            break;
                    }
                }

                LogWriter.WriteLine(LogText);
            }
        }

    }

}
