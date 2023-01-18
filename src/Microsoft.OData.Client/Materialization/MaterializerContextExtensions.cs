//---------------------------------------------------------------------
// <copyright file="MaterializerContextExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Materialization
{
    internal static class MaterializerContextExtensions
    {
        /// <summary>
        /// Associates the specified <paramref name="value"/> with the specified
        /// <paramref name="annotatable"/> to store metadata used for materialization.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="materializerContext">The materializer context.</param>
        /// <param name="annotatable">The item to annotate.</param>
        /// <param name="value">The annotation value.</param>
        public static void SetAnnotation<T>(this IODataMaterializerContext materializerContext, ODataAnnotatable annotatable, T value) where T : class
        {
            materializerContext.MaterializerCache.SetAnnotation(annotatable, value);
        }

        /// <summary>
        /// Retrieves the value associated with the specified <paramref name="annotatable"/>.
        /// </summary>
        /// <typeparam name="T">The expected type of the annotation value.</typeparam>
        /// <param name="materializerContext">The materializer context.</param>
        /// <param name="annotatable">The item for which to retrieve the annotation.</param>
        /// <returns>The annotation value associated with the <paramref name="annotatable"/> if it exists, or null otherwise.</returns>
        public static T GetAnnotation<T>(this IODataMaterializerContext materializerContext, ODataAnnotatable annotatable) where T : class
        {
            return materializerContext.MaterializerCache.GetAnnotation<T>(annotatable);
        }
    }
}
