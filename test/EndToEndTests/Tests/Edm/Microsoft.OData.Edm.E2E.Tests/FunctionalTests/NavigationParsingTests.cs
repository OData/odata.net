//---------------------------------------------------------------------
// <copyright file="NavigationParsingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class NavigationParsingTests : EdmLibTestCaseBase
{
    [Fact]
    public void ParsingNavigationWithBothContainmentEndsCsdl()
    {
        var csdls = NavigationTestModelBuilder.NavigationWithBothContainmentEndsCsdl();
        var model = this.GetParserResult(csdls);

        this.CheckEntityTypeNavigationCount(model, "NS.Person", 1);
        this.CheckEntityTypeNavigationCount(model, "NS.Pet", 1);

        var personToPet = model.FindEntityType("NS.Person").NavigationProperties().First();
        this.CheckNavigationContainment(personToPet, true, true);
    }

    [Fact]
    public void ParsingNavigationWithOneMultiplicityContainmentEndCsdl()
    {
        var csdls = NavigationTestModelBuilder.NavigationWithOneMultiplicityContainmentEndCsdl();
        var model = this.GetParserResult(csdls);
        this.CheckNavigationWithMultiplicityContainmentEnd(model, EdmMultiplicity.One);
    }

    [Fact]
    public void ParsingNavigationWithManyMultiplicityContainmentEndCsdl()
    {
        var csdls = NavigationTestModelBuilder.NavigationWithManyMultiplicityContainmentEndCsdl();
        var model = this.GetParserResult(csdls);
        this.CheckNavigationWithMultiplicityContainmentEnd(model, EdmMultiplicity.Many);
    }

    [Fact]
    public void ParsingNavigationWithZeroOrOneMultiplicityContainmentEndCsdl()
    {
        var csdls = NavigationTestModelBuilder.NavigationWithZeroOrOneMultiplicityContainmentEndCsdl();
        var model = this.GetParserResult(csdls);
        this.CheckNavigationWithMultiplicityContainmentEnd(model, EdmMultiplicity.ZeroOrOne);
    }

    private void CheckNavigationWithMultiplicityContainmentEnd(IEdmModel model, EdmMultiplicity multiplicity)
    {
        this.CheckEntityTypeNavigationCount(model, "NS.Person", 3);
        this.CheckEntityTypeNavigationCount(model, "NS.Pet", 3);
        var personNavigations = model.FindEntityType("NS.Person").NavigationProperties();
        var petNavigations = model.FindEntityType("NS.Pet").NavigationProperties();

        var personToOnePet = personNavigations.Where(n => n.Name.Equals("ToOnePet")).First();
        this.CheckNavigationContainment(personToOnePet, true, false);
        this.CheckNavigationMultiplicity(personToOnePet, multiplicity, EdmMultiplicity.One);

        var personToZeroOrOnePet = personNavigations.Where(n => n.Name.Equals("ToZeroOrOnePet")).First();
        this.CheckNavigationContainment(personToZeroOrOnePet, true, false);
        this.CheckNavigationMultiplicity(personToZeroOrOnePet, multiplicity, EdmMultiplicity.ZeroOrOne);

        var personToManyPet = personNavigations.Where(n => n.Name.Equals("ToManyPet")).First();
        this.CheckNavigationContainment(personToManyPet, true, false);
        this.CheckNavigationMultiplicity(personToManyPet, multiplicity, EdmMultiplicity.Many);

        var onePetToPerson = petNavigations.Where(n => n.Name.Equals("OnePetToPerson")).First();
        Assert.Equal(personToOnePet.Partner, onePetToPerson, "Invalid navigation partner.");

        var zeroOrOnePetToPerson = petNavigations.Where(n => n.Name.Equals("ZeroOrOnePetToPerson")).First();
        this.CheckNavigationsArePartners(personToZeroOrOnePet, zeroOrOnePetToPerson);

        var manyPetToPerson = petNavigations.Where(n => n.Name.Equals("ManyPetToPerson")).First();
        this.CheckNavigationsArePartners(personToManyPet, manyPetToPerson);
    }

    [Fact]
    public void ParsingNavigationWithValidZeroOrOneMultiplicityRecursiveContainmentEndCsdl()
    {
        var csdls = NavigationTestModelBuilder.NavigationWithValidZeroOrOneMultiplicityRecursiveContainmentEndCsdl();
        var model = this.GetParserResult(csdls);

        this.CheckEntityTypeNavigationCount(model, "NS.Person", 4);
        var personNavigations = model.FindEntityType("NS.Person").NavigationProperties();

        var personToFriend = personNavigations.Where(n => n.Name.Equals("ToFriend")).First();
        this.CheckNavigationContainment(personToFriend, false, true);
        this.CheckNavigationMultiplicity(personToFriend, EdmMultiplicity.Many, EdmMultiplicity.ZeroOrOne);

        var personToParent = personNavigations.Where(n => n.Name.Equals("ToParent")).First();
        this.CheckNavigationContainment(personToParent, false, true);
        this.CheckNavigationMultiplicity(personToParent, EdmMultiplicity.ZeroOrOne, EdmMultiplicity.ZeroOrOne);

        var friendToPerson = personNavigations.Where(n => n.Name.Equals("ToPerson")).First();
        this.CheckNavigationsArePartners(personToFriend, friendToPerson);

        var parentToPerson = personNavigations.Where(n => n.Name.Equals("ToSelf")).First();
        this.CheckNavigationsArePartners(personToParent, parentToPerson);
    }

    [Fact]
    public void ParsingNavigationWithInvaliZeroOrOnedMultiplicityRecursiveContainmentEndCsdl()
    {
        var csdls = NavigationTestModelBuilder.NavigationWithInvaliZeroOrOnedMultiplicityRecursiveContainmentEndCsdl();
        var model = this.GetParserResult(csdls);

        this.CheckEntityTypeNavigationCount(model, "NS.Person", 2);
        var personNavigations = model.FindEntityType("NS.Person").NavigationProperties();

        var personToFriend = personNavigations.Where(n => n.Name.Equals("ToFriend")).First();
        this.CheckNavigationContainment(personToFriend, false, true);
        this.CheckNavigationMultiplicity(personToFriend, EdmMultiplicity.One, EdmMultiplicity.ZeroOrOne);

        var friendToPerson = personNavigations.Where(n => n.Name.Equals("ToPerson")).First();
        this.CheckNavigationsArePartners(personToFriend, friendToPerson);
    }

    [Fact]
    public void ParsingNavigationWithOneMultiplicityRecursiveContainmentEndCsdl()
    {
        var csdls = NavigationTestModelBuilder.NavigationWithOneMultiplicityRecursiveContainmentEndCsdl();
        var model = this.GetParserResult(csdls);

        this.CheckNavigationWithMultiplicityRecursiveContainmentEnd(model, EdmMultiplicity.One);
    }

    [Fact]
    public void ParsingNavigationWithManyMultiplicityRecursiveContainmentEndCsdl()
    {
        var csdls = NavigationTestModelBuilder.NavigationWithManyMultiplicityRecursiveContainmentEndCsdl();
        var model = this.GetParserResult(csdls);

        this.CheckNavigationWithMultiplicityRecursiveContainmentEnd(model, EdmMultiplicity.Many);
    }

    private void CheckNavigationWithMultiplicityRecursiveContainmentEnd(IEdmModel model, EdmMultiplicity multiplicity)
    {
        this.CheckEntityTypeNavigationCount(model, "NS.Person", 6);
        var personNavigations = model.FindEntityType("NS.Person").NavigationProperties();

        var manyFriendToPerson = personNavigations.Where(n => n.Name.Equals("ManyFriendToPerson")).First();
        this.CheckNavigationContainment(manyFriendToPerson, false, true);
        this.CheckNavigationMultiplicity(manyFriendToPerson, EdmMultiplicity.Many, multiplicity);

        var zeroOrOneFriendToPerson = personNavigations.Where(n => n.Name.Equals("ZeroOrOneFriendToPerson")).First();
        this.CheckNavigationContainment(zeroOrOneFriendToPerson, false, true);
        this.CheckNavigationMultiplicity(zeroOrOneFriendToPerson, EdmMultiplicity.ZeroOrOne, multiplicity);

        var oneFriendToPerson = personNavigations.Where(n => n.Name.Equals("OneFriendToPerson")).First();
        this.CheckNavigationContainment(oneFriendToPerson, false, true);
        this.CheckNavigationMultiplicity(oneFriendToPerson, EdmMultiplicity.One, multiplicity);

        var toManyFriend = personNavigations.Where(n => n.Name.Equals("ToManyFriend")).First();
        this.CheckNavigationsArePartners(manyFriendToPerson, toManyFriend);

        var toZeroOrOneFriend = personNavigations.Where(n => n.Name.Equals("ToZeroOrOneFriend")).First();
        this.CheckNavigationsArePartners(zeroOrOneFriendToPerson, toZeroOrOneFriend);

        var toOneFriend = personNavigations.Where(n => n.Name.Equals("ToOneFriend")).First();
        this.CheckNavigationsArePartners(oneFriendToPerson, toOneFriend);
    }

    [Fact]
    public void ParsingSingleSimpleContainmentNavigationCsdl()
    {
        var csdls = NavigationTestModelBuilder.SingleSimpleContainmentNavigationCsdl();
        var model = this.GetParserResult(csdls);

        this.CheckEntityTypeNavigationCount(model, "NS.Person", 1);
        this.CheckEntityTypeNavigationCount(model, "NS.Pet", 2);
        this.CheckEntityTypeNavigationCount(model, "NS.Home", 1);

        var personNavigations = model.FindEntityType("NS.Person").NavigationProperties();
        var petNavigations = model.FindEntityType("NS.Pet").NavigationProperties();
        var homeNavigations = model.FindEntityType("NS.Home").NavigationProperties();

        var personToPet = personNavigations.First();
        this.CheckNavigationContainment(personToPet, true, false);
        var petToPerson = petNavigations.Where(n => n.Name.Equals("ToPerson")).First();
        this.CheckNavigationsArePartners(personToPet, petToPerson);

        var homeToPet = homeNavigations.First();
        this.CheckNavigationContainment(homeToPet, true, false);
        var petToHome = petNavigations.Where(n => n.Name.Equals("ToHome")).First();
        this.CheckNavigationsArePartners(homeToPet, petToHome);

        var container = model.EntityContainer;
        var petSet = container.FindEntitySet("PetSet");
        var personSet = container.FindEntitySet("PersonSet");
        var homeSet = container.FindEntitySet("HomeSet");
        Assert.Equal(petSet.FindNavigationTarget(petToPerson), personSet, "Invalid entity set navigation target.");

        // Contained entity set is generated dynamically.
        // Assert.Equal(personSet.FindNavigationTarget(personToPet), petSet, "Invalid entity set navigation target.");
        Assert.True(personSet.FindNavigationTarget(petToPerson) is IEdmUnknownEntitySet, "Not expecting to find entity set.");

        // Assert.Null(petSet.FindNavigationTarget(personToPet), "Not expecting to find entity set.");

        // Assert.Null(homeSet.FindNavigationTarget(homeToPet), "Not expecting to find entity set.");
        Assert.Equal(petSet.FindNavigationTarget(petToHome), homeSet, "Invalid entity set navigation target.");
    }

    [Fact]
    public void ParsingTwoContainmentNavigationWithSameEndCsdl()
    {
        var csdls = NavigationTestModelBuilder.TwoContainmentNavigationWithSameEndCsdl();
        var model = this.GetParserResult(csdls);

        this.CheckEntityTypeNavigationCount(model, "NS.Person", 1);
        this.CheckEntityTypeNavigationCount(model, "NS.Pet", 2);
        this.CheckEntityTypeNavigationCount(model, "NS.Home", 1);

        var personNavigations = model.FindEntityType("NS.Person").NavigationProperties();
        var petNavigations = model.FindEntityType("NS.Pet").NavigationProperties();
        var homeNavigations = model.FindEntityType("NS.Home").NavigationProperties();

        var personToPet = personNavigations.First();
        this.CheckNavigationContainment(personToPet, true, false);
        var petToPerson = petNavigations.Where(n => n.Name.Equals("ToPerson")).First();
        this.CheckNavigationsArePartners(personToPet, petToPerson);

        var homeToPet = homeNavigations.First();
        this.CheckNavigationContainment(homeToPet, true, false);
        var petToHome = petNavigations.Where(n => n.Name.Equals("ToHome")).First();
        this.CheckNavigationsArePartners(homeToPet, petToHome);

        var container = model.EntityContainer;
        var petSet = container.FindEntitySet("PetSet");
        var personSet = container.FindEntitySet("PersonSet");
        var homeSet = container.FindEntitySet("HomeSet");
        Assert.Equal(petSet.FindNavigationTarget(petToPerson), personSet, "Invalid entity set navigation target.");

        // Contained entity set is generated dynamically.
        // Assert.Equal(personSet.FindNavigationTarget(personToPet), petSet, "Invalid entity set navigation target.");
        // Assert.Equal(homeSet.FindNavigationTarget(homeToPet), petSet, "Invalid entity set navigation target.");
        Assert.Equal(petSet.FindNavigationTarget(petToHome), homeSet, "Invalid entity set navigation target.");
    }

    [Fact]
    public void ParsingContainmentNavigationWithDifferentEndsCsdl()
    {
        var csdls = NavigationTestModelBuilder.ContainmentNavigationWithDifferentEndsCsdl();
        var model = this.GetParserResult(csdls);

        this.CheckEntityTypeNavigationCount(model, "NS.Person", 2);
        this.CheckEntityTypeNavigationCount(model, "NS.Pet", 1);
        this.CheckEntityTypeNavigationCount(model, "NS.Home", 1);

        var personNavigations = model.FindEntityType("NS.Person").NavigationProperties();
        var petNavigations = model.FindEntityType("NS.Pet").NavigationProperties();
        var homeNavigations = model.FindEntityType("NS.Home").NavigationProperties();

        var personToPet = personNavigations.Where(n => n.Name.Equals("ToPet")).First();
        this.CheckNavigationContainment(personToPet, true, false);
        var petToPerson = petNavigations.First();
        this.CheckNavigationsArePartners(personToPet, petToPerson);

        var personToHome = personNavigations.Where(n => n.Name.Equals("ToHome")).First();
        this.CheckNavigationContainment(personToHome, true, false);
        var homeToPerson = homeNavigations.First();
        this.CheckNavigationsArePartners(personToHome, homeToPerson);

        var container = model.EntityContainer;
        var petSet = container.FindEntitySet("PetSet");
        var personSet = container.FindEntitySet("PersonSet");
        var homeSet = container.FindEntitySet("HomeSet");
        Assert.Equal(petSet.FindNavigationTarget(petToPerson), personSet, "Invalid entity set navigation target.");

        // Contained entity set is generated dynamically.
        // Assert.Equal(personSet.FindNavigationTarget(personToPet), petSet, "Invalid entity set navigation target.");
        Assert.Equal(homeSet.FindNavigationTarget(homeToPerson), personSet, "Invalid entity set navigation target.");

        // Contained entity set is generated dynamically.
        // Assert.Equal(personSet.FindNavigationTarget(personToHome), homeSet, "Invalid entity set navigation target.");
    }

    [Fact]
    public void ParsingRecursiveThreeContainmentNavigationsCsdl()
    {
        var csdls = NavigationTestModelBuilder.RecursiveThreeContainmentNavigationsCsdl();
        var model = this.GetParserResult(csdls);

        this.CheckEntityTypeNavigationCount(model, "NS.Person", 2);
        this.CheckEntityTypeNavigationCount(model, "NS.Pet", 2);
        this.CheckEntityTypeNavigationCount(model, "NS.Home", 2);

        var personNavigations = model.FindEntityType("NS.Person").NavigationProperties();
        var petNavigations = model.FindEntityType("NS.Pet").NavigationProperties();
        var homeNavigations = model.FindEntityType("NS.Home").NavigationProperties();

        var personToPet = personNavigations.Where(n => n.Name.Equals("ToPet")).First();
        this.CheckNavigationContainment(personToPet, true, false);
        var petToPerson = petNavigations.Where(n => n.Name.Equals("ToPerson")).First();
        this.CheckNavigationsArePartners(personToPet, petToPerson);

        var homeToPerson = homeNavigations.Where(n => n.Name.Equals("ToPerson")).First();
        this.CheckNavigationContainment(homeToPerson, true, false);
        var personToHome = personNavigations.Where(n => n.Name.Equals("ToHome")).First();
        this.CheckNavigationsArePartners(personToHome, homeToPerson);

        var petToHome = petNavigations.Where(n => n.Name.Equals("ToHome")).First();
        this.CheckNavigationContainment(petToHome, true, false);
        var homeToPet = homeNavigations.Where(n => n.Name.Equals("ToPet")).First();
        this.CheckNavigationsArePartners(petToHome, homeToPet);
    }

    [Fact]
    public void ParsingRecursiveThreeContainmentNavigationsWithEntitySetCsdl()
    {
        var csdls = NavigationTestModelBuilder.RecursiveThreeContainmentNavigationsWithEntitySetCsdl();
        var model = this.GetParserResult(csdls);

        this.CheckEntityTypeNavigationCount(model, "NS.Person", 2);
        this.CheckEntityTypeNavigationCount(model, "NS.Pet", 2);
        this.CheckEntityTypeNavigationCount(model, "NS.Home", 2);

        var personNavigations = model.FindEntityType("NS.Person").NavigationProperties();
        var petNavigations = model.FindEntityType("NS.Pet").NavigationProperties();
        var homeNavigations = model.FindEntityType("NS.Home").NavigationProperties();

        var personToPet = personNavigations.Where(n => n.Name.Equals("ToPet")).First();
        this.CheckNavigationContainment(personToPet, true, false);
        var petToPerson = petNavigations.Where(n => n.Name.Equals("ToPerson")).First();
        this.CheckNavigationsArePartners(personToPet, petToPerson);

        var homeToPerson = homeNavigations.Where(n => n.Name.Equals("ToPerson")).First();
        this.CheckNavigationContainment(homeToPerson, true, false);
        var personToHome = personNavigations.Where(n => n.Name.Equals("ToHome")).First();
        this.CheckNavigationsArePartners(personToHome, homeToPerson);

        var petToHome = petNavigations.Where(n => n.Name.Equals("ToHome")).First();
        this.CheckNavigationContainment(petToHome, true, false);
        var homeToPet = homeNavigations.Where(n => n.Name.Equals("ToPet")).First();
        this.CheckNavigationsArePartners(petToHome, homeToPet);

        var container = model.EntityContainer;
        var personSet = container.FindEntitySet("PersonSet");
        var petSet = container.FindEntitySet("PetSet");
        var homeSet = container.FindEntitySet("HomeSet");
        Assert.Equal(petSet.FindNavigationTarget(personToPet.Partner), personSet, "Invalid entity set navigation target.");

        // Contained entity set is generated dynamically.
        // Assert.Equal(personSet.FindNavigationTarget(personToPet), petSet, "Invalid entity set navigation target.");
        // Assert.Equal(homeSet.FindNavigationTarget(homeToPerson), personSet, "Invalid entity set navigation target.");
        Assert.Equal(personSet.FindNavigationTarget(homeToPerson.Partner), homeSet, "Invalid entity set navigation target.");

        //Assert.Equal(petSet.FindNavigationTarget(petToHome), homeSet, "Invalid entity set navigation target.");
        Assert.Equal(homeSet.FindNavigationTarget(petToHome.Partner), petSet, "Invalid entity set navigation target.");
    }

    [Fact]
    public void ParsingRecursiveOneContainmentNavigationSelfPointingEntitySetCsdl()
    {
        var csdls = NavigationTestModelBuilder.RecursiveOneContainmentNavigationSelfPointingEntitySetCsdl();
        var model = this.GetParserResult(csdls);

        this.CheckEntityTypeNavigationCount(model, "NS.Person", 2);
        var personNavigations = model.FindEntityType("NS.Person").NavigationProperties();

        var personToFriend = personNavigations.Where(n => n.Name.Equals("ToFriend")).First();
        this.CheckNavigationContainment(personToFriend, true, false);
        var friendToPerson = personNavigations.Where(n => n.Name.Equals("ToPerson")).First();
        this.CheckNavigationContainment(friendToPerson, false, true);
        this.CheckNavigationsArePartners(personToFriend, friendToPerson);

        var container = model.EntityContainer;
        var personSet = container.FindEntitySet("PersonSet");

        // Contained entity set is generated dynamically.
        // Assert.Equal(personSet.FindNavigationTarget(personToFriend), personSet, "Invalid entity set navigation target.");
        Assert.Equal(personSet.FindNavigationTarget(friendToPerson), personSet, "Invalid entity set navigation target.");
    }

    [Fact]
    public void ParsingRecursiveOneContainmentNavigationWithTwoEntitySetCsdl()
    {
        var csdls = NavigationTestModelBuilder.RecursiveOneContainmentNavigationWithTwoEntitySetCsdl();
        var model = this.GetParserResult(csdls);

        this.CheckEntityTypeNavigationCount(model, "NS.Person", 2);
        var personNavigations = model.FindEntityType("NS.Person").NavigationProperties();

        var personToFriend = personNavigations.Where(n => n.Name.Equals("ToFriend")).First();
        this.CheckNavigationContainment(personToFriend, true, false);
        var friendToPerson = personNavigations.Where(n => n.Name.Equals("ToPerson")).First();
        this.CheckNavigationContainment(friendToPerson, false, true);
        this.CheckNavigationsArePartners(personToFriend, friendToPerson);

        var container = model.EntityContainer;
        var personSet = container.FindEntitySet("PersonSet");
        var friendSet = container.FindEntitySet("FriendSet");

        // Contained entity set is generated dynamically.
        // Assert.Equal(personSet.FindNavigationTarget(personToFriend), friendSet, "Invalid entity set navigation target.");
        Assert.Equal(friendSet.FindNavigationTarget(friendToPerson), personSet, "Invalid entity set navigation target.");
    }

    [Fact]
    public void ParsingDerivedContainmentNavigationWithBaseAssociationSetCsdl()
    {
        var csdls = NavigationTestModelBuilder.DerivedContainmentNavigationWithBaseAssociationSetCsdl();
        var model = this.GetParserResult(csdls);

        this.CheckEntityTypeNavigationCount(model, "NS.Person", 1);
        this.CheckEntityTypeNavigationCount(model, "NS.Employee", 2);
        this.CheckEntityTypeNavigationCount(model, "NS.Home", 1);
        this.CheckEntityTypeNavigationCount(model, "NS.Office", 2);

        var personToHome = model.FindEntityType("NS.Person").NavigationProperties().First();
        this.CheckNavigationContainment(personToHome, true, false);
        var homeToPerson = model.FindEntityType("NS.Home").NavigationProperties().First();
        this.CheckNavigationContainment(homeToPerson, false, true);
        this.CheckNavigationsArePartners(personToHome, homeToPerson);

        var employeeToOffice = model.FindEntityType("NS.Employee").NavigationProperties().Where(n => n.Name.Equals("ToOffice")).First();
        this.CheckNavigationContainment(employeeToOffice, true, false);
        var officeToEmployee = model.FindEntityType("NS.Office").NavigationProperties().Where(n => n.Name.Equals("ToEmployee")).First();
        this.CheckNavigationContainment(officeToEmployee, false, true);
        this.CheckNavigationsArePartners(employeeToOffice, officeToEmployee);
        var container = model.EntityContainer;
        var personSet = container.FindEntitySet("PersonSet");
        var homeSet = container.FindEntitySet("HomeSet");
        var officeSet = container.FindEntitySet("OfficeSet");

        Assert.Equal(officeSet.FindNavigationTarget(employeeToOffice.Partner), personSet, "Invalid entity set navigation target.");
        Assert.Equal(homeSet.FindNavigationTarget(homeToPerson), personSet, "Invalid entity set navigation target.");

        // Contained entity set is generated dynamically.
        // Assert.Equal(personSet.FindNavigationTarget(officeToEmployee.Partner), homeSet, "Invalid entity set navigation target.");
    }

    [Fact]
    public void ParsingDerivedContainmentNavigationWithDerivedAssociationSetCsdl()
    {
        var csdls = NavigationTestModelBuilder.DerivedContainmentNavigationWithDerivedAssociationSetCsdl();
        var model = this.GetParserResult(csdls);

        this.CheckEntityTypeNavigationCount(model, "NS.Person", 1);
        this.CheckEntityTypeNavigationCount(model, "NS.Employee", 2);
        this.CheckEntityTypeNavigationCount(model, "NS.Home", 1);
        this.CheckEntityTypeNavigationCount(model, "NS.Office", 2);

        var personToHome = model.FindEntityType("NS.Person").NavigationProperties().First();
        this.CheckNavigationContainment(personToHome, true, false);
        var homeToPerson = model.FindEntityType("NS.Home").NavigationProperties().First();
        this.CheckNavigationContainment(homeToPerson, false, true);
        this.CheckNavigationsArePartners(personToHome, homeToPerson);

        var employeeToOffice = model.FindEntityType("NS.Employee").NavigationProperties().Where(n => n.Name.Equals("ToOffice")).First();
        this.CheckNavigationContainment(employeeToOffice, true, false);
        var officeToEmployee = model.FindEntityType("NS.Office").NavigationProperties().Where(n => n.Name.Equals("ToEmployee")).First();
        this.CheckNavigationContainment(officeToEmployee, false, true);
        this.CheckNavigationsArePartners(employeeToOffice, officeToEmployee);

        var container = model.EntityContainer;
        var employeeSet = container.FindEntitySet("EmployeeSet");
        var officeSet = container.FindEntitySet("OfficeSet");

        Assert.Equal(officeSet.FindNavigationTarget(homeToPerson), employeeSet, "Invalid entity set navigation target.");

        // Contained entity set is generated dynamically.
        // Assert.Equal(employeeSet.FindNavigationTarget(homeToPerson.Partner), officeSet, "Invalid entity set navigation target.");
    }

    [Fact]
    public void ParsingDerivedContainmentNavigationWithDerivedAndBaseAssociationSetCsdl()
    {
        var csdls = NavigationTestModelBuilder.DerivedContainmentNavigationWithDerivedAndBaseAssociationSetCsdl();
        var model = this.GetParserResult(csdls);

        this.CheckEntityTypeNavigationCount(model, "NS.Person", 1);
        this.CheckEntityTypeNavigationCount(model, "NS.Employee", 2);
        this.CheckEntityTypeNavigationCount(model, "NS.Home", 1);
        this.CheckEntityTypeNavigationCount(model, "NS.Office", 2);

        var personToHome = model.FindEntityType("NS.Person").NavigationProperties().First();
        this.CheckNavigationContainment(personToHome, true, false);
        var homeToPerson = model.FindEntityType("NS.Home").NavigationProperties().First();
        this.CheckNavigationContainment(homeToPerson, false, true);
        this.CheckNavigationsArePartners(personToHome, homeToPerson);

        var employeeToOffice = model.FindEntityType("NS.Employee").NavigationProperties().Where(n => n.Name.Equals("ToOffice")).First();
        this.CheckNavigationContainment(employeeToOffice, true, false);
        var officeToEmployee = model.FindEntityType("NS.Office").NavigationProperties().Where(n => n.Name.Equals("ToEmployee")).First();
        this.CheckNavigationContainment(officeToEmployee, false, true);
        this.CheckNavigationsArePartners(employeeToOffice, officeToEmployee);

        var container = model.EntityContainer;
        var personSet = container.FindEntitySet("PersonSet");
        var employeeSet = container.FindEntitySet("EmployeeSet");
        var homeSet = container.FindEntitySet("HomeSet");
        var officeSet = container.FindEntitySet("OfficeSet");

        Assert.Equal(officeSet.FindNavigationTarget(employeeToOffice.Partner), personSet, "Invalid entity set navigation target.");

        // Contained entity set is generated dynamically.
        // Assert.Equal(employeeSet.FindNavigationTarget(personToHome), homeSet, "Invalid entity set navigation target.");
        // Assert.Equal(personSet.FindNavigationTarget(employeeToOffice), officeSet, "Invalid entity set navigation target.");
        Assert.Equal(homeSet.FindNavigationTarget(personToHome.Partner), employeeSet, "Invalid entity set navigation target.");
    }

    [Fact]
    public void TestNavigationForIsPrincipalCsdl()
    {
        var csdls = NavigationTestModelBuilder.NavigationSinglePrincipalWithNotNullableKeyDependentCsdl();
        var model = this.GetParserResult(csdls);

        var person = model.FindEntityType("DefaultNamespace.Person");
        Assert.NotNull(person, "Invalid entity type.");
        var personNavs = person.NavigationProperties();
        Assert.Equal(1, personNavs.Count(), "Invalid navigation property count.");
        var personNav = personNavs.First();
        Assert.True(personNav.IsPrincipal(), "Invalid navigation principal value.");
        Assert.False(personNav.Partner.IsPrincipal(), "Invalid navigation principal value.");
    }

    [Fact]
    public void TestNavigationBothNotPrincipalRoundTrip()
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

        Assert.False(personToPet.IsPrincipal(), "Invalid navigation principal value.");
        Assert.False(personToPet.Partner.IsPrincipal(), "Invalid navigation principal value.");

        var csdls = this.GetSerializerResult(model);
        var roundTripModel = this.GetParserResult(csdls);

        var roundTripPerson = roundTripModel.FindEntityType("NS.Person");
        Assert.NotNull(roundTripPerson, "Invalid entity type.");
        var roundTripNavs = roundTripPerson.NavigationProperties();
        Assert.Equal(1, roundTripNavs.Count(), "Invalid navigation property count.");
        var roundTripNav = roundTripNavs.First();
        Assert.False(roundTripNav.IsPrincipal(), "Invalid navigation principal value.");
        Assert.False(roundTripNav.Partner.IsPrincipal(), "Invalid navigation principal value.");
    }

    [Fact]
    public void ParsingMultiBindingCsdl()
    {
        // check model
        var model = NavigationTestModelBuilder.MultiNavigationBindingModel();
        CheckMultiBindingModel(model);

        // check csdl
        var csdl = NavigationTestModelBuilder.MultiNavigationBindingModelCsdl();
        model = this.GetParserResult(csdl);
        CheckMultiBindingModel(model);
    }

    private void CheckMultiBindingModel(IEdmModel model)
    {
        var entitySet = model.EntityContainer.FindEntitySet("EntitySet");

        var complexType = model.FindType("NS.ComplexType") as IEdmStructuredType;
        var navComplex = complexType.FindProperty("CollectionOfNavOnComplex") as IEdmNavigationProperty;

        var containedType = model.FindType("NS.ContainedEntityType") as IEdmStructuredType;
        var navOnContained = containedType.FindProperty("NavOnContained") as IEdmNavigationProperty;

        var target11 = entitySet.FindNavigationTarget(navComplex, new EdmPathExpression("complexProp1/CollectionOfNavOnComplex"));
        var target12 = entitySet.FindNavigationTarget(navComplex, new EdmPathExpression("complexProp2/CollectionOfNavOnComplex"));
        var target21 = entitySet.FindNavigationTarget(navOnContained, new EdmPathExpression("ContainedNav1/NavOnContained"));
        var target22 = entitySet.FindNavigationTarget(navOnContained, new EdmPathExpression("ContainedNav2/NavOnContained"));
        Assert.Equal(target11, target21);
        Assert.Equal(target12, target22);
    }

    private void CheckNavigationsArePartners(IEdmNavigationProperty navigation, IEdmNavigationProperty partner)
    {
        Assert.Equal(navigation.Partner, partner, "Invalid navigation partner.");
        Assert.Equal(navigation, partner.Partner, "Invalid navigation partner.");
    }

    private void CheckNavigationMultiplicity(IEdmNavigationProperty navigation, EdmMultiplicity navigationMultiplicity, EdmMultiplicity navigationPartnerMultiplicity)
    {
        Assert.Equal(navigationMultiplicity, navigation.Partner.TargetMultiplicity(), "Invalid navigation property multiplicity.");
        Assert.Equal(navigationPartnerMultiplicity, navigation.TargetMultiplicity(), "Invalid navigation property partner multiplicity.");
    }

    private void CheckNavigationContainment(IEdmNavigationProperty navigation, bool navigationContainsTarget, bool navigationPartnerContainsTarget)
    {
        Assert.Equal(navigationContainsTarget, navigation.ContainsTarget, "Invalid navigation property contains target value.");
        Assert.Equal(navigationPartnerContainsTarget, navigation.Partner.ContainsTarget, "Invalid navigation property partner contains target value.");
    }

    private void CheckEntityTypeNavigationCount(IEdmModel model, string entityTypeFullName, int navigationPropertiesCount)
    {
        var entityType = model.FindEntityType(entityTypeFullName);
        var navigations = entityType.NavigationProperties();
        Assert.Equal(navigationPropertiesCount, navigations.Count(), "Invalid navigation property count.");
    }
}
