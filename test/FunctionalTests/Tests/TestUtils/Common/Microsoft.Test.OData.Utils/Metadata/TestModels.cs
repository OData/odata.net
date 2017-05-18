//---------------------------------------------------------------------
// <copyright file="TestModels.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.Test.OData.Utils.ODataLibTest;

namespace Microsoft.Test.OData.Utils.Metadata
{
    /// <summary>
    /// A helper class to create our test models using the <see cref="ModelBuilder"/>.
    /// </summary>
    public static class TestModels
    {
        private const string DefaultNamespaceName = "TestModel";
        private static readonly IEdmStringTypeReference StringNullableTypeRef = EdmCoreModel.Instance.GetString(isNullable: true);
        private static readonly IEdmPrimitiveTypeReference Int32TypeRef = EdmCoreModel.Instance.GetInt32(isNullable: false);
        private static readonly IEdmPrimitiveTypeReference Int32NullableTypeRef = EdmCoreModel.Instance.GetInt32(isNullable: true);

        /// <summary>
        /// Build a test model shared across several tests.
        /// </summary>
        /// <returns>Returns the test model.</returns>
        public static EdmModel BuildTestModel()
        {
            // The metadata model
            var model = new EdmModel();

            var addressType = new EdmComplexType(DefaultNamespaceName, "Address");
            addressType.AddStructuralProperty("Street", StringNullableTypeRef);
            addressType.AddStructuralProperty("Zip", Int32TypeRef);
            addressType.AddStructuralProperty("SubAddress", new EdmComplexTypeReference(addressType, isNullable: false));
            model.AddElement(addressType);

            var officeType = new EdmEntityType(DefaultNamespaceName, "OfficeType");
            officeType.AddKeys(officeType.AddStructuralProperty("Id", Int32TypeRef));
            officeType.AddStructuralProperty("Address", new EdmComplexTypeReference(addressType, isNullable: false));
            model.AddElement(officeType);

            var officeWithNumberType = new EdmEntityType(DefaultNamespaceName, "OfficeWithNumberType", officeType);
            officeWithNumberType.AddStructuralProperty("Number", Int32TypeRef);
            model.AddElement(officeWithNumberType);

            var cityType = new EdmEntityType(DefaultNamespaceName, "CityType");
            cityType.AddKeys(cityType.AddStructuralProperty("Id", Int32TypeRef));
            cityType.AddStructuralProperty("Name", StringNullableTypeRef);
            cityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "CityHall", Target = officeType, TargetMultiplicity = EdmMultiplicity.Many });
            cityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "DOL", Target = officeType, TargetMultiplicity = EdmMultiplicity.Many });
            cityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "PoliceStation", Target = officeType, TargetMultiplicity = EdmMultiplicity.One });
            cityType.AddStructuralProperty("Skyline", EdmPrimitiveTypeKind.Stream, isNullable: false);
            cityType.AddStructuralProperty("MetroLanes", EdmCoreModel.GetCollection(StringNullableTypeRef));
            model.AddElement(cityType);

            var metropolitanCityType = new EdmEntityType(DefaultNamespaceName, "MetropolitanCityType", cityType);
            metropolitanCityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "ContainedOffice", Target = officeType, TargetMultiplicity = EdmMultiplicity.Many, ContainsTarget = true });
            officeType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "ContainedCity", Target = metropolitanCityType, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
            model.AddElement(metropolitanCityType);

            var cityWithMapType = new EdmEntityType(DefaultNamespaceName, "CityWithMapType", cityType, false, false, true);
            model.AddElement(cityWithMapType);

            var cityOpenType = new EdmEntityType(DefaultNamespaceName, "CityOpenType", cityType, isAbstract: false, isOpen: true);
            model.AddElement(cityOpenType);

            var personType = new EdmEntityType(DefaultNamespaceName, "Person");
            personType.AddKeys(personType.AddStructuralProperty("Id", Int32TypeRef));
            personType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "Friend", Target = personType, TargetMultiplicity = EdmMultiplicity.Many });
            model.AddElement(personType);

            var employeeType = new EdmEntityType(DefaultNamespaceName, "Employee", personType);
            employeeType.AddStructuralProperty("CompanyName", StringNullableTypeRef);
            model.AddElement(employeeType);

            var managerType = new EdmEntityType(DefaultNamespaceName, "Manager", employeeType);
            managerType.AddStructuralProperty("Level", Int32TypeRef);
            model.AddElement(managerType);

            var container = new EdmEntityContainer(DefaultNamespaceName, "DefaultContainer");
            model.AddElement(container);

            container.AddEntitySet("Offices", officeType);
            container.AddEntitySet("Cities", cityType);
            container.AddEntitySet("MetropolitanCities", metropolitanCityType);
            container.AddEntitySet("Persons", personType);
            container.AddEntitySet("Employee", employeeType);
            container.AddEntitySet("Manager", managerType);
            container.AddSingleton("Boss", personType);

            // Fixup will set DefaultContainer\TopLevelEntitySet\AssociationSet
            model.Fixup();

            // NOTE: Function import parameters and return types must be nullable as per current CSDL spec
            var serviceOp = container.AddFunctionAndFunctionImport(model, "ServiceOperation1", Int32NullableTypeRef, null, false /*isComposable*/, false /*isBound*/);
            serviceOp.Function.AsEdmFunction().AddParameter("a", Int32NullableTypeRef);
            serviceOp.Function.AsEdmFunction().AddParameter("b", StringNullableTypeRef);

            container.AddFunctionAndFunctionImport(model, "PrimitiveResultOperation", Int32NullableTypeRef, null, false /*isComposable*/, false /*isBound*/);
            container.AddFunctionAndFunctionImport(model, "ComplexResultOperation", new EdmComplexTypeReference(addressType, isNullable: true), null, false /*isComposable*/, false /*isBound*/);
            container.AddFunctionAndFunctionImport(model, "PrimitiveCollectionResultOperation", EdmCoreModel.GetCollection(Int32NullableTypeRef), null, false /*isComposable*/, false /*isBound*/);
            container.AddFunctionAndFunctionImport(model, "ComplexCollectionResultOperation", EdmCoreModel.GetCollection(new EdmComplexTypeReference(addressType, isNullable: true)), null, false /*isComposable*/, false /*isBound*/);

            // Overload with 0 Param
            container.AddFunctionAndFunctionImport(model, "FunctionImportWithOverload", Int32NullableTypeRef, null, false /*isComposable*/, false /*isBound*/);

            // Overload with 1 Param
            var overloadWithOneParam = container.AddFunctionAndFunctionImport(model, "FunctionImportWithOverload", Int32NullableTypeRef, null, false /*isComposable*/, false /*isBound*/);
            overloadWithOneParam.Function.AsEdmFunction().AddParameter("p1", new EdmEntityTypeReference(cityWithMapType, isNullable: true));

            // Overload with 2 Params
            var overloadWithTwoParams = container.AddFunctionAndFunctionImport(model, "FunctionImportWithOverload", Int32NullableTypeRef, null, false /*isComposable*/, false /*isBound*/);
            overloadWithTwoParams.Function.AsEdmFunction().AddParameter("p1", new EdmEntityTypeReference(cityType, isNullable: true));
            overloadWithTwoParams.Function.AsEdmFunction().AddParameter("p2", StringNullableTypeRef);

            // Overload with 5 Params
            var overloadWithFiveParams = container.AddFunctionAndFunctionImport(model, "FunctionImportWithOverload", Int32NullableTypeRef, null, false /*isComposable*/, false /*isBound*/);
            overloadWithFiveParams.Function.AsEdmFunction().AddParameter("p1", EdmCoreModel.GetCollection(new EdmEntityTypeReference(cityType, isNullable: true)));
            overloadWithFiveParams.Function.AsEdmFunction().AddParameter("p2", EdmCoreModel.GetCollection(StringNullableTypeRef));
            overloadWithFiveParams.Function.AsEdmFunction().AddParameter("p3", StringNullableTypeRef);
            overloadWithFiveParams.Function.AsEdmFunction().AddParameter("p4", new EdmComplexTypeReference(addressType, isNullable: true));
            overloadWithFiveParams.Function.AsEdmFunction().AddParameter("p5", EdmCoreModel.GetCollection(new EdmComplexTypeReference(addressType, isNullable: true)));

            return model;
        }

        /// <summary>
        /// Build a test model shared across several tests.
        /// </summary>
        /// <param name="addAnnotations">true if the annotations should be added upon construction; otherwise false.</param>
        /// <returns>Returns the test model.</returns>
        public static IEdmModel BuildODataAnnotationTestModel(bool addAnnotations)
        {
            // The metadata model with OData-specific annotations
            // - default entity container annotation
            // - HasStream annotation on entity type
            // - MimeType annotation on primitive property
            // - MimeType annotation on service operation
            EdmModel model = new EdmModel();

            var addressType = new EdmComplexType(DefaultNamespaceName, "Address");
            addressType.AddStructuralProperty("Street", StringNullableTypeRef);
            var zipProperty = addressType.AddStructuralProperty("Zip", Int32TypeRef);
            model.AddElement(addressType);
            if (addAnnotations)
            {
                model.SetMimeType(zipProperty, "text/plain");
            }

            var personType = new EdmEntityType(DefaultNamespaceName, "PersonType", null, false, false, true);
            personType.AddKeys(personType.AddStructuralProperty("Id", Int32TypeRef));
            var nameProperty = personType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(isNullable: false));
            personType.AddStructuralProperty("Address", new EdmComplexTypeReference(addressType, isNullable: false));
            personType.AddStructuralProperty("Picture", EdmPrimitiveTypeKind.Stream, isNullable: false);
            model.AddElement(personType);
            if (addAnnotations)
            {
                model.SetMimeType(nameProperty, "text/plain");
            }

            // set the default container
            EdmEntityContainer container = new EdmEntityContainer(DefaultNamespaceName, "DefaultContainer");
            model.AddElement(container);

            container.AddEntitySet("People", personType);

            // NOTE: Function import parameters and return types must be nullable as per current CSDL spec
            var serviceOp = container.AddFunctionAndFunctionImport(model, "ServiceOperation1", Int32NullableTypeRef);
            var serviceOpFunc = serviceOp.Function.AsEdmFunction();
            serviceOpFunc.AddParameter("a", Int32NullableTypeRef);
            serviceOpFunc.AddParameter("b", StringNullableTypeRef);

            if (addAnnotations)
            {
                model.SetMimeType(serviceOpFunc, "img/jpeg");
            }

            return model;
        }

        /// <summary>
        /// Creates a set of models.
        /// </summary>
        /// <returns>List of interesting models.</returns>
        public static IEnumerable<IEdmModel> CreateModels()
        {
            //
            // NOTE: we only create a few models here since we mostly rely on EdmLib to test 
            //       model serialization/deserialization for us
            //
            // Empty model
            EdmModel emptyModel = new EdmModel();
            emptyModel.AddElement(new EdmEntityContainer("DefaultNamespace", "DefaultContainer"));
            yield return emptyModel.Fixup();

            // Model with a single entity type
            EdmModel modelWithSingleEntityType = new EdmModel();
            var singletonEntityType = new EdmEntityType(DefaultNamespaceName, "SingletonEntityType");
            singletonEntityType.AddKeys(singletonEntityType.AddStructuralProperty("Id", Int32TypeRef));
            singletonEntityType.AddStructuralProperty("Name", StringNullableTypeRef);
            modelWithSingleEntityType.AddElement(singletonEntityType);
            modelWithSingleEntityType.Fixup();
            yield return modelWithSingleEntityType;

            // Model with a single complex type
            EdmModel modelWithSingleComplexType = new EdmModel();
            var singletonComplexType = new EdmComplexType(DefaultNamespaceName, "SingletonComplexType");
            singletonComplexType.AddStructuralProperty("City", StringNullableTypeRef);
            modelWithSingleComplexType.AddElement(singletonComplexType);
            modelWithSingleComplexType.Fixup();
            yield return modelWithSingleComplexType;

            // Model with a collection property
            EdmModel modelWithCollectionProperty = new EdmModel();
            var complexTypeWithCollection = new EdmComplexType(DefaultNamespaceName, "ComplexTypeWithCollection");
            complexTypeWithCollection.AddStructuralProperty("Cities", EdmCoreModel.GetCollection(StringNullableTypeRef));
            modelWithCollectionProperty.AddElement(complexTypeWithCollection);
            modelWithCollectionProperty.Fixup();
            yield return modelWithCollectionProperty;

            // Model with an open type
            EdmModel modelWithOpenType = new EdmModel();
            var openType = new EdmEntityType(DefaultNamespaceName, "OpenEntityType", baseType: null, isAbstract: false, isOpen: true);
            openType.AddKeys(openType.AddStructuralProperty("Id", Int32TypeRef));
            modelWithOpenType.AddElement(openType);

            var containerForModelWithOpenType = new EdmEntityContainer(DefaultNamespaceName, "DefaultContainer");
            containerForModelWithOpenType.AddEntitySet("OpenEntityType", openType);
            modelWithOpenType.AddElement(containerForModelWithOpenType);
            yield return modelWithOpenType;

            // Model with a named stream
            EdmModel modelWithNamedStream = new EdmModel();
            var namedStreamEntityType = new EdmEntityType(DefaultNamespaceName, "NamedStreamEntityType");
            namedStreamEntityType.AddKeys(namedStreamEntityType.AddStructuralProperty("Id", Int32TypeRef));
            namedStreamEntityType.AddStructuralProperty("NamedStream", EdmPrimitiveTypeKind.Stream, isNullable: false);
            modelWithNamedStream.AddElement(namedStreamEntityType);

            var containerForModelWithNamedStream = new EdmEntityContainer(DefaultNamespaceName, "DefaultContainer");
            containerForModelWithNamedStream.AddEntitySet("NamedStreamEntityType", namedStreamEntityType);
            modelWithNamedStream.AddElement(containerForModelWithNamedStream);
            yield return modelWithNamedStream;

            // OData Shared Test Model
            yield return BuildTestModel();

            // Model with OData-specific attribute annotations
            yield return BuildODataAnnotationTestModel(true);

#if !NETCOREAPP1_0
            // Astoria Default Test Model
            yield return BuildDefaultAstoriaTestModel();
#endif
        }

        /// <summary>
        /// Creates a test model shared among parameter reader/writer tests.
        /// </summary>
        /// <returns>Returns a model with operation imports.</returns>
        public static IEdmModel BuildModelWithFunctionImport()
        {
            EdmCoreModel coreModel = EdmCoreModel.Instance;
            EdmModel model = new EdmModel();
            const string defaultNamespaceName = DefaultNamespaceName;
            EdmEntityContainer container = new EdmEntityContainer(defaultNamespaceName, "TestContainer");
            model.AddElement(container);

            EdmComplexType complexType = new EdmComplexType(defaultNamespaceName, "ComplexType");
            complexType.AddProperty(new EdmStructuralProperty(complexType, "PrimitiveProperty", coreModel.GetString(false)));
            complexType.AddProperty(new EdmStructuralProperty(complexType, "ComplexProperty", complexType.ToTypeReference(false)));
            model.AddElement(complexType);

            EdmEnumType enumType = new EdmEnumType(defaultNamespaceName, "EnumType");
            model.AddElement(enumType);

            EdmEntityType entityType = new EdmEntityType(defaultNamespaceName, "EntityType");
            entityType.AddKeys(new IEdmStructuralProperty[] { new EdmStructuralProperty(entityType, "ID", coreModel.GetInt32(false)) });
            entityType.AddProperty(new EdmStructuralProperty(entityType, "ComplexProperty", complexType.ToTypeReference()));

            container.AddFunctionAndFunctionImport(model, "FunctionImport_Primitive", coreModel.GetString(false) /*returnType*/, null /*entitySet*/, false /*composable*/, false /*bindable*/).Function.AsEdmFunction().AddParameter("primitive", coreModel.GetString(false));
            container.AddFunctionAndFunctionImport(model, "FunctionImport_NullablePrimitive", coreModel.GetString(false) /*returnType*/, null /*entitySet*/, false /*composable*/, false /*bindable*/).Function.AsEdmFunction().AddParameter("nullablePrimitive", coreModel.GetString(true));
            EdmCollectionType stringCollectionType = new EdmCollectionType(coreModel.GetString(true));
            container.AddFunctionAndFunctionImport(model, "FunctionImport_PrimitiveCollection", coreModel.GetString(false) /*returnType*/, null /*entitySet*/, false /*composable*/, false /*bindable*/).Function.AsEdmFunction().AddParameter("primitiveCollection", stringCollectionType.ToTypeReference(false));
            container.AddFunctionAndFunctionImport(model, "FunctionImport_Complex", coreModel.GetString(false) /*returnType*/, null /*entitySet*/, false /*composable*/, false /*bindable*/).Function.AsEdmFunction().AddParameter("complex", complexType.ToTypeReference(true));
            EdmCollectionType complexCollectionType = new EdmCollectionType(complexType.ToTypeReference());
            container.AddFunctionAndFunctionImport(model, "FunctionImport_ComplexCollection", coreModel.GetString(false) /*returnType*/, null /*entitySet*/, false /*composable*/, false /*bindable*/).Function.AsEdmFunction().AddParameter("complexCollection", complexCollectionType.ToTypeReference());
            container.AddFunctionAndFunctionImport(model, "FunctionImport_Entry", coreModel.GetString(false) /*returnType*/, null /*entitySet*/, false /*composable*/, true /*bindable*/).Function.AsEdmFunction().AddParameter("entry", entityType.ToTypeReference());
            EdmCollectionType entityCollectionType = new EdmCollectionType(entityType.ToTypeReference());
            container.AddFunctionAndFunctionImport(model, "FunctionImport_Feed", coreModel.GetString(false) /*returnType*/, null /*entitySet*/, false /*composable*/, true /*bindable*/).Function.AsEdmFunction().AddParameter("feed", entityCollectionType.ToTypeReference());
            container.AddFunctionAndFunctionImport(model, "FunctionImport_Stream", coreModel.GetString(false) /*returnType*/, null /*entitySet*/, false /*composable*/, false /*bindable*/).Function.AsEdmFunction().AddParameter("stream", coreModel.GetStream(false));
            container.AddFunctionAndFunctionImport(model, "FunctionImport_Enum", coreModel.GetString(false) /*returnType*/, null /*entitySet*/, false /*composable*/, false /*bindable*/).Function.AsEdmFunction().AddParameter("enum", enumType.ToTypeReference());

            var functionImport_PrimitiveTwoParameters = container.AddFunctionAndFunctionImport(model, "FunctionImport_PrimitiveTwoParameters", coreModel.GetString(false) /*returnType*/, null /*entitySet*/, false /*composable*/, false /*bindable*/);
            var function_PrimitiveTwoParameters = functionImport_PrimitiveTwoParameters.Function.AsEdmFunction();
            function_PrimitiveTwoParameters.AddParameter("p1", coreModel.GetInt32(false));
            function_PrimitiveTwoParameters.AddParameter("p2", coreModel.GetString(false));

            container.AddFunctionAndFunctionImport(model, "FunctionImport_Int", coreModel.GetString(false) /*returnType*/, null /*entitySet*/, false /*composable*/, false /*bindable*/).Function.AsEdmFunction().AddParameter("p1", coreModel.GetInt32(false));
            container.AddFunctionAndFunctionImport(model, "FunctionImport_Double", coreModel.GetString(false) /*returnType*/, null /*entitySet*/, false /*composable*/, false /*bindable*/).Function.AsEdmFunction().AddParameter("p1", coreModel.GetDouble(false));
            EdmCollectionType int32CollectionType = new EdmCollectionType(coreModel.GetInt32(false));
            container.AddActionAndActionImport(model, "FunctionImport_NonNullablePrimitiveCollection", null /*returnType*/, null /*entitySet*/, false /*bindable*/).Action.AsEdmAction().AddParameter("p1", int32CollectionType.ToTypeReference(false));

            EdmComplexType complexType2 = new EdmComplexType(defaultNamespaceName, "ComplexTypeWithNullableProperties");
            complexType2.AddProperty(new EdmStructuralProperty(complexType2, "StringProperty", coreModel.GetString(true)));
            complexType2.AddProperty(new EdmStructuralProperty(complexType2, "IntegerProperty", coreModel.GetInt32(true)));
            model.AddElement(complexType2);

            EdmEnumType enumType1 = new EdmEnumType(defaultNamespaceName, "enumType1");
            enumType1.AddMember(new EdmEnumMember(enumType1, "enumType1_value1", new EdmEnumMemberValue(6)));
            model.AddElement(enumType1);

            var functionImport_MultipleNullableParameters = container.AddFunctionAndFunctionImport(model, "FunctionImport_MultipleNullableParameters", coreModel.GetString(false) /*returnType*/, null /*entitySet*/, false /*composable*/, false /*bindable*/);
            var function_MultipleNullableParameters = functionImport_MultipleNullableParameters.Function.AsEdmFunction();
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
            function_MultipleNullableParameters.AddParameter("p17", enumType1.ToTypeReference(true /*isNullable*/));

            return model;
        }

#if !NETCOREAPP1_0
        /// <summary>
        /// Builds the Astoria default test model and applies necessary fixups for use in OData tests.
        /// </summary>
        /// <returns>The default Astoria test model.</returns>
        public static IEdmModel BuildDefaultAstoriaTestModel()
        {
            return LoadModelFromEdmx("Microsoft.Test.OData.Utils.Metadata.AstoriaDefaultModel.metadata");
        }

        private static IEdmModel LoadModelFromEdmx(string edmxResourceName)
        {
            Stream modelStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(edmxResourceName);

            using (XmlReader reader = XmlReader.Create(modelStream))
            {
                IEdmModel model;
                IEnumerable<EdmError> errors;
                if (!CsdlReader.TryParse(reader, out model, out errors))
                {
                    throw new Exception("Model loading failed: " + string.Join("\r\n", errors.Select(e => e.ErrorLocation.ToString() + ": " + e.ErrorMessage)));
                }

                return model;
            }
        }
#endif

        /// <summary>
        /// Creates a test model to test our conversion of OData instances into EDM values.
        /// </summary>
        /// <returns>Returns a model suitable for testing EDM values over OData instances.</returns>
        public static IEdmModel BuildEdmValueModel()
        {
            EdmModel model = new EdmModel();
            var complexType = new EdmComplexType(DefaultNamespaceName, "ComplexType");
            complexType.AddStructuralProperty("IntProp", Int32TypeRef);
            complexType.AddStructuralProperty("StringProp", EdmCoreModel.Instance.GetString(isNullable: false));
            complexType.AddStructuralProperty("ComplexProp", new EdmComplexTypeReference(complexType, isNullable: true));
            model.AddElement(complexType);

            #region Entity types
            var entityContainer = new EdmEntityContainer(DefaultNamespaceName, "TestContainer");
            model.AddElement(entityContainer);

            // Entity type with a single primitive property
            var singlePrimitivePropertyEntityType = new EdmEntityType(DefaultNamespaceName, "SinglePrimitivePropertyEntityType");
            singlePrimitivePropertyEntityType.AddKeys(singlePrimitivePropertyEntityType.AddStructuralProperty("ID", Int32TypeRef));
            singlePrimitivePropertyEntityType.AddStructuralProperty("Int32Prop", Int32NullableTypeRef);
            entityContainer.AddEntitySet("SinglePrimitivePropertyEntityType", singlePrimitivePropertyEntityType);
            model.AddElement(singlePrimitivePropertyEntityType);

            // Entity type with all primitive properties
            var allPrimitivePropertiesEntityType = new EdmEntityType(DefaultNamespaceName, "AllPrimitivePropertiesEntityType");
            allPrimitivePropertiesEntityType.AddKeys(allPrimitivePropertiesEntityType.AddStructuralProperty("ID", Int32TypeRef));
            allPrimitivePropertiesEntityType.AddStructuralProperty("BoolProp", EdmPrimitiveTypeKind.Boolean, isNullable: false);
            allPrimitivePropertiesEntityType.AddStructuralProperty("Int16Prop", EdmPrimitiveTypeKind.Int16, isNullable: false);
            allPrimitivePropertiesEntityType.AddStructuralProperty("Int32Prop", EdmPrimitiveTypeKind.Int32, isNullable: false);
            allPrimitivePropertiesEntityType.AddStructuralProperty("Int64Prop", EdmPrimitiveTypeKind.Int64, isNullable: false);
            allPrimitivePropertiesEntityType.AddStructuralProperty("ByteProp", EdmPrimitiveTypeKind.Byte, isNullable: false);
            allPrimitivePropertiesEntityType.AddStructuralProperty("SByteProp", EdmPrimitiveTypeKind.SByte, isNullable: false);
            allPrimitivePropertiesEntityType.AddStructuralProperty("SingleProp", EdmPrimitiveTypeKind.Single, isNullable: false);
            allPrimitivePropertiesEntityType.AddStructuralProperty("DoubleProp", EdmPrimitiveTypeKind.Double, isNullable: false);
            allPrimitivePropertiesEntityType.AddStructuralProperty("DecimalProp", EdmPrimitiveTypeKind.Decimal, isNullable: false);
            allPrimitivePropertiesEntityType.AddStructuralProperty("DateTimeOffsetProp", EdmPrimitiveTypeKind.DateTimeOffset, isNullable: false);
            allPrimitivePropertiesEntityType.AddStructuralProperty("TimeProp", EdmPrimitiveTypeKind.Duration, isNullable: false);
            allPrimitivePropertiesEntityType.AddStructuralProperty("GuidProp", EdmPrimitiveTypeKind.Guid, isNullable: false);
            allPrimitivePropertiesEntityType.AddStructuralProperty("StringProp", EdmPrimitiveTypeKind.String, isNullable: false);
            allPrimitivePropertiesEntityType.AddStructuralProperty("BinaryProp", EdmPrimitiveTypeKind.Binary, isNullable: false);
            entityContainer.AddEntitySet("AllPrimitivePropertiesEntityType", allPrimitivePropertiesEntityType);
            model.AddElement(allPrimitivePropertiesEntityType);

            // Entity type with a single complex property
            var singleComplexPropertyEntityType = new EdmEntityType(DefaultNamespaceName, "SingleComplexPropertyEntityType");
            singleComplexPropertyEntityType.AddKeys(singleComplexPropertyEntityType.AddStructuralProperty("ID", Int32TypeRef));
            singleComplexPropertyEntityType.AddStructuralProperty("ComplexProp", new EdmComplexTypeReference(complexType, isNullable: true));
            entityContainer.AddEntitySet("SingleComplexPropertyEntityType", singleComplexPropertyEntityType);
            model.AddElement(singleComplexPropertyEntityType);

            // Entity type with a single primitive collection property
            var singlePrimitiveCollectionPropertyEntityType = new EdmEntityType(DefaultNamespaceName, "SinglePrimitiveCollectionPropertyEntityType");
            singlePrimitiveCollectionPropertyEntityType.AddKeys(singlePrimitiveCollectionPropertyEntityType.AddStructuralProperty("ID", Int32TypeRef));
            singlePrimitiveCollectionPropertyEntityType.AddStructuralProperty("PrimitiveCollectionProp", EdmCoreModel.GetCollection(Int32TypeRef));
            entityContainer.AddEntitySet("SinglePrimitiveCollectionPropertyEntityType", singlePrimitiveCollectionPropertyEntityType);
            model.AddElement(singlePrimitiveCollectionPropertyEntityType);

            // Entity type with a single primitive collection property
            var singleComplexCollectionPropertyEntityType = new EdmEntityType(DefaultNamespaceName, "SingleComplexCollectionPropertyEntityType");
            singleComplexCollectionPropertyEntityType.AddKeys(singleComplexCollectionPropertyEntityType.AddStructuralProperty("ID", Int32TypeRef));
            singleComplexCollectionPropertyEntityType.AddStructuralProperty("ComplexCollectionProp", EdmCoreModel.GetCollection(new EdmComplexTypeReference(complexType, isNullable: true)));
            entityContainer.AddEntitySet("SingleComplexCollectionPropertyEntityType", singleComplexCollectionPropertyEntityType);
            model.AddElement(singleComplexCollectionPropertyEntityType);

            // Entity type with different property kinds
            var differentPropertyKindsEntityType = new EdmEntityType(DefaultNamespaceName, "DifferentPropertyKindsEntityType");
            differentPropertyKindsEntityType.AddKeys(differentPropertyKindsEntityType.AddStructuralProperty("ID", Int32TypeRef));
            differentPropertyKindsEntityType.AddStructuralProperty("ComplexProp", new EdmComplexTypeReference(complexType, isNullable: true));
            differentPropertyKindsEntityType.AddStructuralProperty("PrimitiveCollectionProp", EdmCoreModel.GetCollection(Int32TypeRef));
            differentPropertyKindsEntityType.AddStructuralProperty("Int32Prop", EdmPrimitiveTypeKind.Int32, isNullable: false);
            differentPropertyKindsEntityType.AddStructuralProperty("ComplexCollectionProp", EdmCoreModel.GetCollection(new EdmComplexTypeReference(complexType, isNullable: true)));
            entityContainer.AddEntitySet("DifferentPropertyKindsEntityType", differentPropertyKindsEntityType);
            model.AddElement(differentPropertyKindsEntityType);
            #endregion Entity types

            #region Complex types
            // Empty complex type
            model.AddElement(new EdmComplexType(DefaultNamespaceName, "EmptyComplexType"));

            // Complex type with a single primitive property
            var singlePrimitivePropertyComplexType = new EdmComplexType(DefaultNamespaceName, "SinglePrimitivePropertyComplexType");
            singlePrimitivePropertyComplexType.AddStructuralProperty("Int32Prop", Int32NullableTypeRef);
            model.AddElement(singlePrimitivePropertyComplexType);

            // Complex type with all primitive properties
            var allPrimitivePropertiesComplexType = new EdmComplexType(DefaultNamespaceName, "AllPrimitivePropertiesComplexType");
            allPrimitivePropertiesComplexType.AddStructuralProperty("BoolProp", EdmPrimitiveTypeKind.Boolean, isNullable: false);
            allPrimitivePropertiesComplexType.AddStructuralProperty("Int16Prop", EdmPrimitiveTypeKind.Int16, isNullable: false);
            allPrimitivePropertiesComplexType.AddStructuralProperty("Int32Prop", EdmPrimitiveTypeKind.Int32, isNullable: false);
            allPrimitivePropertiesComplexType.AddStructuralProperty("Int64Prop", EdmPrimitiveTypeKind.Int64, isNullable: false);
            allPrimitivePropertiesComplexType.AddStructuralProperty("ByteProp", EdmPrimitiveTypeKind.Byte, isNullable: false);
            allPrimitivePropertiesComplexType.AddStructuralProperty("SByteProp", EdmPrimitiveTypeKind.SByte, isNullable: false);
            allPrimitivePropertiesComplexType.AddStructuralProperty("SingleProp", EdmPrimitiveTypeKind.Single, isNullable: false);
            allPrimitivePropertiesComplexType.AddStructuralProperty("DoubleProp", EdmPrimitiveTypeKind.Double, isNullable: false);
            allPrimitivePropertiesComplexType.AddStructuralProperty("DecimalProp", EdmPrimitiveTypeKind.Decimal, isNullable: false);
            allPrimitivePropertiesComplexType.AddStructuralProperty("DateTimeOffsetProp", EdmPrimitiveTypeKind.DateTimeOffset, isNullable: false);
            allPrimitivePropertiesComplexType.AddStructuralProperty("TimeProp", EdmPrimitiveTypeKind.Duration, isNullable: false);
            allPrimitivePropertiesComplexType.AddStructuralProperty("GuidProp", EdmPrimitiveTypeKind.Guid, isNullable: false);
            allPrimitivePropertiesComplexType.AddStructuralProperty("StringProp", EdmPrimitiveTypeKind.String, isNullable: false);
            allPrimitivePropertiesComplexType.AddStructuralProperty("BinaryProp", EdmPrimitiveTypeKind.Binary, isNullable: false);
            model.AddElement(allPrimitivePropertiesComplexType);

            // Complex type with a single complex property
            var singleComplexPropertyComplexType = new EdmComplexType(DefaultNamespaceName, "SingleComplexPropertyComplexType");
            singleComplexPropertyComplexType.AddStructuralProperty("ComplexProp", new EdmComplexTypeReference(complexType, isNullable: true));
            model.AddElement(singleComplexPropertyComplexType);

            // Complex type with a single primitive collection property
            var singlePrimitiveCollectionPropertyComplexType = new EdmComplexType(DefaultNamespaceName, "SinglePrimitiveCollectionPropertyComplexType");
            singlePrimitiveCollectionPropertyComplexType.AddStructuralProperty("PrimitiveCollectionProp", EdmCoreModel.GetCollection(Int32TypeRef));
            model.AddElement(singlePrimitiveCollectionPropertyComplexType);

            // Complex type with a single primitive collection property
            var singleComplexCollectionPropertyComplexType = new EdmComplexType(DefaultNamespaceName, "SingleComplexCollectionPropertyComplexType");
            singleComplexCollectionPropertyComplexType.AddStructuralProperty("ComplexCollectionProp", EdmCoreModel.GetCollection(new EdmComplexTypeReference(complexType, isNullable: true)));
            model.AddElement(singleComplexCollectionPropertyComplexType);

            // Complex type with different property kinds
            var differentPropertyKindsComplexType = new EdmComplexType(DefaultNamespaceName, "DifferentPropertyKindsComplexType");
            differentPropertyKindsComplexType.AddStructuralProperty("ComplexProp", new EdmComplexTypeReference(complexType, isNullable: true));
            differentPropertyKindsComplexType.AddStructuralProperty("PrimitiveCollectionProp", EdmCoreModel.GetCollection(Int32TypeRef));
            differentPropertyKindsComplexType.AddStructuralProperty("Int32Prop", EdmPrimitiveTypeKind.Int32);
            differentPropertyKindsComplexType.AddStructuralProperty("ComplexCollectionProp", EdmCoreModel.GetCollection(new EdmComplexTypeReference(complexType, isNullable: true)));
            model.AddElement(differentPropertyKindsComplexType);
            #endregion Complex types

            return model;
        }
    }
}
