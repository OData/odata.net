//---------------------------------------------------------------------
// <copyright file="DataServicesSpatialImplementationDataDrivenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataSpatialUnitTests.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public partial class DataServicesSpatialImplementationTests
    {
        string [] spatialPipelineReturnTestedMethods = new string [] 
            {
                 "CreateBuilder",
                 "CreateValidator",
            };
    
        [TestMethod]
        public void TestSpatialPipelineReturn_CreateBuilder()
        {
            var pipeline = testSubject.CreateBuilder();
            AssertPipelineIsForwardingSegment(pipeline);
        }
    
        [TestMethod]
        public void TestSpatialPipelineReturn_CreateValidator()
        {
            var pipeline = testSubject.CreateValidator();
            AssertPipelineIsForwardingSegment(pipeline);
        }
    }
}