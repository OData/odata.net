//---------------------------------------------------------------------
// <copyright file="UriUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client
#else
namespace Microsoft.OData.Service
#endif
{
    #region Namespaces

    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    #endregion Namespaces

    /// <summary>
    /// static utility functions for uris
    /// </summary>
    internal static class UriUtil
    {
#if ODATA_CLIENT
        /// <summary>forward slash char array for triming uris</summary>
        internal static readonly char[] ForwardSlash = new char[1] { '/' };
#endif

        /// <summary>
        /// Turn Uri instance into string representation
        /// This is needed because Uri.ToString unescapes the string
        /// </summary>
        /// <param name="uri">The uri instance</param>
        /// <returns>The string representation of the uri</returns>
        internal static string UriToString(Uri uri)
        {
            Debug.Assert(uri != null, "uri != null");
            return uri.IsAbsoluteUri ? uri.AbsoluteUri : uri.OriginalString;
        }

#if ODATA_CLIENT
        /// <summary>new Uri(string uriString, UriKind uriKind)</summary>
        /// <param name="value">value</param>
        /// <param name="kind">kind</param>
        /// <returns>new Uri(value, kind)</returns>
        internal static Uri CreateUri(string value, UriKind kind)
        {
            return value == null ? null : new Uri(value, kind);
        }

        /// <summary>new Uri(Uri baseUri, Uri requestUri)</summary>
        /// <param name="baseUri">baseUri</param>
        /// <param name="requestUri">relativeUri</param>
        /// <returns>new Uri(baseUri, requestUri)</returns>
        internal static Uri CreateUri(Uri baseUri, Uri requestUri)
        {
            Debug.Assert(baseUri != null, "baseUri != null");
            Util.CheckArgumentNull(requestUri, "requestUri");

            if (!baseUri.IsAbsoluteUri)
            {
                return CreateUri(UriToString(baseUri) + UriHelper.FORWARDSLASH + UriToString(requestUri), UriKind.Relative);
            }

            Debug.Assert(String.IsNullOrEmpty(baseUri.Query) && String.IsNullOrEmpty(baseUri.Fragment), "baseUri has query or fragment");

            // there is a bug in (new Uri(Uri,Uri)) which corrupts the port of the result if out relativeUri is also absolute
            if (requestUri.IsAbsoluteUri)
            {
                return requestUri;
            }

            return AppendBaseUriAndRelativeUri(baseUri, requestUri);
        }
#else
        /// <summary>
        /// Read the identifier from the uri segment value
        /// </summary>
        /// <param name="segment">One of the segments as returned by Uri.Segments method.</param>
        /// <returns>The segment identifier after stripping the last '/' character and unescaping the identifier.</returns>
        internal static string ReadSegmentValue(string segment)
        {
            if (segment.Length != 0 && segment != "/")
            {
                if (segment[segment.Length - 1] == '/')
                {
                    segment = segment.Substring(0, segment.Length - 1);
                }

                return Uri.UnescapeDataString(segment);
            }

            return null;
        }

        /// <summary>is the serviceRoot the base of the request uri</summary>
        /// <param name="baseUriWithSlash">baseUriWithSlash</param>
        /// <param name="requestUri">requestUri</param>
        /// <returns>true if the serviceRoot is the base of the request uri</returns>
        internal static bool IsBaseOf(Uri baseUriWithSlash, Uri requestUri)
        {
            return baseUriWithSlash.IsBaseOf(requestUri);
        }

        /// <summary>
        /// Determines whether the <paramref name="current"/> Uri instance is a
        /// base of the specified Uri instance.
        /// </summary>
        /// <param name="current">Candidate base URI.</param>
        /// <param name="uri">The specified Uri instance to test.</param>
        /// <returns>true if the current Uri instance is a base of uri; otherwise, false.</returns>
        internal static bool UriInvariantInsensitiveIsBaseOf(Uri current, Uri uri)
        {
            Debug.Assert(current != null, "current != null");
            Debug.Assert(uri != null, "uri != null");

            Uri upperCurrent = CreateBaseComparableUri(current);
            Uri upperUri = CreateBaseComparableUri(uri);

            return IsBaseOf(upperCurrent, upperUri);
        }
#endif

#if ODATA_CLIENT
        /// <summary>
        /// Appends the absolute baseUri with the relativeUri to create a new absolute uri
        /// </summary>
        /// <param name="baseUri">An absolute Uri</param>
        /// <param name="relativeUri">A relative Uri</param>
        /// <returns>An absolute Uri that is the combination of the base and relative Uris passed in.</returns>
        private static Uri AppendBaseUriAndRelativeUri(Uri baseUri, Uri relativeUri)
        {
            Debug.Assert(baseUri != null, "baseUri != null");
            Debug.Assert(baseUri.IsAbsoluteUri, "relativeUri is not relative");
            Debug.Assert(relativeUri != null, "relativeUri != null");
            Debug.Assert(!relativeUri.IsAbsoluteUri, "relativeUri is not relative");

            string baseUriString = UriToString(baseUri);
            string relativeUriString = UriToString(relativeUri);

            if (baseUriString.EndsWith("/", StringComparison.Ordinal))
            {
                if (relativeUriString.StartsWith("/", StringComparison.Ordinal))
                {
                    relativeUri = new Uri(baseUri, CreateUri(relativeUriString.TrimStart(ForwardSlash), UriKind.Relative));
                }
                else
                {
                    relativeUri = new Uri(baseUri, relativeUri);
                }
            }
            else
            {
                relativeUri = CreateUri(baseUriString + "/" + relativeUriString.TrimStart(ForwardSlash), UriKind.Absolute);
            }

            return relativeUri;
        }
#else
         /// <summary>Creates a URI suitable for host-agnostic comparison purposes.</summary>
        /// <param name="uri">URI to compare.</param>
        /// <returns>URI suitable for comparison.</returns>
        private static Uri CreateBaseComparableUri(Uri uri)
        {
            Debug.Assert(uri != null, "uri != null");

            uri = new Uri(UriToString(uri).ToUpper(CultureInfo.InvariantCulture), UriKind.RelativeOrAbsolute);

            UriBuilder builder = new UriBuilder(uri);
            builder.Host = "h";
            builder.Port = 80;
            builder.Scheme = "http";
            return builder.Uri;
        }
#endif
    }
}