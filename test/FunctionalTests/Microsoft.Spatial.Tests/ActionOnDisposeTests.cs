//---------------------------------------------------------------------
// <copyright file="ActionOnDisposeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Xunit;

namespace Microsoft.Spatial.Tests
{
    public class ActionOnDisposeTests
    {
        [Fact]
        public void ActionIsExecutedOnDisposeMethodCall()
        {
            bool actionExecuted = false;
            IDisposable disposable = new ActionOnDispose(() => actionExecuted = true);
            Assert.False(actionExecuted);
            disposable.Dispose();
            Assert.True(actionExecuted);
        }

        [Fact]
        public void ActionIsExecutedPostUsing()
        {
            bool actionExecuted = false;
            using (IDisposable disposable = new ActionOnDispose(() => actionExecuted = true))
            {
                Assert.False(actionExecuted);
            }

            Assert.True(actionExecuted);
        }

        [Fact]
        public void ActionIsExecutedOnceWhenDisposeIsCalledTwice()
        {
            bool actionExecuted = false;
            IDisposable disposable = new ActionOnDispose(() => actionExecuted = true);
            Assert.False(actionExecuted);
            disposable.Dispose();
            Assert.True(actionExecuted);

            // the action should only fire the first time
            actionExecuted = false;
            disposable.Dispose();
            Assert.False(actionExecuted);
        }
    }
}
