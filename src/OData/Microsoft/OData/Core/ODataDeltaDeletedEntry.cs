//   OData .NET Libraries ver. 6.9
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
