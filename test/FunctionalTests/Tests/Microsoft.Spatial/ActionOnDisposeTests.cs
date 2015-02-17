//---------------------------------------------------------------------
// <copyright file="ActionOnDisposeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataSpatialUnitTests.Tests
{
    using System;
    using Microsoft.Spatial;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ActionOnDisposeTests
    {
        [TestMethod]
        public void ActionIsExecutedOnDisposeMethodCall()
        {
            bool actionExecuted = false;
            IDisposable disposable = new ActionOnDispose(() => actionExecuted = true);
            Assert.IsFalse(actionExecuted);
            disposable.Dispose();
            Assert.IsTrue(actionExecuted);
        }

        [TestMethod]
        public void ActionIsExecutedPostUsing()
        {
            bool actionExecuted = false;
            using (IDisposable disposable = new ActionOnDispose(() => actionExecuted = true))
            {
                Assert.IsFalse(actionExecuted);
            }

            Assert.IsTrue(actionExecuted);
        }

        [TestMethod]
        public void ActionIsExecutedOnceWhenDisposeIsCalledTwice()
        {
            bool actionExecuted = false;
            IDisposable disposable = new ActionOnDispose(() => actionExecuted = true);
            Assert.IsFalse(actionExecuted);
            disposable.Dispose();
            Assert.IsTrue(actionExecuted);

            // the action should only fire the first time
            actionExecuted = false;
            disposable.Dispose();
            Assert.IsFalse(actionExecuted);
        }
    }
}
