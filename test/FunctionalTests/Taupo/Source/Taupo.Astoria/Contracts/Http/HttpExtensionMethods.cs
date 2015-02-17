//---------------------------------------------------------------------
// <copyright file="HttpExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Http
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// A set of extension methods for interacting with HTTP components
    /// </summary>
    public static class HttpExtensionMethods
    {
        /// <summary>
        /// Returns the http method string for the given verb
        /// </summary>
        /// <param name="verb">The verb to get the http method for</param>
        /// <returns>The http method string (the name of the verb, all uppercase)</returns>
        public static string ToHttpMethod(this HttpVerb verb)
        {
            return verb.ToString().ToUpperInvariant();
        }

        /// <summary>
        /// Returns the http method string for the given verb
        /// </summary>
        /// <param name="verb">The verb to get the http method for</param>
        /// <returns>The http method string (the name of the verb, all uppercase)</returns>
        public static bool IsUpdateVerb(this HttpVerb verb)
        {
            return verb == HttpVerb.Patch || verb == HttpVerb.Put;
        }

        /// <summary>
        /// Returns whether the given status code represents an error response
        /// </summary>
        /// <param name="statusCode">The status code</param>
        /// <returns>Whether the code is greater than or equal to 400</returns>
        public static bool IsError(this HttpStatusCode statusCode)
        {
            return statusCode >= HttpStatusCode.BadRequest;
        }

        /// <summary>
        /// Returns a the value of the given header if it exists, otherwise null
        /// </summary>
        /// <param name="mimePart">Mime part to get header value from</param>
        /// <param name="header">Specific header value to retrieve</param>
        /// <returns>The specified header value or null if it didnt exist</returns>
        public static string GetHeaderValueIfExists(this IMimePart mimePart, string header)
        {
            ExceptionUtilities.CheckArgumentNotNull(mimePart, "mimePart");

            string headerValue;
            if (!mimePart.Headers.TryGetValue(header, out headerValue))
            {
                headerValue = null;
            }

            return headerValue;
        }

        /// <summary>
        /// Gets the effective verb for the given request, taking verb-tunnelling into account
        /// </summary>
        /// <param name="request">The request to get the verb for</param>
        /// <returns>The effective verb for the request, with verb-tunelling accounted for</returns>
        public static HttpVerb GetEffectiveVerb(this IHttpRequest request)
        {
            ExceptionUtilities.CheckArgumentNotNull(request, "request");

            var verb = request.Verb;
            if (verb == HttpVerb.Post)
            {
                string tunnelled;
                if (request.Headers.TryGetValue(HttpHeaders.HttpMethod, out tunnelled) && !string.IsNullOrEmpty(tunnelled))
                {
                    verb = (HttpVerb)Enum.Parse(typeof(HttpVerb), tunnelled, true);
                }
            }

            return verb;
        }

        /// <summary>
        /// Adds the given sequence of key-value pairs to the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TValue">The type of the values</typeparam>
        /// <param name="dictionary">The dictionary to add to</param>
        /// <param name="values">The key-value pairs to add</param>
        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> values)
        {
            ExceptionUtilities.CheckArgumentNotNull(dictionary, "dictionary");
            ExceptionUtilities.CheckArgumentNotNull(values, "values");
            values.ForEach(v => dictionary.Add(v));
        }

        /// <summary>
        /// Tries to get the 'boundary' portion of the given mime-part's content-type
        /// </summary>
        /// <param name="mimePart">The mime part to get the boundary for</param>
        /// <param name="boundary">The boundary if its value is found</param>
        /// <returns>Whether or not the boundary was found</returns>
        public static bool TryGetMimeBoundary(this IMimePart mimePart, out string boundary)
        {
            ExceptionUtilities.CheckArgumentNotNull(mimePart, "mimePart");
            return mimePart.TryGetContentTypeParameter(HttpHeaders.Boundary, out boundary);
        }

        /// <summary>
        /// Tries to get the 'charset' portion of the given mime-part's content-type
        /// </summary>
        /// <param name="mimePart">The mime part to get the charset for</param>
        /// <param name="charset">The charset if its value is found</param>
        /// <returns>Whether or not the charset was found</returns>
        public static bool TryGetMimeCharset(this IMimePart mimePart, out string charset)
        {
            ExceptionUtilities.CheckArgumentNotNull(mimePart, "mimePart");
            return mimePart.TryGetContentTypeParameter(HttpHeaders.Charset, out charset);
        }

        /// <summary>
        /// Gets the appropriate encoding to use based on the mime-part's headers or returns the default encoding
        /// </summary>
        /// <param name="mimePart">The mime part</param>
        /// <returns>The specific encoding or the default encoding</returns>
        public static Encoding GetEncodingFromHeadersOrDefault(this IMimePart mimePart)
        {
            string charset;
            if (!mimePart.TryGetMimeCharset(out charset))
            {
                charset = null;
            }

            return HttpUtilities.GetEncodingOrDefault(charset);
        }

        /// <summary>
        /// Writes the request's information to the given log
        /// </summary>
        /// <param name="request">The request to write</param>
        /// <param name="logger">The log to write to</param>
        /// <param name="level">The log-level to write at</param>
        public static void WriteToLog(this IHttpRequest request, Logger logger, LogLevel level)
        {
            ExceptionUtilities.CheckArgumentNotNull(request, "request");
            ExceptionUtilities.CheckArgumentNotNull(logger, "logger");
            
            logger.WriteLine(level, request.Verb.ToHttpMethod() + " " + request.GetRequestUri());
            logger.WriteLine(level, string.Join(Environment.NewLine, request.Headers.Select(h => h.Key + ": " + h.Value).ToArray()));
            logger.WriteLine(level, string.Empty);
            var requestBody = request.GetRequestBody();
            if (requestBody != null)
            {
                var encoding = request.GetEncodingFromHeadersOrDefault();
                logger.WriteLine(level, encoding.GetString(requestBody, 0, requestBody.Length));
                logger.WriteLine(level, string.Empty);
            }
        }

        /// <summary>
        /// Writes the response's information to the given log
        /// </summary>
        /// <param name="response">The response to write</param>
        /// <param name="logger">The log to write to</param>
        /// <param name="level">The log-level to write at</param>
        public static void WriteToLog(this HttpResponseData response, Logger logger, LogLevel level)
        {
            ExceptionUtilities.CheckArgumentNotNull(response, "response");
            ExceptionUtilities.CheckArgumentNotNull(logger, "logger");

            logger.WriteLine(level, ((int)response.StatusCode) + " " + response.StatusCode);
            logger.WriteLine(level, string.Join(Environment.NewLine, response.Headers.Select(h => h.Key + ": " + h.Value).ToArray()));
            logger.WriteLine(level, string.Empty);
            if (response.Body != null)
            {
                var encoding = response.GetEncodingFromHeadersOrDefault();
                logger.WriteLine(level, encoding.GetString(response.Body, 0, response.Body.Length));
                logger.WriteLine(level, string.Empty);
            }
        }

        /// <summary>
        /// Tries the get header value ignore header case.
        /// </summary>
        /// <param name="mimePart">The MIME part.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <returns>Return true if found</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Required lower caps")]
        public static bool TryGetHeaderValueIgnoreHeaderCase(this IMimePart mimePart, string parameterName, out string parameterValue)
        {
            bool foundContentType = mimePart.Headers.TryGetValue(parameterName, out parameterValue);

            if (!foundContentType)
            {
                if (!mimePart.Headers.TryGetValue(parameterName.ToLowerInvariant(), out parameterValue))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Tries to get the given parameter from the mime part's content type
        /// </summary>
        /// <param name="mimePart">The mime part</param>
        /// <param name="parameterName">The content-type paramter to look for</param>
        /// <param name="parameterValue">The value of the parameter if it is found</param>
        /// <returns>Whether or not the parameter was found</returns>
        internal static bool TryGetContentTypeParameter(this IMimePart mimePart, string parameterName, out string parameterValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(mimePart, "mimePart");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(parameterName, "parameterName");

            parameterValue = null;

            string contentType;
            if (!mimePart.TryGetHeaderValueIgnoreHeaderCase(HttpHeaders.ContentType, out contentType))
            {
                return false;
            }

            return HttpUtilities.TryGetContentTypeParameter(contentType, parameterName, out parameterValue);
        }
    }
}
