//---------------------------------------------------------------------
// <copyright file="NavigationPropertyLinkSegmentUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NavigationPropertyLinkSegmentUnitTests
    {
        [TestMethod]
        public void NavigationPropertyCannotBeNull()
        {
            Action createWithNullNavProp = () => new NavigationPropertyLinkSegment(null, null);
            createWithNullNavProp.ShouldThrow<Exception>(Error.ArgumentNull("navigationProperty").ToString());
        }

        [TestMethod]
        public void IdentifierIsNavPropName()
        {
            NavigationPropertyLinkSegment navigationPropertyLinkSegment = new NavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            navigationPropertyLinkSegment.Identifier.Should().Be(HardCodedTestModel.GetPersonMyDogNavProp().Name);
        }

        [TestMethod]
        public void TargetEdmTypeIsNavPropTypeDefinition()
        {
            NavigationPropertyLinkSegment navigationPropertyLinkSegment = new NavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            navigationPropertyLinkSegment.TargetEdmType.Should().BeSameAs(HardCodedTestModel.GetPersonMyDogNavProp().Type.Definition);
        }

        [TestMethod]
        public void SingleResultIsSetCorrectly()
        {
            NavigationPropertyLinkSegment navigationPropertyLinkSegment1 = new NavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            navigationPropertyLinkSegment1.SingleResult.Should().BeTrue();
            NavigationPropertyLinkSegment navigationPropertyLinkSegment2 = new NavigationPropertyLinkSegment(HardCodedTestModel.GetDogMyPeopleNavProp(), null);
            navigationPropertyLinkSegment2.SingleResult.Should().BeFalse();
        }

        [TestMethod]
        public void TargetKindIsResource()
        {
            NavigationPropertyLinkSegment navigationPropertyLinkSegment = new NavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            navigationPropertyLinkSegment.TargetKind.Should().Be(RequestTargetKind.Resource);
        }

        [TestMethod]
        public void NavigationPropertySetCorrectly()
        {
            NavigationPropertyLinkSegment navigationPropertyLinkSegment = new NavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            navigationPropertyLinkSegment.NavigationProperty.Should().BeSameAs(HardCodedTestModel.GetPersonMyDogNavProp());
        }

        [TestMethod]
        public void EqualityCorrect()
        {
            NavigationPropertyLinkSegment navigationPropertyLinkSegment1 = new NavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            NavigationPropertyLinkSegment navigationPropertyLinkSegment2 = new NavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            navigationPropertyLinkSegment1.Equals(navigationPropertyLinkSegment2).Should().BeTrue();
        }

        [TestMethod]
        public void InequalityCorrect()
        {
            NavigationPropertyLinkSegment navigationPropertyLinkSegment1 = new NavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            NavigationPropertyLinkSegment navigationPropertyLinkSegment3 = new NavigationPropertyLinkSegment(HardCodedTestModel.GetDogMyPeopleNavProp(), null);
            BatchSegment segment3 = BatchSegment.Instance;
            navigationPropertyLinkSegment1.Equals(navigationPropertyLinkSegment3).Should().BeFalse();
            navigationPropertyLinkSegment1.Equals(segment3).Should().BeFalse();
        }
    }
}
