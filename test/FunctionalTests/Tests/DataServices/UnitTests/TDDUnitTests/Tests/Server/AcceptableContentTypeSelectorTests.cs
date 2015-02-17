//---------------------------------------------------------------------
// <copyright file="AcceptableContentTypeSelectorTests.cs" company="Microsoft">
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
    public class AcceptableContentTypeSelectorTests
    {
        [TestMethod]
        public void FormatIsXml()
        {
            RunV3GetFormatTest("application/xml", "xml", null);
            RunV3GetFormatTest("application/xml", "xml", "application/json;odata.metadata=minimal");
            RunV3GetFormatTest("application/xml", "xml", "application/json");
        }
        
        [TestMethod]
        public void FormatIsJson()
        {
            RunV3GetFormatTest("application/json", "json", null);
            RunV3GetFormatTest("application/json", "json", "application/atom+xml");
        }

        [TestMethod]
        public void FormatIsAtom()
        {
            RunV3GetFormatTest("application/atom+xml", "atom", null);
            RunV3GetFormatTest("application/atom+xml", "atom", "application/json;odata.metadata=minimal");
        }

        [TestMethod]
        public void FormatIsApplicationAtomXml()
        {
            RunV3GetFormatTest("application/atom+xml", "application/atom+xml", null);
            RunV3GetFormatTest("application/atom+xml", "application/atom+xml", "application/json");
        }

        [TestMethod]
        public void NotSpecialFormatIsJustReturnedAsIs()
        {
            RunV3GetFormatTest("some not special value", "some not special value", null);
            RunV3GetFormatTest("some not special value", "some not special value", "application/atom+xml");
        }

        private static void RunV3GetFormatTest(string expectedFormat, string queryFormatValue, string acceptHeaderValue)
        {
            RunGetFormatTest(expectedFormat, queryFormatValue, acceptHeaderValue, new Version(4, 0));
        }

        private static void RunGetFormatTest(string expectedFormat, string queryFormatValue, string acceptHeaderValue, Version maxDataServiceVersion)
        {
            var processor = new AcceptableContentTypeSelector();
            var actualFormat = processor.GetFormat(queryFormatValue, acceptHeaderValue, maxDataServiceVersion);
            actualFormat.Should().Be(expectedFormat);
        }
    }
}
