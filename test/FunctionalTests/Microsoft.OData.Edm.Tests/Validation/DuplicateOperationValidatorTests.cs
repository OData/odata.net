//---------------------------------------------------------------------
// <copyright file="DuplicateOperationValidatorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
using FluentAssertions;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Validation;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Validation
{
    /// <summary>
    /// DuplicateOperationValidatorTests tests
    /// </summary>
    public class DuplicateOperationValidatorTests
    {
        private static readonly IEdmEntityTypeReference DefaultValidEntityType = new EdmEntityTypeReference(new EdmEntityType("ds.n", "EntityType"), true);
        private static readonly IEdmCollectionTypeReference DefaultValidCollectionEntityType = new EdmCollectionTypeReference(new EdmCollectionType(DefaultValidEntityType));

        [Fact]
        public void DuplicateFunctionOverloadsWithOnlyNamespaceDifferentShouldNotError()
        {
            var edmFunction = new EdmFunction("n.s1", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt16(true));
            var edmFunction2 = new EdmFunction("n.s2", "GetStuff", EdmCoreModel.Instance.GetInt16(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction2.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt16(true));

            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddElement(edmFunction2);
            ValidateNoError(model);
        }

        [Fact]
        public void DuplicateBoundFunctionOverloadsSameParameterNamesShouldError()
        {
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt32(true));
            edmFunction.AddParameter("param1", EdmCoreModel.Instance.GetInt32(true));
            var edmFunction2 = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetInt16(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction2.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt32(true));
            edmFunction2.AddParameter("param1", EdmCoreModel.Instance.GetInt16(true));

            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddElement(edmFunction2);
            ValidateError(model, EdmErrorCode.DuplicateFunctions, Strings.EdmModel_Validator_Semantic_ModelDuplicateBoundFunctionParameterNames("n.s.GetStuff"));
        }

        [Fact]
        public void DuplicateUnBoundFunctionOverloadsSameParameterNamesShouldError()
        {
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), false /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction.AddParameter("bindingParameter1", EdmCoreModel.Instance.GetInt16(true));
            var edmFunction2 = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetInt16(true), false /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction2.AddParameter("bindingParameter1", EdmCoreModel.Instance.GetInt32(true));

            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddElement(edmFunction2);
            ValidateError(model, EdmErrorCode.DuplicateFunctions, Strings.EdmModel_Validator_Semantic_ModelDuplicateUnBoundFunctionsParameterNames("n.s.GetStuff"));
        }

        [Fact]
        public void DuplicateFunctionOverloadsWithDifferentBindingTypesAndSameNameWithDifferentReturnTypesShouldNotError()
        {
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt16(true));
            var edmFunction2 = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetInt16(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction2.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt32(true));

            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddElement(edmFunction2);
            ValidateNoError(model);
        }

        [Fact]
        public void DuplicateFunctionsUnBoundFunctionsWithSameNameWhereBindingTypeDifferentonNullabilityAndSameParametersShouldError()
        {
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt16(true));
            edmFunction.AddParameter("differentName", EdmCoreModel.Instance.GetInt16(true));
            var edmFunction2 = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction2.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt16(true));
            edmFunction2.AddParameter("otherParameter", EdmCoreModel.Instance.GetInt16(false));

            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddElement(edmFunction2);
            ValidateError(model, EdmErrorCode.DuplicateFunctions, Strings.EdmModel_Validator_Semantic_ModelDuplicateBoundFunctionParameterTypes("n.s.GetStuff"));
        }

        [Fact]
        public void DuplicateFunctionsBoundFunctionsWithDifferentBindingParameterNameButSameTypeShouldError()
        {
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt16(true));
            edmFunction.AddParameter("otherParam", EdmCoreModel.Instance.GetInt16(true));
            var edmFunction2 = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction2.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt16(true));
            edmFunction2.AddParameter("param", EdmCoreModel.Instance.GetInt16(true));

            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddElement(edmFunction2);
            ValidateError(model, EdmErrorCode.DuplicateFunctions, Strings.EdmModel_Validator_Semantic_ModelDuplicateBoundFunctionParameterTypes("n.s.GetStuff"));
        }

        [Fact]
        public void DuplicateFunctionsUnBoundFunctionsWithDifferentBindingParameterNameButSameTypeShouldError()
        {
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), false /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction.AddParameter("parameter", EdmCoreModel.Instance.GetInt16(true));
            var edmFunction2 = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), false /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction2.AddParameter("otherparameter", EdmCoreModel.Instance.GetInt16(true));

            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddElement(edmFunction2);
            ValidateError(model, EdmErrorCode.DuplicateFunctions, Strings.EdmModel_Validator_Semantic_ModelDuplicateUnBoundFunctionsParameterTypes("n.s.GetStuff"));
        }

        [Fact]
        public void DuplicateFunctionsUnBoundAndBoundFunctionWithSameNameAndParametersShouldNotError()
        {
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction.AddParameter("parameter", EdmCoreModel.Instance.GetInt16(true));
            var edmFunction2 = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), false /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction2.AddParameter("parameter", EdmCoreModel.Instance.GetInt16(true));

            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddElement(edmFunction2);
            ValidateNoError(model);
        }

        [Fact]
        public void DuplicateFunctionsBoundFunctionsWithSameNameBindingTypeAndDifferentParametersShouldNotError()
        {
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt16(true));
            var edmFunction2 = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction2.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt16(true));
            edmFunction2.AddParameter("param1", EdmCoreModel.Instance.GetInt16(true));

            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddElement(edmFunction2);
            ValidateNoError(model);
        }

        [Fact]
        public void DuplicateFunctionsBoundFunctionsWithSameNameDifferentBindingTypeAndSameParametersShouldNotError()
        {
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction.AddParameter("bindingParameter", EdmCoreModel.Instance.GetString(true));
            var edmFunction2 = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction2.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt16(true));

            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddElement(edmFunction2);
            ValidateNoError(model);
        }

        [Fact]
        public void EnsureUnBoundFunctionWithDifferentOrderedParameterNamesShouldError()
        {
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), false /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            edmFunction.AddParameter("param2", EdmCoreModel.Instance.GetString(true));
            var edmFunction2 = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), false /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction2.AddParameter("param2", EdmCoreModel.Instance.GetInt32(true));
            edmFunction2.AddParameter("param1", EdmCoreModel.Instance.GetInt32(true));

            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddElement(edmFunction2);
            ValidateError(model, EdmErrorCode.DuplicateFunctions, Strings.EdmModel_Validator_Semantic_ModelDuplicateUnBoundFunctionsParameterNames("n.s.GetStuff"));
        }

        [Fact]
        public void EnsureBoundFunctionWithDifferentOrderedParameterNamesShouldError()
        {
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction.AddParameter("bindingParameter", EdmCoreModel.Instance.GetString(true));
            edmFunction.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            edmFunction.AddParameter("param2", EdmCoreModel.Instance.GetString(true));
            var edmFunction2 = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction2.AddParameter("bindingParameter", EdmCoreModel.Instance.GetString(true));
            edmFunction2.AddParameter("param2", EdmCoreModel.Instance.GetInt32(true));
            edmFunction2.AddParameter("param1", EdmCoreModel.Instance.GetInt32(true));

            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddElement(edmFunction2);
            ValidateError(model, EdmErrorCode.DuplicateFunctions, Strings.EdmModel_Validator_Semantic_ModelDuplicateBoundFunctionParameterNames("n.s.GetStuff"));
        }

        [Fact]
        public void EnsureBoundFunctionWithDifferentSameOrderedParameterTypesShouldError()
        {
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction.AddParameter("bindingParameter", EdmCoreModel.Instance.GetString(true));
            edmFunction.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            edmFunction.AddParameter("param2", EdmCoreModel.Instance.GetInt16(true));
            var edmFunction2 = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction2.AddParameter("bindingParameter", EdmCoreModel.Instance.GetString(true));
            edmFunction2.AddParameter("param", EdmCoreModel.Instance.GetString(true));
            edmFunction2.AddParameter("param1", EdmCoreModel.Instance.GetInt16(true));

            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddElement(edmFunction2);
            ValidateError(model, EdmErrorCode.DuplicateFunctions, Strings.EdmModel_Validator_Semantic_ModelDuplicateBoundFunctionParameterTypes("n.s.GetStuff"));
        }

        [Fact]
        public void EnsureUnBoundFunctionWithDifferentSameOrderedParameterTypesShouldError()
        {
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), false /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            edmFunction.AddParameter("param2", EdmCoreModel.Instance.GetInt16(true));
            var edmFunction2 = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), false /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction2.AddParameter("param", EdmCoreModel.Instance.GetString(true));
            edmFunction2.AddParameter("param1", EdmCoreModel.Instance.GetInt16(true));

            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddElement(edmFunction2);
            ValidateError(model, EdmErrorCode.DuplicateFunctions, Strings.EdmModel_Validator_Semantic_ModelDuplicateUnBoundFunctionsParameterTypes("n.s.GetStuff"));
        }

        [Fact]
        public void EnsureBoundAndUnBoundFunctionsNamedTheSameShouldNotError()
        {
            var edmFunction= new EdmFunction("n.s", "DoStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false);
            edmFunction.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            var edmFunction2 = new EdmFunction("n.s", "DoStuff", EdmCoreModel.Instance.GetString(true), false /*isBound*/, null /*entitySetPath*/, false);
            edmFunction2.AddParameter("param2", EdmCoreModel.Instance.GetString(true));

            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddElement(edmFunction2);
            ValidateNoError(model);
        }

        [Fact]
        public void EnsureBoundFunctionWithDifferentOrderedParameterTypesShouldNotError()
        {
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction.AddParameter("bindingParameter", EdmCoreModel.Instance.GetString(true));
            edmFunction.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            edmFunction.AddParameter("param2", EdmCoreModel.Instance.GetInt16(true));
            var edmFunction2 = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction2.AddParameter("bindingParameter", EdmCoreModel.Instance.GetString(true));
            edmFunction2.AddParameter("param", EdmCoreModel.Instance.GetInt16(true));
            edmFunction2.AddParameter("param1", EdmCoreModel.Instance.GetString(true));

            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddElement(edmFunction2);
            ValidateNoError(model);
        }

        [Fact]
        public void EnsureBoundAndUnBoundActionsNamedTheSameShouldNotError()
        {
            var edmAction = new EdmAction("n.s", "DoStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/);
            edmAction.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            var edmAction2 = new EdmAction("n.s", "DoStuff", EdmCoreModel.Instance.GetString(true), false /*isBound*/, null /*entitySetPath*/);
            edmAction2.AddParameter("param2", EdmCoreModel.Instance.GetString(true));

            EdmModel model = new EdmModel();
            model.AddElement(edmAction);
            model.AddElement(edmAction2);
            ValidateNoError(model);
        }

        [Fact]
        public void EnsureBoundActionWithSameBindingParameterTypesShouldError()
        {
            var edmAction = new EdmAction("n.s", "DoStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/);
            edmAction.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            var edmAction2 = new EdmAction("n.s", "DoStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/);
            edmAction2.AddParameter("param2", EdmCoreModel.Instance.GetString(true));

            EdmModel model = new EdmModel();
            model.AddElement(edmAction);
            model.AddElement(edmAction2);
            ValidateError(model, EdmErrorCode.DuplicateActions, Strings.EdmModel_Validator_Semantic_ModelDuplicateBoundActions("n.s.DoStuff"));
        }

        [Fact]
        public void EnsureNonBoundActionWithSameNameShouldError()
        {
            var edmAction = new EdmAction("n.s", "DoStuff", EdmCoreModel.Instance.GetString(true), false /*isBound*/, null /*entitySetPath*/);
            edmAction.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            edmAction.AddParameter("param2", EdmCoreModel.Instance.GetString(true));
            var edmAction2 = new EdmAction("n.s", "DoStuff", EdmCoreModel.Instance.GetString(true), false /*isBound*/, null /*entitySetPath*/);
            edmAction2.AddParameter("param2", EdmCoreModel.Instance.GetInt32(true));
            edmAction2.AddParameter("param1", EdmCoreModel.Instance.GetInt32(true));

            EdmModel model = new EdmModel();
            model.AddElement(edmAction);
            model.AddElement(edmAction2);
            ValidateError(model, EdmErrorCode.DuplicateActions, Strings.EdmModel_Validator_Semantic_ModelDuplicateUnBoundActions("n.s.DoStuff"));
        }

        [Fact]
        public void UnBoundFunctionOverloadsDifferentParameterTypesShouldNotRaiseError()
        {
            var type1 = EdmCoreModel.GetCollection(new EdmEntityTypeReference(new EdmEntityType("n.s", "Entity1"), true));
            var type2 = EdmCoreModel.GetCollection(new EdmEntityTypeReference(new EdmEntityType("n.s", "Entity2"), true));

            var edmFunction = new EdmFunction("n.s", "GetStuff", type1, false, null, false);
            edmFunction.AddParameter("param1", type1);

            var edmFunction2 = new EdmFunction("n.s", "GetStuff", type1, false, null, false);
            edmFunction.AddParameter("param1", type2);
            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddElement(edmFunction2);
            ValidateNoError(model);
        }

        private static void ValidateNoError(IEdmModel model)
        {
            var context = ExecuteDuplicateOperationValidator(model);

            var errors = context.Errors.ToList();
            errors.Should().HaveCount(0);
        }

        private static ValidationContext ExecuteDuplicateOperationValidator(IEdmModel model)
        {
            ValidationContext context = new ValidationContext(model, (object o) => false);
            DuplicateOperationValidator validator = new DuplicateOperationValidator(context);
            foreach (var operation in model.SchemaElements.OfType<IEdmOperation>())
            {
                validator.ValidateNotDuplicate(operation, false /*skipError*/);
            }
            return context;
        }

        private static void ValidateError(IEdmModel model, EdmErrorCode expectedErrorCode, string expectedError)
        {
            var context = ExecuteDuplicateOperationValidator(model);
            var errors = context.Errors.ToList();
            errors.Should().HaveCount(1);
            errors[0].ErrorCode.Should().Be(expectedErrorCode);
            errors[0].ErrorMessage.Should().Be(expectedError);
        }
    }
}
