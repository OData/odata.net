//---------------------------------------------------------------------
// <copyright file="JsonLightContentNegotiationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests
{
    using System;
    using System.Text;
    using FluentAssertions;
    using FluentAssertions.Primitives;
    using Microsoft.OData.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ErrorStrings = Microsoft.OData.Core.Strings;

    [TestClass]
    public class JsonLightContentNegotiationTests
    {
        [TestMethod]
        public void StreamingAcceptShouldResolveToStreamingResponse()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;odata.streaming=true", ODataVersion.V4).Should().BeJsonLight().And.BeStreaming();
        }

        [TestMethod]
        public void NonStreamingAcceptShouldResolveToNonStreamingResponse()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;odata.streaming=false", ODataVersion.V4).Should().BeJsonLight().And.NotBeStreaming();
        }

        [TestMethod]
        public void UnspecifiedAcceptShouldResolveToStreamingResponse()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal", ODataVersion.V4).Should().BeJsonLight().And.BeStreaming();
        }

        [TestMethod]
        public void UnrecognizedStreamingParameterValueForAcceptShouldFail()
        {
            Action acceptUnrecognizedStreamingValue = () => TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;odata.streaming=foo", ODataVersion.V4);
            acceptUnrecognizedStreamingValue
                .ShouldThrow<ODataException>()
                .WithMessage(ErrorStrings.MediaTypeUtils_DidNotFindMatchingMediaType("*", "application/json;odata.metadata=minimal;odata.streaming=foo"), ComparisonMode.Wildcard);
        }

        [TestMethod]
        public void Ieee754CompatibleAcceptShouldBeResolved()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;IEEE754Compatible=true", ODataVersion.V4).Should().BeJsonLight().And.BeIeee754Compatible();
        }

        [TestMethod]
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

        [TestMethod]
        public void UnrecognizedIeee754CompatibleParameterValueForAcceptShouldFail()
        {
            Action acceptUnrecognizedIeee754CompatibleValue = () => TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;IEEE754Compatible=none", ODataVersion.V4);
            acceptUnrecognizedIeee754CompatibleValue
                .ShouldThrow<ODataException>()
                .WithMessage(ErrorStrings.MediaTypeUtils_DidNotFindMatchingMediaType("*", "application/json;odata.metadata=minimal;IEEE754Compatible=none"), ComparisonMode.Wildcard);
        }

        [TestMethod]
        public void UnrecognizedMetadataParameterValueForAcceptShouldFail()
        {
            Action acceptUnrecognizedStreamingValue = () => TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=foometadata;", ODataVersion.V4);
            acceptUnrecognizedStreamingValue
                .ShouldThrow<ODataException>()
                .WithMessage(ErrorStrings.MediaTypeUtils_DidNotFindMatchingMediaType("*", "application/json;odata.metadata=foometadata;"), ComparisonMode.Wildcard);
        }

        [TestMethod]
        public void UnrecognizedParameterForAcceptShouldFail()
        {
            Action acceptUnrecognizedParameter = () => TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;foo=bar", ODataVersion.V4);
            acceptUnrecognizedParameter
                .ShouldThrow<ODataException>()
                .WithMessage(ErrorStrings.MediaTypeUtils_DidNotFindMatchingMediaType("*", "application/json;odata.metadata=minimal;foo=bar"), ComparisonMode.Wildcard);
        }

        [TestMethod]
        public void StreamingContentTypeShouldParseCorrectly()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal;odata.streaming=true", ODataVersion.V4).Should().BeJsonLight().And.BeStreaming();
        }

        [TestMethod]
        public void NonStreamingContentTypeShouldParseCorrectly()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal;odata.streaming=false", ODataVersion.V4).Should().BeJsonLight().And.NotBeStreaming();
        }

        [TestMethod]
        public void UnspecifiedStreamingInContentTypeShouldMeanNoStreaming()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal;", ODataVersion.V4).Should().BeJsonLight().And.NotBeStreaming();
        }

        [TestMethod]
        public void UnrecognizedContentTypeStreamingValueShouldMeanNoStreaming()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal;odata.streaming=foo", ODataVersion.V4).Should().BeJsonLight().And.NotBeStreaming();
        }

        [TestMethod]
        public void UnrecognizedContentTypeParameterShouldMeanNoStreaming()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal;foo=bar", ODataVersion.V4).Should().BeJsonLight().And.NotBeStreaming();
        }

        [TestMethod]
        public void Ieee754CompatibleShouldParseCorrectly()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal;IEEE754Compatible=true", ODataVersion.V4).Should().BeJsonLight().And.BeIeee754Compatible();
        }

        [TestMethod]
        public void UnrecognizedContentTypeIeee754CompatibleValueShouldMeanNegative()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal;IEEE754Compatible=none", ODataVersion.V4).Should().BeJsonLight().And.NotBeIeee754Compatible();
        }

        [TestMethod]
        public void UnspecifiedIeee754CompatibleInContentTypeShouldMeanNegative()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal", ODataVersion.V4).Should().BeJsonLight().And.NotBeIeee754Compatible();
        }

        [TestMethod]
        public void MetadataAllContentTypeShouldSucceed()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=full", ODataVersion.V4).Should().BeJsonLight().And.SpecifyAllMetadata();
        }

        [TestMethod]
        public void MetadataNoneContentTypeShouldSucceed()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=none", ODataVersion.V4).Should().BeJsonLight().And.SpecifyNoMetadata();
        }

        [TestMethod]
        public void DefaultMetadataContentTypeShouldSucceed()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json;odata.metadata=minimal", ODataVersion.V4).Should().BeJsonLight().And.SpecifyDefaultMetadata();
        }

        [TestMethod]
        public void MetadataAllAcceptShouldSucceed()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=full", ODataVersion.V4).Should().BeJsonLight().And.SpecifyAllMetadata();
        }

        [TestMethod]
        public void UnspecifiedAcceptShouldResolveToDefault()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json", ODataVersion.V4).Should().BeJsonLight().And.SpecifyDefaultMetadata();
        }

        [TestMethod]
        public void MetadataNoneAcceptShouldSucceed()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=none", ODataVersion.V4).Should().BeJsonLight().And.SpecifyNoMetadata();
        }

        [TestMethod]
        public void DefaultMetadataAcceptShouldSucceed()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal", ODataVersion.V4).Should().BeJsonLight().And.SpecifyDefaultMetadata();
        }

        [TestMethod]
        public void StreamingShouldBePreferredOverNonStreaming()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal;odata.streaming=true,application/json;odata.metadata=minimal;odata.streaming=false", ODataVersion.V4).Should().BeJsonLight().And.BeStreaming();
        }

        [TestMethod]
        public void UnspecifiedMetadataShouldBePreferedOverAll()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json,application/json;odata.metadata=full", ODataVersion.V4).Should().BeJsonLight().And.SpecifyDefaultMetadata();
        }

        [TestMethod]
        public void DefaultMetadataShouldBePreferedOverAll()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=minimal,application/json;odata.metadata=full", ODataVersion.V4).Should().BeJsonLight().And.SpecifyDefaultMetadata();
        }

        [TestMethod]
        public void AllMetadataShouldBePreferedOverNone()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json;odata.metadata=full,application/json;odata.metadata=none", ODataVersion.V4).Should().BeJsonLight().And.SpecifyAllMetadata();
        }

        [TestMethod]
        public void UnqualifiedContentTypeInV3ShouldBeJsonLightButUnspecified()
        {
            TestMediaTypeWithFormat.ParseContentType("application/json", ODataVersion.V4).Should().BeUnspecifiedJson().And.HaveExactFormat(ODataFormat.Json);
        }

        [TestMethod]
        public void QualifiedJsonShouldThrow()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("application/json,application/json;odata.metadata=verbose", ODataVersion.V4).Should().BeJsonLight().And.SpecifyDefaultMetadata();
        }

        [TestMethod]
        public void StreamingParameterShouldBeCaseInsensitive()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("appLICatIOn/jSOn;OdAtA.sTrEaMinG=TrUe", ODataVersion.V4).Should().BeJsonLight().And.BeStreaming();
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("APpLiCAtIOn/jSoN;oDaTa.stReAMinG=fAlSe", ODataVersion.V4).Should().BeJsonLight().And.NotBeStreaming();
            TestMediaTypeWithFormat.ParseContentType("appLICatIOn/jSOn;OdAtA.sTrEaMinG=TrUe", ODataVersion.V4).Should().BeJsonLight().And.BeStreaming();
            TestMediaTypeWithFormat.ParseContentType("APpLiCAtIOn/jSoN;oDaTa.stReAMinG=fAlSe", ODataVersion.V4).Should().BeJsonLight().And.NotBeStreaming();
        }

        [TestMethod]
        public void ODataParameterShouldBeCaseInsensitive()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("appLICatIOn/jSOn;oDaTa.MeTaDAtA=MinIMaL", ODataVersion.V4).Should().BeJsonLight().And.SpecifyDefaultMetadata();
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("appLICatIOn/jSOn;oDaTa.MeTaDAta=FuLl", ODataVersion.V4).Should().BeJsonLight().And.SpecifyAllMetadata();
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("appLICatIOn/jSOn;oDaTa.MetADAtA=nONe", ODataVersion.V4).Should().BeJsonLight().And.SpecifyNoMetadata();
            TestMediaTypeWithFormat.ParseContentType("appLICatIOn/jSOn;oDaTa.MeTaDaTa=MinIMaL", ODataVersion.V4).Should().BeJsonLight().And.SpecifyDefaultMetadata();
            TestMediaTypeWithFormat.ParseContentType("appLICatIOn/jSOn;ODATA.MeTaDaTa=FuLl", ODataVersion.V4).Should().BeJsonLight().And.SpecifyAllMetadata();
            TestMediaTypeWithFormat.ParseContentType("appLICatIOn/jSOn;oDaTa.MeTaDaTa=nONe", ODataVersion.V4).Should().BeJsonLight().And.SpecifyNoMetadata();
        }

        [TestMethod]
        public void Ieee754CompatibleShouldBeCaseInsensitive()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("appLICatIOn/jSOn;ieee754Compatible=TRUE", ODataVersion.V4).Should().BeJsonLight().And.BeIeee754Compatible();
            TestMediaTypeWithFormat.GetResponseTypeFromAccept("appLICatIOn/jSOn;ieee754Compatible=False", ODataVersion.V4).Should().BeJsonLight().And.NotBeIeee754Compatible();
            TestMediaTypeWithFormat.ParseContentType("appLICatIOn/jSOn;ieee754Compatible=TRUE", ODataVersion.V4).Should().BeJsonLight().And.BeIeee754Compatible();
            TestMediaTypeWithFormat.ParseContentType("appLICatIOn/jSOn;ieee754Compatible=False", ODataVersion.V4).Should().BeJsonLight().And.NotBeIeee754Compatible();
        }

        [TestMethod]
        public void JsonFormatShouldBeJsonLightInV3()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromFormat(ODataFormat.Json, ODataVersion.V4).Should().BeJsonLight();
        }

        [TestMethod]
        public void JsonLightFormatShouldResolveToMinimalMetadataAndStreaming()
        {
            TestMediaTypeWithFormat.GetResponseTypeFromFormat(ODataFormat.Json, ODataVersion.V4).Should().BeJsonLight().And.SpecifyDefaultMetadata().And.BeStreaming();
        }

        [TestMethod]
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