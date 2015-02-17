//---------------------------------------------------------------------
// <copyright file="JsonLightIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System.Data.Test.Astoria;
    using System.IO;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JsonLightIntegrationTests
    {
        [TestCategory("Partition2")]
        [TestMethod]
        public void JsonLightPayloadMetadataIntegrationTest()
        {
            Stream resultStream = UnitTestsUtil.GetResponseStream(WebServerLocation.InProcess, "application/json;odata.metadata=none", "/Customers?$select=Name", typeof(CustomDataContext));
            Stream stream = TestUtil.EnsureStreamWithSeek(resultStream);
            string actualText = new StreamReader(stream).ReadToEnd();

            const string expectedSuccessText = @"{""value"":[{""Name"":""Customer 0""},{""Name"":""Customer 1""},{""Name"":""Customer 2""}]}";
            Assert.AreEqual(expectedSuccessText, actualText);

            // now test that it is fails for the query option
            // $controlinfo was briefly used for controlling how much metadata a client wanted
            // in JSON-Light payloads. It was removed and replaced with a parameter in the media type.
            using (TestWebRequest request = TestWebRequest.CreateForLocation(WebServerLocation.InProcessWcf))
            {
                request.DataServiceType = typeof(CustomDataContext);
                request.RequestUriString = "/Customers?$controlinfo=all";
                request.Accept = "application/json;odata.metadata=minimal";

                TestUtil.RunCatching(() => request.SendRequest());

                Assert.AreEqual(400, request.ResponseStatusCode);
            }

            // now test that it is fails for 'odata-light' which was also eventually replaced.
            using (TestWebRequest request = TestWebRequest.CreateForLocation(WebServerLocation.InProcessWcf))
            {
                request.DataServiceType = typeof(CustomDataContext);
                request.RequestUriString = "/Customers";
                request.Accept = "application/json;odata.metadata=light";

                TestUtil.RunCatching(() => request.SendRequest());

                Assert.AreEqual(415, request.ResponseStatusCode);
            }

            // now test that it is fails with 'metadata' parameter which was later combined into the 'odata' parameter.
            using (TestWebRequest request = TestWebRequest.CreateForLocation(WebServerLocation.InProcessWcf))
            {
                request.DataServiceType = typeof(CustomDataContext);
                request.RequestUriString = "/Customers";
                request.Accept = "application/json;odata.metadata=minimal;metadata=all";

                TestUtil.RunCatching(() => request.SendRequest());

                Assert.AreEqual(415, request.ResponseStatusCode);
            }

            // Now test that it fails for an invalid type/subtype.
            using (TestWebRequest request = TestWebRequest.CreateForLocation(WebServerLocation.InProcessWcf))
            {
                request.DataServiceType = typeof(CustomDataContext);
                request.RequestUriString = "/Customers";
                request.Accept = "fake/things;odata.metadata=minimal";

                TestUtil.RunCatching(() => request.SendRequest());

                Assert.AreEqual(415, request.ResponseStatusCode);
            }
        }
    }
}