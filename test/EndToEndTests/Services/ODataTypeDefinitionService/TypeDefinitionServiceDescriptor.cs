//---------------------------------------------------------------------
// <copyright file="TypeDefinitionServiceDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using Microsoft.Test.OData.Services.ODataWCFService.Services;
    using System;
    using System.ComponentModel.Composition;

    [Export(typeof(IODataServiceDescriptor))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TypeDefinitionServiceDescriptor : IODataServiceDescriptor
    {
        public Type ServiceType
        {
            get { return typeof(TypeDefinitionService); }
        }

        public string ServiceName
        {
            get { return "TypeDefinitionService"; }
        }
    }
}
