//---------------------------------------------------------------------
// <copyright file="ODataPropertySerializationInfoAnnotationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataPropertySerializationInfoAnnotationTests
    {
        [Fact]
        public void PropertyKindShouldBeUnspecifiedOnCreation()
        {
            var serializationInfo = new ODataPropertySerializationInfo();
            Assert.Equal(ODataPropertyKind.Unspecified, serializationInfo.PropertyKind);
        }
    }
}
