//---------------------------------------------------------------------
// <copyright file="IEdmStructuredType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Common base interface for definitions of EDM structured types.
    /// </summary>
    public interface IEdmStructuredType : IEdmType
    {
        /// <summary>
        /// Gets a value indicating whether this type is abstract.
        /// </summary>
        bool IsAbstract { get; }

        /// <summary>
        /// Gets a value indicating whether this type is open.
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Gets the base type of this type.
        /// </summary>
        IEdmStructuredType BaseType { get; }

        /// <summary>
        /// Gets the properties declared immediately within this type.
        /// </summary>
        IEnumerable<IEdmProperty> DeclaredProperties { get; }

        /// <summary>
        /// Searches for a structural or navigation property with the given name in this type and all base types and returns null if no such property exists.
        /// </summary>
        /// <param name="name">The name of the property being found.</param>
        /// <returns>The requested property, or null if no such property exists.</returns>
        IEdmProperty FindProperty(string name);
    }
}
