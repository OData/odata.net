//---------------------------------------------------------------------
// <copyright file="AnnotatableExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Extension methods which make usage of <see cref="IAnnotatable&lt;T&gt;"/> easier.
    /// </summary>
    public static class AnnotatableExtensionMethods
    {
        /// <summary>
        /// Returns the only annotation of the specified type (exact type)
        /// </summary>
        /// <typeparam name="TBase">The base type of annotations allowed on this annotatable.</typeparam>
        /// <param name="annotatable">The annotatable to get the annotation from.</param>
        /// <param name="annotationType">The annotation type to get.</param>
        /// <returns>The instance of the annotation or null if no such annotation was found.</returns>
        public static object GetAnnotation<TBase>(this IAnnotatable<TBase> annotatable, Type annotationType) where TBase : class
        {
            ExceptionUtilities.CheckArgumentNotNull(annotatable, "annotatable");
            ExceptionUtilities.CheckArgumentNotNull(annotationType, "annotationType");

            return annotatable.Annotations.Where(annotation => annotation != null && annotation.GetType() == annotationType).SingleOrDefault();
        }

        /// <summary>
        /// Sets an annotation of a specified type. This will overwrite the value of already existing annotation of the exact same type.
        /// (the first one found).
        /// </summary>
        /// <typeparam name="T">The type of the annotation to set.</typeparam>
        /// <typeparam name="TBase">The base type of annotations allowed on this annotatable.</typeparam>
        /// <param name="annotatable">The annotatable to set the annotation on.</param>
        /// <param name="annotation">The annotation instance.</param>
        public static void SetAnnotation<T, TBase>(this IAnnotatable<TBase> annotatable, T annotation) where T : TBase where TBase : class
        {
            ExceptionUtilities.CheckArgumentNotNull(annotatable, "annotatable");
            ExceptionUtilities.CheckArgumentNotNull(annotation, "annotation");

            TBase existing = (TBase)annotatable.GetAnnotation(typeof(T));
            if (existing != null)
            {
                annotatable.Annotations.Remove(existing);
            }

            annotatable.Annotations.Add(annotation);
        }
    }
}