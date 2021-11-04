//---------------------------------------------------------------------
// <copyright file="FileLocator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security;
    using System.Security.Permissions;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Helper to locate a file. Also exposes some frequently used File Folder shortcuts.
    /// </summary>
    public class FileLocator
    {
        private Logger logger;

        /// <summary>
        /// Initializes a new instance of the FileLocator class.
        /// </summary>
        /// <param name="logger">the Logger to use</param>
        public FileLocator(Logger logger)
        {
            this.logger = logger;
            this.UseRecursiveSearch = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use recursive search (search all subdirectories)
        /// </summary>
        public bool UseRecursiveSearch { get; set; }

        /// <summary>
        /// Search a file, given several possible directories it could reside in
        /// </summary>
        /// <param name="fileName">name of the file.</param>
        /// <param name="possibleDirectories">several possible directories it could reside in.</param>
        /// <returns>full path fo the file. Will throw if it cannot find the file under all given directories.</returns>
        public string LocateFile(string fileName, IEnumerable<string> possibleDirectories)
        {
            string fullPath;
            if (this.TryLocateFile(fileName, possibleDirectories, out fullPath))
            {
                return fullPath;
            }
            else
            {
                throw new TaupoInvalidOperationException("Cannot find " + fileName + " in all given possible directories");
            }
        }

        /// <summary>
        /// Try to search a file, given several possible directories it could reside in
        /// </summary>
        /// <param name="fileName">name of the file.</param>
        /// <param name="possibleDirectories">several possible directories it could reside in.</param>
        /// <param name="fullPath">output the fullPath if search is successful</param>
        /// <returns>true if found the file</returns>
        [SecuritySafeCritical]
        public bool TryLocateFile(string fileName, IEnumerable<string> possibleDirectories, out string fullPath)
        {
            fullPath = null;
            foreach (string dir in possibleDirectories)
            {
                fullPath = this.TryLocateFileUnder(fileName, dir);
                if (fullPath != null)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Try to search a file in the given directory and subdirectories.
        /// </summary>
        /// <param name="fileName">file name to search</param>
        /// <param name="directory">directory to search</param>
        /// <returns>full path of the file, null if cannot locate it</returns>
        [SecuritySafeCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [AssertJustification("Calling Directory.Exists, File.Exists, and Directory.EnumerateDirectories demands FileIOPermission (Read | PathDiscovery) to the paths used as parameters to those methods. Calling Environment.ExpandEnvironmentVariables also demands EnvironmentPermission (Read) for the variables being read.")]
        private string TryLocateFileUnder(string fileName, string directory)
        {
            directory = Environment.ExpandEnvironmentVariables(directory);

            if (Directory.Exists(directory))
            {
                string fullPath = Path.Combine(directory, fileName);
                if (File.Exists(fullPath))
                {
                    this.logger.WriteLine(LogLevel.Verbose, fileName + " found at " + fullPath);
                    return fullPath;
                }
                else
                {
                    this.logger.WriteLine(LogLevel.Verbose, fileName + " didn't find at " + fullPath);
                }

                if (this.UseRecursiveSearch)
                {
                    foreach (string subDirectory in Directory.EnumerateDirectories(directory))
                    {
                        fullPath = this.TryLocateFileUnder(fileName, subDirectory);
                        if (fullPath != null)
                        {
                            return fullPath;
                        }
                    }
                }
            }

            return null;
        }
    }
}
