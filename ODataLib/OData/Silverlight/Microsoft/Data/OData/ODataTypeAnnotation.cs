//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
