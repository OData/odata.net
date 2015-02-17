//---------------------------------------------------------------------
// <copyright file="CollectionKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Collection kind for a Member Property (Csdl 1.1 concept)
    /// </summary>
    public enum CollectionKind
    {
        /// <summary>
        /// Not a collection (default)
        /// </summary>
        None,

        /// <summary>
        /// Collection of Bag semantics (no duplicate)
        /// </summary>
        Bag,
        
        /// <summary>
        /// Collection of List semantics (can have duplicate)
        /// </summary>
        List,
    }
}
