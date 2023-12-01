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
        internal ODataJsonLightReaderNestedPropertyInfo(ODataPropertyInfo nestedPropertyInfo, IEdmProperty nestedProperty, bool withValue = true) : base(nestedProperty)
        {
            NestedPropertyInfo = nestedPropertyInfo;
            WithValue = withValue;
        }

        internal ODataPropertyInfo NestedPropertyInfo { get; }

        public bool WithValue { get; }
    }
}