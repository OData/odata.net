//---------------------------------------------------------------------
// <copyright file="ODataResourceWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData;

namespace Microsoft.Test.OData
{
    public class ODataResourceWrapper : ODataItemWrapper
    {
        public ODataResource Resource { get; set; }
        public List<ODataNestedResourceInfoWrapper> NestedResourceInfos { get; set; }
    }
}