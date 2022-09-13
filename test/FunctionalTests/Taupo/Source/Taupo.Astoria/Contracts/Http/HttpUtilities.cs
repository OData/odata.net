//---------------------------------------------------------------------
// <copyright file="HttpUtilities.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// A set of extension methods for interacting with HTTP components
    /// </summary>
    public static class HttpUtilities
    {
        private static readonly Encoding[] allEncodings = Encoding.GetEncodings().Select(e => e.GetEncoding()).ToArray();
        private static readonly string[] allEncodingWebNames = allEncodings.Select(e => e.WebName).Distinct().ToArray();
        private static readonly IDictionary<string, Encoding> allEncodingsByWebName = allEncodingWebNames.ToDictionary(n => n, n => allEncodings.First(e => e.WebName == n));

        /// <summary>
        /// Gets the default encoding to use when it cannot be inferred from a content-type header
        /// </summary>
        public static Encoding DefaultEncoding 
        { 
            get { return Encoding.UTF8; } 
        }

        /// <summary>
        /// Tries to get the given parameter from the content type
        /// </summary>
        /// <param name="contentType">The content type</param>
        /// <param name="parameterName">The content-type parameter to look for</param>
        /// <param name="parameterValue">The value of the parameter if it is found</param>
        /// <returns>Whether or not the parameter was found</returns>
        public static bool TryGetContentTypeParameter(string contentType, string parameterName, out string parameterValue)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(contentType, "contentType");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(parameterName, "parameterName");

            parameterValue = null;

            int parameterIndex = contentType.IndexOf(parameterName + "=", StringComparison.Ordinal);
            if (parameterIndex < 0)
            {
                return false;
            }

            parameterIndex += parameterName.Length + 1;

            int endParameterIndex = contentType.IndexOf(';', parameterIndex);
            if (endParameterIndex < 0)
            {
                parameterValue = contentType.Substring(parameterIndex);
            }
            else
            {
                parameterValue = contentType.Substring(parameterIndex, endParameterIndex - parameterIndex);
            }

            return true;
        }

        /// <summary>
        /// Tries to get the encoding with the given name
        /// </summary>
        /// <param name="encodingName">The encoding name</param>
        /// <param name="encoding">The encoding or null if it could not be found</param>
        /// <returns>A value indicating whether an encoding with the given name was found</returns>
        public static bool TryGetEncoding(string encodingName, out Encoding encoding)
        {
            if (string.IsNullOrEmpty(encodingName))
            {
                encoding = null;
                return false;
            }

            return allEncodingsByWebName.TryGetValue(encodingName, out encoding);
        }

        /// <summary>
        /// Gets the encoding with the given name or returns the default encoding if one could not be found
        /// </summary>
        /// <param name="encodingName">The encoding name</param>
        /// <returns>The specific encoding or the default encoding</returns>
        public static Encoding GetEncodingOrDefault(string encodingName)
        {
            Encoding encoding;
            if (TryGetEncoding(encodingName, out encoding))
            {
                return encoding;
            }

            return DefaultEncoding;
        }

        /// <summary>
        /// Concatenates the given information into a valid content-type header value
        /// </summary>
        /// <param name="mediaType">The media type, such as 'application/json'</param>
        /// <param name="charset">The 'charset' parameter value or null</param>
        /// <param name="boundary">The 'boundary' parameter value or null</param>
        /// <returns>The content type</returns>
        public static string BuildContentType(string mediaType, string charset, string boundary)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(mediaType, "mediaType");

            List<string> pieces = new List<string>() { mediaType };

            if (!string.IsNullOrEmpty(charset))
            {
                pieces.Add("charset=" + charset);
            }

            if (!string.IsNullOrEmpty(boundary))
            {
                pieces.Add("boundary=" + boundary);
            }

            return string.Join(";", pieces.ToArray());
        }

        /// <summary>
        /// Gets the value of the 'charset' parameter from the content type or returns null
        /// </summary>
        /// <param name="contentType">The content type</param>
        /// <returns>The 'charset' value or null</returns>
        public static string GetContentTypeCharsetOrNull(string contentType)
        {
            string charset;
            if (TryGetContentTypeParameter(contentType, HttpHeaders.Charset, out charset))
            {
                return charset;
            }

            return null;
        }
    }
}
