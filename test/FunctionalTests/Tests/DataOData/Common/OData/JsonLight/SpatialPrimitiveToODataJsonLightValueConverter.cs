//---------------------------------------------------------------------
// <copyright file="SpatialPrimitiveToODataJsonLightValueConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.JsonLight
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Spatial.Contracts;

    /// <summary>
    /// Implementation to convert a spatial primitive to and OData-specific json value
    /// </summary>
    public class SpatialPrimitiveToODataJsonLightValueConverter
    {
        /// <summary>
        /// Gets or sets the converter from dictionary to json object.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDictionaryToJsonObjectConverter DictionaryConverter { get; set; }

        /// <summary>
        /// Gets or sets the geo json formatter.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IGeoJsonSpatialFormatter GeoJsonFormatter { get; set; }

        /// <summary>
        /// Gets or sets the primitive data type converter
        /// </summary>
        [InjectDependency]
        public IClrToPrimitiveDataTypeConverter PrimitiveDataTypeConverter { get; set; }

        /// <summary>
        /// Tries to convert the value if it is spatial. Adds OData-specific fields to GeoJSON microformat.
        /// </summary>
        /// <param name="value">The value that might be spatial.</param>
        /// <param name="jsonObject">The converted json object.</param>
        /// <returns>Whether the value was spatial and could be converted</returns>
        public bool TryConvertIfSpatial(PrimitiveValue value, out JsonObject jsonObject)
        {
            ExceptionUtilities.CheckArgumentNotNull(value, "value");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            IDictionary<string, object> dictionary;
            if (this.GeoJsonFormatter.TryConvert(value.ClrValue, out dictionary))
            {
                jsonObject = this.DictionaryConverter.Convert(dictionary);
                return true;
            }

            jsonObject = null;
            return false;
        }

        /// <summary>
        /// Tries to convert the value if it is spatial. Preserves any OData-specific fields.
        /// </summary>
        /// <param name="jsonObject">The json object that might be spatial.</param>
        /// <param name="value">The converted spatial value.</param>
        /// <returns>Whether the object was spatial and could be converted</returns>
        public bool TryConvertIfSpatial(JsonObject jsonObject, out PrimitiveValue value)
        {
            ExceptionUtilities.CheckArgumentNotNull(jsonObject, "jsonObject");

            string edmTypeName;
            SpatialTypeKind? expectedType = null;
            if (TryExtractMetadataTypeName(jsonObject, out edmTypeName))
            {
                ExceptionUtilities.CheckObjectNotNull(this.PrimitiveDataTypeConverter, "Cannot infer clr type from edm type without converter");
                var clrType = this.PrimitiveDataTypeConverter.ToClrType(edmTypeName);
                if (clrType == null)
                {
                    // must not be primitive, let alone spatial
                    value = null;
                    return false;
                }

                SpatialUtilities.TryInferSpatialTypeKind(clrType, out expectedType);
            }

            var dictionary = this.DictionaryConverter.Convert(jsonObject);

            object spatialInstance;
            if (this.GeoJsonFormatter.TryParse(dictionary, expectedType, out spatialInstance))
            {
                value = new PrimitiveValue(edmTypeName, spatialInstance);
                return true;
            }

            value = null;
            return false;
        }

        internal static bool TryExtractMetadataTypeName(JsonObject jsonObject, out string metadataTypeName)
        {
            metadataTypeName = null;
            var typeProperty = jsonObject.Properties.SingleOrDefault(p => p.Name == (JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName));
            if (typeProperty == null)
            {
                return false;
            }

            var typeValue = typeProperty.Value as JsonPrimitiveValue;
            if (typeValue == null)
            {
                return false;
            }

            metadataTypeName = typeValue.Value as string;
            return metadataTypeName != null;
        }
    }
}