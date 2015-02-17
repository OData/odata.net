//---------------------------------------------------------------------
// <copyright file="InternalsVisibleTo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#pragma warning disable 436
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AstoriaUnitTests.TDDUnitTests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.Service.Design.UnitTests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.Client.Design.T4.UnitTests" + AssemblyRef.TestPublicKey)]
#pragma warning restore 436