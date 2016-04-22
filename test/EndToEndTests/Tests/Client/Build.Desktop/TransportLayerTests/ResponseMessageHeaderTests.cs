//---------------------------------------------------------------------
// <copyright file="ResponseMessageHeaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.TransportLayerTests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Text;
    using Microsoft.OData.Core;
    using Microsoft.OData.Client;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ResponseMessageHeaderTests : EndToEndTestBase
    {
        public ResponseMessageHeaderTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {
        }

        [TestMethod]
        public void LocationUrlWithoutLocalPathShouldWork()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                {"Content-Type", "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8"},
                {"Preference-Applied", "odata.include-annotations=\"*\""},
                {"Location", "http://bar.com"}
            };

            PostWithCustomMessage(headers);
        }

        [TestMethod]
        public void ODataEntityIdAndLocationWithoutLocalPathShouldWork()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                {"Content-Type", "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8"},
                {"Preference-Applied", "odata.include-annotations=\"*\""},
                {"OData-EntityId", "http://foo.com"},
                {"Location", "http://bar.com"}
            };

            PostWithCustomMessage(headers);
        }

        private void PostWithCustomMessage(Dictionary<string, string> headers)
        {
            var context = this.CreateWrappedContext<InMemoryEntities>().Context;
            string response = @"{""@odata.context"":""http://host/root/$metadata#People/$entity"",""@odata.type"":""#Microsoft.Test.OData.Services.ODataWCFService.Customer"",""PersonID"":999,""FirstName"":""Bob"",""LastName"":""Cat"",""MiddleName"":null,""HomeAddress"":{""@odata.type"":""#Microsoft.Test.OData.Services.ODataWCFService.HomeAddress"",""Street"":""1 Microsoft Way"",""City"":""Tokyo"",""PostalCode"":""98052"",""FamilyName"":""Cats""},""Home"":{""type"":""Point"",""coordinates"":[23.1,32.1],""crs"":{""type"":""name"",""properties"":{""name"":""EPSG:4326""}}},""Numbers"":[""111-111-1111"",""012"",""310"",""bca"",""ayz""],""Emails"":[""abc@abc.com""],""Addresses"":[{""@odata.type"":""#Microsoft.Test.OData.Services.ODataWCFService.HomeAddress"",""Street"":""1 Microsoft Way"",""City"":""Tokyo"",""PostalCode"":""98052"",""FamilyName"":""Cats""},{""Street"":""999 Zixing Road"",""City"":""Shanghai"",""PostalCode"":""200000""}],""City"":""London"",""Birthday"":""1957-04-03T00:00:00+08:00"",""TimeBetweenLastTwoOrders"":""PT0.0000001S""}";

            context.Configurations.RequestPipeline.OnMessageCreating = (args) => new CustomizedRequestMessage(
                args,
                response,
                headers);

            var person = new Person
            {
                PersonID = 0,
                FirstName = "Smith",
                LastName = "John",
                MiddleName = "D",
                HomeAddress = new HomeAddress
                {
                    Street = "999 Zixing Road",
                    City = "Shanghai",
                    PostalCode = "200241",
                    FamilyName = "Cats"
                },
                Home = GeographyPoint.Create(32.1, 23.1),
                Numbers = new ObservableCollection<string>(),
                Emails = new ObservableCollection<string>(),
                Addresses = new ObservableCollection<Address>()
            };

            context.AddToPeople(person);
            context.SaveChanges();
        }
    }

    class CustomizedRequestMessage : HttpWebRequestMessage
    {
        public string Response { get; set; }
        public Dictionary<string, string> CustomizedHeaders { get; set; }

        public CustomizedRequestMessage(DataServiceClientRequestMessageArgs args)
            : base(args)
        {
        }

        public CustomizedRequestMessage(DataServiceClientRequestMessageArgs args, string response, Dictionary<string, string> headers)
            : base(args)
        {
            this.Response = response;
            this.CustomizedHeaders = headers;
        }

        public override IODataResponseMessage GetResponse()
        {
            return new HttpWebResponseMessage(
                this.CustomizedHeaders,
                200,
                () =>
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(this.Response);
                    return new MemoryStream(byteArray);
                });
        }
    }
}
