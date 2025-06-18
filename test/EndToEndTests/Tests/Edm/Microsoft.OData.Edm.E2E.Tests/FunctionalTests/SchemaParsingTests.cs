//---------------------------------------------------------------------
// <copyright file="SchemaParsingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml;
using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class SchemaParsingTests : EdmLibTestCaseBase
{
    [Fact]
    public void Parse_EmptySchema_ParsesSuccessfully()
    {
        var csdl = @"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm""/>";

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);
    }

    [Fact]
    public void Parse_EmptySchemaWithClosingTag_ParsesSuccessfully()
    {
        var csdl = @"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm""></Schema>";

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);
    }

    [Fact]
    public void Parse_SimpleSchemaWithEntityContainer_ParsesAndValidatesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        Assert.Equal("C1", model.EntityContainer.Name);
        Assert.Equal("Customers", model.EntityContainer.Elements.Single().Name);
        Assert.Equal("NS1.Customer", model.SchemaElements.Single(e => e.FullName() == "NS1.Customer").FullName());
        Assert.Equal("CustomerID", ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer")).DeclaredStructuralProperties().Single().Name);
    }

    [Fact]
    public void Parse_NonNullableCollectionProperty_ParsesAndValidatesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        Assert.Equal("C1", model.EntityContainer.Name);
        Assert.Equal("Customers", model.EntityContainer.Elements.Single().Name);
        Assert.Equal("NS1.Customer", model.SchemaElements.Single(e => e.FullName() == "NS1.Customer").FullName());
        Assert.Equal(
            "CustomerID",
            ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer")).DeclaredStructuralProperties().First().Name);
        Assert.Equal(
            "Names",
            ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer")).DeclaredStructuralProperties().Last().Name);

        IEdmEntityType customer = ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer"));
        IEdmStructuralProperty names = customer.DeclaredStructuralProperties().Last();
        IEdmTypeReference typeRef = names.Type;
        Assert.False(typeRef.IsNullable);

        var type = typeRef.Definition as IEdmCollectionType;
        Assert.NotNull(type);
        Assert.Equal(EdmTypeKind.Collection, type.TypeKind);
        IEdmTypeReference elementType = type.ElementType;
        Assert.False(elementType.IsNullable);
    }

    [Fact]
    public void Parse_NullableCollectionProperty_ParsesAndValidatesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        Assert.Equal("C1", model.EntityContainer.Name);
        Assert.Equal("Customers", model.EntityContainer.Elements.Single().Name);
        Assert.Equal("NS1.Customer", model.SchemaElements.Single(e => e.FullName() == "NS1.Customer").FullName());
        Assert.Equal(
            "CustomerID",
            ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer")).DeclaredStructuralProperties().First().Name);
        Assert.Equal(
            "Names",
            ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer")).DeclaredStructuralProperties().Last().Name);

        IEdmEntityType customer = ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer"));
        IEdmStructuralProperty names = customer.DeclaredStructuralProperties().Last();
        var typeRef = names.Type as IEdmCollectionTypeReference;
        Assert.NotNull(typeRef);
        Assert.True(typeRef.IsNullable);

        var type = typeRef.Definition as IEdmCollectionType;
        Assert.NotNull(type);
        Assert.Equal(EdmTypeKind.Collection, type.TypeKind);
        IEdmTypeReference elementType = type.ElementType;
        Assert.True(elementType.IsNullable);
    }

    [Fact]
    public void Parse_CollectionNavigationProperty_ParsesAndValidatesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        bool result = model.Validate(out errors);
        Assert.True(result);
        Assert.Empty(errors);

        IEdmEntityType customer = ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer"));
        var pets = customer.DeclaredNavigationProperties().Single();
        IEdmTypeReference typeRef = pets.Type;
        Assert.True(typeRef.IsNullable);

        var type = typeRef.Definition as IEdmCollectionType;
        Assert.NotNull(type);
        Assert.Equal(EdmTypeKind.Collection, type.TypeKind);
        IEdmTypeReference elementType = type.ElementType;
        Assert.False(elementType.IsNullable);
    }

    [Fact]
    public void Parse_NullableCollectionNavigationProperty_ReportsValidationError()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        bool result = model.Validate(out errors);
        Assert.False(result);
        Assert.Single(errors);
        Assert.Equal(EdmErrorCode.NavigationPropertyWithCollectionTypeCannotHaveNullableAttribute, errors.First().ErrorCode);
        Assert.Equal("The 'Nullable' attribute cannot be specified for a navigation property with collection type.", errors.First().ErrorMessage);
    }

    [Fact]
    public void Parse_NonNullableCollectionNavigationProperty_ReportsValidationError()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        bool result = model.Validate(out errors);
        Assert.False(result);
        Assert.Single(errors);
        Assert.Equal(EdmErrorCode.NavigationPropertyWithCollectionTypeCannotHaveNullableAttribute, errors.First().ErrorCode);
        Assert.Equal("The 'Nullable' attribute cannot be specified for a navigation property with collection type.", errors.First().ErrorMessage);
    }

    [Fact]
    public void Parse_EntityContainerWithNavigationBindings_ParsesAndValidatesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmEntityContainer wild = model.EntityContainer;

        Assert.Equal("Wild", wild.Name);

        Assert.Equal(2, wild.Elements.Count());
        Assert.Equal("Pets", wild.Elements.First().Name);
        Assert.Equal(EdmContainerElementKind.EntitySet, wild.Elements.First().ContainerElementKind);
        Assert.Equal("Hot.Pet", ((IEdmEntitySet)wild.Elements.First()).EntityType.FullName());

        Assert.Equal("People", wild.Elements.ElementAt(1).Name);
        Assert.Equal(EdmContainerElementKind.EntitySet, wild.Elements.ElementAt(1).ContainerElementKind);
        Assert.Equal("Hot.Person", ((IEdmEntitySet)wild.Elements.ElementAt(1)).EntityType.FullName());

        IEdmEntityType person = (IEdmEntityType)model.FindType("Hot.Person");
        IEdmEntityType pet = (IEdmEntityType)model.FindType("Hot.Pet");
        Assert.NotNull(person);
        Assert.NotNull(pet);

        IEdmEntitySet wildpeople = wild.FindEntitySet("People");
        IEdmEntitySet wildpets = wild.FindEntitySet("Pets");

        Assert.Equal("Hot.Wild", wildpeople.Container.FullName());
        Assert.Equal("Hot.Wild", wildpets.Container.FullName());
    }

    [Fact]
    public void Parse_ContainedEntityWithNonContainedEntity_ParsesAndValidatesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);
        bool validated = model.Validate(out errors);
        Assert.True(validated);

        IEdmEntityContainer container = model.EntityContainer;
        IEdmEntitySet peopleEntitySet = container.FindEntitySet("People");
        IEdmEntityType petEntityType = (IEdmEntityType)model.FindType("Hot.Pet");
        var masterNavigationProperty = petEntityType.FindProperty("Master") as IEdmNavigationProperty;
        Assert.NotNull(masterNavigationProperty);

        IEdmNavigationSource navigationSource = peopleEntitySet.FindNavigationTarget(masterNavigationProperty, new EdmPathExpression("Pet/Master"));
        Assert.Equal(navigationSource, peopleEntitySet);
        IEdmEntityType peopleEntityType = (IEdmEntityType)model.FindType("Hot.Person");
        var petNavigationProperty = peopleEntityType.FindProperty("Pet") as IEdmNavigationProperty;
        IEdmNavigationSource containedNavigationSource = peopleEntitySet.FindNavigationTarget(petNavigationProperty);
        Assert.True(containedNavigationSource is IEdmContainedEntitySet);
        IEdmNavigationSource peopleNavigationSource = containedNavigationSource.FindNavigationTarget(masterNavigationProperty);
        Assert.Equal(peopleNavigationSource, navigationSource);
    }

    [Fact]
    public void Parse_ContainedEntityWithInvalidNavigationPath_ReportsValidationError()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);
        bool validated = model.Validate(out errors);

        Assert.False(validated);
        Assert.Single(errors);
        Assert.Equal(EdmErrorCode.BadUnresolvedNavigationPropertyPath, errors.First().ErrorCode);
        Assert.Equal("A navigation property could not be found for the path 'Pet/Master1/Master' starting from the type 'Hot.Pet'.", errors.First().ErrorMessage);
    }

    [Fact]
    public void Parse_ContainedEntityWithDerivedEntity_ParsesAndValidatesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);
        bool validated = model.Validate(out errors);

        Assert.True(validated, "validated");

        IEdmEntityContainer container = model.EntityContainer;
        IEdmEntitySet peopleEntitySet = container.FindEntitySet("People");
        IEdmEntityType dogEntityType = (IEdmEntityType)model.FindType("Hot.Dog");
        var masterNavigationProperty = dogEntityType.FindProperty("Master") as IEdmNavigationProperty;
        Assert.NotNull(masterNavigationProperty);
        IEdmNavigationSource navigationSource = peopleEntitySet.FindNavigationTarget(masterNavigationProperty, new EdmPathExpression("Pet/Hot.Dog/Master"));
        Assert.Equal(navigationSource, peopleEntitySet);
    }

    [Fact]
    public void Parse_SingletonEntityContainer_ParsesAndValidatesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmEntityContainer wild = model.EntityContainer;

        Assert.Equal("AnotherSingletonPeople", wild.Elements.ElementAt(1).Name);
        Assert.Equal(EdmContainerElementKind.Singleton, wild.Elements.ElementAt(1).ContainerElementKind);
        Assert.Equal("Hot.Person", ((IEdmSingleton)wild.Elements.ElementAt(1)).EntityType.FullName());

        IEdmEntityType person = (IEdmEntityType)model.FindType("Hot.Person");
        IEdmEntityType pet = (IEdmEntityType)model.FindType("Hot.Pet");
        IEdmSingleton singletonPeople = wild.FindSingleton("SingletonPeople");
        Assert.NotNull(singletonPeople);
        Assert.Equal(person, singletonPeople.EntityType);
        IEdmSingleton anotherPeople = wild.FindSingleton("AnotherSingletonPeople");
        Assert.NotNull(anotherPeople);
        Assert.Equal(person, singletonPeople.Type);
        IEdmEntitySet entitySetPeople = wild.FindEntitySet("SingletonPeople");
        Assert.Null(entitySetPeople);
        IEdmEntitySet pets = wild.FindEntitySet("Pets");
        Assert.NotNull(pets);

        Assert.Equal("Pets", singletonPeople.FindNavigationTarget(person.NavigationProperties().First()).Name);
        Assert.Equal("SingletonPeople", pets.FindNavigationTarget(pet.NavigationProperties().First()).Name);

        Assert.Equal("SingletonPeople", singletonPeople.Name);
    }

    [Fact]
    public void Parse_AssociationAndNavigationProperty_ParsesAndValidatesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        // Test the structural objects.
        Assert.True(model.SchemaElements.Count() == 2);

        IEdmEntityType type1 = (IEdmEntityType)model.SchemaElements.First();

        Assert.Equal("Feckless", type1.Name);
        Assert.Equal(3, type1.DeclaredStructuralProperties().Count());
        Assert.Equal(2, type1.DeclaredKey.Count());
        Assert.Equal("Id", type1.DeclaredKey.First().Name);
        Assert.Single(type1.DeclaredNavigationProperties());
        var nav1 = type1.DeclaredNavigationProperties().First();
        Assert.Equal("MyReckless", nav1.Name);
        Assert.False(nav1.ContainsTarget);

        IEdmEntityType type2 = (IEdmEntityType)model.SchemaElements.ElementAt(1);

        Assert.Equal("Reckless", type2.Name);
        Assert.Equal(2, type2.DeclaredStructuralProperties().Count());
        Assert.Equal(2, type2.DeclaredKey.Count());
        Assert.Equal("Id", type2.DeclaredKey.First().Name);
        Assert.Single(type2.DeclaredNavigationProperties());
        var nav2 = type2.DeclaredNavigationProperties().First();
        Assert.True(nav2.Type.IsNullable);
        Assert.Equal("MyFecklesses", nav2.Name);
        Assert.True(nav2.ContainsTarget);

        // Test the semantic objects.

        IEdmEntityType feckless = (IEdmEntityType)model.FindType("Cold.Feckless");
        IEdmEntityType reckless = (IEdmEntityType)model.FindType("Cold.Reckless");

        var toReckless = (IEdmNavigationProperty)feckless.FindProperty("MyReckless");
        var toFecklesses = (IEdmNavigationProperty)reckless.FindProperty("MyFecklesses");

        Assert.Equal(toFecklesses, toReckless.Partner);
        Assert.Equal(feckless, toReckless.DeclaringEntityType());
        Assert.Equal(reckless, toReckless.ToEntityType());

        Assert.Equal(toReckless, toFecklesses.Partner);
        Assert.Equal(reckless, toFecklesses.DeclaringEntityType());
        Assert.Equal(feckless, toFecklesses.ToEntityType());

        Assert.True(toFecklesses.IsPrincipal());
        Assert.Null(toFecklesses.DependentProperties());
        Assert.False(toReckless.IsPrincipal());
        Assert.NotNull(toReckless.DependentProperties());

        Assert.Equal(2, toReckless.DependentProperties().Count());
        Assert.Equal(toReckless.DependentProperties().First(), feckless.DeclaredKey.First());
        Assert.Equal(toReckless.DependentProperties().Last(), feckless.DeclaredKey.Last());

        Assert.Equal(EdmOnDeleteAction.Cascade, toReckless.OnDelete);
        Assert.Equal(EdmOnDeleteAction.None, toFecklesses.OnDelete);

        Assert.Equal(EdmMultiplicity.Many, toFecklesses.TargetMultiplicity());
        Assert.Equal(EdmMultiplicity.ZeroOrOne, toReckless.TargetMultiplicity());
        Assert.Equal(EdmMultiplicity.ZeroOrOne, toReckless.TargetMultiplicity());
    }

    [Fact]
    public void Parse_SelfReferencingNavigationProperty_ParsesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);
    }

    [Fact]
    public void Parse_SelfReferencingOneToManyNavigationProperty_ParsesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);
    }

    [Fact]
    public void Parse_SimpleComplexTypesModel_ParsesAndValidatesSuccessfully()
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


        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl1)), XmlReader.Create(new StringReader(csdl2)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        // Test the structural objects.
        Assert.Equal(2, model.SchemaElements.Count());
        IEdmComplexType type1 = (IEdmComplexType)model.SchemaElements.First();
        IEdmComplexType type2 = (IEdmComplexType)model.SchemaElements.Last();
        Assert.Equal("Smod", type1.Name);
        Assert.Equal("Id", type1.DeclaredStructuralProperties().First().Name);
        IEdmPrimitiveTypeReference idType = type1.DeclaredStructuralProperties().First().Type.AsPrimitive();
        Assert.Equal("Int32", idType.PrimitiveDefinition().Name);
        Assert.False(idType.IsNullable);


        Assert.Equal("Clod", type2.Name);
        Assert.Equal(2, type2.DeclaredStructuralProperties().Count());
        Assert.Equal("Address", type2.DeclaredStructuralProperties().Last().Name);
        IEdmStringTypeReference addressType = type2.DeclaredStructuralProperties().Last().Type.AsPrimitive().AsString();
        Assert.Equal("String", addressType.PrimitiveDefinition().Name);
        Assert.Equal(2048, addressType.MaxLength);
        Assert.True(addressType.IsNullable);

        // Test the semantic objects.

        IEdmComplexType smod = (IEdmComplexType)model.FindType("Grumble.Smod");
        IEdmComplexType clod = (IEdmComplexType)model.FindType("Mumble.Clod");

        Assert.Equal("Smod", smod.Name);
        Assert.Equal("Grumble", smod.Namespace);
        Assert.Equal("Clod", clod.Name);
        Assert.Equal("Mumble", clod.Namespace);

        Assert.Equal(smod, clod.BaseType);

        Assert.Equal(3, clod.StructuralProperties().Count());
        Assert.Equal(clod.StructuralProperties().First(), smod.StructuralProperties().First());
        IEdmProperty address = clod.FindProperty("Address");
        Assert.Equal("Address", address.Name);
        Assert.Equal(address, clod.StructuralProperties().Last());

        IEdmProperty id = clod.FindProperty("Id");
        IEdmPrimitiveTypeReference resolvedIdType = id.Type.AsPrimitive();
        Assert.Equal(EdmPrimitiveTypeKind.Int32, resolvedIdType.PrimitiveKind());
    }

    [Fact]
    public void Parse_SimpleComplexTypesModelWithoutUsingAlias_ParsesAndValidatesSuccessfully()
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


        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl1)), XmlReader.Create(new StringReader(csdl2)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        // Test the structural objects.
        Assert.Equal(2, model.SchemaElements.Count());
        IEdmComplexType type1 = (IEdmComplexType)model.SchemaElements.First();
        IEdmComplexType type2 = (IEdmComplexType)model.SchemaElements.Last();
        Assert.Equal("Smod", type1.Name);
        Assert.Equal("Id", type1.DeclaredStructuralProperties().First().Name);
        IEdmPrimitiveTypeReference idType = type1.DeclaredStructuralProperties().First().Type.AsPrimitive();
        Assert.Equal("Int32", idType.PrimitiveDefinition().Name);
        Assert.False(idType.IsNullable);


        Assert.Equal("Clod", type2.Name);
        Assert.Equal(2, type2.DeclaredStructuralProperties().Count());
        Assert.Equal("Address", type2.DeclaredStructuralProperties().Last().Name);
        IEdmStringTypeReference addressType = type2.DeclaredStructuralProperties().Last().Type.AsPrimitive().AsString();
        Assert.Equal("String", addressType.PrimitiveDefinition().Name);
        Assert.Equal(2048, addressType.MaxLength);
        Assert.True(addressType.IsNullable);

        // Test the semantic objects.

        IEdmComplexType smod = (IEdmComplexType)model.FindType("Grumble.Smod");
        IEdmComplexType clod = (IEdmComplexType)model.FindType("Mumble.Clod");

        Assert.Equal("Smod", smod.Name);
        Assert.Equal("Grumble", smod.Namespace);
        Assert.Equal("Clod", clod.Name);
        Assert.Equal("Mumble", clod.Namespace);

        Assert.Equal(smod, clod.BaseType);

        Assert.Equal(3, clod.StructuralProperties().Count());
        Assert.Equal(clod.StructuralProperties().First(), smod.StructuralProperties().First());
        IEdmProperty address = clod.FindProperty("Address");
        Assert.Equal("Address", address.Name);
        Assert.Equal(address, clod.StructuralProperties().Last());

        IEdmProperty id = clod.FindProperty("Id");
        IEdmPrimitiveTypeReference resolvedIdType = id.Type.AsPrimitive();
        Assert.Equal(EdmPrimitiveTypeKind.Int32, resolvedIdType.PrimitiveKind());
    }

    [Fact]
    public void Parse_ComplexTypesInheritanceModel_ParsesAndValidatesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl1)), XmlReader.Create(new StringReader(csdl2)), XmlReader.Create(new StringReader(csdl3)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        //Assert.True(parsed);
        Assert.Empty(errors);

        // Test the structural objects.
        Assert.Equal(7, model.SchemaElements.Count());
        IEdmComplexType type1 = (IEdmComplexType)model.SchemaElements.First();
        IEdmComplexType type2 = (IEdmComplexType)model.SchemaElements.ElementAt(1);
        Assert.Equal("MyComplex1", type1.Name);
        Assert.Equal("Id", type2.DeclaredStructuralProperties().First().Name);

        IEdmPrimitiveTypeReference idType = type2.DeclaredStructuralProperties().First().Type.AsPrimitive();
        Assert.Equal("Int32", idType.PrimitiveDefinition().Name);
        Assert.False(idType.IsNullable);

        // Test the semantic objects.
        IEdmComplexType type2_1 = (IEdmComplexType)model.FindType("NameSpace2.MyComplex1");
        IEdmComplexType type2_2 = (IEdmComplexType)model.FindType("NameSpace2.MyComplex2");
        IEdmComplexType type2_3 = (IEdmComplexType)model.FindType("NameSpace2.MyComplex3");

        Assert.Equal("MyComplex1", type2_1.Name);
        Assert.Equal("MyComplex2", type2_2.Name);
        Assert.Equal("MyComplex3", type2_3.Name);

        Assert.Equal(type2_1, type2_2.BaseType);
        Assert.Equal(type2_2, type2_3.BaseType);
    }

    [Fact]
    public void Parse_SimpleEntityTypesModel_ParsesAndValidatesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl1)), XmlReader.Create(new StringReader(csdl2)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);
        // Test the structural objects.

        IEdmEntityType type1 = (IEdmEntityType)model.SchemaElements.First();
        IEdmEntityType type2 = (IEdmEntityType)model.SchemaElements.Last();
        Assert.Equal("Smod", type1.Name);
        Assert.Single(type1.DeclaredKey);
        Assert.Equal("Id", type1.DeclaredKey.First().Name);
        Assert.Equal("Id", type1.DeclaredStructuralProperties().First().Name);

        IEdmPrimitiveTypeReference idType = type1.DeclaredStructuralProperties().First().Type.AsPrimitive();
        Assert.Equal(EdmPrimitiveTypeKind.Int32, idType.PrimitiveKind());
        Assert.False(idType.IsNullable);
        Assert.Equal("Clod", type2.Name);
        Assert.Equal("Grumble.Smod", type2.BaseEntityType().FullName());
        Assert.Equal(2, type2.DeclaredStructuralProperties().Count());
        Assert.Equal("Address", type2.StructuralProperties().Last().Name);

        IEdmStringTypeReference addressType = type2.StructuralProperties().Last().Type.AsPrimitive().AsString();
        Assert.Equal(addressType.PrimitiveKind(), EdmPrimitiveTypeKind.String);
        Assert.Equal(2048, addressType.MaxLength);
        Assert.True(addressType.IsNullable);

        // Test the semantic objects.

        IEdmEntityType smod = (IEdmEntityType)model.FindType("Grumble.Smod");
        IEdmEntityType smod2 = (IEdmEntityType)model.FindType("Grumble.Smod2");
        IEdmEntityType smod3 = (IEdmEntityType)model.FindType("Grumble.Smod3");
        IEdmEntityType clod = (IEdmEntityType)model.FindType("Mumble.Clod");

        Assert.Equal("Smod", smod.Name);
        Assert.Equal("Clod", clod.Name);

        Assert.False(smod.IsOpen);
        Assert.True(smod2.IsOpen);
        Assert.False(smod3.IsOpen);

        Assert.Equal(smod, clod.BaseType);

        Assert.Equal(3, clod.StructuralProperties().Count());
        Assert.Equal(clod.StructuralProperties().First(), smod.StructuralProperties().First());
        IEdmProperty address = clod.FindProperty("Address");
        Assert.Equal("Address", address.Name);
        Assert.Equal(address, clod.StructuralProperties().Last());

        IEdmProperty id = clod.FindProperty("Id");
        IEdmPrimitiveTypeReference resolvedIdType = id.Type.AsPrimitive();
        Assert.Equal(EdmPrimitiveTypeKind.Int32, resolvedIdType.PrimitiveKind());

        Assert.Equal(1, smod.DeclaredKey.Count());
        Assert.Equal(id, smod.DeclaredKey.First());
        Assert.Equal(smod.DeclaredKey.First(), clod.Key().First());
    }

    [Fact]
    public void Parse_StreamPropertiesInComplexType_ParsesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmComplexType smodDef = (IEdmComplexType)model.SchemaElements.First();
        IEdmStructuralProperty streamProp = (IEdmStructuralProperty)smodDef.FindProperty("Stream");
        Assert.Equal(EdmTypeKind.Primitive, streamProp.Type.TypeKind());
        Assert.Equal(EdmPrimitiveTypeKind.Stream, streamProp.Type.AsPrimitive().PrimitiveKind());
        IEdmStructuralProperty streamProp1 = (IEdmStructuralProperty)smodDef.FindProperty("Stream1");
        IEdmStructuralProperty streamProp2 = (IEdmStructuralProperty)smodDef.FindProperty("Stream2");
        Assert.Equal(EdmTypeKind.Primitive, streamProp1.Type.TypeKind());
        Assert.Equal(EdmPrimitiveTypeKind.Stream, streamProp1.Type.AsPrimitive().PrimitiveKind());
        Assert.Equal(EdmTypeKind.Primitive, streamProp2.Type.TypeKind());
        Assert.Equal(EdmPrimitiveTypeKind.Stream, streamProp2.Type.AsPrimitive().PrimitiveKind());
    }

    [Fact]
    public void Parse_StringAttributeAnnotation_ParsesAndValidatesSuccessfully()
    {
        const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"" xmlns:p=""p"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" p:foo=""Quoth the Raven"" />
  </ComplexType>
</Schema>";

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        // Test the structural objects.
        Assert.Single(model.SchemaElements);
        IEdmComplexType type = (IEdmComplexType)model.SchemaElements.First();
        Assert.Equal("Smod", type.Name);
        Assert.Equal("Id", type.DeclaredStructuralProperties().First().Name);
        IEdmPrimitiveTypeReference idType = type.DeclaredStructuralProperties().First().Type.AsPrimitive();
        Assert.Equal("Int32", idType.PrimitiveDefinition().Name);
        Assert.False(idType.IsNullable);

        // Test the Annotation
        IEdmValue annotationValue = model.GetAnnotationValue<IEdmValue>(type.DeclaredStructuralProperties().First(), "p", "foo");
        IEdmDirectValueAnnotation annotation = (IEdmDirectValueAnnotation)model.DirectValueAnnotations(type.DeclaredStructuralProperties().First()).First();
        Assert.Equal(annotationValue, annotation.Value);
        Assert.Equal("p", annotation.NamespaceUri);
        Assert.Equal("foo", annotation.Name);
        Assert.Equal(EdmPrimitiveTypeKind.String, annotationValue.Type.AsPrimitive().PrimitiveKind());
        Assert.Equal("Quoth the Raven", ((IEdmStringValue)annotationValue).Value);
    }

    [Fact]
    public void Parse_StringElementAnnotation_ParsesAndValidatesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        // Test the structural objects.
        Assert.Single(model.SchemaElements);
        IEdmComplexType type = (IEdmComplexType)model.SchemaElements.First();
        Assert.Equal("Smod", type.Name);
        Assert.Equal("Id", type.DeclaredStructuralProperties().First().Name);
        IEdmPrimitiveTypeReference idType = type.DeclaredStructuralProperties().First().Type.AsPrimitive();
        Assert.Equal("Int32", idType.PrimitiveDefinition().Name);
        Assert.False(idType.IsNullable);

        // Test the Annotation
        IEdmValue annotationValue = model.GetAnnotationValue<IEdmValue>(type.DeclaredStructuralProperties().First(), "p", "foo");
        IEdmDirectValueAnnotation annotation = (IEdmDirectValueAnnotation)model.DirectValueAnnotations(type.DeclaredStructuralProperties().First()).First();
        Assert.Equal(annotationValue, annotation.Value);
        Assert.Equal("p", annotation.NamespaceUri);
        Assert.Equal("foo", annotation.Name);
        Assert.Equal(EdmPrimitiveTypeKind.String, annotationValue.Type.AsPrimitive().PrimitiveKind());
        //TODO: handle complex annotations properly
        Assert.Equal("<p:foo xmlns:p=\"p\">\n           Quoth the Raven\n        </p:foo>", ((IEdmStringValue)annotationValue).Value);
    }

    [Fact]
    public void Parse_EmptyStringElementAnnotation_ParsesAndValidatesSuccessfully()
    {
        const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"" xmlns:p=""p"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" ><p:foo /></Property>
  </ComplexType>
</Schema>";

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        // Test the structural objects.
        Assert.Single(model.SchemaElements);
        IEdmComplexType type = (IEdmComplexType)model.SchemaElements.First();
        Assert.Equal("Smod", type.Name);
        Assert.Equal("Id", type.DeclaredStructuralProperties().First().Name);
        IEdmPrimitiveTypeReference idType = type.DeclaredStructuralProperties().First().Type.AsPrimitive();
        Assert.Equal("Int32", idType.PrimitiveDefinition().Name);
        Assert.False(idType.IsNullable);

        // Test the Annotation
        IEdmValue annotationValue = model.GetAnnotationValue<IEdmValue>(type.DeclaredStructuralProperties().First(), "p", "foo");
        IEdmDirectValueAnnotation annotation = (IEdmDirectValueAnnotation)model.DirectValueAnnotations(type.DeclaredStructuralProperties().First()).First();
        Assert.Equal(annotationValue, annotation.Value);
        Assert.Equal("p", annotation.NamespaceUri);
        Assert.Equal("foo", annotation.Name);
        Assert.Equal(EdmPrimitiveTypeKind.String, annotationValue.Type.AsPrimitive().PrimitiveKind());
        Assert.Equal(@"<p:foo xmlns:p=""p"" />", ((IEdmStringValue)annotationValue).Value);
    }

    [Fact]
    public void Remove_ImmutableAnnotations_UpdatesAnnotationsCorrectly()
    {
        const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"" xmlns:p=""p"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" p:foo=""Quoth the Raven"" p:bar=""Nevermore"" p:baz=""Right Phil?"" />
  </ComplexType>
</Schema>";

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmComplexType type = (IEdmComplexType)model.SchemaElements.First();
        IEdmStructuralProperty id = type.DeclaredStructuralProperties().First();

        Assert.Equal(3, model.DirectValueAnnotations(id).Count());

        IEdmValue annotationValue = model.GetAnnotationValue<IEdmValue>(id, "p", "foo");
        Assert.Equal("Quoth the Raven", ((IEdmStringValue)annotationValue).Value);

        annotationValue = model.GetAnnotationValue<IEdmValue>(id, "p", "bar");
        Assert.Equal("Nevermore", ((IEdmStringValue)annotationValue).Value);

        annotationValue = model.GetAnnotationValue<IEdmValue>(id, "p", "baz");
        Assert.Equal("Right Phil?", ((IEdmStringValue)annotationValue).Value);

        // Remove the "foo" annotation
        model.SetAnnotationValue(id, "p", "foo", null);
        model.SetAnnotationValue(id, "p", "baz", "Sure Bobby");

        Assert.Equal(2, model.DirectValueAnnotations(id).Count());
        annotationValue = model.GetAnnotationValue<IEdmValue>(id, "p", "foo");
        Assert.Null(annotationValue);

        annotationValue = model.GetAnnotationValue<IEdmValue>(id, "p", "bar");
        Assert.Equal("Nevermore", ((IEdmStringValue)annotationValue).Value);
        Assert.Equal("Sure Bobby", model.GetAnnotationValue<string>(id, "p", "baz"));
    }

    [Fact]
    public void Parse_InlineCollectionProperty_ParsesAndValidatesSuccessfully()
    {
        const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Int32""/>
    <Property Name=""Collection"" Type=""Collection(Int32)""/>
  </ComplexType>
</Schema>";

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmComplexType smodDef = (IEdmComplexType)model.SchemaElements.First();
        IEdmStructuralProperty mvProp = (IEdmStructuralProperty)smodDef.FindProperty("Collection");
        Assert.Equal(EdmTypeKind.Collection, mvProp.Type.TypeKind());
        Assert.Equal(EdmTypeKind.Primitive, mvProp.Type.AsCollection().ElementType().TypeKind());
        Assert.Equal(EdmPrimitiveTypeKind.Int32, mvProp.Type.AsCollection().ElementType().AsPrimitive().PrimitiveKind());
    }

    [Fact]
    public void Parse_EmptyNullableFacet_ParsesSuccessfully()
    {
        const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Int32""/>
  </ComplexType>
</Schema>";

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmComplexType smodDef = (IEdmComplexType)model.SchemaElements.First();
        IEdmStructuralProperty idProp = (IEdmStructuralProperty)smodDef.FindProperty("Id");
        Assert.True(idProp.Type.IsNullable);

    }

    [Fact]
    public void Verify_FindPropertyInBaseType_ReturnsExpectedResults()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmEntityType inheritingEntityType = (IEdmEntityType)model.SchemaElements.Last();
        IEdmStructuralProperty baseProperty = (IEdmStructuralProperty)inheritingEntityType.FindProperty("Property0");
        Assert.Equal(baseProperty, inheritingEntityType.StructuralProperties().First());
    }

    [Fact]
    public void Verify_UnicodeDefaultsToTrue_ParsesSuccessfully()
    {
        const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""myString"" Type=""String""/>
  </ComplexType>
</Schema>";

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmComplexType typeDefinition = (IEdmComplexType)model.SchemaElements.First();
        IEdmStructuralProperty stringProp = (IEdmStructuralProperty)typeDefinition.FindProperty("myString");
        Assert.Equal(true, stringProp.Type.AsString().IsUnicode);
    }

    #region Operations and OperationImports

    [Fact]
    public void Parse_SimpleOperation_ParsesAndValidatesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmEntityType personType = (IEdmEntityType)model.SchemaElements.First();
        IEdmOperation getAge = (IEdmOperation)model.FindOperations("foo.GetAge").First();
        Assert.Equal("GetAge", getAge.Name);
        Assert.NotNull(getAge);
        Assert.Equal("GetAge", getAge.Name);
        Assert.Equal("foo", getAge.Namespace);
        Assert.Equal(EdmPrimitiveTypeKind.Int32, getAge.ReturnType.AsPrimitive().PrimitiveKind());
        Assert.Equal(EdmSchemaElementKind.Function, getAge.SchemaElementKind);

        IEdmOperationParameter getAgeParameter = getAge.Parameters.First();
        Assert.Equal(getAgeParameter, getAge.FindParameter("Person"));
        Assert.Equal("Person", getAgeParameter.Name);
        Assert.Equal(personType, getAgeParameter.Type.Definition);
    }

    [Fact]
    public void Parse_FunctionWithDuplicateParameterNames_ReportsValidationError()
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
        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        // validation should detect duplicated parameter names.
        model.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.Equal("Each parameter name in a operation must be unique. The parameter name 'PersonParam' is already defined.", validationErrors.Single().ErrorMessage);

        // FindParameter() should throw exception.
        IEdmEntityType personType = (IEdmEntityType)model.SchemaElements.First();
        IEdmOperation getAge = (IEdmOperation)model.FindOperations("foo.GetAge").First();

        var exception = Assert.Throws<InvalidOperationException>(() => getAge.FindParameter("PersonParam"));
        Assert.Equal("Sequence contains more than one matching element", exception.Message);
    }

    [Fact]
    public void Parse_OperationWithNullableParameterAndReturnType_ParsesSuccessfully()
    {
        const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Function Name=""Foo"">
    <ReturnType Type=""Int32"" Nullable=""false"" />
    <Parameter Name=""Param1"" Type=""Int32"" Nullable=""false"" />
  </Function>
</Schema>";

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);
    }

    [Fact]
    public void Parse_SimpleOperationImport_ParsesAndValidatesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmEntityType personType = (IEdmEntityType)model.SchemaElements.First();
        IEdmEntityContainer fooContainer = (IEdmEntityContainer)model.FindEntityContainer("fooContainer");
        IEdmEntitySet peopleSet = (IEdmEntitySet)fooContainer.Elements.First();

        var fiGroup = fooContainer.FindOperationImports("peopleWhoAreAwesomeAction").ToArray();
        Assert.Equal(4, fiGroup.Length);

        IEdmOperationImport peopleWhoAreAwesome = fiGroup.First();
        Assert.Equal(EdmContainerElementKind.ActionImport, peopleWhoAreAwesome.ContainerElementKind);

        Assert.True(peopleWhoAreAwesome.TryGetStaticEntitySet(model, out IEdmEntitySetBase eset), "peopleWhoAreAwesome.TryGetStaticEntitySet");
        Assert.Equal(peopleSet, eset);

        Assert.True(peopleWhoAreAwesome.TryGetStaticEntitySet(model, out IEdmEntitySetBase fiEntitySet), "peopleWhoAreAwesome.TryGetStaticEntitySet");
        Assert.Equal(personType, fiEntitySet.EntityType);
        Assert.Equal("peopleWhoAreAwesomeAction", peopleWhoAreAwesome.Name);
        Assert.Equal(EdmTypeKind.Collection, peopleWhoAreAwesome.Operation.ReturnType.Definition.TypeKind);
        Assert.Equal(personType, peopleWhoAreAwesome.Operation.ReturnType.AsCollection().ElementType().Definition);

        IEdmOperationParameter peopleWhoAreAwesomeParameter = peopleWhoAreAwesome.Operation.Parameters.First();
        Assert.Equal(peopleWhoAreAwesomeParameter, peopleWhoAreAwesome.Operation.FindParameter("awesomeName"));
        Assert.Equal("awesomeName", peopleWhoAreAwesomeParameter.Name);
        Assert.False(peopleWhoAreAwesomeParameter.Type.AsString().IsBad());

        Assert.True(fiGroup[0] is IEdmActionImport);
        Assert.True(fiGroup[1] is IEdmActionImport);
        Assert.True(fiGroup[2] is IEdmActionImport);
        Assert.True(fiGroup[3] is IEdmActionImport);

        fiGroup = fooContainer.FindOperationImports("peopleWhoAreAwesomeFunction").ToArray();
        Assert.Equal(3, fiGroup.Length);
        Assert.True(fiGroup[0] is IEdmFunctionImport);
        Assert.True(fiGroup[1] is IEdmFunctionImport);
        Assert.True(fiGroup[2] is IEdmFunctionImport);
    }

    // BuildInternalUniqueParameterTypeFunctionString is not including the type name for collection of entities
    [Fact]
    public void Parse_OperationImportWithParameterAndReturnTypes_ParsesAndValidatesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed && !errors.Any());

        var validated = EdmValidator.Validate(model, EdmConstants.EdmVersionLatest, out errors);
        Assert.True(validated && !errors.Any());

        var people = model.EntityContainer.FindEntitySet("People");
        Assert.Equal("Person", people.EntityType.Name);
        Assert.False(((IEdmNavigationProperty)people.EntityType.FindProperty("Orders")).ContainsTarget);

        var operationImports = model.EntityContainer.OperationImports().ToArray();
        Assert.Equal(11, operationImports.Length);

        Assert.Equal(EdmTypeKind.Entity, operationImports[0].Operation.ReturnType.AsCollection().ElementType().TypeKind());
        Assert.Equal(EdmTypeKind.Entity, operationImports[0].Operation.Parameters.Single().Type.AsCollection().ElementType().TypeKind());

        Assert.True(operationImports[0].TryGetStaticEntitySet(model, out IEdmEntitySetBase eset), "operationImports[0].TryGetStaticEntitySet");
        Assert.False(operationImports[0].TryGetRelativeEntitySetPath(model, out IEdmOperationParameter p, out Dictionary<IEdmNavigationProperty, IEdmPathExpression> np, out IEnumerable<EdmError> entitySetPathErrors));
        Assert.Equal(people, eset);

        Assert.Equal(EdmTypeKind.Complex, operationImports[1].Operation.ReturnType.AsCollection().ElementType().TypeKind());
        Assert.Equal(EdmTypeKind.Complex, operationImports[1].Operation.Parameters.Single().Type.AsCollection().ElementType().TypeKind());

        Assert.Equal(EdmTypeKind.Primitive, operationImports[2].Operation.ReturnType.AsCollection().ElementType().TypeKind());
        Assert.Equal(EdmTypeKind.Primitive, operationImports[2].Operation.Parameters.Single().Type.AsCollection().ElementType().TypeKind());

        Assert.Equal(EdmTypeKind.Entity, operationImports[3].Operation.ReturnType.TypeKind());
        Assert.Equal(EdmTypeKind.Entity, operationImports[3].Operation.Parameters.Single().Type.TypeKind());

        Assert.True(operationImports[3].TryGetStaticEntitySet(model, out IEdmEntitySetBase eset2));
        Assert.False(operationImports[0].TryGetRelativeEntitySetPath(model, out p, out np, out entitySetPathErrors));
        Assert.Equal(people, eset2);

        Assert.Equal(EdmTypeKind.Complex, operationImports[4].Operation.ReturnType.TypeKind());
        Assert.Equal(EdmTypeKind.Complex, operationImports[4].Operation.Parameters.Single().Type.TypeKind());

        Assert.Equal(EdmTypeKind.Primitive, operationImports[5].Operation.ReturnType.TypeKind());
        Assert.Equal(EdmTypeKind.Primitive, operationImports[5].Operation.Parameters.Single().Type.TypeKind());

        Assert.False(operationImports[6].TryGetStaticEntitySet(model, out eset));
        Assert.False(operationImports[6].TryGetRelativeEntitySetPath(model, out p, out np, out entitySetPathErrors));
        Assert.Null(p);

        Assert.False(operationImports[7].TryGetStaticEntitySet(model, out eset));
        Assert.False(operationImports[7].TryGetRelativeEntitySetPath(model, out p, out np, out entitySetPathErrors));

        Assert.False(operationImports[8].TryGetStaticEntitySet(model, out eset));
        Assert.False(operationImports[8].TryGetRelativeEntitySetPath(model, out p, out np, out entitySetPathErrors));

        Assert.False(operationImports[9].TryGetStaticEntitySet(model, out eset));
        Assert.False(operationImports[9].TryGetRelativeEntitySetPath(model, out p, out np, out entitySetPathErrors));

        Assert.False(operationImports[10].TryGetStaticEntitySet(model, out eset));
        Assert.False(operationImports[10].TryGetRelativeEntitySetPath(model, out p, out np, out entitySetPathErrors));
    }

    #endregion

    [Fact]
    public void Verify_ReferentialConstraintOutOfOrder_ParsesAndValidatesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmEntityType entity1Def = (IEdmEntityType)model.FindType("Grumble.Entity1");
        IEdmEntityType entity2Def = (IEdmEntityType)model.FindType("Grumble.Entity2");

        Assert.True(((IEdmNavigationProperty)entity1Def.FindProperty("ToEntity2")).IsPrincipal(), "Correct principal end");
        Assert.Equal(entity2Def.FindProperty("Fk1"), ((IEdmNavigationProperty)entity1Def.FindProperty("ToEntity2")).Partner.DependentProperties().First());

        Assert.Equal("Entity1", entity1Def.Name);
        Assert.Equal("Entity2", entity2Def.Name);
    }

    [Fact]
    public void Verify_FacetNullability_ParsesAndValidatesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmComplexType typeDefinition = (IEdmComplexType)model.SchemaElements.First();
        IEdmStructuralProperty stringProp = (IEdmStructuralProperty)typeDefinition.FindProperty("myString");
        Assert.Null(stringProp.Type.AsString().MaxLength);
        IEdmStructuralProperty decimalProp = (IEdmStructuralProperty)typeDefinition.FindProperty("myDecimal");
        Assert.Null(decimalProp.Type.AsDecimal().Scale);
        Assert.Null(decimalProp.Type.AsDecimal().Precision);
        IEdmStructuralProperty datetimeProp = (IEdmStructuralProperty)typeDefinition.FindProperty("myDateTime");
        Assert.Equal(0, datetimeProp.Type.AsTemporal().Precision);
    }

    [Fact]
    public void Verify_FacetManuallySet_ParsesAndValidatesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmComplexType typeDefinition = (IEdmComplexType)model.SchemaElements.First();
        IEdmStructuralProperty stringProp = (IEdmStructuralProperty)typeDefinition.FindProperty("myString");
        Assert.Null(stringProp.Type.AsString().MaxLength);
        Assert.Equal(false, stringProp.Type.AsString().IsUnicode);

        IEdmStructuralProperty decimalProp = (IEdmStructuralProperty)typeDefinition.FindProperty("myDecimal");
        Assert.Null(decimalProp.Type.AsDecimal().Scale);
        Assert.Equal(3, decimalProp.Type.AsDecimal().Precision);

        IEdmStructuralProperty secondDecimalProp = (IEdmStructuralProperty)typeDefinition.FindProperty("mySecondDecimal");
        Assert.Null(secondDecimalProp.Type.AsDecimal().Scale);
        Assert.Equal(3, secondDecimalProp.Type.AsDecimal().Precision);

        IEdmStructuralProperty thirdDecimalProp = (IEdmStructuralProperty)typeDefinition.FindProperty("myThirdDecimal");
        Assert.Equal(0, thirdDecimalProp.Type.AsDecimal().Scale);
        Assert.Equal(3, thirdDecimalProp.Type.AsDecimal().Precision);

        IEdmStructuralProperty datetimeProp = (IEdmStructuralProperty)typeDefinition.FindProperty("myDateTime");
        Assert.Equal(1, datetimeProp.Type.AsTemporal().Precision);
    }

    [Fact]
    public void Parse_EntityReferenceShortcut_ParsesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> error);
        Assert.True(parsed);

        IEdmEntityType entityTypeDefinition = (IEdmEntityType)model.SchemaElements.First();
        IEdmComplexType complexTypeDefinition = (IEdmComplexType)model.SchemaElements.Last();
        IEdmStructuralProperty refProp = (IEdmStructuralProperty)complexTypeDefinition.FindProperty("myRef");

        Assert.Equal(EdmTypeKind.EntityReference, refProp.Type.TypeKind());
        Assert.Equal(entityTypeDefinition, refProp.Type.AsEntityReference().EntityReferenceDefinition().EntityType);
    }

    [Fact]
    public void Parse_EntityTypeWithCycle_ReportsValidationError()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmEntityType entityTypeDefinitionSmod = (IEdmEntityType)model.SchemaElements.First();
        IEdmEntityType entityTypeDefinitionClod = (IEdmEntityType)model.SchemaElements.Last();
        Assert.True(entityTypeDefinitionClod.BaseType.IsBad(), "Clod basetype is bad because of cycle");
        Assert.Equal(EdmErrorCode.BadCyclicEntity, entityTypeDefinitionClod.BaseType.Errors().First().ErrorCode);
        Assert.True(entityTypeDefinitionSmod.BaseType.IsBad(), "Smod basetype is bad because of cycle");

        EdmValidator.Validate(model, EdmConstants.EdmVersion4, out errors);
        Assert.Equal(2, errors.Count());
        Assert.Equal(EdmErrorCode.BadCyclicEntity, errors.ElementAt(0).ErrorCode);
        Assert.Equal(EdmErrorCode.BadCyclicEntity, errors.ElementAt(1).ErrorCode);
    }

    [Fact]
    public void Parse_EntityTypeWithThreeWayCycle_ReportsValidationError()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmEntityType entityTypeDefinitionSmod = (IEdmEntityType)model.SchemaElements.First();
        IEdmEntityType entityTypeDefinitionClod = (IEdmEntityType)model.SchemaElements.ElementAt(1);
        IEdmEntityType entityTypeDefinitionBlob = (IEdmEntityType)model.SchemaElements.Last();
        Assert.True(entityTypeDefinitionClod.BaseType.IsBad());
        Assert.Equal(EdmErrorCode.BadCyclicEntity, entityTypeDefinitionClod.BaseType.Errors().First().ErrorCode);
        Assert.True(entityTypeDefinitionSmod.BaseType.IsBad());
        Assert.True(entityTypeDefinitionBlob.BaseType.IsBad());

        EdmValidator.Validate(model, EdmConstants.EdmVersion4, out errors);
        Assert.Equal(3, errors.Count());
        Assert.All(errors, e => Assert.Equal(EdmErrorCode.BadCyclicEntity, e.ErrorCode));
        Assert.Equal("BadCyclicEntity : The entity 'Grumble.Smod' is invalid because its base type is cyclic. : (3, 4)", errors.ElementAt(0).ToString());
        Assert.Equal("BadCyclicEntity : The entity 'Grumble.Blob' is invalid because its base type is cyclic. : (10, 4)", errors.ElementAt(1).ToString());
        Assert.Equal("BadCyclicEntity : The entity 'Grumble.Clod' is invalid because its base type is cyclic. : (13, 4)", errors.ElementAt(2).ToString());
    }

    [Fact]
    public void Parse_EntityTypeWithUninvolvedNodes_ParsesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmEntityType entityTypeDefinitionSmod = (IEdmEntityType)model.SchemaElements.First();
        IEdmEntityType entityTypeDefinitionClod = (IEdmEntityType)model.SchemaElements.ElementAt(1);
        IEdmEntityType entityTypeDefinitionBlob = (IEdmEntityType)model.SchemaElements.Last();
        Assert.False(entityTypeDefinitionClod.BaseType.IsBad());
        Assert.True(entityTypeDefinitionBlob.BaseType.IsBad());
        Assert.Equal(EdmErrorCode.BadCyclicEntity, entityTypeDefinitionBlob.BaseType.Errors().First().ErrorCode);

        EdmValidator.Validate(model, EdmConstants.EdmVersion4, out errors);
        Assert.Single(errors);
        Assert.Equal(EdmErrorCode.BadCyclicEntity, errors.First().ErrorCode);
        Assert.Equal("BadCyclicEntity : The entity 'Grumble.Blob' is invalid because its base type is cyclic. : (13, 4)", errors.First().ToString());
    }

    [Fact]
    public void Parse_ComplexTypeWithCycle_ReportsValidationError()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmComplexType complexTypeDefinitionSmod = (IEdmComplexType)model.SchemaElements.First();
        IEdmComplexType complexTypeDefinitionClod = (IEdmComplexType)model.SchemaElements.Last();
        Assert.True(complexTypeDefinitionClod.BaseType.IsBad());
        Assert.Equal(EdmErrorCode.BadCyclicComplex, complexTypeDefinitionClod.BaseType.Errors().First().ErrorCode);
        Assert.True(complexTypeDefinitionSmod.BaseType.IsBad());

        EdmValidator.Validate(model, EdmConstants.EdmVersion4, out errors);
        Assert.Equal(2, errors.Count());
        Assert.All(errors, e => Assert.Equal(EdmErrorCode.BadCyclicComplex, e.ErrorCode));
        Assert.Equal("BadCyclicComplex : The complex type 'Grumble.Smod' is invalid because its base type is cyclic. : (3, 4)", errors.First().ToString());
        Assert.Equal("BadCyclicComplex : The complex type 'Grumble.Clod' is invalid because its base type is cyclic. : (6, 4)", errors.Last().ToString());
    }

    [Fact]
    public void Parse_OpenComplexTypeInheritance_ReportsValidationError()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmComplexType complexTypeDefinitionSmod = (IEdmComplexType)model.SchemaElements.First();
        IEdmComplexType complexTypeDefinitionClod = (IEdmComplexType)model.SchemaElements.Last();

        EdmValidator.Validate(model, EdmConstants.EdmVersion4, out errors);
        Assert.Single(errors);
        Assert.Equal(EdmErrorCode.InvalidAbstractComplexType, errors.First().ErrorCode);
        Assert.Equal("InvalidAbstractComplexType : The base type of open type 'Grumble.Smod' is not open type. : (6, 4)", errors.First().ToString());
    }

    [Fact]
    public void Parse_ComplexTypeWithThreeWayCycle_ReportsValidationError()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmComplexType complexTypeDefinitionSmod = (IEdmComplexType)model.SchemaElements.First();
        IEdmComplexType complexTypeDefinitionClod = (IEdmComplexType)model.SchemaElements.ElementAt(1);
        IEdmComplexType complexTypeDefinitionBlob = (IEdmComplexType)model.SchemaElements.Last();
        Assert.True(complexTypeDefinitionClod.BaseType.IsBad());
        Assert.Equal(EdmErrorCode.BadCyclicComplex, complexTypeDefinitionClod.BaseType.Errors().First().ErrorCode);
        Assert.True(complexTypeDefinitionSmod.BaseType.IsBad());
        Assert.True(complexTypeDefinitionBlob.BaseType.IsBad());

        EdmValidator.Validate(model, EdmConstants.EdmVersion4, out errors);
        Assert.Equal(3, errors.Count());
        Assert.All(errors, e => Assert.Equal(EdmErrorCode.BadCyclicComplex, e.ErrorCode));
        Assert.Equal("BadCyclicComplex : The complex type 'Grumble.Smod' is invalid because its base type is cyclic. : (3, 4)", errors.ElementAt(0).ToString());
        Assert.Equal("BadCyclicComplex : The complex type 'Grumble.Blob' is invalid because its base type is cyclic. : (6, 4)", errors.ElementAt(1).ToString());
        Assert.Equal("BadCyclicComplex : The complex type 'Grumble.Clod' is invalid because its base type is cyclic. : (9, 4)", errors.ElementAt(2).ToString());
    }

    [Fact]
    public void Parse_TypeShortcutForReturnTypeAndTypeRef_ParsesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> error);
        Assert.True(parsed);

        IEdmOperation operation = (IEdmOperation)model.SchemaElements.First();
        IEdmTypeReference returnType = operation.ReturnType;
        IEdmTypeReference parameterType = operation.Parameters.First().Type;

        Assert.False(returnType.IsBad());
        Assert.Equal(EdmTypeKind.Collection, returnType.TypeKind());
        Assert.Equal(EdmTypeKind.Collection, parameterType.TypeKind());
        Assert.Equal(EdmTypeKind.Collection, parameterType.AsCollection().ElementType().TypeKind());
        Assert.Equal(EdmPrimitiveTypeKind.Binary, parameterType.AsCollection().ElementType().AsCollection().ElementType().AsPrimitive().PrimitiveKind());
    }


    [Fact]
    public void Parse_CollectionElementTypeRegression_ParsesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> error);
        Assert.True(parsed);

        IEdmOperation operation = (IEdmOperation)model.SchemaElements.First();
        IEdmTypeReference returnType = operation.ReturnType;
        IEdmTypeReference parameterType = operation.Parameters.First().Type;

        Assert.False(returnType.IsBad());
        Assert.False(returnType.AsCollection().IsBad());
        Assert.False(returnType.AsCollection().CollectionDefinition().IsBad());
        Assert.False(returnType.AsCollection().CollectionDefinition().ElementType.IsBad());
        Assert.True(returnType.AsCollection().CollectionDefinition().ElementType.Definition.IsBad());
        Assert.Equal(EdmTypeKind.Collection, returnType.TypeKind());
        Assert.Equal(EdmTypeKind.Collection, parameterType.TypeKind());
        Assert.Equal(EdmTypeKind.Collection, parameterType.AsCollection().ElementType().TypeKind());
        Assert.Equal(EdmPrimitiveTypeKind.Binary, parameterType.AsCollection().ElementType().AsCollection().ElementType().AsPrimitive().PrimitiveKind());
    }

    [Fact]
    public void Parse_SpatialTypeProperties_ParsesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> error);
        Assert.True(parsed);

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
        Assert.Equal(4326, myGeography.SpatialReferenceIdentifier);
        Assert.Equal(4326, myPoint.SpatialReferenceIdentifier);
        Assert.Equal(4326, myLineString.SpatialReferenceIdentifier);
        Assert.Equal(4326, myPolygon.SpatialReferenceIdentifier);
        Assert.Equal(4326, myGeographyCollection.SpatialReferenceIdentifier);
        Assert.Equal(4326, myMultiPolygon.SpatialReferenceIdentifier);
        Assert.Equal(4326, myMultiLineString.SpatialReferenceIdentifier);
        Assert.Equal(4326, myMultiPoint.SpatialReferenceIdentifier);
        Assert.Equal(0, myGeometry.SpatialReferenceIdentifier);
        Assert.Equal(0, myGeometricPoint.SpatialReferenceIdentifier);
        Assert.Equal(0, myGeometricLineString.SpatialReferenceIdentifier);
        Assert.Equal(0, myGeometricPolygon.SpatialReferenceIdentifier);
        Assert.Equal(0, myGeometryCollection.SpatialReferenceIdentifier);
        Assert.Equal(0, myGeometricMultiPolygon.SpatialReferenceIdentifier);
        Assert.Equal(0, myGeometricMultiLineString.SpatialReferenceIdentifier);
        Assert.Null(myGeometricMultiPoint.SpatialReferenceIdentifier);
        Assert.Null(myOtherGeometricMultiPoint.SpatialReferenceIdentifier);
    }

    [Fact]
    public void Parse_CyclicBaseTypeAcrossSchemas_ReportsValidationError()
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
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmComplexType complex1 = (IEdmComplexType)model.SchemaElements.ElementAt(0);
        IEdmComplexType complex2 = (IEdmComplexType)model.SchemaElements.ElementAt(1);
        IEdmComplexType complex3 = (IEdmComplexType)model.SchemaElements.ElementAt(2);

        Assert.Equal(complex1.BaseComplexType().FullName(), complex2.FullName());
        Assert.Equal(complex2.BaseComplexType().FullName(), complex3.FullName());
        Assert.Equal(complex3.BaseComplexType().FullName(), complex1.FullName());

        Assert.True(complex1.BaseType.IsBad());
        Assert.Equal(EdmErrorCode.BadCyclicComplex, complex1.BaseType.Errors().First().ErrorCode);
        Assert.True(complex2.BaseType.IsBad());
        Assert.True(complex3.BaseType.IsBad());

        EdmValidator.Validate(model, EdmConstants.EdmVersion4, out errors);
        Assert.Equal(3, errors.Count());
        Assert.All(errors, e => Assert.Equal(EdmErrorCode.BadCyclicComplex, e.ErrorCode));
        Assert.Equal("BadCyclicComplex : The complex type 'Real2.Complex2' is invalid because its base type is cyclic. : (3, 4)", errors.ElementAt(0).ToString());
        Assert.Equal("BadCyclicComplex : The complex type 'Real3.Complex3' is invalid because its base type is cyclic. : (3, 4)", errors.ElementAt(1).ToString());
        Assert.Equal("BadCyclicComplex : The complex type 'Real1.Complex1' is invalid because its base type is cyclic. : (3, 4)", errors.ElementAt(2).ToString());
    }

    [Fact]
    public void Parse_InvalidEntitySetInOperationImport_ReportsValidationError()
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

        bool parsed = SchemaReader.TryParse(new[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmEntityContainer fooContainer = model.FindEntityContainer("fooContainer");
        IEdmOperationImport peopleWhoAreAwesome = (IEdmOperationImport)fooContainer.Elements.Last();
        Assert.False(peopleWhoAreAwesome.TryGetStaticEntitySet(model, out IEdmEntitySetBase badSet));

        Assert.False(EdmValidator.Validate(model, EdmConstants.EdmVersion4, out errors));
        Assert.Single(errors);
        Assert.Equal(EdmErrorCode.OperationImportEntitySetExpressionIsInvalid, errors.First().ErrorCode);
        Assert.Equal("OperationImportEntitySetExpressionIsInvalid : The operation import 'peopleWhoAreAwesome' specifies an entity set expression which is not valid. Operation import entity set expression can be either an entity set reference or a path starting with a operation import parameter and traversing navigation properties. : (14, 10)", errors.First().ToString());
    }

    [Fact]
    public void Parse_EnumTypeWithValidValues_ParsesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        var colorType = model.FindType("foo.Color") as IEdmEnumType;
        Assert.NotNull(colorType);
        Assert.Equal("Color", colorType.Name);
        Assert.False(colorType.IsFlags);
        Assert.False(colorType.IsBad());
        Assert.Equal(EdmPrimitiveTypeKind.Int32, colorType.UnderlyingType.PrimitiveKind);
        Assert.Equal(4, colorType.Members.Count());

        var colors = colorType.Members.ToArray();
        Assert.Equal("Blue", colors[2].Name);
        Assert.Equal(colorType, colors[2].DeclaringType);

        var color2Type = model.FindType("foo.Color2") as IEdmEnumType;
        Assert.NotNull(color2Type);
        Assert.Equal("Color2", color2Type.Name);
        Assert.True(color2Type.IsFlags);
        Assert.False(color2Type.IsBad());
        Assert.Equal(EdmPrimitiveTypeKind.Int64, color2Type.UnderlyingType.PrimitiveKind);
        Assert.Equal(4, colorType.Members.Count());
        var colors2 = color2Type.Members.ToArray();
        Assert.Equal("Blue", colors2[2].Name);
        Assert.Equal(color2Type, colors2[2].DeclaringType);

        var personType = model.FindType("foo.Person") as IEdmEntityType;
        Assert.NotNull(personType);
        Assert.Equal(colorType, personType.Properties().ElementAt(1).Type.AsEnum().EnumDefinition());
        Assert.True(personType.Properties().ElementAt(1).Type.AsEnum().IsNullable);
        Assert.Equal(color2Type, personType.Properties().ElementAt(2).Type.AsEnum().EnumDefinition());
        Assert.False(personType.Properties().ElementAt(2).Type.AsEnum().IsNullable);
        Assert.True(personType.Properties().ElementAt(3).Type.AsEnum().IsNullable);

        Assert.Equal(5, (color2Type.Members.ElementAt(0).Value).Value);
        Assert.Equal(6, (color2Type.Members.ElementAt(1).Value).Value);
        Assert.Equal(10, (color2Type.Members.ElementAt(2).Value).Value);
        Assert.Equal(11, (color2Type.Members.ElementAt(3).Value).Value);

        Assert.Equal(10, (colorType.Members.ElementAt(0).Value).Value);
        Assert.Equal(11, (colorType.Members.ElementAt(1).Value).Value);
        Assert.Equal(5, (colorType.Members.ElementAt(2).Value).Value);
        Assert.Equal(6, (colorType.Members.ElementAt(3).Value).Value);
    }

    [Fact]
    public void Parse_EnumTypeWithOutOfRangeValues_ReportsValidationError()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        bool valid = model.Validate(out errors);
        Assert.False(valid);
        Assert.Single(errors);
        Assert.Equal(EdmErrorCode.EnumMemberMustHaveValue, errors.First().ErrorCode);
        Assert.Equal("EnumMemberMustHaveValue : The enumeration member must have a value. : (4, 8)", errors.First().ToString());

        IEdmEnumMember enumMember = ((IEdmEnumType)model.FindType("foo.Color")).Members.First(m => m.Name == "Green");
        Assert.Equal(model.FindType("foo.Color"), enumMember.DeclaringType);
        Assert.True(enumMember.Value.IsBad());
    }

    [Fact]
    public void Parse_EnumTypeWithValuesOutOfInt32Range_ReportsValidationError()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        bool valid = model.Validate(out errors);
        Assert.False(valid, "valid");
        Assert.Single(errors);
        Assert.Equal(EdmErrorCode.EnumMemberValueOutOfRange, errors.First().ErrorCode);
        Assert.Equal("EnumMemberValueOutOfRange : The value of enum member 'Green' exceeds the range of its underlying type. : (4, 8)", errors.First().ToString());

        IEdmEnumMember enumMember = ((IEdmEnumType)model.FindType("foo.Color")).Members.First(m => m.Name == "Green");
        Assert.Equal(model.FindType("foo.Color"), enumMember.DeclaringType);
        Assert.False(enumMember.Value.IsBad());
    }

    [Fact]
    public void Parse_EnumTypeWithDefaultValues_ParsesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        bool valid = model.Validate(out errors);
        Assert.True(valid, "valid");
        Assert.Empty(errors);

        IEdmEnumType enumType = ((IEdmEnumType)model.FindType("foo.Color"));
        Assert.Equal(0, (enumType.Members.ElementAt(0).Value).Value);
        Assert.Equal(1, (enumType.Members.ElementAt(1).Value).Value);
        Assert.Equal(5, (enumType.Members.ElementAt(2).Value).Value);
        Assert.Equal(6, (enumType.Members.ElementAt(3).Value).Value);
    }

    [Fact]
    public void Parse_FlagsEnumTypeWithSpecifiedValues_ParsesSuccessfully()
    {
        const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EnumType Name=""Color"" IsFlags=""true"">
      <Member Name=""Red"" Value=""2""/>
      <Member Name=""Green"" Value=""4""/>
      <Member Name=""RedGreen"" Value=""6""/>
      <Member Name=""Blue"" Value=""8""/>
      <Member Name=""Yellow"" Value=""16""/>
    </EnumType>
</Schema>";

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        bool valid = model.Validate(out errors);
        Assert.True(valid, "valid");
        Assert.Empty(errors);

        IEdmEnumType enumType = ((IEdmEnumType)model.FindType("foo.Color"));
        Assert.Equal(2, (enumType.Members.ElementAt(0).Value).Value);
        Assert.Equal(4, (enumType.Members.ElementAt(1).Value).Value);
        Assert.Equal(6, (enumType.Members.ElementAt(2).Value).Value);
        Assert.Equal(8, (enumType.Members.ElementAt(3).Value).Value);
        Assert.Equal(16, (enumType.Members.ElementAt(4).Value).Value);
    }

    [Fact]
    public void Parse_EnumTypeWithNonPrimitiveUnderlyingType_ReportsValidationError()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        bool valid = model.Validate(out errors);
        Assert.False(valid, "valid");
        Assert.Equal(4, errors.Count());

        Assert.Equal(EdmErrorCode.BadUnresolvedPrimitiveType, errors.ElementAt(0).ErrorCode);
        Assert.Equal("BadUnresolvedPrimitiveType : The primitive type 'foo.Person' could not be found. : (9, 6)", errors.ElementAt(0).ToString());

        Assert.Equal(EdmErrorCode.EnumMustHaveIntegerUnderlyingType, errors.ElementAt(1).ErrorCode);
        Assert.Equal("EnumMustHaveIntegerUnderlyingType : The underlying type of 'foo.Permission' is not valid. The underlying type of an enum type must be an integral type.  : (15, 6)", errors.ElementAt(1).ToString());
        
        Assert.Equal(EdmErrorCode.EnumMemberValueOutOfRange, errors.ElementAt(2).ErrorCode);
        Assert.Equal("EnumMemberValueOutOfRange : The value of enum member 'Read' exceeds the range of its underlying type. : (16, 8)", errors.ElementAt(2).ToString());

        Assert.Equal(EdmErrorCode.EnumMemberValueOutOfRange, errors.ElementAt(3).ErrorCode);
        Assert.Equal("EnumMemberValueOutOfRange : The value of enum member 'Write' exceeds the range of its underlying type. : (17, 8)", errors.ElementAt(3).ToString());
    }

    [Fact]
    public void Parse_TermsInSchema_ParsesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmTerm age = model.FindTerm("foo.Age");
        IEdmTerm subject = model.FindTerm("foo.Subject");

        Assert.Equal("Age", age.Name);
        Assert.Equal("Subject", subject.Name);

        Assert.Equal(EdmPrimitiveTypeKind.Int32, age.Type.AsPrimitive().PrimitiveKind());
        Assert.Equal("foo.Person", subject.Type.AsEntity().FullName());

        Assert.Equal("foo", age.Namespace);
    }

    [Fact]
    public void Parse_AnnotationsWithRelaxedOrdering_ParsesSuccessfully()
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


        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmEntityType person = (IEdmEntityType)model.FindType("foo.Person");
        IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model);
        Assert.Equal(3, personAnnotations.Count());

        IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();
        IEdmVocabularyAnnotation first = annotations[0];
        Assert.Equal("First", first.Qualifier);
        Assert.Equal("Age", first.Term.Name);
        Assert.Equal("foo", first.Term.Namespace);

        IEdmIntegerConstantExpression value = (IEdmIntegerConstantExpression)first.Value;
        Assert.Equal(123, value.Value);

        IEdmVocabularyAnnotation second = annotations[1];
        Assert.Equal("Best", second.Qualifier);
        Assert.Equal("Age", second.Term.Name);
        Assert.Equal("foo", second.Term.Namespace);

        value = (IEdmIntegerConstantExpression)second.Value;
        Assert.Equal(456, value.Value);

        IEdmVocabularyAnnotation third = annotations[2];
        Assert.Null(third.Qualifier);
        Assert.Equal("Mage", third.Term.Name);
        Assert.Equal("Funk", third.Term.Namespace);

        value = (IEdmIntegerConstantExpression)third.Value;
        Assert.Equal(789, value.Value);
    }

    [Fact]
    public void Parse_VocabularyAnnotations_ParsesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmEntityType person = (IEdmEntityType)model.FindType("foo.Person");
        IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model);
        Assert.Equal(3, personAnnotations.Count());

        IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();
        IEdmVocabularyAnnotation first = annotations[0];
        Assert.Equal("First", first.Qualifier);
        Assert.Equal("Age", first.Term.Name);
        Assert.Equal("foo", first.Term.Namespace);

        IEdmIntegerConstantExpression value = (IEdmIntegerConstantExpression)first.Value;
        Assert.Equal(123, value.Value);

        IEdmVocabularyAnnotation second = annotations[1];
        Assert.Equal("Best", second.Qualifier);
        Assert.Equal("Age", second.Term.Name);
        Assert.Equal("foo", second.Term.Namespace);

        value = (IEdmIntegerConstantExpression)second.Value;
        Assert.Equal(456, value.Value);

        IEdmVocabularyAnnotation third = annotations[2];
        Assert.Null(third.Qualifier);
        Assert.Equal("Mage", third.Term.Name);
        Assert.Equal("Funk", third.Term.Namespace);
        value = (IEdmIntegerConstantExpression)third.Value;
        Assert.Equal(789, value.Value);
    }

    [Fact]
    public void Parse_OutOfLineAnnotations_ParsesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmEntityType person = (IEdmEntityType)model.FindType("foo.Person");
        IEdmEntityType lyingPerson = (IEdmEntityType)model.FindType("foo.LyingPerson");

        IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model);
        Assert.Equal(3, personAnnotations.Count());
        IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();

        IEdmVocabularyAnnotation first = annotations[0];
        Assert.Equal("First", first.Qualifier);
        Assert.Equal("Age", first.Term.Name);
        Assert.Equal("foo", first.Term.Namespace);
        IEdmIntegerConstantExpression value = (IEdmIntegerConstantExpression)first.Value;
        Assert.Equal(123, value.Value);

        IEdmVocabularyAnnotation second = annotations[1];
        Assert.Equal("Best", second.Qualifier);
        Assert.Equal("Age", second.Term.Name);
        Assert.Equal("foo", second.Term.Namespace);
        value = (IEdmIntegerConstantExpression)second.Value;
        Assert.Equal(456, value.Value);

        IEdmVocabularyAnnotation third = annotations[2];
        Assert.Null(third.Qualifier);
        Assert.Equal("Mage", third.Term.Name);
        Assert.Equal("Funk", third.Term.Namespace);
        value = (IEdmIntegerConstantExpression)third.Value;
        Assert.Equal(789, value.Value);
    }

    [Fact]
    public void Parse_MixedInlineAndOutOfLineAnnotations_ParsesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmEntityType person = (IEdmEntityType)model.FindType("foo.Person");
        IEdmEntityType lyingPerson = (IEdmEntityType)model.FindType("foo.LyingPerson");

        IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model);
        Assert.Equal(6, personAnnotations.Count());
        IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();

        IEdmVocabularyAnnotation first = annotations[0];
        Assert.Equal("Middling", first.Qualifier);
        Assert.Equal("Age", first.Term.Name);
        Assert.Equal("foo", first.Term.Namespace);
        IEdmIntegerConstantExpression value = (IEdmIntegerConstantExpression)first.Value;
        Assert.Equal(777, value.Value);

        IEdmVocabularyAnnotation second = annotations[1];
        Assert.Equal("Middling", second.Qualifier);
        Assert.Equal("Mage", second.Term.Name);
        Assert.Equal("Var1", second.Term.Namespace);
        value = (IEdmIntegerConstantExpression)second.Value;
        Assert.Equal(888, value.Value);

        IEdmVocabularyAnnotation third = annotations[2];
        Assert.Equal("First", third.Qualifier);
        Assert.Equal("Age", third.Term.Name);
        Assert.Equal("foo", third.Term.Namespace);
        value = (IEdmIntegerConstantExpression)third.Value;
        Assert.Equal(123, value.Value);

        IEdmVocabularyAnnotation fourth = annotations[3];
        Assert.Equal("Best", fourth.Qualifier);
        Assert.Equal("Age", fourth.Term.Name);
        Assert.Equal("foo", fourth.Term.Namespace);
        value = (IEdmIntegerConstantExpression)fourth.Value;
        Assert.Equal(456, value.Value);

        IEdmVocabularyAnnotation fifth = annotations[4];
        Assert.Null(fifth.Qualifier);
        Assert.Equal("Mage", fifth.Term.Name);
        Assert.Equal("Funk", fifth.Term.Namespace);
        value = (IEdmIntegerConstantExpression)fifth.Value;
        Assert.Equal(789, value.Value);

        IEdmVocabularyAnnotation sixth = annotations[5];
        Assert.Equal("Zonky", sixth.Qualifier);
        Assert.Equal("Yage", sixth.Term.Name);
        Assert.Equal("Var1", sixth.Term.Namespace);
        value = (IEdmIntegerConstantExpression)sixth.Value;
        Assert.Equal(555, value.Value);
    }

    [Fact]
    public void Parse_ExternalAnnotations_ParsesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl1)) }, out IEdmModel model1, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl2)) }, new IEdmModel[] { model1 }, out IEdmModel model2, out errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmEntityType person = (IEdmEntityType)model1.FindType("foo.Person");
        IEdmEntityType lyingPerson = (IEdmEntityType)model1.FindType("foo.LyingPerson");
        IEdmTerm distantAge = model1.FindTerm("foo.DistantAge");
        Assert.Equal(lyingPerson, model2.FindType("foo.LyingPerson"));

        IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model2);
        Assert.Equal(4, personAnnotations.Count());

        IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();
        IEdmVocabularyAnnotation third = annotations[0];
        Assert.Equal("First", third.Qualifier);
        Assert.Equal("Age", third.Term.Name);
        Assert.Equal("foo", third.Term.Namespace);

        IEdmIntegerConstantExpression value = (IEdmIntegerConstantExpression)third.Value;
        Assert.Equal(123, value.Value);

        IEdmVocabularyAnnotation fourth = annotations[1];
        Assert.Equal("Best", fourth.Qualifier);
        Assert.Equal("Age", fourth.Term.Name);
        Assert.Equal("foo", fourth.Term.Namespace);

        value = (IEdmIntegerConstantExpression)fourth.Value;
        Assert.Equal(456, value.Value);

        IEdmVocabularyAnnotation fifth = annotations[2];
        Assert.Null(fifth.Qualifier);
        Assert.Equal("DistantAge", fifth.Term.Name);
        Assert.Equal("foo", fifth.Term.Namespace);
        Assert.Equal(distantAge, fifth.Term);

        value = (IEdmIntegerConstantExpression)fifth.Value;
        Assert.Equal(99, value.Value);

        IEdmVocabularyAnnotation sixth = annotations[3];
        Assert.Null(sixth.Qualifier);
        Assert.Equal("Mage", sixth.Term.Name);
        Assert.Equal("Funk", sixth.Term.Namespace);

        value = (IEdmIntegerConstantExpression)sixth.Value;
        Assert.Equal(789, value.Value);

        Assert.Single(model1.VocabularyAnnotations);
        Assert.Equal(3, model2.VocabularyAnnotations.Count());
    }

    [Fact]
    public void Parse_AnnotationsOnEntitySet_ParsesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl2)) }, new IEdmModel[] { model }, out IEdmModel model2, out errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmEntitySet persons = model.FindEntityContainer("bar").FindEntitySet("PersonsOfInterest");

        IEnumerable<IEdmVocabularyAnnotation> personsAnnotations = persons.VocabularyAnnotations(model);
        Assert.Single(personsAnnotations);
        IEdmVocabularyAnnotation[] annotations = personsAnnotations.ToArray<IEdmVocabularyAnnotation>();

        IEdmVocabularyAnnotation mage = annotations[0];
        Assert.Null(mage.Qualifier);
        Assert.Equal("Mage", mage.Term.Name);
        Assert.Equal("Funk", mage.Term.Namespace);
        IEdmIntegerConstantExpression value = (IEdmIntegerConstantExpression)mage.Value;
        Assert.Equal(789, value.Value);

        personsAnnotations = persons.VocabularyAnnotations(model2);
        Assert.Equal(2, personsAnnotations.Count());
        annotations = personsAnnotations.ToArray<IEdmVocabularyAnnotation>();

        IEdmVocabularyAnnotation strange = annotations[0];
        Assert.Equal(persons, strange.Target);
        Assert.Null(strange.Qualifier);
        Assert.Equal("Strange", strange.Term.Name);
        Assert.Equal("Funk", strange.Term.Namespace);
        value = (IEdmIntegerConstantExpression)strange.Value;
        Assert.Equal(13, value.Value);

        mage = annotations[1];
        Assert.Equal(persons, mage.Target);
        Assert.Null(mage.Qualifier);
        Assert.Equal("Mage", mage.Term.Name);
        Assert.Equal("Funk", mage.Term.Namespace);
        value = (IEdmIntegerConstantExpression)mage.Value;
        Assert.Equal(789, value.Value);
    }

    [Fact]
    public void Parse_AnnotationsOnProperties_ParsesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl2)) }, model, out IEdmModel model2, out errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmProperty birthday = ((IEdmEntityType)model.FindType("foo.Person")).FindProperty("Birthday");

        IEnumerable<IEdmVocabularyAnnotation> birthdayAnnotations = birthday.VocabularyAnnotations(model);
        Assert.Equal(2, birthdayAnnotations.Count());
        IEdmVocabularyAnnotation[] annotations = birthdayAnnotations.ToArray<IEdmVocabularyAnnotation>();

        IEdmVocabularyAnnotation funkadelic = annotations[0];
        Assert.Null(funkadelic.Qualifier);
        Assert.Equal("ADelic", funkadelic.Term.Name);
        Assert.Equal("Funk", funkadelic.Term.Namespace);
        IEdmIntegerConstantExpression value = (IEdmIntegerConstantExpression)funkadelic.Value;
        Assert.Equal(17, value.Value);

        IEdmVocabularyAnnotation mage = annotations[1];
        Assert.Null(mage.Qualifier);
        Assert.Equal("Mage", mage.Term.Name);
        Assert.Equal("Funk", mage.Term.Namespace);
        value = (IEdmIntegerConstantExpression)mage.Value;
        Assert.Equal(789, value.Value);

        birthdayAnnotations = birthday.VocabularyAnnotations(model2);
        Assert.Equal(3, birthdayAnnotations.Count());
        annotations = birthdayAnnotations.ToArray<IEdmVocabularyAnnotation>();

        IEdmVocabularyAnnotation strange = annotations[0];
        Assert.Null(strange.Qualifier);
        Assert.Equal("Strange", strange.Term.Name);
        Assert.Equal("Funk", strange.Term.Namespace);
        value = (IEdmIntegerConstantExpression)strange.Value;
        Assert.Equal(13, value.Value);

        funkadelic = annotations[1];
        Assert.Null(funkadelic.Qualifier);
        Assert.Equal("ADelic", funkadelic.Term.Name);
        Assert.Equal("Funk", funkadelic.Term.Namespace);
        value = (IEdmIntegerConstantExpression)funkadelic.Value;
        Assert.Equal(17, value.Value);

        mage = annotations[2];
        Assert.Null(mage.Qualifier);
        Assert.Equal("Mage", mage.Term.Name);
        Assert.Equal("Funk", mage.Term.Namespace);
        value = (IEdmIntegerConstantExpression)mage.Value;
        Assert.Equal(789, value.Value);
    }

    [Fact]
    public void Parse_ConstantExpressionsInAnnotations_ParsesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmEntityType person = (IEdmEntityType)model.FindType("foo.Person");

        IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model);
        Assert.Equal(18, personAnnotations.Count());
        IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();

        var byteArrayComparer = new ArrayComp();

        int i = 0;
        Assert.Equal(EdmExpressionKind.IntegerConstant, (annotations[i]).Value.ExpressionKind);
        Assert.Equal(1, ((IEdmIntegerConstantExpression)(annotations[i]).Value).Value);
        Assert.Equal(EdmExpressionKind.IntegerConstant, (annotations[++i]).Value.ExpressionKind);
        Assert.Equal(2, ((IEdmIntegerConstantExpression)(annotations[i]).Value).Value);

        Assert.Equal(EdmExpressionKind.StringConstant, (annotations[++i]).Value.ExpressionKind);
        Assert.Equal("Cat", ((IEdmStringConstantExpression)(annotations[i]).Value).Value);
        Assert.Equal(EdmExpressionKind.StringConstant, (annotations[++i]).Value.ExpressionKind);
        Assert.Equal("Dog", ((IEdmStringConstantExpression)(annotations[i]).Value).Value);

        Assert.Equal(EdmExpressionKind.BinaryConstant, (annotations[++i]).Value.ExpressionKind);
        Assert.True(byteArrayComparer.Equals(((IEdmBinaryConstantExpression)(annotations[i]).Value).Value, new byte[] { 0x12, 0x34, 0x56 }));
        Assert.Equal(EdmExpressionKind.BinaryConstant, (annotations[++i]).Value.ExpressionKind);
        Assert.True(byteArrayComparer.Equals(((IEdmBinaryConstantExpression)(annotations[i]).Value).Value, new byte[] { 0x65, 0x43, 0x21 }));

        Assert.Equal(EdmExpressionKind.FloatingConstant, (annotations[++i]).Value.ExpressionKind);
        Assert.Equal(1.1, ((IEdmFloatingConstantExpression)(annotations[i]).Value).Value);
        Assert.Equal(EdmExpressionKind.FloatingConstant, (annotations[++i]).Value.ExpressionKind);
        Assert.Equal(2.2E10, ((IEdmFloatingConstantExpression)(annotations[i]).Value).Value);

        Assert.Equal(EdmExpressionKind.GuidConstant, (annotations[++i]).Value.ExpressionKind);
        Assert.Equal(((IEdmGuidConstantExpression)(annotations[i]).Value).Value, Guid.Parse("4ae71c81-c21a-40a2-8d53-f1a29ed4a2f2"));
        Assert.Equal(EdmExpressionKind.GuidConstant, (annotations[++i]).Value.ExpressionKind);
        Assert.Equal(((IEdmGuidConstantExpression)(annotations[i]).Value).Value, Guid.Parse("4ae71c81-c21a-40a2-8d53-f1a29ed4a2f3"));

        Assert.Equal(EdmExpressionKind.DecimalConstant, (annotations[++i]).Value.ExpressionKind);
        Assert.Equal(1.2M, ((IEdmDecimalConstantExpression)(annotations[i]).Value).Value);
        Assert.Equal(EdmExpressionKind.DecimalConstant, (annotations[++i]).Value.ExpressionKind);
        Assert.Equal(2.3M, ((IEdmDecimalConstantExpression)(annotations[i]).Value).Value);

        Assert.Equal(EdmExpressionKind.BooleanConstant, (annotations[++i]).Value.ExpressionKind);
        Assert.True(((IEdmBooleanConstantExpression)(annotations[i]).Value).Value);
        Assert.Equal(EdmExpressionKind.BooleanConstant, (annotations[++i]).Value.ExpressionKind);
        Assert.False(((IEdmBooleanConstantExpression)(annotations[i]).Value).Value);

        Assert.Equal(EdmExpressionKind.DateTimeOffsetConstant, (annotations[++i]).Value.ExpressionKind);
        Assert.Equal(new DateTimeOffset(2001, 10, 26, 19, 32, 52, new TimeSpan(0, 0, 0)), ((IEdmDateTimeOffsetConstantExpression)(annotations[i]).Value).Value);
        Assert.Equal(EdmExpressionKind.DateTimeOffsetConstant, (annotations[++i]).Value.ExpressionKind);
        Assert.Equal(new DateTimeOffset(2001, 10, 26, 19, 32, 52, new TimeSpan(0, 0, 0)), ((IEdmDateTimeOffsetConstantExpression)(annotations[i]).Value).Value);

        Assert.Equal(EdmExpressionKind.DurationConstant, (annotations[++i]).Value.ExpressionKind);
        Assert.Equal(new TimeSpan(0, 0, 1), ((IEdmDurationConstantExpression)(annotations[i]).Value).Value);
        Assert.Equal(EdmExpressionKind.DurationConstant, (annotations[++i]).Value.ExpressionKind);
        Assert.Equal(new TimeSpan(0, 0, 1), ((IEdmDurationConstantExpression)(annotations[i]).Value).Value);
    }

    [Fact]
    public void Parse_RecordExpressionsInAnnotations_ParsesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmEntityType person = (IEdmEntityType)model.FindType("foo.Person");

        IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model);
        Assert.Single(personAnnotations);
        IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();

        IEdmVocabularyAnnotation first = annotations[0];
        Assert.Equal(EdmExpressionKind.Record, first.Value.ExpressionKind);

        IEdmRecordExpression record = (IEdmRecordExpression)first.Value;
        IEnumerable<IEdmPropertyConstructor> propertyConstructors = record.Properties;
        Assert.Equal(2, propertyConstructors.Count());
        IEdmPropertyConstructor[] properties = propertyConstructors.ToArray<IEdmPropertyConstructor>();

        IEdmPropertyConstructor x = properties[0];
        Assert.Equal("X", x.Name);
        Assert.Equal(EdmExpressionKind.IntegerConstant, x.Value.ExpressionKind);
        Assert.Equal(10, ((IEdmIntegerConstantExpression)x.Value).Value);

        IEdmPropertyConstructor y = properties[1];
        Assert.Equal("Y", y.Name);
        Assert.Equal(EdmExpressionKind.IntegerConstant, y.Value.ExpressionKind);
        Assert.Equal(20, ((IEdmIntegerConstantExpression)y.Value).Value);
    }

    [Fact]
    public void Parse_PathExpressionsInAnnotations_ParsesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmEntityType person = (IEdmEntityType)model.FindType("foo.Person");
        IEdmProperty name = person.FindProperty("Name");
        IEdmProperty age = person.FindProperty("Age");
        IEdmEntityType lyingPerson = (IEdmEntityType)model.FindType("foo.LyingPerson");

        IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model);
        Assert.Equal(2, personAnnotations.Count());
        IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();

        IEdmVocabularyAnnotation first = annotations[0];
        Assert.Equal("Mage", first.Term.Name);
        IEdmPathExpression value = (IEdmPathExpression)first.Value;
        Assert.Equal("Age", value.PathSegments.First());

        IEdmVocabularyAnnotation second = annotations[1];
        Assert.Equal("Age", second.Term.Name);
        value = (IEdmPathExpression)second.Value;
        Assert.Equal("Age", value.PathSegments.First());
    }

    [Fact]
    public void Parse_ReturnTypeAndTypeRefWithoutTypeAttribute_ReportsValidationError()
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

        var isParsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(isParsed);
        Assert.Equal(4, errors.Count());

        Assert.Equal(EdmErrorCode.MissingType, errors.First().ErrorCode);
        Assert.Equal(EdmErrorCode.UnexpectedXmlElement, errors.ElementAt(1).ErrorCode);
        Assert.Equal(EdmErrorCode.MissingAttribute, errors.ElementAt(2).ErrorCode);
        Assert.Equal(EdmErrorCode.MissingAttribute, errors.ElementAt(3).ErrorCode);

        Assert.Equal("MissingType : An XML attribute or sub-element representing an EDM type is missing. : (21, 8)", errors.ElementAt(0).ToString());
        Assert.Equal("UnexpectedXmlElement : The schema element 'TypeRef' was not expected in the given context. : (23, 10)", errors.ElementAt(1).ToString());
        Assert.Equal("MissingAttribute : Required schema attribute 'Type' is not present on element 'TypeRef'. : (29, 17)", errors.ElementAt(2).ToString());
        Assert.Equal("MissingAttribute : Required schema attribute 'Type' is not present on element 'ReturnType'. : (27, 10)", errors.ElementAt(3).ToString());

    }

    [Fact]
    public void Parse_AnnotationNamespaceRegression_ParsesSuccessfully()
    {
        var csdl = @"<Schema Namespace=""Netflix.Catalog.v2"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Annotations Target=""Self.Person"">
    <Annotation Term=""Org.OData.Vocabularies.Core.Name"">
      <Path>Name</Path>
    </Annotation>
  </Annotations>
</Schema>";

        var isParsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed, "SchemaReader.TryParse failed");

        IEdmVocabularyAnnotation annotation = edmModel.VocabularyAnnotations.First();
        Assert.Equal("Netflix.Catalog.v2", ((IEdmSchemaElement)annotation.Target).Namespace);
    }

    [Fact]
    public void Parse_SchemaAliasWithMultiplePeriods_ReportsValidationError()
    {
        string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""bee.bee.bee"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""bee.bee.bee.Smod"">
        <Annotation Term=""bee.bee.bee.value"" Int=""1"" />
    </Annotations>
    <Term Name=""value"" Type=""Int32"" />
</Schema>";

        var isParsed = SchemaReader.TryParse(new List<XElement> { XElement.Parse(csdl) }.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.False(isParsed);
        Assert.Single(errors);

        Assert.Equal(EdmErrorCode.InvalidQualifiedName, errors.First().ErrorCode);
        Assert.Equal("InvalidQualifiedName : The alias 'bee.bee.bee' is not a valid simple name. : (0, 0)", errors.First().ToString());
    }

    [Fact]
    public void Parse_SchemaAliasWithSinglePeriod_ReportsValidationError()
    {
        string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""bee.bee"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""bee.bee.Smod"">
        <Annotation Term=""bee.bee.bee.value"" Int=""1"" />
    </Annotations>
    <Term Name=""value"" Type=""Int32"" />
</Schema>";

        var isParsed = SchemaReader.TryParse(new List<XElement> { XElement.Parse(csdl) }.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.False(isParsed);
        Assert.Single(errors);
        Assert.Equal(EdmErrorCode.InvalidQualifiedName, errors.First().ErrorCode);
        Assert.Equal("InvalidQualifiedName : The alias 'bee.bee' is not a valid simple name. : (0, 0)", errors.First().ToString());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Parse_AnnotationQualifiersWithNonSimpleValues_ParsesSuccessfully(EdmVersion edmVersion)
    {
        var csdl = @"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""foo.Note"" Qualifier=""3"" Int=""99"" />
        <Annotation Term=""foo.Note"" Qualifier=""ReallyOddQualifer1234567890!@#$%^*()_+&amp;"" Int=""127"" />
    </Annotations>
    <Annotations Target=""foo.Person"" Qualifier=""foo+bar"">
        <Annotation Term=""foo.Note"" Int=""127"" />
    </Annotations>
    <Term Name=""Note"" Type=""Edm.Int32"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" />
    </EntityType>
</Schema>";

        var csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed && !errors.Any());

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var roundTripCsdls = this.GetSerializerResult(model, edmVersion, out errors).Select(n => XElement.Parse(n));
        Assert.Empty(errors);
        Assert.Single(roundTripCsdls);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Parse_SchemaWithoutNamespace_ParsesSuccessfully(EdmVersion edmVersion)
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

        var isParsed = SchemaReader.TryParse(actualCsdls.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        Assert.Single(model.VocabularyAnnotations);

        var roundTripCsdls = this.GetSerializerResult(model, edmVersion, out errors).Select(n => XElement.Parse(n));
        Assert.Empty(errors);
        Assert.Single(roundTripCsdls);

        Compare(actualCsdls.ToList(), roundTripCsdls.ToList());
    }

    [Fact]
    public void Parse_AnnotationTargetUsingAlias_ParsesSuccessfully()
    {
        var csdl = @"
<Schema Namespace='Netflix.Catalog.v2' Alias='Self' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <Annotations Target='Self.Person'>
    <Annotation Term='Org.OData.Vocabularies.Name'>
      <Path>Name</Path>
    </Annotation>
  </Annotations>
</Schema>";

        var isParsed = SchemaReader.TryParse(new string[] { csdl }.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        Assert.Single(model.VocabularyAnnotations);

        var annotationTarget = model.VocabularyAnnotations.First().Target;
        Assert.Contains("Netflix.Catalog.v2.Person", annotationTarget.ToString());
    }

    [Fact]
    public void Parse_OpenComplexTypesModel_ParsesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);

        Assert.True(parsed);
        Assert.Empty(errors);

        var types = model.SchemaElements.OfType<IEdmComplexType>().ToList();
        Assert.Equal(3, types.Count);
        Assert.True(types[0].IsOpen);
        Assert.False(types[1].IsOpen);
        Assert.False(types[2].IsOpen);
    }

    [Fact]
    public void Parse_TypeDefinitionsInSchema_ParsesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);

        Assert.True(parsed);
        Assert.Empty(errors);

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
        Assert.Single(widthElement.VocabularyAnnotations);

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
        Assert.NotNull(personType);
        var weightProperty = personType.FindProperty("Weight");
        var addressProperty = personType.FindProperty("Address");
        var addressProperty2 = personType.FindProperty("Address2");
        var pointProperty = personType.FindProperty("Point");
        Assert.Equal("MyNS.Weight", weightProperty.Type.AsTypeDefinition().FullName());
        Assert.Equal(weightType, weightProperty.Type.AsTypeDefinition().Definition);
        Assert.Equal("MyNS.Address", addressProperty.Type.AsTypeDefinition().FullName());
        Assert.Equal(addressType, addressProperty.Type.AsTypeDefinition().Definition);
        Assert.Equal("MyNS.Address", addressProperty2.Type.AsTypeDefinition().FullName());
        Assert.Equal(addressType, addressProperty2.Type.AsTypeDefinition().Definition);
        Assert.Equal("MyNS.Point", pointProperty.Type.AsTypeDefinition().FullName());
        Assert.Equal(pointType, pointProperty.Type.AsTypeDefinition().Definition);

        // Facets
        Assert.False(((IEdmTypeDefinitionReference)weightProperty.Type).IsUnbounded);
        Assert.Null(((IEdmTypeDefinitionReference)weightProperty.Type).MaxLength);
        Assert.Null(((IEdmTypeDefinitionReference)weightProperty.Type).IsUnicode);
        Assert.Equal(3, ((IEdmTypeDefinitionReference)weightProperty.Type).Precision);
        Assert.Equal(2, ((IEdmTypeDefinitionReference)weightProperty.Type).Scale);
        Assert.Null(((IEdmTypeDefinitionReference)weightProperty.Type).SpatialReferenceIdentifier);

        Assert.False(((IEdmTypeDefinitionReference)addressProperty.Type).IsUnbounded);
        Assert.Equal(10, ((IEdmTypeDefinitionReference)addressProperty.Type).MaxLength);
        Assert.True(((IEdmTypeDefinitionReference)addressProperty.Type).IsUnicode);
        Assert.Null(((IEdmTypeDefinitionReference)addressProperty.Type).Precision);
        Assert.Null(((IEdmTypeDefinitionReference)addressProperty.Type).Scale);
        Assert.Null(((IEdmTypeDefinitionReference)addressProperty.Type).SpatialReferenceIdentifier);

        Assert.True(((IEdmTypeDefinitionReference)addressProperty2.Type).IsUnbounded);
        Assert.Null(((IEdmTypeDefinitionReference)addressProperty2.Type).MaxLength);
        Assert.False(((IEdmTypeDefinitionReference)addressProperty2.Type).IsUnicode);
        Assert.Null(((IEdmTypeDefinitionReference)addressProperty2.Type).Precision);
        Assert.Null(((IEdmTypeDefinitionReference)addressProperty2.Type).Scale);
        Assert.Null(((IEdmTypeDefinitionReference)addressProperty2.Type).SpatialReferenceIdentifier);

        Assert.False(((IEdmTypeDefinitionReference)pointProperty.Type).IsUnbounded);
        Assert.Null(((IEdmTypeDefinitionReference)pointProperty.Type).MaxLength);
        Assert.Null(((IEdmTypeDefinitionReference)pointProperty.Type).IsUnicode);
        Assert.Null(((IEdmTypeDefinitionReference)pointProperty.Type).Precision);
        Assert.Null(((IEdmTypeDefinitionReference)pointProperty.Type).Scale);
        Assert.Equal(123, ((IEdmTypeDefinitionReference)pointProperty.Type).SpatialReferenceIdentifier);
    }

    [Fact]
    public void Parse_AllKindsOfUnsignedIntegers_ParsesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.False(errors.Any());

        var elements = model.SchemaElements.ToList();

        // UInt16
        Assert.Equal(EdmPrimitiveTypeKind.Int32, ((IEdmTypeDefinition)elements[0]).UnderlyingType.PrimitiveKind);
        Assert.Equal(EdmSchemaElementKind.TypeDefinition, ((IEdmSchemaElement)elements[0]).SchemaElementKind);
        Assert.Equal("UInt16", ((IEdmNamedElement)elements[0]).Name);

        // UInt32
        Assert.Equal(EdmPrimitiveTypeKind.Int64, ((IEdmTypeDefinition)elements[1]).UnderlyingType.PrimitiveKind);
        Assert.Equal(EdmSchemaElementKind.TypeDefinition, elements[1].SchemaElementKind);
        Assert.Equal("UInt32", ((IEdmNamedElement)elements[1]).Name);

        // UInt64
        Assert.Equal(EdmPrimitiveTypeKind.Decimal, ((IEdmTypeDefinition)elements[2]).UnderlyingType.PrimitiveKind);
        Assert.Equal(EdmSchemaElementKind.TypeDefinition, elements[2].SchemaElementKind);
        Assert.Equal("UInt64", ((IEdmNamedElement)elements[2]).Name);

        // Person
        var personType = elements[3] as IEdmEntityType;
        Assert.NotNull(personType);
        var idProperty = personType.FindProperty("Id");
        var shortIntProperty = personType.FindProperty("ShortInt");
        var longIntProperty = personType.FindProperty("LongInt");
        Assert.Equal("MyNS.UInt32", idProperty.Type.AsTypeDefinition().FullName());
        Assert.Equal("MyNS.UInt16", shortIntProperty.Type.AsTypeDefinition().FullName());
        Assert.Equal("MyNS.UInt64", longIntProperty.Type.AsTypeDefinition().FullName());
    }

    [Fact]
    public void Parse_DateRelatedEdmTypes_ParsesSuccessfully()
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
    <Property Name=""timeOfDay"" Type=""Edm.TimeOfDay"" />
    <Property Name=""duration"" Type=""Edm.Duration"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""People"" EntityType=""MyNS.Person"" />
  </EntityContainer>
</Schema>";

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        var elements = model.SchemaElements.ToList();

        // Person
        var personType = elements[0] as IEdmEntityType;
        Assert.NotNull(personType);
        var idProperty = personType.FindProperty("Id");
        var dateTimeOffsetProperty = personType.FindProperty("dateTimeOffset");
        var dateProperty = personType.FindProperty("date");
        var timeOfDayProperty = personType.FindProperty("timeOfDay");
        var durationProperty = personType.FindProperty("duration");

        Assert.Equal("Edm.Int32", idProperty.Type.AsTypeDefinition().FullName());
        Assert.Equal("Edm.DateTimeOffset", dateTimeOffsetProperty.Type.AsTypeDefinition().FullName());
        Assert.Equal("Edm.Date", dateProperty.Type.AsTypeDefinition().FullName());
        Assert.Equal("Edm.TimeOfDay", timeOfDayProperty.Type.AsTypeDefinition().FullName());
        Assert.Equal("Edm.Duration", durationProperty.Type.AsTypeDefinition().FullName());
    }

    [Fact]
    public void Parse_DefaultValueAttributeOfTermElement_ParsesSuccessfully()
    {
        const string csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""MyNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""ConventionalIDs"" Type=""Core.Tag"" DefaultValue=""True"" AppliesTo=""EntityContainer"">
    <Annotation Term=""Core.Description"" String=""Entity-ids follow OData URL conventions""/>
  </Term>
</Schema>";

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        var term = model.FindTerm("MyNS.ConventionalIDs");
        Assert.Equal("True", term.DefaultValue);
    }


    [Fact]
    public void Parse_DuplicateTypeDefinitions_ReportsValidationError()
    {
        const string csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""MyNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <TypeDefinition Name=""Length"" UnderlyingType=""Edm.Int32"" />
  <TypeDefinition Name=""Length"" UnderlyingType=""Edm.Double"" />
</Schema>";

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);

        Assert.True(parsed);
        Assert.Empty(errors);

        model.Validate(out IEnumerable<EdmError> validationErrors);

        Assert.Single(validationErrors);
        Assert.Equal(EdmErrorCode.AlreadyDefined, validationErrors.Single().ErrorCode);
        Assert.Equal("AlreadyDefined : An element with the name 'MyNS.Length' is already defined. : (4, 4)", validationErrors.Single().ToString());
    }

    [Fact]
    public void Parse_DuplicateTypeDefinitionWithEntityType_ReportsValidationError()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);

        Assert.True(parsed);
        Assert.Empty(errors);

        model.Validate(out IEnumerable<EdmError> validationErrors);

        Assert.Single(validationErrors);
        Assert.Equal(EdmErrorCode.AlreadyDefined, validationErrors.Single().ErrorCode);
        Assert.Equal("AlreadyDefined : An element with the name 'MyNS.Person' is already defined. : (4, 4)", validationErrors.Single().ToString());
    }

    [Fact]
    public void Parse_DuplicateKeyPropertiesInEntityType_ReportsValidationError()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        model.Validate(out IEnumerable<EdmError> validationErrors);

        Assert.Single(validationErrors);
        Assert.Equal(EdmErrorCode.AlreadyDefined, validationErrors.Single().ErrorCode);
        Assert.Equal("AlreadyDefined : Each property name in a type must be unique. Property name 'Id' is already defined. : (8, 6)", validationErrors.Single().ToString());
    }

    [Fact]
    public void Parse_DuplicateTypeDefinitionWithComplexType_ReportsValidationError()
    {
        const string csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""MyNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <TypeDefinition Name=""Person"" UnderlyingType=""Edm.Int32"" />
  <ComplexType Name=""Person"">
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);

        Assert.True(parsed);
        Assert.Empty(errors);

        model.Validate(out IEnumerable<EdmError> validationErrors);

        Assert.Single(validationErrors);
        Assert.Equal(EdmErrorCode.AlreadyDefined, validationErrors.Single().ErrorCode);
        Assert.Equal("AlreadyDefined : An element with the name 'MyNS.Person' is already defined. : (4, 4)", validationErrors.Single().ToString());
    }

    [Fact]
    public void Validate_ModelWithComputedAnnotation_ParsesSuccessfully()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        bool valid = model.Validate(out errors);
        Assert.True(valid);
    }

    #region Private Methods

    private void Compare(List<XElement> expectXElements, List<XElement> actualXElements)
    {
        Assert.Equal(expectXElements.Count, actualXElements.Count);

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        // compare just the EntityContainers
        this.CsdlXElementComparer.Compare(expectedContainers, actualContainers);

        foreach (var expectXElement in expectXElements)
        {
            var schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace")?.Value));

            Assert.NotNull(actualXElement);

            this.CsdlXElementComparer.Compare(expectXElement, actualXElement);
        }
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

    #endregion
}
