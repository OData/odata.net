//---------------------------------------------------------------------
// <copyright file="PrimitiveValueMaterializationPolicy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Materialization
{
    using System;
    using System.Diagnostics;
    using Microsoft.OData.Client.Metadata;
    using DSClient = Microsoft.OData.Client;

    /// <summary>
    /// Creates a policy that is used for materializing Primitive values
    /// </summary>
    internal class PrimitiveValueMaterializationPolicy
    {
        /// <summary> MaterializerContext used to resolve types for materialization. </summary>
        private readonly IODataMaterializerContext context;

        /// <summary>
        /// primitive property converter used to convert the property have the value has been materialized. </summary>
        private readonly SimpleLazy<PrimitivePropertyConverter> lazyPrimitivePropertyConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveValueMaterializationPolicy" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="lazyPrimitivePropertyConverter">The lazy primitive property converter.</param>
        internal PrimitiveValueMaterializationPolicy(IODataMaterializerContext context, SimpleLazy<PrimitivePropertyConverter> lazyPrimitivePropertyConverter)
        {
            this.context = context;
            this.lazyPrimitivePropertyConverter = lazyPrimitivePropertyConverter;
        }

        /// <summary>
        /// Gets the primitive property converter.
        /// </summary>
        /// <value>
        /// The primitive property converter.
        /// </value>
        private PrimitivePropertyConverter PrimitivePropertyConverter
        {
            get { return this.lazyPrimitivePropertyConverter.Value; }
        }

        /// <summary>
        /// Materializes the primitive data value.
        /// </summary>
        /// <param name="collectionItemType">Type of the collection item.</param>
        /// <param name="wireTypeName">Name of the wire type.</param>
        /// <param name="item">The item.</param>
        /// <returns>Materialized primitive data value.</returns>
        public object MaterializePrimitiveDataValue(Type collectionItemType, string wireTypeName, object item)
        {
            object materializedValue = null;
            this.MaterializePrimitiveDataValue(collectionItemType, wireTypeName, item, () => "TODO: Is this reachable?", out materializedValue);

            return materializedValue;
        }

        /// <summary>
        /// Materializes the primitive data value collection element.
        /// </summary>
        /// <param name="collectionItemType">The collection item type.</param>
        /// <param name="wireTypeName">Name of the wire type.</param>
        /// <param name="item">The item.</param>
        /// <returns>Materialized primitive collection element value</returns>
        public object MaterializePrimitiveDataValueCollectionElement(Type collectionItemType, string wireTypeName, object item)
        {
            object materializedValue = null;
            this.MaterializePrimitiveDataValue(collectionItemType, wireTypeName, item, () => DSClient.Strings.Collection_NullCollectionItemsNotSupported, out materializedValue);

            return materializedValue;
        }

        /// <summary>Materializes a primitive value. No op or non-primitive values.</summary>
        /// <param name="type">Type of value to set.</param>
        /// <param name="wireTypeName">Type name from the payload.</param>
        /// <param name="value">Value of primitive provided by ODL.</param>
        /// <param name="throwOnNullMessage">The exception message if the value is null.</param>
        /// <param name="materializedValue">The materialized value.</param>
        private void MaterializePrimitiveDataValue(Type type, string wireTypeName, object value, Func<string> throwOnNullMessage, out object materializedValue)
        {
            Debug.Assert(type != null, "type != null");

            ClientTypeAnnotation nestedElementType = null;
            Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            PrimitiveType ptype;
            bool knownType = PrimitiveType.TryGetPrimitiveType(underlyingType, out ptype);
            if (!knownType)
            {
                nestedElementType = this.context.ResolveTypeForMaterialization(type, wireTypeName);
                Debug.Assert(nestedElementType != null, "nestedElementType != null -- otherwise ReadTypeAttribute (or someone!) should throw");
                knownType = PrimitiveType.TryGetPrimitiveType(nestedElementType.ElementType, out ptype);
            }

            if (knownType)
            {
                if (value == null)
                {
                    if (!ClientTypeUtil.CanAssignNull(type))
                    {
                        throw new InvalidOperationException(throwOnNullMessage());
                    }

                    materializedValue = null;
                }
                else
                {
                    ODataUntypedValue untypedVal = value as ODataUntypedValue;
                    if ((untypedVal != null)
                        && this.context.UndeclaredPropertyBehavior == UndeclaredPropertyBehavior.Support)
                    {
                        value = CommonUtil.ParseJsonToPrimitiveValue(untypedVal.RawValue);
                    }

                    materializedValue = this.PrimitivePropertyConverter.ConvertPrimitiveValue(value, underlyingType);
                }
            }
            else
            {
                materializedValue = null;
            }
        }
    }
}