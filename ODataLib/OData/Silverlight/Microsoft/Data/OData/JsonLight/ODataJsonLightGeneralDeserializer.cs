//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
