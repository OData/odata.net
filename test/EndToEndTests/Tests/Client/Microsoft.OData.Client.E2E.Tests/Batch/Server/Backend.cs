//-----------------------------------------------------------------------------
// <copyright file="BanksController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd;

namespace Microsoft.OData.Client.E2E.Tests.Batch.Server
{
    internal class Backend
    {
        public static CommonEndToEndDataSource DataSource { get; } = CommonEndToEndDataSource.CreateInstance();

        public static bool TryGetKeyFromUri(Uri link, out int key)
        {
            key = default;

            var segments = link.Segments;
            if (segments.Length > 1)
            {
                var lastSegment = segments[^1].TrimEnd('/');
                var lastIndexOfLeftParen = lastSegment.LastIndexOf('(');
                var lastIndexOfRightParen = lastSegment.LastIndexOf(')');
                if (lastIndexOfLeftParen > 0 && lastIndexOfRightParen > lastIndexOfLeftParen)
                {
                    var keyStr = lastSegment.Substring(lastIndexOfLeftParen + 1, lastIndexOfRightParen - lastIndexOfLeftParen - 1);
                    return int.TryParse(keyStr, out key);
                }
            }

            return false;
        }
    }
}
