//---------------------------------------------------------------------
// <copyright file="BatchResponseChangeset.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;

    /// <summary>
    /// Data structure representing a changeset from a response from $batch
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Doesn't need to end in 'Collection'")]
    public class BatchResponseChangeset : BatchChangeset<HttpResponseData>
    {
        /// <summary>
        /// Initializes a new instance of the BatchResponseChangeset class. Do not construct directly, use BatchPayloadBuilder instead.
        /// </summary>
        public BatchResponseChangeset()
            : base()
        {
        }
    }
}
