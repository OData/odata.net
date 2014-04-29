//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Library.Annotations
{
    /// <summary>
    /// Represents an EDM annotation.
    /// </summary>
    public class EdmAnnotation : EdmVocabularyAnnotation, IEdmValueAnnotation
    {
        private readonly IEdmExpression value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmAnnotation"/> class.
        /// </summary>
        /// <param name="target">Element the annotation applies to.</param>
        /// <param name="term">Term bound by the annotation.</param>
        /// <param name="value">Expression producing the value of the annotation.</param>
        public EdmAnnotation(IEdmVocabularyAnnotatable target, IEdmValueTerm term, IEdmExpression value)
            : this(target, term, null, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmAnnotation"/> class.
        /// </summary>
        /// <param name="target">Element the annotation applies to.</param>
        /// <param name="term">Term bound by the annotation.</param>
        /// <param name="qualifier">Qualifier used to discriminate between multiple bindings of the same property or type.</param>
        /// <param name="value">Expression producing the value of the annotation.</param>
        public EdmAnnotation(IEdmVocabularyAnnotatable target, IEdmValueTerm term, string qualifier, IEdmExpression value)
            : base(target, term, qualifier)
        {
            EdmUtil.CheckArgumentNull(value, "value");
            this.value = value;
        }

        /// <summary>
        /// Gets the expression producing the value of the annotation.
        /// </summary>
        public IEdmExpression Value
        {
            get { return this.value; }
        }
    }
}
