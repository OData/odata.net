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

        [Fact]
        public void ParseRelativeContextUrlWithBaseUrl()
        {
            const bool needParseSegment = true;
            const bool throwIfMetadataConflict = true;

            var model = new EdmModel();
            var entityType = new EdmEntityType("Sample", "R");
            model.AddElement(entityType);
            string relativeUrl1 = "$metadata#Sample.R";
            string relativeUrl2 = "/SampleService/$metadata#Sample.R";
            var parseResult = ODataJsonLightContextUriParser.Parse(model, relativeUrl1, ODataPayloadKind.Unsupported, null, needParseSegment,
                throwIfMetadataConflict, new Uri("http://service/SampleService/EntitySet"));
            parseResult.ContextUri.OriginalString.Should().Be("http://service/SampleService/$metadata#Sample.R");

            parseResult = ODataJsonLightContextUriParser.Parse(model, relativeUrl2, ODataPayloadKind.Unsupported, null, needParseSegment,
                throwIfMetadataConflict, new Uri("http://service/SampleService/EntitySet"));
            parseResult.ContextUri.OriginalString.Should().Be("http://service/SampleService/$metadata#Sample.R");
        }

        [Fact]
        public void ParseRelativeContextUrlWithoutBaseUriShouldThrowException()
        {
            string relativeUrl = "$metadata#R";
            Action parseContextUri = () => ODataJsonLightContextUriParser.Parse(new EdmModel(), relativeUrl, ODataPayloadKind.Unsupported, null, true);
            parseContextUri.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightContextUriParser_TopLevelContextUrlIsInvalid(relativeUrl));
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
