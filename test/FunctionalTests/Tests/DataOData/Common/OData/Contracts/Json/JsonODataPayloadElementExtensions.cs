//---------------------------------------------------------------------
// <copyright file="JsonODataPayloadElementExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Contracts.Json
{
    #region Namespaces
    using System.IO;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Helper extension methods to annotate ODataPayloadElement with JSON specific representations.
    /// </summary>
    public static class JsonODataPayloadElementExtensions
    {
        /// <summary>
        /// Adds a JSON specific representaion annotation to the specified ODataPayloadElement.
        /// </summary>
        /// <typeparam name="T">The type of the payload element to work on.</typeparam>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="json">The JSON value to annotate with.</param>
        /// <returns>The annotated payload element, for composability.</returns>
        public static T JsonRepresentation<T>(this T payloadElement, JsonValue json) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            JsonPayloadElementRepresentationAnnotation annotation = null;
            if (json != null)
            {
                annotation = new JsonPayloadElementRepresentationAnnotation() { Json = json };
            }

            payloadElement.SetAnnotation(annotation);
            return payloadElement;
        }

        /// <summary>
        /// Adds a JSON specific representaion annotation to the specified ODataPayloadElement.
        /// </summary>
        /// <typeparam name="T">The type of the payload element to work on.</typeparam>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="json">The JSON text to annotate with.</param>
        /// <returns>The annotated payload element, for composability.</returns>
        public static T JsonRepresentation<T>(this T payloadElement, string json) where T : ODataPayloadElement
        {
            JsonValue jsonValue = null;
            if (json != null)
            {
                jsonValue = JsonTextPreservingParser.ParseValue(new StringReader(json));
            }

            return payloadElement.JsonRepresentation(jsonValue);
        }
    }
}