//---------------------------------------------------------------------
// <copyright file="LinqToAstoriaQueryResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.LinqToAstoria
{
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Resolves Linq expression.
    /// </summary>
    [ImplementationName(typeof(ILinqToAstoriaQueryResolver), "Default")]
    public class LinqToAstoriaQueryResolver : ILinqToAstoriaQueryResolver
    {
        private LinqToAstoriaParameterNameResolutionVisitor parameterNameResolutionVisitor;
        private LinqToAstoriaTypeResolutionVisitor typeResolver;
        private LinqToAstoriaCustomFunctionResolutionVisitor customFunctionResolutionVisitor;

        /// <summary>
        /// Initializes a new instance of the LinqToAstoriaQueryResolver class.
        /// </summary>
        /// <param name="identifierGenerator">Identifier generator.</param>
        /// <param name="typeLibrary">The query type library.</param>
        public LinqToAstoriaQueryResolver(IIdentifierGenerator identifierGenerator, QueryTypeLibrary typeLibrary)
        {
            this.parameterNameResolutionVisitor = new LinqToAstoriaParameterNameResolutionVisitor(identifierGenerator);
            this.typeResolver = new LinqToAstoriaTypeResolutionVisitor(typeLibrary);
            this.customFunctionResolutionVisitor = new LinqToAstoriaCustomFunctionResolutionVisitor(this.typeResolver);
        }

        /// <summary>
        /// Gets or sets the query evaluation strategy
        /// </summary>
        /// <value>The query evaluation strategy.</value>
        [InjectDependency(IsRequired = true)]
        public ILinqToAstoriaQueryEvaluationStrategy QueryEvaluationStrategy { get; set; }

        /// <summary>
        /// Resolves Linq expression.
        /// </summary>
        /// <param name="expression">Expression to resolve.</param>
        /// <returns>Resolved expression.</returns>
        public QueryExpression Resolve(QueryExpression expression)
        {
            ExceptionUtilities.CheckArgumentNotNull(expression, "expression");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            var resolvedExpression = this.customFunctionResolutionVisitor.ResolveCustomFunctions(expression, this.QueryEvaluationStrategy);
            resolvedExpression = this.parameterNameResolutionVisitor.ResolveParameterNames(expression);
            resolvedExpression = this.typeResolver.ResolveTypes(resolvedExpression, this.QueryEvaluationStrategy);

            return resolvedExpression;
        }
    }
}
