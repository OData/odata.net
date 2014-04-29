//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.Data.Spatial
{
    /// <summary>
    /// Represent an actual Json writing stream in Spatial.
    /// </summary>
    public interface IGeoJsonWriter
    {
        /// <summary>
        /// Start the object scope.
        /// </summary>
        void StartObjectScope();

        /// <summary>
        /// End the current object scope.
        /// </summary>
        void EndObjectScope();

        /// <summary>
        /// Start the array scope.
        /// </summary>
        void StartArrayScope();

        /// <summary>
        /// End the current array scope.
        /// </summary>
        void EndArrayScope();

        /// <summary>
        /// Add a property name to the current json object.
        /// </summary>
        /// <param name="name">The name to add.</param>
        void AddPropertyName(string name);

        /// <summary>
        /// Add a value to the current json scope.
        /// </summary>
        /// <param name="value">The value to add.</param>
        void AddValue(double value);

        /// <summary>
        /// Add a value to the current json scope.
        /// </summary>
        /// <param name="value">The value to add.</param>
        void AddValue(string value);
    }
}
