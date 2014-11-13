//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    #endregion

    /// <summary>Supports custom resolution of URLs found in the payload.</summary>
    /// <remarks>
    /// This interface can be implemented on messages (see <see cref="IODataRequestMessage"/> and
    /// <see cref="IODataResponseMessage"/>). When a message implementing this interface is
    /// passed to an <see cref="ODataMessageWriter"/> or <see cref="ODataMessageReader"/>, the
    /// message writer/reader will use this interface for custom URL resolution.
    /// On writers this means that whenever a URI is written into the payload the resolution
    /// method on this interface is called to resolve a base URI and a payload URI to the
    /// actual URI to be written to the payload. If the method returns null from a resolution
    /// call the default resolution will be used.
    /// On readers this means that a base URI (either from the payload or the reader settings) and
    /// the URI read from the payload are passed to the method. The result is what is being reported
    /// on the OData OM instances. Again if the resolution method returns null the default resolution
    /// kicks in.
    /// </remarks>
    public interface IODataUrlResolver
    {
        /// <summary>Implements a custom URL resolution scheme. This method returns null if no custom resolution is desired. If the method returns a non-null URL that value will be used without further validation.</summary>
        /// <returns>An instance that reflects the custom resolution of the method arguments into a URL or null if no custom resolution is desired; in that case the default resolution is used.</returns>
        /// <param name="baseUri">The (optional) base URI to use for the resolution.</param>
        /// <param name="payloadUri">The URI read from the payload.</param>
        Uri ResolveUrl(Uri baseUri, Uri payloadUri);
    }
}
