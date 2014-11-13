//   OData .NET Libraries ver. 6.8.1
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.OData.Core.Json
{
    using Microsoft.Data.Spatial;

    /// <summary>
    /// Convert writer interface from IJsonWriter to IGeoJsonWriter.
    /// This enables writer instance to be passed to Spatial to boost writing performance.
    /// </summary>
    internal sealed class GeoJsonWriterAdapter : IGeoJsonWriter
    {
        /// <summary>
        /// Inner writer of interface IJsonWriter.
        /// </summary>
        private readonly IJsonWriter writer;

        /// <summary>
        /// Constructor (only accessible from OData.Core
        /// </summary>
        /// <param name="writer">Inner writer of interface IJsonWriter.</param>
        internal GeoJsonWriterAdapter(IJsonWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Start the object scope.
        /// </summary>
        void IGeoJsonWriter.StartObjectScope()
        {
            this.writer.StartObjectScope();
        }

        /// <summary>
        /// End the current object scope.
        /// </summary>
        void IGeoJsonWriter.EndObjectScope()
        {
            this.writer.EndObjectScope();
        }

        /// <summary>
        /// Start the array scope.
        /// </summary>
        void IGeoJsonWriter.StartArrayScope()
        {
            this.writer.StartArrayScope();
        }

        /// <summary>
        /// End the current array scope.
        /// </summary>
        void IGeoJsonWriter.EndArrayScope()
        {
            this.writer.EndArrayScope();
        }

        /// <summary>
        /// Add a property name to the current json object.
        /// </summary>
        /// <param name="name">The name to add.</param>
        void IGeoJsonWriter.AddPropertyName(string name)
        {
            this.writer.WriteName(name);
        }

        /// <summary>
        /// Add a value to the current json scope.
        /// </summary>
        /// <param name="value">The value to add.</param>
        void IGeoJsonWriter.AddValue(double value)
        {
            this.writer.WriteValue(value);
        }

        /// <summary>
        /// Add a value to the current json scope.
        /// </summary>
        /// <param name="value">The value to add.</param>
        void IGeoJsonWriter.AddValue(string value)
        {
            this.writer.WriteValue(value);
        }
    }
}
