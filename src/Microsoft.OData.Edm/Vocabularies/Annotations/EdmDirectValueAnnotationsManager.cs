//---------------------------------------------------------------------
// <copyright file="EdmDirectValueAnnotationsManager.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Direct-value annotations manager provides services for setting and getting transient annotations on elements.
    /// </summary>
    /// <remarks>
    /// An object representing transient annotations is in one of these states:
    ///    1) Null, if the element has no transient annotations.
    ///    2) An EdmVocabularyAnnotation, if the element has exactly one annotation.
    ///    3) A list of EdmVocabularyAnnotation, if the element has more than one annotation.
    /// If the speed of annotation lookup for elements with many annotations becomes a concern, another option
    /// including a dictionary is possible.
    /// </remarks>
    public class EdmDirectValueAnnotationsManager : IEdmDirectValueAnnotationsManager
    {
        /// <summary>
        /// Keeps track of transient annotations on elements.
        /// </summary>
        private VersioningDictionary<IEdmElement, object> annotationsDictionary;

        /// <summary>
        /// Used for locking during updates to the annotations dictionary;
        /// </summary>
        private object annotationsDictionaryLock = new object();

        /// <summary>
        /// Elements for which normal comparison failed to produce a valid result, arbitrarily ordered to enable stable comparisons.
        /// </summary>
        private VersioningList<IEdmElement> unsortedElements = VersioningList<IEdmElement>.Create();

        /// <summary>
        /// Used for locking during updates to the unsorted elements list.
        /// </summary>
        private object unsortedElementsLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDirectValueAnnotationsManager"/> class.
        /// </summary>
        public EdmDirectValueAnnotationsManager()
        {
            this.annotationsDictionary = VersioningDictionary<IEdmElement, object>.Create(this.CompareElements);
        }

        /// <summary>
        /// Gets annotations associated with an element.
        /// </summary>
        /// <param name="element">The annotated element.</param>
        /// <returns>The immediate annotations for the element.</returns>
        public IEnumerable<IEdmDirectValueAnnotation> GetDirectValueAnnotations(IEdmElement element)
        {
            // Fetch the annotations dictionary once and only once, because this.annotationsDictionary might get updated by another thread.
            VersioningDictionary<IEdmElement, object> annotationsDictionary = this.annotationsDictionary;

            IEnumerable<IEdmDirectValueAnnotation> immutableAnnotations = this.GetAttachedAnnotations(element);
            object transientAnnotations = GetTransientAnnotations(element, annotationsDictionary);

            if (immutableAnnotations != null)
            {
                foreach (IEdmDirectValueAnnotation existingAnnotation in immutableAnnotations)
                {
                    if (!IsDead(existingAnnotation.NamespaceUri, existingAnnotation.Name, transientAnnotations))
                    {
                        yield return existingAnnotation;
                    }
                }
            }

            foreach (IEdmDirectValueAnnotation existingAnnotation in TransientAnnotations(transientAnnotations))
            {
                yield return existingAnnotation;
            }
        }

        /// <summary>
        /// Sets an annotation value for an EDM element. If the value is null, no annotation is added and an existing annotation with the same name is removed.
        /// </summary>
        /// <param name="element">The annotated element.</param>
        /// <param name="namespaceName">Namespace that the annotation belongs to.</param>
        /// <param name="localName">Name of the annotation within the namespace.</param>
        /// <param name="value">New annotation to set.</param>
        public void SetAnnotationValue(IEdmElement element, string namespaceName, string localName, object value)
        {
            lock (this.annotationsDictionaryLock)
            {
                // Use a local variable to store any interim changes to the annotations dictionary, and perform one atomic update
                // to the field. Otherwise, other threads could see a dictionary in an interim state.
                VersioningDictionary<IEdmElement, object> annotationsDictionary = this.annotationsDictionary;
                this.SetAnnotationValue(element, namespaceName, localName, value, ref annotationsDictionary);

                this.annotationsDictionary = annotationsDictionary;
            }
        }

        /// <summary>
        /// Sets a set of annotation values. If a supplied value is null, no annotation is added and an existing annotation with the same name is removed.
        /// </summary>
        /// <param name="annotations">The annotations to set</param>
        public void SetAnnotationValues(IEnumerable<IEdmDirectValueAnnotationBinding> annotations)
        {
            lock (this.annotationsDictionaryLock)
            {
                // Use a local variable to store any interim changes to the annotations dictionary, and perform one atomic update
                // to the field. Otherwise, other threads could see a dictionary in an interim state.
                VersioningDictionary<IEdmElement, object> annotationsDictionary = this.annotationsDictionary;

                foreach (IEdmDirectValueAnnotationBinding annotation in annotations)
                {
                    this.SetAnnotationValue(annotation.Element, annotation.NamespaceUri, annotation.Name, annotation.Value, ref annotationsDictionary);
                }

                this.annotationsDictionary = annotationsDictionary;
            }
        }

        /// <summary>
        /// Retrieves an annotation value for an EDM element. Returns null if no annotation with the given name exists for the given element.
        /// </summary>
        /// <param name="element">The annotated element.</param>
        /// <param name="namespaceName">Namespace that the annotation belongs to.</param>
        /// <param name="localName">Local name of the annotation.</param>
        /// <returns>Returns the annotation that corresponds to the provided name. Returns null if no annotation with the given name exists for the given element.</returns>
        public object GetAnnotationValue(IEdmElement element, string namespaceName, string localName)
        {
            // Fetch the annotations dictionary once and only once, because this.annotationsDictionary might get updated by another thread.
            VersioningDictionary<IEdmElement, object> annotationsDictionary = this.annotationsDictionary;

            return this.GetAnnotationValue(element, namespaceName, localName, annotationsDictionary);
        }

        /// <summary>
        /// Retrieves a set of annotation values. For each requested value, returns null if no annotation with the given name exists for the given element.
        /// </summary>
        /// <param name="annotations">The set of requested annotations</param>
        /// <returns>Returns values that correspond to the provided annotations. A value is null if no annotation with the given name exists for the given element.</returns>
        public object[] GetAnnotationValues(IEnumerable<IEdmDirectValueAnnotationBinding> annotations)
        {
            // Fetch the annotations dictionary once and only once, because this.annotationsDictionary might get updated by another thread.
            VersioningDictionary<IEdmElement, object> annotationsDictionary = this.annotationsDictionary;

            object[] values = new object[annotations.Count()];

            int index = 0;
            foreach (IEdmDirectValueAnnotationBinding annotation in annotations)
            {
                values[index++] = this.GetAnnotationValue(annotation.Element, annotation.NamespaceUri, annotation.Name, annotationsDictionary);
            }

            return values;
        }

        /// <summary>
        /// Retrieves the annotations that are directly attached to an element.
        /// </summary>
        /// <param name="element">The element in question.</param>
        /// <returns>The annotations that are directly attached to an element (outside the control of the manager).</returns>
        protected virtual IEnumerable<IEdmDirectValueAnnotation> GetAttachedAnnotations(IEdmElement element)
        {
            return null;
        }

        private static void SetAnnotation(IEnumerable<IEdmDirectValueAnnotation> immutableAnnotations, ref object transientAnnotations, string namespaceName, string localName, object value)
        {
            bool needTombstone = false;
            if (immutableAnnotations != null)
            {
                if (immutableAnnotations.Any(existingAnnotation => existingAnnotation.NamespaceUri == namespaceName && existingAnnotation.Name == localName))
                {
                    needTombstone = true;
                }
            }

            if (value == null)
            {
                // "Removing" an immutable annotation leaves behind a transient annotation with a null value
                // as a tombstone to hide the immutable annotation. The normal logic below makes this happen.
                // Removing a transient annotation actually takes the annotation away.
                if (!needTombstone)
                {
                    RemoveTransientAnnotation(ref transientAnnotations, namespaceName, localName);
                    return;
                }
            }

            if (namespaceName == EdmConstants.DocumentationUri && value != null && !(value is IEdmDocumentation))
            {
                throw new InvalidOperationException(Edm.Strings.Annotations_DocumentationPun(value.GetType().Name));
            }

            IEdmDirectValueAnnotation newAnnotation = value != null ?
                new EdmDirectValueAnnotation(namespaceName, localName, value) :
                new EdmDirectValueAnnotation(namespaceName, localName);

            if (transientAnnotations == null)
            {
                transientAnnotations = newAnnotation;
                return;
            }

            IEdmDirectValueAnnotation singleAnnotation = transientAnnotations as IEdmDirectValueAnnotation;
            if (singleAnnotation != null)
            {
                if (singleAnnotation.NamespaceUri == namespaceName && singleAnnotation.Name == localName)
                {
                    transientAnnotations = newAnnotation;
                }
                else
                {
                    transientAnnotations = VersioningList<IEdmDirectValueAnnotation>.Create().Add(singleAnnotation).Add(newAnnotation);
                }

                return;
            }

            VersioningList<IEdmDirectValueAnnotation> annotationsList = (VersioningList<IEdmDirectValueAnnotation>)transientAnnotations;
            for (int index = 0; index < annotationsList.Count; index++)
            {
                IEdmDirectValueAnnotation existingAnnotation = annotationsList[index];
                if (existingAnnotation.NamespaceUri == namespaceName && existingAnnotation.Name == localName)
                {
                    annotationsList = annotationsList.RemoveAt(index);
                    break;
                }
            }

            transientAnnotations = annotationsList.Add(newAnnotation);
        }

        private static IEdmDirectValueAnnotation FindTransientAnnotation(object transientAnnotations, string namespaceName, string localName)
        {
            if (transientAnnotations != null)
            {
                IEdmDirectValueAnnotation singleAnnotation = transientAnnotations as IEdmDirectValueAnnotation;
                if (singleAnnotation != null)
                {
                    if (singleAnnotation.NamespaceUri == namespaceName && singleAnnotation.Name == localName)
                    {
                        return singleAnnotation;
                    }
                }
                else
                {
                    VersioningList<IEdmDirectValueAnnotation> annotationsList = (VersioningList<IEdmDirectValueAnnotation>)transientAnnotations;
                    return annotationsList.FirstOrDefault(
                        existingAnnotation => existingAnnotation.NamespaceUri == namespaceName && existingAnnotation.Name == localName);
                }
            }

            return null;
        }

        private static void RemoveTransientAnnotation(ref object transientAnnotations, string namespaceName, string localName)
        {
            if (transientAnnotations != null)
            {
                IEdmDirectValueAnnotation singleAnnotation = transientAnnotations as IEdmDirectValueAnnotation;
                if (singleAnnotation != null)
                {
                    if (singleAnnotation.NamespaceUri == namespaceName && singleAnnotation.Name == localName)
                    {
                        transientAnnotations = null;
                        return;
                    }
                }
                else
                {
                    VersioningList<IEdmDirectValueAnnotation> annotationsList = (VersioningList<IEdmDirectValueAnnotation>)transientAnnotations;
                    for (int index = 0; index < annotationsList.Count; index++)
                    {
                        IEdmDirectValueAnnotation existingAnnotation = annotationsList[index];
                        if (existingAnnotation.NamespaceUri == namespaceName && existingAnnotation.Name == localName)
                        {
                            annotationsList = annotationsList.RemoveAt(index);
                            if (annotationsList.Count == 1)
                            {
                                transientAnnotations = annotationsList.Single();
                            }
                            else
                            {
                                transientAnnotations = annotationsList;
                            }

                            return;
                        }
                    }
                }
            }
        }

        private static IEnumerable<IEdmDirectValueAnnotation> TransientAnnotations(object transientAnnotations)
        {
            if (transientAnnotations == null)
            {
                yield break;
            }

            IEdmDirectValueAnnotation singleAnnotation = transientAnnotations as IEdmDirectValueAnnotation;
            if (singleAnnotation != null)
            {
                if (singleAnnotation.Value != null)
                {
                    yield return singleAnnotation;
                }

                yield break;
            }

            VersioningList<IEdmDirectValueAnnotation> annotationsList = (VersioningList<IEdmDirectValueAnnotation>)transientAnnotations;
            foreach (IEdmDirectValueAnnotation existingAnnotation in annotationsList)
            {
                if (existingAnnotation.Value != null)
                {
                    yield return existingAnnotation;
                }
            }
        }

        private static bool IsDead(string namespaceName, string localName, object transientAnnotations)
        {
            return FindTransientAnnotation(transientAnnotations, namespaceName, localName) != null;
        }

        /// <summary>
        /// Retrieves the transient annotations for an EDM element.
        /// </summary>
        /// <param name="element">The annotated element.</param>
        /// <param name="annotationsDictionary">The dictionary for looking up the element's annotations.</param>
        /// <returns>The transient annotations for the element, in a form managed by the annotations manager.</returns>
        /// <remarks>This method is static to guarantee that the annotations dictionary is not fetched more than once per lookup operation.</remarks>
        private static object GetTransientAnnotations(IEdmElement element, VersioningDictionary<IEdmElement, object> annotationsDictionary)
        {
            object transientAnnotations;
            annotationsDictionary.TryGetValue(element, out transientAnnotations);
            return transientAnnotations;
        }

        private void SetAnnotationValue(IEdmElement element, string namespaceName, string localName, object value, ref VersioningDictionary<IEdmElement, object> annotationsDictionary)
        {
            object transientAnnotations = GetTransientAnnotations(element, annotationsDictionary);
            object transientAnnotationsBeforeSet = transientAnnotations;
            SetAnnotation(this.GetAttachedAnnotations(element), ref transientAnnotations, namespaceName, localName, value);

            // There is at least one case (removing an annotation that was not present to begin with) where the transient annotations are not changed,
            // so test to see if updating the dictionary is necessary.
            if (transientAnnotations != transientAnnotationsBeforeSet)
            {
                annotationsDictionary = annotationsDictionary.Set(element, transientAnnotations);
            }
        }

        private object GetAnnotationValue(IEdmElement element, string namespaceName, string localName, VersioningDictionary<IEdmElement, object> annotationsDictionary)
        {
            IEdmDirectValueAnnotation annotation = FindTransientAnnotation(GetTransientAnnotations(element, annotationsDictionary), namespaceName, localName);
            if (annotation != null)
            {
                return annotation.Value;
            }

            IEnumerable<IEdmDirectValueAnnotation> immutableAnnotations = this.GetAttachedAnnotations(element);
            if (immutableAnnotations != null)
            {
                foreach (IEdmDirectValueAnnotation existingAnnotation in immutableAnnotations)
                {
                    if (existingAnnotation.NamespaceUri == namespaceName && existingAnnotation.Name == localName)
                    {
                        // No need to check that the immutable annotation isn't Dead, because if it were
                        // the tombstone would have been found in the transient annotations.
                        return existingAnnotation.Value;
                    }
                }
            }

            return null;
        }

        private int CompareElements(IEdmElement left, IEdmElement right)
        {
            if (left == right)
            {
                return 0;
            }

            /* Left and right are distinct. */

            int leftHash = left.GetHashCode();
            int rightHash = right.GetHashCode();

            if (leftHash < rightHash)
            {
                return -1;
            }

            if (leftHash > rightHash)
            {
                return 1;
            }

            /* Left and right are distinct and have identical hash codes. */

            IEdmNamedElement leftNamed = left as IEdmNamedElement;
            IEdmNamedElement rightNamed = right as IEdmNamedElement;

            if (leftNamed == null)
            {
                if (rightNamed != null)
                {
                    return -1;
                }
            }
            else if (rightNamed == null)
            {
                return 1;
            }
            else
            {
                /* Left and right are both named. */

                int nameComparison = string.Compare(leftNamed.Name, rightNamed.Name, StringComparison.Ordinal);

                if (nameComparison != 0)
                {
                    return nameComparison;
                }
            }

            /* Left and right are distinct, have identical hash codes, and have identical names. */

            /* The first element to occur in the unsorted list is the greatest. */

            while (true)
            {
                foreach (IEdmElement element in this.unsortedElements)
                {
                    if (element == left)
                    {
                        return 1;
                    }

                    if (element == right)
                    {
                        return -1;
                    }
                }

                lock (this.unsortedElementsLock)
                {
                    this.unsortedElements = this.unsortedElements.Add(left);
                }
            }
        }
    }
}
