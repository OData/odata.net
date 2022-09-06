//---------------------------------------------------------------------
// <copyright file="LtmTestLoader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution.Ltm
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// LTM-based test loader.
    /// </summary>
    /// <remarks>
    /// In order to use the loader, create a derived, nested class in your test module:
    /// <code>
    ///[TestModule]
    ///public class SampleTestModule : TestModule
    ///{
    ///    public class Loader : LtmTestLoader { }
    ///}
    ///</code>
    ///</remarks>
    [ComVisible(true)]
    public abstract class LtmTestLoader : LtmTestLoaderBase
    {
        /// <summary>
        /// Initializes a new instance of the LtmTestLoader class
        /// </summary>
        protected LtmTestLoader()
        {
        }

        /// <summary>
        /// Locates all Test Modules
        /// </summary>
        /// <param name="assembly">Assembly to locate Test Modules in</param>
        /// <returns>The names of all Test Modules</returns>
        protected override string[] GetBaseTestItemNames(Assembly assembly)
        {
            var result = assembly.GetTypes()
                .Where(c => c.IsSubclassOf(typeof(TestModule)))
                .Where(c => c.IsDefined(typeof(TestModuleAttribute), false))
                .Select(c => c.FullName)
                .ToArray();

            return result;
        }

        /// <summary>
        /// Builds a Test Module
        /// </summary>
        /// <param name="assembly">Assembly to locate Test Module in</param>
        /// <param name="testItemName">Name of the Test Module</param>
        /// <returns>The newly constructed Test Module</returns>
        [SecurityCritical]
        protected override LtmTestItem BuildBaseTestItem(Assembly assembly, string testItemName)
        {
            Type type = assembly.GetType(testItemName, true);
            TestModule module = (TestModule)Activator.CreateInstance(type);

            module.Log = LtmLogger.Instance;
            module.OnInitialize += delegate
            {
                foreach (var kvp in new LtmTestProperties().GetParameterValues())
                {
                    module.TestParameters.Add(kvp.Key, kvp.Value);
                }
            };

            return new LtmTestModuleItem(module);
        }

        [SecurityCritical]
        internal static LtmTestItem Wrap(TestItem item)
        {
            VariationTestItem vti = item as VariationTestItem;
            if (vti != null)
                return new LtmVariationTestItem(vti);

            TestCase tc = item as TestCase;
            if (tc != null)
                return new LtmTestCaseItem(tc);

            TestModule tm = item as TestModule;
            if (tm != null)
                return new LtmTestModuleItem(tm);

            throw new TaupoNotSupportedException("Test item is not supported by LTM: " + item);
        }
    }
}
