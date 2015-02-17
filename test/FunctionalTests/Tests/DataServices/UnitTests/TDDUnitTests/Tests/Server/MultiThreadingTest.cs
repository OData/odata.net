//---------------------------------------------------------------------
// <copyright file="MultiThreadingTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Reflection;
using Microsoft.OData.Client;

namespace AstoriaUnitTests.Tests.Server
{
    [TestClass]
    public class MultiThreadingTest
    {
        Exception backgroundException = null;

        [TestMethod]
        public void MultiThreadCount()
        {
            // verify the operation of multi-thread runner
            int count = 0;
            RunTargetOnMultipleThreads(() => Interlocked.Increment(ref count));
            Assert.AreEqual(5, count);
        }

        [TestMethod]
        public void DynamicProxyMethodGeneratorTest()
        {
            // DynamicProxyMethodGenerator.dynamicProxyMethods is not thread safe
            var dynamicProxyMethodGenerator = new DynamicProxyMethodGenerator();
            // Method must be within the client dll
            MethodInfo mi = typeof(DataServiceContext).GetMethod("GetMetadataUri", BindingFlags.Public | BindingFlags.Instance);
            RunTargetOnMultipleThreads(() => dynamicProxyMethodGenerator.GetCallWrapper(mi));
        }

        private void RunTargetOnMultipleThreads(Action target, int numOfThreads = 5)
        {
            backgroundException = null;
            Thread[] threads = new Thread[numOfThreads];

            for (int i = 0; i < numOfThreads; ++i)
            {
                threads[i] = new Thread(() => InvokeAction(target));
            }

            for (int i = 0; i < numOfThreads; ++i)
            {
                threads[i].Start();
            }

            for (int i = 0; i < numOfThreads; ++i)
            {
                threads[i].Join();
            }

            Assert.IsNull(backgroundException, "Exception thrown in the background " + backgroundException);
        }

        private void InvokeAction(Action target)
        {
            try
            {
                target();
            }
            catch (Exception ex)
            {
                backgroundException = ex;
            }
        }
    }
}
