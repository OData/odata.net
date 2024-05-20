//---------------------------------------------------------------------
// <copyright file="TargetHelperTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Microsoft.OData.Edm.Tests.ExtensionMethods
{
    public class TargetHelperTests
    {
        private static TargetPathSampleModel sampleModel;
        private static IEdmModel edmModel;
        static TargetHelperTests()
        {
            sampleModel = new TargetPathSampleModel();

            edmModel = sampleModel.Model;
        }

        [Fact]
        public void GetTargetSegmentsForEntitySetProperty()
        {
            string targetPath = "NS.Default/Customers/Name";
            List<IEdmElement> segments = edmModel.GetTargetSegments(targetPath.Split('/'), true).ToList();

            Assert.Equal(3, segments.Count);
            Assert.IsAssignableFrom<IEdmEntityContainer>(segments[0]);
            Assert.IsAssignableFrom<IEdmEntitySet>(segments[1]);
            Assert.IsAssignableFrom<IEdmStructuralProperty>(segments[2]);
            Assert.Equal("Customers", (segments[1] as IEdmEntitySet).Name);
            Assert.Equal("Name", (segments[2] as IEdmStructuralProperty).Name);
        }

        [Fact]
        public void GetTargetSegmentsForEntitySetPropertyOnDerivedType()
        {
            string targetPath = "NS.Default/Customers/NS.VipCustomer/Name";
            List<IEdmElement> segments = edmModel.GetTargetSegments(targetPath.Split('/'), true).ToList();

            Assert.Equal(4, segments.Count);
            Assert.IsAssignableFrom<IEdmEntityContainer>(segments[0]);
            Assert.IsAssignableFrom<IEdmEntitySet>(segments[1]);
            Assert.IsAssignableFrom<IEdmSchemaType>(segments[2]);
            Assert.IsAssignableFrom<IEdmStructuralProperty>(segments[3]);
            Assert.Equal("Customers", (segments[1] as IEdmEntitySet).Name);
            Assert.Equal("VipCustomer", (segments[2] as IEdmSchemaType).Name);
            Assert.Equal("Name", (segments[3] as IEdmStructuralProperty).Name);
        }

        [Fact]
        public void GetTargetSegmentsForEntitySetNavigationProperty()
        {
            string targetPath = "NS.Default/Customers/Cities";
            List<IEdmElement> segments = edmModel.GetTargetSegments(targetPath.Split('/'), true).ToList();

            Assert.Equal(3, segments.Count);
            Assert.IsAssignableFrom<IEdmEntityContainer>(segments[0]);
            Assert.IsAssignableFrom<IEdmEntitySet>(segments[1]);
            Assert.IsAssignableFrom<IEdmNavigationProperty>(segments[2]);
            Assert.Equal("Customers", (segments[1] as IEdmEntitySet).Name);
            Assert.Equal("Cities", (segments[2] as IEdmNavigationProperty).Name);
        }

        [Fact]
        public void GetTargetSegmentsForEntitySetNavigationPropertyUnderComplexProperty()
        {
            string targetPath = "NS.Default/Customers/Address/City";
            List<IEdmElement> segments = edmModel.GetTargetSegments(targetPath.Split('/'), true).ToList();

            Assert.Equal(4, segments.Count);
            Assert.IsAssignableFrom<IEdmEntityContainer>(segments[0]);
            Assert.IsAssignableFrom<IEdmEntitySet>(segments[1]);
            Assert.IsAssignableFrom<IEdmStructuralProperty>(segments[2]);
            Assert.IsAssignableFrom<IEdmNavigationProperty>(segments[3]);
            Assert.Equal("Customers", (segments[1] as IEdmEntitySet).Name);
            Assert.Equal("Address", (segments[2] as IEdmStructuralProperty).Name);
            Assert.Equal("City", (segments[3] as IEdmNavigationProperty).Name);
        }

        [Fact]
        public void GetTargetSegmentsForEntitySetNavigationPropertyUnderDerivedComplexProperty()
        {
            string targetPath = "NS.Default/Customers/Address/NS.DerivedAddress/MyRoad";
            List<IEdmElement> segments = edmModel.GetTargetSegments(targetPath.Split('/'), true).ToList();

            Assert.Equal(5, segments.Count);
            Assert.IsAssignableFrom<IEdmEntityContainer>(segments[0]);
            Assert.IsAssignableFrom<IEdmEntitySet>(segments[1]);
            Assert.IsAssignableFrom<IEdmProperty>(segments[2]);
            Assert.IsAssignableFrom<IEdmSchemaType>(segments[3]);
            Assert.IsAssignableFrom<IEdmStructuralProperty>(segments[4]);
            Assert.Equal("Customers", (segments[1] as IEdmEntitySet).Name);
            Assert.Equal("Address", (segments[2] as IEdmProperty).Name);
            Assert.Equal("DerivedAddress", (segments[3] as IEdmSchemaType).Name);
            Assert.Equal("MyRoad", (segments[4] as IEdmStructuralProperty).Name);
        }

        [Fact]
        public void GetTargetSegmentsForSingletonNavigationPropertyUnderComplexProperty()
        {
            string targetPath = "NS.Default/Me/Address/City";
            List<IEdmElement> segments = edmModel.GetTargetSegments(targetPath.Split('/'), true).ToList();

            Assert.Equal(4, segments.Count);
            Assert.IsAssignableFrom<IEdmEntityContainer>(segments[0]);
            Assert.IsAssignableFrom<IEdmSingleton>(segments[1]);
            Assert.IsAssignableFrom<IEdmStructuralProperty>(segments[2]);
            Assert.IsAssignableFrom<IEdmNavigationProperty>(segments[3]);
            Assert.Equal("Me", (segments[1] as IEdmSingleton).Name);
            Assert.Equal("Address", (segments[2] as IEdmStructuralProperty).Name);
            Assert.Equal("City", (segments[3] as IEdmNavigationProperty).Name);
        }

        [Fact]
        public void GetTargetSegmentsForSingletonNavigationPropertyUnderNavigationProperty()
        {
            string targetPath = "NS.Default/Me/Cities/NightClubs";
            List<IEdmElement> segments = edmModel.GetTargetSegments(targetPath.Split('/'), true).ToList();

            Assert.Equal(4, segments.Count);
            Assert.IsAssignableFrom<IEdmEntityContainer>(segments[0]);
            Assert.IsAssignableFrom<IEdmSingleton>(segments[1]);
            Assert.IsAssignableFrom<IEdmNavigationProperty>(segments[2]);
            Assert.IsAssignableFrom<IEdmNavigationProperty>(segments[3]);
            Assert.Equal("Me", (segments[1] as IEdmSingleton).Name);
            Assert.Equal("Cities", (segments[2] as IEdmNavigationProperty).Name);
            Assert.Equal("NightClubs", (segments[3] as IEdmNavigationProperty).Name);
        }

        [Fact]
        public void GetTargetSegmentsForInValidTypeCastThrows()
        {
            string targetPath = "NS.Default/Customers/NS.City/Name";

            // Act & Assert
            Action action = () => edmModel.GetTargetSegments(targetPath.Split('/'), true).ToList();

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(action);
            Assert.Equal(Strings.TypeCast_HierarchyNotFollowed(sampleModel.EntitySet, sampleModel.City), exception.Message);
        }

        [Fact]
        public void GetTargetSegmentsForTwoTypeCastThrows()
        {
            string targetPath = "NS.Default/Customers/NS.VipCustomer/NS.City/Name";

            // Act & Assert
            Action action = () => edmModel.GetTargetSegments(targetPath.Split('/'), true).ToList();

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(action);
            Assert.Equal(Strings.TypeCast_HierarchyNotFollowed(sampleModel.VipCustomer, sampleModel.City), exception.Message);
        }
    }

    internal class TargetPathSampleModel
    {
        public TargetPathSampleModel()
        {
            this.Model = new EdmModel();
            this.Address = new EdmComplexType("NS", "Address");
            this.Address.AddStructuralProperty("Road", EdmCoreModel.Instance.GetString(false));
            this.Model.AddElement(this.Address);

            this.DerivedAddress = new EdmComplexType("NS", "DerivedAddress", this.Address);
            this.DerivedAddress.AddStructuralProperty("MyRoad", EdmCoreModel.Instance.GetString(false));
            this.Model.AddElement(this.DerivedAddress);

            this.Customer = new EdmEntityType("NS", "Customer");
            this.Customer.AddKeys(this.Customer.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            this.Customer.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String, isNullable: false);
            this.Customer.AddStructuralProperty("Address", new EdmComplexTypeReference(this.Address, false));
            this.Model.AddElement(this.Customer);

            this.VipCustomer = new EdmEntityType("NS", "VipCustomer", this.Customer);
            this.Model.AddElement(this.VipCustomer);

            this.City = new EdmEntityType("NS", "City");
            this.City.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String, isNullable: false);
            this.Model.AddElement(this.City);

            this.Nightclub = new EdmEntityType("NS", "Nightclub");
            this.Nightclub.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String, isNullable: false);
            this.Model.AddElement(this.Nightclub);

            this.NavUnderComplex = this.Address.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "City",
                    Target = this.City,
                    TargetMultiplicity = EdmMultiplicity.One,
                });

            this.NavUnderCustomer = this.Customer.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "Cities",
                    Target = this.City,
                    TargetMultiplicity = EdmMultiplicity.Many,
                });

            this.NavUnderCity = this.City.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "NightClubs",
                    Target = this.Nightclub,
                    TargetMultiplicity = EdmMultiplicity.Many,
                });

            this.NameProperty = this.Customer.DeclaredProperties.Where(x => x.Name == "Name").FirstOrDefault();
            this.AddressProperty = this.Customer.DeclaredProperties.Where(x => x.Name == "Address").FirstOrDefault();

            this.Container = new EdmEntityContainer("NS", "Default");
            this.EntitySet = new EdmEntitySet(this.Container, "Customers", this.Customer);
            this.Singleton = new EdmSingleton(this.Container, "Me", this.Customer);
            this.Container.AddElement(this.EntitySet);
            this.Container.AddElement(this.Singleton);

            this.Model.AddElement(this.Container);
        }

        public EdmModel Model { get; private set; }
        public EdmEntityContainer Container { get; private set; }
        public EdmEntitySet EntitySet { get; private set; }
        public EdmSingleton Singleton { get; private set; }
        public EdmEntityType Customer { get; private set; }
        public EdmEntityType VipCustomer { get; private set; }
        public EdmEntityType City { get; private set; }
        public EdmEntityType Nightclub { get; private set; }
        public EdmComplexType Address { get; private set; }
        public EdmComplexType DerivedAddress { get; private set; }
        public IEdmProperty NameProperty { get; private set; }
        public IEdmProperty AddressProperty { get; private set; }
        public EdmNavigationProperty NavUnderComplex { get; private set; }
        public EdmNavigationProperty NavUnderCustomer { get; private set; }
        public EdmNavigationProperty NavUnderCity { get; private set; }
    }
}
