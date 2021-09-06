//---------------------------------------------------------------------
// <copyright file="ExpressionSerializationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ExpressionSerializationTests : EdmLibTestCaseBase
    {
        private IEdmModel longDefinitionModel;
        private IEdmModel shortDefinitionModel;
        private IEdmModel baseModel;
        private IEdmModel functionsModel;

        private string longDefinitionModelCsdl =
@"<Schema Namespace=""bar"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""BinaryValue"" Type=""Binary"" />
    <Term Name=""BooleanValue"" Type=""Boolean"" />
    <Term Name=""ByteValue"" Type=""Byte"" />
    <Term Name=""DateValue"" Type=""Date"" />
    <Term Name=""DateTimeOffsetValue"" Type=""DateTimeOffset"" />
    <Term Name=""DecimalValue"" Type=""Decimal"" />
    <Term Name=""DoubleValue"" Type=""Double"" />
    <Term Name=""GuidValue"" Type=""Guid"" />
    <Term Name=""Int16Value"" Type=""Int16"" />
    <Term Name=""Int32Value"" Type=""Int32"" />
    <Term Name=""Int64Value"" Type=""Int64"" />
    <Term Name=""SByteValue"" Type=""SByte"" />
    <Term Name=""SingleValue"" Type=""Single"" />
    <Term Name=""StringValue"" Type=""String"" MaxLength=""512"" />
    <Term Name=""TimeOfDayValue"" Type=""TimeOfDay"" />
    <Term Name=""GeographyValue"" Type=""Geography"" SRID=""Variable"" />
    <Term Name=""GeometryValue"" Type=""Geometry"" SRID=""Variable"" />
    <Term Name=""DurationValue"" Type=""Duration"" />
    <EntityType Name=""TransformedPerson"">
        <Property Name=""Age"" Type=""Int32"" Nullable=""false"" />
    </EntityType>
</Schema>";

        private string shortDefinitionModelCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""fooInt"" Type=""Int32"" />
</Schema>";

        private string builtinFunctionsCsdl =
@"<Schema Namespace=""Functions"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Function Name=""StringConcat""><ReturnType Type=""String""/>
    <Parameter Name=""String1"" Type=""String"" />
    <Parameter Name=""String2"" Type=""String"" />
  </Function>
  <Function Name=""True""><ReturnType Type=""Boolean"" /></Function>
  <Function Name=""False""><ReturnType Type=""Boolean"" /></Function>
</Schema>";

        public ExpressionSerializationTests()
            : base()
        {
            this.EdmVersion = EdmVersion.V40;
        }

        private void SetupModels()
        {
            this.longDefinitionModel = this.Parse(longDefinitionModelCsdl);
            this.shortDefinitionModel = this.Parse(shortDefinitionModelCsdl);
            this.functionsModel = this.Parse(builtinFunctionsCsdl);

            var model = new EdmModel();
            var person = new EdmEntityType("NS1", "Person");
            var name = person.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            person.AddKeys(name);
            person.AddStructuralProperty("Birthday", EdmCoreModel.Instance.GetDateTimeOffset(false));
            model.AddElement(person);

            this.baseModel = model;
        }

        [TestMethod]
        public void Term_Constant_OnEntityType()
        {
            this.SetupModels();

            IEdmEntityType person = this.baseModel.FindEntityType("NS1.Person");
            IEdmTerm termInt32Value = this.longDefinitionModel.FindTerm("bar.Int32Value");

            var annotation = this.CreateAndAttachVocabularyAnnotation(person, termInt32Value, new EdmIntegerConstant(100), "onPerson");

            string expectedCsdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
    </EntityType>
    <Annotations Target=""NS1.Person"">
        <Annotation Term=""bar.Int32Value"" Qualifier=""onPerson"" Int=""100"" />
    </Annotations>
</Schema>";
            this.SerializeAndVerifyAgainst(this.baseModel, expectedCsdl);
        }

        [TestMethod]
        public void Term_Constant_OnEntityType_Inline()
        {
            this.SetupModels();

            IEdmEntityType person = this.baseModel.FindEntityType("NS1.Person");
            IEdmTerm termInt32Value = this.longDefinitionModel.FindTerm("bar.Int32Value");

            var annotation = this.CreateAndAttachVocabularyAnnotation(person, termInt32Value, new EdmIntegerConstant(100), "onPerson");

            string expectedCsdlInline =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
        <Annotation Term=""bar.Int32Value"" Qualifier=""onPerson"" Int=""100"" />
    </EntityType>
</Schema>";
            annotation.SetSerializationLocation(this.baseModel, EdmVocabularyAnnotationSerializationLocation.Inline);
            this.SerializeAndVerifyAgainst(this.baseModel, expectedCsdlInline);
        }

        // [EdmLib] Serializing of vocabulary annotations output incorrect <Using>s
        [TestMethod]
        public void Term_Constant_OfDifferentUri_OnEntityType()
        {
            this.SetupModels();
            IEdmEntityType person = this.baseModel.FindEntityType("NS1.Person");

            IEdmTerm termInt32Value = this.longDefinitionModel.FindTerm("bar.Int32Value");
            this.CreateAndAttachVocabularyAnnotation(person, termInt32Value, new EdmIntegerConstant(100));

            IEdmTerm termFooInt = this.shortDefinitionModel.FindTerm("foo.fooInt");
            this.CreateAndAttachVocabularyAnnotation(person, termFooInt, new EdmIntegerConstant(99));

            string expectedCsdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
    </EntityType>
    <Annotations Target=""NS1.Person"">
        <Annotation Term=""bar.Int32Value"" Int=""100"" />
        <Annotation Term=""foo.fooInt"" Int=""99"" />
    </Annotations>
</Schema>";
            this.SerializeAndVerifyAgainst(this.baseModel, expectedCsdl);
        }

        [TestMethod]
        public void Term_Constant_CustomAlias_OnEntityType()
        {
            this.SetupModels();
            IEdmEntityType person = this.baseModel.FindEntityType("NS1.Person");

            IEdmTerm termInt32Value = this.longDefinitionModel.FindTerm("bar.Int32Value");
            this.CreateAndAttachVocabularyAnnotation(person, termInt32Value, new EdmIntegerConstant(100));

            IEdmTerm termFooInt = this.shortDefinitionModel.FindTerm("foo.fooInt");
            this.CreateAndAttachVocabularyAnnotation(person, termFooInt, new EdmIntegerConstant(99));

            string expectedCsdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
    </EntityType>
    <Annotations Target=""NS1.Person"">
        <Annotation Term=""bar.Int32Value"" Int=""100"" />
        <Annotation Term=""foo.fooInt"" Int=""99"" />
    </Annotations>
</Schema>";
            this.SerializeAndVerifyAgainst(this.baseModel, expectedCsdl);
        }

        [TestMethod]
        public void Term_Constant_OnEntityType_SeperateApplication()
        {
            this.SetupModels();
            IEdmEntityType person = this.baseModel.FindEntityType("NS1.Person");

            IEdmTerm termInt32Value = this.longDefinitionModel.FindTerm("bar.Int32Value");
            var annotation = this.CreateAndAttachVocabularyAnnotation(person, termInt32Value, new EdmIntegerConstant(100));

            string expectedCsdl1 =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
    </EntityType>
</Schema>";
            string expectedCsdl2 =
@"<Schema Namespace=""Annotation.Application"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""NS1.Person"">
        <Annotation Term=""bar.Int32Value"" Int=""100"" />
    </Annotations>
</Schema>";
            annotation.SetSchemaNamespace(this.baseModel, "Annotation.Application");
            this.SerializeAndVerifyAgainst(this.baseModel, new[] { expectedCsdl1, expectedCsdl2 });
        }

        [TestMethod]
        public void Term_Constant_AllTypes_OnProperty()
        {
            this.SetupModels();
            IEdmProperty nameProperty = this.baseModel.FindEntityType("NS1.Person").FindProperty("Name");
            IEdmProperty birthdayProperty = this.baseModel.FindEntityType("NS1.Person").FindProperty("Birthday");

            IEdmTerm termBinaryValue = this.longDefinitionModel.FindTerm("bar.BinaryValue");
            this.CreateAndAttachVocabularyAnnotation(nameProperty, termBinaryValue, new EdmBinaryConstant(new byte[] { 0x12, 0x34 }));

            IEdmTerm termBooleanValue = this.longDefinitionModel.FindTerm("bar.BooleanValue");
            this.CreateAndAttachVocabularyAnnotation(nameProperty, termBooleanValue, new EdmBooleanConstant(true));

            IEdmTerm termByteValue = this.longDefinitionModel.FindTerm("bar.ByteValue");
            this.CreateAndAttachVocabularyAnnotation(nameProperty, termByteValue, new EdmIntegerConstant(1));

            IEdmTerm termDateValue = this.longDefinitionModel.FindTerm("bar.DateValue");
            this.CreateAndAttachVocabularyAnnotation(nameProperty, termDateValue, new EdmDateConstant(new Date(2014, 8, 8)));

            IEdmTerm termDateTimeOffsetValue = this.longDefinitionModel.FindTerm("bar.DateTimeOffsetValue");
            this.CreateAndAttachVocabularyAnnotation(nameProperty, termDateTimeOffsetValue, new EdmDateTimeOffsetConstant(DateTimeOffset.Parse("2011-01-01 23:59 -7:00")));

            IEdmTerm termDecimalValue = this.longDefinitionModel.FindTerm("bar.DecimalValue");
            this.CreateAndAttachVocabularyAnnotation(nameProperty, termDecimalValue, new EdmDecimalConstant(3.1425m));

            IEdmTerm termDoubleValue = this.longDefinitionModel.FindTerm("bar.DoubleValue");
            this.CreateAndAttachVocabularyAnnotation(nameProperty, termDoubleValue, new EdmFloatingConstant(3.14159E2));

            IEdmTerm termGuidValue = this.longDefinitionModel.FindTerm("bar.GuidValue");
            this.CreateAndAttachVocabularyAnnotation(nameProperty, termGuidValue, new EdmGuidConstant(Guid.Parse("4ae71c81-c21a-40a2-8d53-f1a29ed4a2f2")));

            IEdmTerm termInt16Value = this.longDefinitionModel.FindTerm("bar.Int16Value");
            this.CreateAndAttachVocabularyAnnotation(birthdayProperty, termInt16Value, new EdmIntegerConstant(10));

            IEdmTerm termInt32Value = this.longDefinitionModel.FindTerm("bar.Int32Value");
            this.CreateAndAttachVocabularyAnnotation(birthdayProperty, termInt32Value, new EdmIntegerConstant(100));

            IEdmTerm termInt64Value = this.longDefinitionModel.FindTerm("bar.Int64Value");
            this.CreateAndAttachVocabularyAnnotation(birthdayProperty, termInt64Value, new EdmIntegerConstant(10000));

            IEdmTerm termSByteValue = this.longDefinitionModel.FindTerm("bar.SByteValue");
            this.CreateAndAttachVocabularyAnnotation(birthdayProperty, termSByteValue, new EdmIntegerConstant(-1));

            IEdmTerm termSingleValue = this.longDefinitionModel.FindTerm("bar.SingleValue");
            this.CreateAndAttachVocabularyAnnotation(birthdayProperty, termSingleValue, new EdmFloatingConstant(3.14159E1));

            IEdmTerm termStringValue = this.longDefinitionModel.FindTerm("bar.StringValue");
            this.CreateAndAttachVocabularyAnnotation(birthdayProperty, termStringValue, new EdmStringConstant("here"));

            IEdmTerm termTimeOfDayValue = this.longDefinitionModel.FindTerm("bar.TimeOfDayValue");
            this.CreateAndAttachVocabularyAnnotation(nameProperty, termTimeOfDayValue, new EdmTimeOfDayConstant(new TimeOfDay(1, 30, 9, 3)));

            string expectedCsdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Annotations Target=""NS1.Person/Birthday"">
    <Annotation Int=""10"" Term=""bar.Int16Value"" />
    <Annotation Int=""100"" Term=""bar.Int32Value"" />
    <Annotation Int=""10000"" Term=""bar.Int64Value"" />
    <Annotation Int=""-1"" Term=""bar.SByteValue"" />
    <Annotation Float=""31.4159"" Term=""bar.SingleValue"" />
    <Annotation String=""here"" Term=""bar.StringValue"" />
  </Annotations>
  <Annotations Target=""NS1.Person/Name"">
    <Annotation Binary=""1234"" Term=""bar.BinaryValue"" />
    <Annotation Bool=""true"" Term=""bar.BooleanValue"" />
    <Annotation Int=""1"" Term=""bar.ByteValue"" />    
    <Annotation DateTimeOffset=""2011-01-01T23:59:00-07:00"" Term=""bar.DateTimeOffsetValue"" />
    <Annotation Date=""2014-08-08"" Term=""bar.DateValue"" />
    <Annotation Decimal=""3.1425"" Term=""bar.DecimalValue"" />
    <Annotation Float=""314.159"" Term=""bar.DoubleValue"" />
    <Annotation Guid=""4ae71c81-c21a-40a2-8d53-f1a29ed4a2f2"" Term=""bar.GuidValue"" />
    <Annotation TimeOfDay=""01:30:09.0030000"" Term=""bar.TimeOfDayValue"" />
  </Annotations>
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Name"" />
    </Key>
    <Property Name=""Birthday"" Nullable=""false"" Type=""Edm.DateTimeOffset"" />
    <Property Name=""Name"" Nullable=""false"" Type=""Edm.String""/>
  </EntityType>
</Schema>";
            this.SerializeAndVerifyAgainst(this.baseModel, new[] { expectedCsdl });
        }

        [TestMethod]
        public void Term_FunctionApplication_OnEntityContainer()
        {
            this.SetupModels();

            var container = new EdmEntityContainer("NS1", "myContainer");
            (this.baseModel as EdmModel).AddElement(container);

            IEdmTerm termStringValue = this.longDefinitionModel.FindTerm("bar.StringValue");
            var apply = new EdmApplyExpression(
                (IEdmFunction)this.functionsModel.FindOperations("Functions.StringConcat").First(),
                new EdmStringConstant("s1"),
                new EdmStringConstant("s2"));
            this.CreateAndAttachVocabularyAnnotation(container, termStringValue, apply);

            string expectedCsdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
    </EntityType>
    <EntityContainer Name=""myContainer"" />
    <Annotations Target=""NS1.myContainer"">
        <Annotation Term=""bar.StringValue"">
            <Apply Function=""Functions.StringConcat"">
                <String>s1</String>
                <String>s2</String>
            </Apply>
        </Annotation>
    </Annotations>
</Schema>";
            this.SerializeAndVerifyAgainst(this.baseModel, expectedCsdl);
        }

        [TestMethod]
        public void Term_If_OnEntitySet()
        {
            this.SetupModels();

            var container = new EdmEntityContainer("NS1", "myContainer");
            var set = container.AddEntitySet("personSet", this.baseModel.FindEntityType("NS1.Person"));
            (this.baseModel as EdmModel).AddElement(container);

            IEdmTerm termStringValue = this.longDefinitionModel.FindTerm("bar.StringValue");
            var ifExpression = new EdmIfExpression(
                new EdmApplyExpression((IEdmFunction)this.functionsModel.FindOperations("Functions.True").First()),
                new EdmStringConstant("s1"),
                new EdmStringConstant("s2"));
            this.CreateAndAttachVocabularyAnnotation(set, termStringValue, ifExpression);

            string expectedCsdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
    </EntityType>
    <EntityContainer Name=""myContainer"">
        <EntitySet Name=""personSet"" EntityType=""NS1.Person"" />
    </EntityContainer>
    <Annotations Target=""NS1.myContainer/personSet"">
        <Annotation Term=""bar.StringValue"">
            <If>
                <Apply Function=""Functions.True"" />
                <String>s1</String>
                <String>s2</String>    
            </If>
        </Annotation>
    </Annotations>
</Schema>";
            this.SerializeAndVerifyAgainst(this.baseModel, expectedCsdl);
        }

        [TestMethod]
        public void Term_Collection_OnEntityType()
        {
            this.SetupModels();

            IEdmEntityType person = this.baseModel.FindEntityType("NS1.Person");
            IEdmTerm termStringValue = this.longDefinitionModel.FindTerm("bar.StringValue");
            var collection = new EdmCollectionExpression(new EdmStringConstant("s1"), new EdmLabeledExpression("xyz", new EdmIntegerConstant(2)));

            this.CreateAndAttachVocabularyAnnotation(person, termStringValue, collection);

            string expectedCsdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
    </EntityType>
    <Annotations Target=""NS1.Person"">
        <Annotation Term=""bar.StringValue"">
            <Collection>
                <String>s1</String>
                <LabeledElement Name=""xyz"">
                    <Int>2</Int>
                </LabeledElement>
            </Collection>
        </Annotation>
    </Annotations>
</Schema>";
            this.SerializeAndVerifyAgainst(this.baseModel, expectedCsdl);
        }

        [TestMethod]
        public void Term_Record_OnEntityType()
        {
            this.SetupModels();

            IEdmEntityType person = this.baseModel.FindEntityType("NS1.Person");
            IEdmTerm termStringValue = this.longDefinitionModel.FindTerm("bar.StringValue");
            var record = new EdmRecordExpression(
                new EdmPropertyConstructor("p1", new EdmStringConstant("s1")),
                new EdmPropertyConstructor("p2", new EdmIntegerConstant(2)));

            this.CreateAndAttachVocabularyAnnotation(person, termStringValue, record);

            string expectedCsdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
    </EntityType>
    <Annotations Target=""NS1.Person"">
        <Annotation Term=""bar.StringValue"">
            <Record>
                <PropertyValue Property=""p1"" String=""s1"" />
                <PropertyValue Property=""p2"" Int=""2"" />
            </Record>
        </Annotation>
    </Annotations>
</Schema>";
            this.SerializeAndVerifyAgainst(this.baseModel, expectedCsdl);
        }

        [TestMethod]
        public void Term_IsType_OnEntityType()
        {
            this.SetupModels();

            IEdmEntityType person = this.baseModel.FindEntityType("NS1.Person");
            IEdmTerm termStringValue = this.longDefinitionModel.FindTerm("bar.StringValue");
            var isPersonType = new EdmIsTypeExpression(new EdmStringConstant("s1"), new EdmEntityTypeReference(person, true));
            this.CreateAndAttachVocabularyAnnotation(person, termStringValue, isPersonType);

            var isStringType = new EdmIsTypeExpression(new EdmStringConstant("s2"), EdmCoreModel.Instance.GetString(true));
            this.CreateAndAttachVocabularyAnnotation(person, termStringValue, isStringType);

            string expectedCsdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
    </EntityType>
    <Annotations Target=""NS1.Person"">
        <Annotation Term=""bar.StringValue"">
            <IsType Type=""NS1.Person"">
                <String>s1</String>
            </IsType>
        </Annotation>
        <Annotation Term=""bar.StringValue"">
            <IsType Type=""Edm.String"">
                <String>s2</String>
            </IsType>
        </Annotation>
    </Annotations>
</Schema>";
            this.SerializeAndVerifyAgainst(this.baseModel, expectedCsdl);
        }

        [TestMethod]
        public void Term_Definition()
        {
            this.SetupModels();

            this.SerializeAndVerifyAgainst(this.longDefinitionModel, longDefinitionModelCsdl);
        }

        [TestMethod]
        public void Term_Definition_Collection()
        {
            this.SetupModels();

            var definitionModel = new EdmModel();
            var collectionOfInt16 = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt16(false /*isNullable*/)));
            var valueTermInt16 = new EdmTerm("NS2", "CollectionOfInt16", collectionOfInt16);
            definitionModel.AddElement(valueTermInt16);

            var collectionOfPerson = new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(this.baseModel.FindEntityType("NS1.Person"), true)));
            var valueTermPerson = new EdmTerm("NS2", "CollectionOfPerson", collectionOfPerson);
            definitionModel.AddElement(valueTermPerson);

            string expectedCsdl =
@"<Schema Namespace=""NS2"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""CollectionOfInt16"" Type=""Collection(Edm.Int16)"" Nullable=""false"" />
    <Term Name=""CollectionOfPerson"" Type=""Collection(NS1.Person)"" />
</Schema>";
            this.SerializeAndVerifyAgainst(definitionModel, expectedCsdl);
        }

        // [EdmLib] is Edm.TypeTerm still needed? If so, please define it and use it
        [TestMethod]
        public void Term_Definition_TypeInheritance()
        {
            this.SetupModels();

            var definitionModel = new EdmModel();
            var transformed = new EdmEntityType("NS2", "Transformed");
            transformed.AddStructuralProperty("Age", EdmCoreModel.Instance.GetInt32(true));
            definitionModel.AddElement(transformed);

            var moreTransformed = new EdmEntityType("NS2", "MoreTransformed", transformed);
            moreTransformed.AddStructuralProperty("FictionalAge", EdmCoreModel.Instance.GetInt32(false));
            definitionModel.AddElement(moreTransformed);

            string expectedCsdl =
@"<Schema Namespace=""NS2"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Transformed"">
        <Property Name=""Age"" Nullable=""true"" Type=""Edm.Int32"" />
    </EntityType>
    <EntityType Name=""MoreTransformed"" BaseType=""NS2.Transformed"">
        <Property Name=""FictionalAge"" Nullable=""false"" Type=""Edm.Int32"" />
    </EntityType>
</Schema>";
            this.SerializeAndVerifyAgainst(definitionModel, expectedCsdl);
        }

        [TestMethod]
        public void SerializeInvalidTypeUsingCastCollectionModel()
        {
            this.SerializeAndVerifyAgainst(ExpressionValidationTestModelBuilder.InvalidTypeUsingCastCollectionModel(), ExpressionValidationTestModelBuilder.InvalidTypeUsingCastCollectionCsdl(EdmVersion.V40), EdmVersion.V40);
        }

        [TestMethod]
        public void SerializeCastNullableToNonNullableModel()
        {
            this.SerializeAndVerifyAgainst(ExpressionValidationTestModelBuilder.CastNullableToNonNullableModel(), ExpressionValidationTestModelBuilder.CastNullableToNonNullableCsdl(EdmVersion.V40), EdmVersion.V40);
        }

        [TestMethod]
        public void SerializeCastNullableToNonNullableOnInlineAnnotationModel()
        {
            this.SerializeAndVerifyAgainst(ExpressionValidationTestModelBuilder.CastNullableToNonNullableOnInlineAnnotationModel(), ExpressionValidationTestModelBuilder.CastNullableToNonNullableOnInlineAnnotationCsdl(EdmVersion.V40), EdmVersion.V40);
        }

        [TestMethod]
        public void SerializeCastResultFalseEvaluationModel()
        {
            this.SerializeAndVerifyAgainst(ExpressionValidationTestModelBuilder.CastResultFalseEvaluationModel(), ExpressionValidationTestModelBuilder.CastResultFalseEvaluationCsdl(EdmVersion.V40), EdmVersion.V40);
        }

        [TestMethod]
        public void SerializeCastResultTrueEvaluationModel()
        {
            this.SerializeAndVerifyAgainst(ExpressionValidationTestModelBuilder.CastResultTrueEvaluationModel(), ExpressionValidationTestModelBuilder.CastResultTrueEvaluationCsdl(EdmVersion.V40), EdmVersion.V40);
        }

        [TestMethod]
        public void SerializeInvalidPropertyTypeUsingIsTypeOnOutOfLineAnnotationModel()
        {
            this.SerializeAndVerifyAgainst(ExpressionValidationTestModelBuilder.InvalidPropertyTypeUsingIsTypeOnOutOfLineAnnotationModel(), ExpressionValidationTestModelBuilder.InvalidPropertyTypeUsingIsTypeOnOutOfLineAnnotationCsdl(EdmVersion.V40), EdmVersion.V40);
        }

        [TestMethod]
        public void SerializeInvalidPropertyTypeUsingIsTypeOnInlineAnnotationModel()
        {
            this.SerializeAndVerifyAgainst(ExpressionValidationTestModelBuilder.InvalidPropertyTypeUsingIsTypeOnInlineAnnotationModel(), ExpressionValidationTestModelBuilder.InvalidPropertyTypeUsingIsTypeOnInlineAnnotationCsdl(EdmVersion.V40), EdmVersion.V40);
        }

        [TestMethod]
        public void SerializeIsTypeResultFalseEvaluationModel()
        {
            this.SerializeAndVerifyAgainst(ExpressionValidationTestModelBuilder.IsTypeResultFalseEvaluationModel(), ExpressionValidationTestModelBuilder.IsTypeResultFalseEvaluationCsdl(EdmVersion.V40), EdmVersion.V40);
        }

        [TestMethod]
        public void SerializeIsTypeResultTrueEvaluationModel()
        {
            this.SerializeAndVerifyAgainst(ExpressionValidationTestModelBuilder.IsTypeResultTrueEvaluationModel(), ExpressionValidationTestModelBuilder.IsTypeResultTrueEvaluationCsdl(EdmVersion.V40), EdmVersion.V40);
        }

        private IEdmVocabularyAnnotation CreateAndAttachVocabularyAnnotation(IEdmVocabularyAnnotatable target, IEdmTerm term, IEdmExpression value)
        {
            return this.CreateAndAttachVocabularyAnnotation(target, term, value, null);
        }

        private IEdmVocabularyAnnotation CreateAndAttachVocabularyAnnotation(IEdmVocabularyAnnotatable target, IEdmTerm term, IEdmExpression value, string qualifier)
        {
            var annotation = new EdmVocabularyAnnotation(
                target,
                term,
                qualifier,
                value);

            // ?? Unnatural API
            ((EdmModel)this.baseModel).AddVocabularyAnnotation(annotation);
            return annotation;
        }

        private IEdmModel Parse(string csdl, params IEdmModel[] referencedModels)
        {
            return this.GetParserResult(new[] { csdl }, referencedModels);
        }

        private void SerializeAndVerifyAgainst(IEdmModel model, string expectedCsdl)
        {
            this.SerializeAndVerifyAgainst(model, new[] { expectedCsdl });
        }

        private void SerializeAndVerifyAgainst(IEdmModel model, IEnumerable<string> expectedCsdls)
        {
            this.SerializeAndVerifyAgainst(model, expectedCsdls.Select(s => XElement.Load(new StringReader(s))));
        }

        private void SerializeAndVerifyAgainst(IEdmModel model, IEnumerable<XElement> expectedCsdls)
        {
            IEnumerable<XElement> actualCsdls = this.GetSerializerResult(model).Select(s => XElement.Load(new StringReader(s)));
            this.CompareCsdls(expectedCsdls, actualCsdls);
        }

        private void SerializeAndVerifyAgainst(IEdmModel model, IEnumerable<XElement> expectedCsdls, EdmVersion version)
        {
            IEnumerable<XElement> actualCsdls = this.GetSerializerResult(model).Select(s => XElement.Load(new StringReader(s)));
            var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), version);
            var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), version);

            this.CompareCsdls(updatedExpectedCsdls, updatedActualCsdls);
        }

        private void CompareCsdls(IEnumerable<XElement> expectedCsdls, IEnumerable<XElement> actualCsdls)
        {
            var verifier = new SerializerResultVerifierUsingXml();
            verifier.Verify(expectedCsdls, actualCsdls);
        }
    }
}
