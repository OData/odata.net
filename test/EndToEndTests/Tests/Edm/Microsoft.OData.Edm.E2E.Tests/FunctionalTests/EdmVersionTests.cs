//-----------------------------------------------------------------
// <copyright file="EdmVersionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class EdmVersionTests : EdmLibTestCaseBase
{
    [Fact]
    public void CheckCsdlV4RoundTrip()
    {
        const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(inputText)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);
        Assert.Equal(EdmConstants.EdmVersion4, model.GetEdmVersion());

        StringWriter sw = new StringWriter();
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.Encoding = System.Text.Encoding.UTF8;
        XmlWriter xw = XmlWriter.Create(sw, settings);
        model.TryWriteSchema(xw, out errors);
        xw.Flush();
        xw.Close();
        string outputText = sw.ToString();
        Assert.Equal(inputText, outputText);
    }

    [Fact]
    public void CheckMultiVersionCsdlsRoundtrip()
    {
        const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Real4"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Complex4"" BaseType=""Real4.Complex1"">
    <Property Name=""Prop4"" Type=""Edm.Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";

        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.Encoding = System.Text.Encoding.UTF8;
        List<StringWriter> outStrings = new List<StringWriter>();
        List<XmlReader> readers = new List<XmlReader>();
        List<XmlWriter> writers = new List<XmlWriter>();

        foreach (string s in new string[] { inputText })
        {
            readers.Add(XmlReader.Create(new StringReader(s)));
        }

        for (int i = 0; i < readers.Count; i++)
        {
            StringWriter sw = new StringWriter();
            outStrings.Add(sw);
            writers.Add(XmlWriter.Create(sw, settings));
        }

        bool parsed = SchemaReader.TryParse(readers, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);
        Assert.Equal(EdmConstants.EdmVersion4, model.GetEdmVersion());

        IEnumerator<XmlWriter> writerEnumerator = writers.GetEnumerator();
        model.TryWriteSchema(s => { writerEnumerator.MoveNext(); return writerEnumerator.Current; }, out errors);

        foreach (XmlWriter xw in writers)
        {
            xw.Flush();
            xw.Close();
        }

        IEnumerator<StringWriter> swEnumerator = outStrings.GetEnumerator();

        foreach (string input in new string[] { inputText })
        {
            swEnumerator.MoveNext();
            Assert.True(swEnumerator.Current.ToString() == input);
        }
    }

    [Fact]
    public void CheckDefaultVersionSerialization()
    {
        const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(inputText)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);
        Assert.Equal(EdmConstants.EdmVersion4, model.GetEdmVersion());

        StringWriter sw = new StringWriter();
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.Encoding = System.Text.Encoding.UTF8;
        XmlWriter xw = XmlWriter.Create(sw, settings);

        // Make sure it is possible to remove version from the model.
        model.SetEdmVersion(null);
        Assert.Null(model.GetEdmVersion());

        model.TryWriteSchema(xw, out errors);
        xw.Flush();
        xw.Close();
        string outputText = sw.ToString();

        parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(outputText)) }, out model, out errors);
        Assert.True(parsed);
        Assert.Empty(errors);
        Assert.Equal(EdmConstants.EdmVersionDefault, model.GetEdmVersion());
    }

    [Fact]
    public void CheckNewModelHasNoVersion()
    {
        EdmModel model = new EdmModel();
        EdmEntityType t1 = new EdmEntityType("Bunk", "T1");
        EdmStructuralProperty p1 = t1.AddStructuralProperty("P1", EdmCoreModel.Instance.GetBoolean(false));
        EdmStructuralProperty p2 = t1.AddStructuralProperty("P2", EdmCoreModel.Instance.GetDecimal(1, 1, false));
        EdmStructuralProperty p3 = t1.AddStructuralProperty("P3", EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, 1, false));
        EdmStructuralProperty p4 = t1.AddStructuralProperty("P4", EdmCoreModel.Instance.GetBinary(false, 4, false));
        EdmStructuralProperty p5 = t1.AddStructuralProperty("P5", EdmCoreModel.Instance.GetBinary(false));
        IEdmStructuralProperty q1 = (IEdmStructuralProperty)t1.FindProperty("P1");
        IEdmStructuralProperty q2 = (IEdmStructuralProperty)t1.FindProperty("P2");
        IEdmStructuralProperty q3 = (IEdmStructuralProperty)t1.FindProperty("P3");
        IEdmStructuralProperty q4 = (IEdmStructuralProperty)t1.FindProperty("P4");
        IEdmStructuralProperty q5 = (IEdmStructuralProperty)t1.FindProperty("P5");
        model.AddElement(t1);

        Assert.Null(model.GetEdmVersion());

        StringWriter sw = new StringWriter();
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.Encoding = System.Text.Encoding.UTF8;
        XmlWriter xw = XmlWriter.Create(sw, settings);

        model.TryWriteSchema(xw, out IEnumerable<EdmError> errors);
        xw.Flush();
        xw.Close();
        string outputText = sw.ToString();

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(outputText)) }, out IEdmModel iEdmModel, out errors);
        Assert.True(parsed);
        Assert.Empty(errors);
        Assert.Equal(EdmConstants.EdmVersionDefault, iEdmModel.GetEdmVersion());
    }

    [Fact]
    public void CheckEdmxVersionRoundtrip()
    {
        const string edmx =
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

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);
        Assert.Equal(EdmConstants.EdmVersion4, model.GetEdmVersion());
        Assert.Equal(CsdlConstants.EdmxVersion4, model.GetEdmxVersion());

        StringWriter sw = new StringWriter();
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.Encoding = System.Text.Encoding.UTF8;

        using (XmlWriter xw = XmlWriter.Create(sw, settings))
        {
            CsdlWriter.TryWriteCsdl(model, xw, CsdlTarget.OData, out errors);
            xw.Close();

            parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(sw.ToString())), out model, out errors);
            Assert.True(parsed);
            Assert.Empty(errors);
            Assert.Equal(EdmConstants.EdmVersion4, model.GetEdmVersion());
            Assert.Equal(CsdlConstants.EdmxVersion4, model.GetEdmxVersion());
        }
    }

    [Fact]
    public void CheckEdmxVersionChange()
    {
        const string edmx =
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

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);
        Assert.Equal(EdmConstants.EdmVersion4, model.GetEdmVersion());
        Assert.Equal(CsdlConstants.EdmxVersion4, model.GetEdmxVersion());

        StringWriter sw = new StringWriter();
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.Encoding = System.Text.Encoding.UTF8;

        using (XmlWriter xw = XmlWriter.Create(sw, settings))
        {
            model.SetEdmxVersion(CsdlConstants.EdmxVersion4);
            CsdlWriter.TryWriteCsdl(model, xw, CsdlTarget.OData, out errors);
            xw.Close();

            parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(sw.ToString())), out model, out errors);
            Assert.True(parsed);
            Assert.True(errors.Count() == 0);
            Assert.Equal(EdmConstants.EdmVersion4, model.GetEdmVersion());
            Assert.Equal(CsdlConstants.EdmxVersion4, model.GetEdmxVersion());
        }
    }

    [Fact]
    public void CheckEdmxVersionInference()
    {
        const string edmx =
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

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);
        Assert.Equal(EdmConstants.EdmVersion4, model.GetEdmVersion());
        Assert.Equal(CsdlConstants.EdmxVersion4, model.GetEdmxVersion());

        StringWriter sw = new StringWriter();
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.Encoding = System.Text.Encoding.UTF8;

        using (XmlWriter xw = XmlWriter.Create(sw, settings))
        {
            model.SetEdmxVersion(CsdlConstants.EdmxVersion4);
            CsdlWriter.TryWriteCsdl(model, xw, CsdlTarget.OData, out errors);
            xw.Close();

            parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(sw.ToString())), out model, out errors);
            Assert.True(parsed);
            Assert.Empty(errors);
            Assert.Equal(EdmConstants.EdmVersion4, model.GetEdmVersion());
            Assert.Equal(CsdlConstants.EdmxVersion4, model.GetEdmxVersion());
        }
    }

    [Fact]
    public void CheckEdmxVersionMismatch()
    {
        // Edmx namespace is for v 2.0 != Version="4.0"
        const string edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""2.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
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

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.False(parsed);
        Assert.Single(errors);
        Assert.Equal(EdmErrorCode.InvalidVersionNumber, errors.First().ErrorCode);
    }

    [Fact]
    public void CheckEdmxUnsupportedVersion()
    {
        const string edmx =
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

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);
        Assert.Equal(EdmConstants.EdmVersion4, model.GetEdmVersion());
        Assert.Equal(CsdlConstants.EdmxVersion4, model.GetEdmxVersion());

        StringWriter sw = new StringWriter();
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.Encoding = System.Text.Encoding.UTF8;

        using (XmlWriter xw = XmlWriter.Create(sw, settings))
        {
            model.SetEdmxVersion(new Version(1, 123));
            Assert.Throws<InvalidOperationException>(() => CsdlWriter.TryWriteCsdl(model, xw, CsdlTarget.OData, out errors));
        }
    }
}
