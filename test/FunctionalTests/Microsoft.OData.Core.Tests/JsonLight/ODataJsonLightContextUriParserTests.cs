//---------------------------------------------------------------------
// <copyright file="ODataJsonLightContextUriParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Edm;
using Xunit;
using ErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.JsonLight
{
    public class ODataJsonLightContextUriParserTests
    {
        // TODO: Support relative context uri and resolving other relative uris
        [Fact]
        public void ParseRelativeContextUrlShouldThrowException()
        {
            string relativeUrl = "$metadata#R";
            Action parseContextUri = () => ODataJsonLightContextUriParser.Parse(new EdmModel(), relativeUrl, ODataPayloadKind.Unsupported, null, true);
            parseContextUri.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightContextUriParser_TopLevelContextUrlShouldBeAbsolute(relativeUrl));
        }
    }
}
