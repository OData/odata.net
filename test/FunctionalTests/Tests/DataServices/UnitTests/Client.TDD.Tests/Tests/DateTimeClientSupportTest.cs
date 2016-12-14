﻿//---------------------------------------------------------------------
// <copyright file="DateTimeClientSupportTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
namespace Microsoft.OData.Client.TDDUnitTests.Tests
{
    using Edm.Library;
    using FluentAssertions;
    using Microsoft.OData.Client;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Test.OData.Services.TestServices.DateTimePrimitiveTypeReference;
    using pkr = Microsoft.Test.OData.Services.TestServices.DateTimePrimitiveTypeReference;

    [TestClass]
    public class DateTimeClientSupportTest
    {
        pkr.TestContext dsc_dateTime = new pkr.TestContext(new Uri("http://odataService/"));
        IEnumerable<MergeOption> mergeOptions = Enum.GetValues(typeof(MergeOption)).Cast<MergeOption>();

        const string SingleEntityPayloadWithDateTimeResponse = @"{
""@odata.context"":""http://odataService/$metadata#EdmDateTimeSet/$entity"",
""Id"":""9999-12-31T23:59:59.9999999Z"",""DateTimeCollection"":[""0001-01-01T00:00:00Z"",""9999-12-31T23:59:59.9999999Z"",""0001-01-01T00:00:00Z""]}";

        const string SingleEntityPayloadWithDateTimeResponse_NonZ = @"{
""@odata.context"":""http://odataService/$metadata#EdmDateTimeSet/$entity"",
""Id"":""2016-11-29T18:07:16-08:00""}";

        const string FeedEntityPayloadWithDateTimeResponse = @"{
""@odata.context"":""http://odataService/$metadata#EdmDateTimeSet"",
""value"":[{""Id"":""0001-01-01T00:00:00Z""},{""Id"":""2016-11-29T18:07:16-08:00""}],
""@odata.nextLink"":""EdmDateTimeSet?$skiptoken=2016-11-02T16%3A08%3A16-08%3A00""}";

        const string ReturnCollectionResponse = @"{
""@odata.context"":""http://odataService/$metadata#EdmDateTimeSet(0001-01-01T00%3A00%3A00Z)/DateTimeCollection"",
""value"":[""0001-01-01T00:00:00Z"",""2016-11-29T18:07:16-08:00"",""9999-12-31T23:59:59.9999999Z""]
}";
        const string ReturnCollectionResponse1 = @"{
""@odata.context"":""http://odataService/$metadata#Collection(Edm.DateTimeOffset)"",
""value"":[""0001-01-01T00:00:00Z"",""2016-11-29T18:07:16-08:00"",""9999-12-31T23:59:59.9999999Z""]
}";

        const string ReturnSingleDateTimeResponse = @"{
""@odata.context"":""http://odataService/$metadata#Edm.DateTimeOffset"",
""value"":""2016-11-29T18:07:16-08:00""
}";
        const string ReturnSingleMaxDateTimeResponse = @"{
""@odata.context"":""http://odataService/$metadata#Edm.DateTimeOffset"",
""value"":""9999-12-31T23:59:59.9999999Z""
}";
        const string ReturnSingleMinDateTimeResponse = @"{
""@odata.context"":""http://odataService/$metadata#Edm.DateTimeOffset"",
""value"":""0001-01-01T00:00:00Z""
}";
        const string ReturnSingleDateTimeResponse1 = @"{
""@odata.context"":""http://odataService/$metadata#EdmDateTimeSet(2016-11-29T18%3A07%3A06-08%3A00)/Id"",
""value"":""2016-11-29T18:07:16-08:00""
}";
        private DateTime fixedDateTimeUtc = new DateTime(2016, 11, 30, 02, 07, 16, DateTimeKind.Utc);

        [TestMethod]
        public void TestClientLibDateTimeSupport()
        {
            TestDateTime(SingleEntityPayloadWithDateTimeResponse, () =>
            {
                var queryResult = ((DataServiceQuery<pkr.EdmDateTime>)dsc_dateTime.CreateQuery<pkr.EdmDateTime>("EdmDateTimeSet").Where(e => e.Id.Equals(DateTime.Now)));
                var result = queryResult.ToArray();
                Assert.AreEqual(1, result.Count(), "Expected a single result");
                Assert.AreEqual(result[0].Id, DateTime.MaxValue);
            });

            TestDateTime(FeedEntityPayloadWithDateTimeResponse, () =>
            {
                var queryResult = ((DataServiceQuery<pkr.EdmDateTime>)dsc_dateTime.CreateQuery<pkr.EdmDateTime>("EdmDateTimeSet").Where(e => e.Id >= (DateTime.Now)));
                var result = queryResult.ToArray();
                Assert.AreEqual(2, result.Count(), "Expected two results");
                Assert.AreEqual(result[0].Id, DateTime.MinValue);
                Assert.AreEqual(result[1].Id, this.fixedDateTimeUtc);
            });

            TestDateTime(FeedEntityPayloadWithDateTimeResponse, () =>
            {
                var queryResult = ((DataServiceQuery<pkr.EdmDateTime>)dsc_dateTime.CreateQuery<pkr.EdmDateTime>("EdmDateTimeSet"));
                var result = queryResult.ToList();
                Assert.AreEqual(2, result.Count(), "Expected 2 result");
                Assert.AreEqual(result[0].Id, DateTime.MinValue);
                Assert.AreEqual(result[1].Id, this.fixedDateTimeUtc);
            });

            TestDateTime(ReturnCollectionResponse, () =>
            {
                var queryResult = dsc_dateTime.EdmDateTimeSet.Where(e => e.Id == (DateTime.MinValue)).Select(e => e.DateTimeCollection).FirstOrDefault();
                var result = queryResult.ToList();
                Assert.IsTrue(result.Count == 3);
                Assert.AreEqual(result[0], DateTime.MinValue);
                Assert.AreEqual(result[1], this.fixedDateTimeUtc);
                Assert.AreEqual(result[2], DateTime.MaxValue);
            });

            TestDateTime(ReturnCollectionResponse1, () =>
            {
                var queryResult = dsc_dateTime.EdmDateTimeSet.Where(e => e.Id == (DateTime.MinValue)).Select(e => e.DateTimeCollection).FirstOrDefault();
                var result = queryResult.ToList();
                Assert.IsTrue(result.Count == 3);
                Assert.AreEqual(result[0], DateTime.MinValue);
                Assert.AreEqual(result[1], this.fixedDateTimeUtc);
                Assert.AreEqual(result[2], DateTime.MaxValue);
            });

            TestDateTime(ReturnSingleDateTimeResponse, () =>
            {
                var queryResult = dsc_dateTime.EdmDateTimeSet.Where(e => e.Id == (DateTime.MinValue)).Select(e => e.Id).FirstOrDefault();
                Assert.AreEqual(queryResult, this.fixedDateTimeUtc);
            });

            TestDateTime(ReturnSingleMaxDateTimeResponse, () =>
            {
                var queryResult = dsc_dateTime.EdmDateTimeSet.Where(e => e.Id == (DateTime.MinValue)).Select(e => e.Id).FirstOrDefault();
                Assert.AreEqual(queryResult, DateTime.MaxValue);
            });

            TestDateTime(ReturnSingleMinDateTimeResponse, () =>
            {
                var queryResult = dsc_dateTime.EdmDateTimeSet.Where(e => e.Id == (DateTime.MinValue)).Select(e => e.Id).FirstOrDefault();
                Assert.AreEqual(queryResult, DateTime.MinValue);
            });

            TestDateTime(ReturnSingleDateTimeResponse1, () =>
            {
                var expected_local = fixedDateTimeUtc.ToLocalTime();
                var queryResult = dsc_dateTime.EdmDateTimeSet.Where(e => e.Id == expected_local).Select(e => e.Id).FirstOrDefault();
                Assert.AreEqual(queryResult, fixedDateTimeUtc);
            });
        }

        [TestMethod]
        public void TestClientLibDateTimeSupport_Post()
        {
            TestDateTime(SingleEntityPayloadWithDateTimeResponse, () =>
            {
                bool payloadChecked = false;
                var dateTimeEntity = new EdmDateTime();
                dateTimeEntity.Id = DateTime.MaxValue;
                dateTimeEntity.DateTimeCollection = new ObservableCollection<DateTime>() { DateTime.MaxValue, DateTime.UtcNow, DateTime.MinValue };
                dsc_dateTime.AddToEdmDateTimeSet(dateTimeEntity);

                dsc_dateTime.Configurations.RequestPipeline.OnEntryEnding((args) =>
                {
                    var properties = args.Entry.Properties;
                    Assert.IsTrue(properties.Count() == 2);
                    var dateTimePropertyInEntry = args.Entry.Properties.Where(p => p.Name == "Id").FirstOrDefault().Value;
                    Assert.IsTrue(dateTimePropertyInEntry is DateTimeOffset);
                    payloadChecked = true;
                });

                try
                {
                    dsc_dateTime.SaveChanges();
                }
                catch (InvalidOperationException)
                {
                    // The invalid operation exception is expected as there is no real service
                    // We only need to validate the request message.
                    Assert.IsTrue(payloadChecked);
                }
            });
        }

        [TestMethod]
        public void WriteEntryAsBodyOperationParameter()
        {
            var requestInfo = new RequestInfo(dsc_dateTime);
            var serializer = new Serializer(requestInfo);

            EdmDateTime dateTimeEntity = new EdmDateTime()
            {
                Id = DateTime.MinValue                
            };

            dateTimeEntity.DateTimeCollection = new ObservableCollection<DateTime>() { this.fixedDateTimeUtc, DateTime.MinValue, DateTime.MaxValue };
            
            // Parameter has the entity with dateTime as Id
            List<BodyOperationParameter> parameters = new List<BodyOperationParameter> { new BodyOperationParameter("entityWithDateTimeProperty", dateTimeEntity) };
            ODataRequestMessageWrapper requestMessage = CreateRequestMessageForPost(requestInfo);
            serializer.WriteBodyOperationParameters(parameters, requestMessage);
            const string parameterString = @"{""entityWithDateTimeProperty"":{""@odata.type"":""#Microsoft.Test.OData.Services.PrimitiveKeysService.EdmDateTime"",""DateTimeCollection@odata.type"":""#Collection(DateTimeOffset)"",""DateTimeCollection"":[""2016-11-30T02:07:16Z"",""0001-01-01T00:00:00Z"",""9999-12-31T23:59:59.9999999Z""],""Id@odata.type"":""#DateTimeOffset"",""Id"":""0001-01-01T00:00:00Z""}}";
            VerifyMessageBody(requestMessage, parameterString);

            // Parameter has DateTime value
            var dateTimeValue2 = DateTime.MinValue;
            List<BodyOperationParameter> parameters2 = new List<BodyOperationParameter> { new BodyOperationParameter("entityWithDateTimeProperty", dateTimeEntity), new BodyOperationParameter("datetimeParameter2", dateTimeValue2) };
            ODataRequestMessageWrapper requestMessage2 = CreateRequestMessageForPost(requestInfo);
            serializer.WriteBodyOperationParameters(parameters2, requestMessage2);
            const string parameterString2 = @"{""entityWithDateTimeProperty"":{""@odata.type"":""#Microsoft.Test.OData.Services.PrimitiveKeysService.EdmDateTime"",""DateTimeCollection@odata.type"":""#Collection(DateTimeOffset)"",""DateTimeCollection"":[""2016-11-30T02:07:16Z"",""0001-01-01T00:00:00Z"",""9999-12-31T23:59:59.9999999Z""],""Id@odata.type"":""#DateTimeOffset"",""Id"":""0001-01-01T00:00:00Z""},""datetimeParameter2"":""0001-01-01T00:00:00Z""}";
            VerifyMessageBody(requestMessage2, parameterString2);

            // add collection case, add a parameter in model
            var dateTimeValue3 = new ObservableCollection<DateTime>() {this.fixedDateTimeUtc};
            List<BodyOperationParameter> parameters3 = new List<BodyOperationParameter> {
                new BodyOperationParameter("entityWithDateTimeProperty", dateTimeEntity),
                new BodyOperationParameter("datetimeParameter2", dateTimeValue2),
                new BodyOperationParameter("collectionDateTime", dateTimeValue3) };

            ODataRequestMessageWrapper requestMessage3 = CreateRequestMessageForPost(requestInfo);
            serializer.WriteBodyOperationParameters(parameters3, requestMessage3);
            const string parameterString3 = @"{""entityWithDateTimeProperty"":{""@odata.type"":""#Microsoft.Test.OData.Services.PrimitiveKeysService.EdmDateTime"",""DateTimeCollection@odata.type"":""#Collection(DateTimeOffset)"",""DateTimeCollection"":[""2016-11-30T02:07:16Z"",""0001-01-01T00:00:00Z"",""9999-12-31T23:59:59.9999999Z""],""Id@odata.type"":""#DateTimeOffset"",""Id"":""0001-01-01T00:00:00Z""},""datetimeParameter2"":""0001-01-01T00:00:00Z"",""collectionDateTime"":[""2016-11-30T02:07:16Z""]}";
            VerifyMessageBody(requestMessage3, parameterString3);
        }

        [TestMethod]
        public void WriteEntryAsBodyOperationParameter_MaxValue()
        {
            var requestInfo = new RequestInfo(dsc_dateTime);
            var serializer = new Serializer(requestInfo);

            EdmDateTime dateTimeEntity = new EdmDateTime()
            {
                Id = DateTime.MaxValue
            };

            dateTimeEntity.DateTimeCollection = new ObservableCollection<DateTime>() { this.fixedDateTimeUtc };

            // Parameter has the entity with dateTime as Id
            List<BodyOperationParameter> parameters = new List<BodyOperationParameter> { new BodyOperationParameter("entityWithDateTimeProperty", dateTimeEntity) };
            ODataRequestMessageWrapper requestMessage = CreateRequestMessageForPost(requestInfo);
            serializer.WriteBodyOperationParameters(parameters, requestMessage);
            const string parameterString = @"{""entityWithDateTimeProperty"":{""@odata.type"":""#Microsoft.Test.OData.Services.PrimitiveKeysService.EdmDateTime"",""DateTimeCollection@odata.type"":""#Collection(DateTimeOffset)"",""DateTimeCollection"":[""2016-11-30T02:07:16Z""],""Id@odata.type"":""#DateTimeOffset"",""Id"":""9999-12-31T23:59:59.9999999Z""}}";
            VerifyMessageBody(requestMessage, parameterString);

            // Parameter has DateTime value
            var dateTimeValue2 = DateTime.MaxValue;
            List<BodyOperationParameter> parameters2 = new List<BodyOperationParameter> { new BodyOperationParameter("entityWithDateTimeProperty", dateTimeEntity), new BodyOperationParameter("datetimeParameter2", dateTimeValue2) };
            ODataRequestMessageWrapper requestMessage2 = CreateRequestMessageForPost(requestInfo);
            serializer.WriteBodyOperationParameters(parameters2, requestMessage2);
            const string parameterString2 = @"{""entityWithDateTimeProperty"":{""@odata.type"":""#Microsoft.Test.OData.Services.PrimitiveKeysService.EdmDateTime"",""DateTimeCollection@odata.type"":""#Collection(DateTimeOffset)"",""DateTimeCollection"":[""2016-11-30T02:07:16Z""],""Id@odata.type"":""#DateTimeOffset"",""Id"":""9999-12-31T23:59:59.9999999Z""},""datetimeParameter2"":""9999-12-31T23:59:59.9999999Z""}";
            VerifyMessageBody(requestMessage2, parameterString2);

            // add collection case, add a parameter in model
            var dateTimeValue3 = new ObservableCollection<DateTime>() { DateTime.MaxValue };
            List<BodyOperationParameter> parameters3 = new List<BodyOperationParameter> {
                new BodyOperationParameter("entityWithDateTimeProperty", dateTimeEntity),
                new BodyOperationParameter("datetimeParameter2", dateTimeValue2),
                new BodyOperationParameter("collectionDateTime", dateTimeValue3) };

            ODataRequestMessageWrapper requestMessage3 = CreateRequestMessageForPost(requestInfo);
            serializer.WriteBodyOperationParameters(parameters3, requestMessage3);
            const string parameterString3 = @"{""entityWithDateTimeProperty"":{""@odata.type"":""#Microsoft.Test.OData.Services.PrimitiveKeysService.EdmDateTime"",""DateTimeCollection@odata.type"":""#Collection(DateTimeOffset)"",""DateTimeCollection"":[""2016-11-30T02:07:16Z""],""Id@odata.type"":""#DateTimeOffset"",""Id"":""9999-12-31T23:59:59.9999999Z""},""datetimeParameter2"":""9999-12-31T23:59:59.9999999Z"",""collectionDateTime"":[""9999-12-31T23:59:59.9999999Z""]}";
            VerifyMessageBody(requestMessage3, parameterString3);
        }

        [TestMethod]
        public void WriteDateAndTimeUriOperationParametersToUriShouldReturnUri()
        {
            var requestInfo = new RequestInfo(dsc_dateTime);
            var serializer = new Serializer(requestInfo);
            Uri requestUri = new Uri("http://odataService");

            EdmDateTime dateTimeEntity = new EdmDateTime()
            {
                Id = DateTime.MinValue
            };
            dateTimeEntity.DateTimeCollection = new ObservableCollection<DateTime>() { this.fixedDateTimeUtc };

            TestDateTime(SingleEntityPayloadWithDateTimeResponse_NonZ, () =>
            {
                // Parameter is an entity with DateTime property
                List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("dateTimeEntity", dateTimeEntity) };
                requestUri = serializer.WriteUriOperationParametersToUri(requestUri, parameters);

                // Validate request Uri
                const string uriString = @"http://odataservice/(dateTimeEntity=@dateTimeEntity)?@dateTimeEntity={""@odata.type"":""%23Microsoft.Test.OData.Services.PrimitiveKeysService.EdmDateTime"",""DateTimeCollection@odata.type"":""%23Collection(DateTimeOffset)"",""DateTimeCollection"":[""2016-11-30T02:07:16Z""],""Id@odata.type"":""%23DateTimeOffset"",""Id"":""0001-01-01T00:00:00Z""}";
                Assert.AreEqual(uriString, requestUri.ToString());

                // Validate return 
                var queryResult = ((DataServiceQuery<pkr.EdmDateTime>)dsc_dateTime.CreateQuery<pkr.EdmDateTime>("EdmDateTimeSet").Where(e => e.Id.Equals(DateTime.MinValue)));
                var result = queryResult.ToArray();
                Assert.AreEqual(1, result.Count(), "Expected a single result");
                var expected = new DateTime(2016, 11, 30, 02, 07, 16);

                // kind, value
                Assert.AreEqual(result[0].Id, expected);
                Assert.AreEqual(result[0].Id.Kind, DateTimeKind.Utc);
            });

            //return datetime
            TestDateTime(ReturnSingleDateTimeResponse, () =>
            {
                requestUri = new Uri("http://odataService");

                // parameter is datetime
                List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("dateTimeValue", this.fixedDateTimeUtc)};
                requestUri = serializer.WriteUriOperationParametersToUri(requestUri, parameters);

                // Validate request Uri
                const string uriString = @"http://odataservice/(dateTimeValue=2016-11-30T02:07:16Z)";
                Assert.AreEqual(uriString, requestUri.ToString());

                var queryResult = dsc_dateTime.EdmDateTimeSet.Where(e => e.Id == (DateTime.MinValue)).Select(e => e.Id).FirstOrDefault();
                var expected = new DateTime(2016, 11, 30, 02, 07, 16, DateTimeKind.Utc);
                Assert.AreEqual(queryResult, expected);
            });

            //return datetime
            TestDateTime(ReturnSingleMaxDateTimeResponse, () =>
            {
                requestUri = new Uri("http://odataService");

                // parameter is datetime
                List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("dateTimeValue", this.fixedDateTimeUtc) };
                requestUri = serializer.WriteUriOperationParametersToUri(requestUri, parameters);

                // Validate request Uri
                const string uriString = @"http://odataservice/(dateTimeValue=2016-11-30T02:07:16Z)";
                Assert.AreEqual(uriString, requestUri.ToString());

                var queryResult = dsc_dateTime.EdmDateTimeSet.Where(e => e.Id == (DateTime.MinValue)).Select(e => e.Id).FirstOrDefault();
                Assert.AreEqual(queryResult, DateTime.MaxValue);
            });

            //return datetime
            TestDateTime(ReturnSingleMinDateTimeResponse, () =>
            {
                requestUri = new Uri("http://odataService");

                // parameter is datetime
                List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("dateTimeValue", this.fixedDateTimeUtc) };
                requestUri = serializer.WriteUriOperationParametersToUri(requestUri, parameters);

                // Validate request Uri
                const string uriString = @"http://odataservice/(dateTimeValue=2016-11-30T02:07:16Z)";
                Assert.AreEqual(uriString, requestUri.ToString());

                var queryResult = dsc_dateTime.EdmDateTimeSet.Where(e => e.Id == (DateTime.MinValue)).Select(e => e.Id).FirstOrDefault();
                Assert.AreEqual(queryResult, DateTime.MinValue);
            });

            // return collection(datetime)
            TestDateTime(ReturnCollectionResponse, () =>
            {
                // parameter is Collection<dateTime>
                requestUri = new Uri("http://odataService");
                var dateTimeCollection = new ObservableCollection<DateTime>() { this.fixedDateTimeUtc };
                List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("dateTimeCollection", dateTimeCollection) };
                requestUri = serializer.WriteUriOperationParametersToUri(requestUri, parameters);

                // Validate request Uri
                const string uriString = @"http://odataservice/(dateTimeCollection=@dateTimeCollection)?@dateTimeCollection=[""2016-11-30T02:07:16Z""]";
                Assert.AreEqual(uriString, requestUri.ToString());

                var queryResult = dsc_dateTime.EdmDateTimeSet.Where(e => e.Id == (DateTime.MinValue)).Select(e => e.DateTimeCollection).FirstOrDefault();
                var result = queryResult.ToList();
                Assert.IsTrue(result.Count == 3);
                Assert.AreEqual(result[0], DateTime.MinValue);
                Assert.AreEqual(result[1], this.fixedDateTimeUtc);
                Assert.AreEqual(result[2], DateTime.MaxValue);
            });
        }

        [TestMethod]
        public void WriteDateAndTimeUriOperationParametersToUriShouldReturnUri_MaxValueId()
        {
            var requestInfo = new RequestInfo(dsc_dateTime);
            var serializer = new Serializer(requestInfo);
            Uri requestUri = new Uri("http://odataService");

            EdmDateTime dateTimeEntity = new EdmDateTime()
            {
                Id = DateTime.MaxValue
            };
            dateTimeEntity.DateTimeCollection = new ObservableCollection<DateTime>() { this.fixedDateTimeUtc, DateTime.MinValue, DateTime.MaxValue };

            TestDateTime(SingleEntityPayloadWithDateTimeResponse_NonZ, () =>
            {
                // Parameter is an entity with DateTime property
                List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("dateTimeEntity", dateTimeEntity) };
                requestUri = serializer.WriteUriOperationParametersToUri(requestUri, parameters);

                // Validate request Uri
                const string uriString = @"http://odataservice/(dateTimeEntity=@dateTimeEntity)?@dateTimeEntity={""@odata.type"":""%23Microsoft.Test.OData.Services.PrimitiveKeysService.EdmDateTime"",""DateTimeCollection@odata.type"":""%23Collection(DateTimeOffset)"",""DateTimeCollection"":[""2016-11-30T02:07:16Z"",""0001-01-01T00:00:00Z"",""9999-12-31T23:59:59.9999999Z""],""Id@odata.type"":""%23DateTimeOffset"",""Id"":""9999-12-31T23:59:59.9999999Z""}";
                Assert.AreEqual(uriString, requestUri.ToString());

                // Validate return 
                var queryResult = ((DataServiceQuery<pkr.EdmDateTime>)dsc_dateTime.CreateQuery<pkr.EdmDateTime>("EdmDateTimeSet").Where(e => e.Id.Equals(DateTime.MinValue)));
                var result = queryResult.ToArray();
                Assert.AreEqual(1, result.Count(), "Expected a single result");
                var expected = new DateTime(2016, 11, 30, 02, 07, 16);

                // kind, value
                Assert.AreEqual(result[0].Id, expected);
                Assert.AreEqual(result[0].Id.Kind, DateTimeKind.Utc);
            });

            //return datetime
            TestDateTime(ReturnSingleDateTimeResponse, () =>
            {
                requestUri = new Uri("http://odataService");

                // parameter is datetime
                List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("dateTimeValue", this.fixedDateTimeUtc) };
                requestUri = serializer.WriteUriOperationParametersToUri(requestUri, parameters);

                // Validate request Uri
                const string uriString = @"http://odataservice/(dateTimeValue=2016-11-30T02:07:16Z)";
                Assert.AreEqual(uriString, requestUri.ToString());

                var queryResult = dsc_dateTime.EdmDateTimeSet.Where(e => e.Id == (DateTime.MinValue)).Select(e => e.Id).FirstOrDefault();
                var expected = new DateTime(2016, 11, 30, 02, 07, 16, DateTimeKind.Utc);
                Assert.AreEqual(queryResult, expected);
            });

            // return collection(datetime)
            TestDateTime(ReturnCollectionResponse, () =>
            {
                // parameter is Collection<dateTime>
                requestUri = new Uri("http://odataService");
                var dateTimeCollection = new ObservableCollection<DateTime>() { this.fixedDateTimeUtc };
                List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("dateTimeCollection", dateTimeCollection) };
                requestUri = serializer.WriteUriOperationParametersToUri(requestUri, parameters);

                // Validate request Uri
                const string uriString = @"http://odataservice/(dateTimeCollection=@dateTimeCollection)?@dateTimeCollection=[""2016-11-30T02:07:16Z""]";
                Assert.AreEqual(uriString, requestUri.ToString());

                var queryResult = dsc_dateTime.EdmDateTimeSet.Where(e => e.Id == (DateTime.MinValue)).Select(e => e.DateTimeCollection).FirstOrDefault();
                var result = queryResult.ToList();
                Assert.IsTrue(result.Count == 3);
                Assert.AreEqual(result[0], DateTime.MinValue);
                Assert.AreEqual(result[1], this.fixedDateTimeUtc);
                Assert.AreEqual(result[2], DateTime.MaxValue);
            });
        }

        private ODataRequestMessageWrapper CreateRequestMessageForPost(RequestInfo requestInfo)
        {
            HeaderCollection headers = new HeaderCollection();
            headers.SetHeader("Content-Type", "application/json;odata.metadata=minimal");

            return ODataRequestMessageWrapper.CreateRequestMessageWrapper(
                new BuildingRequestEventArgs("POST", new Uri("http://odataService/"), headers, null, HttpStack.Auto), requestInfo);
        }

        private void VerifyMessageBody(ODataRequestMessageWrapper requestMessage, string expected)
        {
            MemoryStream stream = (MemoryStream)requestMessage.CachedRequestStream.Stream;
            StreamReader streamReader = new StreamReader(stream);
            String body = streamReader.ReadToEnd();
            Assert.AreEqual(expected, body);
        }

        #region private methods
        private void TestDateTime(string response, Action testAction)
        {
            SetDateTimeResponse(response);
            testAction();
        }


        private void SetDateTimeResponse(string response)
        {
            dsc_dateTime.Configurations.RequestPipeline.OnMessageCreating = (args) =>
            {
                var responseMessage = new CustomizedHttpWebRequestMessage(args,
                response,
                new Dictionary<string, string>()
                {
        {"Content-Type", "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8"},
        { "Content-Length", "409"},
        { "Location", "http://odataService/Products(10001)"},
        { "OData-Version", "4.0"},
                });
                return responseMessage;
            };
        }
        #endregion private methods
    }
}
