//---------------------------------------------------------------------
// <copyright file="IOHelpers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// This class provides utility methods for common I/O tasks not directly supported by .NET Framework BCL,
    /// as well as I/O tasks that require elevated privileges.
    /// </summary>
    public static class IOHelpers
    {
        private const int DefaultCopyBufferSize = 65536;
        private static string baseDir;
        private static string baseTaupoDir;
        private static string baseTempDir;

        /// <summary>
        /// Gets the base directory where test assemblies are located.
        /// </summary>
        public static string BaseDirectory
        {
            [SecuritySafeCritical]
            [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
            [AssertJustification("Calling Environment.CurrentDirectory demands FileIOPermission (PathDiscovery flag) for the current directory path.")]
            get
            {
                if (baseDir == null)
                {
                    string tmp = AppDomain.CurrentDomain.BaseDirectory;

                    // when running in MSTest/trun, AppDomain.CurrentDomain.BaseDirectory is not set correctly
                    // so we fix it
                    if (Process.GetCurrentProcess().MainModule.FileName.EndsWith("testtools\\vstesthost.exe", StringComparison.OrdinalIgnoreCase))
                    {
                        tmp = Environment.CurrentDirectory;
                    }

                    baseDir = tmp;
                }

                return baseDir;
            }
        }

        /// <summary>
        /// Gets the base temp directory - temp and service root directories should be created below it.
        /// </summary>
        public static string BaseTaupoDirectory
        {
            [SecuritySafeCritical]
            [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
            [AssertJustification("Calling Environment.ExpandEnvironmentVariables demands EnvironmentPermission (Read flag) for all environment variables that this method attempts to read.")]
            get
            {
                if (null == baseTaupoDir)
                {
                    // originally "%SystemDrive%\Taupo" was choosen because of the short base path length
                    // however, all temporary files must be under the binaries directory.
                    string tempDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    baseTaupoDir = Path.GetFullPath(Path.Combine(tempDir, "Taupo"));
                }

                return baseTaupoDir;
            }
        }

        /// <summary>
        /// Gets the base temporary directory - working directories should be created below it.
        /// </summary>
        public static string BaseTempDirectory
        {
            get
            {
                if (null == baseTempDir)
                {
                    string tempDir = Path.Combine(BaseTaupoDirectory, "Temp");
                    EnsureDirectoryExists(tempDir);
                    baseTempDir = tempDir;
                }

                return baseTempDir;
            }
        }

        /// <summary>
        /// Safely determines whether the given path refers to an existing directory on disk.
        /// </summary>
        /// <param name="path">The path to test.</param>
        /// <returns>
        /// True if path refers to an existing directory; otherwise, false.
        /// </returns>
        [SecuritySafeCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [AssertJustification("Calling Directory.Exists demands FileIOPermission (Read flag) for the specified path.")]
        public static bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// Creates the specified directory if it doesn't exist or removes all contents of an existing directory.
        /// </summary>
        /// <param name="path">Path to directory to create.</param>
        [SecuritySafeCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [AssertJustification("Calling Directory.Exists demands FileIOPermission (Read flag) for the specified path.")]
        public static void EnsureDirectoryEmpty(string path)
        {
            if (Directory.Exists(path))
            {
                SafeDeleteDirectory(path);
            }

            EnsureDirectoryExists(path);
        }

        /// <summary>Creates the specified directory if it doesn't exist.</summary>
        /// <param name="path">Path to directory to create.</param>
        [SecuritySafeCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [AssertJustification("Calling Directory.Exists and Directory.CreateDirectory demands FileIOPermission (Read | Write) for the specified path.")]
        public static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Safely determines whether the specified file exists.
        /// </summary>
        /// <param name="path">The file to check.</param>
        /// <returns>
        /// True if the caller has the required permissions and path contains the name
        /// of an existing file; otherwise, false. This method also returns false if
        /// path is null, an invalid path, or a zero-length string. If the caller does
        /// not have sufficient permissions to read the specified file, no exception
        /// is thrown and the method returns false regardless of the existence of path.
        /// </returns>
        [SecuritySafeCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [AssertJustification("Calling File.Exists demands FileIOPermission (Read flag) for the specified path.")]
        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Safely returns the absolute path for the specified path string.
        /// </summary>
        /// <param name="path">The file or directory for which to obtain absolute path information.</param>
        /// <returns>
        /// A string containing the fully qualified location of path, such as "C:\MyFile.txt".
        /// </returns>
        [SecuritySafeCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [AssertJustification("Calling Path.GetFullPath demands FileIOPermission (PathDiscovery flag) for the specified path.")]
        public static string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }

        /// <summary>
        /// Safely deletes the file and ignores any access violation exceptions.
        /// </summary>
        /// <param name="path">The directory to delete.</param>
        [SecuritySafeCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [AssertJustification("Calling File.Delete demands FileIOPermission (Write flag) for the specified path.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch everything here.")]
        public static void SafeDeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception)
            {
                // ignore exceptions
            }
        }

        /// <summary>
        /// Safely deletes the directory and ignores any access violation exceptions.
        /// </summary>
        /// <param name="path">The directory to delete.</param>
        [SecuritySafeCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [AssertJustification("Calling Directory.Delete demands FileIOPermission (Write flag) for the specified path.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch everything here.")]
        public static void SafeDeleteDirectory(string path)
        {
            // try different ways of contents of directory, fail after 3 attempts
            for (int i = 0; i < 3; ++i)
            {
                try
                {
                    Directory.Delete(path, true);

                    return;
                }
                catch (Exception)
                {
                }

                Thread.Sleep(500);

                try
                {
                    foreach (string file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
                    {
                        SafeDeleteFile(file);
                    }

                    return;
                }
                catch (Exception)
                {
                }

                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Creates a temporary directory and returns its fully qualified path.
        /// </summary>
        /// <param name="relativeName">Name of the relative.</param>
        /// <returns>Path to the temporary directory</returns>
        /// <remarks>Directory will be created in a path close to application binaries, not in %TEMP% path</remarks>
        public static string CreateTempDirectory(string relativeName)
        {
            string fullPath = Path.Combine(BaseTempDirectory, relativeName);
            EnsureDirectoryExists(fullPath);
            return fullPath;
        }

        /// <summary>
        /// Copies the specified source files to a given directory.
        /// </summary>
        /// <param name="destinationDirectory">The destination directory.</param>
        /// <param name="sourceFiles">The source files.</param>
        [SecuritySafeCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [AssertJustification("Calling File.Copy demands FileIOPermission (Write flag) for the destination file path.")]
        public static void CopyToDirectory(string destinationDirectory, params string[] sourceFiles)
        {
            foreach (string sourceFile in sourceFiles)
            {
                string baseName = Path.GetFileName(sourceFile);
                string destinationFile = Path.Combine(destinationDirectory, baseName);

                if (new FileInfo(sourceFile).FullName != new FileInfo(destinationFile).FullName)
                {
                    File.Copy(sourceFile, destinationFile, true);
                }
            }
        }

        /// <summary>
        /// Copies contents of one stream into another.
        /// </summary>
        /// <param name="source">Stream to read from.</param>
        /// <param name="destination">Stream to write to.</param>
        /// <returns>
        /// The number of bytes copied from the source.
        /// </returns>
        public static int CopyStream(Stream source, Stream destination)
        {
            return CopyStream(source, destination, new byte[DefaultCopyBufferSize]);
        }

        /// <summary>
        /// Copies contents of one stream into another.
        /// </summary>
        /// <param name="source">Stream to read from.</param>
        /// <param name="destination">Stream to write to.</param>
        /// <param name="copyBuffer">The copy buffer.</param>
        /// <returns>
        /// The number of bytes copied from the source.
        /// </returns>
        public static int CopyStream(Stream source, Stream destination, byte[] copyBuffer)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            ExceptionUtilities.CheckArgumentNotNull(destination, "destination");
            ExceptionUtilities.CheckArgumentNotNull(copyBuffer, "copyBuffer");

            int bytesCopied = 0;
            int bytesRead;

            do
            {
                bytesRead = source.Read(copyBuffer, 0, copyBuffer.Length);
                destination.Write(copyBuffer, 0, bytesRead);
                bytesCopied += bytesRead;
            }
            while (bytesRead != 0);

            return bytesCopied;
        }

        /// <summary>
        /// Write an embedded resource to a local file
        /// </summary>
        /// <param name="resourceName">Resource to be written</param>
        /// <param name="fileName">File to write resource to</param>
        /// <param name="assembly">Assembly to extract resource from</param>
        [SecuritySafeCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [AssertJustification("Calling File.Open demands FileIOPermission (Append flag) for the destination file path.")]
        public static void WriteResourceToFile(string resourceName, string fileName, Assembly assembly)
        {
            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    throw new TaupoInvalidOperationException("Resource '" + resourceName + "' not found in '" + assembly.FullName + "'");
                }

                using (FileStream fileStream = File.Open(fileName, FileMode.Append))
                {
                    CopyStream(resourceStream, fileStream);
                }
            }
        }

        /// <summary>
        /// Adds the given set of file attributes to the file at the given path
        /// </summary>
        /// <param name="fileName">The name/path of the file</param>
        /// <param name="toAdd">The bit-field of attributes to add</param>
        public static void AddFileAttributes(string fileName, FileAttributes toAdd)
        {
            File.SetAttributes(fileName, File.GetAttributes(fileName) | toAdd);
        }

        /// <summary>
        /// Removes the given set of file attributes from the file at the given path
        /// </summary>
        /// <param name="fileName">The name/path of the file</param>
        /// <param name="toRemove">The bit-field of attributes to remove</param>
        public static void RemoveFileAttributes(string fileName, FileAttributes toRemove)
        {
            File.SetAttributes(fileName, File.GetAttributes(fileName) & ~toRemove);
        }
    }
}
