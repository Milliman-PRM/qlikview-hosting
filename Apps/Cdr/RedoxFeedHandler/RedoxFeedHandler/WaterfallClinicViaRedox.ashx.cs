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
    /// Summary description for WaterfallClinicViaRedox
    /// </summary>
    public class WaterfallClinicViaRedox : IHttpHandler
    {
        private string LogFileName = Path.Combine(@"C:\RedoxFeedHandler\RedoxFeedTest", "Log.txt");

        public void ProcessRequest(HttpContext context)
        {
            string ExpectedVerificationToken = "83123D10-3436-4AC4-B479-1748DEDFE8F6";
            string ExpectedApplicationName   = "RedoxEngine";

            string ActualVerificationToken = context.Request.Headers["verification-token"];
            string ActualApplicationName = context.Request.Headers["application-name"];

            // HttpContext initial configuration
            context.Response.TrySkipIisCustomErrors = true;  // Don't hijack the response content when unsupported StatusCode is set
            context.Response.ContentType = "text/plain";

            switch (context.Request.HttpMethod.ToUpper())
            {
                case "POST":   // POST for actual data message
                    StreamWriter LogFile = new StreamWriter(LogFileName);

                    if (ActualApplicationName != ExpectedApplicationName)
                    {
                        LogFile.WriteLine("Header >application-name< not as expected.");
                        context.Response.Write("Header >application-name< not as expected.\n");
                        context.Response.StatusCode = 422;
                    }
                    else if (ActualVerificationToken != ExpectedVerificationToken)
                    {
                        LogFile.WriteLine("Header >verification-token< not as expected.");
                        context.Response.Write("Header >verification-token< not as expected.\n");
                        context.Response.StatusCode = 422;
                    }
                    else
                    {
                        // Get the entire body of the POST
                        string FullContentString;
                        using (StreamReader ContentStream = new StreamReader(context.Request.InputStream))
                        {
                            FullContentString = ContentStream.ReadToEnd();
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

                        // test for appropriate message content type
                        if (context.Request.ContentType.IndexOf(@"application/json") == -1)
                        {
                            string Msg = "Received Unexpected ContentType header value: " + context.Request.ContentType;
                            context.Response.AddHeader("Error", Msg);
                            context.Response.Write(Msg);
                            LogFile.WriteLine     (Msg);
                            LogFile.Close();
                            context.Response.StatusCode = 200;
                            return;
                        }

                        // Extract needed metadata
                        JObject DocJson = JObject.Parse(FullContentString);
                        // or   DocJson = JsonConvert.DeserializeObject<JObject>(ContentString);
                        RedoxMeta Meta = new RedoxMeta(DocJson.Property("Meta"));
                        long Transmission = Meta.TransmissionNumber;

#region Database Persistence
                        // Instantiate interface to database
                        string CxStr = Environment.MachineName == "IN-PUCKETTT" ?
                            ConfigurationManager.ConnectionStrings["RedoxCacheContextConnectionStringPort5433"].ConnectionString : 
                            ConfigurationManager.ConnectionStrings["RedoxCacheContextConnectionStringPort5432"].ConnectionString ;
                        RedoxCacheInterface Db = new RedoxCacheInterface(CxStr);

                        // Store this message to database
                        long NewRecord = Db.InsertSchedulingRecord(Transmission, FullContentString);
#endregion

                        LogFile.Close();

                        context.Response.StatusCode = 200;
                    }

                    break;

                case "GET":    // GET for api test message from Redox
                    string ActualChallenge = context.Request.QueryString["challenge"];

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
                        context.Response.Write(ActualChallenge);
                        context.Response.StatusCode = 200;
                    }

                    break;

                default:
                    context.Response.Write("Unsupported HTTP method " + context.Response.StatusCode.ToString() + " was received by the handler.");
                    context.Response.StatusCode = 405;
                    return;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}