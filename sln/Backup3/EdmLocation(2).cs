//---------------------------------------------------------------------
// <copyright file="EdmLocation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents the location of an Edm item.
    /// </summary>
    public abstract class EdmLocation
    {
        /// <summary>
        /// Gets a string representation of the location.
        /// </summary>
        /// <returns>A string representation of the location.</returns>
        public abstract override string ToString();
    }
}
