//---------------------------------------------------------------------
// <copyright file="SkipTokenAndETagParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Parsing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion Namespaces

    /// <summary>
    /// TODO: test the rest of SkipTokenAndETagParser
    /// </summary>
    [TestClass]
    public class SkipTokenAndETagParserTests
    {
        [TestMethod]
        public void RequestUriExtractKeyValuesTest()
        {
            VerifyInvokeExtractKeyValues("123", "123");
            VerifyInvokeExtractKeyValues("123,123", "123", "123");
            VerifyInvokeExtractKeyValues("'string'", "'string'");
            VerifyInvokeExtractKeyValues("'string','other'", "'string'", "'other'");
            VerifyInvokeExtractKeyValues("'string,same','other'", "'string,same'", "'other'");
            VerifyInvokeExtractKeyValues("'string,same()','other'", "'string,same()'", "'other'");
            VerifyInvokeExtractKeyValues("'string ''double''','other'", "'string ''double'''", "'other'");
            VerifyInvokeExtractKeyValues("'string ''double''''','other'", "'string ''double'''''", "'other'");
            VerifyInvokeExtractKeyValues("'string ''double'' ,'", "'string ''double'' ,'");
        }

        [TestMethod]
        public void ParseSkipTokenDoublePrecision()
        {
            IList<object> values;
            bool skipTokenParsed = SkipTokenAndETagParser.TryParseNullableTokens("1.0099999904632568", out values);
            Assert.IsTrue(skipTokenParsed);
            Assert.AreEqual(values.Count, 1);
            Assert.AreEqual(values.First(), "1.0099999904632568");

            object skipTokenValue = SkipTokenExpressionBuilder.ParseSkipTokenLiteral(values.First() as string);
            Assert.AreEqual(skipTokenValue, 1.0099999904632568);
        }

        [TestMethod]
        public void ParseSkipTokenSinglePrecision()
        {
            IList<object> values;
            bool skipTokenParsed = SkipTokenAndETagParser.TryParseNullableTokens(Single.MinValue.ToString("R"), out values);
            Assert.IsTrue(skipTokenParsed);
            Assert.AreEqual(values.Count, 1);
            Assert.AreEqual(values.First(), Single.MinValue.ToString("R"));

            object skipTokenValue = SkipTokenExpressionBuilder.ParseSkipTokenLiteral(Single.MinValue.ToString("R"));
            Assert.AreEqual(skipTokenValue, Single.MinValue);
        }

        private static void VerifyInvokeExtractKeyValues(string filter, params string[] values)
        {
            IEnumerator<string> actualEnumerator = InvokeExtractKeyValues(filter).GetEnumerator();
            int expectedIndex = 0;
            while (expectedIndex < values.Length)
            {
                Assert.IsTrue(actualEnumerator.MoveNext(), "Actual can move along with expected: [" + filter + "]");

                string message = "Checking for property on [" + filter + "] for ";
                string current = actualEnumerator.Current;
                Assert.AreEqual(values[expectedIndex], current, message + "text");
                expectedIndex++;
            }

            Assert.IsFalse(actualEnumerator.MoveNext(), "Actual has no more elements than expected: [" + filter + "]");
        }

        private static IEnumerable<string> InvokeExtractKeyValues(string filter)
        {
            IList<object> values;
            if (!SkipTokenAndETagParser.TryParseNullableTokens(filter, out values))
            {
                throw new Exception("Parsing failed for " + filter);
            }

            return values.Cast<string>();
        }

        [TestMethod]
        public void ParseSkipTokenBooleanTrue()
        {
            IList<object> values;
            bool skipTokenParsed = SkipTokenAndETagParser.TryParseNullableTokens("true", out values);
            Assert.IsTrue(skipTokenParsed);
            Assert.AreEqual(values.Count, 1);
            Assert.AreEqual(values.First(), "true");
        }

        [TestMethod]
        public void ParseSkipTokenBooleanFalse()
        {
            IList<object> values;
            bool skipTokenParsed = SkipTokenAndETagParser.TryParseNullableTokens("false", out values);
            Assert.IsTrue(skipTokenParsed);
            Assert.AreEqual(values.Count, 1);
            Assert.AreEqual(values.First(), "false");
        }
    }
}