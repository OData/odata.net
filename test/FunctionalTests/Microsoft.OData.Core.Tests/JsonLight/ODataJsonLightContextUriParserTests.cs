﻿//---------------------------------------------------------------------
// <copyright file="ODataJsonLightContextUriParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.JsonLight;
using Microsoft.OData.Edm.Library;
using Xunit;
using ErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.JsonLight
{
    public class ODataJsonLightContextUriParserTests
    {
        private const string ContextUriForNullProperty = "http://service/$metadata#Edm.Null";

        [Fact]
        public void ParseNullPropertyContextUriShouldThrowForPayloadKindsExceptPropertyAndUnsupported()
        {
            foreach (ODataPayloadKind payloadKind in Enum.GetValues(typeof(ODataPayloadKind)))
            {
                if (payloadKind != ODataPayloadKind.Property && payloadKind != ODataPayloadKind.Unsupported)
                {
                    Action parseContextUri = () => ODataJsonLightContextUriParser.Parse(new EdmModel(), ContextUriForNullProperty, payloadKind, ODataReaderBehavior.DefaultBehavior, true);
                    parseContextUri.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightContextUriParser_ContextUriDoesNotMatchExpectedPayloadKind(ContextUriForNullProperty, payloadKind.ToString()));
                }
            }
        }

        [Fact]
        public void ParseNullPropertyContextUriShouldReturnPropertyWhenExpectedPayloadKindIsProperty()
        {
            var parseResult = ODataJsonLightContextUriParser.Parse(new EdmModel(), ContextUriForNullProperty, ODataPayloadKind.Property, ODataReaderBehavior.DefaultBehavior, true);
            parseResult.DetectedPayloadKinds.Single().Should().Be(ODataPayloadKind.Property);
            parseResult.IsNullProperty.Should().BeTrue();
        }

        [Fact]
        public void ParseNullPropertyContextUriShouldReturnPropertyWhenExpectedPayloadKindIsUnsupported()
        {
            var parseResult = ODataJsonLightContextUriParser.Parse(new EdmModel(), ContextUriForNullProperty, ODataPayloadKind.Unsupported, ODataReaderBehavior.DefaultBehavior, true);
            parseResult.DetectedPayloadKinds.Single().Should().Be(ODataPayloadKind.Property);
            parseResult.IsNullProperty.Should().BeTrue();
        }

        [Fact]
        public void ParseRelativeContextUrlWithBaseUrl()
        {
            var model = new EdmModel();
            var entityType = new EdmEntityType("Sample", "R");
            model.AddElement(entityType);
            string relativeUrl1 = "$metadata#Sample.R";
            string relativeUrl2 = "/SampleSerivce/$metadata#Sample.R";
            var parseResult = ODataJsonLightContextUriParser.Parse(model, relativeUrl1, ODataPayloadKind.Unsupported, ODataReaderBehavior.DefaultBehavior, true, new Uri("http://service/SampleSerivce/EntitySet"));
            parseResult.ContextUri.OriginalString.Should().Be("http://service/SampleSerivce/$metadata#Sample.R");
            parseResult = ODataJsonLightContextUriParser.Parse(model, relativeUrl2, ODataPayloadKind.Unsupported, ODataReaderBehavior.DefaultBehavior, true, new Uri("http://service/SampleSerivce/EntitySet"));
            parseResult.ContextUri.OriginalString.Should().Be("http://service/SampleSerivce/$metadata#Sample.R");
        }

        [Fact]
        public void ParseRelativeContextUrlWithoutBaseUriShouldThrowException()
        {
            string relativeUrl = "$metadata#R";
            Action parseContextUri = () => ODataJsonLightContextUriParser.Parse(new EdmModel(), relativeUrl, ODataPayloadKind.Unsupported, ODataReaderBehavior.DefaultBehavior, true);
            parseContextUri.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightContextUriParser_TopLevelContextUrlShouldBeAbsolute(relativeUrl));
        }
    }
}
