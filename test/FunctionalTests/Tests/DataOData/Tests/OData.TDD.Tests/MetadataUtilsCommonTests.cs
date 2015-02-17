//---------------------------------------------------------------------
// <copyright file="MetadataUtilsCommonTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MetadataUtilsCommonTests
    {
        [TestMethod]
        public void ValidateSpecificSpatialTypesAreConvertable()
        {
            Assert.AreEqual(true, MetadataUtilsCommon.CanConvertPrimitiveTypeTo(null, EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyCollection), EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography)));
        }
    }
}
