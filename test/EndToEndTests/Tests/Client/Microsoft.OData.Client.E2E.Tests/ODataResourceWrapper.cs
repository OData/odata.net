//-----------------------------------------------------------------------------
// <copyright file="ODataResourceWrapper.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.OData.Client.E2E.Tests
{
    public class ODataResourceWrapper : ODataItemWrapper
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
