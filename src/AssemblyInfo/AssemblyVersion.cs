//---------------------------------------------------------------------
// <copyright file="AssemblyVersion.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Resources;

// The following assembly information is common to all product assemblies.
// If you get compiler errors CS0579, "Duplicate '<attributename>' attribute", check your 
// Properties\AssemblyInfo.cs file and remove any lines duplicating the ones below.
[assembly: AssemblyFileVersion("0.0.0.0")]
[assembly: AssemblyInformationalVersion("0.0.0.0")]

#if ASSEMBLY_ATTRIBUTE_NO_BUILD_NUM_IN_VERSION
[assembly: AssemblyVersion("0.0.0.0")]
[assembly: SatelliteContractVersion("0.0.0.0")]
#else
[assembly: AssemblyVersion("0.0.0")]
[assembly: SatelliteContractVersion("0.0.0")]
#endif
