//---------------------------------------------------------------------
// <copyright file="HeaderCollection.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using Microsoft.OData;

    /// <summary>
    /// Collection for header name/value pairs which is known to be case insensitive.
    /// </summary>
    internal class HeaderCollection
    {
        /// <summary>
        /// Case-insensitive dictionary for storing the header name/value pairs.
        /// </summary>
        private readonly IDictionary<string, string> headers;

        /// <summary>
        /// Current assembly version.
        /// </summary>
        private static Version assemblyVersion = typeof(HeaderCollection).GetAssembly().GetName().Version;

        /// <summary>
        /// Initializes a new instance of <see cref="HeaderCollection"/>.
        /// </summary>
        /// <param name="headers">The initial set of headers for the collection.</param>
        internal HeaderCollection(IEnumerable<KeyValuePair<string, string>> headers)
            : this()
        {
            Debug.Assert(headers != null, "headers != null");
            this.SetHeaders(headers);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="HeaderCollection"/>.
        /// </summary>
        /// <param name="responseMessage">The response message to pull the headers from.</param>
        internal HeaderCollection(IODataResponseMessage responseMessage)
            : this()
        {
            if (responseMessage != null)
            {
                this.SetHeaders(responseMessage.Headers);
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="HeaderCollection"/>.
        /// </summary>
        /// <param name="headers">The initial set of headers for the collection.</param>
        internal HeaderCollection(WebHeaderCollection headers)
            : this()
        {
            Debug.Assert(headers != null, "headers != null");
            foreach (string name in headers.AllKeys)
            {
                this.SetHeader(name, headers[name]);
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="HeaderCollection"/> which is empty.
        /// </summary>
        internal HeaderCollection()
        {
            this.headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the underlying dictionary the headers are stored in. Should only be used when absolutely necessary for maintaining public API.
        /// </summary>
        internal IDictionary<string, string> UnderlyingDictionary
        {
            get { return this.headers; }
        }

        /// <summary>
        /// Gets the names of all the headers in the collection.
        /// </summary>
        internal IEnumerable<string> HeaderNames
        {
            get { return this.headers.Keys; }
        }

        /// <summary>
        /// Gets the number of headers in the collection.
        /// </summary>
        internal int Count
        {
            get { return this.headers.Count; }
        }

        /// <summary>
        /// Adds default system headers
        /// Currently it sets User-Agent header as default system header
        /// </summary>
        internal void SetDefaultHeaders()
        {
            this.SetUserAgent();
        }

        /// <summary>
        /// Tries to get the value of the header with the given name, if it is in the collection.
        /// </summary>
        /// <param name="headerName">The header name to look for.</param>
        /// <param name="headerValue">The header value, if it was in the collection.</param>
        /// <returns>Whether or not the header was in the collection.</returns>
        internal bool TryGetHeader(string headerName, out string headerValue)
        {
            return this.headers.TryGetValue(headerName, out headerValue);
        }

        /// <summary>
        /// Gets the value of the header, or null if it is not in the collection.
        /// </summary>
        /// <param name="headerName">The header name to look for.</param>
        /// <returns>The header value or null.</returns>
        internal string GetHeader(string headerName)
        {
            string headerValue;
            return this.TryGetHeader(headerName, out headerValue) ? headerValue : null;
        }

        /// <summary>
        /// Sets a header value. Will remove the header from the underlying dictionary if the new value is null.
        /// </summary>
        /// <param name="headerName">The header name to set.</param>
        /// <param name="headerValue">The new value of the header.</param>
        internal void SetHeader(string headerName, string headerValue)
        {
            if (headerValue == null)
            {
                this.headers.Remove(headerName);
            }
            else
            {
                this.headers[headerName] = headerValue;
            }
        }

        /// <summary>
        /// Returns whether or not the collection contains the given header.
        /// </summary>
        /// <param name="headerName">The header name to look for.</param>
        /// <returns>Whether the collection contains the header.</returns>
        internal bool HasHeader(string headerName)
        {
            return this.headers.ContainsKey(headerName);
        }

        /// <summary>
        /// Sets multiple header values at once.
        /// </summary>
        /// <param name="headersToSet">The headers to set.</param>
        internal void SetHeaders(IEnumerable<KeyValuePair<string, string>> headersToSet)
        {
            this.headers.SetRange(headersToSet);
        }

        /// <summary>
        /// Gets an enumeration of the header values in the collection.
        /// </summary>
        /// <returns>An enumeration of the header values in the collection.</returns>
        internal IEnumerable<KeyValuePair<string, string>> AsEnumerable()
        {
            return this.headers.AsEnumerable();
        }

        /// <summary>
        /// Sets the request OData-Version and OData-MaxVersion.
        /// </summary>
        /// <param name="requestVersion">DSV to set the request to.</param>
        /// <param name="maxProtocolVersion">Max protocol version, which MDSV will essentially be set to.</param>
        internal void SetRequestVersion(Version requestVersion, Version maxProtocolVersion)
        {
            if (requestVersion != null)
            {
                if (requestVersion > maxProtocolVersion)
                {
                    string message = Strings.Context_RequestVersionIsBiggerThanProtocolVersion(requestVersion.ToString(), maxProtocolVersion.ToString());
                    throw Error.InvalidOperation(message);
                }

                // if request version is 0.x, then we don't put a DSV header
                // in this case it's up to the server to decide what version this request is
                // (happens for example if Execute was used)
                if (requestVersion.Major > 0)
                {
                    // never decrease the version.
                    // if DSV == null || requestVersion > DSV
                    // FIx this. need to parse it or something
                    Version oldVersion = this.GetODataVersion();
                    if (oldVersion == null || requestVersion > oldVersion)
                    {
                        this.SetHeader(XmlConstants.HttpODataVersion, requestVersion.ToString(Util.ODataVersionFieldCount));
                    }
                }
            }

            // for legacy reasons, we don't set this until we set a DSV
            this.SetHeader(XmlConstants.HttpODataMaxVersion, maxProtocolVersion.ToString(Util.ODataVersionFieldCount));
        }

        /// <summary>
        /// Sets the header if it was previously unset.
        /// </summary>
        /// <param name="headerToSet">The header to set.</param>
        /// <param name="headerValue">The new header value.</param>
        internal void SetHeaderIfUnset(string headerToSet, string headerValue)
        {
            // Don't overwrite the header if it has already been set
            if (this.GetHeader(headerToSet) == null)
            {
                this.SetHeader(headerToSet, headerValue);
            }
        }

        /// <summary>
        /// Sets UserAgent header
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This method is an instance method and calls another instance method for non-silverlight scenearios.")]
        internal void SetUserAgent()
        {
            // Add User-Agent header to every request - Since UserAgent field is not supported in silverlight,
            // doing this non-silverlight stacks only
            this.SetHeader(XmlConstants.HttpUserAgent, string.Format(CultureInfo.InvariantCulture, "Microsoft.OData.Client/{0}.{1}.{2}", assemblyVersion.Major, assemblyVersion.Minor, assemblyVersion.Build));
        }

        /// <summary>
        /// Creates a copy of the current header collection which uses a different dictionary to store headers.
        /// </summary>
        /// <returns>A copy of the current headers.</returns>
        internal HeaderCollection Copy()
        {
            return new HeaderCollection(this.headers);
        }

        /// <summary>
        /// Gets the OData-Version as a Version object if it is encoded as a proper version string with an optional Util.VersionSuffix ending.
        /// </summary>
        /// <returns>The OData-Version header as a Version object.</returns>
        private Version GetODataVersion()
        {
            string rawVersion;
            if (!this.TryGetHeader(XmlConstants.HttpODataVersion, out rawVersion))
            {
                return null;
            }

            return Version.Parse(rawVersion);
        }
    }
}
