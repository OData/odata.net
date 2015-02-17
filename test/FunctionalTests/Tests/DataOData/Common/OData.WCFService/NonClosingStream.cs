//---------------------------------------------------------------------
// <copyright file="NonClosingStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.WCFService
{
    using System.IO;

    /// <summary>
    /// Internal memory stream that does nothing when Close is called on the stream
    /// This is required for writing out errors as the WriteODataError method on the writer closes the stream
    /// Since we return streams from our wcf methods, this causes us to return a closed stream, throwing an error during serialization
    /// </summary>
    internal class NonClosingStream : MemoryStream
    {
        /// <summary>
        /// Do nothing when the stream is closed
        /// </summary>
        public override void Close()
        {

        }
    }
}