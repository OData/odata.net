//---------------------------------------------------------------------
// <copyright file="UriUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;

namespace Microsoft.OData
{
    /// <summary>
    /// Uri utility methods.
    /// </summary>
    internal static class UriUtils
    {
        private static readonly Uri DefaultMockBase = new("http://host/");

        /// <summary>
        /// Returns an absolute URI constructed from the specified base URI and a relative URI
        /// </summary>
        /// <param name="baseUri">The base URI to use.</param>
        /// <param name="relativeUri">The relative URI to use.</param>
        /// <returns>The absolute URI as a result of combining the base URI with the relative URI.</returns>
        internal static Uri UriToAbsoluteUri(Uri baseUri, Uri relativeUri)
        {
            Debug.Assert(baseUri != null, "baseUri != null");
            Debug.Assert(baseUri.IsAbsoluteUri, "baseUri is not absolute.");
            Debug.Assert(relativeUri != null, "relativeUri != null");
            Debug.Assert(!relativeUri.IsAbsoluteUri, "relativeUri is not relative.");

            return new Uri(baseUri, relativeUri);
        }

        /// <summary>
        /// A method to ensure that the original string of a relative URI is escaped.
        /// </summary>
        /// <param name="uri">The relative <see cref="System.Uri"/> to escape.</param>
        /// <returns>A relative URI instance with guaranteed escaped original string.</returns>
        internal static Uri EnsureEscapedRelativeUri(Uri uri)
        {
            Debug.Assert(uri != null && !uri.IsAbsoluteUri, "uri != null && !uri.IsAbsoluteUri");

            string escapedRelativeUri = uri.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped);
            if (string.CompareOrdinal(uri.OriginalString, escapedRelativeUri) == 0)
            {
                return uri;
            }

            return new Uri(escapedRelativeUri, UriKind.Relative);
        }

        /// <summary>
        /// Gets the escaped metadata reference property name.
        /// </summary>
        /// <param name="fragmentString">The metadata reference property name in question.</param>
        /// <returns>The Uri escaped metadata reference property name.</returns>
        internal static string EnsureEscapedFragment(string fragmentString)
        {
            Debug.Assert(fragmentString[0] == ODataConstants.ContextUriFragmentIndicator, "fragmentString[0] == " + ODataConstants.ContextUriFragmentIndicator);
            return ODataConstants.ContextUriFragmentIndicator + Uri.EscapeDataString(fragmentString.Substring(1));
        }

        /// <summary>
        /// Returns the unescaped string representation of the Uri; if the Uri is absolute returns the absolute Uri otherwise the original string.
        /// </summary>
        /// <param name="uri">The Uri to convert to a string.</param>
        /// <returns>For absolute Uris the string representation of the absolute Uri; otherwise the URIs original string.</returns>
        internal static string UriToString(Uri uri)
        {
            Debug.Assert(uri != null, "uri != null");

            // TODO: AbsoluteUri is escaped, OriginalString is not escaped
            // Doc comment says: "Return the unescaped string representation of the Uri" We'd need to use
            // Uri.UnescapeDataString on GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped) to achieve that for absolute Uris.
            // Fixing this would be a breaking change?
            return uri.IsAbsoluteUri ? uri.AbsoluteUri : uri.OriginalString;
        }

        /// <summary>
        /// Safely returns the specified string as a relative or absolute Uri.
        /// </summary>
        /// <param name="uriString">The string to convert to a Uri.</param>
        /// <returns>The string as a Uri.</returns>
        internal static Uri StringToUri(string uriString)
        {
            Uri uri = null;
            try
            {
                uri = new Uri(uriString, UriKind.RelativeOrAbsolute);
            }
            catch (System.FormatException)
            {
                // The Uri constructor throws a format exception if it can't figure out the type of Uri
                uri = new Uri(uriString, UriKind.Relative);
            }

            return uri;
        }

        /// <summary>
        /// Ensure the last character of Uri is a "/".
        /// </summary>
        /// <param name="uri">The Uri to deal with.</param>
        /// <returns>The uri with tailling "/".</returns>
        internal static Uri EnsureTaillingSlash(Uri uri)
        {
            if (uri == null)
            {
                return null;
            }

            string baseUriString = UriToString(uri);

            if (baseUriString[baseUriString.Length - 1] != ODataConstants.UriSegmentSeparatorChar)
            {
                return new Uri(baseUriString + ODataConstants.UriSegmentSeparator, UriKind.RelativeOrAbsolute);
            }

            return uri;
        }

        /// <summary>
        /// Determines whether the <paramref name="baseUri"/> Uri instance is a base of the specified Uri instance.
        /// </summary>
        /// <remarks>
        /// The check is host agnostic. For example, "http://host1.com/Service.svc" is a valid base Uri of
        /// "https://host2.org/Service.svc/Bla" but is not a valid base for "http://host1.com/OtherService.svc/Bla".
        /// Path comparison is case-insensitive and segment-aware to align with OData path name resolution.
        /// </remarks>
        /// <param name="baseUri">The candidate base URI.</param>
        /// <param name="uri">The specified Uri instance to test.</param>
        /// <returns>true if the baseUri Uri instance is a base of uri; otherwise false.</returns>
        internal static bool UriInvariantInsensitiveIsBaseOf(Uri baseUri, Uri uri)
        {
            ExceptionUtils.CheckArgumentNotNull(baseUri, nameof(baseUri));
            ExceptionUtils.CheckArgumentNotNull(uri, nameof(uri));

            // Require both Uris to be absolute
            if (!baseUri.IsAbsoluteUri)
            {
                throw new InvalidOperationException(Error.Format(SRResources.UriUtils_RequiresAbsoluteUri, nameof(baseUri)));
            }

            if (!uri.IsAbsoluteUri)
            {
                throw new InvalidOperationException(Error.Format(SRResources.UriUtils_RequiresAbsoluteUri, nameof(uri)));
            }

            Uri absBase = baseUri.IsAbsoluteUri ? baseUri : new Uri(DefaultMockBase, baseUri);
            Uri absUri = uri.IsAbsoluteUri ? uri : new Uri(DefaultMockBase, uri);

            // Host-agnostic path comparison (escaped, OrdinalIgnoreCase)
            ReadOnlySpan<char> basePath = absBase.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
            ReadOnlySpan<char> uriPath = absUri.GetComponents(UriComponents.Path, UriFormat.UriEscaped);

            // Treat empty or root base path as a base of any path on any host
            if (basePath.Length == 0)
            {
                return true;
            }

            // Fast prefix check (case-insensitive)
            if (!uriPath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // If equal length, it's an exact match => base-of
            if (uriPath.Length == basePath.Length)
            {
                return true;
            }

            // Boundary check: "/odata" is not a base of "/odata2"
            // If base ends with '/', it represents a complete segment; otherwise the next char must be '/'.
            return basePath[^1] == '/' || uriPath[basePath.Length] == '/';
        }

        /// <summary>
        /// Converts a string to a GUID value.
        /// </summary>
        /// <param name="text">String text to convert.</param>
        /// <param name="targetValue">After invocation, converted value.</param>
        /// <returns>true if the value was converted; false otherwise.</returns>
        /// <remarks>Copy of WebConvert.TryKeyStringToGuid.</remarks>
        internal static bool TryUriStringToGuid(ReadOnlySpan<char> text, out Guid targetValue)
        {
            // ABNF shows guidValue defined as
            // guidValue = 8HEXDIG "-" 4HEXDIG "-" 4HEXDIG "-" 4HEXDIG "-" 12HEXDIG
            // which comes to length of 36
            ReadOnlySpan<char> trimmedText = text.Trim();
            if (trimmedText.Length != 36 || trimmedText.IndexOf("-", StringComparison.Ordinal) != 8)
            {
                targetValue = default(Guid);
                return false;
            }

            return Guid.TryParse(text, out targetValue);

        }

        /// <summary>
        /// Converts a string to a DateTimeOffset value.
        /// </summary>
        /// <param name="text">String text to convert.</param>
        /// <param name="targetValue">After invocation, converted value.</param>
        /// <returns>true if the value was converted; false otherwise.</returns>
        /// <remarks>Copy of WebConvert.TryKeyStringToDateTimeOffset.</remarks>
        internal static bool ConvertUriStringToDateTimeOffset(ReadOnlySpan<char> text, out DateTimeOffset targetValue)
        {
            targetValue = default(DateTimeOffset);

            try
            {
                targetValue = PlatformHelper.ConvertStringToDateTimeOffset(text);
                return true;
            }
            catch (FormatException exception)
            {
                // This means it is a string similar to DateTimeOffset String, but cannot be parsed as DateTimeOffset and could not be a digit or GUID .etc.
                bool m = PlatformHelper.PotentialDateTimeOffsetValidator.IsMatch(text);
                if (m)
                {
                    // The format should be exactly "yyyy-mm-ddThh:mm:ss('.'s+)?(zzzzzz)?" and each field value is within valid range
                    throw new ODataException(Error.Format(SRResources.UriUtils_DateTimeOffsetInvalidFormat, text.ToString()), exception);
                }

                return false;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                // This means the timezone number is bigger than 14:00, inclusive exception has detail exception.
                throw new ODataException(Error.Format(SRResources.UriUtils_DateTimeOffsetInvalidFormat, text.ToString()), exception);
            }
        }

        /// <summary>
        /// Converts a string to a Date value.
        /// </summary>
        /// <param name="text">String text to convert.</param>
        /// <param name="targetValue">After invocation, converted value.</param>
        /// <returns>true if the value was converted; false otherwise.</returns>
        internal static bool TryUriStringToDate(ReadOnlySpan<char> text, out Date targetValue)
        {
            return PlatformHelper.TryConvertStringToDate(text, out targetValue);
        }

        /// <summary>
        /// Converts a string to a TimeOfDay value.
        /// </summary>
        /// <param name="text">String text to convert.</param>
        /// <param name="targetValue">After invocation, converted value.</param>
        /// <returns>true if the value was converted; false otherwise.</returns>
        internal static bool TryUriStringToTimeOfDay(ReadOnlySpan<char> text, out TimeOfDay targetValue)
        {
            return PlatformHelper.TryConvertStringToTimeOfDay(text, out targetValue);
        }

        /// <summary>
        /// Create mock absolute Uri from given Uri
        /// </summary>
        /// <param name="uri">The Uri to be operated on.</param>
        /// <returns>The mock Uri, the base Uri if given <paramref name="uri"/> is null</returns>
        internal static Uri CreateMockAbsoluteUri(Uri uri = null)
        {
            if (uri == null)
            {
                return DefaultMockBase;
            }

            return uri.IsAbsoluteUri ? uri : new Uri(DefaultMockBase, uri);
        }

        private static ReadOnlySpan<char> TrimSingleLeadingSlash(ReadOnlySpan<char> input)
        {
            // GetComponents(Path, Escaped) typically returns a leading slash for absolute URIs.
            // We can normalize away a single leading slash to make boundary logic simpler.
            return input.Length > 0 && input[0] == '/' ? input.Slice(1) : input;
        }
    }
}
