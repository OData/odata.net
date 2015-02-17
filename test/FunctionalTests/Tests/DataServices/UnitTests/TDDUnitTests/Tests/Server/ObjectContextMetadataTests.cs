//---------------------------------------------------------------------
// <copyright file="ObjectContextMetadataTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using Microsoft.OData.Service.Providers;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace AstoriaUnitTests.Tests.Server
{
    [TestClass]
    public class ObjectContextMetadataTests
    {
        // Entity Framework metadata used in all of the tests, since metadata is immutable, this can be populated once for the entire test run
        private static MetadataWorkspace metadataWorkspace;

        // Class being tested, populated for each test case
        private ObjectContextMetadata objectContextMetadata;

        private const string csdlModelNamespace = "TestModel";
        private const string testCsdl =
        @"<Schema xmlns=""http://schemas.microsoft.com/ado/2008/09/edm"" xmlns:cg=""http://schemas.microsoft.com/ado/2006/04/codegeneration"" xmlns:store=""http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator"" Namespace=""" + csdlModelNamespace + @""" Alias=""Self"" xmlns:annotation=""http://schemas.microsoft.com/ado/2009/02/edm/annotation"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
            <EntityContainer Name=""SelfContainer"">
              <EntitySet Name=""Orders"" EntityType=""Self.Order"" />
              <EntitySet Name=""OrderDetails"" EntityType=""Self.OrderDetail"" />
              <AssociationSet Name=""Order_OrderDetail"" Association=""Self.Order_OrderDetail"">
                <End Role=""Order"" EntitySet=""Orders"" />
                <End Role=""OrderDetail"" EntitySet=""OrderDetails"" />
              </AssociationSet>
            </EntityContainer>
            <EntityType Name=""Order"">
              <Key>
                <PropertyRef Name=""OrderID"" />
              </Key>
              <Property Type=""Int32"" Name=""OrderID"" Nullable=""false"" annotation:StoreGeneratedPattern=""Identity"" />
              <Property Type=""DateTime"" Name=""OrderDate"" Nullable=""false"" />
              <Property Type=""String"" Name=""Description"" Nullable=""false"" m:MimeType=""text/plain"" />
              <NavigationProperty Name=""OrderDetails"" Relationship=""Self.Order_OrderDetail"" FromRole=""Order"" ToRole=""OrderDetail"" />
              <Property Name=""ShippingAddress"" Type=""Self.Address"" Nullable=""false"" />
            </EntityType>
            <EntityType Name=""OrderDetail"">
              <Key>
                <PropertyRef Name=""OrderDetailID"" />
              </Key>
              <Property Type=""Int32"" Name=""OrderDetailID"" Nullable=""false"" annotation:StoreGeneratedPattern=""Identity"" />
              <Property Type=""String"" Name=""ItemName"" Nullable=""false"" />
              <Property Type=""Int32"" Name=""Quantity"" Nullable=""false"" />
              <NavigationProperty Name=""Order"" Relationship=""Self.Order_OrderDetail"" FromRole=""OrderDetail"" ToRole=""Order"" />
            </EntityType>
            <Association Name=""Order_OrderDetail"">
              <End Type=""Self.Order"" Role=""Order"" Multiplicity=""1"" />
              <End Type=""Self.OrderDetail"" Role=""OrderDetail"" Multiplicity=""*"" />
            </Association>
            <EntityType Name=""SpecialOrder"" BaseType=""Self.Order"" >
              <Property Type=""String"" Name=""CustomInstructions"" Nullable=""false"" />
            </EntityType>
            <ComplexType Name=""Address"" >
              <Property Type=""String"" Name=""StreetAddress"" Nullable=""false"" />
              <Property Type=""Int32"" Name=""ZipCode"" Nullable=""false"" />
            </ComplexType>
          </Schema>";

        #region Initialization

        [ClassInitialize]
        public static void InitializeMetadataWorkspace(TestContext testContext)
        {
            StringReader sr = new StringReader(testCsdl);
            XmlReader reader = XmlReader.Create(sr);
            metadataWorkspace = new MetadataWorkspace();
            EdmItemCollection edmItemCollection = new EdmItemCollection(new XmlReader[] { reader });
            metadataWorkspace.RegisterItemCollection(edmItemCollection);
            metadataWorkspace.RegisterItemCollection(new ObjectItemCollection());
            metadataWorkspace.LoadFromAssembly(Assembly.GetExecutingAssembly());            
        }

        [TestInitialize]
        public void InitializeObjectContextMetadata()
        {
            objectContextMetadata = new ObjectContextMetadata(metadataWorkspace);
        }

        #endregion

        #region Tests

        [TestMethod]
        public void GetObjectContextTypeShouldReturnProviderTypeWithBaseTypeAndCorrectMembers()
        {
            IProviderType entityType = TestGetProviderType("Order");
            VerifyOrderMembers(entityType);
        }

        [TestMethod]
        public void GetObjectContextTypeShouldReturnProviderTypeWithCorrectDerivedEntityTypeAndMembers()
        {
            IProviderType entityType = TestGetProviderType("SpecialOrder");
            VerifySpecialOrderMembers(entityType);
        }

        [TestMethod]
        public void ComplexProviderTypeShouldReturnCorrectMembers()
        {
            IProviderType complexType = TestGetProviderType("Address");
            VerifyAddressMembers(complexType);
        }

        [TestMethod]
        public void VerifyObjectContextTypeFailsWhenGettingUnknownType()
        {
            string unknownTypeName = "UnknownType";
            Action test = () => GetProviderType(unknownTypeName);
            // TODO: Match against actual error
            test.ShouldThrow<ArgumentException>();
        }

        [TestMethod]
        public void ReferenceNavigationPropertyObjectContextMemberReturnsCorrectReferenceNav()
        {
            IProviderType entityType = TestGetProviderType("OrderDetail");

            IProviderMember member = GetProviderMember(entityType.Members, "Order");
            VerifyReferenceNavigationMember(member, "Order");
            
            // rest of members are just primitives and are covered by testing other entity types, so don't need to repeat here
        }

        [TestMethod]
        public void GetObjectContextMetadataGetClrTypeOnEntityTypeShouldReturnCorrectBaseType()
        {
            TestGetClrType(typeof(Order));
        }

        [TestMethod]
        public void GetObjectContextMetadataGetClrTypeOnEntityTypeShouldReturnCorrectDerivedType()
        {
            TestGetClrType(typeof(SpecialOrder));
        }

        [TestMethod]
        public void GetObjectContextMetadataGetClrTypeOnComplexTypeShouldReturnCorrectMembers()
        {
            TestGetClrType(typeof(Address));
        }

        #endregion

        #region Helper Methods

        private IProviderType TestGetProviderType(string edmTypeName)
        {
            IProviderType providerType = GetProviderType(edmTypeName);
            VerifyProviderType(edmTypeName, providerType);
            return providerType;
        }

        private IProviderType GetProviderType(string typeName)
        {
            string fullyQualifiedName = GetFullyQualifiedModelName(typeName);
            return objectContextMetadata.GetProviderType(fullyQualifiedName);
        }

        private void TestGetClrType(Type expectedType)
        {
            Type entityClrType = GetClrType(expectedType.Name);
            VerifyClrType(expectedType, entityClrType);
        }

        private static IEnumerable<IProviderMember> GetMembers(IProviderType providerType, int expectedMemberCount)
        {
            IEnumerable<IProviderMember> members = providerType.Members;
            Assert.AreEqual(expectedMemberCount, members.Count(), "Provider type {0} does not have the expected number of members.", providerType.Name);
            return members;
        }

        private IProviderMember GetProviderMember(IEnumerable<IProviderMember> actualMembers, string expectedName)
        {
            IProviderMember actualMember = actualMembers.SingleOrDefault(m => m.Name == expectedName);
            Assert.IsNotNull(actualMember, "Unable to find a member with the name {0}.", expectedName);
            return actualMember;
        }

        private static string GetFullyQualifiedModelName(string typeName)
        {
            return String.Format("{0}.{1}", csdlModelNamespace, typeName);
        }

        private static StructuralType GetStructuralType(string edmTypeName)
        {
            return metadataWorkspace.GetItem<StructuralType>(GetFullyQualifiedModelName(edmTypeName), DataSpace.CSpace);
        }

        private Type GetClrType(string edmTypeName)
        {
            StructuralType structuralType = GetStructuralType(edmTypeName);
            Type clrType = objectContextMetadata.GetClrType(structuralType);
            return clrType;
        }

        #endregion

        #region Verification

        private static void VerifyProviderType(string typeName, IProviderType providerType)
        {
            Assert.IsNotNull(providerType, "Unable to get the provider type {0} from the metadata workspace.", typeName);
            Assert.AreEqual(typeName, providerType.Name, "IProviderType.Name is incorrect for type {0}.", typeName);
        }

        private void VerifyOrderMembers(IProviderType entityType)
        {
            IEnumerable<IProviderMember> members = GetMembers(entityType, 5);

            IProviderMember member;
            member = GetProviderMember(members, "OrderID");
            VerifyPrimitiveMember(member, "Int32", expectedIsKey : true);
            member = GetProviderMember(members, "OrderDate");
            VerifyPrimitiveMember(member, "DateTime");
            member = GetProviderMember(members, "Description");
            VerifyPrimitiveMember(member, "String", expectedMimeType : "text/plain");
            member = GetProviderMember(members, "ShippingAddress");
            VerifyComplexMember(member, "Address");
            member = GetProviderMember(members, "OrderDetails");
            VerifyCollectionNavigationMember(member, "OrderDetail");
        }

        private void VerifySpecialOrderMembers(IProviderType entityType)
        {
            IEnumerable<IProviderMember> members = GetMembers(entityType, 1);

            IProviderMember member = GetProviderMember(members, "CustomInstructions");
            VerifyPrimitiveMember(member, "String");
        }

        private void VerifyAddressMembers(IProviderType complexType)
        {
            IEnumerable<IProviderMember> members = GetMembers(complexType, 2);

            IProviderMember member;
            member = GetProviderMember(members, "StreetAddress");
            VerifyPrimitiveMember(member, "String");
            member = GetProviderMember(members, "ZipCode");
            VerifyPrimitiveMember(member, "Int32");
        }        

        private void VerifyPrimitiveMember(IProviderMember actualMember, string expectedEdmTypeName, string expectedMimeType = null, bool expectedIsKey = false)
        {
            VerifyMember(actualMember, expectedIsKey, expectedEdmTypeName, BuiltInTypeKind.PrimitiveType, expectedMimeType, null);
        }

        private void VerifyComplexMember(IProviderMember actualMember, string expectedEdmTypeName)
        {
            VerifyMember(actualMember, false, expectedEdmTypeName, BuiltInTypeKind.ComplexType, null, null);
        }

        private void VerifyCollectionNavigationMember(IProviderMember actualMember, string collectionItemTypeName)
        {
            EntityType expectedCollectionItemType = (EntityType)GetStructuralType(collectionItemTypeName);
            string expectedEdmTypeName = String.Format("collection[{0}(Nullable=True,DefaultValue=)]", GetFullyQualifiedModelName(collectionItemTypeName));
            VerifyMember(actualMember, false, expectedEdmTypeName, BuiltInTypeKind.CollectionType, null, expectedCollectionItemType);
        }       

        private void VerifyReferenceNavigationMember(IProviderMember actualMember, string referenceTypeName)
        {
            VerifyMember(actualMember, false, referenceTypeName, BuiltInTypeKind.EntityType, null, null);
        }

        private void VerifyMember(IProviderMember actualMember, bool expectedIsKey, string expectedEdmTypeName, BuiltInTypeKind expectedEdmTypeKind, string expectedMimeType, EntityType expectedCollectionType)
        {
            string memberName = actualMember.Name;
            Assert.AreEqual(expectedEdmTypeKind, actualMember.EdmTypeKind, "IProviderMember.EdmTypeKind is not the expected value for the member {0}.", memberName);
            Assert.AreEqual(expectedIsKey, actualMember.IsKey, "IProviderMember.IsKey is not the expected value for the member {0}.", memberName);
            Assert.AreEqual(expectedEdmTypeName, actualMember.EdmTypeName, "IProviderMember.EdmTypeName is not the expected value for the member {0}.", memberName);
            Assert.AreEqual(expectedMimeType, actualMember.MimeType, "IProviderMember.MimeType is not the expected value for the member {0}.", memberName);
            Assert.AreEqual(expectedCollectionType, actualMember.CollectionItemType, "IProviderMember.ExpectedCollectionType is not the expected value for the member {0}.", memberName);
        }
        
        private static void VerifyClrType(Type expectedClrType, Type actualClrType)
        {
            Assert.IsNotNull(actualClrType, "Failed to get CLR type for type {0}", expectedClrType.Name);
            Assert.AreEqual(expectedClrType, actualClrType, "Wrong CLR type");
        }

        #endregion

        #region Metadata Types

        public class Order
        {
            public int OrderID { get; set; }
            public DateTime OrderDate { get; set; }
            public string Description { get; set; }
            public List<OrderDetail> OrderDetails { get; set; }
            public Address ShippingAddress { get; set; }
        }

        public class SpecialOrder : Order
        {
            public string CustomInstructions { get; set; }
        }

        public class OrderDetail
        {
            public int OrderDetailID { get; set; }
            public string ItemName { get; set; }
            public int Quantity { get; set; }
            public Order Order { get; set; }
        }

        public class Address
        {
            public string StreetAddress { get; set; }
            public int ZipCode { get; set; }
        }

        #endregion
    }
}
