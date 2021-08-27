//---------------------------------------------------------------------
// <copyright file="AssemblyResourceLookup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Common
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Resources;

    /// <summary>
    /// Locates localized resources for an assembly
    /// </summary>
    public class AssemblyResourceLookup : IResourceLookup
    {
        private readonly Assembly assembly;
        private readonly ResourceManager resourceManager;

        /// <summary>
        /// Initializes a new instance of the AssemblyResourceLookup class.
        /// </summary>
        /// <param name="assembly">Assembly that resources belong to</param>
        public AssemblyResourceLookup(Assembly assembly)
            : this(assembly, BuildResourceManager(assembly))
        {
        }

        /// <summary>
        /// Initializes a new instance of the AssemblyResourceLookup class.
        /// </summary>
        /// <param name="assembly">Assembly that resources belong to</param>
        /// <param name="resourceTable">Resource table to lookup strings in</param>
        public AssemblyResourceLookup(Assembly assembly, string resourceTable)
            : this(assembly, new ResourceManager(resourceTable, assembly))
        {
        }

        private AssemblyResourceLookup(Assembly assembly, ResourceManager manager)
        {
            ExceptionUtilities.CheckArgumentNotNull(assembly, "assembly");
            this.assembly = assembly;
            this.resourceManager = manager;
        }

        /// <summary>
        /// Builds a resource manager for the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly to build the resource manager for.</param>
        /// <returns>The resource manager.</returns>
        public static ResourceManager BuildResourceManager(Assembly assembly)
        {
            ExceptionUtilities.CheckArgumentNotNull(assembly, "assembly");
            return new ResourceManager(FindSingleResourceTable(assembly), assembly);
        }

        /// <summary>
        /// Finds a specific string resource
        /// </summary>
        /// <param name="resourceKey">Key of the resource to be located</param>
        /// <returns>The localized resource value</returns>
        public virtual string LookupString(string resourceKey)
        {
            string messageFromResources = this.resourceManager.GetString(resourceKey);
            if (messageFromResources == null)
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.InvariantCulture,
                    "No string with key {0} was found in resource table in assembly {1}.",
                    resourceKey,
                    this.assembly.FullName));
            }

            return messageFromResources;
        }

        private static string FindSingleResourceTable(Assembly assembly)
        {
            var resources = assembly.GetManifestResourceNames().Where(r => r.EndsWith(".resources", StringComparison.Ordinal));
            if (resources.Count() != 1)
            {
                throw new NotSupportedException("The supplied assembly does not contain exactly one resource table, if the assembly contains multiple tables call the overload that specifies which table to use.");
            }

            var resource = resources.Single();

            // Need to trim the ".resources" off the end
            return resource.Substring(0, resource.Length - 10);
        }
    }
}
