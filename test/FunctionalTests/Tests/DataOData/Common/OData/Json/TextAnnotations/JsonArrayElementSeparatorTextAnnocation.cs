//---------------------------------------------------------------------
// <copyright file="JsonArrayElementSeparatorTextAnnocation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Json.TextAnnotations
{
    #region Namespaces
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    #endregion Namespaces

    /// <summary>
    /// Annotation for an element of an array. This stored the array seprator (comma) BEFORE the element in question.
    /// </summary>
    public class JsonArrayElementSeparatorTextAnnotation : JsonTextAnnotation
    {
        /// <summary>
        /// Returns the default text annotation for the array element.
        /// </summary>
        /// <param name="firstElement">True if this is for the first element in the array, otherwise false.</param>
        /// <returns>The default text representation of the array element.</returns>
        public static JsonArrayElementSeparatorTextAnnotation GetDefault(bool firstElement)
        {
            return new JsonArrayElementSeparatorTextAnnotation() { Text = firstElement ? "" : "," };
        }
    }
}
