//---------------------------------------------------------------------
// <copyright file="ClrTypeMappingTestModelBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    public static class ClrTypeMappingTestModelBuilder
    {
        public static IEnumerable<XElement> VocabularyAnnotationClassTypeBasicTest()
        {
            var csdl =
@"<Schema Namespace='NS1' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <EntityType Name='Person'>
    <Property Name='Id' Type='Int16' Nullable='false' />
    <Property Name='FirstName' Type='String' Nullable='false' Unicode='true' />
    <Property Name='LastName' Type='String' Nullable='false' Unicode='true' />
  </EntityType>
  <ComplexType Name='Pet'>
    <Property Name='Name' Type='Edm.String' Nullable='false' />
    <Property Name='Breed' Type='Edm.String' Nullable='true' />
    <Property Name='Age' Type='Edm.Int32' Nullable='true' />
  </ComplexType>
  <Term Name='Title' Type='String' Unicode='true' />
  <Term Name='DisplaySize' Type='Int32' Nullable='false' />
  <Term Name='InspectedBy' Type='NS1.Person' Nullable='false' />
  <Term Name='AdoptPet' Type='NS1.Pet' Nullable='false' />
  <Annotations Target='NS1.Person'>
    <Annotation Term=""NS1.Coordination"" Qualifier=""Display"">
        <Record>
            <PropertyValue Property=""X"" Int=""10"" />
            <PropertyValue Property=""Y"">
                <Apply Function=""Functions.IntAdd"">
                    <Int>5</Int>
                    <Int>15</Int>
                </Apply>
            </PropertyValue>
        </Record>
    </Annotation>
    <Annotation Term=""NS1.InspectedBy"">
        <Record>
            <PropertyValue Property=""Id"" Int=""10"" />
            <PropertyValue Property=""FirstName"">
                <String>Young</String>
            </PropertyValue>
            <PropertyValue Property=""LastName"" String='Hong'>
            </PropertyValue>
        </Record>
    </Annotation>
    <Annotation Term=""NS1.AdoptPet"">
        <Record>
            <PropertyValue Property=""Name"" String=""Jacquine"" />
            <PropertyValue Property=""Breed"">
                <String>Bull Dog</String>
            </PropertyValue>
            <PropertyValue Property=""Age"" Int='3'>
            </PropertyValue>
        </Record>
    </Annotation>
    <Annotation Term=""NS1.TVDisplay"">
        <Record>
            <PropertyValue Property=""X"" Int=""10"" />
            <PropertyValue Property=""Y"">
                <Apply Function=""Functions.IntAdd"">
                    <Int>5</Int>
                    <Int>15</Int>
                </Apply>
            </PropertyValue>
            <PropertyValue Property=""Origin"">
                <Record>
                    <PropertyValue Property=""X"" Int=""10"" />
                    <PropertyValue Property=""Y"">
                        <Int>20</Int>
                    </PropertyValue>
                </Record>
            </PropertyValue>
        </Record>
    </Annotation>
    <Annotation Term=""NS1.MultiMonitors"">
        <Collection>
            <Record>
                <PropertyValue Property=""X"" Int=""10"" />
                <PropertyValue Property=""Y"">
                    <Int>20</Int>
                </PropertyValue>
            </Record>
            <Record>
                <PropertyValue Property=""X"" Int=""30"" />
                <PropertyValue Property=""Y"">
                    <Int>40</Int>
                </PropertyValue>
            </Record>
        </Collection>
    </Annotation>
    <Annotation Term=""NS1.EmptyCollection"">
        <Collection>
        </Collection>
    </Annotation>
    <Annotation Term=""NS1.LabledMultiMonitors"">
        <Collection>
            <LabeledElement Name='Label1'>
                <Record>
                    <PropertyValue Property=""X"" Int=""10"" />
                    <PropertyValue Property=""Y"">
                        <Int>20</Int>
                    </PropertyValue>
                </Record>
            </LabeledElement>
        </Collection>
    </Annotation>
  </Annotations>
</Schema>";
            return new XElement[] { XElement.Parse(csdl) };
        }

        public static IEnumerable<XElement> PrimitiveTypeBasicTest()
        {
            var csdl =
@"<Schema Namespace='NS1' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <EntityType Name='Person'>
    <Property Name='Id' Type='Int16' Nullable='false' />
    <Property Name='FirstName' Type='String' Nullable='false' Unicode='true' />
    <Property Name='LastName' Type='String' Nullable='false' Unicode='true' />
  </EntityType>
  <Term Name='Title1' Type='String' Unicode='true' />
  <Term Name='Title2' Type='Int32' Nullable='false' />
  <Term Name=""BinaryValue"" Type=""Binary"" />
  <Term Name=""BooleanValue"" Type=""Boolean"" />
  <Term Name=""ByteValue"" Type=""Byte"" />
  <Term Name=""DateTimeOffsetValue"" Type=""DateTimeOffset"" />
  <Term Name=""DecimalValue"" Type=""Decimal"" />
  <Term Name=""DoubleValue"" Type=""Double"" />
  <Term Name=""GuidValue"" Type=""Guid"" />
  <Term Name=""Int16Value"" Type=""Int16"" />
  <Term Name=""Int32Value"" Type=""Int32"" />
  <Term Name=""Int64Value"" Type=""Int64"" />
  <Term Name=""SByteValue"" Type=""SByte"" />
  <Term Name=""SingleValue"" Type=""Single"" />
  <Term Name=""StringValue"" Type=""String"" />
  <Term Name=""TimeValue"" Type=""Duration"" />
  <Term Name=""GeographyValue"" Type=""Geography"" />
  <Term Name=""GeometryValue"" Type=""Geometry"" />
  <Annotations Target='NS1.Person'>
    <Annotation Term='NS1.Title1' String='Sir' />
    <Annotation Term='NS1.Title2' Int='32' />
    <Annotation Term=""NS1.BinaryValue"" Binary=""1234"" />
    <Annotation Term=""NS1.BooleanValue"" Bool=""true"" />
    <Annotation Term=""NS1.ByteValue"" Int=""124"" />
    <Annotation Term=""NS1.DateTimeOffsetValue"" DateTimeOffset=""2011-01-01 23:59 -7:00"" />
    <Annotation Term=""NS1.DecimalValue"" Decimal=""12.345"" />
    <Annotation Term=""NS1.DoubleValue"" Float=""3.1416"" />
    <Annotation Term=""NS1.GuidValue"" Guid=""4ae71c81-c21a-40a2-8d53-f1a29ed4a2f2"" />
    <Annotation Term=""NS1.Int16Value"" Int=""0"" />
    <Annotation Term=""NS1.Int32Value"" Int=""100"" />
    <Annotation Term=""NS1.Int64Value"" Int=""99"" />
    <Annotation Term=""NS1.SByteValue"" Int=""127"" />
    <Annotation Term=""NS1.SingleValue"" Float=""3.1416E10"" />
    <Annotation Term=""NS1.StringValue"" String=""I am a string."" />
    <Annotation Term=""NS1.TimeValue"" Duration=""00:01:30.000"" />
  </Annotations>
</Schema>";
            return new XElement[] { XElement.Parse(csdl) };
        }

        public static IEnumerable<XElement> PrimitiveTypeOverflowTest()
        {
            var csdl =
@"<Schema Namespace='NS1' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <EntityType Name='Person'>
    <Property Name='Id' Type='Int16' Nullable='false' />
    <Property Name='FirstName' Type='String' Nullable='false' Unicode='true' />
    <Property Name='LastName' Type='String' Nullable='false' Unicode='true' />
  </EntityType>
  <Term Name=""SingleValue"" Type=""Single"" />
  <Term Name=""ByteValue"" Type=""Byte"" />
  <Term Name=""SByteValue"" Type=""SByte"" />
  <Term Name=""DoubleValue"" Type=""Double"" />
  <Term Name=""DurationValue"" Type=""Duration"" />
  <Annotations Target='NS1.Person'>
    <Annotation Term=""NS1.ByteValue"" Int=""257"" />
    <Annotation Term=""NS1.SByteValue"" Int=""-256"" />
    <Annotation Term=""NS1.SingleValue"" Float=""3.402823e39"" />
    <Annotation Term=""NS1.DoubleValue"" Float=""2.7976931348623157E+308"" />
    <Annotation Term=""NS1.NegativeDoubleValue"" Float=""-1.7976931348623157E+309"" />
    <Annotation Term=""NS1.NegativeSingleValue"" Float=""-3.402823e39"" />
    <Annotation Term=""NS1.DurationValue"" Duration=""P10775199DT2H48M5.4775807S"" />
  </Annotations>
</Schema>";
            return new XElement[] { XElement.Parse(csdl) };
        }

        public static IEnumerable<XElement> VocabularyAnnotationClassTypeRecursiveTest()
        {
            var csdl =
@"<Schema Namespace='NS1' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <EntityType Name='Person'>
    <Property Name='Id' Type='Int16' Nullable='false' />
    <Property Name='FirstName' Type='String' Nullable='false' Unicode='true' />
    <Property Name='LastName' Type='String' Nullable='false' Unicode='true' />
  </EntityType>
  <Annotations Target='NS1.Person'>
    <Annotation Term=""NS1.RecursiveProperty"">
        <Record>
            <PropertyValue Property=""X"" Int=""1"" />
            <PropertyValue Property=""Y"">
                <Int>2</Int>
            </PropertyValue>
            <PropertyValue Property=""Origin"">
                <Record>
                    <PropertyValue Property=""X"" Int=""3"" />
                    <PropertyValue Property=""Y"">
                        <Int>4</Int>
                    </PropertyValue>
                </Record>
            </PropertyValue>
        </Record>
    </Annotation>
  </Annotations>
</Schema>";
            return new XElement[] { XElement.Parse(csdl) };
        }

        public static IEnumerable<XElement> VocabularyAnnotationClassTypeWithNewProperties()
        {
            var csdl =
@"<Schema Namespace='NS1' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <EntityType Name='Person'>
    <Property Name='Id' Type='Int16' Nullable='false' />
    <Property Name='FirstName' Type='String' Nullable='false' Unicode='true' />
    <Property Name='LastName' Type='String' Nullable='false' Unicode='true' />
  </EntityType>
  <Annotations Target='NS1.Person'>
    <Annotation Term=""NS1.RecursivePropertyWithNewProperties"">
        <Record>
            <PropertyValue Property=""X"" Int=""1"" />
            <PropertyValue Property=""Y"">
                <Int>2</Int>
            </PropertyValue>
            <PropertyValue Property=""Origin"">
                <Int>4</Int>
            </PropertyValue>
        </Record>
    </Annotation>
  </Annotations>
</Schema>";
            return new XElement[] { XElement.Parse(csdl) };
        }

        public static IEnumerable<XElement> VocabularyAnnotationCollectionPropertyTest()
        {
            var csdl =
@"<Schema Namespace='NS1' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <EntityType Name='Person'>
    <Property Name='Id' Type='Int16' Nullable='false' />
    <Property Name='FirstName' Type='String' Nullable='false' Unicode='true' />
    <Property Name='LastName' Type='String' Nullable='false' Unicode='true' />
  </EntityType>
  <Annotations Target='NS1.Person'>
    <Annotation Term=""NS1.PersonValueAnnotation"">
        <Record>
            <PropertyValue Property=""X"">
                <Collection>
                    <Int>4</Int>
                    <Int>5</Int>
                </Collection>
            </PropertyValue>
            <PropertyValue Property=""Y"">
                <Collection>
                    <Int>6</Int>
                    <Int>7</Int>
                </Collection>
            </PropertyValue>
            <PropertyValue Property=""Z"">
                <Collection>
                    <Int>8</Int>
                    <Int>9</Int>
                </Collection>
            </PropertyValue>
            <PropertyValue Property=""C"">
                <Collection>
                   <Record>
                        <PropertyValue Property=""X"" Int=""10"" />
                        <PropertyValue Property=""Y"">
                            <Int>11</Int>
                        </PropertyValue>
                    </Record> 
                   <Record>
                        <PropertyValue Property=""X"" Int=""12"" />
                        <PropertyValue Property=""Y"">
                            <Int>13</Int>
                        </PropertyValue>
                    </Record> 
                </Collection>
            </PropertyValue>
        </Record>
    </Annotation>
  </Annotations>
</Schema>";
            return new XElement[] { XElement.Parse(csdl) };
        }

        public static IEnumerable<XElement> VocabularyAnnotationCollectionOfCollectionPropertyTest()
        {
            var csdl =
@"<Schema Namespace='NS1' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <EntityType Name='Person'>
    <Property Name='Id' Type='Int16' Nullable='false' />
    <Property Name='FirstName' Type='String' Nullable='false' Unicode='true' />
    <Property Name='LastName' Type='String' Nullable='false' Unicode='true' />
  </EntityType>
  <Annotations Target='NS1.Person'>
    <Annotation Term=""NS1.PersonValueAnnotation"">
        <Record>
            <PropertyValue Property=""C"">
                <Collection>
                    <Collection>
                        <Int>8</Int>
                        <Int>9</Int>
                    </Collection>
                </Collection>
            </PropertyValue>
        </Record>
    </Annotation>
  </Annotations>
</Schema>";
            return new XElement[] { XElement.Parse(csdl) };
        }

        public static IEnumerable<XElement> VocabularyAnnotationEnumTest()
        {
            var csdl =
@"<Schema Namespace='NS1' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <EntityType Name='Person'>
    <Property Name='Id' Type='Int16' Nullable='false' />
    <Property Name='FirstName' Type='String' Nullable='false' Unicode='true' />
    <Property Name='LastName' Type='String' Nullable='false' Unicode='true' />
  </EntityType>
  <Annotations Target='NS1.Person'>
    <Annotation Term=""NS1.PersonValueAnnotation1"">
        <Record>
            <PropertyValue Property=""EnumInt"" Int='-1'>
            </PropertyValue>
            <PropertyValue Property=""EnumByte"" Int='10'>
            </PropertyValue>
            <PropertyValue Property=""EnumULong"" Int='12'>
            </PropertyValue>
        </Record>
    </Annotation>
    <Annotation Term=""NS1.PersonValueAnnotation2"" Decimal=""0.345"" />
    <Annotation Term=""NS1.PersonValueAnnotation3"" Int=""-1"" />
    <Annotation Term=""NS1.PersonValueAnnotation4"" Int=""-2"" />
    <Annotation Term=""NS1.PersonValueAnnotation8"">
        <Record>
            <PropertyValue Property=""EnumInt"" Int='10'>
            </PropertyValue>
        </Record>
    </Annotation>
  </Annotations>
</Schema>";
            return new XElement[] { XElement.Parse(csdl) };
        }

        public static IEnumerable<XElement> VocabularyAnnotationDuplicatePropertyNameTest()
        {
            var csdl =
@"<Schema Namespace='NS1' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <EntityType Name='Person'>
    <Property Name='Id' Type='Int16' Nullable='false' />
    <Property Name='FirstName' Type='String' Nullable='false' Unicode='true' />
    <Property Name='LastName' Type='String' Nullable='false' Unicode='true' />
  </EntityType>
  <Annotations Target='NS1.Person'>
    <Annotation Term=""NS1.PersonValueAnnotation1"">
        <Record>
            <PropertyValue Property=""EnumInt"" Int='10'>
            </PropertyValue>
            <PropertyValue Property=""EnumInt"" Int='11'>
            </PropertyValue>
        </Record>
    </Annotation>
  </Annotations>
</Schema>";
            return new XElement[] { XElement.Parse(csdl) };
        }

        public static IEnumerable<XElement> VocabularyAnnotationEmptyVocabularyAnnotations()
        {
            var csdl =
@"<Schema Namespace='NS1' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <EntityType Name='Person'>
    <Property Name='Id' Type='Int16' Nullable='false' />
    <Key>
        <PropertyRef Name='Id'/>
    </Key>
    <Property Name='LastName' Type='String' Nullable='false' Unicode='true' />
  </EntityType>
  <Annotations Target='NS1.Person'>
    <Annotation Term=""NS1.PersonValueAnnotation1"">
        <Record>
        </Record>
    </Annotation>
    <Annotation Term=""NS1.PersonValueAnnotation2"">
    </Annotation>
  </Annotations>
</Schema>";
            return new XElement[] { XElement.Parse(csdl) };
        }
    }
}

