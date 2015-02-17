//---------------------------------------------------------------------
// <copyright file="OperationCacheTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Strings = Microsoft.OData.Service.Strings;

    [TestClass]
    public class OperationCacheTests
    {
        private readonly ResourceType entityType1;
        private readonly ResourceType entityType2;
        private readonly ServiceAction actionWithBindingParameter1;
        private readonly ServiceAction actionWithBindingParameter2;
        private readonly ServiceOperation serviceOperation;
        private readonly OperationWrapper serviceOperationWrapper;
        private readonly OperationWrapper actionWithBindingParameterWrapper1;
        private readonly OperationWrapper actionWithBindingParameterWrapper2;

        private OperationCache testSubject;
        
        public OperationCacheTests()
        {
            this.entityType1 = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "My.Namespace", "Entity1", false);
            this.entityType2 = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "My.Namespace", "Entity2", false);

            var intType = ResourceType.GetPrimitiveResourceType(typeof(int));
            this.actionWithBindingParameter1 = CreateAction("SameName", intType, this.entityType1);
            this.actionWithBindingParameter2 = CreateAction("SameName", intType, this.entityType2);
            this.serviceOperation = CreateServiceOperation("SameName", intType);

            this.actionWithBindingParameter1.SetReadOnly();
            this.actionWithBindingParameter2.SetReadOnly();
            this.serviceOperation.SetReadOnly();

            this.actionWithBindingParameterWrapper1 = new OperationWrapper(this.actionWithBindingParameter1);
            this.actionWithBindingParameterWrapper2 = new OperationWrapper(this.actionWithBindingParameter2);
            this.serviceOperationWrapper = new OperationWrapper(this.serviceOperation);
        }

        [TestInitialize]
        public void Init()
        {
            this.testSubject = new OperationCache();
            this.testSubject.Add(this.actionWithBindingParameterWrapper1);
            this.testSubject.Add(this.actionWithBindingParameterWrapper2);
            this.testSubject.Add(this.serviceOperationWrapper);
        }

        [TestMethod]
        public void OperationCacheLookupShouldReturnSameInstance()
        {
            OperationWrapper resultWrapper;
            this.testSubject.TryGetWrapper(this.actionWithBindingParameter1, out resultWrapper).Should().BeTrue();
            resultWrapper.Should().BeSameAs(this.actionWithBindingParameterWrapper1);
        }

        [TestMethod]
        public void OperationCacheLookupShouldWorkIfKeyIsAnotherInstance()
        {
            var equivalentAction = CreateAction(this.actionWithBindingParameter1.Name, ResourceType.GetPrimitiveResourceType(typeof(string)), this.entityType1);
            equivalentAction.SetReadOnly();
            OperationWrapper resultWrapper;
            this.testSubject.TryGetWrapper(equivalentAction, out resultWrapper).Should().BeTrue();
            resultWrapper.Should().BeSameAs(this.actionWithBindingParameterWrapper1);
        }

        [TestMethod]
        public void OperationCacheLookupShouldThrowIfActionIsNotReadOnly()
        {
            var equivalentAction = CreateAction(this.actionWithBindingParameter1.Name, ResourceType.GetPrimitiveResourceType(typeof(string)), this.entityType1);
            OperationWrapper resultWrapper;
            Action lookup = () => this.testSubject.TryGetWrapper(equivalentAction, out resultWrapper);
            lookup.ShouldThrow<DataServiceException>().WithMessage(Strings.DataServiceProviderWrapper_ServiceOperationNotReadonly(this.actionWithBindingParameter1.Name));
        }

        [TestMethod]
        public void OperationCacheLookupShouldWorkForServiceOperataion()
        {
            OperationWrapper resultWrapper;
            this.testSubject.TryGetWrapper(this.serviceOperation, out resultWrapper).Should().BeTrue();
            resultWrapper.Should().BeSameAs(this.serviceOperationWrapper);
        }

        [TestMethod]
        public void OperationCacheShouldBeCaseSensitive()
        {
            OperationWrapper resultWrapper;
            this.testSubject.TryGetWrapper(this.actionWithBindingParameter1.Name.ToUpperInvariant(), this.entityType1, out resultWrapper).Should().BeFalse();
        }

        [TestMethod]
        public void OperationCacheShouldReturnCorrectOverload()
        {
            this.actionWithBindingParameter1.Name.Should().Be(this.actionWithBindingParameter2.Name);

            OperationWrapper resultWrapper;
            this.testSubject.TryGetWrapper(this.actionWithBindingParameter1.Name, this.entityType1, out resultWrapper).Should().BeTrue();
            resultWrapper.Should().BeSameAs(this.actionWithBindingParameterWrapper1);

            this.testSubject.TryGetWrapper(this.actionWithBindingParameter1.Name, this.entityType2, out resultWrapper).Should().BeTrue();
            resultWrapper.Should().BeSameAs(this.actionWithBindingParameterWrapper2);
        }

        [TestMethod]
        public void OperationCacheContainsShouldBeTrueForEquivalentAction()
        {
            var equivalentAction = CreateAction(this.actionWithBindingParameter1.Name, ResourceType.GetPrimitiveResourceType(typeof(string)), this.entityType1);
            equivalentAction.SetReadOnly();
            this.testSubject.Contains(equivalentAction).Should().BeTrue();
        }

        [TestMethod]
        public void OperationCacheContainsShouldBeTrueForEquivalentServiceOperation()
        {
            var equivalentServiceOp = CreateServiceOperation(this.serviceOperation.Name, ResourceType.GetPrimitiveResourceType(typeof(string)));
            equivalentServiceOp.SetReadOnly();
            this.testSubject.Contains(equivalentServiceOp).Should().BeTrue();
        }

        [TestMethod]
        public void OperationCacheContainsShouldBeFalseForDifferentAction()
        {
            var differentAction = CreateAction(this.actionWithBindingParameter1.Name + "_Fake", ResourceType.GetPrimitiveResourceType(typeof(string)), this.entityType1);
            differentAction.SetReadOnly();
            this.testSubject.Contains(differentAction).Should().BeFalse();
        }

        [TestMethod]
        public void OperationCacheContainsShouldBeFalseForDifferentServiceOperation()
        {
            var differentServiceOperation = CreateServiceOperation(this.serviceOperation.Name + "_Fake", ResourceType.GetPrimitiveResourceType(typeof(string)));
            differentServiceOperation.SetReadOnly();
            this.testSubject.Contains(differentServiceOperation).Should().BeFalse();
        }

        [TestMethod]
        public void OperationCacheContainsShouldThrowIfActionIsNotReadOnly()
        {
            var equivalentAction = CreateAction(this.actionWithBindingParameter1.Name, ResourceType.GetPrimitiveResourceType(typeof(string)), this.entityType1);
            Action lookup = () => this.testSubject.Contains(equivalentAction);
            lookup.ShouldThrow<DataServiceException>().WithMessage(Strings.DataServiceProviderWrapper_ServiceOperationNotReadonly(this.actionWithBindingParameter1.Name));
        }

        private static ServiceOperation CreateServiceOperation(string operationName, ResourceType returnType)
        {
            return new ServiceOperation(operationName, ServiceOperationResultKind.DirectValue, returnType, null, "GET", null);
        }

        private static ServiceAction CreateAction(string operationName, ResourceType returnType, params ResourceType[] parameterTypes)
        {
            return new ServiceAction(operationName, returnType, OperationParameterBindingKind.Sometimes, parameterTypes.Select((t, i) => new ServiceActionParameter("Param" + i, t)), null);
        }
    }   
}