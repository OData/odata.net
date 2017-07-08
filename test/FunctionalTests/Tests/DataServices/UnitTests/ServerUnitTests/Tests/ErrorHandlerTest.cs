//---------------------------------------------------------------------
// <copyright file="ErrorHandlerTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using AstoriaUnitTests.Data;
    using AstoriaUnitTests.Stubs;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion Namespaces

    /// <summary>This is a test class for ErrorHandler.</summary>
    [TestClass()]
    public class ErrorHandlerTest
    {
        [TestMethod]
        public void VerboseExceptionTest()
        {
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension(CustomDataContext.ExceptionTypeArgument, new object[] { typeof(FormatException) }),
                new Dimension(CustomDataContext.ExceptionAtEndArgument, new object[] { true }),
                new Dimension("verbose", new bool[] { true, false }));
            TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
            {
                bool verbose = (bool)values["verbose"];
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    int customerCount = (0xA0 / "Customer 1".Length) + 1;
                    values[CustomDataContext.CustomerCountArgument] = customerCount;

                    request.TestArguments = values;
                    request.ForceVerboseErrors = verbose;
                    request.DataServiceType = typeof(CustomDataContext);
                    request.RequestUriString = "/Customers";
                    Exception exception = TestUtil.RunCatching(request.SendRequest);

                    Assert.IsNotNull(exception, "Expecting an exception, but no exception was thrown");
                    if (verbose)
                    {
                        Assert.AreEqual(typeof(FormatException), exception.InnerException.GetType(), "Expecting formatexception, when verboseErrors is turned on");
                    }
                    else
                    {
                        Assert.AreEqual(typeof(WebException), exception.GetType(), "Expecting WebException thrown by TestServiceHost.ProcessException method");
                        Assert.AreEqual("WebException from TestServiceHost.ProcessException", exception.Message);
                    }
                }
            });
        }

        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
        [Ignore] // Remove Atom
        // [TestMethod]
        public void ErrorResponseContentTypeHasCharset()
        {
            // regression tests for these bugs :
            // [GQL Failure, ODataLib integration] Server does not write charset in content-type header on error responses
            // [GQL Failure, Astoria-ODataLib Integration] ContentType provided to DataService.HandleException does not match final header value in $batch
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension(CustomDataContext.ExceptionTypeArgument, new object[] { typeof(FormatException) }),
                new Dimension(CustomDataContext.ExceptionAtEndArgument, new object[] { true }),
                new Dimension("verbose", new bool[] { true, false }),
                new Dimension("Accept", new string[] { "application/atom", UnitTestsUtil.JsonLightMimeType }),
                new Dimension("Charset", new string[] { null, Encoding.UTF8.WebName, "iso-8859-1" }));
            TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
            {
                bool verbose = (bool)values["verbose"];
                using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
                {
                    int customerCount = (0xA0 / "Customer 1".Length) + 1;
                    string accept = values["Accept"].ToString();
                    object charset = values["Charset"];
                    values[CustomDataContext.CustomerCountArgument] = customerCount;

                    if (charset == null)
                    {
                        charset = Encoding.UTF8.WebName;
                    }
                    else
                    {
                        request.AcceptCharset = charset.ToString();
                    }

                    request.Accept = accept;
                    request.TestArguments = values;
                    request.ForceVerboseErrors = verbose;
                    request.DataServiceType = typeof(CustomDataContext);
                    request.RequestUriString = "/NonExistantSet";
                    Exception exception = TestUtil.RunCatching(request.SendRequest);

                    string responseType = string.Equals(accept, "application/atom") ? "application/xml" : UnitTestsUtil.JsonLightMimeType + ";odata.streaming=true;IEEE754Compatible=false";
                    string expectedContentType = string.Format("{0};charset={1}", responseType, charset.ToString());
                    
                    Assert.IsNotNull(exception, "Expecting an exception, but no exception was thrown");
                    Assert.AreEqual(expectedContentType, request.ResponseContentType, true /*ignoreCase*/);
                }
            });
        }

        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/877
        [Ignore] // Remove Atom
        // [TestMethod]
        public void ProcessExceptionTest()
        {
            Type[] exceptionTypes = new Type[]
            {
                typeof(OutOfMemoryException), 
                typeof(DataServiceException),
                typeof(FormatException),
                typeof(DataServiceException),
            };

            // At-End is currently always true, because the IQueryable caching
            // doesn't give us a good point to fail before content is written out.
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension(CustomDataContext.ExceptionTypeArgument, exceptionTypes),
                new Dimension(CustomDataContext.ExceptionAtEndArgument, new object[] { true }),
                new Dimension("Format", SerializationFormatData.Values),
                new Dimension("WebServerLocation", new object[] { WebServerLocation.InProcess, WebServerLocation.Local }));

            TestUtil.RunCombinatorialEngineFail(engine, delegate(Hashtable values)
            {
                Type exceptionType = (Type)values[CustomDataContext.ExceptionTypeArgument];
                bool exceptionAtEnd = (bool)values[CustomDataContext.ExceptionAtEndArgument];
                SerializationFormatData format = (SerializationFormatData)values["Format"];
                WebServerLocation location = (WebServerLocation)values["WebServerLocation"];

                // The local web server doesn't handle OOF gracefully - skip that case.
                if (exceptionType == typeof(OutOfMemoryException) &&
                    location == WebServerLocation.Local)
                {
                    return;
                }

                // No binary properties in the model.
                if (format.Name == "Binary") return;

                // We need at least 1024 bytes to be written out for the default
                // StreamWriter used by the XmlTextWriter to flush out (at which point
                // we can assume that throwing at the end of the stream caused
                // a "partial send").
                //
                // However the implementation ends up using an XmlUtf8RawTextWriterIndent
                // object, with a BUFSIZE of 0x1800 as declared on XmlUtf8RawTextWriter.
                //
                // (0x1800 / "Customer 1".Length) + 1 is ideal, but we can make do with much less.
                int customerCount = (0xA0 / "Customer 1".Length) + 1;
                values[CustomDataContext.CustomerCountArgument] = customerCount;

                using (TestWebRequest request = TestWebRequest.CreateForLocation(location))
                {
                    request.DataServiceType = typeof(CustomDataContext);
                    request.TestArguments = values;
                    request.Accept = format.MimeTypes[0];
                    request.RequestUriString =
                        (format.Name == "Text") ? "/Customers(" + customerCount + ")/ID/$value" : "/Customers";

                    Trace.WriteLine("Requesting " + request.RequestUriString);
                    Stream response = new MemoryStream();
                    Exception thrownException = null;
                    try
                    {
                        request.SendRequest();
                        thrownException = new ApplicationException("No exception actually thrown.");
                    }
                    catch (Exception exception)
                    {
                        thrownException = exception;
                    }

                    // InProcess always throws WebException. Look in the inner exception for the right exception type.
                    if (location == WebServerLocation.InProcess && !format.IsPrimitive && exceptionType != typeof(OutOfMemoryException))
                    {
                        Assert.AreEqual(typeof(WebException), thrownException.GetType(), "InProcess should always throw WebException - Look in TestServiceHost.ProcessException");
                        thrownException = thrownException.InnerException;
                    }

                    // Exception may be wrapped by TargetInvocationException.
                    if (thrownException is TargetInvocationException)
                    {
                        thrownException = thrownException.InnerException;
                    }

                    TestUtil.CopyStream(request.GetResponseStream(), response);

                    response.Position = 0;
                    if (exceptionAtEnd && !format.IsPrimitive)
                    {
                        // for inprocess, there will be no exception in the payload
                        if (location == WebServerLocation.InProcess)
                        {
                            // Verify the exception type
                            Assert.AreEqual(exceptionType, thrownException.GetType(), "Exception type did not match");
                            return;
                        }

                        Assert.IsTrue(HasContent(response), "HasContent(response)");
                        Assert.IsTrue(String.Equals(request.Accept, TestUtil.GetMediaType(request.ResponseContentType), StringComparison.OrdinalIgnoreCase));
                        if (exceptionType != typeof(OutOfMemoryException))
                        {
                            string responseText = new StreamReader(response).ReadToEnd();
                            TestUtil.AssertContains(responseText, "error");
                            TestUtil.AssertContains(responseText, "message");
                        }

                        Assert.IsTrue(thrownException is ApplicationException, "No exception thrown.");
                    }
                    else
                    {
                        if (exceptionType == typeof(OutOfMemoryException))
                        {
                            if (location == WebServerLocation.InProcess)
                            {
                                Assert.IsTrue(thrownException is OutOfMemoryException, "thrownException is OutOfMemoryException");
                                Assert.IsTrue(exceptionAtEnd || !HasContent(response), "exceptionAtEnd || !HasContent(response)");
                            }
                            else
                            {
                                Assert.IsTrue(thrownException is WebException, "thrownException is WebException");
                            }
                        }
                        else
                        {
                            Assert.IsTrue(thrownException is WebException, "thrownException is WebException");
                            Assert.IsTrue(HasContent(response), "HasContent(response)");
                            string expected =
                                (location == WebServerLocation.InProcess) ? "text/plain" : "application/xml";
                            Assert.AreEqual(expected, TestUtil.GetMediaType(request.ResponseContentType));
                        }
                    }
                }
            });
        }

        [TestMethod]
        public void VerifyODataLibIsUsedForWritingTopLevelErrors()
        {
            // Only ODL knows about json lite serialization, so we know ODL is being used if the json lite error was serialized correctly.
            const string expectedJsonLitePayload = @"{""error"":{""code"":"""",""message"":""Resource not found for the segment 'Customers'.""}}";

            using (TestWebRequest r = TestWebRequest.CreateForInProcessWcf())
            {
                r.DataServiceType = typeof(CustomDataContext);
                r.RequestUriString = "/Customers(-2345354)";
                r.HttpMethod = "GET";
                r.Accept = "application/json;odata.metadata=minimal";
                TestUtil.RunCatching(() => r.SendRequest());

                string responsePayload = r.GetResponseStreamAsText();
                Assert.AreEqual(404, r.ResponseStatusCode);
                Assert.AreEqual(expectedJsonLitePayload, responsePayload);
            }
        }

        /// <summary>Checks whether the specified <see cref="Stream"/> has content.</summary>
        /// <param name="stream"><see cref="Stream"/> to check.</param>
        /// <returns>true if the <paramref name="stream"/> has any content; false otherwise.</returns>
        /// <remarks>Note that this method will consume up to one byte of content.</remarks>
        private static bool HasContent(Stream stream)
        {
            if (stream == null)
            {
                return false;
            }

            return stream.CanSeek ? (stream.Length > 0) : (stream.ReadByte() != -1);
        }
    }
}
