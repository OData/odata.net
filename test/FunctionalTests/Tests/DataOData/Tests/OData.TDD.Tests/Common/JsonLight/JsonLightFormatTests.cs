//---------------------------------------------------------------------
// <copyright file="JsonLightFormatTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Common.JsonLight
{
    using Microsoft.OData.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;

    [TestClass]
    public class JsonLightFormatTests
    {
        [TestMethod]
        public void JsonLightFormatToStringShouldReturnJsonLight()
        {
            ODataFormat.Json.ToString().Should().Be("JsonLight");
        }
    }
}
