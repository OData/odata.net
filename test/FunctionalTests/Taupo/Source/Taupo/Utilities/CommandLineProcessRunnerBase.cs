//---------------------------------------------------------------------
// <copyright file="CommandLineProcessRunnerBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Runs command-line processes with arguments taken from properties and provides access to standard output and standard error
    /// </summary>
    public abstract class CommandLineProcessRunnerBase
    {
        /// <summary>
        /// Initializes a new instance of the CommandLineProcessRunnerBase class.
        /// </summary>
        /// <param name="executable">The executable to run.</param>
        protected CommandLineProcessRunnerBase(string executable)
        {
            this.ExecutableFile = executable;
            this.WorkingDirectory = Environment.CurrentDirectory;
            this.Timeout = TimeSpan.MaxValue;
        }

        /// <summary>
        /// Gets or sets path to the executable file
        /// </summary>
        public string ExecutableFile { get; set; }

        /// <summary>
        /// Gets or sets process working directory.
        /// </summary>
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Gets or sets the amount of time to wait for process to exit.
        /// </summary>
        /// <remarks>Use <see cref="TimeSpan.MaxValue"/> to wait forever</remarks>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Runs the command-line process and captures standard output and standard error which are
        /// available through <see cref="ProcessOutput.StandardOutputLines"/> and 
        /// <see cref="ProcessOutput.StandardErrorLines"/>.
        /// </summary>
        /// <returns>ProcessOutput object which describes the results of the process</returns>
        [SecuritySafeCritical]
        public ProcessOutput Run()
        {
            // add this.Arguments string to the end of the property generated arguments
            string arguments = this.GenerateArguments();

            if (this.ExecutableFile == null)
            {
                throw new TaupoInvalidOperationException("Executable file cannot be null.");
            }

            ProcessStartInfo psi = new ProcessStartInfo(this.ExecutableFile, arguments)
            {
                WorkingDirectory = this.WorkingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var standardErrorLines = new List<string>();
            var standardOutputLines = new List<string>();

            var endOfStandardOutput = new ManualResetEvent(false);
            var endOfStandardError = new ManualResetEvent(false);

            using (Process proc = Process.Start(psi))
            {
                proc.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        standardErrorLines.Add(e.Data);
                    }
                    else
                    {
                        endOfStandardError.Set();
                    }
                };

                proc.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        standardOutputLines.Add(e.Data);
                    }
                    else
                    {
                        endOfStandardOutput.Set();
                    }
                };

                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();

                if (this.Timeout != TimeSpan.MaxValue)
                {
                    if (!proc.WaitForExit((int)this.Timeout.TotalMilliseconds))
                    {
                        proc.CancelErrorRead();
                        proc.CancelOutputRead();
                        proc.Kill();

                        return new ProcessOutput(
                            this.ExecutableFile,
                            arguments,
                            Path.GetFullPath(this.WorkingDirectory),
                            this.Timeout,
                            standardOutputLines,
                            standardErrorLines,
                            -1,
                            false);
                    }
                }
                else
                {
                    proc.WaitForExit();
                }

                endOfStandardOutput.WaitOne();
                endOfStandardError.WaitOne();
                proc.CancelErrorRead();
                proc.CancelOutputRead();

                return new ProcessOutput(
                    this.ExecutableFile,
                    arguments,
                    Path.GetFullPath(this.WorkingDirectory),
                    this.Timeout,
                    standardOutputLines,
                    standardErrorLines,
                    proc.ExitCode,
                    true);
            }
        }

        /// <summary>
        /// Generates arguments to the command line process from the class's properties.
        /// </summary>
        /// <returns>The generated command line process argument string.</returns>
        public virtual string GenerateArguments()
        {
            StringBuilder sb = new StringBuilder();

            var propertyArguments = this.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(ArgumentPropertyAttribute), false).Any());
            foreach (var propertyArgument in propertyArguments)
            {
                var attribute = (ArgumentPropertyAttribute)propertyArgument.GetCustomAttributes(typeof(ArgumentPropertyAttribute), false).Single();

                object value = propertyArgument.GetValue(this, null);

                string propertyArgumentString = ArgumentToString(attribute, propertyArgument, value);

                if (propertyArgumentString != null && sb.Length > 0)
                {
                    sb.Append(' ');
                }

                sb.Append(propertyArgumentString);
            }

            return sb.ToString();
        }

        private static string ArgumentToString(ArgumentPropertyAttribute attribute, PropertyInfo property, object value)
        {
            bool isRequired = attribute.PropertyType == ArgumentPropertyType.Required;
            bool isSwitch = attribute.PropertyType == ArgumentPropertyType.Switch;

            if (value == null)
            {
                if (isRequired)
                {
                    throw new TaupoArgumentException(string.Format(CultureInfo.CurrentCulture, "Missing value for required argument {0}.", property.Name));
                }

                return null;
            }

            if (property.PropertyType.GetBaseType() == typeof(Enum))
            {
                if (!isRequired && (int)value == 0)
                {
                    return null;
                }
            }

            if (value is bool)
            {
                if (isSwitch && (bool)value == false)
                {
                    return null;
                }
            }

            StringBuilder sb = new StringBuilder();

            var collectionValue = value as ICollection;
            if (collectionValue != null)
            {
                var enumerator = collectionValue.GetEnumerator();
                bool first = true;
                while (enumerator.MoveNext())
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        sb.Append(' ');
                    }

                    sb.Append(ArgumentToString(attribute, property, enumerator.Current));
                }

                return sb.ToString();
            }

            sb.Append(attribute.ArgumentName);

            if (!isSwitch)
            {
                sb.Append(QuoteString(value.ToString()));
            }

            return sb.ToString();
        }

        private static string QuoteString(string s)
        {
            if (s.Contains(' '))
            {
                return "\"" + s + "\"";
            }
            else
            {
                return s;
            }
        }
    }
}
