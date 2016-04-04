using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace RedoxFeedHandler
{
    /// <summary>
    /// TODO Figure out how to conveniently make this serialize/deserialize to/from JSON.  
    /// </summary>
    public class RedoxMeta
    {
        private string _DataModel = "";
        private string _EventType = "";
        private Nullable<DateTime> _EventDateTime;
        private bool _Test;
        private Dictionary<string,string> _Source = new Dictionary<string, string>();
        private Dictionary<string, string> _Message = new Dictionary<string, string>();
        private Dictionary<string, string> _Transmission = new Dictionary<string, string>();
        private string _FacilityCode = null;

        public RedoxMeta()
        {}

        public RedoxMeta(JProperty MetaProperty)
        {
            if (MetaProperty.Name != "Meta")
            {
                throw new Exception("Can't instantiate RedoxMeta with incorrect constructor argument, property name is >" + MetaProperty.Name + "<, not 'Meta'");
            }
            if (MetaProperty.Value.Type != JTokenType.Object)
            {
                throw new Exception("Can't instantiate RedoxMeta with incorrect constructor argument, property value type is >" + MetaProperty.Value.Type.ToString() + "<, not 'Object'");
            }
            JObject Val = MetaProperty.Value.ToObject<JObject>();

            DataModel = Val.Property("DataModel");
            EventType = Val.Property("EventType");
            EventDateTime = Val.Property("EventDateTime");
            Test = Val.Property("Test");
            Source = Val.Property("Source");
            Message = Val.Property("Message");
            Transmission = Val.Property("Transmission");
            EventType = Val.Property("EventType");
            FacilityCode = Val.Property("FacilityCode");
        }

        public JProperty DataModel
        {
            set {
                if (value.Type == JTokenType.Property && value.Name == "DataModel" && value.Value.Type == JTokenType.String)
                {
                    _DataModel = value.Value.ToObject<string>();
                }
                else throw new Exception("Invalid JToken type for property DataModel");
            }
            get
            {
                return new JProperty("DataModel", _DataModel);
            }
        }

        public JProperty EventType
        {
            set
            {
                if (value.Type == JTokenType.Property && value.Name == "EventType" && value.Value.Type == JTokenType.String)
                {
                    _EventType = value.Value.ToObject<string>();
                }
                else throw new Exception("Invalid JToken type for property EventType");
            }
            get
            {
                return new JProperty("EventType", _EventType);
            }
        }

        public JProperty EventDateTime
        {
            set
            {
                if (value.Type == JTokenType.Property && value.Name == "EventDateTime" && value.Value.Type == JTokenType.Date)
                {
                    _EventDateTime = value.Value.ToObject<DateTime>();
                }
                else throw new Exception("Invalid JToken type for property EventDateTime");
            }
            get
            {
                return new JProperty("EventDateTime", _EventDateTime);
            }
        }

        public JProperty Test
        {
            set
            {
                if (value.Type == JTokenType.Property && value.Name == "Test" && value.Value.Type == JTokenType.Boolean)
                {
                    _Test = value.Value.ToObject<bool>();
                }
                else throw new Exception("Invalid JToken type for property Test");
            }
            get
            {
                return new JProperty("Test", _Test);
            }
        }

        public JProperty Source
        {
            set
            {
                _Source = new Dictionary<string, string>();
                if (value.Type == JTokenType.Property && value.Name == "Source" && value.Value.Type == JTokenType.Object)
                {
                    JObject Val = value.Value.ToObject<JObject>();
                    foreach (JProperty Prop in Val.Properties())
                    {
                        _Source[Prop.Name] = Prop.Value.ToObject<string>();
                    }
                }
                else throw new Exception("Invalid JProperty type, name, or value type for RedoxMeta property 'Source'");
            }

            get
            {
                JObject ReturnObj = new JObject();
                foreach (string Key in _Source.Keys)
                {
                    ReturnObj.Add(Key, _Source[Key]);
                }
                return new JProperty("Source", ReturnObj);
            }
        }

        public JProperty Message
        {
            set
            {
                _Message = new Dictionary<string, string>();
                if (value.Type == JTokenType.Property && value.Name == "Message" && value.Value.Type == JTokenType.Object)
                {
                    JObject Val = value.Value.ToObject<JObject>();
                    foreach (JProperty Prop in Val.Properties())
                    {
                        _Message[Prop.Name] = Prop.Value.ToObject<string>();
                    }
                }
                else throw new Exception("Invalid JProperty type, name, or value type for RedoxMeta property 'Message'");
            }

            get
            {
                JObject ReturnObj = new JObject();
                foreach (string Key in _Message.Keys)
                {
                    ReturnObj.Add(Key, Int64.Parse(_Message[Key]));
                }
                return new JProperty("Message", ReturnObj);
            }
        }

        public JProperty Transmission
        {
            set
            {
                _Transmission = new Dictionary<string, string>();
                if (value.Type == JTokenType.Property && value.Name == "Transmission" && value.Value.Type == JTokenType.Object)
                {
                    JObject Val = value.Value.ToObject<JObject>();
                    foreach (JProperty Prop in Val.Properties())
                    {
                        _Transmission[Prop.Name] = Prop.Value.ToObject<string>();
                    }
                }
                else throw new Exception("Invalid JProperty type, name, or value type for RedoxMeta property 'Transmission'");
            }

            get
            {
                JObject ReturnObj = new JObject();
                foreach (string Key in _Transmission.Keys)
                {
                    ReturnObj.Add(Key, Int64.Parse(_Transmission[Key]));
                }
                return new JProperty("Transmission", ReturnObj);
            }
        }

        public long TransmissionNumber
        {
            get
            {
                return long.Parse(_Transmission["ID"]);
            }
        }


        public JProperty FacilityCode
        {
            set
            {
                if (value.Type == JTokenType.Property && value.Name == "FacilityCode")
                {
                    switch (value.Value.Type)
                    {
                        case JTokenType.String:
                            _FacilityCode = value.Value.ToObject<string>();
                            break;
                        case JTokenType.Null:
                            _FacilityCode = null;
                            break;
                    }
                }
                else throw new Exception("Invalid JToken type for property FacilityCode");
            }
            get
            {
                return new JProperty("FacilityCode", _FacilityCode);
            }
        }


    }
}

