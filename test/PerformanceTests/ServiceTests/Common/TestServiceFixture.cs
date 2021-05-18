//---------------------------------------------------------------------
// <copyright file="TestServiceFixture.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Performance
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    public class TestServiceFixture<ClassType> : IDisposable
    {
        private const int ServicePort = 9000;
        private const string ServiceRoot = "PerfService";
        private const string IISExpressProcessName = "iisexpress";
        private static readonly string IISExpressPath =
            Environment.ExpandEnvironmentVariables(@"%ProgramFiles%\IIS Express\iisexpress.exe");

        public Uri ServiceBaseUri;

        public TestServiceFixture()
        {
            KillServices();
            string servicePath = GetServicePath();
            Console.WriteLine("SERVICE PATH {0}", servicePath);
            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = IISExpressPath,
                Arguments = string.Format("/path:{0} /port:{1}", servicePath, ServicePort)
            };

            var process = Process.Start(startInfo);
            if (process == null)
            {
                throw new InvalidOperationException("Failed to start service:" + ServiceRoot);
            }

            System.Threading.Thread.Sleep(2000);

            ServiceBaseUri = new Uri("http://localhost" + ":" + ServicePort + "/" + ServiceRoot + "/");
        }

        public void Dispose()
        {
            KillServices();
        }

        private void KillServices()
        {
            var processes = Process.GetProcessesByName(IISExpressProcessName);
            foreach (var process in processes)
            {
                process.Kill();
                process.WaitForExit();
            }
        }

        /// <summary>
        /// Returns the directory that contains the test service
        /// </summary>
        /// <returns></returns>
        private string GetServicePath()
        {
            // try to get it from cli args -ServicePath "path"
            string[] cmdArgs = Environment.GetCommandLineArgs();
            for (int i = 0; i < cmdArgs.Length; i++)
            {
                if (cmdArgs[i] == "-ServicePath" && (i + 1) < cmdArgs.Length)
                {
                    return cmdArgs[i + 1];
                }
            }

            // then try env var
            string envServicePath = Environment.GetEnvironmentVariable("ServicePath");
            if (!string.IsNullOrEmpty(envServicePath))
            {
                return envServicePath;
            }


            // if the service path is not provided in env vars or CLI args, then
            // let's try to find it

            // the test service is located in odata.net/test/PerformanceTests/Framework/TestService
            // the code being executed (if it was built using dotnet run or dotnet published)
            // will be located in a nested subfolder under odata.net/test/PerformanceTests/ServiceTests/...

            // so let's walk the directory tree backwards until we find the PerformanceTests directory

            var dllPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            var dir = Directory.GetParent(dllPath);
            while (dir.Name != "PerformanceTests" && dir.FullName != dir.Root.FullName)
            {
                dir = dir.Parent;
            }

            if (dir.Name == "PerformanceTests")
            {
                var servicePath = Path.Combine(dir.FullName, "Framework", "TestService");
                return servicePath;
            }

            throw new Exception("Service path not specified."
                + " Please specify the path of the test service directory"
                + " using the -ServicePath CLI arg or ServicePath environment variable.");
        }
    }
}
