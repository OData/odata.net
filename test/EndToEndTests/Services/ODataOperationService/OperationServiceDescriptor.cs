//---------------------------------------------------------------------
// <copyright file="OperationServiceDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.ComponentModel.Composition;
using Microsoft.Test.OData.Services.ODataWCFService.Services;

namespace Microsoft.Test.OData.Services.ODataOperationService
{
    [Export(typeof(IODataServiceDescriptor))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class OperationServiceDescriptor : IODataServiceDescriptor
    {
        public Type ServiceType
        {
            get { return typeof(OperationService); }
        }

        public string ServiceName
        {
            get { return "OperationService"; }
        }
    }
}
