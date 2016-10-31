//   OData .NET Libraries ver. 5.6.3
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

namespace System.Data.Services.Client
{
    using Microsoft.Data.OData;

    /// <summary>
    /// Writing navigation link arguments
    /// </summary>
    public sealed class WritingNavigationLinkArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WritingNavigationLinkArgs"/> class.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        public WritingNavigationLinkArgs(ODataNavigationLink link, object source, object target)
        {
            Util.CheckArgumentNull(link, "link");
            Util.CheckArgumentNull(source, "source");
            Util.CheckArgumentNull(target, "target");
            this.Link = link;
            this.Source = source;
            this.Target = target;
        }

        /// <summary>
        /// Gets the link.
        /// </summary>
        /// <value>
        /// The link.
        /// </value>
        public ODataNavigationLink Link { get; private set; }

        /// <summary>
        /// Gets the source.
        /// </summary>
        public object Source { get; private set; }

        /// <summary>
        /// Gets the target.
        /// </summary>
        public object Target { get; private set; }
    }
}
