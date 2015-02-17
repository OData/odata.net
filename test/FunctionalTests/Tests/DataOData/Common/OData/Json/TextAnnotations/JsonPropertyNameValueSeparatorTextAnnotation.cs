//---------------------------------------------------------------------
// <copyright file="JsonPropertyNameValueSeparatorTextAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Json.TextAnnotations
{
    #region Namespaces
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    #endregion Namespaces

    /// <summary>
    /// Annotation for the separator between property name and value - stores the colon ':' character.
    /// </summary>
    public class JsonPropertyNameValueSeparatorTextAnnotation : JsonTextAnnotation
    {
        /// <summary>
        /// Returns the default text annotation for the end object.
        /// </summary>
        /// <returns>The default text representation of the end object.</returns>
        public static JsonPropertyNameValueSeparatorTextAnnotation GetDefault()
        {
            return new JsonPropertyNameValueSeparatorTextAnnotation() { Text = ":" };
        }
    }
}
