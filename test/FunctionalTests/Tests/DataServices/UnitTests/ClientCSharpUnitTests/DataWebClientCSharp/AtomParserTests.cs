//---------------------------------------------------------------------
// <copyright file="AtomParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using System.Data.Test.Astoria;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Spatial;
    using System.Xml;
    using AstoriaUnitTests.Stubs;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion Namespaces

    [TestClass]
    public class AtomParserTests
    {
        private Uri serviceRoot;
        private DataServiceContext context;

        public const string CommonNamespaces = "xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom'";

        public const string FeedStart = "<feed xml:base='http://localhost/' " + CommonNamespaces + ">";

        internal const string EmptyEntry = EntryStart + "</entry>";

        public const string EmptyFeed = FeedStart + "</feed>";

        internal const string EntryStart = "<entry xml:base='http://localhost/' " + CommonNamespaces + ">";

        public static readonly Dictionary<string, string> EmptyHeaders = new Dictionary<string, string>();

        [TestInitialize]
        public void Initialize()
        {
            ReadOnlyTestContext.ClearBaselineIncludes();
            this.serviceRoot = new Uri("http://localhost/");
            this.context = new DataServiceContext(serviceRoot);
            this.context.EnableAtom = true;
        }

        public static QueryOperationResponse<T> CreateQueryResponse<T>(DataServiceContext context, Dictionary<string, string> headers, IQueryable<T> query, string xml)
        {
            return CreateQueryResponse<T>(context, headers, query as DataServiceQuery, xml) as QueryOperationResponse<T>;
        }

        internal static IEnumerable<T> CreateQueryResponse<T>(DataServiceContext context, Dictionary<string, string> headers, DataServiceQuery query, string xml)
        {
            var reader = ProjectionTests.ToXml(xml);
            object materializeAtom = ProjectionTests.CreateTestMaterializeAtomEnumerable(context, xml, query);
            Type queryType = typeof(DataServiceContext).Assembly.GetTypes().First(t => t.IsGenericType && t.Name.Contains("QueryOperationResponse"));
            queryType = queryType.MakeGenericType(typeof(T));
            ConstructorInfo ctor = queryType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Single();
            object queryOperationResponse = ctor.Invoke(new object[] { CreateHeaderCollection(headers), query, materializeAtom });
            return (IEnumerable<T>)queryOperationResponse;
        }

        private static object CreateHeaderCollection(IEnumerable<KeyValuePair<string, string>> headers)
        {
            var type = typeof(DataServiceContext).Assembly.GetTypes().First(t => t.Name == "HeaderCollection");
            var constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(IEnumerable<KeyValuePair<string, string>>) }, null);
            return constructor.Invoke(new object[] { headers });
        }

        private IEnumerable<T> QueryResponse<T>(string xml, IQueryable<T> query)
        {
            return CreateQueryResponse<T>(this.context, EmptyHeaders, query as DataServiceQuery, xml);
        }

        [TestMethod]
        public void AtomParserReadInlineTest()
        {
            string xml = FeedStart + "<m:count>2</m:count>" +
                AnyEntry("t1", "<d:TeamID>1</d:TeamID>", null) + "</feed>";
            var query = context.CreateQuery<Team>("Teams");
            var response = this.QueryResponse(xml, query);
            QueryOperationResponse qr = (QueryOperationResponse)response;
            long count = qr.TotalCount;
            Assert.AreEqual(2, qr.TotalCount, "qr.TotalCount");
            foreach (var team in response)
            {
                Assert.AreEqual(1, team.TeamID, "TeamID");
            }
        }

        [TestMethod]
        public void AtomParserReadInsistTest()
        {
            string xml = FeedStart + "<m:count>2</m:count>" +
                AnyEntry("t1", "<d:TeamID>1</d:TeamID>", null) + "</feed>";
            var query = context.CreateQuery<Team>("Teams");
            var response = this.QueryResponse(xml, query);
            QueryOperationResponse qr = (QueryOperationResponse)response;
            Assert.AreEqual(2, qr.TotalCount, "qr.TotalCount");
            using (var enumerator = response.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Assert.AreEqual(1, enumerator.Current.TeamID, "TeamID");
                    Assert.AreEqual(2, qr.TotalCount, "qr.TotalCount");
                }

                // Interesting side-effect: there is a cast enumerable
                // that will not clear out the .Current value.
                Assert.IsNotNull(enumerator.Current, "enumerator.Current");

                Assert.IsFalse(enumerator.MoveNext());
                Assert.IsFalse(enumerator.MoveNext());
                Assert.IsNotNull(enumerator.Current, "enumerator.Current");
                Assert.IsNotNull(enumerator.Current, "enumerator.Current");
            }

            Assert.AreEqual(2, qr.TotalCount, "qr.TotalCount");
        }

        public class EntityA
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public int Prop1 { get; set; }
            public EntityB B { get; set; }
            public GeographyPoint Point { get; set; }
        }

        public class EntityB
        {
            public int ID { get; set; }
            public int Prop1 { get; set; }
            public EntityA A { get; set; }
        }

        [TestMethod]
        public void AtomParserDuplicateTest_AfterODataLibIntegration()
        {
            // BUG:
            // The following breaking changes are taken in WCF DS client. Once "ODataLib - WCF DS client" integration is complete, enable the asserts in this test.
            // Current ODataLib behavior for WCF DS client is the following.
            // (a) Read the first entry/category element with a valid scheme even if it does not have a term attribute.
            // (b) Throw if there are duplicate entry/link/m:inline elements
            // (c) For m:properties:
            //      For entry/content/m:properties, the first m:properties in each of the entry/content elements is taken and merged.
            //      For entry/m:properties, all duplicates are read and merged.
            // (d) Relationship links are V3 feature. So, we will change WCF DS client do disallow duplicates.
            // (e) Named streams are V3 feature. If we see two different mime types when processing two named streams with the same name but are read and edit links respectively, we will throw.
            // (f) Named streams are V3 feature. If we see named stream duplicates, we will throw.

            const string atomPayloadTemplate =
@"HTTP/1.1 200 OK
Content-Type: application/atom+xml

<feed xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom'>
  <m:count>3</m:count>
  <title type='text'>ASet</title>
  <id>http://host/ASet</id>
  <updated>2010-09-01T23:36:00Z</updated>
  <link rel='self' title='ASet' href='ASet' />
  <entry>
    <id>http://host/ASet(2)</id>
    <id>http://host/ASet(1)</id>
    <title type='text'></title>
    <updated>2010-09-01T23:36:00Z</updated>
    <author>
      <name />
    </author>
    {0}
  </entry>
</feed>";
            var tests = new[]
                {
                    new
                    {
                        Type = "AssociationLinks",
                        Payload = @"
    <link rel='http://docs.oasis-open.org/odata/ns/relatedlinks/NavProp' href='http://odata.org/link1' type='application/xml'/>
    <link rel='http://docs.oasis-open.org/odata/ns/relatedlinks/NavProp' href='http://odata.org/link2' type='application/xml'/>"
                    },
                    new
                    {
                        Type = "NavigationLinks",
                        Payload = @"
    <link rel='http://docs.oasis-open.org/odata/ns/related/RelProp' href='http://odata.org/link3' type='application/atom+xml;type=entry'/>
    <link rel='http://docs.oasis-open.org/odata/ns/related/RelProp' href='http://odata.org/link4' type='application/atom+xml;type=entry'/>"
                    },
                    new
                    {
                        // mime types differ for the same stream. This is not allowed.
                        Type = "NamedStreams",
                        Payload = @"
    <link rel='http://docs.oasis-open.org/odata/ns/mediaresource/NamedStream' href='http://odata.org/readlink' type='mime/type1'/>
    <link rel='http://docs.oasis-open.org/odata/ns/edit-media/NamedStream' href='http://odata.org/editlink' type='mime/type2'/>"
                    },
                    new
                    {
                        Type = "MediaResourceStreams",
                        Payload = @"
    <link rel='http://docs.oasis-open.org/odata/ns/mediaresource/MediaResourceStreamDup' href='http://odata.org/readlink1' type='mime/type3'/>
    <link rel='http://docs.oasis-open.org/odata/ns/mediaresource/MediaResourceStreamDup' href='http://odata.org/readlink2' type='mime/type4'/>"
                    },
                    new
                    {
                        Type = "EditMedialinks",
                        Payload = @"
    <link rel='http://docs.oasis-open.org/odata/ns/edit-media/EditMediaStreamDup' href='http://odata.org/editlink1' type='mime/type5'/>
    <link rel='http://docs.oasis-open.org/odata/ns/edit-media/EditMediaStreamDup' href='http://odata.org/editlink2' type='mime/type6'/>"
                    },
                    new
                    {
                        Type = "Content",
                        Payload = @"    
          <content type='application/xml' >
             <m:properties>
                <d:ID m:type='Edm.Int32'>1</d:ID>
             </m:properties>
          </content>
          <content src='http://odata.org'/>"
                    },
                    new
                    {
                        Type = "Properties",
                        Payload = @"
    <link rel='http://docs.oasis-open.org/odata/ns/related/B' type='application/atom+xml;type=entry' title='B' href='Customer(1)/B'>
      <m:inline>
        <entry>
          <id>http://host/BSet(1)</id>
          <content type='application/xml' >
             <m:properties>
                <d:ID m:type='Edm.Int32'>1</d:ID>
                <d:Prop1 m:type='Edm.Int32'>1</d:Prop1>
             </m:properties>
             <m:properties>
                <d:Prop1 m:type='Edm.Int32'>2</d:Prop1>
             </m:properties>
          </content>
          <content type='application/xml' >
             <m:properties>
                <d:ID m:type='Edm.Int32'>2</d:ID>
                <d:Prop1 m:type='Edm.Int32'>3</d:Prop1>
             </m:properties>
             <m:properties>
                <d:Prop1 m:type='Edm.Int32'>4</d:Prop1>
             </m:properties>
          </content>
        </entry>      
      </m:inline>
    </link>
    <m:properties>
        <d:ID m:type='Edm.Int32'>1</d:ID>
        <d:Prop1 m:type='Edm.Int32'>1</d:Prop1>
    </m:properties>
    <m:properties>
        <d:Prop1 m:type='Edm.Int32'>2</d:Prop1>
    </m:properties>"
                    },
                    new
                    {
                        Type = "TypeScheme",
                        Payload = @"
    <category scheme='http://docs.oasis-open.org/odata/ns/scheme' />
    <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='NamespaceName.EntityB' />"
                    },
                    new
                    {
                        Type = "InlineElements",
                        Payload = @"
      <link rel='http://docs.oasis-open.org/odata/ns/related/B' type='application/atom+xml;type=entry' title='B' href='Customer(1)/B'>
      <m:inline>
        <entry>
          <id>http://host/BSet(2)</id>
          <content type='application/xml' >
             <m:properties>
                <d:ID m:type='Edm.Int32'>2</d:ID>
                <d:Prop1 m:type='Edm.Int32'>5</d:Prop1>
             </m:properties>
             <m:properties>
                <d:Prop1 m:type='Edm.Int32'>6</d:Prop1>
             </m:properties>
          </content>
        </entry>
      </m:inline>
      <m:inline/>
      </link>"
                    },
                };

            string[] shouldFailTests = new string[] { "AssociationLinks", "NamedStreams", "Content", "MediaResourceStreams", "EditMedialinks", "InlineElements", "Properties" };
            string[] shouldPassTests = new string[] { "NavigationLinks", "TypeScheme" };

            PlaybackServiceDefinition playbackService = new PlaybackServiceDefinition();

            using (TestWebRequest request = playbackService.CreateForInProcessWcf())
            {
                request.ServiceType = typeof(PlaybackService);
                request.ForceVerboseErrors = true;
                request.StartService();

                TestUtil.RunCombinations(tests, (test) =>
                {
                    DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                    ctx.EnableAtom = true;

                    playbackService.OverridingPlayback = string.Format(atomPayloadTemplate, test.Payload);
                    EntityA entity = null;
                    Exception ex = TestUtil.RunCatching(() => entity = ctx.CreateQuery<EntityA>("ASet").FirstOrDefault());

                    if (shouldFailTests.Contains(test.Type))
                    {
                        Assert.IsNotNull(ex, test.Type + " expects an exception but got none.");
                    }
                    else
                    {
                        Assert.IsNull(ex, test.Type + " expects to pass but got an exception: " + ex);
                    }

                    // Duplicate navigation links are allowed.
                    if (test.Type == "NavigationLinks")
                    {
                        EntityDescriptor descriptor = ctx.Entities.First();
                        Assert.AreEqual(1, descriptor.LinkInfos.Count());
                        Assert.AreEqual("http://odata.org/link4", descriptor.LinkInfos[0].NavigationLink.AbsoluteUri);
                    }

                    // Read the first entry/category element with a valid scheme even if the term attribute is not present.
                    if (test.Type == "TypeScheme")
                    {
                        EntityDescriptor descriptor = ctx.Entities.First();
                        Assert.AreEqual(null, descriptor.ServerTypeName);
                    }
                });
            }
        }

        [TestMethod]
        public void AtomParserLinkRelTest()
        {
            const string atomPayloadTemplate =
@"HTTP/1.1 200 OK
Content-Type: application/atom+xml

<feed xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom'>
  <title type='text'>ASet</title>
  <id>http://host/ASet</id>
  <updated>2010-09-01T23:36:00Z</updated>
  <link rel='self' title='ASet' href='ASet' />
  <entry>
    <id>http://host/ASet(1)</id>
    <title type='text'></title>
    <updated>2010-09-01T23:36:00Z</updated>
    <author>
      <name />
    </author>
    <link rel='{0}' title='EntityA' href='ASet(1)' />
    <link rel='{1}' title='EntityA' href='ASet(1)' />
    <link rel='{2}' title='EntityA' href='ASet(1)/$value' />
    <category term='NamespaceName.EntityA' scheme='http://docs.oasis-open.org/odata/ns/scheme' />
    <content type='CustomType/CustomSubType' src='ASet(1)/$value' />
    <m:properties>
      <d:ID m:type='Edm.Int32'>1</d:ID>
    </m:properties>
  </entry>
</feed>";

            string[] selfLinkRefs = { "self", "http://www.iana.org/assignments/relation/self" };
            string[] editLinkRefs = { "edit", "http://www.iana.org/assignments/relation/edit" };
            string[] editMediaLinkRefs = { "edit-media", "http://www.iana.org/assignments/relation/edit-media" };

            PlaybackServiceDefinition playbackService = new PlaybackServiceDefinition();

            using (TestWebRequest request = playbackService.CreateForInProcessWcf())
            {
                request.ServiceType = typeof(PlaybackService);
                request.ForceVerboseErrors = true;
                request.StartService();

                TestUtil.RunCombinations(selfLinkRefs, editLinkRefs, editMediaLinkRefs, (self, edit, editMedia) =>
                {
                    DataServiceContext ctx = new DataServiceContext(request.ServiceRoot);
                    ctx.EnableAtom = true;

                    string atomPayload = string.Format(atomPayloadTemplate, self, edit, editMedia);
                    playbackService.OverridingPlayback = atomPayload;

                    foreach (var item in ctx.CreateQuery<EntityA>("ASet"))
                    {
                        Assert.IsNotNull(item, "item");
                    }

                    EntityDescriptor entity = ctx.Entities.Single();

                    Assert.AreEqual(ctx.BaseUri + "/ASet(1)", entity.SelfLink.OriginalString);
                    Assert.AreEqual(ctx.BaseUri + "/ASet(1)", entity.EditLink.OriginalString);
                    Assert.AreEqual(ctx.BaseUri + "/ASet(1)/$value", entity.ReadStreamUri.OriginalString);
                    Assert.AreEqual(ctx.BaseUri + "/ASet(1)/$value", entity.EditStreamUri.OriginalString);
                });
            }
        }

        public class EntityWithNavigationProperties
        {
            public int ID { get; set; }
            public EntityWithNavigationProperties Singleton { get; set; }
            public List<EntityWithNavigationProperties> Collection { get; set; }
        }

        [TestMethod]
        public void AtomParserNavigationLinkTypeTest()
        {
            const string atomPayloadTemplate =
@"HTTP/1.1 200 OK
Content-Type: application/atom+xml

<feed xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom'>
  <title type='text'>ASet</title>
  <id>http://host/ASet</id>
  <updated>2010-09-01T23:36:00Z</updated>
  <link rel='self' title='ASet' href='ASet' />
  <entry>
    <id>http://host/ASet(1)</id>
    <title type='text'></title>
    <updated>2010-09-01T23:36:00Z</updated>
    <author>
      <name />
    </author>
    <link rel='http://docs.oasis-open.org/odata/ns/related/{0}' title='{0}' href='ASet' type='{1}'>{2}</link>
    <category term='NamespaceName.EntityA' scheme='http://docs.oasis-open.org/odata/ns/scheme' />
    <content type='CustomType/CustomSubType' src='ASet(1)/$value' />
    <m:properties>
      <d:ID m:type='Edm.Int32'>1</d:ID>
    </m:properties>
  </entry>
</feed>";

            var linkTypes = new[]
            {
                new 
                {
                    LinkType = "application/atom+xml;type=entry",
                    ExpectedErrorMessage = new Func<bool, bool?, string>((collection, expandedFeed) => 
                        {
                            // If the link type is entry:
                            // - For collection property, it must fail since the type must match the model
                            // - If the link is expanded the payload must match the model
                            if (collection && expandedFeed == false) return ODataLibResourceUtil.GetString("ODataAtomReader_ExpandedEntryInFeedNavigationLink");
                            if (collection && expandedFeed == true) return ODataLibResourceUtil.GetString("ODataAtomReader_ExpandedFeedInEntryNavigationLink");
                            if (collection) return ODataLibResourceUtil.GetString("ODataAtomReader_DeferredEntryInFeedNavigationLink");
                            if (!collection && expandedFeed == true) return ODataLibResourceUtil.GetString("ODataAtomReader_ExpandedFeedInEntryNavigationLink");
                            return null;
                        }),
                    RecognizedLinkType = true,
                },
                new 
                {
                    LinkType = "application/atom+xml;type=feed",
                    ExpectedErrorMessage = new Func<bool, bool?, string>((collection, expandedFeed) =>
                        {
                            // If the link type is feed:
                            // - If the link is not expanded the type must match the model
                            // - If the link is expanded the expanded payload must match the model as well.
                            if (!collection) return ODataLibResourceUtil.GetString("ODataAtomReader_FeedNavigationLinkForResourceReferenceProperty", "Singleton");
                            if (collection && expandedFeed == false) return ODataLibResourceUtil.GetString("ODataAtomReader_ExpandedEntryInFeedNavigationLink");
                            return null;
                        }),
                    RecognizedLinkType = true,
                },
                new 
                {
                    LinkType = "application/atom+xml",
                    // Breaking change: we are not recognizing links even when type is missing
                    // If the link has no type, client doesn't recognize this as a nav. prop.
                    ExpectedErrorMessage = new Func<bool, bool?, string>((collection, expandedFeed) =>
                        {
                            if (!collection && expandedFeed == true) return ODataLibResourceUtil.GetString("ODataAtomReader_ExpandedFeedInEntryNavigationLink");
                            if (collection && expandedFeed == false) return ODataLibResourceUtil.GetString("ODataAtomReader_ExpandedEntryInFeedNavigationLink");
                            return null;
                        }),
                    RecognizedLinkType = true,
                },
                new 
                {
                    LinkType = "application/atom+xml;type=test",
                    // Breaking change: we are not recognizing links even when type is wrong
                    // If the link has wrong type, client doesn't recognize this as a nav. prop.
                    ExpectedErrorMessage = new Func<bool, bool?, string>((collection, expandedFeed) =>
                        {
                            if (!collection && expandedFeed == true) return ODataLibResourceUtil.GetString("ODataAtomReader_ExpandedFeedInEntryNavigationLink");
                            if (collection && expandedFeed == false) return ODataLibResourceUtil.GetString("ODataAtomReader_ExpandedEntryInFeedNavigationLink");
                            return null;
                        }),
                    RecognizedLinkType = true,
                },
            };

            string deferredLinkBody = string.Empty;
            string expandedFeedBody = "<m:inline><feed/></m:inline>";
            string expandedEntryBody = "<m:inline><entry><id>http://host/ASet</id></entry></m:inline>";

            PlaybackServiceDefinition playbackService = new PlaybackServiceDefinition();

            using (TestWebRequest request = playbackService.CreateForInProcessWcf())
            {
                request.ServiceType = typeof(PlaybackService);
                request.ForceVerboseErrors = true;
                request.StartService();

                TestUtil.RunCombinations(
                    new string[] { "Singleton", "Collection" },
                    new bool?[] { null, false, true },
                    linkTypes,
                    (propertyName, expandedFeed, linkType) =>
                    {
                        DataServiceContext ctx = new DataServiceContext(request.ServiceRoot);
                        ctx.EnableAtom = true;

                        string linkBody = expandedFeed.HasValue ? (expandedFeed == true ? expandedFeedBody : expandedEntryBody) : deferredLinkBody;
                        string atomPayload = string.Format(atomPayloadTemplate, propertyName, linkType.LinkType, linkBody);
                        playbackService.OverridingPlayback = atomPayload;

                        Exception exception = TestUtil.RunCatching(() => ctx.CreateQuery<EntityWithNavigationProperties>("ASet").AsEnumerable().Count());

                        string expectedExceptionMessage = linkType.ExpectedErrorMessage(propertyName == "Collection", expandedFeed);

                        if (exception == null)
                        {
                            Assert.IsNull(expectedExceptionMessage, "The test case was expected to fail.");
                            EntityDescriptor entity = ctx.Entities.Single(e => e.Identity == new Uri("http://host/ASet(1)"));
                            if (linkType.RecognizedLinkType)
                            {
                                Assert.AreEqual(ctx.BaseUri + "/ASet", entity.LinkInfos.Single(li => li.Name == propertyName).NavigationLink.OriginalString);
                            }
                            else
                            {
                                Assert.AreEqual(0, entity.LinkInfos.Count, "The link should not have been recognized as a navigation link.");
                            }
                        }
                        else
                        {
                            Assert.IsNotNull(expectedExceptionMessage, "The test case was expected to succeed.");
                            Assert.AreEqual(expectedExceptionMessage, exception.Message, "Unexpected error.");
                        }
                    });
            }
        }

        private sealed class AtomParserAttributeNamespaceTestCase
        {
            public string CategoryScheme { get; set; }
            public string CategoryTerm { get; set; }
            public string ContentSrc { get; set; }
            public string ContentType { get; set; }
            public string PropertyType { get; set; }
            public Exception ExpectedException { get; set; }
        }

        [TestMethod]
        public void AtomParserAttributeNamespaceTest()
        {
            const string atomPayloadTemplate =
@"HTTP/1.1 200 OK
Content-Type: application/atom+xml

<feed xmlns:d='http://docs.oasis-open.org/odata/ns/data' 
      xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' 
      xmlns:atom='http://www.w3.org/2005/Atom' 
      xmlns:invalid='http://odata.org/invalid/namespace' 
      xmlns:gml='http://www.opengis.net/gml'
      xmlns='http://www.w3.org/2005/Atom'>
  <title type='text'>ASet</title>
  <id>http://host/ASet</id>
  <updated>2010-09-01T23:36:00Z</updated>
  <link rel='self' title='ASet' href='ASet' />
  <entry>
    <id>http://host/ASet(1)</id>
    <title type='text'></title>
    <updated>2010-09-01T23:36:00Z</updated>
    <author>
      <name />
    </author>
    <link rel='self' title='EntityA' href='ASet(1)' />
    <link rel='edit' title='EntityA' href='ASet(1)' />
    <link rel='edit-media' title='EntityA' href='ASet(1)/$value' />
    <category {0}='NamespaceName.EntityA' {1}='http://docs.oasis-open.org/odata/ns/scheme' />
    <content {3}='http://odata.org/readstream1' />
    <m:properties>
      <d:ID {4}='Edm.Int32'>1</d:ID>
    </m:properties>
  </entry>
</feed>";

            const string defaultCategoryTerm = "term";
            const string defaultCategoryScheme = "scheme";
            const string defaultContentType = "type";
            const string defaultContentSource = "src";
            const string defaultPropertyType = "m:type";

            AtomParserAttributeNamespaceTestCase[] testCases = new AtomParserAttributeNamespaceTestCase[]
            {
                // Error cases
                new AtomParserAttributeNamespaceTestCase { CategoryScheme = "invalid:scheme" },
                new AtomParserAttributeNamespaceTestCase { CategoryTerm = "invalid:term" },
                new AtomParserAttributeNamespaceTestCase { ContentType = "invalid:type" },
                new AtomParserAttributeNamespaceTestCase { ContentSrc = "invalid:src" },
                new AtomParserAttributeNamespaceTestCase { PropertyType = "invalid:type" },
            };

            PlaybackServiceDefinition playbackService = new PlaybackServiceDefinition();

            using (TestWebRequest request = playbackService.CreateForInProcessWcf())
            {
                request.ServiceType = typeof(PlaybackService);
                request.ForceVerboseErrors = true;
                request.StartService();

                TestUtil.RunCombinations(
                    testCases,
                    testCase =>
                    {
                        DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                        ctx.EnableAtom = true;

                        // {0} - atom:category/@term
                        // {1} - atom:category/@scheme
                        // {2} - atom:content/@type
                        // {3} - atom:content/@src
                        // {4} - d:property/@m:type
                        string atomPayload = string.Format(
                            atomPayloadTemplate,
                            testCase.CategoryTerm ?? defaultCategoryTerm,
                            testCase.CategoryScheme ?? defaultCategoryScheme,
                            testCase.ContentType ?? defaultContentType,
                            testCase.ContentSrc ?? defaultContentSource,
                            testCase.PropertyType ?? defaultPropertyType);
                        playbackService.OverridingPlayback = atomPayload;


                        EntityA[] items;
                        try
                        {
                            items = ctx.CreateQuery<EntityA>("ASet").ToArray();
                        }
                        catch (InvalidOperationException ex)
                        {
                            if (testCase.ContentSrc != null && testCase.ContentSrc.StartsWith("invalid"))
                            {
                                Assert.AreEqual(ODataLibResourceUtil.GetString("ODataAtomReader_MediaLinkEntryMismatch"),
                                    ex.Message);
                                return;
                            }
                            else if (testCase.PropertyType != null && testCase.PropertyType.StartsWith("invalid"))
                            {
                                Assert.AreEqual(DataServicesClientResourceUtil.GetString("Deserialize_ExpectingSimpleValue"), ex.Message);
                                return;
                            }

                            Assert.Fail("Exception received but did not expect any failure.\n" + ex.Message);
                            return;
                        }

                        Assert.AreEqual(1, items.Length, "Expected 1 item in the payload.");
                        var firstItem = items[0];

                        var entities = ctx.Entities;
                        Assert.AreEqual(1, entities.Count, "Expected 1 entity in the context.");
                        var firstEntity = entities[0];

                        // Verify category/@term and category/@scheme by testing the type name
                        string expectedTypeName =
                            testCase.CategoryScheme != null && testCase.CategoryScheme.StartsWith("invalid") ||
                            testCase.CategoryTerm != null && testCase.CategoryTerm.StartsWith("invalid")
                            ? null
                            : "NamespaceName.EntityA";
                        Assert.AreEqual(expectedTypeName, firstEntity.ServerTypeName, "Type names don't match.");

                        // Verify content/@src by checking that the read stream of the entry is correct
                        Assert.AreEqual("http://odata.org/readstream1", firstEntity.ReadStreamUri.OriginalString, "ReadStreams don't match.");

                        // Verify content/@type by making sure the properties were read
                        Assert.AreEqual(1, firstItem.ID, "IDs don't match.");
                    });
            }
        }

        [TestMethod]
        public void AtomParserAttributeNamespacePrecedenceTest()
        {
            const string atomPayload =
@"HTTP/1.1 200 OK
Content-Type: application/atom+xml

<feed xmlns:d='http://docs.oasis-open.org/odata/ns/data' 
      xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' 
      xmlns:atom='http://www.w3.org/2005/Atom' 
      xmlns:invalid='http://odata.org/invalid/namespace' 
      xmlns:gml='http://www.opengis.net/gml'
      xmlns='http://www.w3.org/2005/Atom'>
  <title type='text'>ASet</title>
  <id>http://host/ASet</id>
  <updated>2010-09-01T23:36:00Z</updated>
  <link rel='self' title='ASet' href='ASet' />
  <entry>
    <id>http://host/ASet(1)</id>
    <title type='text'></title>
    <updated>2010-09-01T23:36:00Z</updated>
    <author>
      <name />
    </author>
    <link rel='self' title='EntityA' href='ASet(1)' />
    <link rel='edit' title='EntityA' href='ASet(1)' />
    <link rel='edit-media' title='EntityA' href='ASet(1)/$value' />
    <category term='NamespaceName.Invalid' scheme='http://docs.oasis-open.org/odata/ns/data/unsupported' 
              atom:term='NamespaceName.EntityA' atom:scheme='http://docs.oasis-open.org/odata/ns/scheme'/>
    <content type='application/invalid' atom:type='application/xml' 
             src='http://odata.org/invalid' atom:src='http://odata.org/readstream1'/>
    <m:properties>
      <d:ID type='Edm.Invalid' m:type='Edm:Int32'>1</d:ID>
    </m:properties>
  </entry>
</feed>";

            PlaybackServiceDefinition playbackService = new PlaybackServiceDefinition();

            using (TestWebRequest request = playbackService.CreateForInProcessWcf())
            {
                request.ServiceType = typeof(PlaybackService);
                request.ForceVerboseErrors = true;
                request.StartService();

                DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                ctx.EnableAtom = true;
                playbackService.OverridingPlayback = atomPayload;


                EntityA[] items;
                try
                {
                    items = ctx.CreateQuery<EntityA>("ASet").ToArray();
                }
                catch (InvalidOperationException ex)
                {
                    Assert.Fail("Exception received but did not expect any failure.\n" + ex.Message);
                    return;
                }

                Assert.AreEqual(1, items.Length, "Expected 1 item in the payload.");
                var firstItem = items[0];

                var entities = ctx.Entities;
                Assert.AreEqual(1, entities.Count, "Expected 1 entity in the context.");
                var firstEntity = entities[0];

                // Verify that the atom:scheme and atom:term attribute are ignored
                // Verify that since the scheme attribute value is wrong, the term attribute value is not read
                Assert.AreEqual(null, firstEntity.ServerTypeName, "Type names don't match.");

                // Verify content/@src by checking that the read stream of the entry is correct
                Assert.AreEqual("http://odata.org/invalid", firstEntity.ReadStreamUri.OriginalString, "ReadStreams don't match.");

                // Verify content/@type by making sure the properties were read
                Assert.AreEqual(1, firstItem.ID, "IDs don't match.");
            }
        }

        #region Payload builder helpers.

        public static string AnyEntry(string id, string properties, string links)
        {
            return "<entry " + CommonNamespaces + "><id>http://localhost/" + id + "</id>" +
                "<link rel='edit' href='" + id + "'/>" +
                "<content type='application/xml'><m:properties>" + properties +
                "</m:properties></content>" + links + "</entry>";
        }

        internal static string AnyEntry(
            string id = null,
            string editLink = null,
            string selfLink = null,
            string entryETag = null,
            string properties = null,
            string links = null,
            string serverTypeName = null)
        {
            return MediaEntry(id: id, editLink: editLink, selfLink: selfLink, entryETag: entryETag, properties: properties, links: links, serverTypeName: serverTypeName);
        }

        internal static string MediaEntry(
            string id = null,
            string editLink = null,
            string selfLink = null,
            string entryETag = null,
            string properties = null,
            string links = null,
            string serverTypeName = null,
            string readStreamUrl = null,
            string editStreamUrl = null,
            string mediaETag = null,
            string mediaType = null)
        {
            string result = "<entry " + CommonNamespaces;

            if (entryETag != null)
            {
                result += " m:etag='W/\"" + entryETag + "\"'";
            }

            // finish the entry tag and then write id and editlink
            result += "><id>" + id + "</id>";

            if (editLink != null)
            {
                result += "<link rel='edit' href='" + editLink + "'/>" + Environment.NewLine;
            }

            if (selfLink != null)
            {
                result += "<link rel='self' href='" + selfLink + "'/>" + Environment.NewLine; ;
            }

            if (serverTypeName != null)
            {
                result += "<category term='" + serverTypeName + "' scheme='http://docs.oasis-open.org/odata/ns/scheme' />" + Environment.NewLine;
            }

            if (editStreamUrl != null)
            {
                result += "<link rel='edit-media' href='" + editStreamUrl + "' ";

                if (mediaETag != null)
                {
                    result += "m:etag='" + mediaETag + "'";
                }

                result += " />" + Environment.NewLine;
            }

            if (readStreamUrl != null)
            {
                result += "<content src='" + readStreamUrl + "' ";
                if (mediaType != null)
                {
                    result += "type='" + mediaType + "' ";
                }

                result += " />" + Environment.NewLine;
                if (properties != null)
                {
                    result += "<m:properties>" + properties + "</m:properties>" + Environment.NewLine;
                }
            }
            else if (properties != null)
            {
                result += "<content type='application/xml'><m:properties>" + properties + "</m:properties></content>" + Environment.NewLine;
            }

            result += links + Environment.NewLine;
            result += "</entry>";

            return result;
        }

        public static string NextLink(string uri)
        {
            return "<link rel='next' href='" + uri + "' />";
        }

        #endregion Payload builder helpers.

        #region Materialization API helpers.

        internal static XmlReader ToXml(string text)
        {
            var settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.NameTable = new NameTable();
            var reader = XmlReader.Create(new StringReader(text), settings);
            return reader;
        }


        #endregion Materialization API helpers.
    }
}
