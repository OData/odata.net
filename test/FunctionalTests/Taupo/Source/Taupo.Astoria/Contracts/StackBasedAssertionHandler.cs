//---------------------------------------------------------------------
// <copyright file="StackBasedAssertionHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Assertion handler that allows a stack of messages to be set up for more useful errors when comparing trees
    /// </summary>
    [ImplementationSelector("StackBasedAssertionHandler", DefaultImplementation = "Default")]
    public abstract class StackBasedAssertionHandler : AssertionHandler
    {
        /// <summary>
        /// Adds a new message to the assertion handler's stack
        /// </summary>
        /// <param name="message">The message to add</param>
        /// <param name="args">The arguments to the message</param>
        /// <returns>A disposable object that will remove the message when disposed</returns>
        public abstract IDisposable WithMessage(string message, params object[] args);
    }
}