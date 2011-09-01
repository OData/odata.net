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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Base class for all annotatable types in OData library.
    /// </summary>
    public abstract class ODataAnnotatable
    {
        /// <summary>The map of annotationsAsArray keyed by type.</summary>
        private object annotations;

        /// <summary>
        /// Get an annotation by type.
        /// </summary>
        /// <typeparam name="T">The type of the annotation .</typeparam>
        /// <returns>The annotation of type T or null if not present.</returns>
        public T GetAnnotation<T>() where T : class
        {
            if (this.annotations != null)
            {
                object[] annotationsAsArray = this.annotations as object[];
                if (annotationsAsArray == null)
                {
                    return (this.annotations as T);
                }

                for (int i = 0; i < annotationsAsArray.Length; i++)
                {
                    object annotation = annotationsAsArray[i];
                    if (annotation == null)
                    {
                        break;
                    }

                    T typedAnnotation = annotation as T;
                    if (typedAnnotation != null)
                    {
                        return typedAnnotation;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Sets an annotation of type T.
        /// </summary>
        /// <typeparam name="T">The type of the annotation .</typeparam>
        /// <param name="annotation">The annotation to set.</param>
        public void SetAnnotation<T>(T annotation) where T : class
        {
            if (annotation == null)
            {
                RemoveAnnotation<T>();
            }
            else
            {
                this.AddOrReplaceAnnotation(annotation);
            }
        }

        /// <summary>
        /// Check whether a given (non-null) instance is of the specified type (no sub-type).
        /// </summary>
        /// <param name="instance">The (non-null) instance to test.</param>
        /// <param name="type">The type to check for.</param>
        /// <returns>True if the types match; otherwise false.</returns>
        private static bool IsOfType(object instance, Type type)
        {
            return instance.GetType() == type;
        }

        /// <summary>
        /// Replace an existing annotation of type T or add a new one 
        /// if no annotation of type T exists.
        /// </summary>
        /// <typeparam name="T">The type of the annotation.</typeparam>
        /// <param name="annotation">The annotation to set.</param>
        private void AddOrReplaceAnnotation<T>(T annotation) where T : class
        {
            Debug.Assert(annotation != null, "annotation != null");

            if (this.annotations == null)
            {
                this.annotations = annotation;
            }
            else
            {
                object[] annotationsAsArray = this.annotations as object[];
                if (annotationsAsArray == null)
                {
                    if (IsOfType(this.annotations, typeof(T)))
                    {
                        this.annotations = annotation;
                    }
                    else
                    {
                        this.annotations = new object[] { this.annotations, annotation };
                    }
                }
                else
                {
                    int index = 0;
                    for (; index < annotationsAsArray.Length; index++)
                    {
                        // NOTE: current is only null if we are past the last annotation
                        object current = annotationsAsArray[index];
                        if (current == null || IsOfType(current, typeof(T)))
                        {
                            annotationsAsArray[index] = annotation;
                            break;
                        }
                    }

                    if (index == annotationsAsArray.Length)
                    {
                        Array.Resize<object>(ref annotationsAsArray, index * 2);
                        this.annotations = annotationsAsArray;
                        annotationsAsArray[index] = annotation;
                    }
                }
            }
        }

        /// <summary>
        /// Remove the annotation of type T from the set of annotations (if such an annotation exists).
        /// We only allow a single occurence of an annotation of type T.
        /// </summary>
        /// <typeparam name="T">The type of the annotation to remove.</typeparam>
        private void RemoveAnnotation<T>() where T : class
        {
            if (this.annotations != null)
            {
                object[] annotationsAsArray = this.annotations as object[];
                if (annotationsAsArray == null)
                {
                    if (IsOfType(this.annotations, typeof(T)))
                    {
                        this.annotations = null;
                    }
                }
                else
                {
                    int index = 0;
                    int foundAt = -1;
                    int length = annotationsAsArray.Length;
                    while (index < length)
                    {
                        object current = annotationsAsArray[index];
                        if (current == null)
                        {
                            break;
                        }
                        else if (IsOfType(current, typeof(T)))
                        {
                            foundAt = index;
                            break;
                        }

                        index++;
                    }

                    if (foundAt >= 0)
                    {
                        for (int i = foundAt; i < length - 1; ++i)
                        {
                            annotationsAsArray[i] = annotationsAsArray[i + 1];
                        }

                        annotationsAsArray[length - 1] = null;
                    }
                }
            }
        }
    }
}
