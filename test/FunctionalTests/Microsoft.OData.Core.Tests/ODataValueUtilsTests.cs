//---------------------------------------------------------------------
// <copyright file="ODataValueUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataValueUtilsTests
    {
        [Fact]
        public void SingleEnumToODataValue()
        {
            var enumValue = Feature.Feature1.ToODataValue() as ODataEnumValue;
            Assert.Equal("Feature1", enumValue.Value);
        }

        [Fact]
        public void EnumFlagsToODataValue()
        {
            var enumValue = (Feature.Feature1 | Feature.Feature2 | Feature.Feature4).ToODataValue() as ODataEnumValue;
            Assert.Equal("Feature1,Feature2,Feature4", enumValue.Value);
        }

        [Fact]
        public void SingleEnumIntToODataValue()
        {
            var enumValue = ((Feature)1).ToODataValue() as ODataEnumValue;
            Assert.Equal("Feature1", enumValue.Value);
        }

        [Fact]
        public void EnumFlagsIntToODataValue()
        {
            var enumValue = ((Feature)11).ToODataValue() as ODataEnumValue;
            Assert.Equal("Feature1,Feature2,Feature4", enumValue.Value);
        }

        [Flags]
        private enum Feature
        {
            Feature1 = 1,
            Feature2 = 2,
            Feature3 = 4,
            Feature4 = 8,
        }
    }
}
