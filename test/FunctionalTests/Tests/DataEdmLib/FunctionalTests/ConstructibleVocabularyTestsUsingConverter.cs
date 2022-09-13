//---------------------------------------------------------------------
// <copyright file="ConstructibleVocabularyTestsUsingConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConstructibleVocabularyTestsUsingConverter : EdmLibTestCaseBase
    {
        public ConstructibleVocabularyTestsUsingConverter()
        {
            this.EdmVersion = EdmVersion.V40;
        }

        [TestMethod]
        public void ConstructibleVocabularyTestOnVocabularyTestModels()
        {
            var stubEdmModels = new EdmLibTestModelExtractor().GetModels<IEdmModel>(typeof(VocabularyTestModelBuilder), this.EdmVersion, new CustomConstructibleVocabularyTestAttribute(), false);
            foreach (var stubEdmModel in stubEdmModels)
            {
                var csdls = new List<XElement>();
                csdls.AddRange(new VocabularyDefinitionCsdlGenerator().GenerateDefinitionCsdl(this.EdmVersion, stubEdmModel.Value));
                csdls.Add(new VocabularyApplicationCsdlGenerator().GenerateApplicationCsdl(this.EdmVersion, stubEdmModel.Value));
                var edmModel = this.GetParserResult(csdls);

                var stockModel = new EdmToStockModelConverter().ConvertToStockModel(edmModel);
                var comparer = new VocabularyModelComparer();
                var compareErrorMessages = comparer.CompareModels(edmModel, stockModel);

                compareErrorMessages.ToList().ForEach(e => Console.WriteLine(e));
                Assert.AreEqual(0, compareErrorMessages.Count, "Comparison errors");
            }
        }

        [TestMethod]
        public void ConstructibleVocabularySerializingAnnotationsWithNoTerm()
        {
            var stockModel = new EdmModel();
            var customer = new EdmEntityType("NS1", "Customer");
            var customerId = customer.AddStructuralProperty("CustomerID", EdmCoreModel.Instance.GetString(false));
            customer.AddKeys(customerId);
            stockModel.AddElement(customer);

            var annotation = new MutableVocabularyAnnotation()
            {
                Target = customer,
                Value = new EdmStringConstant("Hello world2!"),
            };
            stockModel.AddVocabularyAnnotation(annotation);

            var stringWriter = new StringWriter();
            var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings() { Indent = true });
            IEnumerable<EdmError> serializationErrors;
            stockModel.TryWriteSchema(xmlWriter, out serializationErrors);
            xmlWriter.Close();

            Assert.AreEqual(1, serializationErrors.Count(), "Error on serialization");
        }

        private sealed class MutableVocabularyAnnotation : Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation
        {
            public IEdmExpression Value
            {
                get;
                set;
            }

            public string Qualifier
            {
                get;
                set;
            }

            public IEdmTerm Term
            {
                get;
                set;
            }

            public IEdmVocabularyAnnotatable Target
            {
                get;
                set;
            }
        }

        [TestMethod]
        public void ConstructibleVocabularySimpleVocabularyAnnotationOnContainerAndEntitySet()
        {
            var csdl = XElement.Parse(
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityContainer Name=""Container"" />
  <Annotations Target=""NS1.Container"">
    <Annotation Term=""NS1.Title"" String=""Sir"" />
  </Annotations>
  <Term Name=""Title"" Type=""String"" Unicode=""true"" />
</Schema>");
            var model = this.GetParserResult(new[] { csdl });
            var annotations = model.FindVocabularyAnnotations(model.FindEntityContainer("NS1.Container"));
            Assert.AreEqual
                            (
                                ((IEdmStringConstantExpression)annotations.Single().Value).Value,
                                "Sir",
                                "FindVocabularyAnnotations should be able to find annotations on entity containers."
                            );
        }

        [TestMethod]
        public void ConstructibleVocabularyVocabularyAnnotationWithRecord()
        {
            VerifyVocabulary(new EdmToStockModelConverter().ConvertToStockModel(VocabularyTestModelBuilder.VocabularyAnnotationWithRecord()));
        }

        [TestMethod]
        public void ConstructibleVocabularyStructuredVocabularyAnnotation()
        {
            VerifyVocabulary(new EdmToStockModelConverter().ConvertToStockModel(VocabularyTestModelBuilder.StructuredVocabularyAnnotation()));
        }

        private IEnumerable<XElement> GetVocabularyCsdls(IEdmModel edmModel)
        {
            var csdls = new List<XElement>();
            csdls.AddRange(new VocabularyDefinitionCsdlGenerator().GenerateDefinitionCsdl(this.EdmVersion, edmModel));
            csdls.Add(new VocabularyApplicationCsdlGenerator().GenerateApplicationCsdl(this.EdmVersion, edmModel));
            return csdls;
        }

        private void VerifyVocabulary(IEdmModel edmModel)
        {
            IEdmModel parsedModel = this.GetParserResult(GetVocabularyCsdls(edmModel));

            var comparer = new VocabularyModelComparer();
            var compareErrorMessages = comparer.CompareModels(edmModel, parsedModel);

            compareErrorMessages.ToList().ForEach(e => Console.WriteLine(e));
            Assert.AreEqual(0, compareErrorMessages.Count, "comparision errors");
        }
    }
}
