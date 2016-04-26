//---------------------------------------------------------------------
// <copyright file="QueryTokenUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.UriParser
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.UriParser;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Helper methods for testing QueryToken and related classes
    /// </summary>
    internal static class QueryTokenUtils
    {
        /// <summary>
        /// The names of the system $ query options.
        /// </summary>
        /// <remarks>All the query options that are currently not supported are excluded.</remarks>
        private static readonly string[] BuiltInQueryOptions = new string[] { "$filter", "$skip", "$top", "$orderby", "$select", "$format", "$inlinecount", "$expand" /*"$skiptoken"*/ };

        /// <summary>
        /// Parses the specified query into a query descriptor.
        /// </summary>
        /// <param name="path">The query path.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="orderby">The orderby.</param>
        /// <param name="select">The select.</param>
        /// <param name="expand">The expand.</param>
        /// <param name="skip">The skip.</param>
        /// <param name="top">The top.</param>
        /// <param name="inlineCount">The inlineCount.</param>
        /// <param name="format">The format.</param>
        /// <returns>The parsed query.</returns>
        internal static SyntacticTree ParseQuery(
            string path,
            string filter = null,
            string orderby = null,
            string select = null,
            string expand = null,
            string skip = null,
            string top = null,
            string count = null,
            string format = null)
        {
            List<KeyValuePair<string, string>> queryOptions = new List<KeyValuePair<string, string>>();
            if (filter != null) queryOptions.Add(new KeyValuePair<string, string>("$filter", filter));
            if (orderby != null) queryOptions.Add(new KeyValuePair<string, string>("$orderby", orderby));
            if (select != null) queryOptions.Add(new KeyValuePair<string, string>("$select", select));
            if (expand != null) queryOptions.Add(new KeyValuePair<string, string>("$expand", expand));
            if (skip != null) queryOptions.Add(new KeyValuePair<string, string>("$skip", skip));
            if (top != null) queryOptions.Add(new KeyValuePair<string, string>("$top", top));
            if (count != null) queryOptions.Add(new KeyValuePair<string, string>("$count", count));
            if (format != null) queryOptions.Add(new KeyValuePair<string, string>("$format", format));

            return ParseQuery(path, queryOptions, false);
        }

        /// <summary>
        /// Parses the specified query into a query descriptor.
        /// </summary>
        /// <param name="path">The query path.</param>
        /// <param name="queryOptions">The query options.</param>
        /// <returns>The parsed query.</returns>
        internal static SyntacticTree ParseQuery(string path, List<KeyValuePair<string, string>> queryOptions, bool includeSpaceAroundSymbols)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(path);

            if (queryOptions != null && queryOptions.Count > 0)
            {
                bool first = true;
                foreach (var kvp in queryOptions)
                {
                    // %20 = space, %09 = tab
                    sb.Append(first ? "?" : (includeSpaceAroundSymbols ? "%20&%09" : "&"));
                    first = false;

                    if (kvp.Key == null)
                    {
                        // if we have no key we only write the value (if it is not null also)
                        sb.Append(kvp.Value ?? string.Empty);
                    }
                    else
                    {
                        sb.Append(kvp.Key);
                        sb.Append((includeSpaceAroundSymbols ? "%20=%09" : "="));
                        sb.Append(kvp.Value ?? string.Empty);
                    }
                }
            }

            return ParseQuery(sb.ToString());
        }

        /// <summary>
        /// Parses the specified query into a query descriptor.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The parsed query.</returns>
        internal static SyntacticTree ParseQuery(string query)
        {
            Uri baseUri = new Uri("http://odata.org/test/");
            if (query.StartsWith("/")) query = query.Substring(1);

            return SyntacticTree.ParseUri(new Uri(baseUri, query), baseUri);
        }

        /// <summary>
        /// Clones the <paramref name="queryOptions"/> collections and removes all the system $ query options from it.
        /// </summary>
        /// <param name="queryOptions">The query options to remove the built-in query options from.</param>
        /// <returns>All non built-in query options.</returns>
        internal static List<CustomQueryOptionToken> NormalizeAndRemoveBuiltInQueryOptions(List<KeyValuePair<string, string>> queryOptions)
        {
            if (queryOptions == null)
            {
                return null;
            }

            List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>(queryOptions.Where(qo => !BuiltInQueryOptions.Contains(qo.Key)));

            // now replace all 'null' values with string.Empty since an option like 'MyOption=' is parsed as having an empty string as value
            return collection.Select(kvp => new CustomQueryOptionToken(kvp.Key, kvp.Value ?? string.Empty)).ToList();
        }

        /// <summary>
        /// Verifies that two query tokens are equal.
        /// </summary>
        /// <param name="expected">The expected query token.</param>
        /// <param name="actual">The actual query token.</param>
        /// <param name="assert">Assertion handler to use.</param>
        internal static void VerifyStringsAreEqual(ICollection<string> expected, ICollection<string> actual, AssertionHandler assert)
        {
            var expectedEnumerator = expected.GetEnumerator();
            var actualEnumerator = actual.GetEnumerator();

            while(expectedEnumerator.MoveNext())
            {
                assert.IsTrue(actualEnumerator.MoveNext(), "Length should be the same");
                assert.AreEqual(expectedEnumerator.Current, actualEnumerator.Current, "Segment text is different");
            }
        }

        /// <summary>
        /// Verifies that two query tokens are equal.
        /// </summary>
        /// <param name="expected">The expected query token.</param>
        /// <param name="actual">The actual query token.</param>
        /// <param name="assert">Assertion handler to use.</param>
        internal static void VerifyQueryTokensAreEqual(QueryToken expected, QueryToken actual, AssertionHandler assert)
        {
            try
            {
                if(!VerifyNullnessMatches(expected, actual, assert, "token")) return;

                assert.AreEqual(expected.Kind, actual.Kind, "The token kinds are different.");

                switch (expected.Kind)
                {
                    case QueryTokenKind.Literal:
                        VerifyLiteralQueryTokensAreEqual((LiteralToken)expected, (LiteralToken)actual, assert);
                        break;
                    case QueryTokenKind.BinaryOperator:
                        VerifyBinaryOperatorQueryTokensAreEqual((BinaryOperatorToken)expected, (BinaryOperatorToken)actual, assert);
                        break;
                    case QueryTokenKind.UnaryOperator:
                        VerifyUnaryOperatorQueryTokensAreEqual((UnaryOperatorToken)expected, (UnaryOperatorToken)actual, assert);
                        break;
                    case QueryTokenKind.EndPath:
                        VerifyPropertyAccessQueryTokensAreEqual((EndPathToken)expected, (EndPathToken)actual, assert);
                        break;
                    case QueryTokenKind.InnerPath:
                        VerifyNonRootSegmentQueryTokensAreEqual((InnerPathToken) expected,(InnerPathToken) actual, assert);
                        break;
                    case QueryTokenKind.FunctionCall:
                        VerifyFunctionCallQueryTokensAreEqual((FunctionCallToken)expected, (FunctionCallToken)actual, assert);
                        break;
                    case QueryTokenKind.CustomQueryOption:
                        VerifyCustomQueryOptionQueryTokensAreEqual((CustomQueryOptionToken)expected, (CustomQueryOptionToken)actual, assert);
                        break;
                    case QueryTokenKind.OrderBy:
                        VerifyOrderByQueryTokensAreEqual((OrderByToken)expected, (OrderByToken)actual, assert);
                        break;
                    case QueryTokenKind.Select:
                        VerifySelectQueryTokensAreEqual((SelectToken)expected, (SelectToken)actual, assert);
                        break;
                    case QueryTokenKind.Star:
                        VerifyStarQueryTokensAreEqual((StarToken)expected, (StarToken)actual, assert);
                        break;
                    case QueryTokenKind.FunctionParameter:
                        VerifyFunctionParameterTokensAreEqual((FunctionParameterToken)expected, (FunctionParameterToken)actual, assert);
                        break;
                    default:
                        assert.Fail("The token kind '" + expected.Kind.ToString() + "' is not a lexical node.");
                        break;
                }
            }
            catch (Exception)
            {
                assert.Warn("Expected query token: " + expected.ToDebugString());
                assert.Warn("Actual query token: " + actual.ToDebugString());
                throw;
            }
        }

        private static void VerifyPathSegmentTokensAreEqual(PathSegmentToken expected, PathSegmentToken actual, AssertionHandler assert)
        {
            try
            {
                if (!VerifyNullnessMatches(expected, actual, assert, "token")) return;

                assert.AreEqual(expected.GetType(), actual.GetType(), "The token kinds are different.");

                assert.AreEqual(expected.Identifier, actual.Identifier, "The token identifiers are different.");

                VerifyPathSegmentTokensAreEqual(expected.NextToken, actual.NextToken, assert);
            }
            catch (Exception)
            {
                assert.Warn("Expected query token: " + expected.ToDebugString());
                assert.Warn("Actual query token: " + actual.ToDebugString());
                throw;
            }
        }

        private static bool VerifyNullnessMatches(object expected, object actual, AssertionHandler assert, string description)
        {
            if (expected == null)
            {
                assert.IsNull(actual, string.Format("The actual {0} should have been null.", description));
                return false;
            }
            assert.IsNotNull(actual, string.Format("The actual {0} should have been not-null.", description));
            return true;
        }

        /// <summary>
        /// Verifies that two lists of query tokens are equal.
        /// </summary>
        /// <param name="expected">The expected query token enumeration.</param>
        /// <param name="actual">The actual query token enumeration.</param>
        /// <param name="assert">Assertion handler to use.</param>
        internal static void VerifyQueryTokensAreEqual(IEnumerable<QueryToken> expectedTokens, IEnumerable<QueryToken> actualTokens, AssertionHandler assert)
        {
            VerificationUtils.VerifyEnumerationsAreEqual(
                expectedTokens,
                actualTokens,
                VerifyQueryTokensAreEqual,
                ToDebugString,
                assert);
        }

        internal static void VerifyPathSegmentTokensAreEqual(IEnumerable<PathSegmentToken> expectedTokens, IEnumerable<PathSegmentToken> actualTokens, AssertionHandler assert)
        {
            VerificationUtils.VerifyEnumerationsAreEqual(
                expectedTokens,
                actualTokens,
                VerifyPathSegmentTokensAreEqual,
                ToDebugString,
                assert);
        }

        /// <summary>
        /// Verifies that two queries are equal.
        /// </summary>
        /// <param name="expected">The expected query.</param>
        /// <param name="actual">The actual query.</param>
        /// <param name="assert">Assertion handler to use.</param>
        internal static void VerifySyntaxTreesAreEqual(SyntacticTree expected, SyntacticTree actual, AssertionHandler assert)
        {
            try
            {
                if (!VerifyNullnessMatches(expected, actual, assert, "query")) return;
                VerifySyntaxTreesAreEqualImpl(expected, actual, assert);
            }
            catch (Exception)
            {
                assert.Warn("Expected query: " + expected.ToDebugString());
                assert.Warn("Actual query: " + actual.ToDebugString());
                throw;
            }
        }

        private static void VerifySyntaxTreesAreEqualImpl(SyntacticTree expected, SyntacticTree actual, AssertionHandler assert)
        {
            VerifyStringsAreEqual(expected.Path, actual.Path, assert);
            VerifyQueryTokensAreEqual(expected.Filter, actual.Filter, assert);
            if (expected.OrderByTokens != null && actual.OrderByTokens != null)
                VerifyQueryTokensAreEqual(expected.OrderByTokens.Cast<QueryToken>(), actual.OrderByTokens.Cast<QueryToken>(), assert);
            else if ((expected.OrderByTokens != null && actual.OrderByTokens == null) || (expected.OrderByTokens == null && actual.OrderByTokens != null))
                assert.Fail("Query tokens are different");
            assert.AreEqual(expected.Skip, actual.Skip, "Skip values are different.");
            VerificationUtils.VerifyEnumerationsAreEqual(
                expected.QueryOptions,
                actual.QueryOptions,
                VerifyCustomQueryOptionQueryTokensAreEqual,
                (item) => item.ToDebugString(),
                assert);
        }

        private static void VerifyLiteralQueryTokensAreEqual(LiteralToken expected, LiteralToken actual, AssertionHandler assert)
        {
            assert.AreEqual(expected.Value, actual.Value, "Literal values are different.");
        }

        private static void VerifyBinaryOperatorQueryTokensAreEqual(BinaryOperatorToken expected, BinaryOperatorToken actual, AssertionHandler assert)
        {
            assert.AreEqual(expected.OperatorKind, actual.OperatorKind, "The binary operator kind doesn't match the expected one.");
            VerifyQueryTokensAreEqual(expected.Left, actual.Left, assert);
            VerifyQueryTokensAreEqual(expected.Right, actual.Right, assert);
        }

        private static void VerifyUnaryOperatorQueryTokensAreEqual(UnaryOperatorToken expected, UnaryOperatorToken actual, AssertionHandler assert)
        {
            assert.AreEqual(expected.OperatorKind, actual.OperatorKind, "The unary operator kind doesn't match the expected one.");
            VerifyQueryTokensAreEqual(expected.Operand, actual.Operand, assert);
        }

        private static void VerifyPropertyAccessQueryTokensAreEqual(EndPathToken expected, EndPathToken actual, AssertionHandler assert)
        {
            assert.AreEqual(expected.Identifier, actual.Identifier, "The Name of the property access token doesn't match the expected one.");
            VerifyQueryTokensAreEqual(expected.NextToken, actual.NextToken, assert);
        }

        private static void VerifyNonRootSegmentQueryTokensAreEqual(InnerPathToken expected, InnerPathToken actual, AssertionHandler assert)
        {
            assert.AreEqual(expected.Identifier, actual.Identifier, "The Name of the navigation property token doesn't match the expected one.");
            VerifyQueryTokensAreEqual(expected.NextToken, actual.NextToken, assert);
        }

        private static void VerifyFunctionCallQueryTokensAreEqual(FunctionCallToken expected, FunctionCallToken actual, AssertionHandler assert)
        {
            assert.AreEqual(expected.Name, actual.Name, "The Name of the function call token doesn't match the expected one.");
            VerifyQueryTokensAreEqual(expected.Arguments, actual.Arguments, assert);
        }

        private static void VerifyFunctionParameterTokensAreEqual(FunctionParameterToken expected, FunctionParameterToken actual, AssertionHandler assert)
        {
            assert.AreEqual(expected.ParameterName, actual.ParameterName, "The Name of the function call token doesn't match the expected one.");
            VerifyQueryTokensAreEqual(expected.ValueToken, actual.ValueToken, assert);
        }

        private static void VerifyOrderByQueryTokensAreEqual(OrderByToken expected, OrderByToken actual, AssertionHandler assert)
        {
            assert.AreEqual(expected.Direction, actual.Direction, "The Direction of the order by doesn't match the expected one.");
            VerifyQueryTokensAreEqual(expected.Expression, actual.Expression, assert);
        }

        private static void VerifySelectQueryTokensAreEqual(SelectToken expected, SelectToken actual, AssertionHandler assert)
        {
            VerifyPathSegmentTokensAreEqual(expected.Properties, actual.Properties, assert);
        }

        private static void VerifyStarQueryTokensAreEqual(StarToken expected, StarToken actual, AssertionHandler assert)
        {
            VerifyQueryTokensAreEqual(expected.NextToken, actual.NextToken, assert);
        }

        private static void VerifyCustomQueryOptionQueryTokensAreEqual(CustomQueryOptionToken expected, CustomQueryOptionToken actual, AssertionHandler assert)
        {
            assert.AreEqual(expected.Name, actual.Name, "The Name of the query option token doesn't match the expected one.");
            assert.AreEqual(expected.Value, actual.Value, "The Value of the query option token doesn't match the expected one.");
        }

        internal static string ToDebugString(this QueryToken token)
        {
            if (token == null) return "(null)";
            string result = "";

            switch (token.Kind)
            {
                case QueryTokenKind.InnerPath:
                    var nonRootSegment = (InnerPathToken)token;
                    result = nonRootSegment.Identifier;
                    break;
                case QueryTokenKind.Literal:
                    var literal = (LiteralToken)token;
                    if (literal.Value is string)
                    {
                        result = "'" + literal.Value.ToString() + "'";
                    }
                    else
                    {
                        result = literal.Value == null ? "<null>" : literal.Value.ToString();
                    }
                    break;
                case QueryTokenKind.BinaryOperator:
                    var binary = (BinaryOperatorToken)token;
                    result = "(" + binary.Left.ToDebugString() + " " + binary.OperatorKind.ToOperatorName() + " " + binary.Right.ToDebugString() + ")";
                    break;
                case QueryTokenKind.UnaryOperator:
                    var unary = (UnaryOperatorToken)token;
                    result = "(" + unary.OperatorKind.ToOperatorName() + unary.Operand.ToDebugString() + ")";
                    break;
                case QueryTokenKind.EndPath:
                    var propertyAccess = (EndPathToken)token;
                    if (propertyAccess.NextToken != null)
                    {
                        result = propertyAccess.NextToken.ToDebugString() + "/";
                    }
                    result += propertyAccess.Identifier;
                    break;
                case QueryTokenKind.FunctionCall:
                    var functionCall = (FunctionCallToken)token;
                    result = functionCall.Name + "(" + string.Join(", ", functionCall.Arguments.Select(a => a.ToDebugString()).ToArray()) + ")";
                    break;
                case QueryTokenKind.CustomQueryOption:
                    var queryOption = (CustomQueryOptionToken)token;
                    result = queryOption.Name + "=" + queryOption.Value;
                    break;
                case QueryTokenKind.OrderBy:
                    var orderBy = (OrderByToken)token;
                    result = orderBy.Expression.ToDebugString() + " " + (orderBy.Direction == OrderByDirection.Ascending ? "asc" : "desc");
                    break;
                case QueryTokenKind.Select:
                    var select = (SelectToken)token;
                    result = string.Join(", ", select.Properties.Select(p => p.ToDebugString()).ToArray());
                    break;
                case QueryTokenKind.Star:
                    var star = (StarToken)token;
                    if (star.NextToken != null)
                    {
                        result = star.NextToken.ToDebugString() + "/";
                    }
                    result += "*";
                    break;
                case QueryTokenKind.FunctionParameter:
                    var functionParameter = (FunctionParameterToken) token;
                    result += functionParameter.ParameterName + " = " + functionParameter.ValueToken.ToDebugString();
                    break;
                default:
                    throw new Exception("Token kind '" + token.Kind.ToString() + "' not yet supported by ToDebugString.");
            }

            return result;
        }

        internal static string ToDebugString(this SyntacticTree queryDescriptor)
        {
            if (queryDescriptor == null) return "(null)";
            string result = queryDescriptor.Path.ToDebugString();
            var queryOptions = new List<KeyValuePair<string, string>>();
            if (queryDescriptor.Filter != null) queryOptions.Add(new KeyValuePair<string, string>("$filter", queryDescriptor.Filter.ToDebugString()));
            if (queryDescriptor.OrderByTokens != null) queryOptions.Add(new KeyValuePair<string, string>("$orderby", string.Join(",", queryDescriptor.OrderByTokens.Select(ob => ob.ToDebugString()).ToArray())));
            if (queryDescriptor.Select != null) queryOptions.Add(new KeyValuePair<string, string>("$select", queryDescriptor.Select.ToDebugString()));
            if (queryOptions.Count > 0) result += "?" + string.Join("&", queryOptions.Select(qo => qo.Key + "=" + qo.Value).ToArray());

            return result;
        }

        internal static string ToDebugString(this Microsoft.OData.UriParser.NamedValue keyValue)
        {
            string result = "";

            if (keyValue.Name != null)
            {
                result = keyValue.Name + "=";
            }
            result += keyValue.Value.ToDebugString();

            return result;
        }

        internal static string ToDebugString(this PathSegmentToken token)
        {
            if (token == null) return "(null)";
            return token.Identifier;
        }

        internal static string ToDebugString(this IEnumerable<string> path)
        {
            if (path == null) return "(null)";
            return string.Join("/", path);
        }
    }
}
