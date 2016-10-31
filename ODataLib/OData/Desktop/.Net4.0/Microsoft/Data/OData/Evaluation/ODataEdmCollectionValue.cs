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

namespace Microsoft.Data.OData.Evaluation
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library.Values;
    using Microsoft.Data.Edm.Values;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// An <see cref="IEdmCollectionValue"/> implementation of an OData collection value.
    /// </summary>
    internal sealed class ODataEdmCollectionValue : EdmValue, IEdmCollectionValue
    {
        /// <summary>
        /// The <see cref="ODataCollectionValue"/> to convert into an <see cref="IEdmCollectionValue"/>.
        /// </summary>
        private readonly ODataCollectionValue collectionValue;

        /// <summary>
        /// Creates a new instance of an <see cref="ODataEdmCollectionValue"/>.
        /// </summary>
        /// <param name="collectionValue">The <see cref="ODataCollectionValue"/> to create the collection value for.</param>
        internal ODataEdmCollectionValue(ODataCollectionValue collectionValue)
            : base(collectionValue.GetEdmType())
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(collectionValue != null, "collectionValue != null");

            this.collectionValue = collectionValue;
        }

        /// <summary>
        /// Gets the values stored in this collection.
        /// </summary>
        public IEnumerable<IEdmDelayedValue> Elements
        {
            get 
            {
                if (this.collectionValue != null)
                {
                    IEdmTypeReference itemType = this.Type == null ? null : (this.Type.Definition as IEdmCollectionType).ElementType;
                    foreach (object collectionItem in this.collectionValue.Items)
                    {
                        yield return ODataEdmValueUtils.ConvertValue(collectionItem, itemType);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get 
            {
                return EdmValueKind.Collection;
            }
        }
    }
}
