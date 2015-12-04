//---------------------------------------------------------------------
// <copyright file="SpatialImplementationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.Data.Spatial;
using Xunit;

namespace Microsoft.Spatial.Tests
{
    public class SpatialImplementationTests
    {
        [Fact]
        public void CurrentImplementationNotNull()
        {
            Assert.True(SpatialImplementation.CurrentImplementation != null, "There must be default implementation registered always");
            Assert.True(typeof(DataServicesSpatialImplementation) == SpatialImplementation.CurrentImplementation.GetType(), "expecting the default implementation to be registered");
        }

        [Fact]
        public void SpatialOperationsRegistration()
        {
            var operations = new BaseSpatialOperations();
            SpatialImplementation.CurrentImplementation.Operations = operations;
            Assert.True(SpatialImplementation.CurrentImplementation.Operations == operations, "Must be the same instance");

            try
            {
                SpatialImplementation.CurrentImplementation.Operations = operations;
            }
            catch (ArgumentException e)
            {
                Assert.Equal("There are already operations registered with priority 0.3.", e.Message);
            }
        }

        /// <summary>
        /// Non-abstract version of the base class which does not override any methods
        /// </summary>
        private class BaseSpatialOperations : SpatialOperations
        {
        }
    }
}
