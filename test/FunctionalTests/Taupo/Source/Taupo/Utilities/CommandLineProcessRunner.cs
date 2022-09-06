//---------------------------------------------------------------------
// <copyright file="CommandLineProcessRunner.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Runs command-line processes with given argument string and provides access to standard output and standard error
    /// </summary>
    [ImplementationName(typeof(ICommandLineProcessRunner), "Default")]
    public sealed class CommandLineProcessRunner : CommandLineProcessRunnerBase, ICommandLineProcessRunner
    {
        private string additionalArguments;

        /// <summary>
        /// Initializes a new instance of the CommandLineProcessRunner class.
        /// </summary>
        public CommandLineProcessRunner()
            : base(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CommandLineProcessRunner class.
        /// </summary>
        /// <param name="executable">The executable to run.</param>
        /// <param name="arguments">The arguments to the process.</param>
        public CommandLineProcessRunner(string executable, string arguments)
            : base(executable)
        {
            this.additionalArguments = arguments;
        }

        /// <summary>
        /// Runs the executable with the given paramaters.
        /// </summary>
        /// <param name="executable">The executable to run.</param>
        /// <param name="arguments">The process arguments.</param>
        /// <param name="workingDirectory">The working directory for the process.</param>
        /// <param name="timeout">The timeout for the process.</param>
        /// <returns>The results of running the process.</returns>
        public ProcessOutput Run(string executable, string arguments, string workingDirectory, TimeSpan timeout)
        {
            this.ExecutableFile = executable;
            this.WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory;
            this.Timeout = timeout;
            this.additionalArguments = arguments;
            return this.Run();
        }

        /// <summary>
        /// Returns the arguments provided to the Run method.
        /// </summary>
        /// <returns>The arguments to the process.</returns>
        public override string GenerateArguments()
        {
            return string.IsNullOrEmpty(this.additionalArguments) ? string.Empty : this.additionalArguments;
        }
    }
}
