//---------------------------------------------------------------------
// <copyright file="LocalWebServerHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion Namespaces

    /// <summary>
    /// Provides a helper class for tests that rely on a local web
    /// server.
    /// </summary>
    [TestClass]
    public class LocalWebServerHelper
    {
        /// <summary>Source path where suitebin dependencies can be found.</summary>
        private static string suiteBinPath;

        /// <summary>Source path where binary dependencies can be found.</summary>
        private static string binarySourcePath;

        /// <summary>Port number for the local web server.</summary>
        private static int localPortNumber = -1;

        /// <summary>Local web server process.</summary>
        private static Process process;

        /// <summary>Path to which files will be written to.</summary>
        private static string fileTargetPath;

        /// <summary>Reference count for process information.</summary>
        /// <remarks>This is just informational.</remarks>
        private static int processReferenceCount;

        /// <summary>Last text value generated for test arguments.</summary>
        /// <remarks>This value is cached to avoid regenerating the web server files.</remarks>
        private static string lastTestArgumentsText;

        /// <summary>Last service file name generated.</summary>
        /// <remarks>This value is cached to avoid regenerating the web server files.</remarks>
        private static string lastServiceFileName;

        /// <summary>Last setup result returned - the uri to the service currently active.</summary>
        private static string lastSetupResult;

        /// <summary>Unique identifier for service files.</summary>
        private static int serviceFileNumber;

        /// <summary>The lock for serviceFileNumber variable.</summary>
        private static object serviceFileNumberLock = new object();

        /// <summary>Last DataService type generated.</summary>
        /// <remarks>This value is cached to avoid regenerating the web server files.</remarks>
        private static Type lastDataServiceType;

        /// <summary>Last Service type generated.</summary>
        /// <remarks>This value is cached to avoid regenerating the web server files.</remarks>
        private static Type lastServiceType;

        /// <summary>
        /// Whether to run this in medium trust or not
        /// </summary>
        public static bool? RunInMediumTrust;

        /// <summary>Performs cleanup and ensures that there are no active web servers.</summary>
        public static void Cleanup()
        {
            if (processReferenceCount != 0)
            {
                Trace.WriteLine("LocalWebServerHelper.AssemblyCleanup: Outstanding local web server reference count: " + processReferenceCount);
                Trace.WriteLine("LocalWebServerHelper.AssemblyCleanup: Verify correct disposal of test requests.");
            }

            if (process != null)
            {
                // The local web server does not respond to CloseMainWindow.
                Trace.WriteLine("Closing web server process...");
                if (!process.HasExited)
                {
                    try
                    {
                        if (!TryShutdownWebServerGracefully(process))
                        {
                            process.Kill();
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        Trace.WriteLine("Unable to kill local web server process.");
                    }
                }

                process.Dispose();
                process = null;

                localPortNumber = -1;
                lastServiceFileName = null;
                lastDataServiceType = null;
                lastSetupResult = null;
            }
        }

        /// <summary>
        /// Performs cleanup after all tests in the current assembly have
        /// been executed.
        /// </summary>
        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            Cleanup();
        }

        /// <summary>
        /// Path to the root of the suitebin tree.
        /// </summary>
        internal static string SuiteBinPath
        {
            get
            {
                if (suiteBinPath != null)
                {
                    return suiteBinPath;
                }

                string result = Path.GetDirectoryName(typeof(AstoriaUnitTests.Tests.LocalWebServerHelper).Assembly.Location);

                if (String.IsNullOrEmpty(result))
                {
                    throw new InvalidOperationException("unable to set up service files.");
                }

                suiteBinPath = result;
                return result;
            }
        }

        /// <summary>
        /// Path to the root of the binary tree, typically defined by _nttree in a build environment and
        /// DD_BuiltTarget in a test environment.
        /// </summary>
        internal static string BinarySourcePath
        {
            get
            {
                if (binarySourcePath != null)
                {
                    return binarySourcePath;
                }

                string result = Path.GetDirectoryName(typeof(AstoriaUnitTests.Tests.LocalWebServerHelper).Assembly.Location);

                if (String.IsNullOrEmpty(result))
                {
                    throw new InvalidOperationException("unable to set up service files.");
                }

                binarySourcePath = result;
                return result;
            }
            set
            {
                binarySourcePath = value;
            }
        }

        /// <summary>Path to the root of the current enlistment.</summary>
        internal static string EnlistmentRootPath
        {
            get
            {
                string result = TestUtil.EnlistmentRoot;
                if (String.IsNullOrEmpty(result))
                {
                    throw new InvalidOperationException("Enlistment root is undefined - unable to set up service files.");
                }
                return result;
            }
        }

        /// <summary>Port number for the local web server.</summary>
        public static int LocalPortNumber
        {
            get
            {
                return localPortNumber;
            }

            set
            {
                localPortNumber = value;
            }
        }

        /// <summary>Notifies the helper that the web server is no longer needed.</summary>
        internal static void DisposeProcess()
        {
            processReferenceCount--;
            if (processReferenceCount < 0)
            {
                Trace.WriteLine("LocalWebServerHelper.DisposeProcess: Negative local web server reference count: " + processReferenceCount);
                Trace.WriteLine("LocalWebServerHelper.DisposeProcess: Verify correct disposal of test requests.");
            }
        }

        /// <summary>
        /// Sets up the required files locally to test the web data service
        /// through the local web server.
        /// </summary>
        public static string SetupServiceFiles(string serviceFileName, Type dataServiceType)
        {
            return SetupServiceFiles(serviceFileName, dataServiceType, null, null);
        }

        /// <summary>
        /// Sets up the required files locally to test the web data service
        /// through the local web server, including a string with arguments.
        /// </summary>
        public static string SetupServiceFiles(string serviceFileName, Type dataServiceType, Type serviceType, string testArgumentsText, string configSnippet = null, string initializeServiceCode = null)
        {
            Dictionary<string, string> connections = new Dictionary<string, string>();

            if (serviceType == null)
            {
                serviceType = typeof(OpenWebDataService<>).MakeGenericType(dataServiceType);
            }

            string northwindConnectionString = NorthwindModel.NorthwindContext.ContextConnectionString;
            if (northwindConnectionString.StartsWith("name"))
            {
                System.Configuration.ConnectionStringSettings northwindSettings = System.Configuration.ConfigurationManager.ConnectionStrings["NorthwindContext"];
                if (northwindSettings != null)
                {
                    northwindConnectionString = northwindSettings.ConnectionString;
                }
                else
                {
                    AstoriaUnitTests.Data.ServiceModelData.Northwind.EnsureDependenciesAvailable();
                    Debug.Assert(!NorthwindModel.NorthwindContext.ContextConnectionString.StartsWith("name"));
                    northwindConnectionString = NorthwindModel.NorthwindContext.ContextConnectionString;
                }
            }

            connections.Add("NorthwindContext", northwindConnectionString);

            return SetupServiceFiles(serviceFileName, dataServiceType, serviceType, testArgumentsText, connections, configSnippet, initializeServiceCode);
        }

        /// <summary>Path to which files will be written to.</summary>
        public static string FileTargetPath
        {
            get
            {
                if (fileTargetPath != null)
                {
                    return fileTargetPath;
                }

                return System.Environment.CurrentDirectory;
            }
            set
            {
                fileTargetPath = value;
            }
        }

        /// <summary>Text fragment to set up the system.codedom section in a standard web.config file.</summary>
        public static string WebConfigCodeDomFragment
        {
            get
            {
                return " <system.codedom>\r\n" +
                    "  <compilers>\r\n" +
                    "   <compiler language='c#;cs;csharp'\r\n" +
                    "    extension='.cs'\r\n" +
                    "    type='Microsoft.CSharp.CSharpCodeProvider,System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'\r\n" +
                    "    >\r\n" +
                    "    <providerOption name='CompilerVersion' value='v4.0' />\r\n" +
                    "   </compiler>\r\n" +
                    "  </compilers>\r\n" +
                    " </system.codedom>\r\n";
            }
        }

        /// <summary>httpruntime config section (goes to: /configuration/system.web/httpRuntime)</summary>
        public static string WebConfigHttpRuntimeFragment
        {
            get;
            set;
        }

        /// <summary>Text fragment to set up the compilation section in a standard web.config file.</summary>
        public static string WebConfigCompilationFragment
        {
            get
            {
                // DEVNOTE(pqian): Framework 4.0 requires TargetFrameworkMoniker to be set:
                // <compilation debug="true" >
                return
                    "  <compilation debug='true'>\r\n" +
                    "   <assemblies>\r\n" +
                    "    <add assembly='" + DataFxAssemblyRef.SystemCore + "'/>\r\n" +
                    "    <add assembly='" + DataFxAssemblyRef.SystemDataDataSetExtensions + "'/>\r\n" +
                    "    <add assembly='System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35'/>\r\n" +
                    "    <add assembly='" + DataFxAssemblyRef.SystemXmlLinq + "'/>\r\n" +
                    "    <add assembly='System.Data.Entity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089'/>\r\n" +
                    "    <add assembly='" + DataFxAssemblyRef.DataServices + "'/>\r\n" +
                    "    <add assembly='" + DataFxAssemblyRef.DataServicesClient + "'/>\r\n" +
                    "    <add assembly='" + DataFxAssemblyRef.SystemServiceModel + "'/>\r\n" +
                    "    <add assembly='System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35'/>\r\n" +
                    "   </assemblies>\r\n" +
                    "  </compilation>\r\n";
            }
        }

        /// <summary>
        /// Sets up the required files locally to test the web data service
        /// through the local web server.
        /// </summary>
        public static string SetupServiceFiles(string serviceFileName, Type dataServiceType, Type serviceType,
            string testArgumentsText, Dictionary<string, string> entityConnectionStrings, string configSnippet = null, string initializeServiceCode = null)
        {
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(serviceFileName, "serviceFileName");

            if (lastServiceFileName == serviceFileName &&
                lastDataServiceType == dataServiceType &&
                lastServiceType == serviceType &&
                lastTestArgumentsText == testArgumentsText)
            {
                Trace.WriteLine("Service file name, data type, service type and arguments match, so web server will be reused.");
                return lastSetupResult;
            }

            string result = "http://localhost:" + LocalPortNumber.ToString(CultureInfo.InvariantCulture) + "/";
            string uniqueServiceFileName = "";
            lock (serviceFileNumberLock)
            {
                if (serviceFileNumber > 0)
                {
                    uniqueServiceFileName += serviceFileNumber.ToString(CultureInfo.InvariantCulture);
                }

                serviceFileNumber++;
            }

            uniqueServiceFileName += serviceFileName;
            result += uniqueServiceFileName;

            SetupRemoteServiceFiles(uniqueServiceFileName, dataServiceType, serviceType,
                testArgumentsText, entityConnectionStrings, LocalWebServerHelper.FileTargetPath, configSnippet, initializeServiceCode);

            lastServiceFileName = serviceFileName;
            lastDataServiceType = dataServiceType;
            lastTestArgumentsText = testArgumentsText;
            lastSetupResult = result;
            lastServiceType = serviceType;

            return result;
        }

        /// <summary>
        /// Sets up the required files to test the web data service through a web server.
        /// </summary>
        public static void SetupRemoteServiceFiles(string serviceFileName, Type dataSourceType, Type serviceType,
            string testArgumentsText, Dictionary<string, string> entityConnectionStrings, string targetPath, string configSnippet = null, string initializeServiceCode = null)
        {
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(serviceFileName, "serviceFileName");
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(dataSourceType, "dataServiceType");
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(serviceType, "serviceType");
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(targetPath, "targetPath");

            entityConnectionStrings = entityConnectionStrings ?? new Dictionary<string, string>();
            string typeName = GetTypeName(serviceType);

            //
            // To set up an Astoria service, the following files are required:
            //   service.svc    - holds the entry point (we'll need to customize this to test extensions)
            //   web.config     - provides configuration information to setup service and reference assemblies
            //
            // Some notes:
            //   Setting Debug to 'true' includes symbols in the @ServiceHost directive.
            //   Setting IncludeExceptionDetailsInFaults to 'true' in the attribute sends a page with exception information back.
            //
            bool isTestOpenService = serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(OpenWebDataService<>);
            string serviceContents =
                "<%@ ServiceHost Language=\"C#\" Debug=\"true\" Factory=\"Microsoft.OData.Service.DataServiceHostFactory\" Service=\"AstoriaTest.TheDataService\" %>\r\n" +
                "namespace AstoriaTest\r\n" +
                "{\r\n" +
                "    [System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]\r\n";
            if (isTestOpenService || (serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(Microsoft.OData.Service.DataService<>)))
            {
                serviceContents +=
                        "    public class TheDataService : Microsoft.OData.Service.DataService<" + GetTypeName(dataSourceType) + ">, System.IServiceProvider\r\n";
            }
            else
            {
                serviceContents +=
                    "    public class TheDataService : " + typeName + "\r\n";
            }


            serviceContents +=
                "    {\r\n";

            testArgumentsText = testArgumentsText ?? "";
            serviceContents += "        static TheDataService()\r\n";
            serviceContents += "        {\r\n";
            serviceContents += "            AstoriaUnitTests.Stubs.TestWebRequest.LoadSerializedTestArguments(\r\n";
            serviceContents += "                \"" + EscapeForCSharpString(testArgumentsText) + "\"\r\n";
            serviceContents += "                );\r\n";
            serviceContents += "        }\r\n";

            // We use the standard DataService, so if we in fact wanted
            // full access, we can configure the service directly through
            // the generated code.
            if (isTestOpenService)
            {
                serviceContents += "        public static void InitializeService(Microsoft.OData.Service.DataServiceConfiguration configuration)\r\n";
                serviceContents += "        {\r\n";
                serviceContents += "            configuration.SetEntitySetAccessRule(\"*\", Microsoft.OData.Service.EntitySetRights.All);\r\n";
                serviceContents += "            configuration.SetServiceOperationAccessRule(\"*\", Microsoft.OData.Service.ServiceOperationRights.All);\r\n";
                serviceContents += "            configuration.UseVerboseErrors = true;\r\n";
                serviceContents += "            configuration.DataServiceBehavior.MaxProtocolVersion = Microsoft.OData.Client.ODataProtocolVersion.V4;\r\n";
                serviceContents += "            System.Reflection.MethodInfo methodInfo = typeof(" + typeName + ").GetMethod(\"InitializeService\", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly);\r\n";
                serviceContents += "            if (methodInfo != null && methodInfo.ReturnType == typeof(void))\r\n";
                serviceContents += "            {\r\n";
                serviceContents += "                methodInfo.Invoke(null, new object[] { configuration });\r\n";
                serviceContents += "            }\r\n";

                if (initializeServiceCode != null)
                {
                    serviceContents += "            " + initializeServiceCode;
                }

                serviceContents += "        }\r\n";

                serviceContents += "        object System.IServiceProvider.GetService(System.Type serviceType)\r\n";
                serviceContents += "        {\r\n";
                serviceContents += "            // Need to return provider instance for custom providers, to ensure\r\n";
                serviceContents += "            // that they are treated as custom providers.\r\n";
                serviceContents += "            if (typeof(" + typeName + ").GetInterface(typeof(System.IServiceProvider).Name) != null)\r\n";
                serviceContents += "            {\r\n";
                serviceContents += "                object contextInstance;\r\n";
                serviceContents += "                if (typeof(" + typeName + ") == typeof(AstoriaUnitTests.Stubs.CustomRowBasedContext))\r\n";
                serviceContents += "                {\r\n";
                serviceContents += "                    contextInstance = AstoriaUnitTests.Stubs.CustomRowBasedContext.GetInstance();\r\n";
                serviceContents += "                }\r\n";
                serviceContents += "                else if (typeof(" + typeName + ") == typeof(AstoriaUnitTests.Stubs.CustomRowBasedOpenTypesContext))\r\n";
                serviceContents += "                {\r\n";
                serviceContents += "                    contextInstance = AstoriaUnitTests.Stubs.CustomRowBasedOpenTypesContext.GetInstance();\r\n";
                serviceContents += "                }\r\n";
                serviceContents += "                else";
                serviceContents += "                {\r\n";
                serviceContents += "                    contextInstance = typeof(" + typeName + ").GetConstructor(System.Type.EmptyTypes).Invoke(null);\r\n";
                serviceContents += "                }\r\n";
                serviceContents += "                return ((System.IServiceProvider)contextInstance).GetService(serviceType);";
                serviceContents += "            }\r\n";
                serviceContents += "            return null;\r\n";
                serviceContents += "        }\r\n";
            }

            serviceContents +=
                "    }\r\n" +
                "}";
            File.WriteAllText(Path.Combine(targetPath, serviceFileName), serviceContents);

            string configContents =
                "<?xml version='1.0'?>\r\n" +
                "<configuration>\r\n";

            if (configSnippet != null)
            {
                configContents += configSnippet;
            }

            configContents += " <connectionStrings>\r\n";
            foreach (KeyValuePair<string, string> entityConnection in entityConnectionStrings)
            {
                configContents +=
                    "  <add name='" + entityConnection.Key + "' providerName='System.Data.EntityClient' " +
                    "connectionString='" + entityConnection.Value + "'/>\r\n";
            }

            configContents +=
                " </connectionStrings>\r\n" +
                " <system.web>\r\n" +
                WebConfigCompilationFragment +
                (WebConfigHttpRuntimeFragment ?? string.Empty) +
                ((RunInMediumTrust != null && RunInMediumTrust.Value == true) ? "<trust level=\"Medium\" />" : String.Empty) +
                " </system.web>\r\n" +
                WebConfigCodeDomFragment +
                "</configuration>\r\n";
            // clear httpRuntime section to prevent subsequently created web apps from injecting httpRuntime settings unknowingly
            WebConfigHttpRuntimeFragment = null;

            configContents = configContents.Replace("DDSUITEROOT",
                Path.Combine(EnlistmentRootPath, @"ddsuites\src\fx\DataWeb\Models\Northwind"));
            File.WriteAllText(Path.Combine(targetPath, "web.config"), configContents);

            string targetBinPath = Path.Combine(targetPath, "bin");
            System.Data.Test.Astoria.IOUtil.EnsureDirectoryExists(targetBinPath);
            string suiteBinPath = SuiteBinPath;
            string[] binariesToCopy = new string[]
                {
                    "Astoria.Northwind.dll",
                    "Astoria.EFFKModel.dll",
                    "AstoriaTestFramework.dll",
                    "AstoriaTestFramework.FullTrust.dll",
                    "AstoriaUnitTests.dll",
                    "Microsoft.Test.KokoMo.dll",
                    "Microsoft.Test.ModuleCore.dll",
                    "EntityFramework.dll"
                };

            CopyProductBinaries(targetBinPath);
            CopyBinaries(SuiteBinPath, targetBinPath, binariesToCopy);

            if (isTestOpenService || (serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(Microsoft.OData.Service.DataService<>)))
            {
                // Nothing to do for these types.
            }
            else
            {
                // Assume a custom type, include the assembly.
                string sourceFilePath = serviceType.Assembly.Location;
                string targetFilePath = Path.Combine(targetBinPath, Path.GetFileName(sourceFilePath));
                if (!File.Exists(targetFilePath) ||
                    File.GetLastWriteTime(sourceFilePath) > File.GetLastWriteTime(targetFilePath))
                {
                    File.Copy(sourceFilePath, targetFilePath, true /* overwrite */);
                }
            }
        }

        /// <summary>Starts an instance of the local web server.</summary>
        public static void StartWebServer()
        {
            if (process == null)
            {
                string serverPath = FindWebServerPath();
                process = UnitTestsUtil.StartIISExpress(serverPath, LocalWebServerHelper.FileTargetPath, ref localPortNumber);
            }

            processReferenceCount++;
        }

        /// <summary>
        /// Copy the product binaries from the suitebin location to the given location.
        /// </summary>
        /// <param name="destination">folder to which product binaries need to be copied to.</param>
        public static void CopyProductBinaries(string destination)
        {
            var binariesToCopy = new string[]
            {
                "Microsoft.Spatial.dll",
                "Microsoft.OData.Edm.dll",
                "Microsoft.OData.Core.dll",
                "Microsoft.OData.Client.dll",
                "Microsoft.OData.Service.dll"
            };

            CopyBinaries(SuiteBinPath, destination, binariesToCopy);
        }

        /// <summary>Escapes the specified <paramref name='text' /> to be used in a C# string literal.</summary>
        /// <remarks>No quoting is done around <paramref name='text' />.</remarks>
        private static string EscapeForCSharpString(string text)
        {
            Debug.Assert(text != null, "text != null");
            return text.Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n");
        }

        /// <summary>
        /// Helper method to find where the local web server binary is available from.
        /// </summary>
        /// <returns>The path to a local iisexpress.exe file.</returns>
        private static string FindWebServerPath()
        {
            string fullName = Path.Combine(Environment.ExpandEnvironmentVariables(@"%programfiles%\IIS Express"), "iisexpress.exe");

            if (File.Exists(fullName))
            {
                return fullName;
            }

            throw new InvalidOperationException("Unable to find IIS Express");
        }

        private static string GetTypeName(Type type)
        {
            if (!type.IsGenericType)
            {
                return type.FullName.Replace('+', '.');
            }
            else
            {
                string typeName = type.GetGenericTypeDefinition().FullName.Substring(0, type.GetGenericTypeDefinition().FullName.Length - 2);
                string seperator = String.Empty;

                typeName += "<";
                foreach (Type genericType in type.GetGenericArguments())
                {
                    string genericTypeName = GetTypeName(genericType);
                    typeName += seperator + genericTypeName;
                    seperator = ",";
                }
                typeName += ">";

                return typeName;
            }
        }

        private static void CopyBinaries(string source, string destination, params string[] binariesToCopy)
        {
            foreach (string binaryToCopy in binariesToCopy)
            {
                string sourceFilePath = Path.Combine(source, binaryToCopy);
                string targetFilePath = Path.Combine(destination, Path.GetFileName(binaryToCopy));
                System.Data.Test.Astoria.IOUtil.CopyFileIfNewer(sourceFilePath, targetFilePath);
            }
        }

        /// <summary>
        /// Tries to shutdown Cassini by finding the Cassini window and sending WM_QUERYENDSESSION and WM_CLOSE
        /// to the window. When Cassini handles WM_QUERYENDSESSION, it sets some internal state which causes
        /// the systray icon to be cleaned up when WM_CLOSE is handled.
        /// </summary>
        /// <param name="process">The Cassini process</param>
        /// <returns>true if the process was shut down gracefully, else false</returns>
        static bool TryShutdownWebServerGracefully(Process process)
        {
            try
            {
                IntPtr handle = FindCassiniWindow(process.Id);
                if (handle != IntPtr.Zero)
                {
                    NativeMethods.SendMessage(handle, NativeMethods.WM_QUERYENDSESSION, 0, 0);
                    NativeMethods.SendMessage(handle, NativeMethods.WM_CLOSE, 0, 0);
                    process.WaitForExit(100);
                }
            }
            catch
            {
                // Catch everything - errors in the above code should never cause a test to fail.
            }
            return process.HasExited;
        }

        /// <summary>
        /// Finds the "main" Cassini window, which will respond to WM_QUERYENDSESION and WM_CLOSE.
        /// </summary>
        /// <param name="processId">A Cassini process ID</param>
        /// <returns>The IntPtr handle to the Cassini window</returns>
        static IntPtr FindCassiniWindow(int processId)
        {
            IntPtr result = IntPtr.Zero;

            NativeMethods.EnumThreadWindowsCallback callback = (handle, extraP) =>
            {
                int num;
                NativeMethods.GetWindowThreadProcessId(handle, out num);
                if (num == processId)
                {
                    // Heuristic: Cassini window titles start with "ASP.NET Development Server"
                    int length = NativeMethods.GetWindowTextLength(handle) + 1;
                    StringBuilder b = new StringBuilder(length);
                    NativeMethods.GetWindowText(handle, b, length);
                    if (b.ToString().StartsWith("ASP.NET Development Server"))
                    {
                        result = handle;
                        return false;
                    }
                }
                return true;
            };

            // Search through all windows
            NativeMethods.EnumWindows(callback, IntPtr.Zero);

            // Reference the callback to stop it being GC'd before this point.
            GC.KeepAlive(callback);
            return result;
        }

        static class NativeMethods
        {
            public const int WM_CLOSE = 0x10;
            public const int WM_QUERYENDSESSION = 0x11;

            [DllImport("user32", CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

            [DllImport("user32", CharSet = CharSet.Auto)]
            public static extern bool EnumWindows(EnumThreadWindowsCallback callback, IntPtr extraData);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern int GetWindowTextLength(IntPtr hWnd);

            public delegate bool EnumThreadWindowsCallback(IntPtr hWnd, IntPtr lParam);
        }
    }
}
