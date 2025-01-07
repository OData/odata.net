//---------------------------------------------------------------------
// <copyright file="NavigationPropertyLinkSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class NavigationPropertyLinkSegmentTests
    {
        [Fact]
        public void NavigationPropertyCannotBeNull()
        {
            Action createWithNullNavProp = () => new NavigationPropertyLinkSegment(null, null);
            Assert.Throws<ArgumentNullException>("navigationProperty", createWithNullNavProp);
        }

        [Fact]
        public void IdentifierIsNavPropName()
        {
            NavigationPropertyLinkSegment navigationPropertyLinkSegment = new NavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            Assert.Equal(HardCodedTestModel.GetPersonMyDogNavProp().Name, navigationPropertyLinkSegment.Identifier);
        }

        [Fact]
        public void TargetEdmTypeIsNavPropTypeDefinition()
        {
            NavigationPropertyLinkSegment navigationPropertyLinkSegment = new NavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            Assert.Same(HardCodedTestModel.GetPersonMyDogNavProp().Type.Definition, navigationPropertyLinkSegment.TargetEdmType);
        }

        [Fact]
        public void SingleResultIsSetCorrectly()
        {
            NavigationPropertyLinkSegment navigationPropertyLinkSegment1 = new NavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            Assert.True(navigationPropertyLinkSegment1.SingleResult);
            NavigationPropertyLinkSegment navigationPropertyLinkSegment2 = new NavigationPropertyLinkSegment(HardCodedTestModel.GetDogMyPeopleNavProp(), null);
            Assert.False(navigationPropertyLinkSegment2.SingleResult);
        }

        [Fact]
        public void TargetKindIsResource()
        {
            NavigationPropertyLinkSegment navigationPropertyLinkSegment = new NavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            Assert.Equal(RequestTargetKind.Resource, navigationPropertyLinkSegment.TargetKind);
        }

        [Fact]
        public void NavigationPropertySetCorrectly()
        {
            NavigationPropertyLinkSegment navigationPropertyLinkSegment = new NavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            Assert.Same(HardCodedTestModel.GetPersonMyDogNavProp(), navigationPropertyLinkSegment.NavigationProperty);
        }

        [Fact]
        public void EqualityCorrect()
        {
            NavigationPropertyLinkSegment navigationPropertyLinkSegment1 = new NavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            NavigationPropertyLinkSegment navigationPropertyLinkSegment2 = new NavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            Assert.True(navigationPropertyLinkSegment1.Equals(navigationPropertyLinkSegment2));
        }

        [Fact]
        public void InequalityCorrect()
        {
            NavigationPropertyLinkSegment navigationPropertyLinkSegment1 = new NavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            NavigationPropertyLinkSegment navigationPropertyLinkSegment3 = new NavigationPropertyLinkSegment(HardCodedTestModel.GetDogMyPeopleNavProp(), null);
            BatchSegment segment3 = BatchSegment.Instance;
            Assert.False(navigationPropertyLinkSegment1.Equals(navigationPropertyLinkSegment3));
            Assert.False(navigationPropertyLinkSegment1.Equals(segment3));
        }
    }
}
