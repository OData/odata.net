//---------------------------------------------------------------------
// <copyright file="IEdmPropertyValueBinding.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents a property binding specified as part of an EDM type annotation.
    /// </summary>
    public interface IEdmPropertyValueBinding : IEdmElement
    {
        /// <summary>
        /// Gets the property that is given a value by the annotation.
        /// </summary>
        IEdmProperty BoundProperty { get; }

        /// <summary>
        /// Gets the expression producing the value of the annotation.
        /// </summary>
        IEdmExpression Value { get; }
    }
}