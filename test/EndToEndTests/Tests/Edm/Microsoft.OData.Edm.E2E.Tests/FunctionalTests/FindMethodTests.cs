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
using Microsoft.OData.Edm.Vocabularies;

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

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        IEnumerable<EdmError> serializationErrors;
        var testCsdls = this.GetSerializerResult(testModelConstructible, out serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
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

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        IEnumerable<EdmError> serializationErrors;
        var testCsdls = this.GetSerializerResult(testModelConstructible, out serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
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

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        IEnumerable<EdmError> serializationErrors;
        var testCsdls = this.GetSerializerResult(testModelConstructible, out serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
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

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
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

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestAssociationCompositeFk(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var customerType = new EdmEntityType("NS1", "Customer");
        customerType.AddKeys(customerType.AddStructuralProperty("Id1", EdmPrimitiveTypeKind.Int32));
        customerType.AddKeys(customerType.AddStructuralProperty("Id2", EdmPrimitiveTypeKind.Int32));
        model.AddElement(customerType);

        var orderType = new EdmEntityType("NS1", "Order");
        orderType.AddKeys(orderType.AddStructuralProperty("Id1", EdmPrimitiveTypeKind.Int32));
        var customerId1Property = orderType.AddStructuralProperty("CustomerId1", EdmPrimitiveTypeKind.Int32);
        var customerId2Property = orderType.AddStructuralProperty("CustomerId2", EdmPrimitiveTypeKind.Int32);
        model.AddElement(orderType);

        var navProp = new EdmNavigationPropertyInfo { Name = "ToCustomer", Target = customerType, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { customerId1Property, customerId2Property }, PrincipalProperties = customerType.Key() };
        orderType.AddUnidirectionalNavigation(navProp);

        var csdlElements = this.GetSerializerResult(model).Select(XElement.Parse);
        csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestAssociationFk(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var customerType = new EdmEntityType("NS1", "Customer");
        customerType.AddKeys(customerType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        model.AddElement(customerType);

        var orderType = new EdmEntityType("NS1", "Order");
        orderType.AddKeys(orderType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        var orderTypeCustomerId = orderType.AddStructuralProperty("CustomerId", EdmPrimitiveTypeKind.Int32);
        model.AddElement(orderType);

        customerType.AddUnidirectionalNavigation(
            new EdmNavigationPropertyInfo
            {
                Name = "ToOrders",
                Target = orderType,
                TargetMultiplicity = EdmMultiplicity.Many,
            });

        orderType.AddUnidirectionalNavigation(
            new EdmNavigationPropertyInfo
            {
                Name = "Customer",
                Target = customerType,
                TargetMultiplicity = EdmMultiplicity.One,
                DependentProperties = new[] { orderTypeCustomerId },
                PrincipalProperties = orderType.Key()
            });

        var csdlElements = this.GetSerializerResult(model).Select(XElement.Parse);
        csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestAssociationFkWithNavigation(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var customerType = new EdmEntityType("NS1", "Customer");
        customerType.AddKeys(customerType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        model.AddElement(customerType);

        var orderType = new EdmEntityType("NS1", "Order");
        orderType.AddKeys(orderType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        model.AddElement(orderType);

        var customerIdProperty = orderType.AddStructuralProperty("CustomerId", EdmPrimitiveTypeKind.Int32);
        var navProp1 = new EdmNavigationPropertyInfo { Name = "ToOrders", Target = orderType, TargetMultiplicity = EdmMultiplicity.Many, };
        var navProp2 = new EdmNavigationPropertyInfo { Name = "ToCustomer", Target = customerType, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { customerIdProperty }, PrincipalProperties = customerType.Key() };
        customerType.AddBidirectionalNavigation(navProp1, navProp2);

        var csdlElements = this.GetSerializerResult(model).Select(XElement.Parse);
        csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestAssociationIndependent(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var customerType = new EdmEntityType("NS1", "Customer");
        customerType.AddKeys(customerType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        model.AddElement(customerType);

        var orderType = new EdmEntityType("NS1", "Order");
        orderType.AddKeys(orderType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        model.AddElement(orderType);

        var navProp1 = new EdmNavigationPropertyInfo { Name = "ToOrders", Target = orderType, TargetMultiplicity = EdmMultiplicity.Many };
        var navProp2 = new EdmNavigationPropertyInfo { Name = "ToCustomer", Target = customerType, TargetMultiplicity = EdmMultiplicity.One };
        customerType.AddBidirectionalNavigation(navProp1, navProp2);

        var csdlElements = this.GetSerializerResult(model).Select(XElement.Parse);
        csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestAssociationOnDeleteModel(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var customerType = new EdmEntityType("NS1", "Customer");
        customerType.AddKeys(customerType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        model.AddElement(customerType);

        var orderType = new EdmEntityType("NS1", "Order");
        orderType.AddKeys(orderType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        model.AddElement(orderType);

        var customerIdProperty = orderType.AddStructuralProperty("CustomerId", EdmPrimitiveTypeKind.Int32);
        var navProp1 = new EdmNavigationPropertyInfo { Name = "ToOrders", Target = orderType, TargetMultiplicity = EdmMultiplicity.Many, OnDelete = EdmOnDeleteAction.Cascade };
        var navProp2 = new EdmNavigationPropertyInfo { Name = "ToCustomer", Target = customerType, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { customerIdProperty }, PrincipalProperties = customerType.Key(), OnDelete = EdmOnDeleteAction.None };
        customerType.AddBidirectionalNavigation(navProp1, navProp2);

        var csdlElements = this.GetSerializerResult(model).Select(XElement.Parse);
        csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestCollectionTypes(EdmVersion edmVersion)
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
    <Function Name='MyFunction'>
        <ReturnType Type='Collection(MyNamespace.MyEntityType)'/>
        <Parameter Name='P1' Type='Collection(MyNamespace.MyEntityType)'/>
        <Parameter Name='P3' Type='Collection(MyNamespace.MyEntityType)'/>
        <Parameter Name='P4' Type='Collection(Binary)'/>
        <Parameter Name='P5' Type='Collection(MyNamespace.MyEntityType)'/>
    </Function>
    <Function Name='MyFunction'><ReturnType Type='Collection(MyNamespace.MyEntityType)'/>
        <Parameter Name='P1' Type='Collection(Binary)'/>
    </Function>
</Schema>
");
        var testCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(new XElement[] { csdlElement }, edmVersion);
        var isParsed = SchemaReader.TryParse(testCsdls.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindSchemaElementMethod(testCsdls.Single(), testModelImmutable, edmVersion);
        this.VerifyFindEntityContainer(testCsdls.Single(), testModelImmutable, edmVersion);
        this.VerifyFindEntityContainerElement(testCsdls.Single(), testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        testCsdls = this.GetSerializerResult(testModelConstructible).Select(n => XElement.Parse(n));

        this.VerifyFindSchemaElementMethod(testCsdls.Single(), testModelConstructible, edmVersion);
        this.VerifyFindEntityContainer(testCsdls.Single(), testModelConstructible, edmVersion);
        this.VerifyFindEntityContainerElement(testCsdls.Single(), testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestCollectionTypesWithSimpleType(EdmVersion edmVersion)
    {
        var csdlElement = XElement.Parse(@"
<Schema Namespace='MyNamespace' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
    <EntityType Name='MyEntityType'>
        <Key>
            <PropertyRef Name='Property1'/>
        </Key>
        <Property Name='Property1' Type='String' Nullable='false'/>
    </EntityType>
    <Function Name='MyFunction'><ReturnType Type='Edm.Int32' />
        <Parameter Name='P4' Type='Collection(Binary)'/>
    </Function>
    <Function Name='MyFunction'><ReturnType Type='Collection(MyNamespace.MyEntityType)'/>
        <Parameter Name='P2' Type='Collection(Edm.Int32)' /> 
    </Function>
</Schema>
");

        var csdls = new XElement[] { csdlElement };
        var csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdls.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestComplexTypeAttributes(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var baseComplexType = new EdmComplexType("MyNamespace", "MyBaseComplexType");
        model.AddElement(baseComplexType);

        var complexType = new EdmComplexType("MyNamespace", "MyComplexType", baseComplexType, true);
        model.AddElement(complexType);

        var csdlElements = this.GetSerializerResult(model).Select(XElement.Parse);
        csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestEntityContainerAttributes(EdmVersion edmVersion)
    {
        var csdlElement1 = XElement.Parse(@"
<Schema Namespace='NS1' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <EntityContainer Name='MyContainer1'>
    <EntitySet Name='CustomerSet1' EntityType='NS1.Customer'>
      <NavigationPropertyBinding Path=""ToOrders"" Target=""OrderSet1""/>
    </EntitySet>
    <EntitySet Name='OrderSet1' EntityType='NS1.Order'>
      <NavigationPropertyBinding Path=""ToCustomer"" Target=""CustomerSet1""/>
    </EntitySet>
    <EntitySet Name='OrderSet3' EntityType='NS1.Order'/>
    <EntitySet Name='CustomerSet2' EntityType='NS1.Customer'>
      <NavigationPropertyBinding Path=""ToOrders"" Target=""OrderSet2""/>
    </EntitySet>
    <EntitySet Name='OrderSet2' EntityType='NS1.Order'>
      <NavigationPropertyBinding Path=""ToCustomer"" Target=""CustomerSet2""/>
    </EntitySet>
  </EntityContainer>
  <EntityType Name='Customer'>
    <Key>
      <PropertyRef Name='Id' />
    </Key>
    <Property Name='Id' Type='Int32' Nullable='false' />
    <NavigationProperty Name='ToOrders' Type=""Collection(NS1.Order)"" Partner=""ToCustomer"" />
  </EntityType>
  <EntityType Name='Order'>
    <Key>
      <PropertyRef Name='Id' />
    </Key>
    <Property Name='Id' Type='Int32' Nullable='false' />
    <Property Name='CustomerId' Type='Int32' Nullable='false' />
    <NavigationProperty Name='ToCustomer' Type=""NS1.Customer"" Nullable=""false"" Partner=""ToOrders"">
      <ReferentialConstraint Property=""CustomerId"" ReferencedProperty=""Id"" />
    </NavigationProperty>
  </EntityType>
</Schema>
");
        var csdlElement2 = XElement.Parse(@"
<Schema Namespace='NS2' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <EntityType Name='Guest'>
    <Key>
      <PropertyRef Name='Id' />
    </Key>
    <Property Name='Id' Type='Int32' Nullable='false' />
  </EntityType>
  <Function Name='GetGuestsExcluding'><ReturnType Type='Collection(NS2.Guest)'/> 
    <Parameter Name='GuestId' Type='Int32' />
  </Function>
</Schema>
");

        var csdls = new XElement[] { csdlElement1, csdlElement2 };
        var csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdls.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);


    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestEntityContainerWithEntitySets(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var rootType = new EdmEntityType("NS1", "Root");
        rootType.AddKeys(rootType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        model.AddElement(rootType);

        var derivedType1 = new EdmEntityType("NS1", "DerivedLevel1", rootType);
        derivedType1.AddStructuralProperty("PropertyLevel1", EdmPrimitiveTypeKind.Int32);
        model.AddElement(derivedType1);

        var derivedType2 = new EdmEntityType("NS1", "DerivedLevel2", derivedType1);
        derivedType2.AddStructuralProperty("PropertyLevel2", EdmPrimitiveTypeKind.Int32);
        model.AddElement(derivedType2);

        var container = new EdmEntityContainer("NS1", "MyContainer");
        container.AddEntitySet("RootSet", rootType);
        container.AddEntitySet("Level1Set", derivedType1);
        container.AddEntitySet("Level2Set", derivedType2);
        model.AddElement(container);

        var csdlElements = this.GetSerializerResult(model).Select(XElement.Parse);
        csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestEntityContainerWithFunctionImports(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var customerType = new EdmEntityType("NS1", "Customer");
        customerType.AddKeys(customerType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        model.AddElement(customerType);

        var container = new EdmEntityContainer("NS1", "MyContainer");
        var customerSet = container.AddEntitySet("Customer", customerType);

        var function = new EdmFunction("NS1", "GetCustomersExcluding", EdmCoreModel.GetCollection(new EdmEntityTypeReference(customerType, false)), false, null, false);
        function.AddParameter("CustomerId", EdmCoreModel.Instance.GetInt32(true));

        model.AddElement(function);
        container.AddFunctionImport("GetCustomersExcluding", function, new EdmPathExpression(customerSet.Name));

        model.AddElement(container);

        var csdlElements = this.GetSerializerResult(model).Select(XElement.Parse);
        csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestEntityInheritanceTree(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var rootType = new EdmEntityType("NS1", "Root");
        rootType.AddKeys(rootType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        model.AddElement(rootType);

        var derivedType1_1 = new EdmEntityType("NS1", "DerivedLevel1_1", rootType);
        derivedType1_1.AddStructuralProperty("PropertyLevel1", EdmPrimitiveTypeKind.Int32);
        model.AddElement(derivedType1_1);

        var derivedType1_2 = new EdmEntityType("NS1", "DerivedLevel1_2", rootType);
        rootType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "RootDerivedLevel1_2", Target = derivedType1_2, TargetMultiplicity = EdmMultiplicity.Many });
        model.AddElement(derivedType1_2);

        var derivedType2_1 = new EdmEntityType("NS1", "DerivedLevel2_1", derivedType1_1);
        derivedType2_1.AddStructuralProperty("PropertyLevel2", EdmPrimitiveTypeKind.Int32);
        model.AddElement(derivedType2_1);

        var derivedType2_2 = new EdmEntityType("NS1", "DerivedLevel2_2", derivedType1_2);
        derivedType2_2.AddStructuralProperty("PropertyLevel2", EdmPrimitiveTypeKind.Int32);
        model.AddElement(derivedType2_2);

        var derivedType2_3 = new EdmEntityType("NS1", "DerivedLevel2_3", derivedType1_2);
        derivedType2_3.AddStructuralProperty("PropertyLevel2", EdmPrimitiveTypeKind.Int32);
        model.AddElement(derivedType2_3);

        var csdlElements = this.GetSerializerResult(model).Select(XElement.Parse);
        csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestFunctionWithAll(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var function = new EdmFunction("NS1", "FunctionWithAll", EdmCoreModel.Instance.GetInt32(true));
        function.AddParameter("Param1", EdmCoreModel.Instance.GetInt32(false));
        model.AddElement(function);

        var csdlElements = this.GetSerializerResult(model).Select(XElement.Parse);
        csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestModelWithAllConcepts(EdmVersion edmVersion)
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

        var csdlElements = this.GetSerializerResult(model).Select(XElement.Parse);
        csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestMultipleNamespaces(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var personType = new EdmEntityType("NS1", "Person");
        personType.AddKeys(personType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        model.AddElement(personType);

        var complexType1 = new EdmComplexType("NS3", "ComplexLevel1");
        model.AddElement(complexType1);
        var complexType2 = new EdmComplexType("NS2", "ComplexLevel2");
        model.AddElement(complexType2);
        var complexType3 = new EdmComplexType("NS2", "ComplexLevel3");
        model.AddElement(complexType3);

        complexType3.AddStructuralProperty("IntProperty", EdmPrimitiveTypeKind.Int32);
        complexType2.AddStructuralProperty("ComplexProperty", new EdmComplexTypeReference(complexType3, false));
        complexType1.AddStructuralProperty("ComplexProperty", new EdmComplexTypeReference(complexType2, false));
        personType.AddStructuralProperty("ComplexProperty", new EdmComplexTypeReference(complexType1, false));

        var customerType = new EdmEntityType("NS3", "Customer", personType);
        model.AddElement(customerType);

        var orderType = new EdmEntityType("NS2", "Order");
        orderType.AddKeys(orderType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        model.AddElement(orderType);

        var customerIdProperty = orderType.AddStructuralProperty("CustomerId", EdmPrimitiveTypeKind.Int32);
        var navProp1 = new EdmNavigationPropertyInfo { Name = "ToOrders", Target = orderType, TargetMultiplicity = EdmMultiplicity.Many, };
        var navProp2 = new EdmNavigationPropertyInfo { Name = "ToCustomer", Target = customerType, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { customerIdProperty }, PrincipalProperties = customerType.Key() };
        customerType.AddBidirectionalNavigation(navProp1, navProp2);

        var csdlElements = this.GetSerializerResult(model).Select(XElement.Parse);
        csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestOneComplexWithAllPrimitiveProperty(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var type = new EdmEntityType("NS1", "Person");
        type.AddKeys(type.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));

        var complexType = new EdmComplexType("NS1", "ComplexThing");

        int i = 0;
        foreach (var primitiveType in ModelBuilderHelpers.AllNonSpatialPrimitiveEdmTypes())
        {
            complexType.AddStructuralProperty("prop_" + (i++), primitiveType);
        }

        model.AddElement(complexType);
        type.AddStructuralProperty("ComplexProperty", new EdmComplexTypeReference(complexType, false));
        model.AddElement(type);

        var csdlElements = this.GetSerializerResult(model).Select(XElement.Parse);
        csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestOneComplexWithNestedComplex(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var entityType = new EdmEntityType("NS1", "Person");
        entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        model.AddElement(entityType);

        var complexType1 = new EdmComplexType("NS1", "ComplexLevel1");
        model.AddElement(complexType1);
        var complexType2 = new EdmComplexType("NS1", "ComplexLevel2");
        model.AddElement(complexType2);
        var complexType3 = new EdmComplexType("NS1", "ComplexLevel3");
        model.AddElement(complexType3);

        complexType3.AddStructuralProperty("IntProperty", EdmPrimitiveTypeKind.Int32);
        complexType2.AddStructuralProperty("ComplexProperty", new EdmComplexTypeReference(complexType3, false));
        complexType1.AddStructuralProperty("ComplexProperty", new EdmComplexTypeReference(complexType2, false));
        entityType.AddStructuralProperty("ComplexProperty", new EdmComplexTypeReference(complexType1, false));

        var csdlElements = this.GetSerializerResult(model).Select(XElement.Parse);
        csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestOneComplexWithOneProperty(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var type = new EdmComplexType("NS1", "ComplexType");
        type.AddStructuralProperty("Prop1", EdmPrimitiveTypeKind.Int32);
        model.AddElement(type);

        var csdlElements = this.GetSerializerResult(model).Select(XElement.Parse);
        csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestOneEntityWithAllPrimitiveProperty(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var type = new EdmEntityType("NS1", "Person");
        type.AddKeys(type.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));

        int i = 0;
        foreach (var primitiveType in ModelBuilderHelpers.AllNonSpatialPrimitiveEdmTypes())
        {
            type.AddStructuralProperty("prop_" + (i++), primitiveType);
        }

        model.AddElement(type);

        var csdlElements = this.GetSerializerResult(model).Select(XElement.Parse);
        csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestOneEntityWithOneProperty(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var type = new EdmEntityType("NS1", "Person");
        type.AddKeys(type.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        model.AddElement(type);

        var csdlElements = this.GetSerializerResult(model).Select(XElement.Parse);
        csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestPropertyFacetsCollection(EdmVersion edmVersion)
    {
        var typeReferences = new IEdmTypeReference[]
        {
            EdmCoreModel.Instance.GetDecimal(precision:2, scale:2, isNullable:false),
            EdmCoreModel.Instance.GetString(isUnbounded:false, maxLength:100, isUnicode:false, isNullable:false),
            EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(isUnbounded:false, maxLength:100, isUnicode:false, isNullable:false)),
        };

        var model = new EdmModel();
        var entityType = new EdmEntityType("Namespace", "EntityType1");
        var complexType = new EdmComplexType("Namespace", "ComplexType1");
        entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false));

        int counter = 0;
        foreach (var edmTypeReference in typeReferences)
        {
            entityType.AddStructuralProperty("prop_" + counter, edmTypeReference);
            complexType.AddStructuralProperty("property" + counter, edmTypeReference);
            counter++;
        }

        model.AddElements(new IEdmSchemaElement[] { entityType, complexType });

        var csdlElements = this.GetSerializerResult(model).Select(XElement.Parse);
        csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestSimpleAllPrimtiveTypes(EdmVersion edmVersion)
    {
        var namespaceName = "ModelBuilder.SimpleAllPrimitiveTypes";

        var model = new EdmModel();
        var entityType = new EdmEntityType(namespaceName, "validEntityType1");
        entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false));
        model.AddElement(entityType);

        var complexType = new EdmComplexType(namespaceName, "V1alidcomplexType");
        model.AddElement(complexType);

        var stringBuilder = new StringBuilder();
        var xmlWriter = XmlWriter.Create(stringBuilder);
        model.TryWriteSchema((s) => xmlWriter, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        xmlWriter.Close();
        var csdlElements = new[] { XElement.Parse(stringBuilder.ToString()) };

        ModelBuilderHelpers.SetNullableAttributes(csdlElements, true /* isNullable */);

        var csdls = this.GetSerializerResult(model).Select(XElement.Parse);
        csdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements.ToArray(), edmVersion);

        var isParsed = SchemaReader.TryParse(csdls.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdls, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestSimpleConstrucitveApiTestModel(EdmVersion edmVersion)
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

        var csdls = new XElement[] { XElement.Parse(csdl) };
        var csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdls, edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestStringWithFacets(EdmVersion edmVersion)
    {
        var csdl = @"
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
</Schema>";

        var csdls = new XElement[] { XElement.Parse(csdl) };
        var csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdls, edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestDuplicatePropertyName(EdmVersion edmVersion)
    {
        var csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CollectionAtomic"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""ComplexTypeA"">
    <Property Name=""Id"" Type=""Int32""/>
    <Property Name=""Collection"" Type=""Collection(Int32)""/>
    <Property Name=""Collection"" Type=""Collection(Int32)""/>
  </ComplexType>
  <EntityType Name=""ComplexTypeE"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Collection"" Type=""Collection(Int32)""/>
    <Property Name=""Collection"" Type=""Collection(Int32)""/>
  </EntityType>
</Schema>";

        var csdls = new XElement[] { XElement.Parse(csdl) };
        var csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdls, edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var complexType = edmModel.FindType("CollectionAtomic.ComplexTypeA") as IEdmComplexType;
        var property = complexType.FindProperty("Collection");
        Assert.Equal("Collection", property.Name);

        var entityType = edmModel.FindType("CollectionAtomic.ComplexTypeE") as IEdmEntityType;
        property = complexType.FindProperty("Collection");
        Assert.Equal("Collection", property.Name);
    }

    //[TestMethod, Variation(Id = 42, SkipReason = @"[EdmLib] When the name of an element is changed, the FindType method should fail when it is called with the old name. -- postponed")]
    [Fact]
    public void FindMethodsTestForElementsAfterNameChange()
    {
        var model = new EdmModel();
        var complexType = new StubEdmComplexType("MyNamespace", "ComplexType1");
        model.AddElement(complexType);
        complexType.Name = "ComplexType2";

        Assert.Contains(model.SchemaElements, n => n.FullName() == "MyNamespace.ComplexType2");
        Assert.True(!model.SchemaElements.Any(n => n.FullName() == "MyNamespace.ComplexType1"));
        Assert.Null(model.FindType("MyNamespace.ComplexType1"));
        Assert.Equal("MyNamespace.ComplexType2", model.FindType("MyNamespace.ComplexType2").FullName());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestTermAndFunctionCsdl(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.String"" />
    <Term Name=""NotePerson"" Type=""DefaultNamespace.Person"" />
    <Term Name=""NoteSimpleType"" Type=""DefaultNamespace.SimpleType"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <ComplexType Name=""SimpleType"">
      <Property Name=""Data"" Type=""Edm.String"" />
    </ComplexType>
    <Function Name=""SimpleFunction""><ReturnType Type=""Edm.String""/>
        <Parameter Name=""Person"" Type=""Edm.String"" />
    </Function>
</Schema>";

        var csdls = new XElement[] { XElement.Parse(csdl) };
        var csdlElements = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdls, edmVersion);

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdlElements, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestFunctionOverloadingWithDifferentParametersCountCsdls(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Function Name=""SimpleFunction""><ReturnType Type=""Edm.String""/>
        <Parameter Name=""Person"" Type=""Edm.String"" />
    </Function>
    <Function Name=""SimpleFunction""><ReturnType Type=""Edm.String""/>
        <Parameter Name=""Person"" Type=""Edm.String"" />
        <Parameter Name=""Count"" Type=""Edm.Int32"" />
    </Function>
</Schema>";

        var csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

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

        var functionInfos = new List<FunctionInfo>() { simpleFunction1, simpleFunction2 };

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);

        foreach (FunctionInfo functionInfo in functionInfos)
        {
            var functionsFound = model.FindOperations("DefaultNamespace" + "." + functionInfo.Name);
            var functionFoundCount = GetOperationMatchedCount(functionInfo, functionsFound);
            Assert.Equal(1, functionFoundCount);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestFunctionImportOverloadingWithDifferentParametersCountCsdl(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Function Name=""SimpleFunction""><ReturnType Type=""Edm.String""/>
    <Parameter Name=""Person"" Type=""Edm.Int32"" />
  </Function>
  <Function Name=""SimpleFunction""><ReturnType Type=""Edm.String""/>
    <Parameter Name=""Person"" Type=""Edm.Int32"" />
    <Parameter Name=""Count"" Type=""Edm.Int32"" />
  </Function>
  <EntityContainer Name=""Container"">
    <FunctionImport Name=""SimpleFunction"" Function=""DefaultNamespace.SimpleFunction"" />
  </EntityContainer>
</Schema>";

        var csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

        var simpleFunction1 = new FunctionInfo()
        {
            Name = "SimpleFunction",
            Parameters = new ParameterInfoList() { { "Person", "[Edm.Int32 Nullable=True]" } }
        };

        var simpleFunction2 = new FunctionInfo()
        {
            Name = "SimpleFunction",
            Parameters = new ParameterInfoList() { { "Person", "[Edm.Int32 Nullable=True]" },
                                                       { "Count", "[Edm.Int32 Nullable=True]" }}
        };

        var functionInfos = new List<FunctionInfo>() { simpleFunction1, simpleFunction2 };

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);

        foreach (FunctionInfo functionInfo in functionInfos)
        {
            var functionsFound = model.FindOperations("Container" + "." + functionInfo.Name);
            var functionFoundCount = GetOperationMatchedCount(functionInfo, functionsFound);
            Assert.Equal(1, functionFoundCount);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestFunctionImportOverloadingWithComplexParameterCsdl(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name='MyEntityType'>
        <Key>
          <PropertyRef Name='Id' />
        </Key>
        <Property Name='Id' Type='Edm.Int32' Nullable='false' />
      </EntityType>
      <ComplexType Name='MyComplexType'>
        <Property Name=""Data"" Type=""Edm.String"" />
      </ComplexType>
      <Function Name='MyFunction'><ReturnType Type='Edm.String'/>
        <Parameter Name='P1' Type='Collection(DefaultNamespace.MyComplexType)' />
        <Parameter Name='P2' Type='DefaultNamespace.MyEntityType' />
        <Parameter Name='P3' Type='Collection(DefaultNamespace.MyEntityType)' />
        <Parameter Name='P4' Type='Edm.Int32' />
      </Function>
      <Function Name='MyFunction'><ReturnType Type='Edm.String'/>
        <Parameter Name='P1' Type='Collection(DefaultNamespace.MyComplexType)' />
        <Parameter Name='P2' Type='DefaultNamespace.MyEntityType' />
        <Parameter Name='P3b' Type='DefaultNamespace.MyComplexType' />
        <Parameter Name='P4' Type='Edm.Int32' />
      </Function>
      <EntityContainer Name='Container'>
        <FunctionImport Name='MyFunction' Function='DefaultNamespace.MyFunction' />
      </EntityContainer>
</Schema>";

        var csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));

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

        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);

        foreach (FunctionInfo functionInfo in functionInfos)
        {
            var functionsFound = model.FindOperations("Container" + "." + functionInfo.Name);
            var functionFoundCount = GetOperationMatchedCount(functionInfo, functionsFound);
            Assert.Equal(1, functionFoundCount);
        }
    }

    [Fact]
    public void FindMethodTestFindFunctionImportThatDoesNotExist()
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name='MyEntityType'>
        <Key>
          <PropertyRef Name='Id' />
        </Key>
        <Property Name='Id' Type='Edm.Int32' Nullable='false' />
      </EntityType>
      <ComplexType Name='MyComplexType'>
        <Property Name=""Data"" Type=""Edm.String"" />
      </ComplexType>
      <Function Name='MyFunction'><ReturnType Type='Edm.String'/>
        <Parameter Name='P1' Type='Collection(DefaultNamespace.MyComplexType)' />
        <Parameter Name='P2' Type='DefaultNamespace.MyEntityType' />
        <Parameter Name='P3' Type='Collection(DefaultNamespace.MyEntityType)' />
        <Parameter Name='P4' Type='Edm.Int32' />
      </Function>
      <Function Name='MyFunction'><ReturnType Type='Edm.String'/>
        <Parameter Name='P1' Type='Collection(DefaultNamespace.MyComplexType)' />
        <Parameter Name='P2' Type='DefaultNamespace.MyEntityType' />
        <Parameter Name='P3b' Type='DefaultNamespace.MyComplexType' />
        <Parameter Name='P4' Type='Edm.Int32' />
      </Function>
      <EntityContainer Name='Container'>
        <FunctionImport Name='MyFunction' Function='DefaultNamespace.MyFunction' />
      </EntityContainer>
</Schema>";

        var csdlElements = new XElement[] { XElement.Parse(csdl) };
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var entityContainer = model.FindEntityContainer("Container");
        var operationImports = entityContainer.FindOperationImports("foobaz");
        Assert.NotNull(operationImports);
        Assert.Empty(operationImports);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestEntitySetWithSingleNavigationCsdl(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""Container"">
        <EntitySet Name=""BuyerSet"" EntityType=""DefaultNamespace.Person"">
          <NavigationPropertyBinding Path=""ItemPurchased"" Target=""ItemSet"" />
        </EntitySet>
        <EntitySet Name=""ItemSet"" EntityType=""DefaultNamespace.Item"">
          <NavigationPropertyBinding Path=""Purchaser"" Target=""BuyerSet"" />
        </EntitySet>
    </EntityContainer>
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""ItemPurchased"" Type=""DefaultNamespace.Item"" Nullable=""false"" Partner=""Purchaser"" />
    </EntityType>
    <EntityType Name=""Item"">
        <Key>
            <PropertyRef Name=""PersonId"" />
        </Key>
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""Purchaser"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ItemPurchased"" />
    </EntityType>
</Schema>";

        var csdlElements = new XElement[] { XElement.Parse(csdl) };
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var ruleset = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
        var validationResult = model.Validate(ruleset, out IEnumerable<EdmError> actualErrors);
        Assert.True(validationResult);
        Assert.Empty(actualErrors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);

        var personToItem = (model.FindType("DefaultNamespace.Person") as IEdmEntityType).NavigationProperties().Where(n => n.Name.Equals("ItemPurchased")).FirstOrDefault();
        var itemToPerson = (model.FindType("DefaultNamespace.Item") as IEdmEntityType).NavigationProperties().Where(n => n.Name.Equals("Purchaser")).FirstOrDefault();

        var buyerSet = model.EntityContainer.FindEntitySet("BuyerSet");
        var itemSet = model.EntityContainer.FindEntitySet("ItemSet");

        var buyerEntityTarget = buyerSet.FindNavigationTarget(personToItem);
        Assert.NotNull(buyerEntityTarget);
        Assert.Equal(itemSet.Name, buyerEntityTarget.Name);
        Assert.Equal(itemSet, buyerEntityTarget);

        buyerEntityTarget = buyerSet.FindNavigationTarget(itemToPerson);
        Assert.True(buyerEntityTarget is IEdmUnknownEntitySet);

        var itemEntityTarget = itemSet.FindNavigationTarget(itemToPerson);
        Assert.NotNull(itemEntityTarget);
        Assert.Equal(buyerSet.Name, itemEntityTarget.Name);
        Assert.Equal(buyerSet, itemEntityTarget);

        itemEntityTarget = itemSet.FindNavigationTarget(personToItem);
        Assert.True(itemEntityTarget is IEdmUnknownEntitySet);

        var csdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements, edmVersion);

        isParsed = SchemaReader.TryParse(csdls.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdls, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestEntitySetWithTwoNavigationCsdl(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""Container"">
        <EntitySet Name=""BuyerSet"" EntityType=""DefaultNamespace.Person"">
          <NavigationPropertyBinding Path=""ItemPurchased"" Target=""ItemSet"" />
          <NavigationPropertyBinding Path=""Pet"" Target=""PetSet"" />
        </EntitySet>
        <EntitySet Name=""ItemSet"" EntityType=""DefaultNamespace.Item"">
          <NavigationPropertyBinding Path=""Purchaser"" Target=""BuyerSet"" />
        </EntitySet>
        <EntitySet Name=""PetSet"" EntityType=""DefaultNamespace.Pet"">
          <NavigationPropertyBinding Path=""Owner"" Target=""BuyerSet"" />
        </EntitySet>
    </EntityContainer>
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""ItemPurchased"" Type=""DefaultNamespace.Item"" Nullable=""false"" Partner=""Purchaser"" />
        <NavigationProperty Name=""Pet"" Type=""DefaultNamespace.Pet"" Nullable=""false"" Partner=""Owner"" />
    </EntityType>
    <EntityType Name=""Item"">
        <Key>
            <PropertyRef Name=""PersonId"" />
        </Key>
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""Purchaser"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ItemPurchased"" />
    </EntityType>
    <EntityType Name=""Pet"">
        <Key>
            <PropertyRef Name=""PersonId"" />
        </Key>
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""Owner"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""Pet"" />
    </EntityType>
</Schema>";

        var csdlElements = new XElement[] { XElement.Parse(csdl) };
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);

        var personToItem = (model.FindType("DefaultNamespace.Person") as IEdmEntityType).NavigationProperties().Where(n => n.Name.Equals("ItemPurchased")).FirstOrDefault();
        var itemToPerson = (model.FindType("DefaultNamespace.Item") as IEdmEntityType).NavigationProperties().Where(n => n.Name.Equals("Purchaser")).FirstOrDefault();

        var ownerToPet = (model.FindType("DefaultNamespace.Person") as IEdmEntityType).NavigationProperties().Where(n => n.Name.Equals("Pet")).FirstOrDefault();
        var petToOwner = (model.FindType("DefaultNamespace.Pet") as IEdmEntityType).NavigationProperties().Where(n => n.Name.Equals("Owner")).FirstOrDefault();

        var buyerSet = model.EntityContainer.FindEntitySet("BuyerSet");
        var itemSet = model.EntityContainer.FindEntitySet("ItemSet");
        var petSet = model.EntityContainer.FindEntitySet("PetSet");

        var buyerEntityTarget = buyerSet.FindNavigationTarget(personToItem);
        Assert.NotNull(buyerEntityTarget);
        Assert.Equal(itemSet.Name, buyerEntityTarget.Name);
        Assert.Equal(itemSet, buyerEntityTarget);

        buyerEntityTarget = buyerSet.FindNavigationTarget(itemToPerson);
        Assert.True(buyerEntityTarget is IEdmUnknownEntitySet);

        var itemEntityTarget = itemSet.FindNavigationTarget(itemToPerson);
        Assert.NotNull(itemEntityTarget);
        Assert.Equal(buyerSet.Name, itemEntityTarget.Name);
        Assert.Equal(buyerSet, itemEntityTarget);

        itemEntityTarget = itemSet.FindNavigationTarget(personToItem);
        Assert.True(itemEntityTarget is IEdmUnknownEntitySet);

        buyerEntityTarget = buyerSet.FindNavigationTarget(ownerToPet);
        Assert.NotNull(buyerEntityTarget);
        Assert.Equal(itemSet.Name, buyerEntityTarget.Name);
        Assert.Equal(itemSet, buyerEntityTarget);

        buyerEntityTarget = buyerSet.FindNavigationTarget(petToOwner);
        Assert.True(buyerEntityTarget is IEdmUnknownEntitySet);

        var petEntityTarget = petSet.FindNavigationTarget(ownerToPet);
        Assert.NotNull(petEntityTarget);
        Assert.Equal(buyerSet.Name, petEntityTarget.Name);
        Assert.Equal(buyerSet, petEntityTarget);

        petEntityTarget = petSet.FindNavigationTarget(petToOwner);
        Assert.True(petEntityTarget is IEdmUnknownEntitySet);

        var csdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements, edmVersion);

        isParsed = SchemaReader.TryParse(csdls.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdls, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestEntitySetRecursiveNavigationCsdl(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""Container"">
        <EntitySet Name=""PersonSet"" EntityType=""DefaultNamespace.Person"">
          <NavigationPropertyBinding Path=""ToPerson"" Target=""PersonSet"" />
          <NavigationPropertyBinding Path=""ToFriend"" Target=""PersonSet"" />
        </EntitySet>
    </EntityContainer>
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""ToFriend"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToPerson"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToFriend"" />
    </EntityType>
</Schema>";

        var csdlElements = new XElement[] { XElement.Parse(csdl) };
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);

        var personToFriend =  (model.FindType("DefaultNamespace.Person") as IEdmEntityType).NavigationProperties().Where(n => n.Name.Equals("ToFriend")).FirstOrDefault();
        var friendToPerson =  (model.FindType("DefaultNamespace.Person") as IEdmEntityType).NavigationProperties().Where(n => n.Name.Equals("ToPerson")).FirstOrDefault();

        var personSet = model.EntityContainer.FindEntitySet("PersonSet");

        var entityTarget = personSet.FindNavigationTarget(personToFriend);
        Assert.NotNull(entityTarget);
        Assert.Equal(personSet.Name, entityTarget.Name);
        Assert.Equal(personSet, entityTarget);

        entityTarget = personSet.FindNavigationTarget(friendToPerson);
        Assert.NotNull(entityTarget);
        Assert.Equal(personSet.Name, entityTarget.Name);
        Assert.Equal(personSet, entityTarget);

        var csdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(csdlElements, edmVersion);

        isParsed = SchemaReader.TryParse(csdls.Select(e => e.CreateReader()), out IEdmModel testModelImmutable, out errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        this.VerifyFindMethods(csdls, testModelImmutable, edmVersion);

        var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
        var testCsdls = this.GetSerializerResult(testModelConstructible, out serializationErrors).Select(n => XElement.Parse(n));
        Assert.Empty(serializationErrors);

        this.VerifyFindMethods(testCsdls, testModelConstructible, edmVersion);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindMethodTestEntitySetNavigationUsedTwiceCsdl(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""Container"">
        <EntitySet Name=""BuyerSet"" EntityType=""DefaultNamespace.Person"">
          <NavigationPropertyBinding Path=""ItemPurchased"" Target=""ItemSet"" />
        </EntitySet>
        <EntitySet Name=""ItemSet"" EntityType=""DefaultNamespace.Item"">
          <NavigationPropertyBinding Path=""Purchaser"" Target=""BuyerSet"" />
        </EntitySet>
        <EntitySet Name=""SecondBuyerSet"" EntityType=""DefaultNamespace.Person"">
          <NavigationPropertyBinding Path=""ItemPurchased"" Target=""SecondItemSet"" />
        </EntitySet>
        <EntitySet Name=""SecondItemSet"" EntityType=""DefaultNamespace.Item"">
          <NavigationPropertyBinding Path=""Purchaser"" Target=""SecondBuyerSet"" />
        </EntitySet>
    </EntityContainer>
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""ItemPurchased"" Type=""DefaultNamespace.Item"" Nullable=""false"" Partner=""Purchaser"" />
    </EntityType>
    <EntityType Name=""Item"">
        <Key>
            <PropertyRef Name=""PersonId"" />
        </Key>
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""Purchaser"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ItemPurchased"" />
    </EntityType>
</Schema>";

        var csdlElements = new string[] { csdl }.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var serializedCsdls = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        // if the original test model is valid, the round-tripped model should be well-formed and valid.
        var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel roundtrippedModel, out IEnumerable<EdmError> parserErrors);
        Assert.True(isWellformed && !parserErrors.Any());

        var isValid = roundtrippedModel.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.True(!validationErrors.Any() && isValid);

        var personToItem =  (model.FindType("DefaultNamespace.Person") as IEdmEntityType).NavigationProperties().Where(n => n.Name.Equals("ItemPurchased")).FirstOrDefault();
        var itemToPerson =  (model.FindType("DefaultNamespace.Item") as IEdmEntityType).NavigationProperties().Where(n => n.Name.Equals("Purchaser")).FirstOrDefault();

        var buyerSet = model.EntityContainer.FindEntitySet("BuyerSet");
        var itemSet = model.EntityContainer.FindEntitySet("ItemSet");
        var secondBuyerSet = model.EntityContainer.FindEntitySet("SecondBuyerSet");
        var secondItemSet = model.EntityContainer.FindEntitySet("SecondItemSet");

        var buyerEntityTarget = buyerSet.FindNavigationTarget(personToItem);
        Assert.NotNull(buyerEntityTarget);
        Assert.Equal(itemSet.Name, buyerEntityTarget.Name);
        Assert.Equal(itemSet, buyerEntityTarget);

        buyerEntityTarget = buyerSet.FindNavigationTarget(itemToPerson);
        Assert.True(buyerEntityTarget is IEdmUnknownEntitySet);

        var itemEntityTarget = itemSet.FindNavigationTarget(itemToPerson);
        Assert.NotNull(itemEntityTarget);
        Assert.Equal(buyerSet.Name, itemEntityTarget.Name);
        Assert.Equal(buyerSet, itemEntityTarget);

        itemEntityTarget = itemSet.FindNavigationTarget(personToItem);
        Assert.True(itemEntityTarget is IEdmUnknownEntitySet);

        var secondBuyerEntityTarget = secondBuyerSet.FindNavigationTarget(personToItem);
        buyerEntityTarget = secondBuyerSet.FindNavigationTarget(itemToPerson);
        Assert.NotNull(buyerEntityTarget);
        Assert.Equal(secondItemSet.Name, buyerEntityTarget.Name);
        Assert.Equal(secondItemSet, buyerEntityTarget);

        secondBuyerEntityTarget = secondBuyerSet.FindNavigationTarget(itemToPerson);
        Assert.True(secondBuyerEntityTarget is IEdmUnknownEntitySet);

        var secondItemEntityTarget = secondItemSet.FindNavigationTarget(itemToPerson);
        Assert.NotNull(secondItemEntityTarget);
        Assert.Equal(secondBuyerSet.Name, secondItemEntityTarget.Name);
        Assert.Equal(secondBuyerSet, secondItemEntityTarget);

        secondItemEntityTarget = secondItemSet.FindNavigationTarget(personToItem);
        Assert.True(secondItemEntityTarget is IEdmUnknownEntitySet);
    }

    [Fact]
    public void FindMethodTestFindVocabularyAnnotationWithTermCsdl()
    {
        var csdls = new string[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Pet"">
        <Property Name=""OwnerId"" Type=""Edm.Int32"" />
        <Property Name=""Name"" Type=""Edm.String"" />
    </ComplexType>
    <Annotations Target=""DefaultNamespace.Pet"">
        <Annotation Term=""AnnotationNamespace.AddressObject"">
        </Annotation>
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""AddressObject"" Type=""AnnotationNamespace.Address"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""FakeId"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>" };
        
        var csdlElements = csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var modelVocabulary = model.VocabularyAnnotations;
        Assert.Single(modelVocabulary);

        var addressTerm = model.FindTerm("AnnotationNamespace.AddressObject");
        var petType = model.FindType("DefaultNamespace.Pet") as IEdmComplexType;

        var baseValue1 = model.FindDeclaredVocabularyAnnotations(addressTerm);
        Assert.DoesNotContain([baseValue1, model.FindVocabularyAnnotations(addressTerm)], n => n.Count() != baseValue1.Count() || baseValue1.Except(n).Any());
        Assert.Empty(model.FindDeclaredVocabularyAnnotations(addressTerm));

        var baseValue2 = model.FindDeclaredVocabularyAnnotations(petType);
        Assert.DoesNotContain([baseValue2, model.FindVocabularyAnnotations(petType)], n => n.Count() != baseValue2.Count() || baseValue2.Except(n).Any());
        Assert.Single(model.FindDeclaredVocabularyAnnotations(petType));

        var valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(addressTerm, addressTerm);
        Assert.Empty(valueAnnotationsFound);

        valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(petType, "AnnotationNamespace.Person");
        Assert.Empty(valueAnnotationsFound);

        valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(petType, "AnnotationNamespace.Nonsense");
        Assert.Empty(valueAnnotationsFound);

        valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(petType, addressTerm);
        Assert.Single(valueAnnotationsFound);

        valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(petType, "AnnotationNamespace.AddressObject");
        Assert.Single(valueAnnotationsFound);

        var expectedValueAnnotation = petType.VocabularyAnnotations(model).SingleOrDefault();
        Assert.Equal(expectedValueAnnotation, valueAnnotationsFound.SingleOrDefault());
    }

    [Fact]
    public void FindMethodTestFindVocabularyAnnotationWithEntityTypeCsdl()
    {
        var csdls = new string[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Pet"">
        <Property Name=""OwnerId"" Type=""Edm.Int32"" />
        <Property Name=""Name"" Type=""Edm.String"" />
    </ComplexType>
    <Annotations Target=""DefaultNamespace.Pet"">
        <Annotation Term=""AnnotationNamespace.Person"">
        </Annotation>
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""AddressObject"" Type=""AnnotationNamespace.Address"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""FakeId"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>" };

        var csdlElements = csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var modelVocabulary = model.VocabularyAnnotations;
        Assert.Single(modelVocabulary);

        var personEntity = model.FindType("AnnotationNamespace.Person") as IEdmEntityType;
        var addressTerm = model.FindTerm("AnnotationNamespace.AddressObject");
        var petType = model.FindType("DefaultNamespace.Pet") as IEdmComplexType;

        var baseValue1 = model.FindDeclaredVocabularyAnnotations(personEntity);
        Assert.DoesNotContain([baseValue1, model.FindVocabularyAnnotations(personEntity)], n => n.Count() != baseValue1.Count() || baseValue1.Except(n).Any());
        Assert.Empty(model.FindDeclaredVocabularyAnnotations(personEntity));

        var baseValue2 = model.FindDeclaredVocabularyAnnotations(addressTerm);
        Assert.DoesNotContain([baseValue2, model.FindVocabularyAnnotations(addressTerm)], n => n.Count() != baseValue2.Count() || baseValue2.Except(n).Any());
        Assert.Empty(model.FindDeclaredVocabularyAnnotations(addressTerm));

        var baseValue3 = model.FindDeclaredVocabularyAnnotations(petType);
        Assert.DoesNotContain([baseValue3, model.FindVocabularyAnnotations(petType)], n => n.Count() != baseValue2.Count() || baseValue2.Except(n).Any());
        Assert.Single(model.FindDeclaredVocabularyAnnotations(petType));

        var valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(addressTerm, addressTerm);
        Assert.Empty(valueAnnotationsFound);

        valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(petType, addressTerm);
        Assert.Empty(valueAnnotationsFound);
    }

    [Fact]
    public void FindMethodTestFindVocabularyAnnotationWithComplexTypeCsdl()
    {
        var csdls = new string[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Pet"">
        <Property Name=""OwnerId"" Type=""Edm.Int32"" />
        <Property Name=""Name"" Type=""Edm.String"" />
    </ComplexType>
    <Annotations Target=""DefaultNamespace.Pet"">
        <Annotation Term=""AnnotationNamespace.Address"">
        </Annotation>
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""AddressObject"" Type=""AnnotationNamespace.Address"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""FakeId"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>" };

        var csdlElements = csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var modelVocabulary = model.VocabularyAnnotations;
        Assert.Single(modelVocabulary);

        var addressTerm = model.FindTerm("AnnotationNamespace.AddressObject");
        var petType = model.FindType("DefaultNamespace.Pet") as IEdmComplexType;

        var baseValue1 = model.FindDeclaredVocabularyAnnotations(addressTerm);
        Assert.DoesNotContain([baseValue1, model.FindVocabularyAnnotations(addressTerm)], n => n.Count() != baseValue1.Count() || baseValue1.Except(n).Any());

        var baseValue2 = model.FindDeclaredVocabularyAnnotations(petType);
        Assert.DoesNotContain([baseValue2, model.FindVocabularyAnnotations(petType)], n => n.Count() != baseValue2.Count() || baseValue2.Except(n).Any());

        var valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(addressTerm, addressTerm);
        Assert.Empty(valueAnnotationsFound);

        valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(petType, addressTerm);
        Assert.Empty(valueAnnotationsFound);
    }

    [Fact]
    public void FindMethodTestFindVocabularyAnnotationWithMultipleTermsCsdl()
    {
        var csdls = new string[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Pet"">
        <Property Name=""OwnerId"" Type=""Edm.Int32"" />
        <Property Name=""Name"" Type=""Edm.String"" />
    </ComplexType>
    <Annotations Target=""DefaultNamespace.Pet"">
        <Annotation Term=""AnnotationNamespace.Address"">
        </Annotation>
        <Annotation Term=""AnnotationNamespace.AddressObject"">
        </Annotation>
        <Annotation Term=""AnnotationNamespace.AddressObject"">
        </Annotation>
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""AddressObject"" Type=""AnnotationNamespace.Address"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""FakeId"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>" };

        var csdlElements = csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var modelVocabulary = model.VocabularyAnnotations;
        Assert.Equal(3, modelVocabulary.Count());

        var personEntity = model.FindType("AnnotationNamespace.Person") as IEdmEntityType;
        var addressTerm = model.FindTerm("AnnotationNamespace.AddressObject");
        var petType = model.FindType("DefaultNamespace.Pet") as IEdmComplexType;

        var baseValue1 = model.FindDeclaredVocabularyAnnotations(personEntity);
        Assert.DoesNotContain([baseValue1, model.FindVocabularyAnnotations(personEntity)], n => n.Count() != baseValue1.Count() || baseValue1.Except(n).Any());
        Assert.Empty(model.FindDeclaredVocabularyAnnotations(personEntity));

        var baseValue2 = model.FindDeclaredVocabularyAnnotations(addressTerm);
        Assert.DoesNotContain([baseValue2, model.FindVocabularyAnnotations(addressTerm)], n => n.Count() != baseValue2.Count() || baseValue2.Except(n).Any());
        Assert.Empty(model.FindDeclaredVocabularyAnnotations(addressTerm));

        var baseValue3 = model.FindDeclaredVocabularyAnnotations(petType);
        Assert.DoesNotContain([baseValue3, model.FindVocabularyAnnotations(petType)], n => n.Count() != baseValue3.Count() || baseValue3.Except(n).Any());
        Assert.Equal(3, model.FindDeclaredVocabularyAnnotations(petType).Count());

        var expectedValueAnnotation = modelVocabulary.Where(n => n.Term == addressTerm);

        var valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(addressTerm, addressTerm);
        Assert.Empty(valueAnnotationsFound);

        valueAnnotationsFound = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(petType, addressTerm);
        Assert.Equal(2, valueAnnotationsFound.Count());
        Assert.Equal(expectedValueAnnotation.ElementAt(0), valueAnnotationsFound.ElementAt(0));
        Assert.Equal(expectedValueAnnotation.ElementAt(1), valueAnnotationsFound.ElementAt(1));
    }

    [Fact]
    public void FindMethodTestFindVocabularyAnnotationAcrossModelOutOfLineAnnotationCsdl()
    {
        var csdls = new string[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""ValueAnnotationType"">
    </ComplexType>
    <Annotations Target=""DefaultNamespace.ValueAnnotationType"">
        <Annotation Term=""DefaultNamespace.ValueTermInModel"">
        </Annotation>
        <Annotation Term=""AnnotationNamespace.ValueTermOutOfModel"">
        </Annotation>
        <Annotation Term=""fooNamespace.ValueTermDoesNotExist"">
        </Annotation>
    </Annotations>
    <Term Name=""ValueTermInModel"" Type=""Edm.String"" />
    <EntityType Name=""PersonInMode"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""ValueTermOutOfModel"" Type=""Edm.String"" />
    <EntityType Name=""PersonOutOfMode"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
</Schema>" };

        var csdlElements = csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        Assert.Equal(3, model.VocabularyAnnotations.Count());

        var valueAnnotationType = model.FindType("DefaultNamespace.ValueAnnotationType");
        var baseValue1 = model.FindDeclaredVocabularyAnnotations(valueAnnotationType);
        Assert.DoesNotContain([baseValue1, model.FindVocabularyAnnotations(valueAnnotationType)], n => n.Count() != baseValue1.Count() || baseValue1.Except(n).Any());

        var valueAnnotationsFound = model.FindVocabularyAnnotations(valueAnnotationType);
        Assert.Equal(3, valueAnnotationsFound.Count());

        var valueAnnotationFoundCount = valueAnnotationsFound.Where(n => n.Term.Name.Equals("ValueTermInModel")).Count();
        Assert.Equal(1, valueAnnotationFoundCount);

        valueAnnotationFoundCount = valueAnnotationsFound.Where(n => n.Term.Name.Equals("ValueTermOutOfModel")).Count();
        Assert.Equal(1, valueAnnotationFoundCount);

        valueAnnotationFoundCount = valueAnnotationsFound.Where(n => n.Term.Name.Equals("ValueTermDoesNotExist")).Count();
        Assert.Equal(1, valueAnnotationFoundCount);
    }

    [Fact]
    public void FindMethodTestFindVocabularyAnnotationAcrossModelInLineAnnotationCsdl()
    {
        var csdls = new string[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""ValueAnnotationType"">
        <Annotation Term=""DefaultNamespace.ValueTermInModel"">
        </Annotation>
        <Annotation Term=""AnnotationNamespace.ValueTermOutOfModel"">
        </Annotation>
        <Annotation Term=""fooNamespace.ValueTermDoesNotExist"">
        </Annotation>
    </ComplexType>
    <Term Name=""ValueTermInModel"" Type=""Edm.String"" />
    <EntityType Name=""PersonInMode"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""ValueTermOutOfModel"" Type=""Edm.String"" />
    <EntityType Name=""PersonOutOfMode"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
</Schema>" };

        var csdlElements = csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        Assert.Equal(3, model.VocabularyAnnotations.Count());

        var valueAnnotationType = model.FindType("DefaultNamespace.ValueAnnotationType");
        var personType = model.FindType("DefaultNamespace.PersonInMode");

        var baseValue1 = model.FindDeclaredVocabularyAnnotations(valueAnnotationType);
        Assert.DoesNotContain([baseValue1, model.FindVocabularyAnnotations(valueAnnotationType)], n => n.Count() != baseValue1.Count() || baseValue1.Except(n).Any());

        var baseValue2 = model.FindDeclaredVocabularyAnnotations(personType);
        Assert.DoesNotContain([baseValue2, model.FindVocabularyAnnotations(personType)], n => n.Count() != baseValue2.Count() || baseValue2.Except(n).Any());

        var anntationFound = model.FindVocabularyAnnotations(personType);
        Assert.Empty(anntationFound);

        anntationFound = model.FindVocabularyAnnotations(valueAnnotationType);
        Assert.Equal(3, anntationFound.Count());

        var valueAnnotationsFound = model.FindVocabularyAnnotations(valueAnnotationType);
        Assert.Equal(3, valueAnnotationsFound.Count());

        var valueAnnotationFoundCount = valueAnnotationsFound.Where(n => n.Term.Name.Equals("ValueTermInModel")).Count();
        Assert.Equal(1, valueAnnotationFoundCount);

        valueAnnotationFoundCount = valueAnnotationsFound.Where(n => n.Term.Name.Equals("ValueTermOutOfModel")).Count();
        Assert.Equal(1, valueAnnotationFoundCount);

        valueAnnotationFoundCount = valueAnnotationsFound.Where(n => n.Term.Name.Equals("ValueTermDoesNotExist")).Count();
        Assert.Equal(1, valueAnnotationFoundCount);
    }

    [Fact]
    public void FindMethodTestFindTermSingleCsdl()
    {
        var csdls = new string[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""ValueTermInModel"" Type=""Edm.String"" />
    <Term Name=""AmbigousValueTerm"" Type=""Edm.String"" />
    <Term Name=""AmbigousValueTerm"" Type=""Edm.String"" />
    <Term Name=""ReferenceAmbigousValueTerm"" Type=""Edm.String"" />
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""ValueTerm"" Type=""Edm.String"" />
</Schema>" };

        var csdlElements = csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var valueTermInModel = model.FindTerm("DefaultNamespace.ValueTermInModel");
        Assert.NotNull(valueTermInModel);
        Assert.Equal(valueTermInModel, model.FindTerm("DefaultNamespace.ValueTermInModel"));

        var ambiguousValueTerm = model.FindTerm("DefaultNamespace.AmbigousValueTerm");
        Assert.NotNull(ambiguousValueTerm);
        Assert.True(ambiguousValueTerm.IsBad());
        Assert.Equal(ambiguousValueTerm, model.FindTerm("DefaultNamespace.AmbigousValueTerm"));

        var valueTermInOtherModel = model.FindTerm("AnnotationNamespace.ValueTerm");
        Assert.NotNull(valueTermInOtherModel);
        Assert.Equal(valueTermInOtherModel, model.FindTerm("AnnotationNamespace.ValueTerm"));

        var valueTermDoesNotExist = model.FindTerm("fooNamespace.ValueTerm");
        Assert.Null(valueTermDoesNotExist);
    }

    [Fact]
    public void FindMethodTestFindTermAmbiguousReferences()
    {
        var model = new EdmModel();

        var secondValueTerm = new EdmTerm("DefaultNamespace", "SecondValueTermInModel", EdmCoreModel.Instance.GetString(true));
        model.AddElement(secondValueTerm);

        var referenceAmbigous = new EdmTerm("DefaultNamespace", "ReferenceAmbigousValueTerm", EdmCoreModel.Instance.GetString(true));
        model.AddElement(referenceAmbigous);

        var csdls = new string[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""ValueTermInModel"" Type=""Edm.String"" />
    <Term Name=""AmbigousValueTerm"" Type=""Edm.String"" />
    <Term Name=""AmbigousValueTerm"" Type=""Edm.String"" />
    <Term Name=""ReferenceAmbigousValueTerm"" Type=""Edm.String"" />
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""ValueTerm"" Type=""Edm.String"" />
</Schema>" };

        var csdlElements = csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel referencedModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        model.AddReferencedModel(referencedModel);

        var valueTermInModel = model.FindTerm("DefaultNamespace.ValueTermInModel");
        Assert.NotNull(valueTermInModel);
        Assert.Equal(valueTermInModel, referencedModel.FindTerm("DefaultNamespace.ValueTermInModel"));

        var secondValueTermInModel = model.FindTerm("DefaultNamespace.SecondValueTermInModel");
        Assert.NotNull(secondValueTermInModel);
        Assert.Equal(secondValueTermInModel, model.FindTerm("DefaultNamespace.SecondValueTermInModel"));

        model.Validate(out IEnumerable<EdmError> validationErrors);
        Assert.NotEmpty(validationErrors);

        var ambiguousValueTerm = model.FindTerm("DefaultNamespace.ReferenceAmbigousValueTerm");
        Assert.NotNull(ambiguousValueTerm);
        Assert.True(ambiguousValueTerm.IsBad());
    }

    [Fact]
    public void FindMethodTestFindTypeComplexTypeCsdl()
    {
        var csdls = new string[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""SimpleType"" />
    <ComplexType Name=""AmbiguousType"" />
    <ComplexType Name=""AmbiguousType"" />
    <ComplexType Name=""ReferenceAmbiguousType"" />
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""SimpleType"" />
</Schema>" };

        var csdlElements = csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var simpleType = model.FindType("DefaultNamespace.SimpleType");
        Assert.NotNull(simpleType);
        Assert.Equal(simpleType, model.FindType("DefaultNamespace.SimpleType"));

        var ambiguousType = model.FindType("DefaultNamespace.AmbiguousType");
        Assert.NotNull(ambiguousType);
        Assert.True(ambiguousType.IsBad());
        Assert.Equal(ambiguousType, model.FindType("DefaultNamespace.AmbiguousType"));

        var simpleTypeInOtherModel = model.FindType("AnnotationNamespace.SimpleType");
        Assert.NotNull(simpleTypeInOtherModel);
        Assert.Equal(simpleTypeInOtherModel, model.FindType("AnnotationNamespace.SimpleType"));

        var complexTypeDoesNotExist = model.FindType("fooNamespace.SimpleType");
        Assert.Null(complexTypeDoesNotExist);
    }

    [Fact]
    public void FindMethodTestFindTypeEntityTypeCsdl()
    {
        var csdls = new string[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""SimpleEntity"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""AmbiguousEntity"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""AmbiguousEntity"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""ReferenceAmbiguousEntity"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""SimpleEntity"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
</Schema>" };

        var csdlElements = csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var simpleEntity = model.FindType("DefaultNamespace.SimpleEntity");
        Assert.NotNull(simpleEntity);
        Assert.Equal(simpleEntity, model.FindType("DefaultNamespace.SimpleEntity"));

        var ambiguousEntity = model.FindType("DefaultNamespace.AmbiguousEntity");
        Assert.NotNull(ambiguousEntity);
        Assert.True(ambiguousEntity.IsBad());
        Assert.Equal(ambiguousEntity, model.FindType("DefaultNamespace.AmbiguousEntity"));

        var simpleEntityInOtherModel = model.FindType("AnnotationNamespace.SimpleEntity");
        Assert.NotNull(simpleEntityInOtherModel);
        Assert.Equal(simpleEntityInOtherModel, model.FindType("AnnotationNamespace.SimpleEntity"));

        var entityTypeDoesNotExist = model.FindType("fooNamespace.SimpleEntity");
        Assert.Null(entityTypeDoesNotExist);
    }

    [Fact]
    public void FindMethodTestFindTypeModel()
    {
        var model = new EdmModel();

        model.AddElement(new EdmComplexType("DefaultNamespace", "SecondSimpleType"));

        var referenceAmbiguousType = new EdmComplexType("DefaultNamespace", "ReferenceAmbiguousType");
        model.AddElement(referenceAmbiguousType);

        var secondSimpleEntity = new EdmEntityType("DefaultNamespace", "SecondSimpleEntity");
        var secondSimpleEntityId = secondSimpleEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        model.AddElement(secondSimpleEntity);

        var referenceAmbiguousEntity = new EdmEntityType("DefaultNamespace", "ReferenceAmbiguousEntity");
        var referenceAmbiguousEntityId = referenceAmbiguousEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        model.AddElement(referenceAmbiguousEntity);

        var entityTypeCsdls = new string[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""SimpleEntity"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""AmbiguousEntity"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""AmbiguousEntity"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""ReferenceAmbiguousEntity"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""SimpleEntity"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
</Schema>" };

        var isParsed = SchemaReader.TryParse(entityTypeCsdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo)).Select(e => e.CreateReader()), out IEdmModel referencedEntityTypeModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var complexTypeCsdls = new string[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""SimpleType"" />
    <ComplexType Name=""AmbiguousType"" />
    <ComplexType Name=""AmbiguousType"" />
    <ComplexType Name=""ReferenceAmbiguousType"" />
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""SimpleType"" />
</Schema>" };

        isParsed = SchemaReader.TryParse(entityTypeCsdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo)).Select(e => e.CreateReader()), out IEdmModel referencedComplexTypeModel, out errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        model.AddReferencedModel(referencedEntityTypeModel);
        model.AddReferencedModel(referencedComplexTypeModel);

        var secondSimpleType = model.FindType("DefaultNamespace.SecondSimpleType");
        Assert.NotNull(secondSimpleType);
        Assert.Equal(secondSimpleType, model.FindType("DefaultNamespace.SecondSimpleType"));

        var simpleType = model.FindType("DefaultNamespace.SimpleType");
        Assert.NotNull(simpleType);
        Assert.Equal(simpleType, referencedComplexTypeModel.FindType("DefaultNamespace.SimpleType"));

        var ambiguousComplexType = model.FindType("DefaultNamespace.ReferenceAmbiguousType");
        Assert.NotNull(ambiguousComplexType);
        Assert.True(ambiguousComplexType.IsBad());

        var simpleTypeInOtherNamespace = model.FindType("AnnotationNamespace.SimpleType");
        Assert.NotNull(simpleTypeInOtherNamespace);
        Assert.Equal(simpleTypeInOtherNamespace, referencedComplexTypeModel.FindType("AnnotationNamespace.SimpleType"));

        var findSecondSimpleEntity = model.FindType("DefaultNamespace.SecondSimpleEntity");
        Assert.NotNull(findSecondSimpleEntity);
        Assert.Equal(findSecondSimpleEntity, model.FindType("DefaultNamespace.SecondSimpleEntity"));

        var simpleEntity = model.FindType("DefaultNamespace.SimpleEntity");
        Assert.NotNull(simpleEntity);
        Assert.Equal(simpleEntity, referencedEntityTypeModel.FindType("DefaultNamespace.SimpleEntity"));

        var ambiguousEntityType = model.FindType("DefaultNamespace.ReferenceAmbiguousEntity");
        Assert.NotNull(ambiguousEntityType);
        Assert.True(ambiguousEntityType.IsBad());

        var simpleEntityInOtherNamespace = model.FindType("AnnotationNamespace.SimpleEntity");
        Assert.NotNull(simpleEntityInOtherNamespace);
        Assert.Equal(simpleEntityInOtherNamespace, referencedEntityTypeModel.FindType("AnnotationNamespace.SimpleEntity"));
    }

    [Fact]
    public void FindMethodTestFindTypeDefinitionModel()
    {
        var model = new EdmModel();

        var secondSimpleType = new EdmComplexType("DefaultNamespace", "SecondSimpleType");
        model.AddElement(secondSimpleType);

        var referenceAmbiguousType = new EdmComplexType("DefaultNamespace", "ReferenceAmbiguousType");
        model.AddElement(referenceAmbiguousType);

        var secondSimpleEntity = new EdmEntityType("DefaultNamespace", "SecondSimpleEntity");
        var secondSimpleEntityId = secondSimpleEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        model.AddElement(secondSimpleEntity);

        var referenceAmbiguousEntity = new EdmEntityType("DefaultNamespace", "ReferenceAmbiguousEntity");
        var referenceAmbiguousEntityId = referenceAmbiguousEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        model.AddElement(referenceAmbiguousEntity);

        var csdls = new string[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <TypeDefinition Name=""Length"" UnderlyingType=""Edm.String"" />
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <TypeDefinition Name=""Length"" UnderlyingType=""Edm.Int32"" />
</Schema>" };

        var isParsed = SchemaReader.TryParse(csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo)).Select(e => e.CreateReader()), out IEdmModel referencedTypeDefinitionModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        model.AddReferencedModel(referencedTypeDefinitionModel);

        var lengthInDefaultNamespace = model.FindType("DefaultNamespace.Length");
        Assert.NotNull(lengthInDefaultNamespace);
        Assert.Equal(lengthInDefaultNamespace, model.FindType("DefaultNamespace.Length"));

        var lengthInOtherNamespace = model.FindType("AnnotationNamespace.Length");
        Assert.NotNull(lengthInOtherNamespace);
        Assert.Equal(lengthInOtherNamespace, referencedTypeDefinitionModel.FindType("AnnotationNamespace.Length"));
    }

    [Fact]
    public void FindMethodTestFindFunctionAcrossModelsCsdl()
    {
        var csdls = new string[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Function Name=""SimpleFunction""><ReturnType Type=""Edm.Int32"" /></Function>
    <Function Name=""AmbiguousFunction""><ReturnType Type=""Edm.Int32"" /></Function>
    <Function Name=""AmbiguousFunction""><ReturnType Type=""Edm.Int32"" /></Function>
    <Function Name=""ReferenceAmbiguousFunction""><ReturnType Type=""Edm.Int32"" /></Function>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Function Name=""SimpleFunction""><ReturnType Type=""Edm.Int32"" /></Function>
</Schema>" };

        var isParsed = SchemaReader.TryParse(csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo)).Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var simpleFunctions = model.FindOperations("DefaultNamespace.SimpleFunction");
        Assert.Single(simpleFunctions);
        var simpleFunction = simpleFunctions.SingleOrDefault();
        Assert.NotNull(simpleFunction);
        Assert.Equal(simpleFunction, model.FindOperations("DefaultNamespace.SimpleFunction").SingleOrDefault());

        model.Validate(out errors);
        Assert.Equal(2, errors.Count());
        Assert.Equal(EdmErrorCode.DuplicateFunctions, errors.ElementAt(0).ErrorCode);
        Assert.Equal(EdmErrorCode.DuplicateFunctions, errors.ElementAt(1).ErrorCode);

        var ambiguousFunctions = model.FindOperations("DefaultNamespace.AmbiguousFunction");
        Assert.Equal(2, ambiguousFunctions.Count());
        Assert.Equal(ambiguousFunctions.ElementAt(0), model.FindOperations("DefaultNamespace.AmbiguousFunction").ElementAt(0));
        Assert.Equal(ambiguousFunctions.ElementAt(1), model.FindOperations("DefaultNamespace.AmbiguousFunction").ElementAt(1));

        var simpleFunctionInOtherModels = model.FindOperations("AnnotationNamespace.SimpleFunction");
        Assert.Single(simpleFunctionInOtherModels);
        var simpleFunctionInOtherModel = simpleFunctionInOtherModels.SingleOrDefault();
        Assert.NotNull(simpleFunctionInOtherModel);
        Assert.Equal(simpleFunctionInOtherModel, model.FindOperations("AnnotationNamespace.SimpleFunction").SingleOrDefault());

        var functionsDoesNotExist = model.FindOperations("fooNamespace.SimpleFunction");
        Assert.Empty(functionsDoesNotExist);
    }

    [Fact]
    public void FindMethodTestFindFunctionAcrossModelsModel()
    {
        var model = new EdmModel();

        model.AddElement(new EdmFunction("DefaultNamespace", "SecondSimpleFunction", EdmCoreModel.Instance.GetInt32(true)));
        model.AddElement(new EdmFunction("DefaultNamespace", "ReferenceAmbiguousFunction", EdmCoreModel.Instance.GetInt32(true)));

        var csdls = new string[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Function Name=""SimpleFunction""><ReturnType Type=""Edm.Int32"" /></Function>
    <Function Name=""AmbiguousFunction""><ReturnType Type=""Edm.Int32"" /></Function>
    <Function Name=""AmbiguousFunction""><ReturnType Type=""Edm.Int32"" /></Function>
    <Function Name=""ReferenceAmbiguousFunction""><ReturnType Type=""Edm.Int32"" /></Function>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Function Name=""SimpleFunction""><ReturnType Type=""Edm.Int32"" /></Function>
</Schema>" };

        var isParsed = SchemaReader.TryParse(csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo)).Select(e => e.CreateReader()), out IEdmModel referencedFunctioneModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        model.AddReferencedModel(referencedFunctioneModel);

        var simpleFunctions = model.FindOperations("DefaultNamespace.SimpleFunction");
        Assert.Single(simpleFunctions);
        var simpleFunction = simpleFunctions.SingleOrDefault();
        Assert.NotNull(simpleFunction);
        Assert.Equal(simpleFunction, referencedFunctioneModel.FindOperations("DefaultNamespace.SimpleFunction").SingleOrDefault());

        var secondSimpleFunctions = model.FindOperations("DefaultNamespace.SecondSimpleFunction");
        Assert.Single(secondSimpleFunctions);
        var secondSimpleFunction = secondSimpleFunctions.SingleOrDefault();
        Assert.NotNull(secondSimpleFunction);
        Assert.Equal(secondSimpleFunction, model.FindOperations("DefaultNamespace.SecondSimpleFunction").SingleOrDefault());

        var simpleFunctionInOtherNamespaces = model.FindOperations("AnnotationNamespace.SimpleFunction");
        Assert.Single(simpleFunctionInOtherNamespaces);
        var simpleFunctionInOtherNamespace = simpleFunctionInOtherNamespaces.SingleOrDefault();
        Assert.NotNull(simpleFunctionInOtherNamespace);
        Assert.Equal(simpleFunctionInOtherNamespace, referencedFunctioneModel.FindOperations("AnnotationNamespace.SimpleFunction").SingleOrDefault());

        model.Validate(out errors);
        Assert.Single(errors);
        Assert.Equal(EdmErrorCode.AlreadyDefined, errors.First().ErrorCode);

        var referenceAmbiguousFunctions = model.FindOperations("DefaultNamespace.ReferenceAmbiguousFunction");
        Assert.Equal(2, referenceAmbiguousFunctions.Count());

        foreach (var referenceAmbiguousFunction in referenceAmbiguousFunctions)
        {
            Assert.NotNull(referenceAmbiguousFunction);
            Assert.False(referenceAmbiguousFunction.IsBad(), "Invalid is bad.");
        }
    }

    [Fact]
    public void FindMethodTestFindEntityContainerCsdl()
    {
        var csdls = new string[] { @"
<Schema Namespace=""Default"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""SimpleContainer"" />
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
</Schema>" };

        var isParsed = SchemaReader.TryParse(csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo)).Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var simpleContainer = model.FindEntityContainer("SimpleContainer");
        Assert.NotNull(simpleContainer);
        Assert.Equal(simpleContainer, model.FindEntityContainer("SimpleContainer"));
    }

    [Fact]
    public void FindMethodTestFindEntityContainerModel()
    {
        var model = new EdmModel();

        var secondSimpleContainer = new EdmEntityContainer("", "SecondSimpleContainer");
        model.AddElement(secondSimpleContainer);

        var csdls = new string[] { @"
<Schema Namespace=""Default"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""SimpleContainer"" />
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
</Schema>" };

        var isParsed = SchemaReader.TryParse(csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo)).Select(e => e.CreateReader()), out IEdmModel referencedEntityContainerModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        model.AddReferencedModel(referencedEntityContainerModel);

        var simpleContainer = model.FindEntityContainer("SimpleContainer");
        Assert.NotNull(simpleContainer);
        Assert.Equal(simpleContainer, referencedEntityContainerModel.FindEntityContainer("SimpleContainer"));
    }

    [Fact]
    public void FindMethodTestWithReferencesInEdmxParser()
    {
        var csdls = new string[] { @"
<Schema Namespace=""Default"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""SimpleContainer"" />
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
</Schema>" };

        var isParsed = SchemaReader.TryParse(csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo)).Select(e => e.CreateReader()), out IEdmModel referencedEntityContainerModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), new IEdmModel[] { referencedEntityContainerModel }, out IEdmModel model, out errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        var simpleContainer = model.FindEntityContainer("SimpleContainer");
        Assert.NotNull(simpleContainer);
        Assert.Equal(simpleContainer, referencedEntityContainerModel.FindEntityContainer("SimpleContainer"));
    }

    [Fact]
    public void FindMethodTestFindVocabularyAnnotationAcrossModelVocabularyAnnotationModel()
    {
        var model = new EdmModel();

        var containerOne = new EdmEntityContainer("DefaultNamespace", "ContainerOne");
        model.AddElement(containerOne);

        var termOne = new EdmTerm("DefaultNamespace", "TermOne", EdmCoreModel.Instance.GetString(true));
        model.AddElement(termOne);
        var termTwo = new EdmTerm("DefaultNamespace", "TermTwo", EdmCoreModel.Instance.GetString(true));
        model.AddElement(termTwo);

        var valueAnnotationOne = new EdmVocabularyAnnotation(
            containerOne,
            termOne,
            new EdmStringConstant("1"));
        valueAnnotationOne.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
        model.AddVocabularyAnnotation(valueAnnotationOne);

        var csdls = new string[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""ContainerThree"">
        <Annotation Term=""DefaultNamespace.TermThree"" String=""33"" />
    </EntityContainer>
    <Term Name=""TermThree"" Type=""Edm.String"" />
    <Annotations Target=""DefaultNamespace.ContainerTwo"">
        <Annotation Term=""DefaultNamespace.TermTwo"" String=""22"" />
    </Annotations>
</Schema>" };

        var isParsed = SchemaReader.TryParse(csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo)).Select(e => e.CreateReader()), out IEdmModel referenceModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        model.AddReferencedModel(referenceModel);

        var entityContainerOne = model.FindEntityContainer("ContainerOne");
        var containerThree = referenceModel.FindEntityContainer("ContainerThree");

        var baseValue1 = model.FindDeclaredVocabularyAnnotations(entityContainerOne);
        Assert.DoesNotContain([baseValue1, model.FindVocabularyAnnotations(entityContainerOne)], n => n.Count() != baseValue1.Count() || baseValue1.Except(n).Any());

        var baseValue2 = model.FindDeclaredVocabularyAnnotations(containerThree);
        Assert.DoesNotContain([baseValue2, model.FindVocabularyAnnotations(containerThree)], n => n.Count() != baseValue2.Count() || baseValue2.Except(n).Any());

        var termOneVocabAnnotation = model.FindVocabularyAnnotations(containerOne);
        Assert.Single(termOneVocabAnnotation);

        var containerOneVocabAnnotation = containerOne.VocabularyAnnotations(model);
        Assert.Single(containerOneVocabAnnotation);
        Assert.Equal(containerOneVocabAnnotation.SingleOrDefault(), termOneVocabAnnotation.SingleOrDefault());

        var termThreeVocabAnnotation = model.FindVocabularyAnnotations(containerThree);
        Assert.Single(termThreeVocabAnnotation);

        var containerThreeVocabAnnotation = containerThree.VocabularyAnnotations(model);
        Assert.Single(containerThreeVocabAnnotation);
        Assert.Equal(containerThreeVocabAnnotation.SingleOrDefault(), termThreeVocabAnnotation.SingleOrDefault());
    }

    [Fact]
    public void FindMethodTestPrimitiveTypes()
    {
        var csdls = new string[] { @"
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
</Schema>" };

        var isParsed = SchemaReader.TryParse(csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo)).Select(e => e.CreateReader()), out IEdmModel parsedModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        var constructibleModel = new EdmModel();

        Assert.DoesNotContain(parsedModel.SchemaElements, n => n.Namespace == EdmCoreModel.Namespace);
        Assert.DoesNotContain(constructibleModel.SchemaElements, n => n.Namespace == EdmCoreModel.Namespace);

        foreach (var primitiveType in EdmCoreModel.Instance.SchemaElements)
        {
            Assert.Null(parsedModel.FindDeclaredType(primitiveType.FullName()));
            Assert.Null(constructibleModel.FindDeclaredType(primitiveType.FullName()));
            Assert.NotNull(parsedModel.FindType(primitiveType.FullName()));
            Assert.NotNull(constructibleModel.FindType(primitiveType.FullName()));
            Assert.Equal(EdmCoreModel.Instance.FindDeclaredType(primitiveType.FullName()), primitiveType);
        }
    }

    [Fact]
    public void FindPropertyOnOverriddenDeclaredPropertiesTest()
    {
        var customEdmComplex = new CustomEdmComplexType("", "");
        customEdmComplex.AddStructuralProperty("Property3", EdmCoreModel.Instance.GetInt16(true));

        Assert.NotNull(customEdmComplex.FindProperty("Property3"));
        Assert.Equal(customEdmComplex.FindProperty("Property1").Type.Definition, EdmCoreModel.Instance.SchemaElements.Single(n => n.FullName() == "Edm.Int32") as IEdmType);

        Assert.Equal(customEdmComplex.FindProperty("Property2").Type.Definition, EdmCoreModel.Instance.SchemaElements.Single(n => n.FullName() == "Edm.Int32") as IEdmType);
        Assert.Equal(3, customEdmComplex.DeclaredProperties.Count());
    }

    [Fact]
    public void FindVocabularyAnnotationsIncludingInheritedAnnotationsTest()
    {
        Action<IEnumerable<IEdmVocabularyAnnotation>, IEnumerable<string>> checkAnnotationTerms = (annotations, termNames) =>
        {
            Assert.True(!annotations.Select(n => n.Term.FullName()).Except(termNames).Any() && annotations.Count() == termNames.Count());
        };

        var csdls = new string[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Pet"" BaseType=""foo.Animal"">
        <Property Name=""OwnerId"" Type=""Edm.Int32"" />
        <Property Name=""Name"" Type=""Edm.String"" />
    </ComplexType>
    <Annotations Target=""Unknown.UnresolvedTypeBase"">
        <Annotation Term=""AnnotationNamespace.AddressObject"">
        </Annotation>
    </Annotations>
    <Annotations Target=""DefaultNamespace.Pet"">
        <Annotation Term=""AnnotationNamespace.AddressObject"">
        </Annotation>
    </Annotations>
    <EntityType Name=""Child"" BaseType=""AnnotationNamespace.Person"" />
    <EntityType Name=""UnresolvedType"" BaseType=""Unknown.UnresolvedTypeBase""/>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""AddressObject"" Type=""Edm.Int32"" />
    <Term Name=""Habitation"" Type=""Edm.Int32"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <ComplexType Name=""Animal"" BaseType=""AnnotationNamespace.Life"">
        <Property Name=""Gender"" Type=""Edm.String"" Nullable=""false"" />
    </ComplexType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
    <Annotations Target=""AnnotationNamespace.Animal"">
        <Annotation Term=""AnnotationNamespace.Habitation"" Int=""5"">
        </Annotation>
    </Annotations>
    <Annotations Target=""AnnotationNamespace.Life"">
        <Annotation Term=""AnnotationNamespace.Unknown"" Int=""5"">
        </Annotation>
    </Annotations>
    <Annotations Target=""AnnotationNamespace.Derived"">
        <Annotation Term=""AnnotationNamespace.Unknown"" Int=""5"">
        </Annotation>
    </Annotations>
    <ComplexType Name=""Derived"" BaseType=""AnnotationNamespace.Base"">
    </ComplexType>
    <ComplexType Name=""Base"" BaseType=""AnnotationNamespace.Derived"">
        <Annotation Term=""AnnotationNamespace.Habitation"" Int=""5"">
        </Annotation>
    </ComplexType>
    <ComplexType Name=""ComplexTypeWithEntityTypeBase"" BaseType=""DefaultNamespace.Child"">
        <Annotation Term=""AnnotationNamespace.Habitation"" Int=""5"">
        </Annotation>
    </ComplexType>
</Schema>" };

        var isParsed = SchemaReader.TryParse(csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo)).Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

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
    [Fact]
    public void FindVocabularyAnnotationsIncludingInheritedAnnotationsRecursiveTest()
    {
        Action<IEnumerable<IEdmVocabularyAnnotation>, IEnumerable<string>> checkAnnotationTerms = (annotations, termNames) =>
        {
            Assert.True(!annotations.Select(n => n.Term.FullName()).Except(termNames).Any() && annotations.Count() == termNames.Count());
        };

        var csdls = new string[] { @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Pet"" BaseType=""foo.Animal"">
        <Property Name=""OwnerId"" Type=""Edm.Int32"" />
        <Property Name=""Name"" Type=""Edm.String"" />
    </ComplexType>
    <Annotations Target=""Unknown.UnresolvedTypeBase"">
        <Annotation Term=""AnnotationNamespace.AddressObject"">
        </Annotation>
    </Annotations>
    <Annotations Target=""DefaultNamespace.Pet"">
        <Annotation Term=""AnnotationNamespace.AddressObject"">
        </Annotation>
    </Annotations>
    <EntityType Name=""Child"" BaseType=""AnnotationNamespace.Person"" />
    <EntityType Name=""UnresolvedType"" BaseType=""Unknown.UnresolvedTypeBase""/>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""AddressObject"" Type=""Edm.Int32"" />
    <Term Name=""Habitation"" Type=""Edm.Int32"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <ComplexType Name=""Animal"" BaseType=""AnnotationNamespace.Life"">
        <Property Name=""Gender"" Type=""Edm.String"" Nullable=""false"" />
    </ComplexType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
    <Annotations Target=""AnnotationNamespace.Animal"">
        <Annotation Term=""AnnotationNamespace.Habitation"" Int=""5"">
        </Annotation>
    </Annotations>
    <Annotations Target=""AnnotationNamespace.Life"">
        <Annotation Term=""AnnotationNamespace.Unknown"" Int=""5"">
        </Annotation>
    </Annotations>
    <Annotations Target=""AnnotationNamespace.Derived"">
        <Annotation Term=""AnnotationNamespace.Unknown"" Int=""5"">
        </Annotation>
    </Annotations>
    <ComplexType Name=""Derived"" BaseType=""AnnotationNamespace.Base"">
    </ComplexType>
    <ComplexType Name=""Base"" BaseType=""AnnotationNamespace.Derived"">
        <Annotation Term=""AnnotationNamespace.Habitation"" Int=""5"">
        </Annotation>
    </ComplexType>
    <ComplexType Name=""ComplexTypeWithEntityTypeBase"" BaseType=""DefaultNamespace.Child"">
        <Annotation Term=""AnnotationNamespace.Habitation"" Int=""5"">
        </Annotation>
    </ComplexType>
</Schema>" };

        var isParsed = SchemaReader.TryParse(csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo)).Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

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
    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void FindDerivedTypeReferencedModelTests(EdmVersion edmVersion)
    {
        IEdmModel masterModel = null;
        IEdmModel testModel = null;

        Action<string, Func<IEdmModel, IEdmStructuredType, IEnumerable<IEdmStructuredType>>> verifyFindDerivedTypes = (typeName, findDerivedType) =>
        {
            this.VerifyFindDerivedTypes(masterModel, testModel, typeName, findDerivedType);
        };

        Func<IEdmModel, IEdmStructuredType, IEnumerable<IEdmStructuredType>> runFindDirectlyDerivedTypes = (edmModel, type) => edmModel.FindDirectlyDerivedTypes(type);
        Func<IEdmModel, IEdmStructuredType, IEnumerable<IEdmStructuredType>> runFindAllDerivedTypes = (edmModel, type) => edmModel.FindDirectlyDerivedTypes(type);

        var csdls = new[] {
               @"<Schema Namespace=""FindMethodsTestModelBuilder.MultipleSchemasWithUsings.first"" p1:UseStrongSpatialTypes=""false"" xmlns:p1=""http://schemas.microsoft.com/ado/2009/02/edm/annotation"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""validEntityType1"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
  </EntityType>
  <ComplexType Name=""V1alidcomplexType"">
    <Property Name=""aPropertyOne"" Type=""Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>",
                @"<Schema Namespace=""FindMethodsTestModelBuilder.MultipleSchemasWithUsings.second"" p1:UseStrongSpatialTypes=""false"" xmlns:p1=""http://schemas.microsoft.com/ado/2009/02/edm/annotation"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""VALIDeNTITYtYPE2"" BaseType=""FindMethodsTestModelBuilder.MultipleSchemasWithUsings.first.validEntityType1"" />
  <ComplexType Name=""V1alidcomplexType"">
    <Property Name=""aPropertyOne"" Type=""FindMethodsTestModelBuilder.MultipleSchemasWithUsings.first.V1alidcomplexType"" Nullable=""false"" />
  </ComplexType>
  <EntityContainer Name=""ValidEntityContainer1"" />
</Schema>",
                @"<Schema Namespace=""FindMethodsTestModelBuilder.MultipleSchemasWithUsings.third"" p1:UseStrongSpatialTypes=""false"" xmlns:p1=""http://schemas.microsoft.com/ado/2009/02/edm/annotation"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""VALIDeNTITYtYPE1"" BaseType=""FindMethodsTestModelBuilder.MultipleSchemasWithUsings.second.VALIDeNTITYtYPE2"" />
  <ComplexType Name=""V1alidcomplexType"">
    <Property Name=""aPropertyOne"" Type=""FindMethodsTestModelBuilder.MultipleSchemasWithUsings.second.V1alidcomplexType"" Nullable=""false"" />
  </ComplexType>
</Schema>",
            };

        var csdlElements = csdls.Select(XElement.Parse);
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), out masterModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        _ = SchemaReader.TryParse(new XElement[] { csdlElements.ElementAt(0) }.Select(e => e.CreateReader()), out IEdmModel model0, out errors);
        _ = SchemaReader.TryParse(new XElement[] { csdlElements.ElementAt(1) }.Select(e => e.CreateReader()), out IEdmModel model1, out errors);
        _ = SchemaReader.TryParse(new XElement[] { csdlElements.ElementAt(2) }.Select(e => e.CreateReader()), out IEdmModel model2, out errors);

        _ = SchemaReader.TryParse(new XElement[] { csdlElements.ElementAt(0) }.Select(e => e.CreateReader()), new[] { model1, model2 }, out testModel, out errors);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindAllDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindDirectlyDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindAllDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindDirectlyDerivedTypes);

        _ = SchemaReader.TryParse(new XElement[] { csdlElements.ElementAt(1) }.Select(e => e.CreateReader()), new[] { model0, model2 }, out testModel, out errors);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindAllDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindDirectlyDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindAllDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindDirectlyDerivedTypes);

        _ = SchemaReader.TryParse(new XElement[] { csdlElements.ElementAt(2) }.Select(e => e.CreateReader()), new[] { model0, model1 }, out testModel, out errors);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindAllDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindDirectlyDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindAllDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindDirectlyDerivedTypes);

        _ = SchemaReader.TryParse(new XElement[] { csdlElements.ElementAt(1) }.Select(e => e.CreateReader()), model0, out IEdmModel model3, out errors);
        _ = SchemaReader.TryParse(new XElement[] { csdlElements.ElementAt(2) }.Select(e => e.CreateReader()), model3, out testModel, out errors);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindAllDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first.validEntityType1", runFindDirectlyDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindAllDerivedTypes);
        verifyFindDerivedTypes("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2", runFindDirectlyDerivedTypes);
    }

    //[TestMethod, Variation(Id = 204, SkipReason = @"[EdmLib] FindDirectlyDerivedTypes and FindAllDerivedTypes should throw ArgumentNullException instead of NullReferenceException when their parameters are null -- postponed, [EdmLib] FindDirectlyDerivedTypes and FindAllDerivedTypes should return UnresolvedEntityType or UnresolvedComplexType when it could not resolve the derived types. -- postponed")]
    [Fact]
    public void FindDerivedTypeReferencedModelWithUnresolvedTypesTests()
    {
        var csdls = new[]
            {
               @"<Schema Namespace=""FindMethodsTestModelBuilder.MultipleSchemasWithUsings.first"" p1:UseStrongSpatialTypes=""false"" xmlns:p1=""http://schemas.microsoft.com/ado/2009/02/edm/annotation"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""validEntityType1"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
  </EntityType>
  <ComplexType Name=""V1alidcomplexType"">
    <Property Name=""aPropertyOne"" Type=""Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>",
                @"<Schema Namespace=""FindMethodsTestModelBuilder.MultipleSchemasWithUsings.second"" p1:UseStrongSpatialTypes=""false"" xmlns:p1=""http://schemas.microsoft.com/ado/2009/02/edm/annotation"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""VALIDeNTITYtYPE2"" BaseType=""FindMethodsTestModelBuilder.MultipleSchemasWithUsings.first.validEntityType1"" />
  <ComplexType Name=""V1alidcomplexType"">
    <Property Name=""aPropertyOne"" Type=""FindMethodsTestModelBuilder.MultipleSchemasWithUsings.first.V1alidcomplexType"" Nullable=""false"" />
  </ComplexType>
  <EntityContainer Name=""ValidEntityContainer1"" />
</Schema>",
                @"<Schema Namespace=""FindMethodsTestModelBuilder.MultipleSchemasWithUsings.third"" p1:UseStrongSpatialTypes=""false"" xmlns:p1=""http://schemas.microsoft.com/ado/2009/02/edm/annotation"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""VALIDeNTITYtYPE1"" BaseType=""FindMethodsTestModelBuilder.MultipleSchemasWithUsings.second.VALIDeNTITYtYPE2"" />
  <ComplexType Name=""V1alidcomplexType"">
    <Property Name=""aPropertyOne"" Type=""FindMethodsTestModelBuilder.MultipleSchemasWithUsings.second.V1alidcomplexType"" Nullable=""false"" />
  </ComplexType>
</Schema>",
            }.Select(XElement.Parse);

        var isParsed = SchemaReader.TryParse(new[] { csdls.ElementAt(0), csdls.ElementAt(2) }.Select(e => e.CreateReader()), out IEdmModel testModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        Assert.Throws<ArgumentNullException>(() => testModel.FindAllDerivedTypes(testModel.FindType("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2") as IEdmStructuredType));
        Assert.Throws<ArgumentNullException>(() => testModel.FindDirectlyDerivedTypes(testModel.FindType("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2") as IEdmStructuredType));

        var badUnresolvedError1 = testModel.FindDirectlyDerivedTypes(testModel.FindType("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2") as IEdmStructuredType).Single().Errors().Single();
        var badUnresolvedError2 = testModel.FindAllDerivedTypes(testModel.FindType("FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.second.VALIDeNTITYtYPE2") as IEdmStructuredType).Single().Errors().Single();
        Assert.Equal(EdmErrorCode.BadUnresolvedEntityType, badUnresolvedError1.ErrorCode);
        Assert.Equal(EdmErrorCode.BadUnresolvedEntityType, badUnresolvedError2.ErrorCode);
    }

    private void VerifyFindDerivedTypes(IEdmModel masterModel, IEdmModel testModel, string typeName, Func<IEdmModel, IEdmStructuredType, IEnumerable<IEdmStructuredType>> findDerivedType)
    {
        var expectedResults = findDerivedType(masterModel, masterModel.FindType(typeName) as IEdmStructuredType).Select(n => GetFullName(n));
        var actualResults = findDerivedType(testModel, testModel.FindType(typeName) as IEdmStructuredType).Select(n => GetFullName(n));
        Assert.True(expectedResults.Count() == actualResults.Count() && !expectedResults.Except(actualResults).Any());
    }

    private string GetFullName(IEdmStructuredType type)
    {
        return type as IEdmComplexType != null ? ((IEdmComplexType)type).FullName() : type as IEdmEntityType != null ? ((IEdmEntityType)type).FullName() : null;
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

    private void VerifyFindMethods(IEnumerable<XElement> csdls, IEdmModel edmModel, EdmVersion edmVersion)
    {
        foreach (var csdl in csdls)
        {
            this.VerifyFindSchemaElementMethod(csdl, edmModel, edmVersion);
            this.VerifyFindEntityContainer(csdl, edmModel, edmVersion);
            this.VerifyFindEntityContainerElement(csdl, edmModel, edmVersion);
            this.VerifyFindPropertyMethod(csdl, edmModel, edmVersion);
            this.VerifyFindParameterMethod(csdl, edmModel, edmVersion);
            this.VerifyFindNavigationPropertyMethod(csdl, edmModel, edmVersion);
            this.VerifyFindTypeReferencePropertyMethod(csdl, edmModel, edmVersion);
        }

        this.VerifyFindDerivedType(csdls, edmModel, edmVersion);
    }

    private void VerifyFindDerivedType(IEnumerable<XElement> sourceCsdls, IEdmModel testModel, EdmVersion edmVersion)
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

    private void VerifyFindNavigationPropertyMethod(XElement sourceCsdl, IEdmModel testModel, EdmVersion edmVersion)
    {
        var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(edmVersion);
        Assert.Equal(csdlNamespace, sourceCsdl.Name.Namespace);

        var namespaceValue = sourceCsdl.Attribute("Namespace")?.Value;
        var elementTypes = new string[] { "NavigationProperty" };

        foreach (var elementType in elementTypes)
        {
            var elementTypeFoundList = sourceCsdl.Descendants().Elements(XName.Get(elementType, csdlNamespace.NamespaceName));

            foreach (var elementTypeFound in elementTypeFoundList)
            {
                Assert.Contains(new string[] { "EntityType" }, n => n == elementTypeFound.Parent?.Name.LocalName);

                var entityTypeName = elementTypeFound.Parent?.Attribute("Name")?.Value;
                var entityType = testModel.FindType(namespaceValue + "." + entityTypeName) as IEdmEntityType;
                var entityTypeReference = new EdmEntityTypeReference(entityType, true);

                var navigationFound = entityTypeReference.FindNavigationProperty(elementTypeFound.Attribute("Name")?.Value);

                Assert.NotNull(navigationFound);
            }
        }
    }

    private void VerifyFindPropertyMethod(XElement sourceCsdl, IEdmModel testModel, EdmVersion edmVersion)
    {
        var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(edmVersion);
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

    private void VerifyFindTypeReferencePropertyMethod(XElement sourceCsdl, IEdmModel testModel, EdmVersion edmVersion)
    {
        var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(edmVersion);
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

    private void VerifyFindParameterMethod(XElement sourceCsdl, IEdmModel testModel, EdmVersion edmVersion)
    {
        var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(edmVersion);
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

    private void VerifyFindSchemaElementMethod(XElement sourceCsdl, IEdmModel testModel, EdmVersion edmVersion)
    {
        var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(edmVersion);
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

    private void VerifyFindEntityContainer(XElement sourceCsdl, IEdmModel testModel, EdmVersion edmVersion)
    {
        var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(edmVersion);
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

    protected void VerifyFindEntityContainerElement(XElement sourceCsdl, IEdmModel testModel, EdmVersion edmVersion)
    {
        var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(edmVersion);
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
