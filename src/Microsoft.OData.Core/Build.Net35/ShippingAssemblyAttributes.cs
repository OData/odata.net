//---------------------------------------------------------------------
// <copyright file="ShippingAssemblyAttributes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#pragma warning disable 436
#if INTERNAL_DROP
#region Namespaces
using System.Runtime.CompilerServices;
#endregion Namespaces

#else
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.Query.TDD.Tests.NetFX35" + AssemblyRef.TestPublicKey)]
#endif
#pragma warning restore 436
