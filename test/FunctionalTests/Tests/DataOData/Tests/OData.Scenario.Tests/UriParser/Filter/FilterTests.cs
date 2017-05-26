//---------------------------------------------------------------------
// <copyright file="FilterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.UriParser.Filter
{
    using System;
    using System.Runtime.CompilerServices;
    using FluentAssertions;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Scenario.Tests.UriParser;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// End user tests for ParseFilter method on the UriParser
    /// </summary>
    [TestClass]
    public class FilterTests : UriParserTestsBase
    {
        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void GtAndLe()
        {
            this.ApprovalVerifyFilterParser(productBase, "UnitPrice le 200 and UnitPrice gt 3.5");

            this.TestAllInOneExtensionFilter(
               productBase,
               "UnitPrice LE 200 and UNITPRICE gt 3.5",
               "UnitPrice le 200 and UnitPrice gt 3.5");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void DoesNotResolveToBool()
        {
            var parser = this.CreateFilterUriParser(productBase, "UnitPrice mul 200 add UnitPrice div 4");
            try
            {
                parser.ParseFilter();
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
            this.ApprovalVerifyFilterParser(employeeBase, "geo.distance(Home, Office) lt 0.5");
            
            this.TestAllInOneExtensionFilter(
               employeeBase,
               "GEO.distance(Home, Office) lt 0.5",
               "geo.distance(Home, Office) lt 0.5");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void SpatialLength()
        {
            this.ApprovalVerifyFilterParser(employeeBase, "geo.length(geometry'LineString(10 30, 7 28, 6 6)') lt 0.5");

            this.TestAllInOneExtensionFilter(
               employeeBase,
               "geo.LENGTH(geometry'LineString(10 30, 7 28, 6 6)') lt 0.5",
               "geo.length(geometry'LineString(10 30, 7 28, 6 6)') lt 0.5");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void SpatialIntersects()
        {
            this.ApprovalVerifyFilterParser(employeeBase, "geo.intersects(geometry'Point(10 30)', geometry'Polygon((5 16, 5 30, 20 30, 20 16, 5 16))')");

            this.TestAllInOneExtensionFilter(
               employeeBase,
               "geo.intersecTs(geometry'Point(10 30)', geometry'Polygon((5 16, 5 30, 20 30, 20 16, 5 16))')",
               "geo.intersects(geometry'Point(10 30)', geometry'Polygon((5 16, 5 30, 20 30, 20 16, 5 16))')");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void NotEndsWithAndBeginsWith()
        {
            this.ApprovalVerifyFilterParser(productBase, "not endswith(Name,'milk') and startswith(Name, 'word') or length(QuantityPerUnit) eq 15");

            this.TestAllInOneExtensionFilter(
               productBase,
               "not enDswith(Name,'milk') and starTswith(Name, 'word') or leNgth(QuantityPerUnit) EQ 15",
               "not endswith(Name,'milk') and startswith(Name, 'word') or length(QuantityPerUnit) eq 15");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundFloorCeiling()
        {
            this.ApprovalVerifyFilterParser(productBase, "round(UnitPrice) eq floor(UnitPrice) and round(UnitPrice) ne ceiling(UnitPrice)");

            this.TestAllInOneExtensionFilter(
               productBase,
               "rouNd(UnitPrice) EQ floOr(UnitPrice) and rOUnd(UnitPrice) ne ceiLing(UnitPrice)",
               "round(UnitPrice) eq floor(UnitPrice) and round(UnitPrice) ne ceiling(UnitPrice)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void Replace()
        {
            this.ApprovalVerifyFilterParser(customerBase, "replace(FirstName, 'd', 'n') eq 'bran'");

            this.TestAllInOneExtensionFilter(
               customerBase,
               "replAce(FirSTName, 'd', 'n') eQ 'bran'",
               "replace(FirstName, 'd', 'n') eq 'bran'");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ContainsIndexOf()
        {
            this.ApprovalVerifyFilterParser(productBase, "not contains('milk',Name) and indexof(Name, 'a') lt 3 or concat(Name, QuantityPerUnit) eq 'bubbles'");

            this.TestAllInOneExtensionFilter(
               productBase,
               "NOT coNtains('milk',NAme) and indexof(Name, 'a') lt 3 or concat(Name, QuaNtityPerUnit) EQ 'bubbles'",
               "not contains('milk',Name) and indexof(Name, 'a') lt 3 or concat(Name, QuantityPerUnit) eq 'bubbles'");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void AnyCanonicalFunctions()
        {
            this.ApprovalVerifyFilterParser(orderBase, "OrderDetails/any(a: a/ProductID ge 3.2 and year(a/OrderPlaced) eq 1971 or month(a/OrderPlaced) lt 2 or day(a/OrderPlaced) eq 1 and hour(a/OrderPlaced) ne 3 and minute(a/OrderPlaced) eq 5 and second(a/OrderPlaced) lt 40)");

            this.TestAllInOneExtensionFilter(
               orderBase,
               "orderdetails/aNy(a: a/ProductID ge 3.2 and year(a/OrderPlaced) eQ 1971 or month(a/OrderPlaced) lt 2 or day(a/OrderPlaced) eq 1 and hour(a/OrderPlaced) ne 3 and minutE(a/OrdeRPlaced) eq 5 and second(a/OrderPlaced) lt 40)",
               "OrderDetails/any(a: a/ProductID ge 3.2 and year(a/OrderPlaced) eq 1971 or month(a/OrderPlaced) lt 2 or day(a/OrderPlaced) eq 1 and hour(a/OrderPlaced) ne 3 and minute(a/OrderPlaced) eq 5 and second(a/OrderPlaced) lt 40)");
        }

        /// <summary>
        /// [Uri Parser] UriParser handles primitive collections incorrectly
        /// </summary>
        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void AnyOnCollection()
        {
            this.ApprovalVerifyFilterParser(peopleBase, "Numbers/any(a: a eq '4258828080')");

            this.TestAllInOneExtensionFilter(
               peopleBase,
               "Numbers/aNy(a: a eQ '4258828080')",
               "Numbers/any(a: a eq '4258828080')");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void AnyOnOpenCollection()
        {
            this.ApprovalVerifyFilterParser(employeeBase, "OpenNumbers/any(a: a eq '4258828080')");

            this.TestAllInOneExtensionFilter(
               employeeBase,
               "OpenNumbers/anY(a: a EQ '4258828080')",
               "OpenNumbers/any(a: a eq '4258828080')");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void NullLiteralWithoutConvert()
        {
            this.ApprovalVerifyFilterParser(orderBase, "EmployeeID eq null");

            this.TestAllInOneExtensionFilter(
               orderBase,
               "EmployEEID EQ null",
               "EmployeeID eq null");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void NullLiteralWithConvert()
        {
            this.ApprovalVerifyFilterParser(orderBase, "CustomerID eq null");

            this.TestAllInOneExtensionFilter(
               orderBase,
               "CustOMerID eQ null",
               "CustomerID eq null");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void SpatialTypeEqNull()
        {
            this.ApprovalVerifyFilterParser(peopleBase, "Home eq null");

            this.TestAllInOneExtensionFilter(
               peopleBase,
               "home EQ null",
               "Home eq null");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void SpatialTypeNeNull()
        {
            this.ApprovalVerifyFilterParser(peopleBase, "Home ne null");

            this.TestAllInOneExtensionFilter(
               peopleBase,
               "home Ne null",
               "Home ne null");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void CompareTwoDateTimeOffsets()
        {
            this.ApprovalVerifyFilterParser(orderBase, "OrderDetails/any(a: a/ProductID ge 3.2 and a/OrderPlaced eq a/OrderPlaced)");

            this.TestAllInOneExtensionFilter(
               orderBase,
               "orderDetails/any(a: a/productID ge 3.2 and a/OrderPlaced eq a/OrderPlaced)",
               "OrderDetails/any(a: a/ProductID ge 3.2 and a/OrderPlaced eq a/OrderPlaced)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void UnboundFunction()
        {
            this.ApprovalVerifyFilterParser(peopleBase, "Microsoft.Test.Taupo.OData.WCFService.HowManyPotatoesEaten() eq 3");

            this.TestAllInOneExtensionFilter(
                peopleBase,
                "howManyPotatoesEaten() eQ 3",
                "Microsoft.Test.Taupo.OData.WCFService.HowManyPotatoesEaten() eq 3");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void BoundFunctionComposed()
        {
            this.ApprovalVerifyFilterParser(orderBase, "Microsoft.Test.Taupo.OData.WCFService.GetNextOrder/Microsoft.Test.Taupo.OData.WCFService.GetNextOrder/Microsoft.Test.Taupo.OData.WCFService.GetNextOrder/CustomerID eq 3");

            this.TestAllInOneExtensionFilter(
                orderBase,
                "getNextOrder/getNextOrder/Microsoft.Test.Taupo.OData.WCFService.GetNexTOrder/customeriD eq 3",
                "Microsoft.Test.Taupo.OData.WCFService.GetNextOrder/Microsoft.Test.Taupo.OData.WCFService.GetNextOrder/Microsoft.Test.Taupo.OData.WCFService.GetNextOrder/CustomerID eq 3");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void NestedNots()
        {
            this.ApprovalVerifyFilterParser(orderBase, "OrderDetails/all(a: not (not (not (not (not (not (not (not (not ((not (a/ProductID ge 3.2)) or (not (a/OrderPlaced eq a/OrderPlaced))))))))))))");

            this.TestAllInOneExtensionFilter(
                orderBase,
                "OrderDetails/aLl(a: not (noT (not (not (not (not (not (not (not ((not (a/Productid ge 3.2)) or (not (a/OrderPlaced eq a/OrderPlaced))))))))))))",
                "OrderDetails/all(a: not (not (not (not (not (not (not (not (not ((not (a/ProductID ge 3.2)) or (not (a/OrderPlaced eq a/OrderPlaced))))))))))))");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void FunctionWithComplexLiteral()
        {
            this.ApprovalVerifyFilterParser(customerBase, "Microsoft.Test.Taupo.OData.WCFService.Within(Location={ Street:'1 Microsoft Way', City:'Redmond', PostalCode:'98052' }, Distance=5)");

            this.TestAllInOneExtensionFilter(
                customerBase,
                "WIthin(Location={ Street:'1 Microsoft Way', City:'Redmond', PostalCode:'98052' }, DistanCe=5)",
                "Microsoft.Test.Taupo.OData.WCFService.Within(Location={ Street:'1 Microsoft Way', City:'Redmond', PostalCode:'98052' }, Distance=5)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void FunctionWithMultipleAliasedParams()
        {
            this.ApprovalVerifyFilterParser(customerBase, "Microsoft.Test.Taupo.OData.WCFService.Within(Location=@a, Distance=@b)");

            this.TestAllInOneExtensionFilter(
                customerBase,
                "within(location=@a, distance=@b)",
                "Microsoft.Test.Taupo.OData.WCFService.Within(Location=@a, Distance=@b)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RedundantlyParserFilter()
        {
            const string queryOption = "CustomerID eq null";
            ODataUriParser parser = this.CreateFilterUriParser(orderBase, queryOption);
            var result = parser.ParseFilter();
            ApprovalVerify(QueryNodeToStringVisitor.GetTestCaseAndResultString(result, queryOption));
            result = parser.ParseFilter();
            ApprovalVerify(QueryNodeToStringVisitor.GetTestCaseAndResultString(result, queryOption));

            this.TestAllInOneExtensionFilter(
                orderBase,
                "customerid eQ null",
                "CustomerID eq null");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void CompareTwoTimeSpans()
        {
            this.ApprovalVerifyFilterParser(customerBase, "TimeBetweenLastTwoOrders lt TimeBetweenLastTwoOrders");

            this.TestAllInOneExtensionFilter(
                customerBase,
                "timeBetweenLasttwoOrders lT TimeBetweenLasttwoOrders",
                "TimeBetweenLastTwoOrders lt TimeBetweenLastTwoOrders");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void NonEntityCollection()
        {
            this.ApprovalVerifyFilterParser(numbersBase, "contains($it,'200')");

            this.TestAllInOneExtensionFilter(
                numbersBase,
                "conTaIns($it,'200')",
                "contains($it,'200')");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ExceptionSholdThrowForInvalidExpression()
        {
            Action action = () => this.ApprovalVerifyFilterParser(customerBase, "Birthday eq 7-dui:9M7UG{*'!pu:^8LaV8a9~Pt76Fn*sP*1Tdf");
            action.ShouldThrow<ODataException>().WithMessage("Syntax error at position 14 in 'Birthday eq 7-dui:9M7UG{*'!pu:^8LaV8a9~Pt76Fn*sP*1Tdf'.");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ExceptionSholdThrowForFunctionImport()
        {
            Action action = () => this.ApprovalVerifyFilterParser(orderBase, "HasLotsOfOrders() eq 3");
            action.ShouldThrow<ODataException>().WithMessage("An unknown function with name 'HasLotsOfOrders' was found. This may also be a function import or a key lookup on a navigation property, which is not allowed.");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void EntityCastShouldWork()
        {
            this.ApprovalVerifyFilterParser(peopleBase, "Child/Microsoft.Test.Taupo.OData.WCFService.Customer/City ne 'Shanghai'");

            this.TestAllInOneExtensionFilter(
                peopleBase,
                "ChiLd/Microsoft.Test.Taupo.OData.WCFService.CusTomer/city Ne 'Shanghai'",
                "Child/Microsoft.Test.Taupo.OData.WCFService.Customer/City ne 'Shanghai'");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ComplexCastShouldWork()
        {
            this.ApprovalVerifyFilterParser(peopleBase, "HomeAddress/Microsoft.Test.Taupo.OData.WCFService.HomeAddress/HomeNO ne '999'");

            this.TestAllInOneExtensionFilter(
                peopleBase,
                "HomeAddress/MicrOsoft.Test.Taupo.OData.WCFService.HomeAddress/HomeNO ne '999'",
                "HomeAddress/Microsoft.Test.Taupo.OData.WCFService.HomeAddress/HomeNO ne '999'");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void EntityCollectionCastShouldWork()
        {
            this.ApprovalVerifyFilterParser(peopleBase, "Brother/Microsoft.Test.Taupo.OData.WCFService.Customer/any(d:d/City ne 'Shanghai')");

            this.TestAllInOneExtensionFilter(
                peopleBase,
                "Brother/Microsoft.Test.TauPo.OData.WCFService.Customer/any(d:d/City NE 'Shanghai')",
                "Brother/Microsoft.Test.Taupo.OData.WCFService.Customer/any(d:d/City ne 'Shanghai')");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ComplexCollectionCastShouldWork()
        {
            this.ApprovalVerifyFilterParser(productBase, "ManufactureAddresss/Microsoft.Test.Taupo.OData.WCFService.HomeAddress/any(d:d/HomeNO ne '999')");

            this.TestAllInOneExtensionFilter(
                productBase,
                "manufactureAddresss/Microsoft.Test.Taupo.OData.wcfService.HomeAddress/any(d:d/HomeNo ne '999')",
                "ManufactureAddresss/Microsoft.Test.Taupo.OData.WCFService.HomeAddress/any(d:d/HomeNO ne '999')");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void EntityCollectionCastInLambdaShouldWork()
        {
            this.ApprovalVerifyFilterParser(peopleBase, "Brother/any(d:d/Microsoft.Test.Taupo.OData.WCFService.Customer/City ne 'Shanghai')");

            this.TestAllInOneExtensionFilter(
               peopleBase,
               "brother/any(d:d/Microsoft.Test.Taupo.OData.WCFService.CUStomer/City ne 'Shanghai')",
               "Brother/any(d:d/Microsoft.Test.Taupo.OData.WCFService.Customer/City ne 'Shanghai')");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ComplexCollectionCastInLambdaShouldWork()
        {
            this.ApprovalVerifyFilterParser(productBase, "ManufactureAddresss/any(d:d/Microsoft.Test.Taupo.OData.WCFService.HomeAddress/HomeNO ne '999')");

            this.TestAllInOneExtensionFilter(
                productBase,
                "ManufactureAddresss/aNY(d:d/Microsoft.Test.Taupo.ODaTA.WCFService.HomeAddress/HoMeNO nE '999')",
                "ManufactureAddresss/any(d:d/Microsoft.Test.Taupo.OData.WCFService.HomeAddress/HomeNO ne '999')");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void BuiltinDateTimeFunctionFractionalsecondsAndTotaloffsetminutes()
        {
            this.ApprovalVerifyFilterParser(orderBase, "fractionalseconds(OrderDate) lt 0.1 and totaloffsetminutes(OrderDate) gt 5.1");

            this.TestAllInOneExtensionFilter(
                orderBase,
                "fractionalseconds(OrderDate) lT 0.1 aNd totaloffsetminutes(OrdeRDate) gt 5.1",
                "fractionalseconds(OrderDate) lt 0.1 and totaloffsetminutes(OrderDate) gt 5.1");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void BuiltinDateTimeFunctionTotalseconds()
        {
            this.ApprovalVerifyFilterParser(durationInKeysBase, "totalseconds(Id) eq 9.8");

            this.TestAllInOneExtensionFilter(
                durationInKeysBase,
                "totalsecOnds(id) eQ 9.8",
                "totalseconds(Id) eq 9.8");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void BuiltinDateTimeFunctionNowAndMaxdatetimeAndMindatetime()
        {
            this.ApprovalVerifyFilterParser(durationInKeysBase, "now() le maxdatetime() and now() gt mindatetime()");

            this.TestAllInOneExtensionFilter(
                durationInKeysBase,
                "NOW() le maxdatetime() AND now() gT mindatetime()",
                "now() le maxdatetime() and now() gt mindatetime()");
        }
    }
}
