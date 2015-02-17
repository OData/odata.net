//---------------------------------------------------------------------
// <copyright file="AssemblyResourceLookup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

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
            : this(assembly, ResourceUtilities.BuildResourceManager(assembly))
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
        /// Finds a specific string resource
        /// </summary>
        /// <param name="resourceKey">Key of the resource to be located</param>
        /// <returns>The localized resource value</returns>
        public virtual string LookupString(string resourceKey)
        {
            string messageFromResources = this.resourceManager.GetString(resourceKey);
            if (messageFromResources == null)
            {
                throw new TaupoArgumentException(string.Format(
                    CultureInfo.InvariantCulture,
                    "No string with key {0} was found in resource table in assembly {1}.",
                    resourceKey,
                    this.assembly.FullName));
            }

            return messageFromResources;
        }
    }
}
