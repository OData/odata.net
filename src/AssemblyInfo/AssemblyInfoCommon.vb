'---------------------------------------------------------------------
' <copyright file="AssemblyInfoCommon.vb" company="Microsoft">
'      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
' </copyright>
'---------------------------------------------------------------------

Imports System
Imports System.Reflection
Imports System.Resources
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Security

' The following assembly information is common to all product assemblies.
' If you get compiler errors CS0579, "Duplicate '<attributename>' attribute", check your 
' Properties\AssemblyInfo.vb file and remove any lines duplicating the ones below.
' (See also AssemblyVersion.vb in this same directory.)
<Assembly: AssemblyCompany("Microsoft Corporation")>
' If you want to control this metadata globally but not with the VersionProductName property, hard-code the value below.
' If you want to control this metadata at the individual project level with AssemblyInfo.cs, comment-out the line below.
' If you leave the line below unchanged, make sure to set the property in the root build.props, e.g.: <VersionProductName Condition="'$(VersionProductName)'==''">Your Product Name</VersionProductName>
' <Assembly: AssemblyProduct("%VersionProductName%")> 
<Assembly: AssemblyCopyright("Copyright (c) Microsoft Corporation. All rights reserved.")> 
<Assembly: AssemblyTrademark("Microsoft and Windows are either registered trademarks or trademarks of Microsoft Corporation in the U.S. and/or other countries.")> 
<Assembly: AssemblyCulture("")> 
#If (DEBUG OrElse _DEBUG) Then
<Assembly: AssemblyConfiguration("Debug")>
#End If

#If ASSEMBLY_ATTRIBUTE_PRODUCT_VS
<Assembly: AssemblyProduct("Microsoft (R) Visual Studio (R) 2010")>
#Else
<Assembly: AssemblyProduct("Microsoft� .NET Framework")>
#End If

#If ASSEMBLY_ATTRIBUTE_CLS_COMPLIANT
<assembly: CLSCompliant(True)>
#Else
<assembly: CLSCompliant(False)>
#End If

#If ASSEMBLY_ATTRIBUTE_COM_VISIBLE
<Assembly: ComVisible(True)>
#Else
<Assembly: ComVisible(False)>
#End If

#If ASSEMBLY_ATTRIBUTE_COM_COMPATIBLE_SIDEBYSIDE
<assembly: ComCompatibleVersion(1,0,3300,0)>
#End If

#If ASSEMBLY_ATTRIBUTE_ALLOW_PARTIALLY_TRUSTED_CALLERS
<Assembly: AllowPartiallyTrustedCallers>
#Else
#If ASSEMBLY_ATTRIBUTE_CONDITIONAL_APTCA_L2
<assembly: AllowPartiallyTrustedCallers(PartialTrustVisibilityLevel=PartialTrustVisibilityLevel.NotVisibleByDefault)>
#End If
#End If

#If ASSEMBLY_ATTRIBUTE_TRANSPARENT_ASSEMBLY
<Assembly: SecurityTransparent>
#End If

#If NOT SUPPRESS_SECURITY_RULES
#If SECURITY_MIGRATION AND NOT ASSEMBLY_ATTRIBUTE_CONDITIONAL_APTCA_L2
#If ASSEMBLY_ATTRIBUTE_SKIP_VERIFICATION_IN_FULLTRUST
<Assembly: SecurityRules(SecurityRuleSet.Level1, SkipVerificationInFullTrust:=True)>
#Else
<Assembly: SecurityRules(SecurityRuleSet.Level1)>
#End If
#Else
#If ASSEMBLY_ATTRIBUTE_SKIP_VERIFICATION_IN_FULLTRUST
<Assembly: SecurityRules(SecurityRuleSet.Level2, SkipVerificationInFullTrust:=True)>
#Else
<Assembly: SecurityRules(SecurityRuleSet.Level2)>
#End If
#End If
#End If

<assembly:NeutralResourcesLanguageAttribute("en-US")>

''' <summary>
''' Sets public key string for friend assemblies.
''' </summary>
Friend Module AssemblyRef

#If DelaySignKeys Then
   Friend Const ProductPublicKey As String = ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100B5FC90E7027F67871E773A8FDE8938C81DD402BA65B9201D60593E96C492651E889CC13F1415EBB53FAC1131AE0BD333C5EE6021672D9718EA31A8AEBD0DA0072F25D87DBA6FC90FFD598ED4DA35E44C398C454307E8E33B8426143DAEC9F596836F97C8F74750E5975C64E2189F45DEF46B2A2B1247ADC3652BF5C308055DA9"
   Friend Const TestPublicKey As String = ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100197c25d0a04f73cb271e8181dba1c0c713df8deebb25864541a66670500f34896d280484b45fe1ff6c29f2ee7aa175d8bcbd0c83cc23901a894a86996030f6292ce6eda6e6f3e6c74b3c5a3ded4903c951e6747e6102969503360f7781bf8bf015058eb89b7621798ccc85aaca036ff1bc1556bb7f62de15908484886aa8bbae"
   Friend Const ProductPublicKeyToken As String = "b03f5f7f11d50a3a"
#ElseIf TestSignKeys Then
   Friend Const TestPublicKey As String = ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100197c25d0a04f73cb271e8181dba1c0c713df8deebb25864541a66670500f34896d280484b45fe1ff6c29f2ee7aa175d8bcbd0c83cc23901a894a86996030f6292ce6eda6e6f3e6c74b3c5a3ded4903c951e6747e6102969503360f7781bf8bf015058eb89b7621798ccc85aaca036ff1bc1556bb7f62de15908484886aa8bbae"
   Friend Const ProductPublicKey As String = TestPublicKey
   Friend Const ProductPublicKeyToken As String = "69c3241e6f0468ca"
#Else
    '''<summary>No Signing</summary>
    Friend Const ProductPublicKey As String = ""
    Friend Const TestPublicKey As String = ""
    Friend Const ProductPublicKeyToken As String = ""
#End If
End Module
