//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Data.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Reads payload as untyped object.
    /// </summary>   
    internal sealed class ODataJsonLightGeneralDeserializer : ODataJsonLightDeserializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightInputContext">The JsonLight input context to read from.</param>
        internal ODataJsonLightGeneralDeserializer(ODataJsonLightInputContext jsonLightInputContext)
            : base(jsonLightInputContext)
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Reads untyped value.
        /// </summary>
        /// <returns>primitive value or ODataComplexValue orODataCollectionValue.</returns>
        public object ReadValue()
        {
            if (this.JsonReader.NodeType == JsonNodeType.PrimitiveValue)
            {
                return this.JsonReader.ReadPrimitiveValue();
            }

            if (this.JsonReader.NodeType == JsonNodeType.StartObject)
            {
                return this.ReadAsComplexValue();
            }

            if (this.JsonReader.NodeType == JsonNodeType.StartArray)
            {
                return this.ReadAsCollectionValue();
            }

            return null;
        }

        /// <summary>
        /// Reads json object value.
        /// </summary>
        /// <returns>ODataComplexValue with TypeName=null</returns>
        private ODataComplexValue ReadAsComplexValue()
        {
            this.JsonReader.ReadStartObject();
            List<ODataProperty> propertyies = new List<ODataProperty>();
            while (this.JsonReader.NodeType != JsonNodeType.EndObject)
            {
                string propertyName = this.JsonReader.ReadPropertyName();
                object propertyValue = this.ReadValue();
                propertyies.Add(new ODataProperty()
                {
                    Name = propertyName,
                    Value = propertyValue
                });
            }

            this.JsonReader.ReadEndObject();
            return new ODataComplexValue()
            {
                Properties = propertyies,
                TypeName = null
            };
        }

        /// <summary>
        /// Read json array.
        /// </summary>
        /// <returns>ODataCollectionValue with TypeName=null</returns>
        private ODataCollectionValue ReadAsCollectionValue()
        {
            this.JsonReader.ReadStartArray();
            List<object> items = new List<object>();
            while (this.JsonReader.NodeType != JsonNodeType.EndArray)
            {
                object val = this.ReadValue();
                items.Add(val);
            }

            this.JsonReader.ReadEndArray();
            return new ODataCollectionValue()
            {
                Items = items,
                TypeName = null
            };
        }
    }
}
