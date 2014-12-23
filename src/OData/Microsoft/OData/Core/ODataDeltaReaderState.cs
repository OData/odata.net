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
    /// Enumeration of all possible states of an <see cref="ODataDeltaReader" />.
    /// </summary>
    public enum ODataDeltaReaderState
    {
        /// <summary>The reader is at the start; nothing has been read yet.</summary>
        /// <remarks>In this state, the Item property of the <see cref="ODataDeltaReader"/> returns null.</remarks>
        Start,

        /// <summary>The start of a delta feed has been read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataDeltaReader"/> returns 
        /// an <see cref="ODataDeltaFeed"/> but no properties may be filled in until the DeltaFeedEnd state is reached.
        /// </remarks>
        DeltaFeedStart,

        /// <summary>The end of a delta feed has been read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataDeltaReader"/> returns 
        /// an <see cref="ODataDeltaFeed"/> with all properties filled in.
        /// </remarks>
        FeedEnd,

        /// <summary>The start of a delta entry has been read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataDeltaReader"/> returns 
        /// an <see cref="ODataEntry"/> but no properties may be filled in until the EntryEnd state is reached.
        /// </remarks>
        DeltaEntryStart,

        /// <summary>The end of a delta entry has been read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataDeltaReader"/> returns 
        /// an <see cref="ODataEntry"/> with all properties filled in.
        /// </remarks>
        DeltaEntryEnd,

        /// <summary>An delta deleted entry was read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataDeltaReader"/> returns
        /// an <see cref="ODataDeltaDeletedEntry"/> which is fully populated.
        /// Note that there's no End state for this item.
        /// </remarks>
        DeltaDeletedEntry,

        /// <summary>An delta link was read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataDeltaReader"/> returns
        /// an <see cref="ODataDeltaLink"/> which is fully populated.
        /// Note that there's no End state for this item.
        /// </remarks>
        DeltaLink,

        /// <summary>An delta deleted link was read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataDeltaReader"/> returns
        /// an <see cref="ODataDeltaDeletedLink"/> which is fully populated.
        /// Note that there's no End state for this item.
        /// </remarks>
        DeltaDeletedLink,

        /// <summary>The reader has thrown an exception; nothing can be read from the reader anymore.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataDeltaReader"/> returns null.
        /// </remarks>
        Exception,

        /// <summary>The reader has completed; nothing can be read anymore.</summary>
        /// <remarks>
        /// In this state, the Item property of the <see cref="ODataDeltaReader"/> returns null.
        /// </remarks>
        Completed,
    }
}
