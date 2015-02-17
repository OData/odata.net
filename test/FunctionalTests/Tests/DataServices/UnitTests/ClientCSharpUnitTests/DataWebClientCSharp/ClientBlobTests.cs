//---------------------------------------------------------------------
// <copyright file="ClientBlobTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Client;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using AstoriaUnitTests.Data;
    using AstoriaUnitTests.DataWebClientCSharp;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;

    #endregion Namespaces

    [TestClass]
    public class ClientBlobTests
    {
        private const string FeedStart = AtomParserTests.FeedStart;
        private const string EmptyFeed = AtomParserTests.EmptyFeed;

        private Uri serviceRoot;
        private DataServiceContext context;

        [TestInitialize]
        public void Initialize()
        {
            ReadOnlyTestContext.ClearBaselineIncludes();
            this.serviceRoot = new Uri("http://localhost/");
            this.context = new DataServiceContext(serviceRoot);
            this.context.EnableAtom = true;
        }

        [TestMethod]
        public void BlobAttributesTest()
        {
            string xml = FeedStart + AnyEntry("e1", "<d:ID>1</d:ID><d:Member>abc</d:Member>", null) + "</feed>";
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                // AttributeUsage isn't enforced by the CLR and shouldn't be relied upon
                // new Dimension("MediaEntryAttributeCount", new int[] { 0, 1, 2 }),
                new Dimension("MediaEntryAttributeCount", new int[] { 0, 1 }),
                new Dimension("MediaMemberName", new string[] { "", "Member", "NotMember" }),
                new Dimension("HasStreamAttributeCount", new int[] { 0, 1 }),
                new Dimension("MimeTypeAttributeCount", new int[] { 0, 1 }),
                new Dimension("MimeTypeMemberName", new string[] { null, "Member", "Member2" }),
                new Dimension("MimeTypeMime", new string[] { null, "text/plain", "*/*" }));
            ModuleBuilder moduleBuilder = TestUtil.CreateModuleBuilder("BlobAttributesTest");
            int id = 0;
            this.context.MergeOption = MergeOption.NoTracking;
            TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    int MediaEntryAttributeCount = (int)values["MediaEntryAttributeCount"];
                    string MediaMemberName = (string)values["MediaMemberName"];
                    int HasStreamAttributeCount = (int)values["HasStreamAttributeCount"];
                    int MimeTypeAttributeCount = (int)values["MimeTypeAttributeCount"];
                    string MimeTypeMemberName = (string)values["MimeTypeMemberName"];
                    string MimeTypeMime = (string)values["MimeTypeMime"];

                    id++;

                    // Skip uninteresting cases.
                    if (MediaEntryAttributeCount == 0 && MimeTypeAttributeCount == 0 && HasStreamAttributeCount == 0)
                    {
                        return;
                    }

                    if (MediaEntryAttributeCount == 0 && MediaMemberName != "")
                    {
                        return;
                    }

                    if (MimeTypeAttributeCount == 0 && (MimeTypeMemberName != null || MimeTypeMime != null))
                    {
                        return;
                    }

                    const TypeAttributes blobEntityTypeAttributes = TypeAttributes.Class | TypeAttributes.Public;
                    Type parentType = typeof(TypedEntity<int, string>);
                    TypeBuilder entityTypeBuilder = moduleBuilder.DefineType("BlobEntityType" + id, blobEntityTypeAttributes, parentType);

                    for (int i = 0; i < MediaEntryAttributeCount; i++)
                    {
                        CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(
                            typeof(MediaEntryAttribute).GetConstructors().Single(),
                            new object[] { MediaMemberName });
                        entityTypeBuilder.SetCustomAttribute(attributeBuilder);
                    }

                    for (int i = 0; i < MimeTypeAttributeCount; i++)
                    {
                        CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(
                            typeof(MimeTypeAttribute).GetConstructors().Single(),
                            new object[] { MimeTypeMemberName, MimeTypeMime });
                        entityTypeBuilder.SetCustomAttribute(attributeBuilder);
                    }

                    for (int i = 0; i < HasStreamAttributeCount; i++)
                    {
                        CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(typeof(HasStreamAttribute).GetConstructor(Type.EmptyTypes), new object[0]);
                        entityTypeBuilder.SetCustomAttribute(attributeBuilder);
                    }

                    Type entityType = entityTypeBuilder.CreateType();
                    List<object> list = null;
                    Exception exception = TestUtil.RunCatching(() =>
                        {
                            list = (List<object>)typeof(ClientBlobTests).GetMethod("IterateOverQuery").MakeGenericMethod(entityType).Invoke(this, new object[] { xml });
                        });
                    TestUtil.AssertExceptionExpected(exception,
                        MediaEntryAttributeCount > 0 && MediaMemberName != "Member",
                        MediaEntryAttributeCount == 2);
                    if (exception == null)
                    {
                        Assert.AreEqual(1, list.Count, "list.Count");
                    }
                    else
                    {
                        string text = exception.ToString();
                        if (MediaEntryAttributeCount == 2)
                        {
                            // AttributeUsage isn't enforced by the CLR and shouldn't be relied upon
                            TestUtil.AssertContains(text, "The AttributeUsage in the attribute definition should be preventing more than 1 pe");
                        }
                        else if (MediaEntryAttributeCount > 0 && MediaMemberName != "Member")
                        {
                            // Error string references non-existing attribute type
                            // This needs to be updated when the error string is fixed to refer to the correct type name.
                            TestUtil.AssertContains(text, "has a MediaEntry attribute that references a property called");
                        }
                    }
                });
        }

        [TestMethod]
        public void BlobStreamUriRefresh()
        {
            string xml = FeedStart +
                MediaEntry(
                id: "http://localhost/eset(1)",
                editLink: "eset(1)",
                readStreamUrl: "http://media-entry/v1/",
                mediaType: "text/plain",
                properties: "<d:ID>1</d:ID>",
                editStreamUrl: "http://edit-media-link/v1/") + 
                "</feed>";

            string xmlV2 = xml.Replace("/v1/", "/v2/");
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("StartingState", EntityStateData.Values),
                new Dimension("MergeOption", MergeOptionData.Values));
            TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    this.ClearContext();
                    
                    EntityStateData startingState = (EntityStateData)values["StartingState"];
                    MergeOptionData option = (MergeOptionData)values["MergeOption"];

                    // Set up the entity to match the identity on the sample XML.
                    Photo entity = new Photo();
                    entity.ID = 1;
                    startingState.ApplyToObject(this.context, entity, "eset");
                    if (startingState.State != EntityStates.Detached)
                    {
                        Uri identity = this.context.Entities.Single().Identity;
                        Trace.WriteLine("Identity: " + (identity == null ? String.Empty : identity.AbsoluteUri));
                    }

                    this.context.MergeOption = option.Option;
                    var response = QueryResponse(xml, this.context.CreateQuery<Photo>("eset"));
                    foreach (var item in response)
                    {
                        Assert.IsNotNull(item);
                        Assert.AreEqual(1, item.ID, "item.ID");
                    }

                    if (option.IsTracking)
                    {
                        // If we started off detached, then the context created its own copy.
                        if (startingState.State == EntityStates.Detached)
                        {
                            entity = (Photo)this.context.Entities.Single().Entity;
                        }

                        EntityStates expectedState = startingState.ExpectedStateAfterRefresh(option.Option);
                        EntityStates actualState = EntityStateData.GetStateForEntity(this.context, entity);
                        Assert.AreEqual(expectedState, actualState, "state of entity after reading");
                        if (startingState.State != EntityStates.Added)
                        {
                            AssertEntityCount(1, "single tracked entity at this point");
                        }
                        else
                        {
                            AssertEntityCount(2, "two tracked entities at this point");
                            
                            // Get the entity that was loaded, not the one that we originally created.
                            entity = (Photo)this.context.Entities.Where(o => o.Entity != entity).Single().Entity;
                        }
                    }
                    else
                    {
                        if (startingState.State == EntityStates.Detached)
                        {
                            // Nothing else to do in this test, as effectively nothing changes.
                            AssertEntityCount(0, "nothing tracked, nothing attached.");
                            return;
                        }
                        else
                        {
                            AssertEntityCount(1, "nothing tracked, but we did attach something during test setup.");
                        }
                    }

                    Uri readStreamUri = this.context.GetReadStreamUri(entity);
                    EntityDescriptor descriptor = this.context.GetEntityDescriptor(entity);
                    if (option.IsTracking)
                    {
                        Assert.IsNotNull(readStreamUri, "readStreamUri");
                        Assert.AreEqual("http://media-entry/v1/", readStreamUri.OriginalString, "readStreamUri.OriginalString");

                        Assert.IsNotNull(descriptor.EditLink, "descriptor.EditLink");
                        Assert.AreEqual("http://localhost/eset(1)", descriptor.EditLink.OriginalString, "descriptor.EditLink.OriginalString");
                        
                        Assert.IsNotNull(descriptor.EditStreamUri, "descriptor.EditStreamUri");
                        Assert.AreEqual("http://edit-media-link/v1/", descriptor.EditStreamUri.OriginalString, "descriptor.EditStreamUri.OriginalString");
                        
                        Assert.IsNotNull(descriptor.ReadStreamUri, "descriptor.ReadStreamUri");
                        Assert.AreEqual(readStreamUri, descriptor.ReadStreamUri, "GetReadStreamUri and descriptor.ReadStreamUri");
                    }
                    else
                    {
                        Assert.AreEqual(startingState.State, descriptor.State, "descriptor.State");
                        AssertDescriptorForNonTrackedRefresh(readStreamUri, descriptor);
                    }

                    response = QueryResponse(xmlV2, this.context.CreateQuery<Photo>("eset"));
                    foreach (var item in response)
                    {
                        Assert.IsNotNull(item);
                        Assert.AreEqual(1, item.ID, "item.ID");
                    }

                    readStreamUri = this.context.GetReadStreamUri(entity);
                    descriptor = this.context.GetEntityDescriptor(entity);
                    if (option.IsTracking)
                    {
                        Assert.IsNotNull(readStreamUri, "readStreamUri");
                        Assert.AreEqual("http://media-entry/v2/", readStreamUri.OriginalString, "readStreamUri.OriginalString");

                        Assert.IsNotNull(descriptor.EditLink, "descriptor.EditLink");
                        Assert.AreEqual("http://localhost/eset(1)", descriptor.EditLink.OriginalString, "descriptor.EditLink.OriginalString");

                        Assert.IsNotNull(descriptor.EditStreamUri, "descriptor.EditStreamUri");
                        Assert.AreEqual("http://edit-media-link/v2/", descriptor.EditStreamUri.OriginalString, "descriptor.EditStreamUri.OriginalString");

                        Assert.IsNotNull(descriptor.ReadStreamUri, "descriptor.ReadStreamUri");
                        Assert.AreEqual(readStreamUri, descriptor.ReadStreamUri, "GetReadStreamUri and descriptor.ReadStreamUri");
                    }
                    else
                    {
                        AssertDescriptorForNonTrackedRefresh(readStreamUri, descriptor);
                    }
                });
        }

        [TestMethod]
        public void BlobStreamSaveChanges()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            using (PlaybackService.InspectRequestPayload.Restore())
            using (PlaybackService.OverridingPlayback.Restore())
            {
                request.ServiceType = typeof(PlaybackService);
                request.ForceVerboseErrors = true;
                request.StartService();
                byte[] content = new byte[] { 1, 2, 3 };

                DataServiceContext ctx = new DataServiceContext(new Uri(request.BaseUri, UriKind.RelativeOrAbsolute));
                ctx.EnableAtom = true;
                ctx.Format.UseAtom();
                Stream stream = new MemoryStream(content);
                ClientCSharpRegressionTests.CustomerWithStream customer = new ClientCSharpRegressionTests.CustomerWithStream() { ID = 1, Name = "Foo" };
                ctx.AddObject("Customers", customer);
                ctx.SetSaveStream(customer, stream, true, new DataServiceRequestArgs() { ContentType = "application/jpeg" });

                EntityDescriptor ed = ctx.Entities[0];

                int i = 0;
                string id = "http://myidhost/somerandomIDUrl";
                string selfLink = request.BaseUri + "/self-link/Customer(1)";
                string editLink = request.BaseUri + "/edit-link/Customer(1)";
                string etag = "someetagvalue";
                string serverTypeName = "SomeRandomTypeName";

                string editMediaLink = "http://myhost/somerandomeUrl/edit-media-link/v1";
                string readMediaLink = "http://myedithost/somerandomUrl/foo/self-media-link/v1";
                string streamETag = "somerandomeStreamETag";
                string mediaType = "application/jpeg";
                string locationHeader = "http://mylocationhost/somerandomUrl/location/";

                PlaybackService.InspectRequestPayload.Value = (message) =>
                {
                    if (i == 0)
                    {
                        string xml = MediaEntry(
                            id: id,
                            selfLink: selfLink,
                            editLink: editLink, 
                            etag: etag,
                            properties: "<d:ID>5</d:ID>", 
                            serverTypeName: serverTypeName, 
                            readStreamUrl: readMediaLink,
                            editStreamUrl: editMediaLink,
                            mediaETag: streamETag,
                            mediaType: mediaType);

                        // The response payload has some random location header and no etag header. 
                        // Verify that the edit link, etag and type name are used from the payload.
                        string responsePayload =
                            "HTTP/1.1 201 Created" + Environment.NewLine +
                            "Content-Type: application/atom+xml" + Environment.NewLine +
                            "Location: " + locationHeader + Environment.NewLine +
                            Environment.NewLine +
                            xml;
                        PlaybackService.OverridingPlayback.Value = responsePayload;
                    }
                    else if (i == 1)
                    {
                        // Make sure that no metadata from the payload is merged yet into the public entity descriptor
                        // In V2, we already set the edit link and id to the location header. So that part cannot be changed now.
                        Assert.AreEqual(ed.Identity, locationHeader, "id must be set to location header");
                        Assert.AreEqual(ed.EditLink.AbsoluteUri, locationHeader, "edit link value must be equal to the location header");
                        Assert.AreEqual(ed.SelfLink, null, "Self link must not be populated");
                        Assert.AreEqual(ed.ETag, null, "etag must be null");
                        Assert.AreEqual(ed.ServerTypeName, null, "server type name must be null");
                        Assert.AreEqual(ed.EditStreamUri, null, "edit media stream must be null");
                        Assert.AreEqual(ed.ReadStreamUri, null, "read media stream must be null");
                        Assert.AreEqual(ed.StreamETag, null, "stream etag must be null");

                        // no response for PUT
                        string responsePayload =
                            "HTTP/1.1 200 OK" + Environment.NewLine +
                            "Content-Type: application/atom+xml" + Environment.NewLine +
                            Environment.NewLine;
                        PlaybackService.OverridingPlayback.Value = responsePayload;
                    }

                    i++;
                };

                
                ctx.SaveChanges();

                Assert.IsTrue(i == 2, "only 2 requests should be sent to the server)");

                // Make sure the edit link from the payload is used, not the location header
                Assert.IsTrue(PlaybackService.LastPlayback.Contains("PATCH " + editLink), "should use edit link from the POST response payload");

                // make sure the etag is used from the payload
                Assert.IsTrue(PlaybackService.LastPlayback.Contains("If-Match: W/\"" + etag + "\""), "should use the etag value from the POST response payload");

                // make sure the type is used from the payload
                Assert.IsTrue(PlaybackService.LastPlayback.Contains("category term=\"#" + serverTypeName + "\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\""), "should use the server type name as specified in the POST response payload");

                Assert.AreEqual(ed.Identity, id, "id must be set to location header");
                Assert.AreEqual(ed.EditLink.AbsoluteUri, editLink, "edit link value must be equal to the location header");
                Assert.AreEqual(ed.SelfLink, selfLink, "Self link should be populated");
                Assert.AreEqual(ed.ETag, null, "etag must be null, since the PATCH response didn't send any etag in the response header");
                Assert.AreEqual(ed.ServerTypeName, serverTypeName, "server type name must NOT be null");
                Assert.AreEqual(ed.EditStreamUri, editMediaLink, "edit media stream must NOT be null");
                Assert.AreEqual(ed.ReadStreamUri, readMediaLink, "read media stream must NOT be null");
                Assert.AreEqual(ed.StreamETag, streamETag, "stream etag must NOT be null");
            }
        }

        public List<object> IterateOverQuery<T>(string xml)
        {
            List<object> result = new List<object>();
            foreach (var item in this.QueryResponse(xml, this.context.CreateQuery<T>("EntitySet")))
            {
                result.Add(item);
            }

            return result;
        }

        private static void AssertDescriptorForNonTrackedRefresh(Uri readStreamUri, EntityDescriptor descriptor)
        {
            // If we're not tracking, then the URI will always be null,
            // and there's no way to set this on attachment with the
            // current API.
            Assert.IsNull(readStreamUri, "readStreamUri");
            Assert.IsNull(descriptor.EditStreamUri, "descriptor.EditStreamUri");
            Assert.IsNull(descriptor.ReadStreamUri, "descriptor.ReadStreamUri");

            if (descriptor.State == EntityStates.Added)
            {
                Assert.IsNull(descriptor.EditLink, "Edit Link must be null when in added state");
            }
            else
            {
                Assert.IsNotNull(descriptor.EditLink, "descriptor.EditLink");
                Assert.AreEqual(descriptor.Identity, descriptor.EditLink.OriginalString, "identity and edit link are the same");
            }
        }

        #region Helper APIs forwarded to other tests.

        private static string AnyEntry(string id = null, string properties = null, string links = null)
        {
            return AtomParserTests.AnyEntry(id, properties, links);
        }

        private static string LinkEdit(string href)
        {
            return ProjectionTests.LinkEdit(href);
        }

        private static string LinkEditMedia(string href, string etag)
        {
            return ProjectionTests.LinkEditMedia(href, etag);
        }

        private static string MediaEntry(
            string id = null,
            string selfLink = null,
            string editLink = null,
            string etag = null,
            string properties = null,
            string links = null,
            string serverTypeName = null,
            string readStreamUrl = null,
            string editStreamUrl = null,
            string mediaETag = null,
            string mediaType = null)
        {
            return AtomParserTests.MediaEntry(
            id: id,
            selfLink: selfLink,
            editLink: editLink,
            entryETag: etag,
            properties: properties,
            links: links,
            serverTypeName: serverTypeName,
            readStreamUrl: readStreamUrl,
            editStreamUrl: editStreamUrl,
            mediaETag: mediaETag,
            mediaType: mediaType);
        }

        private void AssertEntityCount(int expectedCount, string description)
        {
            ProjectionTests.AssertEntityCountForContext(expectedCount, description, this.context);
        }

        private void AssertLinkCount(int expectedCount, string description)
        {
            ProjectionTests.AssertLinkCountForContext(expectedCount, description, this.context);
        }

        private void ClearContext()
        {
            DataServiceContextTests.ClearContext(this.context);
        }

        private QueryOperationResponse<T> QueryResponse<T>(string xml, IQueryable<T> query)
        {
            return (QueryOperationResponse<T>)
                AtomParserTests.CreateQueryResponse<T>(this.context, AtomParserTests.EmptyHeaders, query as DataServiceQuery, xml);
        }

        #endregion Helper APIs forwarded to other tests.
    }
}
