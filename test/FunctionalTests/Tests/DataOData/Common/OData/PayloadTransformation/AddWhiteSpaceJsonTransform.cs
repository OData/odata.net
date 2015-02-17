//---------------------------------------------------------------------
// <copyright file="AddWhiteSpaceJsonTransform.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.PayloadTransformation
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.OData.Json.TextAnnotations;
    #endregion Namespaces

    /// <summary>
    /// Payload transformation class for adding whitespaces to json payloads.
    /// </summary>
    public class AddWhiteSpaceJsonTransform : IPayloadTransform<JsonValue>
    {
        /// <summary>
        /// Transforms the original payload by adding whitespaces using json annotations.
        /// </summary>
        /// <param name="originalPayload">Original payload.</param>
        /// <param name="transformedPayload">Transformed payload.</param>
        /// <returns>True if the payload is transformed else returns false.</returns>
        public bool TryTransform(JsonValue originalPayload, out JsonValue transformedPayload)
        {
            if (originalPayload.GetType() == typeof(JsonObject))
            {
                JsonObject transformedJsonObject = new JsonObject();
                IEnumerable<JsonProperty> jsonProperties = (originalPayload as JsonObject).Properties;

                foreach (JsonProperty property in jsonProperties)
                {
                    SetWhiteSpaceJsonPropertyAnnotations(property);
                    transformedJsonObject.Add(property);
                }

                SetWhiteSpaceJsonObjectAnnotations(transformedJsonObject);
                transformedPayload = transformedJsonObject;
            }
            else if (originalPayload.GetType() == typeof(JsonArray))
            {
                JsonArray transformedJsonArray = new JsonArray();
                IEnumerable<JsonValue> jsonValues = (originalPayload as JsonArray).Elements;

                foreach (JsonValue jsonValue in jsonValues)
                {
                    if (jsonValue.GetType() == typeof(JsonObject))
                    {
                        SetWhiteSpaceJsonObjectAnnotations(jsonValue);

                        foreach (JsonProperty property in ((JsonObject)(jsonValue)).Properties)
                        {
                            SetWhiteSpaceJsonPropertyAnnotations(property);
                        }
                    }

                    transformedJsonArray.Add(jsonValue);
                }

                SetWhiteSpaceJsonArrayAnnotations(transformedJsonArray);
                transformedPayload = transformedJsonArray;
            }
            else
            {
                transformedPayload = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sets json object annotation text with whitespaces.
        /// </summary>
        /// <param name="transformedPayload">Transformed payload with json annotations.</param>
        private void SetWhiteSpaceJsonObjectAnnotations(JsonValue transformedPayload)
        {
            JsonEndObjectTextAnnotation jsonEndObjectTextAnnotation = new JsonEndObjectTextAnnotation();
            JsonStartObjectTextAnnotation jsonStartObjectTextAnnotation = new JsonStartObjectTextAnnotation();

            jsonEndObjectTextAnnotation.Text = "\t\n } \t\n";
            jsonStartObjectTextAnnotation.Text = "\t\n { \t\n";

            transformedPayload.SetAnnotation<JsonEndObjectTextAnnotation>(jsonEndObjectTextAnnotation);
            transformedPayload.SetAnnotation<JsonStartObjectTextAnnotation>(jsonStartObjectTextAnnotation);
        }

        /// <summary>
        /// Sets json array annotation text with whitespaces.
        /// </summary>
        /// <param name="transformedPayload">Transformed payload with json annotations.</param>
        private void SetWhiteSpaceJsonArrayAnnotations(JsonValue transformedPayload)
        {
            JsonArrayElementSeparatorTextAnnotation jsonArrayElementSeparatorTextAnnotation = new JsonArrayElementSeparatorTextAnnotation();
            JsonEndArrayTextAnnotation jsonEndArrayTextAnnotation = new JsonEndArrayTextAnnotation();
            JsonStartArrayTextAnnotation jsonStartArrayTextAnnotation = new JsonStartArrayTextAnnotation();

            jsonArrayElementSeparatorTextAnnotation.Text = "\t\n , \t\n";
            jsonEndArrayTextAnnotation.Text = "\t\n ] \t\n";
            jsonStartArrayTextAnnotation.Text = "\t\n [ \t\n";

            transformedPayload.SetAnnotation<JsonArrayElementSeparatorTextAnnotation>(jsonArrayElementSeparatorTextAnnotation);
            transformedPayload.SetAnnotation<JsonEndArrayTextAnnotation>(jsonEndArrayTextAnnotation);
            transformedPayload.SetAnnotation<JsonStartArrayTextAnnotation>(jsonStartArrayTextAnnotation);
        }

        /// <summary>
        /// Sets json property annotation text with whitespaces.
        /// </summary>
        /// <param name="transformedPayload">Transformed payload with json annotations.</param>
        private void SetWhiteSpaceJsonPropertyAnnotations(JsonValue transformedPayload)
        {
            JsonPropertyNameValueSeparatorTextAnnotation jsonPropertyNameValueSeparatorTextAnnotation = new JsonPropertyNameValueSeparatorTextAnnotation();
            jsonPropertyNameValueSeparatorTextAnnotation.Text = "\t\n : \t\n";
            transformedPayload.SetAnnotation<JsonPropertyNameValueSeparatorTextAnnotation>(jsonPropertyNameValueSeparatorTextAnnotation);
        }
    }
}
