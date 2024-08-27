//-----------------------------------------------------------------------------
// <copyright file="ODataNestedResourceInfoWrapper.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.OData.Client.E2E.Tests
{
    public class ODataNestedResourceInfoWrapper : ODataItemWrapper
    {
        public ODataNestedResourceInfo NestedResourceInfo { get; set; }

        public ODataItemWrapper NestedResourceOrResourceSet { get; set; }

        public override ODataItem Item
        {
            get { return this.NestedResourceInfo; }
        }
    }
}
