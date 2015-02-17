//---------------------------------------------------------------------
// <copyright file="JsonEndArrayTextAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Json.TextAnnotations
{
    #region Namespaces
    using System.IO;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    #endregion Namespaces

    /// <summary>
    /// Annotation for end of the array - stores the end bracket
    /// </summary>
    public class JsonEndArrayTextAnnotation : JsonTextAnnotation
    {
        /// <summary>
        /// Returns the default text annotation for the end array.
        /// </summary>
        /// <returns>The default text representation of the end array.</returns>
        public static JsonEndArrayTextAnnotation GetDefault(TextWriter writer)
        {
            return new JsonEndArrayTextAnnotation() { Text = writer.NewLine + "]" };
        }
    }
}
