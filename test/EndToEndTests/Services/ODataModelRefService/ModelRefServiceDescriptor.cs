//---------------------------------------------------------------------
// <copyright file="ModelRefServiceDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.SampleService.Models.ModelRefDemo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.OData.Services.ODataWCFService;
    using Microsoft.Test.OData.Services.ODataWCFService.Services;

    [Export(typeof(IODataServiceDescriptor))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ModelRefServiceDescriptor : IODataServiceDescriptor
    {
        public Type ServiceType
        {
            get { return typeof(ModelRefService); }
        }

        public string ServiceName
        {
            get { return "ModelRefService"; }
        }
    }
}
