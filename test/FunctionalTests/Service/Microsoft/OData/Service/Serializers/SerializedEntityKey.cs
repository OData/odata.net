//---------------------------------------------------------------------
// <copyright file="SerializedEntityKey.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Serializers
{
    using System;

    /// <summary>
    /// Data structure for representing the identity and edit-link of an entity being serialized.
    /// </summary>
    internal abstract class SerializedEntityKey
    {
        /// <summary>
        /// Gets the edit link of the entity relative to the service base.
        /// </summary>
        internal abstract Uri RelativeEditLink { get; }

        /// <summary>
        /// Gets the identity of the entity.
        /// </summary>
        internal abstract Uri Identity { get; }

        /// <summary>
        /// Gets the absolute edit link of the entity.
        /// </summary>
        internal abstract Uri AbsoluteEditLink { get; }

        /// <summary>
        /// Gets the absolute edit link of the entity without a type segment or other suffix.
        /// </summary>
        internal abstract Uri AbsoluteEditLinkWithoutSuffix { get; }
    }
}