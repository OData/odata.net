//---------------------------------------------------------------------
// <copyright file="IODataServiceDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IODataServiceDescriptor
    {
        Type ServiceType { get; }
        string ServiceName { get; }
    }
}
