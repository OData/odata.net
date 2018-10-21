//---------------------------------------------------------------------
// <copyright file="ODataAnnotatableExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Microsoft.OData.Client
{
    internal static class ODataAnnotatableExtensions
    {
        public static void SetAnnotation<T>(this ODataAnnotatable annotatable, T annotation)
            where T : class
        {
            Debug.Assert(annotatable != null, "annotatable != null");
            Debug.Assert(annotation != null, "annotation != null");

            InternalDictionary<T>.SetAnnotation(annotatable, annotation);
        }

        public static T GetAnnotation<T>(this ODataAnnotatable annotatable)
            where T : class
        {
            Debug.Assert(annotatable != null, "annotatable != null");

            return InternalDictionary<T>.GetAnnotation(annotatable);
        }

        private static class InternalDictionary<T> where T : class
        {
            private static readonly ConditionalWeakTable<ODataAnnotatable, T> Dictionary =
                new ConditionalWeakTable<ODataAnnotatable, T>();

            public static void SetAnnotation(ODataAnnotatable annotatable, T annotation)
            {
                Dictionary.Add(annotatable, annotation);
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
