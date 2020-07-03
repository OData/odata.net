//---------------------------------------------------------------------
// <copyright file="ReceivingResponseEventArgsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData;
    using Xunit;

    public class ReceivingResponseEventArgsTests
    {
        [Fact]
        public void ConstructorShouldGetHeadersFromParameter()
        {
            var originalHeaders = new List<KeyValuePair<string, string>> {new KeyValuePair<string, string>("first-header", "first_value"), new KeyValuePair<string, string>("second header", "second value")};
            var eventArgs = new ReceivingResponseEventArgs(new ResponseMessageSimulator(originalHeaders), null, false);
            eventArgs.ResponseMessage.Headers.Count().Should().Be(originalHeaders.Count);
            originalHeaders.ForEach((h) => eventArgs.ResponseMessage.Headers.Should().Contain(h));
        }

        [Fact]
        public void IsBatchOperationIsSetInConstructor()
        {
            var eventArgs = new ReceivingResponseEventArgs(new ResponseMessageSimulator(null), null, true);
            eventArgs.IsBatchPart.Should().BeTrue();
            eventArgs = new ReceivingResponseEventArgs(new ResponseMessageSimulator(null), null, false);
            eventArgs.IsBatchPart.Should().BeFalse();
        }

        [Fact]
        public void DescriptorIsFromConstructor()
        {
            var descriptor = new FunctionDescriptor();
            var eventArgs = new ReceivingResponseEventArgs(new ResponseMessageSimulator(null), descriptor);
            eventArgs.Descriptor.Should().BeSameAs(descriptor);
        }
    }

    internal class ResponseMessageSimulator : IODataResponseMessage
    {
        public ResponseMessageSimulator(IEnumerable<KeyValuePair<string, string>> headers)
        {
            this.Headers = headers;
        }

        public IEnumerable<KeyValuePair<string, string>> Headers { get; private set; }
        public int StatusCode { get; set; }

        public string GetHeader(string headerName)
        {
            throw new NotImplementedException();
        }

        public void SetHeader(string headerName, string headerValue)
        {
            throw new NotImplementedException();
        }

        public Stream GetStream()
        {
            throw new NotImplementedException();
        }
    }
}
