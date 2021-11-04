//---------------------------------------------------------------------
// <copyright file="ExpressionRoundTripTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ExpressionRoundTripTests : EdmLibTestCaseBase
    {
        [TestMethod]
        public void RoundTripInvalidTypeUsingCastCollectionCsdl()
        {
            this.RoundTripValidator(ExpressionValidationTestModelBuilder.InvalidTypeUsingCastCollectionCsdl(EdmVersion.V40), EdmVersion.V40);
        }

        [TestMethod]
        public void RoundTripCastNullableToNonNullableCsdl()
        {
            this.RoundTripValidator(ExpressionValidationTestModelBuilder.CastNullableToNonNullableCsdl(EdmVersion.V40), EdmVersion.V40);
        }

        [TestMethod]
        public void RoundTripCastNullableToNonNullableOnInlineAnnotationCsdl()
        {
            this.RoundTripValidator(ExpressionValidationTestModelBuilder.CastNullableToNonNullableOnInlineAnnotationCsdl(EdmVersion.V40), EdmVersion.V40);
        }

        [TestMethod]
        public void RoundTripCastResultFalseEvaluationCsdl()
        {
            this.RoundTripValidator(ExpressionValidationTestModelBuilder.CastResultFalseEvaluationCsdl(EdmVersion.V40), EdmVersion.V40);
        }

        [TestMethod]
        public void RoundTripCastResultTrueEvaluationCsdl()
        {
            this.RoundTripValidator(ExpressionValidationTestModelBuilder.CastResultTrueEvaluationCsdl(EdmVersion.V40), EdmVersion.V40);
        }

        [TestMethod]
        public void RoundTripInvalidPropertyTypeUsingIsTypeOnOutOfLineAnnotationCsdl()
        {
            this.RoundTripValidator(ExpressionValidationTestModelBuilder.InvalidPropertyTypeUsingIsTypeOnOutOfLineAnnotationCsdl(EdmVersion.V40), EdmVersion.V40);
        }

        [TestMethod]
        public void RoundTripInvalidPropertyTypeUsingIsTypeOnInlineAnnotationCsdl()
        {
            this.RoundTripValidator(ExpressionValidationTestModelBuilder.InvalidPropertyTypeUsingIsTypeOnInlineAnnotationCsdl(EdmVersion.V40), EdmVersion.V40);
        }

        [TestMethod]
        public void RoundTripIsTypeResultFalseEvaluationCsdl()
        {
            this.RoundTripValidator(ExpressionValidationTestModelBuilder.IsTypeResultFalseEvaluationCsdl(EdmVersion.V40), EdmVersion.V40);
        }

        [TestMethod]
        public void RoundTripIsTypeResultTrueEvaluationCsdl()
        {
            this.RoundTripValidator(ExpressionValidationTestModelBuilder.IsTypeResultTrueEvaluationCsdl(EdmVersion.V40), EdmVersion.V40);
        }

        private void RoundTripValidator(IEnumerable<XElement> csdls, EdmVersion version)
        {
            var model = this.GetParserResult(csdls);
            var roundTripCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));
            CsdlXElementComparer(csdls, roundTripCsdls, version);
        }

        private void CsdlXElementComparer(IEnumerable<XElement> expectedCsdls, IEnumerable<XElement> actualCsdls, EdmVersion version)
        {
            var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), version);
            var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), version);

            new ConstructiveApiCsdlXElementComparer().Compare(updatedExpectedCsdls.ToList(), updatedActualCsdls.ToList());
        }
    }
}
