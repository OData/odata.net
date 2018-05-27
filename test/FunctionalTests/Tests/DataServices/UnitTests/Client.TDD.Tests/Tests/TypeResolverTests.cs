﻿//---------------------------------------------------------------------
// <copyright file="TypeResolverTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Metadata;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TypeResolverTests
    {
        private ClientEdmModel clientModel;
        private EdmModel serverModel;
        private IEdmEntityType clientEntityType;
        private EdmStructuralProperty clientIdProperty;

        [TestInitialize]
        public void Init()
        {
            this.serverModel = new EdmModel();
            this.clientModel = new ClientEdmModel(ODataProtocolVersion.V4);
            this.clientEntityType = (IEdmEntityType)this.clientModel.GetOrCreateEdmType(typeof(TestClientEntityType));
            this.clientIdProperty = new EdmStructuralProperty(this.clientEntityType, "Id", EdmCoreModel.Instance.GetInt32(false));

            var serverType = new EdmEntityType("FQ.NS", "TestServerType");
            this.serverModel.AddElement(serverType);
            var serverContainer = new EdmEntityContainer("FQ.NS", "Container");
            this.serverModel.AddElement(serverContainer);
            serverContainer.AddEntitySet("Entities", serverType);

            var serverType2 = new EdmEntityType("FQ.NS", "TestServerType2"); 
            this.serverModel.AddElement(serverType2);
            serverType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "Navigation", Target = serverType2, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
        }

        [TestMethod]
        public void ResolutionFromTypeShouldNotRequireTypeToNameResolverWhenServerModelIsPresent()
        {
            var testSubject = this.CreateTypeResolver(true, resolveNameFromType: t => null);
            testSubject.ResolveExpectedTypeForReading(typeof(TestClientEntityType)).Should().BeNull();
        }

        [TestMethod]
        public void ResolutionFromTypeShouldUseServerModelWhenPresentForEntityTypes()
        {
            var entityType = new EdmEntityType("Fake", "MyServerType");
            this.serverModel.AddElement(entityType);

            var testSubject = this.CreateTypeResolver(
                true,
                resolveNameFromType: t =>
                {
                    t.Should().Be(typeof(TestClientEntityType));
                    return "Fake.MyServerType";
                });

            testSubject.ResolveExpectedTypeForReading(typeof(TestClientEntityType)).Should().BeSameAs(entityType);
        }

        [TestMethod]
        public void ResolutionFromTypeShouldReturnClientTypeWhenServerModelIsNotPresent()
        {
            var testSubject = this.CreateTypeResolver(false);
            testSubject.ResolveExpectedTypeForReading(typeof(TestClientEntityType)).Should().BeSameAs(this.clientEntityType);
        }

        [TestMethod]
        public void ResolutionFromTypeShouldReturnClientTypeWhenServerModelNotPresentForComplexTypes()
        {
            this.TestNonEntityResolutionFromType<TestClientComplexType>(false);
        }

        [TestMethod]
        public void ResolutionFromTypeShouldReturnClientTypeWhenServerModelNotPresentForComplexCollectionTypes()
        {
            this.TestNonEntityResolutionFromType<List<TestClientComplexType>>(false);
        }

        [TestMethod]
        public void ResolutionFromTypeShouldUseServerModelWhenPresentForComplexTypes()
        {
            var complexType = new EdmComplexType("Fake", "MyServerType");
            this.serverModel.AddElement(complexType);

            var testSubject = this.CreateTypeResolver(
                true,
                resolveNameFromType: t =>
                {
                    t.Should().Be(typeof(TestClientComplexType));
                    return "Fake.MyServerType";
                });

            testSubject.ResolveExpectedTypeForReading(typeof(TestClientComplexType)).Should().BeSameAs(complexType);
        }

        [TestMethod]
        public void ResolutionFromTypeShouldUseServerModelWhenPresentForComplexCollectionTypes()
        {
            var complexType = new EdmComplexType("Fake", "MyServerType");
            this.serverModel.AddElement(complexType);

            var testSubject = this.CreateTypeResolver(
                true,
                resolveNameFromType: t =>
                {
                    t.Should().Be(typeof(TestClientComplexType));
                    return "Fake.MyServerType";
                });

            IEdmType result = testSubject.ResolveExpectedTypeForReading(typeof(List<TestClientComplexType>));
            result.Should().BeAssignableTo<IEdmCollectionType>();
            result.As<IEdmCollectionType>().ElementType.Definition.Should().BeSameAs(complexType);
        }

        [TestMethod]
        public void ResolutionFromTypeShouldReturnNullForCollectionTypeIfElementTypeCannotBeResolved()
        {
            // Regression coverage for: Query for collection of complex values fails on JSON client when type-name resolver is not defined.
            var complexType = new EdmComplexType("Fake", "MyServerType");
            this.serverModel.AddElement(complexType);

            var testSubject = this.CreateTypeResolver(true, resolveNameFromType: t => null);

            testSubject.ResolveExpectedTypeForReading(typeof(List<TestClientComplexType>)).Should().BeNull();
        }

        [TestMethod]
        public void ResolutionFromTypeShouldNotUseServerModelForPrimitiveTypes()
        {
            this.TestNonEntityResolutionFromType<int>(true);
            this.TestNonEntityResolutionFromType<int>(false);
        }

        [TestMethod]
        public void ResolutionFromTypeShouldNotUseServerModelForPrimitiveCollectionTypes()
        {
            this.TestNonEntityResolutionFromType<List<int>>(true);
            this.TestNonEntityResolutionFromType<List<int>>(false);
        }

        [TestMethod]
        public void TypeResolverShouldTreatAllPropertiesAsDefinedWhenServerModelIsNotAvailable()
        {
            TypeResolver testSubject = this.CreateTypeResolver(false);
            EdmStructuralProperty fakeProperty = new EdmStructuralProperty(new EdmEntityType("Fake", "Type"), "FakeProperty", EdmCoreModel.Instance.GetInt32(false));
            testSubject.ShouldWriteClientTypeForOpenServerProperty(fakeProperty, "SomeServerType").Should().BeFalse();
        }

        [TestMethod]
        public void TypeResolverShouldTreatAllPropertiesAsDefinedWhenServerTypeCannotBeFound()
        {
            TypeResolver testSubject = this.CreateTypeResolver(true);
            testSubject.ShouldWriteClientTypeForOpenServerProperty(this.clientIdProperty, "SomeServerType").Should().BeFalse();
        }

        [TestMethod]
        public void TypeResolverShouldTreatAllPropertiesAsDefinedWhenDefiningTypeIsComplex()
        {
            TypeResolver testSubject = this.CreateTypeResolver(true);
            EdmStructuralProperty fakeProperty = new EdmStructuralProperty(new EdmComplexType("Fake", "Type"), "FakeProperty", EdmCoreModel.Instance.GetInt32(false));
            testSubject.ShouldWriteClientTypeForOpenServerProperty(fakeProperty, "SomeServerType").Should().BeFalse();
        }

        [TestMethod]
        public void TypeResolverShouldTreatAllPropertiesAsDefinedWhenServerTypeNameIsUnknown()
        {
            var testSubject = this.CreateTypeResolver(true);
            testSubject.ShouldWriteClientTypeForOpenServerProperty(this.clientIdProperty, null).Should().BeFalse();
        }

        [TestMethod]
        public void TypeResolverShouldAskServerModelWhetherPropertyIsDefinedIfMatchIsFound()
        {
            var testSubject = this.CreateTypeResolver(true);
            testSubject.ShouldWriteClientTypeForOpenServerProperty(this.clientIdProperty, "FQ.NS.TestServerType").Should().BeTrue();
        }

        [TestMethod]
        public void TypeResolverShouldNotFindEntitySetBaseTypeIfItHasNoServerModel()
        {
            var testSubject = this.CreateTypeResolver(false);
            string serverTypeName;
            testSubject.TryResolveEntitySetBaseTypeName("Entities", out serverTypeName).Should().BeFalse();
            serverTypeName.Should().BeNull();
        }

        [TestMethod]
        public void TypeResolverShouldNotFindEntitySetBaseTypeIfItHasAServerModelButSetDoesNotExist()
        {
            var testSubject = this.CreateTypeResolver(true);
            string serverTypeName;
            testSubject.TryResolveEntitySetBaseTypeName("Fake", out serverTypeName).Should().BeFalse();
            serverTypeName.Should().BeNull();
        }

        [TestMethod]
        public void TypeResolverShouldFindEntitySetBaseTypeIfItHasAServerModel()
        {
            var testSubject = this.CreateTypeResolver(true);
            string serverTypeName;
            testSubject.TryResolveEntitySetBaseTypeName("Entities", out serverTypeName).Should().BeTrue();
            serverTypeName.Should().Be("FQ.NS.TestServerType");
        }

        [TestMethod]
        public void TypeResolverShouldNotFindNavigationTargetTypeIfItHasNoServerModel()
        {
            var testSubject = this.CreateTypeResolver(false);
            string serverTypeName;
            testSubject.TryResolveNavigationTargetTypeName("FQ.NS.TestServerType", "Navigation", out serverTypeName).Should().BeFalse();
            serverTypeName.Should().BeNull();
        }

        [TestMethod]
        public void TypeResolverShouldNotFindNavigationTargetTypeIfItHasAServerModelButSourceTypeDoesNotExist()
        {
            var testSubject = this.CreateTypeResolver(true);
            string serverTypeName;
            testSubject.TryResolveNavigationTargetTypeName("FQ.NS.FakeServerType", "Navigation", out serverTypeName).Should().BeFalse();
            serverTypeName.Should().BeNull();
        }
        
        [TestMethod]
        public void TypeResolverShouldNotFindNavigationTargetTypeIfItHasAServerModelButNavigationDoesNotExist()
        {
            var testSubject = this.CreateTypeResolver(true);
            string serverTypeName;
            testSubject.TryResolveNavigationTargetTypeName("FQ.NS.TestServerType", "FakeNavigation", out serverTypeName).Should().BeFalse();
            serverTypeName.Should().BeNull();
        }
        
        [TestMethod]
        public void TypeResolverShouldFindNavigationTargetTypeIfItHasAServerModel()
        {
            var testSubject = this.CreateTypeResolver(true);
            string serverTypeName;
            testSubject.TryResolveNavigationTargetTypeName("FQ.NS.TestServerType", "Navigation", out serverTypeName).Should().BeTrue();
            serverTypeName.Should().Be("FQ.NS.TestServerType2");
        }

        [TestMethod]
        public void TypeResolverShouldFindNotNavigationTargetTypeIfItHasAServerModelButServerTypeNameIsNull()
        {
            var testSubject = this.CreateTypeResolver(true);
            string serverTypeName;
            testSubject.TryResolveNavigationTargetTypeName(null, "Navigation", out serverTypeName).Should().BeFalse();
            serverTypeName.Should().BeNull();
        }
        
        private void TestNonEntityResolutionFromType<T>(bool includeServerModel)
        {
            var clientNonEntityType = this.clientModel.GetOrCreateEdmType(typeof(T));
            var testSubject = this.CreateTypeResolver(includeServerModel);
            testSubject.ResolveExpectedTypeForReading(typeof(T)).Should().BeSameAs(clientNonEntityType);
        }

        private TypeResolver CreateTypeResolver(bool includeServerModel, Func<string, Type> resolveTypeFromName = null, Func<Type, string> resolveNameFromType = null)
        {
            if (resolveTypeFromName == null)
            {
                resolveTypeFromName = s => { throw new Exception(); };
            }

            if (resolveNameFromType == null)
            {
                resolveNameFromType = t => { throw new Exception(); };
            }

            var testSubject = new TypeResolver(this.clientModel, resolveTypeFromName, resolveNameFromType, includeServerModel ? this.serverModel : null);
            return testSubject;
        }

        [Key("Id")]
        public class TestClientEntityType
        {
            public int Id { get; set; }
        }

        public class TestClientDerivedEntityType : TestClientEntityType
        {
        }

        private class TestClientComplexType
        {
            public int Value { get; set; }
        }
    }
}
