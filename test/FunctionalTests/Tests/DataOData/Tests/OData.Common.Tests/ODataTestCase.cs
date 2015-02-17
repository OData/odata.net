//---------------------------------------------------------------------
// <copyright file="ODataTestCase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Tests
{
    using Microsoft.Test.Taupo.OData;

    /// <summary>
    /// Base class for ODataLib Tests with extra assembly added for unit test
    /// </summary>
    public class ODataTestCase : ODataTestCaseBase
    {
        /// <summary>
        /// This injects the current assembly into the DependencyImplementationSelector
        /// </summary>
        protected override void ConfigureDependencyImplentationSelector(Test.Taupo.DependencyInjection.ImplementationSelector defaultImplementationSelector)
        {
            base.ConfigureDependencyImplentationSelector(defaultImplementationSelector);
            defaultImplementationSelector.AddAssembly(typeof(ODataUnitTestModule).Assembly);
        }
    }
}
