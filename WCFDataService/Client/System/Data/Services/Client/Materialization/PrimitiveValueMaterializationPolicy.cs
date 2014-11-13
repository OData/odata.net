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

namespace System.Data.Services.Client.Materialization
{
    using System.Data.Services.Client.Metadata;
    using System.Diagnostics;
    using DSClient = System.Data.Services.Client;

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
        /// <returns>true if the value was set; false if it wasn't (typically because it's a complex value).</returns>
        private bool MaterializePrimitiveDataValue(Type type, string wireTypeName, object value, Func<string> throwOnNullMessage, out object materializedValue)
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
                    materializedValue = this.PrimitivePropertyConverter.ConvertPrimitiveValue(value, underlyingType);
                }

                return true;
            }
            else
            {
                materializedValue = null;
                return false;
            }
        }
    }
}
