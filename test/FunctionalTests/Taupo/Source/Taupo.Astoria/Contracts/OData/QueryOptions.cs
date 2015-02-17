//---------------------------------------------------------------------
// <copyright file="QueryOptions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// A collection of constants representing the various special query options for a data service
    /// </summary>
    public static class QueryOptions
    {
        /// <summary>
        /// The '$expand' query option
        /// </summary>
        public const string Expand = "$expand";

        /// <summary>
        /// The '$filter' query option
        /// </summary>
        public const string Filter = "$filter";

        /// <summary>
        /// The '$inlinecount' query option
        /// </summary>
        public const string InlineCount = "$inlinecount";

        /// <summary>
        /// The 'allpages' value for the '$inlinecount' query option
        /// </summary>
        public const string InlineCountAllPages = "allpages";

        /// <summary>
        /// The 'none' value for the '$inlinecount' query option
        /// </summary>
        public const string InlineCountNone = "none";

        /// <summary>
        /// The '$orderby' query option
        /// </summary>
        public const string OrderBy = "$orderby";

        /// <summary>
        /// The '$skip' query option
        /// </summary>
        public const string Skip = "$skip";

        /// <summary>
        /// The '$select' query option
        /// </summary>
        public const string Select = "$select";
                
        /// <summary>
        /// The '$skiptoken' query option
        /// </summary>
        public const string SkipToken = "$skiptoken";

        /// <summary>
        /// The '$top' query option
        /// </summary>
        public const string Top = "$top";

        /// <summary>
        /// The '$format' query option
        /// </summary>
        public const string Format = "$format";
    }
}
