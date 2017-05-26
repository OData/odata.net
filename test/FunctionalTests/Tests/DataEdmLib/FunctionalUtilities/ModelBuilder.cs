//---------------------------------------------------------------------
// <copyright file="ModelBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.Test.OData.Utils.Common;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.Test.OData.Utils.ODataLibTest;

    public static class ModelBuilder
    {
        public static IEnumerable<IEdmPrimitiveTypeReference> AllPrimitiveEdmTypes(EdmVersion edmVersion, bool isNullable)
        {
            IEnumerable<IEdmPrimitiveTypeReference> primitiveTypes = AllNonSpatialPrimitiveEdmTypes(isNullable);
            if (edmVersion >= EdmVersion.V40)
            {
                primitiveTypes = primitiveTypes.Concat(AllSpatialEdmTypes(isNullable));
            }

            return primitiveTypes;
        }

        public static IEdmPrimitiveTypeReference[] AllNonSpatialPrimitiveEdmTypes(bool nullable = false)
        {
            return new[] 
            {
                EdmCoreModel.Instance.GetBinary(nullable),
                EdmCoreModel.Instance.GetBoolean(nullable),
                EdmCoreModel.Instance.GetByte(nullable),
                new EdmTemporalTypeReference(EdmCoreModel.Instance.GetDateTimeOffset(nullable).PrimitiveDefinition(), nullable, null),
                new EdmDecimalTypeReference(EdmCoreModel.Instance.GetDecimal(nullable).PrimitiveDefinition(), nullable),
                EdmCoreModel.Instance.GetDouble(nullable),
                EdmCoreModel.Instance.GetGuid(nullable),
                EdmCoreModel.Instance.GetInt16(nullable),
                EdmCoreModel.Instance.GetInt32(nullable),
                EdmCoreModel.Instance.GetInt64(nullable),
                EdmCoreModel.Instance.GetSByte(nullable),
                EdmCoreModel.Instance.GetSingle(nullable),
                EdmCoreModel.Instance.GetStream(nullable),
                EdmCoreModel.Instance.GetString(nullable),
                EdmCoreModel.Instance.GetDuration(nullable),
                EdmCoreModel.Instance.GetDate(nullable),
                EdmCoreModel.Instance.GetTimeOfDay(nullable),
            };
        }

        public static IEdmSpatialTypeReference[] AllSpatialEdmTypes(bool nullable = false)
        {
            return new[] 
            {
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, nullable),
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, nullable),
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPolygon, nullable),
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyLineString, nullable),
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyCollection, nullable),
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiPolygon, nullable),
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiLineString, nullable),
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiPoint, nullable),
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geometry, nullable),
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, nullable),
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPolygon, nullable),
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryLineString, nullable),
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryCollection, nullable),
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryMultiPolygon, nullable),
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryMultiLineString, nullable),
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryMultiPoint, nullable),
            };
        }

        public static IEdmModel OneEntityWithOnePropertyEdm()
        {
            var model = new EdmModel();
            var type = new EdmEntityType("NS1", "Person");
            type.AddKeys(type.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            model.AddElement(type);

            return model;
        }

        [ValidationTestInvalidModel]
        public static IEdmModel OneEntityWithAllPrimitivePropertyEdm()
        {
            var model = new EdmModel();
            var type = new EdmEntityType("NS1", "Person");
            type.AddKeys(type.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));

            int i = 0;
            foreach (var primitiveType in AllNonSpatialPrimitiveEdmTypes())
            {
                type.AddStructuralProperty("prop_" + (i++), primitiveType);
            }

            model.AddElement(type);
            return model;
        }

        public static IEdmModel OneEntityWithAllSpatialPropertyEdm()
        {
            var model = new EdmModel();
            var type = new EdmEntityType("NS1", "Foo");
            type.AddKeys(type.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));

            int i = 0;
            foreach (var spatialType in AllSpatialEdmTypes())
            {
                type.AddStructuralProperty("prop_" + (i++), spatialType);
            }

            model.AddElement(type);
            return model;
        }

        public static IEdmModel OneComplexWithOnePropertyEdm()
        {
            var model = new EdmModel();
            var type = new EdmComplexType("NS1", "ComplexType");
            type.AddStructuralProperty("Prop1", EdmPrimitiveTypeKind.Int32);
            model.AddElement(type);
            return model;
        }

        [ValidationTestInvalidModel]
        public static IEdmModel OneComplexWithAllPrimitivePropertyEdm()
        {
            var model = new EdmModel();
            var type = new EdmEntityType("NS1", "Person");
            type.AddKeys(type.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));

            var complexType = new EdmComplexType("NS1", "ComplexThing");

            int i = 0;
            foreach (var primitiveType in AllNonSpatialPrimitiveEdmTypes())
            {
                complexType.AddStructuralProperty("prop_" + (i++), primitiveType);
            }

            model.AddElement(complexType);
            type.AddStructuralProperty("ComplexProperty", new EdmComplexTypeReference(complexType, false));
            model.AddElement(type);
            return model;
        }

        public static IEdmModel OneComplexWithAllSpatialPropertyEdm()
        {
            var model = new EdmModel();
            var type = new EdmEntityType("NS1", "Foo");
            type.AddKeys(type.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));

            var complexType = new EdmComplexType("NS1", "ComplexThing");

            int i = 0;
            foreach (var spatialType in AllSpatialEdmTypes())
            {
                complexType.AddStructuralProperty("prop_" + (i++), spatialType);
            }

            type.AddStructuralProperty("ComplexProperty", new EdmComplexTypeReference(complexType, false));
            model.AddElement(complexType);
            model.AddElement(type);
            return model;
        }

        public static IEdmModel OneComplexWithNestedComplexEdm()
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

            return model;
        }

        public static IEdmModel MultipleNamespacesEdm()
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

            return model;
        }

        public static IEdmModel EntityInheritanceTreeEdm()
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

            return model;
        }

        public static IEdmModel EntityContainerWithEntitySetsEdm()
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

            return model;
        }

        public static IEdmModel AssociationIndependentEdm()
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

            return model;
        }

        public static IEdmModel AssociationFkEdm()
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

            return model;
        }

        public static IEdmModel AssociationFkWithNavigationEdm()
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

            return model;
        }

        public static IEdmModel AssociationCompositeFkEdm()
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

            return model;
        }

        public static IEdmModel EntityContainerWithOperationImportsEdm()
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
            return model;
        }

        public static IEdmModel FunctionWithAllEdm()
        {
            var model = new EdmModel();
            var function = new EdmFunction("NS1", "FunctionWithAll", EdmCoreModel.Instance.GetInt32(true));
            function.AddParameter("Param1", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(function);

            return model;
        }

        [ValidationTestInvalidModel()]
        public static IEdmModel ModelWithAllConceptsEdm()
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

        [ValidationTestInvalidModel()]
        public static IEdmModel ModelWithDefaultEnumEdm()
        {
            var defaultEnum = new EdmEnumType("NS1", "Color", EdmPrimitiveTypeKind.Int32, false);
            defaultEnum.AddMember("Red", new EdmEnumMemberValue(0));
            defaultEnum.AddMember("Orange", new EdmEnumMemberValue(1));
            defaultEnum.AddMember("Yellow", new EdmEnumMemberValue(2));
            defaultEnum.AddMember("Green", new EdmEnumMemberValue(3));
            defaultEnum.AddMember("Cyan", new EdmEnumMemberValue(4));
            defaultEnum.AddMember("Blue", new EdmEnumMemberValue(5));
            defaultEnum.AddMember("Purple", new EdmEnumMemberValue(6));

            return ModelWithEnumEdm(defaultEnum);
        }

        [ValidationTestInvalidModel]
        [CustomCsdlSchemaCompliantTest]
        public static IEdmModel ModelWithEnumEdm(params IEdmEnumType[] enumTypes)
        {
            var model = new EdmModel();
            var complexType = new EdmComplexType("NS1", "Complex");
            model.AddElement(complexType);
            var entityType = new EdmEntityType("NS1", "Person");
            model.AddElement(entityType);
            var container = new EdmEntityContainer("NS1", "MyContainer");
            model.AddElement(container);

            int counter = 0;
            foreach (var enumType in enumTypes)
            {
                var enumTypeReference = new EdmEnumTypeReference(enumType, false);
                model.AddElement(enumType);
                complexType.AddStructuralProperty("EnumProperty" + counter, enumTypeReference);
                entityType.AddKeys(entityType.AddStructuralProperty("EnumProperty" + counter, enumTypeReference));
                var function = new EdmFunction("NS1", "Function" + counter, enumTypeReference);
                model.AddElement(function);
                container.AddFunctionImport(function);
                function.AddParameter("Param" + counter, enumTypeReference);
                counter++;
            }

            return model;
        }

        public static IEnumerable<XElement> StringWithFacets()
        {
            XElement csdlElement = XElement.Parse(@"
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
            return new XElement[] { csdlElement };
        }

        [ValidationTestInvalidModel]
        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> ComplexTypeWithBaseType(EdmVersion edmVersion)
        {
            var csdls = new List<string>(3);
            csdls.Add(@"
<Schema Namespace='FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.third' xmlns='$EdmNamespace$'>
  <ComplexType Name='VALIDeCOMPLEXtYPE1' BaseType='a.ValidComplexType2' />
  <ComplexType Name='V1alidcomplexType'>
    <Property Name='aPropertyOne' Type='FindMethodsTestModelBuilder.MultipleSchemasWithUsings.second.V1alidcomplexType' Nullable='false' />
  </ComplexType>
  <EntityContainer Name='ValidEntityContainer2' />
</Schema> ");
            csdls.Add(@"<Schema Namespace='FindMethodsTestModelBuilder.MultipleSchemasWithUsings.second' xmlns='$EdmNamespace$'>
  <ComplexType Name='ValidComplexType2' BaseType='FindMethodsTestModelBuilder.MultipleSchemasWithUsings.first.validComplexType1' />
  <ComplexType Name='V1alidcomplexType'>
    <Property Name='aPropertyOne' Type='FindMethodsTestModelBuilder.MultipleSchemasWithUsings.first.V1alidcomplexType' Nullable='false' />
  </ComplexType>
</Schema>");
            csdls.Add(@"
 <Schema Namespace='FindMethodsTestModelBuilder.MultipleSchemasWithDerivedTypes.first' xmlns='$EdmNamespace$'>
  <ComplexType Name='validComplexType1'>
    <Property Name='Id' Type='Int32' Nullable='false' />
  </ComplexType>
  <ComplexType Name='V1alidcomplexType' />
</Schema>");
            var temp = csdls.Select(n => n.Replace("$EdmNamespace$", EdmLibCsdlContentGenerator.GetCsdlFullNamespace(edmVersion).NamespaceName));
            return from t in temp
                   select XElement.Parse(t);
        }

        public static IEdmModel AssociationOnDeleteModelEdm()
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

            return model;
        }

        public static IEdmModel TaupoDefaultModelEdm()
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

            model.AddDefaultContainerFixup("NS1");
            return model;
        }

        [ValidationTestInvalidModel]
        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> SimpleAllPrimitiveTypes(EdmVersion edmVersion, bool explictNullable, bool isNullable)
        {
            var namespaceName = "ModelBuilder.SimpleAllPrimitiveTypes";

            var model = new EdmModel();
            var entityType = new EdmEntityType(namespaceName, "validEntityType1");
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false));
            model.AddElement(entityType);

            var complexType = new EdmComplexType(namespaceName, "V1alidcomplexType");
            model.AddElement(complexType);

            int i = 0;
            bool typesAreNullable = !explictNullable && isNullable;
            foreach (var primitiveType in AllPrimitiveEdmTypes(edmVersion, typesAreNullable))
            {
                entityType.AddStructuralProperty("Property" + i++, primitiveType);
                complexType.AddStructuralProperty("Property" + i++, primitiveType);
            }

            var stringBuilder = new StringBuilder();
            var xmlWriter = XmlWriter.Create(stringBuilder);
            IEnumerable<EdmError> errors;
            if (!model.TryWriteSchema((s) => xmlWriter, out errors) || errors.Any())
            {
                ExceptionUtilities.Assert(false, "Failed to write CSDL: " + string.Join(",", errors.Select(e => e.ErrorMessage)));
            }

            xmlWriter.Close();
            var csdlElements = new[] { XElement.Parse(stringBuilder.ToString()) };

            if (explictNullable)
            {
                ModelBuilderHelpers.SetNullableAttributes(csdlElements, isNullable);
            }

            return csdlElements;
        }

        public static IEnumerable<XElement> SimpleConstructiveApiTestModel()
        {
            var csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Westwind"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Customer"">
    <Key>
      <PropertyRef Name=""IDC"" />
    </Key>
    <Property Name=""IDC"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" Unicode=""true"" p3:Stumble=""Rumble"" p3:Tumble=""Bumble"" xmlns:p3=""Grumble"">
      <Documentation>
        <Summary>Foolishness rules.</Summary>
        <LongDescription>Wisdom drools.</LongDescription>
      </Documentation>
    </Property>
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
            return new XElement[] { XElement.Parse(csdl) };
        }

        [ValidationTestInvalidModel]
        public static IEdmModel ValidNameCheckModelEdm() // M1, M1 - M7 stand for each reference test model in the test plan. 
        {
            /* 
             * SimpleIdentifier Pattern : value="[\p{L}\p{Nl}][\p{L}\p{Nl}\p{Nd}\p{Mn}\p{Mc}\p{Pc}\p{Cf}]{0,}"
             * \u1F98 - Letter, Titlecase
             * \u06E5 - Letter, Modifier
             * \u01C0 - Letter, Other
             * \u0660 - Digital, Decmial
             * \u0300 - Mark, Nonspacing
             * \u0903 - Mark, Spacing Combining
             * \u2040 - Punctuation, Connector
             * \u00AD - Other, Format 
             * \u2160 - Number, Letter
             */
            /*
             * QualifiedName Pattern : value="[\p{L}\p{Nl}][\p{L}\p{Nl}\p{Nd}\p{Mn}\p{Mc}\p{Pc}\p{Cf}]{0,}(\.[\p{L}\p{Nl}][\p{L}\p{Nl}\p{Nd}\p{Mn}\p{Mc}\p{Pc}\p{Cf}]{0,}){0,}"
             */
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

            return model;
        }

        [ValidationTestInvalidModel, CustomCsdlSchemaCompliantTest]
        public static IEdmModel PropertyFacetsCollectionEdm()
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
            return model;
        }

        [ValidationTestInvalidModel]
        public static IEdmModel ComplexTypeAttributesEdm()
        {
            var model = new EdmModel();

            var baseComplexType = new EdmComplexType("MyNamespace", "MyBaseComplexType");
            model.AddElement(baseComplexType);

            var complexType = new EdmComplexType("MyNamespace", "MyComplexType", baseComplexType, true);
            model.AddElement(complexType);

            return model;
        }

        public static IEnumerable<XElement> TypeRefFacets()
        {
            XElement csdlElement = XElement.Parse(@"
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
            return new XElement[] { csdlElement };
        }

        public static IEnumerable<XElement> EntityContainerAttributes()
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

            return new XElement[] { csdlElement1, csdlElement2 };
        }

        [ValidationTestInvalidModel]
        public static IEnumerable<XElement> CollectionTypesWithSimpleType()
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
            return new XElement[] { csdlElement };
        }

        [ValidationTestInvalidModel]
        public static IEnumerable<XElement> CollectionTypes()
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
            return new XElement[] { csdlElement };
        }

        public static IEnumerable<XElement> DependentPropertiesWithNoPartnerProperty()
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
            return new XElement[] { XElement.Parse(csdl) };
        }

        [ValidationTestInvalidModel]
        public static IEnumerable<XElement> InlineCollectionAtomic()
        {
            var csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""InlineCollection"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""ComplexTypeA"">
    <Property Name=""Id"" Type=""Int32""/>
    <Property Name=""Collection"" Type=""Collection(Edm.Int32)""/>
    <Property Name=""Collection"" Type=""Collection(Edm.Int32)""/>
  </ComplexType>
  <EntityType Name=""EntityTypeA"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Collection"" Type=""Collection(Edm.Int32)""/>
    <Property Name=""Collection"" Type=""Collection(Edm.Int32)""/>
  </EntityType>
  <Function Name=""FunctionA""><ReturnType Type=""Collection(Edm.Int32)""/>
    <Parameter Name=""Param1"" Type=""Collection(Edm.Int32)"" Nullable=""false"" />
    <Parameter Name=""Param5"" Type=""Collection(Edm.Int32)"" Nullable=""false"" />
  </Function>
  <Function Name=""FunctionB""><ReturnType Type=""Collection(Edm.Int32)""/>
  </Function>
  <EntityContainer Name=""MyContainer"">
    <EntitySet Name=""CustomerSet"" EntityType=""InlineCollectionAtomic.EntityTypeA"" />
    <FunctionImport Name=""FunctionImportA"" Function=""InlineCollection.FunctionA"" EntitySet=""CustomerSet"" />
    <FunctionImport Name=""FunctionImportB"" Function=""InlineCollection.FunctionB"" EntitySet=""CustomerSet"" />
  </EntityContainer>
</Schema>";

            return new XElement[] { XElement.Parse(csdl) };
        }

        [ValidationTestInvalidModel]
        public static IEnumerable<XElement> ElementCollectionAtomic()
        {
            var csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""ElementCollectionAtomic"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Function Name=""FunctionA"">
    <Parameter Name=""Param3"">
        <CollectionType>
            <TypeRef  Type=""Collection(Int32)""/>
        </CollectionType>
    </Parameter>
    <ReturnType Type='Collection(Edm.Int32)'/>
  </Function>
</Schema>";

            return new XElement[] { XElement.Parse(csdl) };
        }

        public static IEnumerable<XElement> FunctionReturnTypeWithFacets()
        {
            var csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""ElementCollectionAtomic"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Function Name=""FunctionA"">
    <ReturnType Type=""String"" Nullable=""false"" MaxLength=""1023"">
    </ReturnType>
  </Function>
</Schema>";

            return new XElement[] { XElement.Parse(csdl) };
        }

        [CustomCsdlSchemaCompliantTest, ValidationTestInvalidModel]
        public static IEnumerable<XElement> OperationReturnTypeWithDuplicateFacets()
        {
            var csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""ElementCollectionAtomic"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Function Name=""FunctionA"" Nullable=""true"" MaxLength=""25"">
    <ReturnType Type=""String"" Nullable=""false"" MaxLength=""1023"" >
    </ReturnType>
  </Function>
</Schema>";

            return new XElement[] { XElement.Parse(csdl) };
        }

        [CustomCsdlSchemaCompliantTest, ValidationTestInvalidModel]
        public static IEnumerable<XElement> FunctionDuplicateReturnType()
        {
            var csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""ElementCollectionAtomic"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Function Name=""FunctionA""><ReturnType Type=""String"" Nullable=""true"" MaxLength=""25""/>
    <ReturnType Type=""String"" Nullable=""false"" MaxLength=""1023"" >
    </ReturnType>
  </Function>
</Schema>";

            return new XElement[] { XElement.Parse(csdl) };
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> RemovableElements()
        {
            var csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""ElementCollectionAtomic"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Age"" Type=""Edm.Int64"" />
    <EntityType Name=""PersonAddress"" OpenType=""true"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
    <EnumType Name=""Popularity"" UnderlyingType=""Edm.Int32"">
        <Annotation Term=""ElementCollectionAtomic.Age"" Int=""22"" />
    </EnumType>
    <Function Name=""FunctionA"">
        <ReturnType Type=""String"" Nullable=""false"" MaxLength=""1023"">
        </ReturnType>
    </Function>
</Schema>";
            return new XElement[] { XElement.Parse(csdl) };
        }

        public static IEnumerable<XElement> CollectionExpression()
        {
            var csdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""StringValue"" Type=""Collection(Edm.String)"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
    </EntityType>
    <Annotations Target=""NS1.Person"">
        <Annotation Term=""NS1.StringValue"">
            <Collection>
                <String>s1</String>
                <String>s2</String>
            </Collection>
        </Annotation>
    </Annotations>
</Schema>";
            return new XElement[] { XElement.Parse(csdl) };
        }
    }
}
