//---------------------------------------------------------------------
// <copyright file="IEdmOperationReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM operation reference expression.
    /// </summary>
    public interface IEdmOperationReferenceExpression : IEdmExpression
    {
        /// <summary>
        /// Gets the referenced operation.
        /// </summary>
        IEdmOperation ReferencedOperation { get; }
    }
}
