//---------------------------------------------------------------------
// <copyright file="BlobsRequest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.Test.ModuleCore;
using System.Net;
using System.Linq;

namespace System.Data.Test.Astoria
{
    //---------------------------------------------------------------------
    // Specializes AstoriaRequest for MLEs and MRs.
    //---------------------------------------------------------------------
    public partial class BlobsRequest : AstoriaRequest
    {
        protected static string LastURI;

        //---------------------------------------------------------------------
        // Constructs AstoriaRequest and sets.
        //---------------------------------------------------------------------
        private BlobsRequest(Workspace w, SerializationFormatKind format, RequestVerb verb, string uri, HttpStatusCode expectedStatusCode) : base(w)
        {
            // Common settings for MLEs and MRs.
            base.IsBlobRequest = true;
            base.Verb = verb;
            base.ExpectedStatusCode = expectedStatusCode;

            // Construct request URI.
            if (uri.Contains("(*)"))
            {
                // Replace (*) with random key.
                string relativeURI = uri.Remove(0, w.ServiceUri.Length + 1);
                ResourceContainer container = w.ServiceContainer.ResourceContainers[relativeURI.Substring(0, relativeURI.IndexOf("(*)"))];
                KeyExpression key = null;
                try { key = w.GetRandomExistingKey(container, container.BaseType); } catch(Exception e) { AstoriaTestLog.Skip("Unable to get random key"); }
                base.Query = ContainmentUtil.BuildCanonicalQuery(key);
                base.URI += relativeURI.Substring(relativeURI.IndexOf("(*)") + 3);
            }
            else
            {
                // Deterministic URI.
                base.URI = uri;
            }

            LastURI = base.URI;
        }

        //---------------------------------------------------------------------
        // Constructs request to MLE.
        //---------------------------------------------------------------------
        public static BlobsRequest MLE(Workspace w, SerializationFormatKind format, RequestVerb verb, BlobsPayload payload, HttpStatusCode expectedStatusCode, params string[] URI)
        {
            string uri = URI.Length == 0 ? LastURI : URI[0];
            BlobsRequest rq = new BlobsRequest(w, format, verb, uri, expectedStatusCode);

            // Specific request properties.
            rq.Format = format;
            rq.ContentType = SerializationFormatKinds.ContentTypeFromKind(format);
            rq.Accept = rq.ContentType;
            rq.Payload = (payload != null ? payload.ToString() : null);

            return rq;
        }

        //---------------------------------------------------------------------
        // Constructs request to MR.
        //---------------------------------------------------------------------
        public static BlobsRequest MRR(Workspace w, SerializationFormatKind format, RequestVerb verb, string payload, HttpStatusCode expectedStatusCode, params string[] URI)
        {
            string uri = URI.Length == 0 ? LastURI : URI[0];
            BlobsRequest rq = new BlobsRequest(w, format, verb, uri, expectedStatusCode);

            // Specific request properties.
            rq.Format = SerializationFormatKind.PlainText;
            rq.ContentType = "audio/mp3";
            rq.Accept = (verb == RequestVerb.Get ? "*/*" : SerializationFormatKinds.ContentTypeFromKind(format));
            rq.Payload = payload;

            // Append $value, if needed.
            if (uri.StartsWith("$") || uri.EndsWith(")"))
            {
                rq.URI += "/$value";

                // Expect dummy ETags from stream provider.
                byte[] hash = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(System.Text.Encoding.ASCII.GetBytes(rq.URI));
                if ((hash[0] & 3) > 1)
                    rq.ETagHeaderExpected = true;
            }

            return rq;
        }

        //---------------------------------------------------------------------
        // Constructs request with $ref. Assumes parent(*)/child URI format.
        //---------------------------------------------------------------------
        public static BlobsRequest LNK(Workspace w, SerializationFormatKind format, RequestVerb verb, string payload, HttpStatusCode expectedStatusCode, params string[] URI)
        {
            string uri = (URI.Length == 0 ? LastURI : URI[0]);
            BlobsRequest rq = new BlobsRequest(w, format, verb, uri, expectedStatusCode);

            // Specific request properties.
            rq.Format = format;
            rq.ContentType = SerializationFormatKinds.ContentTypeFromKind(format == SerializationFormatKind.JSON ? SerializationFormatKind.JSON : SerializationFormatKind.PlainXml);
            rq.Accept = rq.ContentType;
            
            // Adjust link in payload, if needed.
            if (!string.IsNullOrEmpty(payload))
            {
                rq.Payload = string.Format(format == SerializationFormatKind.JSON ?
                    @"{{""odata.id"": ""{0}"" }}" :
                    @"<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' id=""{0}"" />",
                    payload);
            }

            return rq;
        }

        //---------------------------------------------------------------------
        public AstoriaResponse SendAndVerify(object expectedPayload, params string[] headers)
        {
            // Set auxiliary header values, if any (like ETag).
            for (int i = 0; i < headers.Length; i += 2)
                base.Headers[headers[i]] = headers[i + 1];

            // Determine lowest possible protocol versions.
            string requestVersion = "1.0";
            string responseVersion = "1.0";
            if (base.ExpectedStatusCode == HttpStatusCode.OK)
            {
                if (base.URI.Contains("$select="))
                {
                    requestVersion = "2.0";
                    responseVersion = "1.0";
                }
                if (base.URI.Contains("$inlinecount="))
                {
                    requestVersion = "2.0";
                    responseVersion = "2.0";
                }
                if (base.URI.Contains("/$count"))
                {
                    requestVersion = "2.0";
                    responseVersion = "2.0";
                }
            }

            // Set request version headers semi-randomly.
            switch (base.URI.GetHashCode() & 3)
            {
                case 0: base.DataServiceVersion = requestVersion; break;
                case 1: base.DataServiceVersion = null; break;
            }
            switch ((base.URI.GetHashCode() >> 2) & 3)
            {
                case 0: base.MaxDataServiceVersion = requestVersion; break;
                case 1: base.MaxDataServiceVersion = Versioning.Server.DataServiceVersion; break;
            }

            // Send request to server.
            AstoriaResponse response = GetResponse();

            // Update last seen MLE or MR ETags.
            if (response.ETagHeaderFound)
            {
                if (base.URI.Contains("$value"))
                    ETagMRR = response.ETagHeader;
                else
                    ETagMLE = response.ETagHeader;
            }

            if ((AstoriaTestProperties.DataLayerProviderKinds.Contains(DataLayerProviderKind.NonClr)) && (AstoriaTestProperties.UseOpenTypes))
            {
                //
                //  This section of code is handling the DSV values returned from server changes
                //
                bool not_3_0 = response.DataServiceVersion.StartsWith("1.0");
                not_3_0 = response.DataServiceVersion.StartsWith("2.0") | not_3_0;
                AstoriaTestLog.Compare(not_3_0, "DataServiceVersion response header was returned with " + response.DataServiceVersion);
                AstoriaTestLog.TraceInfo("Returned DSV was " + response.DataServiceVersion);
            }
            else
            {
                // Verify response version header.
                AstoriaTestLog.Compare(response.DataServiceVersion.StartsWith(responseVersion),
                    "DataServiceVersion response header must be " + responseVersion + " but was " + response.DataServiceVersion);
            }

            // Verify content type: xml for Atom, json for JSON.
            if (!base.Batched && !string.IsNullOrEmpty(response.Payload))
            {
                if (base.Format == SerializationFormatKind.Atom && !response.ContentType.Contains("xml") ||
                    (base.Format == SerializationFormatKind.JSON && !response.ContentType.Contains("json")))
                {
                    AstoriaTestLog.WriteLine(string.Format("Wrong Content-Type {0} in response to {1} request: ", response.ContentType, base.Format));
                    AstoriaTestLog.WriteLine("Payload:");
                    AstoriaTestLog.FailAndThrow(response.Payload ?? "{null}");
                }
            }

            // Verify MLE (BlobsPayload) or MR response payload.
            if (expectedPayload is BlobsPayload)
            {
                // Compare MLE payloads.
                BlobsPayload actualPayload = (expectedPayload as BlobsPayload);
                if (actualPayload.ToString() != expectedPayload.ToString())
                    AstoriaTestLog.FailAndThrow(
                        "MLE received:" + TestLog.NewLine + actualPayload + TestLog.NewLine +
                        "MLE expected:" + TestLog.NewLine + expectedPayload);

                // Temporarily morph MLE into normal entity and call Verify().
                string originalPayload = response.Payload;
                response.Payload = actualPayload.AdjustedForVerify();
                response.Verify();
                response.Payload = originalPayload;
            }
            else
            {
                // Compare MR payloads.
                string actualPayload = response.Payload;
                if (expectedPayload != null && actualPayload != expectedPayload as string)
                AstoriaTestLog.FailAndThrow(
                    "MR received:" + TestLog.NewLine + actualPayload + TestLog.NewLine +
                    "MR expected:" + TestLog.NewLine + expectedPayload);

                response.Verify();
            }
                
            // Remove auxiliary headers, if any.
            for (int i = 0; i < headers.Length; i += 2)
                base.Headers.Remove(headers[i]);

            return response;
        }
    }
}
