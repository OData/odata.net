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
    using System.Text.RegularExpressions;
    using Microsoft.OData;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Xunit;

    public class TripPinFilterTests : ODataWCFServiceTestsBase<Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference.InMemoryEntities>, IDisposable
    {
        private const string NameSpacePrefix = "Microsoft.OData.SampleService.Models.TripPin.";

        public TripPinFilterTests()
            : base(ServiceDescriptors.TripPinServiceDescriptor)
        {

        }

        [Fact]
        public void TripPinFilterDefaultStringFunctions()
        {
            #region Contains
            string uri = "People?$filter=contains(UserName,'v')";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "UserName")).Value;
                Assert.Contains("v", userName.ToString());
            },
            "Person");
            #endregion

            #region matchesPattern
            uri = "People?$filter=matchesPattern(UserName,'chum$')";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "UserName")).Value;
                Assert.Contains("chum", userName.ToString());
                Assert.Matches("chum$", userName.ToString());
            },
            "Person");
            #endregion

            #region endswith
            uri = "People?$filter=endswith(UserName,'chum')";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "UserName")).Value;
                Assert.Contains("chum", userName.ToString());
                Assert.EndsWith("chum", userName.ToString());
            },
            "Person");
            #endregion

            #region startswith
            uri = "People?$filter=startswith(UserName,'v')";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "UserName")).Value;
                Assert.Contains("v", userName.ToString());
                Assert.StartsWith("v", userName.ToString());
            },
            "Person");
            #endregion

            #region length
            uri = "People?$filter=length(UserName) eq 12";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "UserName")).Value;
                Assert.Equal(12, userName.ToString().Length);
            },
            "Person");
            #endregion

            #region indexof
            uri = "People?$filter=indexof(UserName,'vincent') eq 1";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "UserName")).Value;
                Assert.Contains("vincent", userName.ToString());
                Assert.Equal(1, userName.ToString().IndexOf("vincent"));
            },
            "Person");
            #endregion

            #region substring
            uri = "People?$filter=substring(UserName,1) eq 'incentcalabrese'";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "UserName")).Value;
                Assert.Contains("incentcalabrese", userName.ToString());
                Assert.Equal("incentcalabrese", userName.ToString().Substring(1));
            },
            "Person");

            uri = "People?$filter=substring(UserName,1,6) eq 'incent'";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "UserName")).Value;
                Assert.Contains("incent", userName.ToString());
                Assert.Equal("incent", userName.ToString().Substring(1, 6));
            },
            "Person");
            #endregion

            #region tolower
            uri = "People?$filter=tolower(UserName) eq 'vincentcalabrese'";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "UserName")).Value;
                Assert.Equal("vincentcalabrese", userName.ToString());
            },
            "Person");
            #endregion

            #region toupper
            uri = "People?$filter=toupper(UserName) eq 'VINCENTCALABRESE'";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "UserName")).Value;
                Assert.Equal("VINCENTCALABRESE", userName.ToString().ToUpper());
            },
            "Person");
            #endregion

            #region trim
            uri = "People?$filter=trim(UserName) eq 'vincentcalabrese'";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "UserName")).Value;
                Assert.Equal("vincentcalabrese", userName.ToString());
            },
            "Person");
            #endregion

            #region concat
            uri = "People?$filter=concat(concat(FirstName,', '), LastName) eq 'Vincent, Calabrese'";
            requestAndCheckResult(uri, (item) =>
            {
                var firstName = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "FirstName")).Value;
                var lastName = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "LastName")).Value;
                Assert.Equal("Vincent, Calabrese", string.Concat(string.Concat(firstName, ", "), lastName));
            },
            "Person");
            #endregion
        }

        [Fact]
        public void TripPinFilterDefaultDateTimeFunctions()
        {
            #region year
            string uri = "People('russellwhyte')/Trips?$filter=year(StartsAt) eq 2014";
            requestAndCheckResult(uri, (item) =>
            {
                var property = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "StartsAt")).Value;
                var startTime = (DateTimeOffset)property;
                Assert.Equal(2014, startTime.Year);
            });
            #endregion

            #region month
            uri = "People('russellwhyte')/Trips?$filter=month(StartsAt) eq 2";
            requestAndCheckResult(uri, (item) =>
            {
                var property = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "StartsAt")).Value;
                var startTime = (DateTimeOffset)property;
                Assert.Equal(2, startTime.Month);
            });
            #endregion

            #region day
            uri = "People('russellwhyte')/Trips?$filter=day(StartsAt) eq 1";
            requestAndCheckResult(uri, (item) =>
            {
                var property = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "StartsAt")).Value;
                var startTime = (DateTimeOffset)property;
                Assert.Equal(1, startTime.Day);
            });
            #endregion

            #region hour
            uri = "People('russellwhyte')/Trips?$filter=hour(StartsAt) eq 0";
            requestAndCheckResult(uri, (item) =>
            {
                var property = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "StartsAt")).Value;
                var startTime = (DateTimeOffset)property;
                Assert.Equal(0, startTime.Hour);
            });
            #endregion

            #region minute
            uri = "People('russellwhyte')/Trips?$filter=minute(StartsAt) eq 0";
            requestAndCheckResult(uri, (item) =>
            {
                var property = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "StartsAt")).Value;
                var startTime = (DateTimeOffset)property;
                Assert.Equal(0, startTime.Minute);
            });
            #endregion

            #region second
            uri = "People('russellwhyte')/Trips?$filter=second(StartsAt) eq 0";
            requestAndCheckResult(uri, (item) =>
            {
                var property = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "StartsAt")).Value;
                var startTime = (DateTimeOffset)property;
                Assert.Equal(0, startTime.Second);
            });
            #endregion
        }

        [Fact]
        public void TripPinFilterDefaultMathFunctions()
        {
            #region round
            string uri = "People?$filter=round(12.66) eq 12";
            requestAndCheckResult(uri, (item) =>
            {
                Assert.Equal(13, Math.Round(12.66));
            });
            #endregion

            #region floor
            uri = "People?$filter=floor(12.66) eq 12";
            requestAndCheckResult(uri, (item) =>
            {
                Assert.Equal(12, Math.Floor(12.66));
            });
            #endregion

            #region ceiling
            uri = "People?$filter=ceiling(12.66) eq 12";
            requestAndCheckResult(uri, (item) =>
            {
                Assert.Equal(13, Math.Ceiling(12.66));
            });
            #endregion
        }

        [Fact]
        public void TripPinFilterDefaultTypeFunctions()
        {
            #region isof
            string uri = "People?$filter=isof(UserName,Edm.String)";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "UserName")).Value;
                Assert.IsType<string>(userName);
            },
            "Person");

            //TODO: should remove the single quotation marks (' '), this should be ODL bug
            uri = string.Format("People?$filter=isof(Gender,'{0}PersonGender')", NameSpacePrefix);
            requestAndCheckResult(uri, (item) =>
            {
                var gender = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "Gender")).Value;

                Assert.Equal("Microsoft.OData.SampleService.Models.TripPin.PersonGender", Assert.IsType<ODataEnumValue>(gender).TypeName);
            },
            "Person");

            //TODO: should remove the single quotation marks (' '), this should be ODL bug
            uri = string.Format("People?$filter=isof('{0}Person')", NameSpacePrefix);
            requestAndCheckFeedResult(uri, (feed) =>
            {
                Assert.True(feed.Count > 0);
            },
            "Person");

            uri = string.Format("People('russellwhyte')/Trips(0)/PlanItems?$filter=isof('{0}Flight')", NameSpacePrefix);
            requestAndCheckFeedResult(uri, (feed) =>
            {
                Assert.True(feed.Count > 0);
            },
            "Flight");
            #endregion

            #region cast
            uri = "People?$filter=cast(UserName, Edm.String) eq 'vincentcalabrese'";
            requestAndCheckResult(uri, (item) =>
            {
                var userName = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "UserName")).Value;
                Assert.Equal("vincentcalabrese", userName.ToString());
            },
            "Person");

            uri = "People('russellwhyte')/Trips?$filter=cast(TripId, Edm.Int32) eq 1001";
            requestAndCheckResult(uri, (item) =>
            {
                var tripId = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "TripId")).Value;
                Assert.Equal(1001, Assert.IsType<int>(tripId));
            },
            "Trip");

            uri = "People('russellwhyte')/Trips?$filter=cast(TripId, Edm.String) eq '1001'";
            requestAndCheckResult(uri, (item) =>
            {
                var tripId = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "TripId")).Value;
                Assert.Equal("1001", tripId.ToString());
            },
            "Trip");

            uri = "People('russellwhyte')/Trips?$filter=cast(TripId, Edm.String) eq '1001'";
            requestAndCheckResult(uri, (item) =>
            {
                var tripId = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "TripId")).Value;
                Assert.Equal("1001", tripId.ToString());
            },
            "Trip");

            //TODO, should add parameter with Doulbe type in trip pin.
            uri = string.Format("People?$filter=cast(12.66, Edm.Int32) eq 13");
            requestAndCheckFeedResult(uri, (feed) =>
            {
                Assert.True(feed.Count > 0);
            },
            "Person");

            //Only support convert enum to string
            uri = string.Format("People?$filter=cast(Gender, Edm.String) eq 'Male'");
            requestAndCheckResult(uri, (item) =>
            {
                var gender = Assert.IsType<ODataProperty>(item.Properties.Single(p => p.Name == "Gender")).Value;

                var genderEnumValue = Assert.IsType<ODataEnumValue>(gender);
                Assert.Equal("Microsoft.OData.SampleService.Models.TripPin.PersonGender", genderEnumValue.TypeName);
                Assert.Equal("Male", genderEnumValue.Value);
            },
            "Person");

            uri = string.Format("People?$filter=cast('Male', {0}PersonGender) eq {0}PersonGender'Male'", NameSpacePrefix);
            requestAndCheckFeedResult(uri, (feed) =>
            {
                Assert.True(feed.Count > 0);
            },
            "Person");
            #endregion
        }

        [Fact]
        public void TripPinFilterDefaultGeoFunctions()
        {
            //No data for test.
            Assert.True(true);
        }

        #region private methods
        private void requestAndCheckResult(string uri, Action<ODataResource> verify, params string[] resourceTypeNames)
        {
            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    List<ODataResource> entries = QueryFeed(uri, mimeType, resourceTypeNames);
                    entries.ForEach(item =>
                    {
                        verify.Invoke(item);
                    });
                }
            }
        }

        private void requestAndCheckFeedResult(string uri, Action<List<ODataResource>> verify, params string[] resourceTypeNames)
        {
            foreach (var mimeType in mimeTypes)
            {
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    List<ODataResource> entries = QueryFeed(uri, mimeType, resourceTypeNames);
                    verify.Invoke(entries);
                }
            }
        }

        private List<ODataResource> QueryFeed(string requestUri, string mimeType, params string[] resourceTypeNames)
        {
            List<ODataResource> entries = new List<ODataResource>();

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + requestUri, UriKind.Absolute));
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                {
                    var reader = messageReader.CreateODataResourceSetReader();
                    ODataResourceSet feed = null;
                    while (reader.Read())
                    {
                        if (reader.State == ODataReaderState.ResourceEnd)
                        {
                            ODataResource entry = reader.Item as ODataResource;
                            if (entry != null && resourceTypeNames.Any(r=>entry.TypeName.EndsWith(r)))
                            {
                                entries.Add(entry);
                            }
                        }
                        else if (reader.State == ODataReaderState.ResourceSetEnd)
                        {
                            feed = reader.Item as ODataResourceSet;
                            
                        }
                    }
                    Assert.NotNull(feed as ODataResourceSet);
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                }
            }
            return entries;
        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
