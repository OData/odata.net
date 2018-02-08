//---------------------------------------------------------------------
// <copyright file="AstoriaRequestMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Text;
    using Microsoft.OData;

    /// <summary>
    /// IODataRequestMessage interface implementation.
    /// </summary>
    internal class AstoriaRequestMessage : IODataRequestMessage, IODataPayloadUriConverter
    {
        #region Private Fields

        /// <summary>Reference to the IDataServiceHost object we are wrapping</summary>
        private readonly IDataServiceHost host;

        /// <summary>Delegation object to figure out the acceptable content types based on $format and accept header.</summary>
        private readonly IAcceptableContentTypeSelector acceptableContentTypeSelector;

        /// <summary>Gets a comma-separated list of client-supported MIME Accept types.</summary>
        private string requestAccept;

        /// <summary>Gets the string with the specification for the character set encoding that the client requested, possibly null.</summary>
        private string requestAcceptCharSet;

        /// <summary>Gets the value of the If-Match header from the request made</summary>
        private string requestIfMatch;

        /// <summary>Gets the value of the If-None-Match header from the request made</summary>
        private string requestIfNoneMatch;

#if DEBUG
        /// <summary>
        /// To keep track of whether the stream was already retrieved or not. After this is set to true, 
        /// we need to make sure that there is no call to retrieve the headers. Not that we would not have the values
        /// but the contract is like that.
        /// </summary>
        private bool streamRetrieved;
#endif

        /// <summary>Gets the value for the OData-Version request header.</summary>
        private Version requestVersion;

        /// <summary>The value of the OData-Version header as a string.</summary>
        private string requestVersionString;

        /// <summary>
        /// Get the enum representing the http method name.
        /// We have this for perf reason since enum comparison is faster than string comparison.
        /// </summary>
        private HttpVerbs httpVerb;

        /// <summary>Gets the HTTP data transfer method (such as GET, POST, or HEAD) used by the client.</summary>
        private string requestHttpMethod;

        /// <summary>Gets the absolute URI to the resource upon which to apply the request.</summary>
        private Uri absoluteRequestUri;

        /// <summary>Gets the absolute URI to the service.</summary>
        private Uri absoluteServiceUri;

        /// <summary>Gets the <see cref="Stream"/> from which the input must be read to the client.</summary>
        private Stream requestStream;

        /// <summary>Request headers</summary>
        private WebHeaderCollection requestHeaders;

        /// <summary>Whether or not the request and service URI have been marked as read-only.</summary>
        private bool requestAndServiceUrisAreReadOnly;

        /// <summary>true if this.InitializeRequestVersionHeaders() has been called; false otherwise.</summary>
        private bool requestVersionHeadersInitialized;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of AstoriaRequestMessage.
        /// </summary>
        /// <param name="host">IDataServiceHost instance to access all the request headers.</param>
        internal AstoriaRequestMessage(IDataServiceHost host)
            : this(host, new AcceptableContentTypeSelector())
        {
        }

        /// <summary>
        /// Creates a new instance of AstoriaRequestMessage. This is meant to be a constructor for unit tests only. 
        /// </summary>
        /// <param name="host">IDataServiceHost instance to access all the request headers.</param>
        /// <param name="selector">Object to select acceptable content types.</param>
        internal AstoriaRequestMessage(IDataServiceHost host, IAcceptableContentTypeSelector selector)
        {
            Debug.Assert(host != null, "host != null");
            this.host = host;
            this.httpVerb = HttpVerbs.None;
            this.acceptableContentTypeSelector = selector;
            this.CacheHeaders();
        }

        #endregion

        #region IODataRequestMessage Properties

        /// <summary>Gets all the request headers.</summary>
        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                Debug.Assert(false, "This method should never get called");
                throw new DataServiceException(500, Strings.DataServiceException_GeneralError);
            }
        }

        /// <summary>The URL of the request.</summary>
        public Uri Url
        {
            get
            {
                Debug.Assert(false, "This method should never get called");
                throw new DataServiceException(500, Strings.DataServiceException_GeneralError);
            }

            set
            {
                Debug.Assert(false, "This method should never get called");
                throw new DataServiceException(500, Strings.DataServiceException_GeneralError);
            }
        }

        /// <summary>The method of the request.</summary>
        public string Method
        {
            get
            {
                Debug.Assert(false, "This method should never get called");
                throw new DataServiceException(500, Strings.DataServiceException_GeneralError);
            }

            set
            {
                Debug.Assert(false, "This method should never get called");
                throw new DataServiceException(500, Strings.DataServiceException_GeneralError);
            }
        }
        #endregion

        #region Other Internal Properties
        /// <summary>
        /// The Content-Type header, this property allows the caller to override the value comming from the host.
        /// </summary>
        internal string ContentType
        {
            get;
            set;
        }

        /// <summary>
        /// Get the enum representing the http method name.
        /// </summary>
        /// <param name="httpMethodName">http method used for the request.</param>
        /// <returns>enum representing the http method name.</returns>
        internal HttpVerbs HttpVerb
        {
            get
            {
                if (this.httpVerb == HttpVerbs.None)
                {
                    string requestMethod = this.RequestHttpMethod;
                    Debug.Assert(!String.IsNullOrEmpty(requestMethod), "!string.IsNullOrEmpty(requestMethod)");
                    switch (requestMethod)
                    {
                        case XmlConstants.HttpMethodGet:
                            this.httpVerb = HttpVerbs.GET;
                            break;

                        case XmlConstants.HttpMethodPost:
                            this.httpVerb = HttpVerbs.POST;
                            break;

                        case XmlConstants.HttpMethodPut:
                            this.httpVerb = HttpVerbs.PUT;
                            break;

                        case XmlConstants.HttpMethodPatch:
                            this.httpVerb = HttpVerbs.PATCH;
                            break;

                        case XmlConstants.HttpMethodDelete:
                            this.httpVerb = HttpVerbs.DELETE;
                            break;

                        default:
                            // 501: Not Implemented (rather than 405 - Method Not Allowed, 
                            // which implies it was understood and rejected).
                            throw DataServiceException.CreateMethodNotImplemented(Strings.DataService_NotImplementedException);
                    }
                }

                return this.httpVerb;
            }
        }

        /// <summary>Gets the absolute resource upon which to apply the request.</summary>
        internal Uri AbsoluteRequestUri
        {
            get
            {
                if (this.absoluteRequestUri == null)
                {
                    this.ValidateAndCacheAbsoluteRequestUri(this.host.AbsoluteRequestUri, false /*validateQueryString*/);
                }

                return this.absoluteRequestUri;
            }

            set
            {
                if (this.requestAndServiceUrisAreReadOnly)
                {
                    throw new InvalidOperationException(Strings.AstoriaRequestMessage_CannotModifyRequestOrServiceUriAfterReadOnly);
                }

                this.ValidateAndCacheAbsoluteRequestUri(value, true /*validateQueryString*/);
            }
        }

        /// <summary>Gets the absolute URI to the service.</summary>
        internal Uri AbsoluteServiceUri
        {
            get
            {
                if (this.absoluteServiceUri == null)
                {
                    this.ValidateAndCacheAbsoluteServiceUri(this.host.AbsoluteServiceUri /*validateQueryString*/);
                }

                return this.absoluteServiceUri;
            }

            set
            {
                if (this.requestAndServiceUrisAreReadOnly)
                {
                    throw new InvalidOperationException(Strings.AstoriaRequestMessage_CannotModifyRequestOrServiceUriAfterReadOnly);
                }

                this.ValidateAndCacheAbsoluteServiceUri(value /*validateQueryString*/);
            }
        }

        /// <summary>Gets the HTTP data transfer method (such as GET, POST, or HEAD) used by the client.</summary>
        internal string RequestHttpMethod
        {
            get
            {
                if (String.IsNullOrEmpty(this.requestHttpMethod))
                {
                    this.requestHttpMethod = this.host.RequestHttpMethod;
                    if (String.IsNullOrEmpty(this.requestHttpMethod))
                    {
                        throw new InvalidOperationException(Strings.DataServiceHost_EmptyHttpMethod);
                    }
                }

                return this.requestHttpMethod;
            }
        }

        /// <summary>Gets the value for the RequestVersion request header.</summary>
        internal Version RequestVersion
        {
            get
            {
                Debug.Assert(this.requestVersion != null, "this.requestVersion != null");
                return this.requestVersion;
            }

            private set 
            { 
                this.requestVersion = value; 
            }
        }

        /// <summary>Gets the value for the OData-MaxVersion request header. If the header is not specified, it returns the max known version.</summary>
        internal Version RequestMaxVersion { get; private set; }

        /// <summary>Gets the <see cref="Stream"/> from which the input must be read to the client.</summary>
        internal Stream RequestStream
        {
            get
            {
                if (this.requestStream == null)
                {
                    this.requestStream = this.host.RequestStream;
                    if (this.requestStream == null)
                    {
                        throw DataServiceException.CreateBadRequestError(Strings.BadRequest_NullRequestStream);
                    }
                }

                return this.requestStream;
            }
        }

        /// <summary>Request headers</summary>
        internal WebHeaderCollection RequestHeaders
        {
            get
            {
                if (this.requestHeaders == null)
                {
                    IDataServiceHost2 host2 = ValidateAndCast<IDataServiceHost2>(this.host);
                    this.requestHeaders = host2.RequestHeaders;
                    if (this.requestHeaders == null)
                    {
                        throw new InvalidOperationException(Strings.DataServiceHost_RequestHeadersCannotBeNull);
                    }
                }

                return this.requestHeaders;
            }
        }

        /// <summary>If the wrapped host is a HttpContextServiceHost, returns the host</summary>
        internal HttpContextServiceHost HttpContextServiceHost
        {
            get { return this.host as HttpContextServiceHost; }
        }

        /// <summary>If the wrapped host is a BatchServiceHost, returns the batch host</summary>
        internal BatchServiceHost BatchServiceHost
        {
            get
            {
                Debug.Assert(this.host is BatchServiceHost, "We shouldn't be calling this outside of the batching code path.");
                return this.host as BatchServiceHost;
            }
        }

        #endregion

        #region IODataRequestMessage Methods

        /// <summary>
        /// Returns the value of the given request header.
        /// </summary>
        /// <param name="headerName">Name of the request header.</param>
        /// <returns>Returns the value of the given request header.</returns>
        public string GetHeader(string headerName)
        {
            Debug.Assert(headerName != null, "headerName != null");

            // In astoria, IDataServiceHost2.RequestHeaders is never called to set the value of these headers.
            // It will be breaking change to call it now. Hence trying to call the right property depending on the 
            // header name to keep it backward-compatible.
            switch (headerName)
            {
                case XmlConstants.HttpContentType:
                    return this.ContentType;
                case XmlConstants.HttpODataVersion:
                    Debug.Assert(this.requestVersionString != null, "this.requestVersionString != null");
                    return this.requestVersionString;
                case XmlConstants.HttpRequestIfMatch:
                    return this.requestIfMatch;
                case XmlConstants.HttpRequestIfNoneMatch:
                    return this.requestIfNoneMatch;
                case XmlConstants.HttpRequestAcceptCharset:
                    return this.requestAcceptCharSet;
                case XmlConstants.HttpPrefer:
                    return this.GetCustomHeaderIfAvailable(headerName);
                case XmlConstants.HttpAccept:
                    return this.requestAccept;
                default:
                    Debug.Assert(false, "Invalid header name encountered: " + headerName);
                    throw new DataServiceException(500, Strings.DataServiceException_GeneralError);
            }
        }

        /// <summary>
        /// Sets the value of the given request header. Since on Astoria Server we are reading requests and
        /// all headers are initialized at construction time, we don't implement this method.
        /// </summary>
        /// <param name="headerName">Name of the request header.</param>
        /// <param name="headerValue">Value for the header to be set.</param>
        public void SetHeader(string headerName, string headerValue)
        {
            Debug.Assert(false, "This method should never get called");
            throw new DataServiceException(500, Strings.DataServiceException_GeneralError);
        }

        /// <summary>
        /// Returns the stream to which request payload needs to be written.
        /// </summary>
        /// <returns>Returns the stream to which request payload needs to be written.</returns>
        public Stream GetStream()
        {
#if DEBUG
            Debug.Assert(!this.streamRetrieved, "The method GetStream can only be called once.");
            this.streamRetrieved = true;
#endif
            Stream stream = this.RequestStream;
            Debug.Assert(stream != null, "requestStream != null");
            return stream;
        }

        #endregion IODataRequestMessage Methods

        #region IODataPayloadUriConverter Members

        /// <summary>
        /// Method to implement a custom URL resolution scheme.
        /// This method returns null if no custom resolution is desired.
        /// If the method returns a non-null URL that value will be used without further validation.
        /// </summary>
        /// <param name="baseUri">The (optional) base URI to use for the resolution.</param>
        /// <param name="payloadUri">The URI read from the payload.</param>
        /// <returns>
        /// A <see cref="Uri"/> instance that reflects the custom resolution of the method arguments
        /// into a URL or null if no custom resolution is desired; in that case the default resolution is used.
        /// </returns>
        public Uri ConvertPayloadUri(Uri baseUri, Uri payloadUri)
        {
            Debug.Assert(payloadUri != null, "The payload URI should never be null.");

            // Just return the payload URI as is, the caller will take care of procesing it correctly.
            return payloadUri;
        }

        #endregion

        #region Internal Methods

        /// <summary>Checks whether the given object instance is of type T.</summary>
        /// <param name="instance">object instance.</param>
        /// <typeparam name="T">type which we need to cast the given instance to.</typeparam>
        /// <returns>Returns strongly typed instance of T, if the given object is an instance of type T.</returns>
        /// <exception cref="InvalidOperationException">If the given object is not of Type T.</exception>
        internal static T ValidateAndCast<T>(object instance) where T : class
        {
            T instanceAsT = instance as T;
            if (instanceAsT == null)
            {
                throw new InvalidOperationException(Strings.DataServiceHost_FeatureRequiresIDataServiceHost2);
            }

            return instanceAsT;
        }

        /// <summary>
        /// Copies over the headers that we cache from the host into this classes fields.
        /// This method should be called after we fire the events that let users alter request headers.
        /// </summary>
        internal void CacheHeaders()
        {
            Debug.Assert(this.requestVersion == null, "this.requestVersion was not null, implying that we called InitializeRequestVersionHeaders before CacheHeaders. We need our version logic to run AFTER the incoming headers get modified by the user.");
            Debug.Assert(this.RequestMaxVersion == null, "this.RequestMaxVersion was not null, implying that we called InitializeRequestVersionHeaders before CacheHeaders. We need our version logic to run AFTER the incoming headers get modified by the user.");
            
            this.requestAccept = this.host.RequestAccept;
            this.requestAcceptCharSet = this.host.RequestAcceptCharSet;
            this.requestIfMatch = this.host.RequestIfMatch;
            this.requestIfNoneMatch = this.host.RequestIfNoneMatch;
            this.ContentType = this.host.RequestContentType;
        }

        /// <summary>
        /// Gets a comma-separated list of client-supported MIME Accept types.
        /// </summary>
        /// <returns>A comma-seperated list of content types the client can accept.</returns>
        internal string GetAcceptableContentTypes()
        {
            Version mdsv = this.RequestMaxVersion ?? new Version(4, 0);
            string dollarFormatValue = this.host.GetQueryStringItem(XmlConstants.HttpQueryStringFormat);
            return this.acceptableContentTypeSelector.GetFormat(dollarFormatValue, this.requestAccept, mdsv);
        }

        /// <summary>Gets the value for the specified item in the request query string.</summary>
        /// <param name="item">Item to return.</param>
        /// <returns>
        /// The value for the specified item in the request query string;
        /// null if <paramref name="item"/> is not found.
        /// </returns>
        internal string GetQueryStringItem(string item)
        {
            return this.host.GetQueryStringItem(item);
        }

        /// <summary>Method to handle a data service exception during processing.</summary>
        /// <param name="args">Exception handling description.</param>
        internal void ProcessException(HandleExceptionArgs args)
        {
            this.host.ProcessException(args);
        }

        /// <summary>
        /// update the request version header, if it is not specified.
        /// </summary>
        /// <param name="maxProtocolVersion">protocol version as specified in the config.</param>
        internal void InitializeRequestVersionHeaders(Version maxProtocolVersion)
        {
            if (this.requestVersionHeadersInitialized)
            {
                return;
            }

            this.requestVersionHeadersInitialized = true;

            Debug.Assert(this.requestVersion == null, "this.requestVersion == null");
            Debug.Assert(this.RequestMaxVersion == null, "this.RequestMaxVersion == null");

            Version maxRequestVersionAllowed = GetMaxRequestVersionAllowed(maxProtocolVersion);

            // read the request version headers from the underlying host
            this.requestVersionString = this.host.RequestVersion;
            this.RequestVersion = ValidateVersionHeader(XmlConstants.HttpODataVersion, this.requestVersionString);
            this.RequestMaxVersion = ValidateVersionHeader(XmlConstants.HttpODataMaxVersion, this.host.RequestMaxVersion);

            // If the request version is not specified.
            if (this.requestVersion == null)
            {
                // In request headers, if the OData-Version header is not specified and OData-MaxVersion is specified,
                // we should ideally set the OData-Version header to whatever the value was specified in OData-MaxVersion
                // header has.
                if (this.RequestMaxVersion != null)
                {
                    // set the OData-Version to minimum of OData-MaxVersion and ProtocolVersion.
                    this.RequestVersion = (this.RequestMaxVersion < maxProtocolVersion) ? this.RequestMaxVersion : maxProtocolVersion;
                }
                else
                {
                    // If both request DSV and request MaxDSV is not specified, then set the request DSV to the MPV
                    this.RequestVersion = maxProtocolVersion;
                }

                this.requestVersionString = this.RequestVersion.ToString(2);
            }
            else
            {                
                if (this.RequestVersion > maxRequestVersionAllowed)
                {
                    throw DataServiceException.CreateBadRequestError(Strings.DataService_RequestVersionMustBeLessThanMPV(this.RequestVersion, maxProtocolVersion));
                }

                // Verify that the request DSV is a known version number.
                if (!VersionUtil.IsKnownRequestVersion(this.RequestVersion))
                {
                    string message = Strings.DataService_InvalidRequestVersion(
                        this.RequestVersion.ToString(2),
                        KnownODataVersionsToString(maxRequestVersionAllowed));
                    throw DataServiceException.CreateBadRequestError(message);
                }
            }

            // Initialize the request OData-MaxVersion if not specified.
            if (this.RequestMaxVersion == null)
            {
                this.RequestMaxVersion = maxProtocolVersion;
            }
            else if (this.RequestMaxVersion < VersionUtil.DataServiceDefaultResponseVersion)
            {
                // We need to make sure the MaxDSV is at least 1.0. This was the V1 behavior.
                // Verified that this was checked both in batch and non-batch cases.
                string message = Strings.DataService_MaxDSVTooLow(
                    this.RequestMaxVersion.ToString(2),
                    VersionUtil.DataServiceDefaultResponseVersion.Major,
                    VersionUtil.DataServiceDefaultResponseVersion.Minor);
                throw DataServiceException.CreateBadRequestError(message);
            }
        }

        /// <summary>Verifies that query parameters are valid.</summary>
        internal void VerifyQueryParameters()
        {
            if (this.host is HttpContextServiceHost)
            {
                ((HttpContextServiceHost)this.host).VerifyQueryParameters();
            }
        }

        /// <summary>
        /// Gets the value of custom header if IDSH2 is implemented, otherwise returns null.
        /// </summary>
        /// <param name="headerName">Name of the custom header.</param>
        /// <returns>The header value if IDSH is implemented, or null otherwise.</returns>
        internal string GetCustomHeaderIfAvailable(string headerName)
        {
            // We can only get custom headers in case of IDataServiceHost2
            // We do this check first because this.RequestHeaders will throw if host is not IDSH2.
            if (!(this.host is IDataServiceHost2))
            {
                // Use default behavior for older hosts.
                return null;
            }

            return this.RequestHeaders[headerName];
        }

        /// <summary>
        /// Makes the AbsoluteServiceUri and AbsoluteRequestUri properties to be read-only. Should only be called immediately after the OnStartProcessingRequest method returns.
        /// </summary>
        internal void MakeRequestAndServiceUrisReadOnly()
        {
            Debug.Assert(!this.requestAndServiceUrisAreReadOnly, "Already set as read-only.");
            this.requestAndServiceUrisAreReadOnly = true;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Validate the given header value and return the Version instance equivalent to the string header value.
        /// If the version header is not specified, then this method returns the max known version.
        /// </summary>
        /// <param name="headerName">name of the request header.</param>
        /// <param name="headerValue">value of the request header.</param>
        /// <returns>Version instance containing the version header information.</returns>
        private static Version ValidateVersionHeader(string headerName, string headerValue)
        {
            KeyValuePair<Version, string> version;

            if (!String.IsNullOrEmpty(headerValue))
            {
                if (!CommonUtil.TryReadVersion(headerValue, out version))
                {
                    throw DataServiceException.CreateBadRequestError(
                        Strings.DataService_VersionCannotBeParsed(headerValue, headerName));
                }

                return version.Key;
            }

            return null;
        }

        /// <summary>
        /// Converts the list of known OData version to a comma's separated string.
        /// <paramref name="maxRequestVersionAllowed"/> restricts the values in the result string.
        /// </summary>
        /// <param name="maxRequestVersionAllowed">Restricts the values in the result string</param>
        /// <returns>Comma separated known version list.</returns>
        private static string KnownODataVersionsToString(Version maxRequestVersionAllowed)
        {
            StringBuilder builder = new StringBuilder();
            string seperator = String.Empty;

            for (int i = 0; i < VersionUtil.KnownODataVersions.Length; i++)
            {
                Version v = VersionUtil.KnownODataVersions[i];
                if (v > maxRequestVersionAllowed)
                {
                    // We need to write all the versions less than or equal to maxRequestVersionAllowed in the error string.
                    break;
                }

                builder.Append(seperator);
                builder.Append(String.Format(CultureInfo.InvariantCulture, "'{0}'", v.ToString()));
                seperator = ", ";
            }

            return builder.ToString();
        }

        /// <summary>
        /// Returns the maximum allowed request version.
        /// </summary>
        /// <param name="maxProtocolVersion">The max protocol version specified in the service.</param>
        /// <returns>The maximum allowed request version.</returns>
        private static Version GetMaxRequestVersionAllowed(Version maxProtocolVersion)
        {
            Debug.Assert(maxProtocolVersion != null, "maxProtocolVersion != null");

            // the request version should always be less than or equal to MPV
            return maxProtocolVersion == VersionUtil.Version4Dot0 ? VersionUtil.Version4Dot0 : maxProtocolVersion;
        }

        /// <summary>
        /// Validates that the given URI is non-null and absolute, then saves it as the absolute request URI.
        /// </summary>
        /// <param name="value">The absolute request uri to validate and cache.</param>
        /// <param name="validateQueryString">Whether or not to validate that the query string has not changed from the host's value.</param>
        private void ValidateAndCacheAbsoluteRequestUri(Uri value, bool validateQueryString)
        {
            if (value == null)
            {
                throw new InvalidOperationException(Strings.RequestUriProcessor_AbsoluteRequestUriCannotBeNull);
            }

            if (!value.IsAbsoluteUri)
            {
                throw new InvalidOperationException(Strings.RequestUriProcessor_AbsoluteRequestUriMustBeAbsolute);
            }

            if (validateQueryString)
            {
                // For now, require the query string to be exactly the same. 
                // This needs to be this strict in order to support the following cases.
                // 1) Add to or replace the query string
                // 2) Completely remove the query string from the original URI by not providing one
                if (this.host.AbsoluteRequestUri.Query != value.Query)
                {
                    throw new InvalidOperationException(Strings.AstoriaRequestMessage_CannotChangeQueryString);
                }
            }

            this.absoluteRequestUri = value;
        }

        /// <summary>
        /// Validates that the given URI is non-null and absolute, then saves it as the absolute service URI.
        /// </summary>
        /// <param name="value">The absolute service uri to validate and cache.</param>
        private void ValidateAndCacheAbsoluteServiceUri(Uri value)
        {
            if (value == null)
            {
                throw new InvalidOperationException(Strings.RequestUriProcessor_AbsoluteServiceUriCannotBeNull);
            }

            if (!value.IsAbsoluteUri)
            {
                throw new InvalidOperationException(Strings.RequestUriProcessor_AbsoluteServiceUriMustBeAbsolute);
            }

            this.absoluteServiceUri = WebUtil.EnsureLastSegmentEmpty(value);
        }

        #endregion
    }
}
