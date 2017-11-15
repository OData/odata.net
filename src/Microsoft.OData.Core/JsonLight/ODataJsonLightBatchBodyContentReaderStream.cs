//---------------------------------------------------------------------
// <copyright file="ODataJsonLightBatchBodyContentReaderStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;

    using Microsoft.OData.Json;

    #endregion Namespaces

    /// <summary>
    /// Wrapper stream backed by memory stream containing body content of request or response in Json batch.
    /// </summary>
    internal sealed class ODataJsonLightBatchBodyContentReaderStream : MemoryStream, IDisposable
    {
        /// <summary>Listener interface to be notified of operation changes.</summary>
        private IODataBatchOperationListener listener;

        /// <summary>
        /// Type of the data in body.
        /// </summary>
        private BatchPayloadBodyContentType contentType;

        ///// <summary>
        ///// Status to ensure that the underlying memory stream is ready for reading.
        ///// </summary>
        // private bool isDataPopulatedToStream;

        /// <summary>
        /// Constructor using default encoding (UTF-8 without the BOM preamble)
        /// </summary>
        internal ODataJsonLightBatchBodyContentReaderStream(IODataBatchOperationListener listener)
            : this(listener, MediaTypeUtils.FallbackEncoding)
        {
            this.listener = listener;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="batchEncoding">The encoding to use to read and write from the batch stream.</param>
        private ODataJsonLightBatchBodyContentReaderStream(IODataBatchOperationListener listener, Encoding batchEncoding)
        {
        }

        /// <summary>
        /// Enum type for data type of body content.
        /// </summary>
        private enum BatchPayloadBodyContentType
        {
            // The body content contains Json data.
            Json,

            // The body content contains binary data, e.g. JPEG image raw data.
            Binary
        }

        /// <summary>
        /// Populate the current property value the Json reader is referencing.
        /// The property value should be of either Json type or binary type.
        /// </summary>
        /// <param name="jsonReader">The Json reader providing access to the data.</param>
        internal void PopulateBodyContent(IJsonReader jsonReader)
        {
            // Debug.Assert(!this.isDataPopulatedToStream, "!this.isDataPopulatedToStream");
            DetectBatchPayloadBodyContentType(jsonReader);

            switch (this.contentType)
            {
                case BatchPayloadBodyContentType.Json:
                    {
                        WriteJsonContent(jsonReader);
                    }

                    break;

                case BatchPayloadBodyContentType.Binary:
                    {
                        // body content is a string value of binary data.
                        string contentStr = jsonReader.ReadStringValue();

                        byte[] contentBytes = Encoding.UTF8.GetBytes(contentStr);

                        WriteBinaryContent(contentBytes);
                    }

                    break;

                default:
                    throw new NotSupportedException("unknown / undefined type, new type that needs to be supported?");
            }
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">True if called from Dispose; false if called form the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.listener != null)
                {
                    // Tell the listener that the stream is being disposed; we expect
                    // that no asynchronous action is triggered by doing so.
                    this.listener.BatchOperationContentStreamDisposed();
                    this.listener = null;
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Detect batch item (request or response) body content's data type.
        /// The content of the "body" property can be either Json type or binary type.
        /// </summary>
        /// <param name="jsonReader">The json reader that provides access to the json object.</param>
        private void DetectBatchPayloadBodyContentType(IJsonReader jsonReader)
        {
            if (jsonReader.NodeType == JsonNodeType.StartObject)
            {
                this.contentType = BatchPayloadBodyContentType.Json;
            }
            else if (jsonReader.NodeType == JsonNodeType.PrimitiveValue)
            {
                this.contentType = BatchPayloadBodyContentType.Binary;
            }
            else
            {
                throw new ODataException(string.Format(
                    CultureInfo.InvariantCulture,
                    "Unsupported body content of NodeType: [{0}]",
                    jsonReader.NodeType));
            }
        }

        /// <summary>
        /// Read off the data of the starting Json object from the Json reader,
        /// and populate the data into the memory stream.
        /// </summary>
        /// <param name="reader"> The json reader pointing at the json structure whose data needs to
        /// be populated into an memory stream.
        /// </param>
        private void WriteJsonContent(IJsonReader reader)
        {
            // Debug.Assert(!this.isDataPopulatedToStream, "!this.isDataPopulatedToStream");
            // Reader is on the value node after the "body" property name node.
            IJsonWriter jsonWriter = new JsonWriter(
                new StreamWriter(this),
                true /*isIeee754Compatible*/);

            WriteCurrentJsonObject(reader, jsonWriter);
            this.Flush();
            this.Position = 0;
        }

        /// <summary>
        /// Writes the binary bytes to the underlying memory stream.
        /// </summary>
        /// <param name="bytes">The bytes to be written.</param>
        private void WriteBinaryContent(byte[] bytes)
        {
            this.Write(bytes, 0, bytes.Length);
            this.Flush();
            this.Position = 0;
        }

        /// <summary>
        /// Write the current Json object.
        /// </summary>
        /// <param name="reader">The Json reader providing the data.</param>
        /// <param name="jsonWriter">The Json writer writes data into memory stream.</param>
        private static void WriteCurrentJsonObject(IJsonReader reader, IJsonWriter jsonWriter)
        {
            Stack<JsonNodeType> nodeTypes = new Stack<JsonNodeType>();

            do
            {
                switch (reader.NodeType)
                {
                    case JsonNodeType.PrimitiveValue:
                        {
                            if (reader.Value != null)
                            {
                                jsonWriter.WritePrimitiveValue(reader.Value);
                            }
                            else
                            {
                                jsonWriter.WriteValue((string)null);
                            }
                        }

                        break;

                    case JsonNodeType.Property:
                        {
                            jsonWriter.WriteName(reader.Value.ToString());
                        }

                        break;

                    case JsonNodeType.StartObject:
                        {
                            nodeTypes.Push(reader.NodeType);
                            jsonWriter.StartObjectScope();
                        }

                        break;

                    case JsonNodeType.StartArray:
                        {
                            nodeTypes.Push(reader.NodeType);
                            jsonWriter.StartArrayScope();
                        }

                        break;

                    case JsonNodeType.EndObject:
                        {
                            Debug.Assert(nodeTypes.Peek() == JsonNodeType.StartObject);
                            nodeTypes.Pop();
                            jsonWriter.EndObjectScope();
                        }

                        break;

                    case JsonNodeType.EndArray:
                        {
                            Debug.Assert(nodeTypes.Peek() == JsonNodeType.StartArray);
                            nodeTypes.Pop();
                            jsonWriter.EndArrayScope();
                        }

                        break;

                    default:
                        {
                            throw new ODataException(String.Format(
                                CultureInfo.InvariantCulture,
                                "Unexpected reader.NodeType: {0}.",
                                reader.NodeType));
                        }
                }

                reader.ReadNext(); // This can be EndOfInput, where nodeTypes should be empty.
            }
            while (nodeTypes.Count != 0);

            jsonWriter.Flush();
        }
    }
}