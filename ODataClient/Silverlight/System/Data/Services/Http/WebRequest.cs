//---------------------------------------------------------------------
// <copyright file="WebRequest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
// <summary>
//      WebRequest type.
// </summary>
//
// @owner  markash
//---------------------------------------------------------------------

namespace System.Data.Services.Http
{
    #region Namespaces.

    using System;
    using System.Data.Services.Client;
    using System.Diagnostics;
    using System.IO;

    #endregion Namespaces.

    /// <summary>Makes a request to a Uniform Resource Identifier (URI).</summary>
    internal abstract class WebRequest
    {
        /// <summary>
        /// When overridden in a descendant class, gets or sets the content type of the request data being sent.
        /// </summary>
        public abstract string ContentType
        {
            get;
            set;
        }

        /// <summary>
        /// When overridden in a descendant class, gets or sets the collection of header name/value pairs associated with the request.
        /// </summary>
        public abstract System.Data.Services.Http.WebHeaderCollection Headers
        {
            get;
        }

        /// <summary>
        /// When overridden in a descendant class, gets or sets the protocol method to use in this request.
        /// </summary>
        public abstract string Method
        {
            get;
            set;
        }

        /// <summary>
        /// When overridden in a descendant class, gets the URI of the Internet resource associated with the request.
        /// </summary>
        public abstract Uri RequestUri
        {
            get;
        }

        /// <summary>
        /// Initializes a new WebRequest instance for the specified URI scheme.
        /// </summary>
        /// <param name="requestUri">A Uri containing the URI of the requested resource. </param>
        /// <param name="httpStack">The HttpStack setting. This can enforce usage of the specified HTTP stack.</param>
        /// <returns>A WebRequest descendant for the specified URI scheme.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller will dispose the returned value")]
        public static System.Data.Services.Http.WebRequest Create(Uri requestUri, HttpStack httpStack)
        {
            Debug.Assert(requestUri != null, "requestUri != null");
            if ((requestUri.Scheme != PlatformHelper.UriSchemeHttp) && (requestUri.Scheme != PlatformHelper.UriSchemeHttps))
            {
                // "SR.GetString(SR.net_unknown_prefix)"
                throw new NotSupportedException();
            }

#if !WIN8 && !WINDOWS_PHONE && !PORTABLELIB
            if (httpStack == HttpStack.Auto)
            {
                if (UriRequiresClientHttpWebRequest(requestUri))
                {
                    httpStack = HttpStack.ClientHttp;
                }
                else
                {
                    httpStack = HttpStack.XmlHttp;
                }
            }

            if (httpStack == HttpStack.ClientHttp)
            {
                return new ClientHttpWebRequest(requestUri);
            }
            else
            {
                Debug.Assert(httpStack == HttpStack.XmlHttp, "Only ClientHttp and XmlHttp are supported for now.");
                return new XHRHttpWebRequest(requestUri);
            }
#else
#if WIN8
           Debug.Assert(httpStack == HttpStack.Auto, "HttpStack should always be Auto for WIN8");
#endif
            return new ClientHttpWebRequest(requestUri);
#endif
        }

        /// <summary>Aborts the Request.</summary>
        public abstract void Abort();

        /// <summary>
        /// When overridden in a descendant class, provides an asynchronous version of the 
        /// GetRequestStream method.
        /// </summary>
        /// <param name="callback">The AsyncCallback delegate.</param>
        /// <param name="state">The state object for this request.</param>
        /// <returns>An IAsyncResult that references the asynchronous request.</returns>
        public abstract IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state);

        /// <summary>
        /// When overridden in a descendant class, begins an asynchronous request for an Internet resource.
        /// </summary>
        /// <param name="callback">The AsyncCallback delegate.</param>
        /// <param name="state">The state object for this request.</param>
        /// <returns>An IAsyncResult that references the asynchronous request for a response.</returns>
        public abstract IAsyncResult BeginGetResponse(AsyncCallback callback, object state);

        /// <summary>
        /// When overridden in a descendant class, returns a Stream for writing data to the Internet resource.
        /// </summary>
        /// <param name="asyncResult">The pending request for a stream.</param>
        /// <returns>A Stream to use to write request data.</returns>
        public abstract Stream EndGetRequestStream(IAsyncResult asyncResult);

        /// <summary>
        /// Ends an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="asyncResult">The pending request for a response.</param>
        /// <returns>A WebResponse that contains the response from the Internet resource.</returns>
        public abstract System.Data.Services.Http.WebResponse EndGetResponse(IAsyncResult asyncResult);

#if !WIN8 && !WINDOWS_PHONE && !PORTABLELIB
        /// <summary>
        /// Determines if a request to the specified URI needs to use the Client HTTP stack
        /// or if it should use the XHR HTTP stack.
        /// </summary>
        /// <param name="uri">The uri for the request</param>
        /// <returns>true if the request needs to use the Client HTTP stack, otherwise false</returns>
        private static bool UriRequiresClientHttpWebRequest(Uri uri)
        {
            // If we can use XHR, we will - to maintain backward compat.
            // So if we can use XHR technically (DOM Bridge is allowed and we can actually create the XHR object)
            //   we need to check if running the request through XHR would work, thus we need to figure out if it
            //   will be considered X-Domain. If it's not X-Domain we will use XHR
            // In all other cases (XHR not available because we're Out-Of-Browser, DOM Bridge is not allowed,
            //   running on a non-UI thread, request would be X-Domain and so on) we will use the client stack always.
            if (!XHRHttpWebRequest.IsAvailable())
            {
                return true;
            }

            // Ideally we would use the algorithm from XHR to determine if the request is X-Domain
            // because we want to use Client for requests which XHR would mark as X-Domain (and thus fail).
            // The best approximation is to compare the request URI to the HTML page URI.
            Uri sameDomainUri = System.Windows.Browser.HtmlPage.Document.DocumentUri;

            // The request is same-domain only if it has same scheme, same domain and same port
            if (sameDomainUri.Scheme != uri.Scheme || sameDomainUri.Port != uri.Port ||
                !string.Equals(sameDomainUri.DnsSafeHost, uri.DnsSafeHost, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
#endif
    }
}
