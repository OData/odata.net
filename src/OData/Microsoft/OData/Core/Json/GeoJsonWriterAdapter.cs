//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
