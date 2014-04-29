//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    #region Namespaces

    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// A segment representing an singleton in a path.
    /// </summary>
    public sealed class SingletonSegment : ODataPathSegment
    {
        /// <summary>
        /// The singleton represented by this segment.
        /// </summary>
        private readonly IEdmSingleton singleton;

        /// <summary>
        /// Build a segment representing an singleton
        /// </summary>
        /// <param name="singleton">The singleton represented by this segment.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input singleton is null.</exception>
        public SingletonSegment(IEdmSingleton singleton)
        {
            ExceptionUtils.CheckArgumentNotNull(singleton, "singleton");

            this.singleton = singleton;

            this.TargetEdmNavigationSource = singleton;
            this.TargetEdmType = singleton.EntityType();
            this.TargetKind = RequestTargetKind.Resource;
            this.SingleResult = true;
        }

        /// <summary>
        /// Gets the singleton represented by this segment.
        /// </summary>
        public IEdmSingleton Singleton
        {
            get { return this.singleton; }
        }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> of this <see cref="SingletonSegment"/>. 
        /// This will always be an  <see cref="IEdmEntityType"/> of this singleton.
        /// </summary>
        public override IEdmType EdmType
        {
            get { return this.singleton.EntityType(); }
        }

        /// <summary>
        /// Translate an <see cref="SingletonSegment"/> into another type using an instance of <see cref="PathSegmentTranslator{T}"/>.
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
        /// Handle an <see cref="SingletonSegment"/> using the an instance of the <see cref="PathSegmentHandler"/>.
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
            SingletonSegment otherSingleton = other as SingletonSegment;
            return otherSingleton != null && otherSingleton.singleton == this.Singleton;
        }
    }
}
