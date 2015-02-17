//---------------------------------------------------------------------
// <copyright file="IEdmDateConstantExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Expressions
{
    using Microsoft.OData.Edm.Values;

    /// <summary>
    /// Represents an EDM date constant expression.
    /// </summary>
    public interface IEdmDateConstantExpression : IEdmExpression, IEdmDateValue
    {
    }
}
