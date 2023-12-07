//---------------------------------------------------------------------
// <copyright file="EdmUtilTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
using Xunit;

namespace Microsoft.OData.Edm.Tests
{
    /// <summary>
    ///Tests EdmUtils functionalities
    ///</summary>
    public class EdmUtilTests
    {
        private static EdmEntityType customer;
        private static EdmEntityType vipCustomer;
        private static EdmEntityType city;
        private static EdmComplexType complex;
        private static EdmComplexType derivedcomplex;
        private static EdmEntityContainer container;
        private static EdmEntitySet entitySet;
        private static EdmSingleton singleton;

        private static IEdmProperty nameProperty;
        private static IEdmProperty addressProperty;
        private static IEdmProperty myRoadProperty;
        private static EdmNavigationProperty navUnderComplex;
        private static EdmNavigationProperty navUnderCustomer;

        static EdmUtilTests()
        {
            city = new EdmEntityType("NS", "City");

            complex = new EdmComplexType("NS", "Address");
            complex.AddStructuralProperty("Road", EdmCoreModel.Instance.GetString(false));

            navUnderComplex = complex.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "City",
                    Target = city,
                    TargetMultiplicity = EdmMultiplicity.One,
                });

            derivedcomplex = new EdmComplexType("NS", "DerivedAddress", complex);
            derivedcomplex.AddStructuralProperty("MyRoad", EdmCoreModel.Instance.GetString(false));

            customer = new EdmEntityType("NS", "Customer");
            customer.AddKeys(customer.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            customer.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String, isNullable: false);
            customer.AddStructuralProperty("Address", new EdmComplexTypeReference(complex, false));

            navUnderCustomer = customer.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "Cities",
                    Target = city,
                    TargetMultiplicity = EdmMultiplicity.Many,
                });

            vipCustomer = new EdmEntityType("NS", "VipCustomer", customer);

            container = new EdmEntityContainer("NS", "Default");
            entitySet = new EdmEntitySet(container, "Customers", customer);
            singleton = new EdmSingleton(container, "Me", customer);

            nameProperty = customer.DeclaredProperties.Where(x => x.Name == "Name").FirstOrDefault();
            addressProperty = customer.DeclaredProperties.Where(x => x.Name == "Address").FirstOrDefault();
            myRoadProperty = derivedcomplex.DeclaredProperties.Where(x => x.Name == "MyRoad").FirstOrDefault();
        }

        [Fact]
        public void FunctionImportShouldProduceCorrectFullyQualifiedNameAndNotHaveParameters()
        {
            var function = new EdmFunction("d.s", "testFunction", EdmCoreModel.Instance.GetString(true));
            function.AddParameter("param1", EdmCoreModel.Instance.GetString(false));
            var functionImport = new EdmFunctionImport(new EdmEntityContainer("d.s", "container"), "testFunction", function);
            Assert.Equal("d.s.container/testFunction", EdmUtil.FullyQualifiedName(functionImport));
        }

        [Fact]
        public void EntitySetShouldProduceCorrectFullyQualifiedName()
        {
            var entitySet = new EdmEntitySet(new EdmEntityContainer("d.s", "container"), "entitySet", new EdmEntityType("foo", "type"));
            Assert.Equal("d.s.container/entitySet", EdmUtil.FullyQualifiedName(entitySet));
        }

        [Fact]
        public void EdmTargetPathEntitySetAndPropertyShouldProduceCorrectFullyQualifiedName()
        {
            EdmTargetPath target = new EdmTargetPath(container, entitySet, nameProperty);

            Assert.Equal("NS.Default/Customers/Name", EdmUtil.FullyQualifiedName(target));
            Assert.Equal("NS.Default/Customers/Name", target.Path);
        }

        [Fact]
        public void EdmTargetPathEntitySetAndPropertyOnDerivedTypeShouldProduceCorrectFullyQualifiedName()
        {
            EdmTargetPath target = new EdmTargetPath(container, entitySet, vipCustomer, nameProperty);

            Assert.Equal("NS.Default/Customers/NS.VipCustomer/Name", EdmUtil.FullyQualifiedName(target));
            Assert.Equal("NS.Default/Customers/NS.VipCustomer/Name", target.Path);
        }

        [Fact]
        public void EdmTargetPathEntitySetAndNavigationPropertyShouldProduceCorrectFullyQualifiedName()
        {
            EdmTargetPath target = new EdmTargetPath(container, entitySet, navUnderCustomer);

            Assert.Equal("NS.Default/Customers/Cities", EdmUtil.FullyQualifiedName(target));
            Assert.Equal("NS.Default/Customers/Cities", target.Path);
        }

        [Fact]
        public void EdmTargetPathEntitySetAndNavigationPropertyInComplexTypeShouldProduceCorrectFullyQualifiedName()
        {
            EdmTargetPath target = new EdmTargetPath(container, entitySet, addressProperty, navUnderComplex);

            Assert.Equal("NS.Default/Customers/Address/City", EdmUtil.FullyQualifiedName(target));
            Assert.Equal("NS.Default/Customers/Address/City", target.Path);
        }

        [Fact]
        public void EdmTargetPathEntitySetAndNavigationPropertyInDerivedComplexTypeShouldProduceCorrectFullyQualifiedName()
        {
            EdmTargetPath target = new EdmTargetPath(container, entitySet, addressProperty, derivedcomplex, myRoadProperty);

            Assert.Equal("NS.Default/Customers/Address/NS.DerivedAddress/MyRoad", EdmUtil.FullyQualifiedName(target));
            Assert.Equal("NS.Default/Customers/Address/NS.DerivedAddress/MyRoad", target.Path);
        }

        [Fact]
        public void EdmTargetPathSingletonAndNavigationPropertyInComplexTypeShouldProduceCorrectFullyQualifiedName()
        {
            EdmTargetPath target = new EdmTargetPath(container, singleton, addressProperty, navUnderComplex);

            Assert.Equal("NS.Default/Me/Address/City", EdmUtil.FullyQualifiedName(target));
            Assert.Equal("NS.Default/Me/Address/City", target.Path);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData("a. ", false)]
        [InlineData(".com", false)]
        [InlineData("com.", false)]
        [InlineData(".", false)]
        [InlineData("com", false)]
        [InlineData("a.b.com", true)]
        [InlineData(" a.b.com", true)]
        [InlineData("a.b.com ", true)]
        [InlineData(" a . b . c ", true)]
        [InlineData("a . . c", false)]
        [InlineData("a .. c", false)]
        public void IsQualifiedName_Test(string name, bool expected)
        {
            var actual = EdmUtil.IsQualifiedName(name);
            Assert.Equal(expected, actual);
        }
        
        [Theory]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData("a.B", false)]
        [InlineData("_abc", true)]
        [InlineData("_ABC", true)]
        [InlineData("AB/CD", false)]
        [InlineData("ab_CD", true)]
        [InlineData("AB12CD", true)]
        [InlineData("12ABCD", false)]
        public void IsValidUndottedName_Test(string name, bool expected)
        {
            var actual = EdmUtil.IsValidUndottedName(name);
            Assert.Equal(expected, actual);
        }
    }
}
