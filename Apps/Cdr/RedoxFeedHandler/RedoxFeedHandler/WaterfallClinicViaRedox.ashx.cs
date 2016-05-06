using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using RedoxCacheDbLib;

namespace RedoxFeedHandler
{
    /// <summary>
    /// A web endpoint that handles unsolicited scheduling messages.  Implements the requirements for a Redox "destination"
    /// </summary>
    public class WaterfallClinicViaRedox : IHttpHandler
    {
        private string LogFileName = Path.Combine(@"C:\RedoxFeedHandler\RedoxFeedTest", "HandlerRequest.txt");

        // The verification token is associated with the "destination" configured on the Redox web site
        // TODO Get this from configuration
        string ExpectedVerificationToken = "83123D10-3436-4AC4-B479-1748DEDFE8F6";
        string ExpectedApplicationName = "RedoxEngine";

        /// <summary>
        /// The entry point handler function that is invoked by the web site
        /// </summary>
        /// <param name="context">An HttpContext instance that encapsulates the request and response</param>
        public void ProcessRequest(HttpContext context)
        {
            context.Response.TrySkipIisCustomErrors = true;  // Discourage IIS from replacing the response when an unsupported StatusCode is set
            context.Response.ContentType = "text/plain";

            string ActualVerificationToken = context.Request.Headers["verification-token"];
            string ActualApplicationName = context.Request.Headers["application-name"];

            if (ActualApplicationName != ExpectedApplicationName)
            {
                context.Response.Write("Header >application-name< not as expected.\n");
                context.Response.StatusCode = 422;
            }
            else if (ActualVerificationToken != ExpectedVerificationToken)
            {
                context.Response.Write("Header >verification-token< not as expected.\n");
                context.Response.StatusCode = 422;
            }
            else
            {
                switch (context.Request.HttpMethod.ToUpper())
                {
                    case "POST":   // POST for actual data message
                        HandleVerifiedPost(context);
                        break;

                    case "GET":    // GET for api test message from Redox
                        HandleVerifiedGet(context);
                        break;

                    default:
                        context.Response.Write("Unsupported HTTP method " + context.Request.HttpMethod + " was received by the handler.");
                        context.Response.StatusCode = 405;
                        return;
                }
            }
        }

        /// <summary>
        /// Processes a POST request
        /// </summary>
        /// <param name="context">The HttpContext instance that encapsulates the request and response</param>
        private void HandleVerifiedPost(HttpContext context)
        {
            StreamWriter LogFile = new StreamWriter(LogFileName);

            // Get the entire body of the POST
            string FullContentString;
            using (StreamReader ContentStream = new StreamReader(context.Request.InputStream))
            {
                FullContentString = ContentStream.ReadToEnd();
                // convert to indent formatted text
                FullContentString = JsonConvert.SerializeObject(JObject.Parse(FullContentString), Formatting.Indented);
            }

#region temporary code to log all received message content
            // log HTTP headers and body
            foreach (string H in context.Request.Headers)
            {
                LogFile.WriteLine("Header: " + H + ": " + context.Request.Headers[H]);
            }
            LogFile.WriteLine("Body: " + FullContentString);
            LogFile.Flush();
#endregion

            // test for unsupported message ContentType, typically encoded as multiple values so can't test for simple equality
            if (context.Request.ContentType.IndexOf(@"application/json") == -1)
            {
                string Msg = "Received unexpected ContentType header value: " + context.Request.ContentType;
                context.Response.AddHeader("Error", Msg);
                context.Response.Write(Msg);
                LogFile.WriteLine(Msg);
                LogFile.Close();
                context.Response.StatusCode = 200;
                return;
            }

            // Extract needed metadata
            JObject DocJson = JObject.Parse(FullContentString);
            // or   DocJson = JsonConvert.DeserializeObject<JObject>(ContentString);
            RedoxMeta Meta = new RedoxMeta(DocJson.Property("Meta"));
            long Transmission = Meta.TransmissionNumber;
            String SourceId = Meta.Source.Value["ID"].Value<String>();
            String SourceName = Meta.Source.Value["Name"].Value<String>();
            String EventType = Meta.EventType.Value.Value<String>();

#region Task Queue Persistence
            // For Dev/Test: If testing with Tom's PG server use the needed database connection string
            string CxStr = Environment.MachineName == "IN-PUCKETTT" ?
                ConfigurationManager.ConnectionStrings["RedoxCacheContextConnectionStringPort5433"].ConnectionString :
                ConfigurationManager.ConnectionStrings["RedoxCacheContextConnectionStringPort5432"].ConnectionString;

            // Instantiate interface to database
            RedoxCacheDbInterface Db = RedoxCacheDbInterface.CreateNewInstance(CxStr);

            // store the received message appropriately depending on message type
            switch (Meta.DataModelString.ToUpper())
            {
                case "SCHEDULING":
                    long NewRecord = Db.InsertSchedulingRecord(Transmission, SourceId, SourceName, FullContentString, EventType);
                    break;

                default:
                    LogFile.WriteLine("Received unsupported message DataModel from Redox: {0}", Meta.DataModelString);
                    break;
            }
#endregion

            LogFile.Close();
            context.Response.StatusCode = 200;
        }

        /// <summary>
        /// Processed a GET request
        /// </summary>
        /// <param name="context">The HttpContext instance that encapsulates the request and response</param>
        private void HandleVerifiedGet(HttpContext context)
        {
            string ActualChallenge = context.Request.QueryString["challenge"];

            context.Response.Write(ActualChallenge);
            context.Response.StatusCode = 200;
        }

        /// <summary>
        /// A class that implements IHttpHandler must provide this property.  
        /// </summary>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}