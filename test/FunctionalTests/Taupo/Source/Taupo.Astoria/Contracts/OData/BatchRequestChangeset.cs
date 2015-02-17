//---------------------------------------------------------------------
// <copyright file="BatchRequestChangeset.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;

    /// <summary>
    /// Data structure representing a changeset from a request to $batch
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Doesn't need to end in 'Collection'")]
    public class BatchRequestChangeset : BatchChangeset<IHttpRequest>
    {
        /// <summary>
        /// Initializes a new instance of the BatchRequestChangeset class. Do not construct directly, use BatchPayloadBuilder instead.
        /// </summary>
        public BatchRequestChangeset()
            : base()
        {
        }
    }
}
