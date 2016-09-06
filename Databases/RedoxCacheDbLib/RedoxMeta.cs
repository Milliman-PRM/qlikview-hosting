using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace RedoxCacheDbLib
{

    /// <summary>
    /// A data class representing the Meta property that is expected in unsolicited datamodel messages from Redox
    /// </summary>
    public class RedoxMeta
    {
        private string _DataModel = null;
        private string _EventType = null;
        private Nullable<DateTime> _EventDateTime;
        private Nullable<bool> _Test;
        private Dictionary<string,string> _Source = new Dictionary<string, string>();
        private Dictionary<string, string> _Message = new Dictionary<string, string>();
        private Dictionary<string, string> _Transmission = new Dictionary<string, string>();
        private string _FacilityCode = null;

        public RedoxMeta()
        {}

        /// <summary>
        /// Constructor, parses Json and initializes member variables
        /// </summary>
        /// <param name="MetaProperty">A JProperty named 'Meta', containing serialized Json representation of message Meta information</param>
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

            // fill in all instance properties, using parsing code of each property setter
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

        /// <summary>
        /// Member that represents the DataModel property of the Redox message as JProperty type
        /// </summary>
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

        /// <summary>
        /// Getter that returns a string representation of the Redox datamodel as JProperty type
        /// </summary>
        public String DataModelString
        {
            get
            {
                return _DataModel;
            }
        }

        /// <summary>
        /// Member that represents the EventType property of the Redox message as JProperty type
        /// </summary>
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

        /// <summary>
        /// Member that represents the EventDateTime property of the Redox message as JProperty type
        /// </summary>
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

        /// <summary>
        /// Member that represents the Test property of the Redox message as JProperty type
        /// </summary>
        public JProperty Test
        {
            set
            {
                if (value.Type == JTokenType.Property && value.Name == "Test")
                {
                    switch (value.Value.Type)
                    {
                    case JTokenType.Boolean:
                        _Test = value.Value.ToObject<bool>();
                        break;
                    case JTokenType.Null:
                        _Test = null;
                        break;
                    }
                }
                else throw new Exception("Invalid JToken type for property Test");
            }
            get
            {
                return new JProperty("Test", _Test);
            }
        }

        /// <summary>
        /// Member that represents the Source property of the Redox message as JProperty type
        /// </summary>
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

        /// <summary>
        /// Member that represents the Message property of the Redox message as JProperty type
        /// </summary>
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

        /// <summary>
        /// Member that represents the Transmission property of the Redox message as JProperty type
        /// </summary>
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

        /// <summary>
        /// Getter that returns the Transmissioin Number property of the Redox message as long type
        /// </summary>
        public long TransmissionNumber
        {
            get
            {
                return long.Parse(_Transmission["ID"]);
            }
        }

        /// <summary>
        /// Member that represents the FacilityCode property of the Redox message as JProperty type
        /// </summary>
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

    }  // class
}  // namespace

