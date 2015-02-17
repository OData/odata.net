//---------------------------------------------------------------------
// <copyright file="UriHelpers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Collection of helpful utility functions for working with URIs
    /// </summary>
    public static class UriHelpers
    {
        /// <summary>
        /// Concatenates the given segments ensuring that only a single '/' character seperates each
        /// </summary>
        /// <param name="segments">The segments to concatenate</param>
        /// <returns>The concatenated segments</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "string is more useful overall")]
        public static string ConcatenateUriSegments(params string[] segments)
        {
            ExceptionUtilities.CheckCollectionDoesNotContainNulls(segments, "segments");

            if (segments.Length == 0)
            {
                return null;
            }

            if (segments.Length == 1)
            {
                return segments[0];
            }

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < segments.Length; i++)
            {
                // if this is the first segment, then only trim trailing slashes
                if (i == 0)
                {
                    builder.Append(segments[i].TrimEnd('/'));
                }
                else
                {
                    // if this is not the first segment, then we need to add a slash before concatenating the next
                    builder.Append('/');

                    // if we're not at the end, then trim slashes from both ends
                    if (i < segments.Length - 1)
                    {
                        builder.Append(segments[i].Trim('/'));
                    }
                    else
                    {
                        // if we're at the end, then trim only the leading slashes
                        builder.Append(segments[i].TrimStart('/'));
                    }
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Build query option string.
        /// </summary>
        /// <param name="builder">StringBuilder to build the query option string</param>
        /// <param name="queryOptions">query option key and value pairs</param>
        public static void ConcatenateQueryOptions(StringBuilder builder, IEnumerable<KeyValuePair<string, string>> queryOptions)
        {
            ExceptionUtilities.CheckArgumentNotNull(builder, "builder");
            ExceptionUtilities.CheckArgumentNotNull(queryOptions, "queryOptions");

            bool firstQueryOption = true;
            foreach (var queryOption in queryOptions)
            {
                if (!firstQueryOption)
                {
                    builder.Append('&');
                }

                firstQueryOption = false;
                builder.Append(queryOption.Key);
                builder.Append('=');
                builder.Append(queryOption.Value);
            }
        }

        /// <summary>
        /// Creates an absolute link for the given string
        /// </summary>
        /// <param name="link">The link string.</param>
        /// <param name="xmlBaseFragments">The XML base fragments, if any.</param>
        /// <returns>An absolute link for the given string</returns>
        public static Uri CreateAbsoluteLink(string link, IEnumerable<string> xmlBaseFragments)
        {
            var linkUri = new Uri(link, UriKind.RelativeOrAbsolute);
            if (linkUri.IsAbsoluteUri)
            {
                return linkUri;
            }

            var withXmlBase = ConcatenateUriSegments(xmlBaseFragments.Concat(new[] { link }).ToArray());
            return new Uri(withXmlBase, UriKind.Absolute);
        }
    }
}