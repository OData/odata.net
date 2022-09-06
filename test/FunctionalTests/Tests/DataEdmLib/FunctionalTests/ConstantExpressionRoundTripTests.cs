//---------------------------------------------------------------------
// <copyright file="ConstantExpressionRoundTripTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConstantExpressionRoundTripTests : EdmLibTestCaseBase
    {
        #region Duration
        [TestMethod]
        public void RoundTripTestVocabularyAnnotationInvalidDurationConstantExpressionCsdl()
        {
            var expectedCsdl = ConstantExpressionModelBuilder.VocabularyAnnotationValidDefaultDurationConstantAttributeCsdl();
            var csdl = ConstantExpressionModelBuilder.VocabularyAnnotationInvalidDurationConstantExpressionCsdl();
            VerifyRoundTrip(expectedCsdl, csdl);
        }

        [TestMethod]
        public void RoundTripTestVocabularyAnnotationInvalidDurationConstantAttributeCsdl()
        {
            var expectedCsdl = ConstantExpressionModelBuilder.VocabularyAnnotationValidDefaultDurationConstantAttributeCsdl();
            var csdl = ConstantExpressionModelBuilder.VocabularyAnnotationInvalidDurationConstantAttributeCsdl();
            VerifyRoundTrip(expectedCsdl, csdl);
        }

        [TestMethod]
        public void RoundTripTestVocabularyAnnotationValidDefaultDurationConstantAttributeCsdl()
        {
            var csdl = ConstantExpressionModelBuilder.VocabularyAnnotationValidDefaultDurationConstantAttributeCsdl();
            VerifyRoundTrip(csdl);
        }

        [TestMethod]
        public void RoundTripTestVocabularyAnnotationValidDurationConstantAttributeCsdl()
        {
            var csdl = ConstantExpressionModelBuilder.VocabularyAnnotationValidDurationConstantAttributeCsdl();
            VerifyRoundTrip(csdl);
        }

        [TestMethod]
        public void RoundTripTestVocabularyAnnotationInvalidValueDurationConstantAttributeCsdl()
        {
            var expectedCsdl = ConstantExpressionModelBuilder.VocabularyAnnotationValidDefaultDurationConstantAttributeCsdl();
            var csdl = ConstantExpressionModelBuilder.VocabularyAnnotationInvalidValueDurationConstantAttributeCsdl();
            VerifyRoundTrip(expectedCsdl, csdl);
        }

        [TestMethod]
        public void RoundTripTestVocabularyAnnotationInvalidFormatDurationConstantAttributeCsdl()
        {
            var expectedCsdl = ConstantExpressionModelBuilder.VocabularyAnnotationValidDefaultDurationConstantAttributeCsdl();
            var csdl = ConstantExpressionModelBuilder.VocabularyAnnotationInvalidFormatDurationConstantAttributeCsdl();
            VerifyRoundTrip(expectedCsdl, csdl);
        }

        [TestMethod]
        public void RoundTripTestVocabularyAnnotationValidDurationConstantAttributeModel()
        {
            var expectedCsdl = ConstantExpressionModelBuilder.VocabularyAnnotationValidDurationConstantAttributeCsdl();
            var csdl = this.GetSerializerResult(ConstantExpressionModelBuilder.VocabularyAnnotationValidDurationConstantModel()).Select(n => XElement.Parse(n));
            VerifyRoundTrip(expectedCsdl, csdl, true);
        }

        [TestMethod]
        public void RoundTripTestVocabularyAnnotationValidLargeDurationConstantModel()
        {
            var expectedCsdl = ConstantExpressionModelBuilder.VocabularyAnnotationValidLargeDurationConstantModelCsdl();
            var csdl = this.GetSerializerResult(ConstantExpressionModelBuilder.VocabularyAnnotationValidLargeDurationConstantModel()).Select(n => XElement.Parse(n));
            VerifyRoundTrip(expectedCsdl, csdl, true);
        }

        [TestMethod]
        public void RoundTripTestVocabularyAnnotationValidDefaultDurationConstantModel()
        {
            var expectedCsdl = ConstantExpressionModelBuilder.VocabularyAnnotationValidDefaultDurationConstantAttributeCsdl();
            var csdl = this.GetSerializerResult(ConstantExpressionModelBuilder.VocabularyAnnotationValidDefaultDurationConstantModel()).Select(n => XElement.Parse(n));
            VerifyRoundTrip(expectedCsdl, csdl, true);
        }

        [TestMethod]
        public void RoundTripTestVocabularyAnnotationInvalidTypeReferenceDurationConstantModel()
        {
            var expectedCsdl = ConstantExpressionModelBuilder.VocabularyAnnotationValidLargeDurationConstantModelCsdl();
            var csdl = this.GetSerializerResult(ConstantExpressionModelBuilder.VocabularyAnnotationInvalidTypeReferenceDurationConstantModel()).Select(n => XElement.Parse(n));
            VerifyRoundTrip(expectedCsdl, csdl, true);
        }
        #endregion

        #region Guid
        [TestMethod]
        public void RoundTripTestVocabularyAnnotationInvalidGuidConstantExpressionCsdl()
        {
            var expectedCsdl = ConstantExpressionModelBuilder.VocabularyAnnotationValidDefaultDurationConstantAttributeCsdl();
            var csdl = ConstantExpressionModelBuilder.VocabularyAnnotationValidDefaultDurationConstantExpressionCsdl();
            VerifyRoundTrip(expectedCsdl, csdl);
        }
        #endregion

        private void VerifyRoundTrip(IEnumerable<XElement> expectedCsdl, IEnumerable<XElement> inputCsdl, bool validate)
        {
            var model = this.GetParserResult(inputCsdl);

            if (validate)
            {
                IEnumerable<EdmError> errors;
                model.Validate(Microsoft.OData.Edm.EdmConstants.EdmVersion4, out errors);
                Assert.AreEqual(0, errors.Count(), "Unexpected errors.");
            }

            var actualCsdl = this.GetSerializerResult(model).Select(n => XElement.Parse(n));
            var updatedActual = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdl.ToArray(), EdmVersion.V40);
            var updatedExpected = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdl.ToArray(), EdmVersion.V40);
            new SerializerResultVerifierUsingXml().Verify(updatedExpected, updatedActual);
        }

        private void VerifyRoundTrip(IEnumerable<XElement> expectedCsdl, IEnumerable<XElement> inputCsdl)
        {
            this.VerifyRoundTrip(expectedCsdl, inputCsdl, false);
        }

        private void VerifyRoundTrip(IEnumerable<XElement> expectedCsdl)
        {
            this.VerifyRoundTrip(expectedCsdl, expectedCsdl, true);
        }
    }
}
