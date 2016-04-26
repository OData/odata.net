//---------------------------------------------------------------------
// <copyright file="ODataMessageReadingHelperTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using Microsoft.OData.Client;
    using FluentAssertions;
    using Microsoft.OData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ClientStrings = Microsoft.OData.Client.Strings;

    [TestClass]
    public class ODataMessageReadingHelperTests
    {
        private DataServiceContext context;
        private ResponseInfo responseInfo;
        private ODataMessageReadingHelper readingHelper;
        private ODataResponseMessageSimulator atomResponseMessage;
        private ODataResponseMessageSimulator jsonResponseMessage;

        [TestInitialize]
        public void Init()
        {
            this.context = new DataServiceContext(new Uri("http://temp.org/"), ODataProtocolVersion.V4);
            this.responseInfo = new ResponseInfo(new RequestInfo(this.context), MergeOption.NoTracking);
            this.readingHelper = new ODataMessageReadingHelper(this.responseInfo);
            this.atomResponseMessage = new ODataResponseMessageSimulator();
            this.atomResponseMessage.SetHeader(XmlConstants.HttpContentType, "ApplIcAtIOn/AtOm");
            this.jsonResponseMessage = new ODataResponseMessageSimulator();
            this.jsonResponseMessage.SetHeader(XmlConstants.HttpContentType, "ApplIcAtIOn/jsOn");
        }

        [TestMethod]
        public void CreateReaderOnV3ShouldNotThrowForAtom()
        {
            this.readingHelper.CreateReader(this.atomResponseMessage, new ODataMessageReaderSettings());
        }

        [TestMethod]
        public void ShortIntegrationValidateSettingAnnotationFilterOnCreateSettings()
        {
            Func<string, bool> instanceAnnotationFilterFunc = (name) => name == "MyAnnotation" ? true : false;
            
            this.responseInfo.ResponsePipeline.OnMessageReaderSettingsCreated((a => a.Settings.ShouldIncludeAnnotation = instanceAnnotationFilterFunc));
            var settings = this.readingHelper.CreateSettings();
            settings.ShouldIncludeAnnotation.Should().BeSameAs(instanceAnnotationFilterFunc);
        }
    }
}
