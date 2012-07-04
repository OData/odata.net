//   Copyright 2011 Microsoft Corporation
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

namespace Microsoft.Data.Edm
{
    /// <summary>
    /// Enumerates the kinds of Edm Primitives.
    /// </summary>
    public enum EdmPrimitiveTypeKind
    {
        /// <summary>
        /// Represents a primitive type of unknown kind.
        /// </summary>
        None,

        /// <summary>
        /// Represents a Binary type.
        /// </summary>
        Binary,

        /// <summary>
        /// Represents a Boolean type.
        /// </summary>
        Boolean,

        /// <summary>
        /// Represents a Byte type.
        /// </summary>
        Byte,

        /// <summary>
        /// Represents a DateTime type.
        /// </summary>
        DateTime,

        /// <summary>
        /// Represents a DateTimeOffset type.
        /// </summary>
        DateTimeOffset,

        /// <summary>
        /// Represents a Decimal type.
        /// </summary>
        Decimal,

        /// <summary>
        /// Represents a Double type.
        /// </summary>
        Double,

        /// <summary>
        /// Represents a Guid type.
        /// </summary>
        Guid,

        /// <summary>
        /// Represents a Int16 type.
        /// </summary>
        Int16,

        /// <summary>
        /// Represents a Int32 type.
        /// </summary>
        Int32,

        /// <summary>
        /// Represents a Int64 type.
        /// </summary>
        Int64,

        /// <summary>
        /// Represents a SByte type.
        /// </summary>
        SByte,

        /// <summary>
        /// Represents a Single type.
        /// </summary>
        Single,

        /// <summary>
        /// Represents a String type.
        /// </summary>
        String,

        /// <summary>
        /// Represents a Stream type.
        /// </summary>
        Stream,

        /// <summary>
        /// Represents a Time type.
        /// </summary>
        Time,

        /// <summary>
        /// Represents an arbitrary Geography type.
        /// </summary>
        Geography,

        /// <summary>
        /// Represents a geography Point type.
        /// </summary>
        GeographyPoint,

        /// <summary>
        /// Represents a geography LineString type.
        /// </summary>
        GeographyLineString,

        /// <summary>
        /// Represents a geography Polygon type.
        /// </summary>
        GeographyPolygon,

        /// <summary>
        /// Represents a geography GeographyCollection type.
        /// </summary>
        GeographyCollection,

        /// <summary>
        /// Represents a geography MultiPolygon type.
        /// </summary>
        GeographyMultiPolygon,

        /// <summary>
        /// Represents a geography MultiLineString type.
        /// </summary>
        GeographyMultiLineString,

        /// <summary>
        /// Represents a geography MultiPoint type.
        /// </summary>
        GeographyMultiPoint,

        /// <summary>
        /// Represents an arbitrary Geometry type.
        /// </summary>
        Geometry,

        /// <summary>
        /// Represents a geometry Point type.
        /// </summary>
        GeometryPoint,

        /// <summary>
        /// Represents a geometry LineString type.
        /// </summary>
        GeometryLineString,

        /// <summary>
        /// Represents a geometry Polygon type.
        /// </summary>
        GeometryPolygon,

        /// <summary>
        /// Represents a geometry GeometryCollection type.
        /// </summary>
        GeometryCollection,

        /// <summary>
        /// Represents a geometry MultiPolygon type.
        /// </summary>
        GeometryMultiPolygon,

        /// <summary>
        /// Represents a geometry MultiLineString type.
        /// </summary>
        GeometryMultiLineString,

        /// <summary>
        /// Represents a geometry MultiPoint type.
        /// </summary>
        GeometryMultiPoint
    }

    /// <summary>
    /// Represents a definition of an EDM primitive type.
    /// </summary>
    public interface IEdmPrimitiveType : IEdmSchemaType
    {
        /// <summary>
        /// Gets the primitive kind of this type.
        /// </summary>
        EdmPrimitiveTypeKind PrimitiveKind { get; }
    }
}
