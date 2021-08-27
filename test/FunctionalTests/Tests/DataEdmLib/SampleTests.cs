//---------------------------------------------------------------------
// <copyright file="SampleTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    /// <summary>
    /// Sample test case, which can run in both:
    ///   1. MSTest
    ///   2. Taupo Runner
    /// </summary>
    /// <remarks>
    /// Current Limitation for MSTest:
    /// - Cannot use Assert in MSTest, use Assert instead
    /// Current Limitation for Taupo Runner:
    /// - Cannot use test case and variation parameters
    /// </remarks>
    [TestClass]
    public class SampleTests : EdmLibTestCaseBase
    {
        [TestMethod]
        public void DummyTest_Succeed()
        {
		    // to ensure running in the lab, please use Assert
			// do not use Assert in MsTest!
            Assert.IsTrue(true, "Should always suceed.");
        }

        // This is just an example, comment out so won't have failure in the lab run.
        ////[TestMethod, Ignore, Variation(Id = 2)]
        public void DummyTest_Fail()
        {
            Assert.Fail("fail!");
        }
    }
}
