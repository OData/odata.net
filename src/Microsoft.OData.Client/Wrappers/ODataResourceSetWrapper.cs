//---------------------------------------------------------------------
// <copyright file="ODataResourceSetWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData;

#if ODATA_SERVICE
namespace Microsoft.OData.Service
#elif ODATA_CLIENT
namespace Microsoft.OData.Client
#else
namespace Microsoft.Test.OData
#endif
{
#if ODATA_SERVICE
    internal class ODataResourceSetWrapper : ODataItemWrapper
#elif ODATA_CLIENT
    internal class ODataResourceSetWrapper : ODataItemWrapper
#else
    public class ODataResourceSetWrapper : ODataItemWrapper
#endif
    {
        public ODataResourceSet ResourceSet { get; set; }

        public IEnumerable<ODataResourceWrapper> Resources { get; set; }

        public override ODataItem Item
        {
            get { return this.ResourceSet; }
        }
    }
}