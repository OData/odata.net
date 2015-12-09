//---------------------------------------------------------------------
// <copyright file="WritingJsonLightWithoutModelAcceptanceTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.Writer.JsonLight
{
    public class WritingJsonLightWithoutModelAcceptanceTests
    {
        [Fact]
        public void ShouldBeAbleToWriteFeedAndEntryResponseInJsonLightWithoutModel()
        {
            const string expectedPayload =
                "{" +
                    "\"@odata.context\":\"http://www.example.com/$metadata#Customers/NS.VIPCustomer\"," +
                    "\"value\":[" +
                        "{" +
                            "\"Name\":\"Bob\"," +
                            "\"MostRecentOrder@odata.navigationLink\":\"MostRecentOrder\"," +
                            "\"MostRecentOrder\":{\"OrderId\":101}" +
                        "}" +
                    "]" +
                "}";

            var writerSettings = new ODataMessageWriterSettings { DisableMessageStreamDisposal = true };
            writerSettings.SetContentType(ODataFormat.Json);
            writerSettings.ODataUri = new ODataUri() { ServiceRoot = new Uri("http://www.example.com") };

            MemoryStream stream = new MemoryStream();
            IODataResponseMessage responseMessage = new InMemoryMessage { StatusCode = 200, Stream = stream };

            // Write payload
            using (var messageWriter = new ODataMessageWriter(responseMessage, writerSettings))
            {
                var odataWriter = messageWriter.CreateODataFeedWriter();

                // Create customers feed with serializtion info to write odata.metadata.
                var customersFeed = new ODataFeed();
                customersFeed.SetSerializationInfo(new ODataFeedAndEntrySerializationInfo { NavigationSourceName = "Customers", NavigationSourceEntityTypeName = "NS.Customer", ExpectedTypeName = "NS.VIPCustomer" });

                // Write customers feed.
                odataWriter.WriteStart(customersFeed);

                // Write VIP customer
                {
                    // Create VIP customer, don't need to pass in serialization info since the writer knows the context from the feed scope.
                    var vipCustomer = new ODataEntry { TypeName = "NS.VIPCustomer" };

                    var customerKey = new ODataProperty { Name = "Name", Value = "Bob" };

                    // Provide serialization info at the property level to compute the edit link.
                    customerKey.SetSerializationInfo(new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key });
                    vipCustomer.Properties = new[] { customerKey };

                    // Write customer entry.
                    odataWriter.WriteStart(vipCustomer);

                    // Write expanded most recent order
                    {
                        // No API to set serialization info on ODataNavigationLink since what we are adding on ODataFeed and ODataEntry should be sufficient for the 5.5 release.
                        var navigationLink = new ODataNavigationLink { Name = "MostRecentOrder", IsCollection = false, Url = new Uri("MostRecentOrder", UriKind.Relative) };
                        odataWriter.WriteStart(navigationLink);

                        // Write the most recent customer.
                        {
                            var mostRecentOrder = new ODataEntry { TypeName = "NS.Order" };

                            // Add serialization info so we can computer links.
                            mostRecentOrder.SetSerializationInfo(new ODataFeedAndEntrySerializationInfo { NavigationSourceName = "Orders", NavigationSourceEntityTypeName = "NS.Order", ExpectedTypeName = "NS.Order" });

                            var orderKey = new ODataProperty { Name = "OrderId", Value = 101 };

                            // Provide serialization info at the property level to compute the edit link.
                            orderKey.SetSerializationInfo(new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key });
                            mostRecentOrder.Properties = new[] { orderKey };

                            // Write order entry.
                            odataWriter.WriteStart(mostRecentOrder);
                            odataWriter.WriteEnd();
                        }

                        // End navigationLink.
                        odataWriter.WriteEnd();
                    }

                    // End customer entry.
                    odataWriter.WriteEnd();
                }

                // End customers feed.
                odataWriter.WriteEnd();
            }

            stream.Position = 0;
            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be(expectedPayload);
        }
    }
}
