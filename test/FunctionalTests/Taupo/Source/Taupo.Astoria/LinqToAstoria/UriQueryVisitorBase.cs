//---------------------------------------------------------------------
// <copyright file="UriQueryVisitorBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.LinqToAstoria
{
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// Visitor base class for generating the URI query string
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling",
        Justification = "Visitor pattern forces coupling")]
    public abstract class UriQueryVisitorBase : ILinqToAstoriaExpressionVisitor<string>
    {
        /// <summary>
        /// Gets or sets clr to data-type converter to use for primitive type checks in the uri
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IClrToPrimitiveDataTypeConverter PrimitiveDataTypeConverter { get; set; }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqAllExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqAllExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqAnyExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqAnyExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqAsEnumerableExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqAsEnumerableExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqBitwiseAndExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqBitwiseAndExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqBitwiseOrExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqBitwiseOrExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqBuiltInFunctionCallExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqBuiltInFunctionCallExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqCastExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqCastExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqConcatExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqConcatExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqContainsExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqContainsExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqCountExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqCountExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqDefaultIfEmptyExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqDefaultIfEmptyExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqDistinctExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqDistinctExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqExceptExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqExceptExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqExclusiveOrExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqExclusiveOrExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqFirstExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqFirstExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqFirstOrDefaultExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqFirstOrDefaultExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqFreeVariableExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqFreeVariableExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqGroupByExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqGroupByExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqGroupJoinExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqGroupJoinExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqJoinExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqJoinExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqLambdaExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqLambdaExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqLengthPropertyExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqLengthPropertyExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqLongCountExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqLongCountExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqMaxExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqMaxExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqMemberMethodExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqMemberMethodExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqMinExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqMinExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqNewArrayExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqNewArrayExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqNewExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqNewExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqNewInstanceExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqNewInstanceExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqOrderByExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqOrderByExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqParameterExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqParameterExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqSelectExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqSelectExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqSelectManyExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqSelectManyExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqSingleExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqSingleExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqSingleOrDefaultExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqSingleOrDefaultExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqSkipExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqSkipExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqTakeExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqTakeExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqToAstoriaAddQueryOptionExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqToAstoriaAddQueryOptionExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqToAstoriaConditionalExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqToAstoriaConditionalExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqToAstoriaExpandExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqToAstoriaExpandExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a LinqToAstoriaExpandLambdaExpression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The result of visiting this expression.</returns>
        public virtual string Visit(LinqToAstoriaExpandLambdaExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqToAstoriaKeyExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqToAstoriaKeyExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqToAstoriaLinksExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqToAstoriaLinksExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqToAstoriaValueExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqToAstoriaValueExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqUnionExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqUnionExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqWhereExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(LinqWhereExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryAddExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryAddExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryAndExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryAndExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryAsExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryAsExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryCastExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryCastExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryConstantExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryConstantExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryCustomFunctionCallExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryCustomFunctionCallExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryDivideExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryDivideExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryEqualToExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryEqualToExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryFunctionImportCallExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryFunctionImportCallExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryFunctionParameterReferenceExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryFunctionParameterReferenceExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqBuiltInFunctionCallExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryGreaterThanExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryGreaterThanOrEqualToExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryGreaterThanOrEqualToExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryIsNotNullExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryIsNotNullExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryIsNullExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryIsNullExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryIsOfExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryIsOfExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryLessThanExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryLessThanExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryLessThanOrEqualToExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryLessThanOrEqualToExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryModuloExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryModuloExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryMultiplyExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryMultiplyExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryNotEqualToExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryNotEqualToExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryNotExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryNotExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryNullExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryNullExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryOfTypeExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryOfTypeExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryOrExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryOrExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryPropertyExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryPropertyExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryRootExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QueryRootExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QuerySubtractExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public virtual string Visit(QuerySubtractExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Gets EDM type for primtive, entity and complex types
        /// </summary>
        /// <param name="type">The Query Type.</param>
        /// <returns>The EDM Type in string format.</returns>
        protected string GetEdmTypeName(QueryType type)
        {
            QueryComplexType complexType = type as QueryComplexType;
            QueryEntityType entityType = type as QueryEntityType;
            IQueryClrType clrBackedType = type as IQueryClrType;

            if (complexType != null)
            {
                return complexType.ComplexType.FullName;
            }
            else if (entityType != null)
            {
                return entityType.EntityType.FullName;
            }
            else if (clrBackedType != null)
            {
                ExceptionUtilities.CheckObjectNotNull(this.PrimitiveDataTypeConverter, "Cannot get edm type name for primitive clr type without converter");
                return this.PrimitiveDataTypeConverter.ToDataType(clrBackedType.ClrType).GetEdmTypeName();
            }
            else
            {
                throw new TaupoNotSupportedException("Unable to find EDM type name for type which is not an entity type or a primitive type");
            }
        }
    }
}
