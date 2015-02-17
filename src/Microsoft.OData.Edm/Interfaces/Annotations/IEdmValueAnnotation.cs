//---------------------------------------------------------------------
// <copyright file="IEdmValueAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Annotations
{
    /// <summary>
    /// Represents an EDM value annotation.
    /// </summary>
    public interface IEdmValueAnnotation : IEdmVocabularyAnnotation
    {
        /// <summary>
        /// Gets the expression producing the value of the annotation.
        /// </summary>
        IEdmExpression Value { get; }
    }
}
