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
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Edm;

    /// <summary>
    /// Class provides context informatation of certain <see cref="IODataRequestMessage"/>
    /// or <see cref="IODataResponseMessage"/>
    /// </summary>
    public sealed class ODataMessageInfo
    {
        /// <summary>The parsed content type as <see cref="ODataMediaType"/>.</summary>
        public ODataMediaType MediaType { get; internal set; }

        /// <summary>The encoding specified in the charset parameter of contentType or the default encoding from MediaType.</summary>
        public Encoding Encoding { get; internal set; }

        /// <summary>The <see cref="IEdmModel"/> for the payload.</summary>
        public IEdmModel Model { get; internal set; }

        /// <summary>
        /// Whether is dealing with response.
        /// </summary>
        public bool IsResponse { get; internal set; }

        /// <summary>
        /// Function to get the message stream
        /// </summary>
        public Func<Stream> GetMessageStream { get; internal set; }

        /// <summary>
        /// The optional URL resolver to perform custom URL resolution for URLs read from the payload.
        /// </summary>
        public IODataUrlResolver UrlResolver { get; internal set; }

        /// <summary>
        /// The optional dependency injection container to get related services for message writing.
        /// </summary>
        public IServiceProvider Container { get; internal set; }

#if PORTABLELIB
        /// <summary>
        /// Function to get the message stream task
        /// </summary>
        public Func<Task<Stream>> GetMessageStreamAsync { get; internal set; }
#endif

        /// <summary>
        /// The payload kind for the message, currently used by <see cref="ODataRawInputContext"/> only.
        /// </summary>
        internal ODataPayloadKind PayloadKind { get; set; }
    }
}
