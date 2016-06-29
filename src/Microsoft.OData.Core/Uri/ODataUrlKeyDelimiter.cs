//---------------------------------------------------------------------
// <copyright file="ODataUrlKeyDelimiter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics;
using Microsoft.OData.Evaluation;
using Microsoft.OData.UriParser;

namespace Microsoft.OData
{
    /// <summary>
    /// Component for controlling what convention are used for generating URLs.
    /// </summary>
    public sealed class ODataUrlKeyDelimiter
    {
        /// <summary>
        /// Instance of slash delimiter.
        /// </summary>
        private static readonly ODataUrlKeyDelimiter slashDelimiter = new ODataUrlKeyDelimiter(enablekeyAsSegment: true);

        /// <summary>
        /// Instance of parentheses delimiter.
        /// </summary>
        private static readonly ODataUrlKeyDelimiter parenthesesDelimiter = new ODataUrlKeyDelimiter(enablekeyAsSegment: false);

        /// <summary>
        /// The url convention to use.
        /// </summary>
        private readonly UrlConvention urlConvention;

        /// <summary>
        /// Prevents instances of the <see cref="ODataUrlKeyDelimiter"/> class from being created.
        /// </summary>
        /// <param name="enablekeyAsSegment">if enable key-as-segment in url parser.</param>
        private ODataUrlKeyDelimiter(bool enablekeyAsSegment)
        {
            this.urlConvention = enablekeyAsSegment
                ? UrlConvention.CreateODataSimplifiedConvention()
                : UrlConvention.CreateWithExplicitValue(generateKeyAsSegment: false);
        }

        /// <summary>
        /// An instance of <see cref="ODataUrlKeyDelimiter"/> which uses Parentheses as key delimiter in URL.
        /// Specifically, this instance will produce keys that use parentheses like "Customers('ALFKI')".
        /// </summary>
        public static ODataUrlKeyDelimiter Parentheses
        {
            get { return parenthesesDelimiter; }
        }

        /// <summary>
        /// An instance of <see cref="ODataUrlKeyDelimiter"/> which uses slash as key delimiter in URL.
        /// Specifically, this instance will produce keys that use segments like "Customers/ALFKI".
        /// </summary>
        public static ODataUrlKeyDelimiter Slash
        {
            get { return slashDelimiter; }
        }

        /// <summary>
        /// Gets the internal representation of the user-specified convention.
        /// </summary>
        internal UrlConvention UrlConvention
        {
            get
            {
                return this.urlConvention;
            }
        }

        internal static ODataUrlKeyDelimiter GetODataUrlKeyDelimiter(IServiceProvider container)
        {
            return ODataSimplifiedOptions.GetODataSimplifiedOptions(container).EnableParsingKeyAsSegmentUrl
                    ? ODataUrlKeyDelimiter.Slash
                    : ODataUrlKeyDelimiter.Parentheses;
        }
    }
}
