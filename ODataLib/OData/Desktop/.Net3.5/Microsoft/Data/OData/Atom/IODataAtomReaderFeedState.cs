//   OData .NET Libraries ver. 5.6.3
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

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// Interface representing a state of the ATOM reader for feed.
    /// </summary>
    internal interface IODataAtomReaderFeedState
    {
        /// <summary>
        /// The feed being read.
        /// </summary>
        ODataFeed Feed { get; }

        /// <summary>
        /// Flag which indicates that the ATOM feed element representing the feed is empty.
        /// </summary>
        bool FeedElementEmpty { get; set; }

        /// <summary>
        /// The ATOM feed metadata to fill as we read the content of the feed.
        /// </summary>
        AtomFeedMetadata AtomFeedMetadata { get; }

        /// <summary>
        /// Flag which indicates if a m:count element was found.
        /// </summary>
        bool HasCount { get; set; }

        /// <summary>
        /// Flag which indicates if a link[@rel='next'] element was found.
        /// </summary>
        bool HasNextPageLink { get; set; }

        /// <summary>
        /// Flag which indicates if a link[@rel='self'] element was found.
        /// </summary>
        bool HasReadLink { get; set; }

        /// <summary>
        /// Flag which indicates if a link[@rel='http://docs.oasis-open.org/odata/ns/delta'] element was found.
        /// </summary>
        bool HasDeltaLink { get; set; }
    }
}
