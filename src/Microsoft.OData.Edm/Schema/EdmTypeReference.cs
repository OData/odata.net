//---------------------------------------------------------------------
// <copyright file="EdmTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a reference to an EDM type.
    /// </summary>
    public abstract class EdmTypeReference : EdmElement, IEdmTypeReference
    {
        private readonly IEdmType definition;
        private readonly bool isNullable;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTypeReference"/> class.
        /// </summary>
        /// <param name="definition">Type that describes this value.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        protected EdmTypeReference(IEdmType definition, bool isNullable)
        {
            EdmUtil.CheckArgumentNull(definition, "definition");

            this.definition = definition;
            this.isNullable = isNullable;
        }

        /// <summary>
        /// Gets a value indicating whether this type is nullable.
        /// </summary>
        public bool IsNullable
        {
            get { return this.isNullable; }
        }

        /// <summary>
        /// Gets the definition to which this type refers.
        /// </summary>
        public IEdmType Definition
        {
            get { return this.definition; }
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
