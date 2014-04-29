//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
