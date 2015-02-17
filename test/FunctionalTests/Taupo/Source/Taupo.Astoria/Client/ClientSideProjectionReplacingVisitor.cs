//---------------------------------------------------------------------
// <copyright file="ClientSideProjectionReplacingVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// Default implementation of the client-side projection replacing visitor
    /// </summary>
    [ImplementationName(typeof(IClientSideProjectionReplacingVisitor), "Default")]
    public class ClientSideProjectionReplacingVisitor : LinqToAstoriaExpressionReplacingVisitor, IClientSideProjectionReplacingVisitor, ILinqToAstoriaExpressionVisitor<QueryExpression>
    {
        private readonly List<QueryExpression> propertyExpressions = new List<QueryExpression>();

        /// <summary>
        /// Gets or sets the identifier generator.
        /// </summary>
        /// <value>
        /// The identifier generator.
        /// </value>
        [InjectDependency(IsRequired = true)]
        public IIdentifierGenerator IdentifierGenerator { get; set; }

        /// <summary>
        /// Replaces projections that will be evaluated client-side with what will be sent to the server.
        /// </summary>
        /// <param name="queryExpression">The query expression to do replacement on.</param>
        /// <returns>
        /// The expression with replacements
        /// </returns>
        public QueryExpression ReplaceClientSideProjections(QueryExpression queryExpression)
        {
            return queryExpression.Accept(this);
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>
        /// Replaced expression.
        /// </returns>
        public override QueryExpression Visit(LinqSelectExpression expression)
        {
            // in a select, only properties will be sent
            this.propertyExpressions.Clear();
            expression.Lambda.Accept(this);

            if (this.propertyExpressions.Count == 0)
            {
                return expression.Source;
            }

            var newExpression = LinqBuilder.New(this.propertyExpressions.Select(p => this.IdentifierGenerator.GenerateIdentifier("temp")), this.propertyExpressions);
            var lambda = LinqBuilder.Lambda(newExpression, expression.Lambda.Parameters.ToArray());
            return expression.Source.Select(lambda);
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>
        /// Replaced expression.
        /// </returns>
        public override QueryExpression Visit(QueryPropertyExpression expression)
        {
            // sub properties of scalar types do not get sent to the server
            // TODO: should this even include sub-properties of complex types?
            if (expression.Instance.ExpressionType is QueryScalarType)
            {
                return base.Visit(expression);
            }
            else
            {
                this.propertyExpressions.Add(expression);
                return expression;
            }
        }
    }
}
