//---------------------------------------------------------------------
// <copyright file="JsonValueUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Json
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.OData;
    using Microsoft.OData.Buffers;
    using Microsoft.OData.Json;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the ODataJsonConvert class
    /// </summary>
    [TestClass, TestCase]
    public class JsonValueUtilsTests : ODataWriterTestCase
    {
        internal class ValueTestCase
        {
            public object Value { get; set; }
            public string ExpectedTextOutput { get; set; }
        }

        #region PrimitiveValueTestCases
        internal static ValueTestCase[] PrimitiveValueTestCases = new ValueTestCase[] {
            new ValueTestCase() {
                Value = true,
                ExpectedTextOutput = "true"
            },
            new ValueTestCase() {
                Value = false,
                ExpectedTextOutput = "false"
            },
            new ValueTestCase() {
                Value = (byte)42,
                ExpectedTextOutput = "42"
            },
            new ValueTestCase() {
                Value = new DateTimeOffset(2010, 10, 1, 0, 0, 0, TimeSpan.FromHours(-8)),
                ExpectedTextOutput = @"""2010-10-01T00:00:00-08:00"""
            },
            new ValueTestCase() {
                Value = new DateTimeOffset(2010, 10, 1, 0, 0, 0, TimeSpan.FromHours(+1)),
                ExpectedTextOutput = @"""2010-10-01T00:00:00+01:00"""
            },
            new ValueTestCase() {
                Value = new TimeSpan(1, 2, 3, 4, 5),
                ExpectedTextOutput = @"""P1DT2H3M4.005S"""
            },
            new ValueTestCase() {
                Value = (decimal)42.42,
                ExpectedTextOutput = "42.42"
            },
            new ValueTestCase() {
                Value = (double)42.42,
                ExpectedTextOutput = "42.42"
            },
            new ValueTestCase() {
                Value = double.PositiveInfinity,
                ExpectedTextOutput = "\"INF\""
            },
            new ValueTestCase() {
                Value = double.NegativeInfinity,
                ExpectedTextOutput = "\"-INF\""
            },
            new ValueTestCase() {
                Value = double.NaN,
                ExpectedTextOutput = "\"NaN\""
            },
            new ValueTestCase() {
                Value = (float)42.42,
                ExpectedTextOutput = "42.42"
            },
            new ValueTestCase() {
                Value = float.PositiveInfinity,
                ExpectedTextOutput = "\"INF\""
            },
            new ValueTestCase() {
                Value = float.NegativeInfinity,
                ExpectedTextOutput = "\"-INF\""
            },
            new ValueTestCase() {
                Value = float.NaN,
                ExpectedTextOutput = "\"NaN\""
            },
            new ValueTestCase() {
                Value = new Guid("{38CF68C2-4010-4CCC-8922-868217F03DDC}"),
                ExpectedTextOutput = "\"38cf68c2-4010-4ccc-8922-868217f03ddc\""
            },
            new ValueTestCase() {
                Value = (int)-42,
                ExpectedTextOutput = "-42"
            },
            new ValueTestCase() {
                Value = (long)-42,
                ExpectedTextOutput = "-42"
            },
            new ValueTestCase() {
                Value = (sbyte)-42,
                ExpectedTextOutput = "-42"
            },
            new ValueTestCase() {
                Value = (short)42,
                ExpectedTextOutput = "42"
            },
            new ValueTestCase() {
                Value = "stringvalue",
                ExpectedTextOutput = "\"stringvalue\""
            },
            new ValueTestCase() {
                Value = "some\"'value",
                ExpectedTextOutput = "\"some\\\"'value\""
            },
            new ValueTestCase() {
                Value = "$&__##",
                ExpectedTextOutput = "\"$&__##\""
            },
            new ValueTestCase() {
                Value = "''''\"\"''\"",
                ExpectedTextOutput = "\"''''\\\"\\\"''\\\"\""
            },
            new ValueTestCase() {
                Value = "",
                ExpectedTextOutput = "\"\""
            },
            new ValueTestCase() {
                Value = (string)null,
                ExpectedTextOutput = "null"
            },
        };
        #endregion PrimitiveValueTestCases

        [TestMethod, Variation(Description = "Verifies that primitive values are correctly converted to JSON.")]
        public void PrimitiveTypesConversionToJsonTest()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                PrimitiveValueTestCases,
                (testCase) =>
                {
                    StringWriter writer = new StringWriter();
                    if (testCase.Value is DateTime)
                    {
                        JsonValueUtils.WriteValue(writer, (DateTime)testCase.Value, ODataJsonDateTimeFormat.ISO8601DateTime);
                    }
                    else if (testCase.Value is DateTimeOffset)
                    {
                        JsonValueUtils.WriteValue(writer, (DateTimeOffset)testCase.Value, ODataJsonDateTimeFormat.ISO8601DateTime);
                    }
                    else if (testCase.Value is double)
                    {
                        JsonValueUtils.WriteValue(writer, (double)testCase.Value);
                    }
                    else
                    {
                        JsonValueUtils.WriteObjectValue(writer, testCase.Value);
                    }

                    writer.Flush();
                    this.Assert.AreEqual(testCase.ExpectedTextOutput, writer.ToString(), "The output doesn't match the expected string.");
                });
        }

        #region StringTestCases
        /// <summary>
        /// Test cases for string values, note that the expected text output does not contain the quotes around the value which are
        /// mandatory if the value is written to the JSON as is.
        /// </summary>
        internal static ValueTestCase[] StringTestCases = new ValueTestCase[] {
            new ValueTestCase() {
                Value = (string)"\"",
                ExpectedTextOutput = "\\\""
            },
            new ValueTestCase() {
                Value = (string)"\n",
                ExpectedTextOutput = "\\n"
            },
            new ValueTestCase() {
                Value = (string)"\r",
                ExpectedTextOutput = "\\r"
            },
            new ValueTestCase() {
                Value = (string)"\t",
                ExpectedTextOutput = "\\t"
            },
            new ValueTestCase() {
                Value = (string)"\\",
                ExpectedTextOutput = "\\\\"
            },
            new ValueTestCase() {
                Value = (string)"\b",
                ExpectedTextOutput = "\\b"
            },
            new ValueTestCase() {
                Value = (string)"\f",
                ExpectedTextOutput = "\\f"
            },
            new ValueTestCase() {
                Value = (string)"\x1",
                ExpectedTextOutput = "\\u0001"
            },
            new ValueTestCase() {
                Value = (string)"\x8A",
                ExpectedTextOutput = "\\u008a"
            },
            new ValueTestCase() {
                Value = (string)"'",
                ExpectedTextOutput = "'"
            },
            new ValueTestCase() {
                Value = (string)" ",
                ExpectedTextOutput = " "
            },
            new ValueTestCase() {
                Value = (string)"a",
                ExpectedTextOutput = "a"
            },
            new ValueTestCase() {
                Value = (string)"1",
                ExpectedTextOutput = "1"
            },
        };
        #endregion StringTestCases

        [TestMethod, Variation(Description = "Verifies that strings are correctly escaped when converted to JSON.")]
        public void StringEscapeRulesTest()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                StringTestCases.Variations(1, 3),
                (testCaseCombination) =>
                {
                    StringBuilder expectedOutput = new StringBuilder();
                    StringBuilder stringToWrite = new StringBuilder();
                    expectedOutput.Append('"');
                    foreach (var testCase in testCaseCombination)
                    {
                        stringToWrite.Append(testCase.Value as string);
                        expectedOutput.Append(testCase.ExpectedTextOutput);
                    }
                    expectedOutput.Append('"');
                    StringWriter writer = new StringWriter();
                    JsonValueUtils.WriteObjectValue(writer, stringToWrite.ToString());
                    writer.Flush();

                    this.Assert.AreEqual(expectedOutput.ToString(), writer.ToString(), "The output doesn't match the expected string.");
                });
        }

        private static class JsonValueUtils
        {
            private static Type classType;
            private static Type formatEnumType;

            static JsonValueUtils()
            {
                classType = typeof(Microsoft.OData.ODataAnnotatable).Assembly.GetType("Microsoft.OData.Json.JsonValueUtils");
                formatEnumType = typeof(Microsoft.OData.ODataAnnotatable).Assembly.GetType("Microsoft.OData.ODataJsonDateTimeFormat");
            }

            public static void WriteObjectValue(TextWriter writer, object value)
            {
                if (value == null) { WriteValue(writer, null); }
                else { ReflectionUtils.InvokeMethod(typeof(JsonValueUtils), "WriteValue", writer, value); }
            }

            public static void WriteValue(TextWriter writer, bool value) { ReflectionUtils.InvokeMethod(classType, "WriteValue", writer, value); }
            public static void WriteValue(TextWriter writer, int value) { ReflectionUtils.InvokeMethod(classType, "WriteValue", writer, value); }
            public static void WriteValue(TextWriter writer, float value) { ReflectionUtils.InvokeMethod(classType, "WriteValue", writer, value); }
            public static void WriteValue(TextWriter writer, short value) { ReflectionUtils.InvokeMethod(classType, "WriteValue", writer, value); }
            public static void WriteValue(TextWriter writer, long value) { ReflectionUtils.InvokeMethod(classType, "WriteValue", writer, value); }
            public static void WriteValue(TextWriter writer, double value) { ReflectionUtils.InvokeMethod(classType, "WriteValue", writer, value); }
            public static void WriteValue(TextWriter writer, Guid value) { ReflectionUtils.InvokeMethod(classType, "WriteValue", writer, value); }
            public static void WriteValue(TextWriter writer, decimal value) { ReflectionUtils.InvokeMethod(classType, "WriteValue", writer, value); }
            public static void WriteValue(TextWriter writer, DateTime value, ODataJsonDateTimeFormat dateTimeFormat) { ReflectionUtils.InvokeMethod(classType, "WriteValue", writer, value, ReflectionUtils.GetEnumerationValue(formatEnumType, dateTimeFormat.ToString())); }
            public static void WriteValue(TextWriter writer, DateTimeOffset value, ODataJsonDateTimeFormat dateTimeFormat) { ReflectionUtils.InvokeMethod(classType, "WriteValue", writer, value, ReflectionUtils.GetEnumerationValue(formatEnumType, dateTimeFormat.ToString())); }
            public static void WriteValue(TextWriter writer, TimeSpan value) { ReflectionUtils.InvokeMethod(classType, "WriteValue", writer, value); }
            public static void WriteValue(TextWriter writer, byte value) { ReflectionUtils.InvokeMethod(classType, "WriteValue", writer, value); }
            public static void WriteValue(TextWriter writer, sbyte value) { ReflectionUtils.InvokeMethod(classType, "WriteValue", writer, value); }
            public static void WriteValue(TextWriter writer, string value) { Ref<char[]> buffer = new Ref<char[]>(); ReflectionUtils.InvokeMethod(classType, "WriteValue", new Type[] { typeof(TextWriter), typeof(string), typeof(ODataStringEscapeOption), typeof(Ref<char[]>), typeof(ICharArrayPool) }, writer, value, ODataStringEscapeOption.EscapeNonAscii, buffer, null); }
        }
    }
}
