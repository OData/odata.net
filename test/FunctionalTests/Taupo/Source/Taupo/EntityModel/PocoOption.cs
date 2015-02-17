//---------------------------------------------------------------------
// <copyright file="PocoOption.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    /// <summary>
    /// Represents the kinds of POCO generation
    /// </summary>
    public enum PocoOption 
    {
        /// <summary>
        /// Represents Pure POCO classes where none of the properties are virtual
        /// </summary>
        None,

        /// <summary>
        /// Represents POCO classes where none of the properties are virtual
        /// </summary>
        Pure, 

        /// <summary>
        /// Represents POCO classes with all navigation properties virtual
        /// </summary>
        NavigationPropertiesVirtual,

        /// <summary>
        /// Represents POCO classes where all properties are virtual
        /// </summary>
        AllPropertiesVirtual, 
    }
}
