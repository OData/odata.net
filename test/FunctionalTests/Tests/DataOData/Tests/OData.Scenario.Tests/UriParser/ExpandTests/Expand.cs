//---------------------------------------------------------------------
// <copyright file="Expand.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Runtime.CompilerServices;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.Test.Taupo.OData.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.UriParser.ExpandTests
{
    [TestClass]
    public class Expand : UriParserTestsBase
    {
        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ExpandBasic()
        {
            this.ApprovalVerifyExpandParser(orderBase, "OrderDetails");

            this.TestAllInOneExtensionExpand(orderBase, "ORDERdetails", "OrderDetails");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ExpandWithPathToNavProp()
        {
            this.ApprovalVerifyExpandParser(orderBase, "CustomerForOrder($expand=Orders)");

            this.TestAllInOneExtensionExpand(
                orderBase,
                "CUSTOMERFORORDER($ExPAND=orders)",
                "CustomerForOrder($expand=Orders)");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ExpandWithTypeCast()
        {
            this.ApprovalVerifyExpandParser(peopleBase, "Microsoft.Test.Taupo.OData.WCFService.Customer/Orders");

            this.TestAllInOneExtensionExpand(
                peopleBase,
                "microsoft.test.taupo.odata.wcfservice.customer/orders",
                "Microsoft.Test.Taupo.OData.WCFService.Customer/Orders");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ExpandDeep()
        {
            this.ApprovalVerifyExpandParser(peopleBase, "Parent($expand=Parent($expand=Parent($expand=Parent($expand=Parent($expand=Parent($expand=Parent($expand=Parent($expand=Parent($expand=Parent($expand=Parent))))))))))");

            this.TestAllInOneExtensionExpand(
                peopleBase,
                "parent($expand=parent($expand=PARENT($expand=Parent($expand=PARENT($expand=parent($expand=Parent($expand=Parent($exPand=Parent($expAnd=parent($expaNd=Parent))))))))))",
                "Parent($expand=Parent($expand=Parent($expand=Parent($expand=Parent($expand=Parent($expand=Parent($expand=Parent($expand=Parent($expand=Parent($expand=Parent))))))))))");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ExpandMultipleNavProps()
        {
            this.ApprovalVerifyExpandParser(orderBase, "OrderDetails, CustomerForOrder");

            this.TestAllInOneExtensionExpand(
                orderBase,
                "orderdetails, CUSTOMERFORORDER",
                "OrderDetails, CustomerForOrder");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ExpandWithQueryOptionsV3ShouldThrow()
        {
            ODataUriParser parser = this.CreateExpandUriParser(orderBase, "OrderDetails(OrderBy=OrderID)");
            try
            {
                parser.ParseSelectAndExpand();
            }
            catch (ODataException e)
            {
                var expected = ODataExpectedExceptions.ODataException("UriSelectParser_TermIsNotValid", "(OrderBy=OrderID)");
                expected.ExpectedMessage.Verifier.VerifyMatch("UriSelectParser_TermIsNotValid", e.Message, "(OrderBy=OrderID)");
            }
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ExpandWithSystemOptionShouldThrow()
        {
            ODataUriParser parser = this.CreateExpandUriParser(orderBase, "$metadata");
            try
            {
                parser.ParseSelectAndExpand();
            }
            catch (ODataException e)
            {
                var expected = ODataExpectedExceptions.ODataException("UriSelectParser_SystemTokenInSelectExpand", "$metadata", "$metadata");
                expected.ExpectedMessage.Verifier.VerifyMatch("UriSelectParser_SystemTokenInSelectExpand", e.Message, "$metadata", "$metadata");
            }
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ExpandPropertyDoesntExistShouldThrow()
        {
            ODataUriParser parser = this.CreateExpandUriParser(orderBase, "Potato");
            try
            {
                parser.ParseSelectAndExpand();
            }
            catch (ODataException e)
            {
                var expected = ODataExpectedExceptions.ODataException("MetadataBinder_PropertyNotDeclared", this.order.FullName(), "Potato");
                expected.ExpectedMessage.Verifier.VerifyMatch("MetadataBinder_PropertyNotDeclared", e.Message, this.order.FullName(), "Potato");
            }
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ExpandComplexPropertyShouldThrow()
        {
            ODataUriParser parser = this.CreateExpandUriParser(peopleBase, "HomeAddress");
            try
            {
                parser.ParseSelectAndExpand();
            }
            catch (ODataException e)
            {
                var expected = ODataExpectedExceptions.ODataException("ExpandItemBinder_PropertyIsNotANavigationPropertyOrComplexProperty", "HomeAddress", this.person.FullName());
                expected.ExpectedMessage.Verifier.VerifyMatch("ExpandItemBinder_PropertyIsNotANavigationPropertyOrComplexProperty", e.Message, "HomeAddress", this.person.FullName());
            }
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ExpandPrimitiveShouldThrow()
        {
            ODataUriParser parser = this.CreateExpandUriParser(orderBase, "CustomerID");
            try
            {
                parser.ParseSelectAndExpand();
            }
            catch (ODataException e)
            {
                var expected = ODataExpectedExceptions.ODataException("ExpandItemBinder_PropertyIsNotANavigationPropertyOrComplexProperty", "CustomerID", this.order.FullName());
                expected.ExpectedMessage.Verifier.VerifyMatch("ExpandItemBinder_PropertyIsNotANavigationPropertyOrComplexProperty", e.Message, "CustomerID", this.order.FullName());
            }
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ExpandFunctionShouldThrow()
        {
            ODataUriParser parser = this.CreateExpandUriParser(orderBase, "OrdersWithMoreThanTwoItems");
            try
            {
                parser.ParseSelectAndExpand();
            }
            catch (ODataException e)
            {
                var expected = ODataExpectedExceptions.ODataException("MetadataBinder_PropertyNotDeclared", this.order.FullName(), "OrdersWithMoreThanTwoItems");
                expected.ExpectedMessage.Verifier.VerifyMatch("MetadataBinder_PropertyNotDeclared", e.Message, this.order.FullName(), "OrdersWithMoreThanTwoItems");
            }
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ExpandOnSingleton()
        {
            this.ApprovalVerifyExpandParser(bossBase, "Parent");

            this.TestAllInOneExtensionExpand(
                bossBase,
                "PARENT",
                "Parent");
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ExpandOnContainedEntitySet()
        {
            this.ApprovalVerifyExpandParser(peopleBase, "Child");

            this.TestAllInOneExtensionExpand(
                peopleBase,
                "ChILd",
                "Child");
        }
    }
}
