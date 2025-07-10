//---------------------------------------------------------------------
// <copyright file="NavigationSerializingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml.Linq;
using Microsoft.OData.Edm.E2E.Tests.Common;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class NavigationSerializingTests : EdmLibTestCaseBase
{
    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_NavigationWithBothContainmentEnds_GeneratesExpectedCsdl(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var pet = new EdmEntityType("NS", "Pet");
        var petId = new EdmStructuralProperty(pet, "Id", EdmCoreModel.Instance.GetInt32(false));
        pet.AddProperty(petId);
        pet.AddKeys(petId);
        model.AddElement(pet);

        var petToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        pet.AddProperty(petToPerson);
        person.AddProperty(petToPerson.Partner);

        var actualCsdls = new[] { @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPet"" Type=""NS.Pet"" Nullable=""false"" Partner=""ToPerson"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToPet"" ContainsTarget=""true"" />
  </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var expectedCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        var expectXElements = updatedExpectedCsdls.ToList();
        var actualXElements = updatedActualCsdls.ToList();

        Assert.Equal(expectXElements.Count(), actualXElements.Count());

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        // compare just the EntityContainers
        CsdlXElementComparer.Compare(expectedContainers, actualContainers);

        foreach (var expectXElement in expectXElements)
        {
            var schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace").Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace").Value));

            Assert.NotNull(actualXElement);

            CsdlXElementComparer.Compare(expectXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_NavigationWithOneMultiplicityContainmentEnd_GeneratesExpectedCsdl(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var pet = new EdmEntityType("NS", "Pet");
        var petId = new EdmStructuralProperty(pet, "Id", EdmCoreModel.Instance.GetInt32(false));
        pet.AddProperty(petId);
        pet.AddKeys(petId);
        model.AddElement(pet);

        var onePetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "OnePetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToOnePet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        pet.AddProperty(onePetToPerson);
        person.AddProperty(onePetToPerson.Partner);

        var zeroOrOnePetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ZeroOrOnePetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToZeroOrOnePet", Target = pet, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true });
        pet.AddProperty(zeroOrOnePetToPerson);
        person.AddProperty(zeroOrOnePetToPerson.Partner);

        var manyPetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ManyPetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToManyPet", Target = pet, TargetMultiplicity = EdmMultiplicity.Many, ContainsTarget = true });
        pet.AddProperty(manyPetToPerson);
        person.AddProperty(manyPetToPerson.Partner);

        var actualCsdls = new[] { @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToOnePet"" Type=""NS.Pet"" Nullable=""false"" Partner=""OnePetToPerson"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ToZeroOrOnePet"" Type=""NS.Pet"" Partner=""ZeroOrOnePetToPerson""  ContainsTarget=""true"" />
    <NavigationProperty Name=""ToManyPet"" Type=""Collection(NS.Pet)"" Partner=""ManyPetToPerson"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""OnePetToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToOnePet""  />
    <NavigationProperty Name=""ZeroOrOnePetToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToZeroOrOnePet"" />
    <NavigationProperty Name=""ManyPetToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToManyPet"" />
  </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var expectedCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        var expectXElements = updatedExpectedCsdls.ToList();
        var actualXElements = updatedActualCsdls.ToList();

        Assert.Equal(expectXElements.Count(), actualXElements.Count());

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        // compare just the EntityContainers
        CsdlXElementComparer.Compare(expectedContainers, actualContainers);

        foreach (var expectXElement in expectXElements)
        {
            var schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace").Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace").Value));

            Assert.NotNull(actualXElement);

            CsdlXElementComparer.Compare(expectXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_NavigationWithManyMultiplicityContainmentEnd_GeneratesExpectedCsdl(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var pet = new EdmEntityType("NS", "Pet");
        var petId = new EdmStructuralProperty(pet, "Id", EdmCoreModel.Instance.GetInt32(false));
        pet.AddProperty(petId);
        pet.AddKeys(petId);
        model.AddElement(pet);

        var onePetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "OnePetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "ToOnePet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        pet.AddProperty(onePetToPerson);
        person.AddProperty(onePetToPerson.Partner);

        var zeroOrOnePetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ZeroOrOnePetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "ToZeroOrOnePet", Target = pet, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true });
        pet.AddProperty(zeroOrOnePetToPerson);
        person.AddProperty(zeroOrOnePetToPerson.Partner);

        var manyPetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ManyPetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "ToManyPet", Target = pet, TargetMultiplicity = EdmMultiplicity.Many, ContainsTarget = true });
        pet.AddProperty(manyPetToPerson);
        person.AddProperty(manyPetToPerson.Partner);

        var actualCsdls = new[] { @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToOnePet"" Type=""NS.Pet"" Nullable=""false"" Partner=""OnePetToPerson""  ContainsTarget=""true"" />
    <NavigationProperty Name=""ToZeroOrOnePet"" Type=""NS.Pet"" Partner=""ZeroOrOnePetToPerson"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ToManyPet"" Type=""Collection(NS.Pet)"" Partner=""ManyPetToPerson"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""OnePetToPerson"" Type=""Collection(NS.Person)"" Partner=""ToOnePet"" />
    <NavigationProperty Name=""ZeroOrOnePetToPerson"" Type=""Collection(NS.Person)"" Partner=""ToZeroOrOnePet"" />
    <NavigationProperty Name=""ManyPetToPerson"" Type=""Collection(NS.Person)"" Partner=""ToManyPet"" />
  </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var expectedCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        var expectXElements = updatedExpectedCsdls.ToList();
        var actualXElements = updatedActualCsdls.ToList();

        Assert.Equal(expectXElements.Count(), actualXElements.Count());

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        // compare just the EntityContainers
        CsdlXElementComparer.Compare(expectedContainers, actualContainers);

        foreach (var expectXElement in expectXElements)
        {
            var schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace").Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace").Value));

            Assert.NotNull(actualXElement);

            CsdlXElementComparer.Compare(expectXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_NavigationWithZeroOrOneMultiplicityContainmentEnd_GeneratesExpectedCsdl(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var pet = new EdmEntityType("NS", "Pet");
        var petId = new EdmStructuralProperty(pet, "Id", EdmCoreModel.Instance.GetInt32(false));
        pet.AddProperty(petId);
        pet.AddKeys(petId);
        model.AddElement(pet);

        var onePetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "OnePetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
            new EdmNavigationPropertyInfo() { Name = "ToOnePet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        pet.AddProperty(onePetToPerson);
        person.AddProperty(onePetToPerson.Partner);

        var zeroOrOnePetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ZeroOrOnePetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
            new EdmNavigationPropertyInfo() { Name = "ToZeroOrOnePet", Target = pet, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true });
        pet.AddProperty(zeroOrOnePetToPerson);
        person.AddProperty(zeroOrOnePetToPerson.Partner);

        var manyPetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ManyPetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
            new EdmNavigationPropertyInfo() { Name = "ToManyPet", Target = pet, TargetMultiplicity = EdmMultiplicity.Many, ContainsTarget = true });
        pet.AddProperty(manyPetToPerson);
        person.AddProperty(manyPetToPerson.Partner);

        var actualCsdls = new[] { @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToOnePet"" Type=""NS.Pet"" Nullable=""false"" Partner=""OnePetToPerson"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ToZeroOrOnePet"" Type=""NS.Pet"" Partner=""ZeroOrOnePetToPerson"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ToManyPet"" Type=""Collection(NS.Pet)"" Partner=""ManyPetToPerson"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""OnePetToPerson"" Type=""NS.Person"" Partner=""ToOnePet"" />
    <NavigationProperty Name=""ZeroOrOnePetToPerson"" Type=""NS.Person"" Partner=""ToZeroOrOnePet"" />
    <NavigationProperty Name=""ManyPetToPerson"" Type=""NS.Person"" Partner=""ToManyPet""  />
  </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var expectedCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        var expectXElements = updatedExpectedCsdls.ToList();
        var actualXElements = updatedActualCsdls.ToList();

        Assert.Equal(expectXElements.Count(), actualXElements.Count());

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        // compare just the EntityContainers
        CsdlXElementComparer.Compare(expectedContainers, actualContainers);

        foreach (var expectXElement in expectXElements)
        {
            var schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace").Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace").Value));

            Assert.NotNull(actualXElement);

            CsdlXElementComparer.Compare(expectXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_NavigationWithValidZeroOrOneMultiplicityRecursiveContainmentEnd_GeneratesExpectedCsdl(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var friendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.Many, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToFriend", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
        person.AddProperty(friendToPerson);
        person.AddProperty(friendToPerson.Partner);

        var parentToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToSelf", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToParent", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
        person.AddProperty(parentToPerson);
        person.AddProperty(parentToPerson.Partner);

        var actualCsdls = new[] { @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""Collection(NS.Person)"" Partner=""ToFriend"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ToFriend"" Type=""NS.Person"" Partner=""ToPerson"" />
    <NavigationProperty Name=""ToSelf"" Type=""NS.Person"" Partner=""ToParent"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ToParent"" Type=""NS.Person"" Partner=""ToSelf"" />
  </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var expectedCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        var expectXElements = updatedExpectedCsdls.ToList();
        var actualXElements = updatedActualCsdls.ToList();

        Assert.Equal(expectXElements.Count(), actualXElements.Count());

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        // compare just the EntityContainers
        CsdlXElementComparer.Compare(expectedContainers, actualContainers);

        foreach (var expectXElement in expectXElements)
        {
            var schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace").Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace").Value));

            Assert.NotNull(actualXElement);

            CsdlXElementComparer.Compare(expectXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_NavigationWithInvalidZeroOrOneMultiplicityRecursiveContainmentEnd_GeneratesExpectedCsdl(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var friendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToFriend", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
        person.AddProperty(friendToPerson);
        person.AddProperty(friendToPerson.Partner);

        var actualCsdls = new[] { @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToFriend"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ToFriend"" Type=""NS.Person"" Partner=""ToPerson""  />
  </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var expectedCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        var expectXElements = updatedExpectedCsdls.ToList();
        var actualXElements = updatedActualCsdls.ToList();

        Assert.Equal(expectXElements.Count(), actualXElements.Count());

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        // compare just the EntityContainers
        CsdlXElementComparer.Compare(expectedContainers, actualContainers);

        foreach (var expectXElement in expectXElements)
        {
            var schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace").Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace").Value));

            Assert.NotNull(actualXElement);

            CsdlXElementComparer.Compare(expectXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_NavigationWithOneMultiplicityRecursiveContainmentEnd_GeneratesExpectedCsdl(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var manyFriendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ManyFriendToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToManyFriend", Target = person, TargetMultiplicity = EdmMultiplicity.Many, ContainsTarget = true });
        person.AddProperty(manyFriendToPerson);
        person.AddProperty(manyFriendToPerson.Partner);

        var zeroOrOneFriendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ZeroOrOneFriendToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToZeroOrOneFriend", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true });
        person.AddProperty(zeroOrOneFriendToPerson);
        person.AddProperty(zeroOrOneFriendToPerson.Partner);

        var oneFriendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "OneFriendToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToOneFriend", Target = person, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        person.AddProperty(oneFriendToPerson);
        person.AddProperty(oneFriendToPerson.Partner);

        var actualCsdls = new[] { @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ManyFriendToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToManyFriend"" />
    <NavigationProperty Name=""ToManyFriend"" Type=""Collection(NS.Person)"" Partner=""ManyFriendToPerson"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ZeroOrOneFriendToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToZeroOrOneFriend"" />
    <NavigationProperty Name=""ToZeroOrOneFriend"" Type=""NS.Person"" Partner=""ZeroOrOneFriendToPerson"" ContainsTarget=""true"" />
    <NavigationProperty Name=""OneFriendToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToOneFriend"" />
    <NavigationProperty Name=""ToOneFriend"" Type=""NS.Person"" Nullable=""false"" Partner=""OneFriendToPerson"" ContainsTarget=""true"" />
  </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var expectedCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        var expectXElements = updatedExpectedCsdls.ToList();
        var actualXElements = updatedActualCsdls.ToList();

        Assert.Equal(expectXElements.Count(), actualXElements.Count());

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        // compare just the EntityContainers
        CsdlXElementComparer.Compare(expectedContainers, actualContainers);

        foreach (var expectXElement in expectXElements)
        {
            var schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace").Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace").Value));

            Assert.NotNull(actualXElement);

            CsdlXElementComparer.Compare(expectXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_NavigationWithManyMultiplicityRecursiveContainmentEnd_GeneratesExpectedCsdl(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var manyFriendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ManyFriendToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "ToManyFriend", Target = person, TargetMultiplicity = EdmMultiplicity.Many, ContainsTarget = true });
        person.AddProperty(manyFriendToPerson);
        person.AddProperty(manyFriendToPerson.Partner);

        var zeroOrOneFriendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ZeroOrOneFriendToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "ToZeroOrOneFriend", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true });
        person.AddProperty(zeroOrOneFriendToPerson);
        person.AddProperty(zeroOrOneFriendToPerson.Partner);

        var oneFriendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "OneFriendToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "ToOneFriend", Target = person, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        person.AddProperty(oneFriendToPerson);
        person.AddProperty(oneFriendToPerson.Partner);

        var actualCsdls = new[] { @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ManyFriendToPerson"" Type=""Collection(NS.Person)"" Partner=""ToManyFriend""  />
    <NavigationProperty Name=""ToManyFriend"" Type=""Collection(NS.Person)"" Partner=""ManyFriendToPerson"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ZeroOrOneFriendToPerson"" Type=""Collection(NS.Person)"" Partner=""ToZeroOrOneFriend"" />
    <NavigationProperty Name=""ToZeroOrOneFriend"" Type=""NS.Person"" Partner=""ZeroOrOneFriendToPerson""  ContainsTarget=""true"" />
    <NavigationProperty Name=""OneFriendToPerson"" Type=""Collection(NS.Person)"" Partner=""ToOneFriend"" />
    <NavigationProperty Name=""ToOneFriend"" Type=""NS.Person"" Nullable=""false"" Partner=""OneFriendToPerson""  ContainsTarget=""true"" />
  </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var expectedCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        var expectXElements = updatedExpectedCsdls.ToList();
        var actualXElements = updatedActualCsdls.ToList();

        Assert.Equal(expectXElements.Count(), actualXElements.Count());

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        // compare just the EntityContainers
        CsdlXElementComparer.Compare(expectedContainers, actualContainers);

        foreach (var expectXElement in expectXElements)
        {
            var schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace").Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace").Value));

            Assert.NotNull(actualXElement);

            CsdlXElementComparer.Compare(expectXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_SingleSimpleContainmentNavigation_GeneratesExpectedCsdl(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var pet = new EdmEntityType("NS", "Pet");
        var petId = new EdmStructuralProperty(pet, "Id", EdmCoreModel.Instance.GetInt32(false));
        pet.AddProperty(petId);
        pet.AddKeys(petId);
        model.AddElement(pet);

        var home = new EdmEntityType("NS", "Home");
        var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        home.AddKeys(homeId);
        model.AddElement(home);

        var personToPet = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One });
        pet.AddProperty(personToPet.Partner);
        person.AddProperty(personToPet);

        var homeToPet = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One });
        pet.AddProperty(homeToPet.Partner);
        home.AddProperty(homeToPet);

        var container = new EdmEntityContainer("NS", "Container");
        var personSet = container.AddEntitySet("PersonSet", person);
        var petSet = container.AddEntitySet("PetSet", pet);
        var homeSet = container.AddEntitySet("HomeSet", home);

        petSet.AddNavigationTarget(personToPet.Partner, personSet);
        petSet.AddNavigationTarget(homeToPet.Partner, homeSet);

        model.AddElement(container);

        var actualCsdls = new[] { @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPet"" Type=""NS.Pet"" Nullable=""false"" Partner=""ToPerson"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToPet"" />
    <NavigationProperty Name=""ToHome"" Type=""NS.Home"" Nullable=""false"" Partner=""ToPet"" />
  </EntityType>
  <EntityType Name=""Home"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPet"" Type=""NS.Pet"" Nullable=""false"" Partner=""ToHome""  ContainsTarget=""true"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""PersonSet"" EntityType=""NS.Person""/>
    <EntitySet Name=""PetSet"" EntityType=""NS.Pet"">
      <NavigationPropertyBinding Path=""ToPerson"" Target=""PersonSet"" />
      <NavigationPropertyBinding Path=""ToHome"" Target=""HomeSet"" />
    </EntitySet>
    <EntitySet Name=""HomeSet"" EntityType=""NS.Home"" />
  </EntityContainer>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var expectedCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        var expectXElements = updatedExpectedCsdls.ToList();
        var actualXElements = updatedActualCsdls.ToList();

        Assert.Equal(expectXElements.Count(), actualXElements.Count());

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        // compare just the EntityContainers
        CsdlXElementComparer.Compare(expectedContainers, actualContainers);

        foreach (var expectXElement in expectXElements)
        {
            var schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace").Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace").Value));

            Assert.NotNull(actualXElement);

            CsdlXElementComparer.Compare(expectXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_TwoContainmentNavigationsWithSameEnd_GeneratesExpectedCsdl(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var pet = new EdmEntityType("NS", "Pet");
        var petId = new EdmStructuralProperty(pet, "Id", EdmCoreModel.Instance.GetInt32(false));
        pet.AddProperty(petId);
        pet.AddKeys(petId);
        model.AddElement(pet);

        var home = new EdmEntityType("NS", "Home");
        var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        home.AddKeys(homeId);
        model.AddElement(home);

        var personToPet = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One });
        pet.AddProperty(personToPet.Partner);
        person.AddProperty(personToPet);

        var homeToPet = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One });
        pet.AddProperty(homeToPet.Partner);
        home.AddProperty(homeToPet);

        var container = new EdmEntityContainer("NS", "Container");
        var personSet = container.AddEntitySet("PersonSet", person);
        var petSet = container.AddEntitySet("PetSet", pet);
        var homeSet = container.AddEntitySet("HomeSet", home);

        petSet.AddNavigationTarget(personToPet.Partner, personSet);
        petSet.AddNavigationTarget(homeToPet.Partner, homeSet);

        model.AddElement(container);

        var actualCsdls = new[] { @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPet"" Type=""NS.Pet"" Nullable=""false"" Partner=""ToPerson"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToPet"" />
    <NavigationProperty Name=""ToHome"" Type=""NS.Home"" Nullable=""false"" Partner=""ToPet"" />
  </EntityType>
  <EntityType Name=""Home"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPet"" Type=""NS.Pet"" Nullable=""false"" Partner=""ToHome"" ContainsTarget=""true"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""PersonSet"" EntityType=""NS.Person""/>
    <EntitySet Name=""PetSet"" EntityType=""NS.Pet"">
      <NavigationPropertyBinding Path=""ToPerson"" Target=""PersonSet"" />
      <NavigationPropertyBinding Path=""ToHome"" Target=""HomeSet"" />
    </EntitySet>
    <EntitySet Name=""HomeSet"" EntityType=""NS.Home""/>
  </EntityContainer>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var expectedCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        var expectXElements = updatedExpectedCsdls.ToList();
        var actualXElements = updatedActualCsdls.ToList();

        Assert.Equal(expectXElements.Count(), actualXElements.Count());

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        // compare just the EntityContainers
        CsdlXElementComparer.Compare(expectedContainers, actualContainers);

        foreach (var expectXElement in expectXElements)
        {
            var schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace").Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace").Value));

            Assert.NotNull(actualXElement);

            CsdlXElementComparer.Compare(expectXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_TwoContainmentNavigationsWithSameEndAddedDifferently_GeneratesExpectedCsdl(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var pet = new EdmEntityType("NS", "Pet");
        var petId = new EdmStructuralProperty(pet, "Id", EdmCoreModel.Instance.GetInt32(false));
        pet.AddProperty(petId);
        pet.AddKeys(petId);
        model.AddElement(pet);

        var home = new EdmEntityType("NS", "Home");
        var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        home.AddKeys(homeId);
        model.AddElement(home);

        var personToPet = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One });
        pet.AddProperty(personToPet.Partner);
        person.AddProperty(personToPet);

        var homeToPet = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One });
        pet.AddProperty(homeToPet.Partner);
        home.AddProperty(homeToPet);

        var container = new EdmEntityContainer("NS", "Container");
        var personSet = container.AddEntitySet("PersonSet", person);
        var petSet = container.AddEntitySet("PetSet", pet);
        var homeSet = container.AddEntitySet("HomeSet", home);

        // [EdmLib] EntitySet.AddNavigationTarget() ordering matters and results into some validation not appearing
        petSet.AddNavigationTarget(personToPet.Partner, personSet);
        petSet.AddNavigationTarget(homeToPet.Partner, homeSet);

        model.AddElement(container);

        var actualCsdls = new[] { @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPet"" Type=""NS.Pet"" Nullable=""false"" Partner=""ToPerson"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToPet"" />
    <NavigationProperty Name=""ToHome"" Type=""NS.Home"" Nullable=""false"" Partner=""ToPet"" />
  </EntityType>
  <EntityType Name=""Home"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPet"" Type=""NS.Pet"" Nullable=""false"" Partner=""ToHome"" ContainsTarget=""true"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""PersonSet"" EntityType=""NS.Person""/>
    <EntitySet Name=""PetSet"" EntityType=""NS.Pet"">
      <NavigationPropertyBinding Path=""ToPerson"" Target=""PersonSet"" />
      <NavigationPropertyBinding Path=""ToHome"" Target=""HomeSet"" />
    </EntitySet>
    <EntitySet Name=""HomeSet"" EntityType=""NS.Home""/>
  </EntityContainer>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var expectedCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        var expectXElements = updatedExpectedCsdls.ToList();
        var actualXElements = updatedActualCsdls.ToList();

        Assert.Equal(expectXElements.Count(), actualXElements.Count());

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        // compare just the EntityContainers
        CsdlXElementComparer.Compare(expectedContainers, actualContainers);

        foreach (var expectXElement in expectXElements)
        {
            var schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace").Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace").Value));

            Assert.NotNull(actualXElement);

            CsdlXElementComparer.Compare(expectXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_ContainmentNavigationWithDifferentEnds_GeneratesExpectedCsdl(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var pet = new EdmEntityType("NS", "Pet");
        var petId = new EdmStructuralProperty(pet, "Id", EdmCoreModel.Instance.GetInt32(false));
        pet.AddProperty(petId);
        pet.AddKeys(petId);
        model.AddElement(pet);

        var home = new EdmEntityType("NS", "Home");
        var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        home.AddKeys(homeId);
        model.AddElement(home);

        var petToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        pet.AddProperty(petToPerson);
        person.AddProperty(petToPerson.Partner);

        var homeToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        person.AddProperty(homeToPerson.Partner);
        home.AddProperty(homeToPerson);

        var container = new EdmEntityContainer("NS", "Container");
        var personSet = container.AddEntitySet("PersonSet", person);
        var petSet = container.AddEntitySet("PetSet", pet);
        var homeSet = container.AddEntitySet("HomeSet", home);

        petSet.AddNavigationTarget(petToPerson, personSet);
        homeSet.AddNavigationTarget(homeToPerson, personSet);
        model.AddElement(container);

        var actualCsdls = new[] { @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPet"" Type=""NS.Pet"" Nullable=""false"" Partner=""ToPerson"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ToHome"" Type=""NS.Home"" Nullable=""false"" Partner=""ToPerson"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToPet"" />
  </EntityType>
  <EntityType Name=""Home"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToHome"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""PersonSet"" EntityType=""NS.Person""/>
    <EntitySet Name=""PetSet"" EntityType=""NS.Pet"">
      <NavigationPropertyBinding Path=""ToPerson"" Target=""PersonSet"" />
    </EntitySet>
    <EntitySet Name=""HomeSet"" EntityType=""NS.Home"">
      <NavigationPropertyBinding Path=""ToPerson"" Target=""PersonSet"" />
    </EntitySet>
  </EntityContainer>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var expectedCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        var expectXElements = updatedExpectedCsdls.ToList();
        var actualXElements = updatedActualCsdls.ToList();

        Assert.Equal(expectXElements.Count(), actualXElements.Count());

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        // compare just the EntityContainers
        CsdlXElementComparer.Compare(expectedContainers, actualContainers);

        foreach (var expectXElement in expectXElements)
        {
            var schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace").Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace").Value));

            Assert.NotNull(actualXElement);

            CsdlXElementComparer.Compare(expectXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_RecursiveThreeContainmentNavigations_GeneratesExpectedCsdl(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var pet = new EdmEntityType("NS", "Pet");
        var petId = new EdmStructuralProperty(pet, "Id", EdmCoreModel.Instance.GetInt32(false));
        pet.AddProperty(petId);
        pet.AddKeys(petId);
        model.AddElement(pet);

        var home = new EdmEntityType("NS", "Home");
        var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        home.AddKeys(homeId);
        model.AddElement(home);

        var personToPet = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One });
        pet.AddProperty(personToPet.Partner);
        person.AddProperty(personToPet);

        var homeToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One });
        person.AddProperty(homeToPerson.Partner);
        home.AddProperty(homeToPerson);

        var petToHome = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One });
        pet.AddProperty(petToHome);
        home.AddProperty(petToHome.Partner);

        var actualCsdls = new[] { @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPet"" Type=""NS.Pet"" Nullable=""false"" Partner=""ToPerson""  ContainsTarget=""true"" />
    <NavigationProperty Name=""ToHome"" Type=""NS.Home"" Nullable=""false"" Partner=""ToPerson"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToPet""  />
    <NavigationProperty Name=""ToHome"" Type=""NS.Home"" Nullable=""false"" Partner=""ToPet""  ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Home"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToHome"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ToPet"" Type=""NS.Pet"" Nullable=""false"" Partner=""ToHome"" />
  </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var expectedCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        var expectXElements = updatedExpectedCsdls.ToList();
        var actualXElements = updatedActualCsdls.ToList();

        Assert.Equal(expectXElements.Count(), actualXElements.Count());

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        // compare just the EntityContainers
        CsdlXElementComparer.Compare(expectedContainers, actualContainers);

        foreach (var expectXElement in expectXElements)
        {
            var schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace").Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace").Value));

            Assert.NotNull(actualXElement);

            CsdlXElementComparer.Compare(expectXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_RecursiveThreeContainmentNavigationsWithEntitySet_GeneratesExpectedCsdl(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var pet = new EdmEntityType("NS", "Pet");
        var petId = new EdmStructuralProperty(pet, "Id", EdmCoreModel.Instance.GetInt32(false));
        pet.AddProperty(petId);
        pet.AddKeys(petId);
        model.AddElement(pet);

        var home = new EdmEntityType("NS", "Home");
        var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        home.AddKeys(homeId);
        model.AddElement(home);

        var personToPet = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One });
        pet.AddProperty(personToPet.Partner);
        person.AddProperty(personToPet);

        var homeToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One });
        person.AddProperty(homeToPerson.Partner);
        home.AddProperty(homeToPerson);

        var petToHome = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One });
        pet.AddProperty(petToHome);
        home.AddProperty(petToHome.Partner);

        var container = new EdmEntityContainer("NS", "Container");
        model.AddElement(container);

        var personSet = container.AddEntitySet("PersonSet", person);
        var petSet = container.AddEntitySet("PetSet", pet);
        var homeSet = container.AddEntitySet("HomeSet", home);

        personSet.AddNavigationTarget(personToPet, petSet);
        petSet.AddNavigationTarget(personToPet.Partner, personSet);
        homeSet.AddNavigationTarget(petToHome.Partner, petSet);
        personSet.AddNavigationTarget(homeToPerson.Partner, homeSet);

        var actualCsdls = new[] { @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPet"" Type=""NS.Pet"" Nullable=""false"" Partner=""ToPerson"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ToHome"" Type=""NS.Home"" Nullable=""false"" Partner=""ToPerson"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToPet"" />
    <NavigationProperty Name=""ToHome"" Type=""NS.Home"" Nullable=""false"" Partner=""ToPet"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Home"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToHome"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ToPet"" Type=""NS.Pet"" Nullable=""false"" Partner=""ToHome"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""PersonSet"" EntityType=""NS.Person"">
      <NavigationPropertyBinding Path=""ToHome"" Target=""HomeSet"" />
    </EntitySet>
    <EntitySet Name=""PetSet"" EntityType=""NS.Pet"">
      <NavigationPropertyBinding Path=""ToPerson"" Target=""PersonSet"" />
    </EntitySet>
    <EntitySet Name=""HomeSet"" EntityType=""NS.Home"">
      <NavigationPropertyBinding Path=""ToPet"" Target=""PetSet"" />
    </EntitySet>
  </EntityContainer>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var expectedCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        var expectXElements = updatedExpectedCsdls.ToList();
        var actualXElements = updatedActualCsdls.ToList();

        Assert.Equal(expectXElements.Count(), actualXElements.Count());

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        // compare just the EntityContainers
        CsdlXElementComparer.Compare(expectedContainers, actualContainers);

        foreach (var expectXElement in expectXElements)
        {
            var schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace").Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace").Value));

            Assert.NotNull(actualXElement);

            CsdlXElementComparer.Compare(expectXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_RecursiveOneContainmentNavigationSelfPointingEntitySet_GeneratesExpectedCsdl(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var personToFriends = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToFriend", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
        person.AddProperty(personToFriends.Partner);
        person.AddProperty(personToFriends);

        var container = new EdmEntityContainer("NS", "Container");
        model.AddElement(container);
        var personSet = container.AddEntitySet("PersonSet", person);
        // personSet.AddNavigationTarget(personToFriends, personSet);
        personSet.AddNavigationTarget(personToFriends.Partner, personSet);

        var actualCsdls = new[] { @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Partner=""ToFriend""  />
    <NavigationProperty Name=""ToFriend"" Type=""NS.Person"" Partner=""ToPerson""  ContainsTarget=""true"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""PersonSet"" EntityType=""NS.Person"">
      <NavigationPropertyBinding Path=""ToPerson"" Target=""PersonSet"" />
    </EntitySet>
  </EntityContainer>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var expectedCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        var expectXElements = updatedExpectedCsdls.ToList();
        var actualXElements = updatedActualCsdls.ToList();

        Assert.Equal(expectXElements.Count(), actualXElements.Count());

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        // compare just the EntityContainers
        CsdlXElementComparer.Compare(expectedContainers, actualContainers);

        foreach (var expectXElement in expectXElements)
        {
            var schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace").Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace").Value));

            Assert.NotNull(actualXElement);

            CsdlXElementComparer.Compare(expectXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_RecursiveOneContainmentNavigationWithTwoEntitySets_GeneratesExpectedCsdl(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var personToFriends = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToFriend", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
        person.AddProperty(personToFriends.Partner);
        person.AddProperty(personToFriends);

        var container = new EdmEntityContainer("NS", "Container");
        model.AddElement(container);
        var personSet = container.AddEntitySet("PersonSet", person);
        var friendSet = container.AddEntitySet("FriendSet", person);
        personSet.AddNavigationTarget(personToFriends, friendSet);
        friendSet.AddNavigationTarget(personToFriends.Partner, personSet);

        var actualCsdls = new[] { @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Partner=""ToFriend"" />
    <NavigationProperty Name=""ToFriend"" Type=""NS.Person"" Partner=""ToPerson"" ContainsTarget=""true"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""FriendSet"" EntityType=""NS.Person"">
      <NavigationPropertyBinding Path=""ToPerson"" Target=""PersonSet"" />
    </EntitySet>
    <EntitySet Name=""PersonSet"" EntityType=""NS.Person""/>
  </EntityContainer>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var expectedCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        var expectXElements = updatedExpectedCsdls.ToList();
        var actualXElements = updatedActualCsdls.ToList();

        Assert.Equal(expectXElements.Count(), actualXElements.Count());

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        // compare just the EntityContainers
        CsdlXElementComparer.Compare(expectedContainers, actualContainers);

        foreach (var expectXElement in expectXElements)
        {
            var schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace").Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace").Value));

            Assert.NotNull(actualXElement);

            CsdlXElementComparer.Compare(expectXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_DerivedContainmentNavigationWithBaseAssociationSet_GeneratesExpectedCsdl(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var employee = new EdmEntityType("NS", "Employee", person);
        model.AddElement(employee);

        var home = new EdmEntityType("NS", "Home");
        var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        home.AddKeys(homeId);
        model.AddElement(home);

        var office = new EdmEntityType("NS", "Office", home);
        model.AddElement(office);

        var officeToEmployee = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToEmployee", Target = employee, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToOffice", Target = office, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        employee.AddProperty(officeToEmployee.Partner);
        office.AddProperty(officeToEmployee);

        var homeToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        person.AddProperty(homeToPerson.Partner);
        home.AddProperty(homeToPerson);

        var container = new EdmEntityContainer("NS", "Container");
        model.AddElement(container);

        var personSet = container.AddEntitySet("PersonSet", person);
        var employeeSet = container.AddEntitySet("EmployeeSet", employee);
        var homeSet = container.AddEntitySet("HomeSet", home);
        var officeSet = container.AddEntitySet("OfficeSet", office);

        officeSet.AddNavigationTarget(officeToEmployee, personSet);
        homeSet.AddNavigationTarget(homeToPerson, personSet);

        var actualCsdls = new[] { @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToHome"" Type=""NS.Home"" Nullable=""false"" Partner=""ToPerson""  ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Employee"" BaseType=""NS.Person"">
    <NavigationProperty Name=""ToOffice"" Type=""NS.Office"" Nullable=""false"" Partner=""ToEmployee"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Home"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToHome""  />
  </EntityType>
  <EntityType Name=""Office"" BaseType=""NS.Home"">
    <NavigationProperty Name=""ToEmployee"" Type=""NS.Employee"" Nullable=""false"" Partner=""ToOffice""  />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""PersonSet"" EntityType=""NS.Person""/>
    <EntitySet Name=""EmployeeSet"" EntityType=""NS.Employee"" />
    <EntitySet Name=""HomeSet"" EntityType=""NS.Home"">
      <NavigationPropertyBinding Path=""ToPerson"" Target=""PersonSet"" />
    </EntitySet>
    <EntitySet Name=""OfficeSet"" EntityType=""NS.Office"">
      <NavigationPropertyBinding Path=""ToEmployee"" Target=""PersonSet"" />
    </EntitySet>
  </EntityContainer>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var expectedCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        var expectXElements = updatedExpectedCsdls.ToList();
        var actualXElements = updatedActualCsdls.ToList();

        Assert.Equal(expectXElements.Count(), actualXElements.Count());

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        // compare just the EntityContainers
        CsdlXElementComparer.Compare(expectedContainers, actualContainers);

        foreach (var expectXElement in expectXElements)
        {
            var schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace").Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace").Value));

            Assert.NotNull(actualXElement);

            CsdlXElementComparer.Compare(expectXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_DerivedContainmentNavigationWithDerivedAssociationSet_GeneratesExpectedCsdl(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var employee = new EdmEntityType("NS", "Employee", person);
        model.AddElement(employee);

        var home = new EdmEntityType("NS", "Home");
        var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        home.AddKeys(homeId);
        model.AddElement(home);

        var office = new EdmEntityType("NS", "Office", home);
        model.AddElement(office);

        var officeToEmployee = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToEmployee", Target = employee, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToOffice", Target = office, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        employee.AddProperty(officeToEmployee.Partner);
        office.AddProperty(officeToEmployee);

        var homeToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        person.AddProperty(homeToPerson.Partner);
        home.AddProperty(homeToPerson);

        var container = new EdmEntityContainer("NS", "Container");
        model.AddElement(container);

        var personSet = container.AddEntitySet("PersonSet", person);
        var employeeSet = container.AddEntitySet("EmployeeSet", employee);
        var homeSet = container.AddEntitySet("HomeSet", home);
        var officeSet = container.AddEntitySet("OfficeSet", office);

        officeSet.AddNavigationTarget(homeToPerson, employeeSet);
        homeSet.AddNavigationTarget(homeToPerson, personSet);

        var actualCsdls = new[] { @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToHome"" Type=""NS.Home"" Nullable=""false"" Partner=""ToPerson"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Employee"" BaseType=""NS.Person"">
    <NavigationProperty Name=""ToOffice"" Type=""NS.Office"" Nullable=""false"" Partner=""ToEmployee"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Home"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToHome"" />
  </EntityType>
  <EntityType Name=""Office"" BaseType=""NS.Home"">
    <NavigationProperty Name=""ToEmployee"" Type=""NS.Employee"" Nullable=""false"" Partner=""ToOffice"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""PersonSet"" EntityType=""NS.Person"" />
    <EntitySet Name=""EmployeeSet"" EntityType=""NS.Employee""/>
    <EntitySet Name=""HomeSet"" EntityType=""NS.Home"" >
      <NavigationPropertyBinding Path=""ToPerson"" Target=""PersonSet"" />
    </EntitySet>
    <EntitySet Name=""OfficeSet"" EntityType=""NS.Office"">
      <NavigationPropertyBinding Path=""ToPerson"" Target=""EmployeeSet"" />
    </EntitySet>
  </EntityContainer>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var expectedCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        var expectXElements = updatedExpectedCsdls.ToList();
        var actualXElements = updatedActualCsdls.ToList();

        Assert.Equal(expectXElements.Count(), actualXElements.Count());

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        // compare just the EntityContainers
        CsdlXElementComparer.Compare(expectedContainers, actualContainers);

        foreach (var expectXElement in expectXElements)
        {
            var schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace").Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace").Value));

            Assert.NotNull(actualXElement);

            CsdlXElementComparer.Compare(expectXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_DerivedContainmentNavigationWithDerivedAndBaseAssociationSet_GeneratesExpectedCsdl(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var person = new EdmEntityType("NS", "Person");
        var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        var employee = new EdmEntityType("NS", "Employee", person);
        model.AddElement(employee);

        var home = new EdmEntityType("NS", "Home");
        var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        home.AddKeys(homeId);
        model.AddElement(home);

        var office = new EdmEntityType("NS", "Office", home);
        model.AddElement(office);

        var officeToEmployee = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToEmployee", Target = employee, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToOffice", Target = office, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        employee.AddProperty(officeToEmployee.Partner);
        office.AddProperty(officeToEmployee);

        var homeToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        person.AddProperty(homeToPerson.Partner);
        home.AddProperty(homeToPerson);

        var container = new EdmEntityContainer("NS", "Container");
        model.AddElement(container);

        var personSet = container.AddEntitySet("PersonSet", person);
        var employeeSet = container.AddEntitySet("EmployeeSet", employee);
        var homeSet = container.AddEntitySet("HomeSet", home);
        var officeSet = container.AddEntitySet("OfficeSet", office);

        officeSet.AddNavigationTarget(officeToEmployee, personSet);
        //            personSet.AddNavigationTarget(officeToEmployee.Partner, officeSet);
        homeSet.AddNavigationTarget(homeToPerson, employeeSet);
        //            employeeSet.AddNavigationTarget(homeToPerson.Partner, homeSet);

        var actualCsdls = new[] { @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToHome"" Type=""NS.Home"" Nullable=""false"" Partner=""ToPerson"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Employee"" BaseType=""NS.Person"">
    <NavigationProperty Name=""ToOffice"" Type=""NS.Office"" Nullable=""false"" Partner=""ToEmployee"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Home"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToHome"" />
  </EntityType>
  <EntityType Name=""Office"" BaseType=""NS.Home"">
    <NavigationProperty Name=""ToEmployee"" Type=""NS.Employee"" Nullable=""false"" Partner=""ToOffice"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""PersonSet"" EntityType=""NS.Person"" />
    <EntitySet Name=""EmployeeSet"" EntityType=""NS.Employee"" />
    <EntitySet Name=""HomeSet"" EntityType=""NS.Home"">
      <NavigationPropertyBinding Path=""ToPerson"" Target=""EmployeeSet"" />
    </EntitySet>
    <EntitySet Name=""OfficeSet"" EntityType=""NS.Office"">
      <NavigationPropertyBinding Path=""ToEmployee"" Target=""PersonSet"" />
    </EntitySet>
  </EntityContainer>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var expectedCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        var expectXElements = updatedExpectedCsdls.ToList();
        var actualXElements = updatedActualCsdls.ToList();

        Assert.Equal(expectXElements.Count(), actualXElements.Count());

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        // compare just the EntityContainers
        CsdlXElementComparer.Compare(expectedContainers, actualContainers);

        foreach (var expectXElement in expectXElements)
        {
            var schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace").Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace").Value));

            Assert.NotNull(actualXElement);

            CsdlXElementComparer.Compare(expectXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_MultiBindingForOneNavigationProperty_GeneratesExpectedCsdl(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var entityType = new EdmEntityType("NS", "EntityType");
        var id = entityType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false));
        entityType.AddKeys(id);

        var containedEntityType = new EdmEntityType("NS", "ContainedEntityType");
        var containedId = containedEntityType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false));
        containedEntityType.AddKeys(containedId);

        var containedNav1 = entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
        {
            Name = "ContainedNav1",
            Target = containedEntityType,
            TargetMultiplicity = EdmMultiplicity.One,
            ContainsTarget = true
        });

        var containedNav2 = entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
        {
            Name = "ContainedNav2",
            Target = containedEntityType,
            TargetMultiplicity = EdmMultiplicity.One,
            ContainsTarget = true
        });

        var navEntityType = new EdmEntityType("NS", "NavEntityType");
        var navEntityId = navEntityType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false));
        navEntityType.AddKeys(navEntityId);

        var complex = new EdmComplexType("NS", "ComplexType");
        complex.AddStructuralProperty("Prop1", EdmCoreModel.Instance.GetInt32(false));

        var complxNavP = complex.AddUnidirectionalNavigation(
            new EdmNavigationPropertyInfo()
            {
                Name = "CollectionOfNavOnComplex",
                Target = navEntityType,
                TargetMultiplicity = EdmMultiplicity.Many,
            });

        entityType.AddStructuralProperty("complexProp1", new EdmComplexTypeReference(complex, false));
        entityType.AddStructuralProperty("complexProp2", new EdmComplexTypeReference(complex, false));

        var navOnContained = containedEntityType.AddUnidirectionalNavigation(
            new EdmNavigationPropertyInfo()
            {
                Name = "NavOnContained",
                Target = navEntityType,
                TargetMultiplicity = EdmMultiplicity.One,
            });

        model.AddElement(entityType);
        model.AddElement(containedEntityType);
        model.AddElement(navEntityType);
        model.AddElement(complex);

        var entityContainer = new EdmEntityContainer("NS", "Container");
        model.AddElement(entityContainer);
        var entitySet = new EdmEntitySet(entityContainer, "EntitySet", entityType);
        var navEntitySet1 = new EdmEntitySet(entityContainer, "NavEntitySet1", navEntityType);
        var navEntitySet2 = new EdmEntitySet(entityContainer, "NavEntitySet2", navEntityType);
        entitySet.AddNavigationTarget(complxNavP, navEntitySet1, new EdmPathExpression("complexProp1/CollectionOfNavOnComplex"));
        entitySet.AddNavigationTarget(complxNavP, navEntitySet2, new EdmPathExpression("complexProp2/CollectionOfNavOnComplex"));
        entitySet.AddNavigationTarget(navOnContained, navEntitySet1, new EdmPathExpression("ContainedNav1/NavOnContained"));
        entitySet.AddNavigationTarget(navOnContained, navEntitySet2, new EdmPathExpression("ContainedNav2/NavOnContained"));
        entityContainer.AddElement(entitySet);
        entityContainer.AddElement(navEntitySet1);
        entityContainer.AddElement(navEntitySet2);

        var csdl = "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                                "<EntityType Name=\"EntityType\">" +
                                    "<Key><PropertyRef Name=\"ID\" /></Key>" +
                                    "<Property Name=\"ID\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                                    "<Property Name=\"complexProp1\" Type=\"NS.ComplexType\" Nullable=\"false\" />" +
                                    "<Property Name=\"complexProp2\" Type=\"NS.ComplexType\" Nullable=\"false\" />" +
                                    "<NavigationProperty Name=\"ContainedNav1\" Type=\"NS.ContainedEntityType\" Nullable=\"false\" ContainsTarget=\"true\" />" +
                                    "<NavigationProperty Name=\"ContainedNav2\" Type=\"NS.ContainedEntityType\" Nullable=\"false\" ContainsTarget=\"true\" />" +
                                "</EntityType>" +
                                "<EntityType Name=\"ContainedEntityType\">" +
                                    "<Key><PropertyRef Name=\"ID\" /></Key>" +
                                    "<Property Name=\"ID\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                                    "<NavigationProperty Name=\"NavOnContained\" Type=\"NS.NavEntityType\" Nullable=\"false\" />" +
                                "</EntityType>" +
                                "<EntityType Name=\"NavEntityType\">" +
                                    "<Key><PropertyRef Name=\"ID\" /></Key>" +
                                    "<Property Name=\"ID\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                                "</EntityType>" +
                                "<ComplexType Name=\"ComplexType\">" +
                                    "<Property Name=\"Prop1\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                                    "<NavigationProperty Name=\"CollectionOfNavOnComplex\" Type=\"Collection(NS.NavEntityType)\" />" +
                                "</ComplexType>" +
                                "<EntityContainer Name=\"Container\">" +
                                "<EntitySet Name=\"EntitySet\" EntityType=\"NS.EntityType\">" +
                                    "<NavigationPropertyBinding Path=\"complexProp1/CollectionOfNavOnComplex\" Target=\"NavEntitySet1\" />" +
                                    "<NavigationPropertyBinding Path=\"complexProp2/CollectionOfNavOnComplex\" Target=\"NavEntitySet2\" />" +
                                    "<NavigationPropertyBinding Path=\"ContainedNav1/NavOnContained\" Target=\"NavEntitySet1\" />" +
                                    "<NavigationPropertyBinding Path=\"ContainedNav2/NavOnContained\" Target=\"NavEntitySet2\" />" +
                                "</EntitySet>" +
                                "<EntitySet Name=\"NavEntitySet1\" EntityType=\"NS.NavEntityType\" />" +
                                "<EntitySet Name=\"NavEntitySet2\" EntityType=\"NS.NavEntityType\" />" +
                                "</EntityContainer>" +
                            "</Schema>";

        var actualCsdls = new[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var expectedCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), edmVersion);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), edmVersion);

        var expectXElements = updatedExpectedCsdls.ToList();
        var actualXElements = updatedActualCsdls.ToList();

        Assert.Equal(expectXElements.Count(), actualXElements.Count());

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        // compare just the EntityContainers
        CsdlXElementComparer.Compare(expectedContainers, actualContainers);

        foreach (var expectXElement in expectXElements)
        {
            var schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace").Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace").Value));

            Assert.NotNull(actualXElement);

            CsdlXElementComparer.Compare(expectXElement, actualXElement);
        }
    }
}
