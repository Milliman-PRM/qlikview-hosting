using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanSupport
{
    public partial class Status : System.Web.UI.Page
    {
        private OverviewType OverviewStatus = OverviewType.Good;
        private string OverviewMessage = "&nbsp;The Milliman PRM server has passed the diagnotics and is functioning properly";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BarChart.Legend.Appearance.Visible = false;

                GetRemoteStatus();
                GetHCIntelStatus();
                SetOverview(OverviewMessage, OverviewStatus);
            }
        }

        protected void AddBar(string Name, int Value, string MeasuredTime)
        {
            
            BarChart.PlotArea.XAxis.Items.Add(Name);
            BarChart.PlotArea.Series[0].Items.Add(Value);
            //BarChart.PlotArea.Series[0].Items[0].TooltipValue = Value.ToString() + "ms at " + MeasuredTime;
            if (Value <= 0)
            {
                if (string.IsNullOrEmpty(LinkMessage.Text) == true)
                    LinkMessage.Text += "*HCIntel server is not accessable from ";
                else
                    LinkMessage.Text += ", ";
                if (Name.IndexOf(',') != -1)
                {
                    Name = Name.Replace(',', '(');
                    Name += " )";
                }
                LinkMessage.Text += Name;
            }
        }

        protected enum ComponentType { Report, Data, User, Disk, Memory };
        protected void SetComponentStatus(ComponentType CT, bool StatusGood)
        {
            string Status = "~/Images/Sign-Select-icon32.png";
            string Message = " is functionally normally";

            if (StatusGood == false)
            {
                Status = "~/Images/abort-icon32.png";
                Message = " is in an error state.";
            }
            switch (CT)
            {
                case ComponentType.Report:
                    reportsubsystem.ImageUrl = Status;
                    reportsubsystem.ToolTip = "Report subsystem" + Message;
                    break;
                case ComponentType.Data:
                    datasubsystem.ImageUrl = Status;
                    datasubsystem.ToolTip = "Data subsystem" + Message;
                    break;
                case ComponentType.User:
                    usersubsystem.ImageUrl = Status;
                    usersubsystem.ToolTip = "User subsystem" + Message;
                    break;
                case ComponentType.Disk:
                    diskspace.ImageUrl = Status;
                    if (StatusGood)
                        diskspace.ToolTip = "Disk space is available";
                    else
                        diskspace.ToolTip = "No disk space is available";
                    break;
                case ComponentType.Memory:
                    memory.ImageUrl = Status;
                    if (StatusGood)
                        memory.ToolTip = "Memory is available";
                    else
                        memory.ToolTip = "Free memory is running low";
                    break;
            }
        }
        protected enum OverviewType { Good, Warning, Error };
        protected void SetOverview(string Message, OverviewType Status)
        {
            Overview.Text = Message;
            switch (Status)
            {
                case OverviewType.Good:
                    OverviewIcon.ImageUrl = "~/Images/Sign-Select-icon64.png";
                    break;
                case OverviewType.Warning:
                    OverviewIcon.ImageUrl = "~/Images/Alert-icon64.png";
                    break;
                case OverviewType.Error:
                    OverviewIcon.ImageUrl = "~/Images/abort-icon64.png";
                    break;
            }
        }

        protected void GetRemoteStatus()
        {
            //9b4e2ae62ae7f255e90f4755d1d8a3c3
            //string URL = "http://www.site24x7.com/api/xml/currentstatus?monitorname=Production Client Website&apikey=9b4e2ae62ae7f255e90f4755d1d8a3c3";
            //Comm Communications = new Comm();
            //string Response = Communications.Execute(URL);
            //return Response;

            MillimanSupport._24x7Monitoring.Monitoring24x7Helper Mon = new _24x7Monitoring.Monitoring24x7Helper();
            foreach (KeyValuePair<string, MillimanSupport._24x7Monitoring.Monitoring24x7Helper.MonitoringPair> MP in Mon.MonitorList)
            {
                AddBar(MP.Value.Location, MP.Value.ResponseTime, MP.Value.MonitoringTime);
                if (MP.Value.ResponseTime == 0)
                {
                    OverviewStatus = OverviewType.Warning;
                    OverviewMessage = "&nbsp;The HCIntel server is functioning properly, however some internet paths are reporting as not accessable.";
                }
            }
            ReportTime.Text = "Checked at " + Mon.MonitorTime;
            BarChart.ToolTip = ReportTime.Text;
            BarChart.ChartTitle.Text = "Remote Monitor Response Times(ms) as of " + Mon.MonitorTime;
            
        }

        private void GetHCIntelStatus()
        {
            HCIntelMonitoring.MonitoringHelper MH = new HCIntelMonitoring.MonitoringHelper();
            SetComponentStatus(ComponentType.Report, MH.ReportSubsystem);
            SetComponentStatus(ComponentType.Data, MH.DataSubsystem);
            SetComponentStatus(ComponentType.User, MH.UserSubsystem);
            SetComponentStatus(ComponentType.Disk, MH.Diskspace);
            SetComponentStatus(ComponentType.Memory, MH.Memory);
            if ((MH.ReportSubsystem == false) || (MH.DataSubsystem == false) ||
                (MH.UserSubsystem == false) || (MH.Diskspace == false) ||
                (MH.Memory == false))
            {
                OverviewStatus = OverviewType.Error;
                OverviewMessage = "&nbsp;The server is currently in an error state due to component failures.";
            }
        }
    }
}