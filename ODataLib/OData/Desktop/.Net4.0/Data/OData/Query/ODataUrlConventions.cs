//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Query
{
    using System.Diagnostics;
    using Microsoft.Data.OData.Evaluation;

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
                DebugUtils.CheckNoExternalCallers();
                return this.urlConvention;
            }
        }
    }
}
