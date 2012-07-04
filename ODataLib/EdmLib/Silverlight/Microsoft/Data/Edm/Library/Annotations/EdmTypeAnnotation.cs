//   Copyright 2011 Microsoft Corporation
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
