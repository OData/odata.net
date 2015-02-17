//---------------------------------------------------------------------
// <copyright file="IODataDataSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.DataSource
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Edm;

    public interface IODataDataSource
    {
        IODataQueryProvider QueryProvider { get; }
        IODataUpdateProvider UpdateProvider { get; }
        IODataOperationProvider OperationProvider { get; }
        IODataStreamProvider StreamProvider { get; }
        IEdmModel Model { get; }

        void Reset();
        void Initialize();
    }
}
