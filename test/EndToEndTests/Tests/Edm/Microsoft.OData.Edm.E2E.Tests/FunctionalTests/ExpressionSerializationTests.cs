//---------------------------------------------------------------------
// <copyright file="ExpressionSerializationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class ExpressionSerializationTests : EdmLibTestCaseBase
{
    private IEdmModel longDefinitionModel;
    private IEdmModel shortDefinitionModel;
    private IEdmModel baseModel;
    private IEdmModel functionsModel;

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

    [Fact]
    public void Serialize_ConstantTermOnEntityType_GeneratesExpectedCsdl()
    {
        this.SetupModels();

        var person = this.baseModel.FindType("NS1.Person") as IEdmEntityType;
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

        IEnumerable<XElement> expectedXElements = new[] { expectedCsdl }.Select(s => XElement.Load(new StringReader(s)));
        IEnumerable<XElement> actualXElements = this.GetSerializerResult(this.baseModel).Select(s => XElement.Load(new StringReader(s)));

        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Fact]
    public void Serialize_ConstantTermOnEntityTypeWithInlineAnnotation_GeneratesExpectedCsdl()
    {
        this.SetupModels();

        var person = this.baseModel.FindType("NS1.Person") as IEdmEntityType;
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

        IEnumerable<XElement> expectedXElements = new[] { expectedCsdlInline }.Select(s => XElement.Load(new StringReader(s)));
        IEnumerable<XElement> actualXElements = this.GetSerializerResult(this.baseModel).Select(s => XElement.Load(new StringReader(s)));
        
        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    // [EdmLib] Serializing of vocabulary annotations output incorrect <Using>s
    [Fact]
    public void Serialize_ConstantTermWithDifferentUriOnEntityType_GeneratesExpectedCsdl()
    {
        this.SetupModels();
        var person = this.baseModel.FindType("NS1.Person") as IEdmEntityType;

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
        
        IEnumerable<XElement> expectedXElements = new[] { expectedCsdl }.Select(s => XElement.Load(new StringReader(s)));
        IEnumerable<XElement> actualXElements = this.GetSerializerResult(this.baseModel).Select(s => XElement.Load(new StringReader(s)));
        
        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Fact]
    public void Serialize_ConstantTermWithCustomAliasOnEntityType_GeneratesExpectedCsdl()
    {
        this.SetupModels();
        var person = this.baseModel.FindType("NS1.Person") as IEdmEntityType;

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
        
        IEnumerable<XElement> expectedXElements = new[] { expectedCsdl }.Select(s => XElement.Load(new StringReader(s)));
        IEnumerable<XElement> actualXElements = this.GetSerializerResult(this.baseModel).Select(s => XElement.Load(new StringReader(s)));
        
        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Fact]
    public void Serialize_ConstantTermOnEntityTypeWithSeparateApplication_GeneratesExpectedCsdl()
    {
        this.SetupModels();
        var person = this.baseModel.FindType("NS1.Person") as IEdmEntityType;

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

        IEnumerable<XElement> expectedXElements = new[] { expectedCsdl1, expectedCsdl2 }.Select(s => XElement.Load(new StringReader(s)));
        IEnumerable<XElement> actualXElements = this.GetSerializerResult(this.baseModel).Select(s => XElement.Load(new StringReader(s)));
        
        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Fact]
    public void Serialize_AllConstantTypesOnProperty_GeneratesExpectedCsdl()
    {
        this.SetupModels();
        var nameProperty = (this.baseModel.FindType("NS1.Person") as IEdmEntityType)?.FindProperty("Name");
        var birthdayProperty = (this.baseModel.FindType("NS1.Person") as IEdmEntityType)?.FindProperty("Birthday");

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


        IEnumerable<XElement> expectedXElements = new[] { expectedCsdl }.Select(s => XElement.Load(new StringReader(s)));
        IEnumerable<XElement> actualXElements = this.GetSerializerResult(this.baseModel).Select(s => XElement.Load(new StringReader(s)));
        
        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Fact]
    public void Serialize_FunctionApplicationTermOnEntityContainer_GeneratesExpectedCsdl()
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
        
        IEnumerable<XElement> expectedXElements = new[] { expectedCsdl }.Select(s => XElement.Load(new StringReader(s)));
        IEnumerable<XElement> actualXElements = this.GetSerializerResult(this.baseModel).Select(s => XElement.Load(new StringReader(s)));
        
        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Fact]
    public void Serialize_IfExpressionOnEntitySet_GeneratesExpectedCsdl()
    {
        this.SetupModels();

        var container = new EdmEntityContainer("NS1", "myContainer");
        var set = container.AddEntitySet("personSet", this.baseModel.FindType("NS1.Person") as IEdmEntityType);
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
        
        IEnumerable<XElement> expectedXElements = new[] { expectedCsdl }.Select(s => XElement.Load(new StringReader(s)));
        IEnumerable<XElement> actualXElements = this.GetSerializerResult(this.baseModel).Select(s => XElement.Load(new StringReader(s)));
        
        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Fact]
    public void Serialize_CollectionTermOnEntityType_GeneratesExpectedCsdl()
    {
        this.SetupModels();

        var person = this.baseModel.FindType("NS1.Person") as IEdmEntityType;
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
        
        IEnumerable<XElement> expectedXElements = new[] { expectedCsdl }.Select(s => XElement.Load(new StringReader(s)));
        IEnumerable<XElement> actualXElements = this.GetSerializerResult(this.baseModel).Select(s => XElement.Load(new StringReader(s)));
        
        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Fact]
    public void Serialize_RecordTermOnEntityType_GeneratesExpectedCsdl()
    {
        this.SetupModels();

        var person = this.baseModel.FindType("NS1.Person") as IEdmEntityType;
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
        
        IEnumerable<XElement> expectedXElements = new[] { expectedCsdl }.Select(s => XElement.Load(new StringReader(s)));
        IEnumerable<XElement> actualXElements = this.GetSerializerResult(this.baseModel).Select(s => XElement.Load(new StringReader(s)));
        
        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Fact]
    public void Serialize_IsOfExpressionOnEntityType_GeneratesExpectedCsdl()
    {
        this.SetupModels();

        var person = this.baseModel.FindType("NS1.Person") as IEdmEntityType;
        IEdmTerm termStringValue = this.longDefinitionModel.FindTerm("bar.StringValue");
        var isPersonType = new EdmIsOfExpression(new EdmStringConstant("s1"), new EdmEntityTypeReference(person, true));
        this.CreateAndAttachVocabularyAnnotation(person, termStringValue, isPersonType);

        var isStringType = new EdmIsOfExpression(new EdmStringConstant("s2"), EdmCoreModel.Instance.GetString(true));
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
            <IsOf Type=""NS1.Person"">
                <String>s1</String>
            </IsOf>
        </Annotation>
        <Annotation Term=""bar.StringValue"">
            <IsOf Type=""Edm.String"">
                <String>s2</String>
            </IsOf>
        </Annotation>
    </Annotations>
</Schema>";
        
        IEnumerable<XElement> expectedXElements = new[] { expectedCsdl }.Select(s => XElement.Load(new StringReader(s)));
        IEnumerable<XElement> actualXElements = this.GetSerializerResult(this.baseModel).Select(s => XElement.Load(new StringReader(s)));
        
        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_TermDefinition_GeneratesExpectedCsdl(EdmVersion edmVersion)
    {
        this.SetupModels();

        IEnumerable<XElement> expectedXElements = new[] { longDefinitionModelCsdl }.Select(s => XElement.Load(new StringReader(s)));
        IEnumerable<XElement> actualXElements = this.GetSerializerResult(this.longDefinitionModel, edmVersion, out _).Select(s => XElement.Load(new StringReader(s)));
        
        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        Assert.Single(expectedXElements);
        var expectedXElement = expectedXElements.Single();
        var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
        var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

        Assert.NotNull(actualXElement);
        comparer.Compare(expectedXElement, actualXElement);
    }

    [Fact]
    public void Serialize_CollectionTermDefinition_GeneratesExpectedCsdl()
    {
        this.SetupModels();

        var definitionModel = new EdmModel();
        var collectionOfInt16 = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt16(false /*isNullable*/)));
        var valueTermInt16 = new EdmTerm("NS2", "CollectionOfInt16", collectionOfInt16);
        definitionModel.AddElement(valueTermInt16);

        var collectionOfPerson = new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(this.baseModel.FindType("NS1.Person") as IEdmEntityType, true)));
        var valueTermPerson = new EdmTerm("NS2", "CollectionOfPerson", collectionOfPerson);
        definitionModel.AddElement(valueTermPerson);

        string expectedCsdl =
@"<Schema Namespace=""NS2"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""CollectionOfInt16"" Type=""Collection(Edm.Int16)"" Nullable=""false"" />
    <Term Name=""CollectionOfPerson"" Type=""Collection(NS1.Person)"" />
</Schema>";

        IEnumerable<XElement> expectedXElements = new[] { expectedCsdl }.Select(s => XElement.Load(new StringReader(s)));
        IEnumerable<XElement> actualXElements = this.GetSerializerResult(definitionModel).Select(s => XElement.Load(new StringReader(s)));
        
        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    // [EdmLib] is Edm.TypeTerm still needed? If so, please define it and use it
    [Fact]
    public void Serialize_TypeInheritanceInTermDefinition_GeneratesExpectedCsdl()
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

        IEnumerable<XElement> expectedXElements = new[] { expectedCsdl }.Select(s => XElement.Load(new StringReader(s)));
        IEnumerable<XElement> actualXElements = this.GetSerializerResult(definitionModel).Select(s => XElement.Load(new StringReader(s)));
        
        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_InvalidTypeUsingCastCollectionModel_GeneratesExpectedCsdl(EdmVersion edmVersion)
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

        IEnumerable<XElement> expectedXElements = new string[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        IEnumerable<XElement> actualXElements = this.GetSerializerResult(model).Select(s => XElement.Load(new StringReader(s)));

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_CastNullableToNonNullableModel_GeneratesExpectedCsdl(EdmVersion edmVersion)
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

        IEnumerable<XElement> expectedXElements = new string[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        IEnumerable<XElement> actualXElements = this.GetSerializerResult(model).Select(s => XElement.Load(new StringReader(s)));
        
        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_CastNullableToNonNullableOnInlineAnnotationModel_GeneratesExpectedCsdl(EdmVersion edmVersion)
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

        IEnumerable<XElement> expectedXElements = new string[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        IEnumerable<XElement> actualXElements = this.GetSerializerResult(model).Select(s => XElement.Load(new StringReader(s)));
        
        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_CastResultFalseEvaluationModel_GeneratesExpectedCsdl(EdmVersion edmVersion)
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

        IEnumerable<XElement> expectedXElements = new string[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        IEnumerable<XElement> actualXElements = this.GetSerializerResult(model).Select(s => XElement.Load(new StringReader(s)));
        
        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_CastResultTrueEvaluationModel_GeneratesExpectedCsdl(EdmVersion edmVersion)
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

        IEnumerable<XElement> expectedXElements = new string[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        IEnumerable<XElement> actualXElements = this.GetSerializerResult(model).Select(s => XElement.Load(new StringReader(s)));
        
        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_InvalidPropertyTypeUsingIsOfOnOutOfLineAnnotationModel_GeneratesExpectedCsdl(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""FriendName"" Type=""Edm.String"" />
    <Annotations Target=""NS.FriendName"">
        <Annotation Term=""NS.FriendName"">
            <IsOf Type=""Edm.String"">
                <String>foo</String>
            </IsOf>
        </Annotation>
    </Annotations>
</Schema>";

        var model = new EdmModel();

        var friendName = new EdmTerm("NS", "FriendName", EdmCoreModel.Instance.GetString(true));
        model.AddElement(friendName);

        var valueAnnotation = new EdmVocabularyAnnotation(
            friendName,
            friendName,
            new EdmIsOfExpression(new EdmStringConstant("foo"), EdmCoreModel.Instance.GetString(true)));
        valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
        model.AddVocabularyAnnotation(valueAnnotation);

        IEnumerable<XElement> expectedXElements = new string[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        IEnumerable<XElement> actualXElements = this.GetSerializerResult(model).Select(s => XElement.Load(new StringReader(s)));
        
        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_InvalidPropertyTypeUsingIsOfOnInlineAnnotationModel_GeneratesExpectedCsdl(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""CarTerm"" Type=""NS.Car"" />
    <ComplexType Name=""Car"">
        <Property Name=""Expensive"" Type=""NS.Bike"" />
        <Annotation Term=""NS.CarTerm"">
            <Record>
                <PropertyValue Property=""Expensive"">
                    <IsOf Type=""Edm.String"">
                        <String>foo</String>
                    </IsOf>
                </PropertyValue>
            </Record>
        </Annotation>
    </ComplexType>
    <ComplexType Name=""Bike"">
        <Property Name=""Color"" Type=""String"" />
    </ComplexType>
</Schema>";

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
                new EdmPropertyConstructor(carExpensive.Name, new EdmIsOfExpression(new EdmStringConstant("foo"), EdmCoreModel.Instance.GetString(true)))));
        valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
        model.AddVocabularyAnnotation(valueAnnotation);

        IEnumerable<XElement> expectedXElements = new string[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        IEnumerable<XElement> actualXElements = this.GetSerializerResult(model).Select(s => XElement.Load(new StringReader(s)));
        
        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_IsOfResultFalseEvaluationModel_GeneratesExpectedCsdl(EdmVersion edmVersion)
    {
        var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""BooleanFlag"" Type=""Edm.Boolean"" />
    <Annotations Target=""NS.BooleanFlag"">
        <Annotation Term=""NS.BooleanFlag"">
            <IsOf Type=""Edm.String"">
                <Int>32</Int>
            </IsOf>
        </Annotation>
    </Annotations>
</Schema>";

        var model = new EdmModel();

        var booleanFlag = new EdmTerm("NS", "BooleanFlag", EdmCoreModel.Instance.GetBoolean(true));
        model.AddElement(booleanFlag);

        var valueAnnotation = new EdmVocabularyAnnotation(
            booleanFlag,
            booleanFlag,
            new EdmIsOfExpression(new EdmIntegerConstant(32), EdmCoreModel.Instance.GetString(true)));
        valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
        model.AddVocabularyAnnotation(valueAnnotation);

        IEnumerable<XElement> expectedXElements = new string[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        IEnumerable<XElement> actualXElements = this.GetSerializerResult(model).Select(s => XElement.Load(new StringReader(s)));
        
        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void Serialize_IsOfResultTrueEvaluationModel_GeneratesExpectedCsdl(EdmVersion edmVersion)
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
                    <IsOf Type=""Edm.String"">
                        <String>foo</String>
                    </IsOf>
                </PropertyValue>
            </Record>
        </Annotation>
    </Annotations>
</Schema>";

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
                new EdmPropertyConstructor(flag.Name, new EdmIsOfExpression(new EdmStringConstant("foo"), EdmCoreModel.Instance.GetString(true)))));
        valueAnnotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
        model.AddVocabularyAnnotation(valueAnnotation);

        IEnumerable<XElement> expectedXElements = new string[] { csdl }.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        IEnumerable<XElement> actualXElements = this.GetSerializerResult(model).Select(s => XElement.Load(new StringReader(s)));
        
        Assert.Equal(expectedXElements.Count(), actualXElements.Count());

        var comparer = new CsdlXElementComparer();

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectedXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        comparer.Compare(expectedContainers, actualContainers);

        // compare non-EntityContainers
        foreach (XElement expectedXElement in expectedXElements)
        {
            var schemaNamespace = expectedXElement.Attribute("Namespace")?.Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace")?.Value);

            Assert.NotNull(actualXElement);
            comparer.Compare(expectedXElement, actualXElement);
        }
    }

    #region Private

    private string longDefinitionModelCsdl =
@"<Schema Namespace=""bar"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""BinaryValue"" Type=""Binary"" />
    <Term Name=""BooleanValue"" Type=""Boolean"" />
    <Term Name=""ByteValue"" Type=""Byte"" />
    <Term Name=""DateValue"" Type=""Date"" />
    <Term Name=""DateTimeOffsetValue"" Type=""DateTimeOffset"" />
    <Term Name=""DecimalValue"" Scale=""variable"" Type=""Decimal"" />
    <Term Name=""DoubleValue"" Type=""Double"" />
    <Term Name=""GuidValue"" Type=""Guid"" />
    <Term Name=""Int16Value"" Type=""Int16"" />
    <Term Name=""Int32Value"" Type=""Int32"" />
    <Term Name=""Int64Value"" Type=""Int64"" />
    <Term Name=""SByteValue"" Type=""SByte"" />
    <Term Name=""SingleValue"" Type=""Single"" />
    <Term Name=""StringValue"" Type=""String"" MaxLength=""512"" />
    <Term Name=""TimeOfDayValue"" Type=""TimeOfDay"" />
    <Term Name=""GeographyValue"" Type=""Geography"" SRID=""variable"" />
    <Term Name=""GeometryValue"" Type=""Geometry"" SRID=""variable"" />
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
        var csdlElements = new[] { csdl }.Select(XElement.Parse);
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), referencedModels, out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.Empty(errors);

        return edmModel;
    }

    #endregion
}
