//---------------------------------------------------------------------
// <copyright file="ScriptXmlHttpRequest.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      Provides an HTTP-specific implementation of the WebRequest class.
// </summary>
//
// @owner  markash
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Http
{
    #region Namespaces.

    using System;
    using System.Diagnostics;
    using System.Windows.Browser;
    using Microsoft.OData.Client;

    #endregion Namespaces.

    /// <summary>Proxy for the browser XmlHttpRequest object.</summary>
    internal sealed class ScriptXmlHttpRequest
    {
        #region Private fields.

        /// <summary>JavaScript object proxy.</summary>
        private ScriptObject request;

        #endregion Private fields.

        #region Constructors.

        /// <summary>Initializes a new <see cref="ScriptXmlHttpRequest"/> instance.</summary>
        public ScriptXmlHttpRequest()
        {
            this.request = CreateNativeRequest();
            Debug.Assert(this.request != null, "this.request != null");
        }

        #endregion Constructors.

        #region Properties.

        /// <summary>Whether the request has completed.</summary>
        internal bool IsCompleted
        {
            get
            {
                return
                    this.request == null ||
                    (Convert.ToInt32((double)this.request.GetProperty("readyState")) == 4);
            }
        }

        #endregion Properties.

        #region Methods.

        /// <summary>Releases all resources associated with this instance.</summary>
        public void Dispose()
        {
            var currentRequest = this.request;
            if (currentRequest != null)
            {
                try
                {
                    ScriptObjectUtility.SetReadyStateChange(currentRequest, null);
                }
                finally
                {
                    this.request = null;
                }

                // This would be a great place to GC.Collect, because there is
                // no way the garbage collector can know that there is a large
                // amount of memory available on the native side (we'd
                // AddMemoryPressure on the full .NET Framework).
                //
                // However, if we make a call to GC.Collect, we can end up
                // making the application spend a lot of time collecting
                // garbage when a number of requests is made one after the
                // other; the application can be more efficient than this and
                // make the appropriate call after the batch.
            }
        }

        /// <summary>Returns the text for all response headers.</summary>
        /// <returns>Response headers.</returns>
        public string GetResponseHeaders()
        {
            string responseHeaders = (string)this.request.Invoke("getAllResponseHeaders", new object[0]);
            if (string.IsNullOrEmpty(responseHeaders))
            {
                return string.Empty;
            }

            int indexOfColon = responseHeaders.IndexOf(':');
            int indexOfSeparator = responseHeaders.IndexOf('\n');
            if (indexOfColon > indexOfSeparator)
            {
                responseHeaders = responseHeaders.Substring(indexOfSeparator + 1);
            }

            if (responseHeaders.IndexOf("\r\n", StringComparison.Ordinal) == -1)
            {
                responseHeaders = responseHeaders.Replace("\n", "\r\n");
            }

            if (responseHeaders.EndsWith("\r\n\r\n", StringComparison.Ordinal))
            {
                return responseHeaders;
            }

            if (!responseHeaders.EndsWith("\r\n", StringComparison.Ordinal))
            {
                return (responseHeaders + "\r\n\r\n");
            }

            return (responseHeaders + "\r\n");
        }

        /// <summary>Reads the status code from the response.</summary>
        /// <param name="statusCode">After invocation, the status code from the response.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0014", Justification = "Throws every time")]
        public void GetResponseStatus(out int statusCode)
        {
            try
            {
                // get status code from XmlHttpRequest
                statusCode = Convert.ToInt32((double)this.request.GetProperty("status"));
            }
            catch (Exception e)
            {
                if (!CommonUtil.IsCatchableExceptionType(e))
                {
                    throw;
                }

                // "Unable to read response status line"
                string message = Microsoft.OData.Client.Strings.HttpWeb_Internal("ScriptXmlHttpRequest.HttpWebRequest");
                throw new WebException(message, e);
            }
        }

        /// <summary>Invokes the 'open' method on a request object.</summary>
        /// <param name="uri">Target URI.</param>
        /// <param name="method">Method name.</param>
        /// <param name="readyStateChangeCallback">Callback for the readyStateChanged method.</param>
        public void Open(string uri, string method, Action readyStateChangeCallback)
        {
            Util.CheckArgumentNull(uri, "uri");
            Util.CheckArgumentNull(method, "method");
            Util.CheckArgumentNull(readyStateChangeCallback, "readyStateChangeCallback");
            
            ScriptObject callback = ScriptObjectUtility.ToScriptFunction(readyStateChangeCallback);
            ScriptObjectUtility.CallOpen(this.request, method, uri);
            ScriptObjectUtility.SetReadyStateChange(this.request, callback);
        }

        /// <summary>Reads the response as text.</summary>
        /// <returns>Response text.</returns>
        public string ReadResponseAsString()
        {
            Debug.Assert(this.request != null, "this.request != null");
            return (string)this.request.GetProperty("responseText");
        }

        /// <summary>Sends the specified content.</summary>
        /// <param name="content">Content to send (possibly null).</param>
        public void Send(string content)
        {
            Debug.Assert(this.request != null, "this.request != null");
            this.request.Invoke("send", content ?? string.Empty);
        }

        /// <summary>Sets a request header value.</summary>
        /// <param name="header">Name of header to set.</param>
        /// <param name="value">Value to set.</param>
        public void SetRequestHeader(string header, string value)
        {
            Debug.Assert(this.request != null, "this.request != null");
            this.request.Invoke("setRequestHeader", header, value);
        }

        /// <summary>Aborts the request.</summary>
        internal void Abort()
        {
            var requestValue = this.request;
            if (requestValue != null)
            {
                requestValue.Invoke("abort", new object[0]);
            }
        }

        /// <summary>Attempts to create the instance of the specified object.</summary>
        /// <param name="typeName">Name of type to create.</param>
        /// <param name="arg">Argument for the constructor, possibly null.</param>
        /// <param name="request">After invocation, a ScriptObject if successfully created one; false otherwise.</param>
        /// <returns>true if the request was created, false otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0014", Justification = "Throws every time")]
        private static bool CreateInstance(string typeName, object arg, out ScriptObject request)
        {
            request = null;
            try
            {
                object[] args = (arg == null) ? null : new object[] { arg };
                request = HtmlPage.Window.CreateInstance(typeName, args);
            }
            catch (Exception exception)
            {
                if (!CommonUtil.IsCatchableExceptionType(exception))
                {
                    throw;
                }

                // Silently ignore other exceptions.
            }

            return (null != request);
        }

        /// <summary>Creates a ScriptObject that encapsulates a native XmlHttpRequest object.</summary>
        /// <returns>A native XmlHttpRequest object.</returns>
        /// <exception cref="WebException">If a native XmlHttpRequest is not available</exception>
        private static ScriptObject CreateNativeRequest()
        {
            // try various approaches to get an XmlHttpRequest object as this varies across browsers and browser versions
            ScriptObject result;
            if (!CreateInstance("XMLHttpRequest", null, out result) &&
                !CreateInstance("ActiveXObject", "MSXML2.XMLHTTP.6.0", out result) &&
                !CreateInstance("ActiveXObject", "MSXML2.XMLHTTP.3.0", out result) &&
                !CreateInstance("ActiveXObject", "MSXML2.XMLHTTP.2.0", out result) &&
                !CreateInstance("ActiveXObject", "MSXML2.XMLHTTP", out result) &&
                !CreateInstance("ActiveXObject", "XMLHttpRequest", out result) &&
                !CreateInstance("ActiveXObject", "Microsoft.XMLHTTP", out result))
            {
                throw WebException.CreateInternal("ScriptXmlHttpRequest.CreateNativeRequest");
            }

            Debug.Assert(result != null, "result != null -- otherwise CreateInstance should not have returned true");
            return result;
        }

        #endregion Methods.
    }
}
