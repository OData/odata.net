//---------------------------------------------------------------------
// <copyright file="OperationLinkBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using Microsoft.OData.Service.Providers;
    using Microsoft.OData.Service.Serializers;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class OperationLinkBuilderTests
    {
        private readonly OperationWrapper operationWithParameters;
        private readonly EntityToSerialize entityToSerialize;
        private readonly OperationWrapper operationBoundToBaseType;
        private readonly OperationWrapper operationWithEscapedParameter;
        private readonly OperationLinkBuilder testSubject;

        public OperationLinkBuilderTests()
        {
            ResourceType intType = ResourceType.GetPrimitiveResourceType(typeof(int));

            var customerType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "FQ.NS", "Customer", false);
            customerType.CanReflectOnInstanceType = false;
            customerType.AddProperty(new ResourceProperty("Id", ResourcePropertyKind.Primitive | ResourcePropertyKind.Key, intType) { CanReflectOnInstanceTypeProperty = false });
            customerType.SetReadOnly();

            var operation = new ServiceAction("Action", intType, OperationParameterBindingKind.Sometimes, new[] { new ServiceActionParameter("P1", customerType), new ServiceActionParameter("P2", intType) }, null);
            operation.SetReadOnly();
            this.operationWithParameters = new OperationWrapper(operation);

            var typeWithEscapedName = new ResourceType(typeof(object), ResourceTypeKind.ComplexType, null, "FQ NS", "+ /", false);
            typeWithEscapedName.CanReflectOnInstanceType = false;
            typeWithEscapedName.AddProperty(new ResourceProperty("Number", ResourcePropertyKind.Primitive, intType) { CanReflectOnInstanceTypeProperty = false });
            typeWithEscapedName.SetReadOnly();

            operation = new ServiceAction("Action", intType, OperationParameterBindingKind.Sometimes, new[] { new ServiceActionParameter("P1", customerType), new ServiceActionParameter("P2", typeWithEscapedName) }, null);
            operation.SetReadOnly();
            this.operationWithEscapedParameter = new OperationWrapper(operation);

            var bestCustomerType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, customerType, "FQ.NS", "BestCustomer", false);
            bestCustomerType.SetReadOnly();

            operation = new ServiceAction("Action", intType, OperationParameterBindingKind.Sometimes, new[] { new ServiceActionParameter("P1", customerType) }, null);
            operation.SetReadOnly();
            this.operationBoundToBaseType = new OperationWrapper(operation);

            this.entityToSerialize = EntityToSerialize.CreateFromExplicitValues(new object(), bestCustomerType, new TestSerializedEntityKey("http://odata.org/Service.svc/Customers/", bestCustomerType.FullName));

            var metadataUri = new Uri("http://odata.org/Service.svc/$metadata");
            this.testSubject = new OperationLinkBuilder("MyContainer", metadataUri);
        }

        [TestMethod]
        public void OperationMetadataLinkShouldContainParametersIfMultipleWithSameName()
        {
            this.testSubject.BuildMetadataLink(this.operationWithParameters, true).OriginalString
                .Should().Be("http://odata.org/Service.svc/$metadata#MyContainer.Action");
        }

        [TestMethod]
        public void OperationMetadataLinkShouldHaveParameterTypeNamesEscaped()
        {
            this.testSubject.BuildMetadataLink(this.operationWithEscapedParameter, true).OriginalString
                .Should().Be("http://odata.org/Service.svc/$metadata#MyContainer.Action");
        }

        [TestMethod]
        public void OperationTargetLinkShouldUseExactBindingTypeIfMultipleWithSameName()
        {
            this.testSubject.BuildTargetLink(this.entityToSerialize, this.operationBoundToBaseType, true).OriginalString
                .Should().Be("http://odata.org/Service.svc/Customers/FQ.NS.Customer/MyContainer.Action");
        }

        [TestMethod]
        public void OperationMetadataLinkShouldNotContainParametersByDefault()
        {
            this.testSubject.BuildMetadataLink(this.operationWithParameters, false).OriginalString
                .Should().Be("http://odata.org/Service.svc/$metadata#MyContainer.Action");
        }

        [TestMethod]
        public void OperationTargetLinkShouldUseNormalEditLinkWithExactTypeByDefault()
        {
            this.testSubject.BuildTargetLink(this.entityToSerialize, this.operationBoundToBaseType, false).OriginalString
                .Should().Be("http://odata.org/Service.svc/Customers/FQ.NS.BestCustomer/MyContainer.Action");
        }
    }
}