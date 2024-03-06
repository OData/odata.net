//---------------------------------------------------------------------
// <copyright file="EdmSingletonTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
using Microsoft.OData.Edm.Vocabularies;
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
            Assert.Same(this.entityContainer, singleton.Container);
            Assert.Equal(EdmContainerElementKind.Singleton, singleton.ContainerElementKind);
            Assert.Same(customerType, singleton.EntityType);
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

            Assert.Empty(vipCustomer.NavigationPropertyBindings);

            vipCustomer.AddNavigationTarget(internalOrderProperty, orderSet);
            vipCustomer.AddNavigationTarget(externalOrderProperty, orderSet);
            vipCustomer.AddNavigationTarget(customerProductProperty, productSet);

            Assert.Equal(3, vipCustomer.NavigationPropertyBindings.Count());
            Assert.Contains(vipCustomer.NavigationPropertyBindings, m => m.NavigationProperty == internalOrderProperty && m.Target == orderSet);
            Assert.Contains(vipCustomer.NavigationPropertyBindings, m => m.NavigationProperty == externalOrderProperty && m.Target == orderSet);
            Assert.Contains(vipCustomer.NavigationPropertyBindings, m => m.NavigationProperty ==  customerProductProperty && m.Target == productSet);

            Assert.Same(orderSet, vipCustomer.FindNavigationTarget(internalOrderProperty));
            Assert.Same(orderSet, vipCustomer.FindNavigationTarget(externalOrderProperty));
            Assert.Same(productSet, vipCustomer.FindNavigationTarget(customerProductProperty));

            productSet.AddNavigationTarget(customerProductProperty.Partner, vipCustomer);
            Assert.Same(vipCustomer, productSet.FindNavigationTarget(customerProductProperty.Partner));
        }

        [Fact]
        public void EdmSingletonAdvancedContainedNavigationPropertyBindingTest()
        {
            // Creates model with:
            // -Singleton "s":
            //    contained navigation properties "a1" and "a2" of type aType,
            //    contained navigation proeprty b of type bType
            //    non-contained navigation property c of type cType
            // -Type aType:
            //    contained navigation property to "b" of bType
            // -Type bType:
            //    non-contained navigation property to "c" of cType
            // -Type dType derives from bType:
            //    contained navigation property to "a" of aType
            //  Each c navigation property is bound to a different entity set
            // Validates that FindNavigationTarget identifies the correct entity set for each path

            EdmModel model = new EdmModel();
            var sType = model.AddEntityType("TestNS", "sType");
            var aType = model.AddEntityType("TestNS", "aType");
            aType.AddKeys(aType.AddStructuralProperty("key", EdmPrimitiveTypeKind.Int32));
            var bType = model.AddEntityType("TestNS", "bType");
            bType.AddKeys(bType.AddStructuralProperty("key", EdmPrimitiveTypeKind.Int32));
            var cType = model.AddEntityType("TestNS", "cType");
            cType.AddKeys(cType.AddStructuralProperty("key", EdmPrimitiveTypeKind.Int32));
            var dType = model.AddEntityType("TestNS", "dType", bType);
            dType.AddKeys(dType.AddStructuralProperty("key", EdmPrimitiveTypeKind.Int32));

            var a1 = new EdmNavigationPropertyInfo()
            {
                Name = "a1",
                Target = aType,
                TargetMultiplicity = EdmMultiplicity.Many,
                ContainsTarget = true
            };

            var a2 = new EdmNavigationPropertyInfo()
            {
                Name = "a2",
                Target = aType,
                TargetMultiplicity = EdmMultiplicity.Many,
                ContainsTarget = true
            };

            var b = new EdmNavigationPropertyInfo()
            {
                Name = "b",
                Target = bType,
                TargetMultiplicity = EdmMultiplicity.Many,
                ContainsTarget = true
            };

            var c = new EdmNavigationPropertyInfo()
            {
                Name = "c",
                Target = cType,
                TargetMultiplicity = EdmMultiplicity.Many,
                ContainsTarget = false
            };

            var stoa1NavProp = sType.AddUnidirectionalNavigation(a1);
            var stoa2NavProp = sType.AddUnidirectionalNavigation(a2);
            var stobNavProp = sType.AddUnidirectionalNavigation(b);
            var stocNavProp = sType.AddUnidirectionalNavigation(c);
            var atobNavProp = aType.AddUnidirectionalNavigation(b);
            var btocNavProp = bType.AddUnidirectionalNavigation(c);
            var dtoa1NavProp = dType.AddUnidirectionalNavigation(a1);

            var container = model.AddEntityContainer("TestNS", "container");
            var s = container.AddSingleton("s", sType);
            var sc = container.AddEntitySet("sc", cType);
            var sbc = container.AddEntitySet("sbc", cType);
            var sa1bc = container.AddEntitySet("sa1bc", cType);
            var sa2bc = container.AddEntitySet("sa2bc", cType);
            var sa2bda1bc = container.AddEntitySet("sa2bda1bc", cType);

            s.AddNavigationTarget(stocNavProp, sc, new EdmPathExpression("c"));
            s.AddNavigationTarget(btocNavProp, sbc, new EdmPathExpression("b/c"));
            s.AddNavigationTarget(btocNavProp, sa1bc, new EdmPathExpression("a1/b/c"));
            s.AddNavigationTarget(btocNavProp, sa2bc, new EdmPathExpression("a2/b/c"));
            s.AddNavigationTarget(btocNavProp, sa2bda1bc, new EdmPathExpression("a2/b/TestNS.dType/a1/b/c"));

            var foundSc = s.FindNavigationTarget(stocNavProp);
            Assert.Same(sc, foundSc);
            var foundSb = s.FindNavigationTarget(stobNavProp);
            Assert.True(foundSb is IEdmContainedEntitySet);
            var foundSbc = foundSb.FindNavigationTarget(btocNavProp);
            Assert.Same(sbc, foundSbc);
            var foundSa1 = s.FindNavigationTarget(stoa1NavProp);
            Assert.True(foundSa1 is IEdmContainedEntitySet);
            var foundSa1b = foundSa1.FindNavigationTarget(atobNavProp);
            Assert.True(foundSa1b is IEdmContainedEntitySet);
            var foundSa1bc = foundSa1b.FindNavigationTarget(btocNavProp);
            Assert.Same(sa1bc, foundSa1bc);
            var foundSa2 = s.FindNavigationTarget(stoa2NavProp);
            Assert.True(foundSa2 is IEdmContainedEntitySet);
            var foundSa2b = foundSa2.FindNavigationTarget(atobNavProp);
            Assert.True(foundSa2b is IEdmContainedEntitySet);
            var foundSa2bc = foundSa2b.FindNavigationTarget(btocNavProp);
            Assert.Same(sa2bc, foundSa2bc);
            var foundSa2bda1 = foundSa2b.FindNavigationTarget(dtoa1NavProp, new EdmPathExpression("TestNS.dType/a1"));
            Assert.True(foundSa2bda1 is IEdmContainedEntitySet);
            var foundSa2bda1b = foundSa2bda1.FindNavigationTarget(atobNavProp);
            Assert.True(foundSa2bda1b is IEdmContainedEntitySet);
            var foundSa2bda1bc = foundSa2bda1b.FindNavigationTarget(btocNavProp);
            Assert.Same(sa2bda1bc, foundSa2bda1bc);
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
            var annotation = new EdmVocabularyAnnotation(vipCustomer, term, new EdmStringConstant("Singleton Annotation"));
            model.AddVocabularyAnnotation(annotation);

            var singletonAnnotation = vipCustomer.VocabularyAnnotations(model).Single();
            Assert.Equal(vipCustomer, singletonAnnotation.Target);
            Assert.Equal("SingletonAnnotation", singletonAnnotation.Term.Name);

            singletonAnnotation = model.FindDeclaredVocabularyAnnotations(vipCustomer).Single();
            Assert.Equal(vipCustomer, singletonAnnotation.Target);
            Assert.Equal("SingletonAnnotation", singletonAnnotation.Term.Name);

            EdmTerm propertyTerm = new EdmTerm(myNamespace, "SingletonPropertyAnnotation", EdmPrimitiveTypeKind.String);
            var propertyAnnotation = new EdmVocabularyAnnotation(customerProperty, propertyTerm, new EdmStringConstant("Singleton Property Annotation"));
            model.AddVocabularyAnnotation(propertyAnnotation);

            var singletonPropertyAnnotation = customerProperty.VocabularyAnnotations(model).Single();
            Assert.Equal(customerProperty, singletonPropertyAnnotation.Target);
            Assert.Equal("SingletonPropertyAnnotation", singletonPropertyAnnotation.Term.Name);
        }
    }
}
