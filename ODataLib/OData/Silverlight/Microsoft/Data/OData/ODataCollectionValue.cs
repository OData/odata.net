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
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    #endregion Namespaces

    /// <summary>
    /// OData representation of a Collection.
    /// </summary>
    public sealed class ODataCollectionValue : ODataValue
    {
        /// <summary>Gets or sets the type of the collection value.</summary>
        /// <returns>The type of the collection value.</returns>
        public string TypeName
        {
            get;
            set;
        }

        /// <summary>Gets or sets the items in the bag value.</summary>
        /// <returns>The items in the bag value.</returns>
        public IEnumerable Items
        {
            get;
            set;
        }
    }
}
