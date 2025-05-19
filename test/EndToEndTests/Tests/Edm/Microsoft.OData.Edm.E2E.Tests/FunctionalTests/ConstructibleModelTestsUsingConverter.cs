//---------------------------------------------------------------------
// <copyright file="ConstructibleModelTestsUsingConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class ConstructibleModelTestsUsingConverter : EdmLibTestCaseBase
{
    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterAssociationFkWithNavigationTest(EdmVersion edmVersion)
    {
        // Arrange
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

        IEnumerable<XElement> testCsdl = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).Select(XElement.Parse);
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));

        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterAssociationIndependentTest(EdmVersion edmVersion)
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

        IEnumerable<XElement> testCsdl = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).Select(XElement.Parse);
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));

        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterAssociationOnDeleteModelTest(EdmVersion edmVersion)
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

        IEnumerable<XElement> testCsdl = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).Select(XElement.Parse);
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));

        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterAssociationFkTest(EdmVersion edmVersion)
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

        IEnumerable<XElement> testCsdl = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).Select(XElement.Parse);
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));

        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterAssociationCompositeFkTest(EdmVersion edmVersion)
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

        IEnumerable<XElement> testCsdl = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).Select(XElement.Parse);
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));

        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterEntityContainerWithEntitySetsTest(EdmVersion edmVersion)
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

        IEnumerable<XElement> testCsdl = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).Select(XElement.Parse);
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));

        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterEntityContainerWithFunctionImportsTest(EdmVersion edmVersion)
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

        IEnumerable<XElement> testCsdl = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).Select(XElement.Parse);
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));

        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterEntityInheritanceTreeTest(EdmVersion edmVersion)
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

        IEnumerable<XElement> testCsdl = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).Select(XElement.Parse);
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));

        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterFunctionWithAllTest(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var function = new EdmFunction("NS1", "FunctionWithAll", EdmCoreModel.Instance.GetInt32(true));
        function.AddParameter("Param1", EdmCoreModel.Instance.GetInt32(false));
        model.AddElement(function);

        IEnumerable<XElement> testCsdl = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).Select(XElement.Parse);
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));

        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterModelWithAllConceptsTest(EdmVersion edmVersion)
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

        IEnumerable<XElement> testCsdl = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).Select(XElement.Parse);
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));

        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterMultipleNamespacesTest(EdmVersion edmVersion)
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

        IEnumerable<XElement> testCsdl = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).Select(XElement.Parse);
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));

        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    // [EdmLib] SchemaReader.TryParse uses wrong default values for the string type's facets such as unicode - Fixed
    // [EdmLib] Precision facet should not be required for the temporal and decimal types - Fixed
    public void ConstructibleModelUsingConverterOneComplexWithAllPrimitivePropertyTest(EdmVersion edmVersion)
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

        IEnumerable<XElement> testCsdl = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).Select(XElement.Parse);
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));

        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterOneComplexWithNestedComplexTest(EdmVersion edmVersion)
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

        IEnumerable<XElement> testCsdl = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).Select(XElement.Parse);
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));

        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterOneComplexWithOnePropertyTest(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var type = new EdmComplexType("NS1", "ComplexType");
        type.AddStructuralProperty("Prop1", EdmPrimitiveTypeKind.Int32);
        model.AddElement(type);

        IEnumerable<XElement> testCsdl = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).Select(XElement.Parse);
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));

        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    // [EdmLib] SchemaReader.TryParse uses wrong default values for the string type's facets such as unicode - Fixed
    // [EdmLib] Precision facet should not be required for the temporal and decimal types - Fixed
    public void ConstructibleModelUsingConverterOneEntityWithAllPrimitivePropertyTest(EdmVersion edmVersion)
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

        IEnumerable<XElement> testCsdl = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).Select(XElement.Parse);
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));

        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterOneEntityWithOnePropertyTest(EdmVersion edmVersion)
    {
        var model = new EdmModel();
        var type = new EdmEntityType("NS1", "Person");
        type.AddKeys(type.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        model.AddElement(type);

        IEnumerable<XElement> testCsdl = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).Select(XElement.Parse);
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));

        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterSimpleAllPrimtiveTypesTest(EdmVersion edmVersion)
    {
        var explicitNullable = true;
        var isNullable = false;
        var namespaceName = "ModelBuilder.SimpleAllPrimitiveTypes";

        var model = new EdmModel();
        var entityType = new EdmEntityType(namespaceName, "validEntityType1");
        entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false));
        model.AddElement(entityType);

        var complexType = new EdmComplexType(namespaceName, "V1alidcomplexType");
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
        var tryWriteSchema = model.TryWriteSchema((s) => xmlWriter, out IEnumerable<EdmError> errors);
        xmlWriter.Close();
        Assert.True(tryWriteSchema);
        Assert.Empty(errors);
        
        var csdlElements = new[] { XElement.Parse(stringBuilder.ToString()) };

        if (explicitNullable)
        {
            ModelBuilderHelpers.SetNullableAttributes(csdlElements, isNullable);
        }

        IEnumerable<XElement> testCsdl = this.GetSerializerResult(model, edmVersion, out errors).Select(XElement.Parse);
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));

        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterStringMaxLengthTest(EdmVersion edmVersion)
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

        var edmModel = GetParserResult(new XElement[] { csdlElement });

        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(edmModel);

        var entityType = (EdmEntityType)stockModel.FindType("ModelBuilder.Content");
        var NullableStringUnboundedUnicodeFixedLength = entityType.FindProperty("NullableStringUnboundedUnicode").Type.AsString();
        Assert.True(NullableStringUnboundedUnicodeFixedLength.IsUnbounded);
        Assert.Null(NullableStringUnboundedUnicodeFixedLength.MaxLength);

        var SimpleString = entityType.FindProperty("SimpleString").Type.AsString();
        Assert.False(SimpleString.IsUnbounded);
        Assert.Null(SimpleString.MaxLength);

        var NullableStringMaxLength10 = entityType.FindProperty("NullableStringMaxLength10").Type.AsString();
        Assert.False(NullableStringMaxLength10.IsUnbounded);
        Assert.NotNull(NullableStringMaxLength10.MaxLength);
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterDependentPropertiesWithNoPartnerPropertyTest(EdmVersion edmVersion)
    {
        string csdl = @"<Schema Namespace='DefaultNamespace' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <EntityType Name='Barcode'>
    <Key>
      <PropertyRef Name='Code' />
    </Key>
    <Property Name='Code' Type='Edm.Binary' Nullable='false' MaxLength='50' />
    <Property Name='ProductId' Type='Edm.Int32' Nullable='false' />
    <Property Name='Text' Type='Edm.String' Nullable='false' />
    <NavigationProperty Name='BadScans' Type=""Collection(DefaultNamespace.IncorrectScan)"" Partner=""ExpectedBarcode"" />
  </EntityType>
  <EntityType Name='IncorrectScan'>
    <Key>
      <PropertyRef Name='IncorrectScanId' />
    </Key>
    <Property Name='IncorrectScanId' Type='Edm.Int32' Nullable='false' />
    <Property Name='ExpectedCode' Type='Edm.Binary' Nullable='false' MaxLength='50' />
    <Property Name='ActualCode' Type='Edm.Binary' MaxLength='50' />
    <Property Name='ScanDate' Type='Edm.DateTimeOffset' Nullable='false' />
    <Property Name='Details' Type='Edm.String' Nullable='false' />
    <NavigationProperty Name='ExpectedBarcode' Type=""DefaultNamespace.Barcode"" Nullable=""false"" Partner=""BadScans"">
      <ReferentialConstraint Property=""ExpectedCode"" ReferencedProperty=""Code"" />
    </NavigationProperty>
    <NavigationProperty Name='ActualBarcode' Type=""DefaultNamespace.Barcode"">
      <ReferentialConstraint Property=""ActualCode"" ReferencedProperty=""Code"" />
    </NavigationProperty>
  </EntityType>
</Schema>
";

        var testCsdls = new XElement[] { XElement.Parse(csdl) };
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(testCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));

        this.Compare(testCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterODataTestModelBasicTest(EdmVersion edmVersion)
    {
        var csdl = @"
    <Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""Address"">
    <Property Name=""Street"" Nullable=""true"" Type=""Edm.String"" />
    <Property Name=""Zip"" Nullable=""false"" Type=""Edm.Int32"" />
  </ComplexType>
  <EntityType BaseType=""TestModel.CityType"" Name=""CityOpenType"" OpenType=""true"" />
  <EntityType Name=""CityType"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <NavigationProperty Name=""CityHall"" Type=""Collection(TestModel.OfficeType)"" />
    <NavigationProperty Name=""DOL"" Type=""Collection(TestModel.OfficeType)"" />
    <NavigationProperty Name=""PoliceStation"" Nullable=""false"" Type=""TestModel.OfficeType"" />
    <Property Name=""Id"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""MetroLanes"" Type=""Collection(Edm.String)"" />
    <Property Name=""Name"" Nullable=""true"" Type=""Edm.String"" />
    <Property Name=""Skyline"" Nullable=""false"" Type=""Edm.Stream"" />
  </EntityType>
  <EntityType BaseType=""TestModel.CityType"" Name=""CityWithMapType"" />
  <EntityType BaseType=""TestModel.Person"" Name=""Employee"">
    <Property Name=""CompanyName"" Nullable=""true"" Type=""Edm.String"" />
  </EntityType>
  <EntityType BaseType=""TestModel.Employee"" Name=""Manager"">
    <Property Name=""Level"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""OfficeType"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Address"" Nullable=""false"" Type=""TestModel.Address"" />
    <Property Name=""Id"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <NavigationProperty Name=""Friend"" Type=""Collection(TestModel.Person)"" />
    <Property Name=""Id"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <Action Name=""ServiceOperation1"">
    <Parameter Name=""a"" Nullable=""true"" Type=""Edm.Int32"" />
    <Parameter Name=""b"" Nullable=""true"" Type=""Edm.String"" />
    <ReturnType Type=""Edm.Int32"" />
  </Action>
      <EntityContainer Name=""DefaultContainer"">
        <EntitySet Name=""OfficeType"" EntityType=""TestModel.OfficeType"" />
        <EntitySet Name=""CityType"" EntityType=""TestModel.CityType"">
          <NavigationPropertyBinding Path=""CityHall"" Target=""OfficeType"" />
          <NavigationPropertyBinding Path=""DOL"" Target=""OfficeType"" />
          <NavigationPropertyBinding Path=""PoliceStation"" Target=""OfficeType"" />
        </EntitySet>
        <EntitySet Name=""Person"" EntityType=""TestModel.Person"">
          <NavigationPropertyBinding Path=""Friend"" Target=""Person"" />
        </EntitySet>
        <ActionImport Name=""ServiceOperation1"" Action=""TestModel.ServiceOperation1"" />
      </EntityContainer>
    </Schema>";

        IEnumerable<XElement> testCsdl = (new string[] { csdl }).Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));

        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterODataTestModelAnnotationTestWithAnnotations(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"">
        <EntitySet Name=""PersonType"" EntityType=""TestModel.PersonType"" />
        <EntitySet Name=""People"" EntityType=""TestModel.PersonType"" />
        <ActionImport Name=""ServiceOperation1"" Action=""TestModel.ServiceOperation1"" p3:MimeType=""img/jpeg"" xmlns:p3=""http://docs.oasis-open.org/odata/ns/metadata""/>
    </EntityContainer>
  <ComplexType Name=""Address"">
    <Property Name=""Street"" Nullable=""true"" Type=""Edm.String"" />
    <Property Name=""Zip"" Nullable=""false"" Type=""Edm.Int32"" p3:MimeType=""text/plain"" xmlns:p3=""http://docs.oasis-open.org/odata/ns/metadata"" />
  </ComplexType>
  <EntityType Name=""PersonType"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Address"" Nullable=""false"" Type=""TestModel.Address"" />
    <Property Name=""Id"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Name"" Nullable=""false"" Type=""Edm.String"" p3:MimeType=""text/plain"" xmlns:p3=""http://docs.oasis-open.org/odata/ns/metadata"" />
    <Property Name=""Picture"" Nullable=""false"" Type=""Edm.Stream"" />
  </EntityType>
  <Action Name=""ServiceOperation1"">
    <Parameter Name=""a"" Nullable=""true"" Type=""Edm.Int32"" />
    <Parameter Name=""b"" Nullable=""true"" Type=""Edm.String"" />
    <ReturnType Type=""Edm.Int32"" Nullable=""false"" />
  </Action>
</Schema>";

        IEnumerable<XElement> testCsdl = (new string[] { csdl }).Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));

        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterODataTestModelAnnotationTestWithoutAnnotations(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"">
        <EntitySet EntityType=""TestModel.PersonType"" Name=""People"" />
        <EntitySet EntityType=""TestModel.PersonType"" Name=""PersonType"" />
        <ActionImport Action=""TestModel.ServiceOperation1"" Name=""ServiceOperation1"" />
    </EntityContainer>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""String"" />
        <Property Name=""Zip"" Type=""Int32"" Nullable=""false"" />
    </ComplexType>
    <EntityType Name=""PersonType"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Address"" Type=""TestModel.Address"" Nullable=""false"" />
        <Property Name=""Picture"" Type=""Stream"" Nullable=""false"" />
    </EntityType>
    <Action Name=""ServiceOperation1"">
        <Parameter Name=""a"" Nullable=""true"" Type=""Edm.Int32"" />
        <Parameter Name=""b"" Nullable=""true"" Type=""Edm.String"" />
        <ReturnType Type=""Edm.Int32"" Nullable=""false"" />
    </Action>
</Schema>";

        IEnumerable<XElement> testCsdl = (new string[] { csdl }).Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));

        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterODataTestModelWithFunctionImport(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""TestNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""TestContainer"">
        <ActionImport Action=""TestNS.FunctionImport_Primitive"" Name=""FunctionImport_Primitive"" />
        <ActionImport Action=""TestNS.FunctionImport_PrimitiveCollection"" Name=""FunctionImport_PrimitiveCollection"" />
        <ActionImport Action=""TestNS.FunctionImport_Complex"" Name=""FunctionImport_Complex"" />
        <ActionImport Action=""TestNS.FunctionImport_ComplexCollection"" Name=""FunctionImport_ComplexCollection"" />
        <ActionImport Action=""TestNS.FunctionImport_Entry"" Name=""FunctionImport_Entry"" />
        <ActionImport Action=""TestNS.FunctionImport_Feed"" Name=""FunctionImport_Feed"" />
        <ActionImport Action=""TestNS.FunctionImport_Stream"" Name=""FunctionImport_Stream"" />
        <ActionImport Action=""TestNS.FunctionImport_Enum"" Name=""FunctionImport_Enum"" />
    </EntityContainer>
    <EntityType Name=""EntityType"">
        <Key>
            <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""ComplexProperty"" Type=""TestModel.ComplexType"" Nullable=""false"" />
    </EntityType>
    <EnumType Name=""EnumType"" />
    <Action Name=""FunctionImport_Primitive"">
        <Parameter Name=""primitive"" Nullable=""true"" Type=""Edm.String"" />
    </Action>
    <Action Name=""FunctionImport_PrimitiveCollection"">
        <Parameter Name=""primitiveCollection"" Nullable=""true"" Type=""Collection(Edm.String)"" />
    </Action>
    <Action Name=""FunctionImport_Complex"">
        <Parameter Name=""complex"" Nullable=""true"" Type=""TestModel.ComplexType"" />
    </Action>
    <Action Name=""FunctionImport_ComplexCollection"">
        <Parameter Name=""complexCollection"" Nullable=""true"" Type=""Collection(TestModel.ComplexType)"" />
    </Action>
    <Action Name=""FunctionImport_Entry"">
        <Parameter Name=""entry"" Nullable=""true"" Type=""TestNS.EntityType"" />
    </Action>
    <Action Name=""FunctionImport_Feed"">
        <Parameter Name=""feed"" Nullable=""true"" Type=""Collection(TestNS.EntityType)"" />
    </Action>
    <Action Name=""FunctionImport_Stream"">
        <Parameter Name=""stream"" Nullable=""true"" Type=""Edm.Stream"" />
    </Action>
    <Action Name=""FunctionImport_Enum"">
        <Parameter Name=""enum"" Nullable=""true"" Type=""TestNS.EnumType"" />
    </Action>
</Schema>
<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""ComplexType"">
        <Property Name=""PrimitiveProperty"" Type=""String"" Nullable=""false"" />
        <Property Name=""ComplexProperty"" Type=""TestModel.ComplexType"" Nullable=""false"" />
    </ComplexType>
</Schema>";

        IEnumerable<XElement> testCsdl = (new string[] { csdl }).Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));

        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterODataTestModelDefaultModels(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"">
        <EntitySet EntityType=""DefaultNamespace.Barcode"" Name=""Barcode"">
            <NavigationPropertyBinding Path=""BadScans"" Target=""IncorrectScan"" />
            <NavigationPropertyBinding Path=""GoodScans"" Target=""IncorrectScan"" />
            <NavigationPropertyBinding Path=""Detail"" Target=""BarcodeDetail"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.BarcodeDetail"" Name=""BarcodeDetail"" />
        <EntitySet EntityType=""DefaultNamespace.Car"" Name=""Car"">
            <NavigationPropertyBinding Path=""SpecialEmployee"" Target=""Person"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.Complaint"" Name=""Complaint"">
            <NavigationPropertyBinding Path=""Customer"" Target=""Customer"" />
            <NavigationPropertyBinding Path=""Resolution"" Target=""Resolution"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.Computer"" Name=""Computer"">
            <NavigationPropertyBinding Path=""ComputerDetail"" Target=""ComputerDetail"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.ComputerDetail"" Name=""ComputerDetail"" />
        <EntitySet EntityType=""DefaultNamespace.Customer"" Name=""Customer"">
            <NavigationPropertyBinding Path=""Orders"" Target=""Order"" />
            <NavigationPropertyBinding Path=""Logins"" Target=""Login"" />
            <NavigationPropertyBinding Path=""Wife"" Target=""Customer"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.CustomerInfo"" Name=""CustomerInfo"">
            <NavigationPropertyBinding Path=""Customer"" Target=""Customer"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.Driver"" Name=""Driver"">
            <NavigationPropertyBinding Path=""License"" Target=""License"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.IncorrectScan"" Name=""IncorrectScan"" />
        <EntitySet EntityType=""DefaultNamespace.LastLogin"" Name=""LastLogin"">
            <NavigationPropertyBinding Path=""SmartCard"" Target=""SmartCard"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.License"" Name=""License"" />
        <EntitySet EntityType=""DefaultNamespace.Login"" Name=""Login"">
            <NavigationPropertyBinding Path=""SentMessages"" Target=""Message"" />
            <NavigationPropertyBinding Path=""ReceivedMessages"" Target=""Message"" />
            <NavigationPropertyBinding Path=""Orders"" Target=""Order"" />
            <NavigationPropertyBinding Path=""LastLogin"" Target=""LastLogin"" />
            <NavigationPropertyBinding Path=""SuspiciousActivity"" Target=""SuspiciousActivity"" />
            <NavigationPropertyBinding Path=""RSAToken"" Target=""RSAToken"" />
            <NavigationPropertyBinding Path=""SmartCard"" Target=""SmartCard"" />
            <NavigationPropertyBinding Path=""PasswordResets"" Target=""PasswordReset"" />
            <NavigationPropertyBinding Path=""PageViews"" Target=""PageView"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.MappedEntityType"" Name=""MappedEntityType"" />
        <EntitySet EntityType=""DefaultNamespace.Message"" Name=""Message"" />
        <EntitySet EntityType=""DefaultNamespace.Order"" Name=""Order"">
            <NavigationPropertyBinding Path=""Notes"" Target=""OrderNote"" />
            <NavigationPropertyBinding Path=""OrderQualityCheck"" Target=""OrderQualityCheck"" />
            <NavigationPropertyBinding Path=""OrderLines"" Target=""OrderLine"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.OrderLine"" Name=""OrderLine"" />
        <EntitySet EntityType=""DefaultNamespace.OrderNote"" Name=""OrderNote"" />
        <EntitySet EntityType=""DefaultNamespace.OrderQualityCheck"" Name=""OrderQualityCheck"" />
        <EntitySet EntityType=""DefaultNamespace.PageView"" Name=""PageView"" />
        <EntitySet EntityType=""DefaultNamespace.PasswordReset"" Name=""PasswordReset"" />
        <EntitySet EntityType=""DefaultNamespace.Person"" Name=""Person"">
            <NavigationPropertyBinding Path=""PersonMetadata"" Target=""PersonMetadata"" />
            <NavigationPropertyBinding Path=""DefaultNamespace.Employee/Manager"" Target=""Person"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.PersonMetadata"" Name=""PersonMetadata"" />
        <EntitySet EntityType=""DefaultNamespace.Product"" Name=""Product"">
            <NavigationPropertyBinding Path=""OrderLines"" Target=""OrderLine"" />
            <NavigationPropertyBinding Path=""RelatedProducts"" Target=""Product"" />
            <NavigationPropertyBinding Path=""Detail"" Target=""ProductDetail"" />
            <NavigationPropertyBinding Path=""Reviews"" Target=""ProductReview"" />
            <NavigationPropertyBinding Path=""Photos"" Target=""ProductPhoto"" />
            <NavigationPropertyBinding Path=""Barcodes"" Target=""Barcode"" />
            <NavigationPropertyBinding Path=""Suppliers"" Target=""Supplier"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.ProductDetail"" Name=""ProductDetail"" />
        <EntitySet EntityType=""DefaultNamespace.ProductPhoto"" Name=""ProductPhoto"" />
        <EntitySet EntityType=""DefaultNamespace.ProductReview"" Name=""ProductReview"" />
        <EntitySet EntityType=""DefaultNamespace.ProductWebFeature"" Name=""ProductWebFeature"">
            <NavigationPropertyBinding Path=""Photo"" Target=""ProductPhoto"" />
            <NavigationPropertyBinding Path=""Review"" Target=""ProductReview"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.RSAToken"" Name=""RSAToken"" />
        <EntitySet EntityType=""DefaultNamespace.Resolution"" Name=""Resolution"" />
        <EntitySet EntityType=""DefaultNamespace.SmartCard"" Name=""SmartCard"" />
        <EntitySet EntityType=""DefaultNamespace.Supplier"" Name=""Supplier"">
            <NavigationPropertyBinding Path=""SupplierInfo"" Target=""SupplierInfo"" />
            <NavigationPropertyBinding Path=""Logo"" Target=""SupplierLogo"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.SupplierInfo"" Name=""SupplierInfo"" />
        <EntitySet EntityType=""DefaultNamespace.SupplierLogo"" Name=""SupplierLogo"" />
        <EntitySet EntityType=""DefaultNamespace.SuspiciousActivity"" Name=""SuspiciousActivity"" />
     </EntityContainer>
    
  <ComplexType Name=""Aliases"">
    <Property MaxLength=""10"" Name=""AlternativeNames"" Nullable=""false"" Type=""Collection(Edm.String)"" />
  </ComplexType>
  <ComplexType Name=""AuditInfo"">
    <Property Name=""Concurrency"" Nullable=""false"" Type=""DefaultNamespace.ConcurrencyInfo"" />
    <Property MaxLength=""50"" Name=""ModifiedBy"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""ModifiedDate"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
  </ComplexType>
  <ComplexType Name=""ComplexToCategory"">
    <Property Name=""Label"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""Scheme"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""Term"" Nullable=""false"" Type=""Edm.String"" />
  </ComplexType>
  <ComplexType Name=""ConcurrencyInfo"">
    <Property Name=""QueriedDateTime"" Nullable=""true"" Type=""Edm.DateTimeOffset"" />
    <Property MaxLength=""20"" Name=""Token"" Nullable=""false"" Type=""Edm.String"" />
  </ComplexType>
  <ComplexType Name=""ContactDetails"">
    <Property MaxLength=""10"" Name=""AlternativeNames"" Nullable=""false"" Type=""Collection(Edm.String)"" />
    <Property Name=""ContactAlias"" Nullable=""false"" Type=""DefaultNamespace.Aliases"" />
    <Property MaxLength=""32"" Name=""EmailBag"" Nullable=""false"" Type=""Collection(Edm.String)"" />
    <Property Name=""HomePhone"" Nullable=""false"" Type=""DefaultNamespace.Phone"" />
    <Property Name=""MobilePhoneBag"" Nullable=""false"" Type=""Collection(DefaultNamespace.Phone)"" />
    <Property Name=""WorkPhone"" Nullable=""false"" Type=""DefaultNamespace.Phone"" />
  </ComplexType>
  <ComplexType Name=""Dimensions"">
    <Property Name=""Depth"" Nullable=""false"" Precision=""10"" Scale=""3"" Type=""Edm.Decimal"" />
    <Property Name=""Height"" Nullable=""false"" Precision=""10"" Scale=""3"" Type=""Edm.Decimal"" />
    <Property Name=""Width"" Nullable=""false"" Precision=""10"" Scale=""3"" Type=""Edm.Decimal"" />
  </ComplexType>
  <ComplexType Name=""Phone"">
    <Property MaxLength=""16"" Name=""Extension"" Nullable=""true"" Type=""Edm.String"" />
    <Property MaxLength=""16"" Name=""PhoneNumber"" Nullable=""false"" Type=""Edm.String"" />
  </ComplexType>
  <EntityType BaseType=""DefaultNamespace.OrderLine"" Name=""BackOrderLine"" />
  <EntityType BaseType=""DefaultNamespace.BackOrderLine"" Name=""BackOrderLine2"" />
  <EntityType Name=""Barcode"">
    <Key>
      <PropertyRef Name=""Code"" />
    </Key>
    <NavigationProperty Name=""BadScans"" Partner=""ExpectedBarcode"" Type=""Collection(DefaultNamespace.IncorrectScan)"" />
    <NavigationProperty Name=""Detail"" Partner=""Barcode"" Type=""DefaultNamespace.BarcodeDetail"" />
    <NavigationProperty Name=""GoodScans"" Partner=""ActualBarcode"" Type=""Collection(DefaultNamespace.IncorrectScan)"" />
    <NavigationProperty Name=""Product"" Nullable=""false"" Partner=""Barcodes"" Type=""DefaultNamespace.Product"">
      <ReferentialConstraint Property=""ProductId"" ReferencedProperty=""ProductId"" />
    </NavigationProperty>
    <Property Name=""Code"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""ProductId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Text"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType Name=""BarcodeDetail"">
    <Key>
      <PropertyRef Name=""Code"" />
    </Key>
    <NavigationProperty Name=""Barcode"" Nullable=""false"" Partner=""Detail"" Type=""DefaultNamespace.Barcode"">
      <ReferentialConstraint Property=""Code"" ReferencedProperty=""Code"" />
    </NavigationProperty>
    <Property Name=""Code"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""RegisteredTo"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType Name=""Car"">
    <Key>
      <PropertyRef Name=""VIN"" />
    </Key>
    <NavigationProperty Name=""SpecialEmployee"" Partner=""Car"" Type=""Collection(DefaultNamespace.SpecialEmployee)"" />
    <Property MaxLength=""100"" Name=""Description"" Nullable=""true"" Type=""Edm.String"" />
    <Property Name=""Photo"" Nullable=""false"" Type=""Edm.Stream"" />
    <Property Name=""VIN"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Video"" Nullable=""false"" Type=""Edm.Stream"" />
  </EntityType>
  <EntityType Name=""Complaint"">
    <Key>
      <PropertyRef Name=""ComplaintId"" />
    </Key>
    <NavigationProperty Name=""Customer"" Partner=""Complaints"" Type=""DefaultNamespace.Customer"">
      <ReferentialConstraint Property=""CustomerId"" ReferencedProperty=""CustomerId"" />
    </NavigationProperty>
    <NavigationProperty Name=""Resolution"" Partner=""Complaint"" Type=""DefaultNamespace.Resolution"" />
    <Property Name=""ComplaintId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""CustomerId"" Nullable=""true"" Type=""Edm.Int32"" />
    <Property Name=""Details"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""Logged"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
  </EntityType>
  <EntityType Name=""Computer"">
    <Key>
      <PropertyRef Name=""ComputerId"" />
    </Key>
    <NavigationProperty Name=""ComputerDetail"" Nullable=""false"" Partner=""Computer"" Type=""DefaultNamespace.ComputerDetail"" />
    <Property Name=""ComputerId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Name"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType Name=""ComputerDetail"">
    <Key>
      <PropertyRef Name=""ComputerDetailId"" />
    </Key>
    <NavigationProperty Name=""Computer"" Nullable=""false"" Partner=""ComputerDetail"" Type=""DefaultNamespace.Computer"" />
    <Property Name=""ComputerDetailId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Dimensions"" Nullable=""false"" Type=""DefaultNamespace.Dimensions"" />
    <Property Name=""Manufacturer"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""Model"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""PurchaseDate"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
    <Property Name=""Serial"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""SpecificationsBag"" Nullable=""false"" Type=""Collection(Edm.String)"" />
  </EntityType>
  <EntityType Name=""Customer"">
    <Key>
      <PropertyRef Name=""CustomerId"" />
    </Key>
    <NavigationProperty Name=""Complaints"" Partner=""Customer"" Type=""Collection(DefaultNamespace.Complaint)"" />
    <NavigationProperty Name=""Husband"" Partner=""Wife"" Type=""DefaultNamespace.Customer"" />
    <NavigationProperty Name=""Info"" Partner=""Customer"" Type=""DefaultNamespace.CustomerInfo"" />
    <NavigationProperty Name=""Logins"" Partner=""Customer"" Type=""Collection(DefaultNamespace.Login)"" />
    <NavigationProperty Name=""Orders"" Partner=""Customer"" Type=""Collection(DefaultNamespace.Order)"" />
    <NavigationProperty Name=""Wife"" Partner=""Husband"" Type=""DefaultNamespace.Customer"" />
    <Property Name=""Auditing"" Nullable=""false"" Type=""DefaultNamespace.AuditInfo"" />
    <Property Name=""BackupContactInfo"" Nullable=""false"" Type=""Collection(DefaultNamespace.ContactDetails)"" />
    <Property Name=""CustomerId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property MaxLength=""100"" Name=""Name"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""PrimaryContactInfo"" Nullable=""false"" Type=""DefaultNamespace.ContactDetails"" />
    <Property Name=""Thumbnail"" Nullable=""false"" Type=""Edm.Stream"" />
    <Property Name=""Video"" Nullable=""false"" Type=""Edm.Stream"" />
  </EntityType>
  <EntityType Name=""CustomerInfo"">
    <Key>
      <PropertyRef Name=""CustomerInfoId"" />
    </Key>
    <NavigationProperty Name=""Customer"" Nullable=""false"" Partner=""Info"" Type=""DefaultNamespace.Customer"" />
    <Property Name=""CustomerInfoId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Information"" Nullable=""true"" Type=""Edm.String"" />
  </EntityType>
  <EntityType BaseType=""DefaultNamespace.Product"" Name=""DiscontinuedProduct"">
    <Property Name=""Discontinued"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
    <Property Name=""DiscontinuedPhone"" Nullable=""false"" Type=""DefaultNamespace.Phone"" />
    <Property Name=""ReplacementProductId"" Nullable=""true"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""Driver"">
    <Key>
      <PropertyRef Name=""Name"" />
    </Key>
    <NavigationProperty Name=""License"" Nullable=""false"" Partner=""Driver"" Type=""DefaultNamespace.License"" />
    <Property Name=""BirthDate"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
    <Property MaxLength=""100"" Name=""Name"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType BaseType=""DefaultNamespace.Person"" Name=""Employee"">
    <NavigationProperty Name=""EmployeeToManager"" Partner=""Manager"" Type=""Collection(DefaultNamespace.Employee)"" />
    <NavigationProperty Name=""Manager"" Nullable=""false"" Partner=""EmployeeToManager"" Type=""DefaultNamespace.Employee"">
      <ReferentialConstraint Property=""ManagersPersonId"" ReferencedProperty=""PersonId"" />
    </NavigationProperty>
    <Property Name=""ManagersPersonId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Salary"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Title"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType Name=""IncorrectScan"">
    <Key>
      <PropertyRef Name=""IncorrectScanId"" />
    </Key>
    <NavigationProperty Name=""ActualBarcode"" Partner=""GoodScans"" Type=""DefaultNamespace.Barcode"">
      <ReferentialConstraint Property=""ActualCode"" ReferencedProperty=""Code"" />
    </NavigationProperty>
    <NavigationProperty Name=""ExpectedBarcode"" Nullable=""false"" Partner=""BadScans"" Type=""DefaultNamespace.Barcode"">
      <ReferentialConstraint Property=""ExpectedCode"" ReferencedProperty=""Code"" />
    </NavigationProperty>
    <Property Name=""ActualCode"" Nullable=""true"" Type=""Edm.Int32"" />
    <Property Name=""Details"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""ExpectedCode"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""IncorrectScanId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""ScanDate"" Nullable=""false"" Type=""Collection(Edm.DateTimeOffset)"" />
  </EntityType>
  <EntityType Name=""LastLogin"">
    <Key>
      <PropertyRef Name=""Username"" />
    </Key>
    <NavigationProperty Name=""Login"" Nullable=""false"" Partner=""LastLogin"" Type=""DefaultNamespace.Login"">
      <ReferentialConstraint Property=""Username"" ReferencedProperty=""Username"" />
    </NavigationProperty>
    <NavigationProperty Name=""SmartCard"" Partner=""LastLogin"" Type=""DefaultNamespace.SmartCard"" />
    <Property Name=""LoggedIn"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
    <Property Name=""LoggedOut"" Nullable=""true"" Type=""Edm.DateTimeOffset"" />
    <Property MaxLength=""50"" Name=""Username"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType Name=""License"">
    <Key>
      <PropertyRef Name=""Name"" />
    </Key>
    <NavigationProperty Name=""Driver"" Nullable=""false"" Partner=""License"" Type=""DefaultNamespace.Driver"">
      <ReferentialConstraint Property=""Name"" ReferencedProperty=""Name"" />
    </NavigationProperty>
    <Property Name=""ExpirationDate"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
    <Property Name=""LicenseClass"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""LicenseNumber"" Nullable=""false"" Type=""Edm.String"" />
    <Property MaxLength=""100"" Name=""Name"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""Restrictions"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType Name=""Login"">
    <Key>
      <PropertyRef Name=""Username"" />
    </Key>
    <NavigationProperty Name=""Customer"" Nullable=""false"" Partner=""Logins"" Type=""DefaultNamespace.Customer"">
      <ReferentialConstraint Property=""CustomerId"" ReferencedProperty=""CustomerId"" />
    </NavigationProperty>
    <NavigationProperty Name=""LastLogin"" Partner=""Login"" Type=""DefaultNamespace.LastLogin"" />
    <NavigationProperty Name=""Orders"" Partner=""Login"" Type=""Collection(DefaultNamespace.Order)"" />
    <NavigationProperty Name=""PageViews"" Partner=""Login"" Type=""Collection(DefaultNamespace.PageView)"" />
    <NavigationProperty Name=""PasswordResets"" Partner=""Login"" Type=""Collection(DefaultNamespace.PasswordReset)"" />
    <NavigationProperty Name=""RSAToken"" Partner=""Login"" Type=""DefaultNamespace.RSAToken"" />
    <NavigationProperty Name=""ReceivedMessages"" Partner=""Recipient"" Type=""Collection(DefaultNamespace.Message)"" />
    <NavigationProperty Name=""SentMessages"" Partner=""Sender"" Type=""Collection(DefaultNamespace.Message)"" />
    <NavigationProperty Name=""SmartCard"" Partner=""Login"" Type=""DefaultNamespace.SmartCard"" />
    <NavigationProperty Name=""SuspiciousActivity"" Partner=""Login"" Type=""Collection(DefaultNamespace.SuspiciousActivity)"" />
    <Property Name=""CustomerId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property MaxLength=""50"" Name=""Username"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType Name=""MappedEntityType"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""BagOfBytes"" Nullable=""false"" Type=""Collection(Edm.Byte)"" />
    <Property Name=""BagOfComplexToCategories"" Nullable=""false"" Type=""Collection(DefaultNamespace.ComplexToCategory)"" />
    <Property Name=""BagOfDecimals"" Nullable=""false"" Type=""Collection(Edm.Decimal)"" Scale=""Variable"" />
    <Property Name=""BagOfDoubles"" Nullable=""false"" Type=""Collection(Edm.Double)"" />
    <Property Name=""BagOfGuids"" Nullable=""false"" Type=""Collection(Edm.Guid)"" />
    <Property Name=""BagOfInt16s"" Nullable=""false"" Type=""Collection(Edm.Int16)"" />
    <Property Name=""BagOfInt32s"" Nullable=""false"" Type=""Collection(Edm.Int32)"" />
    <Property Name=""BagOfInt64s"" Nullable=""false"" Type=""Collection(Edm.Int64)"" />
    <Property Name=""BagOfPrimitiveToLinks"" Nullable=""false"" Type=""Collection(Edm.String)"" />
    <Property Name=""BagOfSingles"" Nullable=""false"" Type=""Collection(Edm.Single)"" />
    <Property Name=""Href"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""HrefLang"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""Id"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Length"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Title"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""Type"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType Name=""Message"">
    <Key>
      <PropertyRef Name=""FromUsername"" />
      <PropertyRef Name=""MessageId"" />
    </Key>
    <NavigationProperty Name=""Recipient"" Nullable=""false"" Partner=""ReceivedMessages"" Type=""DefaultNamespace.Login"">
      <ReferentialConstraint Property=""ToUsername"" ReferencedProperty=""Username"" />
    </NavigationProperty>
    <NavigationProperty Name=""Sender"" Nullable=""false"" Partner=""SentMessages"" Type=""DefaultNamespace.Login"">
      <ReferentialConstraint Property=""FromUsername"" ReferencedProperty=""Username"" />
    </NavigationProperty>
    <Property Name=""Body"" Nullable=""true"" Type=""Edm.String"" />
    <Property MaxLength=""50"" Name=""FromUsername"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""IsRead"" Nullable=""false"" Type=""Edm.Boolean"" />
    <Property Name=""MessageId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Sent"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
    <Property Name=""Subject"" Nullable=""false"" Type=""Edm.String"" />
    <Property MaxLength=""50"" Name=""ToUsername"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType Name=""Order"">
    <Key>
      <PropertyRef Name=""OrderId"" />
    </Key>
    <NavigationProperty Name=""Customer"" Partner=""Orders"" Type=""DefaultNamespace.Customer"">
      <ReferentialConstraint Property=""CustomerId"" ReferencedProperty=""CustomerId"" />
    </NavigationProperty>
    <NavigationProperty Name=""Login"" Partner=""Orders"" Type=""DefaultNamespace.Login"" />
    <NavigationProperty Name=""Notes"" Partner=""Order"" Type=""Collection(DefaultNamespace.OrderNote)"">
      <OnDelete Action=""Cascade"" />
    </NavigationProperty>
    <NavigationProperty Name=""OrderLines"" Partner=""Order"" Type=""Collection(DefaultNamespace.OrderLine)"" />
    <NavigationProperty Name=""OrderQualityCheck"" Partner=""Order"" Type=""DefaultNamespace.OrderQualityCheck"" />
    <Property Name=""Concurrency"" Nullable=""false"" Type=""DefaultNamespace.ConcurrencyInfo"" />
    <Property Name=""CustomerId"" Nullable=""true"" Type=""Edm.Int32"" />
    <Property Name=""OrderId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""OrderLine"">
    <Key>
      <PropertyRef Name=""OrderId"" />
      <PropertyRef Name=""ProductId"" />
    </Key>
    <NavigationProperty Name=""Order"" Nullable=""false"" Partner=""OrderLines"" Type=""DefaultNamespace.Order"">
      <ReferentialConstraint Property=""OrderId"" ReferencedProperty=""OrderId"" />
    </NavigationProperty>
    <NavigationProperty Name=""Product"" Nullable=""false"" Partner=""OrderLines"" Type=""DefaultNamespace.Product"">
      <ReferentialConstraint Property=""ProductId"" ReferencedProperty=""ProductId"" />
    </NavigationProperty>
    <Property Name=""ConcurrencyToken"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""OrderId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""OrderLineStream"" Nullable=""false"" Type=""Edm.Stream"" />
    <Property Name=""ProductId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Quantity"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""OrderNote"">
    <Key>
      <PropertyRef Name=""NoteId"" />
    </Key>
    <NavigationProperty Name=""Order"" Nullable=""false"" Partner=""Notes"" Type=""DefaultNamespace.Order"" />
    <Property Name=""Note"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""NoteId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""OrderQualityCheck"">
    <Key>
      <PropertyRef Name=""OrderId"" />
    </Key>
    <NavigationProperty Name=""Order"" Nullable=""false"" Partner=""OrderQualityCheck"" Type=""DefaultNamespace.Order"">
      <ReferentialConstraint Property=""OrderId"" ReferencedProperty=""OrderId"" />
    </NavigationProperty>
    <Property Name=""CheckedBy"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""CheckedDateTime"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
    <Property Name=""OrderId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""PageView"">
    <Key>
      <PropertyRef Name=""PageViewId"" />
    </Key>
    <NavigationProperty Name=""Login"" Nullable=""false"" Partner=""PageViews"" Type=""DefaultNamespace.Login"">
      <ReferentialConstraint Property=""Username"" ReferencedProperty=""Username"" />
    </NavigationProperty>
    <Property MaxLength=""500"" Name=""PageUrl"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""PageViewId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property MaxLength=""50"" Name=""Username"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""Viewed"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
  </EntityType>
  <EntityType Name=""PasswordReset"">
    <Key>
      <PropertyRef Name=""ResetNo"" />
      <PropertyRef Name=""Username"" />
    </Key>
    <NavigationProperty Name=""Login"" Nullable=""false"" Partner=""PasswordResets"" Type=""DefaultNamespace.Login"">
      <ReferentialConstraint Property=""Username"" ReferencedProperty=""Username"" />
    </NavigationProperty>
    <Property Name=""EmailedTo"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""ResetNo"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""TempPassword"" Nullable=""false"" Type=""Edm.String"" />
    <Property MaxLength=""50"" Name=""Username"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""PersonId"" />
    </Key>
    <NavigationProperty Name=""PersonMetadata"" Partner=""Person"" Type=""Collection(DefaultNamespace.PersonMetadata)"" />
    <Property Name=""Name"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""PersonId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""PersonMetadata"">
    <Key>
      <PropertyRef Name=""PersonMetadataId"" />
    </Key>
    <NavigationProperty Name=""Person"" Nullable=""false"" Partner=""PersonMetadata"" Type=""DefaultNamespace.Person"">
      <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""PersonId"" />
    </NavigationProperty>
    <Property Name=""PersonId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""PersonMetadataId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""PropertyName"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""PropertyValue"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType Name=""Product"">
    <Key>
      <PropertyRef Name=""ProductId"" />
    </Key>
    <NavigationProperty Name=""Barcodes"" Partner=""Product"" Type=""Collection(DefaultNamespace.Barcode)"" />
    <NavigationProperty Name=""Detail"" Partner=""Product"" Type=""DefaultNamespace.ProductDetail"" />
    <NavigationProperty Name=""OrderLines"" Partner=""Product"" Type=""Collection(DefaultNamespace.OrderLine)"" />
    <NavigationProperty Name=""Photos"" Partner=""Product"" Type=""Collection(DefaultNamespace.ProductPhoto)"" />
    <NavigationProperty Name=""ProductToRelatedProducts"" Nullable=""false"" Partner=""RelatedProducts"" Type=""DefaultNamespace.Product"" />
    <NavigationProperty Name=""RelatedProducts"" Partner=""ProductToRelatedProducts"" Type=""Collection(DefaultNamespace.Product)"" />
    <NavigationProperty Name=""Reviews"" Partner=""Product"" Type=""Collection(DefaultNamespace.ProductReview)"" />
    <NavigationProperty Name=""Suppliers"" Partner=""Products"" Type=""Collection(DefaultNamespace.Supplier)"" />
    <Property Name=""BaseConcurrency"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""ComplexConcurrency"" Nullable=""false"" Type=""DefaultNamespace.ConcurrencyInfo"" />
    <Property MaxLength=""1000"" Name=""Description"" Nullable=""true"" Type=""Edm.String"" />
    <Property Name=""Dimensions"" Nullable=""false"" Type=""DefaultNamespace.Dimensions"" />
    <Property Name=""NestedComplexConcurrency"" Nullable=""false"" Type=""DefaultNamespace.AuditInfo"" />
    <Property Name=""Picture"" Nullable=""false"" Type=""Edm.Stream"" />
    <Property Name=""ProductId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""ProductDetail"">
    <Key>
      <PropertyRef Name=""ProductId"" />
    </Key>
    <NavigationProperty Name=""Product"" Nullable=""false"" Partner=""Detail"" Type=""DefaultNamespace.Product"">
      <ReferentialConstraint Property=""ProductId"" ReferencedProperty=""ProductId"" />
    </NavigationProperty>
    <Property Name=""Details"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""ProductId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType BaseType=""DefaultNamespace.PageView"" Name=""ProductPageView"">
    <Property Name=""ProductId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""ProductPhoto"">
    <Key>
      <PropertyRef Name=""PhotoId"" />
      <PropertyRef Name=""ProductId"" />
    </Key>
    <NavigationProperty Name=""Features"" Partner=""Photo"" Type=""Collection(DefaultNamespace.ProductWebFeature)"" />
    <NavigationProperty Name=""Product"" Nullable=""false"" Partner=""Photos"" Type=""DefaultNamespace.Product"">
      <ReferentialConstraint Property=""ProductId"" ReferencedProperty=""ProductId"" />
    </NavigationProperty>
    <Property Name=""Photo"" Nullable=""false"" Type=""Edm.Binary"" />
    <Property Name=""PhotoId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""ProductId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""ProductReview"">
    <Key>
      <PropertyRef Name=""ProductId"" />
      <PropertyRef Name=""ReviewId"" />
    </Key>
    <NavigationProperty Name=""Features"" Partner=""Review"" Type=""Collection(DefaultNamespace.ProductWebFeature)"" />
    <NavigationProperty Name=""Product"" Nullable=""false"" Partner=""Reviews"" Type=""DefaultNamespace.Product"">
      <ReferentialConstraint Property=""ProductId"" ReferencedProperty=""ProductId"" />
    </NavigationProperty>
    <Property Name=""ProductId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Review"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""ReviewId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""ProductWebFeature"">
    <Key>
      <PropertyRef Name=""FeatureId"" />
    </Key>
    <NavigationProperty Name=""Photo"" Partner=""Features"" Type=""DefaultNamespace.ProductPhoto"">
      <ReferentialConstraint Property=""PhotoId"" ReferencedProperty=""ProductId"" />
      <ReferentialConstraint Property=""ProductId"" ReferencedProperty=""PhotoId"" />
    </NavigationProperty>
    <NavigationProperty Name=""Review"" Partner=""Features"" Type=""DefaultNamespace.ProductReview"">
      <ReferentialConstraint Property=""ReviewId"" ReferencedProperty=""ProductId"" />
      <ReferentialConstraint Property=""ProductId"" ReferencedProperty=""ReviewId"" />
    </NavigationProperty>
    <Property Name=""FeatureId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Heading"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""PhotoId"" Nullable=""true"" Type=""Edm.Int32"" />
    <Property Name=""ProductId"" Nullable=""true"" Type=""Edm.Int32"" />
    <Property Name=""ReviewId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""RSAToken"">
    <Key>
      <PropertyRef Name=""Serial"" />
    </Key>
    <NavigationProperty Name=""Login"" Nullable=""false"" Partner=""RSAToken"" Type=""DefaultNamespace.Login"" />
    <Property Name=""Issued"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
    <Property MaxLength=""20"" Name=""Serial"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType Name=""Resolution"">
    <Key>
      <PropertyRef Name=""ResolutionId"" />
    </Key>
    <NavigationProperty Name=""Complaint"" Nullable=""false"" Partner=""Resolution"" Type=""DefaultNamespace.Complaint"" />
    <Property Name=""Details"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""ResolutionId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""SmartCard"">
    <Key>
      <PropertyRef Name=""Username"" />
    </Key>
    <NavigationProperty Name=""LastLogin"" Partner=""SmartCard"" Type=""DefaultNamespace.LastLogin"" />
    <NavigationProperty Name=""Login"" Nullable=""false"" Partner=""SmartCard"" Type=""DefaultNamespace.Login"">
      <ReferentialConstraint Property=""Username"" ReferencedProperty=""Username"" />
    </NavigationProperty>
    <Property Name=""CardSerial"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""Issued"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
    <Property MaxLength=""50"" Name=""Username"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType BaseType=""DefaultNamespace.Employee"" Name=""SpecialEmployee"">
    <NavigationProperty Name=""Car"" Nullable=""false"" Partner=""SpecialEmployee"" Type=""DefaultNamespace.Car"">
      <ReferentialConstraint Property=""CarsVIN"" ReferencedProperty=""VIN"" />
    </NavigationProperty>
    <Property Name=""Bonus"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""CarsVIN"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""IsFullyVested"" Nullable=""false"" Type=""Edm.Boolean"" />
  </EntityType>
  <EntityType Name=""Supplier"">
    <Key>
      <PropertyRef Name=""SupplierId"" />
    </Key>
    <NavigationProperty Name=""Logo"" Partner=""Supplier"" Type=""DefaultNamespace.SupplierLogo"" />
    <NavigationProperty Name=""Products"" Partner=""Suppliers"" Type=""Collection(DefaultNamespace.Product)"" />
    <NavigationProperty Name=""SupplierInfo"" Partner=""Supplier"" Type=""Collection(DefaultNamespace.SupplierInfo)"">
      <OnDelete Action=""Cascade"" />
    </NavigationProperty>
    <Property Name=""Name"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""SupplierId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""SupplierInfo"">
    <Key>
      <PropertyRef Name=""SupplierInfoId"" />
    </Key>
    <NavigationProperty Name=""Supplier"" Nullable=""false"" Partner=""SupplierInfo"" Type=""DefaultNamespace.Supplier"" />
    <Property Name=""Information"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""SupplierInfoId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""SupplierLogo"">
    <Key>
      <PropertyRef Name=""SupplierId"" />
    </Key>
    <NavigationProperty Name=""Supplier"" Nullable=""false"" Partner=""Logo"" Type=""DefaultNamespace.Supplier"">
      <ReferentialConstraint Property=""SupplierId"" ReferencedProperty=""SupplierId"" />
    </NavigationProperty>
    <Property MaxLength=""500"" Name=""Logo"" Nullable=""false"" Type=""Edm.Binary"" />
    <Property Name=""SupplierId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""SuspiciousActivity"">
    <Key>
      <PropertyRef Name=""SuspiciousActivityId"" />
    </Key>
    <NavigationProperty Name=""Login"" Partner=""SuspiciousActivity"" Type=""DefaultNamespace.Login"" />
    <Property Name=""Activity"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""SuspiciousActivityId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <Function Name=""EntityProjectionReturnsCollectionOfComplexTypes"">
    <ReturnType Type=""Collection(DefaultNamespace.ContactDetails)"" />
  </Function>
  <Function Name=""GetArgumentPlusOne"">
    <Parameter Name=""arg1"" Nullable=""false"" Type=""Edm.Int32"" />
    <ReturnType Type=""Edm.Int32"" />
  </Function>
  <Function Name=""GetCustomerCount"">
    <ReturnType Type=""Edm.Int32"" />
  </Function>
  <Function Name=""GetPrimitiveString"">
    <ReturnType Type=""Edm.String"" />
  </Function>
  <Function Name=""GetSpecificCustomer"">
    <Parameter Name=""Name"" Nullable=""false"" Type=""Edm.String"" />
    <ReturnType Type=""Collection(DefaultNamespace.Customer)"" />
  </Function>
  <Action Name=""SetArgumentPlusOne"">
    <Parameter Name=""arg1"" Nullable=""false"" Type=""Edm.Int32"" />
    <ReturnType Type=""Edm.Int32"" />
  </Action>
</Schema>";

        IEnumerable<XElement> testCsdl = (new string[] { csdl }).Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        testCsdl = RemoveImmediateAnnotation(testCsdl);
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));

        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterODataTestModelEmptyModel(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"" />
</Schema>";

        IEnumerable<XElement> testCsdl = (new string[] { csdl }).Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));
        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterODataTestModelWithSingleEntityType(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"">
        <EntitySet Name=""SingletonEntityType"" EntityType=""TestModel.SingletonEntityType"" />
    </EntityContainer>
    <EntityType Name=""SingletonEntityType"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""String"" />
    </EntityType>
</Schema>";

        IEnumerable<XElement> testCsdl = (new string[] { csdl }).Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));
        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterODataTestModelWithSingleComplexType(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"" />
    <ComplexType Name=""SingletonComplexType"">
        <Property Name=""City"" Type=""String"" />
    </ComplexType>
</Schema>";

        IEnumerable<XElement> testCsdl = (new string[] { csdl }).Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));
        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterODataTestModelWithCollectionProperty(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"" />
    <ComplexType Name=""EntityTypeWithCollection"">
        <Property Name=""Cities"" Type=""Collection(Edm.String)"" />
    </ComplexType>
</Schema>";

        IEnumerable<XElement> testCsdl = (new string[] { csdl }).Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));
        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterODataTestModelWithOpenType(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"">
        <EntitySet Name=""OpenEntityType"" EntityType=""TestModel.OpenEntityType"" />
    </EntityContainer>
    <EntityType OpenType=""true"" Name=""OpenEntityType"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    </EntityType>
</Schema>";

        IEnumerable<XElement> testCsdl = (new string[] { csdl }).Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));
        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterODataTestModelWithNamedStream(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"">
        <EntitySet Name=""NamedStreamEntityType"" EntityType=""TestModel.NamedStreamEntityType"" />
    </EntityContainer>
    <EntityType Name=""NamedStreamEntityType"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""NamedStream"" Type=""Stream"" Nullable=""false"" />
    </EntityType>
</Schema>";

        IEnumerable<XElement> testCsdl = (new string[] { csdl }).Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));
        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelUsingConverterEntityTypeWithoutKey(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"">
        <EntitySet Name=""EntityTypeWithoutKey"" EntityType=""TestModel.EntityTypeWithoutKey"" />
        <Singleton Name=""SingletonWhoseEntityTypeWithoutKey"" Type=""TestModel.EntityTypeWithoutKey"" />
    </EntityContainer>
    <EntityType Name=""EntityTypeWithoutKey"" />
</Schema>";

        IEnumerable<XElement> testCsdl = (new string[] { csdl }).Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));
        var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n));
        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelODataTestModelBasicTest(EdmVersion edmVersion)
    {
        EdmModel model = new EdmModel();

        EdmComplexType address = new EdmComplexType("TestModel", "Address");
        model.AddElement(address);

        EdmStructuralProperty addressStreet = address.AddStructuralProperty("Street", EdmCoreModel.Instance.GetString(true));
        EdmStructuralProperty addressZip = new EdmStructuralProperty(address, "Zip", EdmCoreModel.Instance.GetInt32(false));
        address.AddProperty(addressZip);

        EdmEntityType office = new EdmEntityType("TestModel", "OfficeType");
        EdmStructuralProperty officeId = office.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        office.AddKeys(officeId);
        EdmStructuralProperty officeAddress = office.AddStructuralProperty("Address", new EdmComplexTypeReference(address, false));
        model.AddElement(office);

        EdmEntityType city = new EdmEntityType("TestModel", "CityType");
        EdmStructuralProperty cityId = city.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        city.AddKeys(cityId);
        EdmStructuralProperty cityName = city.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
        EdmStructuralProperty citySkyline = city.AddStructuralProperty("Skyline", EdmCoreModel.Instance.GetStream(false));
        EdmStructuralProperty cityMetroLanes = city.AddStructuralProperty("MetroLanes", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
        EdmNavigationProperty cityHall = city.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "CityHall", Target = office, TargetMultiplicity = EdmMultiplicity.Many });
        EdmNavigationProperty dol = city.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "DOL", Target = office, TargetMultiplicity = EdmMultiplicity.Many });
        EdmNavigationProperty policeStation = city.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "PoliceStation", Target = office, TargetMultiplicity = EdmMultiplicity.One });
        model.AddElement(city);

        EdmEntityType cityWithMap = new EdmEntityType("TestModel", "CityWithMapType", city);
        model.AddElement(cityWithMap);

        EdmEntityType cityOpenType = new EdmEntityType("TestModel", "CityOpenType", city, false, true);
        model.AddElement(cityOpenType);

        EdmEntityType person = new EdmEntityType("TestModel", "Person");
        EdmStructuralProperty personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        EdmNavigationProperty friend = person.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "Friend", Target = person, TargetMultiplicity = EdmMultiplicity.Many });

        EdmEntityType employee = new EdmEntityType("TestModel", "Employee", person);
        EdmStructuralProperty companyName = employee.AddStructuralProperty("CompanyName", EdmCoreModel.Instance.GetString(true));
        model.AddElement(employee);

        EdmEntityType manager = new EdmEntityType("TestModel", "Manager", employee);
        EdmStructuralProperty level = manager.AddStructuralProperty("Level", EdmCoreModel.Instance.GetInt32(false));
        model.AddElement(manager);

        EdmEntityContainer container = new EdmEntityContainer("TestModel", "DefaultContainer");
        EdmEntitySet officeSet = new EdmEntitySet(container, "OfficeType", office);
        container.AddElement(officeSet);
        EdmEntitySet citySet = container.AddEntitySet("CityType", city);
        EdmEntitySet personSet = container.AddEntitySet("Person", person);

        citySet.AddNavigationTarget(cityHall, officeSet);
        citySet.AddNavigationTarget(dol, officeSet);
        citySet.AddNavigationTarget(policeStation, officeSet);
        personSet.AddNavigationTarget(friend, personSet);

        EdmAction serviceOperationAction = new EdmAction("TestModel", "ServiceOperation1", EdmCoreModel.Instance.GetInt32(true));
        EdmOperationParameter a = new EdmOperationParameter(serviceOperationAction, "a", EdmCoreModel.Instance.GetInt32(true));
        serviceOperationAction.AddParameter(a);
        serviceOperationAction.AddParameter("b", EdmCoreModel.Instance.GetString(true));
        model.AddElement(serviceOperationAction);

        container.AddActionImport("ServiceOperation1", serviceOperationAction);

        model.AddElement(container);

        var csdl = @"
    <Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""Address"">
    <Property Name=""Street"" Nullable=""true"" Type=""Edm.String"" />
    <Property Name=""Zip"" Nullable=""false"" Type=""Edm.Int32"" />
  </ComplexType>
  <EntityType BaseType=""TestModel.CityType"" Name=""CityOpenType"" OpenType=""true"" />
  <EntityType Name=""CityType"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <NavigationProperty Name=""CityHall"" Type=""Collection(TestModel.OfficeType)"" />
    <NavigationProperty Name=""DOL"" Type=""Collection(TestModel.OfficeType)"" />
    <NavigationProperty Name=""PoliceStation"" Nullable=""false"" Type=""TestModel.OfficeType"" />
    <Property Name=""Id"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""MetroLanes"" Type=""Collection(Edm.String)"" />
    <Property Name=""Name"" Nullable=""true"" Type=""Edm.String"" />
    <Property Name=""Skyline"" Nullable=""false"" Type=""Edm.Stream"" />
  </EntityType>
  <EntityType BaseType=""TestModel.CityType"" Name=""CityWithMapType"" />
  <EntityType BaseType=""TestModel.Person"" Name=""Employee"">
    <Property Name=""CompanyName"" Nullable=""true"" Type=""Edm.String"" />
  </EntityType>
  <EntityType BaseType=""TestModel.Employee"" Name=""Manager"">
    <Property Name=""Level"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""OfficeType"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Address"" Nullable=""false"" Type=""TestModel.Address"" />
    <Property Name=""Id"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <NavigationProperty Name=""Friend"" Type=""Collection(TestModel.Person)"" />
    <Property Name=""Id"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <Action Name=""ServiceOperation1"">
    <Parameter Name=""a"" Nullable=""true"" Type=""Edm.Int32"" />
    <Parameter Name=""b"" Nullable=""true"" Type=""Edm.String"" />
    <ReturnType Type=""Edm.Int32"" />
  </Action>
      <EntityContainer Name=""DefaultContainer"">
        <EntitySet Name=""OfficeType"" EntityType=""TestModel.OfficeType"" />
        <EntitySet Name=""CityType"" EntityType=""TestModel.CityType"">
          <NavigationPropertyBinding Path=""CityHall"" Target=""OfficeType"" />
          <NavigationPropertyBinding Path=""DOL"" Target=""OfficeType"" />
          <NavigationPropertyBinding Path=""PoliceStation"" Target=""OfficeType"" />
        </EntitySet>
        <EntitySet Name=""Person"" EntityType=""TestModel.Person"">
          <NavigationPropertyBinding Path=""Friend"" Target=""Person"" />
        </EntitySet>
        <ActionImport Name=""ServiceOperation1"" Action=""TestModel.ServiceOperation1"" />
      </EntityContainer>
    </Schema>";

        IEnumerable<XElement> testCsdl = (new string[] { csdl }).Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var actualXElements = this.GetSerializerResult(model).Select(n => XElement.Parse(n));
        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelODataTestModelAnnotationTestWithAnnotations(EdmVersion edmVersion)
    {
        EdmModel model = new EdmModel();

        EdmComplexType address = new EdmComplexType("TestModel", "Address");
        EdmStructuralProperty addressStreet = address.AddStructuralProperty("Street", EdmCoreModel.Instance.GetString(true));
        EdmStructuralProperty addressZip = address.AddStructuralProperty("Zip", EdmCoreModel.Instance.GetInt32(false));
        model.SetAnnotationValue(addressZip, "http://docs.oasis-open.org/odata/ns/metadata", "MimeType", new EdmStringConstant(EdmCoreModel.Instance.GetString(false), "text/plain"));
        model.AddElement(address);

        EdmEntityType person = new EdmEntityType("TestModel", "PersonType");
        EdmStructuralProperty personName = person.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
        model.SetAnnotationValue(personName, "http://docs.oasis-open.org/odata/ns/metadata", "MimeType", new EdmStringConstant(EdmCoreModel.Instance.GetString(false), "text/plain"));
        EdmStructuralProperty personAddress = person.AddStructuralProperty("Address", new EdmComplexTypeReference(address, false));
        EdmStructuralProperty personPicture = person.AddStructuralProperty("Picture", EdmCoreModel.Instance.GetStream(false));
        EdmStructuralProperty personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        model.AddElement(person);

        EdmEntityContainer container = new EdmEntityContainer("TestModel", "DefaultContainer");
        EdmEntitySet personSet = container.AddEntitySet("PersonType", person);
        EdmEntitySet peopleSet = container.AddEntitySet("People", person);

        EdmAction serviceOperationAction = new EdmAction("TestModel", "ServiceOperation1", EdmCoreModel.Instance.GetInt32(false));
        EdmOperationParameter a = new EdmOperationParameter(serviceOperationAction, "a", EdmCoreModel.Instance.GetInt32(true));
        EdmOperationParameter b = new EdmOperationParameter(serviceOperationAction, "b", EdmCoreModel.Instance.GetString(true));
        serviceOperationAction.AddParameter(a);
        serviceOperationAction.AddParameter(b);
        model.AddElement(serviceOperationAction);
        EdmOperationImport serviceOperation = new EdmActionImport(container, "ServiceOperation1", serviceOperationAction);
        model.SetAnnotationValue(serviceOperation, "http://docs.oasis-open.org/odata/ns/metadata", "MimeType", new EdmStringConstant(EdmCoreModel.Instance.GetString(false), "img/jpeg"));
        container.AddElement(serviceOperation);
        model.AddElement(container);

        var csdl = @"
<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"">
        <EntitySet Name=""PersonType"" EntityType=""TestModel.PersonType"" />
        <EntitySet Name=""People"" EntityType=""TestModel.PersonType"" />
        <ActionImport Name=""ServiceOperation1"" Action=""TestModel.ServiceOperation1"" p3:MimeType=""img/jpeg"" xmlns:p3=""http://docs.oasis-open.org/odata/ns/metadata""/>
    </EntityContainer>
  <ComplexType Name=""Address"">
    <Property Name=""Street"" Nullable=""true"" Type=""Edm.String"" />
    <Property Name=""Zip"" Nullable=""false"" Type=""Edm.Int32"" p3:MimeType=""text/plain"" xmlns:p3=""http://docs.oasis-open.org/odata/ns/metadata"" />
  </ComplexType>
  <EntityType Name=""PersonType"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Address"" Nullable=""false"" Type=""TestModel.Address"" />
    <Property Name=""Id"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Name"" Nullable=""false"" Type=""Edm.String"" p3:MimeType=""text/plain"" xmlns:p3=""http://docs.oasis-open.org/odata/ns/metadata"" />
    <Property Name=""Picture"" Nullable=""false"" Type=""Edm.Stream"" />
  </EntityType>
  <Action Name=""ServiceOperation1"">
    <Parameter Name=""a"" Nullable=""true"" Type=""Edm.Int32"" />
    <Parameter Name=""b"" Nullable=""true"" Type=""Edm.String"" />
    <ReturnType Type=""Edm.Int32"" Nullable=""false"" />
  </Action>
</Schema>";

        IEnumerable<XElement> testCsdl = (new string[] { csdl }).Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var actualXElements = this.GetSerializerResult(model).Select(n => XElement.Parse(n));
        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void TestingDirectVocabularyAnnotationsWithVariousConstantTypes(EdmVersion edmVersion)
    {
        EdmModel model = new EdmModel();

        EdmEntityContainer container = new EdmEntityContainer("TestModel", "DefaultContainer");
        model.SetAnnotationValue(container, "http://foo.bar.com", "foo", new EdmBooleanConstant(EdmCoreModel.Instance.GetBoolean(false), true));
        model.SetAnnotationValue(container, "http://foo.bar.com", "bar", new EdmFloatingConstant(EdmCoreModel.Instance.GetDouble(false), 3.14));
        model.SetAnnotationValue(container, "http://foo.bar.com", "baz", new EdmGuidConstant(EdmCoreModel.Instance.GetGuid(false), new Guid("12345678-1234-1234-1234-123456781234")));
        model.SetAnnotationValue(container, "http://foo.bar.com", "ham", new EdmBinaryConstant(EdmCoreModel.Instance.GetBinary(false), new byte[] { 13, 14, 10, 13, 11, 14, 14, 15 }));
        model.SetAnnotationValue(container, "http://foo.bar.com", "spam", new EdmDecimalConstant(EdmCoreModel.Instance.GetDecimal(false), (decimal)3.50));
        model.SetAnnotationValue(container, "http://foo.bar.com", "spum", new EdmDateTimeOffsetConstant(EdmCoreModel.Instance.GetDateTimeOffset(false), new DateTimeOffset(2001, 1, 1, 5, 40, 00, new TimeSpan(5, 4, 0))));
        model.SetAnnotationValue(container, "http://foo.bar.com", "spork", new EdmDurationConstant(EdmCoreModel.Instance.GetDuration(false), new TimeSpan(5, 4, 3)));

        model.AddElement(container);

        IEnumerable<XElement> testCsdl = new[]{XElement.Parse(
@"<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"" p2:bar=""3.14"" p2:baz=""12345678-1234-1234-1234-123456781234"" p2:foo=""true"" p2:ham=""0D0E0A0D0B0E0E0F"" p2:spam=""3.5"" p2:spork=""PT5H4M3S"" p2:spum=""2001-01-01T05:40:00+05:04"" xmlns:p2=""http://foo.bar.com"" />
</Schema>", LoadOptions.SetLineInfo)};

        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var actualXElements = this.GetSerializerResult(model).Select(n => XElement.Parse(n));
        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelODataTestModelAnnotationTestWithoutAnnotation(EdmVersion edmVersion)
    {
        EdmModel model = new EdmModel();

        EdmComplexType address = new EdmComplexType("TestModel", "Address");
        EdmStructuralProperty addressStreet = address.AddStructuralProperty("Street", EdmCoreModel.Instance.GetString(true));
        EdmStructuralProperty addressZip = address.AddStructuralProperty("Zip", EdmCoreModel.Instance.GetInt32(false));
        model.AddElement(address);

        EdmEntityType person = new EdmEntityType("TestModel", "PersonType");
        EdmStructuralProperty personID = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        EdmStructuralProperty personName = person.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
        EdmStructuralProperty personAddress = person.AddStructuralProperty("Address", new EdmComplexTypeReference(address, false));
        EdmStructuralProperty personPicture = person.AddStructuralProperty("Picture", EdmCoreModel.Instance.GetStream(false));
        person.AddKeys(personID);
        model.AddElement(person);

        EdmEntityContainer container = new EdmEntityContainer("TestModel", "DefaultContainer");
        EdmEntitySet peopleSet = container.AddEntitySet("People", person);
        EdmEntitySet personSet = container.AddEntitySet("PersonType", person);

        EdmAction serviceOperationAction = new EdmAction("TestModel", "ServiceOperation1", EdmCoreModel.Instance.GetInt32(false));
        EdmOperationParameter a = new EdmOperationParameter(serviceOperationAction, "a", EdmCoreModel.Instance.GetInt32(true));
        EdmOperationParameter b = new EdmOperationParameter(serviceOperationAction, "b", EdmCoreModel.Instance.GetString(true));
        serviceOperationAction.AddParameter(a);
        serviceOperationAction.AddParameter(b);
        model.AddElement(serviceOperationAction);
        container.AddActionImport("ServiceOperation1", serviceOperationAction);
        model.AddElement(container);

        var csdl = @"
<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"">
        <EntitySet EntityType=""TestModel.PersonType"" Name=""People"" />
        <EntitySet EntityType=""TestModel.PersonType"" Name=""PersonType"" />
        <ActionImport Action=""TestModel.ServiceOperation1"" Name=""ServiceOperation1"" />
    </EntityContainer>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""String"" />
        <Property Name=""Zip"" Type=""Int32"" Nullable=""false"" />
    </ComplexType>
    <EntityType Name=""PersonType"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Address"" Type=""TestModel.Address"" Nullable=""false"" />
        <Property Name=""Picture"" Type=""Stream"" Nullable=""false"" />
    </EntityType>
    <Action Name=""ServiceOperation1"">
        <Parameter Name=""a"" Nullable=""true"" Type=""Edm.Int32"" />
        <Parameter Name=""b"" Nullable=""true"" Type=""Edm.String"" />
        <ReturnType Type=""Edm.Int32"" Nullable=""false"" />
    </Action>
</Schema>";

        IEnumerable<XElement> testCsdl = (new string[] { csdl }).Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var actualXElements = this.GetSerializerResult(model).Select(n => XElement.Parse(n));
        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelODataTestModelWithFunctionImport(EdmVersion edmVersion)
    {
        EdmModel model = new EdmModel();

        EdmComplexType complexType = new EdmComplexType("TestModel", "ComplexType");
        EdmStructuralProperty primitiveProperty = complexType.AddStructuralProperty("PrimitiveProperty", EdmCoreModel.Instance.GetString(false));
        EdmStructuralProperty complexProperty = complexType.AddStructuralProperty("ComplexProperty", new EdmComplexTypeReference(complexType, false));
        model.AddElement(complexType);

        EdmEntityType entity = new EdmEntityType("TestNS", "EntityType");
        EdmStructuralProperty entityId = entity.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false));
        entity.AddKeys(entityId);
        EdmStructuralProperty entityComplexProperty = entity.AddStructuralProperty("ComplexProperty", new EdmComplexTypeReference(complexType, false));
        model.AddElement(entity);

        EdmEnumType enumType = new EdmEnumType("TestNS", "EnumType");
        model.AddElement(enumType);

        EdmEntityContainer container = new EdmEntityContainer("TestNS", "TestContainer");
        EdmAction primitiveOperationAction = new EdmAction("TestNS", "FunctionImport_Primitive", null);
        EdmOperationParameter primitiveParameter = new EdmOperationParameter(primitiveOperationAction, "primitive", EdmCoreModel.Instance.GetString(true));
        primitiveOperationAction.AddParameter(primitiveParameter);
        model.AddElement(primitiveOperationAction);
        container.AddActionImport("FunctionImport_Primitive", primitiveOperationAction);

        EdmAction primitiveCollectionOperationAction = new EdmAction("TestNS", "FunctionImport_PrimitiveCollection", null);
        EdmOperationParameter primitiveCollectionParameter = new EdmOperationParameter(primitiveCollectionOperationAction, "primitiveCollection", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
        primitiveCollectionOperationAction.AddParameter(primitiveCollectionParameter);
        model.AddElement(primitiveCollectionOperationAction);
        container.AddActionImport("FunctionImport_PrimitiveCollection", primitiveCollectionOperationAction);

        EdmAction complexOperationAction = new EdmAction("TestNS", "FunctionImport_Complex", null);
        EdmOperationParameter complexParameter = new EdmOperationParameter(complexOperationAction, "complex", new EdmComplexTypeReference(complexType, true));
        complexOperationAction.AddParameter(complexParameter);
        model.AddElement(complexOperationAction);
        container.AddActionImport("FunctionImport_Complex", complexOperationAction);

        EdmAction complexCollectionOperationAction = new EdmAction("TestNS", "FunctionImport_ComplexCollection", null);
        EdmOperationParameter complexCollectionParameter = new EdmOperationParameter(complexCollectionOperationAction, "complexCollection", EdmCoreModel.GetCollection(new EdmComplexTypeReference(complexType, true)));
        complexCollectionOperationAction.AddParameter(complexCollectionParameter);
        model.AddElement(complexCollectionOperationAction);
        container.AddActionImport("FunctionImport_ComplexCollection", complexCollectionOperationAction);

        EdmAction entityOperationAction = new EdmAction("TestNS", "FunctionImport_Entry", null);
        EdmOperationParameter entityParameter = new EdmOperationParameter(entityOperationAction, "entry", new EdmEntityTypeReference(entity, true));
        entityOperationAction.AddParameter(entityParameter);
        model.AddElement(entityOperationAction);
        container.AddActionImport("FunctionImport_Entry", entityOperationAction);

        EdmAction feedOperationAction = new EdmAction("TestNS", "FunctionImport_Feed", null);
        EdmOperationParameter feedParameter = new EdmOperationParameter(feedOperationAction, "feed", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entity, true)));
        feedOperationAction.AddParameter(feedParameter);
        model.AddElement(feedOperationAction);
        container.AddActionImport("FunctionImport_Feed", feedOperationAction);

        EdmAction streamOperationAction = new EdmAction("TestNS", "FunctionImport_Stream", null);
        EdmOperationParameter streamParameter = new EdmOperationParameter(streamOperationAction, "stream", EdmCoreModel.Instance.GetStream(true));
        streamOperationAction.AddParameter(streamParameter);
        model.AddElement(streamOperationAction);
        container.AddActionImport("FunctionImport_Stream", streamOperationAction);

        EdmAction enumOperationAction = new EdmAction("TestNS", "FunctionImport_Enum", null);
        EdmOperationParameter enumParameter = new EdmOperationParameter(enumOperationAction, "enum", new EdmEnumTypeReference(enumType, true));
        enumOperationAction.AddParameter(enumParameter);
        model.AddElement(enumOperationAction);
        container.AddActionImport("FunctionImport_Enum", enumOperationAction);

        model.AddElement(container);

        var csdl = @"
<Schema Namespace=""TestNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""TestContainer"">
        <ActionImport Action=""TestNS.FunctionImport_Primitive"" Name=""FunctionImport_Primitive"" />
        <ActionImport Action=""TestNS.FunctionImport_PrimitiveCollection"" Name=""FunctionImport_PrimitiveCollection"" />
        <ActionImport Action=""TestNS.FunctionImport_Complex"" Name=""FunctionImport_Complex"" />
        <ActionImport Action=""TestNS.FunctionImport_ComplexCollection"" Name=""FunctionImport_ComplexCollection"" />
        <ActionImport Action=""TestNS.FunctionImport_Entry"" Name=""FunctionImport_Entry"" />
        <ActionImport Action=""TestNS.FunctionImport_Feed"" Name=""FunctionImport_Feed"" />
        <ActionImport Action=""TestNS.FunctionImport_Stream"" Name=""FunctionImport_Stream"" />
        <ActionImport Action=""TestNS.FunctionImport_Enum"" Name=""FunctionImport_Enum"" />
    </EntityContainer>
    <EntityType Name=""EntityType"">
        <Key>
            <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""ComplexProperty"" Type=""TestModel.ComplexType"" Nullable=""false"" />
    </EntityType>
    <EnumType Name=""EnumType"" />
    <Action Name=""FunctionImport_Primitive"">
        <Parameter Name=""primitive"" Nullable=""true"" Type=""Edm.String"" />
    </Action>
    <Action Name=""FunctionImport_PrimitiveCollection"">
        <Parameter Name=""primitiveCollection"" Nullable=""true"" Type=""Collection(Edm.String)"" />
    </Action>
    <Action Name=""FunctionImport_Complex"">
        <Parameter Name=""complex"" Nullable=""true"" Type=""TestModel.ComplexType"" />
    </Action>
    <Action Name=""FunctionImport_ComplexCollection"">
        <Parameter Name=""complexCollection"" Nullable=""true"" Type=""Collection(TestModel.ComplexType)"" />
    </Action>
    <Action Name=""FunctionImport_Entry"">
        <Parameter Name=""entry"" Nullable=""true"" Type=""TestNS.EntityType"" />
    </Action>
    <Action Name=""FunctionImport_Feed"">
        <Parameter Name=""feed"" Nullable=""true"" Type=""Collection(TestNS.EntityType)"" />
    </Action>
    <Action Name=""FunctionImport_Stream"">
        <Parameter Name=""stream"" Nullable=""true"" Type=""Edm.Stream"" />
    </Action>
    <Action Name=""FunctionImport_Enum"">
        <Parameter Name=""enum"" Nullable=""true"" Type=""TestNS.EnumType"" />
    </Action>
</Schema>
<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""ComplexType"">
        <Property Name=""PrimitiveProperty"" Type=""String"" Nullable=""false"" />
        <Property Name=""ComplexProperty"" Type=""TestModel.ComplexType"" Nullable=""false"" />
    </ComplexType>
</Schema>";

        IEnumerable<XElement> testCsdl = (new string[] { csdl }).Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var actualXElements = this.GetSerializerResult(model).Select(n => XElement.Parse(n));
        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelODataTestModelDefaultModel(EdmVersion edmVersion)
    {
        EdmModel model = new EdmModel();

        EdmComplexType aliases = new EdmComplexType("DefaultNamespace", "Aliases");
        EdmStructuralProperty aliasesAlternativeNames = aliases.AddStructuralProperty("AlternativeNames", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false, 10, null, false)));
        model.AddElement(aliases);

        EdmComplexType phone = new EdmComplexType("DefaultNamespace", "Phone");
        EdmStructuralProperty phoneNumber = phone.AddStructuralProperty("PhoneNumber", EdmCoreModel.Instance.GetString(false, 16, null, false));
        EdmStructuralProperty phoneExtension = phone.AddStructuralProperty("Extension", EdmCoreModel.Instance.GetString(false, 16, null, true));
        model.AddElement(phone);

        EdmComplexType contact = new EdmComplexType("DefaultNamespace", "ContactDetails");
        EdmStructuralProperty contactEmailBag = contact.AddStructuralProperty("EmailBag", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false, 32, null, false)));
        EdmStructuralProperty contactAlternativeNames = contact.AddStructuralProperty("AlternativeNames", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false, 10, null, false)));
        EdmStructuralProperty contactAlias = contact.AddStructuralProperty("ContactAlias", new EdmComplexTypeReference(aliases, false));
        EdmStructuralProperty contactHomePhone = contact.AddStructuralProperty("HomePhone", new EdmComplexTypeReference(phone, false));
        EdmStructuralProperty contactWorkPhone = contact.AddStructuralProperty("WorkPhone", new EdmComplexTypeReference(phone, false));
        EdmStructuralProperty contactMobilePhoneBag = contact.AddStructuralProperty("MobilePhoneBag", EdmCoreModel.GetCollection(new EdmComplexTypeReference(phone, false)));
        model.AddElement(contact);

        EdmComplexType category = new EdmComplexType("DefaultNamespace", "ComplexToCategory");
        EdmStructuralProperty categoryTerm = category.AddStructuralProperty("Term", EdmCoreModel.Instance.GetString(false));
        EdmStructuralProperty categoryScheme = category.AddStructuralProperty("Scheme", EdmCoreModel.Instance.GetString(false));
        EdmStructuralProperty categoryLabel = category.AddStructuralProperty("Label", EdmCoreModel.Instance.GetString(false));
        model.AddElement(category);

        EdmComplexType dimensions = new EdmComplexType("DefaultNamespace", "Dimensions");
        EdmStructuralProperty dimensionsWidth = dimensions.AddStructuralProperty("Width", EdmCoreModel.Instance.GetDecimal(10, 3, false));
        EdmStructuralProperty dimensionsHeight = dimensions.AddStructuralProperty("Height", EdmCoreModel.Instance.GetDecimal(10, 3, false));
        EdmStructuralProperty dimensionsDepth = dimensions.AddStructuralProperty("Depth", EdmCoreModel.Instance.GetDecimal(10, 3, false));
        model.AddElement(dimensions);

        EdmComplexType concurrency = new EdmComplexType("DefaultNamespace", "ConcurrencyInfo");
        EdmStructuralProperty concurrencyToken = concurrency.AddStructuralProperty("Token", EdmCoreModel.Instance.GetString(false, 20, null, false));
        EdmStructuralProperty concurrencyQueriedDateTime = concurrency.AddStructuralProperty("QueriedDateTime", EdmCoreModel.Instance.GetDateTimeOffset(true));
        model.AddElement(concurrency);

        EdmComplexType audit = new EdmComplexType("DefaultNamespace", "AuditInfo");
        EdmStructuralProperty auditModifiedDate = audit.AddStructuralProperty("ModifiedDate", EdmCoreModel.Instance.GetDateTimeOffset(false));
        EdmStructuralProperty auditModifiedBy = audit.AddStructuralProperty("ModifiedBy", EdmCoreModel.Instance.GetString(false, 50, null, false));
        EdmStructuralProperty auditConcurrency = audit.AddStructuralProperty("Concurrency", new EdmComplexTypeReference(concurrency, false));
        model.AddElement(audit);

        EdmEntityType customer = new EdmEntityType("DefaultNamespace", "Customer");
        EdmStructuralProperty customerId = customer.AddStructuralProperty("CustomerId", EdmCoreModel.Instance.GetInt32(false));
        customer.AddKeys(customerId);
        EdmStructuralProperty customerName = customer.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false, 100, null, false));
        EdmStructuralProperty customerPrimaryContact = customer.AddStructuralProperty("PrimaryContactInfo", new EdmComplexTypeReference(contact, false));
        EdmStructuralProperty customerBackupContact = customer.AddStructuralProperty("BackupContactInfo", EdmCoreModel.GetCollection(new EdmComplexTypeReference(contact, false)));
        EdmStructuralProperty customerAuditing = customer.AddStructuralProperty("Auditing", new EdmComplexTypeReference(audit, false));
        EdmStructuralProperty customerThumbnail = customer.AddStructuralProperty("Thumbnail", EdmCoreModel.Instance.GetStream(false));
        EdmStructuralProperty customerVideo = customer.AddStructuralProperty("Video", EdmCoreModel.Instance.GetStream(false));
        model.AddElement(customer);

        EdmEntityType barcode = new EdmEntityType("DefaultNamespace", "Barcode");
        EdmStructuralProperty barcodeCode = barcode.AddStructuralProperty("Code", EdmCoreModel.Instance.GetInt32(false));
        barcode.AddKeys(barcodeCode);
        EdmStructuralProperty barcodeProductId = barcode.AddStructuralProperty("ProductId", EdmCoreModel.Instance.GetInt32(false));
        EdmStructuralProperty barcodeText = barcode.AddStructuralProperty("Text", EdmCoreModel.Instance.GetString(false));
        model.AddElement(barcode);

        EdmEntityType incorrectScan = new EdmEntityType("DefaultNamespace", "IncorrectScan");
        EdmStructuralProperty incorrectScanId = incorrectScan.AddStructuralProperty("IncorrectScanId", EdmCoreModel.Instance.GetInt32(false));
        incorrectScan.AddKeys(incorrectScanId);
        EdmStructuralProperty incorrectScanExpectedCode = incorrectScan.AddStructuralProperty("ExpectedCode", EdmCoreModel.Instance.GetInt32(false));
        EdmStructuralProperty incorrectScanActualCode = incorrectScan.AddStructuralProperty("ActualCode", EdmCoreModel.Instance.GetInt32(true));
        EdmStructuralProperty incorrectScanDate = incorrectScan.AddStructuralProperty("ScanDate", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetDateTimeOffset(false)));
        EdmStructuralProperty incorrectScanDetails = incorrectScan.AddStructuralProperty("Details", EdmCoreModel.Instance.GetString(false));
        model.AddElement(incorrectScan);

        EdmEntityType barcodeDetail = new EdmEntityType("DefaultNamespace", "BarcodeDetail");
        EdmStructuralProperty barcodeDetailCode = barcodeDetail.AddStructuralProperty("Code", EdmCoreModel.Instance.GetInt32(false));
        barcodeDetail.AddKeys(barcodeDetailCode);
        EdmStructuralProperty barcodeDetailRegisteredTo = barcodeDetail.AddStructuralProperty("RegisteredTo", EdmCoreModel.Instance.GetString(false));
        model.AddElement(barcodeDetail);

        EdmEntityType complaint = new EdmEntityType("DefaultNamespace", "Complaint");
        EdmStructuralProperty complaintId = complaint.AddStructuralProperty("ComplaintId", EdmCoreModel.Instance.GetInt32(false));
        complaint.AddKeys(complaintId);
        EdmStructuralProperty complaintCustomerId = complaint.AddStructuralProperty("CustomerId", EdmCoreModel.Instance.GetInt32(true));
        EdmStructuralProperty complaintLogged = complaint.AddStructuralProperty("Logged", EdmCoreModel.Instance.GetDateTimeOffset(false));
        EdmStructuralProperty complaintDetails = complaint.AddStructuralProperty("Details", EdmCoreModel.Instance.GetString(false));
        model.AddElement(complaint);

        EdmEntityType resolution = new EdmEntityType("DefaultNamespace", "Resolution");
        EdmStructuralProperty resolutionId = resolution.AddStructuralProperty("ResolutionId", EdmCoreModel.Instance.GetInt32(false));
        resolution.AddKeys(resolutionId);
        EdmStructuralProperty resolutionDetails = resolution.AddStructuralProperty("Details", EdmCoreModel.Instance.GetString(false));
        model.AddElement(resolution);

        EdmEntityType login = new EdmEntityType("DefaultNamespace", "Login");
        EdmStructuralProperty loginUsername = login.AddStructuralProperty("Username", EdmCoreModel.Instance.GetString(false, 50, null, false));
        login.AddKeys(loginUsername);
        EdmStructuralProperty loginCustomerId = login.AddStructuralProperty("CustomerId", EdmCoreModel.Instance.GetInt32(false));
        model.AddElement(login);

        EdmEntityType suspiciousActivity = new EdmEntityType("DefaultNamespace", "SuspiciousActivity");
        EdmStructuralProperty suspiciousActivityId = suspiciousActivity.AddStructuralProperty("SuspiciousActivityId", EdmCoreModel.Instance.GetInt32(false));
        suspiciousActivity.AddKeys(suspiciousActivityId);
        EdmStructuralProperty suspiciousActivityProperty = suspiciousActivity.AddStructuralProperty("Activity", EdmCoreModel.Instance.GetString(false));
        model.AddElement(suspiciousActivity);

        EdmEntityType smartCard = new EdmEntityType("DefaultNamespace", "SmartCard");
        EdmStructuralProperty smartCardUsername = smartCard.AddStructuralProperty("Username", EdmCoreModel.Instance.GetString(false, 50, null, false));
        smartCard.AddKeys(smartCardUsername);
        EdmStructuralProperty smartCardSerial = smartCard.AddStructuralProperty("CardSerial", EdmCoreModel.Instance.GetString(false));
        EdmStructuralProperty smartCardIssued = smartCard.AddStructuralProperty("Issued", EdmCoreModel.Instance.GetDateTimeOffset(false));
        model.AddElement(smartCard);

        EdmEntityType rsaToken = new EdmEntityType("DefaultNamespace", "RSAToken");
        EdmStructuralProperty rsaTokenSerial = rsaToken.AddStructuralProperty("Serial", EdmCoreModel.Instance.GetString(false, 20, null, false));
        rsaToken.AddKeys(rsaTokenSerial);
        EdmStructuralProperty rsaTokenIssued = rsaToken.AddStructuralProperty("Issued", EdmCoreModel.Instance.GetDateTimeOffset(false));
        model.AddElement(rsaToken);

        EdmEntityType passwordReset = new EdmEntityType("DefaultNamespace", "PasswordReset");
        EdmStructuralProperty passwordResetNo = passwordReset.AddStructuralProperty("ResetNo", EdmCoreModel.Instance.GetInt32(false));
        EdmStructuralProperty passwordResetUsername = passwordReset.AddStructuralProperty("Username", EdmCoreModel.Instance.GetString(false, 50, null, false));
        passwordReset.AddKeys(passwordResetNo);
        passwordReset.AddKeys(passwordResetUsername);
        EdmStructuralProperty passwordResetTempPassword = passwordReset.AddStructuralProperty("TempPassword", EdmCoreModel.Instance.GetString(false));
        EdmStructuralProperty passwordResetEmailedTo = passwordReset.AddStructuralProperty("EmailedTo", EdmCoreModel.Instance.GetString(false));
        model.AddElement(passwordReset);

        EdmEntityType pageView = new EdmEntityType("DefaultNamespace", "PageView");
        EdmStructuralProperty pageViewId = pageView.AddStructuralProperty("PageViewId", EdmCoreModel.Instance.GetInt32(false));
        pageView.AddKeys(pageViewId);
        EdmStructuralProperty pageViewUsername = pageView.AddStructuralProperty("Username", EdmCoreModel.Instance.GetString(false, 50, null, false));
        EdmStructuralProperty pageViewed = pageView.AddStructuralProperty("Viewed", EdmCoreModel.Instance.GetDateTimeOffset(false));
        EdmStructuralProperty pageViewPageUrl = pageView.AddStructuralProperty("PageUrl", EdmCoreModel.Instance.GetString(false, 500, null, false));
        model.AddElement(pageView);

        EdmEntityType productPageView = new EdmEntityType("DefaultNamespace", "ProductPageView", pageView);
        EdmStructuralProperty productPageViewId = productPageView.AddStructuralProperty("ProductId", EdmCoreModel.Instance.GetInt32(false));
        model.AddElement(productPageView);

        EdmEntityType lastLogin = new EdmEntityType("DefaultNamespace", "LastLogin");
        EdmStructuralProperty lastLoginUsername = lastLogin.AddStructuralProperty("Username", EdmCoreModel.Instance.GetString(false, 50, null, false));
        lastLogin.AddKeys(lastLoginUsername);
        EdmStructuralProperty lastLoginLoggedIn = lastLogin.AddStructuralProperty("LoggedIn", EdmCoreModel.Instance.GetDateTimeOffset(false));
        EdmStructuralProperty lastLoginLoggedOut = lastLogin.AddStructuralProperty("LoggedOut", EdmCoreModel.Instance.GetDateTimeOffset(true));
        model.AddElement(lastLogin);

        EdmEntityType message = new EdmEntityType("DefaultNamespace", "Message");
        EdmStructuralProperty messageId = message.AddStructuralProperty("MessageId", EdmCoreModel.Instance.GetInt32(false));
        EdmStructuralProperty messageFromUsername = message.AddStructuralProperty("FromUsername", EdmCoreModel.Instance.GetString(false, 50, null, false));
        message.AddKeys(messageId);
        message.AddKeys(messageFromUsername);
        EdmStructuralProperty messageToUsername = message.AddStructuralProperty("ToUsername", EdmCoreModel.Instance.GetString(false, 50, null, false));
        EdmStructuralProperty messageSent = message.AddStructuralProperty("Sent", EdmCoreModel.Instance.GetDateTimeOffset(false));
        EdmStructuralProperty messageSubject = message.AddStructuralProperty("Subject", EdmCoreModel.Instance.GetString(false));
        EdmStructuralProperty messageBody = message.AddStructuralProperty("Body", EdmCoreModel.Instance.GetString(true));
        EdmStructuralProperty messageIsRead = message.AddStructuralProperty("IsRead", EdmCoreModel.Instance.GetBoolean(false));
        model.AddElement(message);

        EdmEntityType order = new EdmEntityType("DefaultNamespace", "Order");
        EdmStructuralProperty orderId = order.AddStructuralProperty("OrderId", EdmCoreModel.Instance.GetInt32(false));
        order.AddKeys(orderId);
        EdmStructuralProperty orderCustomerId = order.AddStructuralProperty("CustomerId", EdmCoreModel.Instance.GetInt32(true));
        EdmStructuralProperty orderConcurrency = order.AddStructuralProperty("Concurrency", new EdmComplexTypeReference(concurrency, false));
        model.AddElement(order);

        EdmEntityType orderNote = new EdmEntityType("DefaultNamespace", "OrderNote");
        EdmStructuralProperty orderNoteId = orderNote.AddStructuralProperty("NoteId", EdmCoreModel.Instance.GetInt32(false));
        orderNote.AddKeys(orderNoteId);
        EdmStructuralProperty orderNoteDescription = orderNote.AddStructuralProperty("Note", EdmCoreModel.Instance.GetString(false));
        model.AddElement(orderNote);

        EdmEntityType orderQualityCheck = new EdmEntityType("DefaultNamespace", "OrderQualityCheck");
        EdmStructuralProperty orderQualityCheckId = orderQualityCheck.AddStructuralProperty("OrderId", EdmCoreModel.Instance.GetInt32(false));
        orderQualityCheck.AddKeys(orderQualityCheckId);
        EdmStructuralProperty orderQualityCheckBy = orderQualityCheck.AddStructuralProperty("CheckedBy", EdmCoreModel.Instance.GetString(false));
        EdmStructuralProperty orderQualityCheckDateTime = orderQualityCheck.AddStructuralProperty("CheckedDateTime", EdmCoreModel.Instance.GetDateTimeOffset(false));
        model.AddElement(orderQualityCheck);

        EdmEntityType orderLine = new EdmEntityType("DefaultNamespace", "OrderLine");
        EdmStructuralProperty orderLineOrderId = orderLine.AddStructuralProperty("OrderId", EdmCoreModel.Instance.GetInt32(false));
        EdmStructuralProperty orderLineProductId = orderLine.AddStructuralProperty("ProductId", EdmCoreModel.Instance.GetInt32(false));
        orderLine.AddKeys(orderLineOrderId);
        orderLine.AddKeys(orderLineProductId);
        EdmStructuralProperty orderLineQuantity = orderLine.AddStructuralProperty("Quantity", EdmCoreModel.Instance.GetInt32(false));
        EdmStructuralProperty orderLineConcurrencyToken = orderLine.AddStructuralProperty("ConcurrencyToken", EdmCoreModel.Instance.GetString(false), null);
        EdmStructuralProperty orderLineStream = orderLine.AddStructuralProperty("OrderLineStream", EdmCoreModel.Instance.GetStream(false));
        model.AddElement(orderLine);

        EdmEntityType backOrderLine = new EdmEntityType("DefaultNamespace", "BackOrderLine", orderLine);
        model.AddElement(backOrderLine);
        EdmEntityType backOrderLine2 = new EdmEntityType("DefaultNamespace", "BackOrderLine2", backOrderLine);
        model.AddElement(backOrderLine2);

        EdmEntityType product = new EdmEntityType("DefaultNamespace", "Product");
        EdmStructuralProperty productId = product.AddStructuralProperty("ProductId", EdmCoreModel.Instance.GetInt32(false));
        product.AddKeys(productId);
        EdmStructuralProperty productDescription = product.AddStructuralProperty("Description", EdmCoreModel.Instance.GetString(false, 1000, true, true));
        EdmStructuralProperty productDimensions = product.AddStructuralProperty("Dimensions", new EdmComplexTypeReference(dimensions, false));
        EdmStructuralProperty productBaseConcurrency = new EdmStructuralProperty(product, "BaseConcurrency", EdmCoreModel.Instance.GetString(false), null);
        product.AddProperty(productBaseConcurrency);
        EdmStructuralProperty productComplexConcurrency = product.AddStructuralProperty("ComplexConcurrency", new EdmComplexTypeReference(concurrency, false));
        EdmStructuralProperty productNestedComplexConcurrency = product.AddStructuralProperty("NestedComplexConcurrency", new EdmComplexTypeReference(audit, false));
        EdmStructuralProperty productPicture = product.AddStructuralProperty("Picture", EdmCoreModel.Instance.GetStream(false));
        model.AddElement(product);

        EdmEntityType discontinuedProduct = new EdmEntityType("DefaultNamespace", "DiscontinuedProduct", product);
        EdmStructuralProperty discontinuedProductDate = discontinuedProduct.AddStructuralProperty("Discontinued", EdmCoreModel.Instance.GetDateTimeOffset(false));
        EdmStructuralProperty discontinuedProductReplacementProductId = discontinuedProduct.AddStructuralProperty("ReplacementProductId", EdmCoreModel.Instance.GetInt32(true));
        EdmStructuralProperty discontinuedProductDiscontinuedPhone = discontinuedProduct.AddStructuralProperty("DiscontinuedPhone", new EdmComplexTypeReference(phone, false));
        model.AddElement(discontinuedProduct);

        EdmEntityType productDetail = new EdmEntityType("DefaultNamespace", "ProductDetail");
        EdmStructuralProperty productDetailProductId = productDetail.AddStructuralProperty("ProductId", EdmCoreModel.Instance.GetInt32(false));
        productDetail.AddKeys(productDetailProductId);
        EdmStructuralProperty productDetails = productDetail.AddStructuralProperty("Details", EdmCoreModel.Instance.GetString(false));
        model.AddElement(productDetail);

        EdmEntityType productReview = new EdmEntityType("DefaultNamespace", "ProductReview");
        EdmStructuralProperty productReviewProductId = productReview.AddStructuralProperty("ProductId", EdmCoreModel.Instance.GetInt32(false));
        EdmStructuralProperty productReviewId = productReview.AddStructuralProperty("ReviewId", EdmCoreModel.Instance.GetInt32(false));
        productReview.AddKeys(productReviewProductId);
        productReview.AddKeys(productReviewId);
        EdmStructuralProperty productReviewDescription = productReview.AddStructuralProperty("Review", EdmCoreModel.Instance.GetString(false));
        model.AddElement(productReview);

        EdmEntityType productPhoto = new EdmEntityType("DefaultNamespace", "ProductPhoto");
        EdmStructuralProperty productPhotoProductId = productPhoto.AddStructuralProperty("ProductId", EdmCoreModel.Instance.GetInt32(false));
        EdmStructuralProperty productPhotoId = productPhoto.AddStructuralProperty("PhotoId", EdmCoreModel.Instance.GetInt32(false));
        productPhoto.AddKeys(productPhotoProductId);
        productPhoto.AddKeys(productPhotoId);
        EdmStructuralProperty productPhotoBinary = productPhoto.AddStructuralProperty("Photo", EdmCoreModel.Instance.GetBinary(false));
        model.AddElement(productPhoto);

        EdmEntityType productWebFeature = new EdmEntityType("DefaultNamespace", "ProductWebFeature");
        EdmStructuralProperty productWebFeatureId = productWebFeature.AddStructuralProperty("FeatureId", EdmCoreModel.Instance.GetInt32(false));
        productWebFeature.AddKeys(productWebFeatureId);
        EdmStructuralProperty productWebFeatureProductId = productWebFeature.AddStructuralProperty("ProductId", EdmCoreModel.Instance.GetInt32(true));
        EdmStructuralProperty productWebFeaturePhotoId = productWebFeature.AddStructuralProperty("PhotoId", EdmCoreModel.Instance.GetInt32(true));
        EdmStructuralProperty productWebFeatureReviewId = productWebFeature.AddStructuralProperty("ReviewId", EdmCoreModel.Instance.GetInt32(false));
        EdmStructuralProperty productWebFeatureHeading = productWebFeature.AddStructuralProperty("Heading", EdmCoreModel.Instance.GetString(false));
        model.AddElement(productWebFeature);

        EdmEntityType supplier = new EdmEntityType("DefaultNamespace", "Supplier");
        EdmStructuralProperty supplierId = supplier.AddStructuralProperty("SupplierId", EdmCoreModel.Instance.GetInt32(false));
        supplier.AddKeys(supplierId);
        EdmStructuralProperty supplierName = supplier.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
        model.AddElement(supplier);

        EdmEntityType supplierLogo = new EdmEntityType("DefaultNamespace", "SupplierLogo");
        EdmStructuralProperty supplierLogoSupplierId = supplierLogo.AddStructuralProperty("SupplierId", EdmCoreModel.Instance.GetInt32(false));
        supplierLogo.AddKeys(supplierLogoSupplierId);
        EdmStructuralProperty supplierLogoBinary = supplierLogo.AddStructuralProperty("Logo", EdmCoreModel.Instance.GetBinary(false, 500, false));
        model.AddElement(supplierLogo);

        EdmEntityType supplierInfo = new EdmEntityType("DefaultNamespace", "SupplierInfo");
        EdmStructuralProperty supplierInfoId = supplierInfo.AddStructuralProperty("SupplierInfoId", EdmCoreModel.Instance.GetInt32(false));
        supplierInfo.AddKeys(supplierInfoId);
        EdmStructuralProperty supplierInfoDescription = supplierInfo.AddStructuralProperty("Information", EdmCoreModel.Instance.GetString(false));
        model.AddElement(supplierInfo);

        EdmEntityType customerInfo = new EdmEntityType("DefaultNamespace", "CustomerInfo");
        EdmStructuralProperty customerInfoId = customerInfo.AddStructuralProperty("CustomerInfoId", EdmCoreModel.Instance.GetInt32(false));
        customerInfo.AddKeys(customerInfoId);
        EdmStructuralProperty customerInfoDescription = customerInfo.AddStructuralProperty("Information", EdmCoreModel.Instance.GetString(true));
        model.AddElement(customerInfo);

        EdmEntityType computer = new EdmEntityType("DefaultNamespace", "Computer");
        EdmStructuralProperty computerId = computer.AddStructuralProperty("ComputerId", EdmCoreModel.Instance.GetInt32(false));
        computer.AddKeys(computerId);
        EdmStructuralProperty computerName = computer.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
        model.AddElement(computer);

        EdmEntityType computerDetail = new EdmEntityType("DefaultNamespace", "ComputerDetail");
        EdmStructuralProperty computerDetailId = computerDetail.AddStructuralProperty("ComputerDetailId", EdmCoreModel.Instance.GetInt32(false));
        computerDetail.AddKeys(computerDetailId);
        EdmStructuralProperty computerDetailManufacturer = computerDetail.AddStructuralProperty("Manufacturer", EdmCoreModel.Instance.GetString(false));
        EdmStructuralProperty computerDetailModel = computerDetail.AddStructuralProperty("Model", EdmCoreModel.Instance.GetString(false));
        EdmStructuralProperty computerDetailSerial = computerDetail.AddStructuralProperty("Serial", EdmCoreModel.Instance.GetString(false));
        EdmStructuralProperty computerDetailSpecificationsBag = computerDetail.AddStructuralProperty("SpecificationsBag", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false)));
        EdmStructuralProperty computerDetailPurchaseDate = computerDetail.AddStructuralProperty("PurchaseDate", EdmCoreModel.Instance.GetDateTimeOffset(false));
        EdmStructuralProperty computerDetailDimensions = computerDetail.AddStructuralProperty("Dimensions", new EdmComplexTypeReference(dimensions, false));
        model.AddElement(computerDetail);

        EdmEntityType driver = new EdmEntityType("DefaultNamespace", "Driver");
        EdmStructuralProperty driverName = driver.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false, 100, null, false));
        driver.AddKeys(driverName);
        EdmStructuralProperty driverBirthDate = driver.AddStructuralProperty("BirthDate", EdmCoreModel.Instance.GetDateTimeOffset(false));
        model.AddElement(driver);

        EdmEntityType license = new EdmEntityType("DefaultNamespace", "License");
        EdmStructuralProperty licenseName = license.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false, 100, null, false));
        license.AddKeys(licenseName);
        EdmStructuralProperty licenseNumber = license.AddStructuralProperty("LicenseNumber", EdmCoreModel.Instance.GetString(false));
        EdmStructuralProperty licenseClass = license.AddStructuralProperty("LicenseClass", EdmCoreModel.Instance.GetString(false));
        EdmStructuralProperty licenseRestrictions = license.AddStructuralProperty("Restrictions", EdmCoreModel.Instance.GetString(false));
        EdmStructuralProperty licenseExpiration = license.AddStructuralProperty("ExpirationDate", EdmCoreModel.Instance.GetDateTimeOffset(false));
        model.AddElement(license);

        EdmEntityType mappedEntity = new EdmEntityType("DefaultNamespace", "MappedEntityType");
        EdmStructuralProperty mappedEntityId = mappedEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        mappedEntity.AddKeys(mappedEntityId);
        EdmStructuralProperty mappedEntityHref = mappedEntity.AddStructuralProperty("Href", EdmCoreModel.Instance.GetString(false));
        EdmStructuralProperty mappedEntityTitle = mappedEntity.AddStructuralProperty("Title", EdmCoreModel.Instance.GetString(false));
        EdmStructuralProperty mappedEntityHrefLang = mappedEntity.AddStructuralProperty("HrefLang", EdmCoreModel.Instance.GetString(false));
        EdmStructuralProperty mappedEntityType = mappedEntity.AddStructuralProperty("Type", EdmCoreModel.Instance.GetString(false));
        EdmStructuralProperty mappedEntityLength = mappedEntity.AddStructuralProperty("Length", EdmCoreModel.Instance.GetInt32(false));
        EdmStructuralProperty mappedEntityBagOfPrimitiveToLinks = mappedEntity.AddStructuralProperty("BagOfPrimitiveToLinks", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false)));
        EdmStructuralProperty mappedEntityBagOfDecimals = mappedEntity.AddStructuralProperty("BagOfDecimals", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetDecimal(false)));
        EdmStructuralProperty mappedEntityBagOfDoubles = mappedEntity.AddStructuralProperty("BagOfDoubles", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetDouble(false)));
        EdmStructuralProperty mappedEntityBagOfSingles = mappedEntity.AddStructuralProperty("BagOfSingles", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetSingle(false)));
        EdmStructuralProperty mappedEntityBagOfBytes = mappedEntity.AddStructuralProperty("BagOfBytes", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetByte(false)));
        EdmStructuralProperty mappedEntityBagOfInt16s = mappedEntity.AddStructuralProperty("BagOfInt16s", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt16(false)));
        EdmStructuralProperty mappedEntityBagOfInt32s = mappedEntity.AddStructuralProperty("BagOfInt32s", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(false)));
        EdmStructuralProperty mappedEntityBagOfInt64s = mappedEntity.AddStructuralProperty("BagOfInt64s", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt64(false)));
        EdmStructuralProperty mappedEntityBagOfGuids = mappedEntity.AddStructuralProperty("BagOfGuids", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetGuid(false)));
        EdmStructuralProperty mappedEntityBagOfComplexToCategories = mappedEntity.AddStructuralProperty("BagOfComplexToCategories", EdmCoreModel.GetCollection(new EdmComplexTypeReference(category, false)));
        model.AddElement(mappedEntity);

        EdmEntityType car = new EdmEntityType("DefaultNamespace", "Car");
        EdmStructuralProperty carVin = car.AddStructuralProperty("VIN", EdmCoreModel.Instance.GetInt32(false));
        car.AddKeys(carVin);
        EdmStructuralProperty carDescription = car.AddStructuralProperty("Description", EdmCoreModel.Instance.GetString(false, 100, null, true));
        EdmStructuralProperty carPhoto = car.AddStructuralProperty("Photo", EdmCoreModel.Instance.GetStream(false));
        EdmStructuralProperty carVideo = car.AddStructuralProperty("Video", EdmCoreModel.Instance.GetStream(false));
        model.AddElement(car);

        EdmEntityType person = new EdmEntityType("DefaultNamespace", "Person");
        EdmStructuralProperty personId = person.AddStructuralProperty("PersonId", EdmCoreModel.Instance.GetInt32(false));
        person.AddKeys(personId);
        EdmStructuralProperty personName = person.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
        model.AddElement(person);

        EdmEntityType employee = new EdmEntityType("DefaultNamespace", "Employee", person);
        EdmStructuralProperty employeeManagerId = employee.AddStructuralProperty("ManagersPersonId", EdmCoreModel.Instance.GetInt32(false));
        EdmStructuralProperty employeeSalary = employee.AddStructuralProperty("Salary", EdmCoreModel.Instance.GetInt32(false));
        EdmStructuralProperty employeeTitle = employee.AddStructuralProperty("Title", EdmCoreModel.Instance.GetString(false));
        model.AddElement(employee);

        EdmEntityType specialEmployee = new EdmEntityType("DefaultNamespace", "SpecialEmployee", employee);
        EdmStructuralProperty specialEmployeeCarsVIN = specialEmployee.AddStructuralProperty("CarsVIN", EdmCoreModel.Instance.GetInt32(false));
        EdmStructuralProperty specialEmployeeBonus = specialEmployee.AddStructuralProperty("Bonus", EdmCoreModel.Instance.GetInt32(false));
        EdmStructuralProperty specialEmployeeIsFullyVested = specialEmployee.AddStructuralProperty("IsFullyVested", EdmCoreModel.Instance.GetBoolean(false));
        model.AddElement(specialEmployee);

        EdmEntityType personMetadata = new EdmEntityType("DefaultNamespace", "PersonMetadata");
        EdmStructuralProperty personMetadataId = personMetadata.AddStructuralProperty("PersonMetadataId", EdmCoreModel.Instance.GetInt32(false));
        personMetadata.AddKeys(personMetadataId);
        EdmStructuralProperty personMetadataPersonId = personMetadata.AddStructuralProperty("PersonId", EdmCoreModel.Instance.GetInt32(false));
        EdmStructuralProperty personMetadataPropertyName = personMetadata.AddStructuralProperty("PropertyName", EdmCoreModel.Instance.GetString(false));
        EdmStructuralProperty personMetadataPropertyValue = personMetadata.AddStructuralProperty("PropertyValue", EdmCoreModel.Instance.GetString(false));
        model.AddElement(personMetadata);

        EdmNavigationProperty customerToOrders = customer.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "Orders", Target = order, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "Customer", TargetMultiplicity = EdmMultiplicity.ZeroOrOne, DependentProperties = new[] { orderCustomerId }, PrincipalProperties = customer.Key() });

        EdmNavigationProperty customerToLogins = customer.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "Logins", Target = login, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "Customer", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { loginCustomerId }, PrincipalProperties = customer.Key() });

        EdmNavigationProperty customerToHusband = customer.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "Husband", Target = customer, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
            new EdmNavigationPropertyInfo() { Name = "Wife", TargetMultiplicity = EdmMultiplicity.ZeroOrOne });

        EdmNavigationProperty customerToInfo = customer.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "Info", Target = customerInfo, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
            new EdmNavigationPropertyInfo() { Name = "Customer", TargetMultiplicity = EdmMultiplicity.One });

        EdmNavigationProperty customerToComplaint = customer.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "Complaints", Target = complaint, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "Customer", TargetMultiplicity = EdmMultiplicity.ZeroOrOne, DependentProperties = new[] { complaintCustomerId }, PrincipalProperties = customer.Key() });

        EdmNavigationProperty barcodeToProduct = barcode.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "Product", Target = product, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { barcodeProductId }, PrincipalProperties = product.Key() },
            new EdmNavigationPropertyInfo() { Name = "Barcodes", TargetMultiplicity = EdmMultiplicity.Many });

        EdmNavigationProperty barcodeToExpectedIncorrectScans = barcode.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "BadScans", Target = incorrectScan, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "ExpectedBarcode", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { incorrectScanExpectedCode }, PrincipalProperties = barcode.Key() });

        EdmNavigationProperty barcodeToActualIncorrectScans = barcode.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "GoodScans", Target = incorrectScan, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "ActualBarcode", TargetMultiplicity = EdmMultiplicity.ZeroOrOne, DependentProperties = new[] { incorrectScanActualCode }, PrincipalProperties = barcode.Key() });

        EdmNavigationProperty barcodeToBarcodeDetail = barcode.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "Detail", Target = barcodeDetail, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
            new EdmNavigationPropertyInfo() { Name = "Barcode", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { barcodeDetailCode }, PrincipalProperties = barcode.Key() });

        EdmNavigationProperty complaintToResolution = complaint.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "Resolution", Target = resolution, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
            new EdmNavigationPropertyInfo() { Name = "Complaint", TargetMultiplicity = EdmMultiplicity.One });

        EdmNavigationProperty loginToLastLogin = login.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "LastLogin", Target = lastLogin, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
            new EdmNavigationPropertyInfo() { Name = "Login", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { lastLoginUsername }, PrincipalProperties = login.Key() });

        EdmNavigationProperty loginToSentMessages = login.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "SentMessages", Target = message, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "Sender", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { messageFromUsername }, PrincipalProperties = login.Key() });

        EdmNavigationProperty loginToReceivedMessages = login.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "ReceivedMessages", Target = message, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "Recipient", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { messageToUsername }, PrincipalProperties = login.Key() });

        EdmNavigationProperty loginToOrders = login.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "Orders", Target = order, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "Login", TargetMultiplicity = EdmMultiplicity.ZeroOrOne });

        EdmNavigationProperty loginToSmartCard = login.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "SmartCard", Target = smartCard, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
            new EdmNavigationPropertyInfo() { Name = "Login", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { smartCardUsername }, PrincipalProperties = login.Key() });

        EdmNavigationProperty loginToRsaToken = login.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "RSAToken", Target = rsaToken, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
            new EdmNavigationPropertyInfo() { Name = "Login", TargetMultiplicity = EdmMultiplicity.One });

        EdmNavigationProperty loginToPasswordReset = login.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "PasswordResets", Target = passwordReset, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "Login", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { passwordResetUsername }, PrincipalProperties = login.Key() });

        EdmNavigationProperty loginToPageView = login.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "PageViews", Target = pageView, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "Login", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { pageViewUsername }, PrincipalProperties = login.Key() });

        EdmNavigationProperty loginToSuspiciousActivity = login.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "SuspiciousActivity", Target = suspiciousActivity, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "Login", TargetMultiplicity = EdmMultiplicity.ZeroOrOne });

        EdmNavigationProperty smartCardToLastLogin = smartCard.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "LastLogin", Target = lastLogin, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
            new EdmNavigationPropertyInfo() { Name = "SmartCard", TargetMultiplicity = EdmMultiplicity.ZeroOrOne });

        EdmNavigationProperty orderToOrderLines = order.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "OrderLines", Target = orderLine, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "Order", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { orderLineOrderId }, PrincipalProperties = order.Key() });

        EdmNavigationProperty orderToOrderNotes = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "Notes", Target = orderNote, TargetMultiplicity = EdmMultiplicity.Many, OnDelete = EdmOnDeleteAction.Cascade },
            new EdmNavigationPropertyInfo() { Name = "Order", Target = order, TargetMultiplicity = EdmMultiplicity.One });
        order.AddProperty(orderToOrderNotes);
        orderNote.AddProperty(orderToOrderNotes.Partner);

        EdmNavigationProperty orderToOrderQualityCheck = order.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "OrderQualityCheck", Target = orderQualityCheck, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
            new EdmNavigationPropertyInfo() { Name = "Order", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { orderQualityCheckId }, PrincipalProperties = order.Key() });

        EdmNavigationProperty orderLineToProduct = orderLine.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "Product", Target = product, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { orderLineProductId }, PrincipalProperties = product.Key() },
            new EdmNavigationPropertyInfo() { Name = "OrderLines", TargetMultiplicity = EdmMultiplicity.Many });

        EdmNavigationProperty productToRelatedProducts = product.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "RelatedProducts", Target = product, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "ProductToRelatedProducts", TargetMultiplicity = EdmMultiplicity.One });

        EdmNavigationProperty productToSuppliers = product.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "Suppliers", Target = supplier, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "Products", TargetMultiplicity = EdmMultiplicity.Many });

        EdmNavigationProperty productToProductDetail = product.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "Detail", Target = productDetail, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
            new EdmNavigationPropertyInfo() { Name = "Product", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { productDetailProductId }, PrincipalProperties = product.Key() });

        EdmNavigationProperty productToProductReviews = product.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "Reviews", Target = productReview, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "Product", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { productReviewProductId }, PrincipalProperties = product.Key() });

        EdmNavigationProperty productToProductPhotos = product.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "Photos", Target = productPhoto, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "Product", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { productPhotoProductId }, PrincipalProperties = product.Key() });

        EdmNavigationProperty productReviewToProductWebFeatures = productReview.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "Features", Target = productWebFeature, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "Review", TargetMultiplicity = EdmMultiplicity.ZeroOrOne, DependentProperties = new[] { productWebFeatureReviewId, productWebFeatureProductId }, PrincipalProperties = productReview.Key() });

        EdmNavigationProperty productPhotoToProductWebFeatures = productPhoto.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "Features", Target = productWebFeature, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "Photo", TargetMultiplicity = EdmMultiplicity.ZeroOrOne, DependentProperties = new[] { productWebFeaturePhotoId, productWebFeatureProductId }, PrincipalProperties = productPhoto.Key() });

        EdmNavigationProperty supplierToSupplierLogo = supplier.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "Logo", Target = supplierLogo, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
            new EdmNavigationPropertyInfo() { Name = "Supplier", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { supplierLogoSupplierId }, PrincipalProperties = supplier.Key() });

        EdmNavigationProperty supplierToSupplierInfo = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "SupplierInfo", Target = supplierInfo, TargetMultiplicity = EdmMultiplicity.Many, OnDelete = EdmOnDeleteAction.Cascade },
            new EdmNavigationPropertyInfo() { Name = "Supplier", Target = supplier, TargetMultiplicity = EdmMultiplicity.One });
        supplier.AddProperty(supplierToSupplierInfo);
        supplierInfo.AddProperty(supplierToSupplierInfo.Partner);

        EdmNavigationProperty computerToComputerDetail = computer.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "ComputerDetail", Target = computerDetail, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "Computer", TargetMultiplicity = EdmMultiplicity.One });

        EdmNavigationProperty driverTolicense = driver.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "License", Target = license, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "Driver", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { licenseName }, PrincipalProperties = driver.Key() });

        EdmNavigationProperty personToPersonMetadata = person.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "PersonMetadata", Target = personMetadata, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "Person", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { personMetadataPersonId }, PrincipalProperties = person.Key() });

        EdmNavigationProperty employeeToManager = employee.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "Manager", Target = employee, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { employeeManagerId }, PrincipalProperties = employee.Key() },
            new EdmNavigationPropertyInfo() { Name = "EmployeeToManager", TargetMultiplicity = EdmMultiplicity.Many });

        EdmNavigationProperty specialEmployeeToCar = specialEmployee.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "Car", Target = car, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { specialEmployeeCarsVIN }, PrincipalProperties = car.Key() },
            new EdmNavigationPropertyInfo() { Name = "SpecialEmployee", TargetMultiplicity = EdmMultiplicity.Many });

        EdmOperation getPrimitiveString = new EdmFunction("DefaultNamespace", "GetPrimitiveString", EdmCoreModel.Instance.GetString(true));
        model.AddElement(getPrimitiveString);
        EdmOperation getSpecificCustomer = new EdmFunction("DefaultNamespace", "GetSpecificCustomer", EdmCoreModel.GetCollection(new EdmEntityTypeReference(customer, true)));
        getSpecificCustomer.AddParameter("Name", EdmCoreModel.Instance.GetString(false));
        model.AddElement(getSpecificCustomer);
        EdmOperation getCustomerCount = new EdmFunction("DefaultNamespace", "GetCustomerCount", EdmCoreModel.Instance.GetInt32(true));
        model.AddElement(getCustomerCount);
        EdmOperation getArgumentPlusOne = new EdmFunction("DefaultNamespace", "GetArgumentPlusOne", EdmCoreModel.Instance.GetInt32(true));
        getArgumentPlusOne.AddParameter("arg1", EdmCoreModel.Instance.GetInt32(false));
        model.AddElement(getArgumentPlusOne);
        EdmOperation entityProjectionReturnsCollectionOfComplexTypes = new EdmFunction("DefaultNamespace", "EntityProjectionReturnsCollectionOfComplexTypes", EdmCoreModel.GetCollection(new EdmComplexTypeReference(contact, true)));
        model.AddElement(entityProjectionReturnsCollectionOfComplexTypes);

        EdmOperation setArgumentPlusOne = new EdmAction("DefaultNamespace", "SetArgumentPlusOne", EdmCoreModel.Instance.GetInt32(true));
        setArgumentPlusOne.AddParameter("arg1", EdmCoreModel.Instance.GetInt32(false));
        model.AddElement(setArgumentPlusOne);

        EdmEntityContainer container = new EdmEntityContainer("DefaultNamespace", "DefaultContainer");
        model.AddElement(container);

        EdmEntitySet carSet = container.AddEntitySet("Car", car);
        EdmEntitySet customerSet = container.AddEntitySet("Customer", customer);
        EdmEntitySet barcodeSet = container.AddEntitySet("Barcode", barcode);
        EdmEntitySet incorrectScanSet = container.AddEntitySet("IncorrectScan", incorrectScan);
        EdmEntitySet barcodeDetailSet = container.AddEntitySet("BarcodeDetail", barcodeDetail);
        EdmEntitySet complaintSet = container.AddEntitySet("Complaint", complaint);
        EdmEntitySet resolutionSet = container.AddEntitySet("Resolution", resolution);
        EdmEntitySet loginSet = container.AddEntitySet("Login", login);
        EdmEntitySet suspiciousActivitySet = container.AddEntitySet("SuspiciousActivity", suspiciousActivity);
        EdmEntitySet smartCardSet = container.AddEntitySet("SmartCard", smartCard);
        EdmEntitySet rsaTokenSet = container.AddEntitySet("RSAToken", rsaToken);
        EdmEntitySet passwordResetSet = container.AddEntitySet("PasswordReset", passwordReset);
        EdmEntitySet pageViewSet = container.AddEntitySet("PageView", pageView);
        EdmEntitySet lastLoginSet = container.AddEntitySet("LastLogin", lastLogin);
        EdmEntitySet messageSet = container.AddEntitySet("Message", message);
        EdmEntitySet orderSet = container.AddEntitySet("Order", order);
        EdmEntitySet orderNoteSet = container.AddEntitySet("OrderNote", orderNote);
        EdmEntitySet orderQualityCheckSet = container.AddEntitySet("OrderQualityCheck", orderQualityCheck);
        EdmEntitySet orderLineSet = container.AddEntitySet("OrderLine", orderLine);
        EdmEntitySet productSet = container.AddEntitySet("Product", product);
        EdmEntitySet productDetailSet = container.AddEntitySet("ProductDetail", productDetail);
        EdmEntitySet productReviewSet = container.AddEntitySet("ProductReview", productReview);
        EdmEntitySet productPhotoSet = container.AddEntitySet("ProductPhoto", productPhoto);
        EdmEntitySet productWebFeatureSet = container.AddEntitySet("ProductWebFeature", productWebFeature);
        EdmEntitySet supplierSet = container.AddEntitySet("Supplier", supplier);
        EdmEntitySet supplierLogoSet = container.AddEntitySet("SupplierLogo", supplierLogo);
        EdmEntitySet supplierInfoSet = container.AddEntitySet("SupplierInfo", supplierInfo);
        EdmEntitySet customerInfoSet = container.AddEntitySet("CustomerInfo", customerInfo);
        EdmEntitySet computerSet = container.AddEntitySet("Computer", computer);
        EdmEntitySet computerDetailSet = container.AddEntitySet("ComputerDetail", computerDetail);
        EdmEntitySet driverSet = container.AddEntitySet("Driver", driver);
        EdmEntitySet licenseSet = container.AddEntitySet("License", license);
        EdmEntitySet mappedEntitySet = container.AddEntitySet("MappedEntityType", mappedEntity);
        EdmEntitySet personSet = container.AddEntitySet("Person", person);
        EdmEntitySet personMetadataSet = container.AddEntitySet("PersonMetadata", personMetadata);

        complaintSet.AddNavigationTarget(customerToComplaint.Partner, customerSet);
        loginSet.AddNavigationTarget(loginToSentMessages, messageSet);
        loginSet.AddNavigationTarget(loginToReceivedMessages, messageSet);
        customerInfoSet.AddNavigationTarget(customerToInfo.Partner, customerSet);
        supplierSet.AddNavigationTarget(supplierToSupplierInfo, supplierInfoSet);
        loginSet.AddNavigationTarget(loginToOrders, orderSet);
        orderSet.AddNavigationTarget(orderToOrderNotes, orderNoteSet);
        orderSet.AddNavigationTarget(orderToOrderQualityCheck, orderQualityCheckSet);
        supplierSet.AddNavigationTarget(supplierToSupplierLogo, supplierLogoSet);
        customerSet.AddNavigationTarget(customerToOrders, orderSet);
        customerSet.AddNavigationTarget(customerToLogins, loginSet);
        loginSet.AddNavigationTarget(loginToLastLogin, lastLoginSet);
        lastLoginSet.AddNavigationTarget(smartCardToLastLogin.Partner, smartCardSet);
        orderSet.AddNavigationTarget(orderToOrderLines, orderLineSet);
        productSet.AddNavigationTarget(orderLineToProduct.Partner, orderLineSet);
        productSet.AddNavigationTarget(productToRelatedProducts, productSet);
        productSet.AddNavigationTarget(productToProductDetail, productDetailSet);
        productSet.AddNavigationTarget(productToProductReviews, productReviewSet);
        productSet.AddNavigationTarget(productToProductPhotos, productPhotoSet);
        productWebFeatureSet.AddNavigationTarget(productPhotoToProductWebFeatures.Partner, productPhotoSet);
        productWebFeatureSet.AddNavigationTarget(productReviewToProductWebFeatures.Partner, productReviewSet);
        complaintSet.AddNavigationTarget(complaintToResolution, resolutionSet);
        barcodeSet.AddNavigationTarget(barcodeToExpectedIncorrectScans, incorrectScanSet);
        customerSet.AddNavigationTarget(customerToHusband.Partner, customerSet);
        barcodeSet.AddNavigationTarget(barcodeToActualIncorrectScans, incorrectScanSet);
        productSet.AddNavigationTarget(barcodeToProduct.Partner, barcodeSet);
        barcodeSet.AddNavigationTarget(barcodeToBarcodeDetail, barcodeDetailSet);
        loginSet.AddNavigationTarget(loginToSuspiciousActivity, suspiciousActivitySet);
        loginSet.AddNavigationTarget(loginToRsaToken, rsaTokenSet);
        loginSet.AddNavigationTarget(loginToSmartCard, smartCardSet);
        loginSet.AddNavigationTarget(loginToPasswordReset, passwordResetSet);
        loginSet.AddNavigationTarget(loginToPageView, pageViewSet);
        computerSet.AddNavigationTarget(computerToComputerDetail, computerDetailSet);
        driverSet.AddNavigationTarget(driverTolicense, licenseSet);
        personSet.AddNavigationTarget(personToPersonMetadata, personMetadataSet);
        productSet.AddNavigationTarget(productToSuppliers, supplierSet);
        carSet.AddNavigationTarget(specialEmployeeToCar.Partner, personSet);
        personSet.AddNavigationTarget(employeeToManager, personSet);

        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"">
        <EntitySet EntityType=""DefaultNamespace.Barcode"" Name=""Barcode"">
            <NavigationPropertyBinding Path=""BadScans"" Target=""IncorrectScan"" />
            <NavigationPropertyBinding Path=""GoodScans"" Target=""IncorrectScan"" />
            <NavigationPropertyBinding Path=""Detail"" Target=""BarcodeDetail"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.BarcodeDetail"" Name=""BarcodeDetail"" />
        <EntitySet EntityType=""DefaultNamespace.Car"" Name=""Car"">
            <NavigationPropertyBinding Path=""SpecialEmployee"" Target=""Person"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.Complaint"" Name=""Complaint"">
            <NavigationPropertyBinding Path=""Customer"" Target=""Customer"" />
            <NavigationPropertyBinding Path=""Resolution"" Target=""Resolution"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.Computer"" Name=""Computer"">
            <NavigationPropertyBinding Path=""ComputerDetail"" Target=""ComputerDetail"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.ComputerDetail"" Name=""ComputerDetail"" />
        <EntitySet EntityType=""DefaultNamespace.Customer"" Name=""Customer"">
            <NavigationPropertyBinding Path=""Orders"" Target=""Order"" />
            <NavigationPropertyBinding Path=""Logins"" Target=""Login"" />
            <NavigationPropertyBinding Path=""Wife"" Target=""Customer"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.CustomerInfo"" Name=""CustomerInfo"">
            <NavigationPropertyBinding Path=""Customer"" Target=""Customer"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.Driver"" Name=""Driver"">
            <NavigationPropertyBinding Path=""License"" Target=""License"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.IncorrectScan"" Name=""IncorrectScan"" />
        <EntitySet EntityType=""DefaultNamespace.LastLogin"" Name=""LastLogin"">
            <NavigationPropertyBinding Path=""SmartCard"" Target=""SmartCard"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.License"" Name=""License"" />
        <EntitySet EntityType=""DefaultNamespace.Login"" Name=""Login"">
            <NavigationPropertyBinding Path=""SentMessages"" Target=""Message"" />
            <NavigationPropertyBinding Path=""ReceivedMessages"" Target=""Message"" />
            <NavigationPropertyBinding Path=""Orders"" Target=""Order"" />
            <NavigationPropertyBinding Path=""LastLogin"" Target=""LastLogin"" />
            <NavigationPropertyBinding Path=""SuspiciousActivity"" Target=""SuspiciousActivity"" />
            <NavigationPropertyBinding Path=""RSAToken"" Target=""RSAToken"" />
            <NavigationPropertyBinding Path=""SmartCard"" Target=""SmartCard"" />
            <NavigationPropertyBinding Path=""PasswordResets"" Target=""PasswordReset"" />
            <NavigationPropertyBinding Path=""PageViews"" Target=""PageView"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.MappedEntityType"" Name=""MappedEntityType"" />
        <EntitySet EntityType=""DefaultNamespace.Message"" Name=""Message"" />
        <EntitySet EntityType=""DefaultNamespace.Order"" Name=""Order"">
            <NavigationPropertyBinding Path=""Notes"" Target=""OrderNote"" />
            <NavigationPropertyBinding Path=""OrderQualityCheck"" Target=""OrderQualityCheck"" />
            <NavigationPropertyBinding Path=""OrderLines"" Target=""OrderLine"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.OrderLine"" Name=""OrderLine"" />
        <EntitySet EntityType=""DefaultNamespace.OrderNote"" Name=""OrderNote"" />
        <EntitySet EntityType=""DefaultNamespace.OrderQualityCheck"" Name=""OrderQualityCheck"" />
        <EntitySet EntityType=""DefaultNamespace.PageView"" Name=""PageView"" />
        <EntitySet EntityType=""DefaultNamespace.PasswordReset"" Name=""PasswordReset"" />
        <EntitySet EntityType=""DefaultNamespace.Person"" Name=""Person"">
            <NavigationPropertyBinding Path=""PersonMetadata"" Target=""PersonMetadata"" />
            <NavigationPropertyBinding Path=""DefaultNamespace.Employee/Manager"" Target=""Person"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.PersonMetadata"" Name=""PersonMetadata"" />
        <EntitySet EntityType=""DefaultNamespace.Product"" Name=""Product"">
            <NavigationPropertyBinding Path=""OrderLines"" Target=""OrderLine"" />
            <NavigationPropertyBinding Path=""RelatedProducts"" Target=""Product"" />
            <NavigationPropertyBinding Path=""Detail"" Target=""ProductDetail"" />
            <NavigationPropertyBinding Path=""Reviews"" Target=""ProductReview"" />
            <NavigationPropertyBinding Path=""Photos"" Target=""ProductPhoto"" />
            <NavigationPropertyBinding Path=""Barcodes"" Target=""Barcode"" />
            <NavigationPropertyBinding Path=""Suppliers"" Target=""Supplier"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.ProductDetail"" Name=""ProductDetail"" />
        <EntitySet EntityType=""DefaultNamespace.ProductPhoto"" Name=""ProductPhoto"" />
        <EntitySet EntityType=""DefaultNamespace.ProductReview"" Name=""ProductReview"" />
        <EntitySet EntityType=""DefaultNamespace.ProductWebFeature"" Name=""ProductWebFeature"">
            <NavigationPropertyBinding Path=""Photo"" Target=""ProductPhoto"" />
            <NavigationPropertyBinding Path=""Review"" Target=""ProductReview"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.RSAToken"" Name=""RSAToken"" />
        <EntitySet EntityType=""DefaultNamespace.Resolution"" Name=""Resolution"" />
        <EntitySet EntityType=""DefaultNamespace.SmartCard"" Name=""SmartCard"" />
        <EntitySet EntityType=""DefaultNamespace.Supplier"" Name=""Supplier"">
            <NavigationPropertyBinding Path=""SupplierInfo"" Target=""SupplierInfo"" />
            <NavigationPropertyBinding Path=""Logo"" Target=""SupplierLogo"" />
        </EntitySet>
        <EntitySet EntityType=""DefaultNamespace.SupplierInfo"" Name=""SupplierInfo"" />
        <EntitySet EntityType=""DefaultNamespace.SupplierLogo"" Name=""SupplierLogo"" />
        <EntitySet EntityType=""DefaultNamespace.SuspiciousActivity"" Name=""SuspiciousActivity"" />
     </EntityContainer>
    
  <ComplexType Name=""Aliases"">
    <Property MaxLength=""10"" Name=""AlternativeNames"" Nullable=""false"" Type=""Collection(Edm.String)"" />
  </ComplexType>
  <ComplexType Name=""AuditInfo"">
    <Property Name=""Concurrency"" Nullable=""false"" Type=""DefaultNamespace.ConcurrencyInfo"" />
    <Property MaxLength=""50"" Name=""ModifiedBy"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""ModifiedDate"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
  </ComplexType>
  <ComplexType Name=""ComplexToCategory"">
    <Property Name=""Label"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""Scheme"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""Term"" Nullable=""false"" Type=""Edm.String"" />
  </ComplexType>
  <ComplexType Name=""ConcurrencyInfo"">
    <Property Name=""QueriedDateTime"" Nullable=""true"" Type=""Edm.DateTimeOffset"" />
    <Property MaxLength=""20"" Name=""Token"" Nullable=""false"" Type=""Edm.String"" />
  </ComplexType>
  <ComplexType Name=""ContactDetails"">
    <Property MaxLength=""10"" Name=""AlternativeNames"" Nullable=""false"" Type=""Collection(Edm.String)"" />
    <Property Name=""ContactAlias"" Nullable=""false"" Type=""DefaultNamespace.Aliases"" />
    <Property MaxLength=""32"" Name=""EmailBag"" Nullable=""false"" Type=""Collection(Edm.String)"" />
    <Property Name=""HomePhone"" Nullable=""false"" Type=""DefaultNamespace.Phone"" />
    <Property Name=""MobilePhoneBag"" Nullable=""false"" Type=""Collection(DefaultNamespace.Phone)"" />
    <Property Name=""WorkPhone"" Nullable=""false"" Type=""DefaultNamespace.Phone"" />
  </ComplexType>
  <ComplexType Name=""Dimensions"">
    <Property Name=""Depth"" Nullable=""false"" Precision=""10"" Scale=""3"" Type=""Edm.Decimal"" />
    <Property Name=""Height"" Nullable=""false"" Precision=""10"" Scale=""3"" Type=""Edm.Decimal"" />
    <Property Name=""Width"" Nullable=""false"" Precision=""10"" Scale=""3"" Type=""Edm.Decimal"" />
  </ComplexType>
  <ComplexType Name=""Phone"">
    <Property MaxLength=""16"" Name=""Extension"" Nullable=""true"" Type=""Edm.String"" />
    <Property MaxLength=""16"" Name=""PhoneNumber"" Nullable=""false"" Type=""Edm.String"" />
  </ComplexType>
  <EntityType BaseType=""DefaultNamespace.OrderLine"" Name=""BackOrderLine"" />
  <EntityType BaseType=""DefaultNamespace.BackOrderLine"" Name=""BackOrderLine2"" />
  <EntityType Name=""Barcode"">
    <Key>
      <PropertyRef Name=""Code"" />
    </Key>
    <NavigationProperty Name=""BadScans"" Partner=""ExpectedBarcode"" Type=""Collection(DefaultNamespace.IncorrectScan)"" />
    <NavigationProperty Name=""Detail"" Partner=""Barcode"" Type=""DefaultNamespace.BarcodeDetail"" />
    <NavigationProperty Name=""GoodScans"" Partner=""ActualBarcode"" Type=""Collection(DefaultNamespace.IncorrectScan)"" />
    <NavigationProperty Name=""Product"" Nullable=""false"" Partner=""Barcodes"" Type=""DefaultNamespace.Product"">
      <ReferentialConstraint Property=""ProductId"" ReferencedProperty=""ProductId"" />
    </NavigationProperty>
    <Property Name=""Code"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""ProductId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Text"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType Name=""BarcodeDetail"">
    <Key>
      <PropertyRef Name=""Code"" />
    </Key>
    <NavigationProperty Name=""Barcode"" Nullable=""false"" Partner=""Detail"" Type=""DefaultNamespace.Barcode"">
      <ReferentialConstraint Property=""Code"" ReferencedProperty=""Code"" />
    </NavigationProperty>
    <Property Name=""Code"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""RegisteredTo"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType Name=""Car"">
    <Key>
      <PropertyRef Name=""VIN"" />
    </Key>
    <NavigationProperty Name=""SpecialEmployee"" Partner=""Car"" Type=""Collection(DefaultNamespace.SpecialEmployee)"" />
    <Property MaxLength=""100"" Name=""Description"" Nullable=""true"" Type=""Edm.String"" />
    <Property Name=""Photo"" Nullable=""false"" Type=""Edm.Stream"" />
    <Property Name=""VIN"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Video"" Nullable=""false"" Type=""Edm.Stream"" />
  </EntityType>
  <EntityType Name=""Complaint"">
    <Key>
      <PropertyRef Name=""ComplaintId"" />
    </Key>
    <NavigationProperty Name=""Customer"" Partner=""Complaints"" Type=""DefaultNamespace.Customer"">
      <ReferentialConstraint Property=""CustomerId"" ReferencedProperty=""CustomerId"" />
    </NavigationProperty>
    <NavigationProperty Name=""Resolution"" Partner=""Complaint"" Type=""DefaultNamespace.Resolution"" />
    <Property Name=""ComplaintId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""CustomerId"" Nullable=""true"" Type=""Edm.Int32"" />
    <Property Name=""Details"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""Logged"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
  </EntityType>
  <EntityType Name=""Computer"">
    <Key>
      <PropertyRef Name=""ComputerId"" />
    </Key>
    <NavigationProperty Name=""ComputerDetail"" Nullable=""false"" Partner=""Computer"" Type=""DefaultNamespace.ComputerDetail"" />
    <Property Name=""ComputerId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Name"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType Name=""ComputerDetail"">
    <Key>
      <PropertyRef Name=""ComputerDetailId"" />
    </Key>
    <NavigationProperty Name=""Computer"" Nullable=""false"" Partner=""ComputerDetail"" Type=""DefaultNamespace.Computer"" />
    <Property Name=""ComputerDetailId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Dimensions"" Nullable=""false"" Type=""DefaultNamespace.Dimensions"" />
    <Property Name=""Manufacturer"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""Model"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""PurchaseDate"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
    <Property Name=""Serial"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""SpecificationsBag"" Nullable=""false"" Type=""Collection(Edm.String)"" />
  </EntityType>
  <EntityType Name=""Customer"">
    <Key>
      <PropertyRef Name=""CustomerId"" />
    </Key>
    <NavigationProperty Name=""Complaints"" Partner=""Customer"" Type=""Collection(DefaultNamespace.Complaint)"" />
    <NavigationProperty Name=""Husband"" Partner=""Wife"" Type=""DefaultNamespace.Customer"" />
    <NavigationProperty Name=""Info"" Partner=""Customer"" Type=""DefaultNamespace.CustomerInfo"" />
    <NavigationProperty Name=""Logins"" Partner=""Customer"" Type=""Collection(DefaultNamespace.Login)"" />
    <NavigationProperty Name=""Orders"" Partner=""Customer"" Type=""Collection(DefaultNamespace.Order)"" />
    <NavigationProperty Name=""Wife"" Partner=""Husband"" Type=""DefaultNamespace.Customer"" />
    <Property Name=""Auditing"" Nullable=""false"" Type=""DefaultNamespace.AuditInfo"" />
    <Property Name=""BackupContactInfo"" Nullable=""false"" Type=""Collection(DefaultNamespace.ContactDetails)"" />
    <Property Name=""CustomerId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property MaxLength=""100"" Name=""Name"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""PrimaryContactInfo"" Nullable=""false"" Type=""DefaultNamespace.ContactDetails"" />
    <Property Name=""Thumbnail"" Nullable=""false"" Type=""Edm.Stream"" />
    <Property Name=""Video"" Nullable=""false"" Type=""Edm.Stream"" />
  </EntityType>
  <EntityType Name=""CustomerInfo"">
    <Key>
      <PropertyRef Name=""CustomerInfoId"" />
    </Key>
    <NavigationProperty Name=""Customer"" Nullable=""false"" Partner=""Info"" Type=""DefaultNamespace.Customer"" />
    <Property Name=""CustomerInfoId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Information"" Nullable=""true"" Type=""Edm.String"" />
  </EntityType>
  <EntityType BaseType=""DefaultNamespace.Product"" Name=""DiscontinuedProduct"">
    <Property Name=""Discontinued"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
    <Property Name=""DiscontinuedPhone"" Nullable=""false"" Type=""DefaultNamespace.Phone"" />
    <Property Name=""ReplacementProductId"" Nullable=""true"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""Driver"">
    <Key>
      <PropertyRef Name=""Name"" />
    </Key>
    <NavigationProperty Name=""License"" Nullable=""false"" Partner=""Driver"" Type=""DefaultNamespace.License"" />
    <Property Name=""BirthDate"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
    <Property MaxLength=""100"" Name=""Name"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType BaseType=""DefaultNamespace.Person"" Name=""Employee"">
    <NavigationProperty Name=""EmployeeToManager"" Partner=""Manager"" Type=""Collection(DefaultNamespace.Employee)"" />
    <NavigationProperty Name=""Manager"" Nullable=""false"" Partner=""EmployeeToManager"" Type=""DefaultNamespace.Employee"">
      <ReferentialConstraint Property=""ManagersPersonId"" ReferencedProperty=""PersonId"" />
    </NavigationProperty>
    <Property Name=""ManagersPersonId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Salary"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Title"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType Name=""IncorrectScan"">
    <Key>
      <PropertyRef Name=""IncorrectScanId"" />
    </Key>
    <NavigationProperty Name=""ActualBarcode"" Partner=""GoodScans"" Type=""DefaultNamespace.Barcode"">
      <ReferentialConstraint Property=""ActualCode"" ReferencedProperty=""Code"" />
    </NavigationProperty>
    <NavigationProperty Name=""ExpectedBarcode"" Nullable=""false"" Partner=""BadScans"" Type=""DefaultNamespace.Barcode"">
      <ReferentialConstraint Property=""ExpectedCode"" ReferencedProperty=""Code"" />
    </NavigationProperty>
    <Property Name=""ActualCode"" Nullable=""true"" Type=""Edm.Int32"" />
    <Property Name=""Details"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""ExpectedCode"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""IncorrectScanId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""ScanDate"" Nullable=""false"" Type=""Collection(Edm.DateTimeOffset)"" />
  </EntityType>
  <EntityType Name=""LastLogin"">
    <Key>
      <PropertyRef Name=""Username"" />
    </Key>
    <NavigationProperty Name=""Login"" Nullable=""false"" Partner=""LastLogin"" Type=""DefaultNamespace.Login"">
      <ReferentialConstraint Property=""Username"" ReferencedProperty=""Username"" />
    </NavigationProperty>
    <NavigationProperty Name=""SmartCard"" Partner=""LastLogin"" Type=""DefaultNamespace.SmartCard"" />
    <Property Name=""LoggedIn"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
    <Property Name=""LoggedOut"" Nullable=""true"" Type=""Edm.DateTimeOffset"" />
    <Property MaxLength=""50"" Name=""Username"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType Name=""License"">
    <Key>
      <PropertyRef Name=""Name"" />
    </Key>
    <NavigationProperty Name=""Driver"" Nullable=""false"" Partner=""License"" Type=""DefaultNamespace.Driver"">
      <ReferentialConstraint Property=""Name"" ReferencedProperty=""Name"" />
    </NavigationProperty>
    <Property Name=""ExpirationDate"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
    <Property Name=""LicenseClass"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""LicenseNumber"" Nullable=""false"" Type=""Edm.String"" />
    <Property MaxLength=""100"" Name=""Name"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""Restrictions"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType Name=""Login"">
    <Key>
      <PropertyRef Name=""Username"" />
    </Key>
    <NavigationProperty Name=""Customer"" Nullable=""false"" Partner=""Logins"" Type=""DefaultNamespace.Customer"">
      <ReferentialConstraint Property=""CustomerId"" ReferencedProperty=""CustomerId"" />
    </NavigationProperty>
    <NavigationProperty Name=""LastLogin"" Partner=""Login"" Type=""DefaultNamespace.LastLogin"" />
    <NavigationProperty Name=""Orders"" Partner=""Login"" Type=""Collection(DefaultNamespace.Order)"" />
    <NavigationProperty Name=""PageViews"" Partner=""Login"" Type=""Collection(DefaultNamespace.PageView)"" />
    <NavigationProperty Name=""PasswordResets"" Partner=""Login"" Type=""Collection(DefaultNamespace.PasswordReset)"" />
    <NavigationProperty Name=""RSAToken"" Partner=""Login"" Type=""DefaultNamespace.RSAToken"" />
    <NavigationProperty Name=""ReceivedMessages"" Partner=""Recipient"" Type=""Collection(DefaultNamespace.Message)"" />
    <NavigationProperty Name=""SentMessages"" Partner=""Sender"" Type=""Collection(DefaultNamespace.Message)"" />
    <NavigationProperty Name=""SmartCard"" Partner=""Login"" Type=""DefaultNamespace.SmartCard"" />
    <NavigationProperty Name=""SuspiciousActivity"" Partner=""Login"" Type=""Collection(DefaultNamespace.SuspiciousActivity)"" />
    <Property Name=""CustomerId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property MaxLength=""50"" Name=""Username"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType Name=""MappedEntityType"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""BagOfBytes"" Nullable=""false"" Type=""Collection(Edm.Byte)"" />
    <Property Name=""BagOfComplexToCategories"" Nullable=""false"" Type=""Collection(DefaultNamespace.ComplexToCategory)"" />
    <Property Name=""BagOfDecimals"" Nullable=""false"" Type=""Collection(Edm.Decimal)"" Scale=""Variable"" />
    <Property Name=""BagOfDoubles"" Nullable=""false"" Type=""Collection(Edm.Double)"" />
    <Property Name=""BagOfGuids"" Nullable=""false"" Type=""Collection(Edm.Guid)"" />
    <Property Name=""BagOfInt16s"" Nullable=""false"" Type=""Collection(Edm.Int16)"" />
    <Property Name=""BagOfInt32s"" Nullable=""false"" Type=""Collection(Edm.Int32)"" />
    <Property Name=""BagOfInt64s"" Nullable=""false"" Type=""Collection(Edm.Int64)"" />
    <Property Name=""BagOfPrimitiveToLinks"" Nullable=""false"" Type=""Collection(Edm.String)"" />
    <Property Name=""BagOfSingles"" Nullable=""false"" Type=""Collection(Edm.Single)"" />
    <Property Name=""Href"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""HrefLang"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""Id"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Length"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Title"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""Type"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType Name=""Message"">
    <Key>
      <PropertyRef Name=""FromUsername"" />
      <PropertyRef Name=""MessageId"" />
    </Key>
    <NavigationProperty Name=""Recipient"" Nullable=""false"" Partner=""ReceivedMessages"" Type=""DefaultNamespace.Login"">
      <ReferentialConstraint Property=""ToUsername"" ReferencedProperty=""Username"" />
    </NavigationProperty>
    <NavigationProperty Name=""Sender"" Nullable=""false"" Partner=""SentMessages"" Type=""DefaultNamespace.Login"">
      <ReferentialConstraint Property=""FromUsername"" ReferencedProperty=""Username"" />
    </NavigationProperty>
    <Property Name=""Body"" Nullable=""true"" Type=""Edm.String"" />
    <Property MaxLength=""50"" Name=""FromUsername"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""IsRead"" Nullable=""false"" Type=""Edm.Boolean"" />
    <Property Name=""MessageId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Sent"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
    <Property Name=""Subject"" Nullable=""false"" Type=""Edm.String"" />
    <Property MaxLength=""50"" Name=""ToUsername"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType Name=""Order"">
    <Key>
      <PropertyRef Name=""OrderId"" />
    </Key>
    <NavigationProperty Name=""Customer"" Partner=""Orders"" Type=""DefaultNamespace.Customer"">
      <ReferentialConstraint Property=""CustomerId"" ReferencedProperty=""CustomerId"" />
    </NavigationProperty>
    <NavigationProperty Name=""Login"" Partner=""Orders"" Type=""DefaultNamespace.Login"" />
    <NavigationProperty Name=""Notes"" Partner=""Order"" Type=""Collection(DefaultNamespace.OrderNote)"">
      <OnDelete Action=""Cascade"" />
    </NavigationProperty>
    <NavigationProperty Name=""OrderLines"" Partner=""Order"" Type=""Collection(DefaultNamespace.OrderLine)"" />
    <NavigationProperty Name=""OrderQualityCheck"" Partner=""Order"" Type=""DefaultNamespace.OrderQualityCheck"" />
    <Property Name=""Concurrency"" Nullable=""false"" Type=""DefaultNamespace.ConcurrencyInfo"" />
    <Property Name=""CustomerId"" Nullable=""true"" Type=""Edm.Int32"" />
    <Property Name=""OrderId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""OrderLine"">
    <Key>
      <PropertyRef Name=""OrderId"" />
      <PropertyRef Name=""ProductId"" />
    </Key>
    <NavigationProperty Name=""Order"" Nullable=""false"" Partner=""OrderLines"" Type=""DefaultNamespace.Order"">
      <ReferentialConstraint Property=""OrderId"" ReferencedProperty=""OrderId"" />
    </NavigationProperty>
    <NavigationProperty Name=""Product"" Nullable=""false"" Partner=""OrderLines"" Type=""DefaultNamespace.Product"">
      <ReferentialConstraint Property=""ProductId"" ReferencedProperty=""ProductId"" />
    </NavigationProperty>
    <Property Name=""ConcurrencyToken"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""OrderId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""OrderLineStream"" Nullable=""false"" Type=""Edm.Stream"" />
    <Property Name=""ProductId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Quantity"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""OrderNote"">
    <Key>
      <PropertyRef Name=""NoteId"" />
    </Key>
    <NavigationProperty Name=""Order"" Nullable=""false"" Partner=""Notes"" Type=""DefaultNamespace.Order"" />
    <Property Name=""Note"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""NoteId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""OrderQualityCheck"">
    <Key>
      <PropertyRef Name=""OrderId"" />
    </Key>
    <NavigationProperty Name=""Order"" Nullable=""false"" Partner=""OrderQualityCheck"" Type=""DefaultNamespace.Order"">
      <ReferentialConstraint Property=""OrderId"" ReferencedProperty=""OrderId"" />
    </NavigationProperty>
    <Property Name=""CheckedBy"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""CheckedDateTime"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
    <Property Name=""OrderId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""PageView"">
    <Key>
      <PropertyRef Name=""PageViewId"" />
    </Key>
    <NavigationProperty Name=""Login"" Nullable=""false"" Partner=""PageViews"" Type=""DefaultNamespace.Login"">
      <ReferentialConstraint Property=""Username"" ReferencedProperty=""Username"" />
    </NavigationProperty>
    <Property MaxLength=""500"" Name=""PageUrl"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""PageViewId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property MaxLength=""50"" Name=""Username"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""Viewed"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
  </EntityType>
  <EntityType Name=""PasswordReset"">
    <Key>
      <PropertyRef Name=""ResetNo"" />
      <PropertyRef Name=""Username"" />
    </Key>
    <NavigationProperty Name=""Login"" Nullable=""false"" Partner=""PasswordResets"" Type=""DefaultNamespace.Login"">
      <ReferentialConstraint Property=""Username"" ReferencedProperty=""Username"" />
    </NavigationProperty>
    <Property Name=""EmailedTo"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""ResetNo"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""TempPassword"" Nullable=""false"" Type=""Edm.String"" />
    <Property MaxLength=""50"" Name=""Username"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""PersonId"" />
    </Key>
    <NavigationProperty Name=""PersonMetadata"" Partner=""Person"" Type=""Collection(DefaultNamespace.PersonMetadata)"" />
    <Property Name=""Name"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""PersonId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""PersonMetadata"">
    <Key>
      <PropertyRef Name=""PersonMetadataId"" />
    </Key>
    <NavigationProperty Name=""Person"" Nullable=""false"" Partner=""PersonMetadata"" Type=""DefaultNamespace.Person"">
      <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""PersonId"" />
    </NavigationProperty>
    <Property Name=""PersonId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""PersonMetadataId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""PropertyName"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""PropertyValue"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType Name=""Product"">
    <Key>
      <PropertyRef Name=""ProductId"" />
    </Key>
    <NavigationProperty Name=""Barcodes"" Partner=""Product"" Type=""Collection(DefaultNamespace.Barcode)"" />
    <NavigationProperty Name=""Detail"" Partner=""Product"" Type=""DefaultNamespace.ProductDetail"" />
    <NavigationProperty Name=""OrderLines"" Partner=""Product"" Type=""Collection(DefaultNamespace.OrderLine)"" />
    <NavigationProperty Name=""Photos"" Partner=""Product"" Type=""Collection(DefaultNamespace.ProductPhoto)"" />
    <NavigationProperty Name=""ProductToRelatedProducts"" Nullable=""false"" Partner=""RelatedProducts"" Type=""DefaultNamespace.Product"" />
    <NavigationProperty Name=""RelatedProducts"" Partner=""ProductToRelatedProducts"" Type=""Collection(DefaultNamespace.Product)"" />
    <NavigationProperty Name=""Reviews"" Partner=""Product"" Type=""Collection(DefaultNamespace.ProductReview)"" />
    <NavigationProperty Name=""Suppliers"" Partner=""Products"" Type=""Collection(DefaultNamespace.Supplier)"" />
    <Property Name=""BaseConcurrency"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""ComplexConcurrency"" Nullable=""false"" Type=""DefaultNamespace.ConcurrencyInfo"" />
    <Property MaxLength=""1000"" Name=""Description"" Nullable=""true"" Type=""Edm.String"" />
    <Property Name=""Dimensions"" Nullable=""false"" Type=""DefaultNamespace.Dimensions"" />
    <Property Name=""NestedComplexConcurrency"" Nullable=""false"" Type=""DefaultNamespace.AuditInfo"" />
    <Property Name=""Picture"" Nullable=""false"" Type=""Edm.Stream"" />
    <Property Name=""ProductId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""ProductDetail"">
    <Key>
      <PropertyRef Name=""ProductId"" />
    </Key>
    <NavigationProperty Name=""Product"" Nullable=""false"" Partner=""Detail"" Type=""DefaultNamespace.Product"">
      <ReferentialConstraint Property=""ProductId"" ReferencedProperty=""ProductId"" />
    </NavigationProperty>
    <Property Name=""Details"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""ProductId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType BaseType=""DefaultNamespace.PageView"" Name=""ProductPageView"">
    <Property Name=""ProductId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""ProductPhoto"">
    <Key>
      <PropertyRef Name=""PhotoId"" />
      <PropertyRef Name=""ProductId"" />
    </Key>
    <NavigationProperty Name=""Features"" Partner=""Photo"" Type=""Collection(DefaultNamespace.ProductWebFeature)"" />
    <NavigationProperty Name=""Product"" Nullable=""false"" Partner=""Photos"" Type=""DefaultNamespace.Product"">
      <ReferentialConstraint Property=""ProductId"" ReferencedProperty=""ProductId"" />
    </NavigationProperty>
    <Property Name=""Photo"" Nullable=""false"" Type=""Edm.Binary"" />
    <Property Name=""PhotoId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""ProductId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""ProductReview"">
    <Key>
      <PropertyRef Name=""ProductId"" />
      <PropertyRef Name=""ReviewId"" />
    </Key>
    <NavigationProperty Name=""Features"" Partner=""Review"" Type=""Collection(DefaultNamespace.ProductWebFeature)"" />
    <NavigationProperty Name=""Product"" Nullable=""false"" Partner=""Reviews"" Type=""DefaultNamespace.Product"">
      <ReferentialConstraint Property=""ProductId"" ReferencedProperty=""ProductId"" />
    </NavigationProperty>
    <Property Name=""ProductId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Review"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""ReviewId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""ProductWebFeature"">
    <Key>
      <PropertyRef Name=""FeatureId"" />
    </Key>
    <NavigationProperty Name=""Photo"" Partner=""Features"" Type=""DefaultNamespace.ProductPhoto"">
      <ReferentialConstraint Property=""PhotoId"" ReferencedProperty=""ProductId"" />
      <ReferentialConstraint Property=""ProductId"" ReferencedProperty=""PhotoId"" />
    </NavigationProperty>
    <NavigationProperty Name=""Review"" Partner=""Features"" Type=""DefaultNamespace.ProductReview"">
      <ReferentialConstraint Property=""ReviewId"" ReferencedProperty=""ProductId"" />
      <ReferentialConstraint Property=""ProductId"" ReferencedProperty=""ReviewId"" />
    </NavigationProperty>
    <Property Name=""FeatureId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Heading"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""PhotoId"" Nullable=""true"" Type=""Edm.Int32"" />
    <Property Name=""ProductId"" Nullable=""true"" Type=""Edm.Int32"" />
    <Property Name=""ReviewId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""RSAToken"">
    <Key>
      <PropertyRef Name=""Serial"" />
    </Key>
    <NavigationProperty Name=""Login"" Nullable=""false"" Partner=""RSAToken"" Type=""DefaultNamespace.Login"" />
    <Property Name=""Issued"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
    <Property MaxLength=""20"" Name=""Serial"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType Name=""Resolution"">
    <Key>
      <PropertyRef Name=""ResolutionId"" />
    </Key>
    <NavigationProperty Name=""Complaint"" Nullable=""false"" Partner=""Resolution"" Type=""DefaultNamespace.Complaint"" />
    <Property Name=""Details"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""ResolutionId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""SmartCard"">
    <Key>
      <PropertyRef Name=""Username"" />
    </Key>
    <NavigationProperty Name=""LastLogin"" Partner=""SmartCard"" Type=""DefaultNamespace.LastLogin"" />
    <NavigationProperty Name=""Login"" Nullable=""false"" Partner=""SmartCard"" Type=""DefaultNamespace.Login"">
      <ReferentialConstraint Property=""Username"" ReferencedProperty=""Username"" />
    </NavigationProperty>
    <Property Name=""CardSerial"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""Issued"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
    <Property MaxLength=""50"" Name=""Username"" Nullable=""false"" Type=""Edm.String"" />
  </EntityType>
  <EntityType BaseType=""DefaultNamespace.Employee"" Name=""SpecialEmployee"">
    <NavigationProperty Name=""Car"" Nullable=""false"" Partner=""SpecialEmployee"" Type=""DefaultNamespace.Car"">
      <ReferentialConstraint Property=""CarsVIN"" ReferencedProperty=""VIN"" />
    </NavigationProperty>
    <Property Name=""Bonus"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""CarsVIN"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""IsFullyVested"" Nullable=""false"" Type=""Edm.Boolean"" />
  </EntityType>
  <EntityType Name=""Supplier"">
    <Key>
      <PropertyRef Name=""SupplierId"" />
    </Key>
    <NavigationProperty Name=""Logo"" Partner=""Supplier"" Type=""DefaultNamespace.SupplierLogo"" />
    <NavigationProperty Name=""Products"" Partner=""Suppliers"" Type=""Collection(DefaultNamespace.Product)"" />
    <NavigationProperty Name=""SupplierInfo"" Partner=""Supplier"" Type=""Collection(DefaultNamespace.SupplierInfo)"">
      <OnDelete Action=""Cascade"" />
    </NavigationProperty>
    <Property Name=""Name"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""SupplierId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""SupplierInfo"">
    <Key>
      <PropertyRef Name=""SupplierInfoId"" />
    </Key>
    <NavigationProperty Name=""Supplier"" Nullable=""false"" Partner=""SupplierInfo"" Type=""DefaultNamespace.Supplier"" />
    <Property Name=""Information"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""SupplierInfoId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""SupplierLogo"">
    <Key>
      <PropertyRef Name=""SupplierId"" />
    </Key>
    <NavigationProperty Name=""Supplier"" Nullable=""false"" Partner=""Logo"" Type=""DefaultNamespace.Supplier"">
      <ReferentialConstraint Property=""SupplierId"" ReferencedProperty=""SupplierId"" />
    </NavigationProperty>
    <Property MaxLength=""500"" Name=""Logo"" Nullable=""false"" Type=""Edm.Binary"" />
    <Property Name=""SupplierId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""SuspiciousActivity"">
    <Key>
      <PropertyRef Name=""SuspiciousActivityId"" />
    </Key>
    <NavigationProperty Name=""Login"" Partner=""SuspiciousActivity"" Type=""DefaultNamespace.Login"" />
    <Property Name=""Activity"" Nullable=""false"" Type=""Edm.String"" />
    <Property Name=""SuspiciousActivityId"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <Function Name=""EntityProjectionReturnsCollectionOfComplexTypes"">
    <ReturnType Type=""Collection(DefaultNamespace.ContactDetails)"" />
  </Function>
  <Function Name=""GetArgumentPlusOne"">
    <Parameter Name=""arg1"" Nullable=""false"" Type=""Edm.Int32"" />
    <ReturnType Type=""Edm.Int32"" />
  </Function>
  <Function Name=""GetCustomerCount"">
    <ReturnType Type=""Edm.Int32"" />
  </Function>
  <Function Name=""GetPrimitiveString"">
    <ReturnType Type=""Edm.String"" />
  </Function>
  <Function Name=""GetSpecificCustomer"">
    <Parameter Name=""Name"" Nullable=""false"" Type=""Edm.String"" />
    <ReturnType Type=""Collection(DefaultNamespace.Customer)"" />
  </Function>
  <Action Name=""SetArgumentPlusOne"">
    <Parameter Name=""arg1"" Nullable=""false"" Type=""Edm.Int32"" />
    <ReturnType Type=""Edm.Int32"" />
  </Action>
</Schema>";

        IEnumerable<XElement> testCsdl = (new string[] { csdl }).Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var actualXElements = this.GetSerializerResult(model).Select(n => XElement.Parse(n));
        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelODataTestModelWithSingleEntityType(EdmVersion edmVersion)
    {
        EdmModel model = new EdmModel();

        EdmEntityType singleton = new EdmEntityType("TestModel", "SingletonEntityType");
        EdmStructuralProperty singletonId = singleton.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        singleton.AddKeys(singletonId);
        EdmStructuralProperty singletonName = singleton.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
        model.AddElement(singleton);

        EdmEntityContainer container = new EdmEntityContainer("TestModel", "DefaultContainer");
        EdmEntitySet singletonSet = container.AddEntitySet("SingletonEntityType", singleton);

        model.AddElement(container);

        var csdl = @"
<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"">
        <EntitySet Name=""SingletonEntityType"" EntityType=""TestModel.SingletonEntityType"" />
    </EntityContainer>
    <EntityType Name=""SingletonEntityType"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""String"" />
    </EntityType>
</Schema>";

        IEnumerable<XElement> testCsdl = (new string[] { csdl }).Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var actualXElements = this.GetSerializerResult(model).Select(n => XElement.Parse(n));
        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelODataTestModelWithSingleComplexType(EdmVersion edmVersion)
    {
        EdmModel model = new EdmModel();

        EdmEntityContainer container = new EdmEntityContainer("TestModel", "DefaultContainer");
        model.AddElement(container);

        EdmComplexType singleton = new EdmComplexType("TestModel", "SingletonComplexType");
        EdmStructuralProperty city = singleton.AddStructuralProperty("City", EdmCoreModel.Instance.GetString(true));
        model.AddElement(singleton);

        var csdl = @"
<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"" />
    <ComplexType Name=""SingletonComplexType"">
        <Property Name=""City"" Type=""String"" />
    </ComplexType>
</Schema>";

        IEnumerable<XElement> testCsdl = (new string[] { csdl }).Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var actualXElements = this.GetSerializerResult(model).Select(n => XElement.Parse(n));
        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelODataTestModelWithMultiValueProperty(EdmVersion edmVersion)
    {
        EdmModel model = new EdmModel();

        EdmEntityContainer container = new EdmEntityContainer("TestModel", "DefaultContainer");
        model.AddElement(container);

        EdmComplexType collectionType = new EdmComplexType("TestModel", "EntityTypeWithCollection");
        EdmStructuralProperty cities = collectionType.AddStructuralProperty("Cities", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
        model.AddElement(collectionType);

        var csdl = @"
<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"" />
    <ComplexType Name=""EntityTypeWithCollection"">
        <Property Name=""Cities"" Type=""Collection(Edm.String)"" />
    </ComplexType>
</Schema>";

        IEnumerable<XElement> testCsdl = (new string[] { csdl }).Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var actualXElements = this.GetSerializerResult(model).Select(n => XElement.Parse(n));
        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelODataTestModelWithOpenType(EdmVersion edmVersion)
    {
        EdmModel model = new EdmModel();

        EdmEntityType openEntity = new EdmEntityType("TestModel", "OpenEntityType", null, false, true);
        EdmStructuralProperty openEntityId = openEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        openEntity.AddKeys(openEntityId);
        model.AddElement(openEntity);

        EdmEntityContainer container = new EdmEntityContainer("TestModel", "DefaultContainer");
        EdmEntitySet openSet = container.AddEntitySet("OpenEntityType", openEntity);
        model.AddElement(container);

        var csdl = @"
<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"">
        <EntitySet Name=""OpenEntityType"" EntityType=""TestModel.OpenEntityType"" />
    </EntityContainer>
    <EntityType OpenType=""true"" Name=""OpenEntityType"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    </EntityType>
</Schema>";

        IEnumerable<XElement> testCsdl = (new string[] { csdl }).Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var actualXElements = this.GetSerializerResult(model).Select(n => XElement.Parse(n));
        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void ConstructibleModelODataTestModelWithNamedStream(EdmVersion edmVersion)
    {
        EdmModel model = new EdmModel();

        EdmEntityType streamEntity = new EdmEntityType("TestModel", "NamedStreamEntityType");
        EdmStructuralProperty streamId = streamEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        streamEntity.AddKeys(streamId);
        EdmStructuralProperty namedStream = streamEntity.AddStructuralProperty("NamedStream", EdmCoreModel.Instance.GetStream(false));
        model.AddElement(streamEntity);

        EdmEntityContainer container = new EdmEntityContainer("TestModel", "DefaultContainer");
        EdmEntitySet streamSet = container.AddEntitySet("NamedStreamEntityType", streamEntity);
        model.AddElement(container);

        var csdl = @"
<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""DefaultContainer"">
        <EntitySet Name=""NamedStreamEntityType"" EntityType=""TestModel.NamedStreamEntityType"" />
    </EntityContainer>
    <EntityType Name=""NamedStreamEntityType"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""NamedStream"" Type=""Stream"" Nullable=""false"" />
    </EntityType>
</Schema>";

        IEnumerable<XElement> testCsdl = (new string[] { csdl }).Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
        var actualXElements = this.GetSerializerResult(model).Select(n => XElement.Parse(n));
        this.Compare(tempCsdls.ToList(), actualXElements.ToList());
    }

    private static IEdmModel GetParserResult(IEnumerable<XElement> csdlElements, params IEdmModel[] referencedModels)
    {
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), referencedModels, out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.False(errors.Any());

        return edmModel;
    }

    private void Compare(List<XElement> expectXElements, List<XElement> actualXElements)
    {
        Assert.Equal(expectXElements.Count, actualXElements.Count);

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        // compare just the EntityContainers
        this.CsdlXElementComparer.Compare(expectedContainers, actualContainers);

        foreach (var expectXElement in expectXElements)
        {
            var schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace")?.Value));

            Assert.NotNull(actualXElement);

            this.CsdlXElementComparer.Compare(expectXElement, actualXElement);
        }
    }

    private IEnumerable<XElement> RemoveImmediateAnnotation(IEnumerable<XElement> csdls)
    {
        var newCsdls = csdls.ToList();
        foreach (var csdl in newCsdls)
        {
            RemoveImmediateAnnotation(csdl);
        }
        return newCsdls;
    }

    private void RemoveImmediateAnnotation(XElement csdl)
    {
        var immediateAnnotations = csdl.Attributes().Where(n => !string.IsNullOrEmpty(n.Name.NamespaceName));
        while (immediateAnnotations.Count() > 0)
        {
            csdl.Attributes(immediateAnnotations.ElementAt(0).Name).Remove();
        }

        foreach (var element in csdl.Elements())
        {
            RemoveImmediateAnnotation(element);
        }
    }
}
