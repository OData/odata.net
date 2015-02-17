//---------------------------------------------------------------------
// <copyright file="OpenPropertySegmentUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class OpenPropertySegmentUnitTests
    {
        [TestMethod]
        public void IdentifierIsPropertyName()
        {
            OpenPropertySegment openPropertySegment = new OpenPropertySegment("stuff");
            openPropertySegment.Identifier.Should().Be("stuff");
        }
        
        [TestMethod]
        public void TargetEdmTypeIsNull()
        {
            OpenPropertySegment openPropertySegment = new OpenPropertySegment("evenmoreawesomestuff");
            openPropertySegment.TargetEdmType.Should().BeNull();
        }

        [TestMethod]
        public void TargetKindIsOpenProperty()
        {
            OpenPropertySegment openPropertySegment = new OpenPropertySegment("anincredibleamountofstuff");
            openPropertySegment.TargetKind.Should().Be(RequestTargetKind.OpenProperty);
        }

        [TestMethod]
        public void SingleResultIsTrue()
        {
            OpenPropertySegment openPropertySegment = new OpenPropertySegment("itblowsmymindhowmuchstuffthereisinhere");
            openPropertySegment.SingleResult.Should().BeTrue();
        }

        [TestMethod]
        public void PropertyNameSetCorrectly()
        {
            OpenPropertySegment openPropertySegment = new OpenPropertySegment("beans");
            openPropertySegment.PropertyName.Should().Be("beans");
        }

        [TestMethod]
        public void EqualityIsCorrect()
        {
            OpenPropertySegment openPropertySegment1 = new OpenPropertySegment("superbeans");
            OpenPropertySegment openPropertySegment2 = new OpenPropertySegment("superbeans");
            openPropertySegment1.Equals(openPropertySegment2).Should().BeTrue();
        }

        [TestMethod]
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
