//---------------------------------------------------------------------
// <copyright file="ExecuteFile.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Commander
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System;

    public class ExecutableFile
    {
        private string _executablePath;
        private int _millisecondsToWait = 600000; // 10 minutes
        private string _workingDirectory = null;

        public ExecutableFile(string executablePath, int millisecondsToWait)
        {
            _executablePath = executablePath;
            _millisecondsToWait = millisecondsToWait;
            _workingDirectory = Environment.CurrentDirectory;
        }
        public ExecutableFile(string executablePath)
            : this(executablePath, 60000)
        {
        }
        public string WorkingDirectory
        {
            get { return _workingDirectory; }
            set { _workingDirectory = value; }
        }
        public string ExecutablePath
        {
            get { return _executablePath; }
        }

        public int MillisecondsToWait
        {
            get { return _millisecondsToWait; }
        }
        delegate string ReadToEndDelegate();
       
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, Unrestricted = true)]
        public RemoteExecutableResults Run(string arguments)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = _executablePath;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.WorkingDirectory = this.WorkingDirectory;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();

                ReadToEndDelegate readToEnd = new ReadToEndDelegate(process.StandardOutput.ReadToEnd);
                IAsyncResult readToEndResult = readToEnd.BeginInvoke(null, null);

                try
                {
                    process.WaitForExit(_millisecondsToWait);
                }
                catch (System.ComponentModel.Win32Exception win32Exception)
                {
                    throw new TimeoutException(
                        String.Format("Process {0} failed to finish in less than {1} seconds.{2}Exception information:{2}{3}",
                            _executablePath,
                            _millisecondsToWait / 1000,
                            System.Environment.NewLine,
                            win32Exception.Message));
                }
                catch (System.SystemException systemException)
                {
                    throw new TimeoutException(
                        String.Format("Process {0} failed to finish in less than {1} seconds.{2}Exception information:{2}{3}",
                            _executablePath,
                            _millisecondsToWait / 1000,
                            System.Environment.NewLine,
                            systemException.Message));
                }
                string output = readToEnd.EndInvoke(readToEndResult);
                RemoteExecutableResults result = new RemoteExecutableResults();
                result.ExitCode = process.ExitCode;
                result.Output = output;
                return result;
            }
        }
        public static RemoteExecutableResults Execute(string exePath, string arguments)
        {
            return Execute(exePath, Environment.CurrentDirectory, arguments);
        }
        public static RemoteExecutableResults Execute(string exePath, string workingDir, string arguments)
        {
            ExecutableFile wrapper = new ExecutableFile(exePath);
            wrapper.WorkingDirectory = workingDir;
            return wrapper.Run(arguments);
        }
       
    }
    [Serializable]
    public class RemoteExecutableResults
    {
        private int _exitCode;
        private string _output = null;
        public RemoteExecutableResults()
        {
        }

        public int ExitCode
        {
            get { return _exitCode; }
            set { _exitCode = value; }
        }
        public string Output
        {
            get { return _output; }
            set { _output = value; }
        }
    }

}

