//---------------------------------------------------------------------
// <copyright file="TermAttributeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TermAttributeTests : EdmLibTestCaseBase
    {
        [TestMethod]
        public void CollectionOfPrimitiveTypeTermTests()
        {
            this.RunTest(this.CollectionOfPrimitiveTypeTermModel(), this.CollectionOfPrimitiveTypeTermCsdls());
        }

        [TestMethod]
        public void CollectionOfComplexTypeTermTests()
        {
            this.RunTest(this.CollectionOfComplexTypeTermModel(), this.CollectionOfComplexTypeTermCsdls());
        }

        [TestMethod]
        public void EntityTypeTermTests()
        {
            this.RunTest(this.EntityTypeTermModel(), this.EntityTypeTermCsdls());
        }

        [TestMethod]
        public void CollectionOfEntityTypeTermTests()
        {
            this.RunTest(this.CollectionOfEntityTypeTermModel(), this.CollectionOfEntityTypeTermCsdls());
        }

        [TestMethod]
        public void TermAppliesToAttributeTests()
        {
            this.RunTest(this.TermAppliesToAttributeModel(), this.TermAppliesToAttributeCsdls());
        }

        private void RunTest(IEdmModel model, IEnumerable<XElement> csdls)
        {
            this.RunSerializeTest(model, csdls);
            this.RunDeSerializeTest(model, csdls);
            this.RunRoundTripTest(csdls);
            this.RunValidateTest(csdls);
        }

        private void RunSerializeTest(IEdmModel model, IEnumerable<XElement> expectedCsdls)
        {
            var actualCsdls = this.SerializeModelToCsdls(model);
            new ConstructiveApiCsdlXElementComparer().Compare(expectedCsdls.ToList(), actualCsdls.ToList());
        }

        private void RunDeSerializeTest(IEdmModel expectedModel, IEnumerable<XElement> csdls)
        {
            var actualModel = this.DeSerializeCsdlsToModel(csdls);
            var errors = new VocabularyModelComparer().CompareModels(expectedModel, actualModel);
            Assert.IsFalse(errors.Any(), "The actual model is not equal to the expected model");
        }

        private void RunRoundTripTest(IEnumerable<XElement> expectedCsdls)
        {
            var actualModel = this.DeSerializeCsdlsToModel(expectedCsdls);
            var actualCsdls = this.SerializeModelToCsdls(actualModel);
            new ConstructiveApiCsdlXElementComparer().Compare(expectedCsdls.ToList(), actualCsdls.ToList());
        }

        private void RunValidateTest(IEnumerable<XElement> csdls)
        {
            var model = this.DeSerializeCsdlsToModel(csdls);
            IEnumerable<EdmError> errors;
            model.Validate(out errors);
            Assert.IsFalse(errors.Any(), "The parsed model doesn't pass validation");
        }

        private IEnumerable<XElement> SerializeModelToCsdls(IEdmModel model)
        {
            List<StringBuilder> stringBuilders = new List<StringBuilder>();
            List<XmlWriter> xmlWriters = new List<XmlWriter>();
            IEnumerable<EdmError> errors;

            model.TryWriteSchema(
                s =>
                {
                    stringBuilders.Add(new StringBuilder());
                    xmlWriters.Add(XmlWriter.Create(stringBuilders.Last()));
                    return xmlWriters.Last();
                }, out errors);

            Assert.IsFalse(errors.Any(), "Error occurs while serializing model to csdl");

            for (int i = 0; i < stringBuilders.Count; i++)
            {
                xmlWriters[i].Close();
            }

            var csdls = stringBuilders.Select(stringBuilder => stringBuilder.ToString()).Select(XElement.Parse);
            return csdls;
        }

        private IEdmModel DeSerializeCsdlsToModel(IEnumerable<XElement> csdls)
        {
            IEdmModel model;
            IEnumerable<EdmError> errors;
            var isParsed = SchemaReader.TryParse(csdls.Select(e => e.CreateReader()), out model, out errors);
            Assert.IsTrue(isParsed, "Error occurs while deserializing csdl to model");
            return model;
        }

        private static IEnumerable<XElement> ConvertCsdlsToXElements(params string[] csdls)
        {
            return csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        }

        #region Collection of primitive type

        private IEdmModel CollectionOfPrimitiveTypeTermModel()
        {
            var model = new EdmModel();

            var collectionOfPrimitiveTypeEntity = new EdmEntityType("NS", "CollectionOfPrimitiveTypeEntity");
            collectionOfPrimitiveTypeEntity.AddKeys(collectionOfPrimitiveTypeEntity.AddStructuralProperty("KeyProperty", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(collectionOfPrimitiveTypeEntity);

            // Collection of Boolean
            var collectionOfBooleanProperty = collectionOfPrimitiveTypeEntity.AddStructuralProperty("CollectionOfBooleanProperty", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetBoolean(true)));
            var collectionOfBooleanTerm = new EdmTerm("NS", "CollectionOfBooleanTerm", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetBoolean(true)));
            model.AddElement(collectionOfBooleanTerm);
            var collectionOfBooleanAnnotation = new EdmVocabularyAnnotation(collectionOfBooleanProperty, collectionOfBooleanTerm, new EdmCollectionExpression(
                new EdmBooleanConstant(true),
                new EdmBooleanConstant(false)));
            model.AddVocabularyAnnotation(collectionOfBooleanAnnotation);

            // Collection of Integer
            var collectionOfIntegerProperty = collectionOfPrimitiveTypeEntity.AddStructuralProperty("CollectionOfIntegerProperty", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(true)));
            var collectionOfIntegerTerm = new EdmTerm("NS", "CollectionOfIntegerTerm", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(true)));
            model.AddElement(collectionOfIntegerTerm);
            var collectionOfIntegerAnnotation = new EdmVocabularyAnnotation(collectionOfIntegerProperty, collectionOfIntegerTerm, new EdmCollectionExpression(
                new EdmIntegerConstant(1),
                new EdmIntegerConstant(2),
                new EdmIntegerConstant(3)));
            model.AddVocabularyAnnotation(collectionOfIntegerAnnotation);

            // Collection of Floating
            var collectionOfFloatingProperty = collectionOfPrimitiveTypeEntity.AddStructuralProperty("CollectionOfFloatingProperty", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetDouble(true)));
            var collectionOfFloatingTerm = new EdmTerm("NS", "CollectionOfFloatingTerm", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetDouble(true)));
            model.AddElement(collectionOfFloatingTerm);
            var collectionOfFloatingAnnotation = new EdmVocabularyAnnotation(collectionOfFloatingProperty, collectionOfFloatingTerm, new EdmCollectionExpression(
                new EdmFloatingConstant(1.23),
                new EdmFloatingConstant(99.99)));
            model.AddVocabularyAnnotation(collectionOfFloatingAnnotation);

            // Collection of Guid
            var collectionOfGuidProperty = collectionOfPrimitiveTypeEntity.AddStructuralProperty("CollectionOfGuidProperty", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetGuid(true)));
            var collectionOfGuidTerm = new EdmTerm("NS", "CollectionOfGuidTerm", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetGuid(true)));
            model.AddElement(collectionOfGuidTerm);
            var collectionOfGuidAnnotation = new EdmVocabularyAnnotation(collectionOfGuidProperty, collectionOfGuidTerm, new EdmCollectionExpression(new EdmGuidConstant(new Guid("00000000-0000-0000-0000-000000000000"))));
            model.AddVocabularyAnnotation(collectionOfGuidAnnotation);

            // Collection of Binary
            var collectionOfBinaryProperty = collectionOfPrimitiveTypeEntity.AddStructuralProperty("CollectionOfBinaryProperty", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetBinary(true)));
            var collectionOfBinaryTerm = new EdmTerm("NS", "CollectionOfBinaryTerm", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetBinary(true)));
            model.AddElement(collectionOfBinaryTerm);
            var collectionOfBinaryAnnotation = new EdmVocabularyAnnotation(collectionOfBinaryProperty, collectionOfBinaryTerm, new EdmCollectionExpression(
                new EdmBinaryConstant(new byte[] { 0x41, 0x42 }),
                new EdmBinaryConstant(new byte[] { 0x61, 0x62 }),
                new EdmBinaryConstant(new byte[] { 0x4A, 0x4B, 0x6A, 0x6B })));
            model.AddVocabularyAnnotation(collectionOfBinaryAnnotation);

            // Collection of Decimal
            var collectionOfDecimalProperty = collectionOfPrimitiveTypeEntity.AddStructuralProperty("CollectionOfDecimalProperty", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetDecimal(true)));
            var collectionOfDecimalTerm = new EdmTerm("NS", "CollectionOfDecimalTerm", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetDecimal(true)));
            model.AddElement(collectionOfDecimalTerm);
            var collectionOfDecimalAnnotation = new EdmVocabularyAnnotation(collectionOfDecimalProperty, collectionOfDecimalTerm, new EdmCollectionExpression(
                new EdmDecimalConstant(-1.0000000000m),
                new EdmDecimalConstant(1234567890m),
                new EdmDecimalConstant(99.9999999999m)));
            collectionOfDecimalAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(collectionOfDecimalAnnotation);

            // Collection of String
            var collectionOfStringProperty = collectionOfPrimitiveTypeEntity.AddStructuralProperty("CollectionOfStringProperty", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
            var collectionOfStringTerm = new EdmTerm("NS", "CollectionOfStringTerm", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
            model.AddElement(collectionOfStringTerm);
            var collectionOfStringAnnotation = new EdmVocabularyAnnotation(collectionOfStringProperty, collectionOfStringTerm, new EdmCollectionExpression(
                new EdmStringConstant("12345"),
                new EdmStringConstant("abcdABCD"),
                new EdmStringConstant("Hello World!")));
            collectionOfStringAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(collectionOfStringAnnotation);

            // Collection of DateTimeOffset
            var collectionOfDateTimeOffsetProperty = collectionOfPrimitiveTypeEntity.AddStructuralProperty("CollectionOfDateTimeOffsetProperty", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetDateTimeOffset(true)));
            var collectionOfDateTimeOffsetTerm = new EdmTerm("NS", "CollectionOfDateTimeOffsetTerm", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetDateTimeOffset(true)));
            model.AddElement(collectionOfDateTimeOffsetTerm);
            var collectionOfDateTimeOffsetAnnotation = new EdmVocabularyAnnotation(collectionOfDateTimeOffsetProperty, collectionOfDateTimeOffsetTerm, new EdmCollectionExpression(
                new EdmDateTimeOffsetConstant(new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))),
                new EdmDateTimeOffsetConstant(new DateTimeOffset(new DateTime(2000, 12, 31, 23, 59, 59, DateTimeKind.Utc)))));
            collectionOfDateTimeOffsetAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(collectionOfDateTimeOffsetAnnotation);

            // Collection of Duration
            var collectionOfDurationProperty = collectionOfPrimitiveTypeEntity.AddStructuralProperty("CollectionOfDurationProperty", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetDuration(true)));
            var collectionOfDurationTerm = new EdmTerm("NS", "CollectionOfDurationTerm", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetDuration(true)));
            model.AddElement(collectionOfDurationTerm);
            var collectionOfDurationAnnotation = new EdmVocabularyAnnotation(collectionOfDurationProperty, collectionOfDurationTerm, new EdmCollectionExpression(
                new EdmDurationConstant(new TimeSpan(1, 2, 3, 4)),
                new EdmDurationConstant(new TimeSpan(23, 59, 59))));
            collectionOfDurationAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(collectionOfDurationAnnotation);

            return model;
        }

        private IEnumerable<XElement> CollectionOfPrimitiveTypeTermCsdls()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""CollectionOfPrimitiveTypeEntity"">
    <Key>
      <PropertyRef Name=""KeyProperty"" />
    </Key>
    <Property Name=""KeyProperty"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""CollectionOfBooleanProperty"" Type=""Collection(Edm.Boolean)"" Nullable=""true"" />
    <Property Name=""CollectionOfIntegerProperty"" Type=""Collection(Edm.Int32)"" Nullable=""true"" />
    <Property Name=""CollectionOfFloatingProperty"" Type=""Collection(Edm.Double)"" Nullable=""true"" />
    <Property Name=""CollectionOfGuidProperty"" Type=""Collection(Edm.Guid)"" Nullable=""true"" />
    <Property Name=""CollectionOfBinaryProperty"" Type=""Collection(Edm.Binary)"" Nullable=""true"" />
    <Property Name=""CollectionOfDecimalProperty"" Type=""Collection(Edm.Decimal)"" Nullable=""true"">
      <Annotation Term=""NS.CollectionOfDecimalTerm"">
        <Collection>
          <Decimal>-1.0000000000</Decimal>
          <Decimal>1234567890</Decimal>
          <Decimal>99.9999999999</Decimal>
        </Collection>
      </Annotation>
    </Property>
    <Property Name=""CollectionOfStringProperty"" Type=""Collection(Edm.String)"" Nullable=""true"">
      <Annotation Term=""NS.CollectionOfStringTerm"">
        <Collection>
          <String>12345</String>
          <String>abcdABCD</String>
          <String>Hello World!</String>
        </Collection>
      </Annotation>
    </Property>
    <Property Name=""CollectionOfDateTimeOffsetProperty"" Type=""Collection(Edm.DateTimeOffset)"" Nullable=""true"">
      <Annotation Term=""NS.CollectionOfDateTimeOffsetTerm"">
        <Collection>
          <DateTimeOffset>1970-01-01T00:00:00Z</DateTimeOffset>
          <DateTimeOffset>2000-12-31T23:59:59Z</DateTimeOffset>
        </Collection>
      </Annotation>
    </Property>
    <Property Name=""CollectionOfDurationProperty"" Type=""Collection(Edm.Duration)"" Nullable=""true"">
      <Annotation Term=""NS.CollectionOfDurationTerm"">
        <Collection>
          <Duration>P1DT2H3M4S</Duration>
          <Duration>PT23H59M59S</Duration>
        </Collection>
      </Annotation>
    </Property>
  </EntityType>
  <Term Name=""CollectionOfBooleanTerm"" Type=""Collection(Edm.Boolean)"" />
  <Term Name=""CollectionOfIntegerTerm"" Type=""Collection(Edm.Int32)"" />
  <Term Name=""CollectionOfFloatingTerm"" Type=""Collection(Edm.Double)"" />
  <Term Name=""CollectionOfGuidTerm"" Type=""Collection(Edm.Guid)"" />
  <Term Name=""CollectionOfBinaryTerm"" Type=""Collection(Edm.Binary)"" />
  <Term Name=""CollectionOfDecimalTerm"" Type=""Collection(Edm.Decimal)"" />
  <Term Name=""CollectionOfStringTerm"" Type=""Collection(Edm.String)"" />
  <Term Name=""CollectionOfDateTimeOffsetTerm"" Type=""Collection(Edm.DateTimeOffset)"" />
  <Term Name=""CollectionOfDurationTerm"" Type=""Collection(Edm.Duration)"" />
  <Annotations Target=""NS.CollectionOfPrimitiveTypeEntity/CollectionOfBooleanProperty"">
    <Annotation Term=""NS.CollectionOfBooleanTerm"">
      <Collection>
        <Bool>true</Bool>
        <Bool>false</Bool>
      </Collection>
    </Annotation>
  </Annotations>
  <Annotations Target=""NS.CollectionOfPrimitiveTypeEntity/CollectionOfIntegerProperty"">
    <Annotation Term=""NS.CollectionOfIntegerTerm"">
      <Collection>
        <Int>1</Int>
        <Int>2</Int>
        <Int>3</Int>
      </Collection>
    </Annotation>
  </Annotations>
  <Annotations Target=""NS.CollectionOfPrimitiveTypeEntity/CollectionOfFloatingProperty"">
    <Annotation Term=""NS.CollectionOfFloatingTerm"">
      <Collection>
        <Float>1.23</Float>
        <Float>99.99</Float>
      </Collection>
    </Annotation>
  </Annotations>
  <Annotations Target=""NS.CollectionOfPrimitiveTypeEntity/CollectionOfGuidProperty"">
    <Annotation Term=""NS.CollectionOfGuidTerm"">
      <Collection>
        <Guid>00000000-0000-0000-0000-000000000000</Guid>
      </Collection>
    </Annotation>
  </Annotations>
  <Annotations Target=""NS.CollectionOfPrimitiveTypeEntity/CollectionOfBinaryProperty"">
    <Annotation Term=""NS.CollectionOfBinaryTerm"">
      <Collection>
        <Binary>4142</Binary>
        <Binary>6162</Binary>
        <Binary>4A4B6A6B</Binary>
      </Collection>
    </Annotation>
  </Annotations>
</Schema>");
        }

        #endregion

        #region Collection of Complex type

        private IEdmModel CollectionOfComplexTypeTermModel()
        {
            var model = new EdmModel();

            var complexTypeElement = new EdmComplexType("NS", "ComplexTypeElement");
            complexTypeElement.AddStructuralProperty("IntegerProperty", EdmCoreModel.Instance.GetInt32(true));
            complexTypeElement.AddStructuralProperty("StringProperty", EdmCoreModel.Instance.GetString(true));
            model.AddElement(complexTypeElement);

            var collectionOfComplexTypeTerm = new EdmTerm("NS", "CollectionOfComplexTypeTerm", EdmCoreModel.GetCollection(new EdmComplexTypeReference(complexTypeElement, true)));
            model.AddElement(collectionOfComplexTypeTerm);

            var inlineCollectionOfComplexTypeAnnotation = new EdmVocabularyAnnotation(complexTypeElement, collectionOfComplexTypeTerm, new EdmCollectionExpression(
                new EdmRecordExpression(
                    new EdmPropertyConstructor("IntegerProperty", new EdmIntegerConstant(111)),
                    new EdmPropertyConstructor("StringProperty", new EdmStringConstant("Inline String 111"))),
                new EdmRecordExpression(
                    new EdmPropertyConstructor("IntegerProperty", new EdmIntegerConstant(222)),
                    new EdmPropertyConstructor("StringProperty", new EdmStringConstant("Inline String 222")))));
            inlineCollectionOfComplexTypeAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(inlineCollectionOfComplexTypeAnnotation);

            var outlineCollectionOfComplexTypeAnnotation = new EdmVocabularyAnnotation(collectionOfComplexTypeTerm, collectionOfComplexTypeTerm, new EdmCollectionExpression(
                new EdmRecordExpression(
                    new EdmPropertyConstructor("IntegerProperty", new EdmIntegerConstant(333)),
                    new EdmPropertyConstructor("StringProperty", new EdmStringConstant("Outline String 333"))),
                new EdmRecordExpression(
                    new EdmPropertyConstructor("IntegerProperty", new EdmIntegerConstant(444)),
                    new EdmPropertyConstructor("StringProperty", new EdmStringConstant("Outline String 444"))),
                new EdmRecordExpression(
                    new EdmPropertyConstructor("IntegerProperty", new EdmIntegerConstant(555)),
                    new EdmPropertyConstructor("StringProperty", new EdmStringConstant("Outline String 555")))));
            model.AddVocabularyAnnotation(outlineCollectionOfComplexTypeAnnotation);

            return model;
        }

        private IEnumerable<XElement> CollectionOfComplexTypeTermCsdls()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""ComplexTypeElement"">
    <Property Name=""IntegerProperty"" Type=""Edm.Int32"" />
    <Property Name=""StringProperty"" Type=""Edm.String"" />
    <Annotation Term=""NS.CollectionOfComplexTypeTerm"">
      <Collection>
        <Record>
          <PropertyValue Property=""IntegerProperty"" Int=""111"" />
          <PropertyValue Property=""StringProperty"" String=""Inline String 111"" />
        </Record>
        <Record>
          <PropertyValue Property=""IntegerProperty"" Int=""222"" />
          <PropertyValue Property=""StringProperty"" String=""Inline String 222"" />
        </Record>
      </Collection>
    </Annotation>
  </ComplexType>
  <Term Name=""CollectionOfComplexTypeTerm"" Type=""Collection(NS.ComplexTypeElement)"" />
  <Annotations Target=""NS.CollectionOfComplexTypeTerm"">
    <Annotation Term=""NS.CollectionOfComplexTypeTerm"">
      <Collection>
        <Record>
          <PropertyValue Property=""IntegerProperty"" Int=""333"" />
          <PropertyValue Property=""StringProperty"" String=""Outline String 333"" />
        </Record>
        <Record>
          <PropertyValue Property=""IntegerProperty"" Int=""444"" />
          <PropertyValue Property=""StringProperty"" String=""Outline String 444"" />
        </Record>
        <Record>
          <PropertyValue Property=""IntegerProperty"" Int=""555"" />
          <PropertyValue Property=""StringProperty"" String=""Outline String 555"" />
        </Record>
      </Collection>
    </Annotation>
  </Annotations>
</Schema>");
        }

        #endregion

        #region Entity type

        private IEdmModel EntityTypeTermModel()
        {
            var model = new EdmModel();

            var entityTypeElement = new EdmEntityType("NS", "EntityTypeElement");
            entityTypeElement.AddKeys(entityTypeElement.AddStructuralProperty("KeyProperty", EdmCoreModel.Instance.GetInt32(false)));
            entityTypeElement.AddStructuralProperty("IntegerProperty", EdmCoreModel.Instance.GetInt32(true));
            entityTypeElement.AddStructuralProperty("StringProperty", EdmCoreModel.Instance.GetString(true));
            model.AddElement(entityTypeElement);

            var entityTypeTerm = new EdmTerm("NS", "EntityTypeTerm", new EdmEntityTypeReference(entityTypeElement, true));
            model.AddElement(entityTypeTerm);

            var inlineEntityTypeAnnotation = new EdmVocabularyAnnotation(entityTypeElement, entityTypeTerm, new EdmRecordExpression(
                new EdmPropertyConstructor("KeyProperty", new EdmIntegerConstant(1)),
                new EdmPropertyConstructor("IntegerProperty", new EdmIntegerConstant(111)),
                new EdmPropertyConstructor("StringProperty", new EdmStringConstant("Inline String 111"))));
            inlineEntityTypeAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(inlineEntityTypeAnnotation);

            var outlineEntityTypeAnnotation = new EdmVocabularyAnnotation(entityTypeTerm, entityTypeTerm, new EdmRecordExpression(
                new EdmPropertyConstructor("KeyProperty", new EdmIntegerConstant(2)),
                new EdmPropertyConstructor("IntegerProperty", new EdmIntegerConstant(222)),
                new EdmPropertyConstructor("StringProperty", new EdmStringConstant("Outline String 222"))));
            model.AddVocabularyAnnotation(outlineEntityTypeAnnotation);

            return model;
        }

        private IEnumerable<XElement> EntityTypeTermCsdls()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""EntityTypeElement"">
    <Key>
      <PropertyRef Name=""KeyProperty"" />
    </Key>
    <Property Name=""KeyProperty"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""IntegerProperty"" Type=""Edm.Int32"" Nullable=""true"" />
    <Property Name=""StringProperty"" Type=""Edm.String"" Nullable=""true"" />
    <Annotation Term=""NS.EntityTypeTerm"">
      <Record>
        <PropertyValue Property=""KeyProperty"" Int=""1"" />
        <PropertyValue Property=""IntegerProperty"" Int=""111"" />
        <PropertyValue Property=""StringProperty"" String=""Inline String 111"" />
      </Record>
    </Annotation>
  </EntityType>
  <Term Name=""EntityTypeTerm"" Type=""NS.EntityTypeElement"" />
  <Annotations Target=""NS.EntityTypeTerm"">
    <Annotation Term=""NS.EntityTypeTerm"">
      <Record>
        <PropertyValue Property=""KeyProperty"" Int=""2"" />
        <PropertyValue Property=""IntegerProperty"" Int=""222"" />
        <PropertyValue Property=""StringProperty"" String=""Outline String 222"" />
      </Record>
    </Annotation>
  </Annotations>
</Schema>");
        }

        #endregion

        #region Collection of Entity type

        private IEdmModel CollectionOfEntityTypeTermModel()
        {
            var model = new EdmModel();

            var entityTypeElement = new EdmEntityType("NS", "EntityTypeElement");
            entityTypeElement.AddKeys(entityTypeElement.AddStructuralProperty("KeyProperty", EdmCoreModel.Instance.GetInt32(false)));
            entityTypeElement.AddStructuralProperty("IntegerProperty", EdmCoreModel.Instance.GetInt32(true));
            entityTypeElement.AddStructuralProperty("StringProperty", EdmCoreModel.Instance.GetString(true));
            model.AddElement(entityTypeElement);

            var collectionOfEntityTypeTerm = new EdmTerm("NS", "CollectionOfEntityTypeTerm", EdmCoreModel.GetCollection(new EdmEntityTypeReference(entityTypeElement, true)));
            model.AddElement(collectionOfEntityTypeTerm);

            var inlineCollectionOfEntityTypeAnnotation = new EdmVocabularyAnnotation(entityTypeElement, collectionOfEntityTypeTerm, new EdmCollectionExpression(
                new EdmRecordExpression(
                    new EdmPropertyConstructor("KeyProperty", new EdmIntegerConstant(1)),
                    new EdmPropertyConstructor("IntegerProperty", new EdmIntegerConstant(111)),
                    new EdmPropertyConstructor("StringProperty", new EdmStringConstant("Inline String 111"))),
                new EdmRecordExpression(
                    new EdmPropertyConstructor("KeyProperty", new EdmIntegerConstant(2)),
                    new EdmPropertyConstructor("IntegerProperty", new EdmIntegerConstant(222)),
                    new EdmPropertyConstructor("StringProperty", new EdmStringConstant("Inline String 222"))),
                new EdmRecordExpression(
                    new EdmPropertyConstructor("KeyProperty", new EdmIntegerConstant(3)),
                    new EdmPropertyConstructor("IntegerProperty", new EdmIntegerConstant(333)),
                    new EdmPropertyConstructor("StringProperty", new EdmStringConstant("Inline String 333")))));
            inlineCollectionOfEntityTypeAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(inlineCollectionOfEntityTypeAnnotation);

            var outlineCollectionOfEntityTypeAnnotation = new EdmVocabularyAnnotation(collectionOfEntityTypeTerm, collectionOfEntityTypeTerm, new EdmCollectionExpression(
                new EdmRecordExpression(
                    new EdmPropertyConstructor("KeyProperty", new EdmIntegerConstant(4)),
                    new EdmPropertyConstructor("IntegerProperty", new EdmIntegerConstant(444)),
                    new EdmPropertyConstructor("StringProperty", new EdmStringConstant("Inline String 444"))),
                new EdmRecordExpression(
                    new EdmPropertyConstructor("KeyProperty", new EdmIntegerConstant(5)),
                    new EdmPropertyConstructor("IntegerProperty", new EdmIntegerConstant(555)),
                    new EdmPropertyConstructor("StringProperty", new EdmStringConstant("Inline String 555")))));
            model.AddVocabularyAnnotation(outlineCollectionOfEntityTypeAnnotation);

            return model;
        }

        private IEnumerable<XElement> CollectionOfEntityTypeTermCsdls()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
<EntityType Name=""EntityTypeElement"">
    <Key>
      <PropertyRef Name=""KeyProperty"" />
    </Key>
    <Property Name=""KeyProperty"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""IntegerProperty"" Type=""Edm.Int32"" Nullable=""true"" />
    <Property Name=""StringProperty"" Type=""Edm.String"" Nullable=""true"" />
    <Annotation Term=""NS.CollectionOfEntityTypeTerm"">
      <Collection>
        <Record>
          <PropertyValue Property=""KeyProperty"" Int=""1"" />
          <PropertyValue Property=""IntegerProperty"" Int=""111"" />
          <PropertyValue Property=""StringProperty"" String=""Inline String 111"" />
        </Record>
        <Record>
          <PropertyValue Property=""KeyProperty"" Int=""2"" />
          <PropertyValue Property=""IntegerProperty"" Int=""222"" />
          <PropertyValue Property=""StringProperty"" String=""Inline String 222"" />
        </Record>
        <Record>
          <PropertyValue Property=""KeyProperty"" Int=""3"" />
          <PropertyValue Property=""IntegerProperty"" Int=""333"" />
          <PropertyValue Property=""StringProperty"" String=""Inline String 333"" />
        </Record>
      </Collection>
    </Annotation>
  </EntityType>
  <Term Name=""CollectionOfEntityTypeTerm"" Type=""Collection(NS.EntityTypeElement)"" />
  <Annotations Target=""NS.CollectionOfEntityTypeTerm"">
    <Annotation Term=""NS.CollectionOfEntityTypeTerm"">
      <Collection>
        <Record>
          <PropertyValue Property=""KeyProperty"" Int=""4"" />
          <PropertyValue Property=""IntegerProperty"" Int=""444"" />
          <PropertyValue Property=""StringProperty"" String=""Inline String 444"" />
        </Record>
        <Record>
          <PropertyValue Property=""KeyProperty"" Int=""5"" />
          <PropertyValue Property=""IntegerProperty"" Int=""555"" />
          <PropertyValue Property=""StringProperty"" String=""Inline String 555"" />
        </Record>
      </Collection>
    </Annotation>
  </Annotations>
</Schema>");
        }

        #endregion

        #region AppliesTo Attribute

        private IEdmModel TermAppliesToAttributeModel()
        {
            var model = new EdmModel();

            var inlineWithoutAppliesToIntegerTerm = new EdmTerm("NS", "InlineWithoutAppliesToIntegerTerm", EdmCoreModel.Instance.GetInt32(true));
            model.AddElement(inlineWithoutAppliesToIntegerTerm);
            var inlineWithoutAppliesToIntegerAnnotation = new EdmVocabularyAnnotation(inlineWithoutAppliesToIntegerTerm, inlineWithoutAppliesToIntegerTerm, new EdmIntegerConstant(1));
            inlineWithoutAppliesToIntegerAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(inlineWithoutAppliesToIntegerAnnotation);

            var inlineWithAppliesToIntegerTerm = new EdmTerm("NS", "InlineWithAppliesToIntegerTerm", EdmCoreModel.Instance.GetInt32(true), "Term Property");
            model.AddElement(inlineWithAppliesToIntegerTerm);
            var inlineWithAppliesToIntegerAnnotation = new EdmVocabularyAnnotation(inlineWithAppliesToIntegerTerm, inlineWithAppliesToIntegerTerm, new EdmIntegerConstant(2));
            inlineWithAppliesToIntegerAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(inlineWithAppliesToIntegerAnnotation);

            var outlineWithoutAppliesToStringTerm = new EdmTerm("NS", "OutlineWithoutAppliesToStringTerm", EdmCoreModel.Instance.GetString(true));
            model.AddElement(outlineWithoutAppliesToStringTerm);
            var outlineWithoutAppliesToStringAnnotation = new EdmVocabularyAnnotation(outlineWithoutAppliesToStringTerm, outlineWithoutAppliesToStringTerm, new EdmStringConstant("this is 3"));
            model.AddVocabularyAnnotation(outlineWithoutAppliesToStringAnnotation);

            var outlineWithAppliesToStringTerm = new EdmTerm("NS", "OutlineWithAppliesToStringTerm", EdmCoreModel.Instance.GetString(true), "Property Term");
            model.AddElement(outlineWithAppliesToStringTerm);
            var outlineWithAppliesToStringAnnotation = new EdmVocabularyAnnotation(outlineWithAppliesToStringTerm, outlineWithAppliesToStringTerm, new EdmStringConstant("this is 4"));
            model.AddVocabularyAnnotation(outlineWithAppliesToStringAnnotation);

            return model;
        }

        private IEnumerable<XElement> TermAppliesToAttributeCsdls()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""InlineWithoutAppliesToIntegerTerm"" Type=""Edm.Int32"">
    <Annotation Term=""NS.InlineWithoutAppliesToIntegerTerm"" Int=""1"" />
  </Term>
  <Term Name=""InlineWithAppliesToIntegerTerm"" Type=""Edm.Int32"" AppliesTo=""Term Property"">
    <Annotation Term=""NS.InlineWithAppliesToIntegerTerm"" Int=""2"" />
  </Term>
  <Term Name=""OutlineWithoutAppliesToStringTerm"" Type=""Edm.String"" />
  <Annotations Target=""NS.OutlineWithoutAppliesToStringTerm"">
    <Annotation Term=""NS.OutlineWithoutAppliesToStringTerm"" String=""this is 3"" />
  </Annotations>
  <Term Name=""OutlineWithAppliesToStringTerm"" Type=""Edm.String"" AppliesTo=""Property Term"" />
  <Annotations Target=""NS.OutlineWithAppliesToStringTerm"">
    <Annotation Term=""NS.OutlineWithAppliesToStringTerm"" String=""this is 4"" />
  </Annotations>
</Schema>");
        }

        #endregion
    }
}
