//---------------------------------------------------------------------
// <copyright file="ODataNullValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataNullValueTests
    {
        private ODataNullValue nullValue;

        public ODataNullValueTests()
        {
            this.nullValue = ODataNullValue.Instance;
        }

        [Fact]
        public void IsNullMethodShouldBeTrueForNullValue()
        {
            Assert.True(this.nullValue.IsNullValue);
        }
    }
}
