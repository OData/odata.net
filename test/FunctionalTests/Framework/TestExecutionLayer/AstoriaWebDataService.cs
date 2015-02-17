//---------------------------------------------------------------------
// <copyright file="AstoriaWebDataService.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Data.Test.Astoria.Util; //Account
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using Microsoft.Test.ModuleCore;

namespace System.Data.Test.Astoria.TestExecutionLayer
{
    /// <summary>Use this class to setup and connect to an Astoria web data service.</summary>
    [DebuggerVisualizer("AstoriaWebDataService: {URI}")]
    public class IISWebDataService : AstoriaWebDataService
    {
        /// <summary>Initializes a new <see cref="AstoriaWebDataService"/> instance.</summary>
        /// <param name="WebDataServicePrefixName"></param>
        /// <param name="Database"></param>
        public IISWebDataService(Workspace workspace, string webDataServicePrefixName, string machineName, AstoriaDatabase database)
            : base(workspace, webDataServicePrefixName, machineName, database)
        {

        }

        public static Func<WebConfig> CreateWebConfig;
        protected virtual void DeployWebConfig()
        {
            string path = Path.Combine(DestinationFolder, "Web.Config");
            WebConfig wc = null;
            if (CreateWebConfig != null)
            {
                wc = CreateWebConfig();
            }
            else
            {
                wc = new WebConfig();
            }
            wc._TrustLevel = AstoriaTestProperties.ServiceTrustLevel.ToString();
            //wc._AuthMode = AstoriaTestProperties.HostAuthenicationMethod;
            wc._TransferMode = AstoriaTestProperties.TransferMode;
            wc._MaxBufferSize = AstoriaTestProperties.MaxBufferSize;
            wc._MaxReceivedMessageSize = AstoriaTestProperties.MaxReceivedMessageSize;
            wc._MaxRequestLength = AstoriaTestProperties.MaxRequestLength;
            wc._TransportSecurityMode = AstoriaTestProperties.TransportSecurityMode;
            wc._CloseTimeout = AstoriaTestProperties.CloseTimeout;
            wc._OpenTimeout = AstoriaTestProperties.OpenTimeout;
            wc._ReceiveTimeout = AstoriaTestProperties.ReceiveTimeout;
            wc._SendTimeout = AstoriaTestProperties.SendTimeout;
            wc._AspNetCompatibilityEnabled = AstoriaTestProperties.AspNetCompatibilityEnabled;
	    wc._CompilerOptions += " /Define:" + this.Workspace.DataLayerProviderKind.ToString();

            Versioning.Server.AdjustWebConfig(wc);

            if (this.Database != null)
            {
                wc.ConnectionStringSettings = this.Workspace.GetConnectionStringSettingsForProvider(this, this.Database.DatabaseConnectionString);
            }
            wc.Save(path);
        }

        protected override void CreateWebService(bool verify)
        {
            try
            {
                if (AstoriaTestProperties.Host == Host.LocalIIS)
                {
                    DestinationFolder = Path.Combine(Environment.GetEnvironmentVariable("SystemDrive") + @"\inetpub\wwwroot", this.WebDataServiceName);
                    DestinationFolder_Local = DestinationFolder;
                }
                SourceFolder = Path.Combine(Path.Combine(Environment.CurrentDirectory, databaseFolder), this.WebDataServicePrefixName);
                SourceFolder = Path.Combine(SourceFolder, "DataService");
                string sWindowsAuth = "";
                string sAnonymousAuth = "";

                AstoriaTestLog.WriteLineIgnore("Creating IIS webservice on: " + this.MachineName);
                this.CopySourceFolderToHost();

                DeployWebConfig();

                // if not full trust, then compiler options aren't allowed, so we need to add #defines to each file manually
                if (AstoriaTestProperties.ServiceTrustLevel != TrustLevel.Full)
                {
                    List<string> toDefine = new List<string>() { Workspace.DataLayerProviderKind.ToString() };
                    if (!Versioning.Server.SupportsV2Features)
                    {
                        toDefine.Add("ASTORIA_PRE_V2");
                    }

                    string defines = string.Join(Environment.NewLine, toDefine.Select(d => "#define " + d).ToArray());

                    foreach (string path in Directory.GetFiles(DestinationFolder, "*.cs", SearchOption.AllDirectories))
                    {
                        string fileContent = File.ReadAllText(path);
                        fileContent = defines + Environment.NewLine + fileContent;
                        File.WriteAllText(path, fileContent);
                    }
                }

                Assembly resourceAssembly = this.GetType().Assembly;
                IOUtil.FindAndWriteResourceToFile(resourceAssembly, Path.Combine(DestinationFolder, "CreateVDir.vbs"));
                IOUtil.FindAndWriteResourceToFile(resourceAssembly, Path.Combine(DestinationFolder, "..\\DeleteVDir.vbs"));
                if (AstoriaTestProperties.HostAuthenicationMethod == "Windows")
                {
                    sWindowsAuth = "True";
                    sAnonymousAuth = "False";
                }
                else if (AstoriaTestProperties.HostAuthenicationMethod == "None")
                {
                    sWindowsAuth = "False";
                    sAnonymousAuth = "True";
                }

                string args = String.Format("{0} {1} {2} {3} {4}", Path.Combine(DestinationFolder_Local, "CreateVDir.vbs"), this.WebDataServiceName, DestinationFolder_Local, sAnonymousAuth, sWindowsAuth);
                RunProcess("cscript", args);
                Thread.Sleep(2000);

                // set up service uris
                if (AstoriaTestProperties.TransportSecurityMode.Equals("transport", StringComparison.InvariantCultureIgnoreCase))
                {
                    ServiceRootUri = Uri.UriSchemeHttps + "://" + this.MachineName + "/" + this.WebDataServiceName + "/";
                }
                else
                {
                    ServiceRootUri = Uri.UriSchemeHttp + "://" + this.MachineName + "/" + this.WebDataServiceName + "/";
                }
                ServiceUri = ServiceRootUri + Workspace.WebServiceFileName;
                if (AstoriaTestProperties.UseDomainServices)
                {
                    ServiceUri = ServiceUri + "/dataservice/";
                    AstoriaTestLog.WriteLineIgnore("OData Endpoint is at : " + ServiceUri);
                }
                if (verify)
                    VerifyService();
            }
            catch (Exception e)
            {
                if (AstoriaTestProperties.Host == Host.LocalIIS)
                {
                    throw new TestFailedException("Could not create IIS web service on local host", null, null, e);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                base.Dispose(disposing);
                this.Delete();
            }
        }
        public void Delete()
        {
            AstoriaTestLog.WriteLineIgnore("-------Delete WebdataService ---------");
            AstoriaTestLog.WriteLineIgnore("-------     URI: {0}---------", this.ServiceUri);
            string args = String.Format("{0} {1}", Path.Combine(DestinationFolder_Local, "..\\DeleteVDir.vbs"), this.WebDataServiceName);
            RunProcess("cscript", args);
        }

        private bool ShouldWorkaroundDueToVistaUAC()
        {
            if (AstoriaTestProperties.Host == Host.LocalIIS)
            {
                AstoriaTestLog.TraceLine("Host=LocalIIS");
                if (!ProcessHelper.IsVistaOrLater())
                {
                    AstoriaTestLog.TraceLine("IsVista=False");
                    return false;
                }
                AstoriaTestLog.TraceLine("IsVista=True");
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
        /// <summary>
        /// Runs the specifed application with the given arguments on the Machine
        /// for this service.
        /// </summary>
        /// <param name="appString">Application to run.</param>
        /// <param name="argString">Argument to application.</param>
        private void RunProcess(string appString, string argString)
        {
            if (ShouldWorkaroundDueToVistaUAC())
            {
                Process[] processes = Process.GetProcessesByName("Commander.Server");
                if (processes.Length > 0)
                {
                    ProcessHelper.UseCommander(Environment.MachineName,
                        delegate(Commander.RemoteServer remoteServer)
                        {
                            Commander.RemoteExecutableResults results = remoteServer.ExecuteScript(argString);
                            if (results.ExitCode != 0)
                                throw new TestFailedException("Unable to run create website script:\n" + results.Output);
                            AstoriaTestLog.TraceLine(results.Output);
                        });
                }
                else
                    throw new TestFailedException("Expected a RemoteServer program called Commander.Server.exe to be running");
            }
            else if (ProcessHelper.IsLocalMachine(this.MachineName))
            {
                // Run locally.
                ProcessStartInfo processStart = new ProcessStartInfo(appString, argString);
                processStart.UseShellExecute = false;
                processStart.CreateNoWindow = true;

                AstoriaTestLog.WriteLineIgnore(appString);
                AstoriaTestLog.WriteLineIgnore(argString);
                using (Process process = Process.Start(processStart))
                {
                    if (process != null)
                    {
                        const int timeoutMilliseconds = 20 * 1000;
                        process.WaitForExit(timeoutMilliseconds);
                    }
                }
            }
        }
    }
}
