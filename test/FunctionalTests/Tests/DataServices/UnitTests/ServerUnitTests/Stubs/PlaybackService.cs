//---------------------------------------------------------------------
// <copyright file="PlaybackService.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Xml.Linq;
    using AstoriaUnitTests.Tests;
    using test = System.Data.Test.Astoria;

    public class PlaybackServiceDefinition : TestServiceDefinition
    {
        public string OverridingPlayback { get; set; }
        public Action<Stream> InspectRequestPayload { get; set; }
        public Func<InMemoryWebRequest, InMemoryWebRequest> ProcessRequestOverride { get; set; }

        public string LastPlayback { get { return PlaybackService.LastPlayback; } }

        public PlaybackServiceDefinition()
        {
            this.ServiceType = typeof(PlaybackService);
        }

        public static new PlaybackServiceDefinition Current { get { return (PlaybackServiceDefinition)TestServiceDefinition.Current; } }

        protected override void InitializeService(TestWebRequest request)
        {
            base.InitializeService(request);

            // We reset all the static variables to nulls since we use the service definition if it's available
            // This allows users to modify the settings on the servide definition while the service already runs and those changes to have effect
            OpenWebDataServiceDefinition.ApplySetting(request, PlaybackService.OverridingPlayback, null);
            OpenWebDataServiceDefinition.ApplySetting(request, PlaybackService.InspectRequestPayload, null);
            OpenWebDataServiceDefinition.ApplySetting(request, PlaybackService.ProcessRequestOverride, null);
        }

        /// <summary>Returns a new ProcessRequestOverride func which will take the request, parse it as batch, 
        /// unwrap the first part from it and run the passed in func on it, then wrapping the response in batch again.</summary>
        /// <param name="partProcessRequest">The func to run on the single part in the batch</param>
        public static Func<InMemoryWebRequest, InMemoryWebRequest> UnwrapSingleBatchPart(Func<InMemoryWebRequest, InMemoryWebRequest> partProcessRequest)
        {
            return (request) =>
                {
                    var batch = BatchWebRequest.FromRequest(request);
                    if (batch.Parts.Count > 1 || (batch.Parts.Count == 0 && (batch.Changesets.Count != 1 || batch.Changesets[0].Parts.Count != 1)))
                        throw new Exception("The batch has more than one part in it.");

                    IList<InMemoryWebRequest> parts = null;
                    if (batch.Parts.Count > 0)
                    {
                        parts = batch.Parts;
                    }
                    else
                    {
                        parts = batch.Changesets[0].Parts;
                    }
                    var part = parts[0];
                    string contentId;
                    part.RequestHeaders.TryGetValue("Content-ID", out contentId);
                    parts.Clear();
                    part = partProcessRequest(part);
                    part.ResponseHeaders["Content-ID"] = contentId;
                    parts.Add(part);

                    var response = new InMemoryWebRequest();
                    batch.WriteResponse(response);
                    return response;
                };
        }

        public class PassThroughInterceptor
        {
            public PassThroughInterceptor(string playbackServiceUriInput, TestWebRequest underlyingServiceInput)
            {
                this.PlaybackServiceBaseUri = playbackServiceUriInput;
                this.UnderlyingService = underlyingServiceInput;
            }

            private string playbackServiceBaseUri;
            public string PlaybackServiceBaseUri 
            {
                get { return this.playbackServiceBaseUri; }
                set { this.playbackServiceBaseUri = NormalizeBaseUri(value); }
            }

            private string underlyingServiceBaseUri;
            private TestWebRequest underlyingService;
            public TestWebRequest UnderlyingService
            {
                get { return this.underlyingService; }
                set {
                    this.underlyingService = value;
                    this.underlyingServiceBaseUri = NormalizeBaseUri(value.BaseUri); 
                }
            }

            public InMemoryWebRequest ProcessRequestOverride(InMemoryWebRequest request)
            {
                if (IsTextPayloadType(request.RequestContentType))
                {
                    string payload = 
                        ReplaceUriOccurences(
                            this.playbackServiceBaseUri,
                            this.underlyingServiceBaseUri,
                            new StreamReader(request.GetRequestStream()).ReadToEnd());
                    // Remove all Content-Length headers (if it's a batch since we just changed the length of the requests by replacing strings)
                    StringBuilder sb = new StringBuilder();
                    TextReader reader = new StringReader(payload);
                    string line;
                    while((line = reader.ReadLine()) != null)
                    {
                        if (!line.StartsWith("Content-Length")) sb.AppendLine(line);
                    }
                    request.SetRequestStreamAsText(sb.ToString());
                }

                // Copy the request to the server request
                request.WriteRequest(this.underlyingService);
                // Send the request
                try
                {
                    this.underlyingService.SendRequest();

                    // Copy the response to our in-memory representation
                    var response = InMemoryWebRequest.FromResponse(this.underlyingService);
                    if (IsTextPayloadType(response.ResponseContentType))
                    {
                        response.SetResponseStreamAsText(
                            ReplaceUriOccurences(
                                this.underlyingServiceBaseUri,
                                this.playbackServiceBaseUri,
                                response.GetResponseStreamAsText()));
                    }
                    var headersToReplace = new string[] { "Location", "OData-EntityId" };
                    foreach (var headerName in headersToReplace)
                    {
                        string value;
                        if (response.ResponseHeaders.TryGetValue(headerName, out value))
                        {
                            response.ResponseHeaders[headerName] = 
                                ReplaceUriOccurences(
                                    this.underlyingServiceBaseUri, 
                                    this.playbackServiceBaseUri, 
                                    value);
                        }
                    }
                    return response;
                }
                catch (Exception exception)
                {
                    // Translate everything into a 500, it's easier and we don't need correct error reporting on the client anyway (for versioning tests)
                    var response = new InMemoryWebRequest();
                    response.SetResponseStatusCode(500);
                    response.ResponseHeaders["Content-Type"] = UnitTestsUtil.MimeTextPlain;
                    response.SetResponseStreamAsText(exception.ToString());
                    return response;
                }
            }

            private static string NormalizeBaseUri(string baseUri)
            {
                return baseUri.EndsWith("/") ? baseUri : baseUri + "/";
            }

            private static bool IsTextPayloadType(string contentType)
            {
                if (contentType == null) return false;
                contentType = contentType.Split(';')[0];
                string[] textPayloadTypes = new string[] {
                        UnitTestsUtil.AtomFormat,
                        UnitTestsUtil.JsonLightMimeType,
                        UnitTestsUtil.MimeApplicationXml,
                        UnitTestsUtil.MimeTextXml,
                        UnitTestsUtil.MimeTextPlain,
                        UnitTestsUtil.MimeMultipartMixed
                    };
                foreach (var textPayloadType in textPayloadTypes)
                {
                    if (string.Equals(contentType, textPayloadType, StringComparison.OrdinalIgnoreCase)) return true;
                }
                return false;
            }

            private static string ReplaceUriOccurences(string source, string dest, string content)
            {
                return content.Replace(source, dest);
            }
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceContract]
    public class PlaybackService
    {
        /// <summary>Last playback that was received.</summary>
        private static string lastPlayback;

        /// <summary>Response being built.</summary>
        private StringBuilder response;

        /// <summary>Last playback that was received.</summary>
        public static string LastPlayback { get { return lastPlayback; } }

        /// <summary>A payload to play back, overriding whatever is sent from the client.</summary>
        private static test.Restorable<string> overridingPlayback = new test.Restorable<string>();
        public static test.Restorable<string> OverridingPlayback { get { return overridingPlayback; } }

        /// <summary>An action allowing to verify the request received from the client</summary>
        private static test.Restorable<Action<Stream>> inspectRequestPayload = new test.Restorable<Action<Stream>>();
        public static test.Restorable<Action<Stream>> InspectRequestPayload { get { return inspectRequestPayload; } }

        /// <summary>Func if specified takes the incomming request and returns outgoing response. This override all the other options and takes over the entire processing of the request.</summary>
        private static test.Restorable<Func<InMemoryWebRequest, InMemoryWebRequest>> processRequestOverride = new test.Restorable<Func<InMemoryWebRequest,InMemoryWebRequest>>();
        public static test.Restorable<Func<InMemoryWebRequest, InMemoryWebRequest>> ProcessRequestOverride { get { return processRequestOverride; } }

        // Helper properties to unite the old static settings with the new service definition approach
        private static PlaybackServiceDefinition ServiceDefinition { get { return PlaybackServiceDefinition.Current; } }
        private static string InternalOverridingPlayback { get { if (ServiceDefinition != null) { return ServiceDefinition.OverridingPlayback; } else { return OverridingPlayback.Value; } } }
        private static Action<Stream> InternalInspectRequestPayload { get { if (ServiceDefinition != null) { return ServiceDefinition.InspectRequestPayload; } else { return InspectRequestPayload.Value; } } }
        private static Func<InMemoryWebRequest, InMemoryWebRequest> InternalProcessRequestOverride { get { if (ServiceDefinition != null) { return ServiceDefinition.ProcessRequestOverride; } else { return ProcessRequestOverride.Value; } } }

        [OperationContract]
        [WebInvoke(UriTemplate = "*", Method = "*")]
        public Stream ProcessRequestForMessage(Stream messageBody)
        {
            this.response = new StringBuilder();
            WebOperationContext c = WebOperationContext.Current;

            if (InternalProcessRequestOverride != null)
            {
                InMemoryWebRequest request = new InMemoryWebRequest();
                InMemoryWebRequest response = null;

                try
                {
                    request.HttpMethod = c.IncomingRequest.Method;
                    request.RequestUriString = c.IncomingRequest.UriTemplateMatch.RequestUri.AbsoluteUri.Substring(c.IncomingRequest.UriTemplateMatch.BaseUri.AbsoluteUri.Length);
                    foreach (string headerKey in c.IncomingRequest.Headers.AllKeys)
                    {
                        request.RequestHeaders[headerKey] = c.IncomingRequest.Headers[headerKey];
                    }
                    InMemoryWebRequest.ApplyHeadersToProperties(request);
                    request.SetRequestStream(messageBody);

                    response = InternalProcessRequestOverride(request);
                }
                catch (Exception e)
                {
                    response = new InMemoryWebRequest();
                    response.SetResponseStatusCode(500);
                    response.SetResponseStreamAsText(CreateErrorPayload(e));
                }

                c.OutgoingResponse.StatusCode = (HttpStatusCode)response.ResponseStatusCode;
                foreach (var header in response.ResponseHeaders)
                {
                    c.OutgoingResponse.Headers[header.Key] = header.Value;
                }

                if (response.GetResponseStream() != null)
                {
                    MemoryStream body = new MemoryStream();
                    System.Data.Test.Astoria.TestUtil.CopyStream(response.GetResponseStream(), body);
                    body.Position = 0;
                    return body;
                }
                else
                {
                    return null;
                }
            }

            if (InternalInspectRequestPayload != null)
            {
                MemoryStream ms = new MemoryStream();
                test.TestUtil.CopyStream(messageBody, ms);
                messageBody = ms;
                messageBody.Position = 0;
                InternalInspectRequestPayload(messageBody);
                messageBody.Position = 0;
            }

            Append("<error xmlns='http://docs.oasis-open.org/odata/ns/metadata'>");
            Append("<message>");
            Append(c.IncomingRequest.Method + " " + c.IncomingRequest.UriTemplateMatch.RequestUri.OriginalString);
            Append("\r\n");
            Append(c.IncomingRequest.Headers.ToString());
            Append("\r\n");
            AppendXmlStream(messageBody);
            Append("</message>");
            Append("</error>");

            lastPlayback = this.response.ToString();

            string result = lastPlayback;
            if (InternalOverridingPlayback != null)
            {
                result = ProcessOverridingPlayback(c);

                // For batch request, i need to send the exact content id otherwise the client chokes
                int startIndex = lastPlayback.IndexOf("Content-ID: ");
                if (startIndex != -1)
                {
                    // advance the index to the start of the content id
                    startIndex += "Content-ID: ".Length;
                    int endIndex = lastPlayback.IndexOf(Environment.NewLine, startIndex);
                    int contentID = Int32.Parse(lastPlayback.Substring(startIndex, endIndex - startIndex));

                    int resultStartIndex = result.IndexOf("Content-ID: ") + "Content-ID: ".Length;
                    int resultEndIndex = result.IndexOf(Environment.NewLine, resultStartIndex);
                    result = result.Substring(0, resultStartIndex) + contentID + result.Substring(resultEndIndex);
                }
            }
            else
            {
                if (c.IncomingRequest.ContentType != null)
                {
                    c.OutgoingResponse.ContentType = c.IncomingRequest.ContentType;
                }
                else
                {
                    // Took a breaking change, since the content type now must be correct.
                    // In V1/V2, astoria client used to parse error payloads even when response content type value was application/atom+xml
                    // After integration, it must be application/xml.
                    c.OutgoingResponse.ContentType = AstoriaUnitTests.Data.SerializationFormatData.Atom.MimeTypes[2];
                }

                if (c.IncomingRequest.Method == "POST")
                {
                    c.OutgoingResponse.Location = "http://www.microsoft.com/";
                }
            }

            return (result == null) ? null : new MemoryStream(Encoding.ASCII.GetBytes(result));
        }

        private void Append(string text)
        {
            this.response.Append(text);
        }

        private void AppendXmlStream(Stream inputStream)
        {
            string text = new StreamReader(inputStream).ReadToEnd();
            text = text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
            Append(text);
        }

        private string CreateErrorPayload(Exception exception)
        {
            XNamespace m = AstoriaUnitTests.Tests.UnitTestsUtil.MetadataNamespace;
            XElement error = new XElement(m + "error",
                new XElement(m + "message",
                    exception.ToString()));
            return error.ToString();
        }

        private static string ProcessOverridingPlayback(WebOperationContext context)
        {
            var result = new StringBuilder();
            bool inHeaders = true;
            bool statusCodeSet = false;
            using (StringReader reader = new StringReader(InternalOverridingPlayback))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (inHeaders)
                    {
                        if (line == "")
                        {
                            inHeaders = false;
                        }
                        else if (!statusCodeSet)
                        {
                            int startIndex = "HTTP/1.1 ".Length;
                            int endIndex = line.IndexOf(" ", startIndex);
                            context.OutgoingResponse.StatusCode = (System.Net.HttpStatusCode)Int32.Parse(line.Substring(startIndex, endIndex - startIndex));
                            statusCodeSet = true;
                        }
                        else
                        {
                            int colonIndex = line.IndexOf(':');
                            string headerName = line.Substring(0, colonIndex);

                            // Ignore the Content-ID header, since the content-ID header for POST cases causes an exception
                            // to be thrown in the client, since it automatically also adds an content-ID
                            if (headerName != "Content-ID")
                            {
                                context.OutgoingResponse.Headers[headerName] = line.Substring(colonIndex + 1);
                            }
                        }
                    }
                    else
                    {
                        result.AppendLine(line);
                    }
                }
            }

            return (result.Length == 0) ? null : result.ToString();
        }

        public static string ConvertToBatchQueryResponsePayload(string changesetPayload)
        {
            string batchPayload = null;
            return ConvertToBatchResponsePayload(new string[] { changesetPayload }, true, out batchPayload);
        }

        /// <summary>
        /// Combines the specified payloads into one batch response payload.
        /// </summary>
        /// <param name="changesetPayloads">Individual changeset payloads to be used for the batch.</param>
        /// <param name="queryPayload">Whether or not this payload is for a query response or other type of requests.</param>
        /// <param name="batchPayload">The resulting payload without the batch headers.</param>
        /// <returns>The entire batch payload, including the batch headers. Typically used with PlaybackService.OverriddingPlayback.Value.</returns>
        public static string ConvertToBatchResponsePayload(string[] changesetPayloads, bool queryPayload, out string batchPayload)
        {
            batchPayload = null;

            StringBuilder batchPayloadBuilder = new StringBuilder();
            batchPayloadBuilder.AppendLine("--batchresponse_e9b231d9-72ab-46ea-9613-c7e8f5ece46b");

            if (!queryPayload)
            {
                batchPayloadBuilder.AppendLine("Content-Type: multipart/mixed; boundary=changesetresponse_eaab4754-7965-43f0-a7a9-a5556d12787c");
                batchPayloadBuilder.AppendLine();
            }

            foreach (string changesetPayload in changesetPayloads)
            {
                if (!queryPayload)
                {
                    // Add changeset begin boundary
                    batchPayloadBuilder.AppendLine("--changesetresponse_eaab4754-7965-43f0-a7a9-a5556d12787c");
                }

                batchPayloadBuilder.AppendLine("Content-Type: application/http");
                batchPayloadBuilder.AppendLine("Content-Transfer-Encoding: binary");
                batchPayloadBuilder.AppendLine();
                batchPayloadBuilder.AppendLine(changesetPayload);
            }

            if (!queryPayload)
            {
                // Add changeset end boundary
                batchPayloadBuilder.AppendLine("--changesetresponse_eaab4754-7965-43f0-a7a9-a5556d12787c--");
            }

            batchPayloadBuilder.AppendLine("--batchresponse_e9b231d9-72ab-46ea-9613-c7e8f5ece46b--");

            batchPayload = batchPayloadBuilder.ToString();

            return
                "HTTP/1.1 202 OK" + Environment.NewLine +
                "Content-Type: multipart/mixed; boundary=batchresponse_e9b231d9-72ab-46ea-9613-c7e8f5ece46b" + Environment.NewLine +
                Environment.NewLine +
                batchPayload;
        }

        public static string ConvertToPlaybackServicePayload(IEnumerable<KeyValuePair<string, string>> headers, string payload, HttpStatusCode? statusCode = null)
        {
            string headersAndPayload = 
               (statusCode == null ? "HTTP/1.1 200 OK" : String.Format("HTTP/1.1 {0} {1}", (int)statusCode.Value, statusCode.Value.ToString())) + Environment.NewLine +
               "Content-Type: application/atom+xml" + Environment.NewLine;

            if (headers != null)
            {
                foreach (var h in headers)
                {
                    headersAndPayload += h.Key + ": " + h.Value + Environment.NewLine;
                }
            }

            headersAndPayload += Environment.NewLine;

            if (payload != null)
            {
                headersAndPayload += payload;
            }

            return headersAndPayload;
        }
    }
}
