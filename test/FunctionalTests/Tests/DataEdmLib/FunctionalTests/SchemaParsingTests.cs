//---------------------------------------------------------------------
// <copyright file="SchemaParsingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Csdl.CsdlSemantics;
    using Microsoft.OData.Edm.Csdl.Parsing.Ast;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SchemaParsingTests : EdmLibTestCaseBase
    {
        [TestMethod]
        public void SchemaReadEmptyModel()
        {
            var csdl = @"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm""/>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");
        }

        [TestMethod]
        public void SchemaReadEmptyModel2()
        {
            var csdl = @"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm""></Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");
        }

        [TestMethod]
        public void SchemaReadSimpleModel()
        {
            var csdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""C1"">
      <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
    </EntityContainer>
    <EntityType Name=""Customer"">
      <Key>
          <PropertyRef Name=""CustomerID"" />
      </Key>
      <Property Name=""CustomerID"" Type=""String"" Nullable=""false"" />
    </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            Assert.AreEqual("C1", model.EntityContainer.Name, "model.EntityContainers.First().Name = C1");
            Assert.AreEqual("Customers", model.EntityContainer.Elements.Single().Name, "model.EntityContainers.Single().Elements.Single().Name = Customers");
            Assert.AreEqual("NS1.Customer", model.SchemaElements.Single(e => e.FullName() == "NS1.Customer").FullName(), "model.SchemaElements.Single().FullName() = NS1.Customer");
            Assert.AreEqual("CustomerID", ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer")).DeclaredStructuralProperties().Single().Name, "model.SchemaElements.Single().DeclaredStructuralProperties.Single().Name = CustomerID");
        }

        [TestMethod]
        public void SchemaReadNonNullableCollection()
        {
            var csdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""C1"">
      <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
    </EntityContainer>
    <EntityType Name=""Customer"">
      <Key>
          <PropertyRef Name=""CustomerID"" />
      </Key>
      <Property Name=""CustomerID"" Type=""String"" Nullable=""false"" />
      <Property Name=""Names"" Type=""Collection(String)"" Nullable=""false"" />
    </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            Assert.AreEqual("C1", model.EntityContainer.Name, "model.EntityContainers.First().Name = C1");
            Assert.AreEqual("Customers", model.EntityContainer.Elements.Single().Name, "model.EntityContainers.Single().Elements.Single().Name = Customers");
            Assert.AreEqual("NS1.Customer", model.SchemaElements.Single(e => e.FullName() == "NS1.Customer").FullName(), "model.SchemaElements.Single().FullName() = NS1.Customer");
            Assert.AreEqual(
                "CustomerID",
                ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer")).DeclaredStructuralProperties().First().Name, "model.SchemaElements.Single().DeclaredStructuralProperties.Single().Name = CustomerID");
            Assert.AreEqual(
                "Names",
                ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer")).DeclaredStructuralProperties().Last().Name, "model.SchemaElements.Single().DeclaredStructuralProperties.Single().Name = Names");

            IEdmEntityType customer = ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer"));
            IEdmStructuralProperty names = customer.DeclaredStructuralProperties().Last();
            IEdmTypeReference typeRef = names.Type;
            Assert.IsFalse(typeRef.IsNullable);
            IEdmCollectionType type = typeRef.Definition as IEdmCollectionType;
            Assert.IsNotNull(type);
            Assert.AreEqual(EdmTypeKind.Collection, type.TypeKind);
            IEdmTypeReference elementType = type.ElementType;
            Assert.IsFalse(elementType.IsNullable);
        }

        [TestMethod]
        public void SchemaReadNullableCollection()
        {
            var csdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""C1"">
      <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
    </EntityContainer>
    <EntityType Name=""Customer"">
      <Key>
          <PropertyRef Name=""CustomerID"" />
      </Key>
      <Property Name=""CustomerID"" Type=""String"" Nullable=""false"" />
      <Property Name=""Names"" Type=""Collection(String)"" Nullable=""true"" />
    </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            Assert.AreEqual("C1", model.EntityContainer.Name, "model.EntityContainers.First().Name = C1");
            Assert.AreEqual("Customers", model.EntityContainer.Elements.Single().Name, "model.EntityContainers.Single().Elements.Single().Name = Customers");
            Assert.AreEqual("NS1.Customer", model.SchemaElements.Single(e => e.FullName() == "NS1.Customer").FullName(), "model.SchemaElements.Single().FullName() = NS1.Customer");
            Assert.AreEqual(
                "CustomerID",
                ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer")).DeclaredStructuralProperties().First().Name, "model.SchemaElements.Single().DeclaredStructuralProperties.Single().Name = CustomerID");
            Assert.AreEqual(
                "Names",
                ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer")).DeclaredStructuralProperties().Last().Name, "model.SchemaElements.Single().DeclaredStructuralProperties.Single().Name = Names");

            IEdmEntityType customer = ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer"));
            IEdmStructuralProperty names = customer.DeclaredStructuralProperties().Last();
            IEdmCollectionTypeReference typeRef = names.Type as IEdmCollectionTypeReference;
            Assert.IsNotNull(typeRef);
            Assert.IsTrue(typeRef.IsNullable);
            IEdmCollectionType type = typeRef.Definition as IEdmCollectionType;
            Assert.IsNotNull(type);
            Assert.AreEqual(EdmTypeKind.Collection, type.TypeKind);
            IEdmTypeReference elementType = type.ElementType;
            Assert.IsTrue(elementType.IsNullable);
        }

        [TestMethod]
        public void SchemaReadCollectionNavigationProperty()
        {
            var csdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""C1"">
        <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
    </EntityContainer>
    <EntityType Name=""Customer"">
        <Key>
            <PropertyRef Name=""CustomerID"" />
        </Key>
        <Property Name=""CustomerID"" Type=""String"" Nullable=""false"" />
        <NavigationProperty Name=""Pets"" Type=""Collection(NS1.Pet)"" />
    </EntityType>
    <EntityType Name=""Pet"">
        <Key>
            <PropertyRef Name=""PetId"" />
        </Key>
        <Property Name=""PetId"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""OwnerId"" Type=""Int32"" Nullable=""false"" />
    </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            bool result = model.Validate(out errors);
            Assert.IsTrue(result);
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType customer = ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer"));
            IEdmNavigationProperty pets = customer.DeclaredNavigationProperties().Single();
            IEdmTypeReference typeRef = pets.Type;
            Assert.IsTrue(typeRef.IsNullable);
            IEdmCollectionType type = typeRef.Definition as IEdmCollectionType;
            Assert.IsNotNull(type);
            Assert.AreEqual(EdmTypeKind.Collection, type.TypeKind);
            IEdmTypeReference elementType = type.ElementType;
            Assert.IsFalse(elementType.IsNullable);
        }

        [TestMethod]
        public void SchemaReadNullableCollectionNavigationPropertyShouldFail()
        {
            var csdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""C1"">
      <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
    </EntityContainer>
    <EntityType Name=""Customer"">
      <Key>
          <PropertyRef Name=""CustomerID"" />
      </Key>
      <Property Name=""CustomerID"" Type=""String"" Nullable=""false"" />
      <NavigationProperty Name=""Pets"" Type=""Collection(NS1.Pet)"" Nullable=""true"" />
    </EntityType>
    <EntityType Name=""Pet"">
     <Key>
      <PropertyRef Name=""PetId"" />
    </Key>
    <Property Name=""PetId"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""OwnerId"" Type=""Int32"" Nullable=""false"" />
  </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(!errors.Any(), "No errors");

            bool result = model.Validate(out errors);
            Assert.IsFalse(result);
            Assert.IsTrue(errors.Count() == 1, "Has errors");
            Assert.AreEqual(EdmErrorCode.NavigationPropertyWithCollectionTypeCannotHaveNullableAttribute, errors.First().ErrorCode);
        }

        [TestMethod]
        public void SchemaReadNonNullableCollectionNavigationPropertyShouldFail()
        {
            var csdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""C1"">
      <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
    </EntityContainer>
    <EntityType Name=""Customer"">
      <Key>
          <PropertyRef Name=""CustomerID"" />
      </Key>
      <Property Name=""CustomerID"" Type=""String"" Nullable=""false"" />
      <NavigationProperty Name=""Pets"" Type=""Collection(NS1.Pet)"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""Pet"">
     <Key>
      <PropertyRef Name=""PetId"" />
    </Key>
    <Property Name=""PetId"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""OwnerId"" Type=""Int32"" Nullable=""false"" />
  </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(!errors.Any(), "No errors");

            bool result = model.Validate(out errors);
            Assert.IsFalse(result);
            Assert.IsTrue(errors.Count() == 1, "Has errors");
            Assert.AreEqual(EdmErrorCode.NavigationPropertyWithCollectionTypeCannotHaveNullableAttribute, errors.First().ErrorCode);
        }

        [TestMethod]
        public void SchemaReadEntityContainer()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Hot"" Alias=""Fuzz"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Address"" Type=""String"" MaxLength=""100"" />
    <NavigationProperty Name=""Pet"" Type=""Fuzz.Pet"" Nullable=""false"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""PetId"" />
    </Key>
    <Property Name=""PetId"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""OwnerId"" Type=""Int32"" Nullable=""false"" />
  </EntityType>
  <EntityContainer Name=""Wild"">
    <EntitySet Name=""Pets"" EntityType=""Hot.Pet"" />
    <EntitySet Name=""People"" EntityType=""Hot.Person"">
      <NavigationPropertyBinding Path=""Pet"" Target=""Pets"" />
    </EntitySet>
  </EntityContainer>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityContainer wild = model.EntityContainer;

            Assert.AreEqual("Wild", wild.Name, "Wild: correct name");

            Assert.IsTrue(wild.Elements.Count() == 2, "Wild: correct number of Elements");
            Assert.AreEqual("Pets", wild.Elements.First().Name, "Wild: correct first element");
            Assert.AreEqual(EdmContainerElementKind.EntitySet, wild.Elements.First().ContainerElementKind, "Wild: first element type");
            Assert.AreEqual("Hot.Pet", ((IEdmEntitySet)wild.Elements.First()).EntityType().FullName(), "Wild: element type for first entityset");

            Assert.AreEqual("People", wild.Elements.ElementAt(1).Name, "Wild: correct second element");
            Assert.AreEqual(EdmContainerElementKind.EntitySet, wild.Elements.ElementAt(1).ContainerElementKind, "Zero: second element type");
            Assert.AreEqual("Hot.Person", ((IEdmEntitySet)wild.Elements.ElementAt(1)).EntityType().FullName(), "Zero: element type for second entityset");

            IEdmEntityType person = (IEdmEntityType)model.FindType("Hot.Person");
            IEdmEntityType pet = (IEdmEntityType)model.FindType("Hot.Pet");
            IEdmEntitySet wildpeople = wild.FindEntitySet("People");
            IEdmEntitySet wildpets = wild.FindEntitySet("Pets");

            Assert.AreEqual("Hot.Wild", wildpets.Container.FullName(), "Correct container name");
        }

        [TestMethod]
        public void SchemaReadNonContainedEntityInContainedEntity()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Hot"" Alias=""Fuzz"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Address"" Type=""String"" MaxLength=""100"" />
    <NavigationProperty Name=""Pet"" Type=""Fuzz.Pet"" Nullable=""false"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""PetId"" />
    </Key>
    <Property Name=""PetId"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""OwnerId"" Type=""Int32"" Nullable=""false"" />
    <NavigationProperty Name=""Master"" Type=""Fuzz.Person"" Nullable=""false"" />
  </EntityType>
  <EntityContainer Name=""Wild"">
    <EntitySet Name=""Pets"" EntityType=""Hot.Pet"" />
    <EntitySet Name=""People"" EntityType=""Hot.Person"" >
        <NavigationPropertyBinding Path=""Pet/Master"" Target=""People"" />
    </EntitySet>
  </EntityContainer>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");
            bool validated = model.Validate(out errors);

            Assert.IsTrue(validated, "validated");

            IEdmEntityContainer container = model.EntityContainer;
            IEdmEntitySet peopleEntitySet = container.FindEntitySet("People");
            IEdmEntityType petEntityType = (IEdmEntityType)model.FindType("Hot.Pet");
            IEdmNavigationProperty masterNavigationProperty = petEntityType.FindProperty("Master") as IEdmNavigationProperty;
            Assert.IsNotNull(masterNavigationProperty);
            IEdmNavigationSource navigationSource = peopleEntitySet.FindNavigationTarget(masterNavigationProperty, new EdmPathExpression("Pet/Master"));
            Assert.AreEqual(navigationSource, peopleEntitySet);
            IEdmEntityType peopleEntityType = (IEdmEntityType)model.FindType("Hot.Person");
            IEdmNavigationProperty petNavigationProperty = peopleEntityType.FindProperty("Pet") as IEdmNavigationProperty;
            IEdmNavigationSource containedNavigationSource = peopleEntitySet.FindNavigationTarget(petNavigationProperty);
            Assert.IsTrue(containedNavigationSource is IEdmContainedEntitySet);
            IEdmNavigationSource peopleNavigationSource = containedNavigationSource.FindNavigationTarget(masterNavigationProperty);
            Assert.AreSame(peopleNavigationSource, navigationSource);
        }

        [TestMethod]
        public void SchemaReadNonContainedEntityInContainedEntityWithWrongPath()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Hot"" Alias=""Fuzz"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Address"" Type=""String"" MaxLength=""100"" />
    <NavigationProperty Name=""Pet"" Type=""Fuzz.Pet"" Nullable=""false"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""PetId"" />
    </Key>
    <Property Name=""PetId"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""OwnerId"" Type=""Int32"" Nullable=""false"" />
    <NavigationProperty Name=""Master"" Type=""Fuzz.Person"" Nullable=""false"" />
  </EntityType>
  <EntityContainer Name=""Wild"">
    <EntitySet Name=""Pets"" EntityType=""Hot.Pet"" />
    <EntitySet Name=""People"" EntityType=""Hot.Person"" >
        <NavigationPropertyBinding Path=""Pet/Master1/Master"" Target=""People"" />
    </EntitySet>
  </EntityContainer>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");
            bool validated = model.Validate(out errors);

            Assert.IsFalse(validated, "Validation should fail because Property Master1 does not exist on type Hot.Pet");
            Assert.AreEqual(errors.Count(), 1);
            Assert.AreEqual(errors.FirstOrDefault().ErrorCode, EdmErrorCode.BadUnresolvedNavigationPropertyPath);
        }

        [TestMethod]
        public void SchemaReadNonContainedDerivedEntityInContainedEntity()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Hot"" Alias=""Fuzz"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Address"" Type=""String"" MaxLength=""100"" />
    <NavigationProperty Name=""Pet"" Type=""Fuzz.Pet"" Nullable=""false"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""PetId"" />
    </Key>
    <Property Name=""PetId"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""OwnerId"" Type=""Int32"" Nullable=""false"" />
  </EntityType>
  <EntityType Name=""Dog"" BaseType=""Hot.Pet"">
    <NavigationProperty Name=""Master"" Type=""Fuzz.Person"" Nullable=""false"" />
  </EntityType>
  <EntityContainer Name=""Wild"">
    <EntitySet Name=""Pets"" EntityType=""Hot.Pet"" />
    <EntitySet Name=""People"" EntityType=""Hot.Person"" >
        <NavigationPropertyBinding Path=""Pet/Hot.Dog/Master"" Target=""People"" />
    </EntitySet>
  </EntityContainer>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");
            bool validated = model.Validate(out errors);

            Assert.IsTrue(validated, "validated");

            IEdmEntityContainer container = model.EntityContainer;
            IEdmEntitySet peopleEntitySet = container.FindEntitySet("People");
            IEdmEntityType dogEntityType = (IEdmEntityType)model.FindType("Hot.Dog");
            IEdmNavigationProperty masterNavigationProperty = dogEntityType.FindProperty("Master") as IEdmNavigationProperty;
            Assert.IsNotNull(masterNavigationProperty);
            IEdmNavigationSource navigationSource = peopleEntitySet.FindNavigationTarget(masterNavigationProperty, new EdmPathExpression("Pet/Hot.Dog/Master"));
            Assert.AreEqual(navigationSource, peopleEntitySet);
        }

        [TestMethod]
        public void SchemaReadSingleton()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Hot"" Alias=""Fuzz"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Address"" Type=""String"" MaxLength=""100"" />
    <NavigationProperty Name=""Pet"" Type=""Fuzz.Pet"" Nullable=""false"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""PetId"" />
    </Key>
    <Property Name=""PetId"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""OwnerId"" Type=""Int32"" Nullable=""false"" />
    <NavigationProperty Name=""Owner"" Type=""Hot.Person"" Nullable=""false"" />
  </EntityType>
  <EntityContainer Name=""Wild"">
    <Singleton Name=""AnotherSingletonPeople"" Type=""Hot.Person"">
      <NavigationPropertyBinding Path=""Pet"" Target=""Pets"" />
    </Singleton>
    <EntitySet Name=""Pets"" EntityType=""Hot.Pet"">
        <NavigationPropertyBinding Path=""Owner"" Target=""SingletonPeople"" />
    </EntitySet>
    <Singleton Name=""SingletonPeople"" Type=""Hot.Person"">
      <NavigationPropertyBinding Path=""Pet"" Target=""Pets"" />
    </Singleton>
  </EntityContainer>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityContainer wild = model.EntityContainer;

            Assert.AreEqual("AnotherSingletonPeople", wild.Elements.ElementAt(1).Name, "AnotherSingletonPeople: correct second element");
            Assert.AreEqual(EdmContainerElementKind.Singleton, wild.Elements.ElementAt(1).ContainerElementKind, "Wild: second element type");
            Assert.AreEqual("Hot.Person", ((IEdmSingleton)wild.Elements.ElementAt(1)).EntityType().FullName(), "Wild: type for first singleton");

            IEdmEntityType person = (IEdmEntityType)model.FindType("Hot.Person");
            IEdmEntityType pet = (IEdmEntityType)model.FindType("Hot.Pet");
            IEdmSingleton singletonPeople = wild.FindSingleton("SingletonPeople");
            Assert.IsNotNull(singletonPeople, "singletonPeople singleton is not null");
            Assert.AreEqual(person, singletonPeople.EntityType(), "the type of singletonPeople is Person");
            IEdmSingleton anotherPeople = wild.FindSingleton("AnotherSingletonPeople");
            Assert.IsNotNull(anotherPeople, "anotherPeople singleton is not null");
            Assert.AreEqual(person, singletonPeople.Type, "the type of singletonPeople is Person");
            IEdmEntitySet entitySetPeople = wild.FindEntitySet("SingletonPeople");
            Assert.IsNull(entitySetPeople, "entitySetPeople is null");
            IEdmEntitySet pets = wild.FindEntitySet("Pets");
            Assert.IsNotNull(pets);

            Assert.AreEqual("Pets", singletonPeople.FindNavigationTarget(person.NavigationProperties().First()).Name, "PetsAndPeople: end2 correct entity set name");
            Assert.AreEqual("SingletonPeople", pets.FindNavigationTarget(pet.NavigationProperties().First()).Name);

            Assert.AreEqual("SingletonPeople", singletonPeople.Name, "SingletonPeople: correct name");
        }

        [TestMethod]
        public void SchemaReadParseDocumentation()
        {
            //Only parse, do not do validation here.
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Hot"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Address"" Type=""Edm.String"" MaxLength=""100"">
      <Documentation>
        <LongDescription>Just some silly property</LongDescription>
      </Documentation>
    </Property>
    <Documentation>
      <Summary>Summarize this</Summary>
      <LongDescription>Or at length</LongDescription>
    </Documentation>
  </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType person = (IEdmEntityType)model.SchemaElements.First();
            // Test the structural objects.

            IEdmProperty address = person.DeclaredStructuralProperties().Last();
        }

        [TestMethod]
        public void SchemaReadAssociationAndNavigationProperty()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Cold"" Alias=""Comfort"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Feckless"">
    <Key>
      <PropertyRef Name=""Id"" />
      <PropertyRef Name=""Ego"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Ego"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Bonus"" Type=""String"" MaxLength=""1001"" />
    <NavigationProperty Name=""MyReckless"" Type=""Comfort.Reckless"" Partner=""MyFecklesses"">
      <ReferentialConstraint Property=""Id"" ReferencedProperty=""Id"" />
      <ReferentialConstraint Property=""Ego"" ReferencedProperty=""AlterEgo"" />
      <OnDelete Action=""Cascade"" />
    </NavigationProperty>
  </EntityType>
  <EntityType Name=""Reckless"">
    <Key>
      <PropertyRef Name=""Id"" />
      <PropertyRef Name=""AlterEgo"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""AlterEgo"" Type=""Int32"" Nullable=""false"" />
    <NavigationProperty Name=""MyFecklesses"" Type=""Collection(Comfort.Feckless)"" ContainsTarget=""true"" Partner=""MyReckless"" />
  </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            // Test the structural objects.

            Assert.IsTrue(model.SchemaElements.Count() == 2, "Model: contains proper number of Schema Elements");

            IEdmEntityType type1 = (IEdmEntityType)model.SchemaElements.First();

            Assert.AreEqual("Feckless", type1.Name, "Type1 correct name");
            Assert.AreEqual(3, type1.DeclaredStructuralProperties().Count(), "Type1: correct count of properties");
            Assert.AreEqual(2, type1.DeclaredKey.Count(), "Type1: correct number of key properties");
            Assert.AreEqual("Id", type1.DeclaredKey.First().Name, "Type1: correct first key prop");
            Assert.AreEqual(1, type1.DeclaredNavigationProperties().Count(), "Type1: correct count of nav props");
            IEdmNavigationProperty nav1 = type1.DeclaredNavigationProperties().First();
            Assert.AreEqual("MyReckless", nav1.Name, "Nav1: correct name");
            Assert.IsFalse(nav1.ContainsTarget, "Nav1: ContainsTarget");

            IEdmEntityType type2 = (IEdmEntityType)model.SchemaElements.ElementAt(1);

            Assert.AreEqual("Reckless", type2.Name, "Type2 correct name");
            Assert.AreEqual(2, type2.DeclaredStructuralProperties().Count(), "Type2: correct count of properties");
            Assert.AreEqual(2, type2.DeclaredKey.Count(), "Type2: correct number of key properties");
            Assert.AreEqual("Id", type2.DeclaredKey.First().Name, "Type2 correct first key prop");
            Assert.AreEqual(1, type2.DeclaredNavigationProperties().Count(), "Type2: correct count of nav props");
            IEdmNavigationProperty nav2 = type2.DeclaredNavigationProperties().First();
            Assert.IsTrue(nav2.Type.IsNullable);
            Assert.AreEqual("MyFecklesses", nav2.Name, "Nav2: correct name");
            Assert.IsTrue(nav2.ContainsTarget, "Nav2: ContainsTarget");

            // Test the semantic objects.

            IEdmEntityType feckless = (IEdmEntityType)model.FindType("Cold.Feckless");
            IEdmEntityType reckless = (IEdmEntityType)model.FindType("Cold.Reckless");

            IEdmNavigationProperty toReckless = (IEdmNavigationProperty)feckless.FindProperty("MyReckless");
            IEdmNavigationProperty toFecklesses = (IEdmNavigationProperty)reckless.FindProperty("MyFecklesses");

            Assert.AreEqual(toFecklesses, toReckless.Partner, "Correct partner for toReckless");
            Assert.AreEqual(feckless, toReckless.DeclaringEntityType(), "ToReckless correct from type");
            Assert.AreEqual(reckless, toReckless.ToEntityType(), "ToReckless correct to type");

            Assert.AreEqual(toReckless, toFecklesses.Partner, "Correct partner for toFecklesses");
            Assert.AreEqual(reckless, toFecklesses.DeclaringEntityType(), "toFecklesses correct from type");
            Assert.AreEqual(feckless, toFecklesses.ToEntityType(), "toFecklesses correct to type");

            Assert.AreEqual(true, toFecklesses.IsPrincipal(), "Principal end match");
            Assert.IsNull(toFecklesses.DependentProperties(), "Principal end match");
            Assert.AreEqual(false, toReckless.IsPrincipal(), "Dependent end match");
            Assert.IsNotNull(toReckless.DependentProperties(), "Dependent end match");

            Assert.AreEqual(2, toReckless.DependentProperties().Count(), "DependentProperty count correct");
            Assert.AreEqual(toReckless.DependentProperties().First(), feckless.DeclaredKey.First(), "Dependent property1 match");
            Assert.AreEqual(toReckless.DependentProperties().Last(), feckless.DeclaredKey.Last(), "Dependent property2 match");

            Assert.AreEqual(EdmOnDeleteAction.Cascade, toReckless.OnDelete, "Correct end1 action");
            Assert.AreEqual(EdmOnDeleteAction.None, toFecklesses.OnDelete, "Correct end2 action");

            Assert.AreEqual(EdmMultiplicity.Many, toFecklesses.TargetMultiplicity(), "Correct end1 multiplicity");
            Assert.AreEqual(EdmMultiplicity.ZeroOrOne, toReckless.TargetMultiplicity(), "Correct end2 multiplicity");
        }

        [TestMethod]
        public void SchemaReadSelfReferencingNavigationProperty()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Cold"" Alias=""Comfort"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Feckless"">
    <Key>
      <PropertyRef Name=""Id"" />
      <PropertyRef Name=""Ego"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Ego"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Bonus"" Type=""String"" MaxLength=""1001"" />
    <NavigationProperty Name=""MyFeckless"" Type=""Comfort.Feckless"" Partner=""MyFeckless"">
      <ReferentialConstraint Property=""Id"" ReferencedProperty=""Id"" />
      <ReferentialConstraint Property=""Ego"" ReferencedProperty=""AlterEgo"" />
      <OnDelete Action=""Cascade"" />
    </NavigationProperty>
  </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");
        }

        [TestMethod]
        public void SchemaReadSelfReferencingOneToManyNavigationProperty()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Cold"" Alias=""Comfort"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Feckless"">
    <Key>
      <PropertyRef Name=""Id"" />
      <PropertyRef Name=""Ego"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Ego"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Bonus"" Type=""String"" MaxLength=""1001"" />
    <NavigationProperty Name=""MyFeckless"" Type=""Collection(Comfort.Feckless)"" Partner=""MyFeckless"">
      <ReferentialConstraint Property=""Id"" ReferencedProperty=""Id"" />
      <ReferentialConstraint Property=""Ego"" ReferencedProperty=""AlterEgo"" />
      <OnDelete Action=""Cascade"" />
    </NavigationProperty>
  </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");
        }

        [TestMethod]
        public void ParseSimpleComplexTypesModel()
        {
            const string csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Stumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";
            const string csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Mumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Clod"" BaseType=""Stumble.Smod"">
    <Property Name=""Name"" Type=""String"" Nullable=""false"" MaxLength=""1024"" />
    <Property Name=""Address"" Type=""String"" MaxLength=""2048"" />
  </ComplexType>
</Schema>";


            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl1)), XmlReader.Create(new StringReader(csdl2)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            // Test the structural objects.
            Assert.IsTrue(model.SchemaElements.Count() == 2, "correct schemata element count");
            IEdmComplexType type1 = (IEdmComplexType)model.SchemaElements.First();
            IEdmComplexType type2 = (IEdmComplexType)model.SchemaElements.Last();
            Assert.AreEqual("Smod", type1.Name, "Smod not found");
            Assert.AreEqual("Id", type1.DeclaredStructuralProperties().First().Name, "Type1 correct name");
            IEdmPrimitiveTypeReference idType = type1.DeclaredStructuralProperties().First().Type.AsPrimitive();
            Assert.AreEqual("Int32", idType.PrimitiveDefinition().Name, "ID is correct type");
            Assert.IsFalse(idType.IsNullable, "ID is nullable");


            Assert.AreEqual("Clod", type2.Name, "Clod not found");
            Assert.AreEqual(2, type2.DeclaredStructuralProperties().Count(), "Correct number of properties");
            Assert.AreEqual("Address", type2.DeclaredStructuralProperties().Last().Name, "Correct last property");
            IEdmStringTypeReference addressType = type2.DeclaredStructuralProperties().Last().Type.AsPrimitive().AsString();
            Assert.AreEqual("String", addressType.PrimitiveDefinition().Name, "Correct Address type");
            Assert.AreEqual(2048, addressType.MaxLength, "Correct address max length");
            Assert.IsTrue(addressType.IsNullable, "Correct address nulliblity");

            // Test the semantic objects.

            IEdmComplexType smod = (IEdmComplexType)model.FindType("Grumble.Smod");
            IEdmComplexType clod = (IEdmComplexType)model.FindType("Mumble.Clod");

            Assert.AreEqual("Smod", smod.Name, "Smod Name correct");
            Assert.AreEqual("Grumble", smod.Namespace, "Smod Namespace correct");
            Assert.AreEqual("Clod", clod.Name, "Clod Name correct");
            Assert.AreEqual("Mumble", clod.Namespace, "Clod Namespace correct");

            Assert.AreEqual(clod.BaseType, smod, "Clod base type correct");

            Assert.AreEqual(3, clod.StructuralProperties().Count(), "clod inheritied properties properly");
            Assert.AreEqual(clod.StructuralProperties().First(), smod.StructuralProperties().First(), "share same property");
            IEdmProperty address = clod.FindProperty("Address");
            Assert.AreEqual("Address", address.Name, "Addres has proper name");
            Assert.AreEqual(clod.StructuralProperties().Last(), address, "Clod last property is correct");

            IEdmProperty id = clod.FindProperty("Id");
            IEdmPrimitiveTypeReference resolvedIdType = id.Type.AsPrimitive();
            Assert.IsTrue(resolvedIdType.PrimitiveKind() == EdmPrimitiveTypeKind.Int32, "ID is proper kind");
        }

        [TestMethod]
        public void ParseSimpleComplexTypesModelWithoutUsing()
        {
            const string csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";
            const string csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Mumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Clod"" BaseType=""Grumble.Smod"">
    <Property Name=""Name"" Type=""String"" Nullable=""false"" MaxLength=""1024"" />
    <Property Name=""Address"" Type=""String"" MaxLength=""2048"" />
  </ComplexType>
</Schema>";


            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl1)), XmlReader.Create(new StringReader(csdl2)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            // Test the structural objects.
            Assert.IsTrue(model.SchemaElements.Count() == 2, "correct schemata element count");
            IEdmComplexType type1 = (IEdmComplexType)model.SchemaElements.First();
            IEdmComplexType type2 = (IEdmComplexType)model.SchemaElements.Last();
            Assert.AreEqual("Smod", type1.Name, "Smod not found");
            Assert.AreEqual("Id", type1.DeclaredStructuralProperties().First().Name, "Type1 correct name");
            IEdmPrimitiveTypeReference idType = type1.DeclaredStructuralProperties().First().Type.AsPrimitive();
            Assert.AreEqual("Int32", idType.PrimitiveDefinition().Name, "ID is correct type");
            Assert.IsFalse(idType.IsNullable, "ID is nullable");


            Assert.AreEqual("Clod", type2.Name, "Clod not found");
            Assert.AreEqual(2, type2.DeclaredStructuralProperties().Count(), "Correct number of properties");
            Assert.AreEqual("Address", type2.DeclaredStructuralProperties().Last().Name, "Correct last property");
            IEdmStringTypeReference addressType = type2.DeclaredStructuralProperties().Last().Type.AsPrimitive().AsString();
            Assert.AreEqual("String", addressType.PrimitiveDefinition().Name, "Correct Address type");
            Assert.AreEqual(2048, addressType.MaxLength, "Correct address max length");
            Assert.IsTrue(addressType.IsNullable, "Correct address nulliblity");

            // Test the semantic objects.

            IEdmComplexType smod = (IEdmComplexType)model.FindType("Grumble.Smod");
            IEdmComplexType clod = (IEdmComplexType)model.FindType("Mumble.Clod");

            Assert.AreEqual("Smod", smod.Name, "Smod Name correct");
            Assert.AreEqual("Grumble", smod.Namespace, "Smod Namespace correct");
            Assert.AreEqual("Clod", clod.Name, "Clod Name correct");
            Assert.AreEqual("Mumble", clod.Namespace, "Clod Namespace correct");

            Assert.AreEqual(clod.BaseType, smod, "Clod base type correct");

            Assert.AreEqual(3, clod.StructuralProperties().Count(), "clod inheritied properties properly");
            Assert.AreEqual(clod.StructuralProperties().First(), smod.StructuralProperties().First(), "share same property");
            IEdmProperty address = clod.FindProperty("Address");
            Assert.AreEqual("Address", address.Name, "Addres has proper name");
            Assert.AreEqual(clod.StructuralProperties().Last(), address, "Clod last property is correct");

            IEdmProperty id = clod.FindProperty("Id");
            IEdmPrimitiveTypeReference resolvedIdType = id.Type.AsPrimitive();
            Assert.IsTrue(resolvedIdType.PrimitiveKind() == EdmPrimitiveTypeKind.Int32, "ID is proper kind");
        }

        [TestMethod]
        public void ParseComplexTypesInheritanceModel()
        {
            const string csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""NameSpace1"" Alias=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex1"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
  </ComplexType>
  <ComplexType Name=""MyComplex2"" BaseType=""NS1.MyComplex1"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Name"" Type=""Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";

            const string csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""NameSpace2"" Alias=""NS2"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex1"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
  </ComplexType>
  <ComplexType Name=""MyComplex2"" BaseType=""NS2.MyComplex1"">
    <Property Name=""Name"" Type=""String"" Nullable=""false"" />
  </ComplexType>
  <ComplexType Name=""MyComplex3"" BaseType=""NS2.MyComplex2"">
    <Property Name=""age"" Type=""Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";

            const string csdl3 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""NameSpace3"" Alias=""NS3"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex1"" OpenType=""true"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
  </ComplexType>
  <ComplexType Name=""MyComplex2"" BaseType=""NS3.MyComplex1"" OpenType=""false"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Name"" Type=""Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl1)), XmlReader.Create(new StringReader(csdl2)), XmlReader.Create(new StringReader(csdl3)) }, out model, out errors);
            //Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            // Test the structural objects.
            Assert.AreEqual(7, model.SchemaElements.Count(), "correct schemata element count");
            IEdmComplexType type1 = (IEdmComplexType)model.SchemaElements.First();
            IEdmComplexType type2 = (IEdmComplexType)model.SchemaElements.ElementAt(1);
            Assert.AreEqual("MyComplex1", type1.Name, "MyComplex1 not found");
            Assert.AreEqual("Id", type2.DeclaredStructuralProperties().First().Name, "Type2 correct name");

            IEdmPrimitiveTypeReference idType = type2.DeclaredStructuralProperties().First().Type.AsPrimitive();
            Assert.AreEqual("Int32", idType.PrimitiveDefinition().Name, "ID is correct type");
            Assert.IsFalse(idType.IsNullable, "ID is nullable");

            // Test the semantic objects.
            IEdmComplexType type2_1 = (IEdmComplexType)model.FindType("NameSpace2.MyComplex1");
            IEdmComplexType type2_2 = (IEdmComplexType)model.FindType("NameSpace2.MyComplex2");
            IEdmComplexType type2_3 = (IEdmComplexType)model.FindType("NameSpace2.MyComplex3");

            Assert.AreEqual("MyComplex1", type2_1.Name, "MyComplex1 Name correct");
            Assert.AreEqual("MyComplex2", type2_2.Name, "MyComplex2 Name correct");
            Assert.AreEqual("MyComplex3", type2_3.Name, "MyComplex3 Name correct");

            Assert.AreEqual(type2_2.BaseType, type2_1, "MyComplex2 base type correct");
            Assert.AreEqual(type2_3.BaseType, type2_2, "MyComplex3 base type correct");
        }

        [TestMethod]
        public void ParseSimpleEntityTypesModel()
        {
            const string csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Stumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Smod"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
  </EntityType>
  <EntityType Name=""Smod2"" BaseType=""Grumble.Smod"" OpenType=""true"">
    <Property Name=""Idd"" Type=""Int32"" Nullable=""false"" />
  </EntityType>
  <EntityType Name=""Smod3"" BaseType=""Grumble.Smod2"">
    <Property Name=""Iddd"" Type=""Int32"" Nullable=""false"" />
  </EntityType>
</Schema>";
            const string csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Mumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Clod"" BaseType=""Stumble.Smod"">
    <Property Name=""Name"" Type=""String"" Nullable=""false"" MaxLength=""1024"" />
    <Property Name=""Address"" Type=""String"" MaxLength=""2048"" />
  </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl1)), XmlReader.Create(new StringReader(csdl2)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");
            // Test the structural objects.

            IEdmEntityType type1 = (IEdmEntityType)model.SchemaElements.First();
            IEdmEntityType type2 = (IEdmEntityType)model.SchemaElements.Last();
            Assert.AreEqual(type1.Name, "Smod", "Smod not found");
            Assert.AreEqual(1, type1.DeclaredKey.Count(), "Type1: correct key property count");
            Assert.AreEqual("Id", type1.DeclaredKey.First().Name, "Type1: correct key property name");
            Assert.AreEqual("Id", type1.DeclaredStructuralProperties().First().Name, "Correct first property");
            IEdmPrimitiveTypeReference idType = type1.DeclaredStructuralProperties().First().Type.AsPrimitive();
            Assert.AreEqual(EdmPrimitiveTypeKind.Int32, idType.PrimitiveKind(), "First property correct type");
            Assert.IsFalse(idType.IsNullable, "First prop is correctly not nullable");
            Assert.AreEqual(type2.Name, "Clod", "Clod not found");
            Assert.AreEqual("Grumble.Smod", type2.BaseEntityType().FullName(), "Correct base type name");
            Assert.AreEqual(type2.DeclaredStructuralProperties().Count(), 2, "Correct number of properties on inheriting type");
            Assert.AreEqual("Address", type2.StructuralProperties().Last().Name, "Correct name on last structural prop");
            IEdmStringTypeReference addressType = type2.StructuralProperties().Last().Type.AsPrimitive().AsString();
            Assert.AreEqual(addressType.PrimitiveKind(), EdmPrimitiveTypeKind.String, "Last property correct type");
            Assert.AreEqual(2048, addressType.MaxLength, "Address correct length");
            Assert.IsTrue(addressType.IsNullable, "Address is nullable");

            // Test the semantic objects.

            IEdmEntityType smod = (IEdmEntityType)model.FindType("Grumble.Smod");
            IEdmEntityType smod2 = (IEdmEntityType)model.FindType("Grumble.Smod2");
            IEdmEntityType smod3 = (IEdmEntityType)model.FindType("Grumble.Smod3");
            IEdmEntityType clod = (IEdmEntityType)model.FindType("Mumble.Clod");

            Assert.AreEqual("Smod", smod.Name, "Correct Smod Name");
            Assert.AreEqual("Clod", clod.Name, "Correct Clod Name");

            Assert.IsFalse(smod.IsOpen, "smod.IsOpen");
            Assert.IsTrue(smod2.IsOpen, "smod2.IsOpen");
            Assert.IsFalse(smod3.IsOpen, "smod3.IsOpen");

            Assert.AreEqual(clod.BaseType, smod, "Correct Inheritance");

            Assert.AreEqual(3, clod.StructuralProperties().Count(), "Correct number of total props");
            Assert.AreEqual(clod.StructuralProperties().First(), smod.StructuralProperties().First(), "ID properly inherited");
            IEdmProperty address = clod.FindProperty("Address");
            Assert.AreEqual("Address", address.Name, "Address correct name");
            Assert.AreEqual(clod.StructuralProperties().Last(), address, "Clod correct last property");

            IEdmProperty id = clod.FindProperty("Id");
            IEdmPrimitiveTypeReference resolvedIdType = id.Type.AsPrimitive();
            Assert.AreEqual(resolvedIdType.PrimitiveKind(), EdmPrimitiveTypeKind.Int32, "ID correct kind");

            Assert.AreEqual(1, smod.DeclaredKey.Count(), "Smod correct number of key props");
            Assert.AreEqual(id, smod.DeclaredKey.First(), "Smod correct key prop");
            Assert.AreEqual(smod.DeclaredKey.First(), clod.Key().First(), "Inhertance shared key");
        }

        [TestMethod]
        public void ParseStream()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Int32""/>
    <Property Name=""Stream"" Type=""Edm.Stream""/>
    <Property Name=""Stream1"" Type=""Edm.Stream""/>
    <Property Name=""Stream2"" Type=""Edm.Stream""/>
  </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmComplexType smodDef = (IEdmComplexType)model.SchemaElements.First();
            IEdmStructuralProperty streamProp = (IEdmStructuralProperty)smodDef.FindProperty("Stream");
            Assert.AreEqual(EdmTypeKind.Primitive, streamProp.Type.TypeKind(), "Stream should have type of primitive");
            Assert.AreEqual(EdmPrimitiveTypeKind.Stream, streamProp.Type.AsPrimitive().PrimitiveKind(), "Stream should have primitive kind of stream");
            IEdmStructuralProperty streamProp1 = (IEdmStructuralProperty)smodDef.FindProperty("Stream1");
            IEdmStructuralProperty streamProp2 = (IEdmStructuralProperty)smodDef.FindProperty("Stream2");
        }

        [TestMethod]
        public void ParseStringAttributeAnnotation()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"" xmlns:p=""p"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" p:foo=""Quoth the Raven"" />
  </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            // Test the structural objects.
            Assert.IsTrue(model.SchemaElements.Count() == 1, "correct schemata element count");
            IEdmComplexType type = (IEdmComplexType)model.SchemaElements.First();
            Assert.AreEqual("Smod", type.Name, "Smod not found");
            Assert.AreEqual("Id", type.DeclaredStructuralProperties().First().Name, "Type correct name");
            IEdmPrimitiveTypeReference idType = type.DeclaredStructuralProperties().First().Type.AsPrimitive();
            Assert.AreEqual("Int32", idType.PrimitiveDefinition().Name, "ID is correct type");
            Assert.IsFalse(idType.IsNullable, "ID is nullable");

            // Test the Annotation
            IEdmValue annotationValue = model.GetAnnotationValue<IEdmValue>(type.DeclaredStructuralProperties().First(), "p", "foo");
            IEdmDirectValueAnnotation annotation = (IEdmDirectValueAnnotation)model.DirectValueAnnotations(type.DeclaredStructuralProperties().First()).First();
            Assert.AreEqual(annotation.Value, annotationValue, "Correct annotation found");
            Assert.AreEqual("p", annotation.NamespaceUri, "Correct annotation namespace");
            Assert.AreEqual("foo", annotation.Name, "Correct annotation local name");
            Assert.AreEqual(EdmPrimitiveTypeKind.String, annotationValue.Type.AsPrimitive().PrimitiveKind(), "Correct annotation type");
            Assert.AreEqual("Quoth the Raven", ((IEdmStringValue)annotationValue).Value, "Correct annotation value");
        }

        [TestMethod]
        public void ParseStringElementAnnotation()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"" xmlns:p=""p"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" >
        <p:foo>
           Quoth the Raven
        </p:foo>
    </Property>
  </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            // Test the structural objects.
            Assert.IsTrue(model.SchemaElements.Count() == 1, "correct schemata element count");
            IEdmComplexType type = (IEdmComplexType)model.SchemaElements.First();
            Assert.AreEqual("Smod", type.Name, "Smod not found");
            Assert.AreEqual("Id", type.DeclaredStructuralProperties().First().Name, "Type correct name");
            IEdmPrimitiveTypeReference idType = type.DeclaredStructuralProperties().First().Type.AsPrimitive();
            Assert.AreEqual("Int32", idType.PrimitiveDefinition().Name, "ID is correct type");
            Assert.IsFalse(idType.IsNullable, "ID is nullable");

            // Test the Annotation
            IEdmValue annotationValue = model.GetAnnotationValue<IEdmValue>(type.DeclaredStructuralProperties().First(), "p", "foo");
            IEdmDirectValueAnnotation annotation = (IEdmDirectValueAnnotation)model.DirectValueAnnotations(type.DeclaredStructuralProperties().First()).First();
            Assert.AreEqual(annotation.Value, annotationValue, "Correct annotation found");
            Assert.AreEqual("p", annotation.NamespaceUri, "Correct annotation namespace");
            Assert.AreEqual("foo", annotation.Name, "Correct annotation local name");
            Assert.AreEqual(EdmPrimitiveTypeKind.String, annotationValue.Type.AsPrimitive().PrimitiveKind(), "Correct annotation type");
            //TODO: handle complex annotations properly
            Assert.AreEqual("<p:foo xmlns:p=\"p\">\n           Quoth the Raven\n        </p:foo>", ((IEdmStringValue)annotationValue).Value, "Correct annotation value");
        }

        [TestMethod]
        public void ParseStringEmptyElementAnnotation()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"" xmlns:p=""p"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" ><p:foo /></Property>
  </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            // Test the structural objects.
            Assert.IsTrue(model.SchemaElements.Count() == 1, "correct schemata element count");
            IEdmComplexType type = (IEdmComplexType)model.SchemaElements.First();
            Assert.AreEqual("Smod", type.Name, "Smod not found");
            Assert.AreEqual("Id", type.DeclaredStructuralProperties().First().Name, "Type correct name");
            IEdmPrimitiveTypeReference idType = type.DeclaredStructuralProperties().First().Type.AsPrimitive();
            Assert.AreEqual("Int32", idType.PrimitiveDefinition().Name, "ID is correct type");
            Assert.IsFalse(idType.IsNullable, "ID is nullable");

            // Test the Annotation
            IEdmValue annotationValue = model.GetAnnotationValue<IEdmValue>(type.DeclaredStructuralProperties().First(), "p", "foo");
            IEdmDirectValueAnnotation annotation = (IEdmDirectValueAnnotation)model.DirectValueAnnotations(type.DeclaredStructuralProperties().First()).First();
            Assert.AreEqual(annotation.Value, annotationValue, "Correct annotation found");
            Assert.AreEqual("p", annotation.NamespaceUri, "Correct annotation namespace");
            Assert.AreEqual("foo", annotation.Name, "Correct annotation local name");
            Assert.AreEqual(EdmPrimitiveTypeKind.String, annotationValue.Type.AsPrimitive().PrimitiveKind(), "Correct annotation type");
            //TODO: handle complex annotations properly
            Assert.AreEqual(@"<p:foo xmlns:p=""p"" />", ((IEdmStringValue)annotationValue).Value, "Correct annotation value");
        }

        [TestMethod]
        public void RemoveImmutableAnnotations()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"" xmlns:p=""p"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" p:foo=""Quoth the Raven"" p:bar=""Nevermore"" p:baz=""Right Phil?"" />
  </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmComplexType type = (IEdmComplexType)model.SchemaElements.First();
            IEdmStructuralProperty id = type.DeclaredStructuralProperties().First();

            Assert.IsTrue(model.DirectValueAnnotations(id).Count() == 3, "Annotation count");
            IEdmValue annotationValue = model.GetAnnotationValue<IEdmValue>(id, "p", "foo");
            Assert.AreEqual("Quoth the Raven", ((IEdmStringValue)annotationValue).Value, "Annotation value");
            annotationValue = model.GetAnnotationValue<IEdmValue>(id, "p", "bar");
            Assert.AreEqual("Nevermore", ((IEdmStringValue)annotationValue).Value, "Annotation value");
            annotationValue = model.GetAnnotationValue<IEdmValue>(id, "p", "baz");
            Assert.AreEqual("Right Phil?", ((IEdmStringValue)annotationValue).Value, "Annotation value");

            model.SetAnnotationValue(id, "p", "foo", null);
            model.SetAnnotationValue(id, "p", "baz", "Sure Bobby");

            Assert.IsTrue(model.DirectValueAnnotations(id).Count() == 2, "Annotation count");
            annotationValue = model.GetAnnotationValue<IEdmValue>(id, "p", "foo");
            Assert.IsNull(annotationValue, "Annotation value");
            annotationValue = model.GetAnnotationValue<IEdmValue>(id, "p", "bar");
            Assert.AreEqual("Nevermore", ((IEdmStringValue)annotationValue).Value, "Annotation value");
            Assert.AreEqual(model.GetAnnotationValue<string>(id, "p", "baz"), "Sure Bobby", "Annotation value");
        }

        [TestMethod]
        public void ParseCollectionPropertyInline()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Int32""/>
    <Property Name=""Collection"" Type=""Collection(Int32)""/>
  </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmComplexType smodDef = (IEdmComplexType)model.SchemaElements.First();
            IEdmStructuralProperty mvProp = (IEdmStructuralProperty)smodDef.FindProperty("Collection");
            Assert.AreEqual(EdmTypeKind.Collection, mvProp.Type.TypeKind(), "Collection should have type of collection");
            Assert.AreEqual(EdmTypeKind.Primitive, mvProp.Type.AsCollection().ElementType().TypeKind(), "Collection should be of primitives");
            Assert.AreEqual(EdmPrimitiveTypeKind.Int32, mvProp.Type.AsCollection().ElementType().AsPrimitive().PrimitiveKind(), "Collection should be of Int32s");
        }

        [TestMethod]
        public void EmptyNullableFacetCorrectlyTrue()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Int32""/>
  </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmComplexType smodDef = (IEdmComplexType)model.SchemaElements.First();
            IEdmStructuralProperty idProp = (IEdmStructuralProperty)smodDef.FindProperty("Id");
            Assert.IsTrue(idProp.Type.IsNullable, "Nullable should default to true");

        }

        [TestMethod]
        public void VerifyFindPropertyFindsBaseProperties()
        {
            const string csdl =
@"<Schema Namespace=""FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespacesComplex.first"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""validEntityType1"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Property0"" Type=""Binary"" Nullable=""false"" />
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
  </EntityType>
  <EntityType Name=""validEntityType2"" BaseType=""FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespacesComplex.first.validEntityType1"" />
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType inheritingEntityType = (IEdmEntityType)model.SchemaElements.Last();
            IEdmStructuralProperty baseProperty = (IEdmStructuralProperty)inheritingEntityType.FindProperty("Property0");
            Assert.AreEqual(baseProperty, inheritingEntityType.StructuralProperties().First(), "FindProperty should also return properties of base type");
        }

        [TestMethod]
        public void VerifyUnicodeDefaultsTrue()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""myString"" Type=""String""/>
  </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmComplexType typeDefinition = (IEdmComplexType)model.SchemaElements.First();
            IEdmStructuralProperty stringProp = (IEdmStructuralProperty)typeDefinition.FindProperty("myString");
            Assert.AreEqual(stringProp.Type.AsString().IsUnicode, true, "Unicode should default to true");
        }

        #region Operations and OperationImports

        [TestMethod]
        public void SimpleOperation()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Birthday"" Type=""DateTimeOffset"" />
  </EntityType>
  <Function Name=""GetAge"">
    <Parameter Name=""Person"" Type=""foo.Person"" />
    <ReturnType Type=""Edm.Int32"" />
  </Function>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType personType = (IEdmEntityType)model.SchemaElements.First();
            IEdmOperation getAge = (IEdmOperation)model.FindOperations("foo.GetAge").First();
            Assert.AreEqual("GetAge", getAge.Name, "Group name correct");
            Assert.IsNotNull(getAge, "Function exists and has IEdmFunctionType");
            Assert.AreEqual("GetAge", getAge.Name, "Name correct");
            Assert.AreEqual("foo", getAge.Namespace, "Namespace correct");
            Assert.AreEqual(EdmPrimitiveTypeKind.Int32, getAge.ReturnType.AsPrimitive().PrimitiveKind(), "Return type correct");
            Assert.AreEqual(EdmSchemaElementKind.Function, getAge.SchemaElementKind, "Schema element kind correct");
            IEdmOperationParameter getAgeParameter = getAge.Parameters.First();
            Assert.AreEqual(getAgeParameter, getAge.FindParameter("Person"), "Find parameter returns proper parameter");
            Assert.AreEqual("Person", getAgeParameter.Name, "Parameter has correct name");
            Assert.AreEqual(personType, getAgeParameter.Type.Definition, "Parameter has correct mode");
        }

        [TestMethod]
        public void TestDuplicateParameterNameInFunction()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Birthday"" Type=""DateTimeOffset"" />
  </EntityType>
  <Function Name=""GetAge"">
    <Parameter Name=""PersonParam"" Type=""foo.Person"" />
    <Parameter Name=""PersonParam"" Type=""foo.Person"" />
    <ReturnType Type=""Edm.Int32"" />
  </Function>
</Schema>";

            // parsing should succeed.
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            // validation should detect duplicated parameter names.
            IEnumerable<EdmError> validationErrors;
            model.Validate(out validationErrors);
            Assert.AreEqual(validationErrors.Single().ErrorMessage, "Each parameter name in a operation must be unique. The parameter name 'PersonParam' is already defined.");

            // FindParameter() should throw exception.
            IEdmEntityType personType = (IEdmEntityType)model.SchemaElements.First();
            IEdmOperation getAge = (IEdmOperation)model.FindOperations("foo.GetAge").First();
            try
            {
                var param1 = getAge.FindParameter("PersonParam");
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("Sequence contains more than one matching element", ex.Message);
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public void SimpleOperationWithNullableParameterAndReturnType()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Function Name=""Foo"">
    <ReturnType Type=""Int32"" Nullable=""false"" />
    <Parameter Name=""Param1"" Type=""Int32"" Nullable=""false"" />
  </Function>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");
        }

        [TestMethod]
        public void SimpleOperationImport()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Person"">
        <Key>
          <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""Edm.DateTimeOffset"" />
      </EntityType>
      <Action Name=""peopleWhoAreAwesome"">
        <Parameter Name=""awesomeName"" Type=""Edm.String"" />
        <ReturnType Type=""Collection(foo.Person)"" />
      </Action>
      <Function Name=""peopleWhoAreAwesome"" IsBound=""true"" IsComposable=""true"">
        <Parameter Name=""Persons"" Type=""Collection(foo.Person)"" />
        <ReturnType Type=""Collection(foo.Person)"" />
      </Function>
      <Action Name=""peopleWhoAreAwesome"">
        <Parameter Name=""Persons"" Type=""Collection(foo.Person)"" />
        <ReturnType Type=""Collection(foo.Person)"" />
      </Action>
      <Action Name=""peopleWhoAreAwesome""><ReturnType Type=""Collection(foo.Person)"" />
        <Parameter Name=""Persons"" Type=""Collection(foo.Person)"" />
      </Action>
      <Function Name=""peopleWhoAreAwesome""><ReturnType Type=""Collection(foo.Person)"" />
        <Parameter Name=""Persons"" Type=""Collection(foo.Person)"" />
      </Function>
      <Function Name=""peopleWhoAreAwesome"" IsComposable=""true""><ReturnType Type=""Collection(foo.Person)"" />
        <Parameter Name=""Persons"" Type=""Collection(foo.Person)"" />
      </Function>
      <Action Name=""peopleWhoAreAwesome""><ReturnType Type=""Collection(foo.Person)"" />
        <Parameter Name=""Persons"" Type=""Collection(foo.Person)"" />
      </Action>
      <Function Name=""peopleWhoAreAwesome""><ReturnType Type=""Collection(foo.Person)"" />
        <Parameter Name=""Persons"" Type=""Collection(foo.Person)"" />
      </Function>
      <EntityContainer Name=""fooContainer"">
        <EntitySet Name=""People"" EntityType=""foo.Person"" />
        <ActionImport Name=""peopleWhoAreAwesomeAction"" Action=""foo.peopleWhoAreAwesome"" EntitySet=""People"" />
        <FunctionImport Name=""peopleWhoAreAwesomeFunction"" Function=""foo.peopleWhoAreAwesome"" EntitySet=""People"" />
      </EntityContainer>
    </Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType personType = (IEdmEntityType)model.SchemaElements.First();
            IEdmEntityContainer fooContainer = (IEdmEntityContainer)model.FindEntityContainer("fooContainer");
            IEdmEntitySet peopleSet = (IEdmEntitySet)fooContainer.Elements.First();

            var fiGroup = fooContainer.FindOperationImports("peopleWhoAreAwesomeAction").ToArray();
            Assert.AreEqual(4, fiGroup.Length, "# of overloads expected");

            IEdmOperationImport peopleWhoAreAwesome = fiGroup.First();
            Assert.AreEqual(EdmContainerElementKind.ActionImport, peopleWhoAreAwesome.ContainerElementKind, "FunctionImport has correct ContainerElementKind");
            IEdmEntitySetBase eset;
            Assert.IsTrue(peopleWhoAreAwesome.TryGetStaticEntitySet(model, out eset), "peopleWhoAreAwesome.TryGetStaticEntitySet");
            Assert.AreEqual(peopleSet, eset, "Return EntitySet name is correct");
            IEdmEntitySetBase fiEntitySet;
            Assert.IsTrue(peopleWhoAreAwesome.TryGetStaticEntitySet(model, out fiEntitySet), "peopleWhoAreAwesome.TryGetStaticEntitySet");
            Assert.AreEqual(personType, fiEntitySet.EntityType(), "Return EntitySet type is correct");
            Assert.AreEqual("peopleWhoAreAwesomeAction", peopleWhoAreAwesome.Name, "FunctionImport name is correct");
            Assert.AreEqual(EdmTypeKind.Collection, peopleWhoAreAwesome.Operation.ReturnType.Definition.TypeKind, "Return typekind is correct");
            Assert.AreEqual(personType, peopleWhoAreAwesome.Operation.ReturnType.AsCollection().ElementType().Definition, "Return entitytype is correct");
            IEdmOperationParameter peopleWhoAreAwesomeParameter = peopleWhoAreAwesome.Operation.Parameters.First();
            Assert.AreEqual(peopleWhoAreAwesomeParameter, peopleWhoAreAwesome.Operation.FindParameter("awesomeName"), "Find parameter returns proper parameter");
            Assert.AreEqual("awesomeName", peopleWhoAreAwesomeParameter.Name, "FunctionImport parameter name is correct");
            Assert.IsFalse(peopleWhoAreAwesomeParameter.Type.AsString().IsBad(), "FunctionImport has correct ContainerElementKind");

            Assert.IsTrue(fiGroup[0] is IEdmActionImport, "Expected to be a action import");
            Assert.IsTrue(fiGroup[1] is IEdmActionImport, "Expected to be a action import");
            Assert.IsTrue(fiGroup[2] is IEdmActionImport, "Expected to be a action import");
            Assert.IsTrue(fiGroup[3] is IEdmActionImport, "Expected to be a action import");

            fiGroup = fooContainer.FindOperationImports("peopleWhoAreAwesomeFunction").ToArray();
            Assert.AreEqual(3, fiGroup.Length, "# of overloads expected");
            Assert.IsTrue(fiGroup[0] is IEdmFunctionImport, "Expected to be a function import");
            Assert.IsTrue(fiGroup[1] is IEdmFunctionImport, "Expected to be a function import");
            Assert.IsTrue(fiGroup[2] is IEdmFunctionImport, "Expected to be a function import");
        }

        // BuildInternalUniqueParameterTypeFunctionString is not including the type name for collection of entities
        [TestMethod]
        public void OperationImportParameterAndReturnTypesV3()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
    <NavigationProperty Name=""Orders"" Type=""Collection(foo.Order)"" />
  </EntityType>
  <EntityType Name=""Person2"" BaseType=""foo.Person"">
    <NavigationProperty Name=""Person2Orders"" Type=""Collection(foo.Order)"" />
  </EntityType>
  <EntityType Name=""Person3"" BaseType=""foo.Person2"">
    <NavigationProperty Name=""Person3Orders"" Type=""Collection(foo.Order)"" />
  </EntityType>
  <EntityType Name=""Order"">
    <Key>
      <PropertyRef Name=""ID"" />
    </Key>
    <Property Name=""ID"" Type=""Edm.String"" Nullable=""false"" />
    <Property Name=""PersonId"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Person2Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Person3Id"" Type=""Int32"" Nullable=""false"" />
    <NavigationProperty Name=""Customer"" Type=""foo.Person"" Nullable=""false"" />
  </EntityType>
  <ComplexType Name=""PersonCT"">
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
  </ComplexType>

  <Action Name=""peopleWhoAreAwesome1""><ReturnType Type=""Collection(foo.Person)"" />
    <Parameter Name=""p"" Type=""Collection(foo.Person)"" />
  </Action>
  <Action Name=""peopleWhoAreAwesome2""><ReturnType Type=""Collection(foo.PersonCT)"" />
    <Parameter Name=""p"" Type=""Collection(foo.PersonCT)"" />
  </Action>
  <Action Name=""peopleWhoAreAwesome3""><ReturnType Type=""Collection(String)"" />
    <Parameter Name=""p"" Type=""Collection(Edm.String)"" />
  </Action>
  <Action Name=""peopleWhoAreAwesome4""><ReturnType Type=""foo.Person"" />
    <Parameter Name=""p"" Type=""foo.Person"" />
  </Action>
  <Action Name=""peopleWhoAreAwesome5""><ReturnType Type=""foo.PersonCT"" />
    <Parameter Name=""p"" Type=""foo.PersonCT"" />
  </Action>
  <Action Name=""peopleWhoAreAwesome6""><ReturnType Type=""Edm.String"" />
    <Parameter Name=""p"" Type=""String"" />
  </Action>
  <Function Name=""peopleWhoAreAwesome"" IsBound=""true"" EntitySetPath=""People"" IsComposable=""true""><ReturnType Type=""Collection(foo.Person)"" />
    <Parameter Name=""People"" Type=""Collection(foo.Person)"" />
  </Function>
  <Function Name=""peopleWhoAreAwesome"" IsComposable=""true""><ReturnType Type=""Collection(foo.Order)"" />
    <Parameter Name=""People"" Type=""Collection(foo.Person)"" />
  </Function>
  <Function Name=""peopleWhoAreAwesome"" IsBound=""true"" EntitySetPath=""Purchases/Customer"" IsComposable=""true""><ReturnType Type=""foo.Person"" />
    <Parameter Name=""Purchases"" Type=""Collection(foo.Order)"" />
  </Function>
  <Function Name=""peopleWhoAreAwesome"" IsComposable=""true""><ReturnType Type=""Collection(foo.Order)"" />
    <Parameter Name=""Purchases"" Type=""foo.Order"" />
  </Function>
  <Function Name=""pathWithCasts1"" IsComposable=""true""><ReturnType Type=""Collection(foo.Order)"" />
    <Parameter Name=""Purchases"" Type=""Collection(foo.Order)"" />
  </Function>
  <Function Name=""pathWithCasts2"" IsComposable=""true""><ReturnType Type=""Collection(foo.Order)"" />
    <Parameter Name=""Purchases"" Type=""Collection(foo.Order)"" />
  </Function>
  <Function Name=""pathWithCasts3"" IsComposable=""true""><ReturnType Type=""Collection(foo.Order)"" />
    <Parameter Name=""Purchases"" Type=""Collection(foo.Order)"" />
  </Function>

  <EntityContainer Name=""fooContainer"">
    <EntitySet Name=""People"" EntityType=""foo.Person"">
      <NavigationPropertyBinding Path=""Orders"" Target=""Orders"" />
      <NavigationPropertyBinding Path=""foo.Person2/Person2Orders"" Target=""Orders2"" />
      <NavigationPropertyBinding Path=""foo.Person3/Person3Orders"" Target=""Orders3"" />
    </EntitySet>
    <EntitySet Name=""Orders"" EntityType=""foo.Order"">
      <NavigationPropertyBinding Path=""Customer"" Target=""People"" />
    </EntitySet>
    <EntitySet Name=""Orders2"" EntityType=""foo.Order"" />
    <EntitySet Name=""Orders3"" EntityType=""foo.Order"" />
    <ActionImport Name=""peopleWhoAreAwesome1"" Action=""foo.peopleWhoAreAwesome1"" EntitySet=""People"" />
    <ActionImport Name=""peopleWhoAreAwesome2"" Action=""foo.peopleWhoAreAwesome2"" />
    <ActionImport Name=""peopleWhoAreAwesome3"" Action=""foo.peopleWhoAreAwesome3"" />
    <ActionImport Name=""peopleWhoAreAwesome4"" Action=""foo.peopleWhoAreAwesome4"" EntitySet=""People"" />
    <ActionImport Name=""peopleWhoAreAwesome5"" Action=""foo.peopleWhoAreAwesome5"" />
    <ActionImport Name=""peopleWhoAreAwesome6"" Action=""foo.peopleWhoAreAwesome6"" />
    <FunctionImport Name=""peopleWhoAreAwesome"" Function=""foo.peopleWhoAreAwesome"" />
    <FunctionImport Name=""pathWithCasts1"" Function=""foo.pathWithCasts1"" />
    <FunctionImport Name=""pathWithCasts2"" Function=""foo.pathWithCasts2"" />
    <FunctionImport Name=""pathWithCasts3"" Function=""foo.pathWithCasts3"" />
  </EntityContainer>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No parsing errors");

            var validated = EdmValidator.Validate(model, EdmConstants.EdmVersionLatest, out errors);
            Assert.IsTrue(validated, "validate");

            var people = model.EntityContainer.FindEntitySet("People");
            Assert.AreEqual("Person", people.EntityType().Name, "people.ElementType.Name");
            Assert.IsFalse(((IEdmNavigationProperty)people.EntityType().FindProperty("Orders")).ContainsTarget, "Orders.ContainsTarget");

            var operationImports = model.EntityContainer.OperationImports().ToArray();
            Assert.AreEqual(11, operationImports.Length, "# of operation imports");

            Assert.AreEqual(EdmTypeKind.Entity, operationImports[0].Operation.ReturnType.AsCollection().ElementType().TypeKind(), "operationImports[0] return type");
            Assert.AreEqual(EdmTypeKind.Entity, operationImports[0].Operation.Parameters.Single().Type.AsCollection().ElementType().TypeKind(), "operationImports[0] parameter type");
            IEdmEntitySetBase eset;
            IEdmOperationParameter p;
            Dictionary<IEdmNavigationProperty, IEdmPathExpression> np;
            IEnumerable<EdmError> entitySetPathErrors;
            Assert.IsTrue(operationImports[0].TryGetStaticEntitySet(model, out eset), "operationImports[0].TryGetStaticEntitySet");
            Assert.IsFalse(operationImports[0].TryGetRelativeEntitySetPath(model, out p, out np, out entitySetPathErrors), "operationImports[0].TryGetRelativeEntitySetPath");
            Assert.AreEqual(people, eset, "people, eset");

            Assert.AreEqual(EdmTypeKind.Complex, operationImports[1].Operation.ReturnType.AsCollection().ElementType().TypeKind(), "operationImports[1] return type");
            Assert.AreEqual(EdmTypeKind.Complex, operationImports[1].Operation.Parameters.Single().Type.AsCollection().ElementType().TypeKind(), "operationImports[1] parameter type");

            Assert.AreEqual(EdmTypeKind.Primitive, operationImports[2].Operation.ReturnType.AsCollection().ElementType().TypeKind(), "operationImports[2] return type");
            Assert.AreEqual(EdmTypeKind.Primitive, operationImports[2].Operation.Parameters.Single().Type.AsCollection().ElementType().TypeKind(), "operationImports[2] parameter type");

            Assert.AreEqual(EdmTypeKind.Entity, operationImports[3].Operation.ReturnType.TypeKind(), "operationImports[3] return type");
            Assert.AreEqual(EdmTypeKind.Entity, operationImports[3].Operation.Parameters.Single().Type.TypeKind(), "operationImports[3] parameter type");
            IEdmEntitySetBase eset2;
            Assert.IsTrue(operationImports[3].TryGetStaticEntitySet(model, out eset2), "operationImports[3].TryGetStaticEntitySet");
            Assert.IsFalse(operationImports[0].TryGetRelativeEntitySetPath(model, out p, out np, out entitySetPathErrors), "operationImports[0].TryGetRelativeEntitySetPath");
            Assert.AreEqual(people, eset2, "people, eset2");

            Assert.AreEqual(EdmTypeKind.Complex, operationImports[4].Operation.ReturnType.TypeKind(), "operationImports[4] return type");
            Assert.AreEqual(EdmTypeKind.Complex, operationImports[4].Operation.Parameters.Single().Type.TypeKind(), "operationImports[4] parameter type");

            Assert.AreEqual(EdmTypeKind.Primitive, operationImports[5].Operation.ReturnType.TypeKind(), "operationImports[5] return type");
            Assert.AreEqual(EdmTypeKind.Primitive, operationImports[5].Operation.Parameters.Single().Type.TypeKind(), "operationImports[5] parameter type");

            Assert.IsFalse(operationImports[6].TryGetStaticEntitySet(model, out eset), "operationImports[6].TryGetStaticEntitySet");
            Assert.IsFalse(operationImports[6].TryGetRelativeEntitySetPath(model, out p, out np, out entitySetPathErrors), "operationImports[6].TryGetStaticEntitySet");
            Assert.IsNull(p, "p is null");

            Assert.IsFalse(operationImports[7].TryGetStaticEntitySet(model, out eset), "operationImports[7].TryGetStaticEntitySet");
            Assert.IsFalse(operationImports[7].TryGetRelativeEntitySetPath(model, out p, out np, out entitySetPathErrors), "operationImports[7].TryGetStaticEntitySet");

            Assert.IsFalse(operationImports[8].TryGetStaticEntitySet(model, out eset), "operationImports[8].TryGetStaticEntitySet");
            Assert.IsFalse(operationImports[8].TryGetRelativeEntitySetPath(model, out p, out np, out entitySetPathErrors), "operationImports[8].TryGetStaticEntitySet");

            Assert.IsFalse(operationImports[9].TryGetStaticEntitySet(model, out eset), "operationImports[9].TryGetStaticEntitySet");
            Assert.IsFalse(operationImports[9].TryGetRelativeEntitySetPath(model, out p, out np, out entitySetPathErrors), "operationImports[9].TryGetStaticEntitySet");

            Assert.IsFalse(operationImports[10].TryGetStaticEntitySet(model, out eset), "operationImports[10].TryGetStaticEntitySet");
            Assert.IsFalse(operationImports[10].TryGetRelativeEntitySetPath(model, out p, out np, out entitySetPathErrors), "operationImports[10].TryGetStaticEntitySet");
        }

        [TestMethod]
        public void SimpleOperationWithDocumentation()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Birthday"" Type=""DateTimeOffset"" />
  </EntityType>
  <Function Name=""GetAge""><ReturnType Type=""Edm.Int32""/>
      <Documentation>
        <LongDescription>Function Documentation!!!</LongDescription>
      </Documentation>
    <Parameter Name=""Person"" Type=""foo.Person"" />
    </Function>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmOperation getAge = (IEdmOperation)model.FindOperations("foo.GetAge").First();
        }

        #endregion

        [TestMethod]
        public void VerifyProperRefConstraintOutOfOrder()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"" >
    <EntityType Name=""Entity1"">
        <Key>
            <PropertyRef Name=""Pk1"" />
            <PropertyRef Name=""Pk2"" />
        </Key>
        <Property Type=""Int32"" Name=""Pk1"" Nullable=""false"" />
        <Property Type=""Int32"" Name=""Pk2"" Nullable=""false"" />
        <NavigationProperty Name=""ToEntity2"" Type=""Collection(Grumble.Entity2)"" Partner=""ToEntity1"" />
    </EntityType>
    <EntityType Name=""Entity2"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Type=""Int32"" Name=""Id"" Nullable=""false"" />
        <Property Type=""Int32"" Name=""Fk1"" Nullable=""false"" />
        <Property Type=""Int32"" Name=""Fk2"" Nullable=""false"" />
        <NavigationProperty Name=""ToEntity1"" Type=""Grumble.Entity1"" Nullable=""false"">
          <ReferentialConstraint Property=""Fk1"" ReferencedProperty=""Pk1"" />
          <ReferentialConstraint Property=""Fk2"" ReferencedProperty=""Pk2"" />
        </NavigationProperty>
    </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType entity1Def = (IEdmEntityType)model.FindType("Grumble.Entity1");
            IEdmEntityType entity2Def = (IEdmEntityType)model.FindType("Grumble.Entity2");

            Assert.IsTrue(((IEdmNavigationProperty)entity1Def.FindProperty("ToEntity2")).IsPrincipal(), "Correct principal end");
            Assert.AreEqual(entity2Def.FindProperty("Fk1"), ((IEdmNavigationProperty)entity1Def.FindProperty("ToEntity2")).Partner.DependentProperties().First(), "Referential constraint in correct order");

            Assert.AreEqual("Entity1", entity1Def.Name, "Principal end is correct");
            Assert.AreEqual("Entity2", entity2Def.Name, "Dependent end is correct");
        }

        [TestMethod]
        public void VerifyFacetNullability()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""myDecimal"" Type=""Decimal""/>
    <Property Name=""myDateTime"" Type=""DateTimeOffset""/>
    <Property Name=""myString"" Type=""String""/>
  </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmComplexType typeDefinition = (IEdmComplexType)model.SchemaElements.First();
            IEdmStructuralProperty stringProp = (IEdmStructuralProperty)typeDefinition.FindProperty("myString");
            Assert.IsNull(stringProp.Type.AsString().MaxLength, "Maxlength should default to null");
            IEdmStructuralProperty decimalProp = (IEdmStructuralProperty)typeDefinition.FindProperty("myDecimal");
            Assert.AreEqual(decimalProp.Type.AsDecimal().Scale, 0, "Scale should default to 0");
            Assert.IsNull(decimalProp.Type.AsDecimal().Precision, "Precision should default to null");
            IEdmStructuralProperty datetimeProp = (IEdmStructuralProperty)typeDefinition.FindProperty("myDateTime");
            Assert.AreEqual(0, datetimeProp.Type.AsTemporal().Precision, "Temporal precision should default to 0");
        }

        [TestMethod]
        public void VerifyFacetManuallySet()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""myDecimal"" Type=""Decimal"" Precision=""3""/>
    <Property Name=""mySecondDecimal"" Type=""Decimal"" Precision=""3"" Scale=""Variable""/>
    <Property Name=""myThirdDecimal"" Type=""Decimal"" Precision=""3"" Scale=""Variable""/>
    <Property Name=""myDateTime"" Type=""DateTimeOffset"" Precision=""1""/>
    <Property Name=""myString"" Type=""String"" Unicode=""false""/>
  </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmComplexType typeDefinition = (IEdmComplexType)model.SchemaElements.First();
            IEdmStructuralProperty stringProp = (IEdmStructuralProperty)typeDefinition.FindProperty("myString");
            Assert.IsNull(stringProp.Type.AsString().MaxLength, "Maxlength should default to null");
            Assert.AreEqual(false, stringProp.Type.AsString().IsUnicode, "String unicode should be set to false");
            IEdmStructuralProperty decimalProp = (IEdmStructuralProperty)typeDefinition.FindProperty("myDecimal");
            Assert.AreEqual(decimalProp.Type.AsDecimal().Scale, 0, "Scale should default to 0");
            Assert.AreEqual(3, decimalProp.Type.AsDecimal().Precision, "Decimal precision should be set to 3");
            IEdmStructuralProperty secondDecimalProp = (IEdmStructuralProperty)typeDefinition.FindProperty("mySecondDecimal");
            Assert.IsNull(secondDecimalProp.Type.AsDecimal().Scale, "Decimal scale should be null when its value is Variable");
            Assert.AreEqual(3, secondDecimalProp.Type.AsDecimal().Precision, "Decimal precision should be set to 3");
            IEdmStructuralProperty thirdDecimalProp = (IEdmStructuralProperty)typeDefinition.FindProperty("myThirdDecimal");
            Assert.IsNull(thirdDecimalProp.Type.AsDecimal().Scale, "Decimal scale should be null when its value is variable");
            Assert.AreEqual(3, thirdDecimalProp.Type.AsDecimal().Precision, "Decimal precision should be set to 3");
            IEdmStructuralProperty datetimeProp = (IEdmStructuralProperty)typeDefinition.FindProperty("myDateTime");
            Assert.AreEqual(1, datetimeProp.Type.AsTemporal().Precision, "Temporal precision should be set to 1");
        }

        [TestMethod]
        public void RefEntityReferenceShortcutTest()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Clod"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""ID"" Type=""Int32"" />
  </EntityType>
  <ComplexType Name=""Smod"">
    <Property Name=""myRef"" Type=""Ref(Grumble.Clod)""/>
  </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> error;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out error);
            Assert.IsTrue(parsed, "parsed");

            IEdmEntityType entityTypeDefinition = (IEdmEntityType)model.SchemaElements.First();
            IEdmComplexType complexTypeDefinition = (IEdmComplexType)model.SchemaElements.Last();
            IEdmStructuralProperty refProp = (IEdmStructuralProperty)complexTypeDefinition.FindProperty("myRef");

            Assert.AreEqual(EdmTypeKind.EntityReference, refProp.Type.TypeKind(), "Property has correct type kind");
            Assert.AreEqual(entityTypeDefinition, refProp.Type.AsEntityReference().EntityReferenceDefinition().EntityType, "EntityTypeReference points to correct entity type");
        }

        [TestMethod]
        public void EntityTypeCycleTest()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Clod"" BaseType=""Grumble.Smod"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""Bar"" Type=""Int32"" />
  </EntityType>
  <EntityType Name=""Smod"" BaseType=""Grumble.Clod"">
    <Property Name=""Foo"" Type=""Int32"" />
  </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType entityTypeDefinitionSmod = (IEdmEntityType)model.SchemaElements.First();
            IEdmEntityType entityTypeDefinitionClod = (IEdmEntityType)model.SchemaElements.Last();
            Assert.IsTrue(entityTypeDefinitionClod.BaseType.IsBad(), "Clod basetype is bad because of cycle");
            Assert.AreEqual(EdmErrorCode.BadCyclicEntity, entityTypeDefinitionClod.BaseType.Errors().First().ErrorCode, "Cyclic entity is cyclic?");
            Assert.IsTrue(entityTypeDefinitionSmod.BaseType.IsBad(), "Smod basetype is bad because of cycle");

            EdmValidator.Validate(model, EdmConstants.EdmVersion4, out errors);
            Assert.AreEqual(2, errors.Count(), "Correct number of errors");
            Assert.AreEqual(EdmErrorCode.BadCyclicEntity, errors.ElementAt(0).ErrorCode, "Correct error code 1");
            Assert.AreEqual(EdmErrorCode.BadCyclicEntity, errors.ElementAt(1).ErrorCode, "Correct error code 2");
        }

        [TestMethod]
        public void EntityTypeThreeWayCycleTest()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Clod"" BaseType=""Grumble.Smod"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Bar"" Type=""Int32"" />
  </EntityType>
  <EntityType Name=""Smod"" BaseType=""Grumble.Blob"">
    <Property Name=""Foo"" Type=""Int32"" />
  </EntityType>
  <EntityType Name=""Blob"" BaseType=""Grumble.Clod"">
    <Property Name=""Baz"" Type=""Int32"" />
  </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType entityTypeDefinitionSmod = (IEdmEntityType)model.SchemaElements.First();
            IEdmEntityType entityTypeDefinitionClod = (IEdmEntityType)model.SchemaElements.ElementAt(1);
            IEdmEntityType entityTypeDefinitionBlob = (IEdmEntityType)model.SchemaElements.Last();
            Assert.IsTrue(entityTypeDefinitionClod.BaseType.IsBad(), "Clod basetype is bad because of cycle");
            Assert.AreEqual(EdmErrorCode.BadCyclicEntity, entityTypeDefinitionClod.BaseType.Errors().First().ErrorCode, "Cyclic entity is cyclic?");
            Assert.IsTrue(entityTypeDefinitionSmod.BaseType.IsBad(), "Smod basetype is bad because of cycle");
            Assert.IsTrue(entityTypeDefinitionBlob.BaseType.IsBad(), "Blob basetype is bad because of cycle");

            EdmValidator.Validate(model, EdmConstants.EdmVersion4, out errors);
            Assert.AreEqual(3, errors.Count(), "Correct number of errors");
            Assert.AreEqual(EdmErrorCode.BadCyclicEntity, errors.First().ErrorCode, "Correct error code");
        }

        [TestMethod]
        public void EntityTypeCycleUninvolvedNodesAreStillOkay()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Clod"" BaseType=""Grumble.Smod"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Bar"" Type=""Int32"" />
  </EntityType>
  <EntityType Name=""Smod"" BaseType=""Grumble.Blob"">
    <Key>
        <PropertyRef Name=""Foo"" />
    </Key>
    <Property Name=""Foo"" Type=""Int32"" Nullable=""false"" />
  </EntityType>
  <EntityType Name=""Blob"" BaseType=""Grumble.Blob"">
    <Property Name=""Baz"" Type=""Int32"" />
  </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType entityTypeDefinitionSmod = (IEdmEntityType)model.SchemaElements.First();
            IEdmEntityType entityTypeDefinitionClod = (IEdmEntityType)model.SchemaElements.ElementAt(1);
            IEdmEntityType entityTypeDefinitionBlob = (IEdmEntityType)model.SchemaElements.Last();
            Assert.IsFalse(entityTypeDefinitionClod.BaseType.IsBad(), "Contagious badness not");
            Assert.IsTrue(entityTypeDefinitionBlob.BaseType.IsBad(), "Blob basetype is bad because Blob is in a cycle.");
            Assert.AreEqual(EdmErrorCode.BadCyclicEntity, entityTypeDefinitionBlob.BaseType.Errors().First().ErrorCode, "Cyclic entity is cyclic?");

            EdmValidator.Validate(model, EdmConstants.EdmVersion4, out errors);
            Assert.AreEqual(1, errors.Count(), "Correct number of errors");
            Assert.AreEqual(EdmErrorCode.BadCyclicEntity, errors.First().ErrorCode, "Correct error code");
        }

        [TestMethod]
        public void ComplexTypeCycleTest()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Clod"" BaseType=""Grumble.Smod"">
    <Property Name=""Bar"" Type=""Int32"" />
  </ComplexType>
  <ComplexType Name=""Smod"" BaseType=""Grumble.Clod"">
    <Property Name=""Foo"" Type=""Int32"" />
  </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmComplexType complexTypeDefinitionSmod = (IEdmComplexType)model.SchemaElements.First();
            IEdmComplexType complexTypeDefinitionClod = (IEdmComplexType)model.SchemaElements.Last();
            Assert.IsTrue(complexTypeDefinitionClod.BaseType.IsBad(), "Clod basetype is bad because of cycle");
            Assert.AreEqual(EdmErrorCode.BadCyclicComplex, complexTypeDefinitionClod.BaseType.Errors().First().ErrorCode, "Cyclic entity is cyclic?");
            Assert.IsTrue(complexTypeDefinitionSmod.BaseType.IsBad(), "Smod basetype is bad because of cycle");

            EdmValidator.Validate(model, EdmConstants.EdmVersion4, out errors);
            Assert.AreEqual(2, errors.Count(), "Correct number of errors");
            Assert.AreEqual(1, errors.Select(e => e.ErrorCode).Distinct().Count(), "Correct number of unique errors");
            Assert.IsTrue(errors.Any(e => e.ErrorCode == EdmErrorCode.BadCyclicComplex), "Correct error code 1");
        }

        [TestMethod]
        public void OpenComplexTypeInheritTest()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Clod"" OpenType=""true"">
    <Property Name=""Bar"" Type=""Int32"" />
  </ComplexType>
  <ComplexType Name=""Smod"" BaseType=""Grumble.Clod"" OpenType=""false"">
    <Property Name=""Foo"" Type=""Int32"" />
  </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmComplexType complexTypeDefinitionSmod = (IEdmComplexType)model.SchemaElements.First();
            IEdmComplexType complexTypeDefinitionClod = (IEdmComplexType)model.SchemaElements.Last();

            EdmValidator.Validate(model, EdmConstants.EdmVersion4, out errors);
            Assert.AreEqual(1, errors.Count(), "Correct number of errors");
            Assert.AreEqual(1, errors.Select(e => e.ErrorCode).Distinct().Count(), "Correct number of unique errors");
        }

        [TestMethod]
        public void ComplexTypeThreeWayCycleTest()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Clod"" BaseType=""Grumble.Smod"">
    <Property Name=""Bar"" Type=""Int32"" />
  </ComplexType>
  <ComplexType Name=""Smod"" BaseType=""Grumble.Blob"">
    <Property Name=""Foo"" Type=""Int32"" />
  </ComplexType>
  <ComplexType Name=""Blob"" BaseType=""Grumble.Clod"">
    <Property Name=""Baz"" Type=""Int32"" />
  </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmComplexType complexTypeDefinitionSmod = (IEdmComplexType)model.SchemaElements.First();
            IEdmComplexType complexTypeDefinitionClod = (IEdmComplexType)model.SchemaElements.ElementAt(1);
            IEdmComplexType complexTypeDefinitionBlob = (IEdmComplexType)model.SchemaElements.Last();
            Assert.IsTrue(complexTypeDefinitionClod.BaseType.IsBad(), "Clod basetype is bad because of cycle");
            Assert.AreEqual(EdmErrorCode.BadCyclicComplex, complexTypeDefinitionClod.BaseType.Errors().First().ErrorCode, "Cyclic entity is cyclic?");
            Assert.IsTrue(complexTypeDefinitionSmod.BaseType.IsBad(), "Smod basetype is bad because of cycle");
            Assert.IsTrue(complexTypeDefinitionBlob.BaseType.IsBad(), "Blob basetype is bad because of cycle");

            EdmValidator.Validate(model, EdmConstants.EdmVersion4, out errors);
            Assert.AreEqual(3, errors.Count(), "Correct number of errors");
            Assert.AreEqual(1, errors.Select(e => e.ErrorCode).Distinct().Count(), "Correct number of unique errors");
            Assert.IsTrue(errors.Any(e => e.ErrorCode == EdmErrorCode.BadCyclicComplex), "Correct error code 1");
        }

        [TestMethod]
        public void TypeShortcutReturnTypeAndTypeRefRegresionTest()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Function Name='MyFunction'><ReturnType Type='Collection(MyNamespace.MyEntityType)'/>
    <Parameter Name='P1'>
      <CollectionType>
          <TypeRef Type='Collection(Binary)'/>
      </CollectionType>
    </Parameter>  
  </Function>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> error;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out error);
            Assert.IsTrue(parsed, "parsed");

            IEdmOperation operation = (IEdmOperation)model.SchemaElements.First();
            IEdmTypeReference returnType = operation.ReturnType;
            IEdmTypeReference parameterType = operation.Parameters.First().Type;

            Assert.IsFalse(returnType.IsBad(), "Function return type should not be bad");
            Assert.AreEqual(EdmTypeKind.Collection, returnType.TypeKind(), "Function return type is right kind");
            Assert.AreEqual(EdmTypeKind.Collection, parameterType.TypeKind(), "Function parameter type is right kind");
            Assert.AreEqual(EdmTypeKind.Collection, parameterType.AsCollection().ElementType().TypeKind(), "Collection is of collections");
            Assert.AreEqual(EdmPrimitiveTypeKind.Binary, parameterType.AsCollection().ElementType().AsCollection().ElementType().AsPrimitive().PrimitiveKind(), "root type is binary");
        }


        [TestMethod]
        public void CollectionElementTypeRegressionTest()
        {
            const string csdl =
            @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Function Name='MyFunction'><ReturnType Type='Collection(MyNamespace.MyEntityType)'/>
    <Parameter Name='P1'>
      <CollectionType ElementType='Collection(Binary)'>
      </CollectionType>
    </Parameter>  
  </Function>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> error;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out error);
            Assert.IsTrue(parsed, "parsed");

            IEdmOperation operation = (IEdmOperation)model.SchemaElements.First();
            IEdmTypeReference returnType = operation.ReturnType;
            IEdmTypeReference parameterType = operation.Parameters.First().Type;

            Assert.IsFalse(returnType.IsBad(), "Function return type should not be bad");
            Assert.IsFalse(returnType.AsCollection().IsBad(), "Function return type should not be bad");
            Assert.IsFalse(returnType.AsCollection().CollectionDefinition().IsBad(), "Function return type should not be bad");
            Assert.IsFalse(returnType.AsCollection().CollectionDefinition().ElementType.IsBad(), "Function return type should not be bad");
            Assert.IsTrue(returnType.AsCollection().CollectionDefinition().ElementType.Definition.IsBad(), "Function return type should be bad");
            Assert.AreEqual(EdmTypeKind.Collection, returnType.TypeKind(), "Function return type is right kind");
            Assert.AreEqual(EdmTypeKind.Collection, parameterType.TypeKind(), "Function parameter type is right kind");
            Assert.AreEqual(EdmTypeKind.Collection, parameterType.AsCollection().ElementType().TypeKind(), "Collection is of collections");
            Assert.AreEqual(EdmPrimitiveTypeKind.Binary, parameterType.AsCollection().ElementType().AsCollection().ElementType().AsPrimitive().PrimitiveKind(), "root type is binary");
        }

        [TestMethod]
        public void SpatialTypeParsingTest()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Edm.Int32"" />
    <Property Name=""myGeography"" Type=""Edm.Geography""/>
    <Property Name=""myPoint"" Type=""Edm.GeographyPoint""  />
    <Property Name=""myLineString"" Type=""Edm.GeographyLineString"" />
    <Property Name=""myPolygon"" Type=""Edm.GeographyPolygon""/>
    <Property Name=""myGeographyCollection"" Type=""Edm.GeographyCollection""/>
    <Property Name=""myMultiPolygon"" Type=""Edm.GeographyMultiPolygon""/>
    <Property Name=""myMultiLineString"" Type=""Edm.GeographyMultiLineString"" />
    <Property Name=""myMultiPoint"" Type=""Edm.GeographyMultiPoint""/>
    <Property Name=""myGeometry"" Type=""Edm.Geometry""/>
    <Property Name=""myGeometricPoint"" Type=""Edm.GeometryPoint""/>
    <Property Name=""myGeometricLineString"" Type=""Edm.GeometryLineString"" />
    <Property Name=""myGeometricPolygon"" Type=""Edm.GeometryPolygon""/>
    <Property Name=""myGeometryCollection"" Type=""Edm.GeometryCollection"" />
    <Property Name=""myGeometricMultiPolygon"" Type=""Edm.GeometryMultiPolygon""/>
    <Property Name=""myGeometricMultiLineString"" Type=""Edm.GeometryMultiLineString"" />
    <Property Name=""myGeometricMultiPoint"" Type=""Edm.GeometryMultiPoint"" SRID=""Variable"" />
    <Property Name=""myOtherGeometricMultiPoint"" Type=""Edm.GeometryMultiPoint"" SRID=""variable"" />
  </ComplexType>
</Schema>";
            IEdmModel model;
            IEnumerable<EdmError> error;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out error);
            Assert.IsTrue(parsed, "parsed");

            IEdmComplexType complexType = (IEdmComplexType)model.FindType("Grumble.Smod");
            IEdmSpatialTypeReference myGeography = complexType.FindProperty("myGeography").Type.AsSpatial();
            IEdmSpatialTypeReference myPoint = complexType.FindProperty("myPoint").Type.AsSpatial();
            IEdmSpatialTypeReference myLineString = complexType.FindProperty("myLineString").Type.AsSpatial();
            IEdmSpatialTypeReference myPolygon = complexType.FindProperty("myPolygon").Type.AsSpatial();
            IEdmSpatialTypeReference myGeographyCollection = complexType.FindProperty("myGeographyCollection").Type.AsSpatial();
            IEdmSpatialTypeReference myMultiPolygon = complexType.FindProperty("myMultiPolygon").Type.AsSpatial();
            IEdmSpatialTypeReference myMultiLineString = complexType.FindProperty("myMultiLineString").Type.AsSpatial();
            IEdmSpatialTypeReference myMultiPoint = complexType.FindProperty("myMultiPoint").Type.AsSpatial();
            IEdmSpatialTypeReference myGeometry = complexType.FindProperty("myGeometry").Type.AsSpatial();
            IEdmSpatialTypeReference myGeometricPoint = complexType.FindProperty("myGeometricPoint").Type.AsSpatial();
            IEdmSpatialTypeReference myGeometricLineString = complexType.FindProperty("myGeometricLineString").Type.AsSpatial();
            IEdmSpatialTypeReference myGeometricPolygon = complexType.FindProperty("myGeometricPolygon").Type.AsSpatial();
            IEdmSpatialTypeReference myGeometryCollection = complexType.FindProperty("myGeometryCollection").Type.AsSpatial();
            IEdmSpatialTypeReference myGeometricMultiPolygon = complexType.FindProperty("myGeometricMultiPolygon").Type.AsSpatial();
            IEdmSpatialTypeReference myGeometricMultiLineString = complexType.FindProperty("myGeometricMultiLineString").Type.AsSpatial();
            IEdmSpatialTypeReference myGeometricMultiPoint = complexType.FindProperty("myGeometricMultiPoint").Type.AsSpatial();
            IEdmSpatialTypeReference myOtherGeometricMultiPoint = complexType.FindProperty("myOtherGeometricMultiPoint").Type.AsSpatial();
            Assert.AreEqual(4326, myGeography.SpatialReferenceIdentifier, "Geography SRID correct");
            Assert.AreEqual(4326, myPoint.SpatialReferenceIdentifier, "Geography SRID correct");
            Assert.AreEqual(4326, myLineString.SpatialReferenceIdentifier, "Geography SRID correct");
            Assert.AreEqual(4326, myPolygon.SpatialReferenceIdentifier, "Geography SRID correct");
            Assert.AreEqual(4326, myGeographyCollection.SpatialReferenceIdentifier, "Geography SRID correct");
            Assert.AreEqual(4326, myMultiPolygon.SpatialReferenceIdentifier, "Geography SRID correct");
            Assert.AreEqual(4326, myMultiLineString.SpatialReferenceIdentifier, "Geography SRID correct");
            Assert.AreEqual(4326, myMultiPoint.SpatialReferenceIdentifier, "Geography SRID correct");
            Assert.AreEqual(0, myGeometry.SpatialReferenceIdentifier, "Geometry SRID correct");
            Assert.AreEqual(0, myGeometricPoint.SpatialReferenceIdentifier, "Geometry SRID correct");
            Assert.AreEqual(0, myGeometricLineString.SpatialReferenceIdentifier, "Geometry SRID correct");
            Assert.AreEqual(0, myGeometricPolygon.SpatialReferenceIdentifier, "Geometry SRID correct");
            Assert.AreEqual(0, myGeometryCollection.SpatialReferenceIdentifier, "Geometry SRID correct");
            Assert.AreEqual(0, myGeometricMultiPolygon.SpatialReferenceIdentifier, "Geometry SRID correct");
            Assert.AreEqual(0, myGeometricMultiLineString.SpatialReferenceIdentifier, "Geometry SRID correct");
            Assert.AreEqual(null, myGeometricMultiPoint.SpatialReferenceIdentifier, "Variable SRID correct");
            Assert.AreEqual(null, myOtherGeometricMultiPoint.SpatialReferenceIdentifier, "variable SRID correct");
        }

        [TestMethod]
        public void Parse3SchemaCyclicBassTypeRegressionTest()
        {
            const string inputText1 =
        @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Real1"" Alias=""Real1_Alias"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Complex1"" BaseType=""Real2_Alias.Complex2"">
    <Property Name=""Prop1"" Type=""Edm.Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";

            const string inputText2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Real2"" Alias=""Real2_Alias"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Complex2"" BaseType=""Real3_Alias.Complex3"">
    <Property Name=""Prop2"" Type=""Edm.Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";

            const string inputText3 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Real3"" Alias=""Real3_Alias"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Complex3"" BaseType=""Real1_Alias.Complex1"">
    <Property Name=""Prop3"" Type=""Edm.Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(inputText1)), XmlReader.Create(new StringReader(inputText2)), XmlReader.Create(new StringReader(inputText3)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmComplexType complex1 = (IEdmComplexType)model.SchemaElements.ElementAt(0);
            IEdmComplexType complex2 = (IEdmComplexType)model.SchemaElements.ElementAt(1);
            IEdmComplexType complex3 = (IEdmComplexType)model.SchemaElements.ElementAt(2);

            Assert.AreEqual(complex1.BaseComplexType().FullName(), complex2.FullName(), "Correct base type name");
            Assert.AreEqual(complex2.BaseComplexType().FullName(), complex3.FullName(), "Correct base type name");
            Assert.AreEqual(complex3.BaseComplexType().FullName(), complex1.FullName(), "Correct base type name");

            Assert.IsTrue(complex1.BaseType.IsBad(), "Cyclic base type is bad");
            Assert.AreEqual(EdmErrorCode.BadCyclicComplex, complex1.BaseType.Errors().First().ErrorCode, "Cyclic is cyclic");
            Assert.IsTrue(complex2.BaseType.IsBad(), "Cyclic base type is bad");
            Assert.IsTrue(complex3.BaseType.IsBad(), "Cyclic base type is bad");

            EdmValidator.Validate(model, EdmConstants.EdmVersion4, out errors);
            Assert.AreEqual(3, errors.Count(), "Correct number of errors");
            Assert.AreEqual(1, errors.Select(e => e.ErrorCode).Distinct().Count(), "Correct number of unique errors");
            Assert.IsTrue(errors.Any(e => e.ErrorCode == EdmErrorCode.BadCyclicComplex), "Correct error code 1");
        }

        [TestMethod]
        public void TestBadEntitySet()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Person"">
        <Key>
          <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
      </EntityType>
      <Action Name=""peopleWhoAreAwesome""><ReturnType Type=""Collection(foo.Person)""/>
        <Parameter Name=""awesomeName"" Type=""Edm.String"" />
      </Action>
      <EntityContainer Name=""fooContainer"">
        <EntitySet Name=""People"" EntityType=""foo.Person"" />
        <ActionImport Name=""peopleWhoAreAwesome"" Action=""foo.peopleWhoAreAwesome"" EntitySet=""BadEntitySet"" />
      </EntityContainer>
    </Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new [] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(!errors.Any(), "No errors");

            IEdmEntityContainer fooContainer = model.FindEntityContainer("fooContainer");
            IEdmOperationImport peopleWhoAreAwesome = (IEdmOperationImport)fooContainer.Elements.Last();
            IEdmEntitySetBase badSet;
            Assert.IsFalse(peopleWhoAreAwesome.TryGetStaticEntitySet(model, out badSet), "peopleWhoAreAwesome.TryGetStaticEntitySet");

            Assert.IsFalse(EdmValidator.Validate(model, EdmConstants.EdmVersion4, out errors), "EdmValidator.Validate(model");
            Assert.AreEqual(1, errors.Count(), "Correct number of errors");
            Assert.AreEqual(EdmErrorCode.OperationImportEntitySetExpressionIsInvalid, errors.First().ErrorCode, "Correct error code");
        }

        [TestMethod]
        public void TestEnumTypeParsingPositive()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EnumType Name=""Color"">
      <Member Name=""Red"" Value=""10""/>
      <Member Name=""Green""/>
      <Member Name=""Blue"" Value=""5""/>
      <Member Name=""Yellow""/>
    </EnumType>
    <EnumType Name=""Color2"" IsFlags=""true"" UnderlyingType=""Edm.Int64"">
      <Member Name=""Red"" Value=""5""/>
      <Member Name=""Green""/>
      <Member Name=""Blue"" Value=""10""/>
      <Member Name=""Yellow""/>
    </EnumType>
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""c1"" Type=""foo.Color"" Nullable=""true"" />
        <Property Name=""c2"" Type=""foo.Color2"" Nullable=""false"" />
        <Property Name=""c3"" Type=""foo.Color2"" />
    </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            var colorType = model.FindType("foo.Color") as IEdmEnumType;
            Assert.IsNotNull(colorType, "foo.Color enum type exists");
            Assert.AreEqual("Color", colorType.Name, "Color.Name");
            Assert.IsFalse(colorType.IsFlags, "Color.IsFlags");
            Assert.IsFalse(colorType.IsBad(), "Color.IsBad");
            Assert.AreEqual(EdmPrimitiveTypeKind.Int32, colorType.UnderlyingType.PrimitiveKind, "Color.UnderlyingType.Kind");
            Assert.AreEqual(4, colorType.Members.Count(), "Color has 4 members");
            var colors = colorType.Members.ToArray();
            Assert.AreEqual("Blue", colors[2].Name, "Color.Blue.Name");
            Assert.AreEqual(colorType, colors[2].DeclaringType, "Color.Blue.DeclaringType");

            var color2Type = model.FindType("foo.Color2") as IEdmEnumType;
            Assert.IsNotNull(color2Type, "foo.Color2 enum type exists");
            Assert.AreEqual("Color2", color2Type.Name, "Color2.Name");
            Assert.IsTrue(color2Type.IsFlags, "Color2.IsFlags");
            Assert.IsFalse(color2Type.IsBad(), "Color2.IsBad");
            Assert.AreEqual(EdmPrimitiveTypeKind.Int64, color2Type.UnderlyingType.PrimitiveKind, "Color2.UnderlyingType.Kind");
            Assert.AreEqual(4, colorType.Members.Count(), "Color2 has 4 members");
            var colors2 = color2Type.Members.ToArray();
            Assert.AreEqual("Blue", colors2[2].Name, "Color2.Blue.Name");
            Assert.AreEqual(color2Type, colors2[2].DeclaringType, "Color2.Blue.DeclaringType");

            var personType = model.FindType("foo.Person") as IEdmEntityType;
            Assert.IsNotNull(personType, "foo.Person type exists");
            Assert.AreEqual(colorType, personType.Properties().ElementAt(1).Type.AsEnum().EnumDefinition(), "Person.c1.Type == foo.Color");
            Assert.IsTrue(personType.Properties().ElementAt(1).Type.AsEnum().IsNullable, "Person.c1.Type.Nullable");
            Assert.AreEqual(color2Type, personType.Properties().ElementAt(2).Type.AsEnum().EnumDefinition(), "Person.c2.Type == foo.Color2");
            Assert.IsFalse(personType.Properties().ElementAt(2).Type.AsEnum().IsNullable, "Person.c2.Type.Nullable");
            Assert.IsTrue(personType.Properties().ElementAt(3).Type.AsEnum().IsNullable, "Person.c3.Type.Nullable");

            Assert.AreEqual((color2Type.Members.ElementAt(0).Value).Value, 5, "Correct value");
            Assert.AreEqual((color2Type.Members.ElementAt(1).Value).Value, 6, "Correct value");
            Assert.AreEqual((color2Type.Members.ElementAt(2).Value).Value, 10, "Correct value");
            Assert.AreEqual((color2Type.Members.ElementAt(3).Value).Value, 11, "Correct value");

            Assert.AreEqual((colorType.Members.ElementAt(0).Value).Value, 10, "Correct value");
            Assert.AreEqual((colorType.Members.ElementAt(1).Value).Value, 11, "Correct value");
            Assert.AreEqual((colorType.Members.ElementAt(2).Value).Value, 5, "Correct value");
            Assert.AreEqual((colorType.Members.ElementAt(3).Value).Value, 6, "Correct value");
        }

        [TestMethod]
        public void TestEnumTypeParsingOutOfRange()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EnumType Name=""Color"" UnderlyingType=""Edm.Int64"" >
      <Member Name=""Red"" Value=""9223372036854775807""/>
      <Member Name=""Green""/>
      <Member Name=""Blue"" Value=""5""/>
      <Member Name=""Yellow""/>
    </EnumType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.AreEqual(0, errors.Count(), "No errors");

            bool valid = model.Validate(out errors);
            Assert.IsFalse(valid, "valid");
            Assert.AreEqual(1, errors.Count(), "one validation errors expected");
            Assert.AreEqual(EdmErrorCode.EnumMemberMustHaveValue, errors.ElementAt(0).ErrorCode, "EnumMemberMustHaveValue expected");
            Assert.IsTrue(errors.ElementAt(0).ErrorLocation.ToString().Contains("(4, 8)"), "error location");

            IEdmEnumMember enumMemeber = ((IEdmEnumType)model.FindType("foo.Color")).Members.First(m => m.Name == "Green");
            Assert.AreEqual(model.FindType("foo.Color"), enumMemeber.DeclaringType, "Enum member has correct type.");
            Assert.IsTrue(enumMemeber.Value.IsBad(), "Enum member value is bad.");
        }

        [TestMethod]
        public void TestEnumTypeParsingOutOfInt32Range()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EnumType Name=""Color"" UnderlyingType=""Edm.Int32"" >
      <Member Name=""Red"" Value=""2147483647""/>
      <Member Name=""Green""/>
      <Member Name=""Blue"" Value=""5""/>
      <Member Name=""Yellow""/>
    </EnumType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.AreEqual(0, errors.Count(), "No errors");

            bool valid = model.Validate(out errors);
            Assert.IsFalse(valid, "valid");
            Assert.AreEqual(1, errors.Count(), "one validation errors expected");
            Assert.AreEqual(EdmErrorCode.EnumMemberValueOutOfRange, errors.ElementAt(0).ErrorCode, "EnumMemberValueOutOfRange expected");
            Assert.IsTrue(errors.ElementAt(0).ErrorLocation.ToString().Contains("(4, 8)"), "error location");

            IEdmEnumMember enumMemeber = ((IEdmEnumType)model.FindType("foo.Color")).Members.First(m => m.Name == "Green");
            Assert.AreEqual(model.FindType("foo.Color"), enumMemeber.DeclaringType, "Enum member has correct type.");
            Assert.IsFalse(enumMemeber.Value.IsBad(), "Enum member value is good.");
        }

        [TestMethod]
        public void TestEnumTypeParsingSetDefaultValues()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EnumType Name=""Color"" UnderlyingType=""Edm.Int32"" >
      <Member Name=""Red""/>
      <Member Name=""Green""/>
      <Member Name=""Blue"" Value=""5""/>
      <Member Name=""Yellow""/>
    </EnumType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.AreEqual(0, errors.Count(), "No errors");

            bool valid = model.Validate(out errors);
            Assert.IsTrue(valid, "valid");
            Assert.AreEqual(0, errors.Count(), "no validation errors");

            IEdmEnumType enumType = ((IEdmEnumType)model.FindType("foo.Color"));
            Assert.AreEqual((enumType.Members.ElementAt(0).Value).Value, 0, "Correct value");
            Assert.AreEqual((enumType.Members.ElementAt(1).Value).Value, 1, "Correct value");
            Assert.AreEqual((enumType.Members.ElementAt(2).Value).Value, 5, "Correct value");
            Assert.AreEqual((enumType.Members.ElementAt(3).Value).Value, 6, "Correct value");
        }

        [TestMethod]
        public void TestEnumTypeParsingNonPrimiveUnderlyingType()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""c1"" Type=""foo.Color"" Nullable=""true"" />
    </EntityType>
    <EnumType Name=""Color"" UnderlyingType=""foo.Person"">
      <Member Name=""Red""/>
      <Member Name=""Green""/>
      <Member Name=""Blue""/>
      <Member Name=""Yellow""/>
    </EnumType>
    <EnumType Name=""Permission"" UnderlyingType=""Edm.String"">
      <Member Name=""Read""/>
      <Member Name=""Write""/>
    </EnumType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            bool valid = model.Validate(out errors);
            Assert.IsFalse(valid, "valid");
            Assert.AreEqual(4, errors.Count(), "one validation error expected");

            Assert.AreEqual(EdmErrorCode.BadUnresolvedPrimitiveType, errors.ElementAt(0).ErrorCode, "BadUnresolvedPrimitiveType expected");
            Assert.IsTrue(errors.ElementAt(0).ErrorLocation.ToString().Contains("(9, 6)"), "error location");

            Assert.AreEqual(EdmErrorCode.EnumMustHaveIntegerUnderlyingType, errors.ElementAt(1).ErrorCode, "EnumMustHaveIntegerUnderlyingType expected");
            Assert.IsTrue(errors.ElementAt(1).ErrorLocation.ToString().Contains("(15, 6)"), "error location");
        }

        [TestMethod]
        public void TestTerms()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Age"" Type=""Int32"" />
    <Term Name=""Subject"" Type=""foo.Person"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
    </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmTerm age = model.FindTerm("foo.Age");
            IEdmTerm subject = model.FindTerm("foo.Subject");

            Assert.AreEqual(age.Name, "Age", "Term name");
            Assert.AreEqual(subject.Name, "Subject", "Term name");

            Assert.AreEqual(age.Type.AsPrimitive().PrimitiveKind(), EdmPrimitiveTypeKind.Int32, "Term type");
            Assert.AreEqual(subject.Type.AsEntity().FullName(), "foo.Person", "Term type");

            Assert.AreEqual(age.Namespace, "foo", "Term namespace");
        }

        [TestMethod]
        public void TestRelaxingOrderingAnnotationAndAttributes()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
        <Annotation Term=""foo.Age"" Qualifier=""First"" Int=""123"" />
        <Annotation Term=""foo.Age"" Qualifier=""Best"">
            <Int>456</Int>
        </Annotation>
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Annotation Term=""Funk.Mage"" Int=""789"" />
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
    </EntityType>
    <Term Name=""Age"" Type=""Int32"" />
    <Term Name=""Subject"" Type=""foo.Person"" />
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;

            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType person = (IEdmEntityType)model.FindType("foo.Person");
            IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model);
            Assert.AreEqual(personAnnotations.Count(), 3, "Annotations count");

            IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();
            IEdmVocabularyAnnotation first = annotations[0];
            Assert.AreEqual(first.Qualifier, "First", "Annotation qualifier");
            Assert.AreEqual(first.Term.Name, "Age", "Term name");
            Assert.AreEqual(first.Term.Namespace, "foo", "Term namespace");

            IEdmIntegerConstantExpression value = (IEdmIntegerConstantExpression)first.Value;
            Assert.AreEqual(value.Value, 123, "Annotation value");

            IEdmVocabularyAnnotation second = annotations[1];
            Assert.AreEqual(second.Qualifier, "Best", "Annotation qualifier");
            Assert.AreEqual(second.Term.Name, "Age", "Term name");
            Assert.AreEqual(second.Term.Namespace, "foo", "Term namespace");

            value = (IEdmIntegerConstantExpression)second.Value;
            Assert.AreEqual(value.Value, 456, "Annotation value");

            IEdmVocabularyAnnotation third = annotations[2];
            Assert.IsNull(third.Qualifier, "Annotation qualifier");
            Assert.AreEqual(third.Term.Name, "Mage", "Term name");
            Assert.AreEqual(third.Term.Namespace, "Funk", "Term namespace");

            value = (IEdmIntegerConstantExpression)third.Value;
            Assert.AreEqual(value.Value, 789, "Annotation value");
        }

        [TestMethod]
        public void TestVocabularyAnnotations()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Age"" Type=""Int32"" />
    <Term Name=""Subject"" Type=""foo.Person"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
        <Annotation Term=""foo.Age"" Qualifier=""First"" Int=""123"" />
        <Annotation Term=""foo.Age"" Qualifier=""Best"">
            <Int>456</Int>
        </Annotation>
        <Annotation Term=""Funk.Mage"" Int=""789"" />
    </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType person = (IEdmEntityType)model.FindType("foo.Person");
            IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model);
            Assert.AreEqual(personAnnotations.Count(), 3, "Annotations count");

            IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();
            IEdmVocabularyAnnotation first = annotations[0];
            Assert.AreEqual(first.Qualifier, "First", "Annotation qualifier");
            Assert.AreEqual(first.Term.Name, "Age", "Term name");
            Assert.AreEqual(first.Term.Namespace, "foo", "Term namespace");

            IEdmIntegerConstantExpression value = (IEdmIntegerConstantExpression)first.Value;
            Assert.AreEqual(value.Value, 123, "Annotation value");

            IEdmVocabularyAnnotation second = annotations[1];
            Assert.AreEqual(second.Qualifier, "Best", "Annotation qualifier");
            Assert.AreEqual(second.Term.Name, "Age", "Term name");
            Assert.AreEqual(second.Term.Namespace, "foo", "Term namespace");

            value = (IEdmIntegerConstantExpression)second.Value;
            Assert.AreEqual(value.Value, 456, "Annotation value");

            IEdmVocabularyAnnotation third = annotations[2];
            Assert.IsNull(third.Qualifier, "Annotation qualifier");
            Assert.AreEqual(third.Term.Name, "Mage", "Term name");
            Assert.AreEqual(third.Term.Namespace, "Funk", "Term namespace");
            value = (IEdmIntegerConstantExpression)third.Value;
            Assert.AreEqual(value.Value, 789, "Annotation value");
        }

        [TestMethod]
        public void TestOutOfLineAnnotations()
        {
            const string csdl =
@"<Schema Namespace=""foo"" Alias=""Zorb"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Age"" Type=""Int32"" />
    <Term Name=""Subject"" Type=""foo.Person"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
    </EntityType>
    <EntityType Name=""LyingPerson"" BaseType=""Edm.TypeTerm"">
        <Property Name=""Sobriquet"" Type=""String"" Nullable=""false"" />
        <Property Name=""FictionalAge"" Type=""Int32"" Nullable=""false"" />
    </EntityType>
    <Annotations Target=""Zorb.Person"">
        <Annotation Term=""foo.Age"" Qualifier=""First"" Int=""123"" />
        <Annotation Term=""foo.Age"" Qualifier=""Best"">
            <Int>456</Int>
        </Annotation>
        <Annotation Term=""Funk.Mage"" Int=""789"" />
    </Annotations>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType person = (IEdmEntityType)model.FindType("foo.Person");
            IEdmEntityType lyingPerson = (IEdmEntityType)model.FindType("foo.LyingPerson");

            IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model);
            Assert.AreEqual(personAnnotations.Count(), 3, "Annotations count");
            IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();

            IEdmVocabularyAnnotation first = annotations[0];
            Assert.AreEqual(first.Qualifier, "First", "Annotation qualifier");
            Assert.AreEqual(first.Term.Name, "Age", "Term name");
            Assert.AreEqual(first.Term.Namespace, "foo", "Term namespace");
            IEdmIntegerConstantExpression value = (IEdmIntegerConstantExpression)first.Value;
            Assert.AreEqual(value.Value, 123, "Annotation value");

            IEdmVocabularyAnnotation second = annotations[1];
            Assert.AreEqual(second.Qualifier, "Best", "Annotation qualifier");
            Assert.AreEqual(second.Term.Name, "Age", "Term name");
            Assert.AreEqual(second.Term.Namespace, "foo", "Term namespace");
            value = (IEdmIntegerConstantExpression)second.Value;
            Assert.AreEqual(value.Value, 456, "Annotation value");

            IEdmVocabularyAnnotation third = annotations[2];
            Assert.IsNull(third.Qualifier, "Annotation qualifier");
            Assert.AreEqual(third.Term.Name, "Mage", "Term name");
            Assert.AreEqual(third.Term.Namespace, "Funk", "Term namespace");
            value = (IEdmIntegerConstantExpression)third.Value;
            Assert.AreEqual(value.Value, 789, "Annotation value");
        }

        [TestMethod]
        public void TestMixedInlineAndOutOfLineAnnotations()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Age"" Type=""Int32"" />
    <Term Name=""Subject"" Type=""foo.Person"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
        <Annotation Term=""foo.Age"" Qualifier=""Middling"">
            <Int>777</Int>
        </Annotation>
        <Annotation Term=""Var1.Mage"" Qualifier=""Middling"" Int=""888"" />
    </EntityType>
    <EntityType Name=""LyingPerson"" BaseType=""Edm.TypeTerm"">
        <Property Name=""Sobriquet"" Type=""String"" Nullable=""false"" />
        <Property Name=""FictionalAge"" Type=""Int32"" Nullable=""false"" />
    </EntityType>
    <Annotations Target=""foo.Person"">
        <Annotation Term=""foo.Age"" Qualifier=""First"" Int=""123"" />
        <Annotation Term=""foo.Age"" Qualifier=""Best"">
            <Int>456</Int>
        </Annotation>
        <Annotation Term=""Funk.Mage"" Int=""789"" />
    </Annotations>
    <Annotations Target=""foo.Person"" Qualifier=""Zonky"">
        <Annotation Term=""Var1.Yage"" Int=""555"" />
    </Annotations>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType person = (IEdmEntityType)model.FindType("foo.Person");
            IEdmEntityType lyingPerson = (IEdmEntityType)model.FindType("foo.LyingPerson");

            IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model);
            Assert.AreEqual(personAnnotations.Count(), 6, "Annotations count");
            IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();

            IEdmVocabularyAnnotation first = annotations[0];
            Assert.AreEqual(first.Qualifier, "Middling", "Annotation qualifier");
            Assert.AreEqual(first.Term.Name, "Age", "Term name");
            Assert.AreEqual(first.Term.Namespace, "foo", "Term namespace");
            IEdmIntegerConstantExpression value = (IEdmIntegerConstantExpression)first.Value;
            Assert.AreEqual(value.Value, 777, "Annotation value");

            IEdmVocabularyAnnotation second = annotations[1];
            Assert.AreEqual(second.Qualifier, "Middling", "Annotation qualifier");
            Assert.AreEqual(second.Term.Name, "Mage", "Term name");
            Assert.AreEqual(second.Term.Namespace, "Var1", "Term namespace");
            value = (IEdmIntegerConstantExpression)second.Value;
            Assert.AreEqual(value.Value, 888, "Annotation value");

            IEdmVocabularyAnnotation third = annotations[2];
            Assert.AreEqual(third.Qualifier, "First", "Annotation qualifier");
            Assert.AreEqual(third.Term.Name, "Age", "Term name");
            Assert.AreEqual(third.Term.Namespace, "foo", "Term namespace");
            value = (IEdmIntegerConstantExpression)third.Value;
            Assert.AreEqual(value.Value, 123, "Annotation value");

            IEdmVocabularyAnnotation fourth = annotations[3];
            Assert.AreEqual(fourth.Qualifier, "Best", "Annotation qualifier");
            Assert.AreEqual(fourth.Term.Name, "Age", "Term name");
            Assert.AreEqual(fourth.Term.Namespace, "foo", "Term namespace");
            value = (IEdmIntegerConstantExpression)fourth.Value;
            Assert.AreEqual(value.Value, 456, "Annotation value");

            IEdmVocabularyAnnotation fifth = annotations[4];
            Assert.IsNull(fifth.Qualifier, "Annotation qualifier");
            Assert.AreEqual(fifth.Term.Name, "Mage", "Term name");
            Assert.AreEqual(fifth.Term.Namespace, "Funk", "Term namespace");
            value = (IEdmIntegerConstantExpression)fifth.Value;
            Assert.AreEqual(value.Value, 789, "Annotation value");

            IEdmVocabularyAnnotation sixth = annotations[5];
            Assert.AreEqual(sixth.Qualifier, "Zonky", "Annotation qualifier");
            Assert.AreEqual(sixth.Term.Name, "Yage", "Term name");
            Assert.AreEqual(sixth.Term.Namespace, "Var1", "Term namespace");
            value = (IEdmIntegerConstantExpression)sixth.Value;
            Assert.AreEqual(value.Value, 555, "Annotation value");
        }

        [TestMethod]
        public void TestExternalAnnotations()
        {
            const string csdl1 =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""DistantAge"" Type=""Int32"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
        <Annotation Term=""Funk.Mage"" Int=""789"" />
    </EntityType>
    <EntityType Name=""LyingPerson"" BaseType=""Edm.TypeTerm"">
        <Property Name=""Sobriquet"" Type=""String"" Nullable=""false"" />
        <Property Name=""FictionalAge"" Type=""Int32"" Nullable=""false"" />
    </EntityType>
</Schema>";

            const string csdl2 =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Age"" Type=""Int32"" />
    <Term Name=""Subject"" Type=""foo.Person"" />
    <Annotations Target=""foo.Person"">
        <Annotation Term=""foo.Age"" Qualifier=""First"" Int=""123"" />
        <Annotation Term=""foo.Age"" Qualifier=""Best"">
            <Int>456</Int>
        </Annotation>
        <Annotation Term=""foo.DistantAge"" Int=""99"" />
    </Annotations>
</Schema>";

            IEdmModel model1;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl1)) }, out model1, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmModel model2;
            parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl2)) }, new IEdmModel[] { model1 }, out model2, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType person = (IEdmEntityType)model1.FindType("foo.Person");
            IEdmEntityType lyingPerson = (IEdmEntityType)model1.FindType("foo.LyingPerson");
            IEdmTerm distantAge = model1.FindTerm("foo.DistantAge");
            Assert.AreEqual(lyingPerson, model2.FindType("foo.LyingPerson"), "Lookup through referenced model");

            IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model2);
            Assert.AreEqual(personAnnotations.Count(), 4, "Annotations count");

            IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();
            IEdmVocabularyAnnotation third = annotations[0];
            Assert.AreEqual(third.Qualifier, "First", "Annotation qualifier");
            Assert.AreEqual(third.Term.Name, "Age", "Term name");
            Assert.AreEqual(third.Term.Namespace, "foo", "Term namespace");

            IEdmIntegerConstantExpression value = (IEdmIntegerConstantExpression)third.Value;
            Assert.AreEqual(value.Value, 123, "Annotation value");

            IEdmVocabularyAnnotation fourth = annotations[1];
            Assert.AreEqual(fourth.Qualifier, "Best", "Annotation qualifier");
            Assert.AreEqual(fourth.Term.Name, "Age", "Term name");
            Assert.AreEqual(fourth.Term.Namespace, "foo", "Term namespace");

            value = (IEdmIntegerConstantExpression)fourth.Value;
            Assert.AreEqual(value.Value, 456, "Annotation value");

            IEdmVocabularyAnnotation fifth = annotations[2];
            Assert.IsNull(fifth.Qualifier, "Annotation qualifier");
            Assert.AreEqual(fifth.Term.Name, "DistantAge", "Term name");
            Assert.AreEqual(fifth.Term.Namespace, "foo", "Term namespace");
            Assert.AreEqual(fifth.Term, distantAge, "Annotation term");

            value = (IEdmIntegerConstantExpression)fifth.Value;
            Assert.AreEqual(value.Value, 99, "Annotation value");

            IEdmVocabularyAnnotation sixth = annotations[3];
            Assert.IsNull(sixth.Qualifier, "Annotation qualifier");
            Assert.AreEqual(sixth.Term.Name, "Mage", "Term name");
            Assert.AreEqual(sixth.Term.Namespace, "Funk", "Term namespace");

            value = (IEdmIntegerConstantExpression)sixth.Value;
            Assert.AreEqual(value.Value, 789, "Annotation value");

            Assert.AreEqual(1, model1.VocabularyAnnotations.Count(), "Model annotations count");
            Assert.AreEqual(3, model2.VocabularyAnnotations.Count(), "Model annotations count");
        }

        [TestMethod]
        public void TestAnnotatingEntitySet()
        {
            const string csdl =
@"<Schema Namespace=""foo"" Alias=""Zorb"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
    </EntityType>
    <EntityContainer Name=""bar"">
        <EntitySet Name=""PersonsOfInterest"" EntityType=""foo.Person"" />
    </EntityContainer>
    <Annotations Target=""foo.bar/PersonsOfInterest"">
        <Annotation Term=""Funk.Mage"" Int=""789"" />
    </Annotations>
</Schema>";

            const string csdl2 =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.bar/PersonsOfInterest"">
        <Annotation Term=""Funk.Strange"" Int=""13"" />
    </Annotations>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmModel model2;
            parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl2)) }, new IEdmModel[] { model }, out model2, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntitySet persons = model.FindEntityContainer("bar").FindEntitySet("PersonsOfInterest");

            IEnumerable<IEdmVocabularyAnnotation> personsAnnotations = persons.VocabularyAnnotations(model);
            Assert.AreEqual(personsAnnotations.Count(), 1, "Annotations count");
            IEdmVocabularyAnnotation[] annotations = personsAnnotations.ToArray<IEdmVocabularyAnnotation>();

            IEdmVocabularyAnnotation mage = annotations[0];
            Assert.IsNull(mage.Qualifier, "Annotation qualifier");
            Assert.AreEqual(mage.Term.Name, "Mage", "Term name");
            Assert.AreEqual(mage.Term.Namespace, "Funk", "Term namespace");
            IEdmIntegerConstantExpression value = (IEdmIntegerConstantExpression)mage.Value;
            Assert.AreEqual(value.Value, 789, "Annotation value");

            personsAnnotations = persons.VocabularyAnnotations(model2);
            Assert.AreEqual(personsAnnotations.Count(), 2, "Annotations count");
            annotations = personsAnnotations.ToArray<IEdmVocabularyAnnotation>();

            IEdmVocabularyAnnotation strange = annotations[0];
            Assert.AreEqual(persons, strange.Target, "Annotation target");
            Assert.IsNull(strange.Qualifier, "Annotation qualifier");
            Assert.AreEqual(strange.Term.Name, "Strange", "Term name");
            Assert.AreEqual(strange.Term.Namespace, "Funk", "Term namespace");
            value = (IEdmIntegerConstantExpression)strange.Value;
            Assert.AreEqual(value.Value, 13, "Annotation value");

            mage = annotations[1];
            Assert.AreEqual(persons, mage.Target, "Annotation target");
            Assert.IsNull(mage.Qualifier, "Annotation qualifier");
            Assert.AreEqual(mage.Term.Name, "Mage", "Term name");
            Assert.AreEqual(mage.Term.Namespace, "Funk", "Term namespace");
            value = (IEdmIntegerConstantExpression)mage.Value;
            Assert.AreEqual(value.Value, 789, "Annotation value");
        }

        [TestMethod]
        public void TestAnnotatingProperties()
        {
            const string csdl =
@"<Schema Namespace=""foo"" Alias=""Zorb"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"">
            <Annotation Term=""Funk.ADelic"" Int=""17"" />
        </Property>
    </EntityType>
    <Annotations Target=""foo.Person/Birthday"">
        <Annotation Term=""Funk.Mage"" Int=""789"" />
    </Annotations>
</Schema>";

            const string csdl2 =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person/Birthday"">
        <Annotation Term=""Funk.Strange"" Int=""13"" />
    </Annotations>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmModel model2;
            parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl2)) }, model, out model2, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmProperty birthday = ((IEdmEntityType)model.FindType("foo.Person")).FindProperty("Birthday");

            IEnumerable<IEdmVocabularyAnnotation> birthdayAnnotations = birthday.VocabularyAnnotations(model);
            Assert.AreEqual(birthdayAnnotations.Count(), 2, "Annotations count");
            IEdmVocabularyAnnotation[] annotations = birthdayAnnotations.ToArray<IEdmVocabularyAnnotation>();

            IEdmVocabularyAnnotation funkadelic = annotations[0];
            Assert.IsNull(funkadelic.Qualifier, "Annotation qualifier");
            Assert.AreEqual(funkadelic.Term.Name, "ADelic", "Term name");
            Assert.AreEqual(funkadelic.Term.Namespace, "Funk", "Term namespace");
            IEdmIntegerConstantExpression value = (IEdmIntegerConstantExpression)funkadelic.Value;
            Assert.AreEqual(value.Value, 17, "Annotation value");

            IEdmVocabularyAnnotation mage = annotations[1];
            Assert.IsNull(mage.Qualifier, "Annotation qualifier");
            Assert.AreEqual(mage.Term.Name, "Mage", "Term name");
            Assert.AreEqual(mage.Term.Namespace, "Funk", "Term namespace");
            value = (IEdmIntegerConstantExpression)mage.Value;
            Assert.AreEqual(value.Value, 789, "Annotation value");

            birthdayAnnotations = birthday.VocabularyAnnotations(model2);
            Assert.AreEqual(birthdayAnnotations.Count(), 3, "Annotations count");
            annotations = birthdayAnnotations.ToArray<IEdmVocabularyAnnotation>();

            IEdmVocabularyAnnotation strange = annotations[0];
            Assert.IsNull(strange.Qualifier, "Annotation qualifier");
            Assert.AreEqual(strange.Term.Name, "Strange", "Term name");
            Assert.AreEqual(strange.Term.Namespace, "Funk", "Term namespace");
            value = (IEdmIntegerConstantExpression)strange.Value;
            Assert.AreEqual(value.Value, 13, "Annotation value");

            funkadelic = annotations[1];
            Assert.IsNull(funkadelic.Qualifier, "Annotation qualifier");
            Assert.AreEqual(funkadelic.Term.Name, "ADelic", "Term name");
            Assert.AreEqual(funkadelic.Term.Namespace, "Funk", "Term namespace");
            value = (IEdmIntegerConstantExpression)funkadelic.Value;
            Assert.AreEqual(value.Value, 17, "Annotation value");

            mage = annotations[2];
            Assert.IsNull(mage.Qualifier, "Annotation qualifier");
            Assert.AreEqual(mage.Term.Name, "Mage", "Term name");
            Assert.AreEqual(mage.Term.Namespace, "Funk", "Term namespace");
            value = (IEdmIntegerConstantExpression)mage.Value;
            Assert.AreEqual(value.Value, 789, "Annotation value");
        }

        [TestMethod]
        public void TestConstantExpressions()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
        <Annotation Term=""Funk.IC"" Qualifier=""In"" Int=""1"" />
        <Annotation Term=""Funk.IC"" Qualifier=""Out"">
            <Int>2</Int>
        </Annotation>
        <Annotation Term=""Funk.SC"" Qualifier=""In"" String=""Cat"" />
        <Annotation Term=""Funk.SC"" Qualifier=""Out"">
            <String>Dog</String>
        </Annotation>
        <Annotation Term=""Funk.BinC"" Qualifier=""In"" Binary=""123456"" />
        <Annotation Term=""Funk.BinC"" Qualifier=""Out"">
            <Binary>654321</Binary>
        </Annotation>
        <Annotation Term=""Funk.FC"" Qualifier=""In"" Float=""1.1"" />
        <Annotation Term=""Funk.FC"" Qualifier=""Out"">
            <Float>2.2E10</Float>
        </Annotation>
        <Annotation Term=""Funk.GC"" Qualifier=""In"" Guid=""4ae71c81-c21a-40a2-8d53-f1a29ed4a2f2"" />
        <Annotation Term=""Funk.GC"" Qualifier=""Out"">
            <Guid>4ae71c81-c21a-40a2-8d53-f1a29ed4a2f3</Guid>
        </Annotation>
        <Annotation Term=""Funk.DC"" Qualifier=""In"" Decimal=""1.2"" />
        <Annotation Term=""Funk.DC"" Qualifier=""Out"">
            <Decimal>2.3</Decimal>
        </Annotation>
        <Annotation Term=""Funk.BC"" Qualifier=""In"" Bool=""true"" />
        <Annotation Term=""Funk.BC"" Qualifier=""Out"">
            <Bool>false</Bool>
        </Annotation>
        <Annotation Term=""Funk.DTOfsC"" Qualifier=""In"" DateTimeOffset=""2001-10-26T19:32:52+00:00"" />
        <Annotation Term=""Funk.DTOfsC"" Qualifier=""Out"">
            <DateTimeOffset>2001-10-26T19:32:52+00:00</DateTimeOffset>
        </Annotation>
        <Annotation Term=""Funk.TC"" Qualifier=""In"" Duration=""PT0M1S"" />
        <Annotation Term=""Funk.TC"" Qualifier=""Out"">
            <Duration>PT0M1S</Duration>
        </Annotation>
    </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType person = (IEdmEntityType)model.FindType("foo.Person");

            IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model);
            Assert.AreEqual(personAnnotations.Count(), 18, "Annotations count");
            IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();

            var byteArrayComparer = new ArrayComp();

            int i = 0;
            Assert.AreEqual((annotations[i]).Value.ExpressionKind, EdmExpressionKind.IntegerConstant, "Annotation expression kind");
            Assert.AreEqual(((IEdmIntegerConstantExpression)(annotations[i]).Value).Value, 1, "Annotation value");
            Assert.AreEqual((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.IntegerConstant, "Annotation expression kind");
            Assert.AreEqual(((IEdmIntegerConstantExpression)(annotations[i]).Value).Value, 2, "Annotation value");

            Assert.AreEqual((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.StringConstant, "Annotation expression kind");
            Assert.AreEqual(((IEdmStringConstantExpression)(annotations[i]).Value).Value, "Cat", "Annotation value");
            Assert.AreEqual((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.StringConstant, "Annotation expression kind");
            Assert.AreEqual(((IEdmStringConstantExpression)(annotations[i]).Value).Value, "Dog", "Annotation value");

            Assert.AreEqual((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.BinaryConstant, "Annotation expression kind");
            Assert.IsTrue(byteArrayComparer.Equals(((IEdmBinaryConstantExpression)(annotations[i]).Value).Value, new byte[] { 0x12, 0x34, 0x56 }), "Annotation value");
            Assert.AreEqual((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.BinaryConstant, "Annotation expression kind");
            Assert.IsTrue(byteArrayComparer.Equals(((IEdmBinaryConstantExpression)(annotations[i]).Value).Value, new byte[] { 0x65, 0x43, 0x21 }), "Annotation value");

            Assert.AreEqual((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.FloatingConstant, "Annotation expression kind");
            Assert.AreEqual(((IEdmFloatingConstantExpression)(annotations[i]).Value).Value, 1.1, "Annotation value");
            Assert.AreEqual((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.FloatingConstant, "Annotation expression kind");
            Assert.AreEqual(((IEdmFloatingConstantExpression)(annotations[i]).Value).Value, 2.2E10, "Annotation value");

            Assert.AreEqual((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.GuidConstant, "Annotation expression kind");
            Assert.AreEqual(((IEdmGuidConstantExpression)(annotations[i]).Value).Value, Guid.Parse("4ae71c81-c21a-40a2-8d53-f1a29ed4a2f2"), "Annotation value");
            Assert.AreEqual((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.GuidConstant, "Annotation expression kind");
            Assert.AreEqual(((IEdmGuidConstantExpression)(annotations[i]).Value).Value, Guid.Parse("4ae71c81-c21a-40a2-8d53-f1a29ed4a2f3"), "Annotation value");

            Assert.AreEqual((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.DecimalConstant, "Annotation expression kind");
            Assert.AreEqual(((IEdmDecimalConstantExpression)(annotations[i]).Value).Value, 1.2M, "Annotation value");
            Assert.AreEqual((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.DecimalConstant, "Annotation expression kind");
            Assert.AreEqual(((IEdmDecimalConstantExpression)(annotations[i]).Value).Value, 2.3M, "Annotation value");

            Assert.AreEqual((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.BooleanConstant, "Annotation expression kind");
            Assert.AreEqual(((IEdmBooleanConstantExpression)(annotations[i]).Value).Value, true, "Annotation value");
            Assert.AreEqual((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.BooleanConstant, "Annotation expression kind");
            Assert.AreEqual(((IEdmBooleanConstantExpression)(annotations[i]).Value).Value, false, "Annotation value");

            Assert.AreEqual((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.DateTimeOffsetConstant, "Annotation expression kind");
            Assert.AreEqual(((IEdmDateTimeOffsetConstantExpression)(annotations[i]).Value).Value, new DateTimeOffset(2001, 10, 26, 19, 32, 52, new TimeSpan(0, 0, 0)), "Annotation value");
            Assert.AreEqual((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.DateTimeOffsetConstant, "Annotation expression kind");
            Assert.AreEqual(((IEdmDateTimeOffsetConstantExpression)(annotations[i]).Value).Value, new DateTimeOffset(2001, 10, 26, 19, 32, 52, new TimeSpan(0, 0, 0)), "Annotation value");

            Assert.AreEqual((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.DurationConstant, "Annotation expression kind");
            Assert.AreEqual(new TimeSpan(0, 0, 1), ((IEdmDurationConstantExpression)(annotations[i]).Value).Value, "Annotation value");
            Assert.AreEqual((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.DurationConstant, "Annotation expression kind");
            Assert.AreEqual(new TimeSpan(0, 0, 1), ((IEdmDurationConstantExpression)(annotations[i]).Value).Value, "Annotation value");
        }

        private class ArrayComp : IEqualityComparer<byte[]>
        {
            public static readonly ArrayComp Instance = new ArrayComp();
            public bool Equals(byte[] x, byte[] y)
            {
                if (x == null && y == null)
                {
                    return true;
                }
                if (x == null || y == null || x.Length != y.Length)
                {
                    return false;
                }
                for (int i = 0; i < x.Length; ++i)
                {
                    if (x[i] != y[i])
                        return false;
                }
                return true;
            }

            public int GetHashCode(byte[] obj)
            {
                throw new NotSupportedException();
            }
        }

        [TestMethod]
        public void TestRecordExpressions()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
        <Annotation Term=""Funk.RC"">
            <Record>
                <PropertyValue Property=""X"" Int=""10"" />
                <PropertyValue Property=""Y"">
                    <Int>20</Int>
                </PropertyValue>
            </Record>
        </Annotation>
    </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType person = (IEdmEntityType)model.FindType("foo.Person");

            IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model);
            Assert.AreEqual(personAnnotations.Count(), 1, "Annotations count");
            IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();

            IEdmVocabularyAnnotation first = annotations[0];
            Assert.AreEqual(first.Value.ExpressionKind, EdmExpressionKind.Record, "Annotation expression kind");

            IEdmRecordExpression record = (IEdmRecordExpression)first.Value;
            IEnumerable<IEdmPropertyConstructor> propertyConstructors = record.Properties;
            Assert.AreEqual(propertyConstructors.Count(), 2, "Property constructors count");
            IEdmPropertyConstructor[] properties = propertyConstructors.ToArray<IEdmPropertyConstructor>();

            IEdmPropertyConstructor x = properties[0];
            Assert.AreEqual(x.Name, "X", "Property name");
            Assert.AreEqual(x.Value.ExpressionKind, EdmExpressionKind.IntegerConstant, "Property expression kind");
            Assert.AreEqual(((IEdmIntegerConstantExpression)x.Value).Value, 10, "Property expression value");

            IEdmPropertyConstructor y = properties[1];
            Assert.AreEqual(y.Name, "Y", "Property name");
            Assert.AreEqual(y.Value.ExpressionKind, EdmExpressionKind.IntegerConstant, "Property expression kind");
            Assert.AreEqual(((IEdmIntegerConstantExpression)y.Value).Value, 20, "Property expression value");
        }

        [TestMethod]
        public void TestPathExpressions()
        {
            const string csdl =
@"<Schema Namespace=""foo"" Alias=""Zorb"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Age"" Type=""Int32"" />
    <Term Name=""Subject"" Type=""foo.Person"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Age"" Type=""Int32"" />
        <Annotation Term=""Funk.Mage"">
            <Path>Age</Path>
        </Annotation>
    </EntityType>
    <EntityType Name=""LyingPerson"" BaseType=""Edm.TypeTerm"">
        <Property Name=""Sobriquet"" Type=""String"" Nullable=""false"" />
        <Property Name=""FictionalAge"" Type=""Int32"" Nullable=""false"" />
    </EntityType>
    <Annotations Target=""Zorb.Person"">
        <Annotation Term=""foo.Age"" Path=""Age"" />
    </Annotations>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType person = (IEdmEntityType)model.FindType("foo.Person");
            IEdmProperty name = person.FindProperty("Name");
            IEdmProperty age = person.FindProperty("Age");
            IEdmEntityType lyingPerson = (IEdmEntityType)model.FindType("foo.LyingPerson");

            IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model);
            Assert.AreEqual(personAnnotations.Count(), 2, "Annotations count");
            IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();

            IEdmVocabularyAnnotation first = annotations[0];
            Assert.AreEqual(first.Term.Name, "Mage", "Term name");
            IEdmPathExpression value = (IEdmPathExpression)first.Value;
            Assert.AreEqual(value.PathSegments.First(), "Age", "Bound path name");

            IEdmVocabularyAnnotation second = annotations[1];
            Assert.AreEqual(second.Term.Name, "Age", "Term name");
            value = (IEdmPathExpression)second.Value;
            Assert.AreEqual(value.PathSegments.First(), "Age", "Bound path name");
        }

        [TestMethod]
        public void TestReturnTypeAndTypeRefWithoutTypeAttribute()
        {
            var csdl = @"
<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""C1"">
      <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
      <EntitySet Name=""Addresses"" EntityType=""NS1.Address"" />
    </EntityContainer>
    <EntityType Name=""Person"">
      <Key>
          <PropertyRef Name=""ID"" />
      </Key>
      <Property Name=""ID"" Type=""Int32"" Nullable=""false"" />
      <NavigationProperty Name=""Addresses"" Type=""Collection(NS1.Address)"" />
    </EntityType>
    <EntityType Name=""Address"">
      <Key>
          <PropertyRef Name=""ID"" />
      </Key>
      <Property Name=""ID"" Type=""Int32"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""Customer"" BaseType=""NS1.Person"">
      <Property Name=""Preferred"" Nullable=""false"" />
      <Property Name=""Foo"" Type=""Collection"">
        <TypeRef Name=""Edm.String"" />
      </Property>
    </EntityType>
    <Function Name=""Foo"">
        <ReturnType>
            <CollectionType>
               <TypeRef />
            </CollectionType>
        </ReturnType>
    </Function>
</Schema>";
            IEdmModel edmModel;
            IEnumerable<EdmError> errors;
            var isParsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out edmModel, out errors);
            Assert.IsFalse(isParsed, "SchemaReader.TryParse failed");
            Assert.IsTrue(errors.Count() == 4, "SchemaReader.TryParse returned errors");
            Assert.AreEqual(EdmErrorCode.UnexpectedXmlElement, errors.OrderBy(e => e.ErrorCode).ElementAt(0).ErrorCode, "EdmErrorCode.UnexpectedXmlElement");
            Assert.AreEqual(EdmErrorCode.MissingAttribute, errors.OrderBy(e => e.ErrorCode).ElementAt(1).ErrorCode, "EdmErrorCode.MissingAttribute");
            Assert.AreEqual(EdmErrorCode.MissingType, errors.OrderBy(e => e.ErrorCode).ElementAt(3).ErrorCode, "EdmErrorCode.MissingType");
        }

        [TestMethod]
        public void AnnotationNamespaceRegressionTest()
        {
            var csdl = @"<Schema Namespace=""Netflix.Catalog.v2"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Annotations Target=""Self.Person"">
    <Annotation Term=""Org.OData.Vocabularies.Core.Name"">
      <Path>Name</Path>
    </Annotation>
  </Annotations>
</Schema>";
            IEdmModel edmModel;
            IEnumerable<EdmError> errors;
            var isParsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out edmModel, out errors);
            Assert.IsTrue(isParsed, "SchemaReader.TryParse failed");

            IEdmVocabularyAnnotation annotation = edmModel.VocabularyAnnotations.First();
            Assert.AreEqual("Netflix.Catalog.v2", ((IEdmSchemaElement)annotation.Target).Namespace, "Namespace is resolved correctly");
        }

        [TestMethod]
        public void TestSchemaAliasWithMultiplePeriods()
        {
            string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""bee.bee.bee"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""bee.bee.bee.Smod"">
        <Annotation Term=""bee.bee.bee.value"" Int=""1"" />
    </Annotations>
    <Term Name=""value"" Type=""Int32"" />
</Schema>";
            var csdls = new List<XElement> { XElement.Parse(csdl) };
            IEdmModel model;
            IEnumerable<EdmError> errors;
            var isParsed = SchemaReader.TryParse(csdls.Select(e => e.CreateReader()), out model, out errors);
            Assert.IsFalse(isParsed, "SchemaReader.TryParse failed");
            Assert.AreEqual(1, errors.Count(), "Invalid error count.");
            Assert.AreEqual(EdmErrorCode.InvalidQualifiedName, errors.First().ErrorCode, "Invalid error code.");
        }

        [TestMethod]
        public void TestSchemaAliasWithPeriod()
        {
            string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""bee.bee"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""bee.bee.Smod"">
        <Annotation Term=""bee.bee.bee.value"" Int=""1"" />
    </Annotations>
    <Term Name=""value"" Type=""Int32"" />
</Schema>";
            var csdls = new List<XElement> { XElement.Parse(csdl) };
            IEdmModel model;
            IEnumerable<EdmError> errors;
            var isParsed = SchemaReader.TryParse(csdls.Select(e => e.CreateReader()), out model, out errors);
            Assert.IsFalse(isParsed, "SchemaReader.TryParse failed");
            Assert.AreEqual(1, errors.Count(), "Invalid error count.");
            Assert.AreEqual(EdmErrorCode.InvalidQualifiedName, errors.First().ErrorCode, "Invalid error code.");
        }

        [TestMethod]
        public void TestingAnnotationQualifiersWithNonSimpleValue()
        {
            var csdl = VocabularyTestModelBuilder.AnnotationQualifiersWithNonSimpleValue();
            IEdmModel model = this.GetParserResult(csdl);

            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(model, Microsoft.Test.OData.Utils.Metadata.EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void TestParsingSchemaWithoutNamespace()
        {
            var csdl = XElement.Parse(@"
<Schema xmlns='http://docs.oasis-open.org/odata/ns/edm'>
    <Term Name='VocabsDemo.MovieTerm' Type='MediaVocab.Movie'/>
    <Annotations Target='VocabsDemo.Film'>
        <Annotation Term='VocabsDemo.MovieTerm'>
            <Record>
                <PropertyValue Property='Title' Path='Name' />
                <PropertyValue Property='Synopsis' Path='Description' />
                <PropertyValue Property='Rating' Path='Rating' />
                <PropertyValue Property='Image' Path='BoxArt' />
                <PropertyValue Property='Year' Path='ReleaseDate' />
            </Record>
        </Annotation>
    </Annotations>
</Schema>");
            var actualCsdls = new XElement[] { csdl };

            var model = this.GetParserResult(actualCsdls);
            Assert.AreEqual(1, model.VocabularyAnnotations.Count(), "Invalid annotation count.");

            IEnumerable<EdmError> errors;
            var roundTripCsdls = this.GetSerializerResult(model, Microsoft.Test.OData.Utils.Metadata.EdmVersion.V40, out errors).Select(n => XElement.Parse(n));
            Assert.AreEqual(0, errors.Count(), "Invalid error count.");
            Assert.AreEqual(1, roundTripCsdls.Count(), "Invalid csdl count.");

            new ConstructiveApiCsdlXElementComparer().Compare(actualCsdls.ToList(), roundTripCsdls.ToList());
        }

        [TestMethod]
        public void TestComputeAnnotationTargetThatUsesAlias()
        {
            var csdl = @"
<Schema Namespace='Netflix.Catalog.v2' Alias='Self' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <Annotations Target='Self.Person'>
    <Annotation Term='Org.OData.Vocabularies.Name'>
      <Path>Name</Path>
    </Annotation>
  </Annotations>
</Schema>";

            var model = this.GetParserResult(new string[] { csdl });
            Assert.AreEqual(1, model.VocabularyAnnotations.Count(), "Invalid annotation count.");

            var annotationTarget = model.VocabularyAnnotations.First().Target;
            Assert.IsTrue(annotationTarget.ToString().Contains("Netflix.Catalog.v2.Person"), "Invalid target ToString().");
        }

        [TestMethod]
        public void ParseOpenComplexTypesModel()
        {
            const string csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""MyNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplexType"" OpenType=""true"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
  </ComplexType>
  <ComplexType Name=""MyComplexType1"" OpenType=""false"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
  </ComplexType>
  <ComplexType Name=""MyComplexType2"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsTrue(parsed, "parsed");
            Assert.IsFalse(errors.Any(), "No errors");

            var types = model.SchemaElements.OfType<IEdmComplexType>().ToList();
            Assert.IsTrue(types.Count == 3, "Should have three types");
            Assert.IsTrue(types[0].IsOpen, "Should be open type");
            Assert.IsFalse(types[1].IsOpen, "Should not be open type");
            Assert.IsFalse(types[2].IsOpen, "Should not be open type");
        }

        [TestMethod]
        public void TestTypeDefinition()
        {
            const string csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""MyNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <TypeDefinition Name=""Length"" UnderlyingType=""Edm.Int32"" />
  <TypeDefinition Name=""Width"" UnderlyingType=""Edm.Int32"">
    <Annotation Term=""Org.OData.Measurements.V1.Unit"" String=""Centimeters"" />
  </TypeDefinition>
  <TypeDefinition Name=""Weight"" UnderlyingType=""Edm.Decimal"">
    <Annotation Term=""Org.OData.Measurements.V1.Unit"" String=""Kilograms"" />
  </TypeDefinition>
  <TypeDefinition Name=""Address"" UnderlyingType=""Edm.String"" />
  <TypeDefinition Name=""Point"" UnderlyingType=""Edm.GeographyPoint"" />
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Weight"" Type=""MyNS.Weight"" Precision=""3"" Scale=""2"" />
    <Property Name=""Address"" Type=""MyNS.Address"" MaxLength=""10"" Unicode=""true"" />
    <Property Name=""Address2"" Type=""MyNS.Address"" MaxLength=""max"" Unicode=""false"" />
    <Property Name=""Point"" Type=""MyNS.Point"" SRID=""123"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""People"" EntityType=""MyNS.Person"" />
  </EntityContainer>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsTrue(parsed, "parsed");
            Assert.IsFalse(errors.Any(), "No errors");

            var elements = model.SchemaElements.ToList();

            // Length
            var lengthType = (IEdmTypeDefinition)elements[0];
            Assert.AreEqual(EdmPrimitiveTypeKind.Int32, lengthType.UnderlyingType.PrimitiveKind);
            Assert.AreEqual(EdmSchemaElementKind.TypeDefinition, lengthType.SchemaElementKind);
            Assert.AreEqual("Length", lengthType.Name);

            // Width
            var widthType = (IEdmTypeDefinition)elements[1];
            Assert.AreEqual(EdmPrimitiveTypeKind.Int32, widthType.UnderlyingType.PrimitiveKind);
            Assert.AreEqual(EdmSchemaElementKind.TypeDefinition, widthType.SchemaElementKind);
            Assert.AreEqual("Width", widthType.Name);
            var widthElement = ((CsdlSemanticsElement)widthType).Element;
            Assert.AreEqual(1, widthElement.VocabularyAnnotations.Count());

            // Weight
            var weightType = (IEdmTypeDefinition)elements[2];
            Assert.AreEqual(EdmPrimitiveTypeKind.Decimal, weightType.UnderlyingType.PrimitiveKind);
            Assert.AreEqual(EdmSchemaElementKind.TypeDefinition, weightType.SchemaElementKind);
            Assert.AreEqual("Weight", weightType.Name);
            var weightElement = ((CsdlSemanticsElement)weightType).Element;
            Assert.AreEqual("Edm.Decimal", ((CsdlTypeDefinition)weightElement).UnderlyingTypeName);

            // Address
            var addressType = (IEdmTypeDefinition)elements[3];
            Assert.AreEqual(EdmPrimitiveTypeKind.String, addressType.UnderlyingType.PrimitiveKind);
            Assert.AreEqual(EdmSchemaElementKind.TypeDefinition, addressType.SchemaElementKind);
            Assert.AreEqual("Address", addressType.Name);
            var addressElement = ((CsdlSemanticsElement)addressType).Element;
            Assert.AreEqual("Edm.String", ((CsdlTypeDefinition)addressElement).UnderlyingTypeName);

            // Point
            var pointType = (IEdmTypeDefinition)elements[4];
            Assert.AreEqual(EdmPrimitiveTypeKind.GeographyPoint, pointType.UnderlyingType.PrimitiveKind);
            Assert.AreEqual(EdmSchemaElementKind.TypeDefinition, pointType.SchemaElementKind);
            Assert.AreEqual("Point", pointType.Name);
            var pointElement = ((CsdlSemanticsElement)pointType).Element;
            Assert.AreEqual("Edm.GeographyPoint", ((CsdlTypeDefinition)pointElement).UnderlyingTypeName);

            // Person
            var personType = elements[5] as IEdmEntityType;
            var weightProperty = personType.FindProperty("Weight");
            var addressProperty = personType.FindProperty("Address");
            var addressProperty2 = personType.FindProperty("Address2");
            var pointProperty = personType.FindProperty("Point");
            Assert.AreEqual(weightProperty.Type.AsTypeDefinition().FullName(), "MyNS.Weight");
            Assert.AreEqual(weightProperty.Type.AsTypeDefinition().Definition, weightType);
            Assert.AreEqual(addressProperty.Type.AsTypeDefinition().FullName(), "MyNS.Address");
            Assert.AreEqual(addressProperty.Type.AsTypeDefinition().Definition, addressType);
            Assert.AreEqual(addressProperty2.Type.AsTypeDefinition().FullName(), "MyNS.Address");
            Assert.AreEqual(addressProperty2.Type.AsTypeDefinition().Definition, addressType);
            Assert.AreEqual(pointProperty.Type.AsTypeDefinition().FullName(), "MyNS.Point");
            Assert.AreEqual(pointProperty.Type.AsTypeDefinition().Definition, pointType);

            // Facets
            Assert.AreEqual(false, ((IEdmTypeDefinitionReference)weightProperty.Type).IsUnbounded);
            Assert.AreEqual(null, ((IEdmTypeDefinitionReference)weightProperty.Type).MaxLength);
            Assert.AreEqual(null, ((IEdmTypeDefinitionReference)weightProperty.Type).IsUnicode);
            Assert.AreEqual(3, ((IEdmTypeDefinitionReference)weightProperty.Type).Precision);
            Assert.AreEqual(2, ((IEdmTypeDefinitionReference)weightProperty.Type).Scale);
            Assert.AreEqual(null, ((IEdmTypeDefinitionReference)weightProperty.Type).SpatialReferenceIdentifier);

            Assert.AreEqual(false, ((IEdmTypeDefinitionReference)addressProperty.Type).IsUnbounded);
            Assert.AreEqual(10, ((IEdmTypeDefinitionReference)addressProperty.Type).MaxLength);
            Assert.AreEqual(true, ((IEdmTypeDefinitionReference)addressProperty.Type).IsUnicode);
            Assert.AreEqual(null, ((IEdmTypeDefinitionReference)addressProperty.Type).Precision);
            Assert.AreEqual(null, ((IEdmTypeDefinitionReference)addressProperty.Type).Scale);
            Assert.AreEqual(null, ((IEdmTypeDefinitionReference)addressProperty.Type).SpatialReferenceIdentifier);

            Assert.AreEqual(true, ((IEdmTypeDefinitionReference)addressProperty2.Type).IsUnbounded);
            Assert.AreEqual(null, ((IEdmTypeDefinitionReference)addressProperty2.Type).MaxLength);
            Assert.AreEqual(false, ((IEdmTypeDefinitionReference)addressProperty2.Type).IsUnicode);
            Assert.AreEqual(null, ((IEdmTypeDefinitionReference)addressProperty2.Type).Precision);
            Assert.AreEqual(null, ((IEdmTypeDefinitionReference)addressProperty2.Type).Scale);
            Assert.AreEqual(null, ((IEdmTypeDefinitionReference)addressProperty2.Type).SpatialReferenceIdentifier);

            Assert.AreEqual(false, ((IEdmTypeDefinitionReference)pointProperty.Type).IsUnbounded);
            Assert.AreEqual(null, ((IEdmTypeDefinitionReference)pointProperty.Type).MaxLength);
            Assert.AreEqual(null, ((IEdmTypeDefinitionReference)pointProperty.Type).IsUnicode);
            Assert.AreEqual(null, ((IEdmTypeDefinitionReference)pointProperty.Type).Precision);
            Assert.AreEqual(null, ((IEdmTypeDefinitionReference)pointProperty.Type).Scale);
            Assert.AreEqual(123, ((IEdmTypeDefinitionReference)pointProperty.Type).SpatialReferenceIdentifier);
        }

        [TestMethod]
        public void TestAllKindsOfUnsignedIntegers()
        {
            const string csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""MyNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <TypeDefinition Name=""UInt16"" UnderlyingType=""Edm.Int32"" />
  <TypeDefinition Name=""UInt32"" UnderlyingType=""Edm.Int64"" />
  <TypeDefinition Name=""UInt64"" UnderlyingType=""Edm.Decimal"" />
  <EntityType Name=""Person"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""MyNS.UInt32"" Nullable=""false"" />
    <Property Name=""ShortInt"" Type=""MyNS.UInt16"" />
    <Property Name=""LongInt"" Type=""MyNS.UInt64"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""People"" EntityType=""MyNS.Person"" />
  </EntityContainer>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsTrue(parsed, "parsed");
            Assert.IsFalse(errors.Any(), "No errors");

            var elements = model.SchemaElements.ToList();

            // UInt16
            Assert.AreEqual(((IEdmTypeDefinition)elements[0]).UnderlyingType.PrimitiveKind, EdmPrimitiveTypeKind.Int32);
            Assert.AreEqual(((IEdmSchemaElement)elements[0]).SchemaElementKind, EdmSchemaElementKind.TypeDefinition);
            Assert.AreEqual(((IEdmNamedElement)elements[0]).Name, "UInt16");

            // UInt32
            Assert.AreEqual(((IEdmTypeDefinition)elements[1]).UnderlyingType.PrimitiveKind, EdmPrimitiveTypeKind.Int64);
            Assert.AreEqual(elements[1].SchemaElementKind, EdmSchemaElementKind.TypeDefinition);
            Assert.AreEqual(((IEdmNamedElement)elements[1]).Name, "UInt32");

            // UInt64
            Assert.AreEqual(((IEdmTypeDefinition)elements[2]).UnderlyingType.PrimitiveKind, EdmPrimitiveTypeKind.Decimal);
            Assert.AreEqual(elements[2].SchemaElementKind, EdmSchemaElementKind.TypeDefinition);
            Assert.AreEqual(((IEdmNamedElement)elements[2]).Name, "UInt64");

            // Person
            var personType = elements[3] as IEdmEntityType;
            Assert.IsNotNull(personType, "MyNS.Person");
            var idProperty = personType.FindProperty("Id");
            var shortIntProperty = personType.FindProperty("ShortInt");
            var longIntProperty = personType.FindProperty("LongInt");
            Assert.AreEqual(idProperty.Type.AsTypeDefinition().FullName(), "MyNS.UInt32");
            Assert.AreEqual(shortIntProperty.Type.AsTypeDefinition().FullName(), "MyNS.UInt16");
            Assert.AreEqual(longIntProperty.Type.AsTypeDefinition().FullName(), "MyNS.UInt64");
        }

        [TestMethod]
        public void TestDateRelatedEdmType()
        {
            const string csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""MyNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm""> 
  <EntityType Name=""Person"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""dateTimeOffset"" Type=""Edm.DateTimeOffset"" />
    <Property Name=""date"" Type=""Edm.Date"" />
    <Property Name=""timeofday"" Type=""Edm.TimeOfDay"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""People"" EntityType=""MyNS.Person"" />
  </EntityContainer>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsTrue(parsed, "parsed");
            Assert.IsFalse(errors.Any(), "No errors");

            var elements = model.SchemaElements.ToList();

            // Person
            var personType = elements[0] as IEdmEntityType;
            Assert.IsNotNull(personType, "MyNS.Person");
            var idProperty = personType.FindProperty("Id");
            var dateTimeOffsetProperty = personType.FindProperty("dateTimeOffset");
            var dateProperty = personType.FindProperty("date");
            var timeOfDayProperty = personType.FindProperty("timeofday");
            Assert.AreEqual(idProperty.Type.AsTypeDefinition().FullName(), "Edm.Int32");
            Assert.AreEqual(dateTimeOffsetProperty.Type.AsTypeDefinition().FullName(), "Edm.DateTimeOffset");
            Assert.AreEqual(dateProperty.Type.AsTypeDefinition().FullName(), "Edm.Date");
            Assert.AreEqual(timeOfDayProperty.Type.AsTypeDefinition().FullName(), "Edm.TimeOfDay");
        }

        [TestMethod]
        public void TestParseDefaultValueAttributeOfTermElement()
        {
            const string csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""MyNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""ConventionalIDs"" Type=""Core.Tag"" DefaultValue=""True"" AppliesTo=""EntityContainer"">
    <Annotation Term=""Core.Description"" String=""Entity-ids follow OData URL conventions""/>
  </Term>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsTrue(parsed, "parsed");
            Assert.IsFalse(errors.Any(), "No errors");

            var term = model.FindTerm("MyNS.ConventionalIDs");
            Assert.AreEqual(term.DefaultValue, "True");
        }


        [TestMethod]
        public void TestDuplicateTypeDefinitions()
        {
            const string csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""MyNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <TypeDefinition Name=""Length"" UnderlyingType=""Edm.Int32"" />
  <TypeDefinition Name=""Length"" UnderlyingType=""Edm.Double"" />
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            IEnumerable<EdmError> validationErrors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsTrue(parsed, "parsed");
            Assert.IsFalse(errors.Any(), "No errors");

            model.Validate(out validationErrors);
            Assert.AreEqual(validationErrors.Single().ErrorMessage, "An element with the name 'MyNS.Length' is already defined.");
        }

        [TestMethod]
        public void TestDuplicateTypeDefinitionWithEntityType()
        {
            const string csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""MyNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <TypeDefinition Name=""Person"" UnderlyingType=""Edm.Int32"" />
  <EntityType Name=""Person"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
  </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            IEnumerable<EdmError> validationErrors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsTrue(parsed, "parsed");
            Assert.IsFalse(errors.Any(), "No errors");

            model.Validate(out validationErrors);
            Assert.AreEqual(validationErrors.Single().ErrorMessage, "An element with the name 'MyNS.Person' is already defined.");
        }

        [TestMethod]
        public void TestDuplicateKeyPropertiesInEntityType()
        {
            const string csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""MyNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
  </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            IEnumerable<EdmError> validationErrors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsTrue(parsed, "parsed");
            Assert.IsFalse(errors.Any(), "No errors");

            model.Validate(out validationErrors);
            Assert.AreEqual(validationErrors.Single().ErrorMessage, "Each property name in a type must be unique. Property name 'Id' is already defined.");
        }

        [TestMethod]
        public void TestDuplicateTypeDefinitionWithComplexType()
        {
            const string csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""MyNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <TypeDefinition Name=""Person"" UnderlyingType=""Edm.Int32"" />
  <ComplexType Name=""Person"">
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            IEnumerable<EdmError> validationErrors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsTrue(parsed, "parsed");
            Assert.IsFalse(errors.Any(), "No errors");

            model.Validate(out validationErrors);
            Assert.AreEqual(validationErrors.Single().ErrorMessage, "An element with the name 'MyNS.Person' is already defined.");
        }

        [TestMethod]
        public void ValidationOnModelWithComputedAnnotation()
        {
            var csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Edm.Int32"" />
    <Property Name=""TimeStampValue"" Type=""Edm.Binary"">
    <Annotation Bool=""true"" Term=""Org.OData.Core.V1.Computed""/>
    <Annotation Bool=""false"" Term=""Org.OData.Core.V1.IsURL""/>
    <Annotation Term=""Org.OData.Measurements.V1.Unit"" String=""Kilograms"" />
    </Property>
  </ComplexType>
</Schema>";
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.AreEqual(0, errors.Count());
            bool valid = model.Validate(out errors);
            Assert.IsTrue(valid, "valid");
        }
    }
}
