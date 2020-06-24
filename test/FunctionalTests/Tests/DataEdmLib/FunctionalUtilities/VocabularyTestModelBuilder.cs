//---------------------------------------------------------------------
// <copyright file="VocabularyTestModelBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using EdmLibTests.StubEdm;
    using EdmLibTests.VocabularyStubs;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.Test.OData.Utils.Metadata;

    public static class VocabularyTestModelBuilder
    {
        [CustomConsistentValidationTest]
        public static IEdmModel SimpleTerms()
        {
            StubEdmModel model = new StubEdmModel();

            var titleValueTerm = new StubTerm("NS1", "Title") { Type = EdmCoreModel.Instance.GetString(true) };
            model.Add(titleValueTerm);

            var displaySizeValueTerm = new StubTerm("NS2", "DisplaySize") { Type = EdmCoreModel.Instance.GetInt32(false) };
            model.Add(displaySizeValueTerm);

            return model;
        }

        [CustomConsistentValidationTest]
        public static IEdmModel SimpleTermWithDuplicate()
        {
            StubEdmModel model = new StubEdmModel();

            var titleValueTerm = new StubTerm("NS1", "Title") { Type = EdmCoreModel.Instance.GetString(true) };
            model.Add(titleValueTerm);

            var anotherTitleValueTerm = new StubTerm("NS1", "Title") { Type = EdmCoreModel.Instance.GetInt32(false) };
            model.Add(anotherTitleValueTerm);

            return model;
        }

        [CustomConsistentValidationTest]
        public static IEdmModel SimpleTermInV10()
        {
            StubEdmModel model = new StubEdmModel();

            var titleValueTerm = new StubTerm("NS1", "Title") { Type = EdmCoreModel.Instance.GetString(true) };
            model.Add(titleValueTerm);

            return model;
        }

        [CustomVocabularySerializerTest, CustomConsistentValidationTest]
        public static IEdmModel SimpleVocabularyAnnotation()
        {
            StubEdmModel model = new StubEdmModel();
            CreateVocabularyDefinitions(model);

            var person = new StubEdmEntityType("NS1", "Person") 
            { 
                new StubEdmStructuralProperty("Id") { Type = EdmCoreModel.Instance.GetInt16(false) },
                new StubEdmStructuralProperty("FirstName") { Type = EdmCoreModel.Instance.GetString(false) },
                new StubEdmStructuralProperty("LastName") { Type = EdmCoreModel.Instance.GetString(false) },
            };

            var titleValueTerm = model.FindTerm("NS1.Title");
            var titleValueAnnotation = new StubVocabularyAnnotation() { Term = titleValueTerm, Value = new StubStringConstantExpression("Sir") };
            person.AddVocabularyAnnotation(titleValueAnnotation);
            model.Add(person);

            return model;
        }

        [CustomVocabularySerializerTest]
        public static IEdmModel SimpleVocabularyAnnotationNonStubImplementation()
        {
            var model = new EdmModel();
            
            var titleValueTerm = new EdmTerm("NS1", "Title", EdmCoreModel.Instance.GetString(true));
            model.AddElement(titleValueTerm);

            var displaySizeValueTerm = new EdmTerm("NS1", "DisplaySize", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(displaySizeValueTerm);

            var person = new EdmEntityType("NS1", "Person"); 
            var id = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt16(false));
            var firstName = person.AddStructuralProperty("FirstName", EdmCoreModel.Instance.GetString(false));
            var lastName = person.AddStructuralProperty("LastName", EdmCoreModel.Instance.GetString(false));

            var titleValueAnnotation = new EdmVocabularyAnnotation(person, titleValueTerm, new EdmStringConstant("Sir"));
            model.AddVocabularyAnnotation(titleValueAnnotation);
            model.AddElement(person);

            return model;
        }

        [CustomVocabularySerializerTest, CustomConsistentValidationTest]
        public static IEdmModel SimpleVocabularyAnnotationWithQualifiers()
        {
            StubEdmModel model = new StubEdmModel();
            CreateVocabularyDefinitions(model);

            var person = new StubEdmEntityType("NS1", "Person");

            var titleValueTerm = model.FindTerm("NS1.Title");
            var titleValueAnnotation = new StubVocabularyAnnotation() { Term = titleValueTerm, Value = new StubStringConstantExpression("Sir"), Qualifier = "VAR1" };
            person.AddVocabularyAnnotation(titleValueAnnotation);
            var titleValueAnnotationSp = new StubVocabularyAnnotation() { Term = titleValueTerm, Value = new StubStringConstantExpression("Senor"), Qualifier = "SP" };
            person.AddVocabularyAnnotation(titleValueAnnotationSp);

            model.Add(person);

            return model;
        }

        [CustomVocabularySerializerTest, CustomConsistentValidationTest]
        public static IEdmModel SimpleVocabularyAnnotationConfict()
        {
            StubEdmModel model = new StubEdmModel();
            CreateVocabularyDefinitions(model);

            var person = new StubEdmEntityType("NS1", "Person");

            var titleValueTerm = model.FindTerm("NS1.Title");
            var titleValueAnnotation = new StubVocabularyAnnotation() { Term = titleValueTerm, Value = new StubStringConstantExpression("Sir") };
            person.AddVocabularyAnnotation(titleValueAnnotation);
            var titleValueAnnotation2 = new StubVocabularyAnnotation() { Term = titleValueTerm, Value = new StubStringConstantExpression("Senor") };
            person.AddVocabularyAnnotation(titleValueAnnotation2);

            model.Add(person);

            return model;
        }

        [CustomVocabularySerializerTest, CustomConsistentValidationTest]
        public static IEdmModel MultipleVocabularyAnnotations()
        {
            StubEdmModel model = new StubEdmModel();
            CreateVocabularyDefinitions(model);

            var person = new StubEdmEntityType("NS1", "Person") 
            { 
                new StubEdmStructuralProperty("Id") { Type = EdmCoreModel.Instance.GetInt16(false) },
                new StubEdmStructuralProperty("FirstName") { Type = EdmCoreModel.Instance.GetString(false) },
                new StubEdmStructuralProperty("LastName") { Type = EdmCoreModel.Instance.GetString(false) },
            };

            var titleValueTerm = model.FindTerm("NS1.Title");
            var titleValueAnnotation = new StubVocabularyAnnotation() { Term = titleValueTerm, Value = new StubStringConstantExpression("Sir") };
            person.AddVocabularyAnnotation(titleValueAnnotation);

            var displaySizeValueTerm = model.FindTerm("NS1.DisplaySize");
            var displaySizeValueAnnotation = new StubVocabularyAnnotation() { Term = displaySizeValueTerm, Value = new StubStringConstantExpression("1024") };
            person.AddVocabularyAnnotation(displaySizeValueAnnotation);
            model.Add(person);

            var container = new StubEdmEntityContainer("NS1", "Container");
            var personSet = new StubEdmEntitySet("PersonSet", container) { Type = new EdmCollectionType(new EdmEntityTypeReference(person, false)) };
            personSet.AddVocabularyAnnotation(titleValueAnnotation);
            container.Add(personSet);
            model.Add(container);

            return model;
        }

        [CustomConstructibleVocabularyTest, CustomConsistentValidationTest]
        public static IEdmModel SimpleVocabularyAnnotationOnContainerAndEntitySet()
        {
            StubEdmModel model = new StubEdmModel();
            CreateVocabularyDefinitions(model);

            var person = new StubEdmEntityType("NS1", "Person");
            var container = new StubEdmEntityContainer("NS1", "Container");
            var personSet = new StubEdmEntitySet("PersonSet", container) { Type = new EdmCollectionType(new EdmEntityTypeReference(person, false)) };
            container.Add(personSet);
            model.Add(container);
            model.Add(person);

            var titleValueTerm = model.FindTerm("NS1.Title");
            var titleValueAnnotation = new StubVocabularyAnnotation() { Term = titleValueTerm, Value = new StubStringConstantExpression("Sir") };

            container.AddVocabularyAnnotation(titleValueAnnotation);
            personSet.AddVocabularyAnnotation(titleValueAnnotation);

            return model;
        }

        [CustomConstructibleVocabularyTest, CustomVocabularySerializerTest, CustomConsistentValidationTest]
        public static IEdmModel StructuredVocabularyAnnotation()
        {
            StubEdmModel model = new StubEdmModel();
            CreateVocabularyDefinitions(model);

            var person = new StubEdmEntityType("NS1", "Person");
            model.Add(person);

            var reviewValueTerm = new StubTerm("NS1", "hReviewTerm");
            reviewValueTerm.Type = new StubEdmTypeReference() { Definition = model.FindType("NS1.hReviewEntity"), IsNullable = true };
            model.Add(reviewValueTerm);

            var reviewValueAnnotation = new StubVocabularyAnnotation()
            {
                Term = reviewValueTerm,
                Value = new StubStringConstantExpression("this should be Record"),
            };

            person.AddVocabularyAnnotation(reviewValueAnnotation);

            return model;
        }

        [CustomConsistentValidationTest]
        public static IEdmModel TermOfStructuredDataType()
        {
            StubEdmModel model = new StubEdmModel();

            var person = new StubEdmEntityType("NS1", "Person") 
            { 
                new StubEdmStructuralProperty("Id") { Type = EdmCoreModel.Instance.GetInt16(false) },
                new StubEdmStructuralProperty("FirstName") { Type = EdmCoreModel.Instance.GetString(false) },
                new StubEdmStructuralProperty("LastName") { Type = EdmCoreModel.Instance.GetString(false) },
            };
            model.Add(person);

            var titleValueTerm = new StubTerm("NS1", "Title") { Type = person.ToTypeReference(false) };
            model.Add(titleValueTerm);

            return model;
        }

        [CustomConsistentValidationTest]
        public static IEdmModel TypeTermWithInheritance()
        {
            StubEdmModel model = new StubEdmModel();
            var noteTypeTerm = new StubTypeTerm("NS1", "Note")
            {
                new StubEdmStructuralProperty("text") { Type = EdmCoreModel.Instance.GetString(false) },
            };
            model.Add(noteTypeTerm);
            var reviewTypeTerm = new StubTypeTerm("NS1", "Review")
            {
                new StubEdmStructuralProperty("summary") { Type = EdmCoreModel.Instance.GetString(false) },
                new StubEdmStructuralProperty("itemName") { Type = EdmCoreModel.Instance.GetString(false) },
                new StubEdmStructuralProperty("dateReviewed") { Type = EdmCoreModel.Instance.GetDateTimeOffset(false) },
                new StubEdmStructuralProperty("rating") { Type = EdmCoreModel.Instance.GetInt32(false) },
            };
            reviewTypeTerm.BaseType = noteTypeTerm;

            model.Add(reviewTypeTerm);

            return model;
        }

        [CustomConsistentValidationTest]
        public static IEdmModel TypeTermWithNavigation()
        {
            StubEdmModel model = new StubEdmModel();
            var personTypeTerm = new StubTypeTerm("NS1", "Person")
            {
                new StubEdmStructuralProperty("Name") { Type = EdmCoreModel.Instance.GetString(false) },
            };
            model.Add(personTypeTerm);

            var reviewTypeTerm = new StubTypeTerm("NS1", "Review")
            {
                new StubEdmStructuralProperty("summary") { Type = EdmCoreModel.Instance.GetString(false) },
                new StubEdmStructuralProperty("itemName") { Type = EdmCoreModel.Instance.GetString(false) },
                new StubEdmStructuralProperty("dateReviewed") { Type = EdmCoreModel.Instance.GetDateTimeOffset(false) },
                new StubEdmStructuralProperty("rating") { Type = EdmCoreModel.Instance.GetInt32(false) },
            };
            model.Add(reviewTypeTerm);

            var toPerson = new StubEdmNavigationProperty("ToPerson") { Type = new EdmEntityTypeReference(personTypeTerm, false) };
            reviewTypeTerm.Add(toPerson);

            var toReviews = new StubEdmNavigationProperty("ToReviews") {Type = new EdmCollectionType(reviewTypeTerm.ToTypeReference(false)).ToTypeReference(false)};
            personTypeTerm.Add(toReviews);

            toPerson.Partner = toReviews;
            return model;
        }

        [CustomConstructibleVocabularyTest, CustomVocabularySerializerTest, CustomConsistentValidationTest]
        public static IEdmModel VocabularyAnnotationWithRecord()
        {
            StubEdmModel model = new StubEdmModel();
            CreateVocabularyDefinitions(model);

            var person = new StubEdmEntityType("NS1", "Person");
            model.Add(person);

            var reviewValueTerm = new StubTerm("NS1", "hReviewTerm");
            reviewValueTerm.Type = new StubEdmTypeReference() { Definition = model.FindType("NS1.hReviewEntity"), IsNullable = true };
            model.Add(reviewValueTerm);

            var recordExpression = new StubRecordExpression();
            recordExpression.AddProperty("Name", new StubStringConstantExpression("Young"));
            recordExpression.AddProperty("IdType", new StubStringConstantExpression("Driver License"));
            var reviewValueAnnotation = new StubVocabularyAnnotation()
            {
                Term = reviewValueTerm,
                Value = recordExpression
            };

            person.AddVocabularyAnnotation(reviewValueAnnotation);

            return model;
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> AnnotationTermsWithNoNamespace()
        {
            var csdls = new List<XElement>();
            csdls.Add(XElement.Parse(
@"
<Schema Namespace='NS1' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <EntityType Name='Person'>
    <Property Name='Id' Type='Int16' Nullable='false' />
    <Property Name='FirstName' Type='String' Nullable='false' FixedLength='false' Unicode='true' />
    <Property Name='LastName' Type='String' Nullable='false' FixedLength='false' Unicode='true' />
  </EntityType>
  <Term Name='Title' Type='String' FixedLength='false' Unicode='true' />
  <Term Name='DisplaySize' Type='Int32' Nullable='false' />
  <Annotations Target='NS1.Person'>
    <Annotation Term='NS1.Title' String='Sir' />
  </Annotations>
</Schema>"
            ));
            csdls.Add(XElement.Parse(
@"
<Schema Namespace='Application.NS1' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <Annotations Target='NS1.Person'>
    <Annotation Term='Title' String='Sir' />
  </Annotations>
</Schema>
"
            ));

            return csdls;
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> AnnotationsWithWrongTarget()
        {
            var csdls = new List<XElement>();
            csdls.Add(XElement.Parse(
@"
<Schema Namespace='Application.NS1' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <Annotations Target='NS1.Person'>
    <Annotation Term='Foo.Title' String='Sir' />
  </Annotations>
</Schema>
"
            ));

            return csdls;
        }

        public static IEnumerable<XElement> AnnotationsInlineOnEntityContainer()
        {
            var csdls = new List<XElement>();
            csdls.Add(XElement.Parse(
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityContainer Name=""C1"">
    <Annotation Term=""foo.Age"" Qualifier=""First"" Int=""123"" />
  </EntityContainer>
</Schema>"
            ));

            return csdls;
        }

        private static void CreateVocabularyDefinitions(StubEdmModel model)
        {
            var reviewTypeTerm = new StubTypeTerm("NS1", "hReview")
            {
                new StubEdmStructuralProperty("summary") { Type = EdmCoreModel.Instance.GetString(false) },
                new StubEdmStructuralProperty("itemName") { Type = EdmCoreModel.Instance.GetString(false) },
                new StubEdmStructuralProperty("dateReviewed") { Type = EdmCoreModel.Instance.GetDateTimeOffset(false) },
                new StubEdmStructuralProperty("rating") { Type = EdmCoreModel.Instance.GetInt32(false) },
            };
            model.Add(reviewTypeTerm);

            var reviewEntityType = new StubEdmEntityType("NS1", "hReviewEntity")
            {
                new StubEdmStructuralProperty("summary") { Type = EdmCoreModel.Instance.GetString(false) },
                new StubEdmStructuralProperty("itemName") { Type = EdmCoreModel.Instance.GetString(false) },
                new StubEdmStructuralProperty("dateReviewed") { Type = EdmCoreModel.Instance.GetDateTimeOffset(false) },
                new StubEdmStructuralProperty("rating") { Type = EdmCoreModel.Instance.GetInt32(false) },
            };
            model.Add(reviewEntityType);

            var titleValueTerm = new StubTerm("NS1", "Title") { Type = EdmCoreModel.Instance.GetString(true) };
            model.Add(titleValueTerm);

            var displaySizeValueTerm = new StubTerm("NS1", "DisplaySize") { Type = EdmCoreModel.Instance.GetInt32(false) };
            model.Add(displaySizeValueTerm);
        }

        public static IEnumerable<XElement> InlineVocabularyAnnotationEntityTypeUsingAlias()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Car"">
        <Key>
            <PropertyRef Name=""OwnerId"" />
        </Key>
        <Property Name=""OwnerId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Wheels"" Type=""Edm.Int32"" Nullable=""true"" />
        <Annotation Term=""foo.StringTerm"" String=""foo"" />
    </EntityType>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""StringTerm"" Type=""Edm.String"" />
</Schema>");
        }

        public static IEnumerable<XElement> OutOfLineAnnotationEntityType()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Car"">
        <Key>
            <PropertyRef Name=""OwnerId"" />
        </Key>
        <Property Name=""OwnerId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Wheels"" Type=""Edm.Int32"" Nullable=""true"" />
    </EntityType>
    <Annotations Target=""DefaultNamespace.Car"">
        <Annotation Term=""AnnotationNamespace.StringTerm"" String=""foo"" />
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""StringTerm"" Type=""Edm.String"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
    </EntityType>
</Schema>");
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> OutOfLineAnnotationEntityProperty()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Car"">
        <Key>
            <PropertyRef Name=""OwnerId"" />
        </Key>
        <Property Name=""OwnerId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Wheels"" Type=""Edm.Int32"" Nullable=""true"" />
    </EntityType>
    <EntityType Name=""Owner"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
    </EntityType>
    <Annotations Target=""DefaultNamespace.Owner/Name"">
        <Annotation Term=""AnnotationNamespace.StringTerm"" String=""foo"" />
    </Annotations>
    <Annotations Target=""DefaultNamespace.Car/Wheels"">
        <Annotation Term=""AnnotationNamespace.PersonTerm"">
            <Record>
                <PropertyValue Property=""Id"" Int=""1"" />
                <PropertyValue Property=""Name"" String=""bar"" />
            </Record>
        </Annotation>
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""StringTerm"" Type=""Edm.String"" />
    <Term Name=""PersonTerm"" Type=""AnnotationNamespace.Person"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> InlineAnnotationEntityProperty()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Car"">
        <Key>
            <PropertyRef Name=""OwnerId"" />
        </Key>
        <Property Name=""OwnerId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Wheels"" Type=""Edm.Int32"" Nullable=""true"">
            <Annotation Term=""AnnotationNamespace.StringTerm"" String=""foo"" />
        </Property>
    </EntityType>
    <EntityType Name=""Owner"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"">
            <Annotation Term=""AnnotationNamespace.PersonTerm"">
                <Record>
                    <PropertyValue Property=""Id"" Int=""1"" />
                    <PropertyValue Property=""Name"" String=""bar"" />
                </Record>
            </Annotation>
        </Property>
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
    </EntityType>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""StringTerm"" Type=""Edm.String"" />
    <Term Name=""PersonTerm"" Type=""AnnotationNamespace.Person"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> OutOfLineAnnotationNavigationProperty()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Annotations Target=""DefaultNamespace.Car/Owner"">
    <Annotation Term=""AnnotationNamespace.PersonTerm"">
      <Record>
        <PropertyValue Int=""1"" Property=""Id"" />
        <PropertyValue Property=""Name"" String=""bar"" />
      </Record>
    </Annotation>
  </Annotations>
  <Annotations Target=""DefaultNamespace.Owner/Car"">
    <Annotation Int=""3"" Term=""AnnotationNamespace.Popularity"" />
  </Annotations>
  <EntityType Name=""Car"">
    <Key>
      <PropertyRef Name=""OwnerId"" />
    </Key>
    <NavigationProperty Name=""Owner"" Type=""DefaultNamespace.Owner"" Nullable=""false"" Partner=""Car"" />
    <Property Name=""OwnerId"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Wheels"" Nullable=""true"" Type=""Edm.Int32"" />
  </EntityType>
  <EntityType Name=""Owner"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <NavigationProperty Name=""Car"" Type=""Collection(DefaultNamespace.Car)"" Partner=""Owner"" />
    <Property Name=""Id"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Name"" Nullable=""true"" Type=""Edm.String"" />
  </EntityType>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Popularity"" Type=""Edm.Int32"" />
    <Term Name=""PersonTerm"" Type=""AnnotationNamespace.Person"" />
    <EnumType Name=""PopularityEnum"" UnderlyingType=""Edm.Int64"">
        <Member Name=""Very"" Value=""3"" />
    </EnumType>
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
    </EntityType>
</Schema>");
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> InlineAnnotationNavigationProperty()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Car"">
        <Key>
            <PropertyRef Name=""OwnerId"" />
        </Key>
        <Property Name=""OwnerId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Wheels"" Type=""Edm.Int32"" Nullable=""true"" />
        <NavigationProperty Name=""Owner"" Type=""DefaultNamespace.CarOwner"" Nullable=""false"" Partner=""Car"">
            <Annotation Term=""AnnotationNamespace.StringTerm"" String=""foo"" />
        </NavigationProperty>
    </EntityType>
    <EntityType Name=""Owner"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
        <NavigationProperty Name=""Car"" Type=""Collection(DefaultNamespace.Car)"" Partner=""Owner"">
            <Annotation Term=""AnnotationNamespace.PersonTerm"">
                <Record>
                    <PropertyValue Property=""Id"" Int=""1"" />
                    <PropertyValue Property=""Name"" String=""bar"" />
                </Record>
            </Annotation>
        </NavigationProperty>
    </EntityType>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""StringTerm"" Type=""Edm.String"" />   
    <Term Name=""PersonTerm"" Type=""AnnotationNamespace.Person"" /> 
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
    </EntityType>
</Schema>");
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> InlineAnnotationEntityContainer()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""Container"">
        <Annotation Term=""AnnotationNamespace.StringTerm"" String=""foo"" />
    </EntityContainer>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""StringTerm"" Type=""Edm.String"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> OutOfLineAnnotationEntityContainer()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""Container"">
    </EntityContainer>
    <Annotations Target=""DefaultNamespace.Container"">
        <Annotation Term=""AnnotationNamespace.ListOfPeople"">
            <Collection>
                <String>Joe</String>
                <String>Mary</String>
                <String>Justin</String>
            </Collection>
        </Annotation>
        <Annotation Term=""AnnotationNamespace.Address"">
            <Record>
                <PropertyValue Property=""Street"" String=""foo"" />
                <PropertyValue Property=""City"" String=""bar"" />
            </Record>
        </Annotation>
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""ListOfPeople"" Type=""Collection(Edm.String)"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
        <Property Name=""ListOfFriends"" Type=""Collection(Edm.String)"" />
    </EntityType>
    <EntityType Name=""PersonAddress"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Address"" Type=""AnnotationNamespace.Address"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>");
        }

        public static IEnumerable<XElement> InlineAnnotationComplexType()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Pet"">
        <Property Name=""OwnerId"" Type=""Edm.Int32"" />
        <Property Name=""Name"" Type=""Edm.String"" />
        <Annotation Term=""AnnotationNamespace.Address"">
            <Record>
                <PropertyValue Property=""Street"" String=""foo"" />
                <PropertyValue Property=""City"" String=""bar"" />
            </Record>
        </Annotation>
    </ComplexType>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""AddressObject"" Type=""AnnotationNamespace.Address"" />
    <EntityType Name=""PersonAddress"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Address"" Type=""AnnotationNamespace.Address"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>");
        }

        public static IEnumerable<XElement> OutOfLineAnnotationComplexType()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Pet"">
        <Property Name=""OwnerId"" Type=""Edm.Int32"" />
        <Property Name=""Name"" Type=""Edm.String"" />
    </ComplexType>
    <Annotations Target=""DefaultNamespace.Pet"">
        <Annotation Term=""AnnotationNamespace.ListOfPeople"">
            <Collection>
                <String>Joe</String>
                <String>Mary</String>
                <String>Justin</String>
            </Collection>
        </Annotation>
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""ListOfPeople"" Type=""Collection(Edm.String)"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
        <Property Name=""ListOfFriends"" Type=""Collection(Edm.String)"" />
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> InlineAnnotationComplexTypeProperty()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Pet"">
        <Property Name=""OwnerId"" Type=""Edm.Int32"" />
        <Property Name=""Name"" Type=""Edm.String"">
            <Annotation Term=""AnnotationNamespace.ListOfPeople"">
                <Collection>
                    <String>Joe</String>
                    <String>Mary</String>
                    <String>Justin</String>
                </Collection>
            </Annotation>
        </Property>
    </ComplexType>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""ListOfPeople"" Type=""Collection(Edm.String)"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
        <Property Name=""ListOfFriends"" Type=""Collection(Edm.String)"" />
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> OutOfLineAnnotationComplexTypeProperty()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Pet"">
        <Property Name=""OwnerId"" Type=""Edm.Int32"" />
        <Property Name=""Name"" Type=""Edm.String"" />
    </ComplexType>
    <Annotations Target=""DefaultNamespace.Pet/OwnerId"">
        <Annotation Term=""AnnotationNamespace.Address"">
            <Record>
                <PropertyValue Property=""Street"" String=""foo"" />
                <PropertyValue Property=""City"" String=""bar"" />
            </Record>
        </Annotation>
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""AddressObject"" Type=""AnnotationNamespace.Address"" />
    <EntityType Name=""PersonAddress"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Address"" Type=""AnnotationNamespace.Address"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>");
        }

        public static IEnumerable<XElement> InlineAnnotationFunction()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Function Name=""PeopleCount"">
        <Parameter Name=""PeopleList"" Type=""Collection(Edm.String)"" />
        <Annotation Term=""AnnotationNamespace.VipCount"" String=""3"" />
        <ReturnType Type=""Edm.Int32"" />
    </Function>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""VipCount"" Type=""Edm.Int32"" />
    <EntityType Name=""PersonAddress"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Address"" Type=""AnnotationNamespace.Address"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>");
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> OutOfLineAnnotationFunction()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Function Name=""PeopleCount"">
        <Parameter Name=""PeopleList"" Type=""Collection(Edm.String)"" />
        <ReturnType Type=""Edm.Int32"" />
    </Function>
    <Annotations Target=""DefaultNamespace.PeopleCount(Collection(Edm.String))"">
        <Annotation Term=""AnnotationNamespace.ListOfAge"">
            <Collection>
                <Int>51</Int>
                <Int>39</Int>
                <Int>12</Int>
            </Collection>
        </Annotation>
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""ListOfAge"" Type=""Collection(Edm.Int32)"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
        <Property Name=""ListOfFriends"" Type=""Collection(Edm.String)"" />
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> InlineAnnotationOperationParameter()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Function Name=""PeopleCount"">
        <ReturnType Type=""Edm.Int32"" />
        <Parameter Name=""PeopleList"" Type=""Collection(Edm.String)"">
            <Annotation Term=""AnnotationNamespace.VipCount"" String=""3"" />
        </Parameter>
    </Function>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""VipCount"" Type=""Edm.Int32"" />
    <EntityType Name=""PersonAddress"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Address"" Type=""AnnotationNamespace.Address"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>");
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> OutOfLineAnnotationOperationParameter()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Function Name=""PeopleCount"">
        <ReturnType Type=""Edm.Int32"" />
        <Parameter Name=""PeopleList"" Type=""Collection(Edm.String)"" />
    </Function>
    <Annotations Target=""DefaultNamespace.PeopleCount(Collection(Edm.String))/PeopleList"">
        <Annotation Term=""AnnotationNamespace.ListOfAge"">
            <Collection>
                <Int>51</Int>
                <Int>39</Int>
                <Int>12</Int>
            </Collection>
        </Annotation>
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""ListOfAge"" Type=""Collection(Edm.Int32)"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
        <Property Name=""ListOfFriends"" Type=""Collection(Edm.String)"" />
    </EntityType>
</Schema>");
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> InlineAnnotationFunctionImport()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Action Name=""MedianAge"">
      <ReturnType Type=""Edm.Int32"" />
      <Parameter Name=""PeopleAge"" Type=""Collection(Edm.Int32)"" />
    </Action>
    <EntityContainer Name=""Container"">
        <ActionImport Name=""MedianAge"" Action=""DefaultNamespace.MedianAge"">
            <Annotation Term=""AnnotationNamespace.ListOfAge"">
                <Collection>
                    <Int>51</Int>
                    <Int>39</Int>
                    <Int>12</Int>
                </Collection>
            </Annotation>
        </ActionImport>
    </EntityContainer>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""ListOfAge"" Type=""Collection(Edm.Int32)"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
        <Property Name=""ListOfFriends"" Type=""Collection(Edm.String)"" />
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> OutOfLineAnnotationFunctionImport()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Action Name=""MedianAge"">
      <ReturnType Type=""Edm.Int32"" />
      <Parameter Name=""PeopleAge"" Type=""Collection(Edm.Int32)"" />
    </Action>
    <EntityContainer Name=""Container"">
      <ActionImport Name=""MedianAge"" Action=""DefaultNamespace.MedianAge"" />
    </EntityContainer>
    <Annotations Target=""DefaultNamespace.Container/MedianAge"">
      <Annotation Term=""AnnotationNamespace.VipCount"" String=""3"" />
    </Annotations>    
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""VipCount"" Type=""Edm.Int32"" />
    <EntityType Name=""PersonAddress"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Address"" Type=""AnnotationNamespace.Address"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>");
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> InlineAnnotationFunctionImportParameter()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <Action Name=""MedianAge"">
        <ReturnType Type=""Edm.Int32"" />
        <Parameter Name=""PeopleAge"" Type=""Collection(Edm.Int32)"">
                <Annotation Term=""AnnotationNamespace.ListOfAge"">
                    <Collection>
                        <Int>51</Int>
                        <Int>39</Int>
                        <Int>12</Int>
                    </Collection>
                </Annotation>
            </Parameter>
      </Action>

    <EntityContainer Name=""Container"">
        <ActionImport Name=""MedianAge"" Action=""DefaultNamespace.MedianAge"" />
    </EntityContainer>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""ListOfAge"" Type=""Collection(Edm.Int32)"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
        <Property Name=""ListOfFriends"" Type=""Collection(Edm.String)"" />
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> OutOfLineAnnotationFunctionImportParameter()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Action Name=""MedianAge"">
      <ReturnType Type=""Edm.Int32"" />
      <Parameter Name=""PeopleAge"" Type=""Collection(Edm.Int32)"" />
    </Action>
    <EntityContainer Name=""Container"">
        <ActionImport Name=""MedianAge"" Action=""DefaultNamespace.MedianAge"" />
    </EntityContainer>
    <Annotations Target=""DefaultNamespace.MedianAge(Collection(Edm.Int32))/PeopleAge"">
        <Annotation Term=""AnnotationNamespace.VipCount"" String=""3"" />
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""VipCount"" Type=""Edm.Int32"" />
    <EntityType Name=""PersonAddress"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Address"" Type=""AnnotationNamespace.Address"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>");
        }

        public static IEnumerable<XElement> InlineAnnotationEntitySet()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""Container"">
        <EntitySet Name=""PetDog"" EntityType=""DefaultNamespace.Dog"">
            <Annotation Term=""AnnotationNamespace.Age"" Int=""22"" />
        </EntitySet>
    </EntityContainer>
    <EntityType Name=""Dog"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" />
        <Property Name=""Height"" Type=""Edm.Int32"" />
    </EntityType>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Age"" Type=""Edm.Int32"" />
    <EntityType Name=""PersonAddress"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Address"" Type=""AnnotationNamespace.Address"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>");
        }

        public static IEnumerable<XElement> OutOfLineAnnotationEntitySet()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""Container"">
        <EntitySet Name=""PetDog"" EntityType=""DefaultNamespace.Dog"" />
    </EntityContainer>
    <EntityType Name=""Dog"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" />
        <Property Name=""Height"" Type=""Edm.Int32"" />
    </EntityType>
    <Annotations Target=""DefaultNamespace.Container/PetDog"">
        <Annotation Term=""AnnotationNamespace.Address"">
            <Record>
                <PropertyValue Property=""Street"" String=""foo"" />
                <PropertyValue Property=""City"" String=""bar"" />
            </Record>
        </Annotation>
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
        <Property Name=""ListofFriendsAge"" Type=""Collection(Edm.Int32)"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>");
        }

        public static IEnumerable<XElement> InlineAnnotationEnumType()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EnumType Name=""Popularity"" UnderlyingType=""Edm.Int32"">
        <Annotation Term=""AnnotationNamespace.Age"" Int=""22"" />
    </EnumType>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Age"" Type=""Edm.Int32"" />
    <EntityType Name=""PersonAddress"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Address"" Type=""AnnotationNamespace.Address"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>");
        }

        public static IEnumerable<XElement> OutOfLineAnnotationEnumType()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EnumType Name=""Popularity"" UnderlyingType=""Edm.Int32"" />
    <Annotations Target=""DefaultNamespace.Popularity"">
        <Annotation Term=""AnnotationNamespace.Address"">
            <Record>
                <PropertyValue Property=""Street"" String=""foo"" />
                <PropertyValue Property=""City"" String=""bar"" />
            </Record>
        </Annotation>
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
        <Property Name=""ListofFriendsAge"" Type=""Collection(Edm.Int32)"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>");
        }

        public static IEnumerable<string> inlineAnnotationEnumMember()
        {
            return new List<string>{@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""EnumMemberAnnotation"" Type=""Edm.Int32"" />
    <EnumType Name=""MyEnumType"">
        <Member Name=""EnumElement1"" Value=""0"">
            <Annotation Term=""DefaultNamespace.EnumMemberAnnotation"" Int=""5"" />
        </Member>
    </EnumType>
</Schema>"};
        }

        public static IEnumerable<XElement> InlineAnnotationTerm()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""ValueTerm"" Type=""AnnotationNamespace.Address"">
        <Annotation Term=""AnnotationNamespace.Address"">
            <Record>
                <PropertyValue Property=""Street"" String=""foo"" />
                <PropertyValue Property=""City"" String=""bar"" />
            </Record>
        </Annotation>
    </Term>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
        <Property Name=""ListofFriendsAge"" Type=""Collection(Edm.Int32)"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>");
        }

        public static IEnumerable<XElement> OutOfLineAnnotationTerm()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""ValueTerm"" Type=""AnnotationNamespace.Address"" />
    <Annotations Target=""DefaultNamespace.ValueTerm"">
        <Annotation Term=""AnnotationNamespace.Age"" Int=""22"" />
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Age"" Type=""Edm.Int32"" />
    <EntityType Name=""PersonAddress"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Address"" Type=""AnnotationNamespace.Address"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>");
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> InlineAnnotationInVocabularyAnnotation()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""Container"">
        <Annotation Term=""AnnotationNamespace.Age"" Int=""22""> />
    </EntityContainer>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Age"" Type=""Edm.Int32"" />
    <EntityType Name=""PersonAddress"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Address"" Type=""AnnotationNamespace.Address"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>");
        }

        public static IEnumerable<XElement> OutOfLineAnnotationInvalidAnnotationTarget()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""Container"">
    </EntityContainer>
    <EntityType Name=""Dog"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" />
    </EntityType>
    <Annotations Target=""bar.foo"">
        <Annotation Term=""AnnotationNamespace.Age"" Int=""22"" />
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Age"" Type=""Edm.Int32"" />
    <EntityType Name=""PersonAddress"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Address"" Type=""AnnotationNamespace.Address"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>");
        }

        public static IEnumerable<XElement> OutOfLineAnnotationNamespaceAsAnnotationTarget()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""Container"">
    </EntityContainer>
    <EntityType Name=""Dog"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" />
    </EntityType>
    <Annotations Target=""DefaultNamespace"">
        <Annotation Term=""AnnotationNamespace.PersonAddress"">
            <Record>
                <PropertyValue Property=""Street"" String=""foo"" />
                <PropertyValue Property=""City"" String=""bar"" />
            </Record>
        </Annotation>
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""PersonAddress"" Type=""AnnotationNamespace.Address"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""ListofFriendsAge"" Type=""Collection(Edm.Int32)"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>");
        }

        public static IEnumerable<XElement> OutOfLineVocabularyAnnotationWithoutExpressionCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Pet"">
        <Property Name=""OwnerId"" Type=""Edm.Int32"" />
        <Property Name=""Name"" Type=""Edm.String"" />
    </ComplexType>
    <Annotations Target=""DefaultNamespace.Pet/OwnerId"">
        <Annotation Term=""AnnotationNamespace.Address"">
        </Annotation>
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""AddressObject"" Type=""AnnotationNamespace.Address"" />
    <EntityType Name=""PersonAddress"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Address"" Type=""AnnotationNamespace.Address"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>");
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> OutOfLineAnnotationAmbiguousFunction()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Function Name=""PeopleCount"">
        <ReturnType Type=""Edm.Int32"" />
        <Parameter Name=""PeopleList"" Type=""Collection(Edm.String)"" />
    </Function>
    <Function Name=""PeopleCount"">
        <ReturnType Type=""Edm.Int32"" />
        <Parameter Name=""PeopleList"" Type=""Collection(Edm.String)"" />
    </Function>
    <Annotations Target=""DefaultNamespace.PeopleCount(Collection(Edm.String))"">
        <Annotation Term=""foo.ListOfAge"">
            <Collection>
                <Int>51</Int>
                <Int>39</Int>
                <Int>12</Int>
            </Collection>
        </Annotation>
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""ListOfAge"" Type=""Collection(Edm.Int32)"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
        <Property Name=""ListOfFriends"" Type=""Collection(Edm.String)"" />
    </EntityType>
</Schema>");
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> InlineAnnotationAmbiguousFunction()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Function Name=""PeopleCount"">
        <ReturnType Type=""Edm.Int32"" />
        <Parameter Name=""PeopleList"" Type=""Collection(Edm.String)"" />
        <Annotation Term=""foo.ListOfAge"">
            <Collection>
                <Int>51</Int>
                <Int>39</Int>
                <Int>12</Int>
            </Collection>
        </Annotation>
    </Function>
    <Function Name=""PeopleCount"">
        <ReturnType Type=""Edm.Int32"" />
        <Parameter Name=""PeopleList"" Type=""Collection(Edm.String)"" />
    </Function>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""ListOfAge"" Type=""Collection(Edm.Int32)"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
        <Property Name=""ListOfFriends"" Type=""Collection(Edm.String)"" />
    </EntityType>
</Schema>");
        }


        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> OutOfLineAnnotationAmbiguousFunctionImport()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""Container"">
        <FunctionImport Name=""MedianAge"">
            <ReturnType Type=""Edm.Int32"" />
            <Parameter Name=""PeopleAge"" Type=""Collection(Edm.Int32)"" />
        </FunctionImport>
        <FunctionImport Name=""MedianAge"">
            <ReturnType Type=""Edm.Int32"" />
            <Parameter Name=""PeopleAge"" Type=""Collection(Edm.Int32)"" />
        </FunctionImport>
    </EntityContainer>
    <Annotations Target=""DefaultNamespace.Container/MedianAge(Collection(Edm.Int32))"">
        <Annotation Term=""foo.VipCount"" String=""3"" />
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""VipCount"" Type=""Edm.Int32"" />
    <EntityType Name=""PersonAddress"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Address"" Type=""AnnotationNamespace.Address"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>");
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> OutOfLineAnnotationAmbiguousFunctionImportParameter()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""Container"">
        <FunctionImport Name=""MedianAge"">
            <ReturnType Type=""Edm.Int32"" />
            <Parameter Name=""PeopleAge"" Type=""Collection(Edm.Int32)"" />
        </FunctionImport>
        <FunctionImport Name=""MedianAge"">
            <ReturnType Type=""Edm.Int32"" />
            <Parameter Name=""PeopleAge"" Type=""Collection(Edm.Int32)"" />
        </FunctionImport>
    </EntityContainer>
    <Annotations Target=""DefaultNamespace.Container/MedianAge(Collection(Edm.Int32))/PeopleAge"">
        <Annotation Term=""foo.VipCount"" String=""3"" />
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""VipCount"" Type=""Edm.Int32"" />
    <EntityType Name=""PersonAddress"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Address"" Type=""AnnotationNamespace.Address"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>");
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> OutOfLineAnnotationUnresolvedFunctionImportAndParameterTarget()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""DefaultNamespace.Container/MedianAge(Collection(Edm.Int32))/PeopleAge"">
        <Annotation Term=""foo.VipCount"" Int=""3"" />
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""VipCount"" Type=""Edm.Int32"" />
</Schema>");
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> OutOfLineAnnotationUnresolvedFunctionImportTarget()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""DefaultNamespace.Container/MedianAge(Collection(Edm.Int32))"">
        <Annotation Term=""foo.VipCount"" Int=""3"" />
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""VipCount"" Type=""Edm.Int32"" />
</Schema>");
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> OutOfLineAnnotationResolvedFunctionImportAndParameterTarget()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <Action Name=""MedianAge"">
        <ReturnType Type=""Edm.Int32"" />
        <Parameter Name=""PeopleAge"" Type=""Collection(Edm.Int32)"" />
      </Action>
    <EntityContainer Name=""Container"">
        <ActionImport Name=""MedianAge"" Action=""DefaultNamespace.MedianAge"" />
    </EntityContainer>
    <Annotations Target=""DefaultNamespace.MedianAge(Collection(Edm.Int32))/PeopleAge"">
        <Annotation Term=""foo.VipCount"" Int=""3"" />
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""VipCount"" Type=""Edm.Int32"" />
</Schema>");
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> OutOfLineAnnotationResolvedFunctionImportTarget()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <Action Name=""MedianAge"">
        <ReturnType Type=""Edm.Int32"" />
      </Action>
      <EntityContainer Name=""Container"">
        <ActionImport Name=""MedianAge"" Action=""DefaultNamespace.MedianAge"" />
      </EntityContainer>
    <Annotations Target=""DefaultNamespace.Container/MedianAge"">
        <Annotation Term=""foo.VipCount"" Int=""3"" />
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""VipCount"" Type=""Edm.Int32"" />
</Schema>");
        }

        private static IEnumerable<XElement> ConvertCsdlsToXElements(params string[] csdls)
        {
            return csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        }

        public static EdmModel InlineAnnotationSimpleModel()
        {
            EdmModel model = new EdmModel();

            EdmEntityContainer container = new EdmEntityContainer("DefaultNamespace", "Container");
            model.AddElement(container);

            EdmEntityType carType = new EdmEntityType("DefaultNamespace", "Car");
            EdmStructuralProperty carOwnerId = carType.AddStructuralProperty("OwnerId", EdmCoreModel.Instance.GetInt32(false));
            carType.AddKeys(carOwnerId);
            EdmStructuralProperty carWheels = carType.AddStructuralProperty("Wheels", EdmCoreModel.Instance.GetInt32(true));
            model.AddElement(carType);

            EdmEntityType ownerType = new EdmEntityType("DefaultNamespace", "Owner");
            EdmStructuralProperty ownerId = ownerType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            ownerType.AddKeys(ownerId);
            EdmStructuralProperty ownerName = ownerType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
            model.AddElement(ownerType);

            EdmComplexType addressType = new EdmComplexType("DefaultNamespace", "Address");
            EdmStructuralProperty addressStreet = addressType.AddStructuralProperty("Street", EdmCoreModel.Instance.GetString(false));
            EdmStructuralProperty addressStreetNumber = addressType.AddStructuralProperty("StreetNumber", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(addressType);

            EdmTerm stringTerm = new EdmTerm("AnnotationNamespace", "StringTerm", EdmCoreModel.Instance.GetString(true));
            model.AddElement(stringTerm);

            var valueAnnotation = new FunctionalTests.MutableVocabularyAnnotation()
            {
                Target = carWheels,
                Term = stringTerm,
                Value = new EdmStringConstant("foo")
            };
            valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(valueAnnotation);

            EdmEntityType personType = new EdmEntityType("AnnotationNamespace", "Person");
            EdmStructuralProperty personId = personType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            personType.AddKeys(personId);
            EdmStructuralProperty personName = personType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
            model.AddElement(personType);

            return model;
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> SimpleVocabularyAnnotationCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int32Value"" Type=""Edm.Int32"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Annotation Term=""My.NS1.Int32Value"" Qualifier=""q3"" Int=""300"" />
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""Self.Int32Value"" Qualifier=""q1"" Int=""100"" />
    <Annotation Term=""My.NS1.Int32Value"" Qualifier=""q2"" Int=""200"" />
  </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> TermNameConflictCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int32Value"" Type=""Edm.Int32"" />
  <Term Name=""Int32Value"" Type=""Edm.Int64"" />
</Schema>");
        }

        public static IEnumerable<XElement> TermNameConflictWithOthersCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int32Value"" Type=""Edm.Int32"" />
  <ComplexType Name=""Int32Value"">
    <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
</Schema>");
        }

        public static IEnumerable<XElement> TermTypeNotResolvableCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int32Value"" Type=""Self.UndefinedType"" />
</Schema>");
        }

        public static IEnumerable<XElement> VocabularyAnnotationTargetNotResolvableCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int64Value"" Type=""Int64"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </ComplexType>  
  <Annotations Target=""My.NS1.Undefined"">
    <Annotation Term=""My.NS1.Int64Value"" Qualifier=""q1.q1"" Int=""100"" />
  </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> VocabularyAnnotationTermNotResolvableCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int32Value"" Type=""Int32"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </ComplexType>  
  <Annotations Target=""My.NS1.Address"">
    <Annotation Term=""My.NS1.UndefinedTerm"" Qualifier=""q1"" Int=""144"" />
  </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> VocabularyAnnotationRecordTypeNotResolvableCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int32Value"" Type=""Int32"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </ComplexType>  
  <Annotations Target=""My.NS1.Address"">
    <Annotation Term=""My.NS1.Int32Value"" Qualifier=""q1"">
        <Record Type=""My.Ns1.UndefinedTerm"">
            <PropertyValue Property=""Unknown"" Int=""144"" />
        </Record>
    </Annotation>
  </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> VocabularyAnnotationAmbiguousSameTermNoQualiferCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int64Value"" Type=""Int64"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Annotation Term=""My.NS1.Int64Value"" Int=""300"" />
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.Int64Value"" Int=""100"" />
    <Annotation Term=""My.NS1.Int64Value"" Int=""200"" />
  </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> VocabularyAnnotationAmbiguousSameTermSameQualiferCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int64Value"" Type=""Int64"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Annotation Term=""My.NS1.Int64Value"" Qualifier=""q1.q1"" Int=""300"" />
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.Int64Value"" Qualifier=""q1.q1"" Int=""100"" />
    <Annotation Term=""My.NS1.Int64Value"" Qualifier=""q1.q1"" Int=""200"" />
  </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> InvalidPropertyVocabularyAnnotationCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""Container"">
    </EntityContainer>
    <Annotations Target=""Container"">
        <Annotation Term=""foo.HasPhoneNumber"" Int=""foo"" />
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""HasPhoneNumber"" Type=""Edm.Int32"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""ListofFriendsAge"" Type=""Collection(Edm.Int32)"" />
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> TermOnlyCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""ValueTerm"" Type=""Edm.Int32"" />
</Schema>
");
        }

        public static IEnumerable<XElement> TermWithAnnotationTargetCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Simple"" Type=""Edm.Int32"" />
    <Term Name=""Age"" Type=""Edm.Int32"" />
    <Annotations Target=""DefaultNamespace.Simple"">
        <Annotation Term=""DefaultNamespace.Age"" Int=""22"" />
    </Annotations>
</Schema>");
        }


        public static IEnumerable<XElement> VocabularyAnnotationPropertyTypeExactMatchCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int64Value"" Type=""Int64"" />
  <Term Name=""BooleanValue"" Type=""Boolean"" />
  <Term Name=""StructuredValue"" Type=""My.NS1.Address"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int64"" Nullable=""false""/>
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.Int64Value"" Int=""100"" />
    <Annotation Term=""My.NS1.BooleanValue"" Bool=""false"" />
    <Annotation Term=""My.NS1.StructuredValue"">
        <Record>
          <PropertyValue Property=""Id"" Int=""99"" />
        </Record>
    </Annotation>
  </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> VocabularyAnnotationTypeNotMatchCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false""/>
  </ComplexType>
  <Term Name=""Int64Value"" Type=""Edm.Int64"" />
  <Term Name=""StructuredValue"" Type=""Self.Address"" />
  <Annotations Target=""Self.Address"">
    <Annotation Term=""Self.StructuredValue"" Int=""100"" />
    <Annotation Term=""Self.Int64Value"" Int=""200"" />
    <Annotation Term=""Self.Int64Value"" Qualifier=""Other"">
        <Record>
          <PropertyValue Property=""Id"" Int=""99"" />
        </Record>
    </Annotation>
  </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> VocabularyAnnotationPropertyNameNotMatchCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""StructuredValue"" Type=""My.NS1.Address"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int32"" />
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.StructuredValue"">
        <Record>
          <PropertyValue Property=""Undefined"" Int=""99"" />
        </Record>
    </Annotation>
  </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> VocabularyAnnotationNullablePropertyUndeclaredCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""StructuredValue"" Type=""My.NS1.Address"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""String"" Type=""Edm.String"" Nullable=""true"" />
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.StructuredValue"">
        <Record>
          <PropertyValue Property=""Id"" Int=""99"" />
        </Record>
    </Annotation>
  </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> VocabularyAnnotationPropertyTypeNotMatchCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""StructuredValue"" Type=""My.NS1.Address"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.StructuredValue"">
        <Record>
          <PropertyValue Property=""Id"" String=""99"" />
        </Record>
    </Annotation>
  </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> VocabularyAnnotationNestedCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""StructuredValue"" Type=""My.NS1.Address"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int64"" Nullable=""false""/>
    <Property Name=""DeskPhone"" Type=""My.NS1.Phone"" />
    <Property Name=""MobilePhone"" Type=""My.NS1.Phone"" />
  </ComplexType>  
  <ComplexType Name=""Phone"">
    <Property Name=""Area"" Type=""String"" />
    <Property Name=""Extension"" Type=""String"" />
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.StructuredValue"">
        <Record>
          <PropertyValue Property=""Id"" Int=""99"" />
          <PropertyValue Property=""MobilePhone"">
            <Record>
              <PropertyValue Property=""Area"" String=""425"" />
              <PropertyValue Property=""Extension"" String=""0001234"" />
            </Record>
          </PropertyValue>
          <PropertyValue Property=""DeskPhone"">
            <Record>
              <PropertyValue Property=""Area"" String=""206"" />
              <PropertyValue Property=""Extension"" String=""0004321"" />
            </Record>
          </PropertyValue>
        </Record>
    </Annotation>
  </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> VocabularyAnnotationNestedPropertyNotMatchCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""StructuredValue"" Type=""My.NS1.Address"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int64"" Nullable=""false""/>
    <Property Name=""DeskPhone"" Type=""My.NS1.Phone"" />
    <Property Name=""MobilePhone"" Type=""My.NS1.Phone"" />
  </ComplexType>  
  <ComplexType Name=""Phone"">
    <Property Name=""Area"" Type=""String"" />
    <Property Name=""Extension"" Type=""String"" />
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.StructuredValue"">
        <Record>
          <PropertyValue Property=""Id"" Int=""99"" />
          <PropertyValue Property=""DeskPhone"" Int=""99"" />
          <PropertyValue Property=""MobilePhone"">
            <Record>
              <PropertyValue Property=""Undefined"" String=""0001234"" />
              <PropertyValue Property=""Area"" Int=""425"" />
            </Record>
          </PropertyValue>
        </Record>
    </Annotation>
  </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> VocabularyAnnotationTypeConvertibleCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""SByteValue"" Type=""SByte"" />
  <Term Name=""ByteValue"" Type=""Byte"" />
  <Term Name=""Int16Value"" Type=""Int16"" />
  <Term Name=""Int32Value"" Type=""Int32"" />
  <Term Name=""Int64Value"" Type=""Int64"" />
  <Term Name=""DoubleValue"" Type=""Double"" />
  <Term Name=""SingleValue"" Type=""Single"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int64"" Nullable=""false""/>
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.SByteValue"" Int=""-100"" />
    <Annotation Term=""My.NS1.ByteValue"" Int=""100"" />
    <Annotation Term=""My.NS1.Int16Value"" Int=""-100"" />
    <Annotation Term=""My.NS1.Int32Value"" Int=""-100"" />
    <Annotation Term=""My.NS1.Int64Value"" Int=""-100"" />
    <Annotation Term=""My.NS1.DoubleValue"" Float=""3.1416"" />
    <Annotation Term=""My.NS1.SingleValue"" Float=""3.1416"" />
  </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> VocabularyAnnotationBadValueCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""SByteValue"" Type=""SByte"" />
  <Term Name=""ByteValue"" Type=""Byte"" />
  <Term Name=""Int64Value"" Type=""Int64"" />
  <Term Name=""DoubleValue"" Type=""Double"" />
  <Term Name=""DateTimeValue"" Type=""DateTimeOffset"" />
  <Term Name=""GuidValue"" Type=""Guid"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Int64"" Nullable=""false""/>
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.SByteValue"" Int=""-129"" />
    <Annotation Term=""My.NS1.ByteValue"" Int=""256"" />

    <Annotation Term=""My.NS1.Int64Value"" Int=""Not a Number"" />
    <Annotation Term=""My.NS1.DoubleValue"" Float=""Not a Number"" />

    <Annotation Term=""My.NS1.DateTimeValue"" DateTimeOffset=""Not a Date"" />
    <Annotation Term=""My.NS1.GuidValue"" Guid=""4ae71c81-c21a"" />
  </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> VocabularyAnnotationPathCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int32Value"" Type=""Int32"" />
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>  
  <Annotations Target=""Self.Person"">
    <Annotation Term=""My.NS1.Int32Value"">
      <Path>Id</Path>
    </Annotation>
  </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> PathInAnOverloadedFunctionCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int32Value"" Type=""Int32"" />
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <Function Name=""DoStuff"" IsBound=""true"" >
    <ReturnType Type=""Edm.Int32"" />
    <Parameter Name=""Input"" Type=""Edm.Int32"" />
  </Function>
  <Function Name=""DoStuff"" IsBound=""true"">
    <Parameter Name=""Input"" Type=""Edm.Binary"" />
    <ReturnType Type=""Edm.Decimal"" />
  </Function>
  <Annotations Target=""Self.Person"">
    <Annotation Term=""My.NS1.Int32Value"">
      <Apply Function=""My.NS1.DoStuff"">
        <Path>Id</Path>
      </Apply>
    </Annotation>
  </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> ParsingPathInAnOverloadedFunctionWithAmbiguousPrimitivesCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int32Value"" Type=""Int32"" />
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <Function Name=""DoStuff"" IsBound=""true"">
    <ReturnType Type=""Edm.Int32"" />
    <Parameter Name=""Input"" Type=""Edm.Int32"" />
  </Function>
  <Function Name=""DoStuff"" IsBound=""true"">
    <ReturnType Type=""Edm.Decimal"" />
    <Parameter Name=""Input"" Type=""Edm.Int64"" />
  </Function>
  <Annotations Target=""Self.Person"">
    <Annotation Term=""My.NS1.Int32Value"">
      <Apply Function=""My.NS1.DoStuff"">
        <Path>Id</Path>
      </Apply>
    </Annotation>
  </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> ParsingPathInAnOverloadedFunctionWithUnresolvedPathCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int32Value"" Type=""Int32"" />
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""MyComplex"" Type=""My.NS1.Complex"" Nullable=""false""/>
  </EntityType>
  <ComplexType Name=""Complex"">
    <Property Name=""prop"" Type=""Int32"" Nullable=""false""/>
  </ComplexType>
  <Function Name=""DoStuff"" IsBound=""true"">
    <Parameter Name=""Input"" Type=""Edm.Int32"" />
    <ReturnType Type=""Edm.Int32"" />
  </Function>
  <Function Name=""DoStuff"" IsBound=""true"">
    <Parameter Name=""Input"" Type=""Edm.Int64"" />
    <ReturnType Type=""Edm.Decimal"" />
  </Function>
  <Annotations Target=""Self.Person"">
    <Annotation Term=""My.NS1.Int32Value"">
      <Apply Function=""My.NS1.DoStuff"">
        <Path>MyComplex/NonExistant</Path>
      </Apply>
    </Annotation>
  </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> VocabularyAnnotationPathNotValidCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Int32Value"" Type=""Int32"" />
  <EntityType Name=""Person"">
    <Key><PropertyRef Name=""Id"" /></Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>  
  <Annotations Target=""Self.Person"">
    <Annotation Term=""My.NS1.Int32Value"">
      <Path>Undefined</Path>
    </Annotation>
  </Annotations>
</Schema>");
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> VocabularyAnnotationIfCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""StringValue"" Type=""String"" />
  <EntityType Name=""Person"">
    <Key><PropertyRef Name=""Id"" /></Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>  
  <Annotations Target=""Self.Person"">
    <Annotation Term=""My.NS1.StringValue"">
      <If>
          <IsType Type=""Int32"">
              <Path>Id</Path>
          </IsType>
          <String>100</String>
          <String>200</String>
      </If>
    </Annotation>
  </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> VocabularyAnnotationIfTypeNotMatchCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""StringValue"" Type=""String"" />
  <EntityType Name=""Person"">
    <Key><PropertyRef Name=""Id"" /></Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>  
  <Annotations Target=""Self.Person"">
    <Annotation Term=""My.NS1.StringValue"">
      <If>
        <Bool>false</Bool>
        <String>100</String>
        <Int>200</Int>
      </If>
    </Annotation>
  </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> VocabularyAnnotationIfTypeNotResolvedCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""StringValue"" Type=""String"" />
  <EntityType Name=""Person"">
    <Key><PropertyRef Name=""Id"" /></Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>  
  <Annotations Target=""Self.Person"">
    <Annotation Term=""My.NS1.StringValue"">
      <If>
        <IsType Type=""My.NS1.Undefined"">
          <Path>Id</Path>
        </IsType>
        <String>100</String>
        <Int>200</Int>
      </If>
    </Annotation>
  </Annotations>
</Schema>");
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> VocabularyAnnotationFunctionCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""StringValue"" Type=""String"" />
  <EntityType Name=""Person"">
    <Key><PropertyRef Name=""Id"" /></Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <Function Name=""StringConcat"">
    <ReturnType Type=""String""/>
    <Parameter Name=""String1"" Type=""String"" />
    <Parameter Name=""String2"" Type=""String"" />
  </Function>  
  <Annotations Target=""Self.Person"">
    <Annotation Term=""My.NS1.StringValue"">
       <Apply Function=""Self.StringConcat"">
         <LabeledElement Name=""FunctionParameter""><String>-</String></LabeledElement>
         <String>100</String>
       </Apply>
    </Annotation>
  </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> VocabularyAnnotationFunctionTypeNotMatchCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""BooleanValue"" Type=""Boolean"" />
  <Term Name=""StringValue"" Type=""String"" />
  <EntityType Name=""Person"">
    <Key><PropertyRef Name=""Id"" /></Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <Function Name=""StringConcat"">
    <Parameter Name=""String1"" Type=""String"" />
    <Parameter Name=""String2"" Type=""String"" />
    <ReturnType Type=""String"" />
  </Function>  
  <Annotations Target=""Self.Person"">
    <Annotation Term=""My.NS1.BooleanValue"">
       <Apply Function=""Self.StringConcat"">
         <String>-</String>
         <Int>100</Int>
       </Apply>
    </Annotation>
  </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> VocabularyAnnotationNullablePropertyWithNullExpressionCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""StructuredValue"" Type=""My.NS1.Address"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""String"" Type=""Edm.String"" Nullable=""true"" />
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.StructuredValue"">
        <Record>
          <PropertyValue Property=""Id"" Int=""99"" />
          <PropertyValue Property=""String"">
            <Null />
          </PropertyValue>
        </Record>
    </Annotation>
  </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> VocabularyAnnotationPropertyWithNullExpressionCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""StructuredValue"" Type=""My.NS1.Address"" />
  <ComplexType Name=""Address"">
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""String"" Type=""Edm.String"" Nullable=""true"" />
  </ComplexType>  
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.StructuredValue"">
        <Record>
          <PropertyValue Property=""Id"" >
            <Null />
          </PropertyValue>
          <PropertyValue Property=""String"">
            <Null />
          </PropertyValue>
        </Record>
    </Annotation>
  </Annotations>
</Schema>");
        }

        public static EdmModel AnnotationsOnlyWithNoNamespace()
        {
            var model = new EdmModel();

            var annotation = new EdmVocabularyAnnotation(
                new EdmEntityType("Self", "Address"),
                new EdmTerm("My.NS1", "StructuredValue", new EdmEntityTypeReference(new EdmEntityType("Foo", "Bar"), false)),
                new EdmRecordExpression(new []{new EdmPropertyConstructor("Id", new EdmIntegerConstant(99)), new EdmPropertyConstructor("String", new EdmStringConstant("BorkBorkBork"))}));

            model.AddVocabularyAnnotation(annotation);
            return model;
        }

        public static IEnumerable<XElement> AnnotationsOnlyWithNoNamespaceCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Annotations Target=""Self.Address"">
    <Annotation Term=""My.NS1.StructuredValue"">
        <Record>
          <PropertyValue Property=""Id"" Int=""99"" />
          <PropertyValue Property=""String"" String=""BorkBorkBork"" />
        </Record>
    </Annotation>
  </Annotations>
</Schema>");
        }

        public static EdmModel SimpleModel()
        {
            var model = new EdmModel();

            var complexType = new EdmComplexType("Foo", "SimpleType");
            var stringValue = complexType.AddStructuralProperty("StringValue", EdmCoreModel.Instance.GetString(true));
            model.AddElement(complexType);

            var entityType = new EdmEntityType("Foo", "SimpleEntity");
            var intValue = entityType.AddStructuralProperty("Int32Value", EdmCoreModel.Instance.GetInt32(false));
            entityType.AddKeys(intValue);
            model.AddElement(entityType);

            var simpleTerm = new EdmTerm("Foo", "SimpleTerm", EdmCoreModel.Instance.GetInt32(true));
            model.AddElement(simpleTerm);

            var complexTerm = new EdmTerm("Foo", "ComplexTerm", new EdmComplexTypeReference(complexType, true));
            model.AddElement(complexTerm);

            var entityTerm = new EdmTerm("Foo", "EntityTerm", new EdmEntityTypeReference(entityType, true));
            model.AddElement(entityTerm);

            return model;
        }

        public static EdmModel DefaultVocabularyAnnotationWithTargetOnlyModel()
        {
            EdmModel model = VocabularyTestModelBuilder.SimpleModel();

            var valueAnnotation = new FunctionalTests.MutableVocabularyAnnotation()
            {
                Target = new EdmEntityType("", "")
            };
            model.AddVocabularyAnnotation(valueAnnotation);

            return model;
        }

        public static EdmModel SimpleVocabularyAnnotationModel()
        {
            EdmModel model = VocabularyTestModelBuilder.SimpleModel();
            var simpleTerm = model.FindTerm("Foo.SimpleTerm");
            var complexTerm = model.FindTerm("Foo.ComplexTerm");

            EdmVocabularyAnnotation inlineValueAnnotation = new EdmVocabularyAnnotation(
                simpleTerm,
                simpleTerm,
                new EdmIntegerConstant(1));
            inlineValueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(inlineValueAnnotation);

            EdmVocabularyAnnotation outOfLineValueAnnotation = new EdmVocabularyAnnotation(
                complexTerm,
                simpleTerm,
                new EdmIntegerConstant(2));
            outOfLineValueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.AddVocabularyAnnotation(outOfLineValueAnnotation);

            return model;
        }

        [CustomConsistentValidationTest]
        public static EdmModel AnnotationWithInvalidTargetModel(EdmVocabularyAnnotationSerializationLocation location)
        {
            EdmModel model = VocabularyTestModelBuilder.SimpleModel();
            var simpleTerm = model.FindTerm("Foo.SimpleTerm");
            var entityTerm = model.FindTerm("Foo.EntityTerm");
            var entityType = model.FindType("Foo.SimpleEntity") as EdmEntityType;
            var entityTypeProperty = entityType.FindProperty("Int32Value");

            var invalidType = new EdmComplexType("Foo", "InvalidType");
            var invalidTypeProperty = invalidType.AddStructuralProperty("InvalidValue", EdmCoreModel.Instance.GetString(true));

            var invalidTerm = new EdmTerm("Foo", "InvalidTerm", EdmCoreModel.Instance.GetString(false));

            var invalidEntitySet = new EdmEntitySet(new EdmEntityContainer("", ""), "InvalidEntitySet", entityType);

            var invalidFunction = new EdmFunction("Foo", "InvalidFunction", EdmCoreModel.Instance.GetInt32(true));
            var invalidFunctionParameter = invalidFunction.AddParameter("InvalidParameter", EdmCoreModel.Instance.GetInt32(true));

            model.AddElement(invalidFunction);
            var invalidFunctionImport = new EdmFunctionImport(new EdmEntityContainer("", ""), "InvalidFunctionImport", invalidFunction);

            EdmVocabularyAnnotation valueAnnotationOnContainer = new EdmVocabularyAnnotation(
                new EdmEntityContainer("", ""),
                simpleTerm,
                new EdmIntegerConstant(1));
            valueAnnotationOnContainer.SetSerializationLocation(model, location);
            model.AddVocabularyAnnotation(valueAnnotationOnContainer);

            EdmVocabularyAnnotation valueAnnotationOnEntitySet = new EdmVocabularyAnnotation(
                invalidEntitySet,
                simpleTerm,
                new EdmIntegerConstant(1));
            valueAnnotationOnEntitySet.SetSerializationLocation(model, location);
            model.AddVocabularyAnnotation(valueAnnotationOnEntitySet);

            EdmVocabularyAnnotation valueAnnotationOnType = new EdmVocabularyAnnotation(
                invalidType,
                simpleTerm,
                new EdmIntegerConstant(1));
            valueAnnotationOnType.SetSerializationLocation(model, location);
            model.AddVocabularyAnnotation(valueAnnotationOnType);

            EdmVocabularyAnnotation valueAnnotationOnProperty = new EdmVocabularyAnnotation(
                invalidTypeProperty,
                simpleTerm,
                new EdmIntegerConstant(1));
            valueAnnotationOnProperty.SetSerializationLocation(model, location);
            model.AddVocabularyAnnotation(valueAnnotationOnProperty);

            EdmVocabularyAnnotation valueAnnotationOnTerm = new EdmVocabularyAnnotation(
                invalidTerm,
                simpleTerm,
                new EdmIntegerConstant(1));
            valueAnnotationOnTerm.SetSerializationLocation(model, location);
            model.AddVocabularyAnnotation(valueAnnotationOnTerm);

            EdmVocabularyAnnotation valueAnnotationOnFunction = new EdmVocabularyAnnotation(
                invalidFunction,
                simpleTerm,
                new EdmIntegerConstant(1));
            valueAnnotationOnFunction.SetSerializationLocation(model, location);
            model.AddVocabularyAnnotation(valueAnnotationOnFunction);

            EdmVocabularyAnnotation valueAnnotationOnParameter = new EdmVocabularyAnnotation(
                invalidFunctionParameter,
                simpleTerm,
                new EdmIntegerConstant(1));
            valueAnnotationOnParameter.SetSerializationLocation(model, location);
            model.AddVocabularyAnnotation(valueAnnotationOnParameter);

            EdmVocabularyAnnotation valueAnnotationOnFunctionImport = new EdmVocabularyAnnotation(
                invalidFunctionImport,
                simpleTerm,
                new EdmIntegerConstant(1));
            valueAnnotationOnFunctionImport.SetSerializationLocation(model, location);
            model.AddVocabularyAnnotation(valueAnnotationOnFunctionImport);

            EdmVocabularyAnnotation valueAnnotation = new EdmVocabularyAnnotation(
                new EdmEntityContainer("", ""),
                entityTerm,
                new EdmRecordExpression(new EdmPropertyConstructor(entityTypeProperty.Name, new EdmIntegerConstant(1))));
            valueAnnotation.SetSerializationLocation(model, location);
            model.AddVocabularyAnnotation(valueAnnotation);

            return model;
        }

        [CustomConsistentValidationTest]
        public static EdmModel AnnotationWithInvalidTermModel()
        {
            EdmModel model = VocabularyTestModelBuilder.SimpleModel();
            var simpleTerm = model.FindDeclaredTerm("Foo.SimpleTerm");
            var entityType = model.FindDeclaredType("Foo.SimpleEntity") as EdmEntityType;
            var entityId = entityType.FindProperty("Int32Value");

            EdmVocabularyAnnotation inlineValueAnnotation = new EdmVocabularyAnnotation(
                simpleTerm,
                new EdmTerm("Bar", "Foo", EdmCoreModel.Instance.GetInt32(false)),
                new EdmIntegerConstant(1));
            inlineValueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(inlineValueAnnotation);

            return model;
        }

        public static EdmModel DuplicateVocabularyAnnotationModel()
        {
            EdmModel model = VocabularyTestModelBuilder.SimpleModel();
            var simpleTerm = model.FindTerm("Foo.SimpleTerm");

            EdmVocabularyAnnotation inlineValueAnnotation = new EdmVocabularyAnnotation(
                simpleTerm,
                simpleTerm,
                new EdmIntegerConstant(1));
            inlineValueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(inlineValueAnnotation);

            inlineValueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.AddVocabularyAnnotation(inlineValueAnnotation);

            return model;
        }

        public static EdmModel DuplicateVocabularyAnnotationWithQualifierModel()
        {
            EdmModel model = VocabularyTestModelBuilder.SimpleModel();
            var simpleTerm = model.FindTerm("Foo.SimpleTerm");

            EdmVocabularyAnnotation inlineValueAnnotation = new EdmVocabularyAnnotation(
                simpleTerm,
                simpleTerm,
                "q1",
                new EdmIntegerConstant(1));
            inlineValueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(inlineValueAnnotation);

            inlineValueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.AddVocabularyAnnotation(inlineValueAnnotation);

            return model;
        }

        public static EdmModel VocabularyAnnotationWithQualifierModel()
        {
            EdmModel model = VocabularyTestModelBuilder.SimpleModel();
            var simpleTerm = model.FindTerm("Foo.SimpleTerm");
            var complexTerm = model.FindTerm("Foo.ComplexTerm");

            EdmVocabularyAnnotation inlineValueAnnotation = new EdmVocabularyAnnotation(
                complexTerm,
                simpleTerm,
                "q1",
                new EdmIntegerConstant(1));
            inlineValueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(inlineValueAnnotation);

            EdmVocabularyAnnotation inlineValueAnnotation2 = new EdmVocabularyAnnotation(
                complexTerm,
                simpleTerm,
                "q2",
                new EdmIntegerConstant(1));
            inlineValueAnnotation2.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.AddVocabularyAnnotation(inlineValueAnnotation2);

            return model;
        }

        public static IEnumerable<XElement> AnnotationQualifiersWithNonSimpleValue()
        {
            return ConvertCsdlsToXElements(
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""foo.Note"" Qualifier=""3"" Int=""99"" />
        <Annotation Term=""foo.Note"" Qualifier=""ReallyOddQualifer1234567890!@#$%^*()_+&amp;"" Int=""127"" />
    </Annotations>
    <Annotations Target=""foo.Person"" Qualifier=""foo+bar"">
        <Annotation Term=""foo.Note"" Int=""127"" />
    </Annotations>
    <Term Name=""Note"" Type=""Edm.Int32"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" />
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> SimpleVocabularyAnnotationWithComplexTypeCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""ßÆœÇèÒöæ"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PersonInfo"" Type=""ßÆœÇèÒöæ.Person"" />
  <ComplexType Name=""Person"">
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Address"" Type=""ßÆœÇèÒöæ.नुसौस्वागूूम"" Nullable=""true"" />
    <Property Name=""öøãçšŰŽ"" Type=""Collection(String)"" Nullable=""true"" />
  </ComplexType>  
  <ComplexType Name=""नुसौस्वागूूम"">
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
  </ComplexType>  
  <Annotations Target=""ßÆœÇèÒöæ.PersonInfo"">
    <Annotation Term=""ßÆœÇèÒöæ.PersonInfo"">
        <Record>
            <PropertyValue Property=""Id"" Int=""7"" />
            <PropertyValue Property=""Address"">
                <Record>
                    <PropertyValue Property=""Name"" String=""Microsoft Way"" />
                </Record>
            </PropertyValue>
            <PropertyValue Property=""öøãçšŰŽ"">
                <Collection>
                    <String>伯唯堯帯作停捜桜噂構申表ｱｲｳ￥￥</String>
                    <String>bar</String>
                </Collection>
            </PropertyValue>
        </Record>
    </Annotation>
  </Annotations>
</Schema>");
        }

        [CustomVocabularySerializerTestAttribute]
        public static IEdmModel SimpleVocabularyAnnotationWithComplexTypeModel()
        {
            var model = new EdmModel();

            var address = new EdmComplexType("ßÆœÇèÒöæ", "नुसौस्वागूूम");
            address.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
            model.AddElement(address);

            var person = new EdmComplexType("ßÆœÇèÒöæ", "Person");
            person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddStructuralProperty("Address", new EdmComplexTypeReference(address, true));
            person.AddStructuralProperty("öøãçšŰŽ", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
            model.AddElement(person);

            var personInfoTerm = new EdmTerm("ßÆœÇèÒöæ", "PersonInfo", new EdmComplexTypeReference(person, true));
            model.AddElement(personInfoTerm);

            var addressRecord = new EdmRecordExpression(new EdmPropertyConstructor("Name", new EdmStringConstant("Microsoft Way")));

            var friendNamesRecord = new EdmCollectionExpression(new EdmStringConstant("伯唯堯帯作停捜桜噂構申表ｱｲｳ￥￥"), new EdmStringConstant("bar"));

            var valueAnnotationRecord = new EdmRecordExpression(
                new EdmPropertyConstructor("Id", new EdmIntegerConstant(7)),
                new EdmPropertyConstructor("Address", addressRecord),
                new EdmPropertyConstructor("öøãçšŰŽ", friendNamesRecord));

            var valueAnnotation = new EdmVocabularyAnnotation(
                personInfoTerm,
                personInfoTerm,
                valueAnnotationRecord);

            valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.AddVocabularyAnnotation(valueAnnotation);

            return model;
        }

        public static IEnumerable<XElement> VocabularyAnnotationComplexTypeWithNullValuesCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PersonInfo"" Type=""NS.Person"" />
  <ComplexType Name=""Person"">
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""true"" />
    <Property Name=""Address"" Type=""NS.Address"" Nullable=""true"" />
    <Property Name=""FriendNames"" Type=""Collection(String)"" Nullable=""true"" />
  </ComplexType>  
  <ComplexType Name=""Address"">
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
  </ComplexType>  
  <Annotations Target=""NS.PersonInfo"">
    <Annotation Term=""NS.PersonInfo"">
        <Record>
            <PropertyValue Property=""Id"">
                <Null />
            </PropertyValue>
            <PropertyValue Property=""Address"">
                <Null />
            </PropertyValue>
            <PropertyValue Property=""FriendNames"">
                <Collection>
                    <Null />
                </Collection>
            </PropertyValue>
        </Record>
    </Annotation>
  </Annotations>
</Schema>");
        }

        [CustomVocabularySerializerTestAttribute]
        public static IEdmModel VocabularyAnnotationComplexTypeWithNullValuesModel()
        {
            var model = new EdmModel();

            var address = new EdmComplexType("NS", "Address");
            address.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
            model.AddElement(address);

            var person = new EdmComplexType("NS", "Person");
            person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(true));
            person.AddStructuralProperty("Address", new EdmComplexTypeReference(address, true));
            person.AddStructuralProperty("FriendNames", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
            model.AddElement(person);

            var personInfoTerm = new EdmTerm("NS", "PersonInfo", new EdmComplexTypeReference(person, true));
            model.AddElement(personInfoTerm);

            var friendNamesRecord = new EdmCollectionExpression(EdmNullExpression.Instance);

            var valueAnnotationRecord = new EdmRecordExpression(
                new EdmPropertyConstructor("Id", EdmNullExpression.Instance),
                new EdmPropertyConstructor("Address", EdmNullExpression.Instance),
                new EdmPropertyConstructor("FriendNames", friendNamesRecord));

            var valueAnnotation = new EdmVocabularyAnnotation(
                personInfoTerm,
                personInfoTerm,
                valueAnnotationRecord);

            valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.AddVocabularyAnnotation(valueAnnotation);
            return model;
        }

        public static IEnumerable<XElement> VocabularyAnnotationComplexTypeWithFewerPropertiesCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PersonInfo"" Type=""NS.Person"" />
  <ComplexType Name=""Person"">
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Address"" Type=""NS.Address"" Nullable=""true"" />
    <Property Name=""FriendNames"" Type=""Collection(String)"" Nullable=""true"" />
  </ComplexType>  
  <ComplexType Name=""Address"">
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
  </ComplexType>  
  <Annotations Target=""NS.PersonInfo"">
    <Annotation Term=""NS.PersonInfo"">
        <Record>
            <PropertyValue Property=""Id"" Int=""7"" />
            <PropertyValue Property=""Address"">
                <Record>
                    <PropertyValue Property=""Name"" String=""Microsoft Way"" />
                </Record>
            </PropertyValue>
        </Record>
    </Annotation>
  </Annotations>
</Schema>");
        }

        [CustomVocabularySerializerTestAttribute]
        public static IEdmModel VocabularyAnnotationComplexTypeWithFewerPropertiesModel()
        {
            var model = new EdmModel();

            var address = new EdmComplexType("NS", "Address");
            address.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
            model.AddElement(address);

            var person = new EdmComplexType("NS", "Person");
            person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddStructuralProperty("Address", new EdmComplexTypeReference(address, true));
            person.AddStructuralProperty("FriendNames", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
            model.AddElement(person);

            var personInfoTerm = new EdmTerm("NS", "PersonInfo", new EdmComplexTypeReference(person, true));
            model.AddElement(personInfoTerm);

            var addressRecord = new EdmRecordExpression(new EdmPropertyConstructor("Name", new EdmStringConstant("Microsoft Way")));

            var valueAnnotationRecord = new EdmRecordExpression(
                new EdmPropertyConstructor("Id", new EdmIntegerConstant(7)),
                new EdmPropertyConstructor("Address", addressRecord));

            var valueAnnotation = new EdmVocabularyAnnotation(
                personInfoTerm,
                personInfoTerm,
                valueAnnotationRecord);

            valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.AddVocabularyAnnotation(valueAnnotation);

            return model;
        }

        public static IEnumerable<XElement> VocabularyAnnotationWithCollectionComplexTypeCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""FriendNames"" Type=""Collection(NS.Person)"" />
    <ComplexType Name=""Person"">
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>  
    <Annotations Target=""NS.Person"">
        <Annotation Term=""NS.FriendNames"">
            <Collection>
                <Record>
                    <PropertyValue Property=""Name"" String=""Bill Gates"" />
                </Record>
                <Record>
                    <PropertyValue Property=""Name"" String=""Steve B"" />
                </Record>
            </Collection>
        </Annotation>
    </Annotations>
</Schema>");
        }

        [CustomVocabularySerializerTestAttribute]
        public static IEdmModel VocabularyAnnotationWithCollectionComplexTypeModel()
        {
            var model = new EdmModel();

            var person = new EdmComplexType("NS", "Person");
            var name = person.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
            model.AddElement(person);

            var friendNames = new EdmTerm("NS", "FriendNames", EdmCoreModel.GetCollection(new EdmComplexTypeReference(person, true)));
            model.AddElement(friendNames);

            var billGatesRecord = new EdmRecordExpression(new EdmPropertyConstructor("Name", new EdmStringConstant("Bill Gates")));
            var steveBRecord = new EdmRecordExpression(new EdmPropertyConstructor("Name", new EdmStringConstant("Steve B")));
            var annotationValue = new EdmCollectionExpression(billGatesRecord, steveBRecord);

            var valueAnnotation = new EdmVocabularyAnnotation(
                person,
                friendNames,
                annotationValue);
            model.AddVocabularyAnnotation(valueAnnotation);

            return model;
        }

        public static IEnumerable<XElement> VocabularyAnnotationNonNullablePropertyWithNullValueCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""FriendNames"" Type=""NS.Person"" />
    <ComplexType Name=""Person"">
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
    </ComplexType>  
    <Annotations Target=""NS.Person"">
        <Annotation Term=""NS.FriendNames"">
            <Record>
                <PropertyValue Property=""Name"">
                    <Null />
                </PropertyValue>
            </Record>
        </Annotation>
    </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> AnnotationComplexTypeWithMissingOrExtraPropertiesCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""PersonInfo"" Type=""NS.Person"" />
    <Term Name=""CollectionOfPersonInfo"" Type=""Collection(NS.Person)"" />
    <ComplexType Name=""Person"">
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""true"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
    <Annotations Target=""NS.PersonInfo"">
        <Annotation Term=""NS.PersonInfo"">
            <Record>
                <PropertyValue Property=""Id"" Int=""1"" />
                <PropertyValue Property=""ExtraProperty"" String=""123"" />
            </Record>
        </Annotation>
        <Annotation Term=""NS.CollectionOfPersonInfo"">
            <Collection>
                <Record>
                    <PropertyValue Property=""Id"" Int=""1"" />
                </Record>
                <Record>
                    <PropertyValue Property=""ExtraProperty"" String=""123"" />
                </Record>
            </Collection>
        </Annotation>
    </Annotations>
</Schema>");
        }

        public static IEnumerable<XElement> AnnotationEntityTypeWithMissingOrExtraPropertiesCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""PersonInfo"" Type=""NS.Person"" />
    <Term Name=""CollectionOfPersonInfo"" Type=""Collection(NS.Person)"" />
    <Term Name=""OpenPersonInfo"" Type=""NS.OpenPerson"" />
    <Term Name=""CollectionOfOpenPersonInfo"" Type=""Collection(NS.OpenPerson)"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
    </EntityType>
    <EntityType Name=""OpenPerson"" OpenType=""true"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" />
    </EntityType>
    <Annotations Target=""NS.PersonInfo"">
        <Annotation Term=""NS.PersonInfo"">
            <Record>
                <PropertyValue Property=""Id"" Int=""1"" />
                <PropertyValue Property=""ExtraProperty"" String=""123"" />
            </Record>
        </Annotation>
        <Annotation Term=""NS.CollectionOfPersonInfo"">
            <Collection>
                <Record>
                    <PropertyValue Property=""Id"" Int=""1"" />
                </Record>
                <Record>
                    <PropertyValue Property=""Id"" Int=""1"" />
                    <PropertyValue Property=""ExtraProperty"" String=""123"" />
                </Record>
            </Collection>
        </Annotation>
    </Annotations>
    <Annotations Target=""NS.OpenPersonInfo"">
        <Annotation Term=""NS.OpenPersonInfo"">
            <Record>
                <PropertyValue Property=""Id"" Int=""1"" />
                <PropertyValue Property=""ExtraProperty"" String=""123"" />
            </Record>
        </Annotation>
        <Annotation Term=""NS.CollectionOfOpenPersonInfo"">
            <Collection>
                <Record>
                    <PropertyValue Property=""Id"" Int=""1"" />
                </Record>
                <Record>
                    <PropertyValue Property=""Id"" Int=""1"" />
                    <PropertyValue Property=""ExtraProperty"" String=""123"" />
                </Record>
            </Collection>
        </Annotation>
    </Annotations>
</Schema>");
        }
    }
}

