//---------------------------------------------------------------------
// <copyright file="JsonLightContentNegotiationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Text;
using FluentAssertions;
using FluentAssertions.Primitives;
using Xunit;
using ErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests
{
    public class MediaTypeUtilsTests
    {
        [Fact]
        public void StreamingAcceptShouldResolveToStreamingResponse()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;odata.streaming=true", ODataVersion.V4).Should().BeJsonLight().And.BeStreaming();
        }

        [Fact]
        public void NonStreamingAcceptShouldResolveToNonStreamingResponse()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;odata.streaming=false", ODataVersion.V4).Should().BeJsonLight().And.NotBeStreaming();
        }

        [Fact]
        public void UnspecifiedAcceptShouldResolveToStreamingResponse()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal", ODataVersion.V4).Should().BeJsonLight().And.BeStreaming();
        }

        [Fact]
        public void UnrecognizedStreamingParameterValueForAcceptShouldFail()
        {
            Action acceptUnrecognizedStreamingValue = () => TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;odata.streaming=foo", ODataVersion.V4);
            acceptUnrecognizedStreamingValue
                .ShouldThrow<ODataException>()
                .WithMessage(ErrorStrings.MediaTypeUtils_DidNotFindMatchingMediaType("*", "application/json;odata.metadata=minimal;odata.streaming=foo"), ComparisonMode.Wildcard);
        }

        [Fact]
        public void Ieee754CompatibleAcceptShouldBeResolved()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;IEEE754Compatible=true", ODataVersion.V4).Should().BeJsonLight().And.BeIeee754Compatible();
        }

        [Fact]
        public void NotIeee754CompatibleAcceptShouldResolveToNotCompatible()
        {
            string[] args =
            {
                "application/json;IEEE754Compatible=false", 
                "application/json"
            };
            foreach (var arg in args)
            {
                TestMediaTypeWithFormat.GetResponseTypeFromAccept(arg, ODataVersion.V4).Should().BeJsonLight().And.NotBeIeee754Compatible();
            }
        }

        [Fact]
        public void UnrecognizedIeee754CompatibleParameterValueForAcceptShouldFail()
        {
            Action acceptUnrecognizedIeee754CompatibleValue = () => TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;IEEE754Compatible=none", ODataVersion.V4);
            acceptUnrecognizedIeee754CompatibleValue
                .ShouldThrow<ODataException>()
                .WithMessage(ErrorStrings.MediaTypeUtils_DidNotFindMatchingMediaType("*", "application/json;odata.metadata=minimal;IEEE754Compatible=none"), ComparisonMode.Wildcard);
        }

        [Fact]
        public void UnrecognizedMetadataParameterValueForAcceptShouldFail()
        {
            Action acceptUnrecognizedStreamingValue = () => TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=foometadata;", ODataVersion.V4);
            acceptUnrecognizedStreamingValue
                .ShouldThrow<ODataException>()
                .WithMessage(ErrorStrings.MediaTypeUtils_DidNotFindMatchingMediaType("*", "application/json;odata.metadata=foometadata;"), ComparisonMode.Wildcard);
        }

        [Fact]
        public void UnrecognizedParameterForAcceptShouldFail()
        {
            Action acceptUnrecognizedParameter = () => TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;foo=bar", ODataVersion.V4);
            acceptUnrecognizedParameter
                .ShouldThrow<ODataException>()
                .WithMessage(ErrorStrings.MediaTypeUtils_DidNotFindMatchingMediaType("*", "application/json;odata.metadata=minimal;foo=bar"), ComparisonMode.Wildcard);
        }

        [Fact]
        public void StreamingContentTypeShouldParseCorrectly()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal;odata.streaming=true", ODataVersion.V4).Should().BeJsonLight().And.BeStreaming();
        }

        [Fact]
        public void NonStreamingContentTypeShouldParseCorrectly()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal;odata.streaming=false", ODataVersion.V4).Should().BeJsonLight().And.NotBeStreaming();
        }

        [Fact]
        public void UnspecifiedStreamingInContentTypeShouldMeanNoStreaming()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal;", ODataVersion.V4).Should().BeJsonLight().And.NotBeStreaming();
        }

        [Fact]
        public void UnrecognizedContentTypeStreamingValueShouldMeanNoStreaming()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal;odata.streaming=foo", ODataVersion.V4).Should().BeJsonLight().And.NotBeStreaming();
        }

        [Fact]
        public void UnrecognizedContentTypeParameterShouldMeanNoStreaming()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal;foo=bar", ODataVersion.V4).Should().BeJsonLight().And.NotBeStreaming();
        }

        [Fact]
        public void Ieee754CompatibleShouldParseCorrectly()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal;IEEE754Compatible=true", ODataVersion.V4).Should().BeJsonLight().And.BeIeee754Compatible();
        }

        [Fact]
        public void UnrecognizedContentTypeIeee754CompatibleValueShouldMeanNegative()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal;IEEE754Compatible=none", ODataVersion.V4).Should().BeJsonLight().And.NotBeIeee754Compatible();
        }

        [Fact]
        public void UnspecifiedIeee754CompatibleInContentTypeShouldMeanNegative()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal", ODataVersion.V4).Should().BeJsonLight().And.NotBeIeee754Compatible();
        }

        [Fact]
        public void MetadataAllContentTypeShouldSucceed()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=full", ODataVersion.V4).Should().BeJsonLight().And.SpecifyAllMetadata();
        }

        [Fact]
        public void MetadataNoneContentTypeShouldSucceed()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=none", ODataVersion.V4).Should().BeJsonLight().And.SpecifyNoMetadata();
        }

        [Fact]
        public void DefaultMetadataContentTypeShouldSucceed()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal", ODataVersion.V4).Should().BeJsonLight().And.SpecifyDefaultMetadata();
        }

        [Fact]
        public void MetadataAllAcceptShouldSucceed()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=full", ODataVersion.V4).Should().BeJsonLight().And.SpecifyAllMetadata();
        }

        [Fact]
        public void UnspecifiedAcceptShouldResolveToDefault()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json", ODataVersion.V4).Should().BeJsonLight().And.SpecifyDefaultMetadata();
        }

        [Fact]
        public void MetadataNoneAcceptShouldSucceed()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=none", ODataVersion.V4).Should().BeJsonLight().And.SpecifyNoMetadata();
        }

        [Fact]
        public void DefaultMetadataAcceptShouldSucceed()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal", ODataVersion.V4).Should().BeJsonLight().And.SpecifyDefaultMetadata();
        }

        [Fact]
        public void StreamingShouldBePreferredOverNonStreaming()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;odata.streaming=true,application/json;odata.metadata=minimal;odata.streaming=false", ODataVersion.V4).Should().BeJsonLight().And.BeStreaming();
        }

        [Fact]
        public void UnspecifiedMetadataShouldBePreferedOverAll()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json,application/json;odata.metadata=full", ODataVersion.V4).Should().BeJsonLight().And.SpecifyDefaultMetadata();
        }

        [Fact]
        public void DefaultMetadataShouldBePreferedOverAll()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal,application/json;odata.metadata=full", ODataVersion.V4).Should().BeJsonLight().And.SpecifyDefaultMetadata();
        }

        [Fact]
        public void AllMetadataShouldBePreferedOverNone()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=full,application/json;odata.metadata=none", ODataVersion.V4).Should().BeJsonLight().And.SpecifyAllMetadata();
        }

        [Fact]
        public void UnqualifiedContentTypeInV3ShouldBeJsonLightButUnspecified()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json", ODataVersion.V4).Should().BeUnspecifiedJson().And.HaveExactFormat(ODataFormat.Json);
        }

        [Fact]
        public void QualifiedJsonShouldThrow()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json,application/json;odata.metadata=verbose", ODataVersion.V4).Should().BeJsonLight().And.SpecifyDefaultMetadata();
        }

        [Fact]
        public void StreamingParameterShouldBeCaseInsensitive()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("appLICatIOn/jSOn;OdAtA.sTrEaMinG=TrUe", ODataVersion.V4).Should().BeJsonLight().And.BeStreaming();
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("APpLiCAtIOn/jSoN;oDaTa.stReAMinG=fAlSe", ODataVersion.V4).Should().BeJsonLight().And.NotBeStreaming();
            TestMediaTypeWithFormat.ParseContentType("appLICatIOn/jSOn;OdAtA.sTrEaMinG=TrUe", ODataVersion.V4).Should().BeJsonLight().And.BeStreaming();
            TestMediaTypeWithFormat.ParseContentType("APpLiCAtIOn/jSoN;oDaTa.stReAMinG=fAlSe", ODataVersion.V4).Should().BeJsonLight().And.NotBeStreaming();
        }

        [Fact]
        public void ODataParameterShouldBeCaseInsensitive()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("appLICatIOn/jSOn;oDaTa.MeTaDAtA=MinIMaL", ODataVersion.V4).Should().BeJsonLight().And.SpecifyDefaultMetadata();
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("appLICatIOn/jSOn;oDaTa.MeTaDAta=FuLl", ODataVersion.V4).Should().BeJsonLight().And.SpecifyAllMetadata();
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("appLICatIOn/jSOn;oDaTa.MetADAtA=nONe", ODataVersion.V4).Should().BeJsonLight().And.SpecifyNoMetadata();
            TestMediaTypeWithFormat.ParseContentType("appLICatIOn/jSOn;oDaTa.MeTaDaTa=MinIMaL", ODataVersion.V4).Should().BeJsonLight().And.SpecifyDefaultMetadata();
            TestMediaTypeWithFormat.ParseContentType("appLICatIOn/jSOn;ODATA.MeTaDaTa=FuLl", ODataVersion.V4).Should().BeJsonLight().And.SpecifyAllMetadata();
            TestMediaTypeWithFormat.ParseContentType("appLICatIOn/jSOn;oDaTa.MeTaDaTa=nONe", ODataVersion.V4).Should().BeJsonLight().And.SpecifyNoMetadata();
        }

        [Fact]
        public void Ieee754CompatibleShouldBeCaseInsensitive()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("appLICatIOn/jSOn;ieee754Compatible=TRUE", ODataVersion.V4).Should().BeJsonLight().And.BeIeee754Compatible();
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("appLICatIOn/jSOn;ieee754Compatible=False", ODataVersion.V4).Should().BeJsonLight().And.NotBeIeee754Compatible();
            TestMediaTypeWithFormat.ParseContentType("appLICatIOn/jSOn;ieee754Compatible=TRUE", ODataVersion.V4).Should().BeJsonLight().And.BeIeee754Compatible();
            TestMediaTypeWithFormat.ParseContentType("appLICatIOn/jSOn;ieee754Compatible=False", ODataVersion.V4).Should().BeJsonLight().And.NotBeIeee754Compatible();
        }

        [Fact]
        public void JsonFormatShouldBeJsonLightInV3()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromFormat(ODataFormat.Json, ODataVersion.V4).Should().BeJsonLight();
        }

        [Fact]
        public void JsonLightFormatShouldResolveToMinimalMetadataAndStreaming()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromFormat(ODataFormat.Json, ODataVersion.V4).Should().BeJsonLight().And.SpecifyDefaultMetadata().And.BeStreaming();
        }

        [Fact]
        public void MatchInfoCacheShouldWork()
        {
            var result1 = TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal", ODataVersion.V4);
            var result2 = TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal", ODataVersion.V4);
            var result3 = TestMediaTypeWithFormat.ParseContentType("application/json", ODataVersion.V4);
            var result4 = TestMediaTypeWithFormat.ParseContentType("application/json", ODataVersion.V4, ODataMediaTypeResolver.GetMediaTypeResolver(true));
            var result5 = TestMediaTypeWithFormat.ParseContentType("application/json", ODataVersion.V4, ODataMediaTypeResolver.GetMediaTypeResolver(true));

            result1.Should().BeJsonLight().And.SpecifyDefaultMetadata();
            result2.Should().BeJsonLight().And.SpecifyDefaultMetadata();
            result3.Should().BeUnspecifiedJson();
            result4.Should().BeUnspecifiedJson();
            result5.Should().BeUnspecifiedJson();
        }

        [Fact]
        public void AlterContentTypeForJsonPaddingIfNeededShouldThrowIfAtom()
        {
            const string original = "application/atom+xml";
            Action target = () => MediaTypeUtils.AlterContentTypeForJsonPadding(original);
            target.ShouldThrow<ODataException>().WithMessage(Strings.ODataMessageWriter_JsonPaddingOnInvalidContentType("application/atom+xml"));
        }

        [Fact]
        public void AlterContentTypeForJsonPaddingIfNeededShouldReplaceWithJavaScriptIfAppJson()
        {
            const string original = "application/json";
            var result = MediaTypeUtils.AlterContentTypeForJsonPadding(original);
            result.Should().Be("text/javascript");
        }

        [Fact]
        public void AlterContentTypeForJsonPaddingIfNeededShouldReplaceWithJavaScriptIfTextPlain()
        {
            const string original = "text/plain";
            var result = MediaTypeUtils.AlterContentTypeForJsonPadding(original);
            result.Should().Be("text/javascript");
        }

        [Fact]
        public void AlterContentTypeForJsonPaddingIfNeededShouldKeepParametersWhenReplacing()
        {
            const string original = "application/json;p1=v1;P2=v2";
            var result = MediaTypeUtils.AlterContentTypeForJsonPadding(original);
            result.Should().Be("text/javascript;p1=v1;P2=v2");
        }

        [Fact]
        public void AlterContentTypeForJsonPaddingIfNeededShouldBeCaseInsensitive()
        {
            const string original = "aPplIcAtiOn/JsOn";
            var result = MediaTypeUtils.AlterContentTypeForJsonPadding(original);
            result.Should().Be("text/javascript");
        }

        [Fact]
        public void AlterContentTypeForJsonPaddingIfNeededShouldFailIfAppJsonIsNotAtStart()
        {
            const string original = "tricky/application/json";
            Action target = () => MediaTypeUtils.AlterContentTypeForJsonPadding(original);
            target.ShouldThrow<ODataException>().WithMessage(Strings.ODataMessageWriter_JsonPaddingOnInvalidContentType("tricky/application/json"));
        }
    }

    internal class TestMediaTypeWithFormat
    {
        public ODataMediaType MediaType { get; set; }
        public ODataFormat Format { get; set; }

        internal static TestMediaTypeWithFormat GetResponseTypeFromAccept(string acceptHeader, ODataVersion version)
        {
            return GetResponseType(version, s => s.SetContentType(acceptHeader, Encoding.UTF8.WebName));
        }

        internal static TestMediaTypeWithFormat ParseContentType(string contentType, ODataVersion version, ODataMediaTypeResolver resolver = null)
        {
            ODataMediaType mediaType;
            Encoding encoding;
            ODataPayloadKind payloadKind;
            string batchBoundary;
            var format = MediaTypeUtils.GetFormatFromContentType(contentType, new[] { ODataPayloadKind.Entry }, resolver ?? ODataMediaTypeResolver.DefaultMediaTypeResolver, out mediaType, out encoding, out payloadKind, out batchBoundary);
            mediaType.Should().NotBeNull();
            format.Should().NotBeNull();
            return new TestMediaTypeWithFormat { MediaType = mediaType, Format = format };
        }

        internal static TestMediaTypeWithFormat GetResponseTypeFromFormat(ODataFormat format, ODataVersion version)
        {
            return GetResponseType(version, s => s.SetContentType(format));
        }

        private static TestMediaTypeWithFormat GetResponseType(ODataVersion version, Action<ODataMessageWriterSettings> configureSettings)
        {
            var settings = new ODataMessageWriterSettings { Version = version };
            configureSettings(settings);

            ODataMediaType mediaType;
            Encoding encoding;
            var format = MediaTypeUtils.GetContentTypeFromSettings(settings, ODataPayloadKind.Entry, ODataMediaTypeResolver.DefaultMediaTypeResolver, out mediaType, out encoding);
            mediaType.Should().NotBeNull();
            format.Should().NotBeNull();
            return new TestMediaTypeWithFormat { MediaType = mediaType, Format = format };
        }
    }

    internal class MediaTypeAssertions : ObjectAssertions
    {
        public MediaTypeAssertions(TestMediaTypeWithFormat mediaType)
            : base(mediaType)
        {
        }

        private ODataMediaType MediaType
        {
            get { return this.Subject.As<TestMediaTypeWithFormat>().MediaType; }
        }

        private ODataFormat Format
        {
            get { return this.Subject.As<TestMediaTypeWithFormat>().Format; }
        }

        public AndConstraint<MediaTypeAssertions> BeJsonLight()
        {
            return this.HaveFullTypeName("application/json").And.NotHaveParameterValue("odata.metadata", "verbose").And.HaveExactFormat(ODataFormat.Json);
        }

        public AndConstraint<MediaTypeAssertions> BeUnspecifiedJson()
        {
            return this.HaveFullTypeName("application/json").And.NotHaveParameter("odata.metadata");
        }

        public AndConstraint<MediaTypeAssertions> BeStreaming()
        {
            this.MediaType.HasStreamingSetToTrue().Should().BeTrue();
            return new AndConstraint<MediaTypeAssertions>(this);
        }

        public AndConstraint<MediaTypeAssertions> NotBeStreaming()
        {
            this.MediaType.HasStreamingSetToTrue().Should().BeFalse();
            return new AndConstraint<MediaTypeAssertions>(this);
        }

        public AndConstraint<MediaTypeAssertions> BeIeee754Compatible()
        {
            this.MediaType.HasIeee754CompatibleSetToTrue().Should().BeTrue();
            return new AndConstraint<MediaTypeAssertions>(this);
        }

        public AndConstraint<MediaTypeAssertions> NotBeIeee754Compatible()
        {
            this.MediaType.HasIeee754CompatibleSetToTrue().Should().BeFalse();
            return new AndConstraint<MediaTypeAssertions>(this);
        }

        public AndConstraint<MediaTypeAssertions> SpecifyNoMetadata()
        {
            return this.HaveParameterValue("odata.metadata", "none");
        }

        public AndConstraint<MediaTypeAssertions> SpecifyDefaultMetadata()
        {
            return this.HaveParameterValue("odata.metadata", "minimal");
        }

        public AndConstraint<MediaTypeAssertions> SpecifyAllMetadata()
        {
            return this.HaveParameterValue("odata.metadata", "full");
        }

        public AndConstraint<MediaTypeAssertions> HaveParameterValue(string parameterName, string parameterValue)
        {
            this.MediaType.MediaTypeHasParameterWithValue(parameterName, parameterValue).Should().BeTrue();
            return new AndConstraint<MediaTypeAssertions>(this);
        }

        public AndConstraint<MediaTypeAssertions> NotHaveParameterValue(string parameterName, string parameterValue)
        {
            this.MediaType.MediaTypeHasParameterWithValue(parameterName, parameterValue).Should().BeFalse();
            return new AndConstraint<MediaTypeAssertions>(this);
        }

        public AndConstraint<MediaTypeAssertions> NotHaveParameter(string parameterName)
        {
            if (this.MediaType.Parameters != null)
            {
                this.MediaType.Parameters.Should().NotContain(kvp => kvp.Key == parameterName);
            }

            return new AndConstraint<MediaTypeAssertions>(this);
        }

        public AndConstraint<MediaTypeAssertions> HaveFullTypeName(string fullTypeName)
        {
            this.MediaType.FullTypeName.Should().BeEquivalentTo(fullTypeName);
            return new AndConstraint<MediaTypeAssertions>(this);
        }

        public AndConstraint<MediaTypeAssertions> HaveExactFormat(ODataFormat format)
        {
            this.Format.Should().BeSameAs(format);
            return new AndConstraint<MediaTypeAssertions>(this);
        }
    }

    internal static class MediaTypeAssertionExtensions
    {
        public static MediaTypeAssertions Should(this TestMediaTypeWithFormat mediaType)
        {
            return new MediaTypeAssertions(mediaType);
        }
    }
}