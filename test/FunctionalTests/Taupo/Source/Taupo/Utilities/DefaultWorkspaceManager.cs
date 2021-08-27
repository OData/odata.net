//---------------------------------------------------------------------
// <copyright file="DefaultWorkspaceManager.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Security;
    using System.Security.Permissions;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Default implementation of <see cref="IWorkspaceManager" />
    /// </summary>
    [ImplementationName(typeof(IWorkspaceManager), "Default")]
    public sealed class DefaultWorkspaceManager : IWorkspaceManager, IDisposable
    {
        private const string LockFileName = "WorkspaceManager.lock";
        private static object lockObject = new object();
        private int counter;
        private bool isInitialized;
        private FileStream lockFile;
        private string lockFileName;

        /// <summary>
        /// Initializes a new instance of the DefaultWorkspaceManager class.
        /// </summary>
        public DefaultWorkspaceManager()
            : this(Path.Combine(IOHelpers.BaseTempDirectory, BuildDefaultDirectoryName()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the DefaultWorkspaceManager class.
        /// </summary>
        /// <param name="baseDirectory">The base directory for workspaces.</param>
        public DefaultWorkspaceManager(string baseDirectory)
        {
            this.BaseDirectory = IOHelpers.GetFullPath(baseDirectory);
            this.Logger = Logger.Null;
        }

        /// <summary>
        /// Gets or sets the base directory for all workspaces.
        /// </summary>
        /// <value>The base directory.</value>
        [InjectTestParameter("WorkspaceBaseDirectory", HelpText = "Location of the 'Workspaces' directory", DefaultValueDescription = "(current directory)\\Workspaces")]
        public string BaseDirectory { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        [InjectDependency]
        public Logger Logger { get; set; }

        /// <summary>
        /// Gets the name of the workspace given proposed name.
        /// </summary>
        /// <param name="proposedName">Proposed name.</param>
        /// <returns>Unique workspace name.</returns>
        public string GetUniqueWorkspaceName(string proposedName)
        {
            if (this.IsAvailable(proposedName))
            {
                return proposedName;
            }

            string baseName = proposedName;

            while (true)
            {
                proposedName = baseName + this.counter;
                if (this.IsAvailable(proposedName))
                {
                    return proposedName;
                }

                this.counter++;
            }
        }

        /// <summary>
        /// Gets the workspace path.
        /// </summary>
        /// <param name="workspaceName">Name of the workspace.</param>
        /// <returns>Path to the workspace directory.</returns>
        public string GetWorkspacePath(string workspaceName)
        {
            this.EnsureInitialized();

            return Path.Combine(this.BaseDirectory, workspaceName);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        [SecuritySafeCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [AssertJustification("Calling File.Delete demands FileIOPermission (Write flag) to the file being deleted.")]
        public void Dispose()
        {
            try
            {
                if (this.lockFile != null)
                {
                    lock (lockObject)
                    {
                        this.lockFile.Close();
                        this.lockFile = null;
                        File.Delete(this.lockFileName);
                    }
                }
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Helper method to build a default name for a workspace manager's base directory. Will be different for workspace managers in different processes.
        /// </summary>
        /// <returns>The default name to use for a workspace manager's base directory</returns>
        [SecuritySafeCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [AssertJustification("Process.GetCurrentProcess P/Invokes to native code to the get the process ID without suppressing unmanaged code security, so this demands SecurityPermission/UnmanagedCode.")]
        internal static string BuildDefaultDirectoryName()
        {
            return "Workspaces" + Process.GetCurrentProcess().Id;
        }

        /// <summary>
        /// Ensures this <see cref="DefaultWorkspaceManager"/> is initialized.
        /// </summary>
        [SecuritySafeCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [AssertJustification("Calling Directory.Exists and Directory.CreateDirectory demands FileIOPermission (Read | Write) to the paths used as parameters to those methods.")]
        private void EnsureInitialized()
        {
            if (!this.isInitialized)
            {
                if (Directory.Exists(this.BaseDirectory))
                {
                    this.AcquireDirectoryLock();
                    this.ArchiveDirectoryContents(this.BaseDirectory);
                }
                else
                {
                    Directory.CreateDirectory(this.BaseDirectory);
                    this.AcquireDirectoryLock();
                }

                this.isInitialized = true;
            }
        }

        private void AcquireDirectoryLock()
        {
            // try to claim a directory by creating a non-shareable file in it
            // this will prevent other workspace managers from working on this directory until this 
            // file is closed
            try
            {
                this.lockFileName = Path.Combine(this.BaseDirectory, LockFileName);
                this.lockFile = new FileStream(this.lockFileName, FileMode.Create, FileAccess.Write, FileShare.None, 100);
            }
            catch (IOException ex)
            {
                throw new TaupoInvalidOperationException("Cannot acquire exclusive access to directory '" + this.BaseDirectory + "'", ex);
            }
        }

        private bool IsAvailable(string proposedName)
        {
            return !IOHelpers.DirectoryExists(this.GetWorkspacePath(proposedName));
        }

        /// <summary>
        /// Archives the contents of the specified <paramref name="directory"/>
        /// to a folder of the same name plus a timestamp.
        /// </summary>
        /// <param name="directory">The name of the directory, relative
        /// to the current directory.</param>
        [SecuritySafeCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [AssertJustification("Calling Directory.Exists and Directory.CreateDirectory demands FileIOPermission (Read | Write) to the paths used as parameters to those methods.")]
        private void ArchiveDirectoryContents(string directory)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            string archiveDirectory = directory + "." + timestamp;

            Directory.CreateDirectory(archiveDirectory);

            // move all directories found in baseFolder to archiveDirectory
            foreach (string dirName in Directory.GetDirectories(directory))
            {
                string baseName = Path.GetFileName(dirName);

                try
                {
                    Directory.Move(dirName, Path.Combine(archiveDirectory, baseName));
                }
                catch (IOException)
                {
                    // in use - ignore
                    this.Logger.WriteLine(LogLevel.Warning, "Unable to archive directory '{0}'", dirName);
                }
            }
        }
    }
}
