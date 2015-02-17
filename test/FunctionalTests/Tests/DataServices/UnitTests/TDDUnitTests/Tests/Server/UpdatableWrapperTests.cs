//---------------------------------------------------------------------
// <copyright file="UpdatableWrapperTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests.Server
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Caching;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using System.Reflection;
    using AstoriaUnitTests.Tests.Server.Simulators;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Strings = Microsoft.OData.Service;

    /// <summary>
    ///This is a test class for UpdatableWrapper and is intended to contain all UpdatableWrapper Unit Tests
    ///</summary>
    [TestClass()]
    public class UpdatableWrapperTests
    {
        #region Test Preperation

        private ServiceSimulatorFactory serviceFactory;

        internal class CustomUpdatableProviderService : DataServiceSimulator, IDataServiceUpdateProvider, IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(IDataServiceUpdateProvider))
                {
                    return this;
                }

                return null;
            }

            #region IUpdatable
            public object CreateResource(string containerName, string fullTypeName)
            {
                throw new NotImplementedException();
            }

            public object GetResource(IQueryable query, string fullTypeName)
            {
                throw new NotImplementedException();
            }

            public object ResetResource(object resource)
            {
                throw new NotImplementedException();
            }

            public void SetValue(object targetResource, string propertyName, object propertyValue)
            {
                throw new NotImplementedException();
            }

            public object GetValue(object targetResource, string propertyName)
            {
                throw new NotImplementedException();
            }

            public void SetReference(object targetResource, string propertyName, object propertyValue)
            {
                throw new NotImplementedException();
            }

            public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
            {
                throw new NotImplementedException();
            }

            public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
            {
                throw new NotImplementedException();
            }

            public void DeleteResource(object targetResource)
            {
                throw new NotImplementedException();
            }

            public void SaveChanges()
            {
                throw new NotImplementedException();
            }

            public object ResolveResource(object resource)
            {
                throw new NotImplementedException();
            }

            public void ClearChanges()
            {
                throw new NotImplementedException();
            }

            public void SetConcurrencyValues(object resourceCookie, bool? checkForEquality, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, object>> concurrencyValues)
            {
                throw new NotImplementedException();
            }
            #endregion IUpdatable
        }

        public UpdatableWrapperTests()
        {
            this.serviceFactory = new ServiceSimulatorFactory();
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            this.serviceFactory.ClearQueryArgument();
        }

        #endregion

        #region Tests

        [TestMethod()]
        public void UpdateProvider2PropertyTest()
        {
            IDataService service = this.serviceFactory.CreateService<CustomUpdatableProviderService>();
            UpdatableWrapper updatable = new UpdatableWrapper(service);
            foreach (ODataProtocolVersion version in new[] { ODataProtocolVersion.V4 })
            {
                service.Configuration.DataServiceBehavior.MaxProtocolVersion = version;
                try
                {
                    updatable.GetType().GetProperty("UpdateProvider2", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(updatable, null);
                }
                catch (TargetInvocationException tie)
                {
                    InvalidOperationException e = (InvalidOperationException)tie.InnerException;
                    Assert.AreEqual("To support service actions, the data service must implement IServiceProvider.GetService() to return an implementation of IDataServiceUpdateProvider2 or the data source must implement IDataServiceUpdateProvider2.", e.Message);
                }
            }
        }

        internal class CustomUpdateProvider2Service : DataServiceSimulator, IServiceProvider, IDataServiceUpdateProvider2
        {
            internal static bool ImplementsIUpdatable = false;
            internal static bool ImplementsIDSUP = false;
            internal static bool ImplementsIDSUP2 = false;

            internal static int IUpdatableGetCount = 0;
            internal static int IDataServiceUpdateProviderGetCount = 0;
            internal static int IDataServiceUpdateProvider2GetCount = 0;

            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(IUpdatable))
                {
                    CustomUpdateProvider2Service.IUpdatableGetCount++;
                    if (CustomUpdateProvider2Service.ImplementsIUpdatable)
                    {
                        return this;
                    }
                }
                else if (serviceType == typeof(IDataServiceUpdateProvider))
                {
                    CustomUpdateProvider2Service.IDataServiceUpdateProviderGetCount++;
                    if (CustomUpdateProvider2Service.ImplementsIDSUP)
                    {
                        return this;
                    }
                }
                else if (serviceType == typeof(IDataServiceUpdateProvider2))
                {
                    CustomUpdateProvider2Service.IDataServiceUpdateProvider2GetCount++;
                    if (CustomUpdateProvider2Service.ImplementsIDSUP2)
                    {
                        return this;
                    }
                }

                return null;
            }

            #region IDataServiceUpdateProvider2 interface implementation

            public void ScheduleInvokable(IDataServiceInvokable invokable)
            {
                throw new NotImplementedException();
            }

            public void SetConcurrencyValues(object resourceCookie, bool? checkForEquality, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, object>> concurrencyValues)
            {
                throw new NotImplementedException();
            }

            public object CreateResource(string containerName, string fullTypeName)
            {
                throw new NotImplementedException();
            }

            public object GetResource(IQueryable query, string fullTypeName)
            {
                throw new NotImplementedException();
            }

            public object ResetResource(object resource)
            {
                throw new NotImplementedException();
            }

            public void SetValue(object targetResource, string propertyName, object propertyValue)
            {
                throw new NotImplementedException();
            }

            public object GetValue(object targetResource, string propertyName)
            {
                throw new NotImplementedException();
            }

            public void SetReference(object targetResource, string propertyName, object propertyValue)
            {
                throw new NotImplementedException();
            }

            public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
            {
                throw new NotImplementedException();
            }

            public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
            {
                throw new NotImplementedException();
            }

            public void DeleteResource(object targetResource)
            {
                throw new NotImplementedException();
            }

            public void SaveChanges()
            {
                throw new NotImplementedException();
            }

            public object ResolveResource(object resource)
            {
                throw new NotImplementedException();
            }

            public void ClearChanges()
            {
                throw new NotImplementedException();
            }

            #endregion IDataServiceUpdateProvider2 interface implementation
        }

        private DataServiceProviderWrapper CreateProviderWrapper(DataServiceConfiguration config, bool hasReflectionOrEFProviderQueryBehavior)
        {
            IDataServiceMetadataProvider provider;
            if (hasReflectionOrEFProviderQueryBehavior)
            {
                provider = new ReflectionDataServiceProvider(new object(), new object());
            }
            else
            {
                provider = new DataServiceProviderSimulator();
            }

            var dataService = new DataServiceSimulator();

            DataServiceStaticConfiguration staticConfiguration = new DataServiceStaticConfiguration(dataService.Instance.GetType(), provider);
            IDataServiceProviderBehavior providerBehavior = hasReflectionOrEFProviderQueryBehavior ? new DataServiceProviderBehavior(new ProviderBehavior(ProviderQueryBehaviorKind.ReflectionProviderQueryBehavior)) : DataServiceProviderBehavior.CustomDataServiceProviderBehavior;

            var providerWrapper = new DataServiceProviderWrapper(
                new DataServiceCacheItem(config, staticConfiguration),
                provider,
                (IDataServiceQueryProvider)provider,
                dataService,
                hasReflectionOrEFProviderQueryBehavior ? true : false);

            dataService.ProcessingPipeline = new DataServiceProcessingPipeline();
            dataService.Provider = providerWrapper;
            typeof(DataServiceProviderWrapper).GetField("providerBehavior", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(providerWrapper, providerBehavior);
#if DEBUG
            dataService.ProcessingPipeline.SkipDebugAssert = true;
#endif
            return providerWrapper;
        }

        [TestMethod()]
        public void UpdateProviderPropertyTest()
        {
            IDataService service = this.serviceFactory.CreateService<CustomUpdateProvider2Service>();
            ODataProtocolVersion[] versions = new[] { ODataProtocolVersion.V4 };
            bool[] boolValues = new[] { true, false };
            List<string> failureCombinations = new List<string>();

            foreach (var version in versions)
            {
                foreach (var hasReflectionOrEFProviderQueryBehavior in boolValues)
                {
                    foreach (var implementsIUpdatable in boolValues)
                    {
                        foreach (var implementsIDSUP in boolValues)
                        {
                            foreach (var implementsIDSUP2 in boolValues)
                            {
                                service.Configuration.DataServiceBehavior.MaxProtocolVersion = version;
                                CustomUpdateProvider2Service.ImplementsIUpdatable = implementsIUpdatable;
                                CustomUpdateProvider2Service.ImplementsIDSUP = implementsIDSUP;
                                CustomUpdateProvider2Service.ImplementsIDSUP2 = implementsIDSUP2;
                                CustomUpdateProvider2Service.IUpdatableGetCount = 0;
                                CustomUpdateProvider2Service.IDataServiceUpdateProviderGetCount = 0;
                                CustomUpdateProvider2Service.IDataServiceUpdateProvider2GetCount = 0;

                                ((DataServiceSimulator)service).Provider = CreateProviderWrapper(service.Configuration, hasReflectionOrEFProviderQueryBehavior);
                                UpdatableWrapper updatable = new UpdatableWrapper(service);
                                try
                                {
                                    var instance = updatable.GetType().GetProperty("UpdateProvider", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(updatable, null);
                                    Assert.IsNotNull(instance, "UpdatableWrapper should throw if UpdateProvider fails to get the interface.");
                                    Assert.AreEqual(1, CustomUpdateProvider2Service.IDataServiceUpdateProvider2GetCount);

                                    // None of this code is actually hit, we need to investigate this.
                                    if (!CustomUpdateProvider2Service.ImplementsIDSUP2)
                                    {
                                        if (hasReflectionOrEFProviderQueryBehavior)
                                        {
                                            Assert.AreEqual(1, CustomUpdateProvider2Service.IUpdatableGetCount);
                                        }
                                        else
                                        {
                                            Assert.AreEqual(0, CustomUpdateProvider2Service.IUpdatableGetCount);
                                        }

                                        if (!CustomUpdateProvider2Service.ImplementsIUpdatable)
                                        {
                                            Assert.AreEqual(1, CustomUpdateProvider2Service.IDataServiceUpdateProviderGetCount);
                                        }
                                        else
                                        {
                                            if (hasReflectionOrEFProviderQueryBehavior)
                                            {
                                                Assert.AreEqual(0, CustomUpdateProvider2Service.IDataServiceUpdateProviderGetCount);
                                            }
                                            else
                                            {
                                                Assert.AreEqual(1, CustomUpdateProvider2Service.IDataServiceUpdateProviderGetCount);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Assert.AreEqual(0, CustomUpdateProvider2Service.IUpdatableGetCount);
                                        Assert.AreEqual(0, CustomUpdateProvider2Service.IDataServiceUpdateProviderGetCount);
                                    }
                                }
                                catch (TargetInvocationException tie)
                                {
                                    string errorCaseVariation = string.Format("Version:{0}, hasReflectionOrEFProviderQueryBehavior:{1}, implementsIUpdatable:{2}, implementsIDSUP:{3}, implementsIDSUP2:{4}", version, hasReflectionOrEFProviderQueryBehavior, implementsIUpdatable, implementsIDSUP, implementsIDSUP2);
                                    failureCombinations.Add(errorCaseVariation);
                                    DataServiceException e = (DataServiceException)tie.InnerException;
                                    Assert.AreEqual(501, e.StatusCode);
                                    if (hasReflectionOrEFProviderQueryBehavior)
                                    {
                                        Assert.AreEqual("The data source must implement IUpdatable, IDataServiceUpdateProvider or IDataServiceUpdateProvider2 to support updates.", e.Message);
                                    }
                                    else
                                    {
                                        Assert.AreEqual("The data source must implement IDataServiceUpdateProvider or IDataServiceUpdateProvider2 to support updates.", e.Message);
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

        #endregion
    }
}
