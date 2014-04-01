//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using Microsoft.OData.Edm.Annotations;

namespace Microsoft.OData.Edm.Library.Annotations
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
