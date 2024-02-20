//---------------------------------------------------------------------
// <copyright file="AssemblyInfoCommon.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;

[assembly: AssemblyCompany("Microsoft Corporation")]
// If you want to control this metadata globally but not with the VersionProductName property, hard-code the value below.
// If you want to control this metadata at the individual project level with AssemblyInfo.cs, comment-out the line below.
// If you leave the line below unchanged, make sure to set the property in the root build.props, e.g.: <VersionProductName Condition="'$(VersionProductName)'==''">Your Product Name</VersionProductName>
// [assembly: AssemblyProduct("%VersionProductName%")]
[assembly: AssemblyCopyright("Copyright (c) Microsoft Corporation. All rights reserved.")]
[assembly: AssemblyTrademark("Microsoft and Windows are either registered trademarks or trademarks of Microsoft Corporation in the U.S. and/or other countries.")]
[assembly: AssemblyCulture("")]
[assembly: InternalsVisibleTo("Microsoft.Test.Taupo.Astoria, PublicKey = 0024000004800000940000000602000000240000525341310004000001000100197c25d0a04f73cb271e8181dba1c0c713df8deebb25864541a66670500f34896d280484b45fe1ff6c29f2ee7aa175d8bcbd0c83cc23901a894a86996030f6292ce6eda6e6f3e6c74b3c5a3ded4903c951e6747e6102969503360f7781bf8bf015058eb89b7621798ccc85aaca036ff1bc1556bb7f62de15908484886aa8bbae")]
#if (DEBUG || _DEBUG)
[assembly: AssemblyConfiguration("Debug")]
#endif

#if ASSEMBLY_ATTRIBUTE_PRODUCT_VS
[assembly: AssemblyProduct("Microsoft (R) Visual Studio (R) 2010")]
#else
[assembly: AssemblyProduct("Microsoft® .NET Framework")]
#endif

#if ASSEMBLY_ATTRIBUTE_CLS_COMPLIANT
[assembly: CLSCompliant(true)]
#else
[assembly: CLSCompliant(false)]
#endif

#if ASSEMBLY_ATTRIBUTE_COM_VISIBLE
[assembly: ComVisible(true)]
#else
[assembly: ComVisible(false)]
#endif

#if ASSEMBLY_ATTRIBUTE_COM_COMPATIBLE_SIDEBYSIDE
[assembly:ComCompatibleVersion(1,0,3300,0)]
#endif

#if ASSEMBLY_ATTRIBUTE_ALLOW_PARTIALLY_TRUSTED_CALLERS
[assembly: AllowPartiallyTrustedCallers]
#else
#if ASSEMBLY_ATTRIBUTE_CONDITIONAL_APTCA_L2
[assembly:AllowPartiallyTrustedCallers(PartialTrustVisibilityLevel=PartialTrustVisibilityLevel.NotVisibleByDefault)]
#endif
#endif

#if ASSEMBLY_ATTRIBUTE_TRANSPARENT_ASSEMBLY
[assembly: SecurityTransparent]
#endif

#if !SUPPRESS_SECURITY_RULES
#if SECURITY_MIGRATION && !ASSEMBLY_ATTRIBUTE_CONDITIONAL_APTCA_L2
#if ASSEMBLY_ATTRIBUTE_SKIP_VERIFICATION_IN_FULLTRUST
[assembly: SecurityRules(SecurityRuleSet.Level1, SkipVerificationInFullTrust = true)]
#else
[assembly: SecurityRules(SecurityRuleSet.Level1)]
#endif
#else
#if ASSEMBLY_ATTRIBUTE_SKIP_VERIFICATION_IN_FULLTRUST
[assembly: SecurityRules(SecurityRuleSet.Level2, SkipVerificationInFullTrust = true)]
#else
[assembly: SecurityRules(SecurityRuleSet.Level2)]
#endif
#endif
#endif

[assembly: NeutralResourcesLanguageAttribute("en-US")]
