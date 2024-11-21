//---------------------------------------------------------------------
// <copyright file="ODataJsonContextUriParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Json;
using Microsoft.OData.Edm;
using Xunit;
using ErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.Json
{
    public class ODataJsonContextUriParserTests
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
        public void ParseRelativeContextUrlShouldNotThrowException()
        {
            string relativeUrl = "$metadata#People";
            var parsedContextUrl = ODataJsonContextUriParser.Parse(this.GetModel(), relativeUrl, ODataPayloadKind.Unsupported, null, true, true, new Uri("https://www.example.com/api/"));
            Assert.Equal(new Uri("https://www.example.com/api/$metadata#People"), parsedContextUrl.ContextUri);
        }

        [Fact]
        public void ParseRelativeContextUrlWithoutMetadataShouldNotThrowException()
        {
            string relativeUrl = "People";
            var parsedContextUrl = ODataJsonContextUriParser.Parse(this.GetModel(), relativeUrl, ODataPayloadKind.Unsupported, null, true, true, new Uri("https://www.example.com/api/"));
            Assert.Equal(new Uri("https://www.example.com/api/$metadata#People"), parsedContextUrl.ContextUri);
        }

        [Fact]
        public void ParseRelativeContextUrlWithOnlyDeltaSegmentShouldNotThrowException()
        {
            string relativeUrl = "#$delta";
            IEdmNavigationSource navigationSource = this.GetModel().FindDeclaredEntitySet("People") as IEdmNavigationSource;
            var parsedContextUrl = ODataJsonContextUriParser.Parse(this.GetModel(), relativeUrl, ODataPayloadKind.Unsupported, null, true, true, new Uri("https://www.example.com/api/"), navigationSource);
            Assert.Equal(new Uri("https://www.example.com/api/$metadata#People"), parsedContextUrl.ContextUri);
        }

        [Fact]
        public void ParseContextUrlWithEscapedSpecailMeaningCharactersShouldSucceed()
        {
            string urlWithUnescapedSpecialMeaningCharacters = "https://www.example.com/api/$metadata#People('i%3A0%23.f%7Cmembership%7Cexample%40example.org')/Dogs";
            Action parseContextUri = () => ODataJsonContextUriParser.Parse(GetModel(), urlWithUnescapedSpecialMeaningCharacters, ODataPayloadKind.Unsupported, null, true);
            parseContextUri.DoesNotThrow();
        }
    }
}
