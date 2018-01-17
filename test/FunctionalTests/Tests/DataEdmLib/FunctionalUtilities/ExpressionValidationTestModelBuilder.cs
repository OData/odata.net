//---------------------------------------------------------------------
// <copyright file="ExpressionValidationTestModelBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.Test.OData.Utils.Metadata;

    public static class ExpressionValidationTestModelBuilder
    {
        public static XElement[] NullForNonNullableTerm(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""NonNullableTerm"" Type=""Edm.Int32"" Nullable=""false"" />
</Schema>
";
            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocab.NonNullableTerm"">
      <Null />
    </Annotation>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1, csdl2 }, edmVersion);
        }

        public static XElement[] PrimitiveForNonPrimitiveTerm(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""VocabComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
  </ComplexType>
  <Term Name=""ComplexTerm"" Type=""Vocab.VocabComplex"" />
</Schema>
";
            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocab.ComplexTerm"" Int=""32"" />
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1, csdl2 }, edmVersion);
        }

        public static XElement[] IncorrectPrimitiveTypeForTerm(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" Alias=""Vocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PrimitiveTerm"" Type=""Edm.Int32"" />
</Schema>
";
            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" String=""borkborkbork"" />
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1, csdl2 }, edmVersion);
        }

        public static XElement[] OkayPrimitiveTerm(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""IntegerTerm"" Type=""Edm.Int64"" />
</Schema>
";
            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocab.IntegerTerm"" Int=""32"" />
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1, csdl2 }, edmVersion);
        }

        public static XElement[] OkayCollectionTerm(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""CollectionTerm"" Type=""Collection(Edm.Int64)"" />
</Schema>
";
            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocab.CollectionTerm"">
        <Collection>
            <Int>1</Int>
            <Int>2</Int>
        </Collection>
    </Annotation>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1, csdl2 }, edmVersion);
        }

        public static XElement[] BadCollectionTermItemOfIncorrectType(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""CollectionTerm"" Type=""Collection(Edm.Int64)"" />
</Schema>
";
            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocab.CollectionTerm"">
        <Collection>
            <Int>1</Int>
            <Int>2</Int>
            <String>Not an Int</String>
        </Collection>
    </Annotation>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1, csdl2 }, edmVersion);
        }

        public static XElement[] BadCollectionTermIncorrectDeclaredType(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""CollectionTerm"" Type=""Collection(Edm.Int64)"" />
</Schema>
";
            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocab.CollectionTerm"">
        <Collection Type=""Collection(Edm.String)"">
            <String>1</String>
            <String>2</String>
        </Collection>
    </Annotation>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1, csdl2 }, edmVersion);
        }

        public static XElement[] OkayRecordTerm(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""VocabComplex"">
    <Property Name=""ID"" Type=""Edm.Int64"" />
    <Property Name=""Prop1"" Type=""Edm.Int64"" />
    <Property Name=""Prop2"" Type=""Edm.String"" />
  </ComplexType>
  <Term Name=""StructuredTerm"" Type=""Vocab.VocabComplex"" />
</Schema>";
            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocab.StructuredTerm"">
        <Record>
          <PropertyValue Property=""Prop1"">
            <Int>256</Int>
          </PropertyValue>
          <PropertyValue Property=""Prop2"">
            <String>A road less traveled.</String>
          </PropertyValue>
          <PropertyValue Property=""ID"">
            <Int>1</Int>
          </PropertyValue>
        </Record>
    </Annotation>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1, csdl2 }, edmVersion);
        }

        public static XElement[] BadRecordTermRenamedProperty(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""VocabComplex"">
    <Property Name=""ID"" Type=""Edm.Int64"" />
    <Property Name=""Prop1"" Type=""Edm.Int64"" />
    <Property Name=""Prop2"" Type=""Edm.String"" />
  </ComplexType>
  <Term Name=""StructuredTerm"" Type=""Vocab.VocabComplex"" />
</Schema>";
            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocab.StructuredTerm"">
        <Record>
          <PropertyValue Property=""Prop1"">
            <Int>256</Int>
          </PropertyValue>
          <PropertyValue Property=""PropBAD"">
            <String>A road less traveled.</String>
          </PropertyValue>
          <PropertyValue Property=""ID"">
            <Int>1</Int>
          </PropertyValue>
        </Record>
    </Annotation>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1, csdl2 }, edmVersion);
        }

        public static XElement[] BadRecordTermMisTypedProperty(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""VocabComplex"">
    <Property Name=""ID"" Type=""Edm.Int64"" />
    <Property Name=""Prop1"" Type=""Edm.Int64"" />
    <Property Name=""Prop2"" Type=""Edm.String"" />
  </ComplexType>
  <Term Name=""StructuredTerm"" Type=""Vocab.VocabComplex"" />
</Schema>";
            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocab.StructuredTerm"">
        <Record>
          <PropertyValue Property=""Prop1"">
            <Int>256</Int>
          </PropertyValue>
          <PropertyValue Property=""Prop2"">
            <Binary>DEADBEEFCAFE</Binary>
          </PropertyValue>
          <PropertyValue Property=""ID"">
            <Int>1</Int>
          </PropertyValue>
        </Record>
    </Annotation>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1, csdl2 }, edmVersion);
        }

        public static XElement[] BadCollectionElementInconsistentWithDeclaredType(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""UnTypedTerm"" Type=""Collection(Edm.String)"" />
</Schema>
";
            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocab.UnTypedTerm"">
        <Collection Type=""Collection(Edm.String)"">
            <String>1</String>
            <Int>2</Int>
        </Collection>
    </Annotation>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1, csdl2 }, edmVersion);
        }

        public static XElement[] BadRecordWithNonStructuredType(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""EnumTerm"" Type =""CSDL.Color"" />
</Schema>";

            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EnumType Name=""Color"">
      <Member Name=""Red""/>
      <Member Name=""Green""/>
      <Member Name=""Blue""/>
      <Member Name=""Yellow""/>
   </EnumType>
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocab.EnumTerm"">
        <Record Type=""CSDL.Color"" >
          <PropertyValue Property=""Prop1"">
            <Int>256</Int>
          </PropertyValue>
          <PropertyValue Property=""Prop2"">
            <Binary>DEADBEEFCAFE</Binary>
          </PropertyValue>
          <PropertyValue Property=""ID"">
            <Int>1</Int>
          </PropertyValue>
        </Record>
    </Annotation>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1, csdl2 }, edmVersion);
        }

        public static XElement[] BadRecordTermMisTypedPropertyForUntypedTerm(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""ComplexTerm"" Type=""CSDL.TermComplex""/>
</Schema>";
            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""TermComplex"">
    <Property Name=""ID"" Type=""Edm.Int64"" />
    <Property Name=""Prop1"" Type=""Edm.Int64"" />
    <Property Name=""Prop2"" Type=""Edm.String"" />
  </ComplexType>
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocab.ComplexTerm"">
        <Record Type=""CSDL.TermComplex"">
          <PropertyValue Property=""Prop1"">
            <Int>256</Int>
          </PropertyValue>
          <PropertyValue Property=""Prop2"">
            <Binary>DEADBEEFCAFE</Binary>
          </PropertyValue>
          <PropertyValue Property=""ID"">
            <Int>1</Int>
          </PropertyValue>
        </Record>
    </Annotation>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1, csdl2 }, edmVersion);
        }

        public static XElement[] BinaryValueInHexidecimalFormat(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Annotations"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""bar.BinaryValue"" Binary=""0x1234"" />
    </Annotations>
</Schema>";

            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""bar"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""BinaryValue"" Type=""Binary"" />
</Schema>";

            var csdl3 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
    </EntityType>
</Schema>";

            return FixUpWithEdmVersion(new string[] { csdl1, csdl2, csdl3 }, edmVersion);
        }

        public static XElement[] ComplexTypeTerm(EdmVersion edmVersion)
        {
            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""TermComplex"">
    <Property Name=""ID"" Type=""Edm.Int64"" />
    <Property Name=""Prop1"" Type=""Edm.Int64"" />
    <Property Name=""Prop2"" Type=""Edm.String"" />
  </ComplexType>
  <Term Name=""TermComplexTerm"" Type=""CSDL.TermComplex"" />
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""CSDL.TermComplexTerm"">
        <Record>
            <PropertyValue Property=""Prop1"">
              <Int>256</Int>
            </PropertyValue>
            <PropertyValue Property=""Prop2"">
              <String>HappyHappy</String>
            </PropertyValue>
            <PropertyValue Property=""ID"">
              <Int>1</Int>
            </PropertyValue>
        </Record>
    </Annotation>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl2 }, edmVersion);
        }

        public static XElement[] InvalidTypeUsingCastCollectionCsdl(EdmVersion edmVersion)
        {
            var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""FriendInfo"" Type=""NS.Friend"" />
    <ComplexType Name=""Friend"">
        <Property Name=""Name"" Type=""Edm.String"" />
        <Property Name=""NickNames"" Type=""Collection(String)"" />
        <Property Name=""Address"" Type=""NS.Address"" />
    </ComplexType>
    <ComplexType Name=""Address"">
        <Property Name=""StreetNumber"" Type=""Edm.Int32"" />
        <Property Name=""StreetName"" Type=""String"" />
    </ComplexType>
    <Annotations Target=""NS.FriendInfo"">
        <Annotation Term=""NS.FriendInfo"">
            <Cast Type=""NS.Address"">
                <Collection>
                    <String>foo</String>
                    <String>bar</String>
                </Collection>
            </Cast>
        </Annotation>
    </Annotations>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl }, edmVersion);
        }

        public static IEdmModel InvalidTypeUsingCastCollectionModel()
        {
            var model = new EdmModel();

            var address = new EdmComplexType("NS", "Address");
            address.AddStructuralProperty("StreetNumber", EdmCoreModel.Instance.GetInt32(true));
            address.AddStructuralProperty("StreetName", EdmCoreModel.Instance.GetString(true));
            model.AddElement(address);

            var friend = new EdmComplexType("NS", "Friend");
            friend.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
            friend.AddStructuralProperty("NickNames", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
            friend.AddStructuralProperty("Address", new EdmComplexTypeReference(address, true));
            model.AddElement(friend);

            var friendInfo = new EdmTerm("NS", "FriendInfo", new EdmComplexTypeReference(friend, true));
            model.AddElement(friendInfo);

            var valueAnnotationCast = new EdmCastExpression(new EdmCollectionExpression(new EdmStringConstant("foo"), new EdmStringConstant("bar")), new EdmComplexTypeReference(address, true));
            var valueAnnotation = new EdmVocabularyAnnotation(
                friendInfo,
                friendInfo,
                valueAnnotationCast);
            valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.AddVocabularyAnnotation(valueAnnotation);

            return model;
        }

        public static XElement[] CastNullableToNonNullableCsdl(EdmVersion edmVersion)
        {
            var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""FriendInfo"" Type=""Collection(NS.Friend)"" />
    <ComplexType Name=""Friend"">
        <Property Name=""Name"" Type=""Edm.String"" />
        <Property Name=""NickNames"" Type=""Collection(String)"" />
        <Property Name=""Address"" Type=""NS.Address"" />
    </ComplexType>
    <ComplexType Name=""Address"">
        <Property Name=""StreetNumber"" Type=""Edm.Int32"" />
        <Property Name=""StreetName"" Type=""String"" />
    </ComplexType>
    <Annotations Target=""NS.FriendInfo"">
        <Annotation Term=""NS.FriendInfo"">
            <Cast Type=""NS.Friend"">
                <Collection>
                    <String>foo</String>
                    <String>bar</String>
                </Collection>
            </Cast>
        </Annotation>
    </Annotations>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl }, edmVersion);
        }

        public static IEdmModel CastNullableToNonNullableModel()
        {
            var model = new EdmModel();

            var address = new EdmComplexType("NS", "Address");
            address.AddStructuralProperty("StreetNumber", EdmCoreModel.Instance.GetInt32(true));
            address.AddStructuralProperty("StreetName", EdmCoreModel.Instance.GetString(true));
            model.AddElement(address);

            var friend = new EdmComplexType("NS", "Friend");
            friend.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
            friend.AddStructuralProperty("NickNames", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
            friend.AddStructuralProperty("Address", new EdmComplexTypeReference(address, true));
            model.AddElement(friend);

            var friendInfo = new EdmTerm("NS", "FriendInfo", EdmCoreModel.GetCollection(new EdmComplexTypeReference(friend, true)));
            model.AddElement(friendInfo);

            var valueAnnotationCast = new EdmCastExpression(new EdmCollectionExpression(new EdmStringConstant("foo"), new EdmStringConstant("bar")), new EdmComplexTypeReference(friend, true));
            var valueAnnotation = new EdmVocabularyAnnotation(
                friendInfo,
                friendInfo,
                valueAnnotationCast);
            valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.AddVocabularyAnnotation(valueAnnotation);

            return model;
        }

        public static XElement[] CastNullableToNonNullableOnInlineAnnotationCsdl(EdmVersion edmVersion)
        {
            var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""FriendInfo"" Type=""Collection(NS.Friend)"">
        <Annotation Term=""NS.FriendInfo"">
            <Cast Type=""NS.Friend"">
                <Collection>
                    <String>foo</String>
                    <String>bar</String>
                </Collection>
            </Cast>
        </Annotation>
    </Term>
    <ComplexType Name=""Friend"">
        <Property Name=""Name"" Type=""Edm.String"" />
        <Property Name=""NickNames"" Type=""Collection(String)"" />
        <Property Name=""Address"" Type=""NS.Address"" />
    </ComplexType>
    <ComplexType Name=""Address"">
        <Property Name=""StreetNumber"" Type=""Edm.Int32"" />
        <Property Name=""StreetName"" Type=""String"" />
    </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl }, edmVersion);
        }

        public static IEdmModel CastNullableToNonNullableOnInlineAnnotationModel()
        {
            var model = new EdmModel();

            var address = new EdmComplexType("NS", "Address");
            address.AddStructuralProperty("StreetNumber", EdmCoreModel.Instance.GetInt32(true));
            address.AddStructuralProperty("StreetName", EdmCoreModel.Instance.GetString(true));
            model.AddElement(address);

            var friend = new EdmComplexType("NS", "Friend");
            friend.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
            friend.AddStructuralProperty("NickNames", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
            friend.AddStructuralProperty("Address", new EdmComplexTypeReference(address, true));
            model.AddElement(friend);

            var friendInfo = new EdmTerm("NS", "FriendInfo", EdmCoreModel.GetCollection(new EdmComplexTypeReference(friend, true)));
            model.AddElement(friendInfo);

            var valueAnnotationCast = new EdmCastExpression(new EdmCollectionExpression(new EdmStringConstant("foo"), new EdmStringConstant("bar")), new EdmComplexTypeReference(friend, true));
            var valueAnnotation = new EdmVocabularyAnnotation(
                friendInfo,
                friendInfo,
                valueAnnotationCast);
            valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(valueAnnotation);

            return model;
        }

        public static XElement[] CastResultFalseEvaluationCsdl(EdmVersion edmVersion)
        {
            var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Friend"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""Address"" Type=""NS.Address"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""StreetNumber"" Type=""Edm.Int32"" />
        <Property Name=""StreetName"" Type=""String"" />
    </ComplexType>
    <Term Name=""FriendTerm"" Type=""NS.Friend""/>
    <Annotations Target=""NS.Friend"">
        <Annotation Term=""NS.FriendTerm"">
            <Record>
                <PropertyValue Property=""Name"" String=""foo"" />
                <PropertyValue Property=""Address"">
                    <Cast Type=""NS.Address"">
                        <Collection>
                            <String>foo</String>
                            <String>bar</String>
                        </Collection>
                    </Cast>
                </PropertyValue>
            </Record>
        </Annotation>
    </Annotations>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl }, edmVersion);
        }

        public static IEdmModel CastResultFalseEvaluationModel()
        {
            var model = new EdmModel();

            var address = new EdmComplexType("NS", "Address");
            address.AddStructuralProperty("StreetNumber", EdmCoreModel.Instance.GetInt32(true));
            address.AddStructuralProperty("StreetName", EdmCoreModel.Instance.GetString(true));
            model.AddElement(address);

            var friend = new EdmEntityType("NS", "Friend");
            var friendName = friend.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            friend.AddKeys(friendName);
            var friendAddress = friend.AddStructuralProperty("Address", new EdmComplexTypeReference(address, true));
            model.AddElement(friend);

            var friendAddressCast = new EdmCastExpression(new EdmCollectionExpression(new EdmStringConstant("foo"), new EdmStringConstant("bar")), new EdmComplexTypeReference(address, true));

            var friendTerm = new EdmTerm("NS", "FriendTerm", new EdmEntityTypeReference(friend, true));
            model.AddElement(friendTerm);

            var valueAnnotation = new EdmVocabularyAnnotation(
                friend,
                friendTerm,
                new EdmRecordExpression(
                    new EdmPropertyConstructor(friendName.Name, new EdmStringConstant("foo")),
                    new EdmPropertyConstructor(friendAddress.Name, friendAddressCast)));

            valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.AddVocabularyAnnotation(valueAnnotation);

            return model;
        }

        public static XElement[] CastResultTrueEvaluationCsdl(EdmVersion edmVersion)
        {
            var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Friend"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""Address"" Type=""NS.Address"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""StreetNumber"" Type=""Edm.Int32"" />
        <Property Name=""StreetName"" Type=""String"" />
    </ComplexType>
    <Term Name=""FriendTerm"" Type=""NS.Friend"" />
    <Annotations Target=""NS.Friend"">
        <Annotation Term=""NS.FriendTerm"">
            <Record>
                <PropertyValue Property=""Name"" String=""foo"" />
                <PropertyValue Property=""Address"">
                    <Cast Type=""NS.Address"">
                        <Record>
                            <PropertyValue Property=""StreetNumber"" Int=""3"" />
                            <PropertyValue Property=""StreetName"" String=""에O詰　갂คำŚёæ"" />
                        </Record>
                    </Cast>
                </PropertyValue>
            </Record>
        </Annotation>
    </Annotations>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl }, edmVersion);
        }

        public static IEdmModel CastResultTrueEvaluationModel()
        {
            var model = new EdmModel();

            var address = new EdmComplexType("NS", "Address");
            address.AddStructuralProperty("StreetNumber", EdmCoreModel.Instance.GetInt32(true));
            address.AddStructuralProperty("StreetName", EdmCoreModel.Instance.GetString(true));
            model.AddElement(address);

            var friend = new EdmEntityType("NS", "Friend");
            var friendName = friend.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            friend.AddKeys(friendName);
            var friendAddress = friend.AddStructuralProperty("Address", new EdmComplexTypeReference(address, true));
            model.AddElement(friend);

            var addressRecord = new EdmRecordExpression(new EdmPropertyConstructor[] { 
                new EdmPropertyConstructor("StreetNumber", new EdmIntegerConstant(3)), 
                new EdmPropertyConstructor("StreetName", new EdmStringConstant("에O詰　갂คำŚёæ")) 
            });

            var friendAddressCast = new EdmCastExpression(addressRecord, new EdmComplexTypeReference(address, true));

            var friendTerm = new EdmTerm("NS", "FriendTerm", new EdmEntityTypeReference(friend, true));
            model.AddElement(friendTerm);

            var valueAnnotation = new EdmVocabularyAnnotation(
                friend,
                friendTerm,
                new EdmRecordExpression(
                    new EdmPropertyConstructor(friendName.Name, new EdmStringConstant("foo")),
                    new EdmPropertyConstructor(friendAddress.Name, friendAddressCast)));

            valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.AddVocabularyAnnotation(valueAnnotation);

            return model;
        }

        public static XElement[] InvalidPropertyTypeUsingIsTypeOnOutOfLineAnnotationCsdl(EdmVersion edmVersion)
        {
            var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""FriendName"" Type=""Edm.String"" />
    <Annotations Target=""NS.FriendName"">
        <Annotation Term=""NS.FriendName"">
            <IsType Type=""Edm.String"">
                <String>foo</String>
            </IsType>
        </Annotation>
    </Annotations>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl }, edmVersion);
        }

        public static IEdmModel InvalidPropertyTypeUsingIsTypeOnOutOfLineAnnotationModel()
        {
            var model = new EdmModel();

            var friendName = new EdmTerm("NS", "FriendName", EdmCoreModel.Instance.GetString(true));
            model.AddElement(friendName);

            var valueAnnotation = new EdmVocabularyAnnotation(
                friendName,
                friendName,
                new EdmIsTypeExpression(new EdmStringConstant("foo"), EdmCoreModel.Instance.GetString(true)));
            valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.AddVocabularyAnnotation(valueAnnotation);

            return model;
        }

        public static XElement[] InvalidPropertyTypeUsingIsTypeOnInlineAnnotationCsdl(EdmVersion edmVersion)
        {
            var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""CarTerm"" Type=""NS.Car"" />
    <ComplexType Name=""Car"">
        <Property Name=""Expensive"" Type=""NS.Bike"" />
        <Annotation Term=""NS.CarTerm"">
            <Record>
                <PropertyValue Property=""Expensive"">
                    <IsType Type=""Edm.String"">
                        <String>foo</String>
                    </IsType>
                </PropertyValue>
            </Record>
        </Annotation>
    </ComplexType>
    <ComplexType Name=""Bike"">
        <Property Name=""Color"" Type=""String"" />
    </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl }, edmVersion);
        }

        public static IEdmModel InvalidPropertyTypeUsingIsTypeOnInlineAnnotationModel()
        {
            var model = new EdmModel();

            var bike = new EdmComplexType("NS", "Bike");
            bike.AddStructuralProperty("Color", EdmCoreModel.Instance.GetString(true));
            model.AddElement(bike);

            var car = new EdmComplexType("NS", "Car");
            var carExpensive = car.AddStructuralProperty("Expensive", new EdmComplexTypeReference(bike, true));
            model.AddElement(car);

            var carTerm = new EdmTerm("NS", "CarTerm", new EdmComplexTypeReference(car, true));
            model.AddElement(carTerm);

            var valueAnnotation = new EdmVocabularyAnnotation(
                car,
                carTerm,
                new EdmRecordExpression(
                    new EdmPropertyConstructor(carExpensive.Name, new EdmIsTypeExpression(new EdmStringConstant("foo"), EdmCoreModel.Instance.GetString(true)))));
            valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(valueAnnotation);

            return model;
        }

        public static XElement[] IsTypeResultFalseEvaluationCsdl(EdmVersion edmVersion)
        {
            var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""BooleanFlag"" Type=""Edm.Boolean"" />
    <Annotations Target=""NS.BooleanFlag"">
        <Annotation Term=""NS.BooleanFlag"">
            <IsType Type=""Edm.String"">
                <Int>32</Int>
            </IsType>
        </Annotation>
    </Annotations>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl }, edmVersion);
        }

        public static IEdmModel IsTypeResultFalseEvaluationModel()
        {
            var model = new EdmModel();

            var booleanFlag = new EdmTerm("NS", "BooleanFlag", EdmCoreModel.Instance.GetBoolean(true));
            model.AddElement(booleanFlag);

            var valueAnnotation = new EdmVocabularyAnnotation(
                booleanFlag,
                booleanFlag,
                new EdmIsTypeExpression(new EdmIntegerConstant(32), EdmCoreModel.Instance.GetString(true)));
            valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.AddVocabularyAnnotation(valueAnnotation);

            return model;
        }

        public static XElement[] IsTypeResultTrueEvaluationCsdl(EdmVersion edmVersion)
        {
            var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""BooleanFlag"">
        <Property Name=""Flag"" Type=""Edm.Boolean"" />
    </ComplexType>
    <Term Name=""BooleanFlagTerm"" Type=""NS.BooleanFlag"" />
    <Annotations Target=""NS.BooleanFlag"">
        <Annotation Term=""NS.BooleanFlagTerm"">
            <Record>
                <PropertyValue Property=""Flag"">
                    <IsType Type=""Edm.String"">
                        <String>foo</String>
                    </IsType>
                </PropertyValue>
            </Record>
        </Annotation>
    </Annotations>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl }, edmVersion);
        }

        public static XElement[] IncorrectTypeForCollectionExpression(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" Alias=""Vocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PrimitiveTerm"" Type=""Edm.Int32"" />
</Schema>
";
            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <Collection>
            <String>BorkBorkBork</String>
        </Collection>
    </Annotation>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1, csdl2 }, edmVersion);
        }

        public static XElement[] IncorrectTypeForGuidExpression(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" Alias=""Vocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PrimitiveTerm"" Type=""Edm.Int32"" />
</Schema>
";
            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <Guid>4ae71c81-c21a-40a2-8d53-f1a29ed4a2f3</Guid>
    </Annotation>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1, csdl2 }, edmVersion);
        }

        public static XElement[] IncorrectTypeForFloatingExpression(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" Alias=""Vocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PrimitiveTerm"" Type=""Edm.Int32"" />
</Schema>
";
            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <Float>3.14</Float>
    </Annotation>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1, csdl2 }, edmVersion);
        }

        public static XElement[] IncorrectTypeForDecimalExpression(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" Alias=""Vocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PrimitiveTerm"" Type=""Edm.Int32"" />
</Schema>
";
            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <Decimal>3.14</Decimal>
    </Annotation>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1, csdl2 }, edmVersion);
        }

        public static XElement[] IncorrectTypeForDateExpression(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" Alias=""Vocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PrimitiveTerm"" Type=""Edm.Int32"" />
</Schema>
";
            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <Date>2001-10-26</Date>
    </Annotation>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1, csdl2 }, edmVersion);
        }

        public static XElement[] IncorrectFormatorDateExpression(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Date"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <Date>01-10-26</Date>
    </Annotation>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1 }, edmVersion);
        }

        public static XElement[] IncorrectTypeForDateTimeExpression(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" Alias=""Vocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PrimitiveTerm"" Type=""Edm.Int32"" />
</Schema>
";
            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <DateTime>2001-10-26T21:32:52.126Z</DateTime>
    </Annotation>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1, csdl2 }, edmVersion);
        }

        public static XElement[] IncorrectTypeForDateTimeOffsetExpression(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" Alias=""Vocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PrimitiveTerm"" Type=""Edm.Int32"" />
</Schema>
";
            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <DateTimeOffset>2001-10-26T19:32:52+00:00</DateTimeOffset>
    </Annotation>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1, csdl2 }, edmVersion);
        }

        public static XElement[] IncorrectTypeForTimeOfDayExpression(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" Alias=""Vocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PrimitiveTerm"" Type=""Edm.Date"" />
</Schema>
";
            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Date"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <TimeOfDay>1:30:05.900</TimeOfDay>
    </Annotation>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1, csdl2 }, edmVersion);
        }

        public static XElement[] IncorrectFormatorTimeOfDayExpression(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.TimeOfDay"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <TimeOfDay>-1:10:26</TimeOfDay>
    </Annotation>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1 }, edmVersion);
        }

        public static XElement[] IncorrectTypeForBooleanExpression(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" Alias=""Vocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PrimitiveTerm"" Type=""Edm.Int32"" />
</Schema>
";
            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <Bool>True</Bool>
    </Annotation>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1, csdl2 }, edmVersion);
        }

        public static XElement[] StringTooLong(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" Alias=""Vocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PrimitiveTerm"" Type=""Edm.String"" MaxLength=""5""/>
</Schema>
";
            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <String>Really long string</String>
    </Annotation>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1, csdl2 }, edmVersion);
        }

        public static XElement[] BinaryTooLong(EdmVersion edmVersion)
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" Alias=""Vocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PrimitiveTerm"" Type=""Edm.Binary"" MaxLength=""5""/>
</Schema>
";
            var csdl2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"" >
        <Binary>BAADF00DCAFE</Binary>
    </Annotation>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(new string[] { csdl1, csdl2 }, edmVersion);
        }

        public static IEdmModel IsTypeResultTrueEvaluationModel()
        {
            var model = new EdmModel();

            var booleanFlag = new EdmComplexType("NS", "BooleanFlag");
            var flag = booleanFlag.AddStructuralProperty("Flag", EdmCoreModel.Instance.GetBoolean(true));
            model.AddElement(booleanFlag);

            var booleanFlagTerm = new EdmTerm("NS", "BooleanFlagTerm", new EdmComplexTypeReference(booleanFlag, true));
            model.AddElement(booleanFlagTerm);

            var valueAnnotation = new EdmVocabularyAnnotation(
                booleanFlag,
                booleanFlagTerm,
                new EdmRecordExpression(
                    new EdmPropertyConstructor(flag.Name, new EdmIsTypeExpression(new EdmStringConstant("foo"), EdmCoreModel.Instance.GetString(true)))));
            valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.AddVocabularyAnnotation(valueAnnotation);

            return model;
        }

        private static XElement[] FixUpWithEdmVersion(string[] csdls, EdmVersion edmVersion)
        {
            return csdls.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        }
    }
}
