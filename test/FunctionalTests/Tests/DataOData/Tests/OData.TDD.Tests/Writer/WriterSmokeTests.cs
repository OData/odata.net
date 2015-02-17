//---------------------------------------------------------------------
// <copyright file="WriterSmokeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Writer
{
    using System;
    using System.IO;
    using System.Text;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.Test.OData.TDD.Tests.Common;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Smoke tests for ODataLib writing scenarios. The intent of this file is to provide a location for writing user code that uses
    /// ODataLib to ensure a very simple scenario for a feature works.
    /// </summary>
    [TestClass]
    public class WriterSmokeTests
    {
        /// <summary>
        /// Smoke test for the primary JSONP success scenario - conneg results in JSON and the JSONP function is specified.
        /// </summary>
        [TestMethod]
        public void JsonPaddingEnabledWithJsonFormatSpecified()
        {
            var settings = new ODataMessageWriterSettings {JsonPCallback = "functionName", DisableMessageStreamDisposal = true};
            settings.SetContentType(ODataFormat.Json);
            settings.SetServiceDocumentUri(new Uri("http://stuff"));
            IODataResponseMessage message = new InMemoryMessage {StatusCode = 200, Stream = new MemoryStream()};
            var property = new ODataProperty {Name = "PropertyName", Value = "value"};
            
            using (var writer = new ODataMessageWriter(message, settings))
            {
                writer.WriteProperty(property);
            }

            var responseStream = message.GetStream();
            responseStream.Position = 0;
            var responseString = new StreamReader(responseStream).ReadToEnd();
            responseString.Should().Be("functionName({\"@odata.context\":\"http://stuff/$metadata#Edm.String\",\"value\":\"value\"})");
            message.GetHeader("Content-Type").Should().StartWith("text/javascript");
        }

        /// <summary>
        /// Smoke test raw values and JSONP
        /// </summary>
        [TestMethod]
        public void JsonPaddingEnabledWithRawValueSpecified()
        {
            var settings = new ODataMessageWriterSettings {JsonPCallback = "functionName", DisableMessageStreamDisposal = true};
            settings.SetContentType(ODataFormat.RawValue);
            IODataResponseMessage message = new InMemoryMessage {StatusCode = 200, Stream = new MemoryStream()};

            using (var writer = new ODataMessageWriter(message, settings))
            {
                writer.WriteValue(123);
            }

            var responseStream = message.GetStream();
            responseStream.Position = 0;
            var responseString = new StreamReader(responseStream).ReadToEnd();
            responseString.Should().Be("functionName(123)");
            message.GetHeader("Content-Type").Should().StartWith("text/javascript");
        }

        /// <summary>
        /// Ensures that if a user has set the content-type on their message we overwrite it in the JSONP case.
        /// </summary>
        [TestMethod]
        public void JsonPaddingEnabledWithUserSpecifiedContentType()
        {
            var settings = new ODataMessageWriterSettings {JsonPCallback = "functionName", DisableMessageStreamDisposal = true};
            settings.SetServiceDocumentUri(new Uri("http://stuff"));
            IODataResponseMessage message = new InMemoryMessage {StatusCode = 200, Stream = new MemoryStream()};
            message.SetHeader("Content-Type", "application/json");
            var property = new ODataProperty {Name = "PropertyName", Value = "value"};
            
            using (var writer = new ODataMessageWriter(message, settings))
            {
                writer.WriteProperty(property);
            }

            var responseStream = message.GetStream();
            responseStream.Position = 0;
            var responseString = new StreamReader(responseStream).ReadToEnd();
            responseString.Should().Be("functionName({\"@odata.context\":\"http://stuff/$metadata#Edm.String\",\"value\":\"value\"})");
            message.GetHeader("Content-Type").Should().StartWith("text/javascript");
        }

        [TestMethod]
        public void StreamEncodingRemainsInvariant()
        {
            var settings = new ODataMessageWriterSettings { JsonPCallback = "functionName", DisableMessageStreamDisposal = true };
            settings.SetServiceDocumentUri(new Uri("http://stuff"));
            StreamWriter streamWriter = new StreamWriter(new MemoryStream(), Encoding.GetEncoding("iso-8859-1"));
            IODataResponseMessage message = new InMemoryMessage { StatusCode = 200, Stream = streamWriter.BaseStream};
            message.SetHeader("Content-Type", "application/json");
            var property = new ODataProperty { Name = "PropertyName", Value = "value" };

            using (var writer = new ODataMessageWriter(message, settings))
            {
                writer.WriteProperty(property);
            }

            var responseStream = message.GetStream();
            responseStream.Position = 0;
            var responseString = new StreamReader(responseStream, Encoding.GetEncoding("iso-8859-1")).ReadToEnd();       
            responseString.Should().Be("functionName({\"@odata.context\":\"http://stuff/$metadata#Edm.String\",\"value\":\"value\"})");
            message.GetHeader("Content-Type").Should().StartWith("text/javascript");
        }
    }
}
