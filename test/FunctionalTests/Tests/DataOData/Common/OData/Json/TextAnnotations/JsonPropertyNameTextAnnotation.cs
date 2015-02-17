//---------------------------------------------------------------------
// <copyright file="JsonPropertyNameTextAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Json.TextAnnotations
{
    #region Namespaces
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    #endregion Namespaces

    /// <summary>
    /// Annotation for the property name in an object - stores the name of the property (quoted text usually)
    /// </summary>
    public class JsonPropertyNameTextAnnotation : JsonTextAnnotation
    {
        /// <summary>
        /// Returns the default text annotation for the property name.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The default text representation of the property name.</returns>
        public static JsonPropertyNameTextAnnotation GetDefault(string propertyName)
        {
            return new JsonPropertyNameTextAnnotation() { Text = "\"" + propertyName + "\"" };
        }
    }
}
