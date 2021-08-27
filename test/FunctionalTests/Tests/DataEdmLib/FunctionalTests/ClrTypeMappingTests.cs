//---------------------------------------------------------------------
// <copyright file="ClrTypeMappingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using BindingFlags = System.Reflection.BindingFlags;

    [TestClass]
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
            this.operationDeclarationModel = this.GetParserResult(new string[] { operationCsdl });
            this.operationDefinitions = new Dictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>>();
            this.operationDefinitions[this.operationDeclarationModel.FindOperations("Functions.IntAdd").Single(f => f.Parameters.Count() == 2)] = (a) => new EdmIntegerConstant(((IEdmIntegerValue)a[0]).Value + ((IEdmIntegerValue)a[1]).Value);
        }

        [TestMethod]
        public void ClrTypeMappingVocabularyAnnotationClassTypeBasicTest()
        {
            this.InitializeOperationDefinitions();

            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.VocabularyAnnotationClassTypeBasicTest(), this.operationDeclarationModel);

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

        [TestMethod]
        public void ClrTypeMappingVocabularyAnnotationConvertBetweenCollectionValueAndSingularObject()
        {
            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.VocabularyAnnotationClassTypeBasicTest());
            Action action = null;

            action = () =>
                 this.ValidateClrObjectConverter<IEnumerable<Coordination>>(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "TVDisplay").Single(), null);
            this.VerifyThrowsException(typeof(InvalidCastException), action);

            action = () =>
                this.ValidateClrObjectConverter<Coordination>(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "MultiMonitors").Single(), null);
            this.VerifyThrowsException(typeof(InvalidCastException), action);
        }

        [TestMethod]
        public void ClrTypeMappingVocabularyAnnotationConvertBetweenCollectionValueToCollectionType()
        {
            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.VocabularyAnnotationClassTypeBasicTest());
            this.VerifyThrowsException
                            (
                                typeof(InvalidCastException),
                                () =>
                                    this.ValidateClrObjectConverter
                                                (
                                                    this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "MultiMonitors").Single(),
                                                    new List<Coordination>()
                                                        {
                                                            new Coordination() { X = 10, Y = 20 },
                                                            new Coordination() { X = 30, Y = 40 }
                                                        }
                                                 ),
                                "EdmToClr_CannotConvertEdmCollectionValueToClrType"
                            );
        }

        [TestMethod]
        public void ClrTypeMappingVocabularyAnnotationRecordValueTypeTest()
        {
            var csdls = new List<XElement>();
            csdls.Add(XElement.Parse(
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
</Schema>"
            ));

            var edmModel = this.GetParserResult(csdls);
            var person = edmModel.FindEntityType("NS1.Person");
            var differentPerson = edmModel.FindEntityType("NS1.DifferentPerson");

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
            Assert.AreEqual(edmValue.Type.Definition, differentPerson, "The InspectedBy annotation's type should be same as type defined in the term.");
        }

        [TestMethod]
        public void ClrTypeMappingPrimitiveTypeToObject()
        {
            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.PrimitiveTypeBasicTest());
            var annotation = edmModel.FindVocabularyAnnotations(edmModel.FindType("NS1.Person")).Where(n => n.Term == edmModel.FindTerm("NS1.Title1")).Single();
            var edmValue = new EdmToClrEvaluator(null).Evaluate(annotation.Value);

            this.VerifyThrowsException(typeof(InvalidCastException), () => new EdmToClrConverter().AsClrValue<Coordination>(edmValue));
        }

        [TestMethod]
        public void ClrTypeMappingInterface()
        {
            this.InitializeOperationDefinitions();

            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.VocabularyAnnotationClassTypeBasicTest(), this.operationDeclarationModel);

            this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "Coordination").Single(),
                   new Coordination2() { X = 10, Y = 20 });
            this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "InspectedBy").Single(),
               new Person2() { Id = 10, FirstName = "Young", LastName = "Hong" });
            this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "AdoptPet").Single(),
               new Pet() { Name = "Jacquine", Breed = "Bull Dog", Age = 3 });
        }

        [TestMethod]
        public void ClrTypeMappingPrivateConstructor()
        {
            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.VocabularyAnnotationClassTypeBasicTest());

            var valueAnnotation = this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "InspectedBy").Single();

            this.VerifyThrowsException(typeof(MissingMethodException), () => ConvertToClrObject<Person3>(valueAnnotation));

            this.VerifyThrowsException(typeof(MissingMethodException), () => ConvertToClrObject<Person4>(valueAnnotation));

            this.VerifyThrowsException(typeof(MissingMethodException), () => ConvertToClrObject<Person5>(valueAnnotation));
        }

        [TestMethod]
        public void ClrTypeMappingPrivateProperties()
        {
            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.VocabularyAnnotationClassTypeBasicTest());

            var valueAnnotation = this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "InspectedBy").Single();

            this.ValidateClrObjectConverter(valueAnnotation, new Person6());

            this.ValidateClrObjectConverter(valueAnnotation, new Person7() { Id = 10 });

            Person8 actual = ConvertToClrObject<Person8>(valueAnnotation);
            Assert.AreEqual(10, actual.Id, "Failed");
            Assert.AreEqual("Young", actual.FirstName, "Failed");
        }

        [TestMethod]
        public void ClrTypeMappingGenerics()
        {
            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.VocabularyAnnotationClassTypeBasicTest());

            var valueAnnotation = this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "InspectedBy").Single();

            this.ValidateClrObjectConverter(valueAnnotation, new Person9<string>() { Id = 10, FirstName = "Young", LastName = "Hong" });
        }

        [TestMethod]
        public void ClrTypeMappingDifferentPropertyTypes()
        {
            this.InitializeOperationDefinitions();

            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.VocabularyAnnotationClassTypeBasicTest(), this.operationDeclarationModel);

            var valueAnnotation = this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "TVDisplay").Single();

            this.ValidateClrObjectConverter(valueAnnotation, new Display2() { X = 10, Y = 20, Origin = new Display1() { X = 10, Y = 20 } });
            this.ValidateClrObjectConverter(valueAnnotation, new Display1() { X = 10, Y = 20 });
        }

        [TestMethod]
        public void ClrTypeMappingPrimitiveTypeOverflowTest()
        {
            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.PrimitiveTypeOverflowTest());
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

            Assert.AreEqual(edmToClrConverter.AsClrValue(GetFloatExpression("SingleValue"), typeof(Single)), float.PositiveInfinity, "It should return Infinit for Single when the value is greater than Single.MaxValue.");
            Assert.AreEqual(edmToClrConverter.AsClrValue(GetFloatExpression("NegativeSingleValue"), typeof(Single)), float.NegativeInfinity, "It should return Negative Infinit for Single when the value is less than Single.MinValue.");
            this.VerifyThrowsException(typeof(OverflowException), () => edmToClrConverter.AsClrValue(GetEdmValue("ByteValue"), typeof(byte)));
            this.VerifyThrowsException(typeof(OverflowException), () => edmToClrConverter.AsClrValue<byte>(GetEdmValue("ByteValue")));
            this.VerifyThrowsException(typeof(OverflowException), () => new EdmToClrEvaluator(null).EvaluateToClrValue<byte>(GetIntegerExpression("ByteValue")));
            this.VerifyThrowsException(typeof(OverflowException), () => edmToClrConverter.AsClrValue(GetEdmValue("SByteValue"), typeof(sbyte)));
            this.VerifyThrowsException(typeof(OverflowException), () => edmToClrConverter.AsClrValue<sbyte>(GetEdmValue("SByteValue")));
            this.VerifyThrowsException(typeof(OverflowException), () => new EdmToClrEvaluator(null).EvaluateToClrValue<sbyte>(GetIntegerExpression("SByteValue")));
        }

        [TestMethod]
        public void ClrTypeMappingVocabularyAnnotationClassTypeRecursiveTest()
        {
            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.VocabularyAnnotationClassTypeRecursiveTest());

            var valueAnnotation = this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "RecursiveProperty").Single();

            this.ValidateClrObjectConverter(valueAnnotation, new RecursiveProperty() { X = 1, Y = 2, Origin = new RecursiveProperty() { X = 3, Y = 4 } });
        }

        [TestMethod]
        public void ClrTypeMappingVocabularyAnnotationClassTypeWithNewProperties()
        {
            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.VocabularyAnnotationClassTypeWithNewProperties());

            var valueAnnotation = this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "RecursivePropertyWithNewProperties").Single();

            this.ValidateClrObjectConverter(valueAnnotation, new DerivedRecursiveProperty() { X = 1, Y = 2, Origin = 4 });
        }

        [TestMethod]
        public void ClrTypeMappingVocabularyAnnotationCollectionPropertyTest()
        {
            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.VocabularyAnnotationCollectionPropertyTest());

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

        [TestMethod]
        public void ClrTypeMappingVocabularyAnnotationCollectionOfCollectionPropertyTest()
        {
            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.VocabularyAnnotationCollectionOfCollectionPropertyTest());

            var valueAnnotation = this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "PersonValueAnnotation").Single();

            this.ValidateClrObjectConverter(valueAnnotation,
                                            new ClassWithCollectionOfCollectionProperty()
                                            {
                                                C = new int[][] { new int[] { 8, 9 } }
                                            });
        }

        [TestMethod]
        public void ClrTypeMappingVocabularyAnnotationEnumTest()
        {
            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.VocabularyAnnotationEnumTest());
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

            Assert.AreEqual(edmToClrConverter.AsClrValue(GetIntegerExpression("PersonValueAnnotation3"), typeof(EnumInt)), EnumInt.Member1, "It should return Infinit for Single when the value is greater than Single.MaxValue.");
            Assert.AreEqual(new EdmToClrEvaluator(null).EvaluateToClrValue<EnumInt>(GetIntegerExpression("PersonValueAnnotation3")), EnumInt.Member1, "It should return Infinit for Single when the value is greater than Single.MaxValue.");
            Assert.AreEqual(new EdmToClrEvaluator(null).EvaluateToClrValue<EnumInt>(GetIntegerExpression("PersonValueAnnotation4")), (EnumInt)(-2), "It should return Infinit for Single when the value is greater than Single.MaxValue.");

            this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "PersonValueAnnotation1").Single(),
                   new ClassWithEnum()
                   {
                       EnumInt = EnumInt.Member1,
                       EnumByte = (EnumByte)10,
                       EnumULong = EnumULong.Member2
                   });
            this.VerifyThrowsException(typeof(InvalidCastException), () => new EdmToClrEvaluator(null).EvaluateToClrValue<EnumInt>(GetIntegerExpression("PersonValueAnnotation2")));
            this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "PersonValueAnnotation8").Single(),
                   new ClassWithEnum()
                   {
                       EnumInt = (EnumInt)10,
                   });
        }

        [TestMethod]
        public void ClrTypeMappingVocabularyAnnotationDuplicatePropertyNameTests()
        {
            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.VocabularyAnnotationDuplicatePropertyNameTest());
            this.VerifyThrowsException(typeof(InvalidCastException),
                            () => this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "PersonValueAnnotation1").Single(), new ClassWithEnum())
                        );
        }

        [TestMethod]
        public void ClrTypeMappingVocabularyAnnotationEmptyVocabularyAnnotations()
        {
            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.VocabularyAnnotationEmptyVocabularyAnnotations());
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

            this.VerifyThrowsException(typeof(InvalidCastException),
                            () =>
                                this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "PersonValueAnnotation1").Single(), (int)1)
                        );
            this.VerifyThrowsException(typeof(ArgumentNullException),
                            () =>
                                this.ConvertToClrObject<int>(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "PersonValueAnnotation2").Single())
                        );
        }

        [TestMethod]
        public void ClrTypeMappingVocabularyAnnotationConversionToStructType()
        {
            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.VocabularyAnnotationClassTypeBasicTest());
            this.VerifyThrowsException(typeof(InvalidCastException), () => this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "Coordination").Single(), new Display1StructType()));
            this.VerifyThrowsException(typeof(InvalidCastException), () => this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "TVDisplay").Single(), new Display2StructTypeWithObjectProperty() { X = 10, Y = 20, Origin = new Display1() }));
            this.VerifyThrowsException(typeof(InvalidCastException), () => this.ValidateClrObjectConverter(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "TVDisplay").Single(), new Display2StructTypeWithStructProperty() { X = 10, Y = 20, Origin = new Display1StructType() }));
        }

        [TestMethod]
        public void ClrTypeMappingVocabularyAnnotationConversionToAbstractType()
        {
            this.InitializeOperationDefinitions();

            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.VocabularyAnnotationClassTypeBasicTest());
            this.VerifyThrowsException(typeof(MissingMethodException), () => this.ConvertToClrObject<AbstractClass>(this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "Coordination").Single()));
        }

        [TestMethod]
        public void ClrTypeMappingVocabularyAnnotationTryCreateObjectInstance()
        {
            this.InitializeOperationDefinitions();

            EdmToClrEvaluator ev = new EdmToClrEvaluator(this.operationDefinitions);

            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.VocabularyAnnotationClassTypeBasicTest(), this.operationDeclarationModel);
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
            Assert.IsNull((Display2)ev.EdmToClrConverter.AsClrValue(value, typeof(Display2)), "The returned object should be null.");

            isObjectPopulated = false;
            isObjectInitialized = true;
            ev.EdmToClrConverter = new EdmToClrConverter(tryCreateObjectInstance);
            Assert.IsNull((Display2)ev.EdmToClrConverter.AsClrValue(value, typeof(Display2)), "The returned object should be null.");

            isObjectPopulated = true;
            isObjectInitialized = false;
            ev.EdmToClrConverter = new EdmToClrConverter(tryCreateObjectInstance);
            Assert.IsTrue(CompareObjects((Display2)ev.EdmToClrConverter.AsClrValue(value, typeof(Display2)), new Display2() { X = 10, Y = 20, Origin = new Display1() { X = 10, Y = 20 } }), "The returned object has incorrect values.");

            isObjectPopulated = false;
            isObjectInitialized = false;
            ev.EdmToClrConverter = new EdmToClrConverter(tryCreateObjectInstance);
            Assert.IsTrue(CompareObjects((Display2)ev.EdmToClrConverter.AsClrValue(value, typeof(Display2)), new Display2() { X = 10, Y = 20, Origin = new Display1() { X = 10, Y = 20 } }), "The returned object has incorrect values.");

            createdObjectInstance = new Display2() { X = 0, Y = 1, Origin = new Display1 { X = 3, Y = 4 } };
            isObjectPopulated = true;
            isObjectInitialized = true;
            ev.EdmToClrConverter = new EdmToClrConverter(tryCreateObjectInstance);
            Assert.IsTrue(CompareObjects((Display2)ev.EdmToClrConverter.AsClrValue(value, typeof(Display2)), createdObjectInstance), "The returned object has incorrect values.");

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
            Assert.IsTrue(CompareObjects((Display2)ev.EdmToClrConverter.AsClrValue(value, typeof(Display2)), new Display2() { X = 10, Y = 20, Origin = new Display1() { X = 10, Y = 20 } }), "The returned object has incorrect values.");

            isObjectPopulated = false;
            isObjectInitialized = true;
            createdObjectInstance = new DisplayCoordination();
            ev.EdmToClrConverter = new EdmToClrConverter(tryCreateObjectInstance);
            Coordination actual = (Coordination)ev.EdmToClrConverter.AsClrValue(value, typeof(Coordination));
            Coordination expected = new Coordination() { X = 10, Y = 20 };
            Assert.AreEqual(expected.X, actual.X, "The returned object has incorrect values. X");
            Assert.AreEqual(expected.Y, actual.Y, "The returned object has incorrect values. X");
        }

        [TestMethod]
        public void ClrTypeMappingVocabularyAnnotationTryPopulateObjectInstance()
        {
            this.InitializeOperationDefinitions();

            EdmToClrEvaluator ev = new EdmToClrEvaluator(this.operationDefinitions);

            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.VocabularyAnnotationClassTypeBasicTest(), this.operationDeclarationModel);
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

            this.VerifyThrowsException(typeof(InvalidCastException), () => ev.EdmToClrConverter.AsClrValue(value, typeof(Display2)));

            isObjectPopulated = false;
            isObjectInitialized = false;
            createdObjectInstance = new Display1() { X = 0, Y = 1 };
            ev.EdmToClrConverter = new EdmToClrConverter(tryCreateObjectInstance);
            Assert.IsTrue(CompareObjects((Display2)ev.EdmToClrConverter.AsClrValue(value, typeof(Display2)), new Display2() { X = 10, Y = 20, Origin = new Display1() { X = 10, Y = 20 } }), "The returned object has incorrect values.");

            isObjectPopulated = true;
            isObjectInitialized = true;
            createdObjectInstance = new Display1() { X = 0, Y = 1 };
            ev.EdmToClrConverter = new EdmToClrConverter(tryCreateObjectInstance);
            this.VerifyThrowsException(typeof(InvalidCastException), () => ev.EdmToClrConverter.AsClrValue(value, typeof(Display2)));
        }

        [TestMethod]
        public void ClrTypeMappingVocabularyAnnotationInterfacePropertyTest()
        {
            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.VocabularyAnnotationClassTypeBasicTest());

            var valueAnnotation = this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "TVDisplay").Single();

            this.VerifyThrowsException(typeof(InvalidCastException), () => this.ConvertToClrObject<ClassWithInterfaceProperty>(valueAnnotation));
        }

        [TestMethod]
        public void ClrTypeMappingVocabularyAnnotationVirtualMemberTest()
        {
            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.VocabularyAnnotationClassTypeBasicTest());

            var valueAnnotation = this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "TVDisplay").Single();

            this.ValidateClrObjectConverter(valueAnnotation, new ClassWithVirtualMember() { X = 10 });
            this.ValidateClrObjectConverter(valueAnnotation, new DerivedClassWithVirtualMember() { X = 10 });
        }

        [TestMethod]
        public void ClrTypeMappingValueStructTypePropertyTest()
        {
            this.InitializeOperationDefinitions();

            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.VocabularyAnnotationClassTypeBasicTest(), this.operationDeclarationModel);

            var valueAnnotation = this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "TVDisplay").Single();

            this.VerifyThrowsException(typeof(InvalidCastException), () => this.ConvertToClrObject<ClassWithStructProperty>(valueAnnotation));
        }

        [TestMethod]
        public void ClrTypeMappingVocabularyAnnotationCollectionPropertyToNullListTest()
        {
            var edmModel = this.GetParserResult(ClrTypeMappingTestModelBuilder.VocabularyAnnotationCollectionPropertyTest());
            var valueAnnotation = this.GetVocabularyAnnotations(edmModel, edmModel.FindType("NS1.Person"), "PersonValueAnnotation").Single();
            this.VerifyThrowsException(typeof(System.ArgumentException), () => this.ConvertToClrObject<ClassWithNullCollectionProperty>(valueAnnotation));
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

            Assert.IsTrue(CompareObjects(object1, object2), "The results of EvaluateToClrValue and AsClrValue do not match.");
            Assert.IsTrue(CompareObjects(object2, object3), "The result of the generic version of AsClrValue does not match that of the non-generic version.");

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
                    Assert.IsTrue(CompareObjects(expected, actual), "The actual object content is different from the expected one.");
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
}
