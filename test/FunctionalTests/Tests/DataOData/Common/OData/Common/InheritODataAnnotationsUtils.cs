//---------------------------------------------------------------------
// <copyright file="InheritODataAnnotationsUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using Microsoft.OData.Core;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Helper methods to be able to inherit annotations between objects.
    /// Usefull to be able to make copies of annotatable objects keeping annotations as well, since the ODataAnnotatable
    /// doesn't allow enumeration of all existing annotations.
    /// </summary>
    public static class InheritODataAnnotationsUtils
    {
        /// <summary>
        /// Gets annotation from the object including inherrited annotations.
        /// </summary>
        /// <typeparam name="T">The type of the annotation to get.</typeparam>
        /// <param name="annotatable">The annotatable to get the annotation from.</param>
        /// <returns>The annotation instance or null if no such annotation was found.</returns>
        public static T GetInheritedAnnotation<T>(this ODataAnnotatable annotatable) where T : class
        {
            ExceptionUtilities.CheckArgumentNotNull(annotatable, "annotatable");

            T result = annotatable.GetAnnotation<T>();
            if (result == null)
            {
                ODataAnnotatable baseAnnotatable = annotatable.GetAnnotation<ODataAnnotatable>();
                if (baseAnnotatable != null)
                {
                    result = baseAnnotatable.GetAnnotation<T>();
                }
            }

            return result;
        }

        /// <summary>
        /// Marks the annotatable object such that it will inherit annotations from a base annotatable.
        /// </summary>
        /// <typeparam name="T">The type of the annotatable object to mark.</typeparam>
        /// <param name="annotatable">The annotatable object to mark.</param>
        /// <param name="baseAnnotatable">The base annotatable to inherit annotations from.</param>
        /// <returns>The annotatable object itself for composability.</returns>
        public static T InheritAnnotationsFrom<T>(this T annotatable, ODataAnnotatable baseAnnotatable) where T : ODataAnnotatable
        {
            ExceptionUtilities.CheckArgumentNotNull(annotatable, "annotatable");
            ExceptionUtilities.CheckArgumentNotNull(baseAnnotatable, "baseAnnotatable");
            ExceptionUtilities.Assert(
                annotatable.GetAnnotation<ODataAnnotatable>() == null,
                "Can't mark instance to inherit annotations for a second time.");

            annotatable.SetAnnotation(baseAnnotatable);
            return annotatable;
        }

        /// <summary>
        /// Sets annotation on a specified annotatable. Simple helper useful for composability.
        /// </summary>
        /// <typeparam name="TAnnotatable">The type of the annotatable to set the annotation on.</typeparam>
        /// <typeparam name="TAnnotation">The type of the annotation to set.</typeparam>
        /// <param name="annotatable">The annotatable to set the annotation on.</param>
        /// <param name="annotation">The annotation instance to set.</param>
        /// <returns>The annotatable instance for composability.</returns>
        public static TAnnotatable SetInheritedAnnotation<TAnnotatable, TAnnotation>(this TAnnotatable annotatable, TAnnotation annotation) 
            where TAnnotatable : ODataAnnotatable where TAnnotation : class
        {
            ExceptionUtilities.CheckArgumentNotNull(annotatable, "annotatable");
            annotatable.SetAnnotation(annotation);
            return annotatable;
        }
    }
}