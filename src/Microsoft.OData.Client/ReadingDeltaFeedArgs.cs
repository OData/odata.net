//---------------------------------------------------------------------
// <copyright file="ReadingDeltaFeedArgs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------


namespace Microsoft.OData.Client
{
    /// <summary>
    /// The reading delta feed arguments
    /// </summary>
    public sealed class ReadingDeltaFeedArgs
    {
        /// <summary>
        /// Creates an instance of the <see cref="ReadingDeltaFeedArgs"/>
        /// </summary>
        /// <param name="deltaFeed">The delta dfeed.</param>
        public ReadingDeltaFeedArgs(ODataDeltaResourceSet deltaFeed)
        {
            Util.CheckArgumentNull(deltaFeed, "deltaFeed");
            this.DeltaFeed = deltaFeed;
        }

        /// <summary>
        /// Gets the delta feed.
        /// </summary>
        /// <value>
        /// The delta feed.
        /// </value>
        public ODataDeltaResourceSet DeltaFeed { get; private set; }
    }
}
