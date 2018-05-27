//---------------------------------------------------------------------
// <copyright file="SelectedOperationsCacheTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using System.Collections.Generic;
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
    public class SelectedOperationsCacheTests
    {
        private ServiceAction action;
        private ResourceType entityType;
        private ResourceSetWrapper resourceSetWrapper;
        private SelectedOperationsCache testSubject;
        private OperationWrapper actionWrapper;
        private ResourceType baseType;
        private ResourceType derivedType;
        private ServiceAction derivedAction;
        private OperationWrapper derivedActionWrapper;

        [TestInitialize]
        public void Init()
        {
            this.baseType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Fake.NS", "BaseType", false) {CanReflectOnInstanceType = false};
            this.baseType.AddProperty(new ResourceProperty("Id", ResourcePropertyKind.Key | ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(int))) { CanReflectOnInstanceTypeProperty = false });
            this.baseType.SetReadOnly();
            
            this.entityType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, this.baseType, "Fake.NS", "Type", false) {CanReflectOnInstanceType = false};
            this.entityType.SetReadOnly();

            this.derivedType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, this.entityType, "Fake.NS", "DerivedType", false) { CanReflectOnInstanceType = false };
            this.derivedType.SetReadOnly();

            var resourceSet = new ResourceSet("Set", this.entityType);
            resourceSet.SetReadOnly();
            this.resourceSetWrapper = ResourceSetWrapper.CreateForTests(resourceSet);

            this.action = new ServiceAction("Fake", ResourceType.GetPrimitiveResourceType(typeof(int)), OperationParameterBindingKind.Sometimes, new[] {new ServiceActionParameter("p1", this.entityType)}, null);
            this.action.SetReadOnly();
            this.actionWrapper = new OperationWrapper(action);

            this.derivedAction = new ServiceAction("Fake", ResourceType.GetPrimitiveResourceType(typeof(int)), OperationParameterBindingKind.Sometimes, new[] { new ServiceActionParameter("p1", this.derivedType) }, null);
            this.derivedAction.SetReadOnly();
            this.derivedActionWrapper = new OperationWrapper(derivedAction);

            this.testSubject = new SelectedOperationsCache();
        }

        [TestMethod]
        public void AddingToSelectedOperationsCacheShouldReturnFalseForEmptySet()
        {
            this.testSubject.AddSelectedOperations(this.entityType, new OperationWrapper[0]).Should().BeFalse();
        }

        [TestMethod]
        public void ActionSelectedAndBoundToGivenTypeShouldBeReturned()
        {
            this.testSubject.AddSelectedOperations(this.entityType, new[] { this.actionWrapper }).Should().BeTrue();
            this.testSubject.GetSelectedOperations(this.entityType).Should().BeEquivalentTo(this.actionWrapper);
        }

        [TestMethod]
        public void ActionSelectedForBaseTypeAndBoundToGivenTypeShouldBeReturned()
        {
            this.testSubject.AddSelectedOperations(this.baseType, new[] { this.actionWrapper }).Should().BeTrue();
            this.testSubject.GetSelectedOperations(this.entityType).Should().BeEquivalentTo(this.actionWrapper);
        }

        [TestMethod]
        public void ActionSelectedForButBoundToMoreDerivedTypeShouldNotBeReturned()
        {
            this.testSubject.AddSelectedOperations(this.entityType, new[] { this.actionWrapper, this.derivedActionWrapper }).Should().BeTrue();
            this.testSubject.GetSelectedOperations(this.entityType).Should().BeEquivalentTo(this.actionWrapper);
        }

        [TestMethod]
        public void MultipleActionsSelectedForBaseTypeButBoundToMoreDerivedTypeShouldBeReturned()
        {
            this.testSubject.AddSelectedOperations(this.baseType, new[] { this.actionWrapper, this.derivedActionWrapper }).Should().BeTrue();
            this.testSubject.GetSelectedOperations(this.derivedType).Should().BeEquivalentTo(this.actionWrapper, this.derivedActionWrapper);
        }
    }
}