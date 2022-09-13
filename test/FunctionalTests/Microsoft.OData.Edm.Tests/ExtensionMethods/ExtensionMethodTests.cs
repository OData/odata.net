//---------------------------------------------------------------------
// <copyright file="ExtensionMethodTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
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
            Assert.True(action.HasEquivalentBindingType(DefaultValidEntityType));
        }

        [Fact]
        public void OperationTypeIsSameCollectionTypeAsBindingShouldReturnTrue()
        {
            EdmAction action = new EdmAction("n", "a", null, true, null);
            action.AddParameter("bindingParameter", DefaultValidCollectionEntityTypeRef);
            Assert.True(action.HasEquivalentBindingType(DefaultValidCollectionEntityTypeRef.Definition));
        }

        [Fact]
        public void OperationTypeIsSameAsWithBaseBindingShouldReturnFalse()
        {
            EdmAction action = new EdmAction("n", "a", null, true, null);
            action.AddParameter("bindingParameter", DefaultDerivedValidEntityTypeRef);
            Assert.False(action.HasEquivalentBindingType(DefaultValidEntityType));
        }

        [Fact]
        public void OperationTypeIsSameCollectionTypeAsWithBaseBindingShouldReturnFalse()
        {
            EdmAction action = new EdmAction("n", "a", null, true, null);
            action.AddParameter("bindingParameter", DefaultDerivedValidCollectionEntityTypeRef);
            Assert.False(action.HasEquivalentBindingType(DefaultValidCollectionEntityTypeRef.Definition));
        }

        #endregion
        #region FilterByName Operation Tests
        [Fact]
        public void ForceFilterByFullNameShouldReturnNoOperations()
        {
            EdmAction action = new EdmAction("namespace", "action", null);
            Assert.Empty(new IEdmOperation[] { action }.FilterByName(true, "action"));
        }

        [Fact]
        public void FilterByNameShouldThrowIfOperationNameNull()
        {
            EdmAction action = new EdmAction("namespace", "action", null);
            Action test = () => new IEdmOperation[] { action }.FilterByName(true, null);
            Assert.Throws<ArgumentNullException>("operationName", test);
        }

        [Fact]
        public void FilterByNameShouldThrowIfSequenceNull()
        {
            EdmAction action = new EdmAction("namespace", "action", null);
            Action test = () => Edm.ExtensionMethods.FilterByName(null, true, "action");
            Assert.Throws<ArgumentNullException>("operations", test);
        }

        [Fact]
        public void FilterByPartialNameShouldReturnBothActionsWithDifferenceNamespacesButSameName()
        {
            EdmAction action = new EdmAction("namespace", "action", null);
            EdmAction action2 = new EdmAction("namespace2", "action", null);
            Assert.Equal(2, new IEdmOperation[] { action, action2 }.FilterByName(false, "action").Count());
        }

        [Fact]
        public void FilterByFullNameShouldReturnOneActionsWithCorrectNamespace()
        {
            EdmAction action = new EdmAction("namespace", "action", null);
            EdmAction action2 = new EdmAction("namespace2", "action", null);
            var filteredResults = new IEdmOperation[] { action, action2 }.FilterByName(false, "namespace.action").ToList();
            var filter = Assert.Single(filteredResults);
            Assert.Same(action, filter);
        }

        #endregion

        #region ShortQualifiedName
        [Fact]
        public void ShortQualifiedNameForNonPrimitiveTypeShouldBeFullName()
        {
            string stringOfExpectedShortQualifiedName = String.Format("{0}.{1}", DefaultNamespace, DefaultName);

            var stringOfObservedShortQualifiedName = edmComplexType.ShortQualifiedName();
            Assert.Equal(stringOfExpectedShortQualifiedName, stringOfObservedShortQualifiedName);

            stringOfObservedShortQualifiedName = edmEntityType.ShortQualifiedName();
            Assert.Equal(stringOfExpectedShortQualifiedName, stringOfObservedShortQualifiedName);

            var edmEntityContainer = new EdmEntityContainer(DefaultNamespace, DefaultName);
            stringOfObservedShortQualifiedName = edmEntityContainer.ShortQualifiedName();
            Assert.Equal(stringOfExpectedShortQualifiedName, stringOfObservedShortQualifiedName);
        }

        [Fact]
        public void ShortQualifiedNameForCollectionNonPrimitiveTypeShouldBeNull()
        {
            var iEdmCollectionTypeReference = EdmCoreModel.GetCollection(new EdmComplexTypeReference(edmComplexType, true));
            var stringOfObservedShortQualifiedName = iEdmCollectionTypeReference.ShortQualifiedName();
            Assert.Null(stringOfObservedShortQualifiedName);

            iEdmCollectionTypeReference = EdmCoreModel.GetCollection(new EdmEntityTypeReference(edmEntityType, true));
            stringOfObservedShortQualifiedName = iEdmCollectionTypeReference.ShortQualifiedName();
            Assert.Null(stringOfObservedShortQualifiedName);
        }

        [Fact]
        public void ShortQualifiedNameForPrimitiveTypeShouldBeName()
        {
            foreach (EdmPrimitiveTypeKind edmPrimitiveTypeKind in Enum.GetValues(typeof(EdmPrimitiveTypeKind)))
            {
                if (EdmPrimitiveTypeKind.None == edmPrimitiveTypeKind)
                    continue;
                var stringOfExpectedShortQualifiedName = Enum.GetName(typeof(EdmPrimitiveTypeKind), edmPrimitiveTypeKind);
                Assert.DoesNotContain("EDM.", stringOfExpectedShortQualifiedName.ToUpper());
                var stringOfObservedShortQualifiedName = EdmCoreModel.Instance.GetPrimitiveType(edmPrimitiveTypeKind).ShortQualifiedName();
                Assert.Equal(stringOfExpectedShortQualifiedName, stringOfObservedShortQualifiedName);
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
                Assert.DoesNotContain("EDM.", stringOfName.ToUpper());

                var iEdmPrimitiveType = EdmCoreModel.Instance.GetPrimitiveType(edmPrimitiveTypeKind);
                var edmCollectionType = EdmCoreModel.GetCollection(new EdmPrimitiveTypeReference(iEdmPrimitiveType, true));

                var stringOfObservedShortQualifiedName = edmCollectionType.ShortQualifiedName();
                Assert.Null(stringOfObservedShortQualifiedName);
            }
        }
        #endregion

        [Fact]
        public void FullTypeNameAndFullNameIEdmTypeReferenceShouldBeEqual()
        {
            var enumType = new EdmEnumTypeReference(new EdmEnumType("n", "enumtype"), false);
            Assert.Equal(enumType.Definition.FullTypeName(), enumType.FullName());
        }

        #region IEdmType FullName tests

        [Fact]
        public void CollectionUnNamedStructuralType()
        {
            var fakeStructuredCollectionType = EdmCoreModel.GetCollection(new FakeEdmStructuredTypeReference(new FakeStructuredType(false, false, null)));
            Assert.Null(fakeStructuredCollectionType.Definition.FullTypeName());
        }

        [Fact]
        public void UnNamedStructuralType()
        {
            var fakeStructuredType = new FakeEdmStructuredTypeReference(new FakeStructuredType(false, false, null));
            Assert.Null(fakeStructuredType.Definition.FullTypeName());
        }

        [Fact]
        public void EnumTypeReferenceFullNameTest()
        {
            var enumType = new EdmEnumType("n", "enumtype");
            Assert.Equal("n.enumtype", enumType.FullTypeName());
        }

        [Fact]
        public void CollectionPrimitiveTypeReferenceFullNameTest()
        {
            var collectionPrimitives = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt16(true));
            Assert.Equal("Collection(Edm.Int16)", collectionPrimitives.Definition.FullTypeName());
        }

        [Fact]
        public void CollectionEntityTypeTypeReferenceFullNameTest()
        {
            var entityTypeCollection = EdmCoreModel.GetCollection(new EdmEntityTypeReference(new EdmEntityType("n", "type"), false));
            Assert.Equal("Collection(n.type)", entityTypeCollection.Definition.FullTypeName());
        }

        [Fact]
        public void PrimitiveTypeReferenceFullNameTest()
        {
            var primitiveTypeReference = EdmCoreModel.Instance.GetInt16(true);
            Assert.Equal("Edm.Int16", primitiveTypeReference.Definition.FullTypeName());
        }

        [Fact]
        public void GetReturnShouldReturnCorrectly()
        {
            IEdmTypeReference typeReference = EdmCoreModel.Instance.GetString(true);
            var function = new EdmFunction("d.s", "checkout", typeReference);
            IEdmOperationReturn operationReturn = function.GetReturn();
            Assert.NotNull(operationReturn);
            Assert.Same(operationReturn, function.Return);
            Assert.Same(operationReturn.Type, function.ReturnType);
            Assert.Same(operationReturn.DeclaringOperation, function);
        }

        [Fact]
        public void EntityTypeTypeReferenceFullNameTest()
        {
            var entityType = new EdmEntityType("n", "type");
            Assert.Equal("n.type", entityType.FullTypeName());
        }

        #endregion

        [Fact]
        public void EdmFunctionShouldBeFunction()
        {
            var function = new EdmFunction("d.s", "checkout", EdmCoreModel.Instance.GetString(true));
            Assert.False(function.IsAction());
            Assert.True(function.IsFunction());
        }

        [Fact]
        public void EdmActionImportShouldBeActionImport()
        {
            var action = new EdmAction("d.s", "checkout", null);
            var actionImport = new EdmActionImport(new EdmEntityContainer("d", "c"), "checkout", action);
            Assert.True(actionImport.IsActionImport());
            Assert.False(actionImport.IsFunctionImport());

        }

        [Fact]
        public void EdmFunctionImportShouldBeFunctionImport()
        {
            var function = new EdmFunction("d.s", "checkout", EdmCoreModel.Instance.GetString(true));
            var functionImport = new EdmFunctionImport(new EdmEntityContainer("d", "c"), "checkout", function);
            Assert.False(functionImport.IsActionImport());
            Assert.True(functionImport.IsFunctionImport());
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
            Assert.NotNull(navProp.PrincipalProperties());
            var properties = navProp.PrincipalProperties();
            Assert.Equal(2, properties.Count());
            Assert.Same(key, properties.First());
            Assert.Same(notKey, properties.Last());
        }

        [Fact]
        public void TypeDefinitionUnitTest()
        {
            EdmTypeDefinition typeDef = new EdmTypeDefinition("NS", "Length", EdmPrimitiveTypeKind.Int32);
            EdmTypeDefinitionReference typeRef = new EdmTypeDefinitionReference(typeDef, true);
            Assert.Same(typeDef, typeRef.TypeDefinition());
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
            Dictionary<IEdmNavigationProperty, IEdmPathExpression> navigationProperties = null;

            var result = functionImport.TryGetRelativeEntitySetPath(model, out operationParameter, out navigationProperties, out errorsFound);
            Assert.False(result);
            Assert.Empty(errorsFound);
            Assert.Null(operationParameter);
            Assert.Null(navigationProperties);
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
            Dictionary<IEdmNavigationProperty, IEdmPathExpression> navigationProperties = null;

            var result = functionImport.TryGetRelativeEntitySetPath(model, out operationParameter, out navigationProperties, out errorsFound);
            Assert.True(result);
            Assert.Empty(errorsFound);
            Assert.NotNull(operationParameter);
            Assert.Empty(navigationProperties);
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
            Dictionary<IEdmNavigationProperty, IEdmPathExpression> navigationProperties = null;
            IEdmEntityType entityType = null;

            var result = function.TryGetRelativeEntitySetPath(model, out operationParameter, out navigationProperties, out entityType, out errorsFound);
            Assert.False(result);
            Assert.Empty(errorsFound);
            Assert.Null(operationParameter);
            Assert.Null(navigationProperties);
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
            Dictionary<IEdmNavigationProperty, IEdmPathExpression> navigationProperties = null;
            IEdmEntityType entityType = null;

            var result = function.TryGetRelativeEntitySetPath(model, out operationParameter, out navigationProperties, out entityType, out errorsFound);
            Assert.True(result);
            Assert.Empty(errorsFound);
            Assert.NotNull(operationParameter);
            Assert.Empty(navigationProperties);
            Assert.Same(DefaultValidEntityTypeRef.Definition, entityType);
        }

        [Fact]
        public void TryGetEntitySetWithBoundCsdlSemanticOperationParameterShouldReturnTrueAndHaveNoErrors()
        {
            var csdlEntityType = new CsdlEntityType("EntityType", null, false, false, false, null, Enumerable.Empty<CsdlProperty>(), Enumerable.Empty<CsdlNavigationProperty>(), null);
            var csdlSchema = CsdlBuilder.Schema("FQ.NS", csdlStructuredTypes: new[] { csdlEntityType });

            var csdlModel = new CsdlModel();
            csdlModel.AddSchema(csdlSchema);
            var semanticModel = new CsdlSemanticsModel(csdlModel, new EdmDirectValueAnnotationsManager(), Enumerable.Empty<IEdmModel>());
            var semanticSchema = new CsdlSemanticsSchema(semanticModel, csdlSchema);
            var testLocation = new CsdlLocation(1, 3);

            var action = new CsdlAction(
                "Checkout",
                new CsdlOperationParameter[] { new CsdlOperationParameter("entity", new CsdlNamedTypeReference("FQ.NS.EntityType", false, testLocation), testLocation) },
                new CsdlOperationReturn(new CsdlNamedTypeReference("Edm.String", false, testLocation), testLocation),
                true /*isBound*/,
                "entity",
                testLocation);

            var semanticAction = new CsdlSemanticsAction(semanticSchema, action);
            IEdmOperationParameter edmParameter;
            Dictionary<IEdmNavigationProperty, IEdmPathExpression> navigationProperties;
            IEdmEntityType entityType;
            IEnumerable<EdmError> errors;
            var result = semanticAction.TryGetRelativeEntitySetPath(semanticSchema.Model, out edmParameter, out navigationProperties, out entityType, out errors);
            Assert.True(result);
            Assert.Equal("entity", edmParameter.Name);
            Assert.Empty(navigationProperties);
            Assert.Equal("FQ.NS.EntityType", entityType.FullName());
            Assert.Empty(errors);
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
            Dictionary<IEdmNavigationProperty, IEdmPathExpression> navigationProperties = null;
            IEdmEntityType entityType = null;

            operation.TryGetRelativeEntitySetPath(model, out operationParameter, out navigationProperties, out entityType, out errorsFound);
            Assert.Empty(errorsFound);
        }

        private static void ValidateError(IEdmModel model, IEdmOperation operation, EdmErrorCode expectedErrorCode, string expectedError)
        {
            IEnumerable<EdmError> errorsFound = null;
            IEdmOperationParameter operationParameter = null;
            Dictionary<IEdmNavigationProperty, IEdmPathExpression> navigationProperties = null;
            IEdmEntityType entityType = null;

            operation.TryGetRelativeEntitySetPath(model, out operationParameter, out navigationProperties, out entityType, out errorsFound);
            var error = Assert.Single(errorsFound);
            Assert.Equal(expectedErrorCode, error.ErrorCode);
            Assert.Equal(expectedError, error.ErrorMessage);
        }

        private static void ValidateErrorInList(IEdmModel model, IEdmOperation operation, EdmErrorCode expectedErrorCode, string expectedError)
        {
            IEnumerable<EdmError> errorsFound = null;
            IEdmOperationParameter operationParameter = null;
            Dictionary<IEdmNavigationProperty, IEdmPathExpression> navigationProperties = null;
            IEdmEntityType entityType = null;

            operation.TryGetRelativeEntitySetPath(model, out operationParameter, out navigationProperties, out entityType, out errorsFound);
            var error = errorsFound.SingleOrDefault(e => e.ErrorCode == expectedErrorCode);
            Assert.NotNull(error);
            Assert.Equal(expectedError, error.ErrorMessage);
        }

        #endregion

        [Fact]
        public void CheckExistingContainer()
        {
            Assert.True(TestModel.Instance.Model.ExistsContainer(TestModel.Instance.Container.Name));
        }

        [Fact]
        public void CheckExistingContainerWithQualifiedName()
        {
            Assert.True(TestModel.Instance.Model.ExistsContainer(TestModel.Instance.Container.ShortQualifiedName()));
        }

        [Fact]
        public void CheckNonExistingContainer()
        {
            var containerName = "NonExistingContainer";
            Assert.False(TestModel.Instance.Model.ExistsContainer(containerName));
        }

        [Fact]
        public void FindDeclaredEntitySet()
        {
            var entitySet = TestModel.Instance.Model.FindDeclaredEntitySet(TestModel.Instance.EntitySet.Name);
            Assert.NotNull(entitySet);
            Assert.Same(TestModel.Instance.EntitySet.EntityType(), entitySet.EntityType());
        }

        [Fact]
        public void FindDeclaredEntitySetWithContainerName()
        {
            var entitySet = TestModel.Instance.Model.FindDeclaredEntitySet(TestModel.Instance.Container.Name + "." + TestModel.Instance.EntitySet.Name);
            Assert.NotNull(entitySet);
            Assert.Same(TestModel.Instance.EntitySet.EntityType(), entitySet.EntityType());
        }

        [Fact]
        public void FindDeclaredEntitySetWithContainerQualifiedName()
        {
            var entitySet = TestModel.Instance.Model.FindDeclaredEntitySet(TestModel.Instance.Container.ShortQualifiedName() + "." + TestModel.Instance.EntitySet.Name);
            Assert.NotNull(entitySet);
            Assert.Same(TestModel.Instance.EntitySet.EntityType(), entitySet.EntityType());
        }

        [Fact]
        public void FindDeclaredEntitySetWithNonExistingEntitySetName()
        {
            var entitySetName = "NonExistingEntitySet";
            Assert.Null(TestModel.Instance.Model.FindDeclaredEntitySet(entitySetName));
        }

        [Fact]
        public void TestHasAnyParity()
        {
            IList<IEdmEntityType> list = new List<IEdmEntityType>();
            for(int i=1; i <10; i++)
            {
                Assert.Equal(HasAnyCheap(list), HasAnyExpensive(list));
                list.Add(new EdmEntityType("NS", "f" + i, TestModel.Instance.EntitySet.EntityType()));
            }
        }

        [Fact]
        public void FindDeclaredEntitySetWithSingletonName()
        {
            Assert.Null(TestModel.Instance.Model.FindDeclaredEntitySet(TestModel.Instance.Singleton.Name));
        }

        [Fact]
        public void FindDeclaredSingleton()
        {
            var singleton = TestModel.Instance.Model.FindDeclaredSingleton(TestModel.Instance.Singleton.Name);
            Assert.NotNull(singleton);
            Assert.Same(TestModel.Instance.Singleton.EntityType(), singleton.EntityType());
        }

        [Fact]
        public void FindDeclaredSingletonWithContainerName()
        {
            var singleton = TestModel.Instance.Model.FindDeclaredSingleton(TestModel.Instance.Container.Name + "." + TestModel.Instance.Singleton.Name);
            Assert.NotNull(singleton);
            Assert.Same(TestModel.Instance.Singleton.EntityType(), singleton.EntityType());
        }

        [Fact]
        public void FindDeclaredSingletonWithContainerQualifiedName()
        {
            var singleton = TestModel.Instance.Model.FindDeclaredSingleton(TestModel.Instance.Container.ShortQualifiedName() + "." + TestModel.Instance.Singleton.Name);
            Assert.NotNull(singleton);
            Assert.Same(TestModel.Instance.Singleton.EntityType(), singleton.EntityType());
        }

        [Fact]
        public void FindDeclaredSingletonWithNonExistingSingletonName()
        {
            var singletonName = "NonExistingSingleton";
            Assert.Null(TestModel.Instance.Model.FindDeclaredSingleton(singletonName));
        }

        [Fact]
        public void FindDeclaredSingletonWithEntitySetName()
        {
            Assert.Null(TestModel.Instance.Model.FindDeclaredSingleton(TestModel.Instance.EntitySet.Name));
        }

        [Fact]
        public void FindDeclaredOperationImports()
        {
            var operationImport = TestModel.Instance.Model.FindDeclaredOperationImports(TestModel.Instance.functionImport.Name);
            Assert.Single(operationImport);
            Assert.True(operationImport.ElementAt(0).IsFunctionImport());
        }

        [Fact]
        public void FindDeclaredOperationImportsWithContainerName()
        {
            var operationImportName = TestModel.Instance.Container.Name + "." + TestModel.Instance.functionImport.Name;
            var operationImport = TestModel.Instance.Model.FindDeclaredOperationImports(operationImportName);
            Assert.NotNull(operationImport);
            Assert.True(operationImport.ElementAt(0).IsFunctionImport());
        }

        [Fact]
        public void FindDeclaredOperationImportsWithContainerQualifiedName()
        {
            var operationImportName = TestModel.Instance.Container.ShortQualifiedName() + "." + TestModel.Instance.functionImport.Name;
            var operationImport = TestModel.Instance.Model.FindDeclaredOperationImports(operationImportName);
            Assert.NotNull(operationImport);
            Assert.True(operationImport.ElementAt(0).IsFunctionImport());
        }

        [Fact]
        public void FindDeclaredOperationImportsWithNonExistingOperationImportsName()
        {
            var operationImportName = "NonExistingOperationImports";
            var result = TestModel.Instance.Model.FindDeclaredOperationImports(operationImportName);
            Assert.Empty(result);
        }

        [Fact]
        public void FindDeclaredOperationImportsReturnsEmptyEnumerableForNoEntityContainerInModel()
        {
            var operationImportName = "NonExistingOperationImport";
            var result = new EdmModel().FindDeclaredOperationImports(operationImportName);
            Assert.Empty(result);
        }

        [Fact]
        public void FindTypeByAliasName()
        {
            Assert.Equal("TestModelNameSpace.T1", TestModel.Instance.Model.FindType("TestModelAlias.T1").FullName());
        }

        [Fact]
        public void FindTypeBySinglePartNamespaceQualifiedName()
        {
            Assert.Equal("TestModelNameSpace.T1", TestModel.Instance.Model.FindType("TestModelNameSpace.T1").FullName());
        }

        [Fact]
        public void FindTypeByMultiPartAliasQualfiedName()
        {
            Assert.Equal("Multi.Part.TestModelNameSpace.E1", TestModel.Instance.Model.FindType("MultipartTestModelAlias.E1").FullName());
        }

        [Fact]
        public void FindUndefinedAliasQualifiedNameReturnsNull()
        {
            Assert.Null(TestModel.Instance.Model.FindType("MultipartTestModelAlias.T1"));
        }

        [Fact]
        public void FindUndefinedSinglePartNamespaceQualifiedNameReturnsNull()
        {
            Assert.Null(TestModel.Instance.Model.FindType("TestModelNameSpace.E1"));
        }

        [Fact]
        public void FindUndefinedNamespaceQualifiedNameReturnsNull()
        {
            Assert.Null(TestModel.Instance.Model.FindType("Multi.Part.TestModelNameSpace.T1"));
        }

        [Fact]
        public void FindTypeForUndefinedTypeDoesnotGetIntoInfiniteSearchLoop()
        {
            // Arrange - create the EdmModel with all vacabulary models
            EdmModel model = new EdmModel();
            Assert.Equal(8, model.ReferencedModels.Count()); // core model + 7 vocabulary models

            // Act
            var unknownType = model.FindType("NS.UnKnownType");

            // Assert
            Assert.Null(unknownType);

            // Arrange - create the EdmModel without vacabulary models
            model = new EdmModel(false);
            Assert.Single(model.ReferencedModels); // We have the core model added by default

            // Act
            unknownType = model.FindType("NS.UnKnownType");

            // Assert
            Assert.Null(unknownType);
        }

        [Fact]
        public void FindProperty_WithCaseInsensitive_ThrowsArugmentNull()
        {
            // Arrange & Act & Assert
            IEdmStructuredType structuredType = null;
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => structuredType.FindProperty("name", caseInsensitive: true));
            Assert.Equal("structuredType", ex.ParamName);

            structuredType = new EdmComplexType("NS", "Complex");
            ex = Assert.Throws<ArgumentNullException>(() => structuredType.FindProperty(null, caseInsensitive: true));
            Assert.Equal("propertyName", ex.ParamName);
        }

        [Fact]
        public void FindProperty_WithCaseInsensitive_WorksForCaseSensitiveAndInsensitive()
        {
            // Arrange
            EdmComplexType structuredType = new EdmComplexType("NS", "Complex");
            structuredType.AddStructuralProperty("Title", EdmPrimitiveTypeKind.String);
            structuredType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            structuredType.AddStructuralProperty("nAme", EdmPrimitiveTypeKind.Int32);
            structuredType.AddStructuralProperty("naMe", EdmPrimitiveTypeKind.Double);

            // 1) Act & Assert: Cannot find the property
            IEdmProperty property = structuredType.FindProperty("Unknown", true);
            Assert.Null(property);

            property = structuredType.FindProperty("Unknown", false);
            Assert.Null(property);

            // 2) Act & Assert : Can find one "Title" property
            foreach (var name in new[] { "Title", "title", "tiTle", "TITLE" })
            {
                VerifyResolvedProperty(structuredType, name, "Title", "Edm.String");
            }

            // 3) Act & Assert: Can find the correct overload version
            VerifyResolvedProperty(structuredType, "Name", "Name", "Edm.String");
            VerifyResolvedProperty(structuredType, "nAme", "nAme", "Edm.Int32");
            VerifyResolvedProperty(structuredType, "naMe", "naMe", "Edm.Double");
        }

        private static void VerifyResolvedProperty(IEdmStructuredType structuredType, string propertyName, string expectedName, string expectedTypeName)
        {
            IEdmProperty property = structuredType.FindProperty(propertyName, true);
            Assert.NotNull(property);

            Assert.Equal(expectedName, property.Name);
            Assert.Equal(expectedTypeName, property.Type.FullName());
        }

        [Fact]
        public void FindProperty_WithCaseInsensitive_ThrowsForAmbiguousPropertyName()
        {
            // Arrange
            EdmComplexType structuredType = new EdmComplexType("NS", "Complex");
            structuredType.AddStructuralProperty("Title", EdmPrimitiveTypeKind.String);
            structuredType.AddStructuralProperty("tiTle", EdmPrimitiveTypeKind.Int32);
            structuredType.AddStructuralProperty("tiTlE", EdmPrimitiveTypeKind.Double);

            // Act & Assert - Positive case
            IEdmProperty edmProperty = structuredType.FindProperty("tiTlE", true);
            Assert.NotNull(edmProperty);
            Assert.Equal("Edm.Double", edmProperty.Type.FullName());

            // Act & Assert - Negative case
            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => structuredType.FindProperty("title", caseInsensitive: true));
            Assert.Equal("More than one properties match the name 'title' were found in type 'NS.Complex'.", ex.Message);
        }

        [Theory]
        [InlineData("TestModelNameSpace.MyFunction")]
        [InlineData("TestModelAlias.MyFunction")]
        public void FindBoundFunctionByNamespaceAndAlias(string operation)
        {
            var operations = TestModel.Instance.Model.FindBoundOperations(operation, TestModel.Instance.T1);
            var foundOperation = Assert.Single(operations);
            Assert.Equal("TestModelNameSpace.MyFunction", foundOperation.FullName());
        }

        [Theory]
        [InlineData("TestModelNameSpace.MyAction")]
        [InlineData("TestModelAlias.MyAction")]
        public void FindBoundActionByNamespaceAndAlias(string operation)
        {
            var operations = TestModel.Instance.Model.FindBoundOperations(operation, TestModel.Instance.T1);
            var foundOperation = Assert.Single(operations);
            Assert.Equal("TestModelNameSpace.MyAction", foundOperation.FullName());
        }

        [Fact]
        public void GetNamespaceAliasReturnsNullForNamespaceWithoutAlias()
        {
            Assert.Null(TestModel.Instance.Model.GetNamespaceAlias("SomeNamespace.NotIn.Model"));
        }

        [Fact]
        public void GetNamespaceAliasReturnsNullForModelsWithoutAliases()
        {
            EdmModel model = new EdmModel(false);
            Assert.Null(model.GetNamespaceAlias("SomeNamespace"));
        }

        [Fact]
        public void GetNamespaceAliasForNamespaceWithAlias()
        {
            Assert.Equal(TestModel.TestModelAlias, TestModel.Instance.Model.GetNamespaceAlias(TestModel.TestModelNameSpace));
            Assert.Equal(TestModel.TestModelAlias2, TestModel.Instance.Model.GetNamespaceAlias(TestModel.TestModelNameSpace2));
        }

        internal class TestModel
        {
            public static TestModel Instance = new TestModel();
            public const string TestModelNameSpace = "TestModelNameSpace";
            public const string TestModelAlias = "TestModelAlias";
            public const string TestModelNameSpace2 = "Multi.Part.TestModelNameSpace";
            public const string TestModelAlias2 = "MultipartTestModelAlias";

            private TestModel()
            {
                this.Model = new EdmModel();
                this.Model.SetNamespaceAlias(TestModelNameSpace, TestModelAlias);
                this.Model.SetNamespaceAlias(TestModelNameSpace2, TestModelAlias2);

                this.T1 = new EdmEntityType(TestModelNameSpace, "T1");
                EdmStructuralProperty p11 = this.T1.AddStructuralProperty("P11", EdmCoreModel.Instance.GetInt32(false));
                this.T1.AddKeys(p11);
                this.Model.AddElement(this.T1);

                this.T2 = new EdmEntityType(TestModelNameSpace, "T2");
                EdmStructuralProperty p21 = this.T2.AddStructuralProperty("P21", EdmCoreModel.Instance.GetInt32(false));
                this.T2.AddKeys(p21);
                this.Model.AddElement(this.T2);

                this.Model.AddElement(new EdmEnumType(TestModelNameSpace2, "E1"));

                var function = new EdmFunction(TestModelNameSpace, "MyFunction", EdmCoreModel.Instance.GetBoolean(false), true, null, false);
                function.AddParameter("entity", new EdmEntityTypeReference(this.T1, true));
                this.Model.AddElement(function);

                var action = new EdmAction(TestModelNameSpace, "MyAction", null, true, null);
                action.AddParameter("entity", new EdmEntityTypeReference(this.T1, true));
                this.Model.AddElement(action);

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

        internal static bool HasAnyExpensive<T>(IEnumerable<T> enumerable) where T : class
        {
            IList<T> list = enumerable as IList<T>;
            if (list != null)
            {
                return list.Count > 0;
            }

            T[] array = enumerable as T[];
            if (array != null)
            {
                return array.Length > 0;
            }

            return enumerable.FirstOrDefault() != null;
        }

        internal static bool HasAnyCheap<T>(IEnumerable<T> enumerable) where T : class
        {
            if (enumerable != null && enumerable.GetEnumerator().MoveNext())
            {
                return true;
            }

            return false;
        }
    }
}
