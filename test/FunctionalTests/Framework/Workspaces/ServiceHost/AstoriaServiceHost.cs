//---------------------------------------------------------------------
// <copyright file="AstoriaServiceHost.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Data.Test.Astoria.TestExecutionLayer;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Configuration;
#if !ClientSKUFramework
using System.ServiceModel.Configuration;
#endif
using System.Linq;
using System.Data.Test.Astoria.Util;

namespace System.Data.Test.Astoria
{
    public class AstoriaServiceHost : AstoriaWebDataService
    {
        private Process process;
        private EventWaitHandle waitHandle;
        private readonly string identifier = Guid.NewGuid().ToString();

        private const string waitHandlePrefix = "TestServiceHost_Wait_";
        private string waitHandleName = null;

        private const string serviceName = "TestServiceHost";

        private readonly string serviceHostFolder = Environment.GetEnvironmentVariable("SystemDrive") + @"\serviceHost";
        private readonly string rootUri = Uri.UriSchemeHttp + "://" + Environment.MachineName + ":7777";

        private string serviceInstanceName = null;
        private string exeName = null;
        private string localExecutablePath = null;

        public AstoriaServiceHost(Workspace workspace, string webDataServicePrefixName, string machineName, AstoriaDatabase database)
            : base(workspace, webDataServicePrefixName, machineName, database)
        {

            if (AstoriaTestProperties.Host == Host.IDSH || AstoriaTestProperties.Host == Host.IDSH2)
            {
                if (AstoriaTestProperties.HostAuthenicationMethod != "None")
                    AstoriaTestLog.FailAndThrow("Test implementations of IDSH do not support authentication");
            }

            waitHandleName = waitHandlePrefix + identifier;
            serviceInstanceName = this.WebDataServicePrefixName + "_" + identifier;
            exeName = serviceName + this.WebDataServicePrefixName;

            if (ProcessHelper.IsLocalMachine(this.MachineName))
            {
                DestinationFolder = Path.Combine(serviceHostFolder, serviceInstanceName);
                DestinationFolder_Local = DestinationFolder;
                ExecutablePath = Path.Combine(DestinationFolder, exeName + ".exe");
                localExecutablePath = ExecutablePath;
            }
            else
            {
                string remoteMachineLocalPath = IISHelper.GetLocalMachineWWWRootSharePath(this.MachineName);
                DestinationFolder_Local = Path.Combine(IISHelper.GetLocalMachineWWWRootSharePath(this.MachineName), serviceInstanceName);
                DestinationFolder = Path.Combine(IISHelper.GetWWWRootSharePath(this.MachineName), serviceInstanceName);
                ExecutablePath = Path.Combine(DestinationFolder, exeName + ".exe");
                localExecutablePath = Path.Combine(DestinationFolder_Local, exeName + ".exe");
            }

            rootUri = Uri.UriSchemeHttp + "://" + this.MachineName + ":7777";

        }

        public string ExecutablePath
        {
            get
            {
                return Path.Combine(DestinationFolder, exeName + ".exe");
            }
            private set
            {
            }
        }

        protected override void CreateWebService(bool verify)
        {
#if ClientSKUFramework

        string serviceHostApplicationName = exeName.Replace("TestServiceHost","") +".exe";

        AstoriaTestLog.WriteLine("DestinationFolder is {0}",DestinationFolder);
        AstoriaTestLog.WriteLine("ExecutablePath is {0}",ExecutablePath);

	Directory.CreateDirectory(DestinationFolder);

 	File.Copy( 
                  Path.Combine(Environment.CurrentDirectory,serviceHostApplicationName),
                  Path.Combine(ExecutablePath) 
                  ,true
                 );
	CopyClientSKUFiles(DestinationFolder);
	
#endif
            CopyServiceFiles();

#if !ClientSKUFramework


            if (this.Workspace.ServiceModifications != null)
                this.Workspace.ServiceModifications.ApplyChanges(DestinationFolder);


            string tempDllLocation = Environment.CurrentDirectory;
            if (ProcessHelper.IsLocalMachine(this.MachineName))
            {
                tempDllLocation = DestinationFolder;
            }
            CompileServiceHostExe(tempDllLocation, DestinationFolder, exeName);

            if (!ProcessHelper.IsLocalMachine(this.MachineName))
                File.Copy(Path.Combine(tempDllLocation, exeName + ".exe"), Path.Combine(DestinationFolder, exeName + ".exe"), true);
#endif

            DeployAppConfig();

            // set up service URIs
            ServiceRootUri = rootUri + "/" + serviceInstanceName;
            ServiceUri = ServiceRootUri;


            Start();

            if (verify)
            {
                Thread.Sleep(1000);
                VerifyService();
            }
        }

        protected internal void DeployAppConfig()
        {
            IOUtil.FindAndWriteResourceToFile(this.GetType().Assembly, "app.config", ExecutablePath + ".config");

            Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ExecutablePath);
#if !ClientSKUFramework
            if (AstoriaTestProperties.Host == Host.WebServiceHost)
            {
                BindingsSection bindingsSection = config.GetSection("system.serviceModel/bindings") as BindingsSection;
                WebHttpBindingCollectionElement webHttpBindingCollectionElement;
                WebHttpBindingElement webHttpBindingElement;
                foreach (BindingCollectionElement bindingCollectionElement in bindingsSection.BindingCollections)
                {
                    // find WebHttpBinding element "higherMessageSize" and modify its settings
                    if (bindingCollectionElement.BindingType.ToString().Equals("System.ServiceModel.WebHttpBinding"))
                    {
                        webHttpBindingCollectionElement = bindingCollectionElement as WebHttpBindingCollectionElement;
                        webHttpBindingElement = webHttpBindingCollectionElement.Bindings["higherMessageSize"];

                        if (webHttpBindingElement != null)
                        {
                            // webHttpBinding -> binding -> security ->transport
                            WebHttpSecurityElement webHttpSecurityElement = webHttpBindingElement.Security;
                            switch (AstoriaTestProperties.HostAuthenicationMethod.ToLower())
                            {
                                case "windows":
                                    webHttpSecurityElement.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Windows;
                                    webHttpSecurityElement.Transport.ProxyCredentialType = System.ServiceModel.HttpProxyCredentialType.Windows;
                                    break;
                                default:
                                    webHttpSecurityElement.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.None;
                                    webHttpSecurityElement.Transport.ProxyCredentialType = System.ServiceModel.HttpProxyCredentialType.None;
                                    break;
                            }
                        }
                    }
                }
            }
#endif

            if (Database != null)
            {
                // fixup config files
                config.ConnectionStrings.ConnectionStrings.Clear();
                config.ConnectionStrings.ConnectionStrings.Add(this.Workspace.GetConnectionStringSettingsForProvider(this, this.Database.DatabaseConnectionString));

            }

            config.Save();

            if (AstoriaTestProperties.WebServiceHostTargetFramework != null)
            {
                //Would rather do this through the API but unsure how
                string contents = File.ReadAllText(ExecutablePath + ".config");
                int configurationPos = contents.IndexOf("<configuration>");
                string startupStr = string.Format("<startup><supportedRuntime version=\"{0}\"/></startup>", AstoriaTestProperties.WebServiceHostTargetFramework);
                contents = contents.Insert(configurationPos + 16, startupStr);
                File.WriteAllText(ExecutablePath + ".config", contents);


            }

        }

        protected virtual void Start()
        {
            if (ProcessHelper.IsLocalMachine(this.MachineName))
            {
                if (process == null)
                {
                    waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, waitHandleName);
                    waitHandle.Reset();

                    ProcessStartInfo processStartInfo = new ProcessStartInfo();
                    processStartInfo.FileName = ExecutablePath;
                    
                    if (AstoriaTestProperties.RuntimeEnvironment == TestRuntimeEnvironment.CheckinSuites)
                    {
                        processStartInfo.CreateNoWindow = true;
                        processStartInfo.UseShellExecute = false;
                    }

                    if (AstoriaTestProperties.Host == Host.IDSH)
                        processStartInfo.Arguments = ServiceRootUri + " false" + " " + waitHandleName;
                    else if (AstoriaTestProperties.Host == Host.IDSH2)
                        processStartInfo.Arguments = ServiceRootUri + " true" + " " + waitHandleName;
                    else
                        processStartInfo.Arguments = ServiceRootUri + " " + waitHandleName;

                    process = Process.Start(processStartInfo);
                }
            }
        }

        protected virtual void Stop()
        {
            if (ProcessHelper.IsLocalMachine(this.MachineName))
            {
                if (process != null)
                {
                    waitHandle.Set();
                    process.WaitForExit(2000);
                    if (!process.HasExited)
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                    process = null;
                }
            }
        }

        private void CopyServiceFiles()
        {
            Assembly resourceAssembly = this.GetType().Assembly;

#if !ClientSKUFramework

            IOUtil.EnsureDirectoryExists(DestinationFolder);
            IOUtil.EmptyDirectoryRecusively(DestinationFolder);

#endif
            string serviceSource;
            if (AstoriaTestProperties.Host == Host.IDSH || AstoriaTestProperties.Host == Host.IDSH2)
                serviceSource = "IDataServiceHostRunner";
            else
                serviceSource = "TestServiceHost";

            string serviceSourceCodePath = Path.Combine(DestinationFolder, serviceName + ".cs");
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, serviceSource + ".cs", serviceSourceCodePath);

            string sourceText = File.ReadAllText(serviceSourceCodePath);
            sourceText = sourceText.Replace("//[[Usings]]", this.Workspace.BuildDataServiceClassUsings());
            sourceText = sourceText.Replace("//[[ServiceCode]]", this.Workspace.BuildDataServiceClassCode());
            File.WriteAllText(serviceSourceCodePath, sourceText);

            // copy additional files
            foreach (string file in this.Workspace.ServiceHostFiles())
            {
                string newPath = file.Replace(Workspace.HostSourceFolder, DestinationFolder);
                IOUtil.EnsureDirectoryExists(Path.GetDirectoryName(newPath));
                File.Copy(file, newPath);
            }

            var assembliesToCopy = new List<Assembly>
            {
                typeof(FullTrust.TrustedMethods).Assembly, // this is a special case for the fully-trusted-methods assembly, which must always be copied
                typeof(Microsoft.Spatial.ISpatial).Assembly,
                typeof(Microsoft.OData.Edm.IEdmModel).Assembly,
                typeof(Microsoft.OData.ODataException).Assembly,
#if !ClientSKUFramework
                typeof(Microsoft.OData.Service.DataService<>).Assembly,
#endif
                typeof(Microsoft.OData.Client.DataServiceContext).Assembly,
            };

            foreach (var assembly in assembliesToCopy)
            {
                File.Copy(assembly.Location, Path.Combine(DestinationFolder, Path.GetFileName(assembly.Location)));    
            }

            // TODO: cannot get rid of precompiled library for the client SKU
            // File.Copy(resourceAssembly.Location, Path.Combine(hostFolder, Path.GetFileName(resourceAssembly.Location)));
            CopyServerWorkspaceFiles(DestinationFolder);

            // Copy win7 manifest file if test flag is set
            if (AstoriaTestProperties.WindowsCompatFlag == WindowsCompatFlag.Win7)
            {
                string serviceManifestPath = Path.Combine(DestinationFolder, serviceName + ".exe.manifest");
                IOUtil.FindAndWriteResourceToFile(resourceAssembly, serviceSource + ".exe.manifest", serviceManifestPath);
            }
        }

        protected virtual string[] GetReferencePaths(HashSet<Assembly> assemblies)
        {
            return assemblies.Select(a => a.ManifestModule.FullyQualifiedName).ToArray();
        }

        string _compilerVersion = "v4.0";
        public virtual string CompilerVersion
        {
            get
            {
                return _compilerVersion;
            }
            set
            {
                _compilerVersion = value;
            }
        }

        protected virtual void CompileServiceHostExe(string dllOutPath, string inputFilePath, string name)
        {
            string[] codeFiles = Directory.GetFiles(inputFilePath, "*.cs", SearchOption.AllDirectories).ToArray();

            HashSet<Assembly> references = this.Workspace.GetReferencedAssemblies();
#if !ClientSKUFramework
            references.Add(typeof(System.ServiceModel.ServiceHost).Assembly);
            references.Add(typeof(System.ServiceModel.Web.WebServiceHost).Assembly);
#endif
            references.Add(typeof(System.Configuration.ConfigurationManager).Assembly);

            references.Remove(typeof(System.Data.Test.Astoria.AstoriaTestLog).Assembly);

            Util.CodeCompilerHelper.CompileCodeFiles(
                codeFiles,
                Path.Combine(dllOutPath, name) + ".exe",
                new string[] { this.Workspace.DataLayerProviderKind.ToString() },
                this.Workspace.Language,
                new Dictionary<string, string>() { { "CompilerVersion", CompilerVersion } },
                GetReferencePaths(references),
                true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
                base.Dispose(disposing);
            }
        }
    }
}
