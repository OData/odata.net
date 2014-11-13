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

namespace Microsoft.Data.Spatial
{
    using System;

    /// <summary>
    /// Writer to convert spatial types to Json that wraps a JsonWriter.
    /// </summary>
    internal sealed class WrappedGeoJsonWriter : GeoJsonWriterBase
    {
        #region Private Fields

        /// <summary>
        /// The actual stream to write Json.
        /// </summary>
        private readonly IGeoJsonWriter writer;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="writer">The actual stream to write Json.</param>
        public WrappedGeoJsonWriter(IGeoJsonWriter writer)
        {
            this.writer = writer;
        }

        #region Override Methods

        /// <summary>
        /// Start a new json object scope
        /// </summary>
        protected override void StartObjectScope()
        {
            writer.StartObjectScope();
        }

        /// <summary>
        /// Start a new json array scope
        /// </summary>
        protected override void StartArrayScope()
        {
            writer.StartArrayScope();
        }

        /// <summary>
        /// Add a property name to the current json object
        /// </summary>
        /// <param name="name">The name to add</param>
        protected override void AddPropertyName(String name)
        {
            writer.AddPropertyName(name);
        }

        /// <summary>
        /// Add a value to the current json scope
        /// </summary>
        /// <param name="value">The value to add</param>
        protected override void AddValue(String value)
        {
            writer.AddValue(value);
        }

        /// <summary>
        /// Add a value to the current json scope
        /// </summary>
        /// <param name="value">The value to add</param>
        protected override void AddValue(double value)
        {
            writer.AddValue(value);
        }

        /// <summary>
        /// End the current json array scope
        /// </summary>
        protected override void EndArrayScope()
        {
            writer.EndArrayScope();
        }

        /// <summary>
        /// End the current json object scope
        /// </summary>
        protected override void EndObjectScope()
        {
            writer.EndObjectScope();
        }

        #endregion
    }
}
