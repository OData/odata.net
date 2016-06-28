//---------------------------------------------------------------------
// <copyright file="ODataNestedResourceInfoWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData;

namespace Microsoft.Test.OData
{
    public class ODataNestedResourceInfoWrapper : ODataItemWrapper
    {
        public ODataNestedResourceInfo NestedResourceInfo { get; set; }
        public ODataItemWrapper NestedResourceOrResourceSet { get; set; }
    }
}