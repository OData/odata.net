﻿//---------------------------------------------------------------------
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
            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = IISExpressPath,
                Arguments = string.Format("/path:{0} /port:{1}", GetServicePath(), ServicePort)
            };

            var process = Process.Start(startInfo);
            if (process == null)
            {
                throw new InvalidOperationException("Failed to start service:" + ServiceRoot);
            }

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

        private string GetServicePath()
        {
            var dllPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            var parentPath = Directory.GetParent(dllPath).FullName;
            return parentPath;
        }
    }
}
