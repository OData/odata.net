//---------------------------------------------------------------------
// <copyright file="LinqToAstoriaQueryExpressionExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;

    /// <summary>
    /// Helpers for understanding QueryExpressions
    /// </summary>
    public static class LinqToAstoriaQueryExpressionExtensions 
    {
        /// <summary>
        /// Returns an action if there is one in the Query
        /// </summary>
        /// <param name="query">Query to visit</param>
        /// <returns>Function that is an action or null if nothing found</returns>
        public static QueryCustomFunctionCallExpression ExtractAction(this QueryExpression query)
        {
            ExceptionUtilities.CheckArgumentNotNull(query, "query");
            return new ActionCallExtractingVisitor().ExtractActionCall(query);
        }

        /// <summary>
        /// Returns a service opertion if there is one in the Query
        /// </summary>
        /// <param name="query">Query to visit</param>
        /// <returns>Function that is a serive operation or null if nothing found</returns>
        public static QueryCustomFunctionCallExpression ExtractServiceOperation(this QueryExpression query)
        {
            ExceptionUtilities.CheckArgumentNotNull(query, "query");
            return new ServiceOperationCallExtractingVisitor().ExtractServiceOperationCall(query);
        }

        /// <summary>
        /// Class visits QueryExpression and indicates whether this expression has an Action or not
        /// </summary>
        private class ActionCallExtractingVisitor : LinqToAstoriaExpressionReplacingVisitor
        {
            private QueryCustomFunctionCallExpression actionCallExpression;

            public QueryCustomFunctionCallExpression ExtractActionCall(QueryExpression expression)
            {
                this.actionCallExpression = null;
                expression.Accept(this);
                return this.actionCallExpression;
            }

            /// <summary>
            /// Visits the Query Expression and finds if there is an Action Function Called
            /// </summary>
            /// <param name="expression">Expression to Visit</param>
            /// <returns>Query Expression</returns>
            public override QueryExpression Visit(QueryCustomFunctionCallExpression expression)
            {
                if (expression.Function.IsAction())
                {
                    this.actionCallExpression = expression;
                }

                return base.Visit(expression);
            }
        }

        /// <summary>
        /// Class visits QueryExpression and indicates whether this expression has a serive operation or not
        /// </summary>
        private class ServiceOperationCallExtractingVisitor : LinqToAstoriaExpressionReplacingVisitor
        {
            private QueryCustomFunctionCallExpression serviceOperationCallExpression;

            public QueryCustomFunctionCallExpression ExtractServiceOperationCall(QueryExpression expression)
            {
                this.serviceOperationCallExpression = null;
                expression.Accept(this);
                return this.serviceOperationCallExpression;
            }

            /// <summary>
            /// Visits the Query Expression and finds if there is an service operation Called
            /// </summary>
            /// <param name="expression">Expression to Visit</param>
            /// <returns>Query Expression</returns>
            public override QueryExpression Visit(QueryCustomFunctionCallExpression expression)
            {
                if (expression.Function.IsServiceOperation())
                {
                    this.serviceOperationCallExpression = expression;
                }

                return base.Visit(expression);
            }
        }
    }
}
