//---------------------------------------------------------------------
// <copyright file="ODataPathSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
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
            Assert.Equal("blah", segment.Identifier);
        }

        [Fact]
        public void CopyConstructorAndEquals()
        {
            DummySegment segment = new DummySegment();
            segment.Identifier = "blah";

            DummySegment segment2 = new DummySegment(segment);
            Assert.Equal("blah", segment2.Identifier);
            Assert.True(segment.Equals(segment));

            segment2.Identifier = "different";
            Assert.False(segment.Equals(segment2));
        }

        [Fact]
        public void SingleResultIsSettable()
        {
            DummySegment segment = new DummySegment();
            segment.SingleResult = true;
            Assert.True(segment.SingleResult);
        }

        [Fact]
        public void TargetEdmEntitySetIsSettable()
        {
            DummySegment segment = new DummySegment();
            segment.TargetEdmNavigationSource = HardCodedTestModel.GetPeopleSet();
            Assert.Same(HardCodedTestModel.GetPeopleSet(), segment.TargetEdmNavigationSource);
        }

        [Fact]
        public void TargetEdmTypeIsSettable()
        {
            DummySegment segment = new DummySegment();
            segment.TargetEdmType = HardCodedTestModel.GetPersonType();
            Assert.Same(HardCodedTestModel.GetPersonType(), segment.TargetEdmType);
        }

        [Fact]
        public void TargetKindIsSettable()
        {
            DummySegment segment = new DummySegment();
            segment.TargetKind = RequestTargetKind.Batch;
            Assert.Equal(RequestTargetKind.Batch, segment.TargetKind);
        }
    }
}
