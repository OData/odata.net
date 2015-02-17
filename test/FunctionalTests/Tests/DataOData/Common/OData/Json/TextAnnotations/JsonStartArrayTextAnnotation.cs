//---------------------------------------------------------------------
// <copyright file="JsonStartArrayTextAnnotation.cs" company="Microsoft">
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
    /// Annotation for start of the array - stores the start bracket
    /// </summary>
    public class JsonStartArrayTextAnnotation : JsonTextAnnotation
    {
        /// <summary>
        /// Returns the default text annotation for the start array.
        /// </summary>
        /// <returns>The default text representation of the start array.</returns>
        public static JsonStartArrayTextAnnotation GetDefault(TextWriter writer)
        {
            return new JsonStartArrayTextAnnotation() { Text = "[" + writer.NewLine };
        }
    }
}
