//---------------------------------------------------------------------
// <copyright file="EdmElementComparerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Edm.TDD.Tests
{
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///Tests EdmElementComparerTests functionalities
    ///</summary>
    [TestClass]
    public class EdmElementComparerTests
    {
        [TestMethod]
        public void TestComparingEdmStringTypeReference()
        {
            var stringType = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true);
            var stringTypeCopy = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true);
            Assert.IsTrue(stringType.IsEquivalentTo(stringTypeCopy));

            var stringTypeWithNullableFalse = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false);
            var primitiveType = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true);
            Assert.IsFalse(stringType.IsEquivalentTo(primitiveType));
            Assert.IsFalse(stringType.IsEquivalentTo(stringTypeWithNullableFalse));
        }

        [TestMethod]
        public void TestComparingEdmDecimalTypeReference()
        {
            var decimalTypeWithPrecisionNull = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), false, null, null);
            var decimalTypeWithPrecisionNotNull = new EdmDecimalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), false, 1, null);
            Assert.IsFalse(decimalTypeWithPrecisionNull.IsEquivalentTo(decimalTypeWithPrecisionNotNull));

            var primitiveType = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true);
            Assert.IsFalse(decimalTypeWithPrecisionNull.IsEquivalentTo(primitiveType));
        }

        [TestMethod]
        public void TestComparingEdmBinaryTypeReference()
        {
            var binaryTypeWithDefaultValue = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), true);
            var binaryType = new EdmBinaryTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), true, false, null);
            Assert.IsTrue(binaryType.IsEquivalentTo(binaryTypeWithDefaultValue));

            var primitiveType = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), true);
            Assert.IsFalse(binaryType.IsEquivalentTo(primitiveType));
        }

        [TestMethod]
        public void TestComparingEdmTemporalTypeReference()
        {
            var duration = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration), true);
            var offset = new EdmTemporalTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), true);
            Assert.IsFalse(duration.IsEquivalentTo(offset));

            var primitiveType = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration), true);
            Assert.IsFalse(duration.IsEquivalentTo(primitiveType));
        }

        [TestMethod]
        public void TestComparingEdmSpatialTypeReference()
        {
            var geoCollection = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyCollection), true);
            var point = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPoint), true);
            var point2 = new EdmSpatialTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPoint), true);

            Assert.IsTrue(point.IsEquivalentTo(point2));
            Assert.IsFalse(geoCollection.IsEquivalentTo(point));
        }
    }
}
