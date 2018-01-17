//---------------------------------------------------------------------
// <copyright file="OrderByTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.UriParser.OrderBy
{
    using System;
    using System.Runtime.CompilerServices;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// End user tests for ParseOrderBy method on the UriParser
    /// </summary>
    [TestClass]
    public class OrderByTests : UriParserTestsBase
    {
        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void Property()
        {
            this.ApprovalVerifyOrderByParser(customerBase, "FirstName");

            this.TestAllInOneExtensionOrderBy(
              customerBase,
              "FirstNamE",
              "FirstName");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PropertyDesc()
        {
            this.ApprovalVerifyOrderByParser(customerBase, "FirstName desc");

            this.TestAllInOneExtensionOrderBy(
              customerBase,
              "FirstName dEsc",
              "FirstName desc");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void MultipleProperties()
        {
            this.ApprovalVerifyOrderByParser(customerBase, "FirstName desc, LastName, City asc, Birthday desc");

            this.TestAllInOneExtensionOrderBy(
              customerBase,
              "FIRSTNAME DESC, LASTNAME, CITY ASC, BIRTHDAY DESC",
              "FirstName desc, LastName, City asc, Birthday desc");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void NavigationProperty()
        {
            this.ApprovalVerifyOrderByParser(orderBase, "CustomerForOrder/FirstName desc");

            this.TestAllInOneExtensionOrderBy(
               orderBase,
               "customerfororder/firstname DESC",
               "CustomerForOrder/FirstName desc");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void NestedNots()
        {
            this.ApprovalVerifyOrderByParser(orderBase, "OrderDetails/all(a: not (not (not (not (not (not (not (not (not ((not (a/ProductID ge 3.2)) or (not (a/OrderPlaced eq a/OrderPlaced))))))))))))");

            this.TestAllInOneExtensionOrderBy(
               orderBase,
               "OrderDetails/aLl(a: not (not (not (not (not (not (not (not (nOT ((not (a/PRODUCTID ge 3.2)) or (not (a/OrderPlaCed EQ a/OrderPlaced))))))))))))",
               "OrderDetails/all(a: not (not (not (not (not (not (not (not (not ((not (a/ProductID ge 3.2)) or (not (a/OrderPlaced eq a/OrderPlaced))))))))))))");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void OrderByHoursMinutesSeconds()
        {
            this.ApprovalVerifyOrderByParser(customerBase, "hour(TimeBetweenLastTwoOrders) mul 3 sub minute(TimeBetweenLastTwoOrders) div 2 add second(TimeBetweenLastTwoOrders) mod 30");

            this.TestAllInOneExtensionOrderBy(
               customerBase,
               "hour(TimeBetweenLastTwoOrders) mul 3 sub minUte(TimeBetweenLastTwoOrders) div 2 add second(TimeBetweenLastTwoOrders) mOD 30",
               "hour(TimeBetweenLastTwoOrders) mul 3 sub minute(TimeBetweenLastTwoOrders) div 2 add second(TimeBetweenLastTwoOrders) mod 30");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void Concat()
        {
            this.ApprovalVerifyOrderByParser(customerBase, "endswith(substring(toupper(concat(FirstName, LastName)), 7), 'r') and contains('H','Hello')");

            this.TestAllInOneExtensionOrderBy(
               customerBase,
               "endswith(SUBSTRING(toupper(coNcat(FirStName, LastName)), 7), 'r') and conTAins('H','Hello')",
               "endswith(substring(toupper(concat(FirstName, LastName)), 7), 'r') and contains('H','Hello')");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void IsofCast()
        {
            this.ApprovalVerifyOrderByParser(peopleBase, "isof('Microsoft.Test.Taupo.OData.WCFService.Customer') and cast('Microsoft.Test.Taupo.OData.WCFService.Customer') ne null");

            this.TestAllInOneExtensionOrderBy(
               peopleBase,
               "ISOF('Microsoft.Test.Taupo.OData.WCFService.Customer') and caSt('Microsoft.Test.Taupo.OData.WCFService.Customer') ne null",
               "isof('Microsoft.Test.Taupo.OData.WCFService.Customer') and cast('Microsoft.Test.Taupo.OData.WCFService.Customer') ne null");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void NonEntityCollection()
        {
            this.ApprovalVerifyOrderByParser(numbersBase, "$it");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void SpatialLengthNotLineString()
        {
            ODataUriParser parser = this.CreateOrderByUriParser(employeeBase, "geo.length(geometry'Polygon((10 30, 7 28, 6 6, 10 30))') lt 0.5");
            try
            {
                parser.ParseOrderBy();
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
            ODataUriParser parser = this.CreateOrderByUriParser(employeeBase, "geo.length(geometry'Polygon(10 30, 7 28, 6 6, 10 30)') lt 0.5");
            try
            {
                parser.ParseOrderBy();
                Assert.Fail("Should throw with ODataException");
            }
            catch (ODataException e)
            {
                var expected = ODataExpectedExceptions.ODataException("UriQueryExpressionParser_UnrecognizedLiteralWithReason", "Edm.Geometry", "geometry'Polygon(10 30, 7 28, 6 6, 10 30)'", "11", "geo.length(geometry'Polygon(10 30, 7 28, 6 6, 10 30)') lt 0.5", "Expecting token type \"LeftParen\" with text \"\" but found \"Type:[2] Text:[10]\".");
                expected.ExpectedMessage.Verifier.VerifyMatch("UriQueryExpressionParser_UnrecognizedLiteralWithReason", e.Message, "Edm.Geometry", "geometry'Polygon(10 30, 7 28, 6 6, 10 30)'", "11", "geo.length(geometry'Polygon(10 30, 7 28, 6 6, 10 30)') lt 0.5", "Expecting token type \"LeftParen\" with text \"\" but found \"Type:[2] Text:[10]\".");
            }
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void EntityCastShouldWork()
        {
            this.ApprovalVerifyOrderByParser(peopleBase, "Child/Microsoft.Test.Taupo.OData.WCFService.Customer/City");

            this.TestAllInOneExtensionOrderBy(
                peopleBase,
                "ChIld/Microsoft.Test.taupo.OData.WCFService.Customer/city",
                "Child/Microsoft.Test.Taupo.OData.WCFService.Customer/City");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ComplexCastShouldWork()
        {
            this.ApprovalVerifyOrderByParser(peopleBase, "HomeAddress/Microsoft.Test.Taupo.OData.WCFService.HomeAddress/HomeNO");

            this.TestAllInOneExtensionOrderBy(
                peopleBase,
                "homeaddress/Microsoft.Test.Taupo.OData.WCFService.homeaddress/Homeno",
                "HomeAddress/Microsoft.Test.Taupo.OData.WCFService.HomeAddress/HomeNO");
        }


        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void TemporalValueAddition()
        {
            this.ApprovalVerifyOrderByParser(durationInKeysBase, "now() add (Id add duration'PT130S')");

            this.TestAllInOneExtensionOrderBy(
                durationInKeysBase,
                "NOW() ADD (iD aDd duration'PT130S')",
                "now() add (Id add duration'PT130S')");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void TemporalValueSubtraction()
        {
            this.ApprovalVerifyOrderByParser(durationInKeysBase, "now() sub Id sub mindatetime() sub duration'PT130S'");

            this.TestAllInOneExtensionOrderBy(
                durationInKeysBase,
                "nOw() sub id sub mindatetime() SUB duration'PT130S'",
                "now() sub Id sub mindatetime() sub duration'PT130S'");
        }
    }
}
