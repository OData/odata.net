//---------------------------------------------------------------------
// <copyright file="SingletonSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class SingletonSegmentTests
    {
        [Fact]
        public void CtorSingletonSegmentSetsCorrectly()
        {
            IEdmSingleton singleton = HardCodedTestModel.GetBossSingleton();
            SingletonSegment segment = new SingletonSegment(singleton);

            Assert.Equal("Boss", segment.Identifier);
            Assert.Same(singleton, segment.TargetEdmNavigationSource);
            Assert.Same(singleton.EntityType, segment.TargetEdmType);
            Assert.Equal(RequestTargetKind.Resource, segment.TargetKind);
            Assert.True(segment.SingleResult);
        }

        [Fact]
        public void SingletonCannotBeNull()
        {
            Action createWithNullSingleton = () => new SingletonSegment(null);
            Assert.Throws<ArgumentNullException>("singleton", createWithNullSingleton);
        }

        [Fact]
        public void EqualitySingletonSegmentIsCorrect()
        {
            SingletonSegment segment1 = new SingletonSegment(HardCodedTestModel.GetBossSingleton());
            SingletonSegment segment2 = new SingletonSegment(HardCodedTestModel.GetBossSingleton());
            Assert.True(segment1.Equals(segment2));
        }

        [Fact]
        public void NavigationSourceIsCorrect()
        {
            IEdmSingleton singleton = HardCodedTestModel.GetBossSingleton();
            SingletonSegment segment = new SingletonSegment(singleton);
            Assert.Same(singleton, segment.NavigationSource);
        }
    }
}
