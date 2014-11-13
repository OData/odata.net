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

namespace Microsoft.OData.Core
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

        /// <summary>Payload kind for individual property in an entity.</summary>
        IndividualProperty = 13,

        /// <summary>Payload kind for delta.</summary>
        Delta = 14,

        /// <summary>Payload kind for async.</summary>
        Asynchronous = 15,

        /// <summary>Unknown format</summary>
        Unsupported = int.MaxValue,
    }
}
