//---------------------------------------------------------------------
// <copyright file="ODataUnitTestModule.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData;

    /// <summary>
    /// TestModule for ODataLib Unit Tests.
    /// </summary>
    [TestModule]
    public class ODataUnitTestModule : ODataTestModule
    {
        /// <summary>
        /// Specify assemblies to look for in finding implementations for dependency contracts
        /// </summary>
        /// <param name="defaultImplementationSelector"> implementationSelector on which we register implementations from assemblies</param>
        protected override void ConfigureImplementationSelector(Microsoft.Test.Taupo.Contracts.IImplementationSelector defaultImplementationSelector)
        {
            base.ConfigureImplementationSelector(defaultImplementationSelector);
            defaultImplementationSelector.AddAssembly(typeof(ODataUnitTestModule).Assembly);
        }
    }
}
