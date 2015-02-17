//---------------------------------------------------------------------
// <copyright file="ProcessOutput.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents output from a command-line process.
    /// </summary>
    public class ProcessOutput
    {
        /// <summary>
        /// Initializes a new instance of the ProcessOutput class.
        /// </summary>
        /// <param name="executablePath">The executable path.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="workingDirectory">The working directory.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="standardOutputLines">Contents of process standard output</param>
        /// <param name="standardErrorLines">Contents of process standard error</param>
        /// <param name="exitCode">Process exit code</param>
        /// <param name="hasExited">Value indicating whether process has exited normally</param>
        public ProcessOutput(string executablePath, string arguments, string workingDirectory, TimeSpan timeout, IEnumerable<string> standardOutputLines, IEnumerable<string> standardErrorLines, int exitCode, bool hasExited)
        {
            this.ExecutablePath = executablePath;
            this.Arguments = arguments;
            this.WorkingDirectory = workingDirectory;
            this.Timeout = timeout;
            this.StandardOutputLines = standardOutputLines.ToList().AsReadOnly();
            this.StandardErrorLines = standardErrorLines.ToList().AsReadOnly();
            this.ExitCode = exitCode;
            this.HasExited = hasExited;
        }

        /// <summary>
        /// Gets the executable path.
        /// </summary>
        public string ExecutablePath { get; private set; }

        /// <summary>
        /// Gets the arguments to the process.
        /// </summary>
        public string Arguments { get; private set; }

        /// <summary>
        /// Gets the working directory.
        /// </summary>
        public string WorkingDirectory { get; private set; }

        /// <summary>
        /// Gets the timeout.
        /// </summary>
        /// <value>The timeout.</value>
        public TimeSpan Timeout { get; private set; }

        /// <summary>
        /// Gets the standard output of the process 
        /// </summary>
        public ReadOnlyCollection<string> StandardOutputLines { get; private set; }

        /// <summary>
        /// Gets the standard error of the process 
        /// </summary>
        public ReadOnlyCollection<string> StandardErrorLines { get; private set; }

        /// <summary>
        /// Gets the process exit code
        /// </summary>
        public int ExitCode { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the process has exited normally.
        /// </summary>
        /// <remarks>Value of false means that the process has been killed.</remarks>
        public bool HasExited { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            bool stdoutEmpty = true;
            bool stderrEmpty = true;

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Executable: {0}", this.ExecutablePath).AppendLine();
            sb.AppendFormat("Arguments: {0}", this.Arguments).AppendLine();
            sb.AppendFormat("Working Directory: {0}", this.WorkingDirectory).AppendLine();
            if (this.HasExited)
            {
                sb.AppendFormat("Process exited normally with exit code {0}", this.ExitCode).AppendLine();
            }
            else
            {
                sb.AppendFormat("Process was killed after {0}", this.Timeout).AppendLine();
            }

            foreach (string line in this.StandardOutputLines)
            {
                if (stdoutEmpty)
                {
                    sb.AppendLine("Standard output:");
                    stdoutEmpty = false;
                }

                sb.AppendLine(line);
            }

            if (stdoutEmpty)
            {
                sb.AppendLine("No output.");
            }

            foreach (string line in this.StandardErrorLines)
            {
                if (stderrEmpty)
                {
                    sb.AppendLine("Standard error:");
                    stderrEmpty = false;
                }

                sb.AppendLine(line);
            }

            if (stderrEmpty)
            {
                sb.AppendLine("No error output.");
            }

            return sb.ToString();
        }
    }
}
