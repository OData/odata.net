//-----------------------------------------------------------------------------
// <copyright file="ODataResourceSetWrapper.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.OData.Core.E2E.Tests
{
    public class ODataResourceSetWrapper : ODataItemWrapper
    {
        public ODataResourceSet ResourceSet { get; set; }

        public IEnumerable<ODataResourceWrapper> Resources { get; set; }

        public override ODataItem Item
        {
            get { return ResourceSet; }
        }
    }
}
