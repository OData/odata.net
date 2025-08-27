//-------------------------------------------------------------------
// <copyright file="EdmxSerializingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class EdmxSerializingTests : EdmLibTestCaseBase
{
    [Fact]
    public void Serialize_SimpleSchemaAsEntityFrameworkEdmx_GeneratesExpectedOutput()
    {
        const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Customer"">
    <Key>
      <PropertyRef Name=""CustomerID"" />
    </Key>
    <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
  </EntityType>
  <EntityContainer Name=""C1"">
    <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
  </EntityContainer>
</Schema>";

        const string expectedResult =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""Customer"">
          <Key>
            <PropertyRef Name=""CustomerID"" />
          </Key>
          <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
        </EntityType>
        <EntityContainer Name=""C1"">
          <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
        </EntityContainer>
      </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(inputText)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        StringWriter sw = new StringWriter();
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.Encoding = System.Text.Encoding.UTF8;
        XmlWriter xw = XmlWriter.Create(sw, settings);
        CsdlWriter.TryWriteCsdl(model, xw, CsdlTarget.EntityFramework, out errors);
        xw.Flush();
        xw.Close();
        string outputText = sw.ToString();
        Assert.Equal(expectedResult, outputText);
    }

    [Fact]
    public void Serialize_MultipleCsdlSchemasAsEntityFrameworkEdmx_GeneratesExpectedOutput()
    {
        const string inputText1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";
        const string inputText2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Mumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Clod"" BaseType=""Grumble.Smod"">
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" MaxLength=""1024"" />
    <Property Name=""Address"" Type=""Edm.String"" MaxLength=""2048"" />
  </ComplexType>
</Schema>";

        const string expectedResult =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""Smod"">
          <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        </ComplexType>
      </Schema>
      <Schema Namespace=""Mumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""Clod"" BaseType=""Grumble.Smod"">
          <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" MaxLength=""1024"" />
          <Property Name=""Address"" Type=""Edm.String"" MaxLength=""2048"" />
        </ComplexType>
      </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";

        List<XmlReader> readers = new()
        {
            XmlReader.Create(new StringReader(inputText1)),
            XmlReader.Create(new StringReader(inputText2))
        };

        bool parsed = SchemaReader.TryParse(readers, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        StringWriter sw = new StringWriter();
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.Encoding = System.Text.Encoding.UTF8;
        XmlWriter xw = XmlWriter.Create(sw, settings);
        CsdlWriter.TryWriteCsdl(model, xw, CsdlTarget.EntityFramework, out errors);
        xw.Flush();
        xw.Close();
        string outputText = sw.ToString();
        Assert.Equal(expectedResult, outputText);
    }

    [Fact]
    public void Serialize_SimpleSchemaAsODataEdmx_GeneratesExpectedOutput()
    {
        const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Customer"">
    <Key>
      <PropertyRef Name=""CustomerID"" />
    </Key>
    <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
  </EntityType>
  <EntityContainer Name=""C1"">
    <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
  </EntityContainer>
</Schema>";

        const string expectedResult =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Customer"">
        <Key>
          <PropertyRef Name=""CustomerID"" />
        </Key>
        <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
      </EntityType>
      <EntityContainer Name=""C1"">
        <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(inputText)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        StringWriter sw = new StringWriter();
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.Encoding = System.Text.Encoding.UTF8;
        XmlWriter xw = XmlWriter.Create(sw, settings);
        CsdlWriter.TryWriteCsdl(model, xw, CsdlTarget.OData, out errors);
        xw.Flush();
        xw.Close();
        string outputText = sw.ToString();
        Assert.Equal(expectedResult, outputText);
    }

    [Fact]
    public void Serialize_MultipleCsdlSchemasAsODataEdmx_GeneratesExpectedOutput()
    {
        const string inputText1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";
        const string inputText2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Mumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Clod"" BaseType=""Grumble.Smod"">
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" MaxLength=""1024"" />
    <Property Name=""Address"" Type=""Edm.String"" MaxLength=""2048"" />
  </ComplexType>
</Schema>";

        const string expectedResult =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""Smod"">
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
      </ComplexType>
    </Schema>
    <Schema Namespace=""Mumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""Clod"" BaseType=""Grumble.Smod"">
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" MaxLength=""1024"" />
        <Property Name=""Address"" Type=""Edm.String"" MaxLength=""2048"" />
      </ComplexType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        List<XmlReader> readers = new()
        {
            XmlReader.Create(new StringReader(inputText1)),
            XmlReader.Create(new StringReader(inputText2))
        };

        bool parsed = SchemaReader.TryParse(readers, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        StringWriter sw = new StringWriter();
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.Encoding = System.Text.Encoding.UTF8;
        XmlWriter xw = XmlWriter.Create(sw, settings);
        CsdlWriter.TryWriteCsdl(model, xw, CsdlTarget.OData, out errors);
        xw.Flush();
        xw.Close();
        string outputText = sw.ToString();
        Assert.Equal(expectedResult, outputText);
    }

    [Fact]
    public void Validate_EntityContainerSchemaNamespace_IsPreserved()
    {
        const string inputText1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
      <Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""Smod"">
          <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        </ComplexType>
        <EntityType Name=""Blot"">
          <Key>
            <PropertyRef Name=""Id"" />
          </Key>
          <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        </EntityType>
      </Schema>";

        const string inputText2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
      <Schema Namespace=""Mumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""Clod"" BaseType=""Grumble.Smod"">
          <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" MaxLength=""1024"" />
          <Property Name=""Address"" Type=""Edm.String"" MaxLength=""2048"" />
        </ComplexType>
        <EntityContainer Name=""C1"">
          <EntitySet Name=""Blots"" EntityType=""Grumble.Blot"" />
        </EntityContainer>
      </Schema>";

        const string expectedResult =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""Smod"">
          <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        </ComplexType>
        <EntityType Name=""Blot"">
          <Key>
            <PropertyRef Name=""Id"" />
          </Key>
          <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        </EntityType>
      </Schema>
      <Schema Namespace=""Mumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""Clod"" BaseType=""Grumble.Smod"">
          <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" MaxLength=""1024"" />
          <Property Name=""Address"" Type=""Edm.String"" MaxLength=""2048"" />
        </ComplexType>
        <EntityContainer Name=""C1"">
          <EntitySet Name=""Blots"" EntityType=""Grumble.Blot"" />
        </EntityContainer>
      </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";

        List<XmlReader> readers = new()
        {
            XmlReader.Create(new StringReader(inputText1)),
            XmlReader.Create(new StringReader(inputText2))
        };

        bool parsed = SchemaReader.TryParse(readers, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        Assert.Equal("Mumble", model.EntityContainer.Namespace);

        StringWriter sw = new StringWriter();
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.Encoding = System.Text.Encoding.UTF8;
        XmlWriter xw = XmlWriter.Create(sw, settings);
        CsdlWriter.TryWriteCsdl(model, xw, CsdlTarget.EntityFramework, out errors);
        xw.Flush();
        xw.Close();
        string outputText = sw.ToString();
        Assert.Equal(expectedResult, outputText);
    }

    [Fact]
    public void Validate_EntityContainerPreservesNamespaceOfEmptySchema()
    {
        const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
      <Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityContainer Name=""C1"">
          <EntitySet Name=""Blots"" EntityType=""Grumble.Blot"" />
        </EntityContainer>
      </Schema>";

        const string expectedResult =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityContainer Name=""C1"">
          <EntitySet Name=""Blots"" EntityType=""Grumble.Blot"" />
        </EntityContainer>
      </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(inputText)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        Assert.Equal("Grumble", model.EntityContainer.Namespace);

        StringWriter sw = new StringWriter();
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.Encoding = System.Text.Encoding.UTF8;
        XmlWriter xw = XmlWriter.Create(sw, settings);
        CsdlWriter.TryWriteCsdl(model, xw, CsdlTarget.EntityFramework, out errors);
        xw.Flush();
        xw.Close();
        string outputText = sw.ToString();
        Assert.Equal(expectedResult, outputText);
    }

    [Fact]
    public void Validate_NullArgumentsInCsdlWriter_ThrowsArgumentNullException()
    {
        const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
    <Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Customer"">
        <Key>
          <PropertyRef Name=""CustomerID"" />
        </Key>
        <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
      </EntityType>
      <EntityContainer Name=""C1"">
        <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        var valid = CsdlReader.TryParse(XmlReader.Create(new StringReader(inputText)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(valid);

        var exception1 = Assert.Throws<ArgumentNullException>(() => CsdlWriter.TryWriteCsdl(null, XmlWriter.Create(new StringWriter()), CsdlTarget.EntityFramework, out errors));
        Assert.Equal("Value cannot be null. (Parameter 'model')", exception1.Message);

        var exception2 = Assert.Throws<ArgumentNullException>(() => CsdlWriter.TryWriteCsdl(model, null, CsdlTarget.EntityFramework, out errors));
        Assert.Equal("Value cannot be null. (Parameter 'writer')", exception2.Message);
    }
}