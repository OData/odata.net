//---------------------------------------------------------------------
// <copyright file="HttpHeaderValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension methods for http header values.
    /// </summary>
    internal sealed class HttpHeaderValue : Dictionary<string, HttpHeaderValueElement>
    {
        /// <summary>
        /// Constructs a new instance of <see cref="HttpHeaderValue"/>.
        /// </summary>
        internal HttpHeaderValue()
            : base(StringComparer.OrdinalIgnoreCase /*Should be case-insensitive for tokens.*/)
        {
        }

        /// <summary>
        /// Returns the HTTP header value string which can be used to set the header on the requst and response messages.
        /// </summary>
        /// <returns>Returns the HTTP header value string which can be used to set the header on the requst and response messages.</returns>
        public override string ToString()
        {
            return this.Count == 0 ? null : String.Join(HttpHeaderValueLexer.ElementSeparator, this.Values.Select(element => element.ToString()).ToArray());
        }
    }
}