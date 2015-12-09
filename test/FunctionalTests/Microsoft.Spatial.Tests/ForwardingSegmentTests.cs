//---------------------------------------------------------------------
// <copyright file="ForwardingSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Spatial;
using Microsoft.Spatial.Tests;
using Xunit;

namespace Microsoft.Spatial.Tests
{
    public class ForwardingSegmentTests
    {
        private TracingPipe currentNode;
        private TracingPipe downstreamSegment;
        private ForwardingSegment downstreamForwarding;
        private ForwardingSegment testSubject;
        private readonly Call resetCall = new Call("Geography.Reset");
        private readonly Call endFigure = new Call("Geography.EndFigure");

        public ForwardingSegmentTests()
        {
            this.currentNode = new TracingPipe();
            this.downstreamSegment = new TracingPipe();
            this.downstreamForwarding = new ForwardingSegment(this.downstreamSegment);
            this.testSubject = new ForwardingSegment(this.currentNode);
            this.testSubject.ChainTo(this.downstreamForwarding);
        }

        [Fact]
        public void ForwardVoidCalls()
        {
            this.testSubject.GeographyPipeline.EndFigure();
            this.AssertBothNodesSeeTheseCalls(new Call("Geography.EndFigure"));
        }

        [Fact]
        public void ForwardCallsWithArgs()
        {
            var coordinateSystem = CoordinateSystem.DefaultGeography;
            this.testSubject.GeographyPipeline.SetCoordinateSystem(coordinateSystem);
            this.AssertBothNodesSeeTheseCalls(new Call("Geography.SetCoordinateSystem", coordinateSystem));
        }

        [Fact]
        public void ExceptionInCurentCallsResetForForwardCallsWithArgs()
        {
            this.ExceptionCallsResetForForwardCallsWithArgs(this.currentNode);
        }

        [Fact]
        public void ExceptionInDownstreamCallsResetForForwardCallsWithArgs()
        {
            this.ExceptionCallsResetForForwardCallsWithArgs(this.downstreamSegment);
        }

        [Fact]
        public void ExceptionInCurentCallsResetForForwardCallsWithoutArgs()
        {
            this.ExceptionCallsResetForForwardCallsWithoutArgs(this.currentNode);
        }

        [Fact]
        public void ExceptionInDownstreamCallsResetForForwardCallsWithoutArgs()
        {
            this.ExceptionCallsResetForForwardCallsWithoutArgs(this.downstreamSegment);
        }

        private void ExceptionCallsResetForForwardCallsWithArgs(TracingPipe pipe)
        {
            pipe.CallAdded = (t, c) => this.DoWhenNotCall(resetCall, c, () => { throw new InvalidOperationException(); });
            var coordinateSystem = CoordinateSystem.DefaultGeography;
            Exception ex = SpatialTestUtils.RunCatching(() => this.testSubject.GeographyPipeline.SetCoordinateSystem(coordinateSystem));
            Assert.True(ex.GetType() == typeof(InvalidOperationException), "got the exception we threw");
            Assert.True(ex.StackTrace.Contains("DoWhenNotCall"), "Lost the original stack trace");

            AssertResetCalledLastOnCurrentAndDownstream();
        }

        private void ExceptionCallsResetForForwardCallsWithoutArgs(TracingPipe pipe)
        {
            pipe.CallAdded = (t, c) => this.DoWhenCall(endFigure, c, () => { throw new InvalidOperationException(); });
            var coordinateSystem = CoordinateSystem.DefaultGeography;
            Exception ex = SpatialTestUtils.RunCatching(() => this.testSubject.GeographyPipeline.EndFigure());
            Assert.True(ex.GetType() == typeof(InvalidOperationException), "got the exception we threw");
            Assert.True(ex.StackTrace.Contains("DoWhenCall"), "Lost the original stack trace");

            AssertResetCalledLastOnCurrentAndDownstream();
        }

        private int CountCallsToAMethod(IEnumerable<Call> calls, Call callToCount)
        {
            return calls.Where(c => c.Equals(callToCount)).Count();
        }

        private void AssertResetCalledLastOnCurrentAndDownstream()
        {
            // we should have gotten at a minimum 1 call (usaully more, but at least a reset);
            Assert.True(0 != this.currentNode.Calls.Count, "No calls on currentNode");
            Assert.True(0 != this.downstreamSegment.Calls.Count, "No calls on downstreamSegment");
            
            // the last call should be a reset call
            Assert.Equal(this.resetCall, this.currentNode.Calls[this.currentNode.Calls.Count-1]);
            Assert.Equal(this.resetCall, this.downstreamSegment.Calls[this.downstreamSegment.Calls.Count-1]);
            
            // we should have only gotten 1 reset call each
            Assert.True(1 == this.CountCallsToAMethod(this.currentNode.Calls, resetCall), "go more than one call to reset");
            Assert.True(1 == this.CountCallsToAMethod(this.downstreamSegment.Calls, resetCall), "go more than one call to reset");
        }

        private void DoWhenNotCall(Call expected, Call actual, Action action)
        {
            if (!expected.Equals(actual))
            {
                action();
            }
        }

        private void DoWhenCall(Call expected, Call actual, Action action)
        {
            if (expected.Equals(actual))
            {
                action();
            }
        }

        private void AssertBothNodesSeeTheseCalls(params Call[] expectedCalls)
        {
            SpatialTestUtils.AssertEqualContents(expectedCalls, this.currentNode.Calls);
            SpatialTestUtils.AssertEqualContents(expectedCalls, this.downstreamSegment.Calls);
        }

        private class TracingPipe : SpatialPipeline
        {
            public readonly List<Call> Calls = new List<Call>();
            public Action<TracingPipe, Call> CallAdded = null;
            public override GeographyPipeline GeographyPipeline
            {
                get { return new GPipe(this); }
            }

            public override GeometryPipeline GeometryPipeline
            {
                get { return new MPipe(this); }
            }

            private void Trace(string methodName, params object[] args)
            {
                Call call = new Call(methodName, args);
                this.Calls.Add(call);
                if (CallAdded != null)
                {
                    CallAdded(this, call);
                }
            }

            private class GPipe : GeographyPipeline
            {
                private readonly TracingPipe tracingPipe;

                public GPipe(TracingPipe tracingPipe)
                {
                    this.tracingPipe = tracingPipe;
                }

                public override void BeginGeography(SpatialType type)
                {
                    this.tracingPipe.Trace("Geography.BeginGeography", type);
                }

                public override void BeginFigure(GeographyPosition position)
                {
                    this.tracingPipe.Trace("Geography.BeginFigure", position);
                }

                public override void LineTo(GeographyPosition position)
                {
                    this.tracingPipe.Trace("Geography.LineTo", position);
                }

                public override void EndFigure()
                {
                    this.tracingPipe.Trace("Geography.EndFigure");
                }

                public override void EndGeography()
                {
                    this.tracingPipe.Trace("Geography.EndGeography");
                }

                public override void SetCoordinateSystem(CoordinateSystem coordinateSystem)
                {
                    this.tracingPipe.Trace("Geography.SetCoordinateSystem", coordinateSystem);
                }

                public override void Reset()
                {
                    this.tracingPipe.Trace("Geography.Reset");
                }
            }

            private class MPipe : GeometryPipeline
            {
                private readonly TracingPipe tracingPipe;

                public MPipe(TracingPipe tracingPipe)
                {
                    this.tracingPipe = tracingPipe;
                }

                public override void BeginGeometry(SpatialType type)
                {
                    this.tracingPipe.Trace("Geometry.BeginGeometry", type);
                }

                public override void BeginFigure(GeometryPosition position)
                {
                    this.tracingPipe.Trace("Geometry.BeginFigure", position);
                }

                public override void LineTo(GeometryPosition position)
                {
                    this.tracingPipe.Trace("Geometry.LineTo", position);
                }

                public override void EndFigure()
                {
                    this.tracingPipe.Trace("Geometry.EndFigure");
                }

                public override void EndGeometry()
                {
                    this.tracingPipe.Trace("Geometry.EndGeometry");
                }

                public override void SetCoordinateSystem(CoordinateSystem coordinateSystem)
                {
                    this.tracingPipe.Trace("Geometry.SetCoordinateSystem", coordinateSystem);
                }

                public override void Reset()
                {
                    this.tracingPipe.Trace("Geometry.Reset");
                }
            }
        }

        internal class Call : IEquatable<Call>
        {
            private readonly string methodName;
            private readonly object[] args;

            public Call(string methodName, params object[] args)
            {
                this.methodName = methodName;
                this.args = args;
            }

            public override string ToString()
            {
                return string.Format("{0}({1})", this.methodName, string.Join(", ", this.args));
            }

            public bool Equals(Call other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }
                if (ReferenceEquals(this, other))
                {
                    return true;
                }
                return Equals(other.methodName, this.methodName) && other.args.SequenceEqual(this.args);
            }

            public override bool Equals(object obj)
            {
                return this.Equals(obj as Call);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (this.methodName.GetHashCode() * 397) ^ this.args.GetHashCode();
                }
            }
        }
    }
}