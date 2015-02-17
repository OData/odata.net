//---------------------------------------------------------------------
// <copyright file="Result.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Reliability
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Result class
    /// </summary>
    public abstract class Result : IEnumerable<Entry>
    {
        /// <summary>
        /// Initializes a new instance of the Result class
        /// </summary>
        /// <param name="response">The response</param>
        protected Result(Response response)
        {
            Response = response;
            this.Entries = new List<Entry>();
            this.Links = new List<Link>();
        }

        /// <summary>
        /// Gets the response
        /// </summary>
        public Response Response { get; private set; }

        /// <summary>
        /// Gets the entries
        /// </summary>
        public List<Entry> Entries { get; private set; }

        /// <summary>
        /// Gets the count
        /// </summary>
        public int Count
        {
            get { return this.Entries.Count; }
        }

        /// <summary>
        /// Gets or sets the inline count
        /// </summary>
        public long InLineCount { get; protected set; }

        /// <summary>
        /// Gets the links
        /// </summary>
        public List<Link> Links { get; private set; }

        /// <summary>
        /// Gets the next link
        /// </summary>
        public string NextLink
        {
            get
            {
                var link = this.Links.GetByRel("next");
                return link == null ? null : link.Uri;
            }
        }

        /// <summary>
        /// Gets the ith entry
        /// </summary>
        /// <param name="i">the index</param>
        /// <returns>the entry</returns>
        public Entry this[int i]
        {
            get { return this.Entries[i]; }
        }

        /// <summary>
        /// Parse the response
        /// </summary>
        /// <param name="response">The response</param>
        /// <returns>The result</returns>
        public static Result Parse(Response response)
        {
            switch (response.ContentType)
            {
                case HttpContentType.Atom:
                    return new AtomResult(response);
                case HttpContentType.Json:
                case HttpContentType.JsonVerbose:
                    return new JsonResult(response);
                case HttpContentType.JsonLight:
                    return new JsonLightResult(response);
                default:
                    throw new NotSupportedException(response.ContentType.ToString());
            }
        }

        /// <summary>
        /// Get the enumerator
        /// </summary>
        /// <returns>Returns enumerator</returns>
        public IEnumerator<Entry> GetEnumerator()
        {
            return this.Entries.GetEnumerator();
        }

        /// <summary>
        /// Get the enumerator
        /// </summary>
        /// <returns>Returns enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
