//---------------------------------------------------------------------
// <copyright file="CookieCollection.Ext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Reliability
{
    using System.Linq;
    using System.Net;

    /// <summary>
    /// Cookie collection extensions
    /// </summary>
    public static class CookieCollectionExtensions
    {
        /// <summary>
        /// Get http header
        /// </summary>
        /// <param name="cookies">The cookie collection</param>
        /// <returns>To cookie header</returns>
        public static string GetHttpHeader(this CookieCollection cookies)
        {
            return cookies.Cast<Cookie>().Select(c => string.Format("{0}={1};", c.Name, c.Value)).ToDelimitedString(";");
        }
    }
}
