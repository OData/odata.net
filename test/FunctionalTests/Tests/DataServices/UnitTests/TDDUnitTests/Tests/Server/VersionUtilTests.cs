//---------------------------------------------------------------------
// <copyright file="VersionUtilTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using Microsoft.OData.Service;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VersionUtilTests
    {
        private const string MimeApplicationAtom = "application/atom+xml";
        private const string MimeApplicationJsonODataMinimalMetadata = "application/json;odata.metadata=minimal";
        private const string MimeApplicationJsonODataFullMetadata = "application/json;odata.metadata=full";
        private const string MimeApplicationJsonODataNoMetadata = "application/json;odata.metadata=none";
        private const string MimeApplicationJson = "application/json";
        private const string MimeApplicationXml = "application/xml";
        private static readonly Version Version100Dot0 = new Version(100, 0);
        private static readonly Version Version100Dot1 = new Version(100, 1);

        [TestMethod]
        public void GetEffectiveMaxResponseVersionShouldReturnMpvWhenMaxDsvIsNull()
        {
            VersionUtil.GetEffectiveMaxResponseVersion(VersionUtil.Version4Dot0, null).As<object>().Should().Be(VersionUtil.Version4Dot0);
            VersionUtil.GetEffectiveMaxResponseVersion(Version100Dot1, null).As<object>().Should().Be(Version100Dot1);
        }

        [TestMethod]
        public void GetEffectiveMaxResponseVersionShouldReturnMinOfMpvAndMaxDsv()
        {
            VersionUtil.GetEffectiveMaxResponseVersion(VersionUtil.Version4Dot0, VersionUtil.Version4Dot0).Should().Be(VersionUtil.Version4Dot0);
            VersionUtil.GetEffectiveMaxResponseVersion(Version100Dot1, Version100Dot0).Should().Be(Version100Dot0);
            VersionUtil.GetEffectiveMaxResponseVersion(Version100Dot0, Version100Dot0).Should().Be(Version100Dot0);
        }

        [TestMethod]
        public void WhenMpvIsAtLeast30AndFormatIsNotJsonLightResponseVersionShouldBe30()
        {
            VersionUtil.GetResponseVersionForError( /*acceptableContentType*/ MimeApplicationXml, /*maxDSV*/ VersionUtil.Version4Dot0, /*MPV*/ VersionUtil.Version4Dot0).Should().Be(VersionUtil.Version4Dot0);
        }

        [TestMethod]
        public void WhenMpvAndMaxDsvAreAtLeast30AndFormatIsJsonLightResponseVersionShouldBe30()
        {
            VersionUtil.GetResponseVersionForError( /*acceptableContentType*/ MimeApplicationJsonODataFullMetadata, /*maxDSV*/ VersionUtil.Version4Dot0, /*MPV*/ VersionUtil.Version4Dot0).Should().Be(VersionUtil.Version4Dot0);
            VersionUtil.GetResponseVersionForError( /*acceptableContentType*/ MimeApplicationJsonODataMinimalMetadata, /*maxDSV*/ Version100Dot1, /*MPV*/ Version100Dot1).Should().Be(VersionUtil.Version4Dot0);
            VersionUtil.GetResponseVersionForError( /*acceptableContentType*/ MimeApplicationJsonODataNoMetadata, /*maxDSV*/ Version100Dot1, /*MPV*/ Version100Dot1).Should().Be(VersionUtil.Version4Dot0);
            VersionUtil.GetResponseVersionForError( /*acceptableContentType*/ MimeApplicationJson, /*maxDSV*/ Version100Dot1, /*MPV*/ Version100Dot1).Should().Be(VersionUtil.Version4Dot0);
        }
    }
}
