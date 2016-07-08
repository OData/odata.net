//---------------------------------------------------------------------
// <copyright file="ODataResourceWrapper.cs" company="Microsoft">
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
    internal class ODataResourceWrapper : ODataItemWrapper
#elif ODATA_CLIENT
    internal class ODataResourceWrapper : ODataItemWrapper
#else
    public class ODataResourceWrapper : ODataItemWrapper
#endif
    {
        public ODataResource Resource { get; set; }

        public IEnumerable<ODataNestedResourceInfoWrapper> NestedResourceInfoWrappers { get; set; }

        public override ODataItem Item
        {
            get { return Resource; }
        }

        public object Instance { get; set; }
    }
}