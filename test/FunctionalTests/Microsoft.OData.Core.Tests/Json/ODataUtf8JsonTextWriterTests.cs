//---------------------------------------------------------------------
// <copyright file="ODataUtf8JsonTextWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using static Microsoft.OData.Json.ODataUtf8JsonWriter;
using Xunit;

namespace Microsoft.OData.Core.Tests.Json
{
    public class ODataUtf8JsonTextWriterTests
    {
        [Fact]
        public void Encoding_ThrowsNotImplementedException()
        {
            var stream = new ODataUtf8JsonTextWriter(null);
            Assert.Throws<NotSupportedException>(() => stream.Encoding);
        }
    }
}
