//---------------------------------------------------------------------
// <copyright file="IODataDataSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.DataSource
{
    using System;
    using Microsoft.OData.Edm;

    public interface IODataDataSource
    {
        IODataQueryProvider QueryProvider { get; }
        IODataUpdateProvider UpdateProvider { get; }
        IODataOperationProvider OperationProvider { get; }
        IODataStreamProvider StreamProvider { get; }
        IEdmModel Model { get; }
        IServiceProvider Container { get; }

        void Reset();
        void Initialize();
    }
}
