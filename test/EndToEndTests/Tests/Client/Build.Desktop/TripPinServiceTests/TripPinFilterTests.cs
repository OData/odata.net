//---------------------------------------------------------------------
// <copyright file="TripPinFilterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.ODataWCFServiceTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TripPinFilterTests : ODataWCFServiceTestsBase<Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference.InMemoryEntities>
    {
        private const string NameSpacePrefix = "Microsoft.OData.SampleService.Models.TripPin.";

        public TripPinFilterTests()
            : base(ServiceDescriptors.TripPinServiceDescriptor)
        {

        }

        [TestMethod]
        public void TripPinFilterDefaultStringFunctions()
        {
            #region Contains
            string uri = "People?$filter=contains(UserName,'v')";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = item.Properties.Single(p => p.Name == "UserName").Value;
                Assert.IsTrue(userName.ToString().Contains("v"));
            });
            #endregion

            #region endswith
            uri = "People?$filter=endswith(UserName,'chum')";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = item.Properties.Single(p => p.Name == "UserName").Value;
                Assert.IsTrue(userName.ToString().Contains("chum"));
                Assert.IsTrue(userName.ToString().EndsWith("chum"));
            });
            #endregion

            #region startswith
            uri = "People?$filter=startswith(UserName,'v')";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = item.Properties.Single(p => p.Name == "UserName").Value;
                Assert.IsTrue(userName.ToString().Contains("v"));
                Assert.IsTrue(userName.ToString().StartsWith("v"));
            });
            #endregion

            #region length
            uri = "People?$filter=length(UserName) eq 12";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = item.Properties.Single(p => p.Name == "UserName").Value;
                Assert.AreEqual(userName.ToString().Length, 12);
            });
            #endregion

            #region indexof
            uri = "People?$filter=indexof(UserName,'vincent') eq 1";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = item.Properties.Single(p => p.Name == "UserName").Value;
                Assert.IsTrue(userName.ToString().Contains("vincent"));
                Assert.AreEqual(userName.ToString().IndexOf("vincent"), 1);
            });
            #endregion

            #region substring
            uri = "People?$filter=substring(UserName,1) eq 'incentcalabrese'";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = item.Properties.Single(p => p.Name == "UserName").Value;
                Assert.IsTrue(userName.ToString().Contains("incentcalabrese"));
                Assert.AreEqual(userName.ToString().Substring(1), "incentcalabrese");
            });

            uri = "People?$filter=substring(UserName,1,6) eq 'incent'";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = item.Properties.Single(p => p.Name == "UserName").Value;
                Assert.IsTrue(userName.ToString().Contains("incent"));
                Assert.AreEqual(userName.ToString().Substring(1, 6), "incent");
            });
            #endregion

            #region tolower
            uri = "People?$filter=tolower(UserName) eq 'vincentcalabrese'";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = item.Properties.Single(p => p.Name == "UserName").Value;
                Assert.AreEqual(userName.ToString(), "vincentcalabrese");
            });
            #endregion

            #region toupper
            uri = "People?$filter=toupper(UserName) eq 'VINCENTCALABRESE'";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = item.Properties.Single(p => p.Name == "UserName").Value;
                Assert.AreEqual(userName.ToString().ToUpper(), "VINCENTCALABRESE");
            });
            #endregion

            #region trim
            uri = "People?$filter=trim(UserName) eq 'vincentcalabrese'";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = item.Properties.Single(p => p.Name == "UserName").Value;
                Assert.AreEqual(userName.ToString(), "vincentcalabrese");
            });
            #endregion

            #region concat
            uri = "People?$filter=concat(concat(FirstName,', '), LastName) eq 'Vincent, Calabrese'";
            requestAndCheckResult(uri, (item) =>
            {
                var firstName = item.Properties.Single(p => p.Name == "FirstName").Value;
                var lastName = item.Properties.Single(p => p.Name == "LastName").Value;
                Assert.AreEqual(string.Concat(string.Concat(firstName, ", "), lastName), "Vincent, Calabrese");
            });
            #endregion
        }

        [TestMethod]
        public void TripPinFilterDefaultDateTimeFunctions()
        {
            #region year
            string uri = "People('russellwhyte')/Trips?$filter=year(StartsAt) eq 2014";
            requestAndCheckResult(uri, (item) =>
            {
                var property = item.Properties.Single(p => p.Name == "StartsAt").Value;
                var startTime = (DateTimeOffset)property;
                Assert.AreEqual(startTime.Year, 2014);
            });
            #endregion

            #region month
            uri = "People('russellwhyte')/Trips?$filter=month(StartsAt) eq 2";
            requestAndCheckResult(uri, (item) =>
            {
                var property = item.Properties.Single(p => p.Name == "StartsAt").Value;
                var startTime = (DateTimeOffset)property;
                Assert.AreEqual(startTime.Month, 2);
            });
            #endregion

            #region day
            uri = "People('russellwhyte')/Trips?$filter=day(StartsAt) eq 1";
            requestAndCheckResult(uri, (item) =>
            {
                var property = item.Properties.Single(p => p.Name == "StartsAt").Value;
                var startTime = (DateTimeOffset)property;
                Assert.AreEqual(startTime.Day, 1);
            });
            #endregion

            #region hour
            uri = "People('russellwhyte')/Trips?$filter=hour(StartsAt) eq 0";
            requestAndCheckResult(uri, (item) =>
            {
                var property = item.Properties.Single(p => p.Name == "StartsAt").Value;
                var startTime = (DateTimeOffset)property;
                Assert.AreEqual(startTime.Hour, 0);
            });
            #endregion

            #region minute
            uri = "People('russellwhyte')/Trips?$filter=minute(StartsAt) eq 0";
            requestAndCheckResult(uri, (item) =>
            {
                var property = item.Properties.Single(p => p.Name == "StartsAt").Value;
                var startTime = (DateTimeOffset)property;
                Assert.AreEqual(startTime.Minute, 0);
            });
            #endregion

            #region second
            uri = "People('russellwhyte')/Trips?$filter=second(StartsAt) eq 0";
            requestAndCheckResult(uri, (item) =>
            {
                var property = item.Properties.Single(p => p.Name == "StartsAt").Value;
                var startTime = (DateTimeOffset)property;
                Assert.AreEqual(startTime.Second, 0);
            });
            #endregion
        }

        [TestMethod]
        public void TripPinFilterDefaultMathFunctions()
        {
            #region round
            string uri = "People?$filter=round(12.66) eq 12";
            requestAndCheckResult(uri, (item) =>
            {
                Assert.AreEqual(Math.Round(12.66), 13);
            });
            #endregion

            #region floor
            uri = "People?$filter=floor(12.66) eq 12";
            requestAndCheckResult(uri, (item) =>
            {
                Assert.AreEqual(Math.Floor(12.66), 12);
            });
            #endregion

            #region ceiling
            uri = "People?$filter=ceiling(12.66) eq 12";
            requestAndCheckResult(uri, (item) =>
            {
                Assert.AreEqual(Math.Ceiling(12.66), 13);
            });
            #endregion
        }

        [TestMethod]
        public void TripPinFilterDefaultTypeFunctions()
        {
            #region isof
            string uri = "People?$filter=isof(UserName,Edm.String)";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = item.Properties.Single(p => p.Name == "UserName").Value;
                Assert.AreEqual(userName.GetType(), typeof(String));
            });

            //TODO: should remove the single quotation marks (' '), this should be ODL bug
            uri = string.Format("People?$filter=isof(Gender,'{0}PersonGender')", NameSpacePrefix);
            requestAndCheckResult(uri, (item) =>
            {
                var gender = item.Properties.Single(p => p.Name == "Gender").Value;

                Assert.AreEqual(gender.GetType(), typeof(ODataEnumValue));
                Assert.AreEqual((gender as ODataEnumValue).TypeName, "Microsoft.OData.SampleService.Models.TripPin.PersonGender");
            });

            //TODO: should remove the single quotation marks (' '), this should be ODL bug
            uri = string.Format("People?$filter=isof('{0}Person')", NameSpacePrefix);
            requestAndCheckFeedResult(uri, (feed) =>
            {
                Assert.IsTrue(feed.Count > 0);
            });

            uri = string.Format("People('russellwhyte')/Trips(0)/PlanItems?$filter=isof('{0}Flight')", NameSpacePrefix);
            requestAndCheckFeedResult(uri, (feed) =>
            {
                Assert.IsTrue(feed.Count > 0);
            });
            #endregion

            #region cast
            uri = "People?$filter=cast(UserName, Edm.String) eq 'vincentcalabrese'";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = item.Properties.Single(p => p.Name == "UserName").Value;
                Assert.AreEqual(userName.ToString(), "vincentcalabrese");
            });

            uri = "People('russellwhyte')/Trips?$filter=cast(TripId, Edm.Int32) eq 1001";
            requestAndCheckResult(uri, (item) =>
            {
                var tripId = item.Properties.Single(p => p.Name == "TripId").Value;
                Assert.AreEqual((int)tripId, 1001);
            });

            uri = "People('russellwhyte')/Trips?$filter=cast(TripId, Edm.String) eq '1001'";
            requestAndCheckResult(uri, (item) =>
            {
                var tripId = item.Properties.Single(p => p.Name == "TripId").Value;
                Assert.AreEqual(tripId.ToString(), "1001");
            });

            uri = "People('russellwhyte')/Trips?$filter=cast(TripId, Edm.String) eq '1001'";
            requestAndCheckResult(uri, (item) =>
            {
                var tripId = item.Properties.Single(p => p.Name == "TripId").Value;
                Assert.AreEqual(tripId.ToString(), "1001");
            });

            //TODO, should add parameter with Doulbe type in trip pin.
            uri = string.Format("People?$filter=cast(12.66, Edm.Int32) eq 13");
            requestAndCheckFeedResult(uri, (feed) =>
            {
                Assert.IsTrue(feed.Count > 0);
            });

            //Only support convert enum to string
            uri = string.Format("People?$filter=cast(Gender, Edm.String) eq 'Male'");
            requestAndCheckResult(uri, (item) =>
            {
                var gender = item.Properties.Single(p => p.Name == "Gender").Value;

                Assert.AreEqual(gender.GetType(), typeof(ODataEnumValue));
                Assert.AreEqual((gender as ODataEnumValue).TypeName, "Microsoft.OData.SampleService.Models.TripPin.PersonGender");
                Assert.AreEqual((gender as ODataEnumValue).Value, "Male");
            });

            uri = string.Format("People?$filter=cast('Male', {0}PersonGender) eq {0}PersonGender'Male'", NameSpacePrefix);
            requestAndCheckFeedResult(uri, (feed) =>
            {
                Assert.IsTrue(feed.Count > 0);
            });
            #endregion
        }

        [TestMethod]
        public void TripPinFilterDefaultGeoFunctions()
        {
            //No data for test.
            Assert.IsTrue(true);
        }

        #region private methods
        private void requestAndCheckResult(string uri, Action<ODataResource> verify)
        {
            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    List<ODataResource> entries = QueryFeed(uri, mimeType);
                    entries.ForEach(item =>
                    {
                        verify.Invoke(item);
                    });
                }
            }
        }

        private void requestAndCheckFeedResult(string uri, Action<List<ODataResource>> verify)
        {
            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    List<ODataResource> entries = QueryFeed(uri, mimeType);
                    verify.Invoke(entries);
                }
            }
        }

        private List<ODataResource> QueryFeed(string requestUri, string mimeType)
        {
            List<ODataResource> entries = new List<ODataResource>();

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + requestUri, UriKind.Absolute));
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                {
                    var reader = messageReader.CreateODataResourceSetReader();

                    while (reader.Read())
                    {
                        if (reader.State == ODataReaderState.ResourceEnd)
                        {
                            ODataResource entry = reader.Item as ODataResource;
                            entries.Add(entry);
                        }
                        else if (reader.State == ODataReaderState.ResourceSetEnd)
                        {
                            Assert.IsNotNull(reader.Item as ODataResourceSet);
                        }
                    }

                    Assert.AreEqual(ODataReaderState.Completed, reader.State);
                }
            }
            return entries;
        }
        #endregion
    }
}
