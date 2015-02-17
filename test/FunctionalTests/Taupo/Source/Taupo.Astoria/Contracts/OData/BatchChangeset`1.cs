//---------------------------------------------------------------------
// <copyright file="BatchChangeset`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;

    /// <summary>
    /// Base class for batch changesets
    /// </summary>
    /// <typeparam name="TOperation">The type of individual operations the payload contains</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Doesn't need to end in 'Collection'")]
    public abstract class BatchChangeset<TOperation> : MultipartMimeData<MimePartData<TOperation>> where TOperation : IMimePart
    {
        /// <summary>
        /// Gets the operations that are in the changeset
        /// </summary>
        public IEnumerable<TOperation> Operations
        {
            get
            {
                return this.Select(p => p.Body);
            }
        }
    }
}
