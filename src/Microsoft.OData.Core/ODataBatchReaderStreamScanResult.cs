//---------------------------------------------------------------------
// <copyright file="ODataBatchReaderStreamScanResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// An enumeration representing the result of a scan operation through
    /// the batch reader stream's buffer.
    /// </summary>
    internal enum ODataBatchReaderStreamScanResult
    {
        /// <summary>No match with the requested boundary was found (not even a partial one).</summary>
        NoMatch,

        /// <summary>A partial match with the requested boundary was found.</summary>
        PartialMatch,

        /// <summary>A complete match with the requested boundary was found.</summary>
        /// <remarks>
        /// This is only returned if we could also check whether the boundary is an end
        /// boundary or not; otherwise a partial match is returned.
        /// </remarks>
        Match,
    }
}
