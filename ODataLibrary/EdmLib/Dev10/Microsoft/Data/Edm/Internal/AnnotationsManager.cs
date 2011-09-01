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

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Library;

namespace Microsoft.Data.Edm
{
    /// <summary>
    /// AnnotationsManager provides services for implementing IEdmAnnotatable with transient annotations.
    /// Transient annotations can be added and removed programmatically. Elements that support transient annotations
    /// store them as untyped objects completely under the control of AnnotationsManager.
    /// An object representing transient annotations is in one of these states:
    ///    1) Null, if the element has no transient annotations.
    ///    2) An EdmAnnotation, if the element has exactly one annotation.
    ///    3) A list of EdmAnnotation, if the element has more than one annotation.
    /// If the speed of annotation lookup for elements with many annotations becomes a concern, another option
    /// including a dictionary is possible.
    /// </summary>
    internal class AnnotationsManager
    {
        private readonly bool allowRemovingImmutableAnnotations;

        public AnnotationsManager()
        {
            this.allowRemovingImmutableAnnotations = false;
        }

        public AnnotationsManager(bool allowRemovingImmutableAnnotations)
        {
            this.allowRemovingImmutableAnnotations = allowRemovingImmutableAnnotations;
        }

        public static void SetAnnotation(ref object transientAnnotations, string namespaceName, string localName, object value)
        {
            SetAnnotation(null, ref transientAnnotations, namespaceName, localName, value, false);
        }

        private static void SetAnnotation(IEnumerable<IEdmAnnotation> immutableAnnotations, ref object transientAnnotations, string namespaceName, string localName, object value, bool allowRemovingImmutableAnnotations)
        {
            bool needTombstone = false;
            if (immutableAnnotations != null)
            {
                if (immutableAnnotations.Any(existingAnnotation => existingAnnotation.Namespace() == namespaceName && existingAnnotation.LocalName() == localName))
                {
                    if (allowRemovingImmutableAnnotations)
                    {
                        needTombstone = true;
                    }
                    else
                    {
                        throw new InvalidOperationException(Edm.Strings.Annotations_ImmutableChange(namespaceName,localName));
                    }
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

            IEdmAnnotation newAnnotation = new EdmAnnotation(namespaceName, localName, value);

            if (transientAnnotations == null)
            {
                transientAnnotations = newAnnotation;
                return;
            }

            IEdmAnnotation singleAnnotation = transientAnnotations as IEdmAnnotation;
            if (singleAnnotation != null)
            {
                if (singleAnnotation.Namespace() == namespaceName && singleAnnotation.LocalName() == localName)
                {
                    transientAnnotations = newAnnotation;
                }
                else
                {
                    List<IEdmAnnotation> newAnnotations = new List<IEdmAnnotation> {singleAnnotation, newAnnotation};
                    transientAnnotations = newAnnotations;
                }

                return;
            }

            List<IEdmAnnotation> annotationsList = (List<IEdmAnnotation>)transientAnnotations;
            for (int index = 0; index < annotationsList.Count; index++)
            {
                IEdmAnnotation existingAnnotation = annotationsList[index];
                if (existingAnnotation.Namespace() == namespaceName && existingAnnotation.LocalName() == localName)
                {
                    annotationsList[index] = newAnnotation;
                    return;
                }
            }

            annotationsList.Add(newAnnotation);
        }

        public static object GetAnnotation(object transientAnnotations, string namespaceName, string localName)
        {
            return FindAnnotation(null, transientAnnotations, namespaceName, localName);
        }

        private static IEdmAnnotation FindTransientAnnotation(object transientAnnotations, string namespaceName, string localName)
        {
            if (transientAnnotations != null)
            {
                IEdmAnnotation singleAnnotation = transientAnnotations as IEdmAnnotation;
                if (singleAnnotation != null)
                {
                    if (singleAnnotation.Namespace() == namespaceName && singleAnnotation.LocalName() == localName)
                    {
                        return singleAnnotation;
                    }
                }
                else
                {
                    List<IEdmAnnotation> annotationsList = (List<IEdmAnnotation>)transientAnnotations;
                    return annotationsList.FirstOrDefault(
                        existingAnnotation => existingAnnotation.Namespace() == namespaceName && existingAnnotation.LocalName() == localName);
                }
            }

            return null;
        }

        private static object ImmediateValue(IEdmAnnotation annotation)
        {
            IEdmImmediateValueAnnotation immediate = annotation as IEdmImmediateValueAnnotation;
            return immediate != null ? immediate.Value : null;
        }

        private static object FindAnnotation(IEnumerable<IEdmAnnotation> immutableAnnotations, object transientAnnotations, string namespaceName, string localName)
        {
            IEdmAnnotation annotation = FindTransientAnnotation(transientAnnotations, namespaceName, localName);
            if (annotation != null)
            {
                return ImmediateValue(annotation);
            }

            if (immutableAnnotations != null)
            {
                foreach (IEdmAnnotation existingAnnotation in immutableAnnotations)
                {
                    if (existingAnnotation.Namespace() == namespaceName && existingAnnotation.LocalName() == localName)
                    {
                        // No need to check that the immutable annotation isn't Dead, because if it were
                        // the tombstone would have been found in the transient annotations.
                        return ImmediateValue(existingAnnotation);
                    }
                }
            }

            return null;
        }

        private static void RemoveTransientAnnotation(ref object transientAnnotations, string namespaceName, string localName)
        {
            if (transientAnnotations != null)
            {
                IEdmAnnotation singleAnnotation = transientAnnotations as IEdmAnnotation;
                if (singleAnnotation != null)
                {
                    if (singleAnnotation.Namespace() == namespaceName && singleAnnotation.LocalName() == localName)
                    {
                        transientAnnotations = null;
                        return;
                    }
                }
                else
                {
                    List<IEdmAnnotation> annotationsList = (List<IEdmAnnotation>)transientAnnotations;
                    for (int index = 0; index < annotationsList.Count; index++)
                    {
                        IEdmAnnotation existingAnnotation = annotationsList[index];
                        if (existingAnnotation.Namespace() == namespaceName && existingAnnotation.LocalName() == localName)
                        {
                            annotationsList.RemoveAt(index);
                            if (annotationsList.Count == 1 && (annotationsList == transientAnnotations))
                            {
                                transientAnnotations = annotationsList[0];
                            }

                            return;
                        }
                    }
                }
            }
        }

        public static IEnumerable<IEdmAnnotation> Annotations(object transientAnnotations)
        {
            return Annotations(null, transientAnnotations, false);
        }

        private static IEnumerable<IEdmAnnotation> Annotations(IEnumerable<IEdmAnnotation> immutableAnnotations, object transientAnnotations, bool allowRemovingImmutableAnnotations)
        {
            if (immutableAnnotations != null)
            {
                foreach (IEdmAnnotation existingAnnotation in immutableAnnotations)
                {
                    if (!IsDead(allowRemovingImmutableAnnotations, existingAnnotation.Namespace(), existingAnnotation.LocalName(), transientAnnotations))
                    {
                        yield return existingAnnotation;
                    }
                }
            }

            if (transientAnnotations == null)
            {
                yield break;
            }

            IEdmAnnotation singleAnnotation = transientAnnotations as IEdmAnnotation;
            if (singleAnnotation != null)
            {
                IEdmImmediateValueAnnotation immediateAnnotation = singleAnnotation as IEdmImmediateValueAnnotation;
                if (immediateAnnotation == null || immediateAnnotation.Value != null)
                {
                    yield return singleAnnotation;
                }

                yield break;
            }

            List<IEdmAnnotation> annotationsList = (List<IEdmAnnotation>)transientAnnotations;
            foreach (IEdmAnnotation existingAnnotation in annotationsList)
            {
                IEdmImmediateValueAnnotation immediateAnnotation = existingAnnotation as IEdmImmediateValueAnnotation;
                if (immediateAnnotation == null || immediateAnnotation.Value != null)
                {
                    yield return existingAnnotation;
                }
            }
        }

        public void SetAnnotation(IEnumerable<IEdmAnnotation> immutableAnnotations, ref object transientAnnotations, string namespaceName, string localName, object value)
        {
            SetAnnotation(immutableAnnotations, ref transientAnnotations, namespaceName, localName, value, this.allowRemovingImmutableAnnotations);
        }

        public static object GetAnnotation(IEnumerable<IEdmAnnotation> immutableAnnotations, object transientAnnotations, string namespaceName, string localName)
        {
            return FindAnnotation(immutableAnnotations, transientAnnotations, namespaceName, localName);
        }

        public IEnumerable<IEdmAnnotation> Annotations(IEnumerable<IEdmAnnotation> immutableAnnotations, object transientAnnotations)
        {
            return Annotations(immutableAnnotations, transientAnnotations, this.allowRemovingImmutableAnnotations);
        }

        private static bool IsDead(bool allowRemovingImmutableAnnotations, string namespaceName, string localName, object transientAnnotations)
        {
            return allowRemovingImmutableAnnotations && FindTransientAnnotation(transientAnnotations, namespaceName, localName) != null;
        }
    }
}
