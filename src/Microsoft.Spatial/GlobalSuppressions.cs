//---------------------------------------------------------------------
// <copyright file="GlobalSuppressions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.Spatial")]

// Multi*
[module: SuppressMessage("Microsoft.Naming", "CA1702", Scope = "member", Target = "Microsoft.Spatial.SpatialType.MultiLineString")]
[module: SuppressMessage("Microsoft.Naming", "CA1702", Scope = "member", Target = "Microsoft.Spatial.SpatialType.MultiPoint")]
[module: SuppressMessage("Microsoft.Naming", "CA1702", Scope = "type", Target = "Microsoft.Spatial.GeographyMultiPoint")]
[module: SuppressMessage("Microsoft.Naming", "CA1702", Scope = "type", Target = "Microsoft.Spatial.GeographyMultiLineString")]
[module: SuppressMessage("Microsoft.Naming", "CA1702", Scope = "type", Target = "Microsoft.Spatial.GeographyMultiPolygon")]
[module: SuppressMessage("Microsoft.Naming", "CA1702", Scope = "type", Target = "Microsoft.Spatial.GeometryMultiPoint")]
[module: SuppressMessage("Microsoft.Naming", "CA1702", Scope = "type", Target = "Microsoft.Spatial.GeometryMultiLineString")]

[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "Microsoft.Spatial.GeometryFactory.#MultiPoint(Microsoft.Spatial.CoordinateSystem)", MessageId = "MultiPoint")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "Microsoft.Spatial.GeometryFactory.#MultiLineString(Microsoft.Spatial.CoordinateSystem)", MessageId = "MultiLine")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "Microsoft.Spatial.GeometryFactory.#MultiPoint()", MessageId = "MultiPoint")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "Microsoft.Spatial.GeometryFactory.#MultiLineString()", MessageId = "MultiLine")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "Microsoft.Spatial.GeometryFactory`1.#MultiPoint()", MessageId = "MultiPoint")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "Microsoft.Spatial.GeometryFactory`1.#MultiLineString()", MessageId = "MultiLine")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "Microsoft.Spatial.GeographyFactory`1.#MultiPoint()", MessageId = "MultiPoint")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "Microsoft.Spatial.GeographyFactory`1.#MultiLineString()", MessageId = "MultiLine")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "Microsoft.Spatial.GeographyFactory.#MultiPoint(Microsoft.Spatial.CoordinateSystem)", MessageId = "MultiPoint")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "Microsoft.Spatial.GeographyFactory.#MultiLineString(Microsoft.Spatial.CoordinateSystem)", MessageId = "MultiLine")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "Microsoft.Spatial.GeographyFactory.#MultiPoint()", MessageId = "MultiPoint")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "Microsoft.Spatial.GeographyFactory.#MultiLineString()", MessageId = "MultiLine")]

// *Collection
[module: SuppressMessage("Microsoft.Naming", "CA1711", Scope = "type", Target = "Microsoft.Spatial.GeographyCollection")]
[module: SuppressMessage("Microsoft.Naming", "CA1711", Scope = "type", Target = "Microsoft.Spatial.GeometryCollection")]

// x -> latitude in parameter
[module: SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", Scope = "member", Target = "Microsoft.Spatial.GeographyFactory`1.#BeginFigure(System.Double,System.Double,System.Nullable`1<System.Double>,System.Nullable`1<System.Double>)", MessageId = "0#")]
[module: SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", Scope = "member", Target = "Microsoft.Spatial.GeographyFactory`1.#BeginFigure(System.Double,System.Double,System.Nullable`1<System.Double>,System.Nullable`1<System.Double>)", MessageId = "1#")]
[module: SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", Scope = "member", Target = "Microsoft.Spatial.GeographyFactory`1.#AddLine(System.Double,System.Double,System.Nullable`1<System.Double>,System.Nullable`1<System.Double>)", MessageId = "0#")]
[module: SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", Scope = "member", Target = "Microsoft.Spatial.GeographyFactory`1.#AddLine(System.Double,System.Double,System.Nullable`1<System.Double>,System.Nullable`1<System.Double>)", MessageId = "1#")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Scope = "member", Target = "Microsoft.Spatial.CoordinateSystem.#DefaultGeometry", Justification = "DefaultGeometry object is not mutable externally.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Scope = "member", Target = "Microsoft.Spatial.CoordinateSystem.#DefaultGeography", Justification = "DefaultGeography object is not mutable externally.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Scope = "type", Target = "Microsoft.Spatial.GeographyMultiPoint")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Scope = "type", Target = "Microsoft.Spatial.GeographyMultiSurface")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Scope = "type", Target = "Microsoft.Spatial.GeometryMultiLineString")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Scope = "type", Target = "Microsoft.Spatial.GeometryMultiPolygon")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Scope = "member", Target = "Microsoft.Spatial.SpatialType.#MultiLineString")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Scope = "member", Target = "Microsoft.Spatial.SpatialType.#MultiPolygon")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Scope = "member", Target = "Microsoft.Spatial.SpatialType.#MultiPoint")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Scope = "type", Target = "Microsoft.Spatial.GeometryMultiSurface")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Scope = "type", Target = "Microsoft.Spatial.GeometryMultiPoint")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Scope = "type", Target = "Microsoft.Spatial.GeometryMultiCurve")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Scope = "type", Target = "Microsoft.Spatial.GeographyMultiPolygon")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Scope = "type", Target = "Microsoft.Spatial.GeographyMultiLineString")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Scope = "type", Target = "Microsoft.Spatial.GeographyMultiCurve")]
