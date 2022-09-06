//---------------------------------------------------------------------
// <copyright file="VocabularyRoundTripTests.cs" company="Microsoft">
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
    public class VocabularyRoundTripTests : EdmLibTestCaseBase
    {
        [TestMethod]
        public void RoundTripSimpleVocabularyAnnotationWithComplexTypeCsdl()
        {
            this.RoundTripValidator(VocabularyTestModelBuilder.SimpleVocabularyAnnotationWithComplexTypeCsdl(), EdmVersion.V40);
        }

        [TestMethod]
        public void RoundTripVocabularyAnnotationComplexTypeWithNullValuesCsdl()
        {
            this.RoundTripValidator(VocabularyTestModelBuilder.VocabularyAnnotationComplexTypeWithNullValuesCsdl(), EdmVersion.V40);
        }

        [TestMethod]
        public void RoundTripVocabularyAnnotationComplexTypeWithFewerPropertiesCsdl()
        {
            this.RoundTripValidator(VocabularyTestModelBuilder.VocabularyAnnotationComplexTypeWithFewerPropertiesCsdl(), EdmVersion.V40);
        }

        [TestMethod]
        public void RoundTripVocabularyAnnotationWithCollectionComplexTypeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(VocabularyTestModelBuilder.VocabularyAnnotationWithCollectionComplexTypeCsdl(), EdmVersion.V40, expectedErrors);

            this.RoundTripValidator(VocabularyTestModelBuilder.VocabularyAnnotationWithCollectionComplexTypeCsdl(), EdmVersion.V40);
        }

        //[TestMethod, Variation(Id = 60, SkipReason = @"[EdmLib] NullReferenceException occurs when doing a round trip on vocabulary annotation targetting an ambiguous function/ function import/ function parameter -- postponed")]
        public void RoundTripOutOfLineAnnotationAmbiguousFunction()
        {
            var csdls = VocabularyTestModelBuilder.OutOfLineAnnotationAmbiguousFunction();
            var model = this.GetParserResult(csdls);

            IEnumerable<EdmError> errors;
            model.Validate(out errors);
            Assert.IsTrue(errors.Count() > 0, "Invalid error count.");
            Assert.AreEqual(2, model.VocabularyAnnotations.Count(), "Invalid annotation count.");

            var annotationTarget = model.VocabularyAnnotations.First().Target;
            Assert.IsTrue(annotationTarget.ToString().Contains("AmbiguousFunctionBinding"), "Invalid target");

            annotationTarget = model.VocabularyAnnotations.ElementAt(1).Target;
            Assert.IsTrue(annotationTarget.ToString().Contains("AmbiguousFunctionBinding"), "Invalid target");

            var roundTripCsdl = this.GetSerializerResult(model, EdmVersion.V40, out errors);
            Assert.IsTrue(errors.Count() > 0, "Invalid error count.");
            // Add assert check for EdmErrorCode
        }

        //[TestMethod, Variation(Id = 70, SkipReason = @"[EdmLib] NullReferenceException occurs when doing a round trip on vocabulary annotation targetting an ambiguous function/ function import/ function parameter -- postponed")]
        public void RoundTripOutOfLineAnnotationAmbiguousFunctionImport()
        {
            var csdls = VocabularyTestModelBuilder.OutOfLineAnnotationAmbiguousFunctionImport();
            var model = this.GetParserResult(csdls);

            IEnumerable<EdmError> errors;
            model.Validate(out errors);
            Assert.IsTrue(errors.Count() > 0, "Invalid error count.");

            var annotationTarget = model.VocabularyAnnotations.First().Target;
            Assert.IsTrue(annotationTarget.ToString().Contains("AmbiguousFunctionImportBinding"), "Invalid target");

            annotationTarget = model.VocabularyAnnotations.ElementAt(1).Target;
            Assert.IsTrue(annotationTarget.ToString().Contains("AmbiguousFunctionImportBinding"), "Invalid target");

            var roundTripCsdl = this.GetSerializerResult(model, EdmVersion.V40, out errors);
            Assert.IsTrue(errors.Count() > 0, "Invalid error count.");
            // Add assert check for EdmErrorCode
        }

        //[TestMethod, Variation(Id = 80, SkipReason = @"[EdmLib] NullReferenceException occurs when doing a round trip on vocabulary annotation targetting an ambiguous function/ function import/ function parameter -- postponed")]
        public void RoundTripOutOfLineAnnotationAmbiguousFunctionImportParameter()
        {
            var csdls = VocabularyTestModelBuilder.OutOfLineAnnotationAmbiguousFunctionImportParameter();
            var model = this.GetParserResult(csdls);

            IEnumerable<EdmError> errors;
            model.Validate(out errors);
            Assert.IsTrue(errors.Count() > 0, "Invalid error count.");

            var roundTripCsdl = this.GetSerializerResult(model, EdmVersion.V40, out errors);
            Assert.IsTrue(errors.Count() > 0, "Invalid error count.");
            // Add assert check for EdmErrorCode
        }

        [TestMethod]
        public void RoundTripInlineAnnotationAmbiguousFunction()
        {
            var csdls = VocabularyTestModelBuilder.InlineAnnotationAmbiguousFunction();
            var model = this.GetParserResult(csdls);

            IEnumerable<EdmError> errors;
            model.Validate(out errors);
            Assert.AreEqual(2, errors.Count(), "Invalid error count.");
            Assert.AreEqual(EdmErrorCode.DuplicateFunctions, errors.First().ErrorCode, "Invalid error code.");
            Assert.AreEqual(EdmErrorCode.DuplicateFunctions, errors.ToArray()[1].ErrorCode, "Invalid error code.");

            var roundTripCsdl = this.GetSerializerResult(model, EdmVersion.V40, out errors).Select(n => XElement.Parse(n));
            Assert.AreEqual(0, errors.Count(), "Invalid error count.");
            new ConstructiveApiCsdlXElementComparer().Compare(csdls.ToList(), roundTripCsdl.ToList());
        }


        [TestMethod]
        public void RoundTripOutOfLineAnnotationUnresolvedOperationImportAndParameterTarget()
        {
            var csdls = VocabularyTestModelBuilder.OutOfLineAnnotationUnresolvedFunctionImportAndParameterTarget();
            var model = this.GetParserResult(csdls);

            IEnumerable<EdmError> errors;
            model.Validate(out errors);

            Assert.AreEqual(1, errors.Count(), "Invalid error count.");
            Assert.AreEqual(EdmErrorCode.BadUnresolvedParameter, errors.First().ErrorCode, "Expecting BadUnresolvedFunctionParameter error");
            this.GetSerializerResult(model, EdmVersion.V40, out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid error count.");
        }

        [TestMethod]
        public void RoundTripOutOfLineAnnotationUnresolvedFunctionImportTarget()
        {
            var csdls = VocabularyTestModelBuilder.OutOfLineAnnotationUnresolvedFunctionImportTarget();
            var model = this.GetParserResult(csdls);

            IEnumerable<EdmError> errors;
            model.Validate(out errors);

            Assert.AreEqual(1, errors.Count(), "Invalid error count.");
            Assert.AreEqual(EdmErrorCode.BadUnresolvedProperty, errors.First().ErrorCode, "Expecting BadUnresolvedProperty error");

            this.GetSerializerResult(model, EdmVersion.V40, out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid error count.");
        }

        [TestMethod]
        public void RoundTripOutOfLineAnnotationResolvedFunctionImportAndParameterTarget()
        {
            var csdls = VocabularyTestModelBuilder.OutOfLineAnnotationResolvedFunctionImportAndParameterTarget();
            var model = this.GetParserResult(csdls);

            IEnumerable<EdmError> errors;
            model.Validate(out errors);

            Assert.AreEqual(0, errors.Count(), "Invalid error count.");

            this.GetSerializerResult(model, EdmVersion.V40, out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid error count.");
        }

        [TestMethod]
        public void RoundTripOutOfLineAnnotationResolvedFunctionImportTarget()
        {
            var csdls = VocabularyTestModelBuilder.OutOfLineAnnotationResolvedFunctionImportTarget();
            var model = this.GetParserResult(csdls);

            IEnumerable<EdmError> errors;
            model.Validate(out errors);

            Assert.AreEqual(0, errors.Count(), "Invalid error count.");

            this.GetSerializerResult(model, EdmVersion.V40, out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid error count.");
        }

        [TestMethod]
        public void RoundTripInLineAnnotationEnumMember()
        {
            var csdls = VocabularyTestModelBuilder.inlineAnnotationEnumMember();
            var model = this.GetParserResult(csdls);

            IEnumerable<EdmError> errors;
            model.Validate(out errors);

            Assert.AreEqual(0, errors.Count(), "Invalid error count.");

            var serializedCsdls = this.GetSerializerResult(model, EdmVersion.V40, out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid error count.");
            new ConstructiveApiCsdlXElementComparer().Compare(serializedCsdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo)).ToList(), 
                csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo)).ToList());
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
