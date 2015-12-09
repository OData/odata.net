//---------------------------------------------------------------------
// <copyright file="ShippingAssemblyAttributes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

// PublicKey=MicrosoftPublicKeyFull
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.Core.Tests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.Edm.Tests" + AssemblyRef.TestPublicKey)]
#pragma warning disable 436
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("EdmLibTests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("EdmLibTests.SL" + AssemblyRef.TestPublicKey)]
#pragma warning restore 436