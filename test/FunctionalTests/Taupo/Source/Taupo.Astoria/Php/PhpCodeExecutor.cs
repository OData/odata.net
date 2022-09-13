//---------------------------------------------------------------------
// <copyright file="PhpCodeExecutor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Php
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Code Executor for Php
    /// </summary>
    [ImplementationName(typeof(IPhpCodeExecutor), "Php")]
    public class PhpCodeExecutor : IPhpCodeExecutor
    {
        /// <summary>
        /// Gets or sets the Working directory for the PHP client library
        /// </summary>
        [InjectTestParameter("PhpWorkingDirectory", DefaultValueDescription = "C:\\PHPLib\\odataphp", HelpText = "Working Directory for the PHP Client library")]
        public string PhpWorkingDirectory { get; set; }

        /// <summary>
        /// Gets or sets the logger used to print diagnostic messages.
        /// </summary>
        /// <value>The logger.</value>
        [InjectDependency]
        public Logger Logger { get; set; }

        /// <summary>
        /// Gets or sets the Process Runner to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ICommandLineProcessRunner ProcessRunner { get; set; }

        /// <summary>
        /// Executes the generated code
        /// </summary>
        /// <param name="command">The command to use to execute the generated code</param>
        /// <returns>Returns the result of executing the code</returns>
        public string ExecuteCode(string command)
        {
            string phpExecuteCommand = string.Format(CultureInfo.InvariantCulture, "/c ..\\php.exe {0}", command);
            var output = this.ProcessRunner.Run("cmd", phpExecuteCommand, this.PhpWorkingDirectory, TimeSpan.MaxValue);
            if (output.StandardErrorLines.Count != 0)
            {
                return string.Join("\n", output.StandardErrorLines);
            }

            return string.Join(string.Empty, output.StandardOutputLines).Trim(new char[] { '\n', ' ' });
        }
    }
}
