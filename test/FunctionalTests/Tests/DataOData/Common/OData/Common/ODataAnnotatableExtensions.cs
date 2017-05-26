//---------------------------------------------------------------------
// <copyright file="ODataAnnotatableExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Concurrent;
using Microsoft.OData;

namespace Microsoft.Test.Taupo.OData.Common
{
    public static class ODataAnnotatableExtensions
    {
        public static void SetAnnotation<T>(this ODataAnnotatable annotatable, T annotation)
            where T : class
        {
            ExceptionUtils.CheckArgumentNotNull(annotatable, "annotatable");
            ExceptionUtils.CheckArgumentNotNull(annotation, "annotation");

            InternalDictionary<T>.SetAnnotation(annotatable, annotation);
        }

        public static T GetAnnotation<T>(this ODataAnnotatable annotatable)
            where T : class
        {
            ExceptionUtils.CheckArgumentNotNull(annotatable, "annotatable");

            return InternalDictionary<T>.GetAnnotation(annotatable);
        }

        private static class InternalDictionary<T> where T : class
        {
            private static readonly ConcurrentDictionary<ODataAnnotatable, T> Dictionary =
                new ConcurrentDictionary<ODataAnnotatable, T>();

            public static void SetAnnotation(ODataAnnotatable annotatable, T annotation)
            {
                Dictionary[annotatable] = annotation;
            }

            public static T GetAnnotation(ODataAnnotatable annotatable)
            {
                T annotation;
                if (Dictionary.TryGetValue(annotatable, out annotation))
                {
                    return annotation;
                }

                return default(T);
            }
        }
    }
}
