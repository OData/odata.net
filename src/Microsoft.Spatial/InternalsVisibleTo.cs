//---------------------------------------------------------------------
// <copyright file="InternalsVisibleTo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.Service.Test.Common" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.Service.Test.Client.TDDUnitTests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AstoriaUnitTests.TDDUnitTests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.Spatial.TDDUnitTests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.Spatial.Tests" + AssemblyRef.TestPublicKey)]

// .NET Core
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.TestCommon.NetCore" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.Edm.Tests.NetCore" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.Core.Tests.NetCore" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.Spatial.Tests.NetCore" + AssemblyRef.TestPublicKey)]