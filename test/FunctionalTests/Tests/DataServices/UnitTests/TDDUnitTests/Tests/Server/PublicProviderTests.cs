//---------------------------------------------------------------------
// <copyright file="PublicProviderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Caching;
    using Microsoft.OData.Service.Providers;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using FluentAssertions;
    using AstoriaUnitTests.TDD.Tests.Server.Simulators;
    using AstoriaUnitTests.Tests.Server.Simulators;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class Author
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public IList<Book> Books { get; set; }
    }

    public class Book
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public float Price { get; set; }
        public Author Author { get; set; }
    }

    public class ReflectionBookStore
    {
        private static Author[] authors = new[] { new Author { ID = 1, Name = "Erich Gamma", Age = 35 } };
        private static Book[] books = new[] { new Book { ID = 1, Title = "Design Patterns", Price = 20.0f } };

        static ReflectionBookStore()
        {
            authors[0].Books = new List<Book>();
            authors[0].Books.Add(books[0]);
            books[0].Author = authors[0];
        }

        public IQueryable<Author> Authors
        {
            get
            {
                return authors.AsQueryable();
            }
        }

        public IQueryable<Book> Books
        {
            get
            {
                return books.AsQueryable();
            }
        }
    }

    public class ReflectionBookStoreDataService : DataService<ReflectionBookStore>
    {
        [QueryInterceptor("Books")]
        public Expression<Func<Book, bool>> FilterBooks()
        {
            return b => true;
        }

        [ChangeInterceptor("Authors")]
        public void AuthorChangeHandler(Author a, UpdateOperations op)
        {
        }

        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
        }
    }

    #region Create a provider by implementing the interfaces and using internal Reflection provider as a wrapper

    public class BookStoreWithReflectionProvider : ReflectionBookStore, IDataServiceMetadataProvider, IDataServiceQueryProvider
    {
        private readonly ReflectionDataServiceProvider wrappedProvider;

        public BookStoreWithReflectionProvider(object dataServiceInstance)
        {
            this.wrappedProvider = new ReflectionDataServiceProvider(new DataServiceProviderArgs(dataServiceInstance, this, new Type[0], false));
        }

        public object CurrentDataSource
        {
            get
            {
                return this.wrappedProvider.CurrentDataSource;
            }
            set
            {
                this.wrappedProvider.CurrentDataSource = value;
            }
        }

        #region other IDataService____Provider methods which simply delegate to the wrapped provider
        public bool TryResolveResourceSet(string name, out ResourceSet resourceSet)
        {
            return wrappedProvider.TryResolveResourceSet(name, out resourceSet);
        }

        public ResourceAssociationSet GetResourceAssociationSet(ResourceSet resourceSet, ResourceType resourceType,
                                                                ResourceProperty resourceProperty)
        {
            return wrappedProvider.GetResourceAssociationSet(resourceSet, resourceType, resourceProperty);
        }

        public bool TryResolveResourceType(string name, out ResourceType resourceType)
        {
            return wrappedProvider.TryResolveResourceType(name, out resourceType);
        }

        public bool TryResolveServiceOperation(string name, out ServiceOperation serviceOperation)
        {
            return wrappedProvider.TryResolveServiceOperation(name, out serviceOperation);
        }

        public IEnumerable<ResourceType> GetDerivedTypes(ResourceType resourceType)
        {
            return wrappedProvider.GetDerivedTypes(resourceType);
        }

        public bool HasDerivedTypes(ResourceType resourceType)
        {
            return wrappedProvider.HasDerivedTypes(resourceType);
        }

        public IQueryable GetQueryRootForResourceSet(ResourceSet resourceSet)
        {
            return wrappedProvider.GetQueryRootForResourceSet(resourceSet);
        }

        public ResourceType GetResourceType(object target)
        {
            return wrappedProvider.GetResourceType(target);
        }

        public object GetPropertyValue(object target, ResourceProperty resourceProperty)
        {
            return wrappedProvider.GetPropertyValue(target, resourceProperty);
        }

        public object GetOpenPropertyValue(object target, string propertyName)
        {
            return wrappedProvider.GetOpenPropertyValue(target, propertyName);
        }

        public IEnumerable<KeyValuePair<string, object>> GetOpenPropertyValues(object target)
        {
            return wrappedProvider.GetOpenPropertyValues(target);
        }

        public object InvokeServiceOperation(ServiceOperation serviceOperation, object[] parameters)
        {
            return wrappedProvider.InvokeServiceOperation(serviceOperation, parameters);
        }

        public void FinalizeMetadataModel(IEnumerable<Type> knownTypes, bool useMetadataCacheOrder)
        {
            wrappedProvider.FinalizeMetadataModel(knownTypes, useMetadataCacheOrder);
        }

        public IEnumerable<KeyValuePair<string, object>> GetEntityContainerAnnotations(string entityContainerName)
        {
            return wrappedProvider.GetEntityContainerAnnotations(entityContainerName);
        }

        public object GetService(Type serviceType)
        {
            return wrappedProvider.GetService(serviceType);
        }

        public void Dispose()
        {
            wrappedProvider.Dispose();
        }

        public string ContainerName
        {
            get { return wrappedProvider.ContainerName; }
        }

        public bool IsNullPropagationRequired
        {
            get { return wrappedProvider.IsNullPropagationRequired; }
        }

        public string ContainerNamespace
        {
            get { return wrappedProvider.ContainerNamespace; }
        }

        public IEnumerable<ResourceType> Types
        {
            get { return wrappedProvider.Types; }
        }

        public IEnumerable<ServiceOperation> ServiceOperations
        {
            get { return wrappedProvider.ServiceOperations; }
        }

        public IEnumerable<ResourceSet> ResourceSets
        {
            get { return wrappedProvider.ResourceSets; }
        }
        #endregion
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults=true)]
    public class BookStoreDataServiceWithReflectionProvider : DataService<BookStoreWithReflectionProvider>
    {
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
            config.UseVerboseErrors = true;
        }

        protected override BookStoreWithReflectionProvider CreateDataSource()
        {
            return new BookStoreWithReflectionProvider(this);
        }
    }

    #endregion

    #region Create a provider by inheriting from in-built Reflection Provider 

    public class BookStoreProvider : ReflectionDataServiceProvider
    {
        public BookStoreProvider(DataServiceProviderArgs args) 
            : base(args)
        {
        }
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults=true)]
    public class BookStoreDataServiceWithInheritedReflectionProvider : DataService<BookStoreProvider>
    {
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
            config.UseVerboseErrors = true;
        }

        protected override BookStoreProvider CreateDataSource()
        {
            return new BookStoreProvider(new DataServiceProviderArgs(this, new ReflectionBookStore(), new Type[0], false));
        }
    }

    #endregion

    [TestClass]
    public class PublicProviderTests
    {
        [TestMethod]
        public void ReflectionDataServiceProviderBehavior()
        {
            ReflectionDataServiceProvider reflectionProvider = new ReflectionDataServiceProvider(new ReflectionBookStoreDataService(), new ReflectionBookStore());
            IDataServiceProviderBehavior providerBehavior = (IDataServiceProviderBehavior)reflectionProvider.GetService(typeof(IDataServiceProviderBehavior));
            Assert.IsTrue(providerBehavior.ProviderBehavior.ProviderQueryBehavior == ProviderQueryBehaviorKind.ReflectionProviderQueryBehavior);
        }

        [TestMethod]
        public void EntityFrameworkDataServiceProviderBehavior()
        {
            ObjectContextServiceProvider objContextProvider = new ObjectContextServiceProvider(new object(), new object());
            IDataServiceProviderBehavior providerBehavior = objContextProvider as IDataServiceProviderBehavior;
            Assert.IsTrue(providerBehavior.ProviderBehavior.ProviderQueryBehavior == ProviderQueryBehaviorKind.EntityFrameworkProviderQueryBehavior);
        }

        private class NullReturningProviderBehavior : IDataServiceProviderBehavior
        {
            public ProviderBehavior ProviderBehavior
            {
                get { return null; }
            }
        }

        [TestMethod]
        public void ProviderBehaviorNullPropertyCheck()
        {
            bool success = false;
            try
            {
                DataServiceProviderBehavior.GetBehavior(new NullReturningProviderBehavior());
            }
            catch (InvalidOperationException)
            {
                success = true;
            }

            Assert.IsTrue(success);

        }

        [TestMethod]
        public void ProviderBehaviorNonNullPropertyCheck()
        {
            bool success = true;
            try
            {
                DataServiceProviderBehavior.GetBehavior(DataServiceProviderBehavior.CustomDataServiceProviderBehavior);
            }
            catch (InvalidOperationException)
            {
                success = false;
            }

            Assert.IsTrue(success);

        }


        [TestMethod]
        public void StaticConfigurationCaching()
        {
            ReflectionBookStoreDataService service = new ReflectionBookStoreDataService();
            service.AttachHost(new DataServiceHostSimulator 
            { 
                AbsoluteServiceUri = new Uri("http://www.temp.org"), 
                AbsoluteRequestUri = new Uri("http://www.temp.org/Authors") , 
                RequestHttpMethod = "GET",
                ResponseStream = new MemoryStream()
            });
            
            service.ProcessRequest();

            DataServiceCacheItem cacheItem = MetadataCache<DataServiceCacheItem>.TryLookup(service.GetType(), new object());
            DataServiceStaticConfiguration oldStaticConfig = cacheItem.StaticConfiguration;

            service.AttachHost(new DataServiceHostSimulator 
            {
                AbsoluteServiceUri = new Uri("http://www.temp.org"), 
                AbsoluteRequestUri = new Uri("http://www.temp.org/Books"), 
                RequestHttpMethod = "GET",
                ResponseStream = new MemoryStream()
            });

            service.ProcessRequest();

            cacheItem = MetadataCache<DataServiceCacheItem>.TryLookup(service.GetType(), new object());
            DataServiceStaticConfiguration newStaticConfig = cacheItem.StaticConfiguration;
            Assert.AreEqual(oldStaticConfig, newStaticConfig);

            ResourceSet rsBooks = cacheItem.ResourceSetWrapperCache["Books"].ResourceSet;
            var queryInterceptors = newStaticConfig.GetReadAuthorizationMethods(rsBooks);
            Assert.AreEqual(queryInterceptors.Count(), 1);

            ResourceSet rsAuthors = cacheItem.ResourceSetWrapperCache["Authors"].ResourceSet;
            var changeInterceptors = newStaticConfig.GetWriteAuthorizationMethods(rsAuthors);
            Assert.AreEqual(changeInterceptors.Count(), 1);
        }

        [TestMethod]
        public void ServiceOperationProviderNoMethods()
        {
            ServiceOperationProvider p = new ServiceOperationProvider(typeof(ReflectionBookStoreDataService), t => null, (rt, mi) => null);
            Assert.AreEqual(p.ServiceOperations.Count(), 0);
        }

        [TestMethod]
        public void ServiceOperationProviderNullArgumentsCheck()
        {
            Assert.IsFalse(ConstructServiceOperationProvider(null, null, null));
            Assert.IsFalse(ConstructServiceOperationProvider(null, null, (rt, mi) => null));
            Assert.IsFalse(ConstructServiceOperationProvider(null, t => null, null));
            Assert.IsFalse(ConstructServiceOperationProvider(null, t => null, (rt, mi) => null));
            Assert.IsFalse(ConstructServiceOperationProvider(typeof(ReflectionBookStoreDataService), null, null));
            Assert.IsFalse(ConstructServiceOperationProvider(typeof(ReflectionBookStoreDataService), null, (rt, mi) => null));
            Assert.IsFalse(ConstructServiceOperationProvider(typeof(ReflectionBookStoreDataService), t => null, null));
            Assert.IsTrue(ConstructServiceOperationProvider(typeof(ReflectionBookStoreDataService), t => null, (rt, mi) => null));
        }

        public abstract class PPAbstractType
        {
        }

        [TestMethod]
        public void ServiceOperationProviderAbstractTypeCheck()
        {
            Assert.IsFalse(ConstructServiceOperationProvider(typeof(PPAbstractType), t => null, (rt, mi) => null));
        }

        private bool ConstructServiceOperationProvider(Type t, Func<Type, ResourceType> rt, Func<ResourceType, MethodInfo, ResourceSet> rs)
        {
            try
            {
                ServiceOperationProvider p = new ServiceOperationProvider(t, rt, rs);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }


        public class PPComplexType
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
        }

        public class PPEntityType
        {
            public int ID { get; set; }
            public PPComplexType ComplexProperty { get; set; }
        }

        public class PPServiceOperationMethodClass
        {
            [WebGet]
            public PPEntityType EntityResultMethod(int arg)
            {
                return new PPEntityType { ID = 1, ComplexProperty = new PPComplexType { Field1 = 1, Field2 = "SomeString" } };
            }

            [WebGet]
            public IQueryable<PPEntityType> QueryableEntityResultMethod()
            {
                return new[] { new PPEntityType { ID = 1, ComplexProperty = new PPComplexType { Field1 = 1, Field2 = "SomeString" } } }.AsQueryable();
            }

            [WebInvoke]
            public IEnumerable<PPComplexType> ComplexResultMethod(float arg)
            {
                return Enumerable.Empty<PPComplexType>();
            }

            [WebInvoke]
            public string PrimitiveMethod(int arg, float arg2, DateTime arg3, decimal arg4)
            {
                return "SomeString";
            }
        }

        private static Dictionary<Type, ResourceType> typeToRTMap = new Dictionary<Type,ResourceType>()
        {
            { typeof(PPComplexType), new ResourceType(typeof(PPComplexType), ResourceTypeKind.ComplexType, null, "PPNamespace", "PPComplexType", false) },
            { typeof(PPEntityType), new ResourceType(typeof(PPEntityType), ResourceTypeKind.EntityType, null, "PPNamespace", "PPEntityType", false) },
        };

        private static ResourceType ResolveResourceType(Type t)
        {
            return typeToRTMap[t];
        }

        private static ResourceSet ResolveResourceSet(ResourceType rt, MethodInfo mi)
        {
            if (rt.Name == "PPEntityType")
            {
                return new ResourceSet("PPEntitySet", rt);
            }

            return null;
        }

        [TestMethod]
        public void ServiceOperationProviderNonServiceClass()
        {
            ServiceOperationProvider sop = new ServiceOperationProvider(typeof(PPServiceOperationMethodClass), ResolveResourceType, ResolveResourceSet);
            ServiceOperation[] sos = sop.ServiceOperations.ToArray();
            Assert.AreEqual(sos.Length, 4);
        }


        class ReflectionBookStoreDataServiceWithOps : ReflectionBookStoreDataService
        {
            [WebGet]
            public IEnumerable<Book> GetBooksById(int id)
            {
                return this.CurrentDataSource.Books.Where(b => b.ID == id);
            }
        }

        [TestMethod]
        public void ReflectionProviderServiceOpsLoaded()
        {
            ReflectionDataServiceProvider reflectionProvider = new ReflectionDataServiceProvider(
                new DataServiceProviderArgs(
                    new ReflectionBookStoreDataServiceWithOps(), 
                    new ReflectionBookStore(), 
                    null, 
                    false));

            Assert.AreEqual(reflectionProvider.ServiceOperations.Count(), 1);
        }

        class ReflectionBookStoreDataServiceWithOps2 : ReflectionBookStoreDataServiceWithOps
        {
        }

        [TestMethod]
        public void ReflectionProviderNoServiceOpsLoaded()
        {
            ReflectionDataServiceProvider reflectionProvider = new ReflectionDataServiceProvider(
                new DataServiceProviderArgs(
                    new ReflectionBookStoreDataServiceWithOps2(),
                    new ReflectionBookStore(),
                    null,
                    false) { SkipServiceOperationMetadata = true });

            Assert.AreEqual(reflectionProvider.ServiceOperations.Count(), 0);
        }

        [TestMethod]
        public void ServiceWithReflectionProviderTryingToSetCurrentDataSourceShouldNotThrow()
        {
            // In-box providers do not allow CurrentDataSource to be set, causing an exception in any wrapping provider that simply delegates to it
            var service = new BookStoreDataServiceWithReflectionProvider();
            service.AttachHost(new DataServiceHostSimulator
            {
                AbsoluteServiceUri = new Uri("http://www.temp.org"),
                AbsoluteRequestUri = new Uri("http://www.temp.org/Authors"),
                RequestHttpMethod = "GET",
                ResponseStream = new MemoryStream()
            });

            Action action = service.ProcessRequest;

            action.ShouldNotThrow("By default, a setter for CurrentDataSource is called. It should not throw");
        }

        [TestMethod]
        public void ServiceWithInheritedReflectionProviderTryingToSetCurrentDataSourceShouldNotThrow()
        {
            // In-box providers do not allow CurrentDataSource to be set, causing an exception in any wrapping provider that simply delegates to it
            var service = new BookStoreDataServiceWithInheritedReflectionProvider();
            service.AttachHost(new DataServiceHostSimulator
            {
                AbsoluteServiceUri = new Uri("http://www.temp.org"),
                AbsoluteRequestUri = new Uri("http://www.temp.org/Authors"),
                RequestHttpMethod = "GET",
                ResponseStream = new MemoryStream()
            });

            Action action = service.ProcessRequest;

            action.ShouldNotThrow("By default, a setter for CurrentDataSource is called. It should not throw");
        }

    }
}
