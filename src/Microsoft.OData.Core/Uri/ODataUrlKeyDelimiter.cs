//---------------------------------------------------------------------
// <copyright file="ODataUrlKeyDelimiter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

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
        /// Whether to generate entity keys as '/'-delimited segments instead of using parenthesis.
        /// </summary>
        private readonly bool enableKeyAsSegment;

        /// <summary>
        /// Prevents instances of the <see cref="ODataUrlKeyDelimiter"/> class from being created.
        /// </summary>
        /// <param name="enablekeyAsSegment">if enable key-as-segment in url parser.</param>
        private ODataUrlKeyDelimiter(bool enablekeyAsSegment)
        {
            this.enableKeyAsSegment = enablekeyAsSegment;
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

        internal bool EnableKeyAsSegment
        {
            get
            {
                return this.enableKeyAsSegment;
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
