//---------------------------------------------------------------------
// <copyright file="EdmxRoundTripTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EdmxRoundTripTests : EdmLibTestCaseBase
    {
        [TestMethod]
        public void EdmxRoundTripTestsInvalidNamespaceAndVersionEdmx()
        {
            var errors = new ExpectedEdmErrors()
            {
                { EdmErrorCode.InvalidVersionNumber, "EdmxParser_EdmxVersionMismatch" }
            };

            this.EdmxCheckForErrors(EdmxModelBuilder.NonMatchingNamespaceAndVersionEdmx(), errors);
        }

        [TestMethod]
        public void EdmxRoundTripTestsInvalidConceptualModelsEdmx()
        {
            var errors = new ExpectedEdmErrors()
            {
                { EdmErrorCode.UnexpectedXmlElement, "XmlParser_UnexpectedRootElementNoNamespace" }
            };

            this.EdmxCheckForErrors(EdmxModelBuilder.InvalidConceptualModelsEdmx(), errors);
        }

        [TestMethod]
        public void EdmxRoundTripTestsInvalidConceptualModelNamespaceEdmx()
        {
            var errors = new ExpectedEdmErrors()
            {
                { EdmErrorCode.UnexpectedXmlElement, "XmlParser_UnexpectedRootElementWrongNamespace" }
            };

            this.EdmxCheckForErrors(EdmxModelBuilder.InvalidConceptualModelNamespaceEdmx(), errors);
        }

        [TestMethod]
        public void EdmxRoundTripTestsInvalidDataServicesEdmx()
        {
            var errors = new ExpectedEdmErrors()
            {
                { EdmErrorCode.UnexpectedXmlElement, "XmlParser_UnexpectedRootElementNoNamespace" }
            };

            this.EdmxCheckForErrors(EdmxModelBuilder.InvalidDataServicesEdmx(), errors);
        }

        [TestMethod]
        public void EdmxRoundTripTwoDataServicesEdmx()
        {
            var errors = new ExpectedEdmErrors()
            {
                { EdmErrorCode.UnexpectedXmlElement, "EdmxParser_BodyElement" }
            };

            this.EdmxCheckForErrors(EdmxModelBuilder.TwoDataServicesEdmx(), errors);
        }

        [TestMethod]
        public void EdmxRoundTripTwoRuntimeEdmx()
        {
            var errors = new ExpectedEdmErrors()
            {
                { EdmErrorCode.UnexpectedXmlElement, "EdmxParser_BodyElement" }
            };

            this.EdmxCheckForErrors(EdmxModelBuilder.TwoRuntimeEdmx(), errors);
        }

        [TestMethod]
        public void EdmxRoundTripTestsEmptyConceptualModelsEdmx()
        {
            this.EdmxRoundTripCheck(EdmxModelBuilder.EmptyConceptualModelsEdmx(), CsdlTarget.EntityFramework);
        }

        [TestMethod]
        public void EdmxRoundTripTestsEmptyConceptualModelsModel()
        {
            var model = new EdmModel();
            model.SetEdmxVersion(EdmConstants.EdmVersion4);
            this.EdmxRoundTripCheck(EdmxModelBuilder.EmptyConceptualModelsEdmx(), CsdlTarget.EntityFramework, model);
        }

        [TestMethod]
        public void EdmxRoundTripTestsSimpleConceptualModelsEdmx()
        {
            this.EdmxRoundTripCheck(EdmxModelBuilder.SimpleConceptualModelsEdmx(), CsdlTarget.EntityFramework);
        }

        [TestMethod]
        public void EdmxRoundTripTestsSimpleConceptualoMdelsModel()
        {
            var model = EdmxModelBuilder.SimpleEdmx();
            this.EdmxRoundTripCheck(EdmxModelBuilder.SimpleConceptualModelsEdmx(), CsdlTarget.EntityFramework, model);
        }

        [TestMethod]
        public void EdmxRoundTripTestsEmptyDataServicesEdmx()
        {
            this.EdmxRoundTripCheck(EdmxModelBuilder.EmptyDataServicesEdmx(), CsdlTarget.OData);
        }

        [TestMethod]
        public void EdmxRoundTripTestsEmptyDataServicesModel()
        {
            var model = new EdmModel();
            model.SetEdmxVersion(EdmConstants.EdmVersion4);
            this.EdmxRoundTripCheck(EdmxModelBuilder.EmptyDataServicesEdmx(), CsdlTarget.OData, model);
        }

        [TestMethod]
        public void EdmxRoundTripTestsEmptyDataServicesWithOtherAttributesEdmx()
        {
            this.EdmxRoundTripCheck(EdmxModelBuilder.EmptyDataServicesEdmx(), CsdlTarget.OData, this.GetEdmxParserResult(EdmxModelBuilder.EmptyDataServicesWithOtherAttributesEdmx()));
        }

        [TestMethod]
        public void EdmxRoundTripTestsSimpleDataServicesEdmx()
        {
            this.EdmxRoundTripCheck(EdmxModelBuilder.SimpleDataServicesEdmx(), CsdlTarget.OData);
        }

        [TestMethod]
        public void EdmxRoundTripTestsSimpleDataServicesModel()
        {
            var model = EdmxModelBuilder.SimpleEdmx();
            this.EdmxRoundTripCheck(EdmxModelBuilder.SimpleDataServicesEdmx(), CsdlTarget.OData, model);
        }

        [TestMethod]
        public void EdmxRoundTripTestsSimpleDataServicesWithVersionAttributesModelToConceptualModels()
        {
            var model = EdmxModelBuilder.SimpleEdmx();
            this.EdmxRoundTripCheck(EdmxModelBuilder.SimpleConceptualModelsEdmx(), CsdlTarget.EntityFramework, model);
        }

        [TestMethod]
        public void EdmxRoundTripTestsConceptualModelsToDataServices()
        {
            var model = this.GetEdmxParserResult(EdmxModelBuilder.SimpleConceptualModelsEdmx());
            this.EdmxRoundTripCheck(EdmxModelBuilder.SimpleDataServicesEdmx(), CsdlTarget.OData, model);
        }

        [TestMethod]
        public void EdmxRoundTripTestsDataServicesToConceptualModels()
        {
            var model = this.GetEdmxParserResult(EdmxModelBuilder.SimpleDataServicesEdmx());
            this.EdmxRoundTripCheck(EdmxModelBuilder.SimpleConceptualModelsEdmx(), CsdlTarget.EntityFramework, model);
        }

        [TestMethod]
        public void EdmxRoundTripTestsUsingAliasEdmx()
        {
            var model = this.GetEdmxParserResult(EdmxModelBuilder.UsingAliasEdmx());
            this.EdmxRoundTripCheck(EdmxModelBuilder.UsingAliasEdmx(), CsdlTarget.EntityFramework, model);
        }

        #region referenced model - Include, IncludeAnnotations

        [TestMethod]
        public void EdmxRoundTripTestsUsingAliasInReferencedEdmx()
        {
            string mainEdmx;
            string referencedEdmx1;
            string referencedEdmx2;
            IEdmModel mainModel;
            EdmxModelBuilder.GetReferencedModelEdmx(out mainModel, out mainEdmx, out referencedEdmx1, out referencedEdmx2);
            IEnumerable<EdmError> errors;
            bool valid = mainModel.Validate(out errors);
            Assert.IsTrue(valid);
            string actualEdmx = GetEdmx(mainModel, CsdlTarget.OData);

            // after deserialization & serialization, the alias'ed 'CT.Customer' becomes qualified name 'NS1.Customer',
            // so make some adjustments for verification: 
            actualEdmx = actualEdmx.Replace("EntityType=\"NS1.Customer\"", "EntityType=\"CT.Customer\"");
            actualEdmx = actualEdmx.Replace("EntityType=\"NS.Ref1.VipCustomer\"", "EntityType=\"VPCT.VipCustomer\"");
            actualEdmx = actualEdmx.Replace("EntityType=\"NS.Ref2.VipCard\"", "EntityType=\"VPCD.VipCard\"");
            valid = XElement.DeepEquals(XElement.Parse(mainEdmx), XElement.Parse(actualEdmx));
            Assert.IsTrue(valid, "Invalid actual edmx.");
        }

        [TestMethod]
        public void EdmxRoundTripTests_EdmxReferences()
        {
            string mainEdmx;
            string referencedEdmx1;
            string referencedEdmx2;
            IEdmModel mainModel;
            EdmxModelBuilder.GetReferencedModelEdmx(out mainModel, out mainEdmx, out referencedEdmx1, out referencedEdmx2);
            IEnumerable<EdmError> errors;
            bool valid = mainModel.Validate(out errors);
            Assert.IsTrue(valid);

            // verify reading edmx:Reference
            List<IEdmReference> references = mainModel.GetEdmReferences().ToList();
            Assert.AreEqual(2, references.Count);
            Assert.AreEqual("VPCT", references[0].Includes.First().Alias);
            Assert.AreEqual("NS.Ref1", references[0].Includes.First().Namespace);

            // verify Uri in EdmReference
            string uriString = "http://addedUrl/addedEdm.xml";
            EdmReference newReference = new EdmReference(new Uri(uriString));
            Assert.AreEqual(uriString, EdmValueWriter.UriAsXml(newReference.Uri));

            // verify writing edmx:Reference
            // add a new <edmx:reference>
            newReference.AddInclude(new EdmInclude("adhoc_Alias", "adhoc_Namespace"));
            List<IEdmReference> newReferences = new List<IEdmReference>();
            newReferences.AddRange(references);
            newReferences.Add(newReference);
            mainModel.SetEdmReferences(newReferences);
            string actualEdmx = GetEdmx(mainModel, CsdlTarget.OData);

            // add new Include to verify: Namespace=""adhoc_Namespace"" Alias=""adhoc_Alias""
            mainEdmx = mainEdmx.Replace("  <edmx:DataServices>",
             @"  <edmx:Reference Uri=""http://addedUrl/addedEdm.xml"">
    <edmx:Include Namespace=""adhoc_Namespace"" Alias=""adhoc_Alias"" />
  </edmx:Reference>
  <edmx:DataServices>");

            // after deserialization & serialization, the alias'ed 'CT.Customer' becomes qualified name 'NS1.Customer',
            // so make some adjustments for verification: 
            actualEdmx = actualEdmx.Replace("EntityType=\"NS1.Customer\"", "EntityType=\"CT.Customer\"");
            actualEdmx = actualEdmx.Replace("EntityType=\"NS.Ref1.VipCustomer\"", "EntityType=\"VPCT.VipCustomer\"");
            actualEdmx = actualEdmx.Replace("EntityType=\"NS.Ref2.VipCard\"", "EntityType=\"VPCD.VipCard\"");
            valid = XElement.DeepEquals(XElement.Parse(mainEdmx), XElement.Parse(actualEdmx));
            Assert.IsTrue(valid, "Invalid actual edmx.");
        }
        #endregion

        private void EdmxRoundTripCheck(string edmx, CsdlTarget target)
        {
            IEdmModel model = this.GetEdmxParserResult(edmx);

            this.EdmxRoundTripCheck(edmx, target, model);
        }

        private string GetEdmx(IEdmModel model, CsdlTarget target)
        {
            string edmx = string.Empty;

            using (StringWriter sw = new StringWriter())
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = System.Text.Encoding.UTF8;

                using (XmlWriter xw = XmlWriter.Create(sw, settings))
                {
                    IEnumerable<EdmError> errors;
                    CsdlWriter.TryWriteCsdl(model, xw, target, out errors);
                    xw.Flush();
                }

                edmx = sw.ToString();
            }

            return edmx;
        }

        private void EdmxCheckForErrors(string edmx, IEnumerable<ExpectedEdmError> expectedErrors)
        {
            IEnumerable<EdmError> actualErrors;
            IEdmModel model;
            bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out model, out actualErrors);

            Assert.IsFalse(parsed, "Invalid edmx parsing.");
            ValidationVerificationHelper.Verify(expectedErrors, actualErrors);
        }

        private void EdmxRoundTripCheck(string expectedEdmx, CsdlTarget target, IEdmModel model)
        {
            var errors = new EdmLibTestErrors();
            this.VerifySemanticValidation(model, Microsoft.Test.OData.Utils.Metadata.EdmVersion.V40, errors);

            string actualEdmx = GetEdmx(model, target);
            var valid = XElement.DeepEquals(XElement.Parse(expectedEdmx), XElement.Parse(actualEdmx));
            Assert.IsTrue(valid, "Invalid actual edmx.");
        }
    }
}
