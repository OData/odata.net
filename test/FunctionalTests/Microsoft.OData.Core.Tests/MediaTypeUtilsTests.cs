//---------------------------------------------------------------------
// <copyright file="MediaTypeUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Text;
using FluentAssertions;
using FluentAssertions.Primitives;
using Xunit;
using ErrorStrings = Microsoft.OData.Strings;
using System.Linq;

namespace Microsoft.OData.Tests
{
    public class MediaTypeUtilsTests
    {
        [Fact]
        public void StreamingAcceptShouldResolveToStreamingResponse()
        {
            var mediaTypeWithFormat = TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;odata.streaming=true", ODataVersion.V4);
            mediaTypeWithFormat.Should().BeJsonLight().And.BeStreaming().And.SpecifyDefaultMetadata();
            TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "odata.metadata").Should().BeTrue();
            TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "odata.streaming").Should().BeTrue();
        }

        [Fact]
        public void StreamingAcceptShouldResolveToStreamingResponse_Without_Prefix()
        {
            var mediaTypeWithFormat = TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;metadata=minimal;streaming=true", ODataVersion.V4);
            mediaTypeWithFormat.Should().BeJsonLight().And.BeStreaming();
            TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "metadata").Should().BeTrue();
            TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "streaming").Should().BeTrue();
        }

        [Fact]
        public void NonStreamingAcceptShouldResolveToNonStreamingResponse()
        {
            var mediaTypeWithFormat = TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;odata.streaming=false", ODataVersion.V4);
            mediaTypeWithFormat.Should().BeJsonLight().And.NotBeStreaming();
            TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "odata.metadata").Should().BeTrue();
            TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "odata.streaming").Should().BeTrue();
        }

        [Fact]
        public void NonStreamingAcceptShouldResolveToNonStreamingResponse_Without_Prefix()
        {
            var mediaTypeWithFormat = TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;metadata=minimal;streaming=false", ODataVersion.V4);
            mediaTypeWithFormat.Should().BeJsonLight().And.NotBeStreaming();
            TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "metadata").Should().BeTrue();
            TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "streaming").Should().BeTrue();
        }

        [Fact]
        public void UnspecifiedAcceptShouldResolveToStreamingResponse()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal", ODataVersion.V4).Should().BeJsonLight().And.BeStreaming();
        }

        [Fact]
        public void UnspecifiedAcceptShouldResolveToStreamingResponse_Without_Prefix()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;metadata=minimal", ODataVersion.V4).Should().BeJsonLight().And.BeStreaming();
        }

        [Fact]
        public void UnrecognizedStreamingParameterValueForAcceptShouldFail()
        {
            Action acceptUnrecognizedStreamingValue = () => TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;odata.streaming=foo", ODataVersion.V4);
            acceptUnrecognizedStreamingValue
                .ShouldThrow<ODataException>()
#if NETCOREAPP1_0
                .WithMessage(ErrorStrings.MediaTypeUtils_DidNotFindMatchingMediaType("*", "application/json;odata.metadata=minimal;odata.streaming=foo"));
#else
                .WithMessage(ErrorStrings.MediaTypeUtils_DidNotFindMatchingMediaType("*", "application/json;odata.metadata=minimal;odata.streaming=foo"), ComparisonMode.Wildcard);
#endif
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
#if NETCOREAPP1_0
                .WithMessage(ErrorStrings.MediaTypeUtils_DidNotFindMatchingMediaType("*", "application/json;odata.metadata=minimal;IEEE754Compatible=none"));
#else
                .WithMessage(ErrorStrings.MediaTypeUtils_DidNotFindMatchingMediaType("*", "application/json;odata.metadata=minimal;IEEE754Compatible=none"), ComparisonMode.Wildcard);
#endif
        }

        [Fact]
        public void UnrecognizedMetadataParameterValueForAcceptShouldFail()
        {
            Action acceptUnrecognizedStreamingValue = () => TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=foometadata;", ODataVersion.V4);
            acceptUnrecognizedStreamingValue
                .ShouldThrow<ODataException>()
#if NETCOREAPP1_0
                .WithMessage(ErrorStrings.MediaTypeUtils_DidNotFindMatchingMediaType("*", "application/json;odata.metadata=foometadata;"));
#else
                .WithMessage(ErrorStrings.MediaTypeUtils_DidNotFindMatchingMediaType("*", "application/json;odata.metadata=foometadata;"), ComparisonMode.Wildcard);
#endif
        }

        [Fact]
        public void UnrecognizedParameterForAcceptShouldFail()
        {
            Action acceptUnrecognizedParameter = () => TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;foo=bar", ODataVersion.V4);
            acceptUnrecognizedParameter
                .ShouldThrow<ODataException>()
#if NETCOREAPP1_0
                .WithMessage(ErrorStrings.MediaTypeUtils_DidNotFindMatchingMediaType("*", "application/json;odata.metadata=minimal;foo=bar"));
#else
                .WithMessage(ErrorStrings.MediaTypeUtils_DidNotFindMatchingMediaType("*", "application/json;odata.metadata=minimal;foo=bar"), ComparisonMode.Wildcard);
#endif
        }

        [Fact]
        public void StreamingContentTypeShouldParseCorrectly()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal;odata.streaming=true", ODataVersion.V4).Should().BeJsonLight().And.BeStreaming();
        }

        [Fact]
        public void StreamingContentTypeShouldParseCorrectly_Without_Prefix()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;metadata=minimal;streaming=true", ODataVersion.V4).Should().BeJsonLight().And.BeStreaming();
        }

        [Fact]
        public void NonStreamingContentTypeShouldParseCorrectly()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal;odata.streaming=false", ODataVersion.V4).Should().BeJsonLight().And.NotBeStreaming();
        }

        [Fact]
        public void NonStreamingContentTypeShouldParseCorrectly_Without_Prefix()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;metadata=minimal;streaming=false", ODataVersion.V4).Should().BeJsonLight().And.NotBeStreaming();
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
        public void MetadataAllContentTypeShouldSucceed_Without_Prefix()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;metadata=full", ODataVersion.V4).Should().BeJsonLight().And.SpecifyAllMetadata();
        }

        [Fact]
        public void MetadataNoneContentTypeShouldSucceed()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=none", ODataVersion.V4).Should().BeJsonLight().And.SpecifyNoMetadata();
        }

        [Fact]
        public void MetadataNoneContentTypeShouldSucceed_Without_Prefix()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;metadata=none", ODataVersion.V4).Should().BeJsonLight().And.SpecifyNoMetadata();
        }

        [Fact]
        public void DefaultMetadataContentTypeShouldSucceed()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal", ODataVersion.V4).Should().BeJsonLight().And.SpecifyDefaultMetadata();
        }

        [Fact]
        public void DefaultMetadataContentTypeShouldSucceed_Without_Prefix()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;metadata=minimal", ODataVersion.V4).Should().BeJsonLight().And.SpecifyDefaultMetadata();
        }

        [Fact]
        public void MetadataAllAcceptShouldSucceed()
        {
            var mediaTypeWithFormat = TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=full", ODataVersion.V4);
            mediaTypeWithFormat.Should().BeJsonLight().And.SpecifyAllMetadata();
            TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "odata.metadata").Should().BeTrue();
            TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "metadata").Should().BeFalse();
        }

        [Fact]
        public void MetadataAllAcceptShouldSucceed_Without_Prefix()
        {
            var mediaTypeWithFormat = TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;metadata=full", ODataVersion.V4);
            mediaTypeWithFormat.Should().BeJsonLight().And.SpecifyAllMetadata();
            TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "metadata").Should().BeTrue();
            TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "odata.metadata").Should().BeFalse();
        }

        [Fact]
        public void UnspecifiedAcceptShouldResolveToDefault()
        {
            var mediaTypeWithFormat = TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json", ODataVersion.V4);
            mediaTypeWithFormat.Should().BeJsonLight().And.SpecifyDefaultMetadata();
            TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "odata.metadata").Should().BeTrue();
        }

        [Fact]
        public void UnspecifiedAcceptShouldResolveToDefault_401()
        {
            var mediaTypeWithFormat = TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json", ODataVersion.V401);
            mediaTypeWithFormat.Should().BeJsonLight().And.SpecifyDefaultMetadata();
            TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "metadata").Should().BeTrue();
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
            TestMediaTypeWithFormat.ParseContentType("appLICatIOn/jSOn;OdAtA.sTrEaMinG=TrUe", ODataVersion.V4).Should().BeJsonLight().And.BeStreaming();
            TestMediaTypeWithFormat.ParseContentType("APpLiCAtIOn/jSoN;oDaTa.stReAMinG=fAlSe", ODataVersion.V4).Should().BeJsonLight().And.NotBeStreaming();
            TestMediaTypeWithFormat.ParseContentType("appLICatIOn/jSOn;sTrEaMinG=TrUe", ODataVersion.V4).Should().BeJsonLight().And.BeStreaming();
            TestMediaTypeWithFormat.ParseContentType("APpLiCAtIOn/jSoN;stReAMinG=fAlSe", ODataVersion.V4).Should().BeJsonLight().And.NotBeStreaming();

            var mediaTypeWithFormat = TestMediaTypeWithFormat.GetResponseTypeFromAccept("appLICatIOn/jSOn;OdAtA.sTrEaMinG=TrUe", ODataVersion.V4);
            mediaTypeWithFormat.Should().BeJsonLight().And.BeStreaming();
            
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("APpLiCAtIOn/jSoN;oDaTa.stReAMinG=fAlSe", ODataVersion.V4).Should().BeJsonLight().And.NotBeStreaming();

            TestMediaTypeWithFormat.GetResponseTypeFromAccept("appLICatIOn/jSOn;sTrEaMinG=TrUe", ODataVersion.V4).Should().BeJsonLight().And.BeStreaming();
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("APpLiCAtIOn/jSoN;stReAMinG=fAlSe", ODataVersion.V4).Should().BeJsonLight().And.NotBeStreaming();
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
            var result4 = TestMediaTypeWithFormat.ParseContentType("application/json", ODataVersion.V4, ODataMediaTypeResolver.GetMediaTypeResolver(null));

            result1.Should().BeJsonLight().And.SpecifyDefaultMetadata();
            result2.Should().BeJsonLight().And.SpecifyDefaultMetadata();
            result3.Should().BeUnspecifiedJson();
            result4.Should().BeUnspecifiedJson();
        }

        [Fact]
        public void MediaTypeResolutionForJsonBatchShouldWork()
        {
            string[] contentTypes = new string[]
            {
                "application/json",
                "application/json;odata.metadata=minimal",
                "application/json;odata.metadata=minimal;odata.streaming=true",
                "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false"
            };
            foreach (string contentType in contentTypes)
            {
                ODataMediaType mediaType;
                Encoding encoding;
                ODataPayloadKind payloadKind;

                ODataFormat format = MediaTypeUtils.GetFormatFromContentType(contentType, new[] { ODataPayloadKind.Batch },
                    ODataMediaTypeResolver.GetMediaTypeResolver(null),
                    out mediaType, out encoding, out payloadKind);
                mediaType.Should().NotBeNull();
                encoding.Should().NotBeNull();
                payloadKind.Should().Be(ODataPayloadKind.Batch);
                format.Should().Be(ODataFormat.Json);
            }
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
            var format = MediaTypeUtils.GetFormatFromContentType(contentType, new[] { ODataPayloadKind.Resource }, resolver ?? ODataMediaTypeResolver.GetMediaTypeResolver(null), out mediaType, out encoding, out payloadKind);
            mediaType.Should().NotBeNull();
            format.Should().NotBeNull();
            return new TestMediaTypeWithFormat { MediaType = mediaType, Format = format };
        }

        internal static TestMediaTypeWithFormat GetResponseTypeFromFormat(ODataFormat format, ODataVersion version)
        {
            return GetResponseType(version, s => s.SetContentType(format));
        }

        internal static bool MediaTypeContains(ODataMediaType mediaType, string parameterName)
        {
            return mediaType.Parameters != null && mediaType.Parameters.Any(p => String.Compare(p.Key, parameterName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        private static TestMediaTypeWithFormat GetResponseType(ODataVersion version, Action<ODataMessageWriterSettings> configureSettings)
        {
            var settings = new ODataMessageWriterSettings { Version = version };
            configureSettings(settings);

            ODataMediaType mediaType;
            Encoding encoding;
            var format = MediaTypeUtils.GetContentTypeFromSettings(settings, ODataPayloadKind.Resource, ODataMediaTypeResolver.GetMediaTypeResolver(null), out mediaType, out encoding);
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
            return this.HaveFullTypeName("application/json").And.NotHaveParameter("odata.metadata").And.NotHaveParameter("metadata");
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
            return this.HaveParameterValue(new string[] { "odata.metadata", "metadata" }, "none");
        }

        public AndConstraint<MediaTypeAssertions> SpecifyDefaultMetadata()
        {
            return this.HaveParameterValue(new string[] { "odata.metadata", "metadata" }, "minimal");
        }

        public AndConstraint<MediaTypeAssertions> SpecifyAllMetadata()
        {
            return this.HaveParameterValue(new string[] { "odata.metadata", "metadata" }, "full");
        }

        public AndConstraint<MediaTypeAssertions> HaveParameterValue(string parameterName, string parameterValue)
        {
            this.MediaType.MediaTypeHasParameterWithValue(parameterName, parameterValue).Should().BeTrue();
            return new AndConstraint<MediaTypeAssertions>(this);
        }

        public AndConstraint<MediaTypeAssertions> HaveParameterValue(string[] parameterNames, string parameterValue)
        {
            parameterNames.Any(p => this.MediaType.MediaTypeHasParameterWithValue(p, parameterValue)).Should().BeTrue();
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