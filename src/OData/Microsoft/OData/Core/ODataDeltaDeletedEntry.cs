//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    /// <summary>
    /// The reason of deleted entry in delta response.
    /// </summary>
    public enum DeltaDeletedEntryReason
    {
        /// <summary>
        /// Entity is deleted (destroyed).
        /// </summary>
        Deleted = 0,

        /// <summary>
        /// Entity is removed from membership in the result(i.e., due to a data change).
        /// </summary>
        Changed = 1,
    }

    /// <summary>
    /// Represents a deleted entity in delta response.
    /// </summary>
    public sealed class ODataDeltaDeletedEntry : ODataItem
    {
        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataDeltaWriter"/> for this <see cref="ODataDeltaDeletedEntry"/>.
        /// </summary>
        private ODataDeltaSerializationInfo serializationInfo;

        /// <summary>
        /// Initializes a new <see cref="ODataDeltaDeletedEntry"/>.
        /// </summary>
        /// <param name="id">The id of the deleted entity, which may be absolute or relative.</param>
        /// <param name="reason">The reason of deleted entry.</param>
        public ODataDeltaDeletedEntry(string id, DeltaDeletedEntryReason reason)
        {
            this.Id = id;
            this.Reason = reason;
        }

        /// <summary>
        /// The id of the deleted entity (same as the odata.id returned or computed when calling GET on resource), which may be absolute or relative.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Optional. Either deleted, if the entity was deleted (destroyed), or changed if the entity was removed from membership in the result (i.e., due to a data change).
        /// </summary>
        public DeltaDeletedEntryReason? Reason { get; set; }

        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataDeltaWriter"/> for this <see cref="ODataDeltaDeletedEntry"/>.
        /// </summary>
        internal ODataDeltaSerializationInfo SerializationInfo
        {
            get
            {
                return this.serializationInfo;
            }

            set
            {
                this.serializationInfo = ODataDeltaSerializationInfo.Validate(value);
            }
        }
    }
}
