//---------------------------------------------------------------------
// <copyright file="JsonReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if !(SILVERLIGHT || WINDOWS_PHONE)
    // These tests use Reflection to create & test internal product types,
    // (in the test wrapper of JsonReader) which cannot be done in Silverlight/Phone. 
    // Running these unit tests on desktop only is sufficient.

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Json
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for JsonReader class.
    /// </summary>
    [TestClass, TestCase]
    public class JsonReaderTests : ODataReaderTestCase
    {
        [InjectDependency]
        public IJsonValueComparer JsonValueComparer { get; set; }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Tests correct behavior of the JsonReader for string primitive values.")]
        public void StringPrimitiveValueTest()
        {
            IEnumerable<JsonReaderTestCaseDescriptor> testCases = new JsonReaderTestCaseDescriptor[]
            {
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\"some\"",
                    ExpectedJson = new JsonPrimitiveValue("some")
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\"\"",
                    ExpectedJson = new JsonPrimitiveValue("")
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\'some\'",
                    ExpectedJson = new JsonPrimitiveValue("some")
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\'\'",
                    ExpectedJson = new JsonPrimitiveValue("")
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\"so'me\"",
                    ExpectedJson = new JsonPrimitiveValue("so'me")
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\'so\\\"me\'",
                    ExpectedJson = new JsonPrimitiveValue("so\"me")
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\"\\'\\\"\"",
                    ExpectedJson = new JsonPrimitiveValue("\'\"")
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\'\\'\\\"\'",
                    ExpectedJson = new JsonPrimitiveValue("\'\"")
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\"some\\b\\f\\n\\r\\t\\\\\\/\"",
                    ExpectedJson = new JsonPrimitiveValue("some\b\f\n\r\t\\/")
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\"some\\baa\\faa\\naa\\raa\\taa\\\\aa\\/aa\"",
                    ExpectedJson = new JsonPrimitiveValue("some\baa\faa\naa\raa\taa\\aa/aa")
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\"aa\\u0020aa\"",
                    ExpectedJson = new JsonPrimitiveValue("aa aa")
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\"aa\\u006d\\u006Daa\"",
                    ExpectedJson = new JsonPrimitiveValue("aammaa")
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\"" + new string('a', 5 * 1024) + "\"",
                    ExpectedJson = new JsonPrimitiveValue(new string('a', 5 * 1024))
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\"some",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_UnexpectedEndOfString"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\'some",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_UnexpectedEndOfString"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\"some\\\"",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_UnexpectedEndOfString"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\'some\\\'",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_UnexpectedEndOfString"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\"aa\\",
                    DisablePayloadCombinations = true,
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_UnrecognizedEscapeSequence", "\\"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\"aa\\g\"",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_UnrecognizedEscapeSequence", "\\g"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\"aa\\u\"",
                    DisablePayloadCombinations = true,
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_UnrecognizedEscapeSequence", "\\uXXXX"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\"aa\\u123",
                    DisablePayloadCombinations = true,
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_UnrecognizedEscapeSequence", "\\uXXXX"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\"aa\\uzzzz\"",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_UnrecognizedEscapeSequence", "\\uzzzz"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\"aa\\u-123",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_UnrecognizedEscapeSequence", "\\u-123"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\"aa\\u0020",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_UnexpectedEndOfString"),
                },
            };

            testCases = testCases.PayloadCases(JsonReaderPayloads.ValuePayloads);
            testCases = testCases.PayloadCases(JsonReaderPayloads.WhitespacePaylods);

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                JsonReaderUtils.TestConfigurations,
                (testCase, testConfiguration) =>
                {
                    JsonReaderUtils.ReadAndVerifyJson(testCase, testConfiguration, this.JsonValueComparer, this.Assert, this.ExceptionVerifier);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Tests correct behavior of the JsonReader for number primitive values.")]
        public void NumberPrimitiveTests()
        {
            IEnumerable<JsonReaderTestCaseDescriptor> testCases = new JsonReaderTestCaseDescriptor[]
            {
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "42",
                    ExpectedJson = new JsonPrimitiveValue((int)42)
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "0",
                    ExpectedJson = new JsonPrimitiveValue((int)0)
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "-42",
                    ExpectedJson = new JsonPrimitiveValue((int)-42)
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = Int32.MaxValue.ToString(),
                    ExpectedJson = new JsonPrimitiveValue(Int32.MaxValue)
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = Int32.MinValue.ToString(),
                    ExpectedJson = new JsonPrimitiveValue(Int32.MinValue)
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "42.42",
                    ExpectedJson = new JsonPrimitiveValue((double)42.42)
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = ".42",
                    ExpectedJson = new JsonPrimitiveValue((double).42)
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "-42.42",
                    ExpectedJson = new JsonPrimitiveValue((double)-42.42)
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "42.3e-2",
                    ExpectedJson = new JsonPrimitiveValue((double)42.3e-2)
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "42.3E-2",
                    ExpectedJson = new JsonPrimitiveValue((double)42.3e-2)
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "42.3E+2",
                    ExpectedJson = new JsonPrimitiveValue((double)42.3e+2)
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "0005.2",
                    ExpectedJson = new JsonPrimitiveValue((double)5.2)
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "42-3",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_InvalidNumberFormat", "42-3"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "42..3",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_InvalidNumberFormat", "42..3"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "4e3e3",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_InvalidNumberFormat", "4e3e3"),
                },
                // Number is only up to the first non-number character
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "42a",
                    DisablePayloadCombinations = true,  // We need this since the error is different when inside an array or so.
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_MultipleTopLevelValues"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "42   4",
                    DisablePayloadCombinations = true,  // We need this since the error is different when inside an array or so.
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_MultipleTopLevelValues"),
                },
            };

            testCases = testCases.PayloadCases(JsonReaderPayloads.ValuePayloads);
            testCases = testCases.PayloadCases(JsonReaderPayloads.WhitespacePaylods);

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                JsonReaderUtils.TestConfigurations,
                (testCase, testConfiguration) =>
                {
                    JsonReaderUtils.ReadAndVerifyJson(testCase, testConfiguration, this.JsonValueComparer, this.Assert, this.ExceptionVerifier);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Tests correct behavior of the JsonReader for arrays.")]
        public void ArrayTests()
        {
            IEnumerable<JsonReaderTestCaseDescriptor> testCases = new JsonReaderTestCaseDescriptor[]
            {
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "[^]",
                    ExpectedJson = new JsonArray()
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "[^42^]",
                    ExpectedJson = new JsonArray() { new JsonPrimitiveValue(42) }
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "[^42^,^43^]",
                    ExpectedJson = new JsonArray() { new JsonPrimitiveValue(42), new JsonPrimitiveValue(43) }
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "[,",
                    DisablePayloadCombinations = true,
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_EndOfInputWithOpenScope"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "[^,^]",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_UnexpectedComma", "Array"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "[^42^,^]",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_UnexpectedComma", "Array"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "[^,^42^]",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_UnexpectedComma", "Array"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "[^,^,^42^]",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_UnexpectedComma", "Array"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "[^42^^43^]",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_MissingComma", "Array"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "[^42[]^]",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_MissingComma", "Array"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "[^{}42^]",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_MissingComma", "Array"),
                },
            };

            testCases = testCases.PayloadCases(JsonReaderPayloads.ValuePayloads);
            testCases = testCases.PayloadCases(JsonReaderPayloads.WhitespacePaylods);

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                JsonReaderUtils.TestConfigurations,
                (testCase, testConfiguration) =>
                {
                    JsonReaderUtils.ReadAndVerifyJson(testCase, testConfiguration, this.JsonValueComparer, this.Assert, this.ExceptionVerifier);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Tests correct behavior of the JsonReader for objects.")]
        public void ObjectTests()
        {
            IEnumerable<JsonReaderTestCaseDescriptor> testCases = new JsonReaderTestCaseDescriptor[]
            {
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "{^}",
                    ExpectedJson = new JsonObject()
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "{^\"foo\"^:^42^}",
                    ExpectedJson = new JsonObject() { new JsonProperty("foo", new JsonPrimitiveValue(42)) }
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "{^\"foo\"^:^42^,^\"bar\"^:^\"some\"^}",
                    ExpectedJson = new JsonObject() { new JsonProperty("foo", new JsonPrimitiveValue(42)), new JsonProperty("bar", new JsonPrimitiveValue("some")) }
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "{,",
                    DisablePayloadCombinations = true,
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_EndOfInputWithOpenScope"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "{^,^}",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_UnexpectedComma", "Object"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "{^\"foo\"^:^42^,^}",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_UnexpectedComma", "Object"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "{^,^\"foo\"^:^42^}",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_UnexpectedComma", "Object"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "{^,^,^\"foo\"^:^42^}",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_UnexpectedComma", "Object"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "{^\"foo\"^:^42^^\"bar\"^:^43^}",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_MissingComma", "Object"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "{^\"foo\"^:^42^\"bar\"^:^43^}",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_MissingComma", "Object"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "{^\"foo\"^:^{}^\"bar\"^:^43^]",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_MissingComma", "Object"),
                },
            };

            testCases = testCases.PayloadCases(JsonReaderPayloads.ValuePayloads);
            testCases = testCases.PayloadCases(JsonReaderPayloads.WhitespacePaylods);

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                JsonReaderUtils.TestConfigurations,
                (testCase, testConfiguration) =>
                {
                    JsonReaderUtils.ReadAndVerifyJson(testCase, testConfiguration, this.JsonValueComparer, this.Assert, this.ExceptionVerifier);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Tests correct behavior of the JsonReader for properties.")]
        public void PropertyTests()
        {
            IEnumerable<JsonReaderTestCaseDescriptor> testCases = new JsonReaderTestCaseDescriptor[]
            {
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\"foo\"^:^42",
                    ExpectedJson = new JsonProperty("foo", new JsonPrimitiveValue(42))
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\'foo\'^:^42",
                    ExpectedJson = new JsonProperty("foo", new JsonPrimitiveValue(42))
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "foo^:^42",
                    ExpectedJson = new JsonProperty("foo", new JsonPrimitiveValue(42))
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "_$32a$_^:^42",
                    ExpectedJson = new JsonProperty("_$32a$_", new JsonPrimitiveValue(42))
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "$a32^:^42",
                    ExpectedJson = new JsonProperty("$a32", new JsonPrimitiveValue(42))
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "32^:^42",
                    ExpectedJson = new JsonProperty("32", new JsonPrimitiveValue(42))
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "_^:^42",
                    ExpectedJson = new JsonProperty("_", new JsonPrimitiveValue(42))
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "$^:^42",
                    ExpectedJson = new JsonProperty("$", new JsonPrimitiveValue(42))
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\" \"^:^42",
                    ExpectedJson = new JsonProperty(" ", new JsonPrimitiveValue(42))
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\'\t\'^:^42",
                    ExpectedJson = new JsonProperty("\t", new JsonPrimitiveValue(42))
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\"\r\n\t   \"^:^42",
                    ExpectedJson = new JsonProperty("\r\n\t   ", new JsonPrimitiveValue(42))
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\'\\r\\n\\t   \'^:^42",
                    ExpectedJson = new JsonProperty("\r\n\t   ", new JsonPrimitiveValue(42))
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = new string('a', 5 * 1024) + "^:^42",
                    ExpectedJson = new JsonProperty(new string('a', 5 * 1024), new JsonPrimitiveValue(42))
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\"\"^:^42",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_InvalidPropertyNameOrUnexpectedComma", string.Empty),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\'\'^:^42",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_InvalidPropertyNameOrUnexpectedComma", string.Empty),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "^:^42",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_InvalidPropertyNameOrUnexpectedComma", string.Empty),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\"foo\"^42",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_MissingColon", "foo"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "{\"foo\"",
                    DisablePayloadCombinations = true,
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_MissingColon", "foo"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "{\"foo\"  ",
                    DisablePayloadCombinations = true,
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_MissingColon", "foo"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\"foo\"^,^42",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_MissingColon", "foo"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "\"foo\"^:^,^42",
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_UnexpectedComma", "Property"),
                },
                new JsonReaderTestCaseDescriptor
                {
                    JsonText = "{\"foo\":",
                    DisablePayloadCombinations = true,
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReader_EndOfInputWithOpenScope"),
                },
            };

            testCases = testCases.PayloadCases(JsonReaderPayloads.PropertyPayloads);
            testCases = testCases.PayloadCases(JsonReaderPayloads.WhitespacePaylods);

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                JsonReaderUtils.TestConfigurations,
                (testCase, testConfiguration) =>
                {
                    JsonReaderUtils.ReadAndVerifyJson(testCase, testConfiguration, this.JsonValueComparer, this.Assert, this.ExceptionVerifier);
                });
        }
    }
}
#endif
