//---------------------------------------------------------------------
// <copyright file="EntityIdSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;

    /// <summary>
    /// A segment representing an entity id represented by $id query option
    /// </summary>
    public sealed class EntityIdSegment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityIdSegment"/> class.
        /// </summary>
        /// <param name="id">Uri correspoding to $id</param>
        internal EntityIdSegment(Uri id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets the original Id Uri for $id.
        /// </summary>
        public Uri Id { get; private set; }
    }
}
