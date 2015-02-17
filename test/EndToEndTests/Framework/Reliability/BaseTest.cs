//---------------------------------------------------------------------
// <copyright file="BaseTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Reliability
{
    using Microsoft.Test.DataDriven;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Base test class
    /// </summary>
    [TestClass]
    public class TestBase : DataDrivenTest
    {
        /// <summary>
        /// 'TestContext' is declared as a public property 
        /// so that it can be accessed by the test harness; 
        /// otherwise MSTest will not be able to manage the object.
        /// MSTest also requires this property be non-static.
        /// </summary>
        private TestContext testContext;

        /// <summary>
        /// Gets or sets test context
        /// </summary>
        public TestContext TestContext
        {
            get
            {
                return this.testContext;
            }

            set
            {
                this.testContext = value;
                TestConfig.LoadContextParameter(this.testContext.Properties);
                MsTestLogger.Attach(this.testContext);
            }
        }

        /// <summary>
        /// Test initialization
        /// </summary>
        [TestInitialize]
        public void BaseTestInit()
        {
            Log.StartTestCase(TestContext.TestName);
        }

        /// <summary>
        /// Test Cleanup
        /// </summary>
        [TestCleanup]
        public void BaseTestCleanUp()
        {
            Log.StopTestCase(TestContext.TestName);
        }
    }
}
