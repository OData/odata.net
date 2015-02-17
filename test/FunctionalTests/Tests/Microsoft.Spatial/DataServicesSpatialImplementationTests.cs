//---------------------------------------------------------------------
// <copyright file="DataServicesSpatialImplementationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataSpatialUnitTests.Tests
{
    using System.Linq;
    using System.Reflection;
    using Microsoft.Spatial;
    using Microsoft.Data.Spatial;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public partial class DataServicesSpatialImplementationTests
    {
        private DataServicesSpatialImplementation testSubject;

        [TestInitialize]
        public void TestInitialize()
        {
            testSubject = new DataServicesSpatialImplementation();
        }

        // see DataServicesSpatialImplementationDataDrivenTest.tt for the definition of the data driven tests

        [TestMethod]
        public void MissingSpatialPipelineReturnMethodsTest()
        {
            var methods = typeof(DataServicesSpatialImplementation).GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var method in methods.Where(m => m.Name.Contains("Create") && m.ReturnType != null && typeof(SpatialPipeline).IsAssignableFrom(m.ReturnType)))
            {
                if (!spatialPipelineReturnTestedMethods.Contains(method.Name))
                {
                    Assert.Fail("Not testing the SpatialPipeline return of method " + method.Name + " to be sure it is wrapped in a forwarding pipeline.");
                }
            }
        }
        
        
        private void AssertPipelineIsForwardingSegment(SpatialPipeline pipeline)
        {
            Assert.IsNotNull(pipeline.GeographyPipeline, "The GeographyPipeline should not be null");
            Assert.IsNotNull(pipeline.GeometryPipeline, "The GeometryPipeline should not be null");
            Assert.AreEqual(typeof(ForwardingSegment.GeographyForwarder), pipeline.GeographyPipeline.GetType(), "All pipelines that we create must be wrapped in forwarding segments for exception/reset behavior to work");
            Assert.AreEqual(typeof(ForwardingSegment.GeometryForwarder), pipeline.GeometryPipeline.GetType(), "All pipelines that we create must be wrapped in forwarding segments for exception/reset behavior to work");
        }


    }
}
