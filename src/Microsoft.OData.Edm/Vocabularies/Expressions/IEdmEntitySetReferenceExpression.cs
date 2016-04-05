//---------------------------------------------------------------------
// <copyright file="IEdmEntitySetReferenceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM entity set reference expression.
    /// </summary>
    public interface IEdmEntitySetReferenceExpression : IEdmExpression
    {
        /// <summary>
        /// Gets the referenced entity set.
        /// </summary>
        IEdmEntitySet ReferencedEntitySet { get; }
    }
}
