//---------------------------------------------------------------------
// <copyright file="ODataFormatWithParametersTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using Microsoft.OData.Service;
    using Microsoft.OData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;
    using ErrorStrings = Microsoft.OData.Service.Strings;

    [TestClass]
    public class ODataFormatWithParametersTests
    {
        private ODataFormatWithParameters metadataFormat;
        private ODataFormatWithParameters jsonLightFormat;

        [TestInitialize]
        public void Init()
        {
            // no media type specified
            this.metadataFormat = new ODataFormatWithParameters(ODataFormat.Metadata);

            // media type with parameters
            this.jsonLightFormat = new ODataFormatWithParameters(ODataFormat.Json, "application/atom+xml;odata.metadata=minimal");
        }

        [TestMethod]
        public void FormatShouldBeSame()
        {
            this.metadataFormat.Format.Should().BeSameAs(ODataFormat.Metadata);
            this.jsonLightFormat.Format.Should().BeSameAs(ODataFormat.Json);
        }
        
        [TestMethod]
        public void GetParameterValueShouldBeNullIfNotFound()
        {
            this.metadataFormat.GetParameterValue("foo").Should().BeNull();
            this.jsonLightFormat.GetParameterValue("foo").Should().BeNull();
        }

        [TestMethod]
        public void GetParameterValueShouldReturnValueIfPresent()
        {
            this.jsonLightFormat.GetParameterValue("odata.metadata").Should().Be("minimal");
        }

        [TestMethod]
        public void GetParameterValueShouldBeCaseInsensitive()
        {
            this.jsonLightFormat.GetParameterValue("ODatA.mEtaDatA").Should().Be("minimal");
        }
    }
}