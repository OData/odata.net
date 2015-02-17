//---------------------------------------------------------------------
// <copyright file="TypeSegmentUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TypeSegmentUnitTests
    {
        [TestMethod]
        public void TargetTypeCannotBeNull()
        {
            Action createWithNullTargetType = () => new TypeSegment(null, null);
            createWithNullTargetType.ShouldThrow<Exception>(Error.ArgumentNull("identifier").ToString());
        }

        [TestMethod]
        public void TargetEdmTypeIsTargetType()
        {
            TypeSegment typeSegment = new TypeSegment(HardCodedTestModel.GetPersonType(), null);
            typeSegment.TargetEdmType.Should().BeSameAs(HardCodedTestModel.GetPersonType());
        }

        [TestMethod]
        public void TypeSetCorrectly()
        {
            IEdmType type = ModelBuildingHelpers.BuildValidEntityType();
            TypeSegment segment = new TypeSegment(type, null);
            segment.EdmType.Should().BeSameAs(type);
        }

        [TestMethod]
        public void SetIsSetCorrectly()
        {
            var set = ModelBuildingHelpers.BuildValidEntitySet();
            IEdmType type = set.EntityType();
            TypeSegment segment = new TypeSegment(type, set);
            segment.NavigationSource.Should().BeSameAs(set);
        }

        [TestMethod]
        public void TypeMustBeRelatedToSet()
        {
            Action create = () => new TypeSegment(HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetDogsSet());
            create.ShouldThrow<ODataException>().WithMessage(Strings.PathParser_TypeMustBeRelatedToSet(HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetDogType(), "TypeSegments"));
        }

        [TestMethod]
        public void EqualityIsCorrect()
        {
            TypeSegment typeSegment1 = new TypeSegment(HardCodedTestModel.GetPersonType(), null);
            TypeSegment typeSegment2 = new TypeSegment(HardCodedTestModel.GetPersonType(), null);
            typeSegment1.Equals(typeSegment2).Should().BeTrue();
        }

        [TestMethod]
        public void InequalityIsCorrect()
        {
            TypeSegment typeSegment1 = new TypeSegment(HardCodedTestModel.GetPersonType(), null);
            TypeSegment typeSegment2 = new TypeSegment(HardCodedTestModel.GetDogType(), null);
            BatchSegment batchSegment = BatchSegment.Instance;
            typeSegment1.Equals(typeSegment2).Should().BeFalse();
            typeSegment1.Equals(batchSegment).Should().BeFalse();
        }
    }
}
