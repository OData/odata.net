//---------------------------------------------------------------------
// <copyright file="IISHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Reflection;
using System.Threading;

namespace System.Data.Test.Astoria.Util
{
    public class IISHelper
    {
        internal static string GetWWWRootSharePath(string machineName)
        {
            if (AstoriaTestProperties.Host == Host.LocalIIS)
            {
                return Path.Combine(Environment.GetEnvironmentVariable("SystemDrive"),@"\inetpub\wwwroot");
            }

            return null;
        }
        internal static string GetLocalMachineWWWRootSharePath(string machineName)
        {
            if (AstoriaTestProperties.Host == Host.LocalIIS)
            {
                return GetWWWRootSharePath(machineName);
            }

            return null;
        }
        
    }

 
}
