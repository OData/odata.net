//---------------------------------------------------------------------
// <copyright file="ODataMessageWritingHelperTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using Microsoft.OData.Client;
    using FluentAssertions;
    using Microsoft.OData;
    using Microsoft.OData.Client.TDDUnitTests;
    using ClientStrings = Microsoft.OData.Client.Strings;
    using Xunit;

    public class ODataMessageWritingHelperTests
    {
        private DataServiceContext context;
        private RequestInfo requestInfo;
        private ODataMessageWritingHelper writingHelper;
        private ODataRequestMessageSimulator atomRequestMessage;
        private ODataRequestMessageSimulator jsonRequestMessage;

        public ODataMessageWritingHelperTests()
        {
            this.context = new DataServiceContext(new Uri("http://temp.org/"), ODataProtocolVersion.V4).ReConfigureForNetworkLoadingTests();
            this.requestInfo = new RequestInfo(context);
            this.writingHelper = new ODataMessageWritingHelper(this.requestInfo);
            this.atomRequestMessage = new ODataRequestMessageSimulator();
            this.atomRequestMessage.SetHeader(XmlConstants.HttpContentType, "ApplIcAtIOn/AtOm");
            this.jsonRequestMessage = new ODataRequestMessageSimulator();
            this.jsonRequestMessage.SetHeader(XmlConstants.HttpContentType, "ApplIcAtIOn/jsOn");
        }

        [Fact]
        public void CreateWriterOnV3ShouldNotThrowForAtom()
        {
            this.writingHelper.CreateWriter(this.atomRequestMessage, new ODataMessageWriterSettings(), isParameterPayload: true);
            this.writingHelper.CreateWriter(this.atomRequestMessage, new ODataMessageWriterSettings(), isParameterPayload: false);
        }

        [Fact]
        public void ShortIntegrationCreateWriterSettingsShouldSetInstanceAnnotationFilter()
        {
            this.requestInfo.Configurations.RequestPipeline.OnMessageWriterSettingsCreated((a => a.Settings.EnableMessageStreamDisposal = true));

            var settings = this.writingHelper.CreateSettings(false, false);
            settings.EnableMessageStreamDisposal.Should().BeTrue();
        }
    }
}
