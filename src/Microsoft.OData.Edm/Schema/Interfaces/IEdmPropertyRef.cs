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
    public interface IEdmPropertyRef : IEdmNamedElement
    {
        /// <summary>
        /// Gets the property referenced structural property.
        /// </summary>
        IEdmStructuralProperty ReferencedProperty { get; }

        /// <summary>
        /// Gets the value of Alias
        /// </summary>
        string PropertyAlias { get; }
    }
}
