//---------------------------------------------------------------------
// <copyright file="SemanticValidationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using EdmLibTests.FunctionalUtilities;
    using EdmLibTests.StubEdm;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Annotations;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Expressions;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm.Library.Annotations;
    using Microsoft.OData.Edm.Library.Values;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Values;
    using Microsoft.Test.OData.Utils.Metadata;
#if SILVERLIGHT
    using Microsoft.Silverlight.Testing;
#endif
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SemanticValidationTests : EdmLibTestCaseBase
    {
        public SemanticValidationTests()
        {
            this.EdmVersion = EdmVersion.V40;
        }

#if !SILVERLIGHT
        [TestMethod]
        public void InterfaceValidatorAutoCheck()
        {
            var edmLib = typeof(IEdmElement).Assembly;
            Type interfaceValidatorType = edmLib.GetType("Microsoft.OData.Edm.Validation.Internal.InterfaceValidator");
            var interfaceVisitorsField = interfaceValidatorType.GetField("InterfaceVisitors", BindingFlags.NonPublic | BindingFlags.Static);
            object interfaceVisitors = interfaceVisitorsField.GetValue(null);
            var containsKeyMethod = interfaceVisitors.GetType().GetMethod("ContainsKey");

            Type[] skips = new Type[]
            {
                typeof(IEdmVocabularyAnnotatable),
                typeof(IEdmSchemaType),
                typeof(IEdmComplexType),
                typeof(IEdmLocatable),
                typeof(IEdmBinaryConstantExpression),
                typeof(IEdmGuidConstantExpression),
                typeof(IEdmDateTimeOffsetConstantExpression),
                typeof(IEdmDurationConstantExpression),
                typeof(IEdmBooleanConstantExpression),
                typeof(IEdmStringConstantExpression),
                typeof(IEdmNullExpression),
                typeof(IEdmIntegerConstantExpression),
                typeof(IEdmFloatingConstantExpression),
                typeof(IEdmDecimalConstantExpression),
                typeof(IEdmNullValue),
                typeof(IEdmPrimitiveValue),
                typeof(IEdmDateTimeOffsetValue),
                typeof(IEdmGuidValue),
                typeof(IEdmBooleanValue),
                typeof(IEdmDurationValue),
                typeof(IEdmDecimalValue),
                typeof(IEdmFloatingValue),
                typeof(IEdmIntegerValue),
                typeof(IEdmDirectValueAnnotationsManager),
                typeof(IEdmDirectValueAnnotationBinding),
                typeof(IEdmNavigationPropertyBinding),
                typeof(IEdmEntitySet),
                typeof(IEdmUnknownEntitySet),
                typeof(IEdmDateValue),
                typeof(IEdmDateConstantExpression),
                typeof(IEdmTimeOfDayValue),
                typeof(IEdmTimeOfDayConstantExpression)
            };

            foreach (Type skip in skips)
            {
                if (skip != typeof(IEdmLocatable) && // location is optional
                    skip != typeof(IEdmNavigationPropertyBinding) && // this interface is processed inline inside IEdmEntitySet visitor

                    skip != typeof(IEdmDateTimeOffsetValue) && // the value properties in these interfaces are structs, so they can't be null.
                    skip != typeof(IEdmDateValue) &&
                    skip != typeof(IEdmGuidValue) &&
                    skip != typeof(IEdmBooleanValue) &&
                    skip != typeof(IEdmDurationValue) &&
                    skip != typeof(IEdmDecimalValue) &&
                    skip != typeof(IEdmFloatingValue) &&
                    skip != typeof(IEdmIntegerValue) &&
                    skip != typeof(IEdmDirectValueAnnotationBinding) &&
                    skip != typeof(IEdmTimeOfDayValue)
                    )
                {
                    Assert.AreEqual(0, skip.GetProperties().Length, "It is not safe to skip interfaces with properties.");
                }
            }

            StringBuilder missingVisitors = new StringBuilder();
            foreach (Type type in edmLib.GetTypes())
            {
                if (type.IsInterface && type.IsPublic &&
                    type.Namespace.StartsWith("Microsoft.OData.Edm", StringComparison.Ordinal) &&
                    type.Name.StartsWith("IEdm", StringComparison.Ordinal))
                {
                    if (!(bool)containsKeyMethod.Invoke(interfaceVisitors, new object[] { type }) && !skips.Contains(type))
                    {
                        missingVisitors.AppendLine(type.FullName);
                    }
                }
            }

            Assert.IsTrue(missingVisitors.Length == 0, "The following interfaces might need a visitor in InterfaceValidator class \r\n" + missingVisitors.ToString());

            var isCriticalMethod = edmLib.GetType("Microsoft.OData.Edm.Validation.Internal.ValidationHelper").GetMethod("IsInterfaceCritical", BindingFlags.NonPublic | BindingFlags.Static);
            foreach (var errorCodeName in Enum.GetNames(typeof(EdmErrorCode)))
            {
                if (errorCodeName.StartsWith("InterfaceCritical", StringComparison.Ordinal))
                {
                    Assert.IsTrue((bool)isCriticalMethod.Invoke(null, new object[] { new EdmError(null, (EdmErrorCode)Enum.Parse(typeof(EdmErrorCode), errorCodeName), errorCodeName) }), "InterfaceValidator.IsCritial must return true for " + errorCodeName);
                }
            }
        }
#endif

        [TestMethod]
        public void ValidateSystemNamespace()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {3, 4, EdmErrorCode.SystemNamespaceEncountered},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.SystemNamespace(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateInvalidEntitySetNameReference()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {21, 8, EdmErrorCode.BadUnresolvedEntitySet},
                {24, 8, EdmErrorCode.BadUnresolvedEntitySet},
            };

            this.VerifySemanticValidation(ValidationTestModelBuilder.InvalidEntitySetNameReference(this.EdmVersion), expectedErrors);
        }

        /*[TestMethod, Variation(Id = 6)]
        public void IEdmEntitySet_EntitySetTypeHasNoKeys()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""Entity1_a"" EntityType=""Bork.Entity1"" />
  </EntityContainer>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = CsdlReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            model.Validate(EdmConstants.EdmVersion4, out errors);
            Assert.AreEqual(2, errors.Count(), "Correct number of errors");
            // Error cannot exist on its own, so this test is somewhat dependent on rule order. If that order changes, the error checked by this test can also safely be changed
            Assert.AreEqual(EdmErrorCode.EntitySetTypeHasNoKeys, errors.Last().ErrorCode, "Correct error code");
        }*/

        [TestMethod]
        public void ValidateDuplicateEntityContainerMemberName()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {18, 6, EdmErrorCode.DuplicateEntityContainerMemberName},
                {19, 6, EdmErrorCode.DuplicateEntityContainerMemberName},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.DuplicateEntityContainerMemberName(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateEdmEntityTypeDuplicatePropertyNameSpecifiedInEntityKey()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {8, 6, EdmErrorCode.DuplicatePropertySpecifiedInEntityKey},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.EdmEntityTypeDuplicatePropertyNameSpecifiedInEntityKey(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateInvalidKeyNullablePart()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {7, 6, EdmErrorCode.InvalidKey},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.InvalidKeyNullablePart(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateEntityKeyMustBeScalar()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {7, 6, EdmErrorCode.EntityKeyMustBeScalar},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.EntityKeyMustBeScalar(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void InvalidKeyKeyDefinedInBaseClass()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {9, 4, EdmErrorCode.InvalidKey},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.InvalidKeyKeyDefinedInBaseClass(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateKeyMissingOnEntityType()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {3, 4, EdmErrorCode.KeyMissingOnEntityType},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.KeyMissingOnEntityType(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateAbstractEntityTypeWithoutKey()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(ValidationTestModelBuilder.AbstractEntityTypeWithoutKey(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateInvalidMemberNameMatchesTypeName()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {7, 6, EdmErrorCode.BadProperty},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.InvalidMemberNameMatchesTypeName(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidatePropertyNameAlreadyDefinedDuplicate()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {9, 6, EdmErrorCode.AlreadyDefined},
                {19, 6, EdmErrorCode.AlreadyDefined},
                {25, 6, EdmErrorCode.AlreadyDefined},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.PropertyNameAlreadyDefinedDuplicate(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateIEdmInvalidOperationMultipleEndsInAssociatedNavigationProperties()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {8, 6, EdmErrorCode.InvalidAction },
                {18, 6, EdmErrorCode.InvalidAction },
            };

            this.VerifySemanticValidation(ValidationTestModelBuilder.IEdmInvalidOperationMultipleEndsInAssociatedNavigationProperties(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateIEdmAssociationTypeEndWithManyMultiplicityCannotHaveOperationsSpecified()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {17, 6, EdmErrorCode.EndWithManyMultiplicityCannotHaveOperationsSpecified},
                {21, 6, EdmErrorCode.EndWithManyMultiplicityCannotHaveOperationsSpecified},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.IEdmAssociationTypeEndWithManyMultiplicityCannotHaveOperationsSpecified(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateIEdmReferentialConstraintInvalidMultiplicityFromRoleToPropertyNullable()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.InvalidMultiplicityOfPrincipalEnd},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.IEdmReferentialConstraintInvalidMultiplicityFromRoleToPropertyNullable(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateIEdmReferentialConstraintInvalidMultiplicityFromRoleUpperBoundMustBeOne()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {16, 6, EdmErrorCode.InvalidMultiplicityOfPrincipalEnd},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.IEdmReferentialConstraintInvalidMultiplicityFromRoleUpperBoundMustBeOne(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateIEdmReferentialConstraintInvalidMultiplicityToRoleUpperBoundMustBeMany()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {16, 6, EdmErrorCode.InvalidMultiplicityOfDependentEnd},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.IEdmReferentialConstraintInvalidMultiplicityToRoleUpperBoundMustBeMany(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateIEdmReferentialConstraintInvalidMultiplicityToRoleUpperBoundMustBeOne()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {16, 6, EdmErrorCode.InvalidMultiplicityOfDependentEnd},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.IEdmReferentialConstraintInvalidMultiplicityToRoleUpperBoundMustBeOne(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateIEdmReferentialConstraintTypeMismatchRelationshipConstraint()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {16, 6, EdmErrorCode.TypeMismatchRelationshipConstraint},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.IEdmReferentialConstraintTypeMismatchRelationshipConstraint(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateComplexTypeIsAbstractSupportInV40()
        {
            this.VerifySemanticValidation(ValidationTestModelBuilder.ComplexTypeIsAbstractSupportInV40(EdmVersion.V40), EdmVersion.V40, null);
        }

        [TestMethod]
        public void ValidateEdmComplexTypeInvalidIsPolymorphic()
        {
            this.VerifySemanticValidation(ValidationTestModelBuilder.EdmComplexTypeInvalidIsPolymorphic(EdmVersion.V40), EdmVersion.V40, null);
        }

        [TestMethod]
        public void ValidateIEdmReferentialConstraintInvalidMultiplicityFromRoleToPropertyNonNullableV1()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.InvalidMultiplicityOfPrincipalEnd},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.IEdmReferentialConstraintInvalidMultiplicityFromRoleToPropertyNonNullableV1(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateIEdmReferentialConstraintInvalidToPropertyInRelationshipConstraintV4()
        {
            var expectedErrors = new EdmLibTestErrors();

            var testModel = this.GetParserResult(ValidationTestModelBuilder.IEdmReferentialConstraintInvalidToPropertyInRelationshipConstraintV1(EdmVersion.V40));
            IEnumerable<EdmError> actualErrors = null;
            testModel.Validate(toProductVersionlookup[EdmVersion.V40], out actualErrors);
            this.CompareErrors(actualErrors, expectedErrors);
        }

        [TestMethod]
        public void ValidateEdmPropertyNullableComplexType()
        {
            this.VerifySemanticValidation(ValidationTestModelBuilder.EdmPropertyNullableComplexType(this.EdmVersion), EdmVersion.V40, null);
            this.VerifySemanticValidation(ValidationTestModelBuilder.EdmPropertyNullableComplexType(this.EdmVersion), null);
        }

        [TestMethod]
        public void ValidateEdmPropertyInvalidPropertyType()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {11, 6, EdmErrorCode.InvalidPropertyType},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.EdmPropertyInvalidPropertyType(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateIEdmOperationBaseParameterNameAlreadyDefinedDuplicate()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {11, 8, EdmErrorCode.AlreadyDefined},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.IEdmFunctionBaseParameterNameAlreadyDefinedDuplicate(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateIEdmOperationImportOperationImportEntityTypeDoesNotMatchEntitySet()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {19, 10, EdmErrorCode.OperationImportEntityTypeDoesNotMatchEntitySet},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.IEdmOperationImportOperationImportEntityTypeDoesNotMatchEntitySet(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateIEdmOperationImportOperationImportSpecifiesEntitySetButDoesNotReturnEntityType()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {12, 10, EdmErrorCode.OperationImportSpecifiesEntitySetButDoesNotReturnEntityType},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.IEdmOperationImportOperationImportSpecifiesEntitySetButDoesNotReturnEntityType(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateIEdmFunctionComposableOperationImportHasNoReturnType()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {2, 6, EdmErrorCode.FunctionMustHaveReturnType},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.IEdmComposableFunctionNoReturnType(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateIEdmEntityReferenceTypeEntityTypeNotBad()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {8, 10, EdmErrorCode.BadUnresolvedType},
                {10, 43, EdmErrorCode.BadUnresolvedEntityType},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.IEdmEntityReferenceTypeEntityTypeNotBad(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateIEdmEntitySetElementTypeNotBad()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {10, 10, EdmErrorCode.BadUnresolvedEntityType},
            };

            this.VerifySemanticValidation(ValidationTestModelBuilder.IEdmEntitySetElementTypeNotBad(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateEdmModelNameMustNotBeWhiteSpace()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.InvalidName},
                {null, null, EdmErrorCode.InvalidNamespaceName},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.EdmModelNameMustNotBeWhiteSpace(), expectedErrors);
        }

        [TestMethod]
        public void ValidateEdmModelNameIsTooLong()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.NameTooLong},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.NameIsTooLong(), expectedErrors);
        }

        [TestMethod]
        public void ValidateIEdmNamedElementNameIsNotAllowed()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {"(Foo1._!@#$%^&*())", EdmErrorCode.InvalidName},
                {"(Foo2._!@#$%^&*())", EdmErrorCode.InvalidName},
                {"(.)", EdmErrorCode.InvalidName},
                {"(.)", EdmErrorCode.InvalidNamespaceName},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.IEdmNamedElementNameIsNotAllowed(), expectedErrors);
        }

        [TestMethod]
        public void ValidateIEdmSchemaElementNameIsNotAllowed()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.InvalidNamespaceName},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.IEdmSchemaElementNameIsNotAllowed(), expectedErrors);
        }

        [TestMethod]
        public void ValidateIEdmModelTypeNameAlreadyDefinedDuplicate()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {6, 4, EdmErrorCode.AlreadyDefined},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.IEdmModelTypeNameAlreadyDefinedDuplicate(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateOperationSupportInV20()
        {
            this.VerifySemanticValidation(ValidationTestModelBuilder.OperationSupportInV40(EdmVersion.V40), EdmVersion.V40, null);
        }

        [TestMethod]
        public void ValidateEntitySetOfAbstractBaseType()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(ValidationTestModelBuilder.InstantiateAbstractEntityType(this.EdmVersion),
                expectedErrors);
        }

        [TestMethod]
        public void ValidateEntitySetOfAbstractBaseTypeWithoutKey()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {12, 6, EdmErrorCode.NavigationSourceTypeHasNoKeys}
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.InstantiateAbstractEntityTypeWithoutKey(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateOpenTypeSupportInV12()
        {
            // In CSDL 1.2 and higher versions, an <EntityType> can be an OpenEntityType, denoted by the presence of an OpenType=""true"" attribute
            this.VerifySemanticValidation(ValidationTestModelBuilder.OpenTypeSupportInV40(null), EdmVersion.V40, null);
        }

        [TestMethod]
        public void ValidateOpenComplexTypeSupported()
        {
            var ct = new StubEdmComplexType("NS1", "CT1")
            {
                IsOpen = true
            };
            ct.Add(new EdmStructuralProperty(ct, "p1", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Int16, true)));

            var model = new EdmModel();
            model.AddElement(ct);

            IEnumerable<EdmError> errors;

            foreach (var v in new[] { Microsoft.OData.Edm.Library.EdmConstants.EdmVersionLatest })
            {
                model.SetEdmVersion(v);
                model.Validate(out errors);
                Assert.AreEqual(0, errors.Count(), "No error should be returned");
            }
        }

        [TestMethod]
        public void ValidateBaseTypeStructuralIntegrity()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {3, 4, EdmErrorCode.BadUnresolvedEntityType},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.BaseTypeStructuralIntegrity(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateCyclicEntityTypeBaseType()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {3, 4, EdmErrorCode.BadCyclicEntity},
                {9, 4, EdmErrorCode.BadCyclicEntity},
                {11, 4, EdmErrorCode.BadCyclicEntity},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.CyclicEntityTypeBaseType(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateDerivedEntityTypeKeyProperty()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {9, 4, EdmErrorCode.InvalidKey}
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.DerivedEntityTypeKeyProperty(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateSchemaElementShouldBeUnique()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {6, 4, EdmErrorCode.AlreadyDefined},
                {24, 4, EdmErrorCode.AlreadyDefined},
                {26, 4, EdmErrorCode.AlreadyDefined},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.SchemaElementShouldBeUnique(this.EdmVersion), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidatePropertyTypeShouldBeEdmSimpleTypeOrComplexType()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {4, 6, EdmErrorCode.InvalidPropertyType},
                {11, 6, EdmErrorCode.InvalidPropertyType},
                {13, 6, EdmErrorCode.InvalidPropertyType},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.PropertyTypeShouldBeEdmSimpleTypeOrComplexType(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateComplexTypePropertyWithNullableComplexType()
        {
            this.VerifySemanticValidation(ValidationTestModelBuilder.ComplexTypePropertyWithNullableComplexType(EdmVersion.V40), EdmVersion.V40, null);
        }

        [TestMethod]
        public void ValidateEntityTypePropertyWithNullableComplexType()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(ValidationTestModelBuilder.EntityTypePropertyWithNullableComplexType(EdmVersion.V40), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidatePropertyRefShouldReferWithinDeclaringEntityType()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.KeyPropertyMustBelongToEntity},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.PropertyRefShouldReferWithinDeclaringEntityType(), expectedErrors);
        }

        [TestMethod]
        public void ValidatePropertyRefInteigrity()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {3, 4, EdmErrorCode.BadUnresolvedProperty},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.PropertyRefInteigrity(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateComplexTypeBaseTypeSupportInV11()
        {
            this.VerifySemanticValidation(ValidationTestModelBuilder.ComplexTypeBaseTypeSupportInV11(EdmVersion.V40), EdmVersion.V40, null);
        }

        [TestMethod]
        public void ValidateCyclicComplexTypeBaseType()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {3, 4, EdmErrorCode.BadCyclicComplex},
                {6, 4, EdmErrorCode.BadCyclicComplex},
                {10, 4, EdmErrorCode.BadCyclicComplex},
            };

            this.VerifySemanticValidation(ValidationTestModelBuilder.CyclicComplexTypeBaseType(EdmVersion.V40), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateComplexTypePropertyNameMustBeUnique()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {4, 6, EdmErrorCode.AlreadyDefined},
                {8, 6, EdmErrorCode.AlreadyDefined},
                {12, 6, EdmErrorCode.AlreadyDefined},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.ComplexTypePropertyNameMustBeUnique(EdmVersion.V40), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateComplexTypePropertyNameShouldBeDifferentFromDeclaringType()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {4, 6, EdmErrorCode.BadProperty},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.ComplexTypePropertyNameShouldBeDifferentFromDeclaringType(EdmVersion.V40), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateAssociationNameSimpleIdentifier()
        {
            IEnumerable<EdmError> constructedModelErrors;
            var constructedModel = ValidationTestModelBuilder.AssociationNameSimpleIdentifier();
            constructedModel.Validate(out constructedModelErrors);
            var csdls = this.GetSerializerResult(ValidationTestModelBuilder.AssociationNameSimpleIdentifier());

            IEnumerable<EdmError> parsedModelErrors;
            var parsedModel = this.GetParserResult(csdls.Select(n => XElement.Parse(n)));
            parsedModel.Validate(out parsedModelErrors);

            Assert.AreEqual(0, parsedModelErrors.Count(), "No error should be returned.");
            Assert.AreEqual(constructedModelErrors.Count(), parsedModelErrors.Count(), "Validation results between constructed models and parsed models shold be no different.");
        }

        [TestMethod]
        public void ValidateReferentialConstraintsCanExistBetweenKeyPropertiesInV12()
        {
            this.VerifySemanticValidation(ValidationTestModelBuilder.ReferentialConstraintsCanExistBetweenKeyPropertiesInV12(EdmVersion.V40), EdmVersion.V40, null);
        }

        [TestMethod]
        public void ValidateReferentialConstraintsCanExistBetweenKeyPropertyAndPrimtiveTypeInV40()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.EntityKeyMustBeScalar },
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.ReferentialConstraintsCanExistBetweenKeyPropertyAndPrimtiveTypeInV20(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidatePrincipalPropertyRefNameShouldBeUnique()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(ValidationTestModelBuilder.PrincipalPropertyRefNameShouldBeUnique(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidatePrincipalPropertyRefShouldCorrespondToDependentPropertyRef()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {22, 6, EdmErrorCode.TypeMismatchRelationshipConstraint},
                {22, 6, EdmErrorCode.TypeMismatchRelationshipConstraint},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.PrincipalPropertyRefShouldCorrespondToDependentPropertyRef(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidatePrinciaplMayNotSpecifyAllTheKeyProperties()
        {
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(ValidationTestModelBuilder.PrinciaplMustSpecifyAllTheKeyProperties(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateDependentPropertyRefNameShouldBeUnique()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {null, null, EdmErrorCode.DuplicateDependentProperty}
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.DependentPropertyRefNameShouldBeUnique(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateOperationImportNameSimpleIdentifier()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.InvalidName },
                {null, null, EdmErrorCode.InvalidName },
                {null, null, EdmErrorCode.InvalidName },
                {null, null, EdmErrorCode.NameTooLong },
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.OperationImportNameSimpleIdentifier(), expectedErrors);
        }

        [TestMethod]
        public void ValidateOperationImportEntitySetMustNotBeSetWhenReturnTypeIsComplexOrSimple()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {29, 6, EdmErrorCode.OperationImportSpecifiesEntitySetButDoesNotReturnEntityType},
                {30, 6, EdmErrorCode.OperationImportSpecifiesEntitySetButDoesNotReturnEntityType},
                {31, 6, EdmErrorCode.OperationImportSpecifiesEntitySetButDoesNotReturnEntityType},
                {32, 6, EdmErrorCode.OperationImportSpecifiesEntitySetButDoesNotReturnEntityType},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.OperationImportEntitySetMustNotBeSetWhenReturnTypeIsComplexOrSimple(this.EdmVersion), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateOperationImportParameterNameShouldBeUnique()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {5, 9, EdmErrorCode.AlreadyDefined },
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.OperationImportParameterNameShouldBeUnique(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateEntitySetNameSimpleIdentifier()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {null, null, EdmErrorCode.InvalidName},
                {null, null, EdmErrorCode.InvalidName},
                {null, null, EdmErrorCode.InvalidName},
                {null, null, EdmErrorCode.InvalidName},
                {null, null, EdmErrorCode.NameTooLong},
                {null, null, EdmErrorCode.DuplicateEntityContainerMemberName}, 
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.EntitySetNameSimpleIdentifier(), expectedErrors);
        }

        [TestMethod]
        public void ValidateOperationNameSimpleIdentifier()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {null, null, EdmErrorCode.DuplicateActions},
                {null, null, EdmErrorCode.InvalidName},
                {null, null, EdmErrorCode.InvalidName},
                {null, null, EdmErrorCode.InvalidName},
                {null, null, EdmErrorCode.InvalidNamespaceName},
                {null, null, EdmErrorCode.InvalidNamespaceName},
                {null, null, EdmErrorCode.InvalidNamespaceName},
                {null, null, EdmErrorCode.InvalidNamespaceName},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.OperationNameSimpleIdentifier(), expectedErrors);
        }

        [TestMethod]
        public void ValidateOperationParameterNameSimpleIdentifier()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.InvalidName},
                {null, null, EdmErrorCode.InvalidName},
                {null, null, EdmErrorCode.InvalidName},
                {null, null, EdmErrorCode.InvalidName},
                {null, null, EdmErrorCode.AlreadyDefined},
                {null, null, EdmErrorCode.NameTooLong},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.OperationParameterNameSimpleIdentifier(), expectedErrors);
        }

        [TestMethod]
        public void ValidateOperationParamterValidTypeTest()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {null, null, EdmErrorCode.BadUnresolvedType},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.OperationParamterValidTypeTest(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateOperationReturnTypeValidTypeTest()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {null, null, EdmErrorCode.BadUnresolvedType}, // this is for referencing an accosiation as a return type
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.OperationReturnTypeValidTypeTest(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateTypeRefTypeIntegrity()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {30, 18, EdmErrorCode.BadUnresolvedType },
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.TypeRefTypeIntegrity(this.EdmVersion), expectedErrors);
        }
        
        [TestMethod]
        public void ValidateEntitySetInaccessibleEntityType()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.BadUnresolvedType }
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.EntitySetInaccessibleEntityType(), expectedErrors);
        }

        [TestMethod]
        public void ValidateSingletonInaccessibleEntityType()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.BadUnresolvedType }
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.SingletonInaccessibleEntityType(), expectedErrors);
        }

        [TestMethod]
        public void ValidateEntityBaseTypeInaccessibleEntityType()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.BadUnresolvedType }
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.EntityBaseTypeInaccessibleEntityType(), expectedErrors);
        }

        [TestMethod]
        public void ValidateEntityReferenceInaccessibleEntityType()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.BadUnresolvedType }
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.EntityReferenceInaccessibleEntityType(), expectedErrors);
        }

        [TestMethod]
        public void ValidateTypeReferenceInaccessibleEntityType()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.BadUnresolvedType }
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.TypeReferenceInaccessibleEntityType(), expectedErrors);
        }

        [TestMethod]
        public void ValidateValidDecimalTypePrecisionValue()
        {
            EdmLibTestErrors expectedErrors = new EdmLibTestErrors()
            {
                {11, 6, EdmErrorCode.PrecisionOutOfRange},
                {11, 6, EdmErrorCode.ScaleOutOfRange },
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.ValidDecimalTypePrecisionValue(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateValidDecimalTypeScaleValue()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {10, 6, EdmErrorCode.ScaleOutOfRange },
                {11, 6, EdmErrorCode.ScaleOutOfRange },
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.ValidDecimalTypeScaleValue(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateValidStringMaxLengthValue()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {null, null, EdmErrorCode.MaxLengthOutOfRange },
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.ValidStringMaxLengthValue(), expectedErrors);
        }

        [TestMethod]
        public void ValidateStringMaxLengthAndUnbounded()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {null, null, EdmErrorCode.IsUnboundedCannotBeTrueWhileMaxLengthIsNotNull },
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.MaxLengthAndUnbounded(), expectedErrors);
        }

        [TestMethod]
        public void ValidateConcurrencyModePropertyTypes()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {12, 6, EdmErrorCode.InvalidPropertyType },
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.ConcurrencyModePropertyTypes(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateBinaryKeyTypeSupportInV40()
        {
            this.VerifySemanticValidation(ValidationTestModelBuilder.BinaryKeyTypeSupportInV40(EdmVersion.V40), EdmVersion.V40, null);
        }

        [TestMethod]
        public void ValidateBinaryKeyTypeWithNegativeMaxLength()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {10, 6, EdmErrorCode.MaxLengthOutOfRange}
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.BinaryKeyTypeWithNegativeMaxLength(EdmVersion.V40), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateNamespaceIsNull()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { "(Microsoft.OData.Edm.Library.EdmFunction)", EdmErrorCode.InvalidNamespaceName},
                { "(.ComplexType)", EdmErrorCode.InvalidNamespaceName}
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.EdmFunctionNamespaceIsEmpty(), expectedErrors);
        }

        [TestMethod]
        public void ValidateValidator()
        {
            EdmLibTestErrors expectedErrors = null;
            this.VerifySemanticValidation(new EdmModel(), expectedErrors);

            IEdmModel testModel = null;
            try
            {
                this.VerifySemanticValidation(testModel, expectedErrors);
                Assert.Fail("Exception should have been thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentNullException, "Correct exception type");
            }
        }

        [TestMethod]
        public void ValidateErrorLocationWhenLineInfoisNotAvailable()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Hot"" Alias=""Fuzz"" xmlns:Bogus=""http://bogus.com/schema"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
</Schema>";
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.KeyMissingOnEntityType }
            };
            this.VerifySemanticValidation(new XElement[] { XElement.Parse(csdl) }, expectedErrors);
        }

        [TestMethod]
        public void ValidateModelBuilderValidModels()
        {
            VerifyValidTestModels(typeof(ModelBuilder));
        }

        [TestMethod]
        public void ValidateModelBuilderModelWithAllConcepts()
        {
            var edmModel = this.GetParserResult(this.GetSerializerResult(ModelBuilder.ModelWithAllConceptsEdm()));
            IEnumerable<EdmError> actualErrors = null;
            edmModel.Validate(toProductVersionlookup[this.EdmVersion], out actualErrors);
            this.CompareErrors(actualErrors, null);
        }

        [TestMethod]
        public void ValidateModelBuilderValidNameCheckModel()
        {
            this.VerifySemanticValidation(ModelBuilder.ValidNameCheckModelEdm(), EdmVersion.V40, null);
        }

        [TestMethod]
        public void ValidateModelBuilderPropertyFacetsCollection()
        {
            EdmLibTestErrors expectedErrors = null;
            this.VerifySemanticValidation(ModelBuilder.PropertyFacetsCollectionEdm(), expectedErrors);
        }

        [TestMethod]
        public void ValidateModelBuilderSimpleAllPrimitiveTypes()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.InvalidKey },
            };
            this.VerifySemanticValidation(ModelBuilder.SimpleAllPrimitiveTypes(this.EdmVersion, true, true), expectedErrors);

            expectedErrors = null;
            this.VerifySemanticValidation(ModelBuilder.SimpleAllPrimitiveTypes(this.EdmVersion, false, true), expectedErrors);
            this.VerifySemanticValidation(ModelBuilder.SimpleAllPrimitiveTypes(this.EdmVersion, true, false), expectedErrors);
            this.VerifySemanticValidation(ModelBuilder.SimpleAllPrimitiveTypes(this.EdmVersion, false, false), expectedErrors);
        }

        [TestMethod]
        public void ValidateValidFindTestModels()
        {
            VerifyValidTestModels(typeof(FindMethodsTestModelBuilder));
        }

        [TestMethod]
        public void ValidateFindTestModelMultipleSchemasWithSameNamespace()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.DuplicateFunctions },
                { null, null, EdmErrorCode.DuplicateFunctions },
            };
            this.VerifySemanticValidation(FindMethodsTestModelBuilder.MultipleSchemasWithSameNamespace(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateValidOperationTestModelOperationStandAloneSchemas()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                // TODO: Need to update after the attached bug is fixed. 
            };
            var model = OperationTestModelBuilder.OperationStandAloneSchemasEdm();
            this.VerifySemanticValidation(model, expectedErrors);
        }

        [TestMethod]
        public void ValidateModelBuilderTaupoDefaultModel()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
            };

            var model = ModelBuilder.TaupoDefaultModelEdm();
            this.VerifySemanticValidation(model, expectedErrors);
        }

        [TestMethod]
        public void ValidateValidOperationTestModelOperationsWithNamedStructuralDataTypeSchemas()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
            };

            var edmModel = OperationTestModelBuilder.OperationsWithNamedStructuralDataTypeSchemasEdm();
            this.VerifySemanticValidation(edmModel, expectedErrors);
        }

        [TestMethod]
        public void ValidateIEdmCheckableClasses()
        {
            var expectedEdmCheckableTypes = new string[] {
                "AmbiguousBinding`1", "AmbiguousEntityContainerBinding",
                "AmbiguousEntitySetBinding", "AmbiguousPropertyBinding", "AmbiguousTypeBinding",
                "AmbiguousValueTermBinding", "BadComplexType", "BadElement",
                "BadEntityContainer", "BadEntityReferenceType", "BadEntitySet",
                "BadEntityType", "BadEnumMember",
                "BadEnumType", "BadNamedStructuredType", "BadPrimitiveType",
                "BadProperty",
                "BadStructuredType",
                "CsdlSemanticsEntityContainer", "CsdlSemanticsNavigationProperty",
                "CyclicComplexType", "CyclicEntityContainer", "CyclicEntityType",
                "UnresolvedComplexType",
                "UnresolvedEntityContainer", "UnresolvedEntitySet", "UnresolvedEntityType",
                "UnresolvedPrimitiveType", "UnresolvedProperty", "UnresolvedType",
               };

            var actualEdmCheckableTypes = typeof(IEdmCheckable).Assembly.GetTypes().Where(t => t.GetInterfaces().Any(n => n == typeof(IEdmCheckable))).Select(m => m.Name).ToList();
            Assert.IsTrue(expectedEdmCheckableTypes.Except(actualEdmCheckableTypes).Count() == 0, "Actual IEdmCheckable classes are diffrent from the list of the expected results.");
        }

        [TestMethod]
        public void VerifyIsBadExtensionMethod()
        {
            ValidationTestModelBuilder.ITestEdmCheckableElement testEdmCheckableElement = new ValidationTestModelBuilder.TestEdmCheckableElement(true);
            Assert.IsTrue(testEdmCheckableElement.IsBad(), "The IsBad method should return true when an element is invalid.");

            testEdmCheckableElement = new ValidationTestModelBuilder.TestEdmCheckableElement(false);
            Assert.IsFalse(testEdmCheckableElement.IsBad(), "The IsBad method should return false when an element is valid.");

            testEdmCheckableElement = new ValidationTestModelBuilder.TestEdmCheckableChildElement(true);
            Assert.IsTrue(testEdmCheckableElement.IsBad(), "The IsBad method should return true when an element is invalid.");

            testEdmCheckableElement = new ValidationTestModelBuilder.TestEdmCheckableChildElement(false);
            Assert.IsFalse(testEdmCheckableElement.IsBad(), "The IsBad method should return false when an element is valid.");

            var testEdmElementWithAssociation = new ValidationTestModelBuilder.TestEdmCheckableChildElement();
            testEdmElementWithAssociation.AssociatedProperty = new ValidationTestModelBuilder.TestEdmAssociatedCheckableElement(true);
            Assert.IsTrue(testEdmElementWithAssociation.IsBad(), "The IsBad method should return true when an associated element is invalid.");

            testEdmElementWithAssociation.AssociatedProperty = new ValidationTestModelBuilder.TestEdmAssociatedCheckableElement(false);
            Assert.IsFalse(testEdmElementWithAssociation.IsBad(), "The IsBad method should return false only when the element and all of its associated elements are valid.");
        }

        [TestMethod]
        public void VerifyIsBadExtensionMethodForPolymorphicVariable()
        {
            foreach (bool booleanValue in new Boolean[] { true, false })
            {
                bool b1 = ((ValidationTestModelBuilder.ITestEdmCheckableElement)new ValidationTestModelBuilder.TestEdmCheckableSpecialChildElement(booleanValue)).IsBad();
                bool b2 = (new ValidationTestModelBuilder.TestEdmCheckableSpecialChildElement(booleanValue)).IsBad();
                Assert.AreEqual(b1, b2, "The IsBad method call on the same object should return the same result.");
            }
        }

        [TestMethod]
        public void VerifyIsBadExtensionMethodForNullError()
        {
            Assert.IsFalse((new ValidationTestModelBuilder.TestEdmAssociatedCheckableNullErrorElement()).IsBad(), "IsBad should be true if if the null Error case means no error.");
        }

        [TestMethod]
        public void VerifyValidatorForNullError()
        {
            EdmModel model = new EdmModel();
            model.AddElement(new ValidationTestModelBuilder.TestEdmAssociatedCheckableNullErrorElement());
            IEnumerable<EdmError> edmError = null;
            model.Validate(out edmError);
            Assert.IsFalse(edmError.Any(), "The number of EdmErrors should be 0 if the null Error case means no error.");
        }

        [TestMethod]
        public void TestCreatingRulesetsWithRuleSubset()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {"(Foo1._!@#$%^&*())", EdmErrorCode.InvalidName},
                {"(Foo2._!@#$%^&*())", EdmErrorCode.InvalidName},
                {"(.)", EdmErrorCode.InvalidName},
                {"(.)",  EdmErrorCode.InvalidNamespaceName},
            };
            ValidationRuleSet ruleset = ValidationRuleSet.GetEdmModelRuleSet(Microsoft.OData.Edm.Library.EdmConstants.EdmVersion4);
            this.VerifySemanticValidation(ValidationTestModelBuilder.IEdmNamedElementNameIsNotAllowed(), ruleset, expectedErrors);

            ruleset = new ValidationRuleSet(ruleset.Except(new ValidationRule[] { ValidationRules.NamedElementNameIsNotAllowed }));
            expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.InvalidName},
                {null, null, EdmErrorCode.InvalidNamespaceName},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.IEdmNamedElementNameIsNotAllowed(), ruleset, expectedErrors);
        }

        [TestMethod]
        public void TestDeclaringTypeMustBeCorrect()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.KeyPropertyMustBelongToEntity},
                {null, null, EdmErrorCode.DeclaringTypeMustBeCorrect},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.DeclaringTypeIncorrect(), expectedErrors);
        }

        [TestMethod]
        public void TestEnumsBeforeV3()
        {
            this.VerifySemanticValidation(ValidationTestModelBuilder.ModelWithEnums(), EdmVersion.V40, null);
        }

        [TestMethod]
        public void TestModelValidationWithComplexTypeWithEntityBaseTypeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {9, 4, EdmErrorCode.BadUnresolvedComplexType},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.ModelWithComplexTypeWithEntityBaseTypeCsdl(EdmVersion.V40), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void TestModelWithEntityTypeWithComplexBaseTypeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {3, 4, EdmErrorCode.BadUnresolvedEntityType}
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.ModelWithEntityTypeWithComplexBaseTypeCsdl(EdmVersion.V40), expectedErrors);
        }

        [TestMethod]
        public void TestEnumMemberTypeMustMatchEnumUnderlyingType()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {"(Microsoft.OData.Edm.Library.EdmEnumMember)", EdmErrorCode.EnumMemberTypeMustMatchEnumUnderlyingType},
                {"(Microsoft.OData.Edm.Library.EdmEnumMember)", EdmErrorCode.EnumMemberTypeMustMatchEnumUnderlyingType},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.ModelWithMismatchedEnumMemberTypes(), expectedErrors);
        }

        [TestMethod]
        public void TestModelInconsistentNavigationPropertyPartner()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.InvalidNavigationPropertyType},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.ModelWithInconsistentNavigationPropertyPartner(), expectedErrors);
        }

        [TestMethod]
        public void TestModelWithInvalidDependentProperties()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.BadUnresolvedType},
                {null, null, EdmErrorCode.DependentPropertiesMustBelongToDependentEntity}
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.ModelWithInvalidDependentProperties(), expectedErrors);
        }

        [TestMethod]
        public void TestAssociationEndInaccessibleEntityType()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.BadUnresolvedType},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.ModelWithAssociationEndAsInaccessibleEntityType(), expectedErrors);
        }

        [TestMethod]
        public void TestInvalidMultiplicityFromRoleUpperBoundMustBeOne()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {16, 6, EdmErrorCode.InvalidMultiplicityOfPrincipalEnd},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.IEdmReferentialConstraintInvalidMultiplicityPrincipleEndMany(EdmVersion.V40), expectedErrors);
        }

        [TestMethod]
        public void TestComplexTypeMustHaveComplexBaseType()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.ComplexTypeMustHaveComplexBaseType},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.ModelWithComplexTypeWithEntityBaseType(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void TestEntityMustHaveEntityBaseType()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.EntityMustHaveEntityBaseType},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.ModelWithEntityTypeWithComplexBaseType(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void TestDependentPropertiesMustBelongToDependentEntity()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.DependentPropertiesMustBelongToDependentEntity},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.ModelWithReferentialConstraintWhoseDependentPropertiesNotPartOfDependentEntity(), expectedErrors);
        }

        [TestMethod]
        // [EdmLib] Constructible NavProps should not take types that are invalid for a nav prop
        public void TestNavigationPropertyCorrectType()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.InvalidNavigationPropertyType},
                {null, null, EdmErrorCode.InvalidNavigationPropertyType},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.ModelWithNavigationPropertyWithBadType(), expectedErrors);
        }

        [TestMethod]
        public void TestOperationWithBadReturnType()
        {
            IEdmModel m = ValidationTestModelBuilder.ModelWithInvalidOperationReturnType();
            IEnumerable<EdmError> errors;
            Assert.IsFalse(m.Validate(out errors), "m.Validate(out errors)");
            Assert.AreEqual(1, errors.Count(), "errors.Count()");
            Assert.IsTrue(errors.Any(e => e.ErrorCode == EdmErrorCode.TypeMustNotHaveKindOfNone), "EdmErrorCode.TypeMustNotHaveKindOfNone");

            // Serialization of a model with invalid types will produce malformed csdl with missing type attributes so serialization fails returning errors
            this.GetSerializerResult(m, out errors);
            Assert.AreEqual(1, errors.Count(), "Error generated on serialization");
        }

        [TestMethod]
        public void TestOperationAndOperationParametersWithBadType()
        {
            IEdmModel m = ValidationTestModelBuilder.ModelWithInvalidOperationParameterType();
            IEnumerable<EdmError> errors;
            Assert.IsFalse(m.Validate(out errors), "m.Validate(out errors)");
            Assert.AreEqual(1, errors.Count(), "errors.Count()");
            Assert.IsTrue(errors.Any(e => e.ErrorCode == EdmErrorCode.TypeMustNotHaveKindOfNone), "EdmErrorCode.TypeMustNotHaveKindOfNone");

            // Serialization of a model with invalid types will produce malformed csdl with missing type attributes so serialization fails returning errors
            this.GetSerializerResult(m, out errors);
            Assert.AreEqual(1, errors.Count(), "Error generated on serialization");

        }

        [TestMethod]
        public void TestOperationImportWithBadReturnType()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.TypeMustNotHaveKindOfNone},
            };

            this.VerifySemanticValidation(ValidationTestModelBuilder.ModelWithInvalidFunctionReturnType(), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void TestOperationImportEntitySetExpressionIsInvalid2()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.InvalidPathFirstPathParameterNotMatchingFirstParameterName},
                {null, null, EdmErrorCode.InvalidPathFirstPathParameterNotMatchingFirstParameterName},
                {null, null, EdmErrorCode.InvalidPathUnknownNavigationProperty},
                {null, null, EdmErrorCode.InvalidPathUnknownNavigationProperty},
                {null, null, EdmErrorCode.OperationWithEntitySetPathResolvesToCollectionEntityTypeMismatchesEntityTypeReturnType},
                {null, null, EdmErrorCode.OperationWithEntitySetPathAndReturnTypeTypeNotAssignable},
                {null, null, EdmErrorCode.InvalidPathInvalidTypeCastSegment},
                {null, null, EdmErrorCode.OperationImportEntitySetExpressionIsInvalid},
                {null, null, EdmErrorCode.InvalidPathUnknownTypeCastSegment},
                {null, null, EdmErrorCode.OperationImportEntitySetExpressionIsInvalid},
                {null, null, EdmErrorCode.InvalidPathUnknownNavigationProperty},
                {null, null, EdmErrorCode.OperationImportEntitySetExpressionIsInvalid},
                {null, null, EdmErrorCode.InvalidPathUnknownNavigationProperty},
                {null, null, EdmErrorCode.OperationImportEntitySetExpressionIsInvalid},
                {null, null, EdmErrorCode.InvalidPathUnknownNavigationProperty},
                {null, null, EdmErrorCode.OperationImportEntitySetExpressionIsInvalid},
                {null, null, EdmErrorCode.InvalidPathUnknownNavigationProperty},
                {null, null, EdmErrorCode.NavigationMappingMustBeBidirectional},
                {null, null, EdmErrorCode.NavigationMappingMustBeBidirectional},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.ModelWithInvalidOperationImportEntitySet2(), expectedErrors);
        }

        [TestMethod]
        public void ConstructElementAnnotations()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.InvalidElementAnnotation},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.ModelWithBadElementAnnotation(), expectedErrors);
        }

        [TestMethod]
        public void SemanticValidateODataTestModelBasicTest()
        {
            var expectedErrors = new EdmLibTestErrors() { };
            this.VerifySemanticValidation(ODataTestModelBuilder.ODataTestModelBasicTest, expectedErrors);
        }

        [TestMethod]
        public void SemanticValidateODataTestModelAnnotationTestWithAnnotations()
        {
            var expectedErrors = new EdmLibTestErrors() { };
            this.VerifySemanticValidation(ODataTestModelBuilder.ODataTestModelAnnotationTestWithAnnotations, expectedErrors);
        }

        [TestMethod]
        public void SemanticValidateODataTestModelAnnotationTestWithoutAnnotations()
        {
            var expectedErrors = new EdmLibTestErrors() { };
            this.VerifySemanticValidation(ODataTestModelBuilder.ODataTestModelAnnotationTestWithoutAnnotations, expectedErrors);
        }

        [TestMethod]
        public void SemanticValidateODataTestModelWithFunctionImport()
        {
            var expectedErrors = new EdmLibTestErrors() { };
            this.VerifySemanticValidation(ODataTestModelBuilder.ODataTestModelWithFunctionImport, expectedErrors);
        }

        [TestMethod]
        public void SemanticValidateODataTestModelWithActionImport()
        {
            var expectedErrors = new EdmLibTestErrors() { };
            this.VerifySemanticValidation(ODataTestModelBuilder.ODataTestModelWithActionImport, expectedErrors);
        }

        [TestMethod]
        public void SemanticValidateODataTestModelDefaultModel()
        {
            var expectedErrors = new EdmLibTestErrors() { };
            this.VerifySemanticValidation(ODataTestModelBuilder.ODataTestModelDefaultModel, expectedErrors);
        }

        [TestMethod]
        public void SemanticValidateODataTestModelEmptyModel()
        {
            var expectedErrors = new EdmLibTestErrors() { };
            this.VerifySemanticValidation(ODataTestModelBuilder.ODataTestModelEmptyModel, expectedErrors);
        }

        [TestMethod]
        public void SemanticValidateODataTestModelWithSingleEntityType()
        {
            var expectedErrors = new EdmLibTestErrors() { };
            this.VerifySemanticValidation(ODataTestModelBuilder.ODataTestModelWithSingleEntityType, expectedErrors);
        }

        [TestMethod]
        public void SemanticValidateODataTestModelWithSingleComplexType()
        {
            var expectedErrors = new EdmLibTestErrors() { };
            this.VerifySemanticValidation(ODataTestModelBuilder.ODataTestModelWithSingleComplexType, expectedErrors);
        }

        [TestMethod]
        public void SemanticValidateODataTestModelWithCollectionProperty()
        {
            var expectedErrors = new EdmLibTestErrors() { };
            this.VerifySemanticValidation(ODataTestModelBuilder.ODataTestModelWithCollectionProperty, expectedErrors);
        }

        [TestMethod]
        public void SemanticValidateODataTestModelWithOpenType()
        {
            var expectedErrors = new EdmLibTestErrors() { };
            this.VerifySemanticValidation(ODataTestModelBuilder.ODataTestModelWithOpenType, expectedErrors);
        }

        [TestMethod]
        public void SemanticValidateODataTestModelWithNamedStream()
        {
            var expectedErrors = new EdmLibTestErrors() { };
            this.VerifySemanticValidation(ODataTestModelBuilder.ODataTestModelWithNamedStream, expectedErrors);
        }

        [TestMethod]
        public void TestNavigationPropertyContainTargetMultiplicity()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne},
                {null, null, EdmErrorCode.NavigationPropertyWithRecursiveContainmentTargetMustBeOptional},
                {null, null, EdmErrorCode.NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne},
                {null, null, EdmErrorCode.EntitySetCanOnlyBeContainedByASingleNavigationProperty},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.ModelWithNavigationPropertyWithContainsTargetBadMultiplicity(), expectedErrors);
        }

        [TestMethod]
        public void TestNavigationPropertyRecursiveContainsEntitySets()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {16, 10, EdmErrorCode.EntitySetRecursiveNavigationPropertyMappingsMustPointBackToSourceEntitySet},
                {20, 10, EdmErrorCode.EntitySetRecursiveNavigationPropertyMappingsMustPointBackToSourceEntitySet},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.ModelWithNavigationPropertyWithRecursiveContainsNonSameEntitySet(), expectedErrors);
        }

        [TestMethod]
        public void TestBidirectionalContainment()
        {
            var expectedErrors = new EdmLibTestErrors() 
            {
                {"(Microsoft.OData.Edm.Library.EdmNavigationProperty)", EdmErrorCode.NavigationPropertyEntityMustNotIndirectlyContainItself},
                {"(Microsoft.OData.Edm.Library.EdmNavigationProperty)", EdmErrorCode.NavigationPropertyEntityMustNotIndirectlyContainItself},
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.BidirectionalContainmentModel(), expectedErrors);
        }

        [TestMethod]
        public void TestNavigationPropertyContainTargetBeforeV3()
        {
            this.VerifySemanticValidation(ValidationTestModelBuilder.ModelWithNavigationPropertyWithContainsTarget(), EdmVersion.V40, null);
        }

        [TestMethod]
        public void TestOperationImportParameterNullable()
        {
            var csdl = @"
    <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <Action Name=""SimpleFunction"">
        <Parameter Name=""Count"" Type=""Edm.Int32"" Nullable=""true"" />
      </Action>
      <EntityContainer Name=""Container"">
        <ActionImport Name=""SimpleFunction"" Action=""DefaultNamespace.SimpleFunction"" />
      </EntityContainer>
    </Schema>";
            var model = this.GetParserResult(new List<String>() { csdl });

            var expectedErrors = new EdmLibTestErrors()
            {
            };

            this.VerifySemanticValidation(model, EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void TestModelWithComplexTypeWithEntityBaseTypeCsdl()
        {
            var csdl = @"
<Schema Namespace=""Foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Bar"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int16"" Nullable=""false"" />
  </EntityType>
  <ComplexType Name=""Baz"" BaseType=""Foo.Bar"">
    <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
</Schema>";
            var model = this.GetParserResult(new List<String>() { csdl });
            var expectedErrors = new EdmLibTestErrors() 
            {
                {null, null, EdmErrorCode.BadUnresolvedComplexType},
            };
            this.VerifySemanticValidation(model, expectedErrors);
        }

        [TestMethod]
        public void TestModelWithEnumValueTerm()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.ModelWithEnumValueTerm(), expectedErrors);
        }

        [TestMethod]
        public void ValidatingUsingWithThreeDifferentUsing()
        {
            const string modelCsdl =
@"<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Person"">
        <Property Name=""Name"" Type=""alias1.Name"" />
        <Property Name=""Number"" Type=""alias2.Number"" />
        <Property Name=""Address"" Type=""alias3.Address"" />
    </ComplexType>
</Schema>";

            const string fooCsdl =
@"<Schema Namespace=""foo"" Alias=""alias1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Name"">
        <Property Name=""Data"" Type=""Edm.String"" />
    </ComplexType>
</Schema>";

            const string barCsdl =
@"<Schema Namespace=""bar"" Alias=""alias2"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Number"">
        <Property Name=""Data"" Type=""Edm.String"" />
    </ComplexType>
</Schema>";

            const string foobarCsdl =
@"<Schema Namespace=""foobaz"" Alias=""alias3"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Address"">
        <Property Name=""Data"" Type=""Edm.String"" />
    </ComplexType>
</Schema>";

            IEdmModel model = this.GetParserResult(new string[] { modelCsdl, fooCsdl, barCsdl, foobarCsdl });

            var expectedError = new EdmLibTestErrors() { };
            this.VerifySemanticValidation(model, Microsoft.Test.OData.Utils.Metadata.EdmVersion.Latest, expectedError);

            var person = model.FindType("DefaultNamespace.Person") as IEdmComplexType;
            Assert.IsNotNull(person, "Invalid complex type qualifier.");
            Assert.AreEqual("foo.Name", person.FindProperty("Name").Type.FullName(), "Invalid type full name.");
            Assert.AreEqual("bar.Number", person.FindProperty("Number").Type.FullName(), "Invalid type full name.");
            Assert.AreEqual("foobaz.Address", person.FindProperty("Address").Type.FullName(), "Invalid type full name.");
        }

        [TestMethod]
        public void ValidatingDifferentNamespaceSameAliasInSameModel()
        {
            const string modelCsdl =
@"<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Person"">
        <Property Name=""Name"" Type=""alias1.Name"" />
        <Property Name=""Number"" Type=""alias1.Number"" />
    </ComplexType>
</Schema>";

            const string fooCsdl =
@"<Schema Namespace=""foo"" Alias=""alias1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Name"" />
</Schema>";

            const string barCsdl =
@"<Schema Namespace=""bar"" Alias=""alias1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Number"" />
</Schema>";

            IEdmModel model = this.GetParserResult(new string[] { modelCsdl, fooCsdl, barCsdl });

            var expectedError = new EdmLibTestErrors() 
            { 
                {null, null, EdmErrorCode.DuplicateAlias},
            };

            this.VerifySemanticValidation(model, Microsoft.Test.OData.Utils.Metadata.EdmVersion.Latest, expectedError);
        }

        [TestMethod]
        public void ReferencedModelUnresolvedTypeValidationTest()
        {
            var mainModel = new FunctionalUtilities.ModelWithRemovableElements<EdmModel>(ValidationTestModelBuilder.ReferenceBasicTestMainModel() as EdmModel);
            var referencedModel = ValidationTestModelBuilder.ReferenceBasicTestReferencedModel();

            var partnerType = new EdmEntityType("NS1", "Partner", referencedModel.FindEntityType("NS1.Person"), false, true);
            partnerType.AddStructuralProperty("CustomerID", EdmCoreModel.Instance.GetString(false));
            partnerType.AddStructuralProperty("Region", new EdmComplexTypeReference(referencedModel.FindType("NS1.Region") as EdmComplexType, false));
            mainModel.WrappedModel.AddElement(partnerType);

            IEnumerable<EdmError> edmErrors;
            mainModel.Validate(out edmErrors);
            var expectedErrors = new EdmLibTestErrors()
            {
                { "(NS1.Partner)", EdmErrorCode.BadUnresolvedType},
                { "([NS1.Region Nullable=False])", EdmErrorCode.BadUnresolvedType},
            };
            this.CompareErrors(edmErrors, expectedErrors);

            mainModel.WrappedModel.AddReferencedModel(referencedModel);
            mainModel.Validate(out edmErrors);
            expectedErrors = null;
            this.CompareErrors(edmErrors, expectedErrors);

            mainModel.RemoveReference(referencedModel);
            mainModel.Validate(out edmErrors);
            expectedErrors = new EdmLibTestErrors()
            {
                { "(NS1.Partner)", EdmErrorCode.BadUnresolvedType},
                { "([NS1.Region Nullable=False])", EdmErrorCode.BadUnresolvedType},
            };
            this.CompareErrors(edmErrors, expectedErrors);
        }

        [TestMethod]
        public void ReferencedModelUnresolvedTermValidationTest()
        {
            var mainModel = new FunctionalUtilities.ModelWithRemovableElements<EdmModel>(ValidationTestModelBuilder.ReferenceBasicTestMainModel() as EdmModel);
            var referencedModel = ValidationTestModelBuilder.ReferenceBasicTestReferencedModel();

            var valueAnnotation = new EdmAnnotation(
                mainModel.FindEntityType("NS1.Customer"),
                referencedModel.FindValueTerm("NS1.Title"),
                "q1",
                new EdmStringConstant("Hello world!"));
            mainModel.WrappedModel.AddVocabularyAnnotation(valueAnnotation);

            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.BadUnresolvedTerm},
            };
            IEnumerable<EdmError> edmErrors;
            mainModel.Validate(out edmErrors);
            this.CompareErrors(edmErrors, expectedErrors);

            mainModel.WrappedModel.AddReferencedModel(referencedModel);
            expectedErrors = null;
            mainModel.Validate(out edmErrors);
            this.CompareErrors(edmErrors, expectedErrors);

            mainModel.RemoveReference(referencedModel);
            expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.BadUnresolvedTerm},
            };
            mainModel.Validate(out edmErrors);
            this.CompareErrors(edmErrors, expectedErrors);
        }

        [TestMethod]
        public void ReferencedModelDuplicatePropertyValidationTest()
        {
            var mainModel = new FunctionalUtilities.ModelWithRemovableElements<EdmModel>(ValidationTestModelBuilder.ReferenceBasicTestMainModel() as EdmModel);
            var referencedModel = ValidationTestModelBuilder.ReferenceBasicTestReferencedModel();

            var partnerType = new EdmEntityType("NS1", "Partner", referencedModel.FindEntityType("NS1.Person"), false, true);
            var partnerId = partnerType.AddStructuralProperty("ID2", EdmCoreModel.Instance.GetString(false));
            EdmEntityType regionType = new EdmEntityType("NS1", "Region", null, false, true);
            var regionId = regionType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetString(false));
            regionType.AddKeys(regionId);
            mainModel.WrappedModel.AddElement(partnerType);
            mainModel.WrappedModel.AddElement(regionType);

            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.BadUnresolvedType },
            };
            IEnumerable<EdmError> edmErrors;
            mainModel.Validate(out edmErrors);
            this.CompareErrors(edmErrors, expectedErrors);

            mainModel.WrappedModel.AddReferencedModel(referencedModel);
            expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.AlreadyDefined },
            };
            mainModel.Validate(out edmErrors);
            this.CompareErrors(edmErrors, expectedErrors);

            mainModel.RemoveReference(referencedModel);
            expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.BadUnresolvedType },
            };
            mainModel.Validate(out edmErrors);
            this.CompareErrors(edmErrors, expectedErrors);
        }

        [TestMethod]
        public void ReferencedModelDuplicateTypeValidationTest()
        {
            var mainModel = new FunctionalUtilities.ModelWithRemovableElements<EdmModel>(ValidationTestModelBuilder.ReferenceBasicTestMainModel() as EdmModel);
            var referencedModel = ValidationTestModelBuilder.ReferenceBasicTestReferencedModel();

            var regionType = new EdmComplexType("NS1", "Region");
            regionType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            mainModel.WrappedModel.AddElement(regionType);

            var expectedErrors = new EdmLibTestErrors();
            IEnumerable<EdmError> edmErrors;
            mainModel.Validate(out edmErrors);
            this.CompareErrors(edmErrors, expectedErrors);

            mainModel.WrappedModel.AddReferencedModel(referencedModel);
            expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.AlreadyDefined },
            };
            mainModel.Validate(out edmErrors);
            this.CompareErrors(edmErrors, expectedErrors);

            mainModel.RemoveReference(referencedModel);
            expectedErrors = null;
            mainModel.Validate(out edmErrors);
            this.CompareErrors(edmErrors, expectedErrors);
        }

        [Ignore] // Ignore until we implement edmx:Reference to give alias to Referenced model.
        [TestMethod]
        public void ValidatingModelWithDuplicateReferences()
        {
            const string modelCsdl =
@"<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Using Alias=""alias1"" Namespace=""foo"" />
    <Using Alias=""alias2"" Namespace=""bar"" />
    <Using Alias=""alias3"" Namespace=""foobaz"" />
    <ComplexType Name=""Person"">
        <Property Name=""Name"" Type=""alias1.Name"" />
        <Property Name=""Number"" Type=""alias2.Number"" />
        <Property Name=""Address"" Type=""alias3.Address"" />
    </ComplexType>
</Schema>";

            const string fooCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Name"" />
</Schema>";

            const string barCsdl =
@"<Schema Namespace=""bar"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Number"" />
</Schema>";

            const string foobarCsdl =
@"<Schema Namespace=""foobaz"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Address"" />
</Schema>";

            var fooModel = this.GetParserResult(new string[] { fooCsdl });
            var barModel = this.GetParserResult(new string[] { barCsdl });
            var foobarModel = this.GetParserResult(new string[] { foobarCsdl });

            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.BadAmbiguousElementBinding },
            };
            var edmModel = this.GetParserResult(new string[] { modelCsdl }, fooModel, fooModel, barModel, foobarModel);
            IEnumerable<EdmError> edmErrors;
            edmModel.Validate(out edmErrors);
            this.CompareErrors(edmErrors, expectedErrors);

        }

        [TestMethod]
        public void ValidateCollectionTypeTypeRefSimpleTypeCanHaveFacets()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.CollectionTypeTypeRefSimpleTypeCanHaveFacets(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateTypeRefValidTypes()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.TypeRefValidTypes(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidateConcurrencyModeTypes()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                {8, 6, EdmErrorCode.InvalidPropertyType}
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.ConcurrencyModeTypes(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void DuplicateNavigationPropertyMappingsOkayIfPropertiesAreSilent()
        {
            string csdl = @"
<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Content"">
      <Key>
          <PropertyRef Name=""ID"" />
      </Key>
      <Property Name=""ID"" Type=""Int32"" Nullable=""false"" />
      <NavigationProperty Name=""Fields"" Type=""Collection(NS1.Field)"" ContainsTarget=""true"" />
    </EntityType>
    <EntityType Name=""List"">
      <Key>
          <PropertyRef Name=""ID"" />
      </Key>
      <Property Name=""ID"" Type=""Int32"" Nullable=""false"" />
      <NavigationProperty Name=""Fields"" Type=""Collection(NS1.Field)"" ContainsTarget=""true"" />
    </EntityType>
    <EntityType Name=""Field"">
      <Key>
          <PropertyRef Name=""ID"" />
      </Key>
      <Property Name=""ID"" Type=""Int32"" Nullable=""false"" />
      <NavigationProperty Name=""Description"" Type=""NS1.Description"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""Description"">
      <Key>
          <PropertyRef Name=""ID"" />
      </Key>
      <Property Name=""ID"" Type=""Int32"" Nullable=""false"" />
    </EntityType>
    <EntityContainer Name=""C1"">
      <EntitySet Name=""Contents"" EntityType=""NS1.Content"">
        <NavigationPropertyBinding Path=""Fields"" Target=""ContentFields"" />
      </EntitySet>
      <EntitySet Name=""Lists"" EntityType=""NS1.List"">
        <NavigationPropertyBinding Path=""Fields"" Target=""ListFields"" />
      </EntitySet>
      <EntitySet Name=""ContentFields"" EntityType=""NS1.Field"">
        <NavigationPropertyBinding Path=""Description"" Target=""Descriptions"" />
      </EntitySet>
      <EntitySet Name=""ListFields"" EntityType=""NS1.Field"">
        <NavigationPropertyBinding Path=""Description"" Target=""Descriptions"" />
      </EntitySet>
      <EntitySet Name=""Descriptions"" EntityType=""NS1.Description"" />
    </EntityContainer>
</Schema>";

            IEdmModel model = this.GetParserResult(new string[] { csdl });

            var expectedError = new EdmLibTestErrors()
            {
            };
            this.VerifySemanticValidation(model, Microsoft.Test.OData.Utils.Metadata.EdmVersion.Latest, expectedError);
        }

        [TestMethod]
        public void DuplicateNavigationPropertyMappingsInEntitySet()
        {
            string csdl = @"
<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Content"">
      <Key>
          <PropertyRef Name=""ID"" />
      </Key>
      <Property Name=""ID"" Type=""Int32"" Nullable=""false"" />
      <NavigationProperty Name=""Fields"" Type=""Collection(NS1.Field)"" ContainsTarget=""true"" />
    </EntityType>
    <EntityType Name=""Field"">
      <Key>
          <PropertyRef Name=""ID"" />
      </Key>
      <Property Name=""ID"" Type=""Int32"" Nullable=""false"" />
    </EntityType>
    <EntityContainer Name=""C1"">
      <EntitySet Name=""Contents"" EntityType=""NS1.Content"">
        <NavigationPropertyBinding Path=""Fields"" Target=""ContentFields"" />
        <NavigationPropertyBinding Path=""Fields"" Target=""ListFields"" />
      </EntitySet>
      <EntitySet Name=""ContentFields"" EntityType=""NS1.Field"" />
      <EntitySet Name=""ListFields"" EntityType=""NS1.Field"" />
    </EntityContainer>
</Schema>";

            IEdmModel model = this.GetParserResult(new string[] { csdl });

            var expectedError = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.DuplicateNavigationPropertyMapping }
            };
            this.VerifySemanticValidation(model, Microsoft.Test.OData.Utils.Metadata.EdmVersion.Latest, expectedError);
        }

        [TestMethod]
        public void DuplicateNavigationPropertyMappingsInSingleton()
        {
            string csdl = @"
<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Content"">
      <Key>
          <PropertyRef Name=""ID"" />
      </Key>
      <Property Name=""ID"" Type=""Int32"" Nullable=""false"" />
      <NavigationProperty Name=""Fields"" Type=""Collection(NS1.Field)"" ContainsTarget=""true"" />
    </EntityType>
    <EntityType Name=""Field"">
      <Key>
          <PropertyRef Name=""ID"" />
      </Key>
      <Property Name=""ID"" Type=""Int32"" Nullable=""false"" />
    </EntityType>
    <EntityContainer Name=""C1"">
      <Singleton Name=""Contents"" Type=""NS1.Content"">
        <NavigationPropertyBinding Path=""Fields"" Target=""ContentFields"" />
        <NavigationPropertyBinding Path=""Fields"" Target=""ListFields"" />
      </Singleton>
      <EntitySet Name=""ContentFields"" EntityType=""NS1.Field"" />
      <EntitySet Name=""ListFields"" EntityType=""NS1.Field"" />
    </EntityContainer>
</Schema>";

            IEdmModel model = this.GetParserResult(new string[] { csdl });

            var expectedError = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.DuplicateNavigationPropertyMapping }
            };
            this.VerifySemanticValidation(model, Microsoft.Test.OData.Utils.Metadata.EdmVersion.Latest, expectedError);
        }

        [TestMethod]
        public void MultiplyContainedRegressionTest()
        {
            string csdl = @"
<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""SharePointModel"">
        <EntitySet Name=""Lists"" EntityType=""NS1.List"">
          <NavigationPropertyBinding Path=""Fields"" Target=""ListFields"" />
        </EntitySet>
        <EntitySet Name=""ContentTypes"" EntityType=""NS1.ContentType"">
          <NavigationPropertyBinding Path=""Fields"" Target=""ContentTypeFields"" />
        </EntitySet>
        <EntitySet Name=""ListFields"" EntityType=""NS1.Field"">
          <NavigationPropertyBinding Path=""FieldInfos"" Target=""FieldInfos"" />
        </EntitySet>
        <EntitySet Name=""ContentTypeFields"" EntityType=""NS1.Field"">
          <NavigationPropertyBinding Path=""FieldInfos"" Target=""FieldInfos"" />
        </EntitySet>
        <EntitySet Name=""FieldInfos"" EntityType=""NS1.FieldInfo""/>
    </EntityContainer>
    <EntityType Name=""List"">
        <Key>
            <PropertyRef Name=""ID""/>
        </Key>
        <Property Name=""ID"" Type=""Guid"" Nullable=""false""/>
        <NavigationProperty Name=""Fields"" Type=""Collection(NS1.Field)"" ContainsTarget=""true""/>
    </EntityType>
    <EntityType Name=""ContentType"">
        <Key>
            <PropertyRef Name=""ID""/>
        </Key>
        <Property Name=""ID"" Type=""Guid"" Nullable=""false""/>
        <NavigationProperty Name=""Fields"" Type=""Collection(NS1.Field)"" ContainsTarget=""true""/>
    </EntityType>
    <EntityType Name=""Field"">
        <Key>
            <PropertyRef Name=""ID""/>
        </Key>
        <Property Name=""ID"" Type=""Int32"" Nullable=""false""/>
        <NavigationProperty Name=""FieldInfos"" Type=""Collection(NS1.FieldInfo)"" ContainsTarget=""true""/>
    </EntityType>
    <EntityType Name=""FieldInfo"">
        <Key>
            <PropertyRef Name=""ID""/>
        </Key>
        <Property Name=""ID"" Type=""Int32"" Nullable=""false""/>
    </EntityType>
</Schema>
";

            IEdmModel model = this.GetParserResult(new string[] { csdl });

            var expectedError = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.EntitySetCanOnlyBeContainedByASingleNavigationProperty }
            };
            this.VerifySemanticValidation(model, Microsoft.Test.OData.Utils.Metadata.EdmVersion.Latest, expectedError);
        }

        [TestMethod]
        public void ValidateDerivedEntityTypeWithFixedConcurrencyModeModel()
        {
            var expectedErrors = new EdmLibTestErrors();
            var model = ModelBuilder.DerivedEntityTypeWithFixedConcurrencyModeModel();

            this.VerifySemanticValidation(model, Microsoft.Test.OData.Utils.Metadata.EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateDerivedEntityTypeWithFixedConcurrencyModeCsdl()
        {
            var expectedErrors = new EdmLibTestErrors();
            var csdl = ModelBuilder.DerivedEntityTypeWithFixedConcurrencyModeCsdl();
            var model = this.GetParserResult(csdl);

            this.VerifySemanticValidation(model, Microsoft.Test.OData.Utils.Metadata.EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void TestModelWithCircularNavigationPartner()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { "(EdmLibTests.StubEdm.StubEdmNavigationProperty)", EdmErrorCode.InterfaceCriticalNavigationPartnerInvalid }
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.ModelWithCircularNavigationPartner(), expectedErrors);
        }

        [TestMethod]
        public void OneWayNavigationPropertyMultipleMappingTest()
        {
            EdmModel model = new EdmModel();

            EdmEntityType t3 = new EdmEntityType("Bunk", "T3");
            EdmStructuralProperty f31 = t3.AddStructuralProperty("F31", EdmCoreModel.Instance.GetString(false));
            t3.AddKeys(f31);
            model.AddElement(t3);

            EdmEntityType t1 = new EdmEntityType("Bunk", "T1");
            EdmStructuralProperty f11 = t1.AddStructuralProperty("F11", EdmCoreModel.Instance.GetString(false));
            t1.AddKeys(f11);
            EdmNavigationProperty p101 = t1.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "P101", Target = t3, TargetMultiplicity = EdmMultiplicity.One });
            model.AddElement(t1);

            EdmEntityContainer c1 = new EdmEntityContainer("Bunk", "Gunk");
            EdmEntitySet E1a = c1.AddEntitySet("E1a", t1);
            EdmEntitySet E1b = c1.AddEntitySet("E1b", t1);
            EdmEntitySet E3 = c1.AddEntitySet("E3", t3);
            E1a.AddNavigationTarget(p101, E3);
            E1b.AddNavigationTarget(p101, E3);
            model.AddElement(c1);

            IEnumerable<EdmError> edmErrors;
            model.Validate(out edmErrors);
            Assert.IsTrue(!edmErrors.Any(), @"Enable multiple mappings for the same (silent) navigation property");

            this.GetParserResult(this.GetSerializerResult(model)).Validate(out edmErrors);
            Assert.IsTrue(!edmErrors.Any(), @"Enable multiple mappings for the same (silent) navigation property");
        }

        [TestMethod]
        public void TwoWayNavigationPropertyMultipleMappingTest()
        {
            var model = new EdmModel();
            var t3 = new EdmEntityType("Bunk", "T3");
            var f31 = t3.AddStructuralProperty("F31", EdmCoreModel.Instance.GetString(false));
            t3.AddKeys(f31);
            model.AddElement(t3);

            var t1 = new EdmEntityType("Bunk", "T1");
            var f11 = t1.AddStructuralProperty("F11", EdmCoreModel.Instance.GetString(false));
            t1.AddKeys(f11);
            var p101 = t1.AddBidirectionalNavigation
                                    (
                                        new EdmNavigationPropertyInfo() { Name = "P101", Target = t3, TargetMultiplicity = EdmMultiplicity.One },
                                        new EdmNavigationPropertyInfo() { Name = "P301", TargetMultiplicity = EdmMultiplicity.One }
                                    );
            model.AddElement(t1);

            var c1 = new EdmEntityContainer("Bunk", "Gunk");
            var E1a = c1.AddEntitySet("E1a", t1);
            var E1b = c1.AddEntitySet("E1b", t1);
            var E3 = c1.AddEntitySet("E3", t3);
            E1a.AddNavigationTarget(p101, E3);
            E1b.AddNavigationTarget(p101, E3);
            E3.AddNavigationTarget(p101.Partner, E1a);
            model.AddElement(c1);

            var expectedErrors = new EdmLibTestErrors()
            {
                { "(Microsoft.OData.Edm.Library.EdmEntitySet)", EdmErrorCode.NavigationMappingMustBeBidirectional },
            };
            IEnumerable<EdmError> edmErrors;
            model.Validate(out edmErrors);
            this.CompareErrors(edmErrors, expectedErrors);
        }

        [TestMethod]
        public void TwoWayNavigationPropertyMultipleMappingInSingletonTest()
        {
            var model = new EdmModel();
            var t3 = new EdmEntityType("Bunk", "T3");
            var f31 = t3.AddStructuralProperty("F31", EdmCoreModel.Instance.GetString(false));
            t3.AddKeys(f31);
            model.AddElement(t3);

            var t1 = new EdmEntityType("Bunk", "T1");
            var f11 = t1.AddStructuralProperty("F11", EdmCoreModel.Instance.GetString(false));
            t1.AddKeys(f11);
            var p101 = t1.AddBidirectionalNavigation
                                    (
                                        new EdmNavigationPropertyInfo() { Name = "P101", Target = t3, TargetMultiplicity = EdmMultiplicity.One },
                                        new EdmNavigationPropertyInfo() { Name = "P301", TargetMultiplicity = EdmMultiplicity.One }
                                    );
            model.AddElement(t1);

            var c1 = new EdmEntityContainer("Bunk", "Gunk");
            var E1a = c1.AddEntitySet("E1a", t1);
            var E1b = c1.AddSingleton("E1b", t1);
            var E3 = c1.AddSingleton("E3", t3);
            E1a.AddNavigationTarget(p101, E3);
            E1b.AddNavigationTarget(p101, E3);
            E3.AddNavigationTarget(p101.Partner, E1a);
            model.AddElement(c1);

            var expectedErrors = new EdmLibTestErrors()
            {
                { "(Microsoft.OData.Edm.Library.EdmSingleton)", EdmErrorCode.NavigationMappingMustBeBidirectional },
            };
            IEnumerable<EdmError> edmErrors;
            model.Validate(out edmErrors);
            this.CompareErrors(edmErrors, expectedErrors);
        }

        [TestMethod]
        public void AnnotationNameWhichIsNotSimpleIdentifierShouldPassValidation()
        {
            var model = new EdmModel();
            model.SetEdmVersion(Microsoft.OData.Edm.Library.EdmConstants.EdmVersion4);
            var fredFlintstone = new EdmComplexType("Flintstones", "Fred");
            fredFlintstone.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            model.AddElement(fredFlintstone);

            // set the annotation name to a value which is valid XML, but not a valid SimpleIdentifier.
            model.SetAnnotationValue(fredFlintstone, "sap", "content-version", new EdmStringConstant(EdmCoreModel.Instance.GetString(false), "hello"));

            Assert.AreEqual(1, model.DirectValueAnnotations(fredFlintstone).Count(), "Wrong # of Annotations on {0}.", fredFlintstone);

            IEnumerable<EdmError> errors;
            model.Validate(out errors);
            Assert.AreEqual(0, errors.Count(), "Model expected to be valid.");
        }

        [TestMethod]
        public void ValidationShouldFailOnNonXmlValidAnnotation()
        {
            var model = new EdmModel();
            var fredFlintstone = new EdmComplexType("Flintstones", "Fred");
            fredFlintstone.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            model.AddElement(fredFlintstone);

            // Set annnotation which would be serialized as an attribute to be a value that is invalid per the xml spec.
            model.SetAnnotationValue(fredFlintstone, "sap", "-content-version", new EdmStringConstant(EdmCoreModel.Instance.GetString(false), "hello"));

            Assert.AreEqual(1, model.DirectValueAnnotations(fredFlintstone).Count(), "Wrong # of Annotations on {0}.", fredFlintstone);

            IEnumerable<EdmError> errors;
            model.Validate(out errors);
            Assert.AreEqual(1, errors.Count(), "Model expected to be invalid.");
            EdmError error = errors.Single();
            Assert.AreEqual(error.ErrorCode, EdmErrorCode.InvalidName, "Unexpected error code");
            Assert.AreEqual(error.ErrorMessage, "The specified name is not allowed: '-content-version'.", "Unexpected error message");
        }

        [TestMethod]
        public void ValidationShouldFailOnNonXmlValidAnnotationWhichWontBeSerialized()
        {
            var model = new EdmModel();
            var fredFlintstone = new EdmComplexType("Flintstones", "Fred");
            fredFlintstone.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            model.AddElement(fredFlintstone);

            // Making the value of the annotation not an IEdmValue means that it won't be serialized out as an attribute annotation.
            // We should still fail on this anyway, since being strict now has fewer consequences than trying to introduce strictness later.
            model.SetAnnotationValue(fredFlintstone, "sap", "-content-version", 1);

            Assert.AreEqual(1, model.DirectValueAnnotations(fredFlintstone).Count(), "Wrong # of Annotations on {0}.", fredFlintstone);

            IEnumerable<EdmError> errors;
            model.Validate(out errors);
            Assert.AreEqual(1, errors.Count(), "Model expected to be invalid.");
            EdmError error = errors.Single();
            Assert.AreEqual(error.ErrorCode, EdmErrorCode.InvalidName, "Unexpected error code");
            Assert.AreEqual(error.ErrorMessage, "The specified name is not allowed: '-content-version'.", "Unexpected error message");
        }

#if !SILVERLIGHT
        [TestMethod]
        public void CsdlWriterShouldFailWithGoodMessageWhenWritingInvalidXml()
        {
            var model = new EdmModel();
            model.SetEdmVersion(Microsoft.OData.Edm.Library.EdmConstants.EdmVersion4);
            var fredFlintstone = new EdmComplexType("Flintstones", "Fred");
            fredFlintstone.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            model.AddElement(fredFlintstone);

            // set annotation which will be serialzed (because the value is an IEdmValue) with a name that doesn't match the xml spec for NCName.
            model.SetAnnotationValue(fredFlintstone, "sap", "-contentversion", new EdmStringConstant(EdmCoreModel.Instance.GetString(false), "hello"));

            Assert.AreEqual(1, model.DirectValueAnnotations(fredFlintstone).Count(), "Wrong # of Annotations on {0}.", fredFlintstone);

            var stringWriter = new StringWriter();
            var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings() { Indent = true });

            try
            {
                IEnumerable<EdmError> errors;
                model.TryWriteCsdl(xmlWriter, out errors);
                Assert.Fail("Excepted an exception when trying to serialize a direct value annotation which does not match the xml naming spec.");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual(e.Message, "Invalid name character in '-contentversion'. The '-' character, hexadecimal value 0x2D, cannot be included in a name.", "Expected exception message did not match.");
            }

            xmlWriter.Close();
        }
#endif
    }
}
