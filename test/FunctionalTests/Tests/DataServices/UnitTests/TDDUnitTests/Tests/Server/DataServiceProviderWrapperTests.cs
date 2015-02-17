//---------------------------------------------------------------------
// <copyright file="DataServiceProviderWrapperTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Caching;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using System.Net;
    using AstoriaUnitTests.TDD.Tests.Server.Simulators;
    using AstoriaUnitTests.Tests.Server.Simulators;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ErrorStrings = Microsoft.OData.Service.Strings;

    [TestClass]
    public class DataServiceProviderWrapperTests
    {
        [TestMethod]
        public void DataServiceProviderWrapperShouldFailOnMultipleServiceOperationsWithSameName()
        {
            ResourceType stringType = ResourceType.GetPrimitiveResourceType(typeof(string));
            var duplicateOperation1 = new ServiceOperation("Duplicate", ServiceOperationResultKind.DirectValue, stringType, null, "GET", new[] { new ServiceOperationParameter("p1", stringType) });
            duplicateOperation1.SetReadOnly();
            var duplicateOpration2 = new ServiceOperation("Duplicate", ServiceOperationResultKind.DirectValue, ResourceType.GetPrimitiveResourceType(typeof(int)), null, "GET", new ServiceOperationParameter[0]);
            duplicateOpration2.SetReadOnly();

            var providerWrapper = CreateProviderWrapper(addMetadata: p =>
                                                                     {
                                                                         p.AddServiceOp(duplicateOperation1);
                                                                         p.AddServiceOp(duplicateOpration2);
                                                                     });

            Action getVisibleOperations = () => providerWrapper.GetVisibleOperations().ToList();
            getVisibleOperations.ShouldThrow<DataServiceException>()
                .WithMessage(ErrorStrings.DataServiceProviderWrapper_MultipleServiceOperationsWithSameName("Duplicate"))
                .And.StatusCode.Should().Be(500);
        }

        [TestMethod]
        public void DataServiceProviderWrapperShouldFailOnMultipleActionsWithSameNameAndBindingType()
        {
            var entityType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Fake.NS", "Type", false) { CanReflectOnInstanceType = false };
            entityType.AddProperty(new ResourceProperty("Id", ResourcePropertyKind.Key | ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(int))) {CanReflectOnInstanceTypeProperty = false});
            entityType.SetReadOnly();

            var resourceSet = new ResourceSet("MyEntitySet", entityType);
            resourceSet.SetReadOnly();

            ResourceType stringType = ResourceType.GetPrimitiveResourceType(typeof(string));
            var duplicateAction1 = new ServiceAction("Duplicate", stringType, OperationParameterBindingKind.Always, new[] { new ServiceActionParameter("p1", entityType), new ServiceActionParameter("p2", stringType) }, null);
            duplicateAction1.SetReadOnly();
            var duplicateAction2 = new ServiceAction("Duplicate", ResourceType.GetPrimitiveResourceType(typeof(int)), OperationParameterBindingKind.Sometimes, new[] { new ServiceActionParameter("p1", entityType) }, null);
            duplicateAction2.SetReadOnly();

            var actionProvider = new TestActionProvider
            {
                GetServiceActionsCallback = ctx => new[] { duplicateAction1, duplicateAction2 }
            };

            var providerWrapper = CreateProviderWrapper(actionProvider, p => p.AddResourceSet(resourceSet));

            Action getVisibleOperations = () => providerWrapper.GetVisibleOperations().ToList();
            getVisibleOperations.ShouldThrow<DataServiceException>()
                .WithMessage(ErrorStrings.DataServiceActionProviderWrapper_DuplicateAction("Duplicate"))
                .And.StatusCode.Should().Be(500);
        }

        [TestMethod]
        public void DataServiceProviderWrapperShouldFailOnMultipleActionsWithSameNameAndNoBindingType()
        {
            ResourceType stringType = ResourceType.GetPrimitiveResourceType(typeof(string));
            var duplicateAction1 = new ServiceAction("Duplicate", stringType, OperationParameterBindingKind.Never, new[] { new ServiceActionParameter("p1", stringType) }, null);
            duplicateAction1.SetReadOnly();
            var duplicateAction2 = new ServiceAction("Duplicate", ResourceType.GetPrimitiveResourceType(typeof(int)), OperationParameterBindingKind.Never, new ServiceActionParameter[0], null);
            duplicateAction2.SetReadOnly();

            var actionProvider = new TestActionProvider
            {
                GetServiceActionsCallback = ctx => new[] { duplicateAction1, duplicateAction2 }
            };

            var providerWrapper = CreateProviderWrapper(actionProvider);

            Action getVisibleOperations = () => providerWrapper.GetVisibleOperations().ToList();
            getVisibleOperations.ShouldThrow<DataServiceException>()
                .WithMessage(ErrorStrings.DataServiceActionProviderWrapper_DuplicateAction("Duplicate"))
                .And.StatusCode.Should().Be(500);
        }

        private static DataServiceProviderWrapper CreateProviderWrapper(TestActionProvider actionProvider = null, Action<DataServiceProviderSimulator> addMetadata = null)
        {
            var provider = new DataServiceProviderSimulator();
            if (addMetadata != null)
            {
                addMetadata(provider);
            }

            var dataService = new DataServiceSimulatorWithGetService();
            dataService.Configuration = new DataServiceConfiguration(provider);
            dataService.ProcessingPipeline = new DataServiceProcessingPipeline();
            dataService.Configuration.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);
            dataService.Configuration.SetEntitySetAccessRule("*", EntitySetRights.All);
            dataService.Configuration.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            dataService.Configuration.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;

            dataService.OperationContext = new DataServiceOperationContext(false, new DataServiceHost2Simulator {AbsoluteRequestUri = new Uri("http://fake.org/$metadata") });
            dataService.OperationContext.InitializeAndCacheHeaders(dataService);

            DataServiceStaticConfiguration staticConfiguration = new DataServiceStaticConfiguration(dataService.Instance.GetType(), provider);
            IDataServiceProviderBehavior providerBehavior = DataServiceProviderBehavior.CustomDataServiceProviderBehavior;

            var providerWrapper = new DataServiceProviderWrapper(
                new DataServiceCacheItem(
                    dataService.Configuration, 
                    staticConfiguration), 
                provider, 
                provider, 
                dataService,
                false);
            dataService.Provider = providerWrapper;
            providerWrapper.ProviderBehavior = providerBehavior;

            dataService.ActionProvider = DataServiceActionProviderWrapper.Create(dataService);
            if (actionProvider != null)
            {
                dataService.Providers.Add(typeof(IDataServiceActionProvider), actionProvider);
            }

            return providerWrapper;
        }
    }
}
