//---------------------------------------------------------------------
// <copyright file="BufferingJsonReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

    // These tests use Reflection to create & test internal product types,
    // (in the test wrapper of BufferingJsonReader) which cannot be done in Silverlight/Phone. 
    // Running these unit tests on desktop only is sufficient.

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Json
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for BufferingJsonReader class.
    /// </summary>
    /// <remarks>
    /// Reading JSON payload via the BufferingJsonReader with either no or full buffering 
    /// is tested as part of the JsonReaderTests. These tests ensure that turning on and off
    /// buffering while reading the payload works as expected.
    /// </remarks>
    [TestClass, TestCase]
    public class BufferingJsonReaderTests : ODataReaderTestCase
    {
        private const string jsonPayload = "{ \"a\": 1, \"b\": { \"c\": \"d\" }, \"e\": [ 3, 6, 9 ] }";

        private static readonly BufferingJsonReaderTestCaseDescriptor.ReaderNode[] expectedNodes = new BufferingJsonReaderTestCaseDescriptor.ReaderNode[]
        {
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.None, null),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.StartObject, null),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.Property, "a"),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.PrimitiveValue, 1),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.Property, "b"),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.StartObject, null),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.Property, "c"),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.PrimitiveValue, "d"),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.EndObject, null),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.Property, "e"),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.StartArray, null),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.PrimitiveValue, 3),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.PrimitiveValue, 6),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.PrimitiveValue, 9),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.EndArray, null),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.EndObject, null),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.EndOfInput, null),
        };

        private const string jsonPropertyDeduplicationPayload = "{ \"a\": 1, \"a\": null, \"a\": { \"c\": \"d\", \"c\": null }, \"e\": [ 3, 6, 9 ] }";

        private static readonly BufferingJsonReaderTestCaseDescriptor.ReaderNode[] expectedPropertyDeduplicationNodes = new BufferingJsonReaderTestCaseDescriptor.ReaderNode[]
        {
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.None, null),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.StartObject, null),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.Property, "a"),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.StartObject, null),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.Property, "c"),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.PrimitiveValue, null),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.EndObject, null),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.Property, "e"),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.StartArray, null),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.PrimitiveValue, 3),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.PrimitiveValue, 6),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.PrimitiveValue, 9),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.EndArray, null),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.EndObject, null),
            new BufferingJsonReaderTestCaseDescriptor.ReaderNode(JsonNodeType.EndOfInput, null),
        };

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Tests correct behavior of the BufferingJsonReader when turning buffering on and off while reading.")]
        public void ReadWithBufferingTest()
        {
            IEnumerable<BufferingJsonReaderTestCaseDescriptor> testCases = new BufferingJsonReaderTestCaseDescriptor[]
            {
                new BufferingJsonReaderTestCaseDescriptor() 
                {
                    JsonText = jsonPayload,
                    ExpectedNodes = expectedNodes,
                    RemoveDuplicateProperties = false
                },
                new BufferingJsonReaderTestCaseDescriptor() 
                {
                    JsonText = jsonPayload,
                    ExpectedNodes = expectedNodes,
                    RemoveDuplicateProperties = true
                },
            };

            IEnumerable<int[]> toggleBufferingCallCountsList = new int[][]
            {
                new int[] { 0 },
                new int[] { 15 },
                new int[] { 0, 5, 10 },
                new int[] { 0, 3, 12, 15 },
                new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                toggleBufferingCallCountsList,
                new bool[] { false, true },
                (testCase, toggleBufferingCallCounts, removeDuplicateProperties) =>
                {
                    testCase = new BufferingJsonReaderTestCaseDescriptor(testCase)
                    {
                        ToggleBufferingCallCounts = toggleBufferingCallCounts
                    };
                    
                    JsonReaderUtils.ReadAndVerifyBufferingJson(testCase, this.Assert);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Tests correct state of the BufferingJsonReader after turning buffering off.")]
        public void ReadAfterStopBufferingTest()
        {
            IEnumerable<BufferingJsonReaderTestCaseDescriptor> testCases = new BufferingJsonReaderTestCaseDescriptor[]
            {
                new BufferingJsonReaderTestCaseDescriptor
                {
                    JsonText = jsonPayload,
                    RemoveDuplicateProperties = false,
                },
                new BufferingJsonReaderTestCaseDescriptor
                {
                    JsonText = jsonPayload,
                    RemoveDuplicateProperties = true,
                },
                new BufferingJsonReaderTestCaseDescriptor
                {
                    JsonText = jsonPropertyDeduplicationPayload,
                    RemoveDuplicateProperties = false,
                },
                new BufferingJsonReaderTestCaseDescriptor
                {
                    JsonText = jsonPropertyDeduplicationPayload,
                    RemoveDuplicateProperties = true,
                },
            };

            IEnumerable<int[]> toggleBufferingCallCountsList = new int[][]
            {
                new int[] { 0, 1 },
                new int[] { 0, 3 },
                new int[] { 1, 2 },
                new int[] { 2, 5 },
                new int[] { 10, 11 },
                new int[] { 0, 0 },
                new int[] { 5, 5 },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                toggleBufferingCallCountsList,
                (testCase, toggleBufferingCallCounts) =>
                {
                    testCase = new BufferingJsonReaderTestCaseDescriptor(testCase)
                    {
                        ToggleBufferingCallCounts = toggleBufferingCallCounts
                    };

                    JsonReaderUtils.ReadAndVerifyStateAfterStopBuffering(testCase, this.Assert);
                });
        }
    }
}
