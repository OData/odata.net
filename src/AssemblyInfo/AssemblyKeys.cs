//---------------------------------------------------------------------
// <copyright file="AssemblyKeys.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

/// <summary>
/// Sets public key string for friend assemblies.
/// </summary>
internal static class AssemblyRef
{
#if DelaySignKeys
    /// <summary>ProductPublicKey is an official MS supported public key for external releases.</summary>
    internal const string ProductPublicKey = ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100B5FC90E7027F67871E773A8FDE8938C81DD402BA65B9201D60593E96C492651E889CC13F1415EBB53FAC1131AE0BD333C5EE6021672D9718EA31A8AEBD0DA0072F25D87DBA6FC90FFD598ED4DA35E44C398C454307E8E33B8426143DAEC9F596836F97C8F74750E5975C64E2189F45DEF46B2A2B1247ADC3652BF5C308055DA9";

    /// <summary>TestPublicKey is an unsupported strong key for testing purpose only.</summary>
    internal const string TestPublicKey = ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100197c25d0a04f73cb271e8181dba1c0c713df8deebb25864541a66670500f34896d280484b45fe1ff6c29f2ee7aa175d8bcbd0c83cc23901a894a86996030f6292ce6eda6e6f3e6c74b3c5a3ded4903c951e6747e6102969503360f7781bf8bf015058eb89b7621798ccc85aaca036ff1bc1556bb7f62de15908484886aa8bbae";

    /// <summary>Dont know what this is</summary>
    internal const string ProductPublicKeyToken = "31bf3856ad364e35";

#elif TestSignKeys
    /// <summary>Dont know what this is</summary>
    internal const string TestPublicKey = ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100197c25d0a04f73cb271e8181dba1c0c713df8deebb25864541a66670500f34896d280484b45fe1ff6c29f2ee7aa175d8bcbd0c83cc23901a894a86996030f6292ce6eda6e6f3e6c74b3c5a3ded4903c951e6747e6102969503360f7781bf8bf015058eb89b7621798ccc85aaca036ff1bc1556bb7f62de15908484886aa8bbae";

    /// <summary>Dont know what this is</summary>
    internal const string ProductPublicKey = TestPublicKey;

    /// <summary>Dont know what this is</summary>
    internal const string ProductPublicKeyToken = "69c3241e6f0468ca";

#else
    /// <summary>No signing</summary>
    internal const string ProductPublicKey = "";

    /// <summary>No signing</summary>
    internal const string TestPublicKey = "";

    /// <summary>No signing</summary>
    internal const string ProductPublicKeyToken = "";

#endif
}
