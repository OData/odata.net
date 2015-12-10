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

[assembly: InternalsVisibleTo("System.Runtime.Serialization.OData" + AssemblyRef.ProductPublicKey)]
[assembly: InternalsVisibleTo("Microsoft.OData.Query" + AssemblyRef.ProductPublicKey)]

#else
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AstoriaUnitTests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AstoriaUnitTests.TDDUnitTests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.Client.TDDUnitTests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.Test.OData.TDD.Tests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.Test.OData.User.Tests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.Test.OData.Query.TDD.Tests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.Test.Taupo.OData.Scenario.Tests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.Test.Taupo.OData.Query.Tests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.Test.Taupo.OData.WCFService" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.Test.Taupo.OData.Reader.Tests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.Test.Taupo.OData.Writer.Tests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.Test.Taupo.OData" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.Query" + AssemblyRef.ProductPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.Core.Tests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.Client.Tests" + AssemblyRef.TestPublicKey)]
#endif
#pragma warning restore 436
