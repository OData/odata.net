//---------------------------------------------------------------------
// <copyright file="IEdmDirectValueAnnotationsManager.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Manages getting and setting direct annotations on EDM elements.
    /// </summary>
    public interface IEdmDirectValueAnnotationsManager
    {
        /// <summary>
        /// Gets annotations associated with an element.
        /// </summary>
        /// <param name="element">The annotated element.</param>
        /// <returns>The direct annotations for the element.</returns>
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
