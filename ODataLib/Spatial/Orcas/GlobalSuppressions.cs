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

using System.Diagnostics.CodeAnalysis;

[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.Data.Spatial")]

// Multi*
[module: SuppressMessage("Microsoft.Naming", "CA1702", Scope = "member", Target = "System.Spatial.SpatialType.MultiLineString")]
[module: SuppressMessage("Microsoft.Naming", "CA1702", Scope = "member", Target = "System.Spatial.SpatialType.MultiPoint")]
[module: SuppressMessage("Microsoft.Naming", "CA1702", Scope = "type", Target = "System.Spatial.GeographyMultiPoint")]
[module: SuppressMessage("Microsoft.Naming", "CA1702", Scope = "type", Target = "System.Spatial.GeographyMultiLineString")]
[module: SuppressMessage("Microsoft.Naming", "CA1702", Scope = "type", Target = "System.Spatial.GeographyMultiPolygon")]
[module: SuppressMessage("Microsoft.Naming", "CA1702", Scope = "type", Target = "System.Spatial.GeometryMultiPoint")]
[module: SuppressMessage("Microsoft.Naming", "CA1702", Scope = "type", Target = "System.Spatial.GeometryMultiLineString")]

[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "System.Spatial.GeometryFactory.#MultiPoint(System.Spatial.CoordinateSystem)", MessageId = "MultiPoint")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "System.Spatial.GeometryFactory.#MultiLineString(System.Spatial.CoordinateSystem)", MessageId = "MultiLine")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "System.Spatial.GeometryFactory.#MultiPoint()", MessageId = "MultiPoint")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "System.Spatial.GeometryFactory.#MultiLineString()", MessageId = "MultiLine")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "System.Spatial.GeometryFactory`1.#MultiPoint()", MessageId = "MultiPoint")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "System.Spatial.GeometryFactory`1.#MultiLineString()", MessageId = "MultiLine")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "System.Spatial.GeographyFactory`1.#MultiPoint()", MessageId = "MultiPoint")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "System.Spatial.GeographyFactory`1.#MultiLineString()", MessageId = "MultiLine")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "System.Spatial.GeographyFactory.#MultiPoint(System.Spatial.CoordinateSystem)", MessageId = "MultiPoint")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "System.Spatial.GeographyFactory.#MultiLineString(System.Spatial.CoordinateSystem)", MessageId = "MultiLine")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "System.Spatial.GeographyFactory.#MultiPoint()", MessageId = "MultiPoint")]
[module: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Scope = "member", Target = "System.Spatial.GeographyFactory.#MultiLineString()", MessageId = "MultiLine")]

// *Collection
[module: SuppressMessage("Microsoft.Naming", "CA1711", Scope = "type", Target = "System.Spatial.GeographyCollection")]
[module: SuppressMessage("Microsoft.Naming", "CA1711", Scope = "type", Target = "System.Spatial.GeometryCollection")]

// Readonly static
[module: SuppressMessage("Microsoft.Security", "CA2104", Scope = "member", Target = "System.Spatial.CoordinateSystem.DefaultGeography")]
[module: SuppressMessage("Microsoft.Security", "CA2104", Scope = "member", Target = "System.Spatial.CoordinateSystem.DefaultGeometry")]

// x -> latitude in parameter
[module: SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", Scope = "member", Target = "System.Spatial.GeographyFactory`1.#BeginFigure(System.Double,System.Double,System.Nullable`1<System.Double>,System.Nullable`1<System.Double>)", MessageId = "0#")]
[module: SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", Scope = "member", Target = "System.Spatial.GeographyFactory`1.#BeginFigure(System.Double,System.Double,System.Nullable`1<System.Double>,System.Nullable`1<System.Double>)", MessageId = "1#")]
[module: SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", Scope = "member", Target = "System.Spatial.GeographyFactory`1.#AddLine(System.Double,System.Double,System.Nullable`1<System.Double>,System.Nullable`1<System.Double>)", MessageId = "0#")]
[module: SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", Scope = "member", Target = "System.Spatial.GeographyFactory`1.#AddLine(System.Double,System.Double,System.Nullable`1<System.Double>,System.Nullable`1<System.Double>)", MessageId = "1#")]
