//---------------------------------------------------------------------
// <copyright file="ODataReaderTestsTestModule.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData;

    /// <summary>
    /// TestModule for OData Reader Tests. 
    /// </summary>
    [TestModule(Name = "OData Reader Tests")]
    public class ODataReaderTestsTestModule : ODataTestModule
    {
        /// <summary>
        /// The reader test configuration provider for this test module.
        /// </summary>
        /// <remarks>It's on the module level so that we get the same configuration for all test cases in this module.</remarks>
        [InjectDependency(IsRequired = true)]
        public ReaderTestConfigurationProvider ReaderTestConfigurationProvider { get; set; }

        /// <summary>
        /// Specify assemblies to look for in finding implementations for dependency contracts.
        /// </summary>
        /// <param name="defaultImplementationSelector">The default implementationSelector on which we register implementations from assemblies.</param>
        protected override void ConfigureImplementationSelector(Taupo.Contracts.IImplementationSelector defaultImplementationSelector)
        {
            base.ConfigureImplementationSelector(defaultImplementationSelector);
            defaultImplementationSelector.AddAssembly(typeof(ODataReaderTestsTestModule).Assembly);
        }
    }
}
