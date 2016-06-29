//---------------------------------------------------------------------
// <copyright file="PathTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.UriParser.Path
{
    using System;
    using System.Runtime.CompilerServices;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PathTests : UriParserTestsBase
    {
        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathBaseEntitySet()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/Orders"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));

            this.TestExtensions("OrDERS");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathEntity()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/Orders(2)"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));

            this.TestExtensions("OrDERS(2)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathNavigation()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/Customers(5)/Orders"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));

            this.TestExtensions("CustoMERS(5)/OrDErs");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathComplex()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/Customers(-32)/HomeAddress"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));

            this.TestExtensions("CustomErs(-32)/HOMEAddress");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathMetadata()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/$metadata"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));

            this.TestExtensions("$metaData");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathBatch()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/$batch"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));

            this.TestExtensions("$BATch");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathPrimitive()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/Customers(0)/FirstName"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));

            this.TestExtensions("CustomERS(0)/FiRStName");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathPrimitiveValue()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/Customers(2350)/FirstName/$value"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));

            this.TestExtensions("customers(2350)/FIRSTNAME/$vALUe");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathLinks()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/Customers(-21)/Orders/$ref"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));

            this.TestExtensions("CusTomers(-21)/OrDers/$REF");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathFunction()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/Orders/Microsoft.Test.Taupo.OData.WCFService.OrdersWithMoreThanTwoItems"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));

            this.TestExtensions("Orders/Microsoft.Test.Taupo.OData.WCFService.OrdersWithMoreThanTwoitems",
                "Orders/OrdersWithMoreThanTwoItems",
                "Orders/OrdersWithMoreThanTwoITEMS");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathFunctionOnEntity()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/Orders(1)/Microsoft.Test.Taupo.OData.WCFService.GetOrderRate"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));

            this.TestExtensions("Orders(1)/Microsoft.Test.Taupo.OData.WCFService.GetOrderRatE",
                "Orders(1)/GetOrderRate",
                "ORDErs(1)/GETORDERrate");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathFunctionInContainedEntity()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/People(1)/FirstOrder/Microsoft.Test.Taupo.OData.WCFService.GetOrderRate"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));

            this.TestExtensions("People(1)/FirstORDER/Microsoft.TESt.Taupo.ODaTa.WCFService.GetOrderRate",
                "People(1)/FirstOrder/GetOrderRate",
                "PeopLE(1)/FirstOrder/getorderrate");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathFunctionReturnsContainedEntity()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/People(1)/Microsoft.Test.Taupo.OData.WCFService.GetChild"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));
            OperationSegment operationSegment = result.LastSegment as OperationSegment;
            Assert.IsNotNull(operationSegment);
            Assert.IsTrue(operationSegment.EntitySet is IEdmContainedEntitySet);

            this.TestExtensions("PeopLE(1)/Microsoft.Test.Taupo.OData.wcfservice.GETCHILD",
                "People(1)/GetChild",
                "PEOPLE(1)/getchild");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathActionReturnsContainedEntity()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/People(1)/Microsoft.Test.Taupo.OData.WCFService.GetBrothers"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));
            OperationSegment operationSegment = result.LastSegment as OperationSegment;
            Assert.IsNotNull(operationSegment);
            Assert.IsTrue(operationSegment.EntitySet is IEdmContainedEntitySet);

            this.TestExtensions("people(1)/microsoft.Test.Taupo.OData.WCFService.GetBrothers",
                "People(1)/GetBrothers",
                "people(1)/getbrothers");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathFunctionWithParameter()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/Customers/Microsoft.Test.Taupo.OData.WCFService.InCity(City='Redmond')"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));

            this.TestExtensions("Customers/Microsoft.TEST.Taupo.OData.WCFService.INCITY(CITY='Redmond')",
                "Customers/InCity(City='Redmond')",
                "CustomerS/incity(city='Redmond')");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathFunctionWithAliasedParameter()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/Customers/Microsoft.Test.Taupo.OData.WCFService.InCity(City=@a)"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));

            this.TestExtensions("Customers/Microsoft.TESt.Taupo.OData.WCFService.InCity(CiTy=@a)",
                "Customers/InCity(City=@a)",
                "Customers/inCity(CiTy=@a)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathFunctionAliasedParameterWithResolver()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/Customers/Microsoft.Test.Taupo.OData.WCFService.InCity(City=@a)?@a='Redmond'"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));

            this.TestExtensions("customers/Microsoft.Test.Taupo.OData.wcfservice.InCity(city=@a)?@a='Redmond'",
                "Customers/InCity(City=@a)?@a='Redmond'",
                "customers/incity(CITY=@a)?@a='Redmond'");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathFunctionWithComplexParameters()
        {
            string inputstring = "http://www.potato.com/Customers/Microsoft.Test.Taupo.OData.WCFService.Within(Address={ Street:'1 Microsoft Way', City:'Redmond', PostalCode:'98052' }, " +
                "Distance=@a, Location=geography'Point(10 30 15 6)', ArbitraryInt=@b, DateTimeOffset=null, Byte=@c, LineString=@d)/" +
                "Microsoft.Test.Taupo.OData.WCFService.Within(Address=null, " +
                "Distance=5.04, ArbitraryInt=24555, DateTimeOffset=null, Byte=@f, Location=@e, LineString=@potato)";
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri(inputstring));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));

            this.TestExtensions(
                "customers/Microsoft.Test.Taupo.OData.WCFService.Within(Address={ Street:'1 Microsoft Way', City:'Redmond', PostalCode:'98052' }, " +
                "Distance=@a, LocAtion=geography'Point(10 30 15 6)', ArbitraryInt=@b, DateTimeOffset=null, Byte=@c, LineString=@d)/" +
                "Microsoft.Test.Taupo.OData.WCFService.Within(Address=null, " +
                "Distance=5.04, ArbitraryInt=24555, DateTimeOffset=null, Byte=@f, Location=@e, LineString=@potato)",
                "Customers/Within(Address={ Street:'1 Microsoft Way', City:'Redmond', PostalCode:'98052' }, " +
                "Distance=@a, Location=geography'Point(10 30 15 6)', ArbitraryInt=@b, DateTimeOffset=null, Byte=@c, LineString=@d)/" +
                "Microsoft.Test.Taupo.OData.WCFService.Within(Address=null, " +
                "Distance=5.04, ArbitraryInt=24555, DateTimeOffset=null, Byte=@f, Location=@e, LineString=@potato)",
                "customers/within(Address={ Street:'1 Microsoft Way', City:'Redmond', PostalCode:'98052' }, " +
                "Distance=@a, LocAtion=geography'Point(10 30 15 6)', ArbitraryInt=@b, DateTimeOffset=null, Byte=@c, LineString=@d)/" +
                "Within(Address=null, " +
                "Distance=5.04, ArbitraryInt=24555, DATETimeOffset=null, Byte=@f, Location=@e, LineString=@potato)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathFunctionWithParens()
        {
            ODataUriParser parserWithoutparens = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/Orders/Microsoft.Test.Taupo.OData.WCFService.OrdersWithMoreThanTwoItems"));
            ODataUriParser parserWithparens = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/Orders/Microsoft.Test.Taupo.OData.WCFService.OrdersWithMoreThanTwoItems()"));
            Assert.AreEqual(QueryNodeToStringVisitor.ToString(parserWithoutparens.ParsePath()), QueryNodeToStringVisitor.ToString(parserWithparens.ParsePath()));
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathAction()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/People(1)/Microsoft.Test.Taupo.OData.WCFService.ChangeAddress"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));

            this.TestExtensions("PEOPLE(1)/Microsoft.Test.Taupo.ODATA.WCFService.ChangeAddress",
                "People(1)/ChangeAddress",
                "people(1)/changeaddress");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathActionWithParens()
        {
            ODataUriParser parserWithoutparens = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/People(1)/Microsoft.Test.Taupo.OData.WCFService.ChangeAddress"));
            ODataUriParser parserWithparens = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/People(1)/Microsoft.Test.Taupo.OData.WCFService.ChangeAddress()"));
            Assert.AreEqual(QueryNodeToStringVisitor.ToString(parserWithoutparens.ParsePath()), QueryNodeToStringVisitor.ToString(parserWithparens.ParsePath()));
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathKeysAsSegments()
        {
            var parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/Customers/5")) { UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash };
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathTypeSegmentToNavigation()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/People(1)/Microsoft.Test.Taupo.OData.WCFService.Customer/Orders"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));

            this.TestExtensions("people(1)/microsoft.test.taupo.ODATA.WCFService.Customer/OrderS");
            //, Unqualified type not supported yet.
            //"People(1)/Customer/Orders",
            //"People(1)/customer/Orders");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathTypeSegmentWithODataSimplifiedEnabled()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/Schools(1)/Student/Microsoft.Test.Taupo.OData.WCFService.Customer"))
            {
                UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash
            };
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathKeysAsSegmentsWithODataSimplifiedEnabled()
        {
            var parser = new ODataUriParser(model, new Uri("http://www.potato.com/"),
                new Uri("http://www.potato.com/Schools/1/Student/Microsoft.Test.Taupo.OData.WCFService.Customer"))
            {
                UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash
            };
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathCollection()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/People(1)/Numbers"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));

            this.TestExtensions("people(1)/numbers");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathShouldNotThrowTill100Nodes()
        {
            string pathqry = "http://potato.com/Customers(1)/Orders(1)/";

            for (int i = 0; i < 48; i++)
            {
                pathqry += @"OrderDetails(OrderID=1,ProductID=1)/AssociatedOrder/";
            }

            Assert.IsNotNull(new ODataUriParser(model, new Uri("http://potato.com/"), new Uri(pathqry)).ParsePath());
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ErrorWhenPathAbove100Nodes()
        {
            string pathqry = "http://potato.com/Customers(1)/Orders(1)/";

            for (int i = 0; i < 49; i++)
            {
                pathqry += @"OrderDetails(OrderID=1,ProductID=1)/AssociatedOrder/";
            }

            try
            {
                new ODataUriParser(model, new Uri("http://potato.com/"), new Uri(pathqry));
            }
            catch (ODataException e)
            {
                var expected = ODataExpectedExceptions.ODataException("UriQueryPathParser_TooManySegments");
                expected.ExpectedMessage.Verifier.VerifyMatch("UriQueryPathParser_TooManySegments", e.Message);
            }
        }

        #region Duration
        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathEntityWithDurationKey()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/DurationInKeys(duration'P1DT2H3M4.5678901S')"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));

            this.TestExtensions("durationinkeys(dUration'P1DT2H3M4.5678901S')");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathEntityWithDurationKeyUsingKeyAsSegmments()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/DurationInKeys/P1DT2H3M4.5678901S"));
            parser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash;
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));
        }
        #endregion

        #region Help methods
        protected void TestExtensions(string caseInsensitive, string unqualified = null, string unqualifiedCaseinsensitive = null)
        {
            ODataUriParser uriParser = new ODataUriParser(model, serviceRoot, new Uri(serviceRoot, caseInsensitive));
            this.TestCurrentExtension(uriParser, new ODataUriResolver() { EnableCaseInsensitive = true });

            if (!string.IsNullOrEmpty(unqualified))
            {
                uriParser = new ODataUriParser(model, serviceRoot, new Uri(serviceRoot, unqualified));
                this.TestCurrentExtension(uriParser, new UnqualifiedODataUriResolver());
            }

            if (!string.IsNullOrEmpty(unqualifiedCaseinsensitive))
            {
                uriParser = new ODataUriParser(model, serviceRoot, new Uri(serviceRoot, unqualifiedCaseinsensitive));
                this.TestCurrentExtension(uriParser, new UnqualifiedODataUriResolver() { EnableCaseInsensitive = true });
            }
        }

        protected void TestCurrentExtension(ODataUriParser parser, ODataUriResolver resolver)
        {
            this.TestExtension(parser, resolver, p => p.ParsePath(), QueryNodeToStringVisitor.ToString, this.ApprovalVerify);
        }
        #endregion
    }
}
