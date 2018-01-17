//---------------------------------------------------------------------
// <copyright file="WriterRawValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Writer
{
    using System;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for writing raw values with the ODataMessageWriter.
    /// </summary>
    [TestClass, TestCase]
    public class WriterRawValueTests : ODataWriterTestCase
    {
        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        private const string TextPlainContentType = "text/plain;charset=utf-8";

        private const string ApplicationOctetStreamContentType = "application/octet-stream";

        [TestMethod, Variation(Description = "Test writing primitive values in raw format.")]
        public void RawPrimitiveValueTests()
        {
            var testCases = new PayloadWriterTestDescriptor<object> []
            {
                new PayloadWriterTestDescriptor<object>(this.Settings, (object)(double)1, "1", null, TextPlainContentType), // double
                new PayloadWriterTestDescriptor<object>(this.Settings, (object)new byte[] { 0, 1, 0, 1}, (string)null, new byte[] { 0, 1, 0, 1}, ApplicationOctetStreamContentType), // binary
                new PayloadWriterTestDescriptor<object>(this.Settings, (object)(Single)1,  "1", (byte[])null, TextPlainContentType), // single
                new PayloadWriterTestDescriptor<object>(this.Settings, (object)true, "true", (byte[])null, TextPlainContentType), // boolean
                new PayloadWriterTestDescriptor<object>(this.Settings, (object)(byte)1, "1", (byte[])null, TextPlainContentType), // byte
                new PayloadWriterTestDescriptor<object>(this.Settings, (object)DateTimeOffset.Parse("2010-10-10T10:10:10Z"), "2010-10-10T10:10:10Z", (byte[])null, TextPlainContentType), // DateTimeOffset
                new PayloadWriterTestDescriptor<object>(this.Settings, (object)DateTimeOffset.Parse("2010-10-10T10:10:10+01:00"), "2010-10-10T10:10:10+01:00", (byte[])null, TextPlainContentType), // DateTimeOffset (2)
                new PayloadWriterTestDescriptor<object>(this.Settings, (object)DateTimeOffset.Parse("2010-10-10T10:10:10-08:00"), "2010-10-10T10:10:10-08:00", (byte[])null, TextPlainContentType), // DateTimeOffset (3)
                new PayloadWriterTestDescriptor<object>(this.Settings, (object)(decimal)1, "1", (byte[])null, TextPlainContentType), // Decimal
                new PayloadWriterTestDescriptor<object>(this.Settings, (object)new Guid("11111111-2222-3333-4444-555555555555"), "11111111-2222-3333-4444-555555555555", (byte[])null, TextPlainContentType), // Guid
                new PayloadWriterTestDescriptor<object>(this.Settings, (object)(sbyte)1, "1", (byte[])null, TextPlainContentType), // SByte
                new PayloadWriterTestDescriptor<object>(this.Settings, (object)(Int16)1, "1", (byte[])null, TextPlainContentType), // Int16
                new PayloadWriterTestDescriptor<object>(this.Settings, (object)(Int32)1, "1", (byte[])null, TextPlainContentType), // Int32
                new PayloadWriterTestDescriptor<object>(this.Settings, (object)(Int64)1, "1", (byte[])null, TextPlainContentType), // Int64
                new PayloadWriterTestDescriptor<object>(this.Settings, (object)"1", "1", (byte[])null, TextPlainContentType), // string
                new PayloadWriterTestDescriptor<object>(this.Settings, (object)TimeSpan.FromMinutes(12.34), "PT12M20.4S", (byte[])null, TextPlainContentType), // Duration
                new PayloadWriterTestDescriptor<object>(this.Settings, (object)string.Empty, string.Empty, (byte[])null, TextPlainContentType), // empty 
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.WriterTestConfigurationProvider.DefaultFormatConfigurationsWithIndent,
                (testCase, testConfiguration) =>
                {
                    // fix up the accept header for binary content
                    bool binaryPayload = testCase.PayloadItems.Single() is byte[];
                    if (binaryPayload)
                    {
                        ODataMessageWriterSettings settings = testConfiguration.MessageWriterSettings.Clone();
                        settings.SetContentType("application/octet-stream", null);
                        testConfiguration = new WriterTestConfiguration(testConfiguration.Format, settings, testConfiguration.IsRequest, testConfiguration.Synchronous);
                    }

                    TestWriterUtils.WriteAndVerifyRawContent(testCase, testConfiguration, this.Assert, this.Logger);
                });
        }

        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Error tests for writing primitive values in raw format.")]
        public void RawValueErrorTests()
        {
            var primitiveValueCases = new[]
            {
                new
                {   // invalid value (non-primitive)
                    Value = (object)new ODataCollectionValue(),
                    ExpectedErrorMessage = "The value of type 'Microsoft.OData.ODataCollectionValue' could not be converted to a raw string."
                },
                new
                {   // invalid value (entry)
                    Value = (object)new ODataResource(),
                    ExpectedErrorMessage = "The value of type 'Microsoft.OData.ODataResource' could not be converted to a raw string."
                },
                new
                {   // null value
                    Value = (object)null,
                    ExpectedErrorMessage = "Cannot write the value 'null' in raw format.",
                },
            };

            var testCases = primitiveValueCases.Select(p => new PayloadWriterTestDescriptor<object>(this.Settings, p.Value, CreateErrorResultCallback(p.ExpectedErrorMessage, p.Value, this.Settings.ExpectedResultSettings)));
            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                // include (non-raw) writer test configurations to validate the expected exception for non-raw formats
                this.WriterTestConfigurationProvider.DefaultFormatConfigurationsWithIndent.Concat(
                    this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent),
                (testCase, testConfiguration) =>
                {
                    TestWriterUtils.WriteAndVerifyRawContent(testCase, testConfiguration, this.Assert, this.Logger);
                });
        }

        private static PayloadWriterTestDescriptor.WriterTestExpectedResultCallback CreateErrorResultCallback(
            string expectedErrorMessage, 
            object value,
            WriterTestExpectedResults.Settings expectedResultSettings)
        {
            return testConfiguration =>
                {
                    return new WriterTestExpectedResults(expectedResultSettings)
                    {
                        ExpectedODataExceptionMessage = testConfiguration.Format != null
                            ? (value == null ? expectedErrorMessage : "A default MIME type could not be found for the requested payload in format 'Atom'.")
                            : expectedErrorMessage
                    };
                };
        }
    }
}
