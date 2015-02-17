//---------------------------------------------------------------------
// <copyright file="AssemblyPathResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if !WINDOWS_PHONE        
namespace Microsoft.Test.Taupo.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security;
#if !WIN8
    using System.Security.Permissions;
#endif
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
#if !SILVERLIGHT && !WIN8
            this.TempAssemblyDirectory = IOHelpers.CreateTempDirectory("CompiledAsemblies");
#else
#if WIN8
            this.TempAssemblyDirectory = @"C:\Users\mfrintu\Documents\CompiledAssemblies";
#endif
#endif
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
#if !SILVERLIGHT
        [SecuritySafeCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [AssertJustification("Reading environment variable demands full trust.")]
#endif        
        protected virtual IEnumerable<string> GetDirectoryLookupLocations()
        {
            return new string[] 
            {
                this.TempAssemblyDirectory,
#if !SILVERLIGHT
                Environment.ExpandEnvironmentVariables(DataFxAssemblyRef.File.DS_ReferenceAssemblyPath)
#endif
            };
        }

        private static List<string> FindFilePaths(IEnumerable<string> directories, string assemblyName)
        {
#if !SILVERLIGHT
            return directories.Select(d => Path.Combine(d, assemblyName)).Where(f => IOHelpers.FileExists(f)).ToList();
#else
            return new List<string>();
#endif
        }
    }
}
#endif
