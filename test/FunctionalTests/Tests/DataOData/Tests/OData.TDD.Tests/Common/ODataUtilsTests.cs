//---------------------------------------------------------------------
// <copyright file="ODataUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Common
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataUtilsTests
    {
        [TestMethod]
        public void CreateInstanceAnnotationsFilterShouldTranslateFilterStringToFunc()
        {
            Func<string, bool> filter = ODataUtils.CreateAnnotationFilter("*,-ns.*");
            filter.Should().NotBeNull();
            filter("ns.name").Should().BeFalse();
            filter("foo.bar").Should().BeTrue();
        }

        [TestMethod]
        public void MediaTypeOfJsonShouldHaveDefaultValues()
        {
            this.TestMediaTypeOfJson(MimeConstants.MimeApplicationJson, MimeConstants.MimeMetadataParameterValueMinimal, MimeConstants.MimeParameterValueTrue, MimeConstants.MimeParameterValueFalse);
        }

        [TestMethod]
        public void CustomValueOfStreamingShouldOverrideDefaultValueInMediaTypeOfJson()
        {
            this.TestMediaTypeOfJson(
                MimeConstants.MimeApplicationJson + ";" + MimeConstants.MimeStreamingParameterName + "=" + MimeConstants.MimeParameterValueFalse,
                MimeConstants.MimeMetadataParameterValueMinimal,
                MimeConstants.MimeParameterValueFalse,
                MimeConstants.MimeParameterValueFalse);
        }

        [TestMethod]
        public void CustomValueOfIeee754CompatibleShouldOverrideDefaultValueInMediaTypeOfJson()
        {
            this.TestMediaTypeOfJson(
                MimeConstants.MimeApplicationJson + ";" + MimeConstants.MimeIeee754CompatibleParameterName + "=" + MimeConstants.MimeParameterValueTrue,
                MimeConstants.MimeMetadataParameterValueMinimal,
                MimeConstants.MimeParameterValueTrue,
                MimeConstants.MimeParameterValueTrue);
        }

        [TestMethod]
        public void CustomValueOfMetadataShouldOverrideDefaultValueInMediaTypeOfJson()
        {
            this.TestMediaTypeOfJson(
                MimeConstants.MimeApplicationJson + ";" + MimeConstants.MimeMetadataParameterName + "=" + MimeConstants.MimeMetadataParameterValueFull,
                MimeConstants.MimeMetadataParameterValueFull,
                MimeConstants.MimeParameterValueTrue,
                MimeConstants.MimeParameterValueFalse);
        }

        [TestMethod]
        public void CustomValuesShouldOverrideDefaultValuesInMediaTypeOfJson()
        {
            this.TestMediaTypeOfJson(
                MimeConstants.MimeApplicationJson + ";" +
                MimeConstants.MimeMetadataParameterName + "=" + MimeConstants.MimeMetadataParameterValueNone + ";" +
                MimeConstants.MimeIeee754CompatibleParameterName + "=" + MimeConstants.MimeParameterValueTrue + ";" +
                MimeConstants.MimeStreamingParameterName + "=" + MimeConstants.MimeParameterValueFalse,
                MimeConstants.MimeMetadataParameterValueNone,
                MimeConstants.MimeParameterValueFalse,
                MimeConstants.MimeParameterValueTrue);
        }

        [TestMethod]
        public void MediaTypeOfNonJsonShouldNotHaveDefaultValues()
        {
            ODataMediaType mediaTypeObject = GetAppendedMediaTypeObject(MimeConstants.MimeApplicationAtomXml);

            Assert.IsFalse(mediaTypeObject.ToText().Contains(MimeConstants.MimeStreamingParameterName));
            Assert.IsFalse(mediaTypeObject.ToText().Contains(MimeConstants.MimeStreamingParameterName));
            Assert.IsFalse(mediaTypeObject.ToText().Contains(MimeConstants.MimeIeee754CompatibleParameterName));
        }

        [TestMethod]
        public void NonMediaTypeShouldNotHaveDefaultValues()
        {
            const string headerName = "Some-Header";
            const string headerValue = MimeConstants.MimeApplicationJson;
            Assert.AreEqual(headerValue, ODataUtils.AppendDefaultHeaderValue(headerName, headerValue));
        }

        [TestMethod]
        public void CharsetShouldNotBeIgnored_UTF8()
        {
            const string headerValue = MimeConstants.MimeApplicationJson + ";" +
                MimeConstants.MimeMetadataParameterName + "=" + MimeConstants.MimeMetadataParameterValueNone + ";" +
                MimeConstants.MimeIeee754CompatibleParameterName + "=" + MimeConstants.MimeParameterValueTrue + ";" +
                MimeConstants.MimeStreamingParameterName + "=" + MimeConstants.MimeParameterValueFalse + ";charset=utf-8";

            var appendedHeaderValue = ODataUtils.AppendDefaultHeaderValue(ODataConstants.ContentTypeHeader, headerValue);

            Assert.IsTrue(appendedHeaderValue.Contains("charset=utf-8"));
        }

        [TestMethod]
        public void CharsetShouldNotBeIgnored_ISO_8859_1()
        {
            const string headerValue = MimeConstants.MimeApplicationJson + ";" +
                MimeConstants.MimeMetadataParameterName + "=" + MimeConstants.MimeMetadataParameterValueNone + ";" +
                MimeConstants.MimeIeee754CompatibleParameterName + "=" + MimeConstants.MimeParameterValueTrue + ";" +
                MimeConstants.MimeStreamingParameterName + "=" + MimeConstants.MimeParameterValueFalse + ";charset=ISO_8859-1";

            var appendedHeaderValue = ODataUtils.AppendDefaultHeaderValue(ODataConstants.ContentTypeHeader, headerValue);

            Assert.IsTrue(appendedHeaderValue.Contains("charset=iso-8859-1"));
        }

        [TestMethod]
        public void InvalidCharsetShouldBeIgnored()
        {
            const string headerValue = MimeConstants.MimeApplicationJson + ";" +
                MimeConstants.MimeMetadataParameterName + "=" + MimeConstants.MimeMetadataParameterValueNone + ";" +
                MimeConstants.MimeIeee754CompatibleParameterName + "=" + MimeConstants.MimeParameterValueTrue + ";" +
                MimeConstants.MimeStreamingParameterName + "=" + MimeConstants.MimeParameterValueFalse + ";charset=utf-9";

            var appendedHeaderValue = ODataUtils.AppendDefaultHeaderValue(ODataConstants.ContentTypeHeader, headerValue);

            Assert.IsFalse(appendedHeaderValue.Contains("charset"));
        }

        private void TestMediaTypeOfJson(string mediaType, string metadata, string streaming, string ieee754Compatible)
        {
            ODataMediaType mediaTypeObject = GetAppendedMediaTypeObject(mediaType);

            Assert.IsTrue(mediaTypeObject.MediaTypeHasParameterWithValue(MimeConstants.MimeMetadataParameterName, metadata));
            Assert.IsTrue(mediaTypeObject.MediaTypeHasParameterWithValue(MimeConstants.MimeStreamingParameterName, streaming));
            Assert.IsTrue(mediaTypeObject.MediaTypeHasParameterWithValue(MimeConstants.MimeIeee754CompatibleParameterName, ieee754Compatible));
        }

        private ODataMediaType GetAppendedMediaTypeObject(string mediaType)
        {
            var appendedMediaType = ODataUtils.AppendDefaultHeaderValue(ODataConstants.ContentTypeHeader, mediaType);

            var mediaTypeList = HttpUtils.MediaTypesFromString(appendedMediaType);
            Assert.IsTrue(mediaTypeList.Count == 1);

            return mediaTypeList[0].Key;
        }
    }
}
