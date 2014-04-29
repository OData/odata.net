//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using Microsoft.OData.Edm.Annotations;

namespace Microsoft.OData.Edm.Library.Annotations
{
    /// <summary>
    /// Represents an EDM annotation with an immediate value.
    /// </summary>
    public abstract class EdmVocabularyAnnotation : EdmElement, IEdmVocabularyAnnotation
    {
        private readonly IEdmVocabularyAnnotatable target;
        private readonly IEdmValueTerm term;
        private readonly string qualifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmVocabularyAnnotation"/> class.
        /// </summary>
        /// <param name="target">Element the annotation applies to.</param>
        /// <param name="term">Term bound by the annotation.</param>
        /// <param name="qualifier">Qualifier used to discriminate between multiple bindings of the same property or type.</param>
        protected EdmVocabularyAnnotation(IEdmVocabularyAnnotatable target, IEdmValueTerm term, string qualifier)
        {
            EdmUtil.CheckArgumentNull(target, "target");
            EdmUtil.CheckArgumentNull(term, "term");

            this.target = target;
            this.term = term;
            this.qualifier = qualifier;
        }

        /// <summary>
        /// Gets the element the annotation applies to.
        /// </summary>
        public IEdmVocabularyAnnotatable Target
        {
            get { return this.target; }
        }

        /// <summary>
        /// Gets the term bound by the annotation.
        /// </summary>
        public IEdmTerm Term
        {
            get { return this.term; }
        }

        /// <summary>
        /// Gets the qualifier used to discriminate between multiple bindings of the same property or type.
        /// </summary>
        public string Qualifier
        {
            get { return this.qualifier; }
        }
    }
}
