//---------------------------------------------------------------------
// <copyright file="SchemaParsingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class SchemaParsingTests : EdmLibTestCaseBase
{
    [Fact]
    public void SchemaReadEmptyModel()
    {
        var csdl = @"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm""/>";

        IEdmModel model;
        IEnumerable<EdmError> errors;
        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");
    }

    [Fact]
    public void SchemaReadEmptyModel2()
    {
        var csdl = @"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm""></Schema>";

        IEdmModel model;
        IEnumerable<EdmError> errors;
        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        Assert.Equal("C1", model.EntityContainer.Name, "model.EntityContainers.First().Name = C1");
        Assert.Equal("Customers", model.EntityContainer.Elements.Single().Name, "model.EntityContainers.Single().Elements.Single().Name = Customers");
        Assert.Equal("NS1.Customer", model.SchemaElements.Single(e => e.FullName() == "NS1.Customer").FullName(), "model.SchemaElements.Single().FullName() = NS1.Customer");
        Assert.Equal("CustomerID", ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer")).DeclaredStructuralProperties().Single().Name, "model.SchemaElements.Single().DeclaredStructuralProperties.Single().Name = CustomerID");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        Assert.Equal("C1", model.EntityContainer.Name, "model.EntityContainers.First().Name = C1");
        Assert.Equal("Customers", model.EntityContainer.Elements.Single().Name, "model.EntityContainers.Single().Elements.Single().Name = Customers");
        Assert.Equal("NS1.Customer", model.SchemaElements.Single(e => e.FullName() == "NS1.Customer").FullName(), "model.SchemaElements.Single().FullName() = NS1.Customer");
        Assert.Equal(
            "CustomerID",
            ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer")).DeclaredStructuralProperties().First().Name, "model.SchemaElements.Single().DeclaredStructuralProperties.Single().Name = CustomerID");
        Assert.Equal(
            "Names",
            ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer")).DeclaredStructuralProperties().Last().Name, "model.SchemaElements.Single().DeclaredStructuralProperties.Single().Name = Names");

        IEdmEntityType customer = ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer"));
        IEdmStructuralProperty names = customer.DeclaredStructuralProperties().Last();
        IEdmTypeReference typeRef = names.Type;
        Assert.False(typeRef.IsNullable);
        IEdmCollectionType type = typeRef.Definition as IEdmCollectionType;
        Assert.NotNull(type);
        Assert.Equal(EdmTypeKind.Collection, type.TypeKind);
        IEdmTypeReference elementType = type.ElementType;
        Assert.False(elementType.IsNullable);
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        Assert.Equal("C1", model.EntityContainer.Name, "model.EntityContainers.First().Name = C1");
        Assert.Equal("Customers", model.EntityContainer.Elements.Single().Name, "model.EntityContainers.Single().Elements.Single().Name = Customers");
        Assert.Equal("NS1.Customer", model.SchemaElements.Single(e => e.FullName() == "NS1.Customer").FullName(), "model.SchemaElements.Single().FullName() = NS1.Customer");
        Assert.Equal(
            "CustomerID",
            ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer")).DeclaredStructuralProperties().First().Name, "model.SchemaElements.Single().DeclaredStructuralProperties.Single().Name = CustomerID");
        Assert.Equal(
            "Names",
            ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer")).DeclaredStructuralProperties().Last().Name, "model.SchemaElements.Single().DeclaredStructuralProperties.Single().Name = Names");

        IEdmEntityType customer = ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer"));
        IEdmStructuralProperty names = customer.DeclaredStructuralProperties().Last();
        IEdmCollectionTypeReference typeRef = names.Type as IEdmCollectionTypeReference;
        Assert.NotNull(typeRef);
        Assert.True(typeRef.IsNullable);
        IEdmCollectionType type = typeRef.Definition as IEdmCollectionType;
        Assert.NotNull(type);
        Assert.Equal(EdmTypeKind.Collection, type.TypeKind);
        IEdmTypeReference elementType = type.ElementType;
        Assert.True(elementType.IsNullable);
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        bool result = model.Validate(out errors);
        Assert.True(result);
        Assert.True(errors.Count() == 0, "No errors");

        IEdmEntityType customer = ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer"));
        IEdmNavigationProperty pets = customer.DeclaredNavigationProperties().Single();
        IEdmTypeReference typeRef = pets.Type;
        Assert.True(typeRef.IsNullable);
        IEdmCollectionType type = typeRef.Definition as IEdmCollectionType;
        Assert.NotNull(type);
        Assert.Equal(EdmTypeKind.Collection, type.TypeKind);
        IEdmTypeReference elementType = type.ElementType;
        Assert.False(elementType.IsNullable);
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(!errors.Any(), "No errors");

        bool result = model.Validate(out errors);
        Assert.False(result);
        Assert.True(errors.Count() == 1, "Has errors");
        Assert.Equal(EdmErrorCode.NavigationPropertyWithCollectionTypeCannotHaveNullableAttribute, errors.First().ErrorCode);
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(!errors.Any(), "No errors");

        bool result = model.Validate(out errors);
        Assert.False(result);
        Assert.True(errors.Count() == 1, "Has errors");
        Assert.Equal(EdmErrorCode.NavigationPropertyWithCollectionTypeCannotHaveNullableAttribute, errors.First().ErrorCode);
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmEntityContainer wild = model.EntityContainer;

        Assert.Equal("Wild", wild.Name, "Wild: correct name");

        Assert.True(wild.Elements.Count() == 2, "Wild: correct number of Elements");
        Assert.Equal("Pets", wild.Elements.First().Name, "Wild: correct first element");
        Assert.Equal(EdmContainerElementKind.EntitySet, wild.Elements.First().ContainerElementKind, "Wild: first element type");
        Assert.Equal("Hot.Pet", ((IEdmEntitySet)wild.Elements.First()).EntityType().FullName(), "Wild: element type for first entityset");

        Assert.Equal("People", wild.Elements.ElementAt(1).Name, "Wild: correct second element");
        Assert.Equal(EdmContainerElementKind.EntitySet, wild.Elements.ElementAt(1).ContainerElementKind, "Zero: second element type");
        Assert.Equal("Hot.Person", ((IEdmEntitySet)wild.Elements.ElementAt(1)).EntityType().FullName(), "Zero: element type for second entityset");

        IEdmEntityType person = (IEdmEntityType)model.FindType("Hot.Person");
        IEdmEntityType pet = (IEdmEntityType)model.FindType("Hot.Pet");
        IEdmEntitySet wildpeople = wild.FindEntitySet("People");
        IEdmEntitySet wildpets = wild.FindEntitySet("Pets");

        Assert.Equal("Hot.Wild", wildpets.Container.FullName(), "Correct container name");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");
        bool validated = model.Validate(out errors);

        Assert.True(validated, "validated");

        IEdmEntityContainer container = model.EntityContainer;
        IEdmEntitySet peopleEntitySet = container.FindEntitySet("People");
        IEdmEntityType petEntityType = (IEdmEntityType)model.FindType("Hot.Pet");
        IEdmNavigationProperty masterNavigationProperty = petEntityType.FindProperty("Master") as IEdmNavigationProperty;
        Assert.NotNull(masterNavigationProperty);
        IEdmNavigationSource navigationSource = peopleEntitySet.FindNavigationTarget(masterNavigationProperty, new EdmPathExpression("Pet/Master"));
        Assert.Equal(navigationSource, peopleEntitySet);
        IEdmEntityType peopleEntityType = (IEdmEntityType)model.FindType("Hot.Person");
        IEdmNavigationProperty petNavigationProperty = peopleEntityType.FindProperty("Pet") as IEdmNavigationProperty;
        IEdmNavigationSource containedNavigationSource = peopleEntitySet.FindNavigationTarget(petNavigationProperty);
        Assert.True(containedNavigationSource is IEdmContainedEntitySet);
        IEdmNavigationSource peopleNavigationSource = containedNavigationSource.FindNavigationTarget(masterNavigationProperty);
        Assert.AreSame(peopleNavigationSource, navigationSource);
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");
        bool validated = model.Validate(out errors);

        Assert.False(validated, "Validation should fail because Property Master1 does not exist on type Hot.Pet");
        Assert.Equal(errors.Count(), 1);
        Assert.Equal(errors.FirstOrDefault().ErrorCode, EdmErrorCode.BadUnresolvedNavigationPropertyPath);
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");
        bool validated = model.Validate(out errors);

        Assert.True(validated, "validated");

        IEdmEntityContainer container = model.EntityContainer;
        IEdmEntitySet peopleEntitySet = container.FindEntitySet("People");
        IEdmEntityType dogEntityType = (IEdmEntityType)model.FindType("Hot.Dog");
        IEdmNavigationProperty masterNavigationProperty = dogEntityType.FindProperty("Master") as IEdmNavigationProperty;
        Assert.NotNull(masterNavigationProperty);
        IEdmNavigationSource navigationSource = peopleEntitySet.FindNavigationTarget(masterNavigationProperty, new EdmPathExpression("Pet/Hot.Dog/Master"));
        Assert.Equal(navigationSource, peopleEntitySet);
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmEntityContainer wild = model.EntityContainer;

        Assert.Equal("AnotherSingletonPeople", wild.Elements.ElementAt(1).Name, "AnotherSingletonPeople: correct second element");
        Assert.Equal(EdmContainerElementKind.Singleton, wild.Elements.ElementAt(1).ContainerElementKind, "Wild: second element type");
        Assert.Equal("Hot.Person", ((IEdmSingleton)wild.Elements.ElementAt(1)).EntityType().FullName(), "Wild: type for first singleton");

        IEdmEntityType person = (IEdmEntityType)model.FindType("Hot.Person");
        IEdmEntityType pet = (IEdmEntityType)model.FindType("Hot.Pet");
        IEdmSingleton singletonPeople = wild.FindSingleton("SingletonPeople");
        Assert.NotNull(singletonPeople, "singletonPeople singleton is not null");
        Assert.Equal(person, singletonPeople.EntityType(), "the type of singletonPeople is Person");
        IEdmSingleton anotherPeople = wild.FindSingleton("AnotherSingletonPeople");
        Assert.NotNull(anotherPeople, "anotherPeople singleton is not null");
        Assert.Equal(person, singletonPeople.Type, "the type of singletonPeople is Person");
        IEdmEntitySet entitySetPeople = wild.FindEntitySet("SingletonPeople");
        Assert.Null(entitySetPeople, "entitySetPeople is null");
        IEdmEntitySet pets = wild.FindEntitySet("Pets");
        Assert.NotNull(pets);

        Assert.Equal("Pets", singletonPeople.FindNavigationTarget(person.NavigationProperties().First()).Name, "PetsAndPeople: end2 correct entity set name");
        Assert.Equal("SingletonPeople", pets.FindNavigationTarget(pet.NavigationProperties().First()).Name);

        Assert.Equal("SingletonPeople", singletonPeople.Name, "SingletonPeople: correct name");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        // Test the structural objects.

        Assert.True(model.SchemaElements.Count() == 2, "Model: contains proper number of Schema Elements");

        IEdmEntityType type1 = (IEdmEntityType)model.SchemaElements.First();

        Assert.Equal("Feckless", type1.Name, "Type1 correct name");
        Assert.Equal(3, type1.DeclaredStructuralProperties().Count(), "Type1: correct count of properties");
        Assert.Equal(2, type1.DeclaredKey.Count(), "Type1: correct number of key properties");
        Assert.Equal("Id", type1.DeclaredKey.First().Name, "Type1: correct first key prop");
        Assert.Equal(1, type1.DeclaredNavigationProperties().Count(), "Type1: correct count of nav props");
        IEdmNavigationProperty nav1 = type1.DeclaredNavigationProperties().First();
        Assert.Equal("MyReckless", nav1.Name, "Nav1: correct name");
        Assert.False(nav1.ContainsTarget, "Nav1: ContainsTarget");

        IEdmEntityType type2 = (IEdmEntityType)model.SchemaElements.ElementAt(1);

        Assert.Equal("Reckless", type2.Name, "Type2 correct name");
        Assert.Equal(2, type2.DeclaredStructuralProperties().Count(), "Type2: correct count of properties");
        Assert.Equal(2, type2.DeclaredKey.Count(), "Type2: correct number of key properties");
        Assert.Equal("Id", type2.DeclaredKey.First().Name, "Type2 correct first key prop");
        Assert.Equal(1, type2.DeclaredNavigationProperties().Count(), "Type2: correct count of nav props");
        IEdmNavigationProperty nav2 = type2.DeclaredNavigationProperties().First();
        Assert.True(nav2.Type.IsNullable);
        Assert.Equal("MyFecklesses", nav2.Name, "Nav2: correct name");
        Assert.True(nav2.ContainsTarget, "Nav2: ContainsTarget");

        // Test the semantic objects.

        IEdmEntityType feckless = (IEdmEntityType)model.FindType("Cold.Feckless");
        IEdmEntityType reckless = (IEdmEntityType)model.FindType("Cold.Reckless");

        IEdmNavigationProperty toReckless = (IEdmNavigationProperty)feckless.FindProperty("MyReckless");
        IEdmNavigationProperty toFecklesses = (IEdmNavigationProperty)reckless.FindProperty("MyFecklesses");

        Assert.Equal(toFecklesses, toReckless.Partner, "Correct partner for toReckless");
        Assert.Equal(feckless, toReckless.DeclaringEntityType(), "ToReckless correct from type");
        Assert.Equal(reckless, toReckless.ToEntityType(), "ToReckless correct to type");

        Assert.Equal(toReckless, toFecklesses.Partner, "Correct partner for toFecklesses");
        Assert.Equal(reckless, toFecklesses.DeclaringEntityType(), "toFecklesses correct from type");
        Assert.Equal(feckless, toFecklesses.ToEntityType(), "toFecklesses correct to type");

        Assert.Equal(true, toFecklesses.IsPrincipal(), "Principal end match");
        Assert.Null(toFecklesses.DependentProperties(), "Principal end match");
        Assert.Equal(false, toReckless.IsPrincipal(), "Dependent end match");
        Assert.NotNull(toReckless.DependentProperties(), "Dependent end match");

        Assert.Equal(2, toReckless.DependentProperties().Count(), "DependentProperty count correct");
        Assert.Equal(toReckless.DependentProperties().First(), feckless.DeclaredKey.First(), "Dependent property1 match");
        Assert.Equal(toReckless.DependentProperties().Last(), feckless.DeclaredKey.Last(), "Dependent property2 match");

        Assert.Equal(EdmOnDeleteAction.Cascade, toReckless.OnDelete, "Correct end1 action");
        Assert.Equal(EdmOnDeleteAction.None, toFecklesses.OnDelete, "Correct end2 action");

        Assert.Equal(EdmMultiplicity.Many, toFecklesses.TargetMultiplicity(), "Correct end1 multiplicity");
        Assert.Equal(EdmMultiplicity.ZeroOrOne, toReckless.TargetMultiplicity(), "Correct end2 multiplicity");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        // Test the structural objects.
        Assert.True(model.SchemaElements.Count() == 2, "correct schemata element count");
        IEdmComplexType type1 = (IEdmComplexType)model.SchemaElements.First();
        IEdmComplexType type2 = (IEdmComplexType)model.SchemaElements.Last();
        Assert.Equal("Smod", type1.Name, "Smod not found");
        Assert.Equal("Id", type1.DeclaredStructuralProperties().First().Name, "Type1 correct name");
        IEdmPrimitiveTypeReference idType = type1.DeclaredStructuralProperties().First().Type.AsPrimitive();
        Assert.Equal("Int32", idType.PrimitiveDefinition().Name, "ID is correct type");
        Assert.False(idType.IsNullable, "ID is nullable");


        Assert.Equal("Clod", type2.Name, "Clod not found");
        Assert.Equal(2, type2.DeclaredStructuralProperties().Count(), "Correct number of properties");
        Assert.Equal("Address", type2.DeclaredStructuralProperties().Last().Name, "Correct last property");
        IEdmStringTypeReference addressType = type2.DeclaredStructuralProperties().Last().Type.AsPrimitive().AsString();
        Assert.Equal("String", addressType.PrimitiveDefinition().Name, "Correct Address type");
        Assert.Equal(2048, addressType.MaxLength, "Correct address max length");
        Assert.True(addressType.IsNullable, "Correct address nulliblity");

        // Test the semantic objects.

        IEdmComplexType smod = (IEdmComplexType)model.FindType("Grumble.Smod");
        IEdmComplexType clod = (IEdmComplexType)model.FindType("Mumble.Clod");

        Assert.Equal("Smod", smod.Name, "Smod Name correct");
        Assert.Equal("Grumble", smod.Namespace, "Smod Namespace correct");
        Assert.Equal("Clod", clod.Name, "Clod Name correct");
        Assert.Equal("Mumble", clod.Namespace, "Clod Namespace correct");

        Assert.Equal(clod.BaseType, smod, "Clod base type correct");

        Assert.Equal(3, clod.StructuralProperties().Count(), "clod inheritied properties properly");
        Assert.Equal(clod.StructuralProperties().First(), smod.StructuralProperties().First(), "share same property");
        IEdmProperty address = clod.FindProperty("Address");
        Assert.Equal("Address", address.Name, "Addres has proper name");
        Assert.Equal(clod.StructuralProperties().Last(), address, "Clod last property is correct");

        IEdmProperty id = clod.FindProperty("Id");
        IEdmPrimitiveTypeReference resolvedIdType = id.Type.AsPrimitive();
        Assert.True(resolvedIdType.PrimitiveKind() == EdmPrimitiveTypeKind.Int32, "ID is proper kind");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        // Test the structural objects.
        Assert.True(model.SchemaElements.Count() == 2, "correct schemata element count");
        IEdmComplexType type1 = (IEdmComplexType)model.SchemaElements.First();
        IEdmComplexType type2 = (IEdmComplexType)model.SchemaElements.Last();
        Assert.Equal("Smod", type1.Name, "Smod not found");
        Assert.Equal("Id", type1.DeclaredStructuralProperties().First().Name, "Type1 correct name");
        IEdmPrimitiveTypeReference idType = type1.DeclaredStructuralProperties().First().Type.AsPrimitive();
        Assert.Equal("Int32", idType.PrimitiveDefinition().Name, "ID is correct type");
        Assert.False(idType.IsNullable, "ID is nullable");


        Assert.Equal("Clod", type2.Name, "Clod not found");
        Assert.Equal(2, type2.DeclaredStructuralProperties().Count(), "Correct number of properties");
        Assert.Equal("Address", type2.DeclaredStructuralProperties().Last().Name, "Correct last property");
        IEdmStringTypeReference addressType = type2.DeclaredStructuralProperties().Last().Type.AsPrimitive().AsString();
        Assert.Equal("String", addressType.PrimitiveDefinition().Name, "Correct Address type");
        Assert.Equal(2048, addressType.MaxLength, "Correct address max length");
        Assert.True(addressType.IsNullable, "Correct address nulliblity");

        // Test the semantic objects.

        IEdmComplexType smod = (IEdmComplexType)model.FindType("Grumble.Smod");
        IEdmComplexType clod = (IEdmComplexType)model.FindType("Mumble.Clod");

        Assert.Equal("Smod", smod.Name, "Smod Name correct");
        Assert.Equal("Grumble", smod.Namespace, "Smod Namespace correct");
        Assert.Equal("Clod", clod.Name, "Clod Name correct");
        Assert.Equal("Mumble", clod.Namespace, "Clod Namespace correct");

        Assert.Equal(clod.BaseType, smod, "Clod base type correct");

        Assert.Equal(3, clod.StructuralProperties().Count(), "clod inheritied properties properly");
        Assert.Equal(clod.StructuralProperties().First(), smod.StructuralProperties().First(), "share same property");
        IEdmProperty address = clod.FindProperty("Address");
        Assert.Equal("Address", address.Name, "Addres has proper name");
        Assert.Equal(clod.StructuralProperties().Last(), address, "Clod last property is correct");

        IEdmProperty id = clod.FindProperty("Id");
        IEdmPrimitiveTypeReference resolvedIdType = id.Type.AsPrimitive();
        Assert.True(resolvedIdType.PrimitiveKind() == EdmPrimitiveTypeKind.Int32, "ID is proper kind");
    }

    [Fact]
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
        //Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        // Test the structural objects.
        Assert.Equal(7, model.SchemaElements.Count(), "correct schemata element count");
        IEdmComplexType type1 = (IEdmComplexType)model.SchemaElements.First();
        IEdmComplexType type2 = (IEdmComplexType)model.SchemaElements.ElementAt(1);
        Assert.Equal("MyComplex1", type1.Name, "MyComplex1 not found");
        Assert.Equal("Id", type2.DeclaredStructuralProperties().First().Name, "Type2 correct name");

        IEdmPrimitiveTypeReference idType = type2.DeclaredStructuralProperties().First().Type.AsPrimitive();
        Assert.Equal("Int32", idType.PrimitiveDefinition().Name, "ID is correct type");
        Assert.False(idType.IsNullable, "ID is nullable");

        // Test the semantic objects.
        IEdmComplexType type2_1 = (IEdmComplexType)model.FindType("NameSpace2.MyComplex1");
        IEdmComplexType type2_2 = (IEdmComplexType)model.FindType("NameSpace2.MyComplex2");
        IEdmComplexType type2_3 = (IEdmComplexType)model.FindType("NameSpace2.MyComplex3");

        Assert.Equal("MyComplex1", type2_1.Name, "MyComplex1 Name correct");
        Assert.Equal("MyComplex2", type2_2.Name, "MyComplex2 Name correct");
        Assert.Equal("MyComplex3", type2_3.Name, "MyComplex3 Name correct");

        Assert.Equal(type2_2.BaseType, type2_1, "MyComplex2 base type correct");
        Assert.Equal(type2_3.BaseType, type2_2, "MyComplex3 base type correct");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");
        // Test the structural objects.

        IEdmEntityType type1 = (IEdmEntityType)model.SchemaElements.First();
        IEdmEntityType type2 = (IEdmEntityType)model.SchemaElements.Last();
        Assert.Equal(type1.Name, "Smod", "Smod not found");
        Assert.Equal(1, type1.DeclaredKey.Count(), "Type1: correct key property count");
        Assert.Equal("Id", type1.DeclaredKey.First().Name, "Type1: correct key property name");
        Assert.Equal("Id", type1.DeclaredStructuralProperties().First().Name, "Correct first property");
        IEdmPrimitiveTypeReference idType = type1.DeclaredStructuralProperties().First().Type.AsPrimitive();
        Assert.Equal(EdmPrimitiveTypeKind.Int32, idType.PrimitiveKind(), "First property correct type");
        Assert.False(idType.IsNullable, "First prop is correctly not nullable");
        Assert.Equal(type2.Name, "Clod", "Clod not found");
        Assert.Equal("Grumble.Smod", type2.BaseEntityType().FullName(), "Correct base type name");
        Assert.Equal(type2.DeclaredStructuralProperties().Count(), 2, "Correct number of properties on inheriting type");
        Assert.Equal("Address", type2.StructuralProperties().Last().Name, "Correct name on last structural prop");
        IEdmStringTypeReference addressType = type2.StructuralProperties().Last().Type.AsPrimitive().AsString();
        Assert.Equal(addressType.PrimitiveKind(), EdmPrimitiveTypeKind.String, "Last property correct type");
        Assert.Equal(2048, addressType.MaxLength, "Address correct length");
        Assert.True(addressType.IsNullable, "Address is nullable");

        // Test the semantic objects.

        IEdmEntityType smod = (IEdmEntityType)model.FindType("Grumble.Smod");
        IEdmEntityType smod2 = (IEdmEntityType)model.FindType("Grumble.Smod2");
        IEdmEntityType smod3 = (IEdmEntityType)model.FindType("Grumble.Smod3");
        IEdmEntityType clod = (IEdmEntityType)model.FindType("Mumble.Clod");

        Assert.Equal("Smod", smod.Name, "Correct Smod Name");
        Assert.Equal("Clod", clod.Name, "Correct Clod Name");

        Assert.False(smod.IsOpen, "smod.IsOpen");
        Assert.True(smod2.IsOpen, "smod2.IsOpen");
        Assert.False(smod3.IsOpen, "smod3.IsOpen");

        Assert.Equal(clod.BaseType, smod, "Correct Inheritance");

        Assert.Equal(3, clod.StructuralProperties().Count(), "Correct number of total props");
        Assert.Equal(clod.StructuralProperties().First(), smod.StructuralProperties().First(), "ID properly inherited");
        IEdmProperty address = clod.FindProperty("Address");
        Assert.Equal("Address", address.Name, "Address correct name");
        Assert.Equal(clod.StructuralProperties().Last(), address, "Clod correct last property");

        IEdmProperty id = clod.FindProperty("Id");
        IEdmPrimitiveTypeReference resolvedIdType = id.Type.AsPrimitive();
        Assert.Equal(resolvedIdType.PrimitiveKind(), EdmPrimitiveTypeKind.Int32, "ID correct kind");

        Assert.Equal(1, smod.DeclaredKey.Count(), "Smod correct number of key props");
        Assert.Equal(id, smod.DeclaredKey.First(), "Smod correct key prop");
        Assert.Equal(smod.DeclaredKey.First(), clod.Key().First(), "Inhertance shared key");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmComplexType smodDef = (IEdmComplexType)model.SchemaElements.First();
        IEdmStructuralProperty streamProp = (IEdmStructuralProperty)smodDef.FindProperty("Stream");
        Assert.Equal(EdmTypeKind.Primitive, streamProp.Type.TypeKind(), "Stream should have type of primitive");
        Assert.Equal(EdmPrimitiveTypeKind.Stream, streamProp.Type.AsPrimitive().PrimitiveKind(), "Stream should have primitive kind of stream");
        IEdmStructuralProperty streamProp1 = (IEdmStructuralProperty)smodDef.FindProperty("Stream1");
        IEdmStructuralProperty streamProp2 = (IEdmStructuralProperty)smodDef.FindProperty("Stream2");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        // Test the structural objects.
        Assert.True(model.SchemaElements.Count() == 1, "correct schemata element count");
        IEdmComplexType type = (IEdmComplexType)model.SchemaElements.First();
        Assert.Equal("Smod", type.Name, "Smod not found");
        Assert.Equal("Id", type.DeclaredStructuralProperties().First().Name, "Type correct name");
        IEdmPrimitiveTypeReference idType = type.DeclaredStructuralProperties().First().Type.AsPrimitive();
        Assert.Equal("Int32", idType.PrimitiveDefinition().Name, "ID is correct type");
        Assert.False(idType.IsNullable, "ID is nullable");

        // Test the Annotation
        IEdmValue annotationValue = model.GetAnnotationValue<IEdmValue>(type.DeclaredStructuralProperties().First(), "p", "foo");
        IEdmDirectValueAnnotation annotation = (IEdmDirectValueAnnotation)model.DirectValueAnnotations(type.DeclaredStructuralProperties().First()).First();
        Assert.Equal(annotation.Value, annotationValue, "Correct annotation found");
        Assert.Equal("p", annotation.NamespaceUri, "Correct annotation namespace");
        Assert.Equal("foo", annotation.Name, "Correct annotation local name");
        Assert.Equal(EdmPrimitiveTypeKind.String, annotationValue.Type.AsPrimitive().PrimitiveKind(), "Correct annotation type");
        Assert.Equal("Quoth the Raven", ((IEdmStringValue)annotationValue).Value, "Correct annotation value");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        // Test the structural objects.
        Assert.True(model.SchemaElements.Count() == 1, "correct schemata element count");
        IEdmComplexType type = (IEdmComplexType)model.SchemaElements.First();
        Assert.Equal("Smod", type.Name, "Smod not found");
        Assert.Equal("Id", type.DeclaredStructuralProperties().First().Name, "Type correct name");
        IEdmPrimitiveTypeReference idType = type.DeclaredStructuralProperties().First().Type.AsPrimitive();
        Assert.Equal("Int32", idType.PrimitiveDefinition().Name, "ID is correct type");
        Assert.False(idType.IsNullable, "ID is nullable");

        // Test the Annotation
        IEdmValue annotationValue = model.GetAnnotationValue<IEdmValue>(type.DeclaredStructuralProperties().First(), "p", "foo");
        IEdmDirectValueAnnotation annotation = (IEdmDirectValueAnnotation)model.DirectValueAnnotations(type.DeclaredStructuralProperties().First()).First();
        Assert.Equal(annotation.Value, annotationValue, "Correct annotation found");
        Assert.Equal("p", annotation.NamespaceUri, "Correct annotation namespace");
        Assert.Equal("foo", annotation.Name, "Correct annotation local name");
        Assert.Equal(EdmPrimitiveTypeKind.String, annotationValue.Type.AsPrimitive().PrimitiveKind(), "Correct annotation type");
        //TODO: handle complex annotations properly
        Assert.Equal("<p:foo xmlns:p=\"p\">\n           Quoth the Raven\n        </p:foo>", ((IEdmStringValue)annotationValue).Value, "Correct annotation value");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        // Test the structural objects.
        Assert.True(model.SchemaElements.Count() == 1, "correct schemata element count");
        IEdmComplexType type = (IEdmComplexType)model.SchemaElements.First();
        Assert.Equal("Smod", type.Name, "Smod not found");
        Assert.Equal("Id", type.DeclaredStructuralProperties().First().Name, "Type correct name");
        IEdmPrimitiveTypeReference idType = type.DeclaredStructuralProperties().First().Type.AsPrimitive();
        Assert.Equal("Int32", idType.PrimitiveDefinition().Name, "ID is correct type");
        Assert.False(idType.IsNullable, "ID is nullable");

        // Test the Annotation
        IEdmValue annotationValue = model.GetAnnotationValue<IEdmValue>(type.DeclaredStructuralProperties().First(), "p", "foo");
        IEdmDirectValueAnnotation annotation = (IEdmDirectValueAnnotation)model.DirectValueAnnotations(type.DeclaredStructuralProperties().First()).First();
        Assert.Equal(annotation.Value, annotationValue, "Correct annotation found");
        Assert.Equal("p", annotation.NamespaceUri, "Correct annotation namespace");
        Assert.Equal("foo", annotation.Name, "Correct annotation local name");
        Assert.Equal(EdmPrimitiveTypeKind.String, annotationValue.Type.AsPrimitive().PrimitiveKind(), "Correct annotation type");
        //TODO: handle complex annotations properly
        Assert.Equal(@"<p:foo xmlns:p=""p"" />", ((IEdmStringValue)annotationValue).Value, "Correct annotation value");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmComplexType type = (IEdmComplexType)model.SchemaElements.First();
        IEdmStructuralProperty id = type.DeclaredStructuralProperties().First();

        Assert.True(model.DirectValueAnnotations(id).Count() == 3, "Annotation count");
        IEdmValue annotationValue = model.GetAnnotationValue<IEdmValue>(id, "p", "foo");
        Assert.Equal("Quoth the Raven", ((IEdmStringValue)annotationValue).Value, "Annotation value");
        annotationValue = model.GetAnnotationValue<IEdmValue>(id, "p", "bar");
        Assert.Equal("Nevermore", ((IEdmStringValue)annotationValue).Value, "Annotation value");
        annotationValue = model.GetAnnotationValue<IEdmValue>(id, "p", "baz");
        Assert.Equal("Right Phil?", ((IEdmStringValue)annotationValue).Value, "Annotation value");

        model.SetAnnotationValue(id, "p", "foo", null);
        model.SetAnnotationValue(id, "p", "baz", "Sure Bobby");

        Assert.True(model.DirectValueAnnotations(id).Count() == 2, "Annotation count");
        annotationValue = model.GetAnnotationValue<IEdmValue>(id, "p", "foo");
        Assert.Null(annotationValue, "Annotation value");
        annotationValue = model.GetAnnotationValue<IEdmValue>(id, "p", "bar");
        Assert.Equal("Nevermore", ((IEdmStringValue)annotationValue).Value, "Annotation value");
        Assert.Equal(model.GetAnnotationValue<string>(id, "p", "baz"), "Sure Bobby", "Annotation value");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmComplexType smodDef = (IEdmComplexType)model.SchemaElements.First();
        IEdmStructuralProperty mvProp = (IEdmStructuralProperty)smodDef.FindProperty("Collection");
        Assert.Equal(EdmTypeKind.Collection, mvProp.Type.TypeKind(), "Collection should have type of collection");
        Assert.Equal(EdmTypeKind.Primitive, mvProp.Type.AsCollection().ElementType().TypeKind(), "Collection should be of primitives");
        Assert.Equal(EdmPrimitiveTypeKind.Int32, mvProp.Type.AsCollection().ElementType().AsPrimitive().PrimitiveKind(), "Collection should be of Int32s");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmComplexType smodDef = (IEdmComplexType)model.SchemaElements.First();
        IEdmStructuralProperty idProp = (IEdmStructuralProperty)smodDef.FindProperty("Id");
        Assert.True(idProp.Type.IsNullable, "Nullable should default to true");

    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmEntityType inheritingEntityType = (IEdmEntityType)model.SchemaElements.Last();
        IEdmStructuralProperty baseProperty = (IEdmStructuralProperty)inheritingEntityType.FindProperty("Property0");
        Assert.Equal(baseProperty, inheritingEntityType.StructuralProperties().First(), "FindProperty should also return properties of base type");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmComplexType typeDefinition = (IEdmComplexType)model.SchemaElements.First();
        IEdmStructuralProperty stringProp = (IEdmStructuralProperty)typeDefinition.FindProperty("myString");
        Assert.Equal(stringProp.Type.AsString().IsUnicode, true, "Unicode should default to true");
    }

    #region Operations and OperationImports

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmEntityType personType = (IEdmEntityType)model.SchemaElements.First();
        IEdmOperation getAge = (IEdmOperation)model.FindOperations("foo.GetAge").First();
        Assert.Equal("GetAge", getAge.Name, "Group name correct");
        Assert.NotNull(getAge, "Function exists and has IEdmFunctionType");
        Assert.Equal("GetAge", getAge.Name, "Name correct");
        Assert.Equal("foo", getAge.Namespace, "Namespace correct");
        Assert.Equal(EdmPrimitiveTypeKind.Int32, getAge.ReturnType.AsPrimitive().PrimitiveKind(), "Return type correct");
        Assert.Equal(EdmSchemaElementKind.Function, getAge.SchemaElementKind, "Schema element kind correct");
        IEdmOperationParameter getAgeParameter = getAge.Parameters.First();
        Assert.Equal(getAgeParameter, getAge.FindParameter("Person"), "Find parameter returns proper parameter");
        Assert.Equal("Person", getAgeParameter.Name, "Parameter has correct name");
        Assert.Equal(personType, getAgeParameter.Type.Definition, "Parameter has correct mode");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        // validation should detect duplicated parameter names.
        IEnumerable<EdmError> validationErrors;
        model.Validate(out validationErrors);
        Assert.Equal(validationErrors.Single().ErrorMessage, "Each parameter name in a operation must be unique. The parameter name 'PersonParam' is already defined.");

        // FindParameter() should throw exception.
        IEdmEntityType personType = (IEdmEntityType)model.SchemaElements.First();
        IEdmOperation getAge = (IEdmOperation)model.FindOperations("foo.GetAge").First();
        try
        {
            var param1 = getAge.FindParameter("PersonParam");
        }
        catch (InvalidOperationException ex)
        {
            Assert.Equal("Sequence contains more than one matching element", ex.Message);
            return;
        }

        Assert.Fail();
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmEntityType personType = (IEdmEntityType)model.SchemaElements.First();
        IEdmEntityContainer fooContainer = (IEdmEntityContainer)model.FindEntityContainer("fooContainer");
        IEdmEntitySet peopleSet = (IEdmEntitySet)fooContainer.Elements.First();

        var fiGroup = fooContainer.FindOperationImports("peopleWhoAreAwesomeAction").ToArray();
        Assert.Equal(4, fiGroup.Length, "# of overloads expected");

        IEdmOperationImport peopleWhoAreAwesome = fiGroup.First();
        Assert.Equal(EdmContainerElementKind.ActionImport, peopleWhoAreAwesome.ContainerElementKind, "FunctionImport has correct ContainerElementKind");
        IEdmEntitySetBase eset;
        Assert.True(peopleWhoAreAwesome.TryGetStaticEntitySet(model, out eset), "peopleWhoAreAwesome.TryGetStaticEntitySet");
        Assert.Equal(peopleSet, eset, "Return EntitySet name is correct");
        IEdmEntitySetBase fiEntitySet;
        Assert.True(peopleWhoAreAwesome.TryGetStaticEntitySet(model, out fiEntitySet), "peopleWhoAreAwesome.TryGetStaticEntitySet");
        Assert.Equal(personType, fiEntitySet.EntityType(), "Return EntitySet type is correct");
        Assert.Equal("peopleWhoAreAwesomeAction", peopleWhoAreAwesome.Name, "FunctionImport name is correct");
        Assert.Equal(EdmTypeKind.Collection, peopleWhoAreAwesome.Operation.ReturnType.Definition.TypeKind, "Return typekind is correct");
        Assert.Equal(personType, peopleWhoAreAwesome.Operation.ReturnType.AsCollection().ElementType().Definition, "Return entitytype is correct");
        IEdmOperationParameter peopleWhoAreAwesomeParameter = peopleWhoAreAwesome.Operation.Parameters.First();
        Assert.Equal(peopleWhoAreAwesomeParameter, peopleWhoAreAwesome.Operation.FindParameter("awesomeName"), "Find parameter returns proper parameter");
        Assert.Equal("awesomeName", peopleWhoAreAwesomeParameter.Name, "FunctionImport parameter name is correct");
        Assert.False(peopleWhoAreAwesomeParameter.Type.AsString().IsBad(), "FunctionImport has correct ContainerElementKind");

        Assert.True(fiGroup[0] is IEdmActionImport, "Expected to be a action import");
        Assert.True(fiGroup[1] is IEdmActionImport, "Expected to be a action import");
        Assert.True(fiGroup[2] is IEdmActionImport, "Expected to be a action import");
        Assert.True(fiGroup[3] is IEdmActionImport, "Expected to be a action import");

        fiGroup = fooContainer.FindOperationImports("peopleWhoAreAwesomeFunction").ToArray();
        Assert.Equal(3, fiGroup.Length, "# of overloads expected");
        Assert.True(fiGroup[0] is IEdmFunctionImport, "Expected to be a function import");
        Assert.True(fiGroup[1] is IEdmFunctionImport, "Expected to be a function import");
        Assert.True(fiGroup[2] is IEdmFunctionImport, "Expected to be a function import");
    }

    // BuildInternalUniqueParameterTypeFunctionString is not including the type name for collection of entities
    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No parsing errors");

        var validated = EdmValidator.Validate(model, EdmConstants.EdmVersionLatest, out errors);
        Assert.True(validated, "validate");

        var people = model.EntityContainer.FindEntitySet("People");
        Assert.Equal("Person", people.EntityType().Name, "people.ElementType.Name");
        Assert.False(((IEdmNavigationProperty)people.EntityType().FindProperty("Orders")).ContainsTarget, "Orders.ContainsTarget");

        var operationImports = model.EntityContainer.OperationImports().ToArray();
        Assert.Equal(11, operationImports.Length, "# of operation imports");

        Assert.Equal(EdmTypeKind.Entity, operationImports[0].Operation.ReturnType.AsCollection().ElementType().TypeKind(), "operationImports[0] return type");
        Assert.Equal(EdmTypeKind.Entity, operationImports[0].Operation.Parameters.Single().Type.AsCollection().ElementType().TypeKind(), "operationImports[0] parameter type");
        IEdmEntitySetBase eset;
        IEdmOperationParameter p;
        Dictionary<IEdmNavigationProperty, IEdmPathExpression> np;
        IEnumerable<EdmError> entitySetPathErrors;
        Assert.True(operationImports[0].TryGetStaticEntitySet(model, out eset), "operationImports[0].TryGetStaticEntitySet");
        Assert.False(operationImports[0].TryGetRelativeEntitySetPath(model, out p, out np, out entitySetPathErrors), "operationImports[0].TryGetRelativeEntitySetPath");
        Assert.Equal(people, eset, "people, eset");

        Assert.Equal(EdmTypeKind.Complex, operationImports[1].Operation.ReturnType.AsCollection().ElementType().TypeKind(), "operationImports[1] return type");
        Assert.Equal(EdmTypeKind.Complex, operationImports[1].Operation.Parameters.Single().Type.AsCollection().ElementType().TypeKind(), "operationImports[1] parameter type");

        Assert.Equal(EdmTypeKind.Primitive, operationImports[2].Operation.ReturnType.AsCollection().ElementType().TypeKind(), "operationImports[2] return type");
        Assert.Equal(EdmTypeKind.Primitive, operationImports[2].Operation.Parameters.Single().Type.AsCollection().ElementType().TypeKind(), "operationImports[2] parameter type");

        Assert.Equal(EdmTypeKind.Entity, operationImports[3].Operation.ReturnType.TypeKind(), "operationImports[3] return type");
        Assert.Equal(EdmTypeKind.Entity, operationImports[3].Operation.Parameters.Single().Type.TypeKind(), "operationImports[3] parameter type");
        IEdmEntitySetBase eset2;
        Assert.True(operationImports[3].TryGetStaticEntitySet(model, out eset2), "operationImports[3].TryGetStaticEntitySet");
        Assert.False(operationImports[0].TryGetRelativeEntitySetPath(model, out p, out np, out entitySetPathErrors), "operationImports[0].TryGetRelativeEntitySetPath");
        Assert.Equal(people, eset2, "people, eset2");

        Assert.Equal(EdmTypeKind.Complex, operationImports[4].Operation.ReturnType.TypeKind(), "operationImports[4] return type");
        Assert.Equal(EdmTypeKind.Complex, operationImports[4].Operation.Parameters.Single().Type.TypeKind(), "operationImports[4] parameter type");

        Assert.Equal(EdmTypeKind.Primitive, operationImports[5].Operation.ReturnType.TypeKind(), "operationImports[5] return type");
        Assert.Equal(EdmTypeKind.Primitive, operationImports[5].Operation.Parameters.Single().Type.TypeKind(), "operationImports[5] parameter type");

        Assert.False(operationImports[6].TryGetStaticEntitySet(model, out eset), "operationImports[6].TryGetStaticEntitySet");
        Assert.False(operationImports[6].TryGetRelativeEntitySetPath(model, out p, out np, out entitySetPathErrors), "operationImports[6].TryGetStaticEntitySet");
        Assert.Null(p, "p is null");

        Assert.False(operationImports[7].TryGetStaticEntitySet(model, out eset), "operationImports[7].TryGetStaticEntitySet");
        Assert.False(operationImports[7].TryGetRelativeEntitySetPath(model, out p, out np, out entitySetPathErrors), "operationImports[7].TryGetStaticEntitySet");

        Assert.False(operationImports[8].TryGetStaticEntitySet(model, out eset), "operationImports[8].TryGetStaticEntitySet");
        Assert.False(operationImports[8].TryGetRelativeEntitySetPath(model, out p, out np, out entitySetPathErrors), "operationImports[8].TryGetStaticEntitySet");

        Assert.False(operationImports[9].TryGetStaticEntitySet(model, out eset), "operationImports[9].TryGetStaticEntitySet");
        Assert.False(operationImports[9].TryGetRelativeEntitySetPath(model, out p, out np, out entitySetPathErrors), "operationImports[9].TryGetStaticEntitySet");

        Assert.False(operationImports[10].TryGetStaticEntitySet(model, out eset), "operationImports[10].TryGetStaticEntitySet");
        Assert.False(operationImports[10].TryGetRelativeEntitySetPath(model, out p, out np, out entitySetPathErrors), "operationImports[10].TryGetStaticEntitySet");
    }
    #endregion

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmEntityType entity1Def = (IEdmEntityType)model.FindType("Grumble.Entity1");
        IEdmEntityType entity2Def = (IEdmEntityType)model.FindType("Grumble.Entity2");

        Assert.True(((IEdmNavigationProperty)entity1Def.FindProperty("ToEntity2")).IsPrincipal(), "Correct principal end");
        Assert.Equal(entity2Def.FindProperty("Fk1"), ((IEdmNavigationProperty)entity1Def.FindProperty("ToEntity2")).Partner.DependentProperties().First(), "Referential constraint in correct order");

        Assert.Equal("Entity1", entity1Def.Name, "Principal end is correct");
        Assert.Equal("Entity2", entity2Def.Name, "Dependent end is correct");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmComplexType typeDefinition = (IEdmComplexType)model.SchemaElements.First();
        IEdmStructuralProperty stringProp = (IEdmStructuralProperty)typeDefinition.FindProperty("myString");
        Assert.Null(stringProp.Type.AsString().MaxLength, "Maxlength should default to null");
        IEdmStructuralProperty decimalProp = (IEdmStructuralProperty)typeDefinition.FindProperty("myDecimal");
        Assert.Null(decimalProp.Type.AsDecimal().Scale, "Scale should default to null");
        Assert.Null(decimalProp.Type.AsDecimal().Precision, "Precision should default to null");
        IEdmStructuralProperty datetimeProp = (IEdmStructuralProperty)typeDefinition.FindProperty("myDateTime");
        Assert.Equal(0, datetimeProp.Type.AsTemporal().Precision, "Temporal precision should default to 0");
    }

    [Fact]
    public void VerifyFacetManuallySet()
    {
        const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""myDecimal"" Type=""Decimal"" Precision=""3""/>
    <Property Name=""mySecondDecimal"" Type=""Decimal"" Precision=""3"" Scale=""Variable""/>
    <Property Name=""myThirdDecimal"" Type=""Decimal"" Precision=""3"" Scale=""0""/>
    <Property Name=""myDateTime"" Type=""DateTimeOffset"" Precision=""1""/>
    <Property Name=""myString"" Type=""String"" Unicode=""false""/>
  </ComplexType>
</Schema>";

        IEdmModel model;
        IEnumerable<EdmError> errors;
        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmComplexType typeDefinition = (IEdmComplexType)model.SchemaElements.First();
        IEdmStructuralProperty stringProp = (IEdmStructuralProperty)typeDefinition.FindProperty("myString");
        Assert.Null(stringProp.Type.AsString().MaxLength, "Maxlength should default to null");
        Assert.Equal(false, stringProp.Type.AsString().IsUnicode, "String unicode should be set to false");
        IEdmStructuralProperty decimalProp = (IEdmStructuralProperty)typeDefinition.FindProperty("myDecimal");
        Assert.Null(decimalProp.Type.AsDecimal().Scale, "Scale should default to null");
        Assert.Equal(3, decimalProp.Type.AsDecimal().Precision, "Decimal precision should be set to 3");
        IEdmStructuralProperty secondDecimalProp = (IEdmStructuralProperty)typeDefinition.FindProperty("mySecondDecimal");
        Assert.Null(secondDecimalProp.Type.AsDecimal().Scale, "Decimal scale should be null when its value is Variable");
        Assert.Equal(3, secondDecimalProp.Type.AsDecimal().Precision, "Decimal precision should be set to 3");
        IEdmStructuralProperty thirdDecimalProp = (IEdmStructuralProperty)typeDefinition.FindProperty("myThirdDecimal");
        Assert.Equal(thirdDecimalProp.Type.AsDecimal().Scale, 0, "Decimal scale should be set to 0");
        Assert.Equal(3, thirdDecimalProp.Type.AsDecimal().Precision, "Decimal precision should be set to 3");
        IEdmStructuralProperty datetimeProp = (IEdmStructuralProperty)typeDefinition.FindProperty("myDateTime");
        Assert.Equal(1, datetimeProp.Type.AsTemporal().Precision, "Temporal precision should be set to 1");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");

        IEdmEntityType entityTypeDefinition = (IEdmEntityType)model.SchemaElements.First();
        IEdmComplexType complexTypeDefinition = (IEdmComplexType)model.SchemaElements.Last();
        IEdmStructuralProperty refProp = (IEdmStructuralProperty)complexTypeDefinition.FindProperty("myRef");

        Assert.Equal(EdmTypeKind.EntityReference, refProp.Type.TypeKind(), "Property has correct type kind");
        Assert.Equal(entityTypeDefinition, refProp.Type.AsEntityReference().EntityReferenceDefinition().EntityType, "EntityTypeReference points to correct entity type");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmEntityType entityTypeDefinitionSmod = (IEdmEntityType)model.SchemaElements.First();
        IEdmEntityType entityTypeDefinitionClod = (IEdmEntityType)model.SchemaElements.Last();
        Assert.True(entityTypeDefinitionClod.BaseType.IsBad(), "Clod basetype is bad because of cycle");
        Assert.Equal(EdmErrorCode.BadCyclicEntity, entityTypeDefinitionClod.BaseType.Errors().First().ErrorCode, "Cyclic entity is cyclic?");
        Assert.True(entityTypeDefinitionSmod.BaseType.IsBad(), "Smod basetype is bad because of cycle");

        EdmValidator.Validate(model, EdmConstants.EdmVersion4, out errors);
        Assert.Equal(2, errors.Count(), "Correct number of errors");
        Assert.Equal(EdmErrorCode.BadCyclicEntity, errors.ElementAt(0).ErrorCode, "Correct error code 1");
        Assert.Equal(EdmErrorCode.BadCyclicEntity, errors.ElementAt(1).ErrorCode, "Correct error code 2");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmEntityType entityTypeDefinitionSmod = (IEdmEntityType)model.SchemaElements.First();
        IEdmEntityType entityTypeDefinitionClod = (IEdmEntityType)model.SchemaElements.ElementAt(1);
        IEdmEntityType entityTypeDefinitionBlob = (IEdmEntityType)model.SchemaElements.Last();
        Assert.True(entityTypeDefinitionClod.BaseType.IsBad(), "Clod basetype is bad because of cycle");
        Assert.Equal(EdmErrorCode.BadCyclicEntity, entityTypeDefinitionClod.BaseType.Errors().First().ErrorCode, "Cyclic entity is cyclic?");
        Assert.True(entityTypeDefinitionSmod.BaseType.IsBad(), "Smod basetype is bad because of cycle");
        Assert.True(entityTypeDefinitionBlob.BaseType.IsBad(), "Blob basetype is bad because of cycle");

        EdmValidator.Validate(model, EdmConstants.EdmVersion4, out errors);
        Assert.Equal(3, errors.Count(), "Correct number of errors");
        Assert.Equal(EdmErrorCode.BadCyclicEntity, errors.First().ErrorCode, "Correct error code");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmEntityType entityTypeDefinitionSmod = (IEdmEntityType)model.SchemaElements.First();
        IEdmEntityType entityTypeDefinitionClod = (IEdmEntityType)model.SchemaElements.ElementAt(1);
        IEdmEntityType entityTypeDefinitionBlob = (IEdmEntityType)model.SchemaElements.Last();
        Assert.False(entityTypeDefinitionClod.BaseType.IsBad(), "Contagious badness not");
        Assert.True(entityTypeDefinitionBlob.BaseType.IsBad(), "Blob basetype is bad because Blob is in a cycle.");
        Assert.Equal(EdmErrorCode.BadCyclicEntity, entityTypeDefinitionBlob.BaseType.Errors().First().ErrorCode, "Cyclic entity is cyclic?");

        EdmValidator.Validate(model, EdmConstants.EdmVersion4, out errors);
        Assert.Equal(1, errors.Count(), "Correct number of errors");
        Assert.Equal(EdmErrorCode.BadCyclicEntity, errors.First().ErrorCode, "Correct error code");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmComplexType complexTypeDefinitionSmod = (IEdmComplexType)model.SchemaElements.First();
        IEdmComplexType complexTypeDefinitionClod = (IEdmComplexType)model.SchemaElements.Last();
        Assert.True(complexTypeDefinitionClod.BaseType.IsBad(), "Clod basetype is bad because of cycle");
        Assert.Equal(EdmErrorCode.BadCyclicComplex, complexTypeDefinitionClod.BaseType.Errors().First().ErrorCode, "Cyclic entity is cyclic?");
        Assert.True(complexTypeDefinitionSmod.BaseType.IsBad(), "Smod basetype is bad because of cycle");

        EdmValidator.Validate(model, EdmConstants.EdmVersion4, out errors);
        Assert.Equal(2, errors.Count(), "Correct number of errors");
        Assert.Equal(1, errors.Select(e => e.ErrorCode).Distinct().Count(), "Correct number of unique errors");
        Assert.True(errors.Any(e => e.ErrorCode == EdmErrorCode.BadCyclicComplex), "Correct error code 1");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmComplexType complexTypeDefinitionSmod = (IEdmComplexType)model.SchemaElements.First();
        IEdmComplexType complexTypeDefinitionClod = (IEdmComplexType)model.SchemaElements.Last();

        EdmValidator.Validate(model, EdmConstants.EdmVersion4, out errors);
        Assert.Equal(1, errors.Count(), "Correct number of errors");
        Assert.Equal(1, errors.Select(e => e.ErrorCode).Distinct().Count(), "Correct number of unique errors");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmComplexType complexTypeDefinitionSmod = (IEdmComplexType)model.SchemaElements.First();
        IEdmComplexType complexTypeDefinitionClod = (IEdmComplexType)model.SchemaElements.ElementAt(1);
        IEdmComplexType complexTypeDefinitionBlob = (IEdmComplexType)model.SchemaElements.Last();
        Assert.True(complexTypeDefinitionClod.BaseType.IsBad(), "Clod basetype is bad because of cycle");
        Assert.Equal(EdmErrorCode.BadCyclicComplex, complexTypeDefinitionClod.BaseType.Errors().First().ErrorCode, "Cyclic entity is cyclic?");
        Assert.True(complexTypeDefinitionSmod.BaseType.IsBad(), "Smod basetype is bad because of cycle");
        Assert.True(complexTypeDefinitionBlob.BaseType.IsBad(), "Blob basetype is bad because of cycle");

        EdmValidator.Validate(model, EdmConstants.EdmVersion4, out errors);
        Assert.Equal(3, errors.Count(), "Correct number of errors");
        Assert.Equal(1, errors.Select(e => e.ErrorCode).Distinct().Count(), "Correct number of unique errors");
        Assert.True(errors.Any(e => e.ErrorCode == EdmErrorCode.BadCyclicComplex), "Correct error code 1");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");

        IEdmOperation operation = (IEdmOperation)model.SchemaElements.First();
        IEdmTypeReference returnType = operation.ReturnType;
        IEdmTypeReference parameterType = operation.Parameters.First().Type;

        Assert.False(returnType.IsBad(), "Function return type should not be bad");
        Assert.Equal(EdmTypeKind.Collection, returnType.TypeKind(), "Function return type is right kind");
        Assert.Equal(EdmTypeKind.Collection, parameterType.TypeKind(), "Function parameter type is right kind");
        Assert.Equal(EdmTypeKind.Collection, parameterType.AsCollection().ElementType().TypeKind(), "Collection is of collections");
        Assert.Equal(EdmPrimitiveTypeKind.Binary, parameterType.AsCollection().ElementType().AsCollection().ElementType().AsPrimitive().PrimitiveKind(), "root type is binary");
    }


    [Fact]
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
        Assert.True(parsed, "parsed");

        IEdmOperation operation = (IEdmOperation)model.SchemaElements.First();
        IEdmTypeReference returnType = operation.ReturnType;
        IEdmTypeReference parameterType = operation.Parameters.First().Type;

        Assert.False(returnType.IsBad(), "Function return type should not be bad");
        Assert.False(returnType.AsCollection().IsBad(), "Function return type should not be bad");
        Assert.False(returnType.AsCollection().CollectionDefinition().IsBad(), "Function return type should not be bad");
        Assert.False(returnType.AsCollection().CollectionDefinition().ElementType.IsBad(), "Function return type should not be bad");
        Assert.True(returnType.AsCollection().CollectionDefinition().ElementType.Definition.IsBad(), "Function return type should be bad");
        Assert.Equal(EdmTypeKind.Collection, returnType.TypeKind(), "Function return type is right kind");
        Assert.Equal(EdmTypeKind.Collection, parameterType.TypeKind(), "Function parameter type is right kind");
        Assert.Equal(EdmTypeKind.Collection, parameterType.AsCollection().ElementType().TypeKind(), "Collection is of collections");
        Assert.Equal(EdmPrimitiveTypeKind.Binary, parameterType.AsCollection().ElementType().AsCollection().ElementType().AsPrimitive().PrimitiveKind(), "root type is binary");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");

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
        Assert.Equal(4326, myGeography.SpatialReferenceIdentifier, "Geography SRID correct");
        Assert.Equal(4326, myPoint.SpatialReferenceIdentifier, "Geography SRID correct");
        Assert.Equal(4326, myLineString.SpatialReferenceIdentifier, "Geography SRID correct");
        Assert.Equal(4326, myPolygon.SpatialReferenceIdentifier, "Geography SRID correct");
        Assert.Equal(4326, myGeographyCollection.SpatialReferenceIdentifier, "Geography SRID correct");
        Assert.Equal(4326, myMultiPolygon.SpatialReferenceIdentifier, "Geography SRID correct");
        Assert.Equal(4326, myMultiLineString.SpatialReferenceIdentifier, "Geography SRID correct");
        Assert.Equal(4326, myMultiPoint.SpatialReferenceIdentifier, "Geography SRID correct");
        Assert.Equal(0, myGeometry.SpatialReferenceIdentifier, "Geometry SRID correct");
        Assert.Equal(0, myGeometricPoint.SpatialReferenceIdentifier, "Geometry SRID correct");
        Assert.Equal(0, myGeometricLineString.SpatialReferenceIdentifier, "Geometry SRID correct");
        Assert.Equal(0, myGeometricPolygon.SpatialReferenceIdentifier, "Geometry SRID correct");
        Assert.Equal(0, myGeometryCollection.SpatialReferenceIdentifier, "Geometry SRID correct");
        Assert.Equal(0, myGeometricMultiPolygon.SpatialReferenceIdentifier, "Geometry SRID correct");
        Assert.Equal(0, myGeometricMultiLineString.SpatialReferenceIdentifier, "Geometry SRID correct");
        Assert.Equal(null, myGeometricMultiPoint.SpatialReferenceIdentifier, "Variable SRID correct");
        Assert.Equal(null, myOtherGeometricMultiPoint.SpatialReferenceIdentifier, "variable SRID correct");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmComplexType complex1 = (IEdmComplexType)model.SchemaElements.ElementAt(0);
        IEdmComplexType complex2 = (IEdmComplexType)model.SchemaElements.ElementAt(1);
        IEdmComplexType complex3 = (IEdmComplexType)model.SchemaElements.ElementAt(2);

        Assert.Equal(complex1.BaseComplexType().FullName(), complex2.FullName(), "Correct base type name");
        Assert.Equal(complex2.BaseComplexType().FullName(), complex3.FullName(), "Correct base type name");
        Assert.Equal(complex3.BaseComplexType().FullName(), complex1.FullName(), "Correct base type name");

        Assert.True(complex1.BaseType.IsBad(), "Cyclic base type is bad");
        Assert.Equal(EdmErrorCode.BadCyclicComplex, complex1.BaseType.Errors().First().ErrorCode, "Cyclic is cyclic");
        Assert.True(complex2.BaseType.IsBad(), "Cyclic base type is bad");
        Assert.True(complex3.BaseType.IsBad(), "Cyclic base type is bad");

        EdmValidator.Validate(model, EdmConstants.EdmVersion4, out errors);
        Assert.Equal(3, errors.Count(), "Correct number of errors");
        Assert.Equal(1, errors.Select(e => e.ErrorCode).Distinct().Count(), "Correct number of unique errors");
        Assert.True(errors.Any(e => e.ErrorCode == EdmErrorCode.BadCyclicComplex), "Correct error code 1");
    }

    [Fact]
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
        bool parsed = SchemaReader.TryParse(new[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
        Assert.True(parsed, "parsed");
        Assert.True(!errors.Any(), "No errors");

        IEdmEntityContainer fooContainer = model.FindEntityContainer("fooContainer");
        IEdmOperationImport peopleWhoAreAwesome = (IEdmOperationImport)fooContainer.Elements.Last();
        IEdmEntitySetBase badSet;
        Assert.False(peopleWhoAreAwesome.TryGetStaticEntitySet(model, out badSet), "peopleWhoAreAwesome.TryGetStaticEntitySet");

        Assert.False(EdmValidator.Validate(model, EdmConstants.EdmVersion4, out errors), "EdmValidator.Validate(model");
        Assert.Equal(1, errors.Count(), "Correct number of errors");
        Assert.Equal(EdmErrorCode.OperationImportEntitySetExpressionIsInvalid, errors.First().ErrorCode, "Correct error code");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        var colorType = model.FindType("foo.Color") as IEdmEnumType;
        Assert.NotNull(colorType, "foo.Color enum type exists");
        Assert.Equal("Color", colorType.Name, "Color.Name");
        Assert.False(colorType.IsFlags, "Color.IsFlags");
        Assert.False(colorType.IsBad(), "Color.IsBad");
        Assert.Equal(EdmPrimitiveTypeKind.Int32, colorType.UnderlyingType.PrimitiveKind, "Color.UnderlyingType.Kind");
        Assert.Equal(4, colorType.Members.Count(), "Color has 4 members");
        var colors = colorType.Members.ToArray();
        Assert.Equal("Blue", colors[2].Name, "Color.Blue.Name");
        Assert.Equal(colorType, colors[2].DeclaringType, "Color.Blue.DeclaringType");

        var color2Type = model.FindType("foo.Color2") as IEdmEnumType;
        Assert.NotNull(color2Type, "foo.Color2 enum type exists");
        Assert.Equal("Color2", color2Type.Name, "Color2.Name");
        Assert.True(color2Type.IsFlags, "Color2.IsFlags");
        Assert.False(color2Type.IsBad(), "Color2.IsBad");
        Assert.Equal(EdmPrimitiveTypeKind.Int64, color2Type.UnderlyingType.PrimitiveKind, "Color2.UnderlyingType.Kind");
        Assert.Equal(4, colorType.Members.Count(), "Color2 has 4 members");
        var colors2 = color2Type.Members.ToArray();
        Assert.Equal("Blue", colors2[2].Name, "Color2.Blue.Name");
        Assert.Equal(color2Type, colors2[2].DeclaringType, "Color2.Blue.DeclaringType");

        var personType = model.FindType("foo.Person") as IEdmEntityType;
        Assert.NotNull(personType, "foo.Person type exists");
        Assert.Equal(colorType, personType.Properties().ElementAt(1).Type.AsEnum().EnumDefinition(), "Person.c1.Type == foo.Color");
        Assert.True(personType.Properties().ElementAt(1).Type.AsEnum().IsNullable, "Person.c1.Type.Nullable");
        Assert.Equal(color2Type, personType.Properties().ElementAt(2).Type.AsEnum().EnumDefinition(), "Person.c2.Type == foo.Color2");
        Assert.False(personType.Properties().ElementAt(2).Type.AsEnum().IsNullable, "Person.c2.Type.Nullable");
        Assert.True(personType.Properties().ElementAt(3).Type.AsEnum().IsNullable, "Person.c3.Type.Nullable");

        Assert.Equal((color2Type.Members.ElementAt(0).Value).Value, 5, "Correct value");
        Assert.Equal((color2Type.Members.ElementAt(1).Value).Value, 6, "Correct value");
        Assert.Equal((color2Type.Members.ElementAt(2).Value).Value, 10, "Correct value");
        Assert.Equal((color2Type.Members.ElementAt(3).Value).Value, 11, "Correct value");

        Assert.Equal((colorType.Members.ElementAt(0).Value).Value, 10, "Correct value");
        Assert.Equal((colorType.Members.ElementAt(1).Value).Value, 11, "Correct value");
        Assert.Equal((colorType.Members.ElementAt(2).Value).Value, 5, "Correct value");
        Assert.Equal((colorType.Members.ElementAt(3).Value).Value, 6, "Correct value");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.Equal(0, errors.Count(), "No errors");

        bool valid = model.Validate(out errors);
        Assert.False(valid, "valid");
        Assert.Equal(1, errors.Count(), "one validation errors expected");
        Assert.Equal(EdmErrorCode.EnumMemberMustHaveValue, errors.ElementAt(0).ErrorCode, "EnumMemberMustHaveValue expected");
        Assert.True(errors.ElementAt(0).ErrorLocation.ToString().Contains("(4, 8)"), "error location");

        IEdmEnumMember enumMemeber = ((IEdmEnumType)model.FindType("foo.Color")).Members.First(m => m.Name == "Green");
        Assert.Equal(model.FindType("foo.Color"), enumMemeber.DeclaringType, "Enum member has correct type.");
        Assert.True(enumMemeber.Value.IsBad(), "Enum member value is bad.");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.Equal(0, errors.Count(), "No errors");

        bool valid = model.Validate(out errors);
        Assert.False(valid, "valid");
        Assert.Equal(1, errors.Count(), "one validation errors expected");
        Assert.Equal(EdmErrorCode.EnumMemberValueOutOfRange, errors.ElementAt(0).ErrorCode, "EnumMemberValueOutOfRange expected");
        Assert.True(errors.ElementAt(0).ErrorLocation.ToString().Contains("(4, 8)"), "error location");

        IEdmEnumMember enumMemeber = ((IEdmEnumType)model.FindType("foo.Color")).Members.First(m => m.Name == "Green");
        Assert.Equal(model.FindType("foo.Color"), enumMemeber.DeclaringType, "Enum member has correct type.");
        Assert.False(enumMemeber.Value.IsBad(), "Enum member value is good.");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.Equal(0, errors.Count(), "No errors");

        bool valid = model.Validate(out errors);
        Assert.True(valid, "valid");
        Assert.Equal(0, errors.Count(), "no validation errors");

        IEdmEnumType enumType = ((IEdmEnumType)model.FindType("foo.Color"));
        Assert.Equal((enumType.Members.ElementAt(0).Value).Value, 0, "Correct value");
        Assert.Equal((enumType.Members.ElementAt(1).Value).Value, 1, "Correct value");
        Assert.Equal((enumType.Members.ElementAt(2).Value).Value, 5, "Correct value");
        Assert.Equal((enumType.Members.ElementAt(3).Value).Value, 6, "Correct value");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        bool valid = model.Validate(out errors);
        Assert.False(valid, "valid");
        Assert.Equal(4, errors.Count(), "one validation error expected");

        Assert.Equal(EdmErrorCode.BadUnresolvedPrimitiveType, errors.ElementAt(0).ErrorCode, "BadUnresolvedPrimitiveType expected");
        Assert.True(errors.ElementAt(0).ErrorLocation.ToString().Contains("(9, 6)"), "error location");

        Assert.Equal(EdmErrorCode.EnumMustHaveIntegerUnderlyingType, errors.ElementAt(1).ErrorCode, "EnumMustHaveIntegerUnderlyingType expected");
        Assert.True(errors.ElementAt(1).ErrorLocation.ToString().Contains("(15, 6)"), "error location");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmTerm age = model.FindTerm("foo.Age");
        IEdmTerm subject = model.FindTerm("foo.Subject");

        Assert.Equal(age.Name, "Age", "Term name");
        Assert.Equal(subject.Name, "Subject", "Term name");

        Assert.Equal(age.Type.AsPrimitive().PrimitiveKind(), EdmPrimitiveTypeKind.Int32, "Term type");
        Assert.Equal(subject.Type.AsEntity().FullName(), "foo.Person", "Term type");

        Assert.Equal(age.Namespace, "foo", "Term namespace");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmEntityType person = (IEdmEntityType)model.FindType("foo.Person");
        IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model);
        Assert.Equal(personAnnotations.Count(), 3, "Annotations count");

        IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();
        IEdmVocabularyAnnotation first = annotations[0];
        Assert.Equal(first.Qualifier, "First", "Annotation qualifier");
        Assert.Equal(first.Term.Name, "Age", "Term name");
        Assert.Equal(first.Term.Namespace, "foo", "Term namespace");

        IEdmIntegerConstantExpression value = (IEdmIntegerConstantExpression)first.Value;
        Assert.Equal(value.Value, 123, "Annotation value");

        IEdmVocabularyAnnotation second = annotations[1];
        Assert.Equal(second.Qualifier, "Best", "Annotation qualifier");
        Assert.Equal(second.Term.Name, "Age", "Term name");
        Assert.Equal(second.Term.Namespace, "foo", "Term namespace");

        value = (IEdmIntegerConstantExpression)second.Value;
        Assert.Equal(value.Value, 456, "Annotation value");

        IEdmVocabularyAnnotation third = annotations[2];
        Assert.Null(third.Qualifier, "Annotation qualifier");
        Assert.Equal(third.Term.Name, "Mage", "Term name");
        Assert.Equal(third.Term.Namespace, "Funk", "Term namespace");

        value = (IEdmIntegerConstantExpression)third.Value;
        Assert.Equal(value.Value, 789, "Annotation value");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmEntityType person = (IEdmEntityType)model.FindType("foo.Person");
        IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model);
        Assert.Equal(personAnnotations.Count(), 3, "Annotations count");

        IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();
        IEdmVocabularyAnnotation first = annotations[0];
        Assert.Equal(first.Qualifier, "First", "Annotation qualifier");
        Assert.Equal(first.Term.Name, "Age", "Term name");
        Assert.Equal(first.Term.Namespace, "foo", "Term namespace");

        IEdmIntegerConstantExpression value = (IEdmIntegerConstantExpression)first.Value;
        Assert.Equal(value.Value, 123, "Annotation value");

        IEdmVocabularyAnnotation second = annotations[1];
        Assert.Equal(second.Qualifier, "Best", "Annotation qualifier");
        Assert.Equal(second.Term.Name, "Age", "Term name");
        Assert.Equal(second.Term.Namespace, "foo", "Term namespace");

        value = (IEdmIntegerConstantExpression)second.Value;
        Assert.Equal(value.Value, 456, "Annotation value");

        IEdmVocabularyAnnotation third = annotations[2];
        Assert.Null(third.Qualifier, "Annotation qualifier");
        Assert.Equal(third.Term.Name, "Mage", "Term name");
        Assert.Equal(third.Term.Namespace, "Funk", "Term namespace");
        value = (IEdmIntegerConstantExpression)third.Value;
        Assert.Equal(value.Value, 789, "Annotation value");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmEntityType person = (IEdmEntityType)model.FindType("foo.Person");
        IEdmEntityType lyingPerson = (IEdmEntityType)model.FindType("foo.LyingPerson");

        IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model);
        Assert.Equal(personAnnotations.Count(), 3, "Annotations count");
        IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();

        IEdmVocabularyAnnotation first = annotations[0];
        Assert.Equal(first.Qualifier, "First", "Annotation qualifier");
        Assert.Equal(first.Term.Name, "Age", "Term name");
        Assert.Equal(first.Term.Namespace, "foo", "Term namespace");
        IEdmIntegerConstantExpression value = (IEdmIntegerConstantExpression)first.Value;
        Assert.Equal(value.Value, 123, "Annotation value");

        IEdmVocabularyAnnotation second = annotations[1];
        Assert.Equal(second.Qualifier, "Best", "Annotation qualifier");
        Assert.Equal(second.Term.Name, "Age", "Term name");
        Assert.Equal(second.Term.Namespace, "foo", "Term namespace");
        value = (IEdmIntegerConstantExpression)second.Value;
        Assert.Equal(value.Value, 456, "Annotation value");

        IEdmVocabularyAnnotation third = annotations[2];
        Assert.Null(third.Qualifier, "Annotation qualifier");
        Assert.Equal(third.Term.Name, "Mage", "Term name");
        Assert.Equal(third.Term.Namespace, "Funk", "Term namespace");
        value = (IEdmIntegerConstantExpression)third.Value;
        Assert.Equal(value.Value, 789, "Annotation value");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmEntityType person = (IEdmEntityType)model.FindType("foo.Person");
        IEdmEntityType lyingPerson = (IEdmEntityType)model.FindType("foo.LyingPerson");

        IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model);
        Assert.Equal(personAnnotations.Count(), 6, "Annotations count");
        IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();

        IEdmVocabularyAnnotation first = annotations[0];
        Assert.Equal(first.Qualifier, "Middling", "Annotation qualifier");
        Assert.Equal(first.Term.Name, "Age", "Term name");
        Assert.Equal(first.Term.Namespace, "foo", "Term namespace");
        IEdmIntegerConstantExpression value = (IEdmIntegerConstantExpression)first.Value;
        Assert.Equal(value.Value, 777, "Annotation value");

        IEdmVocabularyAnnotation second = annotations[1];
        Assert.Equal(second.Qualifier, "Middling", "Annotation qualifier");
        Assert.Equal(second.Term.Name, "Mage", "Term name");
        Assert.Equal(second.Term.Namespace, "Var1", "Term namespace");
        value = (IEdmIntegerConstantExpression)second.Value;
        Assert.Equal(value.Value, 888, "Annotation value");

        IEdmVocabularyAnnotation third = annotations[2];
        Assert.Equal(third.Qualifier, "First", "Annotation qualifier");
        Assert.Equal(third.Term.Name, "Age", "Term name");
        Assert.Equal(third.Term.Namespace, "foo", "Term namespace");
        value = (IEdmIntegerConstantExpression)third.Value;
        Assert.Equal(value.Value, 123, "Annotation value");

        IEdmVocabularyAnnotation fourth = annotations[3];
        Assert.Equal(fourth.Qualifier, "Best", "Annotation qualifier");
        Assert.Equal(fourth.Term.Name, "Age", "Term name");
        Assert.Equal(fourth.Term.Namespace, "foo", "Term namespace");
        value = (IEdmIntegerConstantExpression)fourth.Value;
        Assert.Equal(value.Value, 456, "Annotation value");

        IEdmVocabularyAnnotation fifth = annotations[4];
        Assert.Null(fifth.Qualifier, "Annotation qualifier");
        Assert.Equal(fifth.Term.Name, "Mage", "Term name");
        Assert.Equal(fifth.Term.Namespace, "Funk", "Term namespace");
        value = (IEdmIntegerConstantExpression)fifth.Value;
        Assert.Equal(value.Value, 789, "Annotation value");

        IEdmVocabularyAnnotation sixth = annotations[5];
        Assert.Equal(sixth.Qualifier, "Zonky", "Annotation qualifier");
        Assert.Equal(sixth.Term.Name, "Yage", "Term name");
        Assert.Equal(sixth.Term.Namespace, "Var1", "Term namespace");
        value = (IEdmIntegerConstantExpression)sixth.Value;
        Assert.Equal(value.Value, 555, "Annotation value");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmModel model2;
        parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl2)) }, new IEdmModel[] { model1 }, out model2, out errors);
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmEntityType person = (IEdmEntityType)model1.FindType("foo.Person");
        IEdmEntityType lyingPerson = (IEdmEntityType)model1.FindType("foo.LyingPerson");
        IEdmTerm distantAge = model1.FindTerm("foo.DistantAge");
        Assert.Equal(lyingPerson, model2.FindType("foo.LyingPerson"), "Lookup through referenced model");

        IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model2);
        Assert.Equal(personAnnotations.Count(), 4, "Annotations count");

        IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();
        IEdmVocabularyAnnotation third = annotations[0];
        Assert.Equal(third.Qualifier, "First", "Annotation qualifier");
        Assert.Equal(third.Term.Name, "Age", "Term name");
        Assert.Equal(third.Term.Namespace, "foo", "Term namespace");

        IEdmIntegerConstantExpression value = (IEdmIntegerConstantExpression)third.Value;
        Assert.Equal(value.Value, 123, "Annotation value");

        IEdmVocabularyAnnotation fourth = annotations[1];
        Assert.Equal(fourth.Qualifier, "Best", "Annotation qualifier");
        Assert.Equal(fourth.Term.Name, "Age", "Term name");
        Assert.Equal(fourth.Term.Namespace, "foo", "Term namespace");

        value = (IEdmIntegerConstantExpression)fourth.Value;
        Assert.Equal(value.Value, 456, "Annotation value");

        IEdmVocabularyAnnotation fifth = annotations[2];
        Assert.Null(fifth.Qualifier, "Annotation qualifier");
        Assert.Equal(fifth.Term.Name, "DistantAge", "Term name");
        Assert.Equal(fifth.Term.Namespace, "foo", "Term namespace");
        Assert.Equal(fifth.Term, distantAge, "Annotation term");

        value = (IEdmIntegerConstantExpression)fifth.Value;
        Assert.Equal(value.Value, 99, "Annotation value");

        IEdmVocabularyAnnotation sixth = annotations[3];
        Assert.Null(sixth.Qualifier, "Annotation qualifier");
        Assert.Equal(sixth.Term.Name, "Mage", "Term name");
        Assert.Equal(sixth.Term.Namespace, "Funk", "Term namespace");

        value = (IEdmIntegerConstantExpression)sixth.Value;
        Assert.Equal(value.Value, 789, "Annotation value");

        Assert.Equal(1, model1.VocabularyAnnotations.Count(), "Model annotations count");
        Assert.Equal(3, model2.VocabularyAnnotations.Count(), "Model annotations count");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmModel model2;
        parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl2)) }, new IEdmModel[] { model }, out model2, out errors);
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmEntitySet persons = model.FindEntityContainer("bar").FindEntitySet("PersonsOfInterest");

        IEnumerable<IEdmVocabularyAnnotation> personsAnnotations = persons.VocabularyAnnotations(model);
        Assert.Equal(personsAnnotations.Count(), 1, "Annotations count");
        IEdmVocabularyAnnotation[] annotations = personsAnnotations.ToArray<IEdmVocabularyAnnotation>();

        IEdmVocabularyAnnotation mage = annotations[0];
        Assert.Null(mage.Qualifier, "Annotation qualifier");
        Assert.Equal(mage.Term.Name, "Mage", "Term name");
        Assert.Equal(mage.Term.Namespace, "Funk", "Term namespace");
        IEdmIntegerConstantExpression value = (IEdmIntegerConstantExpression)mage.Value;
        Assert.Equal(value.Value, 789, "Annotation value");

        personsAnnotations = persons.VocabularyAnnotations(model2);
        Assert.Equal(personsAnnotations.Count(), 2, "Annotations count");
        annotations = personsAnnotations.ToArray<IEdmVocabularyAnnotation>();

        IEdmVocabularyAnnotation strange = annotations[0];
        Assert.Equal(persons, strange.Target, "Annotation target");
        Assert.Null(strange.Qualifier, "Annotation qualifier");
        Assert.Equal(strange.Term.Name, "Strange", "Term name");
        Assert.Equal(strange.Term.Namespace, "Funk", "Term namespace");
        value = (IEdmIntegerConstantExpression)strange.Value;
        Assert.Equal(value.Value, 13, "Annotation value");

        mage = annotations[1];
        Assert.Equal(persons, mage.Target, "Annotation target");
        Assert.Null(mage.Qualifier, "Annotation qualifier");
        Assert.Equal(mage.Term.Name, "Mage", "Term name");
        Assert.Equal(mage.Term.Namespace, "Funk", "Term namespace");
        value = (IEdmIntegerConstantExpression)mage.Value;
        Assert.Equal(value.Value, 789, "Annotation value");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmModel model2;
        parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl2)) }, model, out model2, out errors);
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmProperty birthday = ((IEdmEntityType)model.FindType("foo.Person")).FindProperty("Birthday");

        IEnumerable<IEdmVocabularyAnnotation> birthdayAnnotations = birthday.VocabularyAnnotations(model);
        Assert.Equal(birthdayAnnotations.Count(), 2, "Annotations count");
        IEdmVocabularyAnnotation[] annotations = birthdayAnnotations.ToArray<IEdmVocabularyAnnotation>();

        IEdmVocabularyAnnotation funkadelic = annotations[0];
        Assert.Null(funkadelic.Qualifier, "Annotation qualifier");
        Assert.Equal(funkadelic.Term.Name, "ADelic", "Term name");
        Assert.Equal(funkadelic.Term.Namespace, "Funk", "Term namespace");
        IEdmIntegerConstantExpression value = (IEdmIntegerConstantExpression)funkadelic.Value;
        Assert.Equal(value.Value, 17, "Annotation value");

        IEdmVocabularyAnnotation mage = annotations[1];
        Assert.Null(mage.Qualifier, "Annotation qualifier");
        Assert.Equal(mage.Term.Name, "Mage", "Term name");
        Assert.Equal(mage.Term.Namespace, "Funk", "Term namespace");
        value = (IEdmIntegerConstantExpression)mage.Value;
        Assert.Equal(value.Value, 789, "Annotation value");

        birthdayAnnotations = birthday.VocabularyAnnotations(model2);
        Assert.Equal(birthdayAnnotations.Count(), 3, "Annotations count");
        annotations = birthdayAnnotations.ToArray<IEdmVocabularyAnnotation>();

        IEdmVocabularyAnnotation strange = annotations[0];
        Assert.Null(strange.Qualifier, "Annotation qualifier");
        Assert.Equal(strange.Term.Name, "Strange", "Term name");
        Assert.Equal(strange.Term.Namespace, "Funk", "Term namespace");
        value = (IEdmIntegerConstantExpression)strange.Value;
        Assert.Equal(value.Value, 13, "Annotation value");

        funkadelic = annotations[1];
        Assert.Null(funkadelic.Qualifier, "Annotation qualifier");
        Assert.Equal(funkadelic.Term.Name, "ADelic", "Term name");
        Assert.Equal(funkadelic.Term.Namespace, "Funk", "Term namespace");
        value = (IEdmIntegerConstantExpression)funkadelic.Value;
        Assert.Equal(value.Value, 17, "Annotation value");

        mage = annotations[2];
        Assert.Null(mage.Qualifier, "Annotation qualifier");
        Assert.Equal(mage.Term.Name, "Mage", "Term name");
        Assert.Equal(mage.Term.Namespace, "Funk", "Term namespace");
        value = (IEdmIntegerConstantExpression)mage.Value;
        Assert.Equal(value.Value, 789, "Annotation value");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmEntityType person = (IEdmEntityType)model.FindType("foo.Person");

        IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model);
        Assert.Equal(personAnnotations.Count(), 18, "Annotations count");
        IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();

        var byteArrayComparer = new ArrayComp();

        int i = 0;
        Assert.Equal((annotations[i]).Value.ExpressionKind, EdmExpressionKind.IntegerConstant, "Annotation expression kind");
        Assert.Equal(((IEdmIntegerConstantExpression)(annotations[i]).Value).Value, 1, "Annotation value");
        Assert.Equal((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.IntegerConstant, "Annotation expression kind");
        Assert.Equal(((IEdmIntegerConstantExpression)(annotations[i]).Value).Value, 2, "Annotation value");

        Assert.Equal((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.StringConstant, "Annotation expression kind");
        Assert.Equal(((IEdmStringConstantExpression)(annotations[i]).Value).Value, "Cat", "Annotation value");
        Assert.Equal((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.StringConstant, "Annotation expression kind");
        Assert.Equal(((IEdmStringConstantExpression)(annotations[i]).Value).Value, "Dog", "Annotation value");

        Assert.Equal((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.BinaryConstant, "Annotation expression kind");
        Assert.True(byteArrayComparer.Equals(((IEdmBinaryConstantExpression)(annotations[i]).Value).Value, new byte[] { 0x12, 0x34, 0x56 }), "Annotation value");
        Assert.Equal((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.BinaryConstant, "Annotation expression kind");
        Assert.True(byteArrayComparer.Equals(((IEdmBinaryConstantExpression)(annotations[i]).Value).Value, new byte[] { 0x65, 0x43, 0x21 }), "Annotation value");

        Assert.Equal((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.FloatingConstant, "Annotation expression kind");
        Assert.Equal(((IEdmFloatingConstantExpression)(annotations[i]).Value).Value, 1.1, "Annotation value");
        Assert.Equal((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.FloatingConstant, "Annotation expression kind");
        Assert.Equal(((IEdmFloatingConstantExpression)(annotations[i]).Value).Value, 2.2E10, "Annotation value");

        Assert.Equal((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.GuidConstant, "Annotation expression kind");
        Assert.Equal(((IEdmGuidConstantExpression)(annotations[i]).Value).Value, Guid.Parse("4ae71c81-c21a-40a2-8d53-f1a29ed4a2f2"), "Annotation value");
        Assert.Equal((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.GuidConstant, "Annotation expression kind");
        Assert.Equal(((IEdmGuidConstantExpression)(annotations[i]).Value).Value, Guid.Parse("4ae71c81-c21a-40a2-8d53-f1a29ed4a2f3"), "Annotation value");

        Assert.Equal((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.DecimalConstant, "Annotation expression kind");
        Assert.Equal(((IEdmDecimalConstantExpression)(annotations[i]).Value).Value, 1.2M, "Annotation value");
        Assert.Equal((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.DecimalConstant, "Annotation expression kind");
        Assert.Equal(((IEdmDecimalConstantExpression)(annotations[i]).Value).Value, 2.3M, "Annotation value");

        Assert.Equal((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.BooleanConstant, "Annotation expression kind");
        Assert.Equal(((IEdmBooleanConstantExpression)(annotations[i]).Value).Value, true, "Annotation value");
        Assert.Equal((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.BooleanConstant, "Annotation expression kind");
        Assert.Equal(((IEdmBooleanConstantExpression)(annotations[i]).Value).Value, false, "Annotation value");

        Assert.Equal((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.DateTimeOffsetConstant, "Annotation expression kind");
        Assert.Equal(((IEdmDateTimeOffsetConstantExpression)(annotations[i]).Value).Value, new DateTimeOffset(2001, 10, 26, 19, 32, 52, new TimeSpan(0, 0, 0)), "Annotation value");
        Assert.Equal((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.DateTimeOffsetConstant, "Annotation expression kind");
        Assert.Equal(((IEdmDateTimeOffsetConstantExpression)(annotations[i]).Value).Value, new DateTimeOffset(2001, 10, 26, 19, 32, 52, new TimeSpan(0, 0, 0)), "Annotation value");

        Assert.Equal((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.DurationConstant, "Annotation expression kind");
        Assert.Equal(new TimeSpan(0, 0, 1), ((IEdmDurationConstantExpression)(annotations[i]).Value).Value, "Annotation value");
        Assert.Equal((annotations[++i]).Value.ExpressionKind, EdmExpressionKind.DurationConstant, "Annotation expression kind");
        Assert.Equal(new TimeSpan(0, 0, 1), ((IEdmDurationConstantExpression)(annotations[i]).Value).Value, "Annotation value");
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

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmEntityType person = (IEdmEntityType)model.FindType("foo.Person");

        IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model);
        Assert.Equal(personAnnotations.Count(), 1, "Annotations count");
        IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();

        IEdmVocabularyAnnotation first = annotations[0];
        Assert.Equal(first.Value.ExpressionKind, EdmExpressionKind.Record, "Annotation expression kind");

        IEdmRecordExpression record = (IEdmRecordExpression)first.Value;
        IEnumerable<IEdmPropertyConstructor> propertyConstructors = record.Properties;
        Assert.Equal(propertyConstructors.Count(), 2, "Property constructors count");
        IEdmPropertyConstructor[] properties = propertyConstructors.ToArray<IEdmPropertyConstructor>();

        IEdmPropertyConstructor x = properties[0];
        Assert.Equal(x.Name, "X", "Property name");
        Assert.Equal(x.Value.ExpressionKind, EdmExpressionKind.IntegerConstant, "Property expression kind");
        Assert.Equal(((IEdmIntegerConstantExpression)x.Value).Value, 10, "Property expression value");

        IEdmPropertyConstructor y = properties[1];
        Assert.Equal(y.Name, "Y", "Property name");
        Assert.Equal(y.Value.ExpressionKind, EdmExpressionKind.IntegerConstant, "Property expression kind");
        Assert.Equal(((IEdmIntegerConstantExpression)y.Value).Value, 20, "Property expression value");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "No errors");

        IEdmEntityType person = (IEdmEntityType)model.FindType("foo.Person");
        IEdmProperty name = person.FindProperty("Name");
        IEdmProperty age = person.FindProperty("Age");
        IEdmEntityType lyingPerson = (IEdmEntityType)model.FindType("foo.LyingPerson");

        IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model);
        Assert.Equal(personAnnotations.Count(), 2, "Annotations count");
        IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();

        IEdmVocabularyAnnotation first = annotations[0];
        Assert.Equal(first.Term.Name, "Mage", "Term name");
        IEdmPathExpression value = (IEdmPathExpression)first.Value;
        Assert.Equal(value.PathSegments.First(), "Age", "Bound path name");

        IEdmVocabularyAnnotation second = annotations[1];
        Assert.Equal(second.Term.Name, "Age", "Term name");
        value = (IEdmPathExpression)second.Value;
        Assert.Equal(value.PathSegments.First(), "Age", "Bound path name");
    }

    [Fact]
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
        Assert.False(isParsed, "SchemaReader.TryParse failed");
        Assert.True(errors.Count() == 4, "SchemaReader.TryParse returned errors");
        Assert.Equal(EdmErrorCode.UnexpectedXmlElement, errors.OrderBy(e => e.ErrorCode).ElementAt(0).ErrorCode, "EdmErrorCode.UnexpectedXmlElement");
        Assert.Equal(EdmErrorCode.MissingAttribute, errors.OrderBy(e => e.ErrorCode).ElementAt(1).ErrorCode, "EdmErrorCode.MissingAttribute");
        Assert.Equal(EdmErrorCode.MissingType, errors.OrderBy(e => e.ErrorCode).ElementAt(3).ErrorCode, "EdmErrorCode.MissingType");
    }

    [Fact]
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
        Assert.True(isParsed, "SchemaReader.TryParse failed");

        IEdmVocabularyAnnotation annotation = edmModel.VocabularyAnnotations.First();
        Assert.Equal("Netflix.Catalog.v2", ((IEdmSchemaElement)annotation.Target).Namespace, "Namespace is resolved correctly");
    }

    [Fact]
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
        Assert.False(isParsed, "SchemaReader.TryParse failed");
        Assert.Equal(1, errors.Count(), "Invalid error count.");
        Assert.Equal(EdmErrorCode.InvalidQualifiedName, errors.First().ErrorCode, "Invalid error code.");
    }

    [Fact]
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
        Assert.False(isParsed, "SchemaReader.TryParse failed");
        Assert.Equal(1, errors.Count(), "Invalid error count.");
        Assert.Equal(EdmErrorCode.InvalidQualifiedName, errors.First().ErrorCode, "Invalid error code.");
    }

    [Fact]
    public void TestingAnnotationQualifiersWithNonSimpleValue()
    {
        var csdl = VocabularyTestModelBuilder.AnnotationQualifiersWithNonSimpleValue();
        IEdmModel model = this.GetParserResult(csdl);

        var expectedErrors = new EdmLibTestErrors();
        this.VerifySemanticValidation(model, Microsoft.Test.OData.Utils.Metadata.EdmVersion.V40, expectedErrors);
    }

    [Fact]
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
        Assert.Equal(1, model.VocabularyAnnotations.Count(), "Invalid annotation count.");

        IEnumerable<EdmError> errors;
        var roundTripCsdls = this.GetSerializerResult(model, Microsoft.Test.OData.Utils.Metadata.EdmVersion.V40, out errors).Select(n => XElement.Parse(n));
        Assert.Equal(0, errors.Count(), "Invalid error count.");
        Assert.Equal(1, roundTripCsdls.Count(), "Invalid csdl count.");

        new ConstructiveApiCsdlXElementComparer().Compare(actualCsdls.ToList(), roundTripCsdls.ToList());
    }

    [Fact]
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
        Assert.Equal(1, model.VocabularyAnnotations.Count(), "Invalid annotation count.");

        var annotationTarget = model.VocabularyAnnotations.First().Target;
        Assert.True(annotationTarget.ToString().Contains("Netflix.Catalog.v2.Person"), "Invalid target ToString().");
    }

    [Fact]
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

        Assert.True(parsed, "parsed");
        Assert.False(errors.Any(), "No errors");

        var types = model.SchemaElements.OfType<IEdmComplexType>().ToList();
        Assert.True(types.Count == 3, "Should have three types");
        Assert.True(types[0].IsOpen, "Should be open type");
        Assert.False(types[1].IsOpen, "Should not be open type");
        Assert.False(types[2].IsOpen, "Should not be open type");
    }

    [Fact]
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

        Assert.True(parsed, "parsed");
        Assert.False(errors.Any(), "No errors");

        var elements = model.SchemaElements.ToList();

        // Length
        var lengthType = (IEdmTypeDefinition)elements[0];
        Assert.Equal(EdmPrimitiveTypeKind.Int32, lengthType.UnderlyingType.PrimitiveKind);
        Assert.Equal(EdmSchemaElementKind.TypeDefinition, lengthType.SchemaElementKind);
        Assert.Equal("Length", lengthType.Name);

        // Width
        var widthType = (IEdmTypeDefinition)elements[1];
        Assert.Equal(EdmPrimitiveTypeKind.Int32, widthType.UnderlyingType.PrimitiveKind);
        Assert.Equal(EdmSchemaElementKind.TypeDefinition, widthType.SchemaElementKind);
        Assert.Equal("Width", widthType.Name);
        var widthElement = ((CsdlSemanticsElement)widthType).Element;
        Assert.Equal(1, widthElement.VocabularyAnnotations.Count());

        // Weight
        var weightType = (IEdmTypeDefinition)elements[2];
        Assert.Equal(EdmPrimitiveTypeKind.Decimal, weightType.UnderlyingType.PrimitiveKind);
        Assert.Equal(EdmSchemaElementKind.TypeDefinition, weightType.SchemaElementKind);
        Assert.Equal("Weight", weightType.Name);
        var weightElement = ((CsdlSemanticsElement)weightType).Element;
        Assert.Equal("Edm.Decimal", ((CsdlTypeDefinition)weightElement).UnderlyingTypeName);

        // Address
        var addressType = (IEdmTypeDefinition)elements[3];
        Assert.Equal(EdmPrimitiveTypeKind.String, addressType.UnderlyingType.PrimitiveKind);
        Assert.Equal(EdmSchemaElementKind.TypeDefinition, addressType.SchemaElementKind);
        Assert.Equal("Address", addressType.Name);
        var addressElement = ((CsdlSemanticsElement)addressType).Element;
        Assert.Equal("Edm.String", ((CsdlTypeDefinition)addressElement).UnderlyingTypeName);

        // Point
        var pointType = (IEdmTypeDefinition)elements[4];
        Assert.Equal(EdmPrimitiveTypeKind.GeographyPoint, pointType.UnderlyingType.PrimitiveKind);
        Assert.Equal(EdmSchemaElementKind.TypeDefinition, pointType.SchemaElementKind);
        Assert.Equal("Point", pointType.Name);
        var pointElement = ((CsdlSemanticsElement)pointType).Element;
        Assert.Equal("Edm.GeographyPoint", ((CsdlTypeDefinition)pointElement).UnderlyingTypeName);

        // Person
        var personType = elements[5] as IEdmEntityType;
        var weightProperty = personType.FindProperty("Weight");
        var addressProperty = personType.FindProperty("Address");
        var addressProperty2 = personType.FindProperty("Address2");
        var pointProperty = personType.FindProperty("Point");
        Assert.Equal(weightProperty.Type.AsTypeDefinition().FullName(), "MyNS.Weight");
        Assert.Equal(weightProperty.Type.AsTypeDefinition().Definition, weightType);
        Assert.Equal(addressProperty.Type.AsTypeDefinition().FullName(), "MyNS.Address");
        Assert.Equal(addressProperty.Type.AsTypeDefinition().Definition, addressType);
        Assert.Equal(addressProperty2.Type.AsTypeDefinition().FullName(), "MyNS.Address");
        Assert.Equal(addressProperty2.Type.AsTypeDefinition().Definition, addressType);
        Assert.Equal(pointProperty.Type.AsTypeDefinition().FullName(), "MyNS.Point");
        Assert.Equal(pointProperty.Type.AsTypeDefinition().Definition, pointType);

        // Facets
        Assert.Equal(false, ((IEdmTypeDefinitionReference)weightProperty.Type).IsUnbounded);
        Assert.Equal(null, ((IEdmTypeDefinitionReference)weightProperty.Type).MaxLength);
        Assert.Equal(null, ((IEdmTypeDefinitionReference)weightProperty.Type).IsUnicode);
        Assert.Equal(3, ((IEdmTypeDefinitionReference)weightProperty.Type).Precision);
        Assert.Equal(2, ((IEdmTypeDefinitionReference)weightProperty.Type).Scale);
        Assert.Equal(null, ((IEdmTypeDefinitionReference)weightProperty.Type).SpatialReferenceIdentifier);

        Assert.Equal(false, ((IEdmTypeDefinitionReference)addressProperty.Type).IsUnbounded);
        Assert.Equal(10, ((IEdmTypeDefinitionReference)addressProperty.Type).MaxLength);
        Assert.Equal(true, ((IEdmTypeDefinitionReference)addressProperty.Type).IsUnicode);
        Assert.Equal(null, ((IEdmTypeDefinitionReference)addressProperty.Type).Precision);
        Assert.Equal(null, ((IEdmTypeDefinitionReference)addressProperty.Type).Scale);
        Assert.Equal(null, ((IEdmTypeDefinitionReference)addressProperty.Type).SpatialReferenceIdentifier);

        Assert.Equal(true, ((IEdmTypeDefinitionReference)addressProperty2.Type).IsUnbounded);
        Assert.Equal(null, ((IEdmTypeDefinitionReference)addressProperty2.Type).MaxLength);
        Assert.Equal(false, ((IEdmTypeDefinitionReference)addressProperty2.Type).IsUnicode);
        Assert.Equal(null, ((IEdmTypeDefinitionReference)addressProperty2.Type).Precision);
        Assert.Equal(null, ((IEdmTypeDefinitionReference)addressProperty2.Type).Scale);
        Assert.Equal(null, ((IEdmTypeDefinitionReference)addressProperty2.Type).SpatialReferenceIdentifier);

        Assert.Equal(false, ((IEdmTypeDefinitionReference)pointProperty.Type).IsUnbounded);
        Assert.Equal(null, ((IEdmTypeDefinitionReference)pointProperty.Type).MaxLength);
        Assert.Equal(null, ((IEdmTypeDefinitionReference)pointProperty.Type).IsUnicode);
        Assert.Equal(null, ((IEdmTypeDefinitionReference)pointProperty.Type).Precision);
        Assert.Equal(null, ((IEdmTypeDefinitionReference)pointProperty.Type).Scale);
        Assert.Equal(123, ((IEdmTypeDefinitionReference)pointProperty.Type).SpatialReferenceIdentifier);
    }

    [Fact]
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

        Assert.True(parsed, "parsed");
        Assert.False(errors.Any(), "No errors");

        var elements = model.SchemaElements.ToList();

        // UInt16
        Assert.Equal(((IEdmTypeDefinition)elements[0]).UnderlyingType.PrimitiveKind, EdmPrimitiveTypeKind.Int32);
        Assert.Equal(((IEdmSchemaElement)elements[0]).SchemaElementKind, EdmSchemaElementKind.TypeDefinition);
        Assert.Equal(((IEdmNamedElement)elements[0]).Name, "UInt16");

        // UInt32
        Assert.Equal(((IEdmTypeDefinition)elements[1]).UnderlyingType.PrimitiveKind, EdmPrimitiveTypeKind.Int64);
        Assert.Equal(elements[1].SchemaElementKind, EdmSchemaElementKind.TypeDefinition);
        Assert.Equal(((IEdmNamedElement)elements[1]).Name, "UInt32");

        // UInt64
        Assert.Equal(((IEdmTypeDefinition)elements[2]).UnderlyingType.PrimitiveKind, EdmPrimitiveTypeKind.Decimal);
        Assert.Equal(elements[2].SchemaElementKind, EdmSchemaElementKind.TypeDefinition);
        Assert.Equal(((IEdmNamedElement)elements[2]).Name, "UInt64");

        // Person
        var personType = elements[3] as IEdmEntityType;
        Assert.NotNull(personType, "MyNS.Person");
        var idProperty = personType.FindProperty("Id");
        var shortIntProperty = personType.FindProperty("ShortInt");
        var longIntProperty = personType.FindProperty("LongInt");
        Assert.Equal(idProperty.Type.AsTypeDefinition().FullName(), "MyNS.UInt32");
        Assert.Equal(shortIntProperty.Type.AsTypeDefinition().FullName(), "MyNS.UInt16");
        Assert.Equal(longIntProperty.Type.AsTypeDefinition().FullName(), "MyNS.UInt64");
    }

    [Fact]
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

        Assert.True(parsed, "parsed");
        Assert.False(errors.Any(), "No errors");

        var elements = model.SchemaElements.ToList();

        // Person
        var personType = elements[0] as IEdmEntityType;
        Assert.NotNull(personType, "MyNS.Person");
        var idProperty = personType.FindProperty("Id");
        var dateTimeOffsetProperty = personType.FindProperty("dateTimeOffset");
        var dateProperty = personType.FindProperty("date");
        var timeOfDayProperty = personType.FindProperty("timeofday");
        Assert.Equal(idProperty.Type.AsTypeDefinition().FullName(), "Edm.Int32");
        Assert.Equal(dateTimeOffsetProperty.Type.AsTypeDefinition().FullName(), "Edm.DateTimeOffset");
        Assert.Equal(dateProperty.Type.AsTypeDefinition().FullName(), "Edm.Date");
        Assert.Equal(timeOfDayProperty.Type.AsTypeDefinition().FullName(), "Edm.TimeOfDay");
    }

    [Fact]
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

        Assert.True(parsed, "parsed");
        Assert.False(errors.Any(), "No errors");

        var term = model.FindTerm("MyNS.ConventionalIDs");
        Assert.Equal(term.DefaultValue, "True");
    }


    [Fact]
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

        Assert.True(parsed, "parsed");
        Assert.False(errors.Any(), "No errors");

        model.Validate(out validationErrors);
        Assert.Equal(validationErrors.Single().ErrorMessage, "An element with the name 'MyNS.Length' is already defined.");
    }

    [Fact]
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

        Assert.True(parsed, "parsed");
        Assert.False(errors.Any(), "No errors");

        model.Validate(out validationErrors);
        Assert.Equal(validationErrors.Single().ErrorMessage, "An element with the name 'MyNS.Person' is already defined.");
    }

    [Fact]
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

        Assert.True(parsed, "parsed");
        Assert.False(errors.Any(), "No errors");

        model.Validate(out validationErrors);
        Assert.Equal(validationErrors.Single().ErrorMessage, "Each property name in a type must be unique. Property name 'Id' is already defined.");
    }

    [Fact]
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

        Assert.True(parsed, "parsed");
        Assert.False(errors.Any(), "No errors");

        model.Validate(out validationErrors);
        Assert.Equal(validationErrors.Single().ErrorMessage, "An element with the name 'MyNS.Person' is already defined.");
    }

    [Fact]
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
        Assert.True(parsed, "parsed");
        Assert.Equal(0, errors.Count());
        bool valid = model.Validate(out errors);
        Assert.True(valid, "valid");
    }
}