//---------------------------------------------------------------------
// <copyright file="MetadataUtilsCommonTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Metadata;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.Metadata
{
    public class MetadataUtilsCommonTests
    {
        [Fact]
        public void ValidateSpecificSpatialTypesAreConvertable()
        {
            Assert.True(MetadataUtilsCommon.CanConvertPrimitiveTypeTo(null, EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyCollection), EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography)));
        }
    }
}
