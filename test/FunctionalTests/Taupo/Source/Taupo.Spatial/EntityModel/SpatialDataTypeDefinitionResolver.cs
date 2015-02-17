//---------------------------------------------------------------------
// <copyright file="SpatialDataTypeDefinitionResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Spatial.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Spatial;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Spatial type definition resolver for the product spatial library
    /// </summary>
    ////[ImplementationName(typeof(ISpatialDataTypeDefinitionResolver), "Default")]
    public class SpatialDataTypeDefinitionResolver : ISpatialDataTypeDefinitionResolver
    {
        private static readonly DataType CoordinateSystemDataType = DataTypes.Spatial
            .Nullable()
            .WithProperties(
                new MemberProperty("Id", EdmDataTypes.Int32),
                new MemberProperty("Name", EdmDataTypes.String()))
            .WithPrimitiveClrType(typeof(Microsoft.Spatial.CoordinateSystem));

        private readonly Dictionary<SpatialShapeKind, SpatialDataType> shapeKindToEdmTypeMapping = new Dictionary<SpatialShapeKind, SpatialDataType>();

        /// <summary>
        /// Gets or sets the clr type resolver
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ISpatialClrTypeResolver ClrTypeResolver { get; set; }

        /// <summary>
        /// Resolves the full definition of the spatial type, potentially given only the EDM type name
        /// </summary>
        /// <param name="dataType">The basic definition which has at least the EDM type name</param>
        /// <returns>The fully defined spatial type with properties and methods</returns>
        public SpatialDataType ResolveFullDefinition(SpatialDataType dataType)
        {
            ExceptionUtilities.CheckArgumentNotNull(dataType, "dataType");

            var kind = this.GetKindBasedOnFacets(dataType);
            var returnType = this.GetDataTypeForKind(kind);

            foreach (var facet in dataType.Facets.Where(f => !(f is EdmTypeNameFacet) && !(f is SpatialShapeKindFacet) && !(f is EdmNamespaceFacet) && !(f is PrimitiveClrTypeFacet)))
            {
                returnType = returnType.WithFacet(facet);
            }

            return returnType;
        }

        /// <summary>
        /// Gets the edm type name from the spatial type based on facets.
        /// </summary>
        /// <param name="spatialType">Spatial type with factes.</param>
        /// <returns>Edm type name.</returns>
        private SpatialShapeKind GetKindBasedOnFacets(SpatialDataType spatialType)
        {
            // first we look for SpatialShapeKind, then EdmTypeName, and lastly UnqualifiedDatabaseTypeName
            // TODO: should SpatialShapeKind override EdmTypeName? 
            // seems reasonable if SSK: Point, ETN: Geography, but what about SSK: Point, ETN: LineString?
            var shape = spatialType.GetFacetValue<SpatialShapeKindFacet, SpatialShapeKind>(SpatialShapeKind.Unspecified);
            if (shape != SpatialShapeKind.Unspecified)
            {
                return shape;
            }

            var edmTypeName = spatialType.GetFacetValue<EdmTypeNameFacet, string>(string.Empty);
            if (!string.IsNullOrEmpty(edmTypeName))
            {
                return (SpatialShapeKind)Enum.Parse(typeof(SpatialShapeKind), edmTypeName, false);
            }

            var providerTypeName = spatialType.GetFacetValue<UnqualifiedDatabaseTypeNameFacet, string>(string.Empty).ToUpperInvariant();
            if (providerTypeName == "GEOMETRY")
            {
                return SpatialShapeKind.Geometry;
            }
            else
            {
                ExceptionUtilities.Assert(providerTypeName == "GEOGRAPHY", "Can't determine edm type given the provided facets.");
                return SpatialShapeKind.Geography;
            }
        }

        private SpatialDataType GetDataTypeForKind(SpatialShapeKind kind)
        {
            SpatialDataType dataType;
            if (!this.shapeKindToEdmTypeMapping.TryGetValue(kind, out dataType))
            {
                dataType = (SpatialDataType)typeof(EdmDataTypes).GetProperty(kind.ToString(), true, true).GetValue(null, null);

                // add the clr type facet immediately so that return/parameter types have it as well
                dataType = dataType.WithPrimitiveClrType(this.ClrTypeResolver.GetClrType(dataType));

                // add to dictionary before recursing
                this.shapeKindToEdmTypeMapping[kind] = dataType;

                // properties
                foreach (var property in this.GetMemberPropertiesForKind(kind).ToList())
                {
                    dataType.Properties.Add(property);
                }

                // methods
                foreach (var method in this.GetMethodsForKind(kind).ToList())
                {
                    dataType.Methods.Add(method);
                }
            }

            return dataType;
        }

        private IEnumerable<Function> GetMethodsForKind(SpatialShapeKind kind)
        {
            var parameterKind = kind.HasFacet(SpatialShapeFacets.Geography) ? SpatialShapeKind.Geography : SpatialShapeKind.Geometry;
            yield return new Function("geo", "Distance")
            {
                ReturnType = EdmDataTypes.Double,
                Parameters = { new FunctionParameter("other", this.GetDataTypeForKind(parameterKind)) },
                Annotations = { new MemberInSpatialTypeAnnotation(false) }
            };
        }

        private IEnumerable<MemberProperty> GetMemberPropertiesForKind(SpatialShapeKind kind)
        {
            yield return new MemberProperty("CoordinateSystem", CoordinateSystemDataType);
            yield return new MemberProperty("IsEmpty", EdmDataTypes.Boolean);

            bool isGeography = kind.HasFacet(SpatialShapeFacets.Geography);
            if (!kind.HasFacet(SpatialShapeFacets.Collection))
            {
                // Point
                if (kind.HasFacet(SpatialShapeFacets.Point))
                {
                    if (isGeography)
                    {
                        yield return new MemberProperty("Latitude", EdmDataTypes.Double.NotNullable());
                        yield return new MemberProperty("Longitude", EdmDataTypes.Double.NotNullable());
                    }
                    else
                    {
                        yield return new MemberProperty("X", EdmDataTypes.Double.NotNullable());
                        yield return new MemberProperty("Y", EdmDataTypes.Double.NotNullable());
                    }

                    yield return new MemberProperty("M", EdmDataTypes.Double.Nullable());
                    yield return new MemberProperty("Z", EdmDataTypes.Double.Nullable());
                }
                else if (kind.HasFacet(SpatialShapeFacets.LineString))
                {
                    var elementKind = isGeography ? SpatialShapeKind.GeographyPoint : SpatialShapeKind.GeometryPoint;
                    yield return new MemberProperty("Points", DataTypes.CollectionType.WithElementDataType(this.GetDataTypeForKind(elementKind)));
                }
                else if (kind.HasFacet(SpatialShapeFacets.Polygon))
                {
                    var elementKind = isGeography ? SpatialShapeKind.GeographyLineString : SpatialShapeKind.GeometryLineString;
                    yield return new MemberProperty("Rings", DataTypes.CollectionType.WithElementDataType(this.GetDataTypeForKind(elementKind)));
                }
            }
            else
            {
                if (kind.HasFacet(SpatialShapeFacets.Point))
                {
                    // MultiPoint
                    var elementKind = isGeography ? SpatialShapeKind.GeographyPoint : SpatialShapeKind.GeometryPoint;
                    yield return new MemberProperty("Points", DataTypes.CollectionType.WithElementDataType(this.GetDataTypeForKind(elementKind)));
                }
                else if (kind.HasFacet(SpatialShapeFacets.LineString))
                {
                    // MultiLineString
                    var elementKind = isGeography ? SpatialShapeKind.GeographyLineString : SpatialShapeKind.GeometryLineString;
                    yield return new MemberProperty("LineStrings", DataTypes.CollectionType.WithElementDataType(this.GetDataTypeForKind(elementKind)));
                }
                else if (kind.HasFacet(SpatialShapeFacets.Polygon))
                {
                    // MultiPolygon
                    var elementKind = isGeography ? SpatialShapeKind.GeographyPolygon : SpatialShapeKind.GeometryPolygon;
                    yield return new MemberProperty("Polygons", DataTypes.CollectionType.WithElementDataType(this.GetDataTypeForKind(elementKind)));
                }
                else
                {
                    var propertyName = isGeography ? "Geographies" : "Geometries";
                    var elementKind = isGeography ? SpatialShapeKind.Geography : SpatialShapeKind.Geometry;
                    yield return new MemberProperty(propertyName, DataTypes.CollectionType.WithElementDataType(this.GetDataTypeForKind(elementKind)));
                }
            }
        }
    }
}