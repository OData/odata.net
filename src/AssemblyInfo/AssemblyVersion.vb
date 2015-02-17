'---------------------------------------------------------------------
' <copyright file="AssemblyVersion.vb" company="Microsoft">
'      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
' </copyright>
'---------------------------------------------------------------------

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Resources

' The following assembly information is common to all product assemblies.
' If you get compiler errors CS0579, "Duplicate '<attributename>' attribute", check your 
' Properties\AssemblyInfo.vb file and remove any lines duplicating the ones below.
<Assembly: AssemblyFileVersion("0.0.0.0")> 

#If ASSEMBLY_ATTRIBUTE_NO_BUILD_NUM_IN_VERSION Then
<Assembly: AssemblyVersion("0.0.0.0")> 
<assembly: SatelliteContractVersion("0.0.0.0")>
#Else
<Assembly: AssemblyVersion("0.0.0")> 
<Assembly: SatelliteContractVersion("0.0.0")> 
#End If
