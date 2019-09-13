//---------------------------------------------------------------------
// <copyright file="ODataMessageExtensionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataMessageExtensionsTests
    {
        private IODataRequestMessage requestMessage;
        private IODataResponseMessage responseMessage;

        public ODataMessageExtensionsTests()
        {
            this.requestMessage = new InMemoryMessage();
            this.responseMessage = new InMemoryMessage();
        }

        [Fact]
        public void GetDataServiceVersionWorksForRequest()
        {
            InMemoryMessage simulatedRequestMessage = new InMemoryMessage();
            simulatedRequestMessage.SetHeader(ODataConstants.ODataVersionHeader, "4.0");

            IODataRequestMessage request = new ODataRequestMessage(simulatedRequestMessage, false, false, long.MaxValue);
            ODataVersion version = request.GetODataVersion(ODataVersion.V4);
            Assert.Equal(ODataUtils.StringToODataVersion(request.GetHeader(ODataConstants.ODataVersionHeader)), version);
        }

        [Fact]
        public void GetDataServiceVersionWorksForResponse()
        {
            InMemoryMessage simulatedRequestMessage = new InMemoryMessage();
            simulatedRequestMessage.SetHeader(ODataConstants.ODataVersionHeader, "4.0");

            IODataResponseMessage response = new ODataResponseMessage(simulatedRequestMessage, false, false, long.MaxValue);
            ODataVersion version = response.GetODataVersion(ODataVersion.V4);
            Assert.Equal(ODataUtils.StringToODataVersion(response.GetHeader(ODataConstants.ODataVersionHeader)), version);
        }

        [Fact]
        public void PreferHeaderShouldReturnODataPreferenceHeader()
        {
            Assert.NotNull(this.requestMessage.PreferHeader());
        }

        [Fact]
        public void ShouldBeAbleToSetPreferHeaderThroughExtension()
        {
            this.requestMessage.PreferHeader().ReturnContent = true;
            this.requestMessage.PreferHeader().AnnotationFilter = "*";
            var headers = this.requestMessage.GetHeader("Prefer").Split(new[] { ',' }, 2);
            Assert.Contains("return=representation", headers);
            Assert.Contains("odata.include-annotations=\"*\"", headers);
        }

        [Fact]
        public void PreferenceAppliedHeaderShouldReturnODataPreferenceHeader()
        {
            Assert.NotNull(this.responseMessage.PreferenceAppliedHeader());
        }

        [Fact]
        public void ShouldBeAbleToSetPreferenceAppliedHeaderThroughExtension()
        {
            this.responseMessage.PreferenceAppliedHeader().ReturnContent = false;
            this.responseMessage.PreferenceAppliedHeader().AnnotationFilter = "*";
            var headers = this.responseMessage.GetHeader("Preference-Applied").Split(new[] { ',' }, 2);
            Assert.Contains("return=minimal", headers);
            Assert.Contains("odata.include-annotations=\"*\"", headers);
        }
    }
}
