//---------------------------------------------------------------------
// <copyright file="LinqQuerySyntaxCodeGenerationVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Linq
{
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// The visitor that creates CodeExpression that represents Linq query built using query-based syntax.
    /// </summary>    
    public abstract class LinqQuerySyntaxCodeGenerationVisitor : LinqCommonSyntaxCodeGenerationVisitor
    {
        /// <summary>
        /// Initializes a new instance of the LinqQuerySyntaxCodeGenerationVisitor class.
        /// </summary>
        /// <param name="rootExpression">CodeExpression that is a root of generated query.</param>
        /// <param name="parameterNameGenerator">IIdentifierGenerator that is to generate multiple variable names in query syntax</param>
        /// <param name="customInitializationCodeGenerator">Code generator for custom initialization.</param>
        protected LinqQuerySyntaxCodeGenerationVisitor(CodeExpression rootExpression, IIdentifierGenerator parameterNameGenerator, ICustomInitializationCodeGenerator customInitializationCodeGenerator)
            : base(rootExpression, customInitializationCodeGenerator)
        {
            this.ParameterNamesDictionary = new Dictionary<LinqParameterExpression, CodeParameterDeclarationExpression>();
            this.ParameterNameGenerator = parameterNameGenerator;
        }

        /// <summary>
        /// Gets the ParameterNameGenerator
        /// </summary>
        protected IIdentifierGenerator ParameterNameGenerator { get; private set; }

        /// <summary>
        /// Gets the Parameter Names Dictionary
        /// </summary>
        protected Dictionary<LinqParameterExpression, CodeParameterDeclarationExpression> ParameterNamesDictionary { get; private set; }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public override CodeExpression Visit(LinqGroupByExpression expression)
        {
            var source = this.GenerateCode(expression.Source);
            var codeQuerySource = source as CodeQueryExpression;

            string inputParameterName = this.GetInputParameterName(codeQuerySource);
            string groupParameterName = this.GetGroupParameterName(codeQuerySource);

            var prm = expression.KeySelector.Parameters.Single();
            var newPrm = new CodeParameterDeclarationExpression(new CodeImplicitTypeReference(), inputParameterName);
            this.ParameterNamesDictionary.Add(prm, newPrm);

            CodeLambdaExpression lambda;
            try
            {
                lambda = (CodeLambdaExpression)this.GenerateCode(expression.KeySelector);
            }
            finally
            {
                this.ParameterNamesDictionary.Remove(prm);
            }

            if (codeQuerySource != null)
            {
                if (codeQuerySource.GroupByKeySelector == null && codeQuerySource.Select == null)
                {
                    return new CodeQueryExpression(
                        inputParameterName,
                        groupParameterName,
                        codeQuerySource.From,
                        codeQuerySource.Where,
                        codeQuerySource.OrderByKeySelectors,
                        codeQuerySource.AreDescending,
                        lambda.Body,
                        codeQuerySource.Select);
                }
                else
                {
                    return new CodeQueryExpression(
                        inputParameterName,
                        groupParameterName,
                        codeQuerySource,
                        null,
                        Enumerable.Empty<CodeExpression>(),
                        Enumerable.Empty<bool>(),
                        lambda.Body,
                        null);
                }
            }
            else
            {
                return new CodeQueryExpression(
                    inputParameterName,
                    groupParameterName,
                    source,
                    null,
                    Enumerable.Empty<CodeExpression>(),
                    Enumerable.Empty<bool>(),
                    lambda.Body,
                    null);
            }
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public override CodeExpression Visit(LinqOrderByExpression expression)
        {
            var source = this.GenerateCode(expression.Source);
            var codeQuerySource = source as CodeQueryExpression;

            string inputParameterName = this.GetInputParameterName(codeQuerySource);
            string groupParameterName = this.GetGroupParameterName(codeQuerySource);

            List<CodeExpression> orderByKeySelectors = new List<CodeExpression>();
            List<bool> areDescending = new List<bool>();
            for (int i = 0; i < expression.KeySelectors.Count; i++)
            {
                var prm = expression.KeySelectors[i].Parameters.Single();
                var newPrm = new CodeParameterDeclarationExpression(new CodeImplicitTypeReference(), inputParameterName);

                this.ParameterNamesDictionary.Add(prm, newPrm);

                CodeExpression keyselector;

                try
                {
                    keyselector = this.GenerateCode(expression.KeySelectors[i].Body);
                }
                finally
                {
                    this.ParameterNamesDictionary.Remove(prm);
                }

                orderByKeySelectors.Add(keyselector);

                if (expression.AreDescending[i])
                {
                    areDescending.Add(true);
                }
                else
                {
                    areDescending.Add(false);
                }
            }

            if (codeQuerySource != null)
            {
                if (codeQuerySource.GroupByKeySelector == null && codeQuerySource.Select == null)
                {
                    return new CodeQueryExpression(
                        codeQuerySource.InputParameterName,
                        codeQuerySource.GroupParameterName,
                        codeQuerySource.From,
                        codeQuerySource.Where,
                        orderByKeySelectors,
                        areDescending,
                        codeQuerySource.GroupByKeySelector,
                        codeQuerySource.Select);
                }
                else
                {
                    return new CodeQueryExpression(
                        inputParameterName,
                        groupParameterName,
                        codeQuerySource,
                        null,
                        orderByKeySelectors,
                        areDescending,
                        null,
                        null);
                }
            }
            else
            {
                return new CodeQueryExpression(
                    inputParameterName,
                    groupParameterName,
                    source,
                    null,
                    orderByKeySelectors,
                    areDescending,
                    null,
                    null);
            }
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public override CodeExpression Visit(LinqParameterExpression expression)
        {
            CodeParameterDeclarationExpression prm;
            if (!this.ParameterNamesDictionary.TryGetValue(expression, out prm))
            {
                // for query syntax specific methods we use ParameterNamesDictionary, for non-specific methods (like Count, All, Any) we fall back to 
                // using parametr name (just like in method syntax)
                return Code.Argument(expression.Name);
            }
            else
            {
                return Code.Argument(prm.Name);
            }
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public override CodeExpression Visit(LinqSelectExpression expression)
        {
            var source = this.GenerateCode(expression.Source);
            var prm = expression.Lambda.Parameters.Single();
            var codeQuerySource = source as CodeQueryExpression;

            string inputParameterName = this.GetInputParameterName(codeQuerySource);
            string groupParameterName = this.GetGroupParameterName(codeQuerySource);

            var newPrm = new CodeParameterDeclarationExpression();
            newPrm = new CodeParameterDeclarationExpression(new CodeImplicitTypeReference(), inputParameterName);
            this.ParameterNamesDictionary.Add(prm, newPrm);

            CodeLambdaExpression lambda;
            try
            {
                lambda = (CodeLambdaExpression)this.GenerateCode(expression.Lambda);
            }
            finally
            {
                this.ParameterNamesDictionary.Remove(prm);
            }

            if (codeQuerySource != null)
            {
                if (codeQuerySource.GroupByKeySelector == null && codeQuerySource.Select == null)
                {
                    return new CodeQueryExpression(
                        inputParameterName,
                        groupParameterName,
                        codeQuerySource.From,
                        codeQuerySource.Where,
                        codeQuerySource.OrderByKeySelectors,
                        codeQuerySource.AreDescending,
                        codeQuerySource.GroupByKeySelector,
                        lambda.Body);
                }
                else
                {
                    return new CodeQueryExpression(
                        inputParameterName,
                        groupParameterName,
                        codeQuerySource,
                        null,
                        Enumerable.Empty<CodeExpression>(),
                        Enumerable.Empty<bool>(),
                        null,
                        lambda.Body);
                }
            }
            else
            {
                return new CodeQueryExpression(
                    inputParameterName,
                    groupParameterName,
                    source,
                    null,
                    Enumerable.Empty<CodeExpression>(),
                    Enumerable.Empty<bool>(),
                    null,
                    lambda.Body);
            }
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public override CodeExpression Visit(LinqSelectManyExpression expression)
        {
            throw new TaupoNotSupportedException("Query syntax is not supported by Taupo at the moment.");
        }

        /// <summary>
        /// Generates System.CodeDom.CodeExpression from the given expression.
        /// </summary>
        /// <param name="expression">Expression from which System.CodeDom.CodeExpression is generated.</param>
        /// <returns>Generated System.CodeDom.CodeExpression.</returns>
        public override CodeExpression Visit(LinqWhereExpression expression)
        {
            var source = this.GenerateCode(expression.Source);
            var prm = expression.Lambda.Parameters.Single();
            var codeQuerySource = source as CodeQueryExpression;

            string inputParameterName = this.GetInputParameterName(codeQuerySource);
            string groupParameterName = this.GetGroupParameterName(codeQuerySource);

            var newPrm = new CodeParameterDeclarationExpression();
            newPrm = new CodeParameterDeclarationExpression(new CodeImplicitTypeReference(), inputParameterName);
            this.ParameterNamesDictionary.Add(prm, newPrm);

            CodeLambdaExpression lambda;
            try
            {
                lambda = (CodeLambdaExpression)this.GenerateCode(expression.Lambda);
            }
            finally
            {
                this.ParameterNamesDictionary.Remove(prm);
            }

            if (codeQuerySource != null)
            {
                if (codeQuerySource.GroupByKeySelector == null && codeQuerySource.Select == null)
                {
                    if (codeQuerySource.Where == null)
                    {
                        return new CodeQueryExpression(
                            inputParameterName,
                            groupParameterName,
                            codeQuerySource.From,
                            lambda.Body,
                            codeQuerySource.OrderByKeySelectors,
                            codeQuerySource.AreDescending,
                            codeQuerySource.GroupByKeySelector,
                            codeQuerySource.Select);
                    }
                    else
                    {
                        return new CodeQueryExpression(
                            inputParameterName,
                            groupParameterName,
                            codeQuerySource.From,
                            codeQuerySource.Where.BooleanAnd(lambda.Body),
                            codeQuerySource.OrderByKeySelectors,
                            codeQuerySource.AreDescending,
                            codeQuerySource.GroupByKeySelector,
                            codeQuerySource.Select);
                    }
                }
                else
                {
                    return new CodeQueryExpression(
                        inputParameterName,
                        groupParameterName,
                        codeQuerySource,
                        lambda.Body,
                        Enumerable.Empty<CodeExpression>(),
                        Enumerable.Empty<bool>(),
                        null,
                        null);
                }
            }
            else
            {
                return new CodeQueryExpression(
                    inputParameterName,
                    groupParameterName,
                    source,
                    lambda.Body,
                    Enumerable.Empty<CodeExpression>(),
                    Enumerable.Empty<bool>(),
                    null,
                    null);
            }
        }

        /// <summary>
        /// Get the input Parameter name
        /// </summary>
        /// <param name="codeQuerySource">Input code query source</param>
        /// <returns>input paramter Name</returns>
        protected string GetInputParameterName(CodeQueryExpression codeQuerySource)
        {
            bool shouldCreateNewInputParameter = codeQuerySource == null || codeQuerySource.GroupByKeySelector != null || codeQuerySource.Select != null;
            if (shouldCreateNewInputParameter)
            {
                return this.ParameterNameGenerator.GenerateIdentifier("c");
            }
            else
            {
                return codeQuerySource.InputParameterName;
            }
        }

        /// <summary>
        /// Get the group Parameter Name
        /// </summary>
        /// <param name="codeQuerySource">Input code query Source</param>
        /// <returns>Group Parameter Name</returns>
        protected string GetGroupParameterName(CodeQueryExpression codeQuerySource)
        {
            bool shouldCreateNewGroupParameter = codeQuerySource == null || codeQuerySource.GroupByKeySelector != null;
            if (shouldCreateNewGroupParameter)
            {
                return this.ParameterNameGenerator.GenerateIdentifier("g");
            }
            else
            {
                return codeQuerySource.GroupParameterName;
            }
        }
    }
}
