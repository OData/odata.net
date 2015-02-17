//---------------------------------------------------------------------
// <copyright file="AssemblyAttributes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable 436
[assembly: InternalsVisibleTo("Microsoft.Test.Taupo.Query.Tests" + AssemblyRef.TestPublicKey)]
[assembly: InternalsVisibleTo("Microsoft.Test.Taupo.EntityFramework.Tests" + AssemblyRef.TestPublicKey)]
[assembly: InternalsVisibleTo("Microsoft.Test.Taupo.EntityFramework.4.0.Tests" + AssemblyRef.TestPublicKey)]
#pragma warning restore 436
