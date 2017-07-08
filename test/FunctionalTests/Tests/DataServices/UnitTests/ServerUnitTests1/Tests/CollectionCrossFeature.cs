//---------------------------------------------------------------------
// <copyright file="CollectionCrossFeature.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Client;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Xml.Linq;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Stubs.DataServiceProvider;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/868
    [TestModule]
    public partial class UnitTestModule1
    {
        [Ignore] // Remove Atom
        public partial class CollectionTest
        {
            #region XFeature entities and helpers
            public class XFeatureTestsEntity
            {
                public int ID { get; set; }
                public string Description { get; set; }
                public List<string> Strings { get; set; }
                public List<XFeatureTestsComplexType> Structs { get; set; }
                public XFeatureTestsEntity NextTestEntity { get; set; }
            }

            [HasStream]
            public class XFeatureTestsMLE : XFeatureTestsEntity
            {
            }

            public class XFeatureTestsComplexType
            {
                public string Text { get; set; }
            }

            private static DSPMetadata CreateMetadataForXFeatureEntity(bool isMLE = false)
            {
                DSPMetadata metadata = new DSPMetadata("CollectionCrossFeatureTests", "AstoriaUnitTests.Tests");
                var entityType = metadata.AddEntityType("XFeatureTestsEntity", typeof(DSPResourceWithCollectionProperty), null, false);
                if (isMLE)
                {
                    entityType.IsMediaLinkEntry = true;
                }
                metadata.AddKeyProperty(entityType, "ID", typeof(int));
                metadata.AddPrimitiveProperty(entityType, "Description", typeof(string));
                metadata.AddCollectionProperty(entityType, "Strings", typeof(string));
                var complexType = metadata.AddComplexType("UnitTestModule+CollectionTest+XFeatureTestsComplexType", typeof(DSPResourceWithCollectionProperty), null, false);
                metadata.AddPrimitiveProperty(complexType, "Text", typeof(string));
                metadata.AddCollectionProperty(entityType, "Structs", complexType);
                var resourceSet = metadata.AddResourceSet("Entities", entityType);
                metadata.AddResourceReferenceProperty(entityType, "NextTestEntity", resourceSet, entityType);
                metadata.SetReadOnly();

                return metadata;
            }

            private static List<object> CreateClientTestEntities()
            {
                List<object> testEntities = new List<object>();
                testEntities.Add(new XFeatureTestsEntity()
                {
                    ID = 1,
                    Description = "Entity 1",
                    Strings = new List<string>(new string[] { "string 1", "string 2", string.Empty }),
                    Structs = new List<XFeatureTestsComplexType>(new XFeatureTestsComplexType[] {
                                new XFeatureTestsComplexType() { Text = "text 1" },
                                new XFeatureTestsComplexType() { Text = "text 2" }})
                });

                testEntities.Add(new XFeatureTestsEntity()
                {
                    ID = 2,
                    Description = "Entity 2",
                    Strings = new List<string>(new string[] { "abc", string.Empty, "xyz" }),
                    Structs = new List<XFeatureTestsComplexType>(new XFeatureTestsComplexType[] {
                                new XFeatureTestsComplexType() { Text = "abc" },
                                new XFeatureTestsComplexType() { Text = null },
                                new XFeatureTestsComplexType() { Text = "xyz" }})
                });

                testEntities.Add(new XFeatureTestsEntity()
                {
                    ID = 3,
                    Description = "Entity 3",
                    Strings = new List<string>(new string[] { }),
                    Structs = new List<XFeatureTestsComplexType>(new XFeatureTestsComplexType[] {
                                new XFeatureTestsComplexType() { Text = null }})
                });

                return testEntities;
            }

            private static void PopulateClientContextWithTestEntities(DataServiceContext context)
            {
                foreach (var entity in CreateClientTestEntities())
                {
                    context.AddObject("Entities", entity);
                }
                context.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
            }

            #endregion XFeature entities and helpers

            #region XFeature - Collection & BatchWithSingleChangeset with IDataServiceHost/IDataServiceHost2 and Change Tracking

            // Cross feature, end to end (server + client) tests for collection combined with BatchWithSingleChangeset, IDataServiceHost/IDataServiceHost2 and Change tracking.
            // [TestCategory("Partition1"), TestMethod]
            public void Collection_BatchIDataServiceHostAndChangeTracking()
            {
                DSPMetadata metadata = CreateMetadataForXFeatureEntity();

                TestUtil.RunCombinations(
                    new bool[] { false, true },
                    new bool[] { false, true },
                    new Type[] { typeof(IDataServiceHost), typeof(IDataServiceHost2) },
                    (sendAsBatch, replaceOnUpdate, hostInterfaceType) => {

                    DSPServiceDefinition service = new DSPServiceDefinition() { Metadata = metadata, Writable = true, HostInterfaceType = hostInterfaceType };
                    
                    DSPContext data = new DSPContext();
                    service.CreateDataSource = (m) => { return data; };
                    // This test operates just on 2 entities - so let's take just first two from the set
                    List<object> testEntities = CreateClientTestEntities().Take(2).ToList<object>();
                    SaveChangesOptions saveOptions = 
                        (sendAsBatch ? SaveChangesOptions.BatchWithSingleChangeset : SaveChangesOptions.None) | 
                        (replaceOnUpdate ? SaveChangesOptions.ReplaceOnUpdate : SaveChangesOptions.None);

                    using (TestWebRequest request = service.CreateForInProcessWcf())
                    using (DSPResourceWithCollectionProperty.CollectionPropertiesResettable.Restore())
                    {
                        DSPResourceWithCollectionProperty.CollectionPropertiesResettable.Value = true;
                        request.StartService();

                        // Add entities
                        DataServiceContext ctx = new DataServiceContext(new Uri(request.BaseUri), ODataProtocolVersion.V4);
                        //ctx.EnableAtom = true;
                        //ctx.Format.UseAtom();
                        foreach (XFeatureTestsEntity entity in testEntities)
                        {
                            ctx.AddObject("Entities", entity);
                        }
                        VerifyStateOfEntities(ctx, testEntities, EntityStates.Added);
                        ctx.SaveChanges(saveOptions);
                        VerifyStateOfEntities(ctx, testEntities, EntityStates.Unchanged);
                        VerifyEntitySetsMatch(testEntities, data.GetResourceSetEntities("Entities"));

                        // Change one of the entities
                        ((XFeatureTestsEntity)testEntities[0]).Structs.RemoveAt(0);
                        ctx.UpdateObject(testEntities[0]);
                        VerifyStateOfEntities(ctx, new[] { testEntities[0] }, EntityStates.Modified);
                        VerifyStateOfEntities(ctx, new[] { testEntities[1] }, EntityStates.Unchanged);
                        ctx.SaveChanges(saveOptions);
                        VerifyStateOfEntities(ctx, testEntities, EntityStates.Unchanged);
                        VerifyEntitySetsMatch(testEntities, data.GetResourceSetEntities("Entities"));

                        // Change collection in both entities
                        List<string> tempCollection = ((XFeatureTestsEntity)testEntities[0]).Strings;
                        ((XFeatureTestsEntity)testEntities[0]).Strings = ((XFeatureTestsEntity)testEntities[1]).Strings;
                        ((XFeatureTestsEntity)testEntities[1]).Strings = tempCollection;
                        ctx.UpdateObject(testEntities[0]);
                        ctx.UpdateObject(testEntities[1]);
                        VerifyStateOfEntities(ctx, testEntities, EntityStates.Modified);
                        ctx.SaveChanges(saveOptions);
                        VerifyStateOfEntities(ctx, testEntities, EntityStates.Unchanged);
                        VerifyEntitySetsMatch(testEntities, data.GetResourceSetEntities("Entities"));

                        // Delete entities
                        ctx.DeleteObject(testEntities[0]);
                        ctx.DeleteObject(testEntities[1]);
                        VerifyStateOfEntities(ctx, testEntities, EntityStates.Deleted);
                        ctx.SaveChanges(saveOptions);
                        testEntities.RemoveAt(0);
                        testEntities.RemoveAt(0);
                        VerifyEntitySetsMatch(testEntities, data.GetResourceSetEntities("Entities"));
                    }
                });
            }

            private static void VerifyStateOfEntities(DataServiceContext ctx, IEnumerable<object> entities, EntityStates expectedState)
            {
                Debug.Assert(ctx != null, "ctx != null");
                Debug.Assert(entities != null, "entities != null");

                foreach (object entity in entities)
                {
                    EntityDescriptor descriptor = ctx.GetEntityDescriptor(entity);
                    Debug.Assert(descriptor != null, "EntityDescriptor not found for the given entity");
                    Assert.AreEqual(expectedState, descriptor.State);
                }
            }

            #endregion XFeature - Collection & BatchWithSingleChangeset with IDataServiceHost/IDataServiceHost2 and Change Tracking

            #region XFeature - Collection & Server Driven Paging

            // Cross feature end to end (server + client) tests for collection combined with server driven paging (built-in and custom). 
            // [TestCategory("Partition1"), TestMethod]
            public void Collection_ServerDrivenPaging()
            {
                Action<DataServiceContext, DSPContext, int?, bool> test = (ctx, data, pageSize, customPaging) => 
                {
                    DataServiceQueryContinuation<XFeatureTestsEntity> continuation = null;
                    int serverEntitiesCount = data.GetResourceSetEntities("Entities").Count();
                    int entityCount = 0;
                    int pagesCount = 0;

                    QueryOperationResponse<XFeatureTestsEntity> result = (QueryOperationResponse<XFeatureTestsEntity>)ctx.CreateQuery<XFeatureTestsEntity>("Entities").Execute();

                    do
                    {
                        pagesCount++;
                        if (continuation != null)
                        {
                            result = ctx.Execute(continuation);
                        }

                        foreach (var entity in result)
                        {
                            entityCount++;
                            ValidateResult(
                                entity, 
                                data.GetResourceSetEntities("Entities").SingleOrDefault(e => ((DSPResource)e).GetValue("ID") != null && (int)((DSPResource)e).GetValue("ID") == entity.ID),
                                true);
                        }
                    }
                    while ((continuation = result.GetContinuation()) != null);

                    Assert.AreEqual(serverEntitiesCount, entityCount, "the number of materialized entities is different than number of entities on the server.");
                    Assert.AreEqual(pagesCount, GetExpectedPageCount(pageSize, serverEntitiesCount, customPaging), "Unexpected number of pages");
                };

                CollectionAndServerDrivenPagingTestRunner(test);
            }

            // Cross feature end to end (server + client) tests for collection combined with server driven paging (built-in and custom). 
            // [TestCategory("Partition1"), TestMethod]
            public void Collection_ServerDrivenPaging_DataServiceCollection()
            {
                Action<DataServiceContext, DSPContext, int?, bool> test = (ctx, data, pageSize, customPaging) =>
                {
                    DataServiceCollection<XFeatureTestsEntity> dsc = 
                        new DataServiceCollection<XFeatureTestsEntity>(ctx.CreateQuery<XFeatureTestsEntity>("Entities").Execute(), TrackingMode.None);

                    int serverEntitiesCount = data.GetResourceSetEntities("Entities").Count();
                        
                    int pagesCount = 1;

                    while(dsc.Continuation != null)
                    {
                        pagesCount++;
                        dsc.Load(ctx.Execute<XFeatureTestsEntity>(dsc.Continuation));
                    }

                    Assert.AreEqual(serverEntitiesCount, dsc.Count, "the number of materialized entities is different than number of entities on the server.");
                    Assert.AreEqual(pagesCount, GetExpectedPageCount(pageSize, serverEntitiesCount, customPaging), "Unexpected number of pages");
                    VerifyEntitySetsMatch(new List<object>(dsc), (IList<object>)data.GetResourceSetEntities("Entities").Cast<object>());
                };

                CollectionAndServerDrivenPagingTestRunner(test);
            }

            private static int GetExpectedPageCount(int? pageSize, int numberOfEntities, bool customPaging)
            {
                // no paging - always 1 page
                if (pageSize == null)
                {
                    return 1;
                }

                if (!customPaging)
                {
                    return 1 + numberOfEntities / (int)pageSize;
                }
                else
                {
                    // in case of custom paging there is no link to the following page if the results ends on page boundary
                    return (numberOfEntities % pageSize != 0 ? 1 : 0) + numberOfEntities / (int)pageSize;
                }
            }

            private static void CollectionAndServerDrivenPagingTestRunner(Action<DataServiceContext, DSPContext, int?, bool> test)
            {
                DSPMetadata metadata = CreateMetadataForXFeatureEntity();

                TestUtil.RunCombinations(
                    new int?[] { null, 1, 2, 7, 100 }, 
                    new bool[] { false, true },
                    (pageSize, enableCustomPaging) =>
                {
                    DSPServiceDefinition service = new DSPServiceDefinition() { 
                        Metadata = metadata, 
                        Writable = true,
                        EnableCustomPaging = enableCustomPaging && pageSize != null /*pageSize == null means "no paging at all" */
                    };

                    if (pageSize != null)
                    {
                        if (!enableCustomPaging)
                        {
                            service.PageSizeCustomizer = (config, serviceType) => config.SetEntitySetPageSize("Entities", (int)pageSize);
                        }
                        else
                        {
                            CountManager.MaxCount = (int)pageSize;
                        }
                    }

                    using (TestWebRequest request = service.CreateForInProcessWcf())
                    {
                        DSPContext data = new DSPContext();
                        service.CreateDataSource = (m) => { return data; };
                        request.StartService();

                        DataServiceContext ctx = new DataServiceContext(new Uri(request.BaseUri), ODataProtocolVersion.V4);
                        //ctx.EnableAtom = true;
                        //ctx.Format.UseAtom();
                        PopulateClientContextWithTestEntities(ctx);

                        test(ctx, data, pageSize, enableCustomPaging);
                    }
                });
            }

            private static void VerifyEntitySetsMatch(IList<object> clientEntitySet, IList<object> serverEntitySet)
            {
                Assert.AreEqual(clientEntitySet.Count, serverEntitySet.Count, "number of items on the client is different than number of items on the server.");

                foreach (object clientEntity in clientEntitySet)
                {
                    string entityId = clientEntity.GetType().GetProperty("ID").GetValue(clientEntity, null).ToString();
                    DSPResource serverEntity = (DSPResource)serverEntitySet.SingleOrDefault(e => ((DSPResource)e).GetValue("ID") != null && ((DSPResource)e).GetValue("ID").ToString() == entityId);
                    Assert.IsNotNull(serverEntity, string.Format("Entity with ID: '{0}' not found on the server.", entityId));
                    ValidateResult(clientEntity, serverEntity, true);
                }
            }

            #endregion XFeature - Collection & Server Driven Paging

            #region XFeature - Collection & Interceptors

            public class InterceptorService : DSPDataService
            {
                [QueryInterceptor("Entities")]
                public Expression<Func<DSPResourceWithCollectionProperty, bool>> EntitiesQueryInterceptor()
                {
                    InterceptorServiceDefinition.Current.QueryInterceptorCallCount++;
                    return dspResource => ((int)dspResource.GetValue("ID") != 2);
                }

                [ChangeInterceptor("Entities")]
                public void EntitiesChangeInterceptor(DSPResourceWithCollectionProperty resource, UpdateOperations operations)
                {
                    if (InterceptorServiceDefinition.Current.EnableChangeInterceptors)
                    {
                        Assert.IsFalse(InterceptorServiceDefinition.Current.ChangeInterceptorCalledOnEntityId.HasValue, "Change interceptor called more than once.");
                        if (operations == UpdateOperations.Add)
                        {
                            // The new resource has no properties set on it yet, so assume the "new" one - 42
                            InterceptorServiceDefinition.Current.ChangeInterceptorCalledOnEntityId = 42;
                        }
                        else
                        {
                            InterceptorServiceDefinition.Current.ChangeInterceptorCalledOnEntityId = (int)resource.GetValue("ID");
                        }
                    }
                }
            }

            public class InterceptorServiceDefinition : DSPServiceDefinition
            {
                public InterceptorServiceDefinition() { this.DataServiceType = typeof(InterceptorService); }
                public int QueryInterceptorCallCount { get; set; }
                public bool EnableChangeInterceptors { get; set; }
                public int? ChangeInterceptorCalledOnEntityId { get; set; }
                public static new InterceptorServiceDefinition Current { get { return (InterceptorServiceDefinition)DSPServiceDefinition.Current; } }
            }

            // [TestCategory("Partition1"), TestMethod, Variation("Verifies that collection properties work correctly with query interceptors")]
            public void Collection_QueryInterceptors()
            {
                var metadata = CreateMetadataForXFeatureEntity();

                InterceptorServiceDefinition service = new InterceptorServiceDefinition()
                {
                    Metadata = metadata,
                    CreateDataSource = (m) => new DSPContext(),
                    Writable = true
                };
                using (TestWebRequest request = service.CreateForInProcessWcf())
                {
                    request.StartService();

                    DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                    //ctx.EnableAtom = true;
                    //ctx.Format.UseAtom();
                    PopulateClientContextWithTestEntities(ctx);

                    ctx.IgnoreResourceNotFoundException = true;

                    // Client test cases
                    QueryInterceptors_VerifyClientEntity(1, ctx.Execute<XFeatureTestsEntity>(new Uri("/Entities", UriKind.Relative)).AsEnumerable().First(), service);
                    QueryInterceptors_VerifyClientEntity(1, ctx.CreateQuery<XFeatureTestsEntity>("Entities").AsEnumerable().First(), service);
                    QueryInterceptors_VerifyClientEntity(1, ctx.CreateQuery<XFeatureTestsEntity>("Entities").OrderBy(e => e.ID).AsEnumerable().First(), service);
                    QueryInterceptors_VerifyClientEntity(1, ctx.CreateQuery<XFeatureTestsEntity>("Entities").Where(e => e.ID == 1).First(), service);
                    QueryInterceptors_VerifyClientEntity(null, ctx.CreateQuery<XFeatureTestsEntity>("Entities").Where(e => e.ID == 2).FirstOrDefault(), service);
                    // .Where(e => e.ID < 3) means "consider just first two entities". In this case we skip the first entity and the second is not there so null is expected
                    QueryInterceptors_VerifyClientEntity(null, ctx.CreateQuery<XFeatureTestsEntity>("Entities").Where(e => e.ID < 3).Skip(1).FirstOrDefault(), service);
                    QueryInterceptors_VerifyClientEntity(1, ctx.CreateQuery<XFeatureTestsEntity>("Entities").Select(e =>
                        new XFeatureTestsEntity { Strings = e.Strings }).AsEnumerable().First(), service);
                    QueryInterceptors_VerifyClientEntity(1, ctx.CreateQuery<XFeatureTestsEntity>("Entities").Select(e =>
                        new XFeatureTestsEntity { Structs = e.Structs }).AsEnumerable().First(), service);
                    QueryInterceptors_VerifyClientEntity(1, ctx.CreateQuery<XFeatureTestsEntity>("Entities").Select(e =>
                        new XFeatureTestsEntity { ID = e.ID }).AsEnumerable().First(), service);

                    // Server test cases (queries like this can't be executed through client API)
                    QueryInterceptors_VerifyServerRequest(200, "/Entities(1)/Strings", request, service);
                    QueryInterceptors_VerifyServerRequest(404, "/Entities(2)/Strings", request, service);
                    QueryInterceptors_VerifyServerRequest(200, "/Entities(1)/Structs", request, service);
                    QueryInterceptors_VerifyServerRequest(404, "/Entities(2)/Structs", request, service);
                }
            }

            private void QueryInterceptors_VerifyClientEntity(int? entityId, XFeatureTestsEntity entity, InterceptorServiceDefinition service)
            {
                QueryInterceptors_VerifyQueryInterceptorCalled(service);
                if (entityId.HasValue)
                {
                    Assert.AreEqual(entityId.Value, entity.ID, "The entity with ID 2 should have been filtered out.");
                }
                else
                {
                    Assert.IsNull(entity, "No entity should have been returned.");
                }
            }

            private void QueryInterceptors_VerifyServerRequest(int statusCode, string uri, TestWebRequest request, InterceptorServiceDefinition service)
            {
                request.RequestUriString = uri;
                TestUtil.RunCatching(request.SendRequest);
                QueryInterceptors_VerifyQueryInterceptorCalled(service);
                Assert.AreEqual(statusCode, request.ResponseStatusCode, "Unexpected status code returned.");
            }

            private void QueryInterceptors_VerifyQueryInterceptorCalled(InterceptorServiceDefinition service)
            {
                Assert.AreEqual(1, service.QueryInterceptorCallCount, "The query interceptor should have been called just once");
                service.QueryInterceptorCallCount = 0;
            }

            // [TestCategory("Partition1"), TestMethod, Variation("Verifies that collection properties work correctly with change interceptors.")]
            public void Collection_ChangeInterceptors()
            {
                var metadata = CreateMetadataForXFeatureEntity();

                InterceptorServiceDefinition service = new InterceptorServiceDefinition()
                {
                    Metadata = metadata,
                    CreateDataSource = (m) => new DSPContext(),
                    Writable = true,
                    EnableChangeInterceptors = true
                };

                // client cases
                TestUtil.RunCombinations(new string[] { "POST", "PUT", "PATCH", "DELETE" }, new bool[] { false, true }, (httpMethod, batch) =>
                {
                    using (DSPResourceWithCollectionProperty.CollectionPropertiesResettable.Restore())
                    using (TestWebRequest request = service.CreateForInProcessWcf())
                    {
                        DSPResourceWithCollectionProperty.CollectionPropertiesResettable.Value = true;
                        request.Accept = "application/atom+xml,application/xml";
                        request.StartService();

                        DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                        //ctx.EnableAtom = true;
                        //ctx.Format.UseAtom();

                        if (httpMethod != "POST")
                        {
                            service.EnableChangeInterceptors = false;
                            PopulateClientContextWithTestEntities(ctx);
                            service.EnableChangeInterceptors = true;
                        }

                        ctx.IgnoreResourceNotFoundException = true;

                        var resource = ctx.CreateQuery<XFeatureTestsEntity>("Entities").FirstOrDefault();
                        SaveChangesOptions saveOptions = batch ? SaveChangesOptions.BatchWithSingleChangeset : SaveChangesOptions.None;
                        switch (httpMethod)
                        {
                            case "POST":
                                resource = new XFeatureTestsEntity() { ID = 42, Strings = new List<string>(), Structs = new List<XFeatureTestsComplexType>() };
                                ctx.AddObject("Entities", resource);
                                break;
                            case "PUT":
                                saveOptions |= SaveChangesOptions.ReplaceOnUpdate;
                                ctx.UpdateObject(resource);
                                break;
                            case "PATCH":
                                ctx.UpdateObject(resource);
                                break;
                            case "DELETE":
                                ctx.DeleteObject(resource);
                                break;
                        }
                        ctx.SaveChanges(saveOptions);

                        Assert.AreEqual((int?)resource.ID, service.ChangeInterceptorCalledOnEntityId, "The change interceptor was not called or it was called with a wrong entity");
                        service.ChangeInterceptorCalledOnEntityId = null;
                    }
                });

                service.EnableChangeInterceptors = true;
                service.ChangeInterceptorCalledOnEntityId = null;

                // server cases (these operations can't be done using client API)
                TestUtil.RunCombinations(
                    new string[] { "Strings", "Structs" }, 
                    new string[] { UnitTestsUtil.MimeApplicationXml}, 
                    (collectionPropertyName, format) =>
                {
                    using (DSPResourceWithCollectionProperty.CollectionPropertiesResettable.Restore())
                    using (TestWebRequest request = service.CreateForInProcessWcf())
                    {
                        DSPResourceWithCollectionProperty.CollectionPropertiesResettable.Value = true;
                        request.StartService();

                        DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                        //ctx.EnableAtom = true;
                        //ctx.Format.UseAtom();
                        service.EnableChangeInterceptors = false;
                        PopulateClientContextWithTestEntities(ctx);
                        service.EnableChangeInterceptors = true;

                        // Get the collection property payload
                        var payload = UnitTestsUtil.GetResponseAsAtomXLinq(request, "/Entities(1)/" + collectionPropertyName, format);

                        // And send a PUT with that payload back
                        request.HttpMethod = "PUT";
                        request.Accept = format;
                        request.RequestContentType = format;
                        request.RequestUriString = "/Entities(1)/" + collectionPropertyName;
                        request.SetRequestStreamAsText(payload.ToString());
                        request.SendRequest();

                        Assert.AreEqual((int?)1, service.ChangeInterceptorCalledOnEntityId, "The change interceptor was not called or it was called with a wrong entity");
                        service.ChangeInterceptorCalledOnEntityId = null;
                    }
                });
            }

            #endregion XFeature - Collection & Interceptors
            #region XFeature - Collection & Blobs/MLEs
            // [TestCategory("Partition1"), TestMethod, Variation("Verifies that MLEs with collection properties work correctly (server and client)")]
            public void Collection_Blobs()
            {
                DSPMetadata metadata = CreateMetadataForXFeatureEntity(true);

                DSPServiceDefinition service = new DSPServiceDefinition() { 
                    Metadata = metadata, 
                    Writable = true, 
                    SupportMediaResource = true,
                    MediaResourceStorage = new DSPMediaResourceStorage()
                };

                byte[] clientBlob = new byte[] { 0xcc, 0x10, 0x00, 0xff };

                DSPContext data = new DSPContext();
                service.CreateDataSource = (m) => { return data; };

                using (TestWebRequest request = service.CreateForInProcessWcf())
                using (DSPResourceWithCollectionProperty.CollectionPropertiesResettable.Restore())
                {
                    DSPResourceWithCollectionProperty.CollectionPropertiesResettable.Value = true;
                    request.StartService();

                    XFeatureTestsMLE clientMle = new XFeatureTestsMLE() {
                        ID = 1,
                        Description = "Entity 1",
                        Strings = new List<string>(new string[] { "string 1", "string 2", string.Empty }),
                        Structs = new List<XFeatureTestsComplexType>(new XFeatureTestsComplexType[] {
                                    new XFeatureTestsComplexType() { Text = "text 1" },
                                    new XFeatureTestsComplexType() { Text = "text 2" }}) };


                    DataServiceContext ctx = new DataServiceContext(new Uri(request.BaseUri), ODataProtocolVersion.V4);
                    //ctx.EnableAtom = true;
                    //ctx.Format.UseAtom();

                    ctx.AddObject("Entities", clientMle);
                    ctx.SetSaveStream(clientMle, new MemoryStream(clientBlob), true, "application/octet-stream", clientMle.ID.ToString());
                    ctx.SaveChanges();
                    VerifyMLEs(service, clientMle, clientBlob);

                    // Read stream and verify stream contents
                    using (Stream serverStream = ctx.GetReadStream(clientMle).Stream)
                    {
                        VerifyStream(clientBlob, serverStream);
                    }

                    // modify MLE and the corresponding stream 
                    clientMle.Structs.Add(new XFeatureTestsComplexType() { Text = "text 3" });
                    clientMle.Strings.RemoveAt(0);
                    clientBlob[0] ^= 0xff;
                    ctx.UpdateObject(clientMle);
                    ctx.SetSaveStream(clientMle, new MemoryStream(clientBlob), true, "application/octet-stream", clientMle.ID.ToString());
                    ctx.SaveChanges();
                    VerifyMLEs(service, clientMle, clientBlob);

                    // delete MLE
                    ctx.DeleteObject(clientMle);
                    ctx.SaveChanges();

                    Assert.IsNull((DSPResource)service.CurrentDataSource.GetResourceSetEntities("Entities").
                            FirstOrDefault(e => (int)(((DSPResource)e).GetValue("ID")) == (int)clientMle.GetType().GetProperty("ID").GetValue(clientMle, null)),
                            "MLE has not been deleted.");

                    Assert.AreEqual(0, service.MediaResourceStorage.Content.Count(), "The stream on the server has not been deleted.");
                };
            }

            private static void VerifyMLEs(DSPServiceDefinition service, object clientMle, byte[] clientBlob)
            {
                DSPResource serverMle = (DSPResource)service.CurrentDataSource.GetResourceSetEntities("Entities").FirstOrDefault(
                    e => (int)(((DSPResource)e).GetValue("ID")) == (int)clientMle.GetType().GetProperty("ID").GetValue(clientMle, null));

                Assert.IsNotNull(serverMle, "MLE not saved correctly on the server");
                ValidateResult(clientMle, serverMle, true);

                DSPMediaResource serverBlob;
                service.MediaResourceStorage.TryGetMediaResource(serverMle, null, out serverBlob);

                bool isEmptyStream;
                using (Stream serverStream = serverBlob.GetReadStream(out isEmptyStream))
                {
                    Assert.IsFalse(isEmptyStream, "Unexpected empty stream");
                    VerifyStream(clientBlob, serverStream);
                }
            }

            private static void VerifyStream(byte[] clientBlob, Stream serverStream)
            {
                byte[] buffer = new byte[clientBlob.Length];
                int bytesRead = serverStream.Read(buffer, 0, buffer.Length);
                Assert.AreEqual(clientBlob.Length, bytesRead, "Server blob is smaller than the client blob");
                Assert.AreEqual(-1, serverStream.ReadByte(), "Server blob is bigger than the client blob");
                Assert.IsTrue(clientBlob.SequenceEqual<byte>(buffer), "Client and server blobs are different.");
            }

            #endregion XFeature - Collection & Blobs/MLEs

            #region XFeature - Collection & Configuration

            internal class ConfigurationTestCase
            {
                public Action<OpenWebDataServiceDefinition> SetupService { get; set; }
                public string RequestUri { get; set; }
                public string ExpectedExceptionMessage { get; set; }
                public Action<TestWebRequest> VerifyResponse { get; set; }
            }

            // [TestCategory("Partition1"), TestMethod, Variation("Verifies that collections work correctly with configuration options")]
            public void Collection_Configuration()
            {
                var testCases = new ConfigurationTestCase[] {
                    // AcceptCountRequests
                    new ConfigurationTestCase{
                        SetupService = (s) => { s.DataServiceBehavior.AcceptCountRequests = true; },
                        RequestUri = "/Entities/$count",
                    },
                    new ConfigurationTestCase{
                        SetupService = (s) => { s.DataServiceBehavior.AcceptCountRequests = false; },
                        RequestUri = "/Entities/$count",
                        ExpectedExceptionMessage = "The ability of the data service to return row count information is disabled. To enable this functionality, set the DataServiceConfiguration.AcceptCountRequests property to true."
                    },
                    new ConfigurationTestCase{
                        SetupService = (s) => { s.DataServiceBehavior.AcceptCountRequests = false; },
                        RequestUri = "/Entities?$count=true",
                        ExpectedExceptionMessage = "The ability of the data service to return row count information is disabled. To enable this functionality, set the DataServiceConfiguration.AcceptCountRequests property to true."
                    },

                    // AcceptProjectionRequests
                    new ConfigurationTestCase{
                        SetupService = (s) => { s.DataServiceBehavior.AcceptProjectionRequests = true; },
                        RequestUri = "/Entities?$select=Strings",
                    },
                    new ConfigurationTestCase{
                        SetupService = (s) => { s.DataServiceBehavior.AcceptProjectionRequests = false; },
                        RequestUri = "/Entities?$select=Strings",
                        ExpectedExceptionMessage = "The ability to use the $select query option to define a projection in a data service query is disabled. To enable this functionality, set the DataServiceConfiguration. AcceptProjectionRequests property to true."
                    },

                    // IncludeAssociationLinksInResponse
                    new ConfigurationTestCase{
                        SetupService = (s) => { s.DataServiceBehavior.IncludeRelationshipLinksInResponse = false; },
                        RequestUri = "/Entities",
                        VerifyResponse = (r) => { 
                            Assert.IsTrue(UnitTestsUtil.GetResponseAsAtomXLinq(r).Descendants(UnitTestsUtil.AtomNamespace + "entry")
                                .Elements(UnitTestsUtil.AtomNamespace + "link").Where(l => l.Attribute("rel").Value.Contains("relatedlinks/NextTestEntity")).Count() == 0,
                                "The payload contains relationship links even though they were turned off."); }
                    },
                    new ConfigurationTestCase{
                        SetupService = (s) => { s.DataServiceBehavior.IncludeRelationshipLinksInResponse = true; },
                        RequestUri = "/Entities",
                        VerifyResponse = (r) => { 
                            Assert.IsTrue(UnitTestsUtil.GetResponseAsAtomXLinq(r).Descendants(UnitTestsUtil.AtomNamespace + "entry")
                                .Elements(UnitTestsUtil.AtomNamespace + "link").Where(l => l.Attribute("rel").Value.Contains("relatedlinks/NextTestEntity")).Count() > 0,
                                "The payload doesn't contain relationship links even though they were turned on."); }
                    },

                    // Max results per collection
                    new ConfigurationTestCase{
                        SetupService = (s) => { s.MaxResultsPerCollection = 1; },
                        RequestUri = "/Entities",
                        ExpectedExceptionMessage = "The response exceeds the maximum 1 results per collection."
                    },
                    new ConfigurationTestCase{
                        SetupService = (s) => { s.MaxResultsPerCollection = 1; },
                        RequestUri = "/Entities?$filter=ID eq 1",
                        VerifyResponse = (r) => {
                            Assert.AreEqual(3, UnitTestsUtil.GetResponseAsAtomXLinq(r).Root.Element(UnitTestsUtil.AtomNamespace + "entry")
                                .Element(UnitTestsUtil.AtomNamespace + "content").Element(UnitTestsUtil.MetadataNamespace + "properties")
                                .Element(UnitTestsUtil.DataNamespace + "Strings").Elements(UnitTestsUtil.MetadataNamespace + "element").Count(),
                                "The collection should contain 3 items.");
                        }
                    },
                };

                TestUtil.RunCombinations(testCases, (testCase) =>
                {
                    var metadata = CreateMetadataForXFeatureEntity();
                    DSPServiceDefinition service = new DSPServiceDefinition()
                    {
                        Metadata = metadata,
                        CreateDataSource = (m) => new DSPContext(),
                        Writable = true,
                        ForceVerboseErrors = true
                    };
                    testCase.SetupService(service);

                    using (DSPResourceWithCollectionProperty.CollectionPropertiesResettable.Restore())
                    using (TestWebRequest request = service.CreateForInProcessWcf())
                    {
                        DSPResourceWithCollectionProperty.CollectionPropertiesResettable.Value = true;
                        request.StartService();

                        DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                        //ctx.EnableAtom = true;
                        //ctx.Format.UseAtom();
                        PopulateClientContextWithTestEntities(ctx);

                        request.RequestUriString = testCase.RequestUri;
                        request.Accept = "application/atom+xml,application/xml,text/plain";
                        Exception e = TestUtil.RunCatching(request.SendRequest);
                        if (testCase.ExpectedExceptionMessage == null)
                        {
                            Assert.IsNull(e, "Request should have succeeded.");
                            if (testCase.VerifyResponse != null)
                            {
                                testCase.VerifyResponse(request);
                            }
                        }
                        else
                        {
                            string message = request.GetResponseStreamAsText();
                            Assert.IsTrue(message.Contains(testCase.ExpectedExceptionMessage), "The exception message is not the one expected.");
                        }
                    }
                });
            }

            #endregion XFeature - Collection & Configuration

            #region XFeature - Collection & IExpandProvider

            // [TestCategory("Partition1"), TestMethod, Variation("Verifies that collection can be correctly expanded with IExpandProvider")]
            public void Collection_IExpandProvider()
            {
                var metadata = CreateMetadataForXFeatureEntity();

                DSPServiceDefinition service = new DSPServiceDefinition()
                {
                    Metadata = metadata,
                    CreateDataSource = (m) => new DSPContext(),
                    Writable = true,
                    SupportIExpandProvider = true
                };

                string[] responseFormats = new string[] { UnitTestsUtil.AtomFormat };

                TestUtil.RunCombinations(responseFormats, (format) =>
                {
                    using (TestWebRequest request = service.CreateForInProcessWcf())
                    {
                        request.StartService();

                        DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                        //ctx.EnableAtom = true;
                        //ctx.Format.UseAtom();
                        PopulateClientContextWithTestEntities(ctx);

                        // set navigation
                        ((XFeatureTestsEntity)ctx.Entities.Last().Entity).NextTestEntity = (XFeatureTestsEntity)ctx.Entities.First().Entity;
                        ctx.SetLink(ctx.Entities.Last().Entity, "NextTestEntity", ctx.Entities.First().Entity);
                        ctx.SaveChanges();

                        request.RequestUriString = "/Entities?$expand=NextTestEntity";
                        request.Accept = format;
                        request.SendRequest();
                        XDocument resp = UnitTestsUtil.GetResponseAsAtomXLinq(request);

                        Assert.IsNotNull(resp.Descendants(UnitTestsUtil.MetadataNamespace + "inline").Elements(UnitTestsUtil.AtomNamespace + "entry").FirstOrDefault(), "No expanded entities in the payload.");

                        foreach (XFeatureTestsEntity entity in ctx.Entities.Select(e => e.Entity))
                        {
                            var entityXElement = (from e in resp.Element(UnitTestsUtil.AtomNamespace + "feed").Elements(UnitTestsUtil.AtomNamespace + "entry")
                                                  where (int)e.Element(UnitTestsUtil.AtomNamespace + "content")
                                                     .Element(UnitTestsUtil.MetadataNamespace + "properties")
                                                     .Element(UnitTestsUtil.DataNamespace + "ID") == entity.ID
                                                  select e).Single();

                            ValidateResult(entity, entityXElement);
                        }
                    }
                });
            }

            private static void ValidateResult(object expected, XElement payload)
            {
                if (expected == null)
                {
                    Assert.AreEqual("true", (string)payload.Attribute(UnitTestsUtil.MetadataNamespace + "null"));
                    return;
                }

                Type expectedType = expected.GetType();

                if (UnitTestsUtil.IsPrimitiveType(expectedType))
                {
                    Assert.AreNotEqual("true", (string)payload.Attribute(UnitTestsUtil.MetadataNamespace + "null"));
                    Assert.AreEqual(expected.ToString(), (string)payload);
                    return;
                }

                if (expectedType.GetInterface("System.Collections.Generic.ICollection`1") != null)
                {
                    Assert.IsNotNull(expected, "Collection cannot be null");
                    Assert.AreNotEqual("true", (string)payload.Attribute(UnitTestsUtil.MetadataNamespace + "null"));

                    IEnumerator expectedCollection = (expected as IEnumerable).GetEnumerator();
                    IEnumerator collectionElements = payload.Elements(UnitTestsUtil.MetadataNamespace + "element").GetEnumerator();

                    do
                    {
                        bool expectedMoved = expectedCollection.MoveNext();
                        bool actualMoved = collectionElements.MoveNext();

                        Assert.AreEqual(expectedMoved, actualMoved, "Missing or unexpected items in the materialized collection");

                        if (!expectedMoved)
                        {
                            return;
                        }

                        ValidateResult(expectedCollection.Current, (XElement)collectionElements.Current);
                    }
                    while (true);
                }

                foreach (PropertyInfo piExpected in expected.GetType().GetProperties())
                {
                    // For top level properties need to get to the m:properties element 
                    // while for nested complex types and collection we just need get the nested property
                    XElement payloadProperty =
                        (payload.Name == UnitTestsUtil.AtomNamespace + "entry" ?
                            payload.Element(UnitTestsUtil.AtomNamespace + "content")
                            .Element(UnitTestsUtil.MetadataNamespace + "properties") :
                            payload)
                            .Element(UnitTestsUtil.DataNamespace + piExpected.Name);

                    if (payloadProperty != null)
                    {
                        // regular property
                        ValidateResult(piExpected.GetValue(expected, null), payloadProperty);
                    }
                    else
                    {
                        // navigation property
                        XElement navProp = payload.Elements(UnitTestsUtil.AtomNamespace + "link")
                                                .Where(e => (string)e.Attribute("rel") == UnitTestsUtil.NavigationLinkNamespace.NamespaceName + piExpected.Name)
                                                .SingleOrDefault();
                        if(navProp != null)
                        {
                            // is related entity expanded?
                            if (piExpected.GetValue(expected, null) != null)
                            {
                                XElement relatedEntity = navProp.Element(UnitTestsUtil.MetadataNamespace + "inline").Element(UnitTestsUtil.AtomNamespace + "entry");
                                Debug.Assert(relatedEntity != null, "Navigation properties to collections not supported.");
                                ValidateResult(piExpected.GetValue(expected, null), relatedEntity);                                
                            }
                            else
                            {
                                XElement inline = navProp.Element(UnitTestsUtil.MetadataNamespace + "inline");
                                Assert.IsNull(inline == null ? inline : inline.Elements().FirstOrDefault(), "Entity should not have been expanded.");
                            }
                        }
                        else
                        {
                            Assert.Fail(string.Format("No corresponding property for '{0}' found in the payload.", piExpected.Name));
                        }
                    }
                }
            }

            private static void ValidateResult(object expected, object actual, bool serverValue)
            {
                if (expected == null)
                {
                    Assert.IsNull(actual);
                    return;
                }
                else
                {
                    Assert.IsNotNull(actual);
                }

                Type expectedType = expected.GetType();

                if (UnitTestsUtil.IsPrimitiveType(expectedType))
                {
                    Assert.AreEqual(expected, actual);
                    return;
                }

                if (expectedType.GetInterface("System.Collections.Generic.ICollection`1") != null)
                {
                    Assert.IsNotNull(expected, "Collection cannot be null");
                    Assert.IsNotNull(actual, "Collection cannot be null");

                    IEnumerator expectedCollection = (expected as IEnumerable).GetEnumerator();
                    IEnumerator actualCollection = (actual as IEnumerable).GetEnumerator();

                    do
                    {
                        bool expectedMoved = expectedCollection.MoveNext();
                        bool actualMoved = actualCollection.MoveNext();

                        Assert.AreEqual(expectedMoved, actualMoved, "Missing or unexpected items in the materialized collection");

                        if (!expectedMoved)
                        {
                            return;
                        }

                        ValidateResult(expectedCollection.Current, actualCollection.Current, serverValue);
                    }
                    while (true);

                }

                foreach (PropertyInfo piExpected in expected.GetType().GetProperties())
                {
                    if (serverValue)
                    {
                        DSPResource resource = (DSPResource)actual;
                        ValidateResult(piExpected.GetValue(expected, null), resource.GetValue(piExpected.Name), serverValue);
                    }
                    else
                    {
                        PropertyInfo piActual = actual.GetType().GetProperty(piExpected.Name);
                        System.Diagnostics.Debug.Assert(piActual != null, "Property should have been found. Expected and actual types are the same");

                        ValidateResult(piExpected.GetValue(expected, null), piActual.GetValue(actual, null), serverValue);
                    }
                }
            }


            #endregion XFeature - Collection & IExpandProvider

            #region XFeature - Collection & ProcessingPipeline

            internal class ProcessingPipelineTestCase
            {
                public Action<TestWebRequest, string> SetupRequest { get; set; }
                public ProcessingPipelineCallCount ExpectedCallCount { get; set; }
            }

            // [TestCategory("Partition1"), TestMethod, Variation("Verifies that collections work correctly with processing pipeline.")]
            public void Collection_ProcessingPipeline()
            {
                var metadata = CreateMetadataForXFeatureEntity();

                Func<int, DSPResource> CreateNewXFeatureEntityResource = (id) =>
                {
                    DSPResourceWithCollectionProperty newResource = new DSPResourceWithCollectionProperty(metadata.GetResourceType("XFeatureTestsEntity"));
                    newResource.SetRawValue("ID", id);
                    newResource.SetRawValue("Description", "Second");
                    newResource.SetRawValue("Strings", new List<string>() { "One", "Two" });
                    newResource.SetRawValue("Structs", new List<DSPResource>());
                    return newResource;
                };

                var actualCallCount = new ProcessingPipelineCallCount();

                DSPServiceDefinition service = new DSPServiceDefinition()
                {
                    Metadata = metadata,
                    CreateDataSource = (m) =>
                    {
                        DSPContext context = new DSPContext();
                        context.GetResourceSetEntities("Entities").Add(CreateNewXFeatureEntityResource(0));
                        return context;
                    },
                    Writable = true,
                };
                service.ProcessingPipeline.ProcessingRequest = (sender, args) => { actualCallCount.ProcessingRequestCallCount++; };
                service.ProcessingPipeline.ProcessedRequest = (sender, args) => { actualCallCount.ProcessedRequestCallCount++; };
                service.ProcessingPipeline.ProcessingChangeset = (sender, args) => { actualCallCount.ProcessingChangesetCallCount++; };
                service.ProcessingPipeline.ProcessedChangeset = (sender, args) => { actualCallCount.ProcessedChangesetCallCount++; };

                var testCases = new ProcessingPipelineTestCase[]
                {
                    new ProcessingPipelineTestCase() {
                        SetupRequest = (r, format) => { r.HttpMethod = "GET"; r.RequestUriString = "/Entities"; r.Accept = format; },
                    },
                    new ProcessingPipelineTestCase() {
                        SetupRequest = (r, format) => { r.HttpMethod = "GET"; r.RequestUriString = "/Entities(0)/Strings"; r.Accept = format == UnitTestsUtil.AtomFormat ? UnitTestsUtil.MimeApplicationXml : format; },
                    },
                    new ProcessingPipelineTestCase() {
                        SetupRequest = (r, format) => { r.HttpMethod = "GET"; r.RequestUriString = "/Entities(0)/Structs"; r.Accept = format == UnitTestsUtil.AtomFormat ? UnitTestsUtil.MimeApplicationXml : format; },
                    },
                    new ProcessingPipelineTestCase() {
                        SetupRequest = (r, format) => {
                            r.HttpMethod = "POST";
                            r.RequestUriString = "/Entities";
                            r.Accept = format;
                            r.RequestContentType = format;
                            r.SetRequestStreamAsText(DSPResourceSerializer.WriteEntity(
                                CreateNewXFeatureEntityResource(1), 
                                DSPResourceSerializer.SerializerFormatFromMimeType(format)));
                        },
                        ExpectedCallCount = new ProcessingPipelineCallCount() {
                            ProcessingChangesetCallCount = 1,
                            ProcessedChangesetCallCount = 1
                        }
                    },
                    new ProcessingPipelineTestCase() {
                        SetupRequest = (r, format) => {
                            r.HttpMethod = "PUT";
                            r.RequestUriString = "/Entities(0)";
                            r.Accept = format;
                            r.RequestContentType = format;
                            r.SetRequestStreamAsText(DSPResourceSerializer.WriteEntity(
                                CreateNewXFeatureEntityResource(0), 
                                DSPResourceSerializer.SerializerFormatFromMimeType(format)));
                        },
                        ExpectedCallCount = new ProcessingPipelineCallCount() {
                            ProcessingChangesetCallCount = 1,
                            ProcessedChangesetCallCount = 1
                        }
                    },
                    new ProcessingPipelineTestCase() {
                        SetupRequest = (r, format) => {
                            r.HttpMethod = "PATCH";
                            r.RequestUriString = "/Entities(0)";
                            r.Accept = format;
                            r.RequestContentType = format;
                            r.SetRequestStreamAsText(DSPResourceSerializer.WriteEntity(
                                CreateNewXFeatureEntityResource(0), 
                                DSPResourceSerializer.SerializerFormatFromMimeType(format)));
                        },
                        ExpectedCallCount = new ProcessingPipelineCallCount() {
                            ProcessingChangesetCallCount = 1,
                            ProcessedChangesetCallCount = 1
                        }
                    },
                    new ProcessingPipelineTestCase() {
                        SetupRequest = (r, format) => {
                            r.HttpMethod = "DELETE";
                            r.RequestUriString = "/Entities(0)";
                            r.Accept = format;
                        },
                        ExpectedCallCount = new ProcessingPipelineCallCount() {
                            ProcessingChangesetCallCount = 1,
                            ProcessedChangesetCallCount = 1
                        }
                    },
                    new ProcessingPipelineTestCase() {
                        SetupRequest = (r, format) => {
                            r.HttpMethod = "PUT";
                            r.RequestUriString = "/Entities(0)/Strings";
                            r.Accept = format == UnitTestsUtil.AtomFormat ? UnitTestsUtil.MimeApplicationXml : format;
                            r.RequestContentType = format == UnitTestsUtil.AtomFormat ? UnitTestsUtil.MimeApplicationXml : format;
                            r.SetRequestStreamAsText(DSPResourceSerializer.WriteProperty(
                                metadata.GetResourceType("XFeatureTestsEntity").Properties.First(p => p.Name == "Strings"),
                                new List<string> { "Foo", "Bar" },
                                DSPResourceSerializer.SerializerFormatFromMimeType(format)));
                        },
                        ExpectedCallCount = new ProcessingPipelineCallCount() {
                            ProcessingChangesetCallCount = 1,
                            ProcessedChangesetCallCount = 1
                        }
                    },
                    new ProcessingPipelineTestCase() {
                        SetupRequest = (r, format) => {
                            r.HttpMethod = "PUT";
                            r.RequestUriString = "/Entities(0)/Structs";
                            r.Accept = format == UnitTestsUtil.AtomFormat ? UnitTestsUtil.MimeApplicationXml : format;
                            r.RequestContentType = format == UnitTestsUtil.AtomFormat ? UnitTestsUtil.MimeApplicationXml : format;
                            r.SetRequestStreamAsText(DSPResourceSerializer.WriteProperty(
                                metadata.GetResourceType("XFeatureTestsEntity").Properties.First(p => p.Name == "Structs"),
                                new List<DSPResource>(),
                                DSPResourceSerializer.SerializerFormatFromMimeType(format)));
                        },
                        ExpectedCallCount = new ProcessingPipelineCallCount() {
                            ProcessingChangesetCallCount = 1,
                            ProcessedChangesetCallCount = 1
                        }
                    },
                };

                using (DSPResourceWithCollectionProperty.CollectionPropertiesResettable.Restore())
                using (TestWebRequest request = service.CreateForInProcess())
                {
                    DSPResourceWithCollectionProperty.CollectionPropertiesResettable.Value = true;

                    TestUtil.RunCombinations(testCases, UnitTestsUtil.BooleanValues, UnitTestsUtil.ResponseFormats, (testCase, batch, format) =>
                    {
                        service.ClearChanges();
                        actualCallCount.Clear();
                        var expectedCallCount = new ProcessingPipelineCallCount(testCase.ExpectedCallCount);
                        // Each request must fire at least one ProcessingRequest and ProcessedRequest
                        expectedCallCount.ProcessingRequestCallCount++;
                        expectedCallCount.ProcessedRequestCallCount++;

                        if (batch)
                        {
                            InMemoryWebRequest batchPart = new InMemoryWebRequest();
                            testCase.SetupRequest(batchPart, format);
                            BatchWebRequest batchRequest = new BatchWebRequest();
                            if (batchPart.HttpMethod == "GET")
                            {
                                batchRequest.Parts.Add(batchPart);
                            }
                            else
                            {
                                var changeset = new BatchWebRequest.Changeset();
                                changeset.Parts.Add(batchPart);
                                batchRequest.Changesets.Add(changeset);
                            }
                            batchRequest.SendRequest(request);
                        }
                        else
                        {
                            request.RequestContentType = null;
                            request.RequestStream = null;
                            testCase.SetupRequest(request, format);
                            request.SendRequest();
                        }

                        actualCallCount.AssertEquals(expectedCallCount);
                    });
                }
            }

            #endregion XFeature - Collection & ProcessingPipeline

            #region XFeature - Collection & Count

            // [TestCategory("Partition1"), TestMethod, Variation("Verifies that collections work correctly with client and server side count and inlinecount.")]
            public void Collection_Count()
            {
                TestUtil.RunCombinations(new int?[] { null, 1, 2 }, (sdpPageSize) =>
                {
                    DSPServiceDefinition service = new DSPServiceDefinition()
                    {
                        Metadata = CreateMetadataForXFeatureEntity(),
                        CreateDataSource = (m) => new DSPContext(),
                        Writable = true,
                        PageSizeCustomizer = (config, type) => 
                        { 
                            if (sdpPageSize.HasValue) { config.SetEntitySetPageSize("Entities", sdpPageSize.Value); }
                        }
                    };

                    using (TestWebRequest request = service.CreateForInProcessWcf())
                    {
                        request.StartService();
                        var ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                        //ctx.EnableAtom = true;
                        //ctx.Format.UseAtom();
                        PopulateClientContextWithTestEntities(ctx);

                        var testCases = new[] {
                            new {  // Just entity set
                                Query = (DataServiceQuery<XFeatureTestsEntity>)ctx.CreateQuery<XFeatureTestsEntity>("Entities"),
                                TotalCount = 3,
                                QueryCount = 3
                            },
                            new {  // $filter
                                Query = (DataServiceQuery<XFeatureTestsEntity>)ctx.CreateQuery<XFeatureTestsEntity>("Entities").Where(e => e.ID < 2),
                                TotalCount = 1,
                                QueryCount = 1
                            },
                            new {  // $top
                                Query = (DataServiceQuery<XFeatureTestsEntity>)ctx.CreateQuery<XFeatureTestsEntity>("Entities").Take(1),
                                TotalCount = 3,
                                QueryCount = 1
                            },
                            new {  // $skip
                                Query = (DataServiceQuery<XFeatureTestsEntity>)ctx.CreateQuery<XFeatureTestsEntity>("Entities").Skip(1),
                                TotalCount = 3,
                                QueryCount = 2
                            },
                            new {  // $orderby
                                Query = (DataServiceQuery<XFeatureTestsEntity>)ctx.CreateQuery<XFeatureTestsEntity>("Entities").OrderBy(e => e.Description),
                                TotalCount = 3,
                                QueryCount = 3
                            },
                            new {  // $select
                                Query = (DataServiceQuery<XFeatureTestsEntity>)ctx.CreateQuery<XFeatureTestsEntity>("Entities")
                                    .Select(e => new XFeatureTestsEntity() { Strings = e.Strings }),
                                TotalCount = 3,
                                QueryCount = 3
                            },
                            new {  // $expand
                                Query = (DataServiceQuery<XFeatureTestsEntity>)ctx.CreateQuery<XFeatureTestsEntity>("Entities").Expand("NextTestEntity"),
                                TotalCount = 3,
                                QueryCount = 3
                            },
                            new {  // $select and $expand
                                Query = (DataServiceQuery<XFeatureTestsEntity>)ctx.CreateQuery<XFeatureTestsEntity>("Entities")
                                    .Select(e => new XFeatureTestsEntity() { 
                                        NextTestEntity = e.NextTestEntity == null ? null : new XFeatureTestsEntity() { Description = e.NextTestEntity.Description } 
                                    }),
                                TotalCount = 3,
                                QueryCount = 3
                            },
                        };

                        TestUtil.RunCombinations(testCases, (testCase) =>
                        {
                            // $count
                            int actualCount = testCase.Query.Count();
                            int expectedCount = testCase.QueryCount;
                            Assert.AreEqual(expectedCount, actualCount, "The $count query didn't return correct number.");

                            // query $count
                            var response = (QueryOperationResponse<XFeatureTestsEntity>)testCase.Query.IncludeTotalCount().Execute();
                            Assert.AreEqual(testCase.TotalCount, response.TotalCount, "The $count query didn't return correct number.");

                            // The actual number of results returned
                            int expectedResultCount = expectedCount;
                            if (sdpPageSize.HasValue && sdpPageSize.Value < expectedResultCount) expectedResultCount = sdpPageSize.Value;
                            Assert.AreEqual(expectedResultCount, response.Count(), "The actual number of results returned doesn't match the expected count.");
                        });
                    }
                });
                
            }

            #endregion XFeature - Collection & Count
        }

        #region ProcessingPipelineCallCount
        internal class ProcessingPipelineCallCount
        {
            public int ProcessingRequestCallCount { get; set; }
            public int ProcessedRequestCallCount { get; set; }
            public int ProcessingChangesetCallCount { get; set; }
            public int ProcessedChangesetCallCount { get; set; }

            public ProcessingPipelineCallCount() { }
            public ProcessingPipelineCallCount(ProcessingPipelineCallCount other)
            {
                if (other == null) return;
                this.ProcessingRequestCallCount = other.ProcessingRequestCallCount;
                this.ProcessedRequestCallCount = other.ProcessedRequestCallCount;
                this.ProcessingChangesetCallCount = other.ProcessingChangesetCallCount;
                this.ProcessedChangesetCallCount = other.ProcessedChangesetCallCount;
            }

            public void Clear()
            {
                this.ProcessingRequestCallCount = 0;
                this.ProcessedRequestCallCount = 0;
                this.ProcessingChangesetCallCount = 0;
                this.ProcessedChangesetCallCount = 0;
            }

            public void AssertEquals(ProcessingPipelineCallCount expected)
            {
                Assert.AreEqual(expected.ProcessingRequestCallCount, this.ProcessingRequestCallCount, "ProcessingRequest was not fired the expected number of times.");
                Assert.AreEqual(expected.ProcessedRequestCallCount, this.ProcessedRequestCallCount, "ProcessedRequest was not fired the expected number of times.");
                Assert.AreEqual(expected.ProcessingChangesetCallCount, this.ProcessingChangesetCallCount, "ProcessingChangeset was not fired the expected number of times.");
                Assert.AreEqual(expected.ProcessedChangesetCallCount, this.ProcessedChangesetCallCount, "ProcessedChangeset was not fired the expected number of times.");
            }
        }
        #endregion ProcessingPipelineCallCount
    }
}
