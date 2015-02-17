//---------------------------------------------------------------------
// <copyright file="QueryToODataUriExpressionConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.LinqToAstoria
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// Visitor class for generating the URI query expression string
    /// </summary>
    [ImplementationName(typeof(IQueryToODataUriExpressionConverter), "Default")]
    public class QueryToODataUriExpressionConverter : IQueryToODataUriExpressionConverter
    {
        /// <summary>
        /// Gets or sets the literal converter to use when building key expressions
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataLiteralConverter LiteralConverter { get; set; }

        /// <summary>
        /// Gets or sets clr to data-type converter to use for primitive type checks in the uri
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IClrToPrimitiveDataTypeConverter PrimitiveDataTypeConverter { get; set; }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>Value of the expression.</returns>
        public string Convert(QueryExpression expression)
        {
            ExceptionUtilities.CheckArgumentNotNull(expression, "expression");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            var visitor = new UriExpressionVisitor(this.LiteralConverter)
            {
                PrimitiveDataTypeConverter = this.PrimitiveDataTypeConverter,
            };

            return visitor.Convert(expression);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling",
            Justification = "Visitor pattern forces coupling")]
        private class UriExpressionVisitor : UriQueryVisitorBase
        {
            private readonly Dictionary<LinqParameterExpression, string> currentLambdaParameterNames = new Dictionary<LinqParameterExpression, string>();
            private readonly IODataLiteralConverter literalConverter;
            
            /// <summary>
            /// Initializes a new instance of the UriExpressionVisitor class
            /// </summary>
            /// <param name="literalConverter">The literal converter to use</param>
            public UriExpressionVisitor(IODataLiteralConverter literalConverter)
            {
                ExceptionUtilities.CheckArgumentNotNull(literalConverter, "literalConverter");
                this.literalConverter = literalConverter;
            }

            /// <summary>
            /// Evaluates the specified expression.
            /// </summary>
            /// <param name="expression">The expression to evaluate.</param>
            /// <returns>Value of the expression.</returns>
            public string Convert(QueryExpression expression)
            {
                ExceptionUtilities.CheckArgumentNotNull(expression, "expression");
                return expression.Accept(this);
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the LinqAllExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(LinqAllExpression expression)
            {
                return this.VisitAnyOrAll(expression, ODataConstants.AllUriOperator);
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the LinqAnyExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(LinqAnyExpression expression)
            {
                return this.VisitAnyOrAll(expression, ODataConstants.AnyUriOperator);
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the LinqBuiltInFunctionCallExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Not normalizing, need lowercase strings for uri")]
            public override string Visit(LinqBuiltInFunctionCallExpression expression)
            {
                ExceptionUtilities.CheckArgumentNotNull(expression, "expression");

                if (expression.Arguments.Count == 0)
                {
                    throw new TaupoNotSupportedException("Not supported");
                }

                var protocolFunctionAnnotation = expression.LinqBuiltInFunction.Annotations.OfType<ODataCanonicalFunctionNameAnnotation>().SingleOrDefault();
                ExceptionUtilities.CheckObjectNotNull(protocolFunctionAnnotation, "Cannot build a uri expression for built-in function without annotation. Function was: {0}", expression.LinqBuiltInFunction.MethodName);

                string argValues = string.Empty;
                var name = protocolFunctionAnnotation.Name;
                if (string.Equals(name, "round", StringComparison.Ordinal))
                {
                    // because of inconsistency between the CLR Math.Round and Sql Server's Round function,
                    // we will have to enforce passing in two parameters, the second of which will be ignored when
                    // we build the Uri for the Round function call.
                    argValues = string.Join(",", expression.Arguments.Take(1).Select(a => this.Convert(a)));
                }
                else
                {
                    argValues = string.Join(",", expression.Arguments.Select(a => this.Convert(a)));
                }

                return string.Format(CultureInfo.InvariantCulture, "{0}({1})", name, argValues);
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the LinqFreeVariableExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(LinqFreeVariableExpression expression)
            {
                return expression.Values.Single().Accept(this);
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the LinqLambdaExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(LinqLambdaExpression expression)
            {
                ExceptionUtilities.Assert(expression.Parameters.Count == 1, "Lambda only supported with exactly 1 parameter");
                var parameter = expression.Parameters.Single();

                int originalParameterCount = this.currentLambdaParameterNames.Count;
                try
                {
                    var parameterString = this.Convert(parameter);
                    var bodyString = this.Convert(expression.Body);
                    if (string.IsNullOrEmpty(parameterString))
                    {
                        return string.Format(CultureInfo.InvariantCulture, "{0}", bodyString);
                    }
                    else
                    {
                        return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", parameterString, bodyString);
                    }
                }
                finally
                {
                    this.currentLambdaParameterNames.Remove(parameter);
                    ExceptionUtilities.Assert(originalParameterCount == this.currentLambdaParameterNames.Count, "Current parameter count does not match");
                }
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the LinqMemberMethodExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>
            /// Uri query string representing the expression.
            /// </returns>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Is not a normalization")]
            public override string Visit(LinqMemberMethodExpression expression)
            {
                ExceptionUtilities.CheckArgumentNotNull(expression, "expression");

                var convertedArguments = expression.Arguments.Select(a => this.Convert(a)).ToList();
                convertedArguments.Insert(0, this.Convert(expression.Source));
                string argValues = string.Join(",", convertedArguments.ToArray());

                return string.Format(CultureInfo.InvariantCulture, "{0}({1})", expression.MemberMethod.FullName.ToLowerInvariant(), argValues);
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the LinqNewExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(LinqNewExpression expression)
            {
                return string.Join(", ", expression.MemberNames.ToArray());
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the LinqNewInstanceExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(LinqNewInstanceExpression expression)
            {
                return this.Convert(LinqBuilder.New(expression.MemberNames, expression.Members));
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the LinqParameterExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(LinqParameterExpression expression)
            {
                string parameterString;
                if (this.currentLambdaParameterNames.Count == 0)
                {
                    parameterString = ODataConstants.ImplicitOuterVariableName;
                    this.currentLambdaParameterNames[expression] = parameterString;
                }
                else if (!this.currentLambdaParameterNames.TryGetValue(expression, out parameterString))
                {
                    parameterString = expression.Name;                    
                    this.currentLambdaParameterNames[expression] = parameterString;
                }

                // until there are 2 known parameters within the current stack, we don't need to provide real names for them
                // in practice, this will trigger as soon as Any/All appears in the filter
                if (this.currentLambdaParameterNames.Count < 2)
                {
                    return string.Empty;
                }

                return parameterString;
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the QueryAddExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(QueryAddExpression expression)
            {
                return this.VisitBinaryExpession(expression, "({0} add {1})");
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the QueryAndExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(QueryAndExpression expression)
            {
                return this.VisitBinaryExpession(expression, "{0} and {1}");
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the QueryAsExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>
            /// Uri query string representing the expression.
            /// </returns>
            public override string Visit(QueryAsExpression expression)
            {
                var source = this.Convert(expression.Source);
                var typeName = this.GetEdmTypeName(expression.TypeToOperateAgainst);
                if (string.IsNullOrEmpty(source))
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0}", typeName);
                }
                else
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0}/{1}", source, typeName);
                }
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the QueryCastExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(QueryCastExpression expression)
            {
                string argument = this.Convert(expression.Source);
                string edmTypeLiteral = this.GetEdmTypeName(expression.TypeToOperateAgainst);
                if (string.IsNullOrEmpty(argument))
                {
                    return string.Format(CultureInfo.InvariantCulture, "cast('{0}')", edmTypeLiteral);
                }

                return string.Format(CultureInfo.InvariantCulture, "cast({0}, '{1}')", argument, edmTypeLiteral);
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the QueryConstantExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(QueryConstantExpression expression)
            {
                object value = expression.ScalarValue.Value;
                string val = this.literalConverter.SerializePrimitive(value);
                return Uri.EscapeDataString(val);
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the QueryCustomFunctionCallExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(QueryCustomFunctionCallExpression expression)
            {
                return expression.Function.Name;
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the QueryDivideExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(QueryDivideExpression expression)
            {
                return this.VisitBinaryExpession(expression, "({0} div {1})");
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the QueryEqualToExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(QueryEqualToExpression expression)
            {
                return this.VisitBinaryExpession(expression, "{0} eq {1}");
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the QueryGreaterThanExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(QueryGreaterThanExpression expression)
            {
                return this.VisitBinaryExpession(expression, "{0} gt {1}");
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the QueryGreaterThanOrEqualToExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(QueryGreaterThanOrEqualToExpression expression)
            {
                return this.VisitBinaryExpession(expression, "{0} ge {1}");
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the QueryIsNotNullExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(QueryIsNotNullExpression expression)
            {
                string argument = this.Convert(expression.Argument);

                return string.Format(CultureInfo.InvariantCulture, "{0} ne null", argument);
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the QueryIsNullExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(QueryIsNullExpression expression)
            {
                string argument = this.Convert(expression.Argument);

                return string.Format(CultureInfo.InvariantCulture, "{0} eq null", argument);
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the QueryCastExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(QueryIsOfExpression expression)
            {
                string argument = this.Convert(expression.Source);
                string edmTypeLiteral = this.GetEdmTypeName(expression.TypeToOperateAgainst);
                if (string.IsNullOrEmpty(argument))
                {
                    return string.Format(CultureInfo.InvariantCulture, "isof('{0}')", edmTypeLiteral);
                }

                return string.Format(CultureInfo.InvariantCulture, "isof({0}, '{1}')", argument, edmTypeLiteral);
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the QueryLesThanExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(QueryLessThanExpression expression)
            {
                return this.VisitBinaryExpession(expression, "{0} lt {1}");
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the QueryLesThanOrEqualToExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(QueryLessThanOrEqualToExpression expression)
            {
                return this.VisitBinaryExpession(expression, "{0} le {1}");
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the QueryModuloExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(QueryModuloExpression expression)
            {
                return this.VisitBinaryExpession(expression, "({0} mod {1})");
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the QueryMultiplyExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(QueryMultiplyExpression expression)
            {
                return this.VisitBinaryExpession(expression, "({0} mul {1})");
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the QueryNotEqualToExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(QueryNotEqualToExpression expression)
            {
                return this.VisitBinaryExpession(expression, "{0} ne {1}");
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the QueryNotExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(QueryNotExpression expression)
            {
                return string.Format(CultureInfo.InvariantCulture, "not ({0})", this.Convert(expression.Argument));
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the QueryNullExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(QueryNullExpression expression)
            {
                return this.literalConverter.SerializePrimitive(null);
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the QueryOrExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(QueryOrExpression expression)
            {
                return this.VisitBinaryExpession(expression, "{0} or {1}");
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the QueryPropertyExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(QueryPropertyExpression expression)
            {
                var instance = this.Convert(expression.Instance);
                if (string.IsNullOrEmpty(instance))
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0}", expression.Name);
                }
                else
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0}/{1}", instance, expression.Name);
                }
            }

            /// <summary>
            /// Visits a QueryExpression tree whose root node is the QuerySubstractExpression.
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <returns>Uri query string representing the expression.</returns>
            public override string Visit(QuerySubtractExpression expression)
            {
                return this.VisitBinaryExpession(expression, "({0} sub {1})");
            }

            /// <summary>
            /// Visits a QueryExpression whose root node is the QueryBinaryToExpression..
            /// </summary>
            /// <param name="expression">The root node of the expression tree being visited.</param>
            /// <param name="uriFormat">The format of uri string.</param>
            /// <returns>Replaced expression.</returns>
            private string VisitBinaryExpession(QueryBinaryExpression expression, string uriFormat)
            {
                string left = this.Convert(expression.Left);
                string right = this.Convert(expression.Right);

                return string.Format(CultureInfo.InvariantCulture, uriFormat, left, right);
            }

            private string VisitAnyOrAll(LinqQueryMethodWithLambdaExpression expression, string operatorName)
            {
                ExceptionUtilities.CheckArgumentNotNull(expression, "expression");
                string source = this.Convert(expression.Source);

                if (expression.Lambda == null)
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0}/{1}()", source, operatorName);
                }

                string lambda = this.Convert(expression.Lambda);
                return string.Format(CultureInfo.InvariantCulture, "{0}/{1}({2})", source, operatorName, lambda);
            }
        }
    }
}
