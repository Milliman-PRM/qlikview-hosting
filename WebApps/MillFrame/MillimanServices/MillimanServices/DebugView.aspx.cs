using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Milliman
{
    public partial class DebugView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
   
        }

        protected override void Render(HtmlTextWriter writer)
        {
            lock (Global.ClientActionLock)
            {
                string ColorTemplate = "<span style='color:_COLOR_'>_MSG_</span><br>";
                List<Global.SystemMsg> Msgs = Global.GetClientActions();
                foreach (Global.SystemMsg S in Msgs)
                {
                    string Entry = ColorTemplate.Replace("_MSG_", S.Msg);
                    if (S.MType == Global.MsgType.ERROR)
                        Entry = Entry.Replace("_COLOR_", "red");
                    else if (S.MType == Global.MsgType.WARNING)
                        Entry = Entry.Replace("_COLOR_", "orange");
                    else
                        Entry = Entry.Replace("_COLOR_", "green");
                    writer.Write(Entry);
                }

                if (Msgs.Count == 0)
                {
                    string Entry = ColorTemplate.Replace("_MSG_", "No debug information is available");
                    Entry = Entry.Replace("_COLOR_", "green");
                    writer.Write(Entry);
                }
            }
            base.Render(writer);
        }
    }
}