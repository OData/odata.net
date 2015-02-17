//---------------------------------------------------------------------
// <copyright file="ODataWriterTestsTestModule.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;

    /// <summary>
    /// TestModule for OData Writer Tests. 
    /// </summary>
    [TestModule(Name = "OData Writer Tests")]
    public class ODataWriterTestsTestModule : ODataTestModule
    {
        /// <summary>
        /// The writer test configuration provider for this test module.
        /// </summary>
        /// <remarks>It's on the module level so that we get the same configuration for all test cases in this module.</remarks>
        [InjectDependency(IsRequired = true)]
        public WriterTestConfigurationProvider WriterTestConfigurationProvider { get; set; }
    }
}
