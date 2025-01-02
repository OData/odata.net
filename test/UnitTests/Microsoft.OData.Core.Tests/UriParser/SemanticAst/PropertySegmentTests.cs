//---------------------------------------------------------------------
// <copyright file="PropertySegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class PropertySegmentTests
    {
        [Fact]
        public void PropertyCannotBeNull()
        {
            Action createWithNullProperty = () => new PropertySegment(null);
            Assert.Throws<ArgumentNullException>("property", createWithNullProperty);
        }

        [Fact]
        public void IdentifierSetToPropertyName()
        {
            PropertySegment propertySegment = new PropertySegment(HardCodedTestModel.GetPersonNameProp());
            Assert.Equal(HardCodedTestModel.GetPersonNameProp().Name, propertySegment.Identifier);
        }
        
        [Fact]
        public void TargetEdmTypeIsPropertyTypeDefinition()
        {
            PropertySegment propertySegment = new PropertySegment(HardCodedTestModel.GetPersonNameProp());
            Assert.Same(HardCodedTestModel.GetPersonNameProp().Type.Definition, propertySegment.TargetEdmType);
        }

        [Fact]
        public void SingleResultIsSetCorrectly()
        {
            PropertySegment propertySegment1 = new PropertySegment(HardCodedTestModel.GetPersonNameProp());
            Assert.True(propertySegment1.SingleResult);
            PropertySegment propertySegment2 = new PropertySegment(HardCodedTestModel.GetPersonGeometryCollectionProp());
            Assert.False(propertySegment2.SingleResult);
        }

        [Fact]
        public void PropertySetCorrectly()
        {
            IEdmStructuralProperty prop = ModelBuildingHelpers.BuildValidPrimitiveProperty();
            PropertySegment segment = new PropertySegment(prop);
            Assert.Same(segment.Property, prop);
        }

        [Fact]
        public void EqualityCorrect()
        {
            PropertySegment propertySegment1 = new PropertySegment(HardCodedTestModel.GetPersonNameProp());
            PropertySegment propertySegment2 = new PropertySegment(HardCodedTestModel.GetPersonNameProp());
            Assert.True(propertySegment1.Equals(propertySegment2));
        }

        [Fact]
        public void InequalityCorrect()
        {
            PropertySegment propertySegment1 = new PropertySegment(HardCodedTestModel.GetPersonNameProp());
            PropertySegment propertySegment2 = new PropertySegment(HardCodedTestModel.GetPersonShoeProp());
            BatchSegment batchSegment = BatchSegment.Instance;
            Assert.False(propertySegment1.Equals(propertySegment2));
            Assert.False(propertySegment1.Equals(batchSegment));
        }
    }
}
