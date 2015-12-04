//---------------------------------------------------------------------
// <copyright file="DataServicesSpatialImplementationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
using System.Reflection;
using Microsoft.Data.Spatial;
using Xunit;

namespace Microsoft.Spatial.Tests
{
    public class DataServicesSpatialImplementationTests
    {
        private DataServicesSpatialImplementation testSubject;

        private string[] spatialPipelineReturnTestedMethods =
        {
                "CreateBuilder",
                "CreateValidator",
        };

        public DataServicesSpatialImplementationTests()
        {
            testSubject = new DataServicesSpatialImplementation();
        }

        [Fact]
        public void TestSpatialPipelineReturn_CreateBuilder()
        {
            var pipeline = testSubject.CreateBuilder();
            AssertPipelineIsForwardingSegment(pipeline);
        }

        [Fact]
        public void TestSpatialPipelineReturn_CreateValidator()
        {
            var pipeline = testSubject.CreateValidator();
            AssertPipelineIsForwardingSegment(pipeline);
        }

        [Fact]
        public void MissingSpatialPipelineReturnMethodsTest()
        {
            var methods = typeof(DataServicesSpatialImplementation).GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var method in methods.Where(m => m.Name.Contains("Create") && m.ReturnType != null && typeof(SpatialPipeline).IsAssignableFrom(m.ReturnType)))
            {
                if (!spatialPipelineReturnTestedMethods.Contains(method.Name))
                {
                    Assert.True(false, "Not testing the SpatialPipeline return of method " + method.Name + " to be sure it is wrapped in a forwarding pipeline.");
                }
            }
        }

        private void AssertPipelineIsForwardingSegment(SpatialPipeline pipeline)
        {
            Assert.True(pipeline.GeographyPipeline != null, "The GeographyPipeline should not be null");
            Assert.True(pipeline.GeometryPipeline != null, "The GeometryPipeline should not be null");
            Assert.True(typeof(ForwardingSegment.GeographyForwarder) == pipeline.GeographyPipeline.GetType(), "All pipelines that we create must be wrapped in forwarding segments for exception/reset behavior to work");
            Assert.True(typeof(ForwardingSegment.GeometryForwarder) == pipeline.GeometryPipeline.GetType(), "All pipelines that we create must be wrapped in forwarding segments for exception/reset behavior to work");
        }
    }
}
