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
[assembly: InternalsVisibleTo("Microsoft.OData.Edm.E2E.Tests, PublicKey = 0024000004800000940000000602000000240000525341310004000001000100c1b376caa4bac2fba60ec13e0eb61b0a71b3dffb0903ccd5d7cf114c1145a17f1e9dfb3c2636e4990312a80b36f5605bf37a4c53a833568f2da628f2b738a882e7e91bc11b629ed2a94b0c522afa971bf607d2c53f3c6c6dd9e1bd20e4d41b07f776d3638823dab3fe36ea3f01259fc03f8ec5153d6391fd23665f0d1dd129b6")]
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

#if !ASSEMBLY_ATTRIBUTE_ON_NETSTANDARD_11
#if !ASSEMBLY_ATTRIBUTE_ON_NETCORE_10
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
#endif
#endif

[assembly: NeutralResourcesLanguageAttribute("en-US")]
