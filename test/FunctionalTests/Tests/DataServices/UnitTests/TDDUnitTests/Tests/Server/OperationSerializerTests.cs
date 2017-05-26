//---------------------------------------------------------------------
// <copyright file="OperationSerializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using Microsoft.OData.Service.Providers;
    using Microsoft.OData.Service.Serializers;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.OData;

    [TestClass]
    public class OperationSerializerTests
    {
        private readonly OperationWrapper baseTypeOperation;
        private readonly EntityToSerialize entityToSerialize;
        private readonly OperationWrapper derivedTypeOperation;
        private readonly OperationSerializer testSubject;
        private readonly OperationWrapper unambiguousOperation;

        public OperationSerializerTests()
        {
            ResourceType intType = ResourceType.GetPrimitiveResourceType(typeof(int));

            var customerType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "FQ.NS", "Customer", false);
            customerType.CanReflectOnInstanceType = false;
            customerType.AddProperty(new ResourceProperty("Id", ResourcePropertyKind.Primitive | ResourcePropertyKind.Key, intType) { CanReflectOnInstanceTypeProperty = false });
            customerType.SetReadOnly();

            var operation = new ServiceAction("Action", intType, OperationParameterBindingKind.Sometimes, new[] { new ServiceActionParameter("P1", customerType), new ServiceActionParameter("P2", intType) }, null);
            operation.SetReadOnly();
            this.baseTypeOperation = new OperationWrapper(operation);

            var bestCustomerType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, customerType, "FQ.NS", "BestCustomer", false);
            bestCustomerType.SetReadOnly();

            operation = new ServiceAction("Action", intType, OperationParameterBindingKind.Sometimes, new[] { new ServiceActionParameter("P1", bestCustomerType) }, null);
            operation.SetReadOnly();
            this.derivedTypeOperation = new OperationWrapper(operation);

            operation = new ServiceAction("Unambiguous", intType, OperationParameterBindingKind.Sometimes, new[] { new ServiceActionParameter("P1", customerType) }, null);
            operation.SetReadOnly();
            this.unambiguousOperation = new OperationWrapper(operation);

            this.entityToSerialize = EntityToSerialize.CreateFromExplicitValues(new object(), bestCustomerType, new TestSerializedEntityKey("http://odata.org/Service.svc/Customers(0)/", bestCustomerType.FullName));

            this.testSubject = CreateOperationSerializer(AlwaysAdvertiseActions);
        }

        [TestMethod]
        public void OperationSerializationWithNameCollisionsShouldBeDistinct()
        {
            var results = this.testSubject.SerializeOperations(this.entityToSerialize, false, new[] { this.baseTypeOperation, this.derivedTypeOperation });
            results.Should().HaveCount(2);
            results.Should().OnlyContain(action => !action.Target.IsAbsoluteUri);
        }

        [TestMethod]
        public void OperationSerializationShouldBeUnchangedForActionsWhichAreAlongSideCollisions()
        {
            var results = this.testSubject.SerializeOperations(this.entityToSerialize, false, new[] { this.baseTypeOperation, this.derivedTypeOperation, this.unambiguousOperation });
            results.Should().HaveCount(3)
                .And.Subject.ElementAt(2).ShouldHave().SharedProperties().EqualTo(
                    new
                    {
                        // Target has type-segment for derived type, from the edit link, not from the binding parameter
                        Target = new Uri("Customers(0)/FQ.NS.BestCustomer/MyContainer.Unambiguous", UriKind.Relative), 
                        Metadata = new Uri("http://odata.org/Service.svc/$metadata#MyContainer.Unambiguous")
                    });
        }

        [TestMethod]
        public void OperationLinksShownToProviderShouldAlreadyBeDistinctAndShouldNotBeChangedAfterwards()
        {
            // The callback will take the link and append to it. The expected results below captures both what the provider saw and ensures that it was not modified afterwards.
            var serializer = CreateOperationSerializer(AppendToLinks);
            var results = serializer.SerializeOperations(this.entityToSerialize, false, new[] {this.baseTypeOperation, this.derivedTypeOperation}).ToList();
            results.Should().HaveCount(2);
            results[0].Metadata.OriginalString.Should().EndWith("MyContainer.Action/SomethingThatWasAppended");
            results[0].Target.OriginalString.Should().EndWith("FQ.NS.Customer/MyContainer.Action/SomethingThatWasAppended");
            results[1].Metadata.OriginalString.Should().EndWith("MyContainer.Action/SomethingThatWasAppended");
            results[1].Target.OriginalString.Should().EndWith("FQ.NS.BestCustomer/MyContainer.Action/SomethingThatWasAppended");
        }

        private static bool AlwaysAdvertiseActions(OperationWrapper serviceAction, object resourceInstance, bool resourceInstanceInFeed, ref ODataAction actionToSerialize)
        {
            // the links should always be provided and always be absolute, even if they will be written relative in JSON-Light.
            actionToSerialize.Metadata.Should().NotBeNull().And.Subject.As<Uri>().IsAbsoluteUri.Should().BeTrue();
            actionToSerialize.Target.Should().NotBeNull().And.Subject.As<Uri>().IsAbsoluteUri.Should().BeTrue();
            return true;
        }

        private static bool AppendToLinks(OperationWrapper serviceAction, object resourceInstance, bool resourceInstanceInFeed, ref ODataAction actionToSerialize)
        {
            actionToSerialize.Metadata = new Uri(actionToSerialize.Metadata.OriginalString + "/SomethingThatWasAppended");
            actionToSerialize.Target = new Uri(actionToSerialize.Target.OriginalString + "/SomethingThatWasAppended");
            return true;
        }

        private static OperationSerializer CreateOperationSerializer(AdvertiseServiceActionCallback advertiseServiceActionCallback)
        {
            var parameterInterpreter = new PayloadMetadataParameterInterpreter(ODataFormat.Json, "full");
            var propertyManager = new PayloadMetadataPropertyManager(parameterInterpreter);

            return new OperationSerializer(parameterInterpreter, propertyManager, advertiseServiceActionCallback, "MyContainer", ODataFormat.Json, new Uri("http://odata.org/Service.svc/$metadata"));
        }
    }
}