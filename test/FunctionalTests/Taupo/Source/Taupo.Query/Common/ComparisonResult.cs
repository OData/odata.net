//---------------------------------------------------------------------
// <copyright file="ComparisonResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Common
{
    /// <summary>
    /// Result of the comparison
    /// </summary>
    public enum ComparisonResult
    {
        /// <summary>
        /// Comparison successful.
        /// </summary>
        Success,

        /// <summary>
        /// Comparison failed.
        /// </summary>
        Failure,

        /// <summary>
        /// Comparison skipped.
        /// </summary>
        Skipped,
    }
}
