//---------------------------------------------------------------------
// <copyright file="WebConfig.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Xml;
using System.Xml.Linq;

#if !ClientSKUFramework
using Microsoft.OData.Service.Configuration;
using System.ServiceModel.Configuration;
using System.Web.Configuration;
#endif

namespace System.Data.Test.Astoria
{
    // Dynamically constructs Web.Config
    [SecuritySafeCritical]
    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    public class WebConfig
    {
        // Common settings.
        public string _AuthMode
        {
            get
            {
                return AstoriaTestProperties.HostAuthenicationMethod;
            }
        }
        public bool _Win7Security
        {
            get
            {
                return Environment.OSVersion.VersionString == "Microsoft Windows NT 6.1.7000.0";
            }
        }

        private const byte mask = 15;
        private const string hex = "0123456789ABCDEF";
        public string _TrustLevel = "Full";                 // "Full", "Partial", "Medium"
        public string _TransferMode = "Buffered";           // "Buffered", "Streamed", etc.
        public int _MaxBufferSize = 2147483647;             // int
        public int _MaxReceivedMessageSize = 2147483647;    // int
        public int _MaxRequestLength = 2097151;             // int
        public string _TransportSecurityMode = "TransportCredentialOnly";   // "Transport", "TransportCredentialOnly", "None"
        public string _CloseTimeout = "00:01:00";           // TimeSpan
        public string _OpenTimeout = "00:01:00";            // TimeSpan
        public string _ReceiveTimeout = "00:10:00";         // TimeSpan
        public string _SendTimeout = "00:01:00";            // TimeSpan
        public string _CompilerOptions = "/warnaserror-";   // options like /Define:*
        public bool _AspNetCompatibilityEnabled = true;


        // Default settings, different for each branch.
        public string _AssemblyDomain = "Microsoft";        // "Microsoft" or "System"
        public string _AssemblyDomainVersion = DataFxAssemblyRef.DataFxAssemblyVersion;
        public string _AssemblyPublicKeyToken = "31bf3856ad364e35";
        public string _AssemblyVersion = "4.0.0.0";         // "4.0.0.0"
        public string _EntityAssemblyVersion = "4.0.0.0";   // "4.0.0.0"
        public string _CompilerAssemblyVersion = "4.0.0.0"; // "2.0.0.0" for .NET 3.5, "4.0.0.0" for .NET 4.0
        public string _CompilerVersion = "4.0";             // "3.5", "4.0", ...
        public bool _CompilationDebug = false;              // debug compilation mode
        public string _TFM = null;                          // target framework moniker: null or ".NETFramework,Version=v4.0.0.0"

        // Connection string settings.
        public ConnectionStringSettings ConnectionStringSettings = null;

        // configuration objects used to modify wcf settings
        protected ExeConfigurationFileMap configFile;
        protected System.Configuration.Configuration config;


        // make a copy of the generic web config template and start to modify the settings
        public virtual void LoadGenericConfig(string filePath)
        {
            configFile = new ExeConfigurationFileMap();

            Assembly testFrameworkAssembly = this.GetType().Assembly;
            string configFileResourceName = "Microsoft.Data.Test.Workspaces.Generic.Web.config";
#if ClientSKUFramework
        configFileResourceName ="Microsoft.Data.Test.Generic.Web.config";
#endif
            IOUtil.CopyResourceToFile(testFrameworkAssembly, configFileResourceName, filePath);
            configFile.ExeConfigFilename = filePath;
            config = ConfigurationManager.OpenMappedExeConfiguration(configFile, ConfigurationUserLevel.None);
        }


        // update the connection strings in web.config
        public void ConnectionStrings()
        {
            // create a new ConnectionStrings object with the correct setting
            config.ConnectionStrings.ConnectionStrings.Clear();
            if (ConnectionStringSettings != null)
            {
                config.ConnectionStrings.ConnectionStrings.Add(ConnectionStringSettings);
            }
        }


        // update the configuration options in <system.web> section with the specified setting
        [SecuritySafeCritical]
        [PermissionSet(Security.Permissions.SecurityAction.Assert, Unrestricted = true)]
        public virtual void SystemWeb()
        {
#if !ClientSKUFramework

            // update MaxRequestLength in system.web/httpRuntime section
            HttpRuntimeSection httpRuntimeSection = (HttpRuntimeSection)config.GetSection("system.web/httpRuntime");
            httpRuntimeSection.MaxRequestLength = _MaxRequestLength;

            // update system.web/trust
            TrustSection trustSection = (TrustSection)config.GetSection("system.web/trust");
            trustSection.SectionInformation.SetRawXml(string.Format(trustSection.SectionInformation.GetRawXml(), _TrustLevel));

            CompilationSection compilationSection = (CompilationSection)config.GetSection("system.web/compilation");

            // update system.web/compilation/debug
            compilationSection.Debug = _CompilationDebug;

            // update assembly strings in system.web/compilation section
            foreach (AssemblyInfo assemblyItem in compilationSection.Assemblies)
            {
                var version = assemblyItem.Assembly.StartsWith("System.Data.Entity") ? _EntityAssemblyVersion : _AssemblyVersion;
                if (assemblyItem.Assembly.Contains("Test"))
                {
                    // Test assemblies have the same version 
                    version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                }

                assemblyItem.Assembly = String.Format(assemblyItem.Assembly, version, _AssemblyDomain, _AssemblyDomainVersion, _AssemblyPublicKeyToken);
            }

            // Setup the correct buildProvider settings if necessary
            bool shouldOverrideMachineWebConfigSettings = ShouldOverrideExistingBuildProviderSettings(_AssemblyDomain, _AssemblyDomainVersion, _AssemblyPublicKeyToken);

            if (shouldOverrideMachineWebConfigSettings)
            {
                // update the handler versions
                compilationSection.FolderLevelBuildProviders.Clear();
                var folderLevelBuildProvider = new FolderLevelBuildProvider("DataServiceBuildProvider", "Microsoft.OData.Service.BuildProvider.DataServiceBuildProvider");
                compilationSection.FolderLevelBuildProviders.Add(folderLevelBuildProvider);
            }

            // update authentication mode in system.web/authentication
            AuthenticationSection authenticationSection = (AuthenticationSection)config.GetSection("system.web/authentication");
            switch (_AuthMode.ToLower())
            {
                case "forms":
                    authenticationSection.Mode = System.Web.Configuration.AuthenticationMode.Forms;
                    break;
                case "passport":
                    authenticationSection.Mode = System.Web.Configuration.AuthenticationMode.Passport;
                    break;
                case "windows":
                    authenticationSection.Mode = System.Web.Configuration.AuthenticationMode.Windows;
                    break;
                default:
                    authenticationSection.Mode = System.Web.Configuration.AuthenticationMode.None;
                    break;
            }

            // update httpHandler versions in system.web/httpHandlers section
            HttpHandlersSection httpHandlersSection = (HttpHandlersSection)config.GetSection("system.web/httpHandlers");
            foreach (HttpHandlerAction httpHandler in httpHandlersSection.Handlers)
            {
                httpHandler.Type = String.Format(httpHandler.Type, _AssemblyVersion);
            }
#endif
        }

        public static bool ShouldOverrideExistingBuildProviderSettings(Assembly systemDataDesignAssembly)
        {
            var assemblyName = systemDataDesignAssembly.GetName();
            string name = assemblyName.FullName;
            string version = assemblyName.Version.ToString();
            string publicKeyToken = ConvertPublicKeyTokenToString(systemDataDesignAssembly);
            return ShouldOverrideExistingBuildProviderSettings(name, version, publicKeyToken);
        }

        private static string ConvertPublicKeyTokenToString(Assembly assembly)
        {
            var pkt = new StringBuilder();
            foreach (byte b in assembly.GetName().GetPublicKeyToken())
            {
                pkt.Append(hex[b / 16 & mask]);
                pkt.Append(hex[b & mask]);
            }

            return pkt.ToString();
        }

        private static bool ShouldOverrideExistingBuildProviderSettings(string assemblyDomain, string assemblyVersion, string assemblyPublicKeyToken)
        {
            bool shouldOverrideMachineWebConfigSettings = false;
            if (assemblyDomain.StartsWith("System."))
            {
                // if the assembly Name is different then we need to override
                shouldOverrideMachineWebConfigSettings = true;
            }
            else if (assemblyVersion != DataFxAssemblyRef.FXAssemblyVersion)
            {
                shouldOverrideMachineWebConfigSettings = true;
            }
            else if (assemblyPublicKeyToken != DataFxAssemblyRef.EcmaPublicKeyToken)
            {
                // if the assembly PublicKeyToken is different then we need to override
                shouldOverrideMachineWebConfigSettings = true;
            }
            return shouldOverrideMachineWebConfigSettings;
        }


        // update the compiler information in <system.codedom> section
        public void SystemCodeDom()
        {
            // update the compiler versions            
            System.Configuration.ConfigurationSection systemCodeDomSection = config.GetSection("system.codedom") as System.Configuration.ConfigurationSection;
            XElement codeDom = XElement.Parse(systemCodeDomSection.SectionInformation.GetRawXml());

            XElement compilers = new XElement("compilers");
            codeDom.Add(compilers);
            XElement compiler = new XElement("compiler");
            compilers.Add(compiler);
            compiler.Add(new XAttribute("language", "c#;cs;csharp"));
            compiler.Add(new XAttribute("extension", ".cs"));
            compiler.Add(new XAttribute("type", "Microsoft.CSharp.CSharpCodeProvider, System, Version=" + _CompilerAssemblyVersion + ", Culture=neutral, PublicKeyToken=b77a5c561934e089"));
            XElement providerOption = new XElement("providerOption");
            compiler.Add(providerOption);
            providerOption.Add(new XAttribute("name", "CompilerVersion"));
            providerOption.Add(new XAttribute("value", "v" + _CompilerVersion));

            // if _TrustLevel is not "Full", do not specify compilerOptions or waringLevel
            if (_TrustLevel.ToLower().Equals("full"))
            {
                string compilerOptions = _CompilerOptions;

                if (!Versioning.Server.SupportsV2Features)
                    compilerOptions += " /Define:ASTORIA_PRE_V2";

                compiler.Add(new XAttribute("compilerOptions", compilerOptions));
                compiler.Add(new XAttribute("warningLevel", 4));
            }

            systemCodeDomSection.SectionInformation.SetRawXml(codeDom.ToString());
        }


        // update the configuration options in <system.webServer> section
        public void SystemWebServer()
        {
            System.Configuration.ConfigurationSection webServerSecton = config.GetSection("system.webServer");

            // some extra settings for Win7
            String extraWin7Nodes = "";
            if (_Win7Security)
            {
                extraWin7Nodes = @"<authentication>
                    <windowsAuthentication useKernelMode='true'>
                        <extendedProtection tokenChecking='None' />
                    </windowsAuthentication>
                </authentication>";
            }
            // update the handler versions
            webServerSecton.SectionInformation.SetRawXml(string.Format(webServerSecton.SectionInformation.GetRawXml(), _AssemblyVersion, extraWin7Nodes));
        }


        // update the configuration options in <system.serviceModel> section
        public virtual void SystemServiceModel()
        {
#if !ClientSKUFramework

            ServiceHostingEnvironmentSection serviceHostingEnvironmentSection = config.GetSection("system.serviceModel/serviceHostingEnvironment") as ServiceHostingEnvironmentSection;

            // serviceHostingEnvironment -> aspNetCompatibilityEnabled
            serviceHostingEnvironmentSection.AspNetCompatibilityEnabled = _AspNetCompatibilityEnabled;

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
                        // webHttpBinding -> binding -> transferMode
                        switch (_TransferMode.ToLower())
                        {
                            case "streamed":
                                webHttpBindingElement.TransferMode = System.ServiceModel.TransferMode.Streamed;
                                break;
                            case "streamedrequest":
                                webHttpBindingElement.TransferMode = System.ServiceModel.TransferMode.StreamedRequest;
                                break;
                            case "streamedresponse":
                                webHttpBindingElement.TransferMode = System.ServiceModel.TransferMode.StreamedResponse;
                                break;
                            default:
                                webHttpBindingElement.TransferMode = System.ServiceModel.TransferMode.Buffered;
                                break;
                        }
                        // webHttpBinding -> binding -> MaxBufferSize, MaxReceivedMessageSize, and timeout's
                        webHttpBindingElement.MaxBufferSize = _MaxBufferSize;
                        webHttpBindingElement.MaxReceivedMessageSize = _MaxReceivedMessageSize;
                        webHttpBindingElement.CloseTimeout = TimeSpan.Parse(_CloseTimeout);
                        webHttpBindingElement.OpenTimeout = TimeSpan.Parse(_OpenTimeout);
                        webHttpBindingElement.ReceiveTimeout = TimeSpan.Parse(_ReceiveTimeout);
                        webHttpBindingElement.SendTimeout = TimeSpan.Parse(_SendTimeout);

                        // webHttpBinding -> binding -> security mode
                        switch (_TransportSecurityMode.ToLower())
                        {
                            case "none":
                                webHttpBindingElement.Security.Mode = System.ServiceModel.WebHttpSecurityMode.None;
                                break;
                            case "transport":
                                webHttpBindingElement.Security.Mode = System.ServiceModel.WebHttpSecurityMode.Transport;
                                break;
                            default:
                                webHttpBindingElement.Security.Mode = System.ServiceModel.WebHttpSecurityMode.TransportCredentialOnly;
                                break;
                        }

                        // webHttpBinding -> binding -> security ->transport
                        WebHttpSecurityElement webHttpSecurityElement = webHttpBindingElement.Security;
                        switch (_AuthMode.ToLower())
                        {
                            case "windows":
                                webHttpSecurityElement.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Windows;
                                webHttpSecurityElement.Transport.ProxyCredentialType = System.ServiceModel.HttpProxyCredentialType.Windows;
                                break;
                            case "ntlm":
                                webHttpSecurityElement.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Ntlm;
                                webHttpSecurityElement.Transport.ProxyCredentialType = System.ServiceModel.HttpProxyCredentialType.Ntlm;
                                break;
                            case "basic":
                                webHttpSecurityElement.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;
                                webHttpSecurityElement.Transport.ProxyCredentialType = System.ServiceModel.HttpProxyCredentialType.Basic;
                                break;
                            case "digest":
                                webHttpSecurityElement.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Digest;
                                webHttpSecurityElement.Transport.ProxyCredentialType = System.ServiceModel.HttpProxyCredentialType.Digest;
                                break;
                            default:
                                webHttpSecurityElement.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.None;
                                webHttpSecurityElement.Transport.ProxyCredentialType = System.ServiceModel.HttpProxyCredentialType.None;
                                break;
                        }
                    }
                }
            }
#endif
        }


        // Print out the WCF configuration option values if different from default setting
        public void LogConfigSetting()
        {
            if (!_TransferMode.Equals("Buffered") ||
                _MaxBufferSize != 2147483647 ||
                _MaxReceivedMessageSize != 2147483647 ||
                _MaxRequestLength != 2097151 ||
                !_TransportSecurityMode.Equals("TransportCredentialOnly") ||
                !_CloseTimeout.Equals("00:01:00") ||
                !_OpenTimeout.Equals("00:01:00") ||
                !_ReceiveTimeout.Equals("00:10:00") ||
                !_SendTimeout.Equals("00:01:00") ||
                !_AspNetCompatibilityEnabled)
            {
                AstoriaTestLog.WriteLineIgnore("WCF Configuration Settings:");
                AstoriaTestLog.WriteLineIgnore("_TransferMode: " + _TransferMode);
                AstoriaTestLog.WriteLineIgnore("_MaxBufferSize: " + _MaxBufferSize);
                AstoriaTestLog.WriteLineIgnore("_MaxReceivedMessageSize: " + _MaxReceivedMessageSize);
                AstoriaTestLog.WriteLineIgnore("_MaxRequestLength: " + _MaxRequestLength);
                AstoriaTestLog.WriteLineIgnore("_TransportSecurityMode: " + _TransportSecurityMode);
                AstoriaTestLog.WriteLineIgnore("_CloseTimeout: " + _CloseTimeout);
                AstoriaTestLog.WriteLineIgnore("_OpenTimeout: " + _OpenTimeout);
                AstoriaTestLog.WriteLineIgnore("_ReceiveTimeout: " + _ReceiveTimeout);
                AstoriaTestLog.WriteLineIgnore("_SendTimeout: " + _SendTimeout);
                AstoriaTestLog.WriteLineIgnore("_AspNetCompatibilityEnabled: " + _AspNetCompatibilityEnabled);
            }
        }


        // if_TFM is not null, update file web.config directly as configuration API does not allow "targetFrameworkMoniker"
        public void fixTFM(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            for (int j = 0; j < lines.Length; j++)
            {
                int i = lines[j].IndexOf("<compilation debug=");
                if (i >= 0)
                {
                    lines[j] = lines[j].Substring(0, i) + "<compilation targetFrameworkMoniker=\"" + _TFM + "\" " + lines[j].Substring(i + 13);
                }
            }
            File.WriteAllLines(filePath, lines);
        }


        // Enable or disable replace function in web.config
        public virtual void SetReplaceFunctionFeature(string filePath, bool includeWcfDataServicesGroupInWebConfig, bool replaceFunctionEnabled)
        {
#if !ClientSKUFramework

            this.configFile = new ExeConfigurationFileMap { ExeConfigFilename = Path.Combine(filePath, "Web.Config") };
            this.config = ConfigurationManager.OpenMappedExeConfiguration(this.configFile, ConfigurationUserLevel.None);

            this.config.SectionGroups.Remove("wcfDataServices");

            if (includeWcfDataServicesGroupInWebConfig)
            {
                DataServicesSectionGroup dataServicesSectionGroup = new DataServicesSectionGroup();
                this.config.SectionGroups.Add("wcfDataServices", dataServicesSectionGroup);

                DataServicesFeaturesSection dataServicesFeaturesSection = new DataServicesFeaturesSection();
                dataServicesSectionGroup.Sections.Add("features", dataServicesFeaturesSection);

                dataServicesFeaturesSection.ReplaceFunction.Enable = replaceFunctionEnabled;
            }

            config.Save();
#endif
        }

        // Load the web config template, modify WCF configuration settings, and save as "web.config".
        public virtual void Save(string filePath)
        {
            LoadGenericConfig(filePath);

            ConnectionStrings();
            SystemWeb();
            SystemCodeDom();
            SystemWebServer();
            SystemServiceModel();

            config.Save();

            if (!string.IsNullOrEmpty(_TFM))
            {
                fixTFM(filePath);
            }

            LogConfigSetting();
        }
    }
}
