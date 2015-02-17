//---------------------------------------------------------------------
// <copyright file="IClientQuerySingleVariationCodeBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System.CodeDom;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Contract for generating a single expression variation
    /// </summary>
    [ImplementationSelector("ClientQuerySingleVariationCodeBuilder", DefaultImplementation = "Default")]
    public interface IClientQuerySingleVariationCodeBuilder
    {
        /// <summary>
        /// Builds a Single Variation method
        /// </summary>
        /// <param name="codeBuilder">Code Builder</param>
        /// <param name="expression">Query Expression to use.</param>
        /// <param name="expectedClientErrorValue">Expected Client Error Value</param>
        /// <param name="contextVariable">Context Variable</param>
        /// <param name="resultComparer">Result Comparer</param>
        void Build(CodeBuilder codeBuilder, QueryExpression expression, CodeExpression expectedClientErrorValue, CodeExpression contextVariable, CodeExpression resultComparer);
    }
}
