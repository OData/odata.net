//---------------------------------------------------------------------
// <copyright file="DataServiceExecutionProviderWrapperTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Service.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using Microsoft.OData.Service;
using AstoriaUnitTests.Tests.Server.Simulators;

namespace AstoriaUnitTests.Tests.Server
{
    
    /// <summary>
    ///This is a test class for DataServiceExecutionProviderWrapperTest and is intended
    ///to contain all DataServiceExecutionProviderWrapperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DataServiceExecutionProviderWrapperTests
    {
        #region Test Preperation

        private ServiceSimulatorFactory serviceFactory;

        private const string CUSTOM_EXE_PROVIDER_INVOKE_RETURN = "Custom Execution Provider's Return Value on all Invoke calls";

        internal class CustomExecutionProviderService : DataServiceSimulator, IDataServiceExecutionProvider, IServiceProvider
        {
            public object Execute(Expression requestExpression, DataServiceOperationContext context)
            {
                return CUSTOM_EXE_PROVIDER_INVOKE_RETURN;
            }

            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(IDataServiceExecutionProvider))
                {
                    return this;
                }

                return null;
            }
        }

        public DataServiceExecutionProviderWrapperTests()
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
        public void InvokeDefaultProvider()
        {
            IDataService service = this.serviceFactory.CreateService();
            DataServiceExecutionProviderWrapper providerWrapper = new DataServiceExecutionProviderWrapper(service);
            Expression exp = Expression.Constant(1);

            object expected = 1;
            object actual = providerWrapper.Execute(exp);
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
