//---------------------------------------------------------------------
// <copyright file="ExtensionMethodTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Annotations;
using Microsoft.OData.Edm.Library.Expressions;
using Microsoft.OData.Edm.Tests;
using Microsoft.OData.Edm.Tests.Validation;
using Microsoft.OData.Edm.Validation;
using Xunit;

namespace Microsoft.OData.Edm.Tests.ExtensionMethods
{
    public class ExtensionMethodTests
    {
        const string DefaultNamespace = "TestModel";
        const string DefaultName = "TestName";
        readonly EdmComplexType edmComplexType = new EdmComplexType(DefaultNamespace, DefaultName);
        readonly EdmEntityType edmEntityType = new EdmEntityType(DefaultNamespace, DefaultName);

        private static readonly IEdmEntityType DefaultValidEntityType = new EdmEntityType("ds.n", "EntityType");
        private static readonly IEdmEntityType DefaultDerivedDefaultEntityType = new EdmEntityType("ds.n", "DerivedEntityType", DefaultValidEntityType);
        private static readonly IEdmEntityTypeReference DefaultValidEntityTypeRef = new EdmEntityTypeReference(DefaultValidEntityType, true);
        private static readonly IEdmEntityTypeReference DefaultDerivedValidEntityTypeRef = new EdmEntityTypeReference(DefaultDerivedDefaultEntityType, true);
        private static readonly IEdmCollectionType DefaultValidCollectionEntityType = new EdmCollectionType(DefaultValidEntityTypeRef);
        private static readonly IEdmCollectionType DefaultDerivedValidCollectionEntityType = new EdmCollectionType(DefaultDerivedValidEntityTypeRef);
        private static readonly IEdmCollectionTypeReference DefaultValidCollectionEntityTypeRef = new EdmCollectionTypeReference(DefaultValidCollectionEntityType);
        private static readonly IEdmCollectionTypeReference DefaultDerivedValidCollectionEntityTypeRef = new EdmCollectionTypeReference(DefaultDerivedValidCollectionEntityType);

        #region IsOperationBindingTypeEquivalentTo tests
        [Fact]
        public void OperationTypeIsSameAsBindingShouldReturnTrue()
        {
            EdmAction action = new EdmAction("n", "a", null, true, null);
            action.AddParameter("bindingParameter", DefaultValidEntityTypeRef);
            action.HasEquivalentBindingType(DefaultValidEntityType).Should().BeTrue();
        }

        [Fact]
        public void OperationTypeIsSameCollectionTypeAsBindingShouldReturnTrue()
        {
            EdmAction action = new EdmAction("n", "a", null, true, null);
            action.AddParameter("bindingParameter", DefaultValidCollectionEntityTypeRef);
            action.HasEquivalentBindingType(DefaultValidCollectionEntityTypeRef.Definition).Should().BeTrue();
        }

        [Fact]
        public void OperationTypeIsSameAsWithBaseBindingShouldReturnFalse()
        {
            EdmAction action = new EdmAction("n", "a", null, true, null);
            action.AddParameter("bindingParameter", DefaultDerivedValidEntityTypeRef);
            action.HasEquivalentBindingType(DefaultValidEntityType).Should().BeFalse();
        }

        [Fact]
        public void OperationTypeIsSameCollectionTypeAsWithBaseBindingShouldReturnFalse()
        {
            EdmAction action = new EdmAction("n", "a", null, true, null);
            action.AddParameter("bindingParameter", DefaultDerivedValidCollectionEntityTypeRef);
            action.HasEquivalentBindingType(DefaultValidCollectionEntityTypeRef.Definition).Should().BeFalse();
        }

        #endregion
        #region FilterByName Operation Tests
        [Fact]
        public void ForceFilterByFullNameShouldReturnNoOperations()
        {
            EdmAction action = new EdmAction("namespace", "action", null);
            new IEdmOperation[] { action }.FilterByName(true, "action").Should().BeEmpty();
        }

        [Fact]
        public void FilterByNameShouldThrowIfOperationNameNull()
        {
            EdmAction action = new EdmAction("namespace", "action", null);
            Action test = () => new IEdmOperation[] { action }.FilterByName(true, null);
            test.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void FilterByNameShouldThrowIfSequenceNull()
        {
            EdmAction action = new EdmAction("namespace", "action", null);
            Action test = () => Microsoft.OData.Edm.ExtensionMethods.FilterByName(null, true, "action");
            test.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void FilterByPartialNameShouldReturnBothActionsWithDifferenceNamespacesButSameName()
        {
            EdmAction action = new EdmAction("namespace", "action", null);
            EdmAction action2 = new EdmAction("namespace2", "action", null);
            new IEdmOperation[] { action, action2 }.FilterByName(false, "action").Should().HaveCount(2);
        }

        [Fact]
        public void FilterByFullNameShouldReturnOneActionsWithCorrectNamespace()
        {
            EdmAction action = new EdmAction("namespace", "action", null);
            EdmAction action2 = new EdmAction("namespace2", "action", null);
            var filteredResults = new IEdmOperation[] { action, action2 }.FilterByName(false, "namespace.action").ToList();
            filteredResults.Should().HaveCount(1);
            filteredResults[0].Should().Be(action);
        }

        #endregion

        #region ShortQualifiedName
        [Fact]
        public void ShortQualifiedNameForNonPrimitiveTypeShouldBeFullName()
        {
            string stringOfExpectedShortQualifiedName = String.Format("{0}.{1}", DefaultNamespace, DefaultName);

            var stringOfObservedShortQualifiedName = edmComplexType.ShortQualifiedName();
            stringOfObservedShortQualifiedName.Should().Be(stringOfExpectedShortQualifiedName);

            stringOfObservedShortQualifiedName = edmEntityType.ShortQualifiedName();
            stringOfObservedShortQualifiedName.Should().Be(stringOfExpectedShortQualifiedName);

            var edmEntityContainer = new EdmEntityContainer(DefaultNamespace, DefaultName);
            stringOfObservedShortQualifiedName = edmEntityContainer.ShortQualifiedName();
            stringOfObservedShortQualifiedName.Should().Be(stringOfExpectedShortQualifiedName);
        }

        [Fact]
        public void ShortQualifiedNameForCollectionNonPrimitiveTypeShouldBeNull()
        {
            var iEdmCollectionTypeReference = EdmCoreModel.GetCollection(new EdmComplexTypeReference(edmComplexType, true));
            var stringOfObservedShortQualifiedName = iEdmCollectionTypeReference.ShortQualifiedName();
            stringOfObservedShortQualifiedName.Should().BeNull();

            iEdmCollectionTypeReference = EdmCoreModel.GetCollection(new EdmEntityTypeReference(edmEntityType, true));
            stringOfObservedShortQualifiedName = iEdmCollectionTypeReference.ShortQualifiedName();
            stringOfObservedShortQualifiedName.Should().BeNull();
        }

        [Fact]
        public void ShortQualifiedNameForPrimitiveTypeShouldBeName()
        {
            foreach (EdmPrimitiveTypeKind edmPrimitiveTypeKind in Enum.GetValues(typeof(EdmPrimitiveTypeKind)))
            {
                if (EdmPrimitiveTypeKind.None == edmPrimitiveTypeKind)
                    continue;
                var stringOfExpectedShortQualifiedName = Enum.GetName(typeof(EdmPrimitiveTypeKind), edmPrimitiveTypeKind);
                stringOfExpectedShortQualifiedName.ToUpper().Should().NotContain("EDM.");
                var stringOfObservedShortQualifiedName = EdmCoreModel.Instance.GetPrimitiveType(edmPrimitiveTypeKind).ShortQualifiedName();
                stringOfObservedShortQualifiedName.Should().Be(stringOfExpectedShortQualifiedName);
            }
        }

        [Fact]
        public void ShortQualifiedNameForCollectionPrimitiveTypeShouldBeNull()
        {
            foreach (EdmPrimitiveTypeKind edmPrimitiveTypeKind in Enum.GetValues(typeof(EdmPrimitiveTypeKind)))
            {
                if (EdmPrimitiveTypeKind.None == edmPrimitiveTypeKind)
                    continue;
                var stringOfName = Enum.GetName(typeof(EdmPrimitiveTypeKind), edmPrimitiveTypeKind);
                stringOfName.ToUpper().Should().NotContain("EDM.");

                var iEdmPrimitiveType = EdmCoreModel.Instance.GetPrimitiveType(edmPrimitiveTypeKind);
                var edmCollectionType = EdmCoreModel.GetCollection(new EdmPrimitiveTypeReference(iEdmPrimitiveType, true));

                var stringOfObservedShortQualifiedName = edmCollectionType.ShortQualifiedName();
                stringOfObservedShortQualifiedName.Should().BeNull();
            }
        }
        #endregion

        [Fact]
        public void FullTypeNameAndFullNameIEdmTypeReferenceShouldBeEqual()
        {
            var enumType = new EdmEnumTypeReference(new EdmEnumType("n", "enumtype"), false);
            enumType.FullName().Should().Be(enumType.Definition.FullTypeName());
        }

        #region IEdmType FullName tests

        [Fact]
        public void CollectionUnNamedStructuralType()
        {
            var fakeStructuredCollectionType = EdmCoreModel.GetCollection(new FakeEdmStructuredTypeReference(new FakeStructuredType(false, false, null)));
            fakeStructuredCollectionType.Definition.FullTypeName().Should().BeNull();
        }

        [Fact]
        public void UnNamedStructuralType()
        {
            var fakeStructuredType = new FakeEdmStructuredTypeReference(new FakeStructuredType(false, false, null));
            fakeStructuredType.Definition.FullTypeName().Should().BeNull();
        }

        [Fact]
        public void EnumTypeReferenceFullNameTest()
        {
            var enumType = new EdmEnumType("n", "enumtype");
            enumType.FullTypeName().Should().Be("n.enumtype");
        }

        [Fact]
        public void CollectionPrimitiveTypeReferenceFullNameTest()
        {
            var collectionPrimitives = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt16(true));
            collectionPrimitives.Definition.FullTypeName().Should().Be("Collection(Edm.Int16)");
        }

        [Fact]
        public void CollectionEntityTypeTypeReferenceFullNameTest()
        {
            var entityTypeCollection = EdmCoreModel.GetCollection(new EdmEntityTypeReference(new EdmEntityType("n", "type"), false));
            entityTypeCollection.Definition.FullTypeName().Should().Be("Collection(n.type)");
        }

        [Fact]
        public void PrimitiveTypeReferenceFullNameTest()
        {
            var primitiveTypeReference = EdmCoreModel.Instance.GetInt16(true);
            primitiveTypeReference.Definition.FullTypeName().Should().Be("Edm.Int16");
        }

        [Fact]
        public void EntityTypeTypeReferenceFullNameTest()
        {
            var entityType = new EdmEntityType("n", "type");
            entityType.FullTypeName().Should().Be("n.type");
        }

        #endregion

        [Fact]
        public void EdmFunctionShouldBeFunction()
        {
            var function = new EdmFunction("d.s", "checkout", EdmCoreModel.Instance.GetString(true));
            function.IsAction().Should().BeFalse();
            function.IsFunction().Should().BeTrue();
        }

        [Fact]
        public void EdmActionImportShouldBeActionImport()
        {
            var action = new EdmAction("d.s", "checkout", null);
            var actionImport = new EdmActionImport(new EdmEntityContainer("d", "c"), "checkout", action);
            actionImport.IsActionImport().Should().BeTrue();
            actionImport.IsFunctionImport().Should().BeFalse();

        }

        [Fact]
        public void EdmFunctionImportShouldBeFunctionImport()
        {
            var function = new EdmFunction("d.s", "checkout", EdmCoreModel.Instance.GetString(true));
            var functionImport = new EdmFunctionImport(new EdmEntityContainer("d", "c"), "checkout", function);
            functionImport.IsActionImport().Should().BeFalse();
            functionImport.IsFunctionImport().Should().BeTrue();
        }

        [Fact]
        public void EdmNavigationPropertyPrincipalPropertiesShouldReturnPrincipalProperties()
        {
            EdmEntityType type = new EdmEntityType("ns", "type");
            var key = type.AddStructuralProperty("Id1", EdmCoreModel.Instance.GetInt32(false));
            var notKey = type.AddStructuralProperty("Id2", EdmCoreModel.Instance.GetString(false));
            var p1 = type.AddStructuralProperty("p1", EdmCoreModel.Instance.GetInt32(false));
            var p2 = type.AddStructuralProperty("p1", EdmCoreModel.Instance.GetString(false));
            type.AddKeys(key);

            var navInfo1 = new EdmNavigationPropertyInfo()
            {
                Name = "nav",
                Target = type,
                TargetMultiplicity = EdmMultiplicity.Many,
                DependentProperties = new[] { p1, p2 },
                PrincipalProperties = new[] { key, notKey }
            };

            EdmNavigationProperty navProp = type.AddUnidirectionalNavigation(navInfo1);
            navProp.PrincipalProperties().Should().NotBeNull();
            navProp.PrincipalProperties().ShouldAllBeEquivalentTo(new[] { key, notKey });
        }

        [Fact]
        public void TypeDefinitionUnitTest()
        {
            EdmTypeDefinition typeDef = new EdmTypeDefinition("NS", "Length", EdmPrimitiveTypeKind.Int32);
            EdmTypeDefinitionReference typeRef = new EdmTypeDefinitionReference(typeDef, true);
            typeRef.TypeDefinition().Should().Be(typeDef);
        }

        #region TryGetRelativeEntitySet OperationaImport Tests

        [Fact]
        public void ValidateCorrectNoEntitySetPathOnOperationImport()
        {
            EdmFunction function = new EdmFunction("ns", "GetStuff", EdmCoreModel.Instance.GetString(false), true /*isBound*/, null, false);
            function.AddParameter("param", DefaultValidCollectionEntityTypeRef);
            EdmFunctionImport functionImport = new EdmFunctionImport(new EdmEntityContainer("ds", "d"), "GetStuff", function, null, false);
            EdmModel model = new EdmModel();
            model.AddElement(function);

            IEnumerable<EdmError> errorsFound = null;
            IEdmOperationParameter operationParameter = null;
            IEnumerable<IEdmNavigationProperty> navigationProperties = null;

            functionImport.TryGetRelativeEntitySetPath(model, out operationParameter, out navigationProperties, out errorsFound).Should().BeFalse();
            errorsFound.ToList().Should().HaveCount(0);
            operationParameter.Should().BeNull();
            navigationProperties.Should().BeNull();
        }

        [Fact]
        public void ValidateCorrectEntitySetPathOnOperationImport()
        {
            EdmFunction function = new EdmFunction("ns", "GetStuff", EdmCoreModel.Instance.GetString(false), true /*isBound*/, null, false);
            function.AddParameter("param", DefaultValidCollectionEntityTypeRef);
            EdmFunctionImport functionImport = new EdmFunctionImport(new EdmEntityContainer("ds", "d"), "GetStuff", function, new EdmPathExpression("param"), false);
            EdmModel model = new EdmModel();
            model.AddElement(function);

            IEnumerable<EdmError> errorsFound = null;
            IEdmOperationParameter operationParameter = null;
            IEnumerable<IEdmNavigationProperty> navigationProperties = null;

            functionImport.TryGetRelativeEntitySetPath(model, out operationParameter, out navigationProperties, out errorsFound).Should().BeTrue();
            errorsFound.ToList().Should().HaveCount(0);
            operationParameter.Should().NotBeNull();
            navigationProperties.Should().HaveCount(0);
        }


        #endregion

        #region TryGetRelativeEntitySet Operation Tests

        [Fact]
        public void TryGetRelativeEntitySetWithNullEntitySetPathShouldReturnFalseAndHaveNoErrors()
        {
            EdmFunction function = new EdmFunction("ns", "GetStuff", EdmCoreModel.Instance.GetString(false), true /*isBound*/, null, false);
            function.AddParameter("param", DefaultValidCollectionEntityTypeRef);
            EdmModel model = new EdmModel();
            model.AddElement(function);

            IEnumerable<EdmError> errorsFound = null;
            IEdmOperationParameter operationParameter = null;
            IEnumerable<IEdmNavigationProperty> navigationProperties = null;
            IEdmEntityType entityType = null;

            function.TryGetRelativeEntitySetPath(model, out operationParameter, out navigationProperties, out entityType, out errorsFound).Should().BeFalse();
            errorsFound.Should().HaveCount(0);
            operationParameter.Should().BeNull();
            navigationProperties.Should().BeNull();
        }

        [Fact]
        public void TryGetRelativeEntitySetWithJustBindingParameterShouldReturnTrueAndHaveNoErrors()
        {
            EdmFunction function = new EdmFunction("ns", "GetStuff", DefaultValidEntityTypeRef, true /*isBound*/, new EdmPathExpression("param"), false);
            function.AddParameter("param", DefaultValidEntityTypeRef);
            EdmModel model = new EdmModel();
            model.AddElement(function);

            IEnumerable<EdmError> errorsFound = null;
            IEdmOperationParameter operationParameter = null;
            IEnumerable<IEdmNavigationProperty> navigationProperties = null;
            IEdmEntityType entityType = null;

            function.TryGetRelativeEntitySetPath(model, out operationParameter, out navigationProperties, out entityType, out errorsFound).Should().BeTrue();
            errorsFound.Should().HaveCount(0);
            operationParameter.Should().NotBeNull();
            navigationProperties.Should().HaveCount(0);
            entityType.Should().Be(DefaultValidEntityTypeRef.Definition);
        }

        [Fact]
        public void TryGetEntitySetWithBoundCsdlSemanticOperationParameterShouldReturnTrueAndHaveNoErrors()
        {
            var csdlEntityType = new CsdlEntityType("EntityType", null, false, false, false, null, Enumerable.Empty<CsdlProperty>(), Enumerable.Empty<CsdlNavigationProperty>(), null, null);
            var csdlSchema = CsdlBuilder.Schema("FQ.NS", csdlStructuredTypes: new[] { csdlEntityType });

            var csdlModel = new CsdlModel();
            csdlModel.AddSchema(csdlSchema);
            var semanticModel = new CsdlSemanticsModel(csdlModel, new EdmDirectValueAnnotationsManager(), Enumerable.Empty<IEdmModel>());
            var semanticSchema = new CsdlSemanticsSchema(semanticModel, csdlSchema);
            var testLocation = new CsdlLocation(1, 3);

            var action = new CsdlAction(
                "Checkout",
                new CsdlOperationParameter[] { new CsdlOperationParameter("entity", new CsdlNamedTypeReference("FQ.NS.EntityType", false, testLocation), null, testLocation) },
                new CsdlNamedTypeReference("Edm.String", false, testLocation),
                true /*isBound*/,
                "entity",
                null /*documentation*/,
                testLocation);

            var semanticAction = new CsdlSemanticsAction(semanticSchema, action);
            IEdmOperationParameter edmParameter;
            IEnumerable<IEdmNavigationProperty> navigationProperties;
            IEdmEntityType entityType;
            IEnumerable<EdmError> errors;
            semanticAction.TryGetRelativeEntitySetPath(semanticSchema.Model, out edmParameter, out navigationProperties, out entityType, out errors).Should().BeTrue();
            edmParameter.Name.Should().Be("entity");
            navigationProperties.Should().BeEmpty();
            entityType.FullName().Should().Be("FQ.NS.EntityType");
            errors.Should().BeEmpty();
        }

        [Fact]
        public void AnyNonBoundOperationWithAnEntitySetPathSpecifiedShouldFailWithError()
        {
            EdmFunction function = new EdmFunction("ns", "GetStuff", EdmCoreModel.Instance.GetString(false), false /*isBound*/, new EdmPathExpression("path1", "path2"), false);
            function.AddParameter("param", DefaultValidCollectionEntityTypeRef);
            EdmModel model = new EdmModel();
            model.AddElement(function);
            ValidateErrorInList(
                model,
                function,
                EdmErrorCode.OperationCannotHaveEntitySetPathWithUnBoundOperation,
                Strings.EdmModel_Validator_Semantic_OperationCannotHaveEntitySetPathWithUnBoundOperation(function.Name));
        }

        [Fact]
        public void OperationWithEntitySetPathWithBindingParameterNotFoundShouldFailWithError()
        {
            EdmFunction function = new EdmFunction("ns", "GetStuff", DefaultValidEntityTypeRef, true /*isBound*/, new EdmPathExpression("incorrectBindingParameterName", "path2"), false);
            EdmEntityType entityType = new EdmEntityType("ns", "entityType");
            function.AddParameter("bindingParameter", new EdmEntityTypeReference(entityType, false));
            EdmModel model = new EdmModel();
            model.AddElement(function);
            ValidateErrorInList(
                model,
                function,
                EdmErrorCode.InvalidPathFirstPathParameterNotMatchingFirstParameterName,
                Strings.EdmModel_Validator_Semantic_InvalidEntitySetPathWithFirstPathParameterNotMatchingFirstParameterName("EntitySetPath", "incorrectBindingParameterName/path2", "incorrectBindingParameterName", "bindingParameter"));
        }

        [Fact]
        public void EdmPathExpressionWithZeroSegmentsShouldFailWithError()
        {
            EdmFunction function = new EdmFunction("ns", "GetStuff", DefaultValidEntityTypeRef, true /*isBound*/, new EdmPathExpression(), false);
            function.AddParameter("param", EdmCoreModel.Instance.GetString(false));
            EdmModel model = new EdmModel();
            model.AddElement(function);
            ValidateError(
                model,
                function,
                EdmErrorCode.OperationWithInvalidEntitySetPathMissingCompletePath,
                Strings.EdmModel_Validator_Semantic_InvalidEntitySetPathMissingBindingParameterName("EntitySetPath"));
        }

        [Fact]
        public void EdmPathExpressionWithBindingParameterNotAnEntityShouldError()
        {
            EdmComplexType complexType = new EdmComplexType("ds.s", "complexType");
            EdmModel model = new EdmModel();
            model.AddElement(complexType);

            EdmFunction function = new EdmFunction("ns", "GetStuff", DefaultValidEntityTypeRef, true /*isBound*/, new EdmPathExpression("complexParam/People"), false);
            function.AddParameter("complexParam", new EdmComplexTypeReference(complexType, false));
            model.AddElement(function);

            ValidateError(
                model,
                function,
                EdmErrorCode.InvalidPathWithNonEntityBindingParameter,
                Strings.EdmModel_Validator_Semantic_InvalidEntitySetPathWithNonEntityBindingParameter("EntitySetPath", "complexParam/People", "complexParam"));
        }

        [Fact]
        public void EdmPathExpressionWithCollectionBindingParameterNotAnEntityShouldError()
        {
            EdmComplexType complexType = new EdmComplexType("ds.s", "complexType");
            EdmModel model = new EdmModel();
            model.AddElement(complexType);

            EdmFunction function = new EdmFunction("ns", "GetStuff", DefaultValidEntityTypeRef, true /*isBound*/, new EdmPathExpression("collectionComplexParam/People"), false);
            function.AddParameter("collectionComplexParam", new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(complexType, false))));
            model.AddElement(function);
            ValidateError(
                model,
                function,
                EdmErrorCode.InvalidPathWithNonEntityBindingParameter,
                Strings.EdmModel_Validator_Semantic_InvalidEntitySetPathWithNonEntityBindingParameter("EntitySetPath", "collectionComplexParam/People", "collectionComplexParam"));
        }

        [Fact]
        public void EdmPathExpressionWithUnknownNavigationPropertyShouldError()
        {
            EdmEntityType entityType = new EdmEntityType("ds.s", "entityType");
            EdmModel model = new EdmModel();
            model.AddElement(entityType);

            EdmFunction function = new EdmFunction("ns", "GetStuff", DefaultValidEntityTypeRef, true /*isBound*/, new EdmPathExpression("bindingEntity/People"), false);
            function.AddParameter("bindingEntity", new EdmEntityTypeReference(entityType, false));
            model.AddElement(function);
            ValidateErrorInList(
                model,
                function,
                EdmErrorCode.InvalidPathUnknownNavigationProperty,
                Strings.EdmModel_Validator_Semantic_InvalidEntitySetPathUnknownNavigationProperty("EntitySetPath", "bindingEntity/People", "People"));
        }

        [Fact]
        public void EdmPathExpressionWithUnknownTypeCastSegmentShouldError()
        {
            EdmEntityType entityType = new EdmEntityType("ds.s", "entityType");
            EdmModel model = new EdmModel();
            model.AddElement(entityType);

            EdmFunction function = new EdmFunction("ns", "GetStuff", DefaultValidEntityTypeRef, true /*isBound*/, new EdmPathExpression("bindingEntity/unknown.EntityType"), false);
            function.AddParameter("bindingEntity", new EdmEntityTypeReference(entityType, false));
            model.AddElement(function);
            ValidateErrorInList(
                model,
                function,
                EdmErrorCode.InvalidPathUnknownTypeCastSegment,
                Strings.EdmModel_Validator_Semantic_InvalidEntitySetPathUnknownTypeCastSegment("EntitySetPath", "bindingEntity/unknown.EntityType", "unknown.EntityType"));
        }

        [Fact]
        public void EdmPathExpressionWithInvalidComplexTypeCaseForTypeCastSegmentShouldError()
        {
            EdmEntityType entityType = new EdmEntityType("ds.s", "entityType");
            EdmComplexType complexType = new EdmComplexType("ds.s", "complexType");
            EdmModel model = new EdmModel();
            model.AddElement(entityType);
            model.AddElement(complexType);

            EdmFunction function = new EdmFunction("ns", "GetStuff", new EdmEntityTypeReference(entityType, true), true /*isBound*/, new EdmPathExpression("bindingEntity/ds.s.complexType"), false);
            function.AddParameter("bindingEntity", new EdmEntityTypeReference(entityType, false));
            model.AddElement(function);

            ValidateErrorInList(
                model,
                function,
                EdmErrorCode.InvalidPathTypeCastSegmentMustBeEntityType,
                Strings.EdmModel_Validator_Semantic_InvalidEntitySetPathTypeCastSegmentMustBeEntityType("EntitySetPath", "bindingEntity/ds.s.complexType", "ds.s.complexType"));
        }

        [Fact]
        public void EdmPathExpressionWithInvalidNonInheritedTypeCastSegmentShouldError()
        {
            EdmEntityType entityType = new EdmEntityType("ds.s", "entityType");
            EdmEntityType otherEntityType = new EdmEntityType("ds.s", "otherEntityType");
            EdmModel model = new EdmModel();
            model.AddElement(entityType);
            model.AddElement(otherEntityType);

            EdmFunction function = new EdmFunction("ns", "GetStuff", new EdmEntityTypeReference(entityType, true), true /*isBound*/, new EdmPathExpression("bindingEntity/ds.s.otherEntityType"), false);
            function.AddParameter("bindingEntity", new EdmEntityTypeReference(entityType, false));
            model.AddElement(function);

            ValidateErrorInList(
                model,
                function,
                EdmErrorCode.InvalidPathInvalidTypeCastSegment,
                Strings.EdmModel_Validator_Semantic_InvalidEntitySetPathInvalidTypeCastSegment("EntitySetPath", "bindingEntity/ds.s.otherEntityType", entityType.FullName(), otherEntityType.FullName()));
        }

        [Fact]
        public void ValidateTypeCaseWithDerivedPropertyEntitySetPathReturnsNoErrors()
        {
            ValidationRulesTests.OperationOperationEntitySetPathMustBeValidValidTestModel testModelContainer = new ValidationRulesTests.OperationOperationEntitySetPathMustBeValidValidTestModel();

            EdmFunction function = new EdmFunction("ns", "GetStuff", new EdmEntityTypeReference(testModelContainer.T3, false), true /*isBound*/, new EdmPathExpression("bindingEntity/Bunk.T1/P101"), false);
            function.AddParameter("bindingEntity", new EdmEntityTypeReference(testModelContainer.T2, false));

            ValidateNoError(testModelContainer.Model, function);
        }

        [Fact]
        public void ValidateEntitySetPathNavPropertyTypeCastNavPropertyReturnsNoErrors()
        {
            ValidationRulesTests.OperationOperationEntitySetPathMustBeValidValidTestModel testModelContainer = new ValidationRulesTests.OperationOperationEntitySetPathMustBeValidValidTestModel();

            EdmFunction function = new EdmFunction("ns", "GetStuff", new EdmEntityTypeReference(testModelContainer.T3, false), true /*isBound*/, new EdmPathExpression("bindingEntity/Nav1/Bunk.T1/P101"), false);
            function.AddParameter("bindingEntity", new EdmEntityTypeReference(testModelContainer.T3, false));

            ValidateNoError(testModelContainer.Model, function);
        }

        [Fact]
        public void ValidateEntitySetPathColNavPropertyTypeCastNavPropertyReturnsNoErrors()
        {
            ValidationRulesTests.OperationOperationEntitySetPathMustBeValidValidTestModel testModelContainer = new ValidationRulesTests.OperationOperationEntitySetPathMustBeValidValidTestModel();

            EdmFunction function = new EdmFunction("ns", "GetStuff", new EdmEntityTypeReference(testModelContainer.T3, false), true /*isBound*/, new EdmPathExpression("bindingEntity/ColNav/Bunk.T1/P101"), false);
            function.AddParameter("bindingEntity", new EdmEntityTypeReference(testModelContainer.T3, false));

            ValidateNoError(testModelContainer.Model, function);
        }

        [Fact]
        public void ValidateEntitySetPathTypeCastColNavPropertyTypeCastNavPropertyReturnsNoErrors()
        {
            ValidationRulesTests.OperationOperationEntitySetPathMustBeValidValidTestModel testModelContainer = new ValidationRulesTests.OperationOperationEntitySetPathMustBeValidValidTestModel();

            var returnType = new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(testModelContainer.T3, false)));
            EdmFunction function = new EdmFunction("ns", "GetStuff", returnType, true /*isBound*/, new EdmPathExpression("bindingEntity/Bunk.T1/ColP101"), false);
            function.AddParameter("bindingEntity", new EdmEntityTypeReference(testModelContainer.T2, false));

            ValidateNoError(testModelContainer.Model, function);
        }

        [Fact]
        public void ValidateEntitySetPathColNavPropertyNavPropertyReturnsNoErrors()
        {
            ValidationRulesTests.OperationOperationEntitySetPathMustBeValidValidTestModel testModelContainer = new ValidationRulesTests.OperationOperationEntitySetPathMustBeValidValidTestModel();

            EdmFunction function = new EdmFunction("ns", "GetStuff", new EdmEntityTypeReference(testModelContainer.T3, false), true /*isBound*/, new EdmPathExpression("bindingEntity/ColNav/RefNav"), false);
            function.AddParameter("bindingEntity", new EdmEntityTypeReference(testModelContainer.T3, false));

            ValidateNoError(testModelContainer.Model, function);
        }

        [Fact]
        public void ValidateEntitySetPathBindingParameterWithEntityCollectionAndValidEntitySetPathSucceeds()
        {
            ValidationRulesTests.OperationOperationEntitySetPathMustBeValidValidTestModel testModelContainer = new ValidationRulesTests.OperationOperationEntitySetPathMustBeValidValidTestModel();

            EdmFunction function = new EdmFunction("ns", "GetStuff", new EdmEntityTypeReference(testModelContainer.T3, false), true /*isBound*/, new EdmPathExpression("bindingEntity/ColNav/RefNav"), false);
            function.AddParameter("bindingEntity", new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(testModelContainer.T3, false))));

            ValidateNoError(testModelContainer.Model, function);
        }

        [Fact]
        public void ValidateEntitySetPathNavPropertyTypeCastUnknownNavPropertyReturnsUnknownNavigationPropertyError()
        {
            ValidationRulesTests.OperationOperationEntitySetPathMustBeValidValidTestModel testModelContainer = new ValidationRulesTests.OperationOperationEntitySetPathMustBeValidValidTestModel();

            EdmFunction function = new EdmFunction("ns", "GetStuff", new EdmEntityTypeReference(testModelContainer.T3, false), true /*isBound*/, new EdmPathExpression("bindingEntity/Bunk.T1/UnknownNav"), false);
            function.AddParameter("bindingEntity", new EdmEntityTypeReference(testModelContainer.T2, false));

            ValidateErrorInList(
                testModelContainer.Model,
                function,
                EdmErrorCode.InvalidPathUnknownNavigationProperty,
                Strings.EdmModel_Validator_Semantic_InvalidEntitySetPathUnknownNavigationProperty("EntitySetPath", "bindingEntity/Bunk.T1/UnknownNav", "UnknownNav"));
        }

        [Fact]
        public void ValidateEntitySetPathNavPropertyUnknownTypeCastShouldError()
        {
            ValidationRulesTests.OperationOperationEntitySetPathMustBeValidValidTestModel testModelContainer = new ValidationRulesTests.OperationOperationEntitySetPathMustBeValidValidTestModel();

            EdmFunction function = new EdmFunction("ns", "GetStuff", new EdmEntityTypeReference(testModelContainer.T3, false), true /*isBound*/, new EdmPathExpression("bindingEntity/Nav1/Bunk.T1Foo"), false);
            function.AddParameter("bindingEntity", new EdmEntityTypeReference(testModelContainer.T3, false));

            ValidateErrorInList(
                testModelContainer.Model,
                function,
                EdmErrorCode.InvalidPathUnknownTypeCastSegment,
                Strings.EdmModel_Validator_Semantic_InvalidEntitySetPathUnknownTypeCastSegment("EntitySetPath", "bindingEntity/Nav1/Bunk.T1Foo", "Bunk.T1Foo"));
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
                this.T3.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "Nav1", Target = this.T2, TargetMultiplicity = EdmMultiplicity.One });
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

        private static void ValidateNoError(IEdmModel model, IEdmOperation operation)
        {
            IEnumerable<EdmError> errorsFound = null;
            IEdmOperationParameter operationParameter = null;
            IEnumerable<IEdmNavigationProperty> navigationProperties = null;
            IEdmEntityType entityType = null;

            operation.TryGetRelativeEntitySetPath(model, out operationParameter, out navigationProperties, out entityType, out errorsFound);
            errorsFound.Should().HaveCount(0);
        }

        private static void ValidateError(IEdmModel model, IEdmOperation operation, EdmErrorCode expectedErrorCode, string expectedError)
        {
            IEnumerable<EdmError> errorsFound = null;
            IEdmOperationParameter operationParameter = null;
            IEnumerable<IEdmNavigationProperty> navigationProperties = null;
            IEdmEntityType entityType = null;

            operation.TryGetRelativeEntitySetPath(model, out operationParameter, out navigationProperties, out entityType, out errorsFound);
            errorsFound.Should().HaveCount(1);
            var errorsFoundList = errorsFound.ToList();
            errorsFoundList[0].ErrorCode.Should().Be(expectedErrorCode);
            errorsFoundList[0].ErrorMessage.Should().Be(expectedError);
        }

        private static void ValidateErrorInList(IEdmModel model, IEdmOperation operation, EdmErrorCode expectedErrorCode, string expectedError)
        {
            IEnumerable<EdmError> errorsFound = null;
            IEdmOperationParameter operationParameter = null;
            IEnumerable<IEdmNavigationProperty> navigationProperties = null;
            IEdmEntityType entityType = null;

            operation.TryGetRelativeEntitySetPath(model, out operationParameter, out navigationProperties, out entityType, out errorsFound);
            var error = errorsFound.SingleOrDefault(e => e.ErrorCode == expectedErrorCode);
            error.Should().NotBeNull();
            error.ErrorMessage.Should().Be(expectedError);
        }

        #endregion

        #region 
        [Fact]
        public void CheckExistingContainer()
        {
            TestModel.Instance.Model.ExistsContainer(TestModel.Instance.Container.Name).Should().BeTrue();
        }

        [Fact]
        public void CheckExistingContainerWithQualifiedName()
        {
            TestModel.Instance.Model.ExistsContainer(TestModel.Instance.Container.ShortQualifiedName()).Should().BeTrue();
        }

        [Fact]
        public void CheckNonExistingContainer()
        {
            var containerName = "NonExistingContainer";
            TestModel.Instance.Model.ExistsContainer(containerName).Should().BeFalse();
        }

        [Fact]
        public void FindDeclaredEntitySet()
        {
            var entitySet = TestModel.Instance.Model.FindDeclaredEntitySet(TestModel.Instance.EntitySet.Name);
            entitySet.Should().NotBeNull();
            entitySet.EntityType().Should().Equals(TestModel.Instance.EntitySet.EntityType());
        }

        [Fact]
        public void FindDeclaredEntitySetWithContainerName()
        {
            var entitySet = TestModel.Instance.Model.FindDeclaredEntitySet(TestModel.Instance.Container.Name + "." + TestModel.Instance.EntitySet.Name);
            entitySet.Should().NotBeNull();
            entitySet.EntityType().Should().Equals(TestModel.Instance.EntitySet.EntityType());
        }

        [Fact]
        public void FindDeclaredEntitySetWithContainerQualifiedName()
        {
            var entitySet = TestModel.Instance.Model.FindDeclaredEntitySet(TestModel.Instance.Container.ShortQualifiedName() + "." + TestModel.Instance.EntitySet.Name);
            entitySet.Should().NotBeNull();
            entitySet.EntityType().Should().Equals(TestModel.Instance.EntitySet.EntityType());
        }

        [Fact]
        public void FindDeclaredEntitySetWithNonExistingEntitySetName()
        {
            var entitySetName = "NonExistingEntitySet";
            TestModel.Instance.Model.FindDeclaredEntitySet(entitySetName).Should().BeNull();
        }

        [Fact]
        public void FindDeclaredEntitySetWithSingletonName()
        {
            TestModel.Instance.Model.FindDeclaredEntitySet(TestModel.Instance.Singleton.Name).Should().BeNull();
        }

        [Fact]
        public void FindDeclaredSingleton()
        {
            var singleton = TestModel.Instance.Model.FindDeclaredSingleton(TestModel.Instance.Singleton.Name);
            singleton.Should().NotBeNull();
            singleton.EntityType().Should().Equals(TestModel.Instance.Singleton.EntityType());
        }

        [Fact]
        public void FindDeclaredSingletonWithContainerName()
        {
            var singleton = TestModel.Instance.Model.FindDeclaredSingleton(TestModel.Instance.Container.Name + "." + TestModel.Instance.Singleton.Name);
            singleton.Should().NotBeNull();
            singleton.EntityType().Should().Equals(TestModel.Instance.Singleton.EntityType());
        }

        [Fact]
        public void FindDeclaredSingletonWithContainerQualifiedName()
        {
            var singleton = TestModel.Instance.Model.FindDeclaredSingleton(TestModel.Instance.Container.ShortQualifiedName() + "." + TestModel.Instance.Singleton.Name);
            singleton.Should().NotBeNull();
            singleton.EntityType().Should().Equals(TestModel.Instance.Singleton.EntityType());
        }

        [Fact]
        public void FindDeclaredSingletonWithNonExistingSingletonName()
        {
            var singletonName = "NonExistingSingleton";
            TestModel.Instance.Model.FindDeclaredSingleton(singletonName).Should().BeNull();
        }

        [Fact]
        public void FindDeclaredSingletonWithEntitySetName()
        {
            TestModel.Instance.Model.FindDeclaredSingleton(TestModel.Instance.EntitySet.Name).Should().BeNull();
        }

        [Fact]
        public void FindDeclaredOperationImports()
        {
            var operationImport = TestModel.Instance.Model.FindDeclaredOperationImports(TestModel.Instance.functionImport.Name);
            operationImport.Should().HaveCount(1);
            operationImport.ElementAt(0).IsFunctionImport().Should().BeTrue();
        }

        [Fact]
        public void FindDeclaredOperationImportsWithContainerName()
        {
            var operationImportName = TestModel.Instance.Container.Name + "." + TestModel.Instance.functionImport.Name;
            var operationImport = TestModel.Instance.Model.FindDeclaredOperationImports(operationImportName);
            operationImport.Should().NotBeNull();
            operationImport.ElementAt(0).IsFunctionImport().Should().BeTrue();
        }

        [Fact]
        public void FindDeclaredOperationImportsWithContainerQualifiedName()
        {
            var operationImportName = TestModel.Instance.Container.ShortQualifiedName() + "." + TestModel.Instance.functionImport.Name;
            var operationImport = TestModel.Instance.Model.FindDeclaredOperationImports(operationImportName);
            operationImport.Should().NotBeNull();
            operationImport.ElementAt(0).IsFunctionImport().Should().BeTrue();
        }

        [Fact]
        public void FindDeclaredOperationImportsWithNonExistingOperationImportsName()
        {
            var operationImportName = "NonExistingOperationImports";
            TestModel.Instance.Model.FindDeclaredOperationImports(operationImportName).Should().HaveCount(0);
        }

        internal class TestModel
        {
            public static TestModel Instance = new TestModel();
            public const string TestModelNameSpace = "TestModelNameSpace";
            private TestModel()
            {
                this.Model = new EdmModel();

                this.T1 = new EdmEntityType(TestModelNameSpace, "T1");
                EdmStructuralProperty p11 = this.T1.AddStructuralProperty("P11", EdmCoreModel.Instance.GetInt32(false));
                this.T1.AddKeys(p11);
                this.Model.AddElement(this.T1);

                this.T2 = new EdmEntityType(TestModelNameSpace, "T2");
                EdmStructuralProperty p21 = this.T2.AddStructuralProperty("P21", EdmCoreModel.Instance.GetInt32(false));
                this.T2.AddKeys(p21);
                this.Model.AddElement(this.T2);

                this.functionImport = new EdmFunction(TestModelNameSpace, "Function1", new EdmEntityTypeReference(this.T1, true));
                this.functionImport.AddParameter("id", EdmCoreModel.Instance.GetInt32(false));
                this.Model.AddElement(this.functionImport);

                this.Container = new EdmEntityContainer(TestModelNameSpace, "Container");
                this.Model.AddElement(this.Container);

                this.Container.AddEntitySet("EntitySet1", this.T1);
                this.Container.AddSingleton("Singleton1", this.T2);
                this.Container.AddFunctionImport(this.functionImport);

                this.EntitySet = (EdmEntitySet)this.Container.FindEntitySet("EntitySet1");
                this.Singleton = (EdmSingleton)this.Container.FindSingleton("Singleton1");

            }

            public EdmModel Model { get; private set; }

            public EdmEntityContainer Container { get; private set; }

            public EdmEntitySet EntitySet { get; private set; }

            public EdmSingleton Singleton { get; private set; }

            public EdmEntityType T1 { get; private set; }

            public EdmEntityType T2 { get; private set; }

            public EdmFunction functionImport { get; private set; }
        }
        #endregion

        internal class FakeStructuredType : EdmStructuredType
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="EdmStructuredType"/> class.
            /// </summary>
            /// <param name="isAbstract">Denotes a structured type that cannot be instantiated.</param>
            /// <param name="isOpen">Denotes if the type is open.</param>
            /// <param name="baseStructuredType">Base type of the type</param>
            public FakeStructuredType(bool isAbstract, bool isOpen, IEdmStructuredType baseStructuredType)
                : base(isAbstract, isOpen, baseStructuredType)
            {
            }

            public override EdmTypeKind TypeKind
            {
                get { return EdmTypeKind.None; }
            }
        }

        internal class FakeEdmStructuredTypeReference : IEdmStructuredTypeReference
        {
            private readonly FakeStructuredType type;

            public FakeEdmStructuredTypeReference(FakeStructuredType type)
            {
                this.type = type;
            }

            public bool IsNullable
            {
                get { return false; }
            }

            public IEdmType Definition
            {
                get { return type; }
            }
        }
    }
}
