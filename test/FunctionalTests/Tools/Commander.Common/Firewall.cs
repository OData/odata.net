//---------------------------------------------------------------------
// <copyright file="Firewall.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Text;
using System.Globalization;
using System.Security.Permissions;
using System.Security.Principal;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Firewall
{


    [ComImport, ComVisible(false), Guid("304CE942-6E39-40D8-943A-B913C40C9CD4")]
    public class NetFwMgr
    {

    }
    [ComImport, ComVisible(false), Guid("F7898AF5-CAC4-4632-A2EC-DA06E5111AF2"), System.Runtime.InteropServices.InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface INetFwMgr
    {

        INetFwPolicy LocalPolicy { get; }

        FirewallProfileType CurrentProfileType { get; }

        void RestoreDefaults();

        void IsPortAllowed(string imageFileName,
         IPVersion ipVersion,
         long portNumber,
         string localAddress,
         IPProtocol ipProtocol,
         [Out] out bool allowed,
         [Out] out bool restricted);

        void IsIcmpTypeAllowed(IPVersion ipVersion,
          string localAddress,
          byte type,
          [Out] out bool allowed,
          [Out] out bool restricted);
    }

    [ComImport, ComVisible(false), Guid("D46D2478-9AC9-4008-9DC7-5563CE5536CC"), System.Runtime.InteropServices.InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface INetFwPolicy
    {

        INetFwProfile CurrentProfile { get; }
        INetFwProfile GetProfileByType(FirewallProfileType profileType);
    }

    [ComImport, ComVisible(false), Guid("174A0DDA-E9F9-449D-993B-21AB667CA456"), System.Runtime.InteropServices.InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface INetFwProfile
    {


        FirewallProfileType Type { get; }
        bool FirewallEnabled { get; set; }
        bool ExceptionsNotAllowed { get; set; }
        bool NotificationsDisabled { get; set; }
        bool UnicastResponsesToMulticastBroadcastDisabled { get; set; }
        INetFwRemoteAdminSettings RemoteAdminSettings { get; }
        INetFwIcmpSettings IcmpSettings { get; }
        INetFwOpenPorts GloballyOpenPorts { get; }
        INetFwServices Services { get; }
        INetFwAuthorizedApplications AuthorizedApplications { get; }


    }

    [ComImport, ComVisible(false), Guid("D4BECDDF-6F73-4A83-B832-9C66874CD20E"), System.Runtime.InteropServices.InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface INetFwRemoteAdminSettings
    {
        IPVersion IpVersion { get; set; }

        Scope Scope { get; set; }

        string RemoteAddresses { get; set; }

        bool Enabled { get; set; }
    }

    [ComImport, ComVisible(false), Guid("A6207B2E-7CDD-426A-951E-5E1CBC5AFEAD"), System.Runtime.InteropServices.InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface INetFwIcmpSettings
    {
        bool AllowOutboundDestinationUnreachable { get; set; }

        bool AllowRedirect { get; set; }

        bool AllowInboundEchoRequest { get; set; }

        bool AllowOutboundTimeExceeded { get; set; }

        bool AllowOutboundParameterProblem { get; set; }

        bool AllowOutboundSourceQuench { get; set; }

        bool AllowInboundRouterRequest { get; set; }

        bool AllowInboundTimestampRequest { get; set; }

        bool AllowInboundMaskRequest { get; set; }

        bool AllowOutboundPacketTooBig { get; set; }

    }

    [ComImport, ComVisible(false), Guid("C0E9D7FA-E07E-430A-B19A-090CE82D92E2"), System.Runtime.InteropServices.InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface INetFwOpenPorts
    {
        long Count { get; }

        void Add(INetFwOpenPort port);

        void Remove(long portNumber, IPProtocol ipProtocol);

        INetFwOpenPort Item(long portNumber, IPProtocol ipProtocol);

        System.Collections.IEnumerator NewEnum { get; }
    }

    [ComImport, ComVisible(false), Guid("E0483BA0-47FF-4D9C-A6D6-7741D0B195F7"), System.Runtime.InteropServices.InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface INetFwOpenPort
    {


        string Name { get; set; }

        IPVersion IpVersion { get; set; }

        IPProtocol Protocol { get; set; }

        long Port { get; set; }

        Scope Scope { get; set; }

        string RemoteAddresses { get; set; }

        bool Enabled { get; set; }

        bool BuiltIn { get; }

    }

    [ComImport, ComVisible(false), Guid("79649BB4-903E-421B-94C9-79848E79F6EE"), System.Runtime.InteropServices.InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface INetFwServices
    {
        long Count { get; }

        INetFwService Item(ServiceType svcType);

        System.Collections.IEnumerator NewEnum { get; }

    }

    [ComImport, ComVisible(false), Guid("79FD57C8-908E-4A36-9888-D5B3F0A444CF"), System.Runtime.InteropServices.InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface INetFwService
    {
        string Name { get; }

        ServiceType Type { get; }

        bool Customized { get; }

        IPVersion IpVersion { get; set; }

        Scope Scope { get; set; }

        string RemoteAddresses { get; set; }

        bool Enabled { get; set; }

        INetFwOpenPorts GloballyOpenPorts { get; }

    }

    [ComImport, ComVisible(false), Guid("644EFD52-CCF9-486C-97A2-39F352570B30"), System.Runtime.InteropServices.InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface INetFwAuthorizedApplications
    {
        long Count { get; }

        void Add(INetFwAuthorizedApplication port);

        void Remove(string imageFileName);

        INetFwAuthorizedApplication Item(string imageFileName);

        System.Collections.IEnumerator NewEnum { get; }
    }

    [ComImport, ComVisible(false), Guid("EC9846B3-2762-4A6B-A214-6ACB603462D2")]
    public class NetFwAuthorizedApplication
    {

    }

    [ComImport, ComVisible(false), Guid("B5E64FFA-C2C5-444E-A301-FB5E00018050"), System.Runtime.InteropServices.InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface INetFwAuthorizedApplication
    {
        string Name { get; set; }

        string ProcessImageFileName { get; set; }


        IPVersion IpVersion { get; set; }

        Scope Scope { get; set; }

        string RemoteAddresses { get; set; }

        bool Enabled { get; set; }
    }

    public enum FirewallProfileType
    {
        Domain = 0,
        Standard = 1,
        Current = 2,
        Max = 3
    }

    public enum IPVersion
    {
        IPv4 = 0,
        IPv6 = 1,
        IPAny = 2,
        IPMax = 3
    }
    public enum IPProtocol
    {
        Tcp = 6,
        Udp = 17
    }

    public enum Scope
    {
        All = 0,
        Subnet = 1,
        Custom = 2,
        Max = 3
    }

    public enum ServiceType
    {
        FileAndPrint = 0,
        UPnP = 1,
        RemoteDesktop = 2,
        None = 3,
        Max = 4

    }

    public class PortHelper
    {
        public static bool IsPortOpen(string portClassName, long port)
        {
            INetFwMgr icfMgr = null;
            Type TicfMgr = Type.GetTypeFromProgID("HNetCfg.FwMgr");
            icfMgr = (INetFwMgr)Activator.CreateInstance(TicfMgr);

            INetFwProfile profile = icfMgr.LocalPolicy.CurrentProfile;

            // Get the current profile
            profile = icfMgr.LocalPolicy.CurrentProfile;

            IEnumerator  enumerator = profile.GloballyOpenPorts.NewEnum;
            while(enumerator.MoveNext())
            {
                INetFwOpenPort openPort = enumerator.Current as INetFwOpenPort;
                if(openPort.Port == port && openPort.Name.Equals(portClassName))
                    return true;
            }
            return false;
        }
        public static void AddGlobalOpenPort(string portClassName, long port)
        {
            INetFwMgr icfMgr = null;
            Type TicfMgr = Type.GetTypeFromProgID("HNetCfg.FwMgr");
            icfMgr = (INetFwMgr)Activator.CreateInstance(TicfMgr);

            Console.WriteLine("CurrentProfileType: " + icfMgr.CurrentProfileType);

            INetFwProfile profile = icfMgr.LocalPolicy.CurrentProfile;

            INetFwOpenPort portClass;
            Type TportClass = Type.GetTypeFromProgID("HNetCfg.FWOpenPort");
            portClass = (INetFwOpenPort)Activator.CreateInstance(TportClass);

            // Set the port properties
            portClass.Scope = Scope.All;
            portClass.Enabled = true;
            portClass.Name = portClassName;
            portClass.Port = port;
            portClass.Protocol =IPProtocol.Tcp;

            // Add the port to the ICF Permissions List
            profile.GloballyOpenPorts.Add(portClass);

        }
    }
}

