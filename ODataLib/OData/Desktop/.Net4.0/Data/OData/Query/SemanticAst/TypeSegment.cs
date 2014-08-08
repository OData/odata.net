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

namespace Microsoft.Data.OData.Query.SemanticAst
{
    #region Namespaces

    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Metadata;

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
        /// The set containing the entities that we are casting.
        /// </summary>
        private readonly IEdmEntitySet entitySet;

        /// <summary>
        /// Build a type segment using the given <paramref name="edmType"/>.
        /// </summary>
        /// <param name="edmType">The target type of this segment, which may be collection type.</param>
        /// <param name="entitySet">The set containing the entities that we are casting. This can be null.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input edmType is null.</exception>
        /// <exception cref="ODataException">Throws if the input edmType is not relaed to the type of elements in the input entitySet.</exception>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Rule only applies to ODataLib Serialization code.")]
        public TypeSegment(IEdmType edmType, IEdmEntitySet entitySet) 
        {
            ExceptionUtils.CheckArgumentNotNull(edmType, "type");

            this.edmType = edmType;
            this.entitySet = entitySet;

            this.TargetEdmType = edmType;
            this.TargetEdmEntitySet = entitySet;

            // Check that the type they gave us is related to the type of the set
            if (entitySet != null)
            {
                UriParserErrorHelper.ThrowIfTypesUnrelated(edmType, entitySet.ElementType, "TypeSegments");
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
        /// Gets the set containing the entities that we are casting.
        /// </summary>
        public IEdmEntitySet EntitySet
        {
            get { return this.entitySet; }
        }

        /// <summary>
        /// Translate a <see cref="TypeSegment"/> into another type using an instance of <see cref="PathSegmentTranslator{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type that the translator will return after visiting this token.</typeparam>
        /// <param name="translator">An implementation of the translator interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the translator.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input translator is null.</exception>
        public override T Translate<T>(PathSegmentTranslator<T> translator)
        {
            ExceptionUtils.CheckArgumentNotNull(translator, "translator");
            return translator.Translate(this);
        }

        /// <summary>
        /// Handle a <see cref="TypeSegment"/> using an instance of <see cref="PathSegmentHandler"/>.
        /// </summary>
        /// <param name="handler">An implementation of the handler interface.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input handler is null.</exception>
        public override void Handle(PathSegmentHandler handler)
        {
            ExceptionUtils.CheckArgumentNotNull(handler, "translator");
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
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(other, "other");
            TypeSegment otherType = other as TypeSegment;
            return otherType != null && otherType.EdmType == this.EdmType;
        }
    }
}
