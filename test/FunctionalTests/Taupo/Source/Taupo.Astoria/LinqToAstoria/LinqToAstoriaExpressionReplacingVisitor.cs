//---------------------------------------------------------------------
// <copyright file="LinqToAstoriaExpressionReplacingVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.LinqToAstoria
{
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Linq;

    /// <summary>
    /// Replaces the expression with a new copy, if any of the expression's children has changed. 
    /// If no changes were made, the same expression is returned.
    /// </summary>
    public class LinqToAstoriaExpressionReplacingVisitor : LinqExpressionReplacingVisitor, ILinqToAstoriaExpressionVisitor<QueryExpression>
    {
        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqAddQueryOptionExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqToAstoriaAddQueryOptionExpression expression)
        {
            QueryExpression source = this.ReplaceExpression(expression.Source);

            if (HasChanged(expression.Source, source))
            {
                return LinqToAstoriaLinqBuilder.AddQueryOption(source, expression.QueryOption, expression.QueryValue);
            }
            else
            {
                return expression;
            }
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqConditionalExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqToAstoriaConditionalExpression expression)
        {
            QueryExpression condition = this.ReplaceExpression(expression.Condition);
            QueryExpression ifTrue = this.ReplaceExpression(expression.IfTrue);
            QueryExpression ifFalse = this.ReplaceExpression(expression.IfFalse);

            if (HasChanged(expression.Condition, condition) || HasChanged(expression.IfTrue, ifTrue) || HasChanged(expression.IfFalse, ifFalse))
            {
                return new LinqToAstoriaConditionalExpression(condition, ifTrue, ifFalse, expression.ExpressionType);
            }
            else
            {
                return expression;
            }
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqExpandExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqToAstoriaExpandExpression expression)
        {
            QueryExpression source = this.ReplaceExpression(expression.Source);

            if (HasChanged(expression.Source, source))
            {
                return new LinqToAstoriaExpandExpression(source, expression.ExpandString, expression.ExpressionType, expression.IsImplicit);
            }
            else
            {
                return expression;
            }
        }

        /// <summary>
        /// Visits a LinqToAstoriaExpandLambdaExpression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The result of visiting this expression.</returns>
        public virtual QueryExpression Visit(LinqToAstoriaExpandLambdaExpression expression)
        {
            QueryExpression source = this.ReplaceExpression(expression.Source);

            if (HasChanged(expression.Source, source))
            {
                return new LinqToAstoriaExpandLambdaExpression(source, expression.Lambda, expression.ExpressionType);
            }
            else
            {
                return expression;
            }
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqKeyExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(LinqToAstoriaKeyExpression expression)
        {
            QueryExpression source = this.ReplaceExpression(expression.Source);

            if (HasChanged(expression.Source, source))
            {
                return LinqToAstoriaLinqBuilder.Key(source, expression.KeyProperties);
            }
            else
            {
                return expression;
            }
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqToAstoriaLinksExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Parameter expression with a resolved name.</returns>
        public virtual QueryExpression Visit(LinqToAstoriaLinksExpression expression)
        {
            return expression;
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqToAstoriaValueExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Parameter expression with a resolved name.</returns>
        public virtual QueryExpression Visit(LinqToAstoriaValueExpression expression)
        {
            return expression;
        }
    }
}
