//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
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

        /// <summary>The navigation source of the value this annotation is on. Only applies to entity values.</summary>
        private readonly IEdmNavigationSource navigationSource;

        /// <summary>
        /// Creates a new instance of the type annotation for an entity value.
        /// </summary>
        /// <param name="navigationSource">The navigation source the entity belongs to (required).</param>
        /// <param name="entityType">The entity type of the entity value if not the base type of the entity set (optional).</param>
        public ODataTypeAnnotation(IEdmNavigationSource navigationSource, IEdmEntityType entityType)
        {
            ExceptionUtils.CheckArgumentNotNull(navigationSource, "navigation source");
            ExceptionUtils.CheckArgumentNotNull(entityType, "entityType");
            this.navigationSource = navigationSource;
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
        /// The navigation source the value belongs to (only applies to entity values).
        /// </summary>
        public IEdmNavigationSource NavigationSource
        {
            get
            {
                return this.navigationSource;
            }
        }
    }
}
