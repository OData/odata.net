//---------------------------------------------------------------------
// <copyright file="WriteFeedWithoutNavigationTargetTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.Test.OData.Utils.Metadata;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.Writer.JsonLight
{
    public class WriteFeedWithoutNavigationTargetTests
    {
        private EdmModel myModel;
        private MemoryStream stream;

        #region Entities

        private static readonly ODataFeed districtFeed = new ODataFeed();

        private static readonly ODataEntry districtEntry = new ODataEntry
        {
            Properties = new List<ODataProperty>
            {
                new ODataProperty { Name = "DistrictId", Value = 1 }
            },
        };

        #endregion

        [Fact]
        public void WriteCollectionNavigationPropertyMissingNavigationTargetWithModel()
        {
            this.TestInit();

            using (var messageWriter = this.CreateMessageWriter(true))
            {
                var navigationSource = this.GetCitySet().FindNavigationTarget(
                    this.GetCityDistrictsProperty()) as IEdmEntitySetBase;
                var writer = messageWriter.CreateODataFeedWriter(navigationSource);
                writer.WriteStart(districtFeed);
                writer.WriteStart(districtEntry);
                writer.WriteEnd(); // districtEntry
                writer.WriteEnd(); // districtFeed
                writer.Flush();
            }

            string payload = this.TestFinish();
            payload.Should().Contain("\"@odata.context\":\"http://host/service/$metadata#Collection(MyNS.District)\"");
        }

        [Fact]
        public void WriteNullableSingletonNavigationPropertyMissingNavigationTargetWithModel()
        {
            this.TestInit();

            using (var messageWriter = this.CreateMessageWriter(true))
            {
                var navigationSource = this.GetCitySet().FindNavigationTarget(
                    this.GetCityCentralDistrictProperty()) as IEdmEntitySetBase;
                var writer = messageWriter.CreateODataEntryWriter(navigationSource);
                writer.WriteStart(districtEntry);
                writer.WriteEnd(); // districtEntry
                writer.Flush();
            }

            string payload = this.TestFinish();
            payload.Should().Contain("\"@odata.context\":\"http://host/service/$metadata#MyNS.District\"");
        }

        [Fact]
        public void WriteCollectionNavigationPropertyMissingNavigationTargetWithoutModel()
        {
            this.TestInit();

            districtFeed.SerializationInfo = new ODataFeedAndEntrySerializationInfo()
            {
                IsFromCollection = true,
                NavigationSourceEntityTypeName = "MyNS.District",
                NavigationSourceKind = EdmNavigationSourceKind.UnknownEntitySet,
                NavigationSourceName = "Districts"
            };

            using (var messageWriter = this.CreateMessageWriter(false))
            {
                var writer = messageWriter.CreateODataFeedWriter();
                writer.WriteStart(districtFeed);
                writer.WriteStart(districtEntry);
                writer.WriteEnd(); // districtEntry
                writer.WriteEnd(); // districtFeed
                writer.Flush();
            }

            string payload = this.TestFinish();
            payload.Should().Contain("\"@odata.context\":\"http://host/service/$metadata#Collection(MyNS.District)\"");
        }

        [Fact]
        public void WriteNullableSingletonNavigationPropertyMissingNavigationTargetWithoutModel()
        {
            this.TestInit();

            districtEntry.SerializationInfo = new ODataFeedAndEntrySerializationInfo()
            {
                IsFromCollection = false,
                NavigationSourceEntityTypeName = "MyNS.District",
                NavigationSourceKind = EdmNavigationSourceKind.UnknownEntitySet,
                NavigationSourceName = "CentralDistrict"
            };

            using (var messageWriter = this.CreateMessageWriter(false))
            {
                var writer = messageWriter.CreateODataEntryWriter();
                writer.WriteStart(districtEntry);
                writer.WriteEnd(); // districtEntry
                writer.Flush();
            }

            string payload = this.TestFinish();
            payload.Should().Contain("\"@odata.context\":\"http://host/service/$metadata#MyNS.District\"");
        }

        #region Private Methods

        private void TestInit()
        {
            this.stream = new MemoryStream();
        }

        private IEdmModel GetModel()
        {
            if (myModel == null)
            {
                myModel = new EdmModel();

                var districtType = new EdmEntityType("MyNS", "District");
                districtType.AddKeys(districtType.AddStructuralProperty("DistrictId", EdmPrimitiveTypeKind.Int32));
                myModel.AddElement(districtType);

                var cityType = new EdmEntityType("MyNS", "City");
                cityType.AddKeys(cityType.AddStructuralProperty("CityId", EdmPrimitiveTypeKind.Int32));
                cityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
                {
                    Name = "Districts",
                    Target = districtType,
                    TargetMultiplicity = EdmMultiplicity.Many,
                    ContainsTarget = false
                });
                cityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
                {
                    Name = "CentralDistrict",
                    Target = districtType,
                    TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
                    ContainsTarget = false
                });
                myModel.AddElement(cityType);

                var container = new EdmEntityContainer("MyNS", "Default");
                container.AddEntitySet("Cities", cityType);
                myModel.AddElement(container);
            }

            return myModel;
        }

        private IEdmEntitySet GetCitySet()
        {
            var model = this.GetModel();
            return model.FindDeclaredEntitySet("Cities");
        }

        private IEdmEntityType GetCityType()
        {
            var model = this.GetModel();
            return (IEdmEntityType)model.FindDeclaredType("MyNS.City");
        }

        private IEdmNavigationProperty GetCityDistrictsProperty()
        {
            var cityType = this.GetCityType();
            return cityType.GetNavigationProperty("Districts");
        }

        private IEdmNavigationProperty GetCityCentralDistrictProperty()
        {
            var cityType = this.GetCityType();
            return cityType.GetNavigationProperty("CentralDistrict");
        }

        private IEdmEntityType GetDistrictType()
        {
            var model = this.GetModel();
            return (IEdmEntityType)model.FindDeclaredType("MyNS.District");
        }

        private string TestFinish()
        {
            this.stream.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(this.stream))
            {
                return reader.ReadToEnd();
            }
        }

        private ODataMessageWriter CreateMessageWriter(bool useModel)
        {
            var responseMessage = new TestResponseMessage(this.stream);
            var writerSettings = new ODataMessageWriterSettings { DisableMessageStreamDisposal = true };
            writerSettings.SetServiceDocumentUri(new Uri("http://host/service"));
            var model = useModel ? this.GetModel() : null;
            return new ODataMessageWriter(responseMessage, writerSettings, model);
        }

        private class TestResponseMessage : IODataResponseMessage
        {
            private readonly Dictionary<string, string> headers =
                new Dictionary<string, string>(); 
            private readonly Stream stream;

            public TestResponseMessage(Stream stream)
            {
                this.stream = stream;
            }

            public IEnumerable<KeyValuePair<string, string>> Headers
            {
                get { return this.headers; }
            }

            public int StatusCode { get; set; }

            public string GetHeader(string headerName)
            {
                string headerValue;
                if (this.headers.TryGetValue(headerName, out headerValue))
                {
                    return headerValue;
                }

                return null;
            }

            public void SetHeader(string headerName, string headerValue)
            {
                this.headers[headerName] = headerValue;
            }

            public Stream GetStream()
            {
                return this.stream;
            }
        }

        #endregion
    }
}
