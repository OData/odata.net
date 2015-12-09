//---------------------------------------------------------------------
// <copyright file="VerificationHelpers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;

namespace Microsoft.OData.Core.Tests.ScenarioTests.UriParser
{
    public class VerificationHelpers
    {
        /// <summary>
        /// Enumerates the segments in a path and calls a corresponding delegate verifier on each segment.
        /// Do not overuse this method: most test cases don't need to over-baseline what the expected segments are.
        /// </summary>
        public static void VerifyPath(ODataPath path, Action<ODataPathSegment>[] segmentVerifiers)
        {
            path.Count().Should().Be(segmentVerifiers.Count());

            var i = 0;
            foreach (var segment in path)
            {
                segmentVerifiers[i++](segment);
            }
        }
    }
}
