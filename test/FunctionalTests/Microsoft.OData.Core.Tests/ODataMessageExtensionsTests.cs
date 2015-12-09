//---------------------------------------------------------------------
// <copyright file="ODataMessageExtensionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests
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
            version.Should().Be(ODataUtils.StringToODataVersion(request.GetHeader(ODataConstants.ODataVersionHeader)));
        }

        [Fact]
        public void GetDataServiceVersionWorksForResponse()
        {
            InMemoryMessage simulatedRequestMessage = new InMemoryMessage();
            simulatedRequestMessage.SetHeader(ODataConstants.ODataVersionHeader, "4.0");

            IODataResponseMessage response = new ODataResponseMessage(simulatedRequestMessage, false, false, long.MaxValue);
            ODataVersion version = response.GetODataVersion(ODataVersion.V4);
            version.Should().Be(ODataUtils.StringToODataVersion(response.GetHeader(ODataConstants.ODataVersionHeader)));
        }

        [Fact]
        public void PreferHeaderShouldReturnODataPreferenceHeader()
        {
            this.requestMessage.PreferHeader().Should().NotBeNull();
        }

        [Fact]
        public void ShouldBeAbleToSetPreferHeaderThroughExtension()
        {
            this.requestMessage.PreferHeader().ReturnContent = true;
            this.requestMessage.PreferHeader().AnnotationFilter = "*";
            this.requestMessage.GetHeader("Prefer").Split(new[] {','}, 2).Should().Contain("return=representation").And.Contain("odata.include-annotations=\"*\"");
        }

        [Fact]
        public void PreferenceAppliedHeaderShouldReturnODataPreferenceHeader()
        {
            this.responseMessage.PreferenceAppliedHeader().Should().NotBeNull();
        }

        [Fact]
        public void ShouldBeAbleToSetPreferenceAppliedHeaderThroughExtension()
        {
            this.responseMessage.PreferenceAppliedHeader().ReturnContent = false;
            this.responseMessage.PreferenceAppliedHeader().AnnotationFilter = "*";
            this.responseMessage.GetHeader("Preference-Applied").Split(new[] { ',' }, 2).Should().Contain("return=minimal").And.Contain("odata.include-annotations=\"*\"");
        }
    }
}
