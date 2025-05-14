//---------------------------------------------------------------------
// <copyright file="FindMethodTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.E2E.Tests.StubEdm;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class FindMethodTests : EdmLibTestCaseBase
{
    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodsTestValidNameCheckModel(EdmVersion edmVersion)
    {
        var namespaces = new string[]
        {
            new string('\u2160', 512),
            "\u1F98\u0660\u0300\u0903\u2040",
            "a"
        };

        var identifiers = new string[]
        {
            "Cc\u1F98\u1F98\u06E5\u01C03c\u1F98\u0660\u0300\u0903\u2040",
             new string('\u2160', 480),
             "a1"
        };

        var model = new EdmModel();
        foreach (var identifier in identifiers)
        {
            var type = new EdmEntityType(namespaces[0], identifier);
            type.AddKeys(type.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false));
            model.AddElement(type);

            var complexType = new EdmComplexType(namespaces[2], identifier);
            type.AddStructuralProperty("ComplexProperty", new EdmComplexTypeReference(complexType, false));
            model.AddElement(complexType);
        }

        IEnumerable<XElement> csdlElements = this.GetSerializerResult(model).Select(XElement.Parse);
        csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        IEnumerable<EdmError> serializationErrors;
        var testCsdls = this.GetSerializerResult(testModelConstructible, out serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodsTestMultipleSchemasWithDifferentNamespacesCyclicInvalid(EdmVersion edmVersion)
    {
        var csdls = new[]
            {
                @"<Schema Namespace=""FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespacesCyclicInvalid.third"" p1:UseStrongSpatialTypes=""false"" xmlns:p1=""http://schemas.microsoft.com/ado/2009/02/edm/annotation"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""VALIDeNTITYtYPE1"" BaseType=""FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespacesCyclicInvalid.second.VALIDeNTITYtYPE2"" />
  <ComplexType Name=""V1alidcomplexType"">
    <Property Name=""aPropertyOne"" Type=""FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespacesCyclicInvalid.second.V1alidcomplexType"" Nullable=""false"" />
  </ComplexType>
</Schema>",
                @"<Schema Namespace=""FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespacesCyclicInvalid.second"" p1:UseStrongSpatialTypes=""false"" xmlns:p1=""http://schemas.microsoft.com/ado/2009/02/edm/annotation"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""VALIDeNTITYtYPE2"" BaseType=""FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespacesCyclicInvalid.first.validEntityType1"" />
  <ComplexType Name=""V1alidcomplexType"">
    <Property Name=""aPropertyOne"" Type=""FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespacesCyclicInvalid.first.V1alidcomplexType"" Nullable=""false"" />
  </ComplexType>
</Schema>",
                @"<Schema Namespace=""FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespacesCyclicInvalid.first"" p1:UseStrongSpatialTypes=""false"" xmlns:p1=""http://schemas.microsoft.com/ado/2009/02/edm/annotation"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""validEntityType1"" BaseType=""FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespacesCyclicInvalid.third.VALIDeNTITYtYPE1"" />
  <ComplexType Name=""V1alidcomplexType"">
    <Property Name=""aPropertyOne"" Type=""FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespacesCyclicInvalid.third.V1alidcomplexType"" Nullable=""false"" />
  </ComplexType>
</Schema>"
            };

        IEnumerable<XElement> csdlElements = csdls.Select(XElement.Parse);
        csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        IEnumerable<EdmError> serializationErrors;
        var testCsdls = this.GetSerializerResult(testModelConstructible, out serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible);
    }


    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodsTestMultipleSchemasWithDifferentNamespaces(EdmVersion edmVersion)
    {
        var namespaces = new string[]
                {
                    "FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespaces.first",
                    "FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespaces.second"
                };

        var model = new EdmModel();
        foreach (var namespaceName in namespaces)
        {
            var entityType1 = new EdmEntityType(namespaceName, "validEntityType1");
            entityType1.AddKeys(entityType1.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            var entityType2 = new EdmEntityType(namespaceName, "VALIDeNTITYtYPE2");
            entityType2.AddKeys(entityType2.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            var entityType3 = new EdmEntityType(namespaceName, "VALIDeNTITYtYPE3");
            entityType3.AddKeys(entityType3.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));

            entityType1.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "Mumble", Target = entityType2, TargetMultiplicity = EdmMultiplicity.Many });

            var complexType = new EdmComplexType(namespaceName, "ValidNameComplexType1");
            complexType.AddStructuralProperty("aPropertyOne", new EdmComplexTypeReference(complexType, false));
            model.AddElements(new IEdmSchemaElement[] { entityType1, entityType2, entityType3, complexType });

            var function1 = new EdmFunction(namespaceName, "ValidFunction1", EdmCoreModel.Instance.GetSingle(false));
            var function2 = new EdmFunction(namespaceName, "ValidFunction1", EdmCoreModel.Instance.GetSingle(false));
            function2.AddParameter("param1", new EdmEntityTypeReference(entityType1, false));
            var function3 = new EdmFunction(namespaceName, "ValidFunction1", EdmCoreModel.Instance.GetSingle(false));
            function3.AddParameter("param1", EdmCoreModel.Instance.GetSingle(false));

            model.AddElements(new IEdmSchemaElement[] { function1, function2, function3 });
        }

        IEnumerable<XElement> csdlElements = this.GetSerializerResult(model).Select(XElement.Parse);
        csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        IEnumerable<EdmError> serializationErrors;
        var testCsdls = this.GetSerializerResult(testModelConstructible, out serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodsTestMultipleSchemasWithSameNamespace(EdmVersion edmVersion)
    {
        var csdls = new[] {
                #region MultipleSchemasWithSameNamespace CSDL
                @"<Schema Namespace=""FindMethodsTestModelBuilder.MultipleSchemasWithSameNamespace"" p1:UseStrongSpatialTypes=""false"" xmlns:p1=""http://schemas.microsoft.com/ado/2009/02/edm/annotation"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""validEntityType1"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <NavigationProperty Name=""Grumble"" Type=""Collection(FindMethodsTestModelBuilder.MultipleSchemasWithSameNamespace.VALIDeNTITYtYPE2)""/>
  </EntityType>
  <EntityType Name=""VALIDeNTITYtYPE2"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
  </EntityType>
  <ComplexType Name=""ValidNameComplexType1"">
    <Property Name=""aPropertyOne"" Type=""FindMethodsTestModelBuilder.MultipleSchemasWithSameNamespace.ValidNameComplexType1"" Nullable=""false"" />
  </ComplexType>
  <Function Name=""ValidFunction1"">
    <ReturnType Type=""Single"" Nullable=""false"" />
  </Function>
  <Function Name=""ValidFunction1"">
    <ReturnType Type=""Single"" Nullable=""false"" />
    <Parameter Name=""param1"" Type=""FindMethodsTestModelBuilder.MultipleSchemasWithSameNamespace.validEntityType1"" />
  </Function>
  <Function Name=""ValidFunction1"">
    <ReturnType Type=""Single"" Nullable=""false"" />
    <Parameter Name=""param1"" Type=""Single"" Nullable=""false"" />
  </Function>
</Schema>", @"<Schema Namespace=""FindMethodsTestModelBuilder.MultipleSchemasWithSameNamespace"" p1:UseStrongSpatialTypes=""false"" xmlns:p1=""http://schemas.microsoft.com/ado/2009/02/edm/annotation"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityContainer Name=""vALIDeNTITYcONTAINER2"">
    <EntitySet Name=""AValidEntitySet2"" EntityType=""FindMethodsTestModelBuilder.MultipleSchemasWithSameNamespace.VALIDeNTITYtYPE2"" />
  </EntityContainer>
  <EntityType Name=""validEntityTypeA"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <NavigationProperty Name=""Mumble"" Type=""Collection(FindMethodsTestModelBuilder.MultipleSchemasWithSameNamespace.VALIDeNTITYtYPEB)"" />
  </EntityType>
  <EntityType Name=""VALIDeNTITYtYPEB"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
  </EntityType>
  <ComplexType Name=""ValidNameComplexTypeA"">
    <Property Name=""aPropertyOne"" Type=""FindMethodsTestModelBuilder.MultipleSchemasWithSameNamespace.ValidNameComplexTypeA"" Nullable=""false"" />
  </ComplexType>
  <Function Name=""ValidFunctionA"">
    <ReturnType Type=""Single"" Nullable=""false"" />
  </Function>
  <Function Name=""ValidFunctionA"">
    <ReturnType Type=""Single"" Nullable=""false"" />
    <Parameter Name=""param1"" Type=""FindMethodsTestModelBuilder.MultipleSchemasWithSameNamespace.validEntityTypeA"" />
  </Function>
  <Function Name=""ValidFunctionA"">
    <ReturnType Type=""Single"" Nullable=""false"" />
    <Parameter Name=""param1"" Type=""Single"" Nullable=""false"" />
  </Function>
</Schema>",
                #endregion
            };


        IEnumerable<XElement> csdlElements = csdls.Select(XElement.Parse);
        csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        IEnumerable<EdmError> serializationErrors;
        var testCsdls = this.GetSerializerResult(testModelConstructible, out serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible);
    }

    [Fact]
    public void FindSchemaTypeForNoContentModelWithNonExistingSchema()
    {
        var testData = new[] { XElement.Parse(@"
<Schema Namespace=""DefaultNamespace"" p1:UseStrongSpatialTypes=""false"" xmlns:p1=""http://schemas.microsoft.com/ado/2009/02/edm/annotation"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"" />") };

        bool parsed = SchemaReader.TryParse(
            from xElement in testData select xElement.CreateReader(),
            out IEdmModel testModel,
            out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);
        Assert.Null(testModel.FindType("NonExistSchema"));
    }

    [Fact]
    public void FindSchemaTypeForPrimitiveTypes()
    {
        var namespaces = new string[]
                {
                    new string('\u2160', 512),
                    "\u1F98\u0660\u0300\u0903\u2040",
                    "a"
                };

        var identifiers = new string[]
            {
                    "Cc\u1F98\u1F98\u06E5\u01C03c\u1F98\u0660\u0300\u0903\u2040",
                     new string('\u2160', 480),
                     "a1"
            };

        var model = new EdmModel();
        foreach (var identifier in identifiers)
        {
            var type = new EdmEntityType(namespaces[0], identifier);
            type.AddKeys(type.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false));
            model.AddElement(type);

            var complexType = new EdmComplexType(namespaces[2], identifier);
            type.AddStructuralProperty("ComplexProperty", new EdmComplexTypeReference(complexType, false));
            model.AddElement(complexType);
        }

        var testData = this.GetSerializerResult(model).Select(XElement.Parse);

        bool parsed = SchemaReader.TryParse(
            from xElement in testData select xElement.CreateReader(),
            out IEdmModel testModel,
            out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);
        Assert.Null(testModel.FindDeclaredType("Edm.Int32"));
        Assert.NotNull(testModel.FindType("Edm.Int32"));
    }

    [Fact]
    public void FindMethodTestAssociationCompositeFk()
    {
        this.BasicFindMethodsTest(ModelBuilder.AssociationCompositeFkEdm());
    }

    [Fact]
    public void FindMethodTestAssociationFk()
    {
        this.BasicFindMethodsTest(ModelBuilder.AssociationFkEdm());
    }

    [Fact]
    public void FindMethodTestAssociationFkWithNavigation()
    {
        this.BasicFindMethodsTest(ModelBuilder.AssociationFkWithNavigationEdm());
    }

    [Fact]
    public void FindMethodTestAssociationIndependent()
    {
        this.BasicFindMethodsTest(ModelBuilder.AssociationIndependentEdm());
    }

    [Fact]
    public void FindMethodTestAssociationOnDeleteModel()
    {
        this.BasicFindMethodsTest(ModelBuilder.AssociationOnDeleteModelEdm());
    }

    [Fact]
    public void FindMethodTestCollectionTypes()
    {
        var testCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(ModelBuilder.CollectionTypes().ToArray(), this.EdmVersion);
        var testModelImmutable = this.GetParserResult(testCsdls);

        this.VerifyFindSchemaElementMethod(testCsdls.Single(), testModelImmutable);
        this.VerifyFindEntityContainer(testCsdls.Single(), testModelImmutable);
        this.VerifyFindEntityContainerElement(testCsdls.Single(), testModelImmutable);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        testCsdls = this.GetSerializerResult(testModelConstructible).Select(n => XElement.Parse(n));

        this.VerifyFindSchemaElementMethod(testCsdls.Single(), testModelConstructible);
        this.VerifyFindEntityContainer(testCsdls.Single(), testModelConstructible);
        this.VerifyFindEntityContainerElement(testCsdls.Single(), testModelConstructible);
    }

    [Fact]
    public void FindMethodTestCollectionTypesWithSimpleType()
    {
        this.BasicFindMethodsTest(ModelBuilder.CollectionTypesWithSimpleType());
    }

    [Fact]
    public void FindMethodTestComplexTypeAttributes()
    {
        this.BasicFindMethodsTest(ModelBuilder.ComplexTypeAttributesEdm());
    }

    [Fact]
    public void FindMethodTestEntityContainerAttributes()
    {
        this.BasicFindMethodsTest(ModelBuilder.EntityContainerAttributes());
    }

    [Fact]
    public void FindMethodTestEntityContainerWithEntitySets()
    {
        this.BasicFindMethodsTest(ModelBuilder.EntityContainerWithEntitySetsEdm());
    }

    [Fact]
    public void FindMethodTestEntityContainerWithFunctionImports()
    {
        this.BasicFindMethodsTest(ModelBuilder.EntityContainerWithOperationImportsEdm());
    }

    [Fact]
    public void FindMethodTestEntityInheritanceTree()
    {
        this.BasicFindMethodsTest(ModelBuilder.EntityInheritanceTreeEdm());
    }

    [Fact]
    public void FindMethodTestFunctionWithAll()
    {
        this.BasicFindMethodsTest(ModelBuilder.FunctionWithAllEdm());
    }

    [Fact]
    public void FindMethodTestModelWithAllConcepts()
    {
        this.BasicFindMethodsTest(ModelBuilder.ModelWithAllConceptsEdm());
    }

    [Fact]
    public void FindMethodTestMultipleNamespaces()
    {
        this.BasicFindMethodsTest(ModelBuilder.MultipleNamespacesEdm());
    }

    [Fact]
    public void FindMethodTestOneComplexWithAllPrimitiveProperty()
    {
        this.BasicFindMethodsTest(ModelBuilder.OneComplexWithAllPrimitivePropertyEdm());
    }

    [Fact]
    public void FindMethodTestOneComplexWithNestedComplex()
    {
        this.BasicFindMethodsTest(ModelBuilder.OneComplexWithNestedComplexEdm());
    }

    [Fact]
    public void FindMethodTestOneComplexWithOneProperty()
    {
        this.BasicFindMethodsTest(ModelBuilder.OneComplexWithOnePropertyEdm());
    }

    [Fact]
    public void FindMethodTestOneEntityWithAllPrimitiveProperty()
    {
        this.BasicFindMethodsTest(ModelBuilder.OneEntityWithAllPrimitivePropertyEdm());
    }

    [Fact]
    public void FindMethodTestOneEntityWithOneProperty()
    {
        this.BasicFindMethodsTest(ModelBuilder.OneEntityWithOnePropertyEdm());
    }

    [Fact]
    public void FindMethodTestPropertyFacetsCollection()
    {
        this.BasicFindMethodsTest(ModelBuilder.PropertyFacetsCollectionEdm());
    }

    [Fact]
    public void FindMethodTestSimpleAllPrimtiveTypes()
    {
        this.BasicFindMethodsTest(ModelBuilder.SimpleAllPrimitiveTypes(this.EdmVersion, true, true));
    }

    [Fact]
    public void FindMethodTestSimpleConstrucitveApiTestModel()
    {
        this.BasicFindMethodsTest(ModelBuilder.SimpleConstructiveApiTestModel());
    }

    [Fact]
    public void FindMethodTestStringWithFacets()
    {
        this.BasicFindMethodsTest(ModelBuilder.StringWithFacets());
    }

    [Fact]
    public void FindMethodTestTaupoDefaultModel()
    {
        this.BasicFindMethodsTest(ModelBuilder.TaupoDefaultModelEdm());
    }

    [Fact]
    // [EdmLib] FindProperty should return AmbiguousElementBinding when the model has duplicate properties
    public void FindMethodTestDuplicatePropertyName()
    {
        var edmModel = this.GetParserResult(ValidationTestModelBuilder.DuplicatePropertyName(this.EdmVersion));

        var complexType = edmModel.FindType("CollectionAtomic.ComplexTypeA") as IEdmComplexType;
        var property = complexType.FindProperty("Collection");
        Assert.Equal(property.Name, "Collection", "FindProperty should return AmbiguousElementBinding since the name of the property is duplicate.");

        var entityType = edmModel.FindType("CollectionAtomic.ComplexTypeE") as IEdmEntityType;
        property = complexType.FindProperty("Collection");
        Assert.Equal(property.Name, "Collection", "FindProperty should return AmbiguousElementBinding since the name of the property is duplicate.");
    }

    //[TestMethod, Variation(Id = 42, SkipReason = @"[EdmLib] When the name of an element is changed, the FindType method should fail when it is called with the old name. -- postponed")]
    public void FindMethodsTestForElementsAfterNameChange()
    {
        var model = new EdmModel();
        var complexType = new StubEdmComplexType("MyNamespace", "ComplexType1");
        model.AddElement(complexType);
        complexType.Name = "ComplexType2";

        Assert.True(model.SchemaElements.Any(n => n.FullName() == "MyNamespace.ComplexType2"), "There should be ComplexType2 type.");
        Assert.True(!model.SchemaElements.Any(n => n.FullName() == "MyNamespace.ComplexType1"), "There should be no ComplexType1 type.");
        Assert.Null(model.FindType("MyNamespace.ComplexType1"), "You should not be able to find the complex type with the old name");
        Assert.Equal(model.FindType("MyNamespace.ComplexType2").FullName(), "MyNamespace.ComplexType2", "You should be able to find the complex type with the new name, ComplexType1");
    }

    [Fact]
    public void FindMethodTestTermAndFunctionCsdl()
    {
        this.BasicFindMethodsTest(FindMethodsTestModelBuilder.TermAndFunctionCsdl());
    }

    [Fact]
    public void FindMethodTestFunctionOverloadingWithDifferentParametersCountCsdls()
    {
        var csdls = FindMethodsTestModelBuilder.FunctionOverloadingWithDifferentParametersCountCsdl();

        FunctionInfo simpleFunction1 = new FunctionInfo()
        {
            Name = "SimpleFunction",
            Parameters = new ParameterInfoList() { { "Person", "[Edm.String Nullable=True]" } }
        };

        FunctionInfo simpleFunction2 = new FunctionInfo()
        {
            Name = "SimpleFunction",
            Parameters = new ParameterInfoList() { { "Person", "[Edm.String Nullable=True]" },
                                                       { "Count", "[Edm.Int32 Nullable=True]" }}
        };

        List<FunctionInfo> functionInfos = new List<FunctionInfo>() { simpleFunction1, simpleFunction2 };

        FunctionOverloadingCheck(csdls, functionInfos, "DefaultNamespace");
    }

    [Fact]
    public void FindMethodTestFunctionImportOverloadingWithDifferentParametersCountCsdl()
    {
        var csdls = FindMethodsTestModelBuilder.FunctionImportOverloadingWithDifferentParametersCountCsdl();

        FunctionInfo simpleFunction1 = new FunctionInfo()
        {
            Name = "SimpleFunction",
            Parameters = new ParameterInfoList() { { "Person", "[Edm.Int32 Nullable=True]" } }
        };

        FunctionInfo simpleFunction2 = new FunctionInfo()
        {
            Name = "SimpleFunction",
            Parameters = new ParameterInfoList() { { "Person", "[Edm.Int32 Nullable=True]" },
                                                       { "Count", "[Edm.Int32 Nullable=True]" }}
        };

        List<FunctionInfo> functionInfos = new List<FunctionInfo>() { simpleFunction1, simpleFunction2 };

        FunctionImportOverloadingCheck(csdls, functionInfos, "Container");
    }

    [Fact]
    public void FindMethodTestFunctionImportOverloadingWithComplexParameterCsdl()
    {
        var csdls = FindMethodsTestModelBuilder.FunctionImportOverloadingWithComplexParameterCsdl();

        FunctionInfo simpleFunction1 = new FunctionInfo()
        {
            Name = "MyFunction",
            Parameters = new ParameterInfoList() { { "P1", "[Collection([DefaultNamespace.MyComplexType Nullable=True]) Nullable=True]" },
                                                       { "P2", "[DefaultNamespace.MyEntityType Nullable=True]" },
                                                       { "P3", "[Collection([DefaultNamespace.MyEntityType Nullable=True]) Nullable=True]" },
                                                       { "P4", "[Edm.Int32 Nullable=True]" }}
        };

        FunctionInfo simpleFunction2 = new FunctionInfo()
        {
            Name = "MyFunction",
            Parameters = new ParameterInfoList() { { "P1", "[Collection([DefaultNamespace.MyComplexType Nullable=True]) Nullable=True]" },
                                                       { "P2", "[DefaultNamespace.MyEntityType Nullable=True]" },
                                                       { "P3b", "[DefaultNamespace.MyComplexType Nullable=True]" },
                                                       { "P4", "[Edm.Int32 Nullable=True]" }}
        };

        List<FunctionInfo> functionInfos = new List<FunctionInfo>() { simpleFunction1, simpleFunction2 };

        FunctionImportOverloadingCheck(csdls, functionInfos, "Container");
    }

    [Fact]
    public void FindMethodTestFindFunctionImportThatDoesNotExist()
    {
        var csdls = FindMethodsTestModelBuilder.FunctionImportOverloadingWithComplexParameterCsdl();
        var model = this.GetParserResult(csdls);

        var entityContainer = model.FindEntityContainer("Container");
        var operationImports = entityContainer.FindOperationImports("foobaz");
        Assert.NotNull(operationImports, "Invalid find function import result.");
        Assert.Equal(0, operationImports.Count(), "Invalid function import count.");
    }

    [Fact]
    public void FindMethodTestEntitySetWithSingleNavigationCsdl()
    {
        var csdls = FindMethodsTestModelBuilder.EntitySetWithSingleNavigationCsdl();
        var model = this.GetParserResult(csdls);
        this.VerifySemanticValidation(model, new EdmLibTestErrors());

        var personToItem = model.FindEntityType("DefaultNamespace.Person").NavigationProperties().Where(n => n.Name.Equals("ItemPurchased")).FirstOrDefault();
        var itemToPerson = model.FindEntityType("DefaultNamespace.Item").NavigationProperties().Where(n => n.Name.Equals("Purchaser")).FirstOrDefault();

        var buyerSet = model.EntityContainer.FindEntitySet("BuyerSet");
        var itemSet = model.EntityContainer.FindEntitySet("ItemSet");

        FindNavigationTargetCheck(buyerSet, personToItem, itemSet, itemToPerson);
        this.BasicFindMethodsTest(csdls);
    }

    [Fact]
    public void FindMethodTestEntitySetWithTwoNavigationCsdl()
    {
        var csdls = FindMethodsTestModelBuilder.EntitySetWithTwoNavigationCsdl();
        var model = this.GetParserResult(csdls);
        this.VerifySemanticValidation(model, new EdmLibTestErrors());

        var personToItem = model.FindEntityType("DefaultNamespace.Person").NavigationProperties().Where(n => n.Name.Equals("ItemPurchased")).FirstOrDefault();
        var itemToPerson = model.FindEntityType("DefaultNamespace.Item").NavigationProperties().Where(n => n.Name.Equals("Purchaser")).FirstOrDefault();

        var ownerToPet = model.FindEntityType("DefaultNamespace.Person").NavigationProperties().Where(n => n.Name.Equals("Pet")).FirstOrDefault();
        var petToOwner = model.FindEntityType("DefaultNamespace.Pet").NavigationProperties().Where(n => n.Name.Equals("Owner")).FirstOrDefault();

        var buyerSet = model.EntityContainer.FindEntitySet("BuyerSet");
        var itemSet = model.EntityContainer.FindEntitySet("ItemSet");
        var petSet = model.EntityContainer.FindEntitySet("PetSet");

        FindNavigationTargetCheck(buyerSet, personToItem, itemSet, itemToPerson);
        FindNavigationTargetCheck(buyerSet, ownerToPet, petSet, petToOwner);
        this.BasicFindMethodsTest(csdls);
    }

    [Fact]
    public void FindMethodTestEntitySetRecursiveNavigationCsdl()
    {
        var csdls = FindMethodsTestModelBuilder.EntitySetRecursiveNavigationCsdl();
        var model = this.GetParserResult(csdls);
        this.VerifySemanticValidation(model, new EdmLibTestErrors());

        var personToFriend = model.FindEntityType("DefaultNamespace.Person").NavigationProperties().Where(n => n.Name.Equals("ToFriend")).FirstOrDefault();
        var friendToPerson = model.FindEntityType("DefaultNamespace.Person").NavigationProperties().Where(n => n.Name.Equals("ToPerson")).FirstOrDefault();

        var personSet = model.EntityContainer.FindEntitySet("PersonSet");

        FindRecursiveNavigationTargetCheck(personSet, personToFriend, friendToPerson);
        this.BasicFindMethodsTest(csdls);
    }

    [Fact]
    public void FindMethodTestEntitySetNavigationUsedTwiceCsdl()
    {
        var csdls = FindMethodsTestModelBuilder.EntitySetNavigationUsedTwiceCsdl();
        var model = this.GetParserResult(csdls);
        this.VerifySemanticValidation(model, new EdmLibTestErrors());

        var personToItem = model.FindEntityType("DefaultNamespace.Person").NavigationProperties().Where(n => n.Name.Equals("ItemPurchased")).FirstOrDefault();
        var itemToPerson = model.FindEntityType("DefaultNamespace.Item").NavigationProperties().Where(n => n.Name.Equals("Purchaser")).FirstOrDefault();

        var buyerSet = model.EntityContainer.FindEntitySet("BuyerSet");
        var itemSet = model.EntityContainer.FindEntitySet("ItemSet");
        var secondBuyerSet = model.EntityContainer.FindEntitySet("SecondBuyerSet");
        var secondItemSet = model.EntityContainer.FindEntitySet("SecondItemSet");

        FindNavigationTargetCheck(buyerSet, personToItem, itemSet, itemToPerson);
        FindNavigationTargetCheck(secondBuyerSet, personToItem, secondItemSet, itemToPerson);
    }

    [Fact]
    public void FindMethodTestFindVocabularyAnnotationWithTermCsdl()
    {
        var csdls = FindMethodsTestModelBuilder.FindVocabularyAnnotationWithTermCsdl();
        var model = this.GetParserResult(csdls);

        var modelVocabulary = model.VocabularyAnnotations;
        Assert.Equal(1, modelVocabulary.Count(), "Invalid vocabulary count.");

        var addressTerm = model.FindTerm("AnnotationNamespace.AddressObject");
        var petType = model.FindType("DefaultNamespace.Pet") as IEdmComplexType;
        this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(addressTerm), model.FindVocabularyAnnotations(addressTerm));
        Assert.Equal(0, model.FindDeclaredVocabularyAnnotations(addressTerm).Count(), "Invalid annotation count.");
        this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(petType), model.FindVocabularyAnnotations(petType));
        Assert.Equal(1, model.FindDeclaredVocabularyAnnotations(petType).Count(), "Invalid annotation count.");

        var valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(addressTerm, addressTerm);
        Assert.Equal(0, valueAnnotationsFound.Count(), "Invalid annotation count.");

        valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(petType, "AnnotationNamespace.Person");
        Assert.Equal(0, valueAnnotationsFound.Count(), "Invalid annotation count.");

        valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(petType, "AnnotationNamespace.Nonsense");
        Assert.Equal(0, valueAnnotationsFound.Count(), "Invalid annotation count.");

        valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(petType, addressTerm);
        Assert.Equal(1, valueAnnotationsFound.Count(), "Invalid annotation count.");

        valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(petType, "AnnotationNamespace.AddressObject");
        Assert.Equal(1, valueAnnotationsFound.Count(), "Invalid annotation count.");

        var expectedValueAnnotation = petType.VocabularyAnnotations(model).SingleOrDefault();
        Assert.AreSame(expectedValueAnnotation, valueAnnotationsFound.SingleOrDefault(), "Invalid annotation.");
    }

    [Fact]
    public void FindMethodTestFindVocabularyAnnotationWithEntityTypeCsdl()
    {
        var csdls = FindMethodsTestModelBuilder.FindVocabularyAnnotationWithEntityTypeCsdl();
        var model = this.GetParserResult(csdls);

        var modelVocabulary = model.VocabularyAnnotations;
        Assert.Equal(1, modelVocabulary.Count(), "Invalid vocabulary count.");

        var personEntity = model.FindEntityType("AnnotationNamespace.Person");
        var addressTerm = model.FindTerm("AnnotationNamespace.AddressObject");
        var petType = model.FindType("DefaultNamespace.Pet") as IEdmComplexType;
        this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(personEntity), model.FindVocabularyAnnotations(personEntity));
        Assert.Equal(0, model.FindDeclaredVocabularyAnnotations(personEntity).Count(), "Invalid annotation count.");
        this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(addressTerm), model.FindVocabularyAnnotations(addressTerm));
        Assert.Equal(0, model.FindDeclaredVocabularyAnnotations(addressTerm).Count(), "Invalid annotation count.");
        this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(petType), model.FindVocabularyAnnotations(petType));
        Assert.Equal(1, model.FindDeclaredVocabularyAnnotations(petType).Count(), "Invalid annotation count.");

        var valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(addressTerm, addressTerm);
        Assert.Equal(0, valueAnnotationsFound.Count(), "Invalid annotation count.");

        valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(petType, addressTerm);
        Assert.Equal(0, valueAnnotationsFound.Count(), "Invalid annotation count.");
    }

    [Fact]
    public void FindMethodTestFindVocabularyAnnotationWithComplexTypeCsdl()
    {
        var csdls = FindMethodsTestModelBuilder.FindVocabularyAnnotationWithComplexTypeCsdl();
        var model = this.GetParserResult(csdls);

        var modelVocabulary = model.VocabularyAnnotations;
        Assert.Equal(1, modelVocabulary.Count(), "Invalid vocabulary count.");

        var addressTerm = model.FindTerm("AnnotationNamespace.AddressObject");
        var petType = model.FindType("DefaultNamespace.Pet") as IEdmComplexType;
        this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(addressTerm), model.FindVocabularyAnnotations(addressTerm));
        this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(petType), model.FindVocabularyAnnotations(petType));

        var valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(addressTerm, addressTerm);
        Assert.Equal(0, valueAnnotationsFound.Count(), "Invalid annotation count.");

        valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(petType, addressTerm);
        Assert.Equal(0, valueAnnotationsFound.Count(), "Invalid annotation count.");
    }

    [Fact]
    public void FindMethodTestFindVocabularyAnnotationWithMultipleTermsCsdl()
    {
        var csdls = FindMethodsTestModelBuilder.FindVocabularyAnnotationWithMultipleTermsCsdl();
        var model = this.GetParserResult(csdls);

        var modelVocabulary = model.VocabularyAnnotations;
        Assert.Equal(3, modelVocabulary.Count(), "Invalid vocabulary count.");

        var personEntity = model.FindEntityType("AnnotationNamespace.Person");
        var addressTerm = model.FindTerm("AnnotationNamespace.AddressObject");
        var petType = model.FindType("DefaultNamespace.Pet") as IEdmComplexType;
        this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(personEntity), model.FindVocabularyAnnotations(personEntity));
        Assert.Equal(0, model.FindDeclaredVocabularyAnnotations(personEntity).Count(), "Invalid annotation count.");
        this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(addressTerm), model.FindVocabularyAnnotations(addressTerm));
        Assert.Equal(0, model.FindDeclaredVocabularyAnnotations(addressTerm).Count(), "Invalid annotation count.");
        this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(petType), model.FindVocabularyAnnotations(petType));
        Assert.Equal(3, model.FindDeclaredVocabularyAnnotations(petType).Count(), "Invalid annotation count.");
        var expectedValueAnnotation = modelVocabulary.Where(n => n.Term == addressTerm);

        var valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(addressTerm, addressTerm);
        Assert.Equal(0, valueAnnotationsFound.Count(), "Invalid annotation count.");

        valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(petType, addressTerm);
        Assert.Equal(2, valueAnnotationsFound.Count(), "Invalid annotation count.");
        Assert.AreSame(expectedValueAnnotation.ElementAt(0), valueAnnotationsFound.ElementAt(0), "Invalid annotation.");
        Assert.AreSame(expectedValueAnnotation.ElementAt(1), valueAnnotationsFound.ElementAt(1), "Invalid annotation.");
    }

    [Fact]
    public void FindMethodTestFindVocabularyAnnotationAcrossModelOutOfLineAnnotationCsdl()
    {
        var csdls = FindMethodsTestModelBuilder.FindVocabularyAnnotationAcrossModelOutOfLineAnnotationCsdl();
        var model = this.GetParserResult(csdls);
        Assert.Equal(3, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        var valueAnnotationType = model.FindType("DefaultNamespace.ValueAnnotationType");
        this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(valueAnnotationType), model.FindVocabularyAnnotations(valueAnnotationType));

        var valueAnnotationsFound = model.FindVocabularyAnnotations(valueAnnotationType);
        Assert.Equal(3, valueAnnotationsFound.Count(), "Invalid annotation count.");

        var valueAnnotationFoundCount = valueAnnotationsFound.Where(n => n.Term.Name.Equals("ValueTermInModel")).Count();
        Assert.Equal(1, valueAnnotationFoundCount, "Type annotation cannot be found.");

        valueAnnotationFoundCount = valueAnnotationsFound.Where(n => n.Term.Name.Equals("ValueTermOutOfModel")).Count();
        Assert.Equal(1, valueAnnotationFoundCount, "Type annotation cannot be found.");

        valueAnnotationFoundCount = valueAnnotationsFound.Where(n => n.Term.Name.Equals("ValueTermDoesNotExist")).Count();
        Assert.Equal(1, valueAnnotationFoundCount, "Type annotation cannot be found.");
    }

    [Fact]
    public void FindMethodTestFindVocabularyAnnotationAcrossModelInLineAnnotationCsdl()
    {
        var csdls = FindMethodsTestModelBuilder.FindVocabularyAnnotationAcrossModelInLineAnnotationCsdl();
        var model = this.GetParserResult(csdls);
        Assert.Equal(3, model.VocabularyAnnotations.Count(), "Invalid vocabulary annotation count.");

        var valueAnnotationType = model.FindType("DefaultNamespace.ValueAnnotationType");
        var personType = model.FindType("DefaultNamespace.PersonInMode");
        this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(valueAnnotationType), model.FindVocabularyAnnotations(valueAnnotationType));
        this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(personType), model.FindVocabularyAnnotations(personType));

        var anntationFound = model.FindVocabularyAnnotations(personType);
        Assert.Equal(0, anntationFound.Count(), "Invalid annotation count.");

        anntationFound = model.FindVocabularyAnnotations(valueAnnotationType);
        Assert.Equal(3, anntationFound.Count(), "Invalid annotation count.");

        var valueAnnotationsFound = model.FindVocabularyAnnotations(valueAnnotationType);
        Assert.Equal(3, valueAnnotationsFound.Count(), "Invalid annotation count.");

        var valueAnnotationFoundCount = valueAnnotationsFound.Where(n => n.Term.Name.Equals("ValueTermInModel")).Count();
        Assert.Equal(1, valueAnnotationFoundCount, "Type annotation cannot be found.");

        valueAnnotationFoundCount = valueAnnotationsFound.Where(n => n.Term.Name.Equals("ValueTermOutOfModel")).Count();
        Assert.Equal(1, valueAnnotationFoundCount, "Type annotation cannot be found.");

        valueAnnotationFoundCount = valueAnnotationsFound.Where(n => n.Term.Name.Equals("ValueTermDoesNotExist")).Count();
        Assert.Equal(1, valueAnnotationFoundCount, "Type annotation cannot be found.");
    }

    [Fact]
    public void FindMethodTestFindTermSingleCsdl()
    {
        var csdls = FindMethodsTestModelBuilder.FindTermCsdl();
        var model = this.GetParserResult(csdls);

        var valueTermInModel = model.FindTerm("DefaultNamespace.ValueTermInModel");
        Assert.NotNull(valueTermInModel, "Invalid term.");
        Assert.AreSame(valueTermInModel, model.FindTerm("DefaultNamespace.ValueTermInModel"), "Invalid term.");

        var ambiguousValueTerm = model.FindTerm("DefaultNamespace.AmbigousValueTerm");
        Assert.NotNull(ambiguousValueTerm, "Invalid ambiguous term.");
        Assert.Equal(true, ambiguousValueTerm.IsBad(), "Invalid IsBad value.");
        Assert.AreSame(ambiguousValueTerm, model.FindTerm("DefaultNamespace.AmbigousValueTerm"), "Invalid term.");

        var valueTermInOtherModel = model.FindTerm("AnnotationNamespace.ValueTerm");
        Assert.NotNull(valueTermInOtherModel, "Invalid term.");
        Assert.AreSame(valueTermInOtherModel, model.FindTerm("AnnotationNamespace.ValueTerm"), "Invalid term.");

        var valueTermDoesNotExist = model.FindTerm("fooNamespace.ValueTerm");
        Assert.Null(valueTermDoesNotExist, "Invalid term.");
    }

    [Fact]
    public void FindMethodTestFindTermAmbiguousReferences()
    {
        var model = FindMethodsTestModelBuilder.FindTermModel();
        var referencedModel = this.GetParserResult(FindMethodsTestModelBuilder.FindTermCsdl());
        model.AddReferencedModel(referencedModel);

        var valueTermInModel = model.FindTerm("DefaultNamespace.ValueTermInModel");
        Assert.NotNull(valueTermInModel, "Invalid term.");
        Assert.AreSame(valueTermInModel, referencedModel.FindTerm("DefaultNamespace.ValueTermInModel"), "Invalid term.");

        var secondValueTermInModel = model.FindTerm("DefaultNamespace.SecondValueTermInModel");
        Assert.NotNull(secondValueTermInModel, "Invalid term.");
        Assert.AreSame(secondValueTermInModel, model.FindTerm("DefaultNamespace.SecondValueTermInModel"), "Invalid term.");

        IEnumerable<EdmError> errors;
        model.Validate(out errors);
        Assert.AreNotEqual(0, errors.Count(), "Ambiguous error should occur.");

        var ambiguousValueTerm = model.FindTerm("DefaultNamespace.ReferenceAmbigousValueTerm");
        Assert.NotNull(ambiguousValueTerm, "Invalid ambiguous term.");
        Assert.Equal(true, ambiguousValueTerm.IsBad(), "Invalid IsBad value.");
    }

    [Fact]
    public void FindMethodTestFindTypeComplexTypeCsdl()
    {
        var csdls = FindMethodsTestModelBuilder.FindTypeComplexTypeCsdl();
        var model = this.GetParserResult(csdls);

        var simpleType = model.FindType("DefaultNamespace.SimpleType");
        Assert.NotNull(simpleType, "Invalid complex type.");
        Assert.AreSame(simpleType, model.FindType("DefaultNamespace.SimpleType"), "Invalid complex type.");

        var ambiguousType = model.FindType("DefaultNamespace.AmbiguousType");
        Assert.NotNull(ambiguousType, "Invalid ambiguous complex type.");
        Assert.Equal(true, ambiguousType.IsBad(), "Invalid IsBad value.");
        Assert.AreSame(ambiguousType, model.FindType("DefaultNamespace.AmbiguousType"), "Invalid complex type.");

        var simpleTypeInOtherModel = model.FindType("AnnotationNamespace.SimpleType");
        Assert.NotNull(simpleTypeInOtherModel, "Invalid complex type.");
        Assert.AreSame(simpleTypeInOtherModel, model.FindType("AnnotationNamespace.SimpleType"), "Invalid complex type.");

        var complexTypeDoesNotExist = model.FindType("fooNamespace.SimpleType");
        Assert.Null(complexTypeDoesNotExist, "Invalid complex type.");
    }

    [Fact]
    public void FindMethodTestFindTypeEntityTypeCsdl()
    {
        var csdls = FindMethodsTestModelBuilder.FindTypeEntityTypeCsdl();
        var model = this.GetParserResult(csdls);

        var simpleEntity = model.FindType("DefaultNamespace.SimpleEntity");
        Assert.NotNull(simpleEntity, "Invalid entity type.");
        Assert.AreSame(simpleEntity, model.FindType("DefaultNamespace.SimpleEntity"), "Invalid entity type.");

        var ambiguousEntity = model.FindType("DefaultNamespace.AmbiguousEntity");
        Assert.NotNull(ambiguousEntity, "Invalid ambiguous entity type.");
        Assert.Equal(true, ambiguousEntity.IsBad(), "Invalid IsBad value.");
        Assert.AreSame(ambiguousEntity, model.FindType("DefaultNamespace.AmbiguousEntity"), "Invalid entity type.");

        var simpleEntityInOtherModel = model.FindType("AnnotationNamespace.SimpleEntity");
        Assert.NotNull(simpleEntityInOtherModel, "Invalid entity type.");
        Assert.AreSame(simpleEntityInOtherModel, model.FindType("AnnotationNamespace.SimpleEntity"), "Invalid entity type.");

        var entityTypeDoesNotExist = model.FindType("fooNamespace.SimpleEntity");
        Assert.Null(entityTypeDoesNotExist, "Invalid entity type.");
    }

    [Fact]
    public void FindMethodTestFindTypeModel()
    {
        var model = FindMethodsTestModelBuilder.FindTypeModel();
        var referencedEntityTypeModel = this.GetParserResult(FindMethodsTestModelBuilder.FindTypeEntityTypeCsdl());
        var referencedComplexTypeModel = this.GetParserResult(FindMethodsTestModelBuilder.FindTypeComplexTypeCsdl());
        model.AddReferencedModel(referencedEntityTypeModel);
        model.AddReferencedModel(referencedComplexTypeModel);

        var secondSimpleType = model.FindType("DefaultNamespace.SecondSimpleType");
        Assert.NotNull(secondSimpleType, "Invalid complex type.");
        Assert.AreSame(secondSimpleType, model.FindType("DefaultNamespace.SecondSimpleType"), "Invalid complex type.");

        var simpleType = model.FindType("DefaultNamespace.SimpleType");
        Assert.NotNull(simpleType, "Invalid complex type.");
        Assert.AreSame(simpleType, referencedComplexTypeModel.FindType("DefaultNamespace.SimpleType"), "Invalid complex type.");

        var ambiguousComplexType = model.FindType("DefaultNamespace.ReferenceAmbiguousType");
        Assert.NotNull(ambiguousComplexType, "Invalid ambiguous complex type.");
        Assert.Equal(true, ambiguousComplexType.IsBad(), "Invalid IsBad value.");

        var simpleTypeInOtherNamespace = model.FindType("AnnotationNamespace.SimpleType");
        Assert.NotNull(simpleTypeInOtherNamespace, "Invalid complex type.");
        Assert.AreSame(simpleTypeInOtherNamespace, referencedComplexTypeModel.FindType("AnnotationNamespace.SimpleType"), "Invalid complex type.");

        var secondSimpleEntity = model.FindType("DefaultNamespace.SecondSimpleEntity");
        Assert.NotNull(secondSimpleEntity, "Invalid entity type.");
        Assert.AreSame(secondSimpleEntity, model.FindType("DefaultNamespace.SecondSimpleEntity"), "Invalid entity type.");

        var simpleEntity = model.FindType("DefaultNamespace.SimpleEntity");
        Assert.NotNull(simpleEntity, "Invalid entity type.");
        Assert.AreSame(simpleEntity, referencedEntityTypeModel.FindType("DefaultNamespace.SimpleEntity"), "Invalid entity type.");

        var ambiguousEntityType = model.FindType("DefaultNamespace.ReferenceAmbiguousEntity");
        Assert.NotNull(ambiguousEntityType, "Invalid ambiguous entity type.");
        Assert.Equal(true, ambiguousEntityType.IsBad(), "Invalid IsBad value.");

        var simpleEntityInOtherNamespace = model.FindType("AnnotationNamespace.SimpleEntity");
        Assert.NotNull(simpleEntityInOtherNamespace, "Invalid entity type.");
        Assert.AreSame(simpleEntityInOtherNamespace, referencedEntityTypeModel.FindType("AnnotationNamespace.SimpleEntity"), "Invalid entity type.");
    }

    [Fact]
    public void FindMethodTestFindTypeDefinitionModel()
    {
        var model = FindMethodsTestModelBuilder.FindTypeModel();
        var referencedTypeDefinitionModel = this.GetParserResult(FindMethodsTestModelBuilder.FindTypeTypeDefinitionCsdl());
        model.AddReferencedModel(referencedTypeDefinitionModel);

        var lengthInDefaultNamespace = model.FindType("DefaultNamespace.Length");
        Assert.NotNull(lengthInDefaultNamespace, "Invalid type definition.");
        Assert.AreSame(lengthInDefaultNamespace, model.FindType("DefaultNamespace.Length"), "Invalid type definition.");

        var lengthInOtherNamespace = model.FindType("AnnotationNamespace.Length");
        Assert.NotNull(lengthInOtherNamespace, "Invalid type definition.");
        Assert.AreSame(lengthInOtherNamespace, referencedTypeDefinitionModel.FindType("AnnotationNamespace.Length"), "Invalid type definition.");
    }

    [Fact]
    public void FindMethodTestFindFunctionAcrossModelsCsdl()
    {
        var csdls = FindMethodsTestModelBuilder.FindFunctionAcrossModelsCsdl();
        var model = this.GetParserResult(csdls);

        var simpleFunctions = model.FindOperations("DefaultNamespace.SimpleFunction");
        Assert.Equal(1, simpleFunctions.Count(), "Invalid functions count.");
        var simpleFunction = simpleFunctions.SingleOrDefault();
        Assert.NotNull(simpleFunction, "Invalid function.");
        Assert.AreSame(simpleFunction, model.FindOperations("DefaultNamespace.SimpleFunction").SingleOrDefault(), "Invalid function.");

        IEnumerable<EdmError> errors;
        model.Validate(out errors);
        List<EdmError> errorsList = errors.ToList();
        Assert.Equal(2, errorsList.Count, "Invalid errors count.");
        Assert.Equal(EdmErrorCode.DuplicateFunctions, errorsList[0].ErrorCode, "Invalid error code.");
        Assert.Equal(EdmErrorCode.DuplicateFunctions, errorsList[1].ErrorCode, "Invalid error code.");

        var ambiguousFunctions = model.FindOperations("DefaultNamespace.AmbiguousFunction");
        Assert.Equal(2, ambiguousFunctions.Count(), "Invalid count of functions.");
        Assert.AreSame(ambiguousFunctions.ElementAt(0), model.FindOperations("DefaultNamespace.AmbiguousFunction").ElementAt(0), "Invalid function.");
        Assert.AreSame(ambiguousFunctions.ElementAt(1), model.FindOperations("DefaultNamespace.AmbiguousFunction").ElementAt(1), "Invalid function.");

        var simpleFunctionInOtherModels = model.FindOperations("AnnotationNamespace.SimpleFunction");
        Assert.Equal(1, simpleFunctionInOtherModels.Count(), "Invalid functions count.");
        var simpleFunctionInOtherModel = simpleFunctionInOtherModels.SingleOrDefault();
        Assert.NotNull(simpleFunctionInOtherModel, "Invalid function.");
        Assert.AreSame(simpleFunctionInOtherModel, model.FindOperations("AnnotationNamespace.SimpleFunction").SingleOrDefault(), "Invalid function.");

        var functionsDoesNotExist = model.FindOperations("fooNamespace.SimpleFunction");
        Assert.Equal(0, functionsDoesNotExist.Count(), "Invalid function.");
    }

    [Fact]
    public void FindMethodTestFindFunctionAcrossModelsModel()
    {
        var model = FindMethodsTestModelBuilder.FindFunctionAcrossModelsModel();
        var referencedFunctioneModel = this.GetParserResult(FindMethodsTestModelBuilder.FindFunctionAcrossModelsCsdl());
        model.AddReferencedModel(referencedFunctioneModel);

        var simpleFunctions = model.FindOperations("DefaultNamespace.SimpleFunction");
        Assert.Equal(1, simpleFunctions.Count(), "Invalid functions count.");
        var simpleFunction = simpleFunctions.SingleOrDefault();
        Assert.NotNull(simpleFunction, "Invalid function.");
        Assert.AreSame(simpleFunction, referencedFunctioneModel.FindOperations("DefaultNamespace.SimpleFunction").SingleOrDefault(), "Invalid function.");

        var secondSimpleFunctions = model.FindOperations("DefaultNamespace.SecondSimpleFunction");
        Assert.Equal(1, secondSimpleFunctions.Count(), "Invalid functions count.");
        var secondSimpleFunction = secondSimpleFunctions.SingleOrDefault();
        Assert.NotNull(secondSimpleFunction, "Invalid function.");
        Assert.AreSame(secondSimpleFunction, model.FindOperations("DefaultNamespace.SecondSimpleFunction").SingleOrDefault(), "Invalid function.");

        var simpleFunctionInOtherNamespaces = model.FindOperations("AnnotationNamespace.SimpleFunction");
        Assert.Equal(1, simpleFunctionInOtherNamespaces.Count(), "Invalid functions count.");
        var simpleFunctionInOtherNamespace = simpleFunctionInOtherNamespaces.SingleOrDefault();
        Assert.NotNull(simpleFunctionInOtherNamespace, "Invalid function.");
        Assert.AreSame(simpleFunctionInOtherNamespace, referencedFunctioneModel.FindOperations("AnnotationNamespace.SimpleFunction").SingleOrDefault(), "Invalid complex type.");

        IEnumerable<EdmError> errors;
        model.Validate(out errors);
        Assert.Equal(1, errors.Count(), "Invalid errors count.");
        Assert.Equal(EdmErrorCode.AlreadyDefined, errors.First().ErrorCode, "Invalid error code.");

        var referenceAmbiguousFunctions = model.FindOperations("DefaultNamespace.ReferenceAmbiguousFunction");
        Assert.Equal(2, referenceAmbiguousFunctions.Count(), "Invalid count of functions.");

        foreach (var referenceAmbiguousFunction in referenceAmbiguousFunctions)
        {
            Assert.NotNull(referenceAmbiguousFunction, "Invalid ambiguous function.");
            Assert.False(referenceAmbiguousFunction.IsBad(), "Invalid is bad.");
        }
    }

    [Fact]
    public void FindMethodTestFindEntityContainerCsdl()
    {
        var csdls = FindMethodsTestModelBuilder.FindEntityContainerCsdl();
        var model = this.GetParserResult(csdls);

        var simpleContainer = model.FindEntityContainer("SimpleContainer");
        Assert.NotNull(simpleContainer, "Invalid entity container.");
        Assert.AreSame(simpleContainer, model.FindEntityContainer("SimpleContainer"), "Invalid entity container.");
    }

    [Fact]
    public void FindMethodTestFindEntityContainerModel()
    {
        var model = FindMethodsTestModelBuilder.FindEntityContainerModel();
        var referencedEntityContainerModel = this.GetParserResult(FindMethodsTestModelBuilder.FindEntityContainerCsdl());
        model.AddReferencedModel(referencedEntityContainerModel);

        var simpleContainer = model.FindEntityContainer("SimpleContainer");
        Assert.NotNull(simpleContainer, "Invalid entity container.");
        Assert.AreSame(simpleContainer, referencedEntityContainerModel.FindEntityContainer("SimpleContainer"), "Invalid entity container.");
    }

    [Fact]
    public void FindMethodTestWithReferencesInEdmxParser()
    {
        var referencedEntityContainerModel = this.GetParserResult(FindMethodsTestModelBuilder.FindEntityContainerCsdl());

        var edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        IEdmModel model;
        IEnumerable<EdmError> errors;
        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), new IEdmModel[] { referencedEntityContainerModel }, out model, out errors);
        Assert.True(parsed, "parsed");
        Assert.True(errors.Count() == 0, "no errors");

        var simpleContainer = model.FindEntityContainer("SimpleContainer");
        Assert.NotNull(simpleContainer, "Invalid entity container.");
        Assert.AreSame(simpleContainer, referencedEntityContainerModel.FindEntityContainer("SimpleContainer"), "Invalid entity container.");
    }

    [Fact]
    public void FindMethodTestFindVocabularyAnnotationAcrossModelVocabularyAnnotationModel()
    {
        var model = FindMethodsTestModelBuilder.FindVocabularyAnnotationAcrossModelAnnotationModel();
        var referenceModel = this.GetParserResult(FindMethodsTestModelBuilder.FindVocabularyAnnotationAcrossModelAnnotationCsdl());
        model.AddReferencedModel(referenceModel);

        var containerOne = model.FindEntityContainer("ContainerOne");
        var containerThree = referenceModel.FindEntityContainer("ContainerThree");
        this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(containerOne), model.FindVocabularyAnnotations(containerOne));
        this.CompareVacabulraryAnnoationsVariations(model.FindDeclaredVocabularyAnnotations(containerThree).Concat(referenceModel.FindDeclaredVocabularyAnnotations(containerThree)), model.FindVocabularyAnnotations(containerThree));

        var termOneVocabAnnotation = model.FindVocabularyAnnotations(containerOne);
        Assert.Equal(1, termOneVocabAnnotation.Count(), "Invalid count of vocabulary annotation.");

        var containerOneVocabAnnotation = containerOne.VocabularyAnnotations(model);
        Assert.Equal(1, containerOneVocabAnnotation.Count(), "Invalid count of vocabulary annotation.");
        Assert.AreSame(containerOneVocabAnnotation.SingleOrDefault(), termOneVocabAnnotation.SingleOrDefault(), "Invalid vocabulary annotation.");

        var termThreeVocabAnnotation = model.FindVocabularyAnnotations(containerThree);
        Assert.Equal(1, termThreeVocabAnnotation.Count(), "Invalid count of vocabulary annotation.");

        var containerThreeVocabAnnotation = containerThree.VocabularyAnnotations(model);
        Assert.Equal(1, containerThreeVocabAnnotation.Count(), "Invalid count of vocabulary annotation.");
        Assert.AreSame(containerThreeVocabAnnotation.SingleOrDefault(), termThreeVocabAnnotation.SingleOrDefault(), "Invalid vocabulary annotation.");
    }

    [Fact]
    public void FindMethodTestPrimitiveTypes()
    {
        var parsedModel = this.GetParserResult(ModelBuilder.StringWithFacets());
        var constructibleModel = new EdmModel();

        Assert.False(parsedModel.SchemaElements.Any(n => n.Namespace == EdmCoreModel.Namespace), "The SchemaElement property should not contain primitive types.");
        Assert.False(constructibleModel.SchemaElements.Any(n => n.Namespace == EdmCoreModel.Namespace), "The SchemaElement property should not contain primitive types.");

        foreach (var primitiveType in EdmCoreModel.Instance.SchemaElements)
        {
            Assert.Null(parsedModel.FindDeclaredType(primitiveType.FullName()), "The FindType method should return null for primitive types.");
            Assert.Null(constructibleModel.FindDeclaredType(primitiveType.FullName()), "The FindType method should return null for primitive types.");
            Assert.NotNull(parsedModel.FindType(primitiveType.FullName()), "The FindType method should not return null for primitive types across models.");
            Assert.NotNull(constructibleModel.FindType(primitiveType.FullName()), "The FindType method should not return null for primitive types across models.");
            Assert.Equal(EdmCoreModel.Instance.FindDeclaredType(primitiveType.FullName()), primitiveType, "The EdmCore should contain the primitive type definitions");
        }
    }

    [Fact]
    public void FindPropertyOnOverriddenDeclaredPropertiesTest()
    {
        var customEdmComplex = new CustomEdmComplexType("", "");
        customEdmComplex.AddStructuralProperty("Property3", EdmCoreModel.Instance.GetInt16(true));

        Assert.NotNull(customEdmComplex.FindProperty("Property3"), "The custom edm complex type should have the property, Property3.");
        Assert.Equal(
                            customEdmComplex.FindProperty("Property1").Type.Definition,
                            EdmCoreModel.Instance.SchemaElements.Single(n => n.FullName() == "Edm.Int32"),
                            "The custom edm complex type should have the property, Property1."
                         );

        Assert.Equal(
                            customEdmComplex.FindProperty("Property2").Type.Definition,
                            EdmCoreModel.Instance.SchemaElements.Single(n => n.FullName() == "Edm.Int32"),
                            "The custom edm complex type should have the property, Property1."
                         );
        Assert.Equal(customEdmComplex.DeclaredProperties.Count(), 3, "The custom edm complex type should have the 3 propertes.");
    }

    [Fact]
    public void FindVocabularyAnnotationsIncludingInheritedAnnotationsTest()
    {
        Action<IEnumerable<IEdmVocabularyAnnotation>, IEnumerable<string>> checkAnnotationTerms = (annotations, termNames) =>
        {
            Assert.True(!annotations.Select(n => n.Term.FullName()).Except(termNames).Any() && annotations.Count() == termNames.Count(), "The actual annotation terms are not correct.");
        };

        var model = this.GetParserResult(FindMethodsTestModelBuilder.FindVocabularyAnnotationsIncludingInheritedAnnotationsCsdl());
        checkAnnotationTerms(
                                model.FindVocabularyAnnotationsIncludingInheritedAnnotations(model.FindType("DefaultNamespace.Pet")),
                                new string[] { "AnnotationNamespace.AddressObject", "AnnotationNamespace.Habitation", "AnnotationNamespace.Unknown" }
                             );

        checkAnnotationTerms(
                                model.FindVocabularyAnnotationsIncludingInheritedAnnotations(model.FindType("AnnotationNamespace.Animal")),
                                new string[] { "AnnotationNamespace.Habitation", "AnnotationNamespace.Unknown" }
                             );

        checkAnnotationTerms(
                                model.FindVocabularyAnnotationsIncludingInheritedAnnotations(model.FindType("DefaultNamespace.Child")),
                                new string[] { }
                             );

        checkAnnotationTerms(
                                model.FindVocabularyAnnotationsIncludingInheritedAnnotations((IEdmEntityType)((IEdmEntityType)model.FindType("DefaultNamespace.UnresolvedType")).BaseType),
                                new string[] { "AnnotationNamespace.AddressObject" }
                             );

        checkAnnotationTerms(
                                model.FindVocabularyAnnotationsIncludingInheritedAnnotations(model.FindType("AnnotationNamespace.ComplexTypeWithEntityTypeBase")),
                                new string[] { "AnnotationNamespace.Habitation" }
                             );
        Assert.Throws<ArgumentNullException>(() => model.FindVocabularyAnnotationsIncludingInheritedAnnotations(model.FindType("Unknown.Unknown")));
    }


    //[TestMethod, Variation(Id = 201, SkipReason = @"[EdmLib]  FindVocabularyAnnotationsIncludingInheritedAnnotations does not return all the annotations for the cyclic base types -- postponed")]
    public void FindVocabularyAnnotationsIncludingInheritedAnnotationsRecursiveTest()
    {
        Action<IEnumerable<IEdmVocabularyAnnotation>, IEnumerable<string>> checkAnnotationTerms = (annotations, termNames) =>
        {
            Assert.True(!annotations.Select(n => n.Term.FullName()).Except(termNames).Any() && annotations.Count() == termNames.Count(), "The actual annotation terms are not correct.");
        };

        var model = this.GetParserResult(FindMethodsTestModelBuilder.FindVocabularyAnnotationsIncludingInheritedAnnotationsCsdl());

        checkAnnotationTerms(
                                model.FindVocabularyAnnotationsIncludingInheritedAnnotations(model.FindType("AnnotationNamespace.Base")),
                                new string[] { "AnnotationNamespace.Unknown", "AnnotationNamespace.Habitation", }
                             );

        checkAnnotationTerms(
                                model.FindVocabularyAnnotationsIncludingInheritedAnnotations(model.FindType("AnnotationNamespace.Derived")),
                                new string[] { "AnnotationNamespace.Unknown", "AnnotationNamespace.Habitation", }
                             );
    }

    //[TestMethod, Variation(Id = 202, SkipReason = @"[EdmLib] FindDirectlyDerivedTypes and FindAllDerivedTypes does not always return the types in referenced models - postponed ")]
    public void FindDerivedTypeReferencedModelTests()
    {
        IEdmModel masterModel = null;
        IEdmModel testModel = null;

        Action<string, Func<IEdmModel, IEdmStructuredType, IEnumerable<IEdmStructuredType>>> verifyFindDerivedTypes = (typeName, findDerivedType) =>
        {
            this.VerifyFindDerivedTypes(masterModel, testModel, typeName, findDerivedType);
        };

        Func<IEdmModel, IEdmStructuredType, IEnumerable<IEdmStructuredType>> runFindDirectlyDerivedTypes = (edmModel, type) => edmModel.FindDirectlyDerivedTypes(type);
        Func<IEdmModel, IEdmStructuredType, IEnumerable<IEdmStructuredType>> runFindAllDerivedTypes = (edmModel, type) => edmModel.FindDirectlyDerivedTypes(type);

        var csdls = FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes(this.EdmVersion);
        masterModel = this.GetParserResult(csdls);
        var models = new[] {
                                  this.GetParserResult(new XElement[] { csdls.ElementAt(0) }),
                                  this.GetParserResult(new XElement[] { csdls.ElementAt(1) }),
                                  this.GetParserResult(new XElement[] { csdls.ElementAt(2) })
                                };

        testModel = this.GetParserResult(new[] { csdls.ElementAt(0) }, models.ElementAt(1), models.ElementAt(2));
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindAllDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindDirectlyDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindAllDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindDirectlyDerivedTypes);

        testModel = this.GetParserResult(new[] { csdls.ElementAt(1) }, models.ElementAt(0), models.ElementAt(2));
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindAllDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindDirectlyDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindAllDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindDirectlyDerivedTypes);

        testModel = this.GetParserResult(new[] { csdls.ElementAt(2) }, models.ElementAt(0), models.ElementAt(1));
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindAllDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindDirectlyDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindAllDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindDirectlyDerivedTypes);

        testModel = this.GetParserResult(new[] { csdls.ElementAt(2) }, this.GetParserResult(new[] { csdls.ElementAt(1) }, models.ElementAt(0)));
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindAllDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindDirectlyDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindAllDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindDirectlyDerivedTypes);
    }

    //[TestMethod, Variation(Id = 203, SkipReason = @"[EdmLib] FindDirectlyDerivedTypes and FindAllDerivedTypes should return an ambiguous type instance when derived types are ambiguous -- postponed")]
    public void FindDerivedTypeWithAmbigousTypesTests()
    {
        var csdls = FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes(this.EdmVersion);
        IEdmModel masterModel = null;
        IEdmModel testModel = null;

        Action<string, Func<IEdmModel, IEdmStructuredType, IEnumerable<IEdmStructuredType>>> verifyFindDerivedTypes = (typeName, findDerivedType) =>
        {
            this.VerifyFindDerivedTypes(masterModel, testModel, typeName, findDerivedType);
        };

        Func<IEdmModel, IEdmStructuredType, IEnumerable<IEdmStructuredType>> runFindDirectlyDerivedTypes = (edmModel, type) => edmModel.FindDirectlyDerivedTypes(type);
        Func<IEdmModel, IEdmStructuredType, IEnumerable<IEdmStructuredType>> runFindAllDerivedTypes = (edmModel, type) => edmModel.FindDirectlyDerivedTypes(type);

        var models = new[] {
                                  this.GetParserResult(new [] { csdls.ElementAt(0) }),
                                  this.GetParserResult(new [] { csdls.ElementAt(1) }),
                                  this.GetParserResult(new [] { csdls.ElementAt(2) })
                                };

        testModel = this.GetParserResult(csdls.Concat(new[] { csdls.ElementAt(1) }));
        Assert.Throws<ArgumentNullException>(() => verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindAllDerivedTypes));
        Assert.Throws<ArgumentNullException>(() => verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindDirectlyDerivedTypes));
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindAllDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindDirectlyDerivedTypes);

        testModel = this.GetParserResult(new[] { csdls.ElementAt(0) }, models.ElementAt(2), this.GetParserResult(new[] { csdls.ElementAt(1) }, models.ElementAt(2)));
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindAllDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindDirectlyDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindAllDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindDirectlyDerivedTypes);
    }

    //[TestMethod, Variation(Id = 204, SkipReason = @"[EdmLib] FindDirectlyDerivedTypes and FindAllDerivedTypes should throw ArgumentNullException instead of NullReferenceException when their parameters are null -- postponed, [EdmLib] FindDirectlyDerivedTypes and FindAllDerivedTypes should return UnresolvedEntityType or UnresolvedComplexType when it could not resolve the derived types. -- postponed")]
    public void FindDerivedTypeReferencedModelWithUnresolvedTypesTests()
    {
        var csdls = FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes(this.EdmVersion);
        var testModel = this.GetParserResult(new[] { csdls.ElementAt(0), csdls.ElementAt(2) });
        Assert.Throws<ArgumentNullException>(() => testModel.FindAllDerivedTypes(testModel.FindType("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2") as IEdmStructuredType));
        Assert.Throws<ArgumentNullException>(() => testModel.FindDirectlyDerivedTypes(testModel.FindType("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2") as IEdmStructuredType));

        Assert.Equal(EdmErrorCode.BadUnresolvedEntityType, testModel.FindDirectlyDerivedTypes(testModel.FindType("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2") as IEdmStructuredType).Single().Errors().Single(), "Unresolved type should be returned.");
        Assert.Equal(EdmErrorCode.BadUnresolvedEntityType, testModel.FindAllDerivedTypes(testModel.FindType("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2") as IEdmStructuredType).Single().Errors().Single(), "Unresolved type should be returned.");
    }

    private void VerifyFindDerivedTypes(IEdmModel masterModel, IEdmModel testModel, string typeName, Func<IEdmModel, IEdmStructuredType, IEnumerable<IEdmStructuredType>> findDerivedType)
    {
        var expectedResults = findDerivedType(masterModel, masterModel.FindType(typeName) as IEdmStructuredType).Select(n => GetFullName(n));
        var actualResults = findDerivedType(testModel, testModel.FindType(typeName) as IEdmStructuredType).Select(n => GetFullName(n));
        Assert.True(
                            expectedResults.Count() == actualResults.Count()
                            && !expectedResults.Except(actualResults).Any(), "FindDerivedTypes returns unexpected results."
                        );
    }

    private string GetFullName(IEdmStructuredType type)
    {
        return type as IEdmComplexType != null ? ((IEdmComplexType)type).FullName() : type as IEdmEntityType != null ? ((IEdmEntityType)type).FullName() : null;
    }

    private void FindRecursiveNavigationTargetCheck(IEdmEntitySet entity, IEdmNavigationProperty entityToSelf, IEdmNavigationProperty selfToEntity)
    {
        var entityTarget = entity.FindNavigationTarget(entityToSelf);
        Assert.NotNull(entityTarget, "Invalid navigation target.");
        Assert.Equal(entity.Name, entityTarget.Name, "Invalid targeted set name.");
        Assert.AreSame(entity, entityTarget, "Invalid targeted set.");

        entityTarget = entity.FindNavigationTarget(selfToEntity);
        Assert.NotNull(entityTarget, "Invalid navigation target.");
        Assert.Equal(entity.Name, entityTarget.Name, "Invalid targeted set name.");
        Assert.AreSame(entity, entityTarget, "Invalid targeted set.");
    }

    private void FindNavigationTargetCheck(IEdmEntitySet entityOne, IEdmNavigationProperty entityOneToEntityTwo, IEdmEntitySet entityTwo, IEdmNavigationProperty entityTwoToEntityOne)
    {
        var entityOneTarget = entityOne.FindNavigationTarget(entityOneToEntityTwo);
        Assert.NotNull(entityOneTarget, "Invalid navigation target.");
        Assert.Equal(entityTwo.Name, entityOneTarget.Name, "Invalid targeted set name.");
        Assert.AreSame(entityTwo, entityOneTarget, "Invalid targeted set.");

        entityOneTarget = entityOne.FindNavigationTarget(entityTwoToEntityOne);
        Assert.True(entityOneTarget is IEdmUnknownEntitySet, "Invalid result.");

        var entityTwoTarget = entityTwo.FindNavigationTarget(entityTwoToEntityOne);
        Assert.NotNull(entityTwoTarget, "Invalid navigation target.");
        Assert.Equal(entityOne.Name, entityTwoTarget.Name, "Invalid targeted set name.");
        Assert.AreSame(entityOne, entityTwoTarget, "Invalid targeted set.");

        entityTwoTarget = entityTwo.FindNavigationTarget(entityOneToEntityTwo);
        Assert.True(entityTwoTarget is IEdmUnknownEntitySet, "Invalid result.");
    }

    private void FunctionOverloadingCheck(IEnumerable<XElement> csdls, List<FunctionInfo> functionInfos, string functionNamespace)
    {
        var model = this.GetParserResult(csdls);
        this.VerifySemanticValidation(model, new EdmLibTestErrors());

        foreach (FunctionInfo functionInfo in functionInfos)
        {
            var functionsFound = model.FindOperations(functionNamespace + "." + functionInfo.Name);
            var functionFoundCount = GetOperationMatchedCount(functionInfo, functionsFound);
            Assert.Equal(1, functionFoundCount, "Invalid count of function found");
        }
    }

    private void FunctionImportOverloadingCheck(IEnumerable<XElement> csdls, List<FunctionInfo> functionInfos, string entityContainerName)
    {
        var model = this.GetParserResult(csdls);
        this.VerifySemanticValidation(model, new EdmLibTestErrors());
        var entityContainer = model.FindEntityContainer(entityContainerName);

        foreach (FunctionInfo functionInfo in functionInfos)
        {
            var operationsFound = entityContainer.FindOperationImports(functionInfo.Name);
            var functionFoundCount = GetOperationMatchedCount(functionInfo, operationsFound);
            Assert.Equal(1, functionFoundCount, "Invalid count of function found");
        }
    }

    private static int GetOperationMatchedCount(FunctionInfo operationInfo, IEnumerable<IEdmOperationImport> operationImportsFound)
    {
        var functionFoundCount = 0;

        foreach (var function in operationImportsFound)
        {
            var parameters = function.Operation.Parameters.ToList();

            if (IsOperationParameterMatched(operationInfo, parameters))
            {
                functionFoundCount++;
            }
        }
        return functionFoundCount;
    }

    private static int GetOperationMatchedCount(FunctionInfo operationInfo, IEnumerable<IEdmOperation> operationsFound)
    {
        var functionFoundCount = 0;

        foreach (var function in operationsFound)
        {
            var parameters = function.Parameters.ToList();

            if (IsOperationParameterMatched(operationInfo, parameters))
            {
                functionFoundCount++;
            }
        }
        return functionFoundCount;
    }

    private static bool IsOperationParameterMatched(FunctionInfo functionInfo, List<IEdmOperationParameter> parameters)
    {
        var parametersMatched = 0;

        if (functionInfo.Parameters.Count == parameters.Count)
        {
            for (int i = 0; i < functionInfo.Parameters.Count; i++)
            {
                bool sameParameter = RemoveParameterFacets(functionInfo.Parameters[i].Type.ToString()).Equals(RemoveParameterFacets(parameters[i].Type.ToString()));
                if (functionInfo.Parameters[i].Name.Equals(parameters[i].Name) && sameParameter)
                {
                    parametersMatched++;
                }
                else
                {
                    break;
                }
            }
        }

        if (parametersMatched == functionInfo.Parameters.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CompareVacabulraryAnnoationsVariations(params IEnumerable<IEdmVocabularyAnnotation>[] addressTerm)
    {
        var baseValue = addressTerm.ElementAt(0);
        Assert.False(addressTerm.Any(n => n.Count() != baseValue.Count() || baseValue.Except(n).Any()), "The results between FindMethod and its declared version should be same.");
    }

    private static string RemoveParameterFacets(string parameter)
    {
        StringBuilder parameterWithoutFacets = new StringBuilder(string.Empty);
        char[] charDelimiter = new char[] { ' ', '(', ')', '[', ']' };
        int delimiterLocation = 0;

        var splitResults = parameter.Split(charDelimiter, StringSplitOptions.None);

        foreach (string split in splitResults)
        {
            delimiterLocation += split.Length + 1;

            if (split.StartsWith("Nullable=")) // Assuming nullable is part of overloading
            {
                parameterWithoutFacets.Append(" ");
                parameterWithoutFacets.Append(split.Trim());
            }
            else if (!split.Contains("="))
            {
                parameterWithoutFacets.Append(split.Trim());
            }

            if (delimiterLocation <= parameter.Length)
            {
                if (parameter[delimiterLocation - 1].Equals('('))
                {
                    parameterWithoutFacets.Append('(');
                }
                else if (parameter[delimiterLocation - 1].Equals(')'))
                {
                    parameterWithoutFacets.Append(')');
                }
            }
        }

        return parameterWithoutFacets.ToString();
    }

    private class FunctionInfo
    {
        public string Name { get; set; }
        public List<ParameterInfo> Parameters { get; set; }
    }

    private class ParameterInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public ParameterInfo(string name, string type)
        {
            this.Name = name;
            this.Type = type;
        }
    }

    private class ParameterInfoList : List<ParameterInfo>
    {
        public void Add(string name, string type)
        {
            if (!(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type)))
            {
                base.Add(new ParameterInfo(name, type));
            }
        }
    }

    private class CustomEdmComplexType : EdmEntityType
    {
        public CustomEdmComplexType(string ns, string n)
            : base(ns, n)
        {
        }

        /// <summary>
        /// Gets the properties declared immediately within this type.
        /// </summary>
        public override IEnumerable<IEdmProperty> DeclaredProperties
        {
            get
            {
                if (!base.DeclaredProperties.Any(n => n.Name == "Property1"))
                {
                    this.AddStructuralProperty("Property1", EdmCoreModel.Instance.GetInt32(false));
                }
                if (!base.DeclaredProperties.Any(n => n.Name == "Property2"))
                {
                    this.AddStructuralProperty("Property2", EdmCoreModel.Instance.GetInt32(false));
                }
                return base.DeclaredProperties;
            }
        }
    }

    private void VerifyFindMethods(IEnumerable<XElement> csdls, IEdmModel edmModel)
    {
        foreach (var csdl in csdls)
        {
            this.VerifyFindSchemaElementMethod(csdl, edmModel);
            this.VerifyFindEntityContainer(csdl, edmModel);
            this.VerifyFindEntityContainerElement(csdl, edmModel);
            this.VerifyFindPropertyMethod(csdl, edmModel);
            this.VerifyFindParameterMethod(csdl, edmModel);
            this.VerifyFindNavigationPropertyMethod(csdl, edmModel);
            this.VerifyFindTypeReferencePropertyMethod(csdl, edmModel);
        }

        this.VerifyFindDerivedType(csdls, edmModel);
    }

    private void VerifyFindDerivedType(IEnumerable<XElement> sourceCsdls, IEdmModel testModel)
    {
        Func<IEdmStructuredType, string> getFullName = (type) =>
        {
            return type as IEdmComplexType != null ? ((IEdmComplexType)type).FullName() : type as IEdmEntityType != null ? ((IEdmEntityType)type).FullName() : null;
        };

        Func<IEdmStructuredType, string> getNamespace = (type) =>
        {
            return type as IEdmComplexType != null ? ((IEdmComplexType)type).Namespace : type as IEdmEntityType != null ? ((IEdmEntityType)type).Namespace : null;
        };

        Action<IEdmStructuredType, string> checkFindAllDerivedFunction = (structuredType, structuralTypeElementName) =>
        {
            var expectedResults = EdmLibCsdlContentGenerator.GetDerivedTypes(sourceCsdls, structuralTypeElementName, getFullName(structuredType));
            var actualResults = testModel.FindAllDerivedTypes(structuredType).Select(n => getFullName(n));
            Assert.True(expectedResults.Count() == actualResults.Count() && !expectedResults.Except(actualResults).Any());
        };

        Action<IEdmStructuredType, string> checkFindDirectlyDerivedFunction = (structuredType, structuralTypeElementName) =>
        {
            var expectedResults = EdmLibCsdlContentGenerator.GetDirectlyDerivedTypes(sourceCsdls, structuralTypeElementName, getFullName(structuredType));
            var actualResults = testModel.FindDirectlyDerivedTypes(structuredType).Select(n => getFullName(n));

            Assert.True(expectedResults.Count() == actualResults.Count() && !expectedResults.Except(actualResults).Any());
        };

        IEnumerable<EdmError> edmErrors;
        testModel.Validate(out edmErrors);

        var structuredTypes = testModel.SchemaElements.OfType<IEdmStructuredType>();
        foreach (var structuredType in structuredTypes)
        {
            if (edmErrors.Any(n => n.ErrorCode == EdmErrorCode.BadCyclicEntity && n.ErrorMessage.Contains(getFullName(structuredType))))
            {
                Assert.Empty(testModel.FindDirectlyDerivedTypes(structuredType));
                Assert.Empty(testModel.FindAllDerivedTypes(structuredType));
            }
            else
            {
                if (structuredType is IEdmEntityType)
                {
                    checkFindDirectlyDerivedFunction(structuredType, "EntityType");
                    checkFindAllDerivedFunction(structuredType, "EntityType");
                }
                else if (structuredType is IEdmComplexType)
                {
                    checkFindDirectlyDerivedFunction(structuredType, "ComplexType");
                    checkFindAllDerivedFunction(structuredType, "ComplexType");
                }
            }
        }

    }

    private void VerifyFindNavigationPropertyMethod(XElement sourceCsdl, IEdmModel testModel)
    {
        var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(this.EdmVersion);
        Assert.Equal(csdlNamespace, sourceCsdl.Name.Namespace);

        var namespaceValue = sourceCsdl.Attribute("Namespace")?.Value;
        var elementTypes = new string[] { "NavigationProperty" };

        foreach (var elementType in elementTypes)
        {
            var elementTypeFoundList = sourceCsdl.Descendants().Elements(XName.Get(elementType, csdlNamespace.NamespaceName));

            foreach (var elementTypeFound in elementTypeFoundList)
            {
                Assert.Contains(new string[] { "EntityType" }, n => n == elementTypeFound.Parent?.Name.LocalName);

                var entityTypeName = elementTypeFound.Parent.Attribute("Name")?.Value;
                var entityType = testModel.FindType(namespaceValue + "." + entityTypeName) as IEdmEntityType;
                var entityTypeReference = new EdmEntityTypeReference(entityType, true);

                var navigationFound = entityTypeReference.FindNavigationProperty(elementTypeFound.Attribute("Name")?.Value);

                Assert.NotNull(navigationFound);
            }
        }
    }

    private void VerifyFindPropertyMethod(XElement sourceCsdl, IEdmModel testModel)
    {
        var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(this.EdmVersion);
        Assert.Equal(csdlNamespace, sourceCsdl.Name.Namespace);

        var namespaceValue = sourceCsdl.Attribute("Namespace")?.Value;
        var schemaElementTypes = new string[] { "Property", "NavigationProperty" };

        foreach (var schemaElementType in schemaElementTypes)
        {
            Console.WriteLine("Test CSDL:\n\r{0}", sourceCsdl.ToString());
            foreach (var propertyElement in sourceCsdl.Descendants().Elements(XName.Get(schemaElementType, csdlNamespace.NamespaceName)))
            {
                IEdmProperty foundProperty = null;
                Assert.Contains(new string[] { "EntityType", "ComplexType", "RowType" }, n => n == propertyElement.Parent?.Name.LocalName);

                if (propertyElement.Parent.Name.LocalName != "RowType")
                {
                    var schemaElementName = string.Format("{0}.{1}", namespaceValue, propertyElement.Parent.Attribute("Name").Value);
                    var elementFound = testModel.FindType(schemaElementName) as IEdmStructuredType;
                    foundProperty = elementFound.FindProperty(propertyElement.Attribute("Name")?.Value);
                }

                Assert.NotNull(foundProperty);
                Assert.Equal(foundProperty.Name, propertyElement.Attribute("Name")?.Value);
            }
        }
    }

    private void VerifyFindTypeReferencePropertyMethod(XElement sourceCsdl, IEdmModel testModel)
    {
        var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(this.EdmVersion);
        Assert.Equal(csdlNamespace, sourceCsdl.Name.Namespace);

        var namespaceValue = sourceCsdl.Attribute("Namespace")?.Value;
        var schemaElementTypes = new string[] { "Property", "NavigationProperty" };

        foreach (var schemaElementType in schemaElementTypes)
        {
            Console.WriteLine("Test CSDL:\n\r{0}", sourceCsdl.ToString());
            foreach (var propertyElement in sourceCsdl.Descendants().Elements(XName.Get(schemaElementType, csdlNamespace.NamespaceName)))
            {
                IEdmProperty foundProperty = null;
                Assert.Contains(new string[] { "EntityType", "ComplexType", "RowType" }, n => n == propertyElement.Parent.Name.LocalName);
                if (propertyElement.Parent.Name.LocalName != "RowType")
                {
                    var elementName = string.Format("{0}.{1}", namespaceValue, propertyElement.Parent.Attribute("Name").Value);
                    var elementTypeFound = testModel.FindType(elementName) as IEdmStructuredType;

                    IEdmStructuredTypeReference elementTypeReferenceFound = null;
                    if (propertyElement.Parent.Name.LocalName.Equals("ComplexType"))
                    {
                        elementTypeReferenceFound = new EdmComplexTypeReference((IEdmComplexType)elementTypeFound, true);
                    }
                    else if (propertyElement.Parent.Name.LocalName.Equals("EntityType"))
                    {
                        elementTypeReferenceFound = new EdmEntityTypeReference((IEdmEntityType)elementTypeFound, true);
                    }

                    foundProperty = elementTypeReferenceFound.FindProperty(propertyElement.Attribute("Name")?.Value);
                }
                else if (propertyElement.Parent.Parent?.Name.LocalName == "CollectionType")
                {
                    // TODO: Make VerifyFindPropertyMethod support properties defined in RowType of CollectionType.
                    throw new NotImplementedException("VerifyFindPropertyMethod does not support properties defined in RowType of CollectionType.");
                }

                Assert.NotNull(foundProperty);
                Assert.Equal(foundProperty.Name, propertyElement.Attribute("Name")?.Value);
            }
        }
    }

    private void VerifyFindParameterMethod(XElement sourceCsdl, IEdmModel testModel)
    {
        var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(this.EdmVersion);
        Assert.Equal(csdlNamespace, sourceCsdl.Name.Namespace);

        var namespaceValue = sourceCsdl.Attribute("Namespace")?.Value;
        var schemaElementTypes = new string[] { "Parameter" };

        foreach (var schemaElementType in schemaElementTypes)
        {
            Console.WriteLine("Test CSDL:\n\r{0}", sourceCsdl.ToString());

            foreach (var parameterElement in sourceCsdl.Descendants().Elements(XName.Get(schemaElementType, csdlNamespace.NamespaceName)))
            {
                IEdmOperationParameter parameterFound = null;
                var parameterName = parameterElement.Attribute("Name")?.Value;
                Assert.Contains(new string[] { "Function", "Action" }, n => n == parameterElement.Parent?.Name.LocalName);

                var schemaElementName = string.Format("{0}.{1}", namespaceValue, parameterElement.Parent?.Attribute("Name")?.Value);
                var elementFound = testModel.FindOperations(schemaElementName).Where(n => n.FindParameter(parameterName) != null).FirstOrDefault();

                parameterFound = elementFound?.FindParameter(parameterName);

                Assert.NotNull(parameterFound);
                Assert.Equal(parameterFound?.Name, parameterName);
            }
        }
    }

    protected void VerifyFindSchemaElementMethod(XElement sourceCsdl, IEdmModel testModel)
    {
        var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(this.EdmVersion);
        Assert.Equal(csdlNamespace, sourceCsdl.Name.Namespace);

        IEnumerable<string> schemaElementTypes = new string[] { "EntityType", "ComplexType", "Function", "Term" };

        var namespaceValue = sourceCsdl.Attribute("Namespace")?.Value;
        foreach (var schemaElementType in schemaElementTypes)
        {
            var schemaElements = from element in sourceCsdl.Elements(XName.Get(schemaElementType, csdlNamespace.NamespaceName))
                                 select element;
            Console.WriteLine("Test CSDL:\n\r{0}", sourceCsdl.ToString());

            foreach (var schemaElement in schemaElements)
            {
                var elementNameExpected = string.Format("{0}.{1}", namespaceValue, schemaElement.Attribute("Name")?.Value);
                Console.WriteLine("FindSchemaType for {0}", elementNameExpected);

                var typeFound = testModel.FindType(elementNameExpected);
                Assert.Equal(typeFound, testModel.FindDeclaredType(elementNameExpected));

                var operationGroup = testModel.FindOperations(elementNameExpected);
                Assert.True(operationGroup.Count() == testModel.FindDeclaredOperations(elementNameExpected).Count() && !operationGroup.Except(testModel.FindDeclaredOperations(elementNameExpected)).Any());

                var valueTermFound = testModel.FindTerm(elementNameExpected);
                Assert.Equal(valueTermFound, testModel.FindDeclaredTerm(elementNameExpected));

                Assert.False((typeFound == null) && (operationGroup == null) && (valueTermFound == null));

                IEdmSchemaElement schemaElementFound = null;
                if (operationGroup.Count() > 0)
                {
                    schemaElementFound = (IEdmSchemaElement)operationGroup.First();
                }
                else if (typeFound != null)
                {
                    schemaElementFound = typeFound;
                }
                else if (valueTermFound != null)
                {
                    schemaElementFound = valueTermFound;
                }

                Assert.Equal(elementNameExpected, schemaElementFound.FullName());
            }
        }
    }

    protected void VerifyFindEntityContainer(XElement sourceCsdl, IEdmModel testModel)
    {
        var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(this.EdmVersion);
        Assert.Equal(csdlNamespace, sourceCsdl.Name.Namespace);

        var entityContainerNames = from element in sourceCsdl.Elements(XName.Get("EntityContainer", csdlNamespace.NamespaceName))
                                   select element.Attribute("Name")?.Value;
        Console.WriteLine("Test CSDL:\n\r{0}", sourceCsdl.ToString());

        foreach (var entityContainerName in entityContainerNames)
        {
            Console.WriteLine("FindEntityContainer for {0}", entityContainerName);

            var elementFound = testModel.FindEntityContainer(entityContainerName);

            Assert.Equal(elementFound, testModel.EntityContainer);
            Assert.NotNull(elementFound);
            Assert.Equal(entityContainerName, elementFound.Name);
        }
    }

    protected void VerifyFindEntityContainerElement(XElement sourceCsdl, IEdmModel testModel)
    {
        var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(this.EdmVersion);
        Assert.Equal(csdlNamespace, sourceCsdl.Name.Namespace);

        Console.WriteLine("Test CSDL:\n\r{0}", sourceCsdl.ToString());

        var entityContainers = from element in sourceCsdl.DescendantsAndSelf(XName.Get("EntityContainer", csdlNamespace.NamespaceName))
                               select element;

        IEnumerable<string> entityContainerElementTypes = new string[] { "EntitySet", "FunctionImport" };

        foreach (var entityContainer in entityContainers)
        {
            var entityContainerElements = from element in entityContainer.Descendants()
                                          where entityContainerElementTypes.Select(n => XName.Get(n, csdlNamespace.NamespaceName)).Any(m => m == element.Name)
                                          select element;
            var entityContainerName = entityContainer.Attribute("Name")?.Value;
            var entityContainerObj = testModel.FindEntityContainer(entityContainerName) as IEdmEntityContainer;

            foreach (var entityContainerElement in entityContainerElements)
            {
                var entityContainerElementName = entityContainerElement.Attribute("Name")?.Value;
                var entitySetFound = entityContainerObj.FindEntitySet(entityContainerElementName);
                var functionImportsFound = entityContainerObj.FindOperationImports(entityContainerElementName);

                IEdmEntityContainerElement entityContainerElementFound = null;
                var elementsWithSameName = entityContainerElements.Where(n => n.Attribute("Name").Value.Equals(entityContainerElementName)).Count();

                if (functionImportsFound != null && functionImportsFound.Count() == elementsWithSameName)
                {
                    entityContainerElementFound = functionImportsFound.First();
                }
                else if (entitySetFound != null)
                {
                    entityContainerElementFound = entitySetFound;
                }

                Assert.NotNull(entityContainerElementFound);
                Assert.Equal(entityContainerElementName, entityContainerElementFound.Name);
                Assert.True(entityContainerElement.Name.LocalName == "EntitySet" || entityContainerElement.Name.LocalName == "FunctionImport");
            }
        }
    }
}
