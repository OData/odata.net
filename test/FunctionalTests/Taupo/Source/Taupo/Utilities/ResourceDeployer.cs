//---------------------------------------------------------------------
// <copyright file="ResourceDeployer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Deploys embedded resources from an assembly to the file system
    /// </summary>
    public class ResourceDeployer
    {
        private Dictionary<string, string> knownSubDirectories = new Dictionary<string, string>();

        /// <summary>
        /// Registers a known sub directory to be used during extraction
        /// This is required because sub-directories are included in the resource name
        /// (i.e. "ProjectFolder\SubDir\File.cs" becomes "AssemblyNamespace.SubDir.File.cs")
        /// </summary>
        /// <param name="prefix">The prefix that ide</param>
        /// <param name="directory">Directory to write files to</param>
        /// <example>
        /// <para>
        /// This is required because directory structure is encoded into the resource name
        /// e.g. "ProjectFolder\Sub Dir\File.cs" becomes "AssemblyNamespace.Sub_Dir.File.cs"
        /// </para>
        /// <para>
        /// In the above case you would use;
        /// var deployer = new ResourceDeployer();
        /// deployer.ResourcePrefix = "AssemblyNamespace.";
        /// deployer.AddKnownSubdirectory("Sub Dir.", "Sub Dir");
        /// </para>
        /// </example>
        public void RegisterKnownSubdirectory(string prefix, string directory)
        {
            this.knownSubDirectories[prefix] = directory;
        }

        /// <summary>
        /// Extracts embedded resources to the file system
        /// </summary>
        /// <param name="assembly">Assembly to locate resources in</param>
        /// <param name="prefix">Prefix of resources to extract</param>
        /// <param name="outputDirectory">Directory resources should be written to</param>
        [SecuritySafeCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [AssertJustification("Calling Directory.CreateDirectory demands FileIOPermission (Read | Write) for all subdirectories created in the specified outputDirectory.")]
        public void Extract(Assembly assembly, string prefix, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            foreach (var subDir in this.knownSubDirectories.Select(s => s.Value))
            {
                Directory.CreateDirectory(Path.Combine(outputDirectory, subDir));
            }

            foreach (string resourceName in assembly.GetManifestResourceNames().Where(r => r.StartsWith(prefix, StringComparison.Ordinal)))
            {
                string filename = resourceName.Substring(prefix.Length);

                string subDirKey = this.knownSubDirectories.Keys
                    .Where(k => filename.StartsWith(k, StringComparison.Ordinal))
                    .OrderByDescending(p => p.Length) // This ensures we get the most qualified prefix
                    .FirstOrDefault();

                if (subDirKey != null)
                {
                    filename = Path.Combine(this.knownSubDirectories[subDirKey], filename.Substring(subDirKey.Length));
                }

                filename = Path.Combine(outputDirectory, filename);
                IOHelpers.WriteResourceToFile(resourceName, filename, assembly);
            }
        }
    }
}
