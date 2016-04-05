//---------------------------------------------------------------------
// <copyright file="IEdmParameterReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM parameter reference expression.
    /// </summary>
    public interface IEdmParameterReferenceExpression : IEdmExpression
    {
        /// <summary>
        /// Gets the referenced parameter.
        /// </summary>
        IEdmOperationParameter ReferencedParameter { get; }
    }
}
