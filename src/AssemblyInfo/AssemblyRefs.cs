//---------------------------------------------------------------------
// <copyright file="AssemblyRefs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

/// <summary>Current assembly reference</summary>
/// <remarks>DO NOT FORGET TO ALSO UPDATE AssemblyRefs.vb</remarks>
internal static class DataFxAssemblyRef
{
    /// <summary>Current assembly names</summary>
    internal static class Name
    {
        internal const string mscorlib = "mscorlib";
        internal const string System = "System";
        internal const string SystemCore = "System.Core";
        internal const string SystemData = "System.Data";
        internal const string SystemDataDataSetExtensions = "System.Data.DataSetExtensions";
        internal const string SystemDataEntity = "System.Data.Entity";
        internal const string SystemDataLinq = "System.Data.Linq";
        internal const string SystemDrawing = "System.Drawing";
        internal const string SystemNumerics = "System.Numerics";
        internal const string SystemRuntimeRemoting = "System.Runtime.Remoting";
        internal const string SystemRuntimeSerialization = "System.Runtime.Serialization";
        internal const string SystemServiceModel = "System.ServiceModel";
        internal const string SystemWeb = "System.Web";
        internal const string SystemWebEntity = "System.Web.Entity";
        internal const string SystemWebEntityDesign = "System.Web.Entity.Design";
        internal const string SystemXml = "System.Xml";
        internal const string SystemXmlLinq = "System.Xml.Linq";

        internal const string DataEntity = "System.Data.Entity";
        internal const string EntityFramework = "EntityFramework";
        internal const string DataEntityDesign = "System.Data.Entity.Design";
        internal const string DataServices = "Microsoft.OData.Service";
        internal const string DataServicesClient = "Microsoft.OData.Client";
        internal const string DataSvcUtil = "DataSvcUtil";
        internal const string OData = "Microsoft.OData.Core";
        internal const string Spatial = "Microsoft.Spatial";
        internal const string EntityDataModel = "Microsoft.OData.Edm";
    }

    /// <summary>Current assembly file names</summary>
    internal static class File
    {
        /// <summary>base version for data framework</summary>
        internal const string DS_BaseVersion = VersionConstants.ReleaseVersion;

        /// <summary>where to find desktop reference client sku reference assemblies</summary>
        internal const string DotNetFrameworkV4_ClientReferenceAssemblyPath = @"%ProgramFiles%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\Profile\Client";

        /// <summary>where to find desktop reference extended sku reference assemblies</summary>
        internal const string DotNetFrameworkV4_ReferenceAssemblyPath = @"%ProgramFiles%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0";

        /// <summary>where find executable binaries</summary>
        internal const string DotNetFrameworkV4_InstallPath = @"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319";

        internal const string SilverlightV5_ReferenceAssemblyPath = @"%ProgramFiles%\Microsoft Silverlight\5.1.20913.0";
        internal const string SilverlightV5_SdkClientReferenceAssemblyPath = @"%ProgramFiles%\Microsoft SDKs\Silverlight\v5.0\Libraries\Client";
        internal const string SilverlightV5_SdkServerReferenceAssemblyPath = @"%ProgramFiles%\Microsoft SDKs\Silverlight\v5.0\Libraries\Server";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal static string DE_InstallPath
        {
            get { return GetDE_InstallPath(); }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal static string DS_InstallPath
        {
            get { return GetDS_InstallPath(); }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal static string DS_PortableInstallPath
        {
            get { return GetDS_PortableInstallPath(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal static string DS_Tools_InstallPath
        {
            get { return GetDS_Tools_InstallPath(); }
        }

        /// <summary>where to find desktop reference assemblies</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal static string DE_ReferenceAssemblyPath
        {
            get { return GetDE_InstallPath(); }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal static string DS_ReferenceAssemblyPath
        {
            get { return GetDS_InstallPath(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static string GetDE_InstallPath()
        {
            return RuntimeEnvironment.GetRuntimeDirectory();
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static string GetDS_InstallPath()
        {
            return Environment.ExpandEnvironmentVariables(Environment.Is64BitOperatingSystem
                ? @"%ProgramFiles(x86)%\Microsoft WCF Data Services\" + DS_BaseVersion + @"\bin\.NetFramework"
                : @"%ProgramFiles%\Microsoft WCF Data Services\" + DS_BaseVersion + @"\bin\.NetFramework");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static string GetDS_PortableInstallPath()
        {
            return Environment.ExpandEnvironmentVariables(Environment.Is64BitOperatingSystem
                ? @"%ProgramFiles(x86)%\Microsoft WCF Data Services\" + DS_BaseVersion + @"\bin\.NetPortable"
                : @"%ProgramFiles%\Microsoft WCF Data Services\" + DS_BaseVersion + @"\bin\.NetPortable");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static string GetDS_Tools_InstallPath()
        {
            return Environment.ExpandEnvironmentVariables(Environment.Is64BitOperatingSystem
                ? @"%ProgramFiles(x86)%\Microsoft WCF Data Services\" + DS_BaseVersion + @"\bin\tools"
                : @"%ProgramFiles%\Microsoft WCF Data Services\" + DS_BaseVersion + @"\bin\tools");
        }

        /// <summary>where to find silverlight reference assemblies</summary>
        internal const string DS_SilverlightReferenceAssemblyPath = @"%ProgramFiles%\Microsoft WCF Data Services\" + DS_BaseVersion + @"\bin\Silverlight";

        internal const string EntityDataModel = "Microsoft.OData.Edm.dll";
        internal const string DataEntity = "System.Data.Entity.dll";
        internal const string EntityFramework = "EntityFramework.dll";
        internal const string DataEntityDesign = "System.Data.Entity.Design.dll";

        internal const string DataServices = "Microsoft.OData.Service.dll";
        internal const string DataServicesClient = "Microsoft.OData.Client.dll";
        internal const string DataServicesSilverlightClient = "Microsoft.OData.Client.SL.dll";
        internal const string DataSvcUtil = "DataSvcUtil.exe";
        internal const string ODataLib = "Microsoft.OData.Core.dll";
        internal const string SpatialCore = "Microsoft.Spatial.dll";

        internal const string System = "System.dll";
        internal const string SystemCore = "System.Core.dll";
        internal const string SystemIO = "System.IO.dll";
        internal const string SystemRuntime = "System.Runtime.dll";
        internal const string SystemXml = "System.Xml.dll";
        internal const string SystemXmlReaderWriter = "System.Xml.ReaderWriter.dll";
    }

    internal const string DataFxAssemblyVersion = VersionConstants.AssemblyVersion;
    internal const string FXAssemblyVersion = "4.0.0.0";

    internal const string EcmaPublicKey = "b77a5c561934e089";
    internal const string EcmaPublicKeyToken = EcmaPublicKey;
    internal const string EcmaPublicKeyFull = "00000000000000000400000000000000";

    internal const string MicrosoftPublicKey = "b03f5f7f11d50a3a";
    internal const string MicrosoftPublicKeyToken = MicrosoftPublicKey;
    internal const string MicrosoftPublicKeyFull = "002400000480000094000000060200000024000052534131000400000100010007D1FA57C4AED9F0A32E84AA0FAEFD0DE9E8FD6AEC8F87FB03766C834C99921EB23BE79AD9D5DCC1DD9AD236132102900B723CF980957FC4E177108FC607774F29E8320E92EA05ECE4E821C0A5EFE8F1645C4C0C93C1AB99285D622CAA652C1DFAD63D745D6F2DE5F17E5EAF0FC4963D261C8A12436518206DC093344D5AD293";

    internal const string SharedLibPublicKey = "31bf3856ad364e35";
    internal const string SharedLibPublicKeyToken = SharedLibPublicKey;
    internal const string SharedLibPublicKeyFull = "0024000004800000940000000602000000240000525341310004000001000100B5FC90E7027F67871E773A8FDE8938C81DD402BA65B9201D60593E96C492651E889CC13F1415EBB53FAC1131AE0BD333C5EE6021672D9718EA31A8AEBD0DA0072F25D87DBA6FC90FFD598ED4DA35E44C398C454307E8E33B8426143DAEC9F596836F97C8F74750E5975C64E2189F45DEF46B2A2B1247ADC3652BF5C308055DA9";

    internal const string SilverlightPublicKey = "31bf3856ad364e35";
    internal const string SilverlightPublicKeyToken = SilverlightPublicKey;
    internal const string SilverlightPublicKeyFull = "0024000004800000940000000602000000240000525341310004000001000100B5FC90E7027F67871E773A8FDE8938C81DD402BA65B9201D60593E96C492651E889CC13F1415EBB53FAC1131AE0BD333C5EE6021672D9718EA31A8AEBD0DA0072F25D87DBA6FC90FFD598ED4DA35E44C398C454307E8E33B8426143DAEC9F596836F97C8F74750E5975C64E2189F45DEF46B2A2B1247ADC3652BF5C308055DA9";

    internal const string SilverlightPlatformPublicKey = "7cec85d7bea7798e";
    internal const string SilverlightPlatformPublicKeyToken = SilverlightPlatformPublicKey;
    internal const string SilverlightPlatformPublicKeyFull = "00240000048000009400000006020000002400005253413100040000010001008D56C76F9E8649383049F383C44BE0EC204181822A6C31CF5EB7EF486944D032188EA1D3920763712CCB12D75FB77E9811149E6148E5D32FBAAB37611C1878DDC19E20EF135D0CB2CFF2BFEC3D115810C3D9069638FE4BE215DBF795861920E5AB6F7DB2E2CEEF136AC23D5DD2BF031700AEC232F6C6B1C785B4305C123B37AB";

    private const string DataFxFramework = ", Version=" + DataFxAssemblyVersion + ", Culture=neutral, PublicKeyToken=" + SharedLibPublicKey;
    private const string MicrosoftFramework = ", Version=" + FXAssemblyVersion + ", Culture=neutral, PublicKeyToken=" + MicrosoftPublicKey;
    private const string NetFxFramework = ", Version=" + FXAssemblyVersion + ", Culture=neutral, PublicKeyToken=" + EcmaPublicKey;

    internal const string Mscorlib = Name.mscorlib + NetFxFramework;
    internal const string System = Name.System + NetFxFramework;
    internal const string SystemCore = Name.SystemCore + NetFxFramework;
    internal const string SystemData = Name.SystemData + NetFxFramework;
    internal const string SystemDataDataSetExtensions = Name.SystemDataDataSetExtensions + NetFxFramework;
    internal const string SystemDataEntity = Name.SystemDataEntity + NetFxFramework;
    internal const string SystemDataLinq = Name.SystemDataLinq + NetFxFramework;
    internal const string SystemDrawing = Name.SystemDrawing + NetFxFramework;
    internal const string SystemNumerics = Name.SystemNumerics + NetFxFramework;
    internal const string SystemRuntimeRemoting = Name.SystemRuntimeRemoting + NetFxFramework;
    internal const string SystemRuntimeSerialization = Name.SystemRuntimeSerialization + NetFxFramework;
    internal const string SystemServiceModel = Name.SystemServiceModel + NetFxFramework;
    internal const string SystemWeb = Name.SystemWeb + MicrosoftFramework;
    internal const string SystemWebEntity = Name.SystemWebEntity + NetFxFramework;
    internal const string SystemWebEntityDesign = Name.SystemWebEntityDesign + NetFxFramework;
    internal const string SystemXml = Name.SystemXml + NetFxFramework;
    internal const string SystemXmlLinq = Name.SystemXmlLinq + NetFxFramework;

    internal const string DataServices = Name.DataServices + DataFxFramework;
    internal const string DataServicesClient = Name.DataServicesClient + DataFxFramework;
    internal const string EntityDataModel = Name.EntityDataModel + DataFxFramework;
    internal const string OData = Name.OData + DataFxFramework;
    internal const string Spatial = Name.Spatial + DataFxFramework;
}
