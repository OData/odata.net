//---------------------------------------------------------------------
// <copyright file="ODataUnresolvedFunctionParameterAliasTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser
{
    public class ODataUnresolvedFunctionParameterAliasTests
    {
        [Fact]
        public void GetValues()
        {
            ODataUnresolvedFunctionParameterAlias alias = new ODataUnresolvedFunctionParameterAlias(
                "alias", HardCodedTestModel.GetPersonAddressProp().Type);

            alias.Alias.Should().Be("alias");
            alias.Type.Should().Be(HardCodedTestModel.GetPersonAddressProp().Type);
        }
    }
}
