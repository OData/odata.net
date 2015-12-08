//---------------------------------------------------------------------
// <copyright file="EdmSingletonTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
using FluentAssertions;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Annotations;
using Microsoft.OData.Edm.Library.Values;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Library
{
    public class EdmSingletonTests
    {
        private EdmEntityContainer entityContainer;
        private EdmEntityType customerType;
        private EdmEntityType orderType;
        private EdmEntityType productType;
        private readonly string myNamespace = "NS";

        public EdmSingletonTests()
        {
            this.entityContainer = new EdmEntityContainer(myNamespace, "Container");
            this.customerType = new EdmEntityType(myNamespace, "Customer");
            this.orderType = new EdmEntityType(myNamespace, "Order");
            this.productType = new EdmEntityType(myNamespace, "Product");
        }

        [Fact]
        public void EdmSingletonBasicAttributeTest()
        {
            EdmSingleton singleton = new EdmSingleton(this.entityContainer, "VIP", customerType);
            singleton.Container.Should().Be(this.entityContainer);
            singleton.ContainerElementKind.Should().Be(EdmContainerElementKind.Singleton);
            singleton.EntityType().Should().Be(customerType);          
        }

        [Fact]
        public void EdmSingletonBasicNavigationPropertyBindingTest()
        {
            var internalOrderProperty = customerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo(){Name = "InternalOrder", Target = this.orderType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne});
            var externalOrderProperty = customerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo(){Name = "ExternalOrders", Target = this.orderType, TargetMultiplicity = EdmMultiplicity.Many});
            var customerProductProperty = customerType.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "Products", Target = this.productType, TargetMultiplicity = EdmMultiplicity.Many }, 
                new EdmNavigationPropertyInfo() { Name = "Buyer", Target = this.customerType, TargetMultiplicity = EdmMultiplicity.One });


            var orderSet = new EdmEntitySet(this.entityContainer, "Orders", this.orderType);
            var productSet = new EdmEntitySet(this.entityContainer, "Products", this.productType);

            var vipCustomer = new EdmSingleton(this.entityContainer, "VIP", this.customerType);

            vipCustomer.NavigationPropertyBindings.Should().HaveCount(0);

            vipCustomer.AddNavigationTarget(internalOrderProperty, orderSet);
            vipCustomer.AddNavigationTarget(externalOrderProperty, orderSet);
            vipCustomer.AddNavigationTarget(customerProductProperty, productSet);

            vipCustomer.NavigationPropertyBindings.Should().HaveCount(3)
                .And.Contain(m => m.NavigationProperty == internalOrderProperty && m.Target == orderSet)
                .And.Contain(m => m.NavigationProperty == externalOrderProperty && m.Target == orderSet)
                .And.Contain(m=>m.NavigationProperty ==  customerProductProperty && m.Target == productSet);

            vipCustomer.FindNavigationTarget(internalOrderProperty).Should().Be(orderSet);
            vipCustomer.FindNavigationTarget(externalOrderProperty).Should().Be(orderSet);
            vipCustomer.FindNavigationTarget(customerProductProperty).Should().Be(productSet);

            productSet.AddNavigationTarget(customerProductProperty.Partner, vipCustomer);
            productSet.FindNavigationTarget(customerProductProperty.Partner).Should().Be(vipCustomer);
        }

        [Fact]
        public void EdmSingletonAnnotationTests() 
        {
            EdmModel model = new EdmModel();

            EdmStructuralProperty customerProperty = new EdmStructuralProperty(customerType, "Name", EdmCoreModel.Instance.GetString(false));
            customerType.AddProperty(customerProperty);
            model.AddElement(this.customerType);

            EdmSingleton vipCustomer = new EdmSingleton(this.entityContainer, "VIP", this.customerType);

            EdmTerm term = new EdmTerm(myNamespace, "SingletonAnnotation", EdmPrimitiveTypeKind.String);
            var annotation = new EdmAnnotation(vipCustomer, term, new EdmStringConstant("Singleton Annotation"));
            model.AddVocabularyAnnotation(annotation);

            var singletonAnnotation = vipCustomer.VocabularyAnnotations(model).Single();
            Assert.Equal(vipCustomer, singletonAnnotation.Target);
            Assert.Equal("SingletonAnnotation", singletonAnnotation.Term.Name);

            singletonAnnotation = model.FindDeclaredVocabularyAnnotations(vipCustomer).Single();
            Assert.Equal(vipCustomer, singletonAnnotation.Target);
            Assert.Equal("SingletonAnnotation", singletonAnnotation.Term.Name);

            EdmTerm propertyTerm = new EdmTerm(myNamespace, "SingletonPropertyAnnotation", EdmPrimitiveTypeKind.String);
            var propertyAnnotation = new EdmAnnotation(customerProperty, propertyTerm, new EdmStringConstant("Singleton Property Annotation"));
            model.AddVocabularyAnnotation(propertyAnnotation);

            var singletonPropertyAnnotation = customerProperty.VocabularyAnnotations(model).Single();
            Assert.Equal(customerProperty, singletonPropertyAnnotation.Target);
            Assert.Equal("SingletonPropertyAnnotation", singletonPropertyAnnotation.Term.Name);
        }
    }
}
