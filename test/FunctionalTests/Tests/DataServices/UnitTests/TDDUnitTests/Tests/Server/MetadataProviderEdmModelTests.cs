//---------------------------------------------------------------------
// <copyright file="MetadataProviderEdmModelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Caching;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using AstoriaUnitTests.TDD.Tests.Server.Simulators;
    using AstoriaUnitTests.Tests.Server.Simulators;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// TODO: add tests for the rest of <see cref="MetadataProviderEdmModel"/>.
    /// </summary>
    [TestClass]
    public class MetadataProviderEdmModelTests
    {
        [TestMethod]
        public void KeyPropertiesOnEdmTypeShouldBeInSameOrderAsOnResourceType()
        {
            // Partial repro: EntityDescriptor identity/links key order changes for json DataServiceContext
            var resourceType = CreateResourceTypeWithKeyProperties("C", "B", "A" );
            var edmType = CreateModelAndGetEdmType(resourceType);
            edmType.Should().NotBeNull();
            edmType.DeclaredKey.Select(p => p.Name).Should().ContainInOrder(resourceType.KeyProperties.Select(p => p.Name));
        }

        [TestMethod]
        public void MetadataProviderModelShouldGoDirectlyToProviderWhenLookingUpAnEntitySetFromThePath()
        {
            var metadataProvider = new DataServiceProviderSimulator();
            var resourceSet = new ResourceSet("Some.Really.Var1.Name.<>+-.Something", CreateResourceTypeWithKeyProperties("Id"));
            resourceSet.SetReadOnly();
            metadataProvider.AddResourceSet(resourceSet);
            var model = CreateMetadataProviderEdmModel(metadataProvider);

            var result = model.FindDeclaredEntitySet(resourceSet.Name);
            result.Should().NotBeNull();
            result.Name.Should().Be(resourceSet.Name);
        }
        
        [TestMethod]
        public void MetadataProviderModelShouldGoDirectlyToProviderWhenLookingUpOperationsInSelect()
        {
            var entityType = CreateResourceTypeWithKeyProperties("Id");
            var derivedType = CreateDerivedType("Derived", entityType);
            var veryDerivedType = CreateDerivedType("VeryDerived", derivedType);
            var unrelatedDerivedType = CreateDerivedType("UnrelatedDerived", entityType);
            var action1 = new ServiceAction("Action", null, null, OperationParameterBindingKind.Always, new[] { new ServiceActionParameter("p1", entityType) });
            var action2 = new ServiceAction("Action", null, null, OperationParameterBindingKind.Always, new[] { new ServiceActionParameter("p1", derivedType) });
            var actionWithDifferentName = new ServiceAction("Action2", null, null, OperationParameterBindingKind.Always, new[] { new ServiceActionParameter("p1", derivedType) });
            var action3 = new ServiceAction("Action", null, null, OperationParameterBindingKind.Always, new[] { new ServiceActionParameter("p1", veryDerivedType) });
            action1.SetReadOnly();
            action2.SetReadOnly();
            actionWithDifferentName.SetReadOnly();
            action3.SetReadOnly();
            int calls = 0;
            var actionProvider = new TestActionProvider
                                 {
                                     GetByBindingTypeCallback = (o, t) =>
                                     {
                                         calls++;

                                         if (t == entityType)
                                         {
                                             return new[] { action1 };
                                         }

                                         if (t == derivedType)
                                         {
                                             return new[] { action2, actionWithDifferentName };
                                         }

                                         if (t == veryDerivedType)
                                         {
                                             return new[] { action3 };
                                         }

                                         t.Should().NotBeSameAs(unrelatedDerivedType);
                                         return null;
                                     }};

            var metadataProvider = new DataServiceProviderSimulator();
            metadataProvider.AddResourceType(entityType);
            metadataProvider.AddResourceType(derivedType);
            metadataProvider.AddResourceType(veryDerivedType);
            metadataProvider.AddResourceType(unrelatedDerivedType);
            var model = CreateMetadataProviderEdmModel(metadataProvider, actionProvider);
            var schemaType = (IEdmEntityType)model.EnsureSchemaType(derivedType);
            var result = model.FindDeclaredBoundOperations(schemaType).FilterByName(false, "Action");
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().OnlyContain(f => f.Name == "Action");
            calls.Should().Be(3);
        }

        private static MetadataProviderEdmModel CreateMetadataProviderEdmModel(DataServiceProviderSimulator metadataProvider, IDataServiceActionProvider actionProvider = null)
        {
            var dataServiceSimulator = new DataServiceSimulatorWithGetService { OperationContext = new DataServiceOperationContext(false, new DataServiceHost2Simulator()), ProcessingPipeline = new DataServiceProcessingPipeline() };
            dataServiceSimulator.OperationContext.InitializeAndCacheHeaders(dataServiceSimulator);

            var dataServiceConfiguration = new DataServiceConfiguration(metadataProvider);
            dataServiceConfiguration.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
            dataServiceConfiguration.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);
            dataServiceConfiguration.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;

            if (actionProvider != null)
            {
                dataServiceSimulator.Providers.Add(typeof(IDataServiceActionProvider), actionProvider);
            }

            DataServiceStaticConfiguration staticConfiguration = new DataServiceStaticConfiguration(dataServiceSimulator.Instance.GetType(), metadataProvider);
            IDataServiceProviderBehavior providerBehavior = DataServiceProviderBehavior.CustomDataServiceProviderBehavior;

            DataServiceProviderWrapper providerWrapper = new DataServiceProviderWrapper(
                    new DataServiceCacheItem(
                        dataServiceConfiguration,
                        staticConfiguration),
                    metadataProvider,
                    metadataProvider,
                    dataServiceSimulator,
                    false);

            dataServiceSimulator.Provider = providerWrapper;

            var model = new MetadataProviderEdmModel(providerWrapper, new DataServiceStreamProviderWrapper(dataServiceSimulator),  DataServiceActionProviderWrapper.Create(dataServiceSimulator));

            model.MetadataProvider.ProviderBehavior = providerBehavior;
            return model;
        }

        private static IEdmEntityType CreateModelAndGetEdmType(ResourceType resourceType)
        {
            var provider = new DataServiceProviderSimulator();
            DataServiceSimulator dataService = new DataServiceSimulator();

            DataServiceStaticConfiguration staticConfiguration = new DataServiceStaticConfiguration(dataService.Instance.GetType(), provider);
            IDataServiceProviderBehavior providerBehavior = DataServiceProviderBehavior.CustomDataServiceProviderBehavior;

            DataServiceProviderWrapper providerWrapper = new DataServiceProviderWrapper(
                    new DataServiceCacheItem(
                        new DataServiceConfiguration(provider),
                        staticConfiguration),
                    provider,
                    provider,
                    dataService,
                    false);

            dataService.Provider = providerWrapper;
            var model = new MetadataProviderEdmModel(providerWrapper, new DataServiceStreamProviderWrapper(dataService), DataServiceActionProviderWrapper.Create(dataService));

            dataService.ProcessingPipeline = new DataServiceProcessingPipeline();
            model.MetadataProvider.ProviderBehavior = providerBehavior;

            var edmType = model.EnsureSchemaType(resourceType) as IEdmEntityType;
            return edmType;
        }

        private static ResourceType CreateResourceTypeWithKeyProperties(params string[] keyPropertyNames)
        {
            var resourceType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "FQ.NS", "EntityType", false) {CanReflectOnInstanceType = false};

            foreach (var keyPropertyName in keyPropertyNames)
            {
                resourceType.AddProperty(new ResourceProperty(keyPropertyName, ResourcePropertyKind.Key | ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(int))) {CanReflectOnInstanceTypeProperty = false});
            }

            resourceType.SetReadOnly();
            return resourceType;
        }

        private static ResourceType CreateDerivedType(string name, ResourceType baseType)
        {
            var resourceType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, baseType, "FQ.NS", name, false) { CanReflectOnInstanceType = false };
            resourceType.SetReadOnly();
            return resourceType;
        }
    }
}
