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

namespace System.Data.OData
{
    /// <summary>
    /// Enumeration representing the different kinds of payloads ODatLib can write.
    /// </summary>
#if INTERNAL_DROP
    internal enum ODataPayloadKind
#else
    public enum ODataPayloadKind
#endif
    {
        /// <summary>Payload kind for writing a feed.</summary>
        Feed = 0,

        /// <summary>Payload kind for writing an entry.</summary>
        Entry = 1,

        /// <summary>Payload kind for writing a property.</summary>
        Property = 2,

        /// <summary>Payload kind for writing an associated entity link.</summary>
        AssociatedEntityLink = 3,

        /// <summary>Payload kind for writing a raw value.</summary>
        Value = 4,

        /// <summary>Payload kind for writing a binary value.</summary>
        BinaryValue = 5,

        /// <summary>Payload kind for writing a collection.</summary>
        Collection = 6,

        /// <summary>Payload kind for writing a service document.</summary>
        ServiceDocument = 7,

        /// <summary>Payload kind for writing a metadata document.</summary>
        MetadataDocument = 8,

        /// <summary>Payload kind for writing an error.</summary>
        Error = 9,

        /// <summary>Payload kind for writing a batch.</summary>
        Batch = 10,

        /// <summary>Unknown format</summary>
        Unsupported = int.MaxValue,
    }
}
