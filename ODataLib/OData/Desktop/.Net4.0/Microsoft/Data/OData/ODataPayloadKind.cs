//   Copyright 2011 Microsoft Corporation
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
    /// Enumeration representing the different kinds of payloads ODatLib can write.
    /// </summary>
    public enum ODataPayloadKind
    {
        /// <summary>Payload kind for a feed.</summary>
        Feed = 0,

        /// <summary>Payload kind for an entry.</summary>
        Entry = 1,

        /// <summary>Payload kind for a property.</summary>
        Property = 2,

        /// <summary>Payload kind for an entity reference link.</summary>
        EntityReferenceLink = 3,

        /// <summary>Payload kind for entity reference links.</summary>
        EntityReferenceLinks = 4,

        /// <summary>Payload kind for a raw value.</summary>
        Value = 5,

        /// <summary>Payload kind for a binary value.</summary>
        BinaryValue = 6,

        /// <summary>Payload kind for a collection.</summary>
        Collection = 7,

        /// <summary>Payload kind for a service document.</summary>
        ServiceDocument = 8,

        /// <summary>Payload kind for a metadata document.</summary>
        MetadataDocument = 9,

        /// <summary>Payload kind for an error.</summary>
        Error = 10,

        /// <summary>Payload kind for a batch.</summary>
        Batch = 11,

        /// <summary>Payload kind for parameters for a service action.</summary>
        Parameter = 12,

        /// <summary>Unknown format</summary>
        Unsupported = int.MaxValue,
    }
}
