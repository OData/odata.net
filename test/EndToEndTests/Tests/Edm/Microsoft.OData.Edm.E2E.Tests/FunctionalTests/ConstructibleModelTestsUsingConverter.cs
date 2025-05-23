//---------------------------------------------------------------------
// <copyright file="ConstructibleModelTestsUsingConverter.cs" company="Microsoft">
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

public class ConstructibleModelTestsUsingConverter : EdmLibTestCaseBase
{
	[Theory]
	[InlineData(EdmVersion.V40)]
	[InlineData(EdmVersion.V401)]
	public void AssociationForeignKeyWithNavigationProperty_ShouldSerializeAndDeserializeCorrectly(EdmVersion edmVersion)
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
		var navProp1 = new EdmNavigationPropertyInfo { Name = "ToOrders", Target = orderType, TargetMultiplicity = EdmMultiplicity.Many };
		var navProp2 = new EdmNavigationPropertyInfo { Name = "ToCustomer", Target = customerType, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { customerIdProperty }, PrincipalProperties = customerType.Key() };
		customerType.AddBidirectionalNavigation(navProp1, navProp2);

		// Act
		IEnumerable<XElement> testCsdl = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).Select(XElement.Parse);
		var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
		var convertedModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));

		var expectXElements = tempCsdls.ToList();
		var actualXElements = this.GetSerializerResult(convertedModel).Select(n => XElement.Parse(n)).ToList();

		// Assert
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

	[Theory]
	[InlineData(EdmVersion.V40)]
	[InlineData(EdmVersion.V401)]
	public void SerializeDeserialize_IndependentAssociationNavigationProperties_ShouldMatch(EdmVersion edmVersion)
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

		var expectXElements = tempCsdls.ToList();
		var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n)).ToList();
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

	[Theory]
	[InlineData(EdmVersion.V40)]
	[InlineData(EdmVersion.V401)]
	public void SerializeDeserialize_AssociationWithOnDeleteAction_ShouldMatch(EdmVersion edmVersion)
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

		var expectXElements = tempCsdls.ToList();
		var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n)).ToList();
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

	[Theory]
	[InlineData(EdmVersion.V40)]
	[InlineData(EdmVersion.V401)]
	public void SerializeDeserialize_AssociationWithForeignKey_ShouldMatch(EdmVersion edmVersion)
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

		var expectXElements = tempCsdls.ToList();
		var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n)).ToList();
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

	[Theory]
	[InlineData(EdmVersion.V40)]
	[InlineData(EdmVersion.V401)]
	public void SerializeDeserialize_AssociationWithCompositeForeignKey_ShouldMatch(EdmVersion edmVersion)
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

		var expectXElements = tempCsdls.ToList();
		var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n)).ToList();
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

	[Theory]
	[InlineData(EdmVersion.V40)]
	[InlineData(EdmVersion.V401)]
	public void SerializeDeserialize_EntityContainerWithEntitySets_ShouldMatch(EdmVersion edmVersion)
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

		var expectXElements = tempCsdls.ToList();
		var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n)).ToList();
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

	[Theory]
	[InlineData(EdmVersion.V40)]
	[InlineData(EdmVersion.V401)]
	public void SerializeDeserialize_EntityContainerWithFunctionImports_ShouldMatch(EdmVersion edmVersion)
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

		var expectXElements = tempCsdls.ToList();
		var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n)).ToList();
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

	[Theory]
	[InlineData(EdmVersion.V40)]
	[InlineData(EdmVersion.V401)]
	public void SerializeDeserialize_EntityInheritanceTree_ShouldMatch(EdmVersion edmVersion)
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

		var expectXElements = tempCsdls.ToList();
		var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n)).ToList();
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

	[Theory]
	[InlineData(EdmVersion.V40)]
	[InlineData(EdmVersion.V401)]
	public void SerializeDeserialize_FunctionWithAllParameterTypes_ShouldMatch(EdmVersion edmVersion)
	{
		var model = new EdmModel();
		var function = new EdmFunction("NS1", "FunctionWithAll", EdmCoreModel.Instance.GetInt32(true));
		function.AddParameter("Param1", EdmCoreModel.Instance.GetInt32(false));
		model.AddElement(function);

		IEnumerable<XElement> testCsdl = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).Select(XElement.Parse);
		var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
		var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));

		var expectXElements = tempCsdls.ToList();
		var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n)).ToList();
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

	[Theory]
	[InlineData(EdmVersion.V40)]
	[InlineData(EdmVersion.V401)]
	public void SerializeDeserialize_ModelWithAllConcepts_ShouldMatch(EdmVersion edmVersion)
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

		var expectXElements = tempCsdls.ToList();
		var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n)).ToList();
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

	[Theory]
	[InlineData(EdmVersion.V40)]
	[InlineData(EdmVersion.V401)]
	public void SerializeDeserialize_ModelWithMultipleNamespaces_ShouldMatch(EdmVersion edmVersion)
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

		var expectXElements = tempCsdls.ToList();
		var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n)).ToList();
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

	[Theory]
	[InlineData(EdmVersion.V40)]
	[InlineData(EdmVersion.V401)]
	// [EdmLib] SchemaReader.TryParse uses wrong default values for the string type's facets such as unicode - Fixed
	// [EdmLib] Precision facet should not be required for the temporal and decimal types - Fixed
	public void SerializeDeserialize_ComplexTypeWithAllPrimitiveProperties_ShouldMatch(EdmVersion edmVersion)
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

		var expectXElements = tempCsdls.ToList();
		var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n)).ToList();
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

	[Theory]
	[InlineData(EdmVersion.V40)]
	[InlineData(EdmVersion.V401)]
	public void SerializeDeserialize_ComplexTypeWithNestedComplexTypes_ShouldMatch(EdmVersion edmVersion)
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

		var expectXElements = tempCsdls.ToList();
		var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n)).ToList();
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

	[Theory]
	[InlineData(EdmVersion.V40)]
	[InlineData(EdmVersion.V401)]
	public void SerializeDeserialize_ComplexTypeWithSingleProperty_ShouldMatch(EdmVersion edmVersion)
	{
		var model = new EdmModel();
		var type = new EdmComplexType("NS1", "ComplexType");
		type.AddStructuralProperty("Prop1", EdmPrimitiveTypeKind.Int32);
		model.AddElement(type);

		IEnumerable<XElement> testCsdl = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).Select(XElement.Parse);
		var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
		var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));

		var expectXElements = tempCsdls.ToList();
		var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n)).ToList();
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

	[Theory]
	[InlineData(EdmVersion.V40)]
	[InlineData(EdmVersion.V401)]
	// [EdmLib] SchemaReader.TryParse uses wrong default values for the string type's facets such as unicode - Fixed
	// [EdmLib] Precision facet should not be required for the temporal and decimal types - Fixed
	public void SerializeDeserialize_EntityWithAllPrimitiveProperties_ShouldMatch(EdmVersion edmVersion)
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

		var expectXElements = tempCsdls.ToList();
		var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n)).ToList();
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

	[Theory]
	[InlineData(EdmVersion.V40)]
	[InlineData(EdmVersion.V401)]
	public void SerializeDeserialize_EntityWithSingleProperty_ShouldMatch(EdmVersion edmVersion)
	{
		var model = new EdmModel();
		var type = new EdmEntityType("NS1", "Person");
		type.AddKeys(type.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
		model.AddElement(type);

		IEnumerable<XElement> testCsdl = this.GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> errors).Select(XElement.Parse);
		var tempCsdls = new List<XElement>(testCsdl.Select(n => new XElement(n)));
		var stockModel = (new EdmToStockModelConverter()).ConvertToStockModel(GetParserResult(tempCsdls));

		var expectXElements = tempCsdls.ToList();
		var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n)).ToList();
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

	[Theory]
	[InlineData(EdmVersion.V40)]
	[InlineData(EdmVersion.V401)]
	public void SerializeDeserialize_AllPrimitiveTypesInEntityAndComplexType_ShouldMatch(EdmVersion edmVersion)
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

		var expectXElements = tempCsdls.ToList();
		var actualXElements = this.GetSerializerResult(stockModel).Select(n => XElement.Parse(n)).ToList();
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

	private static IEdmModel GetParserResult(IEnumerable<XElement> csdlElements, params IEdmModel[] referencedModels)
	{
		var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), referencedModels, out IEdmModel edmModel, out IEnumerable<EdmError> errors);
		Assert.True(isParsed);
		Assert.False(errors.Any());

		return edmModel;
	}
}
