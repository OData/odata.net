//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
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
                return this.serializationInfo;
            }

            set
            {
                this.serializationInfo = ODataCollectionStartSerializationInfo.Validate(value);
            }
        }
    }
}
