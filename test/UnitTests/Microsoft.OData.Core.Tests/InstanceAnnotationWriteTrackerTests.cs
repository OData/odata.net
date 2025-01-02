//---------------------------------------------------------------------
// <copyright file="InstanceAnnotationWriteTrackerTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Xunit;

namespace Microsoft.OData.Tests
{
    public class InstanceAnnotationWriteTrackerTests
    {
        [Fact]
        public void InstanceAnnotationWriteTrackerTracksWrites()
        {
            var tracker = new InstanceAnnotationWriteTracker();
            Assert.False(tracker.IsAnnotationWritten("key"));
            tracker.MarkAnnotationWritten("key");
            Assert.True(tracker.IsAnnotationWritten("key"));
        }

        [Fact]
        public void InstanceAnnotationWriteTrackerIsCaseSensitive()
        {
            var tracker = new InstanceAnnotationWriteTracker();
            string key = "key";

            tracker.MarkAnnotationWritten(key.ToUpper());
            tracker.MarkAnnotationWritten(key.ToLower());

            Assert.True(tracker.IsAnnotationWritten(key.ToLower()));
            Assert.True(tracker.IsAnnotationWritten(key.ToUpper()));
        }
    }
}
