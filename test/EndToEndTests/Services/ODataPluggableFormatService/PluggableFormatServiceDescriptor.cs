//---------------------------------------------------------------------
// <copyright file="PluggableFormatServiceDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PluggableFormat
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.Test.OData.Services.ODataWCFService.Services;

    [Export(typeof(IODataServiceDescriptor))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class PluggableFormatServiceDescriptor : IODataServiceDescriptor
    {
        public Type ServiceType
        {
            get { return typeof(PluggableFormatService); }
        }

        public string ServiceName
        {
            get { return "PluggableFormatService"; }
        }
    }
}
