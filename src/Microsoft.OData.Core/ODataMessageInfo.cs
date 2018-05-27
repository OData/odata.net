//---------------------------------------------------------------------
// <copyright file="ODataMessageInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Class provides context informatation of certain <see cref="IODataRequestMessage"/>
    /// or <see cref="IODataResponseMessage"/>
    /// </summary>
    public sealed class ODataMessageInfo
    {
        /// <summary>The parsed content type as <see cref="ODataMediaType"/>.</summary>
        public ODataMediaType MediaType { get; set; }

        /// <summary>The encoding specified in the charset parameter of contentType or the default encoding from MediaType.</summary>
        public Encoding Encoding { get; set; }

        /// <summary>The <see cref="IEdmModel"/> for the payload.</summary>
        public IEdmModel Model { get; set; }

        /// <summary>
        /// Whether is dealing with response.
        /// </summary>
        public bool IsResponse { get; set; }

        /// <summary>
        /// The optional URL converter to perform custom URL conversion for URLs read from the payload.
        /// </summary>
        public IODataPayloadUriConverter PayloadUriConverter { get; set; }

        /// <summary>
        /// The optional dependency injection container to get related services for message writing.
        /// </summary>
        public IServiceProvider Container { get; set; }

        /// <summary>
        /// Whether the message should be read or written asynchronously.
        /// </summary>
        public bool IsAsync { get; set; }

        /// <summary>
        /// The message stream created by GetMessageStream or GetMessageAsync.
        /// </summary>
        public Stream MessageStream { get; set; }

        /// <summary>
        /// The payload kind for the message, currently used by <see cref="ODataRawInputContext"/> only.
        /// </summary>
        internal ODataPayloadKind PayloadKind { get; set; }
    }
}
