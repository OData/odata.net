//---------------------------------------------------------------------
// <copyright file="IEdmPropertyRef.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a property reference element.
    /// The edm:PropertyRef element MUST contain the Name attribute and MAY contain the Alias attribute.
    /// </summary>
    public interface IEdmPropertyRef : IEdmElement
    {
        /// <summary>
        /// Gets the referenced structural property. It's the leading to a primitive property.
        /// </summary>
        IEdmStructuralProperty ReferencedProperty { get; }

        /// <summary>
        /// The value of Name is a path expression leading to a primitive property. The names of the properties in the path are joined together by forward slashes.
        /// Since it's a path, let's name it as 'Path'.
        /// </summary>
        IEdmPathExpression Path { get; }

        /// <summary>
        /// Gets the value of Alias
        /// </summary>
        string PropertyAlias { get; }
    }
}
