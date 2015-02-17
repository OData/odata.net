//---------------------------------------------------------------------
// <copyright file="MimePartData`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Http
{
    using System.Collections.Generic;

    /// <summary>
    /// Data structure for representing part of a MIME body
    /// </summary>
    /// <typeparam name="TBody">The type of the mime part's body</typeparam>
    public class MimePartData<TBody> : IMimePart
    {
        /// <summary>
        /// Initializes a new instance of the MimePartData class
        /// </summary>
        public MimePartData()
        {
            this.Headers = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the headers for the request
        /// </summary>
        public IDictionary<string, string> Headers { get; private set; }

        /// <summary>
        /// Gets or sets the body of the request
        /// </summary>
        public TBody Body { get; set; }
    }
}
