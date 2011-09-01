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

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Common base class for all EDM elements.
    /// </summary>
    public abstract class EdmElement : IEdmElement
    {
        private object transientAnnotations;

        /// <summary>
        /// Gets annotations associated with this element.
        /// </summary>
        public IEnumerable<IEdmAnnotation> Annotations
        {
            get { return AnnotationsManager.Annotations(this.transientAnnotations); }
        }

        /// <summary>
        /// Sets an annotation value in an EDM element. If the value is null, no annotation is added and an existing annotation with the same name is removed.
        /// </summary>
        /// <param name="namespaceName">The namespace of the annotation.</param>
        /// <param name="localName">The name of the annotation within the namespace.</param>
        /// <param name="value">The value of the new annotation.</param>
        public void SetAnnotation(string namespaceName, string localName, object value)
        {
            AnnotationsManager.SetAnnotation(ref this.transientAnnotations, namespaceName, localName, value);
        }

        /// <summary>
        /// Retrieves an annotation value from an EDM element. Returns null if no annotation with the given name exists.
        /// </summary>
        /// <param name="namespaceName">The namespace of the annotation.</param>
        /// <param name="localName">The name of the annotation within the namespace.</param>
        /// <returns>The requested annotation, or null if no such annotation exists.</returns>
        public object GetAnnotation(string namespaceName, string localName)
        {
            return AnnotationsManager.GetAnnotation(this.transientAnnotations, namespaceName, localName);
        }
    }
}
