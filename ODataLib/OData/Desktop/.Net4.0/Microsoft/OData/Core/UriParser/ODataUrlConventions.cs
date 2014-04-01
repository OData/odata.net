//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser
{
    using System.Diagnostics;
    using Microsoft.OData.Core.Evaluation;

    /// <summary>
    /// Component for controlling what convention are used for generating URLs.
    /// </summary>
    public sealed class ODataUrlConventions
    {
        /// <summary>Singleton instance of the default conventions.</summary>
        private static readonly ODataUrlConventions DefaultInstance = new ODataUrlConventions(UrlConvention.CreateWithExplicitValue(/*generateKeyAsSegment*/ false));

        /// <summary>Singleton instance of the key-as-segment conventions.</summary>
        private static readonly ODataUrlConventions KeyAsSegmentInstance = new ODataUrlConventions(UrlConvention.CreateWithExplicitValue(/*generateKeyAsSegment*/ true));

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
