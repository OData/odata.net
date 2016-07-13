//---------------------------------------------------------------------
// <copyright file="TypeSemanticsUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TypeSemanticsUnitTests : EdmLibTestCaseBase
    {
        [TestMethod]
        public void NonPrimitiveIsXXXMethods()
        {
            IEdmEntityType entityDef = new EdmEntityType("MyNamespace", "MyEntity");
            IEdmEntityTypeReference entityRef = new EdmEntityTypeReference(entityDef, false);

            Assert.IsTrue(entityRef.IsEntity(), "Entity is Entity");

            IEdmPrimitiveTypeReference bad = entityRef.AsPrimitive();
            Assert.IsTrue(bad.Definition.IsBad(), "bad TypeReference is bad");
            Assert.AreEqual(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, bad.Definition.Errors().First().ErrorCode, "Reference is bad from conversion");
            Assert.IsTrue(bad.Definition.IsBad(), "Bad definition is bad");
            Assert.AreEqual(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, bad.Definition.Errors().First().ErrorCode, "Definition is bad from conversion");

            IEdmPrimitiveType intDef = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32);
            IEdmPrimitiveTypeReference intRef = new EdmPrimitiveTypeReference(intDef, false);
            IEdmCollectionTypeReference intCollection = new EdmCollectionTypeReference(new EdmCollectionType(intRef));
            Assert.IsTrue(intCollection.IsCollection(), "Collection is collection");

            IEdmComplexType complexDef = new EdmComplexType("MyNamespace", "MyComplex");
            IEdmComplexTypeReference complexRef = new EdmComplexTypeReference(complexDef, false);
            Assert.IsTrue(complexRef.IsComplex(), "Complex is Complex");

            Assert.IsTrue(entityRef.IsStructured(), "Entity is Structured");
            Assert.IsTrue(complexRef.IsStructured(), "Complex is stuctured");
            Assert.IsFalse(intCollection.IsStructured(), "Collection is not structured");
        }

        [TestMethod]
        public void PrimitiveIsXXXMethods()
        {
            IEdmPrimitiveTypeReference binaryRef = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false);
            IEdmPrimitiveTypeReference booleanRef = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Boolean), false);
            IEdmPrimitiveTypeReference byteRef = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Byte), false);
            IEdmPrimitiveTypeReference dateTimeOffsetRef = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), false);
            IEdmPrimitiveTypeReference decimalRef = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), false);
            IEdmPrimitiveTypeReference doubleRef = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Double), false);
            IEdmPrimitiveTypeReference guidRef = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Guid), false);
            IEdmPrimitiveTypeReference int16Ref = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int16), false);
            IEdmPrimitiveTypeReference int32Ref = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), false);
            IEdmPrimitiveTypeReference int64Ref = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int64), false);
            IEdmPrimitiveTypeReference sByteRef = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.SByte), false);
            IEdmPrimitiveTypeReference singleRef = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Single), false);
            IEdmPrimitiveTypeReference stringRef = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false);
            IEdmPrimitiveTypeReference streamRef = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Stream), false);
            IEdmPrimitiveTypeReference timeRef = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration), false);
            IEdmPrimitiveTypeReference dateRef = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Date), false);
            IEdmPrimitiveTypeReference timeOfDayRef = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.TimeOfDay), false);

            Assert.IsTrue(binaryRef.IsPrimitive(), "Binary is Primitive");
            Assert.IsTrue(binaryRef.IsBinary(), "Binary is Binary");
            Assert.IsTrue(booleanRef.IsBoolean(), "Boolean is Boolean");
            Assert.IsTrue(byteRef.IsByte(), "Byte is Byte");
            Assert.IsTrue(dateTimeOffsetRef.IsDateTimeOffset(), "DateTimeOffset is DateTimeOffset");
            Assert.IsTrue(decimalRef.IsDecimal(), "Decimal is Decimal");
            Assert.IsTrue(doubleRef.IsDouble(), "Double is Double");
            Assert.IsTrue(guidRef.IsGuid(), "Guid is Guid");
            Assert.IsTrue(int16Ref.IsInt16(), "Int16 is Int16");
            Assert.IsTrue(int32Ref.IsInt32(), "Int32 is Int32");
            Assert.IsTrue(int64Ref.IsInt64(), "Int64 is Int64");
            Assert.IsTrue(sByteRef.IsSByte(), "SByte is SByte");
            Assert.IsTrue(singleRef.IsSingle(), "Single is Single");
            Assert.IsTrue(stringRef.IsString(), "String is String");
            Assert.IsTrue(streamRef.IsStream(), "Stream is Stream");
            Assert.IsTrue(timeRef.IsDuration(), "Duration is Duration");
            Assert.IsTrue(dateRef.IsDate(), "Date is Date");
            Assert.IsTrue(timeOfDayRef.IsTimeOfDay(), "TimeOfDay is TimeOfDay");

            Assert.IsTrue(binaryRef.AsPrimitive().IsPrimitive(), "Binary as primitive is Primitive");
            Assert.IsTrue(binaryRef.AsPrimitive().IsBinary(), "Binary as primitive is Binary");
            Assert.IsTrue(booleanRef.AsPrimitive().IsBoolean(), "Boolean as primitive is Boolean");
            Assert.IsTrue(byteRef.AsPrimitive().IsByte(), "Byte as primitive is Byte");
            Assert.IsTrue(dateTimeOffsetRef.AsPrimitive().IsDateTimeOffset(), "DateTimeOffset as primitive is DateTimeOffset");
            Assert.IsTrue(decimalRef.AsPrimitive().IsDecimal(), "Decimal as primitive is Decimal");
            Assert.IsTrue(doubleRef.AsPrimitive().IsDouble(), "Double as primitive is Double");
            Assert.IsTrue(guidRef.AsPrimitive().IsGuid(), "Guid as primitive is Guid");
            Assert.IsTrue(int16Ref.AsPrimitive().IsInt16(), "Int16 as primitive is Int16");
            Assert.IsTrue(int32Ref.AsPrimitive().IsInt32(), "Int32 as primitive is Int32");
            Assert.IsTrue(int64Ref.AsPrimitive().IsInt64(), "Int64 as primitive is Int64");
            Assert.IsTrue(sByteRef.AsPrimitive().IsSByte(), "SByte as primitive is SByte");
            Assert.IsTrue(singleRef.AsPrimitive().IsSingle(), "Single as primitive is Single");
            Assert.IsTrue(stringRef.AsPrimitive().IsString(), "String as primitive is String");
            Assert.IsTrue(streamRef.AsPrimitive().IsStream(), "Stream as primitive is Stream");
            Assert.IsTrue(timeRef.AsPrimitive().IsDuration(), "Duration as primitive is Duration");
            Assert.IsTrue(dateRef.AsPrimitive().IsDate(), "Date as primitive is Date");
            Assert.IsTrue(timeOfDayRef.AsPrimitive().IsTimeOfDay(), "TimeOfDay as primitive is TimeOfDay");

            Assert.IsTrue(dateTimeOffsetRef.IsTemporal(), "DateTimeOffset is Temporal");
            Assert.IsTrue(timeRef.IsTemporal(), "Duration is Temporal");
            Assert.IsTrue(timeOfDayRef.IsTemporal(), "TimeOfDay is Temporal");
            Assert.IsFalse(int32Ref.IsTemporal(), "Int is not Temporal");

            Assert.IsTrue(doubleRef.IsFloating(), "Double is floating");
            Assert.IsTrue(singleRef.IsFloating(), "Single is floating");
            Assert.IsFalse(int32Ref.IsFloating(), "Int is not floating");

            Assert.IsTrue(sByteRef.IsSignedIntegral(), "SByte is signed integral");
            Assert.IsTrue(int16Ref.IsSignedIntegral(), "Int16 is signed integral");
            Assert.IsTrue(int32Ref.IsSignedIntegral(), "Int32 is signed integral");
            Assert.IsTrue(int64Ref.IsSignedIntegral(), "Int64 is signed integral");
            Assert.IsFalse(stringRef.IsSignedIntegral(), "String is not signed integral");

            IEdmEntityType entityDef = new EdmEntityType("MyNamespace", "MyEntity");
            IEdmEntityTypeReference entityRef = new EdmEntityTypeReference(entityDef, false);
            Assert.AreEqual(EdmPrimitiveTypeKind.None, entityRef.PrimitiveKind(), "Non-Primitive Type has primitivetypekind of none");
            Assert.AreEqual(EdmSchemaElementKind.TypeDefinition, int16Ref.PrimitiveDefinition().SchemaElementKind,"SchemaElementKind of primitive type is correct.");
        }

        [TestMethod]
        public void NonPrimitiveAsXXXMethods()
        {
            IEdmPrimitiveType intDef = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32);
            IEdmPrimitiveTypeReference intRef = new EdmPrimitiveTypeReference(intDef, false);

            IEdmEntityType entityDef = new EdmEntityType("MyNamespace", "MyEntity");
            IEdmEntityTypeReference entityRef = new EdmEntityTypeReference(entityDef, false);
            IEdmComplexType complexDef = new EdmComplexType("MyNamespace", "MyComplex");
            IEdmComplexTypeReference complexRef = new EdmComplexTypeReference(complexDef, false);
            IEdmCollectionType collectionDef = new EdmCollectionType(intRef);
            IEdmCollectionTypeReference collectionRef = new EdmCollectionTypeReference(collectionDef);

            IEdmCollectionTypeReference badCollectionRef = entityRef.AsCollection();
            IEdmEntityTypeReference badEntityRef = collectionRef.AsEntity();
            IEdmComplexTypeReference badComplexRef = entityRef.AsComplex();
            IEdmEntityReferenceTypeReference badEntityRefRef= entityRef.AsEntityReference();

            Assert.IsTrue(badCollectionRef.IsBad(), "Bad Collection is bad");
            Assert.AreEqual(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badCollectionRef.Errors().First().ErrorCode, "Reference is bad from conversion");
            Assert.AreEqual("[Collection([UnknownType Nullable=True]) Nullable=True]", badCollectionRef.ToString(), "Correct tostring");

            Assert.IsTrue(badComplexRef.IsBad(), "Bad Complex is Bad");
            Assert.IsTrue(badComplexRef.Definition.IsBad(), "Bad Complex definition is Bad");
            Assert.AreEqual(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badComplexRef.Definition.Errors().First().ErrorCode, "Reference is bad from conversion");
            Assert.AreEqual("TypeSemanticsCouldNotConvertTypeReference:[MyNamespace.MyEntity Nullable=False]", badComplexRef.ToString(), "Correct tostring");

            Assert.IsTrue(badEntityRef.IsBad(), "Bad Entity is bad");
            Assert.IsTrue(badEntityRef.Definition.IsBad(), "Bad Entity Definition is bad");
            Assert.AreEqual(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badEntityRef.Definition.Errors().First().ErrorCode, "Reference is bad from conversion");
            Assert.AreEqual("TypeSemanticsCouldNotConvertTypeReference:[Collection(Edm.Int32) Nullable=False]", badEntityRef.ToString(), "Correct tostring");

            Assert.IsTrue(badEntityRefRef.IsBad(), "Bad Entity Reference is Bad");
            Assert.AreEqual(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badEntityRefRef.Errors().First().ErrorCode, "Reference is bad from conversion");
            Assert.AreEqual("[EntityReference(.) Nullable=False]", badEntityRefRef.ToString(), "Correct tostring");

            Assert.IsFalse(entityRef.AsEntity().IsBad(), "Entity converted to Entity is good");
            Assert.IsFalse(complexRef.AsComplex().IsBad(), "Complex converted to complex is good");
            Assert.IsFalse(collectionRef.AsCollection().IsBad(), "Collection converted to collection is good");

            Assert.IsTrue(entityRef.AsStructured().IsEntity(), "Entity as structured is entity");
            Assert.IsFalse(entityRef.AsStructured().IsBad(), "Entity as structured is good");
            Assert.IsTrue(complexRef.AsStructured().IsComplex(), "Complex as structured is complex");
            Assert.IsFalse(complexRef.AsStructured().IsBad(), "Complex as structured is good");
            Assert.IsFalse(collectionRef.AsStructured().IsCollection(), "Collection as structured is not collection");
            Assert.IsTrue(collectionRef.AsStructured().IsBad(), "Collection as structured is bad");
            Assert.IsTrue(collectionRef.AsStructured().Definition.IsBad(), "Collection as structured definition is bad");
            Assert.AreEqual(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, collectionRef.AsStructured().Definition.Errors().First().ErrorCode, "Reference is bad from conversion");
        }

        [TestMethod]
        public void PrimitiveAsXXXMethods()
        {
            IEdmEntityType entityDef = new EdmEntityType("MyNamespace", "BadEntity");
            IEdmEntityTypeReference entityRef = new EdmEntityTypeReference(entityDef, false);
            IEdmTemporalTypeReference badTemporal = entityRef.AsTemporal();
            IEdmDecimalTypeReference badDecimal = entityRef.AsDecimal();
            IEdmStringTypeReference badString = entityRef.AsString();
            IEdmPrimitiveTypeReference badStream = entityRef.AsPrimitive();
            IEdmBinaryTypeReference badBinary = entityRef.AsBinary();
            IEdmSpatialTypeReference badSpatial = entityRef.AsSpatial();
            IEdmPrimitiveTypeReference badPrimitive = entityRef.AsPrimitive();

            Assert.IsTrue(badTemporal.IsBad(), "Bad Temporal is Bad");
            Assert.IsTrue(badTemporal.Definition.IsBad(), "Bad Temporal Definition is Bad");
            Assert.AreEqual(EdmErrorCode.InterfaceCriticalKindValueMismatch, badTemporal.Errors().First().ErrorCode, "Reference is bad from conversion");
            Assert.AreEqual(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badTemporal.Definition.Errors().First().ErrorCode, "Reference is bad from conversion");
            Assert.AreEqual("TypeSemanticsCouldNotConvertTypeReference:[MyNamespace.BadEntity Nullable=False]", badTemporal.ToString(), "Correct tostring");
            
            Assert.IsTrue(badDecimal.IsBad(), "Bad Decimal is Bad");
            Assert.IsTrue(badDecimal.Definition.IsBad(), "Bad Decimal definition is Bad");
            Assert.AreEqual(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badDecimal.Definition.Errors().First().ErrorCode, "Reference is bad from conversion");
            Assert.AreEqual("TypeSemanticsCouldNotConvertTypeReference:[MyNamespace.BadEntity Nullable=False]", badDecimal.ToString(), "Correct tostring");

            Assert.IsTrue(badString.IsBad(), "Bad String is Bad");
            Assert.IsTrue(badString.Definition.IsBad(), "Bad String Definition is Bad");
            Assert.AreEqual(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badString.Definition.Errors().First().ErrorCode, "Reference is bad from conversion");
            Assert.AreEqual("TypeSemanticsCouldNotConvertTypeReference:[MyNamespace.BadEntity Nullable=False Unicode=False]", badString.ToString(), "Correct tostring");

            Assert.IsTrue(badStream.IsBad(), "Bad Stream is Bad");
            Assert.IsTrue(badStream.Definition.IsBad(), "Bad Stream Definition is Bad");
            Assert.AreEqual(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badStream.Definition.Errors().First().ErrorCode, "Reference is bad from conversion");
            Assert.AreEqual("TypeSemanticsCouldNotConvertTypeReference:[MyNamespace.BadEntity Nullable=False]", badStream.ToString(), "Correct tostring");

            Assert.IsTrue(badBinary.IsBad(), "Bad Binary is Bad");
            Assert.IsTrue(badBinary.Definition.IsBad(), "Bad Binary Definition is Bad");
            Assert.AreEqual(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badBinary.Definition.Errors().First().ErrorCode, "Reference is bad from conversion");
            Assert.AreEqual("TypeSemanticsCouldNotConvertTypeReference:[MyNamespace.BadEntity Nullable=False]", badBinary.ToString(), "Correct tostring");
            
            Assert.IsTrue(badSpatial.IsBad(), "Bad Spatial is Bad");
            Assert.IsTrue(badSpatial.Definition.IsBad(), "Bad Spatial Definition is Bad");
            Assert.AreEqual(EdmErrorCode.InterfaceCriticalKindValueMismatch, badSpatial.Errors().First().ErrorCode, "Reference is bad from conversion");
            Assert.AreEqual(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badSpatial.Definition.Errors().First().ErrorCode, "Reference is bad from conversion");
            Assert.AreEqual("TypeSemanticsCouldNotConvertTypeReference:[MyNamespace.BadEntity Nullable=False]", badSpatial.ToString(), "Correct tostring");

            Assert.IsTrue(badPrimitive.IsBad(), "Bad Primitive is Bad");
            Assert.IsTrue(badPrimitive.Definition.IsBad(), "Bad Primitive Definition is Bad");
            Assert.AreEqual(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badPrimitive.Definition.Errors().First().ErrorCode, "Reference is bad from conversion");
            Assert.AreEqual("TypeSemanticsCouldNotConvertTypeReference:[MyNamespace.BadEntity Nullable=False]", badPrimitive.ToString(), "Correct tostring");
        }

        [TestMethod]
        public void ExerciseBadtypes()
        {
            IEdmModel model = new EdmModel();

            IEdmEntityType entityDef = new EdmEntityType("MyNamespace", "BadEntity");
            IEdmEntityTypeReference entityRef = new EdmEntityTypeReference(entityDef, false);
            IEdmComplexType complexDef = new EdmComplexType("MyNamespace", "BadComplex");
            IEdmComplexTypeReference complexRef = new EdmComplexTypeReference(complexDef, false);

            IEdmCollectionTypeReference badCollectionRef = entityRef.AsCollection();
            IEdmEntityTypeReference badEntityRef = complexRef.AsEntity();
            IEdmComplexTypeReference badComplexRef = entityRef.AsComplex();
            IEdmEntityReferenceTypeReference badEntityRefRef = entityRef.AsEntityReference();
            IEdmPrimitiveTypeReference badTemporal = entityRef.AsTemporal();
            IEdmPrimitiveTypeReference badDecimal = entityRef.AsDecimal();
            IEdmPrimitiveTypeReference badString = entityRef.AsString();
            IEdmPrimitiveTypeReference badSpatial = entityRef.AsSpatial();
            IEdmPrimitiveTypeReference badBinary = entityRef.AsBinary();
            IEdmPrimitiveTypeReference badPrimitive = entityRef.AsPrimitive();
            IEdmEnumTypeReference badEnum = entityRef.AsEnum();

            Assert.IsTrue(badCollectionRef.IsCollection(), "Bad collection is collection");

            Assert.IsTrue(badEntityRef.IsEntity(), "Bad Entity is Entity");
            Assert.AreEqual(0, badEntityRef.Key().Count(), "Bad Entity has no key");
            Assert.AreEqual("MyNamespace.BadComplex", badEntityRef.FullName(), "Bad named refs keep name");
            Assert.AreEqual(EdmSchemaElementKind.TypeDefinition, badEntityRef.EntityDefinition().SchemaElementKind, "Bad named type definitions are still type definitions");
            Assert.IsNull(badEntityRef.EntityDefinition().BaseType, "Bad Entity has no base type");
            Assert.AreEqual(Enumerable.Empty<IEdmProperty>(), badEntityRef.EntityDefinition().DeclaredProperties, "Bad structured types have no properties");
            Assert.IsFalse(badEntityRef.EntityDefinition().IsAbstract, "Bad structured types are not abstract");
            Assert.IsFalse(badEntityRef.EntityDefinition().IsOpen, "Bad structured types are not open");
            model.SetAnnotationValue(badEntityRef.Definition, "foo", "bar", new EdmStringConstant(new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false), "baz"));
            Assert.AreEqual(1, model.DirectValueAnnotations(badEntityRef.Definition).Count(), "Bad Entity can hold annotations");
            Assert.IsNotNull(model.GetAnnotationValue(badEntityRef.Definition, "foo", "bar"), "Bad Entity can find annotations");

            Assert.IsTrue(badComplexRef.IsComplex(), "Bad Complex is Bad");
            Assert.IsNull(badComplexRef.ComplexDefinition().FindProperty("PropertyName"), "Bad structured types return null for find property");

            Assert.AreEqual(EdmTypeKind.EntityReference, badEntityRefRef.TypeKind(), "Bad Entity Reference is Entity Reference");
            Assert.AreEqual(String.Empty, badEntityRefRef.EntityType().Name, "Bad Entity Reference has empty named EntityType");
            model.SetAnnotationValue(badEntityRefRef.Definition, "foo", "bar", new EdmStringConstant(new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false), "baz"));
            Assert.AreEqual(1, model.DirectValueAnnotations(badEntityRefRef.Definition).Count(), "Bad Entity Reference can hold annotations");
            Assert.IsNotNull(model.GetAnnotationValue(badEntityRefRef.Definition, "foo", "bar"), "Bad Entity Reference can find annotations");

            Assert.AreEqual(EdmPrimitiveTypeKind.None, badPrimitive.PrimitiveKind(), "Bad Primitive has no primitive kind");
            Assert.AreEqual(EdmTypeKind.Primitive, badPrimitive.TypeKind(), "Bad Primitive has primitive type kind");
            Assert.AreEqual(EdmSchemaElementKind.TypeDefinition, badPrimitive.PrimitiveDefinition().SchemaElementKind, "Bad Primitive is still Type Definition");
            Assert.AreEqual("BadEntity", badPrimitive.PrimitiveDefinition().Name, "Bad Primitive retains name");
            Assert.AreEqual("MyNamespace", badPrimitive.PrimitiveDefinition().Namespace, "Bad Primitive retains namespace name");

            Assert.AreEqual(EdmTypeKind.Enum, badEnum.TypeKind(), "Bad enum has right kind.");
            Assert.AreEqual(EdmPrimitiveTypeKind.Int32, badEnum.EnumDefinition().UnderlyingType.PrimitiveKind, "Underlying type has correct kind.");
            Assert.AreEqual(0, badEnum.EnumDefinition().Members.Count(), "bad enum has no members.");
            Assert.AreEqual(false, badEnum.EnumDefinition().IsFlags, "bad enum not treated as bits.");
            Assert.AreEqual(EdmSchemaElementKind.TypeDefinition, badEnum.EnumDefinition().SchemaElementKind, "bad enum is type definition.");
            Assert.AreEqual("BadEntity", badEnum.EnumDefinition().Name, "Bad enum retains name");
            Assert.AreEqual("MyNamespace", badEnum.EnumDefinition().Namespace, "Bad enum retains namespace name");

            Assert.AreEqual(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badSpatial.Definition.Errors().First().ErrorCode, "Bad spatial has correct error code.");
        }

        [TestMethod]
        public void EqualityMainPathTests()
        {
            Assert.IsTrue(EdmCoreModel.Instance.GetBinary(false).IsEquivalentTo(EdmCoreModel.Instance.GetBinary(false)), "Type reference equal");
            Assert.IsTrue(EdmCoreModel.Instance.GetBoolean(false).IsEquivalentTo(EdmCoreModel.Instance.GetBoolean(false)), "Type reference equal");
            Assert.IsTrue(EdmCoreModel.Instance.GetDecimal(false).IsEquivalentTo(EdmCoreModel.Instance.GetDecimal(false)), "Type reference equal");
            Assert.IsTrue(EdmCoreModel.Instance.GetString(false).IsEquivalentTo(EdmCoreModel.Instance.GetString(false)), "Type reference equal");
            Assert.IsTrue(EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false).IsEquivalentTo(EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false)), "Type reference equal");
            Assert.IsTrue(EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false)).IsEquivalentTo(EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false))), "Type reference equal");

            Assert.IsFalse(EdmCoreModel.Instance.GetBinary(false).IsEquivalentTo(EdmCoreModel.Instance.GetBinary(true)), "Unequal facets not equal");
            Assert.IsFalse(EdmCoreModel.Instance.GetBoolean(false).IsEquivalentTo(EdmCoreModel.Instance.GetBoolean(true)), "Unequal facets not equal");
            Assert.IsFalse(EdmCoreModel.Instance.GetDecimal(false).IsEquivalentTo(EdmCoreModel.Instance.GetDecimal(true)), "Unequal facets not equal");
            Assert.IsFalse(EdmCoreModel.Instance.GetString(false).IsEquivalentTo(EdmCoreModel.Instance.GetString(true)), "Unequal facets not equal");
            Assert.IsFalse(EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false).Equals(EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, true)), "Unequal facets not equal");

            Assert.IsFalse(EdmCoreModel.Instance.GetBinary(false).IsEquivalentTo(EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false)), "non-equivalent types not equal");
            Assert.IsFalse(EdmCoreModel.Instance.GetBoolean(false).IsEquivalentTo(EdmCoreModel.Instance.GetBinary(false)), "non-equivalent types not equal");
            Assert.IsFalse(EdmCoreModel.Instance.GetDecimal(false).IsEquivalentTo(EdmCoreModel.Instance.GetBoolean(false)), "non-equivalent types not equal");
            Assert.IsFalse(EdmCoreModel.Instance.GetString(false).IsEquivalentTo(EdmCoreModel.Instance.GetDecimal(false)), "non-equivalent types not equal");

            Assert.IsTrue(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal).IsEquivalentTo(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal)), "Primitive type is equivalent.");
            Assert.IsFalse(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal).IsEquivalentTo(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary)), "Primitive type is not equivalent.");

            Assert.IsTrue(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration).IsEquivalentTo(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration)), "Primitive type is equivalent.");
            Assert.IsFalse(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration).IsEquivalentTo(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset)), "Primitive type is not equivalent.");

            IEdmEntityReferenceTypeReference entityRef1 = new EdmEntityReferenceTypeReference(new EdmEntityReferenceType(new EdmEntityType("bar", "foo")), false);
            IEdmEntityReferenceTypeReference entityRef2 = new EdmEntityReferenceTypeReference(new EdmEntityReferenceType(new EdmEntityType("bar", "foo")), false);
            Assert.IsFalse(entityRef1.IsEquivalentTo(entityRef2), "Equivalent EntityReference/nominal types not equal for different type obj refs");

            entityRef1 = new EdmEntityReferenceTypeReference(new EdmEntityReferenceType(new EdmEntityType("bar", "foo")), false);
            entityRef2 = new EdmEntityReferenceTypeReference(new EdmEntityReferenceType(entityRef1.EntityReferenceDefinition().EntityType), false);
            Assert.IsTrue(entityRef1.IsEquivalentTo(entityRef2), "Equivalent EntityReference/nominal types equal");

            Assert.IsFalse(new EdmEntityType("", "").IsEquivalentTo(new EdmComplexType("", "")), "Different type kinds are not equivalent");
            Assert.IsFalse(new EdmEntityType("NS", "E1").IsEquivalentTo(new EdmEntityType("NS", "E2")), "Different names mean types are not equivalent");

            IEdmTypeReference type = EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Int32, false);
            Assert.IsTrue(type.IsEquivalentTo(type), "Same reference is equal to self");
        }

        [TestMethod]
        public void EqualityBinaryReferenceTypeTest()
        {
            var simpleBaseline = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false);
            var simpleDifferentNullibility = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), true);
            var simpleDifferentPrimitiveType = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Stream), false);
            var simpleDifferentReferenceType = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), true);
            var simpleMatch = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false);

            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmBinaryTypeReference(null, false));
            Assert.IsTrue(simpleBaseline.IsEquivalentTo(simpleMatch), "Is the same.");
            Assert.IsFalse(simpleBaseline.IsEquivalentTo(simpleDifferentNullibility), "Different nullibility.");
            Assert.IsFalse(simpleBaseline.IsEquivalentTo(simpleDifferentPrimitiveType), "Different primitive type.");

            Assert.IsTrue(simpleDifferentPrimitiveType.IsBad(), "simpleDifferentReferenceType is bad");
            Assert.IsFalse(simpleBaseline.IsEquivalentTo(simpleDifferentReferenceType));

            var baseline = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false, false, 3);
            var match = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false, false, 3);
            var differentMaxLength = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false, false, 2);
            var nullMaxLength = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false, false, null);
            var differentIsUnbounded = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false, true, null);
            var differentNullibility = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), true, false, 3);

            var differentPrimitiveType = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), false, false, 3);
            var differentTypeReference = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false);

            this.VerifyThrowsException(typeof(InvalidOperationException), () => new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false, true, 3));
            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmBinaryTypeReference(null, false, false, 3));
            Assert.IsTrue(baseline.IsEquivalentTo(match), "Is the same.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentMaxLength), "Different MaxLength.");
            Assert.IsFalse(baseline.IsEquivalentTo(nullMaxLength), "Null MaxLength.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentIsUnbounded), "Different IsUnbound.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentNullibility), "Different nullibility.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentPrimitiveType), "Different primtive type.");

            Assert.IsTrue(differentTypeReference.IsBad(), "differentTypeReference is bad");
            Assert.IsFalse(baseline.IsEquivalentTo(differentTypeReference));
        }

        [TestMethod]
        public void EqualityDecimalReferenceTypeTest()
        {
            var simpleBaseline = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), false);
            var simpleDifferentNullibility = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true);
            var simpleDifferentPrimitiveType = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false);
            var simpleMatch = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), false);

            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmDecimalTypeReference(null, false));
            Assert.IsTrue(simpleBaseline.IsEquivalentTo(simpleMatch), "Is the same.");
            Assert.IsFalse(simpleBaseline.IsEquivalentTo(simpleDifferentNullibility), "Different nullibility.");
            Assert.IsFalse(simpleBaseline.IsEquivalentTo(simpleDifferentPrimitiveType), "Different primitive type.");

            var baseline = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true, 1, 3);
            var match = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true, 1, 3);
            var differentScale = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true, 1, 4);
            var nullScale = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true, 1, null);
            var differentPrecision = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true, 2, 3);
            var nullPrecision = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true, null, 3);
            var differentNullibility = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), false, 1, 3);
            
            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.None), true, 1, 3));
            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmDecimalTypeReference(null, true, 1, 3));
            Assert.IsTrue(baseline.IsEquivalentTo(match), "Is the same.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentScale), "Different Scale.");
            Assert.IsFalse(baseline.IsEquivalentTo(nullScale), "Null Scale.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentPrecision), "Different Precision.");
            Assert.IsFalse(baseline.IsEquivalentTo(nullPrecision), "Null Precision.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentNullibility), "Different nullibility.");
        }

        [TestMethod]
        public void EqualityStringReferenceTypeTest()
        {
            var simpleBaseline = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false);
            var simpleDifferentNullibility = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true);
            var simpleDifferentPrimitiveType = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false);
            var simpleMatch = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false);

            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmStringTypeReference(null, false));
            Assert.IsTrue(simpleBaseline.IsEquivalentTo(simpleMatch), "Is the same.");
            Assert.IsFalse(simpleBaseline.IsEquivalentTo(simpleDifferentNullibility), "Different nullibility.");
            Assert.IsFalse(simpleBaseline.IsEquivalentTo(simpleDifferentPrimitiveType), "Different primitive type.");

            var baseline = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true, false, 4, true);
            var match = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true, false, 4, true);
            var differentUnicode = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true, false, 4, false);
            var nullUnicode = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true, false, 4, null);
            var differentMaxLength = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true, false, 5, true);
            var nullMaxLength = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true, false, null, true);
            var differentIsUnbounded = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true, true, null, true);

            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmStringTypeReference(null, true, false, 4, true));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true, true, 4, true));
            Assert.IsTrue(baseline.IsEquivalentTo(match), "Is the same.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentUnicode), "Different Unicode");
            Assert.IsFalse(baseline.IsEquivalentTo(nullUnicode), "Null Unicode");
            Assert.IsFalse(baseline.IsEquivalentTo(differentMaxLength), "Different MaxLength");
            Assert.IsFalse(baseline.IsEquivalentTo(nullMaxLength), "Null MaxLength");
            Assert.IsFalse(baseline.IsEquivalentTo(differentIsUnbounded), "Different IsUnbounded");
        }

        [TestMethod]
        public void EqualityTemporalReferenceTypeTest()
        {
            var simpleBaseline = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), false);
            var simpleDifferentNullibility = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), true);
            var simpleDifferentPrimitiveType = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration), false);
            var simpleMatch = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), false);

            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmTemporalTypeReference(null, false));
            Assert.IsTrue(simpleBaseline.IsEquivalentTo(simpleMatch), "Is the same.");
            Assert.IsFalse(simpleBaseline.IsEquivalentTo(simpleDifferentNullibility), "Different nullibility.");
            Assert.IsFalse(simpleBaseline.IsEquivalentTo(simpleDifferentPrimitiveType), "Different primitive type.");

            var baseline = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), true, 3);
            var match = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), true, 3);
            var differentPrecision = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), true, 4);
            var nullPrecision = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), true, null);

            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmTemporalTypeReference(null, true, 3));
            Assert.IsTrue(baseline.IsEquivalentTo(match), "Is the same.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentPrecision), "Different Precision.");
            Assert.IsFalse(baseline.IsEquivalentTo(nullPrecision), "Null Precision");
        }

        [TestMethod]
        public void EqualitySpatialReferenceTypeTest()
        {
            var simpleBaseline = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography), false);
            var simpleDifferentNullibility = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography), true);
            var simpleDifferentPrimitiveType = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyLineString), false);
            var simpleMatch = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography), false);

            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmSpatialTypeReference(null, false));
            Assert.IsTrue(simpleBaseline.IsEquivalentTo(simpleMatch), "Is the same.");
            Assert.IsFalse(simpleBaseline.IsEquivalentTo(simpleDifferentNullibility), "Different nullibility.");
            Assert.IsFalse(simpleBaseline.IsEquivalentTo(simpleDifferentPrimitiveType), "Different primitive type.");

            var baseline = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography), true, 3);
            var match = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography), true, 3);
            var differentId = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography), true, 4);
            var nullId = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography), true, null);

            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmTemporalTypeReference(null, true, 3));
            Assert.IsTrue(baseline.IsEquivalentTo(match), "Is the same.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentId), "Different Id.");
            Assert.IsFalse(baseline.IsEquivalentTo(nullId), "Null Id");
        }

        [TestMethod]
        public void EqualityComplexTypeReferenceTypeTest()
        {
            var simpleBaselineComplexType = new EdmComplexType("NS", "Baseline");
            var simpleDifferentNameComplexType= new EdmComplexType("NS", "Different");
            var simpleDifferentNamespaceComplexType = new EdmComplexType("Foo", "Base");

            var simpleBaseline = new EdmComplexTypeReference(simpleBaselineComplexType, true);
            var simpleMatch = new EdmComplexTypeReference(simpleBaselineComplexType, true);
            var simpleDifferentNullibility = new EdmComplexTypeReference(simpleBaselineComplexType, false);
            var simpleDifferentNameType = new EdmComplexTypeReference(simpleDifferentNameComplexType, true);
            var simpleDifferentNamespaceType = new EdmComplexTypeReference(simpleDifferentNamespaceComplexType, true);
            Assert.IsTrue(simpleBaseline.IsEquivalentTo(simpleMatch), "Is the same.");
            Assert.IsFalse(simpleBaseline.IsEquivalentTo(simpleDifferentNullibility), "Different Nullibility.");
            Assert.IsFalse(simpleBaseline.IsEquivalentTo(simpleDifferentNameType), "Different name simple complex type.");
            Assert.IsFalse(simpleBaseline.IsEquivalentTo(simpleDifferentNamespaceType), "Different namespace for simple complex type.");

            var baselineBaseComplexType = new EdmComplexType("NS", "Base");
            var fooBaseComplexType = new EdmComplexType("NS", "Foo");

            var baselineComplexType = new EdmComplexType("NS", "Baseline", baselineBaseComplexType, false);
            var differentBaseComplexType = new EdmComplexType("NS", "Baseline", fooBaseComplexType, false);
            var nullBaseTypeComplexType = new EdmComplexType("NS", "Baseline", null, false);
            var differentAbstractComplexType = new EdmComplexType("NS", "Baseline", baselineBaseComplexType, true);

            var baseline = new EdmComplexTypeReference(baselineComplexType, true);
            var match = new EdmComplexTypeReference(baselineComplexType, true);
            var differentBaseType = new EdmComplexTypeReference(differentBaseComplexType, true);
            var nullBaseType = new EdmComplexTypeReference(nullBaseTypeComplexType, true);
            var differentAbstractType = new EdmComplexTypeReference(differentAbstractComplexType, true);

            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmComplexTypeReference(null, true));
            Assert.IsTrue(baseline.IsEquivalentTo(match), "Is the same.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentBaseType), "Different base type.");
            Assert.IsFalse(baseline.IsEquivalentTo(nullBaseType), "Null base type.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentAbstractType), "Different abstract type.");
        }

        [TestMethod]
        public void EqualityComplexTypeTest()
        {
            var simpleBaselineComplexType = new EdmComplexType("NS", "Baseline");
            var simpleMatchComplexType = new EdmComplexType("NS", "Baseline");
            var simpleDifferentNameComplexType = new EdmComplexType("NS", "Different");
            var simpleDifferentNamespaceComplexType = new EdmComplexType("Foo", "Base");

            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmComplexType(null, "Baseline"));
            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmComplexType("NS", null));
            Assert.IsTrue(simpleBaselineComplexType.IsEquivalentTo(simpleBaselineComplexType), "Is the same.");
            Assert.IsFalse(simpleBaselineComplexType.IsEquivalentTo(simpleMatchComplexType), "Is the same, but different obj refs.");
            Assert.IsFalse(simpleBaselineComplexType.IsEquivalentTo(simpleDifferentNameComplexType), "Different name simple complex type.");
            Assert.IsFalse(simpleBaselineComplexType.IsEquivalentTo(simpleDifferentNamespaceComplexType), "Different namespace for simple complex type.");

            var baselineBaseComplexType = new EdmComplexType("NS", "Base");
            var fooBaseComplexType = new EdmComplexType("NS", "Foo");

            var baselineComplexType = new EdmComplexType("NS", "Baseline", baselineBaseComplexType, false);
            var matchComplexType = new EdmComplexType("NS", "Baseline", baselineBaseComplexType, false);
            var differentBaseComplexType = new EdmComplexType("NS", "Baseline", fooBaseComplexType, false);
            var nullBaseTypeComplexType = new EdmComplexType("NS", "Baseline", null, false);
            var differentAbstractComplexType = new EdmComplexType("NS", "Baseline", baselineBaseComplexType, true);

            Assert.IsTrue(baselineComplexType.IsEquivalentTo(baselineComplexType), "Is the same.");
            Assert.IsFalse(baselineComplexType.IsEquivalentTo(matchComplexType), "Is the same, but different obj refs.");
            Assert.IsFalse(baselineComplexType.IsEquivalentTo(differentBaseComplexType), "Different base type.");
            Assert.IsFalse(baselineComplexType.IsEquivalentTo(nullBaseTypeComplexType), "Null base type.");
            Assert.IsFalse(baselineComplexType.IsEquivalentTo(differentAbstractComplexType), "Different abstract type.");
        }

        [TestMethod]
        public void EqualityEnumTypeTest()
        {
            var baseline = new EdmEnumType("NS", "Baseline", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), true);
            var match = new EdmEnumType("NS", "Baseline", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), true);
            var differentNamespace = new EdmEnumType("foo", "Baseline", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), true);
            var differentName = new EdmEnumType("NS", "foo", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), true);
            var differentPrimitiveType = new EdmEnumType("NS", "Baseline", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int16), true);
            var differentFlag = new EdmEnumType("NS", "Baseline", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), false);

            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmEnumType(null, "Baseline", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), true));
            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmEnumType("NS", null, EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), true));
            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmEnumType("NS", "Baseline", null, true));

            Assert.IsTrue(baseline.IsEquivalentTo(baseline), "Is the same.");
            Assert.IsFalse(baseline.IsEquivalentTo(match), "Same but different obj refs");
            Assert.IsFalse(baseline.IsEquivalentTo(differentNamespace), "Different namespace.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentName), "Different name.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentPrimitiveType), "Different primitive type.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentFlag), "Different flag.");
        }

        [TestMethod]
        public void EqualityEntityTypeTest()
        {
            var baseEntityType = new EdmEntityType("NS", "Base", new EdmEntityType("NS", "BaseBase"), false, false);
            var differentBaseEntityType = new EdmEntityType("NS", "Base", new EdmEntityType("NS", "BaseBase"), true, false);

            var baseline = new EdmEntityType("NS", "Baseline", baseEntityType, false, false);
            baseline.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));

            var match = new EdmEntityType("NS", "Baseline", baseEntityType, false, false);
            match.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));

            var differentNamespace = new EdmEntityType("foo", "Baseline", baseEntityType, false, false);
            differentNamespace.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));

            var differentName = new EdmEntityType("NS", "foo", baseEntityType, false, false);
            differentName.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));

            var differentBaseType = new EdmEntityType("NS", "Baseline", differentBaseEntityType, false, false);
            differentBaseType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));

            var nullBaseType = new EdmEntityType("NS", "Baseline", null, false, false);
            nullBaseType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));

            var differentAbstract = new EdmEntityType("NS", "Baseline", baseEntityType, true, false);
            differentAbstract.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));

            var differentIsOpen = new EdmEntityType("NS", "Baseline", baseEntityType, false, true);
            differentIsOpen.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));

            var differentProperties = new EdmEntityType("NS", "Baseline", baseEntityType, false, false);
            differentProperties.AddStructuralProperty("foo", EdmCoreModel.Instance.GetInt32(false));

            var differentKey = new EdmEntityType("NS", "Baseline", baseEntityType, false, false);
            var differentKeyId = differentProperties.AddStructuralProperty("id", EdmCoreModel.Instance.GetInt32(false));
            differentKey.AddKeys(differentKeyId);

            Assert.IsTrue(baseline.IsEquivalentTo(baseline), "Is the same.");
            Assert.IsFalse(baseline.IsEquivalentTo(match), "Is the same, different obj refs.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentNamespace), "Different namespace.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentName), "Different name.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentBaseType), "Different base type.");
            Assert.IsFalse(baseline.IsEquivalentTo(nullBaseType), "Null base type.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentAbstract), "Different abstract.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentIsOpen), "Different is open.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentProperties), "Different properties.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentKey), "Different key.");
        }

        [TestMethod]
        public void EqualityCollectionTypeTest()
        {
            var baselineCollectionTypeElement = new EdmEntityType("NS", "Baseline", new EdmEntityType("NS", "foo", null, true, false), true, false);
            var differentCollectionTypeElement = new EdmEntityType("NS", "Baseline", new EdmEntityType("NS", "bar", null, false, false), true, false);

            var baseline = new EdmCollectionType(new EdmEntityTypeReference(baselineCollectionTypeElement, true));
            var match = new EdmCollectionType(new EdmEntityTypeReference(baselineCollectionTypeElement, true));
            var differentCollectionNullibility = new EdmCollectionType(new EdmEntityTypeReference(baselineCollectionTypeElement, false));
            var differentCollectionType = new EdmCollectionType(new EdmEntityTypeReference(differentCollectionTypeElement, true));

            Assert.IsTrue(baseline.IsEquivalentTo(match), "Is the same.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentCollectionNullibility), "Different nullibility.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentCollectionType), "Different collection type element (different obj refs).");
            differentCollectionType = new EdmCollectionType(new EdmEntityTypeReference(baselineCollectionTypeElement, true));
            Assert.IsTrue(baseline.IsEquivalentTo(differentCollectionType), "Different collection types equal on element.");
        }

        [TestMethod]
        public void EqualityEntityReferenceTypeTests()
        {
            var baselineEntityType = new EdmEntityType("NS", "Baseline", new EdmEntityType("NS", "foo", null, true, false), true, false);
            var differentBaselineEntityType = new EdmEntityType("NS", "Baseline", new EdmEntityType("NS", "bar", null, false, false), true, false);

            var baseline = new EdmEntityReferenceType(baselineEntityType);
            var match = new EdmEntityReferenceType(baselineEntityType);
            var differentEntityType = new EdmEntityReferenceType(differentBaselineEntityType);

            Assert.IsTrue(baseline.IsEquivalentTo(match), "Is the same.");
            Assert.IsFalse(baseline.IsEquivalentTo(differentEntityType), "Different entity type.");
        }

        [TestMethod]
        public void NullComparisonEquivalence()
        {
            EdmEntityType entity = new EdmEntityType("Foo", "Bar", null, false, false);

            Assert.IsFalse(entity.IsEquivalentTo(null), "Type RHS");
            Assert.IsFalse(((IEdmEntityType)null).IsEquivalentTo(entity), "Type LHS");
            Assert.IsTrue(((IEdmEntityType)null).IsEquivalentTo(null), "Type Both");

            Assert.IsFalse(new EdmEntityTypeReference(entity, false).IsEquivalentTo(null), "TypeReference RHS");
            Assert.IsFalse(((IEdmEntityTypeReference)null).IsEquivalentTo(new EdmEntityTypeReference(entity, false)), "TypeReference LHS");
            Assert.IsTrue(((IEdmEntityTypeReference)null).IsEquivalentTo(null), "TypeReference Both");
        }

        [TestMethod]
        public void AsStructuredCanConvertIfTypeNotOrigionallyStructured()
        {
            // Csdl gives us "CsdlSemanticsNamedTypeReference" to start with, so converting to structured should give us the proper interface implementing types
            string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""bar"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""otherID"" Type=""DateTimeOffset"" />
    </EntityType>
    <EntityType Name=""other"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    </EntityType>
    <ComplexType Name=""baz"">
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
    </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
        }

        [TestMethod]
        public void AsPrimitiveCanConvertIfTypeNotOrigionallyPrimitive()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""myBinary"" Type=""Edm.Binary"" MaxLength=""64"" />
    <Property Name=""myBinaryMax"" Type=""Edm.Binary"" MaxLength=""Max"" />
    <Property Name=""myBinaryOtherMax"" Type=""Edm.Binary"" MaxLength=""max"" />
    <Property Name=""myBoolean"" Type=""Edm.Boolean"" />
    <Property Name=""myDateTime"" Type=""Edm.DateTimeOffset"" Precision=""2"" />
    <Property Name=""myTime"" Type=""Edm.Duration"" Precision=""3"" />
    <Property Name=""myDateTimeOffset"" Type=""Edm.DateTimeOffset"" Precision=""1"" />
    <Property Name=""myDecimal"" Type=""Edm.Decimal"" DefaultValue=""3.5"" Precision=""3"" Scale=""2"" />
    <Property Name=""myFacetlessDecimal"" Type=""Edm.Decimal"" />
    <Property Name=""mySingle"" Type=""Edm.Single"" Nullable=""false"" />
    <Property Name=""myDouble"" Type=""Edm.Double"" Nullable=""false"" />
    <Property Name=""myGuid"" Type=""Edm.Guid"" />
    <Property Name=""mySByte"" Type=""Edm.SByte"" />
    <Property Name=""myInt16"" Type=""Edm.Int16"" />
    <Property Name=""myInt32"" Type=""Edm.Int32"" />
    <Property Name=""myInt64"" Type=""Edm.Int64"" />
    <Property Name=""myByte"" Type=""Edm.Byte"" />
    <Property Name=""myStream"" Type=""Edm.Stream"" />
    <Property Name=""myString"" Type=""Edm.String"" DefaultValue=""BorkBorkBork"" MaxLength=""128"" Unicode=""false"" />
    <Property Name=""myStringMax"" Type=""Edm.String"" MaxLength=""Max"" Unicode=""false"" />
    <Property Name=""myGeography"" Type=""Edm.Geography"" SRID=""1"" />
    <Property Name=""myPoint"" Type=""Edm.GeographyPoint"" SRID=""2"" />
    <Property Name=""myLineString"" Type=""Edm.GeographyLineString"" SRID=""3"" />
    <Property Name=""myPolygon"" Type=""Edm.GeographyPolygon"" SRID=""4"" />
    <Property Name=""myGeographyCollection"" Type=""Edm.GeographyCollection"" SRID=""5"" />
    <Property Name=""myMultiPolygon"" Type=""Edm.GeographyMultiPolygon"" SRID=""6"" />
    <Property Name=""myMultiLineString"" Type=""Edm.GeographyMultiLineString"" SRID=""7"" />
    <Property Name=""myMultiPoint"" Type=""Edm.GeographyMultiPoint"" SRID=""8"" />
    <Property Name=""myGeometry"" Type=""Edm.Geometry"" SRID=""9"" />
    <Property Name=""myGeometricPoint"" Type=""Edm.GeometryPoint"" SRID=""10"" />
    <Property Name=""myGeometricLineString"" Type=""Edm.GeometryLineString"" SRID=""11"" />
    <Property Name=""myGeometricPolygon"" Type=""Edm.GeometryPolygon"" SRID=""12"" />
    <Property Name=""myGeometryCollection"" Type=""Edm.GeometryCollection"" SRID=""13"" />
    <Property Name=""myGeometricMultiPolygon"" Type=""Edm.GeometryMultiPolygon"" SRID=""14"" />
    <Property Name=""myGeometricMultiLineString"" Type=""Edm.GeometryMultiLineString"" SRID=""15"" />
    <Property Name=""myGeometricMultiPoint"" Type=""Edm.GeometryMultiPoint"" SRID=""Variable"" />
  </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");

            IEdmComplexType complex = (IEdmComplexType)model.FindType("Grumble.Smod");

            IEdmTypeReference myBinary = complex.FindProperty("myBinary").Type;
            IEdmTypeReference myBoolean = complex.FindProperty("myBoolean").Type;
            IEdmTypeReference myDateTime = complex.FindProperty("myDateTime").Type;
            IEdmTypeReference myTime = complex.FindProperty("myTime").Type;
            IEdmTypeReference myDateTimeOffset = complex.FindProperty("myDateTimeOffset").Type;
            IEdmTypeReference myDecimal = complex.FindProperty("myDecimal").Type;
            IEdmTypeReference mySingle = complex.FindProperty("mySingle").Type;
            IEdmTypeReference myDouble = complex.FindProperty("myDouble").Type;
            IEdmTypeReference myGuid = complex.FindProperty("myGuid").Type;
            IEdmTypeReference mySByte = complex.FindProperty("mySByte").Type;
            IEdmTypeReference myInt16 = complex.FindProperty("myInt16").Type;
            IEdmTypeReference myInt32 = complex.FindProperty("myInt32").Type;
            IEdmTypeReference myInt64 = complex.FindProperty("myInt64").Type;
            IEdmTypeReference myByte = complex.FindProperty("myByte").Type;
            IEdmTypeReference myStream = complex.FindProperty("myStream").Type;
            IEdmTypeReference myString = complex.FindProperty("myString").Type;
            IEdmTypeReference myGeography = complex.FindProperty("myGeography").Type;
            IEdmTypeReference myPoint = complex.FindProperty("myPoint").Type;
            IEdmTypeReference myLineString = complex.FindProperty("myLineString").Type;
            IEdmTypeReference myPolygon = complex.FindProperty("myPolygon").Type;
            IEdmTypeReference myGeographyCollection = complex.FindProperty("myGeographyCollection").Type;
            IEdmTypeReference myMultiPolygon = complex.FindProperty("myMultiPolygon").Type;
            IEdmTypeReference myMultiLineString = complex.FindProperty("myMultiLineString").Type;
            IEdmTypeReference myMultiPoint = complex.FindProperty("myMultiPoint").Type;
            IEdmTypeReference myGeometry = complex.FindProperty("myGeometry").Type;
            IEdmTypeReference myGeometricPoint = complex.FindProperty("myGeometricPoint").Type;
            IEdmTypeReference myGeometricLineString = complex.FindProperty("myGeometricLineString").Type;
            IEdmTypeReference myGeometricPolygon = complex.FindProperty("myGeometricPolygon").Type;
            IEdmTypeReference myGeometryCollection = complex.FindProperty("myGeometryCollection").Type;
            IEdmTypeReference myGeometricMultiPolygon = complex.FindProperty("myGeometricMultiPolygon").Type;
            IEdmTypeReference myGeometricMultiPoint = complex.FindProperty("myGeometricMultiPoint").Type;

            Assert.IsTrue(myBinary.AsPrimitive() is IEdmBinaryTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myBoolean.AsPrimitive() is IEdmPrimitiveTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myDateTime.AsPrimitive() is IEdmTemporalTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myTime.AsPrimitive() is IEdmTemporalTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myDateTimeOffset.AsPrimitive() is IEdmTemporalTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myDecimal.AsPrimitive() is IEdmDecimalTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myDouble.AsPrimitive() is IEdmPrimitiveTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myGuid.AsPrimitive() is IEdmPrimitiveTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(mySByte.AsPrimitive() is IEdmPrimitiveTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(mySingle.AsPrimitive() is IEdmPrimitiveTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myInt16.AsPrimitive() is IEdmPrimitiveTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myInt32.AsPrimitive() is IEdmPrimitiveTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myInt64.AsPrimitive() is IEdmPrimitiveTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myByte.AsPrimitive() is IEdmPrimitiveTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myStream.AsPrimitive() is IEdmPrimitiveTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myString.AsPrimitive() is IEdmStringTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myGeography.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myPoint.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myLineString.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myPolygon.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myGeographyCollection.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myMultiPolygon.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myMultiLineString.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myMultiPoint.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myGeometry.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myGeometricPoint.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myGeometricLineString.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myGeometricPolygon.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myGeometryCollection.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myGeometricMultiPolygon.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
            Assert.IsTrue(myGeometricMultiPoint.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
        }

        [TestMethod]
        public void UnresolvedFunctionTest()
        {
            const string annotatingModelCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""foo.CoolPersonTerm"">
            <Record>
                <PropertyValue Property=""Street"">
                    <Apply Function=""foo.BorkBorkBork""/>
                </PropertyValue>
            </Record>
        </Annotation>
    </Annotations>
</Schema>";

            const string baseModelCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""DistantAge"" Type=""Int32"" />
    <Term Name=""NewAge"" Type=""Int32"" />
    <Term Name=""Punning"" Type=""Boolean"" />
    <Term Name=""Clear"" Type=""Boolean"" />
    <Term Name=""CoolPersonTerm"" Type=""foo.CoolPerson"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
        <Property Name=""CoolnessIndex"" Type=""Int32"" />
        <Property Name=""Living"" Type=""Boolean"" />
        <Property Name=""Famous"" Type=""Boolean"" />
    </EntityType>
</Schema>";
            IEnumerable<EdmError> errors;
            IEdmModel model;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(annotatingModelCsdl)), XmlReader.Create(new StringReader(baseModelCsdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType person = (IEdmEntityType)model.FindType("foo.Person");
            IEdmVocabularyAnnotation valueAnnotation = person.VocabularyAnnotations(model).First();
            var property = ((IEdmRecordExpression)valueAnnotation.Value).Properties.First();
            IEdmOperation badOperation = ((IEdmApplyExpression)property.Value).AppliedFunction;
            Assert.AreEqual("foo", badOperation.Namespace, "Bad function has correct namespace.");
            Assert.AreEqual("BorkBorkBork", badOperation.Name, "Bad function has correct name.");
            Assert.AreEqual(EdmSchemaElementKind.Function, badOperation.SchemaElementKind, "Bad function has correct schema kind.");
            Assert.IsTrue(badOperation.ReturnType.Definition.IsBad(), "Bad function has bad return return type.");
            Assert.AreEqual(null, badOperation.FindParameter("Foo"), "Bad function returns null for parameters.");
            Assert.AreEqual(0, badOperation.Parameters.Count(), "Bad function has no parameters.");
        }
    }
}
