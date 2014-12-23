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

namespace Microsoft.OData.Core
{
    using System;

    /// <summary>
    /// Represents an added link in delta response. 
    /// </summary>
    public sealed class ODataDeltaLink : ODataDeltaLinkBase
    {
        /// <summary>
        /// Initializes a new <see cref="ODataDeltaLink"/>.
        /// </summary>
        /// <param name="source">The id of the entity from which the relationship is defined, which may be absolute or relative.</param>
        /// <param name="target">The id of the related entity, which may be absolute or relative.</param>
        /// <param name="relationship">The name of the relationship property on the parent object.</param>
        public ODataDeltaLink(Uri source, Uri target, string relationship)
            : base(source, target, relationship)
        {
        }
    }
}
