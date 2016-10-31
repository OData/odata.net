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

namespace Microsoft.Data.Edm.Annotations
{
    /// <summary>
    /// Manages getting and setting direct value annotations on EDM elements.
    /// </summary>
    public interface IEdmDirectValueAnnotationsManager
    {
        /// <summary>
        /// Gets annotations associated with an element.
        /// </summary>
        /// <param name="element">The annotated element.</param>
        /// <returns>The direct value annotations for the element.</returns>
        IEnumerable<IEdmDirectValueAnnotation> GetDirectValueAnnotations(IEdmElement element);
        
        /// <summary>
        /// Sets an annotation value for an EDM element. If the value is null, no annotation is added and an existing annotation with the same name is removed.
        /// </summary>
        /// <param name="element">The annotated element.</param>
        /// <param name="namespaceName">Namespace that the annotation belongs to.</param>
        /// <param name="localName">Name of the annotation within the namespace.</param>
        /// <param name="value">The value of the annotation.</param>
        void SetAnnotationValue(IEdmElement element, string namespaceName, string localName, object value);
       
        /// <summary>
        /// Sets a set of annotation values. If a supplied value is null, no annotation is added and an existing annotation with the same name is removed.
        /// </summary>
        /// <param name="annotations">The annotations to set</param>
        void SetAnnotationValues(IEnumerable<IEdmDirectValueAnnotationBinding> annotations);

        /// <summary>
        /// Retrieves an annotation value for an EDM element. Returns null if no annotation with the given name exists for the given element.
        /// </summary>
        /// <param name="element">The annotated element.</param>
        /// <param name="namespaceName">Namespace that the annotation belongs to.</param>
        /// <param name="localName">Local name of the annotation.</param>
        /// <returns>Returns the annotation value that corresponds to the provided name. Returns null if no annotation with the given name exists for the given element. </returns>
        object GetAnnotationValue(IEdmElement element, string namespaceName, string localName);

        /// <summary>
        /// Retrieves a set of annotation values. For each requested value, returns null if no annotation with the given name exists for the given element.
        /// </summary>
        /// <param name="annotations">The set of requested annotations</param>
        /// <returns>Returns values that correspond to the provided annotations. A value is null if no annotation with the given name exists for the given element.</returns>
        object[] GetAnnotationValues(IEnumerable<IEdmDirectValueAnnotationBinding> annotations);
    }
}
