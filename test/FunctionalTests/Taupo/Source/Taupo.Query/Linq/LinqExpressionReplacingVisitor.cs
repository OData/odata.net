//---------------------------------------------------------------------
// <copyright file="LinqExpressionReplacingVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// Replaces the expression with a new copy, if any of the expression's childeren has changed. 
    /// If no changes were made, the same expression is returned.
    /// </summary>
    public abstract class LinqExpressionReplacingVisitor : CommonExpressionReplacingVisitor, ILinqExpressionVisitor<QueryExpression>
    {
        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqAllExpression expression)
        {
            return this.VisitQueryMethodWithLambdaExpression(expression, (s, l, t) => new LinqAllExpression(s, l, t));
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqAnyExpression expression)
        {
            return this.VisitQueryMethodWithLambdaExpression(expression, (s, l, t) => new LinqAnyExpression(s, l, t));
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqAsEnumerableExpression expression)
        {
            var source = this.ReplaceExpression(expression.Source);

            if (this.HasChanged(expression.Source, source))
            {
                return new LinqAsEnumerableExpression(source, expression.ExpressionType);
            }
            else
            {
                return expression;
            }
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqBitwiseAndExpression expression)
        {
            return this.VisitBinaryExpression(expression, (l, r, t) => new LinqBitwiseAndExpression(l, r, t));
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqBitwiseOrExpression expression)
        {
            return this.VisitBinaryExpression(expression, (l, r, t) => new LinqBitwiseOrExpression(l, r, t));
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqBuiltInFunctionCallExpression expression)
        {
            var replacedArguments = this.ReplaceArguments(expression.Arguments);

            if (this.HasChanged(expression.Arguments.ToList(), replacedArguments))
            {
                return new LinqBuiltInFunctionCallExpression(expression.ExpressionType, expression.LinqBuiltInFunction, replacedArguments.ToArray());
            }
            else
            {
                return expression;
            }
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqCastExpression expression)
        {
            var argument = this.ReplaceExpression(expression.Source);

            if (this.HasChanged(expression.Source, argument))
            {
                return argument.CastEnumerable(expression.TypeToOperateAgainst);
            }
            
            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqConcatExpression expression)
        {
            QueryExpression outer = this.ReplaceExpression(expression.Outer);
            QueryExpression inner = this.ReplaceExpression(expression.Inner);

            if (this.HasChanged(expression.Outer, outer) || this.HasChanged(expression.Inner, inner))
            {
                return new LinqConcatExpression(outer, inner, expression.ExpressionType);
            }

            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqContainsExpression expression)
        {
            var source = this.ReplaceExpression(expression.Source);
            var value = this.ReplaceExpression(expression.Value);

            if (this.HasChanged(expression.Source, source) || this.HasChanged(expression.Value, value))
            {
                return new LinqContainsExpression(source, value, expression.ExpressionType);
            }
            else
            {
                return expression;
            }
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqCountExpression expression)
        {
            return this.VisitQueryMethodWithLambdaExpression(expression, (s, l, t) => new LinqCountExpression(s, l, t));
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqDefaultIfEmptyExpression expression)
        {
            QueryExpression source = this.ReplaceExpression(expression.Source);

            if (expression.DefaultValue != null)
            {
                QueryExpression defaultValue = this.ReplaceExpression(expression.DefaultValue);
                if (this.HasChanged(expression.Source, source) || this.HasChanged(expression.DefaultValue, defaultValue))
                {
                    return new LinqDefaultIfEmptyExpression(source, expression.ExpressionType, defaultValue);
                }
            }

            if (this.HasChanged(expression.Source, source))
            {
                return new LinqDefaultIfEmptyExpression(source, expression.ExpressionType);
            }

            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqDistinctExpression expression)
        {
            QueryExpression source = this.ReplaceExpression(expression.Source);

            if (this.HasChanged(expression.Source, source))
            {
                return new LinqDistinctExpression(source, expression.ExpressionType);
            }

            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public QueryExpression Visit(LinqExceptExpression expression)
        {
            QueryExpression outer = this.ReplaceExpression(expression.Outer);
            QueryExpression inner = this.ReplaceExpression(expression.Inner);

            if (this.HasChanged(expression.Outer, outer) || this.HasChanged(expression.Inner, inner))
            {
                return new LinqExceptExpression(outer, inner, expression.ExpressionType);
            }

            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqExclusiveOrExpression expression)
        {
            return this.VisitBinaryExpression(expression, (l, r, t) => new LinqExclusiveOrExpression(l, r, t));
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqFirstExpression expression)
        {
            return this.VisitQueryMethodWithLambdaExpression(expression, (s, l, t) => new LinqFirstExpression(s, l, t));
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqFirstOrDefaultExpression expression)
        {
            return this.VisitQueryMethodWithLambdaExpression(expression, (s, l, t) => new LinqFirstOrDefaultExpression(s, l, t));
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqFreeVariableExpression expression)
        {
            QueryExpression[] values = expression.Values.Select(this.ReplaceExpression).ToArray();

            if (HasChanged(expression.Values, values))
            {
                return new LinqFreeVariableExpression(expression.Name, expression.ExpressionType, values);
            }
            
            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqGroupByExpression expression)
        {
            QueryExpression source = this.ReplaceExpression(expression.Source);

            var keySelector = (LinqLambdaExpression)this.ReplaceExpression(expression.KeySelector);
            var elementSelector = expression.ElementSelector != null ? (LinqLambdaExpression)this.ReplaceExpression(expression.ElementSelector) : null;
            var resultSelector = expression.ResultSelector != null ? (LinqLambdaExpression)this.ReplaceExpression(expression.ResultSelector) : null;

            if (HasChanged(expression.Source, source) || HasChanged(expression.KeySelector, keySelector) || HasChanged(expression.ElementSelector, elementSelector) || HasChanged(expression.ResultSelector, resultSelector))
            {
                return new LinqGroupByExpression(source, keySelector, elementSelector, resultSelector, expression.ExpressionType);
            }
            
            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqGroupJoinExpression expression)
        {
            return this.VisitJoinExpressionBase(expression, LinqBuilder.GroupJoin);
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqJoinExpression expression)
        {
            return this.VisitJoinExpressionBase(expression, LinqBuilder.Join);
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqLambdaExpression expression)
        {
            var parameters = expression.Parameters.Select(this.ReplaceExpression).Cast<LinqParameterExpression>().ToArray();
            QueryExpression body = this.ReplaceExpression(expression.Body);

            if (HasChanged(expression.Body, body) || HasChanged(expression.Parameters, parameters))
            {
                return LinqBuilder.Lambda(body, parameters, expression.ExpressionType);
            }
            
            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public QueryExpression Visit(LinqLengthPropertyExpression expression)
        {
            var instance = this.ReplaceExpression(expression.Instance);

            if (this.HasChanged(expression.Instance, instance))
            {
                return new LinqLengthPropertyExpression(instance, expression.ExpressionType);
            }
            else
            {
                return expression;
            }
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqLongCountExpression expression)
        {
            return this.VisitQueryMethodWithLambdaExpression(expression, (s, l, t) => new LinqLongCountExpression(s, l, t));
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqMaxExpression expression)
        {
            return this.VisitQueryMethodWithLambdaExpression(expression, (s, l, t) => new LinqMaxExpression(s, l, t));
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqMemberMethodExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqMemberMethodExpression expression)
        {
            var source = this.ReplaceExpression(expression.Source);
            var replacedArguments = this.ReplaceArguments(expression.Arguments);

            if (this.HasChanged(expression.Source, source) || this.HasChanged(expression.Arguments.ToList(), replacedArguments))
            {
                return new LinqMemberMethodExpression(source, expression.MemberMethod, expression.ExpressionType, replacedArguments.ToArray());
            }
            else
            {
                return expression;
            }
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqMinExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqMinExpression expression)
        {
            return this.VisitQueryMethodWithLambdaExpression(expression, (s, l, t) => new LinqMinExpression(s, l, t));
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqNewArrayExpression expression)
        {
            List<QueryExpression> childExpressions = new List<QueryExpression>();
            foreach (var childExpression in expression.Expressions)
            {
                childExpressions.Add(childExpression.Accept(this));
            }

            if (HasChanged(expression.Expressions.ToList(), childExpressions))
            {
                return new LinqNewArrayExpression(expression.ExpressionType, childExpressions);
            }

            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqNewExpression expression)
        {
            QueryExpression[] members = expression.Members.Select(this.ReplaceExpression).ToArray();
            if (HasChanged(expression.Members, members))
            {
                return LinqBuilder.New(expression.MemberNames, members, expression.ExpressionType);
            }
            
            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqNewInstanceExpression expression)
        {
            QueryExpression[] members = expression.Members.Select(this.ReplaceExpression).ToArray();
            QueryExpression[] constructorArguments = expression.ConstructorArguments.Select(this.ReplaceExpression).ToArray();
            if (HasChanged(expression.Members, members) || HasChanged(expression.ConstructorArguments, constructorArguments))
            {
                return LinqBuilder.NewInstance(constructorArguments, expression.MemberNames, members, expression.ExpressionType);
            }

            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqOrderByExpression expression)
        {
            QueryExpression source = this.ReplaceExpression(expression.Source);

            var keySelectors = expression.KeySelectors.Select(this.ReplaceExpression).Cast<LinqLambdaExpression>().ToArray();

            if (HasChanged(expression.Source, source) || HasChanged(expression.KeySelectors, keySelectors))
            {
                // using internal overload, because there may be multiple keySelectors, public overloads only allow to set one at at time
                return LinqBuilder.OrderBy(source, keySelectors, expression.AreDescending, expression.ExpressionType);
            }
            
            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqParameterExpression expression)
        {
            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqSelectExpression expression)
        {
            return this.VisitQueryMethodWithLambdaExpression(expression, (s, l, t) => new LinqSelectExpression(s, l, t));
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqSelectManyExpression expression)
        {
            var source = this.ReplaceExpression(expression.Source);
            var collectionSelector = (LinqLambdaExpression)this.ReplaceExpression(expression.CollectionSelector);
            var resultSelector = expression.ResultSelector != null ? (LinqLambdaExpression)this.ReplaceExpression(expression.ResultSelector) : null;

            if (HasChanged(expression.Source, source) || HasChanged(expression.CollectionSelector, collectionSelector) || HasChanged(expression.ResultSelector, resultSelector))
            {
                return new LinqSelectManyExpression(source, collectionSelector, resultSelector, expression.ExpressionType);
            }
            else
            {
                return expression;
            }
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqSingleExpression expression)
        {
            return this.VisitQueryMethodWithLambdaExpression(expression, (s, l, t) => new LinqSingleExpression(s, l, t));
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqSingleOrDefaultExpression expression)
        {
            return this.VisitQueryMethodWithLambdaExpression(expression, (s, l, t) => new LinqSingleOrDefaultExpression(s, l, t));
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqSkipExpression expression)
        {
            QueryExpression source = this.ReplaceExpression(expression.Source);
            QueryExpression skipCount = this.ReplaceExpression(expression.SkipCount);

            if (HasChanged(expression.Source, source) || HasChanged(expression.SkipCount, skipCount))
            {
                return new LinqSkipExpression(source, skipCount, expression.ExpressionType);
            }
            
            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqTakeExpression expression)
        {
            QueryExpression source = this.ReplaceExpression(expression.Source);
            QueryExpression takeCount = this.ReplaceExpression(expression.TakeCount);

            if (HasChanged(expression.Source, source) || HasChanged(expression.TakeCount, takeCount))
            {
                return new LinqTakeExpression(source, takeCount, expression.ExpressionType);
            }
            
            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqUnionExpression expression)
        {
            var firstSource = this.ReplaceExpression(expression.FirstSource);
            var secondSource = this.ReplaceExpression(expression.SecondSource);

            if (HasChanged(expression.FirstSource, firstSource) ||
               HasChanged(expression.SecondSource, secondSource))
            {
                return new LinqUnionExpression(firstSource, secondSource, expression.ExpressionType);
            }

            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqWhereExpression expression)
        {
            return this.VisitQueryMethodWithLambdaExpression(expression, (s, l, t) => new LinqWhereExpression(s, l, t));
        }

        private QueryExpression VisitQueryMethodWithLambdaExpression(
            LinqQueryMethodWithLambdaExpression expression,
            Func<QueryExpression, LinqLambdaExpression, QueryType, QueryExpression> linqBuilderCall)
        {
            QueryExpression source = this.ReplaceExpression(expression.Source);

            if (expression.Lambda != null)
            {
                var lambda = (LinqLambdaExpression)this.ReplaceExpression(expression.Lambda);

                if (HasChanged(expression.Source, source) || HasChanged(expression.Lambda, lambda))
                {
                    return linqBuilderCall(source, lambda, expression.ExpressionType);
                }
                
                return expression;
            }
            
            if (this.HasChanged(expression.Source, source))
            {
                return linqBuilderCall(source, null, expression.ExpressionType);
            }
            
            return expression;
        }

        private QueryExpression VisitJoinExpressionBase(LinqJoinExpressionBase expression, Func<QueryExpression, QueryExpression, LinqLambdaExpression, LinqLambdaExpression, LinqLambdaExpression, QueryType, QueryExpression> builderFunc)
        {
            var outer = this.ReplaceExpression(expression.Outer);
            var inner = this.ReplaceExpression(expression.Inner);
            var outerKeySelector = (LinqLambdaExpression)this.ReplaceExpression(expression.OuterKeySelector);
            var innerKeySelector = (LinqLambdaExpression)this.ReplaceExpression(expression.InnerKeySelector);
            var resultSelector = (LinqLambdaExpression)this.ReplaceExpression(expression.ResultSelector);

            if (HasChanged(expression.Outer, outer) ||
                HasChanged(expression.Inner, inner) ||
                HasChanged(expression.OuterKeySelector, outerKeySelector) ||
                HasChanged(expression.InnerKeySelector, innerKeySelector) ||
                HasChanged(expression.ResultSelector, resultSelector))
            {
                return builderFunc(outer, inner, outerKeySelector, innerKeySelector, resultSelector, expression.ExpressionType);
            }

            return expression;
        }
    }
}
