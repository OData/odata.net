//---------------------------------------------------------------------
// <copyright file="OperationsJsonReaderTestValues.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Json
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Test cases for JSON operations reader tests.
    /// </summary>
    public static class OperationsJsonReaderTestValues
    {
        /// <summary>
        /// Creates test cases for JSON operations reader tests.
        /// </summary>
        /// <returns>The list of test cases.</returns>
        public static IEnumerable<OperationsJsonReaderTestCase> CreateOperationsReaderTestCases()
        {
            return new[]
            {
                new OperationsJsonReaderTestCase
                {
                    DebugDescription = "null value",
                    Json = "null",
                    ExpectedExceptionFunc = (s) => ODataExpectedExceptions.ODataException("ODataJsonOperationsDeserializerUtils_OperationsPropertyMustHaveObjectValue", s, "PrimitiveValue"),
                },
                new OperationsJsonReaderTestCase
                {
                    DebugDescription = "array value",
                    Json = "[]",
                    ExpectedExceptionFunc = (s) => ODataExpectedExceptions.ODataException("ODataJsonOperationsDeserializerUtils_OperationsPropertyMustHaveObjectValue", s, "StartArray"),
                },
                new OperationsJsonReaderTestCase
                {
                    DebugDescription = "string value",
                    Json = "\"some string\"",
                    ExpectedExceptionFunc = (s) => ODataExpectedExceptions.ODataException("ODataJsonOperationsDeserializerUtils_OperationsPropertyMustHaveObjectValue", s, "PrimitiveValue"),
                },
                new OperationsJsonReaderTestCase
                {
                    DebugDescription = "null for metadata",
                    Json = "{ \"operationMetadata\" : null}",
                    ExpectedExceptionFunc = (s) => ODataExpectedExceptions.ODataException("ODataJsonOperationsDeserializerUtils_MetadataMustHaveArrayValue", "operationMetadata", "PrimitiveValue", s),
                },
                new OperationsJsonReaderTestCase
                { 
                    DebugDescription = "object for metadata",
                    Json = "{ \"operationMetadata\" : { \"target\" : \"http://odata.org\" }}",
                    ExpectedExceptionFunc = (s) => ODataExpectedExceptions.ODataException("ODataJsonOperationsDeserializerUtils_MetadataMustHaveArrayValue", "operationMetadata", "StartObject", s),
                },
                new OperationsJsonReaderTestCase
                {
                    DebugDescription = "empty array",
                    Json = "{ \"operationMetadata\" : []}",
                    ExpectedExceptionFunc = (s) => ODataExpectedExceptions.ODataException("ODataJsonOperationsDeserializerUtils_OperationMetadataArrayExpectedAnObject", "operationMetadata", "EndArray", s),
                },
                new OperationsJsonReaderTestCase
                {
                    DebugDescription = "missing target property",
                    Json = "{ \"operationMetadata\" : [ { \"title\" : \"some title\" }]}",
                    ExpectedExceptionFunc = (s) => ODataExpectedExceptions.ODataException("ODataJsonOperationsDeserializerUtils_OperationMissingTargetProperty", "operationMetadata", s),
                },
                new OperationsJsonReaderTestCase
                {
                    DebugDescription = "null target",
                    Json = "{ \"operationMetadata\" : [ { \"target\" : null }]}",
                    ExpectedExceptionFunc = (s) => ODataExpectedExceptions.ODataException("ODataJsonOperationsDeserializerUtils_OperationPropertyCannotBeNull", "target", "operationMetadata", s),
                },
                new OperationsJsonReaderTestCase
                {
                    DebugDescription = "null title",
                    Json = "{ \"operationMetadata\" : [ { \"title\" : null, \"target\" : \"http://odata.org\" }]}",
                    ExpectedExceptionFunc = (s) => ODataExpectedExceptions.ODataException("ODataJsonOperationsDeserializerUtils_OperationPropertyCannotBeNull", "title", "operationMetadata", s),
                },
                new OperationsJsonReaderTestCase
                {
                    DebugDescription = "duplicate target",
                    Json = "{ \"operationMetadata\" : [ { \"target\" : \"http://odata.org\", \"target\" : \"http://odata.com\" }]}",
                    ExpectedExceptionFunc = (s) => ODataExpectedExceptions.ODataException("ODataJsonOperationsDeserializerUtils_MultipleTargetPropertiesInOperation", "operationMetadata", s),
                },
                new OperationsJsonReaderTestCase
                {
                    DebugDescription = "duplicate title",
                    Json = "{ \"operationMetadata\" : [ { \"title\" : \"title1\", \"title\" : \"title2\", \"target\" : \"http://odata.org\" }]}",
                    ExpectedExceptionFunc = (s) => ODataExpectedExceptions.ODataException("ODataJsonOperationsDeserializerUtils_MultipleOptionalPropertiesInOperation", "title", "operationMetadata", s),
                },
                new OperationsJsonReaderTestCase
                {
                    DebugDescription = "second empty array",
                    Json = "{ \"operationMetadata\" : [ { \"target\" : \"http://odata.org\" }], \"operationMetadata2\" : []}",
                    ExpectedExceptionFunc = (s) => ODataExpectedExceptions.ODataException("ODataJsonOperationsDeserializerUtils_OperationMetadataArrayExpectedAnObject", "operationMetadata2", "EndArray", s),
                },
                new OperationsJsonReaderTestCase
                {
                    DebugDescription = "null title at the end",
                    Json = "{ \"operationMetadata\" : [ { \"target\" : \"http://odata.org\" }, { \"target\" : \"http://odata.org/target2\", \"title\" : null }]}",
                    ExpectedExceptionFunc = (s) => ODataExpectedExceptions.ODataException("ODataJsonOperationsDeserializerUtils_OperationPropertyCannotBeNull", "title", "operationMetadata", s),
                },
                new OperationsJsonReaderTestCase
                {
                    DebugDescription = "multiple metadata values",
                    Json = "{ \"operationMetadata\" : [ { \"target\" : \"http://odata.org\" }], \"operationMetadata\" : [ { \"target\" : \"http://odata.org/target2\" } ]}",
                    ExpectedExceptionFunc = (s) => ODataExpectedExceptions.ODataException("ODataJsonOperationsDeserializerUtils_RepeatMetadataValue", s, "operationMetadata"),
                },
            };
        }

        /// <summary>
        /// Describes a single operations JSON reader test case.
        /// </summary>
        public sealed class OperationsJsonReaderTestCase
        {
            /// <summary>The debug description of the test case.</summary>
            public string DebugDescription { get; set; }

            /// <summary>The JSON text of the operations property value.</summary>
            public string Json { get; set; }

            /// <summary>Func which takes a single parameter which is the name of the operations property and returns the expected exception for the test case.</summary>
            public Func<string, ExpectedException> ExpectedExceptionFunc { get; set; }
        }
    }
}