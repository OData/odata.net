//---------------------------------------------------------------------
// <copyright file="InternalsVisibleTo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.Service.Test.Common")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.Service.Test.Client.TDDUnitTests")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AstoriaUnitTests.TDDUnitTests")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.Spatial.TDDUnitTests")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.Spatial.Tests")]

// .NET Core
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.TestCommon.NetCore")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.Edm.Tests.NetCore")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.Core.Tests.NetCore")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.Spatial.Tests.NetCore")]