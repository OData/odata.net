//---------------------------------------------------------------------
// <copyright file="VocabularyValidationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.Test.OData.Utils.Metadata;
#if SILVERLIGHT
    using Microsoft.Silverlight.Testing;
#endif
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VocabularyValidationTests : EdmLibTestCaseBase
    {

        public VocabularyValidationTests()
        {
            this.EdmVersion = EdmVersion.V40;
        }

        [TestMethod]
        public void SimpleValueAnnotation()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.SimpleValueAnnotationCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValueTerm_NameConflict()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {4, 4, EdmErrorCode.AlreadyDefined},
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueTermNameConflictCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValueTerm_NameConflict_WithOthers()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {3, 4, EdmErrorCode.AlreadyDefined},
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueTermNameConflictWithOthersCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValueTerm_TypeNotResolvable()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {3, 4, EdmErrorCode.BadUnresolvedType},
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueTermTypeNotResolvableCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValueAnnotation_TargetNotResolvable()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {8, 6, EdmErrorCode.BadUnresolvedType},
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationTargetNotResolvableCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValueAnnotation_TermNotResolvable()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                // Unresolved value terms are not reported as errors.
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationTermNotResolvableCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValueAnnotation_RecordTypeNotResolvable()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {9, 10, EdmErrorCode.BadUnresolvedType},
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationRecordTypeNotResolvableCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValueAnnotation_Ambiguous_SameTermNoQualifer()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {9, 6, EdmErrorCode.DuplicateAnnotation},
                {10, 6, EdmErrorCode.DuplicateAnnotation},
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationAmbiguousSameTermNoQualiferCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValueAnnotation_Ambiguous_SameTermSameQualifer()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {9, 6, EdmErrorCode.DuplicateAnnotation},
                {10, 6, EdmErrorCode.DuplicateAnnotation},
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationAmbiguousSameTermSameQualiferCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValueAnnotation_TypeExactMatch()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationPropertyTypeExactMatchCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValueAnnotation_TypeNotMatch()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {9,  6,  EdmErrorCode.PrimitiveConstantExpressionNotValidForNonPrimitiveType},
                {10, 6,  EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType},
                {12, 10, EdmErrorCode.RecordExpressionNotValidForNonStructuredType},
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationTypeNotMatchCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValueAnnotation_TypeStructure_PropertyNameNotMatch()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {9, 10, EdmErrorCode.RecordExpressionHasExtraProperties }
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationPropertyNameNotMatchCsdl(), expectedErrors);
        }

        //[TestMethod, Variation(Id = 46, SkipReason = @"[EdmLib] [Validator] Validation error is occuring for value term property that are nullable -- postponed")]
        public void ValueAnnotation_TypeStructure_NullablePropertyUndeclared()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationNullablePropertyUndeclaredCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValueAnnotation_TypeStructure_PropertyTypeNotMatch()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {10, 12, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType},
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationPropertyTypeNotMatchCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValueAnnotation_TypeStructure_Nested()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationNestedCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValueAnnotation_TypeStructure_NestedPropertyNotMatch()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {17, 12, EdmErrorCode.PrimitiveConstantExpressionNotValidForNonPrimitiveType},
                {21, 16, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType},
                {19, 14, EdmErrorCode.RecordExpressionHasExtraProperties}
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationNestedPropertyNotMatchCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValueAnnotation_TypeConvertible()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationTypeConvertibleCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValueAnnotation_ValueOutOfRange_BadFormat()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {16, 6, EdmErrorCode.InvalidInteger},
                {17, 6, EdmErrorCode.InvalidFloatingPoint},
                {19, 6, EdmErrorCode.InvalidDateTimeOffset},
                {20, 6, EdmErrorCode.InvalidGuid},
                {13, 6, EdmErrorCode.IntegerConstantValueOutOfRange},
                {14, 6, EdmErrorCode.IntegerConstantValueOutOfRange}
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationBadValueCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValueAnnotation_Path()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationPathCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValueAnnotation_Path_NotValid()
        {
            var expectedErrors = new EdmLibTestErrors();
            // No error is expected, since this behaviour is by design
            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationPathNotValidCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValueAnnotation_If()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationIfCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValueAnnotation_If_TypeNotMatch()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {13, 10, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType},
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationIfTypeNotMatchCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValueAnnotation_IsType_TypeNotResolved()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {11, 10, EdmErrorCode.BadUnresolvedType},
                {15, 10, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType},
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationIfTypeNotResolvedCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValueAnnotation_FunctionApplication()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationFunctionCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValueAnnotation_FunctionApplication_TypeNotMatch()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {16, 9, EdmErrorCode.BadUnresolvedOperation},
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationFunctionTypeNotMatchCsdl(), expectedErrors);
        }

        [TestMethod]
        public void VocabularyValidationDuplicateError()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                { "(EdmLibTests.StubEdm.StubEdmEntityType)", EdmErrorCode.KeyMissingOnEntityType },
                { "(EdmLibTests.StubEdm.StubEdmEntityType)", EdmErrorCode.KeyMissingOnEntityType },
                { "(EdmLibTests.VocabularyStubs.StubTypeTerm)", EdmErrorCode.KeyMissingOnEntityType }
            };
            this.VerifySemanticValidation(VocabularyTestModelBuilder.StructuredValueAnnotation(), this.EdmVersion, expectedErrors);
        }

        [TestMethod]
        public void ValidateValueTermOnly()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueTermOnlyCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateValueTermWithAnnotationTarget()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueTermWithAnnotationTargetCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateSimpleValueAnnotation()
        {
            EdmModel model = VocabularyTestModelBuilder.SimpleValueAnnotationModel();
            Assert.AreEqual(2, model.VocabularyAnnotations.Count(), "Invalid count of annotation.");

            IEnumerable<EdmError> actualErrors;
            model.Validate(out actualErrors);

            Assert.AreEqual(0, actualErrors.Count(), "Invalid count of errors");
        }

        [TestMethod]
        public void ValidateDefaultAnnotation()
        {
            EdmModel model = VocabularyTestModelBuilder.SimpleModel();

            var valueAnnotation = new MutableValueAnnotation();
            this.VerifyThrowsException(typeof(InvalidOperationException), () => model.AddVocabularyAnnotation(valueAnnotation));
        }

        [TestMethod]
        public void ValidateDefaultValueAnnotationWithTargetOnly()
        {
            EdmModel model = VocabularyTestModelBuilder.DefaultValueAnnotationWithTargetOnlyModel();

            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull},
                { null, null, EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull}
            };

            this.VerifySemanticValidation(model, expectedErrors);
        }

        [TestMethod]
        public void ValidateInlineAnnotationWithInvalidTargetModel()
        {
            EdmModel model = VocabularyTestModelBuilder.AnnotationWithInvalidTargetModel(EdmVocabularyAnnotationSerializationLocation.Inline);
            Assert.AreEqual(9, model.VocabularyAnnotations.Count(), "Invalid count of annotation.");
        }

        [TestMethod]
        public void ValidateOutOfLineAnnotationWithInvalidTargetModel()
        {
            EdmModel model = VocabularyTestModelBuilder.AnnotationWithInvalidTargetModel(EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            Assert.AreEqual(9, model.VocabularyAnnotations.Count(), "Invalid count of annotation.");
        }

        [TestMethod]
        public void ValidateOutOfLineAnnotationWithValidTargetsInReferencedModel()
        {
            // Test that annotation targets which live in a referenced model don't cause a validation failure
            const string rawModelCsdl = @"
<Schema Namespace=""ReferencedNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""SomeValueTerm"" Type=""Edm.String"" />
  <EntityContainer Name=""SomeContainer"">
    <EntitySet Name=""SomeEntitySet"" EntityType=""ReferencedNS.SomeEntityType"" />
    <ActionImport Name=""SomeFunctionImport"" Action=""ReferencedNS.SomeFunction"" />
  </EntityContainer>
  <EntityType Name=""SomeEntityType"">
    <Key>
      <PropertyRef Name=""ID"" />
    </Key>
    <Property Name=""ID"" Type=""Edm.String"" Nullable=""false"" />
  </EntityType>
  <Action Name=""SomeFunction""><ReturnType Type=""Edm.Int32""/>
    <Parameter Name=""SomeFunctionImportParameter"" Type=""Edm.String"" />
  </Action>
</Schema>";
            const string annotationModelCsdl = @"
<Schema Namespace=""AnnotationNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""CommonValueTerm"" Type=""Edm.String"" />
  <Annotations Target=""ReferencedNS.SomeContainer"">
    <Annotation Term=""AnnotationNS.CommonValueTerm"" String=""Hello world!"" />
  </Annotations>
  <Annotations Target=""ReferencedNS.SomeContainer/SomeEntitySet"">
    <Annotation Term=""AnnotationNS.CommonValueTerm"" String=""Hello world!"" />
  </Annotations>
  <Annotations Target=""ReferencedNS.SomeEntityType"">
    <Annotation Term=""AnnotationNS.CommonValueTerm"" String=""Hello world!"" />
  </Annotations>
  <Annotations Target=""ReferencedNS.SomeValueTerm"">
    <Annotation Term=""AnnotationNS.CommonValueTerm"" String=""Hello world!"" />
  </Annotations>
  <Annotations Target=""ReferencedNS.SomeFunction(Edm.String)"">
    <Annotation Term=""AnnotationNS.CommonValueTerm"" String=""Hello world!"" />
  </Annotations>
  <Annotations Target=""ReferencedNS.SomeContainer/SomeFunctionImport"">
    <Annotation Term=""AnnotationNS.CommonValueTerm"" String=""Hello world!"" />
  </Annotations>
  <Annotations Target=""ReferencedNS.SomeFunction(Edm.String)/SomeFunctionImportParameter"">
    <Annotation Term=""AnnotationNS.CommonValueTerm"" String=""Hello world!"" />
  </Annotations>
</Schema>";

            var rawModel = this.GetParserResult(new List<string> { rawModelCsdl });
            var modelWithAnnotations = this.GetParserResult(new List<string> { annotationModelCsdl }, rawModel);

            Assert.AreEqual(7, modelWithAnnotations.VocabularyAnnotations.Count(), "Invalid count of annotation.");

            var ruleSet = ValidationRuleSet.GetEdmModelRuleSet(Microsoft.OData.Edm.Library.EdmConstants.EdmVersion4);

            IEnumerable<EdmError> actualErrors = null;
            var validationResult = modelWithAnnotations.Validate(ruleSet, out actualErrors);
            Assert.IsTrue(validationResult, "Expected no validation errors from annotation with targets in an external model.");
        }

        [TestMethod]
        public void ValidateAnnotationWithInvalidTerm()
        {
            EdmModel model = VocabularyTestModelBuilder.AnnotationWithInvalidTermModel();
            Assert.AreEqual(1, model.VocabularyAnnotations.Count(), "Invalid count of annotation.");

            var expectedErrors = new EdmLibTestErrors()
            {
                { "(Microsoft.OData.Edm.Library.Annotations.EdmAnnotation)", EdmErrorCode.BadUnresolvedTerm }
            };

            IEnumerable<EdmError> actualErrors = null;
            var validationResult = model.Validate(Microsoft.OData.Edm.Library.EdmConstants.EdmVersionLatest, out actualErrors);
            Assert.IsTrue(actualErrors.Any() ? !validationResult : validationResult, "The return value of the Validate method does not match the reported validation errors.");
            this.CompareErrors(actualErrors, expectedErrors);
        }

        [TestMethod]
        public void ValidateDuplicateValueAnnotation()
        {
            EdmModel model = VocabularyTestModelBuilder.DuplicateValueAnnotationModel();
            Assert.AreEqual(2, model.VocabularyAnnotations.Count(), "Invalid count of annotation.");

            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.DuplicateAnnotation}
            };

            this.VerifySemanticValidation(model, expectedErrors);
        }

        [TestMethod]
        public void ValidateDuplicateValueAnnotationWithQualifier()
        {
            EdmModel model = VocabularyTestModelBuilder.DuplicateValueAnnotationWithQualifierModel();
            Assert.AreEqual(2, model.VocabularyAnnotations.Count(), "Invalid count of annotation.");

            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.DuplicateAnnotation}
            };

            this.VerifySemanticValidation(model, expectedErrors);
        }

        [TestMethod]
        public void ValidateValueAnnotationWithQualifier()
        {
            EdmModel model = VocabularyTestModelBuilder.ValueAnnotationWithQualifierModel();
            Assert.AreEqual(2, model.VocabularyAnnotations.Count(), "Invalid count of annotation.");

            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(model, expectedErrors);
        }

        [TestMethod]
        public void ValueAnnotation_TypeStructure_NullablePropertyWithNullExpression()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationNullablePropertyWithNullExpressionCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValidateValueAnnotationPropertyWithNullExpression()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {null, null, EdmErrorCode.NullCannotBeAssertedToBeANonNullableType}
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationPropertyWithNullExpressionCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValidateAnnotationsWithUnresolvedTermModel()
        {
            var csdls = new List<string>() { @"
<Schema Namespace=""NorthwindModel""
        Alias=""NorthwindModel""
        xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Annotations Target=""NorthwindModel.Customers"">
    <Annotation Term=""Catalog.Title"" String=""Customers"" />
  </Annotations>
</Schema>", @"
<Schema Namespace=""NorthwindModel""
        Alias=""NorthwindModel""
        xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
   <Annotations Target=""NorthwindModel.Customers"">
    <Annotation Term=""Catalog.Title"" String=""Customers"" />
  </Annotations>
</Schema>" }.Select(n => XElement.Parse(n));

            var model = this.GetParserResult(csdls);

            IEnumerable<EdmError> edmErrors;
            model.Validate(out edmErrors);
            Assert.IsTrue(!edmErrors.Any(n => n.ErrorMessage.Contains("Catalog")), "The validator should report no unresolved term errors. This is the only valiation rule that was taken off.");
        }

        [TestMethod]
        public void ValidateSimpleValueAnnotationWithComplexTypeModel()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.SimpleValueAnnotationWithComplexTypeModel(), expectedErrors);
        }

        [TestMethod]
        public void ValidateAnnotationComplexTypeWithMissingAndExtraPropertiesModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { 11, 14, EdmErrorCode.RecordExpressionHasExtraProperties },
                { 21, 18, EdmErrorCode.RecordExpressionHasExtraProperties }
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.AnnotationComplexTypeWithMissingOrExtraPropertiesCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValidateAnnotationEntityTypeWithMissingAndExtraPropertiesModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { 23, 14, EdmErrorCode.RecordExpressionHasExtraProperties },
                { 33, 18, EdmErrorCode.RecordExpressionHasExtraProperties }
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.AnnotationEntityTypeWithMissingOrExtraPropertiesCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValidateValueAnnotationComplexTypeWithNullValuesModel()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationComplexTypeWithNullValuesModel(), expectedErrors);
        }

        [TestMethod]
        public void ValidateValueAnnotationWithCollectionComplexTypeModel()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationWithCollectionComplexTypeModel(), expectedErrors);
        }

        [TestMethod]
        public void ValidateValueAnnotationNonNullablePropertyWithNullValueCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.NullCannotBeAssertedToBeANonNullableType }
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.ValueAnnotationNonNullablePropertyWithNullValueCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValidateEntitySetCanBeContainedByMultipleNavigationProperties()
        {
            var csdl = @"
<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Content"">
      <Key>
          <PropertyRef Name=""ID"" />
      </Key>
      <Property Name=""ID"" Type=""Int32"" Nullable=""false"" />
      <NavigationProperty Name=""Fields"" Type=""Collection(NS1.Field)"" ContainsTarget=""true"" />
    </EntityType>
    <EntityType Name=""List"">
      <Key>
          <PropertyRef Name=""ID"" />
      </Key>
     <Property Name=""ID"" Type=""Int32"" Nullable=""false"" />
      <NavigationProperty Name=""Fields"" Type=""Collection(NS1.Field)"" ContainsTarget=""true"" />
    </EntityType>
    <EntityType Name=""Field"">
      <Key>
          <PropertyRef Name=""ID"" />
      </Key>
      <Property Name=""ID"" Type=""Int32"" Nullable=""false"" />
      <NavigationProperty Name=""Description"" Type=""NS1.List"" Nullable=""false"" />
    </EntityType>
    <EntityContainer Name=""C1"">
      <EntitySet Name=""Contents"" EntityType=""NS1.Content"">
        <NavigationPropertyBinding Path=""Fields"" Target=""ContentFields""/>
      </EntitySet>
      <EntitySet Name=""Lists"" EntityType=""NS1.List"">
        <NavigationPropertyBinding Path=""Fields"" Target=""ListFields""/>
      </EntitySet>
      <EntitySet Name=""ContentFields"" EntityType=""NS1.Field"" />
      <EntitySet Name=""ListFields"" EntityType=""NS1.Field"" />
      <EntitySet Name=""Descriptions"" EntityType=""NS1.Field"" />
    </EntityContainer>
</Schema>";

            this.VerifySemanticValidation(new[] { XElement.Parse(csdl) }, new EdmLibTestErrors());
        }
    }
}
