//---------------------------------------------------------------------
// <copyright file="SingletonPathTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.UriParser
{
    using System;
    using System.Runtime.CompilerServices;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class SingletonPathTest : UriParserTestsBase
    {
        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathBaseSingleton()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/SpecialOrder"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathBaseSingletonWithKeyShouldFail()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/SpecialOrder(1)"));
            try
            {
                parser.ParsePath();
            }
            catch (ODataException e)
            {
                var expected = ODataExpectedExceptions.ODataException("RequestUriProcessor_SyntaxError");
                expected.ExpectedMessage.Verifier.VerifyMatch("RequestUriProcessor_SyntaxError", e.Message);
            }
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathNavigation()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/SpecialOrder/OrderDetails"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathPrimitive()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/SpecialOrder/OrderDate"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathPrimitiveValue()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/SpecialOrder/OrderDate/$value"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathLinks()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/SpecialOrder/OrderDetails/$ref"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathFunction()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/SpecialOrder/Microsoft.Test.Taupo.OData.WCFService.GetOrderRate"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathFunctionOfCollectionShouldFail()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/SpecialOrder/Microsoft.Test.Taupo.OData.WCFService.OrdersWithMoreThanTwoItems"));
            try
            {
                parser.ParsePath();
            }
            catch (ODataException e)
            {
                var expected = ODataExpectedExceptions.ODataException("RequestUriProcessor_ResourceNotFound", "Microsoft.Test.Taupo.OData.WCFService.OrdersWithMoreThanTwoItems");
                expected.ExpectedMessage.Verifier.VerifyMatch("RequestUriProcessor_ResourceNotFound", e.Message);
            }
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathFunctionWithParameter()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/VipCustomer/Microsoft.Test.Taupo.OData.WCFService.Within(Distance=10)"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathFunctionWithAliasedParameter()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/VipCustomer/Microsoft.Test.Taupo.OData.WCFService.Within(Distance=@a)"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathFunctionAliasedParameterWithResolver()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/VipCustomer/Microsoft.Test.Taupo.OData.WCFService.Within(Distance=@a)?@a=100"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));
        }
        
        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathFunctionWithComplexParameters()
        {
            string inputstring = "http://www.potato.com/VipCustomer/Microsoft.Test.Taupo.OData.WCFService.Within(Location={ Street:'1 Microsoft Way', City:'Redmond', PostalCode:'98052' }, Distance=10)";
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri(inputstring));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathFunctionWithParens()
        {
            ODataUriParser parserWithoutparens = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/SpecialOrder/Microsoft.Test.Taupo.OData.WCFService.GetOrderRate"));
            ODataUriParser parserWithparens = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/SpecialOrder/Microsoft.Test.Taupo.OData.WCFService.GetOrderRate()"));
            Assert.AreEqual(QueryNodeToStringVisitor.ToString(parserWithoutparens.ParsePath()), QueryNodeToStringVisitor.ToString(parserWithparens.ParsePath()));
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathAction()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/Boss/Microsoft.Test.Taupo.OData.WCFService.ChangeAddress"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathActionWithParens()
        {
            ODataUriParser parserWithoutparens = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/Boss/Microsoft.Test.Taupo.OData.WCFService.ChangeAddress"));
            ODataUriParser parserWithparens = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/Boss/Microsoft.Test.Taupo.OData.WCFService.ChangeAddress()"));
            Assert.AreEqual(QueryNodeToStringVisitor.ToString(parserWithoutparens.ParsePath()), QueryNodeToStringVisitor.ToString(parserWithparens.ParsePath()));
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void PathTypeSegmentToNavigation()
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://www.potato.com/"), new Uri("http://www.potato.com/SpecialPerson/Microsoft.Test.Taupo.OData.WCFService.Customer/Orders"));
            var result = parser.ParsePath();
            ApprovalVerify(QueryNodeToStringVisitor.ToString(result));
        }
    }
}
