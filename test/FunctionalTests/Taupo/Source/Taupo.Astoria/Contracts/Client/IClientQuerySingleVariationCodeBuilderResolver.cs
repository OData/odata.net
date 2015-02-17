//---------------------------------------------------------------------
// <copyright file="IClientQuerySingleVariationCodeBuilderResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System.CodeDom;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Contract for generating client code layers.
    /// </summary>
    [ImplementationSelector("ClientQuerySingleVariationCodeBuilderResolver", DefaultImplementation = "Default")]
    public interface IClientQuerySingleVariationCodeBuilderResolver
    {
        /// <summary>
        /// Creates a generator that will build code for based on the type of expression andClient Execute expression patterns
        /// </summary>
        /// <param name="query">Query To use to Resolve to a builder</param>
        /// <returns>Generator to return</returns>
        IClientQuerySingleVariationCodeBuilder Resolve(QueryExpression query);
    }
}
