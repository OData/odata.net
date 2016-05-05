//---------------------------------------------------------------------
// <copyright file="ReorderingJsonReaderTestCaseDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight
{
    #region Namespaces
    using System.IO;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Json;
    #endregion Namespaces

    /// <summary>
    /// Describes a single ReorderingJsonReader test case.
    /// </summary>
    public class ReorderingJsonReaderTestCaseDescriptor
    {
        private readonly AssertionHandler assert;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ReorderingJsonReaderTestCaseDescriptor(AssertionHandler assert)
        {
            this.assert = assert;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The <see cref="BufferingJsonReaderTestDescriptor"/> instance to clone.</param>
        public ReorderingJsonReaderTestCaseDescriptor(ReorderingJsonReaderTestCaseDescriptor other)
        {
            this.assert = other.assert;
            this.JsonText = other.JsonText;
            this.ExpectedJsonText = other.ExpectedJsonText;
        }

        /// <summary>
        /// The JSON text to read. This is the input.
        /// </summary>
        public string JsonText { get; set; }

        /// <summary>
        /// The expected JSON text to be read. This is the expected result.
        /// </summary>
        public string ExpectedJsonText { get; set; }

        /// <summary>
        /// Runs the test by reading both payloads - one with the reordering reader 
        /// the other with the regular reader and comparing the results.
        /// </summary>
        public void RunTest()
        {
            using (TextReader expectedJsonTextReader = new StringReader(this.ExpectedJsonText))
            using (TextReader jsonTextReader = new StringReader(this.JsonText))
            {
                JsonReader expectedJsonReader = new JsonReader(expectedJsonTextReader, this.assert, isIeee754Compatible: true);
                ReorderingJsonReader reorderingJsonReader = new ReorderingJsonReader(jsonTextReader, /*maxInnerErrorDepth*/int.MaxValue, this.assert, isIeee754Compatible: true);

                while (reorderingJsonReader.Read())
                {
                    this.assert.IsTrue(expectedJsonReader.Read(), "Reordering Json reader reports more nodes than the regular one.");
                    this.assert.AreEqual(expectedJsonReader.NodeType, reorderingJsonReader.NodeType, "Node types don't match.");
                    this.assert.AreEqual(expectedJsonReader.Value, reorderingJsonReader.Value, "Values don't match.");
                }

                this.assert.IsFalse(expectedJsonReader.Read(), "Reordering Json reader reports less nodes than the regular one.");
            }
        }

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
