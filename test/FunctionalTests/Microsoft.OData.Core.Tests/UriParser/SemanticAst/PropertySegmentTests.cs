//---------------------------------------------------------------------
// <copyright file="PropertySegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    public class PropertySegmentTests
    {
        [Fact]
        public void PropertyCannotBeNull()
        {
            Action createWithNullProperty = () => new PropertySegment(null);
            createWithNullProperty.ShouldThrow<Exception>(Error.ArgumentNull("property").ToString());
        }

        [Fact]
        public void IdentifierSetToPropertyName()
        {
            PropertySegment propertySegment = new PropertySegment(HardCodedTestModel.GetPersonNameProp());
            propertySegment.Identifier.Should().Be(HardCodedTestModel.GetPersonNameProp().Name);
        }
        
        [Fact]
        public void TargetEdmTypeIsPropertyTypeDefinition()
        {
            PropertySegment propertySegment = new PropertySegment(HardCodedTestModel.GetPersonNameProp());
            propertySegment.TargetEdmType.Should().BeSameAs(HardCodedTestModel.GetPersonNameProp().Type.Definition);
        }

        [Fact]
        public void SingleResultIsSetCorrectly()
        {
            PropertySegment propertySegment1 = new PropertySegment(HardCodedTestModel.GetPersonNameProp());
            propertySegment1.SingleResult.Should().BeTrue();
            PropertySegment propertySegment2 = new PropertySegment(HardCodedTestModel.GetPersonGeometryCollectionProp());
            propertySegment2.SingleResult.Should().BeFalse();
        }

        [Fact]
        public void PropertySetCorrectly()
        {
            IEdmStructuralProperty prop = ModelBuildingHelpers.BuildValidPrimitiveProperty();
            PropertySegment segment = new PropertySegment(prop);
            segment.Property.Should().Be(prop);
        }

        [Fact]
        public void EqualityCorrect()
        {
            PropertySegment propertySegment1 = new PropertySegment(HardCodedTestModel.GetPersonNameProp());
            PropertySegment propertySegment2 = new PropertySegment(HardCodedTestModel.GetPersonNameProp());
            propertySegment1.Equals(propertySegment2).Should().BeTrue();
        }

        [Fact]
        public void InequalityCorrect()
        {
            PropertySegment propertySegment1 = new PropertySegment(HardCodedTestModel.GetPersonNameProp());
            PropertySegment propertySegment2 = new PropertySegment(HardCodedTestModel.GetPersonShoeProp());
            BatchSegment batchSegment = BatchSegment.Instance;
            propertySegment1.Equals(propertySegment2).Should().BeFalse();
            propertySegment1.Equals(batchSegment).Should().BeFalse();
        }
    }
}
