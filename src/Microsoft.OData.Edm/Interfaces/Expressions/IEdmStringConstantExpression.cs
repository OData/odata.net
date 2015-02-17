//---------------------------------------------------------------------
// <copyright file="IEdmStringConstantExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Values;

namespace Microsoft.OData.Edm.Expressions
{
    /// <summary>
    /// Represents an EDM string constant expression.
    /// </summary>
    public interface IEdmStringConstantExpression : IEdmExpression, IEdmStringValue
    {
    }
}
