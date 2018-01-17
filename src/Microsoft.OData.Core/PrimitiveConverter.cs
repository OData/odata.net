//---------------------------------------------------------------------
// <copyright file="PrimitiveConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Xml;
    using Microsoft.OData.Json;
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

