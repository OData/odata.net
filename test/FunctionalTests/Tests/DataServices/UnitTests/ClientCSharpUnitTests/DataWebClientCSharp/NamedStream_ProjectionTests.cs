//---------------------------------------------------------------------
// <copyright file="NamedStream_ProjectionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.DataWebClientCSharp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AstoriaUnitTests.ClientExtensions;
    using AstoriaUnitTests.DataWebClientCSharp.Services;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Stubs.DataServiceProvider;
    using AstoriaUnitTests.Tests;
    using Microsoft.OData.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/881
    [Ignore] // Remove Atom
    // [TestClass]
    public class NamedStream_ProjectionTests
    {
        private static TestWebRequest request;

        [TestInitialize]
        public void TestInitialize()
        {
             DSPServiceDefinition service = NamedStreamService.SetUpNamedStreamService();
             request = service.CreateForInProcessWcf();
             request.StartService();
        }

        [TestCleanup]
        public void ClassCleanup()
        {
            if (request != null)
            {
                request.Dispose();
                request = null;
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void NamedStreams_SimpleProjectionWithoutStreams()
        {
            // Doing projection of non-stream properties should work and there should be no stream descriptors populated in the context
            DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
            //context.EnableAtom = true;
            //context.Format.UseAtom();
            var query1 = context.CreateQuery<EntityWithNamedStreams1>("MySet1").AddQueryOption("$select", "ID");
            List<EntityWithNamedStreams1> entities = query1.Execute().ToList();
            Assert.AreEqual(entities.Count, 1, "There must be only 1 entities populated in the context");
            Assert.AreEqual(context.Entities[0].StreamDescriptors.Count, 0, "There must be no named streams associated with the entity yet, since we didn't specify the named streams in the projection query");
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void NamedStreams_SimpleProjectionWithStreams()
        {
            // Doing projection of stream properties using AddQueryOption should work
            DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
            //context.EnableAtom = true;
            //context.Format.UseAtom();
            var query1 = context.CreateQuery<EntityWithNamedStreams1>("MySet1").AddQueryOption("$select", "ID, Stream1");
            List<EntityWithNamedStreams1> entities = query1.Execute().ToList();
            Assert.AreEqual(context.Entities[0].StreamDescriptors.Count, 1, "There must be named streams associated with the entity");
        }

        //[TestMethod, Variation("One should not be able to get named streams via load property api")]
        public void NamedStreams_LoadPropertyTest()
        {
            // populate the context
            DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
            EntityWithNamedStreams1 entity = context.CreateQuery<EntityWithNamedStreams1>("MySet1").Take(1).Single();

            try
            {
                context.LoadProperty(entity, "Stream1");
            }
            catch (DataServiceClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains(DataServicesResourceUtil.GetString("DataService_VersionTooLow", "1.0", "3", "0")), String.Format("The error message was not as expected: {0}", ex.Message));
            }

            try
            {
                context.BeginLoadProperty(
                    entity,
                    "Stream1",
                    (result) => { context.EndLoadProperty(result); },
                    null);
            }
            catch (DataServiceClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains(DataServicesResourceUtil.GetString("DataService_VersionTooLow", "1.0", "3", "0")), String.Format("The error message was not as expected: {0}", ex.Message));
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void NamedStreams_SimpleLinkProjection()
        {
            // Simple projections to get stream url in DSSL property - both narrow type and anonymous type
            {
                // Testing querying anonymous types and making sure one is able to project out the stream url
                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                //context.EnableAtom = true;
                //context.Format.UseAtom();
                var q = from s in context.CreateQuery<EntityWithNamedStreams1>("MySet1")
                        select new
                        {
                            ID = s.ID,
                            Stream11 = s.Stream1
                        };

                Assert.AreEqual(q.ToString(), request.ServiceRoot.AbsoluteUri + "/MySet1?$select=ID,Stream1", "make sure the right uri is produced by the linq translator");
                foreach (var o in q)
                {
                    Assert.IsNotNull(o.Stream11, "Stream11 must have some value");
                    Assert.AreEqual(o.Stream11.EditLink.AbsoluteUri, request.ServiceRoot.AbsoluteUri + "/MySet1(1)/Stream1", "make sure the url property is correctly populated");
                    Assert.IsNull(context.GetEntityDescriptor(o), "anonymous types are never tracked, even if we do a simple projection");
                }

                Assert.AreEqual(context.Entities.Count, 0, "there should be no entity tracked by the context");
            }

            {
                // Testing querying narrow entity types and making sure one is able to project out the stream url
                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                //context.EnableAtom = true;
                //context.Format.UseAtom();
                var q = from s in context.CreateQuery<EntityWithNamedStreams1>("MySet1")
                        select new EntityWithNamedStreams1
                        {
                            ID = s.ID,
                            Stream1 = s.Stream1
                        };

                Assert.AreEqual(q.ToString(), request.ServiceRoot.AbsoluteUri + "/MySet1?$select=ID,Stream1", "make sure the right uri is produced by the linq translator");
                foreach (EntityWithNamedStreams1 o in q)
                {
                    Assert.IsNotNull(o.Stream1, "Stream11 must have some value");
                    Assert.AreEqual(o.Stream1.EditLink, context.GetReadStreamUri(o, "Stream1"), "the value in the entity descriptor must match with the property value");
                }

                Assert.AreEqual(context.Entities.Count, 1, "there should be only one entity tracked by the context");
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void NamedStreams_ProjectingOnlyStreamLinkProperty()
        {
            // Just project out the link properties in an entity, no data properties - both narrow type and anonymous type
            {
                // Testing querying anonymous types and making sure one is able to project out the stream url
                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                //context.EnableAtom = true;
                //context.Format.UseAtom();
                var q = from s in context.CreateQuery<EntityWithNamedStreams1>("MySet1")
                        select new
                        {
                            Stream11 = s.Stream1
                        };

                Assert.AreEqual(q.ToString(), request.ServiceRoot.AbsoluteUri + "/MySet1?$select=Stream1", "make sure the right uri is produced by the linq translator");
                foreach (var o in q)
                {
                    Assert.IsNotNull(o.Stream11, "Stream11 must have some value");
                    Assert.AreEqual(o.Stream11.EditLink.AbsoluteUri, request.ServiceRoot.AbsoluteUri + "/MySet1(1)/Stream1", "make sure the url property is correctly populated");
                    Assert.IsNull(context.GetEntityDescriptor(o), "anonymous types are never tracked, even if we do a simple projection");
                }

                Assert.AreEqual(context.Entities.Count, 0, "there should be no entity tracked by the context");
            }

            {
                // Testing querying narrow entity types and making sure one is able to project out the stream url
                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                //context.EnableAtom = true;
                //context.Format.UseAtom();
                var q = from s in context.CreateQuery<EntityWithNamedStreams1>("MySet1")
                        select new EntityWithNamedStreams1
                        {
                            Stream1 = s.Stream1,
                        };

                Assert.AreEqual(q.ToString(), request.ServiceRoot.AbsoluteUri + "/MySet1?$select=Stream1", "make sure the right uri is produced by the linq translator");
                foreach (EntityWithNamedStreams1 o in q)
                {
                    Assert.IsNotNull(o.Stream1, "Stream11 must have some value");
                    Assert.AreEqual(o.Stream1.EditLink, context.GetReadStreamUri(o, "Stream1"), "the value in the entity descriptor must match with the property value");
                }

                Assert.AreEqual(context.Entities.Count, 1, "there should be only one entity tracked by the context");
            }
        }

        [TestMethod]
        public void NamedStreams_CannotReferenceDeepLinksDuringEntityMaterialization()
        {
            // If the entity getting projected out in an entity, one should not be able to refer deep links
            {
                // Flattening of entity types is not allowed, when the projected type is an entity type.
                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                var q = from s in context.CreateQuery<EntityWithNamedStreams1>("MySet1")
                        select new EntityWithNamedStreams1
                        {
                            ID = s.ID,
                            Name = s.Name,
                            Stream1 = s.Ref.Stream1
                        };

                try
                {
                    foreach (var o in q)
                    {
                    }
                }
                catch (NotSupportedException ex)
                {
                    Assert.AreEqual(ex.Message, DataServicesClientResourceUtil.GetString("ALinq_ProjectionMemberAssignmentMismatch", typeof(EntityWithNamedStreams1).FullName, "s", "s.Ref"));
                }

                Assert.AreEqual(context.Entities.Count, 0, "there should be no entities tracked by the context, since we are doing flattening");
            }

            {
                // Flattening of entity types is not allowed, when the projected type is an entity type.
                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                var q = from s in context.CreateQuery<EntityWithNamedStreams1>("MySet1")
                        select new EntityWithNamedStreams1
                        {
                            ID = s.Ref.ID,
                            Name = s.Ref.Name,
                            Stream1 = s.Stream1
                        };

                try
                {
                    foreach (var o in q)
                    {
                    }
                }
                catch (NotSupportedException ex)
                {
                    Assert.AreEqual(ex.Message, DataServicesClientResourceUtil.GetString("ALinq_ProjectionMemberAssignmentMismatch", typeof(EntityWithNamedStreams1).FullName, "s.Ref", "s"));
                }

                Assert.AreEqual(context.Entities.Count, 0, "there should be no entities tracked by the context, since we are doing flattening");
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void NamedStreams_NonEntity_AccessPropertiesFromDifferentLevels()
        {
            // If the entity getting projected out in an anonymous type, one should be able to project out links from various levels
            {
                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                //context.EnableAtom = true;
                //context.Format.UseAtom();
                var q = from s in context.CreateQuery<EntityWithNamedStreams1>("MySet1")
                        select new
                        {
                            ID = s.ID,
                            Name = s.Name,
                            Stream1Url = s.Ref.RefStream1
                        };

                foreach (var o in q)
                {
                    Assert.AreEqual(o.Stream1Url.EditLink.AbsoluteUri, request.ServiceRoot.AbsoluteUri + "/MySet2(3)/RefStream1", "link must be populated");
                }

                Assert.AreEqual(context.Entities.Count, 0, "there should be no entities tracked by the context, since we are doing flattening");
            }

            {
                // Querying url of the nested type - doing this makes the entity non-tracking, but populated the link property
                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                //context.EnableAtom = true;
                //context.Format.UseAtom();
                var q = from s in context.CreateQuery<EntityWithNamedStreams1>("MySet1")
                        select new
                        {
                            ID = s.Ref.ID,
                            Name = s.Ref.Name,
                            Stream1Url = s.Stream1
                        };

                foreach (var o in q)
                {
                    Assert.AreEqual(o.Stream1Url.EditLink.AbsoluteUri, request.ServiceRoot.AbsoluteUri + "/MySet1(1)/Stream1", "link must be populated");
                }

                Assert.AreEqual(context.Entities.Count, 0, "there should be no entities tracked by the context, since we are doing flattening");
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void NamedStreams_Projection()
        {
            // Projection via selecting properties
            // Testing without projections (payload driven) and making sure one is able to project out the stream url
            DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
            //context.EnableAtom = true;
            //context.Format.UseAtom();
            var q = from c in context.CreateQuery<EntityWithStreamLink>("MySet1")
                    select new EntityWithStreamLink()
                    {
                        ID = c.ID,
                        Stream1 = c.Stream1
                    };

            foreach (var o in q)
            {
                Assert.IsNotNull(o.Stream1);
                Assert.AreEqual(o.Stream1.EditLink.AbsoluteUri, request.ServiceRoot.AbsoluteUri + "/MySet1(1)/Stream1", "link must be populated");
            }

        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void NamedStreams_PayloadDrivenMaterialization()
        {
            // Make sure DSSL properties can materialized and populated with the right url in non-projection cases
            {
                // Testing without projections (payload driven) and making sure one is able to project out the stream url
                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                //context.EnableAtom = true;
                //context.Format.UseAtom();
                context.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;
                var q = context.CreateQuery<EntityWithStreamLink>("MySet1");
                object entity = null;

                foreach (EntityWithStreamLink o in q)
                {
                    Assert.IsNotNull(o.Stream1, "Stream1 must have some value");
                    Assert.AreEqual(o.Stream1.EditLink, context.GetReadStreamUri(o, "Stream1"), "the value in the entity descriptor must match with the property value");
                    Assert.IsNull(o.SomeRandomProperty, "SomeRandomProperty must be null, since the payload does not have the link with the property name");
                    entity = o;
                }

                // Try updating the entity and make sure that the link is not send back in the payload.
                context.UpdateObject(entity);

                WrappingStream wrappingStream = null;
                context.RegisterStreamCustomizer((inputStream) =>
                    {
                        wrappingStream = new WrappingStream(inputStream);
                        return wrappingStream;
                    },
                    null);

                try
                {
                    context.SaveChanges();
                    Assert.Fail("Save changes should throw an exception");
                }
                catch (Exception)
                {
                    // do nothing
                }

                string payload = wrappingStream.GetLoggingStreamAsString();
                Assert.IsTrue(payload.Contains("<d:ID m:type=\"Int32\">1</d:ID>"), "Id element must be present");
                Assert.IsFalse(payload.Contains("Stream1Url"), "link url should not be sent in the payload");
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void NamedStreams_DeepLinkProjection()
        {
            // projecting out deep links to get stream url in DSSL property - both narrow type and anonymous type
            {
                // Querying url of the nested type - doing this makes the entity non-tracking, but populated the link property
                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                //context.EnableAtom = true;
                //context.Format.UseAtom();
                var q = from s in context.CreateQuery<EntityWithNamedStreams1>("MySet1")
                        select new
                        {
                            ID = s.Ref.ID,
                            Url = s.Ref.RefStream1
                        };

                Assert.AreEqual(request.ServiceRoot.AbsoluteUri + "/MySet1?$expand=Ref($select=ID),Ref($select=RefStream1)", q.ToString(), "make sure the right uri is produced by the linq translator");

                foreach (var o in q)
                {
                    Assert.IsNotNull(o.Url, "Stream11 must have some value");
                    Assert.AreEqual(o.Url.EditLink.AbsoluteUri, request.ServiceRoot.AbsoluteUri + "/MySet2(3)/RefStream1", "the stream url must be populated correctly");
                    Assert.IsNull(context.GetEntityDescriptor(o), "the entity must not be tracked by the context since we are trying to flatten the hierarchy");
                }

                Assert.AreEqual(context.Entities.Count, 0, "there should be no entities tracked by the context, since we are doing flattening");
            }

            {
                // Querying url of the nested type - doing this makes the entity non-tracking, but populated the link property
                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                //context.EnableAtom = true;
                //context.Format.UseAtom();
                var q = from s in context.CreateQuery<EntityWithNamedStreams1>("MySet1")
                        select new EntityWithNamedStreams1
                        {
                            ID = s.Ref.ID,
                            RefStream1 = s.Ref.RefStream1
                        };

                Assert.AreEqual(request.ServiceRoot.AbsoluteUri + "/MySet1?$expand=Ref($select=ID),Ref($select=RefStream1)", q.ToString(), "make sure the right uri is produced by the linq translator");

                foreach (var o in q)
                {
                    Assert.IsNotNull(o.RefStream1, "Stream11 must have some value");
                    Assert.AreEqual(o.RefStream1.EditLink, context.GetReadStreamUri(o, "RefStream1"), "the stream url must be populated correctly");
                }

                Assert.AreEqual(context.Entities.Count, 1, "there should be exactly one entity tracked by the context - the nested entity");
                Assert.AreEqual(context.Entities[0].EditLink.AbsoluteUri, request.ServiceRoot.AbsoluteUri + "/MySet2(3)", "the nested entity is the one that should be tracked");
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void NamedStreams_DeepLinkProjection_MultipleParametersInScope()
        {
            // projecting out deep links to get stream url in DSSL property with multiple parameters in scope - both narrow type and anonymous type
            {
                // Querying url of the nested type - doing this makes the entity non-tracking, but populated the link property
                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                //context.EnableAtom = true;
                //context.Format.UseAtom();
                var q = from s in context.CreateQuery<EntityWithNamedStreams1>("MySet1")
                        where s.ID == 1
                        from c in s.Collection
                        select new
                        {
                            Name = c.Name,
                            Url = c.ColStream
                        };

                Assert.AreEqual(q.ToString(), request.ServiceRoot.AbsoluteUri + "/MySet1(1)/Collection?$select=Name,ColStream", "make sure the right uri is produced by the linq translator");

                var entities = q.ToList();

                // Since there is SDP turned on, the inner collection returns only 1 entity.
                Assert.AreEqual(entities[0].Url.EditLink.AbsoluteUri, request.ServiceRoot.AbsoluteUri + "/MySet3('ABCDE')/ColStream", "the stream url must be populated correctly - index 0");
                Assert.AreEqual(context.Entities.Count, 0, "there should be no entities tracked by the context, since we are doing flattening");
            }

            {
                // Querying url of the nested type - doing this makes the entity non-tracking, but populated the link property
                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                //context.EnableAtom = true;
                //context.Format.UseAtom();
                var q = from s in context.CreateQuery<EntityWithNamedStreams1>("MySet1")
                        where s.ID == 1
                        from c in s.Collection
                        select new EntityWithNamedStreams2()
                        {
                            ID = c.ID,
                            ColStream = c.ColStream
                        };

                Assert.AreEqual(q.ToString(), request.ServiceRoot.AbsoluteUri + "/MySet1(1)/Collection?$select=ID,ColStream", "make sure the right uri is produced by the linq translator");

                var entities = q.ToList();

                // Since there is SDP turned on, the inner collection returns only 1 entity.
                Assert.AreEqual(entities[0].ColStream.EditLink, context.GetReadStreamUri(entities[0], "ColStream"), "the stream url must be populated correctly - index 0");
                Assert.AreEqual(context.Entities.Count, 1, "there should be no entities tracked by the context, since we are doing flattening");
            }
        }

        [TestMethod]
        public void NamedStreams_DeepEntityProjection_CannotAccessEntitiesAcrossLevels()
        {
            // projecting out deep links to get stream url in DSSL property with multiple parameters in scope - both narrow type and anonymous type
            {
                // Querying url of the nested type - doing this makes the entity non-tracking, but populated the link property
                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                var q = from s in context.CreateQuery<EntityWithNamedStreams1>("MySet1")
                        where s.ID == 1
                        from c in s.Collection
                        select new EntityWithNamedStreams2()
                        {
                            ID = c.ID,
                            Stream1 = s.Stream1
                        };

                try
                {
                    q.ToList();
                }
                catch (NotSupportedException ex)
                {
                    Assert.AreEqual(ex.Message, DataServicesClientResourceUtil.GetString("ALinq_CanOnlyProjectTheLeaf"), "error message should match as expected");
                }

                Assert.AreEqual(context.Entities.Count, 0, "there should be no entities tracked by the context in error case");
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void NamedStreams_NestedQuery_1()
        {
            // projecting out deep links to get stream url in DSSL property - both narrow type and anonymous type
            {
                // Querying url of the nested type - doing this makes the entity non-tracking, but populated the link property
                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                //context.EnableAtom = true;
                //context.Format.UseAtom();
                var q = from s in context.CreateQuery<EntityWithNamedStreams1>("MySet1")
                        select new EntityWithNamedStreams1
                        {
                            ID = s.ID,
                            Stream1 = s.Stream1,
                            Ref = new EntityWithNamedStreams1
                            {
                                ID = s.Ref.ID,
                                RefStream1 = s.Ref.RefStream1
                            },
                            Collection = (from c in s.Collection
                                          select new EntityWithNamedStreams2()
                                          {
                                              ID = c.ID,
                                              ColStream = c.ColStream
                                          }).ToList()
                        };

                Assert.AreEqual(request.ServiceRoot.AbsoluteUri + "/MySet1?$expand=Ref($select=ID),Ref($select=RefStream1),Collection($select=ID),Collection($select=ColStream)&$select=ID,Stream1", q.ToString(), "make sure the right uri is produced by the linq translator");

                foreach (var o in q)
                {
                    Assert.IsNotNull(o.Stream1, "Stream11 must have some value");
                    Assert.AreEqual(o.Stream1.EditLink, context.GetReadStreamUri(o, "Stream1"), "the stream url for Stream1 must be populated correctly");
                    Assert.AreEqual(o.Ref.RefStream1.EditLink, context.GetReadStreamUri(o.Ref, "RefStream1"), "the stream url for RefStream1 must be populated correctly");
                    foreach (var c in o.Collection)
                    {
                        Assert.AreEqual(c.ColStream.EditLink, context.GetReadStreamUri(c, "ColStream"), "the url for the nested collection entity should match");
                    }
                }

                Assert.AreEqual(context.Entities.Count, 3, "there should be 3 entities tracked by the context");
                Assert.AreEqual(context.Entities[0].EditLink.AbsoluteUri, request.ServiceRoot.AbsoluteUri + "/MySet1(1)", "top level entity must be tracked");
                Assert.AreEqual(context.Entities[1].EditLink.AbsoluteUri, request.ServiceRoot.AbsoluteUri + "/MySet2(3)", "the nested entity must be tracked");
                Assert.AreEqual(context.Entities[2].EditLink.AbsoluteUri, request.ServiceRoot.AbsoluteUri + "/MySet3('ABCDE')", "top level entity must be tracked");
            }

            {
                // Querying url of the nested type - doing this makes the entity non-tracking, but populated the link property
                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                //context.EnableAtom = true;
                //context.Format.UseAtom();
                var q = from s in context.CreateQuery<EntityWithNamedStreams1>("MySet1")
                        select new
                        {
                            ID = s.ID,
                            Stream1Url = s.Stream1,
                            Ref = new
                            {
                                ID = s.Ref.ID,
                                Stream1Url = s.Ref.RefStream1
                            },
                            Collection = (from c in s.Collection
                                          select new
                                          {
                                              Name = c.Name,
                                              Stream1Url = c.ColStream
                                          })
                        };

                Assert.AreEqual(request.ServiceRoot.AbsoluteUri + "/MySet1?$expand=Ref($select=ID),Ref($select=RefStream1),Collection($select=Name),Collection($select=ColStream)&$select=ID,Stream1", q.ToString(), "make sure the right uri is produced by the linq translator");

                foreach (var o in q)
                {
                    Assert.AreEqual(o.Stream1Url.EditLink, request.ServiceRoot.AbsoluteUri + "/MySet1(1)/Stream1", "the stream url for Stream1 must be populated correctly");
                    Assert.AreEqual(o.Ref.Stream1Url.EditLink, request.ServiceRoot.AbsoluteUri + "/MySet2(3)/RefStream1", "the stream url for RefStream1 must be populated correctly");
                    Assert.AreEqual(o.Collection.First().Stream1Url.EditLink, request.ServiceRoot.AbsoluteUri + "/MySet3('ABCDE')/ColStream", "the stream url of the collection stream must be populated correctly - index 0");
                }

                Assert.AreEqual(context.Entities.Count, 0, "there should be no entities tracked by the context");
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void NamedStreams_NestedQuery_2()
        {
            // projecting out collection of collection properties - both narrow type and anonymous type
            {
                // Querying url of the nested type - doing this makes the entity non-tracking, but populated the link property
                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                //context.EnableAtom = true;
                //context.Format.UseAtom();
                DataServiceQuery<EntityWithNamedStreams1> q = (DataServiceQuery<EntityWithNamedStreams1>)from s in context.CreateQuery<EntityWithNamedStreams1>("MySet1")
                        select new EntityWithNamedStreams1
                        {
                            ID = s.ID,
                            Stream1 = s.Stream1,
                            Collection = (from c in s.Collection
                                          select new EntityWithNamedStreams2()
                                          {
                                              ID = c.ID,
                                              ColStream = c.ColStream,
                                              Collection1 = (from c1 in c.Collection1
                                                            select new EntityWithNamedStreams1()
                                                            {
                                                                ID = c1.ID,
                                                                RefStream1 = c1.RefStream1
                                                            }).ToList()
                                          }).ToList()
                        };

                Assert.AreEqual(request.ServiceRoot.AbsoluteUri + "/MySet1?$expand=Collection($select=ID),Collection($select=ColStream),Collection($expand=Collection1($select=ID)),Collection($expand=Collection1($select=RefStream1))&$select=ID,Stream1", q.ToString(), "make sure the right uri is produced by the linq translator");

                var response = (QueryOperationResponse<EntityWithNamedStreams1>)q.Execute();
                DataServiceQueryContinuation<EntityWithNamedStreams2> continuation = null;
                foreach (var o in response)
                {
                    Assert.IsNotNull(o.Stream1.EditLink, "Stream1 should not be null");
                    Assert.AreEqual(o.Stream1.EditLink, context.GetReadStreamUri(o, "Stream1"), "the stream url for Stream1 must be populated correctly");
                    foreach (var c in o.Collection)
                    {
                        Assert.IsNotNull(c.ColStream.EditLink, "ColStream should not be null");
                        Assert.AreEqual(c.ColStream.EditLink, context.GetReadStreamUri(c, "ColStream"), "the url for the nested collection entity should match - Level 0");
                        foreach (var c1 in c.Collection1)
                        {
                            Assert.IsNotNull(c1.RefStream1.EditLink, "RefStream1 should not be null");
                            Assert.AreEqual(c1.RefStream1.EditLink, context.GetReadStreamUri(c1, "RefStream1"), "the url for the nested collection entity should match - Level 1");
                        }
                    }

                    // Make sure you get the continuation token for the collection and try and get the next page
                    continuation = response.GetContinuation(o.Collection);
                }

                Assert.AreEqual(context.Entities.Count, 3, "there should be 3 entities tracked by the context");
                Assert.AreEqual(context.Entities[0].EditLink.AbsoluteUri, request.ServiceRoot.AbsoluteUri + "/MySet1(1)", "top level entity must be tracked");
                Assert.AreEqual(context.Entities[1].EditLink.AbsoluteUri, request.ServiceRoot.AbsoluteUri + "/MySet3('ABCDE')", "top level entity must be tracked");
                Assert.AreEqual(context.Entities[2].EditLink.AbsoluteUri, request.ServiceRoot.AbsoluteUri + "/MySet2(3)", "the nested entity must be tracked");
                Assert.IsNotNull(continuation, "since SDP is turned on, we should get the continuation token");

                // Get the next page and make sure we get the right entity and the link is populated.
                foreach (var entity in context.Execute(continuation))
                {
                    Assert.IsNotNull(entity.ColStream.EditLink, "ColStream should not be null");
                    Assert.AreEqual(entity.ColStream.EditLink, context.GetReadStreamUri(entity, "ColStream"), "the url for the nested collection entity should match - Level 1");
                    foreach (var c1 in entity.Collection1)
                    {
                        Assert.IsNotNull(c1.RefStream1.EditLink, "RefStream1 should not be null");
                        Assert.AreEqual(c1.RefStream1.EditLink, context.GetReadStreamUri(c1, "RefStream1"), "the url for the nested collection entity should match - Level 1");
                    }
                }

                Assert.AreEqual(context.Entities.Count, 4, "there should be 4 entities tracked by the context");
            }

            {
                // Querying url of the nested type - doing this makes the entity non-tracking, but populated the link property
                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                //context.EnableAtom = true;
                //context.Format.UseAtom();
                var q = from s in context.CreateQuery<EntityWithNamedStreams1>("MySet1")
                        select new
                        {
                            ID = s.ID,
                            Stream1Url = s.Stream1,
                            Collection = (from c in s.Collection
                                          select new
                                          {
                                              Name = c.Name,
                                              Stream1Url = c.ColStream,
                                              Collection1 = (from c1 in c.Collection1
                                                             select new
                                                             {
                                                                 ID = c1.ID,
                                                                 Stream1Url = c1.RefStream1
                                                             })
                                          })
                        };

                Assert.AreEqual(request.ServiceRoot.AbsoluteUri + "/MySet1?$expand=Collection($select=Name),Collection($select=ColStream),Collection($expand=Collection1($select=ID)),Collection($expand=Collection1($select=RefStream1))&$select=ID,Stream1", q.ToString(), "make sure the right uri is produced by the linq translator");

                foreach (var o in q)
                {
                    Assert.AreEqual(o.Stream1Url.EditLink, request.ServiceRoot.AbsoluteUri + "/MySet1(1)/Stream1", "the stream url for Stream1 must be populated correctly");
                    Assert.AreEqual(o.Collection.First().Stream1Url.EditLink, request.ServiceRoot.AbsoluteUri + "/MySet3('ABCDE')/ColStream", "the stream url of the collection stream must be populated correctly - index 0");
                    Assert.AreEqual(o.Collection.First().Collection1.Single().Stream1Url.EditLink, request.ServiceRoot.AbsoluteUri + "/MySet2(3)/RefStream1", "the stream url of the collection stream must be populated correctly - index 0 - index 0");
                }

                Assert.AreEqual(context.Entities.Count, 0, "there should be no entities tracked by the context");
            }
        }

        private class EntityWithStreamLink
        {
            public int ID { get; set; }
            public DataServiceStreamLink Stream1 { get; set; }
            public DataServiceStreamLink SomeRandomProperty { get; set; }
        }
    }
}