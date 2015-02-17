//---------------------------------------------------------------------
// <copyright file="CompressionUtility.cs" company="Microsoft">
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
    using System.IO.Packaging;

    #endregion Namespaces

    public class ZipUtility
    {

        public static void UnzipResourceToFolder(Assembly assembly, string partialResourceName, string destFolder)
        {
            using (Package pkgMain = Package.Open(partialResourceName, FileMode.Open, FileAccess.Read))
            {
                foreach (PackagePart pkgPart in pkgMain.GetParts())
                {
                    string strTarget = Path.Combine(destFolder, pkgPart.Uri.OriginalString);
                    using (Stream stmSource = pkgPart.GetStream(FileMode.Open, FileAccess.Read))
                    {
                        using (Stream stmDestination = File.OpenWrite(strTarget))
                        {
                            byte[] arrBuffer = new byte[10000];
                            int intRead;
                            intRead = stmSource.Read(arrBuffer, 0, arrBuffer.Length);
                            while (intRead > 0)
                            {
                                stmDestination.Write(arrBuffer, 0, intRead);
                                intRead = stmSource.Read(arrBuffer, 0, arrBuffer.Length);
                            }
                        }
                    }
                }
            }
        }

    }
}
