//---------------------------------------------------------------------
// <copyright file="TestModels.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.EntityModel;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.EntityModel;
    using Microsoft.Test.Taupo.EntityModel.Edm;
    #endregion Namespaces

    /// <summary>
    /// A helper class to create our test models using the <see cref="ModelBuilder"/>.
    /// </summary>
    public static class TestModels
    {
        /// <summary>
        /// Build a test model shared across several tests.
        /// </summary>
        /// <returns>Returns the test model.</returns>
        public static EntityModelSchema BuildTestModel()
        {
            // The metadata model
            EntityModelSchema model = new EntityModelSchema().MinimumVersion(ODataVersion.V4);

            ComplexType addressType = model.ComplexType("Address")
                .Property("Street", EdmDataTypes.String().Nullable())
                .Property("Zip", EdmDataTypes.Int32);

            EntityType officeType = model.EntityType("OfficeType")
                .KeyProperty("Id", EdmDataTypes.Int32)
                .Property("Address", DataTypes.ComplexType.WithDefinition(addressType));

            EntityType officeWithNumberType = model.EntityType("OfficeWithNumberType")
                .WithBaseType(officeType)
                .Property("Number", EdmDataTypes.Int32);

            EntityType cityType = model.EntityType("CityType")
                .KeyProperty("Id", EdmDataTypes.Int32)
                .Property("Name", EdmDataTypes.String().Nullable())
                .NavigationProperty("CityHall", officeType)
                .NavigationProperty("DOL", officeType)
                .NavigationProperty("PoliceStation", officeType, true)
                .StreamProperty("Skyline")
                .Property("MetroLanes", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.String().Nullable()));

            EntityType cityWithMapType = model.EntityType("CityWithMapType")
                .WithBaseType(cityType)
                .DefaultStream();

            EntityType cityOpenType = model.EntityType("CityOpenType")
                .WithBaseType(cityType)
                .OpenType();

            EntityType personType = model.EntityType("Person")
                .KeyProperty("Id", EdmDataTypes.Int32);
            personType = personType.NavigationProperty("Friend", personType);

            EntityType employeeType = model.EntityType("Employee")
                .WithBaseType(personType)
                .Property("CompanyName", EdmDataTypes.String().Nullable());

            EntityType managerType = model.EntityType("Manager")
                .WithBaseType(employeeType)
                .Property("Level", EdmDataTypes.Int32);

            model.Add(new EntityContainer("DefaultContainer"));

            model.EntitySet("Offices", officeType);
            model.EntitySet("Cities", cityType);
            model.EntitySet("Persons", personType);

            model.Fixup();

            // Fixed up models will have entity sets for all base entity types.
            model.EntitySet("Employee", employeeType);
            model.EntitySet("Manager", managerType);

            EntityContainer container = model.EntityContainers.Single();

            // NOTE: Function import parameters and return types must be nullable as per current CSDL spec
            FunctionImport serviceOp = container.FunctionImport("ServiceOperation1")
                .Parameter("a", EdmDataTypes.Int32.Nullable())
                .Parameter("b", EdmDataTypes.String().Nullable())
                .ReturnType(EdmDataTypes.Int32.Nullable());

            container.FunctionImport("PrimitiveResultOperation")
                .ReturnType(EdmDataTypes.Int32.Nullable());
            container.FunctionImport("ComplexResultOperation")
                .ReturnType(DataTypes.ComplexType.WithDefinition(addressType).Nullable());
            container.FunctionImport("PrimitiveCollectionResultOperation")
                .ReturnType(DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Int32.Nullable()));
            container.FunctionImport("ComplexCollectionResultOperation")
                .ReturnType(DataTypes.CollectionType.WithElementDataType(DataTypes.ComplexType.WithDefinition(addressType).Nullable()));

            // Overload with 0 Param
            container.FunctionImport("FunctionImportWithOverload");

            // Overload with 1 Param
            container.FunctionImport("FunctionImportWithOverload")
                .Parameter("p1", DataTypes.EntityType.WithDefinition(cityWithMapType));

            // Overload with 2 Params
            container.FunctionImport("FunctionImportWithOverload")
                .Parameter("p1", DataTypes.EntityType.WithDefinition(cityType))
                .Parameter("p2", EdmDataTypes.String().Nullable());

            // Overload with 5 Params
            container.FunctionImport("FunctionImportWithOverload")
                .Parameter("p1", DataTypes.CollectionOfEntities(cityType))
                .Parameter("p2", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.String().Nullable()))
                .Parameter("p3", EdmDataTypes.String().Nullable())
                .Parameter("p4", DataTypes.ComplexType.WithDefinition(addressType).Nullable())
                .Parameter("p5", DataTypes.CollectionType.WithElementDataType(DataTypes.ComplexType.WithDefinition(addressType).Nullable()));

            return model;
        }

        /// <summary>
        /// Build a test model shared across several tests.
        /// </summary>
        /// <param name="addAnnotations">true if the annotations should be added upon construction; otherwise false.</param>
        /// <returns>Returns the test model.</returns>
        public static EntityModelSchema BuildODataAnnotationTestModel(bool addAnnotations)
        {
            // The metadata model with OData-specific annotations
            // - default entity container annotation
            // - HasStream annotation on entity type
            // - MimeType annotation on primitive property
            // - MimeType annotation on service operation
            EntityModelSchema model = new EntityModelSchema().MinimumVersion(ODataVersion.V4);

            ComplexType addressType = model.ComplexType("Address")
                .Property("Street", EdmDataTypes.String().Nullable())
                .Property("Zip", EdmDataTypes.Int32);
            if (addAnnotations)
            {
                addressType.Properties.Where(p => p.Name == "Zip").Single().MimeType("text/plain");
            }

            EntityType personType = model.EntityType("PersonType")
                .KeyProperty("Id", EdmDataTypes.Int32)
                .Property("Name", EdmDataTypes.String())
                .Property("Address", DataTypes.ComplexType.WithDefinition(addressType))
                .StreamProperty("Picture")
                .DefaultStream();
            if (addAnnotations)
            {
                personType.Properties.Where(p => p.Name == "Name").Single().MimeType("text/plain");
            }

            model.Fixup();

            // set the default container
            EntityContainer container = model.EntityContainers.Single();

            model.EntitySet("People", personType);

            // NOTE: Function import parameters and return types must be nullable as per current CSDL spec
            FunctionImport serviceOp = container.FunctionImport("ServiceOperation1")
                .Parameter("a", EdmDataTypes.Int32.Nullable())
                .Parameter("b", EdmDataTypes.String().Nullable())
                .ReturnType(EdmDataTypes.Int32.Nullable());
            if (addAnnotations)
            {
                serviceOp.MimeType("img/jpeg");
            }

            return model;
        }

        /// <summary>
        /// Creates a set of models.
        /// </summary>
        /// <returns>List of interesting entity reference link instances.</returns>
        public static IEnumerable<EntityModelSchema> CreateModels()
        {
            //
            // NOTE: we only create a few models here since we mostly rely on EdmLib to test 
            //       model serialization/deserialization for us
            //
            // Empty model
            EntityModelSchema emptyModel = new EntityModelSchema().MinimumVersion(ODataVersion.V4);
            emptyModel.Fixup();
            yield return emptyModel;

            // Model with a single entity type
            EntityModelSchema modelWithSingleEntityType = new EntityModelSchema().MinimumVersion(ODataVersion.V4);
            modelWithSingleEntityType.EntityType("SingletonEntityType")
                .KeyProperty("Id", EdmDataTypes.Int32)
                .Property("Name", EdmDataTypes.String().Nullable());
            modelWithSingleEntityType.Fixup();
            yield return modelWithSingleEntityType;

            // Model with a single complex type
            EntityModelSchema modelWithSingleComplexType = new EntityModelSchema().MinimumVersion(ODataVersion.V4);
            modelWithSingleComplexType.ComplexType("SingletonComplexType")
                .Property("City", EdmDataTypes.String().Nullable());
            modelWithSingleComplexType.Fixup();
            yield return modelWithSingleComplexType;

            // Model with a collection property
            EntityModelSchema modelWithCollectionProperty = new EntityModelSchema().MinimumVersion(ODataVersion.V4);
            modelWithCollectionProperty.ComplexType("EntityTypeWithCollection")
                .Property("Cities", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.String().Nullable()));
            modelWithCollectionProperty.Fixup();
            yield return modelWithCollectionProperty;

            // Model with an open type
            EntityModelSchema modelWithOpenType = new EntityModelSchema().MinimumVersion(ODataVersion.V4);
            EntityType openType = modelWithOpenType.EntityType("OpenEntityType").KeyProperty("Id", EdmDataTypes.Int32);
            openType.IsOpen = true;
            modelWithOpenType.Fixup();
            yield return modelWithOpenType;

            // Model with a named stream
            EntityModelSchema modelWithNamedStream = new EntityModelSchema().MinimumVersion(ODataVersion.V4);
            modelWithNamedStream.EntityType("NamedStreamEntityType")
                .KeyProperty("Id", EdmDataTypes.Int32)
                .StreamProperty("NamedStream");
            modelWithNamedStream.Fixup();
            yield return modelWithNamedStream;

            // OData Shared Test Model
            yield return BuildTestModel();

            // Model with OData-specific attribute annotations
            yield return BuildODataAnnotationTestModel(true);

            // Astoria Default Test Model
            yield return BuildDefaultAstoriaTestModel();
        }

        /// <summary>
        /// Creates a test model shared among parameter reader/writer tests.
        /// </summary>
        /// <returns>Returns a model with function imports.</returns>
        public static IEdmModel BuildModelWithFunctionImport()
        {
            EdmCoreModel coreModel = EdmCoreModel.Instance;
            EdmModel model = new EdmModel();
            const string defaultNamespaceName = "TestModel";
            EdmEntityContainer container = new EdmEntityContainer(defaultNamespaceName, "TestContainer");
            model.AddElement(container);

            EdmComplexType complexType = new EdmComplexType(defaultNamespaceName, "ComplexType");
            complexType.AddProperty(new EdmStructuralProperty(complexType, "PrimitiveProperty", coreModel.GetString(false)));
            complexType.AddProperty(new EdmStructuralProperty(complexType, "ComplexProperty", complexType.ToTypeReference(false)));
            model.AddElement(complexType);

            EdmEnumType enumType = new EdmEnumType(defaultNamespaceName, "EnumType");
            model.AddElement(enumType);

            EdmEntityType entityType = new EdmEntityType(defaultNamespaceName, "EntityType");
            entityType.AddKeys(new IEdmStructuralProperty[] {new EdmStructuralProperty(entityType, "ID", coreModel.GetInt32(false))});
            entityType.AddProperty(new EdmStructuralProperty(entityType, "ComplexProperty", complexType.ToTypeReference()));

            container.AddActionAndActionImport(model, "FunctionImport_Primitive", null /*returnType*/, null /*entitySet*/, false /*bindable*/).Action.AsEdmAction().AddParameter("primitive", coreModel.GetString(false));
            container.AddActionAndActionImport(model, "FunctionImport_NullablePrimitive", null /*returnType*/, null /*entitySet*/, false /*bindable*/).Action.AsEdmAction().AddParameter("nullablePrimitive", coreModel.GetString(true));
            EdmCollectionType stringCollectionType = new EdmCollectionType(coreModel.GetString(true));
            container.AddActionAndActionImport(model, "FunctionImport_PrimitiveCollection", null /*returnType*/, null /*entitySet*/, false /*bindable*/).Action.AsEdmAction().AddParameter("primitiveCollection", stringCollectionType.ToTypeReference(false));
            container.AddActionAndActionImport(model, "FunctionImport_Complex", null /*returnType*/, null /*entitySet*/, false /*bindable*/).Action.AsEdmAction().AddParameter("complex", complexType.ToTypeReference(true));
            EdmCollectionType complexCollectionType = new EdmCollectionType(complexType.ToTypeReference());
            container.AddActionAndActionImport(model, "FunctionImport_ComplexCollection", null /*returnType*/, null /*entitySet*/, false /*bindable*/).Action.AsEdmAction().AddParameter("complexCollection", complexCollectionType.ToTypeReference());
            container.AddActionAndActionImport(model, "FunctionImport_Entry", null /*returnType*/, null /*entitySet*/, true /*bindable*/).Action.AsEdmAction().AddParameter("entry", entityType.ToTypeReference());
            EdmCollectionType entityCollectionType = new EdmCollectionType(entityType.ToTypeReference());
            container.AddActionAndActionImport(model, "FunctionImport_Feed", null /*returnType*/, null /*entitySet*/, true /*bindable*/).Action.AsEdmAction().AddParameter("feed", entityCollectionType.ToTypeReference());
            container.AddActionAndActionImport(model, "FunctionImport_Stream", null /*returnType*/, null /*entitySet*/, false /*bindable*/).Action.AsEdmAction().AddParameter("stream", coreModel.GetStream(false));
            container.AddActionAndActionImport(model, "FunctionImport_Enum", null /*returnType*/, null /*entitySet*/, false /*bindable*/).Action.AsEdmAction().AddParameter("enum", enumType.ToTypeReference());

            var functionImport_PrimitiveTwoParameters = container.AddActionAndActionImport(model, "FunctionImport_PrimitiveTwoParameters", null /*returnType*/, null /*entitySet*/, false /*bindable*/);
            functionImport_PrimitiveTwoParameters.Action.AsEdmAction().AddParameter("p1", coreModel.GetInt32(false));
            functionImport_PrimitiveTwoParameters.Action.AsEdmAction().AddParameter("p2", coreModel.GetString(false));

            container.AddActionAndActionImport(model, "FunctionImport_Int", null /*returnType*/, null /*entitySet*/, false /*bindable*/).Action.AsEdmAction().AddParameter("p1", coreModel.GetInt32(false));
            container.AddActionAndActionImport(model, "FunctionImport_Double", null /*returnType*/, null /*entitySet*/, false /*bindable*/).Action.AsEdmAction().AddParameter("p1", coreModel.GetDouble(false));
            EdmCollectionType int32CollectionType = new EdmCollectionType(coreModel.GetInt32(false));
            container.AddActionAndActionImport(model, "FunctionImport_NonNullablePrimitiveCollection", null /*returnType*/, null /*entitySet*/, false /*bindable*/).Action.AsEdmAction().AddParameter("p1", int32CollectionType.ToTypeReference(false));

            EdmComplexType complexType2 = new EdmComplexType(defaultNamespaceName, "ComplexTypeWithNullableProperties");
            complexType2.AddProperty(new EdmStructuralProperty(complexType2, "StringProperty", coreModel.GetString(true)));
            complexType2.AddProperty(new EdmStructuralProperty(complexType2, "IntegerProperty", coreModel.GetInt32(true)));
            model.AddElement(complexType2);

            var functionImport_MultipleNullableParameters = container.AddActionAndActionImport(model, "FunctionImport_MultipleNullableParameters", null /*returnType*/, null /*entitySet*/, false /*bindable*/);
            var function_MultipleNullableParameters = functionImport_MultipleNullableParameters.Action.AsEdmAction();
            function_MultipleNullableParameters.AddParameter("p1", coreModel.GetBinary(true /*isNullable*/));
            function_MultipleNullableParameters.AddParameter("p2", coreModel.GetBoolean(true /*isNullable*/));
            function_MultipleNullableParameters.AddParameter("p3", coreModel.GetByte(true /*isNullable*/));
            function_MultipleNullableParameters.AddParameter("p5", coreModel.GetDateTimeOffset(true /*isNullable*/));
            function_MultipleNullableParameters.AddParameter("p6", coreModel.GetDecimal(true /*isNullable*/));
            function_MultipleNullableParameters.AddParameter("p7", coreModel.GetDouble(true /*isNullable*/));
            function_MultipleNullableParameters.AddParameter("p8", coreModel.GetGuid(true /*isNullable*/));
            function_MultipleNullableParameters.AddParameter("p9", coreModel.GetInt16(true /*isNullable*/));
            function_MultipleNullableParameters.AddParameter("p10", coreModel.GetInt32(true /*isNullable*/));
            function_MultipleNullableParameters.AddParameter("p11", coreModel.GetInt64(true /*isNullable*/));
            function_MultipleNullableParameters.AddParameter("p12", coreModel.GetSByte(true /*isNullable*/));
            function_MultipleNullableParameters.AddParameter("p13", coreModel.GetSingle(true /*isNullable*/));
            function_MultipleNullableParameters.AddParameter("p14", coreModel.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true /*isNullable*/));
            function_MultipleNullableParameters.AddParameter("p15", coreModel.GetString(true /*isNullable*/));
            function_MultipleNullableParameters.AddParameter("p16", complexType2.ToTypeReference(true /*isNullable*/));

            return model;
        }

        /// <summary>
        /// Builds the Astoria default test model and applies necessary fixups for use in OData tests.
        /// </summary>
        /// <returns>The default Astoria test model.</returns>
        public static EntityModelSchema BuildDefaultAstoriaTestModel()
        {
            var model = new AstoriaDefaultModelGenerator().GenerateModel();
            
            new ApplyDefaultNamespaceFixup("TestNS").Fixup(model);
            new ResolveReferencesFixup().Fixup(model);
            new EntityModelPrimitiveTypeResolver().ResolveProviderTypes(model, new EdmDataTypeResolver());

            return model;
        }

        public static EntityModelSchema BuildSingleEntityTypeEpmModel(int epmCount)
        {
            // Build a model that has a single entity type and the requested number of EPM attributes
            EntityModelSchema modelWithSingleEntityType = new EntityModelSchema().MinimumVersion(ODataVersion.V4);
            EntityType customerType = modelWithSingleEntityType.EntityType("Customer")
                .KeyProperty("Id", EdmDataTypes.Int32);
            AddEpmAttributes(customerType, epmCount, 0);
            modelWithSingleEntityType.Fixup();
            return modelWithSingleEntityType;
        }

        public static EntityModelSchema BuildDerivedEntityTypeEpmModel(int epmCount)
        {
            // Build a model that has type inheritance and the requested number of EPM attributes across all types
            int countPerType = epmCount / 3;
            EntityModelSchema modelWithInheritance = new EntityModelSchema().MinimumVersion(ODataVersion.V4);
            EntityType baseType1 = modelWithInheritance.EntityType("BaseType1").KeyProperty("Id", EdmDataTypes.Int32);
            AddEpmAttributes(baseType1, countPerType, 0);
            EntityType baseType2 = modelWithInheritance.EntityType("BaseType2").WithBaseType(baseType1);
            AddEpmAttributes(baseType2, countPerType, countPerType);
            EntityType customerType = modelWithInheritance.EntityType("Customer").WithBaseType(baseType2);
            AddEpmAttributes(customerType, epmCount - 2 * countPerType, 2 * countPerType);
            modelWithInheritance.Fixup();
            return modelWithInheritance;
        }

        /// <summary>
        /// Creates a test model to test our conversion of OData instances into EDM values.
        /// </summary>
        /// <returns>Returns a model suitable for testing EDM values over OData instances.</returns>
        public static EntityModelSchema BuildEdmValueModel()
        {
            EntityModelSchema model = new EntityModelSchema();
            var complexType = model.ComplexType("ComplexType");
            complexType.Property("IntProp", EdmDataTypes.Int32);
            complexType.Property("StringProp", EdmDataTypes.String());
            complexType.Property("ComplexProp", complexType);

            #region Entity types
            model.EntityContainer("TestContainer");

            // Entity type with a single primitive property
            var singlePrimitivePropertyEntityType = model.EntityType("SinglePrimitivePropertyEntityType");
            singlePrimitivePropertyEntityType.KeyProperty("ID", EdmDataTypes.Int32);
            singlePrimitivePropertyEntityType.Property("Int32Prop", EdmDataTypes.Int32.Nullable());
            model.EntitySet("SinglePrimitivePropertyEntityType", singlePrimitivePropertyEntityType);

            // Entity type with all primitive properties
            var allPrimitivePropertiesEntityType = model.EntityType("AllPrimitivePropertiesEntityType");
            allPrimitivePropertiesEntityType.KeyProperty("ID", EdmDataTypes.Int32);
            allPrimitivePropertiesEntityType.Property("BoolProp", EdmDataTypes.Boolean);
            allPrimitivePropertiesEntityType.Property("Int16Prop", EdmDataTypes.Int16);
            allPrimitivePropertiesEntityType.Property("Int32Prop", EdmDataTypes.Int32);
            allPrimitivePropertiesEntityType.Property("Int64Prop", EdmDataTypes.Int64);
            allPrimitivePropertiesEntityType.Property("ByteProp", EdmDataTypes.Byte);
            allPrimitivePropertiesEntityType.Property("SByteProp", EdmDataTypes.SByte);
            allPrimitivePropertiesEntityType.Property("SingleProp", EdmDataTypes.Single);
            allPrimitivePropertiesEntityType.Property("DoubleProp", EdmDataTypes.Double);
            allPrimitivePropertiesEntityType.Property("DecimalProp", EdmDataTypes.Decimal());
            allPrimitivePropertiesEntityType.Property("DateTimeOffsetProp", EdmDataTypes.DateTimeOffset());
            allPrimitivePropertiesEntityType.Property("DurationProp", EdmDataTypes.Time());
            allPrimitivePropertiesEntityType.Property("GuidProp", EdmDataTypes.Guid);
            allPrimitivePropertiesEntityType.Property("StringProp", EdmDataTypes.String());
            allPrimitivePropertiesEntityType.Property("BinaryProp", EdmDataTypes.Binary());
            model.EntitySet("AllPrimitivePropertiesEntityType", allPrimitivePropertiesEntityType);

            // Entity type with a single complex property
            var singleComplexPropertyEntityType = model.EntityType("SingleComplexPropertyEntityType");
            singleComplexPropertyEntityType.KeyProperty("ID", EdmDataTypes.Int32);
            singleComplexPropertyEntityType.Property("ComplexProp", complexType);
            model.EntitySet("SingleComplexPropertyEntityType", singleComplexPropertyEntityType);

            // Entity type with a single primitive collection property
            var singlePrimitiveCollectionPropertyEntityType = model.EntityType("SinglePrimitiveCollectionPropertyEntityType");
            singlePrimitiveCollectionPropertyEntityType.KeyProperty("ID", EdmDataTypes.Int32);
            singlePrimitiveCollectionPropertyEntityType.Property("PrimitiveCollectionProp", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Int32));
            model.EntitySet("SinglePrimitiveCollectionPropertyEntityType", singlePrimitiveCollectionPropertyEntityType);

            // Entity type with a single primitive collection property
            var singleComplexCollectionPropertyEntityType = model.EntityType("SingleComplexCollectionPropertyEntityType");
            singleComplexCollectionPropertyEntityType.KeyProperty("ID", EdmDataTypes.Int32);
            singleComplexCollectionPropertyEntityType.Property("ComplexCollectionProp", DataTypes.CollectionOfComplex(complexType));
            model.EntitySet("SingleComplexCollectionPropertyEntityType", singleComplexCollectionPropertyEntityType);

            // Entity type with different property kinds
            var differentPropertyKindsEntityType = model.EntityType("DifferentPropertyKindsEntityType");
            differentPropertyKindsEntityType.KeyProperty("ID", EdmDataTypes.Int32);
            differentPropertyKindsEntityType.Property("ComplexProp", complexType);
            differentPropertyKindsEntityType.Property("PrimitiveCollectionProp", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Int32));
            differentPropertyKindsEntityType.Property("Int32Prop", EdmDataTypes.Int32);
            differentPropertyKindsEntityType.Property("ComplexCollectionProp", DataTypes.CollectionOfComplex(complexType));
            model.EntitySet("DifferentPropertyKindsEntityType", differentPropertyKindsEntityType);
            #endregion Entity types

            #region Complex types
            // Empty complex type
            var emptyComplexType = model.ComplexType("EmptyComplexType");

            // Complex type with a single primitive property
            var singlePrimitivePropertyComplexType = model.ComplexType("SinglePrimitivePropertyComplexType");
            singlePrimitivePropertyComplexType.Property("Int32Prop", EdmDataTypes.Int32.Nullable());

            // Complex type with all primitive properties
            var allPrimitivePropertiesComplexType = model.ComplexType("AllPrimitivePropertiesComplexType");
            allPrimitivePropertiesComplexType.Property("BoolProp", EdmDataTypes.Boolean);
            allPrimitivePropertiesComplexType.Property("Int16Prop", EdmDataTypes.Int16);
            allPrimitivePropertiesComplexType.Property("Int32Prop", EdmDataTypes.Int32);
            allPrimitivePropertiesComplexType.Property("Int64Prop", EdmDataTypes.Int64);
            allPrimitivePropertiesComplexType.Property("ByteProp", EdmDataTypes.Byte);
            allPrimitivePropertiesComplexType.Property("SByteProp", EdmDataTypes.SByte);
            allPrimitivePropertiesComplexType.Property("SingleProp", EdmDataTypes.Single);
            allPrimitivePropertiesComplexType.Property("DoubleProp", EdmDataTypes.Double);
            allPrimitivePropertiesComplexType.Property("DecimalProp", EdmDataTypes.Decimal());
            allPrimitivePropertiesComplexType.Property("DateTimeOffsetProp", EdmDataTypes.DateTimeOffset());
            allPrimitivePropertiesComplexType.Property("DurationProp", EdmDataTypes.Time());
            allPrimitivePropertiesComplexType.Property("GuidProp", EdmDataTypes.Guid);
            allPrimitivePropertiesComplexType.Property("StringProp", EdmDataTypes.String());
            allPrimitivePropertiesComplexType.Property("BinaryProp", EdmDataTypes.Binary());

            // Complex type with a single complex property
            var singleComplexPropertyComplexType = model.ComplexType("SingleComplexPropertyComplexType");
            singleComplexPropertyComplexType.Property("ComplexProp", complexType);

            // Complex type with a single primitive collection property
            var singlePrimitiveCollectionPropertyComplexType = model.ComplexType("SinglePrimitiveCollectionPropertyComplexType");
            singlePrimitiveCollectionPropertyComplexType.Property("PrimitiveCollectionProp", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Int32));

            // Complex type with a single primitive collection property
            var singleComplexCollectionPropertyComplexType = model.ComplexType("SingleComplexCollectionPropertyComplexType");
            singleComplexCollectionPropertyComplexType.Property("ComplexCollectionProp", DataTypes.CollectionOfComplex(complexType));

            // Complex type with different property kinds
            var differentPropertyKindsComplexType = model.ComplexType("DifferentPropertyKindsComplexType");
            differentPropertyKindsComplexType.Property("ComplexProp", complexType);
            differentPropertyKindsComplexType.Property("PrimitiveCollectionProp", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Int32));
            differentPropertyKindsComplexType.Property("Int32Prop", EdmDataTypes.Int32);
            differentPropertyKindsComplexType.Property("ComplexCollectionProp", DataTypes.CollectionOfComplex(complexType));
            #endregion Complex types

            return model.Fixup();
        }

        private static void AddEpmAttributes(EntityType entityType, int count, int startIndex)
        {
            for (int i = 0; i < count; ++i)
            {
                int ix = startIndex + i;
                string propertyName = "Name" + ix;
                entityType.Property(propertyName, EdmDataTypes.String().Nullable());
                entityType.EntityPropertyMapping(propertyName, "custom/name" + ix, "c", "http://customuri/", true);
            }
        }
    }
}
