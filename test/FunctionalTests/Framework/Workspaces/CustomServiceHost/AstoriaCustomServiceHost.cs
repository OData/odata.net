//---------------------------------------------------------------------
// <copyright file="AstoriaCustomServiceHost.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Data.Test.Astoria.TestExecutionLayer;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text; //Assembly

namespace System.Data.Test.Astoria
{
    public class AstoriaCustomServiceHost : AstoriaWebDataService
    {
        private readonly string _serviceHostFolder = Environment.GetEnvironmentVariable("SystemDrive") + "\\CustomServiceHost";
        private const string _serviceName = "CustomTestServiceHost";

        private AstoriaCustomServiceHost(Workspace workspace, string webDataServicePrefixName, AstoriaDatabase database)
            : base(workspace, webDataServicePrefixName, database)
        {

        }

        public static AstoriaCustomServiceHost GetCustomHost(Workspace workspace)
        {
            if (workspace.Database == null && (workspace.DataLayerProviderKind == DataLayerProviderKind.Edm || workspace.DataLayerProviderKind == DataLayerProviderKind.LinqToSql))
                workspace.Database = new TestExecutionLayer.AstoriaDatabase(workspace.Name);

            AstoriaCustomServiceHost host = new AstoriaCustomServiceHost(workspace, workspace.WebServiceName, workspace.Database);
            workspace.DataService = host;

            host.CreateWebService(false);

            return host;
        }

        protected override void CreateWebService(bool verify)
        {
            DestinationFolder = _serviceHostFolder + this.WebDataServicePrefixName;
            DestinationFolder_Local = DestinationFolder;
            string dllName = _serviceName + this.WebDataServicePrefixName;

            CopyServiceFiles();
            CreateServiceHostConfigFile(dllName);
            CompileServiceHostExe(DestinationFolder, dllName);

            // don't need these, apparently
            ServiceRootUri = null;
            ServiceUri = null;

            if (verify)
                VerifyService();
        }

        protected internal void CreateServiceHostConfigFile(string dllName)
        {
            StreamWriter writer = File.CreateText(Path.Combine(DestinationFolder, dllName + ".dll.config"));

            string contextTypeName = this.Workspace.ContextTypeName;

            string authMethod = AstoriaTestProperties.HostAuthenicationMethod;

            string text = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" +
                            "<configuration>\r\n" +
                            " <system.serviceModel> \r\n" +
"    <services>\r\n" +
"      <service name=\"TestServiceHost.NorthwindService\">\r\n" +
"        <endpoint binding=\"webHttpBinding\" bindingConfiguration=\"higherMessageSize\"\r\n" +
"         contract =\"Microsoft.OData.Service.IRequestHandler\">\r\n" +
"        </endpoint>\r\n" +
"      </service>\r\n" +
"      <service name=\"TestServiceHost.ArubaService\">\r\n" +
"        <endpoint binding=\"webHttpBinding\" bindingConfiguration=\"higherMessageSize\"\r\n" +
"         contract =\"Microsoft.OData.Service.IRequestHandler\">\r\n" +
"        </endpoint>\r\n" +
"      </service>\r\n" +
"    </services>\r\n" +
"    <bindings>\r\n" +
"      <webHttpBinding>\r\n" +
"        <binding name=\"higherMessageSize\" maxReceivedMessageSize =\"2147483647\">\r\n" +
"          <security mode=\"TransportCredentialOnly\">\r\n" +
"            <transport clientCredentialType=\"" + authMethod + "\" proxyCredentialType=\"" + authMethod + "\"/>\r\n" +
"          </security>\r\n" +
"        </binding>\r\n" +
"      </webHttpBinding>\r\n" +
"    </bindings>\r\n" +
"    <serviceHostingEnvironment aspNetCompatibilityEnabled=\"true\"/>\r\n" +
"</system.serviceModel>\r\n" +
                            "</configuration>\r\n";

            writer.WriteLine(text);
            writer.Close();
        }


        public BaseTestWebRequest GetTestWebRequestObject()
        {
            Assembly resourceAssembly = Assembly.LoadFrom(Path.Combine(DestinationFolder, _serviceName + this.WebDataServicePrefixName) + ".dll");

            BaseTestWebRequest request = BaseTestWebRequest.CreateForInProcess();
            request.ServiceType = resourceAssembly.GetType(_serviceName + "." + Workspace.ServiceClassName);
            request.StartService();

            return request;
        }

        public TestHttpListenerWebRequest GetHttpListenerWebRequestObject()
        {
            Assembly resourceAssembly = Assembly.LoadFrom(Path.Combine(DestinationFolder, _serviceName + this.WebDataServicePrefixName) + ".dll");

            BaseTestWebRequest request = BaseTestWebRequest.CreateForHttpListener();
            request.ServiceType = resourceAssembly.GetType(_serviceName + "." + Workspace.ServiceClassName);
            request.StartService();

            return (TestHttpListenerWebRequest)request;
        }

        private void CopyServiceFiles()
        {
            Assembly resourceAssembly = this.GetType().Assembly;

            IOUtil.EnsureDirectoryExists(DestinationFolder);
            IOUtil.EmptyDirectoryRecusively(DestinationFolder);

            string serviceSourceCodePath = Path.Combine(DestinationFolder, _serviceName + ".cs");

            StringBuilder serviceSource = new StringBuilder();
            serviceSource.AppendLine(this.Workspace.BuildDataServiceClassUsings());
            serviceSource.AppendLine("namespace CustomTestServiceHost");
            serviceSource.AppendLine("{");
            serviceSource.AppendLine(this.Workspace.BuildDataServiceClassCode());
            serviceSource.AppendLine("}");

            File.WriteAllText(serviceSourceCodePath, serviceSource.ToString());

            this.Workspace.PopulateHostSourceFolder();

            // copy additional files
            foreach (string file in this.Workspace.ServiceHostFiles())
            {
                File.Copy(file, Path.Combine(DestinationFolder, Path.GetFileName(file)));

                if (file.Contains("Edm.ObjectLayer"))
                {
                    string copiedFile = Path.Combine(DestinationFolder, Path.GetFileName(file));
                    string fileText = File.ReadAllText(copiedFile);

                    string pathAndName = Path.Combine(DestinationFolder, this.Workspace.Name);

                    System.Data.EntityClient.EntityConnectionStringBuilder connBuilder = new System.Data.EntityClient.EntityConnectionStringBuilder();
                    connBuilder.ProviderConnectionString = this.Workspace.Database.DatabaseConnectionString;
                    connBuilder.Metadata = pathAndName + ".csdl|" + pathAndName + ".ssdl|" + pathAndName + ".msl";
                    connBuilder.Provider = "System.Data.SqlClient";

                    string connString = connBuilder.ToString();
                    connString = connString.Replace(@"\", @"\\");
                    connString = connString.Replace("\"", "\\\"");

                    string modifiedText = fileText.Replace("name=" + this.Workspace.ContextTypeName, connString);

                    File.WriteAllText(copiedFile, modifiedText);
                }
            }

#if false // pulled from GAC - not local filesystem
            // Copy Microsoft.OData.Service.dll and test framework
            if (!AstoriaTestProperties.SetupRunOnWebServer)
            {
                if (AstoriaTestProperties.Host != Host.Debug)
                    File.Copy(Path.Combine(Path.GetDirectoryName(resourceAssembly.Location), DataFxAssemblyRef.File.DataServices), Path.Combine(DestinationFolder, DataFxAssemblyRef.File.DataServices));
                else
                    File.Copy(Path.Combine(TestUtil.GreenBitsReferenceAssembliesDirectory, DataFxAssemblyRef.File.DataServices), Path.Combine(DestinationFolder, DataFxAssemblyRef.File.DataServices));
                if (AstoriaTestProperties.Host != Host.Debug)
                    File.Copy(Path.Combine(Path.GetDirectoryName(resourceAssembly.Location), DataFxAssemblyRef.File.DataServicesClient), Path.Combine(DestinationFolder, DataFxAssemblyRef.File.DataServicesClient));
                else
                    File.Copy(Path.Combine(TestUtil.GreenBitsReferenceAssembliesDirectory, DataFxAssemblyRef.File.DataServicesClient), Path.Combine(DestinationFolder, DataFxAssemblyRef.File.DataServicesClient));
            }
#endif

            // TODO: cannot get rid of framework DLL for client SKU
            // File.Copy(resourceAssembly.Location, Path.Combine(hostFolder, Path.GetFileName(resourceAssembly.Location)));
            CopyServerWorkspaceFiles(DestinationFolder);
        }

        private void CompileServiceHostExe(string path, string name)
        {
            List<string> codeFiles = new List<string>();
            foreach (string file in Directory.GetFiles(path))
            {
                if (Path.GetExtension(file) == ".cs")
                    codeFiles.Add(file);
            }

            HashSet<Assembly> references = this.Workspace.GetReferencedAssemblies();
            references.Add(typeof(System.ServiceModel.ServiceHost).Assembly);
            references.Add(typeof(System.ServiceModel.Web.WebServiceHost).Assembly);

            Util.CodeCompilerHelper.CompileCodeFiles(
                codeFiles.ToArray(),
                Path.Combine(path, name) + ".dll",
                new string[] { this.Workspace.DataLayerProviderKind.ToString() },
                this.Workspace.Language,
                references.Select(a => a.ManifestModule.FullyQualifiedName).ToArray());
        }

        protected override void Dispose(bool disposing)
        {
            
        }
    }
}
