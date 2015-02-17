//---------------------------------------------------------------------
// <copyright file="IODataAtomReaderFeedState.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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
