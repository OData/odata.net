//---------------------------------------------------------------------
// <copyright file="InstanceAnnotationWriteTrackerTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class InstanceAnnotationWriteTrackerTests
    {
        [Fact]
        public void InstanceAnnotationWriteTrackerTracksWrites()
        {
            var tracker = new InstanceAnnotationWriteTracker();
            tracker.IsAnnotationWritten("key").Should().BeFalse();
            tracker.MarkAnnotationWritten("key");
            tracker.IsAnnotationWritten("key").Should().BeTrue();
        }

        [Fact]
        public void InstanceAnnotationWriteTrackerIsCaseSensitive()
        {
            var tracker = new InstanceAnnotationWriteTracker();
            string key = "key";

            tracker.MarkAnnotationWritten(key.ToUpper());
            tracker.MarkAnnotationWritten(key.ToLower());

            tracker.IsAnnotationWritten(key.ToLower()).Should().BeTrue();
            tracker.IsAnnotationWritten(key.ToUpper()).Should().BeTrue();
        }
    }
}
