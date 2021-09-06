//---------------------------------------------------------------------
// <copyright file="AssemblyPathResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Security.Permissions;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Default resolver
    /// </summary>
    [ImplementationName(typeof(IAssemblyPathResolver), "Default")]
    public class AssemblyPathResolver : IAssemblyPathResolver
    {
        /// <summary>
        /// Initializes a new instance of the AssemblyPathResolver class.
        /// </summary>
        public AssemblyPathResolver()
        {
            this.TempAssemblyDirectory = IOHelpers.CreateTempDirectory("CompiledAsemblies");
        }

        /// <summary>
        /// Gets or sets the directory where temporary assembly files should be stored.
        ///  Note this value is not set to its default
        /// </summary>
        [InjectTestParameter("TempAssemblyDirectory", DefaultValueDescription = @".\tmp\assemblies", HelpText = "Directory where temporary assembly files should be stored.")]
        public string TempAssemblyDirectory { get; set; }

        /// <summary>
        /// Resolves an assembly
        /// </summary>
        /// <param name="assemblyName">Assembly Name</param>
        /// <returns>Will return either the original result or a fully resolved assemblypath</returns>
        public string ResolveAssemblyLocation(string assemblyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(assemblyName, "assemblyName");

            if (Path.IsPathRooted(assemblyName))
            {
                return assemblyName;
            }

            List<string> foundFilePaths = FindFilePaths(this.GetDirectoryLookupLocations(), assemblyName);

            ExceptionUtilities.CheckCollectionDoesNotContainNulls<string>(foundFilePaths, "foundFilePaths");

            ExceptionUtilities.Assert(foundFilePaths.Count < 2, "Found '{0}' in the following places '{1}'", assemblyName, string.Join(",", foundFilePaths));
            string fullPath = foundFilePaths.SingleOrDefault();
            if (fullPath != null)
            {
                return fullPath;
            }
            else
            {
                return assemblyName;
            }
        }

        /// <summary>
        /// Directories to look in to find assemblies in
        /// </summary>
        /// <returns>Returns a list of directories to lookup</returns>
        [SecuritySafeCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [AssertJustification("Reading environment variable demands full trust.")]
        protected virtual IEnumerable<string> GetDirectoryLookupLocations()
        {
            return new string[] 
            {
                this.TempAssemblyDirectory,
                Environment.ExpandEnvironmentVariables(DataFxAssemblyRef.File.DS_ReferenceAssemblyPath)
            };
        }

        private static List<string> FindFilePaths(IEnumerable<string> directories, string assemblyName)
        {
            return directories.Select(d => Path.Combine(d, assemblyName)).Where(f => IOHelpers.FileExists(f)).ToList();
        }
    }
}
