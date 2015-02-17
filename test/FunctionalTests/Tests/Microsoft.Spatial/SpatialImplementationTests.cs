//---------------------------------------------------------------------
// <copyright file="SpatialImplementationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataSpatialUnitTests.Tests
{
    using System;
    using Microsoft.Spatial;
    using Microsoft.Data.Spatial;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SpatialImplementationTests
    {
        [TestMethod]
        public void CurrentImplementationNotNull()
        {
            Assert.IsNotNull(SpatialImplementation.CurrentImplementation, "There must be default implementation registered always");
            Assert.AreEqual(typeof(DataServicesSpatialImplementation), SpatialImplementation.CurrentImplementation.GetType(), "expecting the default implementation to be registered");
        }

        [TestMethod]
        public void SpatialOperationsRegistration()
        {
            var operations = new BaseSpatialOperations();
            SpatialImplementation.CurrentImplementation.Operations = operations;
            Assert.AreSame(SpatialImplementation.CurrentImplementation.Operations, operations, "Must be the same instance");

            try
            {
                SpatialImplementation.CurrentImplementation.Operations = operations;
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("There are already operations registered with priority 0.3.", e.Message);
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
