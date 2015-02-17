//---------------------------------------------------------------------
// <copyright file="DataServiceMetadataProviderWrapperTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using System.Reflection;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Stubs.DataServiceProvider;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using AstoriaTestNS = System.Data.Test.Astoria;
    #endregion Namespaces

    /// <summary>
    /// Tests for the internal DataServiceMetadataProviderWrapper class.
    /// </summary>
    [TestClass, TestCase]
    public class DataServiceMetadataProviderWrapperTests
    {
        [TestMethod, Variation("Validates that wrapper verifies that all returned types are read only.")]
        public void TypesMustBeReadOnlyTest()
        {
            var typesToTest = new Func<DSPMetadata, ResourceType>[] {
                (metadata) => metadata.AddEntityType("EntityType", null, null, false),
                (metadata) => metadata.AddComplexType("ComplexType", null, null, false)
            };

            AstoriaTestNS.TestUtil.RunCombinations(
                typesToTest,
                (typeCreate) =>
                {
                    var metadataProvider = new DSPMetadata("Test", "TestNS");
                    var type = typeCreate(metadataProvider);
                    var wrapper = new DataServiceMetadataProviderWrapper(metadataProvider);
                    ExceptionUtils.ExpectedException<DataServiceException>(
                        () => wrapper.TryResolveResourceType(type.FullName),
                        "The resource type '" + type.FullName + "' returned by the provider is not read-only. Please make sure that all the types are set to read-only.",
                        "Resolving resource type to writable type should fail.");
                });
        }

        [TestMethod, Variation("Validates that wrapper verifies that all returned sets are read only.")]
        public void SetsMustBeReadOnlyTest()
        {
            var resourceSetEntryPoints = new Action<DataServiceMetadataProviderWrapper, string>[] {
                (wrapper, name) => wrapper.TryResolveResourceSet(name),
                (wrapper, name) => wrapper.ResourceSets.Cast<object>().Count()
            };

            AstoriaTestNS.TestUtil.RunCombinations(
                resourceSetEntryPoints,
                (resourceSetEntryPoint) =>
                {
                    var metadataProvider = new DSPMetadata("Test", "TestNS");
                    var resourceType = metadataProvider.AddEntityType("EntityType", null, null, false);
                    var resourceSet = metadataProvider.AddResourceSet("Entities", resourceType);
                    resourceType.SetReadOnly();
                    var wrapper = new DataServiceMetadataProviderWrapper(metadataProvider);
                    ExceptionUtils.ExpectedException<DataServiceException>(
                        () => resourceSetEntryPoint(wrapper, resourceSet.Name),
                        "The resource set '" + resourceSet.Name + "' returned by the provider is not read-only. Please make sure that all the resource sets are set to read-only.",
                        "Accessing writable resource set should fail.");
                });
        }

        [TestMethod, Variation("Validates that wrapper verifies that all returned service operations are read only.")]
        public void ServiceOperationsMustBeReadOnlyTest()
        {
            var serviceOperationEntryPoints = new Action<DataServiceMetadataProviderWrapper, string>[] {
                (wrapper, name) => wrapper.TryResolveServiceOperation(name),
            };

            AstoriaTestNS.TestUtil.RunCombinations(
                serviceOperationEntryPoints,
                (serviceOperationEntryPoint) =>
                {
                    var metadataProvider = new DSPMetadata("Test", "TestNS");
                    var serviceOperation = metadataProvider.AddServiceOperation("ServiceOperation", ServiceOperationResultKind.DirectValue, ResourceType.GetPrimitiveResourceType(typeof(string)), null, "GET", new ServiceOperationParameter[0]);
                    var wrapper = new DataServiceMetadataProviderWrapper(metadataProvider);
                    ExceptionUtils.ExpectedException<DataServiceException>(
                        () => serviceOperationEntryPoint(wrapper, serviceOperation.Name),
                        "The service operation '" + serviceOperation.Name + "' returned by the provider is not read-only. Please make sure that all the service operations are set to read-only.",
                        "Accessing writable service operation should fail.");
                });
        }

        [TestMethod, Variation("Validates that the wrapper will only ask the underlying provider onces for a type.")]
        public void AskJustOncesForTypeTest()
        {
            var typesToTest = new Func<DSPMetadata, ResourceType>[] {
                (metadata) =>
                {
                    var t = metadata.AddEntityType("EntityType", null, null, false);
                    metadata.AddKeyProperty(t, "ID", typeof(int));
                    return t;
                },
                (metadata) => metadata.AddComplexType("ComplexType", null, null, false)
            };

            AstoriaTestNS.TestUtil.RunCombinations(
                typesToTest,
                (typeCreate) =>
                {
                    var metadataProvider = new CallCountMetadataProvider("Test", "TestNS");
                    var type = typeCreate(metadataProvider);
                    metadataProvider.SetReadOnly();
                    var wrapper = new DataServiceMetadataProviderWrapper(metadataProvider);
                    var resourceType = wrapper.TryResolveResourceType(type.FullName);
                    Assert.AreSame(type, resourceType, "The returned type is not the one expected.");
                    wrapper.TryResolveResourceType(type.FullName);
                    wrapper.TryResolveResourceType(type.FullName);
                    Assert.AreEqual(1, metadataProvider.TryResolveResourceTypeCallCount, "The underlying metadata provider should be called just once.");
                });
        }

        [TestMethod, Variation("Validates that the wrapper will only ask the underlying provider onces for a resource set.")]
        public void AskJustOncesForResourceSetTest()
        {
            var metadataProvider = new CallCountMetadataProvider("Test", "TestNS");
            var type = metadataProvider.AddEntityType("Entity", null, null, false);
            metadataProvider.AddKeyProperty(type, "ID", typeof(int));
            var resourceSet = metadataProvider.AddResourceSet("Entities", type);
            metadataProvider.SetReadOnly();
            var wrapper = new DataServiceMetadataProviderWrapper(metadataProvider);
            wrapper.TryResolveResourceSet(resourceSet.Name);
            wrapper.TryResolveResourceSet(resourceSet.Name);
            wrapper.TryResolveResourceSet(resourceSet.Name);
            Assert.AreEqual(1, metadataProvider.TryResolveResourceSetCallCount, "The underlying metadata provider should be called just once.");
        }

        [TestMethod, Variation("Validates that the wrapper will only ask the underlying provider onces for a service operation.")]
        public void AskJustOncesForServiceOperationTest()
        {
            var metadataProvider = new CallCountMetadataProvider("Test", "TestNS");
            var serviceOperation = metadataProvider.AddServiceOperation("ServiceOperation", ServiceOperationResultKind.DirectValue, ResourceType.GetPrimitiveResourceType(typeof(string)), null, "GET", new ServiceOperationParameter[0]);
            metadataProvider.SetReadOnly();
            var wrapper = new DataServiceMetadataProviderWrapper(metadataProvider);
            wrapper.TryResolveServiceOperation(serviceOperation.Name);
            wrapper.TryResolveServiceOperation(serviceOperation.Name);
            wrapper.TryResolveServiceOperation(serviceOperation.Name);
            Assert.AreEqual(1, metadataProvider.TryResolveServiceOperationCallCount, "The underlying metadata provider should be called just once.");
        }

        [TestMethod, Variation("Validates that the wrapper makes sure that resource set names are unique.")]
        public void ResourceSetNamesMustBeUniqueTest()
        {
            var metadataProvider = new ResourceSetsOverrideMetadataProvider("Test", "TestNS");
            metadataProvider.ResourceSetValues = new List<ResourceSet>();
            var type = metadataProvider.AddEntityType("EntityType", null, null, false);
            metadataProvider.ResourceSetValues.Add(new ResourceSet("Entities", type));
            metadataProvider.ResourceSetValues.Add(new ResourceSet("Entities", type));
            var wrapper = new DataServiceMetadataProviderWrapper(metadataProvider);
            ExceptionUtils.ExpectedException<DataServiceException>(
                () => wrapper.ResourceSets.Cast<object>().Count(),
                "The resource set 'Entities' returned by the provider is not read-only. Please make sure that all the resource sets are set to read-only.",
                "Two resource sets with the same name should fail in the wrapper.");
        }

        [TestMethod, Variation("Validates that the server allows one to redefine the concurrency token on a derived type.")]
        public void AllowRedefiningConcurrencyTokenOnDerivedType()
        {
            var metadataProvider = new DSPMetadata("DefaultContainer", "Default");

            var entityType = metadataProvider.AddEntityType("EntityType", null, null, false /*isAbstract*/);
            metadataProvider.AddKeyProperty(entityType, "ID", typeof(int));
            metadataProvider.AddPrimitiveProperty(entityType, "LastUpdatedAuthor", typeof(string), true /*eTag*/);

            var derivedType = metadataProvider.AddEntityType("DerivedType", null, entityType, false /*isAbstract*/);
            metadataProvider.AddPrimitiveProperty(derivedType, "LastModifiedAuthor", typeof(string), true /*eTag*/);

            metadataProvider.AddResourceSet("Customers", entityType);

            var wrapper = new DataServiceMetadataProviderWrapper(metadataProvider);

            DSPResource baseTypeInstance = new DSPResource(entityType);
            baseTypeInstance.SetValue("ID", 1);
            baseTypeInstance.SetValue("LastUpdatedAuthor", "Phani");

            DSPResource derivedTypeInstance = new DSPResource(derivedType);
            derivedTypeInstance.SetValue("ID", 1);
            derivedTypeInstance.SetValue("LastModifiedAuthor", "Raj");

            DSPContext dataContext = new DSPContext();
            var entities = dataContext.GetResourceSetEntities("Customers");
            entities.AddRange(new object[] { baseTypeInstance, derivedTypeInstance });

            var service = new DSPUnitTestServiceDefinition(metadataProvider, DSPDataProviderKind.Reflection, dataContext);
            using (TestWebRequest request = service.CreateForInProcess())
            {
                try
                {
                    request.StartService();
                    request.RequestUriString = "/$metadata";
                    request.SendRequestAndCheckResponse();
                }
                finally
                {
                    request.StopService();
                }
            }
        }

        [TestMethod, Variation("Validates that ContainerName and ContainerNamespace are only called once during metadata serialization")]
        public void MetadataProviderContainerNameAndNamespaceCalledOnlyOnceDuringSerialization()
        {
            CallCountMetadataProvider metadataProvider;
            DSPServiceDefinition service;
            CreateCallCounterService(out metadataProvider, out service);
            
            using (TestWebRequest request = service.CreateForInProcess())
            {
                try
                {
                    request.StartService();
                    request.RequestUriString = "/$metadata";
                    request.SendRequestAndCheckResponse();
                }
                finally
                {
                    request.StopService();
                }
            }

            Assert.AreEqual(1, metadataProvider.ContainerNameCallCount, "ContainerName called more than once");
            Assert.AreEqual(1, metadataProvider.ContainerNamespaceCallCount, "ContainerNamespace called more than once");
        }

        private static void CreateCallCounterService(out CallCountMetadataProvider metadataProvider, out DSPServiceDefinition service)
        {
            metadataProvider = new CallCountMetadataProvider("DefaultContainer", "Default");

            var entityType = metadataProvider.AddEntityType("EntityType", null, null, false /*isAbstract*/);
            metadataProvider.AddKeyProperty(entityType, "ID", typeof(int));
            metadataProvider.AddPrimitiveProperty(entityType, "LastUpdatedAuthor", typeof(string), true /*eTag*/);

            var derivedType = metadataProvider.AddEntityType("DerivedType", null, entityType, false /*isAbstract*/);
            metadataProvider.AddPrimitiveProperty(derivedType, "LastModifiedAuthor", typeof(string), true /*eTag*/);

            metadataProvider.AddResourceSet("Customers", entityType);

            var wrapper = new DataServiceMetadataProviderWrapper(metadataProvider);

            DSPContext dataContext = new DSPContext();

            service = new DSPServiceDefinition()
            {
                Metadata = metadataProvider,
                CreateDataSource = (m) => dataContext,
                ForceVerboseErrors = true,
                MediaResourceStorage = new DSPMediaResourceStorage(),
                SupportNamedStream = true,
                Writable = true,
                DataServiceBehavior = new OpenWebDataServiceDefinition.OpenWebDataServiceBehavior() { IncludeRelationshipLinksInResponse = true },
            };
        }

        private class CallCountMetadataProvider : DSPMetadata
        {
            public CallCountMetadataProvider(string containerName, string containerNamespace)
                : base(containerName, containerNamespace)
            {
            }

            public int ContainerNameCallCount { get; set; }
            public int ContainerNamespaceCallCount { get; set; }
            public int TryResolveResourceTypeCallCount { get; set; }
            public int TryResolveResourceSetCallCount { get; set; }
            public int TryResolveServiceOperationCallCount { get; set; }

            public override string ContainerName
            {
                get
                {
                    this.ContainerNameCallCount++;
                    return base.ContainerName;
                }
            }

            public override string ContainerNamespace
            {
                get
                {
                    this.ContainerNamespaceCallCount++;
                    return base.ContainerNamespace;
                }
            }

            public override bool TryResolveResourceType(string name, out ResourceType resourceType)
            {
                this.TryResolveResourceTypeCallCount++;
                return base.TryResolveResourceType(name, out resourceType);
            }

            public override bool TryResolveResourceSet(string name, out ResourceSet resourceSet)
            {
                this.TryResolveResourceSetCallCount++;
                return base.TryResolveResourceSet(name, out resourceSet);
            }

            public override bool TryResolveServiceOperation(string name, out ServiceOperation serviceOperation)
            {
                this.TryResolveServiceOperationCallCount++;
                return base.TryResolveServiceOperation(name, out serviceOperation);
            }
        }

        private class ResourceSetsOverrideMetadataProvider : DSPMetadata
        {
            public ResourceSetsOverrideMetadataProvider(string containerName, string containerNamespace)
                : base(containerName, containerNamespace)
            {
            }

            public List<ResourceSet> ResourceSetValues { get; set; }

            public override IEnumerable<ResourceSet> ResourceSets
            {
                get
                {
                    return this.ResourceSetValues;
                }
            }
        }

        private class DummyServiceType
        {
        }

        private class DataServiceMetadataProviderWrapper
        {
            private object instance;

            public DataServiceMetadataProviderWrapper(DSPMetadata metadataProvider)
            {
                DataServiceConfiguration config = (DataServiceConfiguration)Activator.CreateInstance(
                    typeof(DataServiceConfiguration),
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new object[] { metadataProvider },
                    null);
                config.SetEntitySetAccessRule("*", EntitySetRights.All);
                config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);

                Type staticConfigurationType = typeof(ResourceType).Assembly.GetType("Microsoft.OData.Service.DataServiceStaticConfiguration");
                object staticConfig = (object)Activator.CreateInstance(
                    staticConfigurationType,
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new object[] { typeof(DummyServiceType), metadataProvider },
                    null);

                Type cacheItemType = typeof(ResourceType).Assembly.GetType("Microsoft.OData.Service.Caching.DataServiceCacheItem");
                object cacheItem = Activator.CreateInstance(
                    cacheItemType,
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new object[] { config, staticConfig },
                    null);

                Type dspProviderBehaviorType = typeof(ResourceType).Assembly.GetType("Microsoft.OData.Service.Providers.DataServiceProviderBehavior");
                IDataServiceProviderBehavior providerBehavior = (IDataServiceProviderBehavior)Activator.CreateInstance(
                    dspProviderBehaviorType,
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new object[] { new ProviderBehavior(ProviderQueryBehaviorKind.CustomProviderQueryBehavior) },
                    null);

                Type type = typeof(ResourceType).Assembly.GetType("Microsoft.OData.Service.Providers.DataServiceProviderWrapper");
                instance = Activator.CreateInstance(
                    type,
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new object[] { cacheItem, metadataProvider, new DSPResourceQueryProvider(metadataProvider, false), /*dataService*/ null, false },
                    null);

                type.GetField("providerBehavior", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(instance, providerBehavior);
            }

            public ResourceType TryResolveResourceType(string name)
            {
                return (ResourceType)ReflectionUtils.InvokeMethod(this.instance, "TryResolveResourceType", name);
            }

            public object TryResolveResourceSet(string name)
            {
                return ReflectionUtils.InvokeMethod(this.instance, "TryResolveResourceSet", name);
            }

            public object TryResolveServiceOperation(string name)
            {
                return ReflectionUtils.InvokeMethod(this.instance, "TryResolveServiceOperation", name);
            }

            public IEnumerable ResourceSets
            {
                get
                {
                    return (IEnumerable)ReflectionUtils.InvokeMethod(this.instance, "GetResourceSets");
                }
            }
        }
    }
}
