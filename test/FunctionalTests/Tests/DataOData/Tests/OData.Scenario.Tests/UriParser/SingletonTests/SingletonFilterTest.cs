//---------------------------------------------------------------------
// <copyright file="SingletonFilterTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.UriParser
{
    using System.Runtime.CompilerServices;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SingletonFilterTest : UriParserTestsBase
    {
        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void GtAndLe()
        {
            this.ApprovalVerifyFilterParser(specialProductBase, "UnitPrice le 200 and UnitPrice gt 3.5");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void DoesNotResolveToBool()
        {
            var parser = this.CreateFilterUriParser(specialProductBase, "UnitPrice mul 200 add UnitPrice div 4");
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
            this.ApprovalVerifyFilterParser(bossBase, "geo.distance(Home, Office) lt 0.5");
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void SpatialLength()
        {
            this.ApprovalVerifyFilterParser(bossBase, "geo.length(geometry'LineString(10 30, 7 28, 6 6)') lt 0.5");
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void SpatialIntersects()
        {
            this.ApprovalVerifyFilterParser(bossBase, "geo.intersects(geometry'Point(10 30)', geometry'Polygon((5 16, 5 30, 20 30, 20 16, 5 16))')");
        }
        

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void NotEndsWithAndBeginsWith()
        {
            this.ApprovalVerifyFilterParser(specialProductBase, "not endswith(Name,'milk') and startswith(Name, 'word') or length(QuantityPerUnit) eq 15");
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundFloorCeiling()
        {
            this.ApprovalVerifyFilterParser(specialProductBase, "round(UnitPrice) eq floor(UnitPrice) and round(UnitPrice) ne ceiling(UnitPrice)");
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void Replace()
        {
            this.ApprovalVerifyFilterParser(customerBase, "replace(FirstName, 'd', 'n') eq 'bran'");
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ContainsIndexOf()
        {
            this.ApprovalVerifyFilterParser(specialProductBase, "not contains('milk',Name) and indexof(Name, 'a') lt 3 or concat(Name, QuantityPerUnit) eq 'bubbles'");
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void AnyCanonicalFunctions()
        {
            this.ApprovalVerifyFilterParser(specialOrderBase, "OrderDetails/any(a: a/ProductID ge 3.2 and year(a/OrderPlaced) eq 1971 or month(a/OrderPlaced) lt 2 or day(a/OrderPlaced) eq 1 and hour(a/OrderPlaced) ne 3 and minute(a/OrderPlaced) eq 5 and second(a/OrderPlaced) lt 40)");
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void AnyOnCollection()
        {
            this.ApprovalVerifyFilterParser(bossBase, "Numbers/any(a: a eq '4258828080')");
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void NullLiteralWithoutConvert()
        {
            this.ApprovalVerifyFilterParser(specialOrderBase, "EmployeeID eq null");
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void NullLiteralWithConvert()
        {
            this.ApprovalVerifyFilterParser(specialOrderBase, "CustomerID eq null");
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void CompareTwoDateTimeOffsets()
        {
            this.ApprovalVerifyFilterParser(specialOrderBase, "OrderDetails/any(a: a/ProductID ge 3.2 and a/OrderPlaced eq a/OrderPlaced)");
        }
    }
}
