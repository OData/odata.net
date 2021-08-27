//---------------------------------------------------------------------
// <copyright file="ISandboxObjectFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;

    /// <summary>
    /// A factory for creating <see cref="TestItem"/>s in various contexts.
    /// </summary>
    [ImplementationSelector("SandboxObjectFactory", DefaultImplementation = "Default", HelpText = "Manages object creation in a sandbox.", IsGlobal = true)]
    public interface ISandboxObjectFactory
    {
        /// <summary>
        /// Creates an object of the specified <paramref name="type"/>, passing
        /// the specified <paramref name="args"/> to its constructor.
        /// </summary>
        /// <param name="type">The type of <see cref="TestItem"/> to create.</param>
        /// <param name="args">The arguments to pass to the constructor.</param>
        /// <returns>An instance of <paramref name="type"/>.</returns>
        object CreateInstance(Type type, params object[] args);
    }
}
