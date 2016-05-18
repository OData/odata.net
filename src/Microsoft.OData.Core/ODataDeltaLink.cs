//---------------------------------------------------------------------
// <copyright file="ODataDeltaLink.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;

    /// <summary>
    /// Represents an added link in delta response.
    /// </summary>
    public sealed class ODataDeltaLink : ODataDeltaLinkBase
    {
        /// <summary>
        /// Initializes a new <see cref="ODataDeltaLink"/>.
        /// </summary>
        /// <param name="source">The id of the entity from which the relationship is defined, which may be absolute or relative.</param>
        /// <param name="target">The id of the related entity, which may be absolute or relative.</param>
        /// <param name="relationship">The name of the relationship property on the parent object.</param>
        public ODataDeltaLink(Uri source, Uri target, string relationship)
            : base(source, target, relationship)
        {
        }
    }
}
