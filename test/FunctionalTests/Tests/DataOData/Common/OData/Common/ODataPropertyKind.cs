//---------------------------------------------------------------------
// <copyright file="ODataPropertyKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    #endregion Namespaces
    
    /// <summary>
    /// Enumeration for the kinds of property a resource can have.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1714", Justification = "Avoid unnecesarily changing existing code")]
    [Flags]
    public enum ODataPropertyKind
    {
        /// <summary>A primitive type property.</summary>
        Primitive = 0x1,

        /// <summary>A property that is part of the key.</summary>
        Key = 0x2,

        /// <summary>A complex (compound) property.</summary>
        ComplexType = 0x4,

        /// <summary>A reference to another entity.</summary>
        EntityReference = 0x8,

        /// <summary>A reference to an entity set.</summary>
        EntitySetReference = 0x10,

        /// <summary>Whether this property is a etag property.</summary>
        ETag = 0x20,

        /// <summary>A collection of primitive or complex types.</summary>
        Collection = 0x40,

        /// <summary>A Named Resource Stream</summary>
        Stream = 0x80
    }
}
