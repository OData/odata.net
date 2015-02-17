//---------------------------------------------------------------------
// <copyright file="PlainSerializersTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections;
    using System.Data.Test.Astoria;
    using System.IO;
    using System.Net;
    using AstoriaUnitTests.Data;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for BinarySerialize and TextSerializer and is intended
    /// to contain all Unit Tests for them.
    /// </summary>
    [TestClass]
    public class PlainSerializersTest
    {
        [TestCategory("Partition2"), TestMethod]
        public void PlainSerializersBasicTest()
        {
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("ValidMime", new object[] { true, false }),
                new Dimension("SpecificMime", new object[] { true, false }),
                new Dimension("TypeData", TypeData.Values));
            Hashtable table = new Hashtable();
            while (engine.Next(table))
            {
                bool validMime = (bool)table["ValidMime"];
                bool specificMime = (bool)table["SpecificMime"];
                TypeData typeData = (TypeData)table["TypeData"];

                // If ValidMime is false, whether it's specific or not doesn't matter.
                if (!validMime && specificMime)
                {
                    continue;
                }

                if (!typeData.IsTypeSupported)
                {
                    continue;
                }

                using (TestWebRequest request = TestWebRequest.CreateForLocation(WebServerLocation.InProcess))
                {
                    Type valueType = typeData.ClrType;
                    Type entityType = typeof(TypedEntity<,>).MakeGenericType(typeof(int), valueType);
                    CustomDataContextSetup dataContextSetup = new CustomDataContextSetup(entityType);
                    dataContextSetup.Id = 1;
                    dataContextSetup.MemberValue = typeData.NonNullValue;

                    Type serviceType = dataContextSetup.DataServiceType;
                    request.DataServiceType = serviceType;
                    request.RequestUriString = "/Values(1)/Member/$value";
                    if (validMime)
                    {
                        request.Accept = TypeData.FindForType(valueType).DefaultContentType;
                    }
                    else
                    {
                        request.Accept = "application/unexpected";
                    }

                    try
                    {
                        request.SendRequest();
                        if (!validMime)
                        {
                            Assert.Fail("Request should have failed.");
                        }
                    }
                    catch (WebException)
                    {
                        if (!validMime)
                        {
                            continue;
                        }
                        throw;
                    }

                    string expectedType = request.Accept;
                    Assert.AreEqual(expectedType, TestUtil.GetMediaType(request.ResponseContentType));
                    
                    Stream stream = request.GetResponseStream();
                    if (valueType == typeof(byte[]))
                    {
                        byte[] bytes = (byte[])dataContextSetup.MemberValue;
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            Assert.AreEqual(bytes[i], (byte)stream.ReadByte());
                        }
                    }
                    else
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            typeData.VerifyAreEqual(dataContextSetup.MemberValue, typeData.ValueFromXmlText(reader.ReadToEnd(), request.Accept), request.Accept);
                        }
                    }
                }
            }
        }

        [TestCategory("Partition2"), TestMethod]
        public void PlainSerializersMimeOverrideTest()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(CustomDataContext);
                request.RequestUriString = "/Customers(1)/NameAsHtml/$value";
                request.SendRequest();
                Assert.AreEqual("text/html", TestUtil.GetMediaType(request.ResponseContentType));
                Assert.IsTrue(request.GetResponseStreamAsText().Contains("<html>"), "Payloads contains '<html>'");
            }

            // Make sure an incompatible accept header causes a content negotiation failure.
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.DataServiceType = typeof(CustomDataContext);
                request.RequestUriString = "/Customers(1)/NameAsHtml/$value";
                request.Accept = "text/plain";
                Exception exception = System.Data.Test.Astoria.TestUtil.RunCatching(request.SendRequest);
                Assert.AreEqual(415, request.ResponseStatusCode);
                Assert.AreEqual( DataServicesResourceUtil.GetString("DataServiceException_UnsupportedMediaType"), exception.InnerException.Message);
            }
        }
    }
}
