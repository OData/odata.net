//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
