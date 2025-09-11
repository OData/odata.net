//---------------------------------------------------------------------
// <copyright file="EdmValueKindTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class EdmValueKindTests : EdmLibTestCaseBase
{
    [Fact]
    public void Validate_BinaryConstant_HasCorrectValueKind()
    {
        var constant = new EdmBinaryConstant(new byte[] { 1, 1 });
        Assert.Equal(EdmValueKind.Binary, constant.ValueKind);

        var valueKindType = EdmCoreModel.Instance.GetBinary(true);
        Assert.Equal(EdmValueKind.Binary, constant.ValueKind);

        var model = this.BuildVocabularyAnnotationModelWithEdmValueKind(valueKindType, constant as IEdmExpression);

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var csdl = this.GetSerializerResult(model);
        var isParsed = SchemaReader.TryParse(csdl.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel csdlModel, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        csdlModel.Validate(EdmConstants.EdmVersionLatest, out errors);
        Assert.Empty(errors);

        Assert.Single(csdlModel.VocabularyAnnotations);
        var resultValueAnnotation = csdlModel.VocabularyAnnotations.ElementAt(0);

        Assert.Equal(EdmValueKind.Binary, (resultValueAnnotation.Value as IEdmValue)?.ValueKind);
    }

    [Fact]
    public void Validate_EvaluationOfBinaryConstant_HasCorrectValueKind()
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Binary"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Binary>6E6761</Binary>
        </Annotation>
    </Annotations>
</Schema>";

        var isParsed = SchemaReader.TryParse((new string[] { csdl }).Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var valueAnnotation = model.VocabularyAnnotations.ElementAt(0);
        var expression = valueAnnotation.Value as IEdmBinaryConstantExpression;
        Assert.NotNull(expression);

        var expressionValue = VerifyExpressionEdmValueKind(expression, EdmValueKind.Binary);
        Assert.NotNull(expressionValue);
        Assert.Equal(EdmValueKind.Binary, expressionValue.ValueKind);
    }

    [Fact]
    public void Validate_BooleanConstant_HasCorrectValueKind()
    {
        var constant = new EdmBooleanConstant(true);
        Assert.Equal(EdmValueKind.Boolean, constant.ValueKind);

        var valueKindType = EdmCoreModel.Instance.GetBoolean(true);
        Assert.Equal(EdmValueKind.Boolean, constant.ValueKind);

        var model = this.BuildVocabularyAnnotationModelWithEdmValueKind(valueKindType, constant as IEdmExpression);

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var csdl = this.GetSerializerResult(model);
        var isParsed = SchemaReader.TryParse(csdl.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel csdlModel, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        csdlModel.Validate(EdmConstants.EdmVersionLatest, out errors);
        Assert.Empty(errors);

        Assert.Single(csdlModel.VocabularyAnnotations);
        var resultValueAnnotation = csdlModel.VocabularyAnnotations.ElementAt(0);

        Assert.Equal(EdmValueKind.Boolean, (resultValueAnnotation.Value as IEdmValue)?.ValueKind);
    }

    [Fact]
    public void Validate_EvaluationOfBooleanConstant_HasCorrectValueKind()
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Boolean"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Bool>true</Bool>
        </Annotation>
    </Annotations>
</Schema>";

        var isParsed = SchemaReader.TryParse((new string[] { csdl }).Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var valueAnnotation = model.VocabularyAnnotations.ElementAt(0);
        var expression = valueAnnotation.Value as IEdmBooleanConstantExpression;
        Assert.NotNull(expression);

        var expressionValue = VerifyExpressionEdmValueKind(expression, EdmValueKind.Boolean);
        Assert.NotNull(expressionValue);
        Assert.Equal(EdmValueKind.Boolean, expressionValue.ValueKind);
    }

    [Fact]
    public void Validate_CollectionValue_HasCorrectValueKind()
    {
        var constant = new EdmCollectionValue(EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false)), new IEdmDelayedValue[] { new EdmStringConstant("foo") });
        Assert.Equal(EdmValueKind.Collection, constant.ValueKind);
    }

    [Fact]
    public void Validate_EvaluationOfCollectionValue_HasCorrectValueKind()
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Friends"" Type=""Collection(Edm.String)"" Nullable=""true"" />
    </EntityType>
    <Term Name=""PersonTerm"" Type=""DefaultNamespace.Person"" />
    <Annotations Target=""DefaultNamespace.PersonTerm"">
        <Annotation Term=""DefaultNamespace.Person"">
            <Record>
                <PropertyValue Property=""Id"" Int=""1"" />
                <PropertyValue Property=""Friends"">
                    <Collection>
                        <String>foo</String>
                        <String>bar</String>
                    </Collection>
                </PropertyValue>
            </Record>
        </Annotation>
    </Annotations>
</Schema>";

        var isParsed = SchemaReader.TryParse((new string[] { csdl }).Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var annotation = model.VocabularyAnnotations.ElementAt(0);
        var friendsValueAnnotationProperty = ((IEdmRecordExpression)annotation.Value).Properties.Where(n => n.Name.Equals("Friends"));
        Assert.Single(friendsValueAnnotationProperty);

        var friendsExpression = friendsValueAnnotationProperty.ElementAt(0).Value as IEdmCollectionExpression;
        Assert.NotNull(friendsExpression);

        var expressionValue = VerifyExpressionEdmValueKind(friendsExpression, EdmValueKind.Collection);
        Assert.NotNull(expressionValue);
        Assert.Equal(EdmValueKind.Collection, expressionValue.ValueKind);
    }

    [Fact]
    public void Validate_TimeOfDayConstant_HasCorrectValueKind()
    {
        var constant = new EdmTimeOfDayConstant(new TimeOfDay());
        Assert.Equal(EdmValueKind.TimeOfDay, constant.ValueKind);

        var valueKindType = EdmCoreModel.Instance.GetTimeOfDay(true);
        Assert.Equal(EdmValueKind.TimeOfDay, constant.ValueKind);

        var model = this.BuildVocabularyAnnotationModelWithEdmValueKind(valueKindType, constant as IEdmExpression);

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var csdl = this.GetSerializerResult(model);
        var isParsed = SchemaReader.TryParse(csdl.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel csdlModel, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        csdlModel.Validate(EdmConstants.EdmVersionLatest, out errors);
        Assert.Empty(errors);

        Assert.Single(csdlModel.VocabularyAnnotations);
        var resultValueAnnotation = csdlModel.VocabularyAnnotations.ElementAt(0);

        Assert.Equal(EdmValueKind.TimeOfDay, (resultValueAnnotation.Value as IEdmValue)?.ValueKind);
    }

    [Fact]
    public void Validate_EvaluationOfTimeOfDayConstant_HasCorrectValueKind()
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.TimeOfDay"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <TimeOfDay>12:30:13.123</TimeOfDay>
        </Annotation>
    </Annotations>
</Schema>";

        var isParsed = SchemaReader.TryParse((new string[] { csdl }).Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var valueAnnotation = model.VocabularyAnnotations.ElementAt(0);
        var expression = valueAnnotation.Value as IEdmTimeOfDayConstantExpression;
        Assert.NotNull(expression);

        var expressionValue = VerifyExpressionEdmValueKind(expression, EdmValueKind.TimeOfDay);
        Assert.NotNull(expressionValue);
        Assert.Equal(EdmValueKind.TimeOfDay, expressionValue.ValueKind);
    }

    [Fact]
    public void Validate_DateConstant_HasCorrectValueKind()
    {
        var constant = new EdmDateConstant(new Date());
        Assert.Equal(EdmValueKind.Date, constant.ValueKind);

        var valueKindType = EdmCoreModel.Instance.GetDate(true);
        Assert.Equal(EdmValueKind.Date, constant.ValueKind);

        var model = this.BuildVocabularyAnnotationModelWithEdmValueKind(valueKindType, constant as IEdmExpression);

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var csdl = this.GetSerializerResult(model);
        var isParsed = SchemaReader.TryParse(csdl.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel csdlModel, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        csdlModel.Validate(EdmConstants.EdmVersionLatest, out errors);
        Assert.Empty(errors);

        Assert.Single(csdlModel.VocabularyAnnotations);
        var resultValueAnnotation = csdlModel.VocabularyAnnotations.ElementAt(0);

        Assert.Equal(EdmValueKind.Date, (resultValueAnnotation.Value as IEdmValue)?.ValueKind);
    }

    [Fact]
    public void Validate_EvaluationOfDateConstant_HasCorrectValueKind()
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Date"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Date>2014-08-13</Date>
        </Annotation>
    </Annotations>
</Schema>";

        var isParsed = SchemaReader.TryParse((new string[] { csdl }).Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var valueAnnotation = model.VocabularyAnnotations.ElementAt(0);
        var expression = valueAnnotation.Value as IEdmDateConstantExpression;
        Assert.NotNull(expression);

        var expressionValue = VerifyExpressionEdmValueKind(expression, EdmValueKind.Date);
        Assert.NotNull(expressionValue);
        Assert.Equal(EdmValueKind.Date, expressionValue.ValueKind);
    }

    [Fact]
    public void Validate_DateTimeOffsetConstant_HasCorrectValueKind()
    {
        var constant = new EdmDateTimeOffsetConstant(DateTimeOffset.MinValue);
        Assert.Equal(EdmValueKind.DateTimeOffset, constant.ValueKind);

        var valueKindType = EdmCoreModel.Instance.GetDateTimeOffset(true);
        Assert.Equal(EdmValueKind.DateTimeOffset, constant.ValueKind);

        var model = this.BuildVocabularyAnnotationModelWithEdmValueKind(valueKindType, constant as IEdmExpression);

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var csdl = this.GetSerializerResult(model);
        var isParsed = SchemaReader.TryParse(csdl.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel csdlModel, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        csdlModel.Validate(EdmConstants.EdmVersionLatest, out errors);
        Assert.Empty(errors);

        Assert.Single(csdlModel.VocabularyAnnotations);
        var resultValueAnnotation = csdlModel.VocabularyAnnotations.ElementAt(0);

        Assert.Equal(EdmValueKind.DateTimeOffset, (resultValueAnnotation.Value as IEdmValue)?.ValueKind);
    }

    [Fact]
    public void Validate_EvaluationOfDateTimeOffsetConstant_HasCorrectValueKind()
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.DateTimeOffset"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <DateTimeOffset>2011-01-01T23:59:00-07:00</DateTimeOffset>
        </Annotation>
    </Annotations>
</Schema>";

        var isParsed = SchemaReader.TryParse((new string[] { csdl }).Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var valueAnnotation = model.VocabularyAnnotations.ElementAt(0);
        var expression = valueAnnotation.Value as IEdmDateTimeOffsetConstantExpression;
        Assert.NotNull(expression);

        var expressionValue = VerifyExpressionEdmValueKind(expression, EdmValueKind.DateTimeOffset);
        Assert.NotNull(expressionValue);
        Assert.Equal(EdmValueKind.DateTimeOffset, expressionValue.ValueKind);
    }

    [Fact]
    public void Validate_DecimalConstant_HasCorrectValueKind()
    {
        var constant = new EdmDecimalConstant(new decimal(1.12));
        Assert.Equal(EdmValueKind.Decimal, constant.ValueKind);

        var valueKindType = EdmCoreModel.Instance.GetDecimal(true);
        Assert.Equal(EdmValueKind.Decimal, constant.ValueKind);

        var model = this.BuildVocabularyAnnotationModelWithEdmValueKind(valueKindType, constant as IEdmExpression);

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var csdl = this.GetSerializerResult(model);
        var isParsed = SchemaReader.TryParse(csdl.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel csdlModel, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        csdlModel.Validate(EdmConstants.EdmVersionLatest, out errors);
        Assert.Empty(errors);

        Assert.Single(csdlModel.VocabularyAnnotations);
        var resultValueAnnotation = csdlModel.VocabularyAnnotations.ElementAt(0);

        Assert.Equal(EdmValueKind.Decimal, (resultValueAnnotation.Value as IEdmValue)?.ValueKind);
    }

    [Fact]
    public void Validate_EvaluationOfDecimalConstant_HasCorrectValueKind()
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Decimal"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Decimal>0.12</Decimal>
        </Annotation>
    </Annotations>
</Schema>";

        var isParsed = SchemaReader.TryParse((new string[] { csdl }).Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var valueAnnotation = model.VocabularyAnnotations.ElementAt(0);
        var expression = valueAnnotation.Value as IEdmDecimalConstantExpression;
        Assert.NotNull(expression);

        var expressionValue = VerifyExpressionEdmValueKind(expression, EdmValueKind.Decimal);
        Assert.NotNull(expressionValue);
        Assert.Equal(EdmValueKind.Decimal, expressionValue.ValueKind);
    }

    [Fact]
    public void Validate_EnumType_HasCorrectValueKind()
    {
        var enumType = new EdmEnumType("", "", EdmCoreModel.Instance.GetInt32(true).PrimitiveDefinition(), true);
        var enumMember = enumType.AddMember("foo", new EdmEnumMemberValue(10));
        var constant = new EdmEnumValue(new EdmEnumTypeReference(enumType, false), enumMember);
        Assert.Equal(EdmValueKind.Enum, constant.ValueKind);

        Assert.Throws<ArgumentNullException>(() => new EdmEnumType(null, "", EdmCoreModel.Instance.GetInt32(true).PrimitiveDefinition(), true));
        Assert.Throws<ArgumentNullException>(() => new EdmEnumType("", null, EdmCoreModel.Instance.GetInt32(true).PrimitiveDefinition(), true));
        Assert.Throws<ArgumentNullException>(() => new EdmEnumType("", "", null, true));
    }

    [Fact]
    public void Validate_FloatingConstant_Single_HasCorrectValueKind()
    {
        var constant = new EdmFloatingConstant(11.11);
        Assert.Equal(EdmValueKind.Floating, constant.ValueKind);

        var valueKindType = EdmCoreModel.Instance.GetSingle(true);
        Assert.Equal(EdmValueKind.Floating, constant.ValueKind);

        var model = this.BuildVocabularyAnnotationModelWithEdmValueKind(valueKindType, constant as IEdmExpression);

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var csdl = this.GetSerializerResult(model);
        var isParsed = SchemaReader.TryParse(csdl.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel csdlModel, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        csdlModel.Validate(EdmConstants.EdmVersionLatest, out errors);
        Assert.Empty(errors);

        Assert.Single(csdlModel.VocabularyAnnotations);
        var resultValueAnnotation = csdlModel.VocabularyAnnotations.ElementAt(0);

        Assert.Equal(EdmValueKind.Floating, (resultValueAnnotation.Value as IEdmValue)?.ValueKind);
    }

    [Fact]
    public void Validate_FloatingConstant_Double_HasCorrectValueKind()
    {
        var constant = new EdmFloatingConstant(11.11);
        Assert.Equal(EdmValueKind.Floating, constant.ValueKind);

        var valueKindType = EdmCoreModel.Instance.GetDouble(true);
        Assert.Equal(EdmValueKind.Floating, constant.ValueKind);

        var model = this.BuildVocabularyAnnotationModelWithEdmValueKind(valueKindType, constant as IEdmExpression);

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var csdl = this.GetSerializerResult(model);
        var isParsed = SchemaReader.TryParse(csdl.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel csdlModel, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        csdlModel.Validate(EdmConstants.EdmVersionLatest, out errors);
        Assert.Empty(errors);

        Assert.Single(csdlModel.VocabularyAnnotations);
        var resultValueAnnotation = csdlModel.VocabularyAnnotations.ElementAt(0);

        Assert.Equal(EdmValueKind.Floating, (resultValueAnnotation.Value as IEdmValue)?.ValueKind);
    }

    [Fact]
    public void Validate_EvaluationOfFloatingConstant_HasCorrectValueKind()
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Double"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Float>12.12</Float>
        </Annotation>
    </Annotations>
</Schema>";

        var isParsed = SchemaReader.TryParse((new string[] { csdl }).Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var valueAnnotation = model.VocabularyAnnotations.ElementAt(0);
        var expression = valueAnnotation.Value as IEdmFloatingConstantExpression;
        Assert.NotNull(expression);

        var expressionValue = VerifyExpressionEdmValueKind(expression, EdmValueKind.Floating);
        Assert.NotNull(expressionValue);
        Assert.Equal(EdmValueKind.Floating, expressionValue.ValueKind);
    }

    [Fact]
    public void Validate_GuidConstant_HasCorrectValueKind()
    {
        var constant = new EdmGuidConstant(new Guid());
        Assert.Equal(EdmValueKind.Guid, constant.ValueKind);

        var valueKindType = EdmCoreModel.Instance.GetGuid(true);
        Assert.Equal(EdmValueKind.Guid, constant.ValueKind);

        var model = this.BuildVocabularyAnnotationModelWithEdmValueKind(valueKindType, constant as IEdmExpression);

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var csdl = this.GetSerializerResult(model);
        var isParsed = SchemaReader.TryParse(csdl.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel csdlModel, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        csdlModel.Validate(EdmConstants.EdmVersionLatest, out errors);
        Assert.Empty(errors);

        Assert.Single(csdlModel.VocabularyAnnotations);
        var resultValueAnnotation = csdlModel.VocabularyAnnotations.ElementAt(0);

        Assert.Equal(EdmValueKind.Guid, (resultValueAnnotation.Value as IEdmValue)?.ValueKind);
    }

    [Fact]
    public void Validate_EvaluationOfGuidConstant_HasCorrectValueKind()
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Guid"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Guid>707043F1-E7DD-475C-9928-71DA38EA7D57</Guid>
        </Annotation>
    </Annotations>
</Schema>";

        var isParsed = SchemaReader.TryParse((new string[] { csdl }).Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var valueAnnotation = model.VocabularyAnnotations.ElementAt(0);
        var expression = valueAnnotation.Value as IEdmGuidConstantExpression;
        Assert.NotNull(expression);

        var expressionValue = VerifyExpressionEdmValueKind(expression, EdmValueKind.Guid);
        Assert.NotNull(expressionValue);
        Assert.Equal(EdmValueKind.Guid, expressionValue.ValueKind);
    }

    [Fact]
    public void Validate_IntegerConstant_Byte_HasCorrectValueKind()
    {
        var constant = new EdmIntegerConstant(0);
        Assert.Equal(EdmValueKind.Integer, constant.ValueKind);

        var valueKindType = EdmCoreModel.Instance.GetByte(true);
        Assert.Equal(EdmValueKind.Integer, constant.ValueKind);

        var model = this.BuildVocabularyAnnotationModelWithEdmValueKind(valueKindType, constant as IEdmExpression);

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var csdl = this.GetSerializerResult(model);
        var isParsed = SchemaReader.TryParse(csdl.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel csdlModel, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        csdlModel.Validate(EdmConstants.EdmVersionLatest, out errors);
        Assert.Empty(errors);

        Assert.Single(csdlModel.VocabularyAnnotations);
        var resultValueAnnotation = csdlModel.VocabularyAnnotations.ElementAt(0);

        Assert.Equal(EdmValueKind.Integer, (resultValueAnnotation.Value as IEdmValue)?.ValueKind);
    }

    [Fact]
    public void Validate_IntegerConstant_SByte_HasCorrectValueKind()
    {
        var constant = new EdmIntegerConstant(0);
        Assert.Equal(EdmValueKind.Integer, constant.ValueKind);

        var valueKindType = EdmCoreModel.Instance.GetSByte(true);
        Assert.Equal(EdmValueKind.Integer, constant.ValueKind);

        var model = this.BuildVocabularyAnnotationModelWithEdmValueKind(valueKindType, constant as IEdmExpression);

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var csdl = this.GetSerializerResult(model);
        var isParsed = SchemaReader.TryParse(csdl.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel csdlModel, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        csdlModel.Validate(EdmConstants.EdmVersionLatest, out errors);
        Assert.Empty(errors);

        Assert.Single(csdlModel.VocabularyAnnotations);
        var resultValueAnnotation = csdlModel.VocabularyAnnotations.ElementAt(0);

        Assert.Equal(EdmValueKind.Integer, (resultValueAnnotation.Value as IEdmValue)?.ValueKind);
    }

    [Fact]
    public void Validate_IntegerConstant_Int16_HasCorrectValueKind()
    {
        var constant = new EdmIntegerConstant(0);
        Assert.Equal(EdmValueKind.Integer, constant.ValueKind);

        var valueKindType = EdmCoreModel.Instance.GetInt16(true);
        Assert.Equal(EdmValueKind.Integer, constant.ValueKind);

        var model = this.BuildVocabularyAnnotationModelWithEdmValueKind(valueKindType, constant as IEdmExpression);

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var csdl = this.GetSerializerResult(model);
        var isParsed = SchemaReader.TryParse(csdl.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel csdlModel, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        csdlModel.Validate(EdmConstants.EdmVersionLatest, out errors);
        Assert.Empty(errors);

        Assert.Single(csdlModel.VocabularyAnnotations);
        var resultValueAnnotation = csdlModel.VocabularyAnnotations.ElementAt(0);

        Assert.Equal(EdmValueKind.Integer, (resultValueAnnotation.Value as IEdmValue)?.ValueKind);
    }

    [Fact]
    public void Validate_IntegerConstant_Int32_HasCorrectValueKind()
    {
        var constant = new EdmIntegerConstant(0);
        Assert.Equal(EdmValueKind.Integer, constant.ValueKind);

        var valueKindType = EdmCoreModel.Instance.GetInt32(true);
        Assert.Equal(EdmValueKind.Integer, constant.ValueKind);

        var model = this.BuildVocabularyAnnotationModelWithEdmValueKind(valueKindType, constant as IEdmExpression);

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var csdl = this.GetSerializerResult(model);
        var isParsed = SchemaReader.TryParse(csdl.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel csdlModel, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        csdlModel.Validate(EdmConstants.EdmVersionLatest, out errors);
        Assert.Empty(errors);

        Assert.Single(csdlModel.VocabularyAnnotations);
        var resultValueAnnotation = csdlModel.VocabularyAnnotations.ElementAt(0);

        Assert.Equal(EdmValueKind.Integer, (resultValueAnnotation.Value as IEdmValue)?.ValueKind);
    }

    [Fact]
    public void Validate_IntegerConstant_Int64_HasCorrectValueKind()
    {
        var constant = new EdmIntegerConstant(0);
        Assert.Equal(EdmValueKind.Integer, constant.ValueKind);

        var valueKindType = EdmCoreModel.Instance.GetInt64(true);
        Assert.Equal(EdmValueKind.Integer, constant.ValueKind);

        var model = this.BuildVocabularyAnnotationModelWithEdmValueKind(valueKindType, constant as IEdmExpression);

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var csdl = this.GetSerializerResult(model);
        var isParsed = SchemaReader.TryParse(csdl.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel csdlModel, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        csdlModel.Validate(EdmConstants.EdmVersionLatest, out errors);
        Assert.Empty(errors);

        Assert.Single(csdlModel.VocabularyAnnotations);
        var resultValueAnnotation = csdlModel.VocabularyAnnotations.ElementAt(0);

        Assert.Equal(EdmValueKind.Integer, (resultValueAnnotation.Value as IEdmValue)?.ValueKind);
    }

    [Fact]
    public void Validate_EvaluationOfIntegerConstant_HasCorrectValueKind()
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Int32"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Int>1</Int>
        </Annotation>
    </Annotations>
</Schema>";

        var isParsed = SchemaReader.TryParse((new string[] { csdl }).Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var valueAnnotation = model.VocabularyAnnotations.ElementAt(0);
        var expression = valueAnnotation.Value as IEdmIntegerConstantExpression;
        Assert.NotNull(expression);

        var expressionValue = VerifyExpressionEdmValueKind(expression, EdmValueKind.Integer);
        Assert.NotNull(expressionValue);
        Assert.Equal(EdmValueKind.Integer, expressionValue.ValueKind);
    }

    [Fact]
    public void Validate_StringConstant_HasCorrectValueKind()
    {
        var constant = new EdmStringConstant("foo");
        Assert.Equal(EdmValueKind.String, constant.ValueKind);

        var valueKindType = EdmCoreModel.Instance.GetString(true);
        Assert.Equal(EdmValueKind.String, constant.ValueKind);

        var model = this.BuildVocabularyAnnotationModelWithEdmValueKind(valueKindType, constant as IEdmExpression);

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var csdl = this.GetSerializerResult(model);
        var isParsed = SchemaReader.TryParse(csdl.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel csdlModel, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        csdlModel.Validate(EdmConstants.EdmVersionLatest, out errors);
        Assert.Empty(errors);

        Assert.Single(csdlModel.VocabularyAnnotations);
        var resultValueAnnotation = csdlModel.VocabularyAnnotations.ElementAt(0);

        Assert.Equal(EdmValueKind.String, (resultValueAnnotation.Value as IEdmValue)?.ValueKind);
    }

    [Fact]
    public void Validate_EvaluationOfStringConstant_HasCorrectValueKind()
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.String"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <String>foo</String>
        </Annotation>
    </Annotations>
</Schema>";

        var isParsed = SchemaReader.TryParse((new string[] { csdl }).Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var valueAnnotation = model.VocabularyAnnotations.ElementAt(0);
        var expression = valueAnnotation.Value as IEdmStringConstantExpression;
        Assert.NotNull(expression);

        var expressionValue = VerifyExpressionEdmValueKind(expression, EdmValueKind.String);
        Assert.NotNull(expressionValue);
        Assert.Equal(EdmValueKind.String, expressionValue.ValueKind);
    }

    [Fact]
    public void Validate_StructuredValue_HasCorrectValueKind()
    {
        var constant = new EdmStructuredValue(null, new List<IEdmPropertyValue>());
        Assert.Equal(EdmValueKind.Structured, constant.ValueKind);
    }

    [Fact]
    public void Validate_EvaluationOfStructuredValue_HasCorrectValueKind()
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""DefaultNamespace.PersonName"" />
    </EntityType>
    <ComplexType Name=""PersonName"">
        <Property Name=""Name"" Type=""Edm.String"" />
    </ComplexType>
    <Term Name=""PersonTerm"" Type=""DefaultNamespace.Person"" />
    <Annotations Target=""DefaultNamespace.Person"">
        <Annotation Term=""DefaultNamespace.PersonTerm"">
            <Record>
                <PropertyValue Property=""Id"" Int=""1"" />
                <PropertyValue Property=""PersonName"">
                    <Record>
                        <PropertyValue Property=""Name"" String=""foo"" />
                    </Record>
                </PropertyValue>
            </Record>
        </Annotation>
    </Annotations>
</Schema>";

        var isParsed = SchemaReader.TryParse((new string[] { csdl }).Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var annotation = model.VocabularyAnnotations.ElementAt(0);
        var nameValueAnnotationProperty = ((IEdmRecordExpression)annotation.Value).Properties.Where(n => n.Name.Equals("PersonName"));
        Assert.Single(nameValueAnnotationProperty);

        var nameExpression = nameValueAnnotationProperty.ElementAt(0).Value as IEdmRecordExpression;
        Assert.NotNull(nameExpression);

        var expressionValue = VerifyExpressionEdmValueKind(nameExpression, EdmValueKind.Structured);
        Assert.NotNull(expressionValue);
        Assert.Equal(EdmValueKind.Structured, expressionValue.ValueKind);
    }

    [Fact]
    public void Validate_DurationConstant_HasCorrectValueKind()
    {
        var constant = new EdmDurationConstant(new TimeSpan());
        Assert.Equal(EdmValueKind.Duration, constant.ValueKind);

        var valueKindType = EdmCoreModel.Instance.GetDuration(true);
        Assert.Equal(EdmValueKind.Duration, constant.ValueKind);

        var model = this.BuildVocabularyAnnotationModelWithEdmValueKind(valueKindType, constant as IEdmExpression);

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var csdl = this.GetSerializerResult(model);
        var isParsed = SchemaReader.TryParse(csdl.Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel csdlModel, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        csdlModel.Validate(EdmConstants.EdmVersionLatest, out errors);
        Assert.Empty(errors);

        Assert.Single(csdlModel.VocabularyAnnotations);
        var resultValueAnnotation = csdlModel.VocabularyAnnotations.ElementAt(0);

        Assert.Equal(EdmValueKind.Duration, (resultValueAnnotation.Value as IEdmValue)?.ValueKind);
    }

    [Fact]
    public void Validate_EvaluationOfDurationConstant_HasCorrectValueKind()
    {
        var csdl = @"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.Duration"" />
    <Annotations Target=""DefaultNamespace.Note"">
        <Annotation Term=""DefaultNamespace.Note"">
            <Duration>PT1S</Duration>
        </Annotation>
    </Annotations>
</Schema>";

        var isParsed = SchemaReader.TryParse((new string[] { csdl }).Select(XElement.Parse).Select(e => e.CreateReader()), out IEdmModel model, out IEnumerable<EdmError> parsedErrors);
        Assert.True(isParsed);
        Assert.False(parsedErrors.Any());

        model.Validate(EdmConstants.EdmVersionLatest, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);

        var valueAnnotation = model.VocabularyAnnotations.ElementAt(0);
        var expression = valueAnnotation.Value as IEdmDurationConstantExpression;
        Assert.NotNull(expression);

        var expressionValue = VerifyExpressionEdmValueKind(expression, EdmValueKind.Duration);
        Assert.NotNull(expressionValue);
        Assert.Equal(EdmValueKind.Duration, expressionValue.ValueKind);
    }

    #region Private

    private IEdmValue VerifyExpressionEdmValueKind(IEdmExpression expression, EdmValueKind expressionKind)
    {
        Dictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>> builtInFunctions = new Dictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>>();
        var evaluator = new EdmExpressionEvaluator(builtInFunctions);

        return evaluator.Evaluate(expression) as IEdmValue;
    }

    private IEdmModel BuildVocabularyAnnotationModelWithEdmValueKind(IEdmTypeReference valueKindType, IEdmExpression valueAnnotationValue)
    {
        var model = this.BuildBasicModelWithTerm(valueKindType);

        var valueAnnotation = new EdmVocabularyAnnotation(
            model.FindEntityContainer("foo.Container"),
            model.FindTerm("foo.ValueTerm"),
            valueAnnotationValue);
        model.AddVocabularyAnnotation(valueAnnotation);

        return model;
    }

    private EdmModel BuildBasicModelWithTerm(IEdmTypeReference valueKindType)
    {
        var model = new EdmModel();
        var container = new EdmEntityContainer("foo", "Container");
        model.AddElement(container);

        var valueTerm = new EdmTerm("foo", "ValueTerm", valueKindType);
        model.AddElement(valueTerm);

        return model;
    }

    private class DummyEdmValue : IEdmValue
    {
        public IEdmTypeReference Type
        {
            get { throw new NotImplementedException(); }
        }

        public EdmValueKind ValueKind
        {
            get { return EdmValueKind.None; }
        }
    }

    #endregion
}