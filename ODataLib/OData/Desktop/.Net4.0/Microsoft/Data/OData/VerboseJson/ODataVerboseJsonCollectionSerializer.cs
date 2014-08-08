//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.VerboseJson
{
    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// OData Verbose JSON serializer for collections.
    /// </summary>
    internal sealed class ODataVerboseJsonCollectionSerializer : ODataVerboseJsonPropertyAndValueSerializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="verboseJsonOutputContext">The output context to write to.</param>
        internal ODataVerboseJsonCollectionSerializer(ODataVerboseJsonOutputContext verboseJsonOutputContext)
            : base(verboseJsonOutputContext)
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Writes the start of a collection.
        /// </summary>
        internal void WriteCollectionStart()
        {
            DebugUtils.CheckNoExternalCallers();

            // at the top level, we need to write the "results" wrapper for V2 and higher and for responses only
            if (this.WritingResponse && this.Version >= ODataVersion.V2)
            {
                // { "results":
                this.JsonWriter.StartObjectScope();
                this.JsonWriter.WriteDataArrayName();
            }

            // Write the start of the array for the collection items
            // "["
            this.JsonWriter.StartArrayScope();
        }

        /// <summary>
        /// Writes the end of a collection.
        /// </summary>
        internal void WriteCollectionEnd()
        {
            DebugUtils.CheckNoExternalCallers();

            // Write the end of the array for the collection items
            // "]"
            this.JsonWriter.EndArrayScope();

            // at the top level, we need to close the "results" wrapper for V2 and higher and for responses only
            if (this.WritingResponse && this.Version >= ODataVersion.V2)
            {
                // "}"
                this.JsonWriter.EndObjectScope();
            }
        }
    }
}
