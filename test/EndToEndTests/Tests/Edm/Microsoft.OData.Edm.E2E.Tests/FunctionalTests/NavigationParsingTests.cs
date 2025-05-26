//---------------------------------------------------------------------
// <copyright file="NavigationParsingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class NavigationParsingTests : EdmLibTestCaseBase
{
    [Fact]
    public void Parse_NavigationWithBothContainmentEnds_ValidatesContainmentAndPartners()
    {
        var csdlElements = new[] { @"
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

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var personEntityType = model.FindType("NS.Person") as IEdmEntityType;
        Assert.Single(personEntityType.NavigationProperties());

        var petEntityType = model.FindType("NS.Pet") as IEdmEntityType;
        Assert.Single(petEntityType.NavigationProperties());

        var personToPet = (model.FindType("NS.Person") as IEdmEntityType).NavigationProperties().First();
        Assert.True(personToPet.ContainsTarget);
        Assert.True(personToPet.Partner.ContainsTarget);
    }

    [Fact]
    public void Parse_NavigationWithOneMultiplicityContainmentEnd_ValidatesMultiplicityAndContainment()
    {
        var csdlElements = new[] { @"
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

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var multiplicity = EdmMultiplicity.One;

        var personNavigations = (model.FindType("NS.Person") as IEdmEntityType).NavigationProperties();
        Assert.Equal(3, personNavigations.Count());

        var petNavigations = (model.FindType("NS.Pet") as IEdmEntityType).NavigationProperties();
        Assert.Equal(3, petNavigations.Count());

        var personToOnePet = personNavigations.Where(n => n.Name.Equals("ToOnePet")).First();
        Assert.True(personToOnePet.ContainsTarget);
        Assert.False(personToOnePet.Partner.ContainsTarget);

        Assert.Equal(multiplicity, personToOnePet.Partner.TargetMultiplicity());
        Assert.Equal(EdmMultiplicity.One, personToOnePet.TargetMultiplicity());

        var personToZeroOrOnePet = personNavigations.Where(n => n.Name.Equals("ToZeroOrOnePet")).First();
        Assert.True(personToZeroOrOnePet.ContainsTarget);
        Assert.False(personToZeroOrOnePet.Partner.ContainsTarget);

        // CheckNavigationMultiplicity
        Assert.Equal(multiplicity, personToZeroOrOnePet.Partner.TargetMultiplicity());
        Assert.Equal(EdmMultiplicity.ZeroOrOne, personToZeroOrOnePet.TargetMultiplicity());

        var personToManyPet = personNavigations.Where(n => n.Name.Equals("ToManyPet")).First();
        Assert.True(personToManyPet.ContainsTarget);
        Assert.False(personToManyPet.Partner.ContainsTarget);

        // CheckNavigationMultiplicity
        Assert.Equal(multiplicity, personToManyPet.Partner.TargetMultiplicity());
        Assert.Equal(EdmMultiplicity.Many, personToManyPet.TargetMultiplicity());

        var onePetToPerson = petNavigations.Where(n => n.Name.Equals("OnePetToPerson")).First();
        Assert.Equal(personToOnePet.Partner, onePetToPerson);

        var zeroOrOnePetToPerson = petNavigations.Where(n => n.Name.Equals("ZeroOrOnePetToPerson")).First();
        // CheckNavigationsArePartners
        Assert.Equal(personToZeroOrOnePet.Partner, zeroOrOnePetToPerson);
        Assert.Equal(personToZeroOrOnePet, zeroOrOnePetToPerson.Partner);

        var manyPetToPerson = petNavigations.Where(n => n.Name.Equals("ManyPetToPerson")).First();
        // CheckNavigationsArePartners
        Assert.Equal(personToManyPet.Partner, manyPetToPerson);
        Assert.Equal(personToManyPet, manyPetToPerson.Partner);
    }

    [Fact]
    public void Parse_NavigationWithManyMultiplicityContainmentEnd_ValidatesMultiplicityAndContainment()
    {
        var csdlElements = new[] { @"
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

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var multiplicity = EdmMultiplicity.Many;

        var personNavigations = (model.FindType("NS.Person") as IEdmEntityType).NavigationProperties();
        Assert.Equal(3, personNavigations.Count());

        var petNavigations = (model.FindType("NS.Pet") as IEdmEntityType).NavigationProperties();
        Assert.Equal(3, petNavigations.Count());

        var personToOnePet = personNavigations.Where(n => n.Name.Equals("ToOnePet")).First();
        Assert.True(personToOnePet.ContainsTarget);
        Assert.False(personToOnePet.Partner.ContainsTarget);

        Assert.Equal(multiplicity, personToOnePet.Partner.TargetMultiplicity());
        Assert.Equal(EdmMultiplicity.One, personToOnePet.TargetMultiplicity());

        var personToZeroOrOnePet = personNavigations.Where(n => n.Name.Equals("ToZeroOrOnePet")).First();
        Assert.True(personToZeroOrOnePet.ContainsTarget);
        Assert.False(personToZeroOrOnePet.Partner.ContainsTarget);

        // CheckNavigationMultiplicity
        Assert.Equal(multiplicity, personToZeroOrOnePet.Partner.TargetMultiplicity());
        Assert.Equal(EdmMultiplicity.ZeroOrOne, personToZeroOrOnePet.TargetMultiplicity());

        var personToManyPet = personNavigations.Where(n => n.Name.Equals("ToManyPet")).First();
        Assert.True(personToManyPet.ContainsTarget);
        Assert.False(personToManyPet.Partner.ContainsTarget);

        // CheckNavigationMultiplicity
        Assert.Equal(multiplicity, personToManyPet.Partner.TargetMultiplicity());
        Assert.Equal(EdmMultiplicity.Many, personToManyPet.TargetMultiplicity());

        var onePetToPerson = petNavigations.Where(n => n.Name.Equals("OnePetToPerson")).First();
        Assert.Equal(personToOnePet.Partner, onePetToPerson);

        var zeroOrOnePetToPerson = petNavigations.Where(n => n.Name.Equals("ZeroOrOnePetToPerson")).First();
        // CheckNavigationsArePartners
        Assert.Equal(personToZeroOrOnePet.Partner, zeroOrOnePetToPerson);
        Assert.Equal(personToZeroOrOnePet, zeroOrOnePetToPerson.Partner);

        var manyPetToPerson = petNavigations.Where(n => n.Name.Equals("ManyPetToPerson")).First();
        // CheckNavigationsArePartners
        Assert.Equal(personToManyPet.Partner, manyPetToPerson);
        Assert.Equal(personToManyPet, manyPetToPerson.Partner);
    }

    [Fact]
    public void Parse_NavigationWithZeroOrOneMultiplicityContainmentEnd_ValidatesMultiplicityAndContainment()
    {
        var csdlElements = new[] { @"
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

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var multiplicity = EdmMultiplicity.ZeroOrOne;

        var personNavigations = (model.FindType("NS.Person") as IEdmEntityType).NavigationProperties();
        Assert.Equal(3, personNavigations.Count());

        var petNavigations = (model.FindType("NS.Pet") as IEdmEntityType).NavigationProperties();
        Assert.Equal(3, petNavigations.Count());

        var personToOnePet = personNavigations.Where(n => n.Name.Equals("ToOnePet")).First();
        Assert.True(personToOnePet.ContainsTarget);
        Assert.False(personToOnePet.Partner.ContainsTarget);

        Assert.Equal(multiplicity, personToOnePet.Partner.TargetMultiplicity());
        Assert.Equal(EdmMultiplicity.One, personToOnePet.TargetMultiplicity());

        var personToZeroOrOnePet = personNavigations.Where(n => n.Name.Equals("ToZeroOrOnePet")).First();
        Assert.True(personToZeroOrOnePet.ContainsTarget);
        Assert.False(personToZeroOrOnePet.Partner.ContainsTarget);

        // CheckNavigationMultiplicity
        Assert.Equal(multiplicity, personToZeroOrOnePet.Partner.TargetMultiplicity());
        Assert.Equal(EdmMultiplicity.ZeroOrOne, personToZeroOrOnePet.TargetMultiplicity());

        var personToManyPet = personNavigations.Where(n => n.Name.Equals("ToManyPet")).First();
        Assert.True(personToManyPet.ContainsTarget);
        Assert.False(personToManyPet.Partner.ContainsTarget);

        // CheckNavigationMultiplicity
        Assert.Equal(multiplicity, personToManyPet.Partner.TargetMultiplicity());
        Assert.Equal(EdmMultiplicity.Many, personToManyPet.TargetMultiplicity());

        var onePetToPerson = petNavigations.Where(n => n.Name.Equals("OnePetToPerson")).First();
        Assert.Equal(personToOnePet.Partner, onePetToPerson);

        var zeroOrOnePetToPerson = petNavigations.Where(n => n.Name.Equals("ZeroOrOnePetToPerson")).First();
        // CheckNavigationsArePartners
        Assert.Equal(personToZeroOrOnePet.Partner, zeroOrOnePetToPerson);
        Assert.Equal(personToZeroOrOnePet, zeroOrOnePetToPerson.Partner);

        var manyPetToPerson = petNavigations.Where(n => n.Name.Equals("ManyPetToPerson")).First();
        // CheckNavigationsArePartners
        Assert.Equal(personToManyPet.Partner, manyPetToPerson);
        Assert.Equal(personToManyPet, manyPetToPerson.Partner);
    }

    [Fact]
    public void Parse_NavigationWithValidZeroOrOneMultiplicityRecursiveContainmentEnd_ValidatesContainmentAndPartners()
    {
        var csdlElements = new[] { @"
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

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var personNavigations = (model.FindType("NS.Person") as IEdmEntityType).NavigationProperties();
        Assert.Equal(4, personNavigations.Count());

        var personToFriend = personNavigations.Where(n => n.Name.Equals("ToFriend")).First();
        Assert.False(personToFriend.ContainsTarget);
        Assert.True(personToFriend.Partner.ContainsTarget);

        // CheckNavigationMultiplicity
        Assert.Equal(EdmMultiplicity.Many, personToFriend.Partner.TargetMultiplicity());
        Assert.Equal(EdmMultiplicity.ZeroOrOne, personToFriend.TargetMultiplicity());

        var personToParent = personNavigations.Where(n => n.Name.Equals("ToParent")).First();
        Assert.False(personToFriend.ContainsTarget);
        Assert.True(personToFriend.Partner.ContainsTarget);

        // CheckNavigationMultiplicity
        Assert.Equal(EdmMultiplicity.ZeroOrOne, personToParent.Partner.TargetMultiplicity());
        Assert.Equal(EdmMultiplicity.ZeroOrOne, personToParent.TargetMultiplicity());

        var friendToPerson = personNavigations.Where(n => n.Name.Equals("ToPerson")).First();
        // CheckNavigationsArePartners
        Assert.Equal(personToFriend.Partner, friendToPerson);
        Assert.Equal(personToFriend, friendToPerson.Partner);

        var parentToPerson = personNavigations.Where(n => n.Name.Equals("ToSelf")).First();
        // CheckNavigationsArePartners
        Assert.Equal(personToParent.Partner, parentToPerson);
        Assert.Equal(personToParent, parentToPerson.Partner);
    }

    [Fact]
    public void Parse_NavigationWithInvalidZeroOrOneMultiplicityRecursiveContainmentEnd_ValidatesContainmentAndPartners()
    {
        var csdlElements = new[] { @"
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

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var personNavigations = (model.FindType("NS.Person") as IEdmEntityType).NavigationProperties();
        Assert.Equal(2, personNavigations.Count());

        var personToFriend = personNavigations.Where(n => n.Name.Equals("ToFriend")).First();
        Assert.False(personToFriend.ContainsTarget);
        Assert.True(personToFriend.Partner.ContainsTarget);

        // CheckNavigationMultiplicity
        Assert.Equal(EdmMultiplicity.One, personToFriend.Partner.TargetMultiplicity());
        Assert.Equal(EdmMultiplicity.ZeroOrOne, personToFriend.TargetMultiplicity());

        var friendToPerson = personNavigations.Where(n => n.Name.Equals("ToPerson")).First();
        // CheckNavigationsArePartners
        Assert.Equal(personToFriend.Partner, friendToPerson);
        Assert.Equal(personToFriend, friendToPerson.Partner);
    }

    [Fact]
    public void Parse_NavigationWithOneMultiplicityRecursiveContainmentEnd_ValidatesContainmentAndPartners()
    {
        var csdlElements = new[] { @"
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

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var personNavigations = (model.FindType("NS.Person") as IEdmEntityType).NavigationProperties();
        Assert.Equal(6, personNavigations.Count());

        var manyFriendToPerson = personNavigations.Where(n => n.Name.Equals("ManyFriendToPerson")).First();
        Assert.False(manyFriendToPerson.ContainsTarget);
        Assert.True(manyFriendToPerson.Partner.ContainsTarget);

        // CheckNavigationMultiplicity
        Assert.Equal(EdmMultiplicity.Many, manyFriendToPerson.Partner.TargetMultiplicity());
        Assert.Equal(EdmMultiplicity.One, manyFriendToPerson.TargetMultiplicity());

        var zeroOrOneFriendToPerson = personNavigations.Where(n => n.Name.Equals("ZeroOrOneFriendToPerson")).First();
        // CheckNavigationContainment
        Assert.False(zeroOrOneFriendToPerson.ContainsTarget);
        Assert.True(zeroOrOneFriendToPerson.Partner.ContainsTarget);

        // CheckNavigationMultiplicity
        Assert.Equal(EdmMultiplicity.ZeroOrOne, zeroOrOneFriendToPerson.Partner.TargetMultiplicity());
        Assert.Equal(EdmMultiplicity.One, zeroOrOneFriendToPerson.TargetMultiplicity());

        var oneFriendToPerson = personNavigations.Where(n => n.Name.Equals("OneFriendToPerson")).First();
        Assert.False(oneFriendToPerson.ContainsTarget);
        Assert.True(oneFriendToPerson.Partner.ContainsTarget);

        // CheckNavigationMultiplicity
        Assert.Equal(EdmMultiplicity.One, oneFriendToPerson.Partner.TargetMultiplicity());
        Assert.Equal(EdmMultiplicity.One, oneFriendToPerson.TargetMultiplicity());

        var toManyFriend = personNavigations.Where(n => n.Name.Equals("ToManyFriend")).First();
        // CheckNavigationsArePartners
        Assert.Equal(manyFriendToPerson.Partner, toManyFriend);
        Assert.Equal(manyFriendToPerson, toManyFriend.Partner);

        var toZeroOrOneFriend = personNavigations.Where(n => n.Name.Equals("ToZeroOrOneFriend")).First();
        // CheckNavigationsArePartners
        Assert.Equal(zeroOrOneFriendToPerson.Partner, toZeroOrOneFriend);
        Assert.Equal(zeroOrOneFriendToPerson, toZeroOrOneFriend.Partner);

        var toOneFriend = personNavigations.Where(n => n.Name.Equals("ToOneFriend")).First();
        // CheckNavigationsArePartners
        Assert.Equal(oneFriendToPerson.Partner, toOneFriend);
        Assert.Equal(oneFriendToPerson, toOneFriend.Partner);
    }

    [Fact]
    public void Parse_NavigationWithManyMultiplicityRecursiveContainmentEnd_ValidatesContainmentAndPartners()
    {
        var csdlElements = new[] { @"
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

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var personNavigations = (model.FindType("NS.Person") as IEdmEntityType).NavigationProperties();
        Assert.Equal(6, personNavigations.Count());

        var manyFriendToPerson = personNavigations.Where(n => n.Name.Equals("ManyFriendToPerson")).First();
        Assert.False(manyFriendToPerson.ContainsTarget);
        Assert.True(manyFriendToPerson.Partner.ContainsTarget);

        // CheckNavigationMultiplicity
        Assert.Equal(EdmMultiplicity.Many, manyFriendToPerson.Partner.TargetMultiplicity());
        Assert.Equal(EdmMultiplicity.Many, manyFriendToPerson.TargetMultiplicity());

        var zeroOrOneFriendToPerson = personNavigations.Where(n => n.Name.Equals("ZeroOrOneFriendToPerson")).First();
        // CheckNavigationContainment
        Assert.False(zeroOrOneFriendToPerson.ContainsTarget);
        Assert.True(zeroOrOneFriendToPerson.Partner.ContainsTarget);

        // CheckNavigationMultiplicity
        Assert.Equal(EdmMultiplicity.ZeroOrOne, zeroOrOneFriendToPerson.Partner.TargetMultiplicity());
        Assert.Equal(EdmMultiplicity.Many, zeroOrOneFriendToPerson.TargetMultiplicity());

        var oneFriendToPerson = personNavigations.Where(n => n.Name.Equals("OneFriendToPerson")).First();
        Assert.False(oneFriendToPerson.ContainsTarget);
        Assert.True(oneFriendToPerson.Partner.ContainsTarget);

        // CheckNavigationMultiplicity
        Assert.Equal(EdmMultiplicity.One, oneFriendToPerson.Partner.TargetMultiplicity());
        Assert.Equal(EdmMultiplicity.Many, oneFriendToPerson.TargetMultiplicity());

        var toManyFriend = personNavigations.Where(n => n.Name.Equals("ToManyFriend")).First();
        // CheckNavigationsArePartners
        Assert.Equal(manyFriendToPerson.Partner, toManyFriend);
        Assert.Equal(manyFriendToPerson, toManyFriend.Partner);

        var toZeroOrOneFriend = personNavigations.Where(n => n.Name.Equals("ToZeroOrOneFriend")).First();
        // CheckNavigationsArePartners
        Assert.Equal(zeroOrOneFriendToPerson.Partner, toZeroOrOneFriend);
        Assert.Equal(zeroOrOneFriendToPerson, toZeroOrOneFriend.Partner);

        var toOneFriend = personNavigations.Where(n => n.Name.Equals("ToOneFriend")).First();
        // CheckNavigationsArePartners
        Assert.Equal(oneFriendToPerson.Partner, toOneFriend);
        Assert.Equal(oneFriendToPerson, toOneFriend.Partner);
    }

    [Fact]
    public void Parse_SingleSimpleContainmentNavigation_ValidatesContainmentAndEntitySets()
    {
        var csdlElements = new[] { @"
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

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var personNavigations = (model.FindType("NS.Person") as IEdmEntityType).NavigationProperties();
        Assert.Single(personNavigations);

        var petNavigations = (model.FindType("NS.Pet") as IEdmEntityType).NavigationProperties();
        Assert.Equal(2, petNavigations.Count());

        var homeNavigations = (model.FindType("NS.Home") as IEdmEntityType).NavigationProperties();
        Assert.Single(homeNavigations);

        var personToPet = personNavigations.First();
        // CheckNavigationContainment
        Assert.True(personToPet.ContainsTarget);
        Assert.False(personToPet.Partner.ContainsTarget);

        var petToPerson = petNavigations.Where(n => n.Name.Equals("ToPerson")).First();
        // CheckNavigationsArePartners
        Assert.Equal(personToPet.Partner, petToPerson);
        Assert.Equal(personToPet, petToPerson.Partner);

        var homeToPet = homeNavigations.First();
        Assert.True(homeToPet.ContainsTarget);
        Assert.False(homeToPet.Partner.ContainsTarget);

        var petToHome = petNavigations.Where(n => n.Name.Equals("ToHome")).First();
        // CheckNavigationsArePartners
        Assert.Equal(homeToPet.Partner, petToHome);
        Assert.Equal(homeToPet, petToHome.Partner);

        var container = model.EntityContainer;
        var petSet = container.FindEntitySet("PetSet");
        var personSet = container.FindEntitySet("PersonSet");
        var homeSet = container.FindEntitySet("HomeSet");
        Assert.Equal(petSet.FindNavigationTarget(petToPerson), personSet);

        Assert.True(personSet.FindNavigationTarget(petToPerson) is IEdmUnknownEntitySet);
        Assert.Equal(petSet.FindNavigationTarget(petToHome), homeSet);
    }

    [Fact]
    public void Parse_TwoContainmentNavigationsWithSameEnd_ValidatesContainmentAndEntitySets()
    {
        var csdlElements = new[] { @"
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

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var personNavigations = (model.FindType("NS.Person") as IEdmEntityType).NavigationProperties();
        Assert.Single(personNavigations);

        var petNavigations = (model.FindType("NS.Pet") as IEdmEntityType).NavigationProperties();
        Assert.Equal(2, petNavigations.Count());

        var homeNavigations = (model.FindType("NS.Home") as IEdmEntityType).NavigationProperties();
        Assert.Single(homeNavigations);

        var personToPet = personNavigations.First();
        Assert.True(personToPet.ContainsTarget);
        Assert.False(personToPet.Partner.ContainsTarget);

        var petToPerson = petNavigations.Where(n => n.Name.Equals("ToPerson")).First();
        // CheckNavigationsArePartners
        Assert.Equal(personToPet.Partner, petToPerson);
        Assert.Equal(personToPet, petToPerson.Partner);

        var homeToPet = homeNavigations.First();
        Assert.True(homeToPet.ContainsTarget);
        Assert.False(homeToPet.Partner.ContainsTarget);

        var petToHome = petNavigations.Where(n => n.Name.Equals("ToHome")).First();
        // CheckNavigationsArePartners
        Assert.Equal(homeToPet.Partner, petToHome);
        Assert.Equal(homeToPet, petToHome.Partner);

        var container = model.EntityContainer;
        var petSet = container.FindEntitySet("PetSet");
        var personSet = container.FindEntitySet("PersonSet");
        var homeSet = container.FindEntitySet("HomeSet");

        Assert.Equal(petSet.FindNavigationTarget(petToPerson), personSet);
        Assert.Equal(petSet.FindNavigationTarget(petToHome), homeSet);
    }

    [Fact]
    public void Parse_ContainmentNavigationWithDifferentEnds_ValidatesContainmentAndEntitySets()
    {
        var csdlElements = new[] { @"
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

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var personNavigations = (model.FindType("NS.Person") as IEdmEntityType).NavigationProperties();
        var petNavigations = (model.FindType("NS.Pet") as IEdmEntityType).NavigationProperties();
        var homeNavigations = (model.FindType("NS.Home") as IEdmEntityType).NavigationProperties();

        var personToPet = personNavigations.Where(n => n.Name.Equals("ToPet")).First();
        Assert.True(personToPet.ContainsTarget);
        Assert.False(personToPet.Partner.ContainsTarget);

        var petToPerson = petNavigations.First();
        // CheckNavigationsArePartners
        Assert.Equal(personToPet.Partner, petToPerson);
        Assert.Equal(personToPet, petToPerson.Partner);

        var personToHome = personNavigations.Where(n => n.Name.Equals("ToHome")).First();
        Assert.True(personToHome.ContainsTarget);
        Assert.False(personToHome.Partner.ContainsTarget);

        var homeToPerson = homeNavigations.First();
        // CheckNavigationsArePartners
        Assert.Equal(personToHome.Partner, homeToPerson);
        Assert.Equal(personToHome, homeToPerson.Partner);

        var container = model.EntityContainer;
        var petSet = container.FindEntitySet("PetSet");
        var personSet = container.FindEntitySet("PersonSet");
        var homeSet = container.FindEntitySet("HomeSet");

        Assert.Equal(petSet.FindNavigationTarget(petToPerson), personSet);
        Assert.Equal(homeSet.FindNavigationTarget(homeToPerson), personSet);
    }

    [Fact]
    public void Parse_RecursiveThreeContainmentNavigations_ValidatesContainmentAndPartners()
    {
        var csdlElements = new[] { @"
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

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var personNavigations = (model.FindType("NS.Person") as IEdmEntityType).NavigationProperties();
        Assert.Equal(2, personNavigations.Count());

        var petNavigations = (model.FindType("NS.Pet") as IEdmEntityType).NavigationProperties();
        Assert.Equal(2, petNavigations.Count());

        var homeNavigations = (model.FindType("NS.Home") as IEdmEntityType).NavigationProperties();
        Assert.Equal(2, homeNavigations.Count());

        var personToPet = personNavigations.Where(n => n.Name.Equals("ToPet")).First();
        Assert.True(personToPet.ContainsTarget);
        Assert.False(personToPet.Partner.ContainsTarget);

        var petToPerson = petNavigations.Where(n => n.Name.Equals("ToPerson")).First();
        // CheckNavigationsArePartners
        Assert.Equal(personToPet.Partner, petToPerson);
        Assert.Equal(personToPet, petToPerson.Partner);

        var homeToPerson = homeNavigations.Where(n => n.Name.Equals("ToPerson")).First();
        Assert.True(homeToPerson.ContainsTarget);
        Assert.False(homeToPerson.Partner.ContainsTarget);

        var personToHome = personNavigations.Where(n => n.Name.Equals("ToHome")).First();
        // CheckNavigationsArePartners
        Assert.Equal(personToHome.Partner, homeToPerson);
        Assert.Equal(personToHome, homeToPerson.Partner);

        var petToHome = petNavigations.Where(n => n.Name.Equals("ToHome")).First();
        Assert.True(petToHome.ContainsTarget);
        Assert.False(petToHome.Partner.ContainsTarget);

        var homeToPet = homeNavigations.Where(n => n.Name.Equals("ToPet")).First();
        // CheckNavigationsArePartners
        Assert.Equal(petToHome.Partner, homeToPet);
        Assert.Equal(petToHome, homeToPet.Partner);
    }

    [Fact]
    public void Parse_RecursiveThreeContainmentNavigationsWithEntitySet_ValidatesContainmentAndEntitySets()
    {
        var csdlElements = new[] { @"
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

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var personNavigations = (model.FindType("NS.Person") as IEdmEntityType).NavigationProperties();
        Assert.Equal(2, personNavigations.Count());

        var petNavigations = (model.FindType("NS.Pet") as IEdmEntityType).NavigationProperties();
        Assert.Equal(2, petNavigations.Count());

        var homeNavigations = (model.FindType("NS.Home") as IEdmEntityType).NavigationProperties();
        Assert.Equal(2, homeNavigations.Count());

        var personToPet = personNavigations.Where(n => n.Name.Equals("ToPet")).First();
        Assert.True(personToPet.ContainsTarget);
        Assert.False(personToPet.Partner.ContainsTarget);

        var petToPerson = petNavigations.Where(n => n.Name.Equals("ToPerson")).First();
        // CheckNavigationsArePartners
        Assert.Equal(personToPet.Partner, petToPerson);
        Assert.Equal(personToPet, petToPerson.Partner);

        var homeToPerson = homeNavigations.Where(n => n.Name.Equals("ToPerson")).First();
        Assert.True(homeToPerson.ContainsTarget);
        Assert.False(homeToPerson.Partner.ContainsTarget);

        var personToHome = personNavigations.Where(n => n.Name.Equals("ToHome")).First();
        // CheckNavigationsArePartners
        Assert.Equal(personToHome.Partner, homeToPerson);
        Assert.Equal(personToHome, homeToPerson.Partner);

        var petToHome = petNavigations.Where(n => n.Name.Equals("ToHome")).First();
        Assert.True(petToHome.ContainsTarget);
        Assert.False(petToHome.Partner.ContainsTarget);

        var homeToPet = homeNavigations.Where(n => n.Name.Equals("ToPet")).First();
        // CheckNavigationsArePartners
        Assert.Equal(petToHome.Partner, homeToPet);
        Assert.Equal(petToHome, homeToPet.Partner);

        var container = model.EntityContainer;
        var personSet = container.FindEntitySet("PersonSet");
        var petSet = container.FindEntitySet("PetSet");
        var homeSet = container.FindEntitySet("HomeSet");
        Assert.Equal(petSet.FindNavigationTarget(personToPet.Partner), personSet);

        Assert.Equal(personSet.FindNavigationTarget(homeToPerson.Partner), homeSet);
        Assert.Equal(homeSet.FindNavigationTarget(petToHome.Partner), petSet);
    }

    [Fact]
    public void Parse_RecursiveOneContainmentNavigationSelfPointingEntitySet_ValidatesContainmentAndEntitySets()
    {
        var csdlElements = new[] { @"
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

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var personNavigations = (model.FindType("NS.Person") as IEdmEntityType).NavigationProperties();
        Assert.Equal(2, personNavigations.Count());

        var personToFriend = personNavigations.Where(n => n.Name.Equals("ToFriend")).First();
        Assert.True(personToFriend.ContainsTarget);
        Assert.False(personToFriend.Partner.ContainsTarget);

        var friendToPerson = personNavigations.Where(n => n.Name.Equals("ToPerson")).First();
        Assert.False(friendToPerson.ContainsTarget);
        Assert.True(friendToPerson.Partner.ContainsTarget);

        // CheckNavigationsArePartners
        Assert.Equal(personToFriend.Partner, friendToPerson);
        Assert.Equal(personToFriend, friendToPerson.Partner);

        var container = model.EntityContainer;
        var personSet = container.FindEntitySet("PersonSet");

        Assert.Equal(personSet.FindNavigationTarget(friendToPerson), personSet);
    }

    [Fact]
    public void Parse_RecursiveOneContainmentNavigationWithTwoEntitySets_ValidatesContainmentAndEntitySets()
    {
        var csdlElements = new[] { @"
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

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var personNavigations = (model.FindType("NS.Person") as IEdmEntityType).NavigationProperties();
        Assert.Equal(2, personNavigations.Count());

        var personToFriend = personNavigations.Where(n => n.Name.Equals("ToFriend")).First();
        Assert.True(personToFriend.ContainsTarget);
        Assert.False(personToFriend.Partner.ContainsTarget);

        var friendToPerson = personNavigations.Where(n => n.Name.Equals("ToPerson")).First();
        Assert.False(friendToPerson.ContainsTarget);
        Assert.True(friendToPerson.Partner.ContainsTarget);

        // CheckNavigationsArePartners
        Assert.Equal(personToFriend.Partner, friendToPerson);
        Assert.Equal(personToFriend, friendToPerson.Partner);

        var container = model.EntityContainer;
        var personSet = container.FindEntitySet("PersonSet");
        var friendSet = container.FindEntitySet("FriendSet");

        Assert.Equal(friendSet.FindNavigationTarget(friendToPerson), personSet);
    }

    [Fact]
    public void Parse_DerivedContainmentNavigationWithBaseAssociationSet_ValidatesContainmentAndEntitySets()
    {
        var csdlElements = new[] { @"
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

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var personNavigations = (model.FindType("NS.Person") as IEdmEntityType).NavigationProperties();
        Assert.Single(personNavigations);

        var employeeNavigations = (model.FindType("NS.Employee") as IEdmEntityType).NavigationProperties();
        Assert.Equal(2, employeeNavigations.Count());

        var homeNavigations = (model.FindType("NS.Home") as IEdmEntityType).NavigationProperties();
        Assert.Single(homeNavigations);

        var officeNavigations = (model.FindType("NS.Office") as IEdmEntityType).NavigationProperties();
        Assert.Equal(2, officeNavigations.Count());

        var personToHome = personNavigations.First();
        Assert.True(personToHome.ContainsTarget);
        Assert.False(personToHome.Partner.ContainsTarget);

        var homeToPerson = homeNavigations.First();
        Assert.False(homeToPerson.ContainsTarget);
        Assert.True(homeToPerson.Partner.ContainsTarget);

        // CheckNavigationsArePartners
        Assert.Equal(personToHome.Partner, homeToPerson);
        Assert.Equal(personToHome, homeToPerson.Partner);

        var employeeToOffice = employeeNavigations.Where(n => n.Name.Equals("ToOffice")).First();
        Assert.True(employeeToOffice.ContainsTarget);
        Assert.False(employeeToOffice.Partner.ContainsTarget);

        var officeToEmployee = officeNavigations.Where(n => n.Name.Equals("ToEmployee")).First();
        Assert.False(officeToEmployee.ContainsTarget);
        Assert.True(officeToEmployee.Partner.ContainsTarget);

        // CheckNavigationsArePartners
        Assert.Equal(employeeToOffice.Partner, officeToEmployee);
        Assert.Equal(employeeToOffice, officeToEmployee.Partner);

        var container = model.EntityContainer;
        var personSet = container.FindEntitySet("PersonSet");
        var homeSet = container.FindEntitySet("HomeSet");
        var officeSet = container.FindEntitySet("OfficeSet");

        Assert.Equal(officeSet.FindNavigationTarget(employeeToOffice.Partner), personSet);
        Assert.Equal(homeSet.FindNavigationTarget(homeToPerson), personSet);
    }

    [Fact]
    public void Parse_DerivedContainmentNavigationWithDerivedAssociationSet_ValidatesContainmentAndEntitySets()
    {
        var csdlElements = new[] { @"
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

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var personNavigations = (model.FindType("NS.Person") as IEdmEntityType).NavigationProperties();
        Assert.Single(personNavigations);

        var employeeNavigations = (model.FindType("NS.Employee") as IEdmEntityType).NavigationProperties();
        Assert.Equal(2, employeeNavigations.Count());

        var homeNavigations = (model.FindType("NS.Home") as IEdmEntityType).NavigationProperties();
        Assert.Single(homeNavigations);

        var officeNavigations = (model.FindType("NS.Office") as IEdmEntityType).NavigationProperties();
        Assert.Equal(2, officeNavigations.Count());

        var personToHome = personNavigations.First();
        Assert.True(personToHome.ContainsTarget);
        Assert.False(personToHome.Partner.ContainsTarget);

        var homeToPerson = homeNavigations.First();
        Assert.False(homeToPerson.ContainsTarget);
        Assert.True(homeToPerson.Partner.ContainsTarget);

        // CheckNavigationsArePartners
        Assert.Equal(personToHome.Partner, homeToPerson);
        Assert.Equal(personToHome, homeToPerson.Partner);

        var employeeToOffice = employeeNavigations.Where(n => n.Name.Equals("ToOffice")).First();
        Assert.True(employeeToOffice.ContainsTarget);
        Assert.False(employeeToOffice.Partner.ContainsTarget);

        var officeToEmployee = officeNavigations.Where(n => n.Name.Equals("ToEmployee")).First();
        Assert.False(officeToEmployee.ContainsTarget);
        Assert.True(officeToEmployee.Partner.ContainsTarget);

        // CheckNavigationsArePartners
        Assert.Equal(employeeToOffice.Partner, officeToEmployee);
        Assert.Equal(employeeToOffice, officeToEmployee.Partner);

        var container = model.EntityContainer;
        var employeeSet = container.FindEntitySet("EmployeeSet");
        var officeSet = container.FindEntitySet("OfficeSet");

        Assert.Equal(officeSet.FindNavigationTarget(homeToPerson), employeeSet);
    }

    [Fact]
    public void Parse_DerivedContainmentNavigationWithDerivedAndBaseAssociationSet_ValidatesContainmentAndEntitySets()
    {
        var csdlElements = new[] { @"
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

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var personNavigations = (model.FindType("NS.Person") as IEdmEntityType).NavigationProperties();
        Assert.Single(personNavigations);

        var employeeNavigations = (model.FindType("NS.Employee") as IEdmEntityType).NavigationProperties();
        Assert.Equal(2, employeeNavigations.Count());

        var homeNavigations = (model.FindType("NS.Home") as IEdmEntityType).NavigationProperties();
        Assert.Single(homeNavigations);

        var officeNavigations = (model.FindType("NS.Office") as IEdmEntityType).NavigationProperties();
        Assert.Equal(2, officeNavigations.Count());

        var personToHome = personNavigations.First();
        Assert.True(personToHome.ContainsTarget);
        Assert.False(personToHome.Partner.ContainsTarget);

        var homeToPerson = homeNavigations.First();
        Assert.False(homeToPerson.ContainsTarget);
        Assert.True(homeToPerson.Partner.ContainsTarget);

        // CheckNavigationsArePartners
        Assert.Equal(personToHome.Partner, homeToPerson);
        Assert.Equal(personToHome, homeToPerson.Partner);

        var employeeToOffice = employeeNavigations.Where(n => n.Name.Equals("ToOffice")).First();
        Assert.True(employeeToOffice.ContainsTarget);
        Assert.False(employeeToOffice.Partner.ContainsTarget);

        var officeToEmployee = officeNavigations.Where(n => n.Name.Equals("ToEmployee")).First();
        Assert.False(officeToEmployee.ContainsTarget);
        Assert.True(officeToEmployee.Partner.ContainsTarget);

        // CheckNavigationsArePartners
        Assert.Equal(employeeToOffice.Partner, officeToEmployee);
        Assert.Equal(employeeToOffice, officeToEmployee.Partner);

        var container = model.EntityContainer;
        var personSet = container.FindEntitySet("PersonSet");
        var employeeSet = container.FindEntitySet("EmployeeSet");
        var homeSet = container.FindEntitySet("HomeSet");
        var officeSet = container.FindEntitySet("OfficeSet");

        Assert.Equal(officeSet.FindNavigationTarget(employeeToOffice.Partner), personSet);
        Assert.Equal(homeSet.FindNavigationTarget(personToHome.Partner), employeeSet);
    }

    [Fact]
    public void Parse_NavigationForIsPrincipal_ValidatesPrincipalStatus()
    {
        var csdlElements = new[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""PersonId"" />
            <PropertyRef Name=""PersonName"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>" }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var person = model.FindType("DefaultNamespace.Person") as IEdmEntityType;
        Assert.NotNull(person);

        var personNavs = person.NavigationProperties();
        Assert.Single(personNavs);

        var personNav = personNavs.First();
        Assert.True(personNav.IsPrincipal());
        Assert.False(personNav.Partner.IsPrincipal());
    }

    [Fact]
    public void Parse_NavigationBothNotPrincipal_RoundTripsSuccessfully()
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

        var personToPet = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
            new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One });
        pet.AddProperty(personToPet.Partner);
        person.AddProperty(personToPet);

        Assert.False(personToPet.IsPrincipal());
        Assert.False(personToPet.Partner.IsPrincipal());

        var csdls = this.GetSerializerResult(model);
        var isParsed = SchemaReader.TryParse(csdls.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel roundTripModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var roundTripPerson = roundTripModel.FindType("NS.Person") as IEdmEntityType;
        Assert.NotNull(roundTripPerson);

        var roundTripNavs = roundTripPerson.NavigationProperties();
        Assert.Single(roundTripNavs);

        var roundTripNav = roundTripNavs.First();
        Assert.False(roundTripNav.IsPrincipal());
        Assert.False(roundTripNav.Partner.IsPrincipal());
    }

    [Fact]
    public void Parse_MultiBindingNavigation_ValidatesBindingsAndEntitySets()
    {
        // check model
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

        //CheckMultiBindingModel
        var entitySetFound = model.EntityContainer.FindEntitySet("EntitySet");

        var complexType = model.FindType("NS.ComplexType") as IEdmStructuredType;
        var navComplex = complexType.FindProperty("CollectionOfNavOnComplex") as IEdmNavigationProperty;

        var containedType = model.FindType("NS.ContainedEntityType") as IEdmStructuredType;
        var navOnContainedFound = containedType.FindProperty("NavOnContained") as IEdmNavigationProperty;

        var target11 = entitySetFound.FindNavigationTarget(navComplex, new EdmPathExpression("complexProp1/CollectionOfNavOnComplex"));
        var target12 = entitySetFound.FindNavigationTarget(navComplex, new EdmPathExpression("complexProp2/CollectionOfNavOnComplex"));
        var target21 = entitySetFound.FindNavigationTarget(navOnContainedFound, new EdmPathExpression("ContainedNav1/NavOnContained"));
        var target22 = entitySetFound.FindNavigationTarget(navOnContainedFound, new EdmPathExpression("ContainedNav2/NavOnContained"));
        Assert.Equal(target11, target21);
        Assert.Equal(target12, target22);

        // check csdl
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

        var csdlElements = new[] { XElement.Parse(csdl) };
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model2, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        //CheckMultiBindingModel
        var entitySetFound2 = model2.EntityContainer.FindEntitySet("EntitySet");

        var complexType2 = model2.FindType("NS.ComplexType") as IEdmStructuredType;
        var navComplex2 = complexType2.FindProperty("CollectionOfNavOnComplex") as IEdmNavigationProperty;

        var containedType2 = model2.FindType("NS.ContainedEntityType") as IEdmStructuredType;
        var navOnContainedFound2 = containedType2.FindProperty("NavOnContained") as IEdmNavigationProperty;

        var target112 = entitySetFound2.FindNavigationTarget(navComplex2, new EdmPathExpression("complexProp1/CollectionOfNavOnComplex"));
        var target122 = entitySetFound2.FindNavigationTarget(navComplex2, new EdmPathExpression("complexProp2/CollectionOfNavOnComplex"));
        var target212 = entitySetFound2.FindNavigationTarget(navOnContainedFound2, new EdmPathExpression("ContainedNav1/NavOnContained"));
        var target222 = entitySetFound2.FindNavigationTarget(navOnContainedFound2, new EdmPathExpression("ContainedNav2/NavOnContained"));
        Assert.Equal(target112, target212);
        Assert.Equal(target122, target222);
        Assert.Equal(target12, target22);
    }
}
