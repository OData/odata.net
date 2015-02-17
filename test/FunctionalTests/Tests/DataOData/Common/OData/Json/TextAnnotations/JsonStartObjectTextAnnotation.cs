//---------------------------------------------------------------------
// <copyright file="JsonStartObjectTextAnnotation.cs" company="Microsoft">
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
    /// Annotation for start of the object - stores the start curly bracket
    /// </summary>
    public class JsonStartObjectTextAnnotation : JsonTextAnnotation
    {
        /// <summary>
        /// Returns the default text annotation for the start object.
        /// </summary>
        /// <returns>The default text representation of the start object.</returns>
        public static JsonStartObjectTextAnnotation GetDefault(TextWriter textWriter)
        {
            return new JsonStartObjectTextAnnotation() { Text = "{" + textWriter.NewLine };
        }
    }
}
