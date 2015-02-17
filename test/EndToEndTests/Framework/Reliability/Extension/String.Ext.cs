//---------------------------------------------------------------------
// <copyright file="String.Ext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Reliability
{
    /// <summary>
    /// String extension
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Append the partial query to the base url
        /// </summary>
        /// <param name="baseurl">The base url</param>
        /// <param name="query">The query</param>
        /// <returns>The combined url</returns>
        public static string AppendUri(this string baseurl, string query)
        {
            return string.Format("{0}/{1}", baseurl.TrimEnd('/'), query.TrimStart('/'));
        }

        /// <summary>
        /// Wrap the message with bracket
        /// </summary>
        /// <param name="msg">The message</param>
        /// <returns>The string</returns>
        public static string ToBracketString(this string msg)
        {
            return string.Format("[{0}]", msg);
        }
    }
}
