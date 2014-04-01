//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    /// <summary>
    /// Enumeration of all possible states of an <see cref="ODataCollectionReader" />.
    /// </summary>
    public enum ODataCollectionReaderState
    {
        /// <summary>The reader is at the start; nothing has been read yet.</summary>
        /// <remarks>In this state, the Item property of the <see cref="ODataCollectionReader"/> returns null.</remarks>
        Start,

        /// <summary>
        /// The reader has started reading and is reading the start element of the collection wrapper (if any).
        /// No items have been read.
        /// </summary>
        /// <remarks>
        /// In this state, the Item property of the <see cref="ODataCollectionReader"/> returns 
        /// an instance of <see cref="ODataCollectionStart"/>.
        /// </remarks>
        CollectionStart,

        /// <summary>
        /// The reader read an item from the collection.
        /// </summary>
        /// <remarks>In this state, the Item property of the <see cref="ODataCollectionReader"/> returns the read item (a primitive value, an ODataComplexValue or null).</remarks>
        Value,
        
        /// <summary>
        /// The reader has finished reading and is reading the end element of the collection wrapper (if any).
        /// All items have been read.
        /// </summary>
        /// <remarks>
        /// In this state, the Item property of the <see cref="ODataCollectionReader"/> returns the same
        /// instance of <see cref="ODataCollectionStart"/> as in state CollectionStart.
        /// </remarks>
        CollectionEnd,

        /// <summary>The reader has thrown an exception; nothing can be read from the reader anymore.</summary>
        /// <remarks>
        /// In this state, the Item property of the <see cref="ODataCollectionReader"/> returns null.
        /// </remarks>
        Exception,

        /// <summary>The reader has completed; nothing can be read anymore.</summary>
        /// <remarks>
        /// In this state, the Item property of the <see cref="ODataCollectionReader"/> returns null.
        /// </remarks>
        Completed,
    }
}
