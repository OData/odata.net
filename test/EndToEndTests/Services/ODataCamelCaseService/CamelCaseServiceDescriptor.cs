//---------------------------------------------------------------------
// <copyright file="CamelCaseServiceDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.OData.Services.ODataWCFService.Services;

    [Export(typeof(IODataServiceDescriptor))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class CamelCaseServiceDescriptor : IODataServiceDescriptor
    {
        public Type ServiceType
        {
            get { return typeof(CamelCaseService); }
        }

        public string ServiceName
        {
            get { return "CamelCaseService"; }
        }
    }
}
