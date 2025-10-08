// ---------------------------------------------------------------------
// <copyright file="ParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class ParserTests : EdmLibTestCaseBase
{
    [Fact]
    public void Validate_ParsingStringWithFacets_ReturnsExpectedResults()
    {
        var csdlElement = XElement.Parse(@"
<Schema Namespace='ModelBuilder' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
    <EntityType Name='Content'>
        <Key>
            <PropertyRef Name='NullableStringUnboundedUnicode'/>
        </Key>
        <Property Name='NullableStringUnboundedUnicode' Type='String' Nullable='false' MaxLength='Max' Unicode='true' />
        <Property Name='NullableStringUnbounded' Type='String' Nullable='false' MaxLength='Max' Unicode='false' />
        <Property Name='NullableStringMaxLength10' Type='String' Nullable='false' MaxLength='10'/>
        <Property Name='StringCollation' Type='String' MaxLength='10'/>
        <Property Name='SimpleString' Type='String' />
    </EntityType>
</Schema>");

        var isParsed = SchemaReader.TryParse(new XElement[] { csdlElement }.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);

        Assert.NotNull(edmModel);
        var contentEntityType = edmModel.SchemaElements.Single() as IEdmEntityType;
        Assert.NotNull(contentEntityType);

        var NullableStringUnboundedUnicode = contentEntityType.FindProperty("NullableStringUnboundedUnicode").Type.AsPrimitive().AsString();
        Assert.True(NullableStringUnboundedUnicode.IsUnbounded);
        Assert.Null(NullableStringUnboundedUnicode.MaxLength);
        Assert.True(NullableStringUnboundedUnicode.IsUnicode);

        var SimpleString = contentEntityType.FindProperty("SimpleString").Type.AsPrimitive().AsString();
        Assert.False(SimpleString.IsUnbounded);
        Assert.Null(SimpleString.MaxLength);
        Assert.True(SimpleString.IsUnicode);

        var NullableStringMaxLength10 = contentEntityType.FindProperty("NullableStringMaxLength10").Type.AsPrimitive().AsString();
        Assert.False(NullableStringMaxLength10.IsUnbounded);
        Assert.NotNull(NullableStringMaxLength10.MaxLength);
        Assert.True(NullableStringMaxLength10.IsUnicode);

        var NullableStringUnbounded = contentEntityType.FindProperty("NullableStringUnbounded").Type.AsPrimitive().AsString();
        Assert.False(NullableStringUnbounded.IsUnicode);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Validate_ParsingAllPrimitiveTypesWithDefaultValues_ReturnsExpectedResults(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var entityType = new EdmEntityType("ModelBuilder.SimpleAllPrimitiveTypes", "validEntityType1");
        entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false));
        model.AddElement(entityType);

        var complexType = new EdmComplexType("ModelBuilder.SimpleAllPrimitiveTypes", "V1alidcomplexType");
        model.AddElement(complexType);

        int i = 0;
        foreach (var primitiveType in ModelBuilderHelpers.AllPrimitiveEdmTypes(edmVersion, true))
        {
            entityType.AddStructuralProperty("Property" + i++, primitiveType);
            complexType.AddStructuralProperty("Property" + i++, primitiveType);
        }

        var stringBuilder = new StringBuilder();
        var xmlWriter = XmlWriter.Create(stringBuilder);
        var result = model.TryWriteSchema((s) => xmlWriter, out IEnumerable<EdmError> errors);
        Assert.True(result);
        Assert.Empty(errors);

        xmlWriter.Close();
        var csdlElements = new[] { XElement.Parse(stringBuilder.ToString()) };

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel resultEdmModel, out errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        Assert.Equal(2, resultEdmModel.SchemaElements.Count());

        Assert.All(resultEdmModel.SchemaElements, schemaElement =>
        {
            var structuredType = schemaElement as IEdmStructuredType;
            Assert.All(structuredType.StructuralProperties(), property =>
            {
                if (property.Type is IEdmTypeReference typeReference)
                {
                    if (structuredType is not IEdmEntityType entityType2 || !entityType2.DeclaredKey.Any(n => n.Name == property.Name))
                    {
                        Assert.True(typeReference.IsNullable);
                    }
                }

                if (property.Type is IEdmBinaryTypeReference binaryType)
                {
                    Assert.False(binaryType.MaxLength.HasValue);
                }
                else if (property.Type is IEdmStringTypeReference stringType)
                {
                    Assert.False(stringType.MaxLength.HasValue);
                    Assert.True(stringType.IsUnicode);
                }
                else if (property.Type is IEdmTemporalTypeReference temporalType)
                {
                    Assert.Equal(0, temporalType.Precision);
                }
                else if (property.Type is IEdmDecimalTypeReference decimalType)
                {
                    Assert.False(decimalType.Precision.HasValue);
                    Assert.False(decimalType.Scale.HasValue);
                }
            });
        });
    }

    [Theory]
    [InlineData(EdmVersion.V40, true, true)]
    [InlineData(EdmVersion.V40, false, true)]
    [InlineData(EdmVersion.V40, true, false)]
    [InlineData(EdmVersion.V40, false, false)]
    [InlineData(EdmVersion.V401, true, true)]
    [InlineData(EdmVersion.V401, false, true)]
    [InlineData(EdmVersion.V401, true, false)]
    [InlineData(EdmVersion.V401, false, false)]
    public void Validate_ParsingAllPrimitiveTypesWithNullableAttributes_ReturnsExpectedResults(EdmVersion edmVersion, bool explicitNullable, bool isNullable)
    {
        var model = new EdmModel();
        var entityType = new EdmEntityType("ModelBuilder.SimpleAllPrimitiveTypes", "validEntityType1");
        entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false));
        model.AddElement(entityType);

        var complexType = new EdmComplexType("ModelBuilder.SimpleAllPrimitiveTypes", "V1alidcomplexType");
        model.AddElement(complexType);

        int i = 0;
        bool typesAreNullable = !explicitNullable && isNullable;
        foreach (var primitiveType in ModelBuilderHelpers.AllPrimitiveEdmTypes(edmVersion, typesAreNullable))
        {
            entityType.AddStructuralProperty("Property" + i++, primitiveType);
            complexType.AddStructuralProperty("Property" + i++, primitiveType);
        }

        var stringBuilder = new StringBuilder();
        var xmlWriter = XmlWriter.Create(stringBuilder);
        var isWritten = model.TryWriteSchema((s) => xmlWriter, out IEnumerable<EdmError> errors);
        Assert.True(isWritten && !errors.Any());

        xmlWriter.Close();
        var csdlElements = new[] { XElement.Parse(stringBuilder.ToString()) };

        if (explicitNullable)
        {
            ModelBuilderHelpers.SetNullableAttributes(csdlElements, isNullable);
        }

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel resultEdmModel, out errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        Assert.All(resultEdmModel.SchemaElements.OfType<IEdmStructuredType>(), type =>
        {
            Assert.All(type.DeclaredProperties, property =>
            {
                if (property.Name != "Id")
                {
                    Assert.Equal(property.Type.IsNullable, isNullable);
                }
            });
        });
    }

    [Fact]
    public void Validate_ParsingTypeReferenceFacets_ReturnsExpectedResults()
    {
        var csdlElement = XElement.Parse(@"
<Schema Namespace='MyNamespace' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
    <EntityType Name='MyEntityType'>
        <Key>
            <PropertyRef Name='Property1'/>
        </Key>
        <Property Name='Property1' Type='String' Nullable='false'/>
    </EntityType>
    <ComplexType Name='MyComplexType'>
        <Property Name='Property1' Type='String' />
    </ComplexType>
    <Function Name='MyFunction'><ReturnType Type='MyNamespace.MyEntityType' />
        <Parameter Name='P1' Type='Collection(MyNamespace.MyEntityType)'/>
        <Parameter Name='P2' Type='Collection(Edm.String)' MaxLength='Max' Nullable='false'/>
        <Parameter Name='P3' Type='Collection(MyNamespace.MyComplexType)'/>
        <Parameter Name='P4' Type='Collection(Binary)'/>
    </Function>
</Schema>");

        var csdlElements = new XElement[] { csdlElement };
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        CsdlToEdmModelComparer.Compare(csdlElements, model);

        var operationGroup = model.FindOperations("MyNamespace.MyFunction");
        var operation = operationGroup.Single();
        Assert.True(operation.FindParameter("P1").Type.AsCollection().ElementType().IsNullable);
        Assert.True(operation.FindParameter("P2").Type.AsCollection().ElementType().AsString().IsUnbounded);
        Assert.True(operation.FindParameter("P2").Type.AsCollection().ElementType().AsString().IsUnicode);
        Assert.Null(operation.FindParameter("P2").Type.AsCollection().ElementType().AsString().MaxLength);
        Assert.False(operation.FindParameter("P2").Type.AsCollection().ElementType().IsNullable);
        Assert.True(operation.FindParameter("P3").Type.AsCollection().ElementType().IsNullable);
        Assert.True(operation.FindParameter("P4").Type.AsCollection().ElementType().IsNullable);
        Assert.False(operation.FindParameter("P4").Type.AsCollection().ElementType().IsCollection());
        Assert.False(operation.FindParameter("P4").Type.AsCollection().ElementType().AsBinary().IsUnbounded);
        Assert.Null(operation.FindParameter("P4").Type.AsCollection().ElementType().AsBinary().MaxLength);
    }

    [Fact]
    public void Validate_ParsingWithDifferentUnderlyingXmlReaders_HandlesErrorsCorrectly()
    {
        var csdl =
@"<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <Action Name=""FunctionImportsWithReturnTypePrimitiveDataType"">
        <Parameter Name=""PrimitiveParameter1"" Type=""Edm.Binary"" />
        <Parameter Name=""PrimitiveParameter2""><Foo /></Parameter>
      </Action>
      <EntityContainer Name=""MyContainer"">
        <ActionImport Name=""FunctionImportsWithReturnTypePrimitiveDataType"" Action=""DefaultNamespace.FunctionImportsWithReturnTypePrimitiveDataType"" />
      </EntityContainer>
</Schema>";

        var csdlElements = new XElement[] { XElement.Parse(csdl) };
        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.False(parsed);
        Assert.Equal(2, errors.Count());
        Assert.Equal(EdmErrorCode.UnexpectedXmlElement, errors.First().ErrorCode);

        parsed = SchemaReader.TryParse(new XmlReader[] { csdlElements.First().CreateReader() }, out model, out errors);
        Assert.False(parsed);
        Assert.Equal(2, errors.Count());
        Assert.Equal(EdmErrorCode.UnexpectedXmlElement, errors.First().ErrorCode);
    }

    [Fact]
    public void Validate_ParsingWithIncorrectCsdlNamespace_HandlesErrorsCorrectly()
    {
        var csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Westwind"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Customer"">
    <Key>
      <PropertyRef Name=""IDC"" />
    </Key>
    <Property Name=""IDC"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" Unicode=""true"" p3:Stumble=""Rumble"" p3:Tumble=""Bumble"" xmlns:p3=""Grumble"" />
    <Property Name=""Address"" Type=""Edm.String"" Unicode=""true"" />
    <NavigationProperty Name=""Orders"" Type=""Collection(Westwind.Order)"" Partner=""Customer""  />
  </EntityType>
  <EntityType Name=""Order"">
    <Key>
      <PropertyRef Name=""IDO"" />
    </Key>
    <Property Name=""IDO"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""CustomerID"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Item"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Quantity"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""Customer"" Type=""Westwind.Customer"" Nullable=""false"" Partner=""Orders"">
      <ReferentialConstraint Property=""CustomerID"" ReferencedProperty=""IDC"" />
    </NavigationProperty>
  </EntityType>
  <EntityContainer Name=""Eastwind"">
    <EntitySet Name=""Customers"" EntityType=""Westwind.Customer"">
      <NavigationPropertyBinding Path=""Orders"" Target=""Orders""/>
    </EntitySet>
    <EntitySet Name=""Orders"" EntityType=""Westwind.Order"">
      <NavigationPropertyBinding Path=""Customer"" Target=""Customers""/>
    </EntitySet>
  </EntityContainer>
</Schema>";

        var csdlElement = new XElement[] { XElement.Parse(csdl) }.First();
        bool parsed = SchemaReader.TryParse(new XmlReader[] { csdlElement.CreateReader() }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);

        // http://docs.oasis-open.org/odata/ns/edm is a namespace that OData uses. 
        var csdlStringNewNamespace = csdlElement.ToString();
        parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdlStringNewNamespace)) }, out _, out errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        csdlStringNewNamespace = csdlElement.ToString().Replace(csdlElement.GetDefaultNamespace().NamespaceName, "http://schemas.microsoft.com/ado/2007/09/edm");
        parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdlStringNewNamespace)) }, out _, out errors);
        Assert.False(parsed);
        Assert.Single(errors);
        Assert.Equal(EdmErrorCode.UnexpectedXmlElement, errors.First().ErrorCode);
        Assert.Equal("The namespace 'http://schemas.microsoft.com/ado/2007/09/edm' is invalid. The root element is expected to belong to one of the following namespaces: 'http://docs.oasis-open.org/odata/ns/edm, http://docs.oasis-open.org/odata/ns/edm'.", errors.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ParsingEntityTypeWithoutKey_ReportsExpectedErrors()
    {
        var csdl = @"
<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"">
        <EntitySet Name=""EntityTypeWithoutKey"" EntityType=""TestModel.EntityTypeWithoutKey"" />
        <Singleton Name=""SingletonWhoseEntityTypeWithoutKey"" Type=""TestModel.EntityTypeWithoutKey"" />
    </EntityContainer>
    <EntityType Name=""EntityTypeWithoutKey"" />
</Schema>";

        var csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        Assert.Equal(2, model.SchemaElements.Count());

        IEdmEntityType entityType = (IEdmEntityType)model.SchemaElements.First();
        Assert.Equal("TestModel.EntityTypeWithoutKey", entityType.FullName());
        Assert.Equal("EntityTypeWithoutKey", entityType.Name);

        IEdmEntityContainer entityContainer = model.EntityContainer;
        Assert.Equal("DefaultContainer", entityContainer.Name);
        Assert.Equal(2, entityContainer.Elements.Count());

        IEdmEntitySet entitySet = entityContainer.FindEntitySet("EntityTypeWithoutKey");
        Assert.Equal("EntityTypeWithoutKey", entitySet.Name);
        Assert.Equal("TestModel.EntityTypeWithoutKey", entitySet.EntityType.FullName());

        IEdmSingleton singleton = entityContainer.FindSingleton("SingletonWhoseEntityTypeWithoutKey");
        Assert.Equal("SingletonWhoseEntityTypeWithoutKey", singleton.Name);
        Assert.Equal("TestModel.EntityTypeWithoutKey", singleton.EntityType.FullName());
        model.Validate(out IEnumerable<EdmError> actualErrors);
        Assert.Single(actualErrors);

        Assert.Equal(EdmErrorCode.NavigationSourceTypeHasNoKeys, actualErrors.First().ErrorCode);
        Assert.Equal("(4, 10)", actualErrors.First().ErrorLocation.ToString());
    }

    [Fact]
    public void Validate_ParsingDuplicateEntityTypes_ReportsExpectedErrors()
    {
        var csdl = @"
<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"">
        <EntitySet Name=""DuplicateEntityType"" EntityType=""TestModel.DuplicateEntityType"" />
        <EntitySet Name=""DuplicateEntityType"" EntityType=""TestModel.DuplicateEntityType"" />
    </EntityContainer>
    <EntityType Name=""DuplicateEntityType"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""DuplicateEntityType"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    </EntityType>
</Schema>";

        var csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        IEdmEntityContainer entityContainer = model.EntityContainer;
        Assert.Equal("DefaultContainer", entityContainer.Name);
        Assert.True(entityContainer.Elements.Count() == 2);
        Assert.Equal(EdmContainerElementKind.EntitySet, entityContainer.Elements.ElementAt(0).ContainerElementKind);
        Assert.Equal(EdmContainerElementKind.EntitySet, entityContainer.Elements.ElementAt(1).ContainerElementKind);

        IEdmEntitySet entitySetElement1 = (IEdmEntitySet)entityContainer.Elements.ElementAt(0);
        Assert.Equal("DuplicateEntityType", entitySetElement1.Name);
        Assert.Equal("TestModel.DuplicateEntityType", entitySetElement1.EntityType.FullName());

        IEdmEntitySet entitySetElement2 = (IEdmEntitySet)entityContainer.Elements.ElementAt(1);
        Assert.Equal("DuplicateEntityType", entitySetElement2.Name);
        Assert.Equal("TestModel.DuplicateEntityType", entitySetElement2.EntityType.FullName());

        Assert.True(model.SchemaElements.Count() == 3, "Invalid schema element count");
        Assert.Equal(EdmSchemaElementKind.TypeDefinition, model.SchemaElements.ElementAt(0).SchemaElementKind);
        Assert.Equal(EdmSchemaElementKind.TypeDefinition, model.SchemaElements.ElementAt(1).SchemaElementKind);

        IEdmEntityType entityTypeElement1 = (IEdmEntityType)model.SchemaElements.ElementAt(0);
        Assert.Equal("TestModel.DuplicateEntityType", entityTypeElement1.FullName());
        Assert.Equal("DuplicateEntityType", entityTypeElement1.Name);
        Assert.Single(entityTypeElement1.Properties());
        Assert.Equal("Id", entityTypeElement1.Properties().Single().Name);
        Assert.Single(entityTypeElement1.DeclaredKey);

        IEdmEntityType entityTypeElement2 = (IEdmEntityType)model.SchemaElements.ElementAt(1);
        Assert.Equal("TestModel.DuplicateEntityType", entityTypeElement2.FullName());
        Assert.Equal("DuplicateEntityType", entityTypeElement2.Name);
        Assert.Single(entityTypeElement2.Properties());
        Assert.Equal("Id", entityTypeElement2.Properties().Single().Name);
        Assert.Single(entityTypeElement2.DeclaredKey);

        model.Validate(out IEnumerable<EdmError> actualErrors);
        Assert.Equal(4, actualErrors.Count());
        Assert.Equal(EdmErrorCode.BadUnresolvedEntityType, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The entity type 'TestModel.DuplicateEntityType' could not be found.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal(EdmErrorCode.BadUnresolvedEntityType, actualErrors.ElementAt(1).ErrorCode);
        Assert.Equal("The entity type 'TestModel.DuplicateEntityType' could not be found.", actualErrors.ElementAt(1).ErrorMessage);
        Assert.Equal(EdmErrorCode.AlreadyDefined, actualErrors.ElementAt(2).ErrorCode);
        Assert.Equal("An element with the name 'TestModel.DuplicateEntityType' is already defined.", actualErrors.ElementAt(2).ErrorMessage);
        Assert.Equal(EdmErrorCode.DuplicateEntityContainerMemberName, actualErrors.ElementAt(3).ErrorCode);
        Assert.Equal("Each member name in an EntityContainer must be unique. A member with name 'DuplicateEntityType' is already defined.", actualErrors.ElementAt(3).ErrorMessage);

        Assert.Equal("(4, 10)", actualErrors.ElementAt(0).ErrorLocation.ToString());
        Assert.Equal("(5, 10)", actualErrors.ElementAt(1).ErrorLocation.ToString());
        Assert.Equal("(13, 6)", actualErrors.ElementAt(2).ErrorLocation.ToString());
        Assert.Equal("(5, 10)", actualErrors.ElementAt(3).ErrorLocation.ToString());
    }

    [Fact]
    public void Validate_ParsingDuplicateComplexTypes_ReportsExpectedErrors()
    {
        var csdl = @"
<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"" />
    <ComplexType Name=""DuplicateComplexTypes"">
       <Property Name=""Data"" Type=""Edm.String"" />
    </ComplexType>
    <ComplexType Name=""DuplicateComplexTypes"">
       <Property Name=""Data"" Type=""Edm.String"" />
    </ComplexType>
</Schema>";

        var csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        IEdmEntityContainer entityContainer = model.EntityContainer;
        Assert.Equal("DefaultContainer", entityContainer.Name);
        Assert.Empty(entityContainer.Elements);

        Assert.True(model.SchemaElements.Count() == 3);
        Assert.Equal(EdmSchemaElementKind.TypeDefinition, model.SchemaElements.ElementAt(0).SchemaElementKind);
        Assert.Equal(EdmSchemaElementKind.TypeDefinition, model.SchemaElements.ElementAt(1).SchemaElementKind);

        IEdmComplexType complexTypeElement1 = (IEdmComplexType)model.SchemaElements.ElementAt(0);
        Assert.Equal("TestModel.DuplicateComplexTypes", complexTypeElement1.FullName());
        Assert.Equal("DuplicateComplexTypes", complexTypeElement1.Name);

        IEdmComplexType complexTypeElement2 = (IEdmComplexType)model.SchemaElements.ElementAt(1);
        Assert.Equal("TestModel.DuplicateComplexTypes", complexTypeElement2.FullName());
        Assert.Equal("DuplicateComplexTypes", complexTypeElement2.Name);

        model.Validate(out IEnumerable<EdmError> actualErrors);
        Assert.Single(actualErrors);
        Assert.Equal(EdmErrorCode.AlreadyDefined, actualErrors.Last().ErrorCode);

        Assert.Equal("(7, 6)", actualErrors.Last().ErrorLocation.ToString());
    }

    [Fact]
    public void Validate_ParsingComplexTypeWithDuplicateProperties_ReportsExpectedErrors()
    {
        var csdl = @"
<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"" />
    <ComplexType Name=""DuplicatePropertiesComplexType"">
        <Property Name=""Duplicate"" Type=""String"" />
        <Property Name=""Duplicate"" Type=""String"" />
    </ComplexType>
</Schema>";

        var csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        IEdmEntityContainer entityContainer = model.EntityContainer;
        Assert.Equal("DefaultContainer", entityContainer.Name);
        Assert.Empty(entityContainer.Elements);

        Assert.Equal(2, model.SchemaElements.Count());
        Assert.Equal(EdmSchemaElementKind.TypeDefinition, model.SchemaElements.ElementAt(0).SchemaElementKind);

        IEdmComplexType complexTypeElement = (IEdmComplexType)model.SchemaElements.ElementAt(0);
        Assert.Equal("TestModel.DuplicatePropertiesComplexType", complexTypeElement.FullName());
        Assert.Equal("DuplicatePropertiesComplexType", complexTypeElement.Name);

        Assert.Equal(2, complexTypeElement.DeclaredProperties.Count());
        Assert.Equal(EdmPropertyKind.Structural, complexTypeElement.DeclaredProperties.ElementAt(0).PropertyKind);
        Assert.Equal(EdmPropertyKind.Structural, complexTypeElement.DeclaredProperties.ElementAt(1).PropertyKind);

        IEdmProperty complexProperty1 = (IEdmProperty)complexTypeElement.DeclaredProperties.ElementAt(0);
        Assert.Equal("Duplicate", complexProperty1.Name);

        IEdmProperty complexProperty2 = (IEdmProperty)complexTypeElement.DeclaredProperties.ElementAt(1);
        Assert.Equal("Duplicate", complexProperty2.Name);

        model.Validate(out IEnumerable<EdmError> actualErrors);
        Assert.Single(actualErrors);
        Assert.Equal(EdmErrorCode.AlreadyDefined, actualErrors.Last().ErrorCode);

        Assert.Equal("(6, 10)", actualErrors.Last().ErrorLocation.ToString());
    }

    [Fact]
    public void Validate_ParsingEntityTypeWithDuplicateProperties_ReportsExpectedErrors()
    {
        var csdl = @"
<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"">
        <EntitySet Name=""DuplicatePropertiesEntityType"" EntityType=""TestModel.DuplicatePropertiesEntityType"" />
    </EntityContainer>
    <EntityType Name=""DuplicatePropertiesEntityType"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Duplicate"" Type=""String"" />
        <Property Name=""Duplicate"" Type=""String"" />
    </EntityType>
</Schema>";

        var csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        IEdmEntityContainer entityContainer = model.EntityContainer;
        Assert.Equal("DefaultContainer", entityContainer.Name);
        Assert.Single(entityContainer.Elements);
        Assert.Equal(EdmContainerElementKind.EntitySet, entityContainer.Elements.Single().ContainerElementKind);

        IEdmEntitySet entitySet = (IEdmEntitySet)entityContainer.Elements.Single();
        Assert.Equal("DuplicatePropertiesEntityType", entitySet.Name);
        Assert.Equal("TestModel.DuplicatePropertiesEntityType", entitySet.EntityType.FullName());

        Assert.True(model.SchemaElements.Count() == 2);

        IEdmEntityType entityType = (IEdmEntityType)model.SchemaElements.First();
        Assert.Equal("TestModel.DuplicatePropertiesEntityType", entityType.FullName());
        Assert.Equal("DuplicatePropertiesEntityType", entityType.Name);

        Assert.True(entityType.Properties().Count() == 3);
        Assert.Equal(EdmPropertyKind.Structural, entityType.Properties().ElementAt(0).PropertyKind);
        Assert.Equal(EdmPropertyKind.Structural, entityType.Properties().ElementAt(1).PropertyKind);
        Assert.Equal(EdmPropertyKind.Structural, entityType.Properties().ElementAt(2).PropertyKind);

        Assert.Equal("Id", entityType.Properties().ElementAt(0).Name);
        Assert.Equal("Duplicate", entityType.Properties().ElementAt(1).Name);
        Assert.Equal("Duplicate", entityType.Properties().ElementAt(2).Name);

        model.Validate(out IEnumerable<EdmError> actualErrors);
        Assert.Single(actualErrors);
        Assert.Equal(EdmErrorCode.AlreadyDefined, actualErrors.Last().ErrorCode);

        Assert.Equal("(12, 10)", actualErrors.Last().ErrorLocation.ToString());
    }

    [Fact]
    public void Validate_ParsingSelfReferencingNavigationPropertyWithPrimaryKey_ReportsExpectedErrors()
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""PersonToFriend"" Type=""Self.Person"" Partner=""FriendToPerson"">
            <ReferentialConstraint Property=""Id"" ReferencedProperty=""Id"" />
        </NavigationProperty>
        <NavigationProperty Name=""FriendToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""PersonToFriend"" />
    </EntityType>
</Schema>";

        var csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        model.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.Single(validationErrors);
        Assert.Equal(EdmErrorCode.InvalidMultiplicityOfPrincipalEnd, validationErrors.First().ErrorCode);
        Assert.Equal("Because all dependent properties of the navigation 'PersonToFriend' are non-nullable, the multiplicity of the principal end must be '1'.", validationErrors.First().ErrorMessage);
    }

    [Fact(Skip = @"An FK association based on PK-PK selfjoin should not be allowed")]
    public void Validate_ParsingSelfReferencingNavigationPropertyWithBaseType_ReportsExpectedErrors()
    {
        var csdl = @"
< Schema Namespace=""DefaultNamespace"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""PersonToEmployee"" Type=""Self.Employee"" Partner=""EmployeeToPerson"" />
    </EntityType>
    <EntityType Name=""Employee"" BaseType=""DefaultNamespace.Person"">
        <Property Name=""Name"" Type=""Edm.String"" />
        <NavigationProperty Name=""EmployeeToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""PersonToEmployee"">
             <ReferentialConstraint Property=""Id"" ReferencedProperty=""Id"" />
        </NavigationProperty>
    </EntityType>
</Schema>";

        var csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        model.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.Empty(validationErrors);
    }
}