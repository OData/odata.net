//---------------------------------------------------------------------
// <copyright file="NamedStreamTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.DataWebClientCSharp
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using System.Data.Test.Astoria;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NamedStreamTests
    {
        private static InProcessWcfWebRequest request;
        private const string Id = "http://idservice/identity";
        private const string EditLink = "http://editservice/editlink/Customers(1)";
        private const string MediaEditLink = "http://mediaeditservice/Thumbnail";
        private const string MediaQueryLink = "http://mediaqueryservice/Thumbnail";
        private const string MediaETag = "mediaETag";
        private const string MediaContentType = "image/jpeg";
        private const string Properties = "<d:ID>123</d:ID><d:Name>Foo</d:Name>";
        private const string EditLinkXml = "<link rel='edit' href='" + EditLink + "' />";

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            OpenWebDataServiceHelper.ForceVerboseErrors = true;
            request = (InProcessWcfWebRequest)TestWebRequest.CreateForInProcessWcf();
            request.ServiceType = typeof(PlaybackService);
            request.ForceVerboseErrors = true;
            request.StartService();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            if (request != null)
            {
                request.Dispose();
                request = null;
            }
        }

        [TestMethod]
        public void VerifyStreamInfoAfterQuery()
        {
            // Verifying media link information in query scnearios
            // Populate the context with a single customer instance
            string payload = AtomParserTests.AnyEntry(
                id: Id,
                properties: Properties,
                links: GetNamedStreamEditLink(MediaEditLink, contentType: MediaContentType, etag: MediaETag) + GetNamedStreamSelfLink(MediaQueryLink, contentType: MediaContentType));

            string nonbatchPayload = PlaybackService.ConvertToPlaybackServicePayload(null, payload);
            string batchPayload = PlaybackService.ConvertToBatchQueryResponsePayload(nonbatchPayload);

            TestUtil.RunCombinations((IEnumerable<QueryMode>)Enum.GetValues(typeof(QueryMode)), (queryMode) =>
            {
                using (PlaybackService.OverridingPlayback.Restore())
                {
                    if (queryMode == QueryMode.BatchAsyncExecute ||
                        queryMode == QueryMode.BatchAsyncExecuteWithCallback ||
                        queryMode == QueryMode.BatchExecute)
                    {
                        PlaybackService.OverridingPlayback.Value = batchPayload;
                    }
                    else
                    {
                        PlaybackService.OverridingPlayback.Value = nonbatchPayload;
                    }

                    DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                    context.EnableAtom = true;
                    DataServiceQuery<Customer> q = (DataServiceQuery<Customer>)context.CreateQuery<Customer>("Customers").Where(c1 => c1.ID == 1);
                    Customer c = ((IEnumerable<Customer>)DataServiceContextTestUtil.ExecuteQuery(context, q, queryMode)).Single();

                    Assert.AreEqual(context.Entities.Count, 1, "expecting just one entitydescriptor in the context");
                    Assert.AreEqual(context.Entities[0].StreamDescriptors.Count, 1, "expecting just one stream info in the entitydescriptor");
                    StreamDescriptor streamInfo = context.Entities[0].StreamDescriptors[0];
                    Assert.AreEqual(streamInfo.StreamLink.SelfLink, MediaQueryLink, "Self link should be as expected");
                    Assert.AreEqual(streamInfo.StreamLink.EditLink, MediaEditLink, "Edit link should be as expected");
                    Assert.AreEqual(streamInfo.StreamLink.ContentType, MediaContentType, "Content-Type did not match");
                    Assert.AreEqual(streamInfo.StreamLink.ETag, MediaETag, "ETag should match");
                }
            });
        }

        [TestMethod]
        public void SetSaveStreamNotAllowedInBatch()
        {
            // Saving named stream is not supported in batch
            DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
            context.EnableAtom = true;
            Customer c;
            using (PlaybackService.OverridingPlayback.Restore())
            {
                // Populate the context with a single customer instance
                string payload = AtomParserTests.AnyEntry(
                    id: Id,
                    properties: Properties,
                    links: GetNamedStreamEditLink(MediaEditLink, contentType: MediaContentType, etag: MediaETag) + GetNamedStreamSelfLink(MediaQueryLink, MediaContentType));

                PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(null, payload);
                c = context.Execute<Customer>(new Uri("/Customers(1)", UriKind.Relative)).Single();
            }

            context.SetSaveStream(c, "Thumbnail", new MemoryStream(), true, "image/jpeg");

            TestUtil.RunCombinations((IEnumerable<SaveChangesMode>)Enum.GetValues(typeof(SaveChangesMode)), (mode) =>
            {
                try
                {
                    DataServiceContextTestUtil.SaveChanges(context, SaveChangesOptions.BatchWithSingleChangeset, mode);
                    Assert.Fail("Named streams requests are not support in batch mode");
                }
                catch (NotSupportedException e)
                {
                    Assert.AreEqual(e.Message, AstoriaUnitTests.DataServicesClientResourceUtil.GetString("Context_BatchNotSupportedForNamedStreams"), "Expecting the correct error message");
                }
            });
        }

        [TestMethod]
        public void InsertEntityAndSetSaveStream()
        {
            // Making sure that inserting an entity followed by SetSaveStream works well
            TestUtil.RunCombinations(
                (IEnumerable<SaveChangesMode>)Enum.GetValues(typeof(SaveChangesMode)),
                UnitTestsUtil.BooleanValues,
                (mode, closeStream) =>
                {
                    using (PlaybackService.OverridingPlayback.Restore())
                    using (PlaybackService.InspectRequestPayload.Restore())
                    {
                        DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                        context.EnableAtom = true;
                        context.Format.UseAtom();

                        Customer c = new Customer() { ID = 1, Name = "Foo" };
                        context.AddObject("Customers", c);
                        MemoryStream stream = new MemoryStream(new byte[] { 0, 1, 2, 3 });
                        context.SetSaveStream(c, "Thumbnail", stream, closeStream, new DataServiceRequestArgs() { ContentType = "image/bmp" });

                        // verify the entity descriptor state
                        EntityDescriptor entityDescriptor = context.Entities.Single();
                        StreamDescriptor streamInfo = entityDescriptor.StreamDescriptors.Single();
                        Assert.IsTrue(entityDescriptor.State == EntityStates.Added, "entity must be in added state");
                        Assert.AreEqual(streamInfo.StreamLink.Name, "Thumbnail");
                        Assert.IsTrue(streamInfo.State == EntityStates.Modified, "named stream must be in modified state");

                        int i = 0;
                        PlaybackService.InspectRequestPayload.Value = (message) =>
                        {
                            if (i == 1)
                            {
                                // Verify the first request was a POST request
                                Assert.IsTrue(PlaybackService.LastPlayback.Contains(String.Format("POST {0}/Customers", request.ServiceRoot)), "the first request must be a POST request");
                                PlaybackService.OverridingPlayback.Value = null;
                            }
                            else if (i == 0)
                            {
                                // Populate the context with a single customer instance
                                string payload = AtomParserTests.AnyEntry(
                                    id: Id,
                                    editLink: request.ServiceRoot.AbsoluteUri + "/editLink/Customers(1)",
                                    properties: Properties,
                                    links: GetNamedStreamSelfLink(request.ServiceRoot + "/Customers(1)/SelfLink/Thumbnail", contentType: MediaContentType) +
                                    GetNamedStreamEditLink(request.ServiceRoot.AbsoluteUri + "/Customers(1)/EditLink/Thumbnail", contentType: MediaContentType, etag: MediaETag));

                                var headers = new List<KeyValuePair<string, string>>() {
                                new KeyValuePair<string, string>("Location", "http://locationservice/locationheader") };
                                PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(headers, payload);
                                XDocument document = XDocument.Load(message);
                                Assert.IsTrue(document.Element(AstoriaUnitTests.Tests.UnitTestsUtil.AtomNamespace + "entry") != null, "must contain an entry element");
                            }

                            // entity state should not be modified until all the responses have been changed
                            Assert.IsTrue(entityDescriptor.State == EntityStates.Added, "entity must be in added state");
                            Assert.AreEqual(streamInfo.StreamLink.Name, "Thumbnail");
                            Assert.IsTrue(streamInfo.State == EntityStates.Modified, "named stream must be in modified state");

                            // Also the stream info links should not be modified
                            Assert.IsTrue(streamInfo.StreamLink.SelfLink == null, "descriptor should not have been modified yet - self link must be null");
                            Assert.IsTrue(streamInfo.StreamLink.EditLink == null, "descriptor should not have been modified yet - edit link must be null");
                            Assert.IsTrue(String.IsNullOrEmpty(streamInfo.StreamLink.ContentType), "descriptor should not have been modified yet - content type must be null");
                            Assert.IsTrue(String.IsNullOrEmpty(streamInfo.StreamLink.ETag), "descriptor should not have been modified yet - etag must be null");
                            Assert.IsTrue(stream.CanWrite, "The stream hasn't been closed yet");
                            i++;
                        };

                        DataServiceContextTestUtil.SaveChanges(context, SaveChangesOptions.None, mode);
                        Assert.AreEqual(i, 2, "Only 2 request should have been made");

                        // Verify that the second request was a PUT request
                        Assert.IsTrue(PlaybackService.LastPlayback.Contains(String.Format("PUT {0}", streamInfo.StreamLink.EditLink)), "the second request must be a PUT request with the edit link");
                        Assert.AreEqual(streamInfo.StreamLink.SelfLink.AbsoluteUri, request.ServiceRoot + "/Customers(1)/SelfLink/Thumbnail", "self link must be null, since the payload did not have self link");
                        Assert.AreEqual(streamInfo.StreamLink.EditLink.AbsoluteUri, request.ServiceRoot + "/Customers(1)/EditLink/Thumbnail", "edit link should have been populated");
                        Assert.AreEqual(!stream.CanWrite, closeStream, "The stream must be closed");
                        VerifyRequestWasVersion3(PlaybackService.LastPlayback);
                    }
                });
        }

        [TestMethod]
        public void InsertMLEAndSetSaveStream()
        {
            // Making sure that inserting an MLE/MR followed by SetSaveStream works well
            TestUtil.RunCombinations(
                (IEnumerable<SaveChangesMode>)Enum.GetValues(typeof(SaveChangesMode)),
                UnitTestsUtil.BooleanValues,
                (mode, closeStream) =>
                {
                    using (PlaybackService.OverridingPlayback.Restore())
                    using (PlaybackService.InspectRequestPayload.Restore())
                    {
                        DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                        context.EnableAtom = true;
                        context.Format.UseAtom();

                        ClientCSharpRegressionTests.CustomerWithStream c = new ClientCSharpRegressionTests.CustomerWithStream() { ID = 1, Name = "Foo" };
                        context.AddObject("Customers", c);
                        MemoryStream defaultStream = new MemoryStream(new byte[] { 0, 1, 2, 3 });
                        MemoryStream thumbnailStream = new MemoryStream(new byte[] { 0, 1, 2, 3, 4 });
                        context.SetSaveStream(c, defaultStream, closeStream, new DataServiceRequestArgs() { ContentType = "image/bmp" });
                        context.SetSaveStream(c, "Thumbnail", thumbnailStream, closeStream, new DataServiceRequestArgs() { ContentType = "image/bmp" });

                        // verify the entity descriptor state
                        EntityDescriptor entityDescriptor = context.Entities.Single();
                        StreamDescriptor streamInfo = entityDescriptor.StreamDescriptors.Single();
                        Assert.IsTrue(entityDescriptor.State == EntityStates.Added, "entity must be in added state");
                        Assert.AreEqual(streamInfo.StreamLink.Name, "Thumbnail");
                        Assert.IsTrue(streamInfo.State == EntityStates.Modified, "named stream must be in modified state");

                        string editLink = request.ServiceRoot.AbsoluteUri + "/editLink/Customers(1)";
                        int i = 0;
                        PlaybackService.InspectRequestPayload.Value = (message) =>
                        {
                            if (i == 2)
                            {
                                // Verify that the second request was a PUT request
                                Assert.IsTrue(PlaybackService.LastPlayback.Contains(String.Format("PATCH {0}", editLink)), "the second request must be a PATCH request with the edit link");
                                Assert.AreEqual(!defaultStream.CanWrite, closeStream, "The default stream must have been in the desired state");
                            }
                            else if (i == 1)
                            {
                                // Verify the first request was a POST request
                                Assert.IsTrue(PlaybackService.LastPlayback.Contains(String.Format("POST {0}/Customers", request.ServiceRoot)), "the first request must be a POST request");
                                PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(null, null);
                                XDocument document = XDocument.Load(message);
                                Assert.IsTrue(document.Element(AstoriaUnitTests.Tests.UnitTestsUtil.AtomNamespace + "entry") != null, "must contain an entry element");
                                Assert.AreEqual(!defaultStream.CanWrite, closeStream, "The default stream must have been in the desired state immd after the request");
                            }
                            else if (i == 0)
                            {
                                // Populate the context with a single customer instance
                                string payload = AtomParserTests.MediaEntry(
                                    id: Id,
                                    editLink: editLink,
                                    properties: Properties,
                                    readStreamUrl: request.ServiceRoot + "/Customers(1)/readStreamUrl/$value",
                                    links: GetNamedStreamSelfLink(request.ServiceRoot + "/Customers(1)/SelfLink/Thumbnail", contentType: MediaContentType) +
                                           GetNamedStreamEditLink(request.ServiceRoot.AbsoluteUri + "/Customers(1)/EditLink/Thumbnail", contentType: MediaContentType, etag: MediaETag));

                                var headers = new List<KeyValuePair<string, string>>() {
                                new KeyValuePair<string, string>("Location", "http://locationservice/locationheader") };
                                PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(headers, payload);

                                // entity state should not be modified until all the responses have been changed
                                Assert.IsTrue(entityDescriptor.State == EntityStates.Modified, "entity must be in added state");
                                Assert.IsTrue(defaultStream.CanWrite, "The default stream hasn't been closed yet");
                            }

                            Assert.AreEqual(streamInfo.StreamLink.Name, "Thumbnail");
                            Assert.IsTrue(streamInfo.State == EntityStates.Modified, "named stream must be in modified state");

                            // Also the stream info links should not be modified
                            Assert.IsTrue(streamInfo.StreamLink.SelfLink == null, "descriptor should not have been modified yet - self link must be null");
                            Assert.IsTrue(streamInfo.StreamLink.EditLink == null, "descriptor should not have been modified yet - edit link must be null");
                            Assert.IsTrue(String.IsNullOrEmpty(streamInfo.StreamLink.ContentType), "descriptor should not have been modified yet - content type must be null");
                            Assert.IsTrue(String.IsNullOrEmpty(streamInfo.StreamLink.ETag), "descriptor should not have been modified yet - etag must be null");
                            Assert.IsTrue(thumbnailStream.CanWrite, "The thumbnail stream hasn't been closed yet");
                            i++;

                        };

                        DataServiceContextTestUtil.SaveChanges(context, SaveChangesOptions.None, mode);
                        Assert.AreEqual(i, 3, "Only 2 request should have been made");

                        // Verify that the second request was a PUT request
                        Assert.IsTrue(PlaybackService.LastPlayback.Contains(String.Format("PUT {0}", streamInfo.StreamLink.EditLink)), "the second request must be a PUT request with the edit link");
                        Assert.AreEqual(streamInfo.StreamLink.SelfLink.AbsoluteUri, request.ServiceRoot + "/Customers(1)/SelfLink/Thumbnail", "self link must be null, since the payload did not have self link");
                        Assert.AreEqual(streamInfo.StreamLink.EditLink.AbsoluteUri, request.ServiceRoot + "/Customers(1)/EditLink/Thumbnail", "edit link should have been populated");
                        Assert.AreEqual(!thumbnailStream.CanWrite, closeStream, "The stream must be in the desired state");
                    }
                });
        }

        [TestMethod]
        public void UpdateEntityAndSetSaveStream()
        {
            // Making sure that updating an entity followed by SetSaveStream works well
            TestUtil.RunCombinations(
                 UnitTestsUtil.BooleanValues,
                 (IEnumerable<SaveChangesMode>)Enum.GetValues(typeof(SaveChangesMode)),
                 UnitTestsUtil.BooleanValues,
                 (sendResponse, mode, closeStream) =>
                 {
                     using (PlaybackService.OverridingPlayback.Restore())
                     using (PlaybackService.InspectRequestPayload.Restore())
                     {
                         DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                         context.EnableAtom = true;
                         context.Format.UseAtom();

                         string payload = AtomParserTests.AnyEntry(
                                         id: Id,
                                         editLink: request.ServiceRoot.AbsoluteUri + "/editLink/Customers(1)",
                                         serverTypeName: typeof(Customer).FullName,
                                         properties: Properties,
                                         links: GetNamedStreamSelfLink(request.ServiceRoot + "/Customers(1)/SelfLink/Thumbnail", contentType: MediaContentType) +
                                         GetNamedStreamEditLink(request.ServiceRoot.AbsoluteUri + "/Customers(1)/EditLink/Thumbnail", contentType: MediaContentType, etag: MediaETag));

                         PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(null, payload);
                         context.ResolveType = name => typeof(Customer);
                         Customer c = (Customer)context.Execute<object>(new Uri("/Customers(1)", UriKind.Relative)).Single();
                         context.UpdateObject(c);
                         MemoryStream thumbnailStream = new MemoryStream();
                         context.SetSaveStream(c, "Thumbnail", thumbnailStream, closeStream, new DataServiceRequestArgs() { ContentType = "image/bmp" });

                         string namedStreamETagInEntityResponse = null;
                         string namedStreamETagInResponseHeader = "ETagInResponseHeader";
                         int i = 0;
                         PlaybackService.InspectRequestPayload.Value = (message) =>
                         {
                             if (i == 1)
                             {
                                 // Verify the first request was a PUT request to the entity
                                 Assert.IsTrue(PlaybackService.LastPlayback.Contains(String.Format("PATCH {0}/editLink/Customers(1)", request.ServiceRoot)), "the first request must be a PUT request to the entity");

                                 var headers = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("ETag", namedStreamETagInResponseHeader) };
                                 PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(headers, null);
                             }
                             else if (i == 0)
                             {
                                 string updatePayload = null;
                                 if (sendResponse)
                                 {
                                     namedStreamETagInEntityResponse = "ETagInResponsePayload";
                                     updatePayload = AtomParserTests.AnyEntry(
                                        id: Id,
                                        editLink: request.ServiceRoot.AbsoluteUri + "/editLink/Customers(1)",
                                        properties: Properties,
                                        links: GetNamedStreamSelfLink(request.ServiceRoot + "/Customers(1)/SelfLink/Thumbnail", contentType: MediaContentType) +
                                        GetNamedStreamEditLink(request.ServiceRoot.AbsoluteUri + "/Customers(1)/EditLink/Thumbnail", contentType: MediaContentType, etag: namedStreamETagInEntityResponse));
                                 }
                                 PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(null, updatePayload);

                                 // Verify the payload of the first request - it must have an entry element.
                                 XDocument document = XDocument.Load(message);
                                 Assert.IsTrue(document.Element(AstoriaUnitTests.Tests.UnitTestsUtil.AtomNamespace + "entry") != null, "must contain an entry element");
                             }

                             i++;
                             Assert.IsTrue(thumbnailStream.CanWrite, "The thumbnail stream hasn't been closed yet");
                         };

                         DataServiceContextTestUtil.SaveChanges(context, SaveChangesOptions.None, mode);
                         Assert.AreEqual(i, 2, "Only 2 request should have been made");

                         // Verify that the second request was a PUT request
                         Assert.IsTrue(PlaybackService.LastPlayback.Contains(String.Format("PUT {0}/Customers(1)/EditLink/Thumbnail", request.ServiceRoot)), "the second request must be a PUT request to named stream");

                         // If the first update sent a response, then the new etag must be used
                         Assert.IsTrue(PlaybackService.LastPlayback.Contains(String.Format("If-Match: {0}", namedStreamETagInEntityResponse ?? MediaETag)), "etag must be sent in the PUT request to the named stream");

                         Assert.AreEqual(!thumbnailStream.CanWrite, closeStream, "The thumbnail stream must be desired state after SaveChanges returns");

                         StreamDescriptor sd = context.Entities[0].StreamDescriptors[0];

                         // Verify that the etag from the response overrides the etag from the header when the response is present
                         Assert.AreEqual(sd.StreamLink.ETag, namedStreamETagInResponseHeader, "make sure the etag was updated to the latest etag");
                     }
                 });
        }

        [TestMethod]
        public void SetSaveStreamAndUpdateEntity()
        {
            // Making sure that setting the save stream followed by updating an entity works well
            TestUtil.RunCombinations((IEnumerable<SaveChangesMode>)Enum.GetValues(typeof(SaveChangesMode)), (mode) =>
                {
                    using (PlaybackService.OverridingPlayback.Restore())
                    using (PlaybackService.InspectRequestPayload.Restore())
                    {
                        DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                        context.EnableAtom = true;
                        context.Format.UseAtom();

                        string payload = AtomParserTests.AnyEntry(
                                        id: Id,
                                        editLink: request.ServiceRoot.AbsoluteUri + "/editLink/Customers(1)",
                                        properties: Properties,
                                        links: GetNamedStreamSelfLink(request.ServiceRoot + "/Customers(1)/SelfLink/Thumbnail", contentType: MediaContentType) +
                                        GetNamedStreamEditLink(request.ServiceRoot.AbsoluteUri + "/Customers(1)/EditLink/Thumbnail", MediaContentType));

                        PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(null, payload);
                        Customer c = context.Execute<Customer>(new Uri("/Customers(1)", UriKind.Relative)).Single();
                        PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(null, null);

                        context.SetSaveStream(c, "Thumbnail", new MemoryStream(), true, new DataServiceRequestArgs() { ContentType = "image/bmp" });
                        context.UpdateObject(c);

                        int i = 0;
                        PlaybackService.InspectRequestPayload.Value = (message) =>
                        {
                            if (i == 1)
                            {
                                // Verify that the second request was a PUT request
                                Assert.IsTrue(PlaybackService.LastPlayback.Contains(String.Format("PUT {0}/Customers(1)/EditLink/Thumbnail", request.ServiceRoot)), "the second request must be a PUT request to named stream");
                                Assert.IsTrue(!PlaybackService.LastPlayback.Contains(String.Format("If-Match", "no if-match header must be sent if the named stream does not have etag")));

                                // Verify the payload of the first request - it must have an entry element.
                                XDocument document = XDocument.Load(message);
                                Assert.IsTrue(document.Element(AstoriaUnitTests.Tests.UnitTestsUtil.AtomNamespace + "entry") != null, "must contain an entry element");
                            }

                            i++;
                        };

                        DataServiceContextTestUtil.SaveChanges(context, SaveChangesOptions.ContinueOnError, mode);
                        Assert.AreEqual(i, 2, "Only 2 request should have been made");

                        // Verify the first request was a PUT request to the entity
                        Assert.IsTrue(PlaybackService.LastPlayback.Contains(String.Format("PATCH {0}/editLink/Customers(1)", request.ServiceRoot)), "the first request must be a PUT request to the entity");
                    }
                });
        }

        [TestMethod]
        public void FailureOnInsertEntity()
        {
            // Make sure if the POST fails, then also named stream request also fails, since we don't have the edit link for the named stream
            TestUtil.RunCombinations(
                (IEnumerable<SaveChangesMode>)Enum.GetValues(typeof(SaveChangesMode)), UnitTestsUtil.BooleanValues,
                (mode, continueOnError) =>
                {
                    using (PlaybackService.OverridingPlayback.Restore())
                    using (PlaybackService.InspectRequestPayload.Restore())
                    {
                        DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                        PlaybackService.OverridingPlayback.Value = null;

                        Customer c = new Customer() { ID = 1, Name = "Foo" };
                        context.AddObject("Customers", c);
                        context.SetSaveStream(c, "Thumbnail", new MemoryStream(new byte[] { 0, 1, 2, 3 }), true, new DataServiceRequestArgs() { ContentType = "image/bmp" });

                        EntityDescriptor entityDescriptor = context.Entities.Single();
                        StreamDescriptor streamInfo = entityDescriptor.StreamDescriptors.Single();

                        int i = 0;
                        PlaybackService.InspectRequestPayload.Value = (message) =>
                        {
                            if (i == 1)
                            {
                                // Verify the first request was a POST request
                                Assert.IsTrue(PlaybackService.LastPlayback.Contains(String.Format("POST {0}/Customers", request.ServiceRoot)), "the first request must be a POST request");
                            }

                            i++;
                        };

                        try
                        {
                            DataServiceContextTestUtil.SaveChanges(context, continueOnError ? SaveChangesOptions.ContinueOnError : SaveChangesOptions.None, mode);
                            Assert.Fail("SaveChanges should always fail");
                        }
                        catch (DataServiceRequestException ex)
                        {
                            // here the server throws instream error (bad xml), so we'll always continue on error, even if it's not set..
                            Assert.AreEqual(2, ((IEnumerable<OperationResponse>)ex.Response).Count(), "we are expected 2 response for this SaveChanges operation");
                            Assert.IsTrue(ex.Response.Where<OperationResponse>(or => or.Error == null).SingleOrDefault() == null, "all responses should have errors");
                            Assert.AreEqual(streamInfo.State, EntityStates.Modified, "streamdescriptor should still be in modified state, so that next save changes can try this");
                            Assert.AreEqual(entityDescriptor.State, EntityStates.Added, "entityDescriptor is in the added state");
                        }

                        Assert.AreEqual(i, 1, "the number of requests sent to the server will only be 1, since the named stream request fails on the client side");

                        // Retrying again will always fail, since we have closed the stream.
                    }
                });
        }

        [TestMethod]
        public void FailureOnModifyEntity()
        {
            // Make sure that if the update fails, the state of the stream still remains modified and then next savechanges succeeds
            TestUtil.RunCombinations(
                (IEnumerable<SaveChangesMode>)Enum.GetValues(typeof(SaveChangesMode)),
                (mode) =>
                {
                    using (PlaybackService.OverridingPlayback.Restore())
                    using (PlaybackService.InspectRequestPayload.Restore())
                    {
                        DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                        context.EnableAtom = true;

                        // Populate the context with an entity
                        string payload = AtomParserTests.AnyEntry(
                                        id: Id,
                                        editLink: request.ServiceRoot.AbsoluteUri + "/editLink/Customers(1)",
                                        properties: Properties,
                                        links: GetNamedStreamSelfLink(request.ServiceRoot + "/Customers(1)/SelfLink/Thumbnail", contentType: MediaContentType) +
                                        GetNamedStreamEditLink(request.ServiceRoot.AbsoluteUri + "/Customers(1)/EditLink/Thumbnail", contentType: MediaContentType, etag: MediaETag));
                        PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(null, payload);
                        Customer c = context.Execute<Customer>(new Uri("/Customers(1)", UriKind.Relative)).Single();

                        PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(null, null, statusCode: System.Net.HttpStatusCode.BadRequest);
                        context.SetSaveStream(c, "Thumbnail", new MemoryStream(new byte[] { 0, 1, 2, 3 }), true, new DataServiceRequestArgs() { ContentType = "image/bmp" });

                        EntityDescriptor entityDescriptor = context.Entities.Single();
                        StreamDescriptor streamInfo = entityDescriptor.StreamDescriptors.Single();

                        try
                        {
                            DataServiceContextTestUtil.SaveChanges(context, SaveChangesOptions.None, mode);
                            Assert.Fail("SaveChanges should always fail");
                        }
                        catch (DataServiceRequestException ex)
                        {
                            Assert.AreEqual(1, ((IEnumerable<OperationResponse>)ex.Response).Count(), "we are expected 2 response for this SaveChanges operation");
                            Assert.IsTrue(ex.Response.Where<OperationResponse>(or => or.Error == null).SingleOrDefault() == null, "all responses should have errors");
                            Assert.AreEqual(streamInfo.State, EntityStates.Modified, "streamdescriptor should still be in modified state, so that next save changes can try this");
                        }

                        // Retrying will always fail, since the client closes the underlying stream.
                    }
                });
        }

        [TestMethod]
        public void EmptyOrMissingHRefAttributeValue()
        {
            // Missing href attribute or empty href attribute on named stream link elements
            TestUtil.RunCombinations(
                UnitTestsUtil.BooleanValues, // empty or null href attribute value
                UnitTestsUtil.BooleanValues, // edit or self link
                   (editLink, emptyAttributeValue) =>
                   {
                       string url = emptyAttributeValue ? "" : null;
                       string link = editLink ? GetNamedStreamEditLink(url, MediaContentType) : GetNamedStreamSelfLinkWithNull(url, MediaContentType);

                       // Populate the context with a single customer instance
                       string payload = AtomParserTests.AnyEntry(
                           id: Id,
                           properties: Properties,
                           links: link);

                       using (PlaybackService.OverridingPlayback.Restore())
                       {
                           PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(null, payload);
                           DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                           context.EnableAtom = true;
                           DataServiceQuery<Customer> q = (DataServiceQuery<Customer>)context.CreateQuery<Customer>("Customers").Where(c1 => c1.ID == 1);

                           ((IEnumerable<Customer>)DataServiceContextTestUtil.ExecuteQuery(context, q, QueryMode.AsyncExecute)).Single();
                           StreamDescriptor descriptor = context.Entities.First().StreamDescriptors.First();

                           Uri actual = editLink ? descriptor.StreamLink.EditLink : descriptor.StreamLink.SelfLink;

                           if (emptyAttributeValue)
                           {
                               Assert.IsNotNull(actual, "URI should not be null");
                           }
                           else
                           {
                               Assert.IsNull(actual, "URI should be null");
                           }
                       }
                   });
        }

        [TestMethod]
        public void GetReadStreamUri()
        {
            // passing invalid name should not throw an exception, instead should return null
            // Populate the context with a single customer instance
            string payload = AtomParserTests.AnyEntry(
                id: Id,
                properties: Properties);

            using (PlaybackService.OverridingPlayback.Restore())
            {
                PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(null, payload);
                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                context.EnableAtom = true;
                DataServiceQuery<Customer> q = (DataServiceQuery<Customer>)context.CreateQuery<Customer>("Customers").Where(c1 => c1.ID == 1);
                Customer c = ((IEnumerable<Customer>)DataServiceContextTestUtil.ExecuteQuery(context, q, QueryMode.AsyncExecute)).Single();

                // ask the uri for a random named stream
                Assert.IsNull(context.GetReadStreamUri(c, "random"), "DataServiceContext.GetReadStreamUri should not throw if the stream with the given name is not present");
            }
        }

        [TestMethod]
        public void UpdateNamedStreamOnDeletedEntity()
        {
            // Calling SetSaveStream on deleted entity is not supported
            // Populate the context with a single customer instance
            string payload = AtomParserTests.AnyEntry(
                id: Id,
                properties: Properties,
                links: GetNamedStreamSelfLink(request.ServiceRoot + "/Customers(1)/SelfLink/Thumbnail", contentType: MediaContentType));

            using (PlaybackService.OverridingPlayback.Restore())
            {
                PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(null, payload);
                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                context.EnableAtom = true;
                DataServiceQuery<Customer> q = (DataServiceQuery<Customer>)context.CreateQuery<Customer>("Customers").Where(c1 => c1.ID == 1);
                Customer c = ((IEnumerable<Customer>)DataServiceContextTestUtil.ExecuteQuery(context, q, QueryMode.AsyncExecute)).Single();

                context.DeleteObject(c);

                try
                {
                    context.SetSaveStream(c, "Thumbnail", new MemoryStream(), true /*closeStream*/, "image/jpeg");
                }
                catch (DataServiceClientException ex)
                {
                    Assert.AreEqual(ex.Message, DataServicesClientResourceUtil.GetString("Context_SetSaveStreamOnInvalidEntityState", EntityStates.Deleted), "Error Message not as expected");
                }
            }
        }

        [TestMethod]
        public void VerifyEntityDescriptorMergeFunctionality()
        {
            // Make sure that based on the MergeOption, the right links are exposed in the entity descriptor
            TestUtil.RunCombinations(
                    new EntityStates[] { EntityStates.Added, EntityStates.Deleted, EntityStates.Modified, EntityStates.Unchanged },
                    new MergeOption[] { MergeOption.AppendOnly, MergeOption.OverwriteChanges, MergeOption.PreserveChanges },
                    new int[] { -1, 0, 1 }, // -1 indicates less links, 0 means exact same number of links, 1 means some extra links
                    UnitTestsUtil.BooleanValues,
                    UnitTestsUtil.BooleanValues,
                    (entityState, mergeOption, extraLinks, useBatchMode, returnETag) =>
                    {
                        using (PlaybackService.OverridingPlayback.Restore())
                        {
                            DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                            context.EnableAtom = true;
                            context.MergeOption = mergeOption;

                            // Populate the context with a single customer instance with 2 named streams
                            string originalServiceRoot = "http://randomservice/Foo.svc";
                            string newServiceRoot = "http://randomservice1/Foo1.svc";

                            string payload = AtomParserTests.AnyEntry(
                                id: Id,
                                editLink: request.ServiceRoot.AbsoluteUri + "/editLink/Customers(1)",
                                properties: Properties,
                                links: GetNamedStreamSelfLink(originalServiceRoot + "/Customers(1)/SelfLink/Thumbnail", contentType: MediaContentType) +
                                       GetNamedStreamEditLink(originalServiceRoot + "/Customers(1)/EditLink/Thumbnail", contentType: MediaContentType, etag: MediaETag) +
                                       GetNamedStreamSelfLink(originalServiceRoot + "/Customers(1)/SelfLink/Photo", contentType: MediaContentType, name: "Photo") +
                                       GetNamedStreamEditLink(originalServiceRoot + "/Customers(1)/EditLink/Photo", contentType: MediaContentType, name: "Photo"));

                            PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(null, payload);
                            Customer c = DataServiceContextTestUtil.CreateEntity<Customer>(context, "Customers", entityState);
                            PlaybackService.OverridingPlayback.Value = null;

                            string linksPayload = null;
                            string newETag = returnETag ? MediaETag + 1 : null;

                            if (extraLinks == -1)
                            {
                                linksPayload = GetNamedStreamSelfLink(newServiceRoot + "/Customers(1)/SelfLink/Thumbnail", contentType: MediaContentType) +
                                               GetNamedStreamEditLink(newServiceRoot + "/Customers(1)/EditLink/Thumbnail", contentType: MediaContentType, etag: newETag);
                            }
                            else if (extraLinks == 0)
                            {
                                linksPayload = GetNamedStreamSelfLink(newServiceRoot + "/Customers(1)/SelfLink/Thumbnail", contentType: MediaContentType) +
                                               GetNamedStreamEditLink(newServiceRoot + "/Customers(1)/EditLink/Thumbnail", contentType: MediaContentType, etag: newETag) +
                                               GetNamedStreamSelfLink(newServiceRoot + "/Customers(1)/SelfLink/Photo", contentType: MediaContentType, name: "Photo") +
                                               GetNamedStreamEditLink(newServiceRoot + "/Customers(1)/EditLink/Photo", contentType: MediaContentType, name: "Photo", etag: newETag);
                            }
                            else
                            {
                                linksPayload = GetNamedStreamSelfLink(newServiceRoot + "/Customers(1)/SelfLink/Thumbnail", contentType: MediaContentType) +
                                               GetNamedStreamEditLink(newServiceRoot + "/Customers(1)/EditLink/Thumbnail", contentType: MediaContentType, etag: newETag) +
                                               GetNamedStreamSelfLink(newServiceRoot + "/Customers(1)/SelfLink/Photo", contentType: MediaContentType, name: "Photo") +
                                               GetNamedStreamEditLink(newServiceRoot + "/Customers(1)/EditLink/Photo", contentType: MediaContentType, etag: newETag, name: "Photo") +
                                               GetNamedStreamSelfLink(newServiceRoot + "/Customers(1)/SelfLink/HighResolutionPhoto", contentType: MediaContentType, name: "HighResolutionPhoto") +
                                               GetNamedStreamEditLink(newServiceRoot + "/Customers(1)/EditLink/HighResolutionPhoto", contentType: MediaContentType, name: "HighResolutionPhoto", etag: newETag);
                            }

                            payload = AtomParserTests.AnyEntry(
                                id: Id,
                                editLink: request.ServiceRoot.AbsoluteUri + "/editLink/Customers(1)",
                                properties: Properties,
                                links: linksPayload);

                            PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(null, payload);

                            if (useBatchMode)
                            {
                                PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToBatchQueryResponsePayload(PlaybackService.OverridingPlayback.Value);
                                QueryOperationResponse<Customer> resp = (QueryOperationResponse<Customer>)context.ExecuteBatch(context.CreateQuery<Customer>("/Customers(123)")).Single();
                                c = resp.First();
                            }
                            else
                            {
                                c = context.CreateQuery<Customer>("/Customers(123)").Execute().First();
                            }

                            EntityDescriptor entityDescriptor = context.Entities.Where(ed => Object.ReferenceEquals(c, ed.Entity)).Single();
                            StreamDescriptor thumbnail = entityDescriptor.StreamDescriptors.Where(ns => ns.StreamLink.Name == "Thumbnail").SingleOrDefault();
                            StreamDescriptor photo = entityDescriptor.StreamDescriptors.Where(ns => ns.StreamLink.Name == "Photo").SingleOrDefault();
                            StreamDescriptor highResPhoto = entityDescriptor.StreamDescriptors.Where(ns => ns.StreamLink.Name == "HighResolutionPhoto").SingleOrDefault();

                            string newSelfLink = newServiceRoot + "/Customers(1)/SelfLink/{0}";
                            string newEditLink = newServiceRoot + "/Customers(1)/EditLink/{0}";
                            string existingSelfLink = originalServiceRoot + "/Customers(1)/SelfLink/{0}";
                            string existingEditLink = originalServiceRoot + "/Customers(1)/EditLink/{0}";

                            if (entityState == EntityStates.Added)
                            {
                                Assert.AreEqual(2, context.Entities.Count, "since its in added state, we will get a new entity descriptor");
                                Assert.AreEqual(2 + extraLinks, entityDescriptor.StreamDescriptors.Count, "number of named streams was not as expected");

                                VerifyNamedStreamInfo(thumbnail, newSelfLink, newEditLink, newETag, contentType: MediaContentType);
                                if (extraLinks == -1)
                                {
                                    Assert.AreEqual(photo, null, "photo must be null when extra links = -1 and state = added");
                                    Assert.AreEqual(highResPhoto, null, "highResPhoto must be null when extra links = -1 and state = added");
                                }
                                else if (extraLinks == 0)
                                {
                                    VerifyNamedStreamInfo(photo, newSelfLink, newEditLink, newETag, contentType: MediaContentType);
                                    Assert.AreEqual(highResPhoto, null, "highResPhoto must be null when extra links = 0 and state = added");
                                }
                                else if (extraLinks == 1)
                                {
                                    VerifyNamedStreamInfo(photo, newSelfLink, newEditLink, newETag, contentType: MediaContentType);
                                    VerifyNamedStreamInfo(highResPhoto, newSelfLink, newEditLink, newETag, contentType: MediaContentType);
                                }
                            }
                            else if (mergeOption == MergeOption.OverwriteChanges ||
                                     (mergeOption == MergeOption.PreserveChanges && (entityState == EntityStates.Deleted || entityState == EntityStates.Unchanged)))
                            {
                                Assert.AreEqual(1, context.Entities.Count, "since its not in added state, we will only have one entity descriptor");
                                int numOfNamedStreams = 2;
                                if (extraLinks == 1)
                                {
                                    numOfNamedStreams = 3;
                                }
                                Assert.AreEqual(numOfNamedStreams, entityDescriptor.StreamDescriptors.Count, "number of named streams was not as expected");

                                VerifyNamedStreamInfo(thumbnail, newSelfLink, newEditLink, newETag ?? MediaETag, MediaContentType);
                                if (extraLinks == -1)
                                {
                                    VerifyNamedStreamInfo(photo, existingSelfLink, existingEditLink, null, MediaContentType);
                                    Assert.AreEqual(highResPhoto, null, "highResPhoto must be null when extra links = -1 and state = added");
                                }
                                else if (extraLinks == 0)
                                {
                                    VerifyNamedStreamInfo(photo, newSelfLink, newEditLink, newETag, MediaContentType);
                                    Assert.AreEqual(highResPhoto, null, "highResPhoto must be null when extra links = 0 and state = added");
                                }
                                else if (extraLinks == 1)
                                {
                                    VerifyNamedStreamInfo(photo, newSelfLink, newEditLink, newETag, MediaContentType);
                                    VerifyNamedStreamInfo(highResPhoto, newSelfLink, newEditLink, newETag, MediaContentType);
                                }
                            }
                            else
                            {
                                // no change should be made
                                Assert.AreEqual(1, context.Entities.Count, "since its not in added state, we will only have one entity descriptor");
                                Assert.AreEqual(2, entityDescriptor.StreamDescriptors.Count, "number of named streams was not as expected");
                                VerifyNamedStreamInfo(thumbnail, existingSelfLink, existingEditLink, MediaETag, MediaContentType);
                                VerifyNamedStreamInfo(photo, existingSelfLink, existingEditLink, null, MediaContentType);
                                Assert.AreEqual(highResPhoto, null, "highResPhoto must be null in AppendOnly");
                            }
                        }
                    });
        }

        [TestMethod]
        public void VerifyMissingLinkSaveChangesScenario()
        {
            // Make sure update scenarios fail, whenever edit link is missing
            TestUtil.RunCombinations(
                (IEnumerable<SaveChangesMode>)Enum.GetValues(typeof(SaveChangesMode)),
                UnitTestsUtil.BooleanValues,
                (mode, hasSelfLink) =>
                {
                    using (PlaybackService.OverridingPlayback.Restore())
                    {
                        DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                        context.EnableAtom = true;

                        string links = null;
                        string selfLink = null;
                        string contentType = null;
                        if (hasSelfLink)
                        {
                            selfLink = request.ServiceRoot + "/Customers(1)/SelfLink/Thumbnail";
                            contentType = MediaContentType;
                            links = GetNamedStreamSelfLink(selfLink, contentType);
                        }

                        string payload = AtomParserTests.AnyEntry(
                                    id: Id,
                                    selfLink: request.ServiceRoot.AbsoluteUri + "/selfLink/Customers(1)",
                                    editLink: request.ServiceRoot.AbsoluteUri + "/editLink/Customers(1)",
                                    properties: Properties,
                                    links: links);

                        PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(null, payload);
                        Customer c = context.Execute<Customer>(new Uri("/Customers(1)", UriKind.Relative)).Single();
                        PlaybackService.OverridingPlayback.Value = null;

                        EntityDescriptor ed = context.Entities.Single();
                        if (hasSelfLink)
                        {
                            StreamDescriptor streamInfo = ed.StreamDescriptors.Single();
                            Assert.AreEqual(streamInfo.StreamLink.SelfLink.AbsoluteUri, selfLink, "self links must match");
                            Assert.AreEqual(streamInfo.StreamLink.EditLink, null, "edit link must be null");
                            Assert.AreEqual(streamInfo.StreamLink.ContentType, contentType, "content type must match");
                            Assert.AreEqual(streamInfo.StreamLink.ETag, null, "no etag");
                        }
                        else
                        {
                            Assert.AreEqual(0, ed.StreamDescriptors.Count, "No named streams should be present");
                        }

                        context.SetSaveStream(c, "Thumbnail", new MemoryStream(new byte[] { 0, 1, 2, 3, 4 }), true, new DataServiceRequestArgs() { ContentType = "image/jpeg" });

                        StreamDescriptor ns = ed.StreamDescriptors.Single();
                        Assert.AreEqual(ns.StreamLink.SelfLink, selfLink, "self links must match");
                        Assert.AreEqual(ns.StreamLink.EditLink, null, "edit link must be null");
                        Assert.AreEqual(ns.StreamLink.ContentType, contentType, "content type must match");
                        Assert.AreEqual(ns.StreamLink.ETag, null, "no etag");
                        Assert.AreEqual(EntityStates.Modified, ns.State, "named stream must be in modified state");

                        try
                        {
                            DataServiceContextTestUtil.SaveChanges(context, SaveChangesOptions.None, mode);
                        }
                        catch (DataServiceRequestException ex)
                        {
                            Assert.AreEqual(typeof(DataServiceClientException), ex.InnerException.GetType());
                            Assert.AreEqual(DataServicesClientResourceUtil.GetString("Context_SetSaveStreamWithoutNamedStreamEditLink", "Thumbnail"), ex.InnerException.Message);
                        }

                        Assert.AreEqual(EntityStates.Modified, ns.State, "Since save changes failed, the stream should still be in modified state");
                    }
                });
        }

        [TestMethod]
        public void VerifyMissingLinkQueryScenario()
        {
            // Make sure the query scenarios fail, when self/edit links for named stream is missing
            TestUtil.RunCombinations(
                UnitTestsUtil.BooleanValues,
                UnitTestsUtil.BooleanValues,
                UnitTestsUtil.BooleanValues,
                (syncRead, hasSelfLink, hasEditLink) =>
                {
                    using (PlaybackService.OverridingPlayback.Restore())
                    {
                        DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                        context.EnableAtom = true;

                        string links = null;
                        string contentType = null;
                        string selfLink = null;
                        string editLink = null;

                        if (hasSelfLink)
                        {
                            selfLink = request.ServiceRoot + "/Customers(1)/SelfLink/Thumbnail";
                            links += GetNamedStreamSelfLink(selfLink);
                        }

                        if (hasEditLink)
                        {
                            editLink = request.ServiceRoot + "/Customers(1)/EditLink/Thumbnail";
                            contentType = MediaContentType;
                            links += GetNamedStreamEditLink(editLink, contentType);
                        }

                        string payload = AtomParserTests.AnyEntry(
                                     id: Id,
                                     selfLink: request.ServiceRoot.AbsoluteUri + "/selfLink/Customers(1)",
                                     editLink: request.ServiceRoot.AbsoluteUri + "/editLink/Customers(1)",
                                     properties: Properties,
                                     links: links);

                        PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(null, payload);
                        Customer c = context.Execute<Customer>(new Uri("/Customers(1)", UriKind.Relative)).Single();
                        PlaybackService.OverridingPlayback.Value = null;

                        EntityDescriptor ed = context.Entities.Single();
                        StreamDescriptor ns = null;
                        bool expectException = false;

                        if (!hasEditLink && !hasSelfLink)
                        {
                            expectException = true;
                            Assert.AreEqual(0, ed.StreamDescriptors.Count, "No named streams should be present");
                        }
                        else
                        {
                            ns = ed.StreamDescriptors.Single();
                            Assert.AreEqual(ns.StreamLink.SelfLink, selfLink, "self link must match");
                            Assert.AreEqual(ns.StreamLink.EditLink, editLink, "edit link must match");
                            Assert.AreEqual(ns.StreamLink.ContentType, contentType, "content type must match");
                            Assert.AreEqual(context.GetReadStreamUri(c, ns.StreamLink.Name).AbsoluteUri, ns.StreamLink.SelfLink != null ? ns.StreamLink.SelfLink.AbsoluteUri : ns.StreamLink.EditLink.AbsoluteUri,
                                "Make sure that context.GetReadStreamUri returns the self link if present, otherwise returns edit link");
                        }

                        try
                        {
                            if (syncRead)
                            {
                                context.GetReadStream(c, "Thumbnail", new DataServiceRequestArgs() { ContentType = "image/jpeg" });
                            }
                            else
                            {
                                IAsyncResult result = context.BeginGetReadStream(c, "Thumbnail", new DataServiceRequestArgs() { ContentType = "image/jpeg" }, (r) =>
                                    {
                                        context.EndGetReadStream(r);
                                    },
                                    null);

                                if (!result.CompletedSynchronously)
                                {
                                    Assert.IsTrue(result.AsyncWaitHandle.WaitOne(new TimeSpan(0, 0, AstoriaUnitTests.TestConstants.MaxTestTimeout), false), "BeginExecute timeout");
                                }
                            }

                            Assert.IsTrue(!expectException, "should reach here when no exception is expected");
                            Assert.IsTrue(PlaybackService.LastPlayback.Contains("GET " + selfLink ?? editLink), "should use self link if present, otherwise editlink");
                        }
                        catch (ArgumentException ex)
                        {
                            Assert.IsTrue(expectException, "should get exception when expected");
                            ArgumentException expectedException = new ArgumentException(DataServicesClientResourceUtil.GetString("Context_EntityDoesNotContainNamedStream", "Thumbnail"), "name");
                            Assert.AreEqual(expectedException.Message, ex.Message, "Error message did not match");
                            Assert.AreEqual(0, ed.StreamDescriptors.Count, "No named streams should be present");
                        }
                    }
                });
        }

        [TestMethod]
        public void NamedStreams_ProjectionMissingStream()
        {
            // making sure some of the invalid linq queries fail
            using (PlaybackService.OverridingPlayback.Restore())
            {
                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                context.EnableAtom = true;
                context.Format.UseAtom();
                // Populate the context with a single customer instance with 2 named streams
                string originalServiceRoot = "http://randomservice/Foo.svc";

                string payload = AtomParserTests.AnyEntry(
                    id: NamedStreamTests.Id,
                    editLink: request.ServiceRoot.AbsoluteUri + "/editLink/Customers(1)",
                    properties: NamedStreamTests.Properties,
                    links: NamedStreamTests.GetNamedStreamEditLink(originalServiceRoot + "/Customers(1)/EditLink/Thumbnail"));

                PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(null, payload);
                var q = context.CreateQuery<StreamType2>("Customers").Select(s => new StreamType2()
                            {
                                ID = s.ID,
                                Name = s.Name,
                                Thumbnail = s.Thumbnail,
                                Photo = s.Photo // selecting named stream, but not present in payload
                            });

                Exception ex = TestUtil.RunCatching(() => { q.FirstOrDefault(); });
                Assert.IsNotNull(ex);
                Assert.AreEqual(DataServicesClientResourceUtil.GetString("AtomMaterializer_PropertyMissing", "Photo"), ex.Message);
            }
        }

        [TestMethod]
        public void NamedStreams_Projections_MergeInfoOptions()
        {
            // Make sure that based on the MergeOption, the value of the DataServiceStreamLink is updated
            TestUtil.RunCombinations(
                    new EntityStates[] { EntityStates.Added, EntityStates.Deleted, EntityStates.Modified, EntityStates.Unchanged },
                    new MergeOption[] { MergeOption.AppendOnly, MergeOption.OverwriteChanges, MergeOption.PreserveChanges },
                    new int[] { -1, 0, 1 }, // -1 indicates less links, 0 means exact same number of links, 1 means some extra links
                    UnitTestsUtil.BooleanValues,
                    (entityState, mergeOption, extraLinks, useBatchMode) =>
                    {
                        using (PlaybackService.OverridingPlayback.Restore())
                        {
                            DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                            context.EnableAtom = true;
                            context.Format.UseAtom();
                            context.MergeOption = mergeOption;

                            // Populate the context with a single customer instance with 2 named streams
                            string originalServiceRoot = "http://randomservice/Foo.svc";
                            string newServiceRoot = "http://randomservice1/Foo1.svc";

                            string payload = AtomParserTests.AnyEntry(
                                id: NamedStreamTests.Id,
                                editLink: request.ServiceRoot.AbsoluteUri + "/editLink/Customers(1)",
                                properties: NamedStreamTests.Properties,
                                links: NamedStreamTests.GetNamedStreamEditLink(originalServiceRoot + "/Customers(1)/EditLink/Thumbnail"));

                            PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(null, payload);
                            DataServiceQuery<StreamType2> query = (DataServiceQuery<StreamType2>)context.CreateQuery<StreamType2>("Customers");

                            StreamType2 c = DataServiceContextTestUtil.CreateEntity<StreamType2>(context, "Customers", entityState, query);
                            PlaybackService.OverridingPlayback.Value = null;

                            string linksPayload = null;

                            if (extraLinks == -1)
                            {
                                // send no links
                            }
                            else if (extraLinks == 0)
                            {
                                linksPayload = NamedStreamTests.GetNamedStreamEditLink(newServiceRoot + "/Customers(1)/EditLink/Thumbnail");
                            }
                            else
                            {
                                linksPayload = NamedStreamTests.GetNamedStreamEditLink(newServiceRoot + "/Customers(1)/EditLink/Thumbnail") +
                                               NamedStreamTests.GetNamedStreamEditLink(newServiceRoot + "/Customers(1)/EditLink/Photo", name: "Photo");
                            }

                            payload = AtomParserTests.AnyEntry(
                                id: NamedStreamTests.Id,
                                editLink: request.ServiceRoot.AbsoluteUri + "/editLink/Customers(1)",
                                properties: NamedStreamTests.Properties,
                                links: linksPayload);

                            PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(null, payload);

                            if (useBatchMode)
                            {
                                PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToBatchQueryResponsePayload(PlaybackService.OverridingPlayback.Value);
                                QueryOperationResponse<StreamType2> resp = (QueryOperationResponse<StreamType2>)context.ExecuteBatch((DataServiceRequest)query).Single();
                                c = resp.First();
                            }
                            else
                            {
                                switch (extraLinks)
                                {
                                    case -1:
                                        c = query.Select(s => new StreamType2()
                                        {
                                            ID = s.ID,
                                            Name = s.Name
                                        }).Single();
                                        break;
                                    case 0:
                                        c = query.Select(s => new StreamType2()
                                        {
                                            ID = s.ID,
                                            Name = s.Name,
                                            Thumbnail = s.Thumbnail
                                        }).Single();
                                        break;
                                    default:
                                        c = query.Select(s => new StreamType2()
                                        {
                                            ID = s.ID,
                                            Name = s.Name,
                                            Thumbnail = s.Thumbnail,
                                            Photo = s.Photo
                                        }).Single();
                                        break;
                                }
                            }

                            EntityDescriptor entityDescriptor = context.Entities.Where(ed => Object.ReferenceEquals(c, ed.Entity)).Single();
                            StreamDescriptor thumbnail = entityDescriptor.StreamDescriptors.Where(ns => ns.StreamLink.Name == "Thumbnail").SingleOrDefault();
                            //Assert.IsTrue(thumbnail == null || object.ReferenceEquals(thumbnail.EntityDescriptor, entityDescriptor), "StreamDescriptor.EntityDescriptor should point to the same instance of entity descriptor");
                            StreamDescriptor photo = entityDescriptor.StreamDescriptors.Where(ns => ns.StreamLink.Name == "Photo").SingleOrDefault();
                            //Assert.IsTrue(photo == null || object.ReferenceEquals(photo.EntityDescriptor, entityDescriptor), "StreamDescriptor.EntityDescriptor should point to the same instance of entity descriptor for photo");

                            string newEditLink = newServiceRoot + "/Customers(1)/EditLink/{0}";
                            string existingEditLink = originalServiceRoot + "/Customers(1)/EditLink/{0}";

                            if (entityState == EntityStates.Added)
                            {
                                Assert.AreEqual(2, context.Entities.Count, "since its in added state, we will get a new entity descriptor");
                                Assert.AreEqual(1 + extraLinks, entityDescriptor.StreamDescriptors.Count, "number of named streams was not as expected");

                                if (extraLinks == -1)
                                {
                                    Assert.AreEqual(thumbnail, null, "photo must be null when extra links = -1 and state = added");
                                }
                                else if (extraLinks == 0)
                                {
                                    NamedStreamTests.VerifyNamedStreamInfo(thumbnail, null, newEditLink, null, null);
                                }
                                else if (extraLinks == 1)
                                {
                                    NamedStreamTests.VerifyNamedStreamInfo(thumbnail, null, newEditLink, null, null);
                                    NamedStreamTests.VerifyNamedStreamInfo(photo, null, newEditLink, null, null);
                                }
                            }
                            else if (mergeOption == MergeOption.OverwriteChanges ||
                                     (mergeOption == MergeOption.PreserveChanges && (entityState == EntityStates.Deleted || entityState == EntityStates.Unchanged)))
                            {
                                Assert.AreEqual(1, context.Entities.Count, "since its not in added state, we will only have one entity descriptor");
                                int numOfNamedStreams = 1;
                                if (extraLinks == 1)
                                {
                                    numOfNamedStreams = 2;
                                }
                                Assert.AreEqual(numOfNamedStreams, entityDescriptor.StreamDescriptors.Count, "number of named streams was not as expected");

                                if (extraLinks == -1)
                                {
                                    NamedStreamTests.VerifyNamedStreamInfo(thumbnail, null, existingEditLink, null, null);
                                    Assert.AreEqual(photo, null, "photo must be null when extra links = -1");
                                }
                                else if (extraLinks == 0)
                                {
                                    NamedStreamTests.VerifyNamedStreamInfo(thumbnail, null, newEditLink, null, null);
                                    Assert.AreEqual(photo, null, "photo must be null when extra links = 0");
                                }
                                else if (extraLinks == 1)
                                {
                                    NamedStreamTests.VerifyNamedStreamInfo(thumbnail, null, newEditLink, null, null);
                                    NamedStreamTests.VerifyNamedStreamInfo(photo, null, newEditLink, null, null);
                                }
                            }
                            else
                            {
                                // no change should be made
                                Assert.AreEqual(1, context.Entities.Count, "since its not in added state, we will only have one entity descriptor");
                                Assert.AreEqual(1, entityDescriptor.StreamDescriptors.Count, "number of named streams was not as expected");
                                NamedStreamTests.VerifyNamedStreamInfo(thumbnail, null, existingEditLink, null, null);
                                Assert.AreEqual(photo, null, "photo must be null when extra links = 0");
                            }
                        }
                    });
        }

        [TestMethod]
        public void NamedStreams_PropertyOnClientNotDataServiceStreamLink()
        {
            // [Client-ODataLib-Integration] Astoria client does not fail if the client and server stream property does not match
            TestUtil.RunCombinations(
                    new MergeOption[] { MergeOption.AppendOnly, MergeOption.OverwriteChanges, MergeOption.PreserveChanges },
                    UnitTestsUtil.BooleanValues,
                    (mergeOption, useBatchMode) =>
                    {
                        using (PlaybackService.OverridingPlayback.Restore())
                        {
                            DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                            context.EnableAtom = true;
                            context.MergeOption = mergeOption;

                            // Populate the context with a single customer instance with 2 named streams
                            string originalServiceRoot = "http://randomservice/Foo.svc";

                            string payload = AtomParserTests.AnyEntry(
                                id: NamedStreamTests.Id,
                                editLink: request.ServiceRoot.AbsoluteUri + "/editLink/Customers(1)",
                                properties: NamedStreamTests.Properties,
                                links: NamedStreamTests.GetNamedStreamEditLink(originalServiceRoot + "/Customers(1)/EditLink/Thumbnail"));

                            PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(null, payload);
                            DataServiceQuery<StreamTypeWithMismatchedStreamProperty> query = (DataServiceQuery<StreamTypeWithMismatchedStreamProperty>)context.CreateQuery<StreamTypeWithMismatchedStreamProperty>("Customers");

                            Exception exception = null;
                            if (useBatchMode)
                            {
                                PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToBatchQueryResponsePayload(PlaybackService.OverridingPlayback.Value);
                                var batchResponse = context.ExecuteBatch(query);
                                foreach (var queryResponse in batchResponse.OfType<QueryOperationResponse<StreamTypeWithMismatchedStreamProperty>>())
                                {
                                    exception = TestUtil.RunCatching<InvalidOperationException>(() =>
                                    {
                                        foreach (var entity in queryResponse)
                                        {
                                            Assert.Fail("Should not reach here");
                                        }
                                    });
                                }
                            }
                            else
                            {
                                exception = TestUtil.RunCatching<InvalidOperationException>(() => query.ToList());
                            }

                            Assert.IsNotNull(exception, "Expected error on mismatched client named stream link property type");
                            Assert.IsInstanceOfType(exception, typeof(InvalidOperationException), "Expected InvalidOperationException on mismatched client named stream link property type");
                            string expectedErrorMessage = ODataLibResourceUtil.GetString("ValidationUtils_MismatchPropertyKindForStreamProperty", "Thumbnail");
                            Assert.AreEqual(expectedErrorMessage, exception.Message);
                        }
                    });
        }

        [TestMethod]
        public void NamedStreams_TestPublicAPIParameters()
        {
            // Test public api parameters
            DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);

            #region GetReadStreamUri API
            DataServiceContextTestUtil.CheckArgumentNull("entity", () =>
            {
                context.GetReadStreamUri(null, "Thumbnail");
            });

            DataServiceContextTestUtil.CheckArgumentNull("name", () =>
            {
                context.GetReadStreamUri(new Customer(), null);
            });

            DataServiceContextTestUtil.CheckArgumentEmpty("name", () =>
            {
                context.GetReadStreamUri(new Customer(), "");
            });

            DataServiceContextTestUtil.VerifyInvalidRequest(typeof(InvalidOperationException), "Context_EntityNotContained", () =>
            {
                context.GetReadStreamUri(new Customer(), "Thumbnail");
            });
            #endregion GetReadStreamUri API

            #region BeginGetReadStream
            DataServiceContextTestUtil.CheckArgumentNull("entity", () =>
            {
                context.BeginGetReadStream(null, "Thumbnail", new DataServiceRequestArgs(), null, null);
            });

            DataServiceContextTestUtil.CheckArgumentNull("name", () =>
            {
                context.BeginGetReadStream(new Customer(), null, new DataServiceRequestArgs(), null, null);
            });

            DataServiceContextTestUtil.CheckArgumentEmpty("name", () =>
            {
                context.BeginGetReadStream(new Customer(), "", new DataServiceRequestArgs(), null, null);
            });

            DataServiceContextTestUtil.CheckArgumentNull("args", () =>
            {
                context.BeginGetReadStream(new Customer(), "Thumbnail", null, null, null);
            });

            DataServiceContextTestUtil.VerifyInvalidRequest(typeof(InvalidOperationException), "Context_EntityNotContained", () =>
            {
                context.BeginGetReadStream(new Customer(), "Thumbnail", new DataServiceRequestArgs(), null, null);
            });
            #endregion BeginGetReadStream

            #region GetReadStream
            DataServiceContextTestUtil.CheckArgumentNull("entity", () =>
            {
                context.GetReadStream(null, "Thumbnail", new DataServiceRequestArgs());
            });

            DataServiceContextTestUtil.CheckArgumentNull("name", () =>
            {
                context.GetReadStream(new Customer(), null, new DataServiceRequestArgs());
            });

            DataServiceContextTestUtil.CheckArgumentEmpty("name", () =>
            {
                context.GetReadStream(new Customer(), "", new DataServiceRequestArgs());
            });

            DataServiceContextTestUtil.CheckArgumentNull("args", () =>
            {
                context.GetReadStream(new Customer(), "Thumbnail", null);
            });

            DataServiceContextTestUtil.VerifyInvalidRequest(typeof(InvalidOperationException), "Context_EntityNotContained", () =>
            {
                context.GetReadStream(new Customer(), "Thumbnail", new DataServiceRequestArgs());
            });
            #endregion GetReadStream

            #region SetSaveStream
            DataServiceContextTestUtil.CheckArgumentNull("entity", () =>
            {
                context.SetSaveStream(null, "Thumbnail", new MemoryStream(), true, new DataServiceRequestArgs());
            });

            DataServiceContextTestUtil.CheckArgumentNull("name", () =>
            {
                context.SetSaveStream(new Customer(), null, new MemoryStream(), true, new DataServiceRequestArgs());
            });

            DataServiceContextTestUtil.CheckArgumentEmpty("name", () =>
            {
                context.SetSaveStream(new Customer(), "", new MemoryStream(), true, new DataServiceRequestArgs());
            });

            DataServiceContextTestUtil.CheckArgumentNull("stream", () =>
            {
                context.SetSaveStream(new Customer(), "Thumbnail", null, true, new DataServiceRequestArgs());
            });

            DataServiceContextTestUtil.CheckArgumentNull("args", () =>
            {
                context.SetSaveStream(new Customer(), "Thumbnail", new MemoryStream(), true, (DataServiceRequestArgs)null);
            });

            DataServiceContextTestUtil.CheckArgumentNull("contentType", () =>
            {
                context.SetSaveStream(new Customer(), "Thumbnail", new MemoryStream(), true, (string)null);
            });

            DataServiceContextTestUtil.CheckArgumentEmpty("contentType", () =>
            {
                context.SetSaveStream(new Customer(), "Thumbnail", new MemoryStream(), true, string.Empty);
            });

            DataServiceContextTestUtil.VerifyInvalidRequest(typeof(InvalidOperationException), "Context_EntityNotContained", () =>
            {
                context.SetSaveStream(new Customer(), "Thumbnail", new MemoryStream(), true, "image/jpeg");
            });

            DataServiceContextTestUtil.CheckArgumentException("Context_ContentTypeRequiredForNamedStream", () =>
            {
                context.SetSaveStream(new Customer(), "Thumbnail", new MemoryStream(), true, new DataServiceRequestArgs());
            }, "args");

            DataServiceContextTestUtil.CheckArgumentException("Context_ContentTypeRequiredForNamedStream", () =>
            {
                context.SetSaveStream(new Customer(), "Thumbnail", new MemoryStream(), true, new DataServiceRequestArgs() { ContentType = string.Empty });
            }, "args");

            DataServiceContextTestUtil.VerifyInvalidRequest(typeof(InvalidOperationException), "Context_EntityNotContained", () =>
            {
                context.SetSaveStream(new Customer(), "Thumbnail", new MemoryStream(), true, new DataServiceRequestArgs() { ContentType = "image/jpg" });
            });
            #endregion SetSaveStream
        }

        [TestMethod]
        public void NamedStreams_BindingTest()
        {
            // Making sure changing the DataServiceStreamLink properties raises the binding events
            using (PlaybackService.OverridingPlayback.Restore())
            using (PlaybackService.InspectRequestPayload.Restore())
            {
                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                context.EnableAtom = true;
                context.MergeOption = MergeOption.OverwriteChanges;

                // Populate the context with an entity
                string payload = AtomParserTests.AnyEntry(
                                id: Id,
                                editLink: request.ServiceRoot.AbsoluteUri + "/editLink/Customers(1)",
                                properties: Properties,
                                links: GetNamedStreamSelfLink(request.ServiceRoot + "/Customers(1)/SelfLink/Thumbnail", contentType: MediaContentType) +
                                GetNamedStreamEditLink(request.ServiceRoot.AbsoluteUri + "/Customers(1)/EditLink/Thumbnail", contentType: MediaContentType, etag: MediaETag));
                PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(null, payload);
                var collection = new DataServiceCollection<NamedStreamWithBinding>(context.CreateQuery<NamedStreamWithBinding>("Customers").Where(c1 => c1.ID == 1), TrackingMode.AutoChangeTracking);

                Assert.AreEqual(1, context.Entities.Count, "context should have one entity as tracking");
                NamedStreamWithBinding entity = collection[0];

                // modifying the property directly: nothing should happen...
                // need to use set save stream
                entity.Thumbnail = entity.Thumbnail;
                Assert.AreEqual(EntityStates.Unchanged, context.Entities[0].State, "the entity should be in unchanged state");

                int propertyChangedCallCount = 0;
                entity.PropertyChanged += (o, args) =>
                    {
                        propertyChangedCallCount++;
                    };

                entity.Thumbnail.PropertyChanged += (o, args) =>
                {
                    propertyChangedCallCount++;
                };

                payload = AtomParserTests.AnyEntry(
                                id: Id,
                                editLink: request.ServiceRoot.AbsoluteUri + "/editLink/Customers(1)",
                                properties: Properties,
                                links: GetNamedStreamSelfLink(request.ServiceRoot + "/Customers(10)/SelfLink/Thumbnail", contentType: MediaContentType) +
                                GetNamedStreamEditLink(request.ServiceRoot.AbsoluteUri + "/Customers(10)/EditLink/Thumbnail", contentType: MediaContentType, etag: MediaETag));
                PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(null, payload);
                context.CreateQuery<NamedStreamWithBinding>("Customers").Where(c1 => c1.ID == 1).Single();
                Assert.AreEqual(1, context.Entities.Count, "context should have one entity as tracking");
                Assert.AreEqual(request.ServiceRoot + "/Customers(10)/SelfLink/Thumbnail", entity.Thumbnail.SelfLink.AbsoluteUri);
                Assert.AreEqual(7, propertyChangedCallCount);
            }
        }

        [TestMethod]
        public void NamedStreams_DataServiceStreamLinkNotAllowedInFilterAndOrderBy()
        {
            // Making sure that DataServiceStreamLink is not allowed only in $filter and $orderby query options
            // Populate the context with a single customer instance
            string payload = AtomParserTests.AnyEntry(
                id: Id,
                properties: Properties,
                links: GetNamedStreamEditLink(MediaEditLink, contentType: MediaContentType, etag: MediaETag) + GetNamedStreamSelfLink(MediaQueryLink, contentType: MediaContentType));

            string nonbatchPayload = PlaybackService.ConvertToPlaybackServicePayload(null, payload);
            string batchPayload = PlaybackService.ConvertToBatchQueryResponsePayload(nonbatchPayload);
            DataServiceContext context = new DataServiceContext(request.ServiceRoot);

            TestUtil.RunCombinations(
                (IEnumerable<QueryMode>)Enum.GetValues(typeof(QueryMode)),
                new DataServiceQuery<StreamType>[] {
                    (DataServiceQuery<StreamType>)context.CreateQuery<StreamType>("Customers").Where(c1 => c1.StreamLink.SelfLink.AbsoluteUri.Equals("Foo")),
                    (DataServiceQuery<StreamType>)context.CreateQuery<StreamType>("Customers").OrderBy(c1 => c1.StreamLink) },
                (queryMode, query) =>
                {
                    using (PlaybackService.OverridingPlayback.Restore())
                    {
                        if (queryMode == QueryMode.BatchAsyncExecute ||
                            queryMode == QueryMode.BatchAsyncExecuteWithCallback ||
                            queryMode == QueryMode.BatchExecute)
                        {
                            PlaybackService.OverridingPlayback.Value = batchPayload;
                        }
                        else
                        {
                            PlaybackService.OverridingPlayback.Value = nonbatchPayload;
                        }

                        try
                        {
                            StreamType c = ((IEnumerable<StreamType>)DataServiceContextTestUtil.ExecuteQuery(context, query, queryMode)).Single();
                        }
                        catch (NotSupportedException e)
                        {
                            Assert.AreEqual(e.Message, DataServicesClientResourceUtil.GetString("ALinq_LinkPropertyNotSupportedInExpression", "StreamLink"));
                        }
                    }
                });
        }

        [TestMethod]
        public void NamedSteams_VerifyGetReadStreamVersion()
        {
            // Verify that GetReadStream for a named stream sends version 3 headers
            // Populate the context with a single customer instance
            string payload = AtomParserTests.AnyEntry(
                            id: Id,
                            editLink: request.ServiceRoot.AbsoluteUri + "/editLink/Customers(1)",
                            properties: Properties,
                            links: GetNamedStreamSelfLink(request.ServiceRoot + "/Customers(1)/SelfLink/Thumbnail", contentType: MediaContentType));

            TestUtil.RunCombinations(UnitTestsUtil.BooleanValues, (syncRead) =>
            {
                using (PlaybackService.OverridingPlayback.Restore())
                {
                    PlaybackService.OverridingPlayback.Value = PlaybackService.ConvertToPlaybackServicePayload(null, payload);
                    DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                    context.EnableAtom = true;
                    DataServiceQuery<Customer> q = (DataServiceQuery<Customer>)context.CreateQuery<Customer>("Customers").Where(c1 => c1.ID == 1);
                    Customer c = ((IEnumerable<Customer>)DataServiceContextTestUtil.ExecuteQuery(context, q, QueryMode.AsyncExecute)).Single();

                    if (syncRead)
                    {
                        context.GetReadStream(c, "Thumbnail", new DataServiceRequestArgs() { ContentType = "img/jpeg" });
                    }
                    else
                    {
                        IAsyncResult result = context.BeginGetReadStream(c, "Thumbnail", new DataServiceRequestArgs() { ContentType = "image/jpeg" }, (r) =>
                            {
                                context.EndGetReadStream(r);
                            },
                            null);

                        if (!result.CompletedSynchronously)
                        {
                            Assert.IsTrue(result.AsyncWaitHandle.WaitOne(new TimeSpan(0, 0, AstoriaUnitTests.TestConstants.MaxTestTimeout), false), "BeginExecute timeout");
                        }
                    }

                    VerifyRequestWasVersion3(PlaybackService.LastPlayback);
                }
            });
        }

        public string GetCustomerPayload(
            bool queryResponse,
            string payload,
            string xmlbase = null,
            string locationHeader = null)
        {
            string statusCode = queryResponse ? "HTTP/1.1 200 OK" : "HTTP/1.1 201 Created";
            string locationPayload = queryResponse ? null : ((locationHeader == null) ? null : "Location: " + locationHeader + Environment.NewLine);

            return statusCode + Environment.NewLine +
                "Content-Type: application/atom+xml" + Environment.NewLine +
                "Content-ID: 1" + Environment.NewLine +
                locationPayload +
                Environment.NewLine +
                payload;
        }

        private static string GetNamedStreamSelfLink(string url, string contentType = null, string name = null)
        {
            if (name == null)
            {
                name = "Thumbnail";
            }

            string contentTypeAttribute = null;
            if (contentType != null)
            {
                contentTypeAttribute = "type='" + contentType + "'";
            }

            return String.Format("<link rel='http://docs.oasis-open.org/odata/ns/mediaresource/{0}' {1} title='{0}' href='{2}' />",
                name,
                contentTypeAttribute,
                url);
        }

        private static string GetNamedStreamSelfLinkWithNull(string url, string contentType = null, string name = null)
        {
            if (name == null)
            {
                name = "Thumbnail";
            }

            string contentTypeAttribute = null;
            if (contentType != null)
            {
                contentTypeAttribute = "type='" + contentType + "'";
            }

            string href = url == null ? string.Empty : string.Format("href='{0}'", url);

            return String.Format("<link rel='http://docs.oasis-open.org/odata/ns/mediaresource/{0}' {1} title='{0}' {2} />",
                name,
                contentTypeAttribute,
                href);
        }

        internal static string GetNamedStreamEditLink(string url, string contentType = null, string etag = null, string name = null)
        {
            if (name == null)
            {
                name = "Thumbnail";
            }

            string contentTypeAttribute = contentType == null ? null : "type='" + contentType + "'";
            string etagAttribute = etag == null ? null : "m:etag='" + etag + "'";
            string hrefAttribute = url == null ? null : "href='" + url + "'";
            return String.Format("<link rel='http://docs.oasis-open.org/odata/ns/edit-media/{0}' {1} title='{0}' {2} {3} />",
                name,
                contentTypeAttribute,
                hrefAttribute,
                etagAttribute);
        }

        private static void VerifyNamedStreamInfo(StreamDescriptor streamInfo, string selfLink, string editLink, string etag, string contentType)
        {
            Assert.AreEqual(streamInfo.StreamLink.SelfLink == null ? null : streamInfo.StreamLink.SelfLink.AbsoluteUri, selfLink == null ? null : String.Format(selfLink, streamInfo.StreamLink.Name), "self links should match");
            Assert.AreEqual(streamInfo.StreamLink.EditLink == null ? null : streamInfo.StreamLink.EditLink.AbsoluteUri, editLink == null ? null : String.Format(editLink, streamInfo.StreamLink.Name), "edit links should match");
            Assert.AreEqual(streamInfo.StreamLink.ContentType, contentType, "content type should match");
            Assert.AreEqual(streamInfo.StreamLink.ETag, etag, "etag should match");
        }

        static void VerifyRequestWasVersion3(string playback)
        {
            StringReader reader = new StringReader(playback);
            string s;
            bool foundVersion = false;
            bool foundMaxVersion = false;

            while (!(foundMaxVersion && foundVersion) && (s = reader.ReadLine()) != null)
            {
                foundMaxVersion |= string.Equals(s, "OData-MaxVersion: 4.0");
                foundVersion |= string.Equals(s, "OData-Version: 4.0");
            }

            Assert.IsTrue(foundVersion, "client should send OData-Version: 4.0 in the DSV header");
            Assert.IsTrue(foundMaxVersion, "client should send OData-MaxVersion: 4.0 in the DSV header");
        }

        private class StreamTypeWithMismatchedStreamProperty
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public byte[] Thumbnail { get; set; }
            public DataServiceStreamLink Photo { get; set; }
        }

        private class StreamType2
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public DataServiceStreamLink Thumbnail { get; set; }
            public DataServiceStreamLink Photo { get; set; }
        }

        public class NamedStreamWithBinding : CollectionBinding.PropertyChangedBase
        {
            public int ID
            {
                get
                {
                    return id;
                }
                set
                {
                    id = value;
                    OnPropertyChanged("ID");
                }
            }
            private int id;

            private string name;
            public string Name
            {
                get { return this.name; }
                set
                {
                    this.name = value;
                    OnPropertyChanged("Name");
                }
            }

            private DataServiceStreamLink link;
            public DataServiceStreamLink Thumbnail
            {
                get { return this.link; }
                set
                {
                    this.link = value;
                    OnPropertyChanged("Thumbnail");
                }
            }
        }
    }
}
