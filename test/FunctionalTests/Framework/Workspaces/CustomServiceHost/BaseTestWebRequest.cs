//---------------------------------------------------------------------
// <copyright file="BaseTestWebRequest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using AstoriaUnitTests.Data;

    #endregion Namespaces

    /// <summary>
    /// Provides an enumerator of configurable web server locations.
    /// </summary>
    public enum WebServerLocation
    {
        #region Values.

        /// <summary>Specifies a web server that is running in-process.</summary>
        InProcess,

        /// <summary>Specifies an in-process, WCF-based host.</summary>
        InProcessWcf,

        /// <summary>Specifies an in-process, WCF-based host in Streamed transfer mode.</summary>
        InProcessStreamedWcf,

        /// <summary>Specifies a local web server that is easy to debug.</summary>
        Local,

        /// <summary>Specifies a (possibly) remote web server.</summary>
        Remote

        #endregion Values.
    }

    /// <summary>
    /// Provides support to make tests reusable across in-memory, local and remote web servers.
    /// </summary>
    [DebuggerDisplay("{HttpMethod} {FullRequestUriString}")]
    public abstract class BaseTestWebRequest : IDisposable
    {
        #region Private fields.

        /// <summary>Test arguments that can be serialized across to the server.</summary>
        private static Hashtable serializedTestArguments;

        /// <summary>Type of data service to encapsulate and run against, eg a custom ObjectContext.</summary>
        protected Type dataServiceType;

        /// <summary>The method for the request.</summary>
        private string httpMethod;

        /// <summary>Headers used in this request.</summary>
        private Dictionary<string, string> requestHeaders = new Dictionary<string, string>();

        /// <summary>The original URI of the request.</summary>
        private string requestUriString;

        /// <summary>input stream of the request.</summary>
        private Stream requestStream;

        /// <summary>Actual type of service to use.</summary>
        protected Type serviceType;

        /// <summary>Test arguments to serialize to the server.</summary>
        private Hashtable testArguments;

        /// <summary>Set to true to get additional stack traces for troubleshooting.</summary>
        internal static bool SaveOriginalStackTrace = false;

        private bool verboseErrors = true;

        private static Type hostInterfaceType = typeof(IDataServiceHost);
        #endregion Private fields.

        /// <summary>
        /// Initializes a new TestWebRequest instance with simple defaults 
        /// that have no side-effects on querying.
        /// </summary>
        protected BaseTestWebRequest()
        {
            this.httpMethod = "GET";
        }

        /// <summary>Disposes object resources from the Finalizer thread.</summary>
        ~BaseTestWebRequest()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Provides an opportunity to clean-up resources.
        /// </summary>
        /// <param name="disposing">
        /// Whether the call is being made from an explicit call to 
        /// IDisposable.Dispose() rather than through the finalizer.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Releases resources held onto by this object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region Properties.

        /// <summary>Uri location to the service entry point.</summary>
        public virtual string BaseUri
        {
            get
            {
                Debug.Assert(false, "not supported");
                throw new NotSupportedException();
            }
        }

        public virtual System.Uri ServiceRoot
        {
            get
            {
                return new System.Uri(this.BaseUri, UriKind.Absolute);
            }
        }

        /// <summary>Local web service locations.</summary>
        public static WebServerLocation[] LocalWebServerLocations
        {
            get
            {
                return new WebServerLocation[]
                {
                    WebServerLocation.InProcess,
                    WebServerLocation.InProcessWcf,
                    WebServerLocation.Local
                };
            }
        }

        /// <summary>Local web service locations that support passing arguments.</summary>
        public static WebServerLocation[] LocalWebServerLocationsWithArguments
        {
            get
            {
                return new WebServerLocation[]
                {
                    WebServerLocation.InProcess,
                    WebServerLocation.Local
                };
            }
        }

        /// <summary>Test arguments that were serialized across to the server.</summary>
        public static Hashtable SerializedTestArguments
        {
            get
            {
                if (BaseTestWebRequest.serializedTestArguments == null)
                {
                    BaseTestWebRequest.serializedTestArguments = new Hashtable();
                }

                return BaseTestWebRequest.serializedTestArguments;
            }

            set
            {
                BaseTestWebRequest.serializedTestArguments = value;
            }
        }

        public bool NotDataServiceOfT { get; set; }

        #endregion Properties.

        /// <summary>Loads the serialized test arguments for the current AppDomain.</summary>
        /// <param name="text">Text for test arguments.</param>
        /// <remarks>Passing null or an empty string clears the test arguments.</remarks>
        public static void LoadSerializedTestArguments(string text)
        {
            serializedTestArguments = new Hashtable();

            if (String.IsNullOrEmpty(text))
            {
                return;
            }

            int startIndex = 0;
            while (startIndex < text.Length)
            {
                string entryName = ExtractEntryPart(text, ref startIndex);
                if (text[startIndex] != '\'')
                {
                    throw new FormatException();
                }
                startIndex++;

                if (text[startIndex] != ':')
                {
                    throw new FormatException();
                }
                startIndex++;

                string entryType = ExtractEntryPart(text, ref startIndex);
                if (text[startIndex] != '\'')
                {
                    throw new FormatException();
                }
                startIndex++;

                if (text[startIndex] != '=')
                {
                    throw new FormatException();
                }
                startIndex++;

                string entryValueText = ExtractEntryPart(text, ref startIndex);
                if (text[startIndex] != '\'')
                {
                    throw new FormatException();
                }
                startIndex++;

                object entryValue = GetEntryValue(entryType, entryValueText);
                serializedTestArguments[entryName] = entryValue;

                startIndex++;
            }
        }

        /// <summary>
        /// Gets a typed test argument from the serialized test arguments for
        /// the current <see cref="AppDomain"/>.
        /// </summary>
        /// <typeparam name="T">Type of entry value.</typeparam>
        /// <param name="entryName">Entry name.</param>
        /// <param name="entryValue">Value for named entry.</param>
        /// <returns>true if the test argument was found; false otherwise.</returns>
        public static bool TryGetTestArgument<T>(string entryName, out T entryValue)
        {
            if (serializedTestArguments != null &&
                serializedTestArguments.ContainsKey(entryName))
            {
                entryValue = (T)serializedTestArguments[entryName];
                return true;
            }
            else
            {
                entryValue = default(T);
                return false;
            }
        }

        /// <summary>Creates a TestWebRequest that can run in-process.</summary>
        /// <returns>A new TestWebRequest instance.</returns>
        public static BaseTestWebRequest CreateForInProcess()
        {
            return new TestInProcessWebRequest();
        }

        /// <summary>Creates a TestHttpListenerWebRequest that can run in-process.</summary>
        /// <returns>A new TestWebRequest instance.</returns>
        public static BaseTestWebRequest CreateForHttpListener()
        {
            return new TestHttpListenerWebRequest();
        }

        /// <summary>
        /// Creates a new TestWebRequest instance configured for the specified location.
        /// </summary>
        /// <param name="location">Web server location.</param>
        /// <returns>A new TestWebRequest instance.</returns>
        /// <remarks>
        /// When we have a more complete notion of what 'environment' or 'server requirements' are,
        /// those should also be passed in.
        /// </remarks>
        public static BaseTestWebRequest CreateForLocation(WebServerLocation location)
        {
            switch (location)
            {
                case WebServerLocation.InProcess:
                    return CreateForInProcess();
                default:
                    throw new ArgumentException("Unrecognized value for location.", "location");
            }
        }

        /// <summary>
        /// Returns the server response stream after a call to SendRequest.
        /// </summary>
        /// <returns>The server response stream after a call to SendRequest.</returns>
        public abstract Stream GetResponseStream();

        /// <summary>
        /// Returns the server response text after a call to SendRequest.
        /// </summary>
        /// <returns>The server response text after a call to SendRequest.</returns>
        public string GetResponseStreamAsText()
        {
            Stream stream = this.GetResponseStream();
            if (stream == null)
            {
                throw new InvalidOperationException("GetResponseStream() returned null - ensure SendRequest was called before.");
            }

            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Returns the server response text after a call to SendRequest.
        /// </summary>
        /// <returns>The server response text after a call to SendRequest.</returns>
        public XmlDocument GetResponseStreamAsXmlDocument()
        {
            using (Stream stream = this.GetResponseStream())
            {
                if (stream == null)
                {
                    throw new InvalidOperationException("GetResponseStream() returned null - ensure SendRequest was called before.");
                }

                XmlDocument document = new XmlDocument(TestUtil.TestNameTable);
                document.Load(stream);
                return document;
            }
        }

        /// <summary>
        /// Returns the server response text after a call to SendRequest.
        /// </summary>
        /// <returns>The server response text after a call to SendRequest.</returns>
        public virtual XmlDocument GetResponseStreamAsXmlDocument(string responseFormat)
        {
            throw new NotImplementedException("Don't call this");
        }

        /// <summary>
        /// Returns the server response as XML after a call to SendRequest.
        /// </summary>
        /// <returns>The server response parsed as XML and returned as XDocument instance.</returns>
        public XDocument GetResponseStreamAsXDocument()
        {
            using (Stream stream = this.GetResponseStream())
            {
                if (stream == null)
                {
                    throw new InvalidOperationException("GetResponseStream() returned null - ensure SendRequest was called before.");
                }
                
                XDocument document = XDocument.Load(XmlReader.Create(stream));
                return document;
            }
        }

        /// <summary>Sets the <see cref="RequestStream"/> to the specified UTF-8 text.</summary>
        /// <param name="text">Text to set.</param>
        public void SetRequestStreamAsText(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            this.RequestStream = new MemoryStream(bytes);
            this.RequestContentLength = (int)this.RequestStream.Length;
        }

        /// <summary>Sends the current request to the server.</summary>
        public abstract void SendRequest();

        public Exception CreateExceptionFromError(XmlElement element)
        {
            Debug.Assert(element != null, "element != null");
            string message = element.SelectSingleNode("./message", TestUtil.TestNamespaceManager).InnerText;
            Exception result = new Exception(message);
            return result;
        }

        public Exception CreateExceptionFromError(System.Xml.Linq.XElement element)
        {
            Debug.Assert(element != null, "element != null");
            string message = element.Elements().Where(e => e.Name.LocalName == "message").Single().Value;
            Exception result = new Exception(message);
            return result;
        }

        /// <summary>
        /// Sends the current request to the server, and tries to throw an 
        /// exception if the response indicates an error occurred.
        /// </summary>
        public virtual void SendRequestAndCheckResponse()
        {
            throw new NotImplementedException("Don't call this");
        }

        /// <summary>Starts the service.</summary>
        public virtual void StartService()
        {
        }

        /// <summary>Stops the service.</summary>
        public virtual void StopService()
        {
        }

        /// <summary>Value for the Accept header (MIME specification).</summary>
        public string Accept
        {
            [DebuggerStepThrough]
            get { return this.GetRequestHeaderValue(TestServiceHost.HttpAccept); }
            set { this.requestHeaders[TestServiceHost.HttpAccept] = value; }
        }

        /// <summary>Value for the Accept-Charset header.</summary>
        public string AcceptCharset
        {
            [DebuggerStepThrough]
            get { return this.GetRequestHeaderValue(TestServiceHost.HttpAcceptCharset); }
            set { this.requestHeaders[TestServiceHost.HttpAcceptCharset] = value; }
        }

        /// <summary>Type of data service to encapsulate and run against, eg a custom ObjectContext.</summary>
        public virtual Type DataServiceType
        {
            [DebuggerStepThrough]
            get { return this.dataServiceType; }
            set
            {
                this.dataServiceType = value;
                if (value == null)
                {
                    this.serviceType = null;
                }
                else
                {
                    // TODO: this.serviceType = typeof(OpenWebDataService<>).MakeGenericType(this.dataServiceType);
                }
            }
        }

        /// <summary>Full request URI string, based on RequestUriString, including protocol and host name.</summary>
        public virtual string FullRequestUriString
        {
            get { return this.RequestUriString; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>Gets custom headers for this request.</summary>
        public Dictionary<string, string> RequestHeaders
        {
            [DebuggerStepThrough]
            get { return this.requestHeaders; }
        }

        /// <summary>Gets or sets the method for the request.</summary>
        public string HttpMethod
        {
            [DebuggerStepThrough]
            get
            {
                return this.httpMethod;
            }
            set { this.httpMethod = value; }
        }

        /// <summary>Gets or sets the value for the OData-MaxVersion request header.</summary>
        public string RequestMaxVersion
        {
            [DebuggerStepThrough]
            get { return this.GetRequestHeaderValue(TestServiceHost.HttpMaxDataServiceVersion); }
            set { this.requestHeaders[TestServiceHost.HttpMaxDataServiceVersion] = value; }

        }

        /// <summary>Gets or sets the method for the request.</summary>
        public Stream RequestStream
        {
            get { return this.requestStream; }
            set { this.requestStream = value; }
        }

        /// <summary>Gets or sets the value for the OData-Version request header.</summary>
        public string RequestVersion
        {
            [DebuggerStepThrough]
            get { return this.GetRequestHeaderValue(TestServiceHost.HttpDataServiceVersion); }
            set { this.requestHeaders[TestServiceHost.HttpDataServiceVersion] = value; }
        }

        /// <summary>Gets or sets the original URI of the request.</summary>
        public virtual string RequestUriString
        {
            [DebuggerStepThrough]
            get { return this.requestUriString; }
            set { this.requestUriString = value; }
        }

        /// <summary>Gets response headers for this request.</summary>
        public abstract Dictionary<string, string> ResponseHeaders
        {
            get;
        }

        /// <summary>Cache-Control header in response.</summary>
        public abstract string ResponseCacheControl
        {
            get;
        }

        /// <summary>ContentType header for response.</summary>
        public abstract string ResponseContentType
        {
            get;
        }

        /// <summary>Location header for response.</summary>
        public abstract string ResponseLocation
        {
            get;
        }

        /// <summary>ETag header in response, available after SendRequest.</summary>
        public abstract string ResponseETag
        {
            get;
        }

        /// <summary>Response status code, available after SendRequest.</summary>
        public abstract int ResponseStatusCode
        {
            get;
        }

        /// <summary>Gets or sets the value for the OData-Version response header.</summary>
        public abstract string ResponseVersion
        {
            get;
        }

        /// <summary>Gets the HTTP MIME type of the input stream.</summary>
        public string RequestContentType
        {
            [DebuggerStepThrough]
            get { return this.GetRequestHeaderValue(TestServiceHost.HttpContentType); }
            set { this.requestHeaders[TestServiceHost.HttpContentType] = value; }
        }

        /// <summary>Gets/Sets the length of the request content stream in bytes.</summary>
        public int RequestContentLength
        {
            [DebuggerStepThrough]
            get
            {
                string contentLength = this.GetRequestHeaderValue(TestServiceHost.HttpContentLength);
                if (contentLength == null)
                {
                    return -1;
                };

                return Int32.Parse(contentLength);
            }

            set
            {
                this.requestHeaders[TestServiceHost.HttpContentLength] = value.ToString();
            }
        }

        /// <summary>Actual type of service to run against, eg DataService&lt;NorthwindContext&gt;.</summary>
        public virtual Type ServiceType
        {
            [DebuggerStepThrough]
            get { return this.serviceType; }
            set
            {
                if (value == null)
                {
                    this.serviceType = null;
                    this.dataServiceType = null;
                }
                else
                {
                    this.serviceType = value;

                    Type type = value;
                    while (
                        !type.IsGenericType ||
                        type.GetGenericTypeDefinition() != typeof(DataService<>))
                    {
                        type = type.BaseType;
                        if (this.NotDataServiceOfT = (type == null))
                        {
                            throw new Exception("ServiceType should be of type DataService<T>");
                        }
                    }

                    this.dataServiceType = type.GetGenericArguments()[0];
                }
            }
        }

        /// <summary> Gets/Sets If-Match header value</summary>
        public string IfMatch
        {
            [DebuggerStepThrough]
            get { return this.GetRequestHeaderValue(TestServiceHost.HttpIfMatch); }
            set { this.requestHeaders[TestServiceHost.HttpIfMatch] = value; }
        }

        /// <summary> Gets/Sets If-None-Match header value</summary>
        public string IfNoneMatch
        {
            [DebuggerStepThrough]
            get { return this.GetRequestHeaderValue(TestServiceHost.HttpIfNoneMatch); }
            set { this.requestHeaders[TestServiceHost.HttpIfNoneMatch] = value; }
        }

        /// <summary>Test arguments to serialize to the server.</summary>
        public Hashtable TestArguments
        {
            [DebuggerStepThrough]
            get { return this.testArguments; }
            set { this.testArguments = value; }
        }

        /// <summary>Whether the request should force verbose errors from the server.</summary>
        public bool ForceVerboseErrors
        {
            get { return this.verboseErrors; }
            set { this.verboseErrors = value; }
        }

        /// <summary>Replace semantics for PUT operations</summary>
        public bool ReplaceSemantics { get; set; }

        public static Type HostInterfaceType
        {
            get { return hostInterfaceType; }
            set
            {
                // make sure that the type derives from IDataServiceHost
                if (!typeof(IDataServiceHost).IsAssignableFrom(value))
                {
                    throw new InvalidOperationException("Invalid value set for HostInterfaceType property on BaseTestWebRequest. Make sure the value is of IDataServiceHost interface");
                }

                hostInterfaceType = value;
            }
        }

        /// <summary>
        /// Returns a string representation of the specified <paramref name="values"/>.
        /// </summary>
        /// <param name="values"><see cref="Hashtable"/> of values to turn into string.</param>
        /// <returns>
        /// A string (possibly empty) representating the specified <paramref name="values"/>.
        /// </returns>
        public static string ArgumentsToString(Hashtable values)
        {
            if (values == null)
            {
                return "";
            }

            // Format:
            // result ::= entry *("," entry)
            // entry  ::= name ":" type "=" value
            // name   ::= qstring
            // type   ::= qstring
            // value  ::= qstring
            // qstring::= "'" char "'"
            // char   ::= any character, with single quotes doubled

            bool first = true;
            StringBuilder resultBuilder = new StringBuilder();
            foreach (DictionaryEntry entry in values)
            {
                string entryName = entry.Key.ToString();
                string entryValueText;
                string entryTypeText;
                object entryValue = entry.Value;
                if (entryValue == null)
                {
                    entryValueText = "null";
                    entryTypeText = "null";
                }
                else if (entryValue is IStringIdentifierSupport)
                {
                    entryTypeText = entryValue.GetType().FullName;
                    entryValueText = ((IStringIdentifierSupport)entryValue).StringIdentifier;
                }
                else
                {
                    entryTypeText = entryValue.GetType().FullName;
                    entryValueText = entryValue.ToString();
                }

                entryName = entryName.Replace("'", "''");
                entryValueText = entryValueText.Replace("'", "''");

                if (first)
                {
                    first = false;
                }
                else
                {
                    resultBuilder.Append(",");
                }

                resultBuilder.Append('\'');
                resultBuilder.Append(entryName);
                resultBuilder.Append("':'");
                resultBuilder.Append(entryTypeText);
                resultBuilder.Append("'='");
                resultBuilder.Append(entryValueText);
                resultBuilder.Append('\'');
            }

            return resultBuilder.ToString();
        }

        private static string ExtractEntryPart(string text, ref int startIndex)
        {
            if (text[startIndex] != '\'')
            {
                throw new FormatException();
            }

            startIndex++;
            int savedIndex = startIndex;
            while (true)
            {
                if (text[startIndex] == '\'')
                {
                    if (startIndex < text.Length - 1 && text[startIndex + 1] == '\'')
                    {
                        startIndex++;
                    }
                    else
                    {
                        break;
                    }
                }
                startIndex++;
            }

            return text.Substring(savedIndex, startIndex - savedIndex).Replace("''", "'");
        }

        /// <summary>Gets the typed valued of an entry, given its type and value.</summary>
        /// <param name="entryTypeText">String representation of entry type.</param>
        /// <param name="entryValueText">String representation of entry value.</param>
        /// <returns></returns>
        private static object GetEntryValue(string entryTypeText, string entryValueText)
        {
            if (entryTypeText == "null")
            {
                return null;
            }

            Type type = ResolveType(entryTypeText);
            PropertyInfo valuesProperty = type.GetProperty("Values", BindingFlags.Static | BindingFlags.Public);
            if (valuesProperty != null)
            {
                IEnumerable e = (IEnumerable)valuesProperty.GetValue(null, null);
                foreach (object o in e)
                {
                    if (o == null)
                    {
                        continue;
                    }

                    IStringIdentifierSupport identifier = o as IStringIdentifierSupport;
                    string objectAsText = (identifier == null) ? o.ToString() : identifier.StringIdentifier;
                    if (objectAsText == entryValueText)
                    {
                        return o;
                    }
                }

                throw new InvalidOperationException("Unable to resolve identifier-supporting value: [" + entryValueText + "]");
            }
            else if (type.IsEnum)
            {
                return Enum.Parse(type, entryValueText);
            }
            else if (typeof(Type).IsAssignableFrom(type))
            {
                return ResolveType(entryValueText);
            }
            else
            {
                return Convert.ChangeType(entryValueText, type);
            }
        }

        /// <summary>Resolves the specified <paramref name="typeName"/> into a Type.</summary>
        /// <param name="typeName">The full name of the type to be resolved.</param>
        /// <returns>The <see cref="Type"/> named by <paramref name="typeName"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// This exception is thrown when <paramref name="typeName"/> cannot
        /// be resolved.
        /// </exception>
        private static Type ResolveType(string typeName)
        {
            Type type = Type.GetType(typeName, false);
            if (type == null)
            {
                type = typeof(IDataServiceHost).Assembly.GetType(typeName);
                if (type == null)
                {
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (Assembly assembly in assemblies)
                    {
                        if (assembly.FullName.Contains("AstoriaUnitTests"))
                        {
                            type = assembly.GetType(typeName);
                            break;
                        }
                    }
                }
            }

            if (type == null)
            {
                throw new InvalidOperationException("Unable to resolve type: " + typeName);
            }

            return type;
        }

        private string GetRequestHeaderValue(string headerName)
        {
            string headerValue;
            this.requestHeaders.TryGetValue(headerName, out headerValue);
            return headerValue;
        }
    }

    /// <summary>
    /// Provides a TestWebRequest subclass that can handle requests in the
    /// current process space.
    /// </summary>
    public class TestInProcessWebRequest : BaseTestWebRequest
    {
        #region Private fields.

        /// <summary>In-process host used for the last request.</summary>
        private TestServiceHost2 host;

        /// <summary>Response stream, as returned by the server.</summary>
        private Stream responseStream;

        #endregion Private fields.

        #region Properties.

        /// <summary>Uri location to the service entry point.</summary>
        public override string BaseUri
        {
            get
            {
                return "http://host/";
            }
        }

        /// <summary>Full request URI string, based on RequestUriString, including protocol and host name.</summary>
        public override string FullRequestUriString
        {
            get { return this.RequestUriString; }
            set
            {
                if (!value.StartsWith(this.BaseUri))
                {
                    throw new NotImplementedException("Support for FullRequestUriString different from 'http://host/' NYI.");
                }
                else
                {
                    this.RequestUriString = "/" + value.Remove(0, this.BaseUri.Length);
                }
            }
        }

        /// <summary>Gets response headers for this request.</summary>
        public override Dictionary<string, string> ResponseHeaders
        {
            get
            {
                Dictionary<string, string> responseHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (KeyValuePair<string, string> header in this.host.ResponseHeaders)
                {
                    responseHeaders[header.Key] = header.Value;
                }

                return responseHeaders;
            }
        }

        /// <summary>Cache-Control header in response.</summary>
        public override string ResponseCacheControl
        {
            get { return this.host.ResponseCacheControl; }
        }

        /// <summary>Location header for response.</summary>
        public override string ResponseLocation
        {
            get { return this.host.ResponseLocation; }
        }

        public override string ResponseVersion
        {
            get { return this.host.ResponseVersion; }
        }

        /// <summary>ContentType header for response.</summary>
        public override string ResponseContentType
        {
            [DebuggerStepThrough]
            get
            {
                return host.ResponseContentType;
            }
        }

        public override string ResponseETag
        {
            get { return this.host.ResponseETag; }
        }

        public override int ResponseStatusCode
        {
            get { return this.host.ResponseStatusCode; }
        }

        #endregion Properties.

        #region Methods.

        /// <summary>Gets the response stream, as returned by the server.</summary>
        [DebuggerStepThrough]
        public override Stream GetResponseStream()
        {
            return responseStream;
        }

        /// <summary>Sends the current request to the server.</summary>
        public override void SendRequest()
        {
            if (this.ServiceType == null)
            {
                throw new InvalidOperationException("DataServiceType or ServiceType property should be set for in-process requests.");
            }

            // Set up an in-process host.
            host = new TestServiceHost2();
            host.RequestAcceptCharSet = this.AcceptCharset;
            host.RequestHttpMethod = this.HttpMethod;
            host.RequestPathInfo = this.RequestUriString;
            host.RequestStream = this.RequestStream;
            host.RequestContentType = this.RequestContentType;
            host.RequestContentLength = this.RequestContentLength;
            host.RequestIfMatch = this.IfMatch;
            host.RequestIfNoneMatch = this.IfNoneMatch;
            host.RequestMaxVersion = this.RequestMaxVersion;
            foreach (var header in this.RequestHeaders)
            {
                host.RequestHeaders.Add(header.Key, header.Value);
            }

            host.RequestVersion = this.RequestVersion;

            if (this.Accept != null)
            {
                host.RequestAccept = this.Accept;
            }

            // An important side-effect of the following line of code is to
            // clean the TestArguments static for tests which don't use it.
            BaseTestWebRequest.LoadSerializedTestArguments(ArgumentsToString(this.TestArguments));
            try
            {
                // Create a data service instance and hook into the processing directly - only GET supported right now.
                Type serviceType = this.ServiceType;
                try
                {
                    object serviceInstance = Activator.CreateInstance(serviceType);

                    MethodInfo attachMethod = serviceType.GetMethod("AttachHost");
                    attachMethod.Invoke(serviceInstance, new object[] { host });

                    MethodInfo method = serviceType.GetMethod("ProcessRequest", Type.EmptyTypes);
                    method.Invoke(serviceInstance, null);

                    this.responseStream = GetResponseStream(host);
                }
                catch (TargetInvocationException invocationException)
                {
                    this.responseStream = GetResponseStream(host);
                    if (invocationException.InnerException != null)
                    {
                        if (BaseTestWebRequest.SaveOriginalStackTrace &&
                            invocationException.InnerException.Data != null)
                        {
                            invocationException.InnerException.Data["OriginalStackTrace"] = invocationException.InnerException.StackTrace;
                        }

                        throw invocationException.InnerException;
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    if (this.RequestStream != null)
                    {
                        AstoriaTestLog.IsTrue(this.RequestStream.CanRead, "request stream must be open");
                    }

                    if (this.responseStream != null)
                    {
                        AstoriaTestLog.IsTrue(this.responseStream.CanRead, "response stream must be open");
                    }
                }
            }
            finally
            {
                LoadSerializedTestArguments(null);
            }
        }

        private static Stream GetResponseStream(TestServiceHost host)
        {
            Debug.Assert(host != null, "host != null");
            Stream result = host.ResponseStream;
            if (result != null)
            {
                result.Position = 0;
            }
            return result;
        }

        #endregion Methods.
    }

    /// <summary>
    /// Provides a TestWebRequest subclass that can handle requests.
    /// </summary>
    public class TestHttpListenerWebRequest : BaseTestWebRequest
    {
        #region Private fields.

        /// <summary>HttpListenerHost host used for the request.</summary>
        private HttpListenerHost host;

        /// <summary>dummy response stream, needed to compile.</summary>
        private Stream responseStream = null;
        
        #endregion Private fields.

        #region Properties.

        /// <summary>Uri location to the service entry point.</summary>
        public override string BaseUri
        {
            get { return null; }
        }

        /// <summary>Full request URI string, based on RequestUriString, including protocol and host name.</summary>
        public override string FullRequestUriString
        {
            get { return null; }
            set { return; }
        }

        /// <summary>Gets response headers for this request.</summary>
        public override Dictionary<string, string> ResponseHeaders
        {
            get { return null; }
        }

        /// <summary>Cache-Control header in response.</summary>
        public override string ResponseCacheControl
        {
            get { return null; }
        }

        /// <summary>Location header for response.</summary>
        public override string ResponseLocation
        {
            get { return null;} 
        }

        public override string ResponseVersion
        {
            get { return null;}
        }

        /// <summary>ContentType header for response.</summary>
        public override string ResponseContentType
        {
            get
            {
                return null;
            }
        }

        public override string ResponseETag
        {
            get { return "";} 
        }

        public override int ResponseStatusCode
        {
            get { return 0;}
        }

        #endregion Properties.

        #region Methods.

        /// <summary>Gets the response stream, as returned by the server.</summary>
        public override Stream GetResponseStream()
        {
            return responseStream;
        }

        /// <summary>An empty method to make compiler happy as this class implements BaseTestWebRequest.</summary> 
        public override void SendRequest()
        {
            return;
        }
        /// <summary>Invoke the service and get response.</summary>
        public void SendRequest(HttpListenerHost myhost)
        {
            if (this.ServiceType == null)
            {
                throw new InvalidOperationException("DataServiceType or ServiceType property should be set for in-process requests.");
            }
            host = myhost;
           
                Type serviceType = this.ServiceType;
                try
                {
                    object serviceInstance = Activator.CreateInstance(serviceType);

                    MethodInfo attachMethod = serviceType.GetMethod("AttachHost");
                    attachMethod.Invoke(serviceInstance, new object[] { host });

                    MethodInfo method = serviceType.GetMethod("ProcessRequest", Type.EmptyTypes);
                    method.Invoke(serviceInstance, null);
               }
                catch (TargetInvocationException invocationException)
                {
                    if (invocationException.InnerException != null)
                    {
                        if (BaseTestWebRequest.SaveOriginalStackTrace &&
                            invocationException.InnerException.Data != null)
                        {
                            invocationException.InnerException.Data["OriginalStackTrace"] = invocationException.InnerException.StackTrace;
                        }

                        throw invocationException.InnerException;
                    }
                    else
                    {
                        throw;
                    }
                }
            }    
        #endregion Methods.
    }
}
