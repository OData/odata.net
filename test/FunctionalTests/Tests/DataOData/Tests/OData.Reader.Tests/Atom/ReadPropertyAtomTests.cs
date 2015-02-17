//---------------------------------------------------------------------
// <copyright file="ReadPropertyAtomTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Atom
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for reading top-level property, ATOM specific tests.
    /// </summary>
    [TestClass, TestCase]
    public class ReadPropertyAtomTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Tests reading of top-level properties.")]
        public void TopLevelPropertyTest()
        {
            var testDescriptors = new PayloadReaderTestDescriptor[]
            {
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveProperty(null, "foo")
                        .XmlRepresentation("<value xmlns=''>42</value>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomPropertyAndValueDeserializer_TopLevelPropertyElementWrongNamespace", string.Empty, TestAtomConstants.ODataMetadataNamespace),
                },
                 new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveProperty("propertyName", "foo")
                        .XmlRepresentation("<d:value>42</d:value>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomPropertyAndValueDeserializer_TopLevelPropertyElementWrongNamespace", TestAtomConstants.ODataNamespace, TestAtomConstants.ODataMetadataNamespace),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveProperty(null, "")
                        .XmlRepresentation("<m:value m:type='Edm.String'/>"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveProperty(null, "")
                        .XmlRepresentation("<!--s-->  <?value?><m:value m:type='Edm.String'/><?value?><!-- -->"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveProperty(null, GeographyFactory.Point(50, 70).Build())
                        .XmlRepresentation(
                        "<m:value>" +
                            "<gml:Point gml:srsName=\"http://www.opengis.net/def/crs/EPSG/0/4326\" xmlns:gml=\"http://www.opengis.net/gml\">" +
                                "<gml:pos>50 70</gml:pos>" +
                            "</gml:Point>" +
                        "</m:value>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveProperty(null, GeographyFactory.Point(50, 70).Build())
                        .XmlRepresentation(
                        "<m:value m:type='Edm.GeographyPoint'>" +
                            "<gml:Point gml:srsName=\"http://www.opengis.net/def/crs/EPSG/0/4326\" xmlns:gml=\"http://www.opengis.net/gml\">" +
                                "<gml:pos>50 70</gml:pos>" +
                            "</gml:Point>" +
                        "</m:value>"),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                }
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct handling of different namespaces for top-level property.")]
        public void TopLevelPropertyNamespaceTest()
        {
            var testCases = new[]
            {
                new
                {
                    // Support custom data namespace for compatibility with WCF DS client.
                    NamespaceUri = TestAtomConstants.ODataMetadataNamespace + "/custom",
                    ExpectedException = new System.Func<TestODataBehaviorKind, ODataVersion, ExpectedException>((TestODataBehaviorKind behavior,ODataVersion odataVersion) => 
                    {
                        if (behavior == TestODataBehaviorKind.WcfDataServicesServer)
                        {
                            return null;
                        }
                        else
                        {
                            return ODataExpectedExceptions.ODataException("ODataAtomPropertyAndValueDeserializer_TopLevelPropertyElementWrongNamespace", TestAtomConstants.ODataMetadataNamespace + "/custom", TestAtomConstants.ODataMetadataNamespace);
                        }
                    })
                },
                new
                {
                    NamespaceUri = TestAtomConstants.ODataMetadataNamespace,
                    ExpectedException = new System.Func<TestODataBehaviorKind, ODataVersion, ExpectedException>((TestODataBehaviorKind behavior, ODataVersion odataVersion) => 
                    {
                        return null;
                    })
                },
                new
                {
                    NamespaceUri = TestAtomConstants.ODataNamespace,
                    ExpectedException = new System.Func<TestODataBehaviorKind, ODataVersion, ExpectedException>((TestODataBehaviorKind behavior, ODataVersion odataVersion) => 
                    {
                        if (behavior == TestODataBehaviorKind.WcfDataServicesServer)
                        {
                            return null;
                        }
                        else
                        {
                            return ODataExpectedExceptions.ODataException("ODataAtomPropertyAndValueDeserializer_TopLevelPropertyElementWrongNamespace", TestAtomConstants.ODataNamespace, TestAtomConstants.ODataMetadataNamespace);
                        }
                    })
                },
                new
                {
                    NamespaceUri = "http://odata.org/customUri",
                    ExpectedException = new System.Func<TestODataBehaviorKind, ODataVersion, ExpectedException>((TestODataBehaviorKind behavior,ODataVersion odataVersion) => 
                    {
                        if (behavior == TestODataBehaviorKind.WcfDataServicesServer)
                        {
                            return null;
                        }
                        else
                        {
                            return ODataExpectedExceptions.ODataException("ODataAtomPropertyAndValueDeserializer_TopLevelPropertyElementWrongNamespace", "http://odata.org/customUri", TestAtomConstants.ODataMetadataNamespace);
                        }
                    })
                }
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                TestReaderUtils.ODataBehaviorKinds,
                new[] { ODataVersion.V4 },
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testCase, behavior, odataVersion, testConfiguration) =>
                {
                    var td = new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.PrimitiveProperty(null, string.Empty)
                            .XmlRepresentation("<p:value xmlns:p='" + testCase.NamespaceUri + "'/>"),
                        ExpectedException = testCase.ExpectedException(behavior, odataVersion)
                    };

                    testConfiguration = testConfiguration.CloneAndApplyBehavior(behavior);

                    td.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Tests reading of top-level null properties with invalid type names.")]
        public void TopLevelNullPropertyWithInvalidTypeNameTest()
        {
            EdmModel model = new EdmModel().Fixup();

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveProperty(null, null)
                        .XmlRepresentation("<m:value m:null='true' m:type='UnknownType' />"),
                    PayloadEdmModel = model,
                },
            };

            var expectedTypes = new DataType[]
                {
                    null,
                    EdmDataTypes.Int32.NotNullable(),
                    EdmDataTypes.Int32.Nullable(),
                };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                new bool[] { true, false },
                expectedTypes,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, disablePrimitiveTypeConversion, expectedType, testConfiguration) =>
                {
                    if (disablePrimitiveTypeConversion)
                    {
                        testConfiguration = new ReaderTestConfiguration(testConfiguration);
                        testConfiguration.MessageReaderSettings.DisablePrimitiveTypeConversion = true;
                    }

                    if (expectedType != null)
                    {
                        testDescriptor = new PayloadReaderTestDescriptor(testDescriptor);
                        ((PropertyInstance)testDescriptor.PayloadElement).ExpectedPropertyType(expectedType);

                        if (!disablePrimitiveTypeConversion && !expectedType.IsNullable)
                        {
                            testDescriptor.ExpectedException = ODataExpectedExceptions.ODataException("ReaderValidationUtils_NullValueForNonNullableType", "Edm.Int32");
                        }
                    }

                    testDescriptor.RunTest(testConfiguration);
                });
        }
    }
}
