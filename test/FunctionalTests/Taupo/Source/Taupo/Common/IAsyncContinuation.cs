//---------------------------------------------------------------------
// <copyright file="IAsyncContinuation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;

    /// <summary>
    /// Continuation for running a sequence of asynchronous operations
    /// </summary>
    public interface IAsyncContinuation
    {
        /// <summary>
        /// Indicates the current operation has completed successfully and the next operation should begin
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", 
            Justification = "Correct name for what the method does. If this ever becomes an issue, we will rename it to 'Complete' or something similar. For now, 'Continue' is better.")]
        void Continue();

        /// <summary>
        /// Indicates that the current operation has failed
        /// </summary>
        /// <param name="exception">The exception.</param>
        void Fail(Exception exception);
    }
}
