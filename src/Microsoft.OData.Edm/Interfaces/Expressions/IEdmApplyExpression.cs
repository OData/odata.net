//---------------------------------------------------------------------
// <copyright file="IEdmApplyExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Expressions
{
    /// <summary>
    /// Represents an EDM function application expression.
    /// </summary>
    public interface IEdmApplyExpression : IEdmExpression
    {
        /// <summary>
        /// Gets the applied function.
        /// </summary>
        IEdmExpression AppliedOperation { get; }

        /// <summary>
        /// Gets the arguments to the function.
        /// </summary>
        IEnumerable<IEdmExpression> Arguments { get; }
    }
}
