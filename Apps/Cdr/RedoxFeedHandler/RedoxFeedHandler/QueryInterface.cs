using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace RedoxFeedHandler
{
    public class QueryInterface
    {
        HttpClient client = new HttpClient();
        Uri RedoxDomain = new Uri(@"https://api.redoxengine.com/");
        private string _ApiKey = @"317248c4-3b22-416b-80b8-9e6380d006f8";
        private string _Secret = @"723A1313-84A2-44B2-95A0-B7BECB78E4D8";
        private string _AccessToken = "", _Expires = "", _RefreshToken = "";  // returned from authentication service call

        public bool IsAuthenticated
        {
            get
            {
                return (_AccessToken != "" && _Expires != "" && _RefreshToken != "");
            }
        }

        public QueryInterface()
        {
            client.BaseAddress = RedoxDomain;
            Authenticate();
        }

        public bool Authenticate()
        {
            string Uri = @"auth/authenticate";
            string ResponseBody;
            ResponseBody = PostJObjectToRedox(Uri, new JObject(new JProperty("apiKey", _ApiKey), new JProperty("secret", _Secret)), "application/json");

            // Clear to forget any old values
            _AccessToken = _Expires = _RefreshToken = "";

            JObject Resp;
            try
            {
                Resp = JObject.Parse(ResponseBody);
            }
            catch (Exception e)
            {
                string Error = e.Data + "\n" + e.StackTrace;
                return false;
            }

            foreach (JProperty Prop in Resp.Properties())
            {
                switch (Prop.Name.ToUpper())
                {
                    case "ACCESSTOKEN":
                        _AccessToken = Prop.Value.Value<string>();
                        break;
                    case "EXPIRES":
                        _Expires = Prop.Value.Value<string>();
                        break;
                    case "REFRESHTOKEN":
                        _RefreshToken = Prop.Value.Value<string>();
                        break;
                    default:
                        throw new Exception("Unexpected property encountered in authentication response from Redox");
                }
            }

            return (_AccessToken == "" || _Expires == "" || _RefreshToken == "") ? false : true;
        }

        public bool RefreshAuthentication()
        {
            string Uri = @"auth/refreshToken";

            return true;
        }

        public JObject QueryForClinicalSummary(JObject Patient)
        {
            string Uri = @"query";

#region test
            JProperty MetaProp = new JProperty("Meta", new JObject(
                new JProperty("DataModel", "Clinical Summary"),
                new JProperty("EventType", "Query"),
                new JProperty("EventDateTime", DateTime.UtcNow),
                new JProperty("Test", true),
                new JProperty("Destinations", new JObject[] {
                    new JObject(
                        new JProperty("ID", "ef9e7448-7f65-4432-aa96-059647e9b357"),
                        new JProperty("Name", "Clinical Summary Endpoint")
                    )
                }) 
            ) );
            JProperty PatientProp = new JProperty("Patient", new JObject(
                new JProperty("Identifiers", new JObject[] {
                    new JObject(
                        new JProperty("ID", "ffc486eff2b04b8^^^&1.3.6.1.4.1.21367.2005.13.20.1000&ISO"),
                        new JProperty("IDType", "NIST")
                        // This is a hard coded test patient query straight from the Redox web site
                    )
                })
            ) );

            Patient = new JObject(MetaProp, PatientProp);

            string test = JsonConvert.SerializeObject(Patient, Formatting.Indented);

            string PatientCcdString = PostJObjectToRedox(Uri, Patient, "application/json");

            JObject PatientCcdObject = JObject.Parse(PatientCcdString);
#endregion test

            return PatientCcdObject;
        }

        private string PostJObjectToRedox(string Uri, JObject Payload, string AcceptContentType)
        {
            string Body = JsonConvert.SerializeObject(Payload);
            return PostRequestToRedox(Uri, "application/json", Body, AcceptContentType);
        }

        private string PostRequestToRedox(string Uri, string ContentType, string Body, string AcceptContentType)
        {
            Uri FullUri = new Uri(this.RedoxDomain, Uri);

            HttpResponseMessage response;
            //Body = "{'Meta':{'DataModel':'Scheduling','EventType':'New','EventDateTime':'2016-03-23T20:01:33.304Z','Test':true,'Source':{'ID':'7ce6f387-c33c-417d-8682-81e83628cbd9','Name':'Redox Dev Tools'},'Message':{ 'ID':5565},'Transmission':{ 'ID':1125106},'FacilityCode':null},'Patient':{'Identifiers':[{'ID':'0000000001','IDType':'MR'},{'ID':'e167267c-16c9-4fe3-96ae-9cff5703e90a','IDType':'REDOX'}],'Demographics':{'FirstName':'Timothy','LastName':'Bixby','DOB':'2008-01-06','SSN':'101-01-0001','Sex':'Male','Race':'White','MaritalStatus':'Single','PhoneNumber':{'Home':'+18088675301','Office':null,'Mobile':null},'EmailAddresses':[],'Address':{'StreetAddress':'4762 Hickory Street','City':'Monroe','State':'WI','ZIP':'53566','County':'Green','Country':'US'}},'Notes':[]},'Visit':{'VisitNumber':'1234','VisitDateTime':'2016-03-24T17:51:22.033Z','Duration':15,'Reason':'Checkup','Instructions':null,'AttendingProvider':{'ID':4356789876,'IDType':'NPI','FirstName':'Pat','LastName':'Granite','Credentials':['MD'],'Address':{'StreetAddress':'123 Main St.','City':'Madison','State':'WI','ZIP':'53703','County':'Dane','Country':'USA'},'Location':{'Type':null,'Facility':null,'Department':null},'PhoneNumber':{'Office':null}},'ConsultingProvider':{'ID':null,'IDType':null,'FirstName':null,'LastName':null,'Credentials':[],'Address':{'StreetAddress':null,'City':null,'State':null,'ZIP':null,'County':null,'Country':null},'Location':{'Type':null,'Facility':null,'Department':null},'PhoneNumber':{'Office':null}},'ReferringProvider':{'ID':null,'IDType':null,'FirstName':null,'LastName':null,'Credentials':[],'Address':{'StreetAddress':null,'City':null,'State':null,'ZIP':null,'County':null,'Country':null},'Location':{'Type':null,'Facility':null,'Department':null},'PhoneNumber':{'Office':null}},'Location':{'Type':null,'Facility':null,'Department':'3S'},'Diagnoses':[{'Code':'034.0','Codeset':'ICD-9','Name':'Strepthroat'}]}}";

#if false
            HttpRequestMessage request = new HttpRequestMessage();
            request.Content = new StringContent(Body);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(AcceptContentType));
            request.RequestUri = FullUri;
            request.Method = HttpMethod.Post;

            response = client.SendAsync(request).Result;
#else
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(AcceptContentType));
            //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            //client.DefaultRequestHeaders.Add("application-name", "RedoxEngine");
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _AccessToken);
            //client.DefaultRequestHeaders.Add("verification-token", "83123D10-3436-4AC4-B479-1748DEDFE8F6");

            JObject Payload = JObject.Parse(Body);
            response = client.PostAsJsonAsync(Uri, Payload).Result;
#endif

            string ResponseBody = "";
            ResponseBody = response.Content.ReadAsStringAsync().Result;

            // test for failed operation
            if (!response.IsSuccessStatusCode || response.Content.Headers.ContentType.MediaType != AcceptContentType)
            {
                response.Headers.ToList().ForEach( x => {foreach (string v in x.Value) Debug.WriteLine("Response header> {0}: {1}", x.Key, v); });
            }

            return ResponseBody;
        }
    }

}