//---------------------------------------------------------------------
// <copyright file="AstoriaTestProperties.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    #region Namespaces

    using System.Collections.Generic;
    using System.Data.Test.Astoria.Util;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.KoKoMo;
    using Microsoft.Test.ModuleCore;
    using System.Data.Test.Astoria.FullTrust;

    #endregion Namespaces

    /// <summary>Provides values to describe the runtime environment.</summary>
    public enum TestRuntimeEnvironment
    {
        /// <summary>Tests are being run within the VS IDE with unit test integration.</summary>
        VisualStudioIde,

        /// <summary>Check-in tests are being run in a </summary>
        CheckinSuites,

        /// <summary></summary>
        Lab,
    }

    public enum ClientAccessPolicyFileType
    {
        CallerAnyDomain,
        CallerDifferentDomain,
        HeadersBlocked,
        OnlyServiceRequestAllowed,
        SubPathsNotallowed,
        NoCapFile
    }

    public enum ClientOSLang
    {
        ENUS, GEGE
    }

    public enum ClientOS
    {
        MAC1052, MAC10411, MAC1048, XPHOME, XPStarter, WIN2K, VistaStarter, Vista, XpPro, Win2k3, Local
    }

    public enum TrustLevel
    {
        Full, Medium, High
    }

    public enum ClientBrowser
    {
        IE5, IE6, IE7, IE8, FF15, FF2, FF3, SF2, SF3, Chrome
    }
    public enum TestLanugage
    {
        VB, CSHARP
    }

    public enum Host
    {
        IIS51, IIS6, IIS7, Cassini, Debug, WebServiceHost, WebServiceHostRemote, LocalIIS, IDSH, IDSH2
    }

    public enum ClientEnum
    {
        SILVERLIGHT, XMLHTTP, HTTP, DESKTOP
    }

    public enum ServiceEdmObjectLayer
    {
        Default, PocoWithProxy, PocoWithoutProxy, STE, MultiNamespaces
    }

    ////////////////////////////////////////////////////////
    // AstoriaTestProperties
    ////////////////////////////////////////////////////////
    public static class AstoriaTestProperties
    {
        //Data
        private static TestRuntimeEnvironment? _runtimeEnvironment;
        private static string _dataProviderMachineName;
        private static string _hostMachineName;
        private static string _clientMachineName;
        private static string _designMachineName;

        private static int? _maxPriority = null;
        private static int? _minPriority = null;
        private static DataLayerProviderKind[] _dataLayerProviderKinds;
        private static Dictionary<string, string> _overriddenProperties;
        private static string host;

        //Construction
        public static void Init()
        {
            //ReInitialize the Seed, everytime (so we get randomness without having to shutdown)
            string seed = ResolveProperty("Seed");
            if (seed == null)
                AstoriaTestProperties.Seed = unchecked((int)DateTime.Now.Ticks);
            else
                AstoriaTestProperties.Seed = Int32.Parse(seed);
        }

        public static void Dispose()
        {
            //Reset
        }
        public static bool DebugTrace
        {
            get
            {
                bool value;
                if (bool.TryParse(ResolvePropertyOrDefault("DebugTrace", "false"), out value))
                    return value;
                return false;
            }
        }

        private static T ConvertToEnum<T>(string value, T defaultValue)
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            T converted = Enum.GetValues(typeof(T)).Cast<T>().SingleOrDefault(e => e.ToString().Equals(value, StringComparison.InvariantCultureIgnoreCase));
            if (converted == null)
            {
                AstoriaTestLog.FailAndThrow(String.Format("Could not convert value '{0}' into enum type '{1}'", value, typeof(T)));
            }

            return converted;
        }

        public static TestLanugage ConverToTestLanguage(string language)
        {
            return ConvertToEnum<TestLanugage>(language, TestLanugage.CSHARP);

        }
        public static ClientOSLang ConverToClientOSLang(string param)
        {
            ClientOSLang lang = ClientOSLang.ENUS;
            if (param != null)
            {
                switch (param.ToLower())
                {
                    case "english - united states":
                        lang = ClientOSLang.ENUS;
                        break;
                }
            }
            return lang;
        }

        public static TrustLevel ConvertToTrustLevel(string trustLevel)
        {
            return ConvertToEnum<TrustLevel>(trustLevel, TrustLevel.Full);
        }

        public static ServiceEdmObjectLayer ConvertToEdmObjectLayer(string objectLayer)
        {
            return ConvertToEnum<ServiceEdmObjectLayer>(objectLayer, ServiceEdmObjectLayer.Default);
        }

        public static Host ConvertToHost(string host)
        {
            return ConvertToEnum<Host>(host, Host.Debug);
        }

        public static WindowsCompatFlag ConvertToWindowsCompatFlag(string windowsCompatFlag)
        {
            return ConvertToEnum<WindowsCompatFlag>(windowsCompatFlag, WindowsCompatFlag.None);
        }

        public static ClientBrowser ConvertToClientBrowser(string client)
        {
            if (client != null && client.IndexOf('-') > -1)
            {
                client = client.Split('-')[1];
            }
            return ConvertToEnum<ClientBrowser>(client, ClientBrowser.IE7);
        }

        public static ClientEnum ConvertToClient(string client)
        {
            if (client != null && client.IndexOf('-') > -1)
            {
                client = client.Split('-')[0];
            }
            return ConvertToEnum<ClientEnum>(client, ClientEnum.HTTP);
        }

        public static ClientOS ConverToClientOS(string osName)
        {
            if (osName != null)
                osName = osName.Replace(".", null);

            return ConvertToEnum<ClientOS>(osName, ClientOS.Local);
        }

        public static int MaxPriority
        {
            get
            {
                if (!_maxPriority.HasValue)
                {
                    string result = ResolvePropertyOrDefault("MaxPriority", null);
                    if (result != null)
                    {
                        //NOTE: the only time this is cached is if a result is actually set
                        //DO not cache this setting
                        return int.Parse(result);
                    }
                    else
                    {
                        //NOTE: the only time this is cached is if a result is actually set
                        //DO not cache this setting
                        return int.MaxValue;
                    }
                }
                return _maxPriority.Value;
            }
            set
            {
                _maxPriority = value;
            }
        }

        public static int MinPriority
        {
            get
            {
                if (!_minPriority.HasValue)
                {
                    string result = ResolvePropertyOrDefault("MinPriority", null);
                    if (result != null)
                    {
                        //NOTE: the only time this is cached is if a result is actually set
                        //DO not cache this setting
                        return int.Parse(result);
                    }
                    else
                    {
                        //NOTE: the only time this is cached is if a result is actually set
                        //DO not cache this setting
                        return int.MinValue;
                    }
                }
                return _minPriority.Value;
            }
            set
            {
                _minPriority = value;
            }
        }


        public static bool IsRemoteClient
        {
            get
            {
                return (Client == ClientEnum.SILVERLIGHT || IsRemoteVersioning);
            }
        }

        public static bool IsRemoteVersioning
        {
            get { return !String.IsNullOrEmpty(ClientVersion) && (ClientVersion != Versioning.DefaultAstoriaVersion || DesignVersion != Versioning.DefaultAstoriaVersion); }
        }

        public static bool IsLocalHost
        {
            get
            {
                if (Host == Host.Cassini || Host == Host.LocalIIS || Host == Host.WebServiceHost)
                    return true;
                return false;
            }
        }
        public static ServiceEdmObjectLayer EdmObjectLayer
        {
            get
            {
                return ConvertToEdmObjectLayer(ResolvePropertyOrDefault("EdmObjectLayer", null));
            }
        }
        public static TrustLevel ServiceTrustLevel
        {
            get
            {
                // default now handled by ConvertToTrustLevel
                return ConvertToTrustLevel(ResolvePropertyOrDefault("TrustLevel", null));
            }
        }
        public static TrustLevel ClientTrustLevel
        {
            get
            {
                return ConvertToTrustLevel(ResolvePropertyOrDefault("ClientTrustLevel", null));
            }
        }
        public static bool RunXDomain
        {
            get
            {
                string result = ResolvePropertyOrDefault("RunDomain", "Same");
                bool runXDomain = result.CompareTo("Same") != 0;
                return runXDomain;
            }
        }
        public static bool RunOutOfBrowser
        {
            get
            {
                string result = ResolvePropertyOrDefault("ClientHost", "Browser");
                bool runOutOfBrowser = result.CompareTo("Browser") != 0;
                return runOutOfBrowser;
            }
        }
        public static string WebServiceHostTargetFramework
        {
            get
            {
                return ResolvePropertyOrDefault("WebServiceHostTargetFramework", null);
            }
        }
        public static WindowsCompatFlag WindowsCompatFlag
        {
            get
            {
                string windowsCompatFlagStr = ResolvePropertyOrDefault("WindowsCompatFlag", null);
                if (windowsCompatFlagStr == null)
                {
                    return Astoria.WindowsCompatFlag.None;
                }

                return ConvertToWindowsCompatFlag(windowsCompatFlagStr);
            }
        }
        public static bool SetupRunOnWebServer
        {
            get
            {
                bool value;
                if (bool.TryParse(ResolvePropertyOrDefault("SetupRunOnWebServer", "false"), out value))
                    return value;
                return false;
            }
        }

        public static bool IsLabRun
        {
            get
            {
                bool value;
                if (bool.TryParse(ResolvePropertyOrDefault("IsLabRun", "false"), out value))
                    return value;
                return false;
            }
        }

        public static ClientAccessPolicyFileType PolicyFileType
        {
            get;
            set;
        }

        public static TestLanugage AstoriaClientLanguage
        {
            get
            {
                // default now handled by ConverToTestLanguage
                return ConverToTestLanguage(ResolvePropertyOrDefault("AstoriaClientLanguage", null));
            }
        }

        public static String InitString(String property)
        {
            return AstoriaTestProperties.Properties("Alias/InitString/" + property);
        }

        public static void OverrideProperty(string propertyName, string value)
        {
            if (_overriddenProperties == null)
                _overriddenProperties = new Dictionary<string, string>();
            _overriddenProperties[propertyName] = value;
        }

        public static void ClearOverriddenProperties()
        {
            // this will null out the dictionary and anything else that has been set
            foreach (FieldInfo field in typeof(AstoriaTestProperties).GetFields(BindingFlags.Static | BindingFlags.NonPublic))
            {
                field.SetValue(null, null);
            }
        }

        /// <summary>Resolves a property from the command line or LTM-alias.</summary>
        /// <param name="propertyName">Name of property to resolve.</param>
        /// <returns>The property value, null if not defined.</returns>
        public static string ResolveProperty(string propertyName)
        {
            string result;
            if (_overriddenProperties == null || !_overriddenProperties.TryGetValue(propertyName, out result))
            {
                result = AstoriaTestProperties.Properties("CommandLine/" + propertyName);
                if (result == null)
                {
                    result = InitString(propertyName);
                }
            }

            return result;
        }

        /// <summary>Resolves a property from the command line or LTM-alias.</summary>
        /// <param name="propertyName">Name of property to resolve.</param>
        /// <param name="defaultValue">Value to return if the property is not defined.</param>
        /// <returns>The property value, <paramref name="defaultValue"/> if not defined.</returns>
        public static string ResolvePropertyOrDefault(string propertyName, string defaultValue)
        {
            string result = ResolveProperty(propertyName);
            if (result == null)
            {
                result = defaultValue;
            }

            return result;
        }

        public static DataLayerProviderKind[] DataLayerProviderKinds
        {
            get
            {
                if (_dataLayerProviderKinds == null)
                {
                    string result = ResolvePropertyOrDefault("DataLayerProvidersKinds", null);
                    if (result == null)
                    {
                        //NOTE: the only time this is cached is if a result is actually set
                        //DO not cache this setting
                        return new DataLayerProviderKind[] { DataLayerProviderKind.Edm };
                    }

                    string[] splitResults = result.Split(',');
                    List<DataLayerProviderKind> kinds = new List<DataLayerProviderKind>();
                    for (int i = 0; i < splitResults.Length; i++)
                    {
                        kinds.Add(Workspaces.ConvertToDataLayerKind(splitResults[i]));
                    }

                    _dataLayerProviderKinds = kinds.ToArray();
                }

                return _dataLayerProviderKinds;
            }
            set
            {
                _dataLayerProviderKinds = value;
            }
        }

        public static string HostArch
        {
            get
            {
                return ResolvePropertyOrDefault("HostArch", "x86");
            }
        }

        public static string AstoriaBuild
        {
            get
            {
                return ResolvePropertyOrDefault("AstoriaBuild", null);
            }
        }

        public static string HostLanguage
        {
            get
            {
                return ResolvePropertyOrDefault("HostLanguage", "English");
            }
        }

        public static Host Host
        {
            get
            {
                if (host == null)
                {
                    host = ResolvePropertyOrDefault("AstoriaHost", null);
                    if (host == null)
                    {
                        //NOTE: the only time this is cached is if a result is actually set
                        //DO not cache this setting
                        return Host.LocalIIS;
                    }
                }

                return ConvertToHost(host);
            }
            set
            {
                host = value.ToString();
            }
        }
        public static string DataProviderMachineVersion
        {
            get
            {
                return ResolvePropertyOrDefault("DataProviderMachineVersion", "10.0");
            }
        }
        /// <summary>The name of the machine that should be used to provide data.</summary>
        public static String DataProviderMachineName
        {
            get
            {
                if (_dataProviderMachineName == null)
                {
                    _dataProviderMachineName = ResolveProperty("DataProviderMachineName");
                    if (_dataProviderMachineName == null)
                    {
                        switch (RuntimeEnvironment)
                        {
                            case TestRuntimeEnvironment.CheckinSuites:
                                //NOTE: the only time this is cached is if a result is actually set
                                //DO not cache this setting
                                return "Local";
                            case TestRuntimeEnvironment.Lab:
                                //NOTE: the only time this is cached is if a result is actually set
                                //DO not cache this setting
                                return  "Lab";
                            default:
                                Debug.Assert(RuntimeEnvironment == TestRuntimeEnvironment.VisualStudioIde);
                                //NOTE: the only time this is cached is if a result is actually set
                                //DO not cache this setting
                                return "Local";
                        }
                    }
                }

                return _dataProviderMachineName;
            }
            set
            {
                _dataProviderMachineName = value;
            }

        }

        public static string HostMachineName
        {
            get
            {
                return _hostMachineName = _hostMachineName ?? ResolvePropertyOrDefault("HostMachineName", null);
            }
            set { _hostMachineName = value; }
        }

        public static string ClientMachineName
        {
            get
            {
                return _clientMachineName =
                    _clientMachineName ??
                    ResolvePropertyOrDefault("ClientMachineName", null);
            }
            set { _clientMachineName = value; }
        }

        public static string DesignMachineName
        {
            get 
            {
                // Return value if already set.
                if (_designMachineName != null)
                {
                    return _designMachineName;
                }

                // User-specified property takes priority.
                _designMachineName = ResolvePropertyOrDefault("DesignMachineName", null);
                if (_designMachineName != null)
                {
                    return _designMachineName;
                }

                // Not specified DesignVersion means the version is the same as ClientVersion.
                if (DesignVersion == null)
                {
                    //NOTE: the only time this is cached is if a result is actually set
                    //DO not cache this setting
                    return ClientMachineName;
                }

                // Pick a remote machine if code generation has to occur remotely.
                if (IsRemoteVersioning)
                {
                    //NOTE: leaving this as is, continuing caching behavior
                    return _designMachineName = "var1";
                }

                // Local run, this value should never be used.
                return null;
            }
            set { _designMachineName = value; }
        }

        public static string HostAuthenicationMethod
        {
            get
            {
                string defaultValue = "Windows";
                if (Host == Host.Cassini)
                    defaultValue = "None";
                return ResolvePropertyOrDefault("HostAuthenicationMethod", defaultValue);
            }
        }

        public static long ModelTimeout
        {
            get
            {
                return long.Parse(ResolvePropertyOrDefault("ModelTimeout", "120"));
            }
        }

        public static long ModelSeed
        {
            get
            {
                return long.Parse(ResolvePropertyOrDefault("ModelSeed", "0"));
            }
        }

        public static bool ClientSKURun
        {
            get
            {
                return bool.Parse(ResolvePropertyOrDefault("ClientSKURun", "false"));
            }
        }

        public static ClientEnum Client
        {
            get
            {
                // default now handled by ConvertToClient
                return ConvertToClient(ResolvePropertyOrDefault("AstoriaClient", null));
            }
        }

        public static String Model
        {
            get
            {
                return ResolvePropertyOrDefault("Model", "Northwind");
            }
        }

        public static bool BatchAllRequests
        {
            get
            {
                bool value;
                if (bool.TryParse(ResolvePropertyOrDefault("BatchAllRequests", "false"), out value))
                    return value;
                return false;
            }
        }
        public static ClientBrowser AstoriaClientBrowser
        {
            get
            {
                // default now handled by ConvertToClientBrowser
                return ConvertToClientBrowser(ResolvePropertyOrDefault("AstoriaClient", null));
            }
        }
        public static bool IsManualRun
        {
            get
            {
                bool value;
                if (bool.TryParse(ResolvePropertyOrDefault("ManualRun", "false"), out value))
                    return value;
                return false;
            }
        }
        public static ClientOS AstoriaClientOS
        {
            get
            {
                // default now handled by ConverToClientOS
                return ConverToClientOS(ResolvePropertyOrDefault("AstoriaRemoteClientOS", null));
            }
        }

        public static string ServerVersion
        {
            get
            {
                return ResolvePropertyOrDefault("ServerVersion", null);
            }
        }

        public static string ClientVersion
        {
            get
            {
                return ResolvePropertyOrDefault("ClientVersion", null);
            }
        }

        public static string DesignVersion
        {
            get
            {
                return ResolvePropertyOrDefault("DesignVersion", null);
            }
        }

        /// <summary>The runtime environment in which the test is running.</summary>
        public static TestRuntimeEnvironment RuntimeEnvironment
        {
            get
            {
                if (!_runtimeEnvironment.HasValue)
                {
                    string runtimeEnvironmentString = ResolveProperty("RuntimeEnvironment");
                    if (String.IsNullOrEmpty(runtimeEnvironmentString))
                    {
                        _runtimeEnvironment = GetRuntimeEnvironmentByHeuristics();
                    }
                    else
                    {
                        _runtimeEnvironment = (TestRuntimeEnvironment)Enum.Parse(typeof(TestRuntimeEnvironment), runtimeEnvironmentString);
                    }
                }

                return _runtimeEnvironment.Value;
            }
            set
            {
                _runtimeEnvironment = value;
            }
        }

        public static string TransferMode
        {
            get
            {
                return ResolvePropertyOrDefault("TransferMode", "Buffered");
            }
        }

        public static int MaxBufferSize
        {
            get
            {
                return int.Parse(ResolvePropertyOrDefault("MaxBufferSize", "2147483647"));
            }
        }

        public static int MaxReceivedMessageSize
        {
            get
            {
                return int.Parse(ResolvePropertyOrDefault("MaxReceivedMessageSize", "2147483647"));
            }
        }

        public static int MaxRequestLength
        {
            get
            {
                return int.Parse(ResolvePropertyOrDefault("MaxRequestLength", "2097151"));
            }
        }

        public static string TransportSecurityMode
        {
            get
            {
                return ResolvePropertyOrDefault("TransportSecurityMode", "TransportCredentialOnly");
            }
        }

        public static string CloseTimeout
        {
            get
            {
                return ResolvePropertyOrDefault("CloseTimeout", "00:01:00");
            }
        }

        public static string OpenTimeout
        {
            get
            {
                return ResolvePropertyOrDefault("OpenTimeout", "00:01:00");
            }
        }

        public static string ReceiveTimeout
        {
            get
            {
                return ResolvePropertyOrDefault("ReceiveTimeout", "00:10:00");
            }
        }

        public static string SendTimeout
        {
            get
            {
                return ResolvePropertyOrDefault("SendTimeout", "00:01:00");
            }
        }

        public static bool AspNetCompatibilityEnabled
        {
            get
            {
                return Boolean.Parse(ResolvePropertyOrDefault("AspNetCompatibilityEnabled", "true"));
            }
        }

        public static bool UseOpenTypes
        {
            get
            {
                bool value;
                if (bool.TryParse(ResolvePropertyOrDefault("UseOpenTypes", "false"), out value))
                    return value;
                return false;
            }
        }

        public static bool UseDomainServices
        {
            get
            {
                bool value;
                if (bool.TryParse(ResolvePropertyOrDefault("UseDomainServices", "false"), out value))
                    return value;
                else
                {
                    return false;
                }
            }
        }

        internal static string Properties(string propertyName)
        {
            string property = TestInput.Properties[propertyName];
            return property;
        }

        public static Random Random
        {
            get { return ModelEngineOptions.Default.Random; }
        }

        public static int Seed
        {
            get { return ModelEngineOptions.Default.Seed; }
            set { ModelEngineOptions.Default.Seed = value; }
        }

        /// <summary>
        /// Attempts to determine what the runtime environment is by applying
        /// a set of heuristics.
        /// </summary>
        /// <returns>The runtime environment in which the test is running.</returns>
        private static TestRuntimeEnvironment GetRuntimeEnvironmentByHeuristics()
        {
            bool runningInLtm;
            // Current partial-trust client test only supports lab run mode (using ltm).
            if (AstoriaTestProperties.ServiceTrustLevel == TrustLevel.Medium)
                runningInLtm = true;
            else
            {
                using (Process current = Process.GetCurrentProcess())
                {
                    runningInLtm = current.MainModule.FileName.ToLowerInvariant().EndsWith("ltm.exe");
                }
            }

            if (Environment.UserName.ToLowerInvariant().Contains("sqlbld"))
            {
                return TestRuntimeEnvironment.Lab;
            }
            else
            {
                return TestRuntimeEnvironment.CheckinSuites;
            }
        }
    }
}
