//---------------------------------------------------------------------
// <copyright file="XmlHttpRequestSender.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Data.Test.Astoria.Util;

namespace System.Data.Test.Astoria
{
    public class XmlHttpRequestSender : AstoriaRequestSender
    {
        protected override bool SendRequest_Internal(AstoriaRequest request, out AstoriaResponse response)
        {
            string uri = request.URI;

            // workaround bug that same url is always cached.
            if (request.Verb == RequestVerb.Get)
            {
                if (uri.Contains('?'))
                {
                    uri = String.Format("{0}&bug={1}", uri, DateTime.Now.Ticks.ToString());
                }
                else
                {
                    uri = String.Format("{0}?bug={1}", uri, DateTime.Now.Ticks.ToString());
                }
            }

            Exception exc = null;
            int statusCode = -1;

            XmlHttpClassComWrapper nativeWebRequest;
            try
            {
                nativeWebRequest = XmlHttpClassComWrapper.CreateXmlHttpClassWrapper();
            }
            catch
            {
                AstoriaTestLog.WriteLineIgnore("Could not create a new XmlHttp com wrapper");
                AstoriaTestLog.WriteLineIgnore("Time before sleep: " + DateTime.Now);
                Threading.Thread.Sleep(new TimeSpan(0, 0, 30));
                AstoriaTestLog.WriteLineIgnore("Time after sleep: " + DateTime.Now);
                response = null;
                return false;
            }

            using (nativeWebRequest)
            {
                string userName = null;
                string password = null;
                if (AstoriaTestProperties.HostAuthenicationMethod.ToLower().Equals("windows"))
                {
                    userName = string.Empty;
                    password = string.Empty;
                }

                nativeWebRequest.open(request.Verb.ToHttpMethod(), uri, false, userName, password);


                // this does Accept and Content-Type for us
                foreach (KeyValuePair<string, string> header in request.Headers)
                    nativeWebRequest.setRequestHeader(header.Key, header.Value);

                byte[] toSend = request.PayloadBytes;

                try
                {
                    nativeWebRequest.send(toSend);
                    statusCode = nativeWebRequest.status;
                }
                catch (System.Reflection.TargetInvocationException targetInvokationException)
                {
                    exc = targetInvokationException;

                    if (targetInvokationException.InnerException != null && targetInvokationException.InnerException is System.Runtime.InteropServices.COMException)
                    {
                        //HACK DUE TO XmlHttpIssue
                        // info at    http://www.enhanceie.com/ie/bugs.asp
                        if (nativeWebRequest.status == 1223)
                        {
                            statusCode = (int)HttpStatusCode.NoContent;
                        }
                        else
                        {
                            throw targetInvokationException;
                        }
                    }
                }

                response = new AstoriaResponse(request);

                // Assign status code.
                if (Enum.IsDefined(typeof(HttpStatusCode), statusCode))
                    response.ActualStatusCode = (HttpStatusCode)statusCode;
                else
                    response.ActualStatusCode = HttpStatusCode.Ambiguous;

                // Assign Content-Type and other headers.
                string headers = nativeWebRequest.AllResponseHeaders;
                response.ContentType = null;
                if (headers != null)
                {
                    // Parse headers.
                    foreach (string headerLine in headers.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        int valueIndex = headerLine.IndexOf(':');
                        if (valueIndex > 0 && valueIndex < headerLine.Length - 1)
                            response.Headers[headerLine.Substring(0, valueIndex)] = headerLine.Substring(valueIndex + 2);
                    }
                }
                else
                {
                    // No headers.
                    if (statusCode == (int)HttpStatusCode.NoContent)
                        AstoriaTestLog.WriteLine("Warning: XmlHttp does not return headers when status code is No Content");
                    else
                        AstoriaTestLog.FailAndThrow("Failed to read headers from XmlHttp response");
                }
                response.ETagHeaderFound = response.Headers.ContainsKey("ETag");

                // Assign payload.
                if (response.ContentType == SerializationFormatKinds.MimeApplicationOctetStream)
                    response.Bytes = nativeWebRequest.responseBody;
                else
                    response.Payload = nativeWebRequest.responseText;

                GC.Collect();
                return true;
            }
        }
    }
}
