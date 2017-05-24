﻿//---------------------------------------------------------------------
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
    using Microsoft.OData.Core;
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
        private static readonly Type batchOperationsHeadersType = typeof(ODataBatchReader).Assembly.GetType("Microsoft.OData.Core.ODataBatchOperationHeaders");
        private static readonly Type batchOperationListenerType = typeof(ODataBatchOperationRequestMessage).Assembly.GetType("Microsoft.OData.Core.IODataBatchOperationListener");
        private static readonly Type urlResolverType = typeof(ODataBatchOperationRequestMessage).Assembly.GetType("Microsoft.OData.Core.ODataBatchUrlResolver");

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
                    (writing) => CreateBatchOperationRequestMessage(writing, /*isMultipartMediaType*/ true),
                    (writing) => CreateBatchOperationRequestMessage(writing, /*isMultipartMediaType*/ false),
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
                    (writing) => CreateBatchOperationResponseMessage(writing, /*isMultipartMediaType*/ true),
                    (writing) => CreateBatchOperationResponseMessage(writing, /*isMultipartMediaType*/ false),
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

        private static IODataRequestMessage CreateBatchOperationRequestMessage(bool writing, bool isMultipartMediaType)
        {
            Stream stream = new TestStream();
            return (IODataRequestMessage)ReflectionUtils.CreateInstance(
                typeof(ODataBatchOperationRequestMessage),
                new Type[] { typeof(Func<Stream>), typeof(string), typeof(Uri), batchOperationsHeadersType, batchOperationListenerType, typeof(string), urlResolverType, typeof(bool) },
                (object)(Func<Stream>)(() => stream),
                ODataConstants.MethodGet,
                new Uri("http://www.odata.org/"),
                /*headers*/ null,
                CreateListener(stream, false, writing, isMultipartMediaType),
                "1",
                ReflectionUtils.CreateInstance(urlResolverType, new Type[] { typeof(IODataUrlResolver) }, new object[] { null }),
                writing);
        }

        private static IODataResponseMessage CreateBatchOperationResponseMessage(bool writing, bool isMultipartMediaType)
        {
            Stream stream = new TestStream();

            return (IODataResponseMessage)ReflectionUtils.CreateInstance(
                typeof(ODataBatchOperationResponseMessage),
                new Type[] { typeof(Func<Stream>), batchOperationsHeadersType, batchOperationListenerType, typeof(string), typeof(IODataUrlResolver), typeof(bool) },
                (object)(Func<Stream>)(() => stream), /*headers*/null, CreateListener(stream, true, writing, isMultipartMediaType), "1", /*urlResolver*/null, writing);
        }

        private static object CreateListener(Stream stream, bool response, bool writing, bool isMultipartMediaType)
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();
            object message = response
                ? new ODataResponseMessageWrapper(new TestResponseMessage(stream), writing, settings.DisableMessageStreamDisposal).WrappedMessageObject
                : new ODataRequestMessageWrapper(new TestRequestMessage(stream), writing, settings.DisableMessageStreamDisposal).WrappedMessageObject;
            Type odataRawOutputContextType = typeof(ODataBatchWriter).Assembly.GetType("Microsoft.OData.Core.ODataRawOutputContext");
            object rawOutputContext = ReflectionUtils.CreateInstance(odataRawOutputContextType,
                new Type[]
                {
                    typeof(ODataFormat),
                    typeof(Stream),
                    typeof(Encoding),
                    typeof(ODataMessageWriterSettings),
                    typeof(bool),
                    typeof(bool),
                    typeof(Microsoft.OData.Edm.IEdmModel),
                    typeof(IODataUrlResolver)
                },
                ODataFormat.Batch,
                (Stream)ReflectionUtils.InvokeMethod(message, "GetStream"),
                Encoding.UTF8,
                settings,
                response,
                /*synchronous*/true,
                /*model*/null,
                /*urlResolver*/null);
            object listener = ReflectionUtils.CreateInstance(
                isMultipartMediaType? typeof(ODataBatchMimeWriter) : typeof(ODataBatchJsonWriter),
                new Type[] { odataRawOutputContextType, typeof(string) },
                rawOutputContext, "test-boundary");
            return listener;
        }

        private sealed class ODataRequestMessageWrapper : IODataRequestMessage
        {
            private static readonly Type requestMessageType = typeof(ODataWriter).Assembly.GetType("Microsoft.OData.Core.ODataRequestMessage");

            private readonly object requestMessage;

            internal ODataRequestMessageWrapper(IODataRequestMessage requestMessage, bool writing, bool disableMessageStreamDisposal)
            {
                this.requestMessage = ReflectionUtils.CreateInstance(requestMessageType, requestMessage, writing, disableMessageStreamDisposal, /*maxBytesToBeRead*/ -1);
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
            private static readonly Type responseMessageType = typeof(ODataWriter).Assembly.GetType("Microsoft.OData.Core.ODataResponseMessage");

            private readonly object responseMessage;

            internal ODataResponseMessageWrapper(IODataResponseMessage responseMessage, bool writing, bool disableMessageStreamDisposal)
            {
                this.responseMessage = ReflectionUtils.CreateInstance(responseMessageType, responseMessage, writing, disableMessageStreamDisposal, /*maxBytesToBeRead*/ -1);
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
