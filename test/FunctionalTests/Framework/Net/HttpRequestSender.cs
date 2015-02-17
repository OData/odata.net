//---------------------------------------------------------------------
// <copyright file="HttpRequestSender.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Data.Test.Astoria.Util;

namespace System.Data.Test.Astoria
{
    public class HttpRequestSender : AstoriaRequestSender
    {
        protected override sealed bool SendRequest_Internal(AstoriaRequest request, out AstoriaResponse response)
        {
            HttpWebRequest underlyingRequest = (HttpWebRequest)HttpWebRequest.Create(request.URI);
            WebResponse underlyingResponse;

            #region set up request

            // workaround: Protocol Violation in HttpWebRequest when receiving an immediate error response from server before sending request body
            // ideally, we would only flip this to false if we knew the request would have an error,
            // but we can't reliably tell at this point
            if (AstoriaTestProperties.Host == Host.IDSH || AstoriaTestProperties.Host == Host.IDSH2)
            {
                if (Environment.OSVersion.Version.Major < 6 || Environment.OSVersion.Version.Minor < 1)
                    underlyingRequest.ServicePoint.Expect100Continue = false;
            }
            
            underlyingRequest.UseDefaultCredentials = 
                AstoriaTestProperties.HostAuthenicationMethod.Equals("Windows", StringComparison.InvariantCultureIgnoreCase);
            underlyingRequest.Method = request.Verb.ToHttpMethod();

            underlyingRequest.Accept = request.Accept;
            underlyingRequest.ContentType = request.ContentType;

            foreach (KeyValuePair<string, string> header in request.Headers)
            {
                switch (header.Key)
                {
                    case "Accept":
                    case "Content-Type":
                        break;

                    default:
                        underlyingRequest.Headers.Add(header.Key, header.Value);
                        break;
                }
            }

            underlyingRequest.ContentLength = 0;
        
            byte[] bytes = request.PayloadBytes;
            
            if (bytes != null)
            {
                underlyingRequest.ContentLength = bytes.Length;
                try
                {
                    using (Stream os = underlyingRequest.GetRequestStream())
                    {
                        os.Write(bytes, 0, bytes.Length);
                    }
                }
                catch (WebException ex)
                {
                    HandleWebException(ex, out underlyingResponse);
                    response = null;
                    return false;
                }
            }
            else if (request.HttpStreamWriter != null)
            {
                // Call external stream writer.
                try
                {
                    // Set ContentLength header.
                    underlyingRequest.ContentLength = request.HttpStreamWriter(null);

                    // Stream payload.
                    using (Stream requestStream = underlyingRequest.GetRequestStream())
                    {
                        request.HttpStreamWriter(requestStream);
                    }
                }
                catch (WebException ex)
                {
                    HandleWebException(ex, out underlyingResponse);
                    response = null;
                    return false;
                }
            }

            #endregion

            try
            {
                underlyingResponse = underlyingRequest.GetResponse();
            }
            catch (WebException webException)
            {
                if (HandleWebException(webException, out underlyingResponse))
                {
                    if (underlyingResponse == null)
                    {
                        response = null;
                        return false;
                    }
                }
                else
                    throw webException;
            }

            // This should not be possible
            if (underlyingResponse == null)
                AstoriaTestLog.FailAndThrow("Somehow got a null underlying response");

            HttpWebResponse httpResponse = underlyingResponse as HttpWebResponse;

            response = new AstoriaResponse(request);

            #region populate response
            response.ContentType = underlyingResponse.ContentType;

            // hook everything up
            if (httpResponse != null)
                response.ActualStatusCode = httpResponse.StatusCode;
            else
                response.ActualStatusCode = HttpStatusCode.Ambiguous;

            // have to be careful and only mark ETag as found if it was actually sent
            // regardless of whether the value was null
            response.ETagHeaderFound = false;
            foreach (string header in underlyingResponse.Headers.AllKeys)
            {
                if (header == "ETag")
                    response.ETagHeaderFound = true;

                if (underlyingResponse is HttpWebResponse)
                    response.Headers[header] = (underlyingResponse as HttpWebResponse).GetResponseHeader(header);
                else
                    response.Headers[header] = string.Join(", ", underlyingResponse.Headers.GetValues(header));
            }

            // For Streamed/StreamedResponse, Transfer-Encoding is chunked and ContentLength could be -1
            if ((underlyingResponse.Headers[HttpResponseHeader.TransferEncoding] == "chunked" || underlyingResponse.ContentLength > 0))
            {
                Encoding encoding = Encoding.UTF8;

                using (Stream responseStream = underlyingResponse.GetResponseStream())
                {
                    if (request.HttpStreamReader != null)
                    {
                        // Call external stream reader.
                        response.Payload = request.HttpStreamReader(responseStream);
                    }
                    else
                    {
                        if (underlyingResponse.ContentLength > 0 && response.ContentType == SerializationFormatKinds.MimeApplicationOctetStream)
                        {
                            using (System.IO.BinaryReader reader = new System.IO.BinaryReader(responseStream))
                                response.Bytes = reader.ReadBytes((int)underlyingResponse.ContentLength);
                        }
                        else
                        {
                            using (System.IO.TextReader reader = new System.IO.StreamReader(responseStream, encoding))
                                response.Payload = reader.ReadToEnd();
                        }
                    }
                }
            }
            else
            {
                // only make an assignment to response.Payload if we're sure there was no content
                // if we set this prematurely, it can mess up the internal state of the response
                response.Payload = string.Empty;
            }
            #endregion

            return true;
        }

        private static bool HandleWebException(WebException webException, out WebResponse response)
        {
            // return whether or not it has been handled
            // find out if the exception was caused by a socket exception

            Exception innerException = webException.InnerException;
            System.Net.Sockets.SocketException sockException = null;

            while (innerException != null)
            {
                if (innerException is System.Net.Sockets.SocketException)
                {
                    sockException = innerException as System.Net.Sockets.SocketException;
                    break;
                }
                else
                    innerException = innerException.InnerException;
            }

            if (sockException != null)
            {
                // Ignore exceptions caused by Fiddler.
                if (sockException.SocketErrorCode == System.Net.Sockets.SocketError.ConnectionReset)
                {
                    AstoriaTestLog.WriteLine("Connection was reset. Retrying...");
                    response = null;
                    return true;
                }
                AstoriaTestLog.WriteLine("A socket exception occurred, waiting 30 seconds before retrying");
                AstoriaTestLog.WriteLine("Message: " + sockException.Message);
                AstoriaTestLog.WriteLine("Error Code: " + sockException.SocketErrorCode);
                AstoriaTestLog.WriteLineIgnore("Time before sleep: " + DateTime.Now);
                Threading.Thread.Sleep(new TimeSpan(0, 0, 30));
                AstoriaTestLog.WriteLineIgnore("Time after sleep: " + DateTime.Now);
                response = null;
                return true;
            }

            if (webException.Status == WebExceptionStatus.Timeout)
            {
                AstoriaTestLog.WriteLine("The request timeout out, waiting 30 seconds before retrying");
                AstoriaTestLog.WriteLine("Message: " + webException.Message);
                AstoriaTestLog.WriteLineIgnore("Time before sleep: " + DateTime.Now);
                Threading.Thread.Sleep(new TimeSpan(0, 0, 30));
                AstoriaTestLog.WriteLineIgnore("Time after sleep: " + DateTime.Now);
                response = null;
                return true;
            }

            if (webException.Response != null)
            {
                response = webException.Response;
                return true;
            }

            response = null;
            return false;
        }
    }
}
