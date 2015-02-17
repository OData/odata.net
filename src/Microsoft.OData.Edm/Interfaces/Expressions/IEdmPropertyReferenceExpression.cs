//---------------------------------------------------------------------
// <copyright file="IEdmPropertyReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Expressions
{
    /// <summary>
    /// Represents an EDM property reference expression.
    /// </summary>
    public interface IEdmPropertyReferenceExpression : IEdmExpression
    {
        /// <summary>
        /// Gets the expression for the structured value containing the referenced property.
        /// </summary>
        IEdmExpression Base { get; }

        /// <summary>
        /// Gets the referenced property.
        /// </summary>
        IEdmProperty ReferencedProperty { get; }
    }
}
