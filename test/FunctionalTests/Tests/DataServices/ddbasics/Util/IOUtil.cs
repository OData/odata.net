//---------------------------------------------------------------------
// <copyright file="IOUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    #region Namespaces

    using System.IO;
    using System.Reflection;
    using System.Diagnostics;
    using System.IO.Compression;
    #endregion Namespaces

    /// <summary>This class provides utility methods for I/O tasks.</summary>
    public static class IOUtil
    {
        #region Public methods.

        /// <summary>Copies a file if the source file is newer than the target file.</summary>
        /// <param name="sourceFilePath">Path to source file.</param>
        /// <param name="targetFilePath">Path to target file.</param>
        public static void CopyFileIfNewer(string sourceFilePath, string targetFilePath)
        {
            CheckArgumentNotNull(sourceFilePath, "sourceFilePath");
            CheckArgumentNotNull(targetFilePath, "targetFilePath");

            if (!File.Exists(sourceFilePath))
            {
                throw new InvalidOperationException("Unable to setup service files - file not found at " + sourceFilePath);
            }

            if (!File.Exists(targetFilePath) ||
                File.GetLastWriteTime(sourceFilePath) > File.GetLastWriteTime(targetFilePath))
            {
                File.Copy(sourceFilePath, targetFilePath, true /* overwrite */);
            }
        }

        /// <summary>
        /// Copies all contents from the <paramref name="source"/> folder to the
        /// <paramref name="destination"/> folder, with the option to recurse down
        /// subfolders.
        /// </summary>
        /// <param name="source">Path to source folder.</param>
        /// <param name="destination">Path to destination folder.</param>
        /// <param name="recursive">Whether to copy folders recursively.</param>
        public static void CopyFolder(string source, string destination, bool recursive)
        {
            CopyFolder(source, destination, recursive, false);
        }


        public static void CopyFolder(string source, string destination, bool recursive, bool overWrite)
        {
            CheckArgumentNotNull(source, "source");
            CheckArgumentNotNull(destination, "destination");

            if (!Directory.Exists(source))
            {
                throw new ArgumentException("Source directory '" + source + "' doesn't exist.", "source");
            }

            if (!Directory.Exists(destination))
            {
                Trace.WriteLine("Creating directory '" + destination + "'...");
                Directory.CreateDirectory(destination);
            }

            foreach (string file in Directory.GetFiles(source))
            {
                File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), overWrite);
            }

            if (recursive)
            {
                foreach (string subdir in Directory.GetDirectories(source))
                {
                    CopyFolder(subdir, Path.Combine(destination, Path.GetFileName(subdir)), recursive);
                }
            }
        }


        public static void RenameFile(string directoryPath, string currentFilename, string newFilename)
        {
            CheckArgumentNotNull(directoryPath, "Directory Path");
            CheckArgumentNotNull(currentFilename, "Current Filename");
            CheckArgumentNotNull(newFilename, "New Filename");

            if (!Directory.Exists(directoryPath))
            {
                throw new ArgumentException("Source directory '" + directoryPath + "' doesn't exist.", "Directory Path");
            }

            string current = directoryPath + "\\" + currentFilename;
            string newName = directoryPath + "\\" + newFilename;

            EnsureFileDeleted(newName);
            File.Move(current, newName);
            EnsureFileDeleted(current);
        }


        /// <summary>Copies a resource embedded in the given assembly into a file.</summary>
        /// <param name="assembly">Assembyl to copy resource from.</param>
        /// <param name="resourceName">Resource name.</param>
        /// <param name="destinationPath">Path to file.</param>
        public static void CopyResourceToFile(Assembly assembly, string resourceName, string destinationPath)
        {
            CheckArgumentNotNull(assembly, "assembly");
            CheckArgumentNotNull(resourceName, "resourceName");
            CheckArgumentNotNull(destinationPath, "destinationPath");

            using (Stream sourceStream = assembly.GetManifestResourceStream(resourceName))
            using (Stream destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
            {
                if (sourceStream == null)
                {
                    throw new InvalidOperationException("Unable to extract resource '" + resourceName + "' from " +
                        "assembly '" + assembly.Location + "' with these resource:\r\n" + ListResourceNames(assembly));
                }

                CopyStream(sourceStream, destinationStream);
            }
        }

        /// <summary>Copies content from one stream into another.</summary>
        /// <param name="source">Stream to read from.</param>
        /// <param name="destination">Stream to write to.</param>
        /// <returns>The number of bytes copied from the source.</returns>
        public static int CopyStream(Stream source, Stream destination)
        {
            CheckArgumentNotNull(source, "source");
            CheckArgumentNotNull(destination, "destination");

            int bytesCopied = 0;
            int bytesRead;
            byte[] buffer = new byte[1024];
            do
            {
                bytesRead = source.Read(buffer, 0, buffer.Length);
                destination.Write(buffer, 0, bytesRead);
                bytesCopied += bytesRead;
            }
            while (bytesRead != 0);

            return bytesCopied;
        }

        /// <summary>Creates a stream that can read the specified text as UTF-8 bytes.</summary>
        /// <param name='text'>Text to read.</param>
        /// <returns>A new stream.</returns>
        public static Stream CreateStream(string text)
        {
            return CreateStream(text, System.Text.Encoding.UTF8);
        }

        /// <summary>Creates a stream that can read the specified text as encoded bytes.</summary>
        /// <param name='text'>Text to read.</param>
        /// <param name='encoding'>Encoding to convert text into bytes.</param>
        /// <returns>A new stream.</returns>
        public static Stream CreateStream(string text, System.Text.Encoding encoding)
        {
            CheckArgumentNotNull(text, "text");
            CheckArgumentNotNull(encoding, "encoding");
            return new MemoryStream(encoding.GetBytes(text));
        }

        /// <summary>Compresses <paramref name="sourcePath"/> into <paramref name="targetPath"/> using Deflate.</summary>
        /// <param name="sourcePath">Path to file to compress.</param>
        /// <param name="targetPath">Path to file to create.</param>
        public static void DeflateCompressFile(string sourcePath, string targetPath)
        {
            CheckArgumentNotNull(sourcePath, "sourcePath");
            CheckArgumentNotNull(targetPath, "targetPath");
            using (Stream source = File.Open(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream target = File.Create(targetPath))
            using (Stream compressingStream = new DeflateStream(target, System.IO.Compression.CompressionMode.Compress))
            {
                CopyStream(source, compressingStream);
            }
        }

        /// <summary>Decompresses <paramref name="sourcePath"/> into <paramref name="targetPath"/> using Deflate.</summary>
        /// <param name="sourcePath">Path to file to decompress.</param>
        /// <param name="targetPath">Path to file to create.</param>
        public static void DeflateDecompressFile(string sourcePath, string targetPath)
        {
            CheckArgumentNotNull(sourcePath, "sourcePath");
            CheckArgumentNotNull(targetPath, "targetPath");
            using (Stream source = File.Open(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream target = File.Create(targetPath))
            using (Stream decompressingStream = new DeflateStream(source, System.IO.Compression.CompressionMode.Decompress))
            {
                CopyStream(decompressingStream, target);
            }
        }

        /// <summary>Empties the specified directory of all sub-directories and files.</summary>
        /// <param name="path">Path of directory to empty.</param>
        public static void EmptyDirectoryRecusively(string path)
        {
            CheckArgumentNotNull(path, "path");

            foreach (string dir in Directory.GetDirectories(path))
            {
                EmptyDirectoryRecusively(dir);
            }

            foreach (string file in Directory.GetFiles(path))
            {
                FileAttributes attributes = File.GetAttributes(file);
                if (0 != (attributes & FileAttributes.ReadOnly))
                {
                    attributes &= ~FileAttributes.ReadOnly;
                    File.SetAttributes(file, attributes);
                }

                File.Delete(file);
            }
        }

        /// <summary>Creates the specified directory if it doesn't exist.</summary>
        /// <param name="path">Path to directory to create.</param>
        public static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>Creates the specified directory if it doesn't exist, empties its contents otherwise.</summary>
        /// <param name="path">Path to directory to create or empty.</param>
        public static void EnsureEmptyDirectoryExists(string path)
        {
            if (Directory.Exists(path))
            {
                EmptyDirectoryRecusively(path);

                // On a VM occationally the deleted files are not immediately released. This causes some
                // tests to fail if they try to create the same files we just deleted.
                // Forcing a thread switch seem to fix the issue on the VMs we've tried...
                System.Threading.Thread.Sleep(1);
            }
            else
            {
                Directory.CreateDirectory(path);
            }
        }

        private static System.Collections.ArrayList lockObject = new System.Collections.ArrayList();

        /// <summary>Deletes the specified file if it exists.</summary>
        /// <param name="path">File to delete.</param>
        public static void EnsureFileDeleted(string path)
        {
            lock (lockObject)
            {
                if (File.Exists(path))
                    File.Delete(path);

                while (File.Exists(path))
                    Threading.Thread.Sleep(1000);
            }
        }

        public static void CreateEmptyFile(string path)
        {
            lock (lockObject)
            {
                File.Open(path, FileMode.CreateNew).Close();
            }
        }

        public static string ReadAllFileText(string path)
        {
            lock (lockObject)
            {
                return File.ReadAllText(path);
            }
        }
  
        /// <summary>Writes a resource with a trailing matching file name to a file.</summary>
        /// <param name="assembly">Assembly containing the resource to write.</param>
        /// <param name="path">Path to file to write.</param>
        public static void FindAndWriteResourceToFile(Assembly assembly, string path)
        {
            CheckArgumentNotNull(assembly, "assembly");
            CheckArgumentNotNull(path, "path");
            string resourceName = FindResourceNameForPath(assembly, path);
            CopyResourceToFile(assembly, resourceName, path);
        }
        
        /// <summary>Writes a resource with a trailing matching file name to a file.</summary>
        /// <param name="assembly">Assembly containing the resource to write.</param>
        /// <param name="path">Path to file to write.</param>
        public static void FindAndWriteResourceToFile(Assembly assembly, string partialResourceName, string path)
        {
            CheckArgumentNotNull(assembly, "assembly");
            CheckArgumentNotNull(path, "path");

            string actualResourceName = null;
            foreach (string resourceName in assembly.GetManifestResourceNames())
            {
                if (resourceName.EndsWith(partialResourceName, StringComparison.OrdinalIgnoreCase))
                {
                    actualResourceName= resourceName;
                    break;
                }
            }
            if (actualResourceName == null)
            {
                throw new InvalidOperationException(
                    "Unable to find a resource name in assembly '" + assembly.Location +
                    "' for path '" + partialResourceName + "' in this list:\r\n" +
                    ListResourceNames(assembly));
            }
            CopyResourceToFile(assembly, actualResourceName, path);
        }

            
        /// <summary>Finds the resource with the trailing matching file name.</summary>
        /// <param name="assembly">Assembly containing resource to look for.</param>
        /// <param name="path">Path with file name to find.</param>
        /// <returns>Resource name of resource.</returns>
        /// <exception cref="InvalidOperationException">When a match cannot be found for <paramref name="path"/>.</exception>
        public static string FindResourceNameForPath(Assembly assembly, string path)
        {
            CheckArgumentNotNull(assembly, "assembly");
            CheckArgumentNotNull(path, "path");

            string specificResourceName = Path.GetFileName(path);
            foreach (string resourceName in assembly.GetManifestResourceNames())
            {
                if (resourceName.EndsWith(specificResourceName, StringComparison.OrdinalIgnoreCase))
                {
                    return resourceName;
                }
            }

            throw new InvalidOperationException(
                "Unable to find a resource name in assembly '" + assembly.Location +
                "' for path '" + path + "' in this list:\r\n" +
                ListResourceNames(assembly));
        }

        /// <summary>Creates a newline-separated list of resource names in the specified assembly.</summary>
        /// <param name="assembly">Assembly to list resource names from.</param>
        /// <returns>A string with resource names separated by line breaks.</returns>
        public static string ListResourceNames(Assembly assembly)
        {
            CheckArgumentNotNull(assembly, "assembly");
            string[] resourceNames = assembly.GetManifestResourceNames();
            Array.Sort(resourceNames, StringComparer.OrdinalIgnoreCase);
            return string.Join(Environment.NewLine, resourceNames);
        }

        /// <summary>Reads all text from the specified resource.</summary>
        /// <param name="assembly">Assembly with resource.</param>
        /// <param name="resourceName">Name of resource in assembly.</param>
        /// <returns>All text from the specified resource.</returns>
        public static string ReadResourceText(Assembly assembly, string resourceName)
        {
            CheckArgumentNotNull(assembly, "assembly");
            CheckArgumentNotNull(resourceName, "resourceName");
            using (Stream sourceStream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(sourceStream))
            {
                if (sourceStream == null)
                {
                    throw new InvalidOperationException("Unable to extract resource '" + resourceName + "' from " +
                        "assembly '" + assembly.Location + "' with these resources:\r\n" + ListResourceNames(assembly));
                }

                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Creates a new file, writes the specified string to the file, and then closes the file. If the target 
        /// file already exists, it is overwritten.
        /// </summary>
        /// <param name="path">The file to write to.</param>
        /// <param name="contents">The string to write to the file.</param>
        public static void WriteAllTextWithRetry(string path, string contents)
        {
            CheckArgumentNotNull(path, "path");
            CheckArgumentNotNull(contents, "contents");
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    File.WriteAllText(path, contents);
                    return;
                }
                catch (IOException)
                {
                    Trace.Write("Unable to write contents to " + path + " on iteration #" + (i + 1) + " - waiting 2 seconds...");
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
                }
            }
        }
        
        #endregion Public methods.

        #region Private methods.
        
        /// <summary>Checks that <paramref name="argumentValue"/> is not null, throws an exception otherwise.</summary>
        /// <param name="argumentValue">Argument value to check.</param>
        /// <param name="argumentName">Argument name.</param>
        public static void CheckArgumentNotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }
        
        #endregion Private methods.
    }
}
