//---------------------------------------------------------------------
// <copyright file="ODataPropertySerializationInfoAnnotationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class ODataPropertySerializationInfoAnnotationTests
    {
        [Fact]
        public void PropertyKindShouldBeUnspecifiedOnCreation()
        {
            var serializationInfo = new ODataPropertySerializationInfo();
            serializationInfo.PropertyKind.Should().Be(ODataPropertyKind.Unspecified);
        }
    }
}
