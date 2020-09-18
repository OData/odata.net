//---------------------------------------------------------------------
// <copyright file="MediaTypeUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Text;
using Xunit;
using ErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests
{
    public class MediaTypeUtilsTests
    {
        [Fact]
        public void StreamingAcceptShouldResolveToStreamingResponse()
        {
            var mediaTypeWithFormat = TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;odata.streaming=true", ODataVersion.V4);

            mediaTypeWithFormat.BeJsonLight().BeStreaming().HaveDefaultMetadata();

            Assert.True(TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "odata.metadata"));
            Assert.True(TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "odata.streaming"));
        }

        [Fact]
        public void StreamingAcceptShouldResolveToStreamingResponse_Without_Prefix()
        {
            var mediaTypeWithFormat = TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;metadata=minimal;streaming=true", ODataVersion.V4);
            mediaTypeWithFormat.BeJsonLight().BeStreaming();
            Assert.True(TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "metadata"));
            Assert.True(TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "streaming"));
        }

        [Fact]
        public void NonStreamingAcceptShouldResolveToNonStreamingResponse()
        {
            var mediaTypeWithFormat = TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;odata.streaming=false", ODataVersion.V4);
            mediaTypeWithFormat.BeJsonLight().NotBeStreaming();
            Assert.True(TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "odata.metadata"));
            Assert.True(TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "odata.streaming"));
        }

        [Fact]
        public void NonStreamingAcceptShouldResolveToNonStreamingResponse_Without_Prefix()
        {
            var mediaTypeWithFormat = TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;metadata=minimal;streaming=false", ODataVersion.V4);
            mediaTypeWithFormat.BeJsonLight().NotBeStreaming();
            Assert.True(TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "metadata"));
            Assert.True(TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "streaming"));
        }

        [Fact]
        public void UnspecifiedAcceptShouldResolveToStreamingResponse()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal", ODataVersion.V4).BeJsonLight().BeStreaming();
        }

        [Fact]
        public void UnspecifiedAcceptShouldResolveToStreamingResponse_Without_Prefix()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;metadata=minimal", ODataVersion.V4).BeJsonLight().BeStreaming();
        }

        [Fact]
        public void UnrecognizedStreamingParameterValueForAcceptShouldFail()
        {
            Action acceptUnrecognizedStreamingValue = () => TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;odata.streaming=foo", ODataVersion.V4);

            ODataContentTypeException exception = Assert.Throws<ODataContentTypeException>(acceptUnrecognizedStreamingValue);
            Assert.Contains("do not match any of the acceptable MIME types 'application/json;odata.metadata=minimal;odata.streaming=foo'.", exception.Message);
        }

        [Fact]
        public void Ieee754CompatibleAcceptShouldBeResolved()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;IEEE754Compatible=true", ODataVersion.V4).BeJsonLight().BeIeee754Compatible();
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
                TestMediaTypeWithFormat.GetResponseTypeFromAccept(arg, ODataVersion.V4).BeJsonLight().NotBeIeee754Compatible();
            }
        }

        [Fact]
        public void UnrecognizedIeee754CompatibleParameterValueForAcceptShouldFail()
        {
            Action acceptUnrecognizedIeee754CompatibleValue = () => TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;IEEE754Compatible=none", ODataVersion.V4);

            ODataContentTypeException exception = Assert.Throws<ODataContentTypeException>(acceptUnrecognizedIeee754CompatibleValue);
            Assert.Contains("do not match any of the acceptable MIME types 'application/json;odata.metadata=minimal;IEEE754Compatible=none'.", exception.Message);
        }

        [Fact]
        public void UnrecognizedMetadataParameterValueForAcceptShouldFail()
        {
            Action acceptUnrecognizedStreamingValue = () => TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=foometadata;", ODataVersion.V4);

            ODataContentTypeException exception = Assert.Throws<ODataContentTypeException>(acceptUnrecognizedStreamingValue);
            Assert.Contains("do not match any of the acceptable MIME types 'application/json;odata.metadata=foometadata;'.", exception.Message);
        }

        [Fact]
        public void UnrecognizedParameterForAcceptShouldFail()
        {
            Action acceptUnrecognizedParameter = () => TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;foo=bar", ODataVersion.V4);

            ODataContentTypeException exception = Assert.Throws<ODataContentTypeException>(acceptUnrecognizedParameter);
            Assert.Contains("do not match any of the acceptable MIME types 'application/json;odata.metadata=minimal;foo=bar'.", exception.Message);
        }

        [Fact]
        public void StreamingContentTypeShouldParseCorrectly()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal;odata.streaming=true", ODataVersion.V4).BeJsonLight().BeStreaming();
        }

        [Fact]
        public void StreamingContentTypeShouldParseCorrectly_Without_Prefix()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;metadata=minimal;streaming=true", ODataVersion.V4).BeJsonLight().BeStreaming();
        }

        [Fact]
        public void NonStreamingContentTypeShouldParseCorrectly()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal;odata.streaming=false", ODataVersion.V4).BeJsonLight().NotBeStreaming();
        }

        [Fact]
        public void NonStreamingContentTypeShouldParseCorrectly_Without_Prefix()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;metadata=minimal;streaming=false", ODataVersion.V4).BeJsonLight().NotBeStreaming();
        }

        [Fact]
        public void UnspecifiedStreamingInContentTypeShouldMeanNoStreaming()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal;", ODataVersion.V4).BeJsonLight().NotBeStreaming();
        }

        [Fact]
        public void UnrecognizedContentTypeStreamingValueShouldMeanNoStreaming()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal;odata.streaming=foo", ODataVersion.V4).BeJsonLight().NotBeStreaming();
        }

        [Fact]
        public void UnrecognizedContentTypeParameterShouldMeanNoStreaming()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal;foo=bar", ODataVersion.V4).BeJsonLight().NotBeStreaming();
        }

        [Fact]
        public void Ieee754CompatibleShouldParseCorrectly()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal;IEEE754Compatible=true", ODataVersion.V4).BeJsonLight().BeIeee754Compatible();
        }

        [Fact]
        public void UnrecognizedContentTypeIeee754CompatibleValueShouldMeanNegative()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal;IEEE754Compatible=none", ODataVersion.V4).BeJsonLight().NotBeIeee754Compatible();
        }

        [Fact]
        public void UnspecifiedIeee754CompatibleInContentTypeShouldMeanNegative()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal", ODataVersion.V4).BeJsonLight().NotBeIeee754Compatible();
        }

        [Fact]
        public void MetadataAllContentTypeShouldSucceed()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=full", ODataVersion.V4).BeJsonLight().HaveFullMetadata();
        }

        [Fact]
        public void MetadataAllContentTypeShouldSucceed_Without_Prefix()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;metadata=full", ODataVersion.V4).BeJsonLight().HaveFullMetadata();
        }

        [Fact]
        public void MetadataNoneContentTypeShouldSucceed()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=none", ODataVersion.V4).BeJsonLight().HaveNoMetadata();
        }

        [Fact]
        public void MetadataNoneContentTypeShouldSucceed_Without_Prefix()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;metadata=none", ODataVersion.V4).BeJsonLight().HaveNoMetadata();
        }

        [Fact]
        public void DefaultMetadataContentTypeShouldSucceed()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal", ODataVersion.V4).BeJsonLight().HaveDefaultMetadata();
        }

        [Fact]
        public void DefaultMetadataContentTypeShouldSucceed_Without_Prefix()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;metadata=minimal", ODataVersion.V4).BeJsonLight().HaveDefaultMetadata();
        }

        [Fact]
        public void MetadataAllAcceptShouldSucceed()
        {
            var mediaTypeWithFormat = TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=full", ODataVersion.V4);
            mediaTypeWithFormat.BeJsonLight().HaveFullMetadata();
            Assert.True(TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "odata.metadata"));
            Assert.False(TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "metadata"));
        }

        [Fact]
        public void MetadataAllAcceptShouldSucceed_Without_Prefix()
        {
            var mediaTypeWithFormat = TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;metadata=full", ODataVersion.V4);
            mediaTypeWithFormat.BeJsonLight().HaveFullMetadata();
            Assert.True(TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "metadata"));
            Assert.False(TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "odata.metadata"));
        }

        [Fact]
        public void UnspecifiedAcceptShouldResolveToDefault()
        {
            var mediaTypeWithFormat = TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json", ODataVersion.V4);
            mediaTypeWithFormat.BeJsonLight().HaveDefaultMetadata();
            Assert.True(TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "odata.metadata"));
        }

        [Fact]
        public void UnspecifiedAcceptShouldResolveToDefault_401()
        {
            var mediaTypeWithFormat = TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json", ODataVersion.V401);
            mediaTypeWithFormat.BeJsonLight().HaveDefaultMetadata();
            Assert.True(TestMediaTypeWithFormat.MediaTypeContains(mediaTypeWithFormat.MediaType, "metadata"));
        }

        [Fact]
        public void MetadataNoneAcceptShouldSucceed()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=none", ODataVersion.V4).BeJsonLight().HaveNoMetadata();
        }

        [Fact]
        public void DefaultMetadataAcceptShouldSucceed()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal", ODataVersion.V4).BeJsonLight().HaveDefaultMetadata();
        }

        [Fact]
        public void StreamingShouldBePreferredOverNonStreaming()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;odata.streaming=true,application/json;odata.metadata=minimal;odata.streaming=false", ODataVersion.V4).BeJsonLight().BeStreaming();
        }

        [Fact]
        public void UnspecifiedMetadataShouldBePreferedOverAll()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json,application/json;odata.metadata=full", ODataVersion.V4).BeJsonLight().HaveDefaultMetadata();
        }

        [Fact]
        public void DefaultMetadataShouldBePreferedOverAll()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal,application/json;odata.metadata=full", ODataVersion.V4).BeJsonLight().HaveDefaultMetadata();
        }

        [Fact]
        public void AllMetadataShouldBePreferedOverNone()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=full,application/json;odata.metadata=none", ODataVersion.V4).BeJsonLight().HaveFullMetadata();
        }

        [Fact]
        public void UnqualifiedContentTypeInV3ShouldBeJsonLightButUnspecified()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json", ODataVersion.V4).BeUnspecifiedJson().HaveExactFormat(ODataFormat.Json);
        }

        [Fact]
        public void QualifiedJsonShouldThrow()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json,application/json;odata.metadata=verbose", ODataVersion.V4).BeJsonLight().HaveDefaultMetadata();
        }

        [Fact]
        public void StreamingParameterShouldBeCaseInsensitive()
        {
            TestMediaTypeWithFormat.ParseContentType("appLICatIOn/jSOn;OdAtA.sTrEaMinG=TrUe", ODataVersion.V4).BeJsonLight().BeStreaming();
            TestMediaTypeWithFormat.ParseContentType("APpLiCAtIOn/jSoN;oDaTa.stReAMinG=fAlSe", ODataVersion.V4).BeJsonLight().NotBeStreaming();
            TestMediaTypeWithFormat.ParseContentType("appLICatIOn/jSOn;sTrEaMinG=TrUe", ODataVersion.V4).BeJsonLight().BeStreaming();
            TestMediaTypeWithFormat.ParseContentType("APpLiCAtIOn/jSoN;stReAMinG=fAlSe", ODataVersion.V4).BeJsonLight().NotBeStreaming();

            var mediaTypeWithFormat = TestMediaTypeWithFormat.GetResponseTypeFromAccept("appLICatIOn/jSOn;OdAtA.sTrEaMinG=TrUe", ODataVersion.V4);
            mediaTypeWithFormat.BeJsonLight().BeStreaming();
            
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("APpLiCAtIOn/jSoN;oDaTa.stReAMinG=fAlSe", ODataVersion.V4).BeJsonLight().NotBeStreaming();

            TestMediaTypeWithFormat.GetResponseTypeFromAccept("appLICatIOn/jSOn;sTrEaMinG=TrUe", ODataVersion.V4).BeJsonLight().BeStreaming();
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("APpLiCAtIOn/jSoN;stReAMinG=fAlSe", ODataVersion.V4).BeJsonLight().NotBeStreaming();
        }

        [Fact]
        public void ODataParameterShouldBeCaseInsensitive()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("appLICatIOn/jSOn;oDaTa.MeTaDAtA=MinIMaL", ODataVersion.V4).BeJsonLight().HaveDefaultMetadata();
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("appLICatIOn/jSOn;oDaTa.MeTaDAta=FuLl", ODataVersion.V4).BeJsonLight().HaveFullMetadata();
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("appLICatIOn/jSOn;oDaTa.MetADAtA=nONe", ODataVersion.V4).BeJsonLight().HaveNoMetadata();
            TestMediaTypeWithFormat.ParseContentType("appLICatIOn/jSOn;oDaTa.MeTaDaTa=MinIMaL", ODataVersion.V4).BeJsonLight().HaveDefaultMetadata();
            TestMediaTypeWithFormat.ParseContentType("appLICatIOn/jSOn;ODATA.MeTaDaTa=FuLl", ODataVersion.V4).BeJsonLight().HaveFullMetadata();
            TestMediaTypeWithFormat.ParseContentType("appLICatIOn/jSOn;oDaTa.MeTaDaTa=nONe", ODataVersion.V4).BeJsonLight().HaveNoMetadata();
        }

        [Fact]
        public void Ieee754CompatibleShouldBeCaseInsensitive()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("appLICatIOn/jSOn;ieee754Compatible=TRUE", ODataVersion.V4).BeJsonLight().BeIeee754Compatible();
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("appLICatIOn/jSOn;ieee754Compatible=False", ODataVersion.V4).BeJsonLight().NotBeIeee754Compatible();
            TestMediaTypeWithFormat.ParseContentType("appLICatIOn/jSOn;ieee754Compatible=TRUE", ODataVersion.V4).BeJsonLight().BeIeee754Compatible();
            TestMediaTypeWithFormat.ParseContentType("appLICatIOn/jSOn;ieee754Compatible=False", ODataVersion.V4).BeJsonLight().NotBeIeee754Compatible();
        }

        [Fact]
        public void JsonFormatShouldBeJsonLightInV3()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromFormat(ODataFormat.Json, ODataVersion.V4).BeJsonLight();
        }

        [Fact]
        public void JsonLightFormatShouldResolveToMinimalMetadataAndStreaming()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromFormat(ODataFormat.Json, ODataVersion.V4).BeJsonLight().HaveDefaultMetadata().BeStreaming();
        }

        [Fact]
        public void MatchInfoCacheShouldWork()
        {
            var result1 = TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal", ODataVersion.V4);
            var result2 = TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal", ODataVersion.V4);
            var result3 = TestMediaTypeWithFormat.ParseContentType("application/json", ODataVersion.V4);
            var result4 = TestMediaTypeWithFormat.ParseContentType("application/json", ODataVersion.V4, ODataMediaTypeResolver.GetMediaTypeResolver(null));

            result1.BeJsonLight().HaveDefaultMetadata();
            result2.BeJsonLight().HaveDefaultMetadata();
            result3.BeUnspecifiedJson();
            result4.BeUnspecifiedJson();
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
                Assert.NotNull(mediaType);
                Assert.NotNull(encoding);
                Assert.Equal(ODataPayloadKind.Batch, payloadKind);
                Assert.Equal(ODataFormat.Json, format);
            }
        }

        [Fact]
        public void AlterContentTypeForJsonPaddingIfNeededShouldThrowIfAtom()
        {
            const string original = "application/atom+xml";
            Action target = () => MediaTypeUtils.AlterContentTypeForJsonPadding(original);
            target.Throws<ODataException>(Strings.ODataMessageWriter_JsonPaddingOnInvalidContentType("application/atom+xml"));
        }

        [Fact]
        public void AlterContentTypeForJsonPaddingIfNeededShouldReplaceWithJavaScriptIfAppJson()
        {
            const string original = "application/json";
            var result = MediaTypeUtils.AlterContentTypeForJsonPadding(original);
            Assert.Equal("text/javascript", result);
        }

        [Fact]
        public void AlterContentTypeForJsonPaddingIfNeededShouldReplaceWithJavaScriptIfTextPlain()
        {
            const string original = "text/plain";
            var result = MediaTypeUtils.AlterContentTypeForJsonPadding(original);
            Assert.Equal("text/javascript", result);
        }

        [Fact]
        public void AlterContentTypeForJsonPaddingIfNeededShouldKeepParametersWhenReplacing()
        {
            const string original = "application/json;p1=v1;P2=v2";
            var result = MediaTypeUtils.AlterContentTypeForJsonPadding(original);
            Assert.Equal("text/javascript;p1=v1;P2=v2", result);
        }

        [Fact]
        public void AlterContentTypeForJsonPaddingIfNeededShouldBeCaseInsensitive()
        {
            const string original = "aPplIcAtiOn/JsOn";
            var result = MediaTypeUtils.AlterContentTypeForJsonPadding(original);
            Assert.Equal("text/javascript", result);
        }

        [Fact]
        public void AlterContentTypeForJsonPaddingIfNeededShouldFailIfAppJsonIsNotAtStart()
        {
            const string original = "tricky/application/json";
            Action target = () => MediaTypeUtils.AlterContentTypeForJsonPadding(original);
            target.Throws<ODataException>(Strings.ODataMessageWriter_JsonPaddingOnInvalidContentType("tricky/application/json"));
        }

        [Fact]
        public void MultipartBoundariesShouldNotGrowCache()
        {
            for (int i = 1; i < 100; i++)
            {
                ODataMediaType mediaType;
                Encoding encoding;
                ODataPayloadKind payloadKind;
                MediaTypeUtils.GetFormatFromContentType(string.Format("multipart/mixed;boundary={0}", Guid.NewGuid()), new ODataPayloadKind[] { ODataPayloadKind.Batch }, ODataMediaTypeResolver.GetMediaTypeResolver(null), out mediaType, out encoding, out payloadKind);
            }

            Assert.True(MediaTypeUtils.GetCacheKeys().Count(k => k.StartsWith("multipart/mixed")) == 1, "Multiple multipart/mixed keys in cache");
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
            Assert.NotNull(mediaType);
            Assert.NotNull(format);
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
            Assert.NotNull(mediaType);
            Assert.NotNull(format);
            return new TestMediaTypeWithFormat { MediaType = mediaType, Format = format };
        }
    }

    internal static class MediaTypeAssertionExtensions
    {
        public static TestMediaTypeWithFormat BeJsonLight(this TestMediaTypeWithFormat mediaType)
        {
            mediaType.HaveFullTypeName("application/json");
            mediaType.NotHaveParameterValue("odata.metadata", "verbose");
            mediaType.HaveExactFormat(ODataFormat.Json);
            return mediaType;
        }

        public static TestMediaTypeWithFormat BeUnspecifiedJson(this TestMediaTypeWithFormat mediaType)
        {
            return mediaType.HaveFullTypeName("application/json").NotHaveParameter("odata.metadata").NotHaveParameter("metadata");
        }

        public static TestMediaTypeWithFormat BeStreaming(this TestMediaTypeWithFormat mediaType)
        {
            Assert.True(mediaType.MediaType.HasStreamingSetToTrue());
            return mediaType;
        }

        public static TestMediaTypeWithFormat NotBeStreaming(this TestMediaTypeWithFormat mediaType)
        {
            Assert.False(mediaType.MediaType.HasStreamingSetToTrue());
            return mediaType;
        }

        public static TestMediaTypeWithFormat BeIeee754Compatible(this TestMediaTypeWithFormat mediaType)
        {
            Assert.True(mediaType.MediaType.HasIeee754CompatibleSetToTrue());
            return mediaType;
        }

        public static TestMediaTypeWithFormat NotBeIeee754Compatible(this TestMediaTypeWithFormat mediaType)
        {
            Assert.False(mediaType.MediaType.HasIeee754CompatibleSetToTrue());
            return mediaType;
        }

        public static TestMediaTypeWithFormat HaveFullTypeName(this TestMediaTypeWithFormat mediaType, string fullTypeName)
        {
            Assert.NotNull(mediaType.MediaType);
            Assert.Equal(fullTypeName, mediaType.MediaType.FullTypeName, true);
            return mediaType;
        }

        public static TestMediaTypeWithFormat HaveNoMetadata(this TestMediaTypeWithFormat mediaType)
        {
            return mediaType.HaveParameterValue(new string[] { "odata.metadata", "metadata" }, "none");
        }

        public static TestMediaTypeWithFormat HaveFullMetadata(this TestMediaTypeWithFormat mediaType)
        {
            return mediaType.HaveParameterValue(new string[] { "odata.metadata", "metadata" }, "full");
        }

        public static TestMediaTypeWithFormat HaveDefaultMetadata(this TestMediaTypeWithFormat mediaType)
        {
            return mediaType.HaveParameterValue(new string[] { "odata.metadata", "metadata" }, "minimal");
        }

        public static TestMediaTypeWithFormat HaveParameterValue(this TestMediaTypeWithFormat mediaType, string[] parameterNames, string parameterValue)
        {
            Assert.Contains(parameterNames, p => mediaType.MediaType.MediaTypeHasParameterWithValue(p, parameterValue));
            return mediaType;
        }

        public static TestMediaTypeWithFormat NotHaveParameterValue(this TestMediaTypeWithFormat mediaType, string parameterName, string parameterValue)
        {
            Assert.NotNull(mediaType.MediaType);
            Assert.False(mediaType.MediaType.MediaTypeHasParameterWithValue(parameterName, parameterValue));
            return mediaType;
        }

        public static TestMediaTypeWithFormat HaveExactFormat(this TestMediaTypeWithFormat mediaType, ODataFormat format)
        {
            Assert.Same(format, mediaType.Format);
            return mediaType;
        }

        public static TestMediaTypeWithFormat NotHaveParameter(this TestMediaTypeWithFormat mediaType, string parameterName)
        {
            Assert.NotNull(mediaType.MediaType);
            if (mediaType.MediaType.Parameters != null)
            {
                Assert.DoesNotContain(mediaType.MediaType.Parameters, kvp => kvp.Key == parameterName);
            }

            return mediaType;
        }
    }
}