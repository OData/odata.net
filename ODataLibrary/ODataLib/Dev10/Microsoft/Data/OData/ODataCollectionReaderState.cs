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
        /// an instance of <see cref="ODataCollectionResult"/>.
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
        /// instance of <see cref="ODataCollectionResult"/> as in state CollectionStart.
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
