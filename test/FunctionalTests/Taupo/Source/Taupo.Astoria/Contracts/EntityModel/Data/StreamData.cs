//---------------------------------------------------------------------
// <copyright file="StreamData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel.Data
{
    /// <summary>
    /// Contains information about a media resource or named media resource
    /// </summary>
    internal class StreamData
    {
        /// <summary>
        /// Gets or sets the name of the stream. Null indicates that this is a the default media resource.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the stream's binary content
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Byte array is preferred representation")]
        public byte[] Content { get; set; }

        /// <summary>
        /// Gets or sets the etag of the stream
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ETag", Justification = "Casing is correct")]
        public string ETag { get; set; }

        /// <summary>
        /// Gets or sets the content type of the stream
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the edit link is unknown, but will be based on conventions
        /// </summary>
        public bool IsEditLinkBasedOnConvention { get; set; }

        /// <summary>
        /// Gets or sets the edit link of the stream
        /// </summary>
        public string EditLink { get; set; }

        /// <summary>
        /// Gets or sets the self link of the stream
        /// </summary>
        public string SelfLink { get; set; }
    }
}
