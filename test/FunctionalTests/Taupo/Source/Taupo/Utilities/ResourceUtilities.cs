//---------------------------------------------------------------------
// <copyright file="ResourceUtilities.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using System.Security;
    using System.Security.Permissions;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Various utilities for dealing with embedded resources
    /// </summary>
    public static class ResourceUtilities
    {
        /// <summary>
        /// Extracts the specified resource to a given directory
        /// </summary>
        /// <param name="outputDirectory">Output directory</param>
        /// <param name="assembly">Assembly that contains the resource</param>
        /// <param name="resourceName">Partial resource name</param>
        /// <remarks>The method prefixes the actual resource with unqualified assembly name + '.Resources.' (
        /// so you should put all your resources under 'Resources' directory within a project)</remarks>
        public static void ExtractResourceToDirectory(string outputDirectory, Assembly assembly, string resourceName)
        {
            ExtractResourceToDirectory(outputDirectory, assembly, resourceName, null);
        }

        /// <summary>
        /// Extracts the specified resource to a given directory
        /// </summary>
        /// <param name="outputDirectory">Output directory</param>
        /// <param name="assembly">Assembly that contains the resource</param>
        /// <param name="resourceName">Partial resource name</param>
        /// <param name="fileName">The name of the file.</param>
        /// <remarks>The method prefixes the actual resource with unqualified assembly name + '.Resources.' (
        /// so you should put all your resources under 'Resources' directory within a project)</remarks>
        [SecuritySafeCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [AssertJustification("Calling File.Create demands FileIOPermission (Write flag) for the file path to which the resource is extracted.")]
        public static void ExtractResourceToDirectory(string outputDirectory, Assembly assembly, string resourceName, string fileName)
        {
            ValidateCommonArguments(outputDirectory, assembly, resourceName);

            fileName = fileName ?? resourceName;
            string fullResourceName = new AssemblyName(assembly.FullName).Name + ".Resources." + resourceName;
            using (Stream inputStream = assembly.GetManifestResourceStream(fullResourceName))
            {
                if (inputStream == null)
                {
                    throw new TaupoInfrastructureException("Embedded resource " + fullResourceName + " not found in " + assembly.FullName);
                }

                using (Stream outputStream = File.Create(Path.Combine(outputDirectory, fileName)))
                {
                    IOHelpers.CopyStream(inputStream, outputStream);
                }
            }
        }

        /// <summary>
        /// Extracts the specified compressed resource to a given directory
        /// </summary>
        /// <param name="outputDirectory">Output directory</param>
        /// <param name="assembly">Assembly that contains the resource</param>
        /// <param name="resourceName">Partial resource name</param>
        /// <remarks>The method prefixes the actual resource with unqualified assembly name + '.Resources.' (
        /// so you should put all your resources under 'Resources' directory within a project)
        /// and appends '.gz' to it. You should use gzip to produce compressed files.</remarks>
        [SecuritySafeCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [AssertJustification("Calling File.Create demands FileIOPermission (Write flag) for the file path to which the resource is extracted.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "gz", Justification = ".gz is GZIP file extension")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Bogus warning, object will be disposed once.")]
        public static void ExtractCompressedResourceToDirectory(string outputDirectory, Assembly assembly, string resourceName)
        {
            ValidateCommonArguments(outputDirectory, assembly, resourceName);

            string fullResourceName = new AssemblyName(assembly.FullName).Name + ".Resources." + resourceName + ".gz";
            using (Stream compressedInputStream = assembly.GetManifestResourceStream(fullResourceName))
            {
                if (compressedInputStream == null)
                {
                    throw new TaupoInfrastructureException("Embedded resource " + fullResourceName + " not found in " + assembly.FullName);
                }

                using (GZipStream decompressedInputStream = new GZipStream(compressedInputStream, CompressionMode.Decompress))
                {
                    using (Stream outputStream = File.Create(Path.Combine(outputDirectory, resourceName)))
                    {
                        IOHelpers.CopyStream(decompressedInputStream, outputStream);
                    }
                }
            }
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

        private static void ValidateCommonArguments(string outputDirectory, Assembly assembly, string resourceName)
        {
            ExceptionUtilities.CheckArgumentNotNull(outputDirectory, "outputDirectory");
            ExceptionUtilities.CheckArgumentNotNull(assembly, "assembly");
            ExceptionUtilities.CheckArgumentNotNull(resourceName, "resourceName");

            if (!Directory.Exists(outputDirectory))
            {
                throw new TaupoInfrastructureException("Output directory '" + outputDirectory + "' does not exist.");
            }
        }

        private static string FindSingleResourceTable(Assembly assembly)
        {
            var resources = assembly.GetManifestResourceNames().Where(r => r.EndsWith(".resources", StringComparison.Ordinal));
            if (resources.Count() != 1)
            {
                throw new TaupoNotSupportedException("The supplied assembly does not contain exactly one resource table, if the assembly contains multiple tables call the overload that specifies which table to use.");
            }

            var resource = resources.Single();

            // Need to trim the ".resources" off the end
            return resource.Substring(0, resource.Length - 10);
        }
    }
}
