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
using System.Net;

namespace RedoxDataTaskSvc
{
    /// <summary>
    /// Encapsulates query operations to a Redox "Source"
    /// </summary>
    public class RedoxQueryInterface
    {
        private Mutex Mutx;
        HttpClient client = new HttpClient();

        Uri RedoxDomain = new Uri(@"https://api.redoxengine.com/");

        // TODO The following should be obtained from configuration?
        private string _ApiKey = @"317248c4-3b22-416b-80b8-9e6380d006f8";  // This is for Redox source "MillimanQuerySource"
        private string _Secret = @"723A1313-84A2-44B2-95A0-B7BECB78E4D8";  // This is for Redox source "MillimanQuerySource"

        private string _AccessToken, _RefreshToken;
        private DateTime _Expires;
        private TimeSpan _TokenDuration = new TimeSpan(24,0,0);

        /// <summary>
        /// Property indicating whether a current access token exists for the Redox source
        /// </summary>
        private bool AuthenticationIsCurrent
        {
            get
            {
                TimeSpan MaxTokenAge = _TokenDuration - new TimeSpan(0, 30, 0);  // 1/2 hour less than actual expiry

                return ( !String.IsNullOrEmpty(_AccessToken) &&
                         !String.IsNullOrEmpty(_RefreshToken) &&
                         _Expires > (DateTime.UtcNow - MaxTokenAge) );
            }
        }

        /// <summary>
        /// Authenticates to the server and starts a timer that periodically refreshes the token
        /// </summary>
        public RedoxQueryInterface()
        {
            Mutx = new Mutex();
            TimeSpan RefreshTimeSpan = new TimeSpan(4, 0, 0);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            client.BaseAddress = RedoxDomain;

            UnAuthenticate();
            if (!Authenticate())
            {
                throw new Exception("Failed to authenticate to Redox source");
            }

            Timer RefreshTimer = new Timer(RefreshAuthentication, null, RefreshTimeSpan, RefreshTimeSpan);
        }

        /// <summary>
        /// Clears the access and refresh tokens and sets the expiration timestamp to a long time ago
        /// </summary>
        /// <param name="Arg"></param>
        private void UnAuthenticate(Object Arg = null)
        {
            Mutx.WaitOne();

            _AccessToken = "";
            _RefreshToken = "";
            _Expires = DateTime.UtcNow - new TimeSpan(1000, 0, 0, 0);

            Mutx.ReleaseMutex();
        }

        /// <summary>
        /// Initiates an attempt to refresh authentication
        /// </summary>
        /// <param name="Arg"></param>
        private void RefreshAuthentication(Object Arg)
        {
            Authenticate(true);
        }

        /// <summary>
        /// Authenticates to Redox service iff no prior authentication has occurred or the most recent authentication is expired
        /// </summary>
        /// <param name="Refresh">If true, forces attempt to refresh existing access token; if false authenticates from scratch</param>
        /// <returns>boolean indicating whether an unexpired access token exists at the end of this method</returns>
        public bool Authenticate(bool DoRefresh = false)
        {
            string Uri;
            string ResponseBody;

            Mutx.WaitOne();

            if (DoRefresh)
            {
                Uri = @"auth/refreshToken";
                ResponseBody = PostJObjectToRedox(Uri, new JObject(new JProperty("apiKey", _ApiKey), new JProperty("refreshToken", _RefreshToken)), "application/json");
            }
            else
            {
                Uri = @"auth/authenticate";
                ResponseBody = PostJObjectToRedox(Uri, new JObject(new JProperty("apiKey", _ApiKey), new JProperty("secret", _Secret)), "application/json");
            }

            JObject Response;
            try
            {
                Response = JObject.Parse(ResponseBody);
            }
            catch (Exception e)
            {
                string Error = e.Data + "\n" + e.StackTrace;
                Mutx.ReleaseMutex();
                return false;
            }

            foreach (JProperty Prop in Response.Properties())
            {
                switch (Prop.Name.ToUpper())
                {
                    case "ACCESSTOKEN":
                        _AccessToken = Prop.Value.Value<string>();
                        break;
                    case "EXPIRES":
                        _Expires = DateTime.Parse(Prop.Value.Value<string>());
                        _TokenDuration = _Expires - DateTime.UtcNow;
                        break;
                    case "REFRESHTOKEN":
                        _RefreshToken = Prop.Value.Value<string>();
                        break;
                    default:
                        Mutx.ReleaseMutex();
                        throw new Exception("Unexpected property encountered in authentication response from Redox");
                }
            }

            Mutx.ReleaseMutex();

            return AuthenticationIsCurrent;
        }

        /// <summary>
        /// Submits a POST to Redox using the supplied Json content in the request body
        /// </summary>
        /// <param name="QueryObject"></param>
        /// <returns>Http response body as JObject</returns>
        public JObject QueryRedox(JObject QueryObject)
        {
            string Uri = @"query";

            string ResponseString = PostJObjectToRedox(Uri, QueryObject, "application/json");

            try
            {
                JObject ResponseObject = JObject.Parse(ResponseString);
                return ResponseObject;
            }
            catch (Exception /*e*/)
            {
                return new JObject();
            }
        }

        /// <summary>
        /// Submits a POST to a supplied Uri using a supplied Jobject as the source of request body
        /// </summary>
        /// <param name="Uri">The destination for the Http POST request</param>
        /// <param name="Payload">The request body to send</param>
        /// <param name="AcceptContentType">The expected type for the response body</param>
        /// <returns>The response body as String type</returns>
        private string PostJObjectToRedox(string Uri, JObject Payload, string AcceptContentType)
        {
            string Body = JsonConvert.SerializeObject(Payload);
            return PostRequestToRedox(Uri, "application/json", Body, AcceptContentType);
        }

        /// <summary>
        /// Submits a POST to a supplied Uri using a supplied String as the request body
        /// </summary>
        /// <param name="Uri"></param>
        /// <param name="ContentType">Describes the message content contained in the Body argument</param>
        /// <param name="Body">The request body to send</param>
        /// <param name="AcceptContentType">The expected type for the response body</param>
        /// <returns></returns>
        private string PostRequestToRedox(string Uri, string ContentType, string Body, string AcceptContentType)
        {
            Uri FullUri = new Uri(this.RedoxDomain, Uri);

            HttpResponseMessage response;
            //Body = "{'Meta':{'DataModel':'Scheduling','EventType':'New','EventDateTime':'2016-03-23T20:01:33.304Z','Test':true,'Source':{'ID':'7ce6f387-c33c-417d-8682-81e83628cbd9','Name':'Redox Dev Tools'},'Message':{ 'ID':5565},'Transmission':{ 'ID':1125106},'FacilityCode':null},'Patient':{'Identifiers':[{'ID':'0000000001','IDType':'MR'},{'ID':'e167267c-16c9-4fe3-96ae-9cff5703e90a','IDType':'REDOX'}],'Demographics':{'FirstName':'Timothy','LastName':'Bixby','DOB':'2008-01-06','SSN':'101-01-0001','Sex':'Male','Race':'White','MaritalStatus':'Single','PhoneNumber':{'Home':'+18088675301','Office':null,'Mobile':null},'EmailAddresses':[],'Address':{'StreetAddress':'4762 Hickory Street','City':'Monroe','State':'WI','ZIP':'53566','County':'Green','Country':'US'}},'Notes':[]},'Visit':{'VisitNumber':'1234','VisitDateTime':'2016-03-24T17:51:22.033Z','Duration':15,'Reason':'Checkup','Instructions':null,'AttendingProvider':{'ID':4356789876,'IDType':'NPI','FirstName':'Pat','LastName':'Granite','Credentials':['MD'],'Address':{'StreetAddress':'123 Main St.','City':'Madison','State':'WI','ZIP':'53703','County':'Dane','Country':'USA'},'Location':{'Type':null,'Facility':null,'Department':null},'PhoneNumber':{'Office':null}},'ConsultingProvider':{'ID':null,'IDType':null,'FirstName':null,'LastName':null,'Credentials':[],'Address':{'StreetAddress':null,'City':null,'State':null,'ZIP':null,'County':null,'Country':null},'Location':{'Type':null,'Facility':null,'Department':null},'PhoneNumber':{'Office':null}},'ReferringProvider':{'ID':null,'IDType':null,'FirstName':null,'LastName':null,'Credentials':[],'Address':{'StreetAddress':null,'City':null,'State':null,'ZIP':null,'County':null,'Country':null},'Location':{'Type':null,'Facility':null,'Department':null},'PhoneNumber':{'Office':null}},'Location':{'Type':null,'Facility':null,'Department':'3S'},'Diagnoses':[{'Code':'034.0','Codeset':'ICD-9','Name':'Strepthroat'}]}}";

            Mutx.WaitOne();

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

            Mutx.ReleaseMutex();

            string ResponseBody = "";
            ResponseBody = response.Content.ReadAsStringAsync().Result;

            // test for failed operation
            if (!response.IsSuccessStatusCode || response.Content.Headers.ContentType.MediaType != AcceptContentType)
            {
                response.Headers.ToList().ForEach( x => {foreach (string v in x.Value) Trace.WriteLine("Response header> " + x.Key + ": " + v); });
            }

            return ResponseBody;
        }

    }

}