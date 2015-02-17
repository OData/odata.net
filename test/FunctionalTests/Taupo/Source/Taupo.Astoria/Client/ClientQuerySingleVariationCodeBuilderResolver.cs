//---------------------------------------------------------------------
// <copyright file="ClientQuerySingleVariationCodeBuilderResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;

    /// <summary>
    /// Creates a expression variation code builder
    /// </summary>
    [ImplementationName(typeof(IClientQuerySingleVariationCodeBuilderResolver), "Default")]
    public class ClientQuerySingleVariationCodeBuilderResolver : IClientQuerySingleVariationCodeBuilderResolver
    {
        /// <summary>
        /// Gets or sets InvokeAction Single Variation Builder
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ClientQueryInvokeActionSingleVariationCodeBuilder ClientQueryInvokeActionSingleVariationCodeBuilder { get; set; }

        /// <summary>
        /// Gets or sets the ClientQuerySingleVariationCodeBuilder
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ClientQuerySingleVariationCodeBuilder ClientQuerySingleVariationCodeBuilder { get; set; }

        /// <summary>
        /// Creates a generator that will build code for based on the type of expression andClient Execute expression patterns
        /// </summary>
        /// <param name="query">Query To use to Resolve to a builder</param>
        /// <returns>Generator to return</returns>
        public IClientQuerySingleVariationCodeBuilder Resolve(QueryExpression query)
        {
            var actionCallExpression = query.ExtractAction();
            var serviceOperationCallExpression = query.ExtractServiceOperation();
            if (serviceOperationCallExpression != null && serviceOperationCallExpression.Function.Annotations.OfType<FunctionBodyAnnotation>().Single().IsRoot)
            {
                // TODO: Fix service operation test coverage in Taupo.Client
                return null;
            }

            ClientQuerySingleVariationCodeBuilderBase clientQueryVariationCodeBuilder = null;
            if (actionCallExpression != null || serviceOperationCallExpression != null)
            {
                clientQueryVariationCodeBuilder = this.ClientQueryInvokeActionSingleVariationCodeBuilder;
            }
            else
            {
                clientQueryVariationCodeBuilder = this.ClientQuerySingleVariationCodeBuilder;
            }

            return clientQueryVariationCodeBuilder;
        }
    }
}