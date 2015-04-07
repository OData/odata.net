'---------------------------------------------------------------------
' <copyright file="AssemblyRefs.vb" company="Microsoft">
'      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
' </copyright>
'---------------------------------------------------------------------

Friend Class DataFxAssemblyRef

    Friend Class Name

        Friend Shared mscorlib As String = "mscorlib"
        Friend Shared System As String = "System"
        Friend Shared SystemCore As String = "System.Core"
        Friend Shared SystemData As String = "System.Data"
        Friend Shared SystemDataDataSetExtensions As String = "System.Data.DataSetExtensions"
        Friend Shared SystemDataEntity As String = "System.Data.Entity"
        Friend Shared SystemNumerics As String = "System.Numerics"
        Friend Shared SystemRuntimeRemoting As String = "System.Runtime.Remoting"
        Friend Shared SystemRuntimeSerialization As String = "System.Runtime.Serialization"
        Friend Shared SystemServiceModel As String = "System.ServiceModel"
        Friend Shared SystemXml As String = "System.Xml"
        Friend Shared SystemXmlLinq As String = "System.Xml.Linq"

        Friend Shared DataEntity As String = "System.Data.Entity"
        Friend Shared DataEntityDesign As String = "System.Data.Entity.Design"
        Friend Shared DataServices As String = "Microsoft.OData.Service"
        Friend Shared DataServicesClient As String = "Microsoft.OData.Client"
        Friend Shared DataServicesDesign As String = "Microsoft.OData.Service.Design"
        Friend Shared DataSvcUtil As String = "DataSvcUtil"
    End Class

    Friend Class File
        Friend Shared DotNetFrameworkV4_ClientReferenceAssemblyPath As String = "%ProgramFiles%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\Profile\Client"
        Friend Shared DotNetFrameworkV4_ReferenceAssemblyPath As String = "%ProgramFiles%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0"

        Friend Shared SilverlightV5_ReferenceAssemblyPath As String = "%ProgramFiles%\Microsoft Silverlight\5.1.20913.0"
        Friend Shared SilverlightV5_SdkClientReferenceAssemblyPath As String = "%ProgramFiles%\Microsoft SDKs\Silverlight\v5.0\Libraries\Client"
        Friend Shared SilverlightV5_SdkServerReferenceAssemblyPath As String = "%ProgramFiles%\Microsoft SDKs\Silverlight\v5.0\Libraries\Server"
        Friend Shared DS_BaseVersion As String = VersionConstants.ReleaseVersion
        Friend Shared EF_ReferenceAssemblyPath As String = "%ProgramFiles%\Microsoft Entity Framework June 2011 CTP\bin\.NetFramework"
        Friend Shared DS_ReferenceAssemblyPath As String = "%ProgramFiles%\Microsoft WCF Data Services\" + DS_BaseVersion + "\bin\.NetFramework"
        Friend Shared DS_SilverlightReferenceAssemblyPath As String = "%ProgramFiles%\Microsoft WCF Data Services\" + DS_BaseVersion + "\bin\Silverlight"
        Friend Shared DS_PortableInstallPath As String = "%ProgramFiles%\Microsoft WCF Data Services\" + DS_BaseVersion + "\bin\.NetPortable"

        Friend Shared DataEntity As String = "System.Data.Entity.dll"
        Friend Shared DataEntityDesign As String = "System.Data.Entity.Design.dll"

        Friend Shared DataServices As String = "Microsoft.OData.Service.dll"
        Friend Shared DataServicesClient As String = "Microsoft.OData.Client.dll"
        Friend Shared ODataLib As String = "Microsoft.OData.Core.dll"
        Friend Shared EdmLib As String = "Microsoft.OData.Edm.dll"

        Friend Shared DataServicesSilverlightClient As String = "Microsoft.OData.Client.SL.dll"

        Friend Shared DataServicesDesign As String = "Microsoft.OData.Service.Design.dll"
        Friend Shared DataSvcUtil As String = "DataSvcUtil.exe"


    End Class

    Friend Shared DataFxAssemblyVersion As String = VersionConstants.AssemblyVersion
    Friend Shared FXAssemblyVersion As String = "4.0.0.0"

    Friend Shared EcmaPublicKey As String = "b77a5c561934e089"
    Friend Shared EcmaPublicKeyToken As String = EcmaPublicKey
    Friend Shared EcmaPublicKeyFull As String = "00000000000000000400000000000000"

    Friend Shared MicrosoftPublicKey As String = "b03f5f7f11d50a3a"
    Friend Shared MicrosoftPublicKeyToken As String = MicrosoftPublicKey
    Friend Shared MicrosoftPublicKeyFull As String = "002400000480000094000000060200000024000052534131000400000100010007D1FA57C4AED9F0A32E84AA0FAEFD0DE9E8FD6AEC8F87FB03766C834C99921EB23BE79AD9D5DCC1DD9AD236132102900B723CF980957FC4E177108FC607774F29E8320E92EA05ECE4E821C0A5EFE8F1645C4C0C93C1AB99285D622CAA652C1DFAD63D745D6F2DE5F17E5EAF0FC4963D261C8A12436518206DC093344D5AD293"

    Friend Shared SharedLibPublicKey As String = "31bf3856ad364e35"
    Friend Shared SharedLibPublicKeyToken As String = SharedLibPublicKey
    Friend Shared SharedLibPublicKeyFull As String = "0024000004800000940000000602000000240000525341310004000001000100B5FC90E7027F67871E773A8FDE8938C81DD402BA65B9201D60593E96C492651E889CC13F1415EBB53FAC1131AE0BD333C5EE6021672D9718EA31A8AEBD0DA0072F25D87DBA6FC90FFD598ED4DA35E44C398C454307E8E33B8426143DAEC9F596836F97C8F74750E5975C64E2189F45DEF46B2A2B1247ADC3652BF5C308055DA9"

    Friend Shared SilverlightPublicKey As String = "31bf3856ad364e35"
    Friend Shared SilverlightPublicKeyToken As String = SilverlightPublicKey
    Friend Shared SilverlightPublicKeyFull As String = "0024000004800000940000000602000000240000525341310004000001000100B5FC90E7027F67871E773A8FDE8938C81DD402BA65B9201D60593E96C492651E889CC13F1415EBB53FAC1131AE0BD333C5EE6021672D9718EA31A8AEBD0DA0072F25D87DBA6FC90FFD598ED4DA35E44C398C454307E8E33B8426143DAEC9F596836F97C8F74750E5975C64E2189F45DEF46B2A2B1247ADC3652BF5C308055DA9"

    Friend Shared SilverlightPlatformPublicKey As String = "7cec85d7bea7798e"
    Friend Shared SilverlightPlatformPublicKeyToken As String = SilverlightPlatformPublicKey
    Friend Shared SilverlightPlatformPublicKeyFull As String = "00240000048000009400000006020000002400005253413100040000010001008D56C76F9E8649383049F383C44BE0EC204181822A6C31CF5EB7EF486944D032188EA1D3920763712CCB12D75FB77E9811149E6148E5D32FBAAB37611C1878DDC19E20EF135D0CB2CFF2BFEC3D115810C3D9069638FE4BE215DBF795861920E5AB6F7DB2E2CEEF136AC23D5DD2BF031700AEC232F6C6B1C785B4305C123B37AB"

    Private Shared DataFxFramework As String = ", Version=" + DataFxAssemblyVersion + ", Culture=neutral, PublicKeyToken=" + SharedLibPublicKey
    Private Shared NetFxFramework As String = ", Version=" + FXAssemblyVersion + ", Culture=neutral, PublicKeyToken=" + EcmaPublicKey

    Friend Shared Mscorlib As String = Name.mscorlib + NetFxFramework
    Friend Shared System As String = Name.System + NetFxFramework
    Friend Shared SystemCore As String = Name.SystemCore + NetFxFramework
    Friend Shared SystemData As String = Name.SystemData + NetFxFramework
    Friend Shared SystemDataDatasetExtensions As String = Name.SystemDataDataSetExtensions + NetFxFramework
    Friend Shared SystemDataEntity As String = Name.SystemDataEntity + NetFxFramework
    Friend Shared SystemNumerics As String = Name.SystemNumerics + NetFxFramework
    Friend Shared SystemRuntimeRemoting As String = Name.SystemRuntimeRemoting + NetFxFramework
    Friend Shared SystemRuntimeSerialization As String = Name.SystemRuntimeSerialization + NetFxFramework
    Friend Shared SystemServiceModel As String = Name.SystemServiceModel + NetFxFramework
    Friend Shared SystemXml As String = Name.SystemXml + NetFxFramework
    Friend Shared SystemXmlLinq As String = Name.SystemXmlLinq + NetFxFramework

    Friend Shared DataServices As String = Name.DataServices + DataFxFramework
    Friend Shared DataServicesClient As String = Name.DataServicesClient + DataFxFramework
    Friend Shared DataServicesDesign As String = Name.DataServicesDesign + DataFxFramework
End Class