//---------------------------------------------------------------------
// <copyright file="TestUriUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Helper methods to work with URIs
    /// </summary>
    public static class TestUriUtils
    {
        /// <summary>
        /// Returns the escaped URI string for the specified URI.
        /// </summary>
        /// <param name="uri">The URI to get the escaped string for.</param>
        /// <returns>The escaped URI string for <paramref name="uri"/>.</returns>
        public static string ToEscapedUriString(Uri uri)
        {
            if (uri.IsAbsoluteUri)
            {
                return uri.AbsoluteUri;
            }
            else
            {
                return uri.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped);
            }
        }


        /// <summary>
        /// Returns the absolute URI string for a URI; if the URI is not absolute, combines it with
        /// a specified base URI; otherwise returns the URI's original string.
        /// </summary>
        /// <param name="uri">The URI to get the absolute string for.</param>
        /// <param name="baseUri">The (optional) base URI.</param>
        /// <returns>The absolute URI string for the <paramref name="uri"/>.</returns>
        public static string ToAbsoluteUriString(Uri uri, Uri baseUri)
        {
            ExceptionUtilities.Assert(uri != null, "uri != null");
            ExceptionUtilities.Assert(baseUri == null || baseUri.IsAbsoluteUri, "baseUri == null || baseUri.IsAbsoluteUri");

            if (uri.IsAbsoluteUri)
            {
                return uri.AbsoluteUri;
            }

            if (baseUri != null)
            {
                Uri absoluteUri = new Uri(baseUri, uri);
                return absoluteUri.AbsoluteUri;
            }

            return ToEscapedUriString(uri);
        }
    }
}
