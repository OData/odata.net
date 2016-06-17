//---------------------------------------------------------------------
// <copyright file="ODataMessageTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if !SILVERLIGHT && !WINDOWS_PHONE
namespace Microsoft.Test.Taupo.OData.Common.Tests.ObjectModelTests
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for the API of the various request and response message implementations.
    /// </summary>
    [TestClass, TestCase(Name="OData Message Tests")]
    public class ODataMessageTests : ODataTestCase
    {
        private static readonly Type batchOperationsHeadersType = typeof(ODataBatchReader).Assembly.GetType("Microsoft.OData.ODataBatchOperationHeaders");
        private static readonly Type batchOperationListenerType = typeof(ODataBatchOperationRequestMessage).Assembly.GetType("Microsoft.OData.IODataBatchOperationListener");
        private static readonly Type urlResolverType = typeof(ODataBatchOperationRequestMessage).Assembly.GetType("Microsoft.OData.ODataBatchPayloadUriConverter");

        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        /// <summary>
        /// Gets or sets the exception verifier.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IExceptionVerifier ExceptionVerifier { get; set; }

        [TestMethod, Variation(Description = "Test the proper behavior of the headers of a request message.")]
        public void RequestMessageHeaderTest()
        {
            Func<bool, IODataRequestMessage>[] requestMessageFuncs = new Func<bool, IODataRequestMessage>[]
                {
                    (writing) => CreateBatchOperationRequestMessage(writing),
                    (writing) => new ODataRequestMessageWrapper(new TestRequestMessage(new TestStream()), writing, false),
                };

            this.CombinatorialEngineProvider.RunCombinations(
                new bool[] { true, false},
                requestMessageFuncs,
                (writing, func) =>
                {
                    IODataRequestMessage requestMessage = func(writing);
                    RunHeaderTest(() => requestMessage.Headers, writing, requestMessage.GetHeader, requestMessage.SetHeader, this.Assert, this.ExceptionVerifier);
                });
        }

        [TestMethod, Variation(Description = "Test the proper behavior of the headers of a response message.")]
        public void ResponseMessageHeaderTest()
        {
            Func<bool, IODataResponseMessage>[] responseMessageFuncs = new Func<bool, IODataResponseMessage>[]
                {
                    (writing) => CreateBatchOperationResponseMessage(writing),
                    (writing) => new ODataResponseMessageWrapper(new TestResponseMessage(new TestStream()), writing, false)
                };

            this.CombinatorialEngineProvider.RunCombinations(
                new bool[] { true, false },
                responseMessageFuncs,
                (writing, func) =>
                {
                    IODataResponseMessage responseMessage = func(writing);
                    RunHeaderTest(() => responseMessage.Headers, writing, responseMessage.GetHeader, responseMessage.SetHeader, this.Assert, this.ExceptionVerifier);
                });
        }

        private static void RunHeaderTest(
            Func<IEnumerable<KeyValuePair<string, string>>> getHeadersFunc, 
            bool writing,
            Func<string, string> getHeaderFunc, 
            Action<string, string> setHeaderAction,
            AssertionHandler assert,
            IExceptionVerifier exceptionVerifier)
        {
            assert.IsNotNull(getHeadersFunc(), "Non-null headers expected.");
            assert.AreEqual(0, getHeadersFunc().Count(), "Empty header collection exptected.");
            assert.IsNull(getHeaderFunc("a"), "Unexpectedly found header.");

            ExpectedException expectedException = writing ? null : ODataExpectedExceptions.ODataException("ODataMessage_MustNotModifyMessage");
            TestExceptionUtils.ExpectedException(
                assert,
                () =>
                {
                    setHeaderAction("a", "b");

                    assert.AreEqual(1, getHeadersFunc().Count(), "One header expected.");
                    assert.AreEqual("b", getHeaderFunc("a"), "Header not found or invalid header value.");
                    List<KeyValuePair<string, string>> expectedHeaders = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("a", "b")
                        };
                    VerificationUtils.VerifyEnumerationsAreEqual(
                        expectedHeaders,
                        getHeadersFunc(),
                        (first, second, assert2) => assert2.AreEqual(first, second, "Items differ."),
                        (item) => item.Key + " = " + item.Value,
                        assert);

                    setHeaderAction("a", "c");

                    assert.AreEqual(1, getHeadersFunc().Count(), "One header expected.");
                    assert.AreEqual("c", getHeaderFunc("a"), "Header not found or invalid header value.");
                    expectedHeaders[0] = new KeyValuePair<string, string>("a", "c");
                    VerificationUtils.VerifyEnumerationsAreEqual(
                        expectedHeaders,
                        getHeadersFunc(),
                        (first, second, assert2) => assert2.AreEqual(first, second, "Items differ."),
                        (item) => item.Key + " = " + item.Value,
                        assert);

                    setHeaderAction("d", "e");

                    assert.AreEqual(2, getHeadersFunc().Count(), "Two headers expected.");
                    assert.AreEqual("c", getHeaderFunc("a"), "Header not found or invalid header value.");
                    assert.AreEqual("e", getHeaderFunc("d"), "Header not found or invalid header value.");
                    expectedHeaders.Add(new KeyValuePair<string, string>("d", "e"));
                    VerificationUtils.VerifyEnumerationsAreEqual(
                        expectedHeaders,
                        getHeadersFunc(),
                        (first, second, assert2) => assert2.AreEqual(first, second, "Items differ."),
                        (item) => item.Key + " = " + item.Value,
                        assert);

                    setHeaderAction("d", null);
                    setHeaderAction("a", null);

                    assert.AreEqual(0, getHeadersFunc().Count(), "Empty header collection expected.");
                    assert.IsNull(getHeaderFunc("a"), "Unexpectedly found header.");
                },
                expectedException,
                exceptionVerifier);
        }

        private static IODataRequestMessage CreateBatchOperationRequestMessage(bool writing)
        {
            Stream stream = new TestStream();
            return (IODataRequestMessage)ReflectionUtils.CreateInstance(
                typeof(ODataBatchOperationRequestMessage),
                new Type[] { typeof(Func<Stream>), typeof(string), typeof(Uri), batchOperationsHeadersType, batchOperationListenerType, typeof(string), urlResolverType, typeof(bool), typeof(IServiceProvider) },
                (object)(Func<Stream>)(() => stream),
                ODataConstants.MethodGet,
                new Uri("http://www.odata.org/"),
                /*headers*/ null,
                CreateListener(stream, false, writing),
                "1",
                ReflectionUtils.CreateInstance(urlResolverType, new Type[] { typeof(IODataPayloadUriConverter) }, new object[] { null }),
                writing,
                /*container*/ null);
        }

        private static IODataResponseMessage CreateBatchOperationResponseMessage(bool writing)
        {
            Stream stream = new TestStream();

            return (IODataResponseMessage)ReflectionUtils.CreateInstance(
                typeof(ODataBatchOperationResponseMessage),
                new Type[] { typeof(Func<Stream>), batchOperationsHeadersType, batchOperationListenerType, typeof(string), typeof(IODataPayloadUriConverter), typeof(bool), typeof(IServiceProvider) },
                (object)(Func<Stream>)(() => stream), /*headers*/null, CreateListener(stream, true, writing), "1", /*urlResolver*/null, writing, /*container*/ null);
        }

        private static object CreateListener(Stream stream, bool response, bool writing)
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();
            object message = response
                ? new ODataResponseMessageWrapper(new TestResponseMessage(stream), writing, settings.EnableMessageStreamDisposal).WrappedMessageObject
                : new ODataRequestMessageWrapper(new TestRequestMessage(stream), writing, settings.EnableMessageStreamDisposal).WrappedMessageObject;
            ODataMessageInfo messageInfo = new ODataMessageInfo
            {
                MessageStream = (Stream)ReflectionUtils.InvokeMethod(message, "GetStream"),
                Encoding = Encoding.UTF8,
                IsResponse = response,
                IsAsync = false
            };
            Type odataRawOutputContextType = typeof(ODataBatchWriter).Assembly.GetType("Microsoft.OData.ODataRawOutputContext");
            object rawOutputContext = ReflectionUtils.CreateInstance(odataRawOutputContextType,
                new Type[]
                {
                    typeof(ODataFormat),
                    typeof(ODataMessageInfo),
                    typeof(ODataMessageWriterSettings),
                },
                ODataFormat.Batch,
                messageInfo,
                settings);
            object listener = ReflectionUtils.CreateInstance(
                typeof(ODataBatchWriter),
                new Type[] { odataRawOutputContextType, typeof(string) },
                rawOutputContext, "test-boundary");
            return listener;
        }

        private sealed class ODataRequestMessageWrapper : IODataRequestMessage
        {
            private static readonly Type requestMessageType = typeof(ODataWriter).Assembly.GetType("Microsoft.OData.ODataRequestMessage");

            private readonly object requestMessage;

            internal ODataRequestMessageWrapper(IODataRequestMessage requestMessage, bool writing, bool enableMessageStreamDisposal)
            {
                this.requestMessage = ReflectionUtils.CreateInstance(requestMessageType, requestMessage, writing, enableMessageStreamDisposal, /*maxBytesToBeRead*/ -1);
            }

            public IEnumerable<KeyValuePair<string, string>> Headers
            {
                get
                {
                    return (IEnumerable<KeyValuePair<string, string>>)ReflectionUtils.GetProperty(this.requestMessage, "Headers");
                }
            }

            public string GetHeader(string headerName)
            {
                return (string)ReflectionUtils.InvokeMethod(this.requestMessage, "GetHeader", headerName);
            }

            public void SetHeader(string headerName, string headerValue)
            {
                ReflectionUtils.InvokeMethod(this.requestMessage, "SetHeader", headerName, headerValue);
            }

            public Uri Url
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public string Method
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public Stream GetStream()
            {
                throw new NotImplementedException();
            }

            internal object WrappedMessageObject
            {
                get { return this.requestMessage; }
            }
        }

        private sealed class ODataResponseMessageWrapper : IODataResponseMessage
        {
            private static readonly Type responseMessageType = typeof(ODataWriter).Assembly.GetType("Microsoft.OData.ODataResponseMessage");

            private readonly object responseMessage;

            internal ODataResponseMessageWrapper(IODataResponseMessage responseMessage, bool writing, bool enableMessageStreamDisposal)
            {
                this.responseMessage = ReflectionUtils.CreateInstance(responseMessageType, responseMessage, writing, enableMessageStreamDisposal, /*maxBytesToBeRead*/ -1);
            }

            public IEnumerable<KeyValuePair<string, string>> Headers
            {
                get
                {
                    return (IEnumerable<KeyValuePair<string, string>>)ReflectionUtils.GetProperty(this.responseMessage, "Headers");
                }
            }

            public string GetHeader(string headerName)
            {
                return (string)ReflectionUtils.InvokeMethod(this.responseMessage, "GetHeader", headerName);
            }

            public void SetHeader(string headerName, string headerValue)
            {
                ReflectionUtils.InvokeMethod(this.responseMessage, "SetHeader", headerName, headerValue);
            }

            public Stream GetStream()
            {
                throw new NotImplementedException();
            }

            public int StatusCode
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            internal object WrappedMessageObject
            {
                get { return this.responseMessage; }
            }
        }
    }
}
#endif
