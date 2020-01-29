//---------------------------------------------------------------------
// <copyright file="EdmType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents the definition of an EDM type.
    /// </summary>
    public abstract class EdmType : EdmElement, IEdmType
    {
        /// <summary>
        /// Gets the kind of this type.
        /// </summary>
        public abstract EdmTypeKind TypeKind
        {
            get;
        }

        /// <summary>
        /// Returns the text representation of the current object.
        /// </summary>
        /// <returns>The text representation of the current object.</returns>
        public override string ToString()
        {
            return this.ToTraceString();
        }
    }
}
