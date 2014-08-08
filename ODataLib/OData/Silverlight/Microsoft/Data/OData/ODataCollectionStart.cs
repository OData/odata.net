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

namespace Microsoft.Data.OData
{
    /// <summary>
    /// OData representation of a top-level collection.
    /// </summary>
    public sealed class ODataCollectionStart : ODataAnnotatable
    {
        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataCollectionWriter"/> for this <see cref="ODataCollectionStart"/>.
        /// </summary>
        private ODataCollectionStartSerializationInfo serializationInfo;

        /// <summary>Gets or sets the name of the collection (ATOM only).</summary>
        /// <returns>The name of the collection.</returns>
        public string Name
        {
            get;
            set;
        }
        
        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataCollectionWriter"/> for this <see cref="ODataCollectionStart"/>.
        /// </summary>
        internal ODataCollectionStartSerializationInfo SerializationInfo
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.serializationInfo;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.serializationInfo = ODataCollectionStartSerializationInfo.Validate(value);
            }
        }
    }
}
