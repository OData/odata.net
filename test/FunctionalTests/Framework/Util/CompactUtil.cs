//---------------------------------------------------------------------
// <copyright file="CompactUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System.Diagnostics;
    using System.IO;

    /// <summary>This class provides a wrapper for the compact.exe utility.</summary>
    /// <remarks>
    /// COMPACT [/C | /U] [/S[:dir]] [/A] [/I] [/F] [/Q] [filename [...]]
    /// 
    ///   /C        Compresses the specified files.  Directories will be marked
    ///             so that files added afterward will be compressed.
    ///   /U        Uncompresses the specified files.  Directories will be marked
    ///             so that files added afterward will not be compressed.
    ///   /S        Performs the specified operation on files in the given
    ///             directory and all subdirectories.  Default "dir" is the
    ///             current directory.
    ///   /A        Displays files with the hidden or system attributes.  These
    ///             files are omitted by default.
    ///   /I        Continues performing the specified operation even after errors
    ///             have occurred.  By default, COMPACT stops when an error is
    ///             encountered.
    ///   /F        Forces the compress operation on all specified files, even
    ///             those which are already compressed.  Already-compressed files
    ///             are skipped by default.
    ///   /Q        Reports only the most essential information.
    ///   filename  Specifies a pattern, file, or directory.
    ///   
    ///   Used without parameters, COMPACT displays the compression state of
    ///   the current directory and any files it contains. You may use multiple
    ///   filenames and wildcards.  You must put spaces between multiple
    ///   parameters.
    /// </remarks>
    public class CompactUtil
    {
        /// <summary>Directory and subdirectory on which to act.</summary>
        private string directory;

        /// <summary>Files, directories or patterns on which to act.</summary>
        private string[] files;

        /// <summary>Whether the compress operation should be forced.</summary>
        private bool forceCompression;

        /// <summary>Whether operations should continue after errors.</summary>
        private bool ignoreErrors;

        /// <summary>Whether only the most essential information should be reported.</summary>
        private bool quiet;

        /// <summary>
        /// Initializes a new <see cref="CompactUtil"/> instance.
        /// </summary>
        public CompactUtil(params string[] files)
        {
            this.files = files ?? new string[0];
        }

        /// <summary>Directory and subdirectory on which to act.</summary>
        public string Directory
        {
            get { return this.directory; }
            set { this.directory = value; }
        }

        /// <summary>Files, directories or patterns on which to act.</summary>
        public string[] Files
        {
            [DebuggerStepThrough]
            get { return this.files; }

            [DebuggerStepThrough]
            set { this.files = value; }
        }

        /// <summary>Whether the compress operation should be forced.</summary>
        public bool ForceCompression
        {
            get { return this.forceCompression; }
            set { this.forceCompression = value; }
        }

        /// <summary>Whether operations should continue after errors.</summary>
        public bool IgnoreErrors
        {
            get { return this.ignoreErrors; }
            set { this.ignoreErrors = value; }
        }

        /// <summary>Whether only the most essential information should be reported.</summary>
        public bool Quiet
        {
            get { return this.quiet; }
            set { this.quiet = value; }
        }

        /// <summary>Checks whether the specified file is compressed.</summary>
        /// <param name="path">Path to check for compression.</param>
        /// <returns>true if the file is compressed; false otherwise.</returns>
        public static bool IsFileCompressed(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            return 0 != (File.GetAttributes(path) & FileAttributes.Compressed);
        }

        /// <summary>Uncompresses the specified file.</summary>
        /// <param name="path">Path to file to compress.</param>
        public static void UncompressFile(string path)
        {
            if (IsFileCompressed(path))
            {
                (new CompactUtil(path)).Uncompress();
            }
        }

        /// <summary>Runs a compression operation.</summary>
        public void Compress()
        {
            RunWithArguments("/C" + BuildGeneralArguments());
        }

        /// <summary>Runs an uncompression operation.</summary>
        public void Uncompress()
        {
            RunWithArguments("/U" + BuildGeneralArguments());
        }

        /// <summary>
        /// Builds the argument line that is common to all operations.
        /// </summary>
        /// <returns></returns>
        private string BuildGeneralArguments()
        {
            string result = "";
            if (this.quiet) result += " /Q";
            if (this.forceCompression) result += " /F";
            if (this.ignoreErrors) result += " /I";
            if (!String.IsNullOrEmpty(this.directory)) result += " /S \"" + this.directory + "\" ";
            foreach (string file in this.files)
            {
                result += " \"" + file + "\"";
            }

            return result;
        }

        private static void RunWithArguments(string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.Arguments = arguments;
            startInfo.CreateNoWindow = true;
            startInfo.FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "compact.exe");
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;

            using (Process process = Process.Start(startInfo))
            {
                string outputText = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException("compact command failed:\r\n" + outputText);
                }
            }
        }
    }
}
