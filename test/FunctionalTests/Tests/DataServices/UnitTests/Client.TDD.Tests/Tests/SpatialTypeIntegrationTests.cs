//---------------------------------------------------------------------
// <copyright file="SpatialTypeIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests.Client
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using System.IO;
    using System.Linq;
    using Microsoft.Spatial;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;

    [TestClass]
    public class SpatialTypeIntegrationTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            this.testEntity = new SpatialEntityType()
            {
                ID = 0,
                PropBase = GeographyFactory.Point(55.8, -126.543),
                Prop1 = GeographyFactory.Point(42.123, -121.321),
                Prop2 = GeographyFactory.LineString(42.321, -121.123).LineTo(42.111, -121.222)
            };
        }

        public class SpatialEntityType : System.ComponentModel.INotifyPropertyChanged
        {
            Geography gBase;
            GeographyPoint g1;
            GeographyLineString g2;

            public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
            public int ID { get; set; }
            public Geography PropBase { get { return gBase; } set { gBase = value; this.OnPropertyChanged("PropBase"); } }
            public GeographyPoint Prop1 { get { return g1; } set { g1 = value; this.OnPropertyChanged("Prop1"); } }
            public GeographyLineString Prop2 { get { return g2; } set { g2 = value; this.OnPropertyChanged("Prop2"); } }

            private void OnPropertyChanged(String property)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(property));
                }
            }
        }

        private SpatialEntityType testEntity;

        [TestMethod]
        public void ClientSerializeGeographyTest_AddDataPresentAndNamespaceNotDuplicated()
        {
            DataServiceContext ctx = new DataServiceContext(new Uri("http://localhost"));
            ctx.AddObject("Entities", testEntity);
            ClientSerializeGeographyTest_Validate(ctx);
        }

        [TestMethod]
        public void ClientSerializeGeographyTest_Update()
        {
            DataServiceContext ctx = new DataServiceContext(new Uri("http://localhost"));
            ctx.AttachTo("Entities", testEntity);
            ctx.UpdateObject(testEntity);
            ClientSerializeGeographyTest_Validate(ctx);
        }

        [TestMethod]
        public void ClientSerializeGeographyTest_BindingAddChangeShouldBeDetected()
        {
            DataServiceContext ctx = new DataServiceContext(new Uri("http://localhost"));
            DataServiceCollection<SpatialEntityType> dsc = new DataServiceCollection<SpatialEntityType>(ctx, null, TrackingMode.AutoChangeTracking, "Entities", null, null);
            dsc.Add(testEntity);
            ClientSerializeGeographyTest_Validate(ctx);
        }

        [TestMethod]
        public void ClientSerializeGeographyTest_BindingUpdateChangeShouldBeDetected()
        {
            DataServiceContext ctx = new DataServiceContext(new Uri("http://localhost"));
            DataServiceCollection<SpatialEntityType> dsc = new DataServiceCollection<SpatialEntityType>(ctx, null, TrackingMode.AutoChangeTracking, "Entities", null, null);
            dsc.Add(testEntity);
            Assert.AreEqual(1, ctx.Entities.Count);

            ctx.Entities.FirstOrDefault().State = EntityStates.Unchanged;
            testEntity.Prop1 = testEntity.Prop1;
            Assert.AreEqual(EntityStates.Modified, ctx.Entities.FirstOrDefault().State);

            ClientSerializeGeographyTest_ValidateUpdate(ctx);
        }

        private static void ClientSerializeGeographyTest_Validate(DataServiceContext ctx)
        {
            ctx.EnableAtom = true;
            var sr = new TestBaseSaveResult(ctx, "SaveChanges", SaveChangesOptions.None, null, null);
            var s = sr.GetChangeStream().FirstOrDefault();

            s.Seek(0, SeekOrigin.Begin);
            StreamReader r = new StreamReader(s);
            String actual = r.ReadToEnd();

            // namespace declared with entry
            actual.Should().Contain("<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\">");

            // namespace not repeated for properties
            actual.Should().Contain("<d:PropBase m:type=\"GeographyPoint\"><gml:Point gml:srsName=\"http://www.opengis.net/def/crs/EPSG/0/4326\"><gml:pos>55.8 -126.543</gml:pos></gml:Point></d:PropBase>");
            actual.Should().Contain("<d:Prop1 m:type=\"GeographyPoint\"><gml:Point gml:srsName=\"http://www.opengis.net/def/crs/EPSG/0/4326\"><gml:pos>42.123 -121.321</gml:pos></gml:Point></d:Prop1>");
            actual.Should().Contain("<d:Prop2 m:type=\"GeographyLineString\"><gml:LineString gml:srsName=\"http://www.opengis.net/def/crs/EPSG/0/4326\"><gml:pos>42.321 -121.123</gml:pos><gml:pos>42.111 -121.222</gml:pos></gml:LineString></d:Prop2>");
        }

        private static void ClientSerializeGeographyTest_ValidateUpdate(DataServiceContext ctx)
        {
            ctx.EnableAtom = true;
            var sr = new TestBaseSaveResult(ctx, "SaveChanges", SaveChangesOptions.None, null, null);
            var s = sr.GetChangeStream().FirstOrDefault();

            s.Seek(0, SeekOrigin.Begin);
            StreamReader r = new StreamReader(s);
            String actual = r.ReadToEnd();

            // namespace declared with entry
            actual.Should().Contain("<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\">");

            // namespace not repeated for properties
            actual.Should().Contain("<d:Prop1 m:type=\"GeographyPoint\"><gml:Point gml:srsName=\"http://www.opengis.net/def/crs/EPSG/0/4326\"><gml:pos>42.123 -121.321</gml:pos></gml:Point></d:Prop1>");
        }

        internal class TestBaseSaveResult : SaveResult
        {
            public TestBaseSaveResult(DataServiceContext context, string method, SaveChangesOptions options, AsyncCallback callback, object state)
                : base(context, method, options, callback, state)
            {
            }

            internal List<Stream> GetChangeStream()
            {
                List<Stream> stream = new List<Stream>();
                HeaderCollection headers = new HeaderCollection();
                headers.SetHeader("Content-Type", "application/atom+xml;odata.metadata=minimal");
                for (int i = 0; i < this.ChangedEntries.Count; ++i)
                {
                    ODataRequestMessageWrapper requestMessage = ODataRequestMessageWrapper.CreateRequestMessageWrapper(
                        new BuildingRequestEventArgs("GET", new Uri("http://service.svc/randomuri"), headers, null, HttpStack.Auto),
                        this.RequestInfo);
                    this.CreateChangeData(i, requestMessage);
                    stream.Add(requestMessage.CachedRequestStream.Stream);
                }

                return stream;
            }
        }

    }
}
