//---------------------------------------------------------------------
// <copyright file="TripPinServiceTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.ODataWCFServiceTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Test.OData.Services.TestServices.TrippinServiceReference;

    [TestClass]
    public class TripPinServiceTests : ODataWCFServiceTestsBase<DefaultContainer>
    {
        private const string NameSpacePrefix = "Microsoft.OData.SampleService.Models.TripPin.";
        private int lastResponseStatusCode;

        public TripPinServiceTests()
            : base(ServiceDescriptors.TripPinServiceDescriptor)
        {
        }

        public TestContext TestContext { get; set; }

        #region General cases

        /// <summary>
        /// Send query and verify the results from the service implemented using ODataLib and EDMLib.
        /// </summary>
        [TestMethod]
        public void QueryEntitySet()
        {
            string uri = "People";

            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var resources = new QueryFeedHelper(this) { RequestUri = uri, MimeType = mimeType }.Execute();

                    var entries = resources.Where(e => e != null && e.Id != null);
                    Assert.AreEqual(20, entries.Count());
                    Assert.AreEqual("Russell", entries.ElementAt(0).Properties.Single(p => p.Name == "FirstName").Value);
                }
            }
        }

        [TestMethod]
        public void QueryEntity()
        {
            string uri = "People('russellwhyte')";

            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    ODataResource entry = new QueryEntryHelper(this) { RequestUri = uri, MimeType = mimeType, IfMatch = "*" }.Execute();

                    Assert.IsNotNull(entry);
                    Assert.AreEqual("Russell", entry.Properties.Single(p => p.Name == "FirstName").Value);
                }
            }
        }

        [TestMethod]
        public void QueryContainedNavigationProperty()
        {
            string uri = "People('russellwhyte')/Trips";

            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var entries = new QueryFeedHelper(this) { RequestUri = uri, MimeType = mimeType }.Execute();

                    Assert.AreEqual(3, entries.Count);
                    Assert.AreEqual(0, entries[0].Properties.Single(p => p.Name == "TripId").Value);
                }
            }
        }

        [TestMethod]
        public void QueryNavigationProperty()
        {
            string uri = "People('russellwhyte')/Friends";

            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var resources = new QueryFeedHelper(this) { RequestUri = uri, MimeType = mimeType }.Execute();

                    var entries = resources.Where(e => e != null && e.Id != null);
                    Assert.AreEqual(4, entries.Count());
                    Assert.AreEqual("scottketchum", entries.ElementAt(0).Properties.Single(p => p.Name == "UserName").Value);
                }
            }
        }

        [TestMethod]
        public void CascadeNavigationProperty()
        {
            string uri = string.Format("People('scottketchum')/Trips(0)/PlanItems(11)/{0}Flight/Airline", NameSpacePrefix);

            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    ODataResource entry = new QueryEntryHelper(this) { RequestUri = uri, MimeType = mimeType, IfMatch = "*" }.Execute();
                    Assert.IsNotNull(entry);
                    Assert.AreEqual("AA", entry.Properties.Single(p => p.Name == "AirlineCode").Value);
                }
            }
        }

        [TestMethod]
        public void TestAnyEndsWith()
        {
            string uri = "People?$filter=Emails/any(s:endswith(s, 'll@example.com'))";

            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    int count = 0;
                    List<ODataResource> entries = new List<ODataResource>();
                    new QueryFeedHelper(this)
                    {
                        RequestUri = uri,
                        MimeType = mimeType,
                        ReadEntryEnd = argument =>
                        {
                            if (argument.ParentName == null)
                            {
                                count++;
                                entries.Add(argument.Entry);
                            }
                        }
                    }.Execute();

                    Assert.AreEqual(2, count);// russellwhyte and marshallgaray
                    Assert.AreEqual("russellwhyte", entries[0].Properties.Single(p => p.Name == "UserName").Value);
                }
            }
        }

        [TestMethod]
        public void OpenEntity()
        {
            string[] userNames = { "scottketchum", "russellwhyte", "ronaldmundy" };
            for (int i = 0; i < mimeTypes.Length; i++)
            {
                ODataResource entry = new ODataResource() { TypeName = NameSpacePrefix + "Person" };
                entry.Properties = new[]
                {
                    new ODataProperty
                    {
                        Name = "NickName",
                        Value = "NickName"
                    }
                };
                var settings = new ODataMessageWriterSettings();
                settings.BaseUri = ServiceBaseUri;
                var personType = Model.FindDeclaredType(NameSpacePrefix + "Person") as IEdmEntityType;
                var personSet = Model.EntityContainer.FindEntitySet("People");

                var uri = string.Format("People('{0}')", userNames[i]);
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + uri));
                requestMessage.SetHeader("Content-Type", mimeTypes[i]);
                requestMessage.SetHeader("Accept", mimeTypes[i]);
                requestMessage.SetHeader("If-Match", "*");
                requestMessage.Method = "PATCH";

                using (var messageWriter = new ODataMessageWriter(requestMessage, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter(personSet, personType);
                    odataWriter.WriteStart(entry);
                    odataWriter.WriteEnd();
                }

                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(204, responseMessage.StatusCode);
                if (!mimeTypes[i].Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    entry = new QueryEntryHelper(this) { RequestUri = string.Format("People('{0}')", userNames[i]), MimeType = mimeTypes[i], IfMatch = "*" }.Execute();
                    var nickName = entry.Properties.Single(p => p.Name == "NickName").Value;
                    Assert.AreEqual("\"NickName\"", (entry.Properties.Single(p => p.Name == "NickName").Value as ODataUntypedValue).RawValue);
                }
            }
        }

        [TestMethod]
        public void OpenComplexType()
        {
            for (int i = 0; i < mimeTypes.Length; i++)
            {
                var occursAt = new ODataResource
                {
                    TypeName = NameSpacePrefix + "EventLocation",
                    Properties = new[]
                    {
                        new ODataProperty
                        {
                            Name = "RoomNumber",
                            Value = 100 + i
                        }
                    }
                };

                var settings = new ODataMessageWriterSettings();
                settings.BaseUri = ServiceBaseUri;
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri +
                    string.Format("People('russellwhyte')/Trips(0)/PlanItems(12)/{0}Event/OccursAt", NameSpacePrefix)));
                requestMessage.SetHeader("Content-Type", mimeTypes[i]);
                requestMessage.SetHeader("Accept", mimeTypes[i]);
                requestMessage.Method = "PATCH";

                using (var messageWriter = new ODataMessageWriter(requestMessage, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter();
                    odataWriter.WriteStart(occursAt);
                    odataWriter.WriteEnd();
                    odataWriter.Flush();
                }

                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(204, responseMessage.StatusCode);

                if (!mimeTypes[i].Contains(MimeTypes.ODataParameterNoMetadata))
                {

                    var entry = new QueryEntryHelper(this)
                    {
                        RequestUri = string.Format("People('russellwhyte')/Trips(0)/PlanItems(12)/{0}Event", NameSpacePrefix),
                        MimeType = mimeTypes[i],
                        IfMatch = "*",
                        ReadEntryEnd = argument =>
                            {
                                if (argument.Entry.TypeName.EndsWith("EventLocation"))
                                {
                                    Assert.AreEqual("" + (100 + i), (argument.Entry.Properties.Single(p => p.Name == "RoomNumber").Value as ODataUntypedValue).RawValue);
                                }
                            }
                    }.Execute();
                }
            }
        }

        [TestMethod]
        public void TestAddTripPinNewPerson()
        {
            var mimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata;

            var personEntry = new ODataResource() { TypeName = NameSpacePrefix + "Person" };
            var personUserName = new ODataProperty { Name = "UserName", Value = "VincentZhao" };
            var personFirstName = new ODataProperty { Name = "FirstName", Value = "Zhao" };
            var personLastName = new ODataProperty { Name = "LastName", Value = "Vincent" };
            var personGender = new ODataProperty
            {
                Name = "Gender",
                Value = new ODataEnumValue("0", NameSpacePrefix + "PersonGender")
            };

            var personEmailAddress = new ODataProperty
            {
                Name = "Emails",
                Value = new ODataCollectionValue()
                {
                    TypeName = "Collection(Edm.String)",
                    Items = new[] { "v-vinz@microsft.com", "sqlOdata@microsoft.com" }
                }
            };

            var personAddressInfo_NestedInfo = new ODataNestedResourceInfo()
            {
                Name = "AddressInfo",
                IsCollection = true
            };

            var personAddressInfo_ResourceSet = new ODataResourceSet()
            {
                TypeName = string.Format("Collection({0}Location)", NameSpacePrefix)
            };

            var personAddressInfo_addresses =
                new[] {
                        GenerateLocationInfo("999 zixing"),
                        GenerateLocationInfo("200 xujiahui")
                    };
            personAddressInfo_ResourceSet.SetAnnotation(personAddressInfo_addresses);
            personAddressInfo_NestedInfo.SetAnnotation(personAddressInfo_ResourceSet);

            personEntry.Properties = new[] { personUserName, personFirstName, personLastName, personGender, personEmailAddress };

            var settings = new ODataMessageWriterSettings();
            settings.BaseUri = ServiceBaseUri;

            var personType = Model.FindDeclaredType(NameSpacePrefix + "Person") as IEdmEntityType;
            var personSet = Model.EntityContainer.FindEntitySet("People");

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "People"));
            requestMessage.SetHeader("Content-Type", mimeType);
            requestMessage.SetHeader("Accept", mimeType);
            requestMessage.Method = "POST";
            using (var messageWriter = new ODataMessageWriter(requestMessage, settings))
            {
                var odataWriter = messageWriter.CreateODataResourceWriter(personSet, personType);
                odataWriter.WriteStart(personEntry);
                odataWriter.WriteStart(personAddressInfo_NestedInfo);
                odataWriter.WriteStart(personAddressInfo_ResourceSet);
                foreach (var addressInfo in personAddressInfo_addresses)
                {
                    odataWriter.WriteStart(addressInfo);
                    var city_NestedInfo = addressInfo.GetAnnotation<ODataNestedResourceInfo>();
                    if (city_NestedInfo != null)
                    {
                        odataWriter.WriteStart(city_NestedInfo);
                        var city = city_NestedInfo.GetAnnotation<ODataResource>();
                        if (city != null)
                        {
                            odataWriter.WriteStart(city);
                            odataWriter.WriteEnd();
                        }

                        odataWriter.WriteEnd();
                    }

                    odataWriter.WriteEnd();
                }

                odataWriter.WriteEnd();
                odataWriter.WriteEnd();
                odataWriter.WriteEnd();
            }

            var responseMessage = requestMessage.GetResponse();

            // verify the insert
            Assert.AreEqual(201, responseMessage.StatusCode);
            var entry = new QueryEntryHelper(this) { RequestUri = "People('VincentZhao')", MimeType = mimeType, IfMatch = "*" }.Execute();
            Assert.AreEqual("VincentZhao", entry.Properties.Single(p => p.Name == "UserName").Value);
        }

        [TestMethod]
        public void TestAnyOnEntitySet()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                var entries = new QueryFeedHelper(this)
                {
                    RequestUri = "People?$filter=Trips/any(f:f/Budget gt 100)",
                    MimeType = mimeType,
                    ExpectedStatusCode = 200,
                }.Execute();

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(10, entries.Where(e => e != null && e.Id != null).Count());
                }
            }
        }

        [TestMethod]
        public void TestAnyOnEntityCollectionProperty()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    List<ODataResource> entries = new List<ODataResource>();
                    new QueryFeedHelper(this)
                    {
                        RequestUri = "Me/Friends?$filter=Friends/any(f:f/FirstName eq 'Scott')",
                        MimeType = mimeType,
                        ExpectedStatusCode = 200,
                        ReadEntryEnd = argument =>
                        {
                            if (argument.ParentName == null)
                            {
                                entries.Add(argument.Entry);
                            }
                        }
                    }.Execute();

                    Assert.AreEqual(2, entries.Where(e => e != null && e.Id != null).Count());
                    Assert.IsNotNull(entries.SingleOrDefault(e => (string)e.Properties.Single(p => p.Name == "UserName").Value == "russellwhyte"));
                    Assert.IsNotNull(entries.SingleOrDefault(e => (string)e.Properties.Single(p => p.Name == "UserName").Value == "ronaldmundy"));
                }
            }
        }

        [TestMethod]
        public void TestAllOnEntitySet()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                var entries = new QueryFeedHelper(this)
                {
                    RequestUri = "People?$filter=Trips/all(f:f/Budget gt 5000)",
                    MimeType = mimeType,
                    ExpectedStatusCode = 200,
                }.Execute();

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(11, entries.Count);
                }
            }
        }

        [TestMethod]
        public void TestAllOnEntityCollectionProperty()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var entries = new QueryFeedHelper(this)
                    {
                        RequestUri = "Me/Friends?$filter=Friends/all(f:f/FirstName eq 'Krista')",
                        MimeType = mimeType,
                        ExpectedStatusCode = 200,
                    }.Execute();

                    // the result includes empty friends due to the all Lambda operator is translated to Enumerable.All method which returns true for empty collection
                    Assert.AreEqual(3, entries.Where(e => e != null && e.Id != null).Count());
                }
            }
        }

        #endregion

        #region client test

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        // Move to another service, instead of trippin service.
        // [TestMethod] // github issuse: #896
        public void TestNullableCollection()
        {
            var people = TestClientContext.People.First();
            Assert.AreEqual(2, people.AddressInfo.Count());
            Assert.IsNull(people.AddressInfo[1]);

            Assert.AreEqual(3, people.Emails.Count());
            Assert.IsNull(people.Emails[2]);

            people.AddressInfo.Add(null);
            people.Emails.Add(null);
            TestClientContext.UpdateObject(people);
            TestClientContext.SaveChanges();
    }
#endif

        #endregion

        #region function/action

        [TestMethod]
        public void TestActionTripPinShareTrip()
        {
            var writerSettings = new ODataMessageWriterSettings();
            writerSettings.BaseUri = ServiceBaseUri;
            var readerSettings = new ODataMessageReaderSettings();
            readerSettings.BaseUri = ServiceBaseUri;

            string partUri = string.Format("People('russellwhyte')/{0}ShareTrip", NameSpacePrefix);

            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + partUri));
                requestMessage.SetHeader("Content-Type", MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata);
                requestMessage.SetHeader("Accept", mimeType);
                requestMessage.Method = "POST";
                using (var messageWriter = new ODataMessageWriter(requestMessage, writerSettings, Model))
                {
                    var odataWriter = messageWriter.CreateODataParameterWriter((IEdmOperation)null);
                    odataWriter.WriteStart();
                    odataWriter.WriteValue("userName", "scottketchum");
                    odataWriter.WriteValue("tripId", 0);
                    odataWriter.WriteEnd();
                }

                // send the http request
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(204, responseMessage.StatusCode);
            }
        }

        [TestMethod]
        public void TestFunctionGetFavoriteAirline()
        {
            string uri = string.Format("People('scottketchum')/{0}GetFavoriteAirline()", NameSpacePrefix);

            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    ODataResource entry = new QueryEntryHelper(this) { RequestUri = uri, MimeType = mimeType, IfMatch = "*" }.Execute();
                    Assert.IsNotNull(entry);
                    Assert.AreEqual("AA", entry.Properties.Single(p => p.Name == "AirlineCode").Value);
                }
            }
        }

        [TestMethod]
        public void TestFunctionGetInvolvedPeople()
        {
            string uri = string.Format("People('russellwhyte')/Trips(0)/{0}GetInvolvedPeople()", NameSpacePrefix);

            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var resources = new QueryFeedHelper(this) { RequestUri = uri, MimeType = mimeType }.Execute();

                    var entries = resources.Where(e => e != null && e.Id != null);
                    Assert.AreEqual(2, entries.Count());
                    Assert.AreEqual("scottketchum", entries.ElementAt(1).Properties.Single(p => p.Name == "UserName").Value);
                }
            }
        }

        [TestMethod]
        public void TestFunctionGetFriendsTrips()
        {
            string uri = string.Format("People('russellwhyte')/{0}GetFriendsTrips(userName='ronaldmundy')", NameSpacePrefix);

            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var entries = new QueryFeedHelper(this) { RequestUri = uri, MimeType = mimeType }.Execute();

                    Assert.AreEqual(1, entries.Count);
                    Assert.AreEqual(new Guid("dd6a09c0-e59b-4745-8612-f4499b676c47"), entries[0].Properties.Single(p => p.Name == "ShareId").Value);
                }
            }
        }

        [TestMethod]
        public void TestFunctionGetNearestAirport()
        {
            foreach (var mimeType in mimeTypes)
            {
                string uri = "GetNearestAirport(lat = 33, lon = -118)";

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    ODataResource entry = new QueryEntryHelper(this) { RequestUri = uri, MimeType = mimeType, IfMatch = "*" }.Execute();
                    Assert.IsNotNull(entry);
                    Assert.AreEqual("KLAX", entry.Properties.Single(p => p.Name == "IcaoCode").Value);
                }

                uri = "GetNearestAirport(lat = 37, lon =-122)";

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    ODataResource entry = new QueryEntryHelper(this) { RequestUri = uri, MimeType = mimeType, IfMatch = "*" }.Execute();
                    Assert.IsNotNull(entry);
                    Assert.AreEqual("KSFO", entry.Properties.Single(p => p.Name == "IcaoCode").Value);
                }
            }
        }

        #endregion

        #region $search Tests

        [TestMethod]
        public void BasicSearchTest()
        {
            var predicate = new Func<IEnumerable<ODataResource>, string, bool>((entries, icaoCode) =>
            {
                return entries.Count(entry => entry.Properties.Count(p => p.Name == "IcaoCode" && object.Equals(p.Value, icaoCode)) == 1) == 1;
            });

            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var entries = new QueryFeedHelper(this) { RequestUri = "Airports?$search=\"United States\"", MimeType = mimeType }.Execute();
                    Assert.AreEqual(6, entries.Where(e => e != null && e.Id != null).Count());
                    Assert.IsTrue(predicate(entries, "KSFO"));
                    Assert.IsTrue(predicate(entries, "KLAX"));
                    Assert.IsTrue(predicate(entries, "KJFK"));
                    Assert.IsTrue(predicate(entries, "KATL"));
                    Assert.IsTrue(predicate(entries, "KSEA"));
                    Assert.IsTrue(predicate(entries, "KORD"));
                }
            }
        }

        [TestMethod]
        public void GroupSearchTest()
        {
            var predicate = new Func<IEnumerable<ODataResource>, string, bool>((entries, userName) =>
            {
                return entries.Count(entry => entry.Properties.Count(p => p.Name == "UserName" && object.Equals(p.Value, userName)) == 1) == 1;
            });

            {
                var entries = new QueryFeedHelper(this) { RequestUri = "People?$search=(Male OR Female) AND NOT \"@contoso.com\"", MimeType = MimeTypes.ApplicationJson }.Execute();
                Assert.AreEqual(4, entries.Where(e => e != null && e.Id != null).Count());
                Assert.IsTrue(predicate(entries, "scottketchum"));
                Assert.IsTrue(predicate(entries, "clydeguess"));
                Assert.IsTrue(predicate(entries, "angelhuffman"));
                Assert.IsTrue(predicate(entries, "kristakemp"));
            }
        }

        [TestMethod]
        public void ExpandSearchTest()
        {
            const string PersonTypeName = NameSpacePrefix + "Person";
            const string TripTypeName = NameSpacePrefix + "Trip";

            var predicate = new Func<IEnumerable<ODataResource>, string, int, bool>((entries, typeName, count) =>
            {
                return entries.Count(entry => entry.TypeName == typeName) == count;
            });

            {
                var entries = new QueryFeedHelper(this) { RequestUri = "People?$expand=Trips($search=Shanghai OR Beijing)&$search=Male", MimeType = MimeTypes.ApplicationJson }.Execute();
                Assert.IsTrue(predicate(entries, PersonTypeName, 10));
                Assert.IsTrue(predicate(entries, TripTypeName, 3));
            }
        }

        #endregion

        #region Server-driven paging cases

        [TestMethod]
        public void SimpleUriNextLinkTest()
        {
            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var entries = new QueryFeedHelper(this)
                    {
                        RequestUri = "People",
                        MimeType = mimeType,
                        RequestedHandler = argument =>
                        {
                            if (argument.IsLastTime)
                            {
                                Assert.AreEqual(3, argument.Times);
                            }
                        }
                    }.Execute();

                    Assert.AreEqual(20, entries.Where(e => e != null && e.Id != null).Count());
                }
            }
        }

        [TestMethod]
        public void ComplexUriNextLinkTest()
        {
            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var entries = new QueryFeedHelper(this)
                    {
                        RequestUri = "People?$filter=FirstName ne 'Russell'",
                        MimeType = mimeType,
                        RequestedHandler = argument =>
                        {
                            if (argument.IsLastTime)
                            {
                                Assert.AreEqual(3, argument.Times);
                            }
                        }
                    }.Execute();

                    Assert.AreEqual(19, entries.Where(e => e != null && e.Id != null).Count());
                }
            }
        }

        [TestMethod]
        public void FullSinglePageNextLinkTest()
        {
            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var entries = new QueryFeedHelper(this)
                    {
                        RequestUri = "People?$top=8",
                        MimeType = mimeType,
                        RequestedHandler = argument =>
                        {
                            if (argument.IsLastTime)
                            {
                                Assert.AreEqual(1, argument.Times);
                            }
                        }
                    }.Execute();

                    Assert.AreEqual(8, entries.Where(e => e != null && e.Id != null).Count());
                }
            }
        }

        [TestMethod]
        public void SmallSinglePageNextLinkTest()
        {
            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var entries = new QueryFeedHelper(this)
                    {
                        RequestUri = "People?$top=3",
                        MimeType = mimeType,
                        RequestedHandler = argument =>
                        {
                            if (argument.IsLastTime)
                            {
                                Assert.AreEqual(1, argument.Times);
                            }
                        }
                    }.Execute();

                    Assert.AreEqual(3, entries.Where(e => e != null && e.Id != null).Count());
                }
            }
        }

        [TestMethod]
        public void BasicNextLinkHttpHeaderTest()
        {
            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var entries = new QueryFeedHelper(this)
                    {
                        RequestUri = "People",
                        MimeType = mimeType,
                        RequestingHandler = argument =>
                        {
                            if (argument.Times == 1)
                            {
                                argument.Request.SetHeader("Prefer", "odata.maxpagesize=8");
                            }
                        },
                        RequestedHandler = argument =>
                        {
                            var applied = false;
                            var prefer = argument.Response.GetHeader("Preference-Applied");
                            if (!string.IsNullOrEmpty(prefer))
                            {
                                applied = prefer.Contains("odata.maxpagesize=8");
                            }

                            if (argument.Times == 1)
                            {
                                Assert.IsTrue(applied);
                            }
                            else
                            {
                                Assert.IsFalse(applied);
                            }

                            if (argument.IsLastTime)
                            {
                                Assert.AreEqual(3, argument.Times);
                            }
                        }
                    }.Execute();

                    Assert.AreEqual(20, entries.Where(e => e != null && e.Id != null).Count());
                }
            }
        }

        [TestMethod]
        public void ComplexNextLinkHttpHeaderTest()
        {
            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var entries = new QueryFeedHelper(this)
                    {
                        RequestUri = "People",
                        MimeType = mimeType,
                        RequestingHandler = argument =>
                        {
                            if (argument.Times == 1)
                            {
                                argument.Request.SetHeader("Prefer", "odata.maxpagesize=10");
                            }
                        },
                        RequestedHandler = argument =>
                        {
                            var applied = false;
                            var prefer = argument.Response.GetHeader("Preference-Applied");
                            if (!string.IsNullOrEmpty(prefer))
                            {
                                applied = prefer.Contains("odata.maxpagesize=10");
                            }

                            if (argument.Times == 1)
                            {
                                Assert.IsTrue(applied);
                            }
                            else
                            {
                                Assert.IsFalse(applied);
                            }

                            if (argument.IsLastTime)
                            {
                                Assert.AreEqual(3, argument.Times);
                            }
                        }
                    }.Execute();

                    Assert.AreEqual(20, entries.Where(e => e != null && e.Id != null).Count());
                }
            }
        }

        [TestMethod]
        public void FunctionNextLinkTest()
        {
            var uri = string.Format("Me/Trips(2)/{0}GetInvolvedPeople()", NameSpacePrefix);

            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var entries = new QueryFeedHelper(this)
                    {
                        RequestUri = uri,
                        MimeType = mimeType,
                        RequestingHandler = argument =>
                        {
                            if (argument.Times == 1)
                            {
                                argument.Request.SetHeader("Prefer", "odata.maxpagesize=2");
                            }
                        },
                        RequestedHandler = argument =>
                        {
                            var applied = false;
                            var prefer = argument.Response.GetHeader("Preference-Applied");
                            if (!string.IsNullOrEmpty(prefer))
                            {
                                applied = prefer.Contains("odata.maxpagesize=2");
                            }

                            if (argument.Times == 1)
                            {
                                Assert.IsTrue(applied);
                            }
                            else
                            {
                                Assert.IsFalse(applied);
                            }

                            if (argument.IsLastTime)
                            {
                                Assert.AreEqual(2, argument.Times);
                            }
                        }
                    }.Execute();

                    Assert.AreEqual(3, entries.Where(e => e != null && e.Id != null).Count());
                }
            }
        }

        [TestMethod]
        public void EntityReferenceNextLinkTest()
        {
            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var entries = new QueryReferenceHelper(this) { RequestUri = "Me/Friends/$ref", MimeType = mimeType }.Execute();
                    Assert.AreEqual(20, entries.Count);
                }
            }
        }

        #endregion

        #region Filter with properties on derived types

        [TestMethod]
        public void FilterOnDerivedTypeTest()
        {
            var uri1 = string.Format("People('russellwhyte')/Trips(0)/PlanItems/{0}Flight?$filter=FlightNumber eq 'AA4035'", NameSpacePrefix);
            var uri2 = string.Format("People('scottketchum')/Trips(2004)/PlanItems?$filter={0}Flight/FlightNumber eq 'MU1930'", NameSpacePrefix);

            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var entries1 = new QueryFeedHelper(this) { RequestUri = uri1, MimeType = mimeType }.Execute();
                    Assert.AreEqual(1, entries1.Count);

                    var entries2 = new QueryFeedHelper(this) { RequestUri = uri2, MimeType = mimeType }.Execute();
                    Assert.AreEqual(1, entries2.Count);
                }
            }
        }

        #endregion

        #region ETag create

        [TestMethod]
        public void ETagCreate()
        {
            var entry = new CreateHelper(this)
            {
                RequestUri = "People",
                MimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                EntryToCreate = CreateEntry("newuser"),
                EntitySetName = "People",
                ValidationUri = "People('newuser')",
            }.Execute();
            AssertIsValidETag(entry.ETag);
        }

        #endregion

        #region ETag delete

        [TestMethod]
        public void ETagDeleteIfMatchOk()
        {
            const string uri = "People('russellwhyte')";
            var expectedETag = new QueryEntryHelper(this) { RequestUri = uri, MimeType = MimeTypes.ApplicationJsonLight }.Execute().ETag;

            new DeleteHelper(this)
            {
                RequestUri = uri,
                MimeType = MimeTypes.ApplicationJsonLight,
                ExpectedStatusCode = 204,
                IfMatch = expectedETag,
            }.Execute();
        }

        [TestMethod]
        public void ETagDeleteIfMatchAsterisk()
        {
            new DeleteHelper(this)
            {
                RequestUri = "People('russellwhyte')",
                MimeType = MimeTypes.ApplicationJsonLight,
                ExpectedStatusCode = 204,
                IfMatch = "*",
            }.Execute();
        }

        [TestMethod]
        public void ETagDeleteIfMatchWrong()
        {
            new DeleteHelper(this)
            {
                RequestUri = "People('russellwhyte')",
                MimeType = MimeTypes.ApplicationJsonLight,
                ExpectedStatusCode = 412,
                IfMatch = "W/\"123\"",
            }.Execute();
        }

        [TestMethod]
        public void ETagDeleteIfNoneMatchOk()
        {
            const string uri = "People('russellwhyte')";
            var expectedETag = new QueryEntryHelper(this) { RequestUri = uri, MimeType = MimeTypes.ApplicationJsonLight }.Execute().ETag;

            new DeleteHelper(this)
            {
                RequestUri = uri,
                MimeType = MimeTypes.ApplicationJsonLight,
                ExpectedStatusCode = 412,
                IfNoneMatch = expectedETag,
            }.Execute();
        }

        [TestMethod]
        public void ETagDeleteIfNoneMatchAsterisk()
        {
            new DeleteHelper(this)
            {
                RequestUri = "People('russellwhyte')",
                MimeType = MimeTypes.ApplicationJsonLight,
                ExpectedStatusCode = 412,
                IfNoneMatch = "*",
            }.Execute();
        }

        [TestMethod]
        public void ETagDeleteIfNoneMatchWrong()
        {
            new DeleteHelper(this)
            {
                RequestUri = "People('russellwhyte')",
                MimeType = MimeTypes.ApplicationJsonLight,
                ExpectedStatusCode = 204,
                IfNoneMatch = "W/\"123\"",
            }.Execute();
        }

        [TestMethod]
        public void ETagDeleteNoIfMatchIfNoneMatch()
        {
            new DeleteHelper(this)
            {
                RequestUri = "People('russellwhyte')",
                MimeType = MimeTypes.ApplicationJsonLight,
                ExpectedStatusCode = 428,
            }.Execute();
        }

        #endregion

        #region ETag function

        [TestMethod]
        public void ETagFunctionCollection()
        {
            var uri = string.Format("People('russellwhyte')/Trips(0)/{0}GetInvolvedPeople()", NameSpacePrefix);

            foreach (var mimeType in mimeTypes)
            {
                if (mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    continue;
                }

                var entries = new List<ODataResource>();
                new QueryFeedHelper(this)
                {
                    RequestUri = uri,
                    MimeType = mimeType,
                    RequestedHandler = argument => Assert.IsNull(argument.Response.GetHeader("ETag")),
                    ReadEntryEnd = argument =>
                        {
                            if (argument.ParentName == null)
                            {
                                entries.Add(argument.Entry);
                            }
                        }
                }.Execute();

                Assert.AreEqual(2, entries.Count);
                foreach (var entry in entries)
                {
                    AssertIsValidETag(entry.ETag);
                }
            }
        }

        #endregion

        #region ETag query entity

        [TestMethod]
        public void ETagQueryEntityIfMatch()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                var entry = new QueryEntryHelper(this)
                {
                    RequestUri = "People('russellwhyte')",
                    MimeType = mimeType,
                    ExpectedStatusCode = 200,
                    IfMatch = "W/\"123\"",
                    RequestedHandler = argument => AssertIsValidETag(argument.Response.GetHeader("ETag")),
                }.Execute();

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.IsNotNull(entry);
                }
            }
        }

        [TestMethod]
        public void ETagQueryEntityIfNoneMatchOk()
        {
            const string uri = "People('russellwhyte')";
            var expectedETag = new QueryEntryHelper(this) { RequestUri = uri, MimeType = MimeTypes.ApplicationJsonLight }.Execute().ETag;

            foreach (var mimeType in this.mimeTypes)
            {
                var entry = new QueryEntryHelper(this)
                {
                    RequestUri = uri,
                    MimeType = mimeType,
                    ExpectedStatusCode = 304,
                    IfNoneMatch = expectedETag,
                }.Execute();

                Assert.IsNull(entry);
            }
        }

        [TestMethod]
        public void ETagQueryEntityIfNoneMatchAsterisk()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                var entry = new QueryEntryHelper(this)
                {
                    RequestUri = "People('russellwhyte')",
                    MimeType = mimeType,
                    ExpectedStatusCode = 304,
                    IfNoneMatch = "*",
                }.Execute();

                Assert.IsNull(entry);
            }
        }

        [TestMethod]
        public void ETagQueryEntityIfNoneMatchWrong()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                var entry = new QueryEntryHelper(this)
                {
                    RequestUri = "People('russellwhyte')",
                    MimeType = mimeType,
                    ExpectedStatusCode = 200,
                    IfNoneMatch = "W/\"123\"",
                    RequestedHandler = argument => AssertIsValidETag(argument.Response.GetHeader("ETag")),
                }.Execute();

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.IsNotNull(entry);
                }
            }
        }

        #endregion

        #region ETag query collection

        [TestMethod]
        public void ETagQueryCollection()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                var entries = new List<ODataResource>();

                new QueryFeedHelper(this)
                {
                    RequestUri = "People",
                    MimeType = mimeType,
                    ExpectedStatusCode = 200,
                    ReadEntryEnd = argument =>
                    {
                        if (argument.ParentName == null)
                        {
                            entries.Add(argument.Entry);
                        }
                    }
                }.Execute();

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(20, entries.Count);
                    foreach (var entry in entries)
                    {
                        AssertIsValidETag(entry.ETag);
                    }
                }
            }
        }

        #endregion

        #region ETag update

        [TestMethod]
        public void ETagUpdateIfMatchOk()
        {
            const string uri = "People('russellwhyte')";
            var expectedETag = new QueryEntryHelper(this) { RequestUri = uri, MimeType = MimeTypes.ApplicationJsonLight }.Execute().ETag;

            var helper = new UpdateHelper(this)
            {
                RequestUri = uri,
                MimeType = MimeTypes.ApplicationJsonLight,
                EntitySetName = "People",
                EntryToUpdate = CreateEntry("russellwhyte"),
                ExpectedStatusCode = 204,
                IfMatch = expectedETag,
            };
            helper.Execute();
            AssertIsValidETag(helper.ResponseETag);
            // TODO: GitHub Issue#424
            // Assert.AreNotEqual(expectedETag, helper.ResponseETag);
        }

        [TestMethod]
        public void ETagUpdateIfMatchAsterisk()
        {
            var helper = new UpdateHelper(this)
            {
                RequestUri = "People('russellwhyte')",
                MimeType = MimeTypes.ApplicationJsonLight,
                EntitySetName = "People",
                EntryToUpdate = CreateEntry("russellwhyte"),
                ExpectedStatusCode = 204,
                IfMatch = "*",
            };
            helper.Execute();
            AssertIsValidETag(helper.ResponseETag);
        }

        [TestMethod]
        public void ETagUpdateIfMatchWrong()
        {
            new UpdateHelper(this)
            {
                RequestUri = "People('russellwhyte')",
                MimeType = MimeTypes.ApplicationJsonLight,
                EntitySetName = "People",
                EntryToUpdate = CreateEntry("russellwhyte"),
                ExpectedStatusCode = 412,
                IfMatch = "W/\"123\"",
            }.Execute();
        }

        [TestMethod]
        public void ETagUpdateIfNoneMatchOk()
        {
            const string uri = "People('russellwhyte')";
            var expectedETag = new QueryEntryHelper(this) { RequestUri = uri, MimeType = MimeTypes.ApplicationJsonLight }.Execute().ETag;

            new UpdateHelper(this)
            {
                RequestUri = uri,
                MimeType = MimeTypes.ApplicationJsonLight,
                EntitySetName = "People",
                EntryToUpdate = CreateEntry("russellwhyte"),
                ExpectedStatusCode = 412,
                IfNoneMatch = expectedETag,
            }.Execute();
        }

        [TestMethod]
        public void ETagUpdateIfNoneMatchAsterisk()
        {
            var helper = new UpdateHelper(this)
            {
                RequestUri = "People('newuser')",
                MimeType = MimeTypes.ApplicationJsonLight,
                EntitySetName = "People",
                EntryToUpdate = CreateEntry("newuser"),
                ExpectedStatusCode = 201,
                IfNoneMatch = "*",
            };
            helper.Execute();
            AssertIsValidETag(helper.ResponseETag);
        }

        [TestMethod]
        public void ETagUpdateIfNoneMatchWrong()
        {
            var helper = new UpdateHelper(this)
            {
                RequestUri = "People('russellwhyte')",
                MimeType = MimeTypes.ApplicationJsonLight,
                EntitySetName = "People",
                EntryToUpdate = CreateEntry("russellwhyte"),
                ExpectedStatusCode = 204,
                IfNoneMatch = "W/\"123\"",
            };
            helper.Execute();
            AssertIsValidETag(helper.ResponseETag);
        }

        [TestMethod]
        public void ETagUpdateNoIfMatchIfNoneMatch()
        {
            new UpdateHelper(this)
            {
                RequestUri = "People('russellwhyte')",
                MimeType = MimeTypes.ApplicationJsonLight,
                EntitySetName = "People",
                EntryToUpdate = CreateEntry("russellwhyte"),
                ExpectedStatusCode = 428,
            }.Execute();
        }

        #endregion

        #region ETag other tests

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        [TestMethod]
        public void AttachToWithEtag()
        {
            var context = TestClientContext;

            var person = context.People.First();
            string personEtag = context.GetEntityDescriptor(person).ETag;
            var attachToPeople = new Person { UserName = person.UserName };
            context.Detach(person);

            context.AttachTo("People", attachToPeople, personEtag);
            context.LoadProperty(attachToPeople, "Photo");
        }

        [TestMethod]
        public void SetEtagValueAfterQueryUpdate()
        {
            var context = CreateDefaultContainer();
            context.MergeOption = Microsoft.OData.Client.MergeOption.PreserveChanges;

            var personToModify = context.People.First();
            var personToDelete = context.People.Skip(1).First();

            //caching Etags
            var personToModifyETag = context.GetEntityDescriptor(personToModify).ETag;
            var personToDeleteETag = context.GetEntityDescriptor(personToDelete).ETag;

            context.UpdateObject(personToModify);
            personToModify.FirstName = "Tom";

            context.DeleteObject(personToDelete);
            // We currently do not allow setting state from Deleted to Modified and LS is fine with the extra step to change State to Unchanged first
            context.ChangeState(personToDelete, Microsoft.OData.Client.EntityStates.Unchanged);
            context.ChangeState(personToDelete, Microsoft.OData.Client.EntityStates.Modified);

            //Updating entities in the store using a new Client context object
            var contextToUpdate = this.CreateDefaultContainer();

            var person1 = contextToUpdate.People.First();
            var person2 = contextToUpdate.People.Skip(1).First();

            person1.FirstName = "David";
            person1.Concurrency = 200240;
            person2.FirstName = "Mark";
            person2.Concurrency = 200241;

            contextToUpdate.UpdateObject(person1);
            contextToUpdate.UpdateObject(person2);


            contextToUpdate.SaveChanges();

            //Quering the attached entities to update them
            context.People.First();
            context.People.Skip(1).First();

            Assert.AreEqual(contextToUpdate.GetEntityDescriptor(person1).ETag, context.GetEntityDescriptor(personToModify).ETag, "ETag not updated by query");
            Assert.AreEqual(contextToUpdate.GetEntityDescriptor(person2).ETag, context.GetEntityDescriptor(personToDelete).ETag, "ETag not updated by query");
            Assert.AreNotEqual(personToModify.FirstName, person1.FirstName, "Query updated entity in Modified State");
            Assert.AreNotEqual(personToDelete.FirstName, person2.FirstName, "Query updated entity in Modified State");

            context.GetEntityDescriptor(personToModify).ETag = personToModifyETag;
            context.GetEntityDescriptor(personToDelete).ETag = personToDeleteETag;

            Assert.AreEqual(personToModifyETag, context.GetEntityDescriptor(personToModify).ETag, "Etag not updated");
            Assert.AreEqual(personToDeleteETag, context.GetEntityDescriptor(personToDelete).ETag, "Etag not updated");

            context.ChangeState(personToDelete, Microsoft.OData.Client.EntityStates.Deleted);
            Assert.AreEqual(Microsoft.OData.Client.EntityStates.Deleted, context.GetEntityDescriptor(personToDelete).State, "ChangeState API did not change the entity State form Modified to Deleted");
        }

        [TestMethod]
        public void UpdateEntryWithIncorrectETag()
        {
            var defaultContext = this.CreateDefaultContext();
            var httpClientContext = this.CreateDefaultContext();

            httpClientContext.Configurations.RequestPipeline.OnMessageCreating =
                (args) =>
                {
                    var message = new Microsoft.OData.Client.HttpWebRequestMessage(args);
                    foreach (var header in args.Headers)
                    {
                        message.SetHeader(header.Key, header.Value);
                    }
                    return message;
                };

            defaultContext.SendingRequest2 += (obj, args) => args.RequestMessage.SetHeader("If-Match", "W/\"var1\"");
            httpClientContext.SendingRequest2 += (obj, args) => args.RequestMessage.SetHeader("If-Match", "W/\"var1\"");

            var exceptions = new Exception[2];
            var statusCodes = new int[2];
            int idx = 0;

            foreach (var ctx in new[] { defaultContext, httpClientContext })
            {
                try
                {
                    var person = ctx.People.First();
                    person.FirstName = "NewName" + Guid.NewGuid().ToString();
                    ctx.UpdateObject(person);
                    ctx.SaveChanges();
                }
                catch (Exception ex)
                {
                    exceptions[idx] = ex;
                }

                Assert.IsNotNull(exceptions[idx], "Expected exception but none was thrown");
                statusCodes[idx] = this.lastResponseStatusCode;
                ++idx;
            }

            Assert.AreEqual(statusCodes[0], statusCodes[1]);
            AssertExceptionsAreEqual(exceptions[0], exceptions[1]);
        }
#endif

        #endregion

        #region $count Tests

        [TestMethod]
        public void CountSimpleTest()
        {
            new QueryCountHelper(this)
            {
                RequestUri = "People/$count",
                ExpectedCount = 20,
            }.Execute();
        }

        // TODO: enable the test case
        //[TestMethod]
        //public void CountFilterTest()
        //{
        //    foreach (var mimeType in mimeTypes)
        //    {
        //        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        //        {
        //            var entries = new QueryFeedHelper(this)
        //            {
        //                RequestUri = "People?$filter=Trips/$count gt 1",
        //                MimeType = mimeType,
        //            }.Execute();

        //            Assert.AreEqual(3, entries.Count);
        //        }
        //    }
        //}

        [TestMethod]
        public void CountTrueTest()
        {
            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    int count = 0;
                    var entries = new QueryFeedHelper(this)
                    {
                        RequestUri = "People?$count=true",
                        MimeType = mimeType,
                        ReadFeedEnd = argument =>
                            {
                                if (argument.ParentName == null)
                                {
                                    Assert.IsNotNull(argument.Feed.Count);
                                }
                            },
                        ReadEntryEnd = argument => count += argument.ParentName == null ? 1 : 0
                    }.Execute();

                    Assert.AreEqual(20, count);
                }
            }
        }

        [TestMethod]
        public void CountFalseTest()
        {
            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var entries = new QueryFeedHelper(this)
                    {
                        RequestUri = "People?$count=false",
                        MimeType = mimeType,
                        ReadFeedEnd = argument => Assert.IsNull(argument.Feed.Count),
                    }.Execute();

                    Assert.AreEqual(20, entries.Where(e => e != null && e.Id != null).Count());
                }
            }
        }

        [TestMethod]
        public void CountErrorTest()
        {
            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    new QueryFeedHelper(this)
                    {
                        RequestUri = "People?$count=aaa",
                        MimeType = mimeType,
                        ExpectedStatusCode = 400, // Bad Request
                    }.Execute();
                }
            }
        }

        [TestMethod]
        public void CountExpandTrueTest()
        {
            const string TypeNamePerson = NameSpacePrefix + "Person";
            const string TypeNameTrip = NameSpacePrefix + "Trip";

            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var entries = new QueryFeedHelper(this)
                    {
                        RequestUri = "People?$expand=Trips($count=true)",
                        MimeType = mimeType,
                        ReadFeedEnd = argument =>
                        {
                            if (argument.ParentName == "Trips")
                            {
                                // Trip feed
                                Assert.IsTrue(argument.Feed.Count.HasValue);
                            }
                            else
                            {
                                // People feed or collection of complex property
                                Assert.IsFalse(argument.Feed.Count.HasValue);
                            }
                        },
                    }.Execute();

                    Assert.AreEqual(20, entries.Count(e => e.TypeName == TypeNamePerson));
                    Assert.AreEqual(14, entries.Count(e => e.TypeName == TypeNameTrip));
                }
            }
        }

        [TestMethod]
        public void CountExpandFalseTest()
        {
            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var typeName = default(string);
                    var entries = new QueryFeedHelper(this)
                    {
                        RequestUri = "People?$expand=Trips($count=false)",
                        MimeType = mimeType,
                        ReadEntryEnd = argument =>
                        {
                            typeName = argument.Entry.TypeName;
                        },
                        ReadFeedEnd = argument =>
                        {
                            if (typeName == null || typeName == NameSpacePrefix + "Trip")
                            {
                                Assert.IsNull(argument.Feed.Count);
                            }
                            else if (typeName == NameSpacePrefix + "Person")
                            {
                                Assert.IsNull(argument.Feed.Count);
                            }
                        },
                    }.Execute();

                    Assert.AreEqual(20, entries.Count(e => e.TypeName == NameSpacePrefix + "Person"));
                    Assert.AreEqual(14, entries.Count(e => e.TypeName == NameSpacePrefix + "Trip"));
                }
            }
        }

        #endregion

        #region Media entity

        [TestMethod]
        public void MediaEntity_Query()
        {
            var verification = new Action<ODataResource, bool, int[]>((entry, isFullMetadata, photoIds) =>
            {
                Assert.IsNotNull(entry);
                Assert.IsNotNull(entry.MediaResource);
                Assert.IsNotNull(entry.MediaResource.ETag);
                Assert.AreEqual("image/jpeg", entry.MediaResource.ContentType);
                if (isFullMetadata)
                {
                    var expectedUrls = photoIds.Select(photoId => string.Format("Photos({0})/$value", photoId));
                    Assert.IsTrue(expectedUrls.Any(url => entry.MediaResource.ReadLink.AbsolutePath.EndsWith(url)));
                    Assert.IsTrue(expectedUrls.Any(url => entry.MediaResource.EditLink.AbsolutePath.EndsWith(url)));
                }
            });

            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var isFullMetadata = mimeType.Contains(MimeTypes.ODataParameterFullMetadata);
                    var entry = default(ODataResource);
                    var entries = default(IList<ODataResource>);

                    entry = new QueryEntryHelper(this) { RequestUri = "People('russellwhyte')/Photo", MimeType = mimeType }.Execute();
                    verification(entry, isFullMetadata, new[] { 2 });

                    entry = new QueryEntryHelper(this) { RequestUri = "Me/Photo", MimeType = mimeType }.Execute();
                    verification(entry, isFullMetadata, new[] { 1 });

                    entries = new QueryFeedHelper(this) { RequestUri = "People('russellwhyte')/Trips(0)/Photos", MimeType = mimeType }.Execute();
                    Assert.AreEqual(2, entries.Count);
                    verification(entries[0], isFullMetadata, new[] { 21, 22 });
                    verification(entries[1], isFullMetadata, new[] { 21, 22 });

                    entries = new QueryFeedHelper(this) { RequestUri = "Me/Trips(1001)/Photos", MimeType = mimeType }.Execute();
                    Assert.AreEqual(2, entries.Count);
                    verification(entries[0], isFullMetadata, new[] { 21, 22 });
                    verification(entries[1], isFullMetadata, new[] { 21, 22 });
                }
            }
        }

        [TestMethod]
        public void MediaEntityStream_Query()
        {
            var entry = new QueryEntryHelper(this)
            {
                RequestUri = "People('russellwhyte')/Photo",
                MimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata
            }.Execute();
            var uri = GetPartialUri(entry.MediaResource.ReadLink.AbsoluteUri);

            // IfMatch: OK
            var streamHelper = new QueryStreamHelper(this)
            {
                RequestUri = uri,
                ExpectedContentType = "image/jpeg",
                ExpectedStatusCode = 200,
                IfMatch = "W/\"-1\"",
                RequestedHandler = argument => AssertIsValidETag(argument.Response.GetHeader("ETag"))
            };
            var stream = streamHelper.Execute();
            Assert.IsTrue(stream.Length > 0);

            // IfMatch: Not Modified
            new QueryStreamHelper(this)
            {
                RequestUri = uri,
                ExpectedStatusCode = 304,
                IfMatch = streamHelper.ResponseETag
            }.Execute();

            // IfMatch: Missing
            stream = new QueryStreamHelper(this)
            {
                RequestUri = uri,
                ExpectedETag = streamHelper.ResponseETag,
                ExpectedContentType = "image/jpeg",
                ExpectedStatusCode = 200,
            }.Execute();
            Assert.IsTrue(stream.Length > 0);
        }

        // the case include both media entity and stream creation
        [TestMethod]
        public void MediaEntity_Create()
        {
            var bytes = new byte[] { 1, 2, 3, 4, 5 };
            var createHelper = default(CreateStreamHelper);
            var entry = default(ODataResource);

            // return status code 201
            createHelper = new CreateStreamHelper(this)
            {
                RequestUri = "Photos",
                BytesToCreate = bytes,
                ContentType = "image/jpeg"
            };
            entry = createHelper.Execute();
            Assert.IsNotNull(entry);

            entry = new QueryEntryHelper(this)
            {
                RequestUri = this.GetPartialUri(createHelper.ResponseLocation),
                MimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                ExpectedStatusCode = 200,
            }.Execute();
            Assert.IsNotNull(entry);

            using (var stream = new QueryStreamHelper(this)
            {
                RequestUri = this.GetPartialUri(createHelper.ResponseLocation + "/$value"),
                ExpectedETag = createHelper.ResponseETag,
                ExpectedContentType = "image/jpeg",
                ExpectedStatusCode = 200,
            }.Execute())
            {
                var buffer = new byte[bytes.Length];
                stream.Read(buffer, 0, buffer.Length);
                CollectionAssert.AreEqual(bytes, buffer);
            }

            // return status code 204
            createHelper = new CreateStreamHelper(this)
            {
                RequestUri = "Photos",
                BytesToCreate = bytes,
                ContentType = "image/jpeg",
                Preference = "return=minimal",
                ExpectedStatusCode = 204
            };
            createHelper.Execute();

            entry = new QueryEntryHelper(this)
            {
                RequestUri = this.GetPartialUri(createHelper.ResponseLocation),
                MimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                ExpectedStatusCode = 200,
            }.Execute();
            Assert.IsNotNull(entry);

            using (var stream = new QueryStreamHelper(this)
            {
                RequestUri = this.GetPartialUri(createHelper.ResponseLocation + "/$value"),
                ExpectedETag = createHelper.ResponseETag,
                ExpectedContentType = "image/jpeg",
                ExpectedStatusCode = 200,
            }.Execute())
            {
                var buffer = new byte[bytes.Length];
                stream.Read(buffer, 0, buffer.Length);
                CollectionAssert.AreEqual(bytes, buffer);
            }
        }

        [TestMethod]
        public void MediaEntity_Update()
        {
            new UpdateHelper(this)
            {
                RequestUri = "Photos(1)",
                EntitySetName = "Photos",
                EntryToUpdate = new ODataResourceWrapper()
                {
                    Resource =
                    new ODataResource()
                    {
                        TypeName = NameSpacePrefix + "Photo",
                        Properties = new[]
                        {
                            new ODataProperty() { Name = "Name", Value = "New Photo" }
                        }
                    }
                },
                MimeType = MimeTypes.ApplicationJsonLight
            }.Execute();

            var entry = new QueryEntryHelper(this)
            {
                RequestUri = "Photos(1)",
                MimeType = MimeTypes.ApplicationJsonLight,
            }.Execute();
            Assert.IsNotNull(entry);
            Assert.AreEqual("New Photo", entry.Properties.Single(p => p.Name == "Name").Value);
        }

        [TestMethod]
        public void MediaEntityStream_Update()
        {
            var bytes = new byte[] { 1, 2, 3, 4, 5 };

            // IfMatch: OK
            var queryStreamHelper = new QueryStreamHelper(this) { RequestUri = "Photos(1)/$value" };
            queryStreamHelper.Execute();
            var entry = new UpdateStreamHelper(this)
            {
                RequestUri = "Photos(1)/$value",
                BytesToUpdate = bytes,
                ContentType = "image/jpeg",
                IfMatch = queryStreamHelper.ResponseETag
            }.Execute();
            Assert.IsNotNull(entry);

            using (var stream = new QueryStreamHelper(this)
            {
                RequestUri = "Photos(1)/$value",
                ExpectedContentType = "image/jpeg",
            }.Execute())
            {
                var buffer = new byte[bytes.Length];
                stream.Read(buffer, 0, buffer.Length);
                CollectionAssert.AreEqual(bytes, buffer);
            }

            // IfMatch: Failed
            new UpdateStreamHelper(this)
            {
                RequestUri = "Photos(1)/$value",
                BytesToUpdate = bytes,
                ContentType = "image/jpeg",
                IfMatch = "W/\"123\"",
                ExpectedStatusCode = 412
            }.Execute();

            // IfMatch: Required
            new UpdateStreamHelper(this)
            {
                RequestUri = "Photos(1)/$value",
                BytesToUpdate = bytes,
                ContentType = "image/jpeg",
                ExpectedStatusCode = 428
            }.Execute();
        }

        // the case tests both deletion of entity and stream
        [TestMethod]
        public void MediaEntity_Delete()
        {
            var queryStreamHelper = new QueryStreamHelper(this) { RequestUri = "Photos(1)/$value" };
            queryStreamHelper.Execute();

            new DeleteHelper(this)
            {
                RequestUri = "Photos(1)",
                MimeType = MimeTypes.ApplicationJsonLight,
                ExpectedStatusCode = 204,
            }.Execute();

            new QueryStreamHelper(this)
            {
                RequestUri = "Photos(1)/$value",
                ExpectedStatusCode = 404,
            }.Execute();
        }

        #endregion

        #region Insert annotation

        [TestMethod]
        public void InsertAnnotation_OnNonInsertableEntitySet()
        {
            new CreateHelper(this)
            {
                RequestUri = "Airports",
                MimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                EntryToCreate = new ODataResourceWrapper()
                {
                    Resource = new ODataResource()
                    {
                        TypeName = NameSpacePrefix + "Airport",
                        Properties = new[]
                        {
                            new ODataProperty { Name = "Name", Value = "Test Airport" },
                            new ODataProperty { Name = "IataCode", Value = "TestAirport" },
                            new ODataProperty { Name = "IcaoCode", Value = "TestAirport" },
                        }
                    }
                },
                EntitySetName = "Airports",
                ExpectedStatusCode = 400
            }.Execute();
        }

        #endregion

        #region Delete annotation

        [TestMethod]
        public void DeleteAnnotation_OnNonDeletableEntitySet()
        {
            new DeleteHelper(this)
            {
                RequestUri = "Airports('KSFO')",
                MimeType = MimeTypes.ApplicationJsonLight,
                ExpectedStatusCode = 400,
            }.Execute();
        }

        #endregion

        #region Acceptable media types annotation

        [TestMethod]
        public void AcceptableMediaTypeAnnotation_InvalidMediaType()
        {
            var bytes = new byte[] { 1, 2, 3, 4, 5 };

            // Create fail
            new CreateStreamHelper(this)
            {
                RequestUri = "Photos",
                BytesToCreate = bytes,
                ContentType = "image/png",
                ExpectedStatusCode = 400
            }.Execute();

            // Update fail
            new UpdateStreamHelper(this)
            {
                RequestUri = "Photos(1)/$value",
                BytesToUpdate = bytes,
                ContentType = "image/png",
                IfMatch = "W/\"0\"",
                ExpectedStatusCode = 400
            }.Execute();
        }

        #endregion

        #region Permission annotation

        [TestMethod]
        public void PermissionAnnotation_OnReadOnlyField()
        {
            const string uri = "People('russellwhyte')";

            new UpdateHelper(this)
            {
                RequestUri = uri,
                MimeType = MimeTypes.ApplicationJsonLight,
                EntitySetName = "People",
                EntryToUpdate = new ODataResourceWrapper()
                {
                    Resource =
                    new ODataResource()
                    {
                        TypeName = NameSpacePrefix + "Person",
                        Properties = new[] { new ODataProperty { Name = "UserName", Value = "NewUserName" } }
                    }
                },
                ExpectedStatusCode = 204,
                IfMatch = "*",
            }.Execute();

            var entry = new QueryEntryHelper(this)
            {
                RequestUri = uri,
                MimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                ExpectedStatusCode = 200,
            }.Execute();

            Assert.AreEqual("russellwhyte", entry.Properties.Single(p => p.Name == "UserName").Value);
        }

        #endregion

        #region Immutable annotation

        [TestMethod]
        public void ImmutableAnnotation_OnImmutableField()
        {
            const string uri = "Airports('KSFO')";

            new UpdateHelper(this)
            {
                RequestUri = uri,
                MimeType = MimeTypes.ApplicationJsonLight,
                EntitySetName = "Airports",
                EntryToUpdate = new ODataResourceWrapper()
                {
                    Resource =
                    new ODataResource()
                    {
                        TypeName = NameSpacePrefix + "Airport",
                        Properties = new[] { new ODataProperty { Name = "IataCode", Value = "NewCode" } }
                    }
                },
                ExpectedStatusCode = 204,
            }.Execute();

            var entry = new QueryEntryHelper(this)
            {
                RequestUri = uri,
                MimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                ExpectedStatusCode = 200,
            }.Execute();

            Assert.AreEqual("SFO", entry.Properties.Single(p => p.Name == "IataCode").Value);
        }

        #endregion

        #region Computed annotation

        [TestMethod]
        public void ComputedAnnotation_OnComputedField()
        {
            const string uri = "People('russellwhyte')";

            new UpdateHelper(this)
            {
                RequestUri = uri,
                MimeType = MimeTypes.ApplicationJsonLight,
                EntitySetName = "People",
                EntryToUpdate = new ODataResourceWrapper()
                {
                    Resource =
                    new ODataResource()
                    {
                        TypeName = NameSpacePrefix + "Person",
                        Properties = new[] { new ODataProperty { Name = "Concurrency", Value = long.MinValue } }
                    }
                },
                ExpectedStatusCode = 204,
                IfMatch = "*",
            }.Execute();

            var entry = new QueryEntryHelper(this)
            {
                RequestUri = uri,
                MimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                ExpectedStatusCode = 200,
            }.Execute();

            Assert.AreNotEqual(long.MinValue, entry.Properties.Single(p => p.Name == "Concurrency").Value);
        }

        #endregion

        #region Delete link

        [TestMethod]
        public void DeleteLink_SingleValuedNavigationProperty()
        {
            new DeleteLinkHelper(this)
            {
                RequestUri = "Me/Photo/$ref",
                MimeType = MimeTypes.ApplicationJsonLight
            }.Execute();

            new QueryEntryHelper(this)
            {
                RequestUri = "Me/Photo",
                MimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                ExpectedStatusCode = 204,
            }.Execute();

            var target = new QueryEntryHelper(this)
            {
                RequestUri = "Photos(1)",
                MimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
            }.Execute();
            Assert.IsNotNull(target);
        }

        [TestMethod]
        public void DeleteLink_CollectionValuedNavigationProperty()
        {
            var target = new QueryEntryHelper(this)
            {
                RequestUri = "People('russellwhyte')",
                MimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
            }.Execute();

            new DeleteLinkHelper(this)
            {
                RequestUri = "People('ronaldmundy')/Friends/$ref?$id=" + target.Id.AbsoluteUri,
                MimeType = MimeTypes.ApplicationJsonLight
            }.Execute();

            var resources = new QueryFeedHelper(this)
            {
                RequestUri = "People('ronaldmundy')/Friends",
                MimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
            }.Execute();
            var friends = resources.Where(r => r.Id != null);
            target = friends.SingleOrDefault(friend => object.Equals(friend.Properties.Single(p => p.Name == "UserName").Value, "russellwhyte"));
            Assert.IsNull(target);

            target = new QueryEntryHelper(this)
            {
                RequestUri = "People('russellwhyte')",
                MimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
            }.Execute();
            Assert.IsNotNull(target);
        }

        [TestMethod]
        public void DeleteLink_ContainedNavigationProperty()
        {
            var target = new QueryEntryHelper(this)
            {
                RequestUri = "People('russellwhyte')/Trips(0)",
                MimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
            }.Execute();

            new DeleteLinkHelper(this)
            {
                RequestUri = "People('russellwhyte')/Trips/$ref?$id=" + target.Id.AbsoluteUri,
                MimeType = MimeTypes.ApplicationJsonLight
            }.Execute();

            new QueryEntryHelper(this)
            {
                RequestUri = "People('russellwhyte')/Trips(0)",
                MimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                ExpectedStatusCode = 204
            }.Execute();
        }

        #endregion

        #region Helper

        public abstract class BaseHelper
        {
            protected BaseHelper(TripPinServiceTests host)
            {
                this.Host = host;
            }

            public TripPinServiceTests Host { get; private set; }
            public Action<RequestingArgument> RequestingHandler { get; set; }
            public Action<RequestedArgument> RequestedHandler { get; set; }
            public string RequestUri { get; set; }
            public string MimeType { get; set; }
            public int ExpectedStatusCode { get; set; }

            protected virtual void OnRequestingHandler(RequestingArgument argument)
            {
                if (this.RequestingHandler != null)
                {
                    this.RequestingHandler(argument);
                }
            }

            protected virtual void OnRequestedHandler(RequestedArgument argument)
            {
                if (this.RequestedHandler != null)
                {
                    this.RequestedHandler(argument);
                }
            }
        }

        public abstract class QueryBaseHelper : BaseHelper
        {
            protected QueryBaseHelper(TripPinServiceTests host)
                : base(host)
            {
            }
        }

        public abstract class QuerySingleBaseHelper : BaseHelper
        {
            protected QuerySingleBaseHelper(TripPinServiceTests host)
                : base(host)
            {
            }

            public string IfMatch { get; set; }
            public string IfNoneMatch { get; set; }
            public string ExpectedETag { get; set; }
            public string ResponseETag { get; set; }
        }

        public abstract class QueryCollectionBaseHelper : QueryBaseHelper
        {
            protected QueryCollectionBaseHelper(TripPinServiceTests host)
                : base(host)
            {
            }

            public int PageSize { get; set; }
        }

        public abstract class ModificationBaseHelper : BaseHelper
        {
            protected ModificationBaseHelper(TripPinServiceTests host)
                : base(host)
            {
            }

            public string IfMatch { get; set; }
            public string IfNoneMatch { get; set; }
            public string ResponseETag { get; set; }
        }

        public abstract class CreateBaseHelper : ModificationBaseHelper
        {
            protected CreateBaseHelper(TripPinServiceTests host)
                : base(host)
            {
            }

            public string ResponseLocation { get; set; }
        }

        public class QueryEntryHelper : QuerySingleBaseHelper
        {
            public QueryEntryHelper(TripPinServiceTests host)
                : base(host)
            {
                this.ExpectedStatusCode = 200;
            }

            public Action<ReadEntryArgument> ReadEntryEnd { get; set; }

            protected virtual void OnReadEntryEnd(ReadEntryArgument argument)
            {
                if (this.ReadEntryEnd != null)
                {
                    this.ReadEntryEnd(argument);
                }
            }

            public virtual ODataResource Execute()
            {
                ODataResource result = null;

                var request = new HttpWebRequestMessage(new Uri(this.Host.ServiceBaseUri.AbsoluteUri + this.RequestUri, UriKind.Absolute));
                request.SetHeader("Accept", this.MimeType);
                if (this.IfMatch != null)
                {
                    request.SetHeader("If-Match", this.IfMatch);
                }
                else if (this.IfNoneMatch != null)
                {
                    request.SetHeader("If-None-Match", this.IfNoneMatch);
                }

                OnRequestingHandler(new RequestingArgument() { Request = request });

                var response = request.GetResponse();
                Assert.AreEqual(this.ExpectedStatusCode, response.StatusCode);

                if (response.StatusCode == 200 && !this.MimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var settings = new ODataMessageReaderSettings() { BaseUri = this.Host.ServiceBaseUri };
                    using (var messageReader = new ODataMessageReader(response, settings, this.Host.Model))
                    {
                        var reader = messageReader.CreateODataResourceReader();
                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                result = reader.Item as ODataResource;
                                OnReadEntryEnd(new ReadEntryArgument() { Entry = result });
                            }
                        }

                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }

                    this.ResponseETag = response.GetHeader("ETag");
                    if (this.ExpectedETag != null)
                    {
                        AssertIsValidETag(this.ResponseETag);
                        Assert.AreEqual(this.ExpectedETag, this.ResponseETag);
                    }
                }

                OnRequestedHandler(new RequestedArgument() { Response = response });

                return result;
            }
        }

        public class QueryFeedHelper : QueryCollectionBaseHelper
        {
            public QueryFeedHelper(TripPinServiceTests host)
                : base(host)
            {
                this.ExpectedStatusCode = 200;
            }

            public Action<ReadEntryArgument> ReadEntryStart { get; set; }
            public Action<ReadFeedArgument> ReadFeedStart { get; set; }
            public Action<ReadEntryArgument> ReadEntryEnd { get; set; }
            public Action<ReadFeedArgument> ReadFeedEnd { get; set; }

            public virtual IList<ODataResource> Execute()
            {
                var result = new List<ODataResource>();

                // For supporting server-driven paging,
                // the initial URI is the original request URI,
                // but the remaining URI should be next link.

                // record the total times of the request
                var times = 0;
                // original URI
                var webRequestUri = new Uri(this.Host.ServiceBaseUri.AbsoluteUri + this.RequestUri, UriKind.Absolute);
                do
                {
                    ++times;

                    var request = new HttpWebRequestMessage(webRequestUri);
                    request.SetHeader("Accept", this.MimeType);
                    if (this.PageSize > 0 && times == 1)
                    {
                        request.SetHeader("Prefer", string.Format("odata.maxpagesize={0}", this.PageSize));
                    }

                    OnRequestingHandler(new RequestingArgument() { Request = request, Times = times });

                    var response = request.GetResponse();
                    Assert.AreEqual(this.ExpectedStatusCode, response.StatusCode);

                    // reset to avoid infinite loop
                    webRequestUri = null;

                    if (response.StatusCode == 200 && !this.MimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        var settings = new ODataMessageReaderSettings() { BaseUri = this.Host.ServiceBaseUri };
                        using (var messageReader = new ODataMessageReader(response, settings, this.Host.Model))
                        {
                            var reader = messageReader.CreateODataResourceSetReader();

                            Stack<string> parentNestedResourceInfo = new Stack<string>();
                            while (reader.Read())
                            {
                                switch (reader.State)
                                {
                                    case ODataReaderState.ResourceSetStart:
                                        {
                                            OnReadFeedStart(new ReadFeedArgument() { Feed = (ODataResourceSet)reader.Item });
                                            break;
                                        }
                                    case ODataReaderState.ResourceSetEnd:
                                        {
                                            var feed = (ODataResourceSet)reader.Item;
                                            Assert.IsNotNull(feed);
                                            OnReadFeedEnd(new ReadFeedArgument() { Feed = feed, ParentName = parentNestedResourceInfo.Count() > 0 ? parentNestedResourceInfo.Peek() : null });
                                            // next link
                                            webRequestUri = feed.NextPageLink;

                                            break;
                                        }
                                    case ODataReaderState.ResourceStart:
                                        {
                                            OnReadEntryStart(new ReadEntryArgument() { Entry = (ODataResource)reader.Item });
                                            break;
                                        }
                                    case ODataReaderState.ResourceEnd:
                                        {
                                            var entry = (ODataResource)reader.Item;
                                            OnReadEntryEnd(new ReadEntryArgument() { Entry = entry, ParentName = parentNestedResourceInfo.Count() > 0 ? parentNestedResourceInfo.Peek() : null });
                                            result.Add(entry);

                                            break;
                                        }
                                    case ODataReaderState.NestedResourceInfoStart:
                                        {
                                            parentNestedResourceInfo.Push(((ODataNestedResourceInfo)reader.Item).Name);
                                            break;
                                        }
                                    case ODataReaderState.NestedResourceInfoEnd:
                                        {
                                            parentNestedResourceInfo.Pop();
                                            break;
                                        }
                                }
                            }

                            Assert.AreEqual(ODataReaderState.Completed, reader.State);
                        }
                    }

                    OnRequestedHandler(new RequestedArgument()
                    {
                        Response = response,
                        Times = times,
                        IsLastTime = webRequestUri == null,
                    });
                } while (webRequestUri != null);

                return result;
            }

            protected virtual void OnReadFeedStart(ReadFeedArgument argument)
            {
                if (this.ReadFeedStart != null)
                {
                    this.ReadFeedStart(argument);
                }
            }

            protected virtual void OnReadEntryStart(ReadEntryArgument argument)
            {
                if (this.ReadEntryStart != null)
                {
                    this.ReadEntryStart(argument);
                }
            }

            protected virtual void OnReadFeedEnd(ReadFeedArgument argument)
            {
                if (this.ReadFeedEnd != null)
                {
                    this.ReadFeedEnd(argument);
                }
            }

            protected virtual void OnReadEntryEnd(ReadEntryArgument argument)
            {
                if (this.ReadEntryEnd != null)
                {
                    this.ReadEntryEnd(argument);
                }
            }
        }

        public class QueryReferenceHelper : QueryCollectionBaseHelper
        {
            public QueryReferenceHelper(TripPinServiceTests host)
                : base(host)
            {
                this.ExpectedStatusCode = 200;
            }

            public virtual IList<ODataEntityReferenceLink> Execute()
            {
                var result = new List<ODataEntityReferenceLink>();

                var hasNextLink = false;
                var times = 0;
                var webRequestUri = new Uri(this.Host.ServiceBaseUri.AbsoluteUri + this.RequestUri, UriKind.Absolute);
                do
                {
                    ++times;

                    if (hasNextLink)
                    {
                        Assert.IsTrue(times > 1);
                    }

                    var request = new HttpWebRequestMessage(webRequestUri);
                    request.SetHeader("Accept", this.MimeType);
                    if (this.PageSize > 0 && times == 1)
                    {
                        request.SetHeader("Prefer", string.Format("odata.maxpagesize={0}", this.PageSize));
                    }

                    OnRequestingHandler(new RequestingArgument() { Request = request, Times = times });

                    var response = request.GetResponse();
                    Assert.AreEqual(this.ExpectedStatusCode, response.StatusCode);

                    // reset to avoid infinite loop
                    webRequestUri = null;

                    if (response.StatusCode == 200 && !this.MimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        var settings = new ODataMessageReaderSettings() { BaseUri = this.Host.ServiceBaseUri };
                        using (var messageReader = new ODataMessageReader(response, settings, this.Host.Model))
                        {
                            var links = messageReader.ReadEntityReferenceLinks();
                            result.AddRange(links.Links);

                            webRequestUri = links.NextPageLink;
                            if (webRequestUri != null)
                            {
                                hasNextLink = true;
                            }
                        }
                    }

                    OnRequestedHandler(new RequestedArgument()
                    {
                        Response = response,
                        Times = times,
                        IsLastTime = webRequestUri == null
                    });
                } while (webRequestUri != null);

                return result;
            }
        }

        public class QueryCountHelper : QueryBaseHelper
        {
            public QueryCountHelper(TripPinServiceTests host)
                : base(host)
            {
                this.ExpectedStatusCode = 200;
                this.MimeType = "*/*";
            }

            public int ExpectedCount { get; set; }

            public virtual void Execute()
            {
                var request = new HttpWebRequestMessage(new Uri(this.Host.ServiceBaseUri.AbsoluteUri + this.RequestUri, UriKind.Absolute));
                request.SetHeader("Accept", this.MimeType);

                OnRequestingHandler(new RequestingArgument() { Request = request });

                var response = request.GetResponse();
                Assert.AreEqual(this.ExpectedStatusCode, response.StatusCode);

                if (response.StatusCode == 200)
                {
                    var settings = new ODataMessageReaderSettings() { BaseUri = this.Host.ServiceBaseUri };
                    using (var messageReader = new ODataMessageReader(response, settings, this.Host.Model))
                    {
                        var count = messageReader.ReadValue(EdmCoreModel.Instance.GetInt32(false));

                        Assert.AreEqual(this.ExpectedCount, count);
                    }
                }

                OnRequestedHandler(new RequestedArgument() { Response = response });
            }
        }

        public class CreateHelper : CreateBaseHelper
        {
            public CreateHelper(TripPinServiceTests host)
                : base(host)
            {
                this.ExpectedStatusCode = 201;
            }

            public ODataResourceWrapper EntryToCreate { get; set; }
            public string EntitySetName { get; set; }
            public string ValidationUri { get; set; }

            public virtual ODataResource Execute()
            {
                ODataResource result = null;

                var request = new HttpWebRequestMessage(new Uri(this.Host.ServiceBaseUri + this.RequestUri));
                request.SetHeader("Content-Type", this.MimeType);
                request.SetHeader("Accept", this.MimeType);
                request.Method = "POST";
                using (var messageWriter = new ODataMessageWriter(request, new ODataMessageWriterSettings() { BaseUri = this.Host.ServiceBaseUri }))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter(this.Host.Model.EntityContainer.FindEntitySet(this.EntitySetName), (IEdmEntityType)this.Host.Model.FindDeclaredType(NameSpacePrefix + this.EntitySetName));
                    ODataWriterHelper.WriteResource(odataWriter, this.EntryToCreate);
                }

                OnRequestingHandler(new RequestingArgument() { Request = request });

                var response = request.GetResponse();

                Assert.AreEqual(this.ExpectedStatusCode, response.StatusCode);
                if (response.StatusCode == 201)
                {
                    var settings = new ODataMessageReaderSettings() { BaseUri = this.Host.ServiceBaseUri };
                    using (var messageReader = new ODataMessageReader(response, settings, this.Host.Model))
                    {
                        var reader = messageReader.CreateODataResourceReader();
                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                result = reader.Item as ODataResource;
                            }
                        }

                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }

                    this.ResponseLocation = response.GetHeader("Location");
                    Assert.IsFalse(string.IsNullOrWhiteSpace(this.ResponseLocation));

                    this.ResponseETag = response.GetHeader("ETag");

                    Assert.IsNotNull(new QueryEntryHelper(this.Host) { RequestUri = this.ValidationUri, MimeType = MimeTypes.ApplicationJson, IfMatch = "*" }.Execute());
                }

                OnRequestedHandler(new RequestedArgument() { Response = response });

                return result;
            }
        }

        public class DeleteHelper : ModificationBaseHelper
        {
            public DeleteHelper(TripPinServiceTests host)
                : base(host)
            {
                this.ExpectedStatusCode = 204;
            }

            public virtual void Execute()
            {
                var request = new HttpWebRequestMessage(new Uri(this.Host.ServiceBaseUri + this.RequestUri))
                {
                    Method = "DELETE"
                };
                if (this.IfMatch != null)
                {
                    request.SetHeader("If-Match", this.IfMatch);
                }
                else if (this.IfNoneMatch != null)
                {
                    request.SetHeader("If-None-Match", this.IfNoneMatch);
                }

                OnRequestingHandler(new RequestingArgument() { Request = request });

                var response = request.GetResponse();

                Assert.AreEqual(this.ExpectedStatusCode, response.StatusCode);
                var validationHelper = new QueryEntryHelper(this.Host) { RequestUri = this.RequestUri, MimeType = MimeTypes.ApplicationJsonLight, IfMatch = "*", };
                if (response.StatusCode == 204)
                {
                    // delete succeeded
                    validationHelper.ExpectedStatusCode = 204;
                    Assert.IsNull(validationHelper.Execute());
                }
                else
                {
                    // delete failed
                    validationHelper.ExpectedStatusCode = 200;
                    Assert.IsNotNull(validationHelper.Execute());
                }

                OnRequestedHandler(new RequestedArgument() { Response = response });
            }
        }

        public class DeleteLinkHelper : ModificationBaseHelper
        {
            public DeleteLinkHelper(TripPinServiceTests host)
                : base(host)
            {
                this.ExpectedStatusCode = 204;
            }

            public virtual void Execute()
            {
                var request = new HttpWebRequestMessage(new Uri(this.Host.ServiceBaseUri + this.RequestUri))
                {
                    Method = "DELETE"
                };

                OnRequestingHandler(new RequestingArgument() { Request = request });

                var response = request.GetResponse();

                Assert.AreEqual(this.ExpectedStatusCode, response.StatusCode);

                OnRequestedHandler(new RequestedArgument() { Response = response });
            }
        }

        public class UpdateHelper : ModificationBaseHelper
        {
            public UpdateHelper(TripPinServiceTests host)
                : base(host)
            {
                this.ExpectedStatusCode = 204;
                this.Method = "PUT";
            }

            public ODataResourceWrapper EntryToUpdate { get; set; }
            public string EntitySetName { get; set; }
            public string Method { get; set; }

            public virtual void Execute()
            {
                var request = new HttpWebRequestMessage(new Uri(this.Host.ServiceBaseUri + this.RequestUri));
                request.SetHeader("Content-Type", MimeType);
                request.SetHeader("Accept", MimeType);
                if (this.IfMatch != null)
                {
                    request.SetHeader("If-Match", this.IfMatch);
                }
                else if (this.IfNoneMatch != null)
                {
                    request.SetHeader("If-None-Match", this.IfNoneMatch);
                }
                request.Method = this.Method;

                OnRequestingHandler(new RequestingArgument() { Request = request });

                using (var messageWriter = new ODataMessageWriter(request, new ODataMessageWriterSettings() { BaseUri = this.Host.ServiceBaseUri }))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter(this.Host.Model.EntityContainer.FindEntitySet(this.EntitySetName), (IEdmEntityType)this.Host.Model.FindDeclaredType(NameSpacePrefix + this.EntitySetName));
                    ODataWriterHelper.WriteResource(odataWriter, this.EntryToUpdate);
                }

                var response = request.GetResponse();

                Assert.AreEqual(this.ExpectedStatusCode, response.StatusCode);
                if (response.StatusCode == 204 || response.StatusCode == 201)
                {
                    this.ResponseETag = response.GetHeader("ETag");

                    Assert.IsNotNull(new QueryEntryHelper(this.Host) { RequestUri = this.RequestUri, MimeType = MimeTypes.ApplicationJsonLight, IfMatch = "*" }.Execute());
                }

                OnRequestedHandler(new RequestedArgument() { Response = response });
            }
        }

        public class QueryStreamHelper : QuerySingleBaseHelper
        {
            public QueryStreamHelper(TripPinServiceTests host)
                : base(host)
            {
                this.ExpectedStatusCode = 200;
            }

            public string ExpectedContentType { get; set; }

            public virtual Stream Execute()
            {
                Stream result = null;

                var request = new HttpWebRequestMessage(new Uri(this.Host.ServiceBaseUri.AbsoluteUri + this.RequestUri, UriKind.Absolute));
                request.SetHeader("Accept", this.MimeType);
                if (this.IfMatch != null)
                {
                    request.SetHeader("If-Match", this.IfMatch);
                }

                OnRequestingHandler(new RequestingArgument() { Request = request });

                var response = request.GetResponse();
                Assert.AreEqual(this.ExpectedStatusCode, response.StatusCode);

                if (response.StatusCode == 200)
                {
                    result = new MemoryStream();
                    response.GetStream().CopyTo(result);
                    result.Seek(0, SeekOrigin.Begin);

                    this.ResponseETag = response.GetHeader("ETag");
                    if (this.ExpectedETag != null)
                    {
                        Assert.AreEqual(this.ExpectedETag, this.ResponseETag);
                    }

                    if (this.ExpectedContentType != null)
                    {
                        Assert.AreEqual(this.ExpectedContentType, response.GetHeader("Content-Type"));
                    }
                }

                OnRequestedHandler(new RequestedArgument() { Response = response });

                return result;
            }
        }

        public class CreateStreamHelper : CreateBaseHelper
        {
            public CreateStreamHelper(TripPinServiceTests host)
                : base(host)
            {
                this.ExpectedStatusCode = 201;
            }

            public byte[] BytesToCreate { get; set; }
            public string ContentType { get; set; }
            public string Preference { get; set; }

            public virtual ODataResource Execute()
            {
                ODataResource result = null;

                var request = new HttpWebRequestMessage(new Uri(this.Host.ServiceBaseUri + this.RequestUri));
                request.SetHeader("Content-Type", this.ContentType);
                request.SetHeader("Accept", this.MimeType);
                request.Method = "POST";
                if (this.Preference != null)
                {
                    request.SetHeader("Prefer", this.Preference);
                }
                using (var stream = request.GetStream())
                {
                    stream.Write(this.BytesToCreate, 0, this.BytesToCreate.Length);
                }

                OnRequestingHandler(new RequestingArgument() { Request = request });

                var response = request.GetResponse();

                Assert.AreEqual(this.ExpectedStatusCode, response.StatusCode);
                if (response.StatusCode == 201 || response.StatusCode == 204)
                {
                    this.ResponseLocation = response.GetHeader("Location");
                    Assert.IsFalse(string.IsNullOrWhiteSpace(this.ResponseLocation));
                    this.ResponseETag = response.GetHeader("ETag");

                    if (response.StatusCode == 201)
                    {
                        var settings = new ODataMessageReaderSettings() { BaseUri = this.Host.ServiceBaseUri };
                        using (var messageReader = new ODataMessageReader(response, settings, this.Host.Model))
                        {
                            var reader = messageReader.CreateODataResourceReader();
                            while (reader.Read())
                            {
                                if (reader.State == ODataReaderState.ResourceEnd)
                                {
                                    result = reader.Item as ODataResource;
                                }
                            }

                            Assert.AreEqual(ODataReaderState.Completed, reader.State);
                        }
                    }

                    Assert.IsNotNull(new QueryEntryHelper(this.Host) { RequestUri = this.Host.GetPartialUri(this.ResponseLocation), MimeType = MimeTypes.ApplicationJson, IfMatch = "*" }.Execute());
                }

                OnRequestedHandler(new RequestedArgument() { Response = response });

                return result;
            }
        }

        public class UpdateStreamHelper : ModificationBaseHelper
        {
            public UpdateStreamHelper(TripPinServiceTests host)
                : base(host)
            {
                this.ExpectedStatusCode = 200;
            }

            public byte[] BytesToUpdate { get; set; }
            public string ContentType { get; set; }

            public virtual ODataResource Execute()
            {
                ODataResource result = null;

                var request = new HttpWebRequestMessage(new Uri(this.Host.ServiceBaseUri + this.RequestUri));
                request.SetHeader("Content-Type", ContentType);
                request.SetHeader("Accept", MimeType);
                if (this.IfMatch != null)
                {
                    request.SetHeader("If-Match", this.IfMatch);
                }
                request.Method = "PUT";
                using (var stream = request.GetStream())
                {
                    stream.Write(this.BytesToUpdate, 0, this.BytesToUpdate.Length);
                }

                OnRequestingHandler(new RequestingArgument() { Request = request });

                var response = request.GetResponse();

                Assert.AreEqual(this.ExpectedStatusCode, response.StatusCode);
                if (response.StatusCode == 200)
                {
                    this.ResponseETag = response.GetHeader("ETag");

                    var settings = new ODataMessageReaderSettings() { BaseUri = this.Host.ServiceBaseUri };
                    using (var messageReader = new ODataMessageReader(response, settings, this.Host.Model))
                    {
                        var reader = messageReader.CreateODataResourceReader();
                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                result = reader.Item as ODataResource;
                            }
                        }

                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }

                OnRequestedHandler(new RequestedArgument() { Response = response });

                return result;
            }
        }

        public class RequestingArgument
        {
            public IODataRequestMessage Request { get; set; }
            public int Times { get; set; }
        }

        public class RequestedArgument
        {
            public IODataResponseMessage Response { get; set; }
            public int Times { get; set; }
            public bool IsLastTime { get; set; }
        }

        public class ReadEntryArgument
        {
            public ODataResource Entry { get; set; }

            //Indicate this entry belongs to which navigation or complex property or collection of complex property
            public string ParentName { get; set; }
        }

        public class ReadFeedArgument
        {
            public ODataResourceSet Feed { get; set; }
            public string ParentName { get; set; }
        }

        private ODataResourceWrapper CreateEntry(string userName)
        {
            return new ODataResourceWrapper()
            {
                Resource = new ODataResource
                {
                    TypeName = NameSpacePrefix + "Person",
                    Properties = new[]
                    {
                        new ODataProperty { Name = "UserName", Value = userName },
                        new ODataProperty { Name = "FirstName", Value = "NewFirstName" },
                        new ODataProperty { Name = "LastName", Value = "NewLastName" },
                        new ODataProperty
                        {
                            Name = "Gender",
                            Value = new ODataEnumValue("0", NameSpacePrefix + "PersonGender")
                        },
                        new ODataProperty
                        {
                            Name = "Emails",
                            Value = new ODataCollectionValue()
                            {
                                TypeName = "Collection(Edm.String)",
                                Items = new[] { "userName@microsft.com", "sqlOdata@microsoft.com" }
                            }
                        }
                    }
                },

                NestedResourceInfoWrappers = new List<ODataNestedResourceInfoWrapper>()
                {
                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo
                        {
                            Name = "AddressInfo",
                            IsCollection = true
                        },
                        NestedResourceOrResourceSet = new ODataResourceSetWrapper()
                        {
                            ResourceSet = new ODataResourceSet()
                            {
                                TypeName = string.Format("Collection({0}Location)", NameSpacePrefix)
                            },
                            Resources = new List<ODataResourceWrapper>()
                            {
                                this.GenerateLocationInfoWrapper("999 zixing"),
                                this.GenerateLocationInfoWrapper("200 xujiahui")
                            }
                        }
                    }
                }
            };
        }

        private ODataResource GenerateLocationInfo(string addressName)
        {
            ODataResource addressCity = new ODataResource()
            {
                TypeName = NameSpacePrefix + "City",
                Properties = new[]
                {
                    new ODataProperty
                        {
                            Name = "CountryRegion",
                            Value = "China"
                        },
                    new ODataProperty
                        {
                            Name = "Name",
                            Value = "Shanghai"
                        },
                    new ODataProperty
                        {
                            Name = "Region",
                            Value = "MinHang"
                        },
                }
            };

            var location = new ODataResource()
            {
                TypeName = NameSpacePrefix + "Location",
                Properties = new[]
                {
                    new ODataProperty
                    {
                        Name = "Address",
                        Value = addressName
                    }
                }
            };

            var city_NestedInfo = new ODataNestedResourceInfo() { Name = "City", IsCollection = false };
            city_NestedInfo.SetAnnotation(addressCity);
            location.SetAnnotation(city_NestedInfo);
            return location;
        }

        private ODataResourceWrapper GenerateLocationInfoWrapper(string addressName)
        {
            return new ODataResourceWrapper()
            {
                Resource = new ODataResource()
                {
                    TypeName = NameSpacePrefix + "Location",
                    Properties = new[]
                    {
                        new ODataProperty
                        {
                            Name = "Address",
                            Value = addressName
                        }
                    }
                },
                NestedResourceInfoWrappers = new List<ODataNestedResourceInfoWrapper>()
                {
                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo() { Name = "City", IsCollection = false },
                        NestedResourceOrResourceSet = new ODataResourceWrapper()
                        {
                            Resource = new ODataResource()
                            {
                                TypeName = NameSpacePrefix + "City",
                                Properties = new[]
                                {
                                    new ODataProperty
                                        {
                                            Name = "CountryRegion",
                                            Value = "China"
                                        },
                                    new ODataProperty
                                        {
                                            Name = "Name",
                                            Value = "Shanghai"
                                        },
                                    new ODataProperty
                                        {
                                            Name = "Region",
                                            Value = "MinHang"
                                        },
                                }
                            }
                        }
                    }
                }
            };
        }

        private string GetPartialUri(string absoluteUri)
        {
            return absoluteUri.Substring(this.ServiceBaseUri.AbsoluteUri.Length);
        }

        private static void AssertIsValidETag(string etag)
        {
            if (!(!string.IsNullOrEmpty(etag) && etag.Length > 4 && etag.StartsWith("W/\"") && etag.EndsWith("\"")))
            {
                Assert.Fail("The ETag value {0} is invalid", etag);
            }
        }

        #endregion

        private static void AssertExceptionsAreEqual(Exception expected, Exception actual)
        {
            Assert.AreEqual(expected.GetType(), actual.GetType());

            if (expected.InnerException == null)
            {
                Assert.IsNull(actual.InnerException);
            }
            else
            {
                Assert.IsNotNull(actual.InnerException);
                AssertExceptionsAreEqual(expected.InnerException, actual.InnerException);
            }
        }

        private DefaultContainer CreateDefaultContext()
        {
            var context = CreateDefaultContainer();
            context.ReceivingResponse += (sender, args) => { this.lastResponseStatusCode = args.ResponseMessage.StatusCode; };

            return context;
        }

        internal DefaultContainer CreateDefaultContainer()
        {
            var context = Activator.CreateInstance(typeof(DefaultContainer), ServiceBaseUri) as DefaultContainer;
            Assert.IsNotNull(context, "Failed to cast DataServiceContext to specified type '{0}'", typeof(DefaultContainer).Name);

            return context;
        }
    }
}
