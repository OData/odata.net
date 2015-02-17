//---------------------------------------------------------------------
// <copyright file="Link.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Reliability
{
    /// <summary>
    /// Link class
    /// </summary>
    public class Link
    {
        /// <summary>
        /// Gets name
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets rel
        /// </summary>
        public string Rel { get; internal set; }

        /// <summary>
        /// Gets uri
        /// </summary>
        public string Uri { get; internal set; }

        /// <summary>
        /// Gets type
        /// </summary>
        public string Type { get; internal set; }

        /// <summary>
        /// Gets value
        /// </summary>
        public string Value { get; internal set; }

        /// <summary>
        /// Gets inline feeds
        /// </summary>
        public AtomResult InlineFeed { get; internal set; }

        /// <summary>
        /// Gets inline entry
        /// </summary>
        public Entry InlineEntry { get; internal set; }
    }
}
