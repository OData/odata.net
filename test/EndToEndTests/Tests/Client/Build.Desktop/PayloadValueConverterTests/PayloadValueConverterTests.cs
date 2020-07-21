//---------------------------------------------------------------------
// <copyright file="PluggableFormatServiceTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.ODataWCFServiceTests
{
    using System;
    using System.IO;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.PluggableFormatServiceReference;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Xunit;

    /// <summary>
    /// Tests for pluggable format service
    /// </summary>
    public class PayloadValueConverterTests : ODataWCFServiceTestsBase<PluggableFormatService>
    {
        public PayloadValueConverterTests()
            : base(ServiceDescriptors.PayloadValueConverterServiceDescriptor)
        {
        }

        [Fact]
        public void PostAndQuerySingleBinaryProperty()
        {
            var person = new Person
            {
                Picture = new byte[] { 3, 1, 4 },
            };
            this.TestClientContext.AddToPeople(person);
            this.TestClientContext.SaveChanges();

            var requestMessage =
                new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "People(" + person.Id + ")/Picture", UriKind.Absolute));
            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);
            var dat = new StreamReader(responseMessage.GetStream()).ReadToEnd();
            Assert.True(dat.Contains("\"value\":\"3-1-4\""));
        }
    }
}
