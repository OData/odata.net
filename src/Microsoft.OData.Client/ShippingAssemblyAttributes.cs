//---------------------------------------------------------------------
// <copyright file="ShippingAssemblyAttributes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#pragma warning disable 436
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.Service.Test.Common" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.Service.Test.Client.TDDUnitTests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.Client.TDDUnitTests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AstoriaUnitTests.TDDUnitTests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AstoriaUnitTests.ClientExtensions" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.Data.ServerUnitTests1.UnitTests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.Data.ServerUnitTests2.UnitTests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("RegressionUnitTests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AstoriaUnitTests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AstoriaClientUnitTests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.Test.Data.Services.DDBasics" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AstoriaUnitTests.ClientCSharp" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.Client.Tests" + AssemblyRef.TestPublicKey)]
#pragma warning restore 436

#if PORTABLELIB
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DSClient.Net45.Delta.UnitTests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DSClient.Win8Store.Delta.UnitTests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DSClient.Win8Phone.Delta.UnitTests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DSClient.SL.Delta.UnitTests" + AssemblyRef.TestPublicKey)]
#endif
