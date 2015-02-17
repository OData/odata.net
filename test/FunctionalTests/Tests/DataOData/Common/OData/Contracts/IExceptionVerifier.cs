//---------------------------------------------------------------------
// <copyright file="IExceptionVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Contracts
{
    #region Namespaces
    using System;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Interface for verifying expected exception details.
    /// </summary>
    [ImplementationSelector("ExceptionVerifier", DefaultImplementation="Default", HelpText="Used to verify expected exceptions that occur in tests")]
    public interface IExceptionVerifier
    {
        /// <summary>
        /// Verifies that the actual exception matches expectations.
        /// </summary>
        /// <param name="expected">The expected exception details.</param>
        /// <param name="actual">The actual exception to verify.</param>
        void VerifyExceptionResult(ExpectedException expected, Exception actual);
    }
}
