//---------------------------------------------------------------------
// <copyright file="ToStringTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
#if SILVERLIGHT
    using Microsoft.Silverlight.Testing;
#endif
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ToStringTests : EdmLibTestCaseBase
    {
        [TestMethod]
        public void CsdlNamedTypeDefinitionToStringTest()
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
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType astonishing = (IEdmEntityType)model.FindType("AwesomeNamespace.AstonishingEntity");
            IEdmEntityType aweInspiring = (IEdmEntityType)model.FindType("AwesomeNamespace.AweInspiringEntity");
            IEdmComplexType breathTaking = (IEdmComplexType)model.FindType("AwesomeNamespace.BreathtakingComplex");
            IEdmPrimitiveType primitive = (IEdmPrimitiveType)astonishing.FindProperty("Id").Type.Definition;
            IEdmEnumType fabulous = (IEdmEnumType)model.FindType("AwesomeNamespace.FabulousEnum");

            Assert.AreEqual("AwesomeNamespace.AstonishingEntity", astonishing.ToString(), "To string correct");
            Assert.AreEqual("AwesomeNamespace.AweInspiringEntity", aweInspiring.ToString(), "To string correct");
            Assert.AreEqual("AwesomeNamespace.BreathtakingComplex", breathTaking.ToString(), "To string correct");
            Assert.AreEqual("Edm.Int32", primitive.ToString(), "To string correct");
            Assert.AreEqual("AwesomeNamespace.FabulousEnum", fabulous.ToString(), "To string correct");
        }

        [TestMethod]
        public void CsdlTypeReferenceToStringTest()
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
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

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

            Assert.IsFalse(association.IsBad(), "Associations cannot be types");
            Assert.IsTrue(association.Definition.IsBad(), "Associations cannot be types");
            Assert.AreEqual("[AwesomeNamespace.AstonishingEntity Nullable=True]", entity.ToString(), "To string correct");
            Assert.AreEqual("[AwesomeNamespace.BreathtakingComplex Nullable=True]", complex.ToString(), "To string correct");
            Assert.AreEqual("[Edm.Int32 Nullable=True]", primitive.ToString(), "To string correct");
            Assert.AreEqual("[Edm.String Nullable=True MaxLength=128 Unicode=False]", stringType.ToString(), "To string correct");
            Assert.AreEqual("[Edm.Stream Nullable=True]", stream.ToString(), "To string correct");
            Assert.AreEqual("[Edm.Binary Nullable=True MaxLength=max]", binary.ToString(), "To string correct");
            Assert.AreEqual("[Edm.DateTimeOffset Nullable=True Precision=1]", temporal.ToString(), "To string correct");
            Assert.AreEqual("[Edm.Decimal Nullable=True Precision=3 Scale=2]", decimalType.ToString(), "To string correct");
            Assert.AreEqual("[Edm.Geography Nullable=True SRID=1]", spatial.ToString(), "To string correct");
            Assert.AreEqual("[Collection([Edm.Int32 Nullable=True]) Nullable=True]", collection.ToString(), "To string correct");
            Assert.AreEqual("[EntityReference(AwesomeNamespace.AstonishingEntity) Nullable=True]", entityRef.ToString(), "To string correct");
            Assert.AreEqual("[AwesomeNamespace.FabulousEnum Nullable=True]", enumTypeRef.ToString(), "To string correct");
            Assert.AreEqual("[AwesomeNamespace.AstonishingEntity Nullable=True]", type.ToString(), "To string correct");
        }

        [TestMethod]
        public void CsdlUnnamedTypeDefinitionToStringTest()
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
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmOperation operation = (IEdmOperation)(model.FindOperations("Namespace.Function1")).First();
            IEdmCollectionType collection = (IEdmCollectionType)operation.FindParameter("P1").Type.Definition;
            IEdmEntityReferenceType entityRef = (IEdmEntityReferenceType)operation.FindParameter("P2").Type.Definition;

            Assert.AreEqual("Collection([Edm.Int32 Nullable=True])", collection.ToString(), "To string correct");
            Assert.AreEqual("EntityReference(Namespace.AstonishingEntity)", entityRef.ToString(), "To string correct");
        }

        [TestMethod]
        public void ConstructableNamedTypeDefinitionToStringTest()
        {
            IEdmEntityType astonishing = new EdmEntityType("AwesomeNamespace", "AstonishingEntity", null, false, false);
            IEdmEntityType aweInspiring = new EdmEntityType("AwesomeNamespace", "AweInspiringEntity", null, false, false);
            IEdmComplexType breathTaking = new EdmComplexType("AwesomeNamespace", "BreathtakingComplex", null, false);
            IEdmStructuredType stunning = new EdmUntypedStructuredType("AwesomeNamespace", "StunningStructured");
            IEdmPrimitiveType primitive = (IEdmPrimitiveType)EdmCoreModel.Instance.GetInt32(false).Definition;

            Assert.AreEqual("AwesomeNamespace.AstonishingEntity", astonishing.ToString(), "To string correct");
            Assert.AreEqual("AwesomeNamespace.AweInspiringEntity", aweInspiring.ToString(), "To string correct");
            Assert.AreEqual("AwesomeNamespace.BreathtakingComplex", breathTaking.ToString(), "To string correct");
            Assert.AreEqual("AwesomeNamespace.StunningStructured", stunning.ToString(), "To string correct");
            Assert.AreEqual("Edm.Int32", primitive.ToString(), "To string correct");
        }

        [TestMethod]
        public void ConstructableTypeReferenceToStringTest()
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

            Assert.AreEqual("[AwesomeNamespace.AstonishingEntity Nullable=True]", entity.ToString(), "To string correct");
            Assert.AreEqual("[AwesomeNamespace.BreathtakingComplex Nullable=True]", complex.ToString(), "To string correct");
            Assert.AreEqual("[Edm.Int32 Nullable=True]", primitive.ToString(), "To string correct");
            Assert.AreEqual("[Edm.String Nullable=True MaxLength=128 Unicode=False]", stringType.ToString(), "To string correct");
            Assert.AreEqual("[Edm.Binary Nullable=True MaxLength=max]", binary.ToString(), "To string correct");
            Assert.AreEqual("[Edm.DateTimeOffset Nullable=True Precision=1]", temporal.ToString(), "To string correct");
            Assert.AreEqual("[Edm.Decimal Nullable=True Precision=3 Scale=2]", decimalType.ToString(), "To string correct");
            Assert.AreEqual("[Edm.Geography Nullable=True SRID=1]", spatial.ToString(), "To string correct");
            Assert.AreEqual("[Collection([Edm.Int32 Nullable=True]) Nullable=True]", collection.ToString(), "To string correct");
            Assert.AreEqual("[EntityReference(AwesomeNamespace.AstonishingEntity) Nullable=True]", entityRef.ToString(), "To string correct");
            Assert.AreEqual("[AwesomeNamespace.AstonishingEntity Nullable=True]", type.ToString(), "To string correct");
        }

        [TestMethod]
        public void ConstructableUnnamedTypeDefinitionToStringTest()
        {
            IEdmEntityType astonishing = new EdmEntityType("AwesomeNamespace", "AstonishingEntity", null, false, false);

            IEdmEntityReferenceType entityRef = new EdmEntityReferenceType(astonishing);
            IEdmCollectionType collection = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(true)).CollectionDefinition();
            
            Assert.AreEqual("Collection([Edm.Int32 Nullable=True])", collection.ToString(), "To string correct");
            Assert.AreEqual("EntityReference(AwesomeNamespace.AstonishingEntity)", entityRef.ToString(), "To string correct");
        }
    }
}

