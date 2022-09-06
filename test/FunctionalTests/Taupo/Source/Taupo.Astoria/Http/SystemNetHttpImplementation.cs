//---------------------------------------------------------------------
// <copyright file="SystemNetHttpImplementation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Http
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// An HTTP stack implementation using System.Net's HttpWebRequest and HttpWebResponse
    /// </summary>
    [ImplementationName(typeof(IHttpImplementation), "System.Net",
        HelpText = "Uses System.Net.HttpWebRequest and HttpWebResponse")]
    public class SystemNetHttpImplementation : IHttpImplementation
    {
        /// <summary>
        /// Initializes a new instance of the SystemNetHttpImplementation class.
        /// </summary>
        public SystemNetHttpImplementation()
        {
            this.Logger = Logger.Null;
        }

        /// <summary>
        /// Gets or sets the authenication provider used to authenticate against the Data Service
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IAuthenticationProvider AuthenticationProvider { get; set; }

        /// <summary>
        /// Gets or sets the logger to use
        /// </summary>
        [InjectDependency]
        public Logger Logger { get; set; }

        /// <summary>
        /// Sends the given request and returns a representation of the response
        /// </summary>
        /// <param name="request">The request to send, throws if null</param>
        /// <returns>The response to the given request</returns>
        public HttpResponseData GetResponse(IHttpRequest request)
        {
            ExceptionUtilities.CheckArgumentNotNull(request, "request");

            HttpWebResponse response;
            try
            {
                // this call needs to be inside the try block, as it can throw
                HttpWebRequest underlyingRequest = this.SendRequest(request);
                response = (HttpWebResponse)underlyingRequest.GetResponse();
            }
            catch (WebException e)
            {
                // the underlying stack will often throw due to server errors, we should quietly return the underlying response
                response = (HttpWebResponse)e.Response;
                ThrowWrappedExceptionIfNull(response, "Underlying response was null", e);
            }

            // wrap the underlying response and return it
            return this.BuildResponse(response);
        }

        internal static void ThrowWrappedExceptionIfNull(object value, string message, Exception inner)
        {
            if (value == null)
            {
                throw new TaupoInfrastructureException(message, inner);
            }
        }

        /// <summary>
        /// Constructs an HttpWebRequest for the given information and sends it (by writing to the stream).
        /// If the server responds with certain status-codes, this method will throw due to the underlying stack's implementation.
        /// </summary>
        /// <param name="request">The request to send, should not be null, and should have an absolute Uri</param>
        /// <returns>An HttpWebRequest</returns>
        private HttpWebRequest SendRequest(IHttpRequest request)
        {
            // some sanity checks
            ExceptionUtilities.Assert(request != null, "request == null");

            Uri requestUri = request.GetRequestUri();
            ExceptionUtilities.Assert(requestUri != null, "requestUri == null");
            ExceptionUtilities.Assert(requestUri.IsAbsoluteUri, "!requestUri.IsAbsoluteUri");

            // create the request
            HttpWebRequest underlyingRequest = (HttpWebRequest)HttpWebRequest.Create(requestUri);

            if (this.AuthenticationProvider.UseDefaultCredentials)
            {
                underlyingRequest.UseDefaultCredentials = true;
            }
            else if (this.AuthenticationProvider.GetAuthenticationCredentials() != null)
            {
                underlyingRequest.Credentials = this.AuthenticationProvider.GetAuthenticationCredentials();
            }

            IDictionary<string, string> authenticationHeaders = this.AuthenticationProvider.GetAuthenticationHeaders();
            if (authenticationHeaders != null)
            {
                foreach (var header in authenticationHeaders)
                {
                    underlyingRequest.Headers[header.Key] = header.Value;
                }
            }

            // set the HTTP method
            underlyingRequest.Method = request.Verb.ToHttpMethod();

            // clear default values
            underlyingRequest.Accept = null;
            underlyingRequest.ContentType = null;

            // set the request's headers. Accept and Content-Type are special cases due to HttpWebRequest's API.
            foreach (KeyValuePair<string, string> header in request.Headers)
            {
                switch (header.Key)
                {
                    case HttpHeaders.Accept:
                        // use the special API
                        underlyingRequest.Accept = header.Value;
                        break;

                    case HttpHeaders.ContentType:
                        // use the special API, and randomize it
                        underlyingRequest.ContentType = header.Value;
                        break;

                    default:
                        underlyingRequest.Headers.Add(header.Key, header.Value);
                        break;
                }
            }

            // log request before sending
            this.Logger.WriteLine(LogLevel.Verbose, "HTTP: {0} {1} {2}", underlyingRequest.Method, underlyingRequest.RequestUri, underlyingRequest.Accept);

            // if the request is empty, explicitly set the content length to 0
            var requestBody = request.GetRequestBody();
            if (requestBody == null || requestBody.Length == 0)
            {
                underlyingRequest.ContentLength = 0;
            }
            else
            {
                // set the content length, and write all the bytes to the stream
                // note that this will cause the request to be sent to the server
                underlyingRequest.ContentLength = requestBody.Length;
                using (Stream os = underlyingRequest.GetRequestStream())
                {
                    os.Write(requestBody, 0, requestBody.Length);
                }
            }

            // return the request
            return underlyingRequest;
        }

        /// <summary>
        /// Builds an HttpResponse out of the given underlying response
        /// </summary>
        /// <param name="underlyingResponse">The underlying HttpWebResponse to copy values from</param>
        /// <returns>An HttpResponse based on the given underlying response</returns>
        private HttpResponseData BuildResponse(HttpWebResponse underlyingResponse)
        {
            ExceptionUtilities.CheckArgumentNotNull(underlyingResponse, "underlyingResponse");

            using (Stream underlyingStream = underlyingResponse.GetResponseStream())
            {
                return HttpResponseBuilder.BuildResponse(
                    underlyingResponse.StatusCode,
                    underlyingResponse.Headers.AllKeys.ToDictionary(k => k, k => underlyingResponse.Headers[k]),
                    underlyingStream);                    
            }
        }
    }
}
