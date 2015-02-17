//---------------------------------------------------------------------
// <copyright file="JsonReaderTestCaseDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Json
{
    #region Namespaces
    using System;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Describes a single JsonReader test case.
    /// </summary>
    public class JsonReaderTestCaseDescriptor
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public JsonReaderTestCaseDescriptor()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The other test case to copy values from.</param>
        public JsonReaderTestCaseDescriptor(JsonReaderTestCaseDescriptor other)
        {
            this.JsonText = other.JsonText;
            this.ExpectedJson = other.ExpectedJson;
            this.FragmentExtractor = other.FragmentExtractor;
            this.ExpectedException = other.ExpectedException;
            this.DisablePayloadCombinations = other.DisablePayloadCombinations;
        }

        /// <summary>
        /// The JSON text to read. This is the input.
        /// </summary>
        public string JsonText { get; set; }

        /// <summary>
        /// The expected JSON OM. This is the expected result of reading the JsonText.
        /// If the FragmentExtractor is used this should only contain the part of the expected result
        /// extracted by the FragmentExtractor.
        /// </summary>
        public JsonValue ExpectedJson { get; set; }

        /// <summary>
        /// Func (if not null) which is called to process the result JSON OM and extract only the interesting portion from it.
        /// The result will be compared to the ExpectedJson property.
        /// </summary>
        public Func<JsonValue, JsonValue> FragmentExtractor { get; set; }

        /// <summary>
        /// If the test case should succeed, this needs to be null.
        /// If the test case is expected to fail, this contains the expected exception.
        /// </summary>
        public ExpectedException ExpectedException { get; set; }

        /// <summary>
        /// If set to true JsonReaderPayloads combinations will not be applied to this test case.
        /// </summary>
        public bool DisablePayloadCombinations { get; set; }

        /// <summary>
        /// Describes the test case.
        /// </summary>
        /// <returns>Humanly readable description of the test case - for debugging purposes.</returns>
        public override string ToString()
        {
            return this.JsonText;
        }
    }
}
