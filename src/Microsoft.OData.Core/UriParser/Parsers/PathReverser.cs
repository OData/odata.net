//---------------------------------------------------------------------
// <copyright file="PathReverser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
{
#else
namespace Microsoft.OData.UriParser
{
#endif
    /// <summary>
    /// Reverse a Path
    /// </summary>
    internal static class PathReverser
    {
        /// <summary>
        /// Reverse the path segments.
        /// </summary>
        /// <param name="head">The head of the path</param>
        /// <returns>The reversed path</returns>
        public static PathSegmentToken Reverse(this PathSegmentToken head)
        {
            // 4/3/2/1 => 1/2/3/4
            PathSegmentToken prev = null;
            PathSegmentToken curr = head;

            while (curr != null)
            {
                PathSegmentToken nextTemp = curr.NextToken;
                curr.NextToken = prev;
                prev = curr;
                curr = nextTemp;
            }

            return prev;
        }
    }
}