//---------------------------------------------------------------------
// <copyright file="EdmxTarget.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl
{
    /// <summary>
    /// Specifies what target of an EDMX file.
    /// </summary>
    public enum EdmxTarget
    {
        /// <summary>
        /// The target is Entity Framework.
        /// </summary>
        EntityFramework,

        /// <summary>
        /// The target is OData.
        /// </summary>
        OData
    }
}
