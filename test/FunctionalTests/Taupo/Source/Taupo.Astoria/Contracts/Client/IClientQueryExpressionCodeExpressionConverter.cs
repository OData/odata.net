//---------------------------------------------------------------------
// <copyright file="IClientQueryExpressionCodeExpressionConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System.CodeDom;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Contract for generating non Astoria specific code expressions
    /// </summary>
    [ImplementationSelector("ClientQueryExpressionCodeExpressionConverter", DefaultImplementation = "Default")]
    public interface IClientQueryExpressionCodeExpressionConverter
    {
        /// <summary>
        /// Generates code for a QueryExpression
        /// NOTE:do not use for expression that contain Astoria specific expressions
        /// </summary>
        /// <param name="queryExpression">Query Expression</param>
        /// <returns>Code Expression</returns>
        CodeExpression Convert(QueryExpression queryExpression);
    }
}
