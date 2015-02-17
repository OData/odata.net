//---------------------------------------------------------------------
// <copyright file="SegmentUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SegmentUnitTests
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

        [TestMethod]
        public void IdentifierIsSettable()
        {
            DummySegment segment = new DummySegment();
            segment.Identifier = "blah";
            segment.Identifier.Should().Be("blah");
        }

        [TestMethod]
        public void SingleResultIsSettable()
        {
            DummySegment segment = new DummySegment();
            segment.SingleResult = true;
            segment.SingleResult.Should().BeTrue();
        }

        [TestMethod]
        public void TargetEdmEntitySetIsSettable()
        {
            DummySegment segment = new DummySegment();
            segment.TargetEdmNavigationSource = HardCodedTestModel.GetPeopleSet();
            segment.TargetEdmNavigationSource.Should().BeSameAs(HardCodedTestModel.GetPeopleSet());
        }

        [TestMethod]
        public void TargetEdmTypeIsSettable()
        {
            DummySegment segment = new DummySegment();
            segment.TargetEdmType = HardCodedTestModel.GetPersonType();
            segment.TargetEdmType.Should().BeSameAs(HardCodedTestModel.GetPersonType());
        }

        [TestMethod]
        public void TargetKindIsSettable()
        {
            DummySegment segment = new DummySegment();
            segment.TargetKind = RequestTargetKind.Batch;
            segment.TargetKind.Should().Be(RequestTargetKind.Batch);
        }
    }
}
