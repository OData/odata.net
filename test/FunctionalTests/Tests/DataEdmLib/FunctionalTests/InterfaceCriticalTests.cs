//---------------------------------------------------------------------
// <copyright file="InterfaceCriticalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EdmLibTests.FunctionalUtilities;
    using EdmLibTests.StubEdm;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class InterfaceCriticalTests : EdmLibTestCaseBase
    {
        [TestMethod]
        public void TestInterfaceCriticalPropertyValueMustNotBeNullCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull }
            };

            var csdls = InterfaceCriticalModelBuilder.InterfaceCriticalPropertyValueMustNotBeNullOnlyCsdl();
            var model = this.GetParserResult(csdls);

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestInterfaceCriticalPropertyValueMustNotBeNullOnlyModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull },
                { null, null, EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalPropertyValueMustNotBeNullOnlyModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
        }

        [TestMethod]
        public void TestInterfaceCriticalNavigationPartnerInvalidOnlyCsdl()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalNavigationPartnerInvalid },
                { null, null, EdmErrorCode.BadUnresolvedNavigationPropertyPath }
            };

            var csdls = InterfaceCriticalModelBuilder.InterfaceCriticalNavigationPartnerInvalidOnlyCsdl();
            var model = this.GetParserResult(csdls);

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
        }

        [TestMethod]
        public void TestInterfaceCriticalKindValueMismatchOnlyModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalKindValueMismatch }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalKindValueMismatchOnlyModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestEdmExpressionKindInterfaceCriticalKindValueUnexpectedOnlyModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalKindValueUnexpected }
            };

            var model = InterfaceCriticalModelBuilder.EdmExpressionKindInterfaceCriticalKindValueUnexpectedOnlyModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestEdmValueKindInterfaceCriticalKindValueUnexpectedOnlyModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalKindValueUnexpected }
            };

            var model = InterfaceCriticalModelBuilder.EdmValueKindInterfaceCriticalKindValueUnexpectedOnlyModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestEdmTypeKindInterfaceCriticalKindValueUnexpectedOnlyModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalKindValueUnexpected }
            };

            var model = InterfaceCriticalModelBuilder.EdmTypeKindInterfaceCriticalKindValueUnexpectedOnlyModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
        }

        [TestMethod]
        public void TestInterfaceCriticalEnumerableMustNotHaveNullElementsOnlyModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalEnumerableMustNotHaveNullElements }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalEnumerableMustNotHaveNullElementsOnlyModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestElementInterfaceCriticalEnumerableMustNotHaveNullElements()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalEnumerableMustNotHaveNullElements }
            };

            EdmPropertyValue personName = new EdmPropertyValue("Name", new EdmStringConstant("foo"));
            EdmPropertyValue[] properties = new EdmPropertyValue[] { personName, null };

            var structuralValue = new EdmStructuredValue(new EdmEntityTypeReference(new EdmEntityType("", ""), false), properties);
            var actualErrors = structuralValue.Errors();

            Assert.IsTrue(structuralValue.IsBad(), "Element is expected to be bad.");
            this.CompareErrors(actualErrors, expectedErrors);
        }

        [TestMethod]
        public void TestElementInterfaceCriticalEnumPropertyValueOutOfRange()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalEnumPropertyValueOutOfRange }
            };

            var entity = new EdmEntityType("DefaultNamespace", "Entity");
            var navProperty = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                "Navigation", new EdmEntityTypeReference(entity, false/*isNullable*/), null /*dependentProperties*/, null /*principalProperties*/, false /*containsTarget*/, (EdmOnDeleteAction)123,
                "", new EdmEntityTypeReference(entity, false/*isNullable*/), null /*partnerDependentProperties*/, null /*partnerPrincipalProperties*/, false /*partnerContainsTarget*/, EdmOnDeleteAction.Cascade);
            entity.AddProperty(navProperty);
            this.ValidateElement(navProperty, expectedErrors);
        }

        [TestMethod]
        public void TestElementInterfaceCriticalKindValueUnexpected()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalKindValueUnexpected }
            };

            var model = InterfaceCriticalModelBuilder.EdmTypeKindInterfaceCriticalKindValueUnexpectedOnlyModel();
            this.ValidateElement(model, expectedErrors);

            var entity = model.SchemaElements.OfType<IEdmEntityType>().ElementAt(0);
            this.ValidateElement(entity, expectedErrors);
        }

        [TestMethod]
        public void TestElementInterfaceCriticalKindValueMismatch()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalKindValueMismatch }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalKindValueMismatchOnlyModel();
            this.ValidateElement(model, expectedErrors);

            var valueAnnotation = model.VocabularyAnnotations.ElementAt(0);
            this.ValidateElement(valueAnnotation, expectedErrors);

            var annotationValue = valueAnnotation.Value;
            this.ValidateElement(annotationValue, expectedErrors);
        }

        [TestMethod]
        public void TestElementInterfaceCriticalNavigationPartnerInvalid()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalNavigationPartnerInvalid }
            };

            var entity1 = new EdmEntityType("DefaultNamespace", "Entity1");
            var entity2 = new EdmEntityType("DefaultNamespace", "Entity2");

            var navProperty = new StubEdmNavigationProperty("Nav")
            {
                DeclaringType = entity1,
                Type = new EdmEntityTypeReference(entity2, false),
            };

            navProperty.Partner = navProperty;
            entity1.AddProperty(navProperty);

            this.ValidateElement(navProperty, expectedErrors);
            this.ValidateElement(entity1, expectedErrors);
        }

        [TestMethod]
        public void TestElementInterfaceCriticalEntityTypeBaseType()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalNavigationPartnerInvalid }
            };

            var baseEntity = new EdmEntityType("DefaultNamespace", "baseEntity");
            var entity2 = new EdmEntityType("DefaultNamespace", "Entity2");
            var navProperty = new StubEdmNavigationProperty("Nav")
            {
                DeclaringType = baseEntity,
                Type = new EdmEntityTypeReference(entity2, false)
            };

            navProperty.Partner = navProperty;
            baseEntity.AddProperty(navProperty);

            this.ValidateElement(baseEntity, expectedErrors);

            var entity = new EdmEntityType("DefaultNamespace", "Entity1", baseEntity);
            expectedErrors = new EdmLibTestErrors();
            this.ValidateElement(entity, expectedErrors);
        }

        [TestMethod]
        public void TestElementInterfaceCriticalComplexTypeBaseType()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull },
                { null, null, EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull },
            };

            var baseComplexType = new StubEdmComplexType(null, null);
            var property = new StubEdmStructuralProperty("")
            {
                DeclaringType = baseComplexType
            };
            baseComplexType.Add(property);
            this.ValidateElement(baseComplexType, expectedErrors);

            var derivedComplexType = new EdmComplexType("", "", baseComplexType, false);
            expectedErrors = new EdmLibTestErrors();
            this.ValidateElement(derivedComplexType, expectedErrors);
        }

        [TestMethod]
        public void TestElementInterfaceCriticalPropertyValueMustNotBeNull()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull }
            };

            var baseEntity = new EdmEntityType("DefaultNamespace", "BaseEntity");
            var navProperty = new StubEdmNavigationProperty("Nav")
            {
                DeclaringType = baseEntity,
                Partner = new StubEdmNavigationProperty("Partner") { DeclaringType = baseEntity, Type = new EdmEntityTypeReference(baseEntity, false) }
            };
            baseEntity.AddProperty(navProperty);

            this.ValidateElement(navProperty, expectedErrors);
            this.ValidateElement(baseEntity, expectedErrors);

            expectedErrors = new EdmLibTestErrors();
            var entity = new EdmEntityType("DefaultNamespace", "Entity", baseEntity);
            this.ValidateElement(entity, expectedErrors);
        }

        [TestMethod]
        public void TestAllInterfaceCriticalModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalEnumerableMustNotHaveNullElements },
                { null, null, EdmErrorCode.InterfaceCriticalKindValueMismatch },
                { null, null, EdmErrorCode.InterfaceCriticalKindValueUnexpected },
                { null, null, EdmErrorCode.InterfaceCriticalNavigationPartnerInvalid },
                { null, null, EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull },
                { null, null, EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull },
            };

            var model = InterfaceCriticalModelBuilder.AllInterfaceCriticalModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestManytoManyRelationshipWithOneNavigiationPropertyInOneOfThem()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalNavigationPartnerInvalid }
            };

            var entity1 = new EdmEntityType("DefaultNamespace", "Entity1");
            var entity2 = new EdmEntityType("DefaultNamespace", "Entity2");

            var navProperty1 = new StubEdmNavigationProperty("Nav1")
            {
                DeclaringType = entity1,
                Type = new EdmEntityTypeReference(entity2, false),
            };

            var navProperty2 = new StubEdmNavigationProperty("Nav2")
            {
                DeclaringType = entity1,
                Type = new EdmEntityTypeReference(entity2, false),
            };

            var navProperty3 = new StubEdmNavigationProperty("Nav3")
            {
                DeclaringType = entity2,
                Type = new EdmEntityTypeReference(entity1, false),
            };

            navProperty1.Partner = navProperty3;
            navProperty2.Partner = navProperty3;
            navProperty3.Partner = navProperty1;

            entity1.AddProperty(navProperty1);
            entity1.AddProperty(navProperty2);
            entity2.AddProperty(navProperty3);

            var model = new EdmModel();
            model.AddElement(entity1);
            model.AddElement(entity2);

            this.ValidateUsingEdmValidator(model, expectedErrors);
        }

        [TestMethod]
        public void TestEdmValidator()
        {
            var model = new EdmModel();

            IEnumerable<EdmError> actualErrors;
            model.Validate(out actualErrors);
            Assert.AreEqual(0, actualErrors.Count(), "Invalid error count.");

            this.VerifyThrowsException(typeof(ArgumentNullException), () => model.Validate((ValidationRuleSet)null, out actualErrors));
            this.VerifyThrowsException(typeof(ArgumentNullException), () => model.Validate(new ValidationRuleSet(null), out actualErrors));

            model.Validate(new ValidationRuleSet(new List<ValidationRule>()), out actualErrors);
            Assert.AreEqual(0, actualErrors.Count(), "Invalid error count.");
        }

        [TestMethod]
        public void TestInterfaceCriticalKindValueUnexpectedWithOtherErrorsModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalKindValueUnexpected }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalKindValueUnexpectedWithOtherErrorsModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestElementError()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.ValidateElement(new EdmModel(), expectedErrors);
            this.ValidateElement(new EdmTerm("foo", "bar", EdmCoreModel.Instance.GetInt32(false)), expectedErrors);
            this.ValidateElement(new EdmEntityType("", ""), expectedErrors);
            this.ValidateElement(new EdmComplexType("", ""), expectedErrors);
            this.ValidateElement(new EdmFunction("foo", "bar", EdmCoreModel.Instance.GetStream(true)), expectedErrors);
            this.ValidateElement(new EdmFunctionImport(new EdmEntityContainer("", ""), "foo", new EdmFunction("namespace", "foo", EdmCoreModel.Instance.GetInt32(false))), expectedErrors);
            this.ValidateElement(new EdmEntityType("", "").AddStructuralProperty("foo", EdmCoreModel.Instance.GetString(true)), expectedErrors);
            this.ValidateElement(new EdmEntitySet(new EdmEntityContainer("", ""), "foo", new EdmEntityType("", "")), expectedErrors);
            this.ValidateElement(new EdmEnumType("", ""), expectedErrors);
            this.ValidateElement(EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "", Target = new EdmEntityType("", ""), TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "", Target = new EdmEntityType("", ""), TargetMultiplicity = EdmMultiplicity.One }), expectedErrors);
        }

        [TestMethod]
        public void TestInvalidElementError()
        {
            this.VerifyThrowsException(typeof(ArgumentNullException), () => ((IEdmModel)null).Errors());

            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull }
            };
            this.ValidateElement(new MutableTerm() { Namespace = "f.o/o", Name = "bar" }, expectedErrors);
        }

        [TestMethod]
        public void TestParsingCsdlWithInterfaceCriticalCycleInTypeHierarchyForEntityType()
        {
            var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"" BaseType=""NS.Child"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""Child"" BaseType=""NS.Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
    </EntityType>
</Schema>";
            var model = this.GetParserResult(new string[] { csdl });

            var expectedValidationErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.BadCyclicEntity },
                { null, null, EdmErrorCode.BadCyclicEntity }
            };
            this.VerifySemanticValidation(model, expectedValidationErrors);

            IEnumerable<EdmError> serializingErrors;
            IEnumerable<string> csdls = this.GetSerializerResult(model, out serializingErrors);
            Assert.IsTrue(csdls.Count() > 0, "Invalid csdl count.");
            Assert.IsTrue(serializingErrors.Count() == 0, "Invalid serializing error count.");
        }

        [TestMethod]
        public void TestParsingCsdlWithInterfaceCriticalCycleInTypeHierarchyForComplexType()
        {
            var csdl = @"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Person"" BaseType=""NS.Child"">
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
    </ComplexType>
    <ComplexType Name=""Child"" BaseType=""NS.Person"">
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
    </ComplexType>
</Schema>";
            var model = this.GetParserResult(new string[] { csdl });

            var expectedValidationErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.BadCyclicComplex },
                { null, null, EdmErrorCode.BadCyclicComplex },
            };
            this.VerifySemanticValidation(model, Microsoft.Test.OData.Utils.Metadata.EdmVersion.V40, expectedValidationErrors);

            IEnumerable<EdmError> serializingErrors;
            IEnumerable<string> csdls = this.GetSerializerResult(model, out serializingErrors);
            Assert.IsTrue(csdls.Count() > 0, "Invalid csdl count.");
            Assert.IsTrue(serializingErrors.Count() == 0, "Invalid serializing error count.");
        }

        [TestMethod]
        public void TestInterfaceCriticalKindValueMismatchOnlyUsingComplexTypeReferenceModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalKindValueMismatch }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalKindValueMismatchOnlyUsingComplexTypeReferenceModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestInterfaceCriticalKindValueMismatchOnlyUsingEntityTypeReferenceModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalKindValueMismatch }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalKindValueMismatchOnlyUsingEntityTypeReferenceModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestInterfaceCriticalKindValueMismatchOnlyUsingEntityReferenceTypeReferenceModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalKindValueMismatch }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalKindValueMismatchOnlyUsingEntityReferenceTypeReferenceModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestInterfaceCriticalKindValueMismatchOnlyUsingBinaryTypeReferenceModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalKindValueMismatch }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalKindValueMismatchOnlyUsingBinaryTypeReferenceModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestInterfaceCriticalPropertyValueMustNotBeNullUsingBinaryValueModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalPropertyValueMustNotBeNullUsingBinaryValueModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestInterfaceCriticalKindValueMismatchOnlyUsingDecimalTypeReferenceModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalKindValueMismatch }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalKindValueMismatchOnlyUsingDecimalTypeReferenceModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestInterfaceCriticalKindValueMismatchOnlyUsingStringTypeReferenceModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalKindValueMismatch }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalKindValueMismatchOnlyUsingStringTypeReferenceModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestInterfaceCriticalKindValueMismatchOnlyUsingCollectionTypeReferenceModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalKindValueMismatch }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalKindValueMismatchOnlyUsingCollectionTypeReferenceModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestInterfaceCriticalPropertyValueMustNotBeNullUsingCollectionTypeModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalPropertyValueMustNotBeNullUsingCollectionTypeModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestInterfaceCriticalKindValueMismatchOnlyUsingEnumTypeReferenceModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalKindValueMismatch }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalKindValueMismatchOnlyUsingEnumTypeReferenceModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestInterfaceCriticalPropertyValueMustNotBeNullUsingEntityReferenceTypeModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalPropertyValueMustNotBeNullUsingEntityReferenceTypeModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestInterfaceCriticalPropertyValueMustNotBeNullUsingEntitySetElementTypeModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalPropertyValueMustNotBeNullUsingEntitySetElementTypeModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestInterfaceCriticalPropertyValueMustNotBeNullUsingEntitySetNullNavigationModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalPropertyValueMustNotBeNullUsingEntitySetNullNavigationModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestInterfaceCriticalPropertyValueMustNotBeNullUsingEntitySetNullNavigationSetModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalPropertyValueMustNotBeNullUsingEntitySetNullNavigationSetModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestInterfaceCriticalPropertyValueMustNotBeNullUsingEnumTypeReferenceModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalPropertyValueMustNotBeNullUsingEnumTypeReferenceModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestInterfaceCriticalPropertyValueMustNotBeNullUsingEnumMemberValueModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalPropertyValueMustNotBeNullUsingEnumMemberValueModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestInterfaceCriticalPropertyValueMustNotBeNullUsingEnumMemberDeclaredTypeModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalPropertyValueMustNotBeNullUsingEnumMemberDeclaredTypeModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestInterfaceCriticalPropertyValueMustNotBeNullUsingEnumValueNullMemberModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalPropertyValueMustNotBeNullUsingEnumValueNullMemberModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestInterfaceCriticalPropertyValueMustNotBeNullUsingOperationParameterTypeModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalPropertyValueMustNotBeNullUsingOperationParameterTypeModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        [TestMethod]
        public void TestInterfaceCriticalPropertyValueMustNotBeNullUsingOperationParameterDeclaredTypeModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InterfaceCriticalPropertyValueMustNotBeNull }
            };

            var model = InterfaceCriticalModelBuilder.InterfaceCriticalPropertyValueMustNotBeNullUsingOperationParameterDeclaredTypeModel();

            this.ValidateUsingEdmValidator(model, expectedErrors);
            this.ValidateUsingEdmValidator(model, EdmConstants.EdmVersion4, expectedErrors);
            this.ValidateUsingEdmValidator(model, ValidationRuleSet.GetEdmModelRuleSet(EdmConstants.EdmVersion4), expectedErrors);
        }

        private sealed class MutableTerm : IEdmTerm
        {
            public IEdmTypeReference Type
            {
                get;
                set;
            }

            public string AppliesTo
            {
                get;
                set;
            }

            public string DefaultValue
            {
                get;
                set;
            }

            public EdmSchemaElementKind SchemaElementKind
            {
                get { return EdmSchemaElementKind.Term; }
            }

            public string Name
            {
                get;
                set;
            }

            public string Namespace
            {
                get;
                set;
            }
        }

        private void ValidateElement(IEdmElement element, IEnumerable<EdmError> expectedErrors)
        {
            if (expectedErrors.Count() > 0)
            {
                Assert.IsTrue(element.IsBad(), "Element is expected to be bad.");
            }
            else
            {
                Assert.IsFalse(element.IsBad(), "Element is not expected to be bad.");
            }

            var actualErrors = element.Errors();
            this.CompareErrors(actualErrors, expectedErrors);
        }

        private void ValidateUsingEdmValidator(IEdmModel model, IEnumerable<EdmError> expectedErrors)
        {
            IEnumerable<EdmError> actualErrors;
            model.Validate(out actualErrors);
            this.CompareErrors(actualErrors, expectedErrors);
        }

        private void ValidateUsingEdmValidator(IEdmModel model, Version version, IEnumerable<EdmError> expectedErrors)
        {
            IEnumerable<EdmError> actualErrors;
            model.Validate(version, out actualErrors);
            this.CompareErrors(actualErrors, expectedErrors);
        }

        private void ValidateUsingEdmValidator(IEdmModel model, ValidationRuleSet ruleSet, IEnumerable<EdmError> expectedErrors)
        {
            IEnumerable<EdmError> actualErrors;
            model.Validate(ruleSet, out actualErrors);
            this.CompareErrors(actualErrors, expectedErrors);
        }
    }
}
