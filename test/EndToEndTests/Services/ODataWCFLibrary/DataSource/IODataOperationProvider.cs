//---------------------------------------------------------------------
// <copyright file="IODataOperationProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.DataSource
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IODataOperationProvider
    {
        QueryContext QueryContext { set; }
    }
}
