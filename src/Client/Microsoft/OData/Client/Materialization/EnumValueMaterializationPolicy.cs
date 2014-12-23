//   OData .NET Libraries ver. 6.9
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

namespace Microsoft.OData.Client.Materialization
{
    using System;
    using System.Diagnostics;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData.Core;
    using DSClient = Microsoft.OData.Client;

    /// <summary>
    /// Creates a policy that is used for materializing Enum values
    /// </summary>
    internal class EnumValueMaterializationPolicy
    {
        /// <summary> MaterializerContext used to resolve types for materialization. </summary>
        private readonly IODataMaterializerContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumValueMaterializationPolicy" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        internal EnumValueMaterializationPolicy(IODataMaterializerContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Materializes the enum data value.
        /// </summary>
        /// <param name="valueType">Type of the collection item.</param>
        /// <param name="property">The ODataProperty.</param>
        /// <returns>Materialized enum data CLR value.</returns>
        public object MaterializeEnumTypeProperty(Type valueType, ODataProperty property)
        {
            object materializedValue = null;
            ODataEnumValue value = property.Value as ODataEnumValue;
            this.MaterializeODataEnumValue(valueType, value.TypeName, value.Value, () => "TODO: Is this reachable?", out materializedValue);
            if (!property.HasMaterializedValue())
            {
                property.SetMaterializedValue(materializedValue);
            }

            return materializedValue;
        }

        /// <summary>
        /// Materializes the enum data value collection element.
        /// </summary>
        /// <param name="collectionItemType">The collection item type.</param>
        /// <param name="wireTypeName">Name of the wire type.</param>
        /// <param name="item">The ODataEnumValue object.</param>
        /// <returns>Materialized enum collection element value</returns>
        public object MaterializeEnumDataValueCollectionElement(Type collectionItemType, string wireTypeName, string item)
        {
            object materializedValue = null;
            this.MaterializeODataEnumValue(collectionItemType, wireTypeName, item, () => DSClient.Strings.Collection_NullCollectionItemsNotSupported, out materializedValue);

            return materializedValue;
        }

        /// <summary>Materializes an enum value. No op or non-primitive values.</summary>
        /// <param name="enumType">The clr Type of value to set.</param>
        /// <param name="enumValue">The value of enum.</param>
        /// <returns>The materialized value.</returns>
        internal static object MaterializeODataEnumValue(Type enumType, ODataEnumValue enumValue)
        {
            object tmpValue;
            if (enumValue == null)
            {
                tmpValue = null;
            }
            else
            {
                // TODO: Find better way to parse Enum
                string enumValueStr = enumValue.Value.Trim();
                if (!Enum.IsDefined(enumType, enumValueStr))
                {
                    tmpValue = Enum.Parse(enumType, ClientTypeUtil.GetClientFieldName(enumType, enumValueStr), false);
                }
                else
                {
                    tmpValue = Enum.Parse(enumType, enumValueStr, false);
                }
            }

            return tmpValue;
        }

        /// <summary>Materializes an enum value. No op or non-primitive values.</summary>
        /// <param name="type">The clr Type of value to set.</param>
        /// <param name="wireTypeName">The Type name from the payload.</param>
        /// <param name="enumValueStr">The string of enum value.</param>
        /// <param name="throwOnNullMessage">The exception message if the value is null.</param>
        /// <param name="materializedValue">The materialized value.</param>
        private void MaterializeODataEnumValue(Type type, string wireTypeName, string enumValueStr, Func<string> throwOnNullMessage, out object materializedValue)
        {
            Debug.Assert(type != null, "type != null");
            Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            ClientTypeAnnotation elementTypeAnnotation = this.context.ResolveTypeForMaterialization(underlyingType, wireTypeName);
            Debug.Assert(elementTypeAnnotation != null, "elementTypeAnnotation != null");

            // TODO: Find better way to parse Enum
            Type enumClrType = elementTypeAnnotation.ElementType;
            enumValueStr = enumValueStr.Trim();
            if (!Enum.IsDefined(enumClrType, enumValueStr))
            {
                materializedValue = Enum.Parse(enumClrType, ClientTypeUtil.GetClientFieldName(enumClrType, enumValueStr), false);
            }
            else
            {
                materializedValue = Enum.Parse(enumClrType, enumValueStr, false);
            }
        }
    }
}
