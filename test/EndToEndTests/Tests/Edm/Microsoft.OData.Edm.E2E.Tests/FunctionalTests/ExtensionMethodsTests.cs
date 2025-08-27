//---------------------------------------------------------------------
// <copyright file="ExtensionMethodsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class ExtensionMethodsTests : EdmLibTestCaseBase
{
    [Fact]
    public void Validate_FindEntitySetInContainer_ReturnsExpectedResults()
    {
        IEdmModel edmModel = this.GetEdmModel();
        IEdmEntityContainer container = edmModel.EntityContainer;

        var result = container.FindEntitySet("_Not_Exist_");
        Assert.Null(result);

        var entitySet = container.Elements.OfType<IEdmEntitySet>().First();
        result = container.FindEntitySet(entitySet.Name);
        Assert.Equal(entitySet, result);
    }

    [Fact]
    public void Validate_FindEntitySetWithNullName_ReturnsNull()
    {
        IEdmModel edmModel = this.GetEdmModel();
        IEdmEntityContainer container = edmModel.EntityContainer;

        Assert.Null(container.FindEntitySet(null));
    }

    [Fact]
    public void Validate_ComplexTypeReferenceExtensions_ReturnsExpectedResults()
    {
        IEdmModel edmModel = this.GetEdmModel();
        IEdmComplexType derivedComplexType = edmModel.SchemaElements.OfType<IEdmComplexType>().First(c => c.BaseType != null);
        IEdmComplexType baseComplexType = derivedComplexType.BaseComplexType();

        Assert.NotNull(baseComplexType);

        var derivedComplexTypeRef = derivedComplexType.ToTypeReference() as IEdmComplexTypeReference;

        Assert.Equal(baseComplexType, derivedComplexTypeRef.BaseComplexType());
        Assert.Equal(baseComplexType, derivedComplexTypeRef.BaseType());

        Assert.Equal(derivedComplexType.IsAbstract, derivedComplexTypeRef.IsAbstract());
        Assert.Equal(derivedComplexType.IsOpen, derivedComplexTypeRef.IsOpen());

        Assert.Equal(derivedComplexType.DeclaredStructuralProperties().Count(), derivedComplexTypeRef.DeclaredStructuralProperties().Count());
        Assert.Equal(derivedComplexType.StructuralProperties().Count(), derivedComplexTypeRef.StructuralProperties().Count());
    }

    [Fact]
    public void Validate_EntityTypeReferenceExtensions_ReturnsExpectedResults()
    {
        IEdmModel edmModel = this.GetEdmModel();
        IEdmEntityType derivedEntityType = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.BaseType != null);
        IEdmEntityType baseEntityType = derivedEntityType.BaseEntityType();

        Assert.NotNull(baseEntityType);

        var derivedEntityTypeRef = derivedEntityType.ToTypeReference() as IEdmEntityTypeReference;

        Assert.Equal(baseEntityType, derivedEntityTypeRef.BaseEntityType());
        Assert.Equal(baseEntityType, derivedEntityTypeRef.BaseType());

        Assert.Equal(derivedEntityType.IsAbstract, derivedEntityTypeRef.IsAbstract());
        Assert.Equal(derivedEntityType.IsOpen, derivedEntityTypeRef.IsOpen());

        Assert.Equal(derivedEntityType.DeclaredStructuralProperties().Count(), derivedEntityTypeRef.DeclaredStructuralProperties().Count());
        Assert.Equal(derivedEntityType.StructuralProperties().Count(), derivedEntityTypeRef.StructuralProperties().Count());

        Assert.Equal(derivedEntityType.DeclaredNavigationProperties().Count(), derivedEntityTypeRef.DeclaredNavigationProperties().Count());
        Assert.Equal(derivedEntityType.NavigationProperties().Count(), derivedEntityTypeRef.NavigationProperties().Count());

        IEdmNavigationProperty result = derivedEntityTypeRef.FindNavigationProperty("_Not_Exist_");
        Assert.Null(result);

        var navigation = derivedEntityType.NavigationProperties().First();
        result = derivedEntityTypeRef.FindNavigationProperty(navigation.Name);
        Assert.Equal(navigation, result);
    }

    [Fact]
    public void Validate_FindNavigationPropertyWithNullName_ThrowsArgumentNullException()
    {
        IEdmModel edmModel = this.GetEdmModel();
        IEdmEntityType derivedEntityType = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.BaseType != null);
        var derivedEntityTypeRef = derivedEntityType.ToTypeReference() as IEdmEntityTypeReference;

        var exception = Assert.Throws<ArgumentNullException>(() => derivedEntityTypeRef.FindNavigationProperty(null));
        Assert.Equal("Value cannot be null. (Parameter 'name')", exception.Message);
    }

    [Fact]
    public void Validate_PrimitiveTypeReferenceFullName_ReturnsCorrectFullName()
    {
        Assert.Equal("Edm.Binary", EdmCoreModel.Instance.GetBinary(false).FullName());
        Assert.Equal("Edm.Boolean", EdmCoreModel.Instance.GetBoolean(false).FullName());
        Assert.Equal("Edm.Byte", EdmCoreModel.Instance.GetByte(false).FullName());
        Assert.Equal("Edm.DateTimeOffset", EdmCoreModel.Instance.GetDateTimeOffset(false).FullName());
        Assert.Equal("Edm.Decimal", EdmCoreModel.Instance.GetDecimal(false).FullName());
        Assert.Equal("Edm.Double", EdmCoreModel.Instance.GetDouble(false).FullName());
        Assert.Equal("Edm.Guid", EdmCoreModel.Instance.GetGuid(false).FullName());
        Assert.Equal("Edm.Int16", EdmCoreModel.Instance.GetInt16(false).FullName());
        Assert.Equal("Edm.Int32", EdmCoreModel.Instance.GetInt32(false).FullName());
        Assert.Equal("Edm.Int64", EdmCoreModel.Instance.GetInt64(false).FullName());
        Assert.Equal("Edm.SByte", EdmCoreModel.Instance.GetSByte(false).FullName());
        Assert.Equal("Edm.Single", EdmCoreModel.Instance.GetSingle(false).FullName());
        Assert.Equal("Edm.Stream", EdmCoreModel.Instance.GetStream(false).FullName());
        Assert.Equal("Edm.String", EdmCoreModel.Instance.GetString(false).FullName());
        Assert.Equal("Edm.Duration", EdmCoreModel.Instance.GetDuration(false).FullName());
        Assert.Equal("Edm.Date", EdmCoreModel.Instance.GetDate(false).FullName());
        Assert.Equal("Edm.TimeOfDay", EdmCoreModel.Instance.GetTimeOfDay(false).FullName());
    }

    [Fact]
    public void Validate_PrimitiveTypeReferenceShortQualifiedName_ReturnsCorrectShortName()
    {
        Assert.Equal("Binary", EdmCoreModel.Instance.GetBinary(false).ShortQualifiedName());
        Assert.Equal("Boolean", EdmCoreModel.Instance.GetBoolean(false).ShortQualifiedName());
        Assert.Equal("Byte", EdmCoreModel.Instance.GetByte(false).ShortQualifiedName());
        Assert.Equal("DateTimeOffset", EdmCoreModel.Instance.GetDateTimeOffset(false).ShortQualifiedName());
        Assert.Equal("Decimal", EdmCoreModel.Instance.GetDecimal(false).ShortQualifiedName());
        Assert.Equal("Double", EdmCoreModel.Instance.GetDouble(false).ShortQualifiedName());
        Assert.Equal("Guid", EdmCoreModel.Instance.GetGuid(false).ShortQualifiedName());
        Assert.Equal("Int16", EdmCoreModel.Instance.GetInt16(false).ShortQualifiedName());
        Assert.Equal("Int32", EdmCoreModel.Instance.GetInt32(false).ShortQualifiedName());
        Assert.Equal("Int64", EdmCoreModel.Instance.GetInt64(false).ShortQualifiedName());
        Assert.Equal("SByte", EdmCoreModel.Instance.GetSByte(false).ShortQualifiedName());
        Assert.Equal("Single", EdmCoreModel.Instance.GetSingle(false).ShortQualifiedName());
        Assert.Equal("Stream", EdmCoreModel.Instance.GetStream(false).ShortQualifiedName());
        Assert.Equal("String", EdmCoreModel.Instance.GetString(false).ShortQualifiedName());
        Assert.Equal("Duration", EdmCoreModel.Instance.GetDuration(false).ShortQualifiedName());
        Assert.Equal("Date", EdmCoreModel.Instance.GetDate(false).ShortQualifiedName());
        Assert.Equal("TimeOfDay", EdmCoreModel.Instance.GetTimeOfDay(false).ShortQualifiedName());
    }

    [Fact]
    public void Validate_NamedTypeReferenceFullName_ForEntityType_ReturnsCorrectFullName()
    {
        IEdmModel edmModel = this.GetEdmModel();

        var schemaElementsOfTypeIEdmEntityType = edmModel.SchemaElements.OfType<IEdmEntityType>();
        Assert.NotEmpty(schemaElementsOfTypeIEdmEntityType);

        var personEntity = schemaElementsOfTypeIEdmEntityType.ElementAt(0);
        Assert.Equal("NS1.Person", personEntity.FullName());
        Assert.Equal("NS1.Person", personEntity.ShortQualifiedName());
        Assert.Equal("NS1.Person", (new EdmEntityTypeReference(personEntity, false)).FullName());
        Assert.Equal("NS1.Person", (new EdmEntityTypeReference(personEntity, false)).ShortQualifiedName());

        var customerEntity = schemaElementsOfTypeIEdmEntityType.ElementAt(1);
        Assert.Equal("NS1.Customer", customerEntity.FullName());
        Assert.Equal("NS1.Customer", customerEntity.ShortQualifiedName());
        Assert.Equal("NS1.Customer", (new EdmEntityTypeReference(customerEntity, false)).FullName());
        Assert.Equal("NS1.Customer", (new EdmEntityTypeReference(customerEntity, false)).ShortQualifiedName());

        var orderEntity = schemaElementsOfTypeIEdmEntityType.ElementAt(2);
        Assert.Equal("NS1.Order", orderEntity.FullName());
        Assert.Equal("NS1.Order", orderEntity.ShortQualifiedName());
        Assert.Equal("NS1.Order", (new EdmEntityTypeReference(orderEntity, false)).FullName());
        Assert.Equal("NS1.Order", (new EdmEntityTypeReference(orderEntity, false)).ShortQualifiedName());
    }

    [Fact]
    public void Validate_NamedTypeReferenceFullName_ForComplexType_ReturnsCorrectFullName()
    {
        IEdmModel edmModel = this.GetEdmModel();

        var schemaElementsOfTypeIEdmComplexType = edmModel.SchemaElements.OfType<IEdmComplexType>();
        Assert.NotEmpty(schemaElementsOfTypeIEdmComplexType);

        var addressComplexType = schemaElementsOfTypeIEdmComplexType.ElementAt(0);
        Assert.Equal("NS1.Address", addressComplexType.FullName());
        Assert.Equal("NS1.Address", addressComplexType.ShortQualifiedName());
        Assert.Equal("NS1.Address", (new EdmComplexTypeReference(addressComplexType, false)).FullName());
        Assert.Equal("NS1.Address", (new EdmComplexTypeReference(addressComplexType, false)).ShortQualifiedName());

        var zipCodeComplexType = schemaElementsOfTypeIEdmComplexType.ElementAt(1);
        Assert.Equal("NS1.ZipCode", zipCodeComplexType.FullName());
        Assert.Equal("NS1.ZipCode", zipCodeComplexType.ShortQualifiedName());
        Assert.Equal("NS1.ZipCode", (new EdmComplexTypeReference(zipCodeComplexType, false)).FullName());
        Assert.Equal("NS1.ZipCode", (new EdmComplexTypeReference(zipCodeComplexType, false)).ShortQualifiedName());

        var foreignAddressComplexType = schemaElementsOfTypeIEdmComplexType.ElementAt(2);
        Assert.Equal("NS1.ForeignAddress", foreignAddressComplexType.FullName());
        Assert.Equal("NS1.ForeignAddress", foreignAddressComplexType.ShortQualifiedName());
        Assert.Equal("NS1.ForeignAddress", (new EdmComplexTypeReference(foreignAddressComplexType, false)).FullName());
        Assert.Equal("NS1.ForeignAddress", (new EdmComplexTypeReference(foreignAddressComplexType, false)).ShortQualifiedName());
    }

    [Fact]
    public void Validate_FindAllDerivedTypesAcrossModels_ReturnsExpectedResults()
    {
        EdmModel model = new EdmModel();
        EdmModel referencedModel = new EdmModel();

        var A = new EdmEntityType("Referenced", "A");
        var AProp = A.AddStructuralProperty("ID", EdmCoreModel.Instance.GetString(false));
        A.AddKeys(new[] { AProp });

        var B = new EdmEntityType("Referenced", "B", A);
        var C = new EdmEntityType("Referenced", "C", B);

        var D = new EdmEntityType("Referenced", "D", C);

        referencedModel.AddElements(new[] { A, B, C });
        model.AddReferencedModel(referencedModel);
        model.AddElement(D);

        IEnumerable<IEdmStructuredType> derivedTypes = model.FindAllDerivedTypes(A);
        Assert.Equal(3, derivedTypes.Count());
        Assert.Contains(B, derivedTypes);
        Assert.Contains(C, derivedTypes);
        Assert.Contains(D, derivedTypes);
    }

    [Fact]
    public void Validate_BuildModelWithGetUInt16_SerializesCorrectly()
    {
        var model = new EdmModel();

        var personType = new EdmEntityType("MyNS", "Person");
        personType.AddKeys(personType.AddStructuralProperty("ID", model.GetUInt16("MyNS", true)));
        model.AddElement(personType);

        string outputText = this.SerializeModel(model);
        Assert.Equal("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<Schema Namespace=\"MyNS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">\r\n  <TypeDefinition Name=\"UInt16\" UnderlyingType=\"Edm.Int32\" />\r\n  <EntityType Name=\"Person\">\r\n    <Key>\r\n      <PropertyRef Name=\"ID\" />\r\n    </Key>\r\n    <Property Name=\"ID\" Type=\"MyNS.UInt16\" />\r\n  </EntityType>\r\n</Schema>", outputText);
    }

    [Fact]
    public void Validate_BuildModelWithGetUInt32_SerializesCorrectly()
    {
        var model = new EdmModel();

        var personType = new EdmEntityType("MyNS", "Person");
        personType.AddKeys(personType.AddStructuralProperty("ID", model.GetUInt32("MyNS", true)));
        model.AddElement(personType);

        string outputText = this.SerializeModel(model);
        Assert.Equal("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<Schema Namespace=\"MyNS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">\r\n  <TypeDefinition Name=\"UInt32\" UnderlyingType=\"Edm.Int64\" />\r\n  <EntityType Name=\"Person\">\r\n    <Key>\r\n      <PropertyRef Name=\"ID\" />\r\n    </Key>\r\n    <Property Name=\"ID\" Type=\"MyNS.UInt32\" />\r\n  </EntityType>\r\n</Schema>", outputText);
    }

    [Fact]
    public void Validate_BuildModelWithGetUInt64_SerializesCorrectly()
    {
        var model = new EdmModel();

        var personType = new EdmEntityType("MyNS", "Person");
        personType.AddKeys(personType.AddStructuralProperty("ID", model.GetUInt64("MyNS", true)));
        model.AddElement(personType);

        string outputText = this.SerializeModel(model);
        Assert.Equal("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<Schema Namespace=\"MyNS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">\r\n  <TypeDefinition Name=\"UInt64\" UnderlyingType=\"Edm.Decimal\" />\r\n  <EntityType Name=\"Person\">\r\n    <Key>\r\n      <PropertyRef Name=\"ID\" />\r\n    </Key>\r\n    <Property Name=\"ID\" Type=\"MyNS.UInt64\" Scale=\"variable\" />\r\n  </EntityType>\r\n</Schema>", outputText);
    }

    [Fact]
    public void Validate_SetAndGetPrimitiveValueConverter_WorksAsExpected()
    {
        var model = new EdmModel();
        var typeReference = new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "Test", EdmPrimitiveTypeKind.Int32), true);
        var converter = new TestPrimitiveValueConverter();
        model.SetPrimitiveValueConverter(typeReference, converter);

        Assert.Equal(model.GetPrimitiveValueConverter(typeReference), converter);
    }

    [Fact]
    public void Validate_SetPrimitiveValueConverterWithNullConverter_ThrowsArgumentNullException()
    {
        var model = new EdmModel();
        var typeReference = new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "Test", EdmPrimitiveTypeKind.Int32), true);

        var exception = Assert.Throws<ArgumentNullException>(() => model.SetPrimitiveValueConverter(typeReference, null));
        Assert.Equal("Value cannot be null. (Parameter 'converter')", exception.Message);
    }

    [Fact]
    public void Validate_SetPrimitiveValueConverterWithNullModel_ThrowsArgumentNullException()
    {
        EdmModel? model = null;
        var typeReference = new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "Test", EdmPrimitiveTypeKind.Int32), true);
        var converter = new TestPrimitiveValueConverter();

        var exception = Assert.Throws<ArgumentNullException>(() => model.SetPrimitiveValueConverter(typeReference, converter));
        Assert.Equal("Value cannot be null. (Parameter 'model')", exception.Message);
    }

    [Fact]
    public void Validate_SetPrimitiveValueConverterWithNullTypeReference_ThrowsArgumentNullException()
    {
        var model = new EdmModel();
        var converter = new TestPrimitiveValueConverter();

        var exception = Assert.Throws<ArgumentNullException>(() => model.SetPrimitiveValueConverter((IEdmTypeDefinitionReference)null, converter));
        Assert.Equal("Value cannot be null. (Parameter 'typeDefinition')", exception.Message);
    }

    [Fact]
    public void Validate_GetPrimitiveValueConverterWithNullModel_ThrowsArgumentNullException()
    {
        EdmModel? model = null;
        var typeReference = new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "Test", EdmPrimitiveTypeKind.Int32), true);

        Assert.Throws<ArgumentNullException>(() => model.GetPrimitiveValueConverter(typeReference));
    }

    [Fact]
    public void Validate_GetPrimitiveValueConverterWithNullTypeReference_ReturnsPassThroughConverter()
    {
        EdmModel model = new EdmModel();
        var converter = model.GetPrimitiveValueConverter((IEdmTypeReference)null);

        Assert.IsType<PassThroughPrimitiveValueConverter>(converter);
        Assert.Equal(converter, PassThroughPrimitiveValueConverter.Instance);
    }

    [Fact]
    public void Validate_GetPrimitiveValueConverterWithNonTypeDefinition_ReturnsPassThroughConverter()
    {
        EdmModel model = new EdmModel();
        var converter = model.GetPrimitiveValueConverter(EdmCoreModel.Instance.GetInt32(true));

        Assert.IsType<PassThroughPrimitiveValueConverter>(converter);
        Assert.Equal(converter, PassThroughPrimitiveValueConverter.Instance);
    }

    [Fact]
    public void Validate_GetUIntWithNullModel_ThrowsArgumentNullException()
    {
        EdmModel? model = null;
        var exception = Assert.Throws<ArgumentNullException>(() => model.GetUInt16("MyNS", false));
        Assert.Equal("Value cannot be null. (Parameter 'model')", exception.Message);
    }

    [Fact]
    public void Validate_GetUIntWithNullNamespace_ThrowsArgumentNullException()
    {
        EdmModel model = new EdmModel();
        var exception = Assert.Throws<ArgumentNullException>(() => model.GetUInt16(null, true));
        Assert.Equal("Value cannot be null. (Parameter 'namespaceName')", exception.Message);
    }

    #region referenced model

    [Fact]
    public void Validate_FindEntitySetInExtendedContainer_WithEdmModelReference_ReturnsExpectedResults()
    {
        string mainModelxml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"">
    <edmx:Include Namespace=""Namespace1"" Alias=""A1"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""Namespace0"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">  
        <EntityContainer Name=""DefaultContainer0"" Extends=""Namespace1.Container_sub"">
        </EntityContainer>  
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        EdmModel model1 = new EdmModel();
        EdmEntityType entityType1 = new EdmEntityType("Namespace1", "EntityType");
        EdmEntityContainer edmEntityContainer1 = new EdmEntityContainer("Namespace1", "Container_sub");
        edmEntityContainer1.AddEntitySet("EntitySet1", entityType1);
        model1.AddElement(edmEntityContainer1);

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(mainModelxml)), new IEdmModel[] { model1 }, out IEdmModel mainModel, out IEnumerable<EdmError> errors);
        Assert.True(parsed);

        IEdmEntitySet entitySet1 = mainModel.FindDeclaredEntitySet("EntitySet1");
        IEdmEntitySet entitySet1FromContainer = mainModel.EntityContainer.FindEntitySetExtended("EntitySet1");

        Assert.NotNull(entitySet1);
        Assert.Equal(entitySet1, entitySet1FromContainer);
        Assert.Equal("Namespace1.Container_sub", entitySet1.Container.FullName());
    }

    [Fact]
    public void Validate_FindEntitySetInExtendedContainer_WithCsdlReference_ReturnsExpectedResults()
    {
        string mainModelxml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"">
    <edmx:Include Namespace=""Namespace1"" Alias=""A1"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""Namespace0"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">  
        <EntityContainer Name=""DefaultContainer0"" Extends=""Namespace1.Container_sub"">
        </EntityContainer>  
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
        string model1xml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">  
        <EntityType Name=""VipCustomer"">
            <Key>
            <PropertyRef Name=""VipCustomerID"" />
            </Key>
            <Property Name=""VipCustomerID"" Type=""Edm.String"" Nullable=""false"" />
        </EntityType>
        <EntityContainer Name=""Container_sub"">
            <EntitySet Name=""EntitySet1"" EntityType=""Namespace1.VipCustomer"" />
        </EntityContainer>  
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(model1xml)), out IEdmModel model1, out IEnumerable<EdmError> errors);
        Assert.True(parsed);

        parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(mainModelxml)), new IEdmModel[] { model1 }, out IEdmModel mainModel, out errors);
        Assert.True(parsed);

        IEdmEntitySet entitySet1 = mainModel.FindDeclaredEntitySet("EntitySet1");
        IEdmEntitySet entitySet1FromContainer = mainModel.EntityContainer.FindEntitySetExtended("EntitySet1");

        Assert.NotNull(entitySet1);
        Assert.Equal(entitySet1, entitySet1FromContainer);
        Assert.NotNull(entitySet1);
        Assert.Equal("Namespace1.Container_sub", entitySet1.Container.FullName());
    }

    [Fact]
    public void Validate_FindSingletonInExtendedContainer_ReturnsExpectedResults()
    {
        string mainModelxml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"">
    <edmx:Include Namespace=""Namespace1"" Alias=""A1"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""Namespace0"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityContainer Name=""DefaultContainer0"" Extends=""Namespace1.Container_sub"">
        </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
        string model1xml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">  
        <EntityType Name=""VipCustomer"">
            <Key>
            <PropertyRef Name=""VipCustomerID"" />
            </Key>
            <Property Name=""VipCustomerID"" Type=""Edm.String"" Nullable=""false"" />
        </EntityType>
        <EntityContainer Name=""Container_sub"">
            <Singleton Name=""Singleton1"" Type=""Namespace1.VipCustomer"" />
        </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(model1xml)), out IEdmModel model1, out IEnumerable<EdmError> errors);
        Assert.True(parsed);

        parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(mainModelxml)), new IEdmModel[] { model1 }, out IEdmModel mainModel, out errors);
        Assert.True(parsed);

        IEdmSingleton singleton1 = mainModel.FindDeclaredSingleton("Singleton1");
        IEdmSingleton singleton1FromContainer = mainModel.EntityContainer.FindSingletonExtended("Singleton1");

        Assert.NotNull(singleton1);
        Assert.Equal(singleton1, singleton1FromContainer);
        Assert.Equal("Namespace1.Container_sub", singleton1.Container.FullName());
    }

    [Fact]
    public void Validate_FindOperationImportInExtendedContainer_ReturnsExpectedResults()
    {
        string mainModelxml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"">
    <edmx:Include Namespace=""Namespace1"" Alias=""A1"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""Namespace0"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityContainer Name=""DefaultContainer0"" Extends=""Namespace1.Container_sub"">
        </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
        string model1xml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">  
        <Function Name=""Function1"">
            <ReturnType Type=""Edm.Int16"" />
        </Function>
        <EntityContainer Name=""Container_sub"">
            <FunctionImport Name=""FunctionImport1"" Function=""Namespace1.Function1""/>
        </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(model1xml)), out IEdmModel model1, out IEnumerable<EdmError> errors);
        Assert.True(parsed);

        parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(mainModelxml)), new IEdmModel[] { model1 }, out IEdmModel mainModel, out errors);
        Assert.True(parsed);

        IEnumerable<IEdmOperationImport> operationImports1 = mainModel.FindDeclaredOperationImports("FunctionImport1");
        IEdmOperationImport operationImport1 = operationImports1.Single();
        Assert.Equal("FunctionImport1", operationImport1.Name);
    }

    [Fact]
    public void Validate_FindEntitySetInExtendedContainerWithCircularReference_ThrowsInvalidOperationException()
    {
        string mainModelxml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"">
    <edmx:Include Namespace=""Namespace1"" Alias=""A1"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""Namespace0"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">  
        <EntityContainer Name=""DefaultContainer0"" Extends=""Namespace1.Container_sub"">
        </EntityContainer>  
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
        string model1xml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">  
        <EntityType Name=""VipCustomer"">
            <Key>
            <PropertyRef Name=""VipCustomerID"" />
            </Key>
            <Property Name=""VipCustomerID"" Type=""Edm.String"" Nullable=""false"" />
        </EntityType>
        <EntityContainer Name=""Container_sub"" Extends=""Container_sub"">
            <EntitySet Name=""EntitySet1_hdskfhdslfhssdfhdshdssfsa"" EntityType=""Namespace1.VipCustomer"" />
        </EntityContainer>  
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(model1xml)), out IEdmModel model1, out IEnumerable<EdmError> errors);
        Assert.True(parsed);

        parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(mainModelxml)), new IEdmModel[] { model1 }, out IEdmModel mainModel, out errors);
        Assert.True(parsed);

        Action action = (() => mainModel.FindDeclaredEntitySet("EntitySet1"));
        var exception1 = Assert.Throws<InvalidOperationException>(() => mainModel.FindDeclaredEntitySet("EntitySet1"));
        Assert.Equal("The entity container 'Namespace1.Container_sub' is invalid because its extends hierarchy is cyclic.", exception1.Message);

        var exception2 = Assert.Throws<InvalidOperationException>(() => mainModel.EntityContainer.FindEntitySetExtended("EntitySet1"));
        Assert.Equal("The entity container 'Namespace1.Container_sub' is invalid because its extends hierarchy is cyclic.", exception2.Message);
    }

    [Fact]
    public void Validate_AllElementsInExtendedContainer_ReturnsExpectedResults()
    {
        string mainModelxml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"">
    <edmx:Include Namespace=""Namespace1"" Alias=""A1"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""Namespace0"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">  
        <EntityContainer Name=""DefaultContainer0"" Extends=""Namespace1.Container_sub"">
        </EntityContainer>  
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        EdmModel model1 = new EdmModel();
        EdmEntityType entityType1 = new EdmEntityType("Namespace1", "EntityType");
        EdmEntityContainer edmEntityContainer1 = new EdmEntityContainer("Namespace1", "Container_sub");
        edmEntityContainer1.AddEntitySet("EntitySet1", entityType1);
        model1.AddElement(edmEntityContainer1);

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(mainModelxml)), new IEdmModel[] { model1 }, out IEdmModel mainModel, out IEnumerable<EdmError> errors);
        Assert.True(parsed);

        var container = mainModel.EntityContainer;
        Assert.Single(container.AllElements());
    }

    [Fact]
    public void Validate_AllElementsWithCircularReference_ThrowsInvalidOperationException()
    {
        string mainModelxml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx""> 
  <edmx:DataServices>
    <Schema Namespace=""Namespace0"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">  
        <EntityContainer Name=""Container"" Extends=""Namespace0.Container"" />
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";


        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(mainModelxml)), out IEdmModel mainModel, out IEnumerable<EdmError> errors);
        Assert.True(parsed);

        var container = mainModel.EntityContainer;

        var exception = Assert.Throws<InvalidOperationException>(() => container.AllElements());
        Assert.Equal("The entity container 'Namespace0.Container' is invalid because its extends hierarchy is cyclic.", exception.Message);
    }
    #endregion

    #region Private methods

    private IEdmModel GetEdmModel()
    {
        var stringType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String);

        var model = new EdmModel();
        var addressType = new EdmComplexType("NS1", "Address");
        addressType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
        addressType.AddStructuralProperty("City", new EdmStringTypeReference(stringType, /*isNullable*/false, /*isUnbounded*/false, /*maxLength*/30, /*isUnicode*/true));
        model.AddElement(addressType);

        var zipCodeType = new EdmComplexType("NS1", "ZipCode");
        zipCodeType.AddStructuralProperty("Main", new EdmStringTypeReference(stringType, /*isNullable*/false, /*isUnbounded*/false, /*maxLength*/5, /*isUnicode*/false));
        zipCodeType.AddStructuralProperty("Extended", new EdmStringTypeReference(stringType, /*isNullable*/true, /*isUnbounded*/false, /*maxLength*/5, /*isUnicode*/false));
        model.AddElement(zipCodeType);
        addressType.AddStructuralProperty("Zip", new EdmComplexTypeReference(zipCodeType, false));

        var foreignAddressType = new EdmComplexType("NS1", "ForeignAddress", addressType, false);
        foreignAddressType.AddStructuralProperty("State", EdmPrimitiveTypeKind.String);
        model.AddElement(foreignAddressType);

        var personType = new EdmEntityType("NS1", "Person", null, true, false);
        personType.AddKeys(personType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false));
        model.AddElement(personType);

        var customerType = new EdmEntityType("NS1", "Customer", personType);
        customerType.AddStructuralProperty("IsVIP", EdmPrimitiveTypeKind.Boolean);
        customerType.AddProperty(new EdmStructuralProperty(customerType, "LastUpdated", EdmCoreModel.Instance.GetDateTimeOffset(false), null));
        customerType.AddStructuralProperty("BillingAddress", new EdmComplexTypeReference(addressType, false));
        customerType.AddStructuralProperty("ShippingAddress", new EdmComplexTypeReference(addressType, false));
        model.AddElement(customerType);

        var orderType = new EdmEntityType("NS1", "Order");
        orderType.AddKeys(orderType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false));
        var customerIdProperty = orderType.AddStructuralProperty("CustomerId", EdmPrimitiveTypeKind.Int32, false);
        model.AddElement(orderType);

        var navProp1 = new EdmNavigationPropertyInfo { Name = "ToOrders", Target = orderType, TargetMultiplicity = EdmMultiplicity.Many, };
        var navProp2 = new EdmNavigationPropertyInfo { Name = "ToCustomer", Target = customerType, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { customerIdProperty }, PrincipalProperties = customerType.Key() };
        customerType.AddBidirectionalNavigation(navProp1, navProp2);

        var container = new EdmEntityContainer("NS1", "MyContainer");
        container.AddEntitySet("PersonSet", personType);
        container.AddEntitySet("OrderSet", orderType);
        model.AddElement(container);

        var function = new EdmFunction("NS1", "Function1", EdmCoreModel.Instance.GetInt64(true));
        function.AddParameter("Param1", EdmCoreModel.Instance.GetInt32(true));
        container.AddFunctionImport(function);
        model.AddElement(function);

        return model;
    }

    private string SerializeModel(IEdmModel model)
    {
        StringWriter sw = new StringWriter();
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.Encoding = System.Text.Encoding.UTF8;
        XmlWriter xw = XmlWriter.Create(sw, settings);
        model.TryWriteSchema(xw, out IEnumerable<EdmError> errors);
        xw.Flush();
        xw.Close();

        return sw.ToString();
    }

    private class TestPrimitiveValueConverter : IPrimitiveValueConverter
    {
        public object ConvertToUnderlyingType(object value)
        {
            return value;
        }

        public object ConvertFromUnderlyingType(object value)
        {
            return value;
        }
    }

    #endregion
}