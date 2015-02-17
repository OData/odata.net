//---------------------------------------------------------------------
// <copyright file="StreamWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.OptionalProviders
{
    using System;
    using System.IO;

    /// <summary>
    /// Wrapper class to contain a stream and its associated metadata
    /// </summary>
    internal class StreamWrapper
    {
        /// <summary>
        /// Gets or sets the name of the stream. Null should be used for media resources.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ETag stream for blob/named stream
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// Gets or sets the content-type of the stream
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the stream itself
        /// </summary>
        public Stream Stream { get; set; }

        /// <summary>
        /// Gets or sets the read uri of the stream
        /// </summary>
        public Uri ReadUri { get; set; }
    }
}
