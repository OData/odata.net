//---------------------------------------------------------------------
// <copyright file="EdmWorkspace.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System.Collections.Generic;

    using System.Data.EntityClient; //EntityConnectionStringBUilder

    using System.Data.Objects;

    using System.Data.Test.Astoria.TestExecutionLayer;
    using System.IO;            //Path
    using System.Linq;
    using System.Reflection; //Assembly
    using Microsoft.Test.ModuleCore; //TestFailedException;
    //#if !ClientSKUFramework
    using System.Data.Objects.DataClasses;
    //#endif

    using System.Runtime.Remoting.Channels;
    using System.Diagnostics;
    using System.Security.Principal;
    using System.Management;
    using System.Runtime.Remoting.Channels.Tcp;
    using System.Xml.Linq;
    using System.Linq.Expressions;
    using System.Xml;
    using System.Security.Permissions;
    using System.Security;
    using System.Data.Test.Astoria.FullTrust;

    //[DataLayerProvider("EDM")]
    [WorkspaceDefaultSettings(
        ServiceBaseClass = "System.Data.Test.Astoria.TestDataWebService",
        UpdatableImplementation = UpdatableImplementation.DataServiceUpdateProvider
        )
    ]
    public abstract class EdmWorkspace : Workspace, IDisposable
    {
        internal static BrowserConfig browserConfig;
        internal static bool disableRemoteBrowser = true;
        //Constructor
        public EdmWorkspace(String name, string contextNamespace, string contextTypeName)
            : base(DataLayerProviderKind.Edm, name, contextNamespace, contextTypeName)
        {
            BeforeServiceCreation.Add(() => WorkspaceLibrary.CreateDefaultDatabase(this));

            if (AstoriaTestProperties.IsRemoteClient)
                BeforeServiceCreation.Add(() => WorkspaceLibrary.AddSilverlightHostFiles(this));

            AfterServiceCreation.Add(CreateSpawnBrowserScripts);

            // default EF provider does not perform value-equality checks for ETags before V2
            if (!Versioning.Server.SupportsV2Features)
                Settings.UpdatableImplementation = UpdatableImplementation.IUpdatable;

            CsdlCallbacks = new List<Action<XmlDocument>>() { ApplyAttributesToCsdl };
            SsdlCallbacks = new List<Action<XmlDocument>>();
            MslCallbacks = new List<Action<XmlDocument>>();
            this.createdObjectContexts = new Queue<ObjectContext>();

            // Proxy types with decimal keys do not work in partial trust, so we need to go and 'hide' them so the tests pass
            // Note: this also repros on 64-bit platforms
            if ((AstoriaTestProperties.HostArch.Contains("64") || AstoriaTestProperties.ServiceTrustLevel != TrustLevel.Full) && AstoriaTestProperties.EdmObjectLayer == ServiceEdmObjectLayer.PocoWithProxy)
            {
                // This fix modifies the test model and must be run prior to all other actions
                BeforeServiceCreation.Insert(0, () =>
                    {
                        Func<ResourceType, bool> remove = rt => rt.Key.Properties.Any(p => p.Type is ClrDecimal);
                        foreach (ResourceContainer container in this.ServiceContainer.ResourceContainers.ToList())
                        {
                            if (remove(container.BaseType))
                            {
                                this.ServiceContainer.RemoveNode(container);
                                continue;
                            }

                            foreach (ResourceType type in container.ResourceTypes.ToList())
                            {
                                if (remove(type))
                                {
                                    container.RemoveNode(type);
                                    continue;
                                }

                                foreach (var property in type.Properties.OfType<ResourceProperty>().Where(p => p.IsNavigation && remove(p.OtherAssociationEnd.ResourceType)).ToList())
                                {
                                    type.Properties.Remove(property);
                                }
                            }
                        }
                    });
            }
        }

        public List<Action<XmlDocument>> CsdlCallbacks
        {
            get;
            private set;
        }

        public List<Action<XmlDocument>> SsdlCallbacks
        {
            get;
            private set;
        }

        public List<Action<XmlDocument>> MslCallbacks
        {
            get;
            private set;
        }

        /// <summary>
        /// Store the latest object contexts which have been created to load data from EF provider. The intention is to keep the contexts for future use (expand operation) 
        /// to avoid multiple-context problem.
        /// </summary>
        private Queue<ObjectContext> createdObjectContexts;

        /// <summary>
        /// max number of latest contexts in the queue.
        /// </summary>
        private static int MaximumContextsKeptInQueue = 10;

        #region Silverlight Framework Settings
        internal class Directories
        {
            public string RootPath { get; set; }
            public string RemoteBatchPath { get; set; }
            public string RemoteVBSPath { get; set; }
            public string NTFSPath { get; set; }
            public string SLTestPage { get; set; }
        }
        internal bool IsClientMAC
        {
            get
            {
                return !IsClientWindows;
            }
        }
        internal bool IsClientWindows
        {
            get
            {
                return AstoriaTestProperties.AstoriaClientOS == ClientOS.VistaStarter
                    || AstoriaTestProperties.AstoriaClientOS == ClientOS.WIN2K
                    || AstoriaTestProperties.AstoriaClientOS == ClientOS.XPHOME;
            }
        }
        #region Build Xap Dynamically


        private string SLFWCLientReferenceAssemblies
        {
            get
            {

                string progFiles = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE").Contains("64") ?
                    Environment.GetEnvironmentVariable("ProgramFiles(x86)") :
                    Environment.GetEnvironmentVariable("ProgramFiles");
                string clientLibrariesPath = progFiles + "\\Microsoft SDKs\\Silverlight\\v2.0\\Libraries\\Client";
                return clientLibrariesPath;

            }
        }
        private string SLFWReferenceAssemblies
        {
            get
            {

                string progFiles = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE").Contains("64") ?
                    Environment.GetEnvironmentVariable("ProgramFiles(x86)") :
                    Environment.GetEnvironmentVariable("ProgramFiles");
                string clientLibrariesPath = progFiles + "\\Microsoft SDKs\\Silverlight\\v2.0\\Reference Assemblies";
                return clientLibrariesPath;

            }
        }

        private static void GenerateSilverlightXAP(Assembly resourceAssembly, string webserviceWorkspaceDir, string tempCLRBinDir, string webServiceAppCodeDir, string webServiceClientBinDir)
        {

            #region Generate and Copy XAP

            #region Copy Code files for SL Dll


            IOUtil.EnsureDirectoryExists(tempCLRBinDir);
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, "SilverlightAstoriaTestUI.csproj", Path.Combine(tempCLRBinDir, "SilverlightAstoriaTestUI.csproj"));
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, "AppManifest.xml", Path.Combine(tempCLRBinDir, "AppManifest.xml"));
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, "App.xaml", Path.Combine(tempCLRBinDir, "App.xaml"));
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, "Page.xaml", Path.Combine(tempCLRBinDir, "Page.xaml"));
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, "App.xaml.cs", Path.Combine(tempCLRBinDir, "App.xaml.cs"));
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, "Page.xaml.cs", Path.Combine(tempCLRBinDir, "Page.xaml.cs"));
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, "App.g.cs", Path.Combine(tempCLRBinDir, "App.g.cs"));
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, "Page.g.cs", Path.Combine(tempCLRBinDir, "Page.g.cs"));
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, "RemoteClient.cs", Path.Combine(tempCLRBinDir, "RemoteClient.cs"));
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, "Dispatch.cs", Path.Combine(tempCLRBinDir, "Dispatch.cs"));
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, "Arubaclientmodel.ncs", Path.Combine(tempCLRBinDir, "Arubaclientmodel.cs"));
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, "NorthwindClientModel.ncs", Path.Combine(tempCLRBinDir, "NorthwindClientModel.cs"));

            // "VBDispatch Project"
            string VBCodePath = Path.Combine(tempCLRBinDir, "VBDispatch");
            IOUtil.EnsureDirectoryExists(VBCodePath);
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, "Dispatch.VB", Path.Combine(VBCodePath, "Dispatch.vb"));
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, "Arubaclientmodel.vb", Path.Combine(VBCodePath, "Arubaclientmodel.vb"));
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, "NorthwindClientModel.vb", Path.Combine(VBCodePath, "NorthwindClientModel.vb"));
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, "VBDispatch.vbproj", Path.Combine(VBCodePath, "VBDispatch.vbproj"));

            // "CommonTypes Project"
            string CommonTypesPath = Path.Combine(tempCLRBinDir, "CommonTypes");
            IOUtil.EnsureDirectoryExists(CommonTypesPath);
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, "CommonTypes.cs", Path.Combine(CommonTypesPath, "CommonTypes.cs"));
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, "CommonTypes.csproj", Path.Combine(CommonTypesPath, "CommonTypes.csproj"));
            #endregion

            #region Create the SL XAP File

            TestUtil.GenerateSilverlightXAPUsingMsBuild(Path.Combine(tempCLRBinDir, "SilverlightAstoriaTestUI.csproj"), webServiceClientBinDir, "SilverlightAstoriaTest");

            #endregion

            #endregion


            File.Copy(Path.Combine(tempCLRBinDir, "ClientBin\\SilverlightAstoriaTest.xap"),
                Path.Combine(webserviceWorkspaceDir, "SilverlightAstoriaTest.xap"), true);
        }
        #endregion Build Xap Dynamically

        #region Control Browsers
        internal class BrowserConfig
        {
            internal class Machine
            {
                public string Name { get; set; }
                public string IP { get; set; }
                public string SystemDrive { get; set; }
                public string Architecture { get; set; }
                public bool Visible { get; set; }
            }
            public Machine MachineInfo { get; set; }
            public ClientBrowser BrowserName { get; set; }
            public string Version { get; set; }

        }

        private static void LoadBrowserConfig()
        {
            browserConfig =
                new BrowserConfig()
                {
                    MachineInfo = new BrowserConfig.Machine()
                    {
                        Name = Environment.MachineName,
                        //IP = machineConfig.Attribute(XName.Get("IP")).Value,
                        SystemDrive = Environment.GetEnvironmentVariable("SystemDrive"),
                        Architecture = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE").Contains("86") ? "x86" : "x64",
                    },
                    BrowserName = AstoriaTestProperties.AstoriaClientBrowser
                };
            browserConfig.MachineInfo.Visible = true;

            //////AstoriaTestLog.WriteLineIgnore("From Test Properties :" + AstoriaTestProperties.HostArch);
            AstoriaTestLog.WriteLineIgnore(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE").Contains("86") ? "x86" : "x64");
        }

        private void CopyFilesToRemoteShare()
        {
        }
        private static void UnInstallOOBApplications()
        {
            if (AstoriaTestProperties.RunOutOfBrowser)
            {
                //shutdown the oob applications running
                AstoriaTestLog.WriteLineIgnore("Shutting down running oob applications");
                var oobApps = Process.GetProcessesByName("sllauncher");
                foreach (Process oobApp in oobApps)
                {
                    oobApp.Kill();
                }
                AstoriaTestLog.WriteLineIgnore("Uninstalling oob applications");
                //%UserProfile%\AppData\LocalLow\Microsoft\Silverlight\OutOfBrowser
                //"%USERPROFILE%\\Local Settings\\Application Data\\Microsoft\\Silverlight\\OutOfBrowser"
                string oobAppsInstallPath = Environment.ExpandEnvironmentVariables("%UserProfile%\\AppData\\LocalLow\\Microsoft\\Silverlight\\OutOfBrowser");
                if (Directory.Exists(oobAppsInstallPath))
                    IOUtil.EmptyDirectoryRecusively(oobAppsInstallPath);
            }
        }
        private static void StartRemoteBrowser(string remoteDataService)
        {
            if (disableRemoteBrowser)
                return;

            if (workspaceDirectories.Any<Directories>(dir => dir.RootPath.ToLower() == remoteDataService.ToLower()))
            {
                if (browserConfig == null && AstoriaTestProperties.Client == ClientEnum.SILVERLIGHT)
                {
                    LoadBrowserConfig();
                }
                Directories currentBSessionInfo = workspaceDirectories.Where<Directories>(dir => dir.RootPath.ToLower() == remoteDataService.ToLower())
                    .First<Directories>();

                object[] theProcessToRun = { BrowserPath };
                ConnectionOptions theConnection = new ConnectionOptions();
                //theConnection.Username = string.Emtpy;
                //theConnection.Password = string.Emtpy;

                ManagementScope theScope = new ManagementScope("\\\\" + browserConfig.MachineInfo.IP + "\\root\\cimv2", theConnection);
                theScope.Connect();

                ManagementClass theClass = new ManagementClass(theScope, new ManagementPath("Win32_Process"), new ObjectGetOptions());

                ManagementBaseObject inParams = theClass.GetMethodParameters("Create");
                inParams["CommandLine"] = currentBSessionInfo.RemoteBatchPath;

                ManagementBaseObject outParams = theClass.InvokeMethod("Create", inParams, null);

            }
        }

        public List<KeyValuePair<ClientBrowser, string>> _browserExes;
        private static string BrowserPath
        {
            get
            {
                return _browserPath;
            }
        }
        static string _browserPath = "";
        private string GetBrowserPath()
        {
            if (browserConfig == null)
            {
                LoadBrowserConfig();
            }
            string browserPath = "";
            string programFilesDir = "";
            switch (browserConfig.BrowserName)
            {
                case ClientBrowser.IE5:
                case ClientBrowser.IE6:
                case ClientBrowser.IE7:
                case ClientBrowser.IE8:
                    programFilesDir = SetProgramFilesPath(programFilesDir);
                    browserPath = Path.Combine(programFilesDir, "Internet Explorer\\IEXPLORE.exe");
                    break;
                case ClientBrowser.FF15:
                case ClientBrowser.FF2:
                case ClientBrowser.FF3:
                    programFilesDir = SetProgramFilesPath(programFilesDir);
                    browserPath = Path.Combine(programFilesDir, "Mozilla Firefox\\Firefox.exe");
                    break;
                case ClientBrowser.Chrome:
                    programFilesDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    browserPath = Path.Combine(programFilesDir, "Google\\Chrome\\Application\\Chrome.exe");
                    break;
            }
            _browserPath = browserPath;
            return browserPath;
        }

        private static string SetProgramFilesPath(string programFilesDir)
        {
            programFilesDir = browserConfig.MachineInfo.SystemDrive + "\\Program Files";
            programFilesDir = browserConfig.MachineInfo.Architecture.ToLower() == "x64" ?
                programFilesDir + " (x86)" : programFilesDir;
            AstoriaTestLog.WriteLineIgnore("Program files is :" + programFilesDir);
            return programFilesDir;
        }

        public static void StartRemoteClient(string serviceURI)
        {
            if (!IsLocalRun)
                RestartBrowserAction(new List<string>() { serviceURI.Replace("/AstoriaTestSilverlight.svc", "") + "/SilverlightAstoriaTestPage.html" }.ToArray());

            ShutdownRemoteClient();

            if (browserConfig.BrowserName == ClientBrowser.SF3)
            {
                return;
            }
            if (IsLocalRun)
            {
                Process[] p = new Process[2];
                string targetDir;
                foreach (Directories servicemap in workspaceDirectories)
                {
                    AstoriaTestLog.WriteLine("Running IE via VBs File ");
                    StartVistaUACBrowser(servicemap.RemoteVBSPath);

                }
            }
            else //remote browser maintenance
            {
                if (!disableRemoteBrowser)
                {
                    UnInstallOOBApplications();
                    StartRemoteBrowser(serviceURI);
                }
            }
        }

        public static Process[] StartRemoteClients()
        {
            ShutdownRemoteClient();
            Process[] p = new Process[2];
            string targetDir;
            try
            {
                int index = 0;
                foreach (Directories servicemap in workspaceDirectories)
                {
                    targetDir = servicemap.NTFSPath;
                    p[index] = new Process();
                    p[index].StartInfo.WorkingDirectory = targetDir;
                    p[index].StartInfo.FileName = "launchIE.bat";
                    p[index].StartInfo.Arguments = string.Format("Silverlight Remote Client");
                    p[index].StartInfo.CreateNoWindow = false;
                    p[index].Start();
                    index++;
                }
                return p;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Occurred :{0},{1}",
                          ex.Message, ex.StackTrace.ToString());
                throw ex;
            }

        }

        public static bool IsVistaOrLater()
        {
            return Environment.OSVersion.Platform ==
                     PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6;
        }
        private static bool ShouldWorkaroundDueToVistaUAC()
        {
            if (AstoriaTestProperties.Host == Host.LocalIIS)
            {

                if (!IsVistaOrLater())
                {

                    return false;
                }

                WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principle = new WindowsPrincipal(currentIdentity);
                SecurityIdentifier sidAdmin = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
                if (!principle.IsInRole(sidAdmin))
                {
                    AstoriaTestLog.TraceLine("IsAdmin=False");
                    return true;
                }
                AstoriaTestLog.TraceLine("IsAdmin=true");
            }
            return false;
        }
        private static void StartVistaUACBrowser(string filePath)
        {
            Process[] processes = Process.GetProcessesByName("Commander.Server");
            if (processes.Length > 0)
            {
                if (ChannelServices.RegisteredChannels.Any(ch => ch.ChannelName == "tcp"))
                {
                    return;
                }
                TcpChannel channel = new TcpChannel();
                ChannelServices.RegisterChannel(channel, true);
                Commander.RemoteServer remoteServer = Commander.RemoteServer.CreateLoggedInClientRemoteServer("localhost");

                Commander.RemoteExecutableResults results = remoteServer.ExecuteScript(filePath);
                if (results.ExitCode != 0)
                    throw new TestFailedException("Unable to run create website script:\n" + results.Output);
                AstoriaTestLog.TraceLine(results.Output);
                System.Runtime.Remoting.Channels.ChannelServices.UnregisterChannel(channel);
            }
            else
                throw new TestFailedException("Expected a RemoteServer program called Commander.Server.exe to be running");
        }

        internal static bool IsLocalRun
        {
            get
            {
                return
                    AstoriaTestProperties.AstoriaClientOS == ClientOS.Local
                    && !AstoriaTestProperties.IsManualRun;
                //(browserConfig.MachineInfo.Name.ToLower().Equals(Environment.MachineName.ToLower()));
            }
        }

        internal static void ShutdownRemoteClient()
        {
            if (browserConfig.BrowserName == ClientBrowser.SF2
                || browserConfig.BrowserName == ClientBrowser.SF3)
            {
                return;
            }
            UnInstallOOBApplications();
            if (IsLocalRun)
            {
                Process[] allBrowsers = Process.GetProcessesByName(
                       GetBrowserExe(browserConfig.BrowserName)
                       );
                foreach (Process browserSession in allBrowsers)
                {
                    browserSession.Kill();
                }
            }
        }

        private static string GetBrowserExe(ClientBrowser browserType)
        {
            string browserPath = "";
            switch (browserType)
            {
                case ClientBrowser.IE5:
                case ClientBrowser.IE6:
                case ClientBrowser.IE7:
                case ClientBrowser.IE8:
                    browserPath = "iexplore";
                    break;
                case ClientBrowser.FF2:
                case ClientBrowser.FF3:
                case ClientBrowser.FF15:
                    browserPath = "Firefox";
                    break;
                case ClientBrowser.SF2:
                case ClientBrowser.SF3:
                    browserPath = "Safari";
                    break;
                case ClientBrowser.Chrome:
                    browserPath = "chrome";
                    break;
            }
            return browserPath;
        }
        #endregion

        internal class MachineConfig
        {
            public int? MachineID { get; set; }
            public int BrowserID { get; set; }
            public string MachineName { get; set; }
            public string SilverlightVersion { get; set; }
            public string IPAddress { get; set; }
        }
        internal static string remoteClientConfigDb = "server=sqlpod068-19;user=OLEDB;pwd=fakepwd;database=RemoteClientConfig";
        internal static string remoteClientConfigDropShare = "\\\\wddata\\Public\\steveob\\SlRemoteRun";

        internal static void AssignMachine()
        {
            AssignReleaseMachine(_currentTestMachine, true);
        }

        internal static void ReleaseMachine()
        {

            AssignReleaseMachine(_currentTestMachine, false);
        }

        internal static void AssignReleaseMachine(MachineConfig freeMachine, bool IsAssign)
        {
            if (freeMachine == null)
                return;

            string spName = IsAssign ? "AssignMachine" : "ReleaseMachine";
            SqlClient.SqlConnection sqlCon = new System.Data.SqlClient.SqlConnection(remoteClientConfigDb);
            sqlCon.Open();
            SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand(spName, sqlCon);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            SqlClient.SqlParameter machineIDParam = new System.Data.SqlClient.SqlParameter("@MachineID", freeMachine.MachineID);
            sqlCommand.Parameters.Add(machineIDParam);
            sqlCommand.ExecuteNonQuery();

        }


        internal static MachineConfig GetAvailableMachine()
        {
            MachineConfig freeMachine = new MachineConfig();

            #region Get Available Machine
            SqlClient.SqlConnection sqlCon = new System.Data.SqlClient.SqlConnection(remoteClientConfigDb);
            sqlCon.Open();

            SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("GetAvailableMachines", sqlCon);

            #region staging
            SqlClient.SqlParameter osNameParam = new System.Data.SqlClient.SqlParameter("@OsName", AstoriaTestProperties.AstoriaClientOS.ToString());

            SqlClient.SqlParameter browserNameParam = new System.Data.SqlClient.SqlParameter("@BrowserName", AstoriaTestProperties.AstoriaClientBrowser.ToString());
            SqlClient.SqlParameter silverlightVersionParam = new System.Data.SqlClient.SqlParameter("@SilverlightVersion", "3.0.2.1");
            sqlCommand.Parameters.Add(osNameParam);
            sqlCommand.Parameters.Add(browserNameParam);
            sqlCommand.Parameters.Add(silverlightVersionParam);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            #endregion

            SqlClient.SqlDataReader machines = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
            while (machines.Read())
            {

                freeMachine.MachineID = Int32.Parse(machines["MachineID"].ToString());
                freeMachine.MachineName = machines["MachineName"].ToString();
                freeMachine.IPAddress = machines["IPAddress"].ToString();
                break;
            }
            #endregion Get Available Machine


            if (sqlCon.State == ConnectionState.Open)
            {
                sqlCon.Close();
            }


            return freeMachine;
        }

        internal static MachineConfig _currentTestMachine;

        internal static void RestartBrowserAction(string[] uris)
        {
            WriteConfigFile(uris, "restart");
        }
        internal static void WriteConfigFile(string[] uris)
        {
            WriteConfigFile(uris, "start");
        }
        /// <summary>
        /// Writes the test configuration to an xml file which can be read by remote clients
        /// </summary>
        /// <param name="uris"></param>
        /// <example>
        /// <root>
        ///     <browser></browser>
        ///     <browseraction>start/stop</browseraction>
        ///     <services>
        ///         <service>"Uri Goes Here"</service>
        ///         <service>"Uri Goes Here"</service>
        ///     </services>
        /// </root>
        /// </example> 
        internal static void WriteConfigFile(string[] uris, string browserAction)
        {
            string browserExecutable = GetBrowserExe(AstoriaTestProperties.AstoriaClientBrowser);

            XDocument xDoc = new XDocument();
            XName xnRoot = XName.Get("root");
            XName xnBrowser = XName.Get("browser");
            XName xnBrowserAction = XName.Get("browseraction");
            XName xnServiceS = XName.Get("services");
            XName xnService = XName.Get("service");


            XElement xeRoot = new XElement(xnRoot);

            #region  add <browser> node
            XElement xeBrowser = new XElement(xnBrowser);
            xeBrowser.Value = browserExecutable;
            XElement xeBrowserAction = new XElement(xnBrowserAction);
            xeBrowserAction.Value = browserAction;
            #endregion add <browser> node
            #region  add <services> node
            XElement xeServiceS = new XElement(xnServiceS);
            foreach (string serviceURI in uris)
            {
                XElement xeService = new XElement(xnService);
                xeService.Value = serviceURI;
                xeServiceS.Add(xeService);
            }
            #endregion  add <services> node

            xeRoot.Add(xeBrowser);
            xeRoot.Add(xeBrowserAction);
            xeRoot.Add(xeServiceS);
            xDoc.Add(xeRoot);


            AstoriaTestLog.WriteLine("Waiting for machine {0} , {1}", AstoriaTestProperties.AstoriaClientOS.ToString(), AstoriaTestProperties.AstoriaClientBrowser.ToString());
            while (_currentTestMachine == null || String.IsNullOrEmpty(_currentTestMachine.MachineName))
            {
                _currentTestMachine = GetAvailableMachine();
                if (_currentTestMachine.MachineName == null)
                {
                    AstoriaTestLog.Write(".");
                }
                System.Threading.Thread.Sleep(10000);
            }
            if (!String.IsNullOrEmpty(_currentTestMachine.MachineName))
            {
                AstoriaTestLog.WriteLine("Remote Client Machine is : {0}", _currentTestMachine.MachineName);
                AssignMachine();
                xDoc.Save(Path.Combine(remoteClientConfigDropShare, _currentTestMachine.MachineName.Trim(' ') + ".xml"), System.Xml.Linq.SaveOptions.None);
            }
            else
            {
                throw (new TestFailedException(String.Format("Failed to find Machines for the config : {0} , {1}", AstoriaTestProperties.AstoriaClientOS.ToString(), AstoriaTestProperties.AstoriaClientBrowser.ToString())));
            }
        }
        #endregion

        internal static List<Directories> workspaceDirectories = new List<Directories>();
        public static string SLBatchContents = "";
        public void CreateSpawnBrowserScripts()
        {


            AstoriaTestLog.WriteLineIgnore("Silverlight web page resources:");
            AstoriaTestLog.WriteLineIgnore(this.DataService.ServiceRootUri + "Monitor.aspx");
            AstoriaTestLog.WriteLineIgnore(this.DataService.ServiceRootUri + "SilverlightAstoriaTestPage.html");

            if (browserConfig == null && AstoriaTestProperties.Client == ClientEnum.SILVERLIGHT) LoadBrowserConfig();

            String filePath = "";
            string vbsFilePath = "";

            string slTestPage = this.DataService.ServiceRootUri + "SilverlightAstoriaTestPage.html?" + AstoriaTestProperties.AstoriaClientLanguage.ToString();
            if (AstoriaTestProperties.RunXDomain)
            {
                string strMachineName = "http://" + Environment.MachineName.ToLower();
                slTestPage = this.ContextNamespace.ToLower().Contains("aruba") ?
                                this.DataService.ServiceRootUri.Replace(strMachineName, "http://www.northwind1.com")
                                : this.DataService.ServiceRootUri.Replace(strMachineName, "http://www.aruba1.com");
                slTestPage += "SilverlightAstoriaTestPage.html?" + AstoriaTestProperties.AstoriaClientLanguage.ToString();
            }

            if (AstoriaTestProperties.Client == ClientEnum.SILVERLIGHT)
            {
                slTestPage += (AstoriaTestProperties.RunOutOfBrowser ? "&offline" : "");
                if (IsLocalRun)
                {
                    AstoriaTestLog.WriteLine("Processor Architecture is " + browserConfig.MachineInfo.Architecture);
                    filePath = Path.Combine(this.DataService.DestinationFolder, "launchIE.bat");
                    vbsFilePath = Path.Combine(this.DataService.DestinationFolder, "launchIE.vbs");
                }
                else
                {

                    filePath = String.Format("\\\\{0}\\wwwroot\\{1}", browserConfig.MachineInfo.Name,
                                             this.DataService.WebDataServiceName + "_launchIE.vbs");
                }

                if (workspaceDirectories.Any(wd => wd.NTFSPath == this.DataService.DestinationFolder))
                {
                    workspaceDirectories.Remove(workspaceDirectories.First(wd => wd.NTFSPath == this.DataService.DestinationFolder));
                }

                workspaceDirectories.Add(
                        new Directories()
                        {
                            NTFSPath = this.DataService.DestinationFolder,
                            SLTestPage = slTestPage,
                            RootPath = this.DataService.ServiceRootUri.ToLower().TrimEnd('/'),
                            RemoteBatchPath = filePath,
                            RemoteVBSPath = vbsFilePath
                        }
                    );

            }
            #region Write URIs to local file
            string MonitorURI = "\r\n" + this.DataService.ServiceRootUri + "Monitor.aspx";
            string testServiceURI = "\r\n" + this.DataService.ServiceRootUri + "SilverlightAstoriaTestPage.html";
            #endregion

            #region Write Batch File to run Browser locally
            if (AstoriaTestProperties.Client == ClientEnum.SILVERLIGHT)
            {
                if (IsLocalRun)
                {
                    SLBatchContents = "\r\nSTART \"Silverlight Test Run\" \"{0}\" {1}\r\n";
                    SLBatchContents = String.Format(SLBatchContents,
                        GetBrowserPath(),
                        this.DataService.ServiceRootUri + "SilverlightAstoriaTestPage.html");
                }
                else
                {
                    if (services == null)
                    {
                        services = new List<string>();
                    }
                    services.Add(testServiceURI.Replace("\r\n", ""));
                    if (!IsNewRun && !AstoriaTestProperties.IsManualRun)
                    {
                        WriteConfigFile(services.ToArray());
                    }
                }

                if (IsLocalRun)
                {
                    FileStream fsWriter = new FileStream(filePath, FileMode.Create);
                    fsWriter.Write(System.Text.Encoding.ASCII.GetBytes(SLBatchContents), 0, System.Text.Encoding.ASCII.GetByteCount(SLBatchContents));
                    fsWriter.Flush();
                    fsWriter.Close();
                }

                SLBatchContents = "\r\nSet objWMIService = GetObject _\r\n(\"winmgmts:\\\\{0}\\root\\cimv2:Win32_Process\")\r\nobjWMIService.Create \"{1} {2}\", null , null, intProcessID";
                SLBatchContents = String.Format(SLBatchContents,
                                        browserConfig.MachineInfo.Name,
                                        GetBrowserPath(),
                                        slTestPage);


                if (IsLocalRun)
                {
                    FileStream fsWriter = new FileStream(vbsFilePath, FileMode.Create);
                    fsWriter.Write(System.Text.Encoding.ASCII.GetBytes(SLBatchContents), 0, System.Text.Encoding.ASCII.GetByteCount(SLBatchContents));
                    fsWriter.Flush();
                    fsWriter.Close();
                }

                SLBatchContents = "";
            }
            #endregion Write Batch File to run Browser remotely


            IsNewRun = IsNewRun ? false : true;

        }

        static bool IsNewRun = true;
        static List<string> services;




        /// <summary>
        /// Populates the source folder with the files that should be 
        /// available on the service host.
        /// </summary>
        public override void PopulateHostSourceFolder()
        {
            base.PopulateHostSourceFolder();

            Assembly resourceAssembly = this.GetType().Assembly;

            string codeFilePath = Path.Combine(WebServiceAppCodeDir, this.ObjectLayerOutputFileName);
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, this.ObjectLayerResourceName, codeFilePath);

            // Now write the Edm csdl,msl,ssdl files out to bin dir.
            base._csdlFilePath = Path.Combine(WebServiceBinDir, this.CsdlFileOutputFileName);
            string mslFilePath = Path.Combine(WebServiceBinDir, this.MslFileOutputFileName);
            string ssdlFilePath = Path.Combine(WebServiceBinDir, this.SsdlFileOutputFileName);
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, this.CsdlFileResourceName, base._csdlFilePath);
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, this.MslFileResourceName, mslFilePath);
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, this.SsdlFileResourceName, ssdlFilePath);

            ApplyCsdlCallbacks(base._csdlFilePath);

            // apply SSDL callbacks
            if (SsdlCallbacks.Any())
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(ssdlFilePath);
                SsdlCallbacks.ForEach(callback => callback(doc));
                doc.Save(ssdlFilePath);
            }

            // apply MSL callbacks
            if (MslCallbacks.Any())
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(mslFilePath);
                MslCallbacks.ForEach(callback => callback(doc));
                doc.Save(mslFilePath);
            }

#if !ClientSKUFramework
            //Adding service files for Media Link testing 
            string currentSVCPath = Path.Combine(this.WebServiceWorkspaceDir, "LiveMockService.svc");
            //string GDataXmlFile = Path.Combine(this.WebServiceWorkspaceDir, "GData.xml");
            string currentCSPath = Path.Combine(this.WebServiceAppCodeDir, "LiveMockService.svc.cs");
            string testImagePath = Path.Combine(Environment.CurrentDirectory, "testImage.png");

            IOUtil.FindAndWriteResourceToFile(resourceAssembly, "LiveMockService.svc", currentSVCPath);
            //IOUtil.FindAndWriteResourceToFile(resourceAssembly, "GData.xml", GDataXmlFile);
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, "LiveMockService.svc.cs", currentCSPath);
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, "testImage.png", testImagePath);
#endif

            if (AstoriaTestProperties.EdmObjectLayer != ServiceEdmObjectLayer.Default)
            {
                PopulateHostSourceFolderFixUp();
            }
        }

        private void PopulateHostSourceFolderFixUp()
        {
            Assembly resourceAssembly = this.GetType().Assembly;
            string objectLayerCodeFilePath = Path.Combine(WebServiceAppCodeDir, this.Name + ".Edm.ObjectLayer.cs");
            if (AstoriaTestProperties.EdmObjectLayer != ServiceEdmObjectLayer.MultiNamespaces)
                IOUtil.EnsureFileDeleted(objectLayerCodeFilePath);

            if (AstoriaTestProperties.EdmObjectLayer == ServiceEdmObjectLayer.PocoWithProxy)
            {
                IOUtil.FindAndWriteResourceToFile(resourceAssembly, this.Name + ".Edm.Poco.ObjectLayer.cs", objectLayerCodeFilePath);
                File.WriteAllText(objectLayerCodeFilePath, File.ReadAllText(objectLayerCodeFilePath).Replace("ContextOptions.ProxyCreationEnabled = false;", String.Empty));               
            }
            else if (AstoriaTestProperties.EdmObjectLayer == ServiceEdmObjectLayer.PocoWithoutProxy)
            {
                IOUtil.FindAndWriteResourceToFile(resourceAssembly, this.Name + ".Edm.Poco.ObjectLayer.cs", objectLayerCodeFilePath);  
            }
            else if (AstoriaTestProperties.EdmObjectLayer == ServiceEdmObjectLayer.STE)
            {
                IOUtil.FindAndWriteResourceToFile(resourceAssembly, this.Name + ".Edm.STE.ObjectLayer.cs", objectLayerCodeFilePath);  
            }
            else if (AstoriaTestProperties.EdmObjectLayer == ServiceEdmObjectLayer.MultiNamespaces && this.Name == "Northwind")
            {
                string content = File.ReadAllText(objectLayerCodeFilePath);

                // We need to change the object layer to be compatiable with conceptual model
                content = content.Replace("NamespaceName=\"northwind\", Name=\"Customers\"", "NamespaceName=\"northwind_ext\", Name=\"Customers\"");
                content = content.Replace("NamespaceName=\"northwind\", Name=\"Orders\"", "NamespaceName=\"northwind_ext\", Name=\"Orders\"");
                File.WriteAllText(objectLayerCodeFilePath, content);

                IOUtil.FindAndWriteResourceToFile(resourceAssembly, "northwind1.dll_", Path.Combine(WebServiceBinDir, "northwind1.dll"));
                IOUtil.FindAndWriteResourceToFile(resourceAssembly, "northwind2.dll_", Path.Combine(WebServiceBinDir, "northwind2.dll"));
                IOUtil.EnsureFileDeleted(Path.Combine(WebServiceBinDir, "Northwind.csdl"));

                IOUtil.FindAndWriteResourceToFile(resourceAssembly, "Northwind1.csdl", Path.Combine(WebServiceBinDir, "Northwind.csdl"));
                IOUtil.FindAndWriteResourceToFile(resourceAssembly, "Northwind2.csdl", Path.Combine(WebServiceBinDir, "Northwind_ext.csdl"));

                content = File.ReadAllText(Path.Combine(WebServiceBinDir, "Northwind.msl"));
                content = content.Replace("TypeName=\"northwind.Customers\"", "TypeName=\"northwind_ext.Customers\"");
                content = content.Replace("TypeName=\"northwind.Orders\"", "TypeName=\"northwind_ext.Orders\"");
                File.WriteAllText(Path.Combine(WebServiceBinDir, "Northwind.msl"), content);

                // We cannot delete this immediately because another operation depends on it. Instead we add the deletion operation to the pipeline since we are testing against multiple assemblies.
                this.BeforeServiceCreation.Add(() => IOUtil.EnsureFileDeleted(Path.Combine(WebServiceAppCodeDir, this.Name + ".Edm.ObjectLayer.cs")));
            }
        }

        private IEnumerable<ResourceAttribute> GetAttributesFromProperty(NodeProperty property)
        {
            List<ResourceAttribute> atts = property.Facets.Attributes.ToList();
            if (property is ResourceProperty && (property as ResourceProperty).IsComplexType)
                atts.AddRange((property.Type as ComplexType).Properties.SelectMany<NodeProperty, ResourceAttribute>(GetAttributesFromProperty));
            return atts;
        }

        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        private void ApplyCsdlCallbacks(string filePath)
        {
            if (CsdlCallbacks.Any())
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);
                CsdlCallbacks.ForEach(callback => callback(doc));
                doc.Save(filePath);
            }
        }

        private void ApplyAttributesToCsdl(XmlDocument csdl)
        {
            // apply any attributes to the csdl

            IEnumerable<ResourceAttribute> attributes =
                ServiceContainer.ResourceTypes
                .SelectMany(rt => rt.Facets.Attributes
                    .Union(rt.Properties.SelectMany(p => GetAttributesFromProperty(p))))
                .Union(ServiceContainer.Facets.Attributes).Distinct();

            foreach (ResourceAttribute att in attributes)
            {
                att.Apply(csdl);
            }
        }

        private string FindAndResetCSDL()
        {
            string outputCsdlFilePath = TrustedMethods.GetFilePaths(this.DataService.DestinationFolder, CsdlFileOutputFileName).Single();
            IOUtil.FindAndWriteResourceToFile(this.GetType().Assembly, this.CsdlFileResourceName, outputCsdlFilePath);
            return outputCsdlFilePath;
        }

        public override void ApplyFriendlyFeeds()
        {
            // start with a fresh csdl
            string outputCsdlFilePath = FindAndResetCSDL();

            // reapply everything based on current service container
            if (GenerateServerMappings)
            {
                ApplyCsdlCallbacks(outputCsdlFilePath);
            }
            if (GenerateClientTypes || GenerateClientTypesManually)
            {
                GenerateAndLoadClientTypes();
            }
            PopulateClientTypes();
        }

        public override List<string> ServiceHostFiles()
        {
            List<string> files = base.ServiceHostFiles();

            // these *may* need to come before the base ones
            string binFolder = Path.Combine(this.HostSourceFolder, "Bin");

            files.Add(Path.Combine(binFolder, this.CsdlFileOutputFileName));
            files.Add(Path.Combine(binFolder, this.MslFileOutputFileName));
            files.Add(Path.Combine(binFolder, this.SsdlFileOutputFileName));

            return files;
        }

        #region overrides to allow customization of Edm names
        public virtual string CsdlFileResourceName
        {
            get { return this.Name + ".csdl"; }
        }
        public virtual string SsdlFileResourceName
        {
            get { return this.Name + ".ssdl"; }
        }
        public virtual string MslFileResourceName
        {
            get { return this.Name + ".msl"; }
        }
        protected internal virtual string CsdlFileOutputFileName
        {
            get { return this.Name + ".csdl"; }
        }
        protected internal virtual string SsdlFileOutputFileName
        {
            get { return this.Name + ".ssdl"; }
        }
        protected internal virtual string MslFileOutputFileName
        {
            get { return this.Name + ".msl"; }
        }
        protected internal override string ObjectLayerResourceName
        {
            get { return this.Name + ".Edm.ObjectLayer" + this.LanguageExtension; }
        }
        protected internal override string ObjectLayerOutputFileName
        {
            get { return this.Name + ".Edm.ObjectLayer" + this.LanguageExtension; }
        }
        #endregion

        public void GenerateLoadAndPopulateClientTypes()
        {
            this.GenerateClientTypes = true;
            base.GenerateAndLoadClientTypes();
            this.PopulateClientTypes();
        }

        /// <summary>Populates the client types on the ResourceType</summary>
        protected internal override void PopulateClientTypes()
        {
            string clientNamespace = this.ContextNamespace + "Client";
            Assembly currentAssembly = null;
            //If running under a Friendly feeds test, use the dynamically created assembly and not the current assembly
            if (GenerateClientTypes)
            {
                if (this.ClientTypesAssembly != null)
                {
                    clientNamespace = this.ContextNamespace == "northwind" ? "northwind" : clientNamespace;
                    currentAssembly = this.ClientTypesAssembly;
                }
            }
            else
            {
                currentAssembly = this.GetType().Assembly;
            }
            if (currentAssembly != null)
            {
                _resourceTypeToClientTypeList = CreateResourceTypeToUnderlyingClrTypeMap(currentAssembly, clientNamespace);
                SetResourceTypesToClrTypes(currentAssembly, this.ContextNamespace, _resourceTypeToClientTypeList, true);
            }
        }

        public override System.Configuration.ConnectionStringSettings GetConnectionStringSettingsForProvider(AstoriaWebDataService service, string databaseConnectionString)
        {
            string binFolder = Path.Combine(service.DestinationFolder_Local, "bin");

            if (this.Name == "Northwind" && AstoriaTestProperties.EdmObjectLayer == ServiceEdmObjectLayer.MultiNamespaces)
            {
                return new System.Configuration.ConnectionStringSettings(this.ContextTypeName,
                    "metadata=" + string.Join("|", new string[]
                        {
                            Path.Combine(binFolder, CsdlFileOutputFileName),
                            Path.Combine(binFolder, "Northwind_ext.csdl"),
                            Path.Combine(binFolder, SsdlFileOutputFileName),
                            Path.Combine(binFolder, MslFileOutputFileName)
                        }) + ";" +
                    "provider=System.Data.SqlClient;provider connection string=\"" + databaseConnectionString + "\"",
                    "System.Data.EntityClient");
            }

            return new System.Configuration.ConnectionStringSettings(this.ContextTypeName,
                "metadata=" + string.Join("|", new string[]
                    {
                        Path.Combine(binFolder, CsdlFileOutputFileName),
                        Path.Combine(binFolder, SsdlFileOutputFileName),
                        Path.Combine(binFolder, MslFileOutputFileName)
                    }) + ";" +
                "provider=System.Data.SqlClient;provider connection string=\"" + databaseConnectionString + "\"",
                "System.Data.EntityClient");
        }

        internal override HashSet<Assembly> GetReferencedAssemblies()
        {
            HashSet<Assembly> collection = base.GetReferencedAssemblies();
            collection.Add(typeof(Microsoft.OData.Client.KeyAttribute).Assembly);
            return collection;
        }

        #region helper functions
        //#if !ClientSKUFramework

        public ObjectContext CreateNewTypedObjectContext()
        {
            EntityConnectionStringBuilder EDMConnectionString = new EntityConnectionStringBuilder();
            EDMConnectionString.ProviderConnectionString = this.Database.DatabaseConnectionString;

            string metadataPath = Environment.CurrentDirectory + "\\Workspaces\\Databases\\" + this.Name + "\\DataService\\Bin";
            EDMConnectionString.Metadata = metadataPath;

            EDMConnectionString.Provider = "System.Data.SqlClient";
            ConstructorInfo ci = _underlyingContextType.GetConstructor(new Type[] { typeof(string) });
            ObjectContext context = (ObjectContext)TrustedMethods.InvokeEdmConstructor(ci, EDMConnectionString.ConnectionString);
            return context;
        }

        /// <summary>
        /// Get the object context which loaded the given entity
        /// </summary>
        /// <param name="entity">an object which represents an entity (row)</param>
        /// <returns>the object context which loaded the given entity</returns>
        public ObjectContext GetAssociatedObjectContext(object entity)
        {
            ObjectStateEntry entry = null;
            foreach (var context in this.createdObjectContexts)
            {
                if (context.ObjectStateManager.TryGetObjectStateEntry(entity, out entry))
                {
                    return context;
                }
            }

            return null;
        }

        protected PropertyInfo FindContainerProperty(ObjectContext context, string containerName)
        {
            return context.GetType().GetProperties().Where(propi => propi.Name == containerName).First();
        }
        //#endif

        #endregion



        public override IQueryable ResourceContainerToQueryable(ResourceContainer container)
        {
            // Creating new context every time to avoid breaking more tests. Try to ensure connection every time before creating is just too time-consuming.
            ObjectContext context = CreateNewTypedObjectContext();

            // Here is the way to work around Poco proxy problem. We always keep the 10 lastest contexts we generate and dispose the old ones. In this way we avoid the max pool size failure.
            this.createdObjectContexts.Enqueue(context);
            while (this.createdObjectContexts.Count > MaximumContextsKeptInQueue)
            {
                var ctx = this.createdObjectContexts.Dequeue();
                ctx.Dispose();
            }

            Type contextType = context.GetType();
            //MethodInfo method = contextType.GetMethod("CreateQuery");
            //Get Edm Type
            Type edmType = this._resourceTypeToWorkspaceTypeList[container.BaseType];
            //MethodInfo genMethod = method.MakeGenericMethod(new Type[] { edmType });

            string containerName = container.Name.Replace("]", "]]");
            string propertyName = "System.Data.Objects.ObjectQuery`1[" + edmType.Namespace + "." + edmType.Name + "] " + containerName;
            //object o =genMethod.Invoke(context, new object[] { queryString, new System.Data.Objects.ObjectParameter[] { } });
            PropertyInfo pi = FindContainerProperty(context, container.Name);
            //System.Data.Objects.ObjectQuery`1[northwind.Categories] Categories
            object o = pi.GetValue(context, new object[] { });
            IQueryable containerQueryable = o as IQueryable;

            if (container.HasInterceptorExpression)
            {
                containerQueryable = ApplyQueryInterceptorExpression(container, containerQueryable);
            }
            return containerQueryable;
        }

        protected override ResourceContainer DetermineResourceContainerFromProviderObject(object o)
        {
#if !ClientSKUFramework

            ObjectContext ctx = this.GetAssociatedObjectContext(o);
            ObjectStateEntry entry = null;

            // Use object state entry to get name of entity set
            ctx.ObjectStateManager.TryGetObjectStateEntry(o, out entry);
            string entitySetName = entry.EntityKey.EntitySetName;
            List<ResourceContainer> containers = this.ServiceContainer.ResourceContainers.Where(rc => rc.Name.Equals(entitySetName)).ToList();
            if (containers.Count == 0)
                throw new TestFailedException("Could not find EntitySet:" + entitySetName);
            return containers.First();

#endif
#if ClientSKUFramework
	   return null;
#endif

        }

        public override void PrepareRequest(AstoriaRequest request)
        {
            request.ExtraVerification += ResponseVerification.DefaultVerify;
        }
    }
}

