//---------------------------------------------------------------------
// <copyright file="BufferingJsonReaderTestCaseDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Json
{
    #region Namespaces
    using Microsoft.OData;
    using Microsoft.Test.Taupo.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Describes a single BufferingJsonReader test case.
    /// </summary>
    public class BufferingJsonReaderTestCaseDescriptor
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public BufferingJsonReaderTestCaseDescriptor()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The <see cref="BufferingJsonReaderTestDescriptor"/> instance to clone.</param>
        public BufferingJsonReaderTestCaseDescriptor(BufferingJsonReaderTestCaseDescriptor other)
        {
            this.JsonText = other.JsonText;
            this.ExpectedNodes = other.ExpectedNodes;
            this.ToggleBufferingCallCounts = other.ToggleBufferingCallCounts;
            this.RemoveDuplicateProperties = other.RemoveDuplicateProperties;
        }

        /// <summary>
        /// The JSON text to read. This is the input.
        /// </summary>
        public string JsonText { get; set; }

        /// <summary>
        /// A list of call counts after which to toggle the buffering strategy on the reader.
        /// </summary>
        public int[] ToggleBufferingCallCounts { get; set; }

        /// <summary>
        /// The list of nodes expected to be read from the reader.
        /// </summary>
        public ReaderNode[] ExpectedNodes { get; set; }

        /// <summary>
        /// true if the duplicate property removal should be turned on.
        /// </summary>
        public bool RemoveDuplicateProperties { get; set; }

        /// <summary>
        /// Describes the test case.
        /// </summary>
        /// <returns>Humanly readable description of the test case - for debugging purposes.</returns>
        public override string ToString()
        {
            return this.JsonText;
        }

        /// <summary>
        /// Class used to buffer nodes when reading in buffering mode.
        /// </summary>
        public sealed class ReaderNode
        {
            /// <summary>The type of the node read.</summary>
            private readonly JsonNodeType nodeType;

            /// <summary>The value of the node.</summary>
            private readonly object value;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="nodeType">The type of the node read.</param>
            /// <param name="value">The value of the node.</param>
            internal ReaderNode(JsonNodeType nodeType, object value)
            {
                this.nodeType = nodeType;
                this.value = value;
            }

            /// <summary>
            /// The type of the node read.
            /// </summary>
            internal JsonNodeType NodeType
            {
                get
                {
                    return this.nodeType;
                }
            }

            /// <summary>
            /// The value of the node.
            /// </summary>
            internal object Value
            {
                get
                {
                    return this.value;
                }
            }
        }
    }
}
