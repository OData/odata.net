//---------------------------------------------------------------------
// <copyright file="ODataUrlConventions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics;
using Microsoft.OData.Core.Evaluation;

namespace Microsoft.OData.Core
{
    /// <summary>
    /// Component for controlling what convention are used for generating URLs.
    /// </summary>
    public sealed class ODataUrlConventions
    {
        /// <summary>Singleton instance of the default conventions.</summary>
        private static readonly ODataUrlConventions DefaultInstance = new ODataUrlConventions(UrlConvention.CreateWithExplicitValue(/*generateKeyAsSegment*/ false));

        /// <summary>Singleton instance of the key-as-segment conventions.</summary>
        private static readonly ODataUrlConventions KeyAsSegmentInstance = new ODataUrlConventions(UrlConvention.CreateWithExplicitValue(/*generateKeyAsSegment*/ true));

        /// <summary>Singleton instance of the ODataSimplified conventions.</summary>
        private static readonly ODataUrlConventions ODataSimplifiedInstance = new ODataUrlConventions(UrlConvention.CreateODataSimplifiedConvention());

        /// <summary>The url convention to use.</summary>
        private readonly UrlConvention urlConvention;

        /// <summary>
        /// Prevents a default instance of the <see cref="ODataUrlConventions"/> class from being created.
        /// </summary>
        /// <param name="urlConvention">The url convention to use.</param>
        private ODataUrlConventions(UrlConvention urlConvention)
        {
            Debug.Assert(urlConvention != null, "urlConvention != null");
            this.urlConvention = urlConvention;
        }

        /// <summary>
        /// An instance of <see cref="ODataUrlConventions"/> which uses default URL conventions. Specifically, this instance will produce keys that use parentheses like "Customers('ALFKI')".
        /// </summary>
        public static ODataUrlConventions Default
        {
            get { return DefaultInstance; }
        }

        /// <summary>
        /// An instance of <see cref="ODataUrlConventions"/> which uses key-as-segment URL conventions. Specifically, this instance will produce keys that use segments like "Customers/ALFKI".
        /// </summary>
        public static ODataUrlConventions KeyAsSegment
        {
            get { return KeyAsSegmentInstance; }
        }

        /// <summary>
        /// An instance of <see cref="ODataUrlConventions"/> which uses ODataSimplified URL conventions. Specifically, this instance will support key as segemnt and default convention.
        /// </summary>
        public static ODataUrlConventions ODataSimplified
        {
            get { return ODataSimplifiedInstance; }
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
    }
}
