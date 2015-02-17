//---------------------------------------------------------------------
// <copyright file="ClientQuerySingleVariationCodeBuilderBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Builds code for executing DataService queries against context.Execute
    /// </summary>
    public abstract class ClientQuerySingleVariationCodeBuilderBase : IClientQuerySingleVariationCodeBuilder
    {
        /// <summary>
        /// Initializes a new instance of the ClientQuerySingleVariationCodeBuilderBase class.
        /// </summary>
        protected ClientQuerySingleVariationCodeBuilderBase()
        {
            this.IsAsync = false;
        }

        /// <summary>
        /// Gets or sets the uri expression visitor.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryToUriStringConverter UriQueryVisitor { get; set; }

        /// <summary>
        /// Gets or sets the expression code generator.
        /// </summary>
        /// <value>The expression code generator.</value>
        [InjectDependency(IsRequired = true)]
        public IIdentifierGenerator IIdentifierGenerator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is async.
        /// </summary>
        /// <value><c>true</c> if this instance is async; otherwise, <c>false</c>.</value>
        [InjectTestParameter("Asynchronous", DefaultValueDescription = "true")]
        public bool IsAsync { get; set; }

        /// <summary>
        /// Gets or sets the ContextVariable to generate the expression on
        /// </summary>
        protected CodeExpression ContextVariable { get; set; }

        /// <summary>
        /// Gets or sets the ResultComparer to call
        /// </summary>
        protected CodeExpression ResultComparerVariable { get; set; }

        /// <summary>
        /// Gets or sets the expected Client Error Baseline
        /// </summary>
        protected CodeExpression ExpectedClientErrorValue { get; set; }
        
        /// <summary>
        /// Gets or sets the select paths
        /// </summary>
        protected IEnumerable<string> SelectedPaths { get; set; }
        
        /// <summary>
        /// Gets or sets the expand paths
        /// </summary>
        protected IEnumerable<string> ExpandedPaths { get; set; }

        /// <summary>
        /// Gets or sets the expand paths
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Api requires a string")]
        protected string Uri { get; set; }

        /// <summary>
        /// Gets or sets the QueryExpression
        /// </summary>
        protected QueryExpression QueryExpression { get; set; }

        /// <summary>
        /// Gets or sets the method that contains the test code
        /// </summary>
        protected CodeBuilder CodeBuilder { get; set; }

        /// <summary>
        /// Builds a single variation
        /// </summary>
        /// <param name="codeBuilder">CodeBuilder that adds the code</param>
        /// <param name="expression">Query Expression to create method for</param>
        /// <param name="expectedClientErrorValue">Expected Client Error Baseline</param>
        /// <param name="contextVariable">Context Variable</param>
        /// <param name="resultComparer">Result Comparer</param>
        public void Build(CodeBuilder codeBuilder, QueryExpression expression, CodeExpression expectedClientErrorValue, CodeExpression contextVariable, CodeExpression resultComparer)
        {
            ExceptionUtilities.CheckAllRequiredDependencies(this);
            ExceptionUtilities.CheckArgumentNotNull(expression, "expression");
            ExceptionUtilities.CheckArgumentNotNull(contextVariable, "contextVariable");
            ExceptionUtilities.CheckArgumentNotNull(resultComparer, "resultComparer");

            this.CodeBuilder = codeBuilder;
            this.QueryExpression = expression;
            this.ContextVariable = contextVariable;
            this.ResultComparerVariable = resultComparer;

            this.CalculateUriSelectAndExpandSegments();

            this.ExpectedClientErrorValue = expectedClientErrorValue;

            this.BuildOverride();
        }

        /// <summary>
        /// Builds a Execute call statement for a particular query
        /// </summary>
        protected abstract void BuildOverride();

        /// <summary>
        /// Starts the variation.
        /// </summary>
        /// <returns>The continuation expression for the async execution</returns>
        protected CodeExpression StartVariation()
        {
            CodeExpression continuationExpression = null;
            string queryTestName = this.IIdentifierGenerator.GenerateIdentifier("Query");
            if (this.IsAsync)
            {
                continuationExpression = this.CodeBuilder.BeginAsyncVariation(queryTestName);
            }
            else
            {
                this.CodeBuilder.BeginVariation(queryTestName);
            }

            return continuationExpression;
        }

        private void CalculateUriSelectAndExpandSegments()
        {
            this.Uri = this.UriQueryVisitor.ComputeUri(this.QueryExpression);

            // get the expand segments and select segments from the expression
            string select = this.UriQueryVisitor.SelectString;
            string expand = this.UriQueryVisitor.ExpandString;

            this.SelectedPaths = (new string[] { }).AsEnumerable();
            this.ExpandedPaths = (new string[] { }).AsEnumerable();
            if (select != null)
            {
                this.SelectedPaths = select.Split(new string[] { "," }, StringSplitOptions.None).Select(p => p.Trim());
            }

            if (expand != null)
            {
                var expandedPaths = expand.Split(new string[] { "," }, StringSplitOptions.None).Select(p => p.Trim());
                this.ExpandedPaths = this.CalculateImplicitExpandPaths(expandedPaths);
            }
        }

        private IEnumerable<string> CalculateImplicitExpandPaths(IEnumerable<string> expandedPaths)
        {
            QueryStructuralType queryStructuralType = null;
            var queryCollectionType = this.QueryExpression.ExpressionType as QueryCollectionType;
            if (queryCollectionType != null)
            {
                queryStructuralType = queryCollectionType.ElementType as QueryStructuralType;
            }

            if (queryStructuralType != null)
            {
                // include navigation properties from the selected paths in expanded paths
                var navigationNames = queryStructuralType.GetBaseTypesAndSelf().SelectMany(t => t.Properties.Where(p => p.IsNavigationProperty())).Select(n => n.Name);
                var implicitlyExpandedPaths = this.SelectedPaths.Where(p => navigationNames.Contains(p.Split('/').Last()));
                if (implicitlyExpandedPaths.Any())
                {
                    return expandedPaths.Union(implicitlyExpandedPaths).Except(new string[] { string.Empty });
                }
            }

            return expandedPaths;
        }
    }
}