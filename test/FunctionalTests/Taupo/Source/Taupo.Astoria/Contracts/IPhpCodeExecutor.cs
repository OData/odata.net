//---------------------------------------------------------------------
// <copyright file="IPhpCodeExecutor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for Non-Clr Code Executor
    /// </summary>
    [ImplementationSelector("PhpCodeExecutor", DefaultImplementation = "Php")]
    public interface IPhpCodeExecutor
    {
        /// <summary>
        /// Executes the Generated Non-Clr Code
        /// </summary>
        /// <param name="command">The command to be executed</param>
        /// <returns>The result of the executed code</returns>
        string ExecuteCode(string command);
    }
}
