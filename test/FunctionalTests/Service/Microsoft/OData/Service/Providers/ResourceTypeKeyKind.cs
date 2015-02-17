//---------------------------------------------------------------------
// <copyright file="ResourceTypeKeyKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    /// <summary>
    /// Enumeration for the kind of resource key kind
    /// </summary>
    internal enum ResourceKeyKind
    {
        /// <summary> if the key property was attributed </summary>
        AttributedKey,

        /// <summary> If the key property name was equal to TypeName+ID </summary>
        TypeNameId,

        /// <summary> If the key property name was equal to ID </summary>
        Id,
    }
}
