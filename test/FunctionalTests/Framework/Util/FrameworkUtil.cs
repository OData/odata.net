//---------------------------------------------------------------------
// <copyright file="FrameworkUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    #region Namespaces

    using System;
    using System.Diagnostics;
    using System.IO;

    #endregion Namespaces

    /// <summary>This class provides a variety of utility methods for tests.</summary>
    public static partial class TestUtil
    {
        public static string FrameworkVersion
        {
            get { return Environment.GetEnvironmentVariable("WINFX_REFS_VERSION"); }
        }

        public static string GreenBitsReferenceAssembliesDirectory
        {
            get
            {
                string referenceDirectory = null;

                // If DD_NdpGreenBitsInstallPath environment variable is present,
                // return the latest netfx reference assemblies directory.
                //
                // If the variable is not present,
                // First look for a netfx install directory for version 4.0 or higher of netfx.
                // If a directory is not found that matches, return the latest nexfx reference assemblies directory.
                //
                // If none of these directories can be found, return null.

                Version version = typeof(object).Assembly.GetName().Version;
                if ((version.Major == 2) && (version.Minor == 0))
                {
                    referenceDirectory = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                        @"\Reference Assemblies\Microsoft\Framework\v3.5");
                }
                else
                {   // expecting 4.0
                    referenceDirectory = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
                }

                Debug.Assert(Directory.Exists(referenceDirectory), "Missing directory \"" + referenceDirectory + "\"");
                return referenceDirectory;
            }
        }

        public static string DataFxInstallDirectory
        {
            get
            {
                return System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
            }
        }


        public static string FrameworkDirectory
        {
            get
            {
                return Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "Microsoft.NET\\Framework");
            }
        }

        private static string datasvcutil;
        public static string DataSvcUtilExe
        {
            get
            {
                if (null == datasvcutil)
                {
                    var tmp = Environment.ExpandEnvironmentVariables(Path.Combine(DataFxAssemblyRef.File.DS_Tools_InstallPath, DataFxAssemblyRef.File.DataSvcUtil));
                    if (!File.Exists(tmp))
                    {
                        throw new FileNotFoundException("Cannot find DataSvcUtil in the framework dir, ddsuites dir, or current executing dir");
                    }

                    datasvcutil = tmp;
                }
                return datasvcutil;
            }
        }

        /// <summary>
        ///  Find the latest version of the frameword installed in the base path
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="minimumVersion"></param>
        /// <returns></returns>
        public static string FindLatestNetFxDirectory(string basePath, Version minimumVersion)
        {
            string pathPrefix = Path.Combine(basePath, "v");
            string[] referenceDirectories = Directory.GetDirectories(basePath, "v*", SearchOption.TopDirectoryOnly);
            string highestVersionedDirectory = null;
            Version lowestVersion = new Version(0, 0, 0, 0);
            Version highestVersion = lowestVersion;
            Version version;
            bool isGoodVersionString;

            if (minimumVersion == null)
            {
                minimumVersion = lowestVersion;
            }

            for (int i = 0; i < referenceDirectories.Length; ++i)
            {
                // No TryParse for Version class, so we have to catch exceptions.
                version = lowestVersion;
                try
                {
                    version = new Version(referenceDirectories[i].Substring(pathPrefix.Length));
                    isGoodVersionString = true;
                }
                catch (FormatException) { isGoodVersionString = false; }
                catch (OverflowException) { isGoodVersionString = false; }
                catch (ArgumentException) { isGoodVersionString = false; }

                if (isGoodVersionString)
                {
                    if (version >= minimumVersion && version > highestVersion)
                    {
                        highestVersionedDirectory = referenceDirectories[i];
                        highestVersion = version;
                    }
                }
            }

            return highestVersionedDirectory;
        }

        private static string enlistmentRoot;

        /// <summary>
        /// Heuristically determine the EnlistmentRoot location.
        /// </summary>
        public static string EnlistmentRoot
        {
            get
            {
                if (string.IsNullOrEmpty(enlistmentRoot))
                {
                    enlistmentRoot = FindEnlistmentRootHeuristically();
                }

                return enlistmentRoot;
            }
        }

        private static string FindEnlistmentRootHeuristically()
        {
            // First use environment variable if defined by user.
            string result = Environment.GetEnvironmentVariable("ENLISTMENT_ROOT");
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }

            // Second use current assembly location to determine.
            string assemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(assemblyPath))
            {
                return null;
            }

            string[] traitsFolders =
            {
                @"\bin\",
                @"\src\",
                @"\test\",
                @"\sln\"
            };

            // The assembly path should have the pattern of '$(ENLISTMENT_ROOT)$(traitsFolder)'.
            foreach (string traitsFolder in traitsFolders)
            {
                int start = assemblyPath.IndexOf(traitsFolder, StringComparison.InvariantCultureIgnoreCase);
                if (start > 0)
                {
                    result = assemblyPath.Substring(0, start);
                    break;
                }
            }

            if (string.IsNullOrEmpty(result))
            {
                // Unable to determine ENLISTMENT_ROOT from these traits folders.
                return null;
            }

            // We check the result path again to ensure it is an ENLISTMENT_ROOT.
            foreach (string traitsFolder in traitsFolders)
            {
                string pathToCheck = result + traitsFolder;
                if (!Directory.Exists(pathToCheck))
                {
                    // The result seems not be a valid enlistment root containing some necessary subfolders.
                    return null;
                }
            }

            // At last we find ENLISTMENT_ROOT.
            return result;
        }
    }
}
