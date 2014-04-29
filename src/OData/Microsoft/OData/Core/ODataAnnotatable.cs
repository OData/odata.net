//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Base class for all annotatable types in OData library.
    /// </summary>
#if ORCAS
    [Serializable]
#endif
    public abstract class ODataAnnotatable
    {
        /// <summary>The map of annotationsAsArray keyed by type.</summary>
#if ORCAS
        [NonSerialized]
#endif
        private object annotations;

        /// <summary>
        /// Collection of custom instance annotations.
        /// </summary>
#if ORCAS
        [NonSerialized]
#endif
        private ICollection<ODataInstanceAnnotation> instanceAnnotations = new Collection<ODataInstanceAnnotation>();

        /// <summary>Gets or sets the annotation by type.</summary>
        /// <returns>The annotation of type T or null if not present.</returns>
        /// <typeparam name="T">The type of the annotation.</typeparam>
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

        /// <summary>Sets an annotation of type T.</summary>
        /// <param name="annotation">The annotation to set.</param>
        /// <typeparam name="T">The type of the annotation.</typeparam>
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
        /// Get the annotation of type <typeparamref name="T"/>. If the annotation is not found, create a new
        /// instance of the annotation and call SetAnnotation on it then return the newly created instance.
        /// </summary>
        /// <typeparam name="T">The type of the annotation.</typeparam>
        /// <returns>The annotation of type <typeparamref name="T"/>.</returns>
        internal T GetOrCreateAnnotation<T>() where T : class, new()
        {
            T annotation = this.GetAnnotation<T>();
            if (annotation == null)
            {
                annotation = new T();
                this.SetAnnotation(annotation);
            }

            return annotation;
        }

        /// <summary>
        /// Gets the custom instance annotations.
        /// </summary>
        /// <returns>The custom instance annotations.</returns>
        internal ICollection<ODataInstanceAnnotation> GetInstanceAnnotations()
        {
            Debug.Assert(this.instanceAnnotations != null, "this.instanceAnnotations != null");
            return this.instanceAnnotations;
        }

        /// <summary>
        /// Sets the custom instance annotations.
        /// </summary>
        /// <param name="value">The new value to set.</param>
        internal void SetInstanceAnnotations(ICollection<ODataInstanceAnnotation> value)
        {
            ExceptionUtils.CheckArgumentNotNull(value, "value");
            this.instanceAnnotations = value;
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
