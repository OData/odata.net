//---------------------------------------------------------------------
// <copyright file="ModifyErrorResponseFunctionalTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using System.Data.Test.Astoria;
    using System.Linq;
    using System.ServiceModel.Web;
    using System.Text;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Tests.Server;
    using FluentAssertions;
    using Microsoft.OData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ModifyErrorResponseFunctionalTest
    {
        private Version V4 = new Version(4, 0);
        private static int HandleExceptionCalls;
        private static ODataValue AnnotationValue;
        private static string AnnotationName;

        [TestInitialize]
        public void Initialize()
        {
            HandleExceptionCalls = 0;
            AnnotationValue = null;
            AnnotationName = "location.error";
        }

        [TestMethod]
        [TestCategory("Partition2")]
        public void PrimitiveCustomAnnotationOnErrorShouldGetWrittenInJsonLight()
        {
            using (TestWebRequest webRequest = this.SetupRequest())
            {
                webRequest.HttpMethod = "DELETE";
                webRequest.Accept = "application/json;odata.metadata=minimal";
                AnnotationValue = new ODataPrimitiveValue(66);

                TestUtil.RunCatching(webRequest.SendRequest);

                webRequest.ErrorResponseContent.Should().Be("{\"error\":{\"code\":\"\",\"message\":\"Resource not found for the segment 'ThisDoesNotExist'.\",\"@location.error\":66}}");
                HandleExceptionCalls.Should().Be(1);
            }
        }

        [TestMethod]
        [TestCategory("Partition2")]
        public void MultipleCustomAnnotationOnErrorShouldGetWrittenInJsonLight()
        {
            using (TestWebRequest webRequest = this.SetupRequest())
            {
                webRequest.DataServiceType = typeof(ModifyErrorMessageInHandleExceptionServiceMultipleAnnotation);
                webRequest.Accept = "application/json;odata.metadata=minimal";
                AnnotationValue = new ODataPrimitiveValue(77);

                TestUtil.RunCatching(webRequest.SendRequest);

                webRequest.ErrorResponseContent.Should().Be("{\"error\":{\"code\":\"\",\"message\":\"Resource not found for the segment 'ThisDoesNotExist'.\",\"@location.error\":77,\"@location.error2\":\"AdditionalAnnotationValue\"}}");
                HandleExceptionCalls.Should().Be(1);
            }
        }

        /*
        [TestMethod]
        [TestCategory("Partition2")]
        public void ComplexCustomAnnotationOnErrorShouldGetWrittenInJsonLight()
        {
            using (TestWebRequest webRequest = this.SetupRequest())
            {
                webRequest.Accept = "application/json;odata.metadata=minimal";
                AnnotationValue = new ODataComplexValue
                {
                    TypeName = "AstoriaUnitTests.Stubs.Address",
                    Properties = new[]
                    {
                        new ODataProperty {Name="City", Value = "Troy"},
                        new ODataProperty {Name="State", Value = "New York"}
                    }
                };
                TestUtil.RunCatching(webRequest.SendRequest);

                webRequest.ErrorResponseContent.Should().Be("{\"error\":{\"code\":\"\",\"message\":\"Resource not found for the segment 'ThisDoesNotExist'.\",\"@location.error\":{\"@odata.type\":\"#AstoriaUnitTests.Stubs.Address\",\"City\":\"Troy\",\"State\":\"New York\"}}}");
                HandleExceptionCalls.Should().Be(1);
            }
        }
         */

        [TestMethod]
        [TestCategory("Partition2")]
        public void ComplexCollectionAnnotationOnErrorShouldGetWrittenInJsonLight()
        {
            using (TestWebRequest webRequest = this.SetupRequest())
            {
                webRequest.Accept = "application/json;odata.metadata=minimal";
                AnnotationValue = new ODataCollectionValue
                {
                    TypeName = "Collection(Edm.Int32)",
                    Items = new object[]{1,2,3,4}
                };
                TestUtil.RunCatching(webRequest.SendRequest);

                webRequest.ErrorResponseContent.Should().Be("{\"error\":{\"code\":\"\",\"message\":\"Resource not found for the segment 'ThisDoesNotExist'.\",\"location.error@odata.type\":\"#Collection(Int32)\",\"@location.error\":[1,2,3,4]}}");
                HandleExceptionCalls.Should().Be(1);
            }
        }

        [TestMethod]
        [TestCategory("Partition2")]
        public void EmptyCollectionAnnotationOnErrorShouldGetWrittenInJsonLight()
        {
            using (TestWebRequest webRequest = this.SetupRequest())
            {
                webRequest.Accept = "application/json;odata.metadata=minimal";
                AnnotationValue = new ODataCollectionValue
                {
                    TypeName = "Collection(Edm.Int32)",
                    Items = new object[] { },
                };
                TestUtil.RunCatching(webRequest.SendRequest);

                webRequest.ErrorResponseContent.Should().Be("{\"error\":{\"code\":\"\",\"message\":\"Resource not found for the segment 'ThisDoesNotExist'.\",\"location.error@odata.type\":\"#Collection(Int32)\",\"@location.error\":[]}}");
                HandleExceptionCalls.Should().Be(1);
            }
        }

        [TestMethod]
        [TestCategory("Partition2")]
        public void BatchCustomAnnotationOnInnerRequestErrorShouldGetWrittenInJsonLight()
        {
            StringBuilder batchQueryOperation = new StringBuilder();
            batchQueryOperation.AppendLine("GET Customers(1)/Addresssss?Override-Accept=" + UnitTestsUtil.JsonLightMimeType + " HTTP/1.1");
            batchQueryOperation.AppendLine("Host: host");
            batchQueryOperation.AppendLine("Accept: " + "application/json;odata.metadata=minimal");

            var test = new SimpleBatchTestCase
            {
                RequestPayload = new BatchInfo(new BatchQuery(new Operation(batchQueryOperation.ToString()))),
                ResponseStatusCode = 202,
                ResponseETag = default(string),
                ResponseVersion = V4,
                RequestDataServiceVersion = V4,
                RequestMaxDataServiceVersion = V4,
            };

            using (TestWebRequest webRequest = this.SetupRequest())
            {
                webRequest.HttpMethod = "POST";
                webRequest.RequestUriString = "/$batch";

                webRequest.Accept = UnitTestsUtil.MimeMultipartMixed;
                webRequest.RequestVersion = "4.0;";
                webRequest.RequestMaxVersion = "4.0;";
                webRequest.ForceVerboseErrors = true;

                const string boundary = "batch-set";
                webRequest.RequestContentType = String.Format("{0}; boundary={1}", UnitTestsUtil.MimeMultipartMixed, boundary);
                webRequest.SetRequestStreamAsText(BatchRequestWritingUtils.GetBatchText(test.RequestPayload, boundary));

                AnnotationValue = new ODataCollectionValue
                {
                    TypeName = "Collection(Edm.String)",
                    Items = new []
                    {
                        "404",
                        new DateTimeOffset(2012, 10, 10, 1, 2, 3, new TimeSpan()).ToString()
                    }
                };

                TestUtil.RunCatching(webRequest.SendRequest);
                webRequest.ResponseStatusCode.Should().Be(202);
                webRequest.GetResponseStreamAsText().Should().Contain("{\"error\":{\"code\":\"\",\"message\":\"Resource not found for the segment 'Addresssss'.\",\"location.error@odata.type\":\"#Collection(String)\",\"@location.error\":[\"404\",\"10/10/2012 1:02:03 AM +00:00\"]}}");
                HandleExceptionCalls.Should().Be(1);
            }
        }

        [TestMethod]
        [TestCategory("Partition2")]
        public void FailedTopLevelBatchRequestShouldBeXmlRegardlessOfCustomAnnotation()
        {
            StringBuilder batchQueryOperation = new StringBuilder();
            batchQueryOperation.AppendLine("GET Customers(1)/Address?Override-Accept=" + UnitTestsUtil.JsonLightMimeType + " HTTP/1.1");
            batchQueryOperation.AppendLine("Host: host");
            batchQueryOperation.AppendLine("Accept: " + "application/json;odata.metadata=minimal");

            var test = new SimpleBatchTestCase
            {
                RequestPayload = new BatchInfo(new BatchQuery(new Operation(batchQueryOperation.ToString()))),
                ResponseStatusCode = 400,
            };

            using (TestWebRequest webRequest = this.SetupRequest())
            {
                webRequest.HttpMethod = "POST";
                webRequest.RequestUriString = "/$batch";

                webRequest.Accept = UnitTestsUtil.MimeMultipartMixed;
                webRequest.RequestVersion = "4.0;";
                webRequest.RequestMaxVersion = "4.0;";
                webRequest.ForceVerboseErrors = true;
                const string boundary = "batch-set";

                // set content type to plain so the batch request fails with 400
                webRequest.RequestContentType = String.Format("{0}; boundary={1}", "application/plain", boundary);
                webRequest.SetRequestStreamAsText(BatchRequestWritingUtils.GetBatchText(test.RequestPayload, boundary));

                AnnotationValue = new ODataPrimitiveValue("This is a custom value message");

                TestUtil.RunCatching(webRequest.SendRequest);

                // Since the error response of top level batch request is xml, the custom error annotation will be ignored
                webRequest.ResponseStatusCode.Should().Be(test.ResponseStatusCode);
                webRequest.GetResponseStreamAsText().Should().NotContain("custom value message");
                HandleExceptionCalls.Should().Be(1);
            }
        }

        [TestMethod]
        [TestCategory("Partition2")]
        public void BadCustomAnnotationOnErrorCausesInStreamErrorInErrorPayload()
        {
            using (TestWebRequest webRequest = this.SetupRequest())
            {
                webRequest.Accept = "application/json;odata.metadata=minimal";
                AnnotationValue = new ODataCollectionValue {Items = new[] { "item1", new object() }}; // the new object is not a supported primitive type.
                TestUtil.RunCatching(webRequest.SendRequest);

                webRequest.ErrorResponseContent.Should().Be("{\"error\":{\"code\":\"\",\"message\":\"Resource not found for the segment 'ThisDoesNotExist'.\",\"@location.error\":[\"item1\"{\"error\":{\"code\":\"500\",\"message\":\"An error occurred while trying to write an error payload.\"}}");
                HandleExceptionCalls.Should().Be(1);
            }
        }

        [TestMethod]
        [TestCategory("Partition2")]
        public void BadCustomAnnotationOnErrorWithVerboseErrorsCausesInStreamErrorInErrorPayload()
        {
            using (TestWebRequest webRequest = this.SetupRequest())
            using (ModifyErrorMessageInHandleExceptionService.UseVerboseErrors.Restore())
            using (TestUtil.MetadataCacheCleaner())
            {
                webRequest.Accept = "application/json;odata.metadata=minimal";

                ModifyErrorMessageInHandleExceptionService.UseVerboseErrors.Value = true;
                AnnotationValue = new ODataCollectionValue { Items = new[] { "item1", new object() } }; // the new object is not a supported primitive type.
                TestUtil.RunCatching(webRequest.SendRequest);

                // With verbose error, the in-stream error response shows inner exception call stack
                const string expected = "{\"error\":{\"code\":\"\",\"message\":\"Resource not found for the segment 'ThisDoesNotExist'.\",\"@location.error\":[\"item1\"{\"error\":{\"code\":\"500\",\"message\":\"An error occurred while trying to write an error payload.\",\"innererror\":{\"message\":\"The value of type 'System.Object' is not supported and cannot be converted to a JSON representation.\"";
                webRequest.ErrorResponseContent.Should().StartWith(expected);
                HandleExceptionCalls.Should().Be(1);
            }
        }

        [TestMethod]
        [TestCategory("Partition2")]
        public void BadCustomAnnotationOnErrorWithBatchRequestCausesInStreamErrorInErrorPayload()
        {
            StringBuilder batchQueryOperation = new StringBuilder();
            batchQueryOperation.AppendLine("GET Customers(1)/Addresssss?Override-Accept=" + UnitTestsUtil.JsonLightMimeType + " HTTP/1.1");
            batchQueryOperation.AppendLine("Host: host");
            batchQueryOperation.AppendLine("Accept: " + "application/json;odata.metadata=minimal");

            var test = new SimpleBatchTestCase
            {
                RequestPayload = new BatchInfo(new BatchQuery(new Operation(batchQueryOperation.ToString()))),
                ResponseStatusCode = 202,
                ResponseETag = default(string),
                ResponseVersion = V4,
                RequestDataServiceVersion = V4,
                RequestMaxDataServiceVersion = V4,
            };

            using (TestWebRequest webRequest = this.SetupRequest())
            {
                webRequest.HttpMethod = "POST";
                webRequest.RequestUriString = "/$batch";

                webRequest.Accept = UnitTestsUtil.MimeMultipartMixed;
                webRequest.RequestVersion = "4.0;";
                webRequest.RequestMaxVersion = "4.0;";
                webRequest.ForceVerboseErrors = true;

                const string boundary = "batch-set";
                webRequest.RequestContentType = String.Format("{0}; boundary={1}", UnitTestsUtil.MimeMultipartMixed, boundary);
                webRequest.SetRequestStreamAsText(BatchRequestWritingUtils.GetBatchText(test.RequestPayload, boundary));

                AnnotationValue = new ODataCollectionValue
                {
                    Items = new Object[]{123, new Object()} // collection value is wrong so error payload fails to write
                };

                TestUtil.RunCatching(webRequest.SendRequest);

                // For batch request if ODL fails when writing error, HandleException is called twice and ODataError annotation cannot be be written correctly
                webRequest.ResponseStatusCode.Should().Be(202);
                webRequest.GetResponseStreamAsText().Should().Contain("{\"error\":{\"code\":\"\",\"message\":\"Resource not found for the segment 'Addresssss'.\",\"@location.error\":[123<?xml");
                HandleExceptionCalls.Should().Be(2);
            }
        }

        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/875
        [Ignore] // Remove Atom
        // [TestMethod]
        // [TestCategory("Partition2")]
        public void CustomAnnotationOnErrorShouldBeIgnoredInAtom()
        {
            using (TestWebRequest webRequest = this.SetupRequest())
            {
                webRequest.Accept = "application/atom+xml";
                AnnotationValue = new ODataPrimitiveValue("primitive value");
                TestUtil.RunCatching(webRequest.SendRequest);

                webRequest.ErrorResponseContent.Should().Be("<?xml version=\"1.0\" encoding=\"utf-8\"?><m:error xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\"><m:code /><m:message>Resource not found for the segment 'ThisDoesNotExist'.</m:message></m:error>");
                HandleExceptionCalls.Should().Be(1);
            }
        }

        [TestMethod]
        [TestCategory("Partition2")]
        public void PrimitiveCustomAnnotationOnInStreamError()
        {
            using (TestWebRequest webRequest = this.SetupRequest())
            {
                webRequest.RequestUriString = "/InStreamErrorGetCustomers";
                webRequest.Accept = "application/json;odata.metadata=minimal";
                AnnotationValue = new ODataPrimitiveValue(88);

                TestUtil.RunCatching(webRequest.SendRequest);

                webRequest.ErrorResponseContent.Should().BeEmpty();
                string responseBody = webRequest.GetResponseStreamAsText();
                responseBody.Should().StartWith("{\"@odata.context\":\"http://");
                responseBody.Should().Contain(string.Concat(
                      "\"ID\":1,",
                      "\"Name\":\"Customer 1\",",
                      "\"NameAsHtml\":\"<html><body>Customer 1</body></html>\",",
                      "\"Birthday\":\""
                    ));
                responseBody.Should().EndWith(string.Concat(
                    "},",
                    "{",
                      "\"error\":",
                      "{",
                        "\"code\":\"\",",
                        "\"message\":\"InStreamErrorGetCustomers ThrowForCustomer2 error\",",
                        "\"@location.error\":88",
                      "}",
                    "}"
                    ));

                HandleExceptionCalls.Should().Be(1);
            }
        }

        private TestWebRequest SetupRequest()
        {
            var request = TestWebRequest.CreateForInProcessWcf();
            request.DataServiceType = typeof(ModifyErrorMessageInHandleExceptionService);
            request.RequestUriString = "/ThisDoesNotExist";
            request.RequestMaxVersion = "4.0";
            return request;
        }

        private class ModifyErrorMessageInHandleExceptionService : DataService<CustomDataContext>
        {
            internal static readonly Restorable<bool> UseVerboseErrors = new Restorable<bool>(false);

            public static void InitializeService(DataServiceConfiguration config)
            {
                config.SetEntitySetAccessRule("*", EntitySetRights.All);
                config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                config.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);
                config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                config.UseVerboseErrors = UseVerboseErrors.Value;
            }

            protected override void HandleException(HandleExceptionArgs args)
            {
                base.HandleException(args);
                HandleExceptionCalls++;
                args.InstanceAnnotations.Add(new ODataInstanceAnnotation(AnnotationName, AnnotationValue));
            }

            // A service operation that fails with in-stream error.
            [WebGet()]
            public IEnumerable<Customer> InStreamErrorGetCustomers()
            {
                return this.CurrentDataSource.Customers.AsEnumerable<Customer>().Where(c => c.ID < 5 && this.ThrowForCustomer2(c));
            }

            private bool ThrowForCustomer2(Customer c)
            {
                if (c.ID == 2)
                {
                    throw new DataServiceException("InStreamErrorGetCustomers ThrowForCustomer2 error");
                }

                return c.ID > 0;
            }
        }

        private class ModifyErrorMessageInHandleExceptionServiceMultipleAnnotation : DataService<CustomDataContext>
        {
            internal static readonly Restorable<bool> UseVerboseErrors = new Restorable<bool>(false);

            public static void InitializeService(DataServiceConfiguration config)
            {
                config.SetEntitySetAccessRule("*", EntitySetRights.All);
                config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                config.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);
                config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                config.UseVerboseErrors = UseVerboseErrors.Value;
            }

            protected override void HandleException(HandleExceptionArgs args)
            {
                base.HandleException(args);
                HandleExceptionCalls++;
                args.InstanceAnnotations.Add(new ODataInstanceAnnotation(AnnotationName, AnnotationValue));
                args.InstanceAnnotations.Add(new ODataInstanceAnnotation("location.error2", new ODataPrimitiveValue("AdditionalAnnotationValue")));
            }
        }
    }
}
