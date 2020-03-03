//---------------------------------------------------------------------
// <copyright file="ODataNestedResourceInfoWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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
    internal class ODataNestedResourceInfoWrapper : ODataItemWrapper
#elif ODATA_CLIENT
    internal class ODataNestedResourceInfoWrapper : ODataItemWrapper
#else
    public class ODataNestedResourceInfoWrapper : ODataItemWrapper
#endif
    {
        public ODataNestedResourceInfo NestedResourceInfo { get; set; }

        public ODataItemWrapper NestedResourceOrResourceSet { get; set; }

        public override ODataItem Item
        {
            get { return this.NestedResourceInfo; }
        }
    }
}