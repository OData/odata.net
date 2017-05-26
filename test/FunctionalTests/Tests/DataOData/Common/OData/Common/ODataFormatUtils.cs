//---------------------------------------------------------------------
// <copyright file="ODataFormatUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.OData;
    #endregion Namespaces

    /// <summary>
    /// Helper class for ODataFormats
    /// </summary>
    public static class ODataFormatUtils
    {
        /// <summary>
        /// Returns enumeration of all supported formats
        /// </summary>
        public static readonly IEnumerable<ODataFormat> ODataFormats = new ODataFormat[] { null, ODataFormat.Json };
    }
}
