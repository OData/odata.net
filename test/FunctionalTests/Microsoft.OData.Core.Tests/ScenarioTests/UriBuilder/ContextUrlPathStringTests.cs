//---------------------------------------------------------------------
// <copyright file="ContextUrlPathStringTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.UriBuilder
{
    public class ContextUrlPathStringTests : UriBuilderTestBase
    {
        [Fact]
        public void ContextUrlPathWithSimpleEntitySet()
        {
            Uri queryUri = new Uri("Dogs", UriKind.Relative);
            string res = this.GetContextUrlPathString(HardCodedTestModel.TestModel, queryUri);
            Assert.Equal("Dogs", res);
        }

        [Fact]
        public void ContextUrlPathWithNavigationPropertyLinks()
        {
            Uri queryUri = new Uri("People(1)/MyDog/$ref", UriKind.Relative);
            string res = this.GetContextUrlPathString(HardCodedTestModel.TestModel, queryUri);
            Assert.Equal("People(1)", res);
        }

        [Fact]
        public void ContextUrlPathWithSimpleServiceOperation()
        {
            Uri queryUri = new Uri("GetCoolPeople", UriKind.Relative);
            string res = this.GetContextUrlPathString(HardCodedTestModel.TestModel, queryUri);
            Assert.Equal("People", res);
        }

        [Fact]
        public void ContextUrlPathWithComplexServiceOperationIsComposable()
        {
            Uri queryUri = new Uri("GetSomeAddress/City", UriKind.Relative);
            string res = this.GetContextUrlPathString(HardCodedTestModel.TestModel, queryUri);
            Assert.Equal("Edm.String", res);
        }

        [Fact]
        public void ContextUrlPathWithEntityServiceOperationIsComposable()
        {
            Uri queryUri = new Uri("GetCoolestPerson/Fully.Qualified.Namespace.Employee", UriKind.Relative);
            string res = this.GetContextUrlPathString(HardCodedTestModel.TestModel, queryUri);
            Assert.Equal("People/Fully.Qualified.Namespace.Employee", res);
        }

        [Theory]
        [InlineData("People(1)/Fully.Qualified.Namespace.GetSomeAddressFromPerson()/Street")]
        [InlineData("People(1)/Fully.Qualified.Namespace.GetSomeAddressFromPerson/Street")]
        public void ContextUrlPathWithComplexPropertyAccessAfterOperationIsComposable(string uri)
        {
            Uri queryUri = new Uri(uri, UriKind.Relative);
            string res = this.GetContextUrlPathString(HardCodedTestModel.TestModel, queryUri);
            Assert.Equal("Edm.String", res);
        }


        [Theory]
        [InlineData("Customers(1)/NS.GetSomeOrders")]
        [InlineData("Customers(1)/NS.GetSomeOrders()")]
        [InlineData("Customers(1)/NS.GetAnOrder")]
        [InlineData("Customers(1)/NS.GetAnOrder()")]
        public void ContextUrlPathWithOperationWithEntitySetPathAndReturnType(string uri)
        {
            IEdmModel model = GetEdmModel();
            Uri queryUri = new Uri(uri, UriKind.Relative);
            string res = this.GetContextUrlPathString(model, queryUri);
            Assert.Equal("Customers(1)/Orders", res);
        }

        [Fact]
        public void ContextUrlPathWithOperationWithoutEntitySetPathAndComplexReturnType()
        {
            IEdmModel model = GetEdmModel();
            Uri queryUri = new Uri("Customers(1)/NS.GetSomeAddressFromCustomer1", UriKind.Relative);
            string res = this.GetContextUrlPathString(model, queryUri);
            Assert.Equal("NS.Address", res);
        }

        [Fact]
        public void ContextUrlPathWithPropertyAccessAfterOperationWithoutEntitySetPathAndComplexReturnType()
        {
            IEdmModel model = GetEdmModel();
            Uri queryUri = new Uri("Customers(1)/NS.GetSomeAddressFromCustomer3/City", UriKind.Relative);
            string res = this.GetContextUrlPathString(model, queryUri);
            Assert.Equal("Edm.String", res);
        }

        [Fact]
        public void ContextUrlPathWithOperationWithOutEntitySetPathWithoutReturnType()
        {
            IEdmModel model = GetEdmModel();
            Uri queryUri = new Uri("Customers(1)/NS.DoSomeThing", UriKind.Relative);
            string res = this.GetContextUrlPathString(model, queryUri);
            Assert.Equal("Edm.Untyped", res);
        }

        [Theory]
        [InlineData("GetSomeOrders2")]
        [InlineData("GetSomeOrders2()")]
        [InlineData("GetAnOrder2")]
        [InlineData("GetAnOrder2()")]
        public void ContextUrlPathWithOperationImportWithEntitySetWithReturnType(string uri)
        {
            IEdmModel model = GetEdmModel();
            Uri queryUri = new Uri(uri, UriKind.Relative);
            string res = this.GetContextUrlPathString(model, queryUri);
            Assert.Equal("Orders", res);
        }

        [Fact]
        public void ContextUrlPathWithOperationImportWithoutEntitySetWithComplexReturnType()
        {
            IEdmModel model = GetEdmModel();
            Uri queryUri = new Uri("GetSomeAddress", UriKind.Relative);
            string res = this.GetContextUrlPathString(model, queryUri);
            Assert.Equal("NS.Address", res);
        }

        [Fact]
        public void ContextUrlPathWithPropertyAccessAfterOperationImportWithoutEntitySetWithComplexReturnType()
        {
            IEdmModel model = GetEdmModel();
            Uri queryUri = new Uri("GetSomeAddress/City", UriKind.Relative);
            string res = this.GetContextUrlPathString(model, queryUri);
            Assert.Equal("Edm.String", res);
        }

        [Fact]
        public void ContextUrlPathWithOperationImportWithoutEntitySetWithOutReturnType()
        {
            IEdmModel model = GetEdmModel();
            Uri queryUri = new Uri("DoSomeThing2", UriKind.Relative);
            string res = this.GetContextUrlPathString(model, queryUri);
            Assert.Equal("Edm.Untyped", res);
        }

        internal static IEdmModel GetEdmModel()
        {
            var model = new EdmModel();

            // open address
            var address = new EdmComplexType("NS", "Address", null, false, true);
            address.AddStructuralProperty("Street", EdmCoreModel.Instance.GetString(true));
            address.AddStructuralProperty("City", EdmCoreModel.Instance.GetString(true));
            model.AddElement(address);

            // customer
            var customer = new EdmEntityType("NS", "Customer");
            customer.AddKeys(customer.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            customer.AddStructuralProperty("HomeAddress", new EdmComplexTypeReference(address, true));
            model.AddElement(customer);

            // order
            var order = new EdmEntityType("NS", "Order");
            order.AddKeys(order.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(order);

            var nav = customer.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {

                    Name = "Orders",
                    Target = order,
                    TargetMultiplicity = EdmMultiplicity.Many
                },
                new EdmNavigationPropertyInfo
                {
                    Name = "Customer",
                    TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
                });

            var container = new EdmEntityContainer("Default", "Container");
            model.AddElement(container);

            var customers = new EdmEntitySet(container, "Customers", customer);
            container.AddElement(customers);

            var orders = new EdmEntitySet(container, "Orders", order);
            container.AddElement(orders);
            customers.AddNavigationTarget(nav, orders);

            // bound operations
            var customerReference = new EdmEntityTypeReference(customer, true);
            var orderReference = new EdmEntityTypeReference(order, true);

            IEdmPathExpression path = new EdmPathExpression("binding/Orders");

            // entityset path & return type
            var function = new EdmFunction("NS", "GetSomeOrders", new EdmCollectionTypeReference(new EdmCollectionType(orderReference)), true, path, true /*isComposable*/);
            function.AddParameter("binding", customerReference);
            model.AddElement(function);

            function = new EdmFunction("NS", "GetAnOrder", orderReference, true, path, true /*isComposable*/);
            function.AddParameter("binding", customerReference);
            model.AddElement(function);

            // GetSomeAddressFromCustomer
            function = new EdmFunction("NS", "GetSomeAddressFromCustomer1", new EdmComplexTypeReference(address, true), true, null, true /*isComposable*/);
            function.AddParameter("binding", new EdmEntityTypeReference(customer, true));
            model.AddElement(function);

            // We leave the "GetSomeAddressFromCustomer2" code here.
            // However, the operation with the entity set path containing a complex type doesn't make sense.
            IEdmPathExpression complexPath = new EdmPathExpression("binding/HomeAddress");
            function = new EdmFunction("NS", "GetSomeAddressFromCustomer2", new EdmComplexTypeReference(address, true), true, complexPath, true /*isComposable*/);
            function.AddParameter("binding", new EdmEntityTypeReference(customer, true));
            model.AddElement(function);

            function = new EdmFunction("NS", "GetSomeAddressFromCustomer3", new EdmComplexTypeReference(address, true), true, null, true /*isComposable*/);
            function.AddParameter("binding", new EdmEntityTypeReference(customer, true));
            model.AddElement(function);

            var action = new EdmAction("NS", "DoSomeThing", null, true, null);
            action.AddParameter("binding", new EdmEntityTypeReference(customer, true));
            model.AddElement(action);

            // operation import
            function = new EdmFunction("NS", "GetSomeOrders2", new EdmCollectionTypeReference(new EdmCollectionType(orderReference)));
            EdmFunctionImport functionImport = new EdmFunctionImport(container, "GetSomeOrders2", function, new EdmPathExpression("Orders"), false);
            container.AddElement(functionImport);

            function = new EdmFunction("NS", "GetAnOrder2", orderReference);
            functionImport = new EdmFunctionImport(container, "GetAnOrder2", function, new EdmPathExpression("Orders"), false);
            container.AddElement(functionImport);

            function = new EdmFunction("NS", "GetSomeAddress", new EdmComplexTypeReference(address, true), false, null, true);
            functionImport = new EdmFunctionImport(container, "GetSomeAddress", function, null, false);
            container.AddElement(functionImport);

            action = new EdmAction("NS", "DoSomeThing2", null, false, null);
            var actionImport = new EdmActionImport(container, "DoSomeThing2", action, null);
            container.AddElement(actionImport);
            return model;
        }

        private string GetContextUrlPathString(IEdmModel model, Uri queryUri)
        {
            ODataUriParser odataUriParser = new ODataUriParser(model, queryUri);
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            ODataUri odataUri = odataUriParser.ParseUri();
            ODataPath odataPath = odataUri.Path;
            return odataPath.ToContextUrlPathString();
        }
    }
}
