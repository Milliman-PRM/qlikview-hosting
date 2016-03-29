using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace RedoxFeedHandler
{
    /// <summary>
    /// TODO Figure out how to conveniently make this serialize/deserialize to/from JSON.  
    /// </summary>
    class RedoxMeta
    {
        private string _DataModel;
        private string _EventType;
        private Nullable<DateTime> _EventDateTime;
        private bool _Test;
        private string _SourceId;
        private string _SourceName;
        private string _MessageId;
        private int _TransmissionId;
        private string _FacilityCode;

        public JToken DataModel
        {
            set {
                if (value.Type == JTokenType.String)
                {
                    _DataModel = value.Value<string>();
                }
                else throw new Exception("Invalid JToken type for property DataModel");
            }
        }

        public JToken EventType
        {
            set {
                if (value.Type == JTokenType.String)
                {
                    _EventType = value.Value<string>();
                }
                else throw new Exception("Invalid JToken type for property EventType");
            }
        }

        public JToken EventDateTime
        {
            set {
                if (value.Type == JTokenType.Date)
                {
                    _EventDateTime = value.Value<Nullable<DateTime>>();
                }
                else throw new Exception("Invalid JToken type for property EventDateTime");
            }
        }

        public JToken Test
        {
            set {
                if (value.Type == JTokenType.Boolean)
                {
                    _Test = value.Value<bool>();
                }
                else throw new Exception("Invalid JToken type for property Test");
            }
        }

        public JToken Source
        {
            set
            {
                if (value.Type == JTokenType.Object)
                {
                    _SourceId = value["ID"].Value<string>();
                    _SourceName = value["Name"].Value<string>();
                }
                else throw new Exception("Invalid JToken type for property Source");
            }
        }
        public JToken Message
        {
            set
            {
                if (value.Type == JTokenType.Object)
                {
                    _SourceId = value["ID"].Value<string>();
                    _SourceName = value["Name"].Value<string>();
                }
                else throw new Exception("Invalid JToken type for property Message");
            }
        }

        public JToken Transmission
        {
            set
            {
                if (value.Type == JTokenType.Object)
                {
                    _TransmissionId = value["ID"].Value<int>();
                }
                else throw new Exception("Invalid JToken type for property Transmission");
            }
        }


        public JToken FacilityCode
        {
            set
            {
                if (value.Type == JTokenType.String)
                {
                    _FacilityCode = value.Value<string>();
                }
                else throw new Exception("Invalid JToken type for property FacilityCode");
            }
        }


    }
}
