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
        private EdmModel GetModel()
        {
            EdmModel model = new EdmModel();
            EdmEntityType edmEntityType = new EdmEntityType("NS", "Person");
            edmEntityType.AddKeys(edmEntityType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.String));
            edmEntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "Dogs", TargetMultiplicity = EdmMultiplicity.ZeroOrOne, Target = edmEntityType });
            model.AddElement(edmEntityType);
            EdmEntityContainer container = new EdmEntityContainer("NS", "EntityContainer");
            model.AddElement(container);
            container.AddEntitySet("People", edmEntityType);

            return model;
        }

        // TODO: Support relative context uri and resolving other relative uris
        [Fact]
        public void ParseRelativeContextUrlShouldThrowException()
        {
            string relativeUrl = "$metadata#R";
            Action parseContextUri = () => ODataJsonLightContextUriParser.Parse(new EdmModel(), relativeUrl, ODataPayloadKind.Unsupported, null, true);
            parseContextUri.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightContextUriParser_TopLevelContextUrlShouldBeAbsolute(relativeUrl));
        }

        [Fact]
        public void ParseContextUrlWithEscapedSpecailMeaningCharactersShouldSucceed()
        {
            string urlWithUnescapedSpecialMeaningCharacters = "https://www.example.com/api/$metadata#People('i%3A0%23.f%7Cmembership%7Cexample%40example.org')/Dogs";
            Action parseContextUri = () => ODataJsonLightContextUriParser.Parse(GetModel(), urlWithUnescapedSpecialMeaningCharacters, ODataPayloadKind.Unsupported, null, true);
            parseContextUri.ShouldNotThrow();
        }
    }
}
