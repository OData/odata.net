//---------------------------------------------------------------------
// <copyright file="ReorderMetadataPropertiesJsonTransform.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.PayloadTransformation
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Payload transform class for changing the position of __metadata property and moving it to the end.
    /// </summary>
    public class ReorderMetadataPropertiesJsonTransform : IPayloadTransform<JsonValue>
    {
        /// <summary>
        /// Transforms the original payload by moving json __metadata property to the end.
        /// </summary>
        /// <param name="originalPayload">Original payload.</param>
        /// <param name="transformedPayload">Transformed payload.</param>
        /// <returns>True if the payload is transformed else returns false.</returns>
        public bool TryTransform(JsonValue originalPayload, out JsonValue transformedPayload)
        {
            ExceptionUtilities.CheckObjectNotNull(originalPayload, "Payload cannot be null.");

            transformedPayload = null;
            JsonObject transformedJsonObject = null;

            if (originalPayload.GetType() == typeof(JsonObject))
            {
                IEnumerable<JsonProperty> properties = (originalPayload as JsonObject).Properties;
                AddProperties(properties, out transformedJsonObject);
                
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
                        AddProperties(((JsonObject)(jsonValue)).Properties, out transformedJsonObject);
                    }

                    if (transformedJsonObject != null)
                    {
                        transformedJsonArray.Add(transformedJsonObject);
                    }
                }

                if (transformedJsonArray.Elements.Count > 0)
                {
                    transformedPayload = transformedJsonArray;
                }
            }

            return (transformedPayload != null) ? true : false;
        }

        /// <summary>
        /// Adds json properties to the transformed payload after changing the position of properties.
        /// </summary>
        /// <param name="properties">Properties to add.</param>
        /// <param name="transformedJsonObject">Transformed json payload with properties in a different order.</param>
        private void AddProperties(IEnumerable<JsonProperty> properties, out JsonObject transformedJsonObject)
        {
            JsonProperty metadataProperty = null;
            transformedJsonObject = new JsonObject();

            foreach (JsonProperty property in properties)
            {
                if (property.Name.Equals("__metadata"))
                {
                    metadataProperty = property;
                }
                else
                {
                    transformedJsonObject.Add(property);
                }
            }

            if (metadataProperty != null)
            {
                transformedJsonObject.Add(metadataProperty);
            }
            else
            {
                transformedJsonObject = null;
            }
        }
    }
}
