//---------------------------------------------------------------------
// <copyright file="ValidationRulesTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Validation
{
    /// <summary>
    /// ValidationRulesTests tests
    /// </summary>
    public class ValidationRulesTests
    {
        private static readonly IEdmEntityTypeReference DefaultValidEntityType = new EdmEntityTypeReference(new EdmEntityType("ds.n", "EntityType"), true);
        private static readonly IEdmCollectionTypeReference DefaultValidCollectionEntityType = new EdmCollectionTypeReference(new EdmCollectionType(DefaultValidEntityType));

        [Fact]
        public void OperationImportCannotImportBoundOperationTestShouldIndicateError()
        {
            EdmFunctionImport functionImport = new EdmFunctionImport(new EdmEntityContainer("ds.n", "Container"), "function1", new EdmFunction("ds", "func", EdmCoreModel.Instance.GetString(true), true /*IsBound*/, null, false));
            ValidateError(
                ValidationRules.OperationImportCannotImportBoundOperation, 
                functionImport, 
                EdmErrorCode.OperationImportCannotImportBoundOperation,
                Strings.EdmModel_Validator_Semantic_OperationImportCannotImportBoundOperation("function1", "func"));
        }

        [Fact]
        public void FunctionOverloadsWithDifferentReturnTypesAreInvalid()
        {
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true));
            var edmFunctionOverload = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetInt16(true));
            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddElement(edmFunctionOverload);
            ValidateError(
                ValidationRules.UnBoundFunctionOverloadsMustHaveIdenticalReturnTypes,
                model,
                EdmErrorCode.UnboundFunctionOverloadHasIncorrectReturnType,
                Strings.EdmModel_Validator_Semantic_UnboundFunctionOverloadHasIncorrectReturnType("GetStuff"));
        }
        
        #region EntityContainerDuplicateEntityContainerMemberName Tests
        
        [Fact]
        public void EntitySetAndOperationImportWithSameNameShouldError()
        {
            var container = new EdmEntityContainer("ns", "container");
            container.AddEntitySet("Set", DefaultValidEntityType.EntityDefinition());
            container.AddActionImport("Set", new EdmAction("n", "set", null));
            
            ValidateError(
                ValidationRules.EntityContainerDuplicateEntityContainerMemberName,
                container,
                EdmErrorCode.DuplicateEntityContainerMemberName,
                Strings.EdmModel_Validator_Semantic_DuplicateEntityContainerMemberName("Set"));
        }

        [Fact]
        public void OperationImportAndEntitySetWithSameNameShouldError()
        {
            var container = new EdmEntityContainer("ns", "container");
            container.AddActionImport("Set", new EdmAction("n", "set", null));
            container.AddEntitySet("Set", DefaultValidEntityType.EntityDefinition());
            
            ValidateError(
                ValidationRules.EntityContainerDuplicateEntityContainerMemberName,
                container,
                EdmErrorCode.DuplicateEntityContainerMemberName,
                Strings.EdmModel_Validator_Semantic_DuplicateEntityContainerMemberName("Set"));
        }

        [Fact]
        public void OperationImportsWithSameNameAndSameOperationShouldError()
        {
            var action = new EdmAction("n", "set", null);
            var container = new EdmEntityContainer("ns", "container");
            container.AddActionImport("Set", action);
            container.AddActionImport("Set", action);

            ValidateError(
                ValidationRules.EntityContainerDuplicateEntityContainerMemberName,
                container,
                EdmErrorCode.DuplicateEntityContainerMemberName,
                Strings.EdmModel_Validator_Semantic_DuplicateEntityContainerMemberName("Set"));
        }

        [Fact]
        public void OperationImportsWithSameNameAndDifferentOperationShouldNotError()
        {
            var action = new EdmAction("n", "set", null);
            var action2 = new EdmAction("n", "set2", null);
            var container = new EdmEntityContainer("ns", "container");
            container.AddActionImport("Set", action);
            container.AddActionImport("Set", action2);

            ValidateNoError(ValidationRules.EntityContainerDuplicateEntityContainerMemberName, new EdmModel(), container);
        }

        [Fact]
        public void EntitySetsWithSameNameShouldError()
        {
            var container = new EdmEntityContainer("ns", "container");
            container.AddEntitySet("Set", DefaultValidEntityType.EntityDefinition());
            container.AddEntitySet("Set", DefaultValidEntityType.EntityDefinition());

            ValidateError(
                ValidationRules.EntityContainerDuplicateEntityContainerMemberName,
                container,
                EdmErrorCode.DuplicateEntityContainerMemberName,
                Strings.EdmModel_Validator_Semantic_DuplicateEntityContainerMemberName("Set"));
        }

        #endregion

        #region OperationImportEntitySetExpressionIsInvalid  tests

        [Fact]
        public void OperationImportEntitySetReferencesEntitySetNotOnFunctionImportContainerShouldError()
        {
            var model = new EdmModel();
            var defaultContainer = new EdmEntityContainer("f", "d");
            model.AddElement(defaultContainer);
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true));
            edmFunction.AddParameter("param", DefaultValidEntityType);

            var edmFunctionImport = new EdmFunctionImport(defaultContainer, "GetStuff", edmFunction, new EdmPathExpression("Set"), true);
            ValidateError(
                ValidationRules.OperationImportEntitySetExpressionIsInvalid,
                model,
                edmFunctionImport,
                EdmErrorCode.OperationImportEntitySetExpressionIsInvalid,
                Strings.EdmModel_Validator_Semantic_OperationImportEntitySetExpressionIsInvalid("GetStuff"));
        }

        [Fact]
        public void OperationImportEntitySetReferencesEntitySetOnFunctionImportContainerShouldNotError()
        {
            var model = new EdmModel();
            var defaultContainer = new EdmEntityContainer("f", "d");
            defaultContainer.AddEntitySet("Set", DefaultValidEntityType.EntityDefinition());
            model.AddElement(defaultContainer);
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true));
            edmFunction.AddParameter("param", DefaultValidEntityType);

            var edmFunctionImport = new EdmFunctionImport(defaultContainer, "GetStuff", edmFunction, new EdmPathExpression("Set"), false);
            ValidateNoError(ValidationRules.OperationImportEntitySetExpressionIsInvalid, model, edmFunctionImport);
        }

        [Fact]
        public void OperationImportEntitySetReferenceSingletonShouldError()
        {
            var defaultContainer = new EdmEntityContainer("f", "d");
            var singleton = defaultContainer.AddSingleton("Singleton", DefaultValidEntityType.EntityDefinition());
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true));
            var edmAction = new EdmAction("n.s", "UpdateStuff", EdmCoreModel.Instance.GetString(true));

            var edmFunctionImport = new EdmFunctionImport(defaultContainer, "GetStuff", edmFunction, new EdmPathExpression("Schema.EntityContainer/Singleton"), true);
            ValidateError(
                ValidationRules.OperationImportEntitySetExpressionIsInvalid,
                edmFunctionImport,
                EdmErrorCode.OperationImportEntitySetExpressionIsInvalid,
                Strings.EdmModel_Validator_Semantic_OperationImportEntitySetExpressionIsInvalid("GetStuff"));

            var edmActionImport = new EdmActionImport(defaultContainer, "UpdateStuff", edmAction, new EdmPathExpression("Schema.EntityContainer/Singleton"));
            ValidateError(
                ValidationRules.OperationImportEntitySetExpressionIsInvalid,
                edmActionImport,
                EdmErrorCode.OperationImportEntitySetExpressionIsInvalid,
                Strings.EdmModel_Validator_Semantic_OperationImportEntitySetExpressionIsInvalid("UpdateStuff"));
        }

        [Fact]
        public void OperationImportEntitySetReferenceEntitySetTargetPathShouldNotError()
        {
            var model = new EdmModel();
            var defaultContainer = new EdmEntityContainer("f", "d");
            model.AddElement(defaultContainer);
            var entitySet = defaultContainer.AddEntitySet("EntitySet", DefaultValidEntityType.EntityDefinition());
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true));

            var edmFunctionImport = new EdmFunctionImport(defaultContainer, "GetStuff", edmFunction, new EdmPathExpression("EntitySet"), true);
            ValidateNoError(ValidationRules.OperationImportEntitySetExpressionIsInvalid, model, edmFunctionImport);
        }

        [Fact]
        public void OperationImportEntitySetReferencesBadEntityShouldNotError()
        {
            // Error should be skipped as this entityset will already be noted as a bad entity set by other rules anyway.
            var model = new EdmModel();
            var defaultContainer = new EdmEntityContainer("f", "d");
            model.AddElement(defaultContainer);
            var badEntitySet = new BadEntitySet("Set", defaultContainer, new List<EdmError>());
            defaultContainer.AddElement(badEntitySet);
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true));
            edmFunction.AddParameter("param", DefaultValidEntityType);

            var edmFunctionImport = new EdmFunctionImport(defaultContainer, "GetStuff", edmFunction, new EdmPathExpression("Set"), true);
            ValidationContext context = new ValidationContext(model, (object o) =>
            {
                if (o == badEntitySet)
                {
                    return true;
                }

                return false;
            });

            ValidationRules.OperationImportEntitySetExpressionIsInvalid.Evaluate(context, edmFunctionImport);
            var errors = context.Errors.ToList();
            errors.Should().HaveCount(0);
        }

        #endregion

        #region OperationImportEntityTypeDoesNotMatchEntitySet tests

        [Fact]
        public void OperationImportInvalidEntitySetWithNonEntityNonCollectionReturnType()
        {
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true));
            edmFunction.AddParameter("param", DefaultValidEntityType);
            var edmFunctionImport = new EdmFunctionImport(new EdmEntityContainer("f", "d"), "GetStuff", edmFunction, new EdmPathExpression("param"), true);
            ValidateError(
                ValidationRules.OperationImportEntityTypeDoesNotMatchEntitySet,
                edmFunctionImport,
                EdmErrorCode.OperationImportSpecifiesEntitySetButDoesNotReturnEntityType,
                Strings.EdmModel_Validator_Semantic_OperationImportSpecifiesEntitySetButNotEntityType("GetStuff"));
        }

        [Fact]
        public void OperationImportInvalidEntitySetWithNonEntityCollectionReturnType()
        {
            var edmFunction = new EdmFunction("n.s", "GetStuff", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))));
            edmFunction.AddParameter("param", DefaultValidEntityType);
            var edmFunctionImport = new EdmFunctionImport(new EdmEntityContainer("f", "d"), "GetStuff", edmFunction, new EdmPathExpression("param"), true);
            ValidateError(
                ValidationRules.OperationImportEntityTypeDoesNotMatchEntitySet,
                edmFunctionImport,
                EdmErrorCode.OperationImportSpecifiesEntitySetButDoesNotReturnEntityType,
                Strings.EdmModel_Validator_Semantic_OperationImportSpecifiesEntitySetButNotEntityType("GetStuff"));
        }

        [Fact]
        public void OperationImportInvalidEntitySetShouldNotReturnErrorIfNoEntitySetIsSpecified()
        {
            var edmFunction = new EdmFunction("n.s", "GetStuff", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))));
            edmFunction.AddParameter("param", DefaultValidEntityType);
            var edmFunctionImport = new EdmFunctionImport(new EdmEntityContainer("f", "d"), "GetStuff", edmFunction, null, true);
            ValidateNoError(ValidationRules.OperationImportEntityTypeDoesNotMatchEntitySet, new EdmModel(), edmFunctionImport);
        }

        #endregion

        #region OperationEntitySetPathMustBeValid Tests

        [Fact]
        public void ShortIntegrationTestEnsuresTryGetRelativeEntitySetInvoked()
        {
            EdmFunction function = new EdmFunction("ns", "GetStuff", EdmCoreModel.Instance.GetString(false), false /*isBound*/, new EdmPathExpression("path1", "path2"), false);
            function.AddParameter("param", DefaultValidCollectionEntityType);
            EdmModel model = new EdmModel();
            model.AddElement(function);
            ValidateErrorInList(
                ValidationRules.OperationEntitySetPathMustBeValid,
                model,
                function,
                EdmErrorCode.OperationCannotHaveEntitySetPathWithUnBoundOperation,
                Strings.EdmModel_Validator_Semantic_OperationCannotHaveEntitySetPathWithUnBoundOperation(function.Name));
        }

        [Fact]
        public void ShortIntegrationTestEnsuresTryGetRelativeEntitySetInvoked2()
        {
            EdmFunction function = new EdmFunction("ns", "GetStuff", DefaultValidEntityType, true /*isBound*/, new EdmPathExpression(), false);
            function.AddParameter("param", EdmCoreModel.Instance.GetString(false));
            EdmModel model = new EdmModel();
            model.AddElement(function);
            ValidateError(
                ValidationRules.OperationEntitySetPathMustBeValid,
                model,
                function,
                EdmErrorCode.OperationWithInvalidEntitySetPathMissingCompletePath,
                Strings.EdmModel_Validator_Semantic_InvalidEntitySetPathMissingBindingParameterName("EntitySetPath"));
        }
        #endregion

        #region OperationReturnTypeEntityTypeMustBeValid tests
        [Fact]
        public void OperationWithEntitySetPathWithNonEntityReturnTypeMustFail()
        {
            EdmFunction function = new EdmFunction("ns", "GetStuff", EdmCoreModel.Instance.GetString(false), false /*isBound*/, new EdmPathExpression("param"), false);
            function.AddParameter("param", DefaultValidCollectionEntityType);
            EdmModel model = new EdmModel();
            ValidateErrorInList(
                ValidationRules.OperationReturnTypeEntityTypeMustBeValid,
                model,
                function,
                EdmErrorCode.OperationWithEntitySetPathReturnTypeInvalid,
                Strings.EdmModel_Validator_Semantic_OperationWithEntitySetPathReturnTypeInvalid(function.Name));
        }

        [Fact]
        public void OperationWithEntitySetPathWithNonEntityCollectionReturnTypeMustFail()
        {
            EdmFunction function = new EdmFunction("ns", "GetStuff", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false))), true /*isBound*/, new EdmPathExpression("param"), false);
            function.AddParameter("param", DefaultValidCollectionEntityType);
            EdmModel model = new EdmModel();
            model.AddElement(function);
            ValidateErrorInList(
                ValidationRules.OperationReturnTypeEntityTypeMustBeValid,
                model,
                function,
                EdmErrorCode.OperationWithEntitySetPathReturnTypeInvalid,
                Strings.EdmModel_Validator_Semantic_OperationWithEntitySetPathReturnTypeInvalid(function.Name));
        }

        [Fact]
        public void ValidateEntitySetPathResolvedTypeFailsWithNotAssignableToReturnTypeError()
        {
            OperationOperationEntitySetPathMustBeValidValidTestModel testModelContainer = new OperationOperationEntitySetPathMustBeValidValidTestModel();

            EdmFunction function = new EdmFunction("ns", "GetStuff", new EdmEntityTypeReference(testModelContainer.T3, false), true /*isBound*/, new EdmPathExpression("bindingEntity/ColP101/Nav1"), false);
            function.AddParameter("bindingEntity", new EdmEntityTypeReference(testModelContainer.T1DerivedFromT2, false));

            ValidateErrorInList(
                ValidationRules.OperationReturnTypeEntityTypeMustBeValid,
                testModelContainer.Model,
                function,
                EdmErrorCode.OperationWithEntitySetPathAndReturnTypeTypeNotAssignable,
                Strings.EdmModel_Validator_Semantic_OperationWithEntitySetPathAndReturnTypeTypeNotAssignable(function.Name, testModelContainer.T3.FullName(), testModelContainer.T2.FullName()));
        }

        [Fact]
        public void ValidateNonEntityReturnTypeInvalid()
        {
            OperationOperationEntitySetPathMustBeValidValidTestModel testModelContainer = new OperationOperationEntitySetPathMustBeValidValidTestModel();

            EdmFunction function = new EdmFunction("ns", "GetStuff", new EdmComplexTypeReference(new EdmComplexType("ns","complexType"), false), true /*isBound*/, new EdmPathExpression("bindingEntity/ColNav"), false);
            function.AddParameter("bindingEntity", new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(testModelContainer.T3, false))));

            ValidateErrorInList(
                ValidationRules.OperationReturnTypeEntityTypeMustBeValid,
                testModelContainer.Model,
                function,
                EdmErrorCode.OperationWithEntitySetPathReturnTypeInvalid,
                Strings.EdmModel_Validator_Semantic_OperationWithEntitySetPathReturnTypeInvalid(function.Name));
        }

        [Fact]
        public void ValidateEntitySetPathInvalidAsResolvesToCollectionEntityTypeWhenReturnTypeIsEntityType()
        {
            OperationOperationEntitySetPathMustBeValidValidTestModel testModelContainer = new OperationOperationEntitySetPathMustBeValidValidTestModel();

            EdmFunction function = new EdmFunction("ns", "GetStuff", new EdmEntityTypeReference(testModelContainer.T2, false), true /*isBound*/, new EdmPathExpression("bindingEntity/ColNav"), false);
            function.AddParameter("bindingEntity", new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(testModelContainer.T3, false))));

            ValidateErrorInList(
                ValidationRules.OperationReturnTypeEntityTypeMustBeValid, 
                testModelContainer.Model, 
                function,
                EdmErrorCode.OperationWithEntitySetPathResolvesToEntityTypeMismatchesCollectionEntityTypeReturnType,
                Strings.EdmModel_Validator_Semantic_OperationWithEntitySetPathResolvesToEntityTypeMismatchesCollectionEntityTypeReturnType(function.Name));
        }

        [Fact]
        public void ValidateEntitySetPathInvalidAsResolvesToEntityTypeWhenReturnTypeIsCollectionEntityType()
        {
            OperationOperationEntitySetPathMustBeValidValidTestModel testModelContainer = new OperationOperationEntitySetPathMustBeValidValidTestModel();

            var returnType = new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(testModelContainer.T3, false)));
            EdmFunction function = new EdmFunction("ns", "GetStuff", returnType, true /*isBound*/, new EdmPathExpression("bindingEntity/ColNav/RefNav"), false);
            function.AddParameter("bindingEntity", new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(testModelContainer.T3, false))));

            ValidateErrorInList(
                ValidationRules.OperationReturnTypeEntityTypeMustBeValid,
                testModelContainer.Model,
                function,
                EdmErrorCode.OperationWithEntitySetPathResolvesToCollectionEntityTypeMismatchesEntityTypeReturnType,
                Strings.EdmModel_Validator_Semantic_OperationWithEntitySetPathResolvesToCollectionEntityTypeMismatchesEntityTypeReturnType(function.Name));
        }

        [Fact]
        public void ValidateReturnTypeShouldReturnNoErrorWhenNoEntitySetPathNotValid()
        {
            OperationOperationEntitySetPathMustBeValidValidTestModel testModelContainer = new OperationOperationEntitySetPathMustBeValidValidTestModel();

            EdmFunction function = new EdmFunction("ns", "GetStuff", EdmCoreModel.Instance.GetString(false), true /*isBound*/, new EdmPathExpression("bindingEntity/Nav1/Bunk.T1Foo"), false);
            
            ValidateNoError(ValidationRules.OperationReturnTypeEntityTypeMustBeValid, testModelContainer.Model, function);
        }

        internal class OperationOperationEntitySetPathMustBeValidValidTestModel
        {
            public OperationOperationEntitySetPathMustBeValidValidTestModel()
            {
                this.Model = new EdmModel();

                this.T3 = new EdmEntityType("Bunk", "T3");
                this.T2 = new EdmEntityType("Bunk", "T2");
                this.T1DerivedFromT2 = new EdmEntityType("Bunk", "T1", this.T2);

                EdmStructuralProperty f31 = this.T3.AddStructuralProperty("F31", EdmCoreModel.Instance.GetString(false));
                this.T3.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() {Name = "Nav1", Target = this.T2, TargetMultiplicity = EdmMultiplicity.One });
                this.T3.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "ColNav", Target = this.T2, TargetMultiplicity = EdmMultiplicity.Many });
                this.T3.AddKeys(f31);
                this.Model.AddElement(this.T3);

                EdmStructuralProperty f11 = this.T2.AddStructuralProperty("F11", EdmCoreModel.Instance.GetString(false));
                this.T2.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "RefNav", Target = this.T3, TargetMultiplicity = EdmMultiplicity.One });
                this.T2.AddKeys(f11);
                this.Model.AddElement(this.T2);

                this.T1DerivedFromT2.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "P101", Target = this.T3, TargetMultiplicity = EdmMultiplicity.One });
                this.T1DerivedFromT2.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "ColP101", Target = this.T3, TargetMultiplicity = EdmMultiplicity.Many });
                this.Model.AddElement(this.T1DerivedFromT2);
            }

            public EdmModel Model { get; private set; }

            public EdmEntityType T3 { get; private set; }

            public EdmEntityType T2 { get; private set; }

            public EdmEntityType T1DerivedFromT2 { get; private set; }
        }

        #endregion

        #region FunctionImportWithParameterShouldNotBeIncludedInServiceDocument tests
        [Fact]
        public void FunctionImportIncludedInServiceDocumentHasParametersShouldError()
        {
            EdmModel model = new EdmModel();

            var function = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true));
            function.AddParameter("Id", EdmCoreModel.Instance.GetInt32(true));
            model.AddElement(function);

            var container = new EdmEntityContainer("ns", "container");
            container.AddEntitySet("Set", DefaultValidEntityType.EntityDefinition());
            var functionImport = container.AddFunctionImport("OtherName", function, new EdmPathExpression("Set"), true) as IEdmFunctionImport;
            model.AddElement(container);

            ValidateError(
                ValidationRules.FunctionImportWithParameterShouldNotBeIncludedInServiceDocument,
                model,
                functionImport,
                EdmErrorCode.FunctionImportWithParameterShouldNotBeIncludedInServiceDocument,
                Strings.EdmModel_Validator_Semantic_FunctionImportWithParameterShouldNotBeIncludedInServiceDocument("OtherName"));
        }

        [Fact]
        public void FunctionImportNotIncludedInServiceDocumentHasParametersShouldNotError()
        {
            EdmModel model = new EdmModel();

            var function = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true));
            function.AddParameter("Id", EdmCoreModel.Instance.GetInt32(true));
            model.AddElement(function);

            var container = new EdmEntityContainer("ns", "container");
            container.AddEntitySet("Set", DefaultValidEntityType.EntityDefinition());
            var functionImport = container.AddFunctionImport("OtherName", function, new EdmPathExpression("Set"), false) as IEdmFunctionImport;
            model.AddElement(container);

            ValidateNoError(
                ValidationRules.FunctionImportWithParameterShouldNotBeIncludedInServiceDocument,
                model,
                functionImport);
        }
        #endregion

        #region ModelBoundFunctionOverloadsMustHaveSameReturnType Tests
        [Fact]
        public void FunctionOverloadsWithSameBindingTypesAndSameNameWithDifferentReturnTypesShouldError()
        {
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt16(true));
            var edmFunction2 = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetInt16(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction2.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt16(true));

            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddElement(edmFunction2);
            ValidateError(
                ValidationRules.ModelBoundFunctionOverloadsMustHaveSameReturnType,
                model,
                EdmErrorCode.BoundFunctionOverloadsMustHaveSameReturnType,
                Strings.EdmModel_Validator_Semantic_BoundFunctionOverloadsMustHaveSameReturnType("GetStuff", edmFunction.ReturnType.FullName()));
        }

        [Fact]
        public void FunctionOverloadsWithSameBindingTypesAndSameNameWithDifferentReturnTypesButWithOneInvalidFunctionWithNoParametersShouldNotError()
        {
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt16(true));
            var edmFunction2 = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetInt16(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);

            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddElement(edmFunction2);
            ValidateNoError(ValidationRules.ModelBoundFunctionOverloadsMustHaveSameReturnType, model, model);
        }

        [Fact]
        public void FunctionOverloadsWithSameBindingTypesAndSameNameWithDifferentReturnTypesButWithOneInvalidFunctionWithNullReturnTypeShouldNotError()
        {
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt16(true));
            var edmFunction2 = new TestFunction("n.s", "GetStuff") {IsBound = true};

            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddElement(edmFunction2);
            ValidateNoError(ValidationRules.ModelBoundFunctionOverloadsMustHaveSameReturnType, model, model);
        }

        [Fact]
        public void FunctionOverloadsWithSameBindingTypesAndSameNameWithDifferentReturnTypesShouldErrorMultipleTimesAndNotStopAfterOne()
        {
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt16(true));
            var edmFunction2 = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetInt16(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction2.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt16(true));
            var edmFunction3 = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetInt32(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction3.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt16(true));

            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddElement(edmFunction2);
            model.AddElement(edmFunction3);
            ValidateExactErrorsInList(
                ValidationRules.ModelBoundFunctionOverloadsMustHaveSameReturnType,
                model,
                model,
                new Tuple<EdmErrorCode, string>(EdmErrorCode.BoundFunctionOverloadsMustHaveSameReturnType, Strings.EdmModel_Validator_Semantic_BoundFunctionOverloadsMustHaveSameReturnType("GetStuff", edmFunction.ReturnType.FullName())),
                new Tuple<EdmErrorCode, string>(EdmErrorCode.BoundFunctionOverloadsMustHaveSameReturnType, Strings.EdmModel_Validator_Semantic_BoundFunctionOverloadsMustHaveSameReturnType("GetStuff", edmFunction.ReturnType.FullName())));
        }
        #endregion

        #region ModelDuplicateFunctions Tests

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
            ValidateNoError(ValidationRules.ModelDuplicateSchemaElementName, model, model);
        }

        /// <summary>
        ///  This is a short integration test to ensure that errors from the DuplicateOperationValidatorOccur through the ModelDuplicateSchemaElementName 
        /// </summary>
        [Fact]
        public void DuplicateFunctionsDuplicateFunctionErrorShouldOccur()
        {
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt16(true));
            edmFunction.AddParameter("otherParameter2", EdmCoreModel.Instance.GetInt16(true));
            var edmFunction2 = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetInt16(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction2.AddParameter("otherParameter", EdmCoreModel.Instance.GetInt16(true));
            edmFunction2.AddParameter("myparam", EdmCoreModel.Instance.GetInt16(true));

            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddElement(edmFunction2);
            ValidateError(
                ValidationRules.ModelDuplicateSchemaElementName, 
                model, 
                EdmErrorCode.DuplicateFunctions,
                Strings.EdmModel_Validator_Semantic_ModelDuplicateBoundFunctionParameterTypes("n.s.GetStuff"));
        }

        // TODO: Need to add tests where a function or action conflict with an existing Type, should fail.
        [Fact]
        public void FunctionWithSameNameAsEntityTypeShouldError()
        {
            var entityType = new EdmEntityType("n.s", "GetStuff");
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt16(true));
            edmFunction.AddParameter("otherParameter2", EdmCoreModel.Instance.GetInt16(true));

            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddElement(entityType);
            ValidateError(
                ValidationRules.ModelDuplicateSchemaElementName,
                model,
                EdmErrorCode.AlreadyDefined,
                Strings.EdmModel_Validator_Semantic_SchemaElementNameAlreadyDefined("n.s.GetStuff"));
        }

        // TODO: Need to add tests where a function or action conflict with an existing Type, in a referenced schema, should fail.
        [Fact]
        public void FunctionWithSameNameAsEntityTypeInOtherModelShouldError()
        {
            EdmModel otherModel = new EdmModel();
            var entityType = new EdmEntityType("n.s", "GetStuff");
            otherModel.AddElement(entityType);

            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt16(true));
            edmFunction.AddParameter("otherParameter2", EdmCoreModel.Instance.GetInt16(true));

            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddReferencedModel(otherModel);
            ValidateError(
                ValidationRules.ModelDuplicateSchemaElementName,
                model,
                EdmErrorCode.AlreadyDefined,
                Strings.EdmModel_Validator_Semantic_SchemaElementNameAlreadyDefined("n.s.GetStuff"));
        }

        /// <summary>
        /// Validates that checks are happening against referenced models. Full set of referenced model tests are in ValidationHelperTests
        /// This test ensures that model.OperationOrNameExistsInReferencedModel method is called within the validation sequence.
        /// </summary>
        [Fact]
        public void SameFunctionsShouldInWithOneInReferencedModelShouldRaiseError()
        {
            EdmModel otherModel = new EdmModel();
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt16(true));
            edmFunction.AddParameter("otherParameter2", EdmCoreModel.Instance.GetInt16(true));
            otherModel.AddElement(edmFunction);

            var edmFunction2 = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            edmFunction2.AddParameter("bindingParameter", EdmCoreModel.Instance.GetInt16(true));
            edmFunction2.AddParameter("otherParameter2", EdmCoreModel.Instance.GetInt16(true));

            EdmModel model = new EdmModel();
            model.AddElement(edmFunction2);
            model.AddReferencedModel(otherModel);
            ValidateError(
                ValidationRules.ModelDuplicateSchemaElementName,
                model,
                EdmErrorCode.AlreadyDefined,
                Strings.EdmModel_Validator_Semantic_SchemaElementNameAlreadyDefined("n.s.GetStuff"));
        }
        #endregion

        [Fact]
        public void NavigationPropertyWrongMultiplicityForDependent()
        {
            EdmEntityType type1 = new EdmEntityType("ns", "type1");
            var Id1 = type1.AddStructuralProperty("Id1", EdmCoreModel.Instance.GetInt32(false));

            EdmEntityType type2 = new EdmEntityType("ns", "type2");
            var Id2 = type2.AddStructuralProperty("Id2", EdmCoreModel.Instance.GetInt32(false));
            
            var foreignKey = type2.AddStructuralProperty("foreignKey", EdmCoreModel.Instance.GetInt32(true));

            type1.AddKeys(Id1);
            type2.AddKeys(Id2);

            var navInfo1 = new EdmNavigationPropertyInfo()
            {
                Name = "nav1",
                Target = type2,
                TargetMultiplicity = EdmMultiplicity.One,
            };

            var navInfo2 = new EdmNavigationPropertyInfo()
            {
                Name = "nav2",
                Target = type1,
                TargetMultiplicity = EdmMultiplicity.One,
                DependentProperties = new[] { foreignKey },
                PrincipalProperties = new[] { Id1 },
            };

            EdmNavigationProperty navProp = type2.AddBidirectionalNavigation(navInfo2, navInfo1);

            ValidateError(
                ValidationRules.NavigationPropertyDependentEndMultiplicity, 
                navProp,
                EdmErrorCode.InvalidMultiplicityOfDependentEnd,
                Strings.EdmModel_Validator_Semantic_InvalidMultiplicityOfDependentEndMustBeMany("nav2"));
        }

        [Fact]
        public void NavigationPropertyTypeMismatchOnReferentialConstraint()
        {
            EdmEntityType type1 = new EdmEntityType("ns", "type1");
            var Id1 = type1.AddStructuralProperty("Id1", EdmCoreModel.Instance.GetInt32(false));

            EdmEntityType type2 = new EdmEntityType("ns", "type2");
            var Id2 = type2.AddStructuralProperty("Id2", EdmCoreModel.Instance.GetInt32(false));

            var foreignKey = type2.AddStructuralProperty("foreignKey", EdmCoreModel.Instance.GetString(false));

            type1.AddKeys(Id1);
            type2.AddKeys(Id2);

            var navInfo1 = new EdmNavigationPropertyInfo()
            {
                Name = "nav1",
                Target = type2,
                TargetMultiplicity = EdmMultiplicity.Many,
            };

            var navInfo2 = new EdmNavigationPropertyInfo()
            {
                Name = "nav2",
                Target = type1,
                TargetMultiplicity = EdmMultiplicity.One,
                DependentProperties = new[] { foreignKey },
                PrincipalProperties = new[] { Id1 },
            };

            EdmNavigationProperty navProp = type2.AddBidirectionalNavigation(navInfo2, navInfo1);

            ValidateError(
                ValidationRules.NavigationPropertyTypeMismatchRelationshipConstraint,
                navProp,
                EdmErrorCode.TypeMismatchRelationshipConstraint,
                Strings.EdmModel_Validator_Semantic_TypeMismatchRelationshipConstraint("foreignKey","ns.type2","Id1", "type1", "Fred"));
        }

        [Fact]
        public void OperationWithDuplicateParameterNameShouldError()
        {
            var function = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), false /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            function.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            function.AddParameter("param1", EdmCoreModel.Instance.GetInt16(true));

            EdmModel model = new EdmModel();
            model.AddElement(function);

            ValidateError(
                ValidationRules.OperationParameterNameAlreadyDefinedDuplicate,
                model,
                function,
                EdmErrorCode.AlreadyDefined,
                Strings.EdmModel_Validator_Semantic_ParameterNameAlreadyDefinedDuplicate("param1"));
        }

        [Fact]
        public void OperationWithOptionalParametersBeforeRequiredShouldError()
        {
            var function = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), false /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            function.AddOptionalParameter("param1", EdmCoreModel.Instance.GetString(true));
            function.AddParameter("param2", EdmCoreModel.Instance.GetInt16(true));

            EdmModel model = new EdmModel();
            model.AddElement(function);

            ValidateError(
                ValidationRules.OptionalParametersMustComeAfterRequiredParameters,
                model,
                function,
                EdmErrorCode.RequiredParametersMustPrecedeOptional,
                Strings.EdmModel_Validator_Semantic_RequiredParametersMustPrecedeOptional("param2"));
        }

        [Fact]
        public void BoundOperationWithoutParameterShouldError()
        {
            var function = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);

            EdmModel model = new EdmModel();
            model.AddElement(function);

            ValidateError(
                ValidationRules.BoundOperationMustHaveParameters,
                model,
                function,
                EdmErrorCode.BoundOperationMustHaveParameters,
                Strings.EdmModel_Validator_Semantic_BoundOperationMustHaveParameters(function.Name));
        }

        [Fact]
        public void BoundOperationWithOnlyOptionalParametersShouldError()
        {
            var function = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            function.AddOptionalParameter("param1", EdmCoreModel.Instance.GetString(true));

            EdmModel model = new EdmModel();
            model.AddElement(function);

            ValidateError(
                ValidationRules.BoundOperationMustHaveParameters,
                model,
                function,
                EdmErrorCode.BoundOperationMustHaveParameters,
                Strings.EdmModel_Validator_Semantic_BoundOperationMustHaveParameters(function.Name));
        }

        [Fact]
        public void TestInterfaceSingletonTypeOfCollectionOfEntityTypeModel()
        {
            var model = new EdmModel();

            var entity = new EdmEntityType("NS", "Entity");
            var entityId = entity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(/*isNullable*/false));
            entity.AddKeys(entityId);
            model.AddElement(entity);

            var entityContainer = new EdmEntityContainer("NS", "Container");
            model.AddElement(entityContainer);

            var singleton = new CustomSingleton(entityContainer, "Set", null);
            singleton.Type = new EdmCollectionType(new EdmEntityTypeReference(entity, false));
            entityContainer.AddElement(singleton);

            ValidateError(
                ValidationRules.SingletonTypeMustBeEntityType,
                model,
                singleton,
                EdmErrorCode.SingletonTypeMustBeEntityType,
                Strings.EdmModel_Validator_Semantic_SingletonTypeMustBeEntityType("Collection(NS.Entity)", "Set"));
        }

        [Fact]
        public void TestInterfaceSingletonTypeOfComplexType()
        {
            var model = new EdmModel();

            var complexType = new EdmComplexType("NS", "ComplexType");
            complexType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(/*isNullable*/true));
            model.AddElement(complexType);

            var entityContainer = new EdmEntityContainer("NS", "Container");
            model.AddElement(entityContainer);

            var singleton = new CustomSingleton(entityContainer, "Set", null);
            singleton.Type = complexType;
            entityContainer.AddElement(singleton);

            ValidateError(
                ValidationRules.SingletonTypeMustBeEntityType,
                model,
                singleton,
                EdmErrorCode.SingletonTypeMustBeEntityType,
                Strings.EdmModel_Validator_Semantic_SingletonTypeMustBeEntityType("NS.ComplexType", "Set"));
        }

        [Fact]
        public void TestInterfaceEntitySetTypeOfCollectionOfComplexTypeModel()
        {
            var model = new EdmModel();

            var complexType = new EdmComplexType("NS", "ComplexType");
            complexType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(/*isNullable*/true));
            model.AddElement(complexType);

            var entityContainer = new EdmEntityContainer("NS", "Container");
            model.AddElement(entityContainer);

            var entitySet = new CustomEntitySet(entityContainer, "Set", null);
            entitySet.Type = new EdmCollectionType(new EdmComplexTypeReference(complexType, false));
            entityContainer.AddElement(entitySet);

            ValidateError(
                ValidationRules.EntitySetTypeMustBeCollectionOfEntityType,
                model,
                entitySet,
                EdmErrorCode.EntitySetTypeMustBeCollectionOfEntityType,
                Strings.EdmModel_Validator_Semantic_EntitySetTypeMustBeCollectionOfEntityType("Collection(NS.ComplexType)", "Set"));
        }

        [Fact]
        public void TestInterfaceEntitySetTypeOfComplexTypeModel()
        {
            var model = new EdmModel();

            var complexType = new EdmComplexType("NS", "ComplexType");
            complexType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(/*isNullable*/true));
            model.AddElement(complexType);

            var entityContainer = new EdmEntityContainer("NS", "Container");
            model.AddElement(entityContainer);

            var entitySet = new CustomEntitySet(entityContainer, "Set", null);
            entitySet.Type = complexType;
            entityContainer.AddElement(entitySet);

            ValidateError(
                ValidationRules.EntitySetTypeMustBeCollectionOfEntityType,
                model,
                entitySet,
                EdmErrorCode.EntitySetTypeMustBeCollectionOfEntityType,
                Strings.EdmModel_Validator_Semantic_EntitySetTypeMustBeCollectionOfEntityType("NS.ComplexType", "Set"));
        }

        [Fact]
        public void TestInterfaceEntitySetTypeOfNonCollectionModelModel()
        {
            var model = new EdmModel();

            var entity = new EdmEntityType("NS", "Entity");
            var entityId = entity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(/*isNullable*/false));
            entity.AddKeys(entityId);
            model.AddElement(entity);

            var entityContainer = new EdmEntityContainer("NS", "Container");
            model.AddElement(entityContainer);

            var entitySet = new CustomEntitySet(entityContainer, "Set", null);
            entitySet.Type = entity;
            entityContainer.AddElement(entitySet);

            ValidateError(
                ValidationRules.EntitySetTypeMustBeCollectionOfEntityType,
                model,
                entitySet,
                EdmErrorCode.EntitySetTypeMustBeCollectionOfEntityType,
                Strings.EdmModel_Validator_Semantic_EntitySetTypeMustBeCollectionOfEntityType("NS.Entity", "Set"));
        }

        private static void ValidateNoError<T>(ValidationRule<T> validationRule, IEdmModel model, T item) where T : IEdmElement
        {
            ValidationContext context = new ValidationContext(model, (object o) => false);
            validationRule.Evaluate(context, item);
            var errors = context.Errors.ToList();
            errors.Should().HaveCount(0);
        }

        private static void ValidateError<T>(ValidationRule<T> validationRule, IEdmModel model, T item, EdmErrorCode expectedErrorCode, string expectedError) where T : IEdmElement
        {
            ValidationContext context = new ValidationContext(model, (object o) => false);
            validationRule.Evaluate(context, item);
            var errors = context.Errors.ToList();
            errors.Should().HaveCount(1);
            errors[0].ErrorCode.Should().Be(expectedErrorCode);
            errors[0].ErrorMessage.Should().Be(expectedError);
        }

        private static void ValidateExactErrorsInList<T>(ValidationRule<T> validationRule, IEdmModel model, T item, params Tuple<EdmErrorCode,string> [] expectedErrors) where T : IEdmElement
        {
            ValidationContext context = new ValidationContext(model, (object o) => false);
            validationRule.Evaluate(context, item);
            int currentIndex = 0;
            foreach(var actualError in context.Errors)
            {
                actualError.ErrorCode.Should().Be(expectedErrors[currentIndex].Item1);
                actualError.ErrorMessage.Should().Be(expectedErrors[currentIndex].Item2);
                currentIndex++;
            }

            context.Errors.ToList().Count.Should().Be(expectedErrors.Length);
        }

        private static void ValidateErrorInList<T>(ValidationRule<T> validationRule, IEdmModel model, T item, EdmErrorCode expectedErrorCode, string expectedError) where T : IEdmElement
        {
            ValidationContext context = new ValidationContext(model, (object o) => false);
            validationRule.Evaluate(context, item);
            var error = context.Errors.SingleOrDefault(e=>e.ErrorCode == expectedErrorCode);
            error.Should().NotBeNull();
            error.ErrorMessage.Should().Be(expectedError);
        }

        private static void ValidateError<T>(ValidationRule<T> validationRule, T item, EdmErrorCode expectedErrorCode, string expectedError) where T:IEdmElement
        {
            ValidateError(validationRule, new EdmModel(), item, expectedErrorCode, expectedError);
        }

        private class TestFunction : IEdmFunction
        {
            internal TestFunction(string namespaceName, string name)
            {
                this.Name = name;
                this.Namespace = namespaceName;
                this.IsComposable = false;
                this.ReturnType = null;
                this.Parameters = new Collection<IEdmOperationParameter>();
                this.IsBound = false;
            }

            public bool IsComposable { get; set; }

            public IEdmTypeReference ReturnType { get; set; }

            public IEnumerable<IEdmOperationParameter> Parameters { get; set; }

            public bool IsBound { get; set; }

            public IEdmPathExpression EntitySetPath { get; set; }

            public IEdmOperationParameter FindParameter(string name)
            {
               return this.Parameters.Single(p=>p.Name == name);
            }

            public EdmSchemaElementKind SchemaElementKind
            {
                get { return EdmSchemaElementKind.Function; }
            }

            public string Namespace { get; set; }

            public string Name { get; set; }
        }

        private sealed class CustomEntitySet : EdmNamedElement, IEdmEntitySet
        {
            private readonly IEdmEntityContainer container;
            private IEdmType type;
            private readonly List<IEdmNavigationPropertyBinding> navigationPropertyBindings = new List<IEdmNavigationPropertyBinding>();

            public CustomEntitySet(IEdmEntityContainer container, string name, IEdmEntityType elementType)
                : base(name)
            {
                this.container = container;
                if (elementType != null)
                {
                    this.type = new EdmCollectionType(new EdmEntityTypeReference(elementType, false));
                }
            }

            public IEnumerable<IEdmNavigationPropertyBinding> NavigationPropertyBindings
            {
                get { return this.navigationPropertyBindings; }
            }

            public void AddNavigationTarget(IEdmNavigationProperty property, IEdmEntitySet target)
            {
                this.navigationPropertyBindings.Add(new EdmNavigationPropertyBinding(property, target));
            }

            public IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty)
            {
                throw new System.NotImplementedException();
            }

            public IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty, IEdmPathExpression bindingPath)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<IEdmNavigationPropertyBinding> FindNavigationPropertyBindings(IEdmNavigationProperty navigationProperty)
            {
                throw new NotImplementedException();
            }

            public EdmContainerElementKind ContainerElementKind
            {
                get { return EdmContainerElementKind.EntitySet; }
            }

            public IEdmEntityContainer Container
            {
                get { return this.container; }
            }

            public IEdmPathExpression Path
            {
                get { return null; }
            }

            public IEdmType Type
            {
                get { return type; }
                set { type = value; }
            }

            public bool IncludeInServiceDocument
            {
                get; set;
            }
        }

        private sealed class CustomSingleton : EdmNamedElement, IEdmSingleton
        {
            private readonly IEdmEntityContainer container;
            private IEdmType type;
            private readonly List<IEdmNavigationPropertyBinding> navigationPropertyBindings = new List<IEdmNavigationPropertyBinding>();

            public CustomSingleton(IEdmEntityContainer container, string name, IEdmEntityType entityType)
                : base(name)
            {
                this.container = container;
                type = entityType;
            }

            public IEnumerable<IEdmNavigationPropertyBinding> NavigationPropertyBindings
            {
                get { return this.navigationPropertyBindings; }
            }

            public void AddNavigationTarget(IEdmNavigationProperty property, IEdmEntitySet target)
            {
                this.navigationPropertyBindings.Add(new EdmNavigationPropertyBinding(property, target));
            }

            public IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty)
            {
                throw new System.NotImplementedException();
            }

            public IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty, IEdmPathExpression bindingPath)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<IEdmNavigationPropertyBinding> FindNavigationPropertyBindings(IEdmNavigationProperty navigationProperty)
            {
                throw new NotImplementedException();
            }

            public EdmContainerElementKind ContainerElementKind
            {
                get { return EdmContainerElementKind.Singleton; }
            }

            public IEdmEntityContainer Container
            {
                get { return this.container; }
            }

            public IEdmPathExpression Path
            {
                get { return null; }
            }

            public IEdmType Type
            {
                get { return type; }
                set { type = value; }
            }
        }
    }
}
