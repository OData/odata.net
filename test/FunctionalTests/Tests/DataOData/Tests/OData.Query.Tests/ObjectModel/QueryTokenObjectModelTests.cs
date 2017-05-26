//---------------------------------------------------------------------
// <copyright file="QueryTokenObjectModelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.ObjectModel
{
    #region Namespaces
    using System.Linq;
    using Microsoft.OData.UriParser;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for the QueryTokenObjectModelTests object model type.
    /// </summary>
    [TestClass, TestCase]
    public class QueryTokenObjectModelTests : ODataTestCase
    {
        [TestMethod, Variation(Description = "Test the default values of a key value.")]
        public void NamedValueDefaultTest()
        {
            LiteralToken literal = new LiteralToken(null);
            NamedValue keyValue = new NamedValue("a", literal);
            this.Assert.AreEqual("a", keyValue.Name, "The Name property has an unexpected value.");
            this.Assert.AreEqual(literal, keyValue.Value, "The Value property has an unexpected value.");
        }

        [TestMethod, Variation(Description = "Test the default values of a binary operator.")]
        public void BinaryOperatorQueryTokenDefaultTest()
        {
            LiteralToken left = new LiteralToken(null);
            LiteralToken right = new LiteralToken(null);
            BinaryOperatorToken binary = new BinaryOperatorToken(BinaryOperatorKind.Or, left, right);
            this.Assert.AreEqual(QueryTokenKind.BinaryOperator, binary.Kind, "The InternalKind property has an unexpected value.");
            this.Assert.AreEqual(BinaryOperatorKind.Or, binary.OperatorKind, "The OperatorKind property should be Or.");
            this.Assert.AreEqual(left, binary.Left, "The Left property has an unexpected value.");
            this.Assert.AreEqual(right, binary.Right, "The Right property has an unexpected value.");
        }

        [TestMethod, Variation(Description = "Test the default values of a unary operator.")]
        public void UnaryOperatorQueryTokenDefaultTest()
        {
            LiteralToken operand = new LiteralToken(null);
            UnaryOperatorToken unary = new UnaryOperatorToken(UnaryOperatorKind.Negate, operand);
            this.Assert.AreEqual(QueryTokenKind.UnaryOperator, unary.Kind, "The InternalKind property has an unexpected value.");
            this.Assert.AreEqual(UnaryOperatorKind.Negate, unary.OperatorKind, "The OperatorKind property should be Negate.");
            this.Assert.AreEqual(operand, unary.Operand, "The Operand property has an unexpected value.");
        }

        [TestMethod, Variation(Description = "Test the default values of a literal.")]
        public void LiteralQueryTokenDefaultTest()
        {
            LiteralToken literal = new LiteralToken(null);
            this.Assert.AreEqual(QueryTokenKind.Literal, literal.Kind, "The InternalKind property has an unexpected value.");
            this.Assert.IsNull(literal.Value, "The Value property should be null.");
        }

        [TestMethod, Variation(Description = "Test the default values of a function call.")]
        public void FunctionCallQueryTokenDefaultTest()
        {
            FunctionCallToken functionCall = new FunctionCallToken("substring", null);
            this.Assert.AreEqual(QueryTokenKind.FunctionCall, functionCall.Kind, "The InternalKind property has an unexpected value.");
            this.Assert.AreEqual("substring", functionCall.Name, "The Name property has an unexpected value.");
            this.Assert.IsTrue(functionCall.Arguments != null && functionCall.Arguments.Count() == 0, "The Arguments property should NOT be null but is empty.");
        }

        [TestMethod, Variation(Description = "Test the default values of a property access.")]
        public void PropertyAccessQueryTokenDefaultTest()
        {
            EndPathToken endPath = new EndPathToken("something", null);
            this.Assert.AreEqual(QueryTokenKind.EndPath, endPath.Kind, "The InternalKind property has an unexpected value.");
            this.Assert.AreEqual("something", endPath.Identifier, "The Name property has an unexpected value.");
            this.Assert.IsNull(endPath.NextToken, "The NextToken property should be null.");
        }

        [TestMethod, Variation(Description = "Test the default values of an order by.")]
        public void OrderByQueryTokenDefaultTest()
        {
            var expression = new InnerPathToken(string.Empty, null, null);
            OrderByToken orderby = new OrderByToken(expression, OrderByDirection.Ascending);
            this.Assert.AreEqual(QueryTokenKind.OrderBy, orderby.Kind, "The InternalKind property has an unexpected value.");
            this.Assert.AreEqual(OrderByDirection.Ascending, orderby.Direction, "The Direction property should be Ascending.");
            this.Assert.AreEqual(expression, orderby.Expression, "The Expression property has an unexpected value.");
        }

        [TestMethod, Variation(Description = "Test the default values of a query option.")]
        public void QueryOptionQueryTokenDefaultTest()
        {
            CustomQueryOptionToken queryOption = new CustomQueryOptionToken(null, null);
            this.Assert.AreEqual(QueryTokenKind.CustomQueryOption, queryOption.Kind, "The InternalKind property has an unexpected value.");
            this.Assert.IsNull(queryOption.Name, "The Name property should be null.");
            this.Assert.IsNull(queryOption.Value, "The Value property should be null.");
        }

        [TestMethod, Variation(Description = "Test the default values of a select query option.")]
        public void SelectQueryTokenDefaultTest()
        {
            SelectToken select = new SelectToken(null);
            this.Assert.AreEqual(QueryTokenKind.Select, select.Kind, "The InternalKind property has an unexpected value.");
            this.Assert.IsTrue(select.Properties != null && select.Properties.Count() == 0, "The Properties property should NOT be null but is empty.");
        }

        [TestMethod, Variation(Description = "Test the default values of a expand query option.")]
        public void ExpandQueryTokenDefaultTest()
        {
            ExpandToken expand = new ExpandToken(null);
            this.Assert.AreEqual(QueryTokenKind.Expand, expand.Kind, "The InternalKind property has an unexpected value.");
            this.Assert.IsTrue(expand.ExpandTerms != null && expand.ExpandTerms.Count() == 0, "The Properties property should NOT be null but is empty.");
        }

        [TestMethod, Variation(Description = "Test the default values of a star token.")]
        public void StarQueryTokenDefaultTest()
        {
            StarToken starToken = new StarToken(null);
            this.Assert.AreEqual(QueryTokenKind.Star, starToken.Kind, "The InternalKind property has an unexpected value.");
            this.Assert.IsNull(starToken.NextToken, "The NextToken property should be null.");
        }
    }
}
