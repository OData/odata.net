//---------------------------------------------------------------------
// <copyright file="IEdmStructuralProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM structural (i.e. non-navigation) property.
    /// </summary>
    public interface IEdmStructuralProperty : IEdmProperty
    {
        /// <summary>
        /// Gets the default value of this property.
        /// </summary>
        string DefaultValueString { get; }
    }
}
