//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    #region Namespaces

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;

    #endregion Namespaces

    /// <summary>
    /// A segment representing and open property
    /// </summary>
    public sealed class OpenPropertySegment : ODataPathSegment
    {
        /// <summary>
        /// The name of this open property.
        /// </summary>
        private readonly string propertyName;

        /// <summary>
        /// Build a segment to represent an open property.
        /// </summary>
        /// <param name="propertyName">The name of this open property</param>
        public OpenPropertySegment(string propertyName)
        {
            this.propertyName = propertyName;

            this.Identifier = propertyName;
            this.TargetEdmType = null;
            this.TargetKind = RequestTargetKind.OpenProperty;
            this.SingleResult = true;
        }

        /// <summary>
        /// Gets the name of this open property.
        /// </summary>
        public string PropertyName
        {
            get { return this.propertyName; }
        }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> of this <see cref="OpenPropertySegment"/>, which is always null. 
        /// The type of open properties is unknown at this time.
        /// </summary>
        public override IEdmType EdmType
        {
            get { return null; }
        }

        /// <summary>
        /// Translate a <see cref="PathSegmentTranslator{T}"/>.
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
        /// Handle a <see cref="PathSegmentHandler"/>.
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
            OpenPropertySegment otherOpenProperty = other as OpenPropertySegment;
            return otherOpenProperty != null && otherOpenProperty.PropertyName == this.PropertyName;
        }
    }
}
