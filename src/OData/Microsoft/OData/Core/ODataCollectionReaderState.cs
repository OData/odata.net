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
