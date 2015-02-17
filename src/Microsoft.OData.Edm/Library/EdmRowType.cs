//---------------------------------------------------------------------
// <copyright file="EdmRowType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a definition of an EDM row type.
    /// </summary>
    public class EdmRowType : EdmStructuredType, IEdmRowType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmRowType"/> class.
        /// </summary>
        public EdmRowType()
            : base(false, false, null)
        {
        }

        /// <summary>
        /// Gets the kind of this type.
        /// </summary>
        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Row; }
        }
    }
}
