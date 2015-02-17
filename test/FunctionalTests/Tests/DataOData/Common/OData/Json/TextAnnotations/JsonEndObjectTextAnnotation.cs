//---------------------------------------------------------------------
// <copyright file="JsonEndObjectTextAnnotation.cs" company="Microsoft">
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
    /// Annotation for the end of the object - stores the end curly bracket
    /// </summary>
    public class JsonEndObjectTextAnnotation : JsonTextAnnotation
    {
        /// <summary>
        /// Returns the default text annotation for the end object.
        /// </summary>
        /// <returns>The default text representation of the end object.</returns>
        public static JsonEndObjectTextAnnotation GetDefault(TextWriter writer)
        {
            return new JsonEndObjectTextAnnotation() { Text = writer.NewLine + "}" };
        }
    }
}
