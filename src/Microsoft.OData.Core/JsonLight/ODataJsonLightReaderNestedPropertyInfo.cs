//---------------------------------------------------------------------
// <copyright file="ODataJsonLightReaderNestedPropertyInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using Microsoft.OData.Edm;
    #endregion Namespaces

    internal class ODataJsonLightReaderNestedPropertyInfo : ODataJsonLightReaderNestedInfo
    {
        internal ODataJsonLightReaderNestedPropertyInfo(ODataPropertyInfo nestedPropertyInfo, IEdmProperty nestedProperty) : base(nestedProperty)
        {
            this.NestedPropertyInfo = nestedPropertyInfo;
        }

        internal ODataPropertyInfo NestedPropertyInfo { get; set; }
    }
}