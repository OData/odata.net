//---------------------------------------------------------------------
// <copyright file="ToStringTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.Validation;

public class ToStringTests : EdmLibTestCaseBase
{
    [Fact]
    public void Validate_ToStringForCsdlNamedTypeDefinitions_ReturnsExpectedResults()
    {
        const string csdl =
        @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""AwesomeNamespace"" Alias=""Alias"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""AstonishingEntity"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
  </EntityType>
  <EntityType Name=""AweInspiringEntity"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""AstonishingID"" Type=""Int32"" />
  </EntityType>
  <ComplexType Name=""BreathtakingComplex"">
    <Property Name=""Par1"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Par2"" Type=""Int32"" Nullable=""false"" />
  </ComplexType>
  <EnumType Name=""FabulousEnum"">
    <Member Name=""m1"" />
    <Member Name=""m2"" />
  </EnumType>
</Schema>";

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmEntityType astonishing = (IEdmEntityType)model.FindType("AwesomeNamespace.AstonishingEntity");
        IEdmEntityType aweInspiring = (IEdmEntityType)model.FindType("AwesomeNamespace.AweInspiringEntity");
        IEdmComplexType breathTaking = (IEdmComplexType)model.FindType("AwesomeNamespace.BreathtakingComplex");
        IEdmPrimitiveType primitive = (IEdmPrimitiveType)astonishing.FindProperty("Id").Type.Definition;
        IEdmEnumType fabulous = (IEdmEnumType)model.FindType("AwesomeNamespace.FabulousEnum");

        Assert.Equal("AwesomeNamespace.AstonishingEntity", astonishing.ToString());
        Assert.Equal("AwesomeNamespace.AweInspiringEntity", aweInspiring.ToString());
        Assert.Equal("AwesomeNamespace.BreathtakingComplex", breathTaking.ToString());
        Assert.Equal("Edm.Int32", primitive.ToString());
        Assert.Equal("AwesomeNamespace.FabulousEnum", fabulous.ToString());
    }

    [Fact]
    public void Validate_ToStringForCsdlTypeReferences_ReturnsExpectedResults()
    {
        const string csdl =
        @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""AwesomeNamespace"" Alias=""Alias"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""AstonishingEntity"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
  </EntityType>
  <EntityType Name=""AweInspiringEntity"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""AstonishingID"" Type=""Int32"" />
  </EntityType>
  <ComplexType Name=""BreathtakingComplex"">
    <Property Name=""Par1"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Par2"" Type=""Int32"" Nullable=""false"" />
  </ComplexType>
  <Function Name=""Function1""><ReturnType Type=""Edm.Int32""/>
    <Parameter Name=""P1"" Type=""AwesomeNamespace.AstonishingEntity"" />
    <Parameter Name=""P2"" Type=""AwesomeNamespace.BreathtakingComplex"" />
    <Parameter Name=""P3"" Type=""AwesomeNamespace.ExaltedAssociation"" />
    <Parameter Name=""P4"" Type=""Edm.Int32"" />
    <Parameter Name=""P5"" Type=""Edm.String"" MaxLength=""128"" Unicode=""false"" />
    <Parameter Name=""P6"" Type=""Edm.Stream"" />
    <Parameter Name=""P7"" Type=""Edm.Binary"" MaxLength=""max""/>
    <Parameter Name=""P8"" Type=""Edm.DateTimeOffset"" Precision=""1"" />
    <Parameter Name=""P9"" Type=""Edm.Decimal"" Precision=""3"" Scale=""2""/>
    <Parameter Name=""P10"" Type=""Edm.Geography"" SRID=""1""  />
    <Parameter Name=""P11"" Type=""Ref(AwesomeNamespace.AstonishingEntity)"" />
    <Parameter Name=""P12"" Type=""Collection(Edm.Int32)"" />
    <Parameter Name=""P14"" Type=""AwesomeNamespace.FabulousEnum"" />
  </Function>
  <EnumType Name=""FabulousEnum"">
    <Member Name=""m1"" />
    <Member Name=""m2"" />
  </EnumType>
</Schema>";

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmOperation operation = (IEdmOperation)(model.FindOperations("AwesomeNamespace.Function1")).First();

        IEdmEntityTypeReference entity = operation.FindParameter("P1").Type.AsEntity();
        IEdmComplexTypeReference complex = operation.FindParameter("P2").Type.AsComplex();
        IEdmTypeReference association = operation.FindParameter("P3").Type;
        IEdmPrimitiveTypeReference primitive = operation.FindParameter("P4").Type.AsPrimitive();
        IEdmStringTypeReference stringType = operation.FindParameter("P5").Type.AsString();
        IEdmPrimitiveTypeReference stream = operation.FindParameter("P6").Type.AsPrimitive();
        IEdmBinaryTypeReference binary = operation.FindParameter("P7").Type.AsBinary();
        IEdmTemporalTypeReference temporal = operation.FindParameter("P8").Type.AsTemporal();
        IEdmDecimalTypeReference decimalType = operation.FindParameter("P9").Type.AsDecimal();
        IEdmSpatialTypeReference spatial = operation.FindParameter("P10").Type.AsSpatial();
        IEdmEntityReferenceTypeReference entityRef = operation.FindParameter("P11").Type.AsEntityReference();
        IEdmCollectionTypeReference collection = operation.FindParameter("P12").Type.AsCollection();
        IEdmEnumTypeReference enumTypeRef = operation.FindParameter("P14").Type.AsEnum();
        IEdmTypeReference type = operation.FindParameter("P1").Type;

        Assert.False(association.IsBad(), "Associations cannot be types");
        Assert.True(association.Definition.IsBad(), "Associations cannot be types");
        Assert.Equal("[AwesomeNamespace.AstonishingEntity Nullable=True]", entity.ToString());
        Assert.Equal("[AwesomeNamespace.BreathtakingComplex Nullable=True]", complex.ToString());
        Assert.Equal("[Edm.Int32 Nullable=True]", primitive.ToString());
        Assert.Equal("[Edm.String Nullable=True MaxLength=128 Unicode=False]", stringType.ToString());
        Assert.Equal("[Edm.Stream Nullable=True]", stream.ToString());
        Assert.Equal("[Edm.Binary Nullable=True MaxLength=max]", binary.ToString());
        Assert.Equal("[Edm.DateTimeOffset Nullable=True Precision=1]", temporal.ToString());
        Assert.Equal("[Edm.Decimal Nullable=True Precision=3 Scale=2]", decimalType.ToString());
        Assert.Equal("[Edm.Geography Nullable=True SRID=1]", spatial.ToString());
        Assert.Equal("[Collection([Edm.Int32 Nullable=True]) Nullable=True]", collection.ToString());
        Assert.Equal("[EntityReference(AwesomeNamespace.AstonishingEntity) Nullable=True]", entityRef.ToString());
        Assert.Equal("[AwesomeNamespace.FabulousEnum Nullable=True]", enumTypeRef.ToString());
        Assert.Equal("[AwesomeNamespace.AstonishingEntity Nullable=True]", type.ToString());
    }

    [Fact]
    public void Validate_ToStringForCsdlUnnamedTypeDefinitions_ReturnsExpectedResults()
    {
        const string csdl =
        @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Namespace"" Alias=""Alias"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""AstonishingEntity"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
  </EntityType>
  <Function Name=""Function1""><ReturnType Type=""Edm.Int32""/>
    <Parameter Name=""P1"" Type=""Collection(Edm.Int32)"" />
    <Parameter Name=""P2"" Type=""Ref(Namespace.AstonishingEntity)"" />
  </Function>
</Schema>";

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmOperation operation = (IEdmOperation)(model.FindOperations("Namespace.Function1")).First();
        IEdmCollectionType collection = (IEdmCollectionType)operation.FindParameter("P1").Type.Definition;
        IEdmEntityReferenceType entityRef = (IEdmEntityReferenceType)operation.FindParameter("P2").Type.Definition;

        Assert.Equal("Collection([Edm.Int32 Nullable=True])", collection.ToString());
        Assert.Equal("EntityReference(Namespace.AstonishingEntity)", entityRef.ToString());
    }

    [Fact]
    public void Validate_ToStringForConstructableNamedTypeDefinitions_ReturnsExpectedResults()
    {
        IEdmEntityType astonishing = new EdmEntityType("AwesomeNamespace", "AstonishingEntity", null, false, false);
        IEdmEntityType aweInspiring = new EdmEntityType("AwesomeNamespace", "AweInspiringEntity", null, false, false);
        IEdmComplexType breathTaking = new EdmComplexType("AwesomeNamespace", "BreathtakingComplex", null, false);
        IEdmStructuredType stunning = new EdmUntypedStructuredType("AwesomeNamespace", "StunningStructured");
        IEdmPrimitiveType primitive = (IEdmPrimitiveType)EdmCoreModel.Instance.GetInt32(false).Definition;

        Assert.Equal("AwesomeNamespace.AstonishingEntity", astonishing.ToString());
        Assert.Equal("AwesomeNamespace.AweInspiringEntity", aweInspiring.ToString());
        Assert.Equal("AwesomeNamespace.BreathtakingComplex", breathTaking.ToString());
        Assert.Equal("AwesomeNamespace.StunningStructured", stunning.ToString());
        Assert.Equal("Edm.Int32", primitive.ToString());
    }

    [Fact]
    public void Validate_ToStringForConstructableTypeReferences_ReturnsExpectedResults()
    {
        IEdmEntityType astonishing = new EdmEntityType("AwesomeNamespace", "AstonishingEntity", null, false, false);
        IEdmComplexType breathTaking = new EdmComplexType("AwesomeNamespace", "BreathtakingComplex", null, false);

        IEdmEntityTypeReference entity = new EdmEntityTypeReference(astonishing, true);
        IEdmComplexTypeReference complex = new EdmComplexTypeReference(breathTaking, true);
        IEdmPrimitiveTypeReference primitive = EdmCoreModel.Instance.GetInt32(true);
        IEdmStringTypeReference stringType = EdmCoreModel.Instance.GetString(false, 128, false, true);
        IEdmBinaryTypeReference binary = EdmCoreModel.Instance.GetBinary(true, null, true);
        IEdmTemporalTypeReference temporal = EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, 1, true);
        IEdmDecimalTypeReference decimalType = EdmCoreModel.Instance.GetDecimal(3, 2, true);
        IEdmSpatialTypeReference spatial = EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, 1, true);
        IEdmEntityReferenceTypeReference entityRef = new EdmEntityReferenceTypeReference(new EdmEntityReferenceType(astonishing), true);
        IEdmCollectionTypeReference collection = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(true));
        IEdmTypeReference type = new EdmEntityTypeReference(astonishing, true);

        Assert.Equal("[AwesomeNamespace.AstonishingEntity Nullable=True]", entity.ToString());
        Assert.Equal("[AwesomeNamespace.BreathtakingComplex Nullable=True]", complex.ToString());
        Assert.Equal("[Edm.Int32 Nullable=True]", primitive.ToString());
        Assert.Equal("[Edm.String Nullable=True MaxLength=128 Unicode=False]", stringType.ToString());
        Assert.Equal("[Edm.Binary Nullable=True MaxLength=max]", binary.ToString());
        Assert.Equal("[Edm.DateTimeOffset Nullable=True Precision=1]", temporal.ToString());
        Assert.Equal("[Edm.Decimal Nullable=True Precision=3 Scale=2]", decimalType.ToString());
        Assert.Equal("[Edm.Geography Nullable=True SRID=1]", spatial.ToString());
        Assert.Equal("[Collection([Edm.Int32 Nullable=True]) Nullable=True]", collection.ToString());
        Assert.Equal("[EntityReference(AwesomeNamespace.AstonishingEntity) Nullable=True]", entityRef.ToString());
        Assert.Equal("[AwesomeNamespace.AstonishingEntity Nullable=True]", type.ToString());
    }

    [Fact]
    public void Validate_ToStringForConstructableUnnamedTypeDefinitions_ReturnsExpectedResults()
    {
        IEdmEntityType astonishing = new EdmEntityType("AwesomeNamespace", "AstonishingEntity", null, false, false);

        IEdmEntityReferenceType entityRef = new EdmEntityReferenceType(astonishing);
        IEdmCollectionType collection = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(true)).CollectionDefinition();

        Assert.Equal("Collection([Edm.Int32 Nullable=True])", collection.ToString());
        Assert.Equal("EntityReference(AwesomeNamespace.AstonishingEntity)", entityRef.ToString());
    }
}