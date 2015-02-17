//---------------------------------------------------------------------
// <copyright file="IClientExpectedErrorComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for comparing errors that occur in the Client
    /// </summary>
    [ImplementationSelector("ClientExpectedErrorComparer", DefaultImplementation = "Default", HelpText = "Used to compare against Client exceptions")]
    public interface IClientExpectedErrorComparer
    {
        /// <summary>
        /// Compares the ExpectedClientErrorBaseline to the provided exception and verifies the exception is correct
        /// </summary>
        /// <param name="expectedClientException">Expected Client Exception</param>
        /// <param name="exception">Actual Exception</param>
        void Compare(ExpectedClientErrorBaseline expectedClientException, Exception exception);
    }
}
