//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    #region Namespaces

    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core.Metadata;

    #endregion Namespaces

    /// <summary>
    /// A segment representing a cast on the previous segment to another type.
    /// </summary>
    public sealed class TypeSegment : ODataPathSegment
    {
        /// <summary>
        /// The target type of this type segment.
        /// </summary>
        private readonly IEdmType edmType;

        /// <summary>
        /// The navigation source containing the entities that we are casting.
        /// </summary>
        private readonly IEdmNavigationSource navigationSource;

        /// <summary>
        /// Build a type segment using the given <paramref name="edmType"/>.
        /// </summary>
        /// <param name="edmType">The target type of this segment, which may be collection type.</param>
        /// <param name="navigationSource">The navigation source containing the entities that we are casting. This can be null.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input edmType is null.</exception>
        /// <exception cref="ODataException">Throws if the input edmType is not relaed to the type of elements in the input entitySet.</exception>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Rule only applies to ODataLib Serialization code.")]
        public TypeSegment(IEdmType edmType, IEdmNavigationSource navigationSource) 
        {
            ExceptionUtils.CheckArgumentNotNull(edmType, "type");

            this.edmType = edmType;
            this.navigationSource = navigationSource;

            this.TargetEdmType = edmType;
            this.TargetEdmNavigationSource = navigationSource;

            // Check that the type they gave us is related to the type of the set
            if (navigationSource != null)
            {
                UriParserErrorHelper.ThrowIfTypesUnrelated(edmType, navigationSource.EntityType(), "TypeSegments");
            }
        }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> of this <see cref="TypeSegment"/>.
        /// </summary>
        public override IEdmType EdmType
        {
            get { return this.edmType; }
        }

        /// <summary>
        /// Gets the navigation source containing the entities that we are casting.
        /// </summary>
        public IEdmNavigationSource NavigationSource
        {
            get { return this.navigationSource; }
        }

        /// <summary>
        /// Translate a <see cref="TypeSegment"/> into another type using an instance of <see cref="PathSegmentTranslator{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type that the translator will return after visiting this token.</typeparam>
        /// <param name="translator">An implementation of the translator interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the translator.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input translator is null.</exception>
        public override T TranslateWith<T>(PathSegmentTranslator<T> translator)
        {
            ExceptionUtils.CheckArgumentNotNull(translator, "translator");
            return translator.Translate(this);
        }

        /// <summary>
        /// Handle a <see cref="TypeSegment"/> using an instance of <see cref="PathSegmentHandler"/>.
        /// </summary>
        /// <param name="handler">An implementation of the handler interface.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input handler is null.</exception>
        public override void HandleWith(PathSegmentHandler handler)
        {
            ExceptionUtils.CheckArgumentNotNull(handler, "handler");
            handler.Handle(this);
        }

        /// <summary>
        /// Check if this segment is equal to another segment.
        /// </summary>
        /// <param name="other">the other segment to check.</param>
        /// <returns>true if the other segment is equal.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input other is null.</exception>
        internal override bool Equals(ODataPathSegment other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");
            TypeSegment otherType = other as TypeSegment;
            return otherType != null && otherType.EdmType == this.EdmType;
        }
    }
}
