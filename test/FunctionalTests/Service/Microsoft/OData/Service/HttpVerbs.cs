//---------------------------------------------------------------------
// <copyright file="HttpVerbs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Enum to represent various http methods
    /// </summary>
    internal enum HttpVerbs
    {
        #region Values.

        /// <summary>Not Initialized.</summary>
        None,

        /// <summary>Represents the GET http method.</summary>
        GET,

        /// <summary>Represents the PUT http method.</summary>
        PUT,

        /// <summary>Represents the POST http method.</summary>
        POST,

        /// <summary>Represents the DELETE http method.</summary>
        DELETE,

        /// <summary>Represents the PATCH http method.</summary>
        PATCH,

        #endregion Values.
    }

    /// <summary>
    /// Utility functions for reasoning about HttpVerbs.
    /// </summary>
    internal static class HttpVerbUtils
    {        
        /// <summary>
        /// List of the HTTP Verbs we use for convenience.
        /// </summary>
        internal static IEnumerable<HttpVerbs> KnownVerbs = new List<HttpVerbs>
                                                            {
                                                                HttpVerbs.PUT, HttpVerbs.POST, HttpVerbs.DELETE, HttpVerbs.GET, HttpVerbs.PATCH
                                                            };

        /// <summary>
        /// Returns true if the given verb could cause a creation, update, or deletion.
        /// </summary>
        /// <param name="verb">Http verb to check.</param>
        /// <returns>True if the given verb could cause a creation, update, or deletion.</returns>
        internal static bool IsChange(this HttpVerbs verb)
        {
            Debug.Assert(verb != HttpVerbs.None, "HttpVerb was 'None'");
            switch (verb)
            {
                case HttpVerbs.DELETE:
                case HttpVerbs.PUT:
                case HttpVerbs.POST:
                case HttpVerbs.PATCH:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true if the given verb is querying data (only gets information).
        /// </summary>
        /// <param name="verb">Http Verb to check.</param>
        /// <returns>True if the given verb is querying data.</returns>
        internal static bool IsQuery(this HttpVerbs verb)
        {
            Debug.Assert(verb != HttpVerbs.None, "HttpVerb was 'None'");
            switch (verb)
            {
                case HttpVerbs.GET:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true if the given verb could cause an update.
        /// </summary>
        /// <param name="verb">Http verb to check.</param>
        /// <returns>True if the given verb could cause an update.</returns>
        internal static bool IsUpdate(this HttpVerbs verb)
        {
            Debug.Assert(verb != HttpVerbs.None, "HttpVerb was 'None'");
            switch (verb)
            {
                case HttpVerbs.PUT:
                case HttpVerbs.PATCH:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true if the given verb could cause a creation.
        /// </summary>
        /// <param name="verb">Http verb to check.</param>
        /// <returns>True if the given verb could cause a creation.</returns>
        internal static bool IsInsert(this HttpVerbs verb)
        {
            Debug.Assert(verb != HttpVerbs.None, "HttpVerb was 'None'");
            return verb == HttpVerbs.POST;
        }

        /// <summary>
        /// Returns true if the given verb could cause a deletion.
        /// </summary>
        /// <param name="verb">Http verb to check.</param>
        /// <returns>True if the given verb could cause a deletion.</returns>
        internal static bool IsDelete(this HttpVerbs verb)
        {
            Debug.Assert(verb != HttpVerbs.None, "HttpVerb was 'None'");
            return verb == HttpVerbs.DELETE;
        }
    }
}