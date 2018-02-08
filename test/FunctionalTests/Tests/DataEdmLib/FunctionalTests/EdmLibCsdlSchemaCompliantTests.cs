//---------------------------------------------------------------------
// <copyright file="EdmLibCsdlSchemaCompliantTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if !WINDOWS_PHONE && !SILVERLIGHT
namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using EdmConstants = Microsoft.Test.OData.Utils.Metadata.EdmConstants;

    [TestClass]
    public class EdmLibCsdlSchemaCompliantTests : EdmLibTestCaseBase
    {
        [TestMethod]
        // [EdmLib] Vocabulary annotation within vocabulary annotation can not be parsed by csdlreader but xsd does not provide parsing error - postponed
        public void ParserVocabularyModelsCsdlSchemaCompliantTests()
        {
            this.BasicXsdValidationTestForParserInputCsdl(typeof(VocabularyTestModelBuilder));
        }

        [TestMethod]
        public void ParserComplexTypeWithBaseTypeAbstractCsdlSchemaCompliantTest()
        {
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            foreach (var edmVersion in edmVersions)
            {
                this.BasicXsdValidationTestForParserInputCsdl(ModelBuilder.ComplexTypeWithBaseType(edmVersion), edmVersion);
                this.BasicXsdValidationTestForParserInputCsdl(ValidationTestModelBuilder.ComplexTypeBaseTypeSupportInV11(edmVersion), edmVersion);
                this.BasicXsdValidationTestForParserInputCsdl(ValidationTestModelBuilder.EdmComplexTypeInvalidIsPolymorphic(edmVersion), edmVersion);
                this.BasicXsdValidationTestForParserInputCsdl(ValidationTestModelBuilder.ComplexTypeIsAbstractSupportInV40(edmVersion), edmVersion);
            }
        }

        [TestMethod]
        public void ParserSimpleIdentifierTypeReferenceCsdlSchemaCompliantTest()
        {
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            foreach (var edmVersion in edmVersions)
            {
                this.BasicXsdValidationTestForParserInputCsdl(ValidationTestModelBuilder.CollectionTypeTypeRefSimpleTypeCanHaveFacets(edmVersion), edmVersion);
            }
        }

        [TestMethod]
        public void ParserOpenTypeSupportCsdlSchemaCompliantTest()
        {
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            foreach (var edmVersion in edmVersions)
            {
                this.BasicXsdValidationTestForParserInputCsdl(ValidationTestModelBuilder.OpenTypeSupportInV40(edmVersion), edmVersion);
                this.BasicXsdValidationTestForParserInputCsdl(ODataTestModelBuilder.ODataTestModelWithOpenType, edmVersion);
            }
        }

        [TestMethod]
        public void ParserInvalidDecimalTypePrecisionCsdlSchemaCompliantTest()
        {
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            foreach (var edmVersion in edmVersions)
            {
                this.BasicXsdValidationTestForParserInputCsdl(ValidationTestModelBuilder.ValidDecimalTypePrecisionValue(edmVersion), edmVersion);
                this.BasicXsdValidationTestForParserInputCsdl(ValidationTestModelBuilder.ValidDateTimeOffsetTypePrecisionValue(edmVersion), edmVersion);
                this.BasicXsdValidationTestForParserInputCsdl(ValidationTestModelBuilder.ValidTimeTypePrecisionValue(edmVersion), edmVersion);
            }
        }

        [TestMethod]
        public void SerializerVocabularyTestModelsCsdlSchemaCompliantTests()
        {
            this.BasicXsdValidationTestForSerializerOutputCsdl(typeof(VocabularyTestModelBuilder));
        }

        [TestMethod]
        public void SerializerModelWithAssociationEndAsInaccessibleEntityTypeCsdlSchemaCompliantTest()
        {
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            foreach (var edmVersion in edmVersions)
            {
                this.BasicXsdValidationTestForSerializerOutputCsdl(ValidationTestModelBuilder.ModelWithAssociationEndAsInaccessibleEntityType(), edmVersion);
                this.BasicXsdValidationTestForSerializerOutputCsdl(ValidationTestModelBuilder.ModelWithInconsistentNavigationPropertyPartner(), edmVersion);
            }
        }

        [TestMethod]
        public void SerializerModelWithMismatchedBaseTypeCsdlSchemaCompliantTests()
        {
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            foreach (var edmVersion in edmVersions)
            {
                this.BasicXsdValidationTestForSerializerOutputCsdl(ValidationTestModelBuilder.ModelWithComplexTypeWithEntityBaseType(), edmVersion);
                this.BasicXsdValidationTestForParserInputCsdl(ValidationTestModelBuilder.ModelWithComplexTypeWithEntityBaseTypeCsdl(edmVersion), edmVersion);
                this.BasicXsdValidationTestForSerializerOutputCsdl(ValidationTestModelBuilder.ModelWithEntityTypeWithComplexBaseType(), edmVersion);
                this.BasicXsdValidationTestForParserInputCsdl(ValidationTestModelBuilder.ModelWithEntityTypeWithComplexBaseTypeCsdl(edmVersion), edmVersion);
                this.BasicXsdValidationTestForSerializerOutputCsdl(ValidationTestModelBuilder.ModelWithInvalidDependentProperties(), edmVersion);
            }
        }

        [TestMethod]
        public void SerializerOperationParameterShouldBeInboundCsdlSchemaCompliantTest()
        {
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            foreach (var edmVersion in edmVersions)
            {
                this.BasicXsdValidationTestForSerializerOutputCsdl(ValidationTestModelBuilder.OperationParameterShouldBeInbound(), edmVersion);
            }
        }

        [TestMethod]
        public void SerializerModelWithInvalidFunctionImportReturnTypeCsdlSchemaCompliantTest()
        {
            Console.WriteLine(DateTime.Now.ToShortDateString());
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            foreach (var edmVersion in edmVersions)
            {
                this.BasicXsdValidationTestForSerializerOutputCsdl(ValidationTestModelBuilder.ModelWithInvalidFunctionReturnType(), edmVersion);
            }
        }

        [TestMethod]
        public void SerializerModelWithRowTypePropertyOfTypeNoneCsdlSchemaCompliantTest()
        {
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            foreach (var edmVersion in edmVersions)
            {
                this.BasicXsdValidationTestForSerializerOutputCsdl(ValidationTestModelBuilder.ModelWithInvalidOperationReturnType(), edmVersion);
                this.BasicXsdValidationTestForSerializerOutputCsdl(ValidationTestModelBuilder.ModelWithInvalidOperationParameterType(), edmVersion);
            }
        }

        [TestMethod]
        public void ParserODataTestModelsCsdlSchemaCompliantTests()
        {
            this.BasicXsdValidationTestForParserInputCsdl(typeof(ODataTestModelBuilder));
        }

        [TestMethod]
        public void SerializerODataTestModelBuilderCsdlSchemaCompliantTests()
        {
            this.BasicXsdValidationTestForSerializerOutputCsdl(typeof(ODataTestModelBuilder));
        }

        [TestMethod]
        public void ParserModelWithEnumTerm()
        {
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            foreach (var edmVersion in edmVersions)
            {
                Assert.IsFalse(this.GetXsdValidationResults(ValidationTestModelBuilder.ModelWithEnumTerm(), edmVersion).Any(), "EnumType should be able to have inline vocab. annotations");
            }
        }

        [TestMethod]
        public void TermXsdValidation()
        {
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            foreach (var edmVersion in edmVersions)
            {
                this.BasicXsdValidationTestForSerializerOutputCsdl(this.GetParserResult(VocabularyTestModelBuilder.TermOnlyCsdl()), edmVersion);
            }
        }

        [TestMethod]
        public void ParserVocabularyAnnotationIfCsdlSchemaCompliantTest()
        {
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            foreach (var edmVersion in edmVersions)
            {
                this.BasicXsdValidationTestForParserInputCsdl(VocabularyTestModelBuilder.VocabularyAnnotationIfCsdl(), edmVersion);
            }
        }

        [TestMethod]
        public void ParserAnnotationFunctionCsdlSchemaCompliantTest()
        {
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            foreach (var edmVersion in edmVersions)
            {
                this.BasicXsdValidationTestForParserInputCsdl(VocabularyTestModelBuilder.VocabularyAnnotationFunctionCsdl(), edmVersion);
            }
        }

        [TestMethod]
        public void ParserSimpleAnnotationCsdlSchemaCompliantTest()
        {
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            foreach (var edmVersion in edmVersions)
            {
                this.BasicXsdValidationTestForParserInputCsdl(VocabularyTestModelBuilder.SimpleVocabularyAnnotationCsdl(), edmVersion);
            }
        }

        [TestMethod]
        public void ParserOutOfLineAnnotationEntityPropertyCsdlSchemaCompliantTest()
        {
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            foreach (var edmVersion in edmVersions)
            {
                this.BasicXsdValidationTestForParserInputCsdl(VocabularyTestModelBuilder.OutOfLineAnnotationEntityProperty(), edmVersion);
            }
        }

        [TestMethod]
        public void ParserInlineAnnotationNavigationPropertyCsdlSchemaCompliantTest()
        {
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            foreach (var edmVersion in edmVersions)
            {
                this.BasicXsdValidationTestForParserInputCsdl(VocabularyTestModelBuilder.InlineAnnotationNavigationProperty(), edmVersion);
            }
        }

        [TestMethod]
        public void ParserInlineAnnotationEntityContainerCsdlSchemaCompliantTest()
        {
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            foreach (var edmVersion in edmVersions)
            {
                this.BasicXsdValidationTestForParserInputCsdl(VocabularyTestModelBuilder.InlineAnnotationEntityContainer(), edmVersion);
            }
        }

        [TestMethod]
        public void ParserInlineAnnotationFunctionImportCsdlSchemaCompliantTest()
        {
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            foreach (var edmVersion in edmVersions)
            {
                this.BasicXsdValidationTestForParserInputCsdl(VocabularyTestModelBuilder.InlineAnnotationFunctionImport(), edmVersion);
            }
        }

        [TestMethod]
        public void ParserInlineAnnotationFunctionImportParameterCsdlSchemaCompliantTest()
        {
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            foreach (var edmVersion in edmVersions)
            {
                this.BasicXsdValidationTestForParserInputCsdl(VocabularyTestModelBuilder.InlineAnnotationFunctionImportParameter(), edmVersion);
            }
        }

        [TestMethod]
        public void ParserImmediateAnnotationRoundTripCsdlSchemaCompliantTest()
        {
            var testCsdls = ODataTestModelBuilder.ImmediateAnnotationRoundTrip;
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            foreach (var edmVersion in edmVersions)
            {
                this.BasicXsdValidationTestForParserInputCsdl(testCsdls, edmVersion);
            }
        }

        [TestMethod]
        public void ParserOutOfLineAnnotationOperationAndOperationParameterCsdlSchemaCompliantTest()
        {
            var testCsdls = new List<IEnumerable<XElement>> { VocabularyTestModelBuilder.OutOfLineAnnotationFunction(), VocabularyTestModelBuilder.OutOfLineAnnotationOperationParameter() };
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            foreach (var edmVersion in edmVersions)
            {
                testCsdls.ForEach((n) => this.BasicXsdValidationTestForParserInputCsdl(n, edmVersion));
            }
        }

        [TestMethod]
        public void ConstantExpressionModelBuilderBasicValidationConsistencyTest()
        {
            this.BasicValidationConsistencyTest(typeof(ConstantExpressionModelBuilder));
        }

        [TestMethod]
        public void EdmxModelBuilderBasicValidationConsistencyTest()
        {
            this.BasicValidationConsistencyTest(typeof(EdmxModelBuilder));
        }

        [TestMethod]
        public void FindMethodsTestModelBuilderBasicValidationConsistencyTest()
        {
            this.BasicValidationConsistencyTest(typeof(FindMethodsTestModelBuilder));
        }

        [TestMethod]
        public void NavigationTestModelBuilderBasicValidationConsistencyTest()
        {
            this.BasicValidationConsistencyTest(typeof(NavigationTestModelBuilder));
        }

        [TestMethod]
        public void ValidationTestModelBuilderBasicValidationConsistencyTest()
        {
            this.BasicValidationConsistencyTest(typeof(ValidationTestModelBuilder));
        }

        [TestMethod]
        public void VocabularyTestModelBuilderBasicValidationConsistencyTest()
        {
            this.BasicValidationConsistencyTest(typeof(VocabularyTestModelBuilder));
        }

        [TestMethod]
        public void XElementAnnotationModelBuilderBasicValidationConsistencyTest()
        {
            this.BasicValidationConsistencyTest(typeof(XElementAnnotationModelBuilder));
        }

        [TestMethod]
        public void ParserAnnotationTermCsdlSchemaCompliantTest()
        {
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            foreach (var edmVersion in edmVersions)
            {
                this.BasicXsdValidationTestForParserInputCsdl(VocabularyTestModelBuilder.OutOfLineAnnotationTerm(), edmVersion);
                this.BasicXsdValidationTestForParserInputCsdl(VocabularyTestModelBuilder.InlineAnnotationTerm(), edmVersion);
            }
        }

        [TestMethod]
        public void ParserAnnotationQualifiersWithNonSimpleValueCsdlSchemaCompliantTest()
        {
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            foreach (var edmVersion in edmVersions)
            {
                this.BasicXsdValidationTestForParserInputCsdl(VocabularyTestModelBuilder.AnnotationQualifiersWithNonSimpleValue(), edmVersion);
            }
        }

        private void BasicValidationConsistencyTest(Type modelBuilder)
        {
            var versions = new EdmVersion[] { EdmVersion.V40 };
            var modelExtractor = new EdmLibTestModelExtractor();

            foreach (var version in versions)
            {
                var testModels = new Dictionary<string, IEdmModel>();
                modelExtractor.GetModels<IEdmModel>(modelBuilder, version, new CustomConsistentValidationTestAttribute(), false).ToList().ForEach(n => testModels.Add(n.Key, n.Value));
                modelExtractor.GetModels<EdmModel>(modelBuilder, version, new CustomConsistentValidationTestAttribute(), false).ToList().ForEach(n => testModels.Add(n.Key, n.Value));

                foreach (var testModel in testModels)
                {
                    this.BasicValidationConsistencyTest(testModel.Value, version);
                }
            }
        }

        private void BasicValidationConsistencyTest(IEdmModel model, EdmVersion version)
        {
            IEnumerable<EdmError> initialErrors = null;
            model.Validate(toProductVersionlookup[version], out initialErrors);

            IEnumerable<EdmError> resultingErrors = null;
            IEnumerable<string> csdls = this.GetSerializerResult(model, version, out resultingErrors);

            if (!resultingErrors.Any())
            {
                var resultingModel = this.GetParserResult(csdls);
                resultingModel.Validate(toProductVersionlookup[version], out resultingErrors);
            }

            Assert.IsFalse(initialErrors.Count() < resultingErrors.Count(), "More errors are generated after round trip.");
        }

        private void BasicXsdValidationTestForSerializerOutputCsdl(Type modelBuilder)
        {
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            var modelExtractor = new EdmLibTestModelExtractor();
            foreach (var edmVersion in edmVersions)
            {
                var testModels = new Dictionary<string, IEdmModel>();
                // The Key of the testModels dictionary is for the functiona name that returns a specfic model. 
                // This is not directly used at runtime, but it can be used for debugging purpose. 
                modelExtractor.GetModels<IEdmModel>(modelBuilder, edmVersion, new CustomCsdlSchemaCompliantTestAttribute(), false).ToList().ForEach(n => testModels.Add(n.Key, n.Value));
                foreach (var testModel in testModels)
                {
                    this.BasicXsdValidationTestForSerializerOutputCsdl(testModel.Value, edmVersion);
                }
            }
        }

        private void BasicXsdValidationTestForSerializerOutputCsdl(IEdmModel edmModel, EdmVersion edmVersion)
        {
            IEnumerable<EdmError> valiationErrors = null;
            edmModel.Validate(toProductVersionlookup[edmVersion], out valiationErrors);
            IEnumerable<XElement> csdls = null;
            IEnumerable<EdmError> actualSerializationErrors;
            csdls = this.GetSerializerResult(edmModel, edmVersion, out actualSerializationErrors).Select(n => XElement.Parse(n)).ToArray();
            if (actualSerializationErrors.Count() == 0)
            {
                var xsdErrors = this.GetXsdValidationResults(csdls, edmVersion);
                if (valiationErrors.Count() == 0)
                {
                    Assert.AreEqual(xsdErrors.Count(), 0, "No symantic validation error should guarantee the XSD validation for the corresponding CSDLs.");
                }
                this.BasicXsdValidationTestForParserInputCsdl(csdls, edmVersion);
            }
        }

        private void BasicXsdValidationTestForParserInputCsdl(Type modelBuilder)
        {
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            var modelExtractor = new EdmLibTestModelExtractor();
            foreach (var edmVersion in edmVersions)
            {
                // The Key of the testModels dictionary is for the functiona name that returns a specfic model. 
                // This is not directly used at runtime, but it can be used for debugging purpose. 
                var testModels = new Dictionary<string, IEnumerable<XElement>>();

                // Extract test models in IEnumerable<XElement>
                modelExtractor.GetModels<IEnumerable<XElement>>(modelBuilder, edmVersion, new CustomCsdlSchemaCompliantTestAttribute(), false).ToList().ForEach(n => testModels.Add(n.Key, n.Value));
                // Extract test models in XElement[] 
                modelExtractor.GetModels<XElement[]>(modelBuilder, edmVersion, new CustomCsdlSchemaCompliantTestAttribute(), false).ToList().ForEach(n => testModels.Add(n.Key, n.Value));
                // Extract test models in IEdmModel
                modelExtractor.GetModels<IEdmModel>(modelBuilder, edmVersion, new CustomCsdlSchemaCompliantTestAttribute(), false).ToList().ForEach(n => testModels.Add(n.Key, this.GetSerializerResult(n.Value).Select(XElement.Parse)));
                foreach (var testModel in testModels)
                {
                    this.BasicXsdValidationTestForParserInputCsdl(testModel.Value, edmVersion);
                }
            }
        }

        private void BasicXsdValidationTestForParserInputCsdl(IEnumerable<XElement> csdls, EdmVersion edmVersion)
        {
            IEdmModel edmModel;
            IEnumerable<EdmError> parserErrors;
            var csdlsEdmVersionUpdated = csdls.Select(n => XElement.Parse(n.ToString().Replace(n.Name.NamespaceName, EdmLibCsdlContentGenerator.GetCsdlFullNamespace(edmVersion).NamespaceName)));

            var isParsed = SchemaReader.TryParse(csdlsEdmVersionUpdated.Select(e => e.CreateReader()), out edmModel, out parserErrors);
            if (!isParsed)
            {
                // XSD verification is a sufficent condition, but not a necessary condition of EDMLib parser. 
                Assert.IsTrue(this.GetXsdValidationResults(csdlsEdmVersionUpdated, edmVersion).Count() > 0, "It should be invalid for the CSDL {0} XSDs", edmVersion);
            }
            else
            {
                IEnumerable<EdmError> validationErrors;
                edmModel.Validate(toProductVersionlookup[edmVersion], out validationErrors);
                var xsdValidationErrors = new List<string>();

                xsdValidationErrors.AddRange(this.GetXsdValidationResults(csdlsEdmVersionUpdated, edmVersion));

                if (!validationErrors.Any() && xsdValidationErrors.Any())
                {
                    Assert.Fail("XSD validation must succeed when there is no semantic validation error.");
                }
                else if (validationErrors.Any() && !xsdValidationErrors.Any())
                {
                    foreach (var validationError in validationErrors)
                    {
                        Console.WriteLine("The test data has no XSD error, but it has a validation error : {0}", validationError.ErrorMessage);
                    }
                }
            }
        }

        private IEnumerable<string> GetXsdValidationResults(IEnumerable<XElement> csdls, EdmVersion edmVersion)
        {
            var errorMessages = new List<string>();
            foreach (var csdl in csdls)
            {
                errorMessages.AddRange(GetXsdValidationResults(csdl, edmVersion));
            }
            return errorMessages;
        }

        private IEnumerable<string> GetXsdValidationResults(XElement csdlElement, EdmVersion edmVersion)
        {
            var csdlEdmVersionUpdated = XElement.Parse(csdlElement.ToString().Replace(csdlElement.Name.NamespaceName, EdmLibCsdlContentGenerator.GetCsdlFullNamespace(edmVersion).NamespaceName));
            var errorMessages = new List<string>();
            InitializeEdmLibCsdlSchemas();
            new XDocument(csdlEdmVersionUpdated).Validate(EdmLibXmlSchemas[edmVersion], (o, e) => { errorMessages.Add(e.Message); });
            return errorMessages;
        }

        private Stream GetXsdStream(string xsdFileName)
        {
            return this.GetType().Assembly.GetManifestResourceStream(string.Format("{0}.{1}", "EdmLibTests.Resources", xsdFileName));
        }

        private static Dictionary<EdmVersion, XmlSchemaSet> EdmLibXmlSchemas
        {
            get;
            set;
        }

        private void InitializeEdmLibCsdlSchemas()
        {
            if (EdmLibXmlSchemas != null)
            {
                return;
            }

            var codegenerationTargetNamespace = EdmConstants.CodegenNamespace;
            var annotationTargetNamespace = EdmConstants.AnnotationNamespace;
            var codegenerationXsdFileName = "Microsoft.OData.Resources.CodeGenerationSchema.xsd";
            var annotationXsdFileName = "Microsoft.OData.Resources.AnnotationSchema.xsd";

            EdmLibXmlSchemas = new Dictionary<EdmVersion, XmlSchemaSet>();
            var schemas = new XmlSchemaSet();
            schemas.Add(codegenerationTargetNamespace, XmlReader.Create(this.GetXsdStream(codegenerationXsdFileName)));
            schemas.Add(annotationTargetNamespace, XmlReader.Create(this.GetXsdStream(annotationXsdFileName)));
            schemas.Add(EdmConstants.EdmOasisNamespace, XmlReader.Create(this.GetXsdStream("Microsoft.OData.Resources.CSDLSchema_4.xsd")));
            EdmLibXmlSchemas.Add(EdmVersion.V40, schemas);
        }
    }
}
#endif
