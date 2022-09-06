//---------------------------------------------------------------------
// <copyright file="ConstantExpressionValidationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConstantExpressionValidationTests : EdmLibTestCaseBase
    {
        [TestMethod]
        public void ValidateVocabularyAnnotationInvalidIntegerConstantExpressionCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidInteger }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.VocabularyAnnotationInvalidIntegerConstantExpressionCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateVocabularyAnnotationInvalidIntegerConstantAttributeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidInteger }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.VocabularyAnnotationInvalidIntegerConstantAttributeCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateVocabularyAnnotationInvalidBooleanConstantExpressionCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidBoolean }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.VocabularyAnnotationInvalidBooleanConstantExpressionCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateVocabularyAnnotationInvalidBooleanConstantAttributeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidBoolean }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.VocabularyAnnotationInvalidBooleanConstantAttributeCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateVocabularyAnnotationInvalidFloatConstantExpressionCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidFloatingPoint }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.VocabularyAnnotationInvalidFloatConstantExpressionCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateVocabularyAnnotationInvalidFloatConstantAttributeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidFloatingPoint }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.VocabularyAnnotationInvalidFloatConstantAttributeCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateVocabularyAnnotationInvalidDecimalConstantExpressionCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidDecimal }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.VocabularyAnnotationInvalidDecimalConstantExpressionCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateVocabularyAnnotationInvalidDecimalConstantAttributeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidDecimal }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.VocabularyAnnotationInvalidDecimalConstantAttributeCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateVocabularyAnnotationInvalidDurationConstantExpressionCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidDuration }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.VocabularyAnnotationInvalidDurationConstantExpressionCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateVocabularyAnnotationInvalidDurationConstantAttributeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidDuration }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.VocabularyAnnotationInvalidDurationConstantAttributeCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateVocabularyAnnotationInvalidGuidConstantExpressionCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidGuid }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.VocabularyAnnotationInvalidGuidConstantExpressionCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateVocabularyAnnotationInvalidGuidConstantAttributeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidGuid }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.VocabularyAnnotationInvalidGuidConstantAttributeCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateVocabularyAnnotationInvalidDateTimeOffsetConstantExpressionCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidDateTimeOffset }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.VocabularyAnnotationInvalidDateTimeOffsetConstantExpressionCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateVocabularyAnnotationInvalidDateTimeOffsetConstantAttributeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidDateTimeOffset }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.VocabularyAnnotationInvalidDateTimeOffsetConstantAttributeCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateVocabularyAnnotationInvalidBinaryConstantAttributeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidBinary }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.VocabularyAnnotationInvalidBinaryConstantAttributeCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateVocabularyAnnotationInvalidBinaryConstantExpressionCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidBinary }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.VocabularyAnnotationInvalidBinaryConstantExpressionCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateVocabularyAnnotationInvalidValueDurationConstantAttributeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidDuration }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.VocabularyAnnotationInvalidValueDurationConstantAttributeCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateVocabularyAnnotationInvalidFormatDurationConstantAttributeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidDuration }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.VocabularyAnnotationInvalidFormatDurationConstantAttributeCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateVocabularyAnnotationInvalidTypeReferenceDurationConstantModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType },
                { null, null, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.VocabularyAnnotationInvalidTypeReferenceDurationConstantModel(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateVocabularyAnnotationInvalidMaxTimeConstantAttributeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidDuration }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.VocabularyAnnotationInvalidMaxDurationConstantAttributeCsdl(), EdmVersion.V40, expectedErrors);
        }
    }
}
