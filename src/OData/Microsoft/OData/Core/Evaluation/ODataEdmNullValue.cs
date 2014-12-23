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

namespace Microsoft.OData.Core.Evaluation
{
    #region Namespaces
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library.Values;
    using Microsoft.OData.Edm.Values;
    #endregion Namespaces

    /// <summary>
    /// An <see cref="IEdmValue"/> implementation of an OData entry or complex value.
    /// </summary>
    internal sealed class ODataEdmNullValue : EdmValue, IEdmNullValue
    {
        /// <summary>Static, un-typed <see cref="IEdmNullValue"/> instance for use in ODataLib.</summary>
        internal static ODataEdmNullValue UntypedInstance = new ODataEdmNullValue(/*type*/ null);

        /// <summary>
        /// Creates a new Edm null value with the specified type.
        /// </summary>
        /// <param name="type">The type of the null value (if available).</param>
        internal ODataEdmNullValue(IEdmTypeReference type)
            : base(type)
        {
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get 
            {
                return EdmValueKind.Null;
            }
        }
    }
}
