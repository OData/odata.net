//---------------------------------------------------------------------
// <copyright file="IODataPayloadUriConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    #endregion

    /// <summary>Supports custom conversion of URLs found in the payload.</summary>
    /// <remarks>
    /// This interface can be implemented on messages (see <see cref="IODataRequestMessage"/> and
    /// <see cref="IODataResponseMessage"/>). When a message implementing this interface is
    /// passed to an <see cref="ODataMessageWriter"/> or <see cref="ODataMessageReader"/>, the
    /// message writer/reader will use this interface for custom URL conversion.
    /// On writers this means that whenever a URI is written into the payload the conversion
    /// method on this interface is called to convert a base URI and a payload URI to the
    /// actual URI to be written to the payload. If the method returns null from a conversion
    /// call the default conversion will be used.
    /// On readers this means that a base URI (either from the payload or the reader settings) and
    /// the URI read from the payload are passed to the method. The result is what is being reported
    /// on the OData OM instances. Again if the conversion method returns null the default conversion
    /// kicks in.
    /// </remarks>
    public interface IODataPayloadUriConverter
    {
        /// <summary>Implements a custom URL conversion scheme. This method returns null if no custom conversion is desired. If the method returns a non-null URL that value will be used without further validation.</summary>
        /// <returns>An instance that reflects the custom conversion of the method arguments into a URL or null if no custom conversion is desired; in that case the default conversion is used.</returns>
        /// <param name="baseUri">The (optional) base URI to use for the conversion.</param>
        /// <param name="payloadUri">The URI read from the payload.</param>
        Uri ConvertPayloadUri(Uri baseUri, Uri payloadUri);
    }
}
