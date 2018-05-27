//---------------------------------------------------------------------
// <copyright file="ODataPropertyKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// The enum of property kinds.
    /// </summary>
    public enum ODataPropertyKind
    {
        /// <summary>
        /// Unspecified property kind or if the property is not a key property, an etag property or an open property.
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// The property is a key property.
        /// </summary>
        Key,

        /// <summary>
        /// The property is an etag property
        /// </summary>
        ETag,

        /// <summary>
        /// The property is an open property
        /// </summary>
        Open
    }
}