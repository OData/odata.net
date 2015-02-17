//---------------------------------------------------------------------
// <copyright file="ProcessHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria.Util
{
    using System.Collections.Generic;
    using Microsoft.CSharp;         //CSharpCodeProvider
    using Microsoft.VisualBasic;    //VBCodeProvider
    using System.CodeDom.Compiler;  //CompilerParameters
    using System.Data.Test.Astoria;
    using System.Security.Principal;
    using System.Diagnostics;
    using Microsoft.Test.ModuleCore;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Tcp;


    public class ProcessHelper
    {
        public static bool IsCommanderRunningOnLocalMachine()
        {
            Process[] processes = Process.GetProcessesByName("Commander.Server");
            if (processes.Length == 0)
                return false;
            return true;
        }
        internal static void UseCommander(string machineName, Action<Commander.RemoteServer> commanderFunc)
        {
            TcpChannel channel = new TcpChannel();
            var existingTcpChannel = ChannelServices.GetChannel("tcp");
            if (existingTcpChannel != null)
            {
                ChannelServices.UnregisterChannel(existingTcpChannel);
            }

            ChannelServices.RegisterChannel(channel, true);
            Commander.RemoteServer remoteServer = Commander.RemoteServer.CreateLoggedInClientRemoteServer(machineName);
            commanderFunc(remoteServer);
            ChannelServices.UnregisterChannel(channel);
        }

        public static bool IsVistaOrLater()
        {
            return Environment.OSVersion.Platform ==
                     PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6;
        }

        public static bool IsLocalMachine(string machineName)
        {
            if (machineName == null)
                throw new ArgumentNullException("machineName cannot be null");
            if (machineName.Equals(Environment.MachineName, StringComparison.InvariantCultureIgnoreCase))
                return true;
            else if (machineName.Equals(".", StringComparison.InvariantCultureIgnoreCase))
                return true;
            else if (machineName.Equals("localhost", StringComparison.InvariantCultureIgnoreCase))
                return true;
            else
                return false;

        }
       
    }
}
