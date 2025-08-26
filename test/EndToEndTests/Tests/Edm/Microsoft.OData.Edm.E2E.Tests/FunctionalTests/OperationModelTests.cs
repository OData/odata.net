//---------------------------------------------------------------------
// <copyright file="OperationModelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class OperationModelTests : EdmLibTestCaseBase
{
    private const string DefaultNamespaceName = "DefaultNamespace";

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void OperationStandalone_Roundtrip_Verify(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var function = new EdmFunction(DefaultNamespaceName, new string('a', 10), EdmCoreModel.Instance.GetInt16(false));
        model.AddElement(function);

        var container = new EdmEntityContainer(DefaultNamespaceName, "DefaultContainer");
        model.AddElement(container);
        container.AddFunctionImport(function);

        var csdl1 = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).First();
        Assert.Empty(errors);

        Assert.Equal(
            """
            <Schema Namespace="DefaultNamespace" xmlns="http://docs.oasis-open.org/odata/ns/edm">
              <Function Name="aaaaaaaaaa">
                <ReturnType Type="Edm.Int16" Nullable="false" />
              </Function>
              <EntityContainer Name="DefaultContainer">
                <FunctionImport Name="aaaaaaaaaa" Function="DefaultNamespace.aaaaaaaaaa" />
              </EntityContainer>
            </Schema>
            """, PrettifyXml(csdl1));

        var isParsed = SchemaReader.TryParse(new[] { csdl1 }.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel edmModel, out errors);
        Assert.True(isParsed && !errors.Any());

        var csdl2 = this.GetSerializerResult(edmModel, edmVersion, out errors).First();
        Assert.Empty(errors);

        Assert.Equal(
            """
            <Schema Namespace="DefaultNamespace" xmlns="http://docs.oasis-open.org/odata/ns/edm">
              <Function Name="aaaaaaaaaa">
                <ReturnType Type="Edm.Int16" Nullable="false" />
              </Function>
              <EntityContainer Name="DefaultContainer">
                <FunctionImport Name="aaaaaaaaaa" Function="DefaultNamespace.aaaaaaaaaa" />
              </EntityContainer>
            </Schema>
            """, PrettifyXml(csdl2));
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void OperationStandalone_FindMethods_Verify(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var function = new EdmFunction(DefaultNamespaceName, new string('a', 10), EdmCoreModel.Instance.GetInt16(false));
        model.AddElement(function);

        var container = new EdmEntityContainer(DefaultNamespaceName, "DefaultContainer");
        model.AddElement(container);
        container.AddFunctionImport(function);

        var csdl = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).First();
        Assert.Empty(errors);

        Assert.Equal(
            """
            <Schema Namespace="DefaultNamespace" xmlns="http://docs.oasis-open.org/odata/ns/edm">
              <Function Name="aaaaaaaaaa">
                <ReturnType Type="Edm.Int16" Nullable="false" />
              </Function>
              <EntityContainer Name="DefaultContainer">
                <FunctionImport Name="aaaaaaaaaa" Function="DefaultNamespace.aaaaaaaaaa" />
              </EntityContainer>
            </Schema>
            """, PrettifyXml(csdl));

        var isParsed = SchemaReader.TryParse(new[] { csdl }.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel edmModel, out errors);
        Assert.True(isParsed && !errors.Any());

        var sourceCsdl = XElement.Parse(this.GetSerializerResult(edmModel, edmVersion, out errors).First());

        Assert.Single(edmModel.FindOperations("DefaultNamespace.aaaaaaaaaa"));
        Assert.Empty(edmModel.FindOperations("DefaultNamespace.notexists"));

        Assert.Single(edmModel.FindDeclaredOperations("DefaultNamespace.aaaaaaaaaa"));
        Assert.Empty(edmModel.FindDeclaredOperations("DefaultNamespace.notexists"));

        Assert.Equal("DefaultContainer", edmModel.FindEntityContainer("DefaultContainer").Name);
        Assert.Null(edmModel.FindEntityContainer("NotExistsContainer"));

        Assert.Single(edmModel.FindEntityContainer("DefaultContainer").FindOperationImports("aaaaaaaaaa"));
        Assert.Empty(edmModel.FindEntityContainer("DefaultContainer").FindOperationImports("notexists"));
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void OperationsWithPrimitiveReturnType_Roundtrip_Verify(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var container = new EdmEntityContainer(DefaultNamespaceName, "DefaultContainer");
        model.AddElement(container);

        int index = 0;
        foreach (var primitiveType in ModelBuilderHelpers.AllPrimitiveEdmTypes(edmVersion, false))
        {
            var function = new EdmFunction(DefaultNamespaceName, "FunctionsWithReturnTypePrimitiveDataType" + (++index), primitiveType);
            model.AddElement(function);
            container.AddFunctionImport(function);
        }

        var csdl1 = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).First();
        Assert.Empty(errors);

        Assert.Equal(csdlForPrimitiveTypesWithoutParams, PrettifyXml(csdl1));

        var isParsed = SchemaReader.TryParse(new[] { csdl1 }.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel edmModel, out errors);
        Assert.True(isParsed && !errors.Any());

        var csdl2 = this.GetSerializerResult(edmModel, edmVersion, out errors).First();
        Assert.Empty(errors);

        Assert.Equal(csdlForPrimitiveTypesWithoutParams, PrettifyXml(csdl2));
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void OperationsWithPrimitiveReturnType_FindMethods_Verify(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var container = new EdmEntityContainer(DefaultNamespaceName, "DefaultContainer");
        model.AddElement(container);

        int index = 0;
        foreach (var primitiveType in ModelBuilderHelpers.AllPrimitiveEdmTypes(edmVersion, false))
        {
            var function = new EdmFunction(DefaultNamespaceName, "FunctionsWithReturnTypePrimitiveDataType" + (++index), primitiveType);
            model.AddElement(function);
            container.AddFunctionImport(function);
        }

        var csdl1 = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).First();
        Assert.Empty(errors);

        Assert.Equal(csdlForPrimitiveTypesWithoutParams, PrettifyXml(csdl1));

        var isParsed = SchemaReader.TryParse(new[] { csdl1 }.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel edmModel, out errors);
        Assert.True(isParsed && !errors.Any());

        var sourceCsdl = XElement.Parse(this.GetSerializerResult(edmModel, edmVersion, out errors).First());

        var functionElements = sourceCsdl.Elements().Where(e => e.Name.LocalName == "Function");
        Assert.Equal(33, functionElements.Count());

        var functionImportElements = sourceCsdl.Elements().Where(e => e.Name.LocalName == "EntityContainer").Elements().Where(e => e.Name.LocalName == "FunctionImport");
        Assert.Equal(33, functionImportElements.Count());

        Assert.Single(edmModel.FindOperations("DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType2"));
        Assert.Single(edmModel.FindDeclaredOperations("DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType2"));
        Assert.Single(edmModel.FindOperations("DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType10"));
        Assert.Single(edmModel.FindDeclaredOperations("DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType10"));
        Assert.Single(edmModel.FindOperations("DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType23"));
        Assert.Single(edmModel.FindDeclaredOperations("DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType23"));
        Assert.Single(edmModel.FindOperations("DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType33"));
        Assert.Single(edmModel.FindDeclaredOperations("DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType33"));

        Assert.Equal("DefaultContainer", edmModel.FindEntityContainer("DefaultContainer").Name);
        Assert.Single(edmModel.FindEntityContainer("DefaultContainer").FindOperationImports("FunctionsWithReturnTypePrimitiveDataType2"));
        Assert.Single(edmModel.FindEntityContainer("DefaultContainer").FindOperationImports("FunctionsWithReturnTypePrimitiveDataType10"));
        Assert.Single(edmModel.FindEntityContainer("DefaultContainer").FindOperationImports("FunctionsWithReturnTypePrimitiveDataType23"));
        Assert.Single(edmModel.FindEntityContainer("DefaultContainer").FindOperationImports("FunctionsWithReturnTypePrimitiveDataType33"));
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void OperationsWithTwoParameters_Roundtrip_Verify(EdmVersion edmVersion)
    {
        var firstParamTypes = ModelBuilderHelpers.AllPrimitiveEdmTypes(edmVersion, true)
            .Concat(
            [
                EdmCoreModel.Instance.GetBinary(isUnbounded:false, maxLength:int.MaxValue, isNullable:true),
                EdmCoreModel.Instance.GetString(isUnbounded:false, maxLength:0, isUnicode:null, isNullable:true),
                EdmCoreModel.Instance.GetDuration(false),
                EdmCoreModel.Instance.GetDecimal(precision:int.MaxValue, scale:1, isNullable:false),
                EdmCoreModel.Instance.GetDateTimeOffset(false),
            ]).ToArray();

        var secondParamTypes = new List<IEdmPrimitiveTypeReference>
        {
            EdmCoreModel.Instance.GetBinary(isUnbounded:false, maxLength:int.MaxValue, isNullable:true),
            EdmCoreModel.Instance.GetString(isUnbounded:false, maxLength:0, isUnicode:null, isNullable:true),
        };

        var returnTypes = ModelBuilderHelpers.AllPrimitiveEdmTypes(edmVersion, false).ToArray();

        var model = new EdmModel();
        var container = new EdmEntityContainer(DefaultNamespaceName, "DefaultContainer");
        model.AddElement(container);

        for (int n = 0; n < firstParamTypes.Length; n++)
        {
            var function = new EdmFunction(DefaultNamespaceName, "FunctionWith2Parameters" + n, returnTypes[n % returnTypes.Length]);
            function.AddParameter("Param1", firstParamTypes[n]);
            function.AddParameter("Param2", secondParamTypes[n % secondParamTypes.Count]);
            model.AddElement(function);
            container.AddFunctionImport(function);
        }

        var csdl1 = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).First();
        Assert.Empty(errors);
        Assert.Equal(csdlForPrimitiveTypesWithParams, PrettifyXml(csdl1));

        var isParsed = SchemaReader.TryParse(new[] { csdl1 }.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel edmModel, out errors);
        Assert.True(isParsed && !errors.Any());

        var csdl2 = this.GetSerializerResult(edmModel, edmVersion, out errors).First();
        Assert.Empty(errors);
        Assert.Equal(csdlForPrimitiveTypesWithParams, PrettifyXml(csdl2));
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void OperationsWithTwoParameters_FindMethods_Verify(EdmVersion edmVersion)
    {
        var firstParamTypes = ModelBuilderHelpers.AllPrimitiveEdmTypes(edmVersion, true)
            .Concat(
            [
                EdmCoreModel.Instance.GetBinary(isUnbounded:false, maxLength:int.MaxValue, isNullable:true),
                EdmCoreModel.Instance.GetString(isUnbounded:false, maxLength:0, isUnicode:null, isNullable:true),
                EdmCoreModel.Instance.GetDuration(false),
                EdmCoreModel.Instance.GetDecimal(precision:int.MaxValue, scale:1, isNullable:false),
                EdmCoreModel.Instance.GetDateTimeOffset(false),
            ]).ToArray();

        var secondParamTypes = new List<IEdmPrimitiveTypeReference>
        {
            EdmCoreModel.Instance.GetBinary(isUnbounded:false, maxLength:int.MaxValue, isNullable:true),
            EdmCoreModel.Instance.GetString(isUnbounded:false, maxLength:0, isUnicode:null, isNullable:true),
        };

        var returnTypes = ModelBuilderHelpers.AllPrimitiveEdmTypes(edmVersion, false).ToArray();

        var model = new EdmModel();
        var container = new EdmEntityContainer(DefaultNamespaceName, "DefaultContainer");
        model.AddElement(container);

        for (int n = 0; n < firstParamTypes.Length; n++)
        {
            var function = new EdmFunction(DefaultNamespaceName, "FunctionWith2Parameters" + n, returnTypes[n % returnTypes.Length]);
            function.AddParameter("Param1", firstParamTypes[n]);
            function.AddParameter("Param2", secondParamTypes[n % secondParamTypes.Count]);
            model.AddElement(function);
            container.AddFunctionImport(function);
        }

        var csdl1 = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).First();
        Assert.Empty(errors);
        Assert.Equal(csdlForPrimitiveTypesWithParams, PrettifyXml(csdl1));

        var isParsed = SchemaReader.TryParse(new[] { csdl1 }.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel edmModel, out errors);
        Assert.True(isParsed && !errors.Any());

        var sourceCsdl = XElement.Parse(this.GetSerializerResult(edmModel, edmVersion, out errors).First());

        var functionElements = sourceCsdl.Elements().Where(e => e.Name.LocalName == "Function");
        Assert.Equal(38, functionElements.Count());

        var functionImportElements = sourceCsdl.Elements().Where(e => e.Name.LocalName == "EntityContainer").Elements().Where(e => e.Name.LocalName == "FunctionImport");
        Assert.Equal(38, functionImportElements.Count());

        var paramElements = functionElements.Elements().Where(e => e.Name.LocalName == "Parameter");
        Assert.Equal(76, paramElements.Count());

        Assert.Single(edmModel.FindOperations("DefaultNamespace.FunctionWith2Parameters0"));
        Assert.Single(edmModel.FindDeclaredOperations("DefaultNamespace.FunctionWith2Parameters0"));
        Assert.Equal(2, edmModel.FindOperations("DefaultNamespace.FunctionWith2Parameters0").First().Parameters.Count());
        Assert.Equal("Edm.Binary", edmModel.FindOperations("DefaultNamespace.FunctionWith2Parameters0").First().FindParameter("Param1").Type.Definition.FullTypeName());

        Assert.Single(edmModel.FindOperations("DefaultNamespace.FunctionWith2Parameters10"));
        Assert.Single(edmModel.FindDeclaredOperations("DefaultNamespace.FunctionWith2Parameters10"));
        Assert.Equal(2, edmModel.FindOperations("DefaultNamespace.FunctionWith2Parameters10").First().Parameters.Count());
        Assert.Equal("Edm.SByte", edmModel.FindOperations("DefaultNamespace.FunctionWith2Parameters10").First().FindParameter("Param1").Type.Definition.FullTypeName());

        Assert.Single(edmModel.FindOperations("DefaultNamespace.FunctionWith2Parameters20"));
        Assert.Single(edmModel.FindDeclaredOperations("DefaultNamespace.FunctionWith2Parameters20"));
        Assert.Equal(2, edmModel.FindOperations("DefaultNamespace.FunctionWith2Parameters20").First().Parameters.Count());
        Assert.Equal("Edm.GeographyLineString", edmModel.FindOperations("DefaultNamespace.FunctionWith2Parameters20").First().FindParameter("Param1").Type.Definition.FullTypeName());

        Assert.Single(edmModel.FindOperations("DefaultNamespace.FunctionWith2Parameters35"));
        Assert.Single(edmModel.FindDeclaredOperations("DefaultNamespace.FunctionWith2Parameters35"));
        Assert.Equal(2, edmModel.FindOperations("DefaultNamespace.FunctionWith2Parameters35").First().Parameters.Count());
        Assert.Equal("Edm.String", edmModel.FindOperations("DefaultNamespace.FunctionWith2Parameters35").First().FindParameter("Param2").Type.Definition.FullTypeName());

        Assert.Single(edmModel.FindOperations("DefaultNamespace.FunctionWith2Parameters36"));
        Assert.Single(edmModel.FindDeclaredOperations("DefaultNamespace.FunctionWith2Parameters36"));
        Assert.Equal(2, edmModel.FindOperations("DefaultNamespace.FunctionWith2Parameters36").First().Parameters.Count());
        Assert.Equal("Edm.Decimal", edmModel.FindOperations("DefaultNamespace.FunctionWith2Parameters36").First().FindParameter("Param1").Type.Definition.FullTypeName());

        Assert.Null(edmModel.FindOperations("DefaultNamespace.FunctionWith2Parameters36").First().FindParameter("NotExistsParam"));

        Assert.Equal("DefaultContainer", edmModel.FindEntityContainer("DefaultContainer").Name);

        Assert.Single(edmModel.FindEntityContainer("DefaultContainer").FindOperationImports("FunctionWith2Parameters0"));
        Assert.Single(edmModel.FindEntityContainer("DefaultContainer").FindOperationImports("FunctionWith2Parameters10"));
        Assert.Single(edmModel.FindEntityContainer("DefaultContainer").FindOperationImports("FunctionWith2Parameters23"));
        Assert.Single(edmModel.FindEntityContainer("DefaultContainer").FindOperationImports("FunctionWith2Parameters33"));
        Assert.Single(edmModel.FindEntityContainer("DefaultContainer").FindOperationImports("FunctionWith2Parameters37"));
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void OperationsWithNamedStructuralType_Roundtrip_Verify(EdmVersion edmVersion)
    {
        var model = taupoDefaultModelEdm();

        var csdl1 = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).First();
        Assert.Empty(errors);
        Assert.Equal(taupoDefaultCsdl, PrettifyXml(csdl1));

        var isParsed = SchemaReader.TryParse(new[] { csdl1 }.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel edmModel, out errors);
        Assert.True(isParsed && !errors.Any());

        var csdl2 = this.GetSerializerResult(edmModel, edmVersion, out errors).First();
        Assert.Empty(errors);
        Assert.Equal(taupoDefaultCsdl, PrettifyXml(csdl2));
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void OperationsWithNamedStructuralType_FindMethods_Verify(EdmVersion edmVersion)
    {
        var model = taupoDefaultModelEdm();

        var csdl1 = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).First();
        Assert.Empty(errors);
        Assert.Equal(taupoDefaultCsdl, PrettifyXml(csdl1));

        var isParsed = SchemaReader.TryParse(new[] { csdl1 }.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel edmModel, out errors);
        Assert.True(isParsed && !errors.Any());

        var sourceCsdl = XElement.Parse(this.GetSerializerResult(edmModel, edmVersion, out errors).First());

        // Verify EntityTypes
        var entityTypeElements = sourceCsdl.Elements().Where(e => e.Name.LocalName == "EntityType");
        Assert.Equal(35, entityTypeElements.Count());
        var firstEntityFound = edmModel.FindType($"NS1.{entityTypeElements.First().Attribute("Name")?.Value}") as IEdmEntityType;
        Assert.Equal("SuspiciousActivity", firstEntityFound?.Name);
        Assert.Same(firstEntityFound, edmModel.FindDeclaredType("NS1.SuspiciousActivity"));

        var lastEntityFound = edmModel.FindType($"NS1.{entityTypeElements.Last().Attribute("Name")?.Value}") as IEdmEntityType;
        Assert.Equal("License", lastEntityFound?.Name);
        Assert.Same(lastEntityFound, edmModel.FindDeclaredType("NS1.License"));

        var elementTypeReferenceFound = new EdmEntityTypeReference(lastEntityFound, true);
        var found1 = elementTypeReferenceFound.FindProperty("Restrictions");
        Assert.NotNull(found1);
        Assert.Equal("Edm.String", found1.Type.Definition.FullTypeName());

        // Verify ComplexTypes
        var complexTypeElements = sourceCsdl.Elements().Where(e => e.Name.LocalName == "ComplexType");
        Assert.Equal(5, complexTypeElements.Count());
        var firstComplexTypeFound = edmModel.FindType($"NS1.{complexTypeElements.First().Attribute("Name")?.Value}") as IEdmComplexType;
        Assert.Equal("Phone", firstComplexTypeFound?.Name);
        Assert.Same(firstComplexTypeFound, edmModel.FindDeclaredType("NS1.Phone"));

        var lastComplexTypeFound = edmModel.FindType($"NS1.{complexTypeElements.Last().Attribute("Name")?.Value}") as IEdmComplexType;
        Assert.Equal("Dimensions", lastComplexTypeFound?.Name);
        Assert.Same(lastComplexTypeFound, edmModel.FindDeclaredType("NS1.Dimensions"));

        var elementComplexTypeReferenceFound = new EdmComplexTypeReference(lastComplexTypeFound, true);
        var found2 = elementComplexTypeReferenceFound.FindProperty("Height");
        Assert.NotNull(found2);
        Assert.Equal("Edm.Decimal", found2.Type.Definition.FullTypeName());

        // Verify EntityContainer
        Assert.Equal("DefaultContainer_sub", edmModel.FindEntityContainer("DefaultContainer_sub").Name);

        // Verify Properties
        var propertyElements = entityTypeElements.Elements().Where(e => e.Name.LocalName == "Property");
        Assert.Equal(104, propertyElements.Count());
        var firstPropertyFound = firstEntityFound?.FindProperty(propertyElements.First().Attribute("Name")?.Value ?? string.Empty);
        Assert.Equal("SuspiciousActivityId", firstPropertyFound?.Name);
        Assert.Equal("Edm.Int32", firstPropertyFound?.Type.Definition.FullTypeName());

        var lastPropertyFound = lastEntityFound?.FindProperty(propertyElements.Last().Attribute("Name")?.Value ?? string.Empty);
        Assert.Equal("ExpirationDate", lastPropertyFound?.Name);
        Assert.Equal("Edm.DateTimeOffset", lastPropertyFound?.Type.Definition.FullTypeName());

        // Verify Navigation Properties
        var navigationPropertyElements = entityTypeElements.Elements().Where(e => e.Name.LocalName == "NavigationProperty");
        Assert.Equal(59, navigationPropertyElements.Count());

        var entityWithNavProps = edmModel.FindType("NS1.Message") as IEdmEntityType;
        var firstNavPropertyFound = entityWithNavProps?.FindProperty(navigationPropertyElements.First().Attribute("Name")?.Value ?? string.Empty) as IEdmNavigationProperty;
        Assert.Equal("Sender", firstNavPropertyFound?.Name);
        Assert.Equal("NS1.Login", firstNavPropertyFound?.Type.Definition.FullTypeName());

        // Verify Navigation Property via EntityTypeReference
        var entityTypeReference = new EdmEntityTypeReference(entityWithNavProps, true);
        var navigationFound = entityTypeReference.FindNavigationProperty("Sender");
        Assert.NotNull(navigationFound);
        Assert.Same(navigationFound.DeclaringEntityType(), entityWithNavProps);
    }

    #region Private

    private static string PrettifyXml(string csdl)
    {
        var xml = XElement.Parse(csdl);
        var sb = new StringBuilder(xml.ToString());

        return sb.ToString().Trim();
    }

    private static string csdlForPrimitiveTypesWithoutParams =>
        """
        <Schema Namespace="DefaultNamespace" xmlns="http://docs.oasis-open.org/odata/ns/edm">
          <Function Name="FunctionsWithReturnTypePrimitiveDataType1">
            <ReturnType Type="Edm.Binary" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType2">
            <ReturnType Type="Edm.Boolean" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType3">
            <ReturnType Type="Edm.Byte" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType4">
            <ReturnType Type="Edm.DateTimeOffset" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType5">
            <ReturnType Type="Edm.Decimal" Nullable="false" Scale="variable" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType6">
            <ReturnType Type="Edm.Double" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType7">
            <ReturnType Type="Edm.Guid" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType8">
            <ReturnType Type="Edm.Int16" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType9">
            <ReturnType Type="Edm.Int32" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType10">
            <ReturnType Type="Edm.Int64" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType11">
            <ReturnType Type="Edm.SByte" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType12">
            <ReturnType Type="Edm.Single" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType13">
            <ReturnType Type="Edm.Stream" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType14">
            <ReturnType Type="Edm.String" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType15">
            <ReturnType Type="Edm.Duration" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType16">
            <ReturnType Type="Edm.Date" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType17">
            <ReturnType Type="Edm.TimeOfDay" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType18">
            <ReturnType Type="Edm.Geography" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType19">
            <ReturnType Type="Edm.GeographyPoint" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType20">
            <ReturnType Type="Edm.GeographyPolygon" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType21">
            <ReturnType Type="Edm.GeographyLineString" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType22">
            <ReturnType Type="Edm.GeographyCollection" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType23">
            <ReturnType Type="Edm.GeographyMultiPolygon" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType24">
            <ReturnType Type="Edm.GeographyMultiLineString" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType25">
            <ReturnType Type="Edm.GeographyMultiPoint" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType26">
            <ReturnType Type="Edm.Geometry" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType27">
            <ReturnType Type="Edm.GeometryPoint" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType28">
            <ReturnType Type="Edm.GeometryPolygon" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType29">
            <ReturnType Type="Edm.GeometryLineString" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType30">
            <ReturnType Type="Edm.GeometryCollection" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType31">
            <ReturnType Type="Edm.GeometryMultiPolygon" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType32">
            <ReturnType Type="Edm.GeometryMultiLineString" Nullable="false" />
          </Function>
          <Function Name="FunctionsWithReturnTypePrimitiveDataType33">
            <ReturnType Type="Edm.GeometryMultiPoint" Nullable="false" />
          </Function>
          <EntityContainer Name="DefaultContainer">
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType1" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType1" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType2" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType2" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType3" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType3" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType4" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType4" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType5" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType5" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType6" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType6" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType7" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType7" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType8" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType8" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType9" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType9" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType10" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType10" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType11" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType11" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType12" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType12" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType13" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType13" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType14" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType14" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType15" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType15" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType16" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType16" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType17" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType17" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType18" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType18" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType19" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType19" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType20" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType20" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType21" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType21" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType22" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType22" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType23" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType23" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType24" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType24" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType25" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType25" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType26" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType26" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType27" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType27" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType28" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType28" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType29" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType29" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType30" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType30" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType31" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType31" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType32" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType32" />
            <FunctionImport Name="FunctionsWithReturnTypePrimitiveDataType33" Function="DefaultNamespace.FunctionsWithReturnTypePrimitiveDataType33" />
          </EntityContainer>
        </Schema>
        """;

    private static string csdlForPrimitiveTypesWithParams =>
        """
        <Schema Namespace="DefaultNamespace" xmlns="http://docs.oasis-open.org/odata/ns/edm">
          <Function Name="FunctionWith2Parameters0">
            <Parameter Name="Param1" Type="Edm.Binary" />
            <Parameter Name="Param2" Type="Edm.Binary" MaxLength="2147483647" />
            <ReturnType Type="Edm.Binary" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters1">
            <Parameter Name="Param1" Type="Edm.Boolean" />
            <Parameter Name="Param2" Type="Edm.String" MaxLength="0" />
            <ReturnType Type="Edm.Boolean" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters2">
            <Parameter Name="Param1" Type="Edm.Byte" />
            <Parameter Name="Param2" Type="Edm.Binary" MaxLength="2147483647" />
            <ReturnType Type="Edm.Byte" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters3">
            <Parameter Name="Param1" Type="Edm.DateTimeOffset" />
            <Parameter Name="Param2" Type="Edm.String" MaxLength="0" />
            <ReturnType Type="Edm.DateTimeOffset" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters4">
            <Parameter Name="Param1" Type="Edm.Decimal" Scale="variable" />
            <Parameter Name="Param2" Type="Edm.Binary" MaxLength="2147483647" />
            <ReturnType Type="Edm.Decimal" Nullable="false" Scale="variable" />
          </Function>
          <Function Name="FunctionWith2Parameters5">
            <Parameter Name="Param1" Type="Edm.Double" />
            <Parameter Name="Param2" Type="Edm.String" MaxLength="0" />
            <ReturnType Type="Edm.Double" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters6">
            <Parameter Name="Param1" Type="Edm.Guid" />
            <Parameter Name="Param2" Type="Edm.Binary" MaxLength="2147483647" />
            <ReturnType Type="Edm.Guid" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters7">
            <Parameter Name="Param1" Type="Edm.Int16" />
            <Parameter Name="Param2" Type="Edm.String" MaxLength="0" />
            <ReturnType Type="Edm.Int16" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters8">
            <Parameter Name="Param1" Type="Edm.Int32" />
            <Parameter Name="Param2" Type="Edm.Binary" MaxLength="2147483647" />
            <ReturnType Type="Edm.Int32" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters9">
            <Parameter Name="Param1" Type="Edm.Int64" />
            <Parameter Name="Param2" Type="Edm.String" MaxLength="0" />
            <ReturnType Type="Edm.Int64" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters10">
            <Parameter Name="Param1" Type="Edm.SByte" />
            <Parameter Name="Param2" Type="Edm.Binary" MaxLength="2147483647" />
            <ReturnType Type="Edm.SByte" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters11">
            <Parameter Name="Param1" Type="Edm.Single" />
            <Parameter Name="Param2" Type="Edm.String" MaxLength="0" />
            <ReturnType Type="Edm.Single" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters12">
            <Parameter Name="Param1" Type="Edm.Stream" />
            <Parameter Name="Param2" Type="Edm.Binary" MaxLength="2147483647" />
            <ReturnType Type="Edm.Stream" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters13">
            <Parameter Name="Param1" Type="Edm.String" />
            <Parameter Name="Param2" Type="Edm.String" MaxLength="0" />
            <ReturnType Type="Edm.String" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters14">
            <Parameter Name="Param1" Type="Edm.Duration" />
            <Parameter Name="Param2" Type="Edm.Binary" MaxLength="2147483647" />
            <ReturnType Type="Edm.Duration" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters15">
            <Parameter Name="Param1" Type="Edm.Date" />
            <Parameter Name="Param2" Type="Edm.String" MaxLength="0" />
            <ReturnType Type="Edm.Date" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters16">
            <Parameter Name="Param1" Type="Edm.TimeOfDay" />
            <Parameter Name="Param2" Type="Edm.Binary" MaxLength="2147483647" />
            <ReturnType Type="Edm.TimeOfDay" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters17">
            <Parameter Name="Param1" Type="Edm.Geography" />
            <Parameter Name="Param2" Type="Edm.String" MaxLength="0" />
            <ReturnType Type="Edm.Geography" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters18">
            <Parameter Name="Param1" Type="Edm.GeographyPoint" />
            <Parameter Name="Param2" Type="Edm.Binary" MaxLength="2147483647" />
            <ReturnType Type="Edm.GeographyPoint" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters19">
            <Parameter Name="Param1" Type="Edm.GeographyPolygon" />
            <Parameter Name="Param2" Type="Edm.String" MaxLength="0" />
            <ReturnType Type="Edm.GeographyPolygon" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters20">
            <Parameter Name="Param1" Type="Edm.GeographyLineString" />
            <Parameter Name="Param2" Type="Edm.Binary" MaxLength="2147483647" />
            <ReturnType Type="Edm.GeographyLineString" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters21">
            <Parameter Name="Param1" Type="Edm.GeographyCollection" />
            <Parameter Name="Param2" Type="Edm.String" MaxLength="0" />
            <ReturnType Type="Edm.GeographyCollection" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters22">
            <Parameter Name="Param1" Type="Edm.GeographyMultiPolygon" />
            <Parameter Name="Param2" Type="Edm.Binary" MaxLength="2147483647" />
            <ReturnType Type="Edm.GeographyMultiPolygon" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters23">
            <Parameter Name="Param1" Type="Edm.GeographyMultiLineString" />
            <Parameter Name="Param2" Type="Edm.String" MaxLength="0" />
            <ReturnType Type="Edm.GeographyMultiLineString" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters24">
            <Parameter Name="Param1" Type="Edm.GeographyMultiPoint" />
            <Parameter Name="Param2" Type="Edm.Binary" MaxLength="2147483647" />
            <ReturnType Type="Edm.GeographyMultiPoint" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters25">
            <Parameter Name="Param1" Type="Edm.Geometry" />
            <Parameter Name="Param2" Type="Edm.String" MaxLength="0" />
            <ReturnType Type="Edm.Geometry" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters26">
            <Parameter Name="Param1" Type="Edm.GeometryPoint" />
            <Parameter Name="Param2" Type="Edm.Binary" MaxLength="2147483647" />
            <ReturnType Type="Edm.GeometryPoint" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters27">
            <Parameter Name="Param1" Type="Edm.GeometryPolygon" />
            <Parameter Name="Param2" Type="Edm.String" MaxLength="0" />
            <ReturnType Type="Edm.GeometryPolygon" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters28">
            <Parameter Name="Param1" Type="Edm.GeometryLineString" />
            <Parameter Name="Param2" Type="Edm.Binary" MaxLength="2147483647" />
            <ReturnType Type="Edm.GeometryLineString" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters29">
            <Parameter Name="Param1" Type="Edm.GeometryCollection" />
            <Parameter Name="Param2" Type="Edm.String" MaxLength="0" />
            <ReturnType Type="Edm.GeometryCollection" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters30">
            <Parameter Name="Param1" Type="Edm.GeometryMultiPolygon" />
            <Parameter Name="Param2" Type="Edm.Binary" MaxLength="2147483647" />
            <ReturnType Type="Edm.GeometryMultiPolygon" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters31">
            <Parameter Name="Param1" Type="Edm.GeometryMultiLineString" />
            <Parameter Name="Param2" Type="Edm.String" MaxLength="0" />
            <ReturnType Type="Edm.GeometryMultiLineString" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters32">
            <Parameter Name="Param1" Type="Edm.GeometryMultiPoint" />
            <Parameter Name="Param2" Type="Edm.Binary" MaxLength="2147483647" />
            <ReturnType Type="Edm.GeometryMultiPoint" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters33">
            <Parameter Name="Param1" Type="Edm.Binary" MaxLength="2147483647" />
            <Parameter Name="Param2" Type="Edm.String" MaxLength="0" />
            <ReturnType Type="Edm.Binary" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters34">
            <Parameter Name="Param1" Type="Edm.String" MaxLength="0" />
            <Parameter Name="Param2" Type="Edm.Binary" MaxLength="2147483647" />
            <ReturnType Type="Edm.Boolean" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters35">
            <Parameter Name="Param1" Type="Edm.Duration" Nullable="false" />
            <Parameter Name="Param2" Type="Edm.String" MaxLength="0" />
            <ReturnType Type="Edm.Byte" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters36">
            <Parameter Name="Param1" Type="Edm.Decimal" Nullable="false" Precision="2147483647" Scale="1" />
            <Parameter Name="Param2" Type="Edm.Binary" MaxLength="2147483647" />
            <ReturnType Type="Edm.DateTimeOffset" Nullable="false" />
          </Function>
          <Function Name="FunctionWith2Parameters37">
            <Parameter Name="Param1" Type="Edm.DateTimeOffset" Nullable="false" />
            <Parameter Name="Param2" Type="Edm.String" MaxLength="0" />
            <ReturnType Type="Edm.Decimal" Nullable="false" Scale="variable" />
          </Function>
          <EntityContainer Name="DefaultContainer">
            <FunctionImport Name="FunctionWith2Parameters0" Function="DefaultNamespace.FunctionWith2Parameters0" />
            <FunctionImport Name="FunctionWith2Parameters1" Function="DefaultNamespace.FunctionWith2Parameters1" />
            <FunctionImport Name="FunctionWith2Parameters2" Function="DefaultNamespace.FunctionWith2Parameters2" />
            <FunctionImport Name="FunctionWith2Parameters3" Function="DefaultNamespace.FunctionWith2Parameters3" />
            <FunctionImport Name="FunctionWith2Parameters4" Function="DefaultNamespace.FunctionWith2Parameters4" />
            <FunctionImport Name="FunctionWith2Parameters5" Function="DefaultNamespace.FunctionWith2Parameters5" />
            <FunctionImport Name="FunctionWith2Parameters6" Function="DefaultNamespace.FunctionWith2Parameters6" />
            <FunctionImport Name="FunctionWith2Parameters7" Function="DefaultNamespace.FunctionWith2Parameters7" />
            <FunctionImport Name="FunctionWith2Parameters8" Function="DefaultNamespace.FunctionWith2Parameters8" />
            <FunctionImport Name="FunctionWith2Parameters9" Function="DefaultNamespace.FunctionWith2Parameters9" />
            <FunctionImport Name="FunctionWith2Parameters10" Function="DefaultNamespace.FunctionWith2Parameters10" />
            <FunctionImport Name="FunctionWith2Parameters11" Function="DefaultNamespace.FunctionWith2Parameters11" />
            <FunctionImport Name="FunctionWith2Parameters12" Function="DefaultNamespace.FunctionWith2Parameters12" />
            <FunctionImport Name="FunctionWith2Parameters13" Function="DefaultNamespace.FunctionWith2Parameters13" />
            <FunctionImport Name="FunctionWith2Parameters14" Function="DefaultNamespace.FunctionWith2Parameters14" />
            <FunctionImport Name="FunctionWith2Parameters15" Function="DefaultNamespace.FunctionWith2Parameters15" />
            <FunctionImport Name="FunctionWith2Parameters16" Function="DefaultNamespace.FunctionWith2Parameters16" />
            <FunctionImport Name="FunctionWith2Parameters17" Function="DefaultNamespace.FunctionWith2Parameters17" />
            <FunctionImport Name="FunctionWith2Parameters18" Function="DefaultNamespace.FunctionWith2Parameters18" />
            <FunctionImport Name="FunctionWith2Parameters19" Function="DefaultNamespace.FunctionWith2Parameters19" />
            <FunctionImport Name="FunctionWith2Parameters20" Function="DefaultNamespace.FunctionWith2Parameters20" />
            <FunctionImport Name="FunctionWith2Parameters21" Function="DefaultNamespace.FunctionWith2Parameters21" />
            <FunctionImport Name="FunctionWith2Parameters22" Function="DefaultNamespace.FunctionWith2Parameters22" />
            <FunctionImport Name="FunctionWith2Parameters23" Function="DefaultNamespace.FunctionWith2Parameters23" />
            <FunctionImport Name="FunctionWith2Parameters24" Function="DefaultNamespace.FunctionWith2Parameters24" />
            <FunctionImport Name="FunctionWith2Parameters25" Function="DefaultNamespace.FunctionWith2Parameters25" />
            <FunctionImport Name="FunctionWith2Parameters26" Function="DefaultNamespace.FunctionWith2Parameters26" />
            <FunctionImport Name="FunctionWith2Parameters27" Function="DefaultNamespace.FunctionWith2Parameters27" />
            <FunctionImport Name="FunctionWith2Parameters28" Function="DefaultNamespace.FunctionWith2Parameters28" />
            <FunctionImport Name="FunctionWith2Parameters29" Function="DefaultNamespace.FunctionWith2Parameters29" />
            <FunctionImport Name="FunctionWith2Parameters30" Function="DefaultNamespace.FunctionWith2Parameters30" />
            <FunctionImport Name="FunctionWith2Parameters31" Function="DefaultNamespace.FunctionWith2Parameters31" />
            <FunctionImport Name="FunctionWith2Parameters32" Function="DefaultNamespace.FunctionWith2Parameters32" />
            <FunctionImport Name="FunctionWith2Parameters33" Function="DefaultNamespace.FunctionWith2Parameters33" />
            <FunctionImport Name="FunctionWith2Parameters34" Function="DefaultNamespace.FunctionWith2Parameters34" />
            <FunctionImport Name="FunctionWith2Parameters35" Function="DefaultNamespace.FunctionWith2Parameters35" />
            <FunctionImport Name="FunctionWith2Parameters36" Function="DefaultNamespace.FunctionWith2Parameters36" />
            <FunctionImport Name="FunctionWith2Parameters37" Function="DefaultNamespace.FunctionWith2Parameters37" />
          </EntityContainer>
        </Schema>
        """;

    private static IEdmModel taupoDefaultModelEdm()
    {
        var model = new EdmModel();

        #region TaupoDefault Model code
        var phoneType = new EdmComplexType("NS1", "Phone");
        phoneType.AddStructuralProperty("PhoneNumber", EdmCoreModel.Instance.GetString(isUnbounded: false, maxLength: 16, isUnicode: false, isNullable: false));
        phoneType.AddStructuralProperty("Extension", EdmCoreModel.Instance.GetString(isUnbounded: false, maxLength: 16, isUnicode: false, isNullable: true));
        model.AddElement(phoneType);
        var phoneTypeReference = new EdmComplexTypeReference(phoneType, false);

        var contactDetailsType = new EdmComplexType("NS1", "ContactDetails");
        contactDetailsType.AddStructuralProperty("Email", EdmCoreModel.Instance.GetString(isUnbounded: false, maxLength: 32, isUnicode: false, isNullable: false));
        contactDetailsType.AddStructuralProperty("HomePhone", phoneTypeReference);
        contactDetailsType.AddStructuralProperty("WorkPhone", phoneTypeReference);
        contactDetailsType.AddStructuralProperty("MobilePhone", phoneTypeReference);
        model.AddElement(contactDetailsType);
        var contactDetailsTypeReference = new EdmComplexTypeReference(contactDetailsType, false);

        var concurrencyInfoType = new EdmComplexType("NS1", "ConcurrencyInfo");
        concurrencyInfoType.AddStructuralProperty("Token", EdmCoreModel.Instance.GetString(isUnbounded: false, maxLength: 20, isUnicode: false, isNullable: false), string.Empty);
        concurrencyInfoType.AddStructuralProperty("QueriedDateTimeOffset", EdmCoreModel.Instance.GetDateTimeOffset(true));
        model.AddElement(concurrencyInfoType);
        var concurrencyInfoTypeReference = new EdmComplexTypeReference(concurrencyInfoType, false);

        var auditInfoType = new EdmComplexType("NS1", "AuditInfo");
        auditInfoType.AddStructuralProperty("ModifiedDate", EdmPrimitiveTypeKind.DateTimeOffset);
        auditInfoType.AddStructuralProperty("ModifiedBy", EdmCoreModel.Instance.GetString(isUnbounded: false, maxLength: 50, isUnicode: false, isNullable: false));
        auditInfoType.AddStructuralProperty("Concurrency", new EdmComplexTypeReference(concurrencyInfoType, false));
        model.AddElement(auditInfoType);
        var auditInfoTypeReference = new EdmComplexTypeReference(auditInfoType, false);

        var dimensionsType = new EdmComplexType("NS1", "Dimensions");
        dimensionsType.AddStructuralProperty("Width", EdmCoreModel.Instance.GetDecimal(10, 3, false));
        dimensionsType.AddStructuralProperty("Height", EdmCoreModel.Instance.GetDecimal(10, 3, false));
        dimensionsType.AddStructuralProperty("Depth", EdmCoreModel.Instance.GetDecimal(10, 3, false));
        model.AddElement(dimensionsType);
        var dimensionsTypeReference = new EdmComplexTypeReference(dimensionsType, false);

        var suspiciousActivityType = new EdmEntityType("NS1", "SuspiciousActivity");
        suspiciousActivityType.AddKeys(suspiciousActivityType.AddStructuralProperty("SuspiciousActivityId", EdmPrimitiveTypeKind.Int32, false));
        suspiciousActivityType.AddStructuralProperty("Activity", EdmPrimitiveTypeKind.String);
        model.AddElement(suspiciousActivityType);

        var messageType = new EdmEntityType("NS1", "Message");
        var fromUsername = messageType.AddStructuralProperty("FromUsername", EdmCoreModel.Instance.GetString(isUnbounded: false, isNullable: false, maxLength: 50, isUnicode: false));
        messageType.AddKeys(messageType.AddStructuralProperty("MessageId", EdmPrimitiveTypeKind.Int32, false), fromUsername);
        var toUsername = messageType.AddStructuralProperty("ToUsername", EdmCoreModel.Instance.GetString(isUnbounded: false, isNullable: false, maxLength: 50, isUnicode: false));
        messageType.AddStructuralProperty("Sent", EdmPrimitiveTypeKind.DateTimeOffset);
        messageType.AddStructuralProperty("Subject", EdmPrimitiveTypeKind.String);
        messageType.AddStructuralProperty("Body", EdmCoreModel.Instance.GetString(true));
        messageType.AddStructuralProperty("IsRead", EdmCoreModel.Instance.GetBoolean(false));
        model.AddElement(messageType);

        var loginType = new EdmEntityType("NS1", "Login");
        loginType.AddKeys(loginType.AddStructuralProperty("Username", EdmCoreModel.Instance.GetString(isUnbounded: false, isNullable: false, maxLength: 50, isUnicode: false)));
        var loginCustomerIdProperty = loginType.AddStructuralProperty("CustomerId", EdmPrimitiveTypeKind.Int32, false);
        model.AddElement(loginType);

        var loginSentMessages = new EdmNavigationPropertyInfo { Name = "SentMessages", Target = messageType, TargetMultiplicity = EdmMultiplicity.Many };
        var messageSender = new EdmNavigationPropertyInfo { Name = "Sender", Target = loginType, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { fromUsername }, PrincipalProperties = loginType.Key() };
        loginType.AddBidirectionalNavigation(loginSentMessages, messageSender);
        var loginReceivedMessages = new EdmNavigationPropertyInfo { Name = "ReceivedMessages", Target = messageType, TargetMultiplicity = EdmMultiplicity.Many };
        var messageRecipient = new EdmNavigationPropertyInfo { Name = "Recipient", Target = loginType, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { fromUsername }, PrincipalProperties = loginType.Key() };
        loginType.AddBidirectionalNavigation(loginReceivedMessages, messageRecipient);
        var loginSuspiciousActivity = new EdmNavigationPropertyInfo { Name = "SuspiciousActivity", Target = suspiciousActivityType, TargetMultiplicity = EdmMultiplicity.Many };
        loginType.AddUnidirectionalNavigation(loginSuspiciousActivity);

        var lastLoginType = new EdmEntityType("NS1", "LastLogin");
        var userNameProperty = lastLoginType.AddStructuralProperty("Username", EdmCoreModel.Instance.GetString(isUnbounded: false, isNullable: false, maxLength: 50, isUnicode: false));
        lastLoginType.AddKeys(userNameProperty);
        lastLoginType.AddStructuralProperty("LoggedIn", EdmPrimitiveTypeKind.DateTimeOffset);
        lastLoginType.AddStructuralProperty("LoggedOut", EdmCoreModel.Instance.GetDateTimeOffset(true));
        model.AddElement(lastLoginType);

        var loginLastLogin = new EdmNavigationPropertyInfo { Name = "LastLogin", Target = lastLoginType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne };
        var lastLoginLogin = new EdmNavigationPropertyInfo { Name = "Login", Target = loginType, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { userNameProperty }, PrincipalProperties = loginType.Key() };
        lastLoginType.AddBidirectionalNavigation(lastLoginLogin, loginLastLogin);

        var orderType = new EdmEntityType("NS1", "Order");
        orderType.AddKeys(orderType.AddStructuralProperty("OrderId", EdmPrimitiveTypeKind.Int32, false));
        var orderCustomerId = orderType.AddStructuralProperty("CustomerId", EdmCoreModel.Instance.GetInt32(true));
        orderType.AddStructuralProperty("Concurrency", concurrencyInfoTypeReference);
        model.AddElement(orderType);

        var loginOrders = new EdmNavigationPropertyInfo { Name = "Orders", Target = orderType, TargetMultiplicity = EdmMultiplicity.Many };
        var orderLogin = new EdmNavigationPropertyInfo { Name = "Login", Target = loginType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne };
        orderType.AddBidirectionalNavigation(orderLogin, loginOrders);

        var customerInfoType = new EdmEntityType("NS1", "CustomerInfo");
        customerInfoType.AddKeys(customerInfoType.AddStructuralProperty("CustomerInfoId", EdmPrimitiveTypeKind.Int32, false));
        customerInfoType.AddStructuralProperty("Information", EdmPrimitiveTypeKind.String);
        model.AddElement(customerInfoType);

        var customerType = new EdmEntityType("NS1", "Customer");
        var customerIdProperty = customerType.AddStructuralProperty("CustomerId", EdmPrimitiveTypeKind.Int32, false);
        customerType.AddKeys(customerIdProperty);
        customerType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(isUnbounded: false, maxLength: 100, isUnicode: false, isNullable: false));
        customerType.AddStructuralProperty("ContactInfo", contactDetailsTypeReference);
        model.AddElement(customerType);

        var customerOrders = new EdmNavigationPropertyInfo { Name = "Orders", Target = orderType, TargetMultiplicity = EdmMultiplicity.Many, };
        var orderCustomer = new EdmNavigationPropertyInfo { Name = "Customer", Target = customerType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, DependentProperties = new[] { orderCustomerId }, PrincipalProperties = customerType.Key() };
        customerType.AddBidirectionalNavigation(customerOrders, orderCustomer);
        var customerLogins = new EdmNavigationPropertyInfo { Name = "Logins", Target = loginType, TargetMultiplicity = EdmMultiplicity.Many, };
        var loginCustomer = new EdmNavigationPropertyInfo { Name = "Customer", Target = customerType, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { loginCustomerIdProperty }, PrincipalProperties = customerType.Key() };
        customerType.AddBidirectionalNavigation(customerLogins, loginCustomer);
        var customerHusband = new EdmNavigationPropertyInfo { Name = "Husband", Target = customerType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne };
        var customerWife = new EdmNavigationPropertyInfo { Name = "Wife", Target = customerType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne };
        customerType.AddBidirectionalNavigation(customerHusband, customerWife);
        var customerInfo = new EdmNavigationPropertyInfo { Name = "CustomerInfo", Target = customerInfoType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne };
        customerType.AddUnidirectionalNavigation(customerInfo);

        var productType = new EdmEntityType("NS1", "Product");
        productType.AddKeys(productType.AddStructuralProperty("ProductId", EdmPrimitiveTypeKind.Int32, false));
        productType.AddStructuralProperty("Description", EdmCoreModel.Instance.GetString(isUnbounded: false, isNullable: true, maxLength: 1000, isUnicode: false));
        productType.AddStructuralProperty("Dimensions", dimensionsTypeReference);
        productType.AddStructuralProperty("BaseConcurrency", EdmCoreModel.Instance.GetString(false), string.Empty);
        productType.AddStructuralProperty("ComplexConcurrency", concurrencyInfoTypeReference);
        productType.AddStructuralProperty("NestedComplexConcurrency", auditInfoTypeReference);
        model.AddElement(productType);

        var barCodeType = new EdmEntityType("NS1", "Barcode");
        barCodeType.AddKeys(barCodeType.AddStructuralProperty("Code", EdmCoreModel.Instance.GetBinary(isUnbounded: false, isNullable: false, maxLength: 50)));
        var barCodeProductIdProperty = barCodeType.AddStructuralProperty("ProductId", EdmPrimitiveTypeKind.Int32, false);
        barCodeType.AddStructuralProperty("Text", EdmPrimitiveTypeKind.String);
        model.AddElement(barCodeType);

        var productBarCodes = new EdmNavigationPropertyInfo { Name = "Barcodes", Target = barCodeType, TargetMultiplicity = EdmMultiplicity.Many };
        var barCodeProduct = new EdmNavigationPropertyInfo { Name = "Product", Target = productType, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { barCodeProductIdProperty }, PrincipalProperties = productType.Key() };
        barCodeType.AddBidirectionalNavigation(barCodeProduct, productBarCodes);

        var incorrectScanType = new EdmEntityType("NS1", "IncorrectScan");
        incorrectScanType.AddKeys(incorrectScanType.AddStructuralProperty("IncorrectScanId", EdmPrimitiveTypeKind.Int32, false));
        var expectedCodeProperty = incorrectScanType.AddStructuralProperty("ExpectedCode", EdmCoreModel.Instance.GetBinary(isUnbounded: false, isNullable: false, maxLength: 50));
        var actualCodeProperty = incorrectScanType.AddStructuralProperty("ActualCode", EdmCoreModel.Instance.GetBinary(isUnbounded: false, isNullable: true, maxLength: 50));
        incorrectScanType.AddStructuralProperty("ScanDate", EdmPrimitiveTypeKind.DateTimeOffset);
        incorrectScanType.AddStructuralProperty("Details", EdmPrimitiveTypeKind.String);
        model.AddElement(incorrectScanType);

        var barCodeIncorrectScan = new EdmNavigationPropertyInfo { Name = "BadScans", Target = incorrectScanType, TargetMultiplicity = EdmMultiplicity.Many };
        var incorrectScanExpectedBarCode = new EdmNavigationPropertyInfo { Name = "ExpectedBarcode", Target = barCodeType, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { expectedCodeProperty }, PrincipalProperties = barCodeType.Key() };
        incorrectScanType.AddBidirectionalNavigation(incorrectScanExpectedBarCode, barCodeIncorrectScan);
        var actualBarcode = new EdmNavigationPropertyInfo { Name = "ActualBarcode", Target = barCodeType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, DependentProperties = new[] { actualCodeProperty }, PrincipalProperties = barCodeType.Key() };
        incorrectScanType.AddUnidirectionalNavigation(actualBarcode);

        var barCodeDetailType = new EdmEntityType("NS1", "BarcodeDetail");
        var codeProperty = barCodeDetailType.AddStructuralProperty("Code", EdmCoreModel.Instance.GetBinary(isUnbounded: false, isNullable: false, maxLength: 50));
        barCodeDetailType.AddKeys(codeProperty);
        barCodeDetailType.AddStructuralProperty("RegisteredTo", EdmPrimitiveTypeKind.String);
        model.AddElement(barCodeDetailType);

        var barCodeDetail = new EdmNavigationPropertyInfo { Name = "Detail", Target = barCodeDetailType, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = barCodeType.Key(), PrincipalProperties = barCodeDetailType.Key() };
        barCodeType.AddUnidirectionalNavigation(barCodeDetail);

        var resolutionType = new EdmEntityType("NS1", "Resolution");
        resolutionType.AddKeys(resolutionType.AddStructuralProperty("ResolutionId", EdmPrimitiveTypeKind.Int32, false));
        resolutionType.AddStructuralProperty("Details", EdmPrimitiveTypeKind.String);
        model.AddElement(resolutionType);

        var complaintType = new EdmEntityType("NS1", "Complaint");
        complaintType.AddKeys(complaintType.AddStructuralProperty("ComplaintId", EdmPrimitiveTypeKind.Int32, false));
        var complaintCustomerId = complaintType.AddStructuralProperty("CustomerId", EdmCoreModel.Instance.GetInt32(true));
        complaintType.AddStructuralProperty("Logged", EdmPrimitiveTypeKind.DateTimeOffset);
        complaintType.AddStructuralProperty("Details", EdmPrimitiveTypeKind.String);
        model.AddElement(complaintType);

        var complaintCustomer = new EdmNavigationPropertyInfo { Name = "Customer", Target = customerType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, DependentProperties = new[] { complaintCustomerId }, PrincipalProperties = customerType.Key() };
        complaintType.AddUnidirectionalNavigation(complaintCustomer);
        var complaintResolution = new EdmNavigationPropertyInfo { Name = "Resolution", Target = resolutionType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne };
        var resolutionComplaint = new EdmNavigationPropertyInfo { Name = "Complaint", Target = complaintType, TargetMultiplicity = EdmMultiplicity.One };
        complaintType.AddBidirectionalNavigation(complaintResolution, resolutionComplaint);

        var smartCardType = new EdmEntityType("NS1", "SmartCard");
        var smartCardUsername = smartCardType.AddStructuralProperty("Username", EdmCoreModel.Instance.GetString(isUnbounded: false, isNullable: false, maxLength: 50, isUnicode: false));
        smartCardType.AddKeys(smartCardUsername);
        smartCardType.AddStructuralProperty("CardSerial", EdmPrimitiveTypeKind.String);
        smartCardType.AddStructuralProperty("Issued", EdmPrimitiveTypeKind.DateTimeOffset);
        model.AddElement(smartCardType);

        var smartCardLogin = new EdmNavigationPropertyInfo { Name = "Login", Target = loginType, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { smartCardUsername }, PrincipalProperties = loginType.Key() };
        smartCardType.AddUnidirectionalNavigation(smartCardLogin);
        var smartCardLastLogin = new EdmNavigationPropertyInfo { Name = "LastLogin", Target = lastLoginType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne };
        smartCardType.AddUnidirectionalNavigation(smartCardLastLogin);

        var rsaTokenType = new EdmEntityType("NS1", "RSAToken");
        rsaTokenType.AddKeys(rsaTokenType.AddStructuralProperty("Serial", EdmCoreModel.Instance.GetString(isUnbounded: false, isNullable: false, maxLength: 20, isUnicode: false)));
        rsaTokenType.AddStructuralProperty("Issued", EdmPrimitiveTypeKind.DateTimeOffset);
        model.AddElement(rsaTokenType);

        var rsaTokenLogin = new EdmNavigationPropertyInfo { Name = "Login", Target = loginType, TargetMultiplicity = EdmMultiplicity.One };
        rsaTokenType.AddUnidirectionalNavigation(rsaTokenLogin);

        var passwordResetType = new EdmEntityType("NS1", "PasswordReset");
        passwordResetType.AddKeys(passwordResetType.AddStructuralProperty("ResetNo", EdmPrimitiveTypeKind.Int32, false));
        var passwordResetUsername = passwordResetType.AddStructuralProperty("Username", EdmCoreModel.Instance.GetString(isUnbounded: false, isNullable: false, maxLength: 50, isUnicode: false));
        passwordResetType.AddStructuralProperty("TempPassword", EdmPrimitiveTypeKind.String);
        passwordResetType.AddStructuralProperty("EmailedTo", EdmPrimitiveTypeKind.String);
        model.AddElement(passwordResetType);

        var passwordResetLogin = new EdmNavigationPropertyInfo { Name = "Login", Target = loginType, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { passwordResetUsername }, PrincipalProperties = loginType.Key() };
        passwordResetType.AddUnidirectionalNavigation(passwordResetLogin);

        var pageViewType = new EdmEntityType("NS1", "PageView");
        pageViewType.AddKeys(pageViewType.AddStructuralProperty("PageViewId", EdmPrimitiveTypeKind.Int32, false));
        var pageViewUsername = pageViewType.AddStructuralProperty("Username", EdmCoreModel.Instance.GetString(isUnbounded: false, isNullable: false, maxLength: 50, isUnicode: false));
        pageViewType.AddStructuralProperty("Viewed", EdmPrimitiveTypeKind.DateTimeOffset);
        pageViewType.AddStructuralProperty("PageUrl", EdmCoreModel.Instance.GetString(isUnbounded: false, isNullable: false, maxLength: 500, isUnicode: false));
        model.AddElement(pageViewType);

        var pageViewLogin = new EdmNavigationPropertyInfo { Name = "Login", Target = loginType, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { pageViewUsername }, PrincipalProperties = loginType.Key() };
        pageViewType.AddUnidirectionalNavigation(pageViewLogin);

        var productPageViewType = new EdmEntityType("NS1", "ProductPageView", pageViewType);
        var productPageViewProductId = productPageViewType.AddStructuralProperty("ProductId", EdmPrimitiveTypeKind.Int32, false);
        model.AddElement(productPageViewType);

        var productPageViewProduct = new EdmNavigationPropertyInfo { Name = "Product", Target = productType, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { productPageViewProductId }, PrincipalProperties = productType.Key() };
        productPageViewType.AddUnidirectionalNavigation(productPageViewProduct);

        var supplierType = new EdmEntityType("NS1", "Supplier");
        supplierType.AddKeys(supplierType.AddStructuralProperty("SupplierId", EdmPrimitiveTypeKind.Int32, false));
        supplierType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
        model.AddElement(supplierType);

        var supplierProducts = new EdmNavigationPropertyInfo { Name = "Products", Target = productType, TargetMultiplicity = EdmMultiplicity.Many };
        var productSuppliers = new EdmNavigationPropertyInfo { Name = "Suppliers", Target = supplierType, TargetMultiplicity = EdmMultiplicity.Many };
        supplierType.AddBidirectionalNavigation(supplierProducts, productSuppliers);

        var supplierLogoType = new EdmEntityType("NS1", "SupplierLogo");
        var supplierLogoSupplierId = supplierLogoType.AddStructuralProperty("SupplierId", EdmPrimitiveTypeKind.Int32, false);
        supplierLogoType.AddKeys(supplierLogoSupplierId);
        supplierLogoType.AddStructuralProperty("Logo", EdmCoreModel.Instance.GetBinary(isNullable: false, isUnbounded: false, maxLength: 500));
        model.AddElement(supplierLogoType);

        var supplierSupplierLogo = new EdmNavigationPropertyInfo { Name = "Logo", Target = supplierLogoType, TargetMultiplicity = EdmMultiplicity.One, PrincipalProperties = new[] { supplierLogoSupplierId }, DependentProperties = supplierType.Key() };
        supplierType.AddUnidirectionalNavigation(supplierSupplierLogo);

        var supplierInfoType = new EdmEntityType("NS1", "SupplierInfo");
        supplierInfoType.AddKeys(supplierInfoType.AddStructuralProperty("SupplierInfoId", EdmPrimitiveTypeKind.Int32, false));
        supplierInfoType.AddStructuralProperty("Information", EdmPrimitiveTypeKind.String);
        model.AddElement(supplierInfoType);

        var supplierInfoSupplier = new EdmNavigationPropertyInfo { Name = "Supplier", Target = supplierType, TargetMultiplicity = EdmMultiplicity.One, OnDelete = EdmOnDeleteAction.Cascade };
        supplierInfoType.AddUnidirectionalNavigation(supplierInfoSupplier);

        var orderNoteType = new EdmEntityType("NS1", "OrderNote");
        orderNoteType.AddKeys(orderNoteType.AddStructuralProperty("NoteId", EdmPrimitiveTypeKind.Int32, false));
        orderNoteType.AddStructuralProperty("Note", EdmPrimitiveTypeKind.String);
        model.AddElement(orderNoteType);

        var orderNoteOrder = new EdmNavigationPropertyInfo { Name = "Order", Target = orderType, TargetMultiplicity = EdmMultiplicity.One };
        var orderOrderNotes = new EdmNavigationPropertyInfo { Name = "Notes", Target = orderNoteType, TargetMultiplicity = EdmMultiplicity.Many, OnDelete = EdmOnDeleteAction.Cascade };
        orderNoteType.AddBidirectionalNavigation(orderNoteOrder, orderOrderNotes);

        var orderQualityCheckType = new EdmEntityType("NS1", "OrderQualityCheck");
        var orderQualityCheckOrderId = orderQualityCheckType.AddStructuralProperty("OrderId", EdmPrimitiveTypeKind.Int32, false);
        orderQualityCheckType.AddKeys(orderQualityCheckOrderId);
        orderQualityCheckType.AddStructuralProperty("CheckedBy", EdmPrimitiveTypeKind.String);
        orderQualityCheckType.AddStructuralProperty("CheckedDateTime", EdmPrimitiveTypeKind.DateTimeOffset);
        model.AddElement(orderQualityCheckType);

        var orderQualityCheckOrder = new EdmNavigationPropertyInfo { Name = "Order", Target = orderType, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { orderQualityCheckOrderId }, PrincipalProperties = orderType.Key() };
        orderQualityCheckType.AddUnidirectionalNavigation(orderQualityCheckOrder);

        var orderLineType = new EdmEntityType("NS1", "OrderLine");
        var orderLineOrderId = orderLineType.AddStructuralProperty("OrderId", EdmPrimitiveTypeKind.Int32, false);
        var orderLineProductId = orderLineType.AddStructuralProperty("ProductId", EdmPrimitiveTypeKind.Int32, false);
        orderLineType.AddKeys(orderLineOrderId, orderLineProductId);
        orderLineType.AddStructuralProperty("Quantity", EdmPrimitiveTypeKind.Int32);
        orderLineType.AddStructuralProperty("ConcurrencyToken", EdmCoreModel.Instance.GetString(false), string.Empty);
        model.AddElement(orderLineType);

        var orderLineOrder = new EdmNavigationPropertyInfo { Name = "Order", Target = orderType, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { orderLineOrderId }, PrincipalProperties = orderType.Key() };
        var orderOrderLine = new EdmNavigationPropertyInfo { Name = "OrderLines", Target = orderLineType, TargetMultiplicity = EdmMultiplicity.Many };
        orderLineType.AddBidirectionalNavigation(orderLineOrder, orderOrderLine);

        var orderLineProduct = new EdmNavigationPropertyInfo { Name = "Product", Target = productType, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { orderLineProductId }, PrincipalProperties = productType.Key() };
        orderLineType.AddUnidirectionalNavigation(orderLineProduct);

        var backOrderLineType = new EdmEntityType("NS1", "BackOrderLine", orderLineType);
        backOrderLineType.AddStructuralProperty("ETA", EdmPrimitiveTypeKind.DateTimeOffset);
        model.AddElement(backOrderLineType);

        var backOrderLineSupplier = new EdmNavigationPropertyInfo { Name = "Supplier", Target = supplierType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne };
        var supplierBackOrderLines = new EdmNavigationPropertyInfo { Name = "BackOrderLines", Target = backOrderLineType, TargetMultiplicity = EdmMultiplicity.Many };
        backOrderLineType.AddBidirectionalNavigation(backOrderLineSupplier, supplierBackOrderLines);

        var backOrderLine2Type = new EdmEntityType("NS1", "BackOrderLine2", backOrderLineType);
        model.AddElement(backOrderLine2Type);

        var discontinuedProductType = new EdmEntityType("NS1", "DiscontinuedProduct", productType);
        discontinuedProductType.AddStructuralProperty("Discontinued", EdmPrimitiveTypeKind.DateTimeOffset);
        var replacementProductId = discontinuedProductType.AddStructuralProperty("ReplacementProductId", EdmCoreModel.Instance.GetInt32(true));
        model.AddElement(discontinuedProductType);

        var discontinuedProductReplacement = new EdmNavigationPropertyInfo { Name = "ReplacedBy", Target = productType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, DependentProperties = new[] { replacementProductId }, PrincipalProperties = productType.Key() };
        var productReplaces = new EdmNavigationPropertyInfo { Name = "Replaces", Target = discontinuedProductType, TargetMultiplicity = EdmMultiplicity.Many, };
        discontinuedProductType.AddBidirectionalNavigation(discontinuedProductReplacement, productReplaces);

        var productDetailType = new EdmEntityType("NS1", "ProductDetail");
        var productDetailProductId = productDetailType.AddStructuralProperty("ProductId", EdmPrimitiveTypeKind.Int32, false);
        productDetailType.AddKeys(productDetailProductId);
        productDetailType.AddStructuralProperty("Details", EdmPrimitiveTypeKind.String);
        model.AddElement(productDetailType);

        var productDetailProduct = new EdmNavigationPropertyInfo { Name = "Product", Target = productType, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { productDetailProductId }, PrincipalProperties = productType.Key() };
        var productProductDetail = new EdmNavigationPropertyInfo { Name = "Detail", Target = productDetailType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne };
        productDetailType.AddBidirectionalNavigation(productDetailProduct, productProductDetail);

        var productReviewType = new EdmEntityType("NS1", "ProductReview");
        var productReviewProductId = productReviewType.AddStructuralProperty("ProductId", EdmPrimitiveTypeKind.Int32, false);
        productReviewType.AddKeys(productReviewProductId, productReviewType.AddStructuralProperty("ReviewId", EdmPrimitiveTypeKind.Int32, false));
        productReviewType.AddStructuralProperty("Review", EdmPrimitiveTypeKind.String);
        model.AddElement(productReviewType);

        var productReviewProduct = new EdmNavigationPropertyInfo { Name = "Product", Target = productType, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { productReviewProductId }, PrincipalProperties = productType.Key() };
        var productProductReviews = new EdmNavigationPropertyInfo { Name = "Reviews", Target = productReviewType, TargetMultiplicity = EdmMultiplicity.Many };
        productReviewType.AddBidirectionalNavigation(productReviewProduct, productProductReviews);

        var productPhotoType = new EdmEntityType("NS1", "ProductPhoto");
        var productPhotoProductId = productPhotoType.AddStructuralProperty("ProductId", EdmPrimitiveTypeKind.Int32, false);
        productPhotoType.AddKeys(productPhotoProductId, productPhotoType.AddStructuralProperty("PhotoId", EdmPrimitiveTypeKind.Int32, false));
        productPhotoType.AddStructuralProperty("Photo", EdmPrimitiveTypeKind.Binary);
        model.AddElement(productPhotoType);

        var productProductPhotos = new EdmNavigationPropertyInfo { Name = "Photos", Target = productPhotoType, TargetMultiplicity = EdmMultiplicity.Many };
        var productPhotoProduct = new EdmNavigationPropertyInfo { Name = "Product", Target = productType, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { productPhotoProductId }, PrincipalProperties = productType.Key() };
        productType.AddBidirectionalNavigation(productProductPhotos, productPhotoProduct);

        var productWebFeatureType = new EdmEntityType("NS1", "ProductWebFeature");
        productWebFeatureType.AddKeys(productWebFeatureType.AddStructuralProperty("FeatureId", EdmPrimitiveTypeKind.Int32, false));
        var productWebFeatureProductId = productWebFeatureType.AddStructuralProperty("ProductId", EdmCoreModel.Instance.GetInt32(true));
        var productWebFeaturePhotoId = productWebFeatureType.AddStructuralProperty("PhotoId", EdmCoreModel.Instance.GetInt32(true));
        var productWebFeatureReviewId = productWebFeatureType.AddStructuralProperty("ReviewId", EdmCoreModel.Instance.GetInt32(true));
        productWebFeatureType.AddStructuralProperty("Heading", EdmPrimitiveTypeKind.String);
        model.AddElement(productWebFeatureType);

        var productReviewWebFeatures = new EdmNavigationPropertyInfo { Name = "Features", Target = productWebFeatureType, TargetMultiplicity = EdmMultiplicity.Many };
        var productWebFeatureReview = new EdmNavigationPropertyInfo { Name = "Review", Target = productReviewType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, DependentProperties = new[] { productWebFeatureReviewId, productWebFeatureProductId }, PrincipalProperties = productReviewType.Key() };
        productWebFeatureType.AddBidirectionalNavigation(productWebFeatureReview, productReviewWebFeatures);

        var productPhotoWebFeatures = new EdmNavigationPropertyInfo { Name = "Features", Target = productWebFeatureType, TargetMultiplicity = EdmMultiplicity.Many };
        var productWebFeaturePhoto = new EdmNavigationPropertyInfo { Name = "Photo", Target = productPhotoType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, DependentProperties = new[] { productWebFeaturePhotoId, productWebFeatureProductId }, PrincipalProperties = productPhotoType.Key() };
        productWebFeatureType.AddBidirectionalNavigation(productWebFeaturePhoto, productPhotoWebFeatures);

        var computerType = new EdmEntityType("NS1", "Computer");
        computerType.AddKeys(computerType.AddStructuralProperty("ComputerId", EdmPrimitiveTypeKind.Int32, false));
        computerType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
        model.AddElement(computerType);

        var computerDetailType = new EdmEntityType("NS1", "ComputerDetail");
        computerDetailType.AddKeys(computerDetailType.AddStructuralProperty("ComputerDetailId", EdmPrimitiveTypeKind.Int32, false));
        computerDetailType.AddStructuralProperty("Model", EdmPrimitiveTypeKind.String);
        computerDetailType.AddStructuralProperty("Serial", EdmPrimitiveTypeKind.String);
        computerDetailType.AddStructuralProperty("Specifications", EdmPrimitiveTypeKind.String);
        computerDetailType.AddStructuralProperty("PurchaseDate", EdmPrimitiveTypeKind.DateTimeOffset);
        computerDetailType.AddStructuralProperty("Dimensions", dimensionsTypeReference);
        model.AddElement(computerDetailType);

        var computerDetailComputer = new EdmNavigationPropertyInfo { Name = "Computer", Target = computerType, TargetMultiplicity = EdmMultiplicity.One };
        var computerComputerDetail = new EdmNavigationPropertyInfo { Name = "ComputerDetail", Target = computerDetailType, TargetMultiplicity = EdmMultiplicity.One };
        computerType.AddBidirectionalNavigation(computerComputerDetail, computerDetailComputer);

        var driverType = new EdmEntityType("NS1", "Driver");
        driverType.AddKeys(driverType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(isUnbounded: false, maxLength: 100, isNullable: false, isUnicode: false)));
        driverType.AddStructuralProperty("BirthDate", EdmPrimitiveTypeKind.DateTimeOffset);
        model.AddElement(driverType);

        var licenseType = new EdmEntityType("NS1", "License");
        var licenseDriverName = licenseType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(isUnbounded: false, maxLength: 100, isNullable: false, isUnicode: false));
        licenseType.AddKeys(licenseDriverName);
        licenseType.AddStructuralProperty("LicenseNumber", EdmPrimitiveTypeKind.String);
        licenseType.AddStructuralProperty("LicenseClass", EdmPrimitiveTypeKind.String);
        licenseType.AddStructuralProperty("Restrictions", EdmPrimitiveTypeKind.String);
        licenseType.AddStructuralProperty("ExpirationDate", EdmPrimitiveTypeKind.DateTimeOffset);
        model.AddElement(licenseType);

        var driverLicense = new EdmNavigationPropertyInfo { Name = "License", Target = licenseType, TargetMultiplicity = EdmMultiplicity.One, };
        var licenseDriver = new EdmNavigationPropertyInfo { Name = "Driver", Target = driverType, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { licenseDriverName }, PrincipalProperties = driverType.Key() };
        licenseType.AddBidirectionalNavigation(licenseDriver, driverLicense);

        #endregion

        var container = new EdmEntityContainer("NS1", "DefaultContainer_sub");
        model.AddElement(container);

        // Create EntitySet
        var entityTypes = model.SchemaElements.OfType<IEdmEntityType>().Where(e => e.BaseType == null && container.EntitySets().All(set => set.EntityType != e));
        foreach (EdmEntityType entityType in entityTypes.Cast<EdmEntityType>())
        {
            container.AddEntitySet(entityType.Name, entityType);
        }

        // Create NavigationPropertyBinding
        IEdmEntitySet? findEntitySet(IEdmEntityType? entityType)
        {
            var searchType = entityType;
            while (searchType != null)
            {
                var entitySet = container.EntitySets().FirstOrDefault(s => s.EntityType == searchType);
                if (entitySet != null)
                {
                    return entitySet;
                }

                searchType = searchType.BaseEntityType();
            }
            return null;
        }

        var navigationPropertyBindings = model.SchemaElements.OfType<IEdmEntityType>().SelectMany(entityType => entityType.DeclaredNavigationProperties()).Where(p => !p.ContainsTarget);
        foreach (EdmNavigationProperty property in navigationPropertyBindings.Cast<EdmNavigationProperty>())
        {
            var targetEntityType = (property.Type.Definition.TypeKind == EdmTypeKind.Collection)
                                        ? ((property.Type.Definition as EdmCollectionType).ElementType.Definition as EdmEntityType)
                                        : (property.Type.Definition as EdmEntityType);

            var targetEntitySet = findEntitySet(targetEntityType);
            if (findEntitySet(property.DeclaringType as IEdmEntityType) is EdmEntitySet sourceEntitySet && sourceEntitySet.NavigationPropertyBindings.All(target => target.NavigationProperty != property))
            {
                sourceEntitySet.AddNavigationTarget(property, targetEntitySet);
            }
        }

        return model;
    }

    private static string taupoDefaultCsdl =>
        """
        <Schema Namespace="NS1" xmlns="http://docs.oasis-open.org/odata/ns/edm">
          <ComplexType Name="Phone">
            <Property Name="PhoneNumber" Type="Edm.String" Nullable="false" MaxLength="16" Unicode="false" />
            <Property Name="Extension" Type="Edm.String" MaxLength="16" Unicode="false" />
          </ComplexType>
          <ComplexType Name="ContactDetails">
            <Property Name="Email" Type="Edm.String" Nullable="false" MaxLength="32" Unicode="false" />
            <Property Name="HomePhone" Type="NS1.Phone" Nullable="false" />
            <Property Name="WorkPhone" Type="NS1.Phone" Nullable="false" />
            <Property Name="MobilePhone" Type="NS1.Phone" Nullable="false" />
          </ComplexType>
          <ComplexType Name="ConcurrencyInfo">
            <Property Name="Token" Type="Edm.String" DefaultValue="" Nullable="false" MaxLength="20" Unicode="false" />
            <Property Name="QueriedDateTimeOffset" Type="Edm.DateTimeOffset" />
          </ComplexType>
          <ComplexType Name="AuditInfo">
            <Property Name="ModifiedDate" Type="Edm.DateTimeOffset" />
            <Property Name="ModifiedBy" Type="Edm.String" Nullable="false" MaxLength="50" Unicode="false" />
            <Property Name="Concurrency" Type="NS1.ConcurrencyInfo" Nullable="false" />
          </ComplexType>
          <ComplexType Name="Dimensions">
            <Property Name="Width" Type="Edm.Decimal" Nullable="false" Precision="10" Scale="3" />
            <Property Name="Height" Type="Edm.Decimal" Nullable="false" Precision="10" Scale="3" />
            <Property Name="Depth" Type="Edm.Decimal" Nullable="false" Precision="10" Scale="3" />
          </ComplexType>
          <EntityType Name="SuspiciousActivity">
            <Key>
              <PropertyRef Name="SuspiciousActivityId" />
            </Key>
            <Property Name="SuspiciousActivityId" Type="Edm.Int32" Nullable="false" />
            <Property Name="Activity" Type="Edm.String" />
          </EntityType>
          <EntityType Name="Message">
            <Key>
              <PropertyRef Name="MessageId" />
              <PropertyRef Name="FromUsername" />
            </Key>
            <Property Name="FromUsername" Type="Edm.String" Nullable="false" MaxLength="50" Unicode="false" />
            <Property Name="MessageId" Type="Edm.Int32" Nullable="false" />
            <Property Name="ToUsername" Type="Edm.String" Nullable="false" MaxLength="50" Unicode="false" />
            <Property Name="Sent" Type="Edm.DateTimeOffset" />
            <Property Name="Subject" Type="Edm.String" />
            <Property Name="Body" Type="Edm.String" />
            <Property Name="IsRead" Type="Edm.Boolean" Nullable="false" />
            <NavigationProperty Name="Sender" Type="NS1.Login" Nullable="false" Partner="SentMessages">
              <ReferentialConstraint Property="FromUsername" ReferencedProperty="Username" />
            </NavigationProperty>
            <NavigationProperty Name="Recipient" Type="NS1.Login" Nullable="false" Partner="ReceivedMessages">
              <ReferentialConstraint Property="FromUsername" ReferencedProperty="Username" />
            </NavigationProperty>
          </EntityType>
          <EntityType Name="Login">
            <Key>
              <PropertyRef Name="Username" />
            </Key>
            <Property Name="Username" Type="Edm.String" Nullable="false" MaxLength="50" Unicode="false" />
            <Property Name="CustomerId" Type="Edm.Int32" Nullable="false" />
            <NavigationProperty Name="SentMessages" Type="Collection(NS1.Message)" Partner="Sender" />
            <NavigationProperty Name="ReceivedMessages" Type="Collection(NS1.Message)" Partner="Recipient" />
            <NavigationProperty Name="SuspiciousActivity" Type="Collection(NS1.SuspiciousActivity)" />
            <NavigationProperty Name="LastLogin" Type="NS1.LastLogin" Partner="Login" />
            <NavigationProperty Name="Orders" Type="Collection(NS1.Order)" Partner="Login" />
            <NavigationProperty Name="Customer" Type="NS1.Customer" Nullable="false" Partner="Logins">
              <ReferentialConstraint Property="CustomerId" ReferencedProperty="CustomerId" />
            </NavigationProperty>
          </EntityType>
          <EntityType Name="LastLogin">
            <Key>
              <PropertyRef Name="Username" />
            </Key>
            <Property Name="Username" Type="Edm.String" Nullable="false" MaxLength="50" Unicode="false" />
            <Property Name="LoggedIn" Type="Edm.DateTimeOffset" />
            <Property Name="LoggedOut" Type="Edm.DateTimeOffset" />
            <NavigationProperty Name="Login" Type="NS1.Login" Nullable="false" Partner="LastLogin">
              <ReferentialConstraint Property="Username" ReferencedProperty="Username" />
            </NavigationProperty>
          </EntityType>
          <EntityType Name="Order">
            <Key>
              <PropertyRef Name="OrderId" />
            </Key>
            <Property Name="OrderId" Type="Edm.Int32" Nullable="false" />
            <Property Name="CustomerId" Type="Edm.Int32" />
            <Property Name="Concurrency" Type="NS1.ConcurrencyInfo" Nullable="false" />
            <NavigationProperty Name="Login" Type="NS1.Login" Partner="Orders" />
            <NavigationProperty Name="Customer" Type="NS1.Customer" Partner="Orders">
              <ReferentialConstraint Property="CustomerId" ReferencedProperty="CustomerId" />
            </NavigationProperty>
            <NavigationProperty Name="Notes" Type="Collection(NS1.OrderNote)" Partner="Order">
              <OnDelete Action="Cascade" />
            </NavigationProperty>
            <NavigationProperty Name="OrderLines" Type="Collection(NS1.OrderLine)" Partner="Order" />
          </EntityType>
          <EntityType Name="CustomerInfo">
            <Key>
              <PropertyRef Name="CustomerInfoId" />
            </Key>
            <Property Name="CustomerInfoId" Type="Edm.Int32" Nullable="false" />
            <Property Name="Information" Type="Edm.String" />
          </EntityType>
          <EntityType Name="Customer">
            <Key>
              <PropertyRef Name="CustomerId" />
            </Key>
            <Property Name="CustomerId" Type="Edm.Int32" Nullable="false" />
            <Property Name="Name" Type="Edm.String" Nullable="false" MaxLength="100" Unicode="false" />
            <Property Name="ContactInfo" Type="NS1.ContactDetails" Nullable="false" />
            <NavigationProperty Name="Orders" Type="Collection(NS1.Order)" Partner="Customer" />
            <NavigationProperty Name="Logins" Type="Collection(NS1.Login)" Partner="Customer" />
            <NavigationProperty Name="Husband" Type="NS1.Customer" Partner="Wife" />
            <NavigationProperty Name="Wife" Type="NS1.Customer" Partner="Husband" />
            <NavigationProperty Name="CustomerInfo" Type="NS1.CustomerInfo" />
          </EntityType>
          <EntityType Name="Product">
            <Key>
              <PropertyRef Name="ProductId" />
            </Key>
            <Property Name="ProductId" Type="Edm.Int32" Nullable="false" />
            <Property Name="Description" Type="Edm.String" MaxLength="1000" Unicode="false" />
            <Property Name="Dimensions" Type="NS1.Dimensions" Nullable="false" />
            <Property Name="BaseConcurrency" Type="Edm.String" DefaultValue="" Nullable="false" />
            <Property Name="ComplexConcurrency" Type="NS1.ConcurrencyInfo" Nullable="false" />
            <Property Name="NestedComplexConcurrency" Type="NS1.AuditInfo" Nullable="false" />
            <NavigationProperty Name="Barcodes" Type="Collection(NS1.Barcode)" Partner="Product" />
            <NavigationProperty Name="Suppliers" Type="Collection(NS1.Supplier)" Partner="Products" />
            <NavigationProperty Name="Replaces" Type="Collection(NS1.DiscontinuedProduct)" Partner="ReplacedBy" />
            <NavigationProperty Name="Detail" Type="NS1.ProductDetail" Partner="Product" />
            <NavigationProperty Name="Reviews" Type="Collection(NS1.ProductReview)" Partner="Product" />
            <NavigationProperty Name="Photos" Type="Collection(NS1.ProductPhoto)" Partner="Product" />
          </EntityType>
          <EntityType Name="Barcode">
            <Key>
              <PropertyRef Name="Code" />
            </Key>
            <Property Name="Code" Type="Edm.Binary" Nullable="false" MaxLength="50" />
            <Property Name="ProductId" Type="Edm.Int32" Nullable="false" />
            <Property Name="Text" Type="Edm.String" />
            <NavigationProperty Name="Product" Type="NS1.Product" Nullable="false" Partner="Barcodes">
              <ReferentialConstraint Property="ProductId" ReferencedProperty="ProductId" />
            </NavigationProperty>
            <NavigationProperty Name="BadScans" Type="Collection(NS1.IncorrectScan)" Partner="ExpectedBarcode" />
            <NavigationProperty Name="Detail" Type="NS1.BarcodeDetail" Nullable="false">
              <ReferentialConstraint Property="Code" ReferencedProperty="Code" />
            </NavigationProperty>
          </EntityType>
          <EntityType Name="IncorrectScan">
            <Key>
              <PropertyRef Name="IncorrectScanId" />
            </Key>
            <Property Name="IncorrectScanId" Type="Edm.Int32" Nullable="false" />
            <Property Name="ExpectedCode" Type="Edm.Binary" Nullable="false" MaxLength="50" />
            <Property Name="ActualCode" Type="Edm.Binary" MaxLength="50" />
            <Property Name="ScanDate" Type="Edm.DateTimeOffset" />
            <Property Name="Details" Type="Edm.String" />
            <NavigationProperty Name="ExpectedBarcode" Type="NS1.Barcode" Nullable="false" Partner="BadScans">
              <ReferentialConstraint Property="ExpectedCode" ReferencedProperty="Code" />
            </NavigationProperty>
            <NavigationProperty Name="ActualBarcode" Type="NS1.Barcode">
              <ReferentialConstraint Property="ActualCode" ReferencedProperty="Code" />
            </NavigationProperty>
          </EntityType>
          <EntityType Name="BarcodeDetail">
            <Key>
              <PropertyRef Name="Code" />
            </Key>
            <Property Name="Code" Type="Edm.Binary" Nullable="false" MaxLength="50" />
            <Property Name="RegisteredTo" Type="Edm.String" />
          </EntityType>
          <EntityType Name="Resolution">
            <Key>
              <PropertyRef Name="ResolutionId" />
            </Key>
            <Property Name="ResolutionId" Type="Edm.Int32" Nullable="false" />
            <Property Name="Details" Type="Edm.String" />
            <NavigationProperty Name="Complaint" Type="NS1.Complaint" Nullable="false" Partner="Resolution" />
          </EntityType>
          <EntityType Name="Complaint">
            <Key>
              <PropertyRef Name="ComplaintId" />
            </Key>
            <Property Name="ComplaintId" Type="Edm.Int32" Nullable="false" />
            <Property Name="CustomerId" Type="Edm.Int32" />
            <Property Name="Logged" Type="Edm.DateTimeOffset" />
            <Property Name="Details" Type="Edm.String" />
            <NavigationProperty Name="Customer" Type="NS1.Customer">
              <ReferentialConstraint Property="CustomerId" ReferencedProperty="CustomerId" />
            </NavigationProperty>
            <NavigationProperty Name="Resolution" Type="NS1.Resolution" Partner="Complaint" />
          </EntityType>
          <EntityType Name="SmartCard">
            <Key>
              <PropertyRef Name="Username" />
            </Key>
            <Property Name="Username" Type="Edm.String" Nullable="false" MaxLength="50" Unicode="false" />
            <Property Name="CardSerial" Type="Edm.String" />
            <Property Name="Issued" Type="Edm.DateTimeOffset" />
            <NavigationProperty Name="Login" Type="NS1.Login" Nullable="false">
              <ReferentialConstraint Property="Username" ReferencedProperty="Username" />
            </NavigationProperty>
            <NavigationProperty Name="LastLogin" Type="NS1.LastLogin" />
          </EntityType>
          <EntityType Name="RSAToken">
            <Key>
              <PropertyRef Name="Serial" />
            </Key>
            <Property Name="Serial" Type="Edm.String" Nullable="false" MaxLength="20" Unicode="false" />
            <Property Name="Issued" Type="Edm.DateTimeOffset" />
            <NavigationProperty Name="Login" Type="NS1.Login" Nullable="false" />
          </EntityType>
          <EntityType Name="PasswordReset">
            <Key>
              <PropertyRef Name="ResetNo" />
            </Key>
            <Property Name="ResetNo" Type="Edm.Int32" Nullable="false" />
            <Property Name="Username" Type="Edm.String" Nullable="false" MaxLength="50" Unicode="false" />
            <Property Name="TempPassword" Type="Edm.String" />
            <Property Name="EmailedTo" Type="Edm.String" />
            <NavigationProperty Name="Login" Type="NS1.Login" Nullable="false">
              <ReferentialConstraint Property="Username" ReferencedProperty="Username" />
            </NavigationProperty>
          </EntityType>
          <EntityType Name="PageView">
            <Key>
              <PropertyRef Name="PageViewId" />
            </Key>
            <Property Name="PageViewId" Type="Edm.Int32" Nullable="false" />
            <Property Name="Username" Type="Edm.String" Nullable="false" MaxLength="50" Unicode="false" />
            <Property Name="Viewed" Type="Edm.DateTimeOffset" />
            <Property Name="PageUrl" Type="Edm.String" Nullable="false" MaxLength="500" Unicode="false" />
            <NavigationProperty Name="Login" Type="NS1.Login" Nullable="false">
              <ReferentialConstraint Property="Username" ReferencedProperty="Username" />
            </NavigationProperty>
          </EntityType>
          <EntityType Name="ProductPageView" BaseType="NS1.PageView">
            <Property Name="ProductId" Type="Edm.Int32" Nullable="false" />
            <NavigationProperty Name="Product" Type="NS1.Product" Nullable="false">
              <ReferentialConstraint Property="ProductId" ReferencedProperty="ProductId" />
            </NavigationProperty>
          </EntityType>
          <EntityType Name="Supplier">
            <Key>
              <PropertyRef Name="SupplierId" />
            </Key>
            <Property Name="SupplierId" Type="Edm.Int32" Nullable="false" />
            <Property Name="Name" Type="Edm.String" />
            <NavigationProperty Name="Products" Type="Collection(NS1.Product)" Partner="Suppliers" />
            <NavigationProperty Name="Logo" Type="NS1.SupplierLogo" Nullable="false">
              <ReferentialConstraint Property="SupplierId" ReferencedProperty="SupplierId" />
            </NavigationProperty>
            <NavigationProperty Name="BackOrderLines" Type="Collection(NS1.BackOrderLine)" Partner="Supplier" />
          </EntityType>
          <EntityType Name="SupplierLogo">
            <Key>
              <PropertyRef Name="SupplierId" />
            </Key>
            <Property Name="SupplierId" Type="Edm.Int32" Nullable="false" />
            <Property Name="Logo" Type="Edm.Binary" Nullable="false" MaxLength="500" />
          </EntityType>
          <EntityType Name="SupplierInfo">
            <Key>
              <PropertyRef Name="SupplierInfoId" />
            </Key>
            <Property Name="SupplierInfoId" Type="Edm.Int32" Nullable="false" />
            <Property Name="Information" Type="Edm.String" />
            <NavigationProperty Name="Supplier" Type="NS1.Supplier" Nullable="false">
              <OnDelete Action="Cascade" />
            </NavigationProperty>
          </EntityType>
          <EntityType Name="OrderNote">
            <Key>
              <PropertyRef Name="NoteId" />
            </Key>
            <Property Name="NoteId" Type="Edm.Int32" Nullable="false" />
            <Property Name="Note" Type="Edm.String" />
            <NavigationProperty Name="Order" Type="NS1.Order" Nullable="false" Partner="Notes" />
          </EntityType>
          <EntityType Name="OrderQualityCheck">
            <Key>
              <PropertyRef Name="OrderId" />
            </Key>
            <Property Name="OrderId" Type="Edm.Int32" Nullable="false" />
            <Property Name="CheckedBy" Type="Edm.String" />
            <Property Name="CheckedDateTime" Type="Edm.DateTimeOffset" />
            <NavigationProperty Name="Order" Type="NS1.Order" Nullable="false">
              <ReferentialConstraint Property="OrderId" ReferencedProperty="OrderId" />
            </NavigationProperty>
          </EntityType>
          <EntityType Name="OrderLine">
            <Key>
              <PropertyRef Name="OrderId" />
              <PropertyRef Name="ProductId" />
            </Key>
            <Property Name="OrderId" Type="Edm.Int32" Nullable="false" />
            <Property Name="ProductId" Type="Edm.Int32" Nullable="false" />
            <Property Name="Quantity" Type="Edm.Int32" />
            <Property Name="ConcurrencyToken" Type="Edm.String" DefaultValue="" Nullable="false" />
            <NavigationProperty Name="Order" Type="NS1.Order" Nullable="false" Partner="OrderLines">
              <ReferentialConstraint Property="OrderId" ReferencedProperty="OrderId" />
            </NavigationProperty>
            <NavigationProperty Name="Product" Type="NS1.Product" Nullable="false">
              <ReferentialConstraint Property="ProductId" ReferencedProperty="ProductId" />
            </NavigationProperty>
          </EntityType>
          <EntityType Name="BackOrderLine" BaseType="NS1.OrderLine">
            <Property Name="ETA" Type="Edm.DateTimeOffset" />
            <NavigationProperty Name="Supplier" Type="NS1.Supplier" Partner="BackOrderLines" />
          </EntityType>
          <EntityType Name="BackOrderLine2" BaseType="NS1.BackOrderLine" />
          <EntityType Name="DiscontinuedProduct" BaseType="NS1.Product">
            <Property Name="Discontinued" Type="Edm.DateTimeOffset" />
            <Property Name="ReplacementProductId" Type="Edm.Int32" />
            <NavigationProperty Name="ReplacedBy" Type="NS1.Product" Partner="Replaces">
              <ReferentialConstraint Property="ReplacementProductId" ReferencedProperty="ProductId" />
            </NavigationProperty>
          </EntityType>
          <EntityType Name="ProductDetail">
            <Key>
              <PropertyRef Name="ProductId" />
            </Key>
            <Property Name="ProductId" Type="Edm.Int32" Nullable="false" />
            <Property Name="Details" Type="Edm.String" />
            <NavigationProperty Name="Product" Type="NS1.Product" Nullable="false" Partner="Detail">
              <ReferentialConstraint Property="ProductId" ReferencedProperty="ProductId" />
            </NavigationProperty>
          </EntityType>
          <EntityType Name="ProductReview">
            <Key>
              <PropertyRef Name="ProductId" />
              <PropertyRef Name="ReviewId" />
            </Key>
            <Property Name="ProductId" Type="Edm.Int32" Nullable="false" />
            <Property Name="ReviewId" Type="Edm.Int32" Nullable="false" />
            <Property Name="Review" Type="Edm.String" />
            <NavigationProperty Name="Product" Type="NS1.Product" Nullable="false" Partner="Reviews">
              <ReferentialConstraint Property="ProductId" ReferencedProperty="ProductId" />
            </NavigationProperty>
            <NavigationProperty Name="Features" Type="Collection(NS1.ProductWebFeature)" Partner="Review" />
          </EntityType>
          <EntityType Name="ProductPhoto">
            <Key>
              <PropertyRef Name="ProductId" />
              <PropertyRef Name="PhotoId" />
            </Key>
            <Property Name="ProductId" Type="Edm.Int32" Nullable="false" />
            <Property Name="PhotoId" Type="Edm.Int32" Nullable="false" />
            <Property Name="Photo" Type="Edm.Binary" />
            <NavigationProperty Name="Product" Type="NS1.Product" Nullable="false" Partner="Photos">
              <ReferentialConstraint Property="ProductId" ReferencedProperty="ProductId" />
            </NavigationProperty>
            <NavigationProperty Name="Features" Type="Collection(NS1.ProductWebFeature)" Partner="Photo" />
          </EntityType>
          <EntityType Name="ProductWebFeature">
            <Key>
              <PropertyRef Name="FeatureId" />
            </Key>
            <Property Name="FeatureId" Type="Edm.Int32" Nullable="false" />
            <Property Name="ProductId" Type="Edm.Int32" />
            <Property Name="PhotoId" Type="Edm.Int32" />
            <Property Name="ReviewId" Type="Edm.Int32" />
            <Property Name="Heading" Type="Edm.String" />
            <NavigationProperty Name="Review" Type="NS1.ProductReview" Partner="Features">
              <ReferentialConstraint Property="ReviewId" ReferencedProperty="ProductId" />
              <ReferentialConstraint Property="ProductId" ReferencedProperty="ReviewId" />
            </NavigationProperty>
            <NavigationProperty Name="Photo" Type="NS1.ProductPhoto" Partner="Features">
              <ReferentialConstraint Property="PhotoId" ReferencedProperty="ProductId" />
              <ReferentialConstraint Property="ProductId" ReferencedProperty="PhotoId" />
            </NavigationProperty>
          </EntityType>
          <EntityType Name="Computer">
            <Key>
              <PropertyRef Name="ComputerId" />
            </Key>
            <Property Name="ComputerId" Type="Edm.Int32" Nullable="false" />
            <Property Name="Name" Type="Edm.String" />
            <NavigationProperty Name="ComputerDetail" Type="NS1.ComputerDetail" Nullable="false" Partner="Computer" />
          </EntityType>
          <EntityType Name="ComputerDetail">
            <Key>
              <PropertyRef Name="ComputerDetailId" />
            </Key>
            <Property Name="ComputerDetailId" Type="Edm.Int32" Nullable="false" />
            <Property Name="Model" Type="Edm.String" />
            <Property Name="Serial" Type="Edm.String" />
            <Property Name="Specifications" Type="Edm.String" />
            <Property Name="PurchaseDate" Type="Edm.DateTimeOffset" />
            <Property Name="Dimensions" Type="NS1.Dimensions" Nullable="false" />
            <NavigationProperty Name="Computer" Type="NS1.Computer" Nullable="false" Partner="ComputerDetail" />
          </EntityType>
          <EntityType Name="Driver">
            <Key>
              <PropertyRef Name="Name" />
            </Key>
            <Property Name="Name" Type="Edm.String" Nullable="false" MaxLength="100" Unicode="false" />
            <Property Name="BirthDate" Type="Edm.DateTimeOffset" />
            <NavigationProperty Name="License" Type="NS1.License" Nullable="false" Partner="Driver" />
          </EntityType>
          <EntityType Name="License">
            <Key>
              <PropertyRef Name="Name" />
            </Key>
            <Property Name="Name" Type="Edm.String" Nullable="false" MaxLength="100" Unicode="false" />
            <Property Name="LicenseNumber" Type="Edm.String" />
            <Property Name="LicenseClass" Type="Edm.String" />
            <Property Name="Restrictions" Type="Edm.String" />
            <Property Name="ExpirationDate" Type="Edm.DateTimeOffset" />
            <NavigationProperty Name="Driver" Type="NS1.Driver" Nullable="false" Partner="License">
              <ReferentialConstraint Property="Name" ReferencedProperty="Name" />
            </NavigationProperty>
          </EntityType>
          <EntityContainer Name="DefaultContainer_sub">
            <EntitySet Name="SuspiciousActivity" EntityType="NS1.SuspiciousActivity" />
            <EntitySet Name="Message" EntityType="NS1.Message">
              <NavigationPropertyBinding Path="Recipient" Target="Login" />
              <NavigationPropertyBinding Path="Sender" Target="Login" />
            </EntitySet>
            <EntitySet Name="Login" EntityType="NS1.Login">
              <NavigationPropertyBinding Path="Customer" Target="Customer" />
              <NavigationPropertyBinding Path="LastLogin" Target="LastLogin" />
              <NavigationPropertyBinding Path="Orders" Target="Order" />
              <NavigationPropertyBinding Path="ReceivedMessages" Target="Message" />
              <NavigationPropertyBinding Path="SentMessages" Target="Message" />
              <NavigationPropertyBinding Path="SuspiciousActivity" Target="SuspiciousActivity" />
            </EntitySet>
            <EntitySet Name="LastLogin" EntityType="NS1.LastLogin">
              <NavigationPropertyBinding Path="Login" Target="Login" />
            </EntitySet>
            <EntitySet Name="Order" EntityType="NS1.Order">
              <NavigationPropertyBinding Path="Customer" Target="Customer" />
              <NavigationPropertyBinding Path="Login" Target="Login" />
              <NavigationPropertyBinding Path="Notes" Target="OrderNote" />
              <NavigationPropertyBinding Path="OrderLines" Target="OrderLine" />
            </EntitySet>
            <EntitySet Name="CustomerInfo" EntityType="NS1.CustomerInfo" />
            <EntitySet Name="Customer" EntityType="NS1.Customer">
              <NavigationPropertyBinding Path="CustomerInfo" Target="CustomerInfo" />
              <NavigationPropertyBinding Path="Husband" Target="Customer" />
              <NavigationPropertyBinding Path="Logins" Target="Login" />
              <NavigationPropertyBinding Path="Orders" Target="Order" />
              <NavigationPropertyBinding Path="Wife" Target="Customer" />
            </EntitySet>
            <EntitySet Name="Product" EntityType="NS1.Product">
              <NavigationPropertyBinding Path="Barcodes" Target="Barcode" />
              <NavigationPropertyBinding Path="Detail" Target="ProductDetail" />
              <NavigationPropertyBinding Path="NS1.DiscontinuedProduct/ReplacedBy" Target="Product" />
              <NavigationPropertyBinding Path="Photos" Target="ProductPhoto" />
              <NavigationPropertyBinding Path="Replaces" Target="Product" />
              <NavigationPropertyBinding Path="Reviews" Target="ProductReview" />
              <NavigationPropertyBinding Path="Suppliers" Target="Supplier" />
            </EntitySet>
            <EntitySet Name="Barcode" EntityType="NS1.Barcode">
              <NavigationPropertyBinding Path="BadScans" Target="IncorrectScan" />
              <NavigationPropertyBinding Path="Detail" Target="BarcodeDetail" />
              <NavigationPropertyBinding Path="Product" Target="Product" />
            </EntitySet>
            <EntitySet Name="IncorrectScan" EntityType="NS1.IncorrectScan">
              <NavigationPropertyBinding Path="ActualBarcode" Target="Barcode" />
              <NavigationPropertyBinding Path="ExpectedBarcode" Target="Barcode" />
            </EntitySet>
            <EntitySet Name="BarcodeDetail" EntityType="NS1.BarcodeDetail" />
            <EntitySet Name="Resolution" EntityType="NS1.Resolution">
              <NavigationPropertyBinding Path="Complaint" Target="Complaint" />
            </EntitySet>
            <EntitySet Name="Complaint" EntityType="NS1.Complaint">
              <NavigationPropertyBinding Path="Customer" Target="Customer" />
              <NavigationPropertyBinding Path="Resolution" Target="Resolution" />
            </EntitySet>
            <EntitySet Name="SmartCard" EntityType="NS1.SmartCard">
              <NavigationPropertyBinding Path="LastLogin" Target="LastLogin" />
              <NavigationPropertyBinding Path="Login" Target="Login" />
            </EntitySet>
            <EntitySet Name="RSAToken" EntityType="NS1.RSAToken">
              <NavigationPropertyBinding Path="Login" Target="Login" />
            </EntitySet>
            <EntitySet Name="PasswordReset" EntityType="NS1.PasswordReset">
              <NavigationPropertyBinding Path="Login" Target="Login" />
            </EntitySet>
            <EntitySet Name="PageView" EntityType="NS1.PageView">
              <NavigationPropertyBinding Path="Login" Target="Login" />
              <NavigationPropertyBinding Path="NS1.ProductPageView/Product" Target="Product" />
            </EntitySet>
            <EntitySet Name="Supplier" EntityType="NS1.Supplier">
              <NavigationPropertyBinding Path="BackOrderLines" Target="OrderLine" />
              <NavigationPropertyBinding Path="Logo" Target="SupplierLogo" />
              <NavigationPropertyBinding Path="Products" Target="Product" />
            </EntitySet>
            <EntitySet Name="SupplierLogo" EntityType="NS1.SupplierLogo" />
            <EntitySet Name="SupplierInfo" EntityType="NS1.SupplierInfo">
              <NavigationPropertyBinding Path="Supplier" Target="Supplier" />
            </EntitySet>
            <EntitySet Name="OrderNote" EntityType="NS1.OrderNote">
              <NavigationPropertyBinding Path="Order" Target="Order" />
            </EntitySet>
            <EntitySet Name="OrderQualityCheck" EntityType="NS1.OrderQualityCheck">
              <NavigationPropertyBinding Path="Order" Target="Order" />
            </EntitySet>
            <EntitySet Name="OrderLine" EntityType="NS1.OrderLine">
              <NavigationPropertyBinding Path="NS1.BackOrderLine/Supplier" Target="Supplier" />
              <NavigationPropertyBinding Path="Order" Target="Order" />
              <NavigationPropertyBinding Path="Product" Target="Product" />
            </EntitySet>
            <EntitySet Name="ProductDetail" EntityType="NS1.ProductDetail">
              <NavigationPropertyBinding Path="Product" Target="Product" />
            </EntitySet>
            <EntitySet Name="ProductReview" EntityType="NS1.ProductReview">
              <NavigationPropertyBinding Path="Features" Target="ProductWebFeature" />
              <NavigationPropertyBinding Path="Product" Target="Product" />
            </EntitySet>
            <EntitySet Name="ProductPhoto" EntityType="NS1.ProductPhoto">
              <NavigationPropertyBinding Path="Features" Target="ProductWebFeature" />
              <NavigationPropertyBinding Path="Product" Target="Product" />
            </EntitySet>
            <EntitySet Name="ProductWebFeature" EntityType="NS1.ProductWebFeature">
              <NavigationPropertyBinding Path="Photo" Target="ProductPhoto" />
              <NavigationPropertyBinding Path="Review" Target="ProductReview" />
            </EntitySet>
            <EntitySet Name="Computer" EntityType="NS1.Computer">
              <NavigationPropertyBinding Path="ComputerDetail" Target="ComputerDetail" />
            </EntitySet>
            <EntitySet Name="ComputerDetail" EntityType="NS1.ComputerDetail">
              <NavigationPropertyBinding Path="Computer" Target="Computer" />
            </EntitySet>
            <EntitySet Name="Driver" EntityType="NS1.Driver">
              <NavigationPropertyBinding Path="License" Target="License" />
            </EntitySet>
            <EntitySet Name="License" EntityType="NS1.License">
              <NavigationPropertyBinding Path="Driver" Target="Driver" />
            </EntitySet>
          </EntityContainer>
        </Schema>
        """;

    #endregion
}
