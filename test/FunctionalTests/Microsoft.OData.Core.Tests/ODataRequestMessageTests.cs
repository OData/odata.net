//---------------------------------------------------------------------
// <copyright file="ODataRequestMessageTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    /// <summary>
    /// Tests for the internal ODataRequestMessage that wraps a user-implemented IODataRequestMessage.
    /// </summary>
    public class ODataRequestMessageTests
    {
        [Fact]
        public void GetHeaderGoesToInnerMessageAfterConstruction()
        {
            const string headerName = "CustomHeaderName";
            const string headerValue = "CustomerHeaderValue";

            var simulatedRequestMessage = new InMemoryMessage();
            simulatedRequestMessage.SetHeader(headerName, headerValue);
            
            var odataRequestMessage = new ODataRequestMessage(simulatedRequestMessage, false, false, -1);

            odataRequestMessage.GetHeader(headerName).Should().Be(headerValue);
        }

        [Fact]
        public void GetHeaderGoesToInnerMessageAfterLaterInnerSetHeaderCall()
        {
            const string headerName = "CustomHeaderName";
            const string headerValueBefore = "CustomerHeaderValueBefore";
            const string headerValueAfter = "CustomerHeaderValueAfter";

            var simulatedRequestMessage = new InMemoryMessage();
            simulatedRequestMessage.SetHeader(headerName, headerValueBefore);
            
            var odataRequestMessage = new ODataRequestMessage(simulatedRequestMessage, false, false, -1);
            simulatedRequestMessage.SetHeader(headerName, headerValueAfter);
            
            odataRequestMessage.GetHeader(headerName).Should().Be(headerValueAfter);
        }

        [Fact]
        public void GetHeaderOnInnerMessagePicksUpSetHeaderFromOuterCallOnWriting()
        {
            const string headerName = "CustomHeaderName";
            const string headerValueBefore = "CustomerHeaderValueBefore";
            const string headerValueAfter = "CustomerHeaderValueAfter";

            var simulatedRequestMessage = new InMemoryMessage { Method = "GET", Url = new Uri("http://example.com/Customers") }; 
            simulatedRequestMessage.SetHeader(headerName, headerValueBefore);
            
            var odataRequestMessage = new ODataRequestMessage(simulatedRequestMessage, true, false, -1);
            odataRequestMessage.SetHeader(headerName, headerValueAfter);

            simulatedRequestMessage.GetHeader(headerName).Should().Be(headerValueAfter);
        }

        [Fact]
        public void SetHeaderIsNotAllowedWhenReading()
        {
            const string headerName = "CustomHeaderName";
            const string headerValueBefore = "CustomerHeaderValueBefore";
            const string headerValueAfter = "CustomerHeaderValueAfter";

            var simulatedRequestMessage = new InMemoryMessage(); 
            simulatedRequestMessage.SetHeader(headerName, headerValueBefore);
            
            var odataRequestMessage = new ODataRequestMessage(simulatedRequestMessage, false, false, -1);
            Action setHeader = (() => odataRequestMessage.SetHeader(headerName, headerValueAfter));
            setHeader.ShouldThrow<ODataException>().WithMessage(Strings.ODataMessage_MustNotModifyMessage);
        }

        [Fact]
        public void GetAcceptHeaderWithInnerValueGetsInnerValue()
        {
            const string headerName = "Accept";
            const string headerValue = "json-rox";

            var simulatedRequestMessage = new InMemoryMessage();
            simulatedRequestMessage.SetHeader(headerName, headerValue);

            var odataRequestMessage = new ODataRequestMessage(simulatedRequestMessage, false, false, -1);

            odataRequestMessage.GetHeader(headerName).Should().Be(headerValue);
        }
    }
}
