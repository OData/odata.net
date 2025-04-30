﻿//---------------------------------------------------------------------
// <copyright file="ClrTypeMappingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using BindingFlags = System.Reflection.BindingFlags;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class ClrTypeMappingTests : EdmLibTestCaseBase
{
    private void InitializeOperationDefinitions()
    {
        string operationCsdl = @"<Schema Namespace=""Functions"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Function Name=""IntAdd"">
    <Parameter Name=""Int1"" Type=""Int64"" />
    <Parameter Name=""Int2"" Type=""Int64"" />
    <ReturnType Type=""Int64"" />
  </Function>
</Schema>";

        SchemaReader.TryParse(new XElement[] { XElement.Parse(operationCsdl) }.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        this.operationDeclarationModel = edmModel;
        this.operationDefinitions = new Dictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>>();
        this.operationDefinitions[this.operationDeclarationModel.FindOperations("Functions.IntAdd").Single(f => f.Parameters.Count() == 2)] = (a) => new EdmIntegerConstant(((IEdmIntegerValue)a[0]).Value + ((IEdmIntegerValue)a[1]).Value);
    }

    [Fact]
    public void Should_ValidateBasicClrTypeMappingForVocabularyAnnotations()
    {
        this.InitializeOperationDefinitions();

        SchemaReader.TryParse(VocabularyAnnotationClassTypeBasicTest().Select(e => e.CreateReader()), this.operationDeclarationModel, out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "Coordination").Single(),
               new Coordination() { X = 10, Y = 20 });
        this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "InspectedBy").Single(),
               new Person() { Id = 10, FirstName = "Young", LastName = "Hong" });
        this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "TVDisplay").Single(),
               new DisplayCoordination() { X = 10, Y = 20, Origin = new Coordination() { X = 10, Y = 20 } });
        this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "TVDisplay").Single(),
               new Coordination() { X = 10, Y = 20 });
        this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "MultiMonitors").Single(),
               (IEnumerable<Coordination>)new List<Coordination>() { new Coordination() { X = 10, Y = 20 }, new Coordination() { X = 30, Y = 40 } });
        this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "LabledMultiMonitors").Single(),
               (IEnumerable<Coordination>)new List<Coordination>() { new Coordination() { X = 10, Y = 20 } });
        this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "EmptyCollection").Single(),
               (IEnumerable<Coordination>)new List<Coordination>());
    }

    [Fact]
    public void Should_ThrowInvalidCastException_When_ConvertingBetweenCollectionAndSingularObject()
    {
        SchemaReader.TryParse(VocabularyAnnotationClassTypeBasicTest().Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var exception = Assert.Throws<InvalidCastException>(() =>
            this.ValidateClrObjectConverter<IEnumerable<Coordination>>(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "TVDisplay").Single(), null));
        Assert.Equal("Unable to cast object of type 'Microsoft.OData.Edm.Vocabularies.EdmStructuredValue' to type 'Microsoft.OData.Edm.Vocabularies.IEdmCollectionValue'.", exception.Message);

        var exception2 = Assert.Throws<InvalidCastException>(() =>
            this.ValidateClrObjectConverter<Coordination>(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "MultiMonitors").Single(), null));
        Assert.Equal("Conversion of an edm collection value to the CLR type 'Microsoft.OData.Edm.E2E.Tests.FunctionalTests.ClrTypeMappingTests+Coordination' is not supported. EDM collection values can be converted to System.Collections.Generic.IEnumerable{T}, System.Collections.Generic.IList{T} or System.Collections.Generic.ICollection{T}.",
            exception2.Message);
    }

    [Fact]
    public void Should_ThrowInvalidCastException_When_ConvertingCollectionValueToUnsupportedCollectionType()
    {
        SchemaReader.TryParse(VocabularyAnnotationClassTypeBasicTest().Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var exception = Assert.Throws<InvalidCastException>(() =>
            this.ValidateClrObjectConverter
            (
                this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "MultiMonitors").Single(),
                new List<Coordination>()
                {
                        new Coordination() { X = 10, Y = 20 },
                        new Coordination() { X = 30, Y = 40 }
                }
            ));
        Assert.Equal("Conversion of an edm collection value to the CLR type 'System.Collections.Generic.List`1[[Microsoft.OData.Edm.E2E.Tests.FunctionalTests.ClrTypeMappingTests+Coordination, Microsoft.OData.Edm.E2E.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]' is not supported. EDM collection values can be converted to System.Collections.Generic.IEnumerable{T}, System.Collections.Generic.IList{T} or System.Collections.Generic.ICollection{T}.",
            exception.Message);
    }

    [Fact]
    public void Should_ValidateRecordValueTypeMappingForVocabularyAnnotations()
    {
        var csdl =
@"<Schema Namespace='NS1' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <EntityType Name='Person'>
    <Property Name='Id' Type='Int16' Nullable='false' />
    <Property Name='FirstName' Type='String' Nullable='false' Unicode='true' />
    <Property Name='LastName' Type='String' Nullable='false' Unicode='true' />
  </EntityType>
  <EntityType Name='DifferentPerson'>
    <Property Name='Id' Type='Int16' Nullable='false' />
    <Property Name='FirstName' Type='String' Nullable='false' Unicode='true' />
    <Property Name='LastName' Type='String' Nullable='false' Unicode='true' />
  </EntityType>
  <Term Name='InspectedBy' Type='NS1.DifferentPerson' Nullable='false' />
  <Annotations Target='NS1.Person'>
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
  </Annotations>
</Schema>";

        SchemaReader.TryParse(new XElement[] { XElement.Parse(csdl) }.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var person = edmModel.FindType("NS1.Person") as IEdmEntityType;
        var differentPerson = edmModel.FindType("NS1.DifferentPerson") as IEdmEntityType;

        EdmStructuredValue dummyPerson =
            new EdmStructuredValue(
                new EdmEntityTypeReference(person, false),
                new EdmPropertyValue[3]
                {
                        new EdmPropertyValue("Id", new EdmIntegerConstant(-1)),
                        new EdmPropertyValue("FirstName", new EdmStringConstant("Noman")),
                        new EdmPropertyValue("LastName", new EdmStringConstant("Nobody"))
                });


        var edmValue = edmModel.GetTermValue(dummyPerson, "NS1.InspectedBy", new EdmToClrEvaluator(null));
        Assert.Equal(edmValue.Type.Definition, differentPerson);
    }

    [Fact]
    public void Should_ThrowInvalidCastException_When_ConvertingPrimitiveTypeToUnsupportedClrType()
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

        SchemaReader.TryParse(new XElement[] { XElement.Parse(csdl) }.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var annotation = edmModel.FindVocabularyAnnotations(edmModel.FindType("NS1.Person")).Where(n => n.Term == edmModel.FindTerm("NS1.Title1")).Single();
        var edmValue = new EdmToClrEvaluator(null).Evaluate(annotation.Value);

        var action = () => new EdmToClrConverter().AsClrValue<Coordination>(edmValue);
        var exception = Assert.Throws<InvalidCastException>(action);
        Assert.Equal("Conversion of an EDM value of the type 'IEdmStringConstantExpression' to the CLR type 'Microsoft.OData.Edm.E2E.Tests.FunctionalTests.ClrTypeMappingTests+Coordination' is not supported.",
            exception.Message);
    }

    [Fact]
    public void Should_ValidateClrTypeMappingForInterfaces()
    {
        this.InitializeOperationDefinitions();

        SchemaReader.TryParse(VocabularyAnnotationClassTypeBasicTest().Select(e => e.CreateReader()), this.operationDeclarationModel, out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        
        Assert.False(errors.Any());

        this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "Coordination").Single(),
               new Coordination2() { X = 10, Y = 20 });
        this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "InspectedBy").Single(),
           new Person2() { Id = 10, FirstName = "Young", LastName = "Hong" });
        this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "AdoptPet").Single(),
           new Pet() { Name = "Jacquine", Breed = "Bull Dog", Age = 3 });
    }

    [Fact]
    public void Should_ThrowMissingMethodException_When_ClrTypeHasNoParameterlessConstructor()
    {
        SchemaReader.TryParse(VocabularyAnnotationClassTypeBasicTest().Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var valueAnnotation = this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "InspectedBy").Single();

        var exception = Assert.Throws<MissingMethodException>(() => ConvertToClrObject<Person3>(valueAnnotation));
        Assert.Equal("No parameterless constructor defined for type 'Microsoft.OData.Edm.E2E.Tests.FunctionalTests.ClrTypeMappingTests+Person3'.", exception.Message);

        exception = Assert.Throws<MissingMethodException>(() => ConvertToClrObject<Person4>(valueAnnotation));
        Assert.Equal("No parameterless constructor defined for type 'Microsoft.OData.Edm.E2E.Tests.FunctionalTests.ClrTypeMappingTests+Person4'.", exception.Message);

        exception = Assert.Throws<MissingMethodException>(() => ConvertToClrObject<Person5>(valueAnnotation));
        Assert.Equal("Cannot dynamically create an instance of type 'Microsoft.OData.Edm.E2E.Tests.FunctionalTests.ClrTypeMappingTests+Person5'. Reason: No parameterless constructor defined.", exception.Message);
    }

    [Fact]
    public void Should_ValidateClrTypeMappingForPrivateProperties()
    {
        SchemaReader.TryParse(VocabularyAnnotationClassTypeBasicTest().Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var valueAnnotation = this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "InspectedBy").Single();

        this.ValidateClrObjectConverter(valueAnnotation, new Person6());

        this.ValidateClrObjectConverter(valueAnnotation, new Person7() { Id = 10 });

        Person8 actual = ConvertToClrObject<Person8>(valueAnnotation);
        Assert.Equal(10, actual.Id);
        Assert.Equal("Young", actual.FirstName);
    }

    [Fact]
    public void Should_ValidateClrTypeMappingForGenericTypes()
    {
        SchemaReader.TryParse(VocabularyAnnotationClassTypeBasicTest().Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var valueAnnotation = this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "InspectedBy").Single();

        this.ValidateClrObjectConverter(valueAnnotation, new Person9<string>() { Id = 10, FirstName = "Young", LastName = "Hong" });
    }

    [Fact]
    public void Should_ValidateClrTypeMappingForDifferentPropertyTypes()
    {
        this.InitializeOperationDefinitions();

        SchemaReader.TryParse(VocabularyAnnotationClassTypeBasicTest().Select(e => e.CreateReader()), this.operationDeclarationModel, out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var valueAnnotation = this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "TVDisplay").Single();

        this.ValidateClrObjectConverter(valueAnnotation, new Display2() { X = 10, Y = 20, Origin = new Display1() { X = 10, Y = 20 } });
        this.ValidateClrObjectConverter(valueAnnotation, new Display1() { X = 10, Y = 20 });
    }

    [Fact]
    public void Should_ThrowOverflowException_When_PrimitiveTypeExceedsClrTypeRange()
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

        SchemaReader.TryParse(new XElement[] { XElement.Parse(csdl) }.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var annotations = edmModel.FindVocabularyAnnotations(edmModel.FindType("NS1.Person"));

        Func<string, IEdmIntegerConstantExpression> GetIntegerExpression = (termName) =>
        {
            var valueAnnotation = annotations.Single(n => n.Term.Name == termName);
            return (IEdmIntegerConstantExpression)valueAnnotation.Value;
        };
        Func<string, IEdmFloatingConstantExpression> GetFloatExpression = (termName) =>
        {
            var valueAnnotation = annotations.Single(n => n.Term.Name == termName);
            return (IEdmFloatingConstantExpression)valueAnnotation.Value;
        };
        Func<string, IEdmValue> GetEdmValue = (termName) =>
        {
            var valueAnnotation = annotations.Single(n => n.Term.Name == termName);
            var edmToClrEvaluator = new EdmToClrEvaluator(null);
            return edmToClrEvaluator.Evaluate(valueAnnotation.Value);
        };

        var edmToClrConverter = new EdmToClrConverter();

        Assert.Equal(edmToClrConverter.AsClrValue(GetFloatExpression("SingleValue"), typeof(Single)), float.PositiveInfinity);
        Assert.Equal(edmToClrConverter.AsClrValue(GetFloatExpression("NegativeSingleValue"), typeof(Single)), float.NegativeInfinity);

        var exception = Assert.Throws<OverflowException>(() => edmToClrConverter.AsClrValue(GetEdmValue("ByteValue"), typeof(byte)));
        Assert.Equal("Arithmetic operation resulted in an overflow.", exception.Message);
        exception = Assert.Throws<OverflowException>(() => edmToClrConverter.AsClrValue<byte>(GetEdmValue("ByteValue")));
        Assert.Equal("Arithmetic operation resulted in an overflow.", exception.Message);
        exception = Assert.Throws<OverflowException>(() => new EdmToClrEvaluator(null).EvaluateToClrValue<byte>(GetIntegerExpression("ByteValue")));
        Assert.Equal("Arithmetic operation resulted in an overflow.", exception.Message);
        exception = Assert.Throws<OverflowException>(() => edmToClrConverter.AsClrValue(GetEdmValue("SByteValue"), typeof(sbyte)));
        Assert.Equal("Arithmetic operation resulted in an overflow.", exception.Message);
        exception = Assert.Throws<OverflowException>(() => edmToClrConverter.AsClrValue<sbyte>(GetEdmValue("SByteValue")));
        Assert.Equal("Arithmetic operation resulted in an overflow.", exception.Message);
        exception = Assert.Throws<OverflowException>(() => new EdmToClrEvaluator(null).EvaluateToClrValue<sbyte>(GetIntegerExpression("SByteValue")));
        Assert.Equal("Arithmetic operation resulted in an overflow.", exception.Message);
    }

    [Fact]
    public void Should_ValidateRecursivePropertyMappingForVocabularyAnnotations()
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

        SchemaReader.TryParse(new XElement[] { XElement.Parse(csdl) }.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var valueAnnotation = this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "RecursiveProperty").Single();

        this.ValidateClrObjectConverter(valueAnnotation, new RecursiveProperty() { X = 1, Y = 2, Origin = new RecursiveProperty() { X = 3, Y = 4 } });
    }

    [Fact]
    public void Should_ValidateDerivedPropertyMappingWithNewProperties()
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

        SchemaReader.TryParse(new XElement[] { XElement.Parse(csdl) }.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var valueAnnotation = this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "RecursivePropertyWithNewProperties").Single();

        this.ValidateClrObjectConverter(valueAnnotation, new DerivedRecursiveProperty() { X = 1, Y = 2, Origin = 4 });
    }

    [Fact]
    public void Should_ValidateCollectionPropertyMappingForVocabularyAnnotations()
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

        SchemaReader.TryParse(new XElement[] { XElement.Parse(csdl) }.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var valueAnnotation = this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "PersonValueAnnotation").Single();

        this.ValidateClrObjectConverter(valueAnnotation,
            new ClassWithCollectionProperty()
            {
                X = new int[] { 4, 5 },
                Y = new int[] { 6, 7 },
                Z = new int[] { 8, 9 },
                C = new Display1[] {
                    new Display1() { X = 10, Y = 11 },
                    new Display1() { X = 12, Y = 13 }
                },
            });
    }

    [Fact]
    public void Should_ValidateCollectionOfCollectionPropertyMappingForVocabularyAnnotations()
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

        SchemaReader.TryParse(new XElement[] { XElement.Parse(csdl) }.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var valueAnnotation = this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "PersonValueAnnotation").Single();

        this.ValidateClrObjectConverter(valueAnnotation,
            new ClassWithCollectionOfCollectionProperty()
            {
                C = new int[][] { new int[] { 8, 9 } }
            });
    }

    [Fact]
    public void Should_ValidateEnumMappingForVocabularyAnnotations()
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

        SchemaReader.TryParse(new XElement[] { XElement.Parse(csdl) }.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var annotations = edmModel.FindVocabularyAnnotations(edmModel.FindType("NS1.Person"));

        Func<string, IEdmIntegerConstantExpression> GetIntegerExpression = (termName) =>
        {
            var valueAnnotation = annotations.Single(n => n.Term.Name == termName);
            return (IEdmIntegerConstantExpression)valueAnnotation.Value;
        };

        Func<string, IEdmValue> GetEdmValue = (termName) =>
        {
            var valueAnnotation = annotations.Single(n => n.Term.Name == termName);
            var edmToClrEvaluator = new EdmToClrEvaluator(null);
            return edmToClrEvaluator.Evaluate(valueAnnotation.Value);
        };

        var edmToClrConverter = new EdmToClrConverter();

        Assert.Equal(edmToClrConverter.AsClrValue(GetIntegerExpression("PersonValueAnnotation3"), typeof(EnumInt)), EnumInt.Member1);
        Assert.Equal(EnumInt.Member1, new EdmToClrEvaluator(null).EvaluateToClrValue<EnumInt>(GetIntegerExpression("PersonValueAnnotation3")));
        Assert.Equal((EnumInt)(-2), new EdmToClrEvaluator(null).EvaluateToClrValue<EnumInt>(GetIntegerExpression("PersonValueAnnotation4")));

        this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "PersonValueAnnotation1").Single(),
            new ClassWithEnum()
            {
                EnumInt = EnumInt.Member1,
                EnumByte = (EnumByte)10,
                EnumULong = EnumULong.Member2
            });

        var exception = Assert.Throws<InvalidCastException>(() => new EdmToClrEvaluator(null).EvaluateToClrValue<EnumInt>(GetIntegerExpression("PersonValueAnnotation2")));
        Assert.Equal("Unable to cast object of type 'Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsDecimalConstantExpression' to type 'Microsoft.OData.Edm.Vocabularies.IEdmIntegerConstantExpression'.", exception.Message);

        this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "PersonValueAnnotation8").Single(),
               new ClassWithEnum()
               {
                   EnumInt = (EnumInt)10,
               });
    }

    [Fact]
    public void Should_ThrowInvalidCastException_When_DuplicatePropertyNamesExistInVocabularyAnnotations()
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

        SchemaReader.TryParse(new XElement[] { XElement.Parse(csdl) }.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var exception = Assert.Throws<InvalidCastException>(() =>
            this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "PersonValueAnnotation1").Single(), new ClassWithEnum()));
        Assert.Equal("An EDM structured value contains multiple values for the property 'EnumInt'. Conversion of an EDM structured value with duplicate property values is not supported.", exception.Message);
    }

    [Fact]
    public void Should_HandleEmptyVocabularyAnnotationsCorrectly()
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

        SchemaReader.TryParse(new XElement[] { XElement.Parse(csdl) }.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "PersonValueAnnotation1").Single(),
            new ClassWithEnum()
            {
                EnumInt = EnumInt.Member2,
                EnumByte = EnumByte.Member1,
                EnumULong = (EnumULong)0
            });

        this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "PersonValueAnnotation1").Single(), new ClassWithCollectionProperty());
        this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "PersonValueAnnotation1").Single(), new ClassWithCollectionOfCollectionProperty());
        this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "PersonValueAnnotation1").Single(), new DisplayCoordination());
        this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "PersonValueAnnotation1").Single(), new EmptyClass());

        var exception = Assert.Throws<InvalidCastException>(() =>
            this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "PersonValueAnnotation1").Single(), (int)1));
        Assert.Equal("Unable to cast object of type 'Microsoft.OData.Edm.Vocabularies.EdmStructuredValue' to type 'Microsoft.OData.Edm.Vocabularies.IEdmIntegerValue'.", exception.Message);

        var exception2 = Assert.Throws<ArgumentNullException>(() =>
            this.ConvertToClrObject<int>(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "PersonValueAnnotation2").Single()));
        Assert.Equal("Value cannot be null. (Parameter 'expression')", exception2.Message);
    }

    [Fact]
    public void Should_ThrowInvalidCastException_When_ConvertingToStructType()
    {
        SchemaReader.TryParse(VocabularyAnnotationClassTypeBasicTest().Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var exception = Assert.Throws<InvalidCastException>(() =>
            this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "Coordination").Single(), new Display1StructType()));
        Assert.Equal("Conversion of an EDM structured value is supported only to a CLR class.", exception.Message);

        exception = Assert.Throws<InvalidCastException>(() =>
            this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "TVDisplay").Single(), new Display2StructTypeWithObjectProperty() { X = 10, Y = 20, Origin = new Display1() }));
        Assert.Equal("Conversion of an EDM structured value is supported only to a CLR class.", exception.Message);

        exception = Assert.Throws<InvalidCastException>(() =>
            this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "TVDisplay").Single(), new Display2StructTypeWithStructProperty() { X = 10, Y = 20, Origin = new Display1StructType() }));
        Assert.Equal("Conversion of an EDM structured value is supported only to a CLR class.", exception.Message);
    }

    [Fact]
    public void Should_ThrowMissingMethodException_When_ConvertingToAbstractType()
    {
        this.InitializeOperationDefinitions();

        SchemaReader.TryParse(VocabularyAnnotationClassTypeBasicTest().Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var exception = Assert.Throws<MissingMethodException>(() =>
            this.ConvertToClrObject<AbstractClass>(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "Coordination").Single()));
        Assert.Equal("Cannot dynamically create an instance of type 'Microsoft.OData.Edm.E2E.Tests.FunctionalTests.ClrTypeMappingTests+AbstractClass'. Reason: Cannot create an abstract class.", exception.Message);
    }

    [Fact]
    public void Should_HandleTryCreateObjectInstanceForClrTypeMapping()
    {
        this.InitializeOperationDefinitions();

        EdmToClrEvaluator ev = new EdmToClrEvaluator(this.operationDefinitions);

        SchemaReader.TryParse(VocabularyAnnotationClassTypeBasicTest().Select(e => e.CreateReader()), this.operationDeclarationModel, out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var value = ev.Evaluate(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "TVDisplay").Single().Value);

        var isObjectPopulated = true;
        var isObjectInitialized = true;
        object createdObjectInstance = null;
        TryCreateObjectInstance tryCreateObjectInstance = (IEdmStructuredValue edmValue, Type clrType, EdmToClrConverter converter, out object objectInstance, out bool objectInstanceInitialized) =>
        {
            objectInstance = createdObjectInstance;
            objectInstanceInitialized = isObjectPopulated;
            return isObjectInitialized;
        };

        ev.EdmToClrConverter = new EdmToClrConverter(tryCreateObjectInstance);
        Assert.Null((Display2)ev.EdmToClrConverter.AsClrValue(value, typeof(Display2)));

        isObjectPopulated = false;
        isObjectInitialized = true;
        ev.EdmToClrConverter = new EdmToClrConverter(tryCreateObjectInstance);
        Assert.Null((Display2)ev.EdmToClrConverter.AsClrValue(value, typeof(Display2)));

        isObjectPopulated = true;
        isObjectInitialized = false;
        ev.EdmToClrConverter = new EdmToClrConverter(tryCreateObjectInstance);
        Assert.True(CompareObjects((Display2)ev.EdmToClrConverter.AsClrValue(value, typeof(Display2)), new Display2() { X = 10, Y = 20, Origin = new Display1() { X = 10, Y = 20 } }));

        isObjectPopulated = false;
        isObjectInitialized = false;
        ev.EdmToClrConverter = new EdmToClrConverter(tryCreateObjectInstance);
        Assert.True(CompareObjects((Display2)ev.EdmToClrConverter.AsClrValue(value, typeof(Display2)), new Display2() { X = 10, Y = 20, Origin = new Display1() { X = 10, Y = 20 } }));

        createdObjectInstance = new Display2() { X = 0, Y = 1, Origin = new Display1 { X = 3, Y = 4 } };
        isObjectPopulated = true;
        isObjectInitialized = true;
        ev.EdmToClrConverter = new EdmToClrConverter(tryCreateObjectInstance);
        Assert.True(CompareObjects((Display2)ev.EdmToClrConverter.AsClrValue(value, typeof(Display2)), createdObjectInstance));

        ev.EdmToClrConverter = new EdmToClrConverter((IEdmStructuredValue edmValue, Type clrType, EdmToClrConverter converter, out object objectInstance, out bool objectInstanceInitialized) =>
        {
            if (clrType == typeof(Display2))
            {
                objectInstance = new Display2() { X = 0, Y = 1, Origin = new Display1 { X = 3, Y = 4 } };
                objectInstanceInitialized = false;
                return true;
            }
            else if (clrType == typeof(Display1))
            {
                objectInstance = new Display1 { X = 3, Y = 4 };
                objectInstanceInitialized = false;
                return true;
            }
            else
            {
                objectInstance = null;
                objectInstanceInitialized = false;
                return false;
            }
        });

        Assert.True(CompareObjects((Display2)ev.EdmToClrConverter.AsClrValue(value, typeof(Display2)), new Display2() { X = 10, Y = 20, Origin = new Display1() { X = 10, Y = 20 } }));

        isObjectPopulated = false;
        isObjectInitialized = true;
        createdObjectInstance = new DisplayCoordination();
        ev.EdmToClrConverter = new EdmToClrConverter(tryCreateObjectInstance);
        Coordination actual = (Coordination)ev.EdmToClrConverter.AsClrValue(value, typeof(Coordination));
        Coordination expected = new Coordination() { X = 10, Y = 20 };
        Assert.Equal(expected.X, actual.X);
        Assert.Equal(expected.Y, actual.Y);
    }

    [Fact]
    public void Should_HandleTryPopulateObjectInstanceForClrTypeMapping()
    {
        this.InitializeOperationDefinitions();

        EdmToClrEvaluator ev = new EdmToClrEvaluator(this.operationDefinitions);

        SchemaReader.TryParse(VocabularyAnnotationClassTypeBasicTest().Select(e => e.CreateReader()), this.operationDeclarationModel, out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var value = ev.Evaluate(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "TVDisplay").Single().Value);

        var isObjectPopulated = false;
        var isObjectInitialized = true;
        object createdObjectInstance = new Display1() { X = 0, Y = 1 };
        TryCreateObjectInstance tryCreateObjectInstance = (IEdmStructuredValue edmValue, Type clrType, EdmToClrConverter converter, out object objectInstance, out bool objectInstanceInitialized) =>
        {
            objectInstance = createdObjectInstance;
            objectInstanceInitialized = isObjectPopulated;
            return isObjectInitialized;
        };
        ev.EdmToClrConverter = new EdmToClrConverter(tryCreateObjectInstance);

        var exception = Assert.Throws<InvalidCastException>(() => ev.EdmToClrConverter.AsClrValue(value, typeof(Display2)));
        Assert.Equal("The type 'Microsoft.OData.Edm.E2E.Tests.FunctionalTests.ClrTypeMappingTests+Display1' of the object returned by the TryCreateObjectInstance delegate is not assignable to the expected type 'Microsoft.OData.Edm.E2E.Tests.FunctionalTests.ClrTypeMappingTests+Display2'.", exception.Message);

        isObjectPopulated = false;
        isObjectInitialized = false;
        createdObjectInstance = new Display1() { X = 0, Y = 1 };
        ev.EdmToClrConverter = new EdmToClrConverter(tryCreateObjectInstance);
        Assert.True(CompareObjects((Display2)ev.EdmToClrConverter.AsClrValue(value, typeof(Display2)), new Display2() { X = 10, Y = 20, Origin = new Display1() { X = 10, Y = 20 } }));

        isObjectPopulated = true;
        isObjectInitialized = true;
        createdObjectInstance = new Display1() { X = 0, Y = 1 };
        ev.EdmToClrConverter = new EdmToClrConverter(tryCreateObjectInstance);

        exception = Assert.Throws<InvalidCastException>(() => ev.EdmToClrConverter.AsClrValue(value, typeof(Display2)));
        Assert.Equal("The type 'Microsoft.OData.Edm.E2E.Tests.FunctionalTests.ClrTypeMappingTests+Display1' of the object returned by the TryCreateObjectInstance delegate is not assignable to the expected type 'Microsoft.OData.Edm.E2E.Tests.FunctionalTests.ClrTypeMappingTests+Display2'.", exception.Message);
    }

    [Fact]
    public void Should_ThrowInvalidCastException_When_InterfacePropertyIsMapped()
    {
        SchemaReader.TryParse(VocabularyAnnotationClassTypeBasicTest().Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var valueAnnotation = this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "TVDisplay").Single();
        
        var exception = Assert.Throws<InvalidCastException>(() => this.ConvertToClrObject<ClassWithInterfaceProperty>(valueAnnotation));
        Assert.Equal("Conversion of an EDM structured value is supported only to a CLR class.", exception.Message);
    }

    [Fact]
    public void Should_ValidateClrTypeMappingForVirtualMembers()
    {
        SchemaReader.TryParse(VocabularyAnnotationClassTypeBasicTest().Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var valueAnnotation = this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "TVDisplay").Single();

        this.ValidateClrObjectConverter(valueAnnotation, new ClassWithVirtualMember() { X = 10 });
        this.ValidateClrObjectConverter(valueAnnotation, new DerivedClassWithVirtualMember() { X = 10 });
    }

    [Fact]
    public void Should_ThrowInvalidCastException_When_ValueStructTypePropertyIsMapped()
    {
        this.InitializeOperationDefinitions();

        SchemaReader.TryParse(VocabularyAnnotationClassTypeBasicTest().Select(e => e.CreateReader()), this.operationDeclarationModel, out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var valueAnnotation = this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "TVDisplay").Single();

        var exception = Assert.Throws<InvalidCastException>(() => this.ConvertToClrObject<ClassWithStructProperty>(valueAnnotation));
        Assert.Equal("Conversion of an EDM structured value is supported only to a CLR class.", exception.Message);
    }

    [Fact]
    public void Should_ThrowArgumentException_When_CollectionPropertyIsMappedToNullList()
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

        SchemaReader.TryParse(new XElement[] { XElement.Parse(csdl) }.Select(e => e.CreateReader()), out IEdmModel edmModel, out IEnumerable<EdmError> errors);
        Assert.False(errors.Any());

        var valueAnnotation = this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "PersonValueAnnotation").Single();

        var exception = Assert.Throws<ArgumentException>(() => this.ConvertToClrObject<ClassWithNullCollectionProperty>(valueAnnotation));
        Assert.Equal("Property set method not found.", exception.Message);
    }

    private IEnumerable<IEdmVocabularyAnnotation> GetVocabularyAnnotations(IEdmModel edmModel, IEdmVocabularyAnnotatable targetElement, string termName)
    {
        return edmModel.FindVocabularyAnnotations(targetElement).Where(n => n.Term.Name.Equals(termName));
    }

    private T ConvertToClrObject<T>(IEdmVocabularyAnnotation valueAnnotation)
    {
        var edmClrEvaluator = new EdmToClrEvaluator(this.operationDefinitions);
        var edmClrConverter = new EdmToClrConverter();

        var edmValue = edmClrEvaluator.Evaluate(valueAnnotation.Value);

        var object1 = edmClrEvaluator.EvaluateToClrValue<T>(valueAnnotation.Value);
        var object2 = (T)edmClrConverter.AsClrValue(edmValue, typeof(T));
        var object3 = edmClrEvaluator.EdmToClrConverter.AsClrValue<T>(edmValue);

        Assert.True(CompareObjects(object1, object2));
        Assert.True(CompareObjects(object2, object3));

        return object1;
    }

    private static bool CompareObjects(object x, object y)
    {
        if (x == null ^ y == null)
        {
            return false;
        }
        if (x == null && y == null)
        {
            return true;
        }
        if (x is IEnumerable)
        {
            if (!(y is IEnumerable))
            {
                return false;
            }
            return (x as IEnumerable).Cast<object>().SequenceEqual((y as IEnumerable).Cast<object>(), new CompareObjectEqualityComparer());
        }

        var typeX = x.GetType();
        if (typeX != y.GetType())
        {
            return false;
        }
        bool result = true;
        // TODO: We can update this function for the properties of the collection type such as Item. 
        foreach (var property in typeX.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                if (!(property.GetValue(x, null) is IComparable))
                {
                    result &= CompareObjects(property.GetValue(x, null), property.GetValue(y, null));
                }
                else
                {
                    result &= property.GetValue(x, null).Equals(property.GetValue(y, null));
                }
            }
            else
            {
                var enumerableOfX = (IEnumerable)property.GetValue(x, null);
                var enumerableOfY = (IEnumerable)property.GetValue(y, null);
                if (enumerableOfX == null ^ enumerableOfY == null)
                {
                    return false;
                }
                else if (enumerableOfX == null)
                {
                    result &= true;
                }
                else
                {
                    result &= Enumerable.SequenceEqual<object>(enumerableOfX.Cast<object>(), enumerableOfY.Cast<object>(), new CompareObjectEqualityComparer());
                }
            }
        }
        return result;
    }

    private static IEnumerable<XElement> VocabularyAnnotationClassTypeBasicTest()
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

    private class CompareObjectEqualityComparer : IEqualityComparer<object>
    {
        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            return CompareObjects(x, y);
        }

        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }
    }

    private void ValidateClrObjectConverter<T>(IEdmVocabularyAnnotation valueAnnotation, T expected)
    {
        switch (valueAnnotation.Value.ExpressionKind)
        {
            case EdmExpressionKind.Record:
            case EdmExpressionKind.Collection:
                var actual = this.ConvertToClrObject<T>(valueAnnotation);
                Assert.True(CompareObjects(expected, actual));
                break;
        }
    }
    private IEdmModel operationDeclarationModel
    {
        get;
        set;
    }

    private Dictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>> operationDefinitions
    {
        get;
        set;
    }

    public class DisplayCoordination : Coordination
    {
        public Coordination Origin { get; set; }
    }

    public class Coordination
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Id { get; set; }
    }

    public interface ICoordination1
    {
        int X { get; set; }
    }

    public interface ICoordination2
    {
        int Y { get; set; }
    }

    public class Coordination2 : ICoordination1, ICoordination2
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }

    public class Person2
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Id { get; set; }
        private string Additional { get; set; }
    }

    public class Person3
    {
        private Person3()
        {
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Id { get; set; }
    }

    public class Person4
    {
        protected Person4()
        {
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Id { get; set; }
    }

    public class Person5
    {
        public Person5(int id)
        {
            this.Id = id;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Id { get; set; }
    }

    public class Person6
    {
        private string FirstName { get; set; }
        private string LastName { get; set; }
        protected int Id { get; set; }
    }

    public class Person7
    {
        private string FirstName { get; set; }
        private string LastName { get; set; }
        public int Id { get; set; }
        public string Additional { get; set; }
    }

    private class Person8
    {
        public string FirstName { get; private set; }
        public string LastName { private get; set; }
        public int Id { get; private set; }
    }

    public class Pet
    {
        public string Name { get; set; }
        public string Breed { get; set; }
        public int Age { get; set; }
    }

    public class Person9<T>
    {
        public T FirstName { get; set; }
        public T LastName { get; set; }
        public int Id { get; set; }
    }

    public class Display1
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public struct Display1StructType
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    [ClrTypeMappingTestAttribute]
    public class Display2
    {
        public int X { get; set; }
        public int Y { get; set; }
        [ClrTypeMappingTestAttribute]
        public Display1 Origin { get; set; }
    }

    public struct Display2StructTypeWithObjectProperty
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Display1 Origin { get; set; }
    }

    public struct Display2StructTypeWithStructProperty
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Display1StructType Origin { get; set; }
    }

    public class RecursiveProperty
    {
        public int X { get; set; }
        public int Y { get; set; }
        public RecursiveProperty Origin { get; set; }
    }

    public class DerivedRecursiveProperty : RecursiveProperty
    {
        public new int X { get; set; }
        public new int Origin { get; set; }
    }

    public class ClassWithCollectionProperty
    {
        public IEnumerable<int> X { get; set; }
        public IList<int> Y { get; set; }
        public ICollection<int> Z { get; set; }
        public ICollection<Display1> C { get; set; }
    }

    public class ClassWithNullCollectionProperty
    {
        public IEnumerable<int> X { get { return null; } }
        public IList<int> Y { get { return null; } }
        public ICollection<int> Z { get { return null; } }
        public ICollection<Display1> C { get { return null; } }
    }

    public class ClassWithCollectionOfCollectionProperty
    {
        public ICollection<ICollection<int>> C { get; set; }
    }

    public enum EnumInt { Member1 = -1, Member2 }

    public enum EnumByte : byte { Member1, Member2 }

    private enum EnumULong : ulong { Member1 = 11, Member2 }

    private class ClassWithEnum
    {
        public EnumInt EnumInt { get; set; }
        public EnumByte EnumByte { get; set; }
        public EnumULong EnumULong { get; set; }
    }

    public class EmptyClass
    {

    }

    public abstract class AbstractClass
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class ClassWithStructProperty
    {
        public int X { get; set; }
        public Display1StructType Origin { get; set; }
    }

    public interface DisplayInterface
    {
        int X { get; set; }
        int Y { get; set; }
    }

    public class ClassWithInterfaceProperty
    {
        public DisplayInterface Origin { get; set; }
        public int X { get; set; }
    }

    public class ClassWithVirtualMember
    {
        public virtual int X { get; set; }
    }

    public class DerivedClassWithVirtualMember : ClassWithVirtualMember
    {
        public override int X { get; set; }
    }

    [AttributeUsage(AttributeTargets.All)]
    private class ClrTypeMappingTestAttribute : Attribute
    {
    }
}
