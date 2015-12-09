//---------------------------------------------------------------------
// <copyright file="ODataNullValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class ODataNullValueTests
    {
        private ODataNullValue nullValue;

        public ODataNullValueTests()
        {
            this.nullValue = new ODataNullValue();
        }

        [Fact]
        public void IsNullMethodShouldBeTrueForNullValue()
        {
            this.nullValue.IsNullValue.Should().BeTrue();
        }
    }
}
