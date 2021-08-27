//---------------------------------------------------------------------
// <copyright file="LtmTestLoaderBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution.Ltm
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security;

    /// <summary>
    /// Base functionality for all LTM-based test loaders.
    /// </summary>
    [ComVisible(true)]
    public abstract class LtmTestLoaderBase : ITestLoader
    {
        /// <summary>
        /// Initializes a new instance of the LtmTestLoaderBase class
        /// </summary>
        protected LtmTestLoaderBase()
        {
        }

        /// <summary>
        /// Gets the unique identifier for this loader
        /// </summary>
        string ITestLoader.Guid
        {
            get { return this.GetType().FullName; }
        }

        /// <summary>
        /// Gets the name of this loader
        /// </summary>
        string ITestLoader.Name
        {
            get { return this.GetType().FullName; }
        }

        /// <summary>
        /// Gets a description of this loader
        /// </summary>
        string ITestLoader.Desc
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Gets the metadata for this loader
        /// </summary>
        ITestProperties ITestLoader.Metadata
        {
            [SecurityCritical]
            get { return null; }
        }

        /// <summary>
        /// Gets or sets the test properties for this loader
        /// </summary>
        ITestProperties ITestLoader.Properties
        {
            [SecurityCritical]
            get
            {
                throw new NotImplementedException();
            }

            [SecurityCritical]
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets or sets the Log for this loader
        /// </summary>
        ITestLog ITestLoader.Log
        {
            [SecurityCritical]
            get
            {
                throw new NotImplementedException();
            }

            [SecurityCritical]
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Initializes this loader
        /// </summary>
        void ITestLoader.Init()
        {
        }

        /// <summary>
        /// Locates the top level Test Items
        /// </summary>
        /// <param name="assemblyName">Assembly to locate Test Items in</param>
        /// <returns>The names of the Test Items</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2001", Justification = "We only have the filename of the assembly.")]
        string[] ITestLoader.Enumerate(string assemblyName)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyName);
            return this.GetBaseTestItemNames(assembly);
        }

        /// <summary>
        /// Constructs a top level Test Item
        /// </summary>
        /// <param name="assemblyFile">The assembly the Test Item is located in</param>
        /// <param name="test">The name of the Test Item</param>
        /// <returns>The newly constructed Test Item</returns>
        [SecurityCritical]
        [SuppressMessage("Microsoft.Reliability", "CA2001", Justification = "We only have the filename of the assembly.")]
        ITestItem ITestLoader.CreateTest(string assemblyFile, string test)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyFile);
            var module = this.BuildBaseTestItem(assembly, test);
            module.TestItem.SetTestItemFilter(this.GetTestItemFilter());
            return module;
        }

        /// <summary>
        /// Performs any cleanup needed for this loader
        /// </summary>
        void ITestLoader.Terminate()
        {
        }

        /// <summary>
        /// Locates the top level Test Items
        /// </summary>
        /// <param name="assembly">Assembly to locate Test Items in</param>
        /// <returns>The names of the Test Items</returns>
        protected abstract string[] GetBaseTestItemNames(Assembly assembly);

        /// <summary>
        /// Constructs a top level Test Item
        /// </summary>
        /// <param name="assembly">The assembly the Test Item is located in</param>
        /// <param name="testItemName">The name of the Test Item</param>
        /// <returns>The newly constructed Test Item</returns>
        [SecurityCritical]
        protected abstract LtmTestItem BuildBaseTestItem(Assembly assembly, string testItemName);

        [SecurityCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "MaxPriority", Justification = "MaxPriority is the name of the parameter.")]
        private Func<TestItem, bool> GetTestItemFilter()
        {
            ITestProperties props = (ITestProperties)(new LtmContext());
            int maxPriority = int.MaxValue;

            var maxPriorityArgument = props.Get("CommandLine/MaxPriority");
            if (maxPriorityArgument != null)
            {
                if (!int.TryParse((string)maxPriorityArgument.Value, out maxPriority))
                {
                    maxPriority = int.MaxValue;
                    LtmLogger.Instance.WriteLine(Common.LogLevel.Warning, "Unable to parse MaxPriority command line argument '{0}'. Must be an integer.", maxPriorityArgument.Value);
                }
            }

            return item =>
            {
                return item.Metadata.Priority <= maxPriority;
            };
        }
    }
}
