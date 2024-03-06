//---------------------------------------------------------------------
// <copyright file="TypeSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class TypeSegmentTests
    {
        [Fact]
        public void TargetTypeCannotBeNull()
        {
            Action createWithNullTargetType = () => new TypeSegment(null, null);
            Assert.Throws<ArgumentNullException>("actualType", createWithNullTargetType);
        }

        [Fact]
        public void TargetEdmTypeIsTargetType()
        {
            TypeSegment typeSegment = new TypeSegment(HardCodedTestModel.GetPersonType(), null);
            Assert.Same(HardCodedTestModel.GetPersonType(), typeSegment.ExpectedType);
        }

        [Fact]
        public void IdentifierByDefaultSetsActualFullTypeName()
        {
            // Single
            TypeSegment typeSegment = new TypeSegment(HardCodedTestModel.GetPersonType(), null);
            Assert.Equal("Fully.Qualified.Namespace.Person", typeSegment.Identifier);

            // Collection
            typeSegment = new TypeSegment(new EdmCollectionType(new EdmEntityTypeReference(HardCodedTestModel.GetPersonType(), false)), null);
            Assert.Equal("Fully.Qualified.Namespace.Person", typeSegment.Identifier);
        }

        [Fact]
        public void TypeSetCorrectly()
        {
            IEdmType type = ModelBuildingHelpers.BuildValidEntityType();
            TypeSegment segment = new TypeSegment(type, null);
            Assert.Same(type, segment.EdmType);
        }

        [Fact]
        public void SetIsSetCorrectly()
        {
            var set = ModelBuildingHelpers.BuildValidEntitySet();
            IEdmType type = set.EntityType;
            TypeSegment segment = new TypeSegment(type, set);
            Assert.Same(set, segment.NavigationSource);
        }

        [Fact]
        public void TypeMustBeRelatedToSet()
        {
            Action create = () => new TypeSegment(HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetDogsSet());
            create.Throws<ODataException>(Strings.PathParser_TypeMustBeRelatedToSet(HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetDogType(), "TypeSegments"));
        }

        [Fact]
        public void EqualityIsCorrect()
        {
            TypeSegment typeSegment1 = new TypeSegment(HardCodedTestModel.GetPersonType(), null);
            TypeSegment typeSegment2 = new TypeSegment(HardCodedTestModel.GetPersonType(), null);
            Assert.True(typeSegment1.Equals(typeSegment2));
        }

        [Fact]
        public void InequalityIsCorrect()
        {
            TypeSegment typeSegment1 = new TypeSegment(HardCodedTestModel.GetPersonType(), null);
            TypeSegment typeSegment2 = new TypeSegment(HardCodedTestModel.GetDogType(), null);
            BatchSegment batchSegment = BatchSegment.Instance;
            Assert.False(typeSegment1.Equals(typeSegment2));
            Assert.False(typeSegment1.Equals(batchSegment));
        }

        [Fact]
        public void CreateTypeSegmentWithExpectType()
        {
            TypeSegment typeSegment = new TypeSegment(HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetEmployeeType(), null);
            Assert.Same(HardCodedTestModel.GetPersonType(), typeSegment.EdmType);
            Assert.Same(HardCodedTestModel.GetEmployeeType(), typeSegment.ExpectedType);
        }
    }
}
