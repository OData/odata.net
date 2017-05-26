//---------------------------------------------------------------------
// <copyright file="TestHttpUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.OData;
    #endregion Namespaces

    /// <summary>
    /// Helper methods to work with HTTP concepts.
    /// </summary>
    public static class TestHttpUtils
    {
        public const string HttpVersionInBatching = "HTTP/1.1";

        /// <summary>
        /// HTTP method name for GET requests.
        /// </summary>
        public const string HttpMethodGet = "GET";

        /// <summary>
        /// HTTP method name for POST requests.
        /// </summary>
        public const string HttpMethodPost = "POST";

        /// <summary>
        /// HTTP method name for PUT requests.
        /// </summary>
        public const string HttpMethodPut = "PUT";

        /// <summary>
        /// HTTP method name for DELETE requests.
        /// </summary>
        public const string HttpMethodDelete = "DELETE";

        /// <summary>
        /// HTTP method name for PATCH requests.
        /// </summary>
        public const string HttpMethodPatch = "PATCH";

        /// <summary>
        /// Parses the response line and ensures that the response is using the correct response code and HTTP version.
        /// </summary>
        /// <param name="responseLine">The Http response line to parse.</param>
        /// <param name="expectedHttpVersion">The expected Http version.</param>
        /// <param name="expectedResponseCode">The expected Http response code.</param>
        /// <param name="error">A textual description of the validation error; or null if no error was detected.</param>
        /// <returns>True if the response line was successfully parsed and validated; otherwise false.</returns>
        public static bool ValidateResponseLine(string responseLine, string expectedHttpVersion, int expectedResponseCode, out string error)
        {
            error = null;
            if (!responseLine.StartsWith(expectedHttpVersion))
            {
                error = "Response line " + responseLine + " does not start with expected Http version '" + expectedHttpVersion + "'.";
                return false;
            }

            int responseCode = Int32.Parse((responseLine.Split(' '))[1]);
            if (expectedResponseCode != responseCode)
            {
                error = "Expected response code '" + expectedResponseCode + "' but found response code '" + responseCode + "'.";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Parses the request line and ensures that the request is using the expected Http method and Uri.
        /// </summary>
        /// <param name="requestLine">The Http request line to parse.</param>
        /// <param name="expectedMethod">The expected Http method.</param>
        /// <param name="expectedUri">The expected Uri.</param>
        /// <param name="error">A textual description of the validation error; or null if no error was detected.</param>
        /// <returns>True if the request line was successfully parsed and validated; otherwise false.</returns>
        public static bool ValidateRequestLine(string requestLine, string expectedMethod, Uri expectedUri, out string error)
        {
            error = null;

            string[] lineParts = requestLine.Split(' ');
            if (expectedMethod != lineParts[0])
            {
                error = "Expected Http method '" + expectedMethod + "' but found '" + lineParts[0] + "'.";
                return false;
            }

            Uri requestUri = new Uri(lineParts[1], UriKind.RelativeOrAbsolute);
            if (!expectedUri.Equals(requestUri))
            {
                error = "Expected Uri '" + expectedUri.ToString() + "' but found '" + requestUri.ToString() + "'.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates that the headers in the written message conform to the set of expected headers.
        /// </summary>
        /// <param name="reader">The text reader to read the header values from.</param>
        /// <param name="expectedHeaders">The set of expected headers. If this is null, the headers are just read but not validated.</param>
        /// <param name="error">A textual description of the validation error; or null if no error was detected.</param>
        /// <returns>True if the headers were successfully parsed and validated; otherwise false.</returns>
        public static bool ValidateHeaders(TextReader reader, Dictionary<string, string> expectedHeaders, out string error)
        {
            error = null;

            int readHeaderCount = 0;
            string line;
            while ((line = reader.ReadLine()).Length > 0)
            {
                readHeaderCount++;
                string headerName = line.Substring(0, line.IndexOf(':'));
                string headerValue = line.Substring(line.IndexOf(':') + 1).Trim();

                string expectedValue = null;
                if (expectedHeaders != null && !expectedHeaders.TryGetValue(headerName, out expectedValue))
                {
                    error = "Read header '" + headerName + "' but did not expect this header.";
                    return false;
                }

                // an expected header with null value indicates to ignore the header
                if (expectedValue != null && string.Compare(expectedValue, headerValue, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    error = "Found differing values for header '" + headerName + "'; expected '" + expectedValue + "' but found '" + headerValue + "'.";
                    return false;
                }
            }

            if (expectedHeaders != null)
            {
                if (readHeaderCount != expectedHeaders.Count)
                {
                    error = "Did not find the expected numbers of headers; expected " + expectedHeaders.Count + " but found " + readHeaderCount + ".";
                    return false;
                }
            }

            return true;
        }
    }
}
