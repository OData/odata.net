//---------------------------------------------------------------------
// <copyright file="BufferingJsonReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Json
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// The test wrapper of the BufferingJsonReader implementation in the product which is internal.
    /// </summary>
    public class BufferingJsonReader : JsonReader
    {
        /// <summary>
        /// The type of the BufferingJsonReader from the product.
        /// </summary>
        private static Type BufferingJsonReaderType = typeof(Microsoft.OData.ODataAnnotatable).Assembly.GetType("Microsoft.OData.Json.BufferingJsonReader");

        /// <summary>
        /// The type of the JsonReader from the product.
        /// </summary>
        private static Type JsonReaderType = typeof(Microsoft.OData.ODataAnnotatable).Assembly.GetType("Microsoft.OData.Json.JsonReader");

        /// <summary>A flag indicating whether the reader is currently buffering or not.</summary>
        private bool isBuffering;

        /// <summary>The list of nodes reported when reading in buffered mode.</summary>
        /// <remarks>This list is used to verify that we read the same contents in buffered and non-buffered mode.</remarks>
        private readonly List<KeyValuePair<JsonNodeType, object>> bufferedNodes;

        /// <summary>
        /// Creates new instance of the BufferingJsonReader.
        /// </summary>
        /// <param name="textReader">The text reader to read the input from.</param>
        /// <param name="maxInnerErrorDepth">The maximumum number of recursive internalexception objects to allow when reading in-stream errors.</param>
        /// <param name="assert">Optional assertion handler to use to verify the behavior of the reader.</param>
        /// <param name="isIeee754Compatible">If it is IEEE754Compatible</param>
        public BufferingJsonReader(TextReader textReader, int maxInnerErrorDepth, AssertionHandler assert, bool isIeee754Compatible)
            : this(ReflectionUtils.CreateInstance(BufferingJsonReaderType,
                ReflectionUtils.CreateInstance(JsonReaderType, textReader, isIeee754Compatible),
                JsonConstants.ODataErrorName, maxInnerErrorDepth), assert)
        {
        }

        /// <summary>
        /// Creates new instance of the BufferingJsonReader.
        /// </summary>
        /// <param name="instance">The BufferingJsonReader instance.</param>
        /// <param name="assert">Optional assertion handler to use to verify the behavior of the reader.</param>
        protected BufferingJsonReader(object instance, AssertionHandler assert)
            : base(instance, assert)
        {
            this.bufferedNodes = new List<KeyValuePair<JsonNodeType, object>>();
        }

        /// <summary>
        /// Reads the next node from the input.
        /// </summary>
        /// <returns>true if next node was read, false if end of input was reached.</returns>
        /// <remarks>
        /// This override fills a buffer of what was read if in buffered mode and verifies the content
        /// as the non-buffered Read method is called.
        /// </remarks>
        public override bool Read()
        {
            bool result = base.Read();
            if (this.isBuffering)
            {
                // remember what we read when buffering is on; we use that later when reading in
                // non-buffering mode to ensure that we read the proper payload.
                this.bufferedNodes.Add(new KeyValuePair<JsonNodeType, object>(this.NodeType, this.Value));
            }
            else
            {
                // check the buffer and verify that we see the same node in non-buffering mode.
                if (this.bufferedNodes.Count > 0)
                {
                    KeyValuePair<JsonNodeType, object> kvp = this.bufferedNodes[0];
                    this.bufferedNodes.RemoveAt(0);

                    this.assert.AreEqual(kvp.Key, this.NodeType, "Node types must match.");
                    this.assert.AreEqual(kvp.Value, this.Value, "Node values must match.");
                }

                if (!result)
                {
                    this.assert.AreEqual(0, this.bufferedNodes.Count, "All buffered nodes should have been consumed.");
                }
            }

            return result;
        }

        /// <summary>
        /// Puts the reader into the state where it buffers read nodes.
        /// </summary>
        public void StartBuffering()
        {
            this.isBuffering = true;
            ReflectionUtils.InvokeMethod(this.instance, "StartBuffering");
        }

        /// <summary>
        /// Puts the reader into the state where no buffering happen on read.
        /// Either buffered nodes are consumed or new nodes are read (and not buffered).
        /// </summary>
        public void StopBuffering()
        {
            this.isBuffering = false;
            ReflectionUtils.InvokeMethod(this.instance, "StopBuffering");
        }
   }
}
