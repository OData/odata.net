//---------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
namespace Microsoft.OData.Tests.Nuget.Core
{
    using System;
    using System.Xml;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Xunit;

    public class CoreLibTest
    {
        static Uri baseUri = new Uri("http://services.odata.org/V4/(S(f1yueljyzoy0bfv5deqdqkdq))/TrippinServiceRW/");
        static string nameSpace = "Microsoft.OData.SampleService.Models.TripPin";
        static IEdmModel Model { get; set; }

        [Fact]
        public void BasicTest()
        {
            Model =  GetModel();

            // create entry and insert
            var personEntry = new ODataResource() { TypeName = nameSpace + ".Person" };
            var userName = new ODataProperty { Name = "UserName", Value = "Test" };
            var firstName = new ODataProperty { Name = "FirstName", Value = "Test" };
            var lastName = new ODataProperty { Name = "LastName", Value = "1" };
            var emails = new ODataProperty { Name = "Emails", Value = new ODataCollectionValue() };

            personEntry.Properties = new[] { userName, firstName, lastName, emails };

            var settings = new ODataMessageWriterSettings();
            settings.BaseUri = baseUri;

            var orderType = Model.FindDeclaredType(nameSpace + ".Person") as IEdmEntityType;
            var orderSet = Model.EntityContainer.FindEntitySet("People");

            var requestMessage = new HttpWebRequestMessage(new Uri(settings.BaseUri + "People"));
            requestMessage.SetHeader("Content-Type", "application/json");
            requestMessage.SetHeader("Accept", "application/json");
            requestMessage.Method = "POST";
            using (var messageWriter = new ODataMessageWriter(requestMessage, settings))
            {
                var odataWriter = messageWriter.CreateODataResourceWriter(orderSet, orderType);
                odataWriter.WriteStart(personEntry);
                odataWriter.WriteEnd();
            }

            var responseMessage = requestMessage.GetResponse();

            int expectedStatus = 201;
            Assert.True(expectedStatus.Equals(responseMessage.StatusCode));
        }

        public IEdmModel GetModel()
        {
            HttpWebRequestMessage message = new HttpWebRequestMessage(new Uri(baseUri.AbsoluteUri + "$metadata", UriKind.Absolute));
            message.SetHeader("Accept", "application/xml");

            using (var messageReader = new ODataMessageReader(message.GetResponse()))
            {
                Func<Uri, XmlReader> getReferencedSchemaFunc = uri =>
                {
                    HttpWebRequestMessage msg = new HttpWebRequestMessage(new Uri(uri.AbsoluteUri, UriKind.Absolute));
                    return XmlReader.Create(msg.GetResponse().GetStream());
                };
                return messageReader.ReadMetadataDocument(getReferencedSchemaFunc);
            }
        }
    }
}
