//---------------------------------------------------------------------
// <copyright file="SingletonOrderByTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.UriParser.OrderBy
{
    using System.Runtime.CompilerServices;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SingletonOrderByTest : UriParserTestsBase
    {
        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void Property()
        {
            this.ApprovalVerifyOrderByParser(vipCustomerBase, "FirstName");
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PropertyDesc()
        {
            this.ApprovalVerifyOrderByParser(vipCustomerBase, "FirstName desc");
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void MultipleProperties()
        {
            this.ApprovalVerifyOrderByParser(vipCustomerBase, "FirstName desc, LastName, City asc, Birthday desc");
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void NavigationProperty()
        {
            this.ApprovalVerifyOrderByParser(specialOrderBase, "CustomerForOrder/FirstName desc");
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void NestedNots()
        {
            this.ApprovalVerifyOrderByParser(specialOrderBase, "OrderDetails/all(a: not (not (not (not (not (not (not (not (not ((not (a/ProductID ge 3.2)) or (not (a/OrderPlaced eq a/OrderPlaced))))))))))))");
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void Concat()
        {
            this.ApprovalVerifyOrderByParser(vipCustomerBase, "endswith(substring(toupper(concat(FirstName, LastName)), 7), 'r') and contains('H','Hello')");
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void IsofCast()
        {
            this.ApprovalVerifyOrderByParser(specialPersonBase, "isof('Microsoft.Test.Taupo.OData.WCFService.Customer') and cast('Microsoft.Test.Taupo.OData.WCFService.Customer') ne null");
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void SpatialLengthNotLineString()
        {
            ODataUriParser parser = this.CreateOrderByUriParser(bossBase, "geo.length(geometry'Polygon((10 30, 7 28, 6 6, 10 30))') lt 0.5");
            try
            {
                parser.ParseOrderBy();
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
            ODataUriParser parser = this.CreateOrderByUriParser(bossBase, "geo.length(geometry'Polygon(10 30, 7 28, 6 6, 10 30)') lt 0.5");
            try
            {
                parser.ParseOrderBy();
            }
            catch (ODataException e)
            {
                var expected = ODataExpectedExceptions.ODataException("UriQueryExpressionParser_UnrecognizedLiteralWithReason", "Edm.Geometry", "geometry'Polygon(10 30, 7 28, 6 6, 10 30)'", "11", "geo.length(geometry'Polygon(10 30, 7 28, 6 6, 10 30)') lt 0.5", "Expecting token type \"LeftParen\" with text \"\" but found \"Type:[2] Text:[10]\".");
                expected.ExpectedMessage.Verifier.VerifyMatch("UriQueryExpressionParser_UnrecognizedLiteralWithReason", e.Message, "Edm.Geometry", "geometry'Polygon(10 30, 7 28, 6 6, 10 30)'", "11", "geo.length(geometry'Polygon(10 30, 7 28, 6 6, 10 30)') lt 0.5", "Expecting token type \"LeftParen\" with text \"\" but found \"Type:[2] Text:[10]\".");
            }
        }
    }
}
