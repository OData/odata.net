//---------------------------------------------------------------------
// <copyright file="NavigationPropertySegmentUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NavigationPropertySegmentUnitTests
    {
        [TestMethod]
        public void NavPropCannotBeNull()
        {
            Action createWithNullNavProp = () => new NavigationPropertySegment(null, null);
            createWithNullNavProp.ShouldThrow<Exception>(Error.ArgumentNull("navigationProperty").ToString());
        }

        [TestMethod]
        public void IdentifierShouldBeNavPropName()
        {
            NavigationPropertySegment navigationPropertySegment = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            navigationPropertySegment.Identifier.Should().Be(HardCodedTestModel.GetPersonMyDogNavProp().Name);
        }
        
        [TestMethod]
        public void TargetEdmTypeShouldBeNavPropTypeDefinition()
        {
            NavigationPropertySegment navigationPropertySegment = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            navigationPropertySegment.TargetEdmType.Should().BeSameAs(HardCodedTestModel.GetPersonMyDogNavProp().Type.Definition);
        }

        [TestMethod]
        public void SingleResultSetCorrectly()
        {
            NavigationPropertySegment navigationPropertySegment1 = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            navigationPropertySegment1.SingleResult.Should().BeTrue();
            NavigationPropertySegment navigationPropertySegment2 = new NavigationPropertySegment(HardCodedTestModel.GetDogMyPeopleNavProp(), null);
            navigationPropertySegment2.SingleResult.Should().BeFalse();
        }

        [TestMethod]
        public void TargetKindShouldBeResource()
        {
            NavigationPropertySegment navigationPropertySegment = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            navigationPropertySegment.TargetKind.Should().Be(RequestTargetKind.Resource);
        }

        [TestMethod]
        public void NavPropSetCorrectly()
        {
            NavigationPropertySegment segment = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            segment.NavigationProperty.Should().BeSameAs(HardCodedTestModel.GetPersonMyDogNavProp());
        }

        [TestMethod]
        public void EqualityIsCorrect()
        {
            NavigationPropertySegment navigationPropertySegment1 = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            NavigationPropertySegment navigationPropertySegment2 = new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null);
            navigationPropertySegment1.Equals(navigationPropertySegment2).Should().BeTrue();
        }

        [TestMethod]
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
