//---------------------------------------------------------------------
// <copyright file="EdmElementComparerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Edm.Tests.ExtensionMethods
{
    /// <summary>
    ///Tests EdmElementComparerTests functionalities
    ///</summary>
    public class EdmElementComparerTests
    {
        [Fact]
        public void TestComparingEdmStringTypeReference()
        {
            var stringType = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true);
            var stringTypeCopy = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true);
            Assert.True(stringType.IsEquivalentTo(stringTypeCopy));

            var stringTypeWithNullableFalse = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false);
            var primitiveType = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true);
            Assert.False(stringType.IsEquivalentTo(primitiveType));
            Assert.False(stringType.IsEquivalentTo(stringTypeWithNullableFalse));
        }

        [Fact]
        public void TestComparingEdmDecimalTypeReference()
        {
            var decimalTypeWithPrecisionNull = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), false, null, null);
            var decimalTypeWithPrecisionNotNull = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), false, 1, null);
            Assert.False(decimalTypeWithPrecisionNull.IsEquivalentTo(decimalTypeWithPrecisionNotNull));

            var primitiveType = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true);
            Assert.False(decimalTypeWithPrecisionNull.IsEquivalentTo(primitiveType));
        }

        [Fact]
        public void TestComparingEdmBinaryTypeReference()
        {
            var binaryTypeWithDefaultValue = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), true);
            var binaryType = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), true, false, null);
            Assert.True(binaryType.IsEquivalentTo(binaryTypeWithDefaultValue));

            var primitiveType = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), true);
            Assert.False(binaryType.IsEquivalentTo(primitiveType));
        }

        [Fact]
        public void TestComparingEdmTemporalTypeReference()
        {
            var duration = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration), true);
            var offset = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), true);
            Assert.False(duration.IsEquivalentTo(offset));

            var primitiveType = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration), true);
            Assert.False(duration.IsEquivalentTo(primitiveType));
        }

        [Fact]
        public void TestComparingEdmSpatialTypeReference()
        {
            var geoCollection = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyCollection), true);
            var point = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPoint), true);
            var point2 = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPoint), true);

            Assert.True(point.IsEquivalentTo(point2));
            Assert.False(geoCollection.IsEquivalentTo(point));
        }
    }
}
