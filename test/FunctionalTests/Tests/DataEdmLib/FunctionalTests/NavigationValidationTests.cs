//---------------------------------------------------------------------
// <copyright file="NavigationValidationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NavigationValidationTests : EdmLibTestCaseBase
    {
        [TestMethod]
        public void ValidateNavigationSinglePrincipalWithNotNullableDependentCsdl()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationSinglePrincipalWithNotNullableDependentCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationSinglePrincipalWithNotNullableKeyDependentCsdl()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationSinglePrincipalWithNotNullableKeyDependentCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationSinglePrincipalWithMixNullableDependentCsdl()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationSinglePrincipalWithMixNullableDependentCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationSinglePrincipalWithAllNullableDependentCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidMultiplicityOfPrincipalEnd }
            };
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationSinglePrincipalWithAllNullableDependentCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationMultiplePrincipalWithNotNullableDependentCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidMultiplicityOfPrincipalEnd }
            };
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationMultiplePrincipalWithNotNullableDependentCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationMultiplePrincipalWithMixNullableDependentCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidMultiplicityOfPrincipalEnd }
            };
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationMultiplePrincipalWithMixNullableDependentCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationMultiplePrincipalWithAllNullableDependentCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidMultiplicityOfPrincipalEnd }
            };
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationMultiplePrincipalWithAllNullableDependentCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationZeroOnePrincipalWithNotNullableDependentCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidMultiplicityOfPrincipalEnd }
            };
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationZeroOnePrincipalWithNotNullableDependentCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationZeroOnePrincipalWithNotNullableKeyDependentCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidMultiplicityOfPrincipalEnd }
            };
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationZeroOnePrincipalWithNotNullableKeyDependentCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationZeroOnePrincipalWithMixNullableDependentCsdl()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationZeroOnePrincipalWithMixNullableDependentCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationZeroOnePrincipalWithAllNullableDependentCsdl()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationZeroOnePrincipalWithAllNullableDependentCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationZeroOnePrincipalWithAllNullableKeyDependentCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { 19, 10, EdmErrorCode.InvalidKey },
                { 20, 10, EdmErrorCode.InvalidKey },
            };
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationZeroOnePrincipalWithAllNullableKeyDependentCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationSinglePrincipalWithNotNullableDependentModel()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationSinglePrincipalWithNotNullableDependentModel(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationSinglePrincipalWithNotNullableKeyDependentModel()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationSinglePrincipalWithNotNullableKeyDependentModel(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationSinglePrincipalWithMixNullableDependentModel()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationSinglePrincipalWithMixNullableDependentModel(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationSinglePrincipalWithAllNullableDependentModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidMultiplicityOfPrincipalEnd }
            };
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationSinglePrincipalWithAllNullableDependentModel(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationMultiplePrincipalWithNotNullableDependentModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidMultiplicityOfPrincipalEnd }
            };
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationMultiplePrincipalWithNotNullableDependentModel(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationMultiplePrincipalWithMixNullableDependentModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidMultiplicityOfPrincipalEnd }
            };
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationMultiplePrincipalWithMixNullableDependentModel(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationMultiplePrincipalWithAllNullableDependentModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidMultiplicityOfPrincipalEnd }
            };
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationMultiplePrincipalWithAllNullableDependentModel(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationZeroOnePrincipalWithNotNullableDependentModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidMultiplicityOfPrincipalEnd }
            };
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationZeroOnePrincipalWithNotNullableDependentModel(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationZeroOnePrincipalWithNotNullableKeyDependentModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidMultiplicityOfPrincipalEnd }
            };
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationZeroOnePrincipalWithNotNullableKeyDependentModel(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationZeroOnePrincipalWithMixNullableDependentModel()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationZeroOnePrincipalWithMixNullableDependentModel(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationZeroOnePrincipalWithAllNullableDependentModel()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationZeroOnePrincipalWithAllNullableDependentModel(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationZeroOnePrincipalWithAllNullableKeyDependentModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidKey },
                { null, null, EdmErrorCode.InvalidKey }
            };
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationZeroOnePrincipalWithAllNullableKeyDependentModel(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationNonKeyPrincipalPropertyRefCsdl()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationOneKeywithOneNonKeyPrincipalPropertyRefCsdl(), EdmVersion.V40, expectedErrors);
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationTwoNonKeyPrincipalPropertyRefCsdl(), EdmVersion.V40, expectedErrors);
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationOneNonKeyPrincipalPropertyRefCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationDuplicateDepdendentPropertyRefCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {null, null, EdmErrorCode.DuplicateDependentProperty}
            };
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationDuplicateDependentPropertyRefCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationDuplicatePrincipalPropertyRefCsdl()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationDuplicatePrincipalPropertyRefCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationDuplicateReferentialConstraintCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.DuplicateDependentProperty }
            };
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationDuplicateReferentialConstraintCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationPrincipalPropertyRefDoesNotCorrespondToDependentPropertyRefCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.TypeMismatchRelationshipConstraint },
                { null, null, EdmErrorCode.TypeMismatchRelationshipConstraint }
            };
            this.VerifySemanticValidation(NavigationTestModelBuilder.NavigationPrincipalPropertyRefDoesNotCorrespondToDependentPropertyRefCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationWithEmptyNameModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidName }
            };

            var model = NavigationTestModelBuilder.NavigationWithEmptyNameModel();

            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationWithOneMultiplicityContainmentEnd()
        {
            var model = NavigationTestModelBuilder.NavigationWithOneMultiplicityContainmentEnd();
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationWithManyMultiplicityContainmentEnd()
        {
            var model = NavigationTestModelBuilder.NavigationWithManyMultiplicityContainmentEnd();
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne },
                { null, null, EdmErrorCode.NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne },
                { null, null, EdmErrorCode.NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne }
            };
            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationWithZeroOrOneMultiplicityContainmentEnd()
        {
            var model = NavigationTestModelBuilder.NavigationWithZeroOrOneMultiplicityContainmentEnd();
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne },
                { null, null, EdmErrorCode.NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne },
                { null, null, EdmErrorCode.NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne }
            };

            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationWithValidMultiplicityRecursiveContainmentEnd()
        {
            var model = NavigationTestModelBuilder.NavigationWithValidZeroOrOneMultiplicityRecursiveContainmentEnd();
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationWithInvaliZeroOrOnedMultiplicityRecursiveContainmentEnd()
        {
            var model = NavigationTestModelBuilder.NavigationWithInvaliZeroOrOnedMultiplicityRecursiveContainmentEnd();
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.NavigationPropertyWithRecursiveContainmentTargetMustBeOptional }
            };
            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationWithOneMultiplicityRecursiveContainmentEnd()
        {
            var model = NavigationTestModelBuilder.NavigationWithOneMultiplicityRecursiveContainmentEnd();
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne },
                { null, null, EdmErrorCode.NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne },
                { null, null, EdmErrorCode.NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne },
                { null, null, EdmErrorCode.NavigationPropertyWithRecursiveContainmentTargetMustBeOptional }
            };
            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationWithManyMultiplicityRecursiveContainmentEnd()
        {
            var model = NavigationTestModelBuilder.NavigationWithManyMultiplicityRecursiveContainmentEnd();
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne },
                { null, null, EdmErrorCode.NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne },
                { null, null, EdmErrorCode.NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne },
                { null, null, EdmErrorCode.NavigationPropertyWithRecursiveContainmentTargetMustBeOptional }
            };

            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateSingleSimpleContainmentNavigation()
        {
            var model = NavigationTestModelBuilder.SingleSimpleContainmentNavigation();
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateTwoContainmentNavigationWithSameEnd()
        {
            var model = NavigationTestModelBuilder.TwoContainmentNavigationWithSameEnd();

            var expectedErrors = new EdmLibTestErrors() { };
            //{
            //    { null, null, EdmErrorCode.EntitySetCanOnlyBeContainedByASingleNavigationProperty }
            //};
            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateTwoContainmentNavigationWithSameEndAddedDifferently()
        {
            var model = NavigationTestModelBuilder.TwoContainmentNavigationWithSameEndAddedDifferently();
            var expectedErrors = new EdmLibTestErrors() { };
            //{
            //    { null, null, EdmErrorCode.EntitySetCanOnlyBeContainedByASingleNavigationProperty }
            //};
            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateContainmentNavigationWithDifferentEnds()
        {
            var model = NavigationTestModelBuilder.ContainmentNavigationWithDifferentEnds();
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateRecursiveOneContainmentNavigationSelfPointingEntitySet()
        {
            var model = NavigationTestModelBuilder.RecursiveOneContainmentNavigationSelfPointingEntitySet();
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateRecursiveOneContainmentNavigationInheritedSelfPointingEntitySet()
        {
            var expectedErrors = new EdmLibTestErrors() { };
            //{
            //    { null, null, EdmErrorCode.EntitySetRecursiveNavigationPropertyMappingsMustPointBackToSourceEntitySet }
            //};
            var model = NavigationTestModelBuilder.RecursiveOneContainmentNavigationInheritedSelfPointingEntitySet();
            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateRecursiveOneContainmentNavigationWithTwoEntitySet()
        {
            var expectedErrors = new EdmLibTestErrors() { };
            //{
            //    { null, null, EdmErrorCode.EntitySetRecursiveNavigationPropertyMappingsMustPointBackToSourceEntitySet }
            //};

            var model = NavigationTestModelBuilder.RecursiveOneContainmentNavigationWithTwoEntitySet();

            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateDerivedContainmentNavigationWithBaseAssociationSet()
        {
            var model = NavigationTestModelBuilder.DerivedContainmentNavigationWithBaseAssociationSet();
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateDerivedContainmentNavigationWithDerivedAssociationSet()
        {
            var model = NavigationTestModelBuilder.DerivedContainmentNavigationWithDerivedAssociationSet();
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateDerivedContainmentNavigationWithDerivedAssociationSetCsdl()
        {
            var model = this.GetParserResult(NavigationTestModelBuilder.DerivedContainmentNavigationWithDerivedAssociationSetCsdl());
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateDerivedContainmentNavigationWithDerivedAndBaseAssociationSet()
        {
            var model = NavigationTestModelBuilder.DerivedContainmentNavigationWithDerivedAndBaseAssociationSet();
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationWithInvalidEntitySet()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.BadUnresolvedType },
                { null, null, EdmErrorCode.BadUnresolvedType }
            };

            var model = NavigationTestModelBuilder.NavigationWithInvalidEntitySet();

            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);

            expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.BadUnresolvedType },
                { null, null, EdmErrorCode.BadUnresolvedType }
            };
            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationWithInvalidAssociationSetEntitySet()
        {
            var model = NavigationTestModelBuilder.NavigationAssociationSetWithInvalidEntitySet();

            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.UnresolvedNavigationPropertyBindingPath },
                { null, null, EdmErrorCode.NavigationPropertyMappingMustPointToValidTargetForProperty },
            };
            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationWithInvalidEntitySetInSingleton()
        {
            var model = NavigationTestModelBuilder.NavigationAssociationSetWithInvalidEntitySetInSingleton();

            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.NavigationPropertyMappingMustPointToValidTargetForProperty },
                { null, null, EdmErrorCode.UnresolvedNavigationPropertyBindingPath },
                { null, null, EdmErrorCode.NavigationPropertyMappingMustPointToValidTargetForProperty },
                { null, null, EdmErrorCode.UnresolvedNavigationPropertyBindingPath },
            };
            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationPropertyOfCollectionTypeTargetToSingleton()
        {
            var model = NavigationTestModelBuilder.NavigationPropertyOfCollectionTypeTargetToSingleton();

            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.NavigationPropertyOfCollectionTypeMustNotTargetToSingleton },
            };
            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNavigationWithUnknownMultiplicity()
        {
            this.VerifyThrowsException(typeof(ArgumentOutOfRangeException), () => { NavigationTestModelBuilder.NavigationWithUnknownMultiplicity(); });
        }

        [TestMethod]
        public void ValidateNavigationWithUnknownMultiplicityPartner()
        {
            this.VerifyThrowsException(typeof(ArgumentOutOfRangeException), () => { NavigationTestModelBuilder.NavigationWithUnknownMultiplicityPartner(); });
        }
    }
}
