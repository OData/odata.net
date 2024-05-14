//---------------------------------------------------------------------
// <copyright file="ODataJsonReaderNestedPropertyInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces
    using Microsoft.OData.Edm;
    #endregion Namespaces

    internal class ODataJsonReaderNestedPropertyInfo : ODataJsonReaderNestedInfo
    {
        internal ODataJsonReaderNestedPropertyInfo(ODataPropertyInfo nestedPropertyInfo, IEdmProperty nestedProperty, bool withValue = true) : base(nestedProperty)
        {
            NestedPropertyInfo = nestedPropertyInfo;
            WithValue = withValue;
        }

        internal ODataPropertyInfo NestedPropertyInfo { get; }

        public bool WithValue { get; }
    }
}