//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Xml;
    using Microsoft.OData.Core.Json;
    using Microsoft.Spatial;
    #endregion

    /// <summary>
    /// Handles serialization and deserialization for a specified set of primitive types.
    /// </summary>
    internal sealed class PrimitiveConverter
    {
        /// <summary>Instance of GeographyTypeConverter to register for all Geography types.</summary>
        private static readonly IPrimitiveTypeConverter geographyTypeConverter = new GeographyTypeConverter();

        /// <summary>Instance of GeographyTypeConverter to register for all Geography types.</summary>
        private static readonly IPrimitiveTypeConverter geometryTypeConverter = new GeometryTypeConverter();

        /// <summary>Set of type converters that implement their own conversion using IPrimitiveTypeConverter.</summary>
        private static readonly PrimitiveConverter primitiveConverter =
            new PrimitiveConverter(
                new KeyValuePair<Type, IPrimitiveTypeConverter>[]
                {
                    new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof(GeographyPoint), geographyTypeConverter),
                    new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof(GeographyLineString), geographyTypeConverter),
                    new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof(GeographyPolygon), geographyTypeConverter),
                    new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof(GeographyCollection), geographyTypeConverter),
                    new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof(GeographyMultiPoint), geographyTypeConverter),
                    new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof(GeographyMultiLineString), geographyTypeConverter),
                    new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof(GeographyMultiPolygon), geographyTypeConverter),
                    new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof(Geography), geographyTypeConverter),

                    new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof(GeometryPoint), geometryTypeConverter),
                    new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof(GeometryLineString), geometryTypeConverter),
                    new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof(GeometryPolygon), geometryTypeConverter),
                    new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof(GeometryCollection), geometryTypeConverter),
                    new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof(GeometryMultiPoint), geometryTypeConverter),
                    new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof(GeometryMultiLineString), geometryTypeConverter),
                    new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof(GeometryMultiPolygon), geometryTypeConverter),
                    new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof(Geometry), geometryTypeConverter),
                });

        /// <summary>Set of type converters that are known to this instance which convert values based on the ISpatial type.</summary>
        private readonly Dictionary<Type, IPrimitiveTypeConverter> spatialPrimitiveTypeConverters;

        /// <summary>
        /// Create a new instance of the converter.
        /// </summary>
        /// <param name="spatialPrimitiveTypeConverters">Set of type converters to register for the ISpatial based values.</param>
        internal PrimitiveConverter(KeyValuePair<Type, IPrimitiveTypeConverter>[] spatialPrimitiveTypeConverters)
        {
            Debug.Assert(spatialPrimitiveTypeConverters != null && spatialPrimitiveTypeConverters.Length != 0, "PrimitiveConverter requires a non-null and non-empty array of type converters for ISpatial.");
            this.spatialPrimitiveTypeConverters = new Dictionary<Type, IPrimitiveTypeConverter>(EqualityComparer<Type>.Default);
            foreach (KeyValuePair<Type, IPrimitiveTypeConverter> spatialPrimitiveTypeConverter in spatialPrimitiveTypeConverters)
            {
                this.spatialPrimitiveTypeConverters.Add(spatialPrimitiveTypeConverter.Key, spatialPrimitiveTypeConverter.Value);
            }
        }

        /// <summary>PrimitiveConverter instance for use by the Atom and Json readers and writers.</summary>
        internal static PrimitiveConverter Instance
        {
            get
            {
                return primitiveConverter;
            }
        }

        /// <summary>
        /// Try to create an object of type <paramref name="targetType"/> from the value in <paramref name="reader" />.
        /// </summary>
        /// <param name="reader">XmlReader to use to read the value.</param>
        /// <param name="targetType">Expected type of the value in the reader.</param>
        /// <param name="tokenizedPropertyValue">Object of type <paramref name="targetType"/>, null if no object could be created.</param>
        /// <returns>True if the value was converted to the specified type, otherwise false.</returns>
        internal bool TryTokenizeFromXml(XmlReader reader, Type targetType, out object tokenizedPropertyValue)
        {
            tokenizedPropertyValue = null;

            Debug.Assert(reader != null, "Expected a non-null XmlReader.");
            Debug.Assert(reader.NodeType == XmlNodeType.Element, "Expected the reader to be at the start of an element.");
            Debug.Assert(targetType != null, "Expected a valid type to convert value.");

            IPrimitiveTypeConverter primitiveTypeConverter;
            if (this.TryGetConverter(targetType, out primitiveTypeConverter))
            {
                tokenizedPropertyValue = primitiveTypeConverter.TokenizeFromXml(reader);
                Debug.Assert(reader.NodeType == XmlNodeType.EndElement, "Expected reader to be at the end of an element");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Try to write the XML representation of <paramref name="instance"/> to the specified <paramref name="writer"/>
        /// </summary>
        /// <param name="instance">Object to convert to XML representation.</param>
        /// <param name="writer">XmlWriter to use to write the converted value.</param>
        /// <returns>True if the value was written, otherwise false.</returns>
        internal bool TryWriteAtom(object instance, XmlWriter writer)
        {
            Debug.Assert(instance != null, "Expected a non-null instance to write.");
            Debug.Assert(writer != null, "Expected a non-null XmlWriter.");

            return this.TryWriteValue(instance, ptc => ptc.WriteAtom(instance, writer));
        }

        /// <summary>
        /// Try to write the text representation of <paramref name="instance"/> to the specified <paramref name="writer"/>
        /// </summary>
        /// <param name="instance">Object to convert to text representation.</param>
        /// <param name="writer">TextWriter to use to write the converted value.</param>
        /// <returns>True if the value was written, otherwise false.</returns>
        internal bool TryWriteAtom(object instance, TextWriter writer)
        {
            Debug.Assert(instance != null, "Expected a non-null instance to write.");
            Debug.Assert(writer != null, "Expected a non-null TextWriter.");

            return this.TryWriteValue(instance, ptc => ptc.WriteAtom(instance, writer));
        }

        /// <summary>
        /// Try to write the JSON Lite representation of <paramref name="instance"/> using a registered primitive type converter
        /// </summary>
        /// <param name="instance">Object to convert to JSON representation.</param>
        /// <param name="jsonWriter">JsonWriter instance to write to.</param>
        internal void WriteJsonLight(object instance, IJsonWriter jsonWriter)
        {
            Debug.Assert(instance != null, "Expected a non-null instance to write.");

            Type instanceType = instance.GetType();

            IPrimitiveTypeConverter primitiveTypeConverter;
            this.TryGetConverter(instanceType, out primitiveTypeConverter);
            Debug.Assert(primitiveTypeConverter != null, "primitiveTypeConverter != null");
            primitiveTypeConverter.WriteJsonLight(instance, jsonWriter);
        }

        /// <summary>
        /// Tries to write the value of object instance using a registered primitive type converter.
        /// </summary>
        /// <param name="instance">Object to write.</param>
        /// <param name="writeMethod">Method to use when writing the value, if a registered converter is found for the type.</param>
        /// <returns>True if the value was written using a registered primitive type converter, otherwise false.</returns>
        private bool TryWriteValue(object instance, Action<IPrimitiveTypeConverter> writeMethod)
        {
            Type instanceType = instance.GetType();

            IPrimitiveTypeConverter primitiveTypeConverter;
            if (this.TryGetConverter(instanceType, out primitiveTypeConverter))
            {
                writeMethod(primitiveTypeConverter);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get the primitive type converter for the given type.
        /// </summary>
        /// <param name="type">Clr type whose primitive type converter needs to be returned.</param>
        /// <param name="primitiveTypeConverter">Converter for the given clr type.</param>
        /// <returns>True if a converter was found for the given type, otherwise returns false.</returns>
        private bool TryGetConverter(Type type, out IPrimitiveTypeConverter primitiveTypeConverter)
        {
            if (typeof(ISpatial).IsAssignableFrom(type))
            {
                KeyValuePair<Type, IPrimitiveTypeConverter> bestMatch = new KeyValuePair<Type, IPrimitiveTypeConverter>(typeof(object), null);
                foreach (KeyValuePair<Type, IPrimitiveTypeConverter> possibleMatch in this.spatialPrimitiveTypeConverters)
                {
                    // If the current primitive type is assignable from the given parameter type and
                    // is a more derived type from the previous match, then the current type is a better match.
                    if (possibleMatch.Key.IsAssignableFrom(type) && bestMatch.Key.IsAssignableFrom(possibleMatch.Key))
                    {
                        bestMatch = possibleMatch;
                    }
                }

                primitiveTypeConverter = bestMatch.Value;
                return bestMatch.Value != null;
            }
            else
            {
                primitiveTypeConverter = null;
                return false;
            }
        }
    }
}

