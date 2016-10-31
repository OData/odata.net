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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    #endregion Namespaces

    /// <summary>
    /// Class representing a resource collection in a workspace of a data service.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Temporary name only.")]
    public sealed class ODataResourceCollectionInfo : ODataAnnotatable
    {
        /// <summary>Gets or sets the URI representing the Unified Resource Locator (URL) to the collection.</summary>
        /// <returns>The URI representing the Unified Resource Locator (URL) to the collection.</returns>
        public Uri Url
        {
            get;
            set;
        }

        /// <summary>Gets or sets the name of the collection; this is the entity set name in JSON and the HREF in Atom.</summary>
        /// <returns>The name of the collection.</returns>
        /// <remarks>
        /// This property is required when reading and writing the JSON light format, but has no meaning in the Verbose JSON format.
        /// If present in ATOM, it will be used to populate the title element.
        /// </remarks>
        public string Name
        {
            get;
            set;
        }
    }
}
