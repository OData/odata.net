//---------------------------------------------------------------------
// <copyright file="ODataEdmNullValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Evaluation
{
    #region Namespaces
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library.Values;
    using Microsoft.OData.Edm.Values;
    #endregion Namespaces

    /// <summary>
    /// An <see cref="IEdmValue"/> implementation of an OData resource or complex value.
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
