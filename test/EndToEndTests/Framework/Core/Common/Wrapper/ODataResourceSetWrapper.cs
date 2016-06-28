//---------------------------------------------------------------------
// <copyright file="ODataResourceSetWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData;

namespace Microsoft.Test.OData
{
    public class ODataResourceSetWrapper : ODataItemWrapper
    {
        public ODataResourceSet ResourceSet { get; set; }
        public List<ODataResourceWrapper> Resources { get; set; }
    }
}