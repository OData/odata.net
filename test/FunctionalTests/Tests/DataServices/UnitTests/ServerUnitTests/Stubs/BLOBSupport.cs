//---------------------------------------------------------------------
// <copyright file="BLOBSupport.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.Data.SqlClient;
    using System.Data.Test.Astoria;
    using System.Data.EntityClient;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Xml;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Tests;
    using System.Threading;
    using NorthwindModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Text;
    using System.Net;
using System.CodeDom.Compiler;

    #endregion Namespaces

    public class NorthwindNamedStreamServiceFactory
    {
        private static Assembly NorthwindNamedStreamModelAssembly;
        private static Type NorthwindNamedStreamServiceType;

        private static string NorthwindModelDirectory
        {
            get
            {
                string root = TestUtil.EnlistmentRoot;
                if (String.IsNullOrEmpty(root))
                {
                    throw new InvalidOperationException("Unable to read MetadataDirectory " +
                        "because the ENLISTMENT_ROOT environment variable is not defined.");
                }

                return Path.Combine(root, @"test\FunctionalTests\Tests\DataServices\Models\northwind\");
            }
        }

        private static string SDKRefDirectory
        {
            get
            {
                return System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
            }
        }

        private static void InitializeModelAssembly()
        {
            if (NorthwindNamedStreamServiceType == null)
            {
                var p = CodeDomProvider.CreateProvider("CSharp");

                CompilerParameters cp = new CompilerParameters()
                {
                    GenerateExecutable = false,
                    GenerateInMemory = true,
                    TreatWarningsAsErrors = false
                };

                string sdkRefDir = SDKRefDirectory;
                cp.ReferencedAssemblies.Add(sdkRefDir + "System.dll");
                cp.ReferencedAssemblies.Add(sdkRefDir + "System.Core.dll");
                cp.ReferencedAssemblies.Add(sdkRefDir + "System.Data.dll");
                cp.ReferencedAssemblies.Add(sdkRefDir + "System.Data.Entity.dll");
                cp.ReferencedAssemblies.Add(Environment.ExpandEnvironmentVariables(Path.Combine(Environment.CurrentDirectory, DataFxAssemblyRef.File.DataServicesClient)));
                cp.ReferencedAssemblies.Add(Environment.ExpandEnvironmentVariables(Path.Combine(Environment.CurrentDirectory, DataFxAssemblyRef.File.ODataLib)));
                cp.ReferencedAssemblies.Add(sdkRefDir + "System.Runtime.Serialization.dll");
                cp.ReferencedAssemblies.Add(sdkRefDir + "System.Xml.dll");
                String[] paths = (new DirectoryInfo(NorthwindModelDirectory)).GetFiles("*.cs").Select(f => f.FullName).ToArray();
                CompilerResults results = p.CompileAssemblyFromFile(cp, paths);

                Assert.AreEqual(0, results.Errors.Count);
                Assert.IsNotNull(results.CompiledAssembly);

                NorthwindNamedStreamModelAssembly = results.CompiledAssembly;
                Type contextType = NorthwindNamedStreamModelAssembly.GetType("NorthwindModel.NorthwindContext");
                Type serviceType = typeof(NorthwindStreamServiceBase<>).MakeGenericType(contextType);

                NorthwindNamedStreamServiceType = serviceType;
            }
        }

        public static Type ResolveType(string typeName)
        {
            InitializeModelAssembly();
            return NorthwindNamedStreamModelAssembly.GetType(typeName);
        }

        public static Type GetNamedStreamServiceType()
        {
            InitializeModelAssembly();            
            return NorthwindNamedStreamServiceType;
        }        
    }
    
    public class NorthwindDefaultStreamService : NorthwindStreamServiceBase<NorthwindModel.NorthwindContext>
    {
        public static Func<Type, object> GetServiceOverride = null;
                
        [ChangeInterceptor("Customers")]
        public void OnChangeCustomer(NorthwindModel.Customers c, UpdateOperations operation)
        {
            Interlocked.Increment(ref InterceptorChecker.ChangeInterceptorInvokeCount);
        }

        [QueryInterceptor("Customers")]
        public Expression<Func<NorthwindModel.Customers, bool>> OnQueryCustomers()
        {
            Interlocked.Increment(ref InterceptorChecker.QueryInterceptorInvokeCount);
            return p => true;
        }

        [QueryInterceptor("Orders")]
        public Expression<Func<NorthwindModel.Orders, bool>> OnQueryOrders()
        {
            Interlocked.Increment(ref InterceptorChecker.QueryInterceptorInvokeCount);
            return p => true;
        }

        [QueryInterceptor("Order_Details")]
        public Expression<Func<NorthwindModel.Order_Details, bool>> OnQueryOrderDetails()
        {
            Interlocked.Increment(ref InterceptorChecker.QueryInterceptorInvokeCount);
            return p => true;
        }
    }
    
    public class NorthwindStreamServiceBase<T> : NorthwindTempDbServiceBase<T>, IServiceProvider
    {
        public const string NamedStreamsElementName = "NamedStreams";
        public const string NamedStreamElementName = "NamedStream";
        public const string StreamNameAttributeName = "Name";
        private const string HasStreamAttribute = "HasStream";

        public NorthwindStreamServiceBase()
        {
            this.ProcessingPipeline.ProcessingRequest += new EventHandler<DataServiceProcessingPipelineEventArgs>(BlobDataServicePipelineHandlers.ProcessingRequestHandler);
            this.ProcessingPipeline.ProcessingChangeset += new EventHandler<EventArgs>(BlobDataServicePipelineHandlers.ProcessingChangesetHandler);
            this.ProcessingPipeline.ProcessedChangeset += new EventHandler<EventArgs>(BlobDataServicePipelineHandlers.ProcessedChangesetHandler);
            this.ProcessingPipeline.ProcessedRequest += new EventHandler<DataServiceProcessingPipelineEventArgs>(BlobDataServicePipelineHandlers.ProcessedRequestHandler);
        }

        #region IServiceProvider Members

        object IServiceProvider.GetService(Type serviceType)
        {
            if (NorthwindDefaultStreamService.GetServiceOverride != null)
            {
                return NorthwindDefaultStreamService.GetServiceOverride(serviceType);
            }

            if (serviceType == typeof(IDataServiceStreamProvider))
            {
                return new DataServiceStreamProvider();
            }

            return null;
        }

        #endregion

        #region SetupNorthwind

        /// <summary>
        /// Allows to set up Northwind database that have streams and etags
        /// </summary>
        /// <param name="mediaLinkEntiries">List of types to mark with HasStream attribute. Key is the type name. Value is attribute value.</param>
        /// <param name="namedStreams">List of types and the corresponding named streams declared on them. Key is the type name, Value is the list of named streams declared on the type.</param>
        /// <returns></returns>
        public static IDisposable SetupNorthwindWithStream(
            KeyValuePair<string, string>[] mediaLinkEntiries,
            string testName)
        {
            // Create a copy of northwind files.
            string sourcePath = NorthwindDefaultTempDbService.MetadataDirectory;
            string targetPath = Path.Combine(TestUtil.GeneratedFilesLocation, testName);
            IOUtil.EnsureEmptyDirectoryExists(targetPath);

            string sourceCSDL = Path.Combine(sourcePath, "Northwind.csdl");
            string targetCSDL = Path.Combine(targetPath, "Northwind.csdl");
            string sourceSSDL = Path.Combine(sourcePath, "Northwind.ssdl");
            string targetSSDL = Path.Combine(targetPath, "Northwind.ssdl");
            string sourceMSL = Path.Combine(sourcePath, "Northwind.msl");
            string targetMSL = Path.Combine(targetPath, "Northwind.msl");

            XmlDocument csdlDoc = new XmlDocument();
            csdlDoc.Load(sourceCSDL);

            // Add dataservice metadata namespace
            XmlElement csdlSchema = TestUtil.AssertSelectSingleElement(csdlDoc, string.Format("/csdl1:Schema"));
            csdlSchema.SetAttribute("xmlns:m", UnitTestsUtil.MetadataNamespace.NamespaceName);

            if (mediaLinkEntiries != null)
            {
                foreach (KeyValuePair<string, string> type in mediaLinkEntiries)
                {
                    // Modify the CSDL to include HasStreamAttribute.
                    TestUtil.AssertSelectSingleElement(csdlDoc,
                        string.Format("/csdl1:Schema/csdl1:EntityType[@Name='{0}']", type.Key)).SetAttribute(HasStreamAttribute, UnitTestsUtil.MetadataNamespace.NamespaceName, type.Value);
                }
            }

            csdlDoc.Save(targetCSDL);
            System.Diagnostics.Trace.WriteLine("Saved document to " + targetCSDL);
            IOUtil.CopyFileIfNewer(sourceSSDL, targetSSDL);
            IOUtil.CopyFileIfNewer(sourceMSL, targetMSL);

            // Rebuild the connection string.
            string metadata = targetCSDL + "|" + targetMSL + "|" + targetSSDL;
            return new RestoreNorthwindConectionString(metadata);
        }

        public class RestoreNorthwindConectionString : IDisposable
        {
            private string savedConnectionString;
            private IDisposable disposeNorthwind;

            public RestoreNorthwindConectionString(string metadataString)
            {
                savedConnectionString = NorthwindDefaultTempDbService.ContextConnectionString;

                SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
                sqlBuilder.DataSource = DataUtil.DefaultDataSource;
                sqlBuilder.IntegratedSecurity = true;
                sqlBuilder.MultipleActiveResultSets = true;
                sqlBuilder.ConnectTimeout = 600;

                var entityBuilder = new EntityConnectionStringBuilder();
                entityBuilder.Metadata = metadataString;
                entityBuilder.Provider = "System.Data.SqlClient";
                entityBuilder.ProviderConnectionString = sqlBuilder.ConnectionString;

                NorthwindDefaultTempDbService.ContextConnectionString = entityBuilder.ConnectionString;
                System.Data.Metadata.Edm.MetadataWorkspace.ClearCache();
                TestUtil.ClearMetadataCache();
                disposeNorthwind = NorthwindDefaultTempDbService.SetupNorthwind();

                InterceptorChecker.Reset();
            }

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                NorthwindDefaultTempDbService.ContextConnectionString = savedConnectionString;
                TestUtil.ClearMetadataCache();
                System.Data.Metadata.Edm.MetadataWorkspace.ClearCache();
                disposeNorthwind.Dispose();

                InterceptorChecker.Reset();
            }

            #endregion
        }

        #endregion SetupNorthwind
    }


    public class PhotoDataService : DataService<PhotoDataServiceContext>, IServiceProvider
    {
        public static EntitySetRights Right = EntitySetRights.All;
        public static ServiceOperationRights ServiceOpRights = ServiceOperationRights.All;
        public static bool EnablePipelineVerification = true;

        public static void InitializeService(DataServiceConfiguration config)
        {
            ((DataServiceConfiguration)config).DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
            config.SetEntitySetAccessRule("*", Right);
            config.SetServiceOperationAccessRule("*", ServiceOpRights);

            config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
            config.DataServiceBehavior.AcceptProjectionRequests = true;
            config.UseVerboseErrors = true;

            //config.UseV4ExpandSyntax = false;
        }

        [ChangeInterceptor("Folders")]
        public void OnChangeFolder(Folder f, UpdateOperations operation)
        {
            Interlocked.Increment(ref InterceptorChecker.ChangeInterceptorInvokeCount);
        }

        [QueryInterceptor("Folders")]
        public Expression<Func<Folder, bool>> OnQueryFolder()
        {
            Interlocked.Increment(ref InterceptorChecker.QueryInterceptorInvokeCount);
            return f => true;
        }

        [ChangeInterceptor("Items")]
        public void OnChangeItem(Item p, UpdateOperations operation)
        {
            Interlocked.Increment(ref InterceptorChecker.ChangeInterceptorInvokeCount);
        }

        [QueryInterceptor("Items")]
        public Expression<Func<Item, bool>> OnQueryItem()
        {
            Interlocked.Increment(ref InterceptorChecker.QueryInterceptorInvokeCount);
            return p => true;
        }

        public PhotoDataService()
        {
            if (EnablePipelineVerification)
            {
                this.ProcessingPipeline.ProcessingRequest += new EventHandler<DataServiceProcessingPipelineEventArgs>(BlobDataServicePipelineHandlers.ProcessingRequestHandler);
                this.ProcessingPipeline.ProcessingChangeset += new EventHandler<EventArgs>(BlobDataServicePipelineHandlers.ProcessingChangesetHandler);
                this.ProcessingPipeline.ProcessedChangeset += new EventHandler<EventArgs>(BlobDataServicePipelineHandlers.ProcessedChangesetHandler);
                this.ProcessingPipeline.ProcessedRequest += new EventHandler<DataServiceProcessingPipelineEventArgs>(BlobDataServicePipelineHandlers.ProcessedRequestHandler);
            }
        }

        #region IServiceProvider Members

        object IServiceProvider.GetService(Type serviceType)
        {
            if (serviceType == typeof(IDataServiceStreamProvider))
            {
                return new DataServiceStreamProvider();
            }

            return null;
        }

        #endregion

        [System.ServiceModel.Web.WebGet]
        public Photo GetPhoto()
        {
            return (Photo)this.CurrentDataSource.Items.Where(i => i.GetType() == typeof(Photo)).FirstOrDefault();
        }
    }

    public class InterceptorChecker
    {
        public static int ChangeInterceptorInvokeCount = 0;
        public static int QueryInterceptorInvokeCount = 0;

        public static void Reset()
        {
            ChangeInterceptorInvokeCount = 0;
            QueryInterceptorInvokeCount = 0;
        }

        public static void ValidateChangeInterceptor(int expectedCount)
        {
            int currentCount = ChangeInterceptorInvokeCount;

            ChangeInterceptorInvokeCount = 0;

            if (currentCount != expectedCount)
            {
                throw new InvalidOperationException(string.Format("ChangeInterceptorInvokeCount is {0}, expected count is {1}", currentCount, expectedCount));
            }
        }

        public static void ValidateQueryInterceptor(int expectedCount)
        {
            int currentCount = QueryInterceptorInvokeCount;

            QueryInterceptorInvokeCount = 0;

            if (currentCount != expectedCount)
            {
                throw new InvalidOperationException(string.Format("QueryInterceptorInvokeCount is {0}, expected count is {1}", currentCount, expectedCount));
            }
        }
    }

    public static class BlobDataServicePipelineHandlers
    {
        public static int ProcessingRequestInvokeCount;
        public static int ProcessedRequestInvokeCount;
        public static int ProcessingChangesetInvokeCount;
        public static int ProcessedChangesetInvokeCount;
        public static bool CheckInvokeCount = true;

        public static void ProcessingRequestHandler(object sender, DataServiceProcessingPipelineEventArgs e)
        {
            ProcessingRequestInvokeCount++;
            e.OperationContext.ResponseHeaders["ProcessingRequestInvokeCount"] = ProcessingRequestInvokeCount.ToString();

            if (CheckInvokeCount)
            {
                Assert.AreEqual(1, ProcessingRequestInvokeCount);
                Assert.AreEqual(0, ProcessedRequestInvokeCount);
                Assert.AreEqual(0, ProcessingChangesetInvokeCount);
                Assert.AreEqual(0, ProcessedChangesetInvokeCount);
            }
        }

        public static void ProcessedRequestHandler(object sender, DataServiceProcessingPipelineEventArgs e)
        {
            ProcessedRequestInvokeCount++;
            e.OperationContext.ResponseHeaders["ProcessedRequestInvokeCount"] = ProcessedRequestInvokeCount.ToString();

            if (CheckInvokeCount)
            {
                Assert.AreEqual(1, ProcessingRequestInvokeCount);

                if (!e.OperationContext.IsBatchRequest)
                {
                    if (e.OperationContext.RequestMethod == "GET")
                    {
                        Assert.AreEqual(0, ProcessingChangesetInvokeCount);
                        Assert.AreEqual(0, ProcessedChangesetInvokeCount);
                    }
                    else
                    {
                        Assert.AreEqual(1, ProcessingChangesetInvokeCount);
                        Assert.AreEqual(1, ProcessedChangesetInvokeCount);
                    }
                }

                Assert.AreEqual(1, ProcessedRequestInvokeCount);
            }
        }

        public static void ProcessingChangesetHandler(object sender, EventArgs e)
        {
            ProcessingChangesetInvokeCount++;

            if (CheckInvokeCount)
            {
                Assert.AreEqual(1, ProcessingRequestInvokeCount);
                Assert.AreEqual(0, ProcessedRequestInvokeCount);
                Assert.IsTrue(ProcessingChangesetInvokeCount > ProcessedChangesetInvokeCount);
            }
        }

        public static void ProcessedChangesetHandler(object sender, EventArgs e)
        {
            ProcessedChangesetInvokeCount++;

            if (CheckInvokeCount)
            {
                Assert.AreEqual(1, ProcessingRequestInvokeCount);
                Assert.AreEqual(0, ProcessedRequestInvokeCount);
                Assert.IsTrue(ProcessingChangesetInvokeCount >= ProcessedChangesetInvokeCount);
            }
        }
    }

    public class PhotoDataServiceContext : IUpdatable
    {
        #region Entity Sets

        private Dictionary<int, object> tokens = new Dictionary<int, object>();

        public static List<Item> _items;
        public static List<Folder> _folders;

        public static int NextItemID = 0;
        public static int NextFolderID = 0;


        public IQueryable<Item> Items
        {
            get { return _items.AsQueryable<Item>(); }
        }

        public IQueryable<Folder> Folders
        {
            get { return _folders.AsQueryable<Folder>(); }
        }

        #endregion Entity Sets

        #region Basic Plumbing

        private List<KeyValuePair<object, EntityState>> _pendingChanges;
        private static bool? preserveChanges;

        public PhotoDataServiceContext()
        {
            if (preserveChanges == false || _items == null)
            {
                PopulateData();
            }
        }

        public List<KeyValuePair<object, EntityState>> PendingChanges
        {
            get
            {
                if (_pendingChanges == null)
                {
                    _pendingChanges = new List<KeyValuePair<object, EntityState>>();
                }

                return _pendingChanges;
            }
        }

        public static void PopulateData()
        {
            ClearData();

            // populate data here...
            Item i = new Item()
            {
                ID = NextItemID++,
                Description = "Default Item0",
                Name = "Item0",
            };
            _items.Add(i);

            Photo p = new Photo()
            {
                ID = NextItemID++,
                Description = "Default Photo1",
                Name = "Photo1",
                Rating = 3,
            };
            _items.Add(p);

            DerivedFromPhoto p2 = new DerivedFromPhoto()
            {
                ID = NextItemID++,
                Description = "Derived Photo2",
                Name = "Photo2",
                Rating = 3,
            };
            _items.Add(p2);

            DerivedFromPhoto p3 = new DerivedFromPhoto()
            {
                ID = NextItemID++,
                Description = "A Derived Photo3",
                Name = "A Photo2",
                Rating = 3,
            };
            _items.Add(p3);


            DataServiceStreamProvider.Init();

            using (Stream s = File.OpenWrite(DataServiceStreamProvider.GetStoragePath(p)))
            {
                byte[] buffer = new byte[] { 1, 2, 3, 4 };
                s.Write(buffer, 0, 4);
                s.Close();
            }

            Folder f = new Folder()
            {
                ID = NextFolderID++,
                Name = "Folder1"
            };
            f.Items.Add(i);
            i.ParentFolder = f;
            f.Icon = p;
            i.Icon = p;

            f.Items.Add(p);
            p.ParentFolder = f;
            p.Icon = p;

            f.Items.Add(p2);
            p2.ParentFolder = f;
            p2.Icon = p;
            _folders.Add(f);

            Folder f1 = new Folder()
            {
                ID = NextFolderID++,
                Name = "A Folder2"
            };

            f1.Items.Add(p3);
            p3.ParentFolder = f1;
            f1.Icon = p;
            p3.Icon = p3;
        }

        public static void ClearData()
        {
            _items = new List<Item>();
            _folders = new List<Folder>();

            NextItemID = 0;
            NextFolderID = 0;

            IOUtil.EnsureEmptyDirectoryExists(DataServiceStreamProvider.RootStoragePath);
        }

        private static IList GetEntitySet(Type entityType)
        {
            if (_items == null)
            {
                PopulateData();
            }

            if (typeof(Item).IsAssignableFrom(entityType))
            {
                return _items;
            }

            if (typeof(Folder).IsAssignableFrom(entityType))
            {
                return _folders;
            }

            throw new Exception("Unexpected EntityType encountered: " + entityType.FullName);
        }

        private static void AddResource(object resource, bool throwIfDuplicate)
        {
            IList entitySetInstance = GetEntitySet(resource.GetType());

            // check if there is not another instance with the same id
            object dup = TryGetEntity(entitySetInstance, resource);
            if (dup != null)
            {
                if (throwIfDuplicate)
                {
                    throw new DataServiceException(400, String.Format("Entity with the same key already present. EntityType: '{0}'",
                        resource.GetType().Name));
                }

                // if its already there, do not add it to the global context
                return;
            }

            entitySetInstance.Add(resource);
        }

        private void DeleteEntity(IEnumerable collection, object entity, bool throwIfNotPresent)
        {
            object entityToBeDeleted = TryGetEntity(collection, entity);

            if (entityToBeDeleted == null && throwIfNotPresent)
            {
                throw new Exception("No entity found with the given ID");
            }

            if (entityToBeDeleted != null)
            {
                // Make sure that property type implements ICollection<T> If yes, then call remove method on it to remove the
                // resource
                Type elementType = TestUtil.GetTypeParameter(collection.GetType(), typeof(ICollection<>), 0);
                typeof(ICollection<>).MakeGenericType(elementType).InvokeMember(
                                                "Remove",
                                                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                                                null,
                                                collection,
                                                new object[] { entityToBeDeleted });
            }
        }

        private static object TryGetEntity(IEnumerable collection, object entity)
        {
            object matchingEntity = null;

            foreach (object element in collection)
            {
                // check if there is not another instance with the same id
                if (Equal(element, entity))
                {
                    matchingEntity = element;
                    break;
                }
            }

            return matchingEntity;
        }

        private static bool Equal(object resource1, object resource2)
        {
            if (resource1.GetType() != resource2.GetType())
            {
                return false;
            }

            // check if there is not another instance with the same id
            return (bool)resource1.GetType().InvokeMember("Equals",
                                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod,
                                        null,
                                        resource1,
                                        new object[] { resource2 });
        }

        private object ResourceToToken(object resource)
        {
            int token = this.tokens.Count;
            this.tokens[token] = resource;
            return token;
        }

        private object TokenToResource(object token)
        {
            return this.tokens[(int)token];
        }

        private static bool IsPrimitiveType(Type type)
        {
            return (type.IsPrimitive ||
                    type == typeof(String) ||
                    type == typeof(Guid) ||
                    type == typeof(Decimal) ||
                    type == typeof(DateTime) ||
                    type == typeof(byte[]));
        }

        #endregion Basic Plumbing

        #region Change Scope

        public static IDisposable CreateChangeScope()
        {
            if (preserveChanges.HasValue && preserveChanges.Value)
            {
                throw new InvalidOperationException("Changes are already being preserved.");
            }

            PopulateData();
            preserveChanges = true;
            return new ChangeScope();
        }

        private class ChangeScope : IDisposable
        {
            public ChangeScope()
            {
                InterceptorChecker.Reset();
            }

            public void Dispose()
            {
                preserveChanges = false;
                PopulateData();
                InterceptorChecker.Reset();
            }
        }

        #endregion Change Scope

        #region IUpdatable Members

        void IUpdatable.AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            targetResource = this.TokenToResource(targetResource);
            resourceToBeAdded = this.TokenToResource(resourceToBeAdded);

            object propertyValue = targetResource.GetType().InvokeMember(
                propertyName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty,
                null,
                targetResource,
                null);

            propertyValue.GetType().InvokeMember(
                "Add",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                null,
                propertyValue,
                new object[] { resourceToBeAdded });
        }

        void IUpdatable.ClearChanges()
        {
            PendingChanges.Clear();
        }

        object IUpdatable.CreateResource(string containerName, string fullTypeName)
        {
            object resource = null;
            if (containerName == "Items" && fullTypeName == typeof(Item).FullName)
            {
                Item i = new Item();
                i.ID = NextFolderID++;
                resource = i;
            }
            else if (containerName == "Items" && fullTypeName == typeof(Photo).FullName)
            {
                Photo p = new Photo();
                p.ID = NextItemID++;
                resource = p;
            }
            else if (containerName == "Folders" && fullTypeName == typeof(Folder).FullName)
            {
                Folder f = new Folder();
                f.ID = NextFolderID++;
                resource = f;
            }

            if (resource == null)
            {
                throw new InvalidOperationException(String.Format("Invalid container name '{0}' or type name specified '{1}'", containerName, fullTypeName));
            }
            else
            {
                PendingChanges.Add(new KeyValuePair<object, EntityState>(resource, EntityState.Added));
            }

            return this.ResourceToToken(resource);
        }

        void IUpdatable.DeleteResource(object targetResource)
        {
            targetResource = this.TokenToResource(targetResource);
            PendingChanges.Add(new KeyValuePair<object, EntityState>(targetResource, EntityState.Deleted));
        }

        object IUpdatable.GetResource(IQueryable query, string fullTypeName)
        {
            object resource = null;

            foreach (object r in query)
            {
                if (resource != null)
                {
                    throw new ArgumentException(String.Format("Invalid Uri specified. The query '{0}' must refer to a single resource", query.ToString()));
                }

                resource = r;
            }

            if (resource != null && fullTypeName != null)
            {
                if (resource.GetType().FullName != fullTypeName)
                {
                    throw new ArgumentException(String.Format("Invalid uri specified. ExpectedType: '{0}', ActualType: '{1}'", fullTypeName, resource.GetType().FullName));
                }
            }

            return this.ResourceToToken(resource);
        }

        object IUpdatable.GetValue(object token, string propertyName)
        {
            object targetResource = this.TokenToResource(token);

            object propertyValue = targetResource.GetType().InvokeMember(
                                        propertyName,
                                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty,
                                        null,
                                        targetResource,
                                        null);

            if (propertyValue != null && !IsPrimitiveType(propertyValue.GetType()))
            {
                propertyValue = this.ResourceToToken(propertyValue);
            }

            return propertyValue;
        }

        void IUpdatable.RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            targetResource = this.TokenToResource(targetResource);
            resourceToBeRemoved = this.TokenToResource(resourceToBeRemoved);

            object propertyValue = targetResource.GetType().InvokeMember(
                propertyName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty,
                null,
                targetResource,
                null);

            propertyValue.GetType().InvokeMember(
                "Remove",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                null,
                propertyValue,
                new object[] { resourceToBeRemoved });
        }

        object IUpdatable.ResetResource(object token)
        {
            Debug.Assert(token != null, "token != null");
            object resource = this.TokenToResource(token);

            if (typeof(Item).IsAssignableFrom(resource.GetType()))
            {
                ((Item)resource).ReInit();
            }
            else if (typeof(Folder).IsAssignableFrom(resource.GetType()))
            {
                ((Folder)resource).ReInit();
            }
            else
            {
                throw new InvalidOperationException("Unsupported type '" + resource.GetType().FullName + "'.");
            }

            return token;
        }

        object IUpdatable.ResolveResource(object resource)
        {
            return this.TokenToResource(resource);
        }

        void IUpdatable.SaveChanges()
        {
            foreach (KeyValuePair<object, EntityState> pendingChange in this.PendingChanges)
            {
                // find the entity set for the object
                IList entitySetInstance = GetEntitySet(pendingChange.Key.GetType());

                switch (pendingChange.Value)
                {
                    case EntityState.Added:
                        AddResource(pendingChange.Key, true /*throwIfDuplicate*/);
                        break;
                    case EntityState.Deleted:
                        DeleteEntity(entitySetInstance, pendingChange.Key, true /*throwIfNotPresent*/);
                        if (typeof(Item).IsAssignableFrom(pendingChange.Key.GetType()))
                        {
                            Item toBeDeleted = (Item)pendingChange.Key;
                            foreach (Folder f in this.Folders)
                            {
                                DeleteEntity(f.Items, toBeDeleted, false);
                                if (null != f.Icon && f.Icon.ID == toBeDeleted.ID)
                                {
                                    f.Icon = null;
                                }
                            }
                            foreach (Item i in this.Items)
                            {
                                DeleteEntity(i.RelatedItems, toBeDeleted, false);
                                if (null != i.Icon && i.Icon.ID == toBeDeleted.ID)
                                {
                                    i.Icon = null;
                                }
                            }
                        }
                        else if (typeof(Folder).IsAssignableFrom(pendingChange.Key.GetType()))
                        {
                            Folder toBeDeleted = (Folder)pendingChange.Key;
                            foreach (Folder f in this.Folders)
                            {
                                DeleteEntity(f.Folders, toBeDeleted, false);
                            }
                            foreach (Item i in this.Items)
                            {
                                DeleteEntity(i.RelatedFolders, toBeDeleted, false);
                                if (i.ParentFolder != null && i.ParentFolder.ID == toBeDeleted.ID)
                                {
                                    i.ParentFolder = null;
                                }
                            }
                        }
                        break;
                    default:
                        throw new Exception("Unsupported State");
                }
            }

            this.PendingChanges.Clear();
        }

        void IUpdatable.SetReference(object token, string propertyName, object propertyValueToken)
        {
            object targetResource = this.TokenToResource(token);
            object propertyValue = null;

            if (propertyValueToken != null)
            {
                propertyValue = this.TokenToResource(propertyValueToken);
            }

            PropertyInfo pi = targetResource.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);

            if (propertyValue != null && !pi.PropertyType.IsAssignableFrom(propertyValue.GetType()))
            {
                throw new DataServiceException(
                    400,
                    String.Format("Bad Request. The resource type '{0}' is not a valid type for the property '{1}' in resource '{2}'. Please make sure that the uri refers to the correct type",
                                  propertyValue.GetType().FullName, propertyName, targetResource.GetType().FullName));
            }

            pi.SetValue(targetResource, propertyValue, null);
        }

        void IUpdatable.SetValue(object token, string propertyName, object propertyValue)
        {
            object targetResource = this.TokenToResource(token);

            if (propertyValue != null && !IsPrimitiveType(propertyValue.GetType()))
            {
                propertyValue = this.TokenToResource(propertyValue);
            }

            PropertyInfo pi = targetResource.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);

            if (propertyValue != null && !pi.PropertyType.IsAssignableFrom(propertyValue.GetType()))
            {
                throw new DataServiceException(
                    400,
                    String.Format("Bad Request. The resource type '{0}' is not a valid type for the property '{1}' in resource '{2}'. Please make sure that the uri refers to the correct type",
                                  propertyValue.GetType().FullName, propertyName, targetResource.GetType().FullName));
            }

            pi.SetValue(targetResource, propertyValue, null);
        }

        #endregion
    }

}
