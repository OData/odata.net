//---------------------------------------------------------------------
// <copyright file="SelectExpandTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using ApprovalTests;
using Microsoft.OData.Core;
using Microsoft.OData.Core.UriParser;
using Microsoft.Test.Taupo.OData.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.UriParser
{
    using System;
    using System.Runtime.CompilerServices;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// End user tests for ParseSelectExpand method on the UriParser
    /// </summary>
    [TestClass]
    public class SelectExpandTest : UriParserTestsBase
    {

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void SingleNavPropSelectExpandTest()
        {
            this.ApprovalVerifySelectAndExpandParser(orderDetailBase, "", "AssociatedOrder($expand=CustomerForOrder)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void SingleNavPropSelectExpandWithEntitySet()
        {
            this.ApprovalVerifySelectAndExpandParser(orderDetailBase, "", "AssociatedOrder($expand=CustomerForOrder)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void TwoNapPropsExpanded()
        {
            this.ApprovalVerifySelectAndExpandParser(orderDetailBase, "", "AssociatedOrder($expand=CustomerForOrder),ProductOrdered");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void TwoNapPropsExpandedWithEntitySet()
        {
            this.ApprovalVerifySelectAndExpandParser(orderDetailBase, "", "AssociatedOrder($expand=CustomerForOrder),ProductOrdered");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ExpandWhenPathSelected()
        {
            this.ApprovalVerifySelectAndExpandParser(orderDetailBase, "", "AssociatedOrder($select=*),AssociatedOrder($expand=CustomerForOrder)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void OnlyOneNavPropsExpandedWhenOnlyOneSelected()
        {
            this.ApprovalVerifySelectAndExpandParser(orderDetailBase, "", "AssociatedOrder($expand=CustomerForOrder),ProductOrdered");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void NoExpandWhenNavPropNotSelected()
        {
            this.ApprovalVerifySelectAndExpandParser(orderDetailBase, "Quantity", "AssociatedOrder($expand=CustomerForOrder)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ExpandWhenItemPrimitiveAndNavPropSelected()
        {
            this.ApprovalVerifySelectAndExpandParser(orderDetailBase, "OrderPlaced,AssociatedOrder", "AssociatedOrder");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void SelectPrimitiveOnExpandedNavProp()
        {
            this.ApprovalVerifySelectAndExpandParser(orderDetailBase, "", "AssociatedOrder($expand=CustomerForOrder($select=HomeAddress))");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void DeepSelectionsAreRedundantIfEntireSubtreeIsSelected()
        {
            // note that the selection of 'AssociatedOrder' in the middle of the $select makes the rest of it redundant.
            this.ApprovalVerifySelectAndExpandParser(
                orderDetailBase,
                "",
                "AssociatedOrder($expand=CustomerForOrder($select=City)),AssociatedOrder,AssociatedOrder($select=*),AssociatedOrder($select=OrderDate),AssociatedOrder($expand=CustomerForOrder($expand=Orders($select=Microsoft.Test.Taupo.OData.WCFService.OrdersWithMoreThanTwoItems)))");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void TwoSelectExpandEach800LevelsShouldNotThrow()
        {
            string expqry = "Orders($expand=CustomerForOrder";
            string closeParanthesis = ")";

            // After add $expand support star, there will be a StackOverflow error, change to 299 will not show issue.
            for (int i = 0; i < 299; i++)
            {
                expqry += @"($expand=Orders($expand=CustomerForOrder";
                closeParanthesis += "))";
            }

            // Adding another Expand
            expqry = expqry + closeParanthesis + ",Orders($expand=OrderDetails($expand=AssociatedOrder";
            closeParanthesis = "))";
            for (int i = 0; i < 298; i++)
            {
                expqry += @"($expand=OrderDetails($expand=AssociatedOrder";
                closeParanthesis += "))";
            }
            var parser = this.CreateSelectAndExpandUriParser(customerBase, "", expqry + closeParanthesis);
            Assert.IsNotNull(parser.ParseSelectAndExpand(), "Tree cannot be null");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ExpandOnly800LevelsShouldNotThrow()
        {
            // Testing the width
            string expqry = "Orders($expand=CustomerForOrder";
            string closeParanthesis = ")";

            // After add $expand support star, there will be a StackOverflow error, change to 299 will not show issue.
            for (int i = 0; i < 299; i++)
            {
                expqry += @"($expand=Orders($expand=CustomerForOrder";
                closeParanthesis += "))";
            }
            var parser = this.CreateSelectAndExpandUriParser(customerBase, "", expqry + closeParanthesis);
            Assert.IsNotNull(parser.ParseSelectAndExpand(), "Tree cannot be null");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ErrorWhenSelectExpandMoreThan800LevelsDeep()
        {
            string expqry = "Orders($expand=CustomerForOrder";
            string closeParanthesis = ")";

            for (int i = 0; i < 401; i++)
            {
                expqry += @"Orders/CustomerForOrder/";
                closeParanthesis += "))";
            }

            try
            {
                var parser = this.CreateSelectAndExpandUriParser(customerBase, null, expqry + closeParanthesis);
                parser.ParseSelectAndExpand();
                Assert.Fail("Should throw exception.");
            }
            catch (ODataException e)
            {
                var expected = ODataExpectedExceptions.ODataException("UriQueryExpressionParser_TooDeep");
                expected.ExpectedMessage.Verifier.VerifyMatch("UriQueryExpressionParser_TooDeep", e.Message);
            }
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ErrorWhenExpandInParanthesis()
        {
            var parser = this.CreateSelectAndExpandUriParser(orderBase, "", "(AssociatedOrder($expand=CustomerForOrder))");
            try
            {
                parser.ParseSelectAndExpand();
                Assert.Fail("Should throw exception.");
            }
            catch (ODataException e)
            {
                var expected = ODataExpectedExceptions.ODataException("ExpressionToken_IdentifierExpected", "0");
                expected.ExpectedMessage.Verifier.VerifyMatch("ExpressionToken_IdentifierExpected", e.Message, "0");
            }
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ErrorWhenExpandedNotWellFormatted()
        {
            var parser = this.CreateSelectAndExpandUriParser(orderDetailBase, "", "AssociatedOrder($Expand=CustomerForOrder)");
            try
            {
                parser.ParseSelectAndExpand();
                Assert.Fail("Should throw exception.");
            }
            catch (ODataException e)
            {
                var expected = ODataExpectedExceptions.ODataException("UriSelectParser_TermIsNotValid", "($Expand=CustomerForOrder)");
                expected.ExpectedMessage.Verifier.VerifyMatch("UriSelectParser_TermIsNotValid", e.Message, "($Expand=CustomerForOrder)");
            }
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ErrorWhenSelectExpandOnNonStructuredType()
        {
            var parser = this.CreateSelectAndExpandUriParser(new Uri("http://www.potato.com/Customers(10)/City"), "", "AssociatedOrder($Expand=CustomerForOrder)");
            try
            {
                parser.ParseSelectAndExpand();
                Assert.Fail("Should throw exception.");
            }
            catch (ODataException e)
            {
                var expected = ODataExpectedExceptions.ODataException("UriParser_TypeInvalidForSelectExpand", "Edm.String");
                expected.ExpectedMessage.Verifier.VerifyMatch("UriParser_TypeInvalidForSelectExpand", e.Message, "Edm.String");
            }
        }
    }
}
