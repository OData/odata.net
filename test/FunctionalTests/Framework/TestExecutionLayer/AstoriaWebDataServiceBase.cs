//---------------------------------------------------------------------
// <copyright file="AstoriaWebDataServiceBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Reflection;
using Microsoft.OData.Client;
using System.Xml;
using Microsoft.Test.ModuleCore;
using System.Data.Test.Astoria.Util;

namespace System.Data.Test.Astoria.TestExecutionLayer
{
    [DnsPermission(Security.Permissions.SecurityAction.Assert, Unrestricted = true)]
    public abstract class AstoriaWebDataService : IDisposable
    {
        protected const string databaseFolder = @"workspaces\databases\";

        #region uri-logging for debugging purposes
        private const string UriListFileName = "URIs.txt";
        private const string UriListFileDirectory = @"\inetpub\wwwroot";
        private static readonly string UriListFilePath = Path.Combine(Environment.GetEnvironmentVariable("SystemDrive") + UriListFileDirectory, UriListFileName);
        private static void WriteUriToList(string uri)
        {
            using (StreamWriter writer = new StreamWriter(new FileStream(UriListFilePath, FileMode.OpenOrCreate)))
            {
                writer.WriteLine(uri);
            }
        }
        #endregion

        private ConfigurationSettings _configSettings;

        public static AstoriaWebDataService CreateAstoriaDataWebService(Workspace workspace)
        {
            return AstoriaWebDataService.CreateAstoriaDataWebService(workspace, workspace.WebServiceName, workspace.Database, workspace.Settings.SkipWorkspaceCheck);
        }
        private static string GetMachineName()
        {
            string machineName = AstoriaTestProperties.HostMachineName;
            if (machineName == null)
            {
                if (AstoriaTestProperties.IsLocalHost)
                    machineName = Environment.MachineName;
                AstoriaTestProperties.HostMachineName = machineName;
            }
            return machineName;
        }
        public static AstoriaWebDataService CreateAstoriaDataWebService(Workspace workspace, string webDataServicePrefixName, AstoriaDatabase database, bool skipDataServiceVerify)
        {
            AstoriaWebDataService service = null;
            //Base the creation on the AstoriaTestProperties
            switch (AstoriaTestProperties.Host)
            {
                case Host.WebServiceHost:
                case Host.IDSH:
                case Host.IDSH2:
                    service = new AstoriaServiceHost(workspace, webDataServicePrefixName, Environment.MachineName, database);
                    break;

                case Host.WebServiceHostRemote:
                    service = new AstoriaServiceHost(workspace, webDataServicePrefixName, GetMachineName(), database);
                    break;

                default:
                    service = new IISWebDataService(workspace, webDataServicePrefixName, GetMachineName(), database);
                    break;
            }

            // write out helpful debugging URIs
            if (AstoriaTestProperties.Client == ClientEnum.SILVERLIGHT)
            {
                WriteUriToList(service.ServiceRootUri + "Monitor.aspx");
                WriteUriToList(service.ServiceRootUri + "SilverlightAstoriaTestPage.html");
                WriteUriToList(service.ServiceUri);
            }

            // if running locally in partial trust, ensure that the trusted methods are available
            if (AstoriaTestProperties.IsLocalHost
                && AstoriaTestProperties.ServiceTrustLevel != TrustLevel.Full
                && AstoriaTestProperties.RuntimeEnvironment == TestRuntimeEnvironment.Lab)
            {
                Assembly trustedAssembly = typeof(FullTrust.TrustedMethods).Assembly;
                //if (!trustedAssembly.GlobalAssemblyCache)
                //{
                //    AstoriaTestLog.FailAndThrow("Assembly containing fully trusted components must be in the GAC. " + 
                //        "Run 'gacutil -if <dll>' on AstoriaTestFramework.FullTrust from a visual studio command prompt");
                //}

                if (!trustedAssembly.IsFullyTrusted)
                {
                    AstoriaTestLog.FailAndThrow("Assembly containing components requiring full trust is not trusted");
                }
            }

            try
            {
                // push the verification step down to the derived classes so that a failure there can be handled accordingly
                //
                service.CreateWebService(!skipDataServiceVerify);
            }
            catch (Exception e)
            {
                // we need to make sure that we don't 'leak' services that fail to start, as it can mean a process is left running that will
                // cause subsequent ArtClean's to fail. This has caused build failures at least once.
                //
                if (AstoriaTestProperties.IsLabRun || AstoriaTestProperties.RuntimeEnvironment == TestRuntimeEnvironment.CheckinSuites)
                {
                    service.Dispose();
                }
                throw new TestFailedException("Failed to create web service", null, null, e);
            }

            if (skipDataServiceVerify)
            {
                // for call logging, we need the directory to exist, and the service doesn't necessarily have permission to create it
                // TODO: find some better, common place to put this for IIS, WebServiceHost, etc. Right now this is the best spot
#if !ClientSKUFramework
                IOUtil.EnsureEmptyDirectoryExists(Path.Combine(service.DestinationFolder, CallOrder.APICallLog.DirectoryName));
#endif

                Thread.Sleep(5000);     //To allow time for the service to start running before requests get made
            }

            return service;
        }

        public AstoriaWebDataService(Workspace workspace, string webDataServicePrefixName, string machineName, AstoriaDatabase database)
        {
            Workspace = workspace;
            if (String.IsNullOrEmpty(webDataServicePrefixName))
                throw new ArgumentException("String.IsNullOrEmpty(WebDataServicePrefixName)", webDataServicePrefixName);

            WebDataServicePrefixName = webDataServicePrefixName;
            WebDataServiceName = String.Format("{0}_{1}_{2}", WebDataServicePrefixName, System.Net.Dns.GetHostName(), Guid.NewGuid().ToString("N"));
            MachineName = machineName;
            Database = database;
        }

        public AstoriaWebDataService(Workspace workspace, string webDataServicePrefixName, AstoriaDatabase database)
            : this(workspace, webDataServicePrefixName, "localhost", database)
        {
        }

        public Workspace Workspace
        {
            get;
            private set;
        }

        private string _serviceUri = null;
        public virtual string ServiceUri
        {
            get
            {
                return _serviceUri;
            }
            protected set
            {
                // make sure that the base uri's are valid (this has the useful side-effect of making the machine name lowercase)
                _serviceUri = value;
                if (_serviceUri != null)
                    _serviceUri = new Uri(_serviceUri).AbsoluteUri;
            }
        }

        private string _serviceRootUri = null;
        public virtual string ServiceRootUri
        {
            get
            {
                return _serviceRootUri;
            }
            protected set
            {
                // make sure that the base uri's are valid (this has the useful side-effect of making the machine name lowercase)
                _serviceRootUri = value;
                if (_serviceRootUri != null)
                    _serviceRootUri = new Uri(_serviceRootUri).AbsoluteUri;
            }
        }

        public string MachineName
        {
            get;
            protected set;
        }

        public virtual string DestinationFolder
        {
            get;
            protected set;
        }

        public virtual string DestinationFolder_Local
        {
            get;
            protected set;
        }

        public string SourceFolder
        {
            get;
            protected set;
        }

        public string WebDataServicePrefixName
        {
            get;
            private set;
        }

        public AstoriaDatabase Database
        {
            get;
            private set;
        }

        public string WebDataServiceName
        {
            get;
            private set;
        }

        protected abstract void CreateWebService(bool verify);

        public void VerifyService()
        {
            // for call logging, we need the directory to exist, and the service doesn't necessarily have permission to create it
            // TODO: find some better, common place to put this for IIS, WebServiceHost, etc. Right now this is the best spot
#if !ClientSKUFramework
            IOUtil.EnsureEmptyDirectoryExists(Path.Combine(DestinationFolder, CallOrder.APICallLog.DirectoryName));
#endif

            const int retryCount = 10;
            const int sleepTime = 6000;

            AstoriaTestLog.WriteLineIgnore("Verifying web service: " + this.ServiceUri);

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            WebException webException = null;

            for (int count = 0; count < retryCount; count++)
            {
                try
                {
                    request = (HttpWebRequest)HttpWebRequest.Create(this.ServiceUri + "/$metadata");
                    request.UseDefaultCredentials = true;
                    response = (HttpWebResponse)request.GetResponse();
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        reader.ReadToEnd();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        AstoriaTestLog.WriteLineIgnore("Service verified.");
                        return;
                    }
                    else
                    {
                        // can this happen without throwing?
                        AstoriaTestLog.WriteLine("\tUnexpected status code: " + response.StatusCode);
                    }
                }
                catch (Exception e)
                {
                    string indent = "\t";
                    while (e != null)
                    {
                        if (e is WebException)
                            webException = e as WebException;

                        AstoriaTestLog.WriteLine(indent + e.GetType().Name + ": " + e.Message);
                        indent += "\t";
                        e = e.InnerException;
                    }
                }
                Thread.Sleep(sleepTime);
            }

            if (webException != null)
            {
                AstoriaTestLog.TraceLine("Web exception:");
                AstoriaTestLog.TraceLine(webException.ToString());
                if (webException.Response != null)
                {
                    string exceptionPayload;
                    using (StreamReader reader = new StreamReader(webException.Response.GetResponseStream()))
                        exceptionPayload = reader.ReadToEnd();
                    AstoriaTestLog.TraceLine("Exception Payload:");
                    AstoriaTestLog.TraceLine(exceptionPayload);
                }
            }

            AstoriaTestLog.FailAndThrow("Service could not be verified.");
        }

        protected void CopySourceFolderToHost()
        {
            IOUtil.CopyFolder(SourceFolder, DestinationFolder, true);

            CopyServerWorkspaceFiles(Path.Combine(DestinationFolder, "App_Code"));

            if (this.Workspace.ServiceModifications != null)
                this.Workspace.ServiceModifications.ApplyChanges(DestinationFolder);

            // copy any required files to the bin directory
            string binFolder = Path.Combine(DestinationFolder, "Bin");

            var assembliesToCopy = new List<Assembly>
            {
                typeof(FullTrust.TrustedMethods).Assembly, // this is a special case for the fully-trusted-methods assembly, which must always be copied
                typeof(Microsoft.Spatial.ISpatial).Assembly,
                typeof(Microsoft.OData.Edm.IEdmModel).Assembly,
                typeof(Microsoft.OData.ODataException).Assembly,
                typeof(Microsoft.OData.Client.DataServiceContext).Assembly,
#if !ClientSKUFramework
                typeof(Microsoft.OData.Service.DataService<>).Assembly,
#endif
            };

            foreach (var assembly in assembliesToCopy)
            {
                File.Copy(assembly.Location, Path.Combine(binFolder, Path.GetFileName(assembly.Location)));
            }
        }

        protected internal virtual void CopyServerWorkspaceFiles(string folder)
        {
            // Copy all embedded resource files to web service source code folder.
            Assembly testFrameworkAssembly = Assembly.ReflectionOnlyLoadFrom("AstoriaServerTestFramework.dll");
            foreach (string name in Workspace.RequiredFrameworkSources)
            {
                IOUtil.FindAndWriteResourceToFile(testFrameworkAssembly, name, Path.Combine(folder, name));
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {

        }
        #endregion

        public ConfigurationSettings ConfigSettings
        {
            get
            {
                if (_configSettings == null)
                    _configSettings = new ConfigurationSettings(this);
                return _configSettings;
            }
        }
    }

    public class ConfigurationSettings
    {
        private AstoriaWebDataService _dataService;
        private ODataProtocolVersion? maxProtocolVersion;

        public ConfigurationSettings(AstoriaWebDataService dataService)
        {
            _dataService = dataService;
            this.maxProtocolVersion = dataService.Workspace.Settings.MaxProtocolVersion;
        }

#if !ClientSKUFramework
        private Dictionary<string, Microsoft.OData.Service.EntitySetRights> entitySetRightsCache = new Dictionary<string, Microsoft.OData.Service.EntitySetRights>();
#endif

        private void Call(string uri)
        {
            Call(uri, null);
        }

        private void Call(string uri, WebHeaderCollection headers)
        {
            AstoriaRequest request = _dataService.Workspace.CreateRequest();

            request.URI = _dataService.ServiceUri + "/" + uri;
            request.ExpectedStatusCode = HttpStatusCode.NoContent;
            request.Accept = "*/*";

            if (headers != null)
            {
                foreach (string header in headers.AllKeys)
                {
                    request.Headers[header] = headers[header];
                }
            }

            // don't verify the status code, as this might be intentionally putting the service into an invalid state
            AstoriaResponse response = request.GetResponse();
        }

        private string Retrieve(string url)
        {
            string response;
            RequestUtil.GetAndVerifyStatusCode(_dataService.Workspace, _dataService.ServiceUri + "/" + url, HttpStatusCode.OK, out response);
            return response;
        }

        public void ClearAll()
        {
            WebHeaderCollection webHeaders = new WebHeaderCollection();
            webHeaders.Add("ClearAllSettings", "true");
            Call("ClearAll", webHeaders);
        }

        public int MaxBatchCount
        {
            set { Call("MaxBatchCount?count=" + value.ToString()); }
        }

        public int MaxChangesetCount
        {
            set { Call("MaxChangesetCount?count=" + value.ToString()); }
        }

        public int MaxExpandCount
        {
            set { Call("MaxExpandCount?count=" + value.ToString()); }
        }

        public int MaxExpandDepth
        {
            set { Call("MaxExpandDepth?count=" + value.ToString()); }
        }

        public int MaxObjectCountOnInsert
        {
            set { Call("MaxObjectCountOnInsert?count=" + value.ToString()); }
        }

        public int MaxResultsPerCollection
        {
            set { Call("MaxResultsPerCollection?count=" + value.ToString()); }
        }

        public ODataProtocolVersion? MaxProtocolVersion
        {
            get
            {
                return this.maxProtocolVersion;
            }

            set 
            {
                if (this.maxProtocolVersion != value)
                {
                    this.maxProtocolVersion = value;

                    var headers = new WebHeaderCollection();
                    if (value.HasValue)
                    {
                        headers["OverrideServiceConfig-MaxProtocolVersion"] = Convert.ToString((int?)value);
                        Call("MaxProtocolVersion?value=" + ((int)value).ToString(), headers);
                    }
                    else
                    {
                        headers["OverrideServiceConfig-MaxProtocolVersion"] = Convert.ToString((int?)null);
                        Call("MaxProtocolVersion?value=null", headers);
                    }
                }
            }
        }

        public bool UseVerboseErrors
        {
            set { Call("UseVerboseErrors?value=" + value.ToString().ToLower()); }
        }

        public void AddRegisterKnownType(string typeName)
        {
            Call("AddRegisterKnownType?typeName='" + typeName + "'");
        }

        public void ClearRegisteredKnownTypes()
        {
            Call("ClearRegisteredKnownTypes");
        }

        public bool AllowCountRequests
        {
            set
            {
                Call(String.Format("AllowCountRequests?bValue={0}", value.ToString().ToLower()));
            }
        }

        public bool AllowProjectionRequests
        {
            set
            {
                Call(String.Format("AllowProjectionRequests?bValue={0}", value.ToString().ToLower()));
            }
        }

        public bool InvokeInterceptorsOnLinkDelete
        {
            set
            {
                Call(String.Format("InvokeInterceptorsOnLinkDelete?bValue={0}", value.ToString().ToLower()));
            }
        }

        public int GetEntitySetPageSize(string entityName)
        {
            string payload = Retrieve("GetEntitySetPageSize?entityName='" + entityName + "'");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(payload);

            XmlNode node = xmlDoc.FirstChild.NextSibling;

            return Int32.Parse(node.InnerText);
        }

        public void SetEntitySetPageSize(string entityName, int pageSize)
        {
            Call(String.Format("SetEntitySetPageSize?entityName='{0}'&pageSize={1}", entityName, pageSize.ToString()));
        }

        public void SetEnableTypeConversion(bool typeConversionEnabled)
        {
            Call(String.Format("SetEnableTypeConversion?typeConversionEnabled={0}", typeConversionEnabled.ToString().ToLower()));
        }

        public void ClearEntitySetPageSizes()
        {
            Call("ClearEntitySetPageSizes");
        }

#if !ClientSKUFramework

        public Microsoft.OData.Service.EntitySetRights GetEntitySetAccessRule(string entityName)
        {
            Microsoft.OData.Service.EntitySetRights rights;
            if (!entitySetRightsCache.TryGetValue(entityName, out rights))
            {
                string payload = Retrieve("GetEntitySetAccessRule?entityName='" + entityName + "'");

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(payload);

                XmlNode node = xmlDoc.FirstChild.NextSibling;

                rights = (Microsoft.OData.Service.EntitySetRights)Int32.Parse(node.InnerText);
                entitySetRightsCache[entityName] = rights;
            }
            return rights;
        }

        public void SetEntitySetAccessRule(string entityName, Microsoft.OData.Service.EntitySetRights entitySetRight)
        {
            entitySetRightsCache[entityName] = entitySetRight;
            Call(String.Format("SetEntitySetAccessRule?entityName='{0}'&entitySetRights={1}", entityName, ((int)entitySetRight).ToString()));
        }

        public void ClearEntitySetAccessRules()
        {
            Call("ClearEntitySetAccessRules");
        }

        public Microsoft.OData.Service.ServiceOperationRights GetServiceOperationAccessRule(string entityName)
        {
            string payload = Retrieve("GetServiceOperationAccessRule?entityName='" + entityName + "'");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(payload);

            XmlNode node = xmlDoc.FirstChild.NextSibling;

            return (Microsoft.OData.Service.ServiceOperationRights)Int32.Parse(node.InnerText);
        }


        public void SetServiceOperationAccessRule(string serviceOperationName, Microsoft.OData.Service.ServiceOperationRights serviceOperationRight)
        {
            Call(String.Format("SetServiceOperationAccessRule?serviceOperationName='{0}'&serviceOperationSetRights={1}", serviceOperationName, ((int)serviceOperationRight).ToString()));
        }


#endif

        public void ClearServiceOperationAccessRules()
        {
            Call("ClearServiceOperationAccessRules");
        }

        public void EnableTypeAccess(string typeName)
        {
            Call("EnableTypeAccess?typename='" + typeName + "'");
        }

        public void ClearEnabledTypes()
        {
            Call("ClearEnabledTypes");
        }
    }
}
