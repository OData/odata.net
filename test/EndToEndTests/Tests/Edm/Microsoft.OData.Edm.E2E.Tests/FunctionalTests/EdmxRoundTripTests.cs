//---------------------------------------------------------------------
// <copyright file="EdmxRoundTripTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml;
using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

/// <summary>
/// "Round trip" refers to the process of serializing an EDM (Entity Data Model) to an EDMX (XML-based representation of the model) and then deserializing it back to an EDM. 
/// The goal is to ensure that the model remains consistent and valid throughout this process, meaning that the serialized and deserialized representations are equivalent.
/// </summary>
public class EdmxRoundTripTests : EdmLibTestCaseBase
{
    [Fact]
    public void Validate_InvalidNamespaceAndVersionInEdmx_ThrowsVersionMismatchError()
    {
        var edmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""1.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out IEdmModel model, out IEnumerable<EdmError> actualErrors);
        Assert.False(parsed);
        Assert.Equal(EdmErrorCode.InvalidVersionNumber, actualErrors.First().ErrorCode);
        Assert.Equal("The EDMX version specified in the 'Version' attribute does not match the version corresponding to the namespace of the 'Edmx' element.", actualErrors.First().ErrorMessage);
    }

    [Fact]
    public void Validate_MissingConceptualModelsInEdmx_ThrowsUnexpectedXmlElementError()
    {
        var edmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema />
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out IEdmModel model, out IEnumerable<EdmError> actualErrors);
        Assert.False(parsed);
        Assert.Equal(EdmErrorCode.UnexpectedXmlElement, actualErrors.First().ErrorCode);
        Assert.Equal("The root element has no namespace. The root element is expected to belong to one of the following namespaces: 'http://docs.oasis-open.org/odata/ns/edm, http://docs.oasis-open.org/odata/ns/edm'.", actualErrors.First().ErrorMessage);
    }

    [Fact]
    public void Validate_InvalidConceptualModelNamespaceInEdmx_ThrowsUnexpectedXmlElementError()
    {
        var edmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Name=""DefaultNamespace"" xmlns=""http://foo"" />
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out IEdmModel model, out IEnumerable<EdmError> actualErrors);
        Assert.False(parsed);
        Assert.Equal(EdmErrorCode.UnexpectedXmlElement, actualErrors.First().ErrorCode);
        Assert.Equal("The namespace 'http://foo' is invalid. The root element is expected to belong to one of the following namespaces: 'http://docs.oasis-open.org/odata/ns/edm, http://docs.oasis-open.org/odata/ns/edm'.", actualErrors.First().ErrorMessage);
    }

    [Fact]
    public void Validate_MissingDataServicesInEdmx_ThrowsUnexpectedXmlElementError()
    {
        var edmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema />
  </edmx:DataServices>
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out IEdmModel model, out IEnumerable<EdmError> actualErrors);
        Assert.False(parsed);
        Assert.Equal(EdmErrorCode.UnexpectedXmlElement, actualErrors.First().ErrorCode);
        Assert.Equal("The root element has no namespace. The root element is expected to belong to one of the following namespaces: 'http://docs.oasis-open.org/odata/ns/edm, http://docs.oasis-open.org/odata/ns/edm'.", actualErrors.First().ErrorMessage);
    }

    [Fact]
    public void Validate_TwoDataServicesInEdmx_ThrowsBodyElementError()
    {
        var edmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""ComplexType"">
        <Property Name=""Data"" Type=""Edm.String"" />
      </ComplexType>
      <EntityContainer Name=""Container"" />
    </Schema>
  </edmx:DataServices>
  <edmx:DataServices>
    <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""SimpleType"">
        <Property Name=""Data"" Type=""Edm.String"" />
      </ComplexType>
      <EntityContainer Name=""Container"" />
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out IEdmModel model, out IEnumerable<EdmError> actualErrors);
        Assert.False(parsed);
        Assert.Equal(EdmErrorCode.UnexpectedXmlElement, actualErrors.First().ErrorCode);
        Assert.Equal("Unexpected DataServices element while parsing Edmx. Edmx is expected to have at most one of 'Runtime' or 'DataServices' elements.", actualErrors.First().ErrorMessage);
    }

    [Fact]
    public void Validate_TwoRuntimeSectionsInEdmx_ThrowsBodyElementError()
    {
        var edmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""SimpleType"">
          <Property Name=""Data"" Type=""Edm.String"" />
        </ComplexType>
        <EntityContainer Name=""Container"" />
      </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""SimpleType"">
          <Property Name=""Data"" Type=""Edm.String"" />
        </ComplexType>
        <EntityContainer Name=""Container"" />
      </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out IEdmModel model, out IEnumerable<EdmError> actualErrors);
        Assert.False(parsed);
        Assert.Equal(EdmErrorCode.UnexpectedXmlElement, actualErrors.First().ErrorCode);
        Assert.Equal("Unexpected DataServices element while parsing Edmx. Edmx is expected to have at most one of 'Runtime' or 'DataServices' elements.", actualErrors.First().ErrorMessage);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_EmptyConceptualModelsInEdmx_RoundTripsSuccessfully(EdmVersion edmVersion)
    {
        var expectedEdmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels />
  </edmx:Runtime>
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(expectedEdmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        string actualEdmx = GetEdmx(model, CsdlTarget.EntityFramework);
        var valid = XElement.DeepEquals(XElement.Parse(expectedEdmx), XElement.Parse(actualEdmx));
        Assert.True(valid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_EmptyConceptualModelsInModel_RoundTripsSuccessfully(EdmVersion edmVersion)
    {
        var versionToString = edmVersion == EdmVersion.V401 ? "4.01" : "4.0";

        var expectedEdmx = $@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""{versionToString}"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels />
  </edmx:Runtime>
</edmx:Edmx>";

        var model = new EdmModel();
        model.SetEdmxVersion(toProductVersionlookup[edmVersion]);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        string actualEdmx = GetEdmx(model, CsdlTarget.EntityFramework);
        var valid = XElement.DeepEquals(XElement.Parse(expectedEdmx), XElement.Parse(actualEdmx));
        Assert.True(valid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_SimpleConceptualModelsInEdmx_RoundTripsSuccessfully(EdmVersion edmVersion)
    {
        var expectedEdmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""SimpleType"">
          <Property Name=""Data"" Type=""Edm.String"" />
        </ComplexType>
        <EntityContainer Name=""Container"" />
      </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(expectedEdmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        string actualEdmx = GetEdmx(model, CsdlTarget.EntityFramework);
        var valid = XElement.DeepEquals(XElement.Parse(expectedEdmx), XElement.Parse(actualEdmx));
        Assert.True(valid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_SimpleConceptualModelsInModel_RoundTripsSuccessfully(EdmVersion edmVersion)
    {
        var expectedEdmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""SimpleType"">
          <Property Name=""Data"" Type=""Edm.String"" />
        </ComplexType>
        <EntityContainer Name=""Container"" />
      </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";

        var model = new EdmModel();

        EdmComplexType simpleType = new EdmComplexType("DefaultNamespace", "SimpleType");
        simpleType.AddProperty(new EdmStructuralProperty(simpleType, "Data", EdmCoreModel.Instance.GetString(true)));
        model.AddElement(simpleType);

        EdmEntityContainer container = new EdmEntityContainer("DefaultNamespace", "Container");
        model.AddElement(container);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        string actualEdmx = GetEdmx(model, CsdlTarget.EntityFramework);
        var valid = XElement.DeepEquals(XElement.Parse(expectedEdmx), XElement.Parse(actualEdmx));
        Assert.True(valid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_EmptyDataServicesInEdmx_RoundTripsSuccessfully(EdmVersion edmVersion)
    {
        var expectedEdmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices />
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(expectedEdmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        string actualEdmx = GetEdmx(model, CsdlTarget.OData);
        var valid = XElement.DeepEquals(XElement.Parse(expectedEdmx), XElement.Parse(actualEdmx));
        Assert.True(valid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_EmptyDataServicesInModel_RoundTripsSuccessfully(EdmVersion edmVersion)
    {
        var versionToString = edmVersion == EdmVersion.V401 ? "4.01" : "4.0";

        var expectedEdmx = $@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""{versionToString}"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices />
</edmx:Edmx>";

        var model = new EdmModel();
        model.SetEdmxVersion(toProductVersionlookup[edmVersion]);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        string actualEdmx = GetEdmx(model, CsdlTarget.OData);
        var valid = XElement.DeepEquals(XElement.Parse(expectedEdmx), XElement.Parse(actualEdmx));
        Assert.True(valid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_EmptyDataServicesWithAttributesInEdmx_RoundTripsSuccessfully(EdmVersion edmVersion)
    {
        var modelEdmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices m:DataServiceVersion=""2.31"" m:Other=""foo"" m:MaxDataServiceVersion=""2.5"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" />
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(modelEdmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var expectedEdmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices />
</edmx:Edmx>";

        string actualEdmx = GetEdmx(model, CsdlTarget.OData);
        var valid = XElement.DeepEquals(XElement.Parse(expectedEdmx), XElement.Parse(actualEdmx));
        Assert.True(valid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_SimpleDataServicesInEdmx_RoundTripsSuccessfully(EdmVersion edmVersion)
    {
        var expectedEdmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""SimpleType"">
        <Property Name=""Data"" Type=""Edm.String"" />
      </ComplexType>
      <EntityContainer Name=""Container"" />
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(expectedEdmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        string actualEdmx = GetEdmx(model, CsdlTarget.OData);
        var valid = XElement.DeepEquals(XElement.Parse(expectedEdmx), XElement.Parse(actualEdmx));
        Assert.True(valid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_SimpleDataServicesInModel_RoundTripsSuccessfully(EdmVersion edmVersion)
    {
        var expectedEdmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""SimpleType"">
        <Property Name=""Data"" Type=""Edm.String"" />
      </ComplexType>
      <EntityContainer Name=""Container"" />
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        var model = new EdmModel();

        EdmComplexType simpleType = new EdmComplexType("DefaultNamespace", "SimpleType");
        simpleType.AddProperty(new EdmStructuralProperty(simpleType, "Data", EdmCoreModel.Instance.GetString(true)));
        model.AddElement(simpleType);

        EdmEntityContainer container = new EdmEntityContainer("DefaultNamespace", "Container");
        model.AddElement(container);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        string actualEdmx = GetEdmx(model, CsdlTarget.OData);
        var valid = XElement.DeepEquals(XElement.Parse(expectedEdmx), XElement.Parse(actualEdmx));
        Assert.True(valid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_DataServicesWithVersionAttributesInModelToConceptualModels_RoundTripsSuccessfully(EdmVersion edmVersion)
    {
        var expectedEdmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""SimpleType"">
          <Property Name=""Data"" Type=""Edm.String"" />
        </ComplexType>
        <EntityContainer Name=""Container"" />
      </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";

        var model = new EdmModel();

        EdmComplexType simpleType = new EdmComplexType("DefaultNamespace", "SimpleType");
        simpleType.AddProperty(new EdmStructuralProperty(simpleType, "Data", EdmCoreModel.Instance.GetString(true)));
        model.AddElement(simpleType);

        EdmEntityContainer container = new EdmEntityContainer("DefaultNamespace", "Container");
        model.AddElement(container);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        string actualEdmx = GetEdmx(model, CsdlTarget.EntityFramework);
        var valid = XElement.DeepEquals(XElement.Parse(expectedEdmx), XElement.Parse(actualEdmx));
        Assert.True(valid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_ConceptualModelsToDataServices_RoundTripsSuccessfully(EdmVersion edmVersion)
    {
        var modelEdmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""SimpleType"">
          <Property Name=""Data"" Type=""Edm.String"" />
        </ComplexType>
        <EntityContainer Name=""Container"" />
      </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(modelEdmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var expectedEdmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""SimpleType"">
        <Property Name=""Data"" Type=""Edm.String"" />
      </ComplexType>
      <EntityContainer Name=""Container"" />
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        string actualEdmx = GetEdmx(model, CsdlTarget.OData);
        var valid = XElement.DeepEquals(XElement.Parse(expectedEdmx), XElement.Parse(actualEdmx));
        Assert.True(valid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_DataServicesToConceptualModels_RoundTripsSuccessfully(EdmVersion edmVersion)
    {
        var modelEdmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""SimpleType"">
        <Property Name=""Data"" Type=""Edm.String"" />
      </ComplexType>
      <EntityContainer Name=""Container"" />
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(modelEdmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var expectedEdmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""SimpleType"">
          <Property Name=""Data"" Type=""Edm.String"" />
        </ComplexType>
        <EntityContainer Name=""Container"" />
      </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";

        string actualEdmx = GetEdmx(model, CsdlTarget.EntityFramework);
        var valid = XElement.DeepEquals(XElement.Parse(expectedEdmx), XElement.Parse(actualEdmx));
        Assert.True(valid);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_UsingAliasInEdmx_RoundTripsSuccessfully(EdmVersion edmVersion)
    {
        var modelEdmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""SimpleType"">
          <Property Name=""Data"" Type=""Edm.String"" />
        <Property Name=""DataType"" Type=""Display.SimpleType"" />
        </ComplexType>
        <EntityContainer Name=""Container"" />
      </Schema>
    <Schema Namespace=""Org.OData.Display"" Alias=""Display"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""SimpleType"">
        <Property Name=""Data"" Type=""Edm.String"" />
      </ComplexType>
    </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(modelEdmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var expectedEdmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""SimpleType"">
          <Property Name=""Data"" Type=""Edm.String"" />
        <Property Name=""DataType"" Type=""Display.SimpleType"" />
        </ComplexType>
        <EntityContainer Name=""Container"" />
      </Schema>
    <Schema Namespace=""Org.OData.Display"" Alias=""Display"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""SimpleType"">
        <Property Name=""Data"" Type=""Edm.String"" />
      </ComplexType>
    </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";

        string actualEdmx = GetEdmx(model, CsdlTarget.EntityFramework);
        var valid = XElement.DeepEquals(XElement.Parse(expectedEdmx), XElement.Parse(actualEdmx));
        Assert.True(valid);
    }

    #region referenced model - Include, IncludeAnnotations

    [Fact]
    public void Validate_UsingAliasInReferencedEdmx_RoundTripsSuccessfully()
    {
        var mainEdmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/VipCustomer.xml"">
    <edmx:Include Namespace=""NS.Ref1"" Alias=""VPCT"" />
    <edmx:IncludeAnnotations TermNamespace="""" TargetNamespace=""NS.Ref1"" />
    <edmx:IncludeAnnotations TermNamespace=""org.example.validation"" />
    <edmx:IncludeAnnotations TermNamespace=""org.example.display"" Qualifier=""Tablet""/>
    <edmx:IncludeAnnotations TermNamespace=""org.example.hcm"" TargetNamespace=""com.contoso.Sales"" />
    <edmx:IncludeAnnotations TermNamespace=""org.example.hcm"" Qualifier=""Tablet"" TargetNamespace=""com.contoso.Person"" />
  </edmx:Reference>
  <edmx:Reference Uri=""http://host/schema/VipCard.xml"">
    <edmx:Include Namespace=""NS.Ref2"" Alias=""VPCD"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""NS1"" Alias=""CT"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""Customer"">
            <Key>
            <PropertyRef Name=""CustomerID"" />
            </Key>
            <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
        </EntityType>
        <ComplexType Name=""Address"" >
            <Property Name=""StreetAddress"" Type=""Edm.String"" Nullable=""false"" />
            <Property Name=""ZipCode"" Type=""Edm.Int32"" Nullable=""false"" />
        </ComplexType>
        <EntityContainer Name=""C1"" Extends=""Vip_C2"">
            <EntitySet Name=""Customers"" EntityType=""CT.Customer"" />
            <EntitySet Name=""VipCustomers"" EntityType=""VPCT.VipCustomer"" />
            <EntitySet Name=""VipCards"" EntityType=""VPCD.VipCard"" />
        </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        var referencedEdmx1 = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/VipCard.xml"">
    <edmx:Include Namespace=""NS.Ref2"" Alias=""VCD"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""NS.Ref1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""VipCustomer"">
        <Key>
        <PropertyRef Name=""VipCustomerID"" />
        </Key>
        <Property Name=""VipCustomerID"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""VipAddress"" Type=""NS1.Address"" Nullable=""false"" />
        <NavigationProperty Name=""VipCards"" Type=""Collection(VCD.VipCard)"" />
    </EntityType>
    <EntityContainer Name=""Vip_C1"">
        <EntitySet Name=""VipCustomers"" EntityType=""NS1.VipCustomer"" />
    </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        var referencedEdmx2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS.Ref2"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""VipCard"">
        <Key>
        <PropertyRef Name=""VipCardID"" />
        </Key>
        <Property Name=""VipCardID"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <EntityContainer Name=""Vip_C2"">
        <EntitySet Name=""VipCustomers2"" EntityType=""NS1.VipCustomer"" />
    </EntityContainer>
    </Schema>
    <Schema Namespace=""NS.Ref2_NotIncluded"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""VipCard"">
        <Key>
        <PropertyRef Name=""VipCardID"" />
        </Key>
        <Property Name=""VipCardID"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        Func<Uri, XmlReader> getReferencedModelReaderFunc = uri =>
        {
            if (uri.AbsoluteUri == "http://host/schema/VipCustomer.xml")
            {
                return XmlReader.Create(new StringReader(referencedEdmx1));
            }

            if (uri.AbsoluteUri == "http://host/schema/VipCard.xml")
            {
                return XmlReader.Create(new StringReader(referencedEdmx2));
            }

            return null;
        };

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(mainEdmx)), getReferencedModelReaderFunc, out IEdmModel mainModel, out IEnumerable<EdmError> parsedErrors);
        Assert.True(parsed && !parsedErrors.Any());

        bool valid = mainModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(valid && !validationErrors.Any());

        string actualEdmx = GetEdmx(mainModel, CsdlTarget.OData);

        // after deserialization & serialization, the alias'ed 'CT.Customer' becomes qualified name 'NS1.Customer',
        // so make some adjustments for verification: 
        actualEdmx = actualEdmx.Replace("EntityType=\"NS1.Customer\"", "EntityType=\"CT.Customer\"");
        actualEdmx = actualEdmx.Replace("EntityType=\"NS.Ref1.VipCustomer\"", "EntityType=\"VPCT.VipCustomer\"");
        actualEdmx = actualEdmx.Replace("EntityType=\"NS.Ref2.VipCard\"", "EntityType=\"VPCD.VipCard\"");
        valid = XElement.DeepEquals(XElement.Parse(mainEdmx), XElement.Parse(actualEdmx));
        Assert.True(valid);
    }

    [Fact]
    public void Validate_EdmxReferences_RoundTripsSuccessfully()
    {
        var mainEdmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/VipCustomer.xml"">
    <edmx:Include Namespace=""NS.Ref1"" Alias=""VPCT"" />
    <edmx:IncludeAnnotations TermNamespace="""" TargetNamespace=""NS.Ref1"" />
    <edmx:IncludeAnnotations TermNamespace=""org.example.validation"" />
    <edmx:IncludeAnnotations TermNamespace=""org.example.display"" Qualifier=""Tablet""/>
    <edmx:IncludeAnnotations TermNamespace=""org.example.hcm"" TargetNamespace=""com.contoso.Sales"" />
    <edmx:IncludeAnnotations TermNamespace=""org.example.hcm"" Qualifier=""Tablet"" TargetNamespace=""com.contoso.Person"" />
  </edmx:Reference>
  <edmx:Reference Uri=""http://host/schema/VipCard.xml"">
    <edmx:Include Namespace=""NS.Ref2"" Alias=""VPCD"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""NS1"" Alias=""CT"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""Customer"">
            <Key>
            <PropertyRef Name=""CustomerID"" />
            </Key>
            <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
        </EntityType>
        <ComplexType Name=""Address"" >
            <Property Name=""StreetAddress"" Type=""Edm.String"" Nullable=""false"" />
            <Property Name=""ZipCode"" Type=""Edm.Int32"" Nullable=""false"" />
        </ComplexType>
        <EntityContainer Name=""C1"" Extends=""Vip_C2"">
            <EntitySet Name=""Customers"" EntityType=""CT.Customer"" />
            <EntitySet Name=""VipCustomers"" EntityType=""VPCT.VipCustomer"" />
            <EntitySet Name=""VipCards"" EntityType=""VPCD.VipCard"" />
        </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        var referencedEdmx1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/VipCard.xml"">
    <edmx:Include Namespace=""NS.Ref2"" Alias=""VCD"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""NS.Ref1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""VipCustomer"">
        <Key>
        <PropertyRef Name=""VipCustomerID"" />
        </Key>
        <Property Name=""VipCustomerID"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""VipAddress"" Type=""NS1.Address"" Nullable=""false"" />
        <NavigationProperty Name=""VipCards"" Type=""Collection(VCD.VipCard)"" />
    </EntityType>
    <EntityContainer Name=""Vip_C1"">
        <EntitySet Name=""VipCustomers"" EntityType=""NS1.VipCustomer"" />
    </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        var referencedEdmx2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS.Ref2"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""VipCard"">
        <Key>
        <PropertyRef Name=""VipCardID"" />
        </Key>
        <Property Name=""VipCardID"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <EntityContainer Name=""Vip_C2"">
        <EntitySet Name=""VipCustomers2"" EntityType=""NS1.VipCustomer"" />
    </EntityContainer>
    </Schema>
    <Schema Namespace=""NS.Ref2_NotIncluded"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""VipCard"">
        <Key>
        <PropertyRef Name=""VipCardID"" />
        </Key>
        <Property Name=""VipCardID"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        Func<Uri, XmlReader> getReferencedModelReaderFunc = uri =>
        {
            if (uri.AbsoluteUri == "http://host/schema/VipCustomer.xml")
            {
                return XmlReader.Create(new StringReader(referencedEdmx1));
            }

            if (uri.AbsoluteUri == "http://host/schema/VipCard.xml")
            {
                return XmlReader.Create(new StringReader(referencedEdmx2));
            }

            return null;
        };

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(mainEdmx)), getReferencedModelReaderFunc, out IEdmModel mainModel, out IEnumerable<EdmError> parsedErrors);
        Assert.True(parsed);
        Assert.Empty(parsedErrors);

        bool valid = mainModel.Validate(out IEnumerable<EdmError> errors);
        Assert.True(valid);

        // verify reading edmx:Reference
        List<IEdmReference> references = mainModel.GetEdmReferences().ToList();
        Assert.Equal(2, references.Count);
        Assert.Equal("VPCT", references[0].Includes.First().Alias);
        Assert.Equal("NS.Ref1", references[0].Includes.First().Namespace);

        // verify Uri in EdmReference
        string uriString = "http://addedUrl/addedEdm.xml";
        EdmReference newReference = new EdmReference(new Uri(uriString));
        //Assert.Equal(uriString, EdmValueWriter.UriAsXml(newReference.Uri));

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
        Assert.True(valid);
    }
    #endregion

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
}