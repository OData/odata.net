//---------------------------------------------------------------------
// <copyright file="ODataPathSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Core.UriParser.Visitors;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    public class ODataPathSegmentTests
    {
        private class DummySegment : ODataPathSegment
        {
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
        }

        [Fact]
        public void IdentifierIsSettable()
        {
            DummySegment segment = new DummySegment();
            segment.Identifier = "blah";
            segment.Identifier.Should().Be("blah");
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
