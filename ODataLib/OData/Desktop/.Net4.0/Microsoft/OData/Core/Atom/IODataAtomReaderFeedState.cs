//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Atom
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
