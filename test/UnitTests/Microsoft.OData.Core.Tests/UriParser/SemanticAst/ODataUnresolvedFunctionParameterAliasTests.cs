//---------------------------------------------------------------------
// <copyright file="ODataUnresolvedFunctionParameterAliasTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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

            Assert.Equal("alias", alias.Alias);
            Assert.Same(alias.Type, HardCodedTestModel.GetPersonAddressProp().Type);
        }
    }
}
