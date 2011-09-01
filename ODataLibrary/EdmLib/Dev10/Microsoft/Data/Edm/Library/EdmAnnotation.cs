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

using Microsoft.Data.Edm.Annotations;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents an EDM annotation.
    /// </summary>
    public class EdmAnnotation : IEdmImmediateValueAnnotation
    {
        private readonly object value;
        private readonly EdmValueTerm term;

        /// <summary>
        /// Initializes a new instance of the EdmAnnotation class.
        /// </summary>
        /// <param name="namespaceName">Namespace of the annotation.</param>
        /// <param name="localName">Name of the annotation within the namespace.</param>
        /// <param name="value">Value of the annotation</param>
        public EdmAnnotation(string namespaceName, string localName, object value)
        {
            this.term = new EdmValueTerm(namespaceName, namespaceName, localName);
            this.value = value;
        }

        /// <summary>
        /// Gets the kind of this annotation.
        /// </summary>
        public EdmAnnotationKind Kind
        {
            get { return EdmAnnotationKind.ImmediateValue; }
        }

        /// <summary>
        /// Gets the term of this annotation.
        /// </summary>
        public IEdmTerm Term
        {
            get { return this.term; }
        }

        /// <summary>
        /// Gets the value of this annotation.
        /// </summary>
        public object Value
        {
            get { return this.value; }
        }
    }
}
