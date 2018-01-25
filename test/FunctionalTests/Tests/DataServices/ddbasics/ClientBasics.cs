//---------------------------------------------------------------------
// <copyright file="ClientBasics.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.OData.Service;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Design.T4;
    using Microsoft.OData.Service.Providers;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using AstoriaUnitTests.Stubs.DataServiceProvider;
    using Microsoft.OData.Edm;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Suites.Data.Test;

    public static class DataServiceContextExtensions
    {
        public static IEnumerable<TElement> Execute<TElement>(this DataServiceContext context, string requestString)
        {
            Uri requestUri = new Uri(requestString, UriKind.RelativeOrAbsolute);
            return context.Execute<TElement>(requestUri);
        }
    }

    public partial class ClientModule
    {
        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/885
        /// <summary>This is a test class for update, insert and delete functionality.</summary>
        [Ignore] // Remove Atom
        // [TestClass]
        public class ClientBasics
        {
            private static SimpleWorkspace arubaWorkspace;
            private static DataServiceHost arubaHost;
            private static SimpleWorkspace northwindWorkspace;
            private static DataServiceHost northwindHost;
            private static SimpleWorkspace photoWorkspace;
            private static DataServiceHost photoHost;
            private static SimpleWorkspace ffWorkspace;
            private static DataServiceHost ffHost;
            private static SimpleWorkspace ffUpdatableWorkspace;
            private static DataServiceHost ffUpdatableHost;
            private static SimpleWorkspace customObjectWorkspace;
            private static DataServiceHost customObjectHost;

            public static bool TraceReasons { get; set; }

            private SimpleWorkspace ArubaWorkspace
            {
                get
                {
                    if (arubaWorkspace == null)
                    {
                        Utils.CreateWorkspaceForType(typeof(SimpleDataService<Aruba.ArubaContainer>), typeof(Aruba.ArubaContainer), "Aruba", out arubaWorkspace, out arubaHost, true);
                    }
                    return arubaWorkspace;
                }
            }

            private SimpleWorkspace NorthwindWorkspace
            {
                get
                {
                    if (northwindWorkspace == null)
                    {
                        Utils.CreateWorkspaceForType(typeof(SimpleDataService<northwind.northwindContext>), typeof(northwind.northwindContext), "Northwind", out northwindWorkspace, out northwindHost, true);
                    }
                    return northwindWorkspace;
                }
            }

            private SimpleWorkspace PhotoWorkspace
            {
                get
                {
                    if (photoWorkspace == null)
                    {
                        Utils.CreateWorkspaceForType(typeof(PhotoService), typeof(PhotoContext), "PhotoService", out photoWorkspace, out photoHost, true);
                    }
                    return photoWorkspace;
                }
            }

            private SimpleWorkspace CustomObjectWorkspace
            {
                get
                {
                    if (customObjectWorkspace == null)
                    {
                        Utils.CreateWorkspaceForType(typeof(CustomObjectService), typeof(ClrNamespace.CustomObjectContext), "CustomObjectWorkspace", out customObjectWorkspace, out customObjectHost, true);
                    }
                    return customObjectWorkspace;
                }
            }

            /*
             * DataServiceContext
             *  *Execute, *BeginExecute, *EndExecute
             *  *CreateQuery<T>
             *      *Execute
             *      *GetEnumerator
             *      Expand, Top, Skip, OrderBy, Filter, First, Single
             *      *OpenObject, *Typed
             *  *ExecuteBatch, *BeginExecuteBatch, *EndExecuteBatch
             *      *QueryResponse
             *      SendingRequest
             *      ReadingEntity
             *  *LoadProperty, *BeginLoadProperty, *EndLoadProperty
             *  *SaveChanges, *BeginSaveChanges, *EndSaveChanges
             *      *None, ContinueOnError, BatchWithSingleChangeset,
             *      UsePostTunneling
             *      *ChangesetResponse
             *      WritingEntity
             *  *Entities, *AddObject, AttachObject, UpdateObject, DeleteObject, DetachObject
             *  Links, AddLink, AttachLink, DeleteLink, DetachLink
             *  TryGetEntity, *TryGetUri
             *  Expand   (Reference, Collection { empty, 1, many }
             *  *ResolveType, *ResolveName,
             *  *CodeGeneration
             *      File, *Uri via GetMetadataUri
             *      *CSharp, *VB
             *      *Launch DataSvcUtil.exe, *Directly use System.Data.Entity.Design
             * */

            private static readonly Random random = new Random();

            [TestMethod]
            public void QueryAruba()
            {
                using (CachedConnections.SetupConnection(CachedConnections.ConnectionType.Aruba))
                {
                    TraceReasons = true;
                    SimpleWorkspace workspace = this.ArubaWorkspace;
                    #region QueryAruba
                    QueryWorkspace(workspace,
                        delegate(Uri baseUri)
                        {
                            return new ArubaClient.ArubaContainer(baseUri);
                        },
                        delegate(string typeName)
                        {
                            return typeof(ArubaClient.DefectBug).Assembly.GetType("ArubaClient." + typeName.Substring("Aruba.".Length), true, false);
                        },
                        delegate(Type type)
                        {
                            return "Aruba." + type.Name;
                        },
                        delegate(DataServiceContext context)
                        {
                            //context.Format.UseAtom();
                            //context.EnableAtom = true;
                            context.MergeOption = MergeOption.OverwriteChanges;
                            var q = context.CreateQuery<ArubaClient.OwnerContactInfo>("OwnerContactInfoSet(2)").Execute().Single();
                            var p = context.CreateQuery<ArubaClient.ContactInfo>("OwnerContactInfoSet(2)/ContactInfo").Execute().Single();
                            Assert.IsNotNull(q.ContactInfo);
                            Assert.IsNotNull(p);
                            Assert.AreEqual(q.ContactInfo.Email, p.Email);
                            Assert.AreEqual(q.ContactInfo.AddressInfo.City, p.AddressInfo.City);
                        });
                    #endregion
                }
            }

            [TestMethod]
            public void QueryNorthwind()
            {
                using (CachedConnections.SetupConnection(CachedConnections.ConnectionType.Northwind))
                {
                    TraceReasons = true;
                    SimpleWorkspace workspace = this.NorthwindWorkspace;
                    #region QueryNorthwind
                    QueryWorkspace(workspace,
                        delegate(Uri baseUri)
                        {
                            return new northwindClient.northwindContext(baseUri);
                        },
                        delegate(string typeName)
                        {
                            return typeof(ArubaClient.DefectBug).Assembly.GetType("northwindClient." + typeName.Substring("northwind.".Length), true, false);
                        },
                        delegate(Type type)
                        {
                            return "northwind." + type.Name;
                        },
                        delegate(DataServiceContext context)
                        {
                            // Consider: For a typed DataServiceContext, do we want to enforce for context.AddObject("entityset", object)
                            // where if context.GetProperty("entityset") is of IQueryable<T> that typeof(T).IsInstanceOfType(object)?
                            //context.Format.UseAtom();
                            //context.EnableAtom = true;

                            northwindClient.Customers customer = northwindClient.Customers.CreateCustomers("ASTOR", "Microsoft");
                            northwindClient.Orders order1 = northwindClient.Orders.CreateOrders(999);
                            northwindClient.Products product1 = northwindClient.Products.CreateProducts(9876, "WebService", false);
                            northwindClient.Products product2 = northwindClient.Products.CreateProducts(9879, "Database", false);
                            northwindClient.Region region = northwindClient.Region.CreateRegion(39, "Astoria");
                            northwindClient.Order_Details detail1 = northwindClient.Order_Details.CreateOrder_Details(999, 9876, 0.39M, 42, 0.5f);
                            northwindClient.Order_Details detail2 = northwindClient.Order_Details.CreateOrder_Details(999, 9879, 3.1415M, 13, 0.01f);

                            context.AddObject("Customers", customer);
                            context.AddObject("Orders", order1);

                            // TODO: remove this SaveChanges for a single that contains objects & links
                            SaveChanges(context);

                            context.AddObject("Products", product1);
                            context.AddObject("Products", product2);
                            context.AddObject("Region", region);

                            // TODO: enable this for non-batch
                            // TODO: enable this for batch
                            //context.AddObject("Order_Details", detail1);
                            //context.AddObject("Order_Details", detail2);


                            customer.Orders.Add(order1);
                            order1.Order_Details.Add(detail1);
                            order1.Order_Details.Add(detail2);

                            context.AddLink(customer, "Orders", order1);

                            // TODO: enable this for non-batch
                            // TODO: enable this for batch
                            //context.AddLink(order1, "Order_Details", detail1);
                            //context.AddLink(order1, "Order_Details", detail2);

                            SaveChanges(context);
                        });
                    #endregion
                }
            }

            private void QueryWorkspace(
                SimpleWorkspace workspace,
                Func<Uri, DataServiceContext> constructor,
                Func<string, Type> resolveType,
                Func<Type, string> resolveName,
                Action<DataServiceContext> createUpdateDelete)
            {
                Uri baseUri = new Uri(workspace.ServiceEndPoint + workspace.ServiceContainer.Name + ".svc", UriKind.Absolute);
                DataServiceContext ctx = constructor(baseUri);
                //ctx.Format.UseAtom();
                //ctx.EnableAtom = true;
                Trace.WriteLine("Querying workspace at " + baseUri);

                #region Confirm default context properties
                Assert.AreSame(baseUri, ctx.BaseUri);
                Assert.AreEqual(0, ctx.Timeout);
                Assert.AreEqual(MergeOption.AppendOnly, ctx.MergeOption);
                Assert.IsFalse(ctx.UsePostTunneling);
                Assert.IsNull(ctx.ResolveType);
                Assert.IsNull(ctx.Credentials);
                #endregion

                #region Set some properties to like ResolveType & ResolveName
                ctx.Timeout = TestConstants.MaxTestTimeout;
                ctx.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                ctx.ResolveType = resolveType;
                ctx.ResolveName = resolveName;
                #endregion

                int totalTypedObject = 0;
                int totalRequestCount = 0;
                int totalSendingRequestCount = 0;
                bool expectBadRequestBecauseMisinterpretedUri = false;

                #region generate client code
                ValidateCodeGenScenarios(ctx);

                ctx.SendingRequest2 += delegate(object sender, SendingRequest2EventArgs args)
                {
                    Assert.AreSame(ctx, sender);
                    Assert.IsNotNull(args.RequestMessage);
                    Assert.IsTrue(args.RequestMessage.Url.IsAbsoluteUri);
                    if (!args.IsBatchPart)
                        totalSendingRequestCount++;

                    Assert.IsTrue(args.RequestMessage.Method == "GET" || args.RequestMessage.Method == "PATCH" || args.RequestMessage.Method == "POST", "expecting GET or PATCH operation");
                    if (!args.RequestMessage.Url.OriginalString.EndsWith("/$batch", StringComparison.Ordinal))
                    {
                        LastUriRequest = args.RequestMessage.Url;
                    }

                    Assert.IsNotNull(LastUriRequest);

                    expectBadRequestBecauseMisinterpretedUri = false;

                    if (LastUriRequest.IsAbsoluteUri)
                    {
                        string pathAndQuery = LastUriRequest.PathAndQuery;
                        if (0 <= pathAndQuery.IndexOf(':') || (0 < pathAndQuery.IndexOf("%3A")))
                        {
                            //TraceReason("PathAndQuery contain ':'");
                            //expectBadRequestBecauseMisinterpretedUri = true;
                        }
                        else if (260 < LastUriRequest.OriginalString.Length)    // TODO: reenable when it works
                        {
                            TraceReason("260 < Uri.Length");
                            expectBadRequestBecauseMisinterpretedUri = true;
                        }
                        else
                        {
                            foreach (String segment in LastUriRequest.Segments)
                            {
                                if (0 <= segment.IndexOf('('))
                                {
                                    if (0 > segment.IndexOf(')'))
                                    {
                                        TraceReason("Uri decoded '/'");
                                        expectBadRequestBecauseMisinterpretedUri = true;
                                        break;
                                    }
                                    else if (0 <= segment.IndexOf('.'))
                                    {
                                        //TraceReason("PathAndQuery contain '.'");
                                        //expectBadRequestBecauseMisinterpretedUri = true;
                                        break;
                                    }
                                }
                                else if (0 <= segment.IndexOf(')'))
                                {
                                    TraceReason("PathAndQuery contain ')' without '('");
                                    expectBadRequestBecauseMisinterpretedUri = true;
                                    break;
                                }
                            }
                        }
                    }
                };

                #endregion

                List<string> resourceContainerNames = (from cont in workspace.ServiceContainer.ResourceContainers
                                                       select cont.Name).ToList<string>();

                #region Test Typed object requests
                List<string> resourceContainerNamesWithoutProperty = new List<String>(resourceContainerNames);

                WhichExecuteMethod = 0;
                int entityCount = 0;
                int previousEntityCount = 0;
                foreach (PropertyInfo property in ctx.GetType().GetProperties())
                {
                    if (typeof(DataServiceQuery).IsAssignableFrom(property.PropertyType))
                    {
                        bool expectFailure = false;
                        if (!resourceContainerNamesWithoutProperty.Remove(property.Name))
                        {
                            Assert.IsFalse(resourceContainerNames.Contains(property.Name), "asking for resourceContainerName a second time");
                            TraceReason("asking for resourceContainerName a second time");
                            expectFailure = true;
                        }

                        #region Test CreateQuery via strongly typed property
                        try
                        {
                            totalRequestCount++;
                            DataServiceQuery query = (DataServiceQuery)property.GetValue(ctx, null);
                            foreach (object x in DataServiceContextTestUtil.ExecuteQuery(ctx, query, (QueryMode)(WhichExecuteMethod++ % 7)))
                            {
                                //  Assert.AreSame(x, from y in ctx.Entities where y.Resource == x select y.Resource);
                                totalTypedObject++;
                                entityCount++;
                            }

                            Assert.IsFalse(expectFailure, "expected the non-existant container {0} would fail", property.Name);
                        }
                        catch (AssertFailedException)
                        {
                            throw;
                        }
                        catch
                        {
                            if (!expectFailure)
                            {
                                throw;  // instead of Assert.IsTrue(expectFailure)
                            }
                        }

                        var entities = ctx.Entities;
                        Assert.AreEqual(entityCount, entities.Count, "number of entities enumerated (" + entityCount + ") different than entities stored"); // using AppendOnly
                        for (int i = previousEntityCount + 8; i < entities.Count; i++)
                        {
                            ctx.Detach(entities[i].Entity);
                            entityCount--;
                        }

                        previousEntityCount = entityCount;

                        #endregion
                    }
                }

                Assert.AreEqual(0, resourceContainerNamesWithoutProperty.Count, "not all resourceContainerNames have associated property");

                Assert.AreEqual(0, ctx.Links.Count);
                Assert.IsTrue(7 <= WhichExecuteMethod, "didn't use all execute methods");
                #endregion

                Assert.IsFalse(expectBadRequestBecauseMisinterpretedUri);

                #region Test load property
                int EntityIndexIncrement = 2;
                int PropertyCountPerEntity = 1;

                Dictionary<Type, PropertyInfo[]> props = new Dictionary<Type, PropertyInfo[]>();
                do
                {
                    ReadOnlyCollection<EntityDescriptor> entities = ctx.Entities;
                    for (int entityIndex = 0; entityIndex < entities.Count; entityIndex += EntityIndexIncrement)
                    {
                        EntityDescriptor entity = entities[entityIndex];

                        string resourceContainerName;
                        #region Determine the resourceContainerName for the entity
                        Uri entityUri;
                        ctx.TryGetUri(entity.Entity, out entityUri);

                        // Ideally we'd use the commented line, but System.Uri will incorrecty interpret
                        // A('b%2fc') as A('b/c'), so it will fail for some key values.
                        //
                        // resourceContainerName = entityUri.Segments[entityUri.Segments.Length - 1];
                        resourceContainerName = entityUri.Segments[2];

                        resourceContainerName = resourceContainerName.Substring(0, resourceContainerName.IndexOf('('));
                        #endregion

                        PropertyInfo[] properties;
                        #region Determine properties on entity
                        Type entityType = entity.Entity.GetType();
                        if (!props.TryGetValue(entityType, out properties))
                        {
                            properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                            props.Add(entityType, properties);
                        }
                        #endregion

                        List<string> resourceTypeProperties;
                        #region Determine properties on entity for entity set
                        SimpleResourceContainer container = workspace.ServiceContainer.ResourceContainers[resourceContainerName];
                        SimpleResourceType z = (from x in container.ResourceTypes where x.Name == entityType.Name select x).Single();
                        resourceTypeProperties = (from x in z.Properties select x.Name).ToList();
                        #endregion

                        for (int i = 0; i < Math.Min(PropertyCountPerEntity, properties.Length); ++i)
                        {
                            bool expectFailure = false;
                            bool possibleMissingFailure = false;

                            // just because the type has a navigation property
                            // it doesn't mean its valid for all the entity sets that the type particpates in.
                            PropertyInfo property = properties[random.Next(0, properties.Length)];

                            if (property.DeclaringType.BaseType != typeof(object))
                            {   // can't expand property that doesn't exist on base class
                                expectFailure = true; // TODO: define the rules better
                                if (expectFailure)
                                {
                                    TraceReason("property doesn't exist on base class");
                                }
                            }
                            else if (!resourceTypeProperties.Contains(property.Name) ||
                                (("BugDefectTrackingSet" == resourceContainerName) &&
                                  (("AssignedToOwnerBugsProject" == property.Name) ||
                                   ("ResolvedOwnerBugsProject" == property.Name))) ||
                                (("BugProjectTrackingSet" == resourceContainerName) &&
                                  (("AssignedToOwnerBugsDefect" == property.Name) ||
                                   ("ResolvedOwnerBugsDefect" == property.Name))))
                            {   // MEST - the entity type participates in 2 entity sets which have different relationships
                                TraceReason("the entity type participates in 2 entity sets which have different relationships");
                                expectFailure = true;
                            }
                            else if (null == property.GetValue(entity.Entity, null))
                            {   // null values return 404 - NOT FOUND
                                possibleMissingFailure = true;
                            }

                            try
                            {
                                totalRequestCount++;
                                QueryOperationResponse response = ExpandProperty(ctx, entity.Entity, property.Name);

                                if (!expectFailure && expectBadRequestBecauseMisinterpretedUri)
                                {   // expectBadRequestOrNotFound is determined during the SendingRequest event
                                    TraceReason("expect bad request because misinterpreted uri");
                                    expectFailure = true;
                                }

                                //Assert.IsFalse(expectFailure, "expected failure for Container: {0}, Type: {1}, Property: {2}", resourceContainerName, entity.Entity.GetType(), property.Name);

                                if (response != null)
                                {
                                    Utils.IsSuccessResponse(response, HttpStatusCode.OK);
                                }
                            }
                            catch (AssertFailedException)
                            {
                                throw;
                            }
                            catch (Exception e)
                            {
                                WebException web = (e as WebException) ?? (e.InnerException as WebException);
                                if (!expectFailure && expectBadRequestBecauseMisinterpretedUri)
                                {   // expectBadRequestOrNotFound is determined during the SendingRequest event
                                    TraceReason("expectBadRequestOrNotFound is determined during the SendingRequest event");
                                    expectFailure = true;
                                }

                                if (!expectFailure && possibleMissingFailure)
                                {
                                    TraceReason("possibleMissingFailure actually failed");
                                    expectFailure = true;
                                }

                                if (!expectFailure)
                                {
                                    throw new Exception(String.Format("did not expect failure for '{0}'", LastUriRequest), e);
                                }
                            }

                            expectBadRequestBecauseMisinterpretedUri = false;
                            expectFailure = false;

                            GC.KeepAlive(entity);
                            GC.KeepAlive(property);
                        }
                    }
                } while (WhichExpandMethod < 6);
                #endregion

                Assert.AreEqual(totalRequestCount, totalSendingRequestCount, "tracking request count by event");

                if (null != createUpdateDelete)
                {
                    createUpdateDelete(ctx);
                }
            }

            private static void ValidateCodeGenScenarios(DataServiceContext ctx)
            {
                ValidateCodGen(ctx, ODataT4CodeGenerator.LanguageOption.CSharp, useDataServiceCollection: true);
                ValidateCodGen(ctx, ODataT4CodeGenerator.LanguageOption.CSharp, useDataServiceCollection: false);
                ValidateCodGen(ctx, ODataT4CodeGenerator.LanguageOption.VB, useDataServiceCollection: true);
                ValidateCodGen(ctx, ODataT4CodeGenerator.LanguageOption.VB, useDataServiceCollection: false);
            }

            private static void ValidateCodGen(DataServiceContext ctx, ODataT4CodeGenerator.LanguageOption languageOption, bool useDataServiceCollection)
            {
                string sourceFilename = GenerateClientCode(ctx.GetMetadataUri().AbsoluteUri, languageOption, useDataServiceCollection);
                Assembly generatedAssembly = CompileCode(sourceFilename);

                Type[] types = generatedAssembly.GetTypes();
                Type gtc = (from a in types where typeof(DataServiceContext).IsAssignableFrom(a) select a).SingleOrDefault();
                Assert.IsNotNull(gtc, "didn't generate context in assembly");

                DataServiceContext tmpCtx = (DataServiceContext)Activator.CreateInstance(gtc, ctx.BaseUri);
                //tmpCtx.Format.UseAtom();
                //tmpCtx.EnableAtom = true;
                tmpCtx.Timeout = ctx.Timeout;
                tmpCtx.Credentials = ctx.Credentials;

                Assert.IsNotNull(tmpCtx);
            }

            private static void TraceReason(string reason)
            {
                if (TraceReasons)
                {
                    Debug.WriteLine(reason);
                }
            }

            private static Uri LastUriRequest;

            #region Different ways to execute a query: GetEnumerator, Execute, BeginExecute, ExecuteBatch, BeginExecuteBatch
            private static int WhichExecuteMethod;
            #endregion

            #region Different ways to expand a query: LoadProperty, BeginLoadProperty, Expand
            private static int WhichExpandMethod;
            private static QueryOperationResponse ExpandProperty(DataServiceContext context, object entity, string property)
            {
                QueryOperationResponse response = null;
                switch (WhichExpandMethod++ % 4)
                {
                    case 0:
                        {
                            response = context.LoadProperty(entity, property);
                            break;
                        }

                    case 1:
                        {
                            IAsyncResult async = context.BeginLoadProperty(entity, property, null, null);
                            if (!async.CompletedSynchronously)
                            {
                                Assert.IsTrue(async.AsyncWaitHandle.WaitOne(new TimeSpan(0, 0, TestConstants.MaxTestTimeout), false), "BeginLoadProperty timeout");
                            }

                            Assert.IsTrue(async.IsCompleted);
                            response = context.EndLoadProperty(async);
                            break;
                        }

                    case 2:
                        {
                            LoadPropertyCallback callback = new LoadPropertyCallback();
                            IAsyncResult async = context.BeginLoadProperty(entity, property, callback.CallbackMethod, new object[] { property, context });

                            Assert.IsTrue(callback.Finished.WaitOne(new TimeSpan(0, 0, TestConstants.MaxTestTimeout), false), "Asyncallback timeout");
                            Assert.IsTrue(async.IsCompleted);

                            if (null != callback.CallbackFailure)
                            {
                                Assert.IsNull(callback.CallbackResult, callback.CallbackFailure.ToString());
                                throw new Exception("failure in callback", callback.CallbackFailure);
                            }

                            break;
                        }

                    case 3:
                        {
                            Assert.AreNotEqual<MergeOption>(MergeOption.NoTracking, context.MergeOption);


                            Uri uri;
                            context.TryGetUri(entity, out uri);

                            var edmType = DataServiceContextTestUtil.GetOrCreateEdmType(entity.GetType(), context.MaxProtocolVersion);
                            if (((IEdmStructuredType)edmType).FindProperty(property) is IEdmNavigationProperty)
                            {
                                // $expand no longer works for non-navigation properties after integrating the ODL URI parser.
                                // however, because they are not affected by $expand, the behavior is unchanged by leaving this out of the request.
                                uri = new Uri(uri.OriginalString + "?$expand=" + property);
                            }

                            DataServiceRequest request = (DataServiceRequest)UnitTestCodeGen.InvokeConstructor(typeof(DataServiceRequest<>).MakeGenericType(entity.GetType()), DataServiceContextTestUtil.TypesUri, uri);

                            int count = 0;
                            foreach (object a in DataServiceContextTestUtil.ExecuteQuery(context, request, (QueryMode)(WhichExecuteMethod++ % 7)))
                            {
                                count++;
                                Assert.AreSame(entity, a);
                            }

                            Assert.AreEqual(1, count, "didn't select existing entity: {0}", request);
                            break;
                        }

                    default:
                        Assert.Fail("shouldn't be here");
                        break;
                }

                return response;
            }


            #endregion

            #region Different ways to SaveChanges, BeginSaveChanges
            private static int WhichSaveChanges;
            private static void SaveChanges(DataServiceContext context)
            {
                SaveChangesOptions option = (SaveChangesOptions)random.Next(0, 3);
                SaveChangesMode mode = (SaveChangesMode)(WhichSaveChanges++ % 3);
                DataServiceContextTestUtil.SaveChanges(context, option, mode);
            }

            #endregion

            #region Different ways to generate and compile ADO.NET Data Services client code
            private static int WhichCodeGenerationMethod;

            public static string GreenBitsReferenceAssembliesDirectoryWPF
            {
                get
                {
                    string path = System.IO.Path.Combine(TestEnvironment.GreenBitsReferenceAssembliesDirectory, @"WPF");
                    if (!System.IO.Directory.Exists(path))
                    {
                        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Reference Assemblies\Microsoft\Framework\.NetFramework\v4.0");
                    }

                    return path;
                }
            }

            private static System.Reflection.Assembly CompileCode(string sourceFileName)
            {
                Dictionary<string, string> providerOptions = new Dictionary<string, string>();
                providerOptions.Add("CompilerVersion", "v4.0");

                System.CodeDom.Compiler.CodeDomProvider provider = null;
                if (sourceFileName.EndsWith(".vb", StringComparison.OrdinalIgnoreCase))
                {
                    provider = new Microsoft.VisualBasic.VBCodeProvider(providerOptions);
                }
                else if (sourceFileName.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                {
                    provider = new Microsoft.CSharp.CSharpCodeProvider(providerOptions);
                }
                else
                {
                    Assert.Fail("unrecognized file extension");
                }

                System.CodeDom.Compiler.CompilerParameters compilerOptions = new System.CodeDom.Compiler.CompilerParameters();
                compilerOptions.GenerateExecutable = false;
                compilerOptions.GenerateInMemory = true;
                compilerOptions.IncludeDebugInformation = false;
                compilerOptions.ReferencedAssemblies.Add(Path.Combine(TestEnvironment.GreenBitsReferenceAssembliesDirectory, "System.dll"));
                compilerOptions.ReferencedAssemblies.Add(Path.Combine(TestEnvironment.GreenBitsReferenceAssembliesDirectory, "System.Xml.dll"));
                compilerOptions.ReferencedAssemblies.Add(Path.Combine(GreenBitsReferenceAssembliesDirectoryWPF, "WindowsBase.dll"));
                compilerOptions.ReferencedAssemblies.Add(Path.Combine(TestEnvironment.GreenBitsReferenceAssembliesDirectory, "System.Core.dll"));
                compilerOptions.ReferencedAssemblies.Add(Environment.ExpandEnvironmentVariables(Path.Combine(Environment.CurrentDirectory, DataFxAssemblyRef.File.DataServicesClient)));
                compilerOptions.ReferencedAssemblies.Add(Environment.ExpandEnvironmentVariables(Path.Combine(Environment.CurrentDirectory, DataFxAssemblyRef.File.ODataLib)));
                compilerOptions.ReferencedAssemblies.Add(Environment.ExpandEnvironmentVariables(Path.Combine(Environment.CurrentDirectory, DataFxAssemblyRef.File.EntityDataModel)));
                compilerOptions.TreatWarningsAsErrors = true;
                compilerOptions.WarningLevel = 4;

                System.CodeDom.Compiler.CompilerResults results = provider.CompileAssemblyFromFile(compilerOptions, sourceFileName);
                foreach (System.CodeDom.Compiler.CompilerError error in results.Errors)
                {
                    Assert.Fail(error.ErrorText);
                }

                Assert.IsFalse(results.Errors.HasErrors);
                return results.CompiledAssembly;
            }

            private static string GenerateClientCode(string metadataUri, ODataT4CodeGenerator.LanguageOption languageOption, bool useDataServiceCollection)
            {
                string outputFile = GetTempFile((ODataT4CodeGenerator.LanguageOption.CSharp == languageOption ? "cs" : "vb"));

                ODataT4CodeGenerator codeGenerator = new ODataT4CodeGenerator { MetadataDocumentUri = metadataUri, TargetLanguage = languageOption, UseDataServiceCollection = useDataServiceCollection};
                string generatedCode = codeGenerator.TransformText();

                File.WriteAllText(outputFile, generatedCode);
                return outputFile;
            }

            private static void DeleteFile(string filename)
            {
                if (!String.IsNullOrEmpty(filename) && System.IO.File.Exists(filename))
                {
                    System.IO.File.Delete(filename);
                }
            }

            private static string GetTempFile(string extension)
            {
                string path = System.IO.Path.GetTempPath();
                string name = String.Format(
                        "{0}_{1}.{2}",
                        Assembly.GetExecutingAssembly().GetName().Name,
                        DateTime.UtcNow.Ticks,
                        extension);
                string filename = System.IO.Path.Combine(path, name);
                DeleteFile(filename);
                return filename;
            }

            #endregion

            [TestMethod]
            public void SaveCustomer()
            {
                using (CachedConnections.SetupConnection(CachedConnections.ConnectionType.Northwind))
                {
                    SimpleWorkspace workspace = this.NorthwindWorkspace;
                    Uri baseUri = new Uri(workspace.ServiceEndPoint + workspace.ServiceContainer.Name + ".svc", UriKind.Absolute);
                    DoAsyncType doAsync = new DoAsyncType();
                    SaveCustomer(baseUri, doAsync);

                    doAsync.Finished.WaitOne();

                    if (null != doAsync.CallbackFailure)
                    {
                        throw new Exception("failure in callback", doAsync.CallbackFailure);
                    }
                }
            }

            #region AsyncOnly SaveCustomer

            private const string CustomerID = "ASTQQ";

            private void SaveCustomer(Uri baseUri, DoAsyncType doAsync)
            {
                northwindClient.northwindContext ctx = new northwindClient.northwindContext(baseUri);
                //ctx.Format.UseAtom();
                //ctx.EnableAtom = true;
                ctx.Timeout = TestConstants.MaxTestTimeout;
                ctx.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
                ctx.UsePostTunneling = true;

                northwindClient.Customers cust = northwindClient.Customers.CreateCustomers(CustomerID, "ADO.NET Data Services");
                ctx.AddObject("Customers", cust);

                Func<AsyncCallback, object, IAsyncResult> begin = delegate(AsyncCallback callback, object state)
                {
                    return ctx.BeginSaveChanges(SaveChangesOptions.None, callback, state);
                };

                Action<IAsyncResult> end = delegate(IAsyncResult asyncResult)
                {
                    DataServiceResponse response = ctx.EndSaveChanges(asyncResult);
                    foreach (ChangeOperationResponse changeset in response)
                    {
                        if (changeset.Descriptor is EntityDescriptor)
                        {
                            EntityDescriptor entity = (EntityDescriptor)changeset.Descriptor;
                            if (null != changeset.Error) { throw new Exception(String.Format("failed to save {0} entity: {1}", asyncResult.AsyncState, changeset.Error.ToString())); }
                        }
                        else
                        {
                            LinkDescriptor link = (LinkDescriptor)changeset.Descriptor;
                            if (null != changeset.Error) { throw new Exception(String.Format("failed to save {0} link: {1}", asyncResult.AsyncState, changeset.Error.ToString())); }
                        }
                    }
                };

                doAsync.DoAsync(
                    begin,
                    end,
                    0,
                    delegate(string ignore1)
                    {
                        // verify insert
                        Uri custUri1 = null;
                        ctx.TryGetUri(cust, out custUri1);
                        if (null == custUri1 || String.IsNullOrEmpty(custUri1.ToString()))
                        {
                            throw new Exception("resource Uri is empty");
                        }

                        // update entity
                        cust.Address = "One Microsoft Way";
                        cust.ContactTitle = "owner";
                        ctx.UpdateObject(cust);

                        doAsync.DoAsync(
                            begin,
                            end,
                            1,
                            delegate(string ignore2)
                            {
                                ctx.TryGetUri(cust, out custUri1);
                                if (null == custUri1 || String.IsNullOrEmpty(custUri1.ToString()))
                                {
                                    throw new Exception("resource Uri is empty");
                                }

                                // add relations to entity


                                var singlecust =
                                    ((DataServiceQuery<northwindClient.Customers>)
                                    (from customer in ctx.Customers where customer.CustomerID == "AROUT" select customer))
                                    .Expand("Orders");

                                doAsync.DoAsync(
                                    singlecust.BeginExecute,
                                    delegate(IAsyncResult result)
                                    {
                                        foreach (northwindClient.Orders order in singlecust.EndExecute(result).Single().Orders)
                                        {
                                            cust.Orders.Add(order);
                                            ctx.AddLink(cust, "Orders", order);
                                        }

                                        if (6 != cust.Orders.Count)
                                        {
                                            throw new Exception("didn't add orders " + cust.Orders.Count.ToString());
                                        }
                                    },
                                    2,
                                    delegate(string ignore5)
                                    {
                                        doAsync.DoAsync(
                                            begin,
                                            end,
                                            3,
                                            delegate(string ignore3)
                                            {
                                                // verify relations were added
                                                Uri custUri2 = null;
                                                ctx.TryGetUri(cust, out custUri2);
                                                if (null == custUri2 || custUri1 != custUri2)
                                                {
                                                    throw new Exception("resource Uri changed");
                                                }

                                                ctx.MergeOption = Microsoft.OData.Client.MergeOption.NoTracking;

                                                DataServiceQuery<northwindClient.Customers> verifycust = ((DataServiceQuery<northwindClient.Customers>)
                                                    (from customer in ctx.Customers where CustomerID == customer.CustomerID select customer)).Expand("Orders");

                                                doAsync.DoAsync(
                                                    verifycust.BeginExecute,
                                                    delegate(IAsyncResult result)
                                                    {
                                                        int countOfOrders = verifycust.EndExecute(result).Single().Orders.Count();
                                                        if (6 != countOfOrders)
                                                        {
                                                            throw new Exception("didn't change database " + countOfOrders.ToString());
                                                        }
                                                    },
                                                    4,
                                                    delegate(string ignore7)
                                                    {
                                                        ctx.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;

                                                        // delete relations from entity
                                                        foreach (northwindClient.Orders order in cust.Orders)
                                                        {
                                                            ctx.DeleteLink(cust, "Orders", order);
                                                        }
                                                        cust.Orders.Clear();

                                                        doAsync.DoAsync(
                                                            begin,
                                                            end,
                                                            5,
                                                            delegate(string ignore4)
                                                            {
                                                                // verify links deleted
                                                                doAsync.DoAsync(
                                                                    delegate(AsyncCallback callback, object state)
                                                                    {
                                                                        return ctx.BeginLoadProperty(cust, "Orders", callback, state);
                                                                    },
                                                                    delegate(IAsyncResult result)
                                                                    {
                                                                        ctx.EndLoadProperty(result);
                                                                        if (0 < cust.Orders.Count)
                                                                        {
                                                                            throw new Exception("didn't commit removal of orders");
                                                                        }
                                                                    },
                                                                    6,
                                                                    delegate(string ignore6)
                                                                    {
                                                                        ctx.DeleteObject(cust);
                                                                        doAsync.DoAsync(
                                                                            begin,
                                                                            end,
                                                                            7,
                                                                            delegate(string ignore8)
                                                                            {
                                                                                doAsync.HandleStatus(null);
                                                                            });
                                                                    });
                                                            });
                                                    });
                                            });
                                    });
                            });
                    });
            }

            #endregion

            [TestMethod]
            public void CUDFailureUsingSaveChangesBatch()
            {
                using (CachedConnections.SetupConnection(CachedConnections.ConnectionType.Northwind))
                {
                    SimpleWorkspace workspace = this.NorthwindWorkspace;
                    Uri baseUri = new Uri(workspace.ServiceEndPoint + workspace.ServiceContainer.Name + ".svc", UriKind.Absolute);
                    DataServiceContext ctx = new northwindClient.northwindContext(baseUri);
                    //ctx.EnableAtom = true;
                    //ctx.Format.UseAtom();
                    ctx.Timeout = TestConstants.MaxTestTimeout;
                    Trace.WriteLine("Querying workspace at " + baseUri);

                    northwindClient.Customers customer1 = northwindClient.Customers.CreateCustomers("ABCDE", "DummyCompanyName");
                    northwindClient.Customers customer2 = northwindClient.Customers.CreateCustomers("ABCDE", "Var1");
                    ctx.AddObject("Customers", customer1);
                    ctx.AddObject("Customers", customer2);

                    try
                    {
                        ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
                    }
                    catch (DataServiceRequestException ex)
                    {
                        Utils.IsBatchResponse(ex.Response);

                        Assert.IsTrue(ex.InnerException != null, "expecting inner exception to be set");
                        List<ChangeOperationResponse> responses = new List<ChangeOperationResponse>();
                        foreach (ChangeOperationResponse response in ex.Response)
                        {
                            responses.Add(response);
                        }

                        Assert.IsTrue(responses.Count == 1, "expecting only one response, since this is a failure case in batch mode");

                        Utils.IsErrorResponse(responses[0], HttpStatusCode.InternalServerError, true);
                        //Assert.IsTrue(responses[0].Descriptor == null, "expecting no descriptor in case of error");
                    }
                }
            }

            [TestMethod]
            public void CUDFailureUsingSaveChangesNone()
            {
                using (CachedConnections.SetupConnection(CachedConnections.ConnectionType.Northwind))
                {
                    SimpleWorkspace workspace = this.NorthwindWorkspace;
                    Uri baseUri = new Uri(workspace.ServiceEndPoint + workspace.ServiceContainer.Name + ".svc", UriKind.Absolute);
                    DataServiceContext ctx = new northwindClient.northwindContext(baseUri);
                    //ctx.EnableAtom = true;
                    //ctx.Format.UseAtom();
                    ctx.Timeout = TestConstants.MaxTestTimeout;
                    Trace.WriteLine("Querying workspace at " + baseUri);

                    northwindClient.Customers customer1 = northwindClient.Customers.CreateCustomers("ALFKI", "DummyCompanyName");
                    ctx.AddObject("Customers", customer1);

                    try
                    {
                        ctx.SaveChanges();
                    }
                    catch (DataServiceRequestException ex)
                    {
                        Utils.IsNotBatchResponse(ex.Response);

                        Assert.IsTrue(ex.InnerException != null, "expecting inner exception to be set");
                        List<ChangeOperationResponse> responses = new List<ChangeOperationResponse>();
                        foreach (ChangeOperationResponse response in ex.Response)
                        {
                            responses.Add(response);
                        }

                        Assert.IsTrue(responses.Count == 1, "expecting only one response, since this is a failure case in batch mode");
                        Utils.IsErrorResponse(responses[0], HttpStatusCode.InternalServerError, false);
                    }
                }
            }

            [TestMethod]
            public void CUDFailureUsingSaveChangesContinueOnError()
            {
                using (CachedConnections.SetupConnection(CachedConnections.ConnectionType.Northwind))
                {
                    SimpleWorkspace workspace = this.NorthwindWorkspace;
                    Uri baseUri = new Uri(workspace.ServiceEndPoint + workspace.ServiceContainer.Name + ".svc", UriKind.Absolute);
                    DataServiceContext ctx = new northwindClient.northwindContext(baseUri);
                    //ctx.EnableAtom = true;
                    //ctx.Format.UseAtom();
                    ctx.Timeout = TestConstants.MaxTestTimeout;
                    Trace.WriteLine("Querying workspace at " + baseUri);

                    northwindClient.Customers customer1 = northwindClient.Customers.CreateCustomers("ABCDE", "DummyCompanyName");
                    northwindClient.Customers customer2 = northwindClient.Customers.CreateCustomers("ALFKI", "Var1");
                    ctx.AddObject("Customers", customer1);
                    ctx.AddObject("Customers", customer2);
                    try
                    {
                        ctx.SaveChanges(SaveChangesOptions.ContinueOnError);
                    }
                    catch (DataServiceRequestException ex)
                    {
                        Utils.IsNotBatchResponse(ex.Response);

                        Assert.IsTrue(ex.InnerException != null, "expecting inner exception to be set");
                        List<ChangeOperationResponse> responses = new List<ChangeOperationResponse>();
                        foreach (ChangeOperationResponse response in ex.Response)
                        {
                            responses.Add(response);
                        }

                        Assert.IsTrue(responses.Count == 2, "expecting only one response, since this is a failure case in batch mode");

                        // expecting the firt one to pass
                        Utils.IsSuccessResponse(responses[0], HttpStatusCode.Created);
                        Assert.IsTrue(responses[0].Descriptor.GetType() == typeof(EntityDescriptor), "expecting entity descriptor in case of insert");
                        Assert.IsTrue(((EntityDescriptor)responses[0].Descriptor).Entity == customer1, "expecting same entity instance");

                        // expecting the second one to fail
                        Utils.IsErrorResponse(responses[1], HttpStatusCode.InternalServerError, false);
                    }
                }
            }

            #region BLOB Support

            [TestMethod]
            public void BlobNoProjectedPropertiesTest()
            {
                using (PhotoContext.CreateChangeScope())
                {
                    SimpleWorkspace workspace = this.PhotoWorkspace;
                    Uri baseUri = new Uri(workspace.ServiceEndPoint + workspace.ServiceContainer.Name + ".svc", UriKind.Absolute);
                    DataServiceContext ctx = new DataServiceContext(baseUri);
                    //ctx.EnableAtom = true;
                    //ctx.Format.UseAtom();
                    ctx.Timeout = TestConstants.MaxTestTimeout;
                    Trace.WriteLine("Querying workspace at " + baseUri);

                    var photos = ctx.Execute<Photo>(baseUri + "/Items(1)?$select=Icon");
                    foreach (Photo p in photos)
                    {
                        Assert.AreEqual(baseUri.OriginalString + "/Items(1)", ctx.GetEntityDescriptor(p).Identity.OriginalString);
                        Photo empty = new Photo();
                        foreach (PropertyInfo pi in typeof(Photo).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                        {
                            Assert.AreEqual(pi.GetValue(empty, null), pi.GetValue(p, null));
                        }
                    }

                    var folders = ctx.Execute<Folder>(baseUri + "/Folders(0)?$select=Items");
                    foreach (Folder f in folders)
                    {
                        Assert.AreEqual(baseUri.OriginalString + "/Folders(0)", ctx.GetEntityDescriptor(f).Identity.OriginalString);
                        Folder empty = new Folder();
                        foreach (PropertyInfo pi in typeof(Folder).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                        {
                            if (!pi.PropertyType.IsGenericType)
                            {
                                Assert.AreEqual(pi.GetValue(empty, null), pi.GetValue(f, null));
                            }
                        }
                    }
                }
            }

            private static int SendingRequest2Count;

            [TestMethod]
            public void BLOBSupportTest()
            {
                using (PhotoContext.CreateChangeScope())
                {
                    SimpleWorkspace workspace = this.PhotoWorkspace;
                    Uri baseUri = new Uri(workspace.ServiceEndPoint + workspace.ServiceContainer.Name + ".svc", UriKind.Absolute);
                    DataServiceContext ctx = new DataServiceContext(baseUri);
                    //ctx.EnableAtom = true;
                    //ctx.Format.UseAtom();
                    ctx.Timeout = TestConstants.MaxTestTimeout;
                    Trace.WriteLine("Querying workspace at " + baseUri);

                    SendingRequest2Count = 0;
                    ctx.SendingRequest2 += new EventHandler<SendingRequest2EventArgs>(BlobCtx_SendingRequest);
                    ctx.ResolveName = type =>
                    {
                        if (type == typeof(PhotoMLE))
                        {
                            return typeof(Photo).FullName;
                        }

                        return type.FullName;
                    };
                    ctx.ResolveType = name =>
                    {
                        if (name == typeof(Photo).FullName)
                        {
                            return typeof(PhotoMLE);
                        }
                        else if (name == typeof(Item).FullName)
                        {
                            return typeof(Item);
                        }

                        return typeof(Folder);
                    };

                    // POST Item
                    string itemName = "Item 99";
                    string itemDescription = "Item 99 is cool!";
                    Item i = new Item()
                    {
                        ID = 99,
                        Name = itemName,
                        Description = itemDescription,
                        LastUpdated = DateTime.Now
                    };
                    ctx.AddObject("Items", i);

                    // POST Photo
                    string photoName = "Photo 100";
                    string photoDescription = "Photo 100 is even cooler!";
                    byte[] photoThumbNail = new byte[] { 0, 1, 2 };
                    byte[] photoContent = new byte[] { 0, 1, 2, 3 };
                    PhotoMLE p = new PhotoMLE()
                    {
                        ID = 100,
                        Name = photoName,
                        Description = photoDescription,
                        LastUpdated = DateTime.Now,
                        ThumbNail = photoThumbNail,
                        Rating = 1,
                        Content = photoContent
                    };
                    ctx.AddObject("Items", p);
                    ctx.SaveChanges();
                    Assert.AreEqual(SendingRequest2Count, 3, "SendingRequest2 must be called 3 times - once for Item and twice for MLE");

                    using (FileStream fs = File.OpenRead(DataServiceStreamProvider.GetStoragePath(new Photo() { ID = p.ID })))
                    {
                        int count = 0;
                        int b;
                        while ((b = fs.ReadByte()) != -1)
                        {
                            Assert.AreEqual((byte)b, p.Content[count++]);
                        }
                        Assert.AreEqual(p.Content.Length, count);
                    }

                    // POST Folder
                    string folderName = "Folder 101";
                    Folder f = new Folder() { ID = 101, Name = folderName };
                    ctx.AddObject("Folders", f);
                    ctx.AddLink(f, "Items", i);
                    ctx.AddLink(f, "Items", p);
                    ctx.SaveChanges();

                    // GET Folder Expand Item
                    var result = (QueryOperationResponse<Folder>)ctx.Execute<Folder>(baseUri.OriginalString + "/Folders(101)?$expand=Items");

                    Folder f101 = null;
                    foreach (Folder folder in result)
                    {
                        Assert.IsNull(f101);
                        f101 = folder;
                        Assert.AreEqual(folderName, f101.Name);
                        ctx.LoadProperty(f101, "Items");
                        Assert.AreEqual(2, f101.Items.Count);
                        foreach (Item item in f101.Items)
                        {
                            if (item.ID == 99)
                            {
                                Assert.AreEqual(itemName, item.Name);
                                Assert.AreEqual(itemDescription, item.Description);
                            }
                            else if (item.ID == 100)
                            {
                                PhotoMLE photo = (PhotoMLE)item;
                                Assert.AreEqual(photoName, photo.Name);
                                Assert.AreEqual(photoDescription, photo.Description);
                                Assert.AreEqual(1, photo.Rating);

                                Assert.AreEqual(3, photo.ThumbNail.Length);
                                for (int idx = 0; idx < photo.ThumbNail.Length; idx++)
                                {
                                    Assert.AreEqual((byte)idx, photo.ThumbNail[idx]);
                                }

                                ctx.LoadProperty(photo, "Content");
                                Assert.AreEqual(4, photo.Content.Length);
                                for (int idx = 0; idx < photo.Content.Length; idx++)
                                {
                                    Assert.AreEqual((byte)idx, photo.Content[idx]);
                                }
                            }
                            else
                            {
                                Assert.Fail("Unexpected Item, ID = '{0}'!", item.ID);
                            }
                        }
                    }

                    ctx.DeleteLink(f, "Items", i);
                    ctx.DeleteLink(f, "Items", p);
                    ctx.DeleteObject(i);
                    ctx.DeleteObject(p);
                    ctx.DeleteObject(f);

                    ctx.SaveChanges();

                    Assert.IsFalse(File.Exists(DataServiceStreamProvider.GetStoragePath(new Photo() { ID = p.ID })));
                }
            }

            private static void BlobCtx_SendingRequest(object sender, SendingRequest2EventArgs e)
            {
                Assert.IsTrue(e.Descriptor != null || e.RequestMessage.Method == "GET", "descriptor should never be null for CUD");
                Assert.IsNotNull(e.RequestMessage, "requestMessage is not null");
                Assert.IsFalse(e.IsBatchPart, "Since SaveChanges is called with no parameters, IsBatchPart should always be false");

                HttpWebRequestMessage requestMessage = (HttpWebRequestMessage)e.RequestMessage;
                Assert.IsNotNull(requestMessage.HttpWebRequest, "HttpWebRequest cannot be null");

                EntityDescriptor entityDescriptor = e.Descriptor as EntityDescriptor;
                if (entityDescriptor != null)
                {
                    PhotoMLE photoMLE = entityDescriptor.Entity as PhotoMLE;
                    if (photoMLE != null && requestMessage.Method == "POST")
                    {
                        e.RequestMessage.SetHeader("CustomRequestHeader_ItemType", typeof(Photo).FullName);
                        e.RequestMessage.SetHeader("Slug", photoMLE.ID.ToString());
                        Assert.AreEqual(requestMessage.HttpWebRequest.Headers["CustomRequestHeader_ItemType"], typeof(Photo).FullName, "HttpWebRequest should have CustomRequestHeader_ItemType");
                        Assert.AreEqual(requestMessage.HttpWebRequest.Headers["Slug"], photoMLE.ID.ToString(), "HttpWebRequest should have Slug header");
                    }
                    else if (entityDescriptor.Entity is Item)
                    {
                        e.RequestMessage.SetHeader("CustomRequestHeader_ItemType", typeof(Item).FullName);
                        Assert.AreEqual(requestMessage.HttpWebRequest.Headers["CustomRequestHeader_ItemType"], typeof(Item).FullName, "HttpWebRequest should have CustomRequestHeader_ItemType");
                    }
                }

                try
                {
                    e.RequestMessage.GetStream();
                    Assert.Fail("GetStream method should always fail during SendingRequest2 event");
                }
                catch (NotSupportedException)
                {
                    // do nothing as this is expected.
                }

                SendingRequest2Count++;
            }

            [TestMethod]
            public void BLOBV2_ReadMR()
            {
                using (Utils.RestoreStaticValueOnDispose(typeof(DataServiceStreamProvider), "UseAlternativeReadStreamUri"))
                {
                    DataServiceStreamProvider.UseAlternativeReadStreamUri = false;
                    foreach (bool async in new bool[] { false, true })
                    {
                        SimpleWorkspace workspace = this.PhotoWorkspace;
                        Uri baseUri = new Uri(workspace.ServiceEndPoint + workspace.ServiceContainer.Name + ".svc", UriKind.Absolute);
                        DataServiceContext ctx = new DataServiceContext(baseUri);
                        //ctx.EnableAtom = true;
                        //ctx.Format.UseAtom();
                        ctx.Timeout = TestConstants.MaxTestTimeout;
                        Trace.WriteLine("Querying workspace at " + baseUri);

                        // Verify content
                        foreach (Item item in ctx.Execute<Item>("/Items"))
                        {
                            Photo photo = item as Photo;
                            if (photo == null)
                            {
                                continue;
                            }
                            VerifySinglePhoto(photo, ctx, async);
                        }

                        // Verify headers are passed correctly
                        {
                            Photo photo = ctx.Execute<Item>("/Items").Where(item => item is Photo).Cast<Photo>().FirstOrDefault();
                            DataServiceRequestArgs args = new DataServiceRequestArgs();
                            string testcontent = "testcontent";
                            string testheader = "testheader";
                            args.Headers["Test_ReplyWithThisContent"] = testcontent;
                            args.Headers["Test_RoundtripHeader"] = testheader;
                            using (DataServiceStreamResponse response = GetReadStreamHelper(ctx, photo, args, async))
                            {
                                string realcontent = (new StreamReader(response.Stream, System.Text.Encoding.UTF8)).ReadToEnd();
                                Assert.AreEqual(testcontent, realcontent, "Custom header was not passed correctly.");
                                Assert.AreEqual(testheader, response.Headers["Test_RoundtripHeader"], "Response headers not passed correctly.");
                            }
                        }
                    }
                }
            }

            [TestMethod]
            public void BLOBV2_AddMR()
            {
                using (PhotoContext.CreateChangeScope())
                using (Utils.RestoreStaticValueOnDispose(typeof(DataServiceStreamProvider), "UseAlternativeReadStreamUri"))
                {
                    DataServiceStreamProvider.UseAlternativeReadStreamUri = false;
                    foreach (bool async in new bool[] { false, true })
                    {
                        SimpleWorkspace workspace = this.PhotoWorkspace;
                        Uri baseUri = new Uri(workspace.ServiceEndPoint + workspace.ServiceContainer.Name + ".svc", UriKind.Absolute);
                        DataServiceContext ctx = new DataServiceContext(baseUri);
                        //ctx.EnableAtom = true;
                        //ctx.Format.UseAtom();
                        ctx.Timeout = TestConstants.MaxTestTimeout;
                        Trace.WriteLine("Querying workspace at " + baseUri);
                        ctx.ResolveName = type =>
                        {
                            return type.FullName;
                        };

                        byte[] content1 = new byte[] { 1, 2, 3, 4, 5 };
                        byte[] content2 = new byte[] { 21, 22, 23, 24, 25, 26, 27 };
                        List<Photo> photos = new List<Photo>();
                        for (int i = 0; i < 6; i++)
                        {
                            Photo photo = new Photo();
                            photo.ID = 200 + i;
                            photo.Name = "Photo" + i.ToString();
                            ctx.AddObject("Items", photo);
                            DataServiceRequestArgs args = new DataServiceRequestArgs();
                            args.Slug = photo.ID.ToString();
                            args.ContentType = DataServiceStreamProvider.GetContentType(photo);
                            args.Headers["CustomRequestHeader_ItemType"] = typeof(Photo).FullName;
                            ctx.SetSaveStream(photo, new MemoryStream(i % 2 == 0 ? content1 : content2), true, args);
                            photos.Add(photo);
                        }
                        SaveChangesHelper(ctx, SaveChangesOptions.None, async);

                        // Refresh all entities - we have to do this so that the links are passed from the server to the client
                        //   This is a bug in the design where we don't parse the response to the MR POST
                        ctx.MergeOption = MergeOption.OverwriteChanges;
                        int c = ctx.Execute<Item>("/Items").Count();

                        for (int i = 0; i < 6; i++)
                        {
                            Photo photo = photos[i];
                            VerifySinglePhoto(photo, ctx, async);
                            using (DataServiceStreamResponse response = GetReadStreamHelper(ctx, photo, new DataServiceRequestArgs(), async))
                            {
                                VerifyStreamContent(new MemoryStream(i % 2 == 0 ? content1 : content2), response.Stream);
                            }
                        }

                        foreach (Photo photo in photos)
                        {
                            ctx.DeleteObject(photo);
                        }
                        SaveChangesHelper(ctx, SaveChangesOptions.None, async);
                    }
                }
            }

            [TestMethod]
            public void BLOBV2_UpdateMR()
            {
                using (PhotoContext.CreateChangeScope())
                using (Utils.RestoreStaticValueOnDispose(typeof(DataServiceStreamProvider), "UseAlternativeReadStreamUri"))
                {
                    DataServiceStreamProvider.UseAlternativeReadStreamUri = false;
                    foreach (bool async in new bool[] { false, true })
                    {
                        SimpleWorkspace workspace = this.PhotoWorkspace;
                        Uri baseUri = new Uri(workspace.ServiceEndPoint + workspace.ServiceContainer.Name + ".svc", UriKind.Absolute);
                        DataServiceContext ctx = new DataServiceContext(baseUri);
                        //ctx.EnableAtom = true;
                        //ctx.Format.UseAtom();
                        ctx.Timeout = TestConstants.MaxTestTimeout;
                        Trace.WriteLine("Querying workspace at " + baseUri);
                        ctx.ResolveName = type =>
                        {
                            return type.FullName;
                        };

                        Photo photo = ctx.Execute<Item>("/Items").Where(item => item is Photo).Cast<Photo>().FirstOrDefault();
                        byte[] content = new byte[] { 41, 42, 43, 44, 45, 46, 47 };
                        DataServiceRequestArgs args = new DataServiceRequestArgs();
                        args.ContentType = DataServiceStreamProvider.GetContentType(photo);
                        ctx.SetSaveStream(photo, new MemoryStream(content), true, args);
                        SaveChangesHelper(ctx, SaveChangesOptions.None, async);

                        VerifySinglePhoto(photo, ctx, async);
                        using (var response = GetReadStreamHelper(ctx, photo, new DataServiceRequestArgs(), async))
                        {
                            VerifyStreamContent(new MemoryStream(content), response.Stream);
                        }
                    }
                }
            }

            private DataServiceStreamResponse GetReadStreamHelper(DataServiceContext ctx, object entity, DataServiceRequestArgs args, bool async)
            {
                if (async)
                {
                    DataServiceStreamResponse response = null;
                    Exception ex = null;
                    ManualResetEvent done = new ManualResetEvent(false);
                    ctx.BeginGetReadStream(entity, args, asyncResult =>
                    {
                        try
                        {
                            response = ctx.EndGetReadStream(asyncResult);
                        }
                        catch (Exception e)
                        {
                            ex = e;
                        }
                        finally
                        {
                            done.Set();
                        }
                    }, null);
                    done.WaitOne();
                    if (ex != null)
                    {
                        throw ex;
                    }
                    return response;
                }
                else
                {
                    return ctx.GetReadStream(entity, args);
                }
            }

            private DataServiceResponse SaveChangesHelper(DataServiceContext ctx, SaveChangesOptions options, bool async)
            {
                if (async)
                {
                    DataServiceResponse response = null;
                    Exception ex = null;
                    ManualResetEvent done = new ManualResetEvent(false);
                    ctx.BeginSaveChanges(options, asyncResult =>
                    {
                        try
                        {
                            response = ctx.EndSaveChanges(asyncResult);
                        }
                        catch (Exception e)
                        {
                            ex = e;
                        }
                        finally
                        {
                            done.Set();
                        }
                    }, null);
                    done.WaitOne();
                    if (ex != null)
                    {
                        throw ex;
                    }
                    return response;
                }
                else
                {
                    return ctx.SaveChanges(options);
                }
            }

            private void VerifySinglePhoto(Photo photo, DataServiceContext ctx, bool async)
            {
                Assert.IsNotNull(ctx.GetReadStreamUri(photo), "Photo is MLE so it must have a read stream URI.");
                var entityDescriptor = ctx.GetEntityDescriptor(photo);
                Assert.IsNotNull(entityDescriptor, "The Photo entity must be tracked.");
                Assert.IsNotNull(entityDescriptor.ReadStreamUri, "Photo is MLE so it must have a read stream URI in entity descriptor.");
                Assert.IsNotNull(entityDescriptor.EditStreamUri, "Photo is MLE so it must heav an edit stream URI.");

                Uri readUri = DataServiceStreamProvider.GetReadStreamUri(photo);
                if (readUri != null)
                {
                    Assert.AreEqual(readUri.ToString(), entityDescriptor.ReadStreamUri.ToString(), "The read stream URI doesn't match.");
                }

                using (var response = GetReadStreamHelper(ctx, photo, new DataServiceRequestArgs(), async))
                {
                    Assert.IsNotNull(response);
                    VerifyPhotoContent(photo, response);
                }
            }

            private void VerifyPhotoContent(Photo photo, DataServiceStreamResponse response)
            {
                Assert.AreEqual(DataServiceStreamProvider.GetContentType(photo), response.ContentType);
                Stream responseStream = response.Stream;
                using (Stream fileStream = File.OpenRead(DataServiceStreamProvider.GetStoragePath(photo)))
                {
                    VerifyStreamContent(fileStream, responseStream);
                }
            }

            private void VerifyStreamContent(Stream streamExpected, Stream streamActual)
            {
                int b;
                while ((b = streamExpected.ReadByte()) != -1)
                {
                    int a = streamActual.ReadByte();
                    Assert.AreEqual(a, b, "The content of the MR stream doesn't match.");
                }
                Assert.IsTrue(streamActual.ReadByte() == -1, "The content of the MR stream is longer than expected.");
            }

            #endregion BLOB Support

            #region Named Stream Support

            [NamedStream("Original")]
            [NamedStream("Thumbnail")]
            public class PhotoEx
            {
                public int ID { get; set; }
                public string Name { get; set; }
                public string Description { get; set; }
            }

            [TestMethod]
            public void NamedStreamBasicScenarios()
            {
                using (Utils.RestoreStaticValueOnDispose(typeof(TypedCustomDataContext<PhotoEx>), "PreserveChanges"))
                using (Utils.RestoreStaticValueOnDispose(typeof(SimpleDataServiceHelper), "GetServiceCustomizer"))
                using (Utils.RestoreStaticValueOnDispose(typeof(SimpleDataServiceHelper<TypedCustomDataContext<PhotoEx>>), "CreateDataSourceCustomizer"))
                using (Utils.RestoreStaticValueOnDispose(typeof(SimpleDataService<TypedCustomDataContext<PhotoEx>>), "MaxProtocolVersion"))
                {
                    SimpleDataService<TypedCustomDataContext<PhotoEx>>.MaxProtocolVersion = ODataProtocolVersion.V4;

                    TypedCustomDataContext<PhotoEx>.ClearHandlers();
                    TypedCustomDataContext<PhotoEx>.ClearValues();
                    TypedCustomDataContext<PhotoEx>.CreateChangeScope();

                    DSPMediaResourceStorage storage = new DSPMediaResourceStorage();
                    SimpleDataServiceHelper.GetServiceCustomizer = (t) =>
                    {
                        if (t == typeof(IDataServiceStreamProvider2))
                        {
                            return new TypedCustomStreamProvider2<PhotoEx>(storage);
                        }

                        return null;
                    };

                    SimpleDataServiceHelper<TypedCustomDataContext<PhotoEx>>.CreateDataSourceCustomizer = () =>
                    {
                        return new TypedCustomDataContext<PhotoEx>();
                    };

                    TypedCustomDataContext<PhotoEx>.ValuesRequested += (sender, args) =>
                    {
                        TypedCustomDataContext<PhotoEx> typedContext = (TypedCustomDataContext<PhotoEx>)sender;
                        typedContext.SetValues(
                            new PhotoEx[]
                            {                                
                                new PhotoEx() { ID = 1, Name = "Photo1", Description = "Default Photo 1" },
                                new PhotoEx() { ID = 2, Name = "Photo2", Description = "Default Photo 2" }
                            });

                    };

                    SimpleWorkspace workspace;
                    DataServiceHost host;
                    Utils.CreateWorkspaceForType(typeof(SimpleDataService<TypedCustomDataContext<PhotoEx>>), typeof(TypedCustomDataContext<PhotoEx>), "NamedStreamService", out workspace, out host, false);

                    Uri baseUri = new Uri(workspace.ServiceEndPoint + workspace.ServiceContainer.Name + ".svc", UriKind.Absolute);
                    DataServiceContext ctx = new DataServiceContext(baseUri, ODataProtocolVersion.V4);
                    //ctx.EnableAtom = true;
                    //ctx.Format.UseAtom();
                    ctx.Timeout = TestConstants.MaxTestTimeout;
                    Trace.WriteLine("Querying workspace at " + baseUri);
                    ctx.ResolveName = type =>
                    {
                        return type.FullName;
                    };

                    foreach (PhotoEx p in ctx.Execute<PhotoEx>("Values").ToArray())
                    {
                        // Uninitialized stream.
                        EntityDescriptor descriptor = ctx.GetEntityDescriptor(p);
                        StreamDescriptor stream = descriptor.StreamDescriptors.Single(s => s.StreamLink.Name == "Original");
                        Assert.IsNotNull(stream.StreamLink.EditLink);
                        Assert.AreEqual(stream.StreamLink.EditLink, ctx.GetReadStreamUri(p, "Original"));
                        Assert.IsNull(stream.StreamLink.SelfLink);
                        Assert.IsNull(stream.StreamLink.ContentType);
                        Assert.IsNull(stream.StreamLink.ETag);

                        // GET uninitialized stream.
                        DataServiceStreamResponse response = ctx.GetReadStream(p, "Original", new DataServiceRequestArgs());
                        string responseStreamContent = (new StreamReader(response.Stream)).ReadToEnd();
                        Assert.AreEqual("", responseStreamContent);
                        Assert.IsNull(response.ContentType);
                        Assert.AreEqual("Original", stream.StreamLink.Name);
                        Assert.IsNotNull(stream.StreamLink.EditLink);
                        Assert.AreEqual(stream.StreamLink.EditLink, ctx.GetReadStreamUri(p, "Original"));
                        Assert.IsNull(stream.StreamLink.SelfLink);
                        Assert.IsNull(stream.StreamLink.ContentType);
                        Assert.IsNull(stream.StreamLink.ETag);

                        // PUT named stream.
                        string contentType = "CustomType/CustomSubType";
                        string requestStreamContent = p.Description;
                        ctx.SetSaveStream(p, "Original", new MemoryStream(UTF8Encoding.UTF8.GetBytes(requestStreamContent)), true, contentType);
                        ctx.SaveChanges();
                        Assert.AreEqual("Original", stream.StreamLink.Name);
                        Assert.IsNotNull(stream.StreamLink.EditLink);
                        Assert.AreEqual(stream.StreamLink.EditLink, ctx.GetReadStreamUri(p, "Original"));
                        Assert.IsNull(stream.StreamLink.SelfLink);
                        Assert.IsNull(stream.StreamLink.ContentType);
                        Assert.IsNotNull(stream.StreamLink.ETag);

                        // GET named stream.
                        response = ctx.GetReadStream(p, "Original", new DataServiceRequestArgs());
                        responseStreamContent = (new StreamReader(response.Stream)).ReadToEnd();
                        Assert.AreEqual(requestStreamContent, responseStreamContent);
                        Assert.AreEqual(contentType, response.ContentType);
                        Assert.AreEqual("Original", stream.StreamLink.Name);
                        Assert.IsNotNull(stream.StreamLink.EditLink);
                        Assert.AreEqual(stream.StreamLink.EditLink, ctx.GetReadStreamUri(p, "Original"));
                        Assert.IsNull(stream.StreamLink.SelfLink);
                        Assert.AreEqual(contentType, stream.StreamLink.ContentType);
                        Assert.IsNotNull(stream.StreamLink.ETag);
                    }
                }
            }

            #endregion Named Stream Support

            #region DataBinding

            [TestMethod]
            public void BindingTest()
            {
                using (CachedConnections.SetupConnection(CachedConnections.ConnectionType.Aruba))
                {
                    SimpleWorkspace workspace = this.ArubaWorkspace;
                    Uri baseUri = new Uri(workspace.ServiceEndPoint + workspace.ServiceContainer.Name + ".svc", UriKind.Absolute);
                    DataServiceContext ctx = new ArubaBinding.ArubaContainer(baseUri);
                    //ctx.EnableAtom = true;
                    //ctx.Format.UseAtom();
                    ctx.Timeout = TestConstants.MaxTestTimeout;
                    ctx.ResolveName = ResolveName;
                    ctx.ResolveType = ResolveType;

                    ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;
                    DataServiceCollection<ArubaBinding.Config> configCollection = new DataServiceCollection<ArubaBinding.Config>(ctx.Execute<ArubaBinding.Config>(new Uri("ConfigSet", UriKind.Relative)));

                    ArubaBinding.Config config = new ArubaBinding.Config();
                    config.Id = 9999;

                    ArubaBinding.Failure failure = new ArubaBinding.Failure();
                    failure.Id = 9990;
                    failure.TestId = 1111;
                    failure.Changed = DateTime.Now;

                    configCollection.Add(config);
                    ValidateEntityState(ctx, config, EntityStates.Added);

                    config.Failures.Add(failure);
                    ValidateEntityState(ctx, failure, EntityStates.Added);
                    ValidateLinkState(ctx, config, "Failures", failure, EntityStates.Added);

                    failure.Configs.Add(config);
                    ValidateLinkState(ctx, failure, "Configs", config, EntityStates.Added);

                    ctx.SaveChanges();
                    ValidateEntityState(ctx, failure, EntityStates.Unchanged);
                    ValidateEntityState(ctx, config, EntityStates.Unchanged);
                    ValidateLinkState(ctx, config, "Failures", failure, EntityStates.Unchanged);
                    ValidateLinkState(ctx, failure, "Configs", config, EntityStates.Unchanged);

                    config.Lang = "CSharp";
                    config.OS = "Win7 RC";
                    ValidateEntityState(ctx, config, EntityStates.Modified);

                    ctx.SaveChanges();
                    ValidateEntityState(ctx, config, EntityStates.Unchanged);

                    config.Failures.Remove(failure);
                    ValidateEntityState(ctx, failure, EntityStates.Deleted);

                    configCollection.Remove(config);
                    ValidateEntityState(ctx, config, EntityStates.Deleted);

                    ctx.SaveChanges();
                    ValidateEntityNotPresent(ctx, config);
                    ValidateEntityNotPresent(ctx, failure);
                    ValidateLinkNotPresent(ctx, config, "Failures", failure);
                }
            }

            private static void ValidateEntityState(DataServiceContext context, object entity, EntityStates state)
            {
                Assert.IsTrue(context.Entities.Count(e => e.Entity == entity && e.State == state) == 1, "Couldn't find entity or its not in the right state");
            }

            private static void ValidateLinkState(DataServiceContext context, object source, string propertyName, object target, EntityStates state)
            {
                Assert.IsTrue(context.Links.Count(l => l.Source == source && l.SourceProperty == propertyName && l.Target == target && l.State == state) == 1, "Couldn't find the link or link is not in the right state");
            }

            private static void ValidateEntityNotPresent(DataServiceContext context, object entity)
            {
                Assert.IsTrue(context.Entities.Count(e => e.Entity == entity) == 0, "The entity should be detached");
            }

            private static void ValidateLinkNotPresent(DataServiceContext context, object source, string propertyName, object target)
            {
                Assert.IsTrue(context.Links.Count(l => l.Source == source && l.SourceProperty == propertyName && l.Target == target) == 0, "The link should be detached");
            }

            private static string ResolveName(Type type)
            {
                return type.FullName.Replace("ArubaBinding", "Aruba");
            }

            private static Type ResolveType(string name)
            {
                return typeof(ArubaBinding.Failure).Assembly.GetType(name.Replace("Aruba", "ArubaBinding"));
            }

            #endregion DataBinding

            #region OpenTypes

            public class OpenEntity
            {
                public OpenEntity()
                {
                    this.Properties = new Dictionary<string, object>();
                    this.Numbers = new List<int>();
                    this.Names = new List<string>();
                    this.Addresses = new List<OpenEntity>();
                }

                public int ID { get; set; }
                public string InstanceType { get; set; }
                public IDictionary<string, object> Properties { get; private set; }
                public List<int> Numbers { get; set; }
                public List<string> Names { get; set; }
                public List<OpenEntity> Addresses { get; set; }
            }

            [System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
            public class OpenService : DataService<OpenReflectionDataContext<OpenEntity>>, IServiceProvider
            {
                private OpenReflectionDataContext<OpenEntity> service;
                public static EventHandler<DataServiceProcessingPipelineEventArgs> ProcessingRequest;
                public static EventHandler<DataServiceProcessingPipelineEventArgs> ProcessedRequest;

                public OpenService()
                {
                    this.service = new OpenReflectionDataContext<OpenEntity>();
                    if (ProcessingRequest != null)
                    {
                        this.ProcessingPipeline.ProcessingRequest += ProcessingRequest;
                    }
                    if (ProcessedRequest != null)
                    {
                        this.ProcessingPipeline.ProcessedRequest += ProcessedRequest;
                    }
                }

                // This method is called only once to initialize service-wide policies.
                public static void InitializeService(DataServiceConfiguration config)
                {
                    config.SetEntitySetAccessRule("*", EntitySetRights.All);
                    config.UseVerboseErrors = true;
                    config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                }

                #region IServiceProvider Members

                [ChangeInterceptor("Values")]
                public void OnChangeEntity(OpenEntity entity, UpdateOperations operation)
                {
                    if (operation == UpdateOperations.Add)
                    {
                        entity.Properties["InterceptorInserted"] = true;
                    }
                    else
                        if (operation == UpdateOperations.Change)
                        {
                            entity.Properties["InterceptorUpdated"] = true;
                        }
                }

                public object GetService(Type serviceType)
                {
                    if (serviceType == typeof(IDataServiceQueryProvider) ||
                        serviceType == typeof(IDataServiceMetadataProvider) ||
                        serviceType == typeof(IDataServiceUpdateProvider))
                    {
                        return this.service;
                    }
                    else if (serviceType == typeof(IDataServicePagingProvider))
                    {
                        if (OpenTypeQueryProvider.PagingStrategy != PagingStrategy.None)
                        {
                            return this.service;
                        }
                    }

                    return null;
                }

                #endregion

            }

            [TestMethod]
            public void OpenTypesQuery()
            {
                try
                {
                    OpenReflectionDataContext<OpenEntity>.ResourceTypeNames.Add("Customer");
                    OpenReflectionDataContext<OpenEntity>.ResourceTypeNames.Add("CustomerWithBirthday");
                    OpenReflectionDataContext<OpenEntity>.ComplexTypeName = "Address";

                    OpenReflectionDataContext<OpenEntity>.ValuesRequested += (object sender, EventArgs args) =>
                    {
                        OpenEntity address = new OpenEntity();
                        address.InstanceType = "OpenTypesNamespace.Address";
                        address.Names = new List<string>() { "Var1", "bar" };

                        OpenReflectionDataContext<OpenEntity>.Values = new List<OpenEntity>();
                        OpenEntity oe = new OpenEntity();
                        oe.ID = 1;
                        oe.InstanceType = "OpenTypesNamespace.Customer";
                        oe.Properties.Add("Name", "Waseem");
                        oe.Properties.Add("Age", 31);
                        oe.Properties.Add("Height", 5.6f);
                        oe.Properties.Add("Address", address);
                        oe.Numbers = new List<int>() { 1, 2, 42 };
                        oe.Names = new List<string>() { "First", "Second" };
                        oe.Addresses = new List<OpenEntity>() { address };

                        OpenReflectionDataContext<OpenEntity>.Values.Add(oe);

                        OpenEntity oe2 = new OpenEntity();
                        oe2.ID = 2;
                        oe2.InstanceType = "OpenTypesNamespace.CustomerWithBirthday";
                        oe2.Properties.Add("Name", "Andy");
                        oe2.Properties.Add("Age", 35);
                        oe2.Properties.Add("Birthday", new DateTime(1974, 12, 12));
                        oe2.Properties.Add("Height", 5.7f);

                        OpenReflectionDataContext<OpenEntity>.Values.Add(oe2);
                    };

                    SimpleWorkspace openWorkspace;
                    DataServiceHost openHost;

                    Utils.CreateWorkspaceForType(
                                typeof(OpenService),
                                typeof(OpenReflectionDataContext<OpenEntity>),
                                "OpenTypeService",
                                out openWorkspace,
                                out openHost,
                                false);

                    Uri baseUri = new Uri(openWorkspace.ServiceEndPoint + openWorkspace.ServiceContainer.Name + ".svc", UriKind.Absolute);

                    var testCases = new[] { 
                        new { Query = "/Values?$filter=Age", XPathToCount = "//atom:entry", Count = 2, Accept = Utils.MimeApplicationAtomXml },
                        new { Query = "/Values", XPathToCount = "//atom:entry", Count = 2, Accept = Utils.MimeApplicationAtomXml },
                        new { Query = "/Values(1)", XPathToCount = "//atom:entry", Count = 1, Accept = Utils.MimeApplicationAtomXml },
                        new { Query = "/Values(1)/Name", XPathToCount = "//adsm:value", Count = 1, Accept = Utils.MimeApplicationXml },
                        new { Query = "/Values?$filter=Name eq 'Waseem'", XPathToCount = "//atom:entry", Count = 1, Accept = Utils.MimeApplicationAtomXml },
                        new { Query = "/Values?$filter=Name ne 'Waseem'", XPathToCount = "//atom:entry", Count = 1, Accept = Utils.MimeApplicationAtomXml },
                        new { Query = "/Values?$filter=Name gt 'Amber'", XPathToCount = "//atom:entry", Count = 2, Accept = Utils.MimeApplicationAtomXml },
                        new { Query = "/Values?$filter=Age add 1 eq 32", XPathToCount = "//atom:entry", Count = 1, Accept = Utils.MimeApplicationAtomXml },
                        new { Query = "/Values?$filter=Age div 2 gt 10", XPathToCount = "//atom:entry", Count = 2, Accept = Utils.MimeApplicationAtomXml },
                        new { Query = "/Values?$filter=-Age lt 0", XPathToCount = "//atom:entry", Count = 2, Accept = Utils.MimeApplicationAtomXml },
                        new { Query = "/Values?$filter=Age lt 15 or Age gt 33", XPathToCount = "//atom:entry", Count = 1, Accept = Utils.MimeApplicationAtomXml },
                        new { Query = "/Values?$filter=concat(Name, 'Var1') eq 'AndyVar1'", XPathToCount = "//atom:entry", Count = 1, Accept = Utils.MimeApplicationAtomXml },
                        new { Query = "/Values?$filter=substring(Name, 1) eq 'aseem'", XPathToCount = "//atom:entry", Count = 1, Accept = Utils.MimeApplicationAtomXml },
                        new { Query = "/Values?$filter=length(Name) ge 4", XPathToCount = "//atom:entry", Count = 2, Accept = Utils.MimeApplicationAtomXml },
                        new { Query = "/Values?$filter=round(Height) eq 6f", XPathToCount = "//atom:entry", Count = 2, Accept = Utils.MimeApplicationAtomXml },
                        new { Query = "/Values?$filter=year(cast('OpenTypesNamespace.CustomerWithBirthday')/Birthday) lt 1972", XPathToCount = "//atom:entry", Count = 1, Accept = Utils.MimeApplicationAtomXml },
                        new { Query = "/Values?$filter=not isof('OpenTypesNamespace.CustomerWithBirthday')", XPathToCount = "//atom:entry", Count = 1, Accept = Utils.MimeApplicationAtomXml },
                        new { Query = "/Values?$filter=cast('OpenTypesNamespace.CustomerWithBirthday')/Birthday ne null", XPathToCount = "//atom:entry", Count = 1, Accept = Utils.MimeApplicationAtomXml },
                        new { Query = "/Values?$filter=cast('OpenTypesNamespace.CustomerWithBirthday')/Birthday lt 1975-12-12Z", XPathToCount = "//atom:entry", Count = 1, Accept = Utils.MimeApplicationAtomXml },
                        new { Query = "/Values?$filter=cast('OpenTypesNamespace.CustomerWithBirthday')/Birthday lt 1975-12-12Z&$orderby=Name", XPathToCount = "//atom:entry", Count = 1, Accept = Utils.MimeApplicationAtomXml },
                        new { Query = "/Values?$filter=not isof('OpenTypesNamespace.CustomerWithBirthday')&$top=2", XPathToCount = "//atom:entry", Count = 1, Accept = Utils.MimeApplicationAtomXml },
                        new { Query = "/Values?$filter=not isof('OpenTypesNamespace.CustomerWithBirthday')&$top=1&$skip=1", XPathToCount = "//atom:entry", Count = 0, Accept = Utils.MimeApplicationAtomXml },
                        new { Query = "/Values?$select=Name", XPathToCount = "//ads:Name", Count = 2, Accept = Utils.MimeApplicationAtomXml },
                        new { Query = "/Values?$select=Name", XPathToCount = "//ads:ID", Count = 0, Accept = Utils.MimeApplicationAtomXml },
                        new { Query = "/Values(1)", XPathToCount = "//adsm:element", Count = 10, Accept = Utils.MimeApplicationAtomXml },
                        new { Query = "/Values(1)/Names", XPathToCount = "//adsm:element", Count = 2, Accept = Utils.MimeApplicationXml },
                        new { Query = "/Values(1)/Address", XPathToCount = "//adsm:element", Count = 2, Accept = Utils.MimeApplicationXml },
                    };

                    foreach (var testCase in testCases)
                    {
                        string uri = testCase.Query;
                        Trace.WriteLine("Now processing: " + uri);
                        using (Stream response = Utils.ProcessWebRequest(new Uri(baseUri + uri, UriKind.Absolute), testCase.Accept, "GET"))
                        {
                            XmlDocument document = new XmlDocument(Utils.TestNameTable);
                            document.Load(response);
                            XmlNodeList result = document.SelectNodes(testCase.XPathToCount, Utils.TestNamespaceManager);
                            Assert.AreEqual(testCase.Count, result.Count);
                        }
                    }
                }
                finally
                {
                    OpenReflectionDataContext<OpenEntity>.Clear();
                }
            }

            public class OpenAddress
            {
                public OpenAddress() { this.Names = new List<string>(); }
                public List<string> Names { get; set; }
            }

            public class OpenCustomer
            {
                public OpenCustomer()
                {
                    this.Numbers = new List<int>();
                    this.Names = new List<string>();
                    this.Addresses = new List<OpenAddress>();
                }

                public int ID { get; set; }
                public string Name { get; set; }
                public int Age { get; set; }
                public Single Height { get; set; }
                public bool? InterceptorInserted { get; set; }
                public bool? InterceptorUpdated { get; set; }
                public List<int> Numbers { get; set; }
                public List<string> Names { get; set; }
                public List<OpenAddress> Addresses { get; set; }
            }

            public class OpenCustomerWithBirthday : OpenCustomer
            {
                public DateTimeOffset Birthday { get; set; }
            }

            [TestMethod]
            public void OpenTypesCreateUpdateDelete()
            {
                DataServiceResponsePreference[] responsePreferences = new DataServiceResponsePreference[] {
                    DataServiceResponsePreference.None,
                    DataServiceResponsePreference.IncludeContent,
                    DataServiceResponsePreference.NoContent
                };

                foreach (DataServiceResponsePreference addAndUpdateResponsePreference in responsePreferences)
                {
                    try
                    {
                        OpenReflectionDataContext<OpenEntity>.ResourceTypeNames.Add("Customer");
                        OpenReflectionDataContext<OpenEntity>.ResourceTypeNames.Add("CustomerWithBirthday");
                        OpenReflectionDataContext<OpenEntity>.ComplexTypeName = "Address";

                        OpenReflectionDataContext<OpenEntity>.ValuesRequested += (object sender, EventArgs args) =>
                        {
                            OpenReflectionDataContext<OpenEntity>.Values = new List<OpenEntity>();

                            OpenEntity a1 = new OpenEntity();
                            a1.InstanceType = "OpenTypesNamespace.Address";
                            a1.Properties.Add("Name", "Main St.");

                            OpenEntity a2 = new OpenEntity();
                            a2.InstanceType = "OpenTypesNamespace.Address";
                            a2.Properties.Add("Name", null);

                            OpenEntity oe = new OpenEntity();
                            oe.ID = 1;
                            oe.InstanceType = "OpenTypesNamespace.Customer";
                            oe.Properties.Add("Name", "Waseem");
                            oe.Properties.Add("Age", 31);
                            oe.Properties.Add("Height", 5.6f);
                            oe.Numbers = new List<int>() { 1, 2, 42 };
                            oe.Names = new List<string>() { "", "first", "Second" };
                            oe.Addresses = new List<OpenEntity>() { a1, a2 };
                            OpenReflectionDataContext<OpenEntity>.Values.Add(oe);
                        };

                        Type clientCustomerType = typeof(OpenCustomer);

                        OpenService.ProcessedRequest = (sender, args) =>
                        {
                            if (args.OperationContext.RequestMethod == "POST" &&
                                args.OperationContext.ResponseHeaders["Preference-Applied"] == "return=minimal")
                            {
                                Assert.IsNotNull(args.OperationContext.ResponseHeaders["OData-EntityId"],
                                                 "Response to POST with no payload must include OData-EntityId header.");
                            }
                            else
                            {
                                Assert.IsNull(args.OperationContext.ResponseHeaders["OData-EntityId"],
                                              "Response to anything other than POST without payload must NOT include OData-EntityId header.");
                            }
                        };

                        SimpleWorkspace openWorkspace;
                        DataServiceHost openHost;

                        Utils.CreateWorkspaceForType(
                            typeof(OpenService),
                            typeof(OpenReflectionDataContext<OpenEntity>),
                            "OpenTypeService",
                            out openWorkspace,
                            out openHost,
                            false);

                        Uri baseUri = new Uri(openWorkspace.ServiceEndPoint + openWorkspace.ServiceContainer.Name + ".svc", UriKind.Absolute);

                        DataServiceContext ctx = new DataServiceContext(baseUri, ODataProtocolVersion.V4);
                        //ctx.EnableAtom = true;
                        //ctx.Format.UseAtom();
                        ctx.AddAndUpdateResponsePreference = addAndUpdateResponsePreference;
                        ctx.MergeOption = MergeOption.OverwriteChanges;
                        ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;

                        ctx.ResolveName += (t) => t == clientCustomerType ? "OpenTypesNamespace.Customer" :
                           (t == typeof(OpenCustomerWithBirthday) ? "OpenTypesNamespace.CustomerWithBirthday" : "OpenTypesNamespace.Address");
                        ctx.ResolveType += (s) => s == "OpenTypesNamespace.Customer" ? clientCustomerType :
                            (s == "OpenTypesNamespace.CustomerWithBirthday" ? typeof(OpenCustomerWithBirthday) : typeof(OpenAddress));

                        OpenCustomer newCustomer = new OpenCustomerWithBirthday()
                        {
                            Birthday = new DateTime(1974, 12, 12)
                        };

                        newCustomer.ID = 2;
                        newCustomer.Name = "Andy";
                        newCustomer.Age = 35;
                        newCustomer.Height = 5.7f;
                        newCustomer.Names = new List<string>() { "Var1", "Bar" };
                        newCustomer.Numbers = new List<int>() { -1, -2 };
                        newCustomer.Addresses = new List<OpenAddress>() { };
                        ctx.AddObject("Values", newCustomer);
                        ctx.SaveChanges();

                        VerifyInsertResponse(addAndUpdateResponsePreference, ctx.Entities.Single(e => (e.Entity as OpenCustomer).ID == 2).Entity as OpenCustomer);

                        DataServiceQuery<OpenCustomer> customers = ctx.CreateQuery<OpenCustomer>("Values");

                        // Verify insert
                        int count = 0;
                        foreach (OpenCustomer oc in customers)
                        {
                            Assert.IsTrue(oc.ID == 1 && oc.Name == "Waseem" ||
                                          oc.ID == 2 && oc.Name == "Andy");

                            if (oc.ID == 2)
                            {
                                Assert.AreEqual(2, oc.Names.Count);
                                Assert.AreEqual("Var1", oc.Names[0]);
                                Assert.AreEqual("Bar", oc.Names[1]);
                                Assert.AreEqual(2, oc.Numbers.Count);
                                Assert.AreEqual(-1, oc.Numbers[0]);
                                Assert.AreEqual(-2, oc.Numbers[1]);
                                Assert.AreEqual(0, oc.Addresses.Count);
                            }

                            oc.Age = 50;
                            oc.Numbers.Add(oc.ID + 1000);
                            oc.Names.Clear();
                            oc.Names.Add(oc.ID.ToString());
                            oc.Addresses.Clear();
                            oc.Addresses.Add(new OpenAddress() { Names = new List<string>() { "Address" + oc.ID.ToString() } });

                            ctx.UpdateObject(oc);

                            count++;
                        }
                        Assert.AreEqual(2, count);

                        ctx.SaveChanges();

                        VerifyUpdateResponse(addAndUpdateResponsePreference, ctx.Entities.Single(e => (e.Entity as OpenCustomer).ID == 1).Entity as OpenCustomer);
                        VerifyUpdateResponse(addAndUpdateResponsePreference, ctx.Entities.Single(e => (e.Entity as OpenCustomer).ID == 2).Entity as OpenCustomer);

                        // Verify update
                        count = 0;
                        foreach (OpenCustomer oc in customers)
                        {
                            Assert.AreEqual(50, oc.Age);
                            Assert.IsTrue(oc.Numbers.Contains(oc.ID + 1000));
                            Assert.AreEqual(1, oc.Names.Count);
                            Assert.AreEqual(oc.ID.ToString(), oc.Names[0]);
                            Assert.AreEqual(1, oc.Addresses.Count);
                            Assert.AreEqual(1, oc.Addresses[0].Names.Count);
                            Assert.AreEqual("Address" + oc.ID.ToString(), oc.Addresses[0].Names[0]);
                            count++;
                        }

                        Assert.AreEqual(2, count);

                        // Verify delete
                        ctx.DeleteObject(customers.Where(c => c.Name == "Waseem").First());
                        ctx.SaveChanges();

                        count = 0;
                        foreach (OpenCustomer oc in customers)
                        {
                            Assert.IsTrue(oc.ID == 2 && oc.Name == "Andy");
                            count++;
                        }

                        Assert.AreEqual(1, count);
                    }
                    finally
                    {
                        OpenReflectionDataContext<OpenEntity>.Clear();
                        OpenService.ProcessedRequest = null;
                    }
                }
            }

            private static void VerifyInsertResponse(DataServiceResponsePreference addAndUpdateResponsePreference, OpenCustomer entity)
            {
                switch (addAndUpdateResponsePreference)
                {
                    case DataServiceResponsePreference.None:
                    case DataServiceResponsePreference.IncludeContent:
                        Assert.IsTrue(entity.InterceptorInserted.Value);
                        break;
                    case DataServiceResponsePreference.NoContent:
                        Assert.IsNull(entity.InterceptorInserted);
                        break;
                }
            }

            private static void VerifyUpdateResponse(DataServiceResponsePreference addAndUpdateResponsePreference, OpenCustomer entity)
            {
                switch (addAndUpdateResponsePreference)
                {
                    case DataServiceResponsePreference.None:
                    case DataServiceResponsePreference.NoContent:
                        Assert.IsNull(entity.InterceptorUpdated);
                        break;
                    case DataServiceResponsePreference.IncludeContent:
                        Assert.IsTrue(entity.InterceptorUpdated.Value);
                        break;
                }
            }

            [TestMethod]
            public void ServerDrivenPagingCustomBasic()
            {
                try
                {
                    using (Utils.RestoreStaticValueOnDispose(typeof(OpenTypeQueryProvider), "PagingStrategy"))
                    using (Utils.RestoreStaticValueOnDispose(typeof(OpenTypeQueryProvider), "PageSize"))
                    {
                        OpenReflectionDataContext<OpenEntity>.ResourceTypeNames.Add("Customer");
                        OpenTypeQueryProvider.PagingStrategy = PagingStrategy.FixedPageSize;
                        OpenTypeQueryProvider.PageSize = 2;

                        OpenReflectionDataContext<OpenEntity>.ValuesRequested += (object sender, EventArgs args) =>
                        {
                            OpenReflectionDataContext<OpenEntity>.Values = new List<OpenEntity>();
                            OpenEntity oe3 = new OpenEntity();
                            oe3.ID = 3;
                            oe3.InstanceType = "OpenTypesNamespace.Customer";
                            oe3.Properties.Add("Name", "Pratik");
                            oe3.Properties.Add("Age", 7);
                            oe3.Properties.Add("Height", 5.9f);

                            OpenReflectionDataContext<OpenEntity>.Values.Add(oe3);

                            OpenEntity oe4 = new OpenEntity();
                            oe4.ID = 4;
                            oe4.InstanceType = "OpenTypesNamespace.Customer";
                            oe4.Properties.Add("Name", "Phani");
                            oe4.Properties.Add("Age", 20);
                            oe4.Properties.Add("Height", 5.0f);

                            OpenReflectionDataContext<OpenEntity>.Values.Add(oe4);

                            OpenEntity oe2 = new OpenEntity();
                            oe2.ID = 2;
                            oe2.InstanceType = "OpenTypesNamespace.Customer";
                            oe2.Properties.Add("Name", "Andy");
                            oe2.Properties.Add("Age", 35);
                            oe2.Properties.Add("Height", 5.7f);

                            OpenReflectionDataContext<OpenEntity>.Values.Add(oe2);

                            OpenEntity oe = new OpenEntity();
                            oe.ID = 1;
                            oe.InstanceType = "OpenTypesNamespace.Customer";
                            oe.Properties.Add("Name", "Waseem");
                            oe.Properties.Add("Age", 31);
                            oe.Properties.Add("Height", 5.6f);

                            OpenReflectionDataContext<OpenEntity>.Values.Add(oe);
                        };

                        SimpleWorkspace openWorkspace;
                        DataServiceHost openHost;

                        Utils.CreateWorkspaceForType(
                                    typeof(OpenService),
                                    typeof(OpenReflectionDataContext<OpenEntity>),
                                    "OpenTypeService",
                                    out openWorkspace,
                                    out openHost,
                                    false);

                        Uri baseUri = new Uri(openWorkspace.ServiceEndPoint + openWorkspace.ServiceContainer.Name + ".svc", UriKind.Absolute);

                        string uri = "/Values";
                        int totalCount = 0;

                        do
                        {
                            Trace.WriteLine("Now processing: " + uri);
                            using (Stream response = Utils.ProcessWebRequest(new Uri(baseUri + uri, UriKind.Absolute)))
                            {
                                XmlDocument document = new XmlDocument(Utils.TestNameTable);
                                document.Load(response);
                                XmlNodeList result = document.SelectNodes("//atom:entry", Utils.TestNamespaceManager);
                                int numEntries = result.Count;
                                totalCount += numEntries;
                                Assert.IsTrue(numEntries <= OpenTypeQueryProvider.PageSize);
                                result = document.SelectNodes("/atom:feed/atom:link[@rel='next']", Utils.TestNamespaceManager);
                                Assert.IsTrue(1 == result.Count || numEntries < OpenTypeQueryProvider.PageSize,
                                        "Expected number of next elements did not match the actual number of next elements");
                                if (result.Count == 0)
                                {
                                    break;
                                }
                                // Verify that the next link is correct and set that as the next uri to request.
                                String nextLinkUri = result[0].Attributes["href"].Value;
                                uri = nextLinkUri.Substring(nextLinkUri.LastIndexOf('/'));
                            }
                        }
                        while (true);

                        Assert.AreEqual(4, totalCount);
                    }
                }
                finally
                {
                    OpenReflectionDataContext<OpenEntity>.Clear();
                }
            }

            #endregion

            #region Projection + Concurrency + Interceptors

            [TestMethod]
            public void ServerProjectionConcurrencyInterceptorTests()
            {
                // Customers with odd IDs are MLEs
                // Customers with even IDs are non-MLEs
                var testCases = new[] {
                    // Make sure when there's no projected properties, we omit the <m:properties /> node
                    // This should hold true for both MLE and non-MLE entities.
                    new {
                        QueryStrings = new string[] {
                            "/Customers(0)?$select=Orders",
                            "/Customers(1)?$select=Orders",
                        },
                        XPathExprs = new string[] {
                            "not(/atom:entry[contains(atom:id, 'Customers(0)')]/atom:content/@src) or boolean(/atom:entry[contains(atom:id, 'Customers(1)')]/atom:content/@src)",
                            "not(/atom:entry/adsm:properties)",
                            "not(/atom:entry/atom:content/adsm:properties)",
                            "not(//atom:entry/atom:link[@rel='edit-media' and @adsm:etag != '\"MediaResourceETag1\"'])",
                            "boolean(//atom:entry[contains(@adsm:etag, 'W/\"')])",
                        },
                        ExpectedCustomerQueryInterceptorCalls = 1,
                        ExpectedOrderQueryInterceptorCalls = 0,
                        ExpectETag = true,
                    },
                    // Project a subset of properties
                    new {
                        QueryStrings = new string[] {
                            "/Customers(0)?$select=Concurrency,EditTimeStamp",
                            "/Customers(1)?$select=Concurrency,EditTimeStamp"
                        },
                        XPathExprs = new string[] {
                            "not(/atom:entry[contains(atom:id, 'Customers(0)')]/atom:content/@src) or boolean(/atom:entry[contains(atom:id, 'Customers(1)')]/atom:content/@src)",
                            "boolean(/atom:entry[contains(atom:id, 'Customers(0)')]/atom:content/adsm:properties/ads:Concurrency) or boolean(/atom:entry[contains(atom:id, 'Customers(1)')]/adsm:properties/ads:Concurrency)",
                            "boolean(/atom:entry[contains(atom:id, 'Customers(0)')]/atom:content/adsm:properties/ads:EditTimeStamp) or boolean(/atom:entry[contains(atom:id, 'Customers(1)')]/adsm:properties/ads:EditTimeStamp)",
                            "not(//atom:entry/atom:link[@rel='edit-media' and @adsm:etag != '\"MediaResourceETag1\"'])",
                            "boolean(//atom:entry[contains(@adsm:etag, 'W/\"')])",
                        },
                        ExpectedCustomerQueryInterceptorCalls = 1,
                        ExpectedOrderQueryInterceptorCalls = 0,
                        ExpectETag = true,
                    },
                    // Explicitly project all properties
                    new {
                        QueryStrings = new string[] {
                            "/Customers(0)?$select=*,Orders", 
                            "/Customers(0)?$select=Address,*", 
                            "/Customers(0)?$select=*,*",
                            "/Customers(1)?$select=*,Orders", 
                            "/Customers(1)?$select=Address,*", 
                            "/Customers(1)?$select=*,*",
                        },
                        XPathExprs = new string[] {
                            "not((/atom:entry | /atom:feed/atom:entry)[contains(atom:id, 'Customers(0)')]/atom:content/@src) or boolean((/atom:entry | /atom:feed/atom:entry)[contains(atom:id, 'Customers(1)')]/atom:content/@src)",
                            "count((/atom:entry | /atom:feed/atom:entry)/adsm:properties/*) = 7 or count((/atom:entry | /atom:feed/atom:entry)/atom:content/adsm:properties/*) = 7",

                            "not(//atom:entry/atom:link[@rel='edit-media' and @adsm:etag != '\"MediaResourceETag1\"'])",
                            "boolean(//atom:entry[contains(@adsm:etag, 'W/\"')])",
                        },
                        ExpectedCustomerQueryInterceptorCalls = 1,
                        ExpectedOrderQueryInterceptorCalls = 0,
                        ExpectETag = true,
                    },
                    new {
                        QueryStrings = new string[] {
                            "/Customers?$filter=ID eq 0&$select=*",
                            "/Customers?$filter=ID eq 1&$select=*",
                        },
                        XPathExprs = new string[] {
                            "not((/atom:entry | /atom:feed/atom:entry)[contains(atom:id, 'Customers(0)')]/atom:content/@src) or boolean((/atom:entry | /atom:feed/atom:entry)[contains(atom:id, 'Customers(1)')]/atom:content/@src)",
                            "count((/atom:entry | /atom:feed/atom:entry)/adsm:properties/*) = 7 or count((/atom:entry | /atom:feed/atom:entry)/atom:content/adsm:properties/*) = 7",

                            "not(//atom:entry/atom:link[@rel='edit-media' and @adsm:etag != '\"MediaResourceETag1\"'])",
                            "boolean(//atom:entry[contains(@adsm:etag, 'W/\"')])",
                        },
                        ExpectedCustomerQueryInterceptorCalls = 1,
                        ExpectedOrderQueryInterceptorCalls = 0,
                        ExpectETag = false,
                    },
                    // Implicitly/Explicitly project all properties of the expanded entity
                    new {
                        QueryStrings = new string[] {
                            "/Customers(0)?$select=Orders&$expand=Orders",        /*Implicit projection */
                            "/Customers(0)?$select=Orders&$expand=Orders($select=*)",      /*Explicit projection */
                            "/Customers(1)?$select=Orders&$expand=Orders",        /*Implicit projection */
                            "/Customers(1)?$select=Orders&$expand=Orders($select=*)",      /*Explicit projection */
                        },
                        XPathExprs = new string[] {
                            "not(/atom:entry/adsm:properties)",
                            "not(/atom:entry/atom:content/adsm:properties)",
                            "not(/atom:entry[contains(atom:id, 'Customers(0)')]/atom:content/@src) or boolean(/atom:entry[contains(atom:id, 'Customers(1)')]/atom:content/@src)",
                            "boolean(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content)",
                            "not(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content/@src)",
                            "not(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content[not(count(adsm:properties/*)=3)])",

                            "not(//atom:entry/atom:link[@rel='edit-media' and @adsm:etag!='\"MediaResourceETag1\"'])",
                            "boolean(//atom:entry[contains(atom:id, 'Customers') and contains(@adsm:etag, 'W/\"')])",
                            "not(//atom:entry[contains(atom:id, 'Orders') and contains(@adsm:etag, 'W/\"')])",
                        },
                        ExpectedCustomerQueryInterceptorCalls = 1,
                        ExpectedOrderQueryInterceptorCalls = 1,
                        ExpectETag = false,
                    },
                    // V1 expand
                    new {
                        QueryStrings = new string[] {
                            "/Customers(0)?$expand=Orders",
                            "/Customers(1)?$expand=Orders"
                        },
                        XPathExprs = new string[] {
                            "not(/atom:entry[contains(atom:id, 'Customers(0)')]/atom:content/@src) or boolean(/atom:entry[contains(atom:id, 'Customers(1)')]/atom:content/@src)",
                            "count(/atom:entry[contains(atom:id, 'Customers(1)')]/adsm:properties/*)=7 or count(/atom:entry[contains(atom:id, 'Customers(0)')]/atom:content/adsm:properties/*)=7",
                            
                            "boolean(/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content)",
                            "not    (/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content/@src)",
                            "not    (/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content[not(count(adsm:properties/*)=3)])",

                            "not(//atom:entry/atom:link[@rel='edit-media' and @adsm:etag != '\"MediaResourceETag1\"'])",
                            "boolean(//atom:entry[contains(atom:id, 'Customers') and contains(@adsm:etag, 'W/\"')])",
                            "not(//atom:entry[contains(atom:id, 'Orders') and @adsm:etag])",
                        },
                        ExpectedCustomerQueryInterceptorCalls = 1,
                        ExpectedOrderQueryInterceptorCalls = 1,
                        ExpectETag = false,
                    },
                    // multilevel expand and project, only project 1 property per level
                    new {
                        QueryStrings = new string[] {
                            "/Orders(1)?$select=ID&$expand=Customer($select=Name;$expand=BestFriend($select=Name;$expand=Orders($select=ID)))",
                        },
                        XPathExprs = new string[] {
                            "not(/atom:entry/adsm:properties)",
                            "not(/atom:entry/adsm:content/@src)",
                            "boolean(/atom:entry/atom:content/adsm:properties/ads:ID)",
                            "count(/atom:entry/atom:content/adsm:properties/*) = 1",

                            "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content/@src)",
                            "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties)",
                            "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content/*)",
                            "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:content[not(@src)])",
                            "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties[not(ads:Name)])",
                            "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/adsm:properties[count(*) != 1])",

                            "boolean(/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='BestFriend']/adsm:inline/atom:entry/atom:content)",
                            "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='BestFriend']/adsm:inline/atom:entry/adsm:properties)",
                            "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='BestFriend']/adsm:inline/atom:entry/atom:content/@src)",
                            "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='BestFriend']/adsm:inline/atom:entry/atom:content/adsm:properties[not(ads:Name)])",
                            "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='BestFriend']/adsm:inline/atom:entry/atom:content/adsm:properties[count(*) != 1])",

                            "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='BestFriend']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content/@src)",
                            "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='BestFriend']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/adsm:properties)",
                            "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='BestFriend']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content/adsm:properties[not(ads:ID)])",
                            "not    (/atom:entry/atom:link[@title='Customer']/adsm:inline/atom:entry/atom:link[@title='BestFriend']/adsm:inline/atom:entry/atom:link[@title='Orders']/adsm:inline/atom:feed/atom:entry/atom:content/adsm:properties[count(*) != 1])",

                            "not(//atom:entry/atom:link[@rel='edit-media' and not(@adsm:etag = '\"MediaResourceETag1\"')])",
                            "count(//atom:entry[atom:link[@rel='edit' and @title='Orders'] and contains(@adsm:etag, 'W/\"')]) = 0",
                            "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Customers')] and contains(@adsm:etag, 'W/\"')]) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Customers')]])",
                            "count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders')] and not(@adsm:etag)]) = count(//atom:entry[atom:link[@rel='edit' and contains(@href, 'Orders')]])"
                        },
                        ExpectedCustomerQueryInterceptorCalls = 2,
                        ExpectedOrderQueryInterceptorCalls = 2,
                        ExpectETag = false,
                    },
                };

                using (Utils.RestoreStaticValueOnDispose(typeof(CustomObjectService), "CustomerChangeInterceptorOverride"))
                using (Utils.RestoreStaticValueOnDispose(typeof(CustomObjectService), "CustomerQueryInterceptorOverride"))
                using (Utils.RestoreStaticValueOnDispose(typeof(CustomObjectService), "OrderChangeInterceptorOverride"))
                using (Utils.RestoreStaticValueOnDispose(typeof(CustomObjectService), "OrderQueryInterceptorOverride"))
                using (CachedConnections.SetupConnection(CachedConnections.ConnectionType.CustomObjectContext))
                {
                    SimpleWorkspace workspace = this.CustomObjectWorkspace;
                    Uri baseUri = new Uri(workspace.ServiceEndPoint + workspace.ServiceContainer.Name + ".svc", UriKind.Absolute);

                    foreach (var testCase in testCases)
                    {
                        foreach (string queryString in testCase.QueryStrings)
                        {
                            int customerChangeInterceptorInvokeCount = 0;
                            int customerQueryInterceptorInvokeCount = 0;
                            int orderChangeInterceptorInvokeCount = 0;
                            int orderQueryInterceptorInvokeCount = 0;

                            CustomObjectService.CustomerChangeInterceptorOverride = (c, option) => { customerChangeInterceptorInvokeCount++; };
                            CustomObjectService.CustomerQueryInterceptorOverride = () => { customerQueryInterceptorInvokeCount++; return c => true; };
                            CustomObjectService.OrderChangeInterceptorOverride = (o, option) => { orderChangeInterceptorInvokeCount++; };
                            CustomObjectService.OrderQueryInterceptorOverride = () => { orderQueryInterceptorInvokeCount++; return o => true; };

                            // Validate payload, including etags
                            WebHeaderCollection responseHeaders;
                            XmlDocument atomResponse = Utils.ProcessWebRequestAsAtom(new Uri(baseUri.OriginalString + queryString, UriKind.Absolute), out responseHeaders);
                            Utils.VerifyXPathExpressionResults(atomResponse, true, testCase.XPathExprs);

                            // Validate etag header value
                            string etag = responseHeaders["ETag"];
                            if (testCase.ExpectETag)
                            {
                                string payloadEtag = atomResponse.ChildNodes.Item(1).Attributes.GetNamedItem("m:etag").Value;
                                Assert.AreEqual(payloadEtag, etag);
                            }
                            else
                            {
                                Assert.IsTrue(string.IsNullOrEmpty(etag));
                            }

                            // Validate Query Interceptors
                            Assert.AreEqual(0, customerChangeInterceptorInvokeCount);
                            Assert.AreEqual(0, orderChangeInterceptorInvokeCount);
                            Assert.AreEqual(testCase.ExpectedCustomerQueryInterceptorCalls, customerQueryInterceptorInvokeCount);
                            Assert.AreEqual(testCase.ExpectedOrderQueryInterceptorCalls, orderQueryInterceptorInvokeCount);
                        }
                    }
                }
            }

            [HasStream]
            private class NarrowCustomerWithStream : NarrowCustomer
            {
            }

            private class NarrowCustomer
            {
                public int ID { get; set; }
                public string Name { get; set; }
                public string Concurrency { get; set; }
                public Byte[] EditTimeStamp { get; set; }
                public List<NarrowOrder> Orders { get; set; }
                public NarrowCustomer BestFriend { get; set; }

                public static int UnInitID = -999;

                public NarrowCustomer()
                {
                    this.ID = NarrowCustomer.UnInitID;
                }
            }
            private class NarrowOrder
            {
                public int ID { get; set; }
                public Double DollarAmount { get; set; }
                public NarrowCustomer Customer { get; set; }
            }

            private class TestCaseInfo
            {
                public Func<DataServiceContext, IQueryable> Query;
                public string QueryString;
                public Action<object> Validate;
                public int ExpectedCustomerQueryInterceptorCalls;
                public int ExpectedOrderQueryInterceptorCalls;
            }

            [TestMethod]
            public void ClientProjectionConcurrencyInterceptorTests()
            {
                // Customers with odd IDs are MLEs
                // Customers with even IDs are non-MLEs
                var testCases = new TestCaseInfo[] {

                    // Make sure the client works when there's no projected properties, i.e. we omit the <m:properties /> node
                    // This should hold true for both MLE and non-MLE entities.
                    new TestCaseInfo() {
                        Query = ctx => ctx.CreateQuery<NarrowCustomerWithStream>("Customers").Where(c => c.ID == 0).Select(c => new NarrowCustomerWithStream() {Name = c.Name, Orders = c.Orders.Select(o => new NarrowOrder() { DollarAmount = o.DollarAmount }).ToList()}),
                        QueryString = "Customers(0)?$expand=Orders($select=DollarAmount)&$select=Name",
                        Validate = c => {
                            Assert.AreEqual(NarrowCustomerWithStream.UnInitID, ((NarrowCustomerWithStream)c).ID);
                            Assert.IsNull(((NarrowCustomerWithStream)c).Concurrency);
                            Assert.IsNull(((NarrowCustomerWithStream)c).EditTimeStamp);
                            Assert.AreEqual(2, ((NarrowCustomerWithStream)c).Orders.Count());
                        },
                        ExpectedCustomerQueryInterceptorCalls = 1,
                        ExpectedOrderQueryInterceptorCalls = 1,
                    },
                    new TestCaseInfo() {
                        Query = ctx => ctx.CreateQuery<NarrowCustomerWithStream>("Customers").Where(c => c.ID == 1).Select(c => new NarrowCustomerWithStream() {Name = c.Name, Orders = c.Orders.Select(o => new NarrowOrder() { DollarAmount = o.DollarAmount }).ToList()}),
                        QueryString = "Customers(1)?$expand=Orders($select=DollarAmount)&$select=Name",
                        Validate = c => {
                            Assert.AreEqual(NarrowCustomerWithStream.UnInitID, ((NarrowCustomerWithStream)c).ID);
                            Assert.IsNull(((NarrowCustomerWithStream)c).Concurrency);
                            Assert.IsNull(((NarrowCustomerWithStream)c).EditTimeStamp);
                            Assert.AreEqual(2, ((NarrowCustomerWithStream)c).Orders.Count());
                        },
                        ExpectedCustomerQueryInterceptorCalls = 1,
                        ExpectedOrderQueryInterceptorCalls = 1,
                    },

                    //// Project a subset of properties
                    new TestCaseInfo() {
                        Query = ctx => ctx.CreateQuery<NarrowCustomerWithStream>("Customers").Where(c => c.ID == 0).Select(c => new NarrowCustomerWithStream() { Concurrency = c.Concurrency, EditTimeStamp = c.EditTimeStamp}),
                        QueryString = "Customers(0)?$select=Concurrency,EditTimeStamp",
                        Validate = c => {
                            Assert.AreEqual(NarrowCustomerWithStream.UnInitID, ((NarrowCustomerWithStream)c).ID);
                            Assert.AreEqual("0", ((NarrowCustomerWithStream)c).Concurrency);
                            Assert.IsNull(((NarrowCustomerWithStream)c).Name);
                            Assert.IsNotNull(((NarrowCustomerWithStream)c).EditTimeStamp);
                            Assert.IsNull(((NarrowCustomerWithStream)c).Orders);
                        },
                        ExpectedCustomerQueryInterceptorCalls = 1,
                        ExpectedOrderQueryInterceptorCalls = 0,
                    },
                    new TestCaseInfo() {
                        Query = ctx => ctx.CreateQuery<NarrowCustomerWithStream>("Customers").Where(c => c.ID == 1).Select(c => new NarrowCustomerWithStream() { Concurrency = c.Concurrency, EditTimeStamp = c.EditTimeStamp}),
                        QueryString = "Customers(1)?$select=Concurrency,EditTimeStamp",
                        Validate = c => {
                            Assert.AreEqual(NarrowCustomerWithStream.UnInitID, ((NarrowCustomerWithStream)c).ID);
                            Assert.AreEqual("1", ((NarrowCustomerWithStream)c).Concurrency);
                            Assert.IsNull(((NarrowCustomerWithStream)c).Name);
                            Assert.IsNotNull(((NarrowCustomerWithStream)c).EditTimeStamp);
                            Assert.IsNull(((NarrowCustomerWithStream)c).Orders);
                        },
                        ExpectedCustomerQueryInterceptorCalls = 1,
                        ExpectedOrderQueryInterceptorCalls = 0,
                    },

                    //// Explicitly project all properties
                    new TestCaseInfo() {
                        Query = ctx => { ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support; return from c in ctx.CreateQuery<NarrowCustomerWithStream>("Customers") where c.ID == 1 select new { c, c.BestFriend }; },
                        QueryString = "Customers(1)?$expand=BestFriend",
                        Validate = c => { },
                        ExpectedCustomerQueryInterceptorCalls = 2,
                        ExpectedOrderQueryInterceptorCalls = 0,
                    },

                    //// V1 expand
                    new TestCaseInfo() {
                        Query = ctx => { ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support; return ctx.CreateQuery<NarrowCustomerWithStream>("Customers").Expand("BestFriend").Where(c => c.ID == 0); },
                        QueryString = "Customers(0)?$expand=BestFriend",
                        Validate = c => {
                            Assert.AreEqual(0, ((NarrowCustomerWithStream)c).ID);
                            Assert.AreEqual("0", ((NarrowCustomerWithStream)c).Concurrency);
                            Assert.AreEqual("Customer 0", ((NarrowCustomerWithStream)c).Name);
                            Assert.IsNotNull(((NarrowCustomerWithStream)c).EditTimeStamp);
                            Assert.IsNull(((NarrowCustomerWithStream)c).BestFriend);
                        },
                        ExpectedCustomerQueryInterceptorCalls = 2,
                        ExpectedOrderQueryInterceptorCalls = 0,
                    },

                    //// multilevel expand and project, only project 1 property per level
                    new TestCaseInfo() {
                        Query = ctx => ctx.CreateQuery<NarrowOrder>("Orders").Where(o => o.ID == 1).Select(o => new NarrowOrder()
                        {
                            ID = o.ID,
                            Customer = new NarrowCustomerWithStream()
                            {
                                Name = o.Customer.Name, 
                                BestFriend = new NarrowCustomerWithStream()
                                {
                                    Name = o.Customer.BestFriend.Name, 
                                    Orders = o.Customer.BestFriend.Orders.Select(o2 => new NarrowOrder() { ID = o2.ID }).ToList()
                                }
                            }
                        }),
                        QueryString = "Orders(1)?$expand=Customer($select=Name),Customer($expand=BestFriend($select=Name)),Customer($expand=BestFriend($expand=Orders($select=ID)))&$select=ID",
                        Validate = o => {
                            // Order
                            Assert.AreEqual(1, ((NarrowOrder)o).ID);
                            Assert.AreEqual(0, ((NarrowOrder)o).DollarAmount);

                            // Order/Customer
                            Assert.AreEqual(NarrowCustomerWithStream.UnInitID, ((NarrowOrder)o).Customer.ID);
                            Assert.IsNull(((NarrowOrder)o).Customer.Concurrency);
                            Assert.IsNull(((NarrowOrder)o).Customer.EditTimeStamp);
                            Assert.AreEqual("Customer 1", ((NarrowOrder)o).Customer.Name);
                            Assert.IsNull(((NarrowOrder)o).Customer.Orders);

                            // Order/Customer/BestFriend
                            Assert.AreEqual(NarrowCustomerWithStream.UnInitID, ((NarrowOrder)o).Customer.BestFriend.ID);
                            Assert.IsNull(((NarrowOrder)o).Customer.BestFriend.Concurrency);
                            Assert.IsNull(((NarrowOrder)o).Customer.BestFriend.EditTimeStamp);
                            Assert.AreEqual("Customer 0", ((NarrowOrder)o).Customer.BestFriend.Name);
                            Assert.IsNull(((NarrowOrder)o).Customer.BestFriend.BestFriend);
                            Assert.AreEqual(2, ((NarrowOrder)o).Customer.BestFriend.Orders.Count());

                            // Order/Customer/BestFriend/Orders
                            Assert.AreEqual(0, ((NarrowOrder)o).Customer.BestFriend.Orders[0].ID);
                            Assert.AreEqual(100, ((NarrowOrder)o).Customer.BestFriend.Orders[1].ID);
                            Assert.AreEqual(0, ((NarrowOrder)o).Customer.BestFriend.Orders[0].DollarAmount);
                            Assert.AreEqual(0, ((NarrowOrder)o).Customer.BestFriend.Orders[1].DollarAmount);
                            Assert.IsNull(((NarrowOrder)o).Customer.BestFriend.Orders[0].Customer);
                            Assert.IsNull(((NarrowOrder)o).Customer.BestFriend.Orders[1].Customer);
                        },
                        ExpectedCustomerQueryInterceptorCalls = 2, // because the expansion above is heavily redundant, only 2 calls to the interceptor are made following integration of the ODL uri parser.
                        ExpectedOrderQueryInterceptorCalls = 2,
                    },
                };

                using (Utils.RestoreStaticValueOnDispose(typeof(CustomObjectService), "CustomerChangeInterceptorOverride"))
                using (Utils.RestoreStaticValueOnDispose(typeof(CustomObjectService), "CustomerQueryInterceptorOverride"))
                using (Utils.RestoreStaticValueOnDispose(typeof(CustomObjectService), "OrderChangeInterceptorOverride"))
                using (Utils.RestoreStaticValueOnDispose(typeof(CustomObjectService), "OrderQueryInterceptorOverride"))
                using (CachedConnections.SetupConnection(CachedConnections.ConnectionType.CustomObjectContext))
                {
                    SimpleWorkspace workspace = this.CustomObjectWorkspace;
                    Uri baseUri = new Uri(workspace.ServiceEndPoint + workspace.ServiceContainer.Name + ".svc/", UriKind.Absolute);

                    foreach (var testCase in testCases)
                    {
                        DataServiceContext ctx = new DataServiceContext(baseUri);
                        //ctx.EnableAtom = true;
                        //ctx.Format.UseAtom();

                        ctx.SendingRequest2 += new EventHandler<SendingRequest2EventArgs>((sender, e) =>
                        {
                            Assert.AreEqual(e.RequestMessage.Method, "GET", "Method must be GET");
                            Assert.IsTrue(e.RequestMessage != null && e.RequestMessage is HttpWebRequestMessage, "RequestMessage must be ODataRequestMessage");
                            Assert.IsNotNull(((HttpWebRequestMessage)e.RequestMessage).HttpWebRequest != null, "HttpWebRequest should not be null");
                            Assert.IsFalse(e.IsBatchPart, "Since SaveChanges is called with no parameters, IsBatchPart should always be false");

                            Assert.AreEqual(testCase.QueryString, baseUri.MakeRelativeUri(e.RequestMessage.Url).OriginalString);
                        });

                        int customerChangeInterceptorInvokeCount = 0;
                        int customerQueryInterceptorInvokeCount = 0;
                        int orderChangeInterceptorInvokeCount = 0;
                        int orderQueryInterceptorInvokeCount = 0;

                        CustomObjectService.CustomerChangeInterceptorOverride = (c, option) => { customerChangeInterceptorInvokeCount++; };
                        CustomObjectService.CustomerQueryInterceptorOverride = () => { customerQueryInterceptorInvokeCount++; return c => true; };
                        CustomObjectService.OrderChangeInterceptorOverride = (o, option) => { orderChangeInterceptorInvokeCount++; };
                        CustomObjectService.OrderQueryInterceptorOverride = () => { orderQueryInterceptorInvokeCount++; return o => true; };

                        foreach (object e in testCase.Query(ctx))
                        {
                            testCase.Validate(e);
                            EntityDescriptor descriptor = ctx.GetEntityDescriptor(e);
                            if (descriptor != null)
                            {
                                if (e.GetType() == typeof(NarrowCustomerWithStream))
                                {
                                    Assert.IsFalse(string.IsNullOrEmpty(descriptor.ETag));
                                }
                                else
                                {
                                    Assert.IsTrue(string.IsNullOrEmpty(descriptor.ETag));
                                }
                            }
                        }

                        // Validate Query Interceptors
                        Assert.AreEqual(0, customerChangeInterceptorInvokeCount);
                        Assert.AreEqual(0, orderChangeInterceptorInvokeCount);
                        Assert.AreEqual(testCase.ExpectedCustomerQueryInterceptorCalls, customerQueryInterceptorInvokeCount);
                        Assert.AreEqual(testCase.ExpectedOrderQueryInterceptorCalls, orderQueryInterceptorInvokeCount);
                    }
                }
            }

            [TestMethod]
            public void ChangeInterceptorTests()
            {
                using (Utils.RestoreStaticValueOnDispose(typeof(CustomObjectService), "CustomerChangeInterceptorOverride"))
                using (Utils.RestoreStaticValueOnDispose(typeof(CustomObjectService), "CustomerQueryInterceptorOverride"))
                using (Utils.RestoreStaticValueOnDispose(typeof(CustomObjectService), "OrderChangeInterceptorOverride"))
                using (Utils.RestoreStaticValueOnDispose(typeof(CustomObjectService), "OrderQueryInterceptorOverride"))
                using (CachedConnections.SetupConnection(CachedConnections.ConnectionType.CustomObjectContext))
                {
                    SimpleWorkspace workspace = this.CustomObjectWorkspace;
                    Uri baseUri = new Uri(workspace.ServiceEndPoint + workspace.ServiceContainer.Name + ".svc/", UriKind.Absolute);
                    DataServiceContext ctx = new DataServiceContext(baseUri);
                    //ctx.EnableAtom = true;
                    //ctx.Format.UseAtom();
                    ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;

                    // This TypeResolver is needed because the server sends a media resource on "CustomerBlob" types
                    // and ODataLib validation will try to validate that.
                    ctx.ResolveType = (arg) =>
                    {
                        if (arg.Contains("CustomerBlob"))
                            return typeof(NarrowCustomerWithStream);
                        else
                            return null;
                    };

                    int customerChangeInterceptorInvokeCount = 0;
                    int customerQueryInterceptorInvokeCount = 0;
                    int orderChangeInterceptorInvokeCount = 0;
                    int orderQueryInterceptorInvokeCount = 0;

                    CustomObjectService.CustomerChangeInterceptorOverride = (c, option) =>
                    {
                        customerChangeInterceptorInvokeCount++;
                        Assert.AreEqual("Customer " + c.ID + " Updated", c.Name);
                    };

                    CustomObjectService.OrderChangeInterceptorOverride = (o, option) =>
                    {
                        orderChangeInterceptorInvokeCount++;
                        Assert.AreEqual(123.45, o.DollarAmount);
                    };

                    CustomObjectService.CustomerQueryInterceptorOverride = () => { customerQueryInterceptorInvokeCount++; return c => true; };
                    CustomObjectService.OrderQueryInterceptorOverride = () => { orderQueryInterceptorInvokeCount++; return o => true; };

                    int customerCount = 0;
                    int orderCount = 0;
                    foreach (NarrowCustomer c in ctx.CreateQuery<NarrowCustomer>("Customers").Expand("Orders"))
                    {
                        c.Name = c.Name + " Updated";
                        ctx.UpdateObject(c);
                        customerCount++;
                        foreach (NarrowOrder o in c.Orders)
                        {
                            o.DollarAmount = 123.45;
                            ctx.UpdateObject(o);
                            orderCount++;
                        }
                    }

                    // Validate Interceptors
                    Assert.AreEqual(0, customerChangeInterceptorInvokeCount);
                    Assert.AreEqual(0, orderChangeInterceptorInvokeCount);
                    Assert.AreEqual(1, customerQueryInterceptorInvokeCount);
                    Assert.AreEqual(1, orderQueryInterceptorInvokeCount);

                    customerChangeInterceptorInvokeCount = 0;
                    customerQueryInterceptorInvokeCount = 0;
                    orderChangeInterceptorInvokeCount = 0;
                    orderQueryInterceptorInvokeCount = 0;

                    ctx.SaveChanges();

                    // Validate Query Interceptors
                    Assert.AreEqual(customerCount, customerChangeInterceptorInvokeCount);
                    Assert.AreEqual(orderCount, orderChangeInterceptorInvokeCount);
                    Assert.AreEqual(customerCount, customerQueryInterceptorInvokeCount);
                    Assert.AreEqual(orderCount, orderQueryInterceptorInvokeCount);
                }
            }

            #endregion

            #region Client Entity Descriptor

            [TestMethod]
            public void ClientEntityDescriptorAPITest()
            {
                using (CachedConnections.SetupConnection(CachedConnections.ConnectionType.Northwind))
                {
                    SimpleWorkspace workspace = this.NorthwindWorkspace;
                    Uri baseUri = new Uri(workspace.ServiceEndPoint + workspace.ServiceContainer.Name + ".svc", UriKind.Absolute);
                    DataServiceContext ctx = new northwindClient.northwindContext(baseUri);
                    //ctx.EnableAtom = true;
                    //ctx.Format.UseAtom();

                    // 1: materialization
                    var cust = ctx.CreateQuery<northwindClient.Customers>("Customers").Take(1).FirstOrDefault();
                    var custDescriptor = ctx.GetEntityDescriptor(cust);
                    Assert.AreEqual("northwind.Customers", custDescriptor.ServerTypeName);
                    Assert.IsNotNull(custDescriptor.Identity);
                    Assert.IsNotNull(custDescriptor.EditLink);
                    Assert.AreEqual(EntityStates.Unchanged, custDescriptor.State);

                    // 2: Add via AddObject
                    northwindClient.Customers newCust = northwindClient.Customers.CreateCustomers("Z0001", "Microsoft");
                    ctx.AddObject("Customers", newCust);
                    var newCustDescriptor = ctx.GetEntityDescriptor(newCust);
                    Assert.AreEqual(EntityStates.Added, newCustDescriptor.State);
                    Assert.IsNull(newCustDescriptor.ServerTypeName);
                    Assert.IsNull(newCustDescriptor.ParentForInsert);

                    // 3: Add via AddRelateObject
                    northwindClient.Orders newOrder = northwindClient.Orders.CreateOrders(99999);
                    ctx.AddRelatedObject(cust, "Orders", newOrder);
                    var newOrderDescriptor = ctx.GetEntityDescriptor(newOrder);
                    Assert.AreEqual(EntityStates.Added, newOrderDescriptor.State);
                    Assert.AreEqual(custDescriptor, newOrderDescriptor.ParentForInsert);
                    Assert.AreEqual("Orders", newOrderDescriptor.ParentPropertyForInsert);

                    // 4: Delete
                    ctx.DeleteObject(newCust);
                    Assert.AreEqual(EntityStates.Detached, newCustDescriptor.State);
                    ctx.DeleteObject(cust);
                    Assert.AreEqual(EntityStates.Deleted, custDescriptor.State);
                }
            }

            #endregion

            private class LoadPropertyCallback : AsyncOperationTestCallback
            {
                public override object GetResults(IAsyncResult async)
                {
                    object[] state = (object[])async.AsyncState;
                    string property = (string)state[0];
                    DataServiceContext context = (DataServiceContext)state[1];
                    //context.EnableAtom = true;
                    //context.Format.UseAtom();

                    return context.EndLoadProperty(async);
                }
            }

            private class DoAsyncType : AsyncOperation
            {
                public void DoAsync(Func<AsyncCallback, object, IAsyncResult> beginAction, Action<IAsyncResult> endAction, object state, Action<string> next)
                {
                    DoAsync(next, beginAction, endAction, state);
                }

                public void DoAsync(Action<string> next, Func<AsyncCallback, object, IAsyncResult> beginAction, Action<IAsyncResult> endAction, object state)
                {
                    try
                    {
                        IAsyncResult result = beginAction(
                            delegate(IAsyncResult asyncResult)
                            {
                                try
                                {
                                    endAction(asyncResult);

                                    if (null != next)
                                    {
                                        next(null);
                                    }
                                    else
                                    {
                                        // done
                                    }
                                }
                                catch (AssertFailedException)
                                {
                                    throw;
                                }
                                catch (Exception e)
                                {
                                    HandleStatus(e);
                                }
                                return;
                            },
                            state);
                    }
                    catch (Exception e)
                    {
                        HandleStatus(e);
                        throw;
                    }
                }

                public void HandleStatus(Exception e)
                {
                    CallbackFailure = e;
                    Finished.Set();
                }
            }
        }
    }
}