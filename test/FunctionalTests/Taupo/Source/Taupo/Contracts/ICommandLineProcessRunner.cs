//---------------------------------------------------------------------
// <copyright file="ICommandLineProcessRunner.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Executes non-interactive command-line program and returns results.
    /// </summary>
    [ImplementationSelector("CommandLineProcessRunner", DefaultImplementation = "Default")]
    public interface ICommandLineProcessRunner
    {
        /// <summary>
        /// Runs the specified executable.
        /// </summary>
        /// <param name="executable">The executable.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="workingDirectory">The working directory.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>Output of the process wrapped in <see cref="ProcessOutput"/> object.</returns>
        ProcessOutput Run(string executable, string arguments, string workingDirectory, TimeSpan timeout);
    }
}