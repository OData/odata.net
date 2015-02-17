//---------------------------------------------------------------------
// <copyright file="Common.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Commander
{
    using System;
    using System.IO;
    using System.Diagnostics;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Tcp;

    
    public class RemoteServer : MarshalByRefObject
    {
        public static string AstoriaEndPointUriName = "AstoriaTestRemoteServer";
        public static int StandardPort = 8083;
        public static string PASSCODE = "1PasswordFoo!";
        private bool _authenicated = false;
        public RemoteServer()
        {
            Console.WriteLine("RemoteServer activated");
        }

        public void OpenPort(string portclassName, int portId)
        {
            AuthenicateAssert();
            if (!Firewall.PortHelper.IsPortOpen(portclassName, portId))
                Firewall.PortHelper.AddGlobalOpenPort(portclassName, portId);
        }
        
        public bool KillProcess(int processId)
        {
            AuthenicateAssert();

            Process p = Process.GetProcessById(processId);
            if (p == null)
            {
                return false;
            }
            else
            {
                p.Kill();
                return true;
            }
        }
        public int ExecuteFileASync(string workingDirectory, string filePath, string arguments)
        {
            AuthenicateAssert();
            string expandedFilePath = Environment.ExpandEnvironmentVariables(filePath);
            string expandedWorkingDirectory = Environment.ExpandEnvironmentVariables(workingDirectory);
            Console.WriteLine("Attempting to async execute file:" + expandedFilePath + " " + arguments);

            //Returns a processId
            Process p = new Process();
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.FileName = expandedFilePath;
            p.StartInfo.WorkingDirectory = expandedWorkingDirectory;
            p.StartInfo.Arguments = arguments;
            p.StartInfo.UseShellExecute = false;
            bool started = p.Start();
            if (started)
            {
                p.WaitForInputIdle(2000);
                return p.Id;
            }
            else
                return -1;
        }
        public int ExecuteFileASync(string filePath, string arguments)
        {
            return this.ExecuteFileASync(Environment.CurrentDirectory, filePath, arguments);
        }
        public RemoteExecutableResults ExecuteFile(string workingDirectory, string filePath, string arguments)
        {
            AuthenicateAssert();

            string expandedFilePath = Environment.ExpandEnvironmentVariables(filePath);
            string expandedWorkingDirectory = Environment.ExpandEnvironmentVariables(workingDirectory);
            Console.WriteLine("Attempting to execute file:" + expandedFilePath + " " + arguments);
            RemoteExecutableResults results = new RemoteExecutableResults();
            try
            {
                results =ExecutableFile.Execute(filePath, expandedWorkingDirectory, arguments);
            }
            catch (Exception exc)
            {
                results.ExitCode = 1;
                results.Output = exc.ToString();
            }
            return results;
        }
        public RemoteExecutableResults ExecuteFile(string filePath, string arguments)
        {
            return this.ExecuteFile(Environment.CurrentDirectory, filePath, arguments);
        }
        public RemoteExecutableResults ExecuteScript(string scriptArguments)
        {
            AuthenicateAssert();
            string windowsSystemDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System);
            string cscriptFilePath = Path.Combine(windowsSystemDirectory, "cscript.exe");
            return this.ExecuteFile(cscriptFilePath, scriptArguments);
        }
        private void AuthenicateAssert()
        {
            if (!_authenicated)
                throw new InvalidOperationException("Need to Login");
        }
        /*
        public string CreateDirectory(string parentDirectory, string childDirectoryName)
        {
            AuthenicateAssert();
            if(!Directory.Exists(parentDirectory))
                throw new DirectoryNotFoundException(parentDirectory);
            string newDir = Path.Combine(parentDirectory,childDirectoryName);
            Directory.CreateDirectory(newDir);
            return newDir;
        }
        public string CreateDirectory(string childDirectoryName)
        {
            return CreateDirectory(Environment.CurrentDirectory, childDirectoryName);
        }*/
        public void Login(string passCode)
        {
            Console.WriteLine("Login called");
            if (passCode.Equals(PASSCODE))
                _authenicated = true;
            
        }
        
        public static RemoteServer CreateLoggedInClientRemoteServer(string serverName)
        {
            RemoteServer obj = (RemoteServer)Activator.GetObject(typeof(RemoteServer),
                String.Format("tcp://{0}:{1}/{2}", serverName, RemoteServer.StandardPort, RemoteServer.AstoriaEndPointUriName));
            if (obj == null)
                System.Console.WriteLine("Could not locate server");
            else
            {
                obj.Login(RemoteServer.PASSCODE);
            }
            return obj;
        }
    }
}
