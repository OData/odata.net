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

using System.Collections.Generic;
using Microsoft.Data.Edm.Annotations;

namespace Microsoft.Data.Edm.Library.Annotations
{
    /// <summary>
    /// Represents an EDM type annotation.
    /// </summary>
    public class EdmTypeAnnotation : EdmVocabularyAnnotation, IEdmTypeAnnotation
    {
        private readonly IEnumerable<IEdmPropertyValueBinding> propertyValueBindings;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTypeAnnotation"/> class.
        /// </summary>
        /// <param name="target">Element the annotation applies to.</param>
        /// <param name="term">Term bound by the annotation.</param>
        /// <param name="propertyValueBindings">Value annotations for the properties of the type.</param>
        public EdmTypeAnnotation(IEdmVocabularyAnnotatable target, IEdmTerm term, params IEdmPropertyValueBinding[] propertyValueBindings)
            : this(target, term, null, propertyValueBindings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTypeAnnotation"/> class.
        /// </summary>
        /// <param name="target">Element the annotation applies to.</param>
        /// <param name="term">Term bound by the annotation.</param>
        /// <param name="qualifier">Qualifier used to discriminate between multiple bindings of the same property or type.</param>
        /// <param name="propertyValueBindings">Value annotations for the properties of the type.</param>
        public EdmTypeAnnotation(IEdmVocabularyAnnotatable target, IEdmTerm term, string qualifier, params IEdmPropertyValueBinding[] propertyValueBindings)
            : this(target, term, qualifier, (IEnumerable<IEdmPropertyValueBinding>)propertyValueBindings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTypeAnnotation"/> class.
        /// </summary>
        /// <param name="target">Element the annotation applies to.</param>
        /// <param name="term">Term bound by the annotation.</param>
        /// <param name="qualifier">Qualifier used to discriminate between multiple bindings of the same property or type.</param>
        /// <param name="propertyValueBindings">Value annotations for the properties of the type.</param>
        public EdmTypeAnnotation(IEdmVocabularyAnnotatable target, IEdmTerm term, string qualifier, IEnumerable<IEdmPropertyValueBinding> propertyValueBindings)
            : base(target, term, qualifier)
        {
            EdmUtil.CheckArgumentNull(propertyValueBindings, "propertyValueBindings");
            this.propertyValueBindings = propertyValueBindings;
        }

        /// <summary>
        /// Gets the value annotations for the properties of the type.
        /// </summary>
        public IEnumerable<IEdmPropertyValueBinding> PropertyValueBindings
        {
            get { return this.propertyValueBindings; }
        }
    }
}
