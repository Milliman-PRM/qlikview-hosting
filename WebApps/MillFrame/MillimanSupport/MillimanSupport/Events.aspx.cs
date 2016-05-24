using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MillimanSupport
{
    public partial class Events : System.Web.UI.Page
    {
       
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadScheduleData();
            }
        }

        private void LoadScheduleData()
        {

            Calendar.CalendarItemCollection CIC = new Calendar.CalendarItemCollection();
            CIC.LoadEventList();

            foreach (Calendar.CalendarItem CI in CIC.EventList)
            {
                Telerik.Web.UI.Appointment AP1 = new Telerik.Web.UI.Appointment();
                AP1.Subject = CI.Subject;
                AP1.Font.Bold = CI.IsBold;
                AP1.Font.Size = new FontUnit(CI.FontSize);
                AP1.Font.Strikeout = CI.IsStrikeOut;
                AP1.Font.Italic = CI.IsItalic;
                AP1.Font.Name = CI.FontName;
                AP1.ForeColor = CI.FontColor;
                AP1.Start = CI.Start;
                AP1.End = CI.End;
                AP1.ToolTip = CI.ToolTip;
                AP1.BackColor = CI.BackColor;
                Schedule.InsertAppointment(AP1);
            }
        }
    }
}