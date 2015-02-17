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
#if SILVERLIGHT
    using Microsoft.Silverlight.Testing;
#endif
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConstantExpressionRoundTripTests : EdmLibTestCaseBase
    {
        #region Duration
        [TestMethod]
        public void RoundTripTestValueAnnotationInvalidDurationConstantExpressionCsdl()
        {
            var expectedCsdl = ConstantExpressionModelBuilder.ValueAnnotationValidDefaultDurationConstantAttributeCsdl();
            var csdl = ConstantExpressionModelBuilder.ValueAnnotationInvalidDurationConstantExpressionCsdl();
            VerifyRoundTrip(expectedCsdl, csdl);
        }

        [TestMethod]
        public void RoundTripTestValueAnnotationInvalidDurationConstantAttributeCsdl()
        {
            var expectedCsdl = ConstantExpressionModelBuilder.ValueAnnotationValidDefaultDurationConstantAttributeCsdl();
            var csdl = ConstantExpressionModelBuilder.ValueAnnotationInvalidDurationConstantAttributeCsdl();
            VerifyRoundTrip(expectedCsdl, csdl);
        }

        [TestMethod]
        public void RoundTripTestValueAnnotationValidDefaultDurationConstantAttributeCsdl()
        {
            var csdl = ConstantExpressionModelBuilder.ValueAnnotationValidDefaultDurationConstantAttributeCsdl();
            VerifyRoundTrip(csdl);
        }

        [TestMethod]
        public void RoundTripTestValueAnnotationValidDurationConstantAttributeCsdl()
        {
            var csdl = ConstantExpressionModelBuilder.ValueAnnotationValidDurationConstantAttributeCsdl();
            VerifyRoundTrip(csdl);
        }

        [TestMethod]
        public void RoundTripTestValueAnnotationInvalidValueDurationConstantAttributeCsdl()
        {
            var expectedCsdl = ConstantExpressionModelBuilder.ValueAnnotationValidDefaultDurationConstantAttributeCsdl();
            var csdl = ConstantExpressionModelBuilder.ValueAnnotationInvalidValueDurationConstantAttributeCsdl();
            VerifyRoundTrip(expectedCsdl, csdl);
        }

        [TestMethod]
        public void RoundTripTestValueAnnotationInvalidFormatDurationConstantAttributeCsdl()
        {
            var expectedCsdl = ConstantExpressionModelBuilder.ValueAnnotationValidDefaultDurationConstantAttributeCsdl();
            var csdl = ConstantExpressionModelBuilder.ValueAnnotationInvalidFormatDurationConstantAttributeCsdl();
            VerifyRoundTrip(expectedCsdl, csdl);
        }

        [TestMethod]
        public void RoundTripTestValueAnnotationValidDurationConstantAttributeModel()
        {
            var expectedCsdl = ConstantExpressionModelBuilder.ValueAnnotationValidDurationConstantAttributeCsdl();
            var csdl = this.GetSerializerResult(ConstantExpressionModelBuilder.ValueAnnotationValidDurationConstantModel()).Select(n => XElement.Parse(n));
            VerifyRoundTrip(expectedCsdl, csdl, true);
        }

        [TestMethod]
        public void RoundTripTestValueAnnotationValidLargeDurationConstantModel()
        {
            var expectedCsdl = ConstantExpressionModelBuilder.ValueAnnotationValidLargeDurationConstantModelCsdl();
            var csdl = this.GetSerializerResult(ConstantExpressionModelBuilder.ValueAnnotationValidLargeDurationConstantModel()).Select(n => XElement.Parse(n));
            VerifyRoundTrip(expectedCsdl, csdl, true);
        }

        [TestMethod]
        public void RoundTripTestValueAnnotationValidDefaultDurationConstantModel()
        {
            var expectedCsdl = ConstantExpressionModelBuilder.ValueAnnotationValidDefaultDurationConstantAttributeCsdl();
            var csdl = this.GetSerializerResult(ConstantExpressionModelBuilder.ValueAnnotationValidDefaultDurationConstantModel()).Select(n => XElement.Parse(n));
            VerifyRoundTrip(expectedCsdl, csdl, true);
        }

        [TestMethod]
        public void RoundTripTestValueAnnotationInvalidTypeReferenceDurationConstantModel()
        {
            var expectedCsdl = ConstantExpressionModelBuilder.ValueAnnotationValidLargeDurationConstantModelCsdl();
            var csdl = this.GetSerializerResult(ConstantExpressionModelBuilder.ValueAnnotationInvalidTypeReferenceDurationConstantModel()).Select(n => XElement.Parse(n));
            VerifyRoundTrip(expectedCsdl, csdl, true);
        }
        #endregion

        #region Guid
        [TestMethod]
        public void RoundTripTestValueAnnotationInvalidGuidConstantExpressionCsdl()
        {
            var expectedCsdl = ConstantExpressionModelBuilder.ValueAnnotationValidDefaultDurationConstantAttributeCsdl();
            var csdl = ConstantExpressionModelBuilder.ValueAnnotationValidDefaultDurationConstantExpressionCsdl();
            VerifyRoundTrip(expectedCsdl, csdl);
        }
        #endregion

        private void VerifyRoundTrip(IEnumerable<XElement> expectedCsdl, IEnumerable<XElement> inputCsdl, bool validate)
        {
            var model = this.GetParserResult(inputCsdl);

            if (validate)
            {
                IEnumerable<EdmError> errors;
                model.Validate(Microsoft.OData.Edm.Library.EdmConstants.EdmVersion4, out errors);
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
