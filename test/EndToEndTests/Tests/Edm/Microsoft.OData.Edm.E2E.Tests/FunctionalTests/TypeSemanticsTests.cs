//---------------------------------------------------------------------
// <copyright file="TypeSemanticsUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

using System.Xml;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

public class TypeSemanticsTests : EdmLibTestCaseBase
{
    [Fact]
    public void EntityAndComplexType_IsMethods_ReturnExpectedResults()
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
    public void PrimitiveType_IsMethods_ReturnExpectedResults()
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
        Assert.True(sByteRef.IsSByte());
        Assert.True(singleRef.IsSingle());
        Assert.True(stringRef.IsString());
        Assert.True(streamRef.IsStream());
        Assert.True(timeRef.IsDuration());
        Assert.True(dateRef.IsDate());
        Assert.True(timeOfDayRef.IsTimeOfDay());

        Assert.True(binaryRef.AsPrimitive().IsPrimitive());
        Assert.True(binaryRef.AsPrimitive().IsBinary());
        Assert.True(booleanRef.AsPrimitive().IsBoolean());
        Assert.True(byteRef.AsPrimitive().IsByte());
        Assert.True(dateTimeOffsetRef.AsPrimitive().IsDateTimeOffset());
        Assert.True(decimalRef.AsPrimitive().IsDecimal());
        Assert.True(doubleRef.AsPrimitive().IsDouble());
        Assert.True(guidRef.AsPrimitive().IsGuid());
        Assert.True(int16Ref.AsPrimitive().IsInt16());
        Assert.True(int32Ref.AsPrimitive().IsInt32());
        Assert.True(int64Ref.AsPrimitive().IsInt64());
        Assert.True(sByteRef.AsPrimitive().IsSByte());
        Assert.True(singleRef.AsPrimitive().IsSingle());
        Assert.True(stringRef.AsPrimitive().IsString());
        Assert.True(streamRef.AsPrimitive().IsStream());
        Assert.True(timeRef.AsPrimitive().IsDuration());
        Assert.True(dateRef.AsPrimitive().IsDate());
        Assert.True(timeOfDayRef.AsPrimitive().IsTimeOfDay());

        Assert.True(dateTimeOffsetRef.IsTemporal());
        Assert.True(timeRef.IsTemporal());
        Assert.True(timeOfDayRef.IsTemporal());
        Assert.False(int32Ref.IsTemporal());

        Assert.True(doubleRef.IsFloating());
        Assert.True(singleRef.IsFloating());
        Assert.False(int32Ref.IsFloating());

        Assert.True(sByteRef.IsSignedIntegral());
        Assert.True(int16Ref.IsSignedIntegral());
        Assert.True(int32Ref.IsSignedIntegral());
        Assert.True(int64Ref.IsSignedIntegral());
        Assert.False(stringRef.IsSignedIntegral());

        IEdmEntityType entityDef = new EdmEntityType("MyNamespace", "MyEntity");
        IEdmEntityTypeReference entityRef = new EdmEntityTypeReference(entityDef, false);
        Assert.Equal(EdmPrimitiveTypeKind.None, entityRef.PrimitiveKind());
        Assert.Equal(EdmSchemaElementKind.TypeDefinition, int16Ref.PrimitiveDefinition().SchemaElementKind);
    }

    [Fact]
    public void NonPrimitiveType_AsMethods_ReturnBadTypeReferencesForInvalidConversions()
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
        IEdmEntityReferenceTypeReference badEntityRefRef = entityRef.AsEntityReference();

        Assert.True(badCollectionRef.IsBad());
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badCollectionRef.Errors().First().ErrorCode);
        Assert.Equal("[Collection([UnknownType Nullable=True]) Nullable=True]", badCollectionRef.ToString());

        Assert.True(badComplexRef.IsBad());
        Assert.True(badComplexRef.Definition.IsBad());
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badComplexRef.Definition.Errors().First().ErrorCode);
        Assert.Equal("TypeSemanticsCouldNotConvertTypeReference:[MyNamespace.MyEntity Nullable=False]", badComplexRef.ToString());

        Assert.True(badEntityRef.IsBad());
        Assert.True(badEntityRef.Definition.IsBad());
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badEntityRef.Definition.Errors().First().ErrorCode);
        Assert.Equal("TypeSemanticsCouldNotConvertTypeReference:[Collection(Edm.Int32) Nullable=False]", badEntityRef.ToString());

        Assert.True(badEntityRefRef.IsBad());
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badEntityRefRef.Errors().First().ErrorCode);
        Assert.Equal("[EntityReference(.) Nullable=False]", badEntityRefRef.ToString());

        Assert.False(entityRef.AsEntity().IsBad());
        Assert.False(complexRef.AsComplex().IsBad());
        Assert.False(collectionRef.AsCollection().IsBad());

        Assert.True(entityRef.AsStructured().IsEntity());
        Assert.False(entityRef.AsStructured().IsBad());
        Assert.True(complexRef.AsStructured().IsComplex());
        Assert.False(complexRef.AsStructured().IsBad());
        Assert.False(collectionRef.AsStructured().IsCollection());
        Assert.True(collectionRef.AsStructured().IsBad());
        Assert.True(collectionRef.AsStructured().Definition.IsBad());
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, collectionRef.AsStructured().Definition.Errors().First().ErrorCode);
    }

    [Fact]
    public void PrimitiveType_AsMethods_ReturnBadTypeReferencesForInvalidConversions()
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

        Assert.True(badTemporal.IsBad());
        Assert.True(badTemporal.Definition.IsBad());
        Assert.Equal(EdmErrorCode.InterfaceCriticalKindValueMismatch, badTemporal.Errors().First().ErrorCode);
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badTemporal.Definition.Errors().First().ErrorCode);
        Assert.Equal("TypeSemanticsCouldNotConvertTypeReference:[MyNamespace.BadEntity Nullable=False]", badTemporal.ToString());

        Assert.True(badDecimal.IsBad());
        Assert.True(badDecimal.Definition.IsBad());
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badDecimal.Definition.Errors().First().ErrorCode);
        Assert.Equal("TypeSemanticsCouldNotConvertTypeReference:[MyNamespace.BadEntity Nullable=False]", badDecimal.ToString());

        Assert.True(badString.IsBad());
        Assert.True(badString.Definition.IsBad());
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badString.Definition.Errors().First().ErrorCode);
        Assert.Equal("TypeSemanticsCouldNotConvertTypeReference:[MyNamespace.BadEntity Nullable=False Unicode=False]", badString.ToString());

        Assert.True(badStream.IsBad());
        Assert.True(badStream.Definition.IsBad());
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badStream.Definition.Errors().First().ErrorCode);
        Assert.Equal("TypeSemanticsCouldNotConvertTypeReference:[MyNamespace.BadEntity Nullable=False]", badStream.ToString());

        Assert.True(badBinary.IsBad());
        Assert.True(badBinary.Definition.IsBad());
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badBinary.Definition.Errors().First().ErrorCode);
        Assert.Equal("TypeSemanticsCouldNotConvertTypeReference:[MyNamespace.BadEntity Nullable=False]", badBinary.ToString());

        Assert.True(badSpatial.IsBad());
        Assert.True(badSpatial.Definition.IsBad());
        Assert.Equal(EdmErrorCode.InterfaceCriticalKindValueMismatch, badSpatial.Errors().First().ErrorCode);
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badSpatial.Definition.Errors().First().ErrorCode);
        Assert.Equal("TypeSemanticsCouldNotConvertTypeReference:[MyNamespace.BadEntity Nullable=False]", badSpatial.ToString());

        Assert.True(badPrimitive.IsBad());
        Assert.True(badPrimitive.Definition.IsBad());
        Assert.Equal(EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, badPrimitive.Definition.Errors().First().ErrorCode);
        Assert.Equal("TypeSemanticsCouldNotConvertTypeReference:[MyNamespace.BadEntity Nullable=False]", badPrimitive.ToString());
    }

    [Fact]
    public void BadTypeReferences_ExerciseAllConversions_ReturnExpectedErrors()
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

        Assert.True(badCollectionRef.IsCollection());

        Assert.True(badEntityRef.IsEntity());
        Assert.Empty(badEntityRef.Key());
        Assert.Equal("MyNamespace.BadComplex", badEntityRef.FullName());
        Assert.Equal(EdmSchemaElementKind.TypeDefinition, badEntityRef.EntityDefinition().SchemaElementKind);
        Assert.Null(badEntityRef.EntityDefinition().BaseType);
        Assert.Equal(Enumerable.Empty<IEdmProperty>(), badEntityRef.EntityDefinition().DeclaredProperties);
        Assert.False(badEntityRef.EntityDefinition().IsAbstract);
        Assert.False(badEntityRef.EntityDefinition().IsOpen);
        model.SetAnnotationValue(badEntityRef.Definition, "foo", "bar", new EdmStringConstant(new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false), "baz"));
        Assert.Single(model.DirectValueAnnotations(badEntityRef.Definition));
        Assert.NotNull(model.GetAnnotationValue(badEntityRef.Definition, "foo", "bar"));

        Assert.True(badComplexRef.IsComplex());
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
    public void TypeReference_Equality_ReturnsExpectedResults()
    {
        Assert.True(EdmCoreModel.Instance.GetBinary(false).IsEquivalentTo(EdmCoreModel.Instance.GetBinary(false)));
        Assert.True(EdmCoreModel.Instance.GetBoolean(false).IsEquivalentTo(EdmCoreModel.Instance.GetBoolean(false)));
        Assert.True(EdmCoreModel.Instance.GetDecimal(false).IsEquivalentTo(EdmCoreModel.Instance.GetDecimal(false)));
        Assert.True(EdmCoreModel.Instance.GetString(false).IsEquivalentTo(EdmCoreModel.Instance.GetString(false)));
        Assert.True(EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false).IsEquivalentTo(EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false)));
        Assert.True(EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false)).IsEquivalentTo(EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false))));

        Assert.False(EdmCoreModel.Instance.GetBinary(false).IsEquivalentTo(EdmCoreModel.Instance.GetBinary(true)));
        Assert.False(EdmCoreModel.Instance.GetBoolean(false).IsEquivalentTo(EdmCoreModel.Instance.GetBoolean(true)));
        Assert.False(EdmCoreModel.Instance.GetDecimal(false).IsEquivalentTo(EdmCoreModel.Instance.GetDecimal(true)));
        Assert.False(EdmCoreModel.Instance.GetString(false).IsEquivalentTo(EdmCoreModel.Instance.GetString(true)));
        Assert.False(EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false).Equals(EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, true)));

        Assert.False(EdmCoreModel.Instance.GetBinary(false).IsEquivalentTo(EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false)));
        Assert.False(EdmCoreModel.Instance.GetBoolean(false).IsEquivalentTo(EdmCoreModel.Instance.GetBinary(false)));
        Assert.False(EdmCoreModel.Instance.GetDecimal(false).IsEquivalentTo(EdmCoreModel.Instance.GetBoolean(false)));
        Assert.False(EdmCoreModel.Instance.GetString(false).IsEquivalentTo(EdmCoreModel.Instance.GetDecimal(false)));

        Assert.True(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal).IsEquivalentTo(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal)));
        Assert.False(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal).IsEquivalentTo(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary)));

        Assert.True(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration).IsEquivalentTo(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration)));
        Assert.False(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration).IsEquivalentTo(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset)));

        IEdmEntityReferenceTypeReference entityRef1 = new EdmEntityReferenceTypeReference(new EdmEntityReferenceType(new EdmEntityType("bar", "foo")), false);
        IEdmEntityReferenceTypeReference entityRef2 = new EdmEntityReferenceTypeReference(new EdmEntityReferenceType(new EdmEntityType("bar", "foo")), false);
        Assert.False(entityRef1.IsEquivalentTo(entityRef2));

        entityRef1 = new EdmEntityReferenceTypeReference(new EdmEntityReferenceType(new EdmEntityType("bar", "foo")), false);
        entityRef2 = new EdmEntityReferenceTypeReference(new EdmEntityReferenceType(entityRef1.EntityReferenceDefinition().EntityType), false);
        Assert.True(entityRef1.IsEquivalentTo(entityRef2));

        Assert.False(new EdmEntityType("", "").IsEquivalentTo(new EdmComplexType("", "")));
        Assert.False(new EdmEntityType("NS", "E1").IsEquivalentTo(new EdmEntityType("NS", "E2")));

        IEdmTypeReference type = EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Int32, false);
        Assert.True(type.IsEquivalentTo(type));
    }

    [Fact]
    public void BinaryTypeReference_Equality_ReturnsExpectedResults()
    {
        var simpleBaseline = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false);
        var simpleDifferentNullibility = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), true);
        var simpleDifferentPrimitiveType = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Stream), false);
        var simpleDifferentReferenceType = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), true);
        var simpleMatch = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false);

        Assert.Throws<ArgumentNullException>(() => new EdmBinaryTypeReference(null, false));

        Assert.True(simpleBaseline.IsEquivalentTo(simpleMatch));
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentNullibility));
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentPrimitiveType));

        Assert.True(simpleDifferentPrimitiveType.IsBad());
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

        Assert.True(baseline.IsEquivalentTo(match));
        Assert.False(baseline.IsEquivalentTo(differentMaxLength));
        Assert.False(baseline.IsEquivalentTo(nullMaxLength));
        Assert.False(baseline.IsEquivalentTo(differentIsUnbounded));
        Assert.False(baseline.IsEquivalentTo(differentNullibility));
        Assert.False(baseline.IsEquivalentTo(differentPrimitiveType));

        Assert.True(differentTypeReference.IsBad());
        Assert.False(baseline.IsEquivalentTo(differentTypeReference));
    }

    [Fact]
    public void DecimalTypeReference_Equality_ReturnsExpectedResults()
    {
        var simpleBaseline = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), false);
        var simpleDifferentNullibility = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true);
        var simpleDifferentPrimitiveType = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false);
        var simpleMatch = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), false);

        Assert.Throws<ArgumentNullException>(() => new EdmDecimalTypeReference(null, false));
        Assert.True(simpleBaseline.IsEquivalentTo(simpleMatch));
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentNullibility));
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentPrimitiveType));

        var baseline = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true, 1, 3);
        var match = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true, 1, 3);
        var differentScale = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true, 1, 4);
        var nullScale = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true, 1, null);
        var differentPrecision = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true, 2, 3);
        var nullPrecision = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true, null, 3);
        var differentNullibility = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), false, 1, 3);

        Assert.Throws<ArgumentNullException>(() => new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.None), true, 1, 3));
        Assert.Throws<ArgumentNullException>(() => new EdmDecimalTypeReference(null, true, 1, 3));
        Assert.True(baseline.IsEquivalentTo(match));
        Assert.False(baseline.IsEquivalentTo(differentScale));
        Assert.False(baseline.IsEquivalentTo(nullScale));
        Assert.False(baseline.IsEquivalentTo(differentPrecision));
        Assert.False(baseline.IsEquivalentTo(nullPrecision));
        Assert.False(baseline.IsEquivalentTo(differentNullibility));
    }

    [Fact]
    public void StringTypeReference_Equality_ReturnsExpectedResults()
    {
        var simpleBaseline = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false);
        var simpleDifferentNullibility = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true);
        var simpleDifferentPrimitiveType = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), false);
        var simpleMatch = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false);

        Assert.Throws<ArgumentNullException>(() => new EdmStringTypeReference(null, false));
        Assert.True(simpleBaseline.IsEquivalentTo(simpleMatch));
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentNullibility));
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentPrimitiveType));

        var baseline = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true, false, 4, true);
        var match = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true, false, 4, true);
        var differentUnicode = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true, false, 4, false);
        var nullUnicode = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true, false, 4, null);
        var differentMaxLength = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true, false, 5, true);
        var nullMaxLength = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true, false, null, true);
        var differentIsUnbounded = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true, true, null, true);

        Assert.Throws<ArgumentNullException>(() => new EdmStringTypeReference(null, true, false, 4, true));
        Assert.Throws<InvalidOperationException>(() => new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true, true, 4, true));
        Assert.True(baseline.IsEquivalentTo(match));
        Assert.False(baseline.IsEquivalentTo(differentUnicode));
        Assert.False(baseline.IsEquivalentTo(nullUnicode));
        Assert.False(baseline.IsEquivalentTo(differentMaxLength));
        Assert.False(baseline.IsEquivalentTo(nullMaxLength));
        Assert.False(baseline.IsEquivalentTo(differentIsUnbounded));
    }

    [Fact]
    public void TemporalTypeReference_Equality_ReturnsExpectedResults()
    {
        var simpleBaseline = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), false);
        var simpleDifferentNullibility = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), true);
        var simpleDifferentPrimitiveType = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration), false);
        var simpleMatch = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), false);

        Assert.Throws<ArgumentNullException>(() => new EdmTemporalTypeReference(null, false));
        Assert.True(simpleBaseline.IsEquivalentTo(simpleMatch));
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentNullibility));
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentPrimitiveType));

        var baseline = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), true, 3);
        var match = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), true, 3);
        var differentPrecision = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), true, 4);
        var nullPrecision = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), true, null);

        Assert.Throws<ArgumentNullException>(() => new EdmTemporalTypeReference(null, true, 3));
        Assert.True(baseline.IsEquivalentTo(match));
        Assert.False(baseline.IsEquivalentTo(differentPrecision));
        Assert.False(baseline.IsEquivalentTo(nullPrecision));
    }

    [Fact]
    public void SpatialTypeReference_Equality_ReturnsExpectedResults()
    {
        var simpleBaseline = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography), false);
        var simpleDifferentNullibility = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography), true);
        var simpleDifferentPrimitiveType = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyLineString), false);
        var simpleMatch = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography), false);

        Assert.Throws<ArgumentNullException>(() => new EdmSpatialTypeReference(null, false));
        Assert.True(simpleBaseline.IsEquivalentTo(simpleMatch));
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentNullibility));
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentPrimitiveType));

        var baseline = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography), true, 3);
        var match = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography), true, 3);
        var differentId = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography), true, 4);
        var nullId = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography), true, null);

        Assert.Throws<ArgumentNullException>(() => new EdmTemporalTypeReference(null, true, 3));
        Assert.True(baseline.IsEquivalentTo(match));
        Assert.False(baseline.IsEquivalentTo(differentId));
        Assert.False(baseline.IsEquivalentTo(nullId));
    }

    [Fact]
    public void ComplexTypeReference_Equality_ReturnsExpectedResults()
    {
        var simpleBaselineComplexType = new EdmComplexType("NS", "Baseline");
        var simpleDifferentNameComplexType = new EdmComplexType("NS", "Different");
        var simpleDifferentNamespaceComplexType = new EdmComplexType("Foo", "Base");

        var simpleBaseline = new EdmComplexTypeReference(simpleBaselineComplexType, true);
        var simpleMatch = new EdmComplexTypeReference(simpleBaselineComplexType, true);
        var simpleDifferentNullibility = new EdmComplexTypeReference(simpleBaselineComplexType, false);
        var simpleDifferentNameType = new EdmComplexTypeReference(simpleDifferentNameComplexType, true);
        var simpleDifferentNamespaceType = new EdmComplexTypeReference(simpleDifferentNamespaceComplexType, true);
        Assert.True(simpleBaseline.IsEquivalentTo(simpleMatch));
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentNullibility));
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentNameType));
        Assert.False(simpleBaseline.IsEquivalentTo(simpleDifferentNamespaceType));

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
        Assert.True(baseline.IsEquivalentTo(match));
        Assert.False(baseline.IsEquivalentTo(differentBaseType));
        Assert.False(baseline.IsEquivalentTo(nullBaseType));
        Assert.False(baseline.IsEquivalentTo(differentAbstractType));
    }

    [Fact]
    public void ComplexType_Equality_ReturnsExpectedResults()
    {
        var simpleBaselineComplexType = new EdmComplexType("NS", "Baseline");
        var simpleMatchComplexType = new EdmComplexType("NS", "Baseline");
        var simpleDifferentNameComplexType = new EdmComplexType("NS", "Different");
        var simpleDifferentNamespaceComplexType = new EdmComplexType("Foo", "Base");

        Assert.Throws<ArgumentNullException>(() => new EdmComplexType(null, "Baseline"));
        Assert.Throws<ArgumentNullException>(() => new EdmComplexType("NS", null));
        Assert.True(simpleBaselineComplexType.IsEquivalentTo(simpleBaselineComplexType));
        Assert.False(simpleBaselineComplexType.IsEquivalentTo(simpleMatchComplexType));
        Assert.False(simpleBaselineComplexType.IsEquivalentTo(simpleDifferentNameComplexType));
        Assert.False(simpleBaselineComplexType.IsEquivalentTo(simpleDifferentNamespaceComplexType));

        var baselineBaseComplexType = new EdmComplexType("NS", "Base");
        var fooBaseComplexType = new EdmComplexType("NS", "Foo");

        var baselineComplexType = new EdmComplexType("NS", "Baseline", baselineBaseComplexType, false);
        var matchComplexType = new EdmComplexType("NS", "Baseline", baselineBaseComplexType, false);
        var differentBaseComplexType = new EdmComplexType("NS", "Baseline", fooBaseComplexType, false);
        var nullBaseTypeComplexType = new EdmComplexType("NS", "Baseline", null, false);
        var differentAbstractComplexType = new EdmComplexType("NS", "Baseline", baselineBaseComplexType, true);

        Assert.True(baselineComplexType.IsEquivalentTo(baselineComplexType));
        Assert.False(baselineComplexType.IsEquivalentTo(matchComplexType));
        Assert.False(baselineComplexType.IsEquivalentTo(differentBaseComplexType));
        Assert.False(baselineComplexType.IsEquivalentTo(nullBaseTypeComplexType));
        Assert.False(baselineComplexType.IsEquivalentTo(differentAbstractComplexType));
    }

    [Fact]
    public void EnumType_Equality_ReturnsExpectedResults()
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

        Assert.True(baseline.IsEquivalentTo(baseline));
        Assert.True(baseline.IsEquivalentTo(match));
        Assert.False(baseline.IsEquivalentTo(differentNamespace));
        Assert.False(baseline.IsEquivalentTo(differentName));
        Assert.False(baseline.IsEquivalentTo(differentPrimitiveType));
        Assert.False(baseline.IsEquivalentTo(differentFlag));
    }

    [Fact]
    public void EntityType_Equality_ReturnsExpectedResults()
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

        Assert.True(baseline.IsEquivalentTo(baseline));
        Assert.False(baseline.IsEquivalentTo(match));
        Assert.False(baseline.IsEquivalentTo(differentNamespace));
        Assert.False(baseline.IsEquivalentTo(differentName));
        Assert.False(baseline.IsEquivalentTo(differentBaseType));
        Assert.False(baseline.IsEquivalentTo(nullBaseType));
        Assert.False(baseline.IsEquivalentTo(differentAbstract));
        Assert.False(baseline.IsEquivalentTo(differentIsOpen));
        Assert.False(baseline.IsEquivalentTo(differentProperties));
        Assert.False(baseline.IsEquivalentTo(differentKey));
    }

    [Fact]
    public void CollectionType_Equality_ReturnsExpectedResults()
    {
        var baselineCollectionTypeElement = new EdmEntityType("NS", "Baseline", new EdmEntityType("NS", "foo", null, true, false), true, false);
        var differentCollectionTypeElement = new EdmEntityType("NS", "Baseline", new EdmEntityType("NS", "bar", null, false, false), true, false);

        var baseline = new EdmCollectionType(new EdmEntityTypeReference(baselineCollectionTypeElement, true));
        var match = new EdmCollectionType(new EdmEntityTypeReference(baselineCollectionTypeElement, true));
        var differentCollectionNullibility = new EdmCollectionType(new EdmEntityTypeReference(baselineCollectionTypeElement, false));
        var differentCollectionType = new EdmCollectionType(new EdmEntityTypeReference(differentCollectionTypeElement, true));

        Assert.True(baseline.IsEquivalentTo(match));
        Assert.False(baseline.IsEquivalentTo(differentCollectionNullibility));
        Assert.False(baseline.IsEquivalentTo(differentCollectionType));
        differentCollectionType = new EdmCollectionType(new EdmEntityTypeReference(baselineCollectionTypeElement, true));
        Assert.True(baseline.IsEquivalentTo(differentCollectionType));
    }

    [Fact]
    public void EntityReferenceType_Equality_ReturnsExpectedResults()
    {
        var baselineEntityType = new EdmEntityType("NS", "Baseline", new EdmEntityType("NS", "foo", null, true, false), true, false);
        var differentBaselineEntityType = new EdmEntityType("NS", "Baseline", new EdmEntityType("NS", "bar", null, false, false), true, false);

        var baseline = new EdmEntityReferenceType(baselineEntityType);
        var match = new EdmEntityReferenceType(baselineEntityType);
        var differentEntityType = new EdmEntityReferenceType(differentBaselineEntityType);

        Assert.True(baseline.IsEquivalentTo(match));
        Assert.False(baseline.IsEquivalentTo(differentEntityType));
    }

    [Fact]
    public void NullComparison_Equivalence_ReturnsExpectedResults()
    {
        EdmEntityType entity = new EdmEntityType("Foo", "Bar", null, false, false);

        Assert.False(entity.IsEquivalentTo(null));
        Assert.False((null as IEdmEntityType).IsEquivalentTo(entity));
        Assert.True((null as IEdmEntityType).IsEquivalentTo(null));

        Assert.False(new EdmEntityTypeReference(entity, false).IsEquivalentTo(null));
        Assert.False((null as IEdmEntityTypeReference).IsEquivalentTo(new EdmEntityTypeReference(entity, false)));
        Assert.True((null as IEdmEntityTypeReference).IsEquivalentTo(null));
    }

    [Fact]
    public void StructuredTypeReference_ConversionFromNonStructuredType_ReturnsProperType()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
    }

    [Fact]
    public void PrimitiveTypeReference_ConversionFromNonPrimitiveType_ReturnsProperType()
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

        Assert.True(myBinary.AsPrimitive() is IEdmBinaryTypeReference);
        Assert.True(myBoolean.AsPrimitive() is IEdmPrimitiveTypeReference);
        Assert.True(myDateTime.AsPrimitive() is IEdmTemporalTypeReference);
        Assert.True(myTime.AsPrimitive() is IEdmTemporalTypeReference);
        Assert.True(myDateTimeOffset.AsPrimitive() is IEdmTemporalTypeReference);
        Assert.True(myDecimal.AsPrimitive() is IEdmDecimalTypeReference);
        Assert.True(myDouble.AsPrimitive() is IEdmPrimitiveTypeReference);
        Assert.True(myGuid.AsPrimitive() is IEdmPrimitiveTypeReference);
        Assert.True(mySByte.AsPrimitive() is IEdmPrimitiveTypeReference);
        Assert.True(mySingle.AsPrimitive() is IEdmPrimitiveTypeReference);
        Assert.True(myInt16.AsPrimitive() is IEdmPrimitiveTypeReference);
        Assert.True(myInt32.AsPrimitive() is IEdmPrimitiveTypeReference);
        Assert.True(myInt64.AsPrimitive() is IEdmPrimitiveTypeReference);
        Assert.True(myByte.AsPrimitive() is IEdmPrimitiveTypeReference);
        Assert.True(myStream.AsPrimitive() is IEdmPrimitiveTypeReference);
        Assert.True(myString.AsPrimitive() is IEdmStringTypeReference);
        Assert.True(myGeography.AsPrimitive() is IEdmSpatialTypeReference);
        Assert.True(myPoint.AsPrimitive() is IEdmSpatialTypeReference);
        Assert.True(myLineString.AsPrimitive() is IEdmSpatialTypeReference);
        Assert.True(myPolygon.AsPrimitive() is IEdmSpatialTypeReference);
        Assert.True(myGeographyCollection.AsPrimitive() is IEdmSpatialTypeReference);
        Assert.True(myMultiPolygon.AsPrimitive() is IEdmSpatialTypeReference);
        Assert.True(myMultiLineString.AsPrimitive() is IEdmSpatialTypeReference);
        Assert.True(myMultiPoint.AsPrimitive() is IEdmSpatialTypeReference);
        Assert.True(myGeometry.AsPrimitive() is IEdmSpatialTypeReference);
        Assert.True(myGeometricPoint.AsPrimitive() is IEdmSpatialTypeReference);
        Assert.True(myGeometricLineString.AsPrimitive() is IEdmSpatialTypeReference);
        Assert.True(myGeometricPolygon.AsPrimitive() is IEdmSpatialTypeReference);
        Assert.True(myGeometryCollection.AsPrimitive() is IEdmSpatialTypeReference);
        Assert.True(myGeometricMultiPolygon.AsPrimitive() is IEdmSpatialTypeReference);
        Assert.True(myGeometricMultiPoint.AsPrimitive() is IEdmSpatialTypeReference);
    }

    [Fact]
    public void UnresolvedFunction_ApplyExpression_ReturnsBadOperation()
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
        Assert.True(badOperation.ReturnType.Definition.IsBad());
        Assert.Null(badOperation.FindParameter("Foo"));
        Assert.Empty(badOperation.Parameters);
    }
}
