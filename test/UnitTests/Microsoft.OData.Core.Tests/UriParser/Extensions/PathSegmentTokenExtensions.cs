//---------------------------------------------------------------------
// <copyright file="PathSegmentTokenExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
using System.Text;
using Microsoft.OData.UriParser;

namespace Microsoft.OData.Tests.UriParser
{
    /// <summary>
    /// Extensions for <see cref="PathSegmentToken"/>.
    /// </summary>
    internal static class PathSegmentTokenExtensions
    {
        /// <summary>
        /// Get the path string for a path segment token.
        /// </summary>
        /// <param name="head">The head of the path</param>
        /// <returns>The path string.</returns>
        public static string ToPathString(this PathSegmentToken head)
        {
            StringBuilder sb = new StringBuilder();
            PathSegmentToken curr = head;
            while (curr != null)
            {
                sb.Append(curr.Identifier);

                NonSystemToken nonSystem = curr as NonSystemToken;
                if (nonSystem != null && nonSystem.NamedValues != null)
                {
                    sb.Append("(");
                    sb.Append(string.Join(",", nonSystem.NamedValues.Select(c => c.Name + "=" + c.Value.Value)));
                    sb.Append(")");
                }

                curr = curr.NextToken;
                if (curr != null)
                {
                    sb.Append("/");
                }
            }

            return sb.ToString();
        }
    }
}