//---------------------------------------------------------------------
// <copyright file="ComplexValueReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Reader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Contracts.Json;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using InjectDependency = Microsoft.Test.Taupo.Common.InjectDependencyAttribute;
    using MetadataUtils = Microsoft.Test.OData.Utils.Metadata.MetadataUtils;

    /// <summary>
    /// Tests reading of various complex value payloads.
    /// </summary>
    [TestClass, TestCase]
    public class ComplexValueReaderTests : ODataReaderTestCase
    {
        [InjectDependency]
        public IPayloadGenerator PayloadGenerator { get; set; }

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.ComplexValues"), Variation(Description = "Verifies duplicate property name checking on complex values.")]
        public void DuplicatePropertyNamesTest()
        {
            PropertyInstance primitiveProperty = PayloadBuilder.PrimitiveProperty("DuplicateProperty", 42);
            PropertyInstance complexProperty = PayloadBuilder.Property("DuplicateProperty",
                PayloadBuilder.ComplexValue("TestModel.DuplicateComplexType").PrimitiveProperty("Name", "foo"));
            PropertyInstance collectionProperty = PayloadBuilder.Property("DuplicateProperty",
                PayloadBuilder.PrimitiveMultiValue(EntityModelUtils.GetCollectionTypeName("Edm.String")).WithTypeAnnotation(EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false))));

            PropertyInstance[] allProperties = new[] { primitiveProperty, complexProperty, collectionProperty };
            PropertyInstance[] propertiesWithPossibleDuplication = new[] { primitiveProperty, complexProperty };
            PropertyInstance[] propertiesWithNoDuplication = new[] { collectionProperty };

            IEnumerable<DuplicatePropertySet> duplicatePropertySets;

            // Those which may allow duplication
            duplicatePropertySets = propertiesWithPossibleDuplication
                .Variations(2).Select(properties => new DuplicatePropertySet { Properties = properties, DuplicationPotentiallyAllowed = true });

            // Then for each in those which don't allow duplication try it against all the others
            duplicatePropertySets = duplicatePropertySets.Concat(propertiesWithNoDuplication.SelectMany(
                propertyWithNoDuplication => allProperties.SelectMany(otherProperty =>
                    new[]
                    {
                        new DuplicatePropertySet { Properties = new [] { propertyWithNoDuplication, otherProperty }, DuplicationPotentiallyAllowed = false },
                        new DuplicatePropertySet { Properties = new [] { otherProperty, propertyWithNoDuplication }, DuplicationPotentiallyAllowed = false },
                    })));

            this.CombinatorialEngineProvider.RunCombinations(
                duplicatePropertySets,
                new bool[] { false, true },
                new bool[] { true, false },
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations,
                (duplicatePropertySet, allowDuplicateProperties, useMetadata, testConfiguration) =>
                {
                    EdmModel model = new EdmModel();
                    var complexType = model.ComplexType("DuplicateComplexType");
                    complexType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
                    model.Fixup();

                    PropertyInstance firstProperty = duplicatePropertySet.Properties.ElementAt(0);
                    PropertyInstance secondProperty = duplicatePropertySet.Properties.ElementAt(1);

                    // Non-metadata reading is not possible in JSON
                    if (!useMetadata && (testConfiguration.Format == ODataFormat.Json))
                    {
                        return;
                    }

                    // If we will have metadata then we can only allow combinations of the same kind
                    if (useMetadata)
                    {
                        if (firstProperty.ElementType != secondProperty.ElementType)
                        {
                            return;
                        }
                    }

                    // Copy the test config
                    testConfiguration = new ReaderTestConfiguration(testConfiguration);
                    if (allowDuplicateProperties)
                    {
                        testConfiguration.MessageReaderSettings.Validations &= ~(ValidationKinds.ThrowOnDuplicatePropertyNames | ValidationKinds.ThrowIfTypeConflictsWithMetadata);
                        testConfiguration.MessageReaderSettings.ClientCustomTypeResolver = null;

                    }

                    // Create a descriptor with the first property
                    PayloadReaderTestDescriptor testDescriptor = new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = firstProperty,
                        PayloadEdmModel = useMetadata ? model : null
                    };

                    // Now generate entity around it
                    testDescriptor = testDescriptor.InComplexValue(5, 5);

                    // Now add the second property to it
                    ((ComplexInstance)testDescriptor.PayloadElement).Add(secondProperty);

                    // We expect failure only if we don't allow duplicates or if the property kind doesn't allow duplicates ever
                    if ((!duplicatePropertySet.DuplicationPotentiallyAllowed || !allowDuplicateProperties))
                    {
                        testDescriptor.ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "DuplicateProperty");
                    }

                    IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
                    {
                        testDescriptor.InProperty("TopLevelProperty"),
                        testDescriptor.InProperty("ComplexProperty").InEntity(2, 2),
                        testDescriptor.InCollection(5, 5).InProperty("TopLevelCollection"),
                    };

                    this.CombinatorialEngineProvider.RunCombinations(
                        testDescriptors,
                        td =>
                        {
                            var property = td.PayloadElement as PropertyInstance;
                            td.RunTest(testConfiguration);
                        });
                });
        }

        [TestMethod, TestCategory("Reader.ComplexValues"), Variation(Description = "Verifies correct behavior when null values are to be ignored on otherwise non-nullable properties inside of complex values.")]
        public void ComplexValueIgnorePropertyNullValuesTest()
        {
            var versions = new Version[] {
                    null,
                    new Version(4, 0),
                };

            EdmModel edmModel = new EdmModel();
            IEdmComplexType countryRegionType = edmModel.ComplexType("CountryRegion")
                .Property("Name", EdmPrimitiveTypeKind.String)
                .Property("CountryRegionCode", EdmPrimitiveTypeKind.String);
            IEdmComplexType countryRegionNullType = edmModel.ComplexType("CountryRegionNull")
                .Property("Name", EdmPrimitiveTypeKind.String)
                .Property("CountryRegionCode", EdmPrimitiveTypeKind.String);
            IEdmComplexType addressType = edmModel.ComplexType("Address")
                .Property("Street", EdmPrimitiveTypeKind.String)
                .Property("StreetNull", EdmCoreModel.Instance.GetString(true) as EdmTypeReference)
                .Property("Numbers", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(false)) as EdmTypeReference)
                .Property("CountryRegion", new EdmComplexTypeReference(countryRegionType, false))
                .Property("CountryRegionNull", new EdmComplexTypeReference(countryRegionNullType, true));
            edmModel.EntityType("Customer")
                .KeyProperty("ID", EdmCoreModel.Instance.GetInt32(false) as EdmTypeReference)
                .Property("Address", new EdmComplexTypeReference(addressType, false));
            edmModel.Fixup();

            this.CombinatorialEngineProvider.RunCombinations(
                new ODataNullValueBehaviorKind[] { ODataNullValueBehaviorKind.Default, ODataNullValueBehaviorKind.DisableValidation, ODataNullValueBehaviorKind.IgnoreValue },
                versions,
                versions,
                TestReaderUtils.ODataBehaviorKinds,
                (nullPropertyValueReaderBehavior, dataServiceVersion, edmVersion, behaviorKind) =>
                {
                    edmModel.SetEdmVersion(edmVersion);

                    // Now we set the 'IgnoreNullValues' annotation on all properties
                    IEdmComplexType edmAddressType = (IEdmComplexType)edmModel.FindType("TestModel.Address");
                    foreach (IEdmStructuralProperty edmProperty in edmAddressType.StructuralProperties())
                    {
                        edmModel.SetNullValueReaderBehavior(edmProperty, nullPropertyValueReaderBehavior);
                    }

                    EntityInstance customerPayload = PayloadBuilder.Entity("TestModel.Customer")
                        .PrimitiveProperty("ID", 1)
                        .Property("Address", PayloadBuilder.ComplexValue("TestModel.Address")
                            .PrimitiveProperty("Street", "One Microsoft Way")
                            .PrimitiveProperty("StreetNull", "One Microsoft Way")
                            .Property("Numbers", PayloadBuilder.PrimitiveMultiValue("Collection(Edm.Int32)").Item(1).Item(2))
                            .Property("CountryRegion", PayloadBuilder.ComplexValue("TestModel.CountryRegion")
                                .PrimitiveProperty("Name", "Austria")
                                .PrimitiveProperty("CountryRegionCode", "AUT"))
                            .Property("CountryRegionNull", PayloadBuilder.ComplexValue("TestModel.CountryRegionNull")
                                .PrimitiveProperty("Name", "Austria")
                                .PrimitiveProperty("CountryRegionCode", "AUT")));

                    var testCases = new[]
                    {
                        // Complex types that are not nullable should not allow null values.
                        // Null primitive property in the payload and non-nullable property in the model
                        new IgnoreNullValueTestCase
                        {
                            PropertyName = "Street",
                            ExpectedResponseException = ODataExpectedExceptions.ODataException("ReaderValidationUtils_NullNamedValueForNonNullableType", "Street", "Edm.String"),
                        },
                         // Null complex property in the payload and non-nullable property in the model
                        new IgnoreNullValueTestCase
                        {
                            PropertyName = "CountryRegion",
                            ExpectedResponseException = ODataExpectedExceptions.ODataException("ReaderValidationUtils_NullNamedValueForNonNullableType", "CountryRegion", "TestModel.CountryRegion"),
                        },
                        // Null collection property in the payload and non-nullable property in the model
                        new IgnoreNullValueTestCase
                        {
                            PropertyName = "Numbers",
                            ExpectedResponseException = ODataExpectedExceptions.ODataException("ReaderValidationUtils_NullNamedValueForNonNullableType", "Numbers", "Collection(Edm.Int32)"),
                        },
                        // Complex types that are nullable should allow null values.
                        // Null primitive property in the payload and nullable property in the model
                        new IgnoreNullValueTestCase
                        {
                            PropertyName = "StreetNull",
                        },
                        // Null complex property in the payload and nullable property in the model
                        new IgnoreNullValueTestCase
                        {
                            PropertyName = "CountryRegionNull",
                        },
                    };

                    Func<IgnoreNullValueTestCase, ReaderTestConfiguration, PayloadReaderTestDescriptor> createTestDescriptor =
                        (testCase, testConfig) =>
                        {
                            EntityInstance payloadValue = customerPayload.DeepCopy();
                            ComplexInstance payloadAddressValue = ((ComplexProperty)payloadValue.GetProperty("Address")).Value;
                            SetToNull(payloadAddressValue, testCase.PropertyName);

                            ComplexInstance resultValue = payloadValue;
                            if (testConfig.IsRequest && nullPropertyValueReaderBehavior == ODataNullValueBehaviorKind.IgnoreValue)
                            {
                                resultValue = customerPayload.DeepCopy();
                                ComplexInstance resultAddressValue = ((ComplexProperty)resultValue.GetProperty("Address")).Value;
                                var property = resultAddressValue.GetProperty(testCase.PropertyName);
                                if (!(property is ComplexProperty))
                                {
                                    resultAddressValue.Remove(property);
                                }
                                else
                                {
                                    SetToNull(resultAddressValue, testCase.PropertyName);
                                }
                            }

                            return new PayloadReaderTestDescriptor(this.Settings)
                            {
                                PayloadElement = payloadValue,
                                PayloadEdmModel = edmModel,
                                ExpectedResultPayloadElement =
                                    tc =>
                                    {
                                        if (tc.Format == ODataFormat.Json)
                                        {
                                            // under the client knob ODL will compute edit links, ids, etc
                                            // so we need to update the expected payload
                                            if (tc.RunBehaviorKind == TestODataBehaviorKind.WcfDataServicesClient)
                                            {
                                                var entity = resultValue as EntityInstance;
                                                if (entity != null)
                                                {
                                                    if (!tc.IsRequest)
                                                    {
                                                        entity.Id = "http://odata.org/test/Customer(1)";
                                                        entity.EditLink = "http://odata.org/test/Customer(1)";
                                                        entity.WithSelfLink("http://odata.org/test/Customer(1)");
                                                    }
                                                }
                                            }

                                            var tempDescriptor = new PayloadReaderTestDescriptor(this.Settings)
                                            {
                                                PayloadElement = resultValue,
                                                PayloadEdmModel = edmModel,
                                            };

                                            JsonLightPayloadElementFixup.Fixup(tempDescriptor);
                                            return tempDescriptor.PayloadElement;
                                        }

                                        return resultValue;
                                    },
                                ExpectedException = (testConfig.IsRequest && nullPropertyValueReaderBehavior != ODataNullValueBehaviorKind.Default) ? null : testCase.ExpectedResponseException
                            };
                        };

                    this.CombinatorialEngineProvider.RunCombinations(
                        testCases,
                        this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations,
                        (testCase, testConfiguration) =>
                        {
                            testConfiguration = testConfiguration.CloneAndApplyBehavior(behaviorKind);
                            testConfiguration.MessageReaderSettings.BaseUri = null;

                            PayloadReaderTestDescriptor testDescriptor = createTestDescriptor(testCase, testConfiguration);
                            testDescriptor.RunTest(testConfiguration);
                        });
                });
        }

        private static void SetToNull(ComplexInstance complexInstance, string propertyName)
        {
            PropertyInstance nullProperty = PayloadBuilder.PrimitiveProperty(propertyName, null);
            complexInstance.Properties = complexInstance.Properties.ToList().Select(p => p.Name == propertyName ? nullProperty : p);
        }

        private sealed class IgnoreNullValueTestCase
        {
            public string PropertyName { get; set; }
            public ExpectedException ExpectedResponseException { get; set; }
        }

        private sealed class DuplicatePropertySet
        {
            public IEnumerable<PropertyInstance> Properties { get; set; }
            public bool DuplicationPotentiallyAllowed { get; set; }
        }
    }
}
