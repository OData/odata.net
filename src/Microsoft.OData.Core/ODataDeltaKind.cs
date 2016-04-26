//---------------------------------------------------------------------
// <copyright file="ODataDeltaKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// Delta kinds
    /// </summary>
    internal enum ODataDeltaKind
    {
        /// <summary>None delta</summary>
        None,

        /// <summary>Delta resource set</summary>
        ResourceSet,

        /// <summary>Delta resource</summary>
        Resource,

        /// <summary>Delta deleted resource</summary>
        DeletedEntry,

        /// <summary>Delta link</summary>
        Link,

        /// <summary>Delta deleted link</summary>
        DeletedLink,
    }
}
