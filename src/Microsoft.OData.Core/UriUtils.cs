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
using Microsoft.OData.Edm;

namespace Microsoft.OData
{
    /// <summary>
    /// Uri utility methods.
    /// </summary>
    internal static class UriUtils
    {
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
        /// <returns>For absolute Uris the string representation of the absolute Uri; otherwise the Uri's original string.</returns>
        internal static string UriToString(Uri uri)
        {
            Debug.Assert(uri != null, "uri != null");

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
        /// Determines whether the <paramref name="baseUri"/> Uri instance is a
        /// base of the specified Uri instance.
        /// </summary>
        /// <remarks>
        /// The check is host agnostic. For example, "http://host1.com/Service.svc" is a valid base Uri of "https://host2.org/Service.svc/Bla"
        /// but is not a valid base for "http://host1.com/OtherService.svc/Bla".
        /// </remarks>
        /// <param name="baseUri">The candidate base URI.</param>
        /// <param name="uri">The specified Uri instance to test.</param>
        /// <returns>true if the baseUri Uri instance is a base of uri; otherwise false.</returns>
        internal static bool UriInvariantInsensitiveIsBaseOf(Uri baseUri, Uri uri)
        {
            Debug.Assert(baseUri != null, "baseUri != null");
            Debug.Assert(uri != null, "uri != null");

            Uri upperCurrent = CreateBaseComparableUri(baseUri);
            Uri upperUri = CreateBaseComparableUri(uri);

            return upperCurrent.IsBaseOf(upperUri);
        }

        /// <summary>
        /// Converts a string to a GUID value.
        /// </summary>
        /// <param name="text">String text to convert.</param>
        /// <param name="targetValue">After invocation, converted value.</param>
        /// <returns>true if the value was converted; false otherwise.</returns>
        /// <remarks>Copy of WebConvert.TryKeyStringToGuid.</remarks>
        internal static bool TryUriStringToGuid(string text, out Guid targetValue)
        {
            try
            {
                // ABNF shows guidValue defined as
                // guidValue = 8HEXDIG "-" 4HEXDIG "-" 4HEXDIG "-" 4HEXDIG "-" 12HEXDIG
                // which comes to length of 36
                string trimmedText = text.Trim();
                if (trimmedText.Length != 36 || trimmedText.IndexOf('-') != 8)
                {
                    targetValue = default(Guid);
                    return false;
                }

                targetValue = XmlConvert.ToGuid(text);
                return true;
            }
            catch (FormatException)
            {
                targetValue = default(Guid);
                return false;
            }
        }

        /// <summary>
        /// Converts a string to a DateTimeOffset value.
        /// </summary>
        /// <param name="text">String text to convert.</param>
        /// <param name="targetValue">After invocation, converted value.</param>
        /// <returns>true if the value was converted; false otherwise.</returns>
        /// <remarks>Copy of WebConvert.TryKeyStringToDateTimeOffset.</remarks>
        internal static bool ConvertUriStringToDateTimeOffset(string text, out DateTimeOffset targetValue)
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
                Match m = PlatformHelper.PotentialDateTimeOffsetValidator.Match(text);
                if (m.Success)
                {
                    // The format should be exactly "yyyy-mm-ddThh:mm:ss('.'s+)?(zzzzzz)?" and each field value is within valid range
                    throw new ODataException(Strings.UriUtils_DateTimeOffsetInvalidFormat(text), exception);
                }

                return false;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                // This means the timezone number is bigger than 14:00, inclusive exception has detail exception.
                throw new ODataException(Strings.UriUtils_DateTimeOffsetInvalidFormat(text), exception);
            }
        }

        /// <summary>
        /// Converts a string to a Date value.
        /// </summary>
        /// <param name="text">String text to convert.</param>
        /// <param name="targetValue">After invocation, converted value.</param>
        /// <returns>true if the value was converted; false otherwise.</returns>
        internal static bool TryUriStringToDate(string text, out Date targetValue)
        {
            targetValue = default(Date);

            try
            {
                targetValue = PlatformHelper.ConvertStringToDate(text);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Converts a string to a TimeOfDay value.
        /// </summary>
        /// <param name="text">String text to convert.</param>
        /// <param name="targetValue">After invocation, converted value.</param>
        /// <returns>true if the value was converted; false otherwise.</returns>
        internal static bool TryUriStringToTimeOfDay(string text, out TimeOfDay targetValue)
        {
            targetValue = default(TimeOfDay);

            try
            {
                targetValue = PlatformHelper.ConvertStringToTimeOfDay(text);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Create mock absoulte Uri from given Uri
        /// </summary>
        /// <param name="uri">The Uri to be operated on.</param>
        /// <returns>The mock Uri, the base Uri if given <paramref name="uri"/> is null</returns>
        internal static Uri CreateMockAbsoluteUri(Uri uri = null)
        {
            Uri BaseMockUri = new Uri("http://host/");

            if (uri == null)
            {
                return BaseMockUri;
            }

            if (uri.IsAbsoluteUri)
            {
                return uri;
            }
            else
            {
                return new Uri(BaseMockUri, uri);
            }
        }

        /// <summary>Creates a URI suitable for host-agnostic comparison purposes.</summary>
        /// <param name="uri">URI to compare.</param>
        /// <returns>URI suitable for comparison.</returns>
        private static Uri CreateBaseComparableUri(Uri uri)
        {
            Debug.Assert(uri != null, "uri != null");

#if !ORCAS
            uri = new Uri(UriUtils.UriToString(uri).ToUpperInvariant(), UriKind.RelativeOrAbsolute);
#else
            uri = new Uri(UriUtils.UriToString(uri).ToUpper(CultureInfo.InvariantCulture), UriKind.RelativeOrAbsolute);
#endif

            UriBuilder builder = new UriBuilder(uri);
            builder.Host = "h";
            builder.Port = 80;
            builder.Scheme = "http";
            return builder.Uri;
        }
    }
}
