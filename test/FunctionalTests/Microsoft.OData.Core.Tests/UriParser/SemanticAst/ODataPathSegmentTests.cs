//---------------------------------------------------------------------
// <copyright file="ODataPathSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class ODataPathSegmentTests
    {
        private class DummySegment : ODataPathSegment
        {
            public DummySegment()
            {
            }

            public DummySegment(DummySegment other)
                : base(other)
            {
            }

            public override IEdmType EdmType
            {
                get { throw new NotImplementedException(); }
            }

            public override T TranslateWith<T>(PathSegmentTranslator<T> translator)
            {
                throw new NotImplementedException();
            }

            public override void HandleWith(PathSegmentHandler handler)
            {
                throw new NotImplementedException();
            }

            internal override bool Equals(ODataPathSegment other)
            {
                throw new NotImplementedException();
            }

            internal bool Equals(DummySegment other)
            {
                return base.Equals(other);
            }
        }

        [Fact]
        public void IdentifierIsSettable()
        {
            DummySegment segment = new DummySegment();
            segment.Identifier = "blah";
            segment.Identifier.Should().Be("blah");
        }

        [Fact]
        public void CopyConstructorAndEquals()
        {
            DummySegment segment = new DummySegment();
            segment.Identifier = "blah";

            DummySegment segment2 = new DummySegment(segment);
            segment2.Identifier.Should().Be("blah");
            segment.Equals(segment).Should().BeTrue();

            segment2.Identifier = "different";
            segment.Equals(segment2).Should().BeFalse();
        }

        [Fact]
        public void SingleResultIsSettable()
        {
            DummySegment segment = new DummySegment();
            segment.SingleResult = true;
            segment.SingleResult.Should().BeTrue();
        }

        [Fact]
        public void TargetEdmEntitySetIsSettable()
        {
            DummySegment segment = new DummySegment();
            segment.TargetEdmNavigationSource = HardCodedTestModel.GetPeopleSet();
            segment.TargetEdmNavigationSource.Should().BeSameAs(HardCodedTestModel.GetPeopleSet());
        }

        [Fact]
        public void TargetEdmTypeIsSettable()
        {
            DummySegment segment = new DummySegment();
            segment.TargetEdmType = HardCodedTestModel.GetPersonType();
            segment.TargetEdmType.Should().BeSameAs(HardCodedTestModel.GetPersonType());
        }

        [Fact]
        public void TargetKindIsSettable()
        {
            DummySegment segment = new DummySegment();
            segment.TargetKind = RequestTargetKind.Batch;
            segment.TargetKind.Should().Be(RequestTargetKind.Batch);
        }
    }
}
