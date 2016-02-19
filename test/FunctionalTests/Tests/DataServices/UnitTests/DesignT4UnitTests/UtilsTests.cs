//---------------------------------------------------------------------
// <copyright file="UtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Values;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.OData.Service.Design.UnitTests
{
    using Microsoft.OData.Client.Design.T4;

    public class EdmSchemaTypeForTest : EdmType, IEdmSchemaElement
    {
        private readonly string name;
        public string Namespace { get; private set; }

        public EdmSchemaTypeForTest(string name, string ns)
        {
            this.name = name;
            this.Namespace = ns;
        }

        public string Name
        {
            get { return this.name; }
        }

        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.None; }
        }

        public EdmSchemaElementKind SchemaElementKind { get; set; }
    }

    public class EdmTypeForTest : EdmType
    {
        private readonly string name;

        public EdmTypeForTest(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return this.name; }
        }

        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.None; }
        }
    }

    public class EdmPrimitiveType : IEdmPrimitiveType
    {
        public EdmPrimitiveTypeKind PrimitiveKind { get; set; }
        public string Namespace { get; set; }
        public string Name { get; set; }
        public EdmSchemaElementKind SchemaElementKind { get; set; }
        public EdmTypeKind TypeKind { get; set; }

        public EdmPrimitiveType(EdmPrimitiveTypeKind kind)
        {
            this.PrimitiveKind = kind;
            this.Namespace = "namespace";
            this.Name = "name";
            this.SchemaElementKind = EdmSchemaElementKind.None;
            this.TypeKind = EdmTypeKind.None;
        }
    }

    public class EdmTypeReferenceForTest : EdmTypeReference
    {
        public EdmTypeReferenceForTest(IEdmType definition, bool isNullable)
            : base(definition, isNullable)
        {
        }
    }

    public class EdmStructruedTypeForTest : EdmStructuredType
    {
        public EdmStructruedTypeForTest()
            : this(false, false, null)
        {
        }

        public EdmStructruedTypeForTest(bool isAbstract, bool isOpen, IEdmStructuredType baseStructuredType)
            : base(isAbstract, isOpen, baseStructuredType)
        {
        }

        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.None; }
        }
    }

    public class EdmPropertyForTest : EdmProperty
    {
        public EdmPropertyForTest(IEdmStructuredType declaringType, string name, IEdmTypeReference type)
            : base(declaringType, name, type)
        {
        }

        public override EdmPropertyKind PropertyKind
        {
            get { return EdmPropertyKind.None; }
        }
    }

    [TestClass]
    public class UtilsTests
    {
        private readonly ODataT4CodeGenerator.ODataClientTemplate template;
        private readonly ODataT4CodeGenerator.CodeGenerationContext context;

        public UtilsTests()
        {
            this.context = new ODataT4CodeGenerator.CodeGenerationContext(@"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityContainer Name=""EntityContainer""/>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>", "NamespacePrefix");
            this.template = new ODataT4CodeGenerator.ODataClientCSharpTemplate(context);
        }

        [TestMethod]
        public void SerializeToStringShouldIgnoreCommentsInXElement()
        {
            XElement xmlTree = new XElement("Root",
                new XElement("Child1", 1),
                new XElement("Child2", 2),
                new XElement("Child3", 3)
            );
            XComment comment = new XComment("comment");
            xmlTree.Add(comment);

            ODataT4CodeGenerator.Utils.SerializeToString(xmlTree).Should().Be(@"<Root>
  <Child1>1</Child1>
  <Child2>2</Child2>
  <Child3>3</Child3>
</Root>");
        }

        [TestMethod]
        public void CamelCaseShouldNotThrowOnEmptyStringArgument()
        {
            ODataT4CodeGenerator.Utils.CamelCase(string.Empty).Should().BeEmpty();
        }

        [TestMethod]
        public void CamelCaseShouldNotThrowOnNullArgument()
        {
            ODataT4CodeGenerator.Utils.CamelCase(null).Should().BeNull();
        }

        [TestMethod]
        public void CamelCaseShouldReturnOneLowerCaseWithTextLengthEqualsOne()
        {
            ODataT4CodeGenerator.Utils.CamelCase("A").Should().Be("a");
        }

        [TestMethod]
        public void CamelCaseShouldReturnTextWithFirstCharacterSetToLowerCase()
        {
            ODataT4CodeGenerator.Utils.CamelCase("Text").Should().Be("text");
        }

        [TestMethod]
        public void PascalCaseShouldNotThrowOnEmptyStringArgument()
        {
            ODataT4CodeGenerator.Utils.PascalCase(string.Empty).Should().BeEmpty();
        }

        [TestMethod]
        public void PascalCaseShouldNotThrowOnNullArgument()
        {
            ODataT4CodeGenerator.Utils.PascalCase(null).Should().BeNull();
        }

        [TestMethod]
        public void PascalCaseShouldReturnOneUpperCaseWithTextLengthEqualsOne()
        {
            ODataT4CodeGenerator.Utils.PascalCase("a").Should().Be("A");
        }

        [TestMethod]
        public void PascalCaseShouldReturnTextWithFirstCharacterSetToUpperCase()
        {
            ODataT4CodeGenerator.Utils.PascalCase("text").Should().Be("Text");
        }

        [TestMethod]
        public void GetClrTypeNameShouldThrowOnNonIEdmSchemaElement()
        {
            EdmTypeForTest edmType = new EdmTypeForTest("name");
            IEdmTypeReference edmTypeReference = new EdmTypeReferenceForTest(edmType, false);
            Action getName = () => ODataT4CodeGenerator.Utils.GetClrTypeName(edmTypeReference, false, template, context);
            getName.ShouldThrow<Exception>();
        }

        [TestMethod]
        public void GetClrTypeNameShouldReturnPrimitiveTypeNameForNonNullableIEdmPrimitiveType()
        {
            EdmPrimitiveType edmPrimitiveType = new EdmPrimitiveType(EdmPrimitiveTypeKind.String);
            IEdmTypeReference edmTypeReference = new EdmTypeReferenceForTest(edmPrimitiveType, false);
            ODataT4CodeGenerator.Utils.GetClrTypeName(edmTypeReference, false, template, context).Should().Be("string");
        }

        [TestMethod]
        public void GetClrTypeNameShouldReturnPrimitiveTypeNameForNullableIEdmPrimitiveType()
        {
            EdmPrimitiveType edmPrimitiveType = new EdmPrimitiveType(EdmPrimitiveTypeKind.String);
            IEdmTypeReference edmTypeReference = new EdmTypeReferenceForTest(edmPrimitiveType, true);
            ODataT4CodeGenerator.Utils.GetClrTypeName(edmTypeReference, false, template, context).Should().Be("string");
        }

        [TestMethod]
        public void GetClrTypeNameShouldReturnSystemNullableStructureTemplate()
        {
            EdmPrimitiveType edmPrimitiveType = new EdmPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset);
            IEdmTypeReference edmTypeReference = new EdmTypeReferenceForTest(edmPrimitiveType, true);
            ODataT4CodeGenerator.Utils.GetClrTypeName(edmTypeReference, false, template, context).Should().Be("global::System.Nullable<global::System.DateTimeOffset>");
        }

        [TestMethod]
        public void GetClrTypeNameShouldReturnComplexTypeNameForComplexType()
        {
            EdmComplexType complexType = new EdmComplexType("Namespace", "name");
            IEdmTypeReference edmTypeReference = new EdmTypeReferenceForTest(complexType, false);
            ODataT4CodeGenerator.Utils.GetClrTypeName(edmTypeReference, false, template, context).Should().Be("global::NamespacePrefix.name");
        }

        [TestMethod]
        public void GetClrTypeNameShouldReadEntityTypeNameForEntityType()
        {
            EdmEntityType entityType = new EdmEntityType("Namespace", "name");
            IEdmTypeReference edmTypeReference = new EdmTypeReferenceForTest(entityType, false);
            ODataT4CodeGenerator.Utils.GetClrTypeName(edmTypeReference, false, template, context).Should().Be("global::NamespacePrefix.name");
        }

        [TestMethod]
        public void GetClrTypeNameShouldReadEntitySingleTypeNameForEntityType()
        {
            EdmEntityType entityType = new EdmEntityType("Namespace", "name");
            IEdmTypeReference edmTypeReference = new EdmTypeReferenceForTest(entityType, false);
            ODataT4CodeGenerator.Utils.GetClrTypeName(edmTypeReference, false, template, context, isEntitySingleType:true).Should().Be("global::NamespacePrefix.nameSingle");
        }

        [TestMethod]
        public void GetClrTypeNameShouldReturnObjectModelCollectionStructureTemplate()
        {
            EdmPrimitiveType primitiveType = new EdmPrimitiveType(EdmPrimitiveTypeKind.String);
            IEdmTypeReference elementTypeReference = new EdmTypeReferenceForTest(primitiveType, false);
            IEdmCollectionType collectionType = new EdmCollectionType(elementTypeReference);
            IEdmTypeReference collectionTypeReference = new EdmTypeReferenceForTest(collectionType, false);
            ODataT4CodeGenerator.Utils.GetClrTypeName(collectionTypeReference, false, template, context).Should().Be("global::System.Collections.ObjectModel.Collection<string>");
        }

        [TestMethod]
        public void GetClrTypeNameShouldReturnObservableCollectionStructureTemplate()
        {
            EdmComplexType complexType = new EdmComplexType("Namespace", "elementName");
            IEdmTypeReference elementTypeReference = new EdmTypeReferenceForTest(complexType, false);
            IEdmCollectionType collectionType = new EdmCollectionType(elementTypeReference);
            IEdmTypeReference collectionTypeReference = new EdmTypeReferenceForTest(collectionType, false);
            ODataT4CodeGenerator.Utils.GetClrTypeName(collectionTypeReference, true, template, context).Should().Be("global::System.Collections.ObjectModel.ObservableCollection<global::NamespacePrefix.elementName>");
        }

        [TestMethod]
        public void GetClrTypeNameShouldReturnDataServiceCollectionStructureTemplate()
        {
            EdmEntityType entityType = new EdmEntityType("Namespace", "elementName");
            IEdmTypeReference elementTypeReference = new EdmTypeReferenceForTest(entityType, false);
            IEdmCollectionType collectionType = new EdmCollectionType(elementTypeReference);
            IEdmTypeReference collectionTypeReference = new EdmTypeReferenceForTest(collectionType, false);
            ODataT4CodeGenerator.Utils.GetClrTypeName(collectionTypeReference, true, template, context).Should().Be("global::Microsoft.OData.Client.DataServiceCollection<global::NamespacePrefix.elementName>");
        }

        [TestMethod]
        public void GetClrTypeNameShouldReturnICollectionStructureTemplateForCollectionOfPrimitiveType()
        {
            EdmPrimitiveType primitiveType = new EdmPrimitiveType(EdmPrimitiveTypeKind.String);
            IEdmTypeReference elementTypeReference = new EdmTypeReferenceForTest(primitiveType, false);
            IEdmCollectionType collectionType = new EdmCollectionType(elementTypeReference);
            IEdmTypeReference collectionTypeReference = new EdmTypeReferenceForTest(collectionType, false);
            ODataT4CodeGenerator.Utils.GetClrTypeName(collectionTypeReference, false, template, context, true, true, true)
                .Should().Be("global::System.Collections.Generic.ICollection<string>");
        }

        [TestMethod]
        public void GetClrTypeNameShouldReturnICollectionStructureTemplateForCollectionOfEnumType()
        {
            EdmEnumType gender = new EdmEnumType("Namespace", "Gender", EdmPrimitiveTypeKind.Byte, true);
            gender.AddMember("Male", new EdmIntegerConstant(1));
            gender.AddMember("Female", new EdmIntegerConstant(2));

            IEdmTypeReference elementTypeReference = new EdmTypeReferenceForTest(gender, false);
            IEdmCollectionType collectionType = new EdmCollectionType(elementTypeReference);
            IEdmTypeReference collectionTypeReference = new EdmTypeReferenceForTest(collectionType, false);
            ODataT4CodeGenerator.Utils.GetClrTypeName(collectionTypeReference, false, template, context, true, true, true)
                .Should().Be("global::System.Collections.Generic.ICollection<global::NamespacePrefix.Gender>");
        }

        [TestMethod]
        public void GetClrTypeNameShouldReturnICollectionStructureTemplateForCollectionOfComplexType()
        {
            EdmComplexType complexType = new EdmComplexType("Namespace", "elementName");
            IEdmTypeReference elementTypeReference = new EdmTypeReferenceForTest(complexType, false);
            IEdmCollectionType collectionType = new EdmCollectionType(elementTypeReference);
            IEdmTypeReference collectionTypeReference = new EdmTypeReferenceForTest(collectionType, false);
            ODataT4CodeGenerator.Utils.GetClrTypeName(collectionTypeReference, false, template, context, true, true, true)
                .Should().Be("global::System.Collections.Generic.ICollection<global::NamespacePrefix.elementName>");
        }

        [TestMethod]
        public void GetClrTypeNameShouldReturnICollectionStructureTemplateForCollectionOfEntityType()
        {
            EdmEntityType entityType = new EdmEntityType("Namespace", "elementName");
            IEdmTypeReference elementTypeReference = new EdmTypeReferenceForTest(entityType, false);
            IEdmCollectionType collectionType = new EdmCollectionType(elementTypeReference);
            IEdmTypeReference collectionTypeReference = new EdmTypeReferenceForTest(collectionType, false);
            ODataT4CodeGenerator.Utils.GetClrTypeName(collectionTypeReference, false, template, context, true, true, true)
                .Should().Be("global::System.Collections.Generic.ICollection<global::NamespacePrefix.elementName>");
        }

        [TestMethod]
        public void GetPropertyInitializationValueShouldNotThrowOnPropertyNotInCollectionType()
        {
            EdmPrimitiveType primitiveType = new EdmPrimitiveType(EdmPrimitiveTypeKind.String);
            IEdmTypeReference edmTypeReference = new EdmTypeReferenceForTest(primitiveType, false);
            EdmPropertyForTest property = new EdmPropertyForTest(new EdmStructruedTypeForTest(), "propertyName", edmTypeReference);
            ODataT4CodeGenerator.Utils.GetPropertyInitializationValue(property, false, template, context).Should().BeNull();
        }

        [TestMethod]
        public void GetPropertyInitializationValueShouldReturnConstructorWithNoParametersForElementsInPrimitiveType()
        {
            EdmPrimitiveType primitiveType = new EdmPrimitiveType(EdmPrimitiveTypeKind.String);
            IEdmTypeReference elementTypeReference = new EdmTypeReferenceForTest(primitiveType, false);
            IEdmCollectionType collectionType = new EdmCollectionType(elementTypeReference);
            IEdmCollectionTypeReference collectionTypeReference = new EdmCollectionTypeReference(collectionType);
            EdmPropertyForTest property = new EdmPropertyForTest(new EdmStructruedTypeForTest(), "propertyName", collectionTypeReference);
            ODataT4CodeGenerator.Utils.GetPropertyInitializationValue(property, false, template, context).Should().Be("new global::System.Collections.ObjectModel.Collection<string>()");
        }

        [TestMethod]
        public void GetPropertyInitializationValueShouldReturnConstructorWithNoParametersForElementsInEntityTypeButNotUseDataServiceCollection()
        {
            EdmEntityType entityType = new EdmEntityType("Namespace", "elementName");
            IEdmTypeReference elementTypeReference = new EdmTypeReferenceForTest(entityType, false);
            IEdmCollectionType collectionType = new EdmCollectionType(elementTypeReference);
            IEdmCollectionTypeReference collectionTypeReference = new EdmCollectionTypeReference(collectionType);
            EdmPropertyForTest property = new EdmPropertyForTest(new EdmStructruedTypeForTest(), "propertyName", collectionTypeReference);
            ODataT4CodeGenerator.Utils.GetPropertyInitializationValue(property, false, template, context).Should().Be("new global::System.Collections.ObjectModel.Collection<global::NamespacePrefix.elementName>()");
        }

        [TestMethod]
        public void GetPropertyInitializationValueShouldReturnConstructorWithDataServiceCollectionConstructorParameters()
        {
            EdmEntityType entityType = new EdmEntityType("Namespace", "elementName");
            IEdmTypeReference elementTypeReference = new EdmTypeReferenceForTest(entityType, false);
            IEdmCollectionType collectionType = new EdmCollectionType(elementTypeReference);
            IEdmCollectionTypeReference collectionTypeReference = new EdmCollectionTypeReference(collectionType);
            EdmPropertyForTest property = new EdmPropertyForTest(new EdmStructruedTypeForTest(), "propertyName", collectionTypeReference);
            ODataT4CodeGenerator.Utils.GetPropertyInitializationValue(property, true, template, context).Should().Be("new global::Microsoft.OData.Client.DataServiceCollection<global::NamespacePrefix.elementName>(null, global::Microsoft.OData.Client.TrackingMode.None)");
        }

        [TestMethod]
        public void GetClrTypeNameInt32ShouldBeInt()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.Int32), template).Should().Be("int");
        }

        [TestMethod]
        public void GetClrTypeNameInt16ShouldBeShort()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.Int16), template).Should().Be("short");
        }

        [TestMethod]
        public void GetClrTypeNameInt64ShouldBeLong()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.Int64), template).Should().Be("long");
        }

        [TestMethod]
        public void GetClrTypeNameStringShouldBeString()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.String), template).Should().Be("string");
        }

        [TestMethod]
        public void GetClrTypeNameBinaryShouldBeByte()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.Binary), template).Should().Be("byte[]");
        }

        [TestMethod]
        public void GetClrTypeNameDecimalShouldBeDecimal()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.Decimal), template).Should().Be("decimal");
        }

        [TestMethod]
        public void GetClrTypeNameSingleShouldBeFloat()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.Single), template).Should().Be("float");
        }

        [TestMethod]
        public void GetClrTypeNameBooleanShouldBeBool()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.Boolean), template).Should().Be("bool");
        }

        [TestMethod]
        public void GetClrTypeNameDoubleShouldBeDouble()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.Double), template).Should().Be("double");
        }

        [TestMethod]
        public void GetClrTypeNameGuidShouldBeGlobalSystemGuid()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.Guid), template).Should().Be("global::System.Guid");
        }

        [TestMethod]
        public void GetClrTypeNameByteShouldBeByte()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.Byte), template).Should().Be("byte");
        }

        [TestMethod]
        public void GetClrTypeNameSByteShouldBeSbyte()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.SByte), template).Should().Be("sbyte");
        }

        [TestMethod]
        public void GetClrTypeNameStreamShouldBeDataServiceStreamLink()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.Stream), template).Should().Be("global::Microsoft.OData.Client.DataServiceStreamLink");
        }

        [TestMethod]
        public void GetClrTypeNameGeographyShouldBeSpatialGeography()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.Geography), template).Should().Be("global::Microsoft.Spatial.Geography");
        }

        [TestMethod]
        public void GetClrTypeNameGeographyPointShouldBeSpatialGeographyPoint()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.GeographyPoint), template).Should().Be("global::Microsoft.Spatial.GeographyPoint");
        }

        [TestMethod]
        public void GetClrTypeNameGeographyLineStringShouldBeSpatialGeographyLineString()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.GeographyLineString), template).Should().Be("global::Microsoft.Spatial.GeographyLineString");
        }

        [TestMethod]
        public void GetClrTypeNameGeographyPolygonShouldBeSpatialGeographyPolygon()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.GeographyPolygon), template).Should().Be("global::Microsoft.Spatial.GeographyPolygon");
        }

        [TestMethod]
        public void GetClrTypeNameGeographyCollectionShouldBeSpatialGeographyCollection()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.GeographyCollection), template).Should().Be("global::Microsoft.Spatial.GeographyCollection");
        }

        [TestMethod]
        public void GetClrTypeNameGeographyMultiPolygonShouldBeSpatialGeographyMultiPolygon()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiPolygon), template).Should().Be("global::Microsoft.Spatial.GeographyMultiPolygon");
        }

        [TestMethod]
        public void GetClrTypeNameGeographyMultiLineStringShouldBeSpatialGeographyMultiLineString()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiLineString), template).Should().Be("global::Microsoft.Spatial.GeographyMultiLineString");
        }

        [TestMethod]
        public void GetClrTypeNameGeographyMultiPointShouldBeSpatialGeographyMultiPoint()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiPoint), template).Should().Be("global::Microsoft.Spatial.GeographyMultiPoint");
        }

        [TestMethod]
        public void GetClrTypeNameGeometryShouldBeSpatialGeometry()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.Geometry), template).Should().Be("global::Microsoft.Spatial.Geometry");
        }

        [TestMethod]
        public void GetClrTypeNameGeometryPointShouldBeSpatialGeometryPoint()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.GeometryPoint), template).Should().Be("global::Microsoft.Spatial.GeometryPoint");
        }

        [TestMethod]
        public void GetClrTypeNameGeometryLineStringShouldBeSpatialGeometryLineString()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.GeometryLineString), template).Should().Be("global::Microsoft.Spatial.GeometryLineString");
        }

        [TestMethod]
        public void GetClrTypeNameGeometryPolygonShouldBeSpatialGeometryPolygon()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.GeometryPolygon), template).Should().Be("global::Microsoft.Spatial.GeometryPolygon");
        }

        [TestMethod]
        public void GetClrTypeNameGeometryCollectionShouldBeSpatialGeometryCollection()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.GeometryCollection), template).Should().Be("global::Microsoft.Spatial.GeometryCollection");
        }

        [TestMethod]
        public void GetClrTypeNameGeometryMultiPolygonShouldBeSpatialGeometryMultiPolygon()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiPolygon), template).Should().Be("global::Microsoft.Spatial.GeometryMultiPolygon");
        }

        [TestMethod]
        public void GetClrTypeNameGeometryMultiLineStringShouldBeSpatialGeometryMultiLineString()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiLineString), template).Should().Be("global::Microsoft.Spatial.GeometryMultiLineString");
        }

        [TestMethod]
        public void GetClrTypeNameGeometryMultiPointShouldBeSpatialGeometryMultiPoint()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiPoint), template).Should().Be("global::Microsoft.Spatial.GeometryMultiPoint");
        }

        [TestMethod]
        public void GetClrTypeNameDateTimeOffsetShouldBeGlabalSystemDateTimeOffset()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), template).Should().Be("global::System.DateTimeOffset");
        }

        [TestMethod]
        public void GetClrTypeNameDurationShouldBeGlobalSystemTimeSpan()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.Duration), template).Should().Be("global::System.TimeSpan");
        }

        [TestMethod]
        public void GetClrTypeNameDateShouldBeGlobalMicrosoftODataEdmLibraryDate()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.Date), template).Should().Be("global::Microsoft.OData.Edm.Library.Date");
        }

        [TestMethod]
        public void GetClrTypeNameTimeOfDayShouldBeGlobalMicrosoftODataEdmLibraryTimeOfDay()
        {
            ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.TimeOfDay), template).Should().Be("global::Microsoft.OData.Edm.Library.TimeOfDay");
        }

        [TestMethod]
        public void GetClrTypeNameUnknownTypeShouldThrowException()
        {
            Action translate = () => ODataT4CodeGenerator.Utils.GetClrTypeName(new EdmPrimitiveType(EdmPrimitiveTypeKind.None), template);
            translate.ShouldThrow<Exception>().WithMessage("Type None is unrecognized");
        }
    }
}
