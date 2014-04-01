//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
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
