//---------------------------------------------------------------------
// <copyright file="PropertySegmentUnitTests.cs" company="Microsoft">
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
    public class PropertySegmentUnitTests
    {
        [TestMethod]
        public void PropertyCannotBeNull()
        {
            Action createWithNullProperty = () => new PropertySegment(null);
            createWithNullProperty.ShouldThrow<Exception>(Error.ArgumentNull("property").ToString());
        }

        [TestMethod]
        public void IdentifierSetToPropertyName()
        {
            PropertySegment propertySegment = new PropertySegment(HardCodedTestModel.GetPersonNameProp());
            propertySegment.Identifier.Should().Be(HardCodedTestModel.GetPersonNameProp().Name);
        }
        
        [TestMethod]
        public void TargetEdmTypeIsPropertyTypeDefinition()
        {
            PropertySegment propertySegment = new PropertySegment(HardCodedTestModel.GetPersonNameProp());
            propertySegment.TargetEdmType.Should().BeSameAs(HardCodedTestModel.GetPersonNameProp().Type.Definition);
        }

        [TestMethod]
        public void SingleResultIsSetCorrectly()
        {
            PropertySegment propertySegment1 = new PropertySegment(HardCodedTestModel.GetPersonNameProp());
            propertySegment1.SingleResult.Should().BeTrue();
            PropertySegment propertySegment2 = new PropertySegment(HardCodedTestModel.GetPersonGeometryCollectionProp());
            propertySegment2.SingleResult.Should().BeFalse();
        }

        [TestMethod]
        public void PropertySetCorrectly()
        {
            IEdmStructuralProperty prop = ModelBuildingHelpers.BuildValidPrimitiveProperty();
            PropertySegment segment = new PropertySegment(prop);
            segment.Property.Should().Be(prop);
        }

        [TestMethod]
        public void EqualityCorrect()
        {
            PropertySegment propertySegment1 = new PropertySegment(HardCodedTestModel.GetPersonNameProp());
            PropertySegment propertySegment2 = new PropertySegment(HardCodedTestModel.GetPersonNameProp());
            propertySegment1.Equals(propertySegment2).Should().BeTrue();
        }

        [TestMethod]
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
