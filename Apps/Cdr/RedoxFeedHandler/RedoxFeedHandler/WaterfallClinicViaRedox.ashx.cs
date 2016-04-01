using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace RedoxFeedHandler
{
    /// <summary>
    /// Summary description for WaterfallClinicViaRedox
    /// </summary>
    public class WaterfallClinicViaRedox : IHttpHandler
    {

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
                        string LogFolder = @"C:\RedoxFeedHandler\RedoxFeedTest";
                        string OutFileName = Path.Combine(LogFolder, "Log.txt");

                        StreamReader ContentStream = new StreamReader(context.Request.InputStream);
                        string ContentString = ContentStream.ReadToEnd();
                        ContentStream.Close();

#region Json parsing/handling test
                        JObject DocJson = JObject.Parse(ContentString);
                        // or   DocJson = JsonConvert.DeserializeObject<JObject>(ContentString);
                        RedoxMeta Meta = new RedoxMeta(DocJson.Property("Meta"));

                        JProperty Ret1, Ret2, Ret3, Ret4, Ret5, Ret6, Ret7, Ret8;
                        Ret1 = Meta.DataModel;
                        Ret2 = Meta.EventType;
                        Ret3 = Meta.EventDateTime;
                        Ret4 = Meta.Test;
                        Ret5 = Meta.Source;
                        Ret6 = Meta.Message;
                        Ret7 = Meta.Transmission;
                        Ret8 = Meta.FacilityCode;

                        QueryInterface I = new QueryInterface();
                        if (I.IsAuthenticated)
                        {
                            JObject PatientToQuery = new JObject();
                            I.QueryForClinicalSummary(PatientToQuery);
                        }
#endregion

                        var x = Meta.DataModel.Value.Value<string>();
                        StreamWriter OutFile = new StreamWriter(OutFileName);

                        // log all HTTP headers
                        foreach (string H in context.Request.Headers)
                        {
                            OutFile.WriteLine("Header: " + H + ": " + context.Request.Headers[H]);
                        }

                        if (context.Request.ContentType.IndexOf(@"application/json") == -1)
                        {
                            context.Response.Write  ("Received Unexpected ContentType header value: " + context.Request.ContentType);
                            OutFile.WriteLine       ("Received Unexpected ContentType header value: " + context.Request.ContentType);
                            OutFile.WriteLine("Content: " + ContentString);
                            OutFile.Close();
                            context.Response.StatusCode = 200;
                            return;
                        }

                        JToken Payload = JObject.Parse(ContentString);

                        OutFile.Write("Content:\r\n" + JsonConvert.SerializeObject(Payload, Formatting.Indented) + "\n");
                        OutFile.Close();

                        string TransmissionNumber = Payload["Meta"]["Transmission"]["ID"].ToString();

                        OutFileName = Path.Combine(LogFolder, TransmissionNumber + ".txt");
                        OutFile = new StreamWriter(OutFileName);

                        OutFile.Write("Payload is: " + TransmissionNumber + "\n");
                        OutFile.Write("Patient LName is " + Payload["Patient"]["Demographics"]["LastName"].ToString() + "\n");

                        OutFile.Close();

                        context.Response.Write(@"Stored message content to file " + OutFileName);

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