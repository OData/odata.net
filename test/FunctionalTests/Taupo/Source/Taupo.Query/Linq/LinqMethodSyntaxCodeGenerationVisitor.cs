//---------------------------------------------------------------------
// <copyright file="LinqMethodSyntaxCodeGenerationVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Linq
{
    using System.CodeDom;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// The visitor that creates CodeExpression that represents Linq query built using lambda-based syntax.
    /// </summary>    
    public abstract class LinqMethodSyntaxCodeGenerationVisitor : LinqCommonSyntaxCodeGenerationVisitor
    {
        /// <summary>
        /// Initializes a new instance of the LinqMethodSyntaxCodeGenerationVisitor class.
        /// </summary>
        /// <param name="rootExpression">CodeExpression that is a root of generated query.</param>
        /// <param name="customInitializationCodeGenerator">Code generator for custom initialization.</param>
        protected LinqMethodSyntaxCodeGenerationVisitor(CodeExpression rootExpression, ICustomInitializationCodeGenerator customInitializationCodeGenerator)
            : base(rootExpression, customInitializationCodeGenerator)
        {
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public override CodeExpression Visit(LinqGroupByExpression expression)
        {
            CodeExpression source = this.GenerateCode(expression.Source);
            var keySelector = (CodeLambdaExpression)this.GenerateCode(expression.KeySelector);

            var arguments = new List<CodeLambdaExpression>(new[] { keySelector });

            if (expression.ElementSelector != null)
            {
                var elementSelector = (CodeLambdaExpression)this.GenerateCode(expression.ElementSelector);
                arguments.Add(elementSelector);
            }

            if (expression.ResultSelector != null)
            {
                var resultSelector = (CodeLambdaExpression)this.GenerateCode(expression.ResultSelector);
                arguments.Add(resultSelector);
            }

            return source.Call("GroupBy", arguments.ToArray());
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public override CodeExpression Visit(LinqOrderByExpression expression)
        {
            CodeExpression source = this.GenerateCode(expression.Source);

            CodeExpression result = source;
            for (int i = 0; i < expression.KeySelectors.Count; i++)
            {
                var keySelector = this.GenerateCode(expression.KeySelectors[i]);
                var isDescending = expression.AreDescending[i];

                string methodName = i == 0 ? "OrderBy" : "ThenBy";

                if (isDescending)
                {
                    methodName += "Descending";
                }

                result = Code.Call(result, methodName, keySelector);
            }

            return result;
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public override CodeExpression Visit(LinqParameterExpression expression)
        {
            return Code.Argument(expression.Name);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public override CodeExpression Visit(LinqSelectExpression expression)
        {
            CodeExpression source = this.GenerateCode(expression.Source);
            var lambda = this.GenerateCode(expression.Lambda) as CodeLambdaExpression;

            return source.Call("Select", lambda);
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public override CodeExpression Visit(LinqSelectManyExpression expression)
        {
            var source = this.GenerateCode(expression.Source);
            var collectionSelector = this.GenerateCode(expression.CollectionSelector);
            var resultSelector = expression.ResultSelector != null ? this.GenerateCode(expression.ResultSelector) : null;

            if (resultSelector != null)
            {
                return source.Call("SelectMany", collectionSelector, resultSelector);
            }
            else
            {
                return source.Call("SelectMany", collectionSelector);
            }
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public override CodeExpression Visit(LinqWhereExpression expression)
        {
            CodeExpression source = this.GenerateCode(expression.Source);
            var lambda = this.GenerateCode(expression.Lambda) as CodeLambdaExpression;

            return source.Call("Where", lambda);
        }
    }
}