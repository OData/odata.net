//---------------------------------------------------------------------
// <copyright file="IEdmDateTimeOffsetConstantExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM datetime with offset constant expression.
    /// </summary>
    public interface IEdmDateTimeOffsetConstantExpression : IEdmExpression, IEdmDateTimeOffsetValue
    {
    }
}
