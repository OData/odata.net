//---------------------------------------------------------------------
// <copyright file="EvaluationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using EdmLibTests.FunctionalUtilities;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EvaluationTests : EdmLibTestCaseBase
    {
        readonly IEdmModel baseModel;
        readonly IEdmModel builtInFunctionsModel;
        readonly Dictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>> builtInFunctions;
        readonly IEdmStructuredValue personValue;

        public EvaluationTests()
        {
            const string baseModelCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""DistantAge"" Type=""Int32"" />
    <Term Name=""NewAge"" Type=""Int32"" />
    <Term Name=""Punning0"" Type=""Boolean"" />
    <Term Name=""Punning1"" Type=""Byte"" />
    <Term Name=""Punning2"" Type=""SByte"" />
    <Term Name=""Punning3"" Type=""Int16"" />
    <Term Name=""Punning4"" Type=""Int32"" />
    <Term Name=""Punning5"" Type=""Collection(Int32)"" />
    <Term Name=""Punning6"" Type=""String"" />
    <Term Name=""Punning7"" Type=""String"" />
    <Term Name=""Punning8"" Type=""foo.Cartoon"" />
    <Term Name=""Punning9"" Type=""foo.Cartoon"" />
    <Term Name=""Punning10"" Type=""foo.RealSponsor"" />
    <Term Name=""Punning11"" Type=""foo.Pet"" />
    <Term Name=""PunningBool1"" Type=""Boolean"" />
    <Term Name=""PunningBool2"" Type=""Boolean"" />
    <Term Name=""PunningBool3"" Type=""Boolean"" />
    <Term Name=""PunningBool4"" Type=""Boolean"" />
    <Term Name=""PunningBool5"" Type=""Boolean"" />
    <Term Name=""PunningBool6"" Type=""Boolean"" />
    <Term Name=""PunningBool7"" Type=""Boolean"" />
    <Term Name=""PunningBool8"" Type=""Boolean"" />
    <Term Name=""PunningBool9"" Type=""Boolean"" />
    <Term Name=""PunningBool10"" Type=""Boolean"" />
    <Term Name=""PunningBool11"" Type=""Boolean"" />
    <Term Name=""Clear0"" Type=""foo.Address"" />
    <Term Name=""Clear1"" Type=""Byte"" />
    <Term Name=""Clear2"" Type=""SByte"" />
    <Term Name=""Clear3"" Type=""Int16"" />
    <Term Name=""Clear4"" Type=""Int32"" />
    <Term Name=""Clear6"" Type=""String"" />
    <Term Name=""Clear7"" Type=""String"" />
    <Term Name=""Clear8"" Type=""foo.Cartoon"" />
    <Term Name=""Clear10"" Type=""foo.RealSponsor"" />
    <Term Name=""Clear11"" Type=""foo.Pet"" />
    <Term Name=""ClearBool0"" Type=""Boolean"" />
    <Term Name=""ClearBool1"" Type=""Boolean"" />
    <Term Name=""ClearBool2"" Type=""Boolean"" />
    <Term Name=""ClearBool3"" Type=""Boolean"" />
    <Term Name=""ClearBool4"" Type=""Boolean"" />
    <Term Name=""ClearBool6"" Type=""Boolean"" />
    <Term Name=""ClearBool7"" Type=""Boolean"" />
    <Term Name=""ClearBool8"" Type=""Boolean"" />
    <Term Name=""ClearBool10"" Type=""Boolean"" />
    <Term Name=""ClearBool11"" Type=""Boolean"" />
    <Term Name=""CoolPersonTerm"" Type=""foo.CoolPerson"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
        <Property Name=""CoolnessIndex"" Type=""Int32"" />
        <Property Name=""Living"" Type=""Boolean"" />
        <Property Name=""Famous"" Type=""Boolean"" />
        <NavigationProperty Name=""Address"" Type=""foo.Address"" Nullable=""false""  />
    </EntityType>
    <EntityType Name=""LyingPerson"" BaseType=""Edm.TypeTerm"">
        <Property Name=""Handle"" Type=""String"" Nullable=""false"" />
        <Property Name=""FictionalAge"" Type=""Int32"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""CoolPerson"" BaseType=""Edm.TypeTerm"">
        <Property Name=""Sobriquet"" Type=""String"" Nullable=""false"" />
        <Property Name=""Assessment"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Street"" Type=""String"" />
    </EntityType>
    <EntityType Name=""Address"">
        <Key>
            <PropertyRef Name=""Number"" />
            <PropertyRef Name=""Street"" />
            <PropertyRef Name=""City"" />
            <PropertyRef Name=""State"" />
        </Key>
        <Property Name=""Number"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Street"" Type=""String"" Nullable=""false"" />
        <Property Name=""City"" Type=""String"" Nullable=""false"" />
        <Property Name=""State"" Type=""String"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""Cartoon"">
        <Key>
            <PropertyRef Name=""Lead"" />
        </Key>
        <Property Name=""Lead"" Type=""String"" Nullable=""false"" />
        <Property Name=""Sidekick"" Type=""String"" Nullable=""false"" />
        <NavigationProperty Name=""Sponsor"" Type=""foo.Sponsor"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""Sponsor"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
    </EntityType>
    <ComplexType Name=""Pet"">
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Age"" Type=""Int32"" Nullable=""true"" />
        <Property Name=""Toys"" Type=""Collection(String)"" />
    </ComplexType>
    <EntityType Name=""FakeSponsor"" BaseType=""foo.Sponsor"">
        <Property Name=""Secret"" Type=""String"" />
    </EntityType>
    <EntityType Name=""RealSponsor"" BaseType=""foo.Sponsor"">
        <Property Name=""Secret"" Type=""String"" />
    </EntityType>
</Schema>";

            const string builtinFunctionsCsdl =
@"<Schema Namespace=""Functions"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Function Name=""StringConcat""><ReturnType Type=""String""/>
    <Parameter Name=""String1"" Type=""String"" />
    <Parameter Name=""String2"" Type=""String"" />
  </Function>
  <Function Name=""IntegerToString""><ReturnType Type=""String""/>
    <Parameter Name=""Value"" Type=""Int64"" />
  </Function>
</Schema>";

            IEnumerable<EdmError> errors;
            SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(baseModelCsdl)) }, out this.baseModel, out errors);
            SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(builtinFunctionsCsdl)) }, out this.builtInFunctionsModel, out errors);

            IEdmEntityType person = (IEdmEntityType)this.baseModel.FindType("foo.Person");
            IEdmEntityType address = (IEdmEntityType)this.baseModel.FindType("foo.Address");

            EdmPropertyValue address1Number = new EdmPropertyValue("Number", new EdmIntegerConstant(null, 1));
            EdmPropertyValue address1Street = new EdmPropertyValue("Street", new EdmStringConstant(null, "Joey Ramone Place"));
            EdmPropertyValue address1City = new EdmPropertyValue("City", new EdmStringConstant(null, "New York"));
            EdmPropertyValue address1State = new EdmPropertyValue("State", new EdmStringConstant(null, "New York"));
            EdmPropertyValue[] address1Properties = new EdmPropertyValue[] { address1Number, address1Street, address1City, address1State };
            EdmStructuredValue address1Value = new EdmStructuredValue(new EdmEntityTypeReference(address, true), address1Properties);

            EdmPropertyValue person1Name = new EdmPropertyValue("Name", new EdmStringConstant(null, "Joey Ramone"));
            EdmPropertyValue person1Birthday = new EdmPropertyValue("Birthday", new EdmDateTimeOffsetConstant(null, new DateTimeOffset(new DateTime(1951, 5, 19), TimeSpan.Zero)));
            EdmPropertyValue person1CoolnessIndex = new EdmPropertyValue("CoolnessIndex", new EdmIntegerConstant(null, Int32.MaxValue));
            EdmPropertyValue person1Living = new EdmPropertyValue("Living", new EdmBooleanConstant(false));
            EdmPropertyValue person1Famous = new EdmPropertyValue("Famous", new EdmBooleanConstant(null, true));
            EdmPropertyValue person1Address = new EdmPropertyValue("Address", address1Value);

            EdmPropertyValue[] person1Properties = new EdmPropertyValue[] { person1Name, person1Birthday, person1CoolnessIndex, person1Living, person1Famous, person1Address };
            this.personValue = new EdmStructuredValue(new EdmEntityTypeReference(person, false), person1Properties);

            this.builtInFunctions = new Dictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>>();

            IEdmOperation stringConcat = this.builtInFunctionsModel.FindOperations("Functions.StringConcat").Single();
            this.builtInFunctions[stringConcat] = (a) => new EdmStringConstant(((IEdmStringValue)a[0]).Value + ((IEdmStringValue)a[1]).Value);

            IEdmOperation integerToString = this.builtInFunctionsModel.FindOperations("Functions.IntegerToString").Single();
            this.builtInFunctions[integerToString] = (a) => new EdmStringConstant(((IEdmIntegerValue)a[0]).Value.ToString());
        }

        [TestMethod]
        public void EvaluateSimpleExpressions()
        {
            const string annotatingModelCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""foo.DistantAge"" Int=""99"" />
    </Annotations>
</Schema>";

            IEnumerable<EdmError> errors;
            IEdmModel annotatingModel;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(annotatingModelCsdl)) }, new IEdmModel[] { this.baseModel }, out annotatingModel, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmTerm distantAge = this.baseModel.FindTerm("foo.DistantAge");

            EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.builtInFunctions);
            EdmToClrEvaluator clrEvaluator = new EdmToClrEvaluator(this.builtInFunctions);

            IEdmValue personDistantAge = annotatingModel.GetTermValue(this.personValue, distantAge, expressionEvaluator);
            Assert.AreEqual(99, ((IEdmIntegerValue)personDistantAge).Value, "Term annotation value");
            Assert.AreEqual(99, annotatingModel.GetTermValue<int>(this.personValue, distantAge, clrEvaluator), "Term annotation value 2");

            personDistantAge = annotatingModel.GetTermValue(this.personValue, "foo.DistantAge", expressionEvaluator);
            Assert.AreEqual(99, ((IEdmIntegerValue)personDistantAge).Value, "Term annotation value");
            Assert.AreEqual(99, annotatingModel.GetTermValue<int>(this.personValue, "foo.DistantAge", clrEvaluator), "Term annotation value 2");
        }

        [TestMethod]
        public void EvaluateExpressionsWithoutContext()
        {
            EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.builtInFunctions);

            IEdmExpression integerExpression = new EdmIntegerConstant(44);
            IEdmExpression stringExpression = new EdmStringConstant("Yow!");

            IEdmValue integerValue = expressionEvaluator.Evaluate(integerExpression, null);
            IEdmValue stringValue = expressionEvaluator.Evaluate(stringExpression);

            Assert.AreEqual(44, ((IEdmIntegerValue)integerValue).Value, "Integer constant value");
            Assert.AreEqual("Yow!", ((IEdmStringValue)stringValue).Value, "String constant value");
        }

        [TestMethod]
        public void EvaluatePathUsesFindProperty()
        {
            bool caught = false;

            try
            {
                EdmPathExpression path = new EdmPathExpression("Gronk");
                new EdmExpressionEvaluator(this.builtInFunctions).Evaluate(path, new ThrowingValue());
            }
            catch (FindPropertyValue)
            {
                caught = true;
            }
            catch (EnumeratePropertyValues)
            {
                Assert.Fail("Enumerated property values");
            }

            Assert.IsTrue(caught, "Find property value");
        }

        private class ThrowingValue : IEdmStructuredValue
        {
            public IEnumerable<IEdmPropertyValue> PropertyValues
            {
                get { throw new EnumeratePropertyValues(); }
            }

            public IEdmPropertyValue FindPropertyValue(string propertyName)
            {
                throw new FindPropertyValue();
            }

            public IEdmTypeReference Type
            {
                get { return null; }
            }

            public EdmValueKind ValueKind
            {
                get { return EdmValueKind.Structured; }
            }
        }

        private class EnumeratePropertyValues : Exception
        {
        }

        private class FindPropertyValue : Exception
        {
        }

        [TestMethod]
        public void EvaluatePathOverValueWithManyProperties()
        {
            List<IEdmPropertyValue> contextValues = new List<IEdmPropertyValue>();
            for (int i = 0; i < 10000; i++)
            {
                contextValues.Add(new EdmPropertyValue("P" + i.ToString(), new EdmIntegerConstant(i)));
            }

            EdmStructuredValue context = new EdmStructuredValue(null, contextValues);

            EdmExpressionEvaluator evaluator = new EdmExpressionEvaluator(this.builtInFunctions);

            EdmPathExpression path = new EdmPathExpression("P0");
            IEdmValue first = evaluator.Evaluate(path, context);
            Assert.AreEqual(0, ((IEdmIntegerValue)first).Value, "Property value");

            path = new EdmPathExpression("P555");
            IEdmValue middle = evaluator.Evaluate(path, context);
            Assert.AreEqual(555, ((IEdmIntegerValue)middle).Value, "Property value");

            path = new EdmPathExpression("P9999");
            IEdmValue last = evaluator.Evaluate(path, context);
            Assert.AreEqual(9999, ((IEdmIntegerValue)last).Value, "Property value");
        }

        [TestMethod]
        public void EvaluateExpressionsWithQualifiers()
        {
            const string annotatingModelCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""foo.DistantAge"" Qualifier=""Long"" Int=""99"" />
        <Annotation Term=""foo.DistantAge"" Qualifier=""VeryLong"" Int=""127"" />
        <Annotation Term=""foo.DistantAge"" Qualifier=""Bogus"" Int=""999"" />
    </Annotations>
</Schema>";

            IEnumerable<EdmError> errors;
            IEdmModel annotatingModel;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(annotatingModelCsdl)) }, new IEdmModel[] { this.baseModel }, out annotatingModel, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmTerm distantAge = this.baseModel.FindTerm("foo.DistantAge");

            EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.builtInFunctions);
            EdmToClrEvaluator clrEvaluator = new EdmToClrEvaluator(this.builtInFunctions);

            IEdmValue personDistantAge = annotatingModel.GetTermValue(this.personValue, distantAge, "Long", expressionEvaluator);
            Assert.AreEqual(99, ((IEdmIntegerValue)personDistantAge).Value, "Term annotation value");
            Assert.AreEqual(127, annotatingModel.GetTermValue<int>(this.personValue, distantAge, "VeryLong", clrEvaluator), "Term annotation value too");
            Assert.AreEqual(999, annotatingModel.GetTermValue<int>(this.personValue, distantAge, "Bogus", clrEvaluator), "Term annotation value too");

            personDistantAge = annotatingModel.GetTermValue(this.personValue, "foo.DistantAge", "Long", expressionEvaluator);
            Assert.AreEqual(99, ((IEdmIntegerValue)personDistantAge).Value, "Term annotation value");
            Assert.AreEqual(127, annotatingModel.GetTermValue<int>(this.personValue, "foo.DistantAge", "VeryLong", clrEvaluator), "Term annotation value too");
            Assert.AreEqual(999, annotatingModel.GetTermValue<int>(this.personValue, "foo.DistantAge", "Bogus", clrEvaluator), "Term annotation value too");
        }

        [TestMethod]
        public void EvaluateExpressionsRoundTripWithQualifiers()
        {
            var expectedCsdl = new List<XElement>() 
            {
                XElement.Parse(@"
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Annotations Target=""foo.Person"">
    <Annotation Int=""99"" Qualifier=""3"" Term=""foo.Note"" />
    <Annotation Int=""127"" Qualifier=""ReallyOddQualifer1234567890!@#$%^*()_+&amp;"" Term=""foo.Note"" />
    <Annotation Int=""127"" Qualifier=""foo+bar"" Term=""foo.Note"" />
  </Annotations>
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Nullable=""false"" Type=""Edm.Int32"" />
    <Property Name=""Name"" Nullable=""true"" Type=""Edm.String"" />
  </EntityType>
  <Term Name=""Note"" Type=""Edm.Int32"" />
</Schema>")
            };

            var csdls = VocabularyTestModelBuilder.AnnotationQualifiersWithNonSimpleValue();
            IEdmModel model = this.GetParserResult(csdls);

            EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.builtInFunctions);
            EdmToClrEvaluator clrEvaluator = new EdmToClrEvaluator(this.builtInFunctions);
            var qualifier3 = "3";
            var qualifier4 = "ReallyOddQualifer1234567890!@#$%^*()_+&";
            var qualifier5 = "foo+bar";


            Action<IEdmModel> validateAnnotationQualifier = m =>
            {
                var note = m.FindTerm("foo.Note");

                IEdmValue noteValue = m.GetTermValue(this.personValue, note, qualifier3, expressionEvaluator);
                Assert.AreEqual(99, ((IEdmIntegerValue)noteValue).Value, "Term annotation value");
                Assert.AreEqual(127, m.GetTermValue<int>(this.personValue, note, qualifier4, clrEvaluator), "Term annotation value too");
                Assert.AreEqual(127, m.GetTermValue<int>(this.personValue, note, qualifier5, clrEvaluator), "Term annotation value too");

                noteValue = m.GetTermValue(this.personValue, "foo.Note", qualifier3, expressionEvaluator);
                Assert.AreEqual(99, ((IEdmIntegerValue)noteValue).Value, "Term annotation value");
                Assert.AreEqual(127, m.GetTermValue<int>(this.personValue, "foo.Note", qualifier4, clrEvaluator), "Term annotation value too");
                Assert.AreEqual(127, m.GetTermValue<int>(this.personValue, "foo.Note", qualifier5, clrEvaluator), "Term annotation value too");
            };

            validateAnnotationQualifier(model);

            IEnumerable<EdmError> errors;
            var roundTripCsdls = this.GetSerializerResult(model, Microsoft.Test.OData.Utils.Metadata.EdmVersion.V40, out errors).Select(XElement.Parse).ToList();
            Assert.AreEqual(0, errors.Count(), "Invalid error count.");
            new ConstructiveApiCsdlXElementComparer().Compare(expectedCsdl, roundTripCsdls);

            var roundTripModel = this.GetParserResult(roundTripCsdls);
            validateAnnotationQualifier(roundTripModel);
        }

        [TestMethod]
        public void EvaluateExpressionsOnBaseType()
        {
            const string annotatingModelCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Thing"">
        <Key>
            <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Int32"" />
        <Property Name=""Value"" Type=""String"" />
    </EntityType>
    <EntityType Name=""AnotherThing"" BaseType=""Edm.TypeTerm"">
        <Property Name=""AnotherValue"" Type=""String"" />
    </EntityType>
    <Annotations Target=""foo.Thing"">
        <Annotation Term=""foo.DistantAge"" Qualifier=""Short"" Int=""27"" />
        <Annotation Term=""foo.DistantAge"" Qualifier=""VeryLong"" Int=""127"" />
    </Annotations>
    <EntityType Name=""MiddleThing"" BaseType=""foo.Thing"">
        <Property Name=""AddSome"" Type=""String"" />
    </EntityType>
    <EntityType Name=""DerivedThing"" BaseType=""foo.MiddleThing"">
        <Property Name=""AddSomeMore"" Type=""String"" />
    </EntityType>
</Schema>";

            IEnumerable<EdmError> errors;
            IEdmModel annotatingModel;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(annotatingModelCsdl)) }, new IEdmModel[] { this.baseModel }, out annotatingModel, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType derivedThing = (IEdmEntityType)annotatingModel.FindType("foo.DerivedThing");

            EdmPropertyValue id = new EdmPropertyValue("ID", new EdmIntegerConstant(null, 17));
            EdmPropertyValue value = new EdmPropertyValue("Value", new EdmStringConstant(null, "Very high value"));
            EdmPropertyValue addSome = new EdmPropertyValue("AddSome", new EdmStringConstant(null, "More")); 
            EdmPropertyValue addSomeMore = new EdmPropertyValue("AddSomeMore", new EdmStringConstant(null, "Much, much, more"));

            EdmPropertyValue[] thingProperties = new EdmPropertyValue[] { id, value, addSome, addSomeMore };
            IEdmStructuredValue thingValue = new EdmStructuredValue(new EdmEntityTypeReference(derivedThing, false), thingProperties);

            IEdmEntityType anotherThing = (IEdmEntityType)this.baseModel.FindType("foo.AnotherThing");
            IEdmTerm distantAge = this.baseModel.FindTerm("foo.DistantAge");

            EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.builtInFunctions);
            EdmToClrEvaluator clrEvaluator = new EdmToClrEvaluator(this.builtInFunctions);

            IEdmValue thingDistantAge = annotatingModel.GetTermValue(thingValue, distantAge, "Short", expressionEvaluator);
            Assert.AreEqual(27, ((IEdmIntegerValue)thingDistantAge).Value, "Term annotation value");
            Assert.AreEqual(127, annotatingModel.GetTermValue<int>(thingValue, distantAge, "VeryLong", clrEvaluator), "Term annotation value too");
        }

        [TestMethod]
        public void EvaluateFunctionApplicationExpression()
        {
            const string annotatingModelCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""foo.CoolPersonTerm"">
           <Record>
               <PropertyValue Property=""Street"">
                   <Apply Function=""Functions.StringConcat"">
                       <Apply Function=""Functions.StringConcat"">
                           <Apply Function=""Functions.IntegerToString"">
                               <Path>Address/Number</Path>
                           </Apply>
                           <String>/</String>
                       </Apply>
                       <Apply Function=""Functions.StringConcat"">
                           <Apply Function=""Functions.StringConcat"">
                               <Path>Address/Street</Path>
                               <Apply Function=""Functions.StringConcat"">
                                   <String>/</String>
                                   <Path>Address/City</Path>
                               </Apply>
                           </Apply>
                           <Apply Function=""Functions.StringConcat"">
                               <String>/</String>
                               <Path>Address/State</Path>
                           </Apply>
                       </Apply>
                   </Apply>
               </PropertyValue>
           </Record>
        </Annotation>
    </Annotations>
</Schema>";

            IEnumerable<EdmError> errors;
            IEdmModel annotatingModel;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(annotatingModelCsdl)) }, new IEdmModel[] { this.baseModel, this.builtInFunctionsModel }, out annotatingModel, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType person = (IEdmEntityType)this.baseModel.FindType("foo.Person");

            IEdmEntityType coolPerson = (IEdmEntityType)this.baseModel.FindType("foo.CoolPerson");
            IEdmTerm coolPersonTerm = this.baseModel.FindTerm("foo.CoolPersonTerm");
            IEdmProperty coolStreet = coolPerson.FindProperty("Street");

            EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.builtInFunctions);
            EdmToClrEvaluator clrEvaluator = new EdmToClrEvaluator(this.builtInFunctions);

            IEdmValue personCoolStreet = annotatingModel.GetPropertyValue(this.personValue, coolPersonTerm, coolStreet, expressionEvaluator);
            Assert.AreEqual("1/Joey Ramone Place/New York/New York", ((IEdmStringValue)personCoolStreet).Value, "Concat application");
            Assert.AreEqual("1/Joey Ramone Place/New York/New York", annotatingModel.GetPropertyValue<string>(this.personValue, coolPersonTerm, coolStreet, clrEvaluator), "Concat application 2");
        }
        
        private static IEdmValue MagicEvaluator(string functionName, IEdmValue[] arguments)
        {
            switch (functionName)
            {
                case "Functions.MagicConcat":
                    {
                        string result = "";
                        foreach (IEdmValue value in arguments)
                        {
                            IEdmStringValue stringValue = value as IEdmStringValue;
                            if (stringValue != null)
                            {
                                result = result + stringValue.Value;
                                continue;
                            }

                            IEdmIntegerValue intValue = value as IEdmIntegerValue;
                            if (intValue != null)
                            {
                                result = result + intValue.Value.ToString();
                                continue;
                            }
                        }

                        return new EdmStringConstant(result);
                    }
            }

            return null;
        }

        [TestMethod]
        public void EvaluateLastChanceFunctionApplicationExpression()
        {
            const string annotatingModelCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""foo.CoolPersonTerm"">
           <Record>
               <PropertyValue Property=""Street"">
                   <Apply Function=""Functions.MagicConcat"">
                       <Path>Address/Number</Path>
                       <String>/</String>
                       <Path>Address/Street</Path>
                       <String>/</String>
                       <Path>Address/City</Path>
                       <String>/</String>
                       <Path>Address/State</Path>
                   </Apply>
               </PropertyValue>
           </Record>
        </Annotation>
    </Annotations>
</Schema>";

            IEnumerable<EdmError> errors;
            IEdmModel annotatingModel;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(annotatingModelCsdl)) }, new IEdmModel[] { this.baseModel, this.builtInFunctionsModel }, out annotatingModel, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType person = (IEdmEntityType)this.baseModel.FindType("foo.Person");

            IEdmEntityType coolPerson = (IEdmEntityType)this.baseModel.FindType("foo.CoolPerson");
            IEdmTerm coolPersonTerm = this.baseModel.FindTerm("foo.CoolPersonTerm");
            IEdmProperty coolStreet = coolPerson.FindProperty("Street");

            EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.builtInFunctions, MagicEvaluator);
            EdmToClrEvaluator clrEvaluator = new EdmToClrEvaluator(this.builtInFunctions, MagicEvaluator);

            IEdmValue personCoolStreet = annotatingModel.GetPropertyValue(this.personValue, coolPersonTerm, coolStreet, expressionEvaluator);
            Assert.AreEqual("1/Joey Ramone Place/New York/New York", ((IEdmStringValue)personCoolStreet).Value, "Concat application");
            Assert.AreEqual("1/Joey Ramone Place/New York/New York", annotatingModel.GetPropertyValue<string>(this.personValue, coolPersonTerm, coolStreet, clrEvaluator), "Concat application 2");
        }

        [TestMethod]
        public void EvaluateIfExpressions()
        {
            const string annotatingModelCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""foo.DistantAge"">
            <If>
                <Path>Living</Path>
                <Int>101</Int>
                <Int>99</Int>
            </If>
        </Annotation>
        <Annotation Term=""foo.NewAge"">
            <If>
                <Path>Famous</Path>
                <Int>101</Int>
                <Int>99</Int>
            </If>
        </Annotation>
    </Annotations>
</Schema>";

            IEnumerable<EdmError> errors;
            IEdmModel annotatingModel;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(annotatingModelCsdl)) }, new IEdmModel[] {  this.baseModel }, out annotatingModel, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType person = (IEdmEntityType)this.baseModel.FindType("foo.Person");

            IEdmTerm distantAge = this.baseModel.FindTerm("foo.DistantAge");
            IEdmTerm newAge = this.baseModel.FindTerm("foo.NewAge");

            EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.builtInFunctions);
            EdmToClrEvaluator clrEvaluator = new EdmToClrEvaluator(this.builtInFunctions);

            IEdmValue personDistantAge = annotatingModel.GetTermValue(this.personValue, distantAge, expressionEvaluator);
            Assert.AreEqual(99, ((IEdmIntegerValue)personDistantAge).Value, "Term annotation value");
            Assert.AreEqual(99, annotatingModel.GetTermValue<int>(this.personValue, distantAge, clrEvaluator), "Term annotation value 2");

            IEdmValue personNewAge = annotatingModel.GetTermValue(this.personValue, newAge, expressionEvaluator);
            Assert.AreEqual(101, ((IEdmIntegerValue)personNewAge).Value, "Term annotation value");
            Assert.AreEqual(101, annotatingModel.GetTermValue<int>(this.personValue, newAge, clrEvaluator), "Term annotation value 2");
        }

        [TestMethod]
        public void EvaluateIsTypeExpressions()
        {
            const string annotatingModelCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""foo.Punning0"">
            <IsType Type=""Int32"">
                <Path>Living</Path>
            </IsType>
        </Annotation>
        <Annotation Term=""foo.PunningBool1"">
            <IsType Type=""Byte"">
                <Int>256</Int>
            </IsType>
        </Annotation>
        <Annotation Term=""foo.PunningBool2"">
            <IsType Type=""SByte"">
                <Int>-129</Int>
            </IsType>
        </Annotation>
        <Annotation Term=""foo.PunningBool3"">
            <IsType Type=""Int16"">
                <Int>32768</Int>
            </IsType>
        </Annotation>
        <Annotation Term=""foo.PunningBool4"">
            <IsType Type=""Int32"">
                <Float>1.1</Float>
            </IsType>
        </Annotation>
        <Annotation Term=""foo.PunningBool5"">
            <IsType Type=""Collection(Int32)"">
                <Collection>
                    <Int>1</Int>
                    <String>2</String>
                    <Int>3</Int>
                    <Float>4.1</Float>
                </Collection>
            </IsType>
        </Annotation>
        <Annotation Term=""foo.PunningBool6"">
            <IsType Type=""String"" MaxLength=""5"">
                <String>123456</String>
            </IsType>
        </Annotation>
        <Annotation Term=""foo.PunningBool7"">
            <IsType Type=""String"" Nullable=""False"">
                <Null />
            </IsType>
        </Annotation>
        <Annotation Term=""foo.PunningBool8"">
            <IsType Type=""foo.Cartoon"">
                <Record>
                    <PropertyValue Property=""Lead"" String=""Gumby"" />
                    <PropertyValue Property=""Sidekick"" Int=""144"" />
                    <PropertyValue Property=""Sponsor"">
                        <Record>
                            <PropertyValue Property=""Name"" String=""ImpendingDoom"" />
                        </Record>
                    </PropertyValue>
                </Record>
            </IsType>
        </Annotation>
        <Annotation Term=""foo.PunningBool9"">
            <IsType Type=""foo.Cartoon"">
                <Record>
                    <PropertyValue Property=""Lead"" String=""Fred Flintstone"" />
                    <PropertyValue Property=""Sidekick"" String=""Barney Rubble"" />
                    <PropertyValue Property=""Sponsor"">
                        <Record>
                            <PropertyValue Property=""NotName"" String=""Slate Gravel"" />
                        </Record>
                    </PropertyValue>
                </Record>
            </IsType>
        </Annotation>
        <Annotation Term=""foo.PunningBool11"">
            <IsType Type=""foo.Pet"">
                <Record>
                    <PropertyValue Property=""Name"">
                        <Null />
                    </PropertyValue>
                    <PropertyValue Property=""Age"">
                        <Null />
                    </PropertyValue>
                    <PropertyValue Property=""Toys"">
                        <Collection>
                            <Null />
                        </Collection>
                    </PropertyValue>
                </Record>
            </IsType>
        </Annotation>
        <Annotation Term=""foo.ClearBool0"">
            <IsType Type=""foo.Address"">
                <Path>Address</Path>
            </IsType>
        </Annotation>
        <Annotation Term=""foo.ClearBool1"">
            <IsType Type=""Byte"">
                <Int>255</Int>
            </IsType>
        </Annotation>
        <Annotation Term=""foo.ClearBool2"">
            <IsType Type=""SByte"">
                <Int>-128</Int>
            </IsType>
        </Annotation>
        <Annotation Term=""foo.ClearBool3"">
            <IsType Type=""Int16"">
                <Int>32767</Int>
            </IsType>
        </Annotation>
        <Annotation Term=""foo.ClearBool4"">
            <IsType Type=""Int32"">
                <Int>32768</Int>
            </IsType>
        </Annotation>
        <Annotation Term=""foo.ClearBool6"">
            <IsType Type=""String"" MaxLength=""5"">
                <String>12345</String>
            </IsType>
        </Annotation>
        <Annotation Term=""foo.ClearBool7"">
            <IsType Type=""String"">
                <Null />
            </IsType>
        </Annotation>
        <Annotation Term=""foo.ClearBool8"">
            <IsType Type=""foo.Cartoon"">
                <Record>
                    <PropertyValue Property=""Lead"" String=""Rick Dastardly"" />
                    <PropertyValue Property=""Sidekick"" String=""Muttley"" />
                    <PropertyValue Property=""ExtraSidekick"" String=""Penelope Pitstop"" />
                    <PropertyValue Property=""Sponsor"">
                        <Record>
                            <PropertyValue Property=""Name"" String=""OilSlick"" />
                        </Record>
                    </PropertyValue>
                </Record>
            </IsType>
        </Annotation>
        <Annotation Term=""foo.ClearBool11"">
            <IsType Type=""foo.Pet"">
                <Record>
                    <PropertyValue Property=""Name"">
                        <String>foo</String>
                    </PropertyValue>
                    <PropertyValue Property=""Age"">
                        <Null />
                    </PropertyValue>
                    <PropertyValue Property=""Toys"">
                        <Collection>
                            <Null />
                        </Collection>
                    </PropertyValue>
                </Record>
            </IsType>
        </Annotation>
    </Annotations>
</Schema>";

            IEdmModel annotatingModel = this.GetParserResult(new String[] { annotatingModelCsdl }, this.baseModel);
            EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.builtInFunctions);
            EdmToClrEvaluator clrEvaluator = new EdmToClrEvaluator(this.builtInFunctions);

            Assert.AreEqual(false, annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.Punning0"), clrEvaluator), "Term annotation value");
            Assert.AreEqual(false, annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.PunningBool1"), clrEvaluator), "Term annotation value");
            Assert.AreEqual(false, annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.PunningBool2"), clrEvaluator), "Term annotation value");
            Assert.AreEqual(false, annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.PunningBool3"), clrEvaluator), "Term annotation value");
            Assert.AreEqual(false, annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.PunningBool4"), clrEvaluator), "Term annotation value");
            Assert.AreEqual(false, annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.PunningBool5"), clrEvaluator), "Term annotation value");
            Assert.AreEqual(false, annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.PunningBool6"), clrEvaluator), "Term annotation value");
            Assert.AreEqual(false, annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.PunningBool7"), clrEvaluator), "Term annotation value");
            Assert.AreEqual(false, annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.PunningBool8"), clrEvaluator), "Term annotation value");
            Assert.AreEqual(false, annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.PunningBool9"), clrEvaluator), "Term annotation value");
            Assert.AreEqual(false, annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.PunningBool11"), clrEvaluator), "Term annotation value");
          
            Assert.AreEqual(true, annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.ClearBool0"), clrEvaluator), "Term annotation value");
            Assert.AreEqual(true, annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.ClearBool1"), clrEvaluator), "Term annotation value");
            Assert.AreEqual(true, annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.ClearBool2"), clrEvaluator), "Term annotation value");
            Assert.AreEqual(true, annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.ClearBool3"), clrEvaluator), "Term annotation value");
            Assert.AreEqual(true, annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.ClearBool4"), clrEvaluator), "Term annotation value");
            Assert.AreEqual(true, annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.ClearBool6"), clrEvaluator), "Term annotation value");
            Assert.AreEqual(true, annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.ClearBool7"), clrEvaluator), "Term annotation value");
            Assert.AreEqual(true, annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.ClearBool8"), clrEvaluator), "Term annotation value");
            Assert.AreEqual(true, annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.ClearBool11"), clrEvaluator), "Term annotation value");
        }

        [TestMethod]
        public void EvaluateInvalidTermIsTypeExpressions()
        {
            const string annotatingModelCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""foo.Clear0"">
            <IsType Type=""foo.Address"">
                <Path>Address</Path>
            </IsType>
        </Annotation>
        <Annotation Term=""foo.Clear1"">
            <IsType Type=""Byte"">
                <Int>255</Int>
            </IsType>
        </Annotation>
        <Annotation Term=""foo.Clear2"">
            <IsType Type=""SByte"">
                <Int>-128</Int>
            </IsType>
        </Annotation>
        <Annotation Term=""foo.Clear3"">
            <IsType Type=""Int16"">
                <Int>32767</Int>
            </IsType>
        </Annotation>
        <Annotation Term=""foo.Clear4"">
            <IsType Type=""Int32"">
                <Int>32768</Int>
            </IsType>
        </Annotation>
        <Annotation Term=""foo.Clear6"">
            <IsType Type=""String"" MaxLength=""5"">
                <String>12345</String>
            </IsType>
        </Annotation>
        <Annotation Term=""foo.Clear7"">
            <IsType Type=""String"">
                <Null />
            </IsType>
        </Annotation>
        <Annotation Term=""foo.Clear8"">
            <IsType Type=""foo.Cartoon"">
                <Record>
                    <PropertyValue Property=""Lead"" String=""Rick Dastardly"" />
                    <PropertyValue Property=""Sidekick"" String=""Muttley"" />
                    <PropertyValue Property=""ExtraSidekick"" String=""Penelope Pitstop"" />
                    <PropertyValue Property=""Sponsor"">
                        <Record>
                            <PropertyValue Property=""Name"" String=""OilSlick"" />
                        </Record>
                    </PropertyValue>
                </Record>
            </IsType>
        </Annotation>
        <Annotation Term=""foo.Clear11"">
            <IsType Type=""foo.Pet"">
                <Record>
                    <PropertyValue Property=""Name"">
                        <String>foo</String>
                    </PropertyValue>
                    <PropertyValue Property=""Age"">
                        <Null />
                    </PropertyValue>
                    <PropertyValue Property=""Toys"">
                        <Collection>
                            <Null />
                        </Collection>
                    </PropertyValue>
                </Record>
            </IsType>
        </Annotation>
    </Annotations>
</Schema>";

            IEdmModel annotatingModel = this.GetParserResult(new String[] { annotatingModelCsdl }, this.baseModel);

            EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.builtInFunctions);
            EdmToClrEvaluator clrEvaluator = new EdmToClrEvaluator(this.builtInFunctions);

            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.Punning0"), clrEvaluator));

            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.Clear0"), clrEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.Clear1"), clrEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.Clear2"), clrEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.Clear3"), clrEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.Clear4"), clrEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.Clear6"), clrEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.Clear7"), clrEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.Clear8"), clrEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue<bool>(this.personValue, this.baseModel.FindTerm("foo.Clear11"), clrEvaluator));
        }

        [TestMethod]
        public void EvaluateCastExpressions()
        {
            const string annotatingModelCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""foo.Punning0"">
            <Cast Type=""Int32"">
                <Path>Living</Path>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.Punning1"">
            <Cast Type=""Byte"">
                <Int>256</Int>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.Punning2"">
            <Cast Type=""SByte"">
                <Int>-129</Int>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.Punning3"">
            <Cast Type=""Int16"">
                <Int>32768</Int>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.Punning4"">
            <Cast Type=""Int32"">
                <Float>1.1</Float>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.Punning5"">
            <Cast Type=""Collection(Int32)"">
                <Collection>
                    <Int>1</Int>
                    <String>2</String>
                    <Int>3</Int>
                    <Float>4.1</Float>
                </Collection>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.Punning6"">
            <Cast Type=""String"" MaxLength=""5"">
                <String>123456</String>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.Punning7"">
            <Cast Type=""String"" Nullable=""False"">
                <Null />
            </Cast>
        </Annotation>
        <Annotation Term=""foo.Punning8"">
            <Cast Type=""foo.Cartoon"">
                <Record>
                    <PropertyValue Property=""Lead"" String=""Gumby"" />
                    <PropertyValue Property=""Sidekick"" Int=""144"" />
                    <PropertyValue Property=""Sponsor"">
                        <Record>
                            <PropertyValue Property=""Name"" String=""ImpendingDoom"" />
                        </Record>
                    </PropertyValue>
                </Record>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.Punning9"">
            <Cast Type=""foo.Cartoon"">
                <Record>
                    <PropertyValue Property=""Lead"" String=""Fred Flintstone"" />
                    <PropertyValue Property=""Sidekick"" String=""Barney Rubble"" />
                    <PropertyValue Property=""Sponsor"">
                        <Record>
                            <PropertyValue Property=""NotName"" String=""Slate Gravel"" />
                        </Record>
                    </PropertyValue>
                </Record>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.Punning10"">
            <Cast Type=""foo.RealSponsor"">
                <Cast Type=""foo.Sponsor"">
                    <Record Type=""foo.FakeSponsor"">
                        <PropertyValue Property=""Name"" String=""Nomad"" />
                        <PropertyValue Property=""Secret"" String=""Ariel"" />
                    </Record>
                </Cast>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.Punning11"">
            <Cast Type=""foo.Pet"">
                <Record>
                    <PropertyValue Property=""Name"">
                        <Null />
                    </PropertyValue>
                    <PropertyValue Property=""Age"">
                        <Null />
                    </PropertyValue>
                    <PropertyValue Property=""Toys"">
                        <Collection>
                            <Null />
                        </Collection>
                    </PropertyValue>
                </Record>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.Clear0"">
            <Cast Type=""foo.Address"">
                <Path>Address</Path>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.Clear1"">
            <Cast Type=""Byte"">
                <Int>255</Int>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.Clear2"">
            <Cast Type=""SByte"">
                <Int>-128</Int>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.Clear3"">
            <Cast Type=""Int16"">
                <Int>32767</Int>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.Clear4"">
            <Cast Type=""Int32"">
                <Int>32768</Int>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.Clear6"">
            <Cast Type=""String"" MaxLength=""5"">
                <String>12345</String>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.Clear7"">
            <Cast Type=""String"">
                <Null />
            </Cast>
        </Annotation>
        <Annotation Term=""foo.Clear8"">
            <Cast Type=""foo.Cartoon"">
                <Record>
                    <PropertyValue Property=""Lead"" String=""Rick Dastardly"" />
                    <PropertyValue Property=""Sidekick"" String=""Muttley"" />
                    <PropertyValue Property=""ExtraSidekick"" String=""Penelope Pitstop"" />
                    <PropertyValue Property=""Sponsor"">
                        <Record>
                            <PropertyValue Property=""Name"" String=""OilSlick"" />
                        </Record>
                    </PropertyValue>
                </Record>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.Clear10"">
            <Cast Type=""foo.RealSponsor"">
                <Cast Type=""foo.Sponsor"">
                    <Record Type=""foo.RealSponsor"">
                        <PropertyValue Property=""Name"" String=""Nomad"" />
                        <PropertyValue Property=""Secret"" String=""Ariel"" />
                    </Record>
                </Cast>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.Clear11"">
            <Cast Type=""foo.Pet"">
                <Record>
                    <PropertyValue Property=""Name"">
                        <String>foo</String>
                    </PropertyValue>
                    <PropertyValue Property=""Age"">
                        <Null />
                    </PropertyValue>
                    <PropertyValue Property=""Toys"">
                        <Collection>
                            <Null />
                        </Collection>
                    </PropertyValue>
                </Record>
            </Cast>
        </Annotation>
    </Annotations>
</Schema>";

            IEdmModel annotatingModel = this.GetParserResult(new String[] { annotatingModelCsdl }, this.baseModel);
            IEdmEntityType person = (IEdmEntityType)this.baseModel.FindType("foo.Person");
            EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.builtInFunctions);

            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Punning0"), expressionEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Punning1"), expressionEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Punning2"), expressionEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Punning3"), expressionEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Punning4"), expressionEvaluator));
            
            IEdmValue personPunning = annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Punning5"), expressionEvaluator);
            Assert.AreEqual(EdmValueKind.Collection, personPunning.ValueKind, "Cast succeeds");
            bool odd = true;
            int count = 0;
            foreach (IEdmDelayedValue element in ((IEdmCollectionValue)personPunning).Elements)
            {
                count++;
                IEdmValue elementValue;

                if (odd)
                {
                    elementValue = element.Value;
                    Assert.AreEqual(EdmValueKind.Integer, elementValue.ValueKind, "Cast succeeds");
                    Assert.AreEqual(count, ((IEdmIntegerValue)elementValue).Value, "Cast succeeds");
                }
                else
                {
                    this.VerifyThrowsException(typeof(InvalidOperationException), () => elementValue = element.Value);
                }

                odd = !odd;
            }

            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Punning6"), expressionEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Punning7"), expressionEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Punning8"), expressionEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Punning9"), expressionEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Punning10"), expressionEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Punning11"), expressionEvaluator));

            IEdmValue personClear = annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Clear0"), expressionEvaluator);
            Assert.AreEqual("Joey Ramone Place", ((IEdmStringValue)((IEdmStructuredValue)personClear).FindPropertyValue("Street").Value).Value, "Cast succeeds");

            personClear = annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Clear1"), expressionEvaluator);
            Assert.AreEqual(255, ((IEdmIntegerValue)personClear).Value, "Cast succeeds");

            personClear = annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Clear2"), expressionEvaluator);
            Assert.AreEqual(-128, ((IEdmIntegerValue)personClear).Value, "Cast succeeds");

            personClear = annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Clear3"), expressionEvaluator);
            Assert.AreEqual(32767, ((IEdmIntegerValue)personClear).Value, "Cast succeeds");

            personClear = annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Clear4"), expressionEvaluator);
            Assert.AreEqual(32768, ((IEdmIntegerValue)personClear).Value, "Cast succeeds");

            personClear = annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Clear6"), expressionEvaluator);
            Assert.AreEqual("12345", ((IEdmStringValue)personClear).Value, "Cast succeeds");

            personClear = annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Clear7"), expressionEvaluator);
            Assert.AreEqual(EdmValueKind.Null, personClear.ValueKind, "Cast succeeds");

            personClear = annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Clear8"), expressionEvaluator);
            Assert.AreEqual(EdmValueKind.Structured, personClear.ValueKind, "Cast succeeds");
            Assert.AreEqual("Rick Dastardly", ((IEdmStringValue)((IEdmStructuredValue)personClear).FindPropertyValue("Lead").Value).Value, "Property value");
            Assert.AreEqual("Muttley", ((IEdmStringValue)((IEdmStructuredValue)personClear).FindPropertyValue("Sidekick").Value).Value, "Property value");

            personClear = annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Clear10"), expressionEvaluator);
            Assert.AreEqual(EdmValueKind.Structured, personClear.ValueKind, "Cast succeeds");
            Assert.AreEqual("Nomad", ((IEdmStringValue)((IEdmStructuredValue)personClear).FindPropertyValue("Name").Value).Value, "Property value");
            Assert.AreEqual("Ariel", ((IEdmStringValue)((IEdmStructuredValue)personClear).FindPropertyValue("Secret").Value).Value, "Property value");

            personClear = annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Clear11"), expressionEvaluator);
            Assert.AreEqual(EdmValueKind.Structured, personClear.ValueKind, "Cast succeeds");
            Assert.AreEqual("foo", ((IEdmStringValue)((IEdmStructuredValue)personClear).FindPropertyValue("Name").Value).Value, "Property value");
            Assert.AreEqual(EdmValueKind.Null, ((IEdmStructuredValue)personClear).FindPropertyValue("Age").Value.ValueKind, "Invalid Value Kind");
            Assert.AreEqual(EdmValueKind.Null, (((IEdmCollectionValue)((IEdmStructuredValue)personClear).FindPropertyValue("Toys").Value).Elements.First().Value).ValueKind, "Invalid Value Kind");
        }

        [TestMethod]
        public void EvaluateInvalidTermCastExpressions()
        {
            const string annotatingModelCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""foo.ClearBool0"">
            <Cast Type=""foo.Address"">
                <Path>Address</Path>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.ClearBool1"">
            <Cast Type=""Byte"">
                <Int>255</Int>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.ClearBool2"">
            <Cast Type=""SByte"">
                <Int>-128</Int>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.ClearBool3"">
            <Cast Type=""Int16"">
                <Int>32767</Int>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.ClearBool4"">
            <Cast Type=""Int32"">
                <Int>32768</Int>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.ClearBool6"">
            <Cast Type=""String"" MaxLength=""5"">
                <String>12345</String>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.ClearBool7"">
            <Cast Type=""String"">
                <Null />
            </Cast>
        </Annotation>
        <Annotation Term=""foo.ClearBool8"">
            <Cast Type=""foo.Cartoon"">
                <Record>
                    <PropertyValue Property=""Lead"" String=""Rick Dastardly"" />
                    <PropertyValue Property=""Sidekick"" String=""Muttley"" />
                    <PropertyValue Property=""ExtraSidekick"" String=""Penelope Pitstop"" />
                    <PropertyValue Property=""Sponsor"">
                        <Record>
                            <PropertyValue Property=""Name"" String=""OilSlick"" />
                        </Record>
                    </PropertyValue>
                </Record>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.ClearBool10"">
            <Cast Type=""foo.RealSponsor"">
                <Cast Type=""foo.Sponsor"">
                    <Record Type=""foo.RealSponsor"">
                        <PropertyValue Property=""Name"" String=""Nomad"" />
                        <PropertyValue Property=""Secret"" String=""Ariel"" />
                    </Record>
                </Cast>
            </Cast>
        </Annotation>
        <Annotation Term=""foo.ClearBool11"">
            <Cast Type=""foo.Pet"">
                <Record>
                    <PropertyValue Property=""Name"">
                        <String>foo</String>
                    </PropertyValue>
                    <PropertyValue Property=""Age"">
                        <Null />
                    </PropertyValue>
                    <PropertyValue Property=""Toys"">
                        <Collection>
                            <Null />
                        </Collection>
                    </PropertyValue>
                </Record>
            </Cast>
        </Annotation>
    </Annotations>
</Schema>";

            IEdmModel annotatingModel = this.GetParserResult(new String[] { annotatingModelCsdl }, this.baseModel);
            IEdmEntityType person = (IEdmEntityType)this.baseModel.FindType("foo.Person");
            EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.builtInFunctions);

            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Clear0"), expressionEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Clear1"), expressionEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Clear2"), expressionEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Clear3"), expressionEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Clear4"), expressionEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Clear6"), expressionEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Clear7"), expressionEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Clear8"), expressionEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Clear10"), expressionEvaluator));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => annotatingModel.GetTermValue(this.personValue, this.baseModel.FindTerm("foo.Clear11"), expressionEvaluator));
        }

        [TestMethod]
        public void EvaluateRecordExpressions()
        {
            const string annotatingModelCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""foo.Undefined1"">
            <Record>
                <PropertyValue Property=""X"" Int=""144"" />
                <PropertyValue Property=""Y"">
                    <Int>266</Int>
                </PropertyValue>
            </Record>
        </Annotation>
        <Annotation Term=""foo.Undefined2"">
            <Record>
            </Record>
        </Annotation>
    </Annotations>
</Schema>";

            IEnumerable<EdmError> errors;
            IEdmModel annotatingModel;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(annotatingModelCsdl)) }, new IEdmModel[] { this.baseModel }, out annotatingModel, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType person = (IEdmEntityType)this.baseModel.FindType("foo.Person");
            EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.builtInFunctions);
            EdmToClrEvaluator clrEvaluator = new EdmToClrEvaluator(this.builtInFunctions);

            IEdmTerm undefined = person.VocabularyAnnotations(annotatingModel).ElementAt(0).Term;
            IEdmStructuredValue record = (IEdmStructuredValue)annotatingModel.GetTermValue(this.personValue, undefined, expressionEvaluator);
            fooUndefined1 fooUndefined1 = annotatingModel.GetTermValue<fooUndefined1>(this.personValue, undefined, clrEvaluator);

            Assert.AreEqual(2, record.PropertyValues.Count(), "Record value property count");
            Assert.AreEqual(144, ((IEdmIntegerValue)record.PropertyValues.ElementAt(0).Value).Value, "Term annotation value");
            Assert.AreEqual(266, ((IEdmIntegerValue)record.PropertyValues.ElementAt(1).Value).Value, "Term annotation value");
            Assert.AreEqual(266, ((IEdmIntegerValue)record.PropertyValues.ElementAt(1).Value).Value, "Term annotation value");
            Assert.AreEqual(144, fooUndefined1.X, "Term annotation value fooUndefined1.X");
            Assert.AreEqual(266, fooUndefined1.Y, "Term annotation value fooUndefined1.Y");

            undefined = person.VocabularyAnnotations(annotatingModel).ElementAt(1).Term;
            record = (IEdmStructuredValue)annotatingModel.GetTermValue(this.personValue, undefined, expressionEvaluator);
            fooUndefined2 fooUndefined2 = annotatingModel.GetTermValue<fooUndefined2>(this.personValue, undefined, clrEvaluator);

            Assert.AreEqual(0, record.PropertyValues.Count(), "Record value property count");
            Assert.IsNotNull(fooUndefined2, "fooUndefined2");
        }

        public class fooUndefined1
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        public class fooUndefined2
        {
        }

        [TestMethod]
        public void EvaluateCollectionExpressions()
        {
            const string annotatingModelCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""foo.Undefined1"">
            <Collection>
                <Int>144</Int>
                <Int>266</Int>
            </Collection>
        </Annotation>
        <Annotation Term=""foo.Undefined2"">
            <Collection>
                <Int>377</Int>
            </Collection>
        </Annotation>
        <Annotation Term=""foo.Undefined3"">
            <Collection>
            </Collection>
        </Annotation>
    </Annotations>
</Schema>";

            IEnumerable<EdmError> errors;
            IEdmModel annotatingModel;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(annotatingModelCsdl)) }, new IEdmModel[] { this.baseModel }, out annotatingModel, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType person = (IEdmEntityType)this.baseModel.FindType("foo.Person");
            EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.builtInFunctions);
            EdmToClrEvaluator clrEvaluator = new EdmToClrEvaluator(this.builtInFunctions);

            IEdmTerm undefined = person.VocabularyAnnotations(annotatingModel).ElementAt(0).Term;
            IEdmCollectionValue collection = (IEdmCollectionValue)annotatingModel.GetTermValue(this.personValue, undefined, expressionEvaluator);
            IEnumerable<int> clrCollection = annotatingModel.GetTermValue<IEnumerable<int>>(this.personValue, undefined, clrEvaluator);

            Assert.AreEqual(2, collection.Elements.Count(), "Collection value size");
            Assert.AreEqual(144, ((IEdmIntegerValue)collection.Elements.ElementAt(0).Value).Value, "Term annotation value");
            Assert.AreEqual(266, ((IEdmIntegerValue)collection.Elements.ElementAt(1).Value).Value, "Term annotation value");
            Assert.AreEqual(266, ((IEdmIntegerValue)collection.Elements.ElementAt(1).Value).Value, "Term annotation value");
            Assert.AreEqual(144, clrCollection.ElementAt(0), "Term annotation value clrCollection.ElementAt(0)");
            Assert.AreEqual(266, clrCollection.ElementAt(1), "Term annotation value clrCollection.ElementAt(1)");

            undefined = person.VocabularyAnnotations(annotatingModel).ElementAt(1).Term;
            collection = (IEdmCollectionValue)annotatingModel.GetTermValue(this.personValue, undefined, expressionEvaluator);
            clrCollection = annotatingModel.GetTermValue<IEnumerable<int>>(this.personValue, undefined, clrEvaluator);

            Assert.AreEqual(1, collection.Elements.Count(), "Collection value size");
            Assert.AreEqual(377, ((IEdmIntegerValue)collection.Elements.ElementAt(0).Value).Value, "Term annotation value");
            Assert.AreEqual(377, clrCollection.ElementAt(0), "Term annotation value clrCollection.ElementAt(0)");

            undefined = person.VocabularyAnnotations(annotatingModel).ElementAt(2).Term;
            collection = (IEdmCollectionValue)annotatingModel.GetTermValue(this.personValue, undefined, expressionEvaluator);
            clrCollection = annotatingModel.GetTermValue<IEnumerable<int>>(this.personValue, undefined, clrEvaluator);

            Assert.AreEqual(0, collection.Elements.Count(), "Collection value size");
            Assert.AreEqual(0, clrCollection.Count(), "CLR Collection value size");
        }

        [TestMethod]
        public void EvaluateGraphConstruction()
        {
            const string annotatingModelCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""foo.Flintstones"">
            <Collection>
                <LabeledElement Name=""Fred"">
                    <Record>
                        <PropertyValue Property=""Name"" String=""Fred Flintstone"" />
                        <PropertyValue Property=""Partner"">
                            <LabeledElementReference Name=""Wilma"" />
                        </PropertyValue>
                        <PropertyValue Property=""Children"">
                            <Collection>
                                <LabeledElementReference Name=""Pebbles"" />
                            </Collection>
                        </PropertyValue>
                    </Record>
                </LabeledElement>
                <LabeledElement Name=""Wilma"">
                    <Record>
                        <PropertyValue Property=""Name"" String=""Wilma Flintstone"" />
                        <PropertyValue Property=""Partner"">
                            <LabeledElementReference Name=""Fred"" />
                        </PropertyValue>
                        <PropertyValue Property=""Children"">
                            <Collection>
                                <LabeledElementReference Name=""Pebbles"" />
                            </Collection>
                        </PropertyValue>
                    </Record>
                </LabeledElement>
                <LabeledElement Name=""Pebbles"">
                    <Record>
                        <PropertyValue Property=""Name"" String=""Pebbles Flintstone"" />
                        <PropertyValue Property=""Parents"">
                            <Collection>
                                <LabeledElementReference Name=""Fred"" />
                                <LabeledElementReference Name=""Wilma"" />
                            </Collection>
                        </PropertyValue>
                    </Record>
                </LabeledElement>
            </Collection>
        </Annotation>
    </Annotations>
</Schema>";

            IEnumerable<EdmError> errors;
            IEdmModel annotatingModel;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(annotatingModelCsdl)) }, new IEdmModel[] { this.baseModel }, out annotatingModel, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType person = (IEdmEntityType)this.baseModel.FindType("foo.Person");
            EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.builtInFunctions);
            EdmToClrEvaluator clrEvaluator = new EdmToClrEvaluator(this.builtInFunctions);
            clrEvaluator.EdmToClrConverter = new Microsoft.OData.Edm.Vocabularies.EdmToClrConverter(
                (IEdmStructuredValue edmValue, Type clrType, Microsoft.OData.Edm.Vocabularies.EdmToClrConverter converter, out object objectInstance, out bool objectInstanceInitialized) =>
                {
                    objectInstance = null;
                    objectInstanceInitialized = false;
                    if (clrType == typeof(Person))
                    {
                        if (edmValue.FindPropertyValue("Parents") != null)
                        {
                            objectInstance = new Child();
                        }
                        else
                        {
                            objectInstance = new Parentt();
                        }
                    }
                    else if (clrType == typeof(Parentt))
                    {
                        objectInstance = new Parentt();
                    }

                    if (objectInstance is Parentt)
                    {
                        // deal with the renamed field (edm:Partner -> clr:Partnerr).
                        IEdmStructuredValue sv = (IEdmStructuredValue)edmValue;
                        IEdmPropertyValue pv = sv.FindPropertyValue("Partner");
                        if (pv != null)
                        {
                            // register the object before converting property values in case it's being referenced somewhere down the graph
                            converter.RegisterConvertedObject(edmValue, objectInstance);

                            // call back into converter to get the value of Partner edm property and assign it to Partner clr property.
                            ((Parentt)objectInstance).Partnerr = converter.AsClrValue<Parentt>(pv.Value);
                        }
                    }

                    return objectInstance != null;
                });

            IEdmTerm undefined = person.VocabularyAnnotations(annotatingModel).ElementAt(0).Term;
            IEdmCollectionValue flintstones = (IEdmCollectionValue)annotatingModel.GetTermValue(this.personValue, undefined, expressionEvaluator);
            IEnumerable<Person> clrFlintstones = annotatingModel.GetTermValue<IEnumerable<Person>>(this.personValue, undefined, clrEvaluator);

            Assert.AreEqual(3, flintstones.Elements.Count(), "Collection value size");
            Assert.AreEqual(3, clrFlintstones.Count(), "CLR Collection value size");

            IEdmStructuredValue fred = (IEdmStructuredValue)flintstones.Elements.ElementAt(0).Value;
            Parentt clrFred = (Parentt)clrFlintstones.ElementAt(0);
            IEdmPropertyValue fredPartner = fred.FindPropertyValue("Partner");

            IEdmStructuredValue wilma = (IEdmStructuredValue)flintstones.Elements.ElementAt(1).Value;
            Parentt clrWilma = (Parentt)clrFlintstones.ElementAt(1);
            IEdmPropertyValue wilmaPartner = wilma.FindPropertyValue("Partner");

            Assert.IsNotNull(wilmaPartner, "Partner property");
            Assert.AreSame(fred, wilmaPartner.Value, "Graph reference identity");
            Assert.AreSame(wilma, fredPartner.Value, "Graph reference identity");
            Assert.AreSame(clrFred, clrWilma.Partnerr, "CLR Graph reference identity");
            Assert.AreSame(clrWilma, clrFred.Partnerr, "CLR Graph reference identity");

            IEdmValue fredChild = ((IEdmCollectionValue)(fred.FindPropertyValue("Children").Value)).Elements.First().Value;
            IEdmValue wilmaChild = ((IEdmCollectionValue)(wilma.FindPropertyValue("Children").Value)).Elements.First().Value;
            IEdmStructuredValue pebbles = (IEdmStructuredValue)flintstones.Elements.ElementAt(2).Value;
            Child clrPebbles = (Child)clrFlintstones.ElementAt(2);

            Assert.AreSame(pebbles, wilmaChild, "Graph reference identity");
            Assert.AreSame(fredChild, wilmaChild, "Graph reference identity");
            Assert.AreSame(clrPebbles, clrWilma.Children[0], "CLR Graph reference identity");
            Assert.AreSame(clrFred.Children[0], clrWilma.Children[0], "CLR Graph reference identity");

            IEdmCollectionValue pebblesParents = (IEdmCollectionValue)pebbles.FindPropertyValue("Parents").Value;
            Assert.AreSame(fred, pebblesParents.Elements.ElementAt(0).Value, "Graph reference identity");
            Assert.AreSame(wilma, pebblesParents.Elements.ElementAt(1).Value, "Graph reference identity");
            Assert.AreSame(clrFred, clrPebbles.Parents.ElementAt(0), "CLR Graph reference identity");
            Assert.AreSame(clrWilma, clrPebbles.Parents.ElementAt(1), "CLR Graph reference identity");
        }

        [TestMethod]
        public void EvaluateGraphConstructionWithoutCollection()
        {
            const string annotatingModelCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""foo.FredFlintstone"">
            <LabeledElement Name=""Fred"">
                <Record>
                    <PropertyValue Property=""Name"" String=""Fred Flintstone"" />
                    <PropertyValue Property=""Partner"">
                        <LabeledElement Name=""Wilma"">
                            <Record>
                                <PropertyValue Property=""Name"" String=""Wilma Flintstone"" />
                                <PropertyValue Property=""Partner"">
                                    <LabeledElementReference Name=""Fred"" />
                                </PropertyValue>
                                <PropertyValue Property=""Children"">
                                    <Collection>
                                        <LabeledElementReference Name=""Pebbles"" />
                                    </Collection>
                                </PropertyValue>
                                </Record>
                        </LabeledElement>
                    </PropertyValue>
                    <PropertyValue Property=""Children"">
                        <Collection>
                            <LabeledElement Name=""Pebbles"">
                                <Record>
                                    <PropertyValue Property=""Name"" String=""Pebbles Flintstone"" />
                                    <PropertyValue Property=""Parents"">
                                        <Collection>
                                            <LabeledElementReference Name=""Fred"" />
                                            <LabeledElementReference Name=""Wilma"" />
                                        </Collection>
                                    </PropertyValue>
                                </Record>
                            </LabeledElement>
                        </Collection>
                    </PropertyValue>
                </Record>
            </LabeledElement>
        </Annotation>
    </Annotations>
</Schema>";

            IEnumerable<EdmError> errors;
            IEdmModel annotatingModel;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(annotatingModelCsdl)) }, new IEdmModel[] { this.baseModel }, out annotatingModel, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType person = (IEdmEntityType)this.baseModel.FindType("foo.Person");
            EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.builtInFunctions);

            IEdmTerm undefined = person.VocabularyAnnotations(annotatingModel).ElementAt(0).Term;
            IEdmStructuredValue fred = (IEdmStructuredValue)annotatingModel.GetTermValue(this.personValue, undefined, expressionEvaluator);

            IEdmStructuredValue wilma = (IEdmStructuredValue)fred.FindPropertyValue("Partner").Value;
            IEdmPropertyValue wilmaPartner = wilma.FindPropertyValue("Partner");

            Assert.IsNotNull(wilmaPartner, "Partner property");
            Assert.AreSame(fred, wilmaPartner.Value, "Graph reference identity");
            
            IEdmValue fredChild = ((IEdmCollectionValue)(fred.FindPropertyValue("Children").Value)).Elements.First().Value;
            IEdmValue wilmaChild = ((IEdmCollectionValue)(wilma.FindPropertyValue("Children").Value)).Elements.First().Value;
           
            Assert.AreSame(fredChild, wilmaChild, "Graph reference identity");

            IEdmCollectionValue pebblesParents = (IEdmCollectionValue)((IEdmStructuredValue)fredChild).FindPropertyValue("Parents").Value;
            Assert.AreSame(fred, pebblesParents.Elements.ElementAt(0).Value, "Graph reference identity");
            Assert.AreSame(wilma, pebblesParents.Elements.ElementAt(1).Value, "Graph reference identity");
        }

        public abstract class Person
        {
            public string Name { get; set; }
        }

        public class Parentt : Person
        {
            public Parentt Partnerr { get; set; }
            public IList<Child> Children { get; set; }
        }

        public class Child : Person
        {
            public ICollection<Parentt> Parents { get { return this.parents; } }
            private readonly List<Parentt> parents = new List<Parentt>();
        }

        public class BadRefParentt : Parentt
        {
        }

        [TestMethod]
        public void EvaluateDuplicateAndUnboundLabelsGraphConstruction()
        {
            const string annotatingModelCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""foo.Flintstones"">
            <Collection>
                <LabeledElement Name=""Fred"">
                    <Record>
                        <PropertyValue Property=""Name"" String=""Fred Flintstone"" />
                        <PropertyValue Property=""Partner"">
                            <LabeledElementReference Name=""Wilma"" />
                        </PropertyValue>
                        <PropertyValue Property=""Children"">
                            <Collection>
                                <LabeledElementReference Name=""Pebbles"" />
                            </Collection>
                        </PropertyValue>
                    </Record>
                </LabeledElement>
                <LabeledElement Name=""Wilma"">
                    <Record>
                        <PropertyValue Property=""Name"" String=""Wilma Flintstone"" />
                        <PropertyValue Property=""Partner"">
                            <LabeledElementReference Name=""Freddy"" />
                        </PropertyValue>
                        <PropertyValue Property=""Children"">
                            <Collection>
                                <LabeledElementReference Name=""Pebbles"" />
                            </Collection>
                        </PropertyValue>
                    </Record>
                </LabeledElement>
                <LabeledElement Name=""Wilma"">
                    <Record>
                        <PropertyValue Property=""Name"" String=""Wilma Flintstone"" />
                        <PropertyValue Property=""Partner"">
                            <LabeledElementReference Name=""Frederico"" />
                        </PropertyValue>
                        <PropertyValue Property=""Children"">
                            <Collection>
                                <LabeledElementReference Name=""Pebbles"" />
                            </Collection>
                        </PropertyValue>
                    </Record>
                </LabeledElement>
                <LabeledElement Name=""Pebbles"">
                    <Record>
                        <PropertyValue Property=""Name"" String=""Pebbles Flintstone"" />
                        <PropertyValue Property=""Parents"">
                            <Collection>
                                <LabeledElementReference Name=""Fred"" />
                                <LabeledElementReference Name=""Wilma"" />
                                <LabeledElementReference Name=""Freddy"" />
                            </Collection>
                        </PropertyValue>
                    </Record>
                </LabeledElement>
            </Collection>
        </Annotation>
    </Annotations>
</Schema>";

            IEnumerable<EdmError> errors;
            IEdmModel annotatingModel;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(annotatingModelCsdl)) }, new IEdmModel[] { this.baseModel }, out annotatingModel, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType person = (IEdmEntityType)this.baseModel.FindType("foo.Person");
            EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.builtInFunctions);
            EdmToClrEvaluator clrEvaluator = new EdmToClrEvaluator(this.builtInFunctions);

            // And finally make sure we can handle nulls for bad graph references.
            clrEvaluator.EdmToClrConverter = new Microsoft.OData.Edm.Vocabularies.EdmToClrConverter(
                (IEdmStructuredValue edmValue, Type clrType, Microsoft.OData.Edm.Vocabularies.EdmToClrConverter converter, out object objectInstance, out bool objectInstanceInitialized) =>
                {
                    objectInstance = null;
                    objectInstanceInitialized = false;
                    if (clrType == typeof(Person))
                    {
                        if (edmValue.FindPropertyValue("Parents") != null)
                        {
                            objectInstance = new Child();
                        }
                        else
                        {
                            objectInstance = new Parentt();
                        }
                    }
                    else if (clrType == typeof(Parentt))
                    {
                        objectInstance = new Parentt();
                    }

                    if (objectInstance is Parentt)
                    {
                        // deal with the renamed field (edm:Partner -> clr:Partnerr).
                        IEdmStructuredValue sv = (IEdmStructuredValue)edmValue;
                        IEdmPropertyValue pv = sv.FindPropertyValue("Partner");
                        if (pv != null)
                        {
                            // register the object before converting property values in case it's being referenced somewhere down the graph
                            converter.RegisterConvertedObject(edmValue, objectInstance);

                            // call back into converter to get the value of Partner edm property and assign it to Partner clr property.
                            ((Parentt)objectInstance).Partnerr = converter.AsClrValue<Parentt>(pv.Value);
                        }
                    }

                    return objectInstance != null;
                });


            IEdmTerm undefined = person.VocabularyAnnotations(annotatingModel).ElementAt(0).Term;
            IEdmCollectionValue flintstones = (IEdmCollectionValue)annotatingModel.GetTermValue(this.personValue, undefined, expressionEvaluator);

            IEnumerable<Person> clrFlintstones = annotatingModel.GetTermValue<IEnumerable<Person>>(this.personValue, undefined, clrEvaluator);

            Assert.AreEqual(4, flintstones.Elements.Count(), "Collection value size");
            Assert.AreEqual(4, clrFlintstones.Count(), "CLR Collection value size");

            IEdmStructuredValue fred = (IEdmStructuredValue)flintstones.Elements.ElementAt(0).Value;
            Parentt clrFred = (Parentt)clrFlintstones.ElementAt(0);

            IEdmPropertyValue fredPartner = fred.FindPropertyValue("Partner");

            IEdmStructuredValue wilma = (IEdmStructuredValue)flintstones.Elements.ElementAt(1).Value;
            Parentt clrWilma = (Parentt)clrFlintstones.ElementAt(1);
            IEdmPropertyValue wilmaPartner = wilma.FindPropertyValue("Partner");

            clrFred = (Parentt)clrFlintstones.ElementAt(0);
            clrWilma = (Parentt)clrFlintstones.ElementAt(1);
            Assert.IsNull(clrWilma.Partnerr, "Unbound graph reference");
            Assert.IsNull(clrFred.Partnerr, "Ambiguous graph reference");
        }

        [TestMethod]
        public void EvaluateUnboundPathExpressions()
        {
            const string annotatingModelCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""foo.CoolPersonTerm"">
           <Record>
               <PropertyValue Property=""Sobriquet"" Path=""Nickname"" />
               <PropertyValue Property=""Assessment"" Path=""Nowhere/CoolnessIndex"" />
               <PropertyValue Property=""Street"" Path=""Address/Nowhere"" />
           </Record>
        </Annotation>
    </Annotations>
</Schema>";

            IEnumerable<EdmError> errors;
            IEdmModel annotatingModel;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(annotatingModelCsdl)) }, new IEdmModel[] { this.baseModel }, out annotatingModel, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType person = (IEdmEntityType)this.baseModel.FindType("foo.Person");

            IEdmTerm coolPersonTerm = this.baseModel.FindTerm("foo.CoolPersonTerm");
            IEdmEntityType coolPerson = (IEdmEntityType)this.baseModel.FindType("foo.CoolPerson");
            IEdmProperty coolSobriquet = coolPerson.FindProperty("Sobriquet");
            IEdmProperty coolAssessment = coolPerson.FindProperty("Assessment");
            IEdmProperty coolStreet = coolPerson.FindProperty("Street");

            EdmExpressionEvaluator expressionEvaluator = new EdmExpressionEvaluator(this.builtInFunctions);
            EdmToClrEvaluator clrEvaluator = new EdmToClrEvaluator(this.builtInFunctions);

            bool caught = false;
            try
            {
                IEdmValue personCoolSobriquet = annotatingModel.GetPropertyValue(this.personValue, coolPersonTerm, coolSobriquet, expressionEvaluator);
            }
            catch (InvalidOperationException)
            {
                caught = true;
            }

            Assert.IsTrue(caught, "Invalid operation");

            caught = false;
            try
            {
                IEdmValue personCoolAssessment = annotatingModel.GetPropertyValue(this.personValue, coolPersonTerm, coolAssessment, expressionEvaluator);
            }
            catch (InvalidOperationException)
            {
                caught = true;
            }

            Assert.IsTrue(caught, "Invalid operation");

            caught = false;
            try
            {
                IEdmValue personCoolStreet = annotatingModel.GetPropertyValue(this.personValue, coolPersonTerm, coolStreet, expressionEvaluator);
            }
            catch (InvalidOperationException)
            {
                caught = true;
            }

            Assert.IsTrue(caught, "Invalid operation");

            caught = false;
            try
            {
                IEdmValue personCoolSobriquetRedux = annotatingModel.GetPropertyValue(this.personValue, coolPersonTerm, coolSobriquet, expressionEvaluator);
            }
            catch (InvalidOperationException)
            {
                caught = true;
            }

            Assert.IsTrue(caught, "Invalid operation");
        }
        
        [TestMethod]
        public void EvaluateExpressionsInvalidCasesTest()
        {
            Action<IEnumerable<IEdmPropertyValue>, IEnumerable<IEdmPropertyValue>> compareProperties = (expected, actual) =>
                {
                    Assert.IsTrue(expected == null && actual == null || expected.Count() == actual.Count() && !expected.Except(actual).Any(), "Actual property values are different from the expected ones.");
                };
            var pathInvalid = new EdmPathExpression("DoesNotExist.><#!");
            var pathNoSegment = new EdmPathExpression();
            var pathValid = new EdmPathExpression("P1");
            var pathValidInvalid = new EdmPathExpression("P1", "DoesNotExist.><#!");
            EdmExpressionEvaluator evaluator = new EdmExpressionEvaluator(null);

            IEdmStructuredValue aValue = new EdmStructuredValue(null, new IEdmPropertyValue[]
            {
                new EdmPropertyValue("P1", new EdmStringConstant("aStringValue")),
                new EdmPropertyValue("P3", new EdmIntegerConstant(110))
            });
            compareProperties(aValue.PropertyValues, ((IEdmStructuredValue)evaluator.Evaluate(pathNoSegment, aValue)).PropertyValues);
            this.VerifyThrowsException(typeof(InvalidOperationException), () => evaluator.Evaluate(pathNoSegment));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => evaluator.Evaluate(pathNoSegment, null));
            this.VerifyThrowsException(typeof(ArgumentNullException), () => evaluator.Evaluate(pathNoSegment, aValue, null));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => evaluator.Evaluate(pathInvalid, aValue));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => evaluator.Evaluate(pathValid, aValue, EdmCoreModel.Instance.GetInt32(true)));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => evaluator.Evaluate(pathValidInvalid, aValue));

            aValue = new EdmStructuredValue(null, new IEdmPropertyValue[] { });
            compareProperties(aValue.PropertyValues, ((IEdmStructuredValue)evaluator.Evaluate(pathNoSegment, aValue)).PropertyValues);
            this.VerifyThrowsException(typeof(InvalidOperationException), () => evaluator.Evaluate(pathInvalid, aValue));


            aValue = new MutableStructuredValue();
            compareProperties(aValue.PropertyValues, ((IEdmStructuredValue)evaluator.Evaluate(pathNoSegment, aValue)).PropertyValues);

            aValue = new EdmStructuredValue(null, new IEdmPropertyValue[]
            {
                new EdmPropertyValue("", new EdmStringConstant("aStringValue")),
                new EdmPropertyValue("", new EdmStringConstant("aStringValue")),
            });
            compareProperties(aValue.PropertyValues, ((IEdmStructuredValue)evaluator.Evaluate(pathNoSegment, aValue)).PropertyValues);
            this.VerifyThrowsException(typeof(InvalidOperationException), () => evaluator.Evaluate(pathInvalid, aValue));

            aValue = new EdmStructuredValue(null, new IEdmPropertyValue[]
            {
                new EdmPropertyValue("P1", new EdmStringConstant("aStringValueOne")),
                new EdmPropertyValue("P1", new EdmStringConstant("aStringValueTwo")),
            });
            compareProperties(aValue.PropertyValues, ((IEdmStructuredValue)evaluator.Evaluate(pathNoSegment, aValue)).PropertyValues);
            this.VerifyThrowsException(typeof(InvalidOperationException), () => evaluator.Evaluate(pathInvalid, aValue));

            aValue = new EdmStructuredValue(null, new IEdmPropertyValue[]
            {
                new MutablePropertyValue() { Value = new EdmStringConstant("aStringValueOne") },
                new EdmPropertyValue("P1", new EdmStringConstant("aStringValueTwo")),
            });
            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmPathExpression((string)null));
            compareProperties(aValue.PropertyValues, ((IEdmStructuredValue)evaluator.Evaluate(pathNoSegment, aValue)).PropertyValues);
            Assert.AreEqual("aStringValueTwo", ((IEdmStringConstantExpression)evaluator.Evaluate(pathValid, aValue)).Value, "The evaluated value is not correct.");
        }

        [TestMethod]
        public void EvaluateEnumExpressionOfSingleValue()
        {
            var color = new EdmEnumType("Ns", "Color");
            var blue = color.AddMember("Blue", new EdmEnumMemberValue(0));
            color.AddMember("White", new EdmEnumMemberValue(1));
            var enumExpression = new EdmEnumMemberExpression(blue);
            EdmExpressionEvaluator evaluator = new EdmExpressionEvaluator(null);
            var value = evaluator.Evaluate(enumExpression) as IEdmEnumValue;
            value.Type.Definition.Should().Be(color);
            value.Value.Value.Should().Be(0);
        }

        [TestMethod]
        public void EvaluateEnumExpressionOfMultiValues()
        {
            var color = new EdmEnumType("Ns", "Color", true);
            var blue = color.AddMember("Blue", new EdmEnumMemberValue(1));
            var white = color.AddMember("White", new EdmEnumMemberValue(2));
            var enumExpression = new EdmEnumMemberExpression(blue, white);
            EdmExpressionEvaluator evaluator = new EdmExpressionEvaluator(null);
            var value = evaluator.Evaluate(enumExpression) as IEdmEnumValue;
            value.Type.Definition.Should().Be(color);
            value.Value.Value.Should().Be(3);
        }

        [TestMethod]
        public void EvaluateEnumExpressionOfMultiValuesWithoutFlagsShouldThrow()
        {
            var color = new EdmEnumType("Ns", "Color");
            var blue = color.AddMember("Blue", new EdmEnumMemberValue(1));
            var white = color.AddMember("White", new EdmEnumMemberValue(2));
            var enumExpression = new EdmEnumMemberExpression(blue, white);
            EdmExpressionEvaluator evaluator = new EdmExpressionEvaluator(null);
            Action action = () => evaluator.Evaluate(enumExpression);
            action.ShouldThrow<InvalidOperationException>().WithMessage("Type Ns.Color cannot be assigned with multi-values.");
        }

        [TestMethod]
        public void EvaluateEnumExpression()
        {
            var color = new EdmEnumType("Ns", "Color", true);
            var blue = color.AddMember("Blue", new EdmEnumMemberValue(1));
            color.AddMember("White", new EdmEnumMemberValue(2));
            var enumReferenceExpression = new EdmEnumMemberExpression(blue);
            EdmExpressionEvaluator evaluator = new EdmExpressionEvaluator(null);
            var value = evaluator.Evaluate(enumReferenceExpression) as IEdmEnumValue;
            value.Type.Definition.Should().Be(color);
            value.Value.Value.Should().Be(1);
        }

        private sealed class MutableStructuredValue : IEdmStructuredValue
        {
            public IEnumerable<IEdmPropertyValue> PropertyValues
            {
                get;
                set;
            }

            public IEdmPropertyValue FindPropertyValue(string propertyName)
            {
                return this.PropertyValues != null ? this.PropertyValues.Where(pv => pv.Name == propertyName).First() : null;
            }

            public IEdmTypeReference Type
            {
                get;
                set;
            }

            public EdmValueKind ValueKind
            {
                get { return EdmValueKind.Structured; }
            }
        }

        private sealed class MutablePropertyValue : IEdmPropertyValue
        {
            public string Name
            {
                get;
                set;
            }

            public IEdmValue Value
            {
                get;
                set;
            }
        }

        //[TestMethod, Variation(Id = 102, SkipReason=@"[EdmLib] EdmExpressionEvaluator throws NullReferenceException when EdmStructureValue has null properties. -- postponed")]
        public void EvaluateExpressionsNullPropertiesTest()
        {
            var aValue = new EdmStructuredValue(null, null);
            EdmExpressionEvaluator evaluator = new EdmExpressionEvaluator(null);
            var path = new EdmPathExpression();
            this.VerifyThrowsException(typeof(ArgumentNullException), () => evaluator.Evaluate(path, aValue));

            path = new EdmPathExpression("path");
            this.VerifyThrowsException(typeof(ArgumentNullException), () => evaluator.Evaluate(path, aValue)); // System.NullReferenceException
        }

        [TestMethod]
        public void EvaluateExpressionsDuplicatePropertiesTest()
        {
            var aValue = new EdmStructuredValue(null, new IEdmPropertyValue[]
            {
                new EdmPropertyValue("P1", new EdmStringConstant("aStringValueOne")),
                new EdmPropertyValue("P1", new EdmStringConstant("aStringValueTwo")),
            });
            var evaluator = new EdmExpressionEvaluator(null);
            var pathP1 = new EdmPathExpression("P1");
            Assert.AreEqual("aStringValueOne", ((IEdmStringConstantExpression)evaluator.Evaluate(pathP1, aValue)).Value, "The evaluated value is not correct.");

            aValue = new EdmStructuredValue(null, new IEdmPropertyValue[]
            {
                new EdmPropertyValue("", new EdmStringConstant("aStringValueOne")),
                new EdmPropertyValue("", new EdmStringConstant("aStringValueTwo")),
            });
            var pathEmpty = new EdmPathExpression("");
            Assert.AreEqual("aStringValueOne", ((IEdmStringConstantExpression)evaluator.Evaluate(pathEmpty, aValue)).Value, "The evaluated value is not correct.");
        }
    }
}
