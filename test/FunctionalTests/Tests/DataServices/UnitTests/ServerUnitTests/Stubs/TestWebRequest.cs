//---------------------------------------------------------------------
// <copyright file="TestWebRequest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Configuration;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.DirectoryServices;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using System.Xml;
    using AstoriaUnitTests.Data;
    using AstoriaUnitTests.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion Namespaces

    /// <summary>
    /// Provides support to make tests reusable across in-memory, local and remote web servers.
    /// </summary>
    [DebuggerDisplay("{HttpMethod} {FullRequestUriString}")]
    public abstract class TestWebRequest : BaseTestWebRequest
    {
        /// <summary>List of actions to call once this request is disposed.</summary>
        private List<Action> disposeList = null;

        /// <summary>Set to true to get additional stack traces for troubleshooting.</summary>
        internal static bool SaveOriginalStackTrace = false;

        /// <summary>
        /// Initializes a new TestWebRequest instance with simple defaults 
        /// that have no side-effects on querying.
        /// </summary>
        protected TestWebRequest()
            : base()
        {
        }

        /// <summary>
        /// Gets the response body of an error in the request.
        /// </summary>
        public string ErrorResponseContent { get; set; }

        /// <summary>Creates a TestWebRequest that can run with a local, easy-to-debu server.</summary>
        /// <returns>A new TestWebRequest instance.</returns>
        public static TestWebRequest CreateForLocal()
        {
            return new LocalWebRequest();
        }

        /// <summary>Creates a TestWebRequest that can run with a remote server.</summary>
        /// <returns>A new TestWebRequest instance.</returns>
        public static TestWebRequest CreateForRemote()
        {
            return new RemoteWebRequest();
        }

        /// <summary>Creates a TestWebRequest that can run in-process.</summary>
        /// <returns>A new TestWebRequest instance.</returns>
        public new static TestWebRequest CreateForInProcess()
        {
            return new InProcessWebRequest();
        }

        /// <summary>Creates a TestWebRequest that can run in-process on WCF.</summary>
        /// <returns>A new TestWebRequest instance.</returns>
        public static TestWebRequest CreateForInProcessWcf()
        {
            return new InProcessWcfWebRequest();
        }

        /// <summary>Creates a TestWebRequest that can run in-process on WCF streamed transfer mode.</summary>
        /// <returns>A new TestWebRequest instance.</returns>
        public static TestWebRequest CreateForInProcessStreamedWcf()
        {
            return new InProcessStreamedWcfWebRequest();
        }

        public new static TestWebRequest CreateForLocation(WebServerLocation location)
        {
            switch (location)
            {
                case WebServerLocation.InProcess:
                    return CreateForInProcess();
                case WebServerLocation.InProcessWcf:
                    return CreateForInProcessWcf();
                case WebServerLocation.InProcessStreamedWcf:
                    return CreateForInProcessStreamedWcf();
                case WebServerLocation.Local:
                    return CreateForLocal();
                case WebServerLocation.Remote:
                    return CreateForRemote();
                default:
                    throw new ArgumentException("Unrecognized value for location.", "location");
            }
        }

        /// <summary>
        /// Verify that standard response headers are present.
        /// </summary>
        protected void CheckResponseHeaders()
        {
            ExpectedHeader[] expectedHeaders = new ExpectedHeader[]
            {
                new ExpectedHeader { Header = "X-Content-Type-Options", Value = "nosniff", ContractIsV2AndAbove = true }
            };
            
            bool hostIsV2ContractAndAbove = (BaseTestWebRequest.HostInterfaceType == typeof(IDataServiceHost2));
            foreach (ExpectedHeader expectedHeader in expectedHeaders)
            {
                // check if the expected header can be applied using a V1 contract (!expectedHeader.ContractIsV2AndAbove)
                // or, if the host is V2 and above (hostIsV2ContractAndAbove) any header is applicable
                if (!expectedHeader.ContractIsV2AndAbove || hostIsV2ContractAndAbove)
                {
                    string value;
                    if (!this.ResponseHeaders.TryGetValue(expectedHeader.Header, out value))
                    {
                        Assert.Fail(string.Format("The expected header {0} is missing", expectedHeader.Header));
                        continue;
                    }

                    expectedHeader.VerifyHeader(value);
                }
            }
        }

        /// <summary>
        /// Returns the server response text after a call to SendRequest.
        /// </summary>
        /// <returns>The server response text after a call to SendRequest.</returns>
        public override XmlDocument GetResponseStreamAsXmlDocument(string responseFormat)
        {
            Stream stream = this.GetResponseStream();
            if (stream == null)
            {
                throw new InvalidOperationException("GetResponseStream() returned null - ensure SendRequest was called before.");
            }

            if (TestUtil.CompareMimeType(responseFormat, UnitTestsUtil.JsonLightMimeType) ||
                TestUtil.CompareMimeType(responseFormat, UnitTestsUtil.JsonLightMimeTypeFullMetadata) ||
                TestUtil.CompareMimeType(responseFormat, UnitTestsUtil.JsonLightMimeTypeNoMetadata))
            {
                return JsonValidator.ConvertToXmlDocument(stream);
            }
            else
            {
                XmlDocument document = new XmlDocument(TestUtil.TestNameTable);
                document.Load(stream);
                stream.Dispose();
                return document;
            }
        }

        /// <summary>
        /// Sends the current request to the server, and tries to throw an 
        /// exception if the response indicates an error occurred.
        /// </summary>
        public override void SendRequestAndCheckResponse()
        {
            this.SendRequest();
            string mediaType = new System.Net.Mime.ContentType(this.ResponseContentType).MediaType;
            SerializationFormatData data = SerializationFormatData.Values.Where(
                format => format.MimeTypes.Any(m => String.Equals(m, mediaType, StringComparison.OrdinalIgnoreCase))).Single();
            if (data.IsStructured)
            {
                Stream stream = TestUtil.EnsureStreamWithSeek(this.GetResponseStream());
                XmlReader reader = XmlReader.Create(stream);
                while (reader.Read())
                {
                    if (reader.LocalName == "error")
                    {
                        throw this.CreateExceptionFromError(
                            System.Xml.Linq.XElement.Load(reader, System.Xml.Linq.LoadOptions.PreserveWhitespace));
                    }
                }

                stream.Position = 0;
            }
            else if (data.Name == "Text")
            {
                string text = this.GetResponseStreamAsText();
                if (text.Contains("<?xml"))
                {
                    throw new Exception(text);
                }
            }
            else
            {
                // TODO: implement.
            }
        }

        /// <summary>Type of data service to encapsulate and run against, eg a custom ObjectContext.</summary>
        public override Type DataServiceType
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
                    Type type = value;
                    bool dataContextType = true;
                    while (type != null && type != typeof(object))
                    {
                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(DataService<>))
                        {
                            this.ServiceType = value;
                            dataContextType = false;
                            break;
                        }

                        type = type.BaseType;
                    }

                    if (dataContextType)
                    {
                        this.serviceType = typeof(OpenWebDataService<>).MakeGenericType(this.dataServiceType);
                    }
                }
            }
        }

        /// <summary>Actual type of service to run against, eg DataService&lt;NorthwindContext&gt;.</summary>
        public override Type ServiceType
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
                            // Make an exception for the generic playback service.
                            //if (value == typeof(PlaybackService))
                            {
                                base.DataServiceType = typeof(CustomDataContext);
                                return;
                            }

                            // throw new Exception("ServiceType should be of type DataService<T>");
                        }
                    }

                    this.dataServiceType = type.GetGenericArguments()[0];
                }
            }
        }

        /// <summary>Registers an action to execute when this request is disposed.</summary>
        /// <param name="action">The action to execute.</param>
        public void RegisterForDispose(Action action)
        {
            if (this.disposeList == null)
            {
                this.disposeList = new List<Action>();
            }

            this.disposeList.Add(action);
        }

        /// <summary>Registers an <see cref="IDisposable"/> to be disposed when this request is disposed.</summary>
        /// <param name="disposable">The object to dispose.</param>
        public void RegisterForDispose(IDisposable disposable)
        {
            this.RegisterForDispose(() => disposable.Dispose());
        }

        /// <summary>
        /// Disposes the request.
        /// </summary>
        /// <param name="disposing">
        /// Whether the call is being made from an explicit call to 
        /// IDisposable.Dispose() rather than through the finalizer.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (this.disposeList != null)
            {
                foreach (Action action in this.disposeList)
                {
                    action();
                }
            }

            base.Dispose(disposing);
        }

        private class ExpectedHeader
        {
            public string Header { get; set; }

            public string Value { get; set; }

            public bool ExpectedContains { get; set; }

            public bool ContractIsV2AndAbove { get; set; }

            public void VerifyHeader(string actualValue)
            {
                if (this.ExpectedContains)
                {
                    HashSet<string> actualValues = new HashSet<string>(
                        actualValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    Assert.IsTrue(actualValues.Contains(this.Value), string.Format(
                        "Header \"{0}:{1}\" does not contain value \"{2}\"", this.Header, actualValue, this.Value));
                }
                else
                {
                    Assert.AreEqual(this.Value, actualValue, string.Format(
                        "Header \"{0}:{1}\" must exactly have the value \"{2}\"", this.Header, actualValue, this.Value));
                }
            }
        }
    }

    /// <summary>
    /// Provides a TestWebRequest subclass that can handle requests in a local debugging web server.
    /// </summary>
    public class LocalWebRequest : HttpBasedWebRequest
    {
        /// <summary>Name of the service file under test, with no path information.</summary>
        private const string ServiceFileName = "service.svc";

        /// <summary>Whether the local web server has been setup for the current configuration.</summary>
        private bool localWebSetup;

        /// <summary>Last data type setup.</summary>
        private Type lastDataServiceType;

        /// <summary>Last test arguments setup.</summary>
        private string lastTestArguments;

        private string configSnippet;

        private string initializeServiceCodeSnippet;

        /// <summary>
        /// Shuts down or kills the debugging web server.
        /// </summary>
        /// <param name="disposing">
        /// Whether the call is being made from an explicit call to 
        /// IDisposable.Dispose() rather than through the finalizer.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (localWebSetup)
            {
                ShutdownLocalWeb();
            }

            base.Dispose(disposing);
        }

        /// <summary>Starts the service.</summary>
        public override void StartService()
        {
            this.SetupLocalWeb();
        }

        /// <summary>Stops the service.</summary>
        public override void StopService()
        {
            this.ShutdownLocalWeb();
        }

        public void SetupLocalWeb()
        {
            string currentTestArguments = ArgumentsToString(this.TestArguments);
            if (lastDataServiceType != this.DataServiceType ||
                lastTestArguments != currentTestArguments &&
                (this.DataServiceType == typeof(CustomDataContext) && CustomDataContext.LocalWebClearPending))
            {
                if (localWebSetup)
                {
                    ShutdownLocalWeb();
                }

                if (CustomDataContext.LocalWebClearPending)
                {
                    AstoriaUnitTests.Tests.LocalWebServerHelper.Cleanup();
                }

                IOUtil.EnsureFileDeleted(Path.Combine(AstoriaUnitTests.Tests.LocalWebServerHelper.FileTargetPath, "web.config"));
                CustomDataContext.LocalWebClearPending = false;
                AstoriaUnitTests.Tests.LocalWebServerHelper.StartWebServer();
                SetupServiceFiles();
                this.lastDataServiceType = this.DataServiceType;
                this.lastTestArguments = currentTestArguments;
                localWebSetup = true;
            }
        }

        public void ShutdownLocalWeb()
        {
            AstoriaUnitTests.Tests.LocalWebServerHelper.DisposeProcess();
            localWebSetup = false;
            lastDataServiceType = null;
            lastTestArguments = null;
        }

        /// <summary>Sends the current request to the server.</summary>
        public override void SendRequest()
        {
            SetupLocalWeb();
            base.SendRequest();
        }

        public void AddToConfig(DataServicesFeaturesSection featuresSection)
        {
            Assert.IsNotNull(featuresSection != null, "featuresSection != null");

            this.configSnippet =
                "<configSections>\r\n" +
                "  <sectionGroup name=\"wcfDataServices\" type=\"Microsoft.OData.Service.Configuration.DataServicesSectionGroup\">\r\n" +
                "    <section name=\"features\" type=\"Microsoft.OData.Service.Configuration.DataServicesFeaturesSection\" allowLocation=\"true\" allowDefinition=\"Everywhere\" />\r\n" +
                "  </sectionGroup>\r\n" +
                "</configSections>\r\n";

            this.configSnippet +=
                "<wcfDataServices>\r\n" +
                "  <features>\r\n" +
                "    <replaceFunction enable = \"" + (featuresSection.ReplaceFunction.Enable ? "true" : "false") + "\" />\r\n" +
                "  </features>\r\n" +
                "</wcfDataServices>";
        }

        public void AddToInitializeService(DataServicesFeaturesSection featuresSection)
        {
            Assert.IsNotNull(featuresSection != null, "featuresSection != null");
            this.initializeServiceCodeSnippet = "configuration.DataServiceBehavior.AcceptReplaceFunctionInQuery = " + (featuresSection.ReplaceFunction.Enable ? "true" : "false") + ";\r\n";
        }

        /// <summary>
        /// Sets up the required files locally to test the web data service
        /// through the local web server.
        /// </summary>
        private void SetupServiceFiles()
        {
            serviceEntryPointLocation = AstoriaUnitTests.Tests.LocalWebServerHelper.SetupServiceFiles(
                ServiceFileName,
                this.DataServiceType,
                this.ServiceType,
                BaseTestWebRequest.ArgumentsToString(this.TestArguments),
                configSnippet: this.configSnippet,
                initializeServiceCode: this.initializeServiceCodeSnippet
                );
        }
    }



    /// <summary>
    /// Provides a TestWebRequest subclass that can handle requests in a (possibly remote) web server.
    /// </summary>
    public class RemoteWebRequest : HttpBasedWebRequest
    {
        /// <summary>Name of the service file under test, with no path information.</summary>
        private const string ServiceFileName = "service.svc";

        /// <summary>Whether the local web server has been setup for the current configuration.</summary>
        private bool remoteWebSetup;

        /// <summary>Last data type setup.</summary>
        private Type lastDataServiceType;

        /// <summary>Last test arguments setup.</summary>
        private string lastTestArguments;

        /// <summary>Target path for files.</summary>
        private string targetPath;

        /// <summary>Virtual directory name for service on web server.</summary>
        private string virtualDirectoryName;

        /// <summary>Name of server which will handle the request.</summary>
        private string webServerName = "localhost";

        /// <summary>The root of a temporary folder that has public access permissions.</summary>
        public static string PublicTemporaryPath
        {
            get
            {
                // originally "%SystemDrive%\astoriatest-wwwroot" was choosen because of the short base path length
                // however, all temporary files must be under the binaries directory.
                string result = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                result = Path.GetFullPath(Path.Combine(result, "astoriatest-wwwroot"));
                IOUtil.EnsureDirectoryExists(result);
                return result;
            }
        }

        /// <summary>Virtual directory name for service on web server.</summary>
        public string VirtualDirectoryName
        {
            get { return this.virtualDirectoryName; }
        }

        /// <summary>Name of server which will handle the request.</summary>
        public string WebServerName
        {
            get { return this.webServerName; }
            set
            {
                if (value != this.webServerName)
                {
                    if (this.remoteWebSetup)
                    {
                        throw new InvalidOperationException("Remote web is currently setup - cannot modify WebServerName.");
                    }

                    if (value != "localhost")
                    {
                        throw new ArgumentException("Only localhost is currently supported.");
                    }

                    this.webServerName = value;
                }
            }
        }

        /// <summary>Shuts down the remove web server.</summary>
        /// <param name="disposing">
        /// Whether the call is being made from an explicit call to 
        /// IDisposable.Dispose() rather than through the finalizer.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (remoteWebSetup)
            {
                this.ShutdownRemoteWeb();
            }

            base.Dispose(disposing);
        }

        /// <summary>Starts the service.</summary>
        public override void StartService()
        {
            this.SetupRemoteWeb();
        }

        /// <summary>Stops the service.</summary>
        public override void StopService()
        {
            this.ShutdownRemoteWeb();
        }

        /// <summary>
        /// Sets up the required files locally to test the web data service through the remote web server.
        /// </summary>
        private void SetupServiceFiles()
        {
            var entityConnectionStrings = new System.Collections.Generic.Dictionary<string, string>();
            entityConnectionStrings.Add("NorthwindContext", NorthwindModel.NorthwindContext.ContextConnectionString);
            AstoriaUnitTests.Tests.LocalWebServerHelper.SetupRemoteServiceFiles(
                ServiceFileName,
                this.DataServiceType,
                this.ServiceType,
                ArgumentsToString(this.TestArguments),
                entityConnectionStrings,
                this.targetPath);
            this.serviceEntryPointLocation = String.Format(
                "http://{0}/{1}/{2}", this.WebServerName, this.VirtualDirectoryName, ServiceFileName);
        }

        /// <summary>Sets up a remote web server.</summary>
        private void SetupRemoteWeb()
        {
            string currentTestArguments = ArgumentsToString(this.TestArguments);
            if (lastDataServiceType != this.DataServiceType ||
                lastTestArguments != currentTestArguments)
            {
                if (remoteWebSetup)
                {
                    this.ShutdownRemoteWeb();
                }

                this.virtualDirectoryName = "test-" + Guid.NewGuid().ToString();
                this.targetPath = Path.Combine(PublicTemporaryPath, this.virtualDirectoryName);
                IOUtil.EnsureEmptyDirectoryExists(this.targetPath);
                this.SetupServiceFiles();
                this.CreateVirtualDirectory();
                this.lastDataServiceType = this.DataServiceType;
                this.lastTestArguments = currentTestArguments;
                remoteWebSetup = true;
            }
        }

        public void ShutdownRemoteWeb()
        {
            // TODO: consider unloading the virtual directory.
            this.remoteWebSetup = false;
            this.lastDataServiceType = null;
            this.lastTestArguments = null;
        }

        /// <summary>Sends the current request to the server.</summary>
        public override void SendRequest()
        {
            this.StartService();
            base.SendRequest();
        }

        /// <summary>Creates the virtual directory on the web server for the target of this request.</summary>
        private void CreateVirtualDirectory()
        {
            int siteId = 1;
            string metabasePath = String.Format("IIS://{0}/W3SVC/{1}/Root", this.WebServerName, siteId);
            CreateVDir(metabasePath, this.virtualDirectoryName, this.targetPath);
        }

        /// <summary>Creates a virtual directory using ADSI.</summary>
        /// <param name="metabasePath">
        /// metabasePath is of the form "IIS://[servername]/[service]/[siteID]/Root[/[vdir]]"
        /// for example "IIS://localhost/W3SVC/1/Root" 
        /// </param>
        /// <param name="vDirName">Directory name in the form of [name], for example "MyNewVDir".</param>
        /// <param name="physicalPath">Full path in the form of [drive]:\[path], for example "C:\inetpub\wwwroot".</param>
        private static void CreateVDir(string metabasePath, string vDirName, string physicalPath)
        {
            string message = String.Format("Creating virtual directory {0}/{1}, mapping the Root application to {2}:",
                metabasePath, vDirName, physicalPath);
            Trace.WriteLine(message);
            using (DirectoryEntry site = new DirectoryEntry(metabasePath))
            {
                string className = site.SchemaClassName.ToString();
                if ((className.EndsWith("Server")) || (className.EndsWith("VirtualDir")))
                {
                    DirectoryEntries vdirs = site.Children;
                    DirectoryEntry newVDir = vdirs.Add(vDirName, (className.Replace("Service", "VirtualDir")));
                    newVDir.Properties["Path"][0] = physicalPath;
                    newVDir.Properties["AccessScript"][0] = true;

                    // These properties are necessary for an application to be created.
                    newVDir.Properties["AppFriendlyName"][0] = vDirName;
                    newVDir.Properties["AppIsolated"][0] = "1";
                    newVDir.Properties["AppRoot"][0] = "/LM" + metabasePath.Substring(metabasePath.IndexOf("/", ("IIS://".Length)));
                    newVDir.CommitChanges();
                }
                else
                {
                    throw new InvalidOperationException("A virtual directory can only be created in a site or virtual directory node.");
                }
            }
        }
    }

    /// <summary>
    /// Provides a TestWebRequest subclass that can handle requests in the
    /// current process space.
    /// </summary>
    public class InProcessWebRequest : TestWebRequest
    {
        #region Private fields.

        /// <summary>In-process host used for the last request.</summary>
        private TestServiceHost host;

        /// <summary>Response stream, as returned by the server.</summary>
        private Stream responseStream;

        private string serviceBaseUri = "http://host/";

        #endregion Private fields.

        #region Properties.

        /// <summary>Uri location to the service entry point.</summary>
        public override string BaseUri
        {
            get { return this.CustomServiceBaseUri; }
        }

        public string CustomServiceBaseUri
        {
            get { return this.serviceBaseUri; }
            set { this.serviceBaseUri = value; }
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
                IDataServiceHost2 host2 = this.host as IDataServiceHost2;
                if (host2 != null)
                {
                    foreach (string header in host2.ResponseHeaders.AllKeys)
                    {
                        responseHeaders[header] = host2.ResponseHeaders[header];
                    }
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
            if (BaseTestWebRequest.HostInterfaceType == typeof(IDataServiceHost))
            {
                host = new TestServiceHost(new Uri(this.BaseUri));
            }
            else
            {
                Debug.Assert(BaseTestWebRequest.HostInterfaceType == typeof(IDataServiceHost2), "HostInterfaceType must be either IDataServiceHost or IDataServiceHost2");
                host = new TestServiceHost2(new Uri(this.BaseUri));
            }

            host.RequestHttpMethod = this.HttpMethod;
            host.RequestPathInfo = this.RequestUriString;
            host.RequestStream = this.RequestStream;

            IDataServiceHost2 host2 = host as IDataServiceHost2;
            if (host2 != null)
            {
                foreach (var header in this.RequestHeaders)
                {
                    host2.RequestHeaders.Add(header.Key, header.Value);
                }
            }
            else
            {
                host.RequestAcceptCharSet = this.AcceptCharset;
                host.RequestContentType = this.RequestContentType;
                host.RequestContentLength = this.RequestContentLength;
                host.RequestIfMatch = this.IfMatch;
                host.RequestIfNoneMatch = this.IfNoneMatch;
                host.RequestMaxVersion = this.RequestMaxVersion;
                host.RequestVersion = this.RequestVersion;

                if (this.Accept != null)
                {
                    host.RequestAccept = this.Accept;
                }
            }

            this.SendRequest(host);
        }

        public void SendRequest(IDataServiceHost host)
        {
            // If we're changing the value of verbose errors, we'll need to clear out the metadata cache, because
            // the configuration information is also stored there.
            if (OpenWebDataServiceHelper.ForceVerboseErrors != this.ForceVerboseErrors)
            {
                TestUtil.ClearConfiguration();
            }

            OpenWebDataServiceHelper.ForceVerboseErrors = this.ForceVerboseErrors;

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
                        if (TestWebRequest.SaveOriginalStackTrace &&
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
                        this.CheckResponseHeaders();
                    }
                }
            }
            finally
            {
                LoadSerializedTestArguments(null);
            }
        }

        private static Stream GetResponseStream(IDataServiceHost host)
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
    /// Provides a TestWebRequest subclass that can handle requests through
    /// HTTP requests.
    /// </summary>
    public class HttpBasedWebRequest : TestWebRequest
    {
        #region Fields.

        /// <summary>Uri location to the service entry point.</summary>
        protected string serviceEntryPointLocation;

        /// <summary>Full request URI string, based on RequestUriString, including protocol and host name.</summary>
        private string fullRequestUri;

        /// <summary>Response to the last web server request sent.</summary>
        private System.Net.WebResponse response;

        /// <summary>Response stream to the last web server request sent.</summary>
        private Stream responseStream;

        #endregion Fields.

        #region Properties.

        /// <summary>Uri location to the service entry point.</summary>
        public override string BaseUri
        {
            get
            {
                return this.serviceEntryPointLocation;
            }
        }

        /// <summary>Gets response headers for this request.</summary>
        public override Dictionary<string, string> ResponseHeaders
        {
            get
            {
                Dictionary<string, string> responseHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                WebHeaderCollection headers = this.response.Headers;
                foreach (string header in headers.AllKeys)
                {
                    responseHeaders[header] = string.Join(",", headers.GetValues(header));
                }

                return responseHeaders;
            }
        }

        /// <summary>Cache-Control header in response.</summary>
        public override string ResponseCacheControl
        {
            get { return this.response.Headers[HttpResponseHeader.CacheControl]; }
        }

        /// <summary>Location header for response.</summary>
        public override string ResponseLocation
        {
            get { return this.response.Headers[HttpResponseHeader.Location]; }
        }

        /// <summary>Value for the OData-Version response header.</summary>
        public override string ResponseVersion
        {
            get { return this.response.Headers["OData-Version"]; }
        }

        /// <summary>ContentType header for response.</summary>
        public override string ResponseContentType
        {
            [DebuggerStepThrough]
            get
            {
                return response.ContentType;
            }
        }

        public override string ResponseETag
        {
            get { return this.response.Headers[HttpResponseHeader.ETag]; }
        }

        public override int ResponseStatusCode
        {
            get { return (int)((HttpWebResponse)this.response).StatusCode; }
        }

        /// <summary>Full request URI string, based on RequestUriString, including protocol and host name.</summary>
        public override string FullRequestUriString
        {
            get
            {
                if (this.fullRequestUri == null)
                {
                    if (serviceEntryPointLocation == null)
                    {
                        this.StartService();
                    }

                    if (this.RequestUriString.StartsWith(serviceEntryPointLocation))
                    {
                        return this.RequestUriString;
                    }
                    else
                    {
                        return serviceEntryPointLocation + this.RequestUriString;
                    }
                }
                else
                {
                    return this.fullRequestUri;
                }
            }
            set
            {
                if (value.StartsWith(serviceEntryPointLocation))
                {
                    this.RequestUriString = value.Substring(serviceEntryPointLocation.Length);
                }
                else
                {
                    this.RequestUriString = null;
                }

                this.fullRequestUri = value;
            }
        }

        /// <summary>Gets or sets the original URI of the request.</summary>
        public override string RequestUriString
        {
            [DebuggerStepThrough]
            get { return base.RequestUriString; }
            set
            {
                base.RequestUriString = value;
                this.fullRequestUri = null;
            }
        }

        #endregion properties.

        #region Methods.

        /// <summary>Gets the response stream, as returned by the server.</summary>
        public override Stream GetResponseStream()
        {
            return responseStream;
        }

        /// <summary>
        /// Creates a WebRequest object.
        /// </summary>
        /// <param name="fullUri">Uri to service.</param>
        /// <returns>WebRequest object</returns>
        protected virtual HttpWebRequest CreateWebRequest(string fullUri)
        {
            return (HttpWebRequest)System.Net.WebRequest.Create(fullUri);
        }

        /// <summary>Sends the current request to the server.</summary>
        public override void SendRequest()
        {
            this.ErrorResponseContent = string.Empty;
            if (this.responseStream != null)
            {
                this.responseStream.Close();
                this.responseStream = null;
            }

            if (this.response != null)
            {
                this.response.Close();
                this.response = null;
            }

            string fullUri = this.FullRequestUriString;
            Debug.WriteLine("Sending request to: " + fullUri);
            System.Net.HttpWebRequest request = CreateWebRequest(fullUri);
            request.Timeout = (int)TimeSpan.FromMinutes(5).TotalMilliseconds; // 5 Minutes timeout
            request.Method = this.HttpMethod;
            if (request.Method == "GET")
            {
                // Content-Length or Chunked Encoding cannot be set for an operation that does not write data.
                request.SendChunked = false;
            }

            if (this.RequestContentLength != -1)
            {
                request.ContentLength = this.RequestContentLength;
            }
            if (this.RequestContentType != null)
            {
                request.ContentType = this.RequestContentType;
            }
            if (this.Accept != null)
            {
                request.Accept = this.Accept;
            }

            foreach (var p in this.RequestHeaders)
            {
                if (p.Value != null && p.Key != "Content-Length" && p.Key != "Accept" && p.Key != "Content-Type")
                {
                    request.Headers[p.Key] = p.Value;
                }
            }

            if (this.RequestStream != null)
            {
                using (Stream requestStream = request.GetRequestStream())
                {
                    TestUtil.CopyStream(this.RequestStream, requestStream);
                    if (this.RequestStream.CanSeek)
                    {
                        this.RequestStream.Position = 0;
                    }
                }
            }

            // An important side-effect of the following line of code is to
            // clean the TestArguments static for tests which don't use it.
            BaseTestWebRequest.LoadSerializedTestArguments(ArgumentsToString(this.TestArguments));

            try
            {
                this.response = request.GetResponse();
                this.responseStream = this.response.GetResponseStream();
            }
            catch (WebException webException)
            {
                Trace.WriteLine("WebException: " + webException.Message);
                System.Net.WebResponse exceptionResponse = webException.Response;
                if (exceptionResponse == null)
                {
                    Trace.WriteLine("  Response: null");
                }
                else
                {
                    this.response = exceptionResponse;
                    this.responseStream = TestUtil.EnsureStreamWithSeek(this.response.GetResponseStream());
                    this.ErrorResponseContent = new StreamReader(this.responseStream).ReadToEnd();
                    Trace.WriteLine("  Response: " + this.ErrorResponseContent);
                    this.responseStream.Position = 0;
                }
                throw;
            }
            finally
            {
                this.CheckResponseHeaders();
                BaseTestWebRequest.LoadSerializedTestArguments(null);
            }
        }

        #endregion Methods.
    }

    /// <summary>
    /// Provides a TestWebRequest subclass that can handle requests in the
    /// current process space through a WCF host.
    /// </summary>
    public class InProcessWcfWebRequest : HttpBasedWebRequest
    {
        /// <summary>WCF host.</summary>
        private DataServiceHost host;

        public Dictionary<string, object> IncomingMessageProperties
        {
            set;
            get;
        }

        /// <summary>Closes the WCF port.</summary>
        /// <param name="disposing">
        /// Whether the call is being made from an explicit call to 
        /// IDisposable.Dispose() rather than through the finalizer.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            this.StopService();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Setups up a new WebHttpBinding.
        /// </summary>
        /// <returns>an instance of WebHttpBinding.</returns>
        protected virtual WebHttpBinding SetupBinding()
        {
            return new WebHttpBinding() { MaxReceivedMessageSize = (long)2 * 1024 * 1024 };
        }

        /// <summary>Sets up the WCF host.</summary>
        private void SetupHost()
        {
            if (this.ServiceType == null)
            {
                throw new InvalidOperationException("DataServiceType or ServiceType property should be set for in-process requests.");
            }

            //If debugger is attached use real host name instead of "localhost" to allow a proxy intercepting traffic (i.e. fiddler can be used for debugging messages) 
            string baseAddress = string.Format("http://{0}:{{port}}/TheTest", Debugger.IsAttached ? Dns.GetHostName().ToLower() : "localhost");
            this.host = UnitTestsUtil.StartHostProcess(this.ServiceType, baseAddress, this.BeforeOpening);
            this.serviceEntryPointLocation = this.host.BaseAddresses[0].AbsoluteUri;
        }

        private void BeforeOpening(DataServiceHost host1)
        {
            TestUtil.EnableServiceDebugBehavior(host1);
            host1.Description.Name = this.ServiceType.Name.Replace('`', '_');
            Type implementedContract = (this.NotDataServiceOfT ? this.ServiceType : typeof(IRequestHandler));
            System.ServiceModel.Description.ServiceEndpoint endpoint = host1.AddServiceEndpoint(implementedContract, SetupBinding(), "");
            endpoint.Behaviors.Add(new MessagePropertiesBehavior(this));

            new DataServiceHostListen(host1);
        }

        private sealed class DataServiceHostListen
        {
            private ServiceHostBase host;
            internal DataServiceHostListen(ServiceHostBase host)
            {
                this.host = host;
                host.Opening += Listen;
                host.Opened += Listen;
                host.Closing += Listen;
                host.Closed += Listen;
                host.Faulted += Listen;
                host.UnknownMessageReceived += Listen;
            }

            private void Listen<T>(object sender, T args)
            {
                string message = String.Format("WebServiceHost {0}: {1}", this.host.State, this.host.BaseAddresses.FirstOrDefault());
                Trace.WriteLine(message);
            }
        }

        /// <summary>Sends the current request to the server.</summary>
        public override void SendRequest()
        {
            this.StartService();
            base.SendRequest();
        }

        /// <summary>Starts the service.</summary>
        public override void StartService()
        {
            if (this.host == null)
            {
                this.SetupHost();
            }
        }

        /// <summary>Stops the service.</summary>
        public override void StopService()
        {
            if (this.host != null)
            {
                if (this.host.State == System.ServiceModel.CommunicationState.Opened)
                {
                    this.host.Close();
                }

                this.host = null;
            }
        }

        private class MessagePropertiesInspector : IDispatchMessageInspector
        {
            private InProcessWcfWebRequest httpRequest;

            public MessagePropertiesInspector(InProcessWcfWebRequest request)
            {
                this.httpRequest = request;
            }

            #region IDispatchMessageInspector Members

            public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel, InstanceContext instanceContext)
            {
                if (this.httpRequest.IncomingMessageProperties != null)
                {
                    foreach (KeyValuePair<string, object> property in this.httpRequest.IncomingMessageProperties)
                    {
                        OperationContext.Current.IncomingMessageProperties.Add(property.Key, property.Value);
                    }
                }

                return null;
            }

            public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
            {
            }

            #endregion
        }

        private class MessagePropertiesBehavior : IEndpointBehavior
        {
            private InProcessWcfWebRequest httpRequest;

            public MessagePropertiesBehavior(InProcessWcfWebRequest request)
            {
                this.httpRequest = request;
            }

            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
                throw new Exception("Behavior not supported on the consumer side!");
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                MessagePropertiesInspector inspector = new MessagePropertiesInspector(this.httpRequest);
                endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }
        }
    }

    public class InProcessStreamedWcfWebRequest : InProcessWcfWebRequest
    {
        /// <summary>
        /// Setups up a new WebHttpBinding.
        /// </summary>
        /// <returns>an instance of WebHttpBinding.</returns>
        protected override WebHttpBinding SetupBinding()
        {
            WebHttpBinding binding = new WebHttpBinding();
            binding.TransferMode = TransferMode.Streamed;
            long oneGB = 1024 * 1024 * 1024;
            binding.MaxReceivedMessageSize = 1024 * oneGB; // 1 TB
            binding.SendTimeout = TimeSpan.FromMinutes(5); // timeout: 5 minutes - should be enough for streaming up to 6GB of data
            return binding;
        }

        /// <summary>
        /// Creates a WebRequest object.
        /// </summary>
        /// <param name="fullUri">Uri to service.</param>
        /// <returns>WebRequest object</returns>
        protected override HttpWebRequest CreateWebRequest(string fullUri)
        {
            HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(fullUri);
            request.AllowWriteStreamBuffering = false;
            request.SendChunked = true;
            request.KeepAlive = true;
            request.Timeout = 60 * 60 * 1000;
            request.ReadWriteTimeout = 60 * 60 * 1000;
            return request;
        }
    }
}
