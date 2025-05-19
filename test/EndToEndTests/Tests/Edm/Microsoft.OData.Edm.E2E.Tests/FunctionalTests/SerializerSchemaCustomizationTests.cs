//---------------------------------------------------------------------
// <copyright file="SerializerSchemaCustomizationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.OData.Edm;

public class SerializerSchemaCustomizationTests : EdmLibTestCaseBase
{
    [Fact]
    public void OneContainer_NoSchema_NoCustomization()
    {
        var edmModel = new EdmModel();
        var oneNamespaceAndEntityContainers = new NamespaceAndEntityContainers("Default", "MyContainer");
        foreach (string entityContainer in oneNamespaceAndEntityContainers.Containers)
        {
            var container = new EdmEntityContainer(oneNamespaceAndEntityContainers.Namespace, entityContainer);
            edmModel.AddElement(container);
        }

        IEnumerable<string> actualCsdlStrings = this.GetSerializerResult(edmModel);

        var expectedCsdlStrings = new string[]
            { @"
<Schema Namespace='$$DefaultSchemaNamespace$$' xmlns='$$XmlNamespace$$'>
  <EntityContainer Name='MyContainer' />
</Schema>",
            };

        Assert.Equal(expectedCsdlStrings.Count(), actualCsdlStrings.Count());

        var expectedSchemaElements = expectedCsdlStrings.Select(s =>
        {
            string expectedString = this.FixupAllNamespacePlaceholders(s);
            return XElement.Parse(expectedString);
        });

        var actaulSchemaElements = actualCsdlStrings.Select(s => XElement.Parse(s));

        foreach (XElement expectedSchema in expectedSchemaElements)
        {
            string schemaNamespace = this.ExtractSchemaNamespace(expectedSchema);
            var actualSchemaWithMatchingNamespace = actaulSchemaElements.Where(s => this.ExtractSchemaNamespace(s) == schemaNamespace);
            Assert.Single(actualSchemaWithMatchingNamespace);

            XElement actualSchema = actualSchemaWithMatchingNamespace.Single();
            Assert.Equal(expectedSchema.ToString(), actualSchema.ToString());
        }
    }

    [Fact]
    public void OneContainer_OneSchema_NoCustomization()
    {
        var edmModel = new EdmModel();

        var oneNamespaceAndComplexTypes = new NamespaceAndComplexTypes("NS1", "T1");
        foreach (string complexType in oneNamespaceAndComplexTypes.ComplexTypes)
        {
            var t1 = new EdmComplexType(oneNamespaceAndComplexTypes.Namespace, complexType);
            edmModel.AddElement(t1);
        }

        var oneNamespaceAndEntityContainers = new NamespaceAndEntityContainers("NS1", "MyContainer");
        foreach (string entityContainer in oneNamespaceAndEntityContainers.Containers)
        {
            var container = new EdmEntityContainer(oneNamespaceAndEntityContainers.Namespace, entityContainer);
            edmModel.AddElement(container);
        }

        IEnumerable<string> actualCsdlStrings = this.GetSerializerResult(edmModel);

        var expectedCsdlStrings = new string[]
            { @"
<Schema Namespace='NS1' xmlns='$$XmlNamespace$$'>
  <ComplexType Name='T1' />
  <EntityContainer Name='MyContainer' />
</Schema>",
            };

        Assert.Equal(expectedCsdlStrings.Count(), actualCsdlStrings.Count());

        var expectedSchemaElements = expectedCsdlStrings.Select(s =>
        {
            string expectedString = this.FixupAllNamespacePlaceholders(s);
            return XElement.Parse(expectedString);
        });

        var actaulSchemaElements = actualCsdlStrings.Select(s => XElement.Parse(s));

        foreach (XElement expectedSchema in expectedSchemaElements)
        {
            string schemaNamespace = this.ExtractSchemaNamespace(expectedSchema);
            var actualSchemaWithMatchingNamespace = actaulSchemaElements.Where(s => this.ExtractSchemaNamespace(s) == schemaNamespace);
            Assert.Single(actualSchemaWithMatchingNamespace);

            XElement actualSchema = actualSchemaWithMatchingNamespace.Single();
            Assert.Equal(expectedSchema.ToString(), actualSchema.ToString());
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
                              .Replace("$$DefaultSchemaNamespace$$", "Default");
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
