//---------------------------------------------------------------------
// <copyright file="NavigationPropertySegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class NavigationPropertySegmentTests
    {
        [Fact]
        public void NavPropCannotBeNull()
        {
            Action createWithNullNavProp = () => new NavigationPropertySegment(null, null);
            Assert.Throws<ArgumentNullException>("navigationProperty", createWithNullNavProp);
        }

        [Fact]
        public void IdentifierShouldBeNavPropName()
        {
            NavigationPropertySegment navigationPropertySegment = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            Assert.Equal(HardCodedTestModel.GetPersonMyDogNavProp().Name, navigationPropertySegment.Identifier);
        }
        
        [Fact]
        public void TargetEdmTypeShouldBeNavPropTypeDefinition()
        {
            NavigationPropertySegment navigationPropertySegment = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            Assert.Same(HardCodedTestModel.GetPersonMyDogNavProp().Type.Definition, navigationPropertySegment.TargetEdmType);
        }

        [Fact]
        public void SingleResultSetCorrectly()
        {
            NavigationPropertySegment navigationPropertySegment1 = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            Assert.True(navigationPropertySegment1.SingleResult);
            NavigationPropertySegment navigationPropertySegment2 = new NavigationPropertySegment(HardCodedTestModel.GetDogMyPeopleNavProp(), null);
            Assert.False(navigationPropertySegment2.SingleResult);
        }

        [Fact]
        public void TargetKindShouldBeResource()
        {
            NavigationPropertySegment navigationPropertySegment = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            Assert.Equal(RequestTargetKind.Resource, navigationPropertySegment.TargetKind);
        }

        [Fact]
        public void NavPropSetCorrectly()
        {
            NavigationPropertySegment segment = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            Assert.Same(HardCodedTestModel.GetPersonMyDogNavProp(), segment.NavigationProperty);
        }

        [Fact]
        public void EqualityIsCorrect()
        {
            NavigationPropertySegment navigationPropertySegment1 = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            NavigationPropertySegment navigationPropertySegment2 = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            Assert.True(navigationPropertySegment1.Equals(navigationPropertySegment2));
        }

        [Fact]
        public void InequalityIsCorrect()
        {
            NavigationPropertySegment navigationPropertySegment1 = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            NavigationPropertySegment navigationPropertySegment2 = new NavigationPropertySegment(HardCodedTestModel.GetDogMyPeopleNavProp(), null);
            CountSegment segment = CountSegment.Instance;
            Assert.False(navigationPropertySegment1.Equals(navigationPropertySegment2));
            Assert.False(navigationPropertySegment1.Equals(segment));
        }

        [Fact]
        public void NavigationSourceIsCorrect()
        {
            IEdmEntitySet dogsEntitySet = HardCodedTestModel.GetDogsSet();
            NavigationPropertySegment navigationPropertySegment = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), dogsEntitySet);
            Assert.Same(dogsEntitySet, navigationPropertySegment.NavigationSource);
        }
    }
}
