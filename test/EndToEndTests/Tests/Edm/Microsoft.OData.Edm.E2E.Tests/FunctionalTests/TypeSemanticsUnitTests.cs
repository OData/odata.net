//---------------------------------------------------------------------
// <copyright file="TypeSemanticsUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

public class TypeSemanticsUnitTests : EdmLibTestCaseBase
{
    [Fact]
    public void NonPrimitiveIsXXXMethods()
    {
        IEdmEntityType entityDef = new EdmEntityType("MyNamespace", "MyEntity");
        IEdmEntityTypeReference entityRef = new EdmEntityTypeReference(entityDef, false);

        Assert.True(entityRef.IsEntity());

        IEdmPrimitiveTypeReference bad = entityRef.AsPrimitive();
        Assert.True(bad.Definition.IsBad());
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, bad.Definition.Errors().First().ErrorCode);
        Assert.True(bad.Definition.IsBad());
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, bad.Definition.Errors().First().ErrorCode);

        IEdmPrimitiveType intDef = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32);
        IEdmPrimitiveTypeReference intRef = new EdmPrimitiveTypeReference(intDef, false);
        IEdmCollectionTypeReference intCollection = new EdmCollectionTypeReference(new EdmCollectionType(intRef));
        Assert.True(intCollection.IsCollection());

        IEdmComplexType complexDef = new EdmComplexType("MyNamespace", "MyComplex");
        IEdmComplexTypeReference complexRef = new EdmComplexTypeReference(complexDef, false);
        Assert.True(complexRef.IsComplex());

        Assert.True(entityRef.IsStructured());
        Assert.True(complexRef.IsStructured());
        Assert.False(intCollection.IsStructured());
    }

    [Fact]
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

        Assert.True(binaryRef.IsPrimitive());
        Assert.True(binaryRef.IsBinary());
        Assert.True(booleanRef.IsBoolean());
        Assert.True(byteRef.IsByte());
        Assert.True(dateTimeOffsetRef.IsDateTimeOffset());
        Assert.True(decimalRef.IsDecimal());
        Assert.True(doubleRef.IsDouble());
        Assert.True(guidRef.IsGuid());
        Assert.True(int16Ref.IsInt16());
        Assert.True(int32Ref.IsInt32());
        Assert.True(int64Ref.IsInt64());
        Assert.True(sByteRef.IsSByte(), "SByte is SByte");
        Assert.True(singleRef.IsSingle(), "Single is Single");
        Assert.True(stringRef.IsString(), "String is String");
        Assert.True(streamRef.IsStream(), "Stream is Stream");
        Assert.True(timeRef.IsDuration(), "Duration is Duration");
        Assert.True(dateRef.IsDate(), "Date is Date");
        Assert.True(timeOfDayRef.IsTimeOfDay(), "TimeOfDay is TimeOfDay");

        Assert.True(binaryRef.AsPrimitive().IsPrimitive(), "Binary as primitive is Primitive");
        Assert.True(binaryRef.AsPrimitive().IsBinary(), "Binary as primitive is Binary");
        Assert.True(booleanRef.AsPrimitive().IsBoolean(), "Boolean as primitive is Boolean");
        Assert.True(byteRef.AsPrimitive().IsByte(), "Byte as primitive is Byte");
        Assert.True(dateTimeOffsetRef.AsPrimitive().IsDateTimeOffset(), "DateTimeOffset as primitive is DateTimeOffset");
        Assert.True(decimalRef.AsPrimitive().IsDecimal(), "Decimal as primitive is Decimal");
        Assert.True(doubleRef.AsPrimitive().IsDouble(), "Double as primitive is Double");
        Assert.True(guidRef.AsPrimitive().IsGuid(), "Guid as primitive is Guid");
        Assert.True(int16Ref.AsPrimitive().IsInt16(), "Int16 as primitive is Int16");
        Assert.True(int32Ref.AsPrimitive().IsInt32(), "Int32 as primitive is Int32");
        Assert.True(int64Ref.AsPrimitive().IsInt64(), "Int64 as primitive is Int64");
        Assert.True(sByteRef.AsPrimitive().IsSByte(), "SByte as primitive is SByte");
        Assert.True(singleRef.AsPrimitive().IsSingle(), "Single as primitive is Single");
        Assert.True(stringRef.AsPrimitive().IsString(), "String as primitive is String");
        Assert.True(streamRef.AsPrimitive().IsStream(), "Stream as primitive is Stream");
        Assert.True(timeRef.AsPrimitive().IsDuration(), "Duration as primitive is Duration");
        Assert.True(dateRef.AsPrimitive().IsDate(), "Date as primitive is Date");
        Assert.True(timeOfDayRef.AsPrimitive().IsTimeOfDay(), "TimeOfDay as primitive is TimeOfDay");

        Assert.True(dateTimeOffsetRef.IsTemporal(), "DateTimeOffset is Temporal");
        Assert.True(timeRef.IsTemporal(), "Duration is Temporal");
        Assert.True(timeOfDayRef.IsTemporal(), "TimeOfDay is Temporal");
        Assert.False(int32Ref.IsTemporal(), "Int is not Temporal");

        Assert.True(doubleRef.IsFloating(), "Double is floating");
        Assert.True(singleRef.IsFloating(), "Single is floating");
        Assert.False(int32Ref.IsFloating(), "Int is not floating");

        Assert.True(sByteRef.IsSignedIntegral(), "SByte is signed integral");
        Assert.True(int16Ref.IsSignedIntegral(), "Int16 is signed integral");
        Assert.True(int32Ref.IsSignedIntegral(), "Int32 is signed integral");
        Assert.True(int64Ref.IsSignedIntegral(), "Int64 is signed integral");
        Assert.False(stringRef.IsSignedIntegral(), "String is not signed integral");

        IEdmEntityType entityDef = new EdmEntityType("MyNamespace", "MyEntity");
        IEdmEntityTypeReference entityRef = new EdmEntityTypeReference(entityDef, false);
        Assert.Equal(EdmPrimitiveTypeKind.None, entityRef.PrimitiveKind());
        Assert.Equal(EdmSchemaElementKind.TypeDefinition, int16Ref.PrimitiveDefinition().SchemaElementKind);
    }

    [Fact]
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

        Assert.True(badCollectionRef.IsBad(), "Bad Collection is bad");
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badCollectionRef.Errors().First().ErrorCode);
        Assert.Equal("[Collection([UnknownType Nullable=True]) Nullable=True]", badCollectionRef.ToString());

        Assert.True(badComplexRef.IsBad(), "Bad Complex is Bad");
        Assert.True(badComplexRef.Definition.IsBad(), "Bad Complex definition is Bad");
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badComplexRef.Definition.Errors().First().ErrorCode);
        Assert.Equal("TypeSemanticsCouldNotConvertTypeReference:[MyNamespace.MyEntity Nullable=False]", badComplexRef.ToString());

        Assert.True(badEntityRef.IsBad(), "Bad Entity is bad");
        Assert.True(badEntityRef.Definition.IsBad(), "Bad Entity Definition is bad");
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badEntityRef.Definition.Errors().First().ErrorCode);
        Assert.Equal("TypeSemanticsCouldNotConvertTypeReference:[Collection(Edm.Int32) Nullable=False]", badEntityRef.ToString());

        Assert.True(badEntityRefRef.IsBad(), "Bad Entity Reference is Bad");
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badEntityRefRef.Errors().First().ErrorCode);
        Assert.Equal("[EntityReference(.) Nullable=False]", badEntityRefRef.ToString());

        Assert.False(entityRef.AsEntity().IsBad(), "Entity converted to Entity is good");
        Assert.False(complexRef.AsComplex().IsBad(), "Complex converted to complex is good");
        Assert.False(collectionRef.AsCollection().IsBad(), "Collection converted to collection is good");

        Assert.True(entityRef.AsStructured().IsEntity(), "Entity as structured is entity");
        Assert.False(entityRef.AsStructured().IsBad(), "Entity as structured is good");
        Assert.True(complexRef.AsStructured().IsComplex(), "Complex as structured is complex");
        Assert.False(complexRef.AsStructured().IsBad(), "Complex as structured is good");
        Assert.False(collectionRef.AsStructured().IsCollection(), "Collection as structured is not collection");
        Assert.True(collectionRef.AsStructured().IsBad(), "Collection as structured is bad");
        Assert.True(collectionRef.AsStructured().Definition.IsBad(), "Collection as structured definition is bad");
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, collectionRef.AsStructured().Definition.Errors().First().ErrorCode);
    }

    [Fact]
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

        Assert.True(badTemporal.IsBad(), "Bad Temporal is Bad");
        Assert.True(badTemporal.Definition.IsBad(), "Bad Temporal Definition is Bad");
        Assert.Equal(EdmErrorCode.InterfaceCriticalKindValueMismatch, badTemporal.Errors().First().ErrorCode);
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badTemporal.Definition.Errors().First().ErrorCode);
        Assert.Equal("TypeSemanticsCouldNotConvertTypeReference:[MyNamespace.BadEntity Nullable=False]", badTemporal.ToString());
        
        Assert.True(badDecimal.IsBad(), "Bad Decimal is Bad");
        Assert.True(badDecimal.Definition.IsBad(), "Bad Decimal definition is Bad");
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badDecimal.Definition.Errors().First().ErrorCode);
        Assert.Equal("TypeSemanticsCouldNotConvertTypeReference:[MyNamespace.BadEntity Nullable=False]", badDecimal.ToString());

        Assert.True(badString.IsBad(), "Bad String is Bad");
        Assert.True(badString.Definition.IsBad(), "Bad String Definition is Bad");
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badString.Definition.Errors().First().ErrorCode);
        Assert.Equal("TypeSemanticsCouldNotConvertTypeReference:[MyNamespace.BadEntity Nullable=False Unicode=False]", badString.ToString());

        Assert.True(badStream.IsBad(), "Bad Stream is Bad");
        Assert.True(badStream.Definition.IsBad(), "Bad Stream Definition is Bad");
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badStream.Definition.Errors().First().ErrorCode);
        Assert.Equal("TypeSemanticsCouldNotConvertTypeReference:[MyNamespace.BadEntity Nullable=False]", badStream.ToString());

        Assert.True(badBinary.IsBad(), "Bad Binary is Bad");
        Assert.True(badBinary.Definition.IsBad(), "Bad Binary Definition is Bad");
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badBinary.Definition.Errors().First().ErrorCode);
        Assert.Equal("TypeSemanticsCouldNotConvertTypeReference:[MyNamespace.BadEntity Nullable=False]", badBinary.ToString());
        
        Assert.True(badSpatial.IsBad(), "Bad Spatial is Bad");
        Assert.True(badSpatial.Definition.IsBad(), "Bad Spatial Definition is Bad");
        Assert.Equal(EdmErrorCode.InterfaceCriticalKindValueMismatch, badSpatial.Errors().First().ErrorCode);
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badSpatial.Definition.Errors().First().ErrorCode);
        Assert.Equal("TypeSemanticsCouldNotConvertTypeReference:[MyNamespace.BadEntity Nullable=False]", badSpatial.ToString());

        Assert.True(badPrimitive.IsBad(), "Bad Primitive is Bad");
        Assert.True(badPrimitive.Definition.IsBad(), "Bad Primitive Definition is Bad");
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badPrimitive.Definition.Errors().First().ErrorCode);
        Assert.Equal("TypeSemanticsCouldNotConvertTypeReference:[MyNamespace.BadEntity Nullable=False]", badPrimitive.ToString());
    }

    [Fact]
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

        Assert.True(badCollectionRef.IsCollection(), "Bad collection is collection");

        Assert.True(badEntityRef.IsEntity(), "Bad Entity is Entity");
        Assert.Empty(badEntityRef.Key());
        Assert.Equal("MyNamespace.BadComplex", badEntityRef.FullName());
        Assert.Equal(EdmSchemaElementKind.TypeDefinition, badEntityRef.EntityDefinition().SchemaElementKind);
        Assert.Null(badEntityRef.EntityDefinition().BaseType);
        Assert.Equal(Enumerable.Empty<IEdmProperty>(), badEntityRef.EntityDefinition().DeclaredProperties);
        Assert.False(badEntityRef.EntityDefinition().IsAbstract, "Bad structured types are not abstract");
        Assert.False(badEntityRef.EntityDefinition().IsOpen, "Bad structured types are not open");
        model.SetAnnotationValue(badEntityRef.Definition, "foo", "bar", new EdmStringConstant(new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false), "baz"));
        Assert.Single(model.DirectValueAnnotations(badEntityRef.Definition));
        Assert.NotNull(model.GetAnnotationValue(badEntityRef.Definition, "foo", "bar"));

        Assert.True(badComplexRef.IsComplex(), "Bad Complex is Bad");
        Assert.Null(badComplexRef.ComplexDefinition().FindProperty("PropertyName"));

        Assert.Equal(EdmTypeKind.EntityReference, badEntityRefRef.TypeKind());
        Assert.Equal(string.Empty, badEntityRefRef.EntityType().Name);
        model.SetAnnotationValue(badEntityRefRef.Definition, "foo", "bar", new EdmStringConstant(new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false), "baz"));
        Assert.Single(model.DirectValueAnnotations(badEntityRefRef.Definition));
        Assert.NotNull(model.GetAnnotationValue(badEntityRefRef.Definition, "foo", "bar"));

        Assert.Equal(EdmPrimitiveTypeKind.None, badPrimitive.PrimitiveKind());
        Assert.Equal(EdmTypeKind.Primitive, badPrimitive.TypeKind());
        Assert.Equal(EdmSchemaElementKind.TypeDefinition, badPrimitive.PrimitiveDefinition().SchemaElementKind);
        Assert.Equal("BadEntity", badPrimitive.PrimitiveDefinition().Name);
        Assert.Equal("MyNamespace", badPrimitive.PrimitiveDefinition().Namespace);

        Assert.Equal(EdmTypeKind.Enum, badEnum.TypeKind());
        Assert.Equal(EdmPrimitiveTypeKind.Int32, badEnum.EnumDefinition().UnderlyingType.PrimitiveKind);
        Assert.Empty(badEnum.EnumDefinition().Members);
        Assert.False(badEnum.EnumDefinition().IsFlags);
        Assert.Equal(EdmSchemaElementKind.TypeDefinition, badEnum.EnumDefinition().SchemaElementKind);
        Assert.Equal("BadEntity", badEnum.EnumDefinition().Name);
        Assert.Equal("MyNamespace", badEnum.EnumDefinition().Namespace);

        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badSpatial.Definition.Errors().First().ErrorCode);
    }

    [Fact]
    public void EqualityMainPathTests()
    {
        Assert.True(EdmCoreModel.Instance.GetBinary(false).IsEquivalentTo(EdmCoreModel.Instance.GetBinary(false)), "Type reference equal");
        Assert.True(EdmCoreModel.Instance.GetBoolean(false).IsEquivalentTo(EdmCoreModel.Instance.GetBoolean(false)), "Type reference equal");
        Assert.True(EdmCoreModel.Instance.GetDecimal(false).IsEquivalentTo(EdmCoreModel.Instance.GetDecimal(false)), "Type reference equal");
        Assert.True(EdmCoreModel.Instance.GetString(false).IsEquivalentTo(EdmCoreModel.Instance.GetString(false)), "Type reference equal");
        Assert.True(EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false).IsEquivalentTo(EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false)), "Type reference equal");
        Assert.True(EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false)).IsEquivalentTo(EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false))), "Type reference equal");

        Assert.False(EdmCoreModel.Instance.GetBinary(false).IsEquivalentTo(EdmCoreModel.Instance.GetBinary(true)), "Unequal facets not equal");
        Assert.False(EdmCoreModel.Instance.GetBoolean(false).IsEquivalentTo(EdmCoreModel.Instance.GetBoolean(true)), "Unequal facets not equal");
        Assert.False(EdmCoreModel.Instance.GetDecimal(false).IsEquivalentTo(EdmCoreModel.Instance.GetDecimal(true)), "Unequal facets not equal");
        Assert.False(EdmCoreModel.Instance.GetString(false).IsEquivalentTo(EdmCoreModel.Instance.GetString(true)), "Unequal facets not equal");
        Assert.False(EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false).Equals(EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, true)), "Unequal facets not equal");

        Assert.False(EdmCoreModel.Instance.GetBinary(false).IsEquivalentTo(EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false)), "non-equivalent types not equal");
        Assert.False(EdmCoreModel.Instance.GetBoolean(false).IsEquivalentTo(EdmCoreModel.Instance.GetBinary(false)), "non-equivalent types not equal");
        Assert.False(EdmCoreModel.Instance.GetDecimal(false).IsEquivalentTo(EdmCoreModel.Instance.GetBoolean(false)), "non-equivalent types not equal");
        Assert.False(EdmCoreModel.Instance.GetString(false).IsEquivalentTo(EdmCoreModel.Instance.GetDecimal(false)), "non-equivalent types not equal");

        Assert.True(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal).IsEquivalentTo(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal)), "Primitive type is equivalent.");
        Assert.False(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal).IsEquivalentTo(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary)), "Primitive type is not equivalent.");

        Assert.True(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration).IsEquivalentTo(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration)), "Primitive type is equivalent.");
        Assert.False(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration).IsEquivalentTo(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset)), "Primitive type is not equivalent.");

        IEdmEntityReferenceTypeReference entityRef1 = new EdmEntityReferenceTypeReference(new EdmEntityReferenceType(new EdmEntityType("bar", "foo")), false);
        IEdmEntityReferenceTypeReference entityRef2 = new EdmEntityReferenceTypeReference(new EdmEntityReferenceType(new EdmEntityType("bar", "foo")), false);
        Assert.False(entityRef1.IsEquivalentTo(entityRef2), "Equivalent EntityReference/nominal types not equal for different type obj refs");

        entityRef1 = new EdmEntityReferenceTypeReference(new EdmEntityReferenceType(new EdmEntityType("bar", "foo")), false);
        entityRef2 = new EdmEntityReferenceTypeReference(new EdmEntityReferenceType(entityRef1.EntityReferenceDefinition().EntityType), false);
        Assert.True(entityRef1.IsEquivalentTo(entityRef2), "Equivalent EntityReference/nominal types equal");

        Assert.False(new EdmEntityType("", "").IsEquivalentTo(new EdmComplexType("", "")), "Different type kinds are not equivalent");
        Assert.False(new EdmEntityType("NS", "E1").IsEquivalentTo(new EdmEntityType("NS", "E2")), "Different names mean types are not equivalent");

        IEdmTypeReference type = EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Int32, false);
        Assert.True(type.IsEquivalentTo(type), "Same reference is equal to self");
    }

    [Fact]
    public void EqualityBinaryReferenceTypeTest()
    {
        var simpleBaseline = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false);
        var simpleDifferentNullibility = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), true);
        var simpleDifferentPrimitiveType = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Stream), false);
        var simpleDifferentReferenceType = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), true);
        var simpleMatch = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false);

        Assert.Throws<ArgumentNullException>(() => new EdmBinaryTypeReference(null, false));

        Assert.True(simpleBaseline.IsEquivalentTo(simpleMatch), "Is the same.");
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentNullibility), "Different nullibility.");
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentPrimitiveType), "Different primitive type.");

        Assert.True(simpleDifferentPrimitiveType.IsBad(), "simpleDifferentReferenceType is bad");
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentReferenceType));

        var baseline = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false, false, 3);
        var match = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false, false, 3);
        var differentMaxLength = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false, false, 2);
        var nullMaxLength = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false, false, null);
        var differentIsUnbounded = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false, true, null);
        var differentNullibility = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), true, false, 3);

        var differentPrimitiveType = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), false, false, 3);
        var differentTypeReference = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false);

        Assert.Throws<InvalidOperationException>(() => new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false, true, 3));
        Assert.Throws<ArgumentNullException>(() => new EdmBinaryTypeReference(null, false, false, 3));

        Assert.True(baseline.IsEquivalentTo(match), "Is the same.");
        Assert.False(baseline.IsEquivalentTo(differentMaxLength), "Different MaxLength.");
        Assert.False(baseline.IsEquivalentTo(nullMaxLength), "Null MaxLength.");
        Assert.False(baseline.IsEquivalentTo(differentIsUnbounded), "Different IsUnbound.");
        Assert.False(baseline.IsEquivalentTo(differentNullibility), "Different nullibility.");
        Assert.False(baseline.IsEquivalentTo(differentPrimitiveType), "Different primtive type.");

        Assert.True(differentTypeReference.IsBad(), "differentTypeReference is bad");
        Assert.False(baseline.IsEquivalentTo(differentTypeReference));
    }

    [Fact]
    public void EqualityDecimalReferenceTypeTest()
    {
        var simpleBaseline = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), false);
        var simpleDifferentNullibility = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true);
        var simpleDifferentPrimitiveType = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false);
        var simpleMatch = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), false);

        Assert.Throws<ArgumentNullException>(() => new EdmDecimalTypeReference(null, false));
        Assert.True(simpleBaseline.IsEquivalentTo(simpleMatch), "Is the same.");
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentNullibility), "Different nullibility.");
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentPrimitiveType), "Different primitive type.");

        var baseline = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true, 1, 3);
        var match = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true, 1, 3);
        var differentScale = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true, 1, 4);
        var nullScale = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true, 1, null);
        var differentPrecision = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true, 2, 3);
        var nullPrecision = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true, null, 3);
        var differentNullibility = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), false, 1, 3);
        
        Assert.Throws<ArgumentNullException>(() => new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.None), true, 1, 3));
        Assert.Throws<ArgumentNullException>(() => new EdmDecimalTypeReference(null, true, 1, 3));
        Assert.True(baseline.IsEquivalentTo(match), "Is the same.");
        Assert.False(baseline.IsEquivalentTo(differentScale), "Different Scale.");
        Assert.False(baseline.IsEquivalentTo(nullScale), "Null Scale.");
        Assert.False(baseline.IsEquivalentTo(differentPrecision), "Different Precision.");
        Assert.False(baseline.IsEquivalentTo(nullPrecision), "Null Precision.");
        Assert.False(baseline.IsEquivalentTo(differentNullibility), "Different nullibility.");
    }

    [Fact]
    public void EqualityStringReferenceTypeTest()
    {
        var simpleBaseline = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false);
        var simpleDifferentNullibility = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true);
        var simpleDifferentPrimitiveType = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false);
        var simpleMatch = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false);

        Assert.Throws<ArgumentNullException>(() => new EdmStringTypeReference(null, false));
        Assert.True(simpleBaseline.IsEquivalentTo(simpleMatch), "Is the same.");
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentNullibility), "Different nullibility.");
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentPrimitiveType), "Different primitive type.");

        var baseline = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true, false, 4, true);
        var match = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true, false, 4, true);
        var differentUnicode = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true, false, 4, false);
        var nullUnicode = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true, false, 4, null);
        var differentMaxLength = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true, false, 5, true);
        var nullMaxLength = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true, false, null, true);
        var differentIsUnbounded = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true, true, null, true);

        Assert.Throws<ArgumentNullException>(() => new EdmStringTypeReference(null, true, false, 4, true));
        Assert.Throws<InvalidOperationException>(() => new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true, true, 4, true));
        Assert.True(baseline.IsEquivalentTo(match), "Is the same.");
        Assert.False(baseline.IsEquivalentTo(differentUnicode), "Different Unicode");
        Assert.False(baseline.IsEquivalentTo(nullUnicode), "Null Unicode");
        Assert.False(baseline.IsEquivalentTo(differentMaxLength), "Different MaxLength");
        Assert.False(baseline.IsEquivalentTo(nullMaxLength), "Null MaxLength");
        Assert.False(baseline.IsEquivalentTo(differentIsUnbounded), "Different IsUnbounded");
    }

    [Fact]
    public void EqualityTemporalReferenceTypeTest()
    {
        var simpleBaseline = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), false);
        var simpleDifferentNullibility = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), true);
        var simpleDifferentPrimitiveType = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration), false);
        var simpleMatch = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), false);

        Assert.Throws<ArgumentNullException>(() => new EdmTemporalTypeReference(null, false));
        Assert.True(simpleBaseline.IsEquivalentTo(simpleMatch), "Is the same.");
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentNullibility), "Different nullibility.");
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentPrimitiveType), "Different primitive type.");

        var baseline = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), true, 3);
        var match = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), true, 3);
        var differentPrecision = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), true, 4);
        var nullPrecision = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), true, null);

        Assert.Throws<ArgumentNullException>(() => new EdmTemporalTypeReference(null, true, 3));
        Assert.True(baseline.IsEquivalentTo(match), "Is the same.");
        Assert.False(baseline.IsEquivalentTo(differentPrecision), "Different Precision.");
        Assert.False(baseline.IsEquivalentTo(nullPrecision), "Null Precision");
    }

    [Fact]
    public void EqualitySpatialReferenceTypeTest()
    {
        var simpleBaseline = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography), false);
        var simpleDifferentNullibility = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography), true);
        var simpleDifferentPrimitiveType = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyLineString), false);
        var simpleMatch = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography), false);

        Assert.Throws<ArgumentNullException>(() => new EdmSpatialTypeReference(null, false));
        Assert.True(simpleBaseline.IsEquivalentTo(simpleMatch), "Is the same.");
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentNullibility), "Different nullibility.");
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentPrimitiveType), "Different primitive type.");

        var baseline = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography), true, 3);
        var match = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography), true, 3);
        var differentId = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography), true, 4);
        var nullId = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography), true, null);

        Assert.Throws<ArgumentNullException>(() => new EdmTemporalTypeReference(null, true, 3));
        Assert.True(baseline.IsEquivalentTo(match), "Is the same.");
        Assert.False(baseline.IsEquivalentTo(differentId), "Different Id.");
        Assert.False(baseline.IsEquivalentTo(nullId), "Null Id");
    }

    [Fact]
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
        Assert.True(simpleBaseline.IsEquivalentTo(simpleMatch), "Is the same.");
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentNullibility), "Different Nullibility.");
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentNameType), "Different name simple complex type.");
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentNamespaceType), "Different namespace for simple complex type.");

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

        Assert.Throws<ArgumentNullException>(() => new EdmComplexTypeReference(null, true));
        Assert.True(baseline.IsEquivalentTo(match), "Is the same.");
        Assert.False(baseline.IsEquivalentTo(differentBaseType), "Different base type.");
        Assert.False(baseline.IsEquivalentTo(nullBaseType), "Null base type.");
        Assert.False(baseline.IsEquivalentTo(differentAbstractType), "Different abstract type.");
    }

    [Fact]
    public void EqualityComplexTypeTest()
    {
        var simpleBaselineComplexType = new EdmComplexType("NS", "Baseline");
        var simpleMatchComplexType = new EdmComplexType("NS", "Baseline");
        var simpleDifferentNameComplexType = new EdmComplexType("NS", "Different");
        var simpleDifferentNamespaceComplexType = new EdmComplexType("Foo", "Base");

        Assert.Throws<ArgumentNullException>(() => new EdmComplexType(null, "Baseline"));
        Assert.Throws<ArgumentNullException>(() => new EdmComplexType("NS", null));
        Assert.True(simpleBaselineComplexType.IsEquivalentTo(simpleBaselineComplexType), "Is the same.");
        Assert.False(simpleBaselineComplexType.IsEquivalentTo(simpleMatchComplexType), "Is the same, but different obj refs.");
        Assert.False(simpleBaselineComplexType.IsEquivalentTo(simpleDifferentNameComplexType), "Different name simple complex type.");
        Assert.False(simpleBaselineComplexType.IsEquivalentTo(simpleDifferentNamespaceComplexType), "Different namespace for simple complex type.");

        var baselineBaseComplexType = new EdmComplexType("NS", "Base");
        var fooBaseComplexType = new EdmComplexType("NS", "Foo");

        var baselineComplexType = new EdmComplexType("NS", "Baseline", baselineBaseComplexType, false);
        var matchComplexType = new EdmComplexType("NS", "Baseline", baselineBaseComplexType, false);
        var differentBaseComplexType = new EdmComplexType("NS", "Baseline", fooBaseComplexType, false);
        var nullBaseTypeComplexType = new EdmComplexType("NS", "Baseline", null, false);
        var differentAbstractComplexType = new EdmComplexType("NS", "Baseline", baselineBaseComplexType, true);

        Assert.True(baselineComplexType.IsEquivalentTo(baselineComplexType), "Is the same.");
        Assert.False(baselineComplexType.IsEquivalentTo(matchComplexType), "Is the same, but different obj refs.");
        Assert.False(baselineComplexType.IsEquivalentTo(differentBaseComplexType), "Different base type.");
        Assert.False(baselineComplexType.IsEquivalentTo(nullBaseTypeComplexType), "Null base type.");
        Assert.False(baselineComplexType.IsEquivalentTo(differentAbstractComplexType), "Different abstract type.");
    }

    [Fact]
    public void EqualityEnumTypeTest()
    {
        var baseline = new EdmEnumType("NS", "Baseline", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), true);
        var match = new EdmEnumType("NS", "Baseline", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), true);
        var differentNamespace = new EdmEnumType("foo", "Baseline", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), true);
        var differentName = new EdmEnumType("NS", "foo", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), true);
        var differentPrimitiveType = new EdmEnumType("NS", "Baseline", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int16), true);
        var differentFlag = new EdmEnumType("NS", "Baseline", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), false);

        Assert.Throws<ArgumentNullException>(() => new EdmEnumType(null, "Baseline", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), true));
        Assert.Throws<ArgumentNullException>(() => new EdmEnumType("NS", null, EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), true));
        Assert.Throws<ArgumentNullException>(() => new EdmEnumType("NS", "Baseline", null, true));

        Assert.True(baseline.IsEquivalentTo(baseline), "Is the same.");
        Assert.True(baseline.IsEquivalentTo(match), "Same but different obj refs");
        Assert.False(baseline.IsEquivalentTo(differentNamespace), "Different namespace.");
        Assert.False(baseline.IsEquivalentTo(differentName), "Different name.");
        Assert.False(baseline.IsEquivalentTo(differentPrimitiveType), "Different primitive type.");
        Assert.False(baseline.IsEquivalentTo(differentFlag), "Different flag.");
    }

    [Fact]
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

        Assert.True(baseline.IsEquivalentTo(baseline), "Is the same.");
        Assert.False(baseline.IsEquivalentTo(match), "Is the same, different obj refs.");
        Assert.False(baseline.IsEquivalentTo(differentNamespace), "Different namespace.");
        Assert.False(baseline.IsEquivalentTo(differentName), "Different name.");
        Assert.False(baseline.IsEquivalentTo(differentBaseType), "Different base type.");
        Assert.False(baseline.IsEquivalentTo(nullBaseType), "Null base type.");
        Assert.False(baseline.IsEquivalentTo(differentAbstract), "Different abstract.");
        Assert.False(baseline.IsEquivalentTo(differentIsOpen), "Different is open.");
        Assert.False(baseline.IsEquivalentTo(differentProperties), "Different properties.");
        Assert.False(baseline.IsEquivalentTo(differentKey), "Different key.");
    }

    [Fact]
    public void EqualityCollectionTypeTest()
    {
        var baselineCollectionTypeElement = new EdmEntityType("NS", "Baseline", new EdmEntityType("NS", "foo", null, true, false), true, false);
        var differentCollectionTypeElement = new EdmEntityType("NS", "Baseline", new EdmEntityType("NS", "bar", null, false, false), true, false);

        var baseline = new EdmCollectionType(new EdmEntityTypeReference(baselineCollectionTypeElement, true));
        var match = new EdmCollectionType(new EdmEntityTypeReference(baselineCollectionTypeElement, true));
        var differentCollectionNullibility = new EdmCollectionType(new EdmEntityTypeReference(baselineCollectionTypeElement, false));
        var differentCollectionType = new EdmCollectionType(new EdmEntityTypeReference(differentCollectionTypeElement, true));

        Assert.True(baseline.IsEquivalentTo(match), "Is the same.");
        Assert.False(baseline.IsEquivalentTo(differentCollectionNullibility), "Different nullibility.");
        Assert.False(baseline.IsEquivalentTo(differentCollectionType), "Different collection type element (different obj refs).");
        differentCollectionType = new EdmCollectionType(new EdmEntityTypeReference(baselineCollectionTypeElement, true));
        Assert.True(baseline.IsEquivalentTo(differentCollectionType), "Different collection types equal on element.");
    }

    [Fact]
    public void EqualityEntityReferenceTypeTests()
    {
        var baselineEntityType = new EdmEntityType("NS", "Baseline", new EdmEntityType("NS", "foo", null, true, false), true, false);
        var differentBaselineEntityType = new EdmEntityType("NS", "Baseline", new EdmEntityType("NS", "bar", null, false, false), true, false);

        var baseline = new EdmEntityReferenceType(baselineEntityType);
        var match = new EdmEntityReferenceType(baselineEntityType);
        var differentEntityType = new EdmEntityReferenceType(differentBaselineEntityType);

        Assert.True(baseline.IsEquivalentTo(match), "Is the same.");
        Assert.False(baseline.IsEquivalentTo(differentEntityType), "Different entity type.");
    }

    [Fact]
    public void NullComparisonEquivalence()
    {
        EdmEntityType entity = new EdmEntityType("Foo", "Bar", null, false, false);

        Assert.False(entity.IsEquivalentTo(null), "Type RHS");
        Assert.False(((IEdmEntityType)null).IsEquivalentTo(entity), "Type LHS");
        Assert.True(((IEdmEntityType)null).IsEquivalentTo(null), "Type Both");

        Assert.False(new EdmEntityTypeReference(entity, false).IsEquivalentTo(null), "TypeReference RHS");
        Assert.False(((IEdmEntityTypeReference)null).IsEquivalentTo(new EdmEntityTypeReference(entity, false)), "TypeReference LHS");
        Assert.True(((IEdmEntityTypeReference)null).IsEquivalentTo(null), "TypeReference Both");
    }

    [Fact]
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
        Assert.True(parsed);
    }

    [Fact]
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);

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

        Assert.True(myBinary.AsPrimitive() is IEdmBinaryTypeReference, "AsPrimitive creates correct type");
        Assert.True(myBoolean.AsPrimitive() is IEdmPrimitiveTypeReference, "AsPrimitive creates correct type");
        Assert.True(myDateTime.AsPrimitive() is IEdmTemporalTypeReference, "AsPrimitive creates correct type");
        Assert.True(myTime.AsPrimitive() is IEdmTemporalTypeReference, "AsPrimitive creates correct type");
        Assert.True(myDateTimeOffset.AsPrimitive() is IEdmTemporalTypeReference, "AsPrimitive creates correct type");
        Assert.True(myDecimal.AsPrimitive() is IEdmDecimalTypeReference, "AsPrimitive creates correct type");
        Assert.True(myDouble.AsPrimitive() is IEdmPrimitiveTypeReference, "AsPrimitive creates correct type");
        Assert.True(myGuid.AsPrimitive() is IEdmPrimitiveTypeReference, "AsPrimitive creates correct type");
        Assert.True(mySByte.AsPrimitive() is IEdmPrimitiveTypeReference, "AsPrimitive creates correct type");
        Assert.True(mySingle.AsPrimitive() is IEdmPrimitiveTypeReference, "AsPrimitive creates correct type");
        Assert.True(myInt16.AsPrimitive() is IEdmPrimitiveTypeReference, "AsPrimitive creates correct type");
        Assert.True(myInt32.AsPrimitive() is IEdmPrimitiveTypeReference, "AsPrimitive creates correct type");
        Assert.True(myInt64.AsPrimitive() is IEdmPrimitiveTypeReference, "AsPrimitive creates correct type");
        Assert.True(myByte.AsPrimitive() is IEdmPrimitiveTypeReference, "AsPrimitive creates correct type");
        Assert.True(myStream.AsPrimitive() is IEdmPrimitiveTypeReference, "AsPrimitive creates correct type");
        Assert.True(myString.AsPrimitive() is IEdmStringTypeReference, "AsPrimitive creates correct type");
        Assert.True(myGeography.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
        Assert.True(myPoint.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
        Assert.True(myLineString.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
        Assert.True(myPolygon.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
        Assert.True(myGeographyCollection.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
        Assert.True(myMultiPolygon.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
        Assert.True(myMultiLineString.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
        Assert.True(myMultiPoint.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
        Assert.True(myGeometry.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
        Assert.True(myGeometricPoint.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
        Assert.True(myGeometricLineString.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
        Assert.True(myGeometricPolygon.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
        Assert.True(myGeometryCollection.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
        Assert.True(myGeometricMultiPolygon.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
        Assert.True(myGeometricMultiPoint.AsPrimitive() is IEdmSpatialTypeReference, "AsPrimitive creates correct type");
    }

    [Fact]
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(annotatingModelCsdl)), XmlReader.Create(new StringReader(baseModelCsdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
       Assert.Empty(errors);

        IEdmEntityType person = (IEdmEntityType)model.FindType("foo.Person");
        IEdmVocabularyAnnotation valueAnnotation = person.VocabularyAnnotations(model).First();
        var property = ((IEdmRecordExpression)valueAnnotation.Value).Properties.First();
        IEdmOperation badOperation = ((IEdmApplyExpression)property.Value).AppliedFunction;
        Assert.Equal("foo", badOperation.Namespace);
        Assert.Equal("BorkBorkBork", badOperation.Name);
        Assert.Equal(EdmSchemaElementKind.Function, badOperation.SchemaElementKind);
        Assert.True(badOperation.ReturnType.Definition.IsBad(), "Bad function has bad return return type.");
        Assert.Null(badOperation.FindParameter("Foo"));
        Assert.Empty(badOperation.Parameters);
    }
}
