//---------------------------------------------------------------------
// <copyright file="UriQueryVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.LinqToAstoria
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// Visitor class for generating the URI query string
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Coupling is temporarily allowed until further refactoring of current design.")]
    [ImplementationName(typeof(IQueryToUriStringConverter), "Default")]
    public class UriQueryVisitor : UriQueryVisitorBase, IQueryToUriStringConverter
    {
        private string filterString;
        private string orderByString;
        private string selectString;
        private string inlineCountString;
        private string expandString;
        private string topString;
        private string skipString;
        private bool hasQueryOption;
        private string optionalString;

        /// <summary>
        /// Gets the expand string.
        /// </summary>
        /// <value>The expand string.</value>
        public string ExpandString { get; private set; }

        /// <summary>
        /// Gets the select string.
        /// </summary>
        /// <value>The select string.</value>
        public string SelectString { get; private set; }

        /// <summary>
        /// Gets or sets the expression converter.
        /// </summary>
        /// <value>The expression converter.</value>
        /// <remarks></remarks>
        [InjectDependency(IsRequired = true)]
        public IQueryToODataUriExpressionConverter ExpressionConverter { get; set; }

        /// <summary>
        /// Gets or sets the formatter to use for key values
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataLiteralConverter LiteralConverter { get; set; }

        /// <summary>
        /// Computes the final Uri string.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri String.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings",
            Justification = "We want this to be a string")]
        public string ComputeUri(QueryExpression expression)
        {
            this.ExpandString = string.Empty;
            this.SelectString = string.Empty;

            string uri = this.ComputeUriInternal(expression);

            if (this.hasQueryOption)
            {
                uri += "?";
            }

            bool queryOptionAdded = false;

            if (!string.IsNullOrEmpty(this.filterString))
            {
                uri += this.filterString;
                queryOptionAdded = true;
            }

            if (!string.IsNullOrEmpty(this.inlineCountString))
            {
                if (queryOptionAdded)
                {
                    uri += "&";
                }

                uri += this.inlineCountString;
                queryOptionAdded = true;
            }

            if (!string.IsNullOrEmpty(this.selectString))
            {
                if (queryOptionAdded)
                {
                    uri += "&";
                }

                uri += this.selectString;

                // Remove $select=
                this.SelectString = this.selectString.Remove(0, 8);
                queryOptionAdded = true;
            }

            if (!string.IsNullOrEmpty(this.expandString))
            {
                if (queryOptionAdded)
                {
                    uri += "&";
                }

                uri += this.expandString;

                // Remove $expand=
                this.ExpandString = this.expandString.Remove(0, 8);
                queryOptionAdded = true;
            }

            if (!string.IsNullOrEmpty(this.topString))
            {
                if (queryOptionAdded)
                {
                    uri += "&";
                }

                uri += this.topString;
                queryOptionAdded = true;
            }

            if (!string.IsNullOrEmpty(this.skipString))
            {
                if (queryOptionAdded)
                {
                    uri += "&";
                }

                uri += this.skipString;
                queryOptionAdded = true;
            }

            if (!string.IsNullOrEmpty(this.orderByString))
            {
                if (queryOptionAdded)
                {
                    uri += "&";
                }

                uri += this.orderByString;
                queryOptionAdded = true;
            }

            if (!string.IsNullOrEmpty(this.optionalString))
            {
                if (queryOptionAdded)
                {
                    uri += "&";
                }

                uri += this.optionalString;
                queryOptionAdded = true;
            }

            this.filterString = string.Empty;
            this.expandString = string.Empty;
            this.topString = string.Empty;
            this.selectString = string.Empty;
            this.orderByString = string.Empty;
            this.inlineCountString = string.Empty;
            this.skipString = string.Empty;
            this.hasQueryOption = false;
            this.optionalString = string.Empty;

            return uri;
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqCountExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public override string Visit(LinqCountExpression expression)
        {
            string source = this.ComputeUriInternal(expression.Source);

            return string.Format(CultureInfo.InvariantCulture, "{0}/$count", source);
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqOrderByExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public override string Visit(LinqOrderByExpression expression)
        {
            string source = this.ComputeUriInternal(expression.Source);
            List<string> orderby = new List<string>();
            for (int i = 0; i < expression.KeySelectors.Count; i++)
            {
                var order = this.ExpressionConverter.Convert(expression.KeySelectors[i].Body);
                if (expression.AreDescending[i])
                {
                    order += " desc";
                }

                orderby.Add(order);
            }

            this.SetOrderByQueryOption(string.Join(",", orderby.ToArray()));
            this.hasQueryOption = true;

            return source;
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqSelectExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public override string Visit(LinqSelectExpression expression)
        {
            string source = this.ComputeUriInternal(expression.Source);

            LinqNewExpression newExpression = expression.Lambda.Body as LinqNewExpression;
            LinqNewInstanceExpression newInstanceExpression = expression.Lambda.Body as LinqNewInstanceExpression;
            QueryPropertyExpression propertyExpression = expression.Lambda.Body as QueryPropertyExpression;
            string select = string.Empty;

            if (propertyExpression != null)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}/{1}", source, propertyExpression.Name);
            }    
            else if (newExpression != null)
            {
                 select = this.ExpressionConverter.Convert(newExpression);

                this.SetSelectQueryOption(select);
                this.hasQueryOption = true;

                return string.Format(CultureInfo.InvariantCulture, "{0}{1}", source, string.Empty);
            }
            else if (newInstanceExpression != null)
            {
                select = this.ExpressionConverter.Convert(newInstanceExpression);

                this.SetSelectQueryOption(select);
                this.hasQueryOption = true;

                return string.Format(CultureInfo.InvariantCulture, "{0}{1}", source, string.Empty);
            }

            return source;
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QuerySkipExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public override string Visit(LinqSkipExpression expression)
        {
            string source = this.ComputeUriInternal(expression.Source);
            var skipCount = this.ExpressionConverter.Convert(expression.SkipCount);

            this.SetSkipQueryOption(skipCount);
            this.hasQueryOption = true;

            return source;
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryTakeExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public override string Visit(LinqTakeExpression expression)
        {
            string source = this.ComputeUriInternal(expression.Source);
            var takeCount = this.ExpressionConverter.Convert(expression.TakeCount);

            this.SetTopQueryOption(takeCount);
            this.hasQueryOption = true;

            return source;
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqAddQueryOptionExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public override string Visit(LinqToAstoriaAddQueryOptionExpression expression)
        {
            string source = this.ComputeUriInternal(expression.Source);
            var queryOption = expression.QueryOption;
            var queryValue = expression.QueryValue;

            this.SetAddQueryOption(queryOption, queryValue);
            this.hasQueryOption = true;

            return source;
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqExpandExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public override string Visit(LinqToAstoriaExpandExpression expression)
        {
            string source = this.ComputeUriInternal(expression.Source);

            this.SetExpandQueryOption(expression.ExpandString);
            this.hasQueryOption = true;

            return source;
        }

        /// <summary>
        /// Visits a LinqToAstoriaExpandLambdaExpression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The result of visiting this expression.</returns>
        public override string Visit(LinqToAstoriaExpandLambdaExpression expression)
        {
            return this.Visit(expression.ToLinqToAstoriaExpandExpression());
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqToAstoriaKeyExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public override string Visit(LinqToAstoriaKeyExpression expression)
        {
            ExceptionUtilities.CheckArgumentNotNull(expression, "expression");

            string source = this.ComputeUriInternal(expression.Source);
            StringBuilder builder = new StringBuilder();
            builder.Append(source);
            this.LiteralConverter.AppendKeyExpression(builder, expression.KeyProperties.Select(p => new NamedValue(p.Key.Name, p.Value.ScalarValue.Value)));
            return builder.ToString();
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqToAstoriaLinksExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public override string Visit(LinqToAstoriaLinksExpression expression)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}/{1}", this.ComputeUriInternal(expression.Source), Endpoints.Ref);
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqToAstoriaValueExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public override string Visit(LinqToAstoriaValueExpression expression)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}/{1}", this.ComputeUriInternal(expression.Source), Endpoints.Value);
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqWhereExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public override string Visit(LinqWhereExpression expression)
        {
            string source = this.ComputeUriInternal(expression.Source);
            var lambda = this.ExpressionConverter.Convert(expression.Lambda);
            this.SetFilterQueryOption(lambda);
            this.hasQueryOption = true;
            return source;
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryAsExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public override string Visit(QueryAsExpression expression)
        {
            var queryEntityType = expression.TypeToOperateAgainst as QueryEntityType;
            ExceptionUtilities.CheckObjectNotNull(queryEntityType, "Expected a QueryEntityType any other type is not supported");
            return string.Format(CultureInfo.InvariantCulture, "{0}/{1}", this.ComputeUriInternal(expression.Source), queryEntityType.EntityType.FullName);
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is a Function Call Expression
        /// </summary>
        /// <param name="expression">Query Custom Function Call Expression</param>
        /// <returns>A uri with a Action in it</returns>
        public override string Visit(QueryCustomFunctionCallExpression expression)
        {
            var serviceOperation = expression.Function.Annotations.OfType<ServiceOperationAnnotation>().SingleOrDefault();

            // We are ignoring the other parameters associated with the function call expression because other than a binding parameter
            // these will be added into the payload that is sent
            if (serviceOperation != null && serviceOperation.BindingKind.IsBound())
            {
                // Calculate the uri previous to the action query then append the function name after it
                ExceptionUtilities.Assert(expression.Arguments.Count > 0, "Action Function Expression where IsBinding = true must have at least one argment");
                var argumentExpression = expression.Arguments[0];
                return string.Format(CultureInfo.InvariantCulture, "{0}/{1}", this.ComputeUriInternal(argumentExpression), expression.Function.Name);
            }
            else
            {
               // return the function name because its at the root whether its a legacy serviceoperation or an action
               return expression.Function.Name;
            }
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryOfTypeExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public override string Visit(QueryOfTypeExpression expression)
        {
            var queryEntityType = expression.TypeToOperateAgainst as QueryEntityType;
            ExceptionUtilities.CheckObjectNotNull(queryEntityType, "Expected a QueryEntityType any other type is not supported");
            return string.Format(CultureInfo.InvariantCulture, "{0}/{1}", this.ComputeUriInternal(expression.Source), queryEntityType.EntityType.FullName);
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryPropertyExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public override string Visit(QueryPropertyExpression expression)
        {
            var instanceString = this.ComputeUriInternal(expression.Instance);
            if (instanceString != null)
            {
                instanceString += "/";
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}{1}", instanceString, expression.Name);
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqQueryRootExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri query string representing the expression.</returns>
        public override string Visit(QueryRootExpression expression)
        {
            return expression.Name;
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        internal string ComputeUriInternal(QueryExpression expression)
        {
            ExceptionUtilities.CheckArgumentNotNull(expression, "expression");

            return expression.Accept(this);
        }

        /// <summary>
        /// Sets additional query option.
        /// </summary>
        /// <param name="queryOption">The query option to be set.</param>
        /// <param name="queryValue">The value used to set the option.</param>
        private void SetAddQueryOption(string queryOption, object queryValue)
        {
            string valueAsString = queryValue as string;
            switch (queryOption)
            {
                case "$expand":
                    this.SetExpandQueryOption(valueAsString);
                    break;
                case "$filter":
                    this.SetFilterQueryOption(valueAsString);
                    break;
                case "$inlinecount":
                    this.SetInlineCountQueryOption();
                    break;
                case "$select":
                    this.SetSelectQueryOption(valueAsString);
                    break;
                case "$skip":
                    this.SetSkipQueryOption((int)queryValue);
                    break;
                case "$top":
                    this.SetTopQueryOption((int)queryValue);
                    break;
                default:
                    this.SetOptionalQueryOption(queryOption, queryValue);
                    break;
            }
        }

        /// <summary>
        /// Sets the $expand query option.
        /// </summary>
        /// <param name="value">The value used to set the option.</param>
        private void SetExpandQueryOption(string value)
        {
            if (string.IsNullOrEmpty(this.expandString))
            {
                this.expandString = string.Format(CultureInfo.InvariantCulture, "$expand={0}", value);
            }
            else
            {
                this.expandString += string.Format(CultureInfo.InvariantCulture, ",{0}", value);
            }
        }

        /// <summary>
        /// Sets the $filter query option.
        /// </summary>
        /// <param name="value">The value used to set the option.</param>
        private void SetFilterQueryOption(string value)
        {
            if (string.IsNullOrEmpty(this.filterString))
            {
                this.filterString = string.Format(CultureInfo.InvariantCulture, "$filter={0}", value);
            }
            else
            {
                this.filterString += string.Format(CultureInfo.InvariantCulture, " and {0}", value);
            }
        }

        /// <summary>
        /// Sets the $inlinecount query option.
        /// </summary>
        private void SetInlineCountQueryOption()
        {
            if (string.IsNullOrEmpty(this.inlineCountString))
            {
                this.inlineCountString = "$inlinecount=allpages";
            }
        }

        /// <summary>
        /// Sets optional query option.
        /// </summary>
        /// <param name="name">The name used for the option.</param>
        /// <param name="value">The value used to set the option.</param>
        private void SetOptionalQueryOption(string name, object value)
        {
            if (string.IsNullOrEmpty(this.optionalString))
            {
                this.optionalString = string.Format(CultureInfo.InvariantCulture, "{0}={1}", name, value);
            }
            else
            {
                this.optionalString += string.Format(CultureInfo.InvariantCulture, "&{0}={1}", name, value);
            }
        }

        /// <summary>
        /// Sets the $orderby query option.
        /// </summary>
        /// <param name="value">The value used to set the option.</param>
        private void SetOrderByQueryOption(object value)
        {
            if (string.IsNullOrEmpty(this.orderByString))
            {
                this.orderByString = string.Format(CultureInfo.InvariantCulture, "$orderby={0}", value);
            }
            else
            {
                this.orderByString += string.Format(CultureInfo.InvariantCulture, ",{0}", value);
            }
        }

        /// <summary>
        /// Sets the $select query option.
        /// </summary>
        /// <param name="value">The value used to set the option.</param>
        private void SetSelectQueryOption(string value)
        {
            if (string.IsNullOrEmpty(this.selectString))
            {
                this.selectString = string.Format(CultureInfo.InvariantCulture, "$select={0}", value);
            }
            else
            {
                this.selectString += string.Format(CultureInfo.InvariantCulture, ",{0}", value);
            }
        }

        /// <summary>
        /// Sets the $skip query option.
        /// </summary>
        /// <param name="value">The value used to set the option.</param>
        private void SetSkipQueryOption(object value)
        {
            if (string.IsNullOrEmpty(this.skipString))
            {
                this.skipString = string.Format(CultureInfo.InvariantCulture, "$skip={0}", value);
            }
        }

        /// <summary>
        /// Sets the $top query option.
        /// </summary>
        /// <param name="value">The value used to set the option.</param>
        private void SetTopQueryOption(object value)
        {
            if (string.IsNullOrEmpty(this.topString))
            {
                this.topString = string.Format(CultureInfo.InvariantCulture, "$top={0}", value);
            }
        }
    }
}
