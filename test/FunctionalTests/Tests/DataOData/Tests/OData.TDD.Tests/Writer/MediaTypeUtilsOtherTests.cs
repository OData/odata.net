//---------------------------------------------------------------------
// <copyright file="MediaTypeUtilsOtherTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Writer
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// JsonLightContentNegotiationTests cover much of the methods on MediaTypeUtils.
    /// This is a place for other tests on that class.
    /// </summary>
    [TestClass]
    public class MediaTypeUtilsOtherTests
    {
        [TestMethod]
        public void AlterContentTypeForJsonPaddingIfNeededShouldThrowIfAtom()
        {
            const string original = "application/atom+xml";
            Action target = () => MediaTypeUtils.AlterContentTypeForJsonPadding(original);
            target.ShouldThrow<ODataException>().WithMessage(Strings.ODataMessageWriter_JsonPaddingOnInvalidContentType("application/atom+xml"));
        }

        [TestMethod]
        public void AlterContentTypeForJsonPaddingIfNeededShouldReplaceWithJavaScriptIfAppJson()
        {
            const string original = "application/json";
            var result = MediaTypeUtils.AlterContentTypeForJsonPadding(original);
            result.Should().Be("text/javascript");
        }

        [TestMethod]
        public void AlterContentTypeForJsonPaddingIfNeededShouldReplaceWithJavaScriptIfTextPlain()
        {
            const string original = "text/plain";
            var result = MediaTypeUtils.AlterContentTypeForJsonPadding(original);
            result.Should().Be("text/javascript");
        }

        [TestMethod]
        public void AlterContentTypeForJsonPaddingIfNeededShouldKeepParametersWhenReplacing()
        {
            const string original = "application/json;p1=v1;P2=v2";
            var result = MediaTypeUtils.AlterContentTypeForJsonPadding(original);
            result.Should().Be("text/javascript;p1=v1;P2=v2");
        }

        [TestMethod]
        public void AlterContentTypeForJsonPaddingIfNeededShouldBeCaseInsensitive()
        {
            const string original = "aPplIcAtiOn/JsOn";
            var result = MediaTypeUtils.AlterContentTypeForJsonPadding(original);
            result.Should().Be("text/javascript");
        }

        [TestMethod]
        public void AlterContentTypeForJsonPaddingIfNeededShouldFailIfAppJsonIsNotAtStart()
        {
            const string original = "tricky/application/json";
            Action target = () => MediaTypeUtils.AlterContentTypeForJsonPadding(original);
            target.ShouldThrow<ODataException>().WithMessage(Strings.ODataMessageWriter_JsonPaddingOnInvalidContentType("tricky/application/json"));
        }
    }
}
