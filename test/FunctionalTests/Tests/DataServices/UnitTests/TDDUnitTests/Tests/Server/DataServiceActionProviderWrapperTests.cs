//---------------------------------------------------------------------
// <copyright file="DataServiceActionProviderWrapperTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using AstoriaUnitTests.TDD.Tests.Server.Simulators;
    using AstoriaUnitTests.Tests.Server.Simulators;
    using FluentAssertions;
    using Microsoft.OData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ErrorStrings = Microsoft.OData.Service.Strings;

    [TestClass]
    public class DataServiceActionProviderWrapperTests
    {
        private ServiceAction action;
        private ResourceType entityType;
        private DataServiceActionProviderWrapper actionProvider;
        private DataServiceOperationContext operationContext;
        private TestDataServiceProvider provider;
        private ResourceSetWrapper resourceSetWrapper;

        [TestInitialize]
        public void Init()
        {
            this.entityType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Fake.NS", "Type", false) { CanReflectOnInstanceType = false };
            this.entityType.AddProperty(new ResourceProperty("Id", ResourcePropertyKind.Key | ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(int))) { CanReflectOnInstanceTypeProperty = false });
            this.entityType.SetReadOnly();
            
            var resourceSet = new ResourceSet("Set", this.entityType);
            resourceSet.SetReadOnly();
            this.resourceSetWrapper = ResourceSetWrapper.CreateForTests(resourceSet);

            this.action = new ServiceAction("Fake", ResourceType.GetPrimitiveResourceType(typeof(int)), OperationParameterBindingKind.Sometimes, new[] { new ServiceActionParameter("p1", this.entityType) }, null);
            this.action.SetReadOnly();

            this.provider = new TestDataServiceProvider();

            this.actionProvider = DataServiceActionProviderWrapper.Create(this.provider, ODataProtocolVersion.V4, () => this.operationContext);
            var host = new DataServiceHostSimulator();
            this.operationContext = new DataServiceOperationContext(host);
            this.operationContext.InitializeAndCacheHeaders(new DataServiceSimulator());
        }

        [TestMethod]
        public void ActionProviderWrapperShouldUseMoreSpecificResolutionApiIfProvided()
        {
            this.provider.ActionProvider = new TestActionProviderWithResolution(this.ResolveSpecificActionOnEntityType);
            var operationWrapper = this.actionProvider.TryResolveServiceAction("Fake", this.entityType);
            operationWrapper.Should().NotBeNull();
            operationWrapper.ServiceAction.Should().BeSameAs(this.action);
        }

        [TestMethod]
        public void ActionProviderWrapperShouldNotFallBackToOldInterfaceUseIfMoreSpecificResolutionApiIsProvidedButReturnsFalse()
        {
            this.provider.ActionProvider = new TestActionProviderWithResolution(this.FailToResolveSpecificActionOnEntityType);
            var operationWrapper = this.actionProvider.TryResolveServiceAction("Fake", this.entityType);
            operationWrapper.Should().BeNull();
        }

        [TestMethod]
        public void ActionProviderWrapperShouldFallBackToOldInterfaceIfResolutionApiIsNotProvided()
        {
            this.provider.ActionProvider = new TestActionProvider
                                           {
                                               TryResolveServiceActionCallback = this.ResolveSpecificActionOnEntityType,
                                           };
            var operationWrapper = this.actionProvider.TryResolveServiceAction("Fake", this.entityType);
            operationWrapper.Should().NotBeNull();
            operationWrapper.ServiceAction.Should().BeSameAs(this.action);
        }

        [TestMethod]
        public void ActionResolverCanBeSeperateInstanceFromProvider()
        {
            this.provider.ActionProvider = new TestActionProvider
                                           {
                                               TryResolveServiceActionCallback = this.FailToResolveSpecificActionOnEntityType,
                                           };
            this.provider.ActionResolver = new TestActionResolver(ResolveSpecificActionOnEntityType);
            var operationWrapper = this.actionProvider.TryResolveServiceAction("Fake", this.entityType);
            operationWrapper.Should().NotBeNull();
            operationWrapper.ServiceAction.Should().BeSameAs(this.action);
        }

        [TestMethod]
        public void ActionResolverShouldNotBeRequestedFromGetServiceIfNoActionProviderIsFound()
        {
            this.provider.ActionProvider = null;
            this.provider.ActionResolver = new TestActionResolver(ResolveSpecificActionOnEntityType);
            var operationWrapper = this.actionProvider.TryResolveServiceAction("Fake", this.entityType);
            operationWrapper.Should().BeNull();
        }

        [TestMethod]
        public void ActionProviderWrapperShouldFailOnDuplicateOverload()
        {
            var duplicateAction1 = new ServiceAction("Duplicate", ResourceType.GetPrimitiveResourceType(typeof(string)), OperationParameterBindingKind.Sometimes, new[] { new ServiceActionParameter("p1", this.entityType), new ServiceActionParameter("p2", ResourceType.GetPrimitiveResourceType(typeof(string))) }, null);
            duplicateAction1.SetReadOnly();
            var duplicateAction2 = new ServiceAction("Duplicate", ResourceType.GetPrimitiveResourceType(typeof(int)), OperationParameterBindingKind.Sometimes, new[] { new ServiceActionParameter("p1", this.entityType) }, null);
            duplicateAction2.SetReadOnly();
            this.provider.ActionProvider = new TestActionProvider
                                           {
                                               GetByBindingTypeCallback = (context, rt) => new[] { duplicateAction1, duplicateAction2 },
                                           };

            Action getByParameterType = () => this.actionProvider.GetServiceActionsByBindingParameterType(this.entityType);
            getByParameterType.ShouldThrow<DataServiceException>()
                .WithMessage(ErrorStrings.DataServiceActionProviderWrapper_DuplicateAction("Duplicate"))
                .And.StatusCode.Should().Be(500);
        }

        [TestMethod]
        public void ActionProviderWrapperShouldNotLookupActionsPerHierarchyMoreThanOnce()
        {
            int calls = 0;
            this.provider.ActionProvider = new TestActionProvider { GetByBindingTypeCallback = (c, t) => new ServiceAction[0] };
            this.provider.GetAllTypesInHierarchyFunc = t => { calls++; t.Should().BeSameAs(this.entityType); return new[] { this.entityType }; };
            this.actionProvider.GetActionsBoundToAnyTypeInHierarchy(this.entityType).Should().BeEmpty();
            this.actionProvider.GetActionsBoundToAnyTypeInHierarchy(this.entityType).Should().BeEmpty();
            calls.Should().Be(1);
        }

        [TestMethod]
        public void ActionProviderWrapperShouldNotLookupActionsPerTypeMoreThanOnce()
        {
            int calls = 0;
            this.provider.ActionProvider = new TestActionProvider { GetByBindingTypeCallback = (oc, type) => { calls++; type.Should().BeSameAs(this.entityType); return new ServiceAction[0]; } };
            this.actionProvider.GetServiceActionsByBindingParameterType(this.entityType).Should().BeEmpty();
            this.actionProvider.GetServiceActionsByBindingParameterType(this.entityType).Should().BeEmpty();
            calls.Should().Be(1);
        }

        [TestMethod]
        public void ActionProviderWrapperShouldGatherAllActionsForAllTypesInSet()
        {
            var derivedType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, this.entityType, "Fake", "Derived", false);
            derivedType.SetReadOnly();
            var action1 = new ServiceAction("Duplicate", ResourceType.GetPrimitiveResourceType(typeof(string)), OperationParameterBindingKind.Sometimes, new[] { new ServiceActionParameter("p1", this.entityType), new ServiceActionParameter("p2", ResourceType.GetPrimitiveResourceType(typeof(string))) }, null);
            action1.SetReadOnly();
            var action2 = new ServiceAction("Duplicate", ResourceType.GetPrimitiveResourceType(typeof(int)), OperationParameterBindingKind.Sometimes, new[] { new ServiceActionParameter("p1", derivedType) }, null);
            action2.SetReadOnly();
            this.provider.ActionProvider = new TestActionProvider
            {
                GetByBindingTypeCallback = (context, rt) => rt == this.entityType ? new[] { action1 } : new[] { action2 },
            };

            this.provider.GetAllTypesInHierarchyFunc = set => new[] { this.entityType, derivedType };

            this.actionProvider.GetActionsBoundToAnyTypeInHierarchy(this.entityType).Select(wrapper => wrapper.ServiceAction).Should().BeEquivalentTo(action1, action2);
        }

        private bool ResolveSpecificActionOnEntityType(DataServiceOperationContext operationContextFromProduct, ServiceActionResolverArgs resolverArgs, out ServiceAction serviceAction)
        {
            operationContextFromProduct.Should().BeSameAs(this.operationContext);
            resolverArgs.Should().NotBeNull();
            resolverArgs.ServiceActionName.Should().Be(this.action.Name);
            resolverArgs.BindingType.Should().BeSameAs(this.entityType);
            serviceAction = this.action;
            return true;
        }

        private bool ResolveSpecificActionOnEntityType(DataServiceOperationContext operationContextFromProduct, string actionName, out ServiceAction serviceAction)
        {
            operationContextFromProduct.Should().BeSameAs(this.operationContext);
            actionName.Should().Be(this.action.Name);
            serviceAction = this.action;
            return true;
        }

        private bool FailToResolveSpecificActionOnEntityType(DataServiceOperationContext operationContextFromProduct, ServiceActionResolverArgs resolverArgs, out ServiceAction serviceAction)
        {
            operationContextFromProduct.Should().BeSameAs(this.operationContext);
            resolverArgs.Should().NotBeNull();
            resolverArgs.ServiceActionName.Should().Be(this.action.Name);
            resolverArgs.BindingType.Should().BeSameAs(this.entityType);
            serviceAction = null;
            return false;
        }

        private bool FailToResolveSpecificActionOnEntityType(DataServiceOperationContext operationContextFromProduct, string actionName, out ServiceAction serviceAction)
        {
            operationContextFromProduct.Should().BeSameAs(this.operationContext);
            actionName.Should().Be(this.action.Name);
            serviceAction = null;
            return false;
        }

        private delegate bool ServiceResolutionDelegateWithContext(DataServiceOperationContext operationContext, ServiceActionResolverArgs resolverArgs, out ServiceAction serviceAction);

        private class TestActionProviderWithResolution : IDataServiceActionProvider, IDataServiceActionResolver
        {
            private readonly ServiceResolutionDelegateWithContext callback;

            public TestActionProviderWithResolution(ServiceResolutionDelegateWithContext callback)
            {
                this.callback = callback;
            }

            public IEnumerable<ServiceAction> GetServiceActions(DataServiceOperationContext operationContext)
            {
                throw new NotImplementedException();
            }

            public bool TryResolveServiceAction(DataServiceOperationContext operationContext, string serviceActionName, out ServiceAction serviceAction)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<ServiceAction> GetServiceActionsByBindingParameterType(DataServiceOperationContext operationContext, ResourceType bindingParameterType)
            {
                throw new NotImplementedException();
            }

            public IDataServiceInvokable CreateInvokable(DataServiceOperationContext operationContext, ServiceAction serviceAction, object[] parameterTokens)
            {
                throw new NotImplementedException();
            }

            public bool AdvertiseServiceAction(DataServiceOperationContext operationContext, ServiceAction serviceAction, object resourceInstance, bool resourceInstanceInFeed, ref ODataAction actionToSerialize)
            {
                throw new NotImplementedException();
            }

            public bool TryResolveServiceAction(DataServiceOperationContext operationContext, ServiceActionResolverArgs resolverArgs, out ServiceAction serviceAction)
            {
                if (this.callback != null)
                {
                    return this.callback(operationContext, resolverArgs, out serviceAction);
                }

                throw new NotImplementedException();
            }
        }

        private class TestActionResolver : IDataServiceActionResolver
        {
            private readonly ServiceResolutionDelegateWithContext callback;

            public TestActionResolver(ServiceResolutionDelegateWithContext callback)
            {
                this.callback = callback;
            }

            public bool TryResolveServiceAction(DataServiceOperationContext operationContext, ServiceActionResolverArgs resolverArgs, out ServiceAction serviceAction)
            {
                if (this.callback != null)
                {
                    return this.callback(operationContext, resolverArgs, out serviceAction);
                }

                throw new NotImplementedException();
            }
        }

        private class TestDataServiceProvider : DataServiceActionProviderWrapper.IDataServiceProviderWrapperForActions
        {
            public OperationWrapper ValidateOperation(ServiceAction serviceAction)
            {
                return new OperationWrapper(serviceAction);
            }

            public void AddOperationToEdmModel(OperationWrapper actionWrapper)
            {
            }

            public bool TryGetCachedOperationWrapper(string serviceActionName, ResourceType bindingType, out OperationWrapper actionWrapper)
            {
                actionWrapper = null;
                return false;
            }

            public T GetService<T>() where T : class
            {
                if (typeof(T) == typeof(IDataServiceActionResolver))
                {
                    // should only ask for the resolution provider if an action provider is found and it does not implement the resolver interface.
                    this.ActionProvider.Should().NotBeNull();
                    this.ActionProvider.As<IDataServiceActionResolver>().Should().BeNull();

                    return (T)this.ActionResolver;
                }

                typeof(T).Should().Be(typeof(IDataServiceActionProvider));
                return (T)this.ActionProvider;
            }

            public IEnumerable<ResourceType> GetAllTypesInHierarchy(ResourceType startingType)
            {
                if (this.GetAllTypesInHierarchyFunc != null)
                {
                    return this.GetAllTypesInHierarchyFunc(startingType);
                }

                throw new NotImplementedException();
            }

            internal IDataServiceActionResolver ActionResolver { get; set; }

            internal IDataServiceActionProvider ActionProvider { get; set; }

            internal Func<ResourceType, IEnumerable<ResourceType>> GetAllTypesInHierarchyFunc { get; set; }
        }
    }
}
