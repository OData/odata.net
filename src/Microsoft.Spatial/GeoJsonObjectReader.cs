//---------------------------------------------------------------------
// <copyright file="GeoJsonObjectReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// The spatial reader that can read from a pre parsed GeoJson payload
    /// </summary>
    internal class GeoJsonObjectReader : SpatialReader<IDictionary<string, object>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeoJsonObjectReader"/> class.
        /// </summary>
        /// <param name="destination">The pipeline.</param>
        internal GeoJsonObjectReader(SpatialPipeline destination)
            : base(destination)
        {
        }

        /// <summary>
        /// Parses some serialized format that represents a geography value, passing the result down the pipeline.
        /// </summary>
        /// <param name="input">The jsonObject to read from.</param>
        protected override void ReadGeographyImplementation(IDictionary<string, object> input)
        {
            TypeWashedPipeline pipeline = new TypeWashedToGeographyLongLatPipeline(Destination);
            new SendToTypeWashedPipeline(pipeline).SendToPipeline(input, true);
        }

        /// <summary>
        /// Parses some serialized format that represents a geometry value, passing the result down the pipeline.
        /// </summary>
        /// <param name="input">The jsonObject to read from.</param>
        protected override void ReadGeometryImplementation(IDictionary<string, object> input)
        {
            TypeWashedPipeline pipeline = new TypeWashedToGeometryPipeline(Destination);
            new SendToTypeWashedPipeline(pipeline).SendToPipeline(input, true);
        }

        /// <summary>
        /// A common way to call Geography and Geometry pipeline apis from the structured Json
        /// </summary>
        private class SendToTypeWashedPipeline
        {
            /// <summary>
            /// Pipeline to use for the output of the translation of the GeoJSON object into pipeline method calls.
            /// </summary>
            private TypeWashedPipeline pipeline;

            /// <summary>
            /// Initializes a new instance of the <see cref="SendToTypeWashedPipeline"/> class.
            /// </summary>
            /// <param name="pipeline">Spatial pipeline that will receive the pipeline method calls.</param>
            internal SendToTypeWashedPipeline(TypeWashedPipeline pipeline)
            {
                this.pipeline = pipeline;
            }

            /// <summary>
            /// Translates a dictionary of parsed GeoJSON members and values into method calls on the spatial pipeline.
            /// </summary>
            /// <param name="members">Dictionary containing GeoJSON members and values.</param>
            /// <param name="requireSetCoordinates">Coordinate System must be set for this pipeline</param>
            internal void SendToPipeline(IDictionary<string, object> members, bool requireSetCoordinates)
            {
                SpatialType spatialType = GetSpatialType(members);

                int? epsgId;
                if (!TryGetCoordinateSystemId(members, out epsgId))
                {
                    epsgId = null;
                }

                if (requireSetCoordinates || epsgId != null)
                {
                    // When to set coordinate system:
                    // 1. On outer spatial types, coordinate system must be set (requireSetCoordinates = true)
                    // if the other spatial type does not declare a CRS, set to null.
                    // 2. On inner spatial types, set coordinate system ONLY if the CRS is declared on the type
                    // If the CRS is different from the other one, validator will throw. However, it's still valid GeoJSON.
                    this.pipeline.SetCoordinateSystem(epsgId);
                }

                String contentMemberName;

                if (spatialType == SpatialType.Collection)
                {
                    contentMemberName = GeoJsonConstants.GeometriesMemberName;
                }
                else
                {
                    contentMemberName = GeoJsonConstants.CoordinatesMemberName;
                }

                IEnumerable memberObject = GetMemberValueAsJsonArray(members, contentMemberName);
                SendShape(spatialType, memberObject);
            }

            /// <summary>
            /// Iterates over an object array, verifies that each element in the array is another array, and calls a delgate on the contained array.
            /// </summary>
            /// <param name="array">Array to iterate over.</param>
            /// <param name="send">Delegate to invoke for each element once it has been validated to be an array.</param>
            private static void SendArrayOfArray(IEnumerable array, Action<IEnumerable> send)
            {
                foreach (object element in array)
                {
                    IEnumerable arrayElement = ValueAsJsonArray(element);
                    send(arrayElement);
                }
            }

            /// <summary>
            /// Convert an object to a nullable double value.
            /// </summary>
            /// <param name="value">Object to convert.</param>
            /// <returns>If the specified element was null, returns null, otherwise returns the converted double value.</returns>
            private static double? ValueAsNullableDouble(object value)
            {
                return value == null ? null : (double?)ValueAsDouble(value);
            }

            /// <summary>
            /// Convert an object to a non-null double value.
            /// </summary>
            /// <param name="value">Object to convert.</param>
            /// <returns>Converted double value.</returns>
            private static double ValueAsDouble(object value)
            {
                if (value == null)
                {
                    throw new FormatException(Strings.GeoJsonReader_InvalidNullElement);
                }

                // At this point we are expecting them to only be numeric, so verify that.
                if (value is String || value is IDictionary<string, object> || value is IEnumerable || value is bool)
                {
                    throw new FormatException(Strings.GeoJsonReader_ExpectedNumeric);
                }

                // value is already a numeric value at this point, so can safely convert it using InvariantCulture.
                return Convert.ToDouble(value, CultureInfo.InvariantCulture);
            }

            /// <summary>
            /// Values as json array.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns>The value cast as a json array.</returns>
            private static IEnumerable ValueAsJsonArray(object value)
            {
                if (value == null)
                {
                    return null;
                }

                // need to be sure it isn't a string because string is also enumerable);
                if (value is string)
                {
                    throw new FormatException(Strings.GeoJsonReader_ExpectedArray);
                }

                if (value is IDictionary || value is IDictionary<string, object>)
                {
                    // These are typically signatures of json objects though they can be looked at as IEnumerable,
                    // but that would be a mistake, becuase we wouldn't know how to interpret the KeyValuePair
                    throw new FormatException(Strings.GeoJsonReader_ExpectedArray);
                }

                IEnumerable array = value as IEnumerable;
                if (array != null)
                {
                    return array;
                }

                throw new FormatException(Strings.GeoJsonReader_ExpectedArray);
            }

            /// <summary>
            /// Values as json object.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns>The value cast as IDictionary&lt;string, object&gt;</returns>
            private static IDictionary<string, object> ValueAsJsonObject(object value)
            {
                if (value == null)
                {
                    return null;
                }

                IDictionary<string, object> castValue = value as IDictionary<string, object>;
                if (castValue != null)
                {
                    return castValue;
                }

                throw new FormatException(Strings.JsonReaderExtensions_CannotReadValueAsJsonObject(value));
            }

            /// <summary>
            /// Values as string.
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="value">The value.</param>
            /// <returns>The value cast as a string.</returns>
            private static string ValueAsString(string propertyName, object value)
            {
                if (value == null)
                {
                    return null;
                }

                string castValue = value as string;
                if (castValue != null)
                {
                    return castValue;
                }

                throw new FormatException(Strings.JsonReaderExtensions_CannotReadPropertyValueAsString(value, propertyName));
            }

            /// <summary>
            /// Get the type member value from the specified GeoJSON member dictionary.
            /// </summary>
            /// <param name="geoJsonObject">Dictionary containing the GeoJSON members and their values.</param>
            /// <returns>SpatialType for the GeoJSON object.</returns>
            private static SpatialType GetSpatialType(IDictionary<string, object> geoJsonObject)
            {
                object typeName;
                if (geoJsonObject.TryGetValue(GeoJsonConstants.TypeMemberName, out typeName))
                {
                    return ReadTypeName(ValueAsString(GeoJsonConstants.TypeMemberName, typeName));
                }
                else
                {
                    throw new FormatException(Strings.GeoJsonReader_MissingRequiredMember(GeoJsonConstants.TypeMemberName));
                }
            }

            /// <summary>
            /// Tries to get a coordinate system id from the geo json object's 'crs' property
            /// </summary>
            /// <param name="geoJsonObject">The geo json object.</param>
            /// <param name="epsgId">The coordinate system id.</param>
            /// <returns>True if the object had a coordinate system</returns>
            private static bool TryGetCoordinateSystemId(IDictionary<string, object> geoJsonObject, out int? epsgId)
            {
                object crsValue;
                if (!geoJsonObject.TryGetValue(GeoJsonConstants.CrsMemberName, out crsValue))
                {
                    epsgId = null;
                    return false;
                }

                IDictionary<string, object> crsMembers = ValueAsJsonObject(crsValue);
                epsgId = GetCoordinateSystemIdFromCrs(crsMembers);
                return true;
            }

            /// <summary>
            /// Gets the coordinate system ID from a representation of the CRS object
            /// </summary>
            /// <param name="crsJsonObject">The parsed representation of the CRS object.</param>
            /// <returns>The coordinate system ID</returns>
            private static int GetCoordinateSystemIdFromCrs(IDictionary<string, object> crsJsonObject)
            {
                // get the value of the 'type' property
                object typeValue;
                if (!crsJsonObject.TryGetValue(GeoJsonConstants.TypeMemberName, out typeValue))
                {
                    throw new FormatException(Strings.GeoJsonReader_MissingRequiredMember(GeoJsonConstants.TypeMemberName));
                }

                // we previously validated that the value was a string, so this cast is safe
                var typeString = ValueAsString(GeoJsonConstants.TypeMemberName, typeValue);

                // validate the type is supported
                if (!string.Equals(typeString, GeoJsonConstants.CrsTypeMemberValue, StringComparison.Ordinal))
                {
                    throw new FormatException(Strings.GeoJsonReader_InvalidCrsType(typeString));
                }

                // get the value of the 'properties' property
                object propertiesValue;
                if (!crsJsonObject.TryGetValue(GeoJsonConstants.CrsPropertiesMemberName, out propertiesValue))
                {
                    throw new FormatException(Strings.GeoJsonReader_MissingRequiredMember(GeoJsonConstants.CrsPropertiesMemberName));
                }

                var properties = ValueAsJsonObject(propertiesValue);

                // get the value of the 'name' property
                object nameValue;
                if (!properties.TryGetValue(GeoJsonConstants.CrsNameMemberName, out nameValue))
                {
                    throw new FormatException(Strings.GeoJsonReader_MissingRequiredMember(GeoJsonConstants.CrsNameMemberName));
                }

                // we previously validated that the value was a string
                var nameString = ValueAsString(GeoJsonConstants.CrsNameMemberName, nameValue);

                // the value of 'name' must be of the form 'EPSG:1234' where 1234 is a valid integer
                var offset = GeoJsonConstants.CrsValuePrefix.Length;
                int epsgId;
                if (nameString == null
                    || !nameString.StartsWith(GeoJsonConstants.CrsValuePrefix, StringComparison.Ordinal)
                    || nameString.Length == offset  // just EPSG, no : or value
                    || nameString[offset] != ':'
                    || !int.TryParse(nameString.Substring(offset + 1), out epsgId))
                {
                    throw new FormatException(Strings.GeoJsonReader_InvalidCrsName(nameString));
                }

                return epsgId;
            }

            /// <summary>
            /// Get the designated member value from the specified GeoJSON member dictionary.
            /// </summary>
            /// <param name="geoJsonObject">Dictionary containing the GeoJSON members and their values.</param>
            /// <param name="memberName">The member's tag name</param>
            /// <returns>Member value for the GeoJSON object.</returns>
            private static IEnumerable GetMemberValueAsJsonArray(IDictionary<string, object> geoJsonObject, String memberName)
            {
                object coordinates;
                if (geoJsonObject.TryGetValue(memberName, out coordinates))
                {
                    return ValueAsJsonArray(coordinates);
                }
                else
                {
                    throw new FormatException(Strings.GeoJsonReader_MissingRequiredMember(memberName));
                }
            }

            /// <summary>
            /// This method assumes a non forward only enumerable
            /// </summary>
            /// <param name="enumerable">The enumerable to check</param>
            /// <returns>true if there is at least one element</returns>
            private static bool EnumerableAny(IEnumerable enumerable)
            {
                IEnumerator enumerator = enumerable.GetEnumerator();
                return enumerator.MoveNext();
            }

            /// <summary>
            /// Reads GeoJson 'type' value and maps it a valid SpatialType.
            /// </summary>
            /// <param name="typeName">The GeoJson standard type name</param>
            /// <returns>SpatialType corresponding to the GeoJson type name.</returns>
            private static SpatialType ReadTypeName(string typeName)
            {
                switch (typeName)
                {
                    case GeoJsonConstants.TypeMemberValuePoint:
                        return SpatialType.Point;
                    case GeoJsonConstants.TypeMemberValueLineString:
                        return SpatialType.LineString;
                    case GeoJsonConstants.TypeMemberValuePolygon:
                        return SpatialType.Polygon;
                    case GeoJsonConstants.TypeMemberValueMultiPoint:
                        return SpatialType.MultiPoint;
                    case GeoJsonConstants.TypeMemberValueMultiLineString:
                        return SpatialType.MultiLineString;
                    case GeoJsonConstants.TypeMemberValueMultiPolygon:
                        return SpatialType.MultiPolygon;
                    case GeoJsonConstants.TypeMemberValueGeometryCollection:
                        return SpatialType.Collection;
                    default:
                        throw new FormatException(Strings.GeoJsonReader_InvalidTypeName(typeName));
                }
            }

            /// <summary>
            /// Sends a shape to the spatial pipeline.
            /// </summary>
            /// <param name="spatialType">SpatialType of the shape.</param>
            /// <param name="contentMembers">Content member for the shape</param>
            private void SendShape(SpatialType spatialType, IEnumerable contentMembers)
            {
                this.pipeline.BeginGeo(spatialType);
                SendCoordinates(spatialType, contentMembers);
                this.pipeline.EndGeo();
            }

            /// <summary>
            /// Translates the coordinates member value into method calls on the spatial pipeline.
            /// </summary>
            /// <param name="spatialType">SpatialType of the GeoJSON object.</param>
            /// <param name="contentMembers">Coordinates value of the GeoJSON object, or inner geometries for collection</param>
            private void SendCoordinates(SpatialType spatialType, IEnumerable contentMembers)
            {
                if (EnumerableAny(contentMembers))
                {
                    // non-empty shape
                    switch (spatialType)
                    {
                        case SpatialType.Point:
                            SendPoint(contentMembers);
                            break;
                        case SpatialType.LineString:
                            SendLineString(contentMembers);
                            break;
                        case SpatialType.Polygon:
                            SendPolygon(contentMembers);
                            break;
                        case SpatialType.MultiPoint:
                            SendMultiShape(SpatialType.Point, contentMembers);
                            break;
                        case SpatialType.MultiLineString:
                            SendMultiShape(SpatialType.LineString, contentMembers);
                            break;
                        case SpatialType.MultiPolygon:
                            SendMultiShape(SpatialType.Polygon, contentMembers);
                            break;
                        case SpatialType.Collection:
                            foreach (IDictionary<string, object> collectionMember in contentMembers)
                            {
                                SendToPipeline(collectionMember, false);
                            }

                            break;
                        default:
                            Debug.Assert(false, "SendCoordinates has not been implemented for SpatialType " + spatialType);
                            break;
                    }
                }
            }

            /// <summary>
            /// Translates the coordinates member value of a Point object into method calls on the spatial pipeline.
            /// </summary>
            /// <param name="coordinates">Parsed coordinates array.</param>
            private void SendPoint(IEnumerable coordinates)
            {
                SendPosition(coordinates, true);
                this.pipeline.EndFigure();
            }

            /// <summary>
            /// Translates the coordinates member value of a LineString object into method calls on the spatial pipeline.
            /// </summary>
            /// <param name="coordinates">Parsed coordinates array.</param>
            private void SendLineString(IEnumerable coordinates)
            {
                SendPositionArray(coordinates);
            }

            /// <summary>
            /// Translates the coordinates member value of a Polygon object into method calls on the spatial pipeline.
            /// </summary>
            /// <param name="coordinates">Parsed coordinates array.</param>
            private void SendPolygon(IEnumerable coordinates)
            {
                SendArrayOfArray(coordinates, (positionArray) => this.SendPositionArray(positionArray));
            }

            /// <summary>
            /// Translates the coordinates member value of a MultiPoint, MultiLineString, or MultiPolygon object into method calls on the spatial pipeline.
            /// </summary>
            /// <param name="containedSpatialType">Type of the shape contained in the Multi shape.</param>
            /// <param name="coordinates">Parsed coordinates array.</param>
            private void SendMultiShape(SpatialType containedSpatialType, IEnumerable coordinates)
            {
                Debug.Assert(
                    containedSpatialType == SpatialType.Point ||
                    containedSpatialType == SpatialType.LineString ||
                    containedSpatialType == SpatialType.Polygon,
                    "SendMultiShape only expects to write Point, LineString, or Polygon contained shapes.");

                SendArrayOfArray(coordinates, (containedShapeCoordinates) => this.SendShape(containedSpatialType, containedShapeCoordinates));
            }

            /// <summary>
            /// Translates an array of positions into method calls on the spatial pipeline.
            /// </summary>
            /// <param name="positionArray">List containing the positions.</param>
            private void SendPositionArray(IEnumerable positionArray)
            {
                bool first = true;

                SendArrayOfArray(
                    positionArray,
                    (array) =>
                    {
                        SendPosition(array, first);
                        if (first)
                        {
                            first = false;
                        }
                    });

                this.pipeline.EndFigure();
            }

            /// <summary>
            /// Translates an individual position into a method call on the spatial pipeline.
            /// </summary>
            /// <param name="positionElements">List containing elements of the position.</param>
            /// <param name="first">True if the position is the first one being written to a figure, otherwise false.</param>
            private void SendPosition(IEnumerable positionElements, bool first)
            {
                int count = 0;
                double x = 0.0;
                double y = 0.0;
                double? z = null;
                double? m = null;

                foreach (object element in positionElements)
                {
                    count++;

                    // If values can be read as integer, JsonReader will return them as that type, so we need to explicitly convert here.
                    switch (count)
                    {
                        case 1:
                            x = ValueAsDouble(element);
                            break;
                        case 2:
                            y = ValueAsDouble(element);
                            break;
                        case 3:
                            z = ValueAsNullableDouble(element);
                            break;
                        case 4:
                            m = ValueAsNullableDouble(element);
                            break;
                        default:
                            // will be an error below during range checking
                            break;
                    }
                }

                if (count < 2 || count > 4)
                {
                    throw new FormatException(Strings.GeoJsonReader_InvalidPosition);
                }

                if (first)
                {
                    this.pipeline.BeginFigure(x, y, z, m);
                }
                else
                {
                    this.pipeline.LineTo(x, y, z, m);
                }
            }
        }
    }
}
