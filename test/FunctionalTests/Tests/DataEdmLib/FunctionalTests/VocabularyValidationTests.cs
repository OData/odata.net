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
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VocabularyValidationTests : EdmLibTestCaseBase
    {

        public VocabularyValidationTests()
        {
            this.EdmVersion = EdmVersion.V40;
        }

        [TestMethod]
        public void SimpleVocabularyAnnotation()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.SimpleVocabularyAnnotationCsdl(), expectedErrors);
        }

        [TestMethod]
        public void Term_NameConflict()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {4, 4, EdmErrorCode.AlreadyDefined},
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.TermNameConflictCsdl(), expectedErrors);
        }

        [TestMethod]
        public void Term_NameConflict_WithOthers()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {3, 4, EdmErrorCode.AlreadyDefined},
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.TermNameConflictWithOthersCsdl(), expectedErrors);
        }

        [TestMethod]
        public void Term_TypeNotResolvable()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {3, 4, EdmErrorCode.BadUnresolvedType},
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.TermTypeNotResolvableCsdl(), expectedErrors);
        }

        [TestMethod]
        public void VocabularyAnnotation_TargetNotResolvable()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {8, 6, EdmErrorCode.BadUnresolvedType},
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationTargetNotResolvableCsdl(), expectedErrors);
        }

        [TestMethod]
        public void VocabularyAnnotation_TermNotResolvable()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                // Unresolved terms are not reported as errors.
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationTermNotResolvableCsdl(), expectedErrors);
        }

        [TestMethod]
        public void VocabularyAnnotation_RecordTypeNotResolvable()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {9, 10, EdmErrorCode.BadUnresolvedType},
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationRecordTypeNotResolvableCsdl(), expectedErrors);
        }

        [TestMethod]
        public void VocabularyAnnotation_Ambiguous_SameTermNoQualifer()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {9, 6, EdmErrorCode.DuplicateAnnotation},
                {10, 6, EdmErrorCode.DuplicateAnnotation},
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationAmbiguousSameTermNoQualiferCsdl(), expectedErrors);
        }

        [TestMethod]
        public void VocabularyAnnotation_Ambiguous_SameTermSameQualifer()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {9, 6, EdmErrorCode.DuplicateAnnotation},
                {10, 6, EdmErrorCode.DuplicateAnnotation},
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationAmbiguousSameTermSameQualiferCsdl(), expectedErrors);
        }

        [TestMethod]
        public void VocabularyAnnotation_TypeExactMatch()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationPropertyTypeExactMatchCsdl(), expectedErrors);
        }

        [TestMethod]
        public void VocabularyAnnotation_TypeNotMatch()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {9,  6,  EdmErrorCode.PrimitiveConstantExpressionNotValidForNonPrimitiveType},
                {12, 10, EdmErrorCode.RecordExpressionNotValidForNonStructuredType},
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationTypeNotMatchCsdl(), expectedErrors);
        }

        [TestMethod]
        public void VocabularyAnnotation_TypeStructure_PropertyNameNotMatch()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {9, 10, EdmErrorCode.RecordExpressionHasExtraProperties }
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationPropertyNameNotMatchCsdl(), expectedErrors);
        }

        //[TestMethod, Variation(Id = 46, SkipReason = @"[EdmLib] [Validator] Validation error is occuring for term property that are nullable -- postponed")]
        public void VocabularyAnnotation_TypeStructure_NullablePropertyUndeclared()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationNullablePropertyUndeclaredCsdl(), expectedErrors);
        }

        [TestMethod]
        public void VocabularyAnnotation_TypeStructure_PropertyTypeNotMatch()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {10, 12, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType},
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationPropertyTypeNotMatchCsdl(), expectedErrors);
        }

        [TestMethod]
        public void VocabularyAnnotation_TypeStructure_Nested()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationNestedCsdl(), expectedErrors);
        }

        [TestMethod]
        public void VocabularyAnnotation_TypeStructure_NestedPropertyNotMatch()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {17, 12, EdmErrorCode.PrimitiveConstantExpressionNotValidForNonPrimitiveType},
                {21, 16, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType},
                {19, 14, EdmErrorCode.RecordExpressionHasExtraProperties}
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationNestedPropertyNotMatchCsdl(), expectedErrors);
        }

        [TestMethod]
        public void VocabularyAnnotation_TypeConvertible()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationTypeConvertibleCsdl(), expectedErrors);
        }

        [TestMethod]
        public void VocabularyAnnotation_ValueOutOfRange_BadFormat()
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

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationBadValueCsdl(), expectedErrors);
        }

        [TestMethod]
        public void VocabularyAnnotation_Path()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationPathCsdl(), expectedErrors);
        }

        [TestMethod]
        public void VocabularyAnnotation_Path_NotValid()
        {
            var expectedErrors = new EdmLibTestErrors();
            // No error is expected, since this behaviour is by design
            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationPathNotValidCsdl(), expectedErrors);
        }

        [TestMethod]
        public void VocabularyAnnotation_If()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationIfCsdl(), expectedErrors);
        }

        [TestMethod]
        public void VocabularyAnnotation_If_TypeNotMatch()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {13, 10, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType},
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationIfTypeNotMatchCsdl(), expectedErrors);
        }

        [TestMethod]
        public void VocabularyAnnotation_IsType_TypeNotResolved()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {11, 10, EdmErrorCode.BadUnresolvedType},
                {15, 10, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType},
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationIfTypeNotResolvedCsdl(), expectedErrors);
        }

        [TestMethod]
        public void VocabularyAnnotation_FunctionApplication()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationFunctionCsdl(), expectedErrors);
        }

        [TestMethod]
        public void VocabularyAnnotation_FunctionApplication_TypeNotMatch()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {16, 9, EdmErrorCode.BadUnresolvedOperation},
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationFunctionTypeNotMatchCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValidateVocabularyWithIncorrectType()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {null, null, EdmErrorCode.ExpressionNotValidForTheAssertedType }
            };
            this.VerifySemanticValidation(VocabularyTestModelBuilder.StructuredVocabularyAnnotation(), this.EdmVersion, expectedErrors);
        }

        [TestMethod]
        public void ValidateTermOnly()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.TermOnlyCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateTermWithAnnotationTarget()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.TermWithAnnotationTargetCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateSimpleVocabularyAnnotation()
        {
            EdmModel model = VocabularyTestModelBuilder.SimpleVocabularyAnnotationModel();
            Assert.AreEqual(2, model.VocabularyAnnotations.Count(), "Invalid count of annotation.");

            IEnumerable<EdmError> actualErrors;
            model.Validate(out actualErrors);

            Assert.AreEqual(0, actualErrors.Count(), "Invalid count of errors");
        }

        [TestMethod]
        public void ValidateDefaultAnnotation()
        {
            EdmModel model = VocabularyTestModelBuilder.SimpleModel();

            var annotation = new MutableVocabularyAnnotation();
            this.VerifyThrowsException(typeof(InvalidOperationException), () => model.AddVocabularyAnnotation(annotation));
        }

        [TestMethod]
        public void ValidateDefaultVocabularyAnnotationWithTargetOnly()
        {
            EdmModel model = VocabularyTestModelBuilder.DefaultVocabularyAnnotationWithTargetOnlyModel();

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

            var ruleSet = ValidationRuleSet.GetEdmModelRuleSet(Microsoft.OData.Edm.EdmConstants.EdmVersion4);

            IEnumerable<EdmError> actualErrors = null;
            var validationResult = modelWithAnnotations.Validate(ruleSet, out actualErrors);
            Assert.IsTrue(validationResult, "Expected no validation errors from annotation with targets in an external model.");
        }

        [TestMethod]
        public void ValidateUnresolvedInlineAnnotationTargets()
        {
            // Test that unresolved annotation terms don't cause a validation failure
            const string csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""CommonValueTerm"" Type=""Edm.String"" />
  <EntityContainer Name=""SomeContainer"">
    <EntitySet Name=""SomeEntitySet"" EntityType=""NS.SomeEntityType"">
      <Annotation Term=""NS.CommonValueTerm"" String=""Hello world!"" />
      <Annotation Term=""NS.UnknownValueTerm"" String=""Hello world!"" />
      <Annotation Term=""RefNS.UnknownValueTerm"" String=""Hello world!""/>
    </EntitySet>
    <ActionImport Name=""SomeFunctionImport"" Action=""NS.SomeFunction"" >
      <Annotation Term=""NS.CommonValueTerm"" String=""Hello world!"" />
      <Annotation Term=""AnnotationNS.UnknownValueTerm"" String=""Hello world!"" />
      <Annotation Term=""RefNS.UnknownValueTerm"" String=""Hello world!""/>
    </ActionImport>
    <Annotation Term=""NS.CommonValueTerm"" String=""Hello world!"">
      <Annotation Term=""AnnotationNS.UnknownValueTerm"" String=""Hello world!"" />
      <Annotation Term=""RefNS.UnknownValueTerm"" String=""Hello world!""/>
    </Annotation>
    <Annotation Term=""AnnotationNS.UnknownValueTerm"" String=""Hello world!"" />
    <Annotation Term=""RefNS.UnknownValueTerm"" String=""Hello world!""/>
  </EntityContainer>
  <EntityType Name=""SomeEntityType"">
    <Key>
      <PropertyRef Name=""ID"" />
    </Key>
    <Property Name=""ID"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""NS.CommonValueTerm"" String=""Hello world!"" />
      <Annotation Term=""AnnotationNS.UnknownValueTerm"" String=""Hello world!"" />
      <Annotation Term=""RefNS.UnknownValueTerm"" String=""Hello world!""/>
    </Property>
    <Annotation Term=""NS.CommonValueTerm"" String=""Hello world!"" />
    <Annotation Term=""AnnotationNS.UnknownValueTerm"" String=""Hello world!"" />
    <Annotation Term=""RefNS.UnknownValueTerm"" String=""Hello world!""/>
  </EntityType>
  <Action Name=""SomeFunction"">
    <ReturnType Type=""Edm.Int32""/>
    <Parameter Name=""SomeFunctionImportParameter"" Type=""Edm.String"" >
      <Annotation Term=""NS.CommonValueTerm"" String=""Hello world!"" />
      <Annotation Term=""AnnotationNS.UnknownValueTerm"" String=""Hello world!"" />
      <Annotation Term=""RefNS.UnknownValueTerm"" String=""Hello world!""/>
    </Parameter>
    <Annotation Term=""NS.CommonValueTerm"" String=""Hello world!"" />
    <Annotation Term=""AnnotationNS.UnknownValueTerm"" String=""Hello world!"" />
    <Annotation Term=""RefNS.UnknownValueTerm"" String=""Hello world!""/>
  </Action>
</Schema>";

            var model = this.GetParserResult(new List<string> { csdl });

            // Note: we don't currently appear to support annotations on annotations, which is why the count
            // is 21 when in fact there are 23 in the payload.
            Assert.AreEqual(21, model.VocabularyAnnotations.Count(), "Invalid count of annotation.");

            var ruleSet = ValidationRuleSet.GetEdmModelRuleSet(Microsoft.OData.Edm.EdmConstants.EdmVersion4);

            IEnumerable<EdmError> actualErrors = null;
            var validationResult = model.Validate(ruleSet, out actualErrors);
            Assert.IsTrue(validationResult, "Expected no validation errors from annotation with unresolved terms.");
        }


        [TestMethod]
        public void ValidateUnresolvedInlineAnnotationTargetsWithUnresolvedTypes()
        {
            // Test that unresolved annotation term types load and raise appropriate validation failures
            const string csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""CommonValueTerm"" Type=""Edm.String"" />
  <EntityContainer Name=""SomeContainer"">
    <EntitySet Name=""SomeEntitySet"" EntityType=""NS.SomeEntityType"">
      <Annotation Term=""RefNS.UnknownValueTerm"">
        <Record Type=""RefNs.UnknownType"">
          <PropertyValue Property=""Property"" String=""PropertyValue"" />
        </Record>
      </Annotation>
    </EntitySet>
    <ActionImport Name=""SomeFunctionImport"" Action=""NS.SomeFunction"" >
      <Annotation Term=""RefNS.UnknownValueTerm"">
        <Record Type=""RefNs.UnknownType"">
          <PropertyValue Property=""Property"" String=""PropertyValue"" />
        </Record>
      </Annotation>
    </ActionImport>
    <Annotation Term=""NS.CommonValueTerm"" String=""Hello world!"">
      <Annotation Term=""RefNS.UnknownValueTerm"">
        <Record Type=""RefNs.UnknownType"">
          <PropertyValue Property=""Property"" String=""PropertyValue"" />
        </Record>
      </Annotation>
    </Annotation>
    <Annotation Term=""RefNS.UnknownValueTerm"">
      <Record Type=""RefNs.UnknownType"">
        <PropertyValue Property=""Property"" String=""PropertyValue"" />
      </Record>
    </Annotation>
  </EntityContainer>
  <EntityType Name=""SomeEntityType"">
    <Key>
      <PropertyRef Name=""ID"" />
    </Key>
    <Property Name=""ID"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""RefNS.UnknownValueTerm"">
        <Record Type=""RefNs.UnknownType"">
          <PropertyValue Property=""Property"" String=""PropertyValue"" />
        </Record>
      </Annotation>
    </Property>
    <Annotation Term=""RefNS.UnknownValueTerm"">
      <Record Type=""RefNs.UnknownType"">
        <PropertyValue Property=""Property"" String=""PropertyValue"" />
      </Record>
    </Annotation>
  </EntityType>
  <Action Name=""SomeFunction""><ReturnType Type=""Edm.Int32""/>
    <Parameter Name=""SomeFunctionImportParameter"" Type=""Edm.String"" >
      <Annotation Term=""RefNS.UnknownValueTerm"">
        <Record Type=""RefNs.UnknownType"">
          <PropertyValue Property=""Property"" String=""PropertyValue"" />
        </Record>
      </Annotation>
    </Parameter>
    <Annotation Term=""RefNS.UnknownValueTerm"">
      <Record Type=""RefNs.UnknownType"">
        <PropertyValue Property=""Property"" String=""PropertyValue"" />
      </Record>
    </Annotation>
  </Action>
</Schema>";

            var model = this.GetParserResult(new List<string> { csdl });

            // Note: we don't currently appear to support annotations on annotations, which is why the count
            // is 8 when in fact there are 9 in the payload.
            Assert.AreEqual(8, model.VocabularyAnnotations.Count(), "Invalid count of annotation.");

            var ruleSet = ValidationRuleSet.GetEdmModelRuleSet(Microsoft.OData.Edm.EdmConstants.EdmVersion4);

            IEnumerable<EdmError> actualErrors = null;
            var validationResult = model.Validate(ruleSet, out actualErrors);
            Assert.AreEqual(7, actualErrors.Count(), "Expected errors for unresolved types.");
        }

        [TestMethod]
        public void ValidateAnnotationWithInvalidTerm()
        {
            EdmModel model = VocabularyTestModelBuilder.AnnotationWithInvalidTermModel();
            Assert.AreEqual(1, model.VocabularyAnnotations.Count(), "Invalid count of annotation.");

            var expectedErrors = new EdmLibTestErrors()
            {
                { "(Microsoft.OData.Edm.Vocabularies.EdmVocabularyAnnotation)", EdmErrorCode.BadUnresolvedTerm }
            };

            IEnumerable<EdmError> actualErrors = null;
            var validationResult = model.Validate(Microsoft.OData.Edm.EdmConstants.EdmVersionLatest, out actualErrors);
            Assert.IsTrue(actualErrors.Any() ? !validationResult : validationResult, "The return value of the Validate method does not match the reported validation errors.");
            this.CompareErrors(actualErrors, expectedErrors);
        }

        [TestMethod]
        public void ValidateDuplicateVocabularyAnnotation()
        {
            EdmModel model = VocabularyTestModelBuilder.DuplicateVocabularyAnnotationModel();
            Assert.AreEqual(2, model.VocabularyAnnotations.Count(), "Invalid count of annotation.");

            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.DuplicateAnnotation}
            };

            this.VerifySemanticValidation(model, expectedErrors);
        }

        [TestMethod]
        public void ValidateDuplicateVocabularyAnnotationWithQualifier()
        {
            EdmModel model = VocabularyTestModelBuilder.DuplicateVocabularyAnnotationWithQualifierModel();
            Assert.AreEqual(2, model.VocabularyAnnotations.Count(), "Invalid count of annotation.");

            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.DuplicateAnnotation}
            };

            this.VerifySemanticValidation(model, expectedErrors);
        }

        [TestMethod]
        public void ValidateVocabularyAnnotationWithQualifier()
        {
            EdmModel model = VocabularyTestModelBuilder.VocabularyAnnotationWithQualifierModel();
            Assert.AreEqual(2, model.VocabularyAnnotations.Count(), "Invalid count of annotation.");

            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(model, expectedErrors);
        }

        [TestMethod]
        public void VocabularyAnnotation_TypeStructure_NullablePropertyWithNullExpression()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationNullablePropertyWithNullExpressionCsdl(), expectedErrors);
        }

        [TestMethod]
        public void ValidateVocabularyAnnotationPropertyWithNullExpression()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {null, null, EdmErrorCode.NullCannotBeAssertedToBeANonNullableType}
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationPropertyWithNullExpressionCsdl(), expectedErrors);
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
        public void ValidateSimpleVocabularyAnnotationWithComplexTypeModel()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.SimpleVocabularyAnnotationWithComplexTypeModel(), expectedErrors);
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
        public void ValidateVocabularyAnnotationComplexTypeWithNullValuesModel()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationComplexTypeWithNullValuesModel(), expectedErrors);
        }

        [TestMethod]
        public void ValidateVocabularyAnnotationWithCollectionComplexTypeModel()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationWithCollectionComplexTypeModel(), expectedErrors);
        }

        [TestMethod]
        public void ValidateVocabularyAnnotationNonNullablePropertyWithNullValueCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.NullCannotBeAssertedToBeANonNullableType }
            };

            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationNonNullablePropertyWithNullValueCsdl(), expectedErrors);
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
