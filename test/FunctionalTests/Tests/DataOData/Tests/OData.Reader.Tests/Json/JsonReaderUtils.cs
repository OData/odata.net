//---------------------------------------------------------------------
// <copyright file="JsonReaderUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Json
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Json;
    using ODataConstants = Microsoft.Test.Taupo.Astoria.Contracts.OData.ODataConstants;

    #endregion Namespaces

    /// <summary>
    /// Helper methods for testing JsonReader
    /// </summary>
    public static class JsonReaderUtils
    {
        /// <summary>
        /// All test configuration.
        /// </summary>
        private static readonly IEnumerable<JsonReaderTestConfiguration> allTestConfigurations;

        /// <summary>
        /// Class constructor.
        /// </summary>
        static JsonReaderUtils()
        {
            var readSizes = new int[][]
            {
                new int[] { Int32.MaxValue },
                new int[] { 1 },
                new int[] { 1, 7, 101 }
            };

            Func<TextReader, AssertionHandler, JsonReader>[] creatorFuncs = new Func<TextReader, AssertionHandler, JsonReader>[]
            {
                (textReader, assert) => new JsonReader(textReader, assert, isIeee754Compatible: true),
                (textReader, assert) => new BufferingJsonReader(textReader, ODataConstants.DefaultMaxRecursionDepth, assert, isIeee754Compatible: true),
                (textReader, assert) => 
                {
                    BufferingJsonReader reader = new BufferingJsonReader(textReader, ODataConstants.DefaultMaxRecursionDepth, assert, isIeee754Compatible: true);
                    reader.StartBuffering();
                    return reader;
                },
            };

            allTestConfigurations = readSizes.SelectMany(rs =>
                creatorFuncs.Select(f => new JsonReaderTestConfiguration { ReadSizes = rs, JsonReaderCreatorFunc = f }));
        }

        /// <summary>
        /// All interesting test configurations.
        /// </summary>
        public static IEnumerable<JsonReaderTestConfiguration> TestConfigurations
        {
            get { return allTestConfigurations; }
        }

        /// <summary>
        /// Runs a single JsonReaderTestCaseDescriptor test.
        /// </summary>
        /// <param name="testCase">The test case descriptor to run.</param>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <param name="jsonValueComparer">The comparer to use to compare JSON OMs.</param>
        /// <param name="assert">The assertion handler.</param>
        public static void ReadAndVerifyJson(
            JsonReaderTestCaseDescriptor testCase, 
            JsonReaderTestConfiguration testConfiguration, 
            IJsonValueComparer jsonValueComparer, 
            AssertionHandler assert,
            IExceptionVerifier exceptionVerifier)
        {
            TextReader testReader = new TestTextReader(new StringReader(testCase.JsonText))
            {
                FailOnPeek = true,
                FailOnSingleCharacterRead = true,
                ReadSizesEnumerator = testConfiguration.ReadSizes.EndLessLoop()
            };

            JsonValue actualJsonResult = null;
            assert.ExpectedException(() =>
                {
                    JsonReader jsonReader = testConfiguration.JsonReaderCreatorFunc(testReader, assert);
                    actualJsonResult = ReadJson(jsonReader, assert);
                },
                testCase.ExpectedException,
                exceptionVerifier);

            if (testCase.ExpectedException == null)
            {
                if (testCase.FragmentExtractor != null)
                {
                    actualJsonResult = testCase.FragmentExtractor(actualJsonResult);
                }

                jsonValueComparer = new JsonValueComparer();
                jsonValueComparer.Compare(testCase.ExpectedJson, actualJsonResult);
            }
        }

        /// <summary>
        /// Runs a single BufferingJsonReaderTestCaseDescriptor test.
        /// </summary>
        /// <param name="testCase">The test case descriptor to run.</param>
        /// <param name="assert"></param>
        public static void ReadAndVerifyBufferingJson(BufferingJsonReaderTestCaseDescriptor testCase, AssertionHandler assert)
        {
            TextReader testReader = new StringReader(testCase.JsonText);
            BufferingJsonReader bufferingJsonReader = new BufferingJsonReader(testReader, ODataConstants.DefaultMaxRecursionDepth, assert, isIeee754Compatible: true);

            bool isBuffering = false;
            int callCount = -1;
            int index = 0;
            int[] toggleCallCounts = testCase.ToggleBufferingCallCounts;
            int toggleAt = toggleCallCounts == null || toggleCallCounts.Length == 0 ? -1 : toggleCallCounts[index];

            int nonBufferingResultIndex = -1;
            int bufferingResultIndex = -1;

            do
            {
                callCount++;

                int ixToCompare;
                if (isBuffering)
                {
                    bufferingResultIndex++;
                    ixToCompare = bufferingResultIndex;
                }
                else
                {
                    nonBufferingResultIndex++;
                    ixToCompare = nonBufferingResultIndex;
                }

                assert.IsTrue(!isBuffering || bufferingResultIndex >= nonBufferingResultIndex, "Buffering index must be greater or equal than non-buffering one.");

                if (testCase.ExpectedNodes != null)
                {
                    assert.AreEqual(testCase.ExpectedNodes[ixToCompare].NodeType, bufferingJsonReader.NodeType, "Node types don't match.");
                    assert.AreEqual(testCase.ExpectedNodes[ixToCompare].Value, bufferingJsonReader.Value, "Values don't match.");
                }

                if (toggleAt == callCount)
                {
                    if (!isBuffering)
                    {
                        bufferingJsonReader.StartBuffering();
                        bufferingResultIndex = nonBufferingResultIndex;
                        isBuffering = true;
                    }
                    else
                    {
                        bufferingJsonReader.StopBuffering();
                        isBuffering = false;
                    }

                    if (index + 1 < toggleCallCounts.Length)
                    {
                        index++;
                        toggleAt = toggleCallCounts[index];
                    }
                }
            }
            while (bufferingJsonReader.Read());

            // we might have hit the end of the input in buffering mode; now empty the buffer.
            if (isBuffering)
            {
                bufferingJsonReader.StopBuffering();
                isBuffering = false;

                if (testCase.ExpectedNodes != null)
                {
                    assert.AreEqual(testCase.ExpectedNodes[nonBufferingResultIndex].NodeType, bufferingJsonReader.NodeType, "Node types don't match.");
                    assert.AreEqual(testCase.ExpectedNodes[nonBufferingResultIndex].Value, bufferingJsonReader.Value, "Values don't match.");
                }
            }

            while (bufferingJsonReader.Read())
            {
                nonBufferingResultIndex++;

                if (testCase.ExpectedNodes != null)
                {
                    assert.AreEqual(testCase.ExpectedNodes[nonBufferingResultIndex].NodeType, bufferingJsonReader.NodeType, "Node types don't match.");
                    assert.AreEqual(testCase.ExpectedNodes[nonBufferingResultIndex].Value, bufferingJsonReader.Value, "Values don't match.");
                }
            }

            // reading after end-of-input should stay in state end-of-input
            bufferingJsonReader.Read();
            assert.AreEqual(JsonNodeType.EndOfInput, bufferingJsonReader.NodeType, "Node types don't match.");
            assert.AreEqual(null, bufferingJsonReader.Value, "Values don't match.");
        }

        /// <summary>
        /// Runs a single BufferingJsonReaderTestCaseDescriptor test with a single toggle index in it
        /// and verifies that the reader state after turning off buffering is correct.
        /// </summary>
        /// <param name="testCase">The test case descriptor to run.</param>
        /// <param name="assert"></param>
        public static void ReadAndVerifyStateAfterStopBuffering(BufferingJsonReaderTestCaseDescriptor testCase, AssertionHandler assert)
        {
            assert.AreEqual(2, testCase.ToggleBufferingCallCounts.Length, "Expected a single toggle position.");

            TextReader testReader = new StringReader(testCase.JsonText);
            Exception exception = TestExceptionUtils.RunCatching(() =>
            {
                BufferingJsonReader bufferingJsonReader = new BufferingJsonReader(testReader, ODataConstants.DefaultMaxRecursionDepth, assert, isIeee754Compatible: true);
                   
                int callCount = -1;
                int startBuffering = testCase.ToggleBufferingCallCounts[0];
                int stopBuffering = testCase.ToggleBufferingCallCounts[1];
                bool isBuffering = false;

                List<BufferingJsonReaderTestCaseDescriptor.ReaderNode> bufferedNodes = new List<BufferingJsonReaderTestCaseDescriptor.ReaderNode>();

                bool hasMore = false;
                do
                {
                    callCount++;

                    if (startBuffering == callCount)
                    {
                        BufferingJsonReaderTestCaseDescriptor.ReaderNode bufferedNode = new BufferingJsonReaderTestCaseDescriptor.ReaderNode(bufferingJsonReader.NodeType, bufferingJsonReader.Value);
                        bufferedNodes.Add(bufferedNode);

                        bufferingJsonReader.StartBuffering();
                        isBuffering = true;
                    }

                    if (stopBuffering == callCount)
                    {
                        bufferingJsonReader.StopBuffering();
                        isBuffering = false;

                        assert.AreEqual(bufferedNodes[0].NodeType, bufferingJsonReader.NodeType, "Node types must be equal.");
                        assert.AreEqual(bufferedNodes[0].Value, bufferingJsonReader.Value, "Values must be equal.");
                        bufferedNodes.RemoveAt(0);
                    }

                    hasMore = bufferingJsonReader.Read();
                    if (isBuffering)
                    {
                        bufferedNodes.Add(new BufferingJsonReaderTestCaseDescriptor.ReaderNode(bufferingJsonReader.NodeType, bufferingJsonReader.Value));
                    }
                    else if (bufferedNodes.Count > 0)
                    {
                        assert.AreEqual(bufferedNodes[0].NodeType, bufferingJsonReader.NodeType, "Node types must be equal.");
                        assert.AreEqual(bufferedNodes[0].Value, bufferingJsonReader.Value, "Values must be equal.");
                        bufferedNodes.RemoveAt(0);
                    }
                }
                while (hasMore);
            });

            assert.IsNull(exception, "Did not expect an exception.");
        }

        /// <summary>
        /// Using JsonReader reads the specified <paramref name="textReader"/> input and returns a JSON OM.
        /// </summary>
        /// <param name="jsonReader">The JSON reader to read from.</param>
        /// <param name="assert">Assertion handler to use for validation.</param>
        /// <returns>The input read into a JSON OM using the JsonReader from the product.</returns>
        public static JsonValue ReadJson(JsonReader jsonReader, AssertionHandler assert)
        {
            JsonToObjectModelReader omReader = new JsonToObjectModelReader(jsonReader, assert);
            return omReader.ReadValue();
        }

        /// <summary>
        /// Insert the type name in jsonpayload.
        /// </summary>
        /// <param name="jsonPayload">jsonpayload.</param>
        /// <param name="typeName">type name to add to json payload.</param>
        /// <returns></returns>
        public static string InsertTypeNameInJsonPayload(string jsonPayload, string typeName)
        {
            return "{\"__metadata\":{\"type\":\"" + typeName + "\"}," + jsonPayload.Substring(1);
        }

        /// <summary>
        /// Helper class which takes a JsonReader and produces the JsonValue based object model from it.
        /// It also performs validation of the JsonReader along the way.
        /// </summary>
        private class JsonToObjectModelReader
        {
            /// <summary>
            /// The JsonReader from the product.
            /// </summary>
            private JsonReader reader;

            /// <summary>
            /// Assertion handler for validation.
            /// </summary>
            private AssertionHandler assert;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="reader">The JsonReader to use.</param>
            /// <param name="assert">The assertion handler for validation.</param>
            public JsonToObjectModelReader(JsonReader reader, AssertionHandler assert)
            {
                this.reader = reader;
                this.assert = assert;
            }

            /// <summary>
            /// Reads a JSON value from the reader.
            /// </summary>
            /// <returns>A JsonValue which represents the entire input from the reader.</returns>
            public JsonValue ReadValue()
            {
                if (!this.reader.Read())
                {
                    return null;
                }

                JsonValue value = null;
                switch (this.reader.NodeType)
                {
                    case JsonNodeType.PrimitiveValue:
                        value = this.ReadPrimitiveValue();
                        break;
                    case JsonNodeType.StartArray:
                        value = this.ReadArray();
                        break;
                    case JsonNodeType.StartObject:
                        value = this.ReadObject();
                        break;
                    default:
                        this.assert.Fail("Unexpected node type " + this.reader.NodeType.ToString() + " reported inside an Array.");
                        break;
                }

                this.assert.IsFalse(this.reader.Read(), "After the top-level value, no more nodes should be reported.");

                return value;
            }

            /// <summary>
            /// Reads a single JSON array.
            /// </summary>
            /// <returns>New JsonArray.</returns>
            private JsonArray ReadArray()
            {
                JsonArray array = new JsonArray();

                while (this.reader.Read())
                {
                    switch (this.reader.NodeType)
                    {
                        case JsonNodeType.EndArray:
                            return array;
                        case JsonNodeType.StartArray:
                            array.Add(this.ReadArray());
                            break;
                        case JsonNodeType.StartObject:
                            array.Add(this.ReadObject());
                            break;
                        case JsonNodeType.PrimitiveValue:
                            array.Add(this.ReadPrimitiveValue());
                            break;
                        default:
                            this.assert.Fail("Unexpected node type " + this.reader.NodeType.ToString() + " reported inside an Array.");
                            break;
                    }
                }

                this.assert.Fail("The reader should not report end of input in the middle of an array.");
                return null;
            }

            /// <summary>
            /// Reads a single JSON object.
            /// </summary>
            /// <returns>New JsonObject.</returns>
            private JsonObject ReadObject()
            {
                JsonObject obj = new JsonObject();

                while (this.reader.Read())
                {
                    switch (this.reader.NodeType)
                    {
                        case JsonNodeType.EndObject:
                            return obj;
                        case JsonNodeType.Property:
                            string propertyName = (string)this.reader.Value;
                            this.assert.IsTrue(this.reader.Read(), "After Property the reader must return another node.");
                            JsonValue propertyValue = null;
                            switch (this.reader.NodeType)
                            {
                                case JsonNodeType.StartArray:
                                    propertyValue = this.ReadArray();
                                    break;
                                case JsonNodeType.StartObject:
                                    propertyValue = this.ReadObject();
                                    break;
                                case JsonNodeType.PrimitiveValue:
                                    propertyValue = this.ReadPrimitiveValue();
                                    break;
                                default:
                                    this.assert.Fail("Unexpected node type " + this.reader.NodeType.ToString() + " reported inside a Property.");
                                    break;
                            }

                            obj.Add(new JsonProperty(propertyName, propertyValue));
                            break;
                        default:
                            this.assert.Fail("Unexpected node type " + this.reader.NodeType.ToString() + " reported inside an Object.");
                            break;
                    }
                }

                this.assert.Fail("The reader should not report end of input in the middle of an object.");
                return null;
            }

            /// <summary>
            /// Reads a JSON primitive value.
            /// </summary>
            /// <returns>New JsonPrimitiveValue.</returns>
            private JsonPrimitiveValue ReadPrimitiveValue()
            {
                return new JsonPrimitiveValue(this.reader.Value);
            }
        }
    }
}
