//   OData .NET Libraries ver. 6.8.1
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
