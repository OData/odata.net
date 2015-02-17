//---------------------------------------------------------------------
// <copyright file="JsonPropertySeparatorTextAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Json.TextAnnotations
{
    #region Namespaces
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    #endregion Namespaces

    /// <summary>
    /// Annotation for the separator between properties in an object - stores the command ',' character.
    /// </summary>
    public class JsonPropertySeparatorTextAnnotation : JsonTextAnnotation
    {
        /// <summary>
        /// Returns the default text annotation for the property separator.
        /// </summary>
        /// <param name="firstProperty">True if this is for the first property in the object, otherwise false.</param>
        /// <returns>The default text representation of the property serparator.</returns>
        public static JsonPropertySeparatorTextAnnotation GetDefault(bool firstProperty)
        {
            return new JsonPropertySeparatorTextAnnotation() { Text = firstProperty ? "" : "," };
        }
    }
}
