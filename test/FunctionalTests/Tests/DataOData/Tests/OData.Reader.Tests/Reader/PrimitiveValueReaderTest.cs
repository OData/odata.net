//---------------------------------------------------------------------
// <copyright file="PrimitiveValueReaderTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Reader
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Spatial;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestModels = Microsoft.Test.OData.Utils.Metadata.TestModels;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of various primitive value payloads.
    /// </summary>
    [TestClass, TestCase]
    public class PrimitiveValueReaderTests : ODataReaderTestCase
    {
        // NOTE that these test cases are different from the primitive value test cases for primitive properties since there 
        //      binary values are base64 encoded whereas they are not at the top-level
        #region The primitive value conversion test cases for top-level values
        private static PrimitiveValueConversionTestCase[] primitiveValueConversionTestCases = new PrimitiveValueConversionTestCase[]
        {
            // String
            new PrimitiveValueConversionTestCase
            {
                SourceString = "foo",
                PayloadKind = ODataPayloadKind.Value,
                ConversionValues = new []
                {
                    PayloadBuilder.PrimitiveValue((string)"foo"),
                    PayloadBuilder.PrimitiveValue(new byte[] { 102, 111, 111 })
                    // Everything else should fail
                }
            },

            // Empty string
            new PrimitiveValueConversionTestCase
            {
                SourceString = string.Empty,
                PayloadKind = ODataPayloadKind.Value,
                ConversionValues = new []
                {
                    PayloadBuilder.PrimitiveValue((string)string.Empty),
                    PayloadBuilder.PrimitiveValue(new byte[0])
                }
            },
            
            // Boolean
            new PrimitiveValueConversionTestCase
            {
                SourceString = "true",
                PayloadKind = ODataPayloadKind.Value,
                ConversionValues = new []
                {
                    PayloadBuilder.PrimitiveValue((string)"true"),
                    PayloadBuilder.PrimitiveValue((Boolean)true),
                    PayloadBuilder.PrimitiveValue(new byte[] { 116, 114, 117, 101 })
                }
            },

            // simple number
            new PrimitiveValueConversionTestCase
            {
                SourceString = "42",
                PayloadKind = ODataPayloadKind.Value,
                ConversionValues = new []
                {
                    PayloadBuilder.PrimitiveValue((string)"42"),
                    // bool - fails
                    PayloadBuilder.PrimitiveValue((Byte)42),
                    // DateTime - fails
                    PayloadBuilder.PrimitiveValue((Decimal)42),
                    PayloadBuilder.PrimitiveValue((Double)42),
                    // Guid - fails
                    PayloadBuilder.PrimitiveValue((Int16)42),
                    PayloadBuilder.PrimitiveValue((Int32)42),
                    PayloadBuilder.PrimitiveValue((Int64)42),
                    PayloadBuilder.PrimitiveValue((SByte)42),
                    PayloadBuilder.PrimitiveValue((Single)42),
                    PayloadBuilder.PrimitiveValue(new byte[] { 52, 50 }),
                }
            },

            // negative number
            new PrimitiveValueConversionTestCase
            {
                SourceString = "-42",
                PayloadKind = ODataPayloadKind.Value,
                ConversionValues = new []
                {
                    PayloadBuilder.PrimitiveValue((string)"-42"),
                    // bool - fails
                    // byte - fails
                    // DateTime - fails
                    PayloadBuilder.PrimitiveValue((Decimal)(-42)),
                    PayloadBuilder.PrimitiveValue((Double)(-42)),
                    // Guid - fails
                    PayloadBuilder.PrimitiveValue((Int16)(-42)),
                    PayloadBuilder.PrimitiveValue((Int32)(-42)),
                    PayloadBuilder.PrimitiveValue((Int64)(-42)),
                    PayloadBuilder.PrimitiveValue((SByte)(-42)),
                    PayloadBuilder.PrimitiveValue((Single)(-42)),
                    PayloadBuilder.PrimitiveValue(new byte[] { 45, 52, 50 }),
                }
            },

            // simple number convertible to bool
            new PrimitiveValueConversionTestCase
            {
                SourceString = "0",
                PayloadKind = ODataPayloadKind.Value,
                ConversionValues = new []
                {
                    PayloadBuilder.PrimitiveValue((string)"0"),
                    PayloadBuilder.PrimitiveValue((Boolean)false),
                    PayloadBuilder.PrimitiveValue((Byte)0),
                    // DateTime - fails
                    PayloadBuilder.PrimitiveValue((Decimal)0),
                    PayloadBuilder.PrimitiveValue((Double)0),
                    // Guid - fails
                    PayloadBuilder.PrimitiveValue((Int16)0),
                    PayloadBuilder.PrimitiveValue((Int32)0),
                    PayloadBuilder.PrimitiveValue((Int64)0),
                    PayloadBuilder.PrimitiveValue((SByte)0),
                    PayloadBuilder.PrimitiveValue((Single)0),
                    PayloadBuilder.PrimitiveValue(new byte[] { 48 }),
                }
            },

            // Number too big for a byte
            new PrimitiveValueConversionTestCase
            {
                SourceString = "257",
                PayloadKind = ODataPayloadKind.Value,
                ConversionValues = new []
                {
                    PayloadBuilder.PrimitiveValue((string)"257"),
                    // bool - fails
                    // byte - fails
                    // DateTime - fails
                    PayloadBuilder.PrimitiveValue((Decimal)257),
                    PayloadBuilder.PrimitiveValue((Double)257),
                    // Guid - fails
                    PayloadBuilder.PrimitiveValue((Int16)257),
                    PayloadBuilder.PrimitiveValue((Int32)257),
                    PayloadBuilder.PrimitiveValue((Int64)257),
                    // sbyte - fails,
                    PayloadBuilder.PrimitiveValue((Single)257),
                    PayloadBuilder.PrimitiveValue(new byte[] { 50, 53, 55 }),
                }
            },

            // DateTimeOffset
            new PrimitiveValueConversionTestCase
            {
                SourceString = "2010-10-10T10:10:10+07:00",
                PayloadKind = ODataPayloadKind.Value,
                ConversionValues = new []
                {
                    PayloadBuilder.PrimitiveValue((string)"2010-10-10T10:10:10+07:00"),
                    // DateTime case
                    PayloadBuilder.PrimitiveValue(new DateTimeOffset(2010, 10, 10, 10, 10, 10, new TimeSpan(7, 0, 0))),
                    PayloadBuilder.PrimitiveValue(new byte[] { 50, 48, 49, 48, 45, 49, 48, 45, 49, 48, 84, 49, 48, 58, 49, 48, 58, 49, 48, 43, 48, 55, 58, 48, 48 }),
                    // Everything else fails
                }
            },

            // Non-Integer number
            new PrimitiveValueConversionTestCase
            {
                SourceString = "-42.42",
                PayloadKind = ODataPayloadKind.Value,
                ConversionValues = new []
                {
                    PayloadBuilder.PrimitiveValue((string)"-42.42"),
                    PayloadBuilder.PrimitiveValue((Decimal)(-42.42)),
                    PayloadBuilder.PrimitiveValue((Double)(-42.42)),
                    PayloadBuilder.PrimitiveValue((Single)(-42.42)),
                    PayloadBuilder.PrimitiveValue(new byte[] { 45, 52, 50, 46, 52, 50 }),
                }
            },

            // Guid
            new PrimitiveValueConversionTestCase
            {
                SourceString = "{38CF68C2-4010-4CCC-8922-868217F03DDC}",
                PayloadKind = ODataPayloadKind.Value,
                ConversionValues = new []
                {
                    PayloadBuilder.PrimitiveValue((string)"{38CF68C2-4010-4CCC-8922-868217F03DDC}"),
                    PayloadBuilder.PrimitiveValue(new Guid("{38CF68C2-4010-4CCC-8922-868217F03DDC}")),
                    PayloadBuilder.PrimitiveValue(new byte[] { 123, 51, 56, 67, 70, 54, 56, 67, 50, 45, 52, 48, 49, 48, 45, 52, 67, 67, 67, 45, 56, 57, 50, 50, 45, 56, 54, 56, 50, 49, 55, 70, 48, 51, 68, 68, 67, 125 }),
                }
            },

            // byte[]
            new PrimitiveValueConversionTestCase
            {
                SourceString = "AQID",
                PayloadKind = ODataPayloadKind.BinaryValue,
                ConversionValues = new []
                {
                    PayloadBuilder.PrimitiveValue((string)"AQID"),
                    PayloadBuilder.PrimitiveValue(new byte[] { 65, 81, 73, 68 })
                }
            },

            // Duration
            new PrimitiveValueConversionTestCase
            {
                SourceString = "-P1DT5H10M20.04S",
                PayloadKind = ODataPayloadKind.Value,
                ConversionValues = new []
                {
                    PayloadBuilder.PrimitiveValue((string)"-P1DT5H10M20.04S"),
                    PayloadBuilder.PrimitiveValue(new TimeSpan(-1, -5, -10, -20, -40)),
                    PayloadBuilder.PrimitiveValue(new byte[] { 45, 80, 49, 68, 84, 53, 72, 49, 48, 77, 50, 48, 46, 48, 52, 83}),
                    // Everything else fails
                }
            },
        };
        #endregion The primitive value conversion test cases for top-level values

        [InjectDependency]
        public IPayloadGenerator PayloadGenerator { get; set; }

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [InjectDependency]
        public PayloadKindDetectionTestDescriptor.Settings PayloadKindDetectionSettings { get; set; }

        

        [TestMethod, TestCategory("Reader.PrimitiveValues"), Variation(Description = "Verifies correct reading of top-level primitive values when type conversion is disabled.")]
        public void PrimitiveTopLevelValueWithDisabledTypeConversionTest()
        {
            IEdmModel testModel = TestModels.BuildTestModel();

            IEnumerable<ReaderContentTypeTestDescriptor> testDescriptors = primitiveValueConversionTestCases
                .SelectMany(testCase => TestValues.PrimitiveTypes
                    .SelectMany(nonNullableTargetType => new bool[] { true, false }
                        .SelectMany(includeNullableType => new bool[] { true, false }
                            .Select(useExpectedType =>
                        {
                            PrimitiveDataType targetType = EntityModelUtils.GetPrimitiveEdmType(nonNullableTargetType);
                            if (includeNullableType)
                            {
                                targetType = targetType.Nullable();
                            }

                            ODataPayloadElement resultValue;
                            if (nonNullableTargetType == typeof(byte[]))
                            {
                                resultValue = testCase.ConversionValues.Where(cv => cv.ClrValue.GetType() == typeof(byte[])).Single().DeepCopy();
                            }
                            else
                            {
                                resultValue = testCase.ConversionValues.Where(cv => cv.ClrValue.GetType() == typeof(string)).Single().DeepCopy();
                            }

                            ODataPayloadElement payloadElement;
                            if (useExpectedType)
                            {
                                payloadElement = PayloadBuilder.PrimitiveValue(testCase.SourceString).ExpectedPrimitiveValueType(targetType);
                            }
                            else
                            {
                                payloadElement = PayloadBuilder.PrimitiveValue(testCase.SourceString);
                            }

                            return new ReaderContentTypeTestDescriptor(this.Settings)
                            {
                                PayloadElement = payloadElement,
                                ExpectedResultPayloadElement = (testConfig) => resultValue,
                                ContentType = ComputeContentType(nonNullableTargetType),
                                ExpectedFormat = ODataFormat.RawValue,
                            };
                        }))));

            // add variants that use a metadata provider
            testDescriptors = testDescriptors.Concat(testDescriptors.Select(td => new ReaderContentTypeTestDescriptor(td) { PayloadEdmModel = testModel }));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                // restricting the set of default format configurations to limiti runtime of the tests
                this.ReaderTestConfigurationProvider.DefaultFormatConfigurations.Where(tc =>tc.MessageReaderSettings.EnableMessageStreamDisposal && !tc.IsRequest),
                (testDescriptor, testConfig) =>
                {
                    testConfig = new ReaderTestConfiguration(testConfig);
                    testConfig.MessageReaderSettings.EnablePrimitiveTypeConversion = false;

                    testDescriptor.RunTest(testConfig);
                });
        }

        /// <summary>
        /// Computes the content type header for a primitive value based on its type.
        /// </summary>
        /// <param name="payloadKind">The target type for which to compute the content type header.</param>
        /// <returns>Returns 'text/plain' for all types except for binary types which use 'application/octet-stream'.</returns>
        private static string ComputeContentType(Type targetType)
        {
            if (targetType == typeof(byte[]))
            {
                return "application/octet-stream";
            }
            else
            {
                return "text/plain";
            }
        }

        private sealed class PrimitiveValueConversionTestCase
        {
            public string SourceString { get; set; }
            public ODataPayloadKind PayloadKind { get; set; }
            public IEnumerable<PrimitiveValue> ConversionValues { get; set; }
        }
    }
}
