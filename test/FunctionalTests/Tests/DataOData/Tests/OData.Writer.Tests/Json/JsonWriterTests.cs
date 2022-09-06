//---------------------------------------------------------------------
// <copyright file="JsonWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Json
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using Microsoft.OData;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the JsonWriter class
    /// </summary>
    [TestClass, TestCase]
    public class JsonWriterTests : ODataWriterTestCase
    {
        private class JsonWriterTestCase
        {
            public Action<JsonWriterTestWrapper> Write { get; set; }
            public string ExpectedOutput { get; set; }
            public override string ToString()
            {
                return "<" + this.ExpectedOutput + ">";
            }
        }

        [TestMethod, Variation(Description = "Verifies that json writer writes expected output for some basic constructs.")]
        public void BasicJsonWriterTest()
        {
            DateTimeOffset positiveOffSet = new DateTimeOffset(DateTime.Today).ToUniversalTime();
            string expectedPositiveOffsetISOFormat = XmlConvert.ToString(positiveOffSet);

            DateTimeOffset negativeOffSet = new DateTimeOffset(DateTime.Now.AddDays(-3)).ToUniversalTime();
            string expectedNegativeOffsetISOFormat = XmlConvert.ToString(negativeOffSet);

            var testCases = new JsonWriterTestCase[] {
                new JsonWriterTestCase {
                    Write = (writer) => {
                        writer.StartObjectScope();
                        writer.EndObjectScope();
                    },
                    ExpectedOutput = "{$(NL)$(Indent)$(NL)}"
                },
                new JsonWriterTestCase {
                    Write = (writer) => {
                        writer.StartArrayScope();
                        writer.EndArrayScope();
                    },
                    ExpectedOutput = "[$(NL)$(Indent)$(NL)]"
                },
                new JsonWriterTestCase {
                    Write = (writer) => {
                        writer.StartObjectScope();
                        writer.WriteName("foo");
                        writer.WriteValue("bar");
                        writer.EndObjectScope();
                    },
                    ExpectedOutput = "{$(NL)$(Indent)\"foo\":\"bar\"$(NL)}"
                },
                new JsonWriterTestCase {
                    Write = (writer) => {
                        writer.StartObjectScope();
                        writer.WriteName("foo");
                        writer.WriteValue("bar");
                        writer.WriteName("var1");
                        writer.WriteValue("value1");
                        writer.EndObjectScope();
                    },
                    ExpectedOutput = "{$(NL)$(Indent)\"foo\":\"bar\",\"var1\":\"value1\"$(NL)}"
                },
                new JsonWriterTestCase {
                    Write = (writer) => {
                        writer.StartArrayScope();
                        writer.WriteValue("bar");
                        writer.EndArrayScope();
                    },
                    ExpectedOutput = "[$(NL)$(Indent)\"bar\"$(NL)]"
                },
                new JsonWriterTestCase {
                    Write = (writer) => {
                        writer.StartArrayScope();
                        writer.WriteValue("bar");
                        writer.WriteValue((int)42);
                        writer.EndArrayScope();
                    },
                    ExpectedOutput = "[$(NL)$(Indent)\"bar\",42$(NL)]"
                },
                new JsonWriterTestCase {
                    Write = (writer) => {
                        writer.StartArrayScope();
                        writer.WriteValue("DateTimeOffset");
                        writer.WriteValue((DateTimeOffset)positiveOffSet);
                        writer.EndArrayScope();
                    },
                    ExpectedOutput = "[$(NL)$(Indent)\"DateTimeOffset\",\""+expectedPositiveOffsetISOFormat+"\"$(NL)]"
                },
                new JsonWriterTestCase {
                    Write = (writer) => {
                        writer.StartArrayScope();
                        writer.WriteValue("DateTimeOffset");
                        writer.WriteValue((DateTimeOffset)negativeOffSet);
                        writer.EndArrayScope();
                    },
                    ExpectedOutput = "[$(NL)$(Indent)\"DateTimeOffset\",\""+expectedNegativeOffsetISOFormat+"\"$(NL)]"
                },
                new JsonWriterTestCase {
                    Write = (writer) => {
                        writer.StartObjectScope();
                        writer.WriteName("foo");
                        writer.WriteValue("bar");
                        writer.EndObjectScope();
                    },
                    ExpectedOutput = "{$(NL)$(Indent)\"foo\":\"bar\"$(NL)}"
                },
                new JsonWriterTestCase {
                    Write = (writer) => {
                        writer.StartObjectScope();
                        writer.WriteName("results");
                        writer.StartArrayScope();
                        writer.WriteValue(1);
                        writer.WriteValue(2);
                        writer.EndArrayScope();
                        writer.EndObjectScope();
                    },
                    ExpectedOutput = "{$(NL)$(Indent)\"results\":[$(NL)$(Indent)$(Indent)1,2$(NL)$(Indent)]$(NL)}"
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                (testCase) =>
                {
                    var variables = new Dictionary<string, string>() { { "NL", string.Empty } };
                    StringWriter stringWriter = new StringWriter();
                    JsonWriterTestWrapper jsonWriter = new JsonWriterTestWrapper(stringWriter);
                    testCase.Write(jsonWriter);
                    jsonWriter.Flush();
                    string actualOutput = stringWriter.GetStringBuilder().ToString();
                    variables["Indent"] = string.Empty;
                    string expectedOutput = StringUtils.ResolveVariables(testCase.ExpectedOutput, variables);
                    this.Assert.AreEqual(expectedOutput, actualOutput, "Unexpected output from the writer.");
                });
        }

        #region valuesTestCases
        private static IEnumerable<JsonWriterTestCase> valuesTestCases =
            JsonValueUtilsTests.PrimitiveValueTestCases.Select(tc => new JsonWriterTestCase()
            {
                Write = writer =>
                {
                    if (tc.Value is DateTimeOffset)
                    {
                        writer.WriteValue((DateTimeOffset)tc.Value);
                    }
                    else
                    {
                        writer.WriteObjectValue(tc.Value);
                    }
                },
                ExpectedOutput = tc.ExpectedTextOutput
            })
            .Concat(new JsonWriterTestCase[] {
                new JsonWriterTestCase() {
                    Write = writer => { writer.StartObjectScope(); writer.WriteName("foo"); writer.WriteValue("some"); writer.EndObjectScope(); },
                    ExpectedOutput = "{$(NL)$(Indent)\"foo\":\"some\"$(NL)}"
                },
                new JsonWriterTestCase() {
                    Write = writer => { writer.StartArrayScope(); writer.WriteValue("some"); writer.WriteValue("other"); writer.EndArrayScope(); },
                    ExpectedOutput = "[$(NL)$(Indent)\"some\",\"other\"$(NL)]"
                },
            });
        #endregion valuesTestCases

        [TestMethod, Variation(Description = "Verifies json writer writes correct output for various object property values.")]
        public void ObjectPropertyValueJsonWriterTest()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                valuesTestCases,
                (testCase) =>
                {
                    StringWriter stringWriter = new StringWriter();
                    JsonWriterTestWrapper jsonWriter = new JsonWriterTestWrapper(stringWriter);
                    jsonWriter.StartObjectScope();
                    jsonWriter.WriteName("propname");
                    testCase.Write(jsonWriter);
                    jsonWriter.EndObjectScope();
                    jsonWriter.Flush();
                    string actualOutput = stringWriter.GetStringBuilder().ToString();
                    var variables = new Dictionary<string, string>()
                    {
                        { "InternalNewLine", string.Empty },
                        { "Indent", string.Empty },
                        { "NL", string.Empty },
                    };
                    string expectedOutput = "{$(InternalNewLine)$(Indent)\"propname\":" + testCase.ExpectedOutput + "$(InternalNewLine)}";
                    expectedOutput = StringUtils.ResolveVariables(expectedOutput, variables);
                    this.Assert.AreEqual(expectedOutput, actualOutput, "Unexpected output from the writer.");
                });
        }

        [TestMethod, Variation(Description = "Verifies json writer writes correct output for various array element values.")]
        public void ArrayElementJsonWriterTest()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                valuesTestCases,
                (testCase) =>
                {
                    StringWriter stringWriter = new StringWriter();
                    JsonWriterTestWrapper jsonWriter = new JsonWriterTestWrapper(stringWriter);
                    jsonWriter.StartArrayScope();
                    testCase.Write(jsonWriter);
                    jsonWriter.EndArrayScope();
                    jsonWriter.Flush();
                    string actualOutput = stringWriter.GetStringBuilder().ToString();
                    var variables = new Dictionary<string, string>()
                    {
                        { "InternalNewLine", string.Empty },
                        { "Indent", string.Empty },
                        { "NL", string.Empty },
                    };
                    string expectedOutput = "[$(InternalNewLine)$(Indent)" + testCase.ExpectedOutput + "$(InternalNewLine)]";
                    expectedOutput = StringUtils.ResolveVariables(expectedOutput, variables);
                    this.Assert.AreEqual(expectedOutput, actualOutput, "Unexpected output from the writer.");
                });
        }

        /// <summary>
        /// Wrapper class which provides access to the internal JsonWriter class in the product.
        /// </summary>
        private class JsonWriterTestWrapper
        {
            Type JsonWriterType = typeof(Microsoft.OData.ODataAnnotatable).Assembly.GetType("Microsoft.OData.Json.JsonWriter");
            private object jsonWriter;

            public JsonWriterTestWrapper(TextWriter writer)
            {
                this.jsonWriter = ReflectionUtils.CreateInstance(JsonWriterType, writer, false);
            }

            public void WriteObjectValue(object value)
            {
                if (value == null) { this.WriteValue(null); }
                else { ReflectionUtils.InvokeMethod(this, "WriteValue", value); }
            }

            public void StartObjectScope() { ReflectionUtils.InvokeMethod(this.jsonWriter, "StartObjectScope"); }
            public void EndObjectScope() { ReflectionUtils.InvokeMethod(this.jsonWriter, "EndObjectScope"); }
            public void StartArrayScope() { ReflectionUtils.InvokeMethod(this.jsonWriter, "StartArrayScope"); }
            public void EndArrayScope() { ReflectionUtils.InvokeMethod(this.jsonWriter, "EndArrayScope"); }
            public void WriteName(string name) { ReflectionUtils.InvokeMethod(this.jsonWriter, "WriteName", name); }
            public void WriteValue(bool value) { ReflectionUtils.InvokeMethod(this.jsonWriter, "WriteValue", value); }
            public void WriteValue(int value) { ReflectionUtils.InvokeMethod(this.jsonWriter, "WriteValue", value); }
            public void WriteValue(float value) { ReflectionUtils.InvokeMethod(this.jsonWriter, "WriteValue", value); }
            public void WriteValue(short value) { ReflectionUtils.InvokeMethod(this.jsonWriter, "WriteValue", value); }
            public void WriteValue(long value) { ReflectionUtils.InvokeMethod(this.jsonWriter, "WriteValue", value); }
            public void WriteValue(double value) { ReflectionUtils.InvokeMethod(this.jsonWriter, "WriteValue", value); }
            public void WriteValue(Guid value) { ReflectionUtils.InvokeMethod(this.jsonWriter, "WriteValue", value); }
            public void WriteValue(decimal value) { ReflectionUtils.InvokeMethod(this.jsonWriter, "WriteValue", value); }
            public void WriteValue(DateTimeOffset value) { ReflectionUtils.InvokeMethod(this.jsonWriter, "WriteValue", value); }
            public void WriteValue(TimeSpan value) { ReflectionUtils.InvokeMethod(this.jsonWriter, "WriteValue", value); }
            public void WriteValue(byte value) { ReflectionUtils.InvokeMethod(this.jsonWriter, "WriteValue", value); }
            public void WriteValue(sbyte value) { ReflectionUtils.InvokeMethod(this.jsonWriter, "WriteValue", value); }
            public void WriteValue(string value) { ReflectionUtils.InvokeMethod(this.jsonWriter, "WriteValue", new Type[] { typeof(string) }, value); }
            public void Flush() { ReflectionUtils.InvokeMethod(this.jsonWriter, "Flush"); }
        }
    }
}
