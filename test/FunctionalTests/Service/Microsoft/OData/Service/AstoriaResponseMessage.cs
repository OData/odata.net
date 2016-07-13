//---------------------------------------------------------------------
// <copyright file="AstoriaResponseMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using Microsoft.OData;

    /// <summary>
    /// IODataResponseMessage interface implementation.
    /// </summary>
    internal sealed class AstoriaResponseMessage : IODataResponseMessage
    {
        #region Private Fields

        /// <summary>Reference to the IDataServiceHost object we are wrapping</summary>
        private readonly IDataServiceHost host;

        /// <summary>
        /// Host as IDataServiceHost2, or null if it does not implement it (just ISDH, then).
        /// </summary>
        private readonly IDataServiceHost2 host2;

#if DEBUG
        /// <summary>
        /// To keep track of whether the headers have already been written. This happens
        /// when the stream is set. After this is set to true, we need to make sure that 
        /// there is no call to update the headers, since its too late to write the headers.
        /// </summary>
        private bool headersAlreadyFlushed;
#endif

        /// <summary>
        /// Stream to which response payload needs to be written.
        /// Note that this is NOT always the IDSH Response Stream. For WCF we create an XmlWriterStream 
        /// when it is time to serialize.
        /// </summary>
        private Stream responseStream;

        /// <summary>
        /// Reference to the ResponseHeaders on the host, if it is IDSH2. Otherwise null.
        /// </summary>
        private WebHeaderCollection responseHeadersWebCollection;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of AstoriaResponseMessage.
        /// </summary>
        /// <param name="host">Host instance associated with this request.</param>
        internal AstoriaResponseMessage(IDataServiceHost host)
        {
            Debug.Assert(host != null, "host != null");
            this.host2 = host as IDataServiceHost2;
            this.host = host;
        }

        #endregion

        #region IODataResponseMessage Properties

        /// <summary>Gets all the response headers.</summary>
        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                Debug.Assert(false, "This method should never get called");
                throw new DataServiceException(500, Strings.DataServiceException_GeneralError);
            }
        }

        /// <summary>
        /// The ResponseStatusCode of the underlying host.
        /// </summary>
        public int StatusCode
        {
            get
            {
                return this.host.ResponseStatusCode;
            }

            set 
            { 
#if DEBUG
                Debug.Assert(!this.headersAlreadyFlushed, "!this.headersAlreadyFlushed");
#endif
                this.host.ResponseStatusCode = value;

                if (this.host.ResponseStatusCode == 304)
                {
                    // Remove all headers when responding with a 304
                    this.ClearSystemHeaderFor304();
                }
            }
        }

        #endregion

        #region Temporary Internal Properties

        /// <summary>Response headers from the underlying host. Must be IDSH2 to succeed.</summary>
        internal WebHeaderCollection ResponseHeadersWebCollection
        {
            get
            {
                if (this.responseHeadersWebCollection == null)
                {
                    if (this.host2 == null)
                    {
                        throw new InvalidOperationException(Strings.DataServiceHost_FeatureRequiresIDataServiceHost2);
                    }

                    this.responseHeadersWebCollection = this.host2.ResponseHeaders;
                    if (this.responseHeadersWebCollection == null)
                    {
                        throw new InvalidOperationException(Strings.DataServiceHost_ResponseHeadersCannotBeNull);
                    }
                }

                return this.responseHeadersWebCollection;    
            }
        }

        #endregion

        #region IODataResponseMessage Methods

        /// <summary>
        /// Returns the value of the given response header.
        /// </summary>
        /// <param name="headerName">Name of the response header.</param>
        /// <returns>Returns the value of the given response header.</returns>
        public string GetHeader(string headerName)
        {
            switch (headerName)
            {
                case XmlConstants.HttpContentType:
                    return this.host.ResponseContentType;
                case XmlConstants.HttpResponseETag:
                    return this.host.ResponseETag;
                case XmlConstants.HttpODataVersion:
                    return this.host.ResponseVersion;
                case XmlConstants.HttpResponseLocation:
                    return this.host.ResponseLocation;
                case XmlConstants.HttpResponseCacheControl:
                    return this.host.ResponseCacheControl;
                case XmlConstants.HttpPreferenceApplied:
                    // this.ResponseHeadersWebCollection will throw if host2 is null. We just want to return null as if the header is missing.
                    return this.host2 == null ? null : this.ResponseHeadersWebCollection[headerName];
                default:
                    Debug.Fail("AstoriaResponseMessage.GetHeader called with an unknown header name: " + headerName);
                    throw new DataServiceException(500, Strings.DataServiceException_GeneralError);
            }
        }

        /// <summary>
        /// Sets the value of the given response header. Passes the value through to the host immeadiately.
        /// After we call SetStreamthis should never be called again.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="headerValue">Value for the header to be set.</param>
        public void SetHeader(string headerName, string headerValue)
        {
            // In astoria, IDataServiceHost2.ResponseHeaders is never called to set the value of these headers.
            // It will be breaking change to call it now. Hence trying to call the right property depending on the 
            // header name to keep it backward-compatible.
            switch (headerName)
            {
                case XmlConstants.HttpContentType:
                    this.host.ResponseContentType = headerValue;
                    break;
                case XmlConstants.HttpODataVersion:
                    this.host.ResponseVersion = headerValue;
                    break;
                case XmlConstants.HttpResponseCacheControl:
                    this.host.ResponseCacheControl = headerValue;
                    break;
                case XmlConstants.HttpResponseLocation:
                    this.host.ResponseLocation = headerValue;
                    break;
                case XmlConstants.HttpResponseETag:
                    this.host.ResponseETag = headerValue;
                    break;

                // For headers not on the first ISDH
                case XmlConstants.HttpPreferenceApplied:
                case XmlConstants.HttpODataEntityId:
                case XmlConstants.HttpContentID:
                    this.ResponseHeadersWebCollection.Add(headerName, headerValue);
                    break;
                default:
                    // It would be OK to just do hot2.ResponseHeaders.Add like above.
                    // This is just here to help us keep track of what headers we could possibly set.
                    Debug.Fail("We are trying to set unknown header " + headerName);
                    throw new DataServiceException(500, Strings.DataServiceException_GeneralError);
            }
        }

        /// <summary>
        /// Returns the stream to which response payload needs to be written.
        /// </summary>
        /// <returns>Returns the stream to which response payload needs to be written.</returns>
        public Stream GetStream()
        {
#if DEBUG
            Debug.Assert(this.responseStream != null, "this.responseStream != null");
            Debug.Assert(this.headersAlreadyFlushed, "this.headersAlreadyFlushed");
#endif
            return this.responseStream;
        }

#endregion

        /// <summary>
        /// Set the response stream.
        /// </summary>
        /// <remarks>This gets called in the writer Action that DataService.HandleRequest() returns.  
        /// In the custom host scenario, the value will be the stream we get from the underlying host.  
        /// In the WCF scenario, the stream passed into the action (and thus this) will be the XmlWriterStream we create
        /// in DelegateBodyWriter. </remarks>
        /// <param name="stream">Stream to which the response needs to be written.</param>
        internal void SetStream(Stream stream)
        {
            Debug.Assert(stream != null, "responseStream != null");
            Debug.Assert(this.responseStream == null, "The response stream has already been set once. It should not need to be set again.");

            this.responseStream = stream;
#if DEBUG
            this.headersAlreadyFlushed = true;
#endif
        }

        /// <summary>
        /// Clears the system header for304.
        /// </summary>
        /// <remarks>Only clearing specific system headers to avoid removing custom headers</remarks>
        internal void ClearSystemHeaderFor304()
        {
            this.host.ResponseVersion = null;
            this.host.ResponseContentType = null;
            this.host.ResponseCacheControl = null;
        
            if (this.host2 != null)
            {
                this.ResponseHeadersWebCollection.Remove(XmlConstants.HttpPreferenceApplied);
            }
        }

        /// <summary>Sets the response status code and the default caching and versioning headers.</summary>
        /// <param name="description">The request description for the current request.</param>
        /// <param name="statusCode">The status code for the response.</param>
        internal void SetResponseHeaders(RequestDescription description, int statusCode)
        {
            // Set the caching policy appropriately - for the time being, we disable caching.
            this.SetHeader(XmlConstants.HttpResponseCacheControl, XmlConstants.HttpCacheControlNoCache);

            // If a preference was applied, add corresponding response header.
            ClientPreference preference = description.Preference;
            this.PreferenceAppliedHeader().ReturnContent = preference.ShouldIncludeResponseBody ? true : (preference.ShouldNotIncludeResponseBody ? (bool?)false : null);

            // Only set the annotation filter to the Preference-Applied if we are writing a response body
            // since instance annotations only appear in the response body.
            if (description.ShouldWriteResponseBody && !string.IsNullOrEmpty(preference.AnnotationFilter))
            {
                this.PreferenceAppliedHeader().AnnotationFilter = preference.AnnotationFilter;
            }

            this.SetHeader(XmlConstants.HttpODataVersion, description.ResponseVersion.ToString() + ";");
            this.StatusCode = statusCode;
        }
    }
}
