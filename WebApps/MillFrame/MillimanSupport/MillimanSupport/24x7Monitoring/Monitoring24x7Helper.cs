using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MillimanSupport._24x7Monitoring
{
    public class Monitoring24x7Helper
    {
        public class MonitoringPair
        {
            public string Location;
            public int    ResponseTime;
            public string MonitoringTime;
            public MonitoringPair(string _Location, string _ResponseTime, string _MonitoringTime)
            {
                Location = _Location;
                ResponseTime = System.Convert.ToInt32(_ResponseTime);
                MonitoringTime = _MonitoringTime;
            }
        }
        public string MonitorTime { get; set; }
        public Dictionary<string, MonitoringPair> MonitorList = new Dictionary<string,MonitoringPair>();

        public Monitoring24x7Helper()
        {
            AccessServer(ResponseType.AsJSON);
        }

        private enum ResponseType { AsXML, AsJSON };
        private void AccessServer( ResponseType RespondAs = ResponseType.AsXML)
        {
            string RT = "xml";
            if ( RespondAs == ResponseType.AsJSON )
                RT = "json";
            string URL = "http://www.site24x7.com/api/" + RT + "/currentstatus?monitorname=Production Client Website&apikey=9b4e2ae62ae7f255e90f4755d1d8a3c3";
            Comm Communications = new Comm();
            string Response = Communications.Execute(URL);
            if (string.IsNullOrEmpty(Response) == false)
            {
                string[] Tokens = Response.Split(new char[] { '{', '}', ':', '"',',' }, StringSplitOptions.RemoveEmptyEntries);
                string Token = string.Empty;
                string NextToken = string.Empty;
                string LocationName = string.Empty;
                string LocationTime = string.Empty;
                string LocationStatus = string.Empty;

                for (int i = 2; i < Tokens.Length-1; i++) 
                {
                    Token = Tokens[i];
                    NextToken = Tokens[i + 1];
                    if (( string.Compare(Token,"[",true) == 0 ) && (string.Compare(NextToken,"status",true)==0 ))
                    {
                        if (string.IsNullOrEmpty(MonitorTime) == true)
                        {
                            MonitorTime = Tokens[i - 4] + ":" + Tokens[i - 3] + ":" + Tokens[i - 2].Substring(0, Tokens[i - 2].IndexOf('-'));
                            MonitorTime = MonitorTime.Replace("T", " ");

                            //DateTime DT = System.Convert.ToDateTime(MonitorTime);
                            //MonitorTime = DT.ToLocalTime().ToString();

                        }
                        LocationName = Tokens[i - 1];

                        if (string.Compare(Tokens[i + 2], "up", true) != 0)
                            LocationTime = "0";
                        else
                            LocationTime = Tokens[i + 8].Replace(" ms", "");
                        MonitorList.Add(LocationName, new MonitoringPair(LocationName, LocationTime, MonitorTime));

                    }
                    //if (string.Compare(Token, "time", true) == 0)
                    //{
                    //    //only take first time, it will be the oldest
                    //    if ( string.IsNullOrEmpty(MonitorTime))
                    //        MonitorTime = Tokens[i + 1] + ":" + Tokens[i + 2] + ":" + Tokens[i+3];
                    //}
                    //else if ( string.Compare(Token,"responsecode", true) == 0 )
                    //{
                    //    LocationStatus = Tokens[i+1];
                    //}
                    //else if (string.Compare(Token, "name", true) == 0)
                    //{
                    //    LocationName = Tokens[i+1];
                    //}
                    //else if (string.Compare(Token, "responsetime", true) == 0)
                    //{
                    //    LocationTime = Tokens[i+1];
                    //}

                    //if ( (string.IsNullOrEmpty(LocationStatus)== false) && (string.IsNullOrEmpty(LocationName) == false) && (string.IsNullOrEmpty(LocationTime) == false) )
                    //{
                    //    if (LocationStatus != "200")
                    //        LocationTime = "0";

                    //    if (MonitorList.ContainsKey(LocationName))
                    //        MonitorList[LocationName].ResponseTime = System.Convert.ToInt32(LocationTime);
                    //    else
                    //        MonitorList.Add(LocationName, new MonitoringPair(LocationName, LocationTime, MonitorTime));

                    //    LocationName = string.Empty;
                    //    LocationTime = string.Empty;
                    //    LocationStatus = string.Empty;
                    //}
                }
            }
        }
    }
}