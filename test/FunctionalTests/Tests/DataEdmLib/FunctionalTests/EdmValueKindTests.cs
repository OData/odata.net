//---------------------------------------------------------------------
// <copyright file="EdmValueKindTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EdmValueKindTests : EdmLibTestCaseBase
    {
        [TestMethod]
        public void TestBinaryConstant()
        {
            var constant = new EdmBinaryConstant(new byte[] { 1, 1 });
            Assert.AreEqual(EdmValueKind.Binary, constant.ValueKind, "Invalid value kind.");

            this.ValidateEdmValueKindRoundTrip(EdmValueKind.Binary, EdmCoreModel.Instance.GetBinary(true), constant);
        }

        [TestMethod]
        public void TestEvaluationOfBinaryConstant()
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

            var model = this.GetParserResult(new string[] { csdl });

            IEnumerable<EdmError> errors;
            model.Validate(EdmConstants.EdmVersionLatest, out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid model validation error count.");

            var valueAnnotation = model.VocabularyAnnotations.ElementAt(0);
            var expression = valueAnnotation.Value as IEdmBinaryConstantExpression;
            VerifyExpressionEdmValueKind(expression, EdmValueKind.Binary);
        }

        [TestMethod]
        public void TestBooleanConstant()
        {
            var constant = new EdmBooleanConstant(true);
            Assert.AreEqual(EdmValueKind.Boolean, constant.ValueKind, "Invalid value kind.");

            this.ValidateEdmValueKindRoundTrip(EdmValueKind.Boolean, EdmCoreModel.Instance.GetBoolean(true), constant);
        }

        [TestMethod]
        public void TestEvaluationOfBooleanConstant()
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

            var model = this.GetParserResult(new string[] { csdl });

            IEnumerable<EdmError> errors;
            model.Validate(EdmConstants.EdmVersionLatest, out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid model validation error count.");

            var valueAnnotation = model.VocabularyAnnotations.ElementAt(0);
            var expression = valueAnnotation.Value as IEdmBooleanConstantExpression;
            VerifyExpressionEdmValueKind(expression, EdmValueKind.Boolean);
        }

        [TestMethod]
        public void TestCollectionValue()
        {
            var constant = new EdmCollectionValue(EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false)), new IEdmDelayedValue[] { new EdmStringConstant("foo") });
            Assert.AreEqual(EdmValueKind.Collection, constant.ValueKind, "Invalid value kind.");
        }

        [TestMethod]
        public void TestEvaluationOfCollectionValue()
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

            var model = this.GetParserResult(new string[] { csdl });

            IEnumerable<EdmError> errors;
            model.Validate(EdmConstants.EdmVersionLatest, out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid model validation error count.");

            var annotation = model.VocabularyAnnotations.ElementAt(0);
            var friendsValueAnnotationProperty = ((IEdmRecordExpression)annotation.Value).Properties.Where(n => n.Name.Equals("Friends"));
            Assert.AreEqual(1, friendsValueAnnotationProperty.Count(), "Invalid type annotation property.");

            var friendsExpression = friendsValueAnnotationProperty.ElementAt(0).Value as IEdmCollectionExpression;
            VerifyExpressionEdmValueKind(friendsExpression, EdmValueKind.Collection);
        }

        [TestMethod]
        public void TestTimeOfDayConstant()
        {
            var constant = new EdmTimeOfDayConstant(new TimeOfDay());
            Assert.AreEqual(EdmValueKind.TimeOfDay, constant.ValueKind, "Invalid value kind.");

            this.ValidateEdmValueKindRoundTrip(EdmValueKind.TimeOfDay, EdmCoreModel.Instance.GetTimeOfDay(true), constant);
        }

        [TestMethod]
        public void TestEvaluationOfTimeOfDayConstant()
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

            var model = this.GetParserResult(new string[] { csdl });

            IEnumerable<EdmError> errors;
            model.Validate(EdmConstants.EdmVersionLatest, out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid model validation error count.");

            var valueAnnotation = model.VocabularyAnnotations.ElementAt(0);
            var expression = valueAnnotation.Value as IEdmTimeOfDayConstantExpression;
            VerifyExpressionEdmValueKind(expression, EdmValueKind.TimeOfDay);
        }

        [TestMethod]
        public void TestDateConstant()
        {
            var constant = new EdmDateConstant(new Date());
            Assert.AreEqual(EdmValueKind.Date, constant.ValueKind, "Invalid value kind.");

            this.ValidateEdmValueKindRoundTrip(EdmValueKind.Date, EdmCoreModel.Instance.GetDate(true), constant);
        }

        [TestMethod]
        public void TestEvaluationOfDateConstant()
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

            var model = this.GetParserResult(new string[] { csdl });

            IEnumerable<EdmError> errors;
            model.Validate(EdmConstants.EdmVersionLatest, out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid model validation error count.");

            var valueAnnotation = model.VocabularyAnnotations.ElementAt(0);
            var expression = valueAnnotation.Value as IEdmDateConstantExpression;
            VerifyExpressionEdmValueKind(expression, EdmValueKind.Date);
        }

        [TestMethod]
        public void TestDateTimeOffsetConstant()
        {
            var constant = new EdmDateTimeOffsetConstant(DateTimeOffset.MinValue);
            Assert.AreEqual(EdmValueKind.DateTimeOffset, constant.ValueKind, "Invalid value kind.");

            this.ValidateEdmValueKindRoundTrip(EdmValueKind.DateTimeOffset, EdmCoreModel.Instance.GetDateTimeOffset(true), constant);
        }

        [TestMethod]
        public void TestEvaluationOfDateTimeOffsetConstant()
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

            var model = this.GetParserResult(new string[] { csdl });

            IEnumerable<EdmError> errors;
            model.Validate(EdmConstants.EdmVersionLatest, out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid model validation error count.");

            var valueAnnotation = model.VocabularyAnnotations.ElementAt(0);
            var expression = valueAnnotation.Value as IEdmDateTimeOffsetConstantExpression;
            VerifyExpressionEdmValueKind(expression, EdmValueKind.DateTimeOffset);
        }

        [TestMethod]
        public void TestDecimalConstant()
        {
            var constant = new EdmDecimalConstant(new decimal(1.12));
            Assert.AreEqual(EdmValueKind.Decimal, constant.ValueKind, "Invalid value kind.");

            this.ValidateEdmValueKindRoundTrip(EdmValueKind.Decimal, EdmCoreModel.Instance.GetDecimal(true), constant);
        }

        [TestMethod]
        public void TestEvaluationOfDecimalConstant()
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

            var model = this.GetParserResult(new string[] { csdl });

            IEnumerable<EdmError> errors;
            model.Validate(EdmConstants.EdmVersionLatest, out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid model validation error count.");

            var valueAnnotation = model.VocabularyAnnotations.ElementAt(0);
            var expression = valueAnnotation.Value as IEdmDecimalConstantExpression;
            VerifyExpressionEdmValueKind(expression, EdmValueKind.Decimal);
        }

        [TestMethod]
        public void TestEnumType()
        {
            var enumType = new EdmEnumType("", "", EdmCoreModel.Instance.GetInt32(true).PrimitiveDefinition(), true);
            var enumMember = enumType.AddMember("foo", new EdmEnumMemberValue(10));
            var constant = new EdmEnumValue(new EdmEnumTypeReference(enumType, false), enumMember);
            Assert.AreEqual(EdmValueKind.Enum, constant.ValueKind, "Invalid value kind.");

            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmEnumType(null, "", EdmCoreModel.Instance.GetInt32(true).PrimitiveDefinition(), true));
            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmEnumType("", null, EdmCoreModel.Instance.GetInt32(true).PrimitiveDefinition(), true));
            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmEnumType("", "", null, true));
        }

        [TestMethod]
        public void TestFloatingConstant()
        {
            var constant = new EdmFloatingConstant(11.11);
            Assert.AreEqual(EdmValueKind.Floating, constant.ValueKind, "Invalid value kind.");

            this.ValidateEdmValueKindRoundTrip(EdmValueKind.Floating, EdmCoreModel.Instance.GetDouble(true), constant);
            this.ValidateEdmValueKindRoundTrip(EdmValueKind.Floating, EdmCoreModel.Instance.GetSingle(true), constant);
        }

        [TestMethod]
        public void TestEvaluationOfFloatingConstant()
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

            var model = this.GetParserResult(new string[] { csdl });

            IEnumerable<EdmError> errors;
            model.Validate(EdmConstants.EdmVersionLatest, out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid model validation error count.");

            var valueAnnotation = model.VocabularyAnnotations.ElementAt(0);
            var expression = valueAnnotation.Value as IEdmFloatingConstantExpression;
            VerifyExpressionEdmValueKind(expression, EdmValueKind.Floating);
        }

        [TestMethod]
        public void TestGuidConstant()
        {
            var constant = new EdmGuidConstant(new Guid());
            Assert.AreEqual(EdmValueKind.Guid, constant.ValueKind, "Invalid value kind.");

            this.ValidateEdmValueKindRoundTrip(EdmValueKind.Guid, EdmCoreModel.Instance.GetGuid(true), constant);
        }

        [TestMethod]
        public void TestEvaluationOfGuidConstant()
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

            var model = this.GetParserResult(new string[] { csdl });

            IEnumerable<EdmError> errors;
            model.Validate(EdmConstants.EdmVersionLatest, out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid model validation error count.");

            var valueAnnotation = model.VocabularyAnnotations.ElementAt(0);
            var expression = valueAnnotation.Value as IEdmGuidConstantExpression;
            VerifyExpressionEdmValueKind(expression, EdmValueKind.Guid);
        }

        [TestMethod]
        public void TestIntegerConstant()
        {
            var constant = new EdmIntegerConstant(0);
            Assert.AreEqual(EdmValueKind.Integer, constant.ValueKind, "Invalid value kind.");

            this.ValidateEdmValueKindRoundTrip(EdmValueKind.Integer, EdmCoreModel.Instance.GetByte(true), constant);
            this.ValidateEdmValueKindRoundTrip(EdmValueKind.Integer, EdmCoreModel.Instance.GetSByte(true), constant);
            this.ValidateEdmValueKindRoundTrip(EdmValueKind.Integer, EdmCoreModel.Instance.GetInt16(true), constant);
            this.ValidateEdmValueKindRoundTrip(EdmValueKind.Integer, EdmCoreModel.Instance.GetInt32(true), constant);
            this.ValidateEdmValueKindRoundTrip(EdmValueKind.Integer, EdmCoreModel.Instance.GetInt64(true), constant);
        }

        [TestMethod]
        public void TestEvaluationOfIntegerConstant()
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

            var model = this.GetParserResult(new string[] { csdl });

            IEnumerable<EdmError> errors;
            model.Validate(EdmConstants.EdmVersionLatest, out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid model validation error count.");

            var valueAnnotation = model.VocabularyAnnotations.ElementAt(0);
            var expression = valueAnnotation.Value as IEdmIntegerConstantExpression;
            VerifyExpressionEdmValueKind(expression, EdmValueKind.Integer);
        }

        [TestMethod]
        public void TestStringConstant()
        {
            var constant = new EdmStringConstant("foo");
            Assert.AreEqual(EdmValueKind.String, constant.ValueKind, "Invalid value kind.");

            this.ValidateEdmValueKindRoundTrip(EdmValueKind.String, EdmCoreModel.Instance.GetString(true), constant);
        }

        [TestMethod]
        public void TestEvaluationOfStringConstant()
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

            var model = this.GetParserResult(new string[] { csdl });

            IEnumerable<EdmError> errors;
            model.Validate(EdmConstants.EdmVersionLatest, out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid model validation error count.");

            var valueAnnotation = model.VocabularyAnnotations.ElementAt(0);
            var expression = valueAnnotation.Value as IEdmStringConstantExpression;
            VerifyExpressionEdmValueKind(expression, EdmValueKind.String);
        }

        [TestMethod]
        public void TestStructuredValue()
        {
            var constant = new EdmStructuredValue(null, new List<IEdmPropertyValue>());
            Assert.AreEqual(EdmValueKind.Structured, constant.ValueKind, "Invalid value kind.");
        }

        [TestMethod]
        public void TestEvaluationOfStructuredValue()
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

            var model = this.GetParserResult(new string[] { csdl });

            IEnumerable<EdmError> errors;
            model.Validate(EdmConstants.EdmVersionLatest, out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid model validation error count.");

            var annotation = model.VocabularyAnnotations.ElementAt(0);
            var nameValueAnnotationProperty = ((IEdmRecordExpression)annotation.Value).Properties.Where(n => n.Name.Equals("PersonName"));
            Assert.AreEqual(1, nameValueAnnotationProperty.Count(), "Invalid type annotation property.");

            var nameExpression = nameValueAnnotationProperty.ElementAt(0).Value as IEdmRecordExpression;
            VerifyExpressionEdmValueKind(nameExpression, EdmValueKind.Structured);
        }

        [TestMethod]
        public void TestDurationConstant()
        {
            var constant = new EdmDurationConstant(new TimeSpan());
            Assert.AreEqual(EdmValueKind.Duration, constant.ValueKind, "Invalid value kind.");

            this.ValidateEdmValueKindRoundTrip(EdmValueKind.Duration, EdmCoreModel.Instance.GetDuration(true), constant);
        }

        [TestMethod]
        public void TestEvaluationOfDurationConstant()
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

            var model = this.GetParserResult(new string[] { csdl });

            IEnumerable<EdmError> errors;
            model.Validate(EdmConstants.EdmVersionLatest, out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid model validation error count.");

            var valueAnnotation = model.VocabularyAnnotations.ElementAt(0);
            var expression = valueAnnotation.Value as IEdmDurationConstantExpression;
            VerifyExpressionEdmValueKind(expression, EdmValueKind.Duration);
        }

        private void VerifyExpressionEdmValueKind(IEdmExpression expression, EdmValueKind expressionKind)
        {
            Dictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>> builtInFunctions = new Dictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>>();
            var evaluator = new EdmExpressionEvaluator(builtInFunctions);

            var expressionValue = evaluator.Evaluate(expression) as IEdmValue;
            Assert.IsNotNull(expressionValue, "Invalid expression value.");
            Assert.AreEqual(expressionKind, expressionValue.ValueKind, "Invalid expression value kind.");
        }

        private void ValidateEdmValueKindRoundTrip(EdmValueKind valueKind, IEdmTypeReference valueKindType, IEdmValue valueKindValue)
        {
            Assert.AreEqual(valueKind, valueKindValue.ValueKind, "Invalid value kind.");

            var model = this.BuildVocabularyAnnotationModelWithEdmValueKind(valueKindType, valueKindValue as IEdmExpression);

            IEnumerable<EdmError> errors;
            model.Validate(EdmConstants.EdmVersionLatest, out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid error count.");

            var csdl = this.GetSerializerResult(model);
            var csdlModel = this.GetParserResult(csdl);
            csdlModel.Validate(EdmConstants.EdmVersionLatest, out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid error count.");

            Assert.AreEqual(1, csdlModel.VocabularyAnnotations.Count(), "Invalid count of vocabulary annotation.");
            var resultValueAnnotation = csdlModel.VocabularyAnnotations.ElementAt(0);

            Assert.AreEqual(valueKind, (resultValueAnnotation.Value as IEdmValue).ValueKind, "Invalid value kind.");
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
    }
}
