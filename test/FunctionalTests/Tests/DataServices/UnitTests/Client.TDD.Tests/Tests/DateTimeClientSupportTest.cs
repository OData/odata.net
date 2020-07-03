//---------------------------------------------------------------------
// <copyright file="DateTimeClientSupportTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
namespace Microsoft.OData.Client.TDDUnitTests.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using Microsoft.OData.Client;
    using Test.OData.Services.TestServices.DateTimePrimitiveTypeReference;
    using Xunit;
    using pkr = Microsoft.Test.OData.Services.TestServices.DateTimePrimitiveTypeReference;

    public class DateTimeClientSupportTest
    {
        pkr.TestContext dsc_dateTime = new pkr.TestContext(new Uri("http://odataService/"));
        IEnumerable<MergeOption> mergeOptions = Enum.GetValues(typeof(MergeOption)).Cast<MergeOption>();

        const string SingleEntityPayloadWithDateTimeResponse = @"{
""@odata.context"":""http://odataService/$metadata#EdmDateTimeSet/$entity"",
""Id"":""9999-12-31T23:59:59.9999999Z"",""DateTimeCollection"":[""0001-01-01T00:00:00Z"",""9999-12-31T23:59:59.9999999Z"",""0001-01-01T00:00:00Z""]}";

        const string SingleEntityPayloadWithDateTimeResponse_NonZ = @"{
""@odata.context"":""http://odataService/$metadata#EdmDateTimeSet/$entity"",
""Id"":""2016-11-29T18:07:16-08:00"",""DateTimeCollection"":[""0001-01-01T00:00:00Z"",""9999-12-31T23:59:59.9999999Z"",""0001-01-01T00:00:00Z""]}";

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

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        [Fact]
        public void TestClientLibDateTimeSupport()
        {
            TestDateTime(SingleEntityPayloadWithDateTimeResponse, () =>
            {
                var queryResult = ((DataServiceQuery<pkr.EdmDateTime>)dsc_dateTime.CreateQuery<pkr.EdmDateTime>("EdmDateTimeSet").Where(e => e.Id.Equals(DateTime.Now)));
                var result = queryResult.ToArray();
                Assert.Single(result);
                Assert.Equal(result[0].Id, DateTime.MaxValue);
            });

            TestDateTime(FeedEntityPayloadWithDateTimeResponse, () =>
            {
                var queryResult = ((DataServiceQuery<pkr.EdmDateTime>)dsc_dateTime.CreateQuery<pkr.EdmDateTime>("EdmDateTimeSet").Where(e => e.Id >= (DateTime.Now)));
                var result = queryResult.ToArray();
                Assert.Equal(2, result.Count());
                Assert.Equal(result[0].Id, DateTime.MinValue);
                Assert.Equal(result[1].Id, this.fixedDateTimeUtc);
            });

            TestDateTime(FeedEntityPayloadWithDateTimeResponse, () =>
            {
                var queryResult = ((DataServiceQuery<pkr.EdmDateTime>)dsc_dateTime.CreateQuery<pkr.EdmDateTime>("EdmDateTimeSet"));
                var result = queryResult.ToList();
                Assert.Equal(2, result.Count());
                Assert.Equal(result[0].Id, DateTime.MinValue);
                Assert.Equal(result[1].Id, this.fixedDateTimeUtc);
            });

            TestDateTime(ReturnCollectionResponse, () =>
            {
                var queryResult = dsc_dateTime.EdmDateTimeSet.Where(e => e.Id == (DateTime.MinValue)).Select(e => e.DateTimeCollection).FirstOrDefault();
                var result = queryResult.ToList();
                Assert.True(result.Count == 3);
                Assert.Equal(result[0], DateTime.MinValue);
                Assert.Equal(result[1], this.fixedDateTimeUtc);
                Assert.Equal(result[2], DateTime.MaxValue);
            });

            TestDateTime(ReturnCollectionResponse1, () =>
            {
                var queryResult = dsc_dateTime.EdmDateTimeSet.Where(e => e.Id == (DateTime.MinValue)).Select(e => e.DateTimeCollection).FirstOrDefault();
                var result = queryResult.ToList();
                Assert.True(result.Count == 3);
                Assert.Equal(result[0], DateTime.MinValue);
                Assert.Equal(result[1], this.fixedDateTimeUtc);
                Assert.Equal(result[2], DateTime.MaxValue);
            });

            TestDateTime(ReturnSingleDateTimeResponse, () =>
            {
                var queryResult = dsc_dateTime.EdmDateTimeSet.Where(e => e.Id == (DateTime.MinValue)).Select(e => e.Id).FirstOrDefault();
                Assert.Equal(queryResult, this.fixedDateTimeUtc);
            });

            TestDateTime(ReturnSingleMaxDateTimeResponse, () =>
            {
                var queryResult = dsc_dateTime.EdmDateTimeSet.Where(e => e.Id == (DateTime.MinValue)).Select(e => e.Id).FirstOrDefault();
                Assert.Equal(queryResult, DateTime.MaxValue);
            });

            TestDateTime(ReturnSingleMinDateTimeResponse, () =>
            {
                var queryResult = dsc_dateTime.EdmDateTimeSet.Where(e => e.Id == (DateTime.MinValue)).Select(e => e.Id).FirstOrDefault();
                Assert.Equal(queryResult, DateTime.MinValue);
            });

            TestDateTime(ReturnSingleDateTimeResponse1, () =>
            {
                var expected_local = fixedDateTimeUtc.ToLocalTime();
                var queryResult = dsc_dateTime.EdmDateTimeSet.Where(e => e.Id == expected_local).Select(e => e.Id).FirstOrDefault();
                Assert.Equal(queryResult, fixedDateTimeUtc);
            });
        }

        [Fact]
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
                    Assert.True(properties.Count() == 2);
                    var dateTimePropertyInEntry = args.Entry.Properties.Where(p => p.Name == "Id").FirstOrDefault().Value;
                    Assert.True(dateTimePropertyInEntry is DateTimeOffset);
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
                    Assert.True(payloadChecked);
                }
            });
        }
#endif

        [Fact]
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
            var dateTimeValue3 = new ObservableCollection<DateTime>() { this.fixedDateTimeUtc };
            List<BodyOperationParameter> parameters3 = new List<BodyOperationParameter> {
                new BodyOperationParameter("entityWithDateTimeProperty", dateTimeEntity),
                new BodyOperationParameter("datetimeParameter2", dateTimeValue2),
                new BodyOperationParameter("collectionDateTime", dateTimeValue3) };

            ODataRequestMessageWrapper requestMessage3 = CreateRequestMessageForPost(requestInfo);
            serializer.WriteBodyOperationParameters(parameters3, requestMessage3);
            const string parameterString3 = @"{""entityWithDateTimeProperty"":{""@odata.type"":""#Microsoft.Test.OData.Services.PrimitiveKeysService.EdmDateTime"",""DateTimeCollection@odata.type"":""#Collection(DateTimeOffset)"",""DateTimeCollection"":[""2016-11-30T02:07:16Z"",""0001-01-01T00:00:00Z"",""9999-12-31T23:59:59.9999999Z""],""Id@odata.type"":""#DateTimeOffset"",""Id"":""0001-01-01T00:00:00Z""},""datetimeParameter2"":""0001-01-01T00:00:00Z"",""collectionDateTime"":[""2016-11-30T02:07:16Z""]}";
            VerifyMessageBody(requestMessage3, parameterString3);
        }

        [Fact]
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

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        [Fact]
        public void WriteDateAndTimeUriOperationParametersToUriShouldReturnUri()
        {
            var requestInfo = new RequestInfo(dsc_dateTime);
            var serializer = new Serializer(requestInfo);
            Uri serviceRootUri = new Uri("http://odataService/GetDateTime");
            Uri requestUri = null;

            EdmDateTime dateTimeEntity = new EdmDateTime()
            {
                Id = DateTime.MinValue
            };
            dateTimeEntity.DateTimeCollection = new ObservableCollection<DateTime>() { this.fixedDateTimeUtc };

            TestDateTime(SingleEntityPayloadWithDateTimeResponse_NonZ, () =>
            {
                // Parameter is an entity with DateTime property
                List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("dateTimeEntity", dateTimeEntity) };
                requestUri = serializer.WriteUriOperationParametersToUri(serviceRootUri, parameters);

                string parameterString = @"{""@odata.type"":""#Microsoft.Test.OData.Services.PrimitiveKeysService.EdmDateTime"",""DateTimeCollection@odata.type"":""#Collection(DateTimeOffset)"",""DateTimeCollection"":[""2016-11-30T02:07:16Z""],""Id@odata.type"":""#DateTimeOffset"",""Id"":""0001-01-01T00:00:00Z""}";
                // Validate request Uri
                Uri expectedUri = new Uri("http://odataservice/GetDateTime(dateTimeEntity=%40dateTimeEntity)?%40dateTimeEntity=" + Uri.EscapeDataString(parameterString));
                Assert.Equal(expectedUri, requestUri);

                // Validate return 
                var queryResult = dsc_dateTime.Execute<pkr.EdmDateTime>(requestUri);
                var result = queryResult.ToArray();
                Assert.Single(result);
                var expected = new DateTime(2016, 11, 30, 02, 07, 16);

                // kind, value
                Assert.Equal(result[0].Id, expected);
                Assert.Equal(DateTimeKind.Utc, result[0].Id.Kind);
                Assert.Equal(3, result[0].DateTimeCollection.Count);
            });


            dateTimeEntity.Id = DateTime.MaxValue;
            dateTimeEntity.DateTimeCollection = new ObservableCollection<DateTime>() { this.fixedDateTimeUtc, DateTime.MinValue, DateTime.MaxValue };

            TestDateTime(SingleEntityPayloadWithDateTimeResponse_NonZ, () =>
            {
                // Parameter is an entity with DateTime property
                List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("dateTimeEntity", dateTimeEntity) };
                requestUri = serializer.WriteUriOperationParametersToUri(serviceRootUri, parameters);

                // Validate request Uri
                string parameterString = @"{""@odata.type"":""#Microsoft.Test.OData.Services.PrimitiveKeysService.EdmDateTime"",""DateTimeCollection@odata.type"":""#Collection(DateTimeOffset)"",""DateTimeCollection"":[""2016-11-30T02:07:16Z"",""0001-01-01T00:00:00Z"",""9999-12-31T23:59:59.9999999Z""],""Id@odata.type"":""#DateTimeOffset"",""Id"":""9999-12-31T23:59:59.9999999Z""}";
                Uri expectedUri = new Uri(@"http://odataservice/GetDateTime(dateTimeEntity=%40dateTimeEntity)?%40dateTimeEntity=" + Uri.EscapeDataString(parameterString));
                Assert.Equal(expectedUri, requestUri);

                // Validate return 
                var queryResult = dsc_dateTime.Execute<pkr.EdmDateTime>(requestUri);
                var result = queryResult.ToArray();
                Assert.Single(result);
                var expected = new DateTime(2016, 11, 30, 02, 07, 16);

                // kind, value
                Assert.Equal(result[0].Id, expected);
                Assert.Equal(DateTimeKind.Utc, result[0].Id.Kind);
                Assert.Equal(3, result[0].DateTimeCollection.Count);
            });

            //return datetime
            TestDateTime(ReturnSingleDateTimeResponse, () =>
            {
                requestUri = new Uri("http://odataService/GetDateTime");

                // parameter is datetime
                List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("dateTimeValue", this.fixedDateTimeUtc) };
                requestUri = serializer.WriteUriOperationParametersToUri(requestUri, parameters);

                // Validate request Uri
                const string uriString = @"http://odataservice/GetDateTime(dateTimeValue=2016-11-30T02:07:16Z)";
                Assert.Equal(uriString, requestUri.ToString());

                var query = dsc_dateTime.CreateFunctionQuerySingle<DateTime>("", "GetDateTime", false, new UriOperationParameter("dateTimeValue", this.fixedDateTimeUtc));
                Assert.Equal(new Uri(uriString), query.RequestUri);

                var queryResult = query.GetValue();
                var expected = new DateTime(2016, 11, 30, 02, 07, 16, DateTimeKind.Utc);
                Assert.Equal(queryResult, expected);
            });

            //return datetime
            TestDateTime(ReturnSingleMaxDateTimeResponse, () =>
            {
                requestUri = new Uri("http://odataService/GetDateTime");

                // parameter is datetime
                List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("dateTimeValue", DateTime.MaxValue) };
                requestUri = serializer.WriteUriOperationParametersToUri(requestUri, parameters);

                // Validate request Uri
                const string uriString = @"http://odataservice/GetDateTime(dateTimeValue=9999-12-31T23:59:59.9999999Z)";
                Assert.Equal(uriString, requestUri.ToString());

                var query = dsc_dateTime.CreateFunctionQuerySingle<DateTime>("", "GetDateTime", false, new UriOperationParameter("dateTimeValue", DateTime.MaxValue));
                Assert.Equal(new Uri(uriString), query.RequestUri);

                var queryResult = query.GetValue();
                Assert.Equal(queryResult, DateTime.MaxValue);
            });

            //return datetime
            TestDateTime(ReturnSingleMinDateTimeResponse, () =>
            {
                requestUri = new Uri("http://odataService/GetDateTime");

                // parameter is datetime
                List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("dateTimeValue", DateTime.MinValue) };
                requestUri = serializer.WriteUriOperationParametersToUri(requestUri, parameters);

                // Validate request Uri
                const string uriString = @"http://odataservice/GetDateTime(dateTimeValue=0001-01-01T00:00:00Z)";
                Assert.Equal(uriString, requestUri.ToString());

                var query = dsc_dateTime.CreateFunctionQuerySingle<DateTime>("", "GetDateTime", false, new UriOperationParameter("dateTimeValue", DateTime.MinValue));
                Assert.Equal(new Uri(uriString), query.RequestUri);

                var queryResult = query.GetValue();
                Assert.Equal(queryResult, DateTime.MinValue);
            });

            // return collection(datetime)
            TestDateTime(ReturnCollectionResponse, () =>
            {
                // parameter is Collection<dateTime>
                requestUri = new Uri("http://odataService/GetDateTime");
                var dateTimeCollection = new Collection<DateTime>() { this.fixedDateTimeUtc };
                List<UriOperationParameter> parameters = new List<UriOperationParameter> { new UriOperationParameter("dateTimeCollection", dateTimeCollection) };
                requestUri = serializer.WriteUriOperationParametersToUri(requestUri, parameters);

                // Validate request Uri
                string parameterString = @"[""2016-11-30T02:07:16Z""]";
                Uri expectedUri = new Uri(@"http://odataservice/GetDateTime(dateTimeCollection=%40dateTimeCollection)?%40dateTimeCollection=" + Uri.EscapeDataString(parameterString));
                Assert.Equal(expectedUri, requestUri);

                // Bug for escape
                var query = dsc_dateTime.CreateFunctionQuery<DateTime>("", "GetDateTime", false, new UriOperationParameter("dateTimeCollection", dateTimeCollection));
                expectedUri = new Uri(@"http://odataservice/GetDateTime(dateTimeCollection=@dateTimeCollection)?@dateTimeCollection=" + Uri.EscapeUriString(parameterString));
                Assert.Equal(expectedUri, query.RequestUri);

                var result = query.ToList();
                Assert.True(result.Count == 3);
                Assert.Equal(result[0], DateTime.MinValue);
                Assert.Equal(result[1], this.fixedDateTimeUtc);
                Assert.Equal(result[2], DateTime.MaxValue);
            });
        }
#endif

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
            Assert.Equal(expected, body);
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