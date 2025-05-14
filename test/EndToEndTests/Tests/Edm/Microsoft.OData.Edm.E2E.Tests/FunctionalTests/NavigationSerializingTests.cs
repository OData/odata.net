//---------------------------------------------------------------------
// <copyright file="NavigationSerializingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml.Linq;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class NavigationSerializingTests : EdmLibTestCaseBase
{
    [Fact]
    public void SerializeNavigationWithBothContainmentEnd()
    {
        var model = NavigationTestModelBuilder.NavigationWithBothContainmentEnds();
        var csdls = NavigationTestModelBuilder.NavigationWithBothContainmentEndsCsdl();

        this.SerializingValidator(model, csdls);
    }

    [Fact]
    public void SerializeNavigationWithOneMultiplicityContainmentEnd()
    {
        var model = NavigationTestModelBuilder.NavigationWithOneMultiplicityContainmentEnd();
        var csdls = NavigationTestModelBuilder.NavigationWithOneMultiplicityContainmentEndCsdl();

        this.SerializingValidator(model, csdls);
    }

    [Fact]
    public void SerializeNavigationWithManyMultiplicityContainmentEnd()
    {
        var model = NavigationTestModelBuilder.NavigationWithManyMultiplicityContainmentEnd();
        var csdls = NavigationTestModelBuilder.NavigationWithManyMultiplicityContainmentEndCsdl();

        this.SerializingValidator(model, csdls);
    }

    [Fact]
    public void SerializeNavigationWithZeroOrOneMultiplicityContainmentEnd()
    {
        var model = NavigationTestModelBuilder.NavigationWithZeroOrOneMultiplicityContainmentEnd();
        var csdls = NavigationTestModelBuilder.NavigationWithZeroOrOneMultiplicityContainmentEndCsdl();

        this.SerializingValidator(model, csdls);
    }

    [Fact]
    public void SerializeNavigationWithValidZeroOrOneMultiplicityRecursiveContainmentEnd()
    {
        var model = NavigationTestModelBuilder.NavigationWithValidZeroOrOneMultiplicityRecursiveContainmentEnd();
        var csdls = NavigationTestModelBuilder.NavigationWithValidZeroOrOneMultiplicityRecursiveContainmentEndCsdl();

        this.SerializingValidator(model, csdls);
    }

    [Fact]
    public void SerializeNavigationWithInvaliZeroOrOnedMultiplicityRecursiveContainmentEnd()
    {
        var model = NavigationTestModelBuilder.NavigationWithInvaliZeroOrOnedMultiplicityRecursiveContainmentEnd();
        var csdls = NavigationTestModelBuilder.NavigationWithInvaliZeroOrOnedMultiplicityRecursiveContainmentEndCsdl();

        this.SerializingValidator(model, csdls);
    }

    [Fact]
    public void SerializeNavigationWithOneMultiplicityRecursiveContainmentEnd()
    {
        var model = NavigationTestModelBuilder.NavigationWithOneMultiplicityRecursiveContainmentEnd();
        var csdls = NavigationTestModelBuilder.NavigationWithOneMultiplicityRecursiveContainmentEndCsdl();

        this.SerializingValidator(model, csdls);
    }

    [Fact]
    public void SerializeNavigationWithManyMultiplicityRecursiveContainmentEnd()
    {
        var model = NavigationTestModelBuilder.NavigationWithManyMultiplicityRecursiveContainmentEnd();
        var csdls = NavigationTestModelBuilder.NavigationWithManyMultiplicityRecursiveContainmentEndCsdl();

        this.SerializingValidator(model, csdls);
    }

    [Fact]
    public void SerializeSingleSimpleContainmentNavigation()
    {
        var model = NavigationTestModelBuilder.SingleSimpleContainmentNavigation();
        var csdls = NavigationTestModelBuilder.SingleSimpleContainmentNavigationCsdl();

        this.SerializingValidator(model, csdls);
    }

    [Fact]
    public void SerializeTwoContainmentNavigationWithSameEnd()
    {
        var model = NavigationTestModelBuilder.TwoContainmentNavigationWithSameEnd();
        var csdls = NavigationTestModelBuilder.TwoContainmentNavigationWithSameEndCsdl();

        this.SerializingValidator(model, csdls);
    }

    [Fact]
    public void SerializeTwoContainmentNavigationWithSameEndAddedDifferently()
    {
        var model = NavigationTestModelBuilder.TwoContainmentNavigationWithSameEndAddedDifferently();
        var csdls = NavigationTestModelBuilder.TwoContainmentNavigationWithSameEndCsdl();

        this.SerializingValidator(model, csdls);
    }

    [Fact]
    public void SerializeContainmentNavigationWithDifferentEnds()
    {
        var model = NavigationTestModelBuilder.ContainmentNavigationWithDifferentEnds();
        var csdls = NavigationTestModelBuilder.ContainmentNavigationWithDifferentEndsCsdl();

        this.SerializingValidator(model, csdls);
    }

    [Fact]
    public void SerializeRecursiveThreeContainmentNavigations()
    {
        var model = NavigationTestModelBuilder.RecursiveThreeContainmentNavigations();
        var csdls = NavigationTestModelBuilder.RecursiveThreeContainmentNavigationsCsdl();

        this.SerializingValidator(model, csdls);
    }

    [Fact]
    public void SerializeRecursiveThreeContainmentNavigationsWithEntitySet()
    {
        var model = NavigationTestModelBuilder.RecursiveThreeContainmentNavigationsWithEntitySet();
        var csdls = NavigationTestModelBuilder.RecursiveThreeContainmentNavigationsWithEntitySetCsdl();

        this.SerializingValidator(model, csdls);
    }

    [Fact]
    public void SerializeRecursiveOneContainmentNavigationSelfPointingEntitySet()
    {
        var model = NavigationTestModelBuilder.RecursiveOneContainmentNavigationSelfPointingEntitySet();
        var csdls = NavigationTestModelBuilder.RecursiveOneContainmentNavigationSelfPointingEntitySetCsdl();

        this.SerializingValidator(model, csdls);
    }

    [Fact]
    public void SerializeRecursiveOneContainmentNavigationWithTwoEntitySet()
    {
        var model = NavigationTestModelBuilder.RecursiveOneContainmentNavigationWithTwoEntitySet();
        var csdls = NavigationTestModelBuilder.RecursiveOneContainmentNavigationWithTwoEntitySetCsdl();

        this.SerializingValidator(model, csdls);
    }

    [Fact]
    public void SerializeDerivedContainmentNavigationWithBaseAssociationSet()
    {
        var model = NavigationTestModelBuilder.DerivedContainmentNavigationWithBaseAssociationSet();
        var csdls = NavigationTestModelBuilder.DerivedContainmentNavigationWithBaseAssociationSetCsdl();

        this.SerializingValidator(model, csdls);
    }

    [Fact]
    public void SerializeDerivedContainmentNavigationWithDerivedAssociationSet()
    {
        var model = NavigationTestModelBuilder.DerivedContainmentNavigationWithDerivedAssociationSet();
        var csdls = NavigationTestModelBuilder.DerivedContainmentNavigationWithDerivedAssociationSetCsdl();

        this.SerializingValidator(model, csdls);
    }

    [Fact]
    public void SerializeDerivedContainmentNavigationWithDerivedAndBaseAssociationSet()
    {
        var model = NavigationTestModelBuilder.DerivedContainmentNavigationWithDerivedAndBaseAssociationSet();
        var csdls = NavigationTestModelBuilder.DerivedContainmentNavigationWithDerivedAndBaseAssociationSetCsdl();

        this.SerializingValidator(model, csdls);
    }

    [Fact]
    public void SerializeMultiBindingForOneNavigationProperty()
    {
        var model = NavigationTestModelBuilder.MultiNavigationBindingModel();
        var csdl = NavigationTestModelBuilder.MultiNavigationBindingModelCsdl();

        this.SerializingValidator(model, csdl);
    }

    private void SerializingValidator(IEdmModel expectedModel, IEnumerable<XElement> actualCsdls)
    {
        var expectedCsdls = this.GetSerializerResult(expectedModel).Select(n => XElement.Parse(n));

        var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), EdmVersion.V40);
        var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), EdmVersion.V40);

        new ConstructiveApiCsdlXElementComparer().Compare(updatedExpectedCsdls.ToList(), updatedActualCsdls.ToList());
    }
}
