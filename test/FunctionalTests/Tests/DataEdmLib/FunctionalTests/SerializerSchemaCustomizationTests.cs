//---------------------------------------------------------------------
// <copyright file="SerializerSchemaCustomizationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SerializerSchemaCustomizationTests : EdmLibTestCaseBase
    {
        public SerializerSchemaCustomizationTests()
        {
            this.EdmVersion = EdmVersion.V40;
        }

        private const string defaultSerializerSchemaNamespace = "Default";
        
        [TestMethod]
        public void OneContainer_NoSchema_NoCustomization()
        {
            var edmModel = this.DefineEdmModel(
                    new NamespaceAndEntityContainers[] { new NamespaceAndEntityContainers("Default", "MyContainer") }
                    );

            IEnumerable<string> actualCsdlStrings = this.GetSerializerResult(edmModel);

            this.Compare(new string[] 
                { @"
<Schema Namespace='$$DefaultSchemaNamespace$$' xmlns='$$XmlNamespace$$'>
  <EntityContainer Name='MyContainer' />
</Schema>",
                },
                actualCsdlStrings);
        }

        [TestMethod]
        public void OneContainer_OneSchema_NoCustomization()
        {
            var edmModel = this.DefineEdmModel(
                    new NamespaceAndEntityContainers[] { new NamespaceAndEntityContainers("NS1", "MyContainer") },
                    new NamespaceAndComplexTypes("NS1", "T1")
                    );

            IEnumerable<string> actualCsdlStrings = this.GetSerializerResult(edmModel);

            this.Compare(new string[] 
                { @"
<Schema Namespace='NS1' xmlns='$$XmlNamespace$$'>
  <ComplexType Name='T1' />
  <EntityContainer Name='MyContainer' />
</Schema>",
                },
                actualCsdlStrings);
        }

        private EdmModel DefineEdmModel(IEnumerable<NamespaceAndEntityContainers> allNamespaceAndEntityContainers, params NamespaceAndComplexTypes[] allNamespaceAndComplexTypes)
        {
            var edmModel = new EdmModel();

            foreach (var oneNamespaceAndComplexTypes in allNamespaceAndComplexTypes)
            {
                foreach (string complexType in oneNamespaceAndComplexTypes.ComplexTypes)
                {
                    var t1 = new EdmComplexType(oneNamespaceAndComplexTypes.Namespace, complexType);
                    edmModel.AddElement(t1);
                }
            }

            foreach (var oneNamespaceAndEntityContainers in allNamespaceAndEntityContainers)
            {
                foreach (string entityContainer in oneNamespaceAndEntityContainers.Containers)
                {
                    var container = new EdmEntityContainer(oneNamespaceAndEntityContainers.Namespace, entityContainer);
                    edmModel.AddElement(container);
                }
            }
            return edmModel;
        }

        private void Compare(IEnumerable<string> expectedCsdlStrings, IEnumerable<string> actualCsdlStrings)
        {
            Assert.AreEqual(expectedCsdlStrings.Count(), actualCsdlStrings.Count(), "csdl count wrong.");

            var expectedSchemaElements = expectedCsdlStrings.Select(s =>
                {
                    string expectedString = this.FixupAllNamespacePlaceholders(s);
                    return XElement.Parse(expectedString);
                });

            var actaulSchemaElements = actualCsdlStrings.Select(s => XElement.Parse(s));

            foreach(XElement expectedSchema in expectedSchemaElements)
            {
                string schemaNamespace = this.ExtractSchemaNamespace(expectedSchema);
                var actualSchemaWithMatchingNamespace = actaulSchemaElements.Where(s => this.ExtractSchemaNamespace(s) == schemaNamespace);
                Assert.AreEqual(1, actualSchemaWithMatchingNamespace.Count(), "there should be exactly one actual Csdl string with Schema Namespace: {0}", schemaNamespace);

                XElement actualSchema = actualSchemaWithMatchingNamespace.Single();
                Assert.AreEqual(expectedSchema.ToString(), actualSchema.ToString(), "Csdl not equal!");
            }
        }

        private string ExtractSchemaNamespace(XElement expectedSchemaElement)
        {
            return expectedSchemaElement.Attribute("Namespace").Value;
        }

        private string FixupAllNamespacePlaceholders(string inputCsdlString)
        {
            string xmlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(this.EdmVersion).NamespaceName;
            
            return inputCsdlString.Replace("$$XmlNamespace$$", xmlNamespace)
                                  .Replace("$$DefaultSchemaNamespace$$", defaultSerializerSchemaNamespace);
        }

        private class NamespaceAndComplexTypes
        {
            public NamespaceAndComplexTypes(string namespaceName, params string[] complexTypes)
            {
                this.Namespace = namespaceName;
                this.ComplexTypes = complexTypes;
            }

            public string Namespace { get; private set; }

            public IEnumerable<string> ComplexTypes { get; private set; }
        }

        private class NamespaceAndEntityContainers
        {
            public NamespaceAndEntityContainers(string namespaceName, params string[] containers)
            {
                this.Namespace = namespaceName;
                this.Containers = containers;
            }

            public string Namespace { get; private set; }

            public IEnumerable<string> Containers { get; private set; }
        }
    }
}
