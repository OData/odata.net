//---------------------------------------------------------------------
// <copyright file="UriUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Edm.Library;

    #endregion Namespaces

    /// <summary>
    /// Uri utility methods.
    /// </summary>
    internal static class UriUtils
    {
        /// <summary>
        /// Base mock Uri
        /// </summary>
        private static readonly Uri BaseMockUri = new Uri("http://host/");

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
        /// Parses query options from a specified URI into a dictionary.
        /// </summary>
        /// <param name="uri">The uri to get the query options from.</param>
        /// <returns>The parsed query options.</returns>
        /// <remarks>This method returns <see cref="List&lt;CustomQueryOptionToken&gt;"/> with all the query options.
        /// Note that it is valid to include multiple query options with the same name.</remarks>
        internal static List<CustomQueryOptionToken> ParseQueryOptions(Uri uri)
        {
            Debug.Assert(uri != null, "uri != null");

            List<CustomQueryOptionToken> queryOptions = new List<CustomQueryOptionToken>();

            // COMPAT 31: Query options parsing
            // This method is a copy of System.Web.HttpValueCollection.FillFromString which is effectively the implementation
            // behind the System.Web.HttpUtility.ParseQueryString.
            // TODO: The System.Uri class does not replace/unescape URIs that use the '+' character to escape spaces;
            //      this however is common on the Web (also ASP.Net) and we have to figure out how we want to support it.
            string queryString = uri.Query.Replace('+', ' ');
            int length;
            if (queryString != null)
            {
                if (queryString.Length > 0 && queryString[0] == '?')
                {
                    queryString = queryString.Substring(1);
                }

                length = queryString.Length;
            }
            else
            {
                length = 0;
            }

            for (int i = 0; i < length; i++)
            {
                int startIndex = i;
                int equalSignIndex = -1;
                while (i < length)
                {
                    char ch = queryString[i];
                    if (ch == '=')
                    {
                        if (equalSignIndex < 0)
                        {
                            equalSignIndex = i;
                        }
                    }
                    else if (ch == '&')
                    {
                        break;
                    }

                    i++;
                }

                string queryOptionsName = null;
                string queryOptionValue = null;
                if (equalSignIndex >= 0)
                {
                    queryOptionsName = queryString.Substring(startIndex, equalSignIndex - startIndex);
                    queryOptionValue = queryString.Substring(equalSignIndex + 1, (i - equalSignIndex) - 1);
                }
                else
                {
                    queryOptionValue = queryString.Substring(startIndex, i - startIndex);
                }

                // COMPAT 31: Query options parsing
                // The System.Web version of the code uses HttpUtility.UrlDecode here, which calls into System.Web's own implementation
                // of the decoder. It's unclear if it's OK to use Uri.UnescapeDataString instead.
                queryOptionsName = queryOptionsName == null ? null : Uri.UnescapeDataString(queryOptionsName).Trim();
                queryOptionValue = queryOptionValue == null ? null : Uri.UnescapeDataString(queryOptionValue).Trim();

                queryOptions.Add(new CustomQueryOptionToken(queryOptionsName, queryOptionValue));

                if ((i == (length - 1)) && (queryString[i] == '&'))
                {
                    queryOptions.Add(new CustomQueryOptionToken(null, string.Empty));
                }
            }

            return queryOptions;
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
            uri = new Uri(Core.UriUtils.UriToString(uri).ToUpperInvariant(), UriKind.RelativeOrAbsolute);
#else
            uri = new Uri(Core.UriUtils.UriToString(uri).ToUpper(CultureInfo.InvariantCulture), UriKind.RelativeOrAbsolute);
#endif

            UriBuilder builder = new UriBuilder(uri);
            builder.Host = "h";
            builder.Port = 80;
            builder.Scheme = "http";
            return builder.Uri;
        }
    }
}