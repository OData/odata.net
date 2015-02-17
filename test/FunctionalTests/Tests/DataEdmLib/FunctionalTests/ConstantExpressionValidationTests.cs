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
#if SILVERLIGHT
    using Microsoft.Silverlight.Testing;
#endif
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConstantExpressionValidationTests : EdmLibTestCaseBase
    {
        [TestMethod]
        public void ValidateValueAnnotationInvalidIntegerConstantExpressionCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidInteger }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.ValueAnnotationInvalidIntegerConstantExpressionCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateValueAnnotationInvalidIntegerConstantAttributeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidInteger }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.ValueAnnotationInvalidIntegerConstantAttributeCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateValueAnnotationInvalidBooleanConstantExpressionCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidBoolean }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.ValueAnnotationInvalidBooleanConstantExpressionCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateValueAnnotationInvalidBooleanConstantAttributeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidBoolean }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.ValueAnnotationInvalidBooleanConstantAttributeCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateValueAnnotationInvalidFloatConstantExpressionCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidFloatingPoint }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.ValueAnnotationInvalidFloatConstantExpressionCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateValueAnnotationInvalidFloatConstantAttributeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidFloatingPoint }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.ValueAnnotationInvalidFloatConstantAttributeCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateValueAnnotationInvalidDecimalConstantExpressionCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidDecimal }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.ValueAnnotationInvalidDecimalConstantExpressionCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateValueAnnotationInvalidDecimalConstantAttributeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidDecimal }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.ValueAnnotationInvalidDecimalConstantAttributeCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateValueAnnotationInvalidDurationConstantExpressionCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidDuration }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.ValueAnnotationInvalidDurationConstantExpressionCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateValueAnnotationInvalidDurationConstantAttributeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidDuration }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.ValueAnnotationInvalidDurationConstantAttributeCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateValueAnnotationInvalidGuidConstantExpressionCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidGuid }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.ValueAnnotationInvalidGuidConstantExpressionCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateValueAnnotationInvalidGuidConstantAttributeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidGuid }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.ValueAnnotationInvalidGuidConstantAttributeCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateValueAnnotationInvalidDateTimeOffsetConstantExpressionCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidDateTimeOffset }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.ValueAnnotationInvalidDateTimeOffsetConstantExpressionCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateValueAnnotationInvalidDateTimeOffsetConstantAttributeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidDateTimeOffset }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.ValueAnnotationInvalidDateTimeOffsetConstantAttributeCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateValueAnnotationInvalidBinaryConstantAttributeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidBinary }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.ValueAnnotationInvalidBinaryConstantAttributeCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateValueAnnotationInvalidBinaryConstantExpressionCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidBinary }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.ValueAnnotationInvalidBinaryConstantExpressionCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateValueAnnotationInvalidValueDurationConstantAttributeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidDuration }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.ValueAnnotationInvalidValueDurationConstantAttributeCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateValueAnnotationInvalidFormatDurationConstantAttributeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidDuration }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.ValueAnnotationInvalidFormatDurationConstantAttributeCsdl(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateValueAnnotationInvalidTypeReferenceDurationConstantModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType },
                { null, null, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.ValueAnnotationInvalidTypeReferenceDurationConstantModel(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateValueAnnotationInvalidMaxTimeConstantAttributeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidDuration }
            };

            this.VerifySemanticValidation(ConstantExpressionModelBuilder.ValueAnnotationInvalidMaxDurationConstantAttributeCsdl(), EdmVersion.V40, expectedErrors);
        }
    }
}
