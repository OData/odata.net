//---------------------------------------------------------------------
// <copyright file="NavigationPropertySegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    public class NavigationPropertySegmentTests
    {
        [Fact]
        public void NavPropCannotBeNull()
        {
            Action createWithNullNavProp = () => new NavigationPropertySegment(null, null);
            createWithNullNavProp.ShouldThrow<Exception>(Error.ArgumentNull("navigationProperty").ToString());
        }

        [Fact]
        public void IdentifierShouldBeNavPropName()
        {
            NavigationPropertySegment navigationPropertySegment = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            navigationPropertySegment.Identifier.Should().Be(HardCodedTestModel.GetPersonMyDogNavProp().Name);
        }
        
        [Fact]
        public void TargetEdmTypeShouldBeNavPropTypeDefinition()
        {
            NavigationPropertySegment navigationPropertySegment = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            navigationPropertySegment.TargetEdmType.Should().BeSameAs(HardCodedTestModel.GetPersonMyDogNavProp().Type.Definition);
        }

        [Fact]
        public void SingleResultSetCorrectly()
        {
            NavigationPropertySegment navigationPropertySegment1 = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            navigationPropertySegment1.SingleResult.Should().BeTrue();
            NavigationPropertySegment navigationPropertySegment2 = new NavigationPropertySegment(HardCodedTestModel.GetDogMyPeopleNavProp(), null);
            navigationPropertySegment2.SingleResult.Should().BeFalse();
        }

        [Fact]
        public void TargetKindShouldBeResource()
        {
            NavigationPropertySegment navigationPropertySegment = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            navigationPropertySegment.TargetKind.Should().Be(RequestTargetKind.Resource);
        }

        [Fact]
        public void NavPropSetCorrectly()
        {
            NavigationPropertySegment segment = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            segment.NavigationProperty.Should().BeSameAs(HardCodedTestModel.GetPersonMyDogNavProp());
        }

        [Fact]
        public void EqualityIsCorrect()
        {
            NavigationPropertySegment navigationPropertySegment1 = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            NavigationPropertySegment navigationPropertySegment2 = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            navigationPropertySegment1.Equals(navigationPropertySegment2).Should().BeTrue();
        }

        [Fact]
        public void InequalityIsCorrect()
        {
            NavigationPropertySegment navigationPropertySegment1 = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            NavigationPropertySegment navigationPropertySegment2 = new NavigationPropertySegment(HardCodedTestModel.GetDogMyPeopleNavProp(), null);
            CountSegment segment = CountSegment.Instance;
            navigationPropertySegment1.Equals(navigationPropertySegment2).Should().BeFalse();
            navigationPropertySegment1.Equals(segment).Should().BeFalse();
        }
    }
}
