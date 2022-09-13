//---------------------------------------------------------------------
// <copyright file="CabExtractor.cs" company="Microsoft">
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

    /// <summary>
    /// Utility class that extracts contents of CAB file into a specified directory.
    /// </summary>
    public class CabExtractor
    {
        /// <summary>
        /// Initializes a new instance of the CabExtractor class.
        /// </summary>
        public CabExtractor()
        {
        }

        /// <summary>
        /// Extracts given cab file into the specified directory.
        /// </summary>
        /// <param name="cabFile">Path to CAB file (must exist)</param>
        /// <param name="destinationDirectory">Path to destination directory (must exist)</param>
        [SecuritySafeCritical]
        public void ExtractCab(string cabFile, string destinationDirectory)
        {
            if (!File.Exists(cabFile))
            {
                throw new TaupoInfrastructureException("CAB file '" + cabFile + "' does not exist.");
            }

            if (!Directory.Exists(destinationDirectory))
            {
                throw new TaupoInfrastructureException("Directory '" + destinationDirectory + "' does not exist.");
            }

            bool isTemporary;
            string expandExe = this.GetExpandExePath(out isTemporary);
            try
            {
                string arguments = string.Format(CultureInfo.InvariantCulture, "\"{0}\" -F:* \"{1}\"", cabFile, destinationDirectory);
                var psi = new ProcessStartInfo(expandExe, arguments)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                using (Process proc = Process.Start(psi))
                {
                    proc.WaitForExit();
                    if (proc.ExitCode != 0)
                    {
                        throw new TaupoInvalidOperationException(psi.FileName + " has exited with exit code " + proc.ExitCode);
                    }
                }
            }
            finally
            {
                if (isTemporary)
                {
                    this.CleanUp(expandExe);
                }
            }
        }

        private void CleanUp(string temporaryExpandExe)
        {
            string dirName = Path.GetDirectoryName(temporaryExpandExe);
            Directory.Delete(dirName, true);
        }

        private string GetExpandExePath(out bool isTemporary)
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                // in Vista/2008 we can use expand.exe that comes with the OS
                string expandExe = Path.Combine(Environment.SystemDirectory, "expand.exe");
                if (!File.Exists(expandExe))
                {
                    throw new TaupoInfrastructureException("File " + expandExe + " does not exist.");
                }

                isTemporary = false;
                return expandExe;
            }
            else
            {
                // on previous OSes w use expand.exe and dpx.dll taken from Win2008 and stored in resources
                string tempDir = Path.Combine(Path.GetTempPath(), "expandExeWorkaround" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(tempDir);

                var assembly = this.GetType().GetAssembly();
                ResourceUtilities.ExtractCompressedResourceToDirectory(tempDir, assembly, "expand.exe");
                ResourceUtilities.ExtractCompressedResourceToDirectory(tempDir, assembly, "dpx.dll");
                isTemporary = true;
                return Path.Combine(tempDir, "expand.exe");
            }
        }
    }
}
