//---------------------------------------------------------------------
// <copyright file="UpdateTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Performance
{
    using System;
    using global::Xunit;
    using Microsoft.OData.Core;
    using Microsoft.Xunit.Performance;

    public class UpdateTests : IClassFixture<TestServiceFixture<UpdateTests>>
    {
        TestServiceFixture<UpdateTests> serviceFixture;

        private const string NameSpacePrefix = "Microsoft.Test.OData.Services.PerfService.";

        public UpdateTests(TestServiceFixture<UpdateTests> serviceFixture)
        {
            this.serviceFixture = serviceFixture;
        }

        [Benchmark]
        public void PostEntity_100()
        {
            int RequestsPerIteration = 100;

            foreach (var iteration in Benchmark.Iterations)
            {
                int PersonIdBase = 100;

                ResetDataSource();

                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < RequestsPerIteration; i++)
                    {
                        ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings()
                        {
                            PayloadBaseUri = serviceFixture.ServiceBaseUri
                        };
                        writerSettings.ODataUri = new ODataUri() {ServiceRoot = serviceFixture.ServiceBaseUri};
                        var requestMessage =
                            new HttpWebRequestMessage(
                                new Uri(serviceFixture.ServiceBaseUri.AbsoluteUri + "SimplePeopleSet", UriKind.Absolute));
                        requestMessage.Method = "POST";

                        var peopleEntry = new ODataEntry()
                        {
                            EditLink = new Uri("/SimplePeopleSet(" + (PersonIdBase++) + ")", UriKind.Relative),
                            Id = new Uri("/SimplePeopleSet(" + PersonIdBase + ")", UriKind.Relative),
                            TypeName = NameSpacePrefix + "Person",
                            Properties = new[]
                            {
                                new ODataProperty {Name = "PersonID", Value = PersonIdBase},
                                new ODataProperty {Name = "FirstName", Value = "PostEntity"},
                                new ODataProperty {Name = "LastName", Value = "PostEntity"},
                                new ODataProperty {Name = "MiddleName", Value = "PostEntity"}
                            },
                        };

                        using (var messageWriter = new ODataMessageWriter(requestMessage, writerSettings))
                        {
                            var odataWriter = messageWriter.CreateODataEntryWriter();
                            odataWriter.WriteStart(peopleEntry);
                            odataWriter.WriteEnd();
                        }

                        var responseMessage = requestMessage.GetResponse();
                        Assert.Equal(201, responseMessage.StatusCode);
                    }
                }
            }
        }

        [Benchmark]
        public void DeleteEntity_100()
        {
            int RequestsPerIteration = 100;

            foreach (var iteration in Benchmark.Iterations)
            {
                int PersonIdBase = 1;

                ResetDataSource();

                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < RequestsPerIteration; i++)
                    {
                        ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings() { PayloadBaseUri = serviceFixture.ServiceBaseUri };
                        writerSettings.ODataUri = new ODataUri() { ServiceRoot = serviceFixture.ServiceBaseUri };
                        var requestMessage = new HttpWebRequestMessage(new Uri(serviceFixture.ServiceBaseUri.AbsoluteUri + "SimplePeopleSet(" + (PersonIdBase++) + ")", UriKind.Absolute));
                        requestMessage.Method = "DELETE";
                        var responseMessage = requestMessage.GetResponse();
                        Assert.Equal(204, responseMessage.StatusCode);
                    }
                }
            }
        }

        [Benchmark]
        public void PutEntity_100()
        {
            int RequestsPerIteration = 100;

            foreach (var iteration in Benchmark.Iterations)
            {
                int PersonIdBase = 1;

                ResetDataSource();

                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < RequestsPerIteration; i++)
                    {
                        ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings() { PayloadBaseUri = serviceFixture.ServiceBaseUri };
                        writerSettings.ODataUri = new ODataUri() { ServiceRoot = serviceFixture.ServiceBaseUri };
                        var requestMessage = new HttpWebRequestMessage(new Uri(serviceFixture.ServiceBaseUri.AbsoluteUri + "SimplePeopleSet(" + PersonIdBase + ")", UriKind.Absolute));
                        requestMessage.Method = "PUT";

                        var peopleEntry = new ODataEntry()
                        {
                            Properties = new[] 
                        {
                            new ODataProperty { Name = "PersonID", Value = PersonIdBase++ },
                            new ODataProperty { Name = "FirstName", Value = "PostEntity" },
                            new ODataProperty { Name = "LastName", Value = "PostEntity" },
                            new ODataProperty { Name = "MiddleName", Value = "NewMiddleName" }
                        },
                        };

                        using (var messageWriter = new ODataMessageWriter(requestMessage, writerSettings))
                        {
                            var odataWriter = messageWriter.CreateODataEntryWriter();
                            odataWriter.WriteStart(peopleEntry);
                            odataWriter.WriteEnd();
                        }
                        var responseMessage = requestMessage.GetResponse();
                        Assert.Equal(204, responseMessage.StatusCode);
                    }
                }
            }
        }

        [Benchmark]
        public void PatchEntity_100()
        {
            int RequestsPerIteration = 100;

            foreach (var iteration in Benchmark.Iterations)
            {
                int PersonIdBase = 1;

                ResetDataSource();

                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < RequestsPerIteration; i++)
                    {
                        ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings() { PayloadBaseUri = serviceFixture.ServiceBaseUri };
                        writerSettings.ODataUri = new ODataUri() { ServiceRoot = serviceFixture.ServiceBaseUri };
                        var requestMessage = new HttpWebRequestMessage(new Uri(serviceFixture.ServiceBaseUri.AbsoluteUri + "SimplePeopleSet(" + (PersonIdBase++) + ")", UriKind.Absolute));
                        requestMessage.Method = "PATCH";

                        var peopleEntry = new ODataEntry()
                        {
                            Properties = new[] 
                        {
                            new ODataProperty { Name = "MiddleName", Value = "NewMiddleName" }
                        },
                        };

                        using (var messageWriter = new ODataMessageWriter(requestMessage, writerSettings))
                        {
                            var odataWriter = messageWriter.CreateODataEntryWriter();
                            odataWriter.WriteStart(peopleEntry);
                            odataWriter.WriteEnd();
                        }
                        var responseMessage = requestMessage.GetResponse();
                        Assert.Equal(204, responseMessage.StatusCode);
                    }
                }
            }
        }

        private void ResetDataSource()
        {
            ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings() { PayloadBaseUri = serviceFixture.ServiceBaseUri };
            writerSettings.ODataUri = new ODataUri() { ServiceRoot = serviceFixture.ServiceBaseUri };
            var requestMessage = new HttpWebRequestMessage(new Uri(serviceFixture.ServiceBaseUri.AbsoluteUri + "ResetDataSource", UriKind.Absolute));
            requestMessage.Method = "POST";
            requestMessage.ContentLength = 0;
            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(204, responseMessage.StatusCode);
        }
    }
}
