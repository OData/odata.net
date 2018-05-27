//---------------------------------------------------------------------
// <copyright file="ExpandNestedFilter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.UriParser.ExpandTests
{
    #region namespaces
    using System;
    using System.Runtime.CompilerServices;
    using FluentAssertions;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion namespaces

    [TestClass]
    public class ExpandNestedFilter : UriParserTestsBase
    {
        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void LtAndGe()
        {
            this.ApprovalVerifyExpandParser(orderBase, "OrderDetails($filter=UnitPrice lt 93 and UnitPrice ge 3.14)");

            this.TestAllInOneExtensionExpand(
                orderBase,
                "orderdetails($filter=unitprice LT 93 aNd UnitPricE ge 3.14)",
                "OrderDetails($filter=UnitPrice lt 93 and UnitPrice ge 3.14)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void DoesNotResolveToBool()
        {
            var parser = this.CreateExpandUriParser(orderBase, "OrderDetails($filter=UnitPrice mul 200 add UnitPrice div 4)");
            try
            {
                parser.ParseSelectAndExpand();
                Assert.Fail("Should throw when does not resolve to boolean type");
            }
            catch (ODataException e)
            {
                var expected = ODataExpectedExceptions.ODataException("MetadataBinder_FilterExpressionNotSingleValue");
                expected.ExpectedMessage.Verifier.VerifyMatch("MetadataBinder_FilterExpressionNotSingleValue", e.Message);
            }
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void SpatialDistance()
        {
            this.ApprovalVerifyExpandParser(orderBase, "LoggedInEmployee($filter=geo.distance(Home, Office) lt 0.5)");

            this.TestAllInOneExtensionExpand(
                orderBase,
                "LoggedINEmployee($fILter=geo.disTance(HoMe, Office) LT 0.5)",
                "LoggedInEmployee($filter=geo.distance(Home, Office) lt 0.5)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void SpatialLength()
        {
            this.ApprovalVerifyExpandParser(orderBase, "LoggedInEmployee($filter=geo.length(geometry'LineString(10 30, 7 28, 6 6)') lt 0.5)");

            this.TestAllInOneExtensionExpand(
                orderBase,
                "LoggedInEMPloyee($filter=geo.lENGTH(gEometry'LIneString(10 30, 7 28, 6 6)') lt 0.5)",
                "LoggedInEmployee($filter=geo.length(geometry'LineString(10 30, 7 28, 6 6)') lt 0.5)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void SpatialIntersects()
        {
            this.ApprovalVerifyExpandParser(orderBase, "LoggedInEmployee($filter=geo.intersects(geometry'Point(10 30)', geometry'Polygon((5 16, 5 30, 20 30, 20 16, 5 16))'))");

            this.TestAllInOneExtensionExpand(
                orderBase,
                "LoggedInEmploYEe($FILTER=geo.INTERSECTS(geometry'Point(10 30)', geometry'POLYGON((5 16, 5 30, 20 30, 20 16, 5 16))'))",
                "LoggedInEmployee($filter=geo.intersects(geometry'Point(10 30)', geometry'Polygon((5 16, 5 30, 20 30, 20 16, 5 16))'))");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void NotEndsWithAndBeginsWith()
        {
            this.ApprovalVerifyExpandParser(orderDetailBase, "ProductOrdered($filter=not endswith(Name,'milk') and startswith(Name, 'word') or length(QuantityPerUnit) eq 15)");

            this.TestAllInOneExtensionExpand(
                orderDetailBase,
                "ProduCTOrdered($filter=not endSwith(Name,'milk') and startSwith(Name, 'word') or length(QuantityPerUnit) eq 15)",
                "ProductOrdered($filter=not endswith(Name,'milk') and startswith(Name, 'word') or length(QuantityPerUnit) eq 15)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundFloorCeiling()
        {
            this.ApprovalVerifyExpandParser(orderDetailBase, "ProductOrdered($filter=round(UnitPrice) eq floor(UnitPrice) and round(UnitPrice) ne ceiling(UnitPrice))");

            this.TestAllInOneExtensionExpand(
               orderDetailBase,
               "ProductOrdered($filter=ROUND(UnitPrice) eq floor(unitPrice) and round(UnItPrice) ne CEILING(UnitPrice))",
               "ProductOrdered($filter=round(UnitPrice) eq floor(UnitPrice) and round(UnitPrice) ne ceiling(UnitPrice))");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void Replace()
        {
            this.ApprovalVerifyExpandParser(orderBase, "CustomerForOrder($filter=replace(FirstName, 'd', 'n') eq 'bran')");

            this.TestAllInOneExtensionExpand(
               orderBase,
               "CustomerForOrder($filter=REPLACE(FirSTName, 'd', 'n') eq 'bran')",
               "CustomerForOrder($filter=replace(FirstName, 'd', 'n') eq 'bran')");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ContainsIndexOf()
        {
            this.ApprovalVerifyExpandParser(orderDetailBase, "ProductOrdered($filter=not contains('milk',Name) and indexof(Name, 'a') lt 3 or concat(Name, QuantityPerUnit) eq 'bubbles')");

            this.TestAllInOneExtensionExpand(
               orderDetailBase,
               "productOrdered($filter=not CONTAINS('milk',name) and indexof(Name, 'a') lt 3 or concat(Name, QuantityPerUnit) eq 'bubbles')",
               "ProductOrdered($filter=not contains('milk',Name) and indexof(Name, 'a') lt 3 or concat(Name, QuantityPerUnit) eq 'bubbles')");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void AnyCanonicalFunctions()
        {
            this.ApprovalVerifyExpandParser(orderDetailBase, "AssociatedOrder($filter=OrderDetails/any(a: a/ProductID ge 3.2 and year(a/OrderPlaced) eq 1971 or month(a/OrderPlaced) lt 2 or day(a/OrderPlaced) eq 1 and hour(a/OrderPlaced) ne 3 and minute(a/OrderPlaced) eq 5 and second(a/OrderPlaced) lt 40))");

            this.TestAllInOneExtensionExpand(
               orderDetailBase,
               "AssociatedOrder($filter=OrderDetails/any(a: a/ProDuctID ge 3.2 and year(a/OrderPlaced) eq 1971 or month(a/OrderPlaced) lt 2 or day(a/OrderPlaced) eq 1 and hour(a/orderPlaced) ne 3 and mINute(a/OrderPlaced) eq 5 and second(a/OrderPlaced) lt 40))",
               "AssociatedOrder($filter=OrderDetails/any(a: a/ProductID ge 3.2 and year(a/OrderPlaced) eq 1971 or month(a/OrderPlaced) lt 2 or day(a/OrderPlaced) eq 1 and hour(a/OrderPlaced) ne 3 and minute(a/OrderPlaced) eq 5 and second(a/OrderPlaced) lt 40))");
        }

        /// <summary>
        /// [Uri Parser] UriParser handles primitive collections incorrectly
        /// </summary>
        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void AnyOnCollection()
        {
            this.ApprovalVerifyExpandParser(orderBase, "CustomerForOrder($filter=Numbers/any(a: a eq '4258828080'))");

            this.TestAllInOneExtensionExpand(
               orderBase,
               "CustomerForOrder($filter=Numbers/aNy(a: a eq '4258828080'))",
               "CustomerForOrder($filter=Numbers/any(a: a eq '4258828080'))");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void NullLiteralWithoutConvert()
        {
            this.ApprovalVerifyExpandParser(orderDetailBase, "AssociatedOrder($filter=EmployeeID eq null)");

            this.TestAllInOneExtensionExpand(
               orderDetailBase,
               "associatedOrder($filter=EmployeeID eq null)",
               "AssociatedOrder($filter=EmployeeID eq null)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void NullLiteralWithConvert()
        {
            this.ApprovalVerifyExpandParser(orderDetailBase, "AssociatedOrder($filter=CustomerID eq null)");

            this.TestAllInOneExtensionExpand(
               orderDetailBase,
               "associatedOrder($filter=CustomerID eq null)",
               "AssociatedOrder($filter=CustomerID eq null)");

        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void CompareTwoDateTimeOffsets()
        {
            this.ApprovalVerifyExpandParser(orderDetailBase, "AssociatedOrder($filter=OrderDetails/any(a: a/ProductID ge 3.2 and a/OrderPlaced eq a/OrderPlaced))");

            this.TestAllInOneExtensionExpand(
               orderDetailBase,
               "associatedOrder($filter=orderdetails/any(a: a/Productid ge 3.2 and a/OrderPlaceD eq a/OrderPlaced))",
               "AssociatedOrder($filter=OrderDetails/any(a: a/ProductID ge 3.2 and a/OrderPlaced eq a/OrderPlaced))");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void UnboundFunction()
        {
            this.ApprovalVerifyExpandParser(orderBase, "CustomerForOrder($filter=Microsoft.Test.Taupo.OData.WCFService.HowManyPotatoesEaten() eq 3)");

            this.TestAllInOneExtensionExpand(
               orderBase,
               "CustomerForOrder($filter=howmanypotatoesEaten() eq 3)",
               "CustomerForOrder($filter=Microsoft.Test.Taupo.OData.WCFService.HowManyPotatoesEaten() eq 3)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void BoundFunctionComposed()
        {
            this.ApprovalVerifyExpandParser(customerBase, "Orders($filter=Microsoft.Test.Taupo.OData.WCFService.GetNextOrder()/Microsoft.Test.Taupo.OData.WCFService.GetNextOrder()/Microsoft.Test.Taupo.OData.WCFService.GetNextOrder()/CustomerID eq 3)");

            this.TestAllInOneExtensionExpand(
               customerBase,
               "Orders($filter=getnextorder/getnextorder()/getnextorder/customerid eq 3)",
               "Orders($filter=Microsoft.Test.Taupo.OData.WCFService.GetNextOrder()/Microsoft.Test.Taupo.OData.WCFService.GetNextOrder()/Microsoft.Test.Taupo.OData.WCFService.GetNextOrder()/CustomerID eq 3)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void NestedNots()
        {
            this.ApprovalVerifyExpandParser(orderDetailBase, "AssociatedOrder($filter=OrderDetails/all(a: not (not (not (not (not (not (not (not (not ((not (a/ProductID ge 3.2)) or (not (a/OrderPlaced eq a/OrderPlaced)))))))))))))");

            this.TestAllInOneExtensionExpand(
               orderDetailBase,
               "AssociatedOrder($filter=OrderDetails/all(a: not (not (not (Not (not (not (nOt (noT (not ((not (a/productid gE 3.2)) or (not (a/OrderPlaced eq a/OrderPlaced)))))))))))))",
               "AssociatedOrder($filter=OrderDetails/all(a: not (not (not (not (not (not (not (not (not ((not (a/ProductID ge 3.2)) or (not (a/OrderPlaced eq a/OrderPlaced)))))))))))))");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void  FunctionWithComplexLiteral()
        {
            this.ApprovalVerifyExpandParser(orderBase, "CustomerForOrder($filter=Microsoft.Test.Taupo.OData.WCFService.Within(Location={ Street:'1 Microsoft Way', City:'Redmond', PostalCode:'98052' }, Distance=5))");

            this.TestAllInOneExtensionExpand(
               orderBase,
               "customerfororder($filter=within(LocatioN={ Street:'1 Microsoft Way', City:'Redmond', PostalCode:'98052' }, Distance=5))",
               "CustomerForOrder($filter=Microsoft.Test.Taupo.OData.WCFService.Within(Location={ Street:'1 Microsoft Way', City:'Redmond', PostalCode:'98052' }, Distance=5))");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void FunctionWithMultipleAliasedParams()
        {
            this.ApprovalVerifyExpandParser(orderBase, "CustomerForOrder($filter=Microsoft.Test.Taupo.OData.WCFService.Within(Location=@a, Distance=@b))");

            this.TestAllInOneExtensionExpand(
               orderBase,
               "customerForOrder($filter=within(Location=@a, Distance=@b))",
               "CustomerForOrder($filter=Microsoft.Test.Taupo.OData.WCFService.Within(Location=@a, Distance=@b))");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RedundantlyParserFilter()
        {
            const string queryOption = "AssociatedOrder($filter=CustomerID eq null)";
            ODataUriParser parser = this.CreateExpandUriParser(orderDetailBase, queryOption);
            var result = parser.ParseSelectAndExpand();

            ApprovalVerify(QueryNodeToStringVisitor.GetTestCaseAndResultString(result, null, queryOption));
            result = parser.ParseSelectAndExpand();
            ApprovalVerify(QueryNodeToStringVisitor.GetTestCaseAndResultString(result, null, queryOption));
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void CompareTwoTimeSpans()
        {
            this.ApprovalVerifyExpandParser(orderBase, "CustomerForOrder($filter=TimeBetweenLastTwoOrders lt TimeBetweenLastTwoOrders)");

            this.TestAllInOneExtensionExpand(
               orderBase,
               "CustomerforOrder($filter=TimebetweenLastTwoOrders LT timeBetweenLastTwoOrders)",
               "CustomerForOrder($filter=TimeBetweenLastTwoOrders lt TimeBetweenLastTwoOrders)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ExceptionSholdThrowForInvalidExpression()
        {
            Action action = () => this.ApprovalVerifyExpandParser(orderBase, "CustomerForOrder($filter=Birthday eq 7-dui:9M7UG)");
            action.ShouldThrow<ODataException>().WithMessage("Syntax error at position 14 in 'Birthday eq 7-dui:9M7UG'.");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void MultiNestedExpandWithFilter()
        {
            this.ApprovalVerifyExpandParser(orderDetailBase, "AssociatedOrder($filter=OrderID gt 1941;$expand=LoggedInEmployee($filter=Numbers/any(a: a eq '1942') and year(DateHired) eq 1943))");

            this.TestAllInOneExtensionExpand(
               orderDetailBase,
               "ASSOCIATEDORDER($FILTER=ORDERID GT 1941;$EXPAND=LOGGEDINEMPLOYEE($FILTER=NUMBERS/ANY(a: a EQ '1942') AND YEAR(DateHired) eq 1943))",
               "AssociatedOrder($filter=OrderID gt 1941;$expand=LoggedInEmployee($filter=Numbers/any(a: a eq '1942') and year(DateHired) eq 1943))");
        }
    }
}
