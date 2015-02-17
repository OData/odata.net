//---------------------------------------------------------------------
// <copyright file="DataServiceExecutionProviderTests.cs" company="Microsoft">
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
    ///This is a test class for DataServiceExecutionProviderTest and is intended
    ///to contain all DataServiceExecutionProviderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DataServiceExecutionProviderTests
    {
        #region Test Preperation

        private static readonly Uri baseUri = new Uri("http://localhost/service/");

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

            public int ReturnOne()
            {
                return 1;
            }
        }

        public DataServiceExecutionProviderTests()
        {
            this.serviceFactory = new ServiceSimulatorFactory();
        }

        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            this.serviceFactory.ClearQueryArgument();
        }


        #endregion

        #region Tests

        /// <summary>
        /// A simple test to ensure that DataServiceExecutionProvider.Invoke() uses 
        /// ExpressionEvaluator.Evaluate().
        ///</summary>
        [TestMethod()]
        public void InvokeConstantExpTest()
        {
            DataServiceExecutionProvider provider = new DataServiceExecutionProvider();
            Expression requestExpression = Expression.Constant(5);

            // our provider doesn't care about the context, it just evaluates the expression
            var service = this.serviceFactory.CreateService();

            object expected = ExpressionEvaluator.Evaluate(requestExpression);
            object actual = provider.Execute(requestExpression, service.OperationContext);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected, 5);
        }

        #endregion
    }
}
