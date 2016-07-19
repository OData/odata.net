//---------------------------------------------------------------------
// <copyright file="UriParserTestsBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.UriParser
{
    using System;
    using System.Collections.Generic;
    using ApprovalTests;
    using ApprovalTests.Reporters;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.Common;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [UseReporter(typeof(LoggingReporter))]
    [TestClass]
    [DeploymentItem("UriParser\\ExpandTests")]
    [DeploymentItem("UriParser\\Filter")]
    [DeploymentItem("UriParser\\OrderBy")]
    [DeploymentItem("UriParser\\Path")]
    [DeploymentItem("UriParser\\SelectExpand")]
    [DeploymentItem("UriParser\\SelectTests")]
    [DeploymentItem("UriParser\\SingletonTests")]
    [DeploymentItem("UriParser\\Search")]
    [DeploymentItem("UriParser\\MultiBindingTests")]
    public class UriParserTestsBase
    {
        protected IEdmModel model;

        protected IEdmType customerType;
        protected IEdmType personType;
        protected IEdmType employeeType;
        protected IEdmType productType;
        protected IEdmType orderType;
        protected IEdmType orderDetailType;

        protected IEdmEntityContainer defaultContainer;

        protected IEdmEntitySet peopleSet;
        protected IEdmEntitySet customerSet;
        protected IEdmEntitySet employeeSet;
        protected IEdmEntitySet productSet;
        protected IEdmEntitySet orderSet;
        protected IEdmEntitySet orderDetailSet;

        protected IEdmSingleton boss;
        protected IEdmSingleton specialOrder;
        protected IEdmSingleton vipCustomer;
        protected IEdmSingleton specialProduct;

        protected IEdmContainedEntitySet childSet;
        protected IEdmContainedEntitySet brotherSet;

        protected IEdmEntityType customer;
        protected IEdmEntityType person;
        protected IEdmEntityType employee;
        protected IEdmEntityType product;
        protected IEdmEntityType order;
        protected IEdmEntityType orderDetail;

        protected Uri serviceRoot;
        protected Uri productBase;
        protected Uri employeeBase;
        protected Uri customerBase;
        protected Uri peopleBase;
        protected Uri numbersBase;
        protected Uri orderBase;
        protected Uri orderDetailBase;
        protected Uri bossBase;
        protected Uri specialOrderBase;
        protected Uri specialProductBase;
        protected Uri vipCustomerBase;
        protected Uri specialPersonBase;
        protected Uri durationInKeysBase;

        public TestContext TestContext { get; set; }

        public UriParserTestsBase()
        {
            this.model = new InMemoryModel().GetModel();
            this.customerType = this.model.FindType("Microsoft.Test.Taupo.OData.WCFService.Customer");
            this.personType = this.model.FindType("Microsoft.Test.Taupo.OData.WCFService.Person");
            this.employeeType = this.model.FindType("Microsoft.Test.Taupo.OData.WCFService.Employee");
            this.productType = this.model.FindType("Microsoft.Test.Taupo.OData.WCFService.Product");
            this.orderType = this.model.FindType("Microsoft.Test.Taupo.OData.WCFService.Order");
            this.orderDetailType = this.model.FindType("Microsoft.Test.Taupo.OData.WCFService.OrderDetail");

            this.defaultContainer = this.model.FindEntityContainer("InMemoryEntities");
            this.peopleSet = this.defaultContainer.FindEntitySet("People");
            this.customerSet = this.defaultContainer.FindEntitySet("Customers");
            this.employeeSet = this.defaultContainer.FindEntitySet("Employees");
            this.productSet = this.defaultContainer.FindEntitySet("Products");
            this.orderSet = this.defaultContainer.FindEntitySet("Orders");
            this.orderDetailSet = this.defaultContainer.FindEntitySet("OrderDetails");

            this.boss = this.defaultContainer.FindSingleton("Boss");
            this.specialOrder = this.defaultContainer.FindSingleton("SpecialOrder");
            this.vipCustomer = this.defaultContainer.FindSingleton("VipCustomer");
            this.specialProduct = this.defaultContainer.FindSingleton("SpecialProduct");

            IEdmNavigationProperty childNavigationProperty = ((IEdmEntityType)this.personType).FindProperty("Child") as IEdmNavigationProperty;
            this.childSet = peopleSet.FindNavigationTarget(childNavigationProperty) as IEdmContainedEntitySet;
            IEdmNavigationProperty brotherNavigationProperty = ((IEdmEntityType)this.personType).FindProperty("Brother") as IEdmNavigationProperty;
            this.brotherSet = peopleSet.FindNavigationTarget(brotherNavigationProperty) as IEdmContainedEntitySet;

            this.customer = (IEdmEntityType)customerType;
            this.person = (IEdmEntityType)personType;
            this.employee = (IEdmEntityType)employeeType;
            this.product = (IEdmEntityType)productType;
            this.order = (IEdmEntityType)orderType;
            this.orderDetail = (IEdmEntityType)orderDetailType;

            this.serviceRoot = new Uri("http://www.potato.com/");
            this.productBase = new Uri("http://www.potato.com/Products/");
            this.employeeBase = new Uri("http://www.potato.com/Employees/");
            this.customerBase = new Uri("http://www.potato.com/Customers/");
            this.peopleBase = new Uri("http://www.potato.com/People/");
            this.numbersBase = new Uri("http://www.potato.com/People(1)/Numbers");
            this.orderBase = new Uri("http://www.potato.com/Orders/");
            this.orderDetailBase = new Uri("http://www.potato.com/OrderDetails/");
            this.bossBase = new Uri("http://www.potato.com/Boss/");
            this.specialOrderBase = new Uri("http://www.potato.com/SpecialOrder");
            this.specialProductBase = new Uri("http://www.potato.com/SpecialProduct");
            this.vipCustomerBase = new Uri("http://www.potato.com/VipCustomer");
            this.specialPersonBase = new Uri("http://www.potato.com/SpecialPerson");
            this.durationInKeysBase = new Uri("http://www.potato.com/DurationInKeys");
        }

        public void ApprovalVerify(string result)
        {
            Approvals.Verify(new ApprovalTextWriter(result), new CustomSourcePathNamer(this.TestContext.DeploymentDirectory), Approvals.GetReporter());
        }

        protected ODataUriParser CreateSelectUriParser(Uri resourceBase, string queryOption)
        {
            return new ODataUriParser(this.model, this.serviceRoot, new Uri(resourceBase, "?$select=" + queryOption));
        }

        protected ODataUriParser CreateExpandUriParser(Uri resourceRoot, string queryOption, IEdmModel model = null)
        {
            return new ODataUriParser(model ?? this.model, this.serviceRoot, new Uri(resourceRoot, "?$expand=" + queryOption));
        }

        protected void ApprovalVerifyExpandParser(Uri resourceRoot, string queryOption, IEdmModel model = null)
        {
            ODataUriParser parser = this.CreateExpandUriParser(resourceRoot, queryOption, model);
            var result = parser.ParseSelectAndExpand();
            ApprovalVerify(QueryNodeToStringVisitor.GetTestCaseAndResultString(result, null, queryOption));
        }

        protected ODataUriParser CreateSelectAndExpandUriParser(Uri resourceRoot, string selectQueryOption,
            string expandQueryOption, IEdmModel model = null)
        {
            string queryOption = string.Empty;
            if (selectQueryOption != null && expandQueryOption != null)
            {
                queryOption = "?$select=" + selectQueryOption + "&$expand=" + expandQueryOption;
            }
            else if (selectQueryOption != null)
            {
                queryOption = "?$select=" + selectQueryOption;
            }
            else if (expandQueryOption != null)
            {
                queryOption = "?$expand=" + expandQueryOption;
            }

            return new ODataUriParser(model ?? this.model, this.serviceRoot, new Uri(resourceRoot, new Uri(resourceRoot, queryOption)));
        }

        protected void ApprovalVerifySelectAndExpandParser(Uri resourceRoot, string selectQueryOption, string expandQueryOption)
        {
            ODataUriParser parser = this.CreateSelectAndExpandUriParser(resourceRoot, selectQueryOption, expandQueryOption);
            var result = parser.ParseSelectAndExpand();
            ApprovalVerify(QueryNodeToStringVisitor.GetTestCaseAndResultString(result, selectQueryOption, expandQueryOption));
        }

        protected ODataUriParser CreateFilterUriParser(Uri resourceRoot, string queryOption, IEdmModel model = null)
        {
            return new ODataUriParser(model ?? this.model, this.serviceRoot, new Uri(resourceRoot, "?$filter=" + queryOption));
        }

        protected ODataUriParser CreateSearchUriParser(Uri resourceRoot, string queryOption)
        {
            return new ODataUriParser(this.model, this.serviceRoot, new Uri(resourceRoot, "?$search=" + queryOption));
        }

        protected void ApprovalVerifyFilterParser(Uri resourceRoot, string queryOption, IEdmModel model = null)
        {
            ODataUriParser parser = this.CreateFilterUriParser(resourceRoot, queryOption, model);
            var result = parser.ParseFilter();
            ApprovalVerify(QueryNodeToStringVisitor.GetTestCaseAndResultString(result, queryOption));
        }

        protected ODataUriParser CreateOrderByUriParser(Uri resourceRoot, string queryOption, IEdmModel model = null)
        {
            return new ODataUriParser(model ?? this.model, this.serviceRoot, new Uri(resourceRoot, "?$orderby=" + queryOption));
        }

        protected void ApprovalVerifyOrderByParser(Uri resourceRoot, string queryOption, IEdmModel model = null)
        {
            ODataUriParser parser = this.CreateOrderByUriParser(resourceRoot, queryOption, model);
            var result = parser.ParseOrderBy();
            ApprovalVerify(QueryNodeToStringVisitor.GetTestCaseAndResultString(result, queryOption));
        }

        protected void ApprovalVerifySearchParser(Uri resourceRoot, string queryOption)
        {
            ODataUriParser parser = this.CreateSearchUriParser(resourceRoot, queryOption);
            var result = parser.ParseSearch();
            ApprovalVerify(QueryNodeToStringVisitor.GetTestCaseAndResultString(result, queryOption));
        }

        protected void TestAllInOneExtensionSelectExpand(Uri baseUri, string select, string expand, string origSelect, string origExpand, IEdmModel model = null)
        {
            this.TestExtension(
                this.CreateSelectAndExpandUriParser(baseUri, select, expand, model),
                new AllInOneResolver() { EnableCaseInsensitive = true },
                parser => parser.ParseSelectAndExpand(),
                clause => QueryNodeToStringVisitor.GetTestCaseAndResultString(clause, origSelect, origExpand),
                this.ApprovalVerify);
        }

        protected void TestAllInOneExtensionExpand(Uri baseUri, string expand, string origExpand, IEdmModel model = null)
        {
            this.TestAllInOneExtensionSelectExpand(baseUri, null, expand, null, origExpand, model);
        }

        protected void TestAllInOneExtensionOrderBy(Uri baseUri, string orderby, string origOrderby, IEdmModel model = null)
        {
            this.TestExtension(
                this.CreateOrderByUriParser(baseUri, orderby, model),
                new AllInOneResolver() { EnableCaseInsensitive = true },
                parser => parser.ParseOrderBy(),
                clause => QueryNodeToStringVisitor.GetTestCaseAndResultString(clause, origOrderby),
                this.ApprovalVerify);
        }

        protected void TestAllInOneExtensionFilter(Uri baseUri, string filter, string origFilter, IEdmModel model = null)
        {
            this.TestExtension(
                this.CreateFilterUriParser(baseUri, filter, model),
                new AllInOneResolver() { EnableCaseInsensitive = true },
                parser => parser.ParseFilter(),
                clause => QueryNodeToStringVisitor.GetTestCaseAndResultString(clause, origFilter),
                this.ApprovalVerify);
        }

        protected void TestExtension<TResult>(ODataUriParser parser, ODataUriResolver resolver, Func<ODataUriParser, TResult> parse, Func<TResult, string> convert, Action<string> verify)
        {
            // Note parser changes after calling this method.

            // Case should fail with original parser
            this.ShouldThrow<ODataException>(delegate { parse(parser); });

            // Case should pass when custom resolver is set
            parser.Resolver = resolver;
            verify(convert(parse(parser)));
        }

        private void ShouldThrow<TException>(Action action, string message = null)
            where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException ex)
            {
                if (message != null)
                {
                    Assert.AreEqual(message, ex.Message);
                }

                return;
            }

            Assert.Fail("Exception expected.");
        }

        protected class AllInOneResolver : ODataUriResolver
        {

            private StringAsEnumResolver sae = new StringAsEnumResolver();
            private UnqualifiedODataUriResolver uor = new UnqualifiedODataUriResolver();
            private bool enableCaseInsensitive;

            public override bool EnableCaseInsensitive
            {
                get
                {
                    return this.enableCaseInsensitive;
                }
                set
                {
                    this.enableCaseInsensitive = value;
                    sae.EnableCaseInsensitive = this.enableCaseInsensitive;
                    uor.EnableCaseInsensitive = this.enableCaseInsensitive;
                }
            }

            public override IEnumerable<IEdmOperation> ResolveUnboundOperations(IEdmModel model, string identifier)
            {
                return uor.ResolveUnboundOperations(model, identifier);
            }

            public override IEnumerable<IEdmOperation> ResolveBoundOperations(IEdmModel model, string identifier, IEdmType bindingType)
            {
                return uor.ResolveBoundOperations(model, identifier, bindingType);
            }

            public override void PromoteBinaryOperandTypes(
                BinaryOperatorKind binaryOperatorKind,
                ref SingleValueNode leftNode,
                ref SingleValueNode rightNode,
                out IEdmTypeReference typeReference)
            {
                sae.PromoteBinaryOperandTypes(binaryOperatorKind, ref leftNode, ref rightNode, out typeReference);
            }

            public override IEnumerable<KeyValuePair<string, object>> ResolveKeys(
                IEdmEntityType type,
                IList<string> positionalValues,
                Func<IEdmTypeReference, string, object> convertFunc)
            {
                throw new NotImplementedException("here");
            }

            public override IEnumerable<KeyValuePair<string, object>> ResolveKeys(
                IEdmEntityType type,
                IDictionary<string, string> namedValues,
                Func<IEdmTypeReference, string, object> convertFunc)
            {
                return sae.ResolveKeys(type, namedValues, convertFunc);
            }
        }
    }
}
