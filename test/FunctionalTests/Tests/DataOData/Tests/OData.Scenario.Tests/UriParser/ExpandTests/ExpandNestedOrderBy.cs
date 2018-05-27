//---------------------------------------------------------------------
// <copyright file="ExpandNestedOrderBy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.UriParser.ExpandTests
{
    #region namespaces

    using System.Runtime.CompilerServices;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion namespaces

    [TestClass]
    public class ExpandNestedOrderBy : UriParserTestsBase
    {
        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void Property()
        {
            this.ApprovalVerifyExpandParser(orderBase, "CustomerForOrder($orderby=FirstName)");

            this.TestAllInOneExtensionExpand(
               orderBase,
               "customerfororder($ORDERby=firstname)",
               "CustomerForOrder($orderby=FirstName)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PropertyDesc()
        {
            this.ApprovalVerifyExpandParser(orderBase, "CustomerForOrder($orderby=FirstName desc)");

            this.TestAllInOneExtensionExpand(
               orderBase,
               "CustomerForOrder($orderby=FirstName DESC)",
               "CustomerForOrder($orderby=FirstName desc)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void MultipleProperties()
        {
            this.ApprovalVerifyExpandParser(orderBase, "CustomerForOrder($orderby=FirstName desc, LastName, City asc, Birthday desc)");

            this.TestAllInOneExtensionExpand(
               orderBase,
               "CustomerForOrder($ordeRby=FirstName desc, LastNaMe, City aSc, Birthday desc)",
               "CustomerForOrder($orderby=FirstName desc, LastName, City asc, Birthday desc)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void NavigationProperty()
        {
            this.ApprovalVerifyExpandParser(orderDetailBase, "AssociatedOrder($orderby=CustomerForOrder/FirstName desc)");

            this.TestAllInOneExtensionExpand(
               orderDetailBase,
               "AssociatEDOrder($orderby=CustomerFOROrder/firstNAme deSc)",
               "AssociatedOrder($orderby=CustomerForOrder/FirstName desc)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void NestedNots()
        {
            this.ApprovalVerifyExpandParser(orderDetailBase, "AssociatedOrder($orderby=OrderDetails/all(a: not (not (not (not (not (not (not (not (not ((not (a/ProductID ge 3.2)) or (not (a/OrderPlaced eq a/OrderPlaced)))))))))))))");

            this.TestAllInOneExtensionExpand(
               orderDetailBase,
               "associatedOrder($orderby=OrderDetails/all(a: not (not (not (not (not (not (not (NOT (not ((not (a/ProductID ge 3.2)) OR (not (a/OrderPlacEd EQ a/OrderPlaced)))))))))))))",
               "AssociatedOrder($orderby=OrderDetails/all(a: not (not (not (not (not (not (not (not (not ((not (a/ProductID ge 3.2)) or (not (a/OrderPlaced eq a/OrderPlaced)))))))))))))");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void OrderByHoursMinutesSeconds()
        {
            this.ApprovalVerifyExpandParser(orderBase, "CustomerForOrder($orderby=hour(TimeBetweenLastTwoOrders) mul 3 sub minute(TimeBetweenLastTwoOrders) div 2 add second(TimeBetweenLastTwoOrders) mod 30)");

            this.TestAllInOneExtensionExpand(
               orderBase,
               "CustomerFOROrder($orDErby=hOUr(TimeBetweenLastTwoOrders) mul 3 sub miNute(TimeBetweenLaStTwoOrders) diV 2 add secOnd(TimeBetweenLASTTwoOrders) mod 30)",
               "CustomerForOrder($orderby=hour(TimeBetweenLastTwoOrders) mul 3 sub minute(TimeBetweenLastTwoOrders) div 2 add second(TimeBetweenLastTwoOrders) mod 30)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void Concat()
        {
            this.ApprovalVerifyExpandParser(orderBase, "CustomerForOrder($orderby=endswith(substring(toupper(concat(FirstName, LastName)), 7), 'r') and contains('H','Hello'))");
            
            this.TestAllInOneExtensionExpand(
               orderBase,
               "CustomerForOrder($orderby=endswith(substring(toupper(conCat(FIRSTName, lastname)), 7), 'r') aNd contAIns('H','Hello'))",
               "CustomerForOrder($orderby=endswith(substring(toupper(concat(FirstName, LastName)), 7), 'r') and contains('H','Hello'))");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void IsofCast()
        {
            this.ApprovalVerifyExpandParser(orderBase, "CustomerForOrder($orderby=isof('Microsoft.Test.Taupo.OData.WCFService.Customer') and cast('Microsoft.Test.Taupo.OData.WCFService.Customer') ne null)");

            //The type name could also be case insensitive here, but it would cause result constant node to contain the raw type literal text.
            this.TestAllInOneExtensionExpand(
               orderBase,
               "CustomerForOrder($orderby=ISOF('Microsoft.Test.Taupo.OData.WCFService.Customer') and cast('Microsoft.Test.Taupo.OData.WCFService.Customer') ne null)",
               "CustomerForOrder($orderby=isof('Microsoft.Test.Taupo.OData.WCFService.Customer') and cast('Microsoft.Test.Taupo.OData.WCFService.Customer') ne null)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void SpatialLengthNotLineString()
        {
            ODataUriParser parser = this.CreateExpandUriParser(orderBase, "LoggedInEmployee($orderby=geo.length(geometry'Polygon((10 30, 7 28, 6 6, 10 30))') lt 0.5)");
            try
            {
                parser.ParseSelectAndExpand();
                Assert.Fail("Should throw with ODataException");
            }
            catch (ODataException e)
            {
                var expected = ODataExpectedExceptions.ODataException("MetadataBinder_NoApplicableFunctionFound", "geo.length", "geo.length(Edm.GeometryLineString Nullable=true); geo.length(Edm.GeographyLineString Nullable=true)");
                expected.ExpectedMessage.Verifier.VerifyMatch("MetadataBinder_NoApplicableFunctionFound", e.Message, "geo.length", "geo.length(Edm.GeometryLineString Nullable=true); geo.length(Edm.GeographyLineString Nullable=true)");
            }
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void SpatialLengthPolygonNotFormedCorrectly()
        {
            ODataUriParser parser = this.CreateExpandUriParser(orderBase, "LoggedInEmployee($orderby=geo.length(geometry'Polygon(10 30, 7 28, 6 6, 10 30)') lt 0.5)");
            try
            {
                parser.ParseSelectAndExpand();
                Assert.Fail("Should throw with ODataException");
            }
            catch (ODataException e)
            {
                var expected = ODataExpectedExceptions.ODataException("UriQueryExpressionParser_UnrecognizedLiteralWithReason", "Edm.Geometry", "geometry'Polygon(10 30, 7 28, 6 6, 10 30)'", "11", "geo.length(geometry'Polygon(10 30, 7 28, 6 6, 10 30)') lt 0.5", "Expecting token type \"LeftParen\" with text \"\" but found \"Type:[2] Text:[10]\".");
                expected.ExpectedMessage.Verifier.VerifyMatch("UriQueryExpressionParser_UnrecognizedLiteralWithReason", e.Message, "Edm.Geometry", "geometry'Polygon(10 30, 7 28, 6 6, 10 30)'", "11", "geo.length(geometry'Polygon(10 30, 7 28, 6 6, 10 30)') lt 0.5", "Expecting token type \"LeftParen\" with text \"\" but found \"Type:[2] Text:[10]\".");
            }
        }
    }
}
