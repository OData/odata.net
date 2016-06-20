//---------------------------------------------------------------------
// <copyright file="SpatialUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.Spatial;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Astoria.Json;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    #endregion Namespaces

    /// <summary>
    /// Helper methods to work with spatial values.
    /// </summary>
    public static class SpatialUtils
    {
        /// <summary>
        /// Converts a spatial value into a string representation suitable for the specified format. If specified, injects a type name into the JSON format(s).
        /// </summary>
        /// <param name="format">The format to create the spatial value string representation for.</param>
        /// <param name="spatial">The spatial value.</param>
        /// <param name="typeName">The (optional) type name to inject into a JSON string representation.</param>
        /// <returns>The string representation of the spatial value in the specified format.</returns>
        /// <remarks>When using a type name with the JSON Light format, the resulting string representation will be invalid 
        /// (since the odata.type annotation is not supported inside of spatial values); only used for error tests.</remarks>
        public static string GetSpatialStringValue(ODataFormat format, ISpatial spatial, string typeName = null)
        {
            IDictionary<string, object> dictionary = GeoJsonObjectFormatter.Create().Write(spatial);

            var converter = new DictionaryToJsonObjectConverter();
            ExceptionUtilities.CheckAllRequiredDependencies(converter);

            var jsonObject = converter.Convert(dictionary);
            if (typeName != null)
            {
                // NOTE: using a type name with the JSON Light format will produce invalid payloads and is only used
                //       for error tests.
                jsonObject.Insert(0, new JsonProperty(JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName, new JsonPrimitiveValue(typeName)));
            }

            return jsonObject.ToText(format == ODataFormat.Json);
        }
    }
}
