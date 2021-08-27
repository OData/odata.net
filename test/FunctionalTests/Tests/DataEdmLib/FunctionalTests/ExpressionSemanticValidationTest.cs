//---------------------------------------------------------------------
// <copyright file="ExpressionSemanticValidationTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System.Collections.Generic;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ExpressionSemanticValidationTest : EdmLibTestCaseBase
    {
        public ExpressionSemanticValidationTest()
        {
            this.EdmVersion = EdmVersion.Latest;
        }

        [TestMethod]
        public void NullCannotBeAssertedToBeANonNullableType()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {6, 8, EdmErrorCode.NullCannotBeAssertedToBeANonNullableType}
            };
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.NullForNonNullableTerm(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void CannotReconcilePrimitiveExpressionWithNonPrimitiveType()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {5, 6, EdmErrorCode.PrimitiveConstantExpressionNotValidForNonPrimitiveType}
            };
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.PrimitiveForNonPrimitiveTerm(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void IncorrectPrimitiveTypeForTerm()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {5, 6, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType}
            };
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.IncorrectPrimitiveTypeForTerm(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void OkayPrimitiveTerm()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.OkayPrimitiveTerm(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void OkayCollectionTerm()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.OkayCollectionTerm(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void OkayRecordTerm()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.OkayRecordTerm(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void BadCollectionTermItemOfIncorrectType()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {9, 14, EdmErrorCode.InvalidInteger}
            };
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.BadCollectionTermItemOfIncorrectType(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void BadCollectionTermIncorrectDeclaredType()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {6, 10, EdmErrorCode.ExpressionNotValidForTheAssertedType},
                {7, 14, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType},
                {8, 14, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType}
            };
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.BadCollectionTermIncorrectDeclaredType(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void BadRecordTermRenamedProperty()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {6, 10, EdmErrorCode.RecordExpressionHasExtraProperties}
            };
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.BadRecordTermRenamedProperty(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void BadRecordTermMisTypedProperty()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {11, 14, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType}
            };
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.BadRecordTermMisTypedProperty(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void BadCollectionElementInconsistentWithDeclaredType()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {8, 14, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType}
            };
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.BadCollectionElementInconsistentWithDeclaredType(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void BadRecordWithNonStructuredType()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {12, 10, EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference},
            };
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.BadRecordWithNonStructuredType(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void BadRecordTermMisTypedPropertyForUntypedTerm()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {16, 14, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType}
            };
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.BadRecordTermMisTypedPropertyForUntypedTerm(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void BinaryValueInHexidecimalFormat()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {4, 10, EdmErrorCode.InvalidBinary}
            };

            var model = this.GetParserResult(ExpressionValidationTestModelBuilder.BinaryValueInHexidecimalFormat(this.EdmVersion));
            IEnumerable<EdmError> actualErrors = null;
            model.Validate(out actualErrors);

            this.CompareErrors(actualErrors, expectedErrors);
        }

        [TestMethod]
        public void NoErrorsForComplexTypeTerm()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.ComplexTypeTerm(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateInvalidTypeUsingCastCollectionCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.ExpressionNotValidForTheAssertedType }
            };
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.InvalidTypeUsingCastCollectionCsdl(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateInvalidTypeUsingCastCollectionModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.ExpressionNotValidForTheAssertedType }
            };
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.InvalidTypeUsingCastCollectionModel(), expectedErrors);
        }

        [TestMethod]
        public void ValidateCastNullableToNonNullableCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.ExpressionNotValidForTheAssertedType }
            };
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.CastNullableToNonNullableCsdl(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateCastNullableToNonNullableModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.ExpressionNotValidForTheAssertedType }
            };
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.CastNullableToNonNullableModel(), expectedErrors);
        }

        [TestMethod]
        public void ValidateCastNullableToNonNullableOnInlineAnnotationCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.ExpressionNotValidForTheAssertedType }
            };
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.CastNullableToNonNullableOnInlineAnnotationCsdl(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateCastNullableToNonNullableOnInlineAnnotationModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.ExpressionNotValidForTheAssertedType }
            };
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.CastNullableToNonNullableOnInlineAnnotationModel(), expectedErrors);
        }

        [TestMethod]
        public void ValidateCastResultFalseEvaluationCsdl()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.CastResultFalseEvaluationCsdl(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateCastResultFalseEvaluationModel()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.CastResultFalseEvaluationModel(), expectedErrors);
        }

        [TestMethod]
        public void ValidateCastResultTrueEvaluationCsdl()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.CastResultTrueEvaluationCsdl(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateCastResultTrueEvaluationModel()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.CastResultTrueEvaluationModel(), expectedErrors);
        }

        [TestMethod]
        public void ValidateInvalidPropertyTypeUsingIsTypeOnOutOfLineAnnotationCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType }
            };
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.InvalidPropertyTypeUsingIsTypeOnOutOfLineAnnotationCsdl(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateInvalidPropertyTypeUsingIsTypeOnOutOfLineAnnotationModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType }
            };
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.InvalidPropertyTypeUsingIsTypeOnOutOfLineAnnotationModel(), expectedErrors);
        }

        [TestMethod]
        public void ValidateInvalidPropertyTypeUsingIsTypeOnInlineAnnotationCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.ExpressionNotValidForTheAssertedType }
            };
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.InvalidPropertyTypeUsingIsTypeOnInlineAnnotationCsdl(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateInvalidPropertyTypeUsingIsTypeOnInlineAnnotationModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.ExpressionNotValidForTheAssertedType }
            };
            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.InvalidPropertyTypeUsingIsTypeOnInlineAnnotationModel(), expectedErrors);
        }

        [TestMethod]
        public void ValidateIsTypeResultFalseEvaluationCsdl()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.IsTypeResultFalseEvaluationCsdl(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateIsTypeResultFalseEvaluationModel()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.IsTypeResultFalseEvaluationModel(), expectedErrors);
        }

        [TestMethod]
        public void ValidateIsTypeResultTrueEvaluationCsdl()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.IsTypeResultTrueEvaluationCsdl(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateIsTypeResultTrueEvaluationModel()
        {
            var expectedErrors = new EdmLibTestErrors();

            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.IsTypeResultTrueEvaluationModel(), expectedErrors);
        }

        [TestMethod]
        public void IncorrectTypeForCollectionExpression()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { 6, 10, EdmErrorCode.CollectionExpressionNotValidForNonCollectionType }
            };

            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.IncorrectTypeForCollectionExpression(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void IncorrectTypeForGuidExpression()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { 6, 10, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType }
            };

            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.IncorrectTypeForGuidExpression(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void IncorrectTypeForFloatingExpression()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { 6, 10, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType }
            };

            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.IncorrectTypeForFloatingExpression(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void IncorrectTypeForDecimalExpression()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { 6, 10, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType }
            };

            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.IncorrectTypeForDecimalExpression(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void IncorrectTypeForDateExpression()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { 6, 10, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType }
            };

            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.IncorrectTypeForDateExpression(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void IncorrectFormatForDateExpression()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { 6, 10, EdmErrorCode.InvalidDate }
            };

            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.IncorrectFormatorDateExpression(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void IncorrectTypeForDateTimeOffsetExpression()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { 6, 10, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType }
            };

            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.IncorrectTypeForDateTimeOffsetExpression(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void IncorrectTypeForBooleanExpression()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { 6, 10, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType }
            };

            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.IncorrectTypeForBooleanExpression(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void IncorrectTypeForTimeOfDayExpression()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { 6, 10, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType }
            };

            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.IncorrectTypeForTimeOfDayExpression(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void IncorrectFormatForTimeOfDayExpression()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { 6, 10, EdmErrorCode.InvalidTimeOfDay }
            };

            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.IncorrectFormatorTimeOfDayExpression(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void StringTooLong()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { 6, 10, EdmErrorCode.StringConstantLengthOutOfRange }
            };

            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.StringTooLong(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void BinaryTooLong()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { 6, 10, EdmErrorCode.BinaryConstantLengthOutOfRange }
            };

            this.VerifySemanticValidation(ExpressionValidationTestModelBuilder.BinaryTooLong(this.EdmVersion), expectedErrors);
        }
    }
}
