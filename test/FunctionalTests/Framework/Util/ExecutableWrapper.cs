//---------------------------------------------------------------------
// <copyright file="ExecutableWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Test.ModuleCore;

namespace System.Data.Test.Astoria.Util
{
    public class ExecutableWrapper
    {
        private string _executablePath;
        private int _millisecondsToWait = 600000; // 10 minutes
        private string _workingDirectory = null;

        public ExecutableWrapper(string executablePath, int millisecondsToWait)
        {
            _executablePath = executablePath;
            _millisecondsToWait = millisecondsToWait;
            _workingDirectory = Environment.CurrentDirectory;
            if (!System.IO.File.Exists(_executablePath))
                throw new System.IO.FileNotFoundException("CommandLine exe path bad", _executablePath);
        }
        public ExecutableWrapper(string executablePath)
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
        public ExecutableResults Run(params string []arguments)
        {
            string fullArguments = null;
            foreach(string s in arguments)
            {
                if(fullArguments==null)
                    fullArguments = s;
                else
                    fullArguments = s+" "+fullArguments;
            }
            return Run(fullArguments);
        }
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, Unrestricted = true)]
        public ExecutableResults Run(string arguments)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = _executablePath;
                if (!System.IO.File.Exists(_executablePath))
                    throw new TestFailedException("Unable to find file to execute:" + _executablePath);
                process.StartInfo.Arguments = arguments;
                process.StartInfo.WorkingDirectory = this.WorkingDirectory;
                AstoriaTestLog.TraceLine("Commandline tool arguments:"+arguments);
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
                    throw new TestFailedException(
                        String.Format("Process {0} failed to finish in less than {1} seconds.{2}Exception information:{2}{3}",
                            _executablePath,
                            _millisecondsToWait / 1000,
                            System.Environment.NewLine,
                            win32Exception.Message));
                }
                catch (System.SystemException systemException)
                {
                    throw new TestFailedException(
                        String.Format("Process {0} failed to finish in less than {1} seconds.{2}Exception information:{2}{3}",
                            _executablePath,
                            _millisecondsToWait / 1000,
                            System.Environment.NewLine,
                            systemException.Message));
                }
                string output = readToEnd.EndInvoke(readToEndResult);
                ExecutableResults result = new ExecutableResults(process.ExitCode, output);
                return result;
            }
        }
        public static ExecutableResults Execute(string exePath, params string[] arguments)
        {
            return Execute(exePath, Environment.CurrentDirectory, arguments);
        }
        public static ExecutableResults Execute(string exePath, string workingDir, params string[] arguments)
        {
            ExecutableWrapper wrapper = new ExecutableWrapper(exePath);
            wrapper.WorkingDirectory = workingDir;
            return wrapper.Run(arguments);
        }
    }

    public class ExecutableResults
    {
        public int ExitCode;
        public string OriginalOutput = null;
        private List<string> _lines = new List<string>();
        public ExecutableResults(int exitCode, string originalOutput)
        {
            OriginalOutput = originalOutput;
            ExitCode = exitCode;
            string []allLines = OriginalOutput.Split('\n');
            foreach (string line in allLines)
            {
                string newline = line.Replace("\r", "");
                _lines.Add(newline);
            }
        }
        public string[] OutputLines
        {
            get { return _lines.ToArray(); }
        }
    }

}
