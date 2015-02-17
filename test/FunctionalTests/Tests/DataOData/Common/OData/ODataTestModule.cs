//---------------------------------------------------------------------
// <copyright file="ODataTestModule.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Execution;

    /// <summary>
    /// TestModule for ODataLib Tests. 
    /// </summary>
    [TestModule]
    public class ODataTestModule :  TestModule
    {
        /// <summary>
        /// Helps the Taupo Runner pick test cases from this assembly
        /// </summary>
        /// <returns>All types that are Taupo testcases</returns>
        protected override IEnumerable<Type> DetermineTestCasesTypes()
        {
            return this.GetType().Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(TestCase)));
        }

        /// <summary>
        /// Specify assemblies to look for in finding implementations for dependency contracts
        /// </summary>
        /// <param name="defaultImplementationSelector"> implementationSelector on which we register implementations from assemblies</param>
        protected override void ConfigureImplementationSelector(Microsoft.Test.Taupo.Contracts.IImplementationSelector defaultImplementationSelector)
        {
            foreach (Assembly assembly in DependencyImplementationAssemblies.GetAssemblies())
            {
                defaultImplementationSelector.AddAssembly(assembly);
            }

            base.ConfigureImplementationSelector(defaultImplementationSelector);
        }
    }
}
