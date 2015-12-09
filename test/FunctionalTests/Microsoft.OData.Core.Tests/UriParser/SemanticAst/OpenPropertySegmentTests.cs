//---------------------------------------------------------------------
// <copyright file="OpenPropertySegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    public class OpenPropertySegmentTests
    {
        [Fact]
        public void IdentifierIsPropertyName()
        {
            OpenPropertySegment openPropertySegment = new OpenPropertySegment("stuff");
            openPropertySegment.Identifier.Should().Be("stuff");
        }
        
        [Fact]
        public void TargetEdmTypeIsNull()
        {
            OpenPropertySegment openPropertySegment = new OpenPropertySegment("evenmoreawesomestuff");
            openPropertySegment.TargetEdmType.Should().BeNull();
        }

        [Fact]
        public void TargetKindIsOpenProperty()
        {
            OpenPropertySegment openPropertySegment = new OpenPropertySegment("anincredibleamountofstuff");
            openPropertySegment.TargetKind.Should().Be(RequestTargetKind.OpenProperty);
        }

        [Fact]
        public void SingleResultIsTrue()
        {
            OpenPropertySegment openPropertySegment = new OpenPropertySegment("itblowsmymindhowmuchstuffthereisinhere");
            openPropertySegment.SingleResult.Should().BeTrue();
        }

        [Fact]
        public void PropertyNameSetCorrectly()
        {
            OpenPropertySegment openPropertySegment = new OpenPropertySegment("beans");
            openPropertySegment.PropertyName.Should().Be("beans");
        }

        [Fact]
        public void EqualityIsCorrect()
        {
            OpenPropertySegment openPropertySegment1 = new OpenPropertySegment("superbeans");
            OpenPropertySegment openPropertySegment2 = new OpenPropertySegment("superbeans");
            openPropertySegment1.Equals(openPropertySegment2).Should().BeTrue();
        }

        [Fact]
        public void InequalityIsCorrect()
        {
            OpenPropertySegment openPropertySegment1 = new OpenPropertySegment("superbeans");
            OpenPropertySegment openPropertySegment2 = new OpenPropertySegment("incredibeans");
            BatchSegment segment = BatchSegment.Instance;
            openPropertySegment1.Equals(openPropertySegment2).Should().BeFalse();
            openPropertySegment2.Equals(segment).Should().BeFalse();
        }
    }
}
