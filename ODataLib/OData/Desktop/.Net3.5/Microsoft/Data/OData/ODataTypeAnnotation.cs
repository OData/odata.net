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
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Annotation which stores the EDM type information of a value.
    /// </summary>
    /// <remarks>
    /// This annotation will be used on ODataEntry, ODataComplexValue and ODataCollectionValue.
    /// </remarks>
    internal sealed class ODataTypeAnnotation
    {
        /// <summary>The EDM type of the value this annotation is on.</summary>
        private readonly IEdmTypeReference type;

        /// <summary>The entity set of the value this annotation is on. Only applies to entity values.</summary>
        private readonly IEdmEntitySet entitySet;

        /// <summary>
        /// Creates a new instance of the type annotation for an entity value.
        /// </summary>
        /// <param name="entitySet">The entity set the entity belongs to (required).</param>
        /// <param name="entityType">The entity type of the entity value if not the base type of the entity set (optional).</param>
        public ODataTypeAnnotation(IEdmEntitySet entitySet, IEdmEntityType entityType)
        {
            ExceptionUtils.CheckArgumentNotNull(entitySet, "entitySet");
            ExceptionUtils.CheckArgumentNotNull(entityType, "entityType");
            this.entitySet = entitySet;
            this.type = entityType.ToTypeReference(/*isNullable*/ true);
        }

        /// <summary>
        /// Creates a new instance of the type annotation for a complex value.
        /// </summary>
        /// <param name="complexType">The type of the complex value (required).</param>
        public ODataTypeAnnotation(IEdmComplexTypeReference complexType)
        {
            ExceptionUtils.CheckArgumentNotNull(complexType, "complexType");
            this.type = complexType;
        }

        /// <summary>
        /// Creates a new instance of the type annotation for a collection value.
        /// </summary>
        /// <param name="collectionType">The type of the collection value (required).</param>
        public ODataTypeAnnotation(IEdmCollectionTypeReference collectionType)
        {
            ExceptionUtils.CheckArgumentNotNull(collectionType, "collectionType");
            this.type = collectionType;
        }

        /// <summary>
        /// The EDM type of the value.
        /// </summary>
        public IEdmTypeReference Type
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>
        /// The entity set the value belongs to (only applies to entity values).
        /// </summary>
        public IEdmEntitySet EntitySet
        {
            get
            {
                return this.entitySet;
            }
        }
    }
}
