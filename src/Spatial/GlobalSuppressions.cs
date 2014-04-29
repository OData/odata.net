//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Diagnostics.CodeAnalysis;

[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.Data.Spatial")]

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

#region Task 1268242:Address CodeAnalysis suppressions that were added when moving to FxCop for SDL 6.0
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Scope = "member", Target = "Microsoft.Spatial.CoordinateSystem.#DefaultGeometry")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Scope = "member", Target = "Microsoft.Spatial.CoordinateSystem.#DefaultGeography")]
#endregion
